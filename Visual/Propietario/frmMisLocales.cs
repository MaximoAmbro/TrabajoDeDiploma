
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
using Visual.Vendedor;

namespace Visual
{
    public partial class frmMisLocales : Form
    {
        public int IDUsuario;
        public string NombreUsuario;
        public SqlConnection ConexionSql;
        public frmMisLocales()
        {
            InitializeComponent();
        }

        private void frmMisLocales_Load(object sender, EventArgs e)
        {
            string consulta = "SELECT ID, Nombre, Direccion, Capacidad FROM Locales WHERE PropietarioID = @IDUsuario";
            using (SqlCommand cmd = new SqlCommand(consulta, ConexionSql))
            {
                ConexionSql.Open();
                cmd.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvLocales.DataSource = tabla;

                dgvLocales.Columns["ID"].Visible = false;
                ConexionSql.Close();
            }
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnEventosLocal_Click(object sender, EventArgs e)
        {
            if (dgvLocales.CurrentRow != null && dgvLocales.SelectedRows.Count > 0)
            {
                int idLocalSeleccionado = Convert.ToInt32(dgvLocales.CurrentRow.Cells["ID"].Value);
                string nombreLocal = dgvLocales.CurrentRow.Cells["Nombre"].Value.ToString();
                frmEventosLocal frm = new frmEventosLocal();
                frm.NombreLocal = nombreLocal;
                frm.NombreUsuario = NombreUsuario;
                frm.IDlocal = idLocalSeleccionado;
                frm.ConexionSql = ConexionSql;
                frm.IDUsuario = IDUsuario;
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un local");
            }
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
        }
    }
}
