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
using Microsoft.Data.SqlClient;
using Visual.Vendedor;

namespace Visual.Propietario
{
    public partial class frmAgregarEvento : Form
    {
        public int IDUsuario; public int IDlocal; public string NombreUsuario; public string NombreLocal;
        public SqlConnection ConexionSql; public GestorEventos gestorEventos = new GestorEventos();
        public frmAgregarEvento()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            gestorEventos.AgregarEvento(txtNombre.Text, dtEvento.Value, IDlocal, ConexionSql, out int IDEvento);
            MessageBox.Show("Evento agregado con exito");
            frmAdministrarEvento frm = new frmAdministrarEvento();
            frm.ConexionSql = ConexionSql;
            frm.IDlocal = IDlocal;
            frm.NombreUsuario= NombreUsuario;
            frm.IDEvento = IDEvento; 
            frm.NombreLocal = NombreLocal;
            frm.NombreEvento = txtNombre.Text;
            frm.IDUsuario = IDUsuario;

            frm.Show(); this.Hide();
        }

        private void lblVolver_Click(object sender, EventArgs e)
        {
            frmEventosLocal frm = new frmEventosLocal();
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.IDlocal = IDlocal;
            frm.IDUsuario = IDUsuario;
            frm.NombreLocal = NombreLocal;
            frm.Show(); this.Hide();
        }

        // ------------------------------------- metodo -------------------------------------
    }
}
