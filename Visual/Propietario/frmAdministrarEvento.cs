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
using Microsoft.Data.SqlClient;
using System.Runtime.Versioning;

namespace Visual.Vendedor
{
    public partial class frmAdministrarEvento : Form
    {
        public int IDUsuario; public int IDlocal; public int IDEvento;
        public SqlConnection ConexionSql; 
        public string NombreUsuario;
        public string NombreLocal;
        public GestorEventos gestorEventos = new GestorEventos();
        public string NombreEvento { get; set; }

        public frmAdministrarEvento()
        {
            InitializeComponent();
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmEventosLocal form = new frmEventosLocal();
            form.ConexionSql = ConexionSql;
            form.IDlocal = IDlocal;
            form.NombreUsuario = NombreUsuario;
            form.IDUsuario = IDUsuario;
            form.NombreLocal = NombreEvento;
            form.Show(); this.Hide();
        }
        private void frmAgregarEventoLocal_Load(object sender, EventArgs e)
        {
            lblEvento.Text = NombreEvento;
            CargarDGV();
        }
        public void CargarDGV()
        {
            string consulta = "SELECT Nombre, Capacidad FROM Sectores WHERE EventoID = @EventoID";
            using (SqlCommand cmd = new SqlCommand(consulta, ConexionSql))
            {
                cmd.Parameters.AddWithValue("@EventoID", IDEvento);
                SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dgvSectores.DataSource = tabla;
                ConexionSql.Close();
                if (tabla.Rows.Count == 0)
                {
                    tabla.Columns.Add("Mensaje", typeof(string));
                    tabla.Rows.Add("Sin sectores");
                    dgvSectores.DataSource = tabla;
                    dgvSectores.Columns["Mensaje"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void btnAgregarSector_Click(object sender, EventArgs e)
        {
            frmAgregarSector frm = new frmAgregarSector();
            frm.IDlocal = IDlocal;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.ConexionSql = ConexionSql;
            frm.IDEvento = IDEvento;
            frm.NombreLocal=NombreLocal;
            frm.Show(); this.Close();
        }
        private void btnEliminarSector_Click(object sender, EventArgs e)
        {
            if (dgvSectores.SelectedCells.Count > 0)
            {
                    string nombreSector = dgvSectores.SelectedCells[0].Value.ToString();
                
                    gestorEventos.EliminarSector(nombreSector, IDEvento, ConexionSql);
                    gestorEventos.ReasignarPrecio(IDEvento, ConexionSql);
                    CargarDGV();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un sector para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnVolver_Click1(object sender, EventArgs e)
        {
            frmEventosLocal frmEventosLocal = new frmEventosLocal();
            frmEventosLocal.ConexionSql = ConexionSql;
            frmEventosLocal.IDlocal = IDlocal;
            frmEventosLocal.IDUsuario = IDUsuario;
            frmEventosLocal.NombreUsuario = NombreUsuario;
            frmEventosLocal.NombreLocal = NombreLocal;
            frmEventosLocal.Show(); this.Hide();
        }
    }
}
