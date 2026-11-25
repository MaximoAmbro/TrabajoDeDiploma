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
            Location=area.Location;Size=area.Size;
            int alto=Math.Max(140,area.Height);
            icono.Location=new System.Drawing.Point(20,Math.Max(12,(alto-64)/2));
            explicacion.Location=new System.Drawing.Point(104,Math.Max(12,(alto-84)/2));
            explicacion.Size=new System.Drawing.Size(Math.Max(180,Width-124),48);
            boton.Location=new System.Drawing.Point(104,Math.Min(alto-50,Math.Max(70,(alto-84)/2+54)));
            boton.Size=new System.Drawing.Size(Math.Min(240,Math.Max(160,Width-128)),44);
        }
    }
}
