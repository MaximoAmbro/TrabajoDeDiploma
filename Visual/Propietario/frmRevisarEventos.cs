using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public sealed class frmRevisarEventos : FormularioEventAura
    {
        private readonly Componentes.EtiquetaTitulo tituloRevision = new Componentes.EtiquetaTitulo { Name = "tituloRevisionEventos", Text = "Revisión de eventos", Location = new Point(24, 12), Size = new Size(700, 44) };
        private readonly ListBox pendientes = new ListBox { Name = "lstEventosEnRevision", Location = new Point(24, 88), Size = new Size(620, 400), DisplayMember = "Nombre" };
        private readonly Label detalle = new Componentes.EtiquetaAyuda { Name = "lblDetalleEventoRevision", Location = new Point(670, 88), Size = new Size(360, 120) };
        private readonly Label etiquetaMotivo = new Componentes.EtiquetaAyuda { Name = "lblMotivoRechazoEvento", Text = "Motivo del rechazo (obligatorio)", Location = new Point(670, 210), Size = new Size(350, 20) };
        private readonly TextBox motivo = new TextBox { Name = "txtMotivoRechazoEvento", Location = new Point(670, 232), Size = new Size(350, 60), Multiline = true, MaxLength = 500, ScrollBars = ScrollBars.Vertical };
        private readonly Button aprobar = new Componentes.BotonPrimario { Name = "btnAprobarPrimerEvento", Text = "Aprobar y publicar", Location = new Point(670, 310), Size = new Size(300, 44) };
        private readonly Button rechazar = new Componentes.BotonPeligro { Name = "btnRechazarPrimerEvento", Text = "Rechazar y devolver", Location = new Point(670, 366), Size = new Size(300, 44) };
        private readonly Button cerrar = new Componentes.BotonSecundario { Name = "btnCerrarRevision", Text = "Cerrar", Location = new Point(24, 510), Size = new Size(140, 40) };
        private readonly Componentes.EstadoVacio estadoSinEventos = new Componentes.EstadoVacio { Name = "estadoSinEventosRevision", Visible = false };
        private readonly Web.PanelWeb revisionWeb = new Web.PanelWeb { Name = "revisionEventosWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoWeb = new Label { Name = "avisoRevisionEventosWeb", AutoSize = true, ForeColor = Tema.TextoSecundario, Text = "Vista web no disponible. Se muestra la revisión de Windows Forms.", Visible = false };

        public frmRevisarEventos()
        {
            GestorSesion.Instance.RequerirAdministrador();
            Text = "Revisión de eventos";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1100, 720);
            MinimumSize = new Size(1000, 680);
            Controls.Add(tituloRevision);
            Controls.AddRange(new Control[] { pendientes, detalle, etiquetaMotivo, motivo, aprobar, rechazar, cerrar });
            CancelButton = cerrar;
            Controls.Add(revisionWeb);
            Controls.Add(avisoWeb);
            revisionWeb.VistaLista += ActualizarVistaRevision;
            revisionWeb.VistaNoDisponible += ActualizarVistaRevision;
            revisionWeb.Escape += () => { Message m = default(Message); ProcessCmdKey(ref m, Keys.Escape); };
            revisionWeb.Mensaje += RecibirMensajeRevision;
            estadoSinEventos.AjustarA(pendientes);
            estadoSinEventos.Cargar("Todavía no hay eventos pendientes de revisión. Cuando un Organizador publique su primer evento, va a aparecer acá.", "Actualizar");
            estadoSinEventos.Accion += Cargar;
            Controls.Add(estadoSinEventos);
            pendientes.SelectedIndexChanged += (s, e) => MostrarSeleccion();
            aprobar.Click += (s, e) => Aprobar();
            rechazar.Click += (s, e) => Rechazar();
            cerrar.Click += (s, e) => Close();
            Load += (s, e) => Cargar();
            Shown += (s, e) => { AjustarWebRevision(); revisionWeb.Cargar("gestion.html"); };
            SizeChanged += (s, e) => AjustarWebRevision();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                cerrar.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Cargar()
        {
            pendientes.DataSource = null;
            pendientes.DataSource = GestorEventos.Instance.PendientesDeRevision();
            bool hayPendientes = pendientes.Items.Count > 0;
            pendientes.Visible = hayPendientes;
            estadoSinEventos.Visible = !hayPendientes;
            MostrarSeleccion();
            ActualizarModeloRevision();
        }

        private void MostrarSeleccion()
        {
            Evento evento = pendientes.SelectedItem as Evento;
            detalle.Text = evento == null ? "No hay eventos pendientes." :
                evento.Nombre + Environment.NewLine + evento.Fecha.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + "Al aprobarlo, aparece en el catálogo. Si se rechaza, vuelve a Borrador con el motivo.";
            aprobar.Enabled = evento != null;
            rechazar.Enabled = evento != null;
            motivo.Clear();
            if (revisionWeb != null) ActualizarModeloRevision();
        }

        private void AjustarWebRevision()
        {
            if (revisionWeb == null || IsDisposed) return;
            int top = pendientes.Top;
            int bottom = Math.Min(aprobar.Top - 8, ClientSize.Height - 64);
            revisionWeb.SetBounds(pendientes.Left, top, Math.Max(1, ClientSize.Width - pendientes.Left * 2), Math.Max(1, bottom - top));
            avisoWeb.Location = new Point(revisionWeb.Left, Math.Max(0, revisionWeb.Top - avisoWeb.Height));
        }

        private void ActualizarVistaRevision()
        {
            bool web = revisionWeb.Preparado;
            tituloRevision.Visible = !web;
            revisionWeb.Visible = web;
            avisoWeb.Visible = revisionWeb.InicializacionTerminada && !web;
            pendientes.Visible = !web && pendientes.Items.Count > 0;
            detalle.Visible = etiquetaMotivo.Visible = motivo.Visible = !web;
            estadoSinEventos.Visible = !web && pendientes.Items.Count == 0;
            if (web) revisionWeb.BringToFront(); else avisoWeb.BringToFront();
        }

        private void ActualizarModeloRevision()
        {
            var eventos = pendientes.Items.Cast<Evento>().Select(e => new
                { id = e.Id, nombre = e.Nombre, fecha = e.Fecha.ToString("dd/MM/yyyy HH:mm") }).ToArray();
            Evento seleccionado = pendientes.SelectedItem as Evento;
            revisionWeb.Actualizar(new { vista = "revisionEventos", eventos, seleccionado = seleccionado?.Id,
                nombre = seleccionado?.Nombre, detalle = detalle.Text, motivo = motivo.Text });
        }

        private void RecibirMensajeRevision(string texto)
        {
            var tipo = System.Text.RegularExpressions.Regex.Match(texto ?? "", "\\\"tipo\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"");
            if (!tipo.Success) return;
            if (tipo.Groups[1].Value == "actualizar") { Cargar(); return; }
            if (tipo.Groups[1].Value == "motivoRevision")
            {
                var valor = System.Text.RegularExpressions.Regex.Match(texto, "\\\"texto\\\"\\s*:\\s*\"((?:\\\\.|[^\"])*)\"");
                if (valor.Success) motivo.Text = Newtonsoft.Json.JsonConvert.DeserializeObject<string>("\"" + valor.Groups[1].Value + "\"");
                return;
            }
            var id = System.Text.RegularExpressions.Regex.Match(texto, "\\\"id\\\"\\s*:\\s*(\\d+)");
            if (tipo.Groups[1].Value != "revision" || !id.Success || !int.TryParse(id.Groups[1].Value, out int eventoId)) return;
            for (int i = 0; i < pendientes.Items.Count; i++)
                if (((Evento)pendientes.Items[i]).Id == eventoId) { pendientes.SelectedIndex = i; break; }
        }

        private void Aprobar()
        {
            Evento evento = pendientes.SelectedItem as Evento;
            if (evento == null) return;
            try
            {
                GestorEventos.Instance.AprobarPrimerEvento(evento.Id);
                Cargar();
            }
            catch (Exception ex) when (ex is ReglaNegocioException || ex is InvalidOperationException)
            {
                Avisos.Informativo(this, ex.Message);
                Cargar();
            }
        }

        private void Rechazar()
        {
            Evento evento = pendientes.SelectedItem as Evento;
            if (evento == null) return;
            try
            {
                GestorEventos.Instance.RechazarPrimerEvento(evento.Id, motivo.Text);
                Cargar();
            }
            catch (Exception ex) when (ex is ReglaNegocioException || ex is InvalidOperationException)
            {
                Avisos.Informativo(this, ex.Message);
                Cargar();
            }
        }
    }
}
