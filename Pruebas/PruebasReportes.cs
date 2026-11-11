using System;
using System.Data;
using System.Linq;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasReportes
    {
        private static DateTime Desde => DateTime.Today;
        private static DateTime Hasta => DateTime.Today.AddDays(31);

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

        // CP-35 — RF-11, RN-34: solo una sesión de Administrador permite crear y ejecutar reportes.
        [TestMethod("CP-35: Solo el Administrador accede a reportes")]
        public void CP35_SoloAdministradorAccede()
        {
            ReglaNegocioException sinSesion = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, Desde, Hasta));
            Assert.AreEqual("RN-34", sinSesion.Regla);

            GestorSesion.Instance.Iniciar(Fake.Cliente(NivelFidelidad.Bronce));
            ReglaNegocioException comoCliente = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, Desde, Hasta));
            Assert.AreEqual("RN-34", comoCliente.Regla);

            GestorSesion.Instance.Iniciar(Fake.Administrador());
            Reporte reporte = GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, Desde, Hasta);
            Assert.IsNotNull(reporte);
            Assert.IsNotNull(GestorReportes.Instance.Ejecutar(reporte));
        }

        // CP-36 — RF-11: RPT-01 suma entradas confirmadas y excluye la reserva pendiente.
        [TestMethod("CP-36: RPT-01 cuenta solo entradas confirmadas")]
        public void CP36_VentasCuentaSoloEntradasConfirmadas()
        {
            Evento evento = Fake.EventoPublicado(25000m, 10);
            Cliente primero = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(primero);
            Cliente segundo = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(segundo);
            Cliente tercero = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(tercero);
            Fake.CompraConfirmada(primero, evento, 1, false);
            Fake.CompraConfirmada(segundo, evento, 2, false);
            Fake.ComoCliente(tercero);
            GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 1, false), tercero);
            GestorSesion.Instance.Iniciar(Fake.Administrador());

            Reporte reporte = GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, Desde, Hasta);
            DataTable tabla = GestorReportes.Instance.Ejecutar(reporte);
            DataRow fila = tabla.Rows.Cast<DataRow>().Single(f => Convert.ToString(f["Evento"]) == evento.Nombre);

            Assert.AreEqual(3, Convert.ToInt32(fila["Entradas"]));
            Assert.AreEqual(75000m, Convert.ToDecimal(fila["Ingreso bruto"]));
            Assert.AreEqual(25000m, Convert.ToDecimal(fila["Ticket prom."]));
            Assert.AreEqual(30.0m, Convert.ToDecimal(fila["% ocup."]));
        }

        // CP-37 — RF-11, RF-10, RN-10, RN-36: agotado derivado, inscripción única y cupo recuperado.
        [TestMethod("CP-37: RPT-05 refleja agotado, espera y devolución")]
        public void CP37_DemandaAgotadoYListaDeEspera()
        {
            Evento evento = Fake.EventoPublicado(1000m, 1);
            Cliente comprador = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(comprador);
            Cliente enEspera = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(enEspera);
            Compra compra = Fake.CompraConfirmada(comprador, evento, 1, true);
            Fake.ComoCliente(enEspera);
            GestorListaEspera.Instance.Inscribir(evento.Id, enEspera.Id);
            ReglaNegocioException duplicada = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorListaEspera.Instance.Inscribir(evento.Id, enEspera.Id));
            Assert.AreEqual("RN-36", duplicada.Regla);
            GestorSesion.Instance.Iniciar(Fake.Administrador());
            var reporte = (ReporteDemandaInsatisfecha)GestorReportes.Instance.Crear(ReporteDemandaInsatisfecha.CodigoReporte, Desde, Hasta);
            reporte.IdEvento = evento.Id;
            reporte.NombreEvento = evento.Nombre;

            DataRow agotado = GestorReportes.Instance.Ejecutar(reporte).Rows.Cast<DataRow>()
                .Single(f => Convert.ToString(f["Evento"]) == evento.Nombre);
            Assert.AreEqual(ReporteDemandaInsatisfecha.EstadoAgotado, Convert.ToString(agotado["Estado"]));
            Assert.AreEqual(1, Convert.ToInt32(agotado["En lista de espera"]));

            GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(compra));
            DataRow conCupo = GestorReportes.Instance.Ejecutar(reporte).Rows.Cast<DataRow>()
                .Single(f => Convert.ToString(f["Evento"]) == evento.Nombre);
            Assert.AreEqual(ReporteDemandaInsatisfecha.EstadoConCupo, Convert.ToString(conCupo["Estado"]));
            reporte.SoloAgotados = true;
            Assert.IsFalse(GestorReportes.Instance.Ejecutar(reporte).Rows.Cast<DataRow>()
                .Any(f => Convert.ToString(f["Evento"]) == evento.Nombre));
        }

        // CP-38 — RF-11: exportador recibe los datos del reporte y se rechazan período/código inválidos.
        [TestMethod("CP-38: Exportación de datos y rechazo de período o reporte inválido")]
        public void CP38_ExportacionYPeriodoInvalido()
        {
            Evento evento = Fake.EventoPublicado(1000m, 10);
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(cliente);
            Fake.CompraConfirmada(cliente, evento, 1, false);
            GestorSesion.Instance.Iniciar(Fake.Administrador());
            Reporte reporte = GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, Desde, Hasta);
            var exportador = new ExportadorDePrueba();

            string ruta = GestorReportes.Instance.Exportar(reporte, exportador, "x.pdf");

            Assert.AreEqual("x.pdf", ruta);
            Assert.IsNotNull(exportador.TablaRecibida);
            Assert.AreEqual(GestorReportes.Instance.Ejecutar(reporte).Rows.Count, exportador.TablaRecibida.Rows.Count);
            Assert.ThrowsException<ArgumentException>(() => GestorReportes.Instance.Crear("RPT-01", Hasta, Desde));
            Assert.ThrowsException<ArgumentException>(() => GestorReportes.Instance.Crear("RPT-99", Desde, Hasta));
        }

        private sealed class ExportadorDePrueba : IExportador
        {
            public DataTable TablaRecibida { get; private set; }

            public string Exportar(Reporte reporte, DataTable datos, string ruta)
            {
                TablaRecibida = datos;
                return ruta;
            }
        }
    }
}
