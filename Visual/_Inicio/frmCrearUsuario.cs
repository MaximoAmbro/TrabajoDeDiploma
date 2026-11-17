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
            Shown += (s, e) =>
            {
                float escala; using (var g = CreateGraphics()) escala = g.DpiX / Tema.DpiDiseno * Font.SizeInPoints / Tema.Cuerpo.SizeInPoints;
                _fortaleza.SetBounds((int)Math.Round(24 * escala), (int)Math.Round(348 * escala), (int)Math.Round(392 * escala), (int)Math.Round(24 * escala));
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
                    Avisos.Regla("Crear cuenta", ex);
                    return;
                }
                if (!_altaAdministrador)
                {
                    MostrarBandejaSimulada(txtmail.Text.Trim());
                    return;
                }
                MessageBox.Show("Usuario creado con éxito.", "Crear administrador", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cerrar();
            }
            else { return; }
        }
        private void MostrarBandejaSimulada(string correo)
        {
            foreach (Control control in Controls.Cast<Control>().ToArray())
                if (control.Name != "barraSuperior") control.Visible = false;
            Text = "EventAura · Verificar correo";
            float escala = Disposicion.Escala(this);
            int px(float valor) => (int)Math.Round(valor * escala);
            int ancho = Math.Max(px(280), ClientSize.Width - px(48));
            var bandeja = new Panel { Name = "bandejaCorreoSimulada", Location = new Point(px(24), px(84)), Size = new Size(ancho, Math.Max(px(300), ClientSize.Height - px(108))), AutoScroll = true };
            var titulo = new Componentes.EtiquetaTitulo { Name = "tituloBandeja", Text = "Bandeja simulada de correo", Location = new Point(px(8), px(8)), Size = new Size(ancho - px(24), px(40)) };
            var mensaje = EnviadorSimulado.Ultimo(correo, "Verificacion");
            var contenido = new Componentes.EtiquetaAyuda { Name = "mensajeBandeja", Location = new Point(px(8), px(60)), Size = new Size(ancho - px(24), px(92)), Text = mensaje == null ? "No hay un mensaje disponible. Volvé a registrarte para solicitar otro código." : "Para: " + correo + "\r\nAsunto: Verificá tu correo de EventAura\r\nCódigo: " + mensaje.Codigo + "\r\nVence: " + mensaje.VenceEl.ToLocalTime().ToString("dd/MM/yyyy HH:mm") };
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
            bandeja.Controls.AddRange(new Control[] { titulo, contenido, etiqueta, codigo, estado, confirmar, volver });
            Controls.Add(bandeja);
            bandeja.BringToFront();
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
                MessageBox.Show("Ingresá tu nombre.", "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Ingresá tu apellido.", "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtmail.Text))
            {
                MessageBox.Show("Ingresá tu correo electrónico.", "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                MessageBox.Show("Ingresá contraseña: ", "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;

            }
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text))
            {
                MessageBox.Show("Ingresá segunda contraseña: ", "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (TxtPassword.Text != TxtConfirmpassword.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                return true;
            }
        } // Revisa si los textbox estan vacios
        private void Cerrar()
        {
            if (_altaAdministrador)
            {
                var menu = Application.OpenForms.OfType<frmMenuPropietario>().FirstOrDefault();
                if (menu != null) menu.Show();
                Hide();
                return;
            }
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        } // cierra el form

    }
}
    


    

