
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

namespace Visual
{
    public partial class FrmCompraCliente : FormularioEventAura
    {
        public int IDUsuario;
        public int IDEvento;
        public string NombreUsuario;
        public int? SectorInicial;

        // RF-05: lo que el cliente va armando. Vive en memoria hasta confirmar la compra.
        private Paquete paquete;
        private bool accesoRechazado;
        private Evento evento;
        private Cliente cliente;   // RN-22: su nivel persistido es el que se aplica
        private Dictionary<int, Sector> sectores = new Dictionary<int, Sector>();
        private bool cargandoGrilla;
        private bool compraEnCurso;
        private bool compraFinalizada;
        internal Func<Compra, List<string>> CrearTickets = null;
        internal bool CompraFinalizada => compraFinalizada;
        private bool agotado;
        private Panel contenidoCompra;
        private Panel accionesCompra;
        private Panel documentoResumen;
        private readonly Web.PanelWeb resumenWeb = new Web.PanelWeb
            { Name = "resumenCompraWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoResumenWeb = new Label
            { Name = "avisoCompraWeb", AutoSize = true, ForeColor = Tema.TextoSecundario, Visible = false };
        private readonly Visual.Componentes.BotonSecundario abrirServicios = new Visual.Componentes.BotonSecundario
            { Name = "btnAbrirServicios", Text = "Ver servicios" };
        private readonly Visual.Componentes.BotonSecundario volverASectores = new Visual.Componentes.BotonSecundario
            { Name = "btnVolverASectores", Text = "Ver entradas" };
        private bool distribuyendoCompra;

        public FrmCompraCliente()
        {
            InitializeComponent();
            ConfigurarAreaDeCompra();
            mapaSectores.SectorElegido += ElegirSectorEnMapa;
        }

        // RNF-02/RNF-03: las acciones quedan visibles aun si el contenido necesita
        // desplazamiento en una notebook o con texto ampliado por Windows.
        private void ConfigurarAreaDeCompra()
        {
            Tema.ComoFondo(this);
            Tema.ComoFondo(pnlContenidoCompra);
            foreach (Control superficie in new Control[] { pnlEncabezadoCompra, pnlAccionesCompra, pnlTarjetaEntradas, pnlTarjetaServicios, pnlResumen })
                Tema.ComoSuperficie(superficie);
            lblResumen.AutoSize = false;
            lblResumen.Padding = Padding.Empty;
            mapaSectores.Compacto = true;
            documentoResumen = new Panel { Name = "documentoResumen", AutoScroll = true, BackColor = Tema.Superficie };
            pnlResumen.AutoScroll = false;
            pnlResumen.Controls.Add(documentoResumen);
            documentoResumen.Controls.Add(lblResumen);
            pnlResumen.Controls.Add(resumenWeb);
            pnlResumen.Controls.Add(avisoResumenWeb);
            avisoResumenWeb.Text = "Vista web no disponible";
            avisoResumenWeb.AccessibleDescription = "La vista web no está disponible.";
            resumenWeb.VistaLista += ActualizarVistaResumen;
            resumenWeb.VistaNoDisponible += ActualizarVistaResumen;
            resumenWeb.Escape += () =>
            {
                Message mensaje = default(Message);
                ProcessCmdKey(ref mensaje, Keys.Escape);
            };
            lblResumen.LocationChanged += (s, e) => lblResumen.Invalidate();
            pnlTarjetaEntradas.Controls.Add(abrirServicios);
            pnlTarjetaServicios.Controls.Add(volverASectores);
            pnlTarjetaServicios.Hide();
            abrirServicios.Click += (s, e) =>
            {
                pnlTarjetaServicios.Show();
                pnlTarjetaEntradas.Hide();
                volverASectores.Focus();
            };
            volverASectores.Click += (s, e) =>
            {
                pnlTarjetaEntradas.Show();
                pnlTarjetaServicios.Hide();
                abrirServicios.Focus();
            };
            lblResumen.Location = Point.Empty;
            lblTotalDestacado.AutoSize = false;
            lblTotalDestacado.BackColor = Tema.PrimarioSuave;
            lblTotalDestacado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Tema.ComoEnlace(lnkNiveles);
            Tema.ComoEnlace(lnkPosicionEspera);
            accionesCompra = pnlAccionesCompra;
            contenidoCompra = pnlContenidoCompra;
            // El panel pinta el fondo; las etiquetas hermanas deben quedar delante.
            pnlEncabezadoCompra.SendToBack();
            accionesCompra.Resize += (sender, e) => AlinearAcciones();
            accionesCompra.Layout += (sender, e) => AlinearAcciones();
            pnlEncabezadoCompra.Paint += Disposicion.PintarSeparadorInferior;
            accionesCompra.Paint += Disposicion.PintarSeparadorSuperior;
            pnlTarjetaEntradas.Paint += Disposicion.PintarTarjeta;
            pnlTarjetaServicios.Paint += Disposicion.PintarTarjeta;
            pnlResumen.Paint += Disposicion.PintarTarjeta;
            label1.TextChanged += (sender, e) => AjustarEncabezado();
            label1.FontChanged += (sender, e) => AjustarEncabezado();
            contenidoCompra.AutoScrollMargin = new Size(0, Tema.MargenFormulario);
            contenidoCompra.SizeChanged += (sender, e) => AjustarEncabezado();
            lblResumen.SizeChanged += (sender, e) => AjustarEncabezado();
            AjustarEncabezado();
            AlinearAcciones();
        }

        // El contenedor asigna columnas según la vista disponible. La tarjeta del resumen
        // ocupa como máximo una vista; su documento mantiene fuente y contenido y se desplaza
        // dentro de ella cuando no entra. No se confunde el alto del documento con el de la vista.
        private void AjustarEncabezado()
        {
            if (distribuyendoCompra) return;
            distribuyendoCompra = true;
            try { Disposicion.RepetirUnaVez(contenidoCompra, DistribuirCompra); }
            finally { distribuyendoCompra = false; }
        }

        private void DistribuirCompra()
        {
            int anchoUsado = contenidoCompra.ClientSize.Width;
            try
            {
                float escala = Disposicion.Escala(this);
                int margen = (int)Math.Round(Tema.Espacio.XL * escala);
                int relleno = (int)Math.Round(Tema.Espacio.L * escala);
                int separacion = (int)Math.Round(Tema.Espacio.S * escala);
                Point desplazamiento = contenidoCompra.AutoScrollPosition;
                contenidoCompra.SuspendLayout();
                int ancho = Math.Max(0, anchoUsado - margen * 2);
                indicadorPasos.SetBounds(margen, 0, ancho, (int)(Tema.AltoBoton * escala));
                label1.SetBounds(margen, indicadorPasos.Bottom + separacion,
                    ancho, label1.GetPreferredSize(new Size(ancho, 0)).Height);
                lblNivel.Location = new Point(margen, label1.Bottom + separacion);
                lnkNiveles.Location = new Point(lblNivel.Right + separacion, lblNivel.Top);
                int altoEncabezado = Math.Max((int)(Tema.AltoEncabezadoVentana * escala),
                    Math.Max(lblNivel.Bottom, lnkNiveles.Bottom) + separacion);
                pnlEncabezadoCompra.SetBounds(desplazamiento.X, desplazamiento.Y, anchoUsado, altoEncabezado);
                int huecoEncabezado = Disposicion.Pixeles(this, Tema.Espacio.M);
                int inicio = altoEncabezado + huecoEncabezado + desplazamiento.Y;
                int columna = Math.Max(0, (ancho - margen) / 2);
                pnlTarjetaEntradas.SetBounds(margen + desplazamiento.X, inicio, columna, pnlTarjetaEntradas.Height);
                dgvSectores.Width = Math.Max(0, columna - relleno * 2);
                mapaSectores.Width = dgvSectores.Width;
                int anchoAccion = Disposicion.Pixeles(this, Tema.AnchoBotonCompacto);
                int altoAccion = Disposicion.Pixeles(this, Tema.AltoBoton);
                int xAccion = columna - relleno - anchoAccion;
                abrirServicios.SetBounds(xAccion, relleno, anchoAccion, altoAccion);
                volverASectores.SetBounds(xAccion, relleno, anchoAccion, altoAccion);
                mapaSectores.Top = abrirServicios.Bottom + separacion;
                mapaSectores.Height = mapaSectores.AltoParaTodosLosSectores;
                dgvSectores.Top = mapaSectores.Bottom + separacion;
                dgvSectores.Height = dgvSectores.ColumnHeadersHeight +
                    dgvSectores.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + Disposicion.Pixeles(this, Tema.Espacio.XS);
                lblAgotado.Top = dgvSectores.Top;
                btnListaEspera.Top = lblAgotado.Bottom + separacion;
                lnkPosicionEspera.Top = btnListaEspera.Bottom + separacion;
                pnlTarjetaEntradas.Height = Math.Max(dgvSectores.Bottom,
                    agotado ? lnkPosicionEspera.Bottom : 0) + relleno;
                lblAgotado.Width = dgvSectores.Width;
                btnListaEspera.Left = (columna - btnListaEspera.Width) / 2;
                lnkPosicionEspera.Left = btnListaEspera.Left;
                lnkPosicionEspera.Width = Math.Max(0,columna-relleno*2);
                pnlTarjetaServicios.SetBounds(margen + desplazamiento.X, inicio, columna, pnlTarjetaServicios.Height);
                int yServicio = volverASectores.Bottom + separacion;
                foreach (CheckBox casilla in ChecksDeServicio().Concat(new[] { chkSeguro }))
                {
                    casilla.SetBounds(relleno, yServicio, Math.Max(1, columna - relleno * 2), altoAccion);
                    yServicio = casilla.Bottom + Disposicion.Pixeles(this, Tema.Espacio.XS);
                }
                pnlTarjetaServicios.Height = chkSeguro.Bottom + relleno;
                int anchoResumen = Math.Max(0, ancho - margen - columna);
                pnlResumen.Width = anchoResumen;
                int altoVista = Math.Max(0, contenidoCompra.ClientSize.Height - altoEncabezado - huecoEncabezado);
                pnlResumen.Location = new Point(margen + columna + margen + desplazamiento.X, inicio);
                int arribaDocumento = lblTituloResumen.Bottom + separacion;
                int anchoInterior = Math.Max(1, anchoResumen - relleno * 2);
                int anchoDocumento = Math.Max(1, anchoInterior - SystemInformation.VerticalScrollBarWidth);
                lblResumen.Size = new Size(anchoDocumento, lblResumen.AltoPara(anchoDocumento));
                int altoTotal = Disposicion.Pixeles(this, Tema.AltoBoton);
                int altoFijo = arribaDocumento + separacion + altoTotal + relleno;
                int altoDocumento = Math.Min(lblResumen.Height, Math.Max(0, altoVista - altoFijo));
                pnlResumen.Height = altoFijo + altoDocumento;
                documentoResumen.SetBounds(relleno, arribaDocumento, anchoInterior, altoDocumento);
                lblTotalDestacado.SetBounds(relleno, documentoResumen.Bottom + separacion, anchoInterior, altoTotal);
                resumenWeb.Bounds = RectangleResumenWeb();
                avisoResumenWeb.Location = new Point(Math.Max(lblTituloResumen.Right + separacion,
                    pnlResumen.Width - relleno - avisoResumenWeb.Width), lblTituloResumen.Top);
            }
            finally
            {
                contenidoCompra.ResumeLayout(true);
            }
        }

        private System.Drawing.Rectangle RectangleResumenWeb() =>
            System.Drawing.Rectangle.FromLTRB(documentoResumen.Left, documentoResumen.Top,
                lblTotalDestacado.Right, lblTotalDestacado.Bottom);

        private void ActualizarVistaResumen()
        {
            resumenWeb.Visible = resumenWeb.Preparado;
            avisoResumenWeb.Visible = resumenWeb.InicializacionTerminada && !resumenWeb.Preparado;
            if (resumenWeb.Visible) resumenWeb.BringToFront();
            if (avisoResumenWeb.Visible) avisoResumenWeb.BringToFront();
            AjustarEncabezado();
        }

        private void AlinearAcciones()
        {
            Disposicion.AlinearAcciones(accionesCompra, btnVolver, btnComprar);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
            // Load puede rechazar el acceso durante Show: impedir que quede visible al regresar.
            if (value && accesoRechazado) base.SetVisibleCore(false);
        }

        protected override void OnShown(EventArgs e)
        {
            if (!accesoRechazado) resumenWeb.Cargar("compra.html");
            if (accesoRechazado) return;
            base.OnShown(e);
            AjustarAreaVisible(Screen.FromControl(this).WorkingArea);
        }

        private void AjustarAreaVisible(System.Drawing.Rectangle areaTrabajo)
        {
            Disposicion.AjustarAreaVisible(this, areaTrabajo);
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
                accesoRechazado = true;
                VolverAlListado(ex.Message);
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
            if(SectorInicial.HasValue && sectores.ContainsKey(SectorInicial.Value)) ElegirSectorEnMapa(SectorInicial.Value);
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
            MarcarInscripto(GestorListaEspera.Instance.EstaInscripto(IDEvento, IDUsuario));
            ActualizarHabilitados();
        }

        private void ActualizarHabilitados()
        {
            bool hayEntradas = GestorPaquetes.Instance.CantidadEntradas(paquete) > 0;
            btnComprar.Enabled = hayEntradas && !agotado && !compraEnCurso && !compraFinalizada;
            chkSeguro.Enabled = hayEntradas && !agotado;
            if (!chkSeguro.Enabled && chkSeguro.Checked)
            {
                chkSeguro.Checked = false;
            }
        }

        // RF-10: después de anotarse el botón queda deshabilitado, diciendo que ya está en la lista.
        private void MarcarInscripto(bool inscripto)
        {
            lnkPosicionEspera.Visible = inscripto;
            btnListaEspera.Enabled = !inscripto;
            btnListaEspera.Text = inscripto ? "Ya estás en la lista de espera" : "Anotarme en la lista de espera";
        }

        private void lnkPosicionEspera_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int? posicion = GestorListaEspera.Instance.ConsultarPosicion(IDEvento, IDUsuario);
            Avisos.Informativo(this, posicion.HasValue ? "Tu posición actual en la lista es " + posicion.Value + "." :
                "No estás en la lista de espera de este evento.");
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
            Avisos.Informativo(this, "Te anotaste en la lista de espera de «" + evento.Nombre + "». " +
                            "Podés consultar tu posición. Si vuelve a haber entradas, te lo mostramos al iniciar sesión.");
        }
        // RF-06: reserva el cupo y pasa a la pantalla de pago.
        private void btnComprar_Click(object sender, EventArgs e)
        {
            if (compraEnCurso || compraFinalizada) return;
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
            compraEnCurso = true;
            btnComprar.Enabled = false;
            try
            {
                using (Avisos.Esperar(this, btnComprar))
                    compra = GestorCompras.Instance.Iniciar(paquete, cliente);
            }
            catch (ReglaNegocioException ex)
            {
                compraEnCurso = false;
                ActualizarHabilitados();
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
                compraEnCurso = false;
                ActualizarHabilitados();
                // H-02: venció el plazo; el cupo ya se liberó.
                VolverAlListado("La reserva venció o cambió. Volvé a armar la compra desde eventos.");
                return;
            }
            if (resultado != DialogResult.OK)
            {
                compraEnCurso = false;
                ActualizarHabilitados();
                RecargarDisponibles();
                if (resultado == DialogResult.Cancel)
                    Avisos.Informativo(this, "Cancelaste el pago. La reserva se liberó; podés volver a intentarlo.");
                return;
            }

            var confirmacion = CrearConfirmacion(compra);
            if (NavegacionCliente.Activa)
            {
                NavegacionCliente.Navegar(confirmacion);
                return;
            }

            using (confirmacion)
            {
                confirmacion.ShowDialog(this);
            }

            // U-09: el pedido ya terminó; vuelve al listado, sin un armado vacío.
            VolverAlListado();
        }

        internal frmConfirmacion CrearConfirmacion(Compra compra)
        {
            List<string> rutas;
            string avisoTicket = null;
            try
            {
                rutas = CrearTickets == null ? GenerarTicket(compra) : CrearTickets(compra);
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                rutas = new List<string>();
                avisoTicket = "El pago quedó confirmado como pedido " + frmConfirmacion.NumeroPedido(compra) +
                    ", pero no pudimos crear el PDF. Tus entradas y sus códigos se conservan en Mis entradas; " +
                    "podés consultar allí los códigos QR y recuperar el ticket. Revisá el acceso a Documentos y las dependencias del programa.";
            }
            compraFinalizada = true;
            compraEnCurso = false;
            return new frmConfirmacion { Compra = compra, Rutas = rutas, Carpeta = CarpetaTickets(), AvisoTicket = avisoTicket };
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
            ActualizarMapa();
        }

        private void ActualizarMapa()
        {
            mapaSectores.Cargar(sectores.Values, id =>
                Convert.ToInt32(dgvSectores.Rows.Cast<DataGridViewRow>().Single(f =>
                    Convert.ToInt32(f.Cells["IdSector"].Value) == id).Cells["Disponibles"].Value));
        }

        private void ElegirSectorEnMapa(int id)
        {
            var fila = dgvSectores.Rows.Cast<DataGridViewRow>().Single(f => Convert.ToInt32(f.Cells["IdSector"].Value) == id);
            dgvSectores.CurrentCell = fila.Cells["Cantidad"];
            if (Convert.ToInt32(fila.Cells["Cantidad"].Value) == 0) fila.Cells["Cantidad"].Value = 1;
            contenidoCompra.ScrollControlIntoView(dgvSectores);
            dgvSectores.Focus();
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
            Avisos.Informativo(this, texto.ToString(), 15000);
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
            dgvSectores.Columns["Cantidad"].DefaultCellStyle.BackColor = Tema.EncabezadoGrilla;

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
            ActualizarMapa();
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
                fila.Cells["Cantidad"].ErrorText = "Usá un número entero desde cero.";
                RestaurarCantidad(fila, anterior);
                return;
            }
            fila.Cells["Cantidad"].ErrorText = string.Empty;
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
                Avisos.Informativo(this, ex.Message);
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

            var filas = new List<(string Concepto, string Importe, bool Seccion)>();
            void Seccion(string nombre) => filas.Add((nombre, null, true));
            void Linea(string concepto) => filas.Add((concepto, null, false));
            void Importe(string concepto, decimal monto) => filas.Add((concepto, monto.ToString("C2", Pesos), false));

            int cantidad = GestorPaquetes.Instance.CantidadEntradas(paquete);
            Seccion($"Entradas ({cantidad})");
            if (paquete.Entradas.Count == 0) Linea("Sin entradas");
            foreach (Entrada entrada in paquete.Entradas)
            {
                Sector sector = sectores[entrada.IdSector.Value];
                Importe($"{entrada.CantidadVendida} x {sector.Nombre}", entrada.Precio * entrada.CantidadVendida);
            }
            Importe("Base entradas", resumen.BaseEntradas);
            Importe($"Desc. {resumen.NivelAplicado} {resumen.Porcentaje.ToString("P1", Pesos)}", -resumen.MontoDescuento);
            Importe("Subtotal entradas", resumen.SubtotalEntradas);

            Seccion("Servicios (sin descuento)");
            if (paquete.Servicios.Count == 0) Linea("Sin servicios");
            foreach (Servicio servicio in paquete.Servicios)
                Importe(servicio.Nombre, servicio.Precio);
            Importe("Subtotal servicios", resumen.Servicios);

            Seccion("Cargos");
            Importe("Cargo por servicio", resumen.CargoServicio);
            // RN-37: con suscripción activa el seguro sale sin cargo.
            string seguro = cantidad > 0 && resumen.Seguro == 0m ? "Seguro sin cargo" : "Seguro";
            if (paquete.ConSeguro) Importe($"{seguro} ({cantidad} entradas)", resumen.Seguro);
            else Linea("Seguro: no");

            string total = resumen.Total.ToString("C2", Pesos);
            lblResumen.Cargar(filas, total);
            resumenWeb.Actualizar(lblResumen.ModeloWeb());
            lblTotalDestacado.Text = "TOTAL " + total;
            AjustarEncabezado();
            ActualizarHabilitados();
        }

        // RN-33: importes en pesos argentinos.
        private static readonly System.Globalization.CultureInfo Pesos = new System.Globalization.CultureInfo("es-AR");

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
                // El nombre usa el ancho disponible sobre los datos y el QR para admitir varias líneas.
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
                    "Si volvés al listado se descarta el paquete que armaste. ¿Querés volver?",
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
                    "Si cerrás la ventana se descarta el paquete que armaste. ¿Querés cerrar?",
                    "Cerrar la compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (respuesta != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }

        private void VolverAlListado(string aviso = null)
        {
            frmEventosCliente frm = new frmEventosCliente();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            if (NavegacionCliente.AbrirDesdeMenu(frm))
            {
                if (!string.IsNullOrWhiteSpace(aviso)) Avisos.Informativo(frm, aviso);
                Hide();
                return;
            }
            frm.Show();
            if (!string.IsNullOrWhiteSpace(aviso)) Avisos.Informativo(frm, aviso);
            this.Hide();
        }

    }
}
