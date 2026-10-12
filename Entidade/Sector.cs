using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Sector
    {
        public int Id { get; set; }
        public int IdEvento { get; set; }             // Sectores.EventoID, RN-08
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Capacidad { get; set; }
        public int CantidadReservada { get; set; }    // RN-08 + RN-15
    }
}
