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
- **Ancho de línea:** el texto corrido no pasa de 60 caracteres (`max-width: 60ch`), dentro del tope de 65ch de §10.4.

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

- **Transición:** solo color, fondo y borde, 120 ms. La curva y el resto de las reglas de movimiento están en §10.
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

## 10. Movimiento, interacción y lectura

Sale del método de Emil Kowalski en «Agents with Taste» (emilkowal.ski/ui/agents-with-taste, leído el
25/11/2026). **La idea del método es escribir el porqué de cada regla**: con el porqué, quien construye una
pantalla nueva puede decidir bien un caso que la regla no prevé.

**De dónde sale cada porqué:**
- **(EK)** Lo dice el artículo.
- **(nuestro)** El artículo da el valor sin razón, o la regla no está en el artículo. La razón es nuestra.

La columna **«Hoy»** dice si `sistema.css` y las maquetas ya cumplen la regla.

### 10.1 Curvas

El artículo resuelve la curva con un árbol de decisión, para que nadie tenga que inventarla: «This is strict so
the agent doesn't have to guess or make up its own rules» (EK).

| Qué se mueve | Curva | Por qué | Hoy |
|---|---|---|---|
| Algo que **entra o sale** de la vista | `ease-out` | (nuestro) Arranca rápido y frena al llegar. La respuesta se ve en el primer cuadro, que es cuando la persona mira, y el final suave le dice al ojo dónde quedó. Un `ease-in` haría lo contrario: un arranque lento, que se percibe como demora. Es también la curva por omisión del árbol (EK) | Cumple: el renglón nuevo del desglose |
| Algo que **se mueve o cambia de forma** dentro de la pantalla | `ease-in-out` | (nuestro) El objeto ya está a la vista antes y después. Acelerar y frenar de forma simétrica imita cómo se mueve una cosa física que se desplaza de un lugar a otro, sin saltos en los extremos | Cumple: la perilla usa `--desplazamiento` (120 ms `ease-in-out`) |
| Un cambio **al pasar el mouse** | `ease` | (nuestro) Es un cambio de color o de fondo, sin recorrido. `ease` arranca un poco más suave que `ease-out`, y un efecto que se dispara con solo cruzar el mouse no salta a la vista | Cumple: `--transicion` es 120 ms `ease` |
| Algo **continuo** (una barra de progreso, un indicador que gira) | `linear` | (nuestro) No tiene principio ni fin que marcar. Cualquier aceleración se leería como un tirón en cada vuelta | No hay movimiento continuo |
| **El total que cuenta** en el armador | `ease-out` (curva `1 − (1 − t)³`) | (nuestro) El valor llega a un número nuevo: es una entrada, no un movimiento continuo. Los primeros cuadros cubren casi toda la diferencia, y los centavos se asientan al final, donde la vista los lee | Cumple |

### 10.2 Duraciones

| Tipo | Duración | Por qué | Hoy |
|---|---|---|---|
| Microinteracción (estados de un botón, un interruptor, el borde de un campo) | 100 a 150 ms | (nuestro) Por debajo de unos 100 ms un cambio se percibe como instantáneo y la transición no aporta nada. Por encima de 150 ms, en algo que se toca decenas de veces, se empieza a sentir como espera | Cumple: 120 ms |
| Interfaz (un renglón que entra, un desplegable, un aviso) | 150 a 250 ms | (nuestro) El objeto recorre una distancia y hay que poder seguirlo con la vista, sin que demore la tarea | Cumple: 200 ms el renglón, 200 ms el total |
| Modal o panel lateral | 200 a 300 ms | (nuestro) Es el elemento más grande y el que más recorre. El artículo pide que «Larger elements animate slower than smaller ones» y «Match duration to distance» (EK) | No hay modales en las vistas web |
| Tope | **Nunca más de 300 ms** | «UI animations should stay under 300ms» (EK). (nuestro) Pasado ese punto, la animación deja de acompañar la acción y pasa a ser algo que hay que esperar | Cumple |
| Salidas | **Alrededor de un 20 % más rápidas** que su entrada | «Exit animations can be ~20% faster than entrance» (EK). (nuestro) Cuando algo se va, la persona ya decidió y la atención está en lo que sigue | Cumple: el renglón que sale del desglose se desvanece en 160 ms (`--salida`), contra los 200 de la entrada (`--entrada`), y sin desplazarse, porque ya no es lo que se mira |

### 10.3 Detalles

| Regla | Valor | Por qué | Hoy |
|---|---|---|---|
| Botón presionado | `transform: scale(0.97)` en `:active` | (nuestro) El botón cede un poco bajo el puntero, como un botón físico, y confirma el toque antes de que la aplicación responda. Con 3 % se siente sin que el texto se deforme | Cumple en `.boton` y en los botones de cantidad. **No se aplica a las tarjetas elegibles** (`.eleccion`, `.opcion`, `.interruptor`): en una tarjeta de 480 px, el 3 % mueve los bordes 7 px y se lee como un salto, no como una pulsación. Esas tarjetas marcan la presión con el fondo |
| Lo que entra | Arranca en `scale(0.95)`, **no en 0** | «A higher initial value resembles the real world more. Just like a balloon, even when deflated it has a visible shape, it never disappears completely» (EK). Un elemento que nace de 0 parece salir de la nada | No aplica todavía: nada entra escalando. El renglón del desglose entra con desplazamiento |
| Efecto al pasar el mouse | Se anima un **hijo**, no el elemento que recibe el mouse | (nuestro) Si el elemento que detecta el mouse se mueve o se achica, el puntero puede quedar afuera. Entonces el efecto se apaga, el elemento vuelve, el puntero queda adentro otra vez, y así: parpadea. Animando un hijo, el área que detecta el mouse queda quieta | Cumple: los efectos al pasar el mouse solo cambian color y borde, no tamaño |
| Área de clic | El artículo pide **44 px como mínimo**. **Nos apartamos a propósito: 40 px** | **Por qué no seguimos esta regla.** Los 44 px son una guía de Apple para pantallas táctiles, pensada para dedos. EventAura es una aplicación de escritorio que se usa con mouse o touchpad, y el criterio para puntero es WCAG 2.2, 2.5.8 «Target Size (Minimum)», que pide 24 × 24 px como mínimo. Los 40 px de campos, botones y controles de cantidad lo superan de sobra. Subirlos a 44 empujaría varios formularios por encima del tope de 790 px de alto (`CLAUDE.md`), que es un problema real y ya ocurrió una vez. Conocer la regla y apartarse con un motivo escrito es parte del método | Desvío deliberado (decisión de JC, 25/11/2026). **Lo que sí se cuida:** ningún control queda por debajo de 24 px. Los enlaces de texto y la casilla «Mostrar contraseña» tienen el alto de su renglón; cuando estén al lado de otro control, que haya 24 px de separación entre sus áreas |

### 10.4 Lectura

| Regla | Valor | Por qué | Hoy |
|---|---|---|---|
| Largo de línea | Alrededor de **65ch** como máximo | «Cap body text at about 65ch instead of stretching full width so line length stays comfortable to read» (EK). (nuestro) Con líneas más largas, al terminar una el ojo pierde el comienzo de la siguiente | Cumple: `.medida` usa 60ch |
| Cifras en columnas de precios | `font-variant-numeric: tabular-nums` | «Apply tabular-nums to price columns so digits align and the column reads cleanly» (EK). (nuestro) En el armador, además, el total cambia de valor: con dígitos de ancho fijo el número no baila mientras cuenta | Cumple: `.cifra`, el desglose y el total |
| Subrayado | **Solo en enlaces.** Para destacar texto que no es enlace, peso o color | «Reserve underlines for links… so underline stays a reliable affordance and people are not tempted to click inert copy» (EK) | Cumple: solo `.enlace` subraya |

### 10.5 Movimiento reducido

Con `prefers-reduced-motion: reduce` no hay transiciones, conteo ni deslizamiento: todo cambia de inmediato.
Esta regla no está en el artículo; es nuestra. Para quien activa esa opción, el movimiento no es un detalle de
calidad sino algo que puede marear o distraer, y la información tiene que llegar igual sin él. Hoy se cumple:
`sistema.css` anula toda transición y animación, y el armador no cuenta.

### 10.6 Estado

Aplicado en `sistema.css`, `armador-paquete.css` y `armador-paquete.js` el 25/11/2026, en las cinco pantallas:
- **Hover:** con `ease`.
- **Perilla del interruptor:** con `ease-in-out`.
- **Presión:** `scale(0.97)` en los botones.
- **Desglose:** el renglón que sale se desvanece un 20 % más rápido que lo que entra.

El área de clic de 44 px **no** se aplica, por la razón de §10.3.

**Robustez de los dos movimientos del armador.** No se confía en que la animación llegue a correr. Una vista web
oculta, la pestaña en segundo plano o el movimiento reducido pueden impedir que se dispare `animationend` o
que avance `requestAnimationFrame`. Por eso:
- el renglón que sale se quita a los 220 ms aunque no haya terminado de desvanecerse;
- la marca de «nuevo» se limpia a los 260 ms;
- el total escribe su valor exacto a los 260 ms si el conteo no llegó.

Se encontró al probarlo con Edge sin ventana el 25/11. Sin estos respaldos, los renglones que salían quedaban
en la lista duplicados y el total se quedaba en $ 0,00.
