# Sistema de diseño de EventAura

**Versión 1, 05/11/2026. Aprobación pendiente de JC.** Es la referencia de la renovación
visual (camino A, decisiones V-1 a V-6 del 05/11). Reemplaza a la sección 4 de
`PLAN-RENOVACION-VISUAL.md`.

Todo vive en `Visual/Tema.cs`. **Ninguna pantalla escribe un color, una fuente ni una medida
propia:** los pide al tema. Las medidas están en píxeles a escala 100 % (96 DPI). Con la
compatibilidad con DPI (V-2), Windows las agranda en las otras escalas, nítidas.

Cada contraste de esta página está calculado con la fórmula de WCAG 2.1, no estimado.

---

## 1. Tipografía

**Segoe UI**, la fuente de Windows desde Vista, con su variante **Segoe UI Semibold** (las dos
vienen con Windows). Reemplaza a Nirmala UI (una fuente para escrituras de la India) y a
MS Gothic (una fuente japonesa), que hoy usan los textos y los campos.

| Token | Fuente | Tamaño | Uso |
|---|---|---|---|
| `Tema.Marca` | Segoe UI Semibold | 26 pt | «EventAura» en el inicio de sesión. Una sola vez en toda la aplicación |
| `Tema.Titulo` | Segoe UI Semibold | 18 pt | Título de la pantalla («Duki en Rosario», «Mis eventos»). **Uno por ventana** |
| `Tema.Seccion` | Segoe UI Semibold | 12 pt | Título de una tarjeta o sección («Entradas», «Servicios», «Resumen») |
| `Tema.Cuerpo` | Segoe UI | 10 pt | Texto normal, campos, celdas, casillas |
| `Tema.Boton` | Segoe UI Semibold | 10 pt | Texto de todos los botones |
| `Tema.Rotulo` | Segoe UI Semibold | 9 pt | Rótulo de un campo, encabezado de grilla |
| `Tema.Ayuda` | Segoe UI | 9 pt | Ayudas, leyendas, pie de página |
| `Tema.Comprobante` | Consolas | 10 pt | **Solo** el resumen del paquete y el desglose de la compra confirmada (ver 1.1) |

**Reglas:**
- La negrita marca jerarquía, no se usa para todo: hoy todo está en negrita.
- Nada en cursiva ni subrayado, salvo los enlaces al pasar el mouse.
- Mayúsculas solo en siglas.

### 1.1 Por qué el resumen sigue en Consolas

El plan proponía pasar el resumen a una tabla de dos columnas, con el TOTAL más grande. Lo
reconsideré y propongo dejarlo como está, con fuente monoespaciada y estilo de comprobante:

- Consolas viene con Windows y es una fuente cuidada, no la de una terminal.
- El resumen parece un comprobante, y eso está bien para un desglose de importes.
- Pasarlo a tabla obliga a reescribir CP-13 (que mide sus líneas) y no mejora la lectura.

**Lo que WinForms no da:** dentro de una misma etiqueta no se pueden mezclar tamaños, así que
el TOTAL no puede ser más grande que el resto sin agregar otra etiqueta aparte.

---

## 2. Paleta

| Token | Color | Uso | Contraste |
|---|---|---|---|
| `Primario` | `#7456AE` (el violeta actual) | Fondo del botón principal, enlaces, temporizador normal, franja del ticket | 5,70:1 sobre blanco; blanco sobre él, 5,70:1 |
| `PrimarioEncima` | `#634699` | Botón principal con el mouse encima | Blanco sobre él, 7,33:1 |
| `PrimarioPresionado` | `#523A80` | Botón principal presionado | — |
| `PrimarioSuave` | `#EFEAF8` | Fila seleccionada, celda editable, botón secundario con el mouse encima | `Texto` sobre él, 13,91:1; `Primario`, 4,83:1 |
| `Texto` | `#1F1F24` | Todo el texto normal | 16,41:1 sobre blanco; 15,13:1 sobre `Fondo` |
| `TextoSecundario` | `#5C5F66` | Rótulos, ayudas, encabezados de grilla. **Reemplaza al gris `#A4A5A9`**, que tiene 2,46:1 y hace que todo parezca deshabilitado | 6,40:1 sobre blanco; 5,90:1 sobre `Fondo` |
| `Fondo` | `#F6F5FA` | Fondo de la ventana, detrás de las tarjetas | — |
| `Superficie` | `#FFFFFF` | Tarjetas, grillas, campos, encabezado y pie de la ventana | — |
| `EncabezadoGrilla` | `#F1EFF7` | Fondo del encabezado de las grillas | `TextoSecundario` sobre él, 5,61:1 |
| `FilaAlterna` | `#FAF9FD` | Filas pares de las grillas | — |
| `Borde` | `#DCDAE3` | Borde de tarjetas y líneas de las grillas. Decorativo: separa, no delimita un control | 1,38:1 (no hace falta más) |
| `BordeControl` | `#8E8A9C` | Borde de campos, casillas y botones secundarios | 3,35:1, por encima del mínimo de 3:1 que WCAG pide para el borde de un control |
| `Foco` | `Color.Black` (`#000000`) | **Solo** el borde de foco del botón principal (5.1). Agregado el 10/11/2026 | 3,68:1 contra `Primario` y 21:1 contra `Superficie`: cumple el 3:1 que WCAG pide para un indicador de foco contra los dos colores que toca. `Texto` (#1F1F24) no alcanza: 2,88:1 contra `Primario` |
| `FondoDeshabilitado` | `Color.Gainsboro` | Botón deshabilitado. Es el color actual; con el nombre `Gainsboro`, no con sus números, porque CP-22 y CP-23 comparan el color con nombre | — |
| `Error` | `Color.Firebrick` | Errores, reserva vencida, temporizador en el último minuto. Es el rojo actual, con nombre, por CP-39 | 6,68:1 sobre blanco |
| `ErrorFondo` | `#FCE8E6` | Fondo de un aviso de error dentro de la pantalla | `Error` sobre él, 5,67:1 |
| `Exito` / `ExitoFondo` | `#1E7B4A` / `#E7F4EC` | «¡Compra confirmada!», operación correcta | 4,65:1 |
| `Advertencia` / `AdvertenciaFondo` | `#8A5A00` / `#FFF4D6` | Cupo bajo, seguro no contratado | 5,41:1 |

**Reglas:**
- El `Primario` de fondo se usa en **un solo botón por pantalla**: el de la acción principal.
- Rojo, verde y ámbar solo para estados, nunca para decorar.
- Ningún texto en gris claro: el texto deshabilitado lo dibuja Windows (ver la sección 7).

---

## 3. Espaciado

Escala de 8 px: **4 · 8 · 12 · 16 · 24 · 32 · 48** (`Tema.Espacio.XS` a `XXL`).

| Dónde | Medida |
|---|---|
| Margen interior de la ventana | 24 |
| Entre tarjetas o secciones | 24 |
| Relleno interior de una tarjeta | 16 |
| Entre el título de una tarjeta y su contenido | 12 |
| Entre el rótulo y su campo | 4 |
| Entre un campo y el rótulo siguiente | 16 |
| Entre casillas | 8 |
| Entre botones vecinos | 8 |
| Alto de los botones | 32; el principal, 36 |
| Ancho mínimo de un botón | 120 |
| Relleno horizontal de un botón | 20 |
| Alto de fila de grilla | 32; encabezado 36, o automático si el texto ocupa dos renglones (T-29) |
| Alto del encabezado de la ventana | 72 |
| Alto del pie de la ventana | 60 |

---

## 4. Estructura de una ventana

```
┌───────────────────────────────────────────────────────────────────┐
│ ENCABEZADO  · Superficie · 72 px · línea inferior de 1 px Borde   │
│   Título 18 pt                         Ayuda 9 pt (usuario, nivel)│
├───────────────────────────────────────────────────────────────────┤
│ CONTENIDO  · Fondo · margen 24 · se desplaza si no entra          │
│   ┌──── tarjeta (Superficie, borde 1 px Borde, relleno 16) ─────┐ │
│   │ Sección 12 pt                                               │ │
│   │ …                                                           │ │
│   └─────────────────────────────────────────────────────────────┘ │
├───────────────────────────────────────────────────────────────────┤
│ PIE  · Superficie · 60 px · línea superior de 1 px Borde · fijo   │
│   [ Volver ]  secundario, izquierda          [ Comprar ] primario │
└───────────────────────────────────────────────────────────────────┘
```

- «Volver» siempre abajo a la izquierda, como botón secundario. La acción principal, abajo a
  la derecha. Así ningún «Volver» queda como etiqueta que el teclado no alcanza (V-5).
- El pie no se desplaza: las acciones quedan siempre a la vista, en cualquier escala.
  `FrmCompraCliente` ya lo hace así (`pnlAccionesCompra`).
- Las tarjetas son paneles blancos con borde de 1 px, **con esquinas rectas** (ver la
  sección 7).

---

## 5. Componentes

### 5.1 Botones

Son clases que **heredan de `Button`** (siguen siendo `Button` para las pruebas), con el
estilo puesto en su constructor. El diseñador de Visual Studio las muestra como son.

| Clase | Fondo | Texto | Borde | Encima | Presionado |
|---|---|---|---|---|---|
| `BotonPrimario` | `Primario` | Blanco, `Tema.Boton` | — | `PrimarioEncima` | `PrimarioPresionado` |
| `BotonSecundario` | `Superficie` | `Primario`, `Tema.Boton` | 1 px `BordeControl` | `PrimarioSuave` | `#E2DAF2` |
| `BotonPeligro` | `Superficie` | `Error`, `Tema.Boton` | 1 px `BordeControl` | `ErrorFondo` | `#F8D4D0` |

- **Deshabilitado:** fondo `FondoDeshabilitado`. El texto lo dibuja Windows en gris (sección 7).
- **Con foco de teclado:** borde de 2 px. Hoy los botones planos no muestran el foco, y quien
  usa teclado no sabe dónde está.
  - **Secundario y peligro:** borde `Primario`, que contrasta 5,70:1 con su fondo blanco.
  - **Primario: borde `Foco` (negro).** Corregido el 10/11/2026. Antes este documento pedía
    también `Primario`, y en el botón principal eso es violeta sobre violeta: el foco no se
    veía (`REVISION-6A.md`, O-1). El color del texto (`Texto`) tampoco sirve: 2,88:1 contra el
    violeta, por debajo del 3:1. El negro da 3,68:1.
  - Lo controla CP-62: suma el par `Foco` sobre `Primario`, con un mínimo de 3:1.
- **Cuándo usar cada uno:** primario para la acción que completa la pantalla («Comprar»,
  «Pagar», «Iniciar sesión», «Registrar»); secundario para todo lo demás; peligro para lo que
  borra o cancela algo hecho («Eliminar evento», «Eliminar sector», «Cancelar pago»).
- **Enlace:** `LinkLabel` con `Primario`, subrayado solo al pasar el mouse. Para acciones
  menores que no son la principal: «¿Cómo funcionan los niveles?», «¿Olvidaste tu
  contraseña?», «Crear cuenta». Un `LinkLabel` **sí recibe el foco**; una `Label` con clic,
  no (es la causa de T-1 a T-5).

### 5.2 Grillas (`DataGridView`)

| Propiedad | Valor |
|---|---|
| Encabezado | Fondo `EncabezadoGrilla`, texto `Tema.Rotulo` en `TextoSecundario`, sin el estilo de Windows (`EnableHeadersVisualStyles = false`), ajuste de línea y alto automático (T-29) |
| Filas | 32 px, texto `Tema.Cuerpo` en **`Texto`** (no en gris), filas alternas en `FilaAlterna` |
| Líneas | Solo horizontales (`SingleHorizontal`), en `Borde`; sin borde exterior propio dentro de una tarjeta |
| Columna de encabezado de fila | Oculta |
| Selección | Fondo `PrimarioSuave`, texto `Texto`. No el azul de Windows |
| Celda editable | Fondo `PrimarioSuave` (la cantidad de la compra) |
| Alineación | Números e importes a la derecha; texto a la izquierda |
| Formato | Importes `$ 25.000,00`; fechas `dd/MM/yyyy HH:mm` |
| Teclado | `StandardTab = true`: Tab sale de la grilla en una pulsación; las flechas recorren las celdas. Corrige T-8 (hoy «Mis eventos» lleva 27 Tab) |
| Sin resultados | Una fila de aviso en `TextoSecundario`, como hoy (CP-16) |

### 5.3 Formularios de carga

| Elemento | Estilo |
|---|---|
| Rótulo | **Arriba** del campo, `Tema.Rotulo`, `TextoSecundario`, a 4 px |
| Campo (`TextBox`) | `Tema.Cuerpo`, `Texto`, fondo `Superficie`, borde simple de Windows (`FixedSingle`). Alto natural de la fuente: unos 25 px (sección 7). Ancho según el dato: correo 320, nombre 240, código de seguridad 80 |
| Texto guía | El de Windows (cue banner), en los campos de búsqueda: como hoy (CP-18) |
| Casilla (`CheckBox`) | `Tema.Cuerpo`, `Texto`. El precio va en el mismo texto («Hospedaje · $ 45.000») |
| Desplegable y fecha | Los de Windows, con `Tema.Cuerpo` |
| Teclado | Foco inicial en el primer campo; Tab en orden de lectura; **Enter ejecuta la acción principal** (`AcceptButton`, corrige U-02 en el login); **Esc vuelve** (`CancelButton`, corrige T-10) |

### 5.4 Mensajes

| Tipo | Cómo se muestra | Título | Ejemplo |
|---|---|---|---|
| Regla de negocio que rechaza algo | `MessageBox` nativo, ícono de advertencia | **En castellano y según la pantalla** (V-6). El código va al final del texto | Título «No se puede armar el paquete»; texto «Se pueden comprar como máximo 6 entradas por operación. (RN-12)» |
| Confirmación (Sí / No) | `MessageBox` nativo, ícono de pregunta, **«No» por defecto** si se pierde algo | La pregunta corta | «Cerrar la compra», «Volver al listado» |
| Dato mal escrito | Donde ya está dentro de la pantalla (el pago, en `lblEstado`, en `Error`), sigue igual. En el resto, `MessageBox` | «Revisá los datos» | «El vencimiento debe tener el formato MM/AA.» |
| Error no previsto | `ManejadorErrores` (T-27), sin cambios | «EventAura» | «No se pudo conectar con la base de datos…» |

Los títulos se escriben con un único método, `Avisos.Regla(titulo, ex)`, para que no haya dos
redacciones del mismo caso.

### 5.5 Ícono

Una «E» blanca en Segoe UI Semibold sobre un cuadrado `Primario`, en 16, 24, 32, 48 y 256 px.
Va en la barra de título de las 19 ventanas, en la barra de tareas y en el ejecutable. Hoy es
el ícono genérico de Windows.

### 5.6 Ticket PDF (V-4)

```
┌────────────────────────── A6 apaisada, fondo blanco ──────────────────────────┐
│█████ franja Primario, 28 pt: «EventAura · Entrada» en blanco ████████████████│
│                                                                               │
│  Duki en Rosario                       (Helvetica Bold 16, Texto)  ┌───────┐ │
│                                                                    │       │ │
│  SECTOR      Campo                     (rótulos Helvetica 8,       │  QR   │ │
│  FECHA       04/01/2027 21:00           TextoSecundario;           │ 120 pt│ │
│  PRECIO      $ 25.000,00                datos Helvetica 11, Texto) │       │ │
│  PEDIDO      EA-000072                                             └───────┘ │
│                                                                               │
│  Código EA-D7D94DCD…  (Courier 8)       Presentá este código en el ingreso.  │
└───────────────────────────────────────────────────────────────────────────────┘
```

- **Fondo blanco: el QR queda con zona de silencio de sobra.** Hoy está justo en el mínimo de
  4 módulos sobre fondo negro (observación de JC).
- QR de 120 × 120 pt (42 mm, unos 1,1 mm por módulo; hoy 100 pt y 0,95 mm), a 24 pt de
  cualquier borde y de la franja.
- Helvetica es una fuente estándar de PDF: no hace falta incrustarla y se ve igual en
  cualquier lector.
- El **contenido del QR no cambia** (`CodigoQR`), así que CP-32 sigue valiendo.
- Se imprime sin gastar tinta en un fondo negro.

---

## 6. Accesibilidad y teclado

Sale del recorrido con teclado (`docs/verificaciones/recorrido-teclado.md`) y queda como regla
para las 19 pantallas:

1. Toda acción se alcanza con Tab. Nada con clic en una `Label`.
2. Foco inicial en el primer campo o en la acción principal, **nunca** en un botón que borra o
   registra («Registrar evento» hoy arranca en «Registrar»).
3. Orden de Tab igual al orden de lectura: de arriba abajo, de izquierda a derecha.
4. Enter = acción principal; Esc = volver.
5. El foco se ve: borde de 2 px en los botones (negro en el principal, violeta en los demás,
   5.1); en los campos, el cursor y el borde del sistema.
6. Ningún texto por debajo de 4,5:1 de contraste.

---

## 7. Lo que WinForms no da bien, y qué hacemos en cambio

| Lo que pediría un diseño moderno | Por qué no | Qué hacemos |
|---|---|---|
| Esquinas redondeadas en botones y tarjetas | WinForms no suaviza bordes curvos: hay que dibujarlos a mano y quedan dentados contra el fondo | **Esquinas rectas.** Las ventanas sí salen redondeadas en Windows 11, porque eso lo hace el sistema |
| Color propio para el texto de un botón deshabilitado | Un botón plano deshabilitado dibuja el texto con el gris del sistema, sin opción | Fondo `Gainsboro` (lo controlamos) y texto gris de Windows. Se distingue bien del habilitado |
| Campos de 32 px de alto con borde de color al tener foco | El `TextBox` de una línea toma el alto de su fuente (unos 25 px) y el borde lo dibuja Windows. Lograrlo exige envolver cada uno de los 24 campos en un panel dibujado a mano | **Campos con alto natural y borde de Windows.** El foco se ve por el cursor. Es el aspecto de cualquier programa de Windows |
| Errores de validación debajo de cada campo, sin ventanas | Es posible, pero 11 pruebas (CP-15, 17, 19, 20, 21, 24, 41, 42, 44, 48 y 52) responden el cuadro nativo y habría que reescribirlas | **`MessageBox` con título en castellano** (V-6). Donde ya hay mensaje en la pantalla (el pago), sigue así |
| Estilizar desplegables, calendarios, barras de desplazamiento y `MessageBox` | Son controles nativos de Windows | Conservan el aspecto de Windows, con nuestra fuente donde se puede |
| Sombras, transparencias, efecto Mica o acrílico, animaciones | WinForms no los soporta en forma confiable | No se usan. Tarjetas con borde fino en lugar de sombra |
| TOTAL más grande dentro del resumen | Una etiqueta tiene una sola fuente | El resumen queda como comprobante en Consolas (1.1) |

**El resultado esperable:** una aplicación de escritorio de Windows prolija, coherente y
legible, al nivel de una herramienta administrativa profesional. No una aplicación web.

---

## 8. Reglas de mantenimiento

1. **Nada de colores, fuentes ni medidas fuera de `Tema.cs`.** Una prueba lo verifica (CP-63).
2. **Un solo `BotonPrimario` por pantalla.**
3. **Diseñadores a 96 DPI:** todos con `AutoScaleDimensions = 96F, 96F`. Se editan con
   Windows a 100 %, o con Visual Studio reiniciado al 100 % cuando lo ofrece. Hoy hay 11 a
   120 DPI y 8 a 144 DPI, y cada edición en otra escala reescribe todas las medidas.
4. **No se renombran controles ni se cambia su tipo**, salvo `Button` → `BotonPrimario`,
   `BotonSecundario` o `BotonPeligro` (siguen siendo `Button`), y `Label` con clic → `LinkLabel`
   (sigue siendo `Label`). Las pruebas buscan 32 controles por nombre y tipo.
5. **Cada pantalla terminada se compara con la captura de referencia** aprobada por JC (la del
   piloto).
