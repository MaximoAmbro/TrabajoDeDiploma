# Pruebas manuales pendientes

Las pruebas de interfaz se difirieron hasta terminar las 10 secciones. Este archivo
acumula lo que quedó sin verificar, para no depender de la memoria al final.

**Cada sección agrega sus pendientes acá antes de commitear.** Formato: qué probar,
cómo saber si está bien, y qué commit lo introdujo.

---

## Sección 3 — `824513d`

| # | Qué probar | Resultado esperado |
|---|---|---|
| 3.1 | Login con `admin@eventaura.com` / `Admin1234` | Abre el menú de Propietario |
| 3.2 | Volver a entrar con la misma contraseña | Funciona contra el hash ya migrado |
| 3.3 | 5 intentos fallidos seguidos | El 6.º se rechaza aunque la contraseña sea correcta |
| 3.4 | Botón «crear cuenta» desde el login | Abre `frmCrearUsuario`, no `frmAgregarEvento` |
| 3.5 | Registrar usuario con contraseña `abc` | Lo rechaza por RN-02 |

## Sección 4 — `d2b3c3d`

| # | Qué probar | Resultado esperado |
|---|---|---|
| 4.1 | Alta de evento: cargar evento, primer sector, volver atrás, cargar un segundo | **No** se duplica el evento. Dos caminos distintos: `Publicar` y `AgregarSector` |
| 4.2 | Precio de sector con decimales (`12500,50`) | Acepta coma o punto y guarda los decimales |
| 4.3 | Publicar evento sin sectores | Mensaje de RN-06, no guarda nada |
| 4.4 | Eliminar «Clásico Rosarino» (tiene ventas) | Mensaje de RN-11 y ofrece suspender |
| 4.5 | Compra del lado Cliente | **Va a fallar**: `FrmCompraCliente` consulta `Sectores.PrecioID`, columna inexistente. Se resuelve en la Sección 5 |
| 4.6 | Alta y listado de locales | Funciona desde la interfaz |

## Correcciones C-1 a C-7 — commit «Correcciones: estado Borrador, RN-38, usabilidad y formato del listado»

Antes de probar, aplicar a la base de desarrollo el CHECK nuevo (está en
`BaseDatos/01-esquema.sql`, pero el script no se puede volver a correr entero):

```sql
ALTER TABLE Eventos ADD CONSTRAINT CK_Eventos_Estado
      CHECK (Estado IN ('Borrador','Publicado','Suspendido'));
```

C-5 cambia el flujo de alta: **reemplaza a 4.1 y 4.3**, que describían el flujo en memoria.

| # | Qué probar | Resultado esperado |
|---|---|---|
| C.1 | Abrir la aplicación | El cursor arranca en el campo de mail, no en el de contraseña |
| C.2 | Login con contraseña incorrecta, 5 veces seguidas | Del 1.º al 4.º: «Le quedan 4… 1 intento(s)». El 5.º bloquea: «Vuelva a intentar en 15 minutos» |
| C.3 | Con la cuenta ya bloqueada, intentar de nuevo | Indica los minutos que faltan, que bajan con el tiempo |
| C.4 | Registrar local: tabular desde Nombre | Nombre → Dirección → Altura → Capacidad → botón |
| C.5 | Abrir «Registrar sector» | El botón dice «Agregar sector» |
| C.6 | Agregar evento con nombre y categoría | Mensaje de borrador, vuelve al listado de eventos y la fila aparece con estado **Borrador** |
| C.7 | Agregar evento sin categoría | Pide la categoría y no guarda nada |
| C.8 | Administrar el borrador y agregarle un sector con capacidad 0 | Se guarda; el evento sigue en **Borrador** |
| C.9 | Agregarle un sector con capacidad > 0 y volver al listado | El evento pasa a **Publicado** |
| C.10 | En un local de capacidad 5000, sectores por 4000 y luego uno de 1500 | Mensaje de RN-38 con «Capacidad disponible para este evento: 1000». No guarda el sector |
| C.11 | Eliminar el último sector con capacidad de un evento publicado | Mensaje de RN-06, no lo borra |
| C.12 | Mis locales | Sin columna ID; encabezados «Nombre del local», «Dirección», «Capacidad»; no se edita ni aparece fila vacía; un clic selecciona la fila entera; filas alternadas en gris; columnas ocupan todo el ancho; capacidad alineada a la derecha |
| C.13 | Mis locales → elegir un local con un solo clic → «Eventos del local» | Abre el local elegido (antes había que seleccionar la fila desde el encabezado) |

## Sección 5 — commit «Seccion 5: busqueda, seleccion y armado de paquete»

Entrar como `plata@eventaura.com`. Resuelve 4.5: la pantalla de compra ya no consulta `PrecioID`.

| # | Qué probar | Resultado esperado |
|---|---|---|
| 5.1 | Listado de eventos del cliente | Aparecen el Festival de Rock y el Clásico; **no** la Noche de Teatro (fecha pasada) ni eventos en Borrador o Suspendidos. Columnas: Nombre, Fecha, Categoría, Ubicación, Complejo |
| 5.2 | Elegir el Festival y «Comprar» | Abre la compra de **ese** evento (el título muestra su nombre), aunque haya otro con igual nombre y fecha |
| 5.3 | Pantalla de compra: aspecto general | Grilla con Sector, Precio, Disponibles, Cantidad; checks de servicios y seguro; resumen a la derecha; nada superpuesto ni cortado. **El formulario se ensanchó** de 394 a 760 px |
| 5.4 | Encabezado de nivel | «Tu nivel: Plata · 5,0 % de descuento sobre las entradas» |
| 5.5 | Columna Disponibles (H-03) | VIP 100, Platea 800, Campo 2000 en la semilla |
| 5.6 | Escribir 2 en Campo y salir de la celda | El resumen muestra «2 x Campo» y $ 50.000,00 |
| 5.7 | Marcar Hospedaje | Suma $ 45.000,00; total parcial $ 95.000,00 |
| 5.8 | Escribir 5 en Platea con 2 en Campo | Mensaje de RN-12 y la celda vuelve a 0 |
| 5.9 | Escribir «abc» o un número negativo en Cantidad | Mensaje y la celda vuelve al valor anterior |
| 5.10 | Pedir más que lo disponible en un sector chico (crear uno de capacidad 2 y pedir 3) | Mensaje de RN-08 «quedan 2 entradas» |
| 5.11 | Clásico Rosarino (agotado) | Los cuatro checks dicen «(no disponible)» y están deshabilitados (RN-14) |
| 5.12 | Solo servicios, sin entradas, y «Comprar» | Mensaje de RN-13, no compra |
| 5.13 | «Volver al listado» con algo cargado (H-06) | Pide confirmación; «No» se queda, «Sí» vuelve al listado |
| 5.14 | «Volver al listado» con el paquete vacío | Vuelve sin preguntar |
| 5.15 | Comprar 2 Campo + 1 Platea | Genera 3 PDF en `Documentos\A1.Tickets EVENTAURA` (uno por entrada, con el sector correcto) y graba dos filas en `Entradas` con `ID_Sector`. Disponibles baja al volver a entrar |
| 5.16 | Propietario: agregar un 4.º sector a un evento | Se guarda: ya no hay límite de 3 |

## Sección 5, filtros — commit «Seccion 5: filtros de busqueda en pantalla y limpieza del DataSet muerto»

Pantalla «Eventos» del cliente (RF-03). La grilla bajó 33 px y perdió 33 de alto para dejar
lugar a la fila de filtros; los botones Comprar y Volver no se movieron.

| # | Qué probar | Resultado esperado |
|---|---|---|
| 5.17 | Aspecto de la fila de filtros | Caja de texto con la guía gris «Evento o dirección del local», combo, fecha con casilla, Buscar y Limpiar alineados sobre la grilla, sin tapar el título «Eventos» |
| 5.18 | Al abrir la pantalla | El cursor está en la caja de búsqueda; el combo dice «Todas»; la fecha está destildada |
| 5.19 | Escribir «Rock» y apretar **Enter** | Queda solo el Festival de Rock; no suena el «ding» de Windows |
| 5.20 | Combo «Deportivo» → Buscar | Queda el Clásico Rosarino |
| 5.21 | Combo «Teatro» → Buscar | Mensaje «No hay eventos que coincidan…» en la grilla; la Noche de Teatro no aparece porque ya pasó (RN-09) |
| 5.22 | Con el mensaje en pantalla, «Comprar» | Pide seleccionar un evento; no abre la compra |
| 5.23 | Escribir «Pellegrini» → Buscar | Aparecen los dos eventos del Estadio Rosario Arena (busca en la dirección del local) |
| 5.24 | Tildar la fecha, elegir un día posterior al Clásico → Buscar | Queda solo el Festival |
| 5.25 | «Limpiar» | Vuelven los dos eventos; caja vacía, combo en «Todas», fecha destildada |
| 5.26 | Tabular desde la caja de búsqueda | Caja → combo → fecha → Buscar → Limpiar |

## Sección 6 — commit «Seccion 6: fidelizacion y descuento por nivel»

Los cálculos (RN-19 a RN-24, RN-37, CP-03, CP-05, CP-06, CP-09, CP-10 y la referencia de
$ 99.625) se verificaron con los gestores sobre una base temporal. Lo que sigue es lo que
queda por mirar en la interfaz real. Entrar como `plata@eventaura.com` salvo que se indique.
**Reemplaza a 5.4 y 5.7**: el encabezado de nivel cambió de texto y ya no hay «total parcial».

| # | Qué probar | Resultado esperado |
|---|---|---|
| 6.1 | Encabezado de nivel | «Tu nivel: Plata · 5,0 % de descuento» y a continuación, en la misma línea y sin pisarse, el enlace «¿Cómo funcionan los niveles?» |
| 6.2 | Clic en «¿Cómo funcionan los niveles?» (H-05) | Ventana con: ventana de 12 meses; Bronce 0 a 2 compras 0,0 %; Plata 3 a 6 compras 5,0 %; Oro 7 o más 7,5 %; aclaración de que el descuento es solo sobre entradas |
| 6.3 | 2 Campo + Hospedaje (referencia) | Resumen: Base entradas $ 50.000,00 · Desc. Plata 5,0 % -$ 2.500,00 · Subtotal entradas $ 47.500,00 · Subtotal servicios $ 45.000,00 · Cargo por servicio $ 7.125,00 · **TOTAL $ 99.625,00** |
| 6.4 | Leyenda H-01 | Debajo del TOTAL: «El descuento se aplica sobre el valor de las entradas.» |
| 6.5 | Repetir 6.3 con `bronce@eventaura.com` | Desc. Bronce 0,0 % $ 0,00; cargo $ 7.500,00; TOTAL $ 102.500,00 |
| 6.6 | Repetir con `oro@eventaura.com`: 1 Platea + Hospedaje (CP-05 con la semilla) | Desc. Oro 7,5 % -$ 3.000,00; Hospedaje $ 45.000,00 sin descuento; TOTAL $ 87.550,00 |
| 6.7 | Paquete máximo: 2 VIP + 2 Platea + 2 Campo, los 4 servicios y seguro | Todo el resumen entra en el panel, sin líneas cortadas; «Meet & Greet» se ve con el «&» |
| 6.8 | Evento de nombre largo (el título está en `AutoSize`) | Verificar si el título invade el panel del resumen y tapa «RESUMEN DEL PAQUETE». **Viene de la Sección 5**: en la captura de prueba se superponía, pero `DrawToBitmap` no respeta el orden de capas, así que hay que mirarlo en pantalla |
| 6.9 | RN-23 en la base de desarrollo: poner `FechaUltimaEvaluacion` del mes pasado a `plata@` y `Nivel = 'Bronce'`, abrir la compra | El encabezado ya dice Plata y la base quedó con `Nivel = 'Plata'` y la fecha de hoy |
| 6.10 | Igual que 6.9 pero con la fecha de este mes | El encabezado sigue diciendo Bronce: entre reevaluaciones el nivel no cambia |

## Sección 7 — commit «Seccion 7: reserva, pago, confirmacion y ticket»

Antes de probar, aplicar a la base de desarrollo los tres cambios de esquema (están en
`BaseDatos/01-esquema.sql`, pero el script no se puede volver a correr entero):

```sql
ALTER TABLE Compras ADD MontoSeguro DECIMAL(12,2) NOT NULL DEFAULT 0;
GO
ALTER TABLE Compras DROP CONSTRAINT CK_Compras_Estado;
ALTER TABLE Compras ADD CONSTRAINT CK_Compras_Estado
      CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida'));
GO
CREATE TABLE Pagos (
    ID          INT IDENTITY(1,1) PRIMARY KEY,
    ID_Compra   INT          NOT NULL,
    FechaPago   DATETIME     NOT NULL,
    Estado      VARCHAR(10)  NOT NULL,
    Ultimos4    CHAR(4)      NOT NULL,
    Token       VARCHAR(64)  NOT NULL,
    Motivo      VARCHAR(80)  NULL,
    CONSTRAINT FK_Pagos_Compras FOREIGN KEY (ID_Compra) REFERENCES Compras(ID),
    CONSTRAINT CK_Pagos_Estado CHECK (Estado IN ('Aprobado','Rechazado')),
    CONSTRAINT CK_Pagos_Ultimos4 CHECK (Ultimos4 LIKE '[0-9][0-9][0-9][0-9]')
);
CREATE INDEX IX_Pagos_Compra ON Pagos (ID_Compra);
GO
```

Los gestores se verificaron sobre una base temporal (2a a 2h y CP-07, CP-08). El recorrido
de pantalla también se hizo desde una prueba automática, con capturas; lo que sigue es lo
que falta mirar a mano. Entrar como `plata@eventaura.com`. Tarjetas de prueba:
`4111 1111 1111 1111` se aprueba; cualquiera terminada en `0000` se rechaza.
**Reemplaza a 5.15**: ahora se graba una fila de `Entradas` por entrada, no por sector.

| # | Qué probar | Resultado esperado |
|---|---|---|
| 7.1 | 2 Campo + Hospedaje, sin seguro → «Comprar» (H-04) | Pregunta «¿Continuar sin seguro?» con «No» por defecto. «No» se queda en la pantalla sin reservar nada |
| 7.2 | Marcar el seguro | El resumen muestra «Seguro (2 entradas) $ 10.000,00» y el TOTAL sube a $ 109.625,00. «Comprar» ya no pregunta |
| 7.3 | Pantalla de pago (H-02) | «Reserva: 09:59» arriba a la derecha, bajando de a un segundo; se pone rojo en el último minuto. Total a pagar igual al del resumen |
| 7.4 | Tabular en el pago | Titular → Número → Vencimiento → Código → Pagar → Cancelar. Enter equivale a «Pagar». El código se ve con puntos |
| 7.5 | Datos mal cargados: número de 10 dígitos, vencimiento `13/30` o vencido, código de 2 dígitos | Mensaje en rojo debajo de los campos. No se registra ningún intento en `Pagos` |
| 7.6 | Tarjeta `4111 1111 1111 0000` | «Pago rechazado: Rechazada por el emisor…» en rojo; se borra el número. En `Compras` la fila sigue Pendiente con `ReservaVence` NULL; en `Pagos`, una fila Rechazado |
| 7.7 | Reintentar con `4111 1111 1111 1111` | Se aprueba. `Pagos` queda con dos filas para la compra: Rechazado y Aprobado |
| 7.8 | Confirmación (RF-07) | «¡Compra confirmada!», «Pedido EA-000NNN», desglose igual al resumen, «Tarjeta •••• 1111», token, la carpeta y un PDF por entrada |
| 7.9 | «Abrir carpeta» | Abre `Documentos\A1.Tickets EVENTAURA` en el Explorador |
| 7.10 | Abrir los PDF | Uno por entrada, con pedido, sector, precio, fecha y hora del evento y su código. Escanear los QR con el celular: cada uno muestra su código `EA-…` y son todos distintos |
| 7.11 | «Aceptar» en la confirmación | Vuelve a la compra vacía; Disponibles de Campo bajó 2 |
| 7.12 | Iniciar un pago y «Cancelar pago» → «Sí» (o cerrar con la X) | Vuelve a la compra con lo armado intacto y Disponibles vuelve al valor anterior |
| 7.13 | Dejar vencer la reserva (más rápido: `UPDATE Parametros SET Valor = '1' WHERE Clave = 'RESERVA_MINUTOS'` y reiniciar la aplicación) | Al llegar a 00:00 avisa, vuelve al listado y la compra queda `Vencida`, con sus entradas `Vencida`. **Volver a poner `10` después** |
| 7.14 | CP-08 en pantalla: `plata@` tiene 3 compras; hacer 4 seguidas | En la 4.ª el resumen todavía dice «Desc. Plata 5,0 %». Al aceptar la confirmación, el encabezado ya dice «Tu nivel: Oro · 7,5 %» |
| 7.15 | Dos sesiones a la vez sobre un sector chico (capacidad 2): en una, iniciar el pago de 2; en la otra, abrir la compra del mismo evento | La segunda muestra Disponibles 0 y rechaza pedir 1 (RN-08). Si la primera cancela, al reabrir la compra vuelven a aparecer |
| 7.16 | Iniciar un pago de un evento nuevo, cancelarlo, y después eliminar el evento como propietario | Lo elimina: una reserva abandonada no es una venta |
| 7.17 | Eliminar el último sector con capacidad de un evento publicado que tiene una reserva vencida | Sigue dando el mensaje de RN-06, que es el motivo correcto |

Limitaciones conocidas, a resolver en la Sección 8 (no son fallas de esta sección):

- `frmHistorialCliente` lista todas las filas de `Entradas`, sin mirar el estado: muestra
  también las reservas Pendiente y Vencida, y una fila por entrada.
- La semilla todavía graba las entradas del Clásico Rosarino de a 2 por fila.

Las dos quedan resueltas en la Sección 8 (ver abajo).

## Sección 8 — commit «Seccion 8: suscripciones, seguro, historial y lista de espera»

Antes de probar, aplicar a la base de desarrollo los tres cambios de esquema y los tres
parámetros nuevos (están en `BaseDatos/01-esquema.sql` y `02-semilla.sql`, pero los scripts
no se pueden volver a correr enteros):

```sql
ALTER TABLE Eventos     ADD FinPreventa      DATETIME      NULL;
ALTER TABLE ListaEspera ADD ConSuscripcion   BIT           NOT NULL DEFAULT 0;
ALTER TABLE Compras     ADD MontoReembolsado DECIMAL(12,2) NOT NULL DEFAULT 0;
GO
INSERT INTO Parametros (Clave, Valor, Descripcion, Regla) VALUES
('SUSCRIPCION_MENSUAL_PRECIO',   '12000', 'Precio del plan mensual de suscripción, en pesos',          'RF-13'),
('SUSCRIPCION_ANUAL_PRECIO',     '75000', 'Precio del plan anual de suscripción, en pesos',            'RF-13'),
('SUSCRIPCION_SEGURO_DESCUENTO', '1.000', 'Bonificación del seguro para suscriptores (1 = sin cargo)', 'RN-37');
GO
```

Los gestores se verificaron sobre una base temporal recreada con los dos scripts (2a a 2l:
RN-25, RN-26, seguro sin cargo, referencia de $ 99.625, CP-10, RN-39, RN-35/36, RN-10,
CP-09, devolución parcial, RN-29 y el cierre de todas las filas de `Compras`). Las
pantallas nuevas se abrieron desde una prueba automática con capturas, sin hacer clics.
Lo que sigue es lo que falta mirar a mano.

**Reemplaza a las pruebas del historial de secciones anteriores** y a las dos limitaciones
de la Sección 7: el historial ya no lee `Entradas` con SQL propio, muestra una fila por
compra y solo las Confirmada y Reembolsada; la semilla graba el Clásico de a una entrada por
fila. En una base de desarrollo cargada con la semilla vieja, las 3 filas del Clásico
siguen siendo de a 2 entradas: el detalle de esa compra muestra 3 filas con el importe de 2.

**Formularios agrandados** (declarado): `frmMenuCliente` de 595 a 723 de alto,
`frmMenuPropietario` de 714 a 1020, con «Volver al inicio» bajado lo mismo.
`frmHistorialCliente` se rehízo: de 412 a 560 de alto, con dos grillas en lugar de una.
`FrmCompraCliente` suma dos controles ocultos (aviso de agotado y botón de lista de espera)
sobre la grilla de sectores.

| # | Qué probar | Resultado esperado |
|---|---|---|
| 8.1 | Menú del cliente | Tres botones iguales: «Eventos disponibles», «Mis eventos», «Mi suscripción»; «Volver al inicio» debajo, sin pisarse. Tabular recorre los tres botones en orden |
| 8.2 | Menú del propietario (`admin@`) | «Mis locales», «Registrar local», «Listas de espera» y un hueco del alto de un botón antes de «Volver al inicio» (lugar de «Reportes», Sección 9) |
| 8.3 | «Mi suscripción» con `plata@` | «No tenés una suscripción activa.», los tres beneficios, «Sin descuento extra: rige el de tu nivel.», opciones «Mensual: $ 12.000 por mes» y «Anual: $ 75.000 por año», «Cancelar suscripción» deshabilitado. Nada cortado |
| 8.4 | Contratar el Mensual | Pide confirmación con el precio y el aviso de que no se renueva. Al aceptar: «Plan Mensual activo / Vence el (hoy + 1 mes) / No se renueva automáticamente»; las opciones y «Contratar» quedan deshabilitados y «Cancelar suscripción» habilitado |
| 8.5 | RN-25 en pantalla: abrir la aplicación dos veces con `plata@`, abrir «Mi suscripción» en las dos (sin plan), contratar en una y después en la otra | La segunda muestra el mensaje de RN-25 con el vencimiento del plan vigente. En `Suscripciones` hay una sola fila con `Activa = 1` |
| 8.6 | «Cancelar suscripción» | Pide confirmación con «No» por defecto. Al aceptar vuelve a «No tenés una suscripción activa.» |
| 8.7 | Seguro sin cargo: con `plata@` suscripta, Festival, 2 Campo + seguro | El resumen dice «Seguro sin cargo (2 entradas) $ 0,00» y el TOTAL no sube. «Comprar» no pregunta por el seguro (H-04 sigue igual) |
| 8.8 | Pagar esa compra | La confirmación muestra «Seguro $ 0,00». En `Seguros` hay 2 pólizas con `Precio` 0 |
| 8.9 | La misma compra sin suscripción (cancelarla antes) | «Seguro (2 entradas) $ 10.000,00», como en 7.2 |
| 8.10 | Historial (`plata@`, «Mis eventos») | Una fila por compra: Pedido, Evento, Fecha, Estado, Entradas, Total pagado, Devuelto. Las 3 compras de la semilla aparecen con Evento «—» y 0 entradas. No aparecen compras Pendiente ni Vencida (7.13 deja una Vencida para comparar) |
| 8.11 | Elegir la compra de 8.8 | Abajo, «Entradas del pedido EA-…»: 2 filas con sector, precio, Estado Confirmada, Seguro Sí y el código. El botón dice «Devolver» y está deshabilitado |
| 8.12 | Tildar una entrada | El botón pasa a «Devolver 1 entrada(s)». Destildar la vuelve a deshabilitar |
| 8.13 | Devolver 1 de 2 | Confirmación con «No» por defecto; al aceptar, «Se reintegran …» con el precio menos el descuento del nivel con que se compró (con Plata, $ 23.750,00). La compra sigue Confirmada con ese importe en Devuelto; la entrada queda Reembolsada, en gris y sin poder tildarse |
| 8.14 | Volver a la compra del Festival | Disponibles de Campo subió 1 respecto de antes de devolver |
| 8.15 | Devolver la otra entrada | Se reintegra el resto (más los servicios, si la compra tenía); la compra pasa a Reembolsada |
| 8.16 | Compra sin seguro: tildar una entrada | No se puede tildar: la fila está en gris |
| 8.17 | RN-29 por plazo: `UPDATE Eventos SET Fecha = DATEADD(HOUR, 24, GETDATE())` en el evento de una compra con seguro y tratar de devolver | Mensaje de RN-29 con la fecha límite. **Restaurar la fecha después** |
| 8.18 | RN-10: con `plata@`, Clásico Rosarino | En lugar de la grilla: «Entradas agotadas. Anotate en la lista de espera…» y el botón «Anotarme en la lista de espera». Servicios, seguro y «Comprar» deshabilitados |
| 8.19 | Anotarse | «Te anotaste en la lista de espera…»; el botón pasa a «Ya estás en la lista de espera» y queda deshabilitado (RF-10) |
| 8.20 | Volver al listado y reabrir el Clásico | El botón ya aparece como «Ya estás en la lista de espera» |
| 8.21 | RN-39: `UPDATE Eventos SET FinPreventa = DATEADD(DAY, 3, GETDATE())` en el Festival; abrirlo con un cliente sin suscripción | El evento aparece en el listado; al «Comprar», mensaje de RN-39 con la fecha y hora de fin y vuelve al listado |
| 8.22 | Lo mismo con un cliente suscripto | Abre la compra normal. **Volver a poner `FinPreventa = NULL`** |
| 8.23 | Listas de espera como `admin@` | El combo trae el Clásico («… · agotado»). La grilla: Posición, Cliente, Mail, Suscriptor, Inscripto el; los suscriptores arriba; debajo, la cantidad de inscriptos y cuántos con suscripción. Las columnas no se reordenan con clic |
| 8.24 | RN-35 congelada: anotar a `bronce@` sin suscripción y a `oro@` con suscripción; después cancelar la de `oro@` | `oro@` sigue primero aunque ya no sea suscriptor |
| 8.25 | Sin eventos agotados (otro propietario) | Combo deshabilitado y el aviso «No hay eventos agotados ni listas de espera en sus locales.» |
| 8.26 | Login y navegación completa (inicio → menús → cada pantalla → volver) | Todo funciona igual que antes sin `ConexionSql`: ningún formulario recibe ni abre una conexión |

---

## Riesgo asumido

Al diferir las pruebas, un fallo de interfaz introducido en una sección no se detecta
hasta el final. Mitigación: cada sección está en su propio commit, de modo que
`git bisect` o un `checkout` por commit permiten aislar dónde se rompió.

Secciones que más dependen de que la anterior funcione: **5 → 6 → 7**. Son el flujo
de compra completo y se construyen una sobre otra.
