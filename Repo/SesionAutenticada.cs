using Entidades;

namespace Negocio
{
    // Snapshot de identidad autenticada. No conserva referencias mutables ni credenciales.
    public sealed class SesionAutenticada
    {
        public int Id { get; }
        public string TipoUsuario { get; }
        public string Nombre { get; }
        public string Apellido { get; }
        public string Mail { get; }

        internal SesionAutenticada(Usuario usuario)
        {
            Id = usuario.Id;
            TipoUsuario = usuario.TipoUsuario;
            Nombre = usuario.Nombre;
            Apellido = usuario.Apellido;
            Mail = usuario.Mail;
        }
    }
}
