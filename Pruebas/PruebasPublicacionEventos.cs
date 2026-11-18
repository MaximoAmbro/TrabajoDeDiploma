using System;
using System.Linq;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public sealed class PruebasPublicacionEventos
    {
        [TestInitialize]
        public void Preparar()
        {
            GestorSesion.Instance.Cerrar();
            Fake.Limpiar();
        }

        [TestCleanup]
        public void Limpiar()
        {
            GestorSesion.Instance.Cerrar();
            Fake.Limpiar();
        }

        [TestMethod]
        public void PrimerEventoRequiereRevisionYLaAprobacionHabilitaPublicacionesSiguientes()
        {
            Usuario organizador = Fake.PropietarioSinLocales();
            Fake.ComoOrganizador(organizador);
            var local = new Local { Nombre = "test Local inicial", Direccion = "Calle de prueba 123", Capacidad = 500 };
            local.Id = GestorLocales.Instance.Registrar(local);

            Evento primero = CrearEvento(local.Id, "test Primer evento");
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.Publicar(primero.Id),
                "Un evento sin sectores con capacidad debe seguir sin poder publicarse.");
            GestorEventos.Instance.AgregarSector(primero.Id, new Sector { Nombre = "Campo", Precio = 1000m, Capacidad = 100 });
            Assert.AreEqual(GestorEventos.EstadoBorrador, GestorEventos.Instance.ObtenerPorId(primero.Id).Estado,
                "Agregar un sector no publica el evento.");

            GestorEventos.Instance.Publicar(primero.Id);
            Assert.AreEqual(GestorEventos.EstadoEnRevision, GestorEventos.Instance.ObtenerPorId(primero.Id).Estado);
            Assert.IsFalse(GestorEventos.Instance.Buscar("test Primer evento", null, null).Any(e => e.Id == primero.Id),
                "Un evento en revisión no aparece en el catálogo.");

            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(cliente);
            ReglaNegocioException venta = Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.Nuevo(primero.Id, cliente.Id));
            Assert.AreEqual("RN-09", venta.Regla);

            Fake.ComoOrganizador(organizador);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.AprobarPrimerEvento(primero.Id),
                "El organizador no puede aprobar su propio evento.");
            Fake.ComoAdministrador();
            Assert.IsTrue(GestorEventos.Instance.PendientesDeRevision().Any(e => e.Id == primero.Id));
            GestorEventos.Instance.AprobarPrimerEvento(primero.Id);
            Assert.AreEqual(GestorEventos.EstadoPublicado, GestorEventos.Instance.ObtenerPorId(primero.Id).Estado);

            Fake.ComoOrganizador(organizador);
            Evento segundo = CrearEvento(local.Id, "test Segundo evento");
            GestorEventos.Instance.AgregarSector(segundo.Id, new Sector { Nombre = "Platea", Precio = 2000m, Capacidad = 100 });
            Assert.AreEqual(GestorEventos.EstadoBorrador, GestorEventos.Instance.ObtenerPorId(segundo.Id).Estado);
            GestorEventos.Instance.Publicar(segundo.Id);
            Assert.AreEqual(GestorEventos.EstadoPublicado, GestorEventos.Instance.ObtenerPorId(segundo.Id).Estado);
        }

        [TestMethod]
        public void PublicarSinSectorConCapacidadSeRechaza()
        {
            Usuario organizador = Fake.PropietarioSinLocales();
            Fake.ComoOrganizador(organizador);
            var local = new Local { Nombre = "test Local sin sectores", Direccion = "Calle de prueba 123", Capacidad = 100 };
            local.Id = GestorLocales.Instance.Registrar(local);
            Evento evento = CrearEvento(local.Id, "test Evento vacío");
            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.Publicar(evento.Id));
            Assert.AreEqual("RN-06", error.Regla);
            Assert.AreEqual(GestorEventos.EstadoBorrador, GestorEventos.Instance.ObtenerPorId(evento.Id).Estado);
        }

        private static Evento CrearEvento(int idLocal, string nombre)
        {
            var evento = new Evento { Nombre = nombre, Fecha = DateTime.Today.AddDays(30).AddHours(20), IdLocal = idLocal, Categoria = "Concierto" };
            evento.Id = GestorEventos.Instance.Crear(evento);
            return evento;
        }
    }
}
