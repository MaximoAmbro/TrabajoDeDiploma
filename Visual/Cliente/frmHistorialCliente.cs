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
    public partial class frmHistorialCliente : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario { get; set; }
        public string Mail { get; set; }

        // RN-33: importes en pesos argentinos.
        private static readonly CultureInfo Pesos = new CultureInfo("es-AR");

        private bool cargando;
        private Componentes.EstadoVacio estadoSinCompras;
        private readonly Web.PanelWeb historialWeb = new Web.PanelWeb { Name = "historialWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoWeb = new Label { Name = "avisoHistorialWeb", AutoSize = true, ForeColor = Tema.TextoSecundario, Text = "Vista web no disponible. Se muestra la lista de Windows Forms.", Visible = false };

        public frmHistorialCliente()
        {
            InitializeComponent();
            estadoSinCompras = new Componentes.EstadoVacio { Name = "estadoSinCompras", Visible = false };
            estadoSinCompras.AjustarA(new Rectangle(24, 148, 852, 284));
            estadoSinCompras.Cargar("Todavía no compraste entradas. Cuando lo hagas, van a aparecer acá.", "Ver eventos");
            estadoSinCompras.Accion += AbrirEventos;
            Controls.Add(estadoSinCompras);
            Controls.Add(historialWeb);
            Controls.Add(avisoWeb);
            historialWeb.VistaLista += ActualizarVistaHistorial;
            historialWeb.VistaNoDisponible += ActualizarVistaHistorial;
            historialWeb.Escape += () =>
            {
                Message mensaje = default(Message);
                ProcessCmdKey(ref mensaje, Keys.Escape);
            };
            historialWeb.Mensaje += RecibirMensajeHistorial;
            Shown += (s, e) =>
            {
                btnDevolver.Left = ClientSize.Width - btnDevolver.Width - Disposicion.Pixeles(this, Tema.Espacio.XXL);
                Disposicion.LimitarAncho(lblAyuda);
                Disposicion.LimitarAncho(lblDetalle);
                lblAyuda.Height = Math.Max(1, btnVolver.Top - lblAyuda.Top - Disposicion.Pixeles(this, Tema.Espacio.S));
                Disposicion.ExtenderGrilla(dgvCompras, lblDetalle);
                dgvEntradas.Top = lblDetalle.Bottom + Disposicion.Pixeles(this, Tema.Espacio.XS);
                Disposicion.ExtenderGrilla(dgvEntradas, lblAyuda);
                estadoSinCompras.AjustarA(new Rectangle(dgvCompras.Left, dgvCompras.Top,
                    dgvCompras.Width, btnVolver.Top - dgvCompras.Top));
                AjustarHistorialWeb();
                historialWeb.Cargar("gestion.html");
            };
            SizeChanged += (s, e) => AjustarHistorialWeb();
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
            dgvEntradas.Columns["Codigo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvEntradas.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvEntradas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
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
                dgvCompras.Visible = dgvEntradas.Visible = lblDetalle.Visible = lblAyuda.Visible = btnDevolver.Visible = false;
                estadoSinCompras.Visible = true;
                ActualizarBotonDevolver();
                ActualizarModeloHistorialWeb();
                return;
            }
            estadoSinCompras.Visible = false;
            dgvCompras.Visible = dgvEntradas.Visible = lblDetalle.Visible = lblAyuda.Visible = btnDevolver.Visible = true;
            DataGridViewRow elegida = dgvCompras.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(f => ((Compra)f.Tag).Id == idCompraElegida) ?? dgvCompras.Rows[0];
            elegida.Selected = true;
            dgvCompras.CurrentCell = elegida.Cells["Pedido"];
            CargarEntradas((Compra)elegida.Tag);
            ActualizarModeloHistorialWeb();
        }

        private void AbrirEventos()
        {
            var eventos = new frmEventosCliente { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            if (NavegacionCliente.AbrirDesdeMenu(eventos)) return;
            eventos.Show(); Hide();
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
                    Tema.FilaNoReembolsable(row);
                }
            }
            dgvEntradas.ClearSelection();
            ActualizarBotonDevolver();
            ActualizarModeloHistorialWeb();
        }

        private void AjustarHistorialWeb()
        {
            if (historialWeb == null || IsDisposed) return;
            int abajo = lblAyuda.Top;
            historialWeb.SetBounds(dgvCompras.Left, dgvCompras.Top,
                Math.Max(1, ClientSize.Width - dgvCompras.Left * 2), Math.Max(1, abajo - dgvCompras.Top));
            avisoWeb.Location = new Point(historialWeb.Left, Math.Max(0, historialWeb.Top - avisoWeb.Height));
        }

        private void ActualizarVistaHistorial()
        {
            bool web = historialWeb.Preparado;
            label1.Text = "Mis entradas";
            label1.Visible = !NavegacionCliente.Activa;
            historialWeb.Visible = web;
            avisoWeb.Visible = historialWeb.InicializacionTerminada && !web;
            dgvCompras.Visible = !web && dgvCompras.Rows.Count > 0;
            dgvEntradas.Visible = !web && dgvCompras.Rows.Count > 0;
            lblDetalle.Visible = !web && dgvCompras.Rows.Count > 0;
            lblAyuda.Visible = !web && dgvCompras.Rows.Count > 0;
            estadoSinCompras.Visible = !web && dgvCompras.Rows.Count == 0;
            if (web) historialWeb.BringToFront();
            else avisoWeb.BringToFront();
        }

        private void ActualizarModeloHistorialWeb()
        {
            var compras = dgvCompras.Rows.Cast<DataGridViewRow>().Select(r =>
            {
                var c = (Compra)r.Tag;
                return new { id = c.Id, pedido = Convert.ToString(r.Cells["Pedido"].Value), evento = Convert.ToString(r.Cells["Evento"].Value),
                    fecha = Convert.ToString(r.Cells["Fecha"].Value), estado = Convert.ToString(r.Cells["Estado"].Value),
                    entradas = Convert.ToString(r.Cells["Entradas"].Value), total = Convert.ToDecimal(r.Cells["Total"].Value).ToString("C2", Pesos),
                    devuelto = Convert.ToDecimal(r.Cells["Devuelto"].Value).ToString("C2", Pesos) };
            }).ToArray();
            int? seleccionada = dgvCompras.SelectedRows.Count == 0 ? (int?)null : ((Compra)dgvCompras.SelectedRows[0].Tag).Id;
            var entradas = dgvEntradas.Rows.Cast<DataGridViewRow>().Select(r => new
            {
                id = ((Entrada)r.Tag).Id,
                sector = Convert.ToString(r.Cells["Sector"].Value),
                precio = Convert.ToDecimal(r.Cells["Precio"].Value).ToString("C2", Pesos),
                estado = Convert.ToString(r.Cells["Estado"].Value), seguro = Convert.ToString(r.Cells["Seguro"].Value),
                codigo = Convert.ToString(r.Cells["Codigo"].Value), puedeDevolver = !r.Cells["Devolver"].ReadOnly,
                devolver = Convert.ToBoolean(r.Cells["Devolver"].Value)
            }).ToArray();
            historialWeb.Actualizar(new { vista = "historial", compras, seleccionada, detalle = lblDetalle.Text, entradas });
        }

        private void RecibirMensajeHistorial(string texto)
        {
            var tipo = System.Text.RegularExpressions.Regex.Match(texto ?? "", "\\\"tipo\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"");
            if (!tipo.Success) return;
            if (tipo.Groups[1].Value == "eventos") { AbrirEventos(); return; }
            var idMatch = System.Text.RegularExpressions.Regex.Match(texto ?? "", "\\\"id\\\"\\s*:\\s*(\\d+)");
            if (!idMatch.Success || !int.TryParse(idMatch.Groups[1].Value, out int id)) return;
            if (tipo.Groups[1].Value == "compra")
            {
                var fila = dgvCompras.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => ((Compra)r.Tag).Id == id);
                if (fila != null) { fila.Selected = true; dgvCompras.CurrentCell = fila.Cells["Pedido"]; CargarEntradas((Compra)fila.Tag); }
            }
            else if (tipo.Groups[1].Value == "entrada")
            {
                var fila = dgvEntradas.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => ((Entrada)r.Tag).Id == id);
                if (fila != null && !fila.Cells["Devolver"].ReadOnly)
                    fila.Cells["Devolver"].Value = System.Text.RegularExpressions.Regex.IsMatch(texto, "\\\"devolver\\\"\\s*:\\s*true");
            }
            ActualizarModeloHistorialWeb();
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
            btnDevolver.DefinirRolVisual(true);
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
            DialogResult respuesta = DialogoEventAura.Confirmar(this,
                "Devolver entradas", $"¿Devolver {elegidas.Count} entrada(s)?\n\n" +
                "Se reintegra el precio de cada entrada con su descuento. Si devolvés todas las que " +
                "quedan, también los servicios. El cargo por servicio y el seguro no se reintegran.\n\n" +
                "La devolución no se puede deshacer.", "Confirmar devolución", "Volver", true);
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
                Avisos.Regla("Devolución", ex);
                return;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Avisos.Informativo(this, ex.Message);
                CargarCompras(idCompra);
                return;
            }

            Avisos.Informativo(this, $"Devolución registrada. Se reintegran {monto.ToString("C2", Pesos)}.");
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
            Tema.EstiloGrilla(grilla);
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
            if (NavegacionCliente.Volver()) return;
            frmMenuCliente frm = new frmMenuCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
    }
}
