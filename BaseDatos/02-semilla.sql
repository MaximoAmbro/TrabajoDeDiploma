/* ═══════════════════════════════════════════════════════════════════════════
   EventAura — 02-semilla.sql
   Datos semilla mínimos (docs/03-esquema-datos.md). Ejecutar DESPUÉS de
   01-esquema.sql, sobre una base sin datos.

   Contraseñas en texto plano a propósito: la sección 3 las migra a PBKDF2 en el
   primer inicio de sesión exitoso. Todas cumplen RN-02.
   ═══════════════════════════════════════════════════════════════════════════ */

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;   -- requerido para escribir en tablas con índice filtrado
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ═══════════════ Parámetros (docs/02-reglas-negocio.md) ═══════════════
INSERT INTO Parametros (Clave, Valor, Descripcion, Regla) VALUES
('PWD_LONGITUD_MIN',        '8',     'Longitud mínima de la contraseña',                          'RN-02'),
('LOGIN_INTENTOS_MAX',      '5',     'Intentos fallidos consecutivos antes del bloqueo',          'RN-04'),
('LOGIN_BLOQUEO_MIN',       '15',    'Minutos de bloqueo de la cuenta',                           'RN-04'),
('COMPRA_ENTRADAS_MAX',     '6',     'Máximo de entradas por operación y por evento',             'RN-12'),
('RESERVA_MINUTOS',         '10',    'Minutos de reserva del cupo al iniciar el pago',            'RN-15'),
('FIDELIDAD_VENTANA_MESES', '12',    'Meses de la ventana móvil de fidelización',                 'RN-19'),
('FIDELIDAD_UMBRAL_PLATA',  '3',     'Compras confirmadas mínimas para nivel Plata',              'RN-19'),
('FIDELIDAD_UMBRAL_ORO',    '7',     'Compras confirmadas mínimas para nivel Oro',                'RN-19'),
('DESCUENTO_PLATA',         '0.050', 'Descuento sobre entradas para nivel Plata',                 'RN-20'),
('DESCUENTO_ORO',           '0.075', 'Descuento sobre entradas para nivel Oro',                   'RN-20'),
('DESCUENTO_MAX_ADMISIBLE', '0.086', 'Tope de descuento admisible',                               'RN-20'),
('CARGO_SERVICIO',          '0.150', 'Cargo por servicio sobre el subtotal de entradas',          NULL),
('SEGURO_HORAS_COBERTURA',  '48',    'Horas antes del evento hasta las que el seguro reembolsa',  'RN-29');

-- ═══════════════ Usuarios: un Administrador y tres Clientes ═══════════════
-- El perfil Administrador se guarda como 'Propietario', que es el valor que usa el login actual.
DECLARE @Admin INT, @Bronce INT, @Plata INT, @Oro INT;
DECLARE @InicioMes DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);   -- RN-23

INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
VALUES ('Ana', 'Administradora', 'admin@eventaura.com', 'Admin1234', 'Propietario', 'Bronce', @InicioMes);
SET @Admin = SCOPE_IDENTITY();
INSERT INTO Propietario (ID) VALUES (@Admin);

INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
VALUES ('Bruno', 'Bronce', 'bronce@eventaura.com', 'Cliente123', 'Cliente', 'Bronce', @InicioMes);
SET @Bronce = SCOPE_IDENTITY();

INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
VALUES ('Paula', 'Plata', 'plata@eventaura.com', 'Cliente123', 'Cliente', 'Plata', @InicioMes);
SET @Plata = SCOPE_IDENTITY();

INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
VALUES ('Oscar', 'Oro', 'oro@eventaura.com', 'Cliente123', 'Cliente', 'Oro', @InicioMes);
SET @Oro = SCOPE_IDENTITY();

INSERT INTO Cliente (ID) VALUES (@Bronce), (@Plata), (@Oro);

-- ═══════════════ Local y eventos ═══════════════
DECLARE @Local INT, @Vigente INT, @Agotado INT, @Pasado INT;

INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID)
VALUES ('Estadio Rosario Arena', 'Av. Pellegrini 1500, Rosario', 5000, @Admin);
SET @Local = SCOPE_IDENTITY();

INSERT INTO Eventos (Nombre, Fecha, LocalID, Categoria, Estado)
VALUES ('Festival de Rock Rosario', DATEADD(DAY, 60, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) + CAST('21:00' AS DATETIME),
        @Local, 'Concierto', 'Publicado');
SET @Vigente = SCOPE_IDENTITY();

INSERT INTO Eventos (Nombre, Fecha, LocalID, Categoria, Estado)
VALUES ('Clásico Rosarino', DATEADD(DAY, 30, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) + CAST('17:00' AS DATETIME),
        @Local, 'Deportivo', 'Publicado');
SET @Agotado = SCOPE_IDENTITY();

INSERT INTO Eventos (Nombre, Fecha, LocalID, Categoria, Estado)
VALUES ('Noche de Teatro Independiente', DATEADD(DAY, -30, CAST(CAST(GETDATE() AS DATE) AS DATETIME)) + CAST('20:30' AS DATETIME),
        @Local, 'Teatro', 'Publicado');
SET @Pasado = SCOPE_IDENTITY();

-- ═══════════════ Tres sectores por evento, precios distintos ═══════════════
INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) VALUES
(@Vigente, 'Campo',     25000.00, 2000),
(@Vigente, 'Platea',    40000.00,  800),
(@Vigente, 'VIP',       60000.00,  100),
(@Pasado,  'General',   15000.00,  300),
(@Pasado,  'Preferente',22000.00,  150),
(@Pasado,  'Palco',     30000.00,   20);

-- Los sectores del evento agotado se insertan uno por uno: sus ID se usan en Entradas (RN-08).
DECLARE @AgPopular INT, @AgPlatea INT, @AgPalco INT;

INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) VALUES (@Agotado, 'Popular', 20000.00, 2);
SET @AgPopular = SCOPE_IDENTITY();
INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) VALUES (@Agotado, 'Platea',  35000.00, 2);
SET @AgPlatea = SCOPE_IDENTITY();
INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) VALUES (@Agotado, 'Palco',   55000.00, 2);
SET @AgPalco = SCOPE_IDENTITY();

-- ═══════════════ Servicios: uno por tipo, habilitados en el evento vigente ═══════════════
DECLARE @Hospedaje INT, @Traslado INT, @Merch INT, @VIP INT;

INSERT INTO Servicios (Nombre, Tipo, Precio) VALUES ('Hotel Ríos 1 noche', 'Hospedaje', 45000.00);
SET @Hospedaje = SCOPE_IDENTITY();
INSERT INTO Servicios (Nombre, Tipo, Precio) VALUES ('Traslado ida y vuelta', 'Traslado', 12000.00);
SET @Traslado = SCOPE_IDENTITY();
INSERT INTO Servicios (Nombre, Tipo, Precio) VALUES ('Remera oficial', 'Merchandising', 18000.00);
SET @Merch = SCOPE_IDENTITY();
INSERT INTO Servicios (Nombre, Tipo, Precio) VALUES ('Meet & Greet', 'VIP', 80000.00);
SET @VIP = SCOPE_IDENTITY();

INSERT INTO ServiciosEvento (ID_Evento, ID_Servicio) VALUES
(@Vigente, @Hospedaje), (@Vigente, @Traslado), (@Vigente, @Merch), (@Vigente, @VIP);

-- ═══════════════ Historial de compras coherente con el nivel (RN-19) ═══════════════
-- Plata: 3 compras confirmadas en los últimos 12 meses. Oro: 7.
-- Importe tipo: base 50.000, sin descuento, cargo 15 % = 7.500, total 57.500.
DECLARE @i INT = 1;
WHILE @i <= 3
BEGIN
    INSERT INTO Compras (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado,
                         BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, TotalPagado)
    VALUES (@Plata, DATEADD(MONTH, -@i * 2, GETDATE()), 'Confirmada', 'Bronce', 0,
            50000.00, 0, 0, 7500.00, 57500.00);
    SET @i += 1;
END;

SET @i = 2;
WHILE @i <= 7
BEGIN
    INSERT INTO Compras (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado,
                         BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, TotalPagado)
    VALUES (@Oro, DATEADD(MONTH, -@i, GETDATE()), 'Confirmada', 'Bronce', 0,
            50000.00, 0, 0, 7500.00, 57500.00);
    SET @i += 1;
END;

-- 7.ª compra del cliente Oro: agota el evento @Agotado (capacidad total 6).
-- base = 2×20.000 + 2×35.000 + 2×55.000 = 220.000; cargo 15 % = 33.000; total 253.000.
DECLARE @CompraAgotado INT;
INSERT INTO Compras (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado,
                     BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, TotalPagado)
VALUES (@Oro, DATEADD(DAY, -15, GETDATE()), 'Confirmada', 'Plata', 0,
        220000.00, 0, 0, 33000.00, 253000.00);
SET @CompraAgotado = SCOPE_IDENTITY();

-- Precio = importe de la fila (precio del sector × cantidad).
INSERT INTO Entradas (ID_Evento, ID_Sector, ID_Usuario, Precio, CantidadVendida, FechaVenta, ID_Compra, Estado, CodigoQR) VALUES
(@Agotado, @AgPopular, @Oro,  40000, 2, DATEADD(DAY, -15, GETDATE()), @CompraAgotado, 'Confirmada', 'EA-SEED-' + CONVERT(VARCHAR(36), NEWID())),
(@Agotado, @AgPlatea,  @Oro,  70000, 2, DATEADD(DAY, -15, GETDATE()), @CompraAgotado, 'Confirmada', 'EA-SEED-' + CONVERT(VARCHAR(36), NEWID())),
(@Agotado, @AgPalco,   @Oro, 110000, 2, DATEADD(DAY, -15, GETDATE()), @CompraAgotado, 'Confirmada', 'EA-SEED-' + CONVERT(VARCHAR(36), NEWID()));

COMMIT TRANSACTION;
GO
