# Auditoría visual de las vistas web integradas (26/11/2026)

Claude Code, a pedido de JC. **Solo lectura:** no se tocó nada de `Visual/`. Codex estaba trabajando en
`Visual/Web/`, y durante la medición commiteó `b4ae003`, `756d106` y `b3a0d1b`.
- **Primera pasada:** sobre su árbol de trabajo.
- **Las pantallas de entrada se midieron de nuevo sobre `b3a0d1b`,** porque ese commit agregó el velo del panel
  (ver A-2).
- **Las otras páginas** (armador, catálogo, gestión, datos) no cambiaron entre la medición y esos commits, salvo
  el ancho del resumen y la selección neutra del armador, que ya estaban en el árbol medido.

## Cómo se midió

- **Motor:** Edge sin ventana (`headless`, canal `msedge` 158), que es el mismo motor de WebView2. Se usó la
  librería de Playwright directamente, no el MCP, porque el MCP está configurado con la ventana visible.
- **Servidor:** `Visual/Web/` servida en `127.0.0.1:8766` con `http.server`, que es de solo lectura.
- **Como dentro de la aplicación:** se simuló `window.chrome.webview` para que las páginas se comporten como
  en la aplicación (sin el panel «Maqueta»). El tema se inyecta con `eventAuraActualizar({ tema, datos })`,
  con los valores de `Tema.VariablesWeb()`.
- **Datos:** armados con la misma forma que mandan los formularios (`FrmCompraCliente`, `TarjetaEvento.ModeloWeb`,
  `frmHistorialCliente`, `frmAuditoria`, `PanelAdministrador`, etc.). El armador lleva el paquete del guion:
  2 Campo, hospedaje, traslado y seguro, nivel Plata, total $ 121.625,00.
- **58 casos:** 19 vistas o estados. Cada uno se midió a 1366 × 768 y a 1280 × 720, como vista completa, y
  al área estimada de WebView2 dentro de la aplicación en cada una de esas dos pantallas.
- **Qué se midió en cada caso:**
  - texto cortado o fuera de la vista;
  - textos superpuestos;
  - acciones bajo el pliegue;
  - contraste: el peor píxel real detrás de cada texto, con el texto ocultado;
  - trazos del acento: elementos reales y sus `::before`/`::after`;
  - tamaños y espacios fuera de la escala;
  - consola y respuestas HTTP.

**Áreas de WebView2: estimadas a partir del código, no medidas dentro de la aplicación.** Las ventanas se
recortan al área de trabajo (`FormularioEventAura.OnShown`, `Disposicion.AjustarAreaVisible`). Con la barra de
tareas de 48 px y el marco de Windows 11, las cuentas dan:

| Vista | Pantalla 1366 × 768 | Pantalla 1280 × 720 | De dónde sale |
|---|---|---|---|
| Bienvenida, acceso | 1100 × 681 | 1100 × 633 | `Dock = Fill` en una ventana de 1100 × 720 |
| Alta, recuperación | 1100 × 625 | 1100 × 577 | Debajo de la barra de 56 px |
| Armador | 1350 × 681 | 1264 × 633 | `FrmCompraCliente` (1366 × 768), recortada |
| Catálogo | 1100 × 531 | 1100 × 483 | Debajo del título y los filtros (≈150 px) |
| Gestión | 1052 × 461 | 1052 × 413 | x = 24, desde y ≈ 140 hasta el pie |
| Reportes | 1052 × 341 | 1052 × 293 | `frmReportes`, desde y ≈ 292 hasta los botones |
| Panel del Administrador | 1100 × 681 | 1100 × 633 | Ocupa la ventana |

**Herramienta y capturas**, en el scratchpad de la sesión:
- herramienta: `C:\Users\JUANCR~1\AppData\Local\Temp\claude\9c849513f9e06241\56debd5b-5ae8-4bde-8bf9-4341c8042ddd\scratchpad\auditoria\`
  (`medir.js`, `casos.js`);
- capturas y `resultados.json`: `C:\Users\JUANCR~1\AppData\Local\Temp\claude\9c849513f9e06241\56debd5b-5ae8-4bde-8bf9-4341c8042ddd\scratchpad\auditoria\t2\`.

En cada hallazgo, **[ejecutado]** quiere decir que lo midió la herramienta o que se ve en la captura, y
**[deducido]** que sale de leer el código.

---

## Alta: rompe una pantalla o no cumple AA

### A-1. Historial de compras: cada entrada ocupa una pantalla entera [ejecutado]

`frmHistorialCliente` usa `gestion.html` (vista `historial`). Al elegir un pedido, cada renglón de entrada mide
lo mismo que la vista: 413 px a 1052 × 413 y 830 px con dos entradas. El usuario ve un hueco en blanco debajo de
«Pedido EA-000121 · …». La primera casilla «Devolver» aparece al fondo, y la segunda queda fuera de la vista.
Es la pantalla de la devolución por entrada.

- **Causa** [ejecutado]: un choque de nombres. `sistema.css:166` define `.entrada { display: grid;
  grid-template-columns: var(--ancho-panel) 1fr; min-height: 100vh; }`, que es la retícula de las pantallas
  de entrada. `gestion.js:25` crea cada renglón como `<label class="entrada">`, y `gestion.css:16` pisa la
  grilla pero no el `min-height`.
- **Desde cuándo** [deducido]: desde `b6d05bf`, cuando `gestion.html` empezó a cargar `sistema.css`. Está en
  lo commiteado.
- **Arreglo, para Codex:** renombrar la clase del renglón en `gestion.js:25` y `gestion.css:16-18`, por
  ejemplo a `entrada-ticket`. Otra opción es agregar `min-height: 0` en `gestion.css:16`. Lo primero evita
  que vuelva a pasar.
- Captura: `t2\gestion-historial-app720.png`.

### A-2. Alta de cuenta: los pasos pendientes del panel, por debajo de 4,5:1 [ejecutado]

**Primera medición**, sobre el árbol de trabajo de Codex antes de `b3a0d1b`, sobre el muro, en las cuatro
pantallas y en los cuatro tamaños:

| Texto | Pantallas | Contraste |
|---|---|---|
| Sobrerrótulo N6 | Bienvenida, acceso, alta y recuperación | 3,95 a 4,0:1 |
| Pie del panel | Las mismas | 3,78 a 3,92:1 |
| Pasos pendientes y sus números | Alta | 3,75 a 3,78:1 |

**Segunda medición, sobre lo commiteado (`b3a0d1b`, que ya trae el velo del N6 y del pie):**
- el N6 y el pie cumplen en las cuatro pantallas;
- **siguen fallando los pasos pendientes del alta:** 3,75 a 4,44:1, según el tamaño y el paso.

La maqueta ya lo resuelve: el mismo velo sobre la lista `.pasos` entera, 4,51:1 en el peor píxel. Falta
copiarlo a `Visual/Web/alta-cuenta.css` (`docs/BORRADOR-ENCARGO-CODEX-2026-11-26.md`, sección 2 bis).

---

## Media: se ve mal, o va contra PRINCIPIOS-DISENO.md

### M-1. Reportes y panel del Administrador: el gráfico de barras corta el último renglón [ejecutado]

- El rótulo «La casa del río» y su valor «95» quedan cortados por abajo, en todos los tamaños. Pasa con
  cualquier cantidad de eventos.
  - **Causa** [deducido]: en `datos.js:22-55` (`barras`), el SVG mide `12 × filas` unidades, pero el texto de la última
    fila tiene su línea base en `3 + 12·(n−1) + 10`, que deja los descendentes fuera.
- **Los rótulos del gráfico se ven a 17-18 px**, fuera de la escala (N4 es 13,33 px, N5 es 12 px).
  - **Causa:** el SVG escala con el ancho (`viewBox` de 720) y la letra de 12 unidades crece con él.
  - Los renglones quedan con interlineado 1,0: los nombres casi se tocan. La herramienta marcó 10
    superposiciones de cajas de letra; en la captura no se pisan las letras, pero no hay aire.
- Capturas: `t2\datos-reporte-app768.png`, `t2\datos-admin-app720.png`.

### M-2. Armador: el desglose del paquete del guion no entra en ningún tamaño real [ejecutado]

**El total y «Continuar al pago» se ven siempre**, en los cuatro tamaños. El desglose (9 renglones) se desplaza
dentro de la tarjeta:

| Área | Alto visible del desglose | Contenido | Qué queda oculto |
|---|---|---|---|
| 1350 × 681 (pantalla de 768) | 188 px | 235 px | «Cargo por servicio» y «Seguro (2 entradas)» |
| 1264 × 633 (pantalla de 720) | 140 px | 235 px | Desde «Traslado», que queda cortado por la mitad del renglón |

- **Captura:** `t2\armador-app720.png`. Entre «Hotel Ríos» y la raya del total se ve medio renglón.
- **Barra de desplazamiento:** Edge sin ventana no la dibuja, así que no se pudo ver si aparece. En WebView2 sí
  debería aparecer [deducido].
- **Decisión tomada:** JC decidió no cambiar la retícula del armador. Este hallazgo es para saber qué se va a
  ver en la demostración a 1280 × 720: para mostrar el cargo y el seguro hay que desplazar dentro del
  resumen.

### M-3. El acento pasa de cuatro trazos en tres vistas [ejecutado el conteo; deducido lo que la herramienta no cuenta]

La regla (PRINCIPIOS §2) es: como máximo cuatro trazos violeta por pantalla, y lo elegido no usa el acento.

**Cumplen**, contando solo lo que se ve en reposo (el anillo de foco no cuenta):

| Vista | Trazos |
|---|---|
| Bienvenida | 2 |
| Acceso | 3 |
| Recuperación | 2 |
| Alta | 3 |
| Armador | 3 (el botón, el paso actual y la franja del VIP; P2 ya está en la copia de Codex) |

**No cumplen:**
- **Catálogo:** 8 botones «Ver entradas» en violeta lleno, uno por tarjeta (`catalogo.css:21-22`).
- **Reportes y panel del Administrador:** 9 trazos. Son las 8 barras en `--primario` (`datos.js:50`, un dato
  pintado con el color de la acción) y el borde violeta del botón de gráfico elegido (`datos.css:12`; ya
  anotado en `PROPUESTA-TABLA.md` como fuera de P4).
- **Gestión:**
  - la tarjeta elegida usa `--primario-suave` (`gestion.css:9`, `.tarjeta.activa`) [deducido; la
    herramienta no cuenta el fondo suave];
  - la casilla «Devolver» marcada usa `accent-color: var(--primario)` (`gestion.css:18`) [deducido];
  - el resultado del control de ingreso lleva un borde izquierdo violeta (`gestion.css:37`) [ejecutado]. Es
    un estado, no una acción, y se ve igual cuando el ingreso es válido y cuando no.

### M-4. «Ya tengo una cuenta.» en el botón destacado de la bienvenida: 4,3:1 [ejecutado]

- `#E4DCF3` sobre `--primario`, en N5 (12 px).
- El color está escrito a mano en `sistema.css:158` y no es un token.
- Con `#fff` daría 5,7:1. Pasa igual en la maqueta.

### M-5. Marcadores de los campos del alta: 3,35:1 [ejecutado]

- `.entrada-texto::placeholder` usa `--borde-control` (`sistema.css:112`).
- Se ve en el CUIT, el teléfono y la dirección del alta de organizador.
- Los marcadores son ejemplos de formato, así que es información y debería llegar a 4,5:1. Con
  `--texto-secundario`, 6,4:1.

---

## Baja

- **B-1. Acción principal bajo el pliegue a 1280 × 720** [ejecutado, con el área estimada]. Hay que desplazar
  para llegar:
  - recuperación, paso del código: «Restablecer contraseña» queda 56 px por debajo de 577;
  - alta, paso de datos: «Crear cuenta» queda 81 px por debajo;
  - acceso con error: el enlace «Crear una» queda 12 px por debajo.

  A 1366 × 768 todo entra.
- **B-2. Eventos del local: las etiquetas de estado tienen distinto alto** [ejecutado, en la captura].
  - La grilla iguala el alto de las tarjetas y estira sus filas internas: «Publicado» queda de 43 px y
    «Borrador» de 28.
  - Se arregla con `align-content: start` en `.tarjeta` (`gestion.css:7`).
  - Captura: `t2\gestion-eventos-app720.png`.
- **B-3. Revisión de eventos: el detalle pierde los saltos de línea** [ejecutado con datos armados como los
  manda `frmRevisarEventos`, con `Environment.NewLine`]. El nombre, la fecha y la explicación salen pegados en
  un solo párrafo. Se arregla con `white-space: pre-line`.
- **B-4. Medidas fuera de la escala** [ejecutado]:
  - el título del detalle de revisión mide 15,6 px y pesa 700 (no es ningún nivel, y el sistema usa 600);
  - en el armador, el botón de cantidad lleva un relleno de 6 px, y debajo de 760 de alto los renglones del
    desglose llevan 2 px (`armador-paquete.css:141`);
  - el logo usa 22 px, que es la marca y puede quedar así.
- **B-5. `compra.html`, `compra.css` y `compra.js` no los carga ninguna pantalla** desde `c2f80ca` [deducido
  por búsqueda].
  - Solo los nombra `Pruebas/PruebasInteraccionPantallas.cs:196`.
  - Usan `tema.css`, el sistema anterior: su total mide 20 px y pesa 900.
  - Decide JC: borrarlos con la prueba, o dejarlos.
- **B-6. Las iniciales de las portadas del catálogo** las lee el lector de pantalla («D», «C»…), porque no
  tienen `aria-hidden` [deducido]. Son decorativas.

---

## Lo que dio limpio [ejecutado, 58 casos]

- **Consola:** cero errores. El único 404 es el `favicon` del servidor local, que en WebView2 no se pide.
- **Datos:** `eventAuraActualizar` devolvió `true` en todos los casos.
- **Desborde:** ningún desborde horizontal en ninguna vista ni tamaño.
- **Superposiciones de texto reales:** ninguna. Las que marcó la herramienta en el armador eran renglones
  ocultos dentro del desglose. Las del gráfico, cajas de letra que se tocan (M-1).
- **Texto cortado fuera del gráfico (M-1) y del desglose (M-2):** ninguno.
- **Contraste fuera de A-2, M-4 y M-5:** ninguno. Gestión, auditoría, catálogo (fuera de las iniciales
  decorativas) y armador dan todo por encima de 4,5:1.

## Qué no se midió

- **Otros DPI:** solo 96. No se probaron 120 ni 144.
- **Hover, presionado y foco:** no se midieron. Ejemplo [deducido]: al presionar una tarjeta de gestión, el
  fondo pasa a `--primario-presionado`, pero `.secundario` mantiene su gris; durante el clic queda en 1,4:1.
- **Las áreas dentro de la aplicación:** son estimadas (tabla de arriba). Confirmarlas exige abrir la
  aplicación, y esta noche no se abrió para no sacarle el foco a Codex.
- **Las barras de desplazamiento:** no se ven en Edge sin ventana.

## Qué haría cada uno (decide JC)

- **Codex**, cuando termine lo actual y si JC lo aprueba:
  - A-1, primero;
  - copiar el velo de los pasos de A-2 a `alta-cuenta.css` (está en el borrador del encargo);
  - después M-4, M-5, B-2 y B-3, que son de una línea cada uno.
- **Para decidir:**
  - M-1: rehacer la escala del gráfico de barras;
  - M-3: qué hacer con el catálogo, las barras y lo elegido en gestión;
  - B-5: borrar `compra.*`.
- **Claude Code:** cuando JC avise que Codex terminó, pasar lo aprobado a `docs/TICKETS.md`, volver a medir
  A-1 y A-2 en `Visual/Web/` y anotar en `docs/PRUEBAS-PENDIENTES.md` la verificación del historial con una
  compra de dos entradas.
