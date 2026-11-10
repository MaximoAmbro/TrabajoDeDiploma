using System;
using System.Drawing;
using System.Windows.Forms;

namespace Visual.Componentes
{
    public class BotonPeligro : Button
    {
        public BotonPeligro()
        {
            FlatStyle = FlatStyle.Flat;
            UseVisualStyleBackColor = false;
            BackColor = Tema.Superficie;
            ForeColor = Tema.Error;
            Font = Tema.Boton;
            FlatAppearance.BorderSize = Tema.BordeNormal;
            FlatAppearance.BorderColor = Tema.BordeControl;
            FlatAppearance.MouseOverBackColor = Tema.ErrorFondo;
            FlatAppearance.MouseDownBackColor = Tema.PeligroPresionado;
            MinimumSize = new Size(Tema.AnchoMinimoBoton, Tema.AltoBoton);
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
            FlatAppearance.BorderSize = Tema.BordeNormal;
            FlatAppearance.BorderColor = Tema.BordeControl;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            BackColor = Enabled ? Tema.Superficie : Tema.FondoDeshabilitado;
        }
    }
}
