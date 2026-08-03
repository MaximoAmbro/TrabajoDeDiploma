using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades; using Negocio;
namespace Visual
{
    public partial class frmEventosCliente : Form
    {
        private GestorEventos gestorEventos;
        public string Mail { get; set; }

        public frmEventosCliente()
        {
            InitializeComponent();
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuCliente frm = new frmMenuCliente();
            frm.Mail = Mail;
            frm.Show();
            this.Hide();
        }
        private void frmEventos_Load(object sender, EventArgs e)
        {
            gestorEventos = new GestorEventos();
            dgveventos.DataSource = gestorEventos.ObtenerListaEventos();
            DataGridViewColumn columnaNombre = new DataGridViewColumn();
            DataGridViewColumn columnaUbicacion = new DataGridViewColumn();
            dgveventos.AutoGenerateColumns = true;
            dgveventos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }
        private void btnComprar_Click_1(object sender, EventArgs e)
        {
            if (dgveventos.SelectedCells.Count > 0)
            {
                string nombre = dgveventos.SelectedCells[0].Value.ToString();
                FrmCompraCliente frm = new FrmCompraCliente();
                frm.NombreEvento = nombre;
                frm.Mail = Mail;
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un evento para comprar", "Hola Pedro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }    
    }
}
