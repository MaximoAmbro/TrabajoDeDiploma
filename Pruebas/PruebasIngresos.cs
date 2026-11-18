using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Datos;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasIngresos
    {
        private Evento evento;
        private Compra compra;
        private Entrada entrada;
        private Cliente cliente;

        [TestInitialize]
        public void Preparar()
        {
            Fake.Limpiar();
            cliente = Fake.Cliente(NivelFidelidad.Bronce);
            evento = Fake.EventoPublicado(1000m, 5);
            compra = Fake.CompraConfirmada(cliente, evento, 1, true);
            entrada = GestorCompras.Instance.EntradasDe(compra).Single();
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddMinutes(10));
            Fake.ComoOrganizador();
        }

        [TestCleanup]
        public void Terminar() { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }

        // CP-99 — RF-07, RN-18: registro y persistencia sin alterar ventas.
        [TestMethod("CP-99: Una entrada válida ingresa sin cambiar su estado de venta")]
        public void EntradaValida()
        {
            ResultadoIngreso resultado = GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id);
            Assert.IsTrue(resultado.Valido);
            Entrada guardada = RepositorioEntradas.Instance.ObtenerPorCompra(compra.Id).Single();
            Assert.IsTrue(guardada.FechaIngreso.HasValue);
            Assert.AreEqual(EstadoEntrada.Confirmada, guardada.Estado);
            Assert.AreEqual(1, GestorIngresos.Instance.Contar(evento.Id));
            PruebasSeguridad.EnStaSinCambiarDatos(() => {
                using (var form = new Visual.frmControlIngreso())
                {
                    form.Show();
                    System.Windows.Forms.Application.DoEvents();
                    var combo = (System.Windows.Forms.ComboBox)form.Controls.Find("cmbEventos", true).Single();
                    combo.SelectedValue = evento.Id;
                    var codigo = (System.Windows.Forms.TextBox)form.Controls.Find("txtCodigo", true).Single();
                    codigo.Text = entrada.CodigoQR;
                    ((System.Windows.Forms.Button)form.Controls.Find("btnRegistrar", true).Single()).PerformClick();
                    var resultadoPantalla = form.Controls.Find("lblResultado", true).Single();
                    StringAssert.Contains(resultadoPantalla.Text, "ya ingresó");
                    Assert.AreEqual(Visual.Tema.Error, resultadoPantalla.ForeColor);
                    StringAssert.Contains(form.Controls.Find("lblContador", true).Single().Text, "1");
                    Assert.AreEqual("", codigo.Text);
                    string carpeta = System.IO.Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Seguridad");
                    System.IO.Directory.CreateDirectory(carpeta);
                    using (var imagen = new System.Drawing.Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(imagen, new System.Drawing.Rectangle(0, 0, form.Width, form.Height));
                        imagen.Save(System.IO.Path.Combine(carpeta, "control-ingreso.png"));
                    }
                }
            });
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(cliente);
            // M-22: el rechazo de Registrar ahora es un resultado con su regla, no una excepción.
            ResultadoIngreso sinPermiso = GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id);
            Assert.IsFalse(sinPermiso.Valido);
            Assert.AreEqual(MotivoIngreso.NoAutorizado, sinPermiso.Motivo);
            Assert.AreEqual("RN-48", sinPermiso.Regla);
            Assert.AreEqual(guardada.FechaIngreso, RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso);
            GestorSesion.Instance.Cerrar();
            Assert.ThrowsException<ReglaNegocioException>(() => GestorIngresos.Instance.Contar(evento.Id));
        }

        // CP-100 — RF-07, RN-18: un segundo escaneo informa la fecha del primero.
        [TestMethod("CP-100: El segundo ingreso se rechaza con la fecha original")]
        public void SegundoIngreso()
        {
            Assert.IsTrue(GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id).Valido);
            DateTime fecha = RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso.Value;
            ResultadoIngreso repetido = GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id);
            Assert.AreEqual(MotivoIngreso.YaUsado, repetido.Motivo);
            Assert.AreEqual(fecha, repetido.FechaIngreso);
            StringAssert.Contains(repetido.Mensaje, fecha.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        // CP-101 — RF-07, RN-18/29: un PDF conservado tras el reembolso no habilita el ingreso.
        [TestMethod("CP-101: La entrada reembolsada no ingresa")]
        public void Reembolsada()
        {
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddDays(30));
            Fake.ComoCliente(cliente);
            GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(compra));
            Fake.ComoOrganizador();
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddMinutes(10));
            Assert.AreEqual(MotivoIngreso.Reembolsado, GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id).Motivo);
            Assert.AreEqual(0, GestorIngresos.Instance.Contar(evento.Id));
        }

        // CP-102 — RF-07, RN-18: el formato de un código no prueba su existencia.
        [TestMethod("CP-102: El código inventado se rechaza")]
        public void Inventada()
        {
            Assert.AreEqual(MotivoIngreso.Inexistente, GestorIngresos.Instance.Registrar("EA-" + Guid.NewGuid().ToString("N"), evento.Id).Motivo);
            Assert.AreEqual(0, GestorIngresos.Instance.Contar(evento.Id));
        }

        // CP-103 — RF-07, RN-18: evento y ventana configurada se verifican sin consumir el ticket.
        [TestMethod("CP-103: Evento equivocado y horario inválido no consumen el ticket")]
        public void OtroEvento()
        {
            Evento otro = Fake.EventoPublicado(1000m, 5);
            Assert.AreEqual(MotivoIngreso.OtroEvento, GestorIngresos.Instance.Registrar(entrada.CodigoQR, otro.Id).Motivo);
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddDays(2));
            Assert.AreEqual(MotivoIngreso.FueraDeHorario, GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id).Motivo);
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddDays(-2));
            Assert.AreEqual(MotivoIngreso.FueraDeHorario, GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id).Motivo);
            Assert.IsNull(RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso);
        }

        // CP-104 — RF-07, RN-18: cien tickets frescos, diez puertas y barrera de arranque por ronda.
        [TestMethod("CP-104: Diez ingresos simultáneos admiten exactamente uno durante cien rondas")]
        public void Concurrentes()
        {
            const int rondas = 100, puertas = 10;
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            var eventoLote = Fake.EventoPublicado(1000m, rondas);
            Fake.CambiarFechaDeEvento(eventoLote.Id, DateTime.Now.AddMinutes(10));
            string carpeta = System.IO.Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Seguridad");
            System.IO.Directory.CreateDirectory(carpeta);
            string archivo = System.IO.Path.Combine(carpeta, "atomicidad-100x10.csv");
            System.IO.File.WriteAllText(archivo, "Ronda;Validos;YaUsados;IngresosPersistidos\r\n");
            for (int ronda = 0; ronda < rondas; ronda++)
            {
                var compraLote = Fake.CompraConfirmada(cliente, eventoLote, 1, false);
                var ticket = GestorCompras.Instance.EntradasDe(compraLote).Single();
                Fake.ComoOrganizador();
                GestorIngresos.Instance.Contar(eventoLote.Id); // inicialización antes de soltar las puertas.
                using (var inicio = new Barrier(puertas))
                {
                    var tareas = Enumerable.Range(0, puertas).Select(i => Task.Factory.StartNew(() =>
                    {
                        Assert.IsTrue(inicio.SignalAndWait(TimeSpan.FromSeconds(10)), "La barrera no reunió diez puertas.");
                        return GestorIngresos.Instance.Registrar(ticket.CodigoQR, eventoLote.Id);
                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray();
                    Assert.IsTrue(Task.WaitAll(tareas, TimeSpan.FromSeconds(20)), "Las puertas no terminaron en la ronda " + ronda);
                    var resultados = tareas.Select(t => t.Result).ToArray();
                    int validos = resultados.Count(r => r.Valido), usados = resultados.Count(r => r.Motivo == MotivoIngreso.YaUsado);
                    int persistidos = GestorIngresos.Instance.Contar(eventoLote.Id);
                    System.IO.File.AppendAllText(archivo, (ronda + 1) + ";" + validos + ";" + usados + ";" + persistidos + "\r\n");
                    Assert.AreEqual(1, validos, "Ronda " + ronda + ": más de una puerta admitió el ticket.");
                    Assert.AreEqual(puertas - 1, usados, "Ronda " + ronda + ": los otros nueve rechazos deben ser YaUsado.");
                    Assert.AreEqual(ronda + 1, persistidos, "El conteo persistido no corresponde a un ingreso por ticket.");
                    DateTime fecha = RepositorioEntradas.Instance.PorCodigo(ticket.CodigoQR).FechaIngreso.Value;
                    Assert.IsTrue(resultados.Where(r => !r.Valido).All(r => r.FechaIngreso == fecha),
                        "El rechazo debe informar el mismo ingreso persistido.");
                }
            }
        }

    }
}
