using System;
using System.Windows.Forms;
namespace Visual.Componentes
{
    public partial class EstadoVacio : SuperficieTematica
    {
        public event Action Accion;
        public EstadoVacio(){InitializeComponent();icono.Font=Tema.IconosInterfaz;icono.ForeColor=Tema.TextoSecundario;boton.Click+=(s,e)=>Accion?.Invoke();}
        public void Cargar(string texto,string accion)
        {
            explicacion.Text = texto;
            boton.Text = accion;
            Distribuir();
        }
        public void AjustarA(Control area)
        {
            AjustarA(area.Bounds);
            Anchor=area.Anchor;
        }
        public void AjustarA(System.Drawing.Rectangle area)
        {
            Location = area.Location;
            Size = area.Size;
            Distribuir();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            if (explicacion != null && e.AffectedControl == this) Distribuir();
        }

        private void Distribuir()
        {
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int espacio = Px(Tema.Espacio.L);
            int alto = Math.Max(Px(140), Height);
            icono.SetBounds(espacio, Math.Max(espacio, (alto - Px(64)) / 2), Px(64), Px(64));
            int izquierda = icono.Right + espacio;
            int ancho = Math.Max(1, Width - izquierda - espacio);
            int altoTexto = explicacion.GetPreferredSize(new System.Drawing.Size(ancho, 0)).Height;
            int altoBloque = altoTexto + Px(Tema.Espacio.S + Tema.AltoBoton);
            int arriba = Math.Max(0, (alto - altoBloque) / 2);
            explicacion.SetBounds(izquierda, arriba, ancho, altoTexto);
            boton.SetBounds(izquierda, explicacion.Bottom + Px(Tema.Espacio.S),
                Math.Min(Px(Tema.AnchoBotonAmplio), Math.Max(Px(Tema.AnchoBotonCompacto), explicacion.Width)), Px(Tema.AltoBoton));
        }
    }
}
