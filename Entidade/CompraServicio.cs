namespace Entidades
{
    public class CompraServicio
    {
        public int Id { get; set; }
        public int IdCompra { get; set; }
        public int IdServicio { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }   // congelado, RN-16
    }
}
