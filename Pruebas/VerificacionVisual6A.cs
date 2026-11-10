using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Pruebas
{
    // Herramienta de evidencia, no un caso MSTest. Se compila como exe con el manifiesto de
    // Visual para medir los DPI reales en procesos nuevos. Solo usa la base _Pruebas y Fake.
    internal static class VerificacionVisual6A
    {
        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr ventana);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr ventana, int modo);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr ventana);

        private static object Dato(string metodo, params object[] argumentos)
        {
            return Assembly.Load("Pruebas").GetType("Pruebas.Fake")
                .GetMethod(metodo, BindingFlags.Static | BindingFlags.Public).Invoke(null, argumentos);
        }

        private static T ControlDe<T>(Form f, string nombre) where T : Control
        {
            return (T)f.Controls.Find(nombre, true).Single();
        }

        [STAThread]
        private static int Main(string[] argumentos)
        {
            string cadena = ConfigurationManager.ConnectionStrings[BaseDePrueba.NombreCadena].ConnectionString;
            if (!new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cadena).InitialCatalog
                .EndsWith(BaseDePrueba.SufijoObligatorio, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("La evidencia solo usa la base de pruebas.");
            string carpeta = argumentos[0];
            Directory.CreateDirectory(carpeta);
            Application.EnableVisualStyles();
            Dato("Limpiar");
            try
            {
                Cliente cliente = (Cliente)Dato("Cliente", NivelFidelidad.Bronce);
                if (argumentos.Length > 1 && argumentos[1] == "tickets")
                {
                    CrearTickets(carpeta, cliente);
                    return 0;
                }
                return MedirPiloto(carpeta, cliente);
            }
            finally { Dato("Limpiar"); }
        }

        private static void CrearTickets(string carpeta, Cliente cliente)
        {
            Evento evento = GestorEventos.Instance.ObtenerPorId(1);
            Compra compra = (Compra)Dato("CompraConfirmada", cliente, evento, 2, false);
            string pedido = Visual.frmConfirmacion.NumeroPedido(compra);
            int numero = 0;
            using (var form = new Visual.FrmCompraCliente())
            {
                foreach (Entrada entrada in GestorCompras.Instance.EntradasDe(compra))
                {
                    Sector sector = GestorEventos.Instance.Sectores(evento.Id).Single(s => s.Id == entrada.IdSector);
                    string mensaje = "ENTRADA " + evento.Nombre + "\nSECTOR: " + sector.Nombre +
                        "\nFECHA: " + evento.Fecha.ToString("dd/MM/yyyy HH:mm") +
                        "\nPRECIO: " + entrada.Precio.ToString("C2", new CultureInfo("es-AR")) +
                        "\nPEDIDO: " + pedido + "\nCÓDIGO: " + entrada.CodigoQR;
                    form.GenerarPDF(Path.Combine(carpeta, "Ticket-nuevo-" + (++numero) + ".pdf"),
                        mensaje, GeneradorQR.Instance.Imagen(entrada.CodigoQR));
                }
            }
        }

        private static int MedirPiloto(string carpeta, Cliente cliente)
        {
            using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
            {
                form.Show();
                ShowWindow(form.Handle, 5);
                SetForegroundWindow(form.Handle);
                Application.DoEvents();
                ControlDe<Label>(form, "label1").Text =
                    "Gran Festival Internacional de Música Independiente de Rosario con artistas nacionales e invitados VIP";
                foreach (DataGridViewRow fila in ControlDe<DataGridView>(form, "dgvSectores").Rows)
                    fila.Cells["Cantidad"].Value = 2;
                foreach (string nombre in new[] { "chkHospedaje", "chkTraslado", "chkMerch", "chkVIP", "chkSeguro" })
                    ControlDe<CheckBox>(form, nombre).Checked = true;
                Application.DoEvents();
                var contenido = ControlDe<Panel>(form, "pnlContenidoCompra");
                var resumen = ControlDe<Panel>(form, "pnlResumen");
                var documento = ControlDe<Label>(form, "lblResumen");
                var pie = ControlDe<Panel>(form, "pnlAccionesCompra");
                var titulo = ControlDe<Label>(form, "label1");
                uint dpi = GetDpiForWindow(form.Handle);
                string prefijo = (dpi * 100 / 96).ToString(CultureInfo.InvariantCulture);
                contenido.AutoScrollPosition = Point.Empty;
                Capturar(form, Path.Combine(carpeta, "piloto-" + prefijo + "-inicio.png"));
                contenido.ScrollControlIntoView(resumen);
                resumen.AutoScrollPosition = new Point(0, resumen.VerticalScroll.Maximum);
                Application.DoEvents();
                bool finTarjeta = resumen.Bottom <= contenido.ClientSize.Height;
                bool finDocumento = documento.Bottom <= resumen.ClientSize.Height;
                bool sinSuperposicion = !contenido.Bounds.IntersectsWith(pie.Bounds);
                bool tituloCompleto = titulo.GetPreferredSize(new Size(titulo.Width, 0)).Height <= titulo.Height;
                bool acciones = new[] { "btnComprar", "btnVolver" }.All(n =>
                {
                    Button boton = ControlDe<Button>(form, n);
                    return form.ClientRectangle.Contains(form.RectangleToClient(boton.Parent.RectangleToScreen(boton.Bounds)));
                });
                bool lineas = documento.Text.Split('\n').All(linea =>
                    TextRenderer.MeasureText(linea.TrimEnd('\r'), documento.Font, Size.Empty, TextFormatFlags.NoPadding).Width
                    <= documento.Width - documento.Padding.Horizontal);
                var datos = new List<string>
                {
                    "DPI=" + dpi, "Ventana=" + form.Bounds, "Vista=" + contenido.ClientSize,
                    "Tarjeta=" + resumen.Bounds, "Documento=" + documento.Bounds,
                    "FinTarjetaVisible=" + finTarjeta, "FinDocumentoVisible=" + finDocumento,
                    "SinSuperposicion=" + sinSuperposicion, "SinBarraHorizontal=" + !contenido.HorizontalScroll.Visible,
                    "TituloCompleto=" + tituloCompleto, "AccionesVisibles=" + acciones, "LineasCompletas=" + lineas
                };
                File.WriteAllLines(Path.Combine(carpeta, "piloto-" + prefijo + ".txt"), datos);
                Capturar(form, Path.Combine(carpeta, "piloto-" + prefijo + "-resumen.png"));
                return finTarjeta && finDocumento && sinSuperposicion && tituloCompleto && acciones && lineas &&
                    !contenido.HorizontalScroll.Visible ? 0 : 1;
            }
        }

        private static void Capturar(Form form, string ruta)
        {
            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                // Captura de pantalla real: DrawToBitmap altera el orden de hermanos
                // superpuestos y no sirve para juzgar la visibilidad del encabezado.
                form.Refresh();
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
                using (Graphics grafico = Graphics.FromImage(bitmap))
                    grafico.CopyFromScreen(form.Location, Point.Empty, bitmap.Size);
                bitmap.Save(ruta);
            }
        }
    }
}
