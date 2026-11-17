using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace Visual.Componentes
{
    public class SuperficieTematica : UserControl
    {
        public SuperficieTematica() { Font=Tema.Cuerpo;BackColor=Tema.Superficie;ForeColor=Tema.Texto; }
        protected bool ShouldSerializeFont(){return false;}
        protected bool ShouldSerializeBackColor(){return false;}
        protected bool ShouldSerializeForeColor(){return false;}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font {get{return base.Font;}set{base.Font=value;}}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor {get{return base.BackColor;}set{base.BackColor=value;}}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor {get{return base.ForeColor;}set{base.ForeColor=value;}}
    }
}
