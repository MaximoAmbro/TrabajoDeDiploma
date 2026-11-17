using System;
namespace Visual.Componentes
{
    public partial class EstadoVacio : SuperficieTematica
    {
        public event Action Accion;
        public EstadoVacio(){InitializeComponent();icono.Font=Tema.IconosInterfaz;icono.ForeColor=Tema.TextoSecundario;boton.Click+=(s,e)=>Accion?.Invoke();}
        public void Cargar(string texto,string accion){explicacion.Text=texto;boton.Text=accion;}
    }
}
