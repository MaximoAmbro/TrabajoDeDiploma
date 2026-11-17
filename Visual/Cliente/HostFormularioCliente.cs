using System;
using System.Linq;
using System.Windows.Forms;

namespace Visual.Componentes
{
    /// <summary>Adapta una pantalla WinForms existente a una página de navegación del cliente.</summary>
    public sealed class HostFormularioCliente : UserControl
    {
        public Form Pantalla { get; }

        public HostFormularioCliente(Form pantalla)
        {
            Pantalla = pantalla ?? throw new ArgumentNullException(nameof(pantalla));
            Name = "pagina" + pantalla.Name;
            Dock = DockStyle.Fill;
            pantalla.TopLevel = false;
            pantalla.FormBorderStyle = FormBorderStyle.None;
            pantalla.ShowInTaskbar = false;
            Control barra = pantalla.Controls.Find("barraSuperior", true).FirstOrDefault();
            if (barra != null) barra.Visible = false;
            pantalla.Dock = DockStyle.Fill;
            Controls.Add(pantalla);
        }

        public void Activar()
        {
            if (Pantalla.IsDisposed) return;
            Pantalla.Show();
            Pantalla.Dock = DockStyle.Fill;
            Pantalla.Bounds = ClientRectangle;
            Pantalla.BringToFront();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Pantalla != null && !Pantalla.IsDisposed)
            {
                Pantalla.Dock = DockStyle.Fill;
                Pantalla.Bounds = ClientRectangle;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Pantalla != null && !Pantalla.IsDisposed) Pantalla.Dispose();
            base.Dispose(disposing);
        }
    }
}
