-- Ejecutar sobre la base de EventAura. Corrige los datos antes de imponer la restricción.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

UPDATE dbo.Entradas
SET CantidadVendida = 1
WHERE CantidadVendida IS NULL OR CantidadVendida <> 1;
DECLARE @FilasCorregidas INT = @@ROWCOUNT;
SELECT @FilasCorregidas AS FilasCantidadVendidaCorregidas;

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Entradas')
      AND name = 'CantidadVendida'
      AND is_nullable = 1
)
    ALTER TABLE dbo.Entradas ALTER COLUMN CantidadVendida INT NOT NULL;

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID('dbo.Entradas')
      AND name = 'CK_Entradas_CantidadVendida'
)
    ALTER TABLE dbo.Entradas WITH CHECK
        ADD CONSTRAINT CK_Entradas_CantidadVendida CHECK (CantidadVendida = 1);
ELSE
    ALTER TABLE dbo.Entradas WITH CHECK CHECK CONSTRAINT CK_Entradas_CantidadVendida;

COMMIT;
