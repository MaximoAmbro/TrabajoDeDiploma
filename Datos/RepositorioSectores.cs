using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioSectores : RepositorioBase
    {
        private const string InsertarSector =
            "INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) " +
            "OUTPUT INSERTED.ID " +
            "VALUES (@EventoID, @Nombre, @Precio, @Capacidad)";

        private static RepositorioSectores _instance;

        public static RepositorioSectores Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioSectores();
                }
                return _instance;
            }
        }

        private RepositorioSectores() { }

        public int Insertar(Sector sector)
        {
            using (SqlConnection conexion = AbrirConexion())
            {
                return Insertar(sector, conexion, null);
            }
        }

        // Lo usa RepositorioEventos para dar de alta el evento y sus sectores en una sola transacción.
        internal int Insertar(Sector sector, SqlConnection conexion, SqlTransaction transaccion)
        {
            using (SqlCommand comando = CrearComando(conexion, InsertarSector, transaccion))
            {
                AgregarParametro(comando, "@EventoID", sector.IdEvento);
                AgregarParametro(comando, "@Nombre", sector.Nombre);
                AgregarParametro(comando, "@Precio", sector.Precio);
                AgregarParametro(comando, "@Capacidad", sector.Capacidad);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        // Devuelve null si no existe.
        public Sector ObtenerPorId(int idSector)
        {
            List<Sector> sectores = Consultar(Columnas + "WHERE ID = @Valor", idSector);
            return sectores.Count == 0 ? null : sectores[0];
        }

        public List<Sector> ObtenerPorEvento(int idEvento)
        {
            return Consultar(Columnas + "WHERE EventoID = @Valor ORDER BY Precio DESC", idEvento);
        }

        private const string Columnas =
            "SELECT ID, EventoID, Nombre, Precio, Capacidad, CantidadReservada FROM Sectores ";

        private List<Sector> Consultar(string consulta, int valor)
        {
            var sectores = new List<Sector>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Valor", valor);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        sectores.Add(new Sector
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            IdEvento = Convert.ToInt32(lector["EventoID"]),
                            Nombre = LeerString(lector, "Nombre"),
                            Precio = LeerDecimal(lector, "Precio"),
                            Capacidad = LeerEntero(lector, "Capacidad"),
                            CantidadReservada = Convert.ToInt32(lector["CantidadReservada"])
                        });
                    }
                }
            }
            return sectores;
        }

        public int ContarEntradas(int idSector)
        {
            const string consulta = "SELECT COUNT(*) FROM Entradas WHERE ID_Sector = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idSector);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        // Devuelve false si no existe el sector.
        public bool Eliminar(int idSector)
        {
            const string consulta = "DELETE FROM Sectores WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idSector);
                return comando.ExecuteNonQuery() > 0;
            }
        }
    }
}
