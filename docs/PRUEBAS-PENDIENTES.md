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

---

## Riesgo asumido

Al diferir las pruebas, un fallo de interfaz introducido en una sección no se detecta
hasta el final. Mitigación: cada sección está en su propio commit, de modo que
`git bisect` o un `checkout` por commit permiten aislar dónde se rompió.

Secciones que más dependen de que la anterior funcione: **5 → 6 → 7**. Son el flujo
de compra completo y se construyen una sobre otra.
