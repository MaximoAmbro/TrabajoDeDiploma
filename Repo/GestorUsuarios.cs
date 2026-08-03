using Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Text;
namespace Repositorio
{
    public partial class GestorUsuarios
    {
        private readonly SqlConnection _conexion;

        // Constructor recibe la conexión abierta o cerrada
        public GestorUsuarios(SqlConnection conexion)
        {
            _conexion = conexion;
        }
    } // llamamos a la cadena de conexion
    public partial class GestorUsuarios
    {
        public int ObtenerIDUsuario(string mail, string contraseña, out int IdUsuario)
        {
            string consulta = "SELECT ID FROM Usuario WHERE Mail = @Mail AND Contraseña = @Contraseña";

            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@Mail", mail);
                comando.Parameters.AddWithValue("@Contraseña", contraseña);

                _conexion.Open();
                object resultado = comando.ExecuteScalar();
                _conexion.Close();

                if (resultado != null)
                {
                    IdUsuario = Convert.ToInt32(resultado);
                    return Convert.ToInt32(resultado);
                }
            }
            IdUsuario = 0;
            return 0;
        }
        public string ObtenerTipoUsuario(string mail, string contraseña, out string tipoUsuario)
        {
            string consulta = "SELECT TipoUsuario FROM Usuario WHERE Mail = @Mail AND Contraseña = @Contraseña";

            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@Mail", mail);
                comando.Parameters.AddWithValue("@Contraseña", contraseña);

                _conexion.Open();
                object resultado = comando.ExecuteScalar();
                _conexion.Close();
                tipoUsuario = resultado.ToString();
                return resultado.ToString();
            }
        }
        public string ObtenerNombreUsuario(string mail, string contraseña, out string nombreUsuario)
        {
            string consulta = "SELECT Nombre FROM Usuario WHERE Mail = @Mail AND Contraseña = @Contraseña";

            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@Mail", mail);
                comando.Parameters.AddWithValue("@Contraseña", contraseña);

                _conexion.Open();
                object resultado = comando.ExecuteScalar();
                _conexion.Close();
                nombreUsuario = resultado.ToString();
                return resultado?.ToString();
            }
        }
        public bool ValidarCredenciales(string mail, string contraseña)
        {
            bool esValido = false;
            string consulta = "SELECT COUNT(*) FROM Usuario WHERE Mail = @Mail AND Contraseña = @Contraseña";

            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@Mail", mail);
                comando.Parameters.AddWithValue("@Contraseña", contraseña);

                _conexion.Open();
                int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                if (cantidad > 0)
                { esValido = true; }
                else
                { esValido = false; }
                _conexion.Close();
            }
            return esValido;
        }
        public void AgregarUsuario (string nombre, string apellido, string mail, string contraseña, string tipo, out string Mensaje)
        {
             if (ValidarUsuario(contraseña, mail, out Mensaje) == true) 
            {
                string consulta = "INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario) VALUES (@Nombre, @Apellido, @Mail, @Contraseña, @TipoUsuario)";
                using (SqlCommand comando = new SqlCommand(consulta, _conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", nombre);
                    comando.Parameters.AddWithValue("@Apellido", apellido);
                    comando.Parameters.AddWithValue("@Mail", mail);
                    comando.Parameters.AddWithValue("@Contraseña", contraseña);
                    comando.Parameters.AddWithValue("@TipoUsuario", tipo);
                    _conexion.Open();
                    comando.ExecuteNonQuery();
                    _conexion.Close();
                }
            }
            else { return; }
        }
        public bool ValidarUsuario(string contraseña, string mail, out string Mensaje)
            {
                if (contraseña == mail)
                {
                    Mensaje = "La contraseña no puede ser igual al mail.";;
                    return false;
            }
                string consulta = "SELECT COUNT(*) FROM Usuario WHERE Mail = @Mail";
                using (SqlCommand comando = new SqlCommand(consulta, _conexion))
                {
                    comando.Parameters.AddWithValue("@Mail", mail);
                    _conexion.Open();
                    int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    _conexion.Close();
                    if (cantidad > 0)
                    {
                        Mensaje = "El mail ya existe.";
                        return false;
                    }
                }
                Mensaje = "Usuario creado con exito.";
                return true;
            }
        public void CambiarContraseña (string MailUsuario, string nuevaContrasena)
        {
            string consulta = "UPDATE Usuario SET Contraseña = @NuevaContrasena WHERE Mail = @MailUsuario";
            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@NuevaContrasena", nuevaContrasena);
                comando.Parameters.AddWithValue("@MailUsuario", MailUsuario);
                _conexion.Open();
                comando.ExecuteNonQuery();
                _conexion.Close();
            }
        }
    }// Aca son los metodos para devolver cosas
}
