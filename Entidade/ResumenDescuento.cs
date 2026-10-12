namespace Entidades
{
    // Desglose que Negocio devuelve a Visual (orden de cálculo en docs/02-reglas-negocio.md).
    public class ResumenDescuento
    {
        public NivelFidelidad NivelAplicado { get; set; }
        public decimal BaseEntradas { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal SubtotalEntradas { get; set; }
        public decimal Servicios { get; set; }
        public decimal CargoServicio { get; set; }
        public decimal Total { get; set; }
    }
}
