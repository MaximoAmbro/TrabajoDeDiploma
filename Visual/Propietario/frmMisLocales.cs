
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
using Negocio;
using Visual.Vendedor;

namespace Visual
{
    public partial class frmMisLocales : Form
    {
        public int IDUsuario;
        public string NombreUsuario;
        public SqlConnection ConexionSql; // TODO Seccion 5-7: quitar ConexionSql
        public frmMisLocales()
        {
            InitializeComponent();
        }

        private void frmMisLocales_Load(object sender, EventArgs e)
        {
            dgvLocales.DataSource = GestorLocales.Instance.PorPropietario(IDUsuario)
                .Select(l => new { ID = l.Id, l.Nombre, l.Direccion, l.Capacidad })
                .ToList();

            FormatearGrilla();
        }

        private void FormatearGrilla()
        {
            dgvLocales.ReadOnly = true;
            dgvLocales.AllowUserToAddRows = false;
            dgvLocales.AllowUserToDeleteRows = false;
            dgvLocales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLocales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvLocales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvLocales.Columns["ID"].Visible = false;
            dgvLocales.Columns["Nombre"].HeaderText = "Nombre del local";
            dgvLocales.Columns["Direccion"].HeaderText = "Dirección";
            dgvLocales.Columns["Capacidad"].HeaderText = "Capacidad";
            dgvLocales.Columns["Capacidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvLocales.Columns["Capacidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
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
