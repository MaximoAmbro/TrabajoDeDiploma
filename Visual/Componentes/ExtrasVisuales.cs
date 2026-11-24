using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace Visual.Componentes
{
    // Sin geometría ni estilo serializable; el Designer decide sus medidas.
    public class IndicadorPasos : EtiquetaAyuda
    {
        private int pasoActual = 1;
        [DefaultValue(1)]
        public int PasoActual
        {
            get { return pasoActual; }
            set { if (value < 1 || value > 4) throw new ArgumentOutOfRangeException(nameof(value)); pasoActual = value; Invalidate(); }
        }
        public IndicadorPasos() { SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true); }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Tema.Superficie);
            string[] nombres = { "Elegí", "Armá", "Pagá", "Listo" };
            for (int i = 0; i < nombres.Length; i++)
            {
                int x = ClientSize.Width * i / 4, derecha = ClientSize.Width * (i + 1) / 4;
                Color color = i + 1 == PasoActual ? Tema.Texto : i + 1 < PasoActual ? Tema.Exito : Tema.TextoSecundario;
                var rectangulo = new Rectangle(x, 0, derecha - x, ClientSize.Height);
                TextRenderer.DrawText(e.Graphics, (i + 1) + " " + nombres[i], Font, rectangulo, color,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                if (i + 1 == PasoActual)
                    using (var lapiz = new Pen(Tema.Texto, Tema.BordeFoco))
                        e.Graphics.DrawLine(lapiz, x, ClientSize.Height - Tema.BordeFoco, derecha, ClientSize.Height - Tema.BordeFoco);
            }
        }
    }
    public class EtiquetaTotal : EtiquetaTitulo
    {
        public EtiquetaTotal() { Font = Tema.PrecioTarjeta; ForeColor = Tema.Texto; }
    }
    public class IconoExito : EtiquetaAyuda
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Tema.Superficie);
            TextRenderer.DrawText(e.Graphics, "\uE73E", Tema.IconosGrandes, ClientRectangle, Tema.Exito,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    public static class IconosDocumentos
    {
        public static void Preparar(ListBox lista)
        {
            lista.DrawItem += (sender, e) =>
            {
                if (e.Index < 0) return;
                bool elegido = (e.State & DrawItemState.Selected) != 0;
                using (var fondo = new SolidBrush(elegido ? Tema.EncabezadoGrilla : Tema.Superficie)) e.Graphics.FillRectangle(fondo, e.Bounds);
                int icono = e.Bounds.Height;
                TextRenderer.DrawText(e.Graphics, "\uE8A5", Tema.IconosDocumentos,
                    new Rectangle(e.Bounds.Left, e.Bounds.Top, icono, icono), Tema.Texto,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                TextRenderer.DrawText(e.Graphics, Convert.ToString(lista.Items[e.Index]), Tema.Cuerpo,
                    new Rectangle(e.Bounds.Left + icono, e.Bounds.Top, Math.Max(0, e.Bounds.Width - icono), e.Bounds.Height),
                    Tema.Texto, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                e.DrawFocusRectangle();
            };
        }
    }
}
