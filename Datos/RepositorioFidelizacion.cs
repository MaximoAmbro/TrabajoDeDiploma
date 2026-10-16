using System;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // RF-08: datos del nivel de fidelidad (Usuario.Nivel, Usuario.FechaUltimaEvaluacion) y
    // conteo de compras de la ventana móvil sobre la tabla Compras.
    public sealed class RepositorioFidelizacion : RepositorioBase
    {
        private static RepositorioFidelizacion _instance;

        public static RepositorioFidelizacion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioFidelizacion();
                }
                return _instance;
            }
        }

        private RepositorioFidelizacion() { }

        // RN-19 + RN-24: solo compras Confirmadas, en el intervalo (desde, hasta].
        // Consulta de docs/03-esquema-datos.md; la ventana la calcula Negocio.
        public int ContarComprasConfirmadas(int idUsuario, DateTime desde, DateTime hasta)
        {
            const string consulta =
                "SELECT COUNT(*) FROM Compras " +
                "WHERE ID_Usuario = @IdUsuario " +
                "  AND Estado = 'Confirmada' " +
                "  AND FechaCompra > @Desde " +
                "  AND FechaCompra <= @Hasta";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdUsuario", idUsuario);
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        // Cliente con su nivel persistido (RN-22). Devuelve null si no existe el usuario.
        public Cliente ObtenerCliente(int idUsuario)
        {
            const string consulta =
                "SELECT ID, Nombre, Apellido, Mail, TipoUsuario, Nivel, FechaUltimaEvaluacion " +
                "FROM Usuario WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idUsuario);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return null;
                    }
                    NivelFidelidad nivel;
                    if (!Enum.TryParse(LeerString(lector, "Nivel"), out nivel))
                    {
                        nivel = NivelFidelidad.Bronce;
                    }
                    return new Cliente
                    {
                        Id = Convert.ToInt32(lector["ID"]),
                        Nombre = LeerString(lector, "Nombre"),
                        Apellido = LeerString(lector, "Apellido"),
                        Mail = LeerString(lector, "Mail"),
                        TipoUsuario = LeerString(lector, "TipoUsuario"),
                        Nivel = nivel,
                        FechaUltimaEvaluacion = LeerFechaNula(lector, "FechaUltimaEvaluacion")
                    };
                }
            }
        }

        // RN-23: persiste el resultado de la reevaluación mensual.
        public void ActualizarNivel(int idUsuario, NivelFidelidad nivel, DateTime fechaEvaluacion)
        {
            const string consulta =
                "UPDATE Usuario SET Nivel = @Nivel, FechaUltimaEvaluacion = @Fecha WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Nivel", nivel.ToString());
                AgregarParametro(comando, "@Fecha", fechaEvaluacion.Date);
                AgregarParametro(comando, "@ID", idUsuario);
                comando.ExecuteNonQuery();
            }
        }
    }
}
