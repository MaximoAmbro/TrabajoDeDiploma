using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioLocales : RepositorioBase
    {
        private static RepositorioLocales _instance;

        public static RepositorioLocales Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioLocales();
                }
                return _instance;
            }
        }

        private RepositorioLocales() { }

        public int Insertar(Local local)
        {
            const string consulta =
                "INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@Nombre, @Direccion, @Capacidad, @PropietarioID)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Nombre", local.Nombre);
                AgregarParametro(comando, "@Direccion", local.Direccion);
                AgregarParametro(comando, "@Capacidad", local.Capacidad);
                AgregarParametro(comando, "@PropietarioID", local.IdPropietario);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public List<Local> ObtenerPorPropietario(int idPropietario)
        {
            const string consulta =
                "SELECT ID, Nombre, Direccion, Capacidad, PropietarioID " +
                "FROM Locales WHERE PropietarioID = @PropietarioID ORDER BY Nombre";
            var locales = new List<Local>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@PropietarioID", idPropietario);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        locales.Add(new Local
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            Nombre = LeerString(lector, "Nombre"),
                            Direccion = LeerString(lector, "Direccion"),
                            Capacidad = LeerEntero(lector, "Capacidad"),
                            IdPropietario = Convert.ToInt32(lector["PropietarioID"])
                        });
                    }
                }
            }
            return locales;
        }
    }
}
