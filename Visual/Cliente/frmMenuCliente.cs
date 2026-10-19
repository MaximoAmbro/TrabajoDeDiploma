using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Visual
{
    public partial class frmMenuCliente : Form
    {
        public int IDUsuario;
        public string NombreUsuario;
        public frmMenuCliente()
        {
            InitializeComponent();
        }
        private void frmMenuCliente_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreUsuario;

        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnEventos_Click(object sender, EventArgs e)
        {
            frmEventosCliente frm = new frmEventosCliente();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            frmHistorialCliente frm = new frmHistorialCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
        private void btnSuscripcion_Click(object sender, EventArgs e)
        {
            frmSuscripcion frm = new frmSuscripcion();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
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
