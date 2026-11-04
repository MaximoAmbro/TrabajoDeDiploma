using System;
using System.Linq;
using System.Text.RegularExpressions;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasCompra
    {
        [TestInitialize]
        public void Preparar()
        {
            Fake.Limpiar();
            GestorSesion.Instance.Cerrar();
        }

        [TestCleanup]
        public void Terminar()
        {
            GestorSesion.Instance.Cerrar();
        }

        [ClassCleanup]
        public static void LimpiarAlFinal()
        {
            Fake.Limpiar();
        }

        // CP-30 — RF-03, RN-09: filtros por nombre, dirección, categoría y fecha; solo eventos vigentes.
        [TestMethod("CP-30: Filtros y rechazo de eventos pasados o suspendidos")]
        public void CP30_FiltrosYEventoNoVigente()
        {
            Evento vigente = Fake.EventoPublicado(1000m, 10);
            Assert.IsTrue(GestorEventos.Instance.Buscar(vigente.Nombre, null, null).Any(e => e.Id == vigente.Id), "Filtro por nombre.");
            Assert.IsTrue(GestorEventos.Instance.Buscar("Calle de prueba", null, null).Any(e => e.Id == vigente.Id), "Filtro por ubicación.");
            Assert.IsTrue(GestorEventos.Instance.Buscar(null, "Concierto", null).Any(e => e.Id == vigente.Id), "Categoría coincidente.");
            string otraCategoria = GestorEventos.Categorias.First(c => c != "Concierto");
            Assert.IsFalse(GestorEventos.Instance.Buscar(null, otraCategoria, null).Any(e => e.Id == vigente.Id), "Categoría distinta.");
            Assert.IsTrue(GestorEventos.Instance.Buscar(null, null, DateTime.Today.AddDays(29)).Any(e => e.Id == vigente.Id), "Fecha anterior al evento.");
            Assert.IsFalse(GestorEventos.Instance.Buscar(null, null, DateTime.Today.AddDays(31)).Any(e => e.Id == vigente.Id), "Fecha posterior al evento.");

            Evento pasado = Fake.EventoPublicado(1000m, 10);
            Fake.CambiarFechaDeEvento(pasado.Id, DateTime.Now.AddDays(-1));
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Assert.IsFalse(GestorEventos.Instance.Buscar(pasado.Nombre, null, null).Any(e => e.Id == pasado.Id));
            ReglaNegocioException ventaPasada = Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.Nuevo(pasado.Id, cliente.Id));
            Assert.AreEqual("RN-09", ventaPasada.Regla);
            ReglaNegocioException esperaPasada = Assert.ThrowsException<ReglaNegocioException>(() => GestorListaEspera.Instance.Inscribir(pasado.Id, cliente.Id));
            Assert.AreEqual("RN-09", esperaPasada.Regla);

            Evento suspendido = Fake.EventoPublicado(1000m, 10);
            Fake.CambiarEstadoDeEvento(suspendido.Id, GestorEventos.EstadoSuspendido);
            Assert.IsFalse(GestorEventos.Instance.Buscar(suspendido.Nombre, null, null).Any(e => e.Id == suspendido.Id));
        }

        // CP-31 — RF-04, RN-08, RN-12, RN-13: cupo por sector, máximo total y entrada obligatoria.
        [TestMethod("CP-31: Topes de cupo y paquete sin entradas")]
        public void CP31_TopesDelPaquete()
        {
            Evento evento = Fake.EventoPublicado(1000m, 3);
            Sector chico = evento.Sectores.Single();
            Sector grande = Fake.AgregarSector(evento, 1000m, 100);
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);

            Paquete paquete = GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id);
            ReglaNegocioException cupo = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorPaquetes.Instance.AgregarEntradas(paquete, chico.Id, 4));
            Assert.AreEqual("RN-08", cupo.Regla);

            paquete = GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id);
            int maximo = GestorParametros.Instance.ObtenerEntero("COMPRA_ENTRADAS_MAX");
            GestorPaquetes.Instance.AgregarEntradas(paquete, grande.Id, maximo);
            Assert.AreEqual(maximo, GestorPaquetes.Instance.CantidadEntradas(paquete));
            ReglaNegocioException tope = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorPaquetes.Instance.AgregarEntradas(paquete, chico.Id, 1));
            Assert.AreEqual("RN-12", tope.Regla);

            paquete = GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id);
            Servicio servicio = Fake.ServicioHabilitado(evento, "Hospedaje", 1000m);
            GestorPaquetes.Instance.AgregarServicio(paquete, servicio.Id);
            ReglaNegocioException sinEntradas = Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.Validar(paquete));
            Assert.AreEqual("RN-13", sinEntradas.Regla);
        }

        // CP-32 — RF-06, RF-07, RN-17, RN-18, RN-30, RN-31: aprobación, QR únicos y pago sin número completo.
        [TestMethod("CP-32: Pago aprobado, QR únicos y registro tokenizado")]
        public void CP32_PagoAprobadoConfirmacionQrYRegistro()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Evento evento = Fake.EventoPublicado(1000m, 10);
            Paquete paquete = Fake.Paquete(evento, 3, false);
            Compra compra = GestorCompras.Instance.Iniciar(paquete, cliente);

            Assert.IsTrue(GestorCompras.Instance.Confirmar(compra, Fake.Tarjeta()));
            Assert.AreEqual(EstadoCompra.Confirmada, GestorCompras.Instance.ObtenerPorId(compra.Id).Estado);
            var entradas = GestorCompras.Instance.EntradasDe(compra);
            Assert.AreEqual(3, entradas.Count);
            foreach (Entrada entrada in entradas)
            {
                Assert.IsNotNull(entrada.CodigoQR);
                StringAssert.Matches(entrada.CodigoQR, new Regex("^EA-[0-9A-F]{32}$"));
            }
            Assert.AreEqual(3, entradas.Select(e => e.CodigoQR).Distinct().Count());

            var pagos = GestorCompras.Instance.PagosDe(compra);
            Assert.AreEqual(1, pagos.Count);
            Pago pago = pagos.Single();
            Assert.AreEqual(EstadoPago.Aprobado, pago.Estado);
            Assert.AreEqual("1111", pago.Ultimos4);
            StringAssert.StartsWith(pago.Token, "tok_sim_");
            foreach (var propiedad in typeof(Pago).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                string valor = (string)propiedad.GetValue(pago);
                Assert.IsFalse(valor?.Contains("4111111111111111") ?? false,
                    "El pago expone el número completo en " + propiedad.Name + ".");
            }
        }

        // CP-33 — RF-06, RF-07, RN-17, RN-31, RN-32: rechazo registrado que libera cupo para otro cliente.
        [TestMethod("CP-33: Pago rechazado libera el cupo y registra el intento")]
        public void CP33_PagoRechazadoLiberaCupoYQuedaRegistrado()
        {
            Evento evento = Fake.EventoPublicado(1000m, 1);
            Cliente primero = Fake.Cliente(NivelFidelidad.Bronce);
            Cliente segundo = Fake.Cliente(NivelFidelidad.Bronce);
            int sector = evento.Sectores.Single().Id;
            Compra compra = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 1, false), primero);
            Assert.AreEqual(0, GestorPaquetes.Instance.DisponiblesDe(sector));

            DatosTarjeta tarjeta = Fake.Tarjeta();
            tarjeta.Numero = "4111 1111 1111 0000";
            Assert.IsFalse(GestorCompras.Instance.Confirmar(compra, tarjeta));

            Assert.AreEqual(EstadoCompra.Pendiente, GestorCompras.Instance.ObtenerPorId(compra.Id).Estado);
            var pagos = GestorCompras.Instance.PagosDe(compra);
            Assert.AreEqual(1, pagos.Count);
            Assert.AreEqual(EstadoPago.Rechazado, pagos.Single().Estado);
            Assert.AreEqual("0000", pagos.Single().Ultimos4);
            Assert.AreEqual(1, GestorPaquetes.Instance.DisponiblesDe(sector));
            Paquete otroPaquete = GestorPaquetes.Instance.Nuevo(evento.Id, segundo.Id);
            GestorPaquetes.Instance.AgregarEntradas(otroPaquete, sector, 1);
            Compra otraCompra = GestorCompras.Instance.Iniciar(otroPaquete, segundo);
            Assert.IsTrue(otraCompra.Id > 0);
            Assert.AreEqual(0, GestorPaquetes.Instance.DisponiblesDe(sector));
        }

        // CP-34 — RF-06, RN-15: reserva vencida rechaza el cobro, vence la compra y devuelve el cupo.
        [TestMethod("CP-34: Reserva vencida libera el cupo sin cobrar")]
        public void CP34_ReservaVencida()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            int sector = evento.Sectores.Single().Id;
            Compra compra = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 2, false), cliente);
            Assert.AreEqual(3, GestorPaquetes.Instance.DisponiblesDe(sector));
            int minutos = GestorParametros.Instance.ObtenerEntero("RESERVA_MINUTOS");
            Fake.RetrasarInicioDeCompra(compra.Id, minutos + 1);

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorCompras.Instance.Confirmar(compra, Fake.Tarjeta()));
            Assert.AreEqual("RN-15", error.Regla);
            Assert.AreEqual(EstadoCompra.Vencida, GestorCompras.Instance.ObtenerPorId(compra.Id).Estado);
            Assert.AreEqual(5, GestorPaquetes.Instance.DisponiblesDe(sector));
            Assert.AreEqual(0, GestorCompras.Instance.PagosDe(compra).Count);
        }

        // CP-43 — RF-06, RN-15: el plazo lo fija la reserva persistida, no el parámetro vigente.
        [TestMethod("CP-43: El plazo lo fija la reserva, no el parámetro vigente")]
        public void CP43_PlazoFijadoPorLaReservaPersistida()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            int sector = evento.Sectores.Single().Id;
            Compra compra = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 2, false), cliente);
            Assert.AreEqual(3, GestorPaquetes.Instance.DisponiblesDe(sector));

            // El objeto en memoria conserva el vencimiento original: Confirmar debe releer la base.
            Fake.VencerReserva(compra.Id);
            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorCompras.Instance.Confirmar(compra, Fake.Tarjeta()));

            Assert.AreEqual("RN-15", error.Regla);
            Assert.AreEqual(EstadoCompra.Vencida, GestorCompras.Instance.ObtenerPorId(compra.Id).Estado);
            Assert.AreEqual(5, GestorPaquetes.Instance.DisponiblesDe(sector));
            Assert.AreEqual(0, GestorCompras.Instance.PagosDe(compra).Count);
        }
    }
}
