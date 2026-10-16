namespace Entidades
{
    // RF-06: datos que el cliente escribe en frmPago. Transitoria: NUNCA se persiste ni llega a
    // la capa Datos (RN-30). De la tarjeta solo se guardan los últimos 4 dígitos y el token (Pago).
    public class DatosTarjeta
    {
        public string Titular { get; set; }
        public string Numero { get; set; }
        public string Vencimiento { get; set; }   // MM/AA
        public string Cvv { get; set; }
    }
}
