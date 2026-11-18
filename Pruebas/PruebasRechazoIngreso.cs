using System;
using System.Linq;
using Datos;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Pruebas {
 [TestClass] public class PruebasRechazoIngreso {
  [TestInitialize] public void Preparar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  [TestCleanup] public void Limpiar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  // CP-146 — RF-07, RN-34: rechazo de autorización como resultado, sin excepciones ni ingreso.
  [DataTestMethod]
  [DataRow(false,DisplayName="CP-146: Cliente recibe NoAutorizado y RN-34 sin excepción al registrar ingreso")]
  [DataRow(true,DisplayName="CP-146: Sin sesión recibe NoAutorizado y RN-34 sin excepción al registrar ingreso")]
  public void CP146_Rechazo(bool sinSesion){
   var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);var compra=Fake.CompraConfirmada(cliente,evento,1,true);
   var entrada=GestorCompras.Instance.EntradasDe(compra).Single();Fake.CambiarFechaDeEvento(evento.Id,DateTime.Now.AddMinutes(10));
   if(sinSesion)GestorSesion.Instance.Cerrar();else Fake.ComoCliente(cliente);
   ResultadoIngreso resultado=null;
   try{resultado=GestorIngresos.Instance.Registrar(entrada.CodigoQR,evento.Id);}catch(Exception error){Assert.Fail("El rechazo debe ser un resultado, no una excepción: "+error.GetType().Name);}
   Assert.IsNotNull(resultado);Assert.IsFalse(resultado.Valido);Assert.AreEqual(MotivoIngreso.NoAutorizado,resultado.Motivo);
   Assert.AreEqual("RN-48",resultado.Regla);Assert.AreEqual("Esta operación es solo para el Organizador.",resultado.Mensaje);Assert.IsNull(resultado.FechaIngreso);
   var persistida=RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR);Assert.IsNull(persistida.FechaIngreso);Assert.AreEqual(EstadoEntrada.Confirmada,persistida.Estado);Assert.AreEqual(0,RepositorioEntradas.Instance.ContarIngresos(evento.Id));
   if(sinSesion)Assert.IsNull(GestorSesion.Instance.UsuarioActual);else Assert.AreEqual(cliente.Id,GestorSesion.Instance.UsuarioActual.Id);
  }
 }
}
