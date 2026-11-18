using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RequisitoEsquemaFaltante
    {
        public string Descripcion { get; set; }
        public string Script { get; set; }
    }

    /// <summary>Registra, en orden, los cambios de esquema requeridos por este ejecutable.</summary>
    public static class VerificadorEsquema
    {
        // Mantener una entrada por cada script de migración numerado desde 04.
        // PruebasEsquemaInicio compara este registro con BaseDatos/*.sql.
        public static readonly IReadOnlyList<string> ScriptsRegistrados = Array.AsReadOnly(new[]
        {
            "04-seguridad-ingreso.sql",
            "05-check-estado-entradas.sql",
            "06-prioridad-temporal.sql",
            "07-indice-unico-seguros.sql",
            "08-seguridad-cuenta-auditoria.sql",
            "09-imagenes-eventos.sql",
            "10-restringir-cantidad-vendida.sql",
            "11-perfiles-administrador-organizador.sql",
            "12-solicitudes-organizador.sql",
            "13-revision-primer-evento.sql",
            "14-primer-evento-aprobado.sql",
            "15-motivo-rechazo-evento.sql"
        });

        public static RequisitoEsquemaFaltante PrimerFaltante()
        {
            const string consulta = @"
WITH requisitos(Orden, Descripcion, Script) AS
(
    SELECT * FROM (VALUES
      (4, COALESCE(CASE WHEN COL_LENGTH('dbo.Entradas','FechaIngreso') IS NULL THEN N'la columna Entradas.FechaIngreso' END,
                      CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='INGRESO_HORAS_ANTES') THEN N'el parámetro INGRESO_HORAS_ANTES' END,
                      CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='INGRESO_HORAS_DESPUES') THEN N'el parámetro INGRESO_HORAS_DESPUES' END), '04-seguridad-ingreso.sql'),
      (5, CASE WHEN NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Entradas') AND name='CK_Entradas_Estado' AND is_disabled=0 AND is_not_trusted=0) THEN N'la restricción Entradas.CK_Entradas_Estado' END, '05-check-estado-entradas.sql'),
      (6, COALESCE(CASE WHEN COL_LENGTH('dbo.Sectores','CupoLiberadoEl') IS NULL THEN N'la columna Sectores.CupoLiberadoEl' END,
                      CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='PRIORIDAD_ESPERA_MINUTOS') THEN N'el parámetro PRIORIDAD_ESPERA_MINUTOS' END), '06-prioridad-temporal.sql'),
      (7, CASE WHEN NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Seguros') AND name='UX_Seguros_ID_Entrada' AND is_unique=1 AND is_disabled=0) THEN N'el índice único Seguros.UX_Seguros_ID_Entrada' END, '07-indice-unico-seguros.sql'),
      (8, COALESCE(CASE WHEN COL_LENGTH('dbo.Usuario','CorreoVerificado') IS NULL THEN N'la columna Usuario.CorreoVerificado' END,
                      CASE WHEN OBJECT_ID('dbo.CodigosCorreo','U') IS NULL THEN N'la tabla CodigosCorreo' END,
                      CASE WHEN OBJECT_ID('dbo.Auditoria','U') IS NULL THEN N'la tabla Auditoria' END,
                      CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='PWD_FORTALEZA_MIN') THEN N'el parámetro PWD_FORTALEZA_MIN' END), '08-seguridad-cuenta-auditoria.sql'),
      (9, CASE WHEN COL_LENGTH('dbo.Eventos','ImagenPortada') IS NULL THEN N'la columna Eventos.ImagenPortada' END, '09-imagenes-eventos.sql'),
      (10, COALESCE(CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Entradas') AND name='CantidadVendida' AND is_nullable=1) THEN N'Entradas.CantidadVendida NOT NULL' END,
                       CASE WHEN NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Entradas') AND name='CK_Entradas_CantidadVendida' AND is_disabled=0 AND is_not_trusted=0) THEN N'la restricción Entradas.CK_Entradas_CantidadVendida' END), '10-restringir-cantidad-vendida.sql'),
      (11, COALESCE(CASE WHEN OBJECT_ID('dbo.Administrador','U') IS NULL THEN N'la tabla Administrador' END,
                       CASE WHEN NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Usuario') AND name='CK_Usuario_TipoUsuario' AND is_disabled=0 AND is_not_trusted=0) THEN N'la restricción Usuario.CK_Usuario_TipoUsuario' END), '11-perfiles-administrador-organizador.sql'),
      (12, COALESCE(CASE WHEN OBJECT_ID('dbo.SolicitudesOrganizador','U') IS NULL THEN N'la tabla SolicitudesOrganizador' END,
                       CASE WHEN COL_LENGTH('dbo.CodigosCorreo','Destinatario') IS NULL THEN N'la columna CodigosCorreo.Destinatario' END), '12-solicitudes-organizador.sql'),
      (13, CASE WHEN NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Eventos') AND name='CK_Eventos_Estado' AND definition LIKE '%EnRevision%' AND is_disabled=0 AND is_not_trusted=0) THEN N'el estado Eventos.EnRevision' END, '13-revision-primer-evento.sql'),
      (14, CASE WHEN COL_LENGTH('dbo.Propietario','PrimerEventoAprobado') IS NULL THEN N'la columna Propietario.PrimerEventoAprobado' END, '14-primer-evento-aprobado.sql'),
      (15, CASE WHEN COL_LENGTH('dbo.Eventos','MotivoRechazo') IS NULL THEN N'la columna Eventos.MotivoRechazo' END, '15-motivo-rechazo-evento.sql')
    ) AS valores(Orden, Descripcion, Script)
)
SELECT TOP (1) Descripcion, Script FROM requisitos
WHERE Descripcion IS NOT NULL ORDER BY Orden;";
            using (var conexion = Conexion.Instance.Crear())
            using (var comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    if (!lector.Read()) return null;
                    return new RequisitoEsquemaFaltante
                    {
                        Descripcion = lector.GetString(0),
                        Script = lector.GetString(1)
                    };
                }
            }
        }
    }
}
