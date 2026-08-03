
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
        public string NombreEvento;
        public int IDUsuario;
        public int IDEvento;
        public SqlConnection ConexionSql;
        public string NombreUsuario;
        public DateTime FechaEvento;
        public int CantidadTotal = 0;
        public int Precio = 0;
        public int PrecioA, PrecioB, PrecioC;
        GestorEventos GestorEventos = new GestorEventos();
        public FrmCompraCliente()
        {
            frmEventosCliente frm = new frmEventosCliente();

            InitializeComponent();
        }
        private void FrmCompraCliente_Load(object sender, EventArgs e)
        {
            CargarSectores();
        }
        private void btnComprar_Click(object sender, EventArgs e)
        {
            GenerarTicket(); CargarEntradas();
            MessageBox.Show("Compra realizada con exito," + "revise documentos para recibir su entrada");
        }
        
        // --------------------------------------        FUNCIONES          --------------------------------------
        private void CargarEntradas()
        {
            string insert = "INSERT INTO Entradas (ID_Usuario, ID_Evento, Precio, FechaVenta, CantidadVendida) VALUES (@IDUsuario, @IDEvento,@Precio, @FechaVenta, @CantidadVendida)";
            using (SqlCommand cmdInsert = new SqlCommand(insert, ConexionSql))
            {
                cmdInsert.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                cmdInsert.Parameters.AddWithValue("@IDEvento", IDEvento);
                cmdInsert.Parameters.AddWithValue("@FechaVenta", FechaEvento);
                cmdInsert.Parameters.AddWithValue("@CantidadVendida", CantidadTotal);
                cmdInsert.Parameters.AddWithValue("@Precio", Precio);


                ConexionSql.Open();
                cmdInsert.ExecuteNonQuery();
                ConexionSql.Close();
            }
        }
        private void CargarSectores()
        {
            string consulta = "SELECT Nombre, Precio FROM Sectores WHERE EventoID = @ID_Evento AND PrecioID = @PrecioID";

            using (SqlConnection conexion = new SqlConnection(ConexionSql.ConnectionString))
            {
                conexion.Open();

                for (int i = 1; i <= 3; i++)
                {
                    using (SqlCommand sqlcomando = new SqlCommand(consulta, conexion))
                    {
                        sqlcomando.Parameters.AddWithValue("@ID_Evento", IDEvento);
                        sqlcomando.Parameters.AddWithValue("@PrecioID", i);

                        using (SqlDataReader lector = sqlcomando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                string nombre = lector["Nombre"].ToString();
                                string precio = lector["Precio"].ToString() + " $";
                                int PrecioSeleccionado = Convert.ToInt32(lector["Precio"]);

                                if (i == 1)
                                {
                                    lblSectorA.Text = nombre;
                                    lblPrecioA.Text = precio;
                                    PrecioA = PrecioSeleccionado;
                                }
                                else if (i == 2)
                                {
                                    lblSectorB.Text = nombre;
                                    lblPrecioB.Text = precio;
                                    PrecioB = PrecioSeleccionado;
                                }
                                else if (i == 3)
                                {
                                    lblSectorC.Text = nombre;
                                    lblPrecioC.Text = precio;
                                    PrecioC = PrecioSeleccionado;
                                }
                            }
                            else
                            {
                                if (i == 1)
                                {
                                    lblSectorA.Text = "Sector A";
                                    lblPrecioA.Text = "Precio A";
                                }
                                else if (i == 2)
                                {
                                    lblSectorB.Text = "Sector B";
                                    lblPrecioB.Text = "Precio B";
                                }
                                else if (i == 3)
                                {
                                    lblSectorC.Text = "Sector C";
                                    lblPrecioC.Text = "Precio C";
                                }
                            }
                        }
                    }
                }
            }
        }
        public void GenerarTicket()
        {

            int cantidadA = Convert.ToInt32(NumPrimero.Text);
            int CantidadB = Convert.ToInt32(NumSegundo.Text);
            int CantidadC = Convert.ToInt32(NumTercero.Text);
            int CantidadTickets = 0;
            CantidadTotal = cantidadA + CantidadB + CantidadC;
            string carpetaTickets = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\A1.Tickets EVENTAURA";
            if (!Directory.Exists(carpetaTickets))
            {
                Directory.CreateDirectory(carpetaTickets);
            }

            for (int i = 0; i <= CantidadTotal; i++)
            {
                string SectorNombre = ""; string PrecioSector = ""; 
                // Rescata el nombre del evento
                string consulta = "SELECT Nombre FROM Eventos WHERE ID = @ID_Evento";
                using (SqlCommand sqlcomando = new SqlCommand(consulta, ConexionSql))
                {
                    sqlcomando.Parameters.Clear();
                    sqlcomando.Parameters.AddWithValue("@ID_Evento", IDEvento);
                    ConexionSql.Open();
                    object result = sqlcomando.ExecuteScalar();
                    NombreEvento = result.ToString();
                    ConexionSql.Close();
                }
                if (cantidadA > 0)
                {
                    SectorNombre = lblSectorA.Text;
                    PrecioSector = lblPrecioA.Text;
                    Precio = PrecioA;
                    Precio = PrecioA;
                }
                else if (CantidadB > 0)
                {
                    SectorNombre = lblSectorB.Text;
                    PrecioSector = lblPrecioB.Text;
                    Precio = PrecioB;
                }
                else if (CantidadC > 0)
                {
                    SectorNombre = lblSectorC.Text;
                    PrecioSector = lblPrecioC.Text;
                    Precio = PrecioC;
                }
                // Rescata la fecha del evento
                string fechaevento = "";
                string consulta2 = "SELECT Fecha FROM Eventos WHERE ID = @ID_Evento";
                using (SqlCommand sqlcomando = new SqlCommand(consulta2, ConexionSql))
                {
                    sqlcomando.Parameters.AddWithValue("@ID_Evento", IDEvento);
                    ConexionSql.Open();
                    object result = sqlcomando.ExecuteScalar();
                    ConexionSql.Close();

                    if (result != null && result != DBNull.Value)
                    {
                        FechaEvento = Convert.ToDateTime(result);
                        fechaevento = FechaEvento.ToString("dd/MM/yyyy");
                    }
                }

                string Ruta = System.IO.Path.Combine(carpetaTickets, $"Ticket_{NombreEvento}_{lblSectorA.Text}_{CantidadTickets}.PDF");
                CantidadTickets++;
                GestorEventos.GenerarTicket(NombreEvento, SectorNombre, PrecioSector, fechaevento);
                GenerarPDF(Ruta, GestorEventos.MensajeTicket, GestorEventos.QRCodeImage);
                cantidadA--;
                CantidadTotal--;
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
            frmEventosCliente frm = new frmEventosCliente();
            frm.IDUsuario = IDUsuario;
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }

    }
}

