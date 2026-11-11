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
        public frmCambiarContraseña()
        {
            InitializeComponent();
        }


        private void frmCambiarContraseña_Load(object sender, EventArgs e)
        {
            txtActual.BackColor = TxtPassword.BackColor;
            txtActual.BorderStyle = TxtPassword.BorderStyle;
            txtActual.Font = TxtPassword.Font;
            txtActual.PasswordChar = '*';
            TxtPassword.PasswordChar = '*';
            TxtConfirmpassword.PasswordChar = '*';
        } // el load 
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbxShowPass.Checked == true)
            {
                txtActual.PasswordChar = '\0';
                TxtPassword.PasswordChar = '\0';
                TxtConfirmpassword.PasswordChar = '\0';
            }
            else
            {
                txtActual.PasswordChar = '*';
                TxtPassword.PasswordChar = '*';
                TxtConfirmpassword.PasswordChar = '*';
            }
        } // le pone asterisco a la contraseña
        private void btnconfirmar_Click(object sender, EventArgs e)
        {
            if (ControlarTxt() == true)
            {
                try
                {
                    GestorUsuarios.Instance.CambiarContrasena(txtMail.Text, txtActual.Text, TxtPassword.Text);
                }
                catch (ReglaNegocioException ex)
                {
                    Avisos.Regla("Cambiar contraseña", ex);
                    return;
                }
                txtActual.Clear();
                TxtPassword.Clear();
                TxtConfirmpassword.Clear();
                MessageBox.Show("La contraseña se cambió correctamente.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } // se carga a la bdd

        } // el boton confirmar

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        } // Boton volver

        // -------------------------------FUNCIONES DE CONTROL----------------------------------------------------
        public bool ControlarTxt()
        {
            if (string.IsNullOrEmpty(txtActual.Text))
            {
                MessageBox.Show("Ingrese la contraseña actual.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                MessageBox.Show("Ingrese contraseña: ", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text))
            {
                MessageBox.Show("Ingrese la confirmación de la nueva contraseña.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtMail.Text))
            {
                MessageBox.Show("Ingrese su correo electrónico.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (TxtPassword.Text != TxtConfirmpassword.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                return true;
            }
        } // Controla que se llenen todos los campos
    }
}
