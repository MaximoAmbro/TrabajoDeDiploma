using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // CU-03: historial de compras del cliente, una fila por compra. Al elegir una compra se ve el
    // detalle de sus entradas y desde ahí se devuelven las que tienen seguro (RN-29).
    public partial class frmHistorialCliente : Form
    {
        public int IDUsuario;
        public string NombreUsuario { get; set; }
        public string Mail { get; set; }

        // RN-33: importes en pesos argentinos.
        private static readonly CultureInfo Pesos = new CultureInfo("es-AR");
        private static readonly Color Violeta = Color.FromArgb(116, 86, 174);

        private bool cargando;

        public frmHistorialCliente()
        {
            InitializeComponent();
        }

        private void frmHistorialCliente_Load(object sender, EventArgs e)
        {
            FormatearGrilla(dgvCompras);
            dgvCompras.MultiSelect = false;
            dgvCompras.ReadOnly = true;
            dgvCompras.Columns.Add("Pedido", "Pedido");
            dgvCompras.Columns.Add("Evento", "Evento");
            dgvCompras.Columns.Add("Fecha", "Fecha");
            dgvCompras.Columns.Add("Estado", "Estado");
            dgvCompras.Columns.Add("Entradas", "Entradas");
            dgvCompras.Columns.Add("Total", "Total pagado");
            dgvCompras.Columns.Add("Devuelto", "Devuelto");
            dgvCompras.Columns["Pedido"].FillWeight = 80;
            dgvCompras.Columns["Evento"].FillWeight = 150;
            dgvCompras.Columns["Fecha"].FillWeight = 85;
            dgvCompras.Columns["Estado"].FillWeight = 95;
            dgvCompras.Columns["Entradas"].FillWeight = 65;
            AlinearDerecha(dgvCompras, "Entradas", "Total", "Devuelto");
            FormatoPesos(dgvCompras, "Total", "Devuelto");
            dgvCompras.SelectionChanged += dgvCompras_SelectionChanged;

            FormatearGrilla(dgvEntradas);
            dgvEntradas.MultiSelect = false;
            dgvEntradas.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvEntradas.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Devolver", HeaderText = "Devolver", FillWeight = 60 });
            dgvEntradas.Columns.Add("Sector", "Sector");
            dgvEntradas.Columns.Add("Precio", "Precio");
            dgvEntradas.Columns.Add("Estado", "Estado");
            dgvEntradas.Columns.Add("Seguro", "Seguro");
            dgvEntradas.Columns.Add("Codigo", "Código");
            dgvEntradas.Columns["Codigo"].FillWeight = 180;
            foreach (DataGridViewColumn columna in dgvEntradas.Columns)
            {
                columna.ReadOnly = columna.Name != "Devolver";
            }
            AlinearDerecha(dgvEntradas, "Precio");
            FormatoPesos(dgvEntradas, "Precio");
            dgvEntradas.CurrentCellDirtyStateChanged += dgvEntradas_CurrentCellDirtyStateChanged;
            dgvEntradas.CellValueChanged += dgvEntradas_CellValueChanged;

            int horas = GestorParametros.Instance.ObtenerEntero("SEGURO_HORAS_COBERTURA");
            lblAyuda.Text = $"Con seguro, podés devolver entradas hasta {horas} horas antes del evento. " +
                            "No se reintegran el cargo por servicio ni el seguro.";
            CargarCompras(null);
        }

        // Pendiente y Vencida no aparecen: GestorCompras.HistorialDe solo trae Confirmada y Reembolsada.
        private void CargarCompras(int? idCompraElegida)
        {
            cargando = true;
            dgvCompras.Rows.Clear();
            foreach (Compra compra in GestorCompras.Instance.HistorialDe(IDUsuario))
            {
                int fila = dgvCompras.Rows.Add(
                    frmConfirmacion.NumeroPedido(compra),
                    compra.NombreEvento ?? "—",
                    compra.FechaCompra.ToString("dd/MM/yyyy"),
                    compra.Estado.ToString(),
                    compra.CantidadEntradas,
                    compra.TotalPagado,
                    compra.MontoReembolsado);
                dgvCompras.Rows[fila].Tag = compra;
            }
            dgvCompras.ClearSelection();
            cargando = false;

            if (dgvCompras.Rows.Count == 0)
            {
                lblDetalle.Text = "Todavía no tenés compras.";
                dgvEntradas.Rows.Clear();
                ActualizarBotonDevolver();
                return;
            }
            DataGridViewRow elegida = dgvCompras.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(f => ((Compra)f.Tag).Id == idCompraElegida) ?? dgvCompras.Rows[0];
            elegida.Selected = true;
            dgvCompras.CurrentCell = elegida.Cells["Pedido"];
            CargarEntradas((Compra)elegida.Tag);
        }

        private void dgvCompras_SelectionChanged(object sender, EventArgs e)
        {
            if (cargando || dgvCompras.SelectedRows.Count == 0)
            {
                return;
            }
            CargarEntradas((Compra)dgvCompras.SelectedRows[0].Tag);
        }

        // Detalle de la compra. Solo se puede tildar una entrada Confirmada con póliza: las demás
        // no admiten devolución (RN-29). El plazo de 48 h lo controla GestorCompras al devolver.
        private void CargarEntradas(Compra compra)
        {
            dgvEntradas.Rows.Clear();
            List<Entrada> entradas = GestorCompras.Instance.EntradasDe(compra);
            lblDetalle.Text = $"Entradas del pedido {frmConfirmacion.NumeroPedido(compra)}";
            if (entradas.Count == 0)
            {
                lblDetalle.Text += " (sin detalle de entradas)";
                ActualizarBotonDevolver();
                return;
            }

            var aseguradas = new HashSet<int>(GestorCompras.Instance.SegurosDe(compra).Select(s => s.IdEntrada));
            Dictionary<int, string> sectores = GestorEventos.Instance.Sectores(entradas[0].IdEvento)
                .ToDictionary(s => s.Id, s => s.Nombre);

            foreach (Entrada entrada in entradas)
            {
                bool conSeguro = aseguradas.Contains(entrada.Id);
                string sector;
                if (!entrada.IdSector.HasValue || !sectores.TryGetValue(entrada.IdSector.Value, out sector))
                {
                    sector = "—";
                }
                int fila = dgvEntradas.Rows.Add(false, sector, entrada.Precio, entrada.Estado.ToString(),
                    conSeguro ? "Sí" : "No", entrada.CodigoQR);
                DataGridViewRow row = dgvEntradas.Rows[fila];
                row.Tag = entrada;
                if (entrada.Estado != EstadoEntrada.Confirmada || !conSeguro)
                {
                    row.Cells["Devolver"].ReadOnly = true;
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
            dgvEntradas.ClearSelection();
            ActualizarBotonDevolver();
        }

        // El tilde se confirma en el acto, así CellValueChanged habilita el botón sin salir de la celda.
        private void dgvEntradas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvEntradas.IsCurrentCellDirty)
            {
                dgvEntradas.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvEntradas_CellValueChanged(object sender, EventArgs e)
        {
            ActualizarBotonDevolver();
        }

        private List<Entrada> EntradasElegidas()
        {
            return dgvEntradas.Rows.Cast<DataGridViewRow>()
                .Where(f => Convert.ToBoolean(f.Cells["Devolver"].Value))
                .Select(f => (Entrada)f.Tag)
                .ToList();
        }

        private void ActualizarBotonDevolver()
        {
            int cantidad = EntradasElegidas().Count;
            btnDevolver.Enabled = cantidad > 0;
            btnDevolver.Text = cantidad > 0 ? $"Devolver {cantidad} entrada(s)" : "Devolver";
        }

        // RN-29: se devuelven entradas y servicios; no el cargo por servicio ni el seguro.
        private void btnDevolver_Click(object sender, EventArgs e)
        {
            List<Entrada> elegidas = EntradasElegidas();
            if (elegidas.Count == 0)
            {
                return;
            }
            DialogResult respuesta = MessageBox.Show(
                $"¿Devolver {elegidas.Count} entrada(s)?\n\n" +
                "Se reintegra el precio de cada entrada con su descuento. Si devolvés todas las que " +
                "quedan, también los servicios. El cargo por servicio y el seguro no se reintegran.\n\n" +
                "La devolución no se puede deshacer.",
                "Devolver entradas", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            int idCompra = elegidas[0].IdCompra.Value;
            decimal monto;
            try
            {
                monto = GestorCompras.Instance.Reembolsar(elegidas);
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Devolución", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarCompras(idCompra);
                return;
            }

            MessageBox.Show($"Devolución registrada. Se reintegran {monto.ToString("C2", Pesos)}.",
                "Devolución", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarCompras(idCompra);
        }

        // --------------------------------------        FUNCIONES          --------------------------------------

        private static void FormatearGrilla(DataGridView grilla)
        {
            grilla.Columns.Clear();
            grilla.AllowUserToAddRows = false;
            grilla.AllowUserToDeleteRows = false;
            grilla.AllowUserToResizeRows = false;
            grilla.RowHeadersVisible = false;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grilla.BackgroundColor = Color.White;
            grilla.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            grilla.DefaultCellStyle.SelectionBackColor = Violeta;
            grilla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private static void AlinearDerecha(DataGridView grilla, params string[] columnas)
        {
            foreach (string nombre in columnas)
            {
                grilla.Columns[nombre].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private static void FormatoPesos(DataGridView grilla, params string[] columnas)
        {
            foreach (string nombre in columnas)
            {
                grilla.Columns[nombre].DefaultCellStyle.Format = "C2";
                grilla.Columns[nombre].DefaultCellStyle.FormatProvider = Pesos;
            }
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuCliente frm = new frmMenuCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
    }
}
