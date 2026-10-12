using System;

namespace Entidades
{
    // RN-25, RN-26, RN-37
    public class Suscripcion
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Plan { get; set; }          // Mensual|Anual
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Precio { get; set; }
        public bool Activa { get; set; }
    }
}
