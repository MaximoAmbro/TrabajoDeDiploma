using System.Collections.Generic;

namespace Entidades
{
    // En memoria, sin tabla: lo que el cliente arma antes de confirmar la compra.
    public class Paquete
    {
        public int IdEvento { get; set; }
        public List<Entrada> Entradas { get; set; } = new List<Entrada>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
    }
}
