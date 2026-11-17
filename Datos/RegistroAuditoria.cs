using System;

namespace Datos
{
    public sealed class RegistroAuditoria
    {
        public long Id { get; set; }
        public int? UsuarioId { get; set; }
        public string UsuarioMail { get; set; }
        public string Accion { get; set; }
        public string Entidad { get; set; }
        public int? EntidadId { get; set; }
        public DateTime Fecha { get; set; }
        public string Resultado { get; set; }
    }
}
