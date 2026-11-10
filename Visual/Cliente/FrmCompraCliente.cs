
using iText;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa.Checker;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Point = System.Drawing.Point;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using Entidades;
using Microsoft.Identity.Client;

namespace Visual
{
    public partial class FrmCompraCliente : FormularioEventAura
    {
        public int IDUsuario;
        public int IDEvento;
        public string NombreUsuario;

        // RF-05: lo que el cliente va armando. Vive en memoria hasta confirmar la compra.
        private Paquete paquete;
        private Evento evento;
        private Cliente cliente;   // RN-22: su nivel persistido es el que se aplica
        private Dictionary<int, Sector> sectores = new Dictionary<int, Sector>();
        private bool cargandoGrilla;
        private bool agotado;
        private Panel contenidoCompra;
        private Panel accionesCompra;
        private bool distribuyendoCompra;

        public FrmCompraCliente()
        {
            InitializeComponent();
            ConfigurarAreaDeCompra();
        }

        // RNF-02/RNF-03: las acciones quedan visibles aun si el contenido necesita
        // desplazamiento en una notebook o con texto ampliado por Windows.
        private void ConfigurarAreaDeCompra()
        {
            accionesCompra = pnlAccionesCompra;
            contenidoCompra = pnlContenidoCompra;
            // El panel pinta el fondo; las etiquetas hermanas deben quedar delante.
            pnlEncabezadoCompra.SendToBack();
            accionesCompra.Resize += (sender, e) => AlinearAcciones();
            accionesCompra.Layout += (sender, e) => AlinearAcciones();
            pnlEncabezadoCompra.Paint += PintarSeparadorInferior;
            accionesCompra.Paint += PintarSeparadorSuperior;
            pnlTarjetaEntradas.Paint += PintarTarjeta;
            pnlTarjetaServicios.Paint += PintarTarjeta;
            pnlResumen.Paint += PintarTarjeta;
            label1.TextChanged += (sender, e) => AjustarEncabezado();
            label1.FontChanged += (sender, e) => AjustarEncabezado();
            contenidoCompra.AutoScrollMargin = Size.Empty;
            contenidoCompra.SizeChanged += (sender, e) => AjustarEncabezado();
            lblResumen.SizeChanged += (sender, e) => AjustarEncabezado();
            AjustarEncabezado();
            AlinearAcciones();
        }

        // El contenedor asigna columnas seg?n la vista disponible. La tarjeta del resumen
        // ocupa como m?ximo una vista; su documento mantiene fuente y contenido y se desplaza
        // dentro de ella cuando no entra. No se confunde el alto del documento con el de la vista.
        private void AjustarEncabezado()
        {
            if (distribuyendoCompra) return;
            distribuyendoCompra = true;
            int anchoUsado = contenidoCompra.ClientSize.Width;
            try
            {
                float escala = DeviceDpi / Tema.DpiDiseno * Font.SizeInPoints / Tema.Cuerpo.SizeInPoints;
                int margen = (int)Math.Round(Tema.Espacio.XL * escala);
                int relleno = (int)Math.Round(Tema.Espacio.L * escala);
                int separacion = (int)Math.Round(Tema.Espacio.S * escala);
                Point desplazamiento = contenidoCompra.AutoScrollPosition;
                contenidoCompra.SuspendLayout();
                int ancho = Math.Max(0, anchoUsado - margen * 2);
                label1.SetBounds(margen + desplazamiento.X, (int)(Tema.Espacio.XS * escala) + desplazamiento.Y,
                    ancho, label1.GetPreferredSize(new Size(ancho, 0)).Height);
                lblNivel.Location = new Point(margen + desplazamiento.X, label1.Bottom + separacion);
                lnkNiveles.Location = new Point(lblNivel.Right + separacion, lblNivel.Top);
                int altoEncabezado = Math.Max((int)(Tema.AltoEncabezadoVentana * escala),
                    Math.Max(lblNivel.Bottom, lnkNiveles.Bottom) - desplazamiento.Y + separacion);
                pnlEncabezadoCompra.SetBounds(desplazamiento.X, desplazamiento.Y, anchoUsado, altoEncabezado);
                int inicio = altoEncabezado + margen + desplazamiento.Y;
                int columna = Math.Max(0, (ancho - margen) / 2);
                pnlTarjetaEntradas.SetBounds(margen + desplazamiento.X, inicio, columna, pnlTarjetaEntradas.Height);
                dgvSectores.Width = Math.Max(0, columna - relleno * 2);
                lblAgotado.Width = dgvSectores.Width;
                btnListaEspera.Left = (columna - btnListaEspera.Width) / 2;
                pnlTarjetaServicios.SetBounds(margen + desplazamiento.X, pnlTarjetaEntradas.Bottom + margen,
                    columna, pnlTarjetaServicios.Height);
                foreach (CheckBox casilla in ChecksDeServicio().Concat(new[] { chkSeguro }))
                    casilla.Width = Math.Max(0, columna - relleno * 2);
                int anchoResumen = Math.Max(0, ancho - margen - columna);
                pnlResumen.Width = anchoResumen;
                int anchoDocumento = Math.Max(0, anchoResumen - relleno * 2 - SystemInformation.VerticalScrollBarWidth);
                lblResumen.MinimumSize = new Size(anchoDocumento, 0);
                lblResumen.MaximumSize = new Size(anchoDocumento, 0);
                int altoDocumento = lblResumen.GetPreferredSize(new Size(anchoDocumento, 0)).Height;
                int altoNatural = lblResumen.Top - pnlResumen.AutoScrollPosition.Y + altoDocumento + relleno;
                int altoVista = Math.Max(0, contenidoCompra.ClientSize.Height - margen * 2);
                pnlResumen.SetBounds(margen + columna + margen + desplazamiento.X, inicio,
                    anchoResumen, Math.Min(altoNatural, altoVista));
            }
            finally
            {
                contenidoCompra.ResumeLayout(true);
                distribuyendoCompra = false;
            }
            // Una barra vertical puede cambiar el ancho ?til durante la distribuci?n.
            if (anchoUsado != contenidoCompra.ClientSize.Width) AjustarEncabezado();
        }

        private static void PintarTarjeta(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var lapiz = new Pen(Tema.Borde, Tema.BordeNormal))
                e.Graphics.DrawRectangle(lapiz, 0, 0, panel.ClientSize.Width - Tema.BordeNormal,
                    panel.ClientSize.Height - Tema.BordeNormal);
        }

        private static void PintarSeparadorInferior(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var lapiz = new Pen(Tema.Borde, Tema.BordeNormal))
                e.Graphics.DrawLine(lapiz, 0, panel.ClientSize.Height - Tema.BordeNormal,
                    panel.ClientSize.Width, panel.ClientSize.Height - Tema.BordeNormal);
        }

        private static void PintarSeparadorSuperior(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var lapiz = new Pen(Tema.Borde, Tema.BordeNormal))
                e.Graphics.DrawLine(lapiz, 0, 0, panel.ClientSize.Width, 0);
        }

        private void AlinearAcciones()
        {
            btnVolver.Location = new Point(accionesCompra.Padding.Left, (accionesCompra.Height - btnVolver.Height) / 2);
            btnComprar.Location = new Point(accionesCompra.ClientSize.Width - accionesCompra.Padding.Right - btnComprar.Width,
                (accionesCompra.Height - btnComprar.Height) / 2);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            AjustarAreaVisible(Screen.FromControl(this).WorkingArea);
        }

        private void AjustarAreaVisible(System.Drawing.Rectangle areaTrabajo)
        {
            Size = new Size(Math.Min(Width, areaTrabajo.Width), Math.Min(Height, areaTrabajo.Height));
            Location = new Point(areaTrabajo.Left + (areaTrabajo.Width - Width) / 2,
                areaTrabajo.Top + (areaTrabajo.Height - Height) / 2);
            AjustarEncabezado();
            AlinearAcciones();
        }
        private void FrmCompraCliente_Load(object sender, EventArgs e)
        {
            // RN-15: liberación perezosa de reservas vencidas, antes de calcular los disponibles.
            GestorCompras.Instance.LiberarVencidas();
            try
            {
                paquete = GestorPaquetes.Instance.Nuevo(IDEvento, IDUsuario);   // RN-09 + RN-39
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("No se puede armar el paquete", ex);
                VolverAlListado();
                return;
            }
            evento = GestorEventos.Instance.ObtenerPorId(IDEvento);
            label1.Text = evento.Nombre;

            // RN-23: reevaluación mensual antes de iniciar la operación, nunca en medio de ella.
            cliente = GestorFidelizacion.Instance.ObtenerCliente(IDUsuario);
            GestorFidelizacion.Instance.ReevaluarNivel(cliente, DateTime.Now);
            CargarNivel();
            CargarSectores();
            CargarServicios();
            chkSeguro.CheckedChanged += chkSeguro_CheckedChanged;
            AjustarEncabezado();
            lblResumen.UseMnemonic = false;   // «Meet & Greet»: el & no es tecla de acceso
            MostrarSiAgotado();
            ActualizarResumen();
        }

        // RN-10: con el evento agotado, en lugar de la grilla de sectores se ofrece la lista de espera.
        private void MostrarSiAgotado()
        {
            agotado = GestorEventos.Instance.EstaAgotado(IDEvento);
            dgvSectores.Visible = !agotado;
            lblAgotado.Visible = agotado;
            btnListaEspera.Visible = agotado;
            foreach (CheckBox chk in ChecksDeServicio())
            {
                // Si una devolución libera cupo mientras esta ventana está abierta,
                // rehabilitar solo los servicios que el evento realmente ofrece (RN-14).
                chk.Enabled = !agotado && chk.Tag is Servicio;
            }
            if (agotado)
            {
                MarcarInscripto(GestorListaEspera.Instance.EstaInscripto(IDEvento, IDUsuario));
            }
            ActualizarHabilitados();
        }

        private void ActualizarHabilitados()
        {
            bool hayEntradas = GestorPaquetes.Instance.CantidadEntradas(paquete) > 0;
            btnComprar.Enabled = hayEntradas && !agotado;
            chkSeguro.Enabled = hayEntradas && !agotado;
            if (!chkSeguro.Enabled && chkSeguro.Checked)
            {
                chkSeguro.Checked = false;
            }
        }

        // RF-10: después de anotarse el botón queda deshabilitado, diciendo que ya está en la lista.
        private void MarcarInscripto(bool inscripto)
        {
            btnListaEspera.Enabled = !inscripto;
            btnListaEspera.Text = inscripto ? "Ya estás en la lista de espera" : "Anotarme en la lista de espera";
        }

        private void btnListaEspera_Click(object sender, EventArgs e)
        {
            try
            {
                GestorListaEspera.Instance.Inscribir(IDEvento, IDUsuario);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("No se puede armar el paquete", ex);
                if (ex.Regla == "RN-36")
                {
                    MarcarInscripto(true);
                }
                else if (ex.Regla == "RN-10")
                {
                    MostrarSiAgotado();   // se liberó cupo: vuelve la grilla
                    RecargarDisponibles();
                }
                return;
            }
            MarcarInscripto(true);
            MessageBox.Show("Te anotaste en la lista de espera de «" + evento.Nombre + "». " +
                            "Te vamos a notificar si se libera un lugar.",
                            "Lista de espera", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // RF-06: reserva el cupo y pasa a la pantalla de pago.
        private void btnComprar_Click(object sender, EventArgs e)
        {
            // H-04: el seguro solo se contrata durante la compra (RN-28).
            if (!paquete.ConSeguro && GestorPaquetes.Instance.CantidadEntradas(paquete) > 0)
            {
                DialogResult sinSeguro = MessageBox.Show(
                    "No agregaste el seguro de entrada. Solo se puede contratar ahora: después de " +
                    "pagar no se puede incorporar.\n\n¿Continuar sin seguro?",
                    "Seguro de entrada", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (sinSeguro != DialogResult.Yes)
                {
                    return;
                }
            }

            Compra compra;
            try
            {
                compra = GestorCompras.Instance.Iniciar(paquete, cliente);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("No se puede completar la compra", ex);
                RecargarDisponibles();
                return;
            }

            DialogResult resultado;
            using (var pago = new frmPago { Compra = compra })
            {
                resultado = pago.ShowDialog(this);
            }

            if (resultado == DialogResult.Abort)
            {
                // H-02: venció el plazo; el cupo ya se liberó.
                VolverAlListado();
                return;
            }
            if (resultado != DialogResult.OK)
            {
                RecargarDisponibles();
                return;
            }

            List<string> rutas = GenerarTicket(compra);
            using (var confirmacion = new frmConfirmacion { Compra = compra, Rutas = rutas, Carpeta = CarpetaTickets() })
            {
                confirmacion.ShowDialog(this);
            }

            // RN-22: si la compra subió el nivel, rige desde esta operación siguiente.
            cliente = GestorFidelizacion.Instance.ObtenerCliente(IDUsuario);
            CargarNivel();
            try
            {
                paquete = GestorPaquetes.Instance.Nuevo(IDEvento, IDUsuario);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("No se puede armar el paquete", ex);
                VolverAlListado();
                return;
            }
            CargarSectores();
            foreach (CheckBox chk in ChecksDeServicio())
            {
                chk.Checked = false;
            }
            chkSeguro.Checked = false;
            MostrarSiAgotado();
            ActualizarResumen();
        }

        // --------------------------------------        FUNCIONES          --------------------------------------

        // Vuelve a leer el cupo sin perder lo que el cliente pidió.
        private void RecargarDisponibles()
        {
            cargandoGrilla = true;
            foreach (DataGridViewRow fila in dgvSectores.Rows)
            {
                fila.Cells["Disponibles"].Value =
                    GestorPaquetes.Instance.DisponiblesDe(Convert.ToInt32(fila.Cells["IdSector"].Value));
            }
            cargandoGrilla = false;
        }

        private void CargarNivel()
        {
            decimal porcentaje = GestorFidelizacion.Instance.PorcentajePara(cliente.Nivel);
            lblNivel.Text = $"Tu nivel: {cliente.Nivel} · {porcentaje.ToString("P1", Pesos)} de descuento";
            lnkNiveles.Left = lblNivel.Right + Tema.Espacio.S;
            AjustarEncabezado();
        }

        // H-05: explica los niveles con los valores vigentes de la tabla Parametros.
        private void lnkNiveles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int meses = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_VENTANA_MESES");
            int plata = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_PLATA");
            int oro = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_ORO");
            string Porcentaje(NivelFidelidad nivel) =>
                GestorFidelizacion.Instance.PorcentajePara(nivel).ToString("P1", Pesos);

            var texto = new StringBuilder();
            texto.AppendLine($"Tu nivel depende de las compras confirmadas en los últimos {meses} meses:");
            texto.AppendLine();
            texto.AppendLine($"  •  Bronce: de 0 a {plata - 1} compras  →  {Porcentaje(NivelFidelidad.Bronce)} de descuento");
            texto.AppendLine($"  •  Plata: de {plata} a {oro - 1} compras  →  {Porcentaje(NivelFidelidad.Plata)} de descuento");
            texto.AppendLine($"  •  Oro: {oro} compras o más  →  {Porcentaje(NivelFidelidad.Oro)} de descuento");
            texto.AppendLine();
            texto.AppendLine("El descuento se aplica sobre el valor de las entradas, no sobre los servicios ni el cargo por servicio.");
            texto.AppendLine();
            texto.AppendLine("El nivel se reevalúa una vez por mes. Una compra cuenta para tu nivel " +
                             "a partir de la operación siguiente, no en la misma.");
            MessageBox.Show(texto.ToString(), "¿Cómo funcionan los niveles?",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // H-03: la grilla muestra el cupo restante de cada sector. Solo Cantidad es editable.
        private void CargarSectores()
        {
            cargandoGrilla = true;
            dgvSectores.Columns.Clear();
            dgvSectores.Rows.Clear();
            dgvSectores.AllowUserToAddRows = false;
            dgvSectores.AllowUserToDeleteRows = false;
            dgvSectores.AllowUserToResizeRows = false;
            dgvSectores.RowHeadersVisible = false;
            dgvSectores.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvSectores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Tema.EstiloGrilla(dgvSectores);
            dgvSectores.EditMode = DataGridViewEditMode.EditOnEnter;

            dgvSectores.Columns.Add("IdSector", "IdSector");
            dgvSectores.Columns.Add("Sector", "Sector");
            dgvSectores.Columns.Add("Precio", "Precio");
            dgvSectores.Columns.Add("Disponibles", "Disponibles");
            dgvSectores.Columns.Add("Cantidad", "Cantidad (escribí aquí)");
            dgvSectores.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvSectores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSectores.Columns["IdSector"].Visible = false;
            foreach (DataGridViewColumn columna in dgvSectores.Columns)
            {
                columna.ReadOnly = columna.Name != "Cantidad";
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgvSectores.Columns["Precio"].DefaultCellStyle.Format = "C2";
            dgvSectores.Columns["Precio"].DefaultCellStyle.FormatProvider = Pesos;
            foreach (string nombre in new[] { "Precio", "Disponibles", "Cantidad" })
            {
                dgvSectores.Columns[nombre].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvSectores.Columns["Cantidad"].DefaultCellStyle.BackColor = Tema.PrimarioSuave;

            sectores.Clear();
            int max = GestorParametros.Instance.ObtenerEntero("COMPRA_ENTRADAS_MAX");
            foreach (Sector sector in GestorEventos.Instance.Sectores(IDEvento))
            {
                sectores[sector.Id] = sector;
                int fila = dgvSectores.Rows.Add(sector.Id, sector.Nombre, sector.Precio,
                    GestorPaquetes.Instance.DisponiblesDe(sector.Id), 0);
                dgvSectores.Rows[fila].Cells["Cantidad"].ToolTipText =
                    $"Escribí cuántas entradas querés de este sector (hasta {max} por compra).";
            }

            DataGridViewRow primeraDisponible = dgvSectores.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(f => Convert.ToInt32(f.Cells["Disponibles"].Value) > 0);
            if (primeraDisponible != null)
            {
                dgvSectores.CurrentCell = primeraDisponible.Cells["Cantidad"];
            }

            dgvSectores.CellValueChanged -= dgvSectores_CellValueChanged;
            dgvSectores.CellValueChanged += dgvSectores_CellValueChanged;
            cargandoGrilla = false;
        }

        // RN-08 y RN-12 los valida GestorPaquetes; si rechaza, la celda vuelve a lo que había.
        private void dgvSectores_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (cargandoGrilla || e.RowIndex < 0 || dgvSectores.Columns[e.ColumnIndex].Name != "Cantidad")
            {
                return;
            }
            DataGridViewRow fila = dgvSectores.Rows[e.RowIndex];
            int idSector = Convert.ToInt32(fila.Cells["IdSector"].Value);
            int anterior = paquete.Entradas.Where(x => x.IdSector == idSector).Sum(x => x.CantidadVendida);

            int cantidad;
            string texto = Convert.ToString(fila.Cells["Cantidad"].Value);
            if (!int.TryParse(string.IsNullOrWhiteSpace(texto) ? "0" : texto.Trim(), out cantidad) || cantidad < 0)
            {
                MessageBox.Show("La cantidad debe ser un número entero de 0 en adelante.", "Revisá los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RestaurarCantidad(fila, anterior);
                return;
            }
            try
            {
                GestorPaquetes.Instance.AgregarEntradas(paquete, idSector, cantidad);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("No se puede armar el paquete", ex);
                RestaurarCantidad(fila, anterior);
                return;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "No se puede modificar la cantidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RestaurarCantidad(fila, anterior);
                return;
            }
            ActualizarResumen();
        }

        private void RestaurarCantidad(DataGridViewRow fila, int cantidad)
        {
            cargandoGrilla = true;
            fila.Cells["Cantidad"].Value = cantidad;
            cargandoGrilla = false;
        }

        // RN-14: cada casilla se habilita solo si el evento ofrece un servicio de ese tipo.
        private void CargarServicios()
        {
            List<Servicio> servicios = GestorPaquetes.Instance.ServiciosDe(IDEvento);
            var tipos = new Dictionary<CheckBox, string>
            {
                { chkHospedaje, "Hospedaje" },
                { chkTraslado, "Traslado" },
                { chkMerch, "Merchandising" },
                { chkVIP, "VIP" }
            };
            foreach (KeyValuePair<CheckBox, string> par in tipos)
            {
                CheckBox chk = par.Key;
                Servicio servicio = servicios.FirstOrDefault(s => s.Tipo == par.Value);
                chk.Tag = servicio;
                chk.Enabled = servicio != null;
                chk.Text = servicio == null
                    ? par.Value + " (no disponible)"
                    : $"{par.Value} \u00b7 {servicio.Precio.ToString("C0", Pesos)}";
                chk.CheckedChanged -= chkServicio_CheckedChanged;
                chk.CheckedChanged += chkServicio_CheckedChanged;
            }
        }

        private IEnumerable<CheckBox> ChecksDeServicio()
        {
            return new[] { chkHospedaje, chkTraslado, chkMerch, chkVIP };
        }

        private void chkServicio_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            Servicio servicio = chk.Tag as Servicio;
            if (servicio == null)
            {
                return;
            }
            try
            {
                if (chk.Checked)
                {
                    GestorPaquetes.Instance.AgregarServicio(paquete, servicio.Id);
                }
                else
                {
                    GestorPaquetes.Instance.QuitarServicio(paquete, servicio.Id);
                }
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("No se puede armar el paquete", ex);
                chk.Checked = false;
            }
            ActualizarResumen();
        }

        private void chkSeguro_CheckedChanged(object sender, EventArgs e)
        {
            paquete.ConSeguro = chkSeguro.Checked;
            ActualizarResumen();
        }

        // RF-05 + RF-08: desglose del paquete con el orden de cálculo de docs/02-reglas-negocio.md.
        private void ActualizarResumen()
        {
            ResumenDescuento resumen = GestorFidelizacion.Instance.CalcularDescuento(cliente, paquete);

            var texto = new StringBuilder();
            texto.AppendLine("RESUMEN DEL PAQUETE");
            texto.AppendLine();
            texto.AppendLine($"Entradas ({GestorPaquetes.Instance.CantidadEntradas(paquete)})");
            if (paquete.Entradas.Count == 0)
            {
                texto.AppendLine("  (ninguna)");
            }
            foreach (Entrada entrada in paquete.Entradas)
            {
                Sector sector = sectores[entrada.IdSector.Value];
                texto.AppendLine(Importe($"  {entrada.CantidadVendida} x {sector.Nombre}", entrada.Precio * entrada.CantidadVendida));
            }
            texto.AppendLine(Importe("Base entradas", resumen.BaseEntradas));
            texto.AppendLine(Importe($"Desc. {resumen.NivelAplicado} {resumen.Porcentaje.ToString("P1", Pesos)}", -resumen.MontoDescuento));
            texto.AppendLine(Importe("Subtotal entradas", resumen.SubtotalEntradas));
            texto.AppendLine();
            texto.AppendLine("Servicios (sin descuento)");
            if (paquete.Servicios.Count == 0)
            {
                texto.AppendLine("  (ninguno)");
            }
            foreach (Servicio servicio in paquete.Servicios)
            {
                texto.AppendLine(Importe("  " + servicio.Nombre, servicio.Precio));
            }
            texto.AppendLine(Importe("Subtotal servicios", resumen.Servicios));
            texto.AppendLine();
            texto.AppendLine(Importe("Cargo por servicio", resumen.CargoServicio));
            int cantidad = GestorPaquetes.Instance.CantidadEntradas(paquete);
            // RN-37: con suscripción activa el seguro sale sin cargo.
            string seguro = cantidad > 0 && resumen.Seguro == 0m ? "Seguro sin cargo" : "Seguro";
            texto.AppendLine(paquete.ConSeguro
                ? Importe($"{seguro} ({cantidad} entradas)", resumen.Seguro)
                : "Seguro: no");
            texto.AppendLine();
            texto.AppendLine(Importe("TOTAL", resumen.Total));
            // H-01: el hallazgo de usabilidad más repetido.
            texto.AppendLine("El descuento se aplica sobre el valor");
            texto.AppendLine("de las entradas.");
            lblResumen.Text = texto.ToString();
            ActualizarHabilitados();
        }

        // RN-33: importes en pesos argentinos.
        private static readonly System.Globalization.CultureInfo Pesos = new System.Globalization.CultureInfo("es-AR");

        // Una línea de 38 columnas: concepto a la izquierda (recortado si no entra), importe a la derecha.
        private const int AnchoLinea = 38;

        private static string Importe(string concepto, decimal monto)
        {
            string importe = monto.ToString("C2", Pesos);
            int ancho = AnchoLinea - importe.Length - 1;
            if (concepto.Length > ancho)
            {
                concepto = concepto.Substring(0, ancho - 1) + "…";
            }
            return concepto.PadRight(ancho) + " " + importe;
        }

        // H-06: confirma antes de descartar lo cargado.
        private bool PaqueteVacio()
        {
            return paquete == null
                || (paquete.Entradas.Count == 0 && paquete.Servicios.Count == 0 && !paquete.ConSeguro);
        }

        private static string CarpetaTickets()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A1.Tickets EVENTAURA");
        }

        // RF-07 + RN-18: un PDF por entrada confirmada, con el QR de su propio código.
        public List<string> GenerarTicket(Compra compra)
        {
            string carpetaTickets = CarpetaTickets();
            if (!Directory.Exists(carpetaTickets))
            {
                Directory.CreateDirectory(carpetaTickets);
            }

            string fechaevento = evento.Fecha.ToString("dd/MM/yyyy HH:mm");
            string pedido = frmConfirmacion.NumeroPedido(compra);
            var rutas = new List<string>();
            int CantidadTickets = 0;
            foreach (Entrada entrada in GestorCompras.Instance.EntradasDe(compra))
            {
                if (entrada.Estado != EstadoEntrada.Confirmada)
                {
                    continue;
                }
                CantidadTickets++;
                Sector sector = sectores[entrada.IdSector.Value];
                string mensaje =
                    "ENTRADA " + evento.Nombre + "\n" +
                    "SECTOR: " + sector.Nombre + "\n" +
                    "PRECIO: " + entrada.Precio.ToString("C2", Pesos) + "\n" +
                    "FECHA: " + fechaevento + "\n" +
                    "PEDIDO: " + pedido + "\n" +
                    "CÓDIGO: " + entrada.CodigoQR + "\n";
                string Ruta = System.IO.Path.Combine(carpetaTickets,
                    $"Ticket_{pedido}_{sector.Nombre}_{CantidadTickets}.PDF");
                GenerarPDF(Ruta, mensaje, GeneradorQR.Instance.Imagen(entrada.CodigoQR));
                rutas.Add(Ruta);
            }
            return rutas;
        }
        public void GenerarPDF(string ruta, string mensaje, byte[] QR)
        {
            string[] lineas = mensaje.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string nombre = lineas.FirstOrDefault() ?? string.Empty;
            if (nombre.StartsWith("ENTRADA ", StringComparison.Ordinal)) nombre = nombre.Substring("ENTRADA ".Length);
            Func<string, string> dato = rotulo =>
            {
                string prefijo = rotulo + ":";
                string linea = lineas.FirstOrDefault(x => x.StartsWith(prefijo, StringComparison.Ordinal));
                return linea == null ? string.Empty : linea.Substring(prefijo.Length).Trim();
            };
            using (var escritor = new PdfWriter(ruta))
            using (var pdf = new PdfDocument(escritor))
            {
                PdfPage pagina = pdf.AddNewPage(PageSize.A6.Rotate());
                float ancho = pagina.GetPageSize().GetWidth();
                float alto = pagina.GetPageSize().GetHeight();
                float margen = Tema.Ticket.Margen;
                float anchoTexto = ancho - margen * 2 - Tema.Ticket.LadoQr - Tema.Ticket.Separacion;
                PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont negrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont codigo = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                var lienzo = new PdfCanvas(pagina);
                // El papel queda blanco; solo se imprime la franja de marca.
                lienzo.SetFillColor(ColorPdf(Tema.Primario));
                lienzo.Rectangle(0, alto - Tema.Ticket.Franja, ancho, Tema.Ticket.Franja).Fill();
                using (var franja = new Canvas(lienzo, new iText.Kernel.Geom.Rectangle(
                    margen, alto - Tema.Ticket.Franja, ancho - margen * 2, Tema.Ticket.Franja)))
                {
                    franja.Add(new Paragraph("EventAura \u00b7 Entrada").SetFont(negrita).SetFontSize(Tema.Ticket.Dato)
                        .SetFontColor(ColorPdf(Tema.Superficie)).SetMargin(0));
                }
                // El nombre usa el ancho disponible sobre los datos y el QR para admitir varias l?neas.
                float baseTitulo = Tema.Ticket.BaseQr + Tema.Ticket.LadoQr + Tema.Ticket.EspacioTitulo;
                using (var titulo = new Canvas(lienzo, new iText.Kernel.Geom.Rectangle(margen, baseTitulo,
                    ancho - margen * 2, alto - Tema.Ticket.Franja - margen - baseTitulo)))
                {
                    titulo.Add(new Paragraph(nombre).SetFont(negrita).SetFontSize(Tema.Ticket.Titulo)
                        .SetFontColor(ColorPdf(Tema.Texto)).SetMargin(0).SetMultipliedLeading(1));
                }
                using (var datos = new Canvas(lienzo, new iText.Kernel.Geom.Rectangle(margen, Tema.Ticket.BaseDatos,
                    anchoTexto, Tema.Ticket.BaseQr + Tema.Ticket.LadoQr - Tema.Ticket.BaseDatos)))
                {
                    var tabla = new Table(UnitValue.CreatePercentArray(new[] { Tema.Ticket.PesoRotulo, Tema.Ticket.PesoDato }))
                        .UseAllAvailableWidth();
                    foreach (string rotulo in new[] { "SECTOR", "FECHA", "PRECIO", "PEDIDO" })
                    {
                        tabla.AddCell(new Cell().SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetPadding(0)
                            .SetPaddingBottom(Tema.Ticket.EspacioFila).Add(new Paragraph(rotulo).SetFont(normal)
                                .SetFontSize(Tema.Ticket.Rotulo).SetFontColor(ColorPdf(Tema.TextoSecundario)).SetMargin(0)));
                        tabla.AddCell(new Cell().SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetPadding(0)
                            .SetPaddingBottom(Tema.Ticket.EspacioFila).Add(new Paragraph(dato(rotulo)).SetFont(normal)
                                .SetFontSize(Tema.Ticket.Dato).SetFontColor(ColorPdf(Tema.Texto)).SetMargin(0)));
                    }
                    datos.Add(tabla);
                }
                var imagen = new iText.Layout.Element.Image(ImageDataFactory.Create(QR));
                imagen.ScaleAbsolute(Tema.Ticket.LadoQr, Tema.Ticket.LadoQr);
                using (var qr = new Canvas(lienzo, new iText.Kernel.Geom.Rectangle(
                    ancho - margen - Tema.Ticket.LadoQr, Tema.Ticket.BaseQr, Tema.Ticket.LadoQr, Tema.Ticket.LadoQr)))
                    qr.Add(imagen);
                using (var pie = new Canvas(lienzo, pagina.GetPageSize()))
                {
                    pie.ShowTextAligned(new Paragraph("C\u00f3digo " + dato("C\u00d3DIGO")).SetFont(codigo)
                        .SetFontSize(Tema.Ticket.Rotulo).SetFontColor(ColorPdf(Tema.Texto)),
                        margen, Tema.Ticket.BaseCodigo, TextAlignment.LEFT);
                    pie.ShowTextAligned(new Paragraph("Present\u00e1 este c\u00f3digo en el ingreso.").SetFont(normal)
                        .SetFontSize(Tema.Ticket.Rotulo).SetFontColor(ColorPdf(Tema.TextoSecundario)),
                        margen, Tema.Ticket.BaseAyuda, TextAlignment.LEFT);
                }
            }
        }

        private static DeviceRgb ColorPdf(System.Drawing.Color color)
        {
            return new DeviceRgb(color.R, color.G, color.B);
        }
        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (!PaqueteVacio())
            {
                DialogResult respuesta = MessageBox.Show(
                    "Si vuelve al listado se descarta el paquete que armó. ¿Desea volver?",
                    "Volver al listado", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (respuesta != DialogResult.Yes)
                {
                    return;
                }
            }
            VolverAlListado();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !PaqueteVacio())
            {
                DialogResult respuesta = MessageBox.Show(
                    "Si cierra la ventana se descarta el paquete que armó. ¿Desea cerrar?",
                    "Cerrar la compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (respuesta != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }

        private void VolverAlListado()
        {
            frmEventosCliente frm = new frmEventosCliente();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }

    }
}
