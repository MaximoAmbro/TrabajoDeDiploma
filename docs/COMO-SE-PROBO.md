# Cómo se probó EventAura

Resumen para la defensa, al 04/11/2026. Cuenta la historia completa: qué pruebas hay, de
dónde salieron, quién las ejecutó, con qué método, qué falta y cómo se relaciona todo con
la matriz de trazabilidad.

## En pocas palabras

El sistema se probó de tres maneras que se complementan:

1. **Pruebas manuales:** 129 en total, repartidas entre Máximo (64) y JC (65). Las de JC se
   ejecutaron todas el 03/11/2026, salvo dos que necesitan un celular. Las de Máximo todavía
   no se ejecutaron.
2. **Pruebas automáticas:** 43 casos (CP-01 a CP-43) que hoy corren 49 veces, todas
   correctas. Son las que cuenta la matriz de trazabilidad, porque se pueden repetir en
   cualquier momento y dan siempre el mismo resultado.
3. **Inspecciones medidas sobre la aplicación:** el estudio de usabilidad, la verificación
   de la escala de pantalla y la medición de los QR.

**El hilo que une las tres:** cada falla que encontró una prueba manual se corrigió y quedó
como prueba automática, para que no vuelva a aparecer sin que nadie lo note.

---

## 1. De dónde salieron las pruebas manuales

El sistema se construyó en diez secciones (`docs/GUIA-PROMPTS.md`), entre el 08 y el 20 de
octubre de 2026, en los commits de Máximo. La definición de terminado obligaba a una cosa:
lo que no se pudiera probar en el momento había que **anotarlo** en
`docs/PRUEBAS-PENDIENTES.md`, con qué probar, qué resultado esperar y el commit que lo
introducía. Las pruebas de interfaz se postergaron a propósito hasta terminar las diez
secciones, para no probar dos veces pantallas que todavía iban a cambiar.

Al cerrar la sección 10 había **129 pruebas pendientes**, que se repartieron por área para
que cada uno recorriera un flujo entero sin depender del otro:

| | Máximo (64 pruebas) | JC (65 pruebas) |
|---|---|---|
| Área | Propietario: login, eventos y locales, suscripción y preventa, listas de espera del propietario, reportes y su PDF, cierre | Cliente: registro, búsqueda y filtros, armado del paquete, fidelización, reserva, pago, ticket, historial, devolución, lista de espera |
| Secciones del archivo | A, B, C, D, F, G | H, I, J, K, L, M y la 10.5 |
| Lo más pesado | Reportes con el juego de datos de `03-datos-reporte.sql` y el PDF en tres lectores | Pago y vencimiento de la reserva, dos sesiones a la vez, devoluciones |

Además, tres filas quedaron reemplazadas por otras posteriores y ya no se prueban
(«Filas que ya no se prueban», al final del archivo).

## 2. Qué se ejecutó, cuándo, quién y cómo

### La parte de JC: 65 pruebas, el 03/11/2026

- **Quién:** Claude Code, a pedido de JC.
- **Cómo:** manejando la aplicación real con **UI Automation** desde PowerShell. Hizo clics,
  escribió texto, leyó cada MessageBox y capturó cada pantalla. La base se verificó con
  consultas SQL.
- **Sobre qué:** la base de desarrollo, creada de cero con los scripts, a escala 100 %, en el
  commit `9e9bbbe`.
- **Antes de empezar:** la batería automática dio 12 de 12.
- **Resultado** (`docs/RESULTADOS-PRUEBAS-JC.md`):

| Resultado | Cantidad |
|---|---|
| Pasa | 48 |
| Pasa con observación | 10 (las observaciones O-01 a O-11) |
| Falla | 5, todas de presentación de la pantalla de compra a 100 % (los fallos F-01 a F-03) |
| Pendiente | 2: escanear los QR con un celular (7.10 y 10.5) |

**Ninguna falla fue de reglas de negocio:** pagos, reservas, devoluciones y lista de espera
funcionaron bien en todas las pruebas.

### Las correcciones y su reverificación: 03 y 04/11/2026

Las fallas y observaciones se corrigieron en dos sesiones de **Codex**:
`docs/CORRECCIONES-JC-2026-11-03.md` y `docs/VERIFICACION-JC-2026-11-04.md`.

- **Prueba automática por cada corrección.** Cada corrección quedó cubierta por una prueba
  automática de interfaz: son los casos CP-13 a CP-25, que abren los formularios reales y
  responden sus diálogos.
- **Reverificación JC-R01 a JC-R11.** Lo que había que reverificar se anotó con esos códigos
  en `PRUEBAS-PENDIENTES.md`:

| Reverificación | Estado | Cómo |
|---|---|---|
| JC-R02 a JC-R09 | **Comprobadas** | Automáticas: CP-13 a CP-25 |
| JC-R01, compra a 125 % y 150 % | **Abierta**. Verificada solo a 100 % real | La simulación de CP-25 no reproduce el escalado real. Por decisión de JC, la falta de compatibilidad con DPI queda como limitación de RNF-03, y JC lo confirma a mano |
| JC-R10, QR con el celular | **Abierta** | Las medidas cumplen la norma ISO/IEC 18004; falta el escaneo |
| JC-R11, recorrido del resto de las pantallas | **Abierta** | Manual |

Claude Code revisó esos cambios el 04/11 y los commiteó en bloques, con la aprobación de JC.

### La ampliación de las pruebas automáticas: 04/11/2026

La matriz de trazabilidad mostraba que solo 3 de los 13 requerimientos tenían pruebas
(brecha G-02). Claude Code escribió encargos cerrados y Codex los implementó:

| Tanda | Casos | Qué cubre | Resultado |
|---|---|---|---|
| T-01 a T-18 | CP-26 a CP-42, más el etiquetado de CP-11 a CP-25 | Registro, login y bloqueo; filtros; topes del paquete; pago aprobado, rechazado y vencido; reportes; los hallazgos de usabilidad H-02 a H-05 | 47 correctas y 1 omitida (CP-34) |
| T-19 y T-20 | CP-43; CP-34 corregida | La omisión era un error de la preparación de la prueba, no de la aplicación. De paso se unificó el plazo de la reserva (RN-15) | 49 de 49 |
| T-21 | — | La semilla reproduce el ejemplo del documento («Duki en Rosario», $99.625) | 49 de 49 |

### Inspecciones medidas sobre la aplicación: 04/11/2026

Las hizo Claude Code. Los resultados están en `docs/`, y los scripts para repetirlas en
`tools/`.

- **Escala de pantalla:** a 100 % real, la compra con el paquete máximo entra completa.
- **QR de los tickets:**
  - módulo de 0,95 mm;
  - 4 módulos de zona de silencio;
  - corrección de errores Q;
  - contraste de 100 %.

  Cumplen la norma.
- **Estudio de usabilidad (simulado):**
  - **Qué se midió:** la aplicación real, abriendo sus formularios y actuando sobre ellos.
  - **Con UI Automation sobre el ejecutable:** se confirmó que cerrar una pantalla con la X
    deja la aplicación corriendo sin ventanas.
  - **Qué se derivó de esas mediciones:** los resultados de seis perfiles.
  - **Resultado:** trece hallazgos (U-01 a U-13).

---

## 3. Qué sigue sin ejecutarse y por qué

| Qué | Cuántas | Por qué no se ejecutó | De quién |
|---|---|---|---|
| Parte de Máximo | 64 | No hay resultados en el repositorio | **Máximo** |
| JC-R01: confirmar a mano la compra a 125 % y 150 % | 1 | Hay que cambiar la escala de Windows | **JC** |
| JC-R10: escanear dos QR con el celular | 1 | Hace falta un celular | **JC** |
| JC-R11: recorrer el resto de las pantallas con el teclado | 1 | No entró en las tandas automáticas | **JC** |
| CX-02: dejar vencer una reserva por reloj | 1 | Tarda 10 minutos reales. La regla ya está probada (CP-34 y CP-43); esta prueba verifica el temporizador y el aviso de punta a punta | **JC** |

**Sobre la parte de Máximo:** buena parte de lo que cubre ya tiene prueba automática, aunque
eso no reemplaza mirar las pantallas:
- eliminar y suspender eventos: CP-21;
- reportes: CP-35 a CP-38;
- lista de espera: CP-37.

Lo que solo puede verificar una persona es lo visual: el PDF de los reportes en tres
lectores, las pantallas del propietario y los reportes con volumen de datos.

---

## 4. Cómo se relaciona todo con CP-01 a CP-43 y la matriz

- **La matriz cuenta solo las pruebas automáticas.** Un caso CP se ejecuta igual cada vez y
  deja registro, así que puede respaldar «este requerimiento está verificado». Una prueba
  manual vale el día que se hizo.
- **De dónde salió cada grupo de casos:**

| Casos | Origen |
|---|---|
| CP-01 a CP-10 | Los define el documento entregable, en la sección de la lógica de descuento. Se automatizaron en la sección 10 |
| CP-11 y CP-12 | Ya existían en el código (la verificación de $99.625 y el alto máximo de las pantallas); se les dio ID el 04/11 |
| CP-13 a CP-25 | Nacieron de las **pruebas manuales de JC**: cada falla u observación corregida tiene su caso |
| CP-26 a CP-43 | Nacieron de la **matriz**: cubren los requerimientos y reglas que no tenían prueba, y los hallazgos de usabilidad H-02 a H-05 |
| CP-44 a CP-50 | Están encargados a Codex: seguridad del alta, cierre de sesión, cierre con la X, indicación de la cantidad y «Comprar» sin entradas |

- **Cobertura de la matriz:** 12 de los 13 requerimientos (el que falta, RF-09, quedó
  excluido), 27 de 38 reglas de negocio y 6 de 6 hallazgos del primer estudio de usabilidad.
- **Las pruebas manuales siguen siendo necesarias** para lo que una prueba automática no
  puede juzgar: si algo se ve bien, si se entiende, si un QR impreso se lee con una cámara.

---

## 5. Qué le corresponde a cada uno

| Persona | Qué le falta | Dónde está |
|---|---|---|
| **Máximo** | Sus 64 pruebas. Además, mirar la pantalla de compra en su PC al 125 %, porque se rediseñó | `docs/PRUEBAS-PENDIENTES.md`, secciones A, B, C, D, F y G |
| **JC** | JC-R01 (a mano), JC-R10 (celular), JC-R11 (recorrido con teclado) y CX-02 (reloj) | `docs/PRUEBAS-PENDIENTES.md`, al final |

---

## 6. Cómo contarlo en la defensa

> «Construimos el sistema por secciones y cada sección dejó anotadas sus pruebas de interfaz
> pendientes, con qué probar y qué resultado esperar: fueron 129. Las del flujo del cliente
> se ejecutaron manejando la aplicación real con UI Automation, y encontraron cinco fallas de
> presentación y ninguna de reglas de negocio. Cada falla corregida quedó como prueba
> automática. Después usamos la matriz de trazabilidad para ver qué requerimientos no tenían
> prueba, y escribimos casos hasta cubrir los doce que están en alcance. Hoy son 43 casos que
> corren 49 veces sin fallas. Además, medimos la usabilidad sobre la aplicación real, y eso
> nos dio los hallazgos que estamos corrigiendo ahora.»
