using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Visual_.Conexion
{
    class Conexion
    {
        public string MiConexion = ConfigurationManager.ConnectionStrings["EventAura.Propiertis.Settings.BddEventAuraConnectionString"].ConnectionString;
    }
}
