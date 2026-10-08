namespace Datos
{
    public sealed class RepositorioEntradas
    {
        private static RepositorioEntradas _instance;

        public static RepositorioEntradas Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioEntradas();
                }
                return _instance;
            }
        }

        private RepositorioEntradas() { }
    }
}
