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

---

## Riesgo asumido

Al diferir las pruebas, un fallo de interfaz introducido en una sección no se detecta
hasta el final. Mitigación: cada sección está en su propio commit, de modo que
`git bisect` o un `checkout` por commit permiten aislar dónde se rompió.

Secciones que más dependen de que la anterior funcione: **5 → 6 → 7**. Son el flujo
de compra completo y se construyen una sobre otra.
