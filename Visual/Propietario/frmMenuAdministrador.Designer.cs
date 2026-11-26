using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    public sealed partial class frmMenuAdministrador
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "EventAura · Administración de plataforma";
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.MinimumSize = new System.Drawing.Size(1000, 680);
            barra = new Componentes.BarraSuperior { Name = "barraSuperior", Dock = DockStyle.Top, Height = 60 };
            titulo = new Componentes.EtiquetaTitulo { Name = "tituloAdministrador", Text = "Panel de plataforma", Location = new Point(24, 84), Size = new Size(370, 40) };
            this.btnReportesGlobales = new Componentes.BotonPrimario();
            this.btnReportesGlobales.Name = "btnReportesGlobales";
            this.btnReportesGlobales.Text = "Reportes globales";
            this.btnReportesGlobales.Location = new Point(24, 132);
            this.btnReportesGlobales.Size = new System.Drawing.Size(184, 40);
            this.btnCerrarSesion = new Componentes.BotonSecundario();
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.Location = new Point(408, 180);
            this.btnCerrarSesion.Size = new System.Drawing.Size(136, 40);
            this.btnSolicitudes = new Componentes.BotonSecundario();
            this.btnSolicitudes.Name = "btnSolicitudesOrganizador";
            this.btnSolicitudes.Text = "Solicitudes de organizador";
            this.btnSolicitudes.Location = new Point(216, 132);
            this.btnSolicitudes.Size = new System.Drawing.Size(240, 40);
            this.btnEventosEnRevision = new Componentes.BotonSecundario();
            this.btnEventosEnRevision.Name = "btnEventosEnRevision";
            this.btnEventosEnRevision.Text = "Eventos en revisión";
            this.btnEventosEnRevision.Location = new Point(464, 132);
            this.btnEventosEnRevision.Size = new System.Drawing.Size(184, 40);
            panel = new Componentes.PanelAdministrador { Name = "panelAdministrador", Location = new Point(24, 232), Size = new Size(952, 394), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            Controls.AddRange(new Control[] { barra, titulo, btnReportesGlobales, btnCerrarSesion, btnSolicitudes, btnEventosEnRevision, panel });
            ResumeLayout(false);
        }
    }
}
