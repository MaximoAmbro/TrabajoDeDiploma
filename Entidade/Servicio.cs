namespace Entidades
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }        // Hospedaje|Traslado|Merchandising|VIP
        public decimal Precio { get; set; }
        public bool Activo { get; set; }
    }
}
