using System;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-08: nivel de fidelidad y descuento por nivel. Orden de cálculo en
    // docs/02-reglas-negocio.md. Umbrales y porcentajes salen de la tabla Parametros.
    public sealed class GestorFidelizacion
    {
        private static GestorFidelizacion _instance;

        public static GestorFidelizacion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorFidelizacion();
                }
                return _instance;
            }
        }

        private GestorFidelizacion() { }

        public Cliente ObtenerCliente(int idUsuario)
        {
            Cliente cliente = RepositorioFidelizacion.Instance.ObtenerCliente(idUsuario);
            if (cliente == null)
            {
                throw new InvalidOperationException("No se encontró ningún usuario con ese ID.");
            }
            return cliente;
        }

        public int ComprasEnVentana(int idUsuario, DateTime fecha)
        {
            int meses = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_VENTANA_MESES");
            return RepositorioFidelizacion.Instance.ContarComprasConfirmadas(
                idUsuario, fecha.AddMonths(-meses), fecha);
        }

        // RN-19 + RN-24: nivel según las compras confirmadas en la ventana móvil que termina en fecha.
        public NivelFidelidad DeterminarNivel(int idUsuario, DateTime fecha)
        {
            return NivelPorCantidad(ComprasEnVentana(idUsuario, fecha));
        }

        // RN-20
        public decimal PorcentajePara(NivelFidelidad nivel)
        {
            switch (nivel)
            {
                case NivelFidelidad.Oro:
                    return GestorParametros.Instance.ObtenerDecimal("DESCUENTO_ORO");
                case NivelFidelidad.Plata:
                    return GestorParametros.Instance.ObtenerDecimal("DESCUENTO_PLATA");
                default:
                    return 0m;   // RN-20: Bronce no tiene descuento
            }
        }

        // RN-21: el porcentaje se aplica solo a las entradas. RN-22: se usa el nivel persistido del
        // cliente, sin recalcularlo, para que la compra en curso no se beneficie a sí misma.
        // RN-37: la suscripción no suma porcentaje sobre las entradas. Su beneficio acá es el seguro:
        // con suscripción activa la prima se multiplica por (1 − SUSCRIPCION_SEGURO_DESCUENTO).
        public ResumenDescuento CalcularDescuento(Cliente c, Paquete p)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            decimal baseEntradas = GestorPaquetes.Instance.SubtotalEntradas(p);
            decimal porcentaje = PorcentajePara(c.Nivel);
            decimal monto = Redondear(baseEntradas * porcentaje);
            decimal subtotalEntradas = baseEntradas - monto;
            decimal servicios = GestorPaquetes.Instance.SubtotalServicios(p);
            decimal cargo = Redondear(subtotalEntradas * GestorParametros.Instance.ObtenerDecimal("CARGO_SERVICIO"));
            // RN-28: SEGURO_PRECIO por entrada, sin descuento por nivel ni cargo por servicio.
            decimal seguro = p.ConSeguro
                ? GestorPaquetes.Instance.CantidadEntradas(p) * GestorParametros.Instance.ObtenerDecimal("SEGURO_PRECIO")
                : 0m;
            if (seguro > 0m && GestorSuscripciones.Instance.TieneActiva(c.Id))
            {
                // RN-37: con 1.000 queda en 0. La póliza se graba igual: hay cobertura sin cobro.
                decimal bonificacion = GestorParametros.Instance.ObtenerDecimal("SUSCRIPCION_SEGURO_DESCUENTO");
                seguro = Redondear(seguro * (1m - bonificacion));
            }

            return new ResumenDescuento
            {
                NivelAplicado = c.Nivel,
                BaseEntradas = baseEntradas,
                Porcentaje = porcentaje,
                MontoDescuento = monto,
                SubtotalEntradas = subtotalEntradas,
                Servicios = servicios,
                CargoServicio = cargo,
                Seguro = seguro,
                Total = subtotalEntradas + servicios + cargo + seguro
            };
        }

        // RN-23: el nivel se reevalúa una vez por mes. Si ya se evaluó en el mes de fecha, no cambia.
        public void ReevaluarNivel(Cliente c, DateTime fecha)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            if (c.FechaUltimaEvaluacion.HasValue && !EsDeUnMesAnterior(c.FechaUltimaEvaluacion.Value, fecha))
            {
                return;
            }

            int compras = ComprasEnVentana(c.Id, fecha);
            NivelFidelidad nivel = NivelPorCantidad(compras);
            RepositorioFidelizacion.Instance.ActualizarNivel(c.Id, nivel, fecha);
            c.Nivel = nivel;
            c.FechaUltimaEvaluacion = fecha.Date;
            c.ComprasUltimos12Meses = compras;
        }

        // RN-22: al confirmarse una compra el nivel se recalcula y, si sube, rige desde la
        // operación siguiente. Un descenso no se aplica acá: espera a la reevaluación mensual
        // (RN-23; conflicto RN-23 vs RN-19 en docs/02-reglas-negocio.md).
        public void AscenderSiCorresponde(Cliente c, DateTime fecha)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            int compras = ComprasEnVentana(c.Id, fecha);
            NivelFidelidad nivel = NivelPorCantidad(compras);
            c.ComprasUltimos12Meses = compras;
            if (nivel <= c.Nivel)
            {
                return;
            }
            DateTime evaluacion = c.FechaUltimaEvaluacion ?? fecha;
            RepositorioFidelizacion.Instance.ActualizarNivel(c.Id, nivel, evaluacion);
            c.Nivel = nivel;
            c.FechaUltimaEvaluacion = evaluacion.Date;
        }

        // ------------------------------------- privados -------------------------------------

        // RN-19: Bronce por debajo de FIDELIDAD_UMBRAL_PLATA, Oro desde FIDELIDAD_UMBRAL_ORO.
        private static NivelFidelidad NivelPorCantidad(int compras)
        {
            if (compras >= GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_ORO"))
            {
                return NivelFidelidad.Oro;
            }
            if (compras >= GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_PLATA"))
            {
                return NivelFidelidad.Plata;
            }
            return NivelFidelidad.Bronce;
        }

        private static bool EsDeUnMesAnterior(DateTime evaluacion, DateTime fecha)
        {
            return evaluacion.Year * 12 + evaluacion.Month < fecha.Year * 12 + fecha.Month;
        }

        // Redondeo comercial a centavos: 0,005 sube.
        private static decimal Redondear(decimal importe)
        {
            return Math.Round(importe, 2, MidpointRounding.AwayFromZero);
        }
    }
}
