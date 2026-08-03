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

namespace Visual
{
    public partial class frmHistorialCliente : Form
    {
        public string Mail { get; set; }
        public frmHistorialCliente()
        {
            InitializeComponent();
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuCliente frm = new frmMenuCliente();
            frm.Mail = Mail;
            frm.Show();
            dgvHistorial.ClearSelection();
            this.Hide();
        }
        private void frmHistorial_Load(object sender, EventArgs e)
        {
             dgvHistorial.DataSource = GestorClientes.Instance.ObtenerListaEventos(Mail);
             dgvHistorial.Refresh();
             DataGridViewColumn columnaNombre = new DataGridViewColumn();
             DataGridViewColumn columnaUbicacion = new DataGridViewColumn();
             dgvHistorial.AutoGenerateColumns = true;
             dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
    }
}
