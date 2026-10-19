/* ═══════════════════════════════════════════════════════════════════════════
   EventAura — 03-datos-reporte.sql
   Juego de datos simulados para los reportes del anexo A02 (RPT-01 a RPT-05).

   NO se corre con la semilla. Se corre aparte, después de 01-esquema.sql y
   02-semilla.sql, solo cuando se quieren ver los reportes con volumen. Empieza
   borrando lo que él mismo cargó antes, así se puede correr dos veces seguidas.

   Qué carga (período de los ejemplos: 01/09/2025 al 30/11/2025):
   - Un local «Predio Rosario (datos de reporte)» del Administrador, con cinco
     eventos ya pasados, un sector «General» cada uno:
       Duki en Rosario              Concierto  cap 1.560  1.500 entradas a $25.000
       Rosario Central vs Newell's  Deportivo  cap 1.350  1.240 entradas a $18.000
       Airbag – Gira 2025           Concierto  cap 1.120    980 entradas a $20.000
       Fiesta de la Colectividad    Cultural   cap 1.000    450 entradas a $10.000
       El fantasma de Canterville   Teatro     cap   500    310 entradas a  $9.000
     = 4.480 entradas confirmadas y $86.710.000 (RPT-01).
   - 1.178 clientes rpt0001..rpt1178@datos.eventaura.com y 2.800 compras
     confirmadas: 818 con 1 compra (Bronce), 260 con 4 o 5 (Plata, 4,2 de
     promedio) y 100 con 8 o 9 (Oro, 8,9 de promedio) (RPT-03).
   - Servicios por compra, con el precio congelado de entonces en
     CompraServicios.PrecioUnit (RN-16; el catálogo de hoy puede valer otra cosa):
       Solo entrada            2.690 entradas   $48.420.000
       Entrada + Traslado        890            $19.580.000  ($10.000 por compra)
       Entrada + Hospedaje       510            $22.950.000  ($40.000 por compra)
       Entrada + VIP + Merch     250            $18.750.000  ($80.000 + $20.000)
       Entrada + Merchandising   140             $3.360.000  ($20.000 por compra)
     = $113.060.000; la diferencia con RPT-01 son los $26.350.000 de servicios (RPT-02).
   - 504 compras de una entrada con seguro, de clientes sin suscripción (18,0 %),
     y 164 suscripciones mensuales y 51 anuales (RPT-04). Los precios salen de
     Parametros: hay que tener SUSCRIPCION_MENSUAL_PRECIO = 9000 y
     SUSCRIPCION_ANUAL_PRECIO = 90000, como en la semilla.
   - Listas de espera: Duki 430, Airbag 180, Central 95 = 705 (RPT-05).

   Las compras se cargan sin descuento por nivel (NivelAplicado Bronce, 0 %): los
   reportes miden entradas e ingreso bruto, que no dependen del descuento, y cada
   fila cierra Base − Descuento + Servicios + Cargo + Seguro = TotalPagado igual.
   Cada compra tiene su pago Aprobado (RN-31), cada entrada su QR único (RN-18) y
   cada entrada asegurada su póliza.
   ═══════════════════════════════════════════════════════════════════════════ */

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;   -- índices filtrados de Entradas y Suscripciones
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ═══════════════ 1. Borrar lo cargado antes por este script ═══════════════
DECLARE @Local INT = (SELECT ID FROM Locales WHERE Nombre = N'Predio Rosario (datos de reporte)');

DECLARE @ViejosEventos TABLE (ID INT PRIMARY KEY);
INSERT INTO @ViejosEventos SELECT ID FROM Eventos WHERE LocalID = @Local;

DECLARE @ViejosUsuarios TABLE (ID INT PRIMARY KEY);
INSERT INTO @ViejosUsuarios
SELECT ID FROM Usuario WHERE Mail LIKE 'rpt[0-9][0-9][0-9][0-9]@datos.eventaura.com';

DECLARE @ViejasCompras TABLE (ID INT PRIMARY KEY);
INSERT INTO @ViejasCompras
SELECT ID FROM Compras WHERE ID_Usuario IN (SELECT ID FROM @ViejosUsuarios)
UNION
SELECT DISTINCT ID_Compra FROM Entradas
WHERE ID_Evento IN (SELECT ID FROM @ViejosEventos) AND ID_Compra IS NOT NULL;

DELETE s FROM Seguros s JOIN Entradas e ON e.ID = s.ID_Entrada
WHERE e.ID_Compra IN (SELECT ID FROM @ViejasCompras) OR e.ID_Evento IN (SELECT ID FROM @ViejosEventos);
DELETE FROM Entradas WHERE ID_Compra IN (SELECT ID FROM @ViejasCompras) OR ID_Evento IN (SELECT ID FROM @ViejosEventos);
DELETE FROM CompraServicios WHERE ID_Compra IN (SELECT ID FROM @ViejasCompras);
DELETE FROM Pagos WHERE ID_Compra IN (SELECT ID FROM @ViejasCompras);
DELETE FROM Compras WHERE ID IN (SELECT ID FROM @ViejasCompras);
DELETE FROM ListaEspera WHERE ID_Usuario IN (SELECT ID FROM @ViejosUsuarios) OR ID_Evento IN (SELECT ID FROM @ViejosEventos);
DELETE FROM Suscripciones WHERE ID_Usuario IN (SELECT ID FROM @ViejosUsuarios);
DELETE FROM ServiciosEvento WHERE ID_Evento IN (SELECT ID FROM @ViejosEventos);
DELETE FROM Sectores WHERE EventoID IN (SELECT ID FROM @ViejosEventos);
DELETE FROM Eventos WHERE ID IN (SELECT ID FROM @ViejosEventos);
DELETE FROM Cliente WHERE ID IN (SELECT ID FROM @ViejosUsuarios);
DELETE FROM Usuario WHERE ID IN (SELECT ID FROM @ViejosUsuarios);
DELETE FROM Locales WHERE ID = @Local;

-- ═══════════════ 2. Parámetros: los mismos valores que usa la aplicación ═══════════════
DECLARE @Cargo        DECIMAL(6,3)  = (SELECT CAST(Valor AS DECIMAL(6,3))  FROM Parametros WHERE Clave = 'CARGO_SERVICIO');
DECLARE @PrecioSeguro DECIMAL(12,2) = (SELECT CAST(Valor AS DECIMAL(12,2)) FROM Parametros WHERE Clave = 'SEGURO_PRECIO');
DECLARE @HorasSeguro  INT           = (SELECT CAST(Valor AS INT)           FROM Parametros WHERE Clave = 'SEGURO_HORAS_COBERTURA');
DECLARE @PrecioMensual DECIMAL(12,2) = (SELECT CAST(Valor AS DECIMAL(12,2)) FROM Parametros WHERE Clave = 'SUSCRIPCION_MENSUAL_PRECIO');
DECLARE @PrecioAnual   DECIMAL(12,2) = (SELECT CAST(Valor AS DECIMAL(12,2)) FROM Parametros WHERE Clave = 'SUSCRIPCION_ANUAL_PRECIO');

DECLARE @Admin INT = (SELECT TOP 1 ID FROM Usuario WHERE TipoUsuario = 'Propietario' ORDER BY ID);

-- Números 1..N para generar filas sin bucles fila por fila.
DECLARE @Numeros TABLE (n INT PRIMARY KEY);
INSERT INTO @Numeros
SELECT TOP (1200) ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
FROM sys.all_objects a CROSS JOIN sys.all_objects b;

-- ═══════════════ 3. Local, eventos y sectores ═══════════════
INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID)
VALUES (N'Predio Rosario (datos de reporte)', N'Bv. Oroño 2000, Rosario', 2000, @Admin);
SET @Local = SCOPE_IDENTITY();

DECLARE @Def TABLE (Clave CHAR(1) PRIMARY KEY, Nombre NVARCHAR(100), Categoria VARCHAR(20),
                    Fecha DATETIME, Capacidad INT, Precio DECIMAL(12,2));
INSERT INTO @Def VALUES
('D', N'Duki en Rosario',              'Concierto', '2025-09-27T21:00:00', 1560, 25000),
('C', N'Rosario Central vs Newell''s', 'Deportivo', '2025-10-19T17:00:00', 1350, 18000),
('A', N'Airbag – Gira 2025',           'Concierto', '2025-11-01T21:00:00', 1120, 20000),
('F', N'Fiesta de la Colectividad',    'Cultural',  '2025-11-15T18:00:00', 1000, 10000),
('T', N'El fantasma de Canterville',   'Teatro',    '2025-11-29T20:30:00',  500,  9000);

DECLARE @Ev TABLE (Clave CHAR(1) PRIMARY KEY, ID INT, Sector INT);
MERGE INTO Eventos USING @Def d ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (Nombre, Fecha, LocalID, Categoria, Estado)
    VALUES (d.Nombre, d.Fecha, @Local, d.Categoria, 'Publicado')
OUTPUT d.Clave, INSERTED.ID INTO @Ev (Clave, ID);

INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad)
SELECT ev.ID, 'General', d.Precio, d.Capacidad FROM @Ev ev JOIN @Def d ON d.Clave = ev.Clave;
UPDATE ev SET Sector = s.ID FROM @Ev ev JOIN Sectores s ON s.EventoID = ev.ID;

-- ═══════════════ 4. Servicios: los del catálogo de la semilla, uno por tipo ═══════════════
DECLARE @SvTraslado INT = (SELECT MIN(ID) FROM Servicios WHERE Tipo = 'Traslado');
DECLARE @SvHospedaje INT = (SELECT MIN(ID) FROM Servicios WHERE Tipo = 'Hospedaje');
DECLARE @SvMerch INT = (SELECT MIN(ID) FROM Servicios WHERE Tipo = 'Merchandising');
DECLARE @SvVIP INT = (SELECT MIN(ID) FROM Servicios WHERE Tipo = 'VIP');

INSERT INTO ServiciosEvento (ID_Evento, ID_Servicio)            -- RN-14
SELECT ev.ID, x.Servicio
FROM @Ev ev
JOIN (VALUES ('D', @SvTraslado), ('D', @SvHospedaje), ('D', @SvMerch), ('D', @SvVIP),
             ('T', @SvTraslado), ('T', @SvMerch), ('F', @SvMerch)) x (Clave, Servicio)
  ON x.Clave = ev.Clave;

-- ═══════════════ 5. Clientes ═══════════════
-- n 1..818 Bronce (1 compra), 819..1026 Plata (4), 1027..1078 Plata (5),
-- 1079..1088 Oro (8), 1089..1178 Oro (9). Las contraseñas se migran a PBKDF2 al
-- primer inicio de sesión, igual que las de la semilla.
INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion)
SELECT N'Cliente', N'Reporte ' + RIGHT('000' + CAST(n AS VARCHAR(4)), 4),
       'rpt' + RIGHT('000' + CAST(n AS VARCHAR(4)), 4) + '@datos.eventaura.com',
       'Cliente123', 'Cliente',
       CASE WHEN n <= 818 THEN 'Bronce' WHEN n <= 1078 THEN 'Plata' ELSE 'Oro' END,
       '2025-11-01'
FROM @Numeros WHERE n <= 1178;

DECLARE @U TABLE (n INT PRIMARY KEY, ID INT, Compras INT);
INSERT INTO @U (n, ID, Compras)
SELECT CAST(SUBSTRING(Mail, 4, 4) AS INT), ID,
       CASE WHEN CAST(SUBSTRING(Mail, 4, 4) AS INT) <= 818  THEN 1
            WHEN CAST(SUBSTRING(Mail, 4, 4) AS INT) <= 1026 THEN 4
            WHEN CAST(SUBSTRING(Mail, 4, 4) AS INT) <= 1078 THEN 5
            WHEN CAST(SUBSTRING(Mail, 4, 4) AS INT) <= 1088 THEN 8
            ELSE 9 END
FROM Usuario WHERE Mail LIKE 'rpt[0-9][0-9][0-9][0-9]@datos.eventaura.com';

INSERT INTO Cliente (ID) SELECT ID FROM @U;

-- ═══════════════ 6. Plan de compras: evento, combinación, entradas por compra ═══════════════
-- Combo: S solo entrada · T traslado · H hospedaje · V VIP + merchandising · M merchandising.
-- Las aseguradas van primero: así les tocan los primeros clientes Bronce (n 1..504), que
-- no tienen suscripción (RN-37: a un suscriptor el seguro no se le cobra).
DECLARE @Spec TABLE (Orden INT IDENTITY(1,1), Clave CHAR(1), Combo CHAR(1), Entradas INT, Compras INT, Seguro BIT);
INSERT INTO @Spec (Clave, Combo, Entradas, Compras, Seguro) VALUES
('C','S',1,300,1), ('A','S',1,204,1),                                   -- 504 con seguro
('C','S',1,540,0), ('C','S',2,200,0),                                   -- Central   1.240
('A','S',1,376,0), ('A','S',2,200,0),                                   -- Airbag      980
('D','S',2, 35,0), ('D','S',1, 50,0),                                   -- Duki        120
('F','S',2,100,0), ('F','S',1,150,0),                                   -- Fiesta      350
('D','T',6, 45,0), ('D','T',5, 70,0), ('T','T',6, 20,0), ('T','T',5, 30,0),  -- Traslado  890
('D','H',2,255,0),                                                      -- Hospedaje   510
('D','V',2,125,0),                                                      -- VIP         250
('F','M',2, 30,0), ('F','M',1, 40,0), ('T','M',2, 10,0), ('T','M',1, 20,0);  -- Merch     140

CREATE TABLE #Plan (n INT PRIMARY KEY, Clave CHAR(1), Combo CHAR(1), Entradas INT, Seguro BIT,
                    IdUsuario INT, IdCompra INT NULL);
INSERT INTO #Plan (n, Clave, Combo, Entradas, Seguro)
SELECT ROW_NUMBER() OVER (ORDER BY s.Orden, nu.n), s.Clave, s.Combo, s.Entradas, s.Seguro
FROM @Spec s JOIN @Numeros nu ON nu.n <= s.Compras;

-- Un turno por compra de cada cliente, en orden de cliente: 2.800 turnos.
WITH Turnos AS (
    SELECT ROW_NUMBER() OVER (ORDER BY u.n, nu.n) AS n, u.ID
    FROM @U u JOIN @Numeros nu ON nu.n <= u.Compras
)
UPDATE p SET IdUsuario = t.ID FROM #Plan p JOIN Turnos t ON t.n = p.n;

-- ═══════════════ 7. Compras ═══════════════
-- Fecha: entre el 01/09/2025 y el día anterior al evento, repartida por el número de plan.
DECLARE @Map TABLE (n INT PRIMARY KEY, ID INT);
MERGE INTO Compras USING (
    SELECT p.n, p.IdUsuario, p.Entradas, p.Seguro, d.Precio,
           DATEADD(MINUTE,
                   -((p.n * 7919) % DATEDIFF(MINUTE, '2025-09-01', DATEADD(DAY, -1, d.Fecha))),
                   DATEADD(DAY, -1, d.Fecha)) AS Fecha,
           p.Entradas * d.Precio AS Base,
           CASE p.Combo WHEN 'T' THEN 10000 WHEN 'H' THEN 40000 WHEN 'V' THEN 100000
                        WHEN 'M' THEN 20000 ELSE 0 END AS Servicios
    FROM #Plan p JOIN @Def d ON d.Clave = p.Clave
) x ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado, BaseEntradas,
            MontoDescuento, MontoServicios, CargoServicio, MontoSeguro, ConSeguro, TotalPagado,
            MontoReembolsado, ReservaVence)
    VALUES (x.IdUsuario, x.Fecha, 'Confirmada', 'Bronce', 0, x.Base,
            0, x.Servicios, ROUND(x.Base * @Cargo, 2),
            CASE WHEN x.Seguro = 1 THEN x.Entradas * @PrecioSeguro ELSE 0 END, x.Seguro,
            x.Base + x.Servicios + ROUND(x.Base * @Cargo, 2)
              + CASE WHEN x.Seguro = 1 THEN x.Entradas * @PrecioSeguro ELSE 0 END,
            0, NULL)
OUTPUT x.n, INSERTED.ID INTO @Map (n, ID);

UPDATE p SET IdCompra = m.ID FROM #Plan p JOIN @Map m ON m.n = p.n;

-- RN-30 + RN-31: el pago aprobado de cada compra.
INSERT INTO Pagos (ID_Compra, FechaPago, Estado, Ultimos4, Token, Motivo)
SELECT c.ID, c.FechaCompra, 'Aprobado', '1111', 'tok_rpt_' + CAST(c.ID AS VARCHAR(12)), NULL
FROM #Plan p JOIN Compras c ON c.ID = p.IdCompra;

-- ═══════════════ 8. Entradas: una fila por entrada, con su QR (RN-18) ═══════════════
INSERT INTO Entradas (ID_Evento, ID_Sector, ID_Usuario, Precio, CantidadVendida, FechaVenta,
                      ID_Compra, Estado, CodigoQR)
SELECT ev.ID, ev.Sector, p.IdUsuario, d.Precio, 1, c.FechaCompra, p.IdCompra, 'Confirmada',
       'EA-RPT-' + REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', '')
FROM #Plan p
JOIN @Ev ev ON ev.Clave = p.Clave
JOIN @Def d ON d.Clave = p.Clave
JOIN Compras c ON c.ID = p.IdCompra
JOIN @Numeros nu ON nu.n <= p.Entradas;

-- RN-28: una póliza por entrada asegurada, vence SEGURO_HORAS_COBERTURA antes del evento.
INSERT INTO Seguros (ID_Entrada, Precio, VenceEl)
SELECT e.ID, @PrecioSeguro, DATEADD(HOUR, -@HorasSeguro, d.Fecha)
FROM #Plan p
JOIN Entradas e ON e.ID_Compra = p.IdCompra
JOIN @Def d ON d.Clave = p.Clave
WHERE p.Seguro = 1;

-- ═══════════════ 9. Servicios de cada compra (precio congelado, RN-16) ═══════════════
INSERT INTO CompraServicios (ID_Compra, ID_Servicio, Cantidad, PrecioUnit)
SELECT p.IdCompra, x.Servicio, 1, x.Precio
FROM #Plan p
JOIN (VALUES ('T', @SvTraslado, 10000), ('H', @SvHospedaje, 40000),
             ('V', @SvVIP, 80000), ('V', @SvMerch, 20000), ('M', @SvMerch, 20000)) x (Combo, Servicio, Precio)
  ON x.Combo = p.Combo;

-- ═══════════════ 10. Suscripciones (RN-25: una por cliente) ═══════════════
-- Mensuales: clientes 819..982 (164). Anuales: 983..1033 (51). Todos Plata u Oro.
INSERT INTO Suscripciones (ID_Usuario, [Plan], FechaInicio, FechaFin, Precio, Activa)
SELECT u.ID,
       CASE WHEN u.n <= 982 THEN 'Mensual' ELSE 'Anual' END,
       DATEADD(DAY, u.n % 90, CAST('2025-09-01' AS DATE)),
       CASE WHEN u.n <= 982 THEN DATEADD(MONTH, 1, DATEADD(DAY, u.n % 90, CAST('2025-09-01' AS DATE)))
            ELSE DATEADD(YEAR, 1, DATEADD(DAY, u.n % 90, CAST('2025-09-01' AS DATE))) END,
       CASE WHEN u.n <= 982 THEN @PrecioMensual ELSE @PrecioAnual END,
       1
FROM @U u WHERE u.n BETWEEN 819 AND 1033;

-- ═══════════════ 11. Listas de espera (RN-35, RN-36) ═══════════════
-- Duki: clientes 101..530 (430). Airbag: 601..780 (180). Central: 901..995 (95).
-- ConSuscripcion se congela al inscribirse: 1 si ya tenía la suscripción ese día.
INSERT INTO ListaEspera (ID_Evento, ID_Usuario, FechaAlta, ConSuscripcion)
SELECT ev.ID, u.ID, x.FechaAlta,
       CASE WHEN EXISTS (SELECT 1 FROM Suscripciones s
                         WHERE s.ID_Usuario = u.ID AND s.FechaInicio <= x.FechaAlta) THEN 1 ELSE 0 END
FROM @U u
JOIN (VALUES ('D', 101, 530), ('A', 601, 780), ('C', 901, 995)) r (Clave, Desde, Hasta)
  ON u.n BETWEEN r.Desde AND r.Hasta
JOIN @Ev ev ON ev.Clave = r.Clave
JOIN @Def d ON d.Clave = r.Clave
CROSS APPLY (SELECT DATEADD(MINUTE, -(u.n * 37 % 7200), DATEADD(DAY, -2, d.Fecha)) AS FechaAlta) x;

DROP TABLE #Plan;
COMMIT TRANSACTION;
GO
