using System;
using System.Data;
using Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasIntegridadEntradas
    {
        // CP-181 — RF-06, RN-08: impedir cantidades nulas o distintas de una por entrada vendida.
        [TestMethod("CP-181: SQL rechaza CantidadVendida nula, cero o distinta de uno")]
        public void CantidadVendidaDebeSerUno()
        {
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            var evento = Fake.EventoPublicado(1000m, 5);
            try
            {
                using (var conexion = new SqlConnection(BaseDePrueba.CadenaDePrueba()))
                {
                    conexion.Open();
                    int sector;
                    using (var comando = conexion.CreateCommand())
                    {
                        comando.CommandText = "SELECT TOP 1 ID FROM Sectores WHERE EventoID=@Evento";
                        comando.Parameters.AddWithValue("@Evento", evento.Id);
                        sector = Convert.ToInt32(comando.ExecuteScalar());
                    }

                    RechazarCantidad(conexion, evento.Id, cliente.Id, sector, DBNull.Value, 515, "CantidadVendida");
                    RechazarCantidad(conexion, evento.Id, cliente.Id, sector, 2, 547, "CK_Entradas_CantidadVendida");
                    RechazarCantidad(conexion, evento.Id, cliente.Id, sector, 0, 547, "CK_Entradas_CantidadVendida");
                }
            }
            finally
            {
                Fake.Limpiar();
            }
        }

        private static void RechazarCantidad(SqlConnection conexion, int evento, int usuario, int sector,
            object cantidad, int codigoEsperado, string motivoEsperado)
        {
            string qr = "CP181-" + Guid.NewGuid().ToString("N");
            SqlException rechazo = null;
            using (var transaccion = conexion.BeginTransaction())
            using (var comando = conexion.CreateCommand())
            {
                comando.Transaction = transaccion;
                comando.CommandText = "INSERT INTO Entradas " +
                    "(ID_Evento, ID_Sector, ID_Usuario, Precio, CantidadVendida, FechaVenta, Estado, CodigoQR) " +
                    "VALUES (@Evento, @Sector, @Usuario, 1000, @Cantidad, GETDATE(), 'Confirmada', @QR)";
                comando.Parameters.AddWithValue("@Evento", evento);
                comando.Parameters.AddWithValue("@Sector", sector);
                comando.Parameters.AddWithValue("@Usuario", usuario);
                comando.Parameters.Add("@Cantidad", SqlDbType.Int).Value = cantidad;
                comando.Parameters.AddWithValue("@QR", qr);
                try { comando.ExecuteNonQuery(); }
                catch (SqlException ex) { rechazo = ex; }
                transaccion.Rollback();
            }

            Assert.IsNotNull(rechazo, "SQL aceptó una entrada con CantidadVendida=" + cantidad + ".");
            Assert.AreEqual(codigoEsperado, rechazo.Number, rechazo.Message);
            StringAssert.Contains(rechazo.Message, motivoEsperado);
            using (var comando = conexion.CreateCommand())
            {
                comando.CommandText = "SELECT COUNT(*) FROM Entradas WHERE CodigoQR=@QR";
                comando.Parameters.AddWithValue("@QR", qr);
                Assert.AreEqual(0, Convert.ToInt32(comando.ExecuteScalar()), "La fila rechazada quedó persistida.");
            }
        }

        // CP-110 — RF-07, RN-18: restringir los estados persistidos de una entrada a los valores del dominio.
        [TestMethod("CP-110: SQL admite los cuatro estados de entrada y rechaza uno ajeno")]
        public void EstadosRestringidos()
        {
            using (var conexion = new SqlConnection(BaseDePrueba.CadenaDePrueba()))
            {
                conexion.Open();
                foreach (string estado in Enum.GetNames(typeof(EstadoEntrada)))
                using (var transaccion = conexion.BeginTransaction())
                using (var comando = conexion.CreateCommand())
                {
                    comando.Transaction = transaccion;
                    comando.CommandText = "UPDATE Entradas SET Estado=@Estado WHERE ID=(SELECT MIN(ID) FROM Entradas)";
                    comando.Parameters.AddWithValue("@Estado", estado);
                    Assert.AreEqual(1, comando.ExecuteNonQuery());
                    transaccion.Rollback();
                }
                using (var transaccion = conexion.BeginTransaction())
                using (var comando = conexion.CreateCommand())
                {
                    comando.Transaction = transaccion;
                    comando.CommandText = "UPDATE Entradas SET Estado='Usada' WHERE ID=(SELECT MIN(ID) FROM Entradas)";
                    var error = Assert.ThrowsException<SqlException>(() => comando.ExecuteNonQuery());
                    Assert.AreEqual(547, error.Number);
                    StringAssert.Contains(error.Message, "CK_Entradas_Estado");
                    transaccion.Rollback();
                }
                using (var comando = conexion.CreateCommand())
                {
                    comando.CommandText = "SELECT COUNT(*) FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Entradas') AND name='CK_Entradas_Estado' AND is_disabled=0 AND is_not_trusted=0";
                    Assert.AreEqual(1, Convert.ToInt32(comando.ExecuteScalar()));
                }
            }
        }
    }
}
