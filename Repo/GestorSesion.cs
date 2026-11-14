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

        public SesionAutenticada UsuarioActual { get; private set; }

        public bool HaySesion
        {
            get { return UsuarioActual != null; }
        }

        internal void Iniciar(Usuario usuario)
        {
            if (usuario == null) throw new System.ArgumentNullException(nameof(usuario));
            UsuarioActual = new SesionAutenticada(usuario);
        }

        // RN-34 generalizada: toda gestión administrativa exige este perfil.
        public void RequerirAdministrador()
        {
            if (UsuarioActual == null || UsuarioActual.TipoUsuario != "Propietario")
                throw new ReglaNegocioException("RN-34", "Esta operación es solo para el Administrador");
        }

        public void RequerirUsuario(int idUsuario)
        {
            if (UsuarioActual == null || (UsuarioActual.TipoUsuario != "Propietario" && UsuarioActual.Id != idUsuario))
                throw new ReglaNegocioException("RN-41", "Solo puede operar sobre los datos de tu propia cuenta.");
        }

        public void Cerrar()
        {
            UsuarioActual = null;
        }
    }
}
