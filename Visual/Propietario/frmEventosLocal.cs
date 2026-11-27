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
        private readonly Web.PanelWeb eventosWeb = new Web.PanelWeb { Name = "eventosLocalWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoWeb = new Label { Name = "avisoEventosLocalWeb", AutoSize = true, ForeColor = Tema.TextoSecundario, Text = "Vista web no disponible. Se muestra la lista de Windows Forms.", Visible = false };
        public frmEventosLocal()
        {
            InitializeComponent();
            Shown += (s, e) =>
            {
                ColocarTitulo(label2);
                Disposicion.AlinearGrupo(btnVolver, btnEliminarEvento, btnAdministrarEvento, btnAgregarEvento);
                int margen = Disposicion.Pixeles(this, Tema.Espacio.XXL);
                ClientSize = new System.Drawing.Size(Math.Max(ClientSize.Width, btnAgregarEvento.Right + margen), ClientSize.Height);
                int separacion = Disposicion.Pixeles(this, Tema.Espacio.XL);
                int finalEncabezado = label2.Bottom;
                if (!string.IsNullOrWhiteSpace(lblNombre.Text))
                {
                    lblNombre.Font = Tema.Cuerpo;
                    lblNombre.Left = label2.Left;
                    lblNombre.Top = label2.Bottom + Disposicion.Pixeles(this, Tema.Espacio.S);
                    lblNombre.Height = lblNombre.GetPreferredSize(new System.Drawing.Size(lblNombre.Width, 0)).Height;
                    finalEncabezado = lblNombre.Bottom;
                }
                dgvEventos.Top = finalEncabezado + separacion;
                Disposicion.ExtenderGrilla(dgvEventos, btnVolver);
                estadoSinEventos.AjustarA(dgvEventos);
                dgvEventos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                AjustarWebEventos();
                eventosWeb.Cargar("gestion.html");
            };
            Controls.Add(eventosWeb);
            Controls.Add(avisoWeb);
            eventosWeb.VistaLista += ActualizarVistaEventos;
            eventosWeb.VistaNoDisponible += ActualizarVistaEventos;
            eventosWeb.Escape += () => { Message m = default(Message); ProcessCmdKey(ref m, Keys.Escape); };
            eventosWeb.Mensaje += RecibirMensajeEventos;
            SizeChanged += (s, e) => AjustarWebEventos();
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
            if (DialogoEventAura.Confirmar(this, "Eliminar evento",
                $"¿Eliminar el evento «{nombre}»? Esta acción no se puede deshacer.",
                "Eliminar evento", "Volver", true) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                GestorEventos.Instance.Eliminar(idEvento);
            }
            catch (ReglaNegocioException ex)
            {
                DialogResult respuesta = DialogoEventAura.Confirmar(this, "Suspender evento",
                    ex.Message + "\n\n¿Querés suspenderlo?", "Suspender evento", "Volver", true);
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
            dgvEventos.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvEventos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            if (dgvEventos.Columns.Contains("MotivoDelRechazo")) dgvEventos.Columns["MotivoDelRechazo"].HeaderText = "Motivo del rechazo";
            ActualizarModeloEventos(eventos);
        }

        private void AjustarWebEventos()
        {
            if (eventosWeb == null || IsDisposed) return;
            eventosWeb.Bounds = dgvEventos.Bounds;
            avisoWeb.Location = new Point(dgvEventos.Left, Math.Max(0, dgvEventos.Top - avisoWeb.Height));
        }

        private void ActualizarVistaEventos()
        {
            bool web = eventosWeb.Preparado;
            label2.Text = "Eventos del local";
            label2.Visible = !web;
            lblNombre.Visible = !web;
            eventosWeb.Visible = web;
            avisoWeb.Visible = eventosWeb.InicializacionTerminada && !web;
            // La grilla conserva su ruta de teclado y queda cubierta por el panel web.
            dgvEventos.Visible = dgvEventos.Rows.Count > 0;
            estadoSinEventos.Visible = !web && dgvEventos.Rows.Count == 0;
            if (web) eventosWeb.BringToFront(); else avisoWeb.BringToFront();
        }

        private void ActualizarModeloEventos(System.Collections.Generic.IEnumerable<Entidades.Evento> origen = null)
        {
            var eventos = origen?.ToArray() ?? GestorEventos.Instance.PorLocal(IDlocal).ToArray();
            int? seleccionado = dgvEventos.CurrentRow == null || !dgvEventos.Columns.Contains("ID")
                ? (int?)null : Convert.ToInt32(dgvEventos.CurrentRow.Cells["ID"].Value);
            eventosWeb.Actualizar(new { vista = "eventosLocal", local = NombreLocal, seleccionado,
                eventos = eventos.Select(e => new { id = e.Id, nombre = e.Nombre,
                    fecha = e.Fecha.ToString("dd/MM/yyyy HH:mm"), estado = e.Estado,
                    motivo = e.MotivoRechazo }).ToArray() });
        }

        private void RecibirMensajeEventos(string texto)
        {
            var tipo = System.Text.RegularExpressions.Regex.Match(texto ?? "", "\\\"tipo\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"");
            if (!tipo.Success) return;
            if (tipo.Groups[1].Value == "agregar") { btnAgregarEvento.PerformClick(); return; }
            var id = System.Text.RegularExpressions.Regex.Match(texto, "\\\"id\\\"\\s*:\\s*(\\d+)");
            if (tipo.Groups[1].Value != "eventoLocal" || !id.Success || !int.TryParse(id.Groups[1].Value, out int eventoId)) return;
            var fila = dgvEventos.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => Convert.ToInt32(r.Cells["ID"].Value) == eventoId);
            if (fila != null) { fila.Selected = true; dgvEventos.CurrentCell = fila.Cells["Nombre"]; }
            ActualizarModeloEventos();
        }
    }
}
