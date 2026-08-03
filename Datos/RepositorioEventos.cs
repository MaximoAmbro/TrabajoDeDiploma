using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace SistemaVentaEntradas
{

    public class Evento
    {
        public int Id { get; set; }
        public string NombreEstablecimiento { get; set; }
        public DateTime DiaHora { get; set; }
        public int CuposTotales { get; set; }
        public int LimiteMaximo { get; set; }

        public Evento(int id, string nombreEstablecimiento, DateTime diaHora, int cuposTotales, int limiteMaximo)
        {
            Id = id;
            NombreEstablecimiento = nombreEstablecimiento;
            DiaHora = diaHora;
            CuposTotales = cuposTotales;
            LimiteMaximo = limiteMaximo;
        }

        public Evento() { }
    }



}
namespace Datos
{
    interface RepositorioEventos
    {

    }
}
