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
    }
}
