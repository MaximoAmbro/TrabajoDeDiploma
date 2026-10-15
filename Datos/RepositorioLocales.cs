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

        // Devuelve null si no existe.
        public Local ObtenerPorId(int idLocal)
        {
            List<Local> locales = Consultar(Columnas + "WHERE ID = @Valor", idLocal);
            return locales.Count == 0 ? null : locales[0];
        }

        public List<Local> ObtenerPorPropietario(int idPropietario)
        {
            return Consultar(Columnas + "WHERE PropietarioID = @Valor ORDER BY Nombre", idPropietario);
        }

        private const string Columnas =
            "SELECT ID, Nombre, Direccion, Capacidad, PropietarioID FROM Locales ";

        private List<Local> Consultar(string consulta, int valor)
        {
            var locales = new List<Local>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Valor", valor);
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
