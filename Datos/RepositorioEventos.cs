using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioEventos : RepositorioBase
    {
        private const string Columnas = "SELECT ID, Nombre, Fecha, LocalID, Categoria, Estado FROM Eventos ";

        private static RepositorioEventos _instance;

        public static RepositorioEventos Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioEventos();
                }
                return _instance;
            }
        }

        private RepositorioEventos() { }

        // Inserta el evento y sus sectores en una misma transacción: o se publica completo o no se publica.
        // Asigna el Id generado al evento y a cada sector.
        public int Insertar(Evento evento)
        {
            const string consulta =
                "INSERT INTO Eventos (Nombre, Fecha, LocalID, Categoria, Estado) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@Nombre, @Fecha, @LocalID, @Categoria, @Estado)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                    {
                        AgregarParametro(comando, "@Nombre", evento.Nombre);
                        AgregarParametro(comando, "@Fecha", evento.Fecha);
                        AgregarParametro(comando, "@LocalID", evento.IdLocal);
                        AgregarParametro(comando, "@Categoria", evento.Categoria);
                        AgregarParametro(comando, "@Estado", evento.Estado);
                        evento.Id = Convert.ToInt32(comando.ExecuteScalar());
                    }

                    foreach (Sector sector in evento.Sectores)
                    {
                        sector.IdEvento = evento.Id;
                        sector.Id = RepositorioSectores.Instance.Insertar(sector, conexion, transaccion);
                    }

                    transaccion.Commit();
                    return evento.Id;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // Devuelve null si no existe.
        public Evento ObtenerPorId(int idEvento)
        {
            List<Evento> eventos = Consultar(Columnas + "WHERE ID = @Valor", idEvento);
            return eventos.Count == 0 ? null : eventos[0];
        }

        public List<Evento> ObtenerPorLocal(int idLocal)
        {
            return Consultar(Columnas + "WHERE LocalID = @Valor ORDER BY Fecha", idLocal);
        }

        public List<Evento> ObtenerPorEstadoPosterioresA(string estado, DateTime fecha)
        {
            return Consultar(Columnas + "WHERE Estado = @Estado AND Fecha > @Valor ORDER BY Fecha", fecha,
                comando => AgregarParametro(comando, "@Estado", estado));
        }

        // RF-03: eventos en el estado indicado, con fecha posterior a "ahora" y que cumplan los
        // filtros no nulos. El texto se busca en el nombre del evento y en el nombre y la
        // dirección del local.
        public List<Evento> Buscar(string estado, DateTime ahora, string texto, string categoria, DateTime? desde)
        {
            const string consulta =
                "SELECT e.ID, e.Nombre, e.Fecha, e.LocalID, e.Categoria, e.Estado " +
                "FROM Eventos e JOIN Locales l ON l.ID = e.LocalID " +
                "WHERE e.Estado = @Estado AND e.Fecha > @Valor " +
                "  AND (@Texto IS NULL OR e.Nombre LIKE '%' + @Texto + '%' " +
                "       OR l.Nombre LIKE '%' + @Texto + '%' OR l.Direccion LIKE '%' + @Texto + '%') " +
                "  AND (@Categoria IS NULL OR e.Categoria = @Categoria) " +
                "  AND (@Desde IS NULL OR e.Fecha >= @Desde) " +
                "ORDER BY e.Fecha";

            return Consultar(consulta, ahora, comando =>
            {
                AgregarParametro(comando, "@Estado", estado);
                AgregarParametro(comando, "@Texto", texto);
                AgregarParametro(comando, "@Categoria", categoria);
                AgregarParametro(comando, "@Desde", desde);
            });
        }

        public int ContarEntradas(int idEvento)
        {
            const string consulta = "SELECT COUNT(*) FROM Entradas WHERE ID_Evento = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idEvento);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        // Borra el evento junto con lo que cuelga de él. No borra entradas: Negocio verifica
        // antes que no haya (RN-11) y, si se colara alguna, FK_Entradas_Sectores aborta la transacción.
        // Devuelve false si no existe el evento.
        public bool Eliminar(int idEvento)
        {
            const string consulta =
                "DELETE FROM ServiciosEvento WHERE ID_Evento = @ID; " +
                "DELETE FROM ListaEspera WHERE ID_Evento = @ID; " +
                "DELETE FROM Sectores WHERE EventoID = @ID; " +
                "DELETE FROM Eventos WHERE ID = @ID; " +
                "SELECT @@ROWCOUNT;";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int filas;
                    using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                    {
                        AgregarParametro(comando, "@ID", idEvento);
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

        // Devuelve false si no existe el evento.
        public bool ActualizarEstado(int idEvento, string estado)
        {
            const string consulta = "UPDATE Eventos SET Estado = @Estado WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Estado", estado);
                AgregarParametro(comando, "@ID", idEvento);
                return comando.ExecuteNonQuery() > 0;
            }
        }

        private List<Evento> Consultar(string consulta, object valor, Action<SqlCommand> otrosParametros = null)
        {
            var eventos = new List<Evento>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Valor", valor);
                otrosParametros?.Invoke(comando);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        DateTime fecha = LeerFechaNula(lector, "Fecha") ?? DateTime.MinValue;
                        eventos.Add(new Evento
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            Nombre = LeerString(lector, "Nombre"),
                            Fecha = fecha,
                            Hora = fecha.TimeOfDay,
                            IdLocal = Convert.ToInt32(lector["LocalID"]),
                            Categoria = LeerString(lector, "Categoria"),
                            Estado = LeerString(lector, "Estado")
                        });
                    }
                }
            }
            return eventos;
        }
    }
}
