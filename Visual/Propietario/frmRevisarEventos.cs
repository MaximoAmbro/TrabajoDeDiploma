using System;
using System.Drawing;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public sealed class frmRevisarEventos : FormularioEventAura
    {
        private readonly ListBox pendientes = new ListBox { Name = "lstEventosEnRevision", Location = new Point(24, 88), Size = new Size(620, 400), DisplayMember = "Nombre" };
        private readonly Label detalle = new Componentes.EtiquetaAyuda { Name = "lblDetalleEventoRevision", Location = new Point(670, 88), Size = new Size(360, 120) };
        private readonly Label etiquetaMotivo = new Componentes.EtiquetaAyuda { Name = "lblMotivoRechazoEvento", Text = "Motivo del rechazo (obligatorio)", Location = new Point(670, 210), Size = new Size(350, 20) };
        private readonly TextBox motivo = new TextBox { Name = "txtMotivoRechazoEvento", Location = new Point(670, 232), Size = new Size(350, 60), Multiline = true, MaxLength = 500, ScrollBars = ScrollBars.Vertical };
        private readonly Button aprobar = new Componentes.BotonPrimario { Name = "btnAprobarPrimerEvento", Text = "Aprobar y publicar", Location = new Point(670, 310), Size = new Size(300, 44) };
        private readonly Button rechazar = new Componentes.BotonPeligro { Name = "btnRechazarPrimerEvento", Text = "Rechazar y devolver", Location = new Point(670, 366), Size = new Size(300, 44) };
        private readonly Button cerrar = new Componentes.BotonSecundario { Text = "Cerrar", Location = new Point(24, 510), Size = new Size(140, 40) };

        public frmRevisarEventos()
        {
            GestorSesion.Instance.RequerirAdministrador();
            Text = "Eventos en revisión";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1060, 570);
            Controls.Add(new Componentes.EtiquetaTitulo { Text = "Primer evento de cada organizador", Location = new Point(24, 24), Size = new Size(700, 44) });
            Controls.AddRange(new Control[] { pendientes, detalle, etiquetaMotivo, motivo, aprobar, rechazar, cerrar });
            pendientes.SelectedIndexChanged += (s, e) => MostrarSeleccion();
            aprobar.Click += (s, e) => Aprobar();
            rechazar.Click += (s, e) => Rechazar();
            cerrar.Click += (s, e) => Close();
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            pendientes.DataSource = null;
            pendientes.DataSource = GestorEventos.Instance.PendientesDeRevision();
            MostrarSeleccion();
        }

        private void MostrarSeleccion()
        {
            Evento evento = pendientes.SelectedItem as Evento;
            detalle.Text = evento == null ? "No hay eventos pendientes." :
                evento.Nombre + Environment.NewLine + evento.Fecha.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + "Al aprobarlo, aparece en el catálogo. Si se rechaza, vuelve a Borrador con el motivo.";
            aprobar.Enabled = evento != null;
            rechazar.Enabled = evento != null;
            motivo.Clear();
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
