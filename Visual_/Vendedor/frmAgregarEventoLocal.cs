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
    public partial class frmAgregarEventoLocal : Form
    {
        public string Mail { get; set; }
        public string NombreLocal { get; set; }
        public string NombreEvento { get; set; }
        public frmAgregarEventoLocal()
        {
            InitializeComponent();
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmEventosLocal form = new frmEventosLocal();
            form.Mail = Mail;
            form.NombreLocal = NombreLocal;
            form.Show(); this.Hide();
        }
        private void frmAgregarEventoLocal_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreLocal;
            CargarDGV();
        }
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            DateTime fechaHora = dtpFechaHora.Value;
            DateTime fecha = fechaHora.Date;
            TimeSpan hora = fechaHora.TimeOfDay;
            NombreEvento = txtNombre.Text;
            GestorPropietario.Instance.AgregarEventoLocal(Mail, NombreLocal, txtNombre.Text, hora, fecha);
            CargarDGV();
        }
        public void CargarDGV()
        {
            dgvSectores.DataSource = GestorPropietario.Instance.ObtenerListaSectores(Mail, NombreLocal, NombreEvento);
            dgvSectores.Refresh();
            DataGridViewColumn columnaNombre = new DataGridViewColumn();
            DataGridViewColumn columnaUbicacion = new DataGridViewColumn();
            dgvSectores.AutoGenerateColumns = true;
            dgvSectores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
    }
}
