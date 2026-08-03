using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Repositorio;

namespace Negocio
{
    public partial class GestorClientes
    {
        private static GestorClientes _instance;

        public static GestorClientes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorClientes();
                }
                return _instance;
            }
        }

    }
}

