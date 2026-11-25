# Diseños de entrada: bienvenida, acceso y alta de cuenta

Claude Code, 24/11/2026, a pedido de JC. Son **maquetas funcionales** en HTML, CSS y JavaScript, sin red
ni recursos externos, para que Codex las integre con WebView2 como hizo con el catálogo. No tocan
`Visual/`, `Repo/` ni `Datos/`.

## Archivos

| Archivo | Qué es |
|---|---|
| `bienvenida.html` + `bienvenida.js` | Pantalla de entrada: qué es EventAura y los dos caminos |
| `acceso.html` + `acceso.js` | Inicio de sesión |
| `alta-cuenta.html` + `alta-cuenta.js` + `alta-cuenta.css` | Alta con la elección «comprar» u «organizar» |
| `entrada.css` | Estilos comunes. La paleta y las medidas salen de `Visual/Tema.cs` |
| `entrada-puente.js` | El puente con la pantalla de Windows Forms (mismo contrato que `Visual/Web/tema.js`) |
| `validaciones.js` | Validaciones de formato, copia exacta de las del negocio (tabla de abajo) |

## Cómo verlas

Abrí cualquier `.html` con doble clic: funciona sin servidor. Abajo a la derecha aparece el panel
**«Maqueta · aplicación simulada»**. Ese panel:
- muestra cada mensaje que la página le mandaría a la aplicación, con las contraseñas y los códigos tapados;
- responde como lo haría la aplicación;
- muestra lo que en la realidad pasa **fuera** de EventAura, por ejemplo los códigos de la bandeja simulada,
  para poder completar el recorrido;
- tiene botones para previsualizar estados (error del negocio, base no disponible, etc.).

Dentro de WebView2 el panel no existe: `entrada-puente.js` lo crea solo si no hay `window.chrome.webview`.

Para revisar el alta sin hacer clics, hay tres anclas (solo en la maqueta):
- `alta-cuenta.html#datos-organizador`: el formulario del organizador vacío;
- `alta-cuenta.html#demo-organizador`: el mismo formulario, completo y válido;
- `alta-cuenta.html#listo-organizador`: la pantalla final.

**Verificado el 24/11/2026:**
- `validaciones.js` contra 18 casos con Node, entre ellos los CUIT de la semilla de demostración
  (30000000007, 20000000044, 27000000022, 20000000095 y 30000000031), el CBU y el teléfono del guion, y un
  CUIT con el dígito verificador mal;
- las tres pantallas, dibujadas con Edge sin ventana a 1100 × 720 y a 600 de ancho.

**No probado todavía:** dentro de WebView2 y a 144 DPI. Eso lo hace Codex al integrarlas.

---

## Integración

### Dónde van y cómo se cargan

- Copiar los archivos a `Visual/Web/`, junto a `catalogo.html`. Los nombres no chocan con los existentes.
- Cada `.html` ya trae la misma política de contenido que el catálogo: `script-src 'self'`, sin scripts en
  línea, e imágenes solo `self` y `data:`. No hay fuentes externas: todo usa Segoe UI del sistema.
- El tema llega por `eventAuraActualizar({ tema, datos })`, igual que en el catálogo. **`Tema.VariablesWeb()`
  tiene que sumar** las variables marcadas «extra» en `entrada.css`:
  - `primario-presionado`;
  - `lateral-fondo`, `lateral-encima`, `lateral-texto` y `lateral-seccion`;
  - `fondo`, `borde-control`, `error` y `error-texto`;
  - `espacio-xl` y `espacio-xxl`;
  - `rotulo`, `titulo`, `seccion` y `marca`.

  Mientras no lleguen, la página usa los valores por omisión, que son los de `Tema.cs`.
- **Ventana:** las tres pantallas están pensadas para unos 1100 × 720 lógicos. Con menos de 820 px de ancho
  pasan a una columna. El alta del organizador es más larga que la ventana: el contenido se desplaza y la
  columna de la marca queda fija. Ningún formulario pasa de 790 px de alto (`CLAUDE.md`); acá eso se cumple
  porque el desplazamiento es interno.

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
| `{ "tipo": "navegar", "destino": "recuperar", "mail": "…" }` | Abre la recuperación de contraseña actual (no está rediseñada) |
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
- La recuperación de contraseña no está rediseñada: el acceso la abre como hoy.
- La secuencia del organizador: opción (a) o (b), arriba.
