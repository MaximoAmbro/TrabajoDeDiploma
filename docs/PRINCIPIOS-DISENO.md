# Principios de diseño de las vistas web de EventAura

Claude Code, 24/11/2026. Son los valores fijos contra los que se construyen las pantallas web que aloja
WebView2: bienvenida, acceso, alta de cuenta, recuperación de contraseña y armador de paquete. Las maquetas
están en `docs/disenos/`. La implementación de referencia es `docs/disenos/sistema.css`: **una medida que no
esté acá no se usa**. Si una pantalla necesita algo nuevo, primero se agrega acá y en `sistema.css`.

La paleta y las familias tipográficas salen de `Visual/Tema.cs`. No se edita `Tema.cs` para esto: las vistas
web las reciben por `Tema.VariablesWeb()`.

## 0. Restricciones que no se negocian

- **Sin red.** Nada de CDN, fuentes externas ni imágenes en archivo. Todo es CSS, SVG en línea y `data:`. Si
  algo se carga de afuera, la pantalla queda en blanco el día de la defensa.
- **Una aplicación de escritorio, no una página de marketing:**
  - radio de 3 px en los controles y de 4 px en las tarjetas;
  - sin sombras, salvo el anillo de foco;
  - sin animaciones que se noten: solo transiciones de 120 ms en los estados.
- **El negocio decide.** Lo que la página calcula o valida es una vista previa. El número y el error que valen
  son los que devuelve la aplicación.

## 1. Tipografía: seis niveles con saltos reales

Familias:
- Segoe UI (peso 400) para `Tema.Cuerpo` y `Tema.Ayuda`;
- Segoe UI Semibold (peso 600) para `Tema.Titulo`, `Tema.Seccion` y `Tema.Rotulo`.

**El contraste de peso es solo 400 contra 600.** No se usan 300, 500 ni 700.

| Nivel | Clase | Tamaño | Peso | Interlineado | Interletrado | Uso |
|---|---|---|---|---|---|---|
| N1 · Exhibición | `.n1` | 44 px | 600 | 1,05 | −0,03 em | **Una por pantalla.** El lema de la bienvenida, el total del armador |
| N2 · Título | `.n2` | 28 px | 600 | 1,12 | −0,02 em | El título de cada pantalla |
| N3 · Sección | `.n3` | 18 px | 600 | 1,30 | −0,01 em | Títulos de grupo, nombre del sector, precio de una tarjeta |
| N4 · Cuerpo | `.n4` | 13,33 px (10 pt, `Tema.Cuerpo`) | 400 | 1,50 | 0 | Texto corrido, campos, botones (botones en 600) |
| N5 · Detalle | `.n5` | 12 px (9 pt, `Tema.Ayuda`) | 400; rótulo en 600 | 1,50 | +0,01 em | Rótulos de campo, ayudas, estados de validación |
| N6 · Sobrerrótulo | `.n6` | 11 px | 600, mayúsculas | 1,55 | +0,10 em (`Tema.InterletradoSeccion`) | Encabezados de grupo, pasos, etiquetas |

- **Cifras:** todo importe y toda cantidad usan `font-variant-numeric: tabular-nums`, alineados a la
  derecha.
- **Ancho de línea:** el texto corrido no pasa de 60 caracteres (`max-width: 60ch`).

## 2. Color: casi todo neutro, el acento cuenta

| Rol | Variable | Valor | Origen en `Tema.cs` |
|---|---|---|---|
| Acento | `--primario` | `#7456AE` | `Primario` |
| Acento al pasar el mouse | `--primario-encima` | `#634699` | `PrimarioEncima` |
| Acento al presionar | `--primario-presionado` | `#523A80` | `PrimarioPresionado` |
| Fondo del acento | `--primario-suave` | `#EFEAF8` | `PrimarioSuave` |
| Texto | `--texto` | `#1F1F24` | `Texto` |
| Texto secundario | `--texto-secundario` | `#5C5F66` | `TextoSecundario` |
| Borde de control | `--borde-control` | `#8E8A9C` | `BordeControl` |
| Línea | `--borde` | `#DCDAE3` | `Borde` |
| Fondo | `--fondo` | `#F6F5FA` | `Fondo` |
| Superficie | `--superficie` | `#FFFFFF` | `Superficie` |
| Panel oscuro | `--lateral-fondo`, `--lateral-encima`, `--lateral-texto`, `--lateral-seccion` | `#241D30`, `#322A42`, `#CFC9DC`, `#8B839E` | `Lateral*` |
| Estados | `--exito-texto` y `--exito`, `--error-texto` y `--error`, `--advertencia-texto` y `--advertencia` | `#1E7B4A` y `#E7F4EC`, `#B22222` y `#FCE8E6`, `#8A5A00` y `#FFF4D6` | `Exito*`, `Error*`, `Advertencia*` |
| Portadas | `--portada-0` a `--portada-5` | `#5C5F66`, `#3E4757`, `#A0568D`, `#487591`, `#69458F`, `#53847E` | `PaletaPortadas` |

**Regla del acento:** el violeta aparece **como máximo cuatro veces por pantalla**, contando cada lugar donde se
ve en reposo:
- el botón principal;
- el ítem activo;
- lo seleccionado;
- un enlace.

El anillo de foco no cuenta, porque solo aparece al usar el teclado. Los colores de estado (éxito, error,
advertencia) solo aparecen cuando hay un estado que comunicar. Los colores de portada son **datos** (identifican
un evento o un sector), nunca adorno.

Cuenta por pantalla:

| Pantalla | Usos del acento |
|---|---|
| Bienvenida | Botón «Iniciar sesión» · punto del logo |
| Acceso | Botón «Iniciar sesión» · punto del logo · enlace «Crear cuenta» |
| Recuperar contraseña | Botón del paso · punto del logo |
| Alta de cuenta | Botón del paso · opción elegida · paso actual · punto del logo |
| Armador de paquete | Botón «Continuar al pago» · paso actual · selección (las tarjetas con entradas, los servicios y el seguro encendidos comparten el mismo trazo). El ítem activo del lateral usa `--lateral-encima`, no el acento |

## 3. Espacio: ritmo, no uniformidad

**Escala:** 4 · 8 · 12 · 16 · 24 · 32 · 48 · 64 (`--e1` a `--e8`). No existe otra medida.

| Relación | Separación |
|---|---|
| Dentro de un grupo (rótulo y campo, título y bajada, línea de un desglose) | 4 a 8 |
| Entre controles de un mismo bloque | 16 |
| Entre grupos | 24 a 32 |
| Entre secciones | 64 |

**Las separaciones las pone el contenedor, nunca el elemento.** Las pilas (`.pila-1` a `.pila-8`) usan `gap`.
Ningún elemento lleva `margin` propio. Así no hay elementos corridos 2 px de su eje.

## 4. Alineación

- **Un eje vertical por columna.** Todo cuelga del borde izquierdo de su columna: títulos, rótulos, campos,
  botones y avisos. El único elemento alineado a la derecha es la columna de importes.
- **La columna de contenido tiene un ancho fijo:**
  - 480 px en acceso, recuperación y bienvenida;
  - 608 px en el alta.
- **Equilibrio vertical:** en las pantallas de entrada, el contenido se centra en vertical en el espacio
  disponible. Si no entra, se desplaza desde arriba, con 64 px de margen.
- **Retícula del armador:** el lateral mide 240 px y la barra superior 56 px. El contenido tiene márgenes de
  32 px, las elecciones van en una columna flexible y el resumen en una columna fija de 360 px, separadas por 32.

## 5. Formas y controles

| Elemento | Medida |
|---|---|
| Campo | 40 px de alto, borde de 1 px `--borde-control`, radio de 3 px, relleno horizontal de 12 px |
| Botón | 40 px de alto, relleno horizontal de 24 px, radio de 3 px, N4 en 600 |
| Tarjeta | Borde de 1 px `--borde`, radio de 4 px, relleno de 16 a 24 px |
| Anillo de foco | 2 px `--primario` con 2 px de separación (`outline`) |
| Línea divisoria | 1 px `--borde` |

Los campos y los botones miden 40 px en las vistas web, frente a los 32 y 36 px de `Tema.AltoCampo` y
`Tema.AltoBoton` en Windows Forms. Es una decisión: con texto de 13 px y más aire alrededor, 40 px da
objetivos más cómodos y la misma proporción visual.

### Medidas propias de componentes

Fuera de la escala de espacio solo existen estas medidas, cada una atada a un componente. No se reutilizan para
separar cosas.

| Componente | Medida |
|---|---|
| Logo | Ícono de 32 px; texto de 22 px |
| Íconos de interfaz | 20 px (24 px en las opciones del alta) |
| Interruptor | 32 × 18 px, botón de 14 px |
| Barras (fortaleza, ocupación) | 4 px de alto |
| Número de paso | Círculo de 24 px en el panel, de 20 px en el encabezado del armador |
| Portadas | 528 × 336 px en el muro (inicial de 252 px, separación de 24); 96 × 64 px en el armador (inicial de 48 px) |
| Tarjeta de sector | Mínimo de 176 px de ancho; franja de color de 4 px |
| Línea de estado | Alto reservado de 18 px; signo de 12 px |
| Panel oscuro | 440 px de ancho |

## 6. Estados

Todo lo que se puede tocar tiene estado de reposo, al pasar el mouse, al presionar, con foco de teclado y
deshabilitado.

| Elemento | Al pasar el mouse | Al presionar |
|---|---|---|
| Botón principal | `--primario-encima` | `--primario-presionado` |
| Botón secundario | Borde `--texto-secundario` y fondo `--fondo` | Fondo `--borde` |
| Campo | Borde `--texto-secundario` | — (foco: borde y anillo `--primario`) |
| Tarjeta elegible | Borde `--texto-secundario` | Fondo `--fondo` |
| Enlace | Subrayado más grueso | Color `--primario-presionado` |

- **Transición:** solo color, fondo y borde, 120 ms `ease-out`.
- **Movimiento:** en todo el proyecto hay **dos movimientos, y los dos están en el armador** (§8). Ninguna
  otra cosa se desplaza ni cuenta. Con `prefers-reduced-motion: reduce`, ni esos dos ni ninguna transición.
- **Deshabilitado:** fondo `--fondo`, texto `--texto-secundario`, cursor `not-allowed`. Nunca solo opacidad.

## 7. El motivo: el muro de portadas

El panel oscuro de las pantallas de entrada muestra, detrás del texto, un muro de portadas de eventos
generado igual que `Visual/Componentes/PortadaEvento.cs`:
- degradado a 45° desde `PaletaPortadas[i]` hasta `PaletaPortadas[(i + 2) % 6]`;
- índice `i`: el primer byte del SHA-256 del nombre, módulo 6 (`ColorDelNombre`);
- la inicial en blanco al 28 % (`Tema.MarcaAguaPortada`, alfa 71) abajo a la izquierda, al 75 % del alto.

La cápsula blanca de la categoría que dibuja `PortadaEvento` **se omite en el muro**: atenuada, sigue siendo lo
más claro de cada portada y compite con el texto del panel.

Los nombres son los de la semilla de demostración, con sus índices calculados de antemano en `muro.js`.

**Las portadas del muro son grandes** (528 × 336 px, tres veces las del catálogo): por el panel asoman dos o
tres, cortadas por el borde. Con portadas chicas el muro se leía como una fila de iniciales sueltas. Se probó
también bajar la opacidad al 6 %, y las letras seguían leyéndose (24/11/2026).

El muro queda **atenuado al 12 %** bajo una capa de `--lateral-fondo` al 88 %, con un refuerzo del 50 % hacia
la izquierda, donde está el texto. Así el texto en blanco y en `--lateral-texto` mantiene un contraste mayor
que 7:1. El muro no se mueve.

## 8. El momento de autor: el armador de paquete

Es la única pantalla con un elemento grande y vivo: **el total, en N1, que se recalcula a la vista mientras se
elige**. Tiene los dos únicos movimientos del proyecto:

| Movimiento | Cómo es | Con `prefers-reduced-motion` |
|---|---|---|
| **El total cuenta** | Cuando cambia, sube o baja hasta el valor nuevo en 200 ms, con desaceleración (`1 − (1 − t)³`). Cada cuadro muestra un importe redondeado a centavos, y el último es exactamente el del negocio | Salta al valor nuevo |
| **El renglón nuevo entra** | Solo el renglón que se agrega al desglose: de 8 px más arriba a su lugar, con opacidad de 0 a 1, en 200 ms `ease-out`. Los que ya estaban no se mueven | Aparece sin movimiento |

El descuento por nivel aparece solo cuando hay entradas y el nivel lo da. El total se anuncia una sola vez al
lector de pantalla, cuando termina de contar, no en cada cuadro. Todo lo demás de la pantalla queda quieto.

## 9. Accesibilidad

- Contraste de texto de 4,5:1 como mínimo (los niveles N5 y N6 sobre `--superficie` y `--fondo` lo cumplen con
  `--texto-secundario`).
- Ningún estado se comunica solo con color: siempre va con texto o con un ícono.
- Las validaciones y el total se anuncian con `aria-live="polite"`.
- Todo se puede hacer con el teclado. Escape vuelve, como en el resto de la aplicación.
