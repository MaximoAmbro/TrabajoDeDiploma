using System;
using System.Linq;
using Entidades;
using Negocio;
namespace Visual.Componentes
{
    public partial class FichaEvento : SuperficieTematica
    {
        private Evento evento;
        private int? elegido;
        public event Action Volver;
        public event Action<Evento,int?> Comprar;
        public FichaEvento()
        {
            InitializeComponent();
            volver.Click+=(s,e)=>Volver?.Invoke();comprar.Click+=(s,e)=>Comprar?.Invoke(evento,elegido);
            mapa.SectorElegido+=id=>{elegido=id;seleccion.Text="Elegiste "+GestorEventos.Instance.Sectores(evento.Id).Single(x=>x.Id==id).Nombre+". Podés ajustar la cantidad al armar el paquete.";};
        }
        public void Cargar(Evento dato)
        {
            evento=dato;elegido=null;portada.Cargar(dato.Nombre,dato.Categoria,dato.ImagenPortada);
            var local=GestorLocales.Instance.PorId(dato.IdLocal);
            titulo.Text=dato.Nombre;
            detalle.Text=dato.Fecha.ToString("dddd d 'de' MMMM · HH:mm",System.Globalization.CultureInfo.GetCultureInfo("es-AR"))+Environment.NewLine+
                (local==null?"":local.Nombre+Environment.NewLine+local.Direccion)+Environment.NewLine+Environment.NewLine+
                (dato.FinPreventa.HasValue && dato.FinPreventa.Value>DateTime.Now?"Preventa para suscriptores hasta "+dato.FinPreventa.Value.ToString("dd/MM HH:mm"):"Venta general")+Environment.NewLine+
                "Elegí un sector en el mapa para empezar tu paquete.";
            mapa.Cargar(GestorEventos.Instance.Sectores(dato.Id),GestorPaquetes.Instance.DisponiblesDe);
            mapa.Height=mapa.AltoParaTodosLosSectores;
            seleccion.Top=mapa.Bottom+16;
            seleccion.Text="Mapa orientativo: los sectores más caros están cerca del escenario.";
        }
    }
}
