using Entidades;

namespace Negocio
{
    // Usuario autenticado en la aplicación. Lo carga GestorUsuarios.Autenticar.
    public sealed class GestorSesion
    {
        private static GestorSesion _instance;

        public static GestorSesion Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorSesion();
                }
                return _instance;
            }
        }

        private GestorSesion() { }

        public Usuario UsuarioActual { get; private set; }

        public bool HaySesion
        {
            get { return UsuarioActual != null; }
        }

        public void Iniciar(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public void Cerrar()
        {
            UsuarioActual = null;
        }
    }
}
