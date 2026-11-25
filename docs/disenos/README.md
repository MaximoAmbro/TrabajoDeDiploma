# Diseños de las vistas web: entrada y armador de paquete

Claude Code, 24/11/2026, a pedido de JC. Son **maquetas funcionales** en HTML, CSS y JavaScript, sin red, sin
CDN, sin fuentes externas y sin archivos de imagen, para que Codex las integre con WebView2 como hizo con el
catálogo. No tocan `Visual/`, `Repo/` ni `Datos/`.

**Las cinco pantallas usan el mismo sistema.** Los valores están en `docs/PRINCIPIOS-DISENO.md` y su única
implementación es `sistema.css`. Una medida o un color que no esté ahí no se usa.

## Archivos

| Archivo | Qué es |
|---|---|
| `sistema.css` | El sistema: colores de `Visual/Tema.cs`, seis niveles tipográficos, escala de espacio, controles, estados y estructura de las pantallas de entrada |
| `muro.js` | El muro de portadas del panel oscuro, generado como `PortadaEvento` (PRINCIPIOS, §7) |
| `entrada-puente.js` | El puente con la pantalla de Windows Forms (mismo contrato que `Visual/Web/tema.js`) y el panel de la maqueta |
| `validaciones.js` | Validaciones de formato, copia exacta de las del negocio (tabla de abajo) |
| `bienvenida.html` + `.js` | Qué es EventAura y los dos caminos |
| `acceso.html` + `.js` | Inicio de sesión |
| `recuperar-clave.html` + `.js` | **Nueva.** Recuperación de la contraseña con código |
| `alta-cuenta.html` + `.js` + `.css` | Alta con la elección «comprar» u «organizar» |
| `armador-paquete.html` + `.js` + `.css` | **Nueva.** El armador de paquete: sectores, servicios, seguro y el total en vivo |

## Cómo verlas

Abrí cualquier `.html` con doble clic: funciona sin servidor. Abajo a la derecha aparece el panel
**«Maqueta · aplicación simulada»**. Ese panel:
- muestra cada mensaje que la página le mandaría a la aplicación, con las contraseñas y los códigos tapados;
- responde como lo haría la aplicación;
- muestra lo que en la realidad pasa **fuera** de EventAura, por ejemplo los códigos de la bandeja simulada;
- tiene botones para previsualizar estados (error del negocio, base no disponible, nivel Bronce, Plata u Oro,
  suscripción, sector agotado…).

Dentro de WebView2 el panel no existe: `entrada-puente.js` lo crea solo si no hay `window.chrome.webview`.

Anclas para revisar un estado sin hacer clics (solo en la maqueta):

| Ancla | Qué muestra |
|---|---|
| `alta-cuenta.html#datos-organizador` | El formulario del organizador, vacío |
| `alta-cuenta.html#demo-organizador` | El mismo formulario, completo y válido |
| `alta-cuenta.html#listo-organizador` | La pantalla final del organizador |
| `recuperar-clave.html#codigo` | El paso del código y la contraseña nueva |
| `recuperar-clave.html#listo` | El paso final |
| `armador-paquete.html#guion` | El paquete del guion: 2 Campo, hospedaje, traslado y seguro. Total: $ 121.625,00, el mismo que da la aplicación |

**Verificado el 24/11/2026:**
- `validaciones.js` contra 18 casos con Node, entre ellos los CUIT de la semilla de demostración;
- el armador calcula el total del guion ($ 121.625,00), igual que `GestorFidelizacion.CalcularDescuento` en
  el recorrido de la sección 3;
- las cinco pantallas dibujadas con Edge sin ventana:
  - las de entrada a 1100 × 720 y a 600 de ancho;
  - el armador a 1366 × 768.

**No probado todavía:** dentro de WebView2 y a 144 DPI. Eso lo hace Codex al integrarlas.

---

## Integración

### Dónde van y cómo se cargan

- Copiar los archivos a `Visual/Web/`, junto a `catalogo.html`. Los nombres no chocan con los existentes.
  `sistema.css` reemplaza, para estas pantallas, a `tema.css`.
- Cada `.html` trae la misma política de contenido que el catálogo: `script-src 'self'`, sin scripts en línea,
  e imágenes solo `self` y `data:`. No se carga nada de afuera: Segoe UI es del sistema; los íconos y las
  portadas son SVG y CSS.
- El tema llega por `eventAuraActualizar({ tema, datos })`, igual que en el catálogo. **`Tema.VariablesWeb()`
  tiene que mandar todos los colores del bloque `:root` de `sistema.css` con esos mismos nombres:**
  - `primario`, `primario-encima`, `primario-presionado` y `primario-suave`;
  - `texto`, `texto-secundario`, `borde-control`, `borde`, `fondo` y `superficie`;
  - los cuatro `lateral-*`;
  - los seis de estado;
  - `portada-0` a `portada-5`.

  Hoy manda solo una parte. Las medidas y la tipografía no viajan: son del sistema. Mientras un color no
  llegue, la página usa el valor por omisión, que es el de `Tema.cs`.
- **Ventana:**
  - las pantallas de entrada están pensadas para unos 1100 × 720 lógicos; con menos de 880 px de ancho
    pasan a una columna;
  - el armador, para el área de contenido de la navegación del cliente.

  Si el contenido no entra, se desplaza dentro de la página y la ventana no pasa de 790 px de alto
  (`CLAUDE.md`).

### El respaldo de Windows Forms no se pierde

Si WebView2 no está o falla, `frmInicio` y `frmCrearUsuario` siguen siendo las pantallas, como hoy. La
bienvenida no tiene equivalente en Windows Forms: sin vista web, la aplicación abre directo en `frmInicio`.
Para el alta del organizador sin vista web, el camino es el actual: crear la cuenta y después «Solicitar ser
Organizador». Lo aprendido en T-03 aplica igual: nada de tipos de WebView2 ni de Newtonsoft fuera del `try`.

### Lo que no hay que perder al integrar

- **No revelar qué cuentas existen.** En el acceso, todo error de credenciales es «Mail o contraseña
  incorrectos.». En el alta, registrar un correo que ya existe pasa igual al paso «Verificá tu correo», con el
  mismo texto: `GestorUsuarios.RegistrarCliente` ya lo hace así.
- **La ruta de la bandeja simulada no se muestra** en ninguna pantalla. El texto es el actual: «abrí fuera de
  EventAura el correo simulado de esa dirección». La ruta va en el guion del operador.
- **Contraseñas y códigos** viajan solo dentro del mensaje que los usa, por `postMessage`, que no sale del
  proceso. La aplicación no los guarda en el registro ni en la auditoría.
- **La documentación** llega como el archivo original en base64. La aplicación la pasa por
  `ImagenEventoCodec.Preparar`, que acepta hasta 10 MB y la reduce a 512 KB o menos, y después el negocio
  valida RN-52 como hoy.

---

## Contrato de cada pantalla

Las dos direcciones:
- **Aplicación → página:** `eventAuraActualizar({ tema, datos })`, que devuelve `true`.
- **Página → aplicación:** `window.chrome.webview.postMessage(mensaje)`.

En las tres pantallas, Escape manda `{ "tipo": "escape" }`.

### `bienvenida.html`

| `datos` | Para qué |
|---|---|
| `{}` | Estado normal |
| `{ "aviso": "…", "bloquear": true }` | La base no responde: muestra el mismo aviso de T-20 y deshabilita los dos caminos |

| Mensaje | Qué hace la aplicación |
|---|---|
| `{ "tipo": "navegar", "destino": "acceso" }` | Abre el acceso |
| `{ "tipo": "navegar", "destino": "alta" }` | Abre el alta |

**Al abrir**, la aplicación corre primero la verificación de esquema de `frmInicio` (T-20 y T-21). Si falla,
manda el aviso con `bloquear: true`. Si la base está desactualizada, lo más simple es saltar la bienvenida y
mostrar el aviso de `frmInicio`, como hoy.

### `acceso.html`

| `datos` | Para qué |
|---|---|
| `{ "estado": "listo", "mail": "…" }` | Estado normal. `mail` es opcional y sirve para precargar, por ejemplo al volver del alta |
| `{ "estado": "error", "mensaje": "Mail o contraseña incorrectos." }` | Credenciales rechazadas: la página borra la contraseña y vuelve a ese campo |
| `{ "estado": "sinBase", "mensaje": "EventAura no pudo comprobar la base de datos. …" }` | Oculta el formulario y muestra el aviso, igual que T-20 |

| Mensaje | Qué hace la aplicación |
|---|---|
| `{ "tipo": "iniciarSesion", "mail": "…", "contrasena": "…" }` | `GestorUsuarios.Autenticar`. Si entra, abre el menú del perfil como hoy. Si no, devuelve `estado: "error"` con `GestorUsuarios.MensajeAccesoRechazado` |
| `{ "tipo": "navegar", "destino": "recuperar", "mail": "…" }` | Abre `recuperar-clave.html` con el correo precargado |
| `{ "tipo": "navegar", "destino": "alta" }` / `"bienvenida"` | Cambia de pantalla |

Los campos vacíos los marca la página, con los textos de hoy («Escribí tu correo electrónico para continuar.»,
«Escribí tu contraseña para continuar.»), y en ese caso no manda nada.

### `alta-cuenta.html`

**Datos que manda la aplicación** (todos los campos son opcionales):

```json
{
  "paso": "elegir | datos | correo | telefono | listo",
  "perfil": "cliente | organizador",
  "parametros": { "longitudMinima": 8, "fortalezaMinima": 4 },
  "error": { "campo": "cuit", "mensaje": "El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador." },
  "envioTelefono": "Si el teléfono es válido, guardamos un SMS simulado fuera de EventAura. Abrí el archivo del mensaje para continuar.",
  "telefonoVerificado": true,
  "resultado": { "perfil": "organizador" }
}
```

- **`parametros`:** los manda la aplicación al abrir, leídos de la tabla `Parametros` (`PWD_LONGITUD_MIN` y
  `PWD_FORTALEZA_MIN`). La página no tiene los números escritos: usa 8 y 4 solo hasta que llegan.
- **`error.campo`:** el `id` del campo donde se muestra el error. Los valores posibles son `nombre`,
  `apellido`, `mail`, `contrasena`, `confirmar`, `responsable`, `cuit`, `telefono`, `local`, `capacidad`,
  `direccion`, `cbu`, `documento`, `codigo-correo` y `codigo-telefono`. Sin `campo`, el mensaje va arriba de
  los botones del paso.
- **Mensajes de error:** son los de la `ReglaNegocioException`, tal cual. Un error inesperado usa
  `ManejadorErrores.MensajeParaUsuario`. **No hay que repetir T-23:** una regla de negocio nunca se muestra
  como «error inesperado».

**Mensajes que manda la página:**

| Mensaje | Qué hace la aplicación | Responde |
|---|---|---|
| `{ "tipo": "registrar", "perfil": "cliente" \| "organizador", "cuenta": { "nombre", "apellido", "mail", "contrasena" } }` | `GestorUsuarios.RegistrarCliente` (envía el código de verificación) | `{ "paso": "correo" }`, también si el correo ya existía |
| `{ "tipo": "confirmarCorreo", "codigo": "…" }` | `GestorCodigosCorreo.ConfirmarVerificacion`, como `frmCrearUsuario` hoy | Cliente: `{ "paso": "listo", "resultado": { "perfil": "cliente" } }`. Organizador: inicia la sesión (ver abajo) y responde `{ "paso": "telefono" }` |
| `{ "tipo": "enviarCodigoTelefono", "telefono": "+549…" }` | `GestorSolicitudesOrganizador.EnviarCodigoTelefono` | `{ "paso": "telefono", "envioTelefono": "…" }` |
| `{ "tipo": "confirmarTelefono", "telefono": "+549…", "codigo": "…" }` | `GestorSolicitudesOrganizador.ConfirmarCodigoTelefono` | `{ "paso": "telefono", "telefonoVerificado": true }` |
| `{ "tipo": "presentarSolicitud", "solicitud": { "responsable", "cuit", "telefono", "nombreLocal", "direccion", "capacidad", "cbuAlias", "documentacion": { "nombreArchivo", "base64" } } }` | `ImagenEventoCodec.Preparar` sobre la documentación y `GestorSolicitudesOrganizador.Solicitar` | `{ "paso": "listo", "resultado": { "perfil": "organizador" } }` |
| `{ "tipo": "navegar", "destino": "acceso" \| "bienvenida" }` | Cambia de pantalla | — |

`cuit` llega solo con dígitos, `cbuAlias` recortado y en minúsculas, y `telefono` ya normalizado: las mismas
normalizaciones que hace el negocio, que igual las vuelve a hacer.

### Secuencia del organizador: un punto que hay que decidir

Hoy, `GestorSolicitudesOrganizador` exige **una sesión de Cliente** para enviar el código del teléfono y para
presentar la solicitud (RN-50 y RN-51). El alta todavía no tiene sesión. La maqueta resuelve el orden así:
1. crear la cuenta Cliente;
2. verificar el correo;
3. **la aplicación inicia la sesión**;
4. verificar el teléfono;
5. presentar la solicitud.

Para el paso 3 hay dos caminos, y **decide JC**:
- **(a, recomendado) La aplicación guarda la contraseña en memoria** desde «registrar» hasta «confirmarCorreo»
  y llama a `GestorUsuarios.Autenticar`; después la borra. No cambia el negocio.
  - Si el correo ya existía (registro duplicado), la contraseña nueva no coincide, `Autenticar` falla, y la
    aplicación responde `{ "paso": "listo", "resultado": { "perfil": "cliente" } }` sin solicitud. Así no
    revela que la cuenta existía.
- **(b) Presentar la solicitud en el primer inicio de sesión.** El alta termina en «Verificá tu correo» y la
  solicitud queda guardada para cuando la persona entre. Hay que guardar los datos de la solicitud fuera de la
  sesión, lo que es más complejo.

En los dos casos **la cuenta se crea como Cliente y la solicitud queda Pendiente**. La aprobación es del
Administrador, en `frmRevisarSolicitudesOrganizador`, y no cambia: RN-53.

---

### `recuperar-clave.html` (nueva)

Reemplaza la pantalla de recuperación de `frmCambiarContraseña`. Mantiene las tres garantías de hoy:
- la respuesta al pedir el código es la misma exista o no la cuenta;
- cada código vence y se usa una vez;
- pedir otro código invalida el anterior.

| `datos` | Para qué |
|---|---|
| `{ "paso": "mail", "parametros": { "longitudMinima": 8, "fortalezaMinima": 4 }, "mail": "…" }` | Al abrir. `mail` es opcional y sirve para precargar el correo escrito en el acceso |
| `{ "paso": "codigo" }` | Después de pedir el código. La bajada pasa a ser el texto de siempre: «Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.» |
| `{ "paso": "codigo", "error": { "campo": "codigo", "mensaje": "El código no es válido o venció. Solicitá uno nuevo." } }` | Código rechazado. `campo` puede ser `codigo` o `mail`; sin `campo`, el mensaje va arriba del botón |
| `{ "paso": "listo" }` | Contraseña cambiada |

| Mensaje | Qué hace la aplicación |
|---|---|
| `{ "tipo": "enviarCodigoRecuperacion", "mail": "…" }` | `GestorCodigosCorreo.SolicitarRecuperacion(mail)` y responde `paso: "codigo"`, siempre. Sirve también para «Pedir otro código» |
| `{ "tipo": "restablecer", "mail", "codigo", "contrasena" }` | `GestorCodigosCorreo.RecuperarContrasena(mail, codigo, contrasena)`. Responde `paso: "listo"` o el error de la regla |
| `{ "tipo": "navegar", "destino": "acceso" }` | Vuelve al acceso |

La fortaleza de la contraseña nueva se muestra con el mismo medidor que el alta, alimentado por los
`parametros`.

### `armador-paquete.html` (nueva): el momento de autor

Reemplaza el armado de `FrmCompraCliente`:
- **cada sector es una tarjeta** con su precio, su disponibilidad y el control de cantidad adentro;
- **los servicios y el seguro** son interruptores;
- **el total se recalcula a la vista** mientras el desglose se arma línea por línea;
- **el descuento por nivel** aparece solo cuando hay entradas y el nivel lo da.

**Qué es web y qué no.** La barra superior y el lateral de la maqueta son contexto: en la aplicación los dibuja
la navegación del cliente en Windows Forms. **WebView2 aloja solo `<main class="area-web">`.** La maqueta
muestra la barra oscura: hoy es violeta, y en esta pantalla sumaría un quinto uso del acento. Si JC quiere esa
coherencia, es un cambio de `Tema`/`frmMenuCliente` (decisión aparte).

**La aplicación es la que calcula.** La página muestra una vista previa con la misma fórmula que
`GestorFidelizacion.CalcularDescuento` (el comentario del `.js` la transcribe). Con cada cambio manda
`seleccion` con un número de `version`. La aplicación responde con el `resumen` del negocio para esa versión, y
ese resumen reemplaza la vista previa. Si llega un `resumen` de una versión vieja, la página lo ignora.

**Datos que manda la aplicación al abrir** (y de nuevo si cambia la disponibilidad):

```json
{
  "evento": { "nombre": "Duki en Rosario", "categoria": "Concierto", "lugar": "Galpón 17",
              "direccion": "Estévez Boero 600, Rosario", "fecha": "Sábado 23 de enero · 21:00", "colorIndice": 1 },
  "cliente": { "nombre": "Paula Plata", "nivel": "Plata", "porcentaje": 0.05, "suscripcionActiva": false },
  "parametros": { "cargoServicio": 0.15, "seguroPrecio": 5000, "maxEntradas": 6, "bonificacionSeguro": 1 },
  "sectores": [ { "id": 1, "nombre": "Campo", "precio": 25000, "disponibles": 2000, "capacidad": 2000 } ],
  "servicios": [ { "id": 1, "nombre": "Hotel Ríos 1 noche", "tipo": "Hospedaje", "precio": 45000 } ]
}
```

- **`colorIndice`:** `PortadaEvento.ColorDelNombre(nombre)`, para que la portada chica sea la misma del
  catálogo.
- **`porcentaje`:** sale de `GestorFidelizacion.PorcentajePara(nivel)`, igual que el nivel persistido del
  cliente (RN-22).
- **`parametros`:** salen de la tabla `Parametros`: `CARGO_SERVICIO`, `SEGURO_PRECIO`, `COMPRA_ENTRADAS_MAX` y
  `SUSCRIPCION_SEGURO_DESCUENTO`. La página no tiene ningún número escrito.
- **Al liberar reservas vencidas** (RN-15, `LiberarVencidas`), la aplicación vuelve a mandar `sectores`. Si
  cambió la disponibilidad, la página recorta lo elegido.

**Después:**
- `{ "resumen": { "version", "baseEntradas", "porcentaje", "montoDescuento", "subtotalEntradas", "servicios", "cargoServicio", "seguro", "total" } }`
  es el `ResumenDescuento` tal cual, más la `version` recibida;
- `{ "error": { "mensaje": "…" } }` es un rechazo del negocio, por ejemplo «En el sector Campo quedan 1
  entradas.» al reservar. Se muestra arriba del botón con el texto de la regla, nunca como «error inesperado»
  (T-23).

**Mensajes que manda la página:**

| Mensaje | Qué hace la aplicación |
|---|---|
| `{ "tipo": "seleccion", "version", "entradas": [ { "sectorId", "cantidad" } ], "servicios": [ id ], "seguro" }` | Arma el `Paquete`, llama a `CalcularDescuento` y responde `resumen` con esa `version` |
| `{ "tipo": "comprar", …misma selección }` | Lo que hoy hace «Comprar»: valida (RN-12, cupo, preventa, prioridad), reserva y abre el pago. Si el negocio rechaza, responde `error` |
| `{ "tipo": "listaEspera", "sectorId" }` | Desde un sector agotado: anota en la lista de espera, como hoy |
| `{ "tipo": "niveles" }` | Abre la explicación de los niveles («¿Cómo funcionan los niveles?») |
| `{ "tipo": "navegar", "destino": "eventos" }` | Vuelve a «Eventos disponibles» (hoy pide confirmar si hay un paquete armado) |

**Lo que ya resuelve la página, para no repetirlo en Windows Forms:**
- el aviso del seguro, con el texto de hoy («No agregaste el seguro de entrada. Solo se puede contratar ahora:
  después de pagar no se puede incorporar.»), como bloque dentro del resumen y no como cuadro de diálogo;
- el límite de 6 entradas y el de la disponibilidad de cada sector, con los botones deshabilitados;
- «Continuar al pago» deshabilitado sin entradas (los servicios no se venden solos).

**Respaldo:** sin WebView2, la compra sigue siendo la de hoy: la grilla de sectores, las casillas de servicios
y `lblResumen`.

## Las validaciones en vivo son de formato, no de identidad (RN-52)

La maqueta lo dice dos veces:
- en un aviso al principio del bloque de la solicitud;
- en cada marca: «✓ Formato válido…», seguida de lo que **no** confirma («No confirma a quién pertenece», «No
  confirma que la cuenta exista ni que sea tuya», «No confirma que el documento sea auténtico»).

El bloque final y la pantalla «Listo» repiten que la cuenta queda como Cliente y que la habilitación como
Organizador **no es automática**. Al integrar, ese texto no se cambia por «verificado» ni por «aprobado».

| Validación en la página (`validaciones.js`) | Copia de | Regla |
|---|---|---|
| `cuitValido`: 11 dígitos, pesos 5-4-3-2-7-6-5-4-3-2 y el caso especial del prefijo 23 | `GestorSolicitudesOrganizador.CuitValido` | RN-52 |
| `cbuValido`: 22 dígitos y los dos verificadores (7-1-3-9-7-1-3 y 3-9-7-1-3-9-7-1-3-9-7-1-3) | `CbuValido` | RN-52 |
| `tipoCbuAlias`: CBU, o alias de 6 a 20 caracteres (minúsculas, dígitos, `.` y `-`) | `CbuAliasValido` | RN-52 |
| `normalizarTelefono`: quita 54, 549, 0 y 9; exige 10 dígitos con característica del 2 al 4; devuelve `+549…` | `NormalizarTelefonoArgentino` | RN-51 |
| `revisarImagen`: PNG o JPEG por cabecera; original de hasta 10 MB | `CabeceraImagenValida` y `ImagenEventoCodec.MaximoArchivoBytes` | RN-52 |
| `puntajeFortaleza` (0 a 5) y `contrasenaCumpleMinimo` | `GestorUsuarios.PuntajeFortaleza` y `ValidarContrasena` | RN-47 y RN-02 |
| `mailConFormato` | Solo una ayuda de escritura: el negocio no valida el formato del correo | — |

Si cambia una regla en `Repo/`, hay que cambiar la copia en `validaciones.js`. Lo que valga es siempre lo que
diga el negocio al recibir el mensaje.

## Lo que no está decidido

- Si la bienvenida aparece en cada arranque o solo en el primero. La maqueta supone que en cada arranque.
- La barra superior de la navegación del cliente: violeta (hoy) u oscura (maqueta del armador).
- La secuencia del organizador: opción (a) o (b), arriba.
