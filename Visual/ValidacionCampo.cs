using System;
using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    /// <summary>Inline validation message attached to the input that needs attention.</summary>
    public static class ValidacionCampo
    {
        private const string Prefijo = "errorInline_";

        public static void Mostrar(Control campo, string mensaje)
        {
            if (campo == null || campo.IsDisposed || campo.Parent == null) return;
            Limpiar(campo);
            campo.BackColor = Tema.ErrorFondo;
            var aviso = new Label
            {
                Name = Prefijo + campo.Name,
                Text = mensaje,
                AutoSize = false,
                Location = new Point(campo.Left, campo.Bottom + 1),
                Size = new Size(campo.Width, Math.Max(18, campo.Font.Height + 2)),
                Font = Tema.Ayuda,
                ForeColor = Tema.Error,
                BackColor = campo.Parent.BackColor,
                AutoEllipsis = true,
                AccessibleRole = AccessibleRole.Alert
            };
            campo.Parent.Controls.Add(aviso);
            aviso.BringToFront();
        }

        public static void Limpiar(Control campo)
        {
            if (campo == null || campo.IsDisposed) return;
            campo.BackColor = Tema.Superficie;
            if (campo.Parent == null) return;
            Control anterior = campo.Parent.Controls[Prefijo + campo.Name];
            if (anterior != null) anterior.Dispose();
        }
    }
}
