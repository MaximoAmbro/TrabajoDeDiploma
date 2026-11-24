using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Negocio;

namespace Visual.Componentes
{
    public partial class BarraSuperior : UserControl
    {
        private string persona = string.Empty;
        private string beneficio = string.Empty;
        public BarraSuperior()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Font = Tema.Ayuda; BackColor = Tema.Primario; ForeColor = Tema.Superficie;
            TabStop = false;
            AccessibleName = "EventAura";
        }
        internal bool MostrarSesion { get; set; } = true;
        public void ActualizarSesion()
        {
            var usuario = GestorSesion.Instance.UsuarioActual;
            persona = beneficio = string.Empty;
            AccessibleDescription = null;
            if (!MostrarSesion || usuario == null || !GestorUsuarios.Instance.ExisteUsuarioDeSesion()) { Invalidate(); return; }
            else
            {
                persona = usuario.Nombre + " " + usuario.Apellido;
                if (usuario.TipoUsuario == "Administrador") beneficio = "Administrador de plataforma";
                else if (usuario.TipoUsuario == "Organizador") beneficio = "Organizador";
                else
                {
                    var cliente = GestorFidelizacion.Instance.ClienteActualParaEncabezado();
                    if (cliente == null) { persona = string.Empty; Invalidate(); return; }
                    beneficio = cliente.Nivel + " · " + GestorFidelizacion.Instance.PorcentajePara(cliente.Nivel)
                        .ToString("P1", System.Globalization.CultureInfo.GetCultureInfo("es-AR")) + " de descuento";
                }
            }
            AccessibleDescription = persona + ". " + beneficio;
            Invalidate();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime) ActualizarSesion();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Tema.Primario);
            float escala = e.Graphics.DpiX / 96f;
            int lado = (int)(48 * escala), margen = (int)(8 * escala);
            MarcaAura.Dibujar(e.Graphics, new Rectangle(margen,(Height-lado)/2,lado,lado), Tema.Superficie);
            var firma = new Point(margen+lado, Math.Max(0,(Height-Tema.FirmaNormal.Height)/2));
            TextRenderer.DrawText(e.Graphics,"Event",Tema.FirmaNormal,firma,Tema.Superficie,TextFormatFlags.NoPadding);
            int eventAncho = TextRenderer.MeasureText(e.Graphics,"Event",Tema.FirmaNormal,Size.Empty,TextFormatFlags.NoPadding).Width;
            TextRenderer.DrawText(e.Graphics,"Aura",Tema.FirmaFuerte,new Point(firma.X+eventAncho,firma.Y),Tema.Superficie,TextFormatFlags.NoPadding);
            int inicio = firma.X+eventAncho+TextRenderer.MeasureText(e.Graphics,"Aura",Tema.FirmaFuerte,Size.Empty,TextFormatFlags.NoPadding).Width+margen;
            var identidad = new Rectangle(inicio,margen,Math.Max(0,Width-inicio-margen),Math.Max(0,Height-margen*2));
            TextRenderer.DrawText(e.Graphics,persona+Environment.NewLine+beneficio,Tema.Ayuda,identidad,Tema.Superficie,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
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
}
