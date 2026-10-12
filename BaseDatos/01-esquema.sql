/* ═══════════════════════════════════════════════════════════════════════════
   EventAura — 01-esquema.sql
   Crea el esquema completo. Ejecutar ANTES de 02-semilla.sql.

   Parte A: tablas preexistentes (Usuario, Cliente, Propietario, Locales, Eventos,
            Sectores, Entradas) con la estructura de la base actual. Se crean solo
            si no existen, así el script sirve tanto para una base vacía como para
            la base de desarrollo ya poblada.
   Parte B: DDL de docs/03-esquema-datos.md, sin cambios de nombres.
   ═══════════════════════════════════════════════════════════════════════════ */

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;   -- requerido por el índice filtrado UX_Suscripcion_Activa
GO

-- ═══════════════ Parte A: tablas preexistentes ═══════════════
IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
CREATE TABLE Usuario (
    ID          INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      VARCHAR(100) NULL,
    Apellido    VARCHAR(100) NULL,
    Mail        VARCHAR(150) NULL,
    Contraseña  VARCHAR(100) NULL,
    TipoUsuario VARCHAR(20)  NULL      -- Cliente|Propietario (RN-05)
);

IF OBJECT_ID('dbo.Cliente', 'U') IS NULL
CREATE TABLE Cliente (
    ID INT PRIMARY KEY,
    FOREIGN KEY (ID) REFERENCES Usuario(ID)
);

IF OBJECT_ID('dbo.Propietario', 'U') IS NULL
CREATE TABLE Propietario (
    ID INT PRIMARY KEY,
    FOREIGN KEY (ID) REFERENCES Usuario(ID)
);

IF OBJECT_ID('dbo.Locales', 'U') IS NULL
CREATE TABLE Locales (
    ID            INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        VARCHAR(100) NULL,
    Direccion     VARCHAR(100) NULL,
    Capacidad     INT          NULL,
    PropietarioID INT          NULL
);

IF OBJECT_ID('dbo.Eventos', 'U') IS NULL
CREATE TABLE Eventos (
    ID      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  VARCHAR(100) NULL,
    Fecha   DATETIME     NULL,
    LocalID INT          NULL,
    FOREIGN KEY (LocalID) REFERENCES Locales(ID)
);

IF OBJECT_ID('dbo.Sectores', 'U') IS NULL
CREATE TABLE Sectores (
    ID        INT IDENTITY(1,1) PRIMARY KEY,
    EventoID  INT           NULL,
    Nombre    VARCHAR(50)   NULL,
    Precio    DECIMAL(12,2) NULL,
    Capacidad INT           NULL,
    FOREIGN KEY (EventoID) REFERENCES Eventos(ID)
);

IF OBJECT_ID('dbo.Entradas', 'U') IS NULL
CREATE TABLE Entradas (
    ID              INT IDENTITY(1,1) PRIMARY KEY,
    ID_Evento       INT      NULL,
    ID_Usuario      INT      NULL,
    Precio          INT      NULL,
    CantidadVendida INT      NULL,
    FechaVenta      DATETIME NULL,
    FOREIGN KEY (ID_Evento) REFERENCES Eventos(ID)
);
GO

-- ═══════════════ Parte B: docs/03-esquema-datos.md ═══════════════

-- ═══════════════ Existentes: columnas nuevas ═══════════════
ALTER TABLE Usuario  ADD Nivel VARCHAR(8) NOT NULL DEFAULT 'Bronce';
ALTER TABLE Usuario  ADD FechaUltimaEvaluacion DATE NULL;
ALTER TABLE Usuario  ADD IntentosFallidos INT NOT NULL DEFAULT 0;
ALTER TABLE Usuario  ADD BloqueadoHasta DATETIME NULL;

ALTER TABLE Eventos  ADD Categoria VARCHAR(20) NULL;   -- Concierto|Deportivo|Teatro|Cultural
ALTER TABLE Eventos  ADD Estado VARCHAR(12) NOT NULL DEFAULT 'Publicado';

ALTER TABLE Sectores ADD CantidadReservada INT NOT NULL DEFAULT 0;  -- RN-08 + RN-15

ALTER TABLE Entradas ADD ID_Compra INT NULL;
ALTER TABLE Entradas ADD ID_Sector INT NULL;           -- RN-08: sin esto no hay cupo por sector
ALTER TABLE Entradas ADD Estado VARCHAR(12) NOT NULL DEFAULT 'Confirmada';
ALTER TABLE Entradas ADD CodigoQR VARCHAR(64) NULL;    -- RN-18

-- Los importes van en decimal (ver CLAUDE.md). Entradas.Precio era INT.
ALTER TABLE Entradas ALTER COLUMN Precio DECIMAL(12,2) NOT NULL;
GO

-- Constraints sobre las columnas nuevas: van en otro lote porque SQL Server no resuelve
-- en el mismo lote una columna agregada por ALTER TABLE.
ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Sectores
      FOREIGN KEY (ID_Sector) REFERENCES Sectores(ID);

CREATE UNIQUE INDEX UX_Entradas_CodigoQR                -- RN-18: único e irrepetible
    ON Entradas (CodigoQR) WHERE CodigoQR IS NOT NULL;

-- RN-01: la unicidad del correo es una restriccion estructural, no solo de codigo.
ALTER TABLE Usuario ADD CONSTRAINT UQ_Usuario_Mail UNIQUE (Mail);
GO

-- ═══════════════ Operación de compra ═══════════════
CREATE TABLE Compras (
    ID                INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario        INT           NOT NULL,
    FechaCompra       DATETIME      NOT NULL,
    Estado            VARCHAR(12)   NOT NULL,   -- Pendiente|Confirmada|Reembolsada
    NivelAplicado     VARCHAR(8)    NOT NULL,   -- congelado, RN-22
    DescuentoAplicado DECIMAL(4,3)  NOT NULL,   -- congelado, RN-16
    BaseEntradas      DECIMAL(12,2) NOT NULL,
    MontoDescuento    DECIMAL(12,2) NOT NULL,
    MontoServicios    DECIMAL(12,2) NOT NULL,
    CargoServicio     DECIMAL(12,2) NOT NULL,
    TotalPagado       DECIMAL(12,2) NOT NULL,
    ReservaVence      DATETIME      NULL,       -- RN-15
    CONSTRAINT FK_Compras_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID),
    CONSTRAINT CK_Compras_Estado CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada'))
);

ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Compras
      FOREIGN KEY (ID_Compra) REFERENCES Compras(ID);

CREATE INDEX IX_Compras_Usuario_Fecha
    ON Compras (ID_Usuario, FechaCompra) INCLUDE (Estado);   -- RN-19
GO

-- ═══════════════ Servicios complementarios ═══════════════
CREATE TABLE Servicios (
    ID          INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      VARCHAR(60)   NOT NULL,
    Tipo        VARCHAR(20)   NOT NULL,   -- Hospedaje|Traslado|Merchandising|VIP
    Precio      DECIMAL(12,2) NOT NULL,
    Activo      BIT           NOT NULL DEFAULT 1
);

CREATE TABLE ServiciosEvento (            -- RN-14: qué ofrece cada evento
    ID_Evento   INT NOT NULL,
    ID_Servicio INT NOT NULL,
    PRIMARY KEY (ID_Evento, ID_Servicio),
    FOREIGN KEY (ID_Evento)   REFERENCES Eventos(ID),
    FOREIGN KEY (ID_Servicio) REFERENCES Servicios(ID)
);

CREATE TABLE CompraServicios (            -- servicios de una compra concreta
    ID           INT IDENTITY(1,1) PRIMARY KEY,
    ID_Compra    INT           NOT NULL,
    ID_Servicio  INT           NOT NULL,
    Cantidad     INT           NOT NULL DEFAULT 1,
    PrecioUnit   DECIMAL(12,2) NOT NULL,   -- congelado, RN-16
    FOREIGN KEY (ID_Compra)   REFERENCES Compras(ID),
    FOREIGN KEY (ID_Servicio) REFERENCES Servicios(ID)
);
GO

-- ═══════════════ Seguro, suscripciones y lista de espera ═══════════════
CREATE TABLE Seguros (                    -- RN-28, RN-29
    ID          INT IDENTITY(1,1) PRIMARY KEY,
    ID_Entrada  INT           NOT NULL,
    Precio      DECIMAL(12,2) NOT NULL,
    VenceEl     DATETIME      NOT NULL,   -- 48 h antes del evento
    FOREIGN KEY (ID_Entrada) REFERENCES Entradas(ID)
);

CREATE TABLE Suscripciones (              -- RN-25, RN-26, RN-37
    ID          INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario  INT           NOT NULL,
    [Plan]      VARCHAR(10)   NOT NULL,   -- Mensual|Anual
    FechaInicio DATE          NOT NULL,
    FechaFin    DATE          NOT NULL,
    Precio      DECIMAL(12,2) NOT NULL,
    Activa      BIT           NOT NULL DEFAULT 1,
    FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID)
);
CREATE UNIQUE INDEX UX_Suscripcion_Activa                  -- RN-25
    ON Suscripciones (ID_Usuario) WHERE Activa = 1;

CREATE TABLE ListaEspera (                -- RN-35, RN-36
    ID           INT IDENTITY(1,1) PRIMARY KEY,
    ID_Evento    INT      NOT NULL,
    ID_Usuario   INT      NOT NULL,
    FechaAlta    DATETIME NOT NULL,
    CONSTRAINT UX_ListaEspera UNIQUE (ID_Evento, ID_Usuario),   -- RN-36
    FOREIGN KEY (ID_Evento)  REFERENCES Eventos(ID),
    FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID)
);
GO

-- ═══════════════ Parámetros del sistema ═══════════════
CREATE TABLE Parametros (
    Clave       VARCHAR(40)  PRIMARY KEY,
    Valor       VARCHAR(40)  NOT NULL,
    Descripcion VARCHAR(160) NULL,
    Regla       VARCHAR(10)  NULL
);
GO
