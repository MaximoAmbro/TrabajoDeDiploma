using System;
using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    public static class Disposicion
    {
        public static float Escala(Control control)
        {
            return control.DeviceDpi / Tema.DpiDiseno * control.Font.SizeInPoints / Tema.Cuerpo.SizeInPoints;
        }

        // Una barra vertical puede cambiar el ancho: admite una sola repetición, nunca recursión.
        public static void RepetirUnaVez(Control contenido, Action distribuir)
        {
            for (int pasada = 0; pasada < 2; pasada++)
            {
                int ancho = contenido.ClientSize.Width;
                distribuir();
                if (ancho == contenido.ClientSize.Width) break;
            }
        }

        public static void AjustarDocumento(Panel tarjeta, Label etiqueta, int altoDisponible)
        {
            int relleno = (int)Math.Round(Tema.Espacio.L * Escala(tarjeta.FindForm() ?? (Control)tarjeta));
            int ancho = Math.Max(0, tarjeta.ClientSize.Width - relleno * 2 - SystemInformation.VerticalScrollBarWidth);
            etiqueta.MinimumSize = new Size(ancho, 0);
            etiqueta.MaximumSize = new Size(ancho, 0);
            int alto = etiqueta.GetPreferredSize(new Size(ancho, 0)).Height;
            int natural = etiqueta.Top - tarjeta.AutoScrollPosition.Y + alto + relleno;
            tarjeta.Height = Math.Min(natural, Math.Max(0, altoDisponible));
        }

        public static void AlinearAcciones(Control pie, Control secundario, Control principal)
        {
            secundario.Location = new Point(pie.Padding.Left, (pie.Height - secundario.Height) / 2);
            principal.Location = new Point(pie.ClientSize.Width - pie.Padding.Right - principal.Width,
                (pie.Height - principal.Height) / 2);
        }

        public static void AjustarAreaVisible(Form form)
        {
            AjustarAreaVisible(form, Screen.FromControl(form).WorkingArea);
        }

        public static void AjustarAreaVisible(Form form, Rectangle area)
        {
            form.Size = new Size(Math.Min(form.Width, area.Width), Math.Min(form.Height, area.Height));
            form.Location = new Point(area.Left + (area.Width - form.Width) / 2, area.Top + (area.Height - form.Height) / 2);
        }

        public static void PintarTarjeta(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var lapiz = new Pen(Tema.Borde, Tema.BordeNormal))
                e.Graphics.DrawRectangle(lapiz, 0, 0, panel.ClientSize.Width - Tema.BordeNormal,
                    panel.ClientSize.Height - Tema.BordeNormal);
        }

        public static void PintarSeparadorInferior(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var lapiz = new Pen(Tema.Borde, Tema.BordeNormal))
                e.Graphics.DrawLine(lapiz, 0, panel.ClientSize.Height - Tema.BordeNormal,
                    panel.ClientSize.Width, panel.ClientSize.Height - Tema.BordeNormal);
        }

        public static void PintarSeparadorSuperior(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using (var lapiz = new Pen(Tema.Borde, Tema.BordeNormal))
                e.Graphics.DrawLine(lapiz, 0, 0, panel.ClientSize.Width, 0);
        }
    }
}
