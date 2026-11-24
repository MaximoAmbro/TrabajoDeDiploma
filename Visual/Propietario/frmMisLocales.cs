
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
        private readonly Web.PanelWeb localesWeb = new Web.PanelWeb { Name = "localesWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoWeb = new Label { Name = "avisoLocalesWeb", AutoSize = true, ForeColor = Tema.TextoSecundario, Text = "Vista web no disponible. Se muestra la lista de Windows Forms.", Visible = false };
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
            Controls.Add(localesWeb);
            Controls.Add(avisoWeb);
            localesWeb.VistaLista += ActualizarVistaLocales;
            localesWeb.VistaNoDisponible += ActualizarVistaLocales;
            localesWeb.Escape += () => { Message m = default(Message); ProcessCmdKey(ref m, Keys.Escape); };
            localesWeb.Mensaje += RecibirMensajeLocales;
            Shown += (s, e) =>
            {
                ColocarTitulo(label2);
                Disposicion.ExtenderGrilla(dgvLocales, btnVolver);
                estadoSinLocales.AjustarA(dgvLocales);
                AjustarWebLocales();
                localesWeb.Cargar("gestion.html");
            };
            SizeChanged += (s, e) => AjustarWebLocales();
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
            ActualizarModeloLocales(locales);
        }

        private void AjustarWebLocales()
        {
            if (localesWeb == null || IsDisposed) return;
            localesWeb.Bounds = dgvLocales.Bounds;
            avisoWeb.Location = new Point(dgvLocales.Left, Math.Max(0, dgvLocales.Top - avisoWeb.Height));
        }

        private void ActualizarVistaLocales()
        {
            bool web = localesWeb.Preparado;
            localesWeb.Visible = web;
            avisoWeb.Visible = localesWeb.InicializacionTerminada && !web;
            // La grilla queda debajo del panel web para conservar el acceso por teclado y el respaldo.
            dgvLocales.Visible = dgvLocales.Rows.Count > 0;
            estadoSinLocales.Visible = !web && dgvLocales.Rows.Count == 0;
            if (web) localesWeb.BringToFront(); else avisoWeb.BringToFront();
        }

        private void ActualizarModeloLocales(System.Collections.Generic.IEnumerable<Entidades.Local> origen = null)
        {
            var locales = origen?.ToArray() ?? dgvLocales.Rows.Cast<DataGridViewRow>().Select(r => new Entidades.Local
                { Id = Convert.ToInt32(r.Cells["ID"].Value), Nombre = Convert.ToString(r.Cells["Nombre"].Value),
                  Direccion = Convert.ToString(r.Cells["Direccion"].Value), Capacidad = Convert.ToInt32(r.Cells["Capacidad"].Value) }).ToArray();
            int? seleccionado = dgvLocales.CurrentRow == null ? (int?)null : Convert.ToInt32(dgvLocales.CurrentRow.Cells["ID"].Value);
            localesWeb.Actualizar(new { vista = "locales", seleccionado, locales = locales.Select(l => new
                { id = l.Id, nombre = l.Nombre, direccion = l.Direccion, capacidad = l.Capacidad }).ToArray() });
        }

        private void RecibirMensajeLocales(string texto)
        {
            var tipo = System.Text.RegularExpressions.Regex.Match(texto ?? "", "\\\"tipo\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"");
            if (!tipo.Success) return;
            if (tipo.Groups[1].Value == "registro") { AbrirRegistroLocal(); return; }
            var id = System.Text.RegularExpressions.Regex.Match(texto, "\\\"id\\\"\\s*:\\s*(\\d+)");
            if (tipo.Groups[1].Value != "local" || !id.Success || !int.TryParse(id.Groups[1].Value, out int localId)) return;
            var fila = dgvLocales.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => Convert.ToInt32(r.Cells["ID"].Value) == localId);
            if (fila != null) { fila.Selected = true; dgvLocales.CurrentCell = fila.Cells["Nombre"]; }
            ActualizarModeloLocales();
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
