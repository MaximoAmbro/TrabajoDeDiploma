using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public sealed class PruebasInteraccionPantallas
    {
        public TestContext TestContext { get; set; }

        // CP-200 — RF-07, RNF-02: un fallo del PDF no revierte ni oculta el pago confirmado.
        [TestMethod("CP-200: Fallo al generar el ticket conserva el pedido y muestra recuperación")]
        public void CP200_FalloDeTicketDejaCompraRecuperable()
        {
            using (var pantalla = new Visual.FrmCompraCliente())
            {
                pantalla.CrearTickets = _ => { throw new IOException("Documentos no permite escritura"); };
                var confirmacion = pantalla.CrearConfirmacion(new Entidades.Compra { Id = 804, TotalPagado = 1200m });
                using (confirmacion)
                {
                    Assert.IsTrue(pantalla.CompraFinalizada, "El pago ya fue confirmado y no debe ofrecerse un segundo cobro.");
                    Assert.AreEqual(0, confirmacion.Rutas.Count);
                    StringAssert.Contains(confirmacion.AvisoTicket, "pedido EA-000804");
                    StringAssert.Contains(confirmacion.AvisoTicket, "Mis entradas");
                    StringAssert.Contains(confirmacion.AvisoTicket, "Documentos");
                }
            }
        }

        [TestMethod("CP-364: El alta web completa correo y continúa al acceso sin volver a crear la cuenta")]
        public void CP364_AltaWebCompletaElRecorridoHastaAcceso()
        {
            SeleccionEnviadorCodigos.Actual = new EnviadorSimulado();
            InterfazGrupoA.EnSta(() =>
            {
                string correo = Fake.MailNuevo();
                using (var bienvenida = new Visual.frmBienvenida())
                {
                    InterfazGrupoA.Mostrar(bienvenida);
                    var panelBienvenida = InterfazGrupoA.ControlDe<Visual.Web.PanelWeb>(bienvenida, "bienvenidaWeb");
                    Esperar(() => panelBienvenida.Preparado && panelBienvenida.Visible, "La bienvenida debe abrir su vista web.");
                    Ejecutar(panelBienvenida, "document.querySelector('[data-destino=alta]').click()" );

                    var alta = EsperarFormulario<Visual.frmCrearUsuario>("El botón de alta de la bienvenida debe abrir Crear cuenta.");
                    var panelAlta = InterfazGrupoA.ControlDe<Visual.Web.PanelWeb>(alta, "altaCuentaWeb");
                    Esperar(() => panelAlta.Preparado && panelAlta.Visible, "El alta debe mostrar la maqueta web y no el respaldo WinForms.");
                    string opciones = Ejecutar(panelAlta, "document.querySelector('[data-vista=elegir]').innerText");
                    StringAssert.Contains(opciones, "Quiero comprar entradas");
                    StringAssert.Contains(opciones, "Quiero organizar eventos");

                    Ejecutar(panelAlta, "document.getElementById('continuar').click()");
                    Esperar(() => Ejecutar(panelAlta, "!document.querySelector('[data-vista=datos]').classList.contains('oculto')") == "true",
                        "La elección debe avanzar al formulario de datos.");
                    Ejecutar(panelAlta,
                        "(()=>{const v=" + Newtonsoft.Json.JsonConvert.SerializeObject(correo) + ";" +
                        "for(const [id,valor] of Object.entries({nombre:'Prueba',apellido:'Recorrido',mail:v,contrasena:'Cliente123',confirmar:'Cliente123'})){"+
                        "const e=document.getElementById(id);e.value=valor;e.dispatchEvent(new Event('input',{bubbles:true}));}"+
                        "document.getElementById('formulario').requestSubmit();return true;})()");
                    Esperar(() => Ejecutar(panelAlta, "!document.querySelector('[data-vista=correo]').classList.contains('oculto')") == "true",
                        "Crear cuenta debe avanzar a la verificación del correo.");

                    Assert.IsTrue(panelAlta.Visible, "El alta no debe activar el respaldo WinForms al enviar sus datos.");
                    Assert.IsFalse(alta.Controls.Find("txtmail", true).Single().Visible,
                        "El formulario WinForms viejo debe seguir oculto mientras la maqueta web está activa.");
                    var mensaje = EnviadorSimulado.Ultimo(correo, "Verificacion");
                    Assert.IsNotNull(mensaje, "El recorrido real debe emitir el correo simulado de verificación.");
                    Ejecutar(panelAlta, "document.getElementById('codigo-correo').value=" + Newtonsoft.Json.JsonConvert.SerializeObject(mensaje.Codigo) + ";"+
                        "document.getElementById('confirmar-correo').click()");
                    Esperar(() => Ejecutar(panelAlta, "!document.querySelector('[data-vista=listo]').classList.contains('oculto')") == "true",
                        "Un correo confirmado debe avanzar a la pantalla final y no regresar al comienzo del alta.");
                    Assert.IsTrue(panelAlta.Visible, "La verificación exitosa no debe caer al respaldo WinForms.");
                    string final = Ejecutar(panelAlta, "document.querySelector('[data-vista=listo]').innerText");
                    StringAssert.Contains(final, "Tu cuenta está lista");
                    StringAssert.Contains(final, "Ir a iniciar sesión");

                    Ejecutar(panelAlta, "document.querySelector('[data-vista=listo] [data-destino=acceso]').click()");
                    var acceso = EsperarFormulario<Visual.frmInicio>("Después de completar el alta, la acción final debe abrir Acceso.");
                    var panelAcceso = InterfazGrupoA.ControlDe<Visual.Web.PanelWeb>(acceso, "accesoWeb");
                    Esperar(() => panelAcceso.Preparado && panelAcceso.Visible, "El destino final del alta debe ser el acceso web.");
                    Assert.IsFalse(alta.Visible, "El formulario de alta no debe reaparecer tras confirmar el correo.");
                    StringAssert.Contains(Ejecutar(panelAcceso, "document.querySelector('h1').innerText"), "Iniciar sesión");
                }
            });
        }

        private static void Esperar(Func<bool> condicion, string mensaje)
        {
            var reloj = Stopwatch.StartNew();
            while (reloj.Elapsed < TimeSpan.FromSeconds(20))
            {
                Application.DoEvents();
                if (condicion()) return;
                Thread.Sleep(15);
            }
            Assert.Fail(mensaje);
        }

        private static string Ejecutar(Visual.Web.PanelWeb panel, string script)
        {
            Task<string> tarea = panel.EjecutarScriptAsync(script);
            var reloj = Stopwatch.StartNew();
            while (!tarea.IsCompleted && reloj.Elapsed < TimeSpan.FromSeconds(10))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(tarea.IsCompleted, "WebView2 debe responder al guion del recorrido.");
            return tarea.GetAwaiter().GetResult();
        }

        private static T EsperarFormulario<T>(string mensaje) where T : Form
        {
            T resultado = null;
            Esperar(() =>
            {
                resultado = Application.OpenForms.Cast<Form>().OfType<T>().FirstOrDefault(f => f.Visible);
                return resultado != null;
            }, mensaje);
            return resultado;
        }

        // CP-199 — RF-03/RF-07/RF-11, RNF-02: sin DLL opcionales, la carga real termina en respaldo.
        [TestMethod("CP-199: Quitar las DLL de WebView2 y Newtonsoft activa el respaldo de las vistas")]
        public void CP199_RespaldoConDependenciasAusentesEnCopia()
        {
            if (Environment.GetEnvironmentVariable("EVENTAURA_CP199_HIJO") == "1")
            {
                ProbarPanelSinDependencias();
                return;
            }

            string raiz = Path.GetDirectoryName(typeof(PruebasInteraccionPantallas).Assembly.Location);
            string temporal = Path.Combine(Path.GetTempPath(), "EventAura-CP199-" + Guid.NewGuid().ToString("N"));
            CopiarCarpeta(raiz, temporal);
            try
            {
                var dependencias = Directory.GetFiles(temporal, "Microsoft.Web.WebView2.*.dll", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(temporal, "Newtonsoft.Json.dll", SearchOption.AllDirectories))
                    .Concat(Directory.GetFiles(temporal, "WebView2Loader.dll", SearchOption.AllDirectories)).Distinct().ToArray();
                Assert.IsTrue(dependencias.Any(ruta => Path.GetFileName(ruta) == "Microsoft.Web.WebView2.Core.dll"), "La copia debe incluir WebView2.Core antes de quitarla.");
                Assert.IsTrue(dependencias.Any(ruta => Path.GetFileName(ruta) == "Microsoft.Web.WebView2.WinForms.dll"), "La copia debe incluir WebView2.WinForms antes de quitarla.");
                Assert.IsTrue(dependencias.Any(ruta => Path.GetFileName(ruta) == "Newtonsoft.Json.dll"), "La copia debe incluir Newtonsoft antes de quitarla.");
                Assert.IsTrue(dependencias.Any(ruta => Path.GetFileName(ruta) == "WebView2Loader.dll"), "La copia debe incluir los loaders antes de quitarla.");
                foreach (string archivo in dependencias)
                    File.Delete(archivo);

                string vstest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Microsoft Visual Studio", "2022", "BuildTools", "Common7", "IDE", "CommonExtensions", "Microsoft", "TestWindow", "vstest.console.exe");
                Assert.IsTrue(File.Exists(vstest), "Se requiere vstest.console.exe para ejecutar la copia aislada.");
                var inicio = new ProcessStartInfo(vstest,
                    "\"" + Path.Combine(temporal, "Pruebas.dll") + "\" /TestCaseFilter:\"FullyQualifiedName~CP199_RespaldoConDependenciasAusentesEnCopia\" /Logger:console;verbosity=normal")
                { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
                inicio.EnvironmentVariables["EVENTAURA_CP199_HIJO"] = "1";
                using (var proceso = Process.Start(inicio))
                {
                    string salida = proceso.StandardOutput.ReadToEnd();
                    string error = proceso.StandardError.ReadToEnd();
                    Assert.IsTrue(proceso.WaitForExit(120000), "El proceso aislado debe terminar.");
                    Assert.AreEqual(0, proceso.ExitCode, salida + Environment.NewLine + error);
                    Assert.IsTrue(salida.Contains("CP-199"), salida);
                }
            }
            finally
            {
                if (Directory.Exists(temporal)) Directory.Delete(temporal, true);
            }
        }

        private static void ProbarPanelSinDependencias()
        {
            string[] paginas = { "catalogo.html", "compra.html", "armador-paquete.html", "datos.html", "datos.html", "gestion.html", "bienvenida.html", "acceso.html" };
            foreach (string pagina in paginas)
            {
                using (var panel = new Visual.Web.PanelWeb())
                {
                    bool respaldo = false;
                    panel.VistaNoDisponible += () => respaldo = true;
                    panel.Cargar(pagina);
                    Assert.IsTrue(respaldo, pagina + " debe activar el evento de respaldo al faltar las DLL.");
                    Assert.IsTrue(panel.InicializacionTerminada, pagina);
                    Assert.IsFalse(panel.Preparado, pagina);
                    Assert.IsFalse(panel.Visible, pagina);
                }
            }
        }

        private static void CopiarCarpeta(string origen, string destino)
        {
            Directory.CreateDirectory(destino);
            foreach (string archivo in Directory.GetFiles(origen))
                File.Copy(archivo, Path.Combine(destino, Path.GetFileName(archivo)), true);
            foreach (string carpeta in Directory.GetDirectories(origen))
                CopiarCarpeta(carpeta, Path.Combine(destino, Path.GetFileName(carpeta)));
        }

        private static void VerificarMigracionWeb(Form formulario)
        {
            string nombre = formulario.GetType().Name;
            if (!new[] { "frmHistorialCliente", "frmMisLocales", "frmEventosLocal", "frmControlIngreso", "frmAuditoria", "frmRevisarEventos", "frmInicio", "frmBienvenida" }.Contains(nombre)) return;
            var panel = Descendientes(formulario).OfType<Visual.Web.PanelWeb>().Single();
            var reloj = System.Diagnostics.Stopwatch.StartNew();
            while (!panel.InicializacionTerminada && reloj.ElapsedMilliseconds < 5000)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(panel.InicializacionTerminada, nombre + " debe finalizar en vista web o respaldo.");
            if (panel.Preparado)
            {
                Assert.IsTrue(panel.Visible, nombre + " dejó oculta una vista web preparada.");
                return;
            }
            Assert.IsFalse(panel.Visible, nombre + " debe ocultar un motor web no disponible.");
            if (formulario is Visual.frmBienvenida)
            {
                Assert.IsTrue(Application.OpenForms.OfType<Visual.frmInicio>().Any(f => f.Visible),
                    "Si la bienvenida web falla, debe abrir directamente el acceso WinForms.");
                return;
            }
            if (formulario is Visual.frmInicio)
            {
                bool respaldoAcceso = formulario.Controls.Find("txtMailLogin", true).Any(c => c.Visible) ||
                    formulario.Controls.Find("avisoBaseNoDisponible", true).Any(c => c.Visible);
                Assert.IsTrue(respaldoAcceso, "El acceso WinForms debe seguir visible si falla WebView2.");
                return;
            }
            bool respaldo = nombre == "frmHistorialCliente"
                ? formulario.Controls.Find("dgvCompras", true).Any(c => c.Visible) || formulario.Controls.Find("estadoSinCompras", true).Any(c => c.Visible)
                : nombre == "frmMisLocales"
                    ? formulario.Controls.Find("dgvLocales", true).Any(c => c.Visible) || formulario.Controls.Find("estadoSinLocales", true).Any(c => c.Visible)
                    : nombre == "frmEventosLocal"
                        ? formulario.Controls.Find("dgvEventos", true).Any(c => c.Visible) || formulario.Controls.Find("estadoSinEventos", true).Any(c => c.Visible)
                        : nombre == "frmControlIngreso"
                            ? formulario.Controls.Find("txtCodigo", true).Any(c => c.Visible) || formulario.Controls.Find("estadoSinIngresoAbierto", true).Any(c => c.Visible)
                            : nombre == "frmAuditoria"
                                ? formulario.Controls.Find("tablaAuditoria", true).Any(c => c.Visible) || formulario.Controls.Find("estadoSinAuditoria", true).Any(c => c.Visible)
                                : formulario.Controls.Find("lstEventosEnRevision", true).Any(c => c.Visible) || formulario.Controls.Find("estadoSinEventosRevision", true).Any(c => c.Visible);
            Assert.IsTrue(respaldo, nombre + " no dejó visible su contenido WinForms de respaldo.");
        }

        // CP-357 — RNF-02/RNF-03: interacción WinForms real, foco, teclado, y geometría en ventanas abiertas.
        [TestMethod("CP-357: Teclado y geometría de las 26 ventanas reales")]
        public void CP357_InteraccionRealDeLasPantallas()
        {
            var todosLosTipos = typeof(Visual.FormularioEventAura).Assembly.GetTypes()
                .Where(t => typeof(Form).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(Visual.FormularioEventAura))
                .OrderBy(t => t.Name).ToArray();
            Assert.AreEqual(26, todosLosTipos.Length);
            string filtro = Environment.GetEnvironmentVariable("EVENTAURA_CP357_FILTRO");
            var tipos = string.IsNullOrWhiteSpace(filtro) ? todosLosTipos :
                todosLosTipos.Where(t => t.Name.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            Assert.IsTrue(tipos.Length > 0, "EVENTAURA_CP357_FILTRO no coincide con una ventana.");
            var resultados = new List<string>();
            InterfazGrupoA.EnSta(() =>
            {
                using (var temporizador = new System.Threading.Timer(_ => CerrarDialogosDePrueba(), null, 100, 100))
                {
                    foreach (var tipo in tipos)
                    {
                        Form formulario = null;
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
                                Fake.Limpiar();
                            if (tipo == typeof(Visual.frmAuditoria) &&
                                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
                            {
                                GestorUsuarios.Instance.Autenticar("bronce@eventaura.com", "Cliente123");
                                GestorSesion.Instance.Cerrar();
                                GestorUsuarios.Instance.Autenticar("admin@eventaura.com", "Admin1234");
                            }
                            PrepararSesion(tipo);
                            formulario = (Form)Activator.CreateInstance(tipo);
                            PrepararContextoSeleccionado(formulario);
                            formulario.Show();
                            if (formulario is Visual.FrmCompraCliente &&
                                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CP357_TAMANO")))
                            {
                                var dimensiones = Environment.GetEnvironmentVariable("EVENTAURA_CP357_TAMANO").Split('x');
                                if (dimensiones.Length == 2 && int.TryParse(dimensiones[0], out int ancho) && int.TryParse(dimensiones[1], out int alto))
                                {
                                    formulario.Size = new Size(ancho, alto);
                                    Application.DoEvents();
                                }
                            }
                            Application.DoEvents();
                            Thread.Sleep(30);
                            Application.DoEvents();
                            VerificarMigracionWeb(formulario);

                            if (formulario is Visual.frmReportes &&
                                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
                            {
                                bool demo = Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_DEMO") == "1";
                                if (demo)
                                    ((DateTimePicker)formulario.Controls["dtpDesde"]).Value = DateTime.Today.AddMonths(-6);
                                ((DateTimePicker)formulario.Controls["dtpHasta"]).Value = DateTime.Today.AddMonths(demo ? 4 : 3);
                                ((Button)formulario.Controls["btnGenerar"]).PerformClick();
                            }
                            DataGridViewCell cantidadCapturada = null;
                            if (formulario is Visual.FrmCompraCliente &&
                                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
                            {
                                var sectores = Descendientes(formulario).OfType<DataGridView>().Single(g => g.Name == "dgvSectores");
                                var filaDisponible = sectores.Rows.Cast<DataGridViewRow>().First(f => Convert.ToInt32(f.Cells["Disponibles"].Value) >= 2);
                                sectores.EndEdit();
                                cantidadCapturada = filaDisponible.Cells["Cantidad"];
                                cantidadCapturada.Value = 2;
                                Descendientes(formulario).OfType<Button>().Single(b => b.Name == "btnComprar").Focus();
                                var contenido = Descendientes(formulario).OfType<Panel>().Single(p => p.Name == "pnlContenidoCompra");
                                contenido.AutoScrollPosition = Point.Empty;
                                Assert.IsTrue(Descendientes(formulario).OfType<Button>().Single(b => b.Name == "btnComprar").Enabled,
                                    "La captura de compra requiere un paquete con entradas disponibles.");
                                Application.DoEvents();
                                var total = Descendientes(formulario).OfType<Label>().Single(l => l.Name == "lblTotalDestacado");
                                Rectangle areaTotal = total.RectangleToScreen(total.ClientRectangle);
                                Rectangle areaSectores = sectores.RectangleToScreen(sectores.ClientRectangle);
                                Assert.IsTrue(contenido.ClientRectangle.Contains(contenido.RectangleToClient(areaSectores)),
                                    "La grilla de sectores del evento debe entrar completa en la captura inicial.");
                                for (Control padre = total.Parent; padre != null; padre = padre.Parent)
                                    Assert.IsTrue(padre.ClientRectangle.Contains(padre.RectangleToClient(areaTotal)),
                                        "El total debe aparecer completo en la captura inicial.");
                            }
                            CapturarSiSeSolicito(formulario);
                            if (cantidadCapturada != null)
                            {
                                cantidadCapturada.Value = 0;
                                cantidadCapturada.DataGridView.Focus();
                            }
                            VerificarMedidas(formulario);
                            if (formulario is Visual.frmCambiarContraseña cambio) VerificarFilaAviso(cambio);
                            string foco = VerificarFoco(formulario);
                            string geometria = VerificarGeometria(formulario);
                            VerificarRespaldoPosteriorALaCarga(formulario);
                            string teclado = VerificarTeclas(formulario, tipo);
                            string fila = tipo.Name + " | " + foco + " | " + teclado + " | " + geometria;
                            resultados.Add(fila);
                            Console.WriteLine(fila);
                        }
                        catch (Exception ex)
                        {
                            string fila = tipo.Name + " | DEFECTO: " + ex;
                            resultados.Add(fila);
                            Console.WriteLine(fila);
                        }
                        finally
                        {
                            foreach (Form abierta in Application.OpenForms.Cast<Form>().ToArray())
                                if (!abierta.IsDisposed) abierta.Dispose();
                            formulario?.Dispose();
                            GestorSesion.Instance.Cerrar();
                            Application.DoEvents();
                        }
                    }
                }
            });

            foreach (string resultado in resultados) TestContext?.WriteLine(resultado);
            string[] noAislables = resultados.Where(r => r.Contains("NO AISLABLE:")).ToArray();
            Assert.AreEqual(0, noAislables.Length, "Quedaron pantallas que no pudieron abrirse: " + string.Join("; ", noAislables));
            string[] defectos = resultados.Where(r => r.Contains("DEFECTO:")).ToArray();
            Assert.AreEqual(0, defectos.Length, "Interacción real defectuosa: " + string.Join("; ", defectos));
        }

        private static void VerificarRespaldoPosteriorALaCarga(Form formulario)
        {
            foreach (var panel in Descendientes(formulario).OfType<Visual.Web.PanelWeb>().Where(p => p.Preparado).ToArray())
            {
                panel.SimularFallaPosteriorALaCarga();
                Application.DoEvents();
                Assert.IsFalse(panel.Visible, "La vista web debe ocultarse cuando falla tras quedar visible.");
                if (formulario is Visual.frmBienvenida)
                {
                    Assert.IsTrue(Application.OpenForms.OfType<Visual.frmInicio>().Any(f => f.Visible),
                        "El fallo de la bienvenida debe conducir al acceso WinForms.");
                    continue;
                }
                if (formulario is Visual.frmCrearUsuario)
                {
                    Assert.IsTrue(formulario.Controls.Find("txtmail", true).Any(c => c.Visible) &&
                        formulario.Controls.Find("btnregistrarse", true).Any(c => c.Visible),
                        "El alta WinForms debe recuperar sus campos y acción al fallar WebView2.");
                    continue;
                }
                var aviso = Descendientes(formulario).OfType<Control>()
                    .FirstOrDefault(c => c.Name.StartsWith("aviso", StringComparison.OrdinalIgnoreCase) && c.Name.EndsWith("Web", StringComparison.OrdinalIgnoreCase));
                Assert.IsNotNull(aviso, "El formulario debe conservar el aviso de respaldo.");
                Assert.IsTrue(aviso.Visible, "El aviso de respaldo debe aparecer después del fallo. " + formulario.Name + ": aviso=" + aviso.Name + ", panel terminado=" + panel.InicializacionTerminada + ", preparado=" + panel.Preparado + ", visible=" + panel.Visible + ".");
                if (formulario is Visual.frmMenuAdministrador)
                {
                    var indicadores = Descendientes(formulario).OfType<FlowLayoutPanel>().FirstOrDefault(c => c.Name == "indicadoresPanel");
                    Assert.IsNotNull(indicadores);
                    Assert.IsTrue(indicadores.Visible && indicadores.Controls.Count > 0,
                        "Los indicadores WinForms del Administrador deben quedar visibles.");
                }
            }
        }

        private static void PrepararSesion(Type tipo)
        {
            string nombre = tipo.Name;
            if (nombre == "frmInicio") { GestorSesion.Instance.Cerrar(); return; }
            if (nombre == "frmMenuAdministrador" || nombre == "frmAuditoria" || nombre == "frmReportes" ||
                nombre == "frmRevisarEventos" || nombre == "frmRevisarSolicitudesOrganizador")
                Fake.ComoAdministrador();
            else if (tipo.Namespace == "Visual.Vendedor" || nombre == "frmControlIngreso" || nombre == "frmMenuPropietario" || nombre == "frmMisLocales" ||
                nombre == "frmListasEspera" || nombre == "frmRegistrarLocal" || nombre == "frmReportes")
                Fake.ComoOrganizador();
            else if ((nombre == "frmMenuCliente" || nombre == "frmHistorialCliente") && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
            {
                var cuenta = Datos.RepositorioUsuarios.Instance.ObtenerPorMail("oro@eventaura.com");
                Fake.ComoCliente(Datos.RepositorioFidelizacion.Instance.ObtenerCliente(cuenta.Id));
            }
            else
                Fake.ComoCliente(Fake.Cliente(Entidades.NivelFidelidad.Bronce));
        }

        private static string VerificarFoco(Form formulario)
        {
            Control esperadoNativo = DescendientesEnOrden(formulario).FirstOrDefault(EsCampoInicial);
            Control actualNativo = FocoProfundo(formulario);
            if (esperadoNativo != null && actualNativo == esperadoNativo)
                return VerificarFocoNativo(formulario, esperadoNativo);
            var vistaWeb = Descendientes(formulario).OfType<Visual.Web.PanelWeb>()
                .FirstOrDefault(panel => panel.Visible && panel.Preparado);
            if (vistaWeb != null)
            {
                vistaWeb.EnfocarVista();
                var tareaFoco = vistaWeb.EjecutarScriptAsync(
                    "(function(){var a=Array.from(document.querySelectorAll('a[href],button:not(:disabled),input:not(:disabled),select:not(:disabled),textarea:not(:disabled),[tabindex]')).filter(function(e){return e.tabIndex>=0&&e.getClientRects().length>0&&getComputedStyle(e).visibility!=='hidden'});if(!a.length)return 'sin-control|0';a.sort(function(x,y){return x.tabIndex-y.tabIndex});a[0].focus();var e=document.activeElement;return e.tagName+'|'+(e.id||'')+'|'+(e.disabled?'deshabilitado':'habilitado')+'|'+a.length})()");
                var reloj = System.Diagnostics.Stopwatch.StartNew();
                while (!tareaFoco.IsCompleted && reloj.ElapsedMilliseconds < 3000)
                {
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
                Assert.IsTrue(tareaFoco.IsCompleted && !tareaFoco.IsFaulted, "La vista web debe responder a Tab y permitir consultar el foco DOM.");
                string focoDom = (tareaFoco.Result ?? string.Empty).Trim().Trim('"');
                string[] partes = focoDom.Split('|');
                int cantidadFoco = 0;
                Assert.IsTrue(partes.Length == 4 && int.TryParse(partes[3], out cantidadFoco) && cantidadFoco > 0,
                    "La vista web debe exponer elementos interactivos en orden de tabulación: " + focoDom);
                Assert.IsTrue(partes[2] == "habilitado", "El foco DOM debe quedar en un control habilitado: " + focoDom);
                return "foco DOM: OK (" + partes[0] + "#" + partes[1] + "; " + cantidadFoco + " controles tabulables)";
            }
            Control esperado = esperadoNativo;
            Control actual = actualNativo;
            if (esperado == null) return actual == null ? "foco: sin campo" : "foco: " + actual.Name;
            if (actual == null || actual != esperado)
                throw new AssertFailedException("Foco inicial real: " + (actual?.Name ?? "ninguno") + "; esperado: " + esperado.Name);

            var visitados = new HashSet<Control>();
            for (int i = 0; i < 40; i++)
            {
                SendKeys.SendWait("{TAB}");
                Application.DoEvents();
                Control siguiente = FocoProfundo(formulario);
                if (siguiente == null || !siguiente.TabStop || !siguiente.Enabled || !siguiente.Visible)
                    throw new AssertFailedException("Tab dejó el foco en un control no navegable.");
                if (siguiente == esperado && visitados.Count > 0) break;
                if (!visitados.Add(siguiente)) throw new AssertFailedException("El recorrido de Tab repitió " + siguiente.Name + " antes de cerrar el ciclo.");
            }
            return "foco/tab: OK (" + visitados.Count + " controles)";
        }

        private static string VerificarFocoNativo(Form formulario, Control esperado)
        {
            Control actual = FocoProfundo(formulario);
            if (actual != esperado)
                throw new AssertFailedException("El foco nativo debe quedar en " + esperado.Name + ".");
            var visitados = new HashSet<Control>();
            for (int i = 0; i < 40; i++)
            {
                SendKeys.SendWait("{TAB}");
                Application.DoEvents();
                Control siguiente = FocoProfundo(formulario);
                if (siguiente == null || !siguiente.TabStop || !siguiente.Enabled || !siguiente.Visible)
                    throw new AssertFailedException("Tab dejó el foco nativo en un control no navegable.");
                if (siguiente == esperado && visitados.Count > 0) break;
                if (!visitados.Add(siguiente)) throw new AssertFailedException("Tab repitió el control " + siguiente.Name + ".");
            }
            return "foco/tab nativo: OK (" + visitados.Count + " controles)";
        }

        private static string VerificarTeclas(Form formulario, Type tipo)
        {
            string nombre = formulario.GetType().Name;
            bool menu = nombre == "frmMenuCliente" || nombre == "frmMenuPropietario" || nombre == "frmMenuAdministrador";
            var botonPrincipal = formulario.AcceptButton as Button;
            bool tieneAccion = Descendientes(formulario).OfType<Button>().Any(b => b.Visible && b.Enabled && b is Visual.Componentes.BotonPrimario);
            if (tieneAccion && !menu && botonPrincipal == null)
                throw new AssertFailedException("Tiene acción primaria visible, pero el formulario no asigna AcceptButton.");

            string enter = "Enter: " + (botonPrincipal == null ? "sin acción predeterminada" : botonPrincipal.Name);
            if (nombre == "frmSuscripcion") enter = "Enter no enviado: abriría confirmación modal; pendiente de portón humano";
            else if (botonPrincipal != null && botonPrincipal.Enabled && botonPrincipal.Visible)
            {
                bool disparo = false;
                EventHandler observador = (s, e) => disparo = true;
                botonPrincipal.Click += observador;
                Control primerCampo = DescendientesEnOrden(formulario).FirstOrDefault(EsCampoInicial);
                if (primerCampo != null) primerCampo.Focus();
                ActivarVentana(formulario);
                SendKeys.SendWait("{ENTER}");
                Application.DoEvents();
                botonPrincipal.Click -= observador;
                if (!disparo) throw new AssertFailedException("Enter no disparó el botón principal " + botonPrincipal.Name + ".");
            }
            else if (botonPrincipal != null) enter += " (deshabilitado por falta de selección/contexto)";

            var botonCancelar = formulario.CancelButton as Button;
            string nombreCancelacion = botonCancelar?.Name;
            if (botonCancelar == null)
            {
                // ProcessCmdKey ofrece Escape como cierre incluso si el formulario no tiene botón dedicado.
                using (var ventanaCancelar = CrearVentana(tipo))
                {
                    ventanaCancelar.Show();
                    Application.DoEvents();
                    ActivarVentana(ventanaCancelar);
                    SendKeys.SendWait("{ESC}");
                    Application.DoEvents();
                    if (!ventanaCancelar.IsDisposed && ventanaCancelar.Visible)
                        throw new AssertFailedException("Esc no cerró la ventana sin CancelButton.");
                }
                return enter + "; Esc: cierra la ventana sin CancelButton";
            }

            using (var segundaVentana = CrearVentana(tipo))
            {
                segundaVentana.Show();
                Application.DoEvents();
                var cancelarReal = segundaVentana.CancelButton as Button;
                bool cancelado = false;
                if (cancelarReal != null)
                {
                    EventHandler alCancelar = (s, e) => cancelado = true;
                    cancelarReal.Click += alCancelar;
                    ActivarVentana(segundaVentana);
                    SendKeys.SendWait("{ESC}");
                    Application.DoEvents();
                    try { cancelarReal.Click -= alCancelar; } catch (ObjectDisposedException) { }
                    if (!cancelado) throw new AssertFailedException("Esc no disparó " + cancelarReal.Name + ".");
                }
                else
                {
                    ActivarVentana(segundaVentana);
                    SendKeys.SendWait("{ESC}");
                    Application.DoEvents();
                    if (!segundaVentana.IsDisposed && segundaVentana.Visible)
                        throw new AssertFailedException("Esc no cerró la ventana sin botón Cancelar.");
                }
            }
            return enter + "; Esc: " + nombreCancelacion;
        }

        private static Form CrearVentana(Type tipo)
        {
            PrepararSesion(tipo);
            var formulario = (Form)Activator.CreateInstance(tipo);
            PrepararContextoSeleccionado(formulario);
            return formulario;
        }
        private static void ActivarVentana(Form formulario)
        {
            formulario.Activate();
            formulario.BringToFront();
            SetForegroundWindow(formulario.Handle);
            Application.DoEvents();
        }

        private static void PrepararContextoSeleccionado(Form formulario)
        {
            if (formulario is Visual.frmPago pago)
            {
                pago.Compra = CrearCompraPendiente();
                return;
            }
            if (formulario is Visual.frmConfirmacion confirmacion)
            {
                var clienteConfirmacion = Fake.Cliente(Entidades.NivelFidelidad.Bronce);
                var eventoConfirmacion = Fake.EventoPublicado(10000m, 100);
                confirmacion.Compra = Fake.CompraConfirmada(clienteConfirmacion, eventoConfirmacion, 1, false);
                confirmacion.Rutas = new List<string>();
                return;
            }
            var usuario = GestorSesion.Instance.UsuarioActual;
            if (usuario == null) return;
            var locales = usuario.TipoUsuario == "Organizador" ? GestorLocales.Instance.PorPropietario(usuario.Id) : new List<Entidades.Local>();
            int idLocal = locales.FirstOrDefault()?.Id ?? 0;
            if (formulario is Visual.Vendedor.frmEventosLocal eventosLocal)
                eventosLocal.NombreLocal = locales.FirstOrDefault()?.Nombre ?? string.Empty;
            var eventos = idLocal > 0 ? GestorEventos.Instance.PorLocal(idLocal) : GestorEventos.Instance.Vigentes();
            int idEvento = eventos.FirstOrDefault()?.Id ?? 0;
            if (formulario is Visual.FrmCompraCliente &&
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
            {
                var candidatos = eventos.Select(evento => new { Evento = evento, Sectores = GestorEventos.Instance.Sectores(evento.Id) });
                if (Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_DEMO") == "1")
                    candidatos = candidatos.OrderByDescending(candidato => candidato.Sectores.Count)
                        .ThenBy(candidato => candidato.Evento.Fecha);
                var demostracion = candidatos.FirstOrDefault(candidato => candidato.Sectores.Count > 0 &&
                    candidato.Sectores.All(sector => GestorPaquetes.Instance.DisponiblesDe(sector.Id) >= 2))?.Evento;
                idEvento = (demostracion ?? Fake.EventoPublicado(20000m, 100)).Id;
            }
            foreach (var campo in formulario.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (campo.FieldType == typeof(int))
                {
                    if (campo.Name.IndexOf("Usuario", StringComparison.OrdinalIgnoreCase) >= 0) campo.SetValue(formulario, usuario.Id);
                    else if (campo.Name.IndexOf("Local", StringComparison.OrdinalIgnoreCase) >= 0) campo.SetValue(formulario, idLocal);
                    else if (campo.Name.IndexOf("Evento", StringComparison.OrdinalIgnoreCase) >= 0) campo.SetValue(formulario, idEvento);
                }
                else if (campo.FieldType == typeof(string))
                {
                    if (campo.Name.IndexOf("NombreUsuario", StringComparison.OrdinalIgnoreCase) >= 0) campo.SetValue(formulario, usuario.Nombre + " " + usuario.Apellido);
                    else if (campo.Name.IndexOf("NombreLocal", StringComparison.OrdinalIgnoreCase) >= 0) campo.SetValue(formulario, locales.FirstOrDefault()?.Nombre ?? string.Empty);
                    else if (campo.Name.IndexOf("NombreEvento", StringComparison.OrdinalIgnoreCase) >= 0) campo.SetValue(formulario, eventos.FirstOrDefault()?.Nombre ?? string.Empty);
                }
            }
        }

        private static Entidades.Compra CrearCompraPendiente()
        {
            var cliente = Fake.Cliente(Entidades.NivelFidelidad.Bronce);
            Fake.ComoCliente(cliente);
            var evento = Fake.EventoPublicado(10000m, 100);
            var paquete = GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id);
            GestorPaquetes.Instance.AgregarEntradas(paquete, GestorEventos.Instance.Sectores(evento.Id).First().Id, 1);
            return GestorCompras.Instance.Iniciar(paquete, GestorFidelizacion.Instance.ObtenerCliente(cliente.Id));
        }

        private static string VerificarGeometria(Form formulario)
        {
            if (formulario.FormBorderStyle == FormBorderStyle.Sizable || formulario.FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                var original = formulario.Size;
                var minimo = formulario.MinimumSize;
                formulario.Size = new Size(Math.Max(1, minimo.Width - 32), Math.Max(1, minimo.Height - 32));
                Application.DoEvents();
                Assert.IsTrue(formulario.Width >= minimo.Width && formulario.Height >= minimo.Height,
                    "El tamaño mínimo debe impedir que colapse el contenido.");
                formulario.Size = original;
                formulario.WindowState = FormWindowState.Maximized;
                Application.DoEvents();
                formulario.WindowState = FormWindowState.Normal;
                formulario.Size = new Size(formulario.Width + 80, formulario.Height + 50);
                Application.DoEvents();
            }
            else if (formulario.MaximizeBox)
                throw new AssertFailedException("Ventana no adaptable ofrece maximizar.");

            foreach (Control control in Descendientes(formulario).Where(c => c.Visible && c.Parent != null))
            {
                if (control.Dock != DockStyle.None || (control.Parent as ScrollableControl)?.AutoScroll == true) continue;
                if (control.Left < 0 || control.Top < 0 || control.Right > control.Parent.ClientSize.Width + 2 || control.Bottom > control.Parent.ClientSize.Height + 2)
                    throw new AssertFailedException(control.Name + " " + control.Bounds + " queda fuera de " + control.Parent.Name + " " + control.Parent.ClientSize + ".");
            }
            foreach (var grupo in Descendientes(formulario).GroupBy(c => c.Parent))
            {
                var interactivos = grupo.Where(c => EsInteractivoVisible(c) || EsTextoVisible(c)).ToArray();
                for (int i = 0; i < interactivos.Length; i++)
                    for (int j = i + 1; j < interactivos.Length; j++)
                        if (interactivos[i].Bounds.IntersectsWith(interactivos[j].Bounds))
                            throw new AssertFailedException(interactivos[i].Name + " " + interactivos[i].Bounds + " se superpone con " + interactivos[j].Name + " " + interactivos[j].Bounds + ".");
            }
            return "ventana: OK (" + Visual.Disposicion.Dpi(formulario) + " DPI; " + formulario.FormBorderStyle + ")";
        }

        private static bool EsTextoVisible(Control control)
        {
            return control.Visible && !string.IsNullOrWhiteSpace(control.Text) &&
                !(control is Panel) && !(control is UserControl) && !(control is GroupBox);
        }

        private static void VerificarMedidas(Form formulario)
        {
            int Px(int medida) => Visual.Disposicion.Pixeles(formulario, medida);
            Assert.IsTrue(formulario.MinimumSize.Width > 0 && formulario.MinimumSize.Height > 0, "La ventana requiere un tamaño mínimo.");
            if (formulario is Visual.frmBienvenida ||
                (formulario is Visual.frmInicio && formulario.Controls["accesoWeb"]?.Visible == true))
            {
                var web = formulario.Controls.OfType<Visual.Web.PanelWeb>().Single();
                Assert.AreEqual(DockStyle.Fill, web.Dock, "La vista web de entrada debe ocupar el área de la ventana.");
                Assert.AreEqual(formulario.ClientSize, web.Size, "La vista web de entrada debe cubrir toda la ventana.");
                return;
            }
            var barra = formulario.Controls.OfType<Visual.Componentes.BarraSuperior>().Single();
            Assert.AreEqual(Px(56), barra.Height, "Alto de la barra superior");
            Assert.IsTrue(barra.Visible, "La barra superior tiene que estar visible");
            foreach (Control control in formulario.Controls)
            {
                if (control == barra || !control.Visible) continue;
                Assert.IsFalse(control.Bounds.IntersectsWith(barra.Bounds),
                    "La barra superior se superpone con " + control.Name + " " + control.Bounds);
            }
            var lateral = formulario.Controls["navegacionLateral"];
            if (lateral != null)
            {
                Assert.AreEqual(Px(240), lateral.Width, "Ancho del menú lateral");
                foreach (Button boton in lateral.Controls.OfType<Button>())
                    Assert.AreEqual(new Size(Px(240), Px(40)), boton.Size, boton.Name);
            }

            if (formulario is Visual.frmInicio || formulario is Visual.frmCrearUsuario ||
                formulario is Visual.frmCambiarContraseña)
            {
                foreach (TextBox campo in formulario.Controls.OfType<TextBox>().Where(c => c.Visible))
                {
                    int ancho = formulario is Visual.frmCrearUsuario && campo.Name == "txtmail"
                        ? Px(2 * Visual.Tema.AnchoCampo + Visual.Tema.Espacio.L) : Px(Visual.Tema.AnchoCampo);
                    Assert.AreEqual(new Size(ancho, Px(32)), campo.Size, campo.Name);
                    Assert.IsTrue(formulario.Controls.OfType<Label>().Any(rotulo => rotulo.Visible &&
                        rotulo.Left == campo.Left && campo.Top - rotulo.Bottom == Px(4)),
                        "Falta el rótulo a 4 px de " + campo.Name);
                }
            }

            foreach (var grilla in Descendientes(formulario).OfType<FlowLayoutPanel>())
            {
                var tarjetas = grilla.Controls.OfType<Visual.Componentes.DatoPanel>().Where(t => t.Visible).ToArray();
                foreach (var fila in tarjetas.GroupBy(t => t.Top))
                    Assert.AreEqual(1, fila.Select(t => t.Height).Distinct().Count(), "Altura común de la fila " + grilla.Name);
                foreach (var tarjeta in tarjetas)
                    Assert.AreEqual(Px(grilla.Name == "tarjetasInicioCliente" ? 264 : 208), tarjeta.Width,
                        "Ancho fijo de tarjeta");
            }

            if (formulario is Visual.frmEventosCliente)
            {
                foreach (string nombre in new[] { "marcoBusqueda", "cmbCategoria", "dtpDesde" })
                {
                    var filtro = Descendientes(formulario).Single(control => control.Name == nombre);
                    Assert.AreEqual(Px(Visual.Tema.AltoCampo), filtro.Height, nombre);
                    Assert.AreEqual(Visual.Tema.Cuerpo, filtro.Font, nombre);
                }
            }

            if (formulario is Visual.frmReportes || formulario is Visual.frmAuditoria)
            {
                foreach (Control selector in Descendientes(formulario).Where(c => c.Visible && (c is ComboBox || c is DateTimePicker)))
                    Assert.AreEqual(Px(Visual.Tema.AltoCampo), selector.Height, "Alto del selector " + selector.Name);
            }

            var acciones = formulario is Visual.frmCambiarContraseña ? new[] { "btnVolver", "btnconfirmar" }
                : formulario is Visual.frmPago ? new[] { "btnCancelar", "btnPagar" }
                : formulario is Visual.frmSuscripcion ? new[] { "btnCancelar", "btnContratar" }
                : Array.Empty<string>();
            if (acciones.Length > 0)
            {
                var botones = acciones.Select(nombre => formulario.Controls.Find(nombre, true).OfType<Button>().Single()).ToArray();
                Assert.AreEqual(botones[0].Width, botones[1].Width, "Anchos del grupo de acciones");
                Assert.IsTrue(botones.All(b => b.Height == Px(36) && b.Width >= Px(160)),
                    "Cada botón de acción mide 36 px de alto y al menos 160 px de ancho.");
            }

            foreach (var grupo in Descendientes(formulario).OfType<FlowLayoutPanel>()
                .Where(p => p.Visible && p.FlowDirection == FlowDirection.LeftToRight))
            {
                var botones = grupo.Controls.OfType<Button>().Where(b => b.Visible).ToArray();
                if (botones.Length < 2) continue;
                Assert.IsTrue(botones.All(b => b.Width == botones[0].Width),
                    "Las acciones del grupo " + grupo.Name + " tienen que usar un mismo ancho.");
            }
        }

        private static void VerificarFilaAviso(Visual.frmCambiarContraseña formulario)
        {
            var fila = (Panel)formulario.Controls["filaAvisoValidacion"];
            var campo = (TextBox)formulario.Controls["txtMail"];
            Assert.AreEqual(0, fila.Height);
            Assert.IsFalse(formulario.ControlarTxt());
            Assert.IsTrue(fila.Visible && fila.Height > 0, "La fila debe reservar espacio para el aviso.");
            foreach (Control control in formulario.Controls)
            {
                if (control == fila || !control.Visible) continue;
                Assert.IsFalse(control.Bounds.IntersectsWith(fila.Bounds), "El aviso se superpone con " + control.Name);
            }
            campo.Text = "prueba@eventaura.com";
            Assert.AreEqual(0, fila.Height, "La altura se libera al corregir el campo.");
            campo.Clear();
        }

        private static void CapturarSiSeSolicito(Form formulario)
        {
            string carpeta = Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI");
            if (string.IsNullOrWhiteSpace(carpeta)) return;
            string dpiEsperado = Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_DPI");
            Assert.IsTrue(int.TryParse(dpiEsperado, out int dpi) && (dpi == 96 || dpi == 144),
                "Especificá el DPI real esperado (96 o 144).");
            int dpiReal = (int)GetDpiForWindow(formulario.Handle);
            Assert.AreEqual(dpi, dpiReal, $"DPI informado por Windows, WinForms informa {formulario.DeviceDpi}");
            Console.WriteLine($"{formulario.Name}: DPI Windows={dpiReal}, DPI WinForms={formulario.DeviceDpi}");
            ActivarVentana(formulario);
            var monitor = Screen.FromControl(formulario);
            var area = monitor.WorkingArea;
            if (formulario.Width > area.Width || formulario.Height > area.Height) area = monitor.Bounds;
            Assert.IsTrue(formulario.Width <= area.Width && formulario.Height <= area.Height,
                "La ventana tiene que caber entera en el monitor para capturarla.");
            formulario.Location = new Point(
                Math.Max(area.Left, Math.Min(formulario.Left, area.Right - formulario.Width)),
                Math.Max(area.Top, Math.Min(formulario.Top, area.Bottom - formulario.Height)));
            Application.DoEvents();
            var vistasWeb = Descendientes(formulario).OfType<Visual.Web.PanelWeb>().ToArray();
            var esperaWeb = Stopwatch.StartNew();
            while (vistasWeb.Any(vista => !vista.InicializacionTerminada) && esperaWeb.ElapsedMilliseconds < 10000)
            {
                Application.DoEvents();
                Thread.Sleep(20);
            }
            foreach (var vista in vistasWeb)
                Console.WriteLine(formulario.GetType().Name + ": " + vista.Name + " preparado=" + vista.Preparado +
                    " inicializado=" + vista.InicializacionTerminada + " visible=" + vista.Visible + " bounds=" + vista.Bounds);
            if (formulario is Visual.FrmCompraCliente)
            {
                var compraWeb = vistasWeb.Single();
                if (compraWeb.Preparado)
                {
                    var totalVisible = compraWeb.EjecutarScriptAsync(
                        "document.querySelector('.total').getBoundingClientRect().bottom <= window.innerHeight");
                    var totalAnclado = compraWeb.EjecutarScriptAsync(
                        "getComputedStyle(document.querySelector('.resumen')).position === 'sticky' && getComputedStyle(document.querySelector('.total')).gridRowStart === '4'");
                    var esperaCompra = Stopwatch.StartNew();
                    while ((!totalVisible.IsCompleted || !totalAnclado.IsCompleted) && esperaCompra.ElapsedMilliseconds < 3000)
                    {
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }
                    Assert.AreEqual("true", totalVisible.GetAwaiter().GetResult(),
                        "El TOTAL del armador debe quedar visible en la ventana capturada.");
                    Assert.AreEqual("true", totalAnclado.GetAwaiter().GetResult(),
                        "El resumen debe conservar el panel fijo y el TOTAL anclado al pie.");
                }
            }
            Application.DoEvents();
            var bounds = formulario.RectangleToScreen(formulario.ClientRectangle);
            Directory.CreateDirectory(carpeta);
            using (var imagen = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics lienzo = Graphics.FromImage(imagen))
                {
                    var hdc = lienzo.GetHdc();
                    try
                    {
                        Assert.IsTrue(PrintWindow(formulario.Handle, hdc, 1), "Windows debe dibujar el área cliente.");
                    }
                    finally { lienzo.ReleaseHdc(hdc); }
                }
                foreach (var vista in vistasWeb.Where(v => v.Visible && v.Preparado))
                {
                    using (var memoria = new MemoryStream())
                    {
                        var captura = vista.CapturarAsync(memoria);
                        var espera = Stopwatch.StartNew();
                        while (!captura.IsCompleted && espera.ElapsedMilliseconds < 5000)
                        {
                            Application.DoEvents();
                            Thread.Sleep(20);
                        }
                        Assert.IsTrue(captura.IsCompleted, "WebView2 debe devolver la captura.");
                        captura.GetAwaiter().GetResult();
                        memoria.Position = 0;
                        using (var capturaWeb = Image.FromStream(memoria))
                        using (var lienzo = Graphics.FromImage(imagen))
                        {
                            var destino = vista.RectangleToScreen(vista.ClientRectangle);
                            destino.Offset(-bounds.X, -bounds.Y);
                            lienzo.DrawImage(capturaWeb, destino);
                        }
                    }
                    Console.WriteLine(formulario.GetType().Name + ": contenido WebView2 capturado.");
                }
                imagen.Save(Path.Combine(carpeta, formulario.GetType().Name + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);


        private static IEnumerable<Control> Descendientes(Control raiz)
        {
            foreach (Control hijo in raiz.Controls)
            {
                yield return hijo;
                foreach (Control nieto in Descendientes(hijo)) yield return nieto;
            }
        }
        private static IEnumerable<Control> DescendientesEnOrden(Control raiz)
        {
            foreach (Control hijo in raiz.Controls.Cast<Control>().OrderBy(c => c.Top).ThenBy(c => c.Left))
            {
                yield return hijo;
                foreach (Control nieto in DescendientesEnOrden(hijo)) yield return nieto;
            }
        }
        private static Control FocoProfundo(Control raiz)
        {
            if (raiz is DataGridView grilla && grilla.ContainsFocus) return grilla;
            foreach (Control hijo in raiz.Controls)
            {
                if (hijo.Focused) return hijo;
                if (hijo.ContainsFocus) return FocoProfundo(hijo);
            }
            return raiz is Form f && f.Focused ? f : null;
        }
        private static bool EsCampoInicial(Control c) => c.Visible && c.Enabled && c.TabStop &&
            !(c is Button) && !(c is LinkLabel) && (c is TextBoxBase && !((TextBoxBase)c).ReadOnly ||
            c is ComboBox || c is NumericUpDown || c is DateTimePicker || c is DataGridView || c is RadioButton || c is CheckBox || c is ListBox);
        private static bool EsInteractivoVisible(Control c) => c.Visible && c.Enabled &&
            (c is Button || c is TextBoxBase || c is ComboBox || c is NumericUpDown || c is DateTimePicker || c is DataGridView || c is CheckBox || c is RadioButton || c is ListBox);

        private static void CerrarDialogosDePrueba()
        {
            int procesoPruebas = Process.GetCurrentProcess().Id;
            EnumWindows((hwnd, lparam) =>
            {
                var clase = new System.Text.StringBuilder(128);
                GetClassName(hwnd, clase, clase.Capacity);
                GetWindowThreadProcessId(hwnd, out uint proceso);
                if (proceso == procesoPruebas && clase.ToString() == "#32770") PostMessage(hwnd, 0x0010, IntPtr.Zero, IntPtr.Zero);
                return true;
            }, IntPtr.Zero);
        }

        private delegate bool EnumVentana(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")] private static extern bool EnumWindows(EnumVentana callback, IntPtr lparam);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint idProceso);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern int GetClassName(IntPtr hwnd, System.Text.StringBuilder clase, int maximo);
        [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr hwnd, int mensaje, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hwnd);
    }
}
