using System;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioUsuarios : RepositorioBase
    {
        private static RepositorioUsuarios _instance;

        public static RepositorioUsuarios Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioUsuarios();
                }
                return _instance;
            }
        }

        private RepositorioUsuarios() { }

        // Devuelve null si no existe ningún usuario con ese mail.
        public Usuario ObtenerPorMail(string mail)
        {
            const string consulta =
                "SELECT ID, Nombre, Apellido, Mail, Contraseña, TipoUsuario, IntentosFallidos, BloqueadoHasta " +
                "FROM Usuario WHERE Mail = @Mail";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Mail", mail);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return null;
                    }
                    return new Usuario
                    {
                        Id = Convert.ToInt32(lector["ID"]),
                        Nombre = LeerString(lector, "Nombre"),
                        Apellido = LeerString(lector, "Apellido"),
                        Mail = LeerString(lector, "Mail"),
                        Contraseña = LeerString(lector, "Contraseña"),
                        TipoUsuario = LeerString(lector, "TipoUsuario"),
                        IntentosFallidos = Convert.ToInt32(lector["IntentosFallidos"]),
                        BloqueadoHasta = LeerFechaNula(lector, "BloqueadoHasta")
                    };
                }
            }
        }

        // Nivel persistido en Usuario.Nivel (RN-22: se lee, no se recalcula). Bronce si no existe.
        public NivelFidelidad ObtenerNivel(int idUsuario)
        {
            const string consulta = "SELECT Nivel FROM Usuario WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idUsuario);
                object valor = comando.ExecuteScalar();
                NivelFidelidad nivel;
                return valor != null && Enum.TryParse(valor.ToString(), out nivel) ? nivel : NivelFidelidad.Bronce;
            }
        }

        public bool ExisteMail(string mail)
        {
            const string consulta = "SELECT COUNT(*) FROM Usuario WHERE Mail = @Mail";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Mail", mail);
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        // RN-05: inserta en Usuario y en Cliente o Propietario dentro de una misma transacción.
        // Lanza RegistroDuplicadoException si el mail ya existe (UQ_Usuario_Mail, RN-01).
        public int Insertar(Usuario usuario, NivelFidelidad nivel)
        {
            const string insertarUsuario =
                "INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@Nombre, @Apellido, @Mail, @Contraseña, @TipoUsuario, @Nivel)";
            const string insertarCliente = "INSERT INTO Cliente (ID) VALUES (@ID)";
            const string insertarPropietario = "INSERT INTO Propietario (ID) VALUES (@ID)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int id;
                    using (SqlCommand comando = CrearComando(conexion, insertarUsuario, transaccion))
                    {
                        AgregarParametro(comando, "@Nombre", usuario.Nombre);
                        AgregarParametro(comando, "@Apellido", usuario.Apellido);
                        AgregarParametro(comando, "@Mail", usuario.Mail);
                        AgregarParametro(comando, "@Contraseña", usuario.Contraseña);
                        AgregarParametro(comando, "@TipoUsuario", usuario.TipoUsuario);
                        AgregarParametro(comando, "@Nivel", nivel.ToString());
                        id = Convert.ToInt32(comando.ExecuteScalar());
                    }

                    // Negocio ya validó que TipoUsuario sea Cliente o Propietario.
                    string insertarPerfil = usuario.TipoUsuario == "Propietario"
                        ? insertarPropietario
                        : insertarCliente;
                    using (SqlCommand comando = CrearComando(conexion, insertarPerfil, transaccion))
                    {
                        AgregarParametro(comando, "@ID", id);
                        comando.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    return id;
                }
                catch (SqlException ex)
                {
                    transaccion.Rollback();
                    if (EsViolacionUnicidad(ex))
                    {
                        throw new RegistroDuplicadoException("Ya existe un usuario con ese mail.", ex);
                    }
                    throw;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // Devuelve false si no existe ningún usuario con ese mail.
        public bool ActualizarContrasena(string mail, string contrasena)
        {
            const string consulta = "UPDATE Usuario SET Contraseña = @Contraseña WHERE Mail = @Mail";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Contraseña", contrasena);
                AgregarParametro(comando, "@Mail", mail);
                return comando.ExecuteNonQuery() > 0;
            }
        }

        // RN-04
        public void ActualizarIntentos(int idUsuario, int intentosFallidos, DateTime? bloqueadoHasta)
        {
            const string consulta =
                "UPDATE Usuario SET IntentosFallidos = @Intentos, BloqueadoHasta = @BloqueadoHasta WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Intentos", intentosFallidos);
                AgregarParametro(comando, "@BloqueadoHasta", bloqueadoHasta);
                AgregarParametro(comando, "@ID", idUsuario);
                comando.ExecuteNonQuery();
            }
        }
    }
}
