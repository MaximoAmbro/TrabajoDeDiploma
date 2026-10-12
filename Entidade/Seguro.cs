using System;

namespace Entidades
{
    // RN-28, RN-29
    public class Seguro
    {
        public int Id { get; set; }
        public int IdEntrada { get; set; }
        public decimal Precio { get; set; }
        public DateTime VenceEl { get; set; }
    }
}
