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
            this.ClientSize = new System.Drawing.Size(1000, 650);
            barra = new Componentes.BarraSuperior { Name = "barraSuperior", Dock = DockStyle.Top, Height = 60 };
            titulo = new Componentes.EtiquetaTitulo { Name = "tituloAdministrador", Text = "Panel de plataforma", Location = new Point(24, 84), Size = new Size(370, 40) };
            this.btnReportesGlobales = new Componentes.BotonPrimario();
            this.btnReportesGlobales.Name = "btnReportesGlobales";
            this.btnReportesGlobales.Text = "Reportes globales";
            this.btnReportesGlobales.Location = new Point(24, 132);
            this.btnReportesGlobales.Size = new System.Drawing.Size(220, 40);
            this.btnCerrarSesion = new Componentes.BotonSecundario();
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.Location = new Point(260, 132);
            this.btnCerrarSesion.Size = new System.Drawing.Size(180, 40);
            panel = new Componentes.PanelAdministrador { Name = "panelAdministrador", Location = new Point(24, 184), Size = new Size(952, 442), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            Controls.AddRange(new Control[] { barra, titulo, btnReportesGlobales, btnCerrarSesion, panel });
            ResumeLayout(false);
        }
    }
}
