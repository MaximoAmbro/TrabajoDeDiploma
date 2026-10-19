using System;

namespace Entidades
{
    // RN-35, RN-36
    public class ListaEspera
    {
        public int Id { get; set; }
        public int IdEvento { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool ConSuscripcion { get; set; }  // RN-35: congelado al inscribirse
        public Usuario Usuario { get; set; }      // para mostrar; lo completa la consulta
    }
}
