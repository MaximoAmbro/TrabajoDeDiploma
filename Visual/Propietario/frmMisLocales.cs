
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using Visual.Vendedor;

namespace Visual
{
    public partial class frmMisLocales : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;
        private Componentes.EstadoVacio estadoSinLocales;
        public frmMisLocales()
        {
            InitializeComponent();
            dgvLocales.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && btnEventosLocal.Enabled)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    btnEventosLocal.PerformClick();
                }
            };
            estadoSinLocales = new Componentes.EstadoVacio { Name = "estadoSinLocales", Visible = false };
            estadoSinLocales.AjustarA(dgvLocales);
            estadoSinLocales.Cargar("Todavía no registraste un local. Cuando lo hagas, vas a poder publicar eventos desde acá.", "Registrar local");
            estadoSinLocales.Accion += AbrirRegistroLocal;
            Controls.Add(estadoSinLocales);
            Shown += (s, e) =>
            {
                ColocarTitulo(label2);
                Disposicion.ExtenderGrilla(dgvLocales, btnVolver);
                estadoSinLocales.AjustarA(dgvLocales);
            };
        }

        private void frmMisLocales_Load(object sender, EventArgs e)
        {
            var locales = GestorLocales.Instance.PorPropietario(IDUsuario);
            dgvLocales.DataSource = locales
                .Select(l => new { ID = l.Id, l.Nombre, l.Direccion, l.Capacidad })
                .ToList();

            FormatearGrilla();
            dgvLocales.Visible = locales.Count > 0;
            estadoSinLocales.Visible = locales.Count == 0;
            btnEventosLocal.Enabled = locales.Count > 0;
        }

        private void AbrirRegistroLocal()
        {
            var form = new frmRegistrarLocal { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            form.Show(); Hide();
        }

        private void FormatearGrilla()
        {
            dgvLocales.ReadOnly = true;
            dgvLocales.AllowUserToAddRows = false;
            dgvLocales.AllowUserToDeleteRows = false;
            dgvLocales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Tema.EstiloGrilla(dgvLocales);
            dgvLocales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvLocales.Columns["ID"].Visible = false;
            dgvLocales.Columns["Nombre"].HeaderText = "Nombre del local";
            dgvLocales.Columns["Direccion"].HeaderText = "Dirección";
            dgvLocales.Columns["Capacidad"].HeaderText = "Capacidad";
            dgvLocales.Columns["Capacidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvLocales.Columns["Capacidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnEventosLocal_Click(object sender, EventArgs e)
        {
            if (dgvLocales.CurrentRow != null && dgvLocales.SelectedRows.Count > 0)
            {
                int idLocalSeleccionado = Convert.ToInt32(dgvLocales.CurrentRow.Cells["ID"].Value);
                string nombreLocal = dgvLocales.CurrentRow.Cells["Nombre"].Value.ToString();
                frmEventosLocal frm = new frmEventosLocal();
                frm.NombreLocal = nombreLocal;
                frm.NombreUsuario = NombreUsuario;
                frm.IDlocal = idLocalSeleccionado;
                frm.IDUsuario = IDUsuario;
                frm.Show();
                this.Hide();
            }
            else
            {
                Avisos.Informativo(this, "Elegí un local para ver sus eventos.");
            }
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
        }
    }
}
