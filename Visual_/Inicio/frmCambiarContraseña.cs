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
    public partial class frmCambiarContraseña : Form
    {
        public frmCambiarContraseña()
        {
            InitializeComponent();
        }
        private void btnVolver_Click_1(object sender, EventArgs e)
        {
            frmInicio frmInicio = new frmInicio();
            frmInicio.Show();
            this.Hide();       
        }
        private void frmCambiarContraseña_Load(object sender, EventArgs e)
        {
            txtMail.Text = "PedroLopez@gmail.com";
            TxtPassword.Text = "";
            TxtPassword.PasswordChar = '*';
            TxtConfirmpassword.PasswordChar = '*';
        }
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbxShowPass.Checked == true)
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
        private void btnconfirmar_Click(object sender, EventArgs e)
        {
            GestorClientes gestorClientes = new GestorClientes();

            if (ControlarTxt() == true)
            {
                    if(gestorClientes.EncontrarMail(txtMail.Text) == true)
                    {
                        MessageBox.Show("Tengo que terminar el codigo");
                    }
            }
                    else
                    {
                        MessageBox.Show("Usuario no encontrado");
                    }
         }
        public bool ControlarTxt()
        {
            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                MessageBox.Show("Ingrese contraseña: ");
                return false;
            }
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text))
            {
                MessageBox.Show("Ingrese confirmacion de contraseña: ");
                return false;
            }
            if(string.IsNullOrEmpty(txtMail.Text))
            {
                MessageBox.Show("Ingrese Mail: ");
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
    }  
}
