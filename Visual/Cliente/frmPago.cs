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
        private bool ajustandoVencimiento;
        private readonly Label ayudaDatosPrueba = new Label
        {
            Name = "ayudaDatosPruebaPago",
            AutoSize = false,
            Text = "Acepta 13 a 19 dígitos; vencimiento MM/AA no vencido; código de 3 o 4 dígitos. Ejemplo: 4111 1111 1111 1111 · 12/29 · 123. Una tarjeta terminada en 0000 simula rechazo.",
            ForeColor = Tema.TextoSecundario
        };

        public frmPago()
        {
            InitializeComponent();
            lblSeguro.Text = "Prototipo: usá estos datos de prueba";
            Controls.Add(ayudaDatosPrueba);
            txtVencimiento.TextChanged += txtVencimiento_TextChanged;
            Shown += (s, e) =>
            {
                int Px(int medida) => Disposicion.Pixeles(this, medida);
                txtTitular.Left = txtNumero.Left = txtVencimiento.Left = Disposicion.Pixeles(this, Tema.Espacio.XXL);
                lblTitulo.Top = Px(108);
                lblTemporizador.Top = lblTitulo.Top;
                lblSeguro.SetBounds(txtTitular.Left, lblTitulo.Bottom + Px(Tema.Espacio.S), txtTitular.Width, Px(Tema.Espacio.L));
                lblSeguro.AutoSize = false;
                lblSeguro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                ayudaDatosPrueba.Font = Tema.Ayuda;
                ayudaDatosPrueba.SetBounds(txtTitular.Left, lblSeguro.Bottom + Px(Tema.Espacio.XS), txtTitular.Width, 0);
                ayudaDatosPrueba.Height = ayudaDatosPrueba.GetPreferredSize(new System.Drawing.Size(ayudaDatosPrueba.Width, 0)).Height;
                lblTotal.Top = ayudaDatosPrueba.Bottom + Px(Tema.Espacio.S);
                txtTitular.Top = lblTotal.Bottom + Px(Tema.Espacio.L) + lblTitular.Height + Px(Tema.Espacio.XS);
                ColocarCampo(lblTitular, txtTitular);
                lblTemporizador.Top = lblTitulo.Top;
                indicadorPasos.Top = lblTitulo.Top - indicadorPasos.Height - Px(Tema.Espacio.XS);
                ColocarCampo(lblNumero, txtNumero);
                txtCvv.Left = txtVencimiento.Left + Px(Tema.AnchoCampo + Tema.Espacio.L);
                ClientSize = new System.Drawing.Size(txtCvv.Left + Px(Tema.AnchoCampo + Tema.Espacio.XXL), ClientSize.Height);
                ColocarCampo(lblVencimiento, txtVencimiento);
                ColocarCampo(lblCvv, txtCvv);
                Disposicion.AlinearAccionesFormulario(this, txtTitular, btnPagar, btnCancelar);
                lblEstado.Top = txtCvv.Bottom + Px(Tema.Espacio.S);
                btnPagar.Top = btnCancelar.Top = lblEstado.Bottom + Px(Tema.Espacio.L);
                ClientSize = new System.Drawing.Size(ClientSize.Width, Math.Max(ClientSize.Height, btnPagar.Bottom + Px(Tema.Espacio.XXL)));
                Disposicion.LimitarAncho(lblEstado);
            };
        }

        private void txtVencimiento_TextChanged(object sender, EventArgs e)
        {
            if (ajustandoVencimiento) return;
            string anterior = txtVencimiento.Text;
            int posicion = txtVencimiento.SelectionStart;
            int digitosAntes = anterior.Substring(0, Math.Min(posicion, anterior.Length)).Count(char.IsDigit);
            string digitos = new string(anterior.Where(char.IsDigit).Take(4).ToArray());
            string formateado = digitos.Length <= 2 ? digitos : digitos.Substring(0, 2) + "/" + digitos.Substring(2, Math.Min(2, digitos.Length - 2));
            if (formateado == anterior) return;
            ajustandoVencimiento = true;
            txtVencimiento.Text = formateado;
            int nuevoCursor = digitosAntes <= 2 ? digitosAntes : digitosAntes + 1;
            txtVencimiento.SelectionStart = Math.Min(formateado.Length, nuevoCursor);
            ajustandoVencimiento = false;
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
