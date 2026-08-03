using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Entidades
{
    public partial class Entrada: Evento
    {
        public int Precio { get; set; }
        public int CantidadVendida { get; set; }
        public int IDEntrada { get; set; }
        public Cliente Cliente { get; set; }

    } 
}
