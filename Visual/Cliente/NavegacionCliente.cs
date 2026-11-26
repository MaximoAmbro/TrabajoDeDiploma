using System.Windows.Forms;

namespace Visual
{
    /// <summary>Puente para conservar los formularios independientes en pruebas y alojarlos en el cliente real.</summary>
    public static class NavegacionCliente
    {
        private static frmMenuCliente contenedor;

        internal static void Registrar(frmMenuCliente formulario) { contenedor = formulario; }
        internal static void Desregistrar(frmMenuCliente formulario)
        {
            if (contenedor == formulario) contenedor = null;
        }

        public static bool Activa => contenedor != null && !contenedor.IsDisposed && contenedor.Visible;

        public static bool AbrirDesdeMenu(Form pantalla)
        {
            if (!Activa) return false;
            contenedor.AbrirDesdeMenu(pantalla);
            return true;
        }

        public static bool Navegar(Form pantalla)
        {
            if (!Activa) return false;
            contenedor.NavegarDesdePantalla(pantalla);
            return true;
        }

        public static bool Volver()
        {
            if (!Activa) return false;
            contenedor.VolverPagina();
            return true;
        }

        public static bool Inicio()
        {
            if (!Activa) return false;
            contenedor.MostrarInicio();
            return true;
        }

        public static bool CerrarSesion()
        {
            if (!Activa) return false;
            contenedor.CerrarSesion();
            return true;
        }

        public static bool MostrarAviso(string texto, bool error, int duracionMs)
        {
            return Activa && contenedor.MostrarAvisoEnLinea(texto, error, duracionMs);
        }

        public static bool VerificarCorreo()
        {
            if (!Activa) return false;
            contenedor.IniciarVerificacionCorreo();
            return true;
        }
    }
}
