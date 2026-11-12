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
    public class EtiquetaAyuda : EtiquetaTematica
    {
        public EtiquetaAyuda() : base(Tema.Ayuda, Tema.TextoSecundario) { }
    }
}
