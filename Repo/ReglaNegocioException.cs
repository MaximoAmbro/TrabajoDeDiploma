using System;

namespace Negocio
{
    // Excepción de dominio: una regla de negocio rechazó la operación.
    // El formulario la atrapa y muestra Message al usuario.
    public class ReglaNegocioException : Exception
    {
        public string Regla { get; }

        public ReglaNegocioException(string regla, string mensaje)
            : base(mensaje)
        {
            Regla = regla;
        }
    }
}
