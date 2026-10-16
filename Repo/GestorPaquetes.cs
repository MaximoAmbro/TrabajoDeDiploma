using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-04 y RF-05: armado del paquete (entradas de varios sectores + servicios) antes de
    // confirmar la compra. El paquete vive en memoria; no tiene tabla. La compra la registra
    // GestorCompras.
    public sealed class GestorPaquetes
    {
        private static GestorPaquetes _instance;

        public static GestorPaquetes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorPaquetes();
                }
                return _instance;
            }
        }

        private GestorPaquetes() { }

        // RN-14: solo los servicios habilitados para el evento.
        public List<Servicio> ServiciosDe(int idEvento)
        {
            return RepositorioServicios.Instance.ObtenerPorEvento(idEvento);
        }

        // RN-09: solo se arma un paquete para un evento Publicado con fecha futura.
        public Paquete Nuevo(int idEvento)
        {
            VerificarEventoVigente(idEvento);
            return new Paquete { IdEvento = idEvento };
        }

        // Fija la cantidad pedida de un sector; 0 lo quita del paquete. Se llama cada vez que
        // el cliente cambia la cantidad, así que reemplaza la anterior en lugar de sumarle.
        public void AgregarEntradas(Paquete p, int idSector, int cantidad)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            if (cantidad < 0)
            {
                throw new ArgumentException("La cantidad no puede ser negativa.");
            }
            Sector sector = RepositorioSectores.Instance.ObtenerPorId(idSector);
            if (sector == null || sector.IdEvento != p.IdEvento)
            {
                throw new ArgumentException("El sector no pertenece al evento del paquete.");
            }

            Entrada existente = p.Entradas.FirstOrDefault(x => x.IdSector == idSector);
            if (cantidad == 0)
            {
                if (existente != null)
                {
                    p.Entradas.Remove(existente);
                }
                return;
            }

            VerificarCupo(sector, cantidad);
            int enOtrosSectores = p.Entradas.Where(x => x.IdSector != idSector).Sum(x => x.CantidadVendida);
            VerificarMaximoPorOperacion(enOtrosSectores + cantidad, enOtrosSectores);

            if (existente == null)
            {
                existente = new Entrada { IdEvento = p.IdEvento, IdSector = idSector };
                p.Entradas.Add(existente);
            }
            existente.Precio = sector.Precio;   // RN-33: pesos, decimal
            existente.CantidadVendida = cantidad;
        }

        public void AgregarServicio(Paquete p, int idServicio)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            if (p.Servicios.Any(s => s.Id == idServicio))
            {
                return;
            }
            Servicio servicio = ServiciosDe(p.IdEvento).FirstOrDefault(s => s.Id == idServicio);
            if (servicio == null)
            {
                throw new ReglaNegocioException("RN-14",
                    "Ese servicio no está habilitado para este evento.");
            }
            p.Servicios.Add(servicio);
        }

        public void QuitarServicio(Paquete p, int idServicio)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            p.Servicios.RemoveAll(s => s.Id == idServicio);
        }

        // Revalida todo el paquete antes de confirmarlo: el cupo pudo cambiar mientras se armaba.
        public void Validar(Paquete p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            VerificarEventoVigente(p.IdEvento);

            int total = CantidadEntradas(p);
            if (total == 0)
            {
                throw new ReglaNegocioException("RN-13",
                    "El paquete debe tener al menos una entrada: los servicios no se venden solos.");
            }
            VerificarMaximoPorOperacion(total, 0);

            foreach (Entrada entrada in p.Entradas)
            {
                Sector sector = RepositorioSectores.Instance.ObtenerPorId(entrada.IdSector.Value);
                if (sector == null || sector.IdEvento != p.IdEvento)
                {
                    throw new ArgumentException("El paquete tiene un sector que no pertenece al evento.");
                }
                VerificarCupo(sector, entrada.CantidadVendida);
            }

            List<Servicio> habilitados = ServiciosDe(p.IdEvento);
            foreach (Servicio servicio in p.Servicios)
            {
                if (!habilitados.Any(h => h.Id == servicio.Id))
                {
                    throw new ReglaNegocioException("RN-14",
                        $"El servicio «{servicio.Nombre}» ya no está habilitado para este evento.");
                }
            }
        }

        // RN-08 + RN-15
        public int DisponiblesDe(int idSector)
        {
            return Math.Max(0, RepositorioSectores.Instance.ObtenerDisponibles(idSector));
        }

        public int CantidadEntradas(Paquete p)
        {
            return p.Entradas.Sum(x => x.CantidadVendida);
        }

        // Base de cálculo de RN-21: solo entradas. El descuento lo aplica GestorFidelizacion.
        public decimal SubtotalEntradas(Paquete p)
        {
            return p.Entradas.Sum(x => x.Precio * x.CantidadVendida);
        }

        public decimal SubtotalServicios(Paquete p)
        {
            return p.Servicios.Sum(s => s.Precio);
        }

        // ------------------------------------- privados -------------------------------------

        private static void VerificarEventoVigente(int idEvento)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null)
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            if (evento.Estado != GestorEventos.EstadoPublicado || evento.Fecha <= DateTime.Now)
            {
                throw new ReglaNegocioException("RN-09",
                    "El evento no está disponible para la venta: ya pasó o no está publicado.");
            }
        }

        // RN-08: lo pedido no supera el cupo disponible del sector.
        private void VerificarCupo(Sector sector, int cantidad)
        {
            int disponibles = DisponiblesDe(sector.Id);
            if (cantidad > disponibles)
            {
                throw new ReglaNegocioException("RN-08",
                    $"En el sector {sector.Nombre} quedan {disponibles} " +
                    (disponibles == 1 ? "entrada." : "entradas."));
            }
        }

        // RN-12: máximo COMPRA_ENTRADAS_MAX entradas por operación, sumando todos los sectores.
        private static void VerificarMaximoPorOperacion(int total, int enOtrosSectores)
        {
            int maximo = GestorParametros.Instance.ObtenerEntero("COMPRA_ENTRADAS_MAX");
            if (total > maximo)
            {
                string detalle = enOtrosSectores > 0
                    ? $" Ya tiene {enOtrosSectores} en otros sectores."
                    : "";
                throw new ReglaNegocioException("RN-12",
                    $"Se pueden comprar como máximo {maximo} entradas por operación.{detalle}");
            }
        }
    }
}
