using System;
using System.Linq;
using System.Windows.Forms;
using Datos;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pruebas.InterfazGrupoA;
namespace Pruebas {
 [TestClass] public class PruebasReglasD5 {
  // CP-121 — RN-26/RF-13: vencimiento persistido; consultar y abrir la pantalla no renueva.
  [TestMethod("CP-121: Una suscripción vencida no se renueva al consultar ni abrir su pantalla")]
  public void CP121_VencidaNoSeRenueva() {
   EnSta(()=> {
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);Fake.ComoCliente(cliente);
    var original=GestorSuscripciones.Instance.Contratar(cliente.Id,GestorSuscripciones.PlanMensual);
    Assert.IsTrue(GestorSuscripciones.Instance.TieneActiva(cliente.Id));
    Fake.VencerSuscripcion(cliente.Id);
    for(int consulta=0;consulta<3;consulta++) {
     Assert.IsNull(GestorSuscripciones.Instance.ActivaDe(cliente.Id));
     Assert.IsFalse(GestorSuscripciones.Instance.TieneActiva(cliente.Id));
    }
    using(var form=new Visual.frmSuscripcion{IDUsuario=cliente.Id}) {
     Mostrar(form);Assert.IsTrue(ControlDe<Button>(form,"btnContratar").Enabled);
    }
    var persistida=RepositorioSuscripciones.Instance.ObtenerActiva(cliente.Id);
    Assert.IsNotNull(persistida);Assert.AreEqual(original.Id,persistida.Id);
    Assert.AreEqual(DateTime.Today,persistida.FechaFin);Assert.AreEqual(original.Precio,persistida.Precio);
    Assert.IsFalse(GestorSuscripciones.Instance.TieneActiva(cliente.Id));
   });
  }
  // CP-122 — RN-28/RF-06: intento real de incorporar seguro después de confirmar; sin nueva póliza ni cobro.
  [TestMethod("CP-122: Agregar seguro a una compra confirmada se rechaza sin modificarla")]
  public void CP122_SeguroPosteriorRechazado() {
   EnSta(()=> {
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);
    var compra=Fake.CompraConfirmada(cliente,evento,1,false);Fake.ComoCliente(cliente);
    decimal total=compra.TotalPagado;int pagos=GestorCompras.Instance.PagosDe(compra).Count;
    Assert.AreEqual(0,GestorCompras.Instance.SegurosDe(compra).Count);
    compra.ConSeguro=true;compra.MontoSeguro=GestorParametros.Instance.ObtenerDecimal("SEGURO_PRECIO");
    var error=Assert.ThrowsException<InvalidOperationException>(()=>GestorCompras.Instance.Confirmar(compra,Fake.Tarjeta()));
    Assert.AreEqual("La compra ya no está pendiente de pago.",error.Message);
    var guardada=GestorCompras.Instance.ObtenerPorId(compra.Id);
    Assert.AreEqual(EstadoCompra.Confirmada,guardada.Estado);Assert.IsFalse(guardada.ConSeguro);
    Assert.AreEqual(0m,guardada.MontoSeguro);Assert.AreEqual(total,guardada.TotalPagado);
    Assert.AreEqual(0,GestorCompras.Instance.SegurosDe(guardada).Count);Assert.AreEqual(pagos,GestorCompras.Instance.PagosDe(guardada).Count);
   });
  }
 }
}
