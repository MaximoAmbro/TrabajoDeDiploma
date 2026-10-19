using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-06 + RF-07: cierra el flujo de compra. Iniciar reserva el cupo (RN-15), Confirmar cobra
    // con la pasarela simulada (RN-17) y emite las entradas con su QR (RN-18).
    public sealed class GestorCompras
    {
        // ISO/IEC 7812: longitud de un número de tarjeta. Es formato, no una regla del negocio.
        private const int DigitosTarjetaMin = 13;
        private const int DigitosTarjetaMax = 19;

        private static GestorCompras _instance;

        public static GestorCompras Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorCompras();
                }
                return _instance;
            }
        }

        private GestorCompras() { }

        // RN-15: deja la compra Pendiente con el cupo reservado RESERVA_MINUTOS. Los importes se
        // calculan con los precios ya congelados en el paquete (RN-16) y el nivel persistido del
        // cliente (RN-22); quedan congelados en la fila de Compras.
        public Compra Iniciar(Paquete p, Cliente c)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            GestorPaquetes.Instance.Validar(p);
            GestorPaquetes.Instance.VerificarPreventa(p.IdEvento, c.Id);   // RN-39
            ResumenDescuento resumen = GestorFidelizacion.Instance.CalcularDescuento(c, p);

            DateTime ahora = DateTime.Now;
            var compra = new Compra
            {
                IdUsuario = c.Id,
                FechaCompra = ahora,
                Estado = EstadoCompra.Pendiente,
                NivelAplicado = resumen.NivelAplicado,
                DescuentoAplicado = resumen.Porcentaje,
                BaseEntradas = resumen.BaseEntradas,
                MontoDescuento = resumen.MontoDescuento,
                MontoServicios = resumen.Servicios,
                CargoServicio = resumen.CargoServicio,
                MontoSeguro = resumen.Seguro,
                TotalPagado = resumen.Total,
                ReservaVence = ahora.AddMinutes(MinutosReserva()),
                ConSeguro = p.ConSeguro   // RN-37: con suscripción, MontoSeguro puede ser 0
            };

            // Una fila por entrada: cada una tendrá su QR y, si corresponde, su póliza.
            var entradas = new List<Entrada>();
            foreach (Entrada pedida in p.Entradas)
            {
                for (int i = 0; i < pedida.CantidadVendida; i++)
                {
                    entradas.Add(new Entrada
                    {
                        IdEvento = p.IdEvento,
                        IdSector = pedida.IdSector,
                        IdUsuario = c.Id,
                        Precio = pedida.Precio,   // RN-16: el congelado, no se relee el sector
                        CantidadVendida = 1,
                        FechaVenta = ahora,
                        Estado = EstadoEntrada.Pendiente
                    });
                }
            }
            List<CompraServicio> servicios = p.Servicios
                .Select(s => new CompraServicio { IdServicio = s.Id, Cantidad = 1, PrecioUnit = s.Precio })
                .ToList();

            try
            {
                RepositorioCompras.Instance.Iniciar(compra, servicios, entradas);
            }
            catch (CupoInsuficienteException ex)
            {
                throw CupoAgotado(ex.IdSector);
            }
            return compra;
        }

        // RN-17: la compra se confirma solo si la pasarela aprueba. Todo intento queda en Pagos
        // (RN-31) con los últimos 4 dígitos y el token, nunca el número (RN-30). Si se rechaza,
        // el cupo se libera en el acto (RN-32) y la compra sigue Pendiente para un reintento.
        public bool Confirmar(Compra c, DatosTarjeta t)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            ValidarTarjeta(t);

            Compra actual = RepositorioCompras.Instance.ObtenerPorId(c.Id);
            if (actual == null || actual.Estado != EstadoCompra.Pendiente)
            {
                throw new InvalidOperationException("La compra ya no está pendiente de pago.");
            }

            // RN-15: el plazo corre desde que se inició la compra, también en un reintento.
            DateTime ahora = DateTime.Now;
            DateTime vence = VencimientoDe(actual);
            if (ahora >= vence)
            {
                LiberarVencidas();
                c.Estado = EstadoCompra.Vencida;
                throw new ReglaNegocioException("RN-15",
                    "Venció el plazo de la reserva y se liberó el cupo. Vuelva a armar la compra.");
            }
            if (!actual.ReservaVence.HasValue)
            {
                // Reintento después de un rechazo: el cupo se había devuelto (RN-32).
                try
                {
                    RepositorioCompras.Instance.Rereservar(actual.Id, vence);
                }
                catch (CupoInsuficienteException ex)
                {
                    throw CupoAgotado(ex.IdSector);
                }
                c.ReservaVence = vence;
            }

            ResultadoPago resultado = PasarelaSimulada.Instance.Cobrar(t, actual.TotalPagado);
            RepositorioPagos.Instance.Insertar(new Pago
            {
                IdCompra = actual.Id,
                FechaPago = ahora,
                Estado = resultado.Aprobado ? EstadoPago.Aprobado : EstadoPago.Rechazado,
                Ultimos4 = resultado.Ultimos4,
                Token = resultado.Token,
                Motivo = resultado.Motivo
            });

            if (!resultado.Aprobado)
            {
                Liberar(c);
                return false;
            }

            List<Entrada> entradas = RepositorioEntradas.Instance.ObtenerPorCompra(actual.Id)
                .Where(e => e.Estado == EstadoEntrada.Pendiente)
                .ToList();
            foreach (Entrada entrada in entradas)
            {
                entrada.CodigoQR = GeneradorQR.Instance.NuevoCodigo();
            }
            List<Seguro> seguros = PolizasDe(actual, c.ConSeguro, entradas);

            if (!RepositorioCompras.Instance.Confirmar(actual.Id, entradas, ahora, seguros))
            {
                // El pago ya quedó registrado como aprobado: el rastro sirve para devolverlo.
                throw new InvalidOperationException(
                    "El pago se aprobó pero la reserva ya no estaba vigente. Consulte con soporte.");
            }
            c.Estado = EstadoCompra.Confirmada;

            // RN-22 + RN-23: el nivel nuevo rige desde la operación siguiente; esta ya se cobró
            // con el nivel congelado al iniciarla.
            Cliente cliente = GestorFidelizacion.Instance.ObtenerCliente(actual.IdUsuario);
            GestorFidelizacion.Instance.ReevaluarNivel(cliente, ahora);
            GestorFidelizacion.Instance.AscenderSiCorresponde(cliente, ahora);
            return true;
        }

        // RN-32: devuelve el cupo reservado. La compra queda Pendiente sin reserva: admite un
        // reintento dentro del plazo, y si no hay ninguno LiberarVencidas la cierra como Vencida.
        public void Liberar(Compra c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            RepositorioCompras.Instance.Liberar(c.Id);
            c.ReservaVence = null;
        }

        // RN-15: no hay proceso en segundo plano. Se llama al abrir la pantalla de compra y al
        // agotarse el temporizador de pago. Devuelve cuántas compras pasaron a Vencida.
        public int LiberarVencidas()
        {
            return RepositorioCompras.Instance.MarcarVencidas(DateTime.Now, MinutosReserva());
        }

        // Momento en que vence la reserva de la compra (RN-15), haya o no un rechazo en el medio.
        public DateTime VencimientoDe(Compra c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }
            return c.FechaCompra.AddMinutes(MinutosReserva());
        }

        public Compra ObtenerPorId(int idCompra)
        {
            return RepositorioCompras.Instance.ObtenerPorId(idCompra);
        }

        public List<Entrada> EntradasDe(Compra c)
        {
            return RepositorioEntradas.Instance.ObtenerPorCompra(c.Id);
        }

        public List<Pago> PagosDe(Compra c)
        {
            return RepositorioPagos.Instance.ObtenerPorCompra(c.Id);
        }

        public List<Seguro> SegurosDe(Compra c)
        {
            return RepositorioCompras.Instance.ObtenerSeguros(c.Id);
        }

        // CU-03: las compras del cliente. Pendiente y Vencida no van: son intentos que no se
        // pagaron, no compras.
        public List<Compra> HistorialDe(int idUsuario)
        {
            return RepositorioCompras.Instance.ObtenerPorUsuario(idUsuario,
                new[] { EstadoCompra.Confirmada, EstadoCompra.Reembolsada });
        }

        // RN-29: devuelve las entradas elegidas, todas de una misma compra. Cada una tiene que estar
        // Confirmada, tener póliza y faltar más de SEGURO_HORAS_COBERTURA para el evento.
        //
        // Se devuelve el precio congelado de cada entrada menos su parte del descuento por nivel, y
        // los servicios solo si con esto no queda ninguna entrada Confirmada. Nunca el cargo por
        // servicio ni la prima del seguro. El último reembolso de la compra devuelve lo que falte
        // para llegar a Base − Descuento + Servicios, así los redondeos parciales no dejan centavos.
        //
        // RN-24: una compra Reembolsada deja de contar para el nivel, porque el conteo filtra por
        // Confirmada. Una reembolsada en PARTE sigue Confirmada y sigue contando: la operación
        // existió. RN-23: el nivel no baja en el momento; espera a la reevaluación mensual.
        //
        // El cupo vuelve solo: el disponible cuenta únicamente las entradas Confirmada.
        // Devuelve el importe reembolsado.
        public decimal Reembolsar(List<Entrada> entradas)
        {
            if (entradas == null || entradas.Count == 0)
            {
                throw new ArgumentException("Elija al menos una entrada para devolver.");
            }
            List<int> ids = entradas.Select(e => e.Id).Distinct().ToList();
            int? idCompra = entradas[0].IdCompra;
            if (!idCompra.HasValue || entradas.Any(e => e.IdCompra != idCompra))
            {
                throw new ArgumentException("Las entradas a devolver tienen que ser de una misma compra.");
            }

            // Se relee todo de la base: lo que muestra la pantalla puede estar viejo.
            Compra compra = RepositorioCompras.Instance.ObtenerPorId(idCompra.Value);
            if (compra == null || compra.Estado != EstadoCompra.Confirmada)
            {
                throw new ReglaNegocioException("RN-29", "La compra no está confirmada: no hay nada para devolver.");
            }
            List<Entrada> todas = RepositorioEntradas.Instance.ObtenerPorCompra(compra.Id);
            List<Entrada> elegidas = todas.Where(e => ids.Contains(e.Id)).ToList();
            if (elegidas.Count != ids.Count)
            {
                throw new ArgumentException("Alguna de las entradas no pertenece a la compra.");
            }
            if (elegidas.Any(e => e.Estado != EstadoEntrada.Confirmada))
            {
                throw new ReglaNegocioException("RN-29", "Solo se pueden devolver entradas confirmadas.");
            }
            var aseguradas = new HashSet<int>(
                RepositorioCompras.Instance.ObtenerSeguros(compra.Id).Select(s => s.IdEntrada));
            if (elegidas.Any(e => !aseguradas.Contains(e.Id)))
            {
                throw new ReglaNegocioException("RN-29",
                    "Solo se pueden devolver entradas con seguro, y el seguro se contrata únicamente durante la compra.");
            }
            int horas = GestorParametros.Instance.ObtenerEntero("SEGURO_HORAS_COBERTURA");
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(elegidas[0].IdEvento);
            DateTime limite = evento.Fecha.AddHours(-horas);
            if (DateTime.Now >= limite)
            {
                throw new ReglaNegocioException("RN-29",
                    $"El seguro permite devolver hasta {horas} horas antes del evento " +
                    $"(hasta el {limite:dd/MM/yyyy HH:mm}). Ese plazo ya pasó.");
            }

            bool completa = todas.Count(e => e.Estado == EstadoEntrada.Confirmada) == elegidas.Count;
            decimal monto = completa
                ? compra.BaseEntradas - compra.MontoDescuento + compra.MontoServicios - compra.MontoReembolsado
                : elegidas.Sum(e => e.Precio - ParteDelDescuento(compra, e.Precio));

            if (!RepositorioCompras.Instance.Reembolsar(compra.Id, ids, monto, completa))
            {
                throw new InvalidOperationException(
                    "La compra cambió mientras se procesaba la devolución. Vuelva a abrir el historial.");
            }
            return monto;
        }

        // Formato de la tarjeta. Se valida antes de cobrar, así un dato mal tipeado no gasta un
        // intento en Pagos.
        public void ValidarTarjeta(DatosTarjeta t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }
            if (string.IsNullOrWhiteSpace(t.Titular))
            {
                throw new ArgumentException("Ingrese el nombre del titular de la tarjeta.");
            }
            string numero = (t.Numero ?? "").Replace(" ", "").Replace("-", "");
            if (!numero.All(char.IsDigit) || numero.Length < DigitosTarjetaMin || numero.Length > DigitosTarjetaMax)
            {
                throw new ArgumentException(
                    $"El número de tarjeta debe tener entre {DigitosTarjetaMin} y {DigitosTarjetaMax} dígitos.");
            }
            string cvv = (t.Cvv ?? "").Trim();
            if (!cvv.All(char.IsDigit) || cvv.Length < 3 || cvv.Length > 4)
            {
                throw new ArgumentException("El código de seguridad debe tener 3 o 4 dígitos.");
            }
            DateTime vence;
            if (!DateTime.TryParseExact((t.Vencimiento ?? "").Trim(), "MM/yy", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out vence))
            {
                throw new ArgumentException("El vencimiento debe tener el formato MM/AA.");
            }
            // Vale hasta el último día del mes impreso.
            if (vence.AddMonths(1) <= DateTime.Today)
            {
                throw new ArgumentException("La tarjeta está vencida.");
            }
        }

        // ------------------------------------- privados -------------------------------------

        private static int MinutosReserva()
        {
            return GestorParametros.Instance.ObtenerEntero("RESERVA_MINUTOS");
        }

        // RN-28 + RN-29: una póliza por entrada, al precio congelado al iniciar la compra. Cubre
        // hasta SEGURO_HORAS_COBERTURA antes del evento. Con suscripción el precio puede ser 0
        // (RN-37): la póliza se graba igual, porque el cliente tiene cobertura aunque no la pagó.
        private static List<Seguro> PolizasDe(Compra compra, bool conSeguro, List<Entrada> entradas)
        {
            var seguros = new List<Seguro>();
            if (!(conSeguro || compra.MontoSeguro > 0m) || entradas.Count == 0)
            {
                return seguros;
            }
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(entradas[0].IdEvento);
            int horas = GestorParametros.Instance.ObtenerEntero("SEGURO_HORAS_COBERTURA");
            decimal precio = compra.MontoSeguro / entradas.Count;
            foreach (Entrada entrada in entradas)
            {
                seguros.Add(new Seguro
                {
                    IdEntrada = entrada.Id,
                    Precio = precio,
                    VenceEl = evento.Fecha.AddHours(-horas)
                });
            }
            return seguros;
        }

        // RN-21: el descuento por nivel se aplicó solo sobre las entradas; a cada una le toca la
        // parte proporcional a su precio.
        private static decimal ParteDelDescuento(Compra compra, decimal precio)
        {
            if (compra.BaseEntradas <= 0m)
            {
                return 0m;
            }
            return Math.Round(compra.MontoDescuento * precio / compra.BaseEntradas, 2, MidpointRounding.AwayFromZero);
        }

        private static ReglaNegocioException CupoAgotado(int idSector)
        {
            Sector sector = RepositorioSectores.Instance.ObtenerPorId(idSector);
            int disponibles = Math.Max(0, RepositorioSectores.Instance.ObtenerDisponibles(idSector));
            return new ReglaNegocioException("RN-08",
                $"En el sector {sector?.Nombre} quedan {disponibles} " +
                (disponibles == 1 ? "entrada." : "entradas.") + " Ajuste la cantidad y vuelva a intentar.");
        }
    }
}
