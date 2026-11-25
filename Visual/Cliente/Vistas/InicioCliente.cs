using System;
using System.Collections.Generic;
using System.Linq;
using Entidades;
using Negocio;
namespace Visual.Componentes
{
    public partial class InicioCliente : SuperficieTematica
    {
        public event Action VerEventos;
        public InicioCliente(){InitializeComponent();verEventos.Click+=(s,e)=>VerEventos?.Invoke();icono.Font=Tema.IconosInterfaz;}
        public void Cargar(int id)
        {
            if(id<=0 || !GestorUsuarios.Instance.ExisteUsuarioDeSesion()) {proximas.Text="Ingresá para ver tus próximas entradas.";return;}
            var cliente=GestorFidelizacion.Instance.ObtenerCliente(id);
            GestorFidelizacion.Instance.ReevaluarNivel(cliente,DateTime.Now);
            nivel.Text=cliente.Nivel+" · "+GestorFidelizacion.Instance.PorcentajePara(cliente.Nivel).ToString("P1",System.Globalization.CultureInfo.GetCultureInfo("es-AR"))+" de descuento";
            int compras=GestorFidelizacion.Instance.ComprasEnVentana(id,DateTime.Now);
            int umbral=GestorParametros.Instance.ObtenerEntero(cliente.Nivel==NivelFidelidad.Bronce?"FIDELIDAD_UMBRAL_PLATA":"FIDELIDAD_UMBRAL_ORO");
            siguiente.Text=cliente.Nivel==NivelFidelidad.Oro?"Ya estás en el nivel más alto. Disfrutá tus beneficios.":
                "Te faltan "+Math.Max(0,umbral-compras)+" compras confirmadas para llegar a "+(cliente.Nivel==NivelFidelidad.Bronce?"Plata":"Oro")+".";
            var agenda=new List<string>();
            foreach(var compra in GestorCompras.Instance.HistorialDe(id).Where(c=>c.Estado==EstadoCompra.Confirmada)) {
                foreach(var grupo in GestorCompras.Instance.EntradasDe(compra).Where(e=>e.Estado==EstadoEntrada.Confirmada).GroupBy(e=>e.IdEvento)) {
                    var ev=GestorEventos.Instance.ObtenerPorId(grupo.Key);
                    if(ev!=null && ev.Fecha>=DateTime.Now)agenda.Add(ev.Fecha.ToString("dd/MM HH:mm")+" · "+ev.Nombre+" · "+grupo.Count()+" entrada(s)");
                }
            }
            proximas.Text=agenda.Count==0?"Todavía no compraste entradas para tus próximos eventos. Cuando lo hagas, van a aparecer acá.":string.Join(Environment.NewLine,agenda.Take(5));
            var noticias=GestorListaEspera.Instance.AvisosAlIngresar(id);
            avisos.Text=noticias.Count==0?"Si vuelve a haber lugar en un evento de tu lista de espera, te lo mostramos acá.":
                "Volvió a haber lugar en:"+Environment.NewLine+string.Join(Environment.NewLine,noticias);
            ultimo.Text=GestorSesion.Instance.IniciadaEl.HasValue?"Sesión iniciada: "+GestorSesion.Instance.IniciadaEl.Value.ToString("dd/MM/yyyy HH:mm"):"No hay un inicio de sesión registrado.";
        }
    }
}
