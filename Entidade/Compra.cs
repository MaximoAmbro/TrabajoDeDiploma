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
        public decimal TotalPagado { get; set; }
        public DateTime? ReservaVence { get; set; }            // RN-15
    }
}
