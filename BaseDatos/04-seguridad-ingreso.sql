-- SEG-2: ejecutar una vez sobre la base de desarrollo existente, antes de usar el control.
-- No cambia estados ni importes; puede repetirse sin duplicar columnas o parámetros.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Entradas', 'FechaIngreso') IS NULL
    ALTER TABLE dbo.Entradas ADD FechaIngreso DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave = 'INGRESO_HORAS_ANTES')
    INSERT INTO dbo.Parametros (Clave, Valor, Descripcion, Regla)
    VALUES ('INGRESO_HORAS_ANTES', '6', 'Horas de ingreso antes del evento', 'RN-18');
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave = 'INGRESO_HORAS_DESPUES')
    INSERT INTO dbo.Parametros (Clave, Valor, Descripcion, Regla)
    VALUES ('INGRESO_HORAS_DESPUES', '12', 'Horas de ingreso después del evento', 'RN-18');
COMMIT;
