using System;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // RF-06: pago simulado de una compra ya iniciada (cupo reservado por RN-15).
    // Devuelve OK si se pagó, Abort si venció el plazo y Cancel si el cliente desistió.
    public partial class frmPago : FormularioEventAura
    {
        public Compra Compra;

        private static readonly System.Globalization.CultureInfo Pesos = new System.Globalization.CultureInfo("es-AR");

        private DateTime vence;
        private bool cerrada;   // pagada o vencida: ya no hay reserva que liberar al cerrar
        private bool procesandoPago;

        public frmPago()
        {
            InitializeComponent();
            Shown += (s, e) =>
            {
                ColocarCampo(lblTitular, txtTitular);
                int espacio = Disposicion.Pixeles(this, Tema.Espacio.XS);
                lblTotal.Top = lblTitular.Top - lblTotal.Height - espacio;
                lblSeguro.Top = lblTotal.Top - lblSeguro.Height - espacio;
                lblTitulo.Top = lblSeguro.Top - lblTitulo.Height - espacio;
                lblTemporizador.Top = lblTitulo.Top;
                indicadorPasos.Top = lblTitulo.Top - indicadorPasos.Height - espacio;
                ColocarCampo(lblNumero, txtNumero);
                int Px(int medida) => Disposicion.Pixeles(this, medida);
                txtCvv.Left = txtVencimiento.Left + Px(Tema.AnchoCampo + Tema.Espacio.S);
                ClientSize = new System.Drawing.Size(txtCvv.Left + Px(Tema.AnchoCampo + Tema.Espacio.XL), ClientSize.Height);
                ColocarCampo(lblVencimiento, txtVencimiento);
                ColocarCampo(lblCvv, txtCvv);
                Disposicion.AlinearGrupo(btnCancelar, btnPagar);
                Disposicion.LimitarAncho(lblEstado);
            };
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
                DialogResult = DialogResult.Abort;
                return;
            }
            lblTemporizador.Text = $"Reserva: {(int)restante.TotalMinutes:00}:{restante.Seconds:00}";
            lblTemporizador.ForeColor = Tema.ColorTemporizador(restante);
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            if (procesandoPago || cerrada) return;
            // RN-30: el número completo vive solo en este objeto, en memoria, durante el cobro.
            var tarjeta = new DatosTarjeta
            {
                Titular = txtTitular.Text,
                Numero = txtNumero.Text,
                Vencimiento = txtVencimiento.Text,
                Cvv = txtCvv.Text
            };

            bool aprobado;
            procesandoPago = true;
            btnPagar.Enabled = false;
            try
            {
                using (Avisos.Esperar(this, btnPagar))
                    aprobado = GestorCompras.Instance.Confirmar(Compra, tarjeta);
                procesandoPago = false;
                btnPagar.Enabled = true;
            }
            catch (ArgumentException ex)
            {
                procesandoPago = false;
                btnPagar.Enabled = true;
                lblEstado.Text = ex.Message;
                return;
            }
            catch (ReglaNegocioException ex)
            {
                tmrReserva.Stop();
                procesandoPago = false;
                btnPagar.Enabled = true;
                cerrada = ex.Regla == "RN-15";
                Avisos.Regla("Pago", ex);
                DialogResult = cerrada ? DialogResult.Abort : DialogResult.Cancel;
                return;
            }
            catch (InvalidOperationException)
            {
                // La compra dejó de estar pendiente por fuera de esta pantalla (por ejemplo, el evento
                // se eliminó mientras se pagaba). El cupo ya se liberó: se vuelve al listado.
                tmrReserva.Stop();
                procesandoPago = false;
                btnPagar.Enabled = false;
                cerrada = true;
                DialogResult = DialogResult.Abort;
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
                             "Se liberaron las entradas. Podés reintentar antes de que venza la reserva.";
            txtNumero.Clear();
            txtNumero.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
