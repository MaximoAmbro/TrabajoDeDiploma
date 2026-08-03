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

namespace Visual
{
    public partial class frmMenuVendedor : Form
    {
        public string NombreUsuario { get; set; }
        public string Mail { get; set; }
        public frmMenuVendedor()
        {
            InitializeComponent();
        }

        private void btnMisLocales_Click(object sender, EventArgs e)
        {
            frmMisLocales frm = new frmMisLocales();
            frm.Mail = Mail;
            frm.Show(); this.Hide();
        }
        private void btnRegistrarLocal_Click(object sender, EventArgs e)
        {
            frmRegistrarLocal frm = new frmRegistrarLocal();
            frm.Mail = Mail;
            frm.Show(); this.Hide();
        }
        private void frmMenuVendedor_Load(object sender, EventArgs e)
        {
            GestorPropietario.Instance.DevolverNombre(Mail, out string NombreUsuario);
            lblNombre.Text = NombreUsuario;
        }
        private void lblvolver_Click(object sender, EventArgs e)
        {
            frmInicio frmInicio = new frmInicio();
            
            frmInicio.Show(); this.Hide();
        }
    }
}
