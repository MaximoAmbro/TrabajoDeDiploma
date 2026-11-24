using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Visual
{
    public static class Disposicion
    {
        public const int AnchoLateral = 240;
        public const int AltoOpcionLateral = 40;

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr handle);

        public static int Dpi(Control control)
        {
            var ventana = control.FindForm();
            if (ventana != null && ventana.IsHandleCreated)
            {
                int dpi = (int)GetDpiForWindow(ventana.Handle);
                if (dpi > 0) return dpi;
            }
            return control.DeviceDpi;
        }

        public static int Pixeles(Control control, int medida)
        {
            return (int)Math.Round(medida * Dpi(control) / Tema.DpiDiseno, MidpointRounding.AwayFromZero);
        }

        internal static Panel CrearLateral(Form ventana, params Control[] elementos)
        {
            var lateral = new Panel { Name = "navegacionLateral", AutoScroll = true, BackColor = Tema.Fondo };
            foreach (Control elemento in elementos)
            {
                elemento.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                elemento.Margin = Padding.Empty;
                lateral.Controls.Add(elemento);
                if (elemento is Componentes.BotonTematico boton) boton.MarcarOpcionActual(false);
            }
            ventana.Controls.Add(lateral);
            return lateral;
        }

        internal static void DistribuirMenu(Form ventana, Panel lateral, Control contenido, Label titulo)
        {
            int Px(int medida) => Pixeles(ventana, medida);
            int margen = Px(Tema.Espacio.XL);
            int arriba = Px(Tema.AltoBarraSuperior);
            int anchoLateral = Px(AnchoLateral);
            lateral.SetBounds(0, arriba, anchoLateral, Math.Max(0, ventana.ClientSize.Height - arriba));

            int y = Px(Tema.Espacio.S);
            foreach (Control elemento in lateral.Controls)
            {
                if (elemento is Label && y > Px(Tema.Espacio.S)) y += Px(Tema.Espacio.S);
                int alto = elemento is Button ? Px(AltoOpcionLateral) : Px(Tema.Espacio.XL);
                if (elemento is Button boton)
                    boton.TextAlign = ContentAlignment.MiddleLeft;
                if (elemento is Label etiqueta)
                {
                    etiqueta.BackColor = Tema.Fondo;
                    etiqueta.Padding = new Padding(Px(Tema.Espacio.L), 0, 0, 0);
                    etiqueta.TextAlign = ContentAlignment.MiddleLeft;
                }
                elemento.SetBounds(0, y + lateral.AutoScrollPosition.Y, anchoLateral, alto);
                y += alto;
            }

            int izquierda = anchoLateral + margen;
            int ancho = Math.Max(1, ventana.ClientSize.Width - izquierda - margen);
            titulo.AutoSize = false;
            titulo.TextAlign = ContentAlignment.MiddleLeft;
            titulo.SetBounds(izquierda, arriba + margen, ancho, Px(Tema.Espacio.XXL));
            int inicio = titulo.Bottom + Px(Tema.Espacio.L);
            contenido.SetBounds(izquierda, inicio, ancho, Math.Max(1, ventana.ClientSize.Height - inicio - margen));
        }

        public static float Escala(Control control)
        {
            return Dpi(control) / Tema.DpiDiseno * control.Font.SizeInPoints / Tema.Cuerpo.SizeInPoints;
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

        internal static void AlinearGrupo(params Button[] botones)
        {
            int ancho = botones.Max(b => b.Width);
            int x = botones[0].Left;
            int y = botones[0].Top;
            int separacion = Pixeles(botones[0], Tema.Espacio.S);
            foreach (Button boton in botones)
            {
                boton.SetBounds(x, y, ancho, Pixeles(boton, Tema.AltoBoton));
                x += ancho + separacion;
            }
        }

        internal static void LimitarAncho(Label rotulo)
        {
            int ancho = rotulo.Parent.ClientSize.Width - rotulo.Left - Pixeles(rotulo, Tema.Espacio.XL);
            if (rotulo.Width <= ancho) return;
            rotulo.AutoSize = false;
            rotulo.Width = Math.Max(1, ancho);
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
            var panel = (Control)sender;
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
