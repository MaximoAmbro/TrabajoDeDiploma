using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public sealed partial class frmAuditoria : FormularioEventAura
    {
        private Componentes.EstadoVacio estadoVacio;
        private readonly Componentes.EtiquetaTitulo titulo = new Componentes.EtiquetaTitulo { Text = "Registro de auditoría", AutoSize = true };
        private readonly Componentes.EtiquetaRotulo etiquetaDesde = new Componentes.EtiquetaRotulo { Text = "Desde", AutoSize = true };
        private readonly Componentes.EtiquetaRotulo etiquetaHasta = new Componentes.EtiquetaRotulo { Text = "Hasta", AutoSize = true };
        private readonly Componentes.EtiquetaRotulo etiquetaAccion = new Componentes.EtiquetaRotulo { Text = "Acción", AutoSize = true };
        private bool vistaLista;
        public frmAuditoria()
        {
            InitializeComponent();
            Controls.AddRange(new Control[] { titulo, etiquetaDesde, etiquetaHasta, etiquetaAccion });
            desde.Value = DateTime.Today.AddDays(-30);
            hasta.Value = DateTime.Today;
            accion.Items.AddRange(new object[] { "Todas", "Inicio de sesión", "Cambio de contraseña", "Compra iniciada", "Compra confirmada", "Devolución", "Control de ingreso", "Gestión de eventos: crear", "Gestión de eventos: publicar", "Gestión de eventos: eliminar", "Gestión de eventos: suspender", "Gestión de eventos: preventa", "Gestión de eventos: sector" });
            accion.SelectedIndex = 0;
            buscar.Click += (s, e) => Cargar();
            Shown += (s, e) =>
            {
                Tema.EstiloSelector(desde);
                Tema.EstiloSelector(hasta);
                Tema.EstiloSelector(accion);
                vistaLista = true;
                AjustarGeometria();
                Cargar();
            };
            Resize += (s, e) => AjustarGeometria();
            tabla.SizeChanged += (s, e) => AjustarAnchosColumnas();
            estadoVacio = new Componentes.EstadoVacio { Name = "estadoSinAuditoria", Visible = false };
            estadoVacio.AjustarA(tabla);
            estadoVacio.Cargar("No encontramos movimientos con estos filtros. Probá ampliar el período.", "Ampliar período");
            estadoVacio.Accion += () => { desde.Value = DateTime.Today.AddYears(-1); accion.SelectedIndex = 0; Cargar(); };
            Controls.Add(estadoVacio);
        }

        private void Cargar()
        {
            var filas = GestorAuditoria.Instance.Consultar(desde.Value.Date, hasta.Value.Date.AddDays(1), accion.SelectedIndex <= 0 ? null : Convert.ToString(accion.SelectedItem));
            foreach (var fila in filas)
                fila.Fecha = DateTime.SpecifyKind(fila.Fecha, DateTimeKind.Utc).ToLocalTime();
            tabla.DataSource = filas;
            Tema.EstiloTablaDeConsulta(tabla);
            FormatearColumnas();
            tabla.Visible = filas.Count > 0;
            estadoVacio.Visible = filas.Count == 0;
        }

        private void FormatearColumnas()
        {
            var nombres = new System.Collections.Generic.Dictionary<string, string>
            {
                { "UsuarioMail", "Correo" }, { "Accion", "Acción" }, { "Entidad", "Qué se modificó" },
                { "Fecha", "Fecha y hora" }, { "Resultado", "Resultado" }
            };
            var minimos = new System.Collections.Generic.Dictionary<string, int>
            {
                { "UsuarioMail", 126 }, { "Accion", 118 }, { "Entidad", 120 }, { "Fecha", 108 }, { "Resultado", 112 }
            };
            foreach (DataGridViewColumn columna in tabla.Columns)
            {
                if (nombres.ContainsKey(columna.DataPropertyName)) columna.HeaderText = nombres[columna.DataPropertyName];
                if (minimos.ContainsKey(columna.DataPropertyName)) columna.MinimumWidth = minimos[columna.DataPropertyName];
                if (columna.DataPropertyName == "Id" || columna.DataPropertyName == "UsuarioId" || columna.DataPropertyName == "EntidadId")
                    columna.Visible = false;
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
                columna.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            tabla.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            tabla.ScrollBars = ScrollBars.Vertical;
            tabla.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            if (tabla.Columns.Contains("Fecha")) tabla.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            AjustarAnchosColumnas();
        }

        private void AjustarAnchosColumnas()
        {
            if (tabla.Columns.Count == 0) return;
            int anchoDisponible = tabla.ClientSize.Width - tabla.RowHeadersWidth - SystemInformation.VerticalScrollBarWidth - 4;
            if (anchoDisponible <= 0) return;
            var visibles = tabla.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToArray();
            int[] pesos = { 28, 24, 12, 20, 16 };
            int sumaPesos = 100;
            int asignado = 0;
            for (int i = 0; i < visibles.Length; i++)
            {
                int ancho = i == visibles.Length - 1
                    ? anchoDisponible - asignado
                    : (int)Math.Floor(anchoDisponible * (double)pesos[Math.Min(i, pesos.Length - 1)] / sumaPesos);
                visibles[i].Width = Math.Max(visibles[i].MinimumWidth, ancho);
                asignado += visibles[i].Width;
            }
        }

        private void AjustarGeometria()
        {
            if (!vistaLista) return;
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int margen = Px(Tema.Espacio.XXL);
            int separacion = Px(Tema.Espacio.L);
            int anchoFecha = Px(160);
            ColocarTitulo(titulo);
            int arriba = titulo.Bottom + separacion + etiquetaDesde.Height + Px(Tema.Espacio.XS);
            desde.SetBounds(margen, arriba, anchoFecha, Px(Tema.AltoCampo));
            hasta.SetBounds(desde.Right + separacion, arriba, anchoFecha, Px(Tema.AltoCampo));
            buscar.Location = new Point(ClientSize.Width - buscar.Width - margen, arriba);
            int izquierda = hasta.Right + separacion;
            accion.SetBounds(izquierda, arriba, Math.Max(1, buscar.Left - izquierda - separacion), Px(Tema.AltoCampo));
            ColocarRotulo(etiquetaDesde, desde);
            ColocarRotulo(etiquetaHasta, hasta);
            ColocarRotulo(etiquetaAccion, accion);
            int inicio = buscar.Bottom + Px(Tema.Espacio.XL);
            tabla.SetBounds(margen, inicio, Math.Max(1, ClientSize.Width - 2 * margen),
                Math.Max(1, ClientSize.Height - inicio - margen));
            estadoVacio?.AjustarA(tabla);
            AjustarAnchosColumnas();
        }
    }
}
