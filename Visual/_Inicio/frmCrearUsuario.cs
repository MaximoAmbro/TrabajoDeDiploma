using Repositorio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visual
{
    public partial class frmAgregarEvento : Form
    {
        public SqlConnection ConexionSql;
        public GestorUsuarios gestorusuarios;
        public frmAgregarEvento()
        {
            InitializeComponent();
        }
        private void frmCrearUsuario_Load(object sender, EventArgs e)
        {
            txtmail.Text="";
            txtApellido.Text = "A";
            txtNombre.Text = "A";
            TxtPassword.Text = "A";
            TxtConfirmpassword.Text = "A";
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
                gestorusuarios.AgregarUsuario (txtNombre.Text, txtApellido.Text, txtmail.Text, TxtPassword.Text, cbTipoUsuario.Text, out string mensaje);
                MessageBox.Show(mensaje); Cerrar();
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
        } // Revisa si los textbox estan vacios
        private void Cerrar()
        {
            frmInicio frm = new frmInicio();
            frm.ConexionSql = ConexionSql;
            frm.Show();
            this.Hide();
        } // cierra el form

    }
}
    


    

