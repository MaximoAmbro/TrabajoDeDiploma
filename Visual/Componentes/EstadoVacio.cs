using System;
using System.Windows.Forms;
namespace Visual.Componentes
{
    public partial class EstadoVacio : SuperficieTematica
    {
        public event Action Accion;
        public EstadoVacio(){InitializeComponent();icono.Font=Tema.IconosInterfaz;icono.ForeColor=Tema.TextoSecundario;boton.Click+=(s,e)=>Accion?.Invoke();}
        public void Cargar(string texto,string accion){explicacion.Text=texto;boton.Text=accion;}
        public void AjustarA(Control area)
        {
            AjustarA(area.Bounds);
            Anchor=area.Anchor;
        }
        public void AjustarA(System.Drawing.Rectangle area)
        {
            Location = area.Location;
            Size = area.Size;
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int espacio = Px(Tema.Espacio.L);
            int alto = Math.Max(Px(140), area.Height);
            icono.SetBounds(espacio, Math.Max(espacio, (alto - Px(64)) / 2), Px(64), Px(64));
            int izquierda = icono.Right + espacio;
            int arriba = Math.Max(espacio, (alto - Px(84)) / 2);
            explicacion.SetBounds(izquierda, arriba, Math.Max(1, Width - izquierda - espacio), Px(48));
            boton.SetBounds(izquierda, Math.Min(alto - Px(36), explicacion.Bottom + Px(Tema.Espacio.S)),
                Math.Min(Px(Tema.AnchoBotonAmplio), Math.Max(Px(Tema.AnchoBotonCompacto), explicacion.Width)), Px(Tema.AltoBoton));
        }
    }
}
