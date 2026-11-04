using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasInterfaz
    {
        // Regresión de F-01 a F-03 (pruebas manuales del 03/11/2026) con datos aislados; nunca usa la base de desarrollo.
        [TestMethod]
        public void Compra_PaqueteMaximoYNombreLargoSinRecortes()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Plata);
                var evento = GestorEventos.Instance.ObtenerPorId(1); // Festival: tres sectores de la semilla.
                using (var form = new Visual.FrmCompraCliente { IDEvento = evento.Id, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    var titulo = Control<Label>(form, "label1");
                    titulo.Text = "Gran Festival Internacional de Música Independiente de Rosario con artistas nacionales e invitados VIP";
                    var grilla = Control<DataGridView>(form, "dgvSectores");
                    foreach (DataGridViewRow fila in grilla.Rows)
                        fila.Cells["Cantidad"].Value = 2;
                    foreach (string nombre in new[] { "chkHospedaje", "chkTraslado", "chkMerch", "chkVIP" })
                        Control<CheckBox>(form, nombre).Checked = true;
                    Control<CheckBox>(form, "chkSeguro").Checked = true;
                    var resumen = Control<Label>(form, "lblResumen");
                    var panel = Control<Panel>(form, "pnlResumen");
                    form.PerformLayout();
                    panel.PerformLayout();
                    Assert.IsFalse(titulo.Bounds.IntersectsWith(panel.Bounds), "El título invade el resumen.");
                    Assert.IsTrue(titulo.GetPreferredSize(new Size(titulo.Width, 0)).Height <= titulo.Height, "El título largo se corta verticalmente.");
                    Assert.IsFalse(Control<Label>(form, "lblNivel").Bounds.IntersectsWith(Control<LinkLabel>(form, "lnkNiveles").Bounds));
                    Assert.IsTrue(resumen.Text.Contains("TOTAL") && resumen.Text.Contains("de las entradas."));
                    foreach (string linea in resumen.Text.Split('\n'))
                    {
                        int ancho = TextRenderer.MeasureText(linea.TrimEnd('\r'), resumen.Font, Size.Empty, TextFormatFlags.NoPadding).Width;
                        Assert.IsTrue(ancho <= resumen.Width - resumen.Padding.Horizontal, $"Línea recortada ({ancho} > {resumen.Width} - {resumen.Padding.Horizontal}, panel {panel.Width}): " + linea);
                    }
                    Assert.IsTrue(resumen.Height <= panel.ClientSize.Height, "El paquete máximo oculta el total.");
                    foreach (var check in Descendientes(form).OfType<CheckBox>())
                    {
                        Assert.IsTrue(check.GetPreferredSize(Size.Empty).Width <= check.Width, "Casilla recortada: " + check.Text);
                    }
                    string carpeta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Interfaz");
                    Directory.CreateDirectory(carpeta);
                    using (var imagen = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(imagen, new Rectangle(0, 0, form.Width, form.Height));
                        imagen.Save(Path.Combine(carpeta, "compra-paquete-maximo.png"));
                    }
                }
                using (var agotado = new Visual.FrmCompraCliente { IDEvento = 2, IDUsuario = cliente.Id })
                {
                    Cargar(agotado);
                    foreach (string nombre in new[] { "chkHospedaje", "chkTraslado", "chkMerch", "chkVIP" })
                    {
                        var check = Control<CheckBox>(agotado, nombre);
                        Assert.IsFalse(check.Enabled);
                        Assert.IsTrue(check.Text.Contains("no disponible"));
                        Assert.IsTrue(check.GetPreferredSize(Size.Empty).Width <= check.Width);
                    }
                }
            });
        }

        [TestMethod]
        public void Registro_AbreSinDatosPrecargados()
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmCrearUsuario())
                {
                    Cargar(form);
                    Assert.AreEqual("frmCrearUsuario", form.Name);
                    foreach (string nombre in new[] { "txtNombre", "txtApellido", "txtmail", "TxtPassword", "TxtConfirmpassword" })
                        Assert.AreEqual("", Control<TextBox>(form, nombre).Text, nombre);
                }
            });
        }

        [TestMethod]
        public void Eventos_SinResultadosNoPermiteAgregarFilas()
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmEventosCliente())
                {
                    Cargar(form);
                    var grilla = Control<DataGridView>(form, "dgveventos");
                    Assert.AreEqual("Categoría", grilla.Columns["Categoria"].HeaderText);
                    Assert.AreEqual("Ubicación", grilla.Columns["Ubicacion"].HeaderText);
                    Assert.AreEqual(DataGridViewAutoSizeColumnsMode.Fill, grilla.AutoSizeColumnsMode);
                    form.GetType().GetMethod("CargarEventos", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(form, new object[] { "evento-inexistente-prueba", null, null });
                    Assert.AreEqual(1, grilla.Rows.Count);
                    Assert.IsFalse(grilla.AllowUserToAddRows);
                    Assert.IsTrue(grilla.ReadOnly);
                    Assert.IsTrue(grilla.Rows[0].Cells[0].Value.ToString().Contains("No hay eventos"));
                }
            });
        }

        [TestMethod]
        public void Historial_SinSeleccionDistingueBotonDeshabilitado()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.frmHistorialCliente { IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    var boton = Control<Button>(form, "btnDevolver");
                    Assert.IsFalse(boton.Enabled);
                    Assert.AreEqual(Color.Gainsboro, boton.BackColor);
                }
            });
        }

        [TestMethod]
        public void Eventos_ComprarSinResultadosMuestraAvisoCorrecto()
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmEventosCliente())
                {
                    Cargar(form);
                    Control<TextBox>(form, "txtBuscar").Text = "evento-inexistente-prueba";
                    Control<Button>(form, "btnBuscar").PerformClick();
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnComprar").PerformClick(),
                        new DialogoEsperado { Titulo = "Seleccionar evento", Texto = "Debe seleccionar un evento para comprar" });
                    Assert.IsTrue(form.Visible);
                    Assert.IsFalse(Application.OpenForms.Cast<Form>().Any(f => f is Visual.FrmCompraCliente));
                }
            });
        }

        [TestMethod]
        public void Compra_VolverConfirmaSinPerderPaqueteAlCancelar()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Plata);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    var grilla = Control<DataGridView>(form, "dgvSectores");
                    grilla.Rows[0].Cells["Cantidad"].Value = 2;
                    string resumen = Control<Label>(form, "lblResumen").Text;
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnVolver").PerformClick(),
                        Confirmacion("Volver al listado", "se descarta el paquete", DialogResult.No));
                    Assert.IsTrue(form.Visible);
                    Assert.AreEqual(resumen, Control<Label>(form, "lblResumen").Text);
                    Assert.AreEqual(2, grilla.Rows[0].Cells["Cantidad"].Value);
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnVolver").PerformClick(),
                        Confirmacion("Volver al listado", "se descarta el paquete", DialogResult.Yes));
                    Assert.IsFalse(form.Visible);
                    Assert.IsTrue(Application.OpenForms.Cast<Form>().Any(f => f is Visual.frmEventosCliente && f.Visible));
                }
            });
        }

        [DataTestMethod]
        [DataRow("abc")]
        [DataRow("-1")]
        public void Compra_CantidadInvalidaRestauraElPaquete(string cantidad)
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    var celda = Control<DataGridView>(form, "dgvSectores").Rows[0].Cells["Cantidad"];
                    celda.Value = 2;
                    string resumen = Control<Label>(form, "lblResumen").Text;
                    DialogosPrueba.Comprobar(() => celda.Value = cantidad,
                        new DialogoEsperado { Titulo = "", Texto = "La cantidad debe ser un número entero de 0 en adelante." });
                    Assert.AreEqual(2, celda.Value);
                    Assert.AreEqual(resumen, Control<Label>(form, "lblResumen").Text);
                }
            });
        }

        [DataTestMethod]
        [DataRow("txtNombre", " ", "", "Ingrese su nombre.")]
        [DataRow("txtApellido", " ", "", "Ingrese su apellido.")]
        [DataRow("txtmail", " ", "", "Ingrese su correo electrónico.")]
        [DataRow("TxtPassword", "abc", "No se pudo crear la cuenta", "La contraseña debe tener al menos")]
        public void Registro_RechazaDatosInvalidosSinCrearUsuario(string campo, string valor, string titulo, string mensaje)
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmCrearUsuario())
                {
                    Cargar(form);
                    const string correo = "testvalidacion@pruebas.eventaura.com";
                    Control<TextBox>(form, "txtNombre").Text = "Cliente";
                    Control<TextBox>(form, "txtApellido").Text = "Prueba";
                    Control<TextBox>(form, "txtmail").Text = correo;
                    Control<TextBox>(form, "TxtPassword").Text = "Cliente123";
                    Control<TextBox>(form, "TxtConfirmpassword").Text = "Cliente123";
                    Control<TextBox>(form, campo).Text = valor;
                    if (campo == "TxtPassword") Control<TextBox>(form, "TxtConfirmpassword").Text = valor;
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnregistrarse").PerformClick(),
                        new DialogoEsperado { Titulo = titulo, Texto = mensaje });
                    Assert.IsTrue(form.Visible);
                    Assert.IsNull(Datos.RepositorioUsuarios.Instance.ObtenerPorMail(correo));
                }
            });
        }

        [TestMethod]
        public void Propietario_EliminarRequiereConfirmacionYPreservaVentas()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                var evento = Fake.EventoPublicado(1000m, 5);
                using (var form = new Visual.Vendedor.frmEventosLocal { IDlocal = evento.IdLocal, IDUsuario = 1 })
                {
                    Cargar(form);
                    var grilla = Control<DataGridView>(form, "dgvEventos");
                    Assert.IsTrue(grilla.Rows.Count > 0, "No se cargaron los eventos del local.");
                    grilla.CurrentCell = grilla.Rows[0].Cells["Nombre"];
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnEliminarEvento").PerformClick(),
                        Confirmacion("Eliminar evento", evento.Nombre, DialogResult.No));
                    Assert.IsNotNull(GestorEventos.Instance.ObtenerPorId(evento.Id));
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnEliminarEvento").PerformClick(),
                        Confirmacion("Eliminar evento", evento.Nombre, DialogResult.Yes));
                    Assert.IsNull(GestorEventos.Instance.ObtenerPorId(evento.Id));
                }
                evento = Fake.EventoPublicado(1000m, 5);
                Fake.CompraConfirmada(cliente, evento, 1, false);
                using (var form = new Visual.Vendedor.frmEventosLocal { IDlocal = evento.IdLocal, IDUsuario = 1 })
                {
                    Cargar(form);
                    var grilla = Control<DataGridView>(form, "dgvEventos");
                    Assert.IsTrue(grilla.Rows.Count > 0, "No se cargó el evento con venta.");
                    grilla.CurrentCell = grilla.Rows[0].Cells["Nombre"];
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnEliminarEvento").PerformClick(),
                        Confirmacion("Eliminar evento", evento.Nombre, DialogResult.Yes),
                        Confirmacion("RN-11", "¿Desea suspenderlo?", DialogResult.No));
                    Assert.AreEqual(GestorEventos.EstadoPublicado, GestorEventos.Instance.ObtenerPorId(evento.Id).Estado);
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnEliminarEvento").PerformClick(),
                        Confirmacion("Eliminar evento", evento.Nombre, DialogResult.Yes),
                        Confirmacion("RN-11", "¿Desea suspenderlo?", DialogResult.Yes));
                    Assert.AreEqual(GestorEventos.EstadoSuspendido, GestorEventos.Instance.ObtenerPorId(evento.Id).Estado);
                }
            });
        }

        [TestMethod]
        public void Historial_TildarYDestildarActualizaEstadoYColor()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                var evento = Fake.EventoPublicado(1000m, 5);
                Fake.CompraConfirmada(cliente, evento, 1, true);
                using (var form = new Visual.frmHistorialCliente { IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    var grilla = Control<DataGridView>(form, "dgvEntradas");
                    var boton = Control<Button>(form, "btnDevolver");
                    Assert.IsFalse(boton.Enabled);
                    grilla.Rows[0].Cells["Devolver"].Value = true;
                    Assert.IsTrue(boton.Enabled);
                    Assert.AreEqual("Devolver 1 entrada(s)", boton.Text);
                    Assert.AreEqual(Color.FromArgb(116, 86, 174), boton.BackColor);
                    grilla.Rows[0].Cells["Devolver"].Value = false;
                    Assert.IsFalse(boton.Enabled);
                    Assert.AreEqual(Color.Gainsboro, boton.BackColor);
                }
            });
        }

        [TestMethod]
        public void Compra_CupoLiberadoVuelveAHabilitarServiciosDisponibles()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                var evento = Fake.EventoPublicado(1000m, 1);
                Fake.ServicioHabilitado(evento, "Hospedaje", 45000m);
                var compra = Fake.CompraConfirmada(cliente, evento, 1, true);
                using (var form = new Visual.FrmCompraCliente { IDEvento = evento.Id, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    Assert.IsFalse(Control<CheckBox>(form, "chkHospedaje").Enabled);
                    Assert.IsFalse(Control<Button>(form, "btnComprar").Enabled);
                    GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(compra));
                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnListaEspera").PerformClick(),
                        new DialogoEsperado { Titulo = "RN-10", Texto = "El evento todavía tiene entradas" });
                    Assert.IsTrue(Control<DataGridView>(form, "dgvSectores").Visible);
                    Assert.IsTrue(Control<Button>(form, "btnComprar").Enabled);
                    Assert.IsTrue(Control<CheckBox>(form, "chkHospedaje").Enabled, "El servicio sigue bloqueado después de liberarse el cupo.");
                    Assert.IsFalse(Control<CheckBox>(form, "chkTraslado").Enabled, "No se debe habilitar un servicio que el evento no ofrece.");
                    Assert.IsFalse(GestorListaEspera.Instance.EstaInscripto(evento.Id, cliente.Id));
                }
            });
        }

        private static DialogoEsperado Confirmacion(string titulo, string texto, DialogResult respuesta) =>
            new DialogoEsperado { Titulo = titulo, Texto = texto, Predeterminado = DialogResult.No, Respuesta = respuesta };

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr ventana, int mensaje, StringBuilder texto, IntPtr longitud);

        [TestMethod]
        public void Eventos_GuiaRegistradaConFocoEnLaBusqueda()
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmEventosCliente())
                {
                    Cargar(form);
                    var caja = Control<TextBox>(form, "txtBuscar");
                    caja.Focus();
                    Application.DoEvents();
                    var texto = new StringBuilder(256);
                    SendMessage(caja.Handle, 0x1502, texto, new IntPtr(texto.Capacity)); // EM_GETCUEBANNER
                    Assert.AreEqual("Evento o dirección del local", texto.ToString());
                    Assert.IsTrue(caja.Focused);
                    string carpeta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Interfaz");
                    Directory.CreateDirectory(carpeta);
                    using (var imagen = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(imagen, new Rectangle(0, 0, form.Width, form.Height));
                        imagen.Save(Path.Combine(carpeta, "eventos-guia-con-foco.png"));
                    }
                }
            });
        }

        [DataTestMethod]
        [DataRow(1.25f, 1366, 728)]
        [DataRow(1.50f, 1366, 728)]
        [DataRow(1.50f, 1920, 1032)]
        public void Compra_EscalaSimuladaMantieneAccionesDentroDelArea(float factor, int ancho, int alto)
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Plata);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    Control<Label>(form, "label1").Text = "Gran Festival Internacional de Música Independiente de Rosario con artistas nacionales e invitados VIP";
                    foreach (DataGridViewRow fila in Control<DataGridView>(form, "dgvSectores").Rows)
                        fila.Cells["Cantidad"].Value = 2;
                    foreach (string nombre in new[] { "chkHospedaje", "chkTraslado", "chkMerch", "chkVIP", "chkSeguro" })
                        Control<CheckBox>(form, nombre).Checked = true;
                    var controles = Descendientes(form).Prepend(form).ToArray();
                    var fuentes = controles.Select(c => c.Font).ToArray();
                    form.Scale(new SizeF(factor, factor));
                    // Escalar tamaño y raster de texto en una pantalla física de 96 DPI.
                    // Es simulación geométrica; no cambia la configuración de Windows.
                    for (int i = 0; i < controles.Length; i++)
                        controles[i].Font = new Font(fuentes[i].FontFamily, fuentes[i].SizeInPoints * factor, fuentes[i].Style);
                    form.GetType().GetMethod("AjustarAreaVisible", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(form, new object[] { new Rectangle(0, 0, ancho, alto) });
                    form.PerformLayout();
                    Assert.IsTrue(form.Width <= ancho && form.Height <= alto,
                        $"La compra mide {form.Width} × {form.Height} para un área de {ancho} × {alto} a {factor:P0}.");
                    var titulo = Control<Label>(form, "label1");
                    Assert.IsTrue(titulo.GetPreferredSize(new Size(titulo.Width, 0)).Height <= titulo.Height, "Título recortado en la escala simulada.");
                    var resumen = Control<Label>(form, "lblResumen");
                    foreach (string linea in resumen.Text.Split('\n'))
                        Assert.IsTrue(TextRenderer.MeasureText(linea.TrimEnd('\r'), resumen.Font, Size.Empty, TextFormatFlags.NoPadding).Width <= resumen.Width - resumen.Padding.Horizontal,
                            "Importe recortado en escala simulada: " + linea);
                    var contenido = Control<Panel>(form, "pnlContenidoCompra");
                    var panelResumen = Control<Panel>(form, "pnlResumen");
                    contenido.ScrollControlIntoView(panelResumen);
                    Assert.IsTrue(panelResumen.Bottom <= contenido.ClientSize.Height, "No se puede llegar al final del resumen con desplazamiento.");
                    foreach (string nombre in new[] { "btnComprar", "btnVolver" })
                    {
                        var boton = Control<Button>(form, nombre);
                        var area = form.RectangleToClient(boton.Parent.RectangleToScreen(boton.Bounds));
                        Assert.IsTrue(form.ClientRectangle.Contains(area), $"Acción fuera de pantalla: {nombre}, {area} dentro de {form.ClientRectangle}.");
                    }
                    string carpeta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Interfaz");
                    Directory.CreateDirectory(carpeta);
                    using (var imagen = new Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(imagen, new Rectangle(0, 0, form.Width, form.Height));
                        imagen.Save(Path.Combine(carpeta, $"compra-simulada-{(int)(factor * 100)}-{ancho}x{alto}.png"));
                    }
                }
            });
        }

        private static System.Collections.Generic.IEnumerable<Control> Descendientes(Control padre)
        {
            foreach (Control control in padre.Controls)
            {
                yield return control;
                foreach (Control hijo in Descendientes(control)) yield return hijo;
            }
        }

        private static T Control<T>(Form form, string nombre) where T : Control => (T)form.Controls.Find(nombre, true).Single();
        private static void Cargar(Form form)
        {
            form.Show();
            Application.DoEvents();
        }
        private static void EnSta(Action accion)
        {
            Exception error = null;
            var hilo = new Thread(() =>
            {
                try { Application.EnableVisualStyles(); accion(); }
                catch (Exception ex) { error = ex; }
                finally
                {
                    foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                    {
                        if (form.InvokeRequired) continue; // no tocar formularios de otro hilo/proceso de prueba
                        form.Dispose();
                    }
                }
            }) { IsBackground = true };
            hilo.SetApartmentState(ApartmentState.STA);
            hilo.Start();
            Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(30)), "La interfaz no terminó en 30 segundos.");
            Fake.Limpiar();
            if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        }
    }
}
