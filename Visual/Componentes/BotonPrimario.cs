using System;
using System.Drawing;
using System.Windows.Forms;

namespace Visual.Componentes
{
    public class BotonPrimario : Button
    {
        public BotonPrimario()
        {
            FlatStyle = FlatStyle.Flat;
            UseVisualStyleBackColor = false;
            BackColor = Tema.Primario;
            ForeColor = Tema.Superficie;
            Font = Tema.Boton;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Tema.BordeControl;
            FlatAppearance.MouseOverBackColor = Tema.PrimarioEncima;
            FlatAppearance.MouseDownBackColor = Tema.PrimarioPresionado;
            MinimumSize = new Size(Tema.AnchoMinimoBoton, Tema.AltoBotonPrimario);
            Padding = new Padding(Tema.RellenoHorizontalBoton, 0, Tema.RellenoHorizontalBoton, 0);
            Cursor = Cursors.Hand;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            FlatAppearance.BorderSize = Tema.BordeFoco;
            FlatAppearance.BorderColor = Tema.Primario;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Tema.BordeControl;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            BackColor = Enabled ? Tema.Primario : Tema.FondoDeshabilitado;
        }
    }
}
