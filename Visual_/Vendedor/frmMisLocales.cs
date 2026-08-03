using Entidades;
using Negocio;
using Repositorio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public string Mail { get; set; }
        public List<Local> ListaLocales { get; set; }
        public frmMisLocales()
        {
            InitializeComponent();
        }
        private void btnEventosLocal_Click(object sender, EventArgs e)
        {
            if (dgvLocales.SelectedCells.Count > 0)
            {
                string nombreLocal = dgvLocales.SelectedCells[0].Value.ToString();
                frmEventosLocal frm = new frmEventosLocal();
                frm.NombreLocal = nombreLocal;
                frm.Mail = Mail;
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un local", "Hola Pedro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuVendedor frm = new frmMenuVendedor();
            frm.Mail = Mail;
            frm.Show(); this.Hide();
        }
        private void frmMisLocales_Load(object sender, EventArgs e)
        {
            dgvLocales.DataSource = GestorPropietario.Instance.ObtenerListaLocales(Mail);
            dgvLocales.Refresh();
            DataGridViewColumn columnaNombre = new DataGridViewColumn();
            DataGridViewColumn columnaUbicacion = new DataGridViewColumn();
            dgvLocales.AutoGenerateColumns = true;
            dgvLocales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
    }
}
