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

namespace Visual
{
    public partial class frmCrearUsuario : FormularioEventAura
    {
        private readonly bool _altaAdministrador;
        private Componentes.IndicadorFortaleza _fortaleza;
        public frmCrearUsuario() : this(false) { }
        public frmCrearUsuario(bool altaAdministrador)
        {
            _altaAdministrador = altaAdministrador;
            InitializeComponent();
            _fortaleza = new Componentes.IndicadorFortaleza { Name = "fortalezaContrasena", Location = new Point(24, 348), Size = new Size(392, 24) };
            Controls.Add(_fortaleza);
            TxtPassword.TextChanged += (s, e) => ActualizarFortaleza();
            txtNombre.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtNombre);
            txtApellido.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtApellido);
            txtmail.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtmail);
            TxtPassword.TextChanged += (s, e) => ValidacionCampo.Limpiar(TxtPassword);
            TxtConfirmpassword.TextChanged += (s, e) => ValidacionCampo.Limpiar(TxtConfirmpassword);
            Shown += (s, e) =>
            {
                float escala; using (var g = CreateGraphics()) escala = g.DpiX / Tema.DpiDiseno * Font.SizeInPoints / Tema.Cuerpo.SizeInPoints;
                int margen = (int)Math.Round(24 * escala);
                _fortaleza.SetBounds(margen, (int)Math.Round(364 * escala), Math.Max(1, ClientSize.Width - margen * 2), (int)Math.Round(24 * escala));
            };
            if (_altaAdministrador)
            {
                Text = "EventAura · Crear administrador";
                label1.Text = "Crear administrador";
                btnregistrarse.Text = "Crear administrador";
                lblyatengocuen.Text = "Volver a administración";
            }
            Tema.ComoEnlace(lblyatengocuen);
        }
        private void frmCrearUsuario_Load(object sender, EventArgs e)
        {
            txtmail.Text="";
            txtApellido.Clear();
            txtNombre.Clear();
            TxtPassword.Clear();
            TxtConfirmpassword.Clear();
            TxtPassword.PasswordChar = '*';
            TxtConfirmpassword.PasswordChar='*';
            ActualizarFortaleza();
        }  // El Load del formulario
        private void ActualizarFortaleza()
        {
            if (_fortaleza == null) return;
            int minimo = GestorParametros.Instance.ObtenerEntero("PWD_FORTALEZA_MIN");
            int puntaje = GestorUsuarios.Instance.PuntajeFortaleza(TxtPassword.Text);
            _fortaleza.Actualizar(TxtPassword.Text, puntaje, minimo);
        }
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if(checkbxShowPass.Checked == true)
            {
                TxtPassword.PasswordChar = '\0';
                TxtConfirmpassword.PasswordChar = '\0';
            }
            else
            {
                TxtPassword.PasswordChar = '*';
                TxtConfirmpassword.PasswordChar = '*';
            }
        } // El de la contraseña

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnregistrarse_Click(object sender, EventArgs e)
        {
            if (RevisarTextbox()== true)
            {
                try
                {
                    if (_altaAdministrador) GestorUsuarios.Instance.CrearAdministrador(txtmail.Text, TxtPassword.Text, txtNombre.Text, txtApellido.Text);
                    else GestorUsuarios.Instance.RegistrarCliente(txtmail.Text, TxtPassword.Text, txtNombre.Text, txtApellido.Text);
                }
                catch (ReglaNegocioException ex)
                {
                    if (ex.Regla == "RN-02" || ex.Regla == "RN-47")
                    {
                        ValidacionCampo.Mostrar(TxtPassword, ex.Message); TxtPassword.Focus();
                    }
                    else if (ex.Regla == "RN-01")
                    {
                        ValidacionCampo.Mostrar(txtmail, ex.Message); txtmail.Focus();
                    }
                    else Avisos.Regla(this, "Crear cuenta", ex);
                    return;
                }
                if (!_altaAdministrador)
                {
                    MostrarVerificacionCorreo(txtmail.Text.Trim());
                    return;
                }
                var menu = Cerrar();
                Avisos.Informativo(menu, "Usuario creado con éxito.");
            }
            else { return; }
        }
        private void MostrarVerificacionCorreo(string correo)
        {
            foreach (Control control in Controls.Cast<Control>().ToArray())
                if (control.Name != "barraSuperior") control.Visible = false;
            Text = "EventAura · Verificar correo";
            float escala = Disposicion.Escala(this);
            int px(float valor) => (int)Math.Round(valor * escala);
            int ancho = Math.Max(px(280), ClientSize.Width - px(48));
            var verificacion = new Panel { Name = "verificacionCorreo", Location = new Point(px(24), px(84)), Size = new Size(ancho, Math.Max(px(300), ClientSize.Height - px(108))), AutoScroll = true };
            var titulo = new Componentes.EtiquetaTitulo { Name = "tituloVerificacion", Text = "Verificá tu correo", Location = new Point(px(8), px(8)), Size = new Size(ancho - px(24), px(40)) };
            var contenido = new Componentes.EtiquetaAyuda { Name = "instruccionesVerificacion", Location = new Point(px(8), px(60)), Size = new Size(ancho - px(24), px(80)), Text = "Si la cuenta existe, enviamos un código. Abrí fuera de EventAura el archivo de correo en %LOCALAPPDATA%\\EventAura\\BandejaSimulada para continuar." };
            var etiqueta = new Componentes.EtiquetaRotulo { Text = "Código de verificación", Location = new Point(px(8), px(164)), Size = new Size(px(240), px(24)) };
            var codigo = new TextBox { Name = "txtCodigoCorreo", Location = new Point(px(8), px(192)), Size = new Size(px(240), px(32)), MaxLength = 6 };
            var estado = new Componentes.EtiquetaAyuda { Name = "estadoVerificacion", Location = new Point(px(8), px(232)), Size = new Size(ancho - px(24), px(54)) };
            var confirmar = new Componentes.BotonPrimario { Name = "btnConfirmarCorreo", Text = "Confirmar correo", Location = new Point(px(8), px(292)), Size = new Size(px(200), px(40)) };
            var volver = new Componentes.BotonSecundario { Name = "btnVolverAcceso", Text = "Volver al acceso", Location = new Point(px(220), px(292)), Size = new Size(px(180), px(40)), Visible = false };
            confirmar.Click += (s, e) =>
            {
                try
                {
                    GestorCodigosCorreo.Instance.ConfirmarVerificacion(correo, codigo.Text.Trim());
                    estado.Text = "Correo verificado. Ya podés iniciar sesión y comprar.";
                    confirmar.Enabled = false;
                    volver.Visible = true;
                }
                catch (ReglaNegocioException ex) { estado.Text = ex.Message; }
            };
            volver.Click += (s, e) => Cerrar();
            verificacion.Controls.AddRange(new Control[] { titulo, contenido, etiqueta, codigo, estado, confirmar, volver });
            Controls.Add(verificacion);
            verificacion.BringToFront();
        }
        private void lblyatengocuen_Click(object sender, EventArgs e)
        {
            Cerrar();
        } // Nos devuelve al incio

        // -------------------------------FUNCIONES DE CONTROL--------------------------------------------------------------------
        private bool RevisarTextbox()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                ValidacionCampo.Mostrar(txtNombre, "Escribí tu nombre."); txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                ValidacionCampo.Mostrar(txtApellido, "Escribí tu apellido."); txtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtmail.Text))
            {
                ValidacionCampo.Mostrar(txtmail, "Escribí tu correo electrónico."); txtmail.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                ValidacionCampo.Mostrar(TxtPassword, "Elegí una contraseña."); TxtPassword.Focus();
                return false;

            }
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text))
            {
                ValidacionCampo.Mostrar(TxtConfirmpassword, "Repetí tu contraseña."); TxtConfirmpassword.Focus();
                return false;
            }
            if (TxtPassword.Text != TxtConfirmpassword.Text)
            {
                ValidacionCampo.Mostrar(TxtConfirmpassword, "Las contraseñas no coinciden. Revisá lo que escribiste."); TxtConfirmpassword.Focus();
                return false;
            }
            else
            {
                return true;
            }
        } // Revisa si los textbox estan vacios
        private Form Cerrar()
        {
            if (_altaAdministrador)
            {
                var menu = Application.OpenForms.OfType<frmMenuAdministrador>().FirstOrDefault();
                if (menu != null) menu.Show();
                Hide();
                return menu;
            }
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
            return null;
        } // cierra el form

    }
}
    


    

