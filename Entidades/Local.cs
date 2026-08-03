using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Local()
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int Capacidad { get; set; }
        public List<Evento> Eventos { get; set; }
    }
}
