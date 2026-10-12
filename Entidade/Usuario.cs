using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Contraseña { get; set; }
        public string TipoUsuario { get; set; }       // Cliente|Propietario, RN-05
        public int IntentosFallidos { get; set; }     // RN-04
        public DateTime? BloqueadoHasta { get; set; } // RN-04
    }
}
