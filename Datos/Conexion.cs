using System.Configuration;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Datos
{
    internal sealed class Conexion
    {
        // Nombre con el que Visual/App.config declara la cadena de conexión.
        private const string NombreCadena = "Visual.Properties.Settings.BddEventAura";

        private static Conexion _instance;

        public static Conexion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Conexion();
                }
                return _instance;
            }
        }

        public string Cadena { get; }

        private Conexion()
        {
            var config = ConfigurationManager.ConnectionStrings[NombreCadena];
            if (config == null)
            {
                throw new ConfigurationErrorsException(
                    $"No se encontró la cadena de conexión '{NombreCadena}' en App.config.");
            }
            Cadena = config.ConnectionString;
        }

        public SqlConnection Crear()
        {
            var parametros = new DbConnectionStringBuilder { ConnectionString = Cadena };
            parametros["Connect Timeout"] = 3;
            return new SqlConnection(parametros.ConnectionString);
        }
    }
}
