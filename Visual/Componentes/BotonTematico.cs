using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Visual.Componentes
{
    public enum RolBoton { Primario, Secundario, Peligro }

    public abstract class BotonTematico : Button
    {
        private readonly RolBoton rol;
        private RolBoton? rolVisual;
        private bool opcionActual;

        internal void DefinirRolVisual(bool principal)
        {
            rolVisual = principal ? RolBoton.Primario : rol == RolBoton.Peligro ? RolBoton.Peligro : RolBoton.Secundario;
            Tema.EstadoBoton(this, rolVisual.Value, opcionActual);
        }

        internal void MarcarOpcionActual(bool actual)
        {
            opcionActual = actual;
            DefinirRolVisual(actual);
        }

        protected BotonTematico(RolBoton rol)
        {
            this.rol = rol;
            Tema.PrepararBoton(this, rol);
        }

        // El estilo pertenece al tema, también cuando el diseñador guarda el componente.
        protected bool ShouldSerializeFont() { return false; }
        protected bool ShouldSerializeBackColor() { return false; }
        protected bool ShouldSerializeForeColor() { return false; }
        protected bool ShouldSerializeFlatStyle() { return false; }
        protected bool ShouldSerializeFlatAppearance() { return false; }
        protected bool ShouldSerializePadding() { return false; }
        protected bool ShouldSerializeCursor() { return false; }
        protected bool ShouldSerializeUseVisualStyleBackColor() { return false; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font { get { return base.Font; } set { base.Font = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding { get { return base.Padding; } set { base.Padding = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Cursor Cursor { get { return base.Cursor; } set { base.Cursor = value; } }

        [DefaultValue(FlatStyle.Flat)]
        public new FlatStyle FlatStyle { get { return base.FlatStyle; } set { base.FlatStyle = value; } }

        [DefaultValue(false)]
        public new bool UseVisualStyleBackColor { get { return base.UseVisualStyleBackColor; } set { base.UseVisualStyleBackColor = value; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FlatButtonAppearance FlatAppearance { get { return base.FlatAppearance; } }

        protected override bool ShowFocusCues => false;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!Focused || (rolVisual ?? rol) != RolBoton.Primario) return;

            // La separación clara mantiene visible el borde púrpura sobre el relleno púrpura.
            int borde = FlatAppearance.BorderSize;
            Rectangle interior = Rectangle.Inflate(ClientRectangle, -borde, -borde);
            if (interior.Width <= 0 || interior.Height <= 0) return;
            using (var separador = new Pen(Tema.Superficie))
                e.Graphics.DrawRectangle(separador, interior.X, interior.Y, interior.Width - 1, interior.Height - 1);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Tema.EstadoBoton(this, rolVisual ?? rol, opcionActual);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Tema.EstadoBoton(this, rolVisual ?? rol, opcionActual);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Tema.EstadoBoton(this, rolVisual ?? rol, opcionActual);
        }
    }
}
