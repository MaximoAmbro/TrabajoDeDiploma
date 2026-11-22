using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
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
                            PrepararSesion(tipo);
                            formulario = (Form)Activator.CreateInstance(tipo);
                            PrepararContextoSeleccionado(formulario);
                            formulario.Show();
                            Application.DoEvents();
                            Thread.Sleep(30);
                            Application.DoEvents();

                            string foco = VerificarFoco(formulario);
                            string geometria = VerificarGeometria(formulario);
                            string teclado = VerificarTeclas(formulario, tipo);
                            if (EsMenu(tipo)) geometria += "; 150% simulado: " + VerificarMenuEscala150(tipo);
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
            var eventos = idLocal > 0 ? GestorEventos.Instance.PorLocal(idLocal) : GestorEventos.Instance.Vigentes();
            int idEvento = eventos.FirstOrDefault()?.Id ?? 0;
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
                    throw new AssertFailedException(control.Name + " queda fuera de " + control.Parent.Name + " tras maximizar/redimensionar.");
            }
            foreach (var grupo in Descendientes(formulario).GroupBy(c => c.Parent))
            {
                var interactivos = grupo.Where(EsInteractivoVisible).ToArray();
                for (int i = 0; i < interactivos.Length; i++)
                    for (int j = i + 1; j < interactivos.Length; j++)
                        if (interactivos[i].Bounds.IntersectsWith(interactivos[j].Bounds))
                            throw new AssertFailedException(interactivos[i].Name + " se superpone con " + interactivos[j].Name + ".");
            }
            return "ventana: OK (" + formulario.DeviceDpi + " DPI; " + formulario.FormBorderStyle + ")";
        }

        private static string VerificarMenuEscala150(Type tipo)
        {
            using (var formulario = CrearVentana(tipo))
            {
                formulario.Show();
                Application.DoEvents();
                formulario.Scale(new SizeF(1.5f, 1.5f));
                formulario.PerformLayout();
                Application.DoEvents();
                foreach (var grupo in Descendientes(formulario).GroupBy(c => c.Parent))
                {
                    var interactivos = grupo.Where(EsInteractivoVisible).ToArray();
                    for (int i = 0; i < interactivos.Length; i++)
                        for (int j = i + 1; j < interactivos.Length; j++)
                            if (interactivos[i].Bounds.IntersectsWith(interactivos[j].Bounds))
                                throw new AssertFailedException("Escala 150%: " + interactivos[i].Name + " se superpone con " + interactivos[j].Name + ".");
                }
                return "sin superposición de controles interactivos; simulación a 96 DPI";
            }
        }

        private static bool EsMenu(Type tipo) => tipo.Name == "frmMenuCliente" || tipo.Name == "frmMenuPropietario" || tipo.Name == "frmMenuAdministrador";

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
