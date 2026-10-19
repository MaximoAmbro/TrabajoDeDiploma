using System;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // RF-13: suscripciones. UX_Suscripcion_Activa (índice filtrado WHERE Activa = 1) impone RN-25
    // en la base: una sola fila Activa por usuario.
    public sealed class RepositorioSuscripciones : RepositorioBase
    {
        private const string Columnas =
            "SELECT ID, ID_Usuario, [Plan], FechaInicio, FechaFin, Precio, Activa FROM Suscripciones ";

        private static RepositorioSuscripciones _instance;

        public static RepositorioSuscripciones Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioSuscripciones();
                }
                return _instance;
            }
        }

        private RepositorioSuscripciones() { }

        // La fila con Activa = 1 del usuario, esté o no vencida. null si no hay.
        public Suscripcion ObtenerActiva(int idUsuario)
        {
            const string consulta = Columnas + "WHERE ID_Usuario = @IdUsuario AND Activa = 1";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdUsuario", idUsuario);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return null;
                    }
                    return new Suscripcion
                    {
                        Id = Convert.ToInt32(lector["ID"]),
                        IdUsuario = Convert.ToInt32(lector["ID_Usuario"]),
                        Plan = LeerString(lector, "Plan"),
                        FechaInicio = Convert.ToDateTime(lector["FechaInicio"]),
                        FechaFin = Convert.ToDateTime(lector["FechaFin"]),
                        Precio = LeerDecimal(lector, "Precio"),
                        Activa = Convert.ToBoolean(lector["Activa"])
                    };
                }
            }
        }

        // Inserta la suscripción nueva. Antes, en la misma transacción, cierra (Activa = 0) la que
        // el usuario tenga vencida antes de hoy: RN-26, vencida no cuenta aunque siga en 1, y si no
        // se cierra el índice impide contratar otra. Si queda una activa vigente, el índice
        // rechaza el INSERT: lanza RegistroDuplicadoException y no escribe nada.
        public int Insertar(Suscripcion suscripcion, DateTime hoy)
        {
            const string cerrarVencida =
                "UPDATE Suscripciones SET Activa = 0 " +
                "WHERE ID_Usuario = @IdUsuario AND Activa = 1 AND FechaFin <= @Hoy";
            const string insertar =
                "INSERT INTO Suscripciones (ID_Usuario, [Plan], FechaInicio, FechaFin, Precio, Activa) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@IdUsuario, @Plan, @FechaInicio, @FechaFin, @Precio, 1)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    using (SqlCommand comando = CrearComando(conexion, cerrarVencida, transaccion))
                    {
                        AgregarParametro(comando, "@IdUsuario", suscripcion.IdUsuario);
                        AgregarParametro(comando, "@Hoy", hoy.Date);
                        comando.ExecuteNonQuery();
                    }
                    using (SqlCommand comando = CrearComando(conexion, insertar, transaccion))
                    {
                        AgregarParametro(comando, "@IdUsuario", suscripcion.IdUsuario);
                        AgregarParametro(comando, "@Plan", suscripcion.Plan);
                        AgregarParametro(comando, "@FechaInicio", suscripcion.FechaInicio.Date);
                        AgregarParametro(comando, "@FechaFin", suscripcion.FechaFin.Date);
                        AgregarParametro(comando, "@Precio", suscripcion.Precio);
                        suscripcion.Id = Convert.ToInt32(comando.ExecuteScalar());
                    }
                    transaccion.Commit();
                    suscripcion.Activa = true;
                    return suscripcion.Id;
                }
                catch (SqlException ex) when (EsViolacionUnicidad(ex))
                {
                    transaccion.Rollback();
                    throw new RegistroDuplicadoException("El usuario ya tiene una suscripción activa.", ex);
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // RN-26: la baja pone Activa en 0 y libera el índice. Devuelve false si no había activa.
        public bool Cancelar(int idUsuario)
        {
            const string consulta =
                "UPDATE Suscripciones SET Activa = 0 WHERE ID_Usuario = @IdUsuario AND Activa = 1";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdUsuario", idUsuario);
                return comando.ExecuteNonQuery() > 0;
            }
        }
    }
}
