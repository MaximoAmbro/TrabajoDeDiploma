using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    // RNF-03 + CLAUDE.md: ningún formulario declara un ClientSize de más de 790 px de alto.
    //
    // El alto NO se puede leer de la instancia: el constructor ya escala el formulario a los DPI
    // del proceso (AutoScaleMode.Dpi) y pisa AutoScaleDimensions, así que el valor declarado se
    // pierde. Un formulario diseñado a 144 DPI con 1020 de alto se lee como 680 en un proceso a
    // 96 DPI y pasaría la prueba. Por eso se lee lo declarado en el .Designer.cs y se contrasta
    // con la instancia: el alto escalado tiene que ser el declarado × DPI actual ÷ DPI de diseño.
    // Instanciar sirve además para comprobar que ningún constructor necesita la base.
    [TestClass]
    public class PruebasFormularios
    {
        public const int AltoMaximo = 790;

        private static readonly Regex DeclaracionClase = new Regex(@"partial\s+class\s+(\w+)");
        private static readonly Regex Tamano =
            new Regex(@"this\.ClientSize\s*=\s*new\s+System\.Drawing\.Size\(\s*(\d+)\s*,\s*(\d+)\s*\)");
        private static readonly Regex Escala =
            new Regex(@"this\.AutoScaleDimensions\s*=\s*new\s+System\.Drawing\.SizeF\(\s*([\d.]+)F\s*,\s*([\d.]+)F\s*\)");

        // CP-12 — RNF-03: altura de los formularios.
        [TestMethod("CP-12: Altura de los formularios")]
        public void RNF03_NingunFormularioPasaDe790DeAlto()
        {
            List<Type> formularios = typeof(Visual.frmInicio).Assembly.GetTypes()
                // La base común no es una pantalla y no tiene Designer.
                .Where(t => typeof(Form).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(Visual.FormularioEventAura))
                .OrderBy(t => t.Name)
                .ToList();
            Dictionary<string, string> designers = LeerDesigners();
            var problemas = new List<string>();

            foreach (Type tipo in formularios)
            {
                string fuente;
                if (!designers.TryGetValue(tipo.Name, out fuente))
                {
                    problemas.Add($"{tipo.Name}: no se encontró su .Designer.cs");
                    continue;
                }
                Match tamano = Tamano.Match(fuente);
                Match escala = Escala.Match(fuente);
                if (!tamano.Success || !escala.Success)
                {
                    problemas.Add($"{tipo.Name}: el .Designer.cs no declara ClientSize o AutoScaleDimensions");
                    continue;
                }
                int alto = int.Parse(tamano.Groups[2].Value);
                float dpiDiseno = float.Parse(escala.Groups[2].Value, CultureInfo.InvariantCulture);

                using (var formulario = (Form)Activator.CreateInstance(tipo))
                {
                    float dpiActual = formulario.CurrentAutoScaleDimensions.Height;
                    int esperado = (int)Math.Round(alto * dpiActual / dpiDiseno);
                    if (Math.Abs(formulario.ClientSize.Height - esperado) > 2)
                    {
                        problemas.Add($"{tipo.Name}: la instancia mide {formulario.ClientSize.Height} y lo declarado " +
                                      $"({alto} a {dpiDiseno} DPI) da {esperado}: o Windows lo recortó al alto de la " +
                                      "pantalla, o no se leyó el Designer correcto");
                    }
                }
                if (alto > AltoMaximo)
                {
                    problemas.Add($"{tipo.Name}: declara {alto} px de alto (máximo {AltoMaximo})");
                }
            }

            Assert.AreEqual(19, formularios.Count, "Formularios encontrados: " + string.Join(", ", formularios.Select(t => t.Name)));
            Assert.AreEqual(0, problemas.Count, string.Join(Environment.NewLine, problemas));
        }

        // CP-53 — RNF-02: el punto de entrada instala el manejador antes del bucle de formularios.
        [TestMethod("CP-53: Program instala el manejador global antes de ejecutar formularios")]
        public void CP53_ProgramInstalaManejadorGlobal()
        {
            string ruta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "Visual", "Program.cs");
            string fuente = File.ReadAllText(ruta);
            int instalacion = fuente.IndexOf("ManejadorErrores.Instalar();", StringComparison.Ordinal);
            int ejecucion = fuente.IndexOf("Application.Run", StringComparison.Ordinal);
            Assert.IsTrue(instalacion >= 0, "Falta instalar el manejador en Program.Main.");
            Assert.IsTrue(ejecucion > instalacion, "El manejador debe instalarse antes de Application.Run.");
        }

        // Nombre de la clase → texto de su .Designer.cs.
        private static Dictionary<string, string> LeerDesigners()
        {
            string visual = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "Visual");
            var designers = new Dictionary<string, string>();
            foreach (string archivo in Directory.GetFiles(visual, "*.Designer.cs", SearchOption.AllDirectories))
            {
                string texto = File.ReadAllText(archivo);
                Match clase = DeclaracionClase.Match(texto);
                if (clase.Success && Tamano.IsMatch(texto))
                {
                    designers[clase.Groups[1].Value] = texto;
                }
            }
            return designers;
        }
    }
}
