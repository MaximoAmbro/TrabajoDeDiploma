using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Cliente: Usuario
    {
        public List<Evento> entradasUsuario = new List<Evento>();

        public NivelFidelidad Nivel { get; set; }                  // persistido, RN-22
        public DateTime? FechaUltimaEvaluacion { get; set; }       // RN-23
        public int ComprasUltimos12Meses { get; set; }             // RN-19
    }
}
