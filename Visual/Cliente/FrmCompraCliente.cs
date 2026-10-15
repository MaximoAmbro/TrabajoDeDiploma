
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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa.Checker;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Repositorio;
using Negocio;
using Entidades;
using Microsoft.Identity.Client;

namespace Visual
{
    public partial class FrmCompraCliente : Form
    {
        public int IDUsuario;
        public int IDEvento;
        public SqlConnection ConexionSql; // TODO Seccion 7: quitar ConexionSql (se devuelve a frmEventosCliente)
        public string NombreUsuario;
        GestorEventos GestorEventos = new GestorEventos();

        // RF-05: lo que el cliente va armando. Vive en memoria hasta confirmar la compra.
        private Paquete paquete;
        private Evento evento;
        private Dictionary<int, Sector> sectores = new Dictionary<int, Sector>();
        private bool cargandoGrilla;

        public FrmCompraCliente()
        {
            InitializeComponent();
        }
        private void FrmCompraCliente_Load(object sender, EventArgs e)
        {
            try
            {
                paquete = GestorPaquetes.Instance.Nuevo(IDEvento);
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                VolverAlListado();
                return;
            }
            evento = GestorEventos.Instance.ObtenerPorId(IDEvento);
            label1.Text = evento.Nombre;

            CargarNivel();
            CargarSectores();
            CargarServicios();
            chkSeguro.CheckedChanged += chkSeguro_CheckedChanged;
            ActualizarResumen();
        }
        private void btnComprar_Click(object sender, EventArgs e)
        {
            try
            {
                GestorPaquetes.Instance.Validar(paquete);
                GenerarTicket(); CargarEntradas();
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Compra realizada con exito," + "revise documentos para recibir su entrada");
            paquete = GestorPaquetes.Instance.Nuevo(IDEvento);
            CargarSectores();
            foreach (CheckBox chk in ChecksDeServicio())
            {
                chk.Checked = false;
            }
            chkSeguro.Checked = false;
            ActualizarResumen();
        }

        // --------------------------------------        FUNCIONES          --------------------------------------

        // TODO Seccion 7: flujo provisorio, sin reserva, pago ni QR guardado.
        private void CargarEntradas()
        {
            GestorPaquetes.Instance.RegistrarEntradas(paquete, IDUsuario);
        }

        private void CargarNivel()
        {
            NivelFidelidad nivel = GestorPaquetes.Instance.NivelDe(IDUsuario);
            decimal porcentaje = GestorPaquetes.Instance.PorcentajeDescuento(nivel);
            lblNivel.Text = $"Tu nivel: {nivel} · {porcentaje:P1} de descuento sobre las entradas";
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

        // RF-05: desglose del paquete. Todavía sin descuento por nivel ni cargo por servicio (Sección 6).
        private void ActualizarResumen()
        {
            var texto = new StringBuilder();
            texto.AppendLine("RESUMEN DEL PAQUETE");
            texto.AppendLine();
            texto.AppendLine("Entradas");
            if (paquete.Entradas.Count == 0)
            {
                texto.AppendLine("  (ninguna)");
            }
            foreach (Entrada entrada in paquete.Entradas)
            {
                Sector sector = sectores[entrada.IdSector.Value];
                texto.AppendLine($"  {entrada.CantidadVendida} x {sector.Nombre}");
                texto.AppendLine(Importe("", entrada.Precio * entrada.CantidadVendida));
            }
            texto.AppendLine(Importe("Subtotal entradas", GestorPaquetes.Instance.SubtotalEntradas(paquete)));
            texto.AppendLine();
            texto.AppendLine("Servicios");
            if (paquete.Servicios.Count == 0)
            {
                texto.AppendLine("  (ninguno)");
            }
            foreach (Servicio servicio in paquete.Servicios)
            {
                texto.AppendLine($"  {servicio.Nombre}");
                texto.AppendLine(Importe("", servicio.Precio));
            }
            texto.AppendLine(Importe("Subtotal servicios", GestorPaquetes.Instance.SubtotalServicios(paquete)));
            texto.AppendLine();
            texto.AppendLine("Seguro de entrada: " + (paquete.ConSeguro ? "sí, se cotiza al pagar" : "no"));
            texto.AppendLine();
            decimal total = GestorPaquetes.Instance.SubtotalEntradas(paquete) + GestorPaquetes.Instance.SubtotalServicios(paquete);
            texto.AppendLine(Importe("TOTAL PARCIAL", total));
            texto.AppendLine();
            texto.AppendLine("Sin descuento por nivel ni cargo");
            texto.AppendLine("por servicio: se suman al pagar.");
            texto.AppendLine($"Entradas: {GestorPaquetes.Instance.CantidadEntradas(paquete)}");
            lblResumen.Text = texto.ToString();
        }

        // RN-33: importes en pesos argentinos.
        private static readonly System.Globalization.CultureInfo Pesos = new System.Globalization.CultureInfo("es-AR");

        private static string Importe(string concepto, decimal monto)
        {
            return concepto.PadRight(20) + monto.ToString("C2", Pesos).PadLeft(18);
        }

        // H-06: confirma antes de descartar lo cargado.
        private bool PaqueteVacio()
        {
            return paquete == null
                || (paquete.Entradas.Count == 0 && paquete.Servicios.Count == 0 && !paquete.ConSeguro);
        }

        public void GenerarTicket()
        {
            string carpetaTickets = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\A1.Tickets EVENTAURA";
            if (!Directory.Exists(carpetaTickets))
            {
                Directory.CreateDirectory(carpetaTickets);
            }

            string fechaevento = evento.Fecha.ToString("dd/MM/yyyy");
            string marca = DateTime.Now.ToString("yyyyMMddHHmmss");
            int CantidadTickets = 0;
            foreach (Entrada entrada in paquete.Entradas)
            {
                Sector sector = sectores[entrada.IdSector.Value];
                string PrecioSector = entrada.Precio.ToString("C2", Pesos);
                for (int i = 0; i < entrada.CantidadVendida; i++)
                {
                    CantidadTickets++;
                    string Ruta = System.IO.Path.Combine(carpetaTickets, $"Ticket_{evento.Nombre}_{sector.Nombre}_{marca}_{CantidadTickets}.PDF");
                    GestorEventos.GenerarTicket(evento.Nombre, sector.Nombre, PrecioSector, fechaevento);
                    GenerarPDF(Ruta, GestorEventos.MensajeTicket, GestorEventos.QRCodeImage);
                }
            }
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
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }

    }
}
