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
    public partial class frmRegistrarLocal : Form
    {
        public int IDUsuario;
        public string NombreUsuario;
        public SqlConnection ConexionSql;
        public frmRegistrarLocal()
        {
            InitializeComponent();
        }

        private void lblyatengocuen_Click(object sender, EventArgs e)
        {
            Cerrar();
        }
        private void btnregistrarse_Click(object sender, EventArgs e)
        {
            string consulta = "INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID) " +
                "VALUES (@Nombre, @Direccion, @Capacidad, @PropietarioID)";
            SqlCommand sqlComando = new SqlCommand(consulta, ConexionSql);
            ConexionSql.Open();
            sqlComando.Parameters.AddWithValue("@Nombre", txtNombre.Text);
            sqlComando.Parameters.AddWithValue("@Direccion", txtDireccion.Text + " " + txtAltura.Text);
            sqlComando.Parameters.AddWithValue("@Capacidad", txtCapacidad.Text);
            sqlComando.Parameters.AddWithValue("@PropietarioID", IDUsuario);
            sqlComando.ExecuteNonQuery();
            ConexionSql.Close();
            MessageBox.Show("Local registrado correctamente");
            Cerrar();
        }
        private void Cerrar()
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
        }

        private void frmRegistrarLocal_Load(object sender, EventArgs e)
        {

        }
    }
}
