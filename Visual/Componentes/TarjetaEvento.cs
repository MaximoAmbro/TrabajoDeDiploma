using System;
using System.Globalization;
using System.Linq;
using Entidades;
namespace Visual.Componentes
{
    public partial class TarjetaEvento : SuperficieTematica
    {
        public event Action<Evento> VerEntradas;
        private Evento evento;
        public TarjetaEvento()
        {
            InitializeComponent();
            titulo.Font=Tema.TituloTarjeta;fecha.Font=Tema.DatoTarjeta;lugar.Font=Tema.DatoTarjeta;
            precio.Font=Tema.PrecioTarjeta;precio.ForeColor=Tema.Texto;
            accion.Click+=(s,e)=>VerEntradas?.Invoke(evento);
            Paint+=Disposicion.PintarTarjeta;
            Disposicion.ActivarTarjeta(this, accion);
        }
        public void Cargar(Evento dato,Local local,System.Collections.Generic.IEnumerable<Sector> sectores)
        {
            evento=dato;portada.Cargar(dato.Nombre,dato.Categoria,dato.ImagenPortada);
            titulo.Text=dato.Nombre;fecha.Text=dato.Fecha.ToString("ddd d MMM · HH:mm",CultureInfo.GetCultureInfo("es-AR"));
            lugar.Text=local==null?"Consultá los datos del evento":local.Nombre+" · "+local.Direccion;
            var precios=sectores.Select(s=>s.Precio).ToArray();
            precio.Text=precios.Length==0?"Próximamente":"Desde "+precios.Min().ToString("C0",CultureInfo.GetCultureInfo("es-AR"));
            AccessibleName=dato.Nombre;
        }
    }
}
