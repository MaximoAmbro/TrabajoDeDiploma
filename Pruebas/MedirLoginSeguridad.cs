using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Datos;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class MedirLoginSeguridad
    {
        // CP-105 — RF-02, SEG-6: mide autenticaciones reales sin alterar las iteraciones de producción.
        [DataTestMethod]
        [DataRow(600000, DisplayName = "CP-105: Medición del login PBKDF2 con 600000 iteraciones")]
        [DataRow(310000, DisplayName = "CP-105: Medición del login PBKDF2 con 310000 iteraciones")]
        public void LoginConIteraciones(int iteraciones)
        {
            Fake.Limpiar();
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            MethodInfo derivar = typeof(Seguridad).GetMethod("Derivar", BindingFlags.Static | BindingFlags.NonPublic);
            byte[] hash = (byte[])derivar.Invoke(null, new object[] { "Cliente123", salt, iteraciones, 32 });
            string almacenado = "PBKDF2$" + iteraciones + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            RepositorioUsuarios.Instance.ActualizarContrasena(cliente.Mail, almacenado);
            var tiempos = new double[5];
            try
            {
                for (int i = 0; i < tiempos.Length; i++)
                {
                    GestorSesion.Instance.Cerrar();
                    var reloj = Stopwatch.StartNew();
                    Usuario usuario = GestorUsuarios.Instance.Autenticar(cliente.Mail, "Cliente123");
                    reloj.Stop();
                    Assert.IsNotNull(usuario);
                    Assert.AreEqual(almacenado, RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña,
                        "El login no debe degradar un hash más fuerte ni recalcular uno ya vigente.");
                    tiempos[i] = reloj.Elapsed.TotalMilliseconds;
                }
                string ruta = System.IO.Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "seg6-medicion-login-" + iteraciones + ".txt");
                System.IO.File.WriteAllText(ruta, iteraciones + " iteraciones; login real (ms): " +
                    string.Join(", ", tiempos.Select(t => t.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))) +
                    Environment.NewLine + "Promedio (ms): " + tiempos.Average().ToString("F2", System.Globalization.CultureInfo.InvariantCulture) +
                    Environment.NewLine + "Maximo (ms): " + tiempos.Max().ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
            }
            finally { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }
        }

        // CP-109 — RF-11, SEG-6: preparación completa real, con la guarda _Pruebas de BaseDePrueba.
        [TestMethod("CP-109: Preparación completa con 1178 clientes y hashes a 310000")]
        public void PreparacionCompleta()
        {
            GestorSesion.Instance.Cerrar();
            var reloj = Stopwatch.StartNew();
            BaseDePrueba.Preparar(null);
            double baseMs = reloj.Elapsed.TotalMilliseconds;
            try
            {
                BaseDePrueba.CargarDatosReporte();
                reloj.Stop();
                double totalMs = reloj.Elapsed.TotalMilliseconds;
                Assert.AreEqual(1179, Fake.ContarDatosReporte(), "Un local y 1178 clientes de reporte.");
                using (var conexion = new Microsoft.Data.SqlClient.SqlConnection(BaseDePrueba.CadenaDePrueba()))
                using (var comando = conexion.CreateCommand())
                {
                    conexion.Open();
                    comando.CommandText = "SELECT COUNT(*) FROM Usuario WHERE Mail LIKE 'rpt%@datos.eventaura.com' AND Contraseña LIKE 'PBKDF2$310000$%'";
                    Assert.AreEqual(1178, Convert.ToInt32(comando.ExecuteScalar()));
                    comando.CommandText = "SELECT COUNT(*) FROM Usuario WHERE Mail IN ('admin@eventaura.com', 'bronce@eventaura.com', 'plata@eventaura.com', 'oro@eventaura.com') AND Contraseña LIKE 'PBKDF2$310000$%'";
                    Assert.AreEqual(4, Convert.ToInt32(comando.ExecuteScalar()));
                }
                string ruta = System.IO.Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "seg6-medicion-semilla.txt");
                System.IO.File.WriteAllText(ruta,
                    "Recrear base + esquema + semilla (ms): " + baseMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) +
                    Environment.NewLine + "Datos de reporte, 1178 clientes (ms): " + (totalMs - baseMs).ToString("F2", System.Globalization.CultureInfo.InvariantCulture) +
                    Environment.NewLine + "Preparacion completa (ms): " + totalMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) +
                    Environment.NewLine + "Verificado: 1178 clientes de reporte y 4 cuentas de semilla con PBKDF2$310000$. Sin hashing por fila durante la carga SQL.");
            }
            finally { GestorSesion.Instance.Cerrar(); Fake.LimpiarDatosReporte(); }
        }
    }
}
