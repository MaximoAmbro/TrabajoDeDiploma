using System.Drawing;
using System.Drawing.Drawing2D;

namespace Visual.Componentes
{
    public static class MarcaAura
    {
        public static void Dibujar(Graphics g, RectangleF caja, Color color, bool travesano = true)
        {
            var estado = g.Save();
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(caja.X, caja.Y);
                g.ScaleTransform(caja.Width / 64f, caja.Height / 64f);
                using (var camino = new GraphicsPath())
                using (var lapiz = new Pen(color, 7f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                {
                    camino.AddLines(new[] { new PointF(13,51), new PointF(32,13), new PointF(51,38) });
                    g.DrawPath(lapiz, camino);
                    if (travesano)
                    {
                        lapiz.DashPattern = new[] { 0.5f, 8f };
                        lapiz.DashCap = DashCap.Round;
                        g.DrawLine(lapiz, 22.5f, 39f, 41.5f, 39f);
                    }
                }
            }
            finally { g.Restore(estado); }
        }
    }
}
