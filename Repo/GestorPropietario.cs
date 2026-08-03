using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Negocio;

namespace Repositorio
{
    public partial class GestorPropietario
    {
        private static GestorPropietario _instance;
        public static GestorPropietario Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorPropietario();
                }
                return _instance;
            }
        }
    } 
}
