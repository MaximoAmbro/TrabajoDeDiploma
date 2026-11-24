using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public sealed class PruebasInteraccionPantallas
    {
        public TestContext TestContext { get; set; }

        // CP-357 — RNF-02/RNF-03: interacción WinForms real, foco, teclado, y geometría en ventanas abiertas.
        [TestMethod("CP-357: Teclado y geometría de las 25 ventanas reales")]
        public void CP357_InteraccionRealDeLasPantallas()
        {
            var tipos = typeof(Visual.FormularioEventAura).Assembly.GetTypes()
                .Where(t => typeof(Form).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(Visual.FormularioEventAura))
                .OrderBy(t => t.Name).ToArray();
            Assert.AreEqual(25, tipos.Length);
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
                            Application.DoEvents();
                            Thread.Sleep(30);
                            Application.DoEvents();

                            if (formulario is Visual.frmReportes &&
                                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")))
                            {
                                ((DateTimePicker)formulario.Controls["dtpHasta"]).Value = DateTime.Today.AddMonths(3);
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
            Control esperado = DescendientesEnOrden(formulario).FirstOrDefault(EsCampoInicial);
            Control actual = FocoProfundo(formulario);
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
                var demostracion = eventos.FirstOrDefault(evento =>
                {
                    var sectores = GestorEventos.Instance.Sectores(evento.Id);
                    return sectores.Count > 0 && sectores.All(sector => GestorPaquetes.Instance.DisponiblesDe(sector.Id) >= 2);
                });
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
