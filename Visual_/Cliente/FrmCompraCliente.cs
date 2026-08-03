using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Entidades;
using iText;
using iText.IO.Image;
using iText.Kernel;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using QRCoder;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Pdfa.Checker;

namespace Visual
{
    public partial class FrmCompraCliente : Form
    {
        public string NombreEvento { get; set; }
        public string NombreUsuario { get; set; }
        public string Mail { get; set; }
        GestorEventos gestorE = new GestorEventos();
        public FrmCompraCliente()
        {
            frmEventosCliente frm = new frmEventosCliente();

            InitializeComponent();
        }
        private void FrmCompra_Load(object sender, EventArgs e)
        {
            label1.Text = NombreEvento;
            string SectorA, SectorB, SectorC, PrecioA, PrecioB, PrecioC;
            GestorEventos gestor = new GestorEventos();
            gestor.BuscarLista(NombreEvento);
            SectorA = gestor.SectorA.ToString();
            SectorB = gestor.SectorB.ToString();
            SectorC = gestor.SectorC.ToString();
            PrecioA = gestor.PrecioA.ToString();
            PrecioB = gestor.PrecioB.ToString();
            PrecioC = gestor.PrecioC.ToString();
            lblSectorA.Text = SectorA;
            lblSectorB.Text = SectorB;
            lblSectorC.Text = SectorC;
            lblPrecioA.Text = PrecioA + "$";
            lblPrecioB.Text = PrecioB + "$";
            lblPrecioC.Text = PrecioC + "$";
        }
        private void btnVolver_Click_1(object sender, EventArgs e)
        {
            frmEventosCliente frm = new frmEventosCliente();
            frm.Mail = Mail;
            frm.Show();
            this.Hide();
        }
        private void btnComprar_Click(object sender, EventArgs e)
        {
            int cantidadA = Convert.ToInt32(NumPrimero.Text);
            int CantidadB = Convert.ToInt32(NumSegundo.Text);
            int CantidadC = Convert.ToInt32(NumTercero.Text);
            int CantidadTotal = cantidadA + CantidadB + CantidadC;
            CargarEntrada();
            GenerarTicket();
            MessageBox.Show("Compra realizada con exito,"+ "revise documentos para recibir su entrada");
        }
        public void GenerarTicket()
        {
            GestorEventos gestor = new GestorEventos();
            int cantidadA = Convert.ToInt32(NumPrimero.Text);
            int CantidadB = Convert.ToInt32(NumSegundo.Text);
            int CantidadC = Convert.ToInt32(NumTercero.Text);
            int CantidadTickets = 0;
            int CantidadTotal = cantidadA + CantidadB + CantidadC;
            string carpetaTickets = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\A1.Tickets EVENTAURA";
            if (!Directory.Exists(carpetaTickets))
            {
                Directory.CreateDirectory(carpetaTickets);
            }

            for (int i = 0; i <= CantidadTotal; i++)
            {
                if (cantidadA > 0)
                {
                    gestor.GenerarTicket(NombreEvento, lblSectorA.Text);
                    string Ruta = System.IO.Path.Combine(carpetaTickets, $"Ticket_{NombreEvento}_{lblSectorA.Text}_{CantidadTickets}.PDF" );
                    CantidadTickets++;
                    GenerarPDF(Ruta, gestor.MensajeTicket, gestor.QRCodeImage);
                    cantidadA--;
                    CantidadTotal--;
                }
                if (CantidadB > 0)
                {
                    gestor.GenerarTicket(NombreEvento, lblSectorB.Text);
                    string Ruta = System.IO.Path.Combine(carpetaTickets, $"Ticket_{NombreEvento}_{lblSectorB.Text}_{CantidadTickets}.PDF");
                    CantidadTickets++;
                    GenerarPDF(Ruta, gestor.MensajeTicket, gestor.QRCodeImage);
                    CantidadB--;
                    CantidadTotal--;
                }
                if (CantidadC>0)
                {
                    gestor.GenerarTicket(NombreEvento, lblSectorC.Text);
                    string Ruta =  System.IO.Path.Combine(carpetaTickets, $"Ticket_{NombreEvento}_{lblSectorC.Text}_{CantidadTickets}.PDF");
                    CantidadTickets++;
                    GenerarPDF(Ruta, gestor.MensajeTicket, gestor.QRCodeImage);
                    CantidadC--;
                    CantidadTotal--;
                }
            }
        }
        public void GenerarPDF(string ruta, string mensaje, byte[] QR )
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
        public void CargarEntrada()
        {
            List<Evento> listaEventos = gestorE.ObtenerListaEventos();

            foreach (Evento _evento in listaEventos)
            {
                if (_evento.Nombre == NombreEvento)
                {
                    GestorClientes.Instance.AgregarEntrada(_evento, Mail);
                    break;
                }
            }
        }
    }
}
