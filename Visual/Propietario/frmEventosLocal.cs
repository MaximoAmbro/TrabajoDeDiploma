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
using Visual.Propietario;

namespace Visual.Vendedor
{
    public partial class frmEventosLocal : Form
    {
        public int IDUsuario; public int IDlocal;
        public string NombreLocal { get; set; } public string NombreUsuario { get; set; }
        public frmEventosLocal()
        {
            InitializeComponent();
        }

        private void frmEventosLocal_Load(object sender, EventArgs e)
        {
            CargaEventos();
        }

        // RN-11: si el evento tiene entradas vendidas, Eliminar lo rechaza y se ofrece suspenderlo.
        private void btnEliminarEvento_Click(object sender, EventArgs e)
        {
            if (dgvEventos.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un evento");
                return;
            }

            int idEvento = Convert.ToInt32(dgvEventos.CurrentRow.Cells["ID"].Value);
            try
            {
                GestorEventos.Instance.Eliminar(idEvento);
            }
            catch (ReglaNegocioException ex)
            {
                DialogResult respuesta = MessageBox.Show(ex.Message + "\n\n¿Desea suspenderlo?",
                    ex.Regla, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    GestorEventos.Instance.Suspender(idEvento);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            CargaEventos();
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMisLocales frm = new frmMisLocales();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
        }
        private void btnAgregarEvento_Click(object sender, EventArgs e)
        {
            Propietario.frmAgregarEvento frm = new Propietario.frmAgregarEvento();
            frm.IDlocal = IDlocal;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.NombreLocal = NombreLocal;
            frm.Show(); this.Close();
        } // este es modificar pero me da paja cambiar el nombre
        private void btnAdministrarEvento_Click(object sender, EventArgs e)
        {
            if (dgvEventos.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un evento");
                return;
            }
            frmAdministrarEvento frm = new frmAdministrarEvento();
            int EventoSeleccionado = Convert.ToInt32(dgvEventos.CurrentRow.Cells["ID"].Value);
            frm.IDlocal = IDlocal;
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.NombreLocal = NombreLocal;
            frm.IDEvento = EventoSeleccionado;
            frm.NombreEvento = Convert.ToString(dgvEventos.CurrentRow.Cells["Nombre"].Value);
            frm.Show(); this.Close();

        }


        // --------------------------------Funciones--------------------------------
        public void CargaEventos()
        {
            lblNombre.Text = NombreLocal;
            dgvEventos.DataSource = GestorEventos.Instance.PorLocal(IDlocal)
                .Select(ev => new { ID = ev.Id, ev.Nombre, ev.Fecha, ev.Estado })
                .ToList();
            dgvEventos.Columns["ID"].Visible = false;
        }
    }
}
