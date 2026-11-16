-- Elegir la base de EventAura en SSMS. No borra ni fusiona seguros existentes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS (SELECT ID_Entrada FROM dbo.Seguros WITH (TABLOCKX,HOLDLOCK)
           GROUP BY ID_Entrada HAVING COUNT(*) > 1)
BEGIN
    SELECT ID_Entrada, COUNT(*) AS Cantidad FROM dbo.Seguros
    GROUP BY ID_Entrada HAVING COUNT(*) > 1;
    ROLLBACK;
    THROW 50007, 'Hay entradas con mas de un seguro. No se creo el indice ni se modificaron datos.', 1;
END;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Seguros') AND name='UX_Seguros_ID_Entrada')
    CREATE UNIQUE INDEX UX_Seguros_ID_Entrada ON dbo.Seguros (ID_Entrada);
COMMIT;
