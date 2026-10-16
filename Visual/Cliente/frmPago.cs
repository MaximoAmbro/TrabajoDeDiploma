using System;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // RF-06: pago simulado de una compra ya iniciada (cupo reservado por RN-15).
    // Devuelve OK si se pagó, Abort si venció el plazo y Cancel si el cliente desistió.
    public partial class frmPago : Form
    {
        public Compra Compra;

        private static readonly System.Globalization.CultureInfo Pesos = new System.Globalization.CultureInfo("es-AR");

        private DateTime vence;
        private bool cerrada;   // pagada o vencida: ya no hay reserva que liberar al cerrar

        public frmPago()
        {
            InitializeComponent();
        }

        private void frmPago_Load(object sender, EventArgs e)
        {
            vence = GestorCompras.Instance.VencimientoDe(Compra);
            lblTotal.Text = "Total a pagar: " + Compra.TotalPagado.ToString("C2", Pesos);
            lblEstado.Text = "";
            ActualizarTemporizador();
            tmrReserva.Start();
            ActiveControl = txtTitular;
        }

        // H-02: cuenta regresiva de la reserva, siempre a la vista en el encabezado.
        private void tmrReserva_Tick(object sender, EventArgs e)
        {
            ActualizarTemporizador();
        }

        private void ActualizarTemporizador()
        {
            TimeSpan restante = vence - DateTime.Now;
            if (restante <= TimeSpan.Zero)
            {
                tmrReserva.Stop();
                lblTemporizador.Text = "Reserva: 00:00";
                GestorCompras.Instance.LiberarVencidas();
                cerrada = true;
                MessageBox.Show("Venció el plazo de la reserva y se liberaron las entradas. " +
                                "Vuelva a armar la compra desde el listado.",
                                "Reserva vencida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Abort;
                return;
            }
            lblTemporizador.Text = $"Reserva: {(int)restante.TotalMinutes:00}:{restante.Seconds:00}";
            lblTemporizador.ForeColor = restante.TotalMinutes < 1
                ? System.Drawing.Color.Firebrick
                : System.Drawing.Color.FromArgb(116, 86, 174);
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            // RN-30: el número completo vive solo en este objeto, en memoria, durante el cobro.
            var tarjeta = new DatosTarjeta
            {
                Titular = txtTitular.Text,
                Numero = txtNumero.Text,
                Vencimiento = txtVencimiento.Text,
                Cvv = txtCvv.Text
            };

            bool aprobado;
            try
            {
                aprobado = GestorCompras.Instance.Confirmar(Compra, tarjeta);
            }
            catch (ArgumentException ex)
            {
                lblEstado.Text = ex.Message;
                return;
            }
            catch (ReglaNegocioException ex)
            {
                tmrReserva.Stop();
                cerrada = ex.Regla == "RN-15";
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = cerrada ? DialogResult.Abort : DialogResult.Cancel;
                return;
            }
            finally
            {
                txtCvv.Clear();
            }

            if (aprobado)
            {
                tmrReserva.Stop();
                cerrada = true;
                txtNumero.Clear();
                DialogResult = DialogResult.OK;
                return;
            }

            // RN-32: el cupo ya se liberó. Se puede reintentar dentro del plazo.
            Pago rechazo = GestorCompras.Instance.PagosDe(Compra).LastOrDefault();
            lblEstado.Text = "Pago rechazado: " + rechazo?.Motivo + "\n" +
                             "Se liberaron las entradas. Puede reintentar antes de que venza la reserva.";
            txtNumero.Clear();
            txtNumero.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show(
                "Si cancela el pago se liberan las entradas reservadas. ¿Desea cancelar?",
                "Cancelar pago", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta == DialogResult.Yes)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        // Cerrar sin pagar (Cancelar o la X) libera el cupo en el acto (RN-32).
        private void frmPago_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrReserva.Stop();
            if (!cerrada)
            {
                GestorCompras.Instance.Liberar(Compra);
                cerrada = true;
            }
        }
    }
}
