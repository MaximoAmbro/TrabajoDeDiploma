using System;

namespace Entidades
{
    // RN-35, RN-36
    public class ListaEspera
    {
        public int Id { get; set; }
        public int IdEvento { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
