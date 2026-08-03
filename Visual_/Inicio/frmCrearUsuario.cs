using Negocio;
using Repositorio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Visual
{
    public partial class frmCrearUsuario : Form
    {
        public frmCrearUsuario()
        {
            InitializeComponent();

        }
        private void frmCrearUsuario_Load(object sender, EventArgs e)
        {

            txtmail.Text="A";
            txtApellido.Text = "A";
            txtNombre.Text = "A";
            TxtPassword.Text = "A";
            TxtConfirmpassword.Text = "A";
            TxtPassword.PasswordChar = '*';
            TxtConfirmpassword.PasswordChar='*';
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
        }
        private void lblyatengocuen_Click(object sender, EventArgs e)
        {
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        }
        private void btnregistrarse_Click(object sender, EventArgs e)
        {
            if (RevisarTextbox()== true)
            {
                ValidarLista(txtmail.Text);
            }
            else { return; }
        }
        private bool RevisarTextbox()
        {
            if (string.IsNullOrEmpty(txtmail.Text))
            {
                MessageBox.Show("Ingrese nombre: ");
                return false;
            }
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("Ingrese Usuario: ");
                return false;

            }
            if (string.IsNullOrEmpty(txtmail.Text))
            {
                MessageBox.Show("Ingrese Mail: ");
                return false;

            }
            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                MessageBox.Show("Ingrese contraseña: ");
                return false;

            }
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text))
            {
                MessageBox.Show("Ingrese segunda contraseña: ");
                return false;
            }
            if (TxtPassword.Text != TxtConfirmpassword.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden");
                return false;
            }
            else
            {
                return true;
            }
        }
        public void ValidarLista(string mail)
        {
            if (cbTipoUsuario.Text == "Cliente")
            {
                if (GestorClientes.Instance.EncontrarMail(mail) == true)
                {
                    MessageBox.Show("Mail: " + mail + " ya está registrado");
                }
                if (GestorPropietario.Instance.EncontrarMail(mail) == true)
                {
                    MessageBox.Show("Mail: " + mail + " ya está registrado como propietario");
                }

                else
                {
                    GestorClientes.Instance.AgregarCliente(txtNombre.Text, txtApellido.Text, txtmail.Text, TxtPassword.Text);
                    MessageBox.Show("Usuario creado");
                    frmInicio frm = new frmInicio();
                    frm.Show();
                    this.Hide();
                }
            }
            if (cbTipoUsuario.Text == "Propietario")
            {
                if (GestorPropietario.Instance.EncontrarMail(mail) == true)
                {
                    MessageBox.Show("Mail: " + mail + " ya está registrado");
                }
                if (GestorClientes.Instance.EncontrarMail(mail) == true)
                {
                    MessageBox.Show("Mail: " + mail + " ya está registrado como cliente");
                }
                else
                {
                    GestorPropietario.Instance.AgregarPropietario(txtNombre.Text, txtApellido.Text, txtmail.Text, TxtPassword.Text);
                    MessageBox.Show("Usuario creado");
                    frmInicio frm = new frmInicio();
                    frm.Show();
                    this.Hide();
                }
            }
        }

    }
}
    


    

