using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Entidades
{
    public partial class Entrada
    {
        public decimal Precio { get; set; }
        public int CantidadVendida { get; set; }
        public int Id { get; set; }
        public Cliente Cliente { get; set; }
        public int IdEvento { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaVenta { get; set; }
        public int? IdSector { get; set; }            // RN-08
        public int? IdCompra { get; set; }
        public EstadoEntrada Estado { get; set; }
        public string CodigoQR { get; set; }          // RN-18

    } 
}
