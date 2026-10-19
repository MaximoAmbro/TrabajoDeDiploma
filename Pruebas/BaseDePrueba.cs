using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

// Comparten una base y los singletons de Negocio (Conexion, GestorParametros, GestorSesion):
// dos pruebas a la vez se corrompen entre sí.
[assembly: DoNotParallelize]

namespace Pruebas
{
    // Las pruebas son de INTEGRACIÓN contra SQL Server: GestorParametros lee de la base y no hay
    // punto de inyección. Antes de todas, se borra BddEventAura_Pruebas, se crea de nuevo y se le
    // aplican 01-esquema.sql y 02-semilla.sql. Queda creada al terminar, para poder mirarla si
    // algo falló; la próxima corrida la vuelve a borrar.
    [TestClass]
    public class BaseDePrueba
    {
        // La misma clave que lee Datos/Conexion.cs.
        public const string NombreCadena = "Visual.Properties.Settings.BddEventAura";
        public const string SufijoObligatorio = "_Pruebas";

        // Una línea que, sin espacios, es exactamente GO. Nunca un Split sobre "GO": la semilla
        // tiene «Agotado», y un corte ingenuo deja la base a medio crear sin ningún error.
        private static readonly Regex SeparadorGo =
            new Regex(@"^[ \t]*GO[ \t]*\r?$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static readonly Regex NombreValido = new Regex(@"^[A-Za-z0-9_]+$");

        [AssemblyInitialize]
        public static void Preparar(TestContext contexto)
        {
            string cadena = CadenaDePrueba();
            string nombre = new SqlConnectionStringBuilder(cadena).InitialCatalog;

            // La guarda va ANTES de tocar nada: nunca se borra la base de desarrollo.
            if (!nombre.EndsWith(SufijoObligatorio, StringComparison.OrdinalIgnoreCase) || !NombreValido.IsMatch(nombre))
            {
                throw new InvalidOperationException(
                    $"Las pruebas se niegan a correr: el App.config de Pruebas apunta a «{nombre}». " +
                    $"La base de pruebas tiene que terminar en «{SufijoObligatorio}», porque se borra y se " +
                    "crea de nuevo en cada corrida. No se tocó ninguna base.");
            }

            RecrearBase(cadena, nombre);
            string carpeta = Path.Combine(RaizDelRepositorio(), "BaseDatos");
            EjecutarScript(cadena, Path.Combine(carpeta, "01-esquema.sql"));
            EjecutarScript(cadena, Path.Combine(carpeta, "02-semilla.sql"));

            // GestorParametros cachea: que lea la tabla recién poblada.
            GestorParametros.Instance.Recargar();
        }

        [AssemblyCleanup]
        public static void Terminar()
        {
            // Suelta las conexiones del pool: la próxima corrida puede borrar la base sin esperar.
            SqlConnection.ClearAllPools();
        }

        public static string CadenaDePrueba()
        {
            ConnectionStringSettings config = ConfigurationManager.ConnectionStrings[NombreCadena];
            if (config == null)
            {
                throw new InvalidOperationException($"Falta la cadena «{NombreCadena}» en el App.config de Pruebas.");
            }
            return config.ConnectionString;
        }

        // Sube desde la carpeta del binario hasta la que contiene SistemaEntradas.sln: no depende
        // de la configuración ni del framework con que se compiló.
        public static string RaizDelRepositorio()
        {
            var carpeta = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (carpeta != null && !File.Exists(Path.Combine(carpeta.FullName, "SistemaEntradas.sln")))
            {
                carpeta = carpeta.Parent;
            }
            if (carpeta == null)
            {
                throw new InvalidOperationException(
                    "No se encontró SistemaEntradas.sln subiendo desde " + AppDomain.CurrentDomain.BaseDirectory);
            }
            return carpeta.FullName;
        }

        // Desde master: la cadena de Conexion apunta a la base que se va a borrar.
        private static void RecrearBase(string cadena, string nombre)
        {
            var master = new SqlConnectionStringBuilder(cadena) { InitialCatalog = "master" };

            // Una conexión del pool de una corrida anterior bloquearía el DROP.
            SqlConnection.ClearAllPools();

            const string consulta =
                "DECLARE @Base SYSNAME = @Nombre; " +
                "DECLARE @Sql NVARCHAR(MAX); " +
                "IF DB_ID(@Base) IS NOT NULL " +
                "BEGIN " +
                "    SET @Sql = N'ALTER DATABASE ' + QUOTENAME(@Base) + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; ' + " +
                "               N'DROP DATABASE ' + QUOTENAME(@Base) + N';'; " +
                "    EXEC (@Sql); " +
                "END; " +
                "SET @Sql = N'CREATE DATABASE ' + QUOTENAME(@Base) + N';'; " +
                "EXEC (@Sql);";

            using (var conexion = new SqlConnection(master.ConnectionString))
            using (var comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@Nombre", nombre);
                comando.CommandTimeout = 120;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static void EjecutarScript(string cadena, string ruta)
        {
            string texto = File.ReadAllText(ruta);
            using (var conexion = new SqlConnection(cadena))
            {
                conexion.Open();
                foreach (string lote in SeparadorGo.Split(texto))
                {
                    if (string.IsNullOrWhiteSpace(lote))
                    {
                        continue;
                    }
                    using (var comando = new SqlCommand(lote, conexion))
                    {
                        comando.CommandTimeout = 120;
                        comando.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
