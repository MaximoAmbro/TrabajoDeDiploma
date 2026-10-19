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
using Visual.Vendedor;

namespace Visual.Propietario
{
    public partial class frmAgregarEvento : Form
    {
        public int IDUsuario; public int IDlocal; public string NombreUsuario; public string NombreLocal;
        public frmAgregarEvento()
        {
            InitializeComponent();
            // Items y no DataSource: con DataSource, WinForms preselecciona el primer valor al
            // mostrarse y la categoría dejaría de ser una elección explícita.
            cmbCategoria.Items.AddRange(GestorEventos.Categorias.ToArray<object>());
        }

        // El evento se guarda en Borrador y se vuelve al listado. Se publica solo al agregarle,
        // desde Administrar evento, su primer sector con capacidad mayor que cero.
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
                // RN-39: destildado = sin preventa.
                FinPreventa = dtpFinPreventa.Checked ? dtpFinPreventa.Value : (DateTime?)null
            };

            try
            {
                GestorEventos.Instance.Crear(evento);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Evento guardado como borrador. Para publicarlo, agréguele un sector desde «Administrar Evento».");
            VolverAlListado();
        }

        private void lblVolver_Click(object sender, EventArgs e)
        {
            VolverAlListado();
        }

        private void VolverAlListado()
        {
            frmEventosLocal frm = new frmEventosLocal();
            frm.NombreUsuario = NombreUsuario;
            frm.IDlocal = IDlocal;
            frm.IDUsuario = IDUsuario;
            frm.NombreLocal = NombreLocal;
            frm.Show(); this.Hide();
        }

        // ------------------------------------- metodo -------------------------------------
    }
}
