using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Visual.Componentes
{
    public abstract class EtiquetaTematica : Label
    {
        protected EtiquetaTematica(System.Drawing.Font fuente, System.Drawing.Color color)
        {
            Font = fuente;
            ForeColor = color;
            BackColor = Tema.Superficie;
        }

        protected bool ShouldSerializeFont() { return false; }
        protected bool ShouldSerializeBackColor() { return false; }
        protected bool ShouldSerializeForeColor() { return false; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font { get { return base.Font; } set { base.Font = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
    }

    public class EtiquetaTitulo : EtiquetaTematica
    {
        public EtiquetaTitulo() : base(Tema.Titulo, Tema.Texto) { }
    }
    public class EtiquetaSeccion : EtiquetaTematica
    {
        public EtiquetaSeccion() : base(Tema.Seccion, Tema.Texto) { }
    }
    public class EtiquetaRotulo : EtiquetaTematica
    {
        public EtiquetaRotulo() : base(Tema.Rotulo, Tema.Texto) { }
    }
    public sealed class EtiquetaEncabezado : EtiquetaRotulo
    {
        public EtiquetaEncabezado()
        {
            Font = Tema.Seccion;
            ForeColor = Tema.TextoSecundario;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            string texto = Text.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
            float x = Padding.Left;
            float y = (Height - Font.GetHeight(e.Graphics)) / 2;
            float espacio = Font.SizeInPoints * e.Graphics.DpiX / 72f * Tema.InterletradoSeccion;
            using (var pincel = new SolidBrush(ForeColor))
            using (var formato = (StringFormat)StringFormat.GenericTypographic.Clone())
            {
                formato.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                foreach (char letra in texto)
                {
                    string glifo = letra.ToString();
                    e.Graphics.DrawString(glifo, Font, pincel, x, y, formato);
                    x += e.Graphics.MeasureString(glifo, Font, PointF.Empty, formato).Width + espacio;
                }
            }
        }
    }

    public class EtiquetaAyuda : EtiquetaTematica
    {
        public EtiquetaAyuda() : base(Tema.Ayuda, Tema.TextoSecundario) { }
    }
}
