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
using Newtonsoft.Json.Linq;

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
        private Label _avisoValidacion;
        private Panel _filaAviso;
        private Control _campoConAviso;
        private int _altoSinAviso;
        private readonly Web.PanelWeb _recuperacionWeb = new Web.PanelWeb { Name = "recuperacionWeb", Visible = false };
        private bool _recuperacionInicializada;
        public frmCambiarContraseña() : this(false) { }
        public frmCambiarContraseña(bool recuperacion)
        {
            _recuperacion = recuperacion;
            InitializeComponent();
            ConfigurarVista();
            if (_recuperacion)
            {
                ClientSize = new Size(1100, 720);
                MinimumSize = new Size(1000, 680);
                Controls.Add(_recuperacionWeb);
                _recuperacionWeb.SetBounds(0, barraSuperior.Bottom, ClientSize.Width, ClientSize.Height - barraSuperior.Bottom);
                _recuperacionWeb.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                barraSuperior.BringToFront();
                _recuperacionWeb.VistaLista += MostrarRecuperacionWeb;
                _recuperacionWeb.VistaNoDisponible += () => { _recuperacionWeb.Visible = false; MostrarRespaldoRecuperacion(); };
                _recuperacionWeb.Mensaje += ProcesarMensajeRecuperacion;
                _recuperacionWeb.Escape += () => btnVolver_Click(this, EventArgs.Empty);
                Shown += (s, e) => _recuperacionWeb.Cargar("recuperar-clave.html");
            }
        }

        private void MostrarRecuperacionWeb()
        {
            foreach (Control control in Controls)
                if (control != barraSuperior && control != _recuperacionWeb) control.Visible = false;
            _recuperacionWeb.Visible = true;
            _recuperacionWeb.BringToFront();
            barraSuperior.BringToFront();
            if (_recuperacionInicializada) return;
            _recuperacionInicializada = true;
            _recuperacionWeb.Actualizar(new
            {
                paso = "mail",
                mail = CorreoInicial,
                parametros = new
                {
                    longitudMinima = GestorParametros.Instance.ObtenerEntero("PWD_LONGITUD_MIN"),
                    fortalezaMinima = GestorParametros.Instance.ObtenerEntero("PWD_FORTALEZA_MIN")
                }
            });
        }

        private void MostrarRespaldoRecuperacion()
        {
            foreach (Control control in Controls)
                if (control != barraSuperior && control != _recuperacionWeb) control.Visible = true;
            barraSuperior.BringToFront();
        }

        private void ProcesarMensajeRecuperacion(string json)
        {
            JObject mensaje;
            try { mensaje = JObject.Parse(json); }
            catch { return; }
            string tipo = (string)mensaje["tipo"];
            try
            {
                if (tipo == "navegar") { btnVolver_Click(this, EventArgs.Empty); return; }
                if (tipo == "enviarCodigoRecuperacion")
                {
                    GestorCodigosCorreo.Instance.SolicitarRecuperacion((string)mensaje["mail"]);
                    _recuperacionWeb.Actualizar(new { paso = "codigo", mail = (string)mensaje["mail"] });
                    return;
                }
                if (tipo == "restablecer")
                {
                    GestorCodigosCorreo.Instance.RecuperarContrasena((string)mensaje["mail"], (string)mensaje["codigo"], (string)mensaje["contrasena"]);
                    _recuperacionWeb.Actualizar(new { paso = "listo", mail = (string)mensaje["mail"] });
                }
            }
            catch (ReglaNegocioException ex)
            {
                string campo = ex.Regla == "RN-02" || ex.Regla == "RN-47" ? "contrasena" : "codigo";
                _recuperacionWeb.Actualizar(new { paso = "codigo", mail = (string)mensaje["mail"], error = new { campo, mensaje = ex.Message } });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                string aviso = tipo == "enviarCodigoRecuperacion"
                    ? "No pudimos enviar el código. Revisá la conexión y volvé a intentarlo."
                    : ManejadorErrores.MensajeParaUsuario(ex);
                _recuperacionWeb.Actualizar(new { paso = tipo == "enviarCodigoRecuperacion" ? "mail" : "codigo", mail = (string)mensaje["mail"], error = new { mensaje = aviso } });
            }
        }


        private void ConfigurarVista()
        {
            _fortaleza = new Componentes.IndicadorFortaleza { Name = "fortalezaContrasena", Location = new Point(24, 316), Size = new Size(392, 24) };
            Controls.Add(_fortaleza);
            TxtPassword.TextChanged += (s, e) => ActualizarFortaleza();
            txtMail.TextChanged += (s, e) => LimpiarAviso(txtMail);
            txtActual.TextChanged += (s, e) => LimpiarAviso(txtActual);
            TxtPassword.TextChanged += (s, e) => LimpiarAviso(TxtPassword);
            TxtConfirmpassword.TextChanged += (s, e) => LimpiarAviso(TxtConfirmpassword);
            _avisoValidacion = new Label
            {
                Font = Tema.Ayuda,
                ForeColor = Tema.Error,
                AccessibleRole = AccessibleRole.Alert,
                Dock = DockStyle.Fill
            };
            _filaAviso = new Panel { Name = "filaAvisoValidacion", Height = 0, Visible = false };
            _filaAviso.Controls.Add(_avisoValidacion);
            Controls.Add(_filaAviso);
            Shown += AjustarVistaDpi;
            if (!_recuperacion) return;

            Text = "EventAura · Recuperar contraseña";
            label1.Text = "Recuperar contraseña";
            lblActual.Text = "Código de recuperación";
            txtActual.UseSystemPasswordChar = false;
            checkbxShowPass.Text = "Mostrar contraseña nueva";
            btnconfirmar.Text = "Restablecer contraseña";
            _enviarCodigo = new Componentes.BotonSecundario { Name = "btnEnviarCodigo", Text = "Enviar código", Location = new Point(232, 216), Size = new Size(184, 36) };
            _estadoCorreo = new Componentes.EtiquetaAyuda { Name = "estadoRecuperacion", Location = new Point(24, 260), Size = new Size(392, 54) };
            _enviarCodigo.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtMail.Text))
                {
                    ErrorCampo(txtMail, "Escribí tu correo electrónico.");
                    return;
                }
                try
                {
                    using (Avisos.Esperar(this, _enviarCodigo))
                        GestorCodigosCorreo.Instance.SolicitarRecuperacion(txtMail.Text);
                    _estadoCorreo.Text = "Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.";
                }
                catch (ReglaNegocioException ex) { _estadoCorreo.Text = ex.Message; }
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
            float escala = Disposicion.Dpi(this) / Tema.DpiDiseno;
            int px(float valor) { return (int)Math.Round(valor * escala); }
            int margen = px(Tema.Espacio.XXL);
            _fortaleza.SetBounds(margen, px(_recuperacion ? 388 : 332), Math.Max(1, ClientSize.Width - 2 * margen), px(24));
            if (_recuperacion)
            {
                ClientSize = new Size(1100, 720);
                int izquierda = (ClientSize.Width - px(568)) / 2;
                _fortaleza.Left = izquierda + margen;
                _fortaleza.Width = px(320);
                txtActual.SetBounds(izquierda + px(24), px(220), px(184), px(28));
                label3.Location = new Point(izquierda + px(24), px(328));
                TxtPassword.Location = new Point(izquierda + px(24), px(352));
                label4.Location = new Point(izquierda + px(24), px(420));
                TxtConfirmpassword.Location = new Point(izquierda + px(24), px(444));
                checkbxShowPass.Location = new Point(izquierda + px(24), px(484));
                btnVolver.Location = new Point(izquierda + px(24), px(524));
                btnconfirmar.Location = new Point(izquierda + px(232), px(524));
                _enviarCodigo.SetBounds(izquierda + px(368), px(216), px(184), px(36));
                _estadoCorreo.SetBounds(izquierda + px(24), px(260), px(392), px(54));
            }
            label1.Top = px(Tema.AltoBarraSuperior + Tema.Espacio.L);
            if (_recuperacion) label1.Left = (ClientSize.Width - label1.Width) / 2;
            int izquierdaCampos = _recuperacion ? (ClientSize.Width - px(568)) / 2 + margen : margen;
            txtMail.Left = txtActual.Left = TxtPassword.Left = TxtConfirmpassword.Left = izquierdaCampos;
            ColocarCampo(label5, txtMail);
            ColocarCampo(lblActual, txtActual);
            ColocarCampo(label3, TxtPassword);
            ColocarCampo(label4, TxtConfirmpassword);
            _fortaleza.Width = TxtPassword.Width;
            checkbxShowPass.Left = TxtPassword.Left;
            btnconfirmar.Top = Math.Max(btnconfirmar.Top, checkbxShowPass.Bottom + px(Tema.Espacio.S));
            Disposicion.AlinearAccionesFormulario(this, TxtPassword, btnconfirmar, btnVolver, apilar: !_recuperacion);
            _altoSinAviso = ClientSize.Height;
            _filaAviso.SetBounds(margen, Math.Max(btnVolver.Bottom, btnconfirmar.Bottom) + px(Tema.Espacio.S),
                ClientSize.Width - 2 * margen, 0);
        }
        private void frmCambiarContraseña_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CorreoInicial)) txtMail.Text = CorreoInicial;
            txtActual.BackColor = TxtPassword.BackColor;
            txtActual.BorderStyle = TxtPassword.BorderStyle;
            txtActual.Font = TxtPassword.Font;
            txtActual.UseSystemPasswordChar = !_recuperacion;
            TxtPassword.UseSystemPasswordChar = true;
            TxtConfirmpassword.UseSystemPasswordChar = true;
            ActualizarFortaleza();
        } // el load 
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtActual.UseSystemPasswordChar = !_recuperacion && !checkbxShowPass.Checked;
            TxtPassword.UseSystemPasswordChar = !checkbxShowPass.Checked;
            TxtConfirmpassword.UseSystemPasswordChar = !checkbxShowPass.Checked;
        }

        private void btnconfirmar_Click(object sender, EventArgs e)
        {
            if (!ControlarTxt()) return;
            if (_recuperacion)
            {
                try
                {
                    using (Avisos.Esperar(this, btnconfirmar))
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
                    ErrorCampo(TxtPassword, ex.Message);
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
            LimpiarAviso(_campoConAviso);
            _campoConAviso = campo;
            campo.BackColor = Tema.ErrorFondo;
            _avisoValidacion.Name = "errorInline_" + campo.Name;
            _avisoValidacion.Text = mensaje;
            _filaAviso.Height = TextRenderer.MeasureText(mensaje, _avisoValidacion.Font,
                new Size(Math.Max(1, _filaAviso.Width), int.MaxValue), TextFormatFlags.WordBreak).Height;
            ClientSize = new Size(ClientSize.Width, Math.Max(_altoSinAviso, _filaAviso.Bottom + Padding.Bottom));
            _filaAviso.Show();
            campo.Focus();
            return false;
        }

        private void LimpiarAviso(Control campo)
        {
            if (campo == null) return;
            campo.BackColor = Tema.Superficie;
            if (campo != _campoConAviso) return;
            _campoConAviso = null;
            _filaAviso.Hide();
            _filaAviso.Height = 0;
            _avisoValidacion.Text = string.Empty;
            if (_altoSinAviso > 0) ClientSize = new Size(ClientSize.Width, _altoSinAviso);
        }
    }
}
