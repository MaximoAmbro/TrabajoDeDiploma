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
using Microsoft.Data.SqlClient;
using Visual.Vendedor;

namespace Visual.Propietario
{
    public partial class frmAgregarEvento : Form
    {
        public int IDUsuario; public int IDlocal; public string NombreUsuario; public string NombreLocal;
        public SqlConnection ConexionSql; // TODO Seccion 5-7: quitar ConexionSql
        public frmAgregarEvento()
        {
            InitializeComponent();
            // Items y no DataSource: con DataSource, WinForms preselecciona el primer valor al
            // mostrarse y la categoría dejaría de ser una elección explícita.
            cmbCategoria.Items.AddRange(GestorEventos.Categorias.ToArray<object>());
        }

        // RN-06: el evento todavía no se guarda. Se publica en frmAgregarSector junto con su
        // primer sector con capacidad mayor que cero.
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del evento.");
                return;
            }
            if (cmbCategoria.SelectedItem == null)
            {
                MessageBox.Show("Seleccione la categoría del evento.");
                return;
            }

            Evento evento = new Evento
            {
                Nombre = txtNombre.Text.Trim(),
                Fecha = dtEvento.Value,
                IdLocal = IDlocal,
                Categoria = cmbCategoria.SelectedItem.ToString(),
                Sectores = new List<Sector>()
            };

            MessageBox.Show("Agregue al menos un sector con capacidad mayor que cero para publicar el evento.");
            frmAgregarSector frm = new frmAgregarSector();
            frm.ConexionSql = ConexionSql;
            frm.IDlocal = IDlocal;
            frm.NombreUsuario = NombreUsuario;
            frm.EventoPendiente = evento;
            frm.NombreEvento = evento.Nombre;
            frm.NombreLocal = NombreLocal;
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
