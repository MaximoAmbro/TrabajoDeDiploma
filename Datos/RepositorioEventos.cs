using System;
using System.Collections.Generic;
using System.Data;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioEventos : RepositorioBase
    {
        private const string Columnas =
            "SELECT ID, Nombre, Fecha, LocalID, Categoria, Estado, FinPreventa, ImagenPortada, MotivoRechazo FROM Eventos ";

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
                "INSERT INTO Eventos (Nombre, Fecha, LocalID, Categoria, Estado, FinPreventa, ImagenPortada) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@Nombre, @Fecha, @LocalID, @Categoria, @Estado, @FinPreventa, @ImagenPortada)";

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
                        AgregarParametro(comando, "@FinPreventa", evento.FinPreventa);
                        comando.Parameters.Add("@ImagenPortada", SqlDbType.VarBinary, -1).Value = (object)evento.ImagenPortada ?? DBNull.Value;
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

        public List<Evento> ObtenerPorEstado(string estado)
        {
            return Consultar(Columnas + "WHERE Estado = @Valor ORDER BY Fecha,ID", estado);
        }

        public bool OrganizadorYaTuvoEventoAprobado(int idOrganizador)
        {
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion,
                "SELECT CASE WHEN EXISTS(SELECT 1 FROM Propietario WHERE ID=@ID AND PrimerEventoAprobado=1) THEN 1 ELSE 0 END"))
            {
                AgregarParametro(comando, "@ID", idOrganizador);
                return Convert.ToInt32(comando.ExecuteScalar()) == 1;
            }
        }

        public bool AprobarPrimerEvento(int idEvento)
        {
            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int cambiado;
                    using (SqlCommand comando = CrearComando(conexion,
                        "UPDATE dbo.Eventos SET Estado='Publicado' WHERE ID=@ID AND Estado='EnRevision'", transaccion))
                    {
                        AgregarParametro(comando, "@ID", idEvento);
                        cambiado = comando.ExecuteNonQuery();
                    }
                    if (cambiado != 1) { transaccion.Rollback(); return false; }
                    using (SqlCommand comando = CrearComando(conexion,
                        "UPDATE dbo.Propietario SET PrimerEventoAprobado=1 WHERE ID=(SELECT l.PropietarioID FROM dbo.Eventos e JOIN dbo.Locales l ON l.ID=e.LocalID WHERE e.ID=@ID)", transaccion))
                    {
                        AgregarParametro(comando, "@ID", idEvento);
                        if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("El evento no tiene un organizador válido.");
                    }
                    transaccion.Commit();
                    return true;
                }
                catch { transaccion.Rollback(); throw; }
            }
        }

        public bool RechazarPrimerEvento(int idEvento, string motivo)
        {
            const string sql = "UPDATE dbo.Eventos SET Estado='Borrador', MotivoRechazo=@Motivo WHERE ID=@ID AND Estado='EnRevision'";
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@Motivo", motivo);
                AgregarParametro(comando, "@ID", idEvento);
                return comando.ExecuteNonQuery() == 1;
            }
        }

        public List<Evento> ConIngresoAbierto(int idPropietario, DateTime desde, DateTime hasta)
        {
            return Consultar(Columnas + "WHERE Estado='Publicado' AND Fecha>=@Valor AND Fecha<=@Hasta " +
                "AND LocalID IN(SELECT ID FROM Locales WHERE PropietarioID=@Propietario) ORDER BY Fecha,ID", desde,
                comando => { AgregarParametro(comando, "@Hasta", hasta); AgregarParametro(comando, "@Propietario", idPropietario); });
        }

        public List<Evento> DelDia(DateTime dia)
        {
            return Consultar(Columnas + "WHERE Fecha >= @Valor AND Fecha < @Hasta AND Estado = 'Publicado' ORDER BY Fecha",
                dia.Date, comando => AgregarParametro(comando, "@Hasta", dia.Date.AddDays(1)));
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
                "SELECT e.ID, e.Nombre, e.Fecha, e.LocalID, e.Categoria, e.Estado, e.FinPreventa, e.ImagenPortada, e.MotivoRechazo " +
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
                AgregarParametro(comando, "@Texto", texto == null ? null :
                    texto.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]"));
                AgregarParametro(comando, "@Categoria", categoria);
                AgregarParametro(comando, "@Desde", desde);
            });
        }

        // RN-11: solo cuentan las ventas. Las reservas Pendiente y las Vencida no son ventas: una
        // reserva abandonada no debe impedir eliminar el evento. Una Reembolsada sí fue una venta
        // (RN-29): su compra, su pago y su póliza quedan como rastro y la frenan las claves foráneas.
        public int ContarEntradas(int idEvento)
        {
            const string consulta =
                "SELECT COUNT(*) FROM Entradas " +
                "WHERE ID_Evento = @ID AND Estado IN ('Confirmada', 'Reembolsada')";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idEvento);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        // RN-10: agotado cuando ningún sector tiene capacidad sin vender. Cuentan solo las entradas
        // Confirmada: una reserva de RN-15 todavía puede liberarse y no agota el evento.
        // Un evento sin sectores no está agotado: no tiene nada que vender.
        public bool EstaAgotado(int idEvento)
        {
            string consulta = "SELECT CASE WHEN " + CondicionAgotado("@ID") + " THEN 1 ELSE 0 END";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idEvento);
                return Convert.ToInt32(comando.ExecuteScalar()) == 1;
            }
        }

        // RN-10 como condición SQL sobre el evento idEvento (un parámetro o una columna, p. ej.
        // "e.ID"). Única definición del criterio: la usan EstaAgotado y el reporte RPT-05.
        // Solo expresiones SQL constantes del código ("@ID", "e.ID"). Nunca valores del usuario.
        internal static string CondicionAgotado(string idEvento)
        {
            return "(EXISTS (SELECT 1 FROM Sectores WHERE EventoID = " + idEvento + ") " +
                   " AND NOT EXISTS (SELECT 1 FROM Sectores sa " +
                   "                 WHERE sa.EventoID = " + idEvento +
                   "                   AND sa.Capacidad - ISNULL((SELECT SUM(ea.CantidadVendida) FROM Entradas ea " +
                   "                                              WHERE ea.ID_Sector = sa.ID AND ea.Estado = 'Confirmada'), 0) > 0))";
        }

        // Borra el evento junto con lo que cuelga de él. Las entradas no confirmadas (reservas
        // Pendiente o Vencida) se borran: antes, sus compras abiertas pasan a Vencida, así un pago en
        // curso no puede confirmarse sin entradas. Compras y Pagos quedan como rastro (RN-31).
        // Las Confirmadas no se borran: Negocio verifica antes que no haya (RN-11) y, si se colara
        // alguna, FK_Entradas_Sectores aborta la transacción. Devuelve false si no existe el evento.
        public bool Eliminar(int idEvento)
        {
            const string consulta =
                "DELETE FROM Entradas WHERE ID_Evento = @ID AND Estado IN ('Pendiente', 'Vencida'); " +
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
                    RepositorioCompras.Instance.VencerAbiertas(
                        "SELECT DISTINCT ID_Compra FROM Entradas WHERE ID_Evento = @ID AND Estado = 'Pendiente'",
                        idEvento, conexion, transaccion);
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

        // RN-39: null saca la preventa. Devuelve false si no existe el evento.
        public bool ActualizarPreventa(int idEvento, DateTime? finPreventa)
        {
            const string consulta = "UPDATE Eventos SET FinPreventa = @FinPreventa WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@FinPreventa", finPreventa);
                AgregarParametro(comando, "@ID", idEvento);
                return comando.ExecuteNonQuery() > 0;
            }
        }

        public bool ActualizarImagenPortada(int idEvento, byte[] imagen)
        {
            const string consulta = "UPDATE dbo.Eventos SET ImagenPortada=@Imagen WHERE ID=@ID";
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                comando.Parameters.Add("@Imagen", SqlDbType.VarBinary, -1).Value = (object)imagen ?? DBNull.Value;
                AgregarParametro(comando, "@ID", idEvento);
                return comando.ExecuteNonQuery() == 1;
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

        public bool ActualizarEstadoSiEs(int idEvento, string estadoActual, string estadoNuevo)
        {
            const string sql = "UPDATE Eventos SET Estado=@Nuevo WHERE ID=@ID AND Estado=@Actual";
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@Nuevo", estadoNuevo);
                AgregarParametro(comando, "@ID", idEvento);
                AgregarParametro(comando, "@Actual", estadoActual);
                return comando.ExecuteNonQuery() == 1;
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
                            Estado = LeerString(lector, "Estado"),
                            FinPreventa = LeerFechaNula(lector, "FinPreventa"),
                            ImagenPortada = lector.IsDBNull(7) ? null : (byte[])lector["ImagenPortada"],
                            MotivoRechazo = LeerString(lector, "MotivoRechazo")
                        });
                    }
                }
            }
            return eventos;
        }
    }
}
