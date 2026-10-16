using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // RF-07: compra exitosa con la identificación del pedido, lo pagado y dónde quedaron los PDF.
    public partial class frmConfirmacion : Form
    {
        public Compra Compra;
        public List<string> Rutas = new List<string>();
        public string Carpeta;

        private static readonly System.Globalization.CultureInfo Pesos = new System.Globalization.CultureInfo("es-AR");

        public frmConfirmacion()
        {
            InitializeComponent();
        }

        // Identificación del pedido que ve el cliente; también va impresa en cada ticket.
        public static string NumeroPedido(Compra compra)
        {
            return $"EA-{compra.Id:D6}";
        }

        private void frmConfirmacion_Load(object sender, EventArgs e)
        {
            lblPedido.Text = "Pedido " + NumeroPedido(Compra);

            // Los importes son los congelados en la fila de Compras: lo que efectivamente se cobró.
            decimal subtotalEntradas = Compra.BaseEntradas - Compra.MontoDescuento;
            var texto = new StringBuilder();
            texto.AppendLine(Linea("Base entradas", Compra.BaseEntradas));
            texto.AppendLine(Linea($"Desc. {Compra.NivelAplicado} {Compra.DescuentoAplicado.ToString("P1", Pesos)}", -Compra.MontoDescuento));
            texto.AppendLine(Linea("Subtotal entradas", subtotalEntradas));
            texto.AppendLine(Linea("Servicios", Compra.MontoServicios));
            texto.AppendLine(Linea("Cargo por servicio", Compra.CargoServicio));
            texto.AppendLine(Linea("Seguro", Compra.MontoSeguro));
            texto.AppendLine();
            texto.AppendLine(Linea("TOTAL PAGADO", Compra.TotalPagado));
            texto.AppendLine();

            // RF-06: tokenización simulada. Solo se conservan los últimos 4 dígitos (RN-30).
            Pago pago = GestorCompras.Instance.PagosDe(Compra).LastOrDefault(p => p.Estado == EstadoPago.Aprobado);
            if (pago != null)
            {
                texto.AppendLine($"Tarjeta •••• {pago.Ultimos4}");
                texto.AppendLine("Token de pago:");
                texto.AppendLine(pago.Token);
            }
            lblDesglose.UseMnemonic = false;
            lblDesglose.Text = texto.ToString();

            lblTickets.Text = Rutas.Count == 1
                ? "1 entrada en PDF, con su código QR:"
                : $"{Rutas.Count} entradas en PDF, cada una con su QR:";
            txtCarpeta.Text = Carpeta;
            lstArchivos.Items.Clear();
            foreach (string ruta in Rutas)
            {
                lstArchivos.Items.Add(System.IO.Path.GetFileName(ruta));
            }
            ActiveControl = btnAceptar;
        }

        private const int AnchoLinea = 40;

        private static string Linea(string concepto, decimal monto)
        {
            string importe = monto.ToString("C2", Pesos);
            int ancho = AnchoLinea - importe.Length - 1;
            if (concepto.Length > ancho)
            {
                concepto = concepto.Substring(0, ancho - 1) + "…";
            }
            return concepto.PadRight(ancho) + " " + importe;
        }

        private void btnAbrirCarpeta_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Carpeta) && System.IO.Directory.Exists(Carpeta))
            {
                Process.Start("explorer.exe", "\"" + Carpeta + "\"");
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
