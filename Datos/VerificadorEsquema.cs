using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RequisitoEsquemaFaltante
    {
        public string Descripcion { get; set; }
        public string Script { get; set; }
    }

    /// <summary>Identifica la primera migración requerida por el ejecutable actual.</summary>
    public static class VerificadorEsquema
    {
        public static RequisitoEsquemaFaltante PrimerFaltante()
        {
            const string consulta = @"
SELECT TOP (1) Descripcion, Script
FROM (VALUES
    (1, CASE WHEN COL_LENGTH('dbo.Entradas','FechaIngreso') IS NULL THEN N'la columna Entradas.FechaIngreso' END, '04-seguridad-ingreso.sql'),
    (2, CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='INGRESO_HORAS_ANTES') THEN N'el parámetro INGRESO_HORAS_ANTES' END, '04-seguridad-ingreso.sql'),
    (3, CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='INGRESO_HORAS_DESPUES') THEN N'el parámetro INGRESO_HORAS_DESPUES' END, '04-seguridad-ingreso.sql'),
    (4, CASE WHEN NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Entradas') AND name='CK_Entradas_Estado' AND is_disabled=0 AND is_not_trusted=0) THEN N'la restricción Entradas.CK_Entradas_Estado' END, '05-check-estado-entradas.sql'),
    (5, CASE WHEN COL_LENGTH('dbo.Sectores','CupoLiberadoEl') IS NULL THEN N'la columna Sectores.CupoLiberadoEl' END, '06-prioridad-temporal.sql'),
    (6, CASE WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='PRIORIDAD_ESPERA_MINUTOS') THEN N'el parámetro PRIORIDAD_ESPERA_MINUTOS' END, '06-prioridad-temporal.sql'),
    (7, CASE WHEN NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Seguros') AND name='UX_Seguros_ID_Entrada' AND is_unique=1 AND is_disabled=0) THEN N'el índice único Seguros.UX_Seguros_ID_Entrada' END, '07-indice-unico-seguros.sql'),
    (8, CASE WHEN COL_LENGTH('dbo.Usuario','CorreoVerificado') IS NULL THEN N'la columna Usuario.CorreoVerificado'
             WHEN OBJECT_ID('dbo.CodigosCorreo','U') IS NULL THEN N'la tabla CodigosCorreo'
             WHEN OBJECT_ID('dbo.Auditoria','U') IS NULL THEN N'la tabla Auditoria'
             WHEN NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='PWD_FORTALEZA_MIN') THEN N'el parámetro PWD_FORTALEZA_MIN'
             ELSE NULL END, '08-seguridad-cuenta-auditoria.sql')
) AS requisitos(Orden, Descripcion, Script)
WHERE Descripcion IS NOT NULL
ORDER BY Orden;";
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
