using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasArranqueYAlcance
    {
        // CP-209 — RF-01, RNF-03: comprobar que el ejecutable real completa el arranque.
        [TestMethod("CP-209: El ejecutable Visual.exe arranca y permanece abierto")]
        [TestCategory("Smoke")]
        public void CP209_EjecutableArranca()
        {
            string raiz = RaizRepositorio();
            string exe = Path.Combine(raiz, "Visual", "bin", "Debug", "Visual.exe");
            Assert.IsTrue(File.Exists(exe), "No se encontró el ejecutable compilado: " + exe);
            CerrarVisualesPrevios(exe);

            Process proceso = null;
            try
            {
                proceso = Process.Start(new ProcessStartInfo(exe) { WorkingDirectory = Path.GetDirectoryName(exe), UseShellExecute = true });
                Assert.IsNotNull(proceso, "Windows no inició Visual.exe.");
                bool termino = proceso.WaitForExit(5000);
                Assert.IsFalse(termino, termino
                    ? "Visual.exe terminó durante los primeros cinco segundos (código " + proceso.ExitCode + ")."
                    : "Visual.exe debe seguir abierto después del arranque.");
            }
            finally
            {
                if (proceso != null)
                {
                    try
                    {
                        if (!proceso.HasExited && !proceso.CloseMainWindow()) proceso.Kill();
                        if (!proceso.HasExited && !proceso.WaitForExit(2500)) proceso.Kill();
                    }
                    catch (InvalidOperationException) { }
                    proceso.Dispose();
                }
            }
        }

        // CP-210 — RF-01/RF-22, RNF-03: comprobar referencias de navegación y salida de formularios.
        [TestMethod("CP-210: Las pantallas tienen una ruta de entrada y una salida visible")]
        [TestCategory("Experiencia")]
        public void CP210_RutasYSalidasDePantallas()
        {
            string raizVisual = Path.Combine(RaizRepositorio(), "Visual");
            var fuentes = Directory.GetFiles(raizVisual, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            string codigo = string.Join("\n", fuentes.Select(File.ReadAllText));
            StringAssert.Contains(codigo, "Application.Run(new frmBienvenida())");
            StringAssert.Contains(codigo, "Cargar(\"bienvenida.html\")");
            StringAssert.Contains(codigo, "Cargar(\"acceso.html\")");
            var pantallas = Regex.Matches(codigo, @"\bclass\s+(\w+)\s*:\s*FormularioEventAura\b")
                .Cast<Match>().Select(m => m.Groups[1].Value).Distinct().ToArray();
            Assert.IsTrue(pantallas.Length >= 25, "El inventario esperaba al menos 25 formularios y encontró " + pantallas.Length + ".");

            var sinRuta = pantallas.Where(nombre => !Regex.IsMatch(codigo,
                @"\bnew\s+(?:Visual\.)?(?:Propietario\.)?(?:Cliente\.)?" + Regex.Escape(nombre) + @"\s*(?:\(|\{)", RegexOptions.IgnoreCase)).ToArray();
            Assert.AreEqual(0, sinRuta.Length, "Formularios sin referencia de navegación desde Visual/: " + string.Join(", ", sinRuta));

            var sinSalida = pantallas.Where(nombre =>
            {
                string ruta = fuentes.FirstOrDefault(p => Path.GetFileName(p).Equals(nombre + ".cs", StringComparison.OrdinalIgnoreCase));
                string fuente = ruta == null ? string.Empty : File.ReadAllText(ruta);
                string designerPath = Path.Combine(Path.GetDirectoryName(ruta ?? string.Empty), nombre + ".Designer.cs");
                string designer = File.Exists(designerPath) ? File.ReadAllText(designerPath) : string.Empty;
                bool controlBoxDeshabilitado = Regex.IsMatch(fuente + designer, @"ControlBox\s*=\s*false", RegexOptions.IgnoreCase);
                bool botonSalida = Regex.IsMatch(fuente + designer,
                    @"Text\s*=\s*""[^""]*(Volver|Cerrar|Cancelar|Salir|Atrás)[^""]*""", RegexOptions.IgnoreCase);
                return controlBoxDeshabilitado && !botonSalida;
            }).ToArray();
            Assert.AreEqual(0, sinSalida.Length, "Formularios sin botón de salida ni control de cierre: " + string.Join(", ", sinSalida));

            string menuCliente = File.ReadAllText(Path.Combine(raizVisual, "Cliente", "frmMenuCliente.cs"));
            string menuOrganizador = File.ReadAllText(Path.Combine(raizVisual, "Propietario", "frmMenuPropietario.cs"));
            string menuAdministrador = File.ReadAllText(Path.Combine(raizVisual, "Propietario", "frmMenuAdministrador.cs"));
            foreach (string menu in new[] { menuCliente, menuOrganizador, menuAdministrador })
            {
                StringAssert.Contains(menu, "btnCambiarContrasena");
                StringAssert.Contains(menu, "new frmCambiarContraseña");
                StringAssert.Contains(menu, "Retorno = this");
            }
        }

        private static string RaizRepositorio()
        {
            var directorio = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directorio != null && !File.Exists(Path.Combine(directorio.FullName, "Visual", "Visual.csproj")))
                directorio = directorio.Parent;
            Assert.IsNotNull(directorio, "No se pudo localizar la raíz del repositorio desde la carpeta de pruebas.");
            return directorio.FullName;
        }

        private static void CerrarVisualesPrevios(string exe)
        {
            string esperado = Path.GetFullPath(exe);
            foreach (var proceso in Process.GetProcessesByName("Visual"))
            {
                try
                {
                    if (string.Equals(Path.GetFullPath(proceso.MainModule.FileName), esperado, StringComparison.OrdinalIgnoreCase))
                    {
                        proceso.CloseMainWindow();
                        if (!proceso.WaitForExit(1500)) proceso.Kill();
                    }
                }
                catch (System.ComponentModel.Win32Exception) { }
                catch (InvalidOperationException) { }
                finally { proceso.Dispose(); }
            }
        }
    }
}
