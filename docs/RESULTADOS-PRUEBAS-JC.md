# Resultados de las pruebas manuales — Parte de JC

> Corrida inicial de Claude, conservada como historial. Las correcciones posteriores,
> pruebas nuevas y pendientes de reverificación están en
> [Correcciones de JC — 03/11/2026](CORRECCIONES-JC-2026-11-03.md).
> Última verificación: [04/11/2026](VERIFICACION-JC-2026-11-04.md), 31/31 pruebas automáticas;
> no reemplaza ni altera retrospectivamente la corrida inicial.

Corrida de la parte de JC de [PRUEBAS-PENDIENTES.md](PRUEBAS-PENDIENTES.md) (recorrido del
cliente, 65 pruebas). Cada fila dice qué se vio y, si falló, remite a la lista de fallos del
final, que es lo que queda para corregir **después** de terminar todas las pruebas.

**Entorno:** Windows 11 Pro, pantalla 1920 × 1080 con escala **100 %** (96 DPI), SQL Server
2022 Express (`.\SQLEXPRESS`), VS 2022 Build Tools, base `BddEventAura` creada de cero con
`01-esquema.sql` + `02-semilla.sql` (sin `03-datos-reporte.sql`). Commit probado: `9e9bbbe`.
Fecha de la corrida: 03/11/2026. Antes de empezar, la batería automática (`Pruebas/`) dio
**12 de 12 correctas** sobre esta instancia.

**Cómo se probó:** la aplicación se manejó con UI Automation desde PowerShell (clics, texto,
teclado real, lectura de MessageBox y capturas de cada pantalla), y la base se verificó con
`sqlcmd`. Lo que necesita un celular (escanear los QR) queda para JC.

**Ojo con la escala:** casi todos los formularios declaran `AutoScaleDimensions` 120F (se
diseñaron al 125 %). Los fallos F-01 a F-03 se ven a 100 %; es probable que a 125 % no
aparezcan, pero RNF-03 pide que la aplicación funcione en las dos.

Estados: ✅ pasa · ❌ falla (ver fallo) · ⚠️ pasa con observación · ⏳ pendiente · 🚫 no se pudo probar

---

## H. Registro — Sección 3

| # | Estado | Resultado observado |
|---|---|---|
| 3.4 | ✅ | «Crear Cuenta» abre «Crear usuario» (`frmCrearUsuario`). Por dentro el formulario se llama `frmAgregarEvento` (O-02), no se ve |
| 3.5 | ⚠️ | Con `abc`: «No se pudo crear la cuenta — La contraseña debe tener al menos 8 caracteres, con al menos una letra y un dígito.» No se creó el usuario. Nombre, Apellido y las dos contraseñas aparecen precargados con «A» (O-01) |

## I. Búsqueda y armado de paquete — Sección 5

| # | Estado | Resultado observado |
|---|---|---|
| 5.1 | ⚠️ | Aparecen Clásico y Festival; no la Noche de Teatro. Columnas Nombre, Fecha, Categoria, Ubicacion, Complejo: sin tilde, y Complejo queda cortada con barra horizontal (O-05) |
| 5.2 | ✅ | Abre la compra del Festival, con su nombre de título |
| 5.3 | ❌ | Grilla Sector/Precio/Disponibles/Cantidad, checks y resumen a la derecha, pero: cada importe del resumen baja a la línea siguiente (F-01), el título y el enlace de niveles quedan tapados por el panel (F-02) y «Merchandising $» sale sin el precio (F-03) |
| 5.5 | ✅ | VIP 100, Platea 800, Campo 2000 |
| 5.6 | ✅ | «2 x Campo $ 50.000,00» |
| 5.8 | ✅ | «RN-12 — Se pueden comprar como máximo 6 entradas por operación. Ya tiene 2 en otros sectores.» y la celda vuelve a 0 |
| 5.9 | ✅ | «abc» y «-1»: «La cantidad debe ser un número entero.» y vuelve al valor anterior. Para «-1» el texto no es exacto (O-08) |
| 5.10 | ✅ | Sector «Chico» de capacidad 2, pedir 3: «RN-08 — En el sector Chico quedan 2 entradas.» |
| 5.11 | ⚠️ | Clásico: los cuatro checks deshabilitados con «(no disponible)», pero tres se ven cortados como «Hospedaje (no» (F-03) |
| 5.12 | ✅ | Solo Hospedaje: «RN-13 — El paquete debe tener al menos una entrada…». No compra |
| 5.13 | ⚠️ | Pregunta «Si vuelve al listado se descarta el paquete que armó. ¿Desea volver?»; «No» se queda, «Sí» vuelve. El botón por defecto es «Sí» (O-07) |
| 5.14 | ✅ | Con el paquete vacío vuelve sin preguntar |
| 5.16 | ✅ | Evento de prueba con 4 sectores (Chico, Campo, Platea, VIP): los cuatro se guardan y el evento se publica |

## J. Filtros de búsqueda — Sección 5

| # | Estado | Resultado observado |
|---|---|---|
| 5.17 | ⚠️ | Caja, combo, fecha con casilla, Buscar y Limpiar alineados sobre la grilla, sin tapar el título. La guía «Evento o dirección del local» solo aparece cuando la caja **no** tiene el foco, y el foco arranca ahí: al abrir no se ve (O-04) |
| 5.18 | ✅ | Foco en la caja de búsqueda, combo «Todas», fecha destildada |
| 5.19 | ✅ | «Rock» + Enter deja solo el Festival. El «ding» no se pudo oír; el código lo suprime (`SuppressKeyPress`) |
| 5.20 | ✅ | «Deportivo» deja el Clásico |
| 5.21 | ⚠️ | «Teatro»: «No hay eventos que coincidan con la búsqueda…». Debajo queda una fila vacía editable con «*» (O-06) |
| 5.22 | ⚠️ | Pide seleccionar un evento y no abre la compra, pero el MessageBox se titula **«Hola Pedro»** (O-03) |
| 5.23 | ✅ | «Pellegrini» trae los dos eventos del Estadio Rosario Arena |
| 5.24 | ✅ | Fecha 10/12/2026 tildada: queda solo el Festival |
| 5.25 | ✅ | Vuelven los dos; caja vacía, combo «Todas», fecha destildada |
| 5.26 | ✅ | Caja → combo → fecha → Buscar → Limpiar |

## K. Fidelización y descuento — Sección 6

| # | Estado | Resultado observado |
|---|---|---|
| 6.1 | ❌ | El texto es «Tu nivel: Plata · 5,0 % de descuento» y el enlace va en la misma línea, pero el enlace queda tapado por el panel del resumen: se lee «¿Cómo funcionan los» (F-02) |
| 6.2 | ✅ | Ventana con los 12 meses, Bronce 0–2 0,0 %, Plata 3–6 5,0 %, Oro 7+ 7,5 % y la aclaración de que el descuento es solo sobre entradas |
| 6.3 | ⚠️ | Importes correctos: Base $ 50.000,00 · Desc. Plata −$ 2.500,00 · Subtotal $ 47.500,00 · Servicios $ 45.000,00 · Cargo $ 7.125,00 · **TOTAL $ 99.625,00**. En pantalla cada importe cae a la línea siguiente (F-01) |
| 6.4 | ❌ | La leyenda «El descuento se aplica sobre el valor de las entradas.» existe pero queda fuera del panel, cortada (F-01) |
| 6.5 | ✅ | `bronce@`: Desc. Bronce 0,0 % $ 0,00; cargo $ 7.500,00; TOTAL $ 102.500,00 |
| 6.6 | ✅ | `oro@`, 1 Platea + Hospedaje: Desc. Oro 7,5 % −$ 3.000,00; Hospedaje $ 45.000,00; TOTAL $ 87.550,00 |
| 6.7 | ❌ | 2+2+2 con los 4 servicios y seguro: los números son correctos y en pantalla «Meet & Greet» se ve con el «&», pero el resumen no entra: se corta en «Meet & Greet» y no se ven el TOTAL ni la leyenda (F-01) |
| 6.8 | ❌ | Con un nombre largo el título pasa por detrás del panel y asoma a la derecha; no se puede leer. «RESUMEN DEL PAQUETE» no queda tapado porque el panel está adelante (F-02) |
| 6.9 | ✅ | Hecha con `oro@` (7 compras en la semilla) porque `plata@` ya había subido a Oro en 7.14: `Nivel = 'Bronce'` y fecha del mes pasado → el encabezado dice Oro y la base quedó con Oro y la fecha de hoy |
| 6.10 | ✅ | `Nivel = 'Bronce'` con fecha de este mes → el encabezado sigue en Bronce. Se restauró a Oro |

## L. Reserva, pago, confirmación y ticket — Sección 7

| # | Estado | Resultado observado |
|---|---|---|
| 7.1 | ✅ | «¿Continuar sin seguro?» con «No» por defecto; «No» se queda y no reserva nada |
| 7.2 | ✅ | «Seguro (2 entradas) $ 10.000,00», TOTAL $ 109.625,00; «Comprar» ya no pregunta |
| 7.3 | ✅ | «Reserva: 09:59» baja de a un segundo; en el último minuto se pone rojo (probado con 1 minuto). Total a pagar $ 109.625,00 |
| 7.4 | ✅ | Titular → Número → Vencimiento → Código → Pagar → Cancelar; Enter paga; el código se ve con puntos |
| 7.5 | ✅ | 10 dígitos, `13/30`, `01/24` y código de 2: mensaje en rojo para cada uno; `Pagos` sin filas |
| 7.6 | ✅ | «Pago rechazado: Rechazada por el emisor…» en rojo, número borrado; compra Pendiente con `ReservaVence` NULL y una fila Rechazado en `Pagos` |
| 7.7 | ✅ | Aprobado; `Pagos` con Rechazado y Aprobado para la compra 11 |
| 7.8 | ✅ | «¡Compra confirmada!», «Pedido EA-000011», desglose igual al resumen, «Tarjeta •••• 1111», token, carpeta y 2 PDF |
| 7.9 | ✅ | Abre `Documentos\A1.Tickets EVENTAURA` en el Explorador |
| 7.10 | ⏳ | Un PDF por entrada con evento, sector, precio, fecha y hora, pedido y código; los dos códigos son distintos. **Falta escanear los QR con el celular** |
| 7.11 | ✅ | Vuelve a la compra vacía; Campo pasó de 2000 a 1998 |
| 7.12 | ✅ | «Cancelar pago» → «Sí»: vuelve con 1 Campo + Traslado intactos y Disponibles en 1998 |
| 7.13 | ✅ | Con `RESERVA_MINUTOS = 1`: «Reserva vencida…», vuelve al listado; compra y entrada `Vencida`. Se volvió a poner 10 |
| 7.14 | ✅ | `plata@` tenía 4 (3 de la semilla + 7.7); en la 7.ª el resumen dice «Desc. Plata 5,0 %» y al aceptar el encabezado pasa a «Tu nivel: Oro · 7,5 %» |
| 7.15 | ✅ | Dos instancias: A reserva los 2 de «Chico»; B ve Disponibles 0 y pedir 1 da RN-08. A cancela y B, al reabrir, ve 2 |
| 7.16 | ⚠️ | Evento nuevo con una reserva cancelada: el propietario lo elimina. Lo borra **sin pedir confirmación** (O-10) |
| 7.17 | ✅ | Evento con un solo sector y una reserva vencida: «RN-06 — No se puede eliminar el sector: es el último con capacidad mayor que cero de un evento publicado.» |
| 10.5 | ⏳ | Igual que 7.10: el PDF está bien; falta escanear los QR |

## M. Menú, historial, devolución y lista de espera — Sección 8

| # | Estado | Resultado observado |
|---|---|---|
| 8.1 | ✅ | Tres botones iguales y «Volver al inicio» debajo; Tab recorre los tres en orden |
| 8.10 | ✅ | Una fila por compra con las 7 columnas; las 3 de la semilla con «—» y 0 entradas; no aparecen las Pendiente ni Vencida (12, 13) |
| 8.11 | ⚠️ | EA-000011: 2 filas Campo $ 25.000,00, Confirmada, Seguro Sí y código; «Devolver» deshabilitado, aunque se ve igual de violeta que habilitado (O-09) |
| 8.12 | ✅ | Tildar → «Devolver 1 entrada(s)»; destildar → deshabilitado |
| 8.13 | ✅ | Confirmación con «No» por defecto; «Se reintegran $ 23.750,00»; compra Confirmada con $ 23.750,00 devueltos; entrada Reembolsada en gris y sin poder tildarse |
| 8.14 | ✅ | Campo pasó de 1995 a 1996 |
| 8.15 | ✅ | «Se reintegran $ 68.750,00» (entrada + Hospedaje); la compra pasa a Reembolsada con $ 92.500,00 devueltos |
| 8.16 | ✅ | En una compra sin seguro (EA-000016) la fila está en gris y no se tilda |
| 8.17 | ✅ | Compra con seguro y el evento a 24 h: «RN-29 — El seguro permite devolver hasta 48 horas antes del evento (hasta el 02/11/2026 17:55). Ese plazo ya pasó.» Se restauró la fecha |
| 8.18 | ✅ | Clásico: «Entradas agotadas. Anotate en la lista de espera…» y el botón; servicios, seguro y «Comprar» deshabilitados |
| 8.19 | ✅ | «Te anotaste en la lista de espera de «Clásico Rosarino»…»; el botón pasa a «Ya estás en la lista de espera», deshabilitado |
| 8.20 | ✅ | Al reabrir el Clásico, el botón ya dice «Ya estás en la lista de espera» |

---

## Resumen

| | Cantidad |
|---|---|
| ✅ Pasan | 48 |
| ⚠️ Pasan con observación | 10 |
| ❌ Fallan | 5 |
| ⏳ Pendientes (escanear QR con el celular) | 2 |
| 🚫 No se pudieron probar | 0 |

Los cinco fallos son de presentación en `FrmCompraCliente` con la pantalla al 100 %: los
cálculos, las reglas y lo que se graba en la base dieron bien en todas las pruebas.

---

## Fallos y observaciones para corregir después

Numerados `F-NN` (fallo: no cumple el resultado esperado) y `O-NN` (observación: cumple, pero
hay algo que conviene arreglar). No se corrigió nada durante la corrida.

| ID | Prueba | Qué pasa | Dónde está | Prioridad |
|---|---|---|---|---|
| F-01 | 5.3, 6.3, 6.4, 6.7 | El resumen del paquete no entra en su panel a 100 %: cada importe baja a la línea siguiente y, con un paquete grande, el TOTAL y la leyenda H-01 quedan fuera de la vista | `FrmCompraCliente.cs:458` (`AnchoLinea = 38`, Consolas 9 pt) dentro de `lblResumen` de 348 px diseñado a 120 DPI | Alta (RNF-03) |
| F-02 | 5.3, 6.1, 6.8 | El título del evento (`label1`, 22 pt, `AutoSize`) y el enlace «¿Cómo funcionan los niveles?» quedan detrás de `pnlResumen`: «Festival de Rock Ros…», «¿Cómo funcionan los». Con un nombre largo el título asoma del otro lado del panel | `FrmCompraCliente.Designer.cs` (`label1`, `lnkNiveles`, `pnlResumen`) y `FrmCompraCliente.cs:224` | Alta |
| F-03 | 5.3, 5.11 | Los checks de servicios cortan su texto: «Merchandising $» sin el precio, y «Hospedaje (no», «Traslado (no», «Merchandising (no» en un evento agotado | `FrmCompraCliente.Designer.cs` (`chkMerch` y los demás, 175 × 23 fijos) | Media |
| O-01 | 3.5 | El alta de usuario abre con «A» en Nombre, Apellido, Contraseña y Confirmar contraseña (restos de depuración) | `frmCrearUsuario.cs:23-26` | Media |
| O-02 | 3.4 | `frmCrearUsuario` tiene `Name = "frmAgregarEvento"` (copiado de otro formulario). No se ve en pantalla | `frmCrearUsuario.Designer.cs:244` | Baja |
| O-03 | 5.22 | El aviso «Debe seleccionar un evento para comprar» tiene de título «Hola Pedro» | `frmEventosCliente.cs`, `btnComprar_Click` | Media |
| O-04 | 5.17 | El texto guía se oculta mientras la caja tiene el foco, y el foco arranca en la caja: al abrir la pantalla no se ve. Con `wParam = 1` en `EM_SETCUEBANNER` se ve también con foco | `frmEventosCliente.cs`, `PonerTextoGuia` | Baja |
| O-05 | 5.1 | Encabezados «Categoria» y «Ubicacion» sin tilde; la columna Complejo queda cortada y aparece barra horizontal | `frmEventosCliente.cs`, `CargarEventos` | Baja |
| O-06 | 5.21 | Con el mensaje de «sin resultados», la grilla muestra una fila nueva editable («*») | `frmEventosCliente.cs` (`AllowUserToAddRows`) | Baja |
| O-07 | 5.13 | Al volver con un paquete armado el botón por defecto es «Sí», que descarta el paquete; en H-04 (7.1) el criterio es «No» por defecto | `FrmCompraCliente.cs`, `btnVolver_Click` | Baja |
| O-08 | 5.9 | Para «-1» el mensaje dice «La cantidad debe ser un número entero.», y -1 lo es; convendría «un número entero de 0 en adelante» | `FrmCompraCliente.cs`, `dgvSectores_CellValueChanged` | Baja |
| O-09 | 8.10, 8.11 | «Devolver» deshabilitado se ve igual (violeta) que habilitado | `frmHistorialCliente` (estilo del botón) | Baja |
| O-10 | 7.16 | «Eliminar evento» borra sin pedir confirmación (área del propietario) | `frmEventosLocal.cs:41` | Media |
| O-11 | general | Las barras de título muestran nombres internos: «frmEventos», «FrmCompra», «frmMenuUsuario», «frmHistorial», «frmInicio», «MenuVendedor» | `this.Text` en los `.Designer.cs` | Baja |

---

## Estado en que quedó la base de JC

Para que las pruebas de arriba se puedan repetir, se restauró lo que se tocó a propósito
(`RESERVA_MINUTOS = 10`, fecha del Festival, nivel de `oro@`). Quedan, como datos de prueba:
9 compras nuevas de `plata@` (4 confirmadas, 1 reembolsada y 4 vencidas; `plata@` subió a
**Oro**), el evento «Prueba JC 2» con un sector y una reserva vencida, `plata@` en la lista de
espera del Clásico, y la compra vencida 18, del evento de prueba que se eliminó en 7.16. Si hace falta
volver a la semilla limpia, recrear la base con `01-esquema.sql` + `02-semilla.sql`.
