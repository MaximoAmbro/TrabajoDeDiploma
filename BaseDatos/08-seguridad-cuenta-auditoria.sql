-- RN-43 a RN-47: verificación y recuperación de correo, auditoría y sesión.
-- Idempotente: conserva cuentas y registros existentes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Usuario','CorreoVerificado') IS NULL
    ALTER TABLE dbo.Usuario ADD CorreoVerificado BIT NOT NULL
        CONSTRAINT DF_Usuario_CorreoVerificado DEFAULT (1) WITH VALUES;

IF OBJECT_ID('dbo.CodigosCorreo','U') IS NULL
BEGIN
    CREATE TABLE dbo.CodigosCorreo (
        ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UsuarioID INT NOT NULL,
        Proposito VARCHAR(16) NOT NULL,
        Sal VARBINARY(16) NOT NULL,
        CodigoHash VARBINARY(32) NOT NULL,
        CreadoEl DATETIME2(0) NOT NULL CONSTRAINT DF_CodigosCorreo_Creado DEFAULT SYSUTCDATETIME(),
        VenceEl DATETIME2(0) NOT NULL,
        Usado BIT NOT NULL CONSTRAINT DF_CodigosCorreo_Usado DEFAULT (0),
        Intentos INT NOT NULL CONSTRAINT DF_CodigosCorreo_Intentos DEFAULT (0),
        CONSTRAINT CK_CodigosCorreo_Proposito CHECK (Proposito IN ('Verificacion','Recuperacion')),
        CONSTRAINT FK_CodigosCorreo_Usuario FOREIGN KEY (UsuarioID) REFERENCES dbo.Usuario(ID) ON DELETE CASCADE
    );
    CREATE INDEX IX_CodigosCorreo_Usuario_Proposito ON dbo.CodigosCorreo(UsuarioID, Proposito, VenceEl DESC);
END;

IF OBJECT_ID('dbo.Auditoria','U') IS NULL
BEGIN
    CREATE TABLE dbo.Auditoria (
        ID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UsuarioID INT NULL,
        UsuarioMail VARCHAR(150) NULL,
        Accion VARCHAR(80) NOT NULL,
        Entidad VARCHAR(80) NULL,
        EntidadID INT NULL,
        Fecha DATETIME2(0) NOT NULL CONSTRAINT DF_Auditoria_Fecha DEFAULT SYSUTCDATETIME(),
        Resultado VARCHAR(200) NOT NULL
    );
    CREATE INDEX IX_Auditoria_Fecha_Accion ON dbo.Auditoria(Fecha DESC, Accion);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='CORREO_CODIGO_MIN')
    INSERT dbo.Parametros(Clave,Valor,Descripcion,Regla) VALUES ('CORREO_CODIGO_MIN','10','Vigencia del código enviado por correo, en minutos','RN-43');
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='CORREO_CODIGO_INTENTOS_MAX')
    INSERT dbo.Parametros(Clave,Valor,Descripcion,Regla) VALUES ('CORREO_CODIGO_INTENTOS_MAX','5','Intentos máximos para usar un código de correo','RN-43');
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='SESION_INACTIVIDAD_MIN')
    INSERT dbo.Parametros(Clave,Valor,Descripcion,Regla) VALUES ('SESION_INACTIVIDAD_MIN','15','Minutos de inactividad antes de cerrar sesión','RN-46');
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='SESION_AVISO_SEG')
    INSERT dbo.Parametros(Clave,Valor,Descripcion,Regla) VALUES ('SESION_AVISO_SEG','60','Segundos de aviso antes del cierre por inactividad','RN-46');
IF NOT EXISTS (SELECT 1 FROM dbo.Parametros WHERE Clave='PWD_FORTALEZA_MIN')
    INSERT dbo.Parametros(Clave,Valor,Descripcion,Regla) VALUES ('PWD_FORTALEZA_MIN','4','Puntaje mínimo de fortaleza de contraseña','RN-47');

COMMIT;
