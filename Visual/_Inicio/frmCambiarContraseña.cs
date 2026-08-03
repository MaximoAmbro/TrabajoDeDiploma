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
using Repositorio;

namespace Visual
{
    public partial class frmCambiarContraseña : Form
    {
        public SqlConnection ConexionSql;
        public GestorUsuarios gestorusuarios;
        public frmCambiarContraseña()
        {
            InitializeComponent();
        }


        private void frmCambiarContraseña_Load(object sender, EventArgs e)
        {
            txtMail.Text = "A";
            TxtPassword.Text = "B";
            TxtConfirmpassword.Text = "B";
            TxtPassword.PasswordChar = '*';
            TxtConfirmpassword.PasswordChar = '*';
        } // el load 
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
        } // le pone asterisco a la contraseña
        private void btnconfirmar_Click(object sender, EventArgs e)
        {
            if (ControlarTxt() == true)
            {
                gestorusuarios.CambiarContraseña(txtMail.Text, TxtPassword.Text);
                MessageBox.Show("Contraseña cambiada");
            } // se carga a la bdd
            else
            {
                MessageBox.Show("Dale, son 3 cositos nomas cargalos viejo");
            }
        } // el boton confirmar

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmInicio frm = new frmInicio();
            frm.ConexionSql = ConexionSql;
            frm.Show();
            this.Hide();
        } // Boton volver

        // -------------------------------FUNCIONES DE CONTROL----------------------------------------------------
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
            if (string.IsNullOrEmpty(txtMail.Text))
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
        } // Controla que se llenen todos los campos
    }
}
