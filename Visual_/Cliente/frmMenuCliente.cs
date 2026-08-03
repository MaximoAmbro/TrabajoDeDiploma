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
    public partial class frmMenuCliente : Form
    {
        public string NombreUsuario { get; set; }
        public string Mail { get; set; }
        public frmMenuCliente()
        {
            InitializeComponent();
        }
        private void frmMenuUsuario_Load(object sender, EventArgs e)
        {
            GestorClientes.Instance.ObtenerNombre(Mail, out string NombreUsuario);
            GestorClientes gestorClientes = new GestorClientes();
            lblNombre.Text = NombreUsuario;
        }
        private void btnEventos_Click(object sender, EventArgs e)
        {
            frmEventosCliente frm = new frmEventosCliente();
            frm.Mail = Mail;
            frm.Show();
            this.Hide();
        }
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            frmHistorialCliente frm = new frmHistorialCliente();
            frm.Mail = Mail;
            frm.Show();
            this.Hide();
        }
        private void lblvolver_Click(object sender, EventArgs e)
        {
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        }
    }
}
