using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasAutorizacionOrganizador
    {
        // CP-183 — RF-22, RN-05: separar la cuenta Administrador de plataforma de la cuenta Organizador de la semilla.
        [TestMethod("CP-183: Administrador sin locales y organizador de la semilla con acceso propio")]
        public void PerfilesDeSemillaSeparados()
        {
            Fake.Limpiar();
            var admin = Fake.Administrador();
            var organizador = Fake.Organizador();
            Assert.AreNotEqual(admin.Id, organizador.Id);
            Assert.AreEqual("Administrador", admin.TipoUsuario);
            Assert.AreEqual("Organizador", organizador.TipoUsuario);
            Assert.AreEqual(0, Datos.RepositorioLocales.Instance.ObtenerPorPropietario(admin.Id).Count);
            Assert.IsTrue(Datos.RepositorioLocales.Instance.ObtenerPorPropietario(organizador.Id).Count > 0);

            InterfazGrupoA.EnSta(() =>
            {
                using (var login = new Visual.frmInicio())
                {
                    InterfazGrupoA.Mostrar(login);
                    InterfazGrupoA.ControlDe<TextBox>(login, "txtMailLogin").Text = organizador.Mail;
                    InterfazGrupoA.ControlDe<TextBox>(login, "txtContraseñaLogin").Text = "Cliente123";
                    InterfazGrupoA.ControlDe<Button>(login, "btninicio").PerformClick();
                    Application.DoEvents();
                    var menu = Application.OpenForms.OfType<Visual.frmMenuPropietario>().Single(f => f.Visible);
                    Assert.AreEqual(organizador.Id, GestorSesion.Instance.UsuarioActual.Id);
                    Assert.AreEqual("Organizador", GestorSesion.Instance.UsuarioActual.TipoUsuario);
                    menu.Close();
                }
            });
        }

        [TestCleanup]
        public void Limpiar()
        {
            GestorSesion.Instance.Cerrar();
            Fake.Limpiar();
        }

        // CP-182 — RF-11/RF-22, RN-34/RN-49: limitar reportes y eventos al Organizador titular.
        [TestMethod("CP-182: Organizador solo ve sus eventos y cifras; Administrador conserva vista global")]
        public void ReportesYEventosQuedanLimitadosAlOrganizador()
        {
            Fake.Limpiar();
            var primero = Fake.PropietarioSinLocales();
            var segundo = Fake.PropietarioSinLocales();
            var eventoPropio = Fake.EventoPublicadoPara(primero, 1000m, 5);
            var eventoAjeno = Fake.EventoPublicadoPara(segundo, 2000m, 5);
            Fake.CambiarFechaDeEvento(eventoPropio.Id, DateTime.Now.AddMinutes(10));
            Fake.CambiarFechaDeEvento(eventoAjeno.Id, DateTime.Now.AddMinutes(10));
            var cliente1 = Fake.Cliente(Entidades.NivelFidelidad.Bronce);
            var cliente2 = Fake.Cliente(Entidades.NivelFidelidad.Bronce);
            Fake.CompraConfirmada(cliente1, eventoPropio, 1, false);
            Fake.CompraConfirmada(cliente2, eventoAjeno, 2, false);
            DateTime desde = DateTime.Today.AddDays(-1), hasta = DateTime.Today.AddYears(1);

            Fake.ComoOrganizador(primero);
            var reporte = GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, desde, hasta);
            var filas = GestorReportes.Instance.Ejecutar(reporte).Rows.Cast<System.Data.DataRow>()
                .Where(f => !Convert.ToString(f["Evento"]).StartsWith("TOTAL", StringComparison.Ordinal)).ToList();
            Assert.AreEqual(1, filas.Count, string.Join("; ", filas.Select(f => Convert.ToString(f["Evento"]) + "=" + Convert.ToString(f["Entradas"]))));
            Assert.AreEqual(eventoPropio.Nombre, Convert.ToString(filas[0]["Evento"]));
            Assert.AreEqual(1, Convert.ToInt32(filas[0]["Entradas"]));
            Assert.AreEqual(1, GestorReportes.Instance.EventosEntre(desde, hasta).Count);
            Assert.AreEqual(eventoPropio.Id, GestorReportes.Instance.EventosEntre(desde, hasta).Single().Id);

            var localAjeno = Datos.RepositorioLocales.Instance.ObtenerPorId(eventoAjeno.IdLocal);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.PorLocal(localAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.ObtenerPorId(eventoAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.Sectores(eventoAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.CapacidadTotal(eventoAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorLocales.Instance.PorId(localAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.EstaAgotado(eventoAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.ServiciosDe(eventoAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.DisponiblesDe(
                Datos.RepositorioSectores.Instance.ObtenerPorEvento(eventoAjeno.Id).Single().Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.Validar(
                new Entidades.Paquete { IdEvento = eventoAjeno.Id }));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorListaEspera.Instance.DeEvento(eventoAjeno.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorIngresos.Instance.Contar(eventoAjeno.Id));
            Assert.IsFalse(GestorIngresos.Instance.EventosConIngresoAbierto().Any(e => e.Id == eventoAjeno.Id));

            Fake.ComoAdministrador();
            var reporteGlobal = GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, desde, hasta);
            var filasGlobales = GestorReportes.Instance.Ejecutar(reporteGlobal).Rows.Cast<System.Data.DataRow>()
                .Where(f => !Convert.ToString(f["Evento"]).StartsWith("TOTAL", StringComparison.Ordinal)).ToList();
            var filaPropia = filasGlobales.Single(f => Convert.ToString(f["Evento"]) == eventoPropio.Nombre);
            var filaAjena = filasGlobales.Single(f => Convert.ToString(f["Evento"]) == eventoAjeno.Nombre);
            Assert.AreEqual(1, Convert.ToInt32(filaPropia["Entradas"]));
            Assert.AreEqual(2, Convert.ToInt32(filaAjena["Entradas"]));
            var eventosGlobales = GestorReportes.Instance.EventosEntre(desde, hasta);
            CollectionAssert.IsSubsetOf(new[] { eventoPropio.Id, eventoAjeno.Id }, eventosGlobales.Select(e => e.Id).ToArray());
        }
    }
}
