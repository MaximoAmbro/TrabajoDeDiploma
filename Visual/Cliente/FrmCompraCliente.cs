
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
    public partial class FrmCompraCliente : Form
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

        public FrmCompraCliente()
        {
            InitializeComponent();
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
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            lblResumen.UseMnemonic = false;   // «Meet & Greet»: el & no es tecla de acceso
            MostrarSiAgotado();
            ActualizarResumen();
        }

        // RN-10: con el evento agotado, en lugar de la grilla de sectores se ofrece la lista de espera.
        private void MostrarSiAgotado()
        {
            bool agotado = GestorEventos.Instance.EstaAgotado(IDEvento);
            dgvSectores.Visible = !agotado;
            lblAgotado.Visible = agotado;
            btnListaEspera.Visible = agotado;
            btnComprar.Enabled = !agotado;
            chkSeguro.Enabled = !agotado;
            if (agotado)
            {
                foreach (CheckBox chk in ChecksDeServicio())
                {
                    chk.Enabled = false;
                }
                MarcarInscripto(GestorListaEspera.Instance.EstaInscripto(IDEvento, IDUsuario));
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
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            lnkNiveles.Left = lblNivel.Right + 2;
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
            dgvSectores.BackgroundColor = System.Drawing.Color.White;
            dgvSectores.EditMode = DataGridViewEditMode.EditOnEnter;

            dgvSectores.Columns.Add("IdSector", "IdSector");
            dgvSectores.Columns.Add("Sector", "Sector");
            dgvSectores.Columns.Add("Precio", "Precio");
            dgvSectores.Columns.Add("Disponibles", "Disponibles");
            dgvSectores.Columns.Add("Cantidad", "Cantidad");
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
            dgvSectores.Columns["Cantidad"].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 241, 252);

            sectores.Clear();
            foreach (Sector sector in GestorEventos.Instance.Sectores(IDEvento))
            {
                sectores[sector.Id] = sector;
                dgvSectores.Rows.Add(sector.Id, sector.Nombre, sector.Precio,
                    GestorPaquetes.Instance.DisponiblesDe(sector.Id), 0);
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
            if (!int.TryParse(string.IsNullOrWhiteSpace(texto) ? "0" : texto.Trim(), out cantidad))
            {
                MessageBox.Show("La cantidad debe ser un número entero.");
                RestaurarCantidad(fila, anterior);
                return;
            }
            try
            {
                GestorPaquetes.Instance.AgregarEntradas(paquete, idSector, cantidad);
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RestaurarCantidad(fila, anterior);
                return;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    : $"{par.Value} {servicio.Precio.ToString("C0", Pesos)}";
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
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            PdfWriter pw = new PdfWriter(ruta);
            PdfDocument pdf = new PdfDocument(pw);
            // aca elegimos el tamaño de la pagina
            Document doc = new Document(pdf, PageSize.A6.Rotate());
            // aca seteamos el color de la pagina
            PdfPage page = pdf.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.BLACK);
            canvas.Rectangle(0, 0, page.GetPageSize().GetWidth(), page.GetPageSize().GetHeight());
            canvas.Fill();

            // aca seteamos el color del texto
            doc.Add(new Paragraph(mensaje)
                .SetTextAlignment(TextAlignment.CENTER)
               .SetFontColor(ColorConstants.WHITE)
               .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
               .SetFontSize(10))
               .SetBottomMargin(50);

            // aca se agrega el QR y se setea tamaño y ubicacion
            var img = new iText.Layout.Element.Image(ImageDataFactory.Create(QR));
            img.ScaleAbsolute(100, 100);
            img.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
            doc.Add(img);

            doc.Close();
        }
        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (!PaqueteVacio())
            {
                DialogResult respuesta = MessageBox.Show(
                    "Si vuelve al listado se descarta el paquete que armó. ¿Desea volver?",
                    "Volver al listado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta != DialogResult.Yes)
                {
                    return;
                }
            }
            VolverAlListado();
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
