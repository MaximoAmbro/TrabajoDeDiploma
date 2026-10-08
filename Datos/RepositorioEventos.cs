namespace Datos
{
    public sealed class RepositorioEventos
    {
        private static RepositorioEventos _instance;

        public static RepositorioEventos Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioEventos();
                }
                return _instance;
            }
        }

        private RepositorioEventos() { }
    }
}
