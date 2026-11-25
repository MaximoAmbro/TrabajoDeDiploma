-- Fase 5: la portada opcional viaja con la fila del evento.
-- Idempotente: no reemplaza imágenes cargadas.
IF COL_LENGTH('dbo.Eventos','ImagenPortada') IS NULL
    ALTER TABLE dbo.Eventos ADD ImagenPortada VARBINARY(MAX) NULL;
