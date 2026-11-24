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
                bool cumplido = i + 1 < PasoActual;
                Color color = i + 1 == PasoActual ? Tema.Texto : Tema.TextoSecundario;
                var rectangulo = new Rectangle(x, 0, derecha - x, ClientSize.Height);
                TextRenderer.DrawText(e.Graphics, (i + 1) + " " + nombres[i] + (cumplido ? "  ✓" : string.Empty), Font, rectangulo, color,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                if (i + 1 == PasoActual)
                    using (var lapiz = new Pen(Tema.Texto, Tema.BordeFoco))
                        e.Graphics.DrawLine(lapiz, x, ClientSize.Height - Tema.BordeFoco, derecha, ClientSize.Height - Tema.BordeFoco);
            }
        }
    }
    public sealed class ResumenPaquete : Label
    {
        private const string AyudaDescuento = "El descuento se aplica sobre el valor de las entradas.";
        private string importeTotal = string.Empty;
        private System.Collections.Generic.IReadOnlyList<(string Concepto, string Importe, bool Seccion)> filas =
            Array.Empty<(string, string, bool)>();

        public ResumenPaquete()
        {
            Font = Tema.Cuerpo;
            ForeColor = Tema.Texto;
            UseMnemonic = false;
        }

        internal void Cargar(System.Collections.Generic.IReadOnlyList<(string Concepto, string Importe, bool Seccion)> detalle,
            string total)
        {
            filas = detalle;
            importeTotal = total;
            var texto = new System.Text.StringBuilder();
            foreach (var fila in filas)
                texto.AppendLine(fila.Importe == null ? fila.Concepto : fila.Concepto + " " + fila.Importe);
            texto.AppendLine("TOTAL " + total);
            texto.Append(AyudaDescuento);
            Text = texto.ToString();
            Invalidate();
        }

        internal object ModeloWeb()
        {
            var lineas = new System.Collections.Generic.List<object>(filas.Count);
            foreach (var fila in filas)
                lineas.Add(new { concepto = fila.Concepto, importe = fila.Importe, seccion = fila.Seccion });
            return new { filas = lineas, total = importeTotal, ayuda = AyudaDescuento };
        }

        internal int AltoPara(int ancho)
        {
            using (Graphics lienzo = CreateGraphics()) return Distribuir(lienzo, ancho, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Distribuir(e.Graphics, ClientSize.Width, true);
        }

        private int Distribuir(Graphics lienzo, int ancho, bool pintar)
        {
            const TextFormatFlags formato = TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak;
            int margen = Disposicion.Pixeles(this, Tema.Espacio.XS);
            int separacion = Disposicion.Pixeles(this, Tema.Espacio.S);
            int anchoImporte = 0;
            foreach (var fila in filas)
                if (fila.Importe != null)
                    anchoImporte = Math.Max(anchoImporte, TextRenderer.MeasureText(lienzo, fila.Importe, Font,
                        Size.Empty, TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine).Width);
            int arriba = margen;
            bool EnVista(int y, int alto) =>
                Parent == null || (y + Top >= 0 && y + Top + alto <= Parent.ClientSize.Height);
            foreach (var fila in filas)
            {
                Font fuente = fila.Seccion ? Tema.Seccion : Font;
                Color color = fila.Seccion ? Tema.TextoSecundario : ForeColor;
                if (fila.Seccion && arriba > margen) arriba += separacion;
                string concepto = fila.Seccion ? fila.Concepto.ToUpperInvariant() : fila.Concepto;
                int anchoFila = Math.Max(1, ancho - 2 * margen - (fila.Importe == null ? 0 : anchoImporte + separacion));
                int alto = TextRenderer.MeasureText(lienzo, concepto, fuente, new Size(anchoFila, int.MaxValue), formato).Height;
                if (pintar && EnVista(arriba, alto))
                {
                    TextRenderer.DrawText(lienzo, concepto, fuente, new Rectangle(margen, arriba, anchoFila, alto), color, formato);
                    if (fila.Importe != null)
                        TextRenderer.DrawText(lienzo, fila.Importe, Font,
                            new Rectangle(ancho - margen - anchoImporte, arriba, anchoImporte, alto), ForeColor,
                            TextFormatFlags.Right | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                }
                arriba += alto;
            }
            const string ayuda = AyudaDescuento;
            int anchoAyuda = Math.Max(1, ancho - 2 * margen);
            int altoAyuda = TextRenderer.MeasureText(lienzo, ayuda, Tema.Ayuda, new Size(anchoAyuda, int.MaxValue), formato).Height;
            if (pintar && EnVista(arriba + separacion, altoAyuda))
                TextRenderer.DrawText(lienzo, ayuda, Tema.Ayuda,
                    new Rectangle(margen, arriba + separacion, anchoAyuda, altoAyuda), Tema.TextoSecundario, formato);
            return arriba + separacion + altoAyuda + margen;
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
