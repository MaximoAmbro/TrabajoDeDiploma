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
        // CP-111 — RN-18/RN-29: ingreso y devolución se superponen al ampliar el parámetro.
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
                var originales = GestorCompras.Instance.EntradasDe(compra);
                Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddHours(72));
                Fake.ComoAdministrador();
                Assert.IsTrue(GestorIngresos.Instance.Registrar(originales[0].CodigoQR, evento.Id).Valido);
                var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorCompras.Instance.Reembolsar(originales));
                Assert.AreEqual("RN-29", error.Regla);
                StringAssert.Contains(error.Message, "ya se usó");
                int[] orden = usadaAlFinal ? new[] { originales[1].Id, originales[0].Id } : new[] { originales[0].Id, originales[1].Id };
                Assert.IsFalse(RepositorioCompras.Instance.Reembolsar(compra.Id, orden.ToList(), 2000m, true));
                Assert.IsTrue(GestorCompras.Instance.EntradasDe(compra).All(e => e.Estado == EstadoEntrada.Confirmada));
                Assert.AreEqual(0m, GestorCompras.Instance.ObtenerPorId(compra.Id).MontoReembolsado);
                Assert.IsTrue(RepositorioEntradas.Instance.PorCodigo(originales[0].CodigoQR).FechaIngreso.HasValue);
                Assert.AreEqual(1000m, GestorCompras.Instance.Reembolsar(originales.Skip(1).ToList()));
                Assert.AreEqual(EstadoEntrada.Confirmada, RepositorioEntradas.Instance.PorCodigo(originales[0].CodigoQR).Estado);
                Assert.AreEqual(EstadoEntrada.Reembolsada, RepositorioEntradas.Instance.PorCodigo(originales[1].CodigoQR).Estado);
                Assert.AreEqual(1000m, GestorCompras.Instance.ObtenerPorId(compra.Id).MontoReembolsado);
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
