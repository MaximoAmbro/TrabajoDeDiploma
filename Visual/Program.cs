using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Las dos llamadas de Application van primero: SetCompatibleTextRenderingDefault
            // falla si ya existe una ventana, y ControlSesionInactiva.Instalar crea una.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ManejadorErrores.Instalar();
            ControlSesionInactiva.Instalar();
            SeleccionEnviadorCodigos.Actual = EnviadorSmtp.Seleccionar();
            Application.Run(new frmBienvenida());
        }
    }
}
