using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioServicios : RepositorioBase
    {
        private static RepositorioServicios _instance;

        public static RepositorioServicios Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioServicios();
                }
                return _instance;
            }
        }

        private RepositorioServicios() { }

        // RN-14: servicios activos habilitados para el evento en ServiciosEvento.
        public List<Servicio> ObtenerPorEvento(int idEvento)
        {
            const string consulta =
                "SELECT s.ID, s.Nombre, s.Tipo, s.Precio, s.Activo " +
                "FROM Servicios s JOIN ServiciosEvento se ON se.ID_Servicio = s.ID " +
                "WHERE se.ID_Evento = @EventoID AND s.Activo = 1 ORDER BY s.Tipo, s.Nombre";
            var servicios = new List<Servicio>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@EventoID", idEvento);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        servicios.Add(new Servicio
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            Nombre = LeerString(lector, "Nombre"),
                            Tipo = LeerString(lector, "Tipo"),
                            Precio = LeerDecimal(lector, "Precio"),
                            Activo = Convert.ToBoolean(lector["Activo"])
                        });
                    }
                }
            }
            return servicios;
        }
    }
}
