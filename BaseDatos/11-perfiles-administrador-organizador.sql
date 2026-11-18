/* RN-05: separa Administrador de plataforma y Organizador.
   Ejecutar después de 01–10. Idempotente. La cuenta organizador de demostración
   se crea solo para conservar los locales que antes pertenecían a Ana. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID('dbo.Administrador','U') IS NULL
BEGIN
    CREATE TABLE dbo.Administrador (
        ID INT NOT NULL PRIMARY KEY,
        CONSTRAINT FK_Administrador_Usuario FOREIGN KEY (ID) REFERENCES dbo.Usuario(ID)
    );
END;

DECLARE @AdminID INT = (SELECT ID FROM dbo.Usuario WHERE Mail='admin@eventaura.com');
IF @AdminID IS NULL
    THROW 51101, 'No existe admin@eventaura.com; no se puede migrar el administrador de plataforma.', 1;

DECLARE @OrganizadorID INT = (SELECT ID FROM dbo.Usuario WHERE Mail='organizador@eventaura.com');
IF @OrganizadorID IS NULL
BEGIN
    INSERT dbo.Usuario(Nombre,Apellido,Mail,Contraseña,TipoUsuario,Nivel,CorreoVerificado)
    VALUES('Organizador','Demostración','organizador@eventaura.com',
           'PBKDF2$310000$KV/dZk+9huEPBtAM2mpBSQ==$bNLNWj5C/VqgxqOgyF5aANRLOjFWXZvwWOJR8JFbEZs=',
           'Organizador','Bronce',1);
    SET @OrganizadorID = SCOPE_IDENTITY();
END;

UPDATE dbo.Usuario SET TipoUsuario='Administrador' WHERE ID=@AdminID;
UPDATE dbo.Usuario SET TipoUsuario='Organizador' WHERE ID<>@AdminID AND TipoUsuario='Propietario';
UPDATE dbo.Usuario SET TipoUsuario='Organizador' WHERE ID=@OrganizadorID;

IF NOT EXISTS (SELECT 1 FROM dbo.Administrador WHERE ID=@AdminID)
    INSERT dbo.Administrador(ID) VALUES(@AdminID);
IF NOT EXISTS (SELECT 1 FROM dbo.Propietario WHERE ID=@OrganizadorID)
    INSERT dbo.Propietario(ID) VALUES(@OrganizadorID);

UPDATE dbo.Locales SET PropietarioID=@OrganizadorID WHERE PropietarioID=@AdminID;
DELETE dbo.Propietario WHERE ID=@AdminID;

IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE TipoUsuario NOT IN ('Cliente','Organizador','Administrador') OR TipoUsuario IS NULL)
    THROW 51102, 'Hay usuarios con un perfil no reconocido; revisá sus datos antes de agregar CK_Usuario_TipoUsuario.', 1;

IF OBJECT_ID('dbo.CK_Usuario_TipoUsuario','C') IS NULL
    ALTER TABLE dbo.Usuario WITH CHECK ADD CONSTRAINT CK_Usuario_TipoUsuario
        CHECK (TipoUsuario IN ('Cliente','Organizador','Administrador'));

COMMIT TRANSACTION;
PRINT 'Perfiles RN-05 migrados: Administrador y Organizador separados.';
