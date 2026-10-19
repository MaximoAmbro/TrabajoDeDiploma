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
