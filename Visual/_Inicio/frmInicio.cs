using Repositorio; 
using System;
using System.Configuration;  
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace Visual
{
    public partial class frmInicio : Form
    {
        public SqlConnection ConexionSql;
        private Repositorio.GestorUsuarios gestorUsuarios;
        public frmInicio()
        {
            string miConexion =
            ConfigurationManager.ConnectionStrings["Visual.Properties.Settings.BddEventAura"].ConnectionString;
            ConexionSql = new SqlConnection(miConexion);
            gestorUsuarios = new GestorUsuarios(ConexionSql);

            InitializeComponent();
        }
        
        private void frmInicio_Load(object sender, EventArgs e)
        {
            
            txtMailLogin.Text = "";
            txtContraseñaLogin.Text = "";
            txtContraseñaLogin.PasswordChar = '*';
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
            frm.gestorusuarios = gestorUsuarios;
            frm.ConexionSql = ConexionSql;
            frm.Show();
            this.Hide();
        } // Llama frmCambiarContraseña
        private void lblcrearcuenta_Click(object sender, EventArgs e)
        {
            frmAgregarEvento frm = new frmAgregarEvento();  // este es agregar usuario pero se cambio el nombre y no le encuentro la vuelta
            frm.ConexionSql = ConexionSql;
            frm.gestorusuarios = gestorUsuarios;
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
          if( gestorUsuarios.ValidarCredenciales(mail, contraseña) == true)
            {
                gestorUsuarios.ObtenerIDUsuario(mail, contraseña, out int IdUsuario);
                gestorUsuarios.ObtenerNombreUsuario(mail, contraseña, out string nombreUsuario);
                gestorUsuarios.ObtenerTipoUsuario(mail, contraseña, out string tipoUsuario);
                if (tipoUsuario == "Cliente")
                {
                    frmMenuCliente frm = new frmMenuCliente();
                    frm.IDUsuario = IdUsuario;
                    frm.NombreUsuario = nombreUsuario;
                    frm.ConexionSql = ConexionSql;
                    frm.Show();
                    this.Hide();
                } // Se abre el menu de cliente
                if (tipoUsuario == "Propietario")
                {
                    frmMenuPropietario frm = new frmMenuPropietario();
                    frm.IDUsuario= IdUsuario;
                    frm.NombreUsuario = nombreUsuario;
                    frm.ConexionSql = ConexionSql;
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