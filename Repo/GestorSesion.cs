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
        public System.DateTime? IniciadaEl { get; private set; }
        public System.DateTime? UltimaActividadEl { get; private set; }

        public bool HaySesion
        {
            get { return UsuarioActual != null; }
        }

        internal void Iniciar(Usuario usuario)
        {
            if (usuario == null) throw new System.ArgumentNullException(nameof(usuario));
            UsuarioActual = new SesionAutenticada(usuario);
            IniciadaEl = System.DateTime.Now;
            UltimaActividadEl = IniciadaEl;
        }

        public void RegistrarActividad(System.DateTime? momento = null)
        {
            if (UsuarioActual != null) UltimaActividadEl = momento ?? System.DateTime.Now;
        }

        public bool DebeAvisarInactividad(System.DateTime ahora)
        {
            if (UsuarioActual == null || !UltimaActividadEl.HasValue) return false;
            int minutos = GestorParametros.Instance.ObtenerEntero("SESION_INACTIVIDAD_MIN");
            int aviso = GestorParametros.Instance.ObtenerEntero("SESION_AVISO_SEG");
            var vence = UltimaActividadEl.Value.AddMinutes(minutos);
            return ahora >= vence.AddSeconds(-aviso) && ahora < vence;
        }

        public bool DebeCerrarPorInactividad(System.DateTime ahora)
        {
            if (UsuarioActual == null || !UltimaActividadEl.HasValue) return false;
            return ahora >= UltimaActividadEl.Value.AddMinutes(GestorParametros.Instance.ObtenerEntero("SESION_INACTIVIDAD_MIN"));
        }

        // RN-34: acciones de administración de la plataforma, no de un local.
        public void RequerirAdministrador()
        {
            if (UsuarioActual == null || UsuarioActual.TipoUsuario != "Administrador")
                throw new ReglaNegocioException("RN-34", "Esta operación es solo para el Administrador");
        }

        // RN-48: la gestión de locales, eventos e ingresos corresponde al Organizador.
        public void RequerirOrganizador()
        {
            if (UsuarioActual == null || UsuarioActual.TipoUsuario != "Organizador")
                throw new ReglaNegocioException("RN-48", "Esta operación es solo para el Organizador.");
        }

        public void RequerirOrganizadorDeLocal(int idOrganizador)
        {
            RequerirOrganizador();
            if (UsuarioActual.Id != idOrganizador)
                throw new ReglaNegocioException("RN-49", "Solo podés operar sobre los locales de tu cuenta.");
        }

        public void RequerirUsuario(int idUsuario)
        {
            if (UsuarioActual == null || UsuarioActual.Id != idUsuario)
                throw new ReglaNegocioException("RN-41", "Solo puede operar sobre los datos de tu propia cuenta.");
        }

        public void Cerrar()
        {
            UsuarioActual = null;
            IniciadaEl = null;
            UltimaActividadEl = null;
        }
    }
}
