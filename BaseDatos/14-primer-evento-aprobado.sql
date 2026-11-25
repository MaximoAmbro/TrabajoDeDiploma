/* RN-54: conserva la aprobación histórica del primer evento, incluso si luego se suspende o elimina. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Propietario','PrimerEventoAprobado') IS NULL
    ALTER TABLE dbo.Propietario ADD PrimerEventoAprobado BIT NOT NULL
        CONSTRAINT DF_Propietario_PrimerEventoAprobado DEFAULT (0) WITH VALUES;
GO

-- Los organizadores que ya tienen una publicación se consideran aprobados al migrar.
UPDATE p SET PrimerEventoAprobado=1
FROM dbo.Propietario p
WHERE EXISTS (SELECT 1 FROM dbo.Locales l JOIN dbo.Eventos e ON e.LocalID=l.ID
              WHERE l.PropietarioID=p.ID AND e.Estado='Publicado');

COMMIT TRANSACTION;
PRINT 'Se conservó el estado de aprobación inicial de los organizadores.';
