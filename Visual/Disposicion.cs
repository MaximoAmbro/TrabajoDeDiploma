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
            var lateral = new Panel { Name = "navegacionLateral", AutoScroll = true, BackColor = Tema.LateralFondo };
            foreach (Control elemento in elementos)
            {
                elemento.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                elemento.Margin = Padding.Empty;
                lateral.Controls.Add(elemento);
                if (elemento is Componentes.BotonTematico boton)
                {
                    boton.GlifoLateral = GlifoDeOpcion(boton.Name);
                    boton.MarcarOpcionActual(false);
                }
            }
            ventana.Controls.Add(lateral);
            return lateral;
        }

        private static string GlifoDeOpcion(string nombre)
        {
            switch (nombre)
            {
                case "btnInicio": return "\uE80F";
                case "btnEventos":
                case "btnEventosEnRevision": return "\uE787";
                case "btnHistorial": return "\uE8A5";
                case "btnSuscripcion": return "\uE734";
                case "btnMisLocales": return "\uE707";
                case "btnRegistrarLocal": return "\uE710";
                case "btnControlIngreso": return "\uE8B3";
                case "btnListasEspera": return "\uE823";
                case "btnReportes":
                case "btnReportesGlobales": return "\uE9D9";
                case "btnAuditoria": return "\uE81C";
                case "btnCambiarContrasena": return "\uE72E";
                case "btnSolicitudOrganizador":
                case "btnSolicitudesOrganizador":
                case "btnCrearAdministrador": return "\uE8FA";
                case "lblvolver":
                case "btnCerrarSesion": return "\uF3B1";
                default: return "\uE8A7";
            }
        }

        internal static void DistribuirMenu(Form ventana, Panel lateral, Control contenido, Label titulo)
        {
            int Px(int medida) => Pixeles(ventana, medida);
            int margen = Px(Tema.Espacio.XXL);
            int arriba = Px(Tema.AltoBarraSuperior);
            int anchoLateral = Px(AnchoLateral);
            lateral.SetBounds(0, arriba, anchoLateral, Math.Max(0, ventana.ClientSize.Height - arriba));

            int y = Px(Tema.Espacio.S);
            foreach (Control elemento in lateral.Controls)
            {
                if (elemento is Label && y > Px(Tema.Espacio.S)) y += Px(Tema.Espacio.S);
                int alto = elemento is Button ? Px(AltoOpcionLateral) : Px(Tema.Espacio.XL);
                if (elemento is Button boton)
                {
                    boton.TextAlign = ContentAlignment.MiddleLeft;
                    boton.Padding = new Padding(Px(Tema.Espacio.XXL + Tema.Espacio.S), 0, Px(Tema.Espacio.S), 0);
                }
                if (elemento is Label etiqueta)
                {
                    etiqueta.BackColor = Tema.LateralFondo;
                    etiqueta.ForeColor = Tema.LateralSeccion;
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
            int inicio = titulo.Bottom + Px(Tema.Espacio.XL);
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

        internal static void AlinearAccionesFormulario(Form ventana, Control campo, Button principal, Button volver, bool apilar = false)
        {
            int alto = Pixeles(ventana, Tema.AltoBoton);
            int separacion = Pixeles(ventana, Tema.Espacio.L);
            int margen = Pixeles(ventana, Tema.Espacio.XXL);
            int arriba = Math.Max(principal.Top, volver.Top);
            principal.SetBounds(campo.Left, arriba, campo.Width, alto);
            volver.SetBounds(apilar ? campo.Left : principal.Right + separacion,
                apilar ? principal.Bottom + separacion : arriba, campo.Width, alto);
            ventana.ClientSize = new Size(Math.Max(ventana.ClientSize.Width, volver.Right + margen),
                Math.Max(ventana.ClientSize.Height, volver.Bottom + margen));
        }

        internal static void ExtenderGrilla(DataGridView grilla, Control pie)
        {
            int margen = Pixeles(grilla, Tema.Espacio.XXL);
            int separacion = Pixeles(grilla, Tema.Espacio.L);
            grilla.Left = margen;
            grilla.Width = Math.Max(1, grilla.Parent.ClientSize.Width - 2 * margen);
            grilla.Height = Math.Max(1, pie.Top - grilla.Top - separacion);
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
            if (form.MinimumSize.Width > area.Width || form.MinimumSize.Height > area.Height)
                form.MinimumSize = new Size(Math.Min(form.MinimumSize.Width, area.Width),
                    Math.Min(form.MinimumSize.Height, area.Height));
            form.Size = new Size(Math.Min(form.Width, area.Width), Math.Min(form.Height, area.Height));
            form.Location = new Point(area.Left + (area.Width - form.Width) / 2, area.Top + (area.Height - form.Height) / 2);
        }

        internal static void ActivarTarjeta(Control tarjeta, Button accion, Label etiquetaConFondo = null)
        {
            void Pintar(Color fondo)
            {
                tarjeta.BackColor = fondo;
                foreach (Label etiqueta in tarjeta.Controls.OfType<Label>())
                    if (etiqueta != etiquetaConFondo) etiqueta.BackColor = fondo;
            }

            foreach (Control superficie in tarjeta.Controls.Cast<Control>().Prepend(tarjeta))
            {
                superficie.Cursor = Cursors.Hand;
                superficie.MouseEnter += (s, e) => Pintar(Tema.PrimarioSuave);
                superficie.MouseLeave += (s, e) =>
                {
                    if (!tarjeta.ClientRectangle.Contains(tarjeta.PointToClient(Control.MousePosition)))
                        Pintar(Tema.Superficie);
                };
                superficie.MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Left) Pintar(Tema.SecundarioPresionado);
                };
                superficie.MouseUp += (s, e) => Pintar(tarjeta.ClientRectangle.Contains(tarjeta.PointToClient(Control.MousePosition))
                    ? Tema.PrimarioSuave : Tema.Superficie);
                if (superficie != accion)
                    superficie.Click += (s, e) => accion.PerformClick();
            }
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
