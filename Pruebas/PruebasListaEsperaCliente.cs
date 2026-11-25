using System;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pruebas.InterfazGrupoA;
namespace Pruebas {
 [TestClass] public class PruebasListaEsperaCliente {
  private static void Preparar(out Cliente comprador,out Cliente esperando,out Evento evento,out Compra compra) {
   comprador=Fake.Cliente(NivelFidelidad.Bronce); esperando=Fake.Cliente(NivelFidelidad.Bronce);
   evento=Fake.EventoPublicado(1000m,1); compra=Fake.CompraConfirmada(comprador,evento,1,true);
   Fake.ComoCliente(esperando); GestorListaEspera.Instance.Inscribir(evento.Id,esperando.Id);
  }
  // CP-117 — RF-10, RN-35/RN-41: posición privada, estable y con prioridad congelada.
  [DataTestMethod]
  [DataRow("propia",DisplayName="CP-117: Cliente consulta su posición y el enlace la muestra")]
  [DataRow("prioridad",DisplayName="CP-117: Posición respeta prioridad de suscriptores")]
  [DataRow("ajena",DisplayName="CP-117: Cliente no consulta la posición de otro")]
  [DataRow("sin-sesion",DisplayName="CP-117: Sin sesión no se consulta posición")]
  public void CP117_Posicion(string caso) {
   EnSta(()=> {
    Cliente comprador,esperando; Evento evento; Compra compra; Preparar(out comprador,out esperando,out evento,out compra);
    if(caso=="ajena") {Assert.ThrowsException<ReglaNegocioException>(()=>GestorListaEspera.Instance.ConsultarPosicion(evento.Id,comprador.Id));return;}
    if(caso=="sin-sesion") {GestorSesion.Instance.Cerrar();Assert.ThrowsException<ReglaNegocioException>(()=>GestorListaEspera.Instance.ConsultarPosicion(evento.Id,esperando.Id));return;}
    if(caso=="prioridad") {
     var otro=Fake.Cliente(NivelFidelidad.Bronce); Fake.SuscripcionActiva(otro,GestorSuscripciones.PlanMensual);
     Fake.ComoCliente(otro); GestorListaEspera.Instance.Inscribir(evento.Id,otro.Id); Fake.ComoCliente(esperando);
     Assert.AreEqual(2,GestorListaEspera.Instance.ConsultarPosicion(evento.Id,esperando.Id));return;
    }
    Assert.AreEqual(1,GestorListaEspera.Instance.ConsultarPosicion(evento.Id,esperando.Id));
    using(var form=new Visual.FrmCompraCliente{IDUsuario=esperando.Id,IDEvento=evento.Id}) {
     Mostrar(form); var enlace=ControlDe<LinkLabel>(form,"lnkPosicionEspera"); Assert.IsTrue(enlace.Visible);
     var metodo=form.GetType().GetMethod("lnkPosicionEspera_LinkClicked",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
     DialogosPrueba.Comprobar(()=>metodo.Invoke(form,new object[]{enlace,null}),new DialogoEsperado{Titulo="Mi posición",Texto="posición actual en la lista es 1"});
    }
   });
  }
  // CP-118 — RF-10/RN-41: aviso de disponibilidad con datos existentes, sin subsistema nuevo.
  [DataTestMethod]
  [DataRow("agotado",DisplayName="CP-118: No se avisa mientras el evento siga agotado")]
  [DataRow("liberado",DisplayName="CP-118: Liberar una entrada genera aviso al iniciar sesión")]
  [DataRow("ajena",DisplayName="CP-118: Cliente no lee avisos de otra cuenta")]
  [DataRow("sin-sesion",DisplayName="CP-118: Sin sesión no se leen avisos")]
  [DataRow("reservado",DisplayName="CP-118: Una reserva que ocupa el último cupo impide el aviso")]
  [DataRow("suspendido",DisplayName="CP-118: Un evento suspendido no genera aviso de venta")]
  public void CP118_Avisos(string caso) {
   EnSta(()=> {
    Cliente comprador,esperando; Evento evento; Compra compra; Preparar(out comprador,out esperando,out evento,out compra);
    if(caso=="ajena") {Assert.ThrowsException<ReglaNegocioException>(()=>GestorListaEspera.Instance.AvisosAlIngresar(comprador.Id));return;}
    if(caso=="sin-sesion") {GestorSesion.Instance.Cerrar();Assert.ThrowsException<ReglaNegocioException>(()=>GestorListaEspera.Instance.AvisosAlIngresar(esperando.Id));return;}
    if(caso=="agotado") {Assert.AreEqual(0,GestorListaEspera.Instance.AvisosAlIngresar(esperando.Id).Count);return;}
    Fake.ComoCliente(comprador); GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(compra));
    if(caso=="reservado") {
     GestorCompras.Instance.Iniciar(Fake.Paquete(evento,1,false),GestorFidelizacion.Instance.ObtenerCliente(comprador.Id));
     Fake.ComoCliente(esperando);Assert.AreEqual(0,GestorListaEspera.Instance.AvisosAlIngresar(esperando.Id).Count);return;
    }
    if(caso=="suspendido") {Fake.ComoOrganizador();GestorEventos.Instance.Suspender(evento.Id);Fake.ComoCliente(esperando);Assert.AreEqual(0,GestorListaEspera.Instance.AvisosAlIngresar(esperando.Id).Count);return;}
    Fake.ComoCliente(esperando);
    CollectionAssert.AreEqual(new[]{evento.Nombre},GestorListaEspera.Instance.AvisosAlIngresar(esperando.Id));
    // No se marca como leído ni se asigna cupo; el mismo estado produce el mismo aviso.
    CollectionAssert.AreEqual(new[]{evento.Nombre},GestorListaEspera.Instance.AvisosAlIngresar(esperando.Id));
    GestorSesion.Instance.Cerrar();
    using(var form=new Visual.frmInicio()) {
     Mostrar(form);ControlDe<TextBox>(form,"txtMailLogin").Text=esperando.Mail;ControlDe<TextBox>(form,"txtContraseñaLogin").Text="Cliente123";
     DialogosPrueba.Comprobar(()=>ControlDe<Button>(form,"btninicio").PerformClick(),new DialogoEsperado{Titulo="Lista de espera",Texto=evento.Nombre});
     Assert.IsTrue(GestorSesion.Instance.HaySesion);Assert.IsTrue(Application.OpenForms.OfType<Visual.frmMenuCliente>().Any(f=>f.Visible));
    }
   });
  }
 }
}
