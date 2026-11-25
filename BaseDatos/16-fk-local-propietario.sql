/* RN-49: cada local debe tener un propietario que exista en Usuario.
   Ejecutar después de 01–15. Si hay datos huérfanos, corregirlos antes de agregar la relación. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF EXISTS (
    SELECT 1
    FROM dbo.Locales AS l
    LEFT JOIN dbo.Usuario AS u ON u.ID = l.PropietarioID
    WHERE l.PropietarioID IS NULL OR u.ID IS NULL
)
BEGIN
    ;THROW 51161, 'No se puede agregar FK_Locales_Propietario: hay locales sin un usuario propietario válido.', 1;
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('dbo.Locales')
      AND name = 'FK_Locales_Propietario'
)
    ALTER TABLE dbo.Locales WITH CHECK
        ADD CONSTRAINT FK_Locales_Propietario
        FOREIGN KEY (PropietarioID) REFERENCES dbo.Usuario(ID);

ALTER TABLE dbo.Locales WITH CHECK CHECK CONSTRAINT FK_Locales_Propietario;

COMMIT TRANSACTION;
