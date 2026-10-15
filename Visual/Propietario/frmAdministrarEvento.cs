using Entidades;
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
        public SqlConnection ConexionSql; // TODO Seccion 5-7: quitar ConexionSql
        public string NombreUsuario;
        public string NombreLocal;
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
            List<Sector> sectores = GestorEventos.Instance.Sectores(IDEvento);
            if (sectores.Count == 0)
            {
                DataTable tabla = new DataTable();
                tabla.Columns.Add("Mensaje", typeof(string));
                tabla.Rows.Add("Sin sectores");
                dgvSectores.DataSource = tabla;
                dgvSectores.Columns["Mensaje"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                return;
            }

            dgvSectores.DataSource = sectores
                .Select(s => new { ID = s.Id, s.Nombre, s.Capacidad, s.Precio })
                .ToList();
            dgvSectores.Columns["ID"].Visible = false;
        }

        private void btnAgregarSector_Click(object sender, EventArgs e)
        {
            frmAgregarSector frm = new frmAgregarSector();
            frm.IDlocal = IDlocal;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.ConexionSql = ConexionSql;
            frm.IDEvento = IDEvento;
            frm.NombreEvento = NombreEvento;
            frm.NombreLocal=NombreLocal;
            frm.Show(); this.Close();
        }
        private void btnEliminarSector_Click(object sender, EventArgs e)
        {
            if (dgvSectores.CurrentRow != null && dgvSectores.Columns.Contains("ID"))
            {
                int idSector = Convert.ToInt32(dgvSectores.CurrentRow.Cells["ID"].Value);
                try
                {
                    GestorEventos.Instance.EliminarSector(idSector);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
