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
        public string CorreoInicial { get; set; }
        public Form Retorno { get; set; }
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
            txtMail.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtMail);
            txtActual.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtActual);
            TxtPassword.TextChanged += (s, e) => ValidacionCampo.Limpiar(TxtPassword);
            TxtConfirmpassword.TextChanged += (s, e) => ValidacionCampo.Limpiar(TxtConfirmpassword);
            Shown += AjustarVistaDpi;
            if (!_recuperacion) return;

            Text = "EventAura · Recuperar contraseña";
            label1.Text = "Recuperar contraseña";
            lblActual.Text = "Código de recuperación";
            txtActual.PasswordChar = '\0';
            checkbxShowPass.Text = "Mostrar contraseña nueva";
            btnconfirmar.Text = "Restablecer contraseña";
            _enviarCodigo = new Componentes.BotonSecundario { Name = "btnEnviarCodigo", Text = "Enviar código", Location = new Point(232, 216), Size = new Size(184, 36) };
            _estadoCorreo = new Componentes.EtiquetaAyuda { Name = "estadoRecuperacion", Location = new Point(24, 260), Size = new Size(392, 54) };
            _enviarCodigo.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtMail.Text))
                {
                    ValidacionCampo.Mostrar(txtMail, "Escribí tu correo electrónico."); txtMail.Focus();
                    return;
                }
                try
                {
                    GestorCodigosCorreo.Instance.SolicitarRecuperacion(txtMail.Text);
                    _estadoCorreo.Text = "Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.";
                }
                catch (Exception)
                {
                    _estadoCorreo.Text = "No pudimos enviar el código. Revisá la conexión o la configuración de correo y volvé a intentarlo.";
                }
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
            int margen = px(24);
            _fortaleza.SetBounds(margen, px(_recuperacion ? 388 : 332), Math.Max(1, ClientSize.Width - 2 * margen), px(24));
            if (!_recuperacion) return;
            ClientSize = new Size(px(440), px(580));
            txtActual.SetBounds(px(24), px(220), px(184), px(28));
            label3.Location = new Point(px(24), px(328));
            TxtPassword.Location = new Point(px(24), px(352));
            label4.Location = new Point(px(24), px(420));
            TxtConfirmpassword.Location = new Point(px(24), px(444));
            checkbxShowPass.Location = new Point(px(24), px(484));
            btnVolver.Location = new Point(px(24), px(524));
            btnconfirmar.Location = new Point(px(232), px(524));
            _enviarCodigo.SetBounds(px(232), px(216), px(184), px(36));
            _estadoCorreo.SetBounds(px(24), px(260), px(392), px(54));
        }
        private void frmCambiarContraseña_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CorreoInicial)) txtMail.Text = CorreoInicial;
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
                if (ex.Regla == "RN-02" || ex.Regla == "RN-47")
                    ValidacionCampo.Mostrar(TxtPassword, ex.Message);
                else Avisos.Regla(this, "Cambiar contraseña", ex);
                return;
            }
            txtActual.Clear();
            TxtPassword.Clear();
            TxtConfirmpassword.Clear();
            Avisos.Informativo(this, "La contraseña se cambió correctamente.");
        }
        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (Retorno != null && !Retorno.IsDisposed)
            {
                Retorno.Show();
                Hide();
                return;
            }
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        } // Boton volver

        // -------------------------------FUNCIONES DE CONTROL----------------------------------------------------
        public bool ControlarTxt()
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text)) return ErrorCampo(txtMail, "Escribí tu correo electrónico.");
            if (string.IsNullOrWhiteSpace(txtActual.Text)) return ErrorCampo(txtActual, _recuperacion ? "Ingresá el código de recuperación." : "Ingresá la contraseña actual.");
            if (string.IsNullOrEmpty(TxtPassword.Text)) return ErrorCampo(TxtPassword, "Escribí tu contraseña nueva.");
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text)) return ErrorCampo(TxtConfirmpassword, "Repetí tu contraseña nueva.");
            if (TxtPassword.Text != TxtConfirmpassword.Text) return ErrorCampo(TxtConfirmpassword, "Las contraseñas no coinciden. Revisá lo que escribiste.");
            return true;
        }

        private bool ErrorCampo(Control campo, string mensaje)
        {
            ValidacionCampo.Mostrar(campo, mensaje);
            campo.Focus();
            return false;
        }
 // Controla que se llenen todos los campos
    }
}
