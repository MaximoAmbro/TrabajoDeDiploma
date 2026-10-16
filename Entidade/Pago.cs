using System;

namespace Entidades
{
    // RN-30 + RN-31: un intento de pago de una compra. Puede haber varios por compra.
    public class Pago
    {
        public int Id { get; set; }
        public int IdCompra { get; set; }
        public DateTime FechaPago { get; set; }
        public EstadoPago Estado { get; set; }
        public string Ultimos4 { get; set; }    // RN-30: nunca el número completo
        public string Token { get; set; }
        public string Motivo { get; set; }      // solo si fue rechazado
    }
}
