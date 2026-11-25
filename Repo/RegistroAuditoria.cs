using System;

namespace Negocio
{
    // Vista de auditoría que cruza el límite de Datos solo después de la autorización del gestor.
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
