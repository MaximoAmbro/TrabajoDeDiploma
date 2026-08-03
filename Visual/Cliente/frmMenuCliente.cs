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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Visual
{
    public partial class frmMenuCliente : Form
    {
        public int IDUsuario;
        public SqlConnection ConexionSql;
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
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            frmHistorialCliente frm = new frmHistorialCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.ConexionSql = ConexionSql;
            frm.Show();
            this.Hide();
        }
        private void lblvolver_Click(object sender, EventArgs e)
        {
            
            frmInicio frm = new frmInicio();
            frm.ConexionSql = ConexionSql;
            frm.Show();
            this.Hide();
        }
    }
}
