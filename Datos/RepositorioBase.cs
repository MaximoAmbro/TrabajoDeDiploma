using System;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // Base de todos los repositorios: apertura de conexión y helpers de comandos.
    public abstract class RepositorioBase
    {
        // Códigos de SQL Server para violación de UNIQUE / PRIMARY KEY.
        private const int ErrorClaveUnica = 2627;
        private const int ErrorIndiceUnico = 2601;

        // Devuelve una conexión ya abierta. El llamador la cierra con using.
        protected SqlConnection AbrirConexion()
        {
            SqlConnection conexion = Conexion.Instance.Crear();
            conexion.Open();
            return conexion;
        }

        protected SqlCommand CrearComando(SqlConnection conexion, string consulta, SqlTransaction transaccion = null)
        {
            return new SqlCommand(consulta, conexion, transaccion);
        }

        // AddWithValue no acepta null: lo traduce a DBNull.
        protected void AgregarParametro(SqlCommand comando, string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        protected static string LeerString(SqlDataReader lector, string columna)
        {
            object valor = lector[columna];
            return valor == DBNull.Value ? null : valor.ToString();
        }

        protected static DateTime? LeerFechaNula(SqlDataReader lector, string columna)
        {
            object valor = lector[columna];
            return valor == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(valor);
        }

        // Columnas heredadas anulables (Sectores.Capacidad, Locales.Capacidad...): NULL se lee como 0.
        protected static int LeerEntero(SqlDataReader lector, string columna)
        {
            object valor = lector[columna];
            return valor == DBNull.Value ? 0 : Convert.ToInt32(valor);
        }

        protected static decimal LeerDecimal(SqlDataReader lector, string columna)
        {
            object valor = lector[columna];
            return valor == DBNull.Value ? 0m : Convert.ToDecimal(valor);
        }

        protected static bool EsViolacionUnicidad(SqlException ex)
        {
            return ex.Number == ErrorClaveUnica || ex.Number == ErrorIndiceUnico;
        }
    }

    // Se lanza cuando la base rechaza un registro por una restricción de unicidad.
    // Negocio la traduce a la regla que corresponda (p. ej. RN-01).
    public class RegistroDuplicadoException : Exception
    {
        public RegistroDuplicadoException(string mensaje, Exception interna)
            : base(mensaje, interna) { }
    }
}
