using System;
using System.Linq;
using Datos;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasRechazosNegocio
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

        // CP-55 — RF-01, RN-05: el registro genérico existente rechaza un perfil inválido sin crear la cuenta.
        [TestMethod("CP-55: Registro rechaza un tipo de usuario inválido")]
        public void CP55_RegistroRechazaPerfilInvalido()
        {
            string mail = Fake.MailNuevo();

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "PerfilInexistente"));

            Assert.AreEqual("RN-05", error.Regla);
            Assert.IsNull(RepositorioUsuarios.Instance.ObtenerPorMail(mail));
        }

        // CP-56 — RF-12, RN-06: no se elimina el último sector con capacidad de un evento publicado.
        [TestMethod("CP-56: Evento publicado conserva su último sector con capacidad")]
        public void CP56_RechazaEliminarUltimoSectorPublicado()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Sector unico = evento.Sectores.Single();

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorEventos.Instance.EliminarSector(unico.Id));

            Assert.AreEqual("RN-06", error.Regla);
            Assert.AreEqual(unico.Id, GestorEventos.Instance.Sectores(evento.Id).Single().Id);
            Assert.AreEqual(GestorEventos.EstadoPublicado, GestorEventos.Instance.ObtenerPorId(evento.Id).Estado);
        }

        // CP-57 — RF-05, RN-14: un servicio de otro evento no se incorpora al paquete.
        [TestMethod("CP-57: Paquete rechaza un servicio que su evento no ofrece")]
        public void CP57_RechazaServicioDeOtroEvento()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Evento otroEvento = Fake.EventoPublicado(1000m, 5);
            Servicio ajeno = Fake.ServicioHabilitado(otroEvento, "Hospedaje", 1000m);
            Paquete paquete = Fake.Paquete(evento, 1, false);

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorPaquetes.Instance.AgregarServicio(paquete, ajeno.Id));

            Assert.AreEqual("RN-14", error.Regla);
            Assert.AreEqual(0, paquete.Servicios.Count);
            Assert.AreEqual(1, GestorPaquetes.Instance.CantidadEntradas(paquete));
        }

        // CP-58 — RF-13, RN-25: rechazar otra suscripción vigente conserva el plan activo original.
        [TestMethod("CP-58: Cliente con suscripción activa no puede contratar otra")]
        public void CP58_RechazaSegundaSuscripcionActiva()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Suscripcion primera = Fake.SuscripcionActiva(cliente, GestorSuscripciones.PlanMensual);
            Assert.IsNotNull(primera);

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorSuscripciones.Instance.Contratar(cliente.Id, GestorSuscripciones.PlanAnual));

            Assert.AreEqual("RN-25", error.Regla);
            Suscripcion actual = GestorSuscripciones.Instance.ActivaDe(cliente.Id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(primera.Id, actual.Id);
            Assert.AreEqual(GestorSuscripciones.PlanMensual, actual.Plan);
        }

        // CP-59 — RF-05, RN-29: las devoluciones sin seguro o fuera del plazo se rechazan sin reembolso.
        [TestMethod("CP-59: Devolución rechazada sin seguro o fuera del plazo")]
        public void CP59_RechazaDevolucionSinSeguroOFueraDePlazo()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Compra sinSeguro = Fake.CompraConfirmada(cliente, evento, 1, false);

            ReglaNegocioException faltaSeguro = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(sinSeguro)));
            Assert.AreEqual("RN-29", faltaSeguro.Regla);
            Assert.AreEqual(EstadoCompra.Confirmada, GestorCompras.Instance.ObtenerPorId(sinSeguro.Id).Estado);
            Assert.AreEqual(0m, GestorCompras.Instance.ObtenerPorId(sinSeguro.Id).MontoReembolsado);

            Compra fueraDePlazo = Fake.CompraConfirmada(cliente, evento, 1, true);
            int horas = GestorParametros.Instance.ObtenerEntero("SEGURO_HORAS_COBERTURA");
            Fake.CambiarFechaDeEvento(evento.Id, DateTime.Now.AddHours(horas / 2.0));

            ReglaNegocioException plazo = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(fueraDePlazo)));
            Assert.AreEqual("RN-29", plazo.Regla);
            Assert.AreEqual(EstadoCompra.Confirmada, GestorCompras.Instance.ObtenerPorId(fueraDePlazo.Id).Estado);
            Assert.AreEqual(0m, GestorCompras.Instance.ObtenerPorId(fueraDePlazo.Id).MontoReembolsado);
        }

        // CP-60 — RF-12, RN-38: agregar un sector no puede superar la capacidad del local.
        [TestMethod("CP-60: Sectores no pueden superar la capacidad del local")]
        public void CP60_RechazaSectorQueSuperaCapacidadDelLocal()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Sector original = evento.Sectores.Single();
            Local local = GestorLocales.Instance.PorId(evento.IdLocal);
            Sector excesivo = new Sector
            {
                Nombre = "Sector que excede el cupo",
                Capacidad = local.Capacidad,
                Precio = 1000m
            };

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorEventos.Instance.AgregarSector(evento.Id, excesivo));

            Assert.AreEqual("RN-38", error.Regla);
            Sector conservado = GestorEventos.Instance.Sectores(evento.Id).Single();
            Assert.AreEqual(original.Id, conservado.Id);
            Assert.AreEqual(original.Capacidad, conservado.Capacidad);
            Assert.AreEqual(0, excesivo.Id);
        }

        // CP-61 — RF-05, RN-39: un cliente sin suscripción no puede comprar durante la preventa.
        [TestMethod("CP-61: Preventa rechaza al cliente sin suscripción activa")]
        public void CP61_RechazaPreventaSinSuscripcion()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Sector sector = evento.Sectores.Single();
            Assert.IsFalse(GestorSuscripciones.Instance.TieneActiva(cliente.Id));
            GestorEventos.Instance.CambiarPreventa(evento.Id, DateTime.Now.AddDays(1));
            int disponibles = GestorPaquetes.Instance.DisponiblesDe(sector.Id);

            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id));

            Assert.AreEqual("RN-39", error.Regla);
            Assert.AreEqual(disponibles, GestorPaquetes.Instance.DisponiblesDe(sector.Id));
            Assert.IsFalse(GestorSuscripciones.Instance.TieneActiva(cliente.Id));
        }
    }
}
