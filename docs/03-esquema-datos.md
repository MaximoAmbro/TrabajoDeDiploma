# Esquema de datos

Tablas ya existentes: `Usuario`, `Propietario`, `Locales`, `Eventos`, `Sectores`, `Entradas`.
El resto se crea. Los `ALTER` sobre tablas existentes están marcados.

## Por qué `Compras` es una entidad propia

`Entradas` guarda una fila por sector adquirido. RN-19 cuenta **operaciones**, no entradas:
sin agrupar, un cliente que compra Campo y Platea en la misma transacción asciende de nivel
al doble de velocidad. Además RN-16 y RN-22 exigen congelar el nivel y el porcentaje
aplicados, y esos son atributos de la operación, no de cada entrada.

## DDL

```sql
-- ═══════════════ Existentes: columnas nuevas ═══════════════
ALTER TABLE Usuario  ADD Nivel VARCHAR(8) NOT NULL DEFAULT 'Bronce';
ALTER TABLE Usuario  ADD FechaUltimaEvaluacion DATE NULL;
ALTER TABLE Usuario  ADD IntentosFallidos INT NOT NULL DEFAULT 0;
ALTER TABLE Usuario  ADD BloqueadoHasta DATETIME NULL;

ALTER TABLE Eventos  ADD Categoria VARCHAR(20) NULL;   -- Concierto|Deportivo|Teatro|Cultural
ALTER TABLE Eventos  ADD Estado VARCHAR(12) NOT NULL DEFAULT 'Publicado';

ALTER TABLE Sectores ADD CantidadReservada INT NOT NULL DEFAULT 0;  -- RN-08 + RN-15

ALTER TABLE Entradas ADD ID_Compra INT NULL;
ALTER TABLE Entradas ADD Estado VARCHAR(12) NOT NULL DEFAULT 'Confirmada';
ALTER TABLE Entradas ADD CodigoQR VARCHAR(64) NULL;    -- RN-18, único

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
    Plan        VARCHAR(10)   NOT NULL,   -- Mensual|Anual
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

-- ═══════════════ Parámetros del sistema ═══════════════
CREATE TABLE Parametros (
    Clave       VARCHAR(40)  PRIMARY KEY,
    Valor       VARCHAR(40)  NOT NULL,
    Descripcion VARCHAR(160) NULL,
    Regla       VARCHAR(10)  NULL
);
```

## Consulta clave — conteo de la ventana móvil (RN-19)

```sql
SELECT COUNT(*)
FROM   Compras
WHERE  ID_Usuario  =  @IdUsuario
  AND  Estado      = 'Confirmada'
  AND  FechaCompra >  DATEADD(MONTH, -12, @FechaReferencia)
  AND  FechaCompra <= @FechaReferencia;
```

## Datos semilla mínimos

- Los 13 registros de `Parametros` de `docs/02-reglas-negocio.md`.
- Un usuario Administrador y tres Clientes, uno por nivel.
- Un local con tres eventos: uno vigente con cupo, uno agotado, uno con fecha pasada.
- Tres sectores por evento con precios distintos.
- Cuatro servicios, uno de cada tipo, habilitados en el evento vigente.
