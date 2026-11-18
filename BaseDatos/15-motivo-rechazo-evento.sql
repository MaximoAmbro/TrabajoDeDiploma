/* RN-54: guarda el motivo escrito cuando el Administrador devuelve a Borrador un evento en revisión. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Eventos','MotivoRechazo') IS NULL
    ALTER TABLE dbo.Eventos ADD MotivoRechazo NVARCHAR(500) NULL;

COMMIT TRANSACTION;
PRINT 'Motivo de rechazo del primer evento disponible para el Organizador.';
