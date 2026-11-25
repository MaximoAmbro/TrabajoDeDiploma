using System.Runtime.CompilerServices;

// Solo la capa de negocio puede escribir registros de auditoría a nombre de una identidad.
[assembly: InternalsVisibleTo("Repositorio")]
[assembly: InternalsVisibleTo("Pruebas")]
