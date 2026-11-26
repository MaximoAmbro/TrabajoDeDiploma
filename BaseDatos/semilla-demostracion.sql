/* Demostración. Aplicar después de 02-semilla.sql y las migraciones del esquema,
   en una base separada que termine en _Demostracion. Programación, aforos de
   muestra, personas y operaciones ficticias. Direcciones de sedes de Rosario.

   Qué carga (reescrita el 26/11 a pedido de JC):
   - siete organizadores (el de la semilla base y seis nuevos), con nueve sedes en cinco
     ciudades: Rosario, Santa Fe, Paraná, Córdoba y Buenos Aires. Una es el local de la
     semilla base, renombrado;
   - 34 eventos: 11 que ya pasaron, para el historial de los clientes, y 23 futuros. Con los
     dos futuros de la semilla base, el catálogo muestra **25 eventos futuros**;
   - sectores con el precio bajando a medida que se alejan del escenario (VIP o mesas, campo
     delantero, campo; platea baja, palcos, platea alta, tertulia o paraíso). Se insertan en
     ese orden. RN-38: la suma de los sectores no pasa la capacidad de la sede;
   - ventas repartidas desparejo: **un solo evento agotado**, el «Clásico Rosarino» de la
     semilla base; tres con pocas entradas («Muestra de cine rosarino», «Taller de fotografía
     urbana» y «Hamlet en la costanera»); varios a media venta y dos sin ninguna venta;
   - 58 clientes nuevos (14 con nombre fijo y 44 generados) y los tres de la semilla base, con un
     historial que da exactamente su nivel (RN-19: Bronce 0-2, Plata 3-6, Oro 7 o más compras en 12
     meses). Quedan unos 20 por nivel y nadie pasa de 12 compras en el año: la semilla lo comprueba;
   - importes calculados como GestorFidelizacion.CalcularDescuento: descuento solo sobre las
     entradas (RN-20, RN-21), cargo sobre el subtotal con descuento, servicios sin descuento
     y seguro por entrada (RN-28). El nivel de cada compra es el vigente al hacerla: sube
     después de cada compra confirmada (RN-22);
   - ingresos registrados en los eventos que ya pasaron, una lista de espera en el evento
     agotado y solicitudes de Organizador pendientes, aprobada y rechazada (RN-52).

   La contraseña de los organizadores nuevos es la misma de las cuentas de la semilla base.
   La batería de pruebas no usa esta semilla: corre sobre BddEventAura_Pruebas. Solo la usa
   Pruebas/CapturarReforma.ps1 para la base BddEventAura_Capturas_Demostracion.

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

-- ═══════════════ Organizadores ═══════════════
-- Seis más, ya aprobados y con el primer evento aprobado (RN-54): publican directo.
DECLARE @productoras TABLE (Clave VARCHAR(10) PRIMARY KEY, Nombre VARCHAR(40), Apellido VARCHAR(40), Mail VARCHAR(150));
INSERT @productoras VALUES
('comedia', 'Laura',  'Méndez',  'laura.mendez@demo.eventaura.example'),
('cultura', 'Diego',  'Sosa',    'diego.sosa@demo.eventaura.example'),
('santafe', 'Carla',  'Peralta', 'carla.peralta@demo.eventaura.example'),
('cordoba', 'Ramiro', 'Quiroga', 'ramiro.quiroga@demo.eventaura.example'),
('parana',  'Inés',   'Correa',  'ines.correa@demo.eventaura.example'),
('baires',  'Gabriel','Ortiz',   'gabriel.ortiz@demo.eventaura.example');
INSERT Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
SELECT Nombre, Apellido, Mail, @hash, 'Organizador', 'Bronce', @mes FROM @productoras;
INSERT Propietario (ID, PrimerEventoAprobado)
SELECT u.ID, 1 FROM Usuario u JOIN @productoras p ON p.Mail = u.Mail;

-- Ocho sedes más; con el Galpón 17, nueve en cinco ciudades. Las de Rosario son las de la versión anterior.
INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID)
SELECT s.Nombre, s.Direccion, s.Capacidad, ISNULL(u.ID, @organizador)
FROM (VALUES
    ('Galpón 11',                       'Estévez Boero 980, Rosario',       420, NULL),
    ('Teatro Municipal La Comedia',     'Mitre 958, Rosario',               620, 'comedia'),
    ('Cultural Fontanarrosa',           'San Martín 1080, Rosario',         280, 'cultura'),
    ('Centro Cultural Parque Alem',     'Nansen 100, Rosario',              180, 'cultura'),
    ('Teatro Municipal 1° de Mayo',     'San Martín 2020, Santa Fe',        900, 'santafe'),
    ('Teatro del Libertador',           'Av. Vélez Sarsfield 365, Córdoba', 1000, 'cordoba'),
    ('Teatro 3 de Febrero',             '25 de Junio 60, Paraná',           700, 'parana'),
    ('Teatro Ópera',                    'Av. Corrientes 860, Buenos Aires', 2000, 'baires')
) AS s(Nombre, Direccion, Capacidad, Clave)
LEFT JOIN @productoras p ON p.Clave = s.Clave
LEFT JOIN Usuario u ON u.Mail = p.Mail;

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
(20,'Fiesta de fin de temporada',        110,23,'Concierto','Galpón 17'),
(21,'Noche de tango en el 1° de Mayo',    12,21,'Concierto','Teatro Municipal 1° de Mayo'),
(22,'Hamlet en la costanera',             26,20,'Teatro',   'Teatro Municipal 1° de Mayo'),
(23,'Cuarteto del centro',                15,22,'Concierto','Teatro del Libertador'),
(24,'Ballet folclórico serrano',          44,20,'Teatro',   'Teatro del Libertador'),
(25,'Feria del libro entrerriano',        20,17,'Cultural', 'Teatro 3 de Febrero'),
(26,'Paraná canta',                       33,21,'Concierto','Teatro 3 de Febrero'),
(27,'Comedia musical en la avenida',      41,21,'Teatro',   'Teatro Ópera'),
(28,'Noche de stand-up',                  19,22,'Teatro',   'Teatro Ópera'),
(29,'Orquesta juvenil del litoral',        5,20,'Concierto','Teatro Municipal La Comedia'),
(30,'Jazz en el galpón',                  62,21,'Concierto','Galpón 11'),
(31,'Muestra de cine rosarino',            3,19,'Cultural', 'Cultural Fontanarrosa'),
(32,'Electrónica en el puerto',           75,23,'Concierto','Galpón 17'),
(33,'Rock santafesino',                   96,21,'Concierto','Teatro Municipal 1° de Mayo'),
(34,'Cine bajo las estrellas',           120,20,'Cultural', 'Centro Cultural Parque Alem');

-- Sectores por sede, del más cercano al escenario al más lejano: el precio baja en ese orden.
-- Vendidas = entradas que se cargan. Ningún evento de esta agenda queda agotado: el único es
-- «Clásico Rosarino», de la semilla base. Pocas entradas: 12 (3 libres), 22 (Platea baja,
-- 4 libres) y 31 (Preferencial, 2 libres). Sin ventas: 18 y 34 (y 7, que ya pasó).
DECLARE @sectores TABLE (Pos INT, Orden INT, Nombre VARCHAR(50), Precio DECIMAL(12,2),
                         Capacidad INT, Vendidas INT, Sector INT NULL, PRIMARY KEY (Pos, Orden));
INSERT @sectores (Pos, Orden, Nombre, Precio, Capacidad, Vendidas) VALUES
( 1,1,'Platea',      22000, 40,16),( 1,2,'Palcos',      18000, 12, 6),( 1,3,'Tertulia',    12000, 20, 3),
( 2,1,'Preferencial',12000, 20, 8),( 2,2,'General',      9000, 60,10),
( 3,1,'General',      8500, 24,22),
( 4,1,'VIP',         32000, 20, 6),( 4,2,'Campo',       18000,120,26),
( 5,1,'Platea',      16000, 36,12),( 5,2,'Palcos',      13000, 10, 2),
( 6,1,'Mesas',       26000, 16, 8),( 6,2,'General',     18500, 40,14),
( 7,1,'Platea',      18000, 30, 0),( 7,2,'Palcos',      15000,  8, 0),
( 8,1,'General',     11500, 45,15),
( 9,1,'Preferencial',13000, 10, 6),( 9,2,'General',      9000, 50,22),
(10,1,'Mesas',       19000, 12, 5),(10,2,'General',     13000, 50,12),
(11,1,'Platea',      16500, 40,15),(11,2,'Palcos',      14000, 10, 4),(11,3,'Tertulia',    11000, 16, 2),
-- Futuros
(12,1,'General',     12500, 30,27),
(13,1,'Mesas frente al escenario',31000, 24, 6),(13,2,'General',23500,120,18),
(14,1,'VIP frente al escenario',  28000, 24, 4),(14,2,'Campo',  15000,150,22),
(15,1,'Preferencial',13000, 40,14),(15,2,'General',     10500,100,20),
(16,1,'Platea baja', 25000,200,30),(16,2,'Palcos',      21000, 60, 6),(16,3,'Platea alta', 17000,200,10),(16,4,'Tertulia',    11000,120, 0),
(17,1,'VIP',         44500, 60, 6),(17,2,'Campo delantero',36500,400,20),(17,3,'Campo general',28500,1200,24),
(18,1,'General',     10500, 24, 0),
(19,1,'Preferencial',12000, 30, 2),(19,2,'General',      9500,120, 6),
(20,1,'VIP',         34000, 50, 0),(20,2,'Campo',       20000,800, 3),
(21,1,'Platea baja', 30000,300,30),(21,2,'Platea alta', 22000,300,12),(21,3,'Paraíso',     12000,200, 2),
(22,1,'Platea baja', 26000, 40,36),(22,2,'Platea alta', 19000,300,30),(22,3,'Paraíso',     10000,200, 8),
(23,1,'Platea preferencial',38000,200,20),(23,2,'Platea',28000,400,30),(23,3,'Cazuela',16000,300, 4),
(24,1,'Platea preferencial',34000,200,12),(24,2,'Platea',24000,400, 8),(24,3,'Cazuela',14000,300, 0),
(25,1,'Platea',      12000,300,20),(25,2,'Pullman',      9000,200, 6),
(26,1,'Platea',      26000,300,30),(26,2,'Pullman',     19000,200,10),(26,3,'Galería',     11000,150, 2),
(27,1,'Platea preferencial',55000,400,40),(27,2,'Platea',42000,800,50),(27,3,'Pullman',30000,500,20),(27,4,'Superpullman',20000,300, 0),
(28,1,'Platea',      35000,800,25),(28,2,'Pullman',     25000,600,10),(28,3,'Superpullman',15000,400, 0),
(29,1,'Platea baja', 15000,200,20),(29,2,'Palcos',      12000, 60, 4),(29,3,'Platea alta',  9000,200, 0),
(30,1,'Mesas frente al escenario',26000, 40, 3),(30,2,'General',18500,300,12),
(31,1,'Preferencial', 9000, 40,38),(31,2,'General',      6500,200,30),
(32,1,'VIP',         40000, 60, 0),(32,2,'Campo',       24000,1000, 8),
(33,1,'Platea baja', 28000,300, 6),(33,2,'Platea alta', 20000,300, 0),(33,3,'Paraíso',     11000,200, 0),
(34,1,'Preferencial', 8000, 30, 0),(34,2,'General',      5000,100, 0);

-- Servicios que ofrece cada evento (RN-14), por tipo de servicio de la semilla base.
DECLARE @ofertas TABLE (Pos INT, Tipo VARCHAR(20));
INSERT @ofertas VALUES
(4,'Merchandising'),(4,'Traslado'),
(6,'Merchandising'),(10,'Merchandising'),(13,'Merchandising'),
(14,'Merchandising'),(14,'Traslado'),
(17,'Merchandising'),(17,'Traslado'),(17,'Hospedaje'),(17,'VIP'),
(20,'Merchandising'),(20,'Traslado'),
(2,'Merchandising'),(9,'Merchandising'),(15,'Merchandising'),
(1,'Traslado'),(11,'Traslado'),(16,'Traslado'),
(21,'Traslado'),(21,'Hospedaje'),(22,'Hospedaje'),(23,'Hospedaje'),(23,'Merchandising'),
(24,'Hospedaje'),(26,'Merchandising'),(26,'Hospedaje'),(27,'Hospedaje'),(27,'Traslado'),(27,'VIP'),
(28,'Hospedaje'),(30,'Merchandising'),(32,'Merchandising'),(32,'Traslado'),(33,'Traslado');

DECLARE @pos INT = 1, @evento INT;
WHILE @pos <= 34
BEGIN
    INSERT Eventos (Nombre, Fecha, LocalID, Categoria, Estado)
    SELECT a.Nombre, DATEADD(HOUR, a.Hora, CONVERT(DATETIME, DATEADD(DAY, a.Dias, @hoy))), l.ID, a.Categoria, 'Publicado'
    FROM @agenda a JOIN Locales l ON l.Nombre = a.Sede
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

-- Más clientes para repartir el plan de ventas con cantidades verosímiles (RN-19: Bronce de 0 a 2
-- compras en 12 meses, Plata de 3 a 6, Oro desde 7). Sin esto, los pocos clientes Oro absorbían
-- todo lo que sobraba y quedaban con más de 40 compras cada uno. Se suman de a tres (Bronce, Plata y
-- Oro), mientras alcancen las compras; para el cálculo, un Oro cuenta como 9.
DECLARE @compras INT = (SELECT COUNT(*) FROM @plan);
DECLARE @libres INT = @compras - (SELECT SUM(Objetivo) FROM @clientes WHERE Nivel <> 'Oro')
                    - (SELECT COUNT(*) FROM @clientes WHERE Nivel = 'Oro') * 9;
DECLARE @nombres TABLE (Orden INT, Nombre VARCHAR(40), Clave VARCHAR(40));
INSERT @nombres VALUES (1,'Camila','camila'),(2,'Gonzalo','gonzalo'),(3,'Rocío','rocio'),(4,'Ignacio','ignacio'),
    (5,'Florencia','florencia'),(6,'Lautaro','lautaro'),(7,'Milagros','milagros'),(8,'Santiago','santiago'),
    (9,'Abril','abril'),(10,'Franco','franco'),(11,'Josefina','josefina'),(12,'Benjamín','benjamin'),
    (13,'Delfina','delfina'),(14,'Thiago','thiago'),(15,'Catalina','catalina'),(16,'Bautista','bautista');
DECLARE @apellidos TABLE (Orden INT, Apellido VARCHAR(40), Clave VARCHAR(40));
INSERT @apellidos VALUES (1,'Gómez','gomez'),(2,'Romero','romero'),(3,'Medina','medina'),
    (4,'Herrera','herrera'),(5,'Domínguez','dominguez'),(6,'Ojeda','ojeda');
DECLARE @extra TABLE (k INT PRIMARY KEY, Nombre VARCHAR(40), Apellido VARCHAR(40), Mail VARCHAR(150),
                      Nivel VARCHAR(8), Objetivo INT, Peso INT);
INSERT @extra (k, Nombre, Apellido, Mail)
SELECT ROW_NUMBER() OVER (ORDER BY a.Orden, n.Orden), n.Nombre, a.Apellido,
       n.Clave + '.' + a.Clave + '@demo.eventaura.example'
FROM @nombres n CROSS JOIN @apellidos a;
UPDATE @extra SET Nivel = CHOOSE(k % 3 + 1, 'Oro', 'Bronce', 'Plata'),
                  Objetivo = CHOOSE(k % 3 + 1, 0, 1 + k % 2, 3 + k % 4);
UPDATE @extra SET Peso = IIF(Nivel = 'Oro', 9, Objetivo);
DELETE e FROM @extra e
WHERE e.k > ISNULL((SELECT MAX(t.k) FROM (SELECT k, SUM(Peso) OVER (ORDER BY k) AS acumulado FROM @extra) t
                    WHERE t.acumulado <= @libres), 0);
INSERT Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
SELECT Nombre, Apellido, Mail, @hash, 'Cliente', 'Bronce', @mes FROM @extra;
INSERT Cliente (ID) SELECT u.ID FROM Usuario u JOIN @extra e ON e.Mail = u.Mail;
INSERT @clientes (ID, Nivel, Objetivo) SELECT u.ID, e.Nivel, e.Objetivo FROM Usuario u JOIN @extra e ON e.Mail = u.Mail;

-- Los Oro se reparten lo que sobra después de los objetivos de Bronce y Plata.
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
WITH numeros AS (SELECT TOP (200) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS k FROM sys.all_objects),
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
-- «Clásico Rosarino», de la semilla base, es el único evento agotado. Se anotan tres clientes
-- que no compraron para ese evento, después de la última venta. Ninguno tiene suscripción (RN-35).
DECLARE @agotado INT = (SELECT TOP (1) ID FROM Eventos WHERE Nombre = 'Clásico Rosarino');
DECLARE @ultimaVenta DATETIME = (SELECT MAX(FechaVenta) FROM Entradas WHERE ID_Evento = @agotado);
INSERT ListaEspera (ID_Evento, ID_Usuario, FechaAlta, ConSuscripcion)
SELECT TOP (3) @agotado, c.ID,
       DATEADD(MINUTE, 90 * ROW_NUMBER() OVER (ORDER BY c.Nivel DESC, c.ID), @ultimaVenta), 0
FROM @clientes c
WHERE c.ID NOT IN (SELECT ID_Usuario FROM Entradas WHERE ID_Evento = @agotado)
  AND c.ID <> @plata   -- Paula se anota en vivo (guion, sección 6, paso 4)
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

-- ═══════════════ Comprobaciones de lo que pidió JC (26/11) ═══════════════
IF (SELECT COUNT(*) FROM Eventos WHERE Estado = 'Publicado' AND Fecha > @ahora) <> 25
    THROW 51009, 'El catálogo no tiene exactamente 25 eventos futuros.', 1;
IF (SELECT COUNT(DISTINCT l.PropietarioID) FROM Eventos e JOIN Locales l ON l.ID = e.LocalID
    WHERE e.Estado = 'Publicado' AND e.Fecha > @ahora) NOT BETWEEN 6 AND 8
    THROW 51010, 'Los eventos futuros no son de 6 a 8 organizadores.', 1;
IF (SELECT COUNT(*) FROM Eventos e WHERE e.Estado = 'Publicado' AND e.Fecha > @ahora
      AND NOT EXISTS (SELECT 1 FROM Sectores x WHERE x.EventoID = e.ID
                      AND x.Capacidad > (SELECT COUNT(*) FROM Entradas en WHERE en.ID_Sector = x.ID
                                         AND en.Estado IN ('Pendiente','Confirmada')))) <> 1
    THROW 51011, 'No hay exactamente un evento futuro agotado.', 1;
IF EXISTS (SELECT 1 FROM @sectores a JOIN @sectores b ON b.Pos = a.Pos AND b.Orden = a.Orden + 1
           WHERE b.Precio >= a.Precio)
    THROW 51012, 'Un sector más lejano del escenario no es más barato que el anterior.', 1;
IF EXISTS (SELECT 1 FROM Compras WHERE Estado = 'Confirmada' AND FechaCompra >= DATEADD(MONTH, -@ventana, @ahora)
           GROUP BY ID_Usuario HAVING COUNT(*) > 12)
    THROW 51013, 'Un cliente quedó con más de 12 compras en 12 meses: no es verosímil.', 1;

COMMIT TRANSACTION;
