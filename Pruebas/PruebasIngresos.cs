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

        [TestInitialize]
        public void Preparar()
        {
            Fake.Limpiar();
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            evento = Fake.EventoPublicado(1000m, 5);
            compra = Fake.CompraConfirmada(cliente, evento, 1, true);
            entrada = GestorCompras.Instance.EntradasDe(compra).Single();
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddMinutes(10));
            Fake.ComoAdministrador();
        }

        [TestCleanup]
        public void Terminar() { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }

        // CP-99 — RF-07, RN-18: registro y persistencia sin alterar ventas.
        [TestMethod("CP-99: Una entrada válida ingresa sin cambiar su estado de venta")]
        public void EntradaValida()
        {
            ResultadoIngreso resultado = GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id);
            Assert.IsTrue(resultado.Valido);
            Entrada guardada = GestorCompras.Instance.EntradasDe(compra).Single();
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
            Assert.ThrowsException<ReglaNegocioException>(() => GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id));
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
            GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(compra));
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

        // CP-104 — RF-07, RN-18: dos puertas compiten contra la misma fila en SQL Server.
        [TestMethod("CP-104: Dos ingresos simultáneos admiten exactamente uno")]
        public void Concurrentes()
        {
            using (var inicio = new Barrier(2))
            {
                Func<ResultadoIngreso> escanear = () => {
                    Assert.IsTrue(inicio.SignalAndWait(TimeSpan.FromSeconds(10)));
                    return GestorIngresos.Instance.Registrar(entrada.CodigoQR, evento.Id);
                };
                Task<ResultadoIngreso> uno = Task.Run(escanear), dos = Task.Run(escanear);
                Assert.IsTrue(Task.WaitAll(new Task[] { uno, dos }, TimeSpan.FromSeconds(20)));
                var resultados = new[] { uno.Result, dos.Result };
                Assert.AreEqual(1, resultados.Count(r => r.Valido));
                Assert.AreEqual(1, resultados.Count(r => r.Motivo == MotivoIngreso.YaUsado));
                Assert.AreEqual(1, GestorIngresos.Instance.Contar(evento.Id));
            }
        }
    }
}
