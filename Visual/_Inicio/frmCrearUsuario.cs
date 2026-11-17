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
        public frmCrearUsuario() : this(false) { }
        public frmCrearUsuario(bool altaAdministrador)
        {
            _altaAdministrador = altaAdministrador;
            InitializeComponent();
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
        }  // El Load del formulario
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
                MessageBox.Show("Usuario creado con éxito.", _altaAdministrador ? "Crear administrador" : "Crear cuenta", MessageBoxButtons.OK, MessageBoxIcon.Information); Cerrar();
            }
            else { return; }
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
    


    

