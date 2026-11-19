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

        // CP-204 — RF-12, RN-54: enviar el primer evento a revision y publicar directamente los siguientes tras la aprobacion.
        [TestMethod("CP-204: Primer evento requiere revisión; los siguientes se publican tras aprobación")]
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

        // CP-205 — RF-12, RN-06: no publicar eventos que no tengan un sector con capacidad positiva.
        [TestMethod("CP-205: Publicar sin sector con capacidad se rechaza")]
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

        // CP-198 — RF-12/RN-48/RN-54: la revisión es global y exige fundamento.
        [TestMethod("CP-198: Organizador no rechaza eventos y el Administrador debe justificarlo")]
        public void CP198_RechazoProtegidoYMotivoObligatorio()
        {
            Usuario titular = CrearOrganizadorConLocal(out var local);
            var propio = CrearEvento(local.Id, "test Evento propio en revisión");
            GestorEventos.Instance.AgregarSector(propio.Id, new Sector { Nombre = "Campo", Precio = 1000m, Capacidad = 20 });
            GestorEventos.Instance.Publicar(propio.Id);

            Usuario otro = CrearOrganizadorConLocal(out var localAjeno);
            var ajeno = CrearEvento(localAjeno.Id, "test Evento ajeno en revisión");
            GestorEventos.Instance.AgregarSector(ajeno.Id, new Sector { Nombre = "Campo", Precio = 1000m, Capacidad = 20 });
            GestorEventos.Instance.Publicar(ajeno.Id);

            Fake.ComoOrganizador(titular);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.RechazarPrimerEvento(propio.Id, "Motivo válido."));
            Fake.ComoOrganizador(otro);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.RechazarPrimerEvento(ajeno.Id, "Motivo válido."));

            Fake.ComoAdministrador();
            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(
                () => GestorEventos.Instance.RechazarPrimerEvento(propio.Id, "  "));
            Assert.AreEqual("RN-54", error.Regla);
            Assert.AreEqual("Escribí el motivo del rechazo.", error.Message);
            Assert.AreEqual(GestorEventos.EstadoEnRevision, GestorEventos.Instance.ObtenerPorId(propio.Id).Estado);
        }

        // CP-199 — RF-03/RN-09/RN-54: el evento rechazado no se vende y vuelve a revisión al publicarse.
        [TestMethod("CP-199: Rechazo devuelve a borrador, muestra el motivo y exige nueva revisión")]
        public void CP199_RechazoDevuelveABorradorYOrganizadorVeMotivo()
        {
            Usuario organizador = CrearOrganizadorConLocal(out var local);
            Fake.ComoOrganizador(organizador);
            Evento evento = CrearEvento(local.Id, "test Evento rechazado");
            GestorEventos.Instance.AgregarSector(evento.Id, new Sector { Nombre = "Campo", Precio = 1000m, Capacidad = 20 });
            GestorEventos.Instance.Publicar(evento.Id);

            const string fundamento = "La fecha necesita una corrección.";
            Fake.ComoAdministrador();
            GestorEventos.Instance.RechazarPrimerEvento(evento.Id, fundamento);
            Evento rechazado = GestorEventos.Instance.ObtenerPorId(evento.Id);
            Assert.AreEqual(GestorEventos.EstadoBorrador, rechazado.Estado);
            Assert.AreEqual(fundamento, rechazado.MotivoRechazo);
            Assert.IsFalse(GestorEventos.Instance.Buscar("test Evento rechazado", null, null).Any(e => e.Id == evento.Id));

            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(cliente);
            ReglaNegocioException venta = Assert.ThrowsException<ReglaNegocioException>(
                () => GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id));
            Assert.AreEqual("RN-09", venta.Regla);

            Fake.ComoOrganizador(organizador);
            using (var lista = new Visual.Vendedor.frmEventosLocal { IDlocal = local.Id })
            {
                lista.CargaEventos();
                var grilla = lista.Controls.Find("dgvEventos", true).OfType<System.Windows.Forms.DataGridView>().Single();
                Assert.AreEqual(fundamento, grilla.Rows.Cast<System.Windows.Forms.DataGridViewRow>()
                    .Single(r => Convert.ToInt32(r.Cells["ID"].Value) == evento.Id).Cells["MotivoDelRechazo"].Value);
            }

            GestorEventos.Instance.Publicar(evento.Id);
            Assert.AreEqual(GestorEventos.EstadoEnRevision, GestorEventos.Instance.ObtenerPorId(evento.Id).Estado,
                "Sigue siendo la primera publicación aprobada del Organizador.");
            Assert.IsFalse(GestorEventos.Instance.Buscar("test Evento rechazado", null, null).Any(e => e.Id == evento.Id));
        }

        private static Usuario CrearOrganizadorConLocal(out Local local)
        {
            Usuario organizador = Fake.PropietarioSinLocales();
            Fake.ComoOrganizador(organizador);
            local = new Local { Nombre = "test Local de revisión", Direccion = "Calle de prueba 123", Capacidad = 100 };
            local.Id = GestorLocales.Instance.Registrar(local);
            return organizador;
        }

        private static Evento CrearEvento(int idLocal, string nombre)
        {
            var evento = new Evento { Nombre = nombre, Fecha = DateTime.Today.AddDays(30).AddHours(20), IdLocal = idLocal, Categoria = "Concierto" };
            evento.Id = GestorEventos.Instance.Crear(evento);
            return evento;
        }
    }
}
