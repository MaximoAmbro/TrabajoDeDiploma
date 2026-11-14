using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // RF-10: lista de espera de los eventos agotados. UX_ListaEspera impone RN-36 en la base.
    public sealed class RepositorioListaEspera : RepositorioBase
    {
        private static RepositorioListaEspera _instance;

        public static RepositorioListaEspera Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioListaEspera();
                }
                return _instance;
            }
        }

        private RepositorioListaEspera() { }

        // RN-36: si el usuario ya está inscripto, el índice rechaza el INSERT y se lanza
        // RegistroDuplicadoException sin escribir nada.
        public int Insertar(ListaEspera inscripcion)
        {
            const string consulta =
                "INSERT INTO ListaEspera (ID_Evento, ID_Usuario, FechaAlta, ConSuscripcion) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@IdEvento, @IdUsuario, @FechaAlta, @ConSuscripcion)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdEvento", inscripcion.IdEvento);
                AgregarParametro(comando, "@IdUsuario", inscripcion.IdUsuario);
                AgregarParametro(comando, "@FechaAlta", inscripcion.FechaAlta);
                AgregarParametro(comando, "@ConSuscripcion", inscripcion.ConSuscripcion);
                try
                {
                    inscripcion.Id = Convert.ToInt32(comando.ExecuteScalar());
                    return inscripcion.Id;
                }
                catch (SqlException ex) when (EsViolacionUnicidad(ex))
                {
                    throw new RegistroDuplicadoException("El usuario ya está en la lista de espera del evento.", ex);
                }
            }
        }

        // La ventana se calcula antes de filtrar al usuario: no se altera el orden de RN-35.
        public int? Posicion(int idEvento, int idUsuario)
        {
            const string consulta = "WITH orden AS (SELECT ID_Usuario, ROW_NUMBER() OVER (ORDER BY ConSuscripcion DESC, FechaAlta, ID) AS Posicion " +
                "FROM ListaEspera WHERE ID_Evento=@Evento) SELECT Posicion FROM orden WHERE ID_Usuario=@Usuario";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Evento", idEvento);
                AgregarParametro(comando, "@Usuario", idUsuario);
                object valor = comando.ExecuteScalar();
                return valor == null || valor == DBNull.Value ? (int?)null : Convert.ToInt32(valor);
            }
        }

        // Aviso calculado al ingresar; no hay estado de lectura ni reserva automática.
        public List<string> EventosConLugar(int idUsuario)
        {
            string consulta = "SELECT e.Nombre FROM ListaEspera le JOIN Eventos e ON e.ID=le.ID_Evento " +
                "WHERE le.ID_Usuario=@Usuario AND e.Estado='Publicado' AND e.Fecha>@Ahora " +
                "AND EXISTS(SELECT 1 FROM Sectores s WHERE s.EventoID=e.ID AND s.Capacidad - s.CantidadReservada " +
                "- ISNULL((SELECT SUM(en.CantidadVendida) FROM Entradas en WHERE en.ID_Sector=s.ID AND en.Estado='Confirmada'),0)>0) " +
                "ORDER BY e.Fecha,e.ID";
            var nombres = new List<string>();
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Usuario", idUsuario);
                AgregarParametro(comando, "@Ahora", DateTime.Now);
                using (var lector = comando.ExecuteReader())
                    while (lector.Read()) nombres.Add(LeerString(lector, "Nombre"));
            }
            return nombres;
        }

        public bool Existe(int idEvento, int idUsuario)
        {
            const string consulta =
                "SELECT COUNT(*) FROM ListaEspera WHERE ID_Evento = @IdEvento AND ID_Usuario = @IdUsuario";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdEvento", idEvento);
                AgregarParametro(comando, "@IdUsuario", idUsuario);
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        // RN-35: primero los que se inscribieron con suscripción, después el resto; dentro de cada
        // grupo, por orden de inscripción. ConSuscripcion es la congelada al inscribirse.
        public List<ListaEspera> ObtenerPorEvento(int idEvento)
        {
            const string consulta =
                "SELECT le.ID, le.ID_Evento, le.ID_Usuario, le.FechaAlta, le.ConSuscripcion, " +
                "       u.Nombre, u.Apellido, u.Mail " +
                "FROM ListaEspera le JOIN Usuario u ON u.ID = le.ID_Usuario " +
                "WHERE le.ID_Evento = @IdEvento " +
                "ORDER BY le.ConSuscripcion DESC, le.FechaAlta, le.ID";
            var lista = new List<ListaEspera>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdEvento", idEvento);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        int idUsuario = Convert.ToInt32(lector["ID_Usuario"]);
                        lista.Add(new ListaEspera
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            IdEvento = Convert.ToInt32(lector["ID_Evento"]),
                            IdUsuario = idUsuario,
                            FechaAlta = Convert.ToDateTime(lector["FechaAlta"]),
                            ConSuscripcion = Convert.ToBoolean(lector["ConSuscripcion"]),
                            Usuario = new Usuario
                            {
                                Id = idUsuario,
                                Nombre = LeerString(lector, "Nombre"),
                                Apellido = LeerString(lector, "Apellido"),
                                Mail = LeerString(lector, "Mail")
                            }
                        });
                    }
                }
            }
            return lista;
        }
    }
}
