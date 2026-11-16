-- Elegir la base de EventAura en SSMS. Idempotente; no inventa liberaciones históricas.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Sectores', 'CupoLiberadoEl') IS NULL
    ALTER TABLE dbo.Sectores ADD CupoLiberadoEl DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='PRIORIDAD_ESPERA_MINUTOS')
    INSERT INTO dbo.Parametros (Clave, Valor, Descripcion, Regla)
    VALUES ('PRIORIDAD_ESPERA_MINUTOS', '15', 'Ventana exclusiva para suscriptores inscriptos tras liberar cupo', 'RN-35');
COMMIT;
