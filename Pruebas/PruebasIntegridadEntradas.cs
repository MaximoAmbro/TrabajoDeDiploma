using System;
using Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasIntegridadEntradas
    {
        // CP-110 — RN-18/RN-29: los mismos cuatro estados de la clase están restringidos en SQL.
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
