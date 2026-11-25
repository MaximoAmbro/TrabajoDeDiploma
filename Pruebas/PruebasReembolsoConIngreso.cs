using System;
using System.Linq;
using Datos;
using Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasReembolsoConIngreso
    {
        // CP-111 — RF-07, RN-18/RN-29: no devolver entradas que ya registraron ingreso.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-111: Entrada usada rechazada antes de actualizar las demás")]
        [DataRow(true, DisplayName = "CP-111: Entrada usada revierte también la devolución previa de otra entrada")]
        public void ReembolsoRechazadoAunqueSeSuperponganLasVentanas(bool usadaAlFinal)
        {
            Fake.Limpiar();
            int anterior = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_ANTES");
            try
            {
                CambiarHorasIngreso(96);
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Compra compra = Fake.CompraConfirmada(cliente, evento, 2, true);
                var originales = RepositorioEntradas.Instance.ObtenerPorCompra(compra.Id);
                Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddHours(72));
                Fake.ComoOrganizador();
                Assert.IsTrue(GestorIngresos.Instance.Registrar(originales[0].CodigoQR, evento.Id).Valido);
                Fake.ComoCliente(cliente);
                var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorCompras.Instance.Reembolsar(originales));
                Assert.AreEqual("RN-29", error.Regla);
                StringAssert.Contains(error.Message, "ya se usó");
                int[] orden = usadaAlFinal ? new[] { originales[1].Id, originales[0].Id } : new[] { originales[0].Id, originales[1].Id };
                Assert.IsFalse(RepositorioCompras.Instance.Reembolsar(compra.Id, orden.ToList(), 2000m, true));
                Assert.IsTrue(RepositorioEntradas.Instance.ObtenerPorCompra(compra.Id).All(e => e.Estado == EstadoEntrada.Confirmada));
                Assert.AreEqual(0m, RepositorioCompras.Instance.ObtenerPorId(compra.Id).MontoReembolsado);
                Assert.IsTrue(RepositorioEntradas.Instance.PorCodigo(originales[0].CodigoQR).FechaIngreso.HasValue);
                Fake.ComoCliente(cliente);
                Assert.AreEqual(1000m, GestorCompras.Instance.Reembolsar(originales.Skip(1).ToList()));
                Assert.AreEqual(EstadoEntrada.Confirmada, RepositorioEntradas.Instance.PorCodigo(originales[0].CodigoQR).Estado);
                Assert.AreEqual(EstadoEntrada.Reembolsada, RepositorioEntradas.Instance.PorCodigo(originales[1].CodigoQR).Estado);
                Assert.AreEqual(1000m, RepositorioCompras.Instance.ObtenerPorId(compra.Id).MontoReembolsado);
            }
            finally { CambiarHorasIngreso(anterior); GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }
        }

        private static void CambiarHorasIngreso(int horas)
        {
            using (var conexion = new SqlConnection(BaseDePrueba.CadenaDePrueba()))
            using (var comando = conexion.CreateCommand())
            {
                conexion.Open();
                comando.CommandText = "UPDATE Parametros SET Valor=@Horas WHERE Clave='INGRESO_HORAS_ANTES'";
                comando.Parameters.AddWithValue("@Horas", horas.ToString(System.Globalization.CultureInfo.InvariantCulture));
                Assert.AreEqual(1, comando.ExecuteNonQuery());
            }
            GestorParametros.Instance.Recargar();
        }
    }
}
