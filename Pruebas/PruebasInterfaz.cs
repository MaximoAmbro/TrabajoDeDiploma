using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
        // CP-13 — RF-05, RN-10, H-01: paquete máximo y título largo sin recortes.
        [TestMethod("CP-13: Paquete máximo y título largo sin recortes")]
        public void Compra_PaqueteMaximoYNombreLargoSinRecortes()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Plata);
                var evento = GestorEventos.Instance.ObtenerPorId(1); // Duki en Rosario: tres sectores de la semilla.
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

        // CP-14 — RF-01: registro sin datos precargados.
        [TestMethod("CP-14: Registro sin datos precargados")]
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

        // CP-16 — RF-03: búsqueda sin filas editables.
        [TestMethod("CP-16: Búsqueda sin filas editables")]
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

        // CP-22 — RF-05: devolver deshabilitado sin selección.
        [TestMethod("CP-22: Devolver deshabilitado sin selección")]
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

        // CP-17 — RF-03: aviso al comprar sin resultados.
        [TestMethod("CP-17: Aviso al comprar sin resultados")]
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

        // CP-19 — RF-05, H-06: confirmación al descartar el paquete.
        [TestMethod("CP-19: Confirmación al descartar el paquete")]
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

        // CP-20 — RF-04: cantidad inválida restaura el paquete.
        [DataTestMethod]
        [DataRow("abc", DisplayName = "CP-20: Cantidad inválida restaura el paquete (variante 1)")]
        [DataRow("-1", DisplayName = "CP-20: Cantidad inválida restaura el paquete (variante 2)")]
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
                        new DialogoEsperado { Titulo = "Revisá los datos", Texto = "La cantidad debe ser un número entero de 0 en adelante." });
                    Assert.AreEqual(2, celda.Value);
                    Assert.AreEqual(resumen, Control<Label>(form, "lblResumen").Text);
                }
            });
        }

        // CP-15 — RF-01, RN-02: registro rechaza datos inválidos.
        [DataTestMethod]
        [DataRow("txtNombre", " ", "", "Ingrese su nombre.", DisplayName = "CP-15: Registro rechaza datos inválidos (variante 1)")]
        [DataRow("txtApellido", " ", "", "Ingrese su apellido.", DisplayName = "CP-15: Registro rechaza datos inválidos (variante 2)")]
        [DataRow("txtmail", " ", "", "Ingrese su correo electrónico.", DisplayName = "CP-15: Registro rechaza datos inválidos (variante 3)")]
        [DataRow("TxtPassword", "abc", "No se pudo crear la cuenta", "La contraseña debe tener al menos", DisplayName = "CP-15: Registro rechaza datos inválidos (variante 4)")]
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

        // CP-21 — RF-12, RN-11: confirmación y protección de eventos con ventas.
        [TestMethod("CP-21: Confirmación y protección de eventos con ventas")]
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

        // CP-23 — RF-05: selección actualiza devolver.
        [TestMethod("CP-23: Selección actualiza Devolver")]
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

        // CP-24 — RF-05, RF-10, RN-10, RN-14: servicios al recuperar cupo.
        [TestMethod("CP-24: Servicios al recuperar cupo")]
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
                        new DialogoEsperado { Titulo = "No se puede armar el paquete", Texto = "El evento todavía tiene entradas" });
                    Assert.IsTrue(Control<DataGridView>(form, "dgvSectores").Visible);
                    Assert.IsFalse(Control<Button>(form, "btnComprar").Enabled);
                    Control<DataGridView>(form, "dgvSectores").Rows[0].Cells["Cantidad"].Value = 1;
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

        // CP-18 — RF-03: guía de búsqueda con foco.
        [TestMethod("CP-18: Guía de búsqueda con foco")]
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

        // CP-25 — RF-05, RNF-03: acciones visibles en escala simulada.
        [DataTestMethod]
        [DataRow(1.25f, 1366, 728, DisplayName = "CP-25: Acciones visibles en escala simulada (variante 1)")]
        [DataRow(1.50f, 1366, 728, DisplayName = "CP-25: Acciones visibles en escala simulada (variante 2)")]
        [DataRow(1.50f, 1920, 1032, DisplayName = "CP-25: Acciones visibles en escala simulada (variante 3)")]
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

        // CP-39 — RF-06, RN-15, H-02: cuenta regresiva visible y aviso rojo durante el último minuto.
        [TestMethod("CP-39: Temporizador de reserva normal y en el último minuto")]
        public void CP39_TemporizadorDeReservaALaVista()
        {
            EnSta(() =>
            {
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                int minutos = GestorParametros.Instance.ObtenerEntero("RESERVA_MINUTOS");
                Compra compra = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 1, false), cliente);
                using (var form = new Visual.frmPago { Compra = compra })
                {
                    Cargar(form);
                    Label temporizador = Control<Label>(form, "lblTemporizador");
                    Match formato = Regex.Match(temporizador.Text, @"^Reserva: (\d{2}):(\d{2})$");
                    Assert.IsTrue(formato.Success, "Formato de cuenta regresiva incorrecto: " + temporizador.Text);
                    Assert.IsTrue(int.Parse(formato.Groups[1].Value) <= minutos);
                    Assert.AreEqual(Color.FromArgb(116, 86, 174), temporizador.ForeColor);
                    Assert.IsTrue(temporizador.Visible);
                    form.Close();
                }

                Compra porVencer = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 1, false), cliente);
                porVencer.FechaCompra = DateTime.Now.AddMinutes(-minutos).AddSeconds(45);
                porVencer.ReservaVence = porVencer.FechaCompra.AddMinutes(minutos);
                using (var form = new Visual.frmPago { Compra = porVencer })
                {
                    Cargar(form);
                    Assert.AreEqual(Color.Firebrick, Control<Label>(form, "lblTemporizador").ForeColor);
                    form.Close();
                }
            });
        }

        // CP-40 — RF-04, RN-08, H-03: cupo restante por sector y edición exclusiva de Cantidad.
        [TestMethod("CP-40: Cupo restante por sector y única columna editable")]
        public void CP40_CupoRestantePorSector()
        {
            EnSta(() =>
            {
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Cliente comprador = Fake.Cliente(NivelFidelidad.Bronce);
                Cliente interesado = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.CompraConfirmada(comprador, evento, 1, false);
                using (var form = new Visual.FrmCompraCliente { IDEvento = evento.Id, IDUsuario = interesado.Id })
                {
                    Cargar(form);
                    DataGridView grilla = Control<DataGridView>(form, "dgvSectores");
                    DataGridViewRow fila = grilla.Rows.Cast<DataGridViewRow>()
                        .Single(f => Convert.ToInt32(f.Cells["IdSector"].Value) == evento.Sectores.Single().Id);
                    Assert.AreEqual(4, Convert.ToInt32(fila.Cells["Disponibles"].Value));
                    foreach (DataGridViewColumn columna in grilla.Columns)
                        Assert.AreEqual(columna.Name != "Cantidad", columna.ReadOnly, "Edición incorrecta en " + columna.Name + ".");
                }
            });
        }

        // CP-41 — RF-05, RN-28, H-04: cancelar la advertencia de compra sin seguro no reserva cupo.
        [TestMethod("CP-41: Confirmación al comprar sin seguro con No por defecto")]
        public void CP41_ConfirmacionAlComprarSinSeguro()
        {
            EnSta(() =>
            {
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.FrmCompraCliente { IDEvento = evento.Id, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    Control<DataGridView>(form, "dgvSectores").Rows[0].Cells["Cantidad"].Value = 1;
                    Assert.IsFalse(Control<CheckBox>(form, "chkSeguro").Checked);

                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnComprar").PerformClick(),
                        new DialogoEsperado
                        {
                            Titulo = "Seguro de entrada",
                            Texto = "¿Continuar sin seguro?",
                            Predeterminado = DialogResult.No,
                            Respuesta = DialogResult.No
                        });

                    Assert.AreEqual(5, GestorPaquetes.Instance.DisponiblesDe(evento.Sectores.Single().Id));
                    Assert.IsTrue(form.Visible);
                }
            });
        }

        // CP-42 — RF-08, RN-19, RN-20, H-05: enlace visible y explicación con umbrales configurados.
        [TestMethod("CP-42: Explicación de niveles con los umbrales vigentes")]
        public void CP42_ExplicacionDeLosNiveles()
        {
            EnSta(() =>
            {
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                int plata = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_PLATA");
                int oro = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_ORO");
                using (var form = new Visual.FrmCompraCliente { IDEvento = evento.Id, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    LinkLabel enlace = Control<LinkLabel>(form, "lnkNiveles");
                    Assert.IsTrue(enlace.Visible);
                    Assert.AreEqual("¿Cómo funcionan los niveles?", enlace.Text);
                    MethodInfo manejador = form.GetType().GetMethod("lnkNiveles_LinkClicked", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.IsNotNull(manejador);
                    DialogosPrueba.Comprobar(() => manejador.Invoke(form, new object[] { enlace, null }),
                        new DialogoEsperado
                        {
                            Titulo = "¿Cómo funcionan los niveles?",
                            Texto = $"Plata: de {plata} a {oro - 1} compras"
                        });
                }
            });
        }

        // CP-44 — RF-01, RN-05: el alta pública no permite elegir ni escribir un perfil privilegiado.
        [TestMethod("CP-44: Registro sin selector de perfil crea solo clientes")]
        public void CP44_AltaPublicaSinSelectorDePerfil()
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmCrearUsuario())
                {
                    Cargar(form);
                    Assert.IsFalse(Descendientes(form).OfType<ComboBox>().Any());
                    Assert.AreEqual(0, form.Controls.Find("cbTipoUsuario", true).Length);
                    string mail = Fake.MailNuevo();
                    Control<TextBox>(form, "txtNombre").Text = "Nombre";
                    Control<TextBox>(form, "txtApellido").Text = "Prueba";
                    Control<TextBox>(form, "txtmail").Text = mail;
                    Control<TextBox>(form, "TxtPassword").Text = "Cliente123";
                    Control<TextBox>(form, "TxtConfirmpassword").Text = "Cliente123";

                    DialogosPrueba.Comprobar(() => Control<Button>(form, "btnregistrarse").PerformClick(),
                        new DialogoEsperado { Titulo = "", Texto = "Usuario creado" });

                    Usuario usuario = Datos.RepositorioUsuarios.Instance.ObtenerPorMail(mail);
                    Assert.IsNotNull(usuario);
                    Assert.AreEqual("Cliente", usuario.TipoUsuario);
                }
            });
        }

        // CP-46 — RF-02: volver al inicio cierra la sesión de cliente o administrador y muestra el login.
        [DataTestMethod]
        [DataRow("cliente", DisplayName = "CP-46: Volver al inicio cierra sesión de cliente")]
        [DataRow("administrador", DisplayName = "CP-46: Volver al inicio cierra sesión de administrador")]
        public void CP46_VolverAlInicioCierraLaSesion(string perfil)
        {
            EnSta(() =>
            {
                Usuario usuario = perfil == "cliente" ? (Usuario)Fake.Cliente(NivelFidelidad.Bronce) : Fake.Administrador();
                GestorSesion.Instance.Iniciar(usuario);
                using (Form menu = perfil == "cliente"
                    ? (Form)new Visual.frmMenuCliente { IDUsuario = usuario.Id, NombreUsuario = usuario.Nombre }
                    : new Visual.frmMenuPropietario { IDUsuario = usuario.Id, NombreUsuario = usuario.Nombre })
                {
                    Cargar(menu);
                    MethodInfo volver = menu.GetType().GetMethod("lblvolver_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.IsNotNull(volver);
                    volver.Invoke(menu, new object[] { menu, EventArgs.Empty });

                    Assert.IsFalse(GestorSesion.Instance.HaySesion);
                    Assert.IsTrue(Application.OpenForms.Cast<Form>().Any(f => f is Visual.frmInicio && f.Visible));
                }
            });
        }

        // CP-47 — RNF-02: cerrar la última ventana visible termina solo un cierre solicitado por el usuario.
        [DataTestMethod]
        [DataRow(CloseReason.UserClosing, true, false, false, DisplayName = "CP-47: Otra ventana visible mantiene la aplicación")]
        [DataRow(CloseReason.UserClosing, false, false, true, DisplayName = "CP-47: Sin otras ventanas termina la aplicación")]
        [DataRow(CloseReason.UserClosing, false, true, true, DisplayName = "CP-47: Solo ventanas ocultas no mantienen la aplicación")]
        [DataRow(CloseReason.ApplicationExitCall, false, false, false, DisplayName = "CP-47: ApplicationExitCall no vuelve a pedir salida")]
        public void CP47_CierreDeUltimaVentanaVisible(CloseReason motivo, bool otraVisible, bool otraOculta, bool esperado)
        {
            EnSta(() =>
            {
                using (var cerrado = new Form())
                using (var otra = new Form())
                {
                    if (otraVisible) Cargar(otra);
                    Form[] abiertos = otraVisible || otraOculta ? new[] { cerrado, otra } : new[] { cerrado };
                    Assert.AreEqual(esperado, Visual.FormularioEventAura.DebeTerminarAplicacion(cerrado, motivo, abiertos));
                }
            });
        }

        // CP-48 — RF-05: la X confirma el descarte con No por defecto y cierra sin aviso un paquete vacío.
        [TestMethod("CP-48: Cerrar la compra confirma el descarte del paquete")]
        public void CP48_CerrarCompraConfirmaDescarte()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    DataGridViewRow campo = Control<DataGridView>(form, "dgvSectores").Rows.Cast<DataGridViewRow>()
                        .Single(f => Convert.ToString(f.Cells["Sector"].Value) == "Campo");
                    campo.Cells["Cantidad"].Value = 2;
                    DialogosPrueba.Comprobar(() => form.Close(),
                        Confirmacion("Cerrar la compra", "se descarta el paquete", DialogResult.No));
                    Assert.IsTrue(form.Visible);
                    Assert.AreEqual(2, campo.Cells["Cantidad"].Value);
                    DialogosPrueba.Comprobar(() => form.Close(),
                        Confirmacion("Cerrar la compra", "se descarta el paquete", DialogResult.Yes));
                    Assert.IsFalse(form.Visible);
                }

                using (var vacia = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(vacia);
                    DialogosPrueba.Comprobar(() => vacia.Close());
                    Assert.IsFalse(vacia.Visible);
                }
            });
        }

        // CP-49 — RF-04, H-03: la primera cantidad disponible queda seleccionada con encabezado y ayuda explícitos.
        [TestMethod("CP-49: Cantidad seleccionada e indicación para escribir entradas")]
        public void CP49_CantidadIndicadaYSeleccionada()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    DataGridView grilla = Control<DataGridView>(form, "dgvSectores");
                    Assert.IsNotNull(grilla.CurrentCell);
                    Assert.AreEqual("Cantidad", grilla.CurrentCell.OwningColumn.Name);
                    Assert.IsTrue(Convert.ToInt32(grilla.CurrentCell.OwningRow.Cells["Disponibles"].Value) > 0);
                    StringAssert.Contains(grilla.Columns["Cantidad"].HeaderText, "escribí");
                    int maximo = GestorParametros.Instance.ObtenerEntero("COMPRA_ENTRADAS_MAX");
                    StringAssert.Contains(grilla.CurrentCell.ToolTipText, maximo.ToString());
                }
            });
        }

        // CP-50 — RF-05, RN-13, RN-28: Comprar y Seguro requieren entradas; quitarlas desactiva y destilda el seguro.
        [TestMethod("CP-50: Comprar y Seguro se habilitan solo con entradas")]
        public void CP50_CompraYSeguroRequierenEntradas()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    Button comprar = Control<Button>(form, "btnComprar");
                    CheckBox seguro = Control<CheckBox>(form, "chkSeguro");
                    CheckBox hospedaje = Control<CheckBox>(form, "chkHospedaje");
                    DataGridViewRow campo = Control<DataGridView>(form, "dgvSectores").Rows.Cast<DataGridViewRow>()
                        .Single(f => Convert.ToString(f.Cells["Sector"].Value) == "Campo");
                    Assert.IsFalse(comprar.Enabled);
                    Assert.IsFalse(seguro.Enabled);
                    Assert.IsTrue(hospedaje.Enabled);

                    campo.Cells["Cantidad"].Value = 1;
                    Assert.IsTrue(comprar.Enabled);
                    Assert.IsTrue(seguro.Enabled);
                    seguro.Checked = true;

                    campo.Cells["Cantidad"].Value = 0;
                    Assert.IsFalse(comprar.Enabled);
                    Assert.IsFalse(seguro.Enabled);
                    Assert.IsFalse(seguro.Checked);
                    Assert.IsTrue(hospedaje.Enabled);
                }
            });
        }

        // CP-51 — RNF-02: mensajes seguros en castellano para errores de base, negocio y generales.
        [DataTestMethod]
        [DataRow("base de datos", DisplayName = "CP-51: Error de base de datos sin detalle técnico")]
        [DataRow("negocio", DisplayName = "CP-51: Error real de negocio conserva su mensaje")]
        [DataRow("general", DisplayName = "CP-51: Error inesperado sin detalle técnico")]
        public void CP51_MensajeDeErrorParaUsuario(string caso)
        {
            EnSta(() =>
            {
                Exception error;
                string esperado;
                if (caso == "base de datos")
                {
                    error = new ErrorDeBaseDeDatosDePrueba();
                    esperado = Visual.ManejadorErrores.MensajeBaseDeDatos;
                }
                else if (caso == "negocio")
                {
                    error = Assert.ThrowsException<InvalidOperationException>(() => GestorPaquetes.Instance.Nuevo(999999, 3));
                    esperado = "No se encontró ningún evento con ese ID.";
                    Assert.AreEqual(esperado, error.Message);
                }
                else
                {
                    error = new Exception("detalle técnico");
                    esperado = Visual.ManejadorErrores.MensajeGeneral;
                }

                string mensaje = Visual.ManejadorErrores.MensajeParaUsuario(error);
                Assert.AreEqual(esperado, mensaje);
                Assert.IsFalse(mensaje.Contains("detalle técnico"));
            });
        }

        private sealed class ErrorDeBaseDeDatosDePrueba : System.Data.Common.DbException
        {
            public ErrorDeBaseDeDatosDePrueba() : base("detalle técnico") { }
        }

        // CP-52 — RF-06, RN-11: eliminar un evento durante el pago aborta con aviso y no registra un cobro.
        [TestMethod("CP-52: Pago de evento eliminado vuelve al listado sin cobrar")]
        public void CP52_PagoDeEventoEliminado()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Compra compra = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 1, false), cliente);
                using (var pago = new Visual.frmPago { Compra = compra })
                {
                    Cargar(pago);
                    GestorEventos.Instance.Eliminar(evento.Id);
                    DatosTarjeta tarjeta = Fake.Tarjeta();
                    Control<TextBox>(pago, "txtTitular").Text = tarjeta.Titular;
                    Control<TextBox>(pago, "txtNumero").Text = tarjeta.Numero;
                    Control<TextBox>(pago, "txtVencimiento").Text = tarjeta.Vencimiento;
                    Control<TextBox>(pago, "txtCvv").Text = tarjeta.Cvv;

                    DialogosPrueba.Comprobar(() => Control<Button>(pago, "btnPagar").PerformClick(),
                        new DialogoEsperado { Titulo = "Pago no realizado", Texto = "ya no está pendiente" });

                    Assert.AreEqual(DialogResult.Abort, pago.DialogResult);
                    Assert.AreEqual(0, GestorCompras.Instance.PagosDe(compra).Count);
                }
            });
        }

        // CP-54 — RF-04, H-03: el alto automático permite leer cada encabezado completo a su ancho real.
        [TestMethod("CP-54: Encabezados de sectores completos con ajuste de línea")]
        public void CP54_EncabezadosDeSectoresCompletos()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Cargar(form);
                    DataGridView grilla = Control<DataGridView>(form, "dgvSectores");
                    foreach (DataGridViewColumn columna in grilla.Columns)
                    {
                        if (!columna.Visible) continue;
                        Font fuente = columna.HeaderCell.InheritedStyle.Font;
                        Size texto = TextRenderer.MeasureText(columna.HeaderText, fuente,
                            new Size(columna.Width - 8, int.MaxValue), TextFormatFlags.WordBreak);
                        Assert.IsTrue(texto.Height <= grilla.ColumnHeadersHeight,
                            $"Encabezado recortado: {columna.HeaderText}; necesita {texto.Height} y tiene {grilla.ColumnHeadersHeight} px.");
                    }
                }
            });
        }

        // CP-65 — RF-07, RN-18: PDF temporal con los datos y el QR de una entrada confirmada.
        [TestMethod("CP-65: Ticket PDF conserva evento, sector, pedido y código completo")]
        public void CP65_TicketPdfConDatosDeCompraConfirmada()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Compra compra = Fake.CompraConfirmada(cliente, evento, 1, false);
                Entrada entrada = GestorCompras.Instance.EntradasDe(compra).Single();
                Sector sector = GestorEventos.Instance.Sectores(evento.Id).Single();
                string pedido = Visual.frmConfirmacion.NumeroPedido(compra);
                string mensaje = "ENTRADA " + evento.Nombre + "\nSECTOR: " + sector.Nombre +
                    "\nPRECIO: " + entrada.Precio.ToString("C2", new System.Globalization.CultureInfo("es-AR")) +
                    "\nFECHA: " + evento.Fecha.ToString("dd/MM/yyyy HH:mm") +
                    "\nPEDIDO: " + pedido + "\nCÓDIGO: " + entrada.CodigoQR;
                string ruta = Path.GetTempFileName();
                Exception fallo = null;
                try
                {
                    using (var form = new Visual.FrmCompraCliente())
                        form.GenerarPDF(ruta, mensaje, GeneradorQR.Instance.Imagen(entrada.CodigoQR));
                    using (var lector = new iText.Kernel.Pdf.PdfReader(ruta))
                    using (var pdf = new iText.Kernel.Pdf.PdfDocument(lector))
                    {
                        Assert.AreEqual(1, pdf.GetNumberOfPages());
                        string texto = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdf.GetPage(1));
                        foreach (string esperado in new[] { evento.Nombre, sector.Nombre, pedido, entrada.CodigoQR })
                            StringAssert.Contains(texto, esperado);
                    }
                }
                catch (Exception ex) { fallo = ex; throw; }
                finally
                {
                    try { File.Delete(ruta); }
                    catch (IOException ex)
                    {
                        if (fallo != null) throw new AggregateException("Fallaron el PDF y su limpieza temporal.", fallo, ex);
                        throw;
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
