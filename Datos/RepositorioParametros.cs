using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioParametros : RepositorioBase
    {
        private static RepositorioParametros _instance;

        public static RepositorioParametros Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioParametros();
                }
                return _instance;
            }
        }

        private RepositorioParametros() { }

        public List<Parametro> ObtenerTodos()
        {
            const string consulta = "SELECT Clave, Valor, Descripcion, Regla FROM Parametros";
            var parametros = new List<Parametro>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    parametros.Add(new Parametro
                    {
                        Clave = LeerString(lector, "Clave"),
                        Valor = LeerString(lector, "Valor"),
                        Descripcion = LeerString(lector, "Descripcion"),
                        Regla = LeerString(lector, "Regla")
                    });
                }
            }
            return parametros;
        }
    }
}
