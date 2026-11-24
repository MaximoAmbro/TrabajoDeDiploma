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
    public partial class frmEventosLocal : FormularioEventAura
    {
        public int IDUsuario; public int IDlocal;
        public string NombreLocal { get; set; } public string NombreUsuario { get; set; }
        private Componentes.EstadoVacio estadoSinEventos;
        public frmEventosLocal()
        {
            InitializeComponent();
            Shown += (s, e) =>
            {
                ColocarTitulo(label2);
                Disposicion.AlinearGrupo(btnVolver, btnEliminarEvento, btnAdministrarEvento, btnAgregarEvento);
                int margen = Disposicion.Pixeles(this, Tema.Espacio.XXL);
                ClientSize = new System.Drawing.Size(Math.Max(ClientSize.Width, btnAgregarEvento.Right + margen), ClientSize.Height);
                Disposicion.ExtenderGrilla(dgvEventos, btnVolver);
                dgvEventos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            };
            AcceptButton = btnAgregarEvento;
            dgvEventos.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && AcceptButton is Button boton && boton.Enabled)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    boton.PerformClick();
                }
            };
            estadoSinEventos = new Componentes.EstadoVacio { Name = "estadoSinEventos", Visible = false };
            estadoSinEventos.AjustarA(dgvEventos);
            estadoSinEventos.Cargar("Todavía no hay eventos en este local. Creá uno para empezar a ofrecer entradas.", "Agregar evento");
            estadoSinEventos.Accion += () => btnAgregarEvento.PerformClick();
            Controls.Add(estadoSinEventos);
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
                Avisos.Informativo(this, "Elegí un evento para administrarlo.");
                return;
            }

            int idEvento = Convert.ToInt32(dgvEventos.CurrentRow.Cells["ID"].Value);
            string nombre = Convert.ToString(dgvEventos.CurrentRow.Cells["Nombre"].Value);
            if (MessageBox.Show($"¿Eliminar el evento «{nombre}»? Esta acción no se puede deshacer.",
                "Eliminar evento", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                GestorEventos.Instance.Eliminar(idEvento);
            }
            catch (ReglaNegocioException ex)
            {
                DialogResult respuesta = MessageBox.Show(ex.Message + "\n\n¿Querés suspenderlo?",
                    "Suspender evento", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (respuesta == DialogResult.Yes)
                {
                    GestorEventos.Instance.Suspender(idEvento);
                }
            }
            catch (InvalidOperationException ex)
            {
                Avisos.Informativo(this, ex.Message);
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
        }
        private void btnAdministrarEvento_Click(object sender, EventArgs e)
        {
            if (dgvEventos.CurrentRow == null)
            {
                Avisos.Informativo(this, "Elegí un evento para administrarlo.");
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
            var eventos = GestorEventos.Instance.PorLocal(IDlocal);
            dgvEventos.DataSource = eventos
                .Select(ev => new { ID = ev.Id, ev.Nombre, ev.Fecha, ev.Estado, MotivoDelRechazo = ev.MotivoRechazo })
                .ToList();
            dgvEventos.Visible = eventos.Count > 0;
            estadoSinEventos.Visible = eventos.Count == 0;
            if (dgvEventos.Columns.Contains("ID")) dgvEventos.Columns["ID"].Visible = false;
            if (dgvEventos.Columns.Contains("MotivoDelRechazo")) dgvEventos.Columns["MotivoDelRechazo"].HeaderText = "Motivo del rechazo";
        }
    }
}
