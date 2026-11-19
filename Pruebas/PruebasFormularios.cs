using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
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

        // CP-12 — RF-12, RNF-03: mantener los formularios dentro del alto compatible con notebook.
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
                    // Las vistas creadas por codigo se validan por su tamano real.
                    if (tipo.Name == "frmRevisarSolicitudesOrganizador") Fake.ComoAdministrador();
                    else if (tipo.Name == "frmRevisarEventos") Fake.ComoAdministrador();
                    else if (tipo.Name == "frmSolicitudOrganizador") Fake.ComoCliente(Fake.Cliente(Entidades.NivelFidelidad.Bronce));
                    try
                    {
                        Form formulario;
                        if (tipo.Name == "frmBandejaSimulada")
                            formulario = new Visual.frmBandejaSimulada("sin-mensajes@pruebas.eventaura.com", "Recuperacion");
                        else
                            formulario = (Form)Activator.CreateInstance(tipo);
                        using (formulario)
                        {
                            if (formulario.ClientSize.Height > AltoMaximo)
                                problemas.Add($"{tipo.Name}: declara {formulario.ClientSize.Height} px de alto (maximo {AltoMaximo})");
                        }
                    }
                    finally { Negocio.GestorSesion.Instance.Cerrar(); }
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

            // SEG-2 incorpora control de ingreso y la fase de seguridad incorpora auditoría.
            // Las vistas de solicitudes, revisión del primer evento y bandeja simulada elevan el inventario a 26.
            Assert.AreEqual(26, formularios.Count, "Formularios encontrados: " + string.Join(", ", formularios.Select(t => t.Name)));
            Assert.AreEqual(0, problemas.Count, string.Join(Environment.NewLine, problemas));
        }

        // CP-53 — RF-02, RNF-04: instalar el manejo global de errores antes de abrir formularios.
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

        // CP-62 — RF-05, RNF-02: verificar tipografia y contraste del sistema visual.
        [TestMethod("CP-62: Contraste y fuentes del sistema de diseño")]
        public void CP62_ContrasteYFuentesDelTema()
        {
            foreach (Color fondo in new[] { Visual.Tema.Superficie, Visual.Tema.Fondo })
                foreach (Color texto in new[] { Visual.Tema.Texto, Visual.Tema.TextoSecundario })
                    Assert.IsTrue(Contraste(texto, fondo) >= 4.5, $"Contraste insuficiente: {texto} sobre {fondo}.");
            Assert.IsTrue(Contraste(Visual.Tema.Superficie, Visual.Tema.Primario) >= 4.5);
            Assert.IsTrue(Contraste(Visual.Tema.Error, Visual.Tema.Superficie) >= 4.5);
            Assert.IsTrue(Contraste(Visual.Tema.BordeControl, Visual.Tema.Superficie) >= 3.0);
            Assert.IsTrue(Contraste(Visual.Tema.Foco, Visual.Tema.Primario) >= 3.0, "El foco del botón primario debe contrastar con su fondo.");
            Assert.AreEqual(Color.Firebrick, Visual.Tema.Error);
            Assert.AreEqual(Color.Gainsboro, Visual.Tema.FondoDeshabilitado);
            foreach (Font fuente in new[] { Visual.Tema.Marca, Visual.Tema.Titulo, Visual.Tema.Seccion,
                Visual.Tema.Boton, Visual.Tema.Rotulo }) Assert.AreEqual("Segoe UI Semibold", fuente.Name);
            Assert.AreEqual("Segoe UI", Visual.Tema.Cuerpo.Name);
            Assert.AreEqual("Segoe UI", Visual.Tema.Ayuda.Name);
            Assert.AreEqual("Consolas", Visual.Tema.Comprobante.Name);
        }

        private static double Contraste(Color a, Color b)
        {
            double la = Luminancia(a), lb = Luminancia(b);
            return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
        }

        private static double Luminancia(Color color)
        {
            Func<byte, double> lineal = canal =>
            {
                double s = canal / 255.0;
                return s <= 0.04045 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
            };
            return 0.2126 * lineal(color.R) + 0.7152 * lineal(color.G) + 0.0722 * lineal(color.B);
        }

        // CP-63 — RF-05, RNF-02: las 20 pantallas centralizan el estilo; solo el piloto migra a 96 DPI.
        [DataTestMethod]
        [DataRow("Cliente/FrmCompraCliente", DisplayName = "CP-63: FrmCompraCliente sin estilo propio")]
        [DataRow("Cliente/frmConfirmacion", DisplayName = "CP-63: frmConfirmacion sin estilo propio")]
        [DataRow("Cliente/frmEventosCliente", DisplayName = "CP-63: frmEventosCliente sin estilo propio")]
        [DataRow("Cliente/frmHistorialCliente", DisplayName = "CP-63: frmHistorialCliente sin estilo propio")]
        [DataRow("Cliente/frmMenuCliente", DisplayName = "CP-63: frmMenuCliente sin estilo propio")]
        [DataRow("Cliente/frmPago", DisplayName = "CP-63: frmPago sin estilo propio")]
        [DataRow("Cliente/frmSuscripcion", DisplayName = "CP-63: frmSuscripcion sin estilo propio")]
        [DataRow("Propietario/frmAdministrarEvento", DisplayName = "CP-63: frmAdministrarEvento sin estilo propio")]
        [DataRow("Propietario/frmAgregarEvento", DisplayName = "CP-63: frmAgregarEvento sin estilo propio")]
        [DataRow("Propietario/frmAgregarSector", DisplayName = "CP-63: frmAgregarSector sin estilo propio")]
        [DataRow("Propietario/frmControlIngreso", DisplayName = "CP-63: frmControlIngreso sin estilo propio")]
        [DataRow("Propietario/frmEventosLocal", DisplayName = "CP-63: frmEventosLocal sin estilo propio")]
        [DataRow("Propietario/frmListasEspera", DisplayName = "CP-63: frmListasEspera sin estilo propio")]
        [DataRow("Propietario/frmMenuPropietario", DisplayName = "CP-63: frmMenuPropietario sin estilo propio")]
        [DataRow("Propietario/frmMenuAdministrador", DisplayName = "CP-63: frmMenuAdministrador sin estilo serializado")]
        [DataRow("Propietario/frmMisLocales", DisplayName = "CP-63: frmMisLocales sin estilo propio")]
        [DataRow("Propietario/frmRegistrarLocal", DisplayName = "CP-63: frmRegistrarLocal sin estilo propio")]
        [DataRow("Propietario/frmReportes", DisplayName = "CP-63: frmReportes sin estilo propio")]
        [DataRow("_Inicio/frmCambiarContraseña", DisplayName = "CP-63: frmCambiarContraseña sin estilo propio")]
        [DataRow("_Inicio/frmCrearUsuario", DisplayName = "CP-63: frmCrearUsuario sin estilo propio")]
        [DataRow("_Inicio/frmInicio", DisplayName = "CP-63: frmInicio sin estilo propio")]
        public void CP63_PantallasSinEstiloPropio(string pantalla)
        {
            string carpeta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "Visual");
            string ruta = Path.Combine(carpeta, pantalla.Replace('/', Path.DirectorySeparatorChar));
            foreach (string archivo in new[] { ruta + ".cs", ruta + ".Designer.cs" })
            {
                string fuente = File.ReadAllText(archivo);
                Assert.IsFalse(Regex.IsMatch(fuente, @"\b(?:Color|SystemColors)\s*\.\s*\w+"),
                    pantalla + ": color fuera de Tema en " + Path.GetFileName(archivo));
                Assert.IsFalse(Regex.IsMatch(fuente, @"new\s+(?:System\.Drawing\.)?Font\s*\("),
                    pantalla + ": fuente fuera de Tema en " + Path.GetFileName(archivo));
            }
            string designer = File.ReadAllText(ruta + ".Designer.cs");
            Assert.IsFalse(Regex.IsMatch(designer, @"\bTema\s*\."),
                pantalla + ": el Designer debe contener geometría literal, sin referencias al tema.");
            Assert.IsFalse(Regex.IsMatch(designer,
                @"\.(Font|ForeColor|BackColor|BackgroundColor|GridColor|FlatStyle|FlatAppearance|LinkColor|ActiveLinkColor|VisitedLinkColor|UseVisualStyleBackColor)\b"),
                pantalla + ": el estilo debe vivir en componentes y código del tema.");
            Match escala = Escala.Match(designer);
            Assert.IsTrue(escala.Success, pantalla + ": falta la escala de diseño.");
            // T-47 conserva la geometría y los DPI heredados; el rediseño hará la migración.
            if (pantalla == "Cliente/FrmCompraCliente")
            {
                Assert.AreEqual(96f, float.Parse(escala.Groups[1].Value, CultureInfo.InvariantCulture));
                Assert.AreEqual(96f, float.Parse(escala.Groups[2].Value, CultureInfo.InvariantCulture));
            }
        }

        // CP-64 — RF-05, RNF-03: declarar DPI de sistema para escalar de forma consistente.
        [TestMethod("CP-64: DPI de sistema declarado mediante el manifiesto")]
        public void CP64_DpiEnManifiesto()
        {
            string visual = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "Visual");
            var proyecto = new XmlDocument();
            proyecto.Load(Path.Combine(visual, "Visual.csproj"));
            Assert.AreEqual("app.manifest", proyecto.SelectSingleNode("//*[local-name()='ApplicationManifest']")?.InnerText);
            var manifiesto = new XmlDocument();
            manifiesto.Load(Path.Combine(visual, "app.manifest"));
            Assert.AreEqual("true", manifiesto.SelectSingleNode("//*[local-name()='dpiAware']")?.InnerText.Trim());
            Assert.AreEqual("system", manifiesto.SelectSingleNode("//*[local-name()='dpiAwareness']")?.InnerText.Trim());
            Assert.IsFalse(File.ReadAllText(Path.Combine(visual, "Program.cs")).Contains("SetProcessDPIAware"));
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
