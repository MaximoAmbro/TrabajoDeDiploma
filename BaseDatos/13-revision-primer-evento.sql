/* RN-54: el primer evento publicado por un Organizador nuevo requiere revisión del Administrador. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Eventos') AND name='CK_Eventos_Estado')
    ALTER TABLE dbo.Eventos DROP CONSTRAINT CK_Eventos_Estado;

ALTER TABLE dbo.Eventos WITH CHECK ADD CONSTRAINT CK_Eventos_Estado
    CHECK (Estado IN ('Borrador','EnRevision','Publicado','Suspendido'));

COMMIT TRANSACTION;
PRINT 'Estado EnRevision habilitado para el primer evento de cada organizador.';
