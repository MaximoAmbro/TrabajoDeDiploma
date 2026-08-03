using Microsoft.Data.SqlClient;
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
using Visual.Propietario;

namespace Visual.Vendedor
{
    public partial class frmEventosLocal : Form
    {
        public int IDUsuario; public int IDlocal; 
        public SqlConnection ConexionSql; GestorEventos gestorEventos = new GestorEventos();
        public string NombreLocal { get; set; } public string NombreUsuario { get; set; }
        public frmEventosLocal()
        {
            InitializeComponent();
        }

        private void frmEventosLocal_Load(object sender, EventArgs e)
        {
            CargaEventos();
        }

        private void btnEliminarEvento_Click(object sender, EventArgs e)
        {
            if (dgvEventos.SelectedCells.Count > 0)
            {
                gestorEventos.EliminarEvento(Convert.ToInt32(dgvEventos.CurrentRow.Cells["ID"].Value), ConexionSql);
                CargaEventos();
            }
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMisLocales frm = new frmMisLocales();
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
        }
        private void btnAgregarEvento_Click(object sender, EventArgs e)
        {
            Propietario.frmAgregarEvento frm = new Propietario.frmAgregarEvento();
            frm.IDlocal = IDlocal;
            frm.NombreUsuario = NombreUsuario;
            frm.ConexionSql = ConexionSql;
            frm.IDUsuario = IDUsuario;
            frm.NombreLocal = NombreLocal;
            frm.Show(); this.Close();
        } // este es modificar pero me da paja cambiar el nombre
        private void btnAdministrarEvento_Click(object sender, EventArgs e)
        {
            frmAdministrarEvento frm = new frmAdministrarEvento();
            int EventoSeleccionado = Convert.ToInt32(dgvEventos.CurrentRow.Cells["ID"].Value);
            frm.IDlocal = IDlocal;
            frm.ConexionSql = ConexionSql;
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.NombreLocal = NombreLocal;
            frm.IDEvento = EventoSeleccionado;
            frm.Show(); this.Close();

        }


        // --------------------------------Funciones--------------------------------
        public void CargaEventos()
        {
            lblNombre.Text = NombreLocal;
            string consulta = "SELECT Nombre, Fecha, ID FROM Eventos " +
                              "WHERE LocalID = @LocalID";
            using (SqlCommand cmd = new SqlCommand(consulta, ConexionSql))
            {
                cmd.Parameters.AddWithValue("@LocalID", IDlocal);

                SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvEventos.DataSource = tabla;
                dgvEventos.Columns["ID"].Visible = false;
                ConexionSql.Close();

            }
        }
    }
}
