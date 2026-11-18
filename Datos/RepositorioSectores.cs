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

        // Inserta el sector y cambia el estado de su evento en una misma transacción
        // (un Borrador pasa a Publicado con su primer sector con capacidad).
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

        // RN-08 + RN-15: capacidad menos vendidas confirmadas menos reservadas vigentes
        // (docs/03-esquema-datos.md, "Cupo disponible de un sector"). 0 si el sector no existe.
        public int ObtenerDisponibles(int idSector)
        {
            const string consulta =
                "SELECT s.Capacidad " +
                "     - ISNULL((SELECT SUM(e.CantidadVendida) FROM Entradas e " +
                "               WHERE e.ID_Sector = s.ID AND e.Estado = 'Confirmada'), 0) " +
                "     - s.CantidadReservada AS Disponible " +
                "FROM Sectores s WHERE s.ID = @IdSector";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdSector", idSector);
                object valor = comando.ExecuteScalar();
                return valor == null || valor == DBNull.Value ? 0 : Convert.ToInt32(valor);
            }
        }

        // RN-08 + RN-15: suma la cantidad a CantidadReservada solo si el cupo alcanza. La condición
        // va en el mismo UPDATE, así dos reservas simultáneas no pueden pasarse del cupo.
        // Devuelve false si no alcanza. Lo usa RepositorioCompras dentro de su transacción.
        internal bool Reservar(int idSector, int cantidad, int idCompra, SqlConnection conexion, SqlTransaction transaccion)
        {
            // Mismo bloqueo y transacción que la reserva: no hay hueco entre prioridad y cupo.
            const string prioridad =
                "SELECT COUNT(*) FROM Sectores s WITH (UPDLOCK, HOLDLOCK) WHERE s.ID=@IdSector " +
                "AND s.CupoLiberadoEl <= GETDATE() AND DATEADD(MINUTE, " +
                "(SELECT CONVERT(INT,Valor) FROM Parametros WHERE Clave='PRIORIDAD_ESPERA_MINUTOS'), s.CupoLiberadoEl) > GETDATE() " +
                "AND EXISTS (SELECT 1 FROM ListaEspera l WHERE l.ID_Evento=s.EventoID AND l.ConSuscripcion=1) " +
                "AND NOT EXISTS (SELECT 1 FROM ListaEspera l JOIN Compras c ON c.ID_Usuario=l.ID_Usuario " +
                "WHERE c.ID=@IdCompra AND l.ID_Evento=s.EventoID AND l.ConSuscripcion=1)";
            using (SqlCommand comando = CrearComando(conexion, prioridad, transaccion))
            {
                AgregarParametro(comando, "@IdSector", idSector);
                AgregarParametro(comando, "@IdCompra", idCompra);
                if (Convert.ToInt32(comando.ExecuteScalar()) != 0) throw new CupoPrioritarioException();
            }
            const string consulta =
                "UPDATE s SET s.CantidadReservada = s.CantidadReservada + @Cantidad " +
                "FROM Sectores s " +
                "WHERE s.ID = @IdSector " +
                "  AND s.Capacidad " +
                "    - ISNULL((SELECT SUM(e.CantidadVendida) FROM Entradas e " +
                "              WHERE e.ID_Sector = s.ID AND e.Estado = 'Confirmada'), 0) " +
                "    - s.CantidadReservada >= @Cantidad";

            using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
            {
                AgregarParametro(comando, "@Cantidad", cantidad);
                AgregarParametro(comando, "@IdSector", idSector);
                return comando.ExecuteNonQuery() == 1;
            }
        }

        // Reserva de una vez todas las entradas Pendiente de la compra, sector por sector.
        // Devuelve el ID del primer sector sin cupo, o null si todo entró.
        internal int? ReservarCompra(int idCompra, SqlConnection conexion, SqlTransaction transaccion)
        {
            foreach (KeyValuePair<int, int> par in PendientesPorSector(idCompra, conexion, transaccion))
            {
                if (!Reservar(par.Key, par.Value, idCompra, conexion, transaccion))
                {
                    return par.Key;
                }
            }
            return null;
        }

        // RN-15 + RN-32: devuelve a CantidadReservada lo que reservaron las entradas Pendiente de la
        // compra. Hay que llamarlo ANTES de cambiarles el estado a esas entradas.
        internal void LiberarCompra(int idCompra, SqlConnection conexion, SqlTransaction transaccion, bool disponible = true)
        {
            const string consulta =
                "UPDATE s SET s.CantidadReservada = s.CantidadReservada - r.Cantidad, " +
                "CupoLiberadoEl = CASE WHEN @Disponible=1 THEN GETDATE() ELSE s.CupoLiberadoEl END " +
                "FROM Sectores s " +
                "JOIN (SELECT ID_Sector, SUM(CantidadVendida) AS Cantidad FROM Entradas " +
                "      WHERE ID_Compra = @IdCompra AND Estado = 'Pendiente' GROUP BY ID_Sector) r " +
                "  ON r.ID_Sector = s.ID";

            using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
            {
                AgregarParametro(comando, "@IdCompra", idCompra);
                AgregarParametro(comando, "@Disponible", disponible);
                comando.ExecuteNonQuery();
            }
        }

        private Dictionary<int, int> PendientesPorSector(int idCompra, SqlConnection conexion, SqlTransaction transaccion)
        {
            const string consulta =
                "SELECT ID_Sector, SUM(CantidadVendida) AS Cantidad FROM Entradas " +
                "WHERE ID_Compra = @IdCompra AND Estado = 'Pendiente' GROUP BY ID_Sector";
            var cantidades = new Dictionary<int, int>();

            using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
            {
                AgregarParametro(comando, "@IdCompra", idCompra);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        cantidades[Convert.ToInt32(lector["ID_Sector"])] = Convert.ToInt32(lector["Cantidad"]);
                    }
                }
            }
            return cantidades;
        }

        // RN-11: solo cuentan las ventas. Las reservas Pendiente y las Vencida no son ventas: una
        // reserva abandonada no debe impedir eliminar el sector. Una Reembolsada sí fue una venta.
        public int ContarEntradas(int idSector)
        {
            const string consulta =
                "SELECT COUNT(*) FROM Entradas " +
                "WHERE ID_Sector = @ID AND Estado IN ('Confirmada', 'Reembolsada')";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idSector);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        // Devuelve false si no existe el sector.
        // Igual que RepositorioEventos.Eliminar: las compras abiertas con entradas en el sector se
        // vencen enteras (devolviendo el cupo que retengan en los otros sectores del evento) y se
        // borran las entradas no confirmadas del sector. Las Confirmadas no: las frena la FK.
        public bool Eliminar(int idSector)
        {
            const string consulta =
                "DELETE FROM Entradas WHERE ID_Sector = @ID AND Estado IN ('Pendiente', 'Vencida'); " +
                "DELETE FROM Sectores WHERE ID = @ID; " +
                "SELECT @@ROWCOUNT;";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    RepositorioCompras.Instance.VencerAbiertas(
                        "SELECT DISTINCT ID_Compra FROM Entradas WHERE ID_Sector = @ID AND Estado = 'Pendiente'",
                        idSector, conexion, transaccion);
                    int filas;
                    using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                    {
                        AgregarParametro(comando, "@ID", idSector);
                        filas = Convert.ToInt32(comando.ExecuteScalar());
                    }
                    transaccion.Commit();
                    return filas > 0;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }

    public sealed class CupoPrioritarioException : Exception { }

    // El cupo del sector no alcanzó para la reserva (RN-08). Negocio la traduce a la regla.
    public class CupoInsuficienteException : Exception
    {
        public int IdSector { get; }

        public CupoInsuficienteException(int idSector)
            : base("No hay cupo suficiente en el sector.")
        {
            IdSector = idSector;
        }
    }
}
