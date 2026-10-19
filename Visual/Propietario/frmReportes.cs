using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    public partial class frmReportes : Form
    {
        public int IDUsuario;
        public string NombreUsuario;

        private const string Todas = "Todas";
        private const string Todos = "Todos";

        private Reporte reporte;
        private bool cargando;

        public frmReportes()
        {
            InitializeComponent();
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
            dgvReporte.BackgroundColor = Color.White;
            dgvReporte.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvReporte.DefaultCellStyle.SelectionBackColor = Color.FromArgb(116, 86, 174);
            dgvReporte.DefaultCellStyle.NullValue = "—";
            dgvReporte.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 231, 231);
            dgvReporte.EnableHeadersVisualStyles = false;
            dgvReporte.DataBindingComplete += dgvReporte_DataBindingComplete;
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
                    MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                datos = GestorReportes.Instance.Ejecutar(r);
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            reporte = r;
            dgvReporte.DataSource = datos;
            FormatearColumnas(datos);
            lblLectura.Text = "Lectura. " + r.Lectura(datos);
            btnExportar.Enabled = true;
        }

        // Anexo A02: Mis Documentos\A1.Reportes EVENTAURA, EVAU_[código]_[inicio]_[fin].pdf.
        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (reporte == null)
            {
                return;
            }
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A1.Reportes EVENTAURA");
            string ruta = Path.Combine(carpeta, reporte.NombreArchivo + ".pdf");
            try
            {
                ruta = GestorReportes.Instance.Exportar(reporte, new ExportadorPDF(), ruta);
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (IOException ex)
            {
                MessageBox.Show("No se pudo escribir el archivo. ¿Está abierto en otro programa?\n\n" + ex.Message,
                    "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult abrir = MessageBox.Show($"Reporte exportado:\n{ruta}\n\n¿Abrirlo ahora?",
                "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (abrir == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
            }
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
                    columna.DefaultCellStyle.Format = formato;
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
            totales.DefaultCellStyle.Font = new Font(dgvReporte.Font, FontStyle.Bold);
            totales.DefaultCellStyle.BackColor = Color.FromArgb(236, 231, 246);
            dgvReporte.ClearSelection();
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
    }
}
