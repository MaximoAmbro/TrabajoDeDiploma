# Esquema de datos

Tablas ya existentes: `Usuario`, `Cliente`, `Propietario`, `Locales`, `Eventos`, `Sectores`,
`Entradas`. El resto se crea. Los `ALTER` sobre tablas existentes están marcados.

> `Cliente` y `Propietario` son tablas de especialización: solo tienen `ID`, clave foránea
> hacia `Usuario`. El perfil efectivo se resuelve por `Usuario.TipoUsuario` (RN-05).

## Estructura de las tablas preexistentes

Estas tablas ya existían antes de esta etapa. Se documentan porque sus claves primarias y
foráneas también deben tener propiedad en la capa `Entidades`: sin ellas la capa `Datos` no
puede mapear un `SELECT` ni resolver una relación.

```sql
Usuario      (ID PK, Nombre, Apellido, Mail, Contraseña, TipoUsuario)
Cliente      (ID PK/FK → Usuario.ID)
Propietario  (ID PK/FK → Usuario.ID)
Locales      (ID PK, Nombre, Direccion, Capacidad, PropietarioID FK → Usuario.ID)
Eventos      (ID PK, Nombre, Fecha, LocalID FK → Locales.ID)
Sectores     (ID PK, EventoID FK → Eventos.ID, Nombre, Precio, Capacidad)
Entradas     (ID PK, ID_Evento FK → Eventos.ID, ID_Usuario, Precio, CantidadVendida, FechaVenta)
```

`TipoUsuario` toma los valores `Cliente` o `Propietario`, y es el que resuelve RN-05.
`Cliente` y `Propietario` son tablas de especialización: solo guardan el `ID`.

`Eventos` no tiene columna de hora: `Fecha` es `DATETIME` y la incluye. La entidad `Evento`
conserva una propiedad `Hora` heredada que no corresponde a ninguna columna.

```sql
-- Los importes van en decimal (ver CLAUDE.md). Entradas.Precio era INT.
ALTER TABLE Entradas ALTER COLUMN Precio DECIMAL(12,2) NOT NULL;
```

## Por qué `Compras` es una entidad propia

`Entradas` guarda una fila por entrada vendida (desde la Sección 7; ver «Ciclo de vida de una
compra»). RN-19 cuenta **operaciones**, no entradas: sin agrupar, un cliente que compra tres
entradas en la misma transacción asciende de nivel al triple de velocidad. Además RN-16 y RN-22 exigen congelar el nivel y el porcentaje
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
ALTER TABLE Entradas ADD ID_Sector INT NULL;           -- RN-08: sin esto no hay cupo por sector
ALTER TABLE Entradas ADD Estado VARCHAR(12) NOT NULL DEFAULT 'Confirmada';
ALTER TABLE Entradas ADD CodigoQR VARCHAR(64) NULL;    -- RN-18

ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Sectores
      FOREIGN KEY (ID_Sector) REFERENCES Sectores(ID);

CREATE UNIQUE INDEX UX_Entradas_CodigoQR                -- RN-18: único e irrepetible
    ON Entradas (CodigoQR) WHERE CodigoQR IS NOT NULL;

-- RN-01: la unicidad del correo es una restriccion estructural, no solo de codigo.
ALTER TABLE Usuario ADD CONSTRAINT UQ_Usuario_Mail UNIQUE (Mail);

-- ═══════════════ Operación de compra ═══════════════
CREATE TABLE Compras (
    ID                INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario        INT           NOT NULL,
    FechaCompra       DATETIME      NOT NULL,
    Estado            VARCHAR(12)   NOT NULL,   -- Pendiente|Confirmada|Reembolsada|Vencida
    NivelAplicado     VARCHAR(8)    NOT NULL,   -- congelado, RN-22
    DescuentoAplicado DECIMAL(4,3)  NOT NULL,   -- congelado, RN-16
    BaseEntradas      DECIMAL(12,2) NOT NULL,
    MontoDescuento    DECIMAL(12,2) NOT NULL,
    MontoServicios    DECIMAL(12,2) NOT NULL,
    CargoServicio     DECIMAL(12,2) NOT NULL,
    MontoSeguro       DECIMAL(12,2) NOT NULL DEFAULT 0,   -- RN-28
    TotalPagado       DECIMAL(12,2) NOT NULL,
    ReservaVence      DATETIME      NULL,       -- RN-15; NULL = sin cupo reservado
    CONSTRAINT FK_Compras_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID),
    CONSTRAINT CK_Compras_Estado
        CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida'))
);

ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Compras
      FOREIGN KEY (ID_Compra) REFERENCES Compras(ID);

CREATE INDEX IX_Compras_Usuario_Fecha
    ON Compras (ID_Usuario, FechaCompra) INCLUDE (Estado);   -- RN-19

-- RN-30 + RN-31: un intento de pago por fila; un cliente puede reintentar la misma compra.
-- De la tarjeta solo se guardan los últimos 4 dígitos y el token, nunca el número completo.
CREATE TABLE Pagos (
    ID          INT IDENTITY(1,1) PRIMARY KEY,
    ID_Compra   INT          NOT NULL,
    FechaPago   DATETIME     NOT NULL,
    Estado      VARCHAR(10)  NOT NULL,   -- Aprobado|Rechazado
    Ultimos4    CHAR(4)      NOT NULL,
    Token       VARCHAR(64)  NOT NULL,
    Motivo      VARCHAR(80)  NULL,       -- solo en los rechazados
    CONSTRAINT FK_Pagos_Compras FOREIGN KEY (ID_Compra) REFERENCES Compras(ID),
    CONSTRAINT CK_Pagos_Estado CHECK (Estado IN ('Aprobado','Rechazado')),
    CONSTRAINT CK_Pagos_Ultimos4 CHECK (Ultimos4 LIKE '[0-9][0-9][0-9][0-9]')
);
CREATE INDEX IX_Pagos_Compra ON Pagos (ID_Compra);

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

## Cambios de la Sección 7: pago y cierre de la compra

**Tabla `Pagos`.** RN-30 y RN-31 no tenían dónde guardarse. Un cliente puede reintentar el
pago de una misma compra, así que son varios intentos por compra: tabla propia, no columnas
en `Compras`. Solo guarda los últimos 4 dígitos y el token; `CK_Pagos_Ultimos4` impide por
estructura que se cuele otra cosa en esa columna.

**`Compras.MontoSeguro`.** Sin esta columna la fila no cierra: con seguro,
`BaseEntradas − MontoDescuento + MontoServicios + CargoServicio` no da `TotalPagado`.
Ahora se cumple `Base − Descuento + Servicios + Cargo + Seguro = TotalPagado`.

**Estado `Vencida`.** Una reserva que caducó sin pagarse (RN-15) no es «Reembolsada», y
borrar la fila perdería el rastro del intento que pide RN-31.

### Ciclo de vida de una compra

| Paso | `Compras` | `Entradas` | `Sectores.CantidadReservada` |
|---|---|---|---|
| Iniciar (RN-15) | `Pendiente`, `ReservaVence` = ahora + `RESERVA_MINUTOS` | una fila por entrada, `Pendiente`, sin QR | + cantidad |
| Pago rechazado (RN-32) | sigue `Pendiente`, `ReservaVence` = NULL | siguen `Pendiente` | − cantidad |
| Reintento | `ReservaVence` = el plazo original | — | + cantidad, si todavía hay cupo |
| Pago aprobado (RN-17) | `Confirmada` | `Confirmada`, con `CodigoQR` | − cantidad (pasan a contar como vendidas) |
| Plazo vencido | `Vencida` | `Vencida` | − cantidad, solo si la retenía |

- **Una fila de `Entradas` por entrada**, con `CantidadVendida = 1`. RN-18 exige un QR por
  entrada, válido para un único ingreso, y `Entradas.CodigoQR` es uno por fila. Lo mismo vale
  para `Seguros`, que tiene una póliza por `ID_Entrada`.
- **Las entradas nacen `Pendiente` al reservar.** Así la reserva queda en la base: un
  reintento de pago o la liberación de una reserva vencida (en otra sesión, incluso con la
  aplicación cerrada a mitad del pago) saben qué sectores, cuántas entradas y a qué precio
  congelado se reservaron. El cupo disponible no cambia porque solo cuentan las `Confirmada`.
- **`ReservaVence` en NULL** indica que la compra ya no retiene cupo. Evita devolverlo dos
  veces si una compra rechazada vence después. El plazo es fijo desde que se inició la compra:
  un reintento no lo extiende.
- **Liberación perezosa.** No hay proceso en segundo plano: `GestorCompras.LiberarVencidas`
  se corre al abrir la pantalla de compra y al agotarse el temporizador de pago.

## Estados de un evento

`Eventos.Estado` admite tres valores y gobierna la visibilidad del evento:

| Estado | Cuándo | Visible para el cliente | RN-06 aplica |
|---|---|---|---|
| `Borrador` | Al crearse, antes de tener sectores | No | No |
| `Publicado` | Al confirmarse el primer sector con capacidad > 0 | Sí | **Sí** |
| `Suspendido` | Al suspenderlo por RN-11 | No | No |

El estado `Borrador` permite guardar el evento apenas se crea, sin violar RN-06: la
regla exige que un evento **publicado** tenga siempre al menos un sector, y un borrador
todavía no lo está. Así el propietario no pierde lo cargado si interrumpe el alta.

`GestorEventos.Vigentes()` devuelve únicamente eventos en estado `Publicado` con fecha
posterior a la actual (RN-09).

```sql
ALTER TABLE Eventos ADD CONSTRAINT CK_Eventos_Estado
      CHECK (Estado IN ('Borrador','Publicado','Suspendido'));
```

## Por qué `Entradas` necesita `ID_Sector`

RN-08 exige que las entradas vendidas de un sector nunca superen su capacidad. La tabla
`Entradas` vincula la entrada con el **evento**, no con el **sector**, de modo que sin esta
columna la cantidad vendida por sector no se puede calcular y la regla queda sin forma de
verificarse.

Se descartó la alternativa de llevar un contador `CantidadVendida` en `Sectores`: un
contador desnormalizado se desincroniza ante cualquier reembolso o error de transacción.
La suma sobre `Entradas` siempre refleja el estado real.

`Sectores.CantidadReservada` sí es un contador, pero su naturaleza es distinta: representa
un estado transitorio de diez minutos (RN-15) que no tiene filas propias en `Entradas`.

## Consultas clave

### Cupo disponible de un sector (RN-08 + RN-15)

```sql
SELECT  s.Capacidad
      - ISNULL((SELECT SUM(e.CantidadVendida)
                FROM   Entradas e
                WHERE  e.ID_Sector = s.ID
                  AND  e.Estado    = 'Confirmada'), 0)
      - s.CantidadReservada                              AS Disponible
FROM    Sectores s
WHERE   s.ID = @IdSector;
```

El control computa vendidas **más** reservadas vigentes, según el conflicto RN-08 vs RN-15
resuelto en `docs/02-reglas-negocio.md`.

### Conteo de la ventana móvil (RN-19)

```sql
SELECT COUNT(*)
FROM   Compras
WHERE  ID_Usuario  =  @IdUsuario
  AND  Estado      = 'Confirmada'
  AND  FechaCompra >  DATEADD(MONTH, -12, @FechaReferencia)
  AND  FechaCompra <= @FechaReferencia;
```

## Restricciones estructurales que la base debe garantizar

| Regla | Restricción | Por qué en la base y no solo en el código |
|---|---|---|
| RN-01 | `UQ_Usuario_Mail` | Dos registros simultáneos podrían colarse con una validación solo en memoria |
| RN-08 | `FK_Entradas_Sectores` | Sin el vínculo, la regla no se puede evaluar |
| RN-18 | `UX_Entradas_CodigoQR` | Un QR repetido habilitaría dos ingresos con una sola entrada |
| RN-25 | `UX_Suscripcion_Activa` | Índice filtrado: una sola suscripción activa por usuario |
| RN-36 | `UX_ListaEspera` | Una sola inscripción por evento y usuario |

## Obligatoriedad de las claves foráneas

El esquema heredado declara anulables seis columnas cuyos estados nulos no son válidos en
el dominio: un sector sin evento, un evento sin lugar, un local sin dueño o una entrada sin
comprador no representan nada. Se corrigen en la base y no en las entidades: declararlas
anulables en el código obligaría a verificar `null` en las cuatro capas de forma permanente
y además dejaría entrar datos inconsistentes.

```sql
-- Ejecutar antes de las FK, y solo sobre datos ya saneados.
UPDATE Locales  SET PropietarioID = 0 WHERE PropietarioID IS NULL;   -- revisar antes
ALTER TABLE Locales  ALTER COLUMN PropietarioID INT NOT NULL;
ALTER TABLE Eventos  ALTER COLUMN LocalID       INT NOT NULL;
ALTER TABLE Sectores ALTER COLUMN EventoID      INT NOT NULL;
ALTER TABLE Entradas ALTER COLUMN ID_Evento     INT NOT NULL;
ALTER TABLE Entradas ALTER COLUMN ID_Usuario    INT NOT NULL;
ALTER TABLE Entradas ALTER COLUMN FechaVenta    DATETIME NOT NULL;

-- RN-38 compara contra Locales.Capacidad: si es NULL se lee como 0 y el local no admite
-- ningun sector. Un local sin capacidad declarada no es un estado valido del dominio.
ALTER TABLE Locales  ALTER COLUMN Capacidad     INT NOT NULL;
ALTER TABLE Locales  ADD CONSTRAINT CK_Locales_Capacidad CHECK (Capacidad > 0);

-- Faltaba: la entrada debe apuntar a un usuario existente.
ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Usuario
      FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID);
```

Con esto, las propiedades `int` y `DateTime` de las entidades se corresponden con columnas
que nunca devuelven `DBNull`, y la capa `Datos` puede mapear sin verificaciones defensivas.

## Correspondencia obligatoria entre columna y propiedad

Toda clave primaria y toda clave foránea del esquema —incluidas las de las tablas
preexistentes— debe tener su propiedad en la entidad correspondiente. Sin `Usuario.Id` no
hay forma de identificar al usuario autenticado; sin `Sector.Id` e `IdEvento`, la regla
RN-08 no se puede evaluar desde el código aunque la base la soporte.

Convención de nombres: la columna `ID_Usuario` se mapea como `IdUsuario`, `LocalID` como
`IdLocal`, y la clave primaria siempre como `Id`.

## Datos semilla mínimos

- Los 14 registros de `Parametros` de `docs/02-reglas-negocio.md`.
- Un usuario Administrador y tres Clientes, uno por nivel.
- Un local con tres eventos: uno vigente con cupo, uno agotado, uno con fecha pasada.
- Tres sectores por evento con precios distintos.
- Cuatro servicios, uno de cada tipo, habilitados en el evento vigente.
