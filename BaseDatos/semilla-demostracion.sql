/* Demostración. Aplicar después de 02-semilla.sql y las migraciones del esquema,
   en una base separada que termine en _Demostracion. Programación, aforos de
   muestra, personas y operaciones ficticias. Direcciones de sedes de Rosario.

   Qué carga:
   - cinco sedes reales de Rosario (una es el local de la semilla base, renombrado);
   - 20 eventos entre los últimos seis meses y los próximos cuatro, con sectores y aforos
     distintos según la sede (RN-38: la suma de los sectores no pasa la capacidad del local);
   - 337 entradas vendidas, repartidas desparejo: dos eventos agotados (tres con el de la
     semilla base), uno casi agotado, varios a media venta y dos sin ninguna venta;
   - 14 clientes nuevos y los tres de la semilla base, con un historial que da exactamente su
     nivel (RN-19: Bronce 0-2, Plata 3-6, Oro 7 o más compras en 12 meses);
   - importes calculados como GestorFidelizacion.CalcularDescuento: descuento solo sobre las
     entradas (RN-20, RN-21), cargo sobre el subtotal con descuento, servicios sin descuento
     y seguro por entrada (RN-28). El nivel de cada compra es el vigente al hacerla: sube
     después de cada compra confirmada (RN-22);
   - ingresos registrados en los eventos que ya pasaron, una lista de espera en un evento
     agotado y solicitudes de Organizador pendientes, aprobada y rechazada (RN-52).

   Los CUIT, teléfonos y alias bancarios son ficticios a propósito: pasan las validaciones
   del sistema, pero no corresponden a ninguna persona ni cuenta real. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
IF RIGHT(DB_NAME(), LEN('_Demostracion')) <> '_Demostracion'
    THROW 51000, 'Esta semilla se ejecuta solo en una base _Demostracion.', 1;

DECLARE @organizador INT = (SELECT ID FROM Usuario WHERE Mail = 'organizador@eventaura.com');
DECLARE @admin INT = (SELECT ID FROM Usuario WHERE Mail = 'admin@eventaura.com');
DECLARE @bronce INT = (SELECT ID FROM Usuario WHERE Mail = 'bronce@eventaura.com');
DECLARE @plata INT = (SELECT ID FROM Usuario WHERE Mail = 'plata@eventaura.com');
DECLARE @oro INT = (SELECT ID FROM Usuario WHERE Mail = 'oro@eventaura.com');
IF @organizador IS NULL OR @admin IS NULL OR @bronce IS NULL OR @plata IS NULL OR @oro IS NULL
    THROW 51001, 'Aplicar primero la semilla base y las migraciones.', 1;
IF EXISTS (SELECT 1 FROM Usuario WHERE Mail LIKE '%@demo.eventaura.example')
    RETURN;
IF NOT EXISTS (SELECT 1 FROM Locales WHERE PropietarioID = @organizador AND Nombre = 'Sala Demostrativa EventAura')
   OR NOT EXISTS (SELECT 1 FROM SolicitudesOrganizador WHERE UsuarioID = @bronce AND Estado = 'Pendiente')
    THROW 51002, 'Falta el local o la solicitud de la semilla base.', 1;

DECLARE @hoy DATE = CAST(GETDATE() AS DATE);
DECLARE @ahora DATETIME = GETDATE();
DECLARE @mes DATE = DATEFROMPARTS(YEAR(@hoy), MONTH(@hoy), 1);
DECLARE @hash VARCHAR(200) = (SELECT Contraseña FROM Usuario WHERE ID = @bronce);
-- Porcentajes e importes de los mismos parámetros que usa la aplicación.
DECLARE @cargo DECIMAL(8,4) = (SELECT CAST(Valor AS DECIMAL(8,4)) FROM Parametros WHERE Clave = 'CARGO_SERVICIO');
DECLARE @descuentoPlata DECIMAL(8,4) = (SELECT CAST(Valor AS DECIMAL(8,4)) FROM Parametros WHERE Clave = 'DESCUENTO_PLATA');
DECLARE @descuentoOro DECIMAL(8,4) = (SELECT CAST(Valor AS DECIMAL(8,4)) FROM Parametros WHERE Clave = 'DESCUENTO_ORO');
DECLARE @umbralPlata INT = (SELECT CAST(Valor AS INT) FROM Parametros WHERE Clave = 'FIDELIDAD_UMBRAL_PLATA');
DECLARE @umbralOro INT = (SELECT CAST(Valor AS INT) FROM Parametros WHERE Clave = 'FIDELIDAD_UMBRAL_ORO');
DECLARE @ventana INT = (SELECT CAST(Valor AS INT) FROM Parametros WHERE Clave = 'FIDELIDAD_VENTANA_MESES');
DECLARE @prima DECIMAL(12,2) = (SELECT CAST(Valor AS DECIMAL(12,2)) FROM Parametros WHERE Clave = 'SEGURO_PRECIO');
DECLARE @horasSeguro INT = (SELECT CAST(Valor AS INT) FROM Parametros WHERE Clave = 'SEGURO_HORAS_COBERTURA');
IF @cargo IS NULL OR @descuentoPlata IS NULL OR @descuentoOro IS NULL OR @umbralPlata IS NULL
   OR @umbralOro IS NULL OR @ventana IS NULL OR @prima IS NULL OR @horasSeguro IS NULL
    THROW 51003, 'Faltan parámetros de fidelización, cargo o seguro.', 1;

BEGIN TRANSACTION;

-- La semilla base aproxima el historial de estos perfiles con compras sin detalle.
-- En la demostración lo reemplazamos con las compras y entradas de esta agenda.
DELETE c FROM Compras c
WHERE c.ID_Usuario IN (@plata, @oro)
  AND NOT EXISTS (SELECT 1 FROM Entradas e WHERE e.ID_Compra = c.ID)
  AND NOT EXISTS (SELECT 1 FROM CompraServicios s WHERE s.ID_Compra = c.ID)
  AND NOT EXISTS (SELECT 1 FROM Pagos p WHERE p.ID_Compra = c.ID);

-- ═══════════════ Sedes ═══════════════
-- El local de la semilla base pasa a ser el Galpón 17. Sus eventos de la semilla base siguen
-- ahí, y el de mayor aforo («Duki en Rosario») suma 2.900 lugares: la capacidad no puede
-- bajar de eso (RN-38).
UPDATE Locales
SET Nombre = 'Galpón 17', Direccion = 'Estévez Boero 600, Rosario', Capacidad = 3000
WHERE PropietarioID = @organizador AND Nombre = 'Sala Demostrativa EventAura';

INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID)
VALUES ('Galpón 11', 'Estévez Boero 980, Rosario', 420, @organizador),
       ('Teatro Municipal La Comedia', 'Mitre 958, Rosario', 620, @organizador),
       ('Cultural Fontanarrosa', 'San Martín 1080, Rosario', 280, @organizador),
       ('Centro Cultural Parque Alem', 'Nansen 100, Rosario', 180, @organizador);

-- ═══════════════ Clientes ═══════════════
-- Objetivo: compras confirmadas que tendrá cada uno en la ventana de 12 meses. Da su nivel
-- (RN-19). Para los Oro, 0 significa «el mínimo de Oro más lo que sobre del plan de ventas».
DECLARE @personas TABLE (Nombre VARCHAR(40), Apellido VARCHAR(40), Mail VARCHAR(150), Nivel VARCHAR(8), Objetivo INT);
INSERT @personas VALUES
('Malena','Ríos','malena.rios@demo.eventaura.example','Bronce',1),
('Mateo','Luna','mateo.luna@demo.eventaura.example','Bronce',2),
('Lucía','Vega','lucia.vega@demo.eventaura.example','Bronce',1),
('Joaquín','Arias','joaquin.arias@demo.eventaura.example','Bronce',2),
('Agustina','Ferreyra','agustina.ferreyra@demo.eventaura.example','Bronce',2),
('Clara','Molina','clara.molina@demo.eventaura.example','Plata',3),
('Julián','Silva','julian.silva@demo.eventaura.example','Plata',5),
('Emilia','Costa','emilia.costa@demo.eventaura.example','Plata',4),
('Facundo','Godoy','facundo.godoy@demo.eventaura.example','Plata',6),
('Sofía','Acosta','sofia.acosta@demo.eventaura.example','Oro',0),
('Tomás','Ibarra','tomas.ibarra@demo.eventaura.example','Oro',0),
('Valentina','Bernal','valentina.bernal@demo.eventaura.example','Oro',0),
('Martina','Paz','martina.paz@demo.eventaura.example','Oro',0),
('Nicolás','Benítez','nicolas.benitez@demo.eventaura.example','Oro',0);
-- Se dan de alta en Bronce: el nivel final sale del historial, al terminar.
INSERT Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
SELECT Nombre, Apellido, Mail, @hash, 'Cliente', 'Bronce', @mes FROM @personas;
INSERT Cliente (ID)
SELECT u.ID FROM Usuario u JOIN @personas p ON p.Mail = u.Mail;

DECLARE @clientes TABLE (ID INT PRIMARY KEY, Nivel VARCHAR(8), Objetivo INT);
INSERT @clientes (ID, Nivel, Objetivo)
SELECT u.ID, p.Nivel, p.Objetivo FROM Usuario u JOIN @personas p ON p.Mail = u.Mail;
-- Los de la semilla base. El cliente Oro ya tiene una compra con entradas (la del evento
-- agotado de la semilla base); con las de acá queda en Oro igual.
INSERT @clientes (ID, Nivel, Objetivo) VALUES (@bronce, 'Bronce', 2), (@plata, 'Plata', 4), (@oro, 'Oro', 0);

-- ═══════════════ Agenda ═══════════════
DECLARE @agenda TABLE (Pos INT PRIMARY KEY, Nombre VARCHAR(100), Dias INT, Hora INT,
                       Categoria VARCHAR(20), Sede VARCHAR(100), Evento INT NULL);
INSERT @agenda (Pos, Nombre, Dias, Hora, Categoria, Sede) VALUES
( 1,'Orquesta Típica del Litoral',      -168,21,'Concierto','Teatro Municipal La Comedia'),
( 2,'Copa Costanera de básquet 3x3',    -152,17,'Deportivo','Centro Cultural Parque Alem'),
( 3,'Feria gráfica del Litoral',        -137,18,'Cultural', 'Cultural Fontanarrosa'),
( 4,'Fiesta de los Galpones',           -121,23,'Concierto','Galpón 17'),
( 5,'La casa del río',                  -104,21,'Teatro',   'Teatro Municipal La Comedia'),
( 6,'Noches de jazz en el puerto',       -90,21,'Concierto','Galpón 11'),
( 7,'La orilla invisible',               -76,21,'Teatro',   'Teatro Municipal La Comedia'),
( 8,'Mapas de la memoria',               -61,19,'Cultural', 'Cultural Fontanarrosa'),
( 9,'Torneo de vóley de la costanera',   -47,16,'Deportivo','Centro Cultural Parque Alem'),
(10,'Milonga de los andenes',            -30,22,'Concierto','Galpón 11'),
(11,'Bajo la misma luna',                -12,21,'Teatro',   'Teatro Municipal La Comedia'),
(12,'Taller de fotografía urbana',         8,18,'Cultural', 'Cultural Fontanarrosa'),
(13,'Guitarras del litoral',              17,21,'Concierto','Galpón 11'),
(14,'Fiesta de la cumbia santafesina',    23,23,'Concierto','Galpón 17'),
(15,'Copa de verano 3x3',                 30,17,'Deportivo','Centro Cultural Parque Alem'),
(16,'Una noche en el puerto',             38,21,'Teatro',   'Teatro Municipal La Comedia'),
(17,'Festival del río',                   52,20,'Concierto','Galpón 17'),
(18,'Laboratorio de historietas',         67,18,'Cultural', 'Cultural Fontanarrosa'),
(19,'Encuentro de vóley mixto',           88,16,'Deportivo','Centro Cultural Parque Alem'),
(20,'Fiesta de fin de temporada',        110,23,'Concierto','Galpón 17');

-- Sectores por sede: el teatro vende platea, palcos y tertulia; los galpones, campo y un
-- sector preferencial o mesas; los centros culturales, general. Vendidas = entradas que se
-- cargan. Agotados: 3 y 13 (todos sus sectores), más «Clásico Rosarino» de la semilla base.
-- Casi agotado: 12. Sin ventas: 7 y 18 (y dos eventos de la semilla base).
DECLARE @sectores TABLE (Pos INT, Orden INT, Nombre VARCHAR(50), Precio DECIMAL(12,2),
                         Capacidad INT, Vendidas INT, Sector INT NULL, PRIMARY KEY (Pos, Orden));
INSERT @sectores (Pos, Orden, Nombre, Precio, Capacidad, Vendidas) VALUES
( 1,1,'Platea',      22000, 40,16),( 1,2,'Palcos',      28000, 12, 6),( 1,3,'Tertulia',    15000, 20, 3),
( 2,1,'General',      9000, 60,18),
( 3,1,'General',      8500, 24,24),
( 4,1,'Campo',       18000,120,26),( 4,2,'VIP',         32000, 20, 6),
( 5,1,'Platea',      16000, 36,12),( 5,2,'Palcos',      21000, 10, 2),
( 6,1,'General',     18500, 40,14),( 6,2,'Mesas',       26000, 16, 8),
( 7,1,'Platea',      18000, 30, 0),( 7,2,'Palcos',      23000,  8, 0),
( 8,1,'General',     11500, 45,15),
( 9,1,'General',      9000, 50,22),( 9,2,'Preferencial',13000, 10, 6),
(10,1,'General',     13000, 50,12),(10,2,'Mesas',       19000, 12, 5),
(11,1,'Platea',      16500, 40,15),(11,2,'Palcos',      22000, 10, 4),(11,3,'Tertulia',    11000, 16, 2),
(12,1,'General',     12500, 12,10),
(13,1,'General',     23500, 24,24),(13,2,'Mesas',       31000,  8, 8),
(14,1,'Campo',       15000,150,22),(14,2,'VIP',         28000, 24, 4),
(15,1,'General',     10500, 40,14),
(16,1,'Platea',      19000, 44,10),(16,2,'Palcos',      25000, 10, 3),(16,3,'Tertulia',    13000, 18, 0),
(17,1,'Campo',       28500, 90,12),(17,2,'Platea',      36500, 20, 3),(17,3,'Preferencial',44500, 15, 2),
(18,1,'General',     10500, 24, 0),
(19,1,'General',      9500, 40, 6),
(20,1,'Campo',       20000,100, 3),(20,2,'VIP',         34000, 20, 0);

-- Servicios que ofrece cada evento (RN-14), por tipo de servicio de la semilla base.
DECLARE @ofertas TABLE (Pos INT, Tipo VARCHAR(20));
INSERT @ofertas VALUES
(4,'Merchandising'),(4,'Traslado'),
(6,'Merchandising'),(10,'Merchandising'),(13,'Merchandising'),
(14,'Merchandising'),(14,'Traslado'),
(17,'Merchandising'),(17,'Traslado'),(17,'Hospedaje'),(17,'VIP'),
(20,'Merchandising'),(20,'Traslado'),
(2,'Merchandising'),(9,'Merchandising'),(15,'Merchandising'),
(1,'Traslado'),(11,'Traslado'),(16,'Traslado');

DECLARE @pos INT = 1, @evento INT;
WHILE @pos <= 20
BEGIN
    INSERT Eventos (Nombre, Fecha, LocalID, Categoria, Estado)
    SELECT a.Nombre, DATEADD(HOUR, a.Hora, CONVERT(DATETIME, DATEADD(DAY, a.Dias, @hoy))), l.ID, a.Categoria, 'Publicado'
    FROM @agenda a JOIN Locales l ON l.Nombre = a.Sede AND l.PropietarioID = @organizador
    WHERE a.Pos = @pos;
    SET @evento = SCOPE_IDENTITY();
    UPDATE @agenda SET Evento = @evento WHERE Pos = @pos;
    INSERT Sectores (EventoID, Nombre, Precio, Capacidad)
    SELECT @evento, Nombre, Precio, Capacidad FROM @sectores WHERE Pos = @pos ORDER BY Orden;
    INSERT ServiciosEvento (ID_Evento, ID_Servicio)
    SELECT @evento, MIN(s.ID) FROM @ofertas o JOIN Servicios s ON s.Tipo = o.Tipo AND s.Activo = 1
    WHERE o.Pos = @pos GROUP BY o.Tipo;
    SET @pos += 1;
END;
UPDATE s SET Sector = x.ID
FROM @sectores s JOIN @agenda a ON a.Pos = s.Pos JOIN Sectores x ON x.EventoID = a.Evento AND x.Nombre = s.Nombre;

IF EXISTS (SELECT 1 FROM @agenda WHERE Evento IS NULL) OR EXISTS (SELECT 1 FROM @sectores WHERE Sector IS NULL)
    THROW 51004, 'No se pudo ubicar una sede o un sector de la agenda.', 1;
IF EXISTS (SELECT 1 FROM Sectores x JOIN Eventos e ON e.ID = x.EventoID JOIN Locales l ON l.ID = e.LocalID
           GROUP BY e.ID, l.Capacidad HAVING SUM(x.Capacidad) > l.Capacidad)
    THROW 51005, 'Un evento supera la capacidad de su sede (RN-38).', 1;

-- ═══════════════ Plan de ventas ═══════════════
-- Cada sector se vende en compras de 1 a 6 entradas. Las compras van hacia atrás desde la
-- víspera del evento (o desde hoy, si el evento es futuro), separadas por día y medio.
DECLARE @plan TABLE (ID INT IDENTITY PRIMARY KEY, Pos INT, Sector INT, Precio DECIMAL(12,2),
                     Cantidad INT, Venta DATETIME, Comprador INT NULL);
DECLARE @tandas TABLE (n INT PRIMARY KEY, Cantidad INT);
INSERT @tandas VALUES (0,2),(1,4),(2,1),(3,3),(4,2),(5,5),(6,6);
DECLARE @sPos INT, @sOrden INT, @sSector INT, @sPrecio DECIMAL(12,2), @restan INT, @k INT,
        @q INT, @fin DATETIME, @fechaEvento DATETIME;
DECLARE sectoresCur CURSOR LOCAL FAST_FORWARD FOR
    SELECT s.Pos, s.Orden, s.Sector, s.Precio, s.Vendidas, e.Fecha
    FROM @sectores s JOIN @agenda a ON a.Pos = s.Pos JOIN Eventos e ON e.ID = a.Evento
    WHERE s.Vendidas > 0 ORDER BY s.Pos, s.Orden;
OPEN sectoresCur;
FETCH NEXT FROM sectoresCur INTO @sPos, @sOrden, @sSector, @sPrecio, @restan, @fechaEvento;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @fin = IIF(DATEADD(DAY, -1, @fechaEvento) < @ahora, DATEADD(DAY, -1, @fechaEvento), DATEADD(HOUR, -2, @ahora));
    SET @k = 0;
    WHILE @restan > 0
    BEGIN
        SET @q = (SELECT Cantidad FROM @tandas WHERE n = (@sPos + @sOrden + @k) % 7);
        IF @q > @restan SET @q = @restan;
        INSERT @plan (Pos, Sector, Precio, Cantidad, Venta)
        VALUES (@sPos, @sSector, @sPrecio, @q, DATEADD(MINUTE, -(2160 * @k + 47 * @sOrden + 13 * @sPos), @fin));
        SET @restan -= @q;
        SET @k += 1;
    END;
    FETCH NEXT FROM sectoresCur INTO @sPos, @sOrden, @sSector, @sPrecio, @restan, @fechaEvento;
END;
CLOSE sectoresCur; DEALLOCATE sectoresCur;

-- Los Oro se reparten lo que sobra después de los objetivos de Bronce y Plata.
DECLARE @compras INT = (SELECT COUNT(*) FROM @plan);
DECLARE @fijas INT = (SELECT SUM(Objetivo) FROM @clientes WHERE Nivel <> 'Oro');
DECLARE @cantOro INT = (SELECT COUNT(*) FROM @clientes WHERE Nivel = 'Oro');
DECLARE @sobran INT = @compras - @fijas - @cantOro * @umbralOro;
IF @sobran < 0
    THROW 51006, 'El plan de ventas no alcanza para el historial de los clientes Oro.', 1;
WITH oros AS (SELECT ID, ROW_NUMBER() OVER (ORDER BY ID) AS n FROM @clientes WHERE Nivel = 'Oro')
UPDATE c SET Objetivo = @umbralOro + @sobran / @cantOro + IIF(o.n <= @sobran % @cantOro, 1, 0)
FROM @clientes c JOIN oros o ON o.ID = c.ID;

-- Cada cliente aparece tantas veces como su objetivo, repartido a lo largo del semestre; la
-- compra n-ésima en el tiempo va al cliente del lugar n-ésimo.
WITH numeros AS (SELECT TOP (60) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS k FROM sys.all_objects),
lugares AS (
    SELECT c.ID, ROW_NUMBER() OVER (ORDER BY (n.k - 0.5) / c.Objetivo, c.ID) AS orden
    FROM @clientes c JOIN numeros n ON n.k <= c.Objetivo),
ventas AS (SELECT ID, ROW_NUMBER() OVER (ORDER BY Venta, ID) AS orden FROM @plan)
UPDATE p SET Comprador = l.ID
FROM @plan p JOIN ventas v ON v.ID = p.ID JOIN lugares l ON l.orden = v.orden;
IF EXISTS (SELECT 1 FROM @plan WHERE Comprador IS NULL)
    THROW 51007, 'Quedó una compra sin comprador.', 1;

-- ═══════════════ Compras, pagos, entradas, servicios y seguros ═══════════════
-- En orden cronológico: el nivel de cada compra es el que el cliente tenía al hacerla
-- (compras confirmadas previas en la ventana; RN-19 y RN-22).
DECLARE @pID INT, @pPos INT, @pSector INT, @pPrecio DECIMAL(12,2), @pCant INT, @pVenta DATETIME,
        @cliente INT, @previas INT, @nivel VARCHAR(8), @tasa DECIMAL(8,4), @base DECIMAL(12,2),
        @descuento DECIMAL(12,2), @subtotal DECIMAL(12,2), @comision DECIMAL(12,2),
        @servicio INT, @precioServicio DECIMAL(12,2), @conSeguro BIT, @seguro DECIMAL(12,2),
        @compra INT, @eventoCompra INT, @tarjeta CHAR(4);
DECLARE comprasCur CURSOR LOCAL FAST_FORWARD FOR
    SELECT p.ID, p.Pos, p.Sector, p.Precio, p.Cantidad, p.Venta, p.Comprador, a.Evento, e.Fecha
    FROM @plan p JOIN @agenda a ON a.Pos = p.Pos JOIN Eventos e ON e.ID = a.Evento
    ORDER BY p.Venta, p.ID;
OPEN comprasCur;
FETCH NEXT FROM comprasCur INTO @pID, @pPos, @pSector, @pPrecio, @pCant, @pVenta, @cliente, @eventoCompra, @fechaEvento;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @previas = (SELECT COUNT(*) FROM Compras WHERE ID_Usuario = @cliente AND Estado = 'Confirmada'
                    AND FechaCompra >= DATEADD(MONTH, -@ventana, @pVenta) AND FechaCompra < @pVenta);
    SET @nivel = CASE WHEN @previas >= @umbralOro THEN 'Oro' WHEN @previas >= @umbralPlata THEN 'Plata' ELSE 'Bronce' END;
    SET @tasa = CASE @nivel WHEN 'Plata' THEN @descuentoPlata WHEN 'Oro' THEN @descuentoOro ELSE 0 END;
    SET @base = @pPrecio * @pCant;
    SET @descuento = ROUND(@base * @tasa, 2);
    SET @subtotal = @base - @descuento;
    SET @comision = ROUND(@subtotal * @cargo, 2);
    -- Una de cada tres compras de un evento con servicios suma uno de ellos, rotando.
    SET @servicio = NULL; SET @precioServicio = 0;
    IF @pID % 3 = 0
        SELECT @servicio = s.ID, @precioServicio = s.Precio
        FROM (SELECT x.ID, x.Precio, ROW_NUMBER() OVER (ORDER BY x.ID) - 1 AS n,
                     COUNT(*) OVER () AS total
              FROM ServiciosEvento se JOIN Servicios x ON x.ID = se.ID_Servicio
              WHERE se.ID_Evento = @eventoCompra) s
        WHERE s.n = (@pID / 3) % s.total;
    -- Una de cada cuatro compras contrata el seguro. Ningún cliente de esta semilla tiene
    -- suscripción, así que paga la prima entera (RN-37 no aplica).
    SET @conSeguro = IIF(@pID % 4 = 1, 1, 0);
    SET @seguro = IIF(@conSeguro = 1, @prima * @pCant, 0);

    INSERT Compras (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado,
                    BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, MontoSeguro, ConSeguro, TotalPagado)
    VALUES (@cliente, @pVenta, 'Confirmada', @nivel, @tasa, @base, @descuento, @precioServicio,
            @comision, @seguro, @conSeguro, @subtotal + @precioServicio + @comision + @seguro);
    SET @compra = SCOPE_IDENTITY();

    SET @tarjeta = CHOOSE(@pID % 5 + 1, '4242', '1881', '0457', '7310', '5096');
    -- Algunas compras se confirman al segundo intento (RN-31).
    IF @pID % 9 = 4
        INSERT Pagos (ID_Compra, FechaPago, Estado, Ultimos4, Token, Motivo)
        VALUES (@compra, DATEADD(MINUTE, -3, @pVenta), 'Rechazado', @tarjeta,
                'demo-' + CONVERT(VARCHAR(36), NEWID()), 'Fondos insuficientes');
    INSERT Pagos (ID_Compra, FechaPago, Estado, Ultimos4, Token)
    VALUES (@compra, @pVenta, 'Aprobado', @tarjeta, 'demo-' + CONVERT(VARCHAR(36), NEWID()));

    IF @servicio IS NOT NULL
        INSERT CompraServicios (ID_Compra, ID_Servicio, Cantidad, PrecioUnit)
        VALUES (@compra, @servicio, 1, @precioServicio);

    INSERT Entradas (ID_Evento, ID_Sector, ID_Usuario, Precio, CantidadVendida, FechaVenta, ID_Compra, Estado, CodigoQR)
    SELECT @eventoCompra, @pSector, @cliente, @pPrecio, 1, @pVenta, @compra, 'Confirmada',
           'EA-DEMO-' + CONVERT(VARCHAR(36), NEWID())
    FROM (VALUES (1),(2),(3),(4),(5),(6)) AS numeros(n) WHERE n <= @pCant;

    IF @conSeguro = 1
        INSERT Seguros (ID_Entrada, Precio, VenceEl)
        SELECT ID, @prima, DATEADD(HOUR, -@horasSeguro, @fechaEvento) FROM Entradas WHERE ID_Compra = @compra;

    FETCH NEXT FROM comprasCur INTO @pID, @pPos, @pSector, @pPrecio, @pCant, @pVenta, @cliente, @eventoCompra, @fechaEvento;
END;
CLOSE comprasCur; DEALLOCATE comprasCur;

-- La compra con entradas de la semilla base (cliente Oro, evento agotado) quedó grabada como
-- Plata sin descuento. Con el historial de esta semilla se recalcula con el nivel que el
-- cliente tenía en esa fecha, igual que las demás. No tiene servicios ni seguro.
UPDATE c SET NivelAplicado = n.Nivel, DescuentoAplicado = n.Tasa,
             MontoDescuento = ROUND(c.BaseEntradas * n.Tasa, 2),
             CargoServicio = ROUND((c.BaseEntradas - ROUND(c.BaseEntradas * n.Tasa, 2)) * @cargo, 2),
             TotalPagado = c.BaseEntradas - ROUND(c.BaseEntradas * n.Tasa, 2) + c.MontoServicios
                           + ROUND((c.BaseEntradas - ROUND(c.BaseEntradas * n.Tasa, 2)) * @cargo, 2) + c.MontoSeguro
FROM Compras c
CROSS APPLY (SELECT COUNT(*) AS previas FROM Compras p WHERE p.ID_Usuario = c.ID_Usuario AND p.Estado = 'Confirmada'
             AND p.FechaCompra >= DATEADD(MONTH, -@ventana, c.FechaCompra) AND p.FechaCompra < c.FechaCompra) x
CROSS APPLY (SELECT CASE WHEN x.previas >= @umbralOro THEN 'Oro' WHEN x.previas >= @umbralPlata THEN 'Plata' ELSE 'Bronce' END AS Nivel) v
CROSS APPLY (SELECT v.Nivel, CASE v.Nivel WHEN 'Plata' THEN @descuentoPlata WHEN 'Oro' THEN @descuentoOro ELSE 0 END AS Tasa) n
WHERE c.ID_Usuario = @oro AND c.MontoReembolsado = 0
  AND EXISTS (SELECT 1 FROM Entradas e WHERE e.ID_Compra = c.ID AND e.CodigoQR LIKE 'EA-SEED-%');

-- Nivel final: el que da el historial en la ventana (RN-19). Solo sube respecto del de la
-- evaluación del mes, así que coincide con lo que dejaría RN-22 compra a compra.
UPDATE u SET Nivel = CASE WHEN h.n >= @umbralOro THEN 'Oro' WHEN h.n >= @umbralPlata THEN 'Plata' ELSE 'Bronce' END,
             FechaUltimaEvaluacion = @mes
FROM Usuario u JOIN @clientes c ON c.ID = u.ID
CROSS APPLY (SELECT COUNT(*) AS n FROM Compras x WHERE x.ID_Usuario = u.ID AND x.Estado = 'Confirmada'
             AND x.FechaCompra >= DATEADD(MONTH, -@ventana, @ahora)) h;
IF EXISTS (SELECT 1 FROM Usuario u JOIN @clientes c ON c.ID = u.ID WHERE u.Nivel <> c.Nivel)
    THROW 51008, 'El historial no da el nivel previsto para algún cliente.', 1;

-- Ingreso registrado en los eventos que ya pasaron: nueve de cada diez entradas (RN-18).
UPDATE en SET FechaIngreso = DATEADD(MINUTE, -45 + en.ID % 60, e.Fecha)
FROM Entradas en JOIN Eventos e ON e.ID = en.ID_Evento JOIN @agenda a ON a.Evento = e.ID
WHERE e.Fecha < @ahora AND en.ID % 10 <> 0;

-- ═══════════════ Lista de espera ═══════════════
-- «Guitarras del litoral» está agotado. Se anotan tres clientes que no compraron para ese
-- evento, después de la última venta. Ninguno tiene suscripción (RN-35).
DECLARE @agotado INT = (SELECT Evento FROM @agenda WHERE Pos = 13);
DECLARE @ultimaVenta DATETIME = (SELECT MAX(FechaVenta) FROM Entradas WHERE ID_Evento = @agotado);
INSERT ListaEspera (ID_Evento, ID_Usuario, FechaAlta, ConSuscripcion)
SELECT TOP (3) @agotado, c.ID,
       DATEADD(MINUTE, 90 * ROW_NUMBER() OVER (ORDER BY c.Nivel DESC, c.ID), @ultimaVenta), 0
FROM @clientes c
WHERE c.ID NOT IN (SELECT ID_Usuario FROM Entradas WHERE ID_Evento = @agotado)
ORDER BY c.Nivel DESC, c.ID;

-- ═══════════════ Solicitudes para ser Organizador ═══════════════
-- CUIT con dígito verificador válido y alias bancario con el formato de RN-52. Teléfonos de
-- Rosario con una característica sin asignar (341 000).
DECLARE @doc VARBINARY(MAX) = (SELECT TOP (1) Documentacion FROM SolicitudesOrganizador WHERE UsuarioID = @bronce);
UPDATE SolicitudesOrganizador
SET Responsable = N'Bruno Bronce', CUIT = '20000000044', Telefono = '+5493410000101',
    NombreLocal = N'Espacio Ribera', Direccion = N'Bv. Oroño 1450, Rosario', Capacidad = 150,
    NombreArchivo = N'habilitacion-espacio-ribera.png', CBUAlias = 'espacio.ribera.demo',
    CreadaEl = DATEADD(DAY, -2, SYSUTCDATETIME())
WHERE UsuarioID = @bronce AND Estado = 'Pendiente';
INSERT SolicitudesOrganizador (UsuarioID, Responsable, CUIT, Telefono, TelefonoVerificado,
        NombreLocal, Direccion, Capacidad, Documentacion, NombreArchivo, CBUAlias, Estado,
        MotivoRevision, RevisadaPor, RevisadaEl, CreadaEl)
VALUES ((SELECT ID FROM Usuario WHERE Mail = 'malena.rios@demo.eventaura.example'), N'Malena Ríos',
        '27000000022', '+5493410000102', 1, N'Patio de la Siesta', N'Córdoba 3150, Rosario', 120,
        @doc, N'habilitacion-patio-de-la-siesta.png', 'patio.siesta.demo', 'Pendiente',
        NULL, NULL, NULL, DATEADD(HOUR, -20, SYSUTCDATETIME())),
       ((SELECT ID FROM Usuario WHERE Mail = 'mateo.luna@demo.eventaura.example'), N'Mateo Luna',
        '20000000095', '+5493410000103', 1, N'Sótano Luna', N'Entre Ríos 1720, Rosario', 90,
        @doc, N'habilitacion-sotano-luna.png', 'sotano.luna.demo', 'Rechazada',
        N'La habilitación adjunta está vencida. Puede presentar una nueva solicitud con la vigente.',
        @admin, DATEADD(DAY, -9, SYSUTCDATETIME()), DATEADD(DAY, -12, SYSUTCDATETIME())),
       (@organizador, N'Organizador Demostración', '30000000031', '+5493410000104', 1,
        N'Galpón 17', N'Estévez Boero 600, Rosario', 3000, @doc, N'habilitacion-galpon-17.png',
        'galpones.puerto.demo', 'Aprobada', N'Documentación y datos fiscales verificados.',
        @admin, DATEADD(DAY, -185, SYSUTCDATETIME()), DATEADD(DAY, -188, SYSUTCDATETIME()));

COMMIT TRANSACTION;
