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
                Color color = i + 1 == PasoActual ? Tema.Primario : i + 1 < PasoActual ? Tema.Exito : Tema.TextoSecundario;
                var rectangulo = new Rectangle(x, 0, derecha - x, ClientSize.Height);
                TextRenderer.DrawText(e.Graphics, (i + 1) + " " + nombres[i], Font, rectangulo, color,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                if (i + 1 == PasoActual)
                    using (var lapiz = new Pen(Tema.Primario, Tema.BordeFoco))
                        e.Graphics.DrawLine(lapiz, x, ClientSize.Height - Tema.BordeFoco, derecha, ClientSize.Height - Tema.BordeFoco);
            }
        }
    }
    public class EtiquetaTotal : EtiquetaTitulo
    {
        public EtiquetaTotal() { ForeColor = Tema.Primario; }
    }
}
