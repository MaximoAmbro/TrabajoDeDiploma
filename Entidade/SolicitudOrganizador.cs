using System;

namespace Entidades
{
    public sealed class SolicitudOrganizador
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Responsable { get; set; }
        public string Cuit { get; set; }
        public string Telefono { get; set; }
        public string NombreLocal { get; set; }
        public string Direccion { get; set; }
        public int Capacidad { get; set; }
        public byte[] Documentacion { get; set; }
        public string NombreArchivo { get; set; }
        public string CbuAlias { get; set; }
        public string Estado { get; set; }
        public string MotivoRevision { get; set; }
        public int? RevisadaPor { get; set; }
        public DateTime? RevisadaEl { get; set; }
    }
}
