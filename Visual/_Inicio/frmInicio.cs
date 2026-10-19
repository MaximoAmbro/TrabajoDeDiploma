using Entidades;
using Negocio;
using System;
using System.Windows.Forms;

namespace Visual
{
    public partial class frmInicio : Form
    {
        // La cadena de conexión la lee Datos/Conexion.cs de App.config: Visual no abre conexiones.
        public frmInicio()
        {
            InitializeComponent();
        }
        
        private void frmInicio_Load(object sender, EventArgs e)
        {
            
            txtMailLogin.Text = "";
            txtContraseñaLogin.Text = "";
            txtContraseñaLogin.PasswordChar = '*';
            txtMailLogin.Select();   // no depende del orden de TabIndex
        }

        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbxShowPass.Checked == true)
            {
                txtContraseñaLogin.PasswordChar = '\0';
            }
            else
            {
                txtContraseñaLogin.PasswordChar = '*';
            }
        } //Le pone el asterisco a la contraseña

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void lblolividarcontra_Click(object sender, EventArgs e)
        {
            frmCambiarContraseña frm = new frmCambiarContraseña();
            frm.Show();
            this.Hide();
        } // Llama frmCambiarContraseña
        private void lblcrearcuenta_Click(object sender, EventArgs e)
        {
            frmCrearUsuario frm = new frmCrearUsuario();
            frm.Show();
            this.Hide();
        } // Llama a frmCrearCuenta
        private void btninicio_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMailLogin.Text))
            {
                MessageBox.Show("Por favor, ingrese usuario",
                              "Campo vacíos",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtContraseñaLogin.Text))
            {
                MessageBox.Show("Por favor, ingrese contraseña",
                              "Campo vacíos",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }
            else
            {
                ValidarLista(txtMailLogin.Text, txtContraseñaLogin.Text);
            }
        } // Llamado a los menues

        // --------------------------------------FUNCIONES DE CONTROL----------------------------------------------
        public void ValidarLista(string mail, string contraseña)
        {
            Usuario usuario;
            try
            {
                usuario = GestorUsuarios.Instance.Autenticar(mail, contraseña);
            }
            catch (ReglaNegocioException ex)
            {
                // RN-04: intentos restantes o minutos de bloqueo.
                MessageBox.Show(ex.Message, "Inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (usuario != null)
            {
                if (usuario.TipoUsuario == "Cliente")
                {
                    frmMenuCliente frm = new frmMenuCliente();
                    frm.IDUsuario = usuario.Id;
                    frm.NombreUsuario = usuario.Nombre;
                    frm.Show();
                    this.Hide();
                } // Se abre el menu de cliente
                if (usuario.TipoUsuario == "Propietario")
                {
                    frmMenuPropietario frm = new frmMenuPropietario();
                    frm.IDUsuario = usuario.Id;
                    frm.NombreUsuario = usuario.Nombre;
                    frm.Show();
                    this.Hide();
                } // Se abre el menu de vendedor
            }
            else
            {
                MessageBox.Show ("Mail o contraseña incorrecta");
            }
        }
    } // Valida la lista de usuarios

}