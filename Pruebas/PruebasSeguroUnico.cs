using System;
using System.Linq;
using Datos;
using Entidades;
using Negocio;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Pruebas {
 [TestClass] public class PruebasSeguroUnico {
  [TestInitialize] public void Preparar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  [TestCleanup] public void Limpiar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  // CP-147 — RF-06, RN-28: SQL exige una póliza por entrada y permite otras entradas.
  [DataTestMethod]
  [DataRow(true,DisplayName="CP-147: Segundo seguro de la misma entrada se rechaza por UX_Seguros_ID_Entrada")]
  [DataRow(false,DisplayName="CP-147: Una entrada distinta admite su propio seguro")]
  public void CP147_UnSeguro(bool duplicada){
   var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);
   var compra=Fake.CompraConfirmada(cliente,evento,1,true);var entrada=GestorCompras.Instance.EntradasDe(compra).Single();
   if(!duplicada){compra=Fake.CompraConfirmada(cliente,evento,1,false);entrada=GestorCompras.Instance.EntradasDe(compra).Single();}
   int polizasAntes=RepositorioCompras.Instance.ObtenerSeguros(compra.Id).Count;
   using(var cn=new SqlConnection(BaseDePrueba.CadenaDePrueba())){cn.Open();using(var tx=cn.BeginTransaction()){
    using(var cmd=cn.CreateCommand()){
     cmd.Transaction=tx;cmd.CommandText="INSERT INTO Seguros(ID_Entrada,Precio,VenceEl) VALUES(@Entrada,@Precio,@Vence)";
     cmd.Parameters.AddWithValue("@Entrada",entrada.Id);cmd.Parameters.AddWithValue("@Precio",5000m);cmd.Parameters.AddWithValue("@Vence",evento.Fecha.AddHours(-48));
     if(duplicada){var error=Assert.ThrowsException<SqlException>(()=>cmd.ExecuteNonQuery());Assert.AreEqual(2601,error.Number);StringAssert.Contains(error.Message,"UX_Seguros_ID_Entrada");}
     else Assert.AreEqual(1,cmd.ExecuteNonQuery());
    }
    tx.Rollback();
   }}
   Assert.AreEqual(polizasAntes,RepositorioCompras.Instance.ObtenerSeguros(compra.Id).Count);
   Assert.AreEqual(EstadoEntrada.Confirmada,RepositorioEntradas.Instance.ObtenerPorCompra(compra.Id).Single().Estado);
   Assert.AreEqual(cliente.Id,GestorSesion.Instance.UsuarioActual.Id);
  }
 }
}
