using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public partial class frmCambiarContraseña : FormularioEventAura
    {
        private readonly bool _recuperacion;
        private Componentes.IndicadorFortaleza _fortaleza;
        private Componentes.EtiquetaAyuda _estadoCorreo;
        private Componentes.BotonSecundario _enviarCodigo;
        public frmCambiarContraseña() : this(false) { }
        public frmCambiarContraseña(bool recuperacion)
        {
            _recuperacion = recuperacion;
            InitializeComponent();
            ConfigurarVista();
        }


        private void ConfigurarVista()
        {
            _fortaleza = new Componentes.IndicadorFortaleza { Name = "fortalezaContrasena", Location = new Point(24, 316), Size = new Size(392, 24) };
            Controls.Add(_fortaleza);
            TxtPassword.TextChanged += (s, e) => ActualizarFortaleza();
            Shown += AjustarVistaDpi;
            if (!_recuperacion) return;

            Text = "EventAura · Recuperar contraseña";
            label1.Text = "Recuperar contraseña";
            lblActual.Text = "Código de recuperación";
            txtActual.PasswordChar = '\0';
            checkbxShowPass.Text = "Mostrar contraseña nueva";
            btnconfirmar.Text = "Restablecer contraseña";
            _enviarCodigo = new Componentes.BotonSecundario { Name = "btnEnviarCodigo", Text = "Enviar código", Location = new Point(232, 216), Size = new Size(184, 36) };
            _estadoCorreo = new Componentes.EtiquetaAyuda { Name = "estadoRecuperacion", Location = new Point(24, 260), Size = new Size(392, 36) };
            _enviarCodigo.Click += (s, e) =>
            {
                GestorCodigosCorreo.Instance.SolicitarRecuperacion(txtMail.Text);
                var mensaje = EnviadorSimulado.Ultimo(txtMail.Text.Trim(), "Recuperacion");
                _estadoCorreo.Text = "Si la cuenta existe, enviamos un código. Revisá la bandeja simulada." +
                    (mensaje == null ? string.Empty : " Código: " + mensaje.Codigo + ". Vence: " + mensaje.VenceEl.ToLocalTime().ToString("HH:mm") + ".");
            };
            Controls.Add(_enviarCodigo);
            Controls.Add(_estadoCorreo);
        }

        private void ActualizarFortaleza()
        {
            if (_fortaleza == null) return;
            int minimo = GestorParametros.Instance.ObtenerEntero("PWD_FORTALEZA_MIN");
            _fortaleza.Actualizar(TxtPassword.Text, GestorUsuarios.Instance.PuntajeFortaleza(TxtPassword.Text), minimo);
        }
        private void AjustarVistaDpi(object sender, EventArgs e)
        {
            float escala; using (var g = CreateGraphics()) escala = g.DpiX / Tema.DpiDiseno * Font.SizeInPoints / Tema.Cuerpo.SizeInPoints;
            int px(float valor) { return (int)Math.Round(valor * escala); }
            _fortaleza.SetBounds(px(24), px(_recuperacion ? 368 : 332), px(392), px(24));
            if (!_recuperacion) return;
            ClientSize = new Size(px(440), px(556));
            txtActual.SetBounds(px(24), px(220), px(184), px(28));
            label3.Location = new Point(px(24), px(300));
            TxtPassword.Location = new Point(px(24), px(324));
            label4.Location = new Point(px(24), px(400));
            TxtConfirmpassword.Location = new Point(px(24), px(424));
            checkbxShowPass.Location = new Point(px(24), px(468));
            btnVolver.Location = new Point(px(24), px(508));
            btnconfirmar.Location = new Point(px(232), px(508));
            _enviarCodigo.SetBounds(px(232), px(216), px(184), px(36));
            _estadoCorreo.SetBounds(px(24), px(260), px(392), px(36));
        }
        private void frmCambiarContraseña_Load(object sender, EventArgs e)
        {
            txtActual.BackColor = TxtPassword.BackColor;
            txtActual.BorderStyle = TxtPassword.BorderStyle;
            txtActual.Font = TxtPassword.Font;
            txtActual.PasswordChar = _recuperacion ? '\0' : '*';
            TxtPassword.PasswordChar = '*';
            TxtConfirmpassword.PasswordChar = '*';
            ActualizarFortaleza();
        } // el load 
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            char mascara = checkbxShowPass.Checked ? '\0' : '*';
            txtActual.PasswordChar = _recuperacion ? '\0' : mascara;
            TxtPassword.PasswordChar = mascara;
            TxtConfirmpassword.PasswordChar = mascara;
        }

        private void btnconfirmar_Click(object sender, EventArgs e)
        {
            if (!ControlarTxt()) return;
            if (_recuperacion)
            {
                try
                {
                    GestorCodigosCorreo.Instance.RecuperarContrasena(txtMail.Text, txtActual.Text.Trim(), TxtPassword.Text);
                    _estadoCorreo.Text = "La contraseña se actualizó. Ya podés iniciar sesión.";
                    btnconfirmar.Enabled = false;
                }
                catch (ReglaNegocioException ex) { _estadoCorreo.Text = ex.Message; }
                return;
            }
            try
            {
                GestorUsuarios.Instance.CambiarContrasena(txtMail.Text, txtActual.Text, TxtPassword.Text);
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla("Cambiar contraseña", ex);
                return;
            }
            txtActual.Clear();
            TxtPassword.Clear();
            TxtConfirmpassword.Clear();
            MessageBox.Show("La contraseña se cambió correctamente.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        } // Boton volver

        // -------------------------------FUNCIONES DE CONTROL----------------------------------------------------
        public bool ControlarTxt()
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text)) return ErrorCampo("Ingresá tu correo electrónico.");
            if (string.IsNullOrWhiteSpace(txtActual.Text)) return ErrorCampo(_recuperacion ? "Ingresá el código de recuperación." : "Ingresá la contraseña actual.");
            if (string.IsNullOrEmpty(TxtPassword.Text)) return ErrorCampo("Ingresá la contraseña nueva.");
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text)) return ErrorCampo("Ingresá la confirmación de la nueva contraseña.");
            if (TxtPassword.Text != TxtConfirmpassword.Text) return ErrorCampo("Las contraseñas no coinciden.");
            if (_recuperacion && string.IsNullOrWhiteSpace(TxtPassword.Text)) return ErrorCampo("Solicitá un código antes de restablecer la contraseña.");
            return true;
        }

        private bool ErrorCampo(string mensaje)
        {
            if (_recuperacion && _estadoCorreo != null) _estadoCorreo.Text = mensaje;
            else MessageBox.Show(mensaje, "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
 // Controla que se llenen todos los campos
    }
}
