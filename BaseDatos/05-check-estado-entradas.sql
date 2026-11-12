-- Ejecutar sobre la base existente de EventAura, después de elegir la base correcta en SSMS.
-- No corrige ni borra filas: si aparece un estado desconocido, muestra el valor y se detiene.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS (
    SELECT 1 FROM dbo.Entradas WITH (TABLOCKX, HOLDLOCK)
    WHERE Estado IS NULL OR NOT EXISTS (
        SELECT 1 FROM (VALUES ('Pendiente'), ('Confirmada'), ('Reembolsada'), ('Vencida')) p(Estado)
        WHERE CONVERT(VARBINARY(8000), dbo.Entradas.Estado) = CONVERT(VARBINARY(8000), p.Estado)
    )
)
BEGIN
    SELECT Estado, COUNT(*) AS Cantidad FROM dbo.Entradas
    WHERE Estado IS NULL OR NOT EXISTS (
        SELECT 1 FROM (VALUES ('Pendiente'), ('Confirmada'), ('Reembolsada'), ('Vencida')) p(Estado)
        WHERE CONVERT(VARBINARY(8000), dbo.Entradas.Estado) = CONVERT(VARBINARY(8000), p.Estado)
    ) GROUP BY Estado;
    ROLLBACK;
    THROW 50001, 'Hay estados de entradas fuera del conjunto. No se agrego la restriccion ni se modificaron datos.', 1;
END;
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints
               WHERE parent_object_id = OBJECT_ID('dbo.Entradas') AND name = 'CK_Entradas_Estado')
    ALTER TABLE dbo.Entradas WITH CHECK ADD CONSTRAINT CK_Entradas_Estado
        CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida'));
ALTER TABLE dbo.Entradas WITH CHECK CHECK CONSTRAINT CK_Entradas_Estado;
COMMIT;
