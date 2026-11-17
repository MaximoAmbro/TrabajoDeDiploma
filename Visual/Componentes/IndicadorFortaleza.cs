using System.Drawing;
using System.Windows.Forms;

namespace Visual.Componentes
{
    public sealed class IndicadorFortaleza : SuperficieTematica
    {
        public int Puntaje { get; private set; }
        public int Minimo { get; private set; }

        public void Actualizar(string contrasena, int puntaje, int minimo)
        {
            Puntaje = puntaje;
            Minimo = minimo;
            AccessibleName = "Fortaleza de contraseña";
            AccessibleDescription = "Puntaje " + puntaje + " de 5. Mínimo requerido: " + minimo + ".";
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            string nivel = Puntaje < Minimo ? "Insuficiente" : Puntaje < 5 ? "Aceptable" : "Fuerte";
            TextRenderer.DrawText(e.Graphics, "Fortaleza: " + nivel + " (" + Puntaje + "/5; mínimo " + Minimo + ")",
                Tema.Ayuda, new Rectangle(0, 0, Width, System.Math.Max(16, Height - 8)), Tema.Texto,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            int separacion = 4;
            int ancho = (Width - (separacion * 4)) / 5;
            for (int i = 0; i < 5; i++)
            {
                var rect = new Rectangle(i * (ancho + separacion), Height - 6, ancho, 5);
                using (var brocha = new SolidBrush(i < Puntaje ? (Puntaje < Minimo ? Tema.Error : Tema.Exito) : Tema.Borde))
                    e.Graphics.FillRectangle(brocha, rect);
            }
        }
    }
}
