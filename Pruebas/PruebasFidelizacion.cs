using System;
using System.Collections.Generic;
using System.Linq;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    // docs/04-casos-prueba.md: CP-01 a CP-10 y la verificación numérica de referencia.
    // Son de integración contra BddEventAura_Pruebas (ver BaseDePrueba). Los resultados esperados
    // son los del documento: si uno no da, es un hallazgo, no se ajusta el código al resultado.
    [TestClass]
    public class PruebasFidelizacion
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
            // Un Cliente que queda en sesión haría fallar a la prueba siguiente por otro motivo.
            GestorSesion.Instance.Cerrar();
        }

        // La base de pruebas queda como la dejó la semilla.
        [ClassCleanup]
        public static void LimpiarAlFinal()
        {
            Fake.Limpiar();
        }

        // CP-01 a CP-04 — RN-19 + RN-20: compras confirmadas en la ventana → nivel y descuento.
        [DataTestMethod]
        [DataRow(2, NivelFidelidad.Bronce, "0.000", DisplayName = "CP-01: 2 compras → Bronce, 0 %")]
        [DataRow(3, NivelFidelidad.Plata, "0.050", DisplayName = "CP-02: 3 compras → Plata, 5 %")]
        [DataRow(6, NivelFidelidad.Plata, "0.050", DisplayName = "CP-03: 6 compras → Plata, no asciende")]
        [DataRow(7, NivelFidelidad.Oro, "0.075", DisplayName = "CP-04: 7 compras → Oro, 7,5 %")]
        public void CP01a04_NivelYDescuentoSegunCompras(int compras, NivelFidelidad esperado, string porcentaje)
        {
            Cliente c = Fake.ClienteConCompras(compras, 1);

            NivelFidelidad nivel = GestorFidelizacion.Instance.DeterminarNivel(c.Id, DateTime.Now);

            Assert.AreEqual(esperado, nivel);
            Assert.AreEqual(decimal.Parse(porcentaje, System.Globalization.CultureInfo.InvariantCulture),
                            GestorFidelizacion.Instance.PorcentajePara(nivel));
        }

        // CP-05 — RN-21: Oro, entradas por $40.000 y un servicio de $30.000. El descuento es
        // $3.000 y el servicio no se descuenta.
        [TestMethod]
        public void CP05_ElDescuentoNoAlcanzaALosServicios()
        {
            Cliente c = Fake.Cliente(NivelFidelidad.Oro);
            Evento e = Fake.EventoPublicado(40000m, 100);
            Servicio hospedaje = Fake.ServicioHabilitado(e, "Hospedaje", 30000m);
            Paquete p = Fake.Paquete(e, 1, false);
            GestorPaquetes.Instance.AgregarServicio(p, hospedaje.Id);

            ResumenDescuento r = GestorFidelizacion.Instance.CalcularDescuento(c, p);

            Assert.AreEqual(40000m, r.BaseEntradas);
            Assert.AreEqual(3000m, r.MontoDescuento);
            Assert.AreEqual(37000m, r.SubtotalEntradas);
            Assert.AreEqual(30000m, r.Servicios, "El hospedaje no recibe descuento (RN-21).");
            Assert.AreEqual(r.SubtotalEntradas + r.Servicios + r.CargoServicio + r.Seguro, r.Total);
        }

        // CP-06 — RN-19: 7 compras, una de hace 13 meses: queda fuera de la ventana → Plata.
        [TestMethod]
        public void CP06_LaCompraDeHace13MesesQuedaFueraDeLaVentana()
        {
            Cliente c = Fake.ClienteConCompras(6, 1);
            Fake.AgregarCompra(c, 13, EstadoCompra.Confirmada);

            Assert.AreEqual(6, GestorFidelizacion.Instance.ComprasEnVentana(c.Id, DateTime.Now));
            Assert.AreEqual(NivelFidelidad.Plata, GestorFidelizacion.Instance.DeterminarNivel(c.Id, DateTime.Now));
        }

        // CP-07 — RN-16: cambia la tarifa con la compra ya iniciada; se factura el precio congelado.
        [TestMethod]
        public void CP07_SeFacturaElPrecioCongelado()
        {
            Cliente c = Fake.Cliente(NivelFidelidad.Bronce);
            Evento e = Fake.EventoPublicado(25000m, 100);
            Paquete p = Fake.Paquete(e, 2, false);
            Compra compra = GestorCompras.Instance.Iniciar(p, c);
            decimal totalAlIniciar = compra.TotalPagado;

            Fake.CambiarPrecioDeSector(e.Sectores[0].Id, 30000m);
            Assert.IsTrue(GestorCompras.Instance.Confirmar(compra, Fake.Tarjeta()));

            Compra cobrada = GestorCompras.Instance.ObtenerPorId(compra.Id);
            Assert.AreEqual(EstadoCompra.Confirmada, cobrada.Estado);
            Assert.AreEqual(50000m, cobrada.BaseEntradas);
            Assert.AreEqual(totalAlIniciar, cobrada.TotalPagado);
            Assert.IsTrue(GestorCompras.Instance.EntradasDe(cobrada).All(x => x.Precio == 25000m),
                          "Cada entrada conserva el precio congelado al iniciar.");
            // Una operación nueva ya toma la tarifa nueva.
            Assert.AreEqual(60000m, GestorPaquetes.Instance.SubtotalEntradas(Fake.Paquete(e, 2, false)));
        }

        // CP-08 — RN-22: con 6 compras hace la séptima. Se cobra al 5 % (Plata) y Oro rige desde
        // la octava. Recorre Iniciar y Confirmar enteros.
        [TestMethod]
        public void CP08_LaSeptimaSeCobraAl5YOroRigeDesdeLaOctava()
        {
            Cliente c = Fake.ClienteConCompras(6, 1);
            Assert.AreEqual(NivelFidelidad.Plata, c.Nivel);
            Evento e = Fake.EventoPublicado(25000m, 100);

            Compra septima = Fake.CompraConfirmada(c, e, 1, false);

            Assert.AreEqual(NivelFidelidad.Plata, septima.NivelAplicado);
            Assert.AreEqual(0.050m, septima.DescuentoAplicado);
            Assert.AreEqual(1250m, septima.MontoDescuento);
            Assert.AreEqual(7, GestorFidelizacion.Instance.ComprasEnVentana(c.Id, DateTime.Now));
            Cliente despues = GestorFidelizacion.Instance.ObtenerCliente(c.Id);
            Assert.AreEqual(NivelFidelidad.Oro, despues.Nivel, "El nivel sube al confirmarse la séptima.");

            Compra octava = Fake.CompraConfirmada(c, e, 1, false);
            Assert.AreEqual(NivelFidelidad.Oro, octava.NivelAplicado);
            Assert.AreEqual(0.075m, octava.DescuentoAplicado);
        }

        // CP-09 — RN-23 + RN-24: Oro con 7 compras devuelve una. El contador baja a 6; el nivel
        // persistido se mantiene hasta la reevaluación mensual.
        [TestMethod]
        public void CP09_ReembolsoBajaElContadorPeroNoElNivel()
        {
            Cliente c = Fake.ClienteConCompras(6, 1);
            Evento e = Fake.EventoPublicado(25000m, 100);
            Compra septima = Fake.CompraConfirmada(c, e, 2, true);
            Assert.AreEqual(NivelFidelidad.Oro, GestorFidelizacion.Instance.ObtenerCliente(c.Id).Nivel);
            Assert.AreEqual(7, GestorFidelizacion.Instance.ComprasEnVentana(c.Id, DateTime.Now));

            GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(septima));

            Assert.AreEqual(EstadoCompra.Reembolsada, GestorCompras.Instance.ObtenerPorId(septima.Id).Estado);
            Assert.AreEqual(6, GestorFidelizacion.Instance.ComprasEnVentana(c.Id, DateTime.Now), "RN-24");
            Cliente despues = GestorFidelizacion.Instance.ObtenerCliente(c.Id);
            Assert.AreEqual(NivelFidelidad.Oro, despues.Nivel, "RN-23: el nivel no baja en el momento.");

            DateTime proximoMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1);
            GestorFidelizacion.Instance.ReevaluarNivel(despues, proximoMes);
            Assert.AreEqual(NivelFidelidad.Plata, despues.Nivel, "En la reevaluación mensual pasa a Plata.");
        }

        // CP-10 — RN-37: Oro con suscripción activa. 7,5 % y ni un punto más.
        [TestMethod]
        public void CP10_LaSuscripcionNoSumaPorcentaje()
        {
            Cliente c = Fake.Cliente(NivelFidelidad.Oro);
            Assert.IsNotNull(Fake.SuscripcionActiva(c, GestorSuscripciones.PlanAnual));
            Evento e = Fake.EventoPublicado(40000m, 100);

            ResumenDescuento r = GestorFidelizacion.Instance.CalcularDescuento(c, Fake.Paquete(e, 1, false));

            Assert.AreEqual(0.075m, r.Porcentaje);
            Assert.AreEqual(3000m, r.MontoDescuento);
        }

        // Verificación numérica de referencia: Plata, 2 entradas Campo a $25.000 y hospedaje de
        // $45.000 dan EXACTAMENTE $99.625. Si da otro número hay un error en la implementación o
        // en el documento.
        [TestMethod]
        public void Referencia_DaExactamente99625()
        {
            Cliente c = Fake.Cliente(NivelFidelidad.Plata);
            Evento e = Fake.EventoPublicado(25000m, 100);
            Servicio hospedaje = Fake.ServicioHabilitado(e, "Hospedaje", 45000m);
            Paquete p = Fake.Paquete(e, 2, false);
            GestorPaquetes.Instance.AgregarServicio(p, hospedaje.Id);

            ResumenDescuento r = GestorFidelizacion.Instance.CalcularDescuento(c, p);

            var pasos = new List<Tuple<string, decimal, decimal>>
            {
                Tuple.Create("1. Base de cálculo", 50000m, r.BaseEntradas),
                Tuple.Create("2. Porcentaje Plata", 0.050m, r.Porcentaje),
                Tuple.Create("3. Monto del descuento", 2500m, r.MontoDescuento),
                Tuple.Create("4. Subtotal de entradas", 47500m, r.SubtotalEntradas),
                Tuple.Create("5. Hospedaje", 45000m, r.Servicios),
                Tuple.Create("6. Cargo por servicio", 7125m, r.CargoServicio),
                Tuple.Create("7. Total a pagar", 99625m, r.Total)
            };
            foreach (var paso in pasos)
            {
                Assert.AreEqual(paso.Item2, paso.Item3, paso.Item1);
            }
        }
    }
}
