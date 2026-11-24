using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // RF-11 + CU-08: el Administrador elige un reporte, el período y sus filtros, lo ve en la
    // grilla y lo exporta a PDF. RN-34 lo controla GestorReportes contra la sesión.
    public partial class frmReportes : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;

        private const string Todas = "Todas";
        private const string Todos = "Todos";

        private Reporte reporte;
        private bool cargando;
        private bool mostrarResultado;
        private string carpetaExportacion;
        private Componentes.EstadoVacio estadoSinReporte;
        private readonly Web.PanelWeb vistaWeb = new Web.PanelWeb
            { Name = "reporteWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoWeb = new Label
            { Name = "avisoReporteWeb", ForeColor = Tema.TextoSecundario, Visible = false };

        // El valor por omisión conserva la ruta del usuario; las pruebas pueden aislar la salida.
        internal string CarpetaExportacion
        {
            get { return carpetaExportacion ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A1.Reportes EVENTAURA"); }
            set { carpetaExportacion = value; }
        }

        public frmReportes()
        {
            InitializeComponent();
            Controls.Add(vistaWeb);
            Controls.Add(avisoWeb);
            vistaWeb.VistaLista += ActualizarVistaWeb;
            vistaWeb.VistaNoDisponible += ActualizarVistaWeb;
            vistaWeb.Escape += () =>
            {
                Message mensaje = default(Message);
                ProcessCmdKey(ref mensaje, Keys.Escape);
            };
            avisoWeb.Text = "Vista web no disponible. El reporte se muestra en la tabla.";
            SizeChanged += (s, e) => ActualizarVistaWeb();
            Shown += (s, e) =>
            {
                Disposicion.LimitarAncho(lblLectura);
                int pie = Math.Min(btnExportar.Top, btnVolver.Top);
                lblLectura.Height = Math.Min(lblLectura.Height,
                    Math.Max(0, pie - lblLectura.Top - Disposicion.Pixeles(this, Tema.Espacio.S)));
                foreach (Control selector in new Control[] { cmbReporte, cmbFiltro, dtpDesde, dtpHasta, dtpCorte })
                    Tema.EstiloSelector(selector);
                ColocarRotulo(lblReporte, cmbReporte);
                ColocarRotulo(lblFiltro, cmbFiltro);
                ColocarRotulo(lblDesde, dtpDesde);
                ColocarRotulo(lblHasta, dtpHasta);
                int espacio = Disposicion.Pixeles(this, Tema.Espacio.S);
                lblLectura.Top = pie - lblLectura.Height - espacio;
                dgvReporte.Height = Math.Max(1, lblLectura.Top - dgvReporte.Top - espacio);
                estadoSinReporte.AjustarA(dgvReporte);
                btnGenerar.PerformClick();
                vistaWeb.Cargar("datos.html");
            };
            estadoSinReporte = new Componentes.EstadoVacio { Name = "estadoSinReporte", Visible = false };
            estadoSinReporte.AjustarA(dgvReporte);
            estadoSinReporte.Cargar("Elegí un reporte y un período para empezar a revisar tus datos.", "Generar reporte");
            estadoSinReporte.Accion += () =>
            {
                if (reporte != null) dtpDesde.Value = dtpDesde.Value.AddYears(-1);
                btnGenerar.PerformClick();
            };
            Controls.Add(estadoSinReporte);
        }

        // Lo que muestran los combos de reporte y de evento.
        private sealed class Item
        {
            public string Texto;
            public object Valor;
            public override string ToString() { return Texto; }
        }

        private void frmReportes_Load(object sender, EventArgs e)
        {
            cargando = true;
            foreach (string codigo in GestorReportes.Codigos)
            {
                cmbReporte.Items.Add(new Item { Texto = $"{codigo} · {GestorReportes.Instance.TituloDe(codigo)}", Valor = codigo });
            }
            DateTime hoy = DateTime.Today;
            dtpHasta.Value = hoy;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-2);
            cmbReporte.SelectedIndex = 0;
            cargando = false;
            PrepararFiltros();

            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.AllowUserToDeleteRows = false;
            dgvReporte.AllowUserToResizeRows = false;
            dgvReporte.ReadOnly = true;
            dgvReporte.RowHeadersVisible = false;
            dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReporte.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvReporte.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            Tema.EstiloTablaDeConsulta(dgvReporte);
            dgvReporte.DefaultCellStyle.NullValue = "—";
            dgvReporte.EnableHeadersVisualStyles = false;
            dgvReporte.DataBindingComplete += dgvReporte_DataBindingComplete;
            MostrarEstadoSinReporte(true);
        }

        private string Codigo
        {
            get { return (string)((Item)cmbReporte.SelectedItem).Valor; }
        }

        // Cada reporte muestra solo los filtros que acepta.
        private void PrepararFiltros()
        {
            if (cargando)
            {
                return;
            }
            string codigo = Codigo;
            bool porCategoria = codigo == ReporteVentasPorEvento.CodigoReporte;
            bool porEvento = codigo == ReporteRankingPaquetes.CodigoReporte || codigo == ReporteDemandaInsatisfecha.CodigoReporte;
            bool porCorte = codigo == ReporteUsuariosPorNivel.CodigoReporte;

            cmbFiltro.Visible = porCategoria || porEvento;
            dtpCorte.Visible = porCorte;
            chkSoloAgotados.Visible = codigo == ReporteDemandaInsatisfecha.CodigoReporte;
            lblFiltro.Visible = porCategoria || porEvento || porCorte;
            lblFiltro.Text = porCategoria ? "Categoría" : porEvento ? "Evento" : "Corte";

            cmbFiltro.Items.Clear();
            if (porCategoria)
            {
                cmbFiltro.Items.Add(new Item { Texto = Todas, Valor = null });
                foreach (string categoria in GestorEventos.Categorias)
                {
                    cmbFiltro.Items.Add(new Item { Texto = categoria, Valor = categoria });
                }
                cmbFiltro.SelectedIndex = 0;
            }
            else if (porEvento)
            {
                cmbFiltro.Items.Add(new Item { Texto = Todos, Valor = null });
                try
                {
                    foreach (Evento evento in GestorReportes.Instance.EventosEntre(dtpDesde.Value, dtpHasta.Value))
                    {
                        cmbFiltro.Items.Add(new Item { Texto = $"{evento.Nombre} · {evento.Fecha:dd/MM/yyyy}", Valor = evento });
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Avisos.Regla("Reportes", ex);
                }
                cmbFiltro.SelectedIndex = 0;
            }
            if (porCorte)
            {
                dtpCorte.Value = dtpHasta.Value.Date;
            }
            LimpiarResultado();
        }

        private void cmbReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrepararFiltros();
        }

        private void dtpPeriodo_ValueChanged(object sender, EventArgs e)
        {
            PrepararFiltros();
        }

        private void LimpiarResultado()
        {
            reporte = null;
            dgvReporte.DataSource = null;
            lblLectura.Text = "";
            btnExportar.Enabled = false;
            MostrarEstadoSinReporte(true);
        }

        private void MostrarEstadoSinReporte(bool visible)
        {
            if (estadoSinReporte == null) return;
            estadoSinReporte.Visible = visible;
            mostrarResultado = !visible;
            ActualizarVistaWeb();
        }

        private void ActualizarVistaWeb()
        {
            if (estadoSinReporte == null) return;
            vistaWeb.Bounds = Rectangle.FromLTRB(dgvReporte.Left, dgvReporte.Top, dgvReporte.Right,
                Math.Max(dgvReporte.Bottom, lblLectura.Bottom));
            bool web = mostrarResultado && vistaWeb.Preparado;
            vistaWeb.Visible = web;
            dgvReporte.Visible = mostrarResultado && !web;
            lblLectura.Visible = mostrarResultado && !web;
            avisoWeb.Visible = vistaWeb.InicializacionTerminada && !vistaWeb.Preparado;
            avisoWeb.SetBounds(dgvReporte.Left, dgvReporte.Top - avisoWeb.Font.Height,
                dgvReporte.Width, avisoWeb.Font.Height);
            if (vistaWeb.Visible) vistaWeb.BringToFront();
            if (avisoWeb.Visible) avisoWeb.BringToFront();
        }

        // Crea el reporte con los filtros de la pantalla. null si GestorReportes lo rechaza.
        private Reporte ArmarReporte()
        {
            Reporte r;
            try
            {
                r = GestorReportes.Instance.Crear(Codigo, dtpDesde.Value, dtpHasta.Value);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("Reportes", ex);
                return null;
            }
            catch (ArgumentException ex)
            {
                Avisos.Informativo(this, ex.Message);
                return null;
            }

            object valor = cmbFiltro.Visible ? ((Item)cmbFiltro.SelectedItem).Valor : null;
            Evento evento = valor as Evento;
            var ventas = r as ReporteVentasPorEvento;
            if (ventas != null)
            {
                ventas.Categoria = valor as string;
            }
            var paquetes = r as ReporteRankingPaquetes;
            if (paquetes != null && evento != null)
            {
                paquetes.IdEvento = evento.Id;
                paquetes.NombreEvento = evento.Nombre;
            }
            var niveles = r as ReporteUsuariosPorNivel;
            if (niveles != null)
            {
                niveles.FechaCorte = dtpCorte.Value.Date;
            }
            var demanda = r as ReporteDemandaInsatisfecha;
            if (demanda != null)
            {
                if (evento != null)
                {
                    demanda.IdEvento = evento.Id;
                    demanda.NombreEvento = evento.Nombre;
                }
                demanda.SoloAgotados = chkSoloAgotados.Checked;
            }
            return r;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Reporte r = ArmarReporte();
            if (r == null)
            {
                return;
            }
            DataTable datos;
            try
            {
                using (Avisos.Esperar(this, btnGenerar))
                    datos = GestorReportes.Instance.Ejecutar(r);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("Reportes", ex);
                return;
            }
            reporte = r;
            dgvReporte.DataSource = datos;
            if (datos.Rows.Count == 0)
            {
                estadoSinReporte.Cargar("No encontramos datos para este período y estos filtros. Probá ampliar el período.", "Ampliar período");
                MostrarEstadoSinReporte(true);
                btnExportar.Enabled = false;
                return;
            }
            MostrarEstadoSinReporte(false);
            FormatearColumnas(datos);
            Tema.EstiloTablaDeConsulta(dgvReporte);
            lblLectura.Text = "Lectura. " + r.Lectura(datos);
            vistaWeb.Actualizar(new { reporte = Web.DatosWebReportes.Crear(r, datos), lectura = lblLectura.Text });
            ActualizarVistaWeb();
            btnExportar.Enabled = true;
        }

        // Anexo A02: Mis Documentos\A1.Reportes EVENTAURA, EVAU_[código]_[inicio]_[fin].pdf.
        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (reporte == null)
            {
                return;
            }
            string carpeta = CarpetaExportacion;
            string ruta = Path.Combine(carpeta, reporte.NombreArchivo + ".pdf");
            try
            {
                using (Avisos.Esperar(this, btnExportar))
                    ruta = GestorReportes.Instance.Exportar(reporte, new ExportadorPDF(), ruta);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("Reportes", ex);
                return;
            }
            catch (IOException)
            {
                Avisos.Informativo(this, "No pudimos guardar el PDF. Cerrá el archivo si está abierto y volvé a exportarlo.");
                return;
            }
            Avisos.Informativo(this, "El reporte se guardó en tu carpeta de documentos. Abrilo desde el Explorador de archivos.");
        }

        // Los formatos los declara el reporte en la tabla: la pantalla no conoce ninguno.
        private void FormatearColumnas(DataTable datos)
        {
            foreach (DataGridViewColumn columna in dgvReporte.Columns)
            {
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;   // el orden es el del reporte
                DataColumn origen = datos.Columns[columna.DataPropertyName];
                string formato = origen.ExtendedProperties[Reporte.PropiedadFormato] as string;
                if (formato != null)
                {
                    columna.DefaultCellStyle.Format = formato == "C0" ? "C2" : formato;
                    columna.DefaultCellStyle.FormatProvider = Reporte.Cultura;
                }
                if (formato != null || origen.ExtendedProperties.ContainsKey(Reporte.PropiedadDerecha))
                {
                    columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    columna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        // La fila de totales, en negrita y resaltada como en el PDF.
        private void dgvReporte_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataTable datos = dgvReporte.DataSource as DataTable;
            if (datos == null || dgvReporte.Rows.Count == 0 || !datos.ExtendedProperties.ContainsKey(Reporte.PropiedadFilaTotales))
            {
                return;
            }
            DataGridViewRow totales = dgvReporte.Rows[dgvReporte.Rows.Count - 1];
            Tema.FilaTotal(totales);
            dgvReporte.ClearSelection();
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Form frm;
            if (GestorSesion.Instance.UsuarioActual != null && GestorSesion.Instance.UsuarioActual.TipoUsuario == "Administrador")
                frm = new frmMenuAdministrador { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            else
                frm = new frmMenuPropietario { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            frm.Show(); this.Hide();
        }
    }
}
