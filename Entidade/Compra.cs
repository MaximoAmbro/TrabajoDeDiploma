using System;

namespace Entidades
{
    public class Compra
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaCompra { get; set; }
        public EstadoCompra Estado { get; set; }
        public NivelFidelidad NivelAplicado { get; set; }      // congelado, RN-22
        public decimal DescuentoAplicado { get; set; }         // congelado, RN-16
        public decimal BaseEntradas { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal MontoServicios { get; set; }
        public decimal CargoServicio { get; set; }
        public decimal MontoSeguro { get; set; }               // RN-28, sin descuento ni cargo
        public decimal TotalPagado { get; set; }               // lo que el cliente pagó; no cambia
        public decimal MontoReembolsado { get; set; }          // RN-29; neto = TotalPagado − MontoReembolsado
        public DateTime? ReservaVence { get; set; }            // RN-15; null = sin cupo reservado

        // RN-28: el cliente eligió el seguro. Con suscripción activa la prima puede ser 0 (RN-37),
        // así que MontoSeguro > 0 no alcanza para saber si se contrató.
        public bool ConSeguro { get; set; }

        // Sin columna: los completa la consulta del historial (CU-03).
        public int CantidadEntradas { get; set; }
        public string NombreEvento { get; set; }
    }
}
