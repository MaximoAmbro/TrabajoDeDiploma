using System;
using System.Linq;
using Datos;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Pruebas {
 [TestClass] public class PruebasPrioridadTemporal {
  [TestInitialize] public void Preparar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  [TestCleanup] public void Limpiar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  // CP-125 — RF-10, RN-35/37: prioridad real, persistencia y límites del privilegio.
  [TestMethod("CP-125: Prioridad temporal por sector")]
  [DataRow("reembolso",false,false,DisplayName="CP-125: Reembolso rechaza no suscriptor dentro de la ventana")]
  [DataRow("reembolso",true,false,DisplayName="CP-125: Suscriptor inscripto compra dentro de la ventana")]
  [DataRow("reembolso",false,true,DisplayName="CP-125: No suscriptor compra después de la ventana")]
  [DataRow("vencimiento",false,false,DisplayName="CP-125: Reserva vencida activa prioridad")]
  [DataRow("vencimiento",true,false,DisplayName="CP-125: Suscriptor compra el cupo de reserva vencida")]
  [DataRow("rechazo",false,false,DisplayName="CP-125: Pago rechazado activa prioridad")]
  [DataRow("sin-inscripcion",true,false,DisplayName="CP-125: Suscriptor sin inscripción no usa prioridad")]
  [DataRow("reintento",false,false,DisplayName="CP-125: Reintento de pago no saltea prioridad")]
  public void CP125_Ventana(string liberacion,bool suscriptor,bool fuera) {
   var e=Fake.EventoPublicado(1000m,1);var vendedor=Fake.Cliente(NivelFidelidad.Bronce);
   var comun=Fake.Cliente(NivelFidelidad.Bronce);Compra anterior=null;
   if(liberacion=="reintento"){Fake.ComoCliente(comun);anterior=GestorCompras.Instance.Iniciar(Fake.Paquete(e,1,false),comun);var rechazada=Fake.Tarjeta();rechazada.Numero="4111111111110000";Assert.IsFalse(GestorCompras.Instance.Confirmar(anterior,rechazada));}
   Fake.ComoCliente(vendedor);
   Compra c=Fake.CompraConfirmada(vendedor,e,1,true);
   var preferido=Fake.Cliente(NivelFidelidad.Bronce);Fake.SuscripcionActiva(preferido,GestorSuscripciones.PlanMensual);
   Fake.ComoCliente(preferido);GestorListaEspera.Instance.Inscribir(e.Id,preferido.Id);
   Fake.ComoCliente(comun);GestorListaEspera.Instance.Inscribir(e.Id,comun.Id);
   Fake.ComoCliente(vendedor);
   GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(c));
   if(liberacion=="vencimiento"||liberacion=="rechazo"){Fake.ComoCliente(preferido);c=GestorCompras.Instance.Iniciar(Fake.Paquete(e,1,false),preferido);}
   if(liberacion=="vencimiento"){Fake.VencerReserva(c.Id);GestorCompras.Instance.LiberarVencidas();}
   else if(liberacion=="rechazo"){var t=Fake.Tarjeta();t.Numero="4111111111110000";Assert.IsFalse(GestorCompras.Instance.Confirmar(c,t));}

   Assert.AreEqual(1,RepositorioSectores.Instance.ObtenerDisponibles(e.Sectores.Single().Id));
   if(fuera) Fake.EnvejecerLiberacion(e.Sectores.Single().Id);
   var comprador=suscriptor?preferido:comun;
   if(liberacion=="sin-inscripcion"){comprador=Fake.Cliente(NivelFidelidad.Bronce);Fake.SuscripcionActiva(comprador,GestorSuscripciones.PlanMensual);}
   Fake.ComoCliente(comprador);var p=Fake.Paquete(e,1,false);
   if((!suscriptor||liberacion=="sin-inscripcion")&&!fuera){
    var error=Assert.ThrowsException<ReglaNegocioException>(()=>{if(anterior!=null)GestorCompras.Instance.Confirmar(anterior,Fake.Tarjeta());else GestorCompras.Instance.Iniciar(p,comprador);});Assert.AreEqual("RN-35",error.Regla);
    Assert.AreEqual(1,RepositorioSectores.Instance.ObtenerDisponibles(e.Sectores.Single().Id));
    if(anterior==null)Assert.IsFalse(RepositorioCompras.Instance.ObtenerPorUsuario(comprador.Id,new[]{EstadoCompra.Pendiente}).Any());
    else {Assert.IsNull(RepositorioCompras.Instance.ObtenerPorId(anterior.Id).ReservaVence);Assert.AreEqual(1,RepositorioPagos.Instance.ObtenerPorCompra(anterior.Id).Count);}
   }else{var nueva=GestorCompras.Instance.Iniciar(p,comprador);Assert.IsTrue(GestorCompras.Instance.Confirmar(nueva,Fake.Tarjeta()));Assert.AreEqual(0,RepositorioSectores.Instance.ObtenerDisponibles(e.Sectores.Single().Id));}
  }
 }
}
