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

namespace Visual.Vendedor
{
    public partial class frmEventosLocal : Form
    {
        public string Mail { get; set; }
        public string NombreLocal { get; set; }
        public frmEventosLocal()
        {
            InitializeComponent();
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMisLocales frm = new frmMisLocales();
            frm.Mail = Mail;
            frm.Show(); this.Hide();
        }
        private void frmRegistrarEvento_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreLocal;
            dgvEventos.DataSource = GestorPropietario.Instance.ObtenerListaEventos(Mail, NombreLocal);
            dgvEventos.Refresh();
            DataGridViewColumn columnaNombre = new DataGridViewColumn();
            DataGridViewColumn columnaUbicacion = new DataGridViewColumn();
            dgvEventos.AutoGenerateColumns = true;
            dgvEventos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }
        private void btnAgregarEvento_Click(object sender, EventArgs e)
        {
            frmAgregarEventoLocal frm = new frmAgregarEventoLocal();
            frm.Mail = Mail;
            frm.NombreLocal = NombreLocal;
            frm.Show(); this.Close();
        }
        private void btnEliminarEvento_Click(object sender, EventArgs e)
        {
            if (dgvEventos.SelectedCells.Count > 0)
            {
                string NombreEvento = dgvEventos.SelectedCells[0].Value.ToString();
                GestorPropietario.Instance.EliminarEvento(Mail, NombreLocal, NombreEvento);
                frmEventosLocal frm = new frmEventosLocal();
                frm .Mail = Mail;
                frm.NombreLocal = NombreLocal;
                frm.Show(); this.Hide();
            }
        }
    }
}
