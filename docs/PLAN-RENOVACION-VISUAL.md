# Plan de la renovación visual de EventAura

## 0. Plan vigente: orden, dependencias y horas

**Cómo se planifica, por decisión de JC:**
- por orden y dependencias, **sin fechas**;
- cada tarea dice quién la hace, de qué depende y cuántas **horas estimadas** lleva;
- **la entrega real es a principios de diciembre.** Se trabaja al máximo ritmo para terminar
  antes y tener tiempo de corregir detalles o agregar lo que pida el docente;
- **nada espera a Máximo:** quedó fuera del camino crítico, y lo que dependía de él lo hace JC;
- **los agentes trabajan en paralelo** y nunca escriben los mismos archivos (`CLAUDE.md`);
- **se hace 6B completo:** la limpieza, el rediseño de las 18 y los tres extras visuales. Los
  recortes quedan en reserva (`ENCARGO-6B-BORRADOR.md`, sección 5).

Las secciones 1 a 9 son el análisis de base del 05/11: siguen valiendo como fundamento. La
sección 10 es historia (cronogramas con fechas, ya no vigentes).

### 0.1 La secuencia

La columna «Depende de» manda: una tarea arranca cuando terminaron todas las que nombra.
Dentro de cada agente, el orden de la tabla es el orden de trabajo.

| # | Tarea | Quién | Depende de | Horas estimadas |
|---|---|---|---|---|
| **Pruebas del grupo A** (`TANDAS-PRUEBAS-64.md`) | | | | |
| S-1 | T-44: suscripción y preventa | Codex | — (en curso) | 5 a 7 |
| S-2 | T-45: listas de espera y menús | Codex | S-1 | 3 a 4 |
| S-3 | Las dos autorizaciones de T-46: la carpeta de exportación configurable en `Visual/`, y cargar `03-datos-reporte.sql` en la base de pruebas | JC | — | 0,2 |
| S-4 | T-46: reportes | Codex | S-2, S-3 | 5 a 7 |
| **Seguridad** (`ENCARGO-SEGURIDAD-BORRADOR.md`; hallazgos en `AUDITORIA-SEGURIDAD.md`) | | | | |
| SG-0 | Decisiones D-1 (cambios en `BaseDatos/`), D-2 («Cambiar contraseña» en lugar de «¿Olvidaste tu contraseña?») y D-3 (cuándo va SEG-3) | JC | — | 0,2 |
| SG-1 | SEG-1: cambiar la contraseña exige la actual (**alta**) | Codex | S-2, SG-0 | 3 a 4 |
| SG-2 | SEG-2: control de ingreso, el QR vale para un único ingreso (**alta**) | Codex | S-2, SG-0 | 8 a 12 |
| SG-3 | SEG-4 y SEG-5: el perfil Administrador no se asigna desde un método público; contraseñas de los scripts como hash | Codex | SG-1 | 2 a 3 |
| SG-4 | SEG-3: el negocio verifica quién llama. **Según D-3:** acá (recomendado) o después de S-14 | Codex | SG-3 | 8 a 12 |
| SG-5 | Prueba manual del control de ingreso con el celular (y un lector USB, si hay) | JC | SG-2 | 0,1 |
| SG-6 | SEG-6 a SEG-10, las bajas | Codex | — (en cualquier hueco) | 2 |
| **6B** (`ENCARGO-6B-BORRADOR.md`, versión 3) | | | | |
| S-5 | T-41: el piloto corregido (R-1 a R-4, foco, `Disposicion.cs`, prueba de serialización). **Si S-3 todavía no está, va antes que S-4** | Codex | S-2, SG-1, SG-2, SG-3 (y SG-4 si D-3 lo pone acá) | 6 a 8 |
| S-6 | Revisar el código de T-41; adaptar `Captura.exe` a DPI; capturas del piloto a 100, 125 y 150 % | Claude Code | S-5 | 3 a 4 |
| S-7 | Aprobar las capturas y commitear T-41 | JC | S-6 | 0,25 |
| S-8 | **Verificar el piloto en Visual Studio** (instrucciones en el encargo, sección 4) | JC | S-7 | 1 a 1,5 (con la instalación) |
| S-9 | T-47: limpieza de los 18 Designer, con los roles | Codex | S-5 | 4 a 6 |
| S-10 | T-48: lo que pisa el tema en ejecución, y CP-63 en las 19 | Codex | S-9 | 3 a 4 |
| — | **Control de JC** (0.3) | JC | S-10 | 0,25 |
| S-11 | T-39: el cliente, con el patrón en Eventos, Pago, Confirmación e Historial | Codex | S-10, S-8 | 10 a 14 |
| S-12 | T-49: los tres extras visuales (indicador de pasos, TOTAL destacado, confirmación de éxito) | Codex | S-11 | 10 a 15 |
| S-13 | T-38: la entrada | Codex | S-10, S-8 | 4 a 6 |
| S-14 | T-40: la administración | Codex | S-10, S-8, S-4 (las pruebas de reportes protegen el rediseño de `frmReportes`) | 8 a 12 |
| S-15 | T-50: el PDF de los reportes con los tokens | Codex | S-4 | 1 |
| S-16 | Capturas de cada tarea de rediseño contra la referencia del piloto, para que JC apruebe | Claude Code | S-11, S-12, S-13, S-14 (una por tarea) | 4 a 6 |
| — | Las pruebas que cambian a propósito (CP-21 y las de V-6) | Codex | Repartidas en S-11 a S-14 | 2 a 3 |
| **Cierre de las pruebas** | | | | |
| S-17 | AH-1 y AH-2 (10.2 y 10.3) como scripts | Claude Code | S-4 | 1 |
| S-18 | Copiar los reportes de Codex a `docs/` y actualizar los casos, la matriz, `COMO-SE-PROBO.md` y `PRUEBAS-PENDIENTES.md` | Claude Code | S-4 | 2 a 3 |
| S-19 | JC-R10: escanear dos tickets nuevos con el celular. **Ya se puede:** los PDF están en `TestResults/6A/Tickets/` | JC | — | 0,2 |
| S-20 | La lista B y C de las 64 (PDF del reporte y pantallas del propietario) | JC | S-4, S-14 | 0,4 |
| **Después de 6B** | | | | |
| S-21 | Repetir las verificaciones sobre la aplicación nueva: escala real (125, 150 y 175 %), teclado, excepciones y reserva | Claude Code | S-12, S-13, S-14 | 3 a 4 |
| S-22 | Regenerar las figuras del instructivo | Claude Code | S-12 | 1 |
| S-23 | Corregir lo que encuentre S-21 | Codex | S-21 | Sin estimar: depende de lo que aparezca |
| S-24 | **Tercera medición de usabilidad** e informe | Claude Code | S-21 (y S-23, si hubo) | 4 a 6 |

*Nota:* S-8 suma 45 a 60 min de instalación y 20 a 30 min de verificación.

**El camino crítico:** SG-1 → SG-2 → SG-3 → (SG-4) → S-5 → S-9 → S-10 → S-11 → S-12 → S-13
→ S-14 → S-21 → S-24. La seguridad suma 13 a 19 h de Codex al principio, o 21 a 31 si SG-4
también va antes de 6B. El detalle de abajo es solo de 6B.
- **Codex:** 45 a 65 h estimadas.
- **Claude Code:** 7 a 10 h.
- **JC:** S-8 tiene que estar antes de S-11.

**Totales estimados:**
- **Codex:** 61 a 87 h (13 a 18 de pruebas, 48 a 69 de 6B), más S-23;
- **Claude Code:** 18 a 25 h;
- **JC:** unas 3 h de tareas propias, más la aprobación de las capturas.

**Fuera de la secuencia, sin estimar todavía:**
- **Seguridad: ahora está en la secuencia** (bloque SG-0 a SG-6), con 23 a 33 h estimadas de
  Codex que se suman a los totales de abajo. Las dos altas (el QR sin control de ingreso y el
  cambio de contraseña sin autenticar) van antes de 6B.
- **El documento.** Está en pausa por decisión de JC. Las correcciones están en
  `CORRECCIONES-DOCUMENTO.md`, y el plan B2 (8 a 12 h estimadas) en
  `DOCUMENTO-ENTREGABLE.md`. Los anexos T01 a T08 los maneja JC con el docente.

### 0.2 Reglas del trabajo en paralelo que afectan al orden

- **Las herramientas de Claude Code no corren mientras Codex corre pruebas:**
  - las que ejecutan la batería (S-17), porque las dos usan `BddEventAura_Pruebas`, que se
    borra y se recrea en cada corrida;
  - las que abren pantallas o cambian la escala de Windows (S-6, S-16, S-21, S-24), porque
    comparten el escritorio y una prueba de interfaz de Codex fallaría.

  **JC avisa cuándo Codex no está probando.**
- **`ESTADO-AGENTES.md` y `TAREAS-CODEX-PRUEBAS.md`:**
  - no se tocan mientras Codex trabaja;
  - el encargo de 6B pasa a `TAREAS-CODEX-PRUEBAS.md` en una pausa de Codex, antes de S-5.

### 0.3 El control de JC al cerrar T-47 y T-48

**Por tarea terminada, no por fecha.** Cuando Codex entregue T-48, se compara su reporte con
estos números del encargo:

| Qué se compara | Estimado | Dónde lo da Codex |
|---|---|---|
| Horas reales de T-47 | **4 a 6 h** | Reporte de T-47 |
| Horas reales de T-48 | **3 a 4 h** | Reporte de T-48 |
| **Horas reales de las dos juntas** | **7 a 10 h** | Suma de las dos |
| Líneas de fuente y color borradas de los 18 Designer | **314** (medidas el 10/11) | Reporte de T-47 |
| Pantallas limpiadas en T-47 | **18** | Reporte de T-47 |
| Asignaciones de estilo en ejecución reemplazadas | **23 líneas en 5 pantallas** (Historial 8, Reportes 6, Listas de espera 5, Pago 3, Mis locales 1), más la revisión de los pesos de columna de Eventos (auditoría de Codex) | Reporte de T-48 |
| Filas nuevas de la batería | **25**: 7 de T-41 y 18 de T-48 | La batería al cerrar T-48 debe dar la de T-46 + 25, todas correctas |

**Cómo se lee:**
- **El factor** es las horas reales de las dos juntas divididas por **10** (el máximo
  estimado).
- **Factor de 1 o menos:** el plan sigue igual.
- **Factor entre 1 y 1,5:** se multiplica el resto de 6B por ese factor (S-11 a S-15,
  hoy 33 a 48 h) y se sigue sin recortar.
- **Factor mayor que 1,5:** el encargo subestimó la limpieza. JC decide si saca algo de la
  reserva.
- **Si las líneas encontradas se apartan más de un 20 % de las 314,** el reporte tiene que
  explicar por qué: o la medición del 10/11 estaba mal, o se borró algo de más.

---

## 1. Diagnóstico: por qué hoy se ve «de facultad»

No es una cuestión de gusto. Hay cinco causas concretas, todas medidas:

| # | Qué pasa | Dato | Efecto |
|---|---|---|---|
| D-1 | **Tipografías prestadas** | Los textos usan «Nirmala UI», una fuente para escrituras de la India, siempre en negrita (85 declaraciones). Los campos de texto usan «MS Gothic», una fuente japonesa (18). El resumen usa Consolas | El Mail se ve monoespaciado; todo en negrita no tiene jerarquía |
| D-2 | **Gris ilegible** | 33 rótulos en gris `#A4A5A9` sobre blanco: contraste 2,5:1. El mínimo de legibilidad (WCAG AA) es 4,5:1 | Los textos parecen deshabilitados (se ve en la figura 2: los servicios y las filas de la grilla) |
| D-3 | **Dos escalas de diseño mezcladas** | `AutoScaleMode.Dpi` en las 19 pantallas: 11 diseñadas a 120 DPI (125 %) y 8 a 144 DPI (150 %) | Márgenes y tamaños distintos entre pantallas, aunque nadie los haya elegido |
| D-4 | **Sin sistema** | Ninguna clase de estilos; 119 colores escritos a mano en los diseñadores; 7 tamaños de letra distintos para títulos | Cada pantalla resuelve lo mismo de otra forma |
| D-5 | **Borroso en notebooks** | La aplicación no declara compatibilidad con DPI: a 125 % o 150 % Windows estira la imagen (JC-R01, capturas en `docs/capturas/escalado/`) | En la notebook del docente, todo el texto se ve desenfocado |

Además: ícono genérico de Windows en la barra de título; ticket PDF con fondo negro (gasta
tinta y desentona); el resumen del paquete es un texto monoespaciado alineado con espacios.

**Lo que ya está bien y se conserva:** el violeta de marca `#7456AE` tiene 5,7:1 de contraste
sobre blanco (pasa AA), los botones planos, la estructura de las pantallas y el orden de la
compra.

---

## 2. Qué se puede lograr con WinForms y qué no

WinForms sobre .NET Framework 4.8 es una tecnología de 2002 con mantenimiento. El techo es real:

**Se puede, con poco riesgo:**
- tipografía coherente (Segoe UI, la de Windows), jerarquía de tamaños y pesos;
- una paleta con contraste correcto, aplicada desde un solo lugar;
- espaciado consistente en una grilla de 8 px;
- botones planos con estados (normal, encima, presionado, deshabilitado, con foco);
- grillas (`DataGridView`) prolijas: encabezados propios, filas altas, sin líneas verticales,
  selección en el color de marca, números a la derecha;
- tarjetas (paneles blancos con borde fino sobre un fondo gris claro), encabezado de ventana,
  pie con las acciones;
- avisos dentro de la pantalla (banda de color con ícono) en lugar de ventanas emergentes para
  validaciones;
- ícono propio y ticket PDF rediseñado.

**Se puede, con costo o con riesgo:**
- **texto nítido en notebooks:** declarar compatibilidad con DPI. `App.config` está
  protegido, pero el modo «system aware» se declara en el manifiesto (`app.manifest`), sin
  tocarlo. Cambia el escalado de las 19 pantallas: hay que verificarlas a 125 % y 150 %, cosa
  que ahora es automática (`tools/VerificacionesManuales/Escala.ps1`). **Es la mejora visual
  más grande para el docente, y la decisión que tomaste el 04/11 fue anterior a la
  renovación: te propongo revisarla** (ver la sección 9);
- bordes redondeados en botones y tarjetas: hay que dibujarlos (`GraphicsPath`), y WinForms no
  suaviza el borde contra el fondo. Se ven bien solo sobre fondo liso;
- colores de la barra de título: solo en Windows 11 (`DwmSetWindowAttribute`).

**No se puede, o no vale la pena en un mes:**
- animaciones y transiciones fluidas;
- transparencias reales, sombras, efecto Mica o acrílico confiables;
- estilizar los controles nativos completos: el desplegable del `ComboBox`, el calendario del
  `DateTimePicker`, las barras de desplazamiento y el `MessageBox` conservan el aspecto de
  Windows;
- un diseño «web» (diseño adaptable, tarjetas que se reacomodan).

**Conclusión honesta:** el techo alcanzable es «aplicación de escritorio de Windows prolija y
coherente», al nivel de una herramienta administrativa profesional. No va a parecer una
aplicación web moderna, y no hace falta: para el docente, la diferencia entre «de facultad» y
«profesional» está en D-1 a D-5, y todas tienen solución.

---

## 3. Los tres caminos

| | A. Controles estándar con sistema de diseño | B. Biblioteca de terceros | C. Controles dibujados a mano |
|---|---|---|---|
| Qué es | Los mismos controles de WinForms, con un tema central (tokens de color, tipografía y espaciado) que aplica la base común `FormularioEventAura`, y tres o cuatro componentes propios mínimos (botón primario, botón secundario, tarjeta, banda de aviso) | Krypton Toolkit (libre, aspecto Office), MaterialSkin 2 (libre, aspecto Material de Android) o comerciales (Guna UI2, DevExpress, Telerik, Syncfusion) | Botones, campos, tarjetas y grillas como `UserControl` con `OnPaint` propio |
| Aspecto final | Windows prolijo y coherente | El de la biblioteca: Krypton se ve «Office 2013»; MaterialSkin, «Android»; los comerciales, muy pulidos | El que se diseñe; el más libre |
| Ventajas | Una sola fuente de verdad; el diseñador de Visual Studio lo muestra; las pruebas que buscan `Button` o `TextBox` siguen funcionando; Máximo no necesita instalar nada; no empeora la accesibilidad | Mucho aspecto «de regalo»; controles que WinForms no trae | Control total del aspecto |
| Desventajas | El techo de la sección 2 | Hay que reemplazar los controles en los 19 diseñadores. Las pruebas buscan `Control<Button>(form, "btnComprar")`: un `KryptonButton` no es un `Button`, así que se rompe la mayoría de las 44 ejecuciones de interfaz. La base pasa a ser la de la biblioteca, y choca con `FormularioEventAura`. Dependencias NuGet en un `.csproj` viejo. Licencias que verificar en los comerciales | Accesibilidad: un control dibujado no se anuncia a UI Automation salvo que se programe (`AccessibleObject`). Empeora U-12 y el recorrido con teclado. Foco, teclado, alto contraste y DPI hay que hacerlos a mano |
| Riesgo | **Bajo** | **Alto**: toca todo a la vez, a un mes de la entrega | **Alto** |
| Horas | **~70 h** con todo incluido (sección 7) | 100 a 130 h | 130 a 170 h |

**Recomiendo A.** Es el único camino que entra en el mes con margen, no rompe las 73
pruebas de forma masiva, no agrega dependencias que Máximo tenga que instalar y mejora la
accesibilidad en lugar de empeorarla. Además, **la base común de T-24 ya existe**: las 19
pantallas heredan de `FormularioEventAura`. El tema se aplica ahí una vez y llega a todas.
Con solo la fase 0 (sección 5), cada pantalla gana tipografía, color, grillas y botones nuevos,
antes de retocar ninguna a mano.

---

## 4. Sistema de diseño (camino A)

> **Reemplazada por `docs/SISTEMA-DISENO.md`** (05/11/2026), que es la versión para aplicar,
> con los contrastes calculados y lo que WinForms no da bien. Esta sección queda como la
> propuesta original.

Todo vive en un archivo, `Visual/Tema.cs`, con constantes. Ninguna pantalla escribe un color o
una fuente: los pide al tema. Medidas en píxeles a 100 %.

### 4.1 Tipografía

Segoe UI, la fuente de Windows desde Vista: está en cualquier PC donde se instale. Segoe UI
Variable es más linda, pero solo existe en Windows 11.

| Uso | Fuente | Tamaño | Peso | Color |
|---|---|---|---|---|
| Marca (login) | Segoe UI | 26 pt | Semibold | Primario |
| Título de pantalla | Segoe UI | 18 pt | Semibold | Texto |
| Título de sección o tarjeta | Segoe UI | 12 pt | Semibold | Texto |
| Cuerpo, campos, celdas, botones | Segoe UI | 10 pt | Regular (botones: Semibold) | Texto |
| Rótulo de campo, encabezado de grilla | Segoe UI | 9 pt | Semibold | Texto secundario |
| Ayuda, leyendas, pie | Segoe UI | 9 pt | Regular | Texto secundario |
| Importe destacado (TOTAL) | Segoe UI | 14 pt | Semibold | Texto |

Reglas: **no más de un título de 18 pt por ventana**; la negrita se usa para jerarquía, no
para todo; se eliminan Nirmala UI, MS Gothic y Consolas. Los importes se alinean a la derecha en una tabla, así que no hace
falta fuente monoespaciada (se comprueba en la fase 2 con CP-13).

### 4.2 Paleta

| Token | Color | Contraste sobre blanco | Uso |
|---|---|---|---|
| `Primario` | `#7456AE` | 5,7:1 | Botón principal, enlaces, foco, temporizador normal. Es el violeta actual |
| `PrimarioEncima` | `#634699` | — | Botón principal con el mouse encima |
| `PrimarioPresionado` | `#523A80` | — | Botón principal presionado |
| `PrimarioSuave` | `#EFEAF8` | — | Fila seleccionada, celda editable, fondo de ayuda |
| `Texto` | `#1F1F24` | 16:1 | Todo el texto normal |
| `TextoSecundario` | `#5C5F66` | 6,4:1 | Rótulos, ayudas, encabezados de grilla. **Reemplaza a `#A4A5A9`** |
| `TextoDeshabilitado` | `#9A9CA3` | — | Solo controles deshabilitados (WCAG los exceptúa) |
| `Fondo` | `#F6F5FA` | — | Fondo de la ventana, detrás de las tarjetas |
| `Superficie` | `#FFFFFF` | — | Tarjetas, campos, grillas |
| `Borde` | `#DCDAE3` | — | Borde de tarjetas, campos y grillas |
| `FondoDeshabilitado` | `Gainsboro` `#DCDCDC` | — | Botón deshabilitado. Es el actual: CP-22 y CP-23 no cambian |
| `Exito` / `ExitoFondo` | `#1E7B4A` / `#E7F4EC` | 5,3:1 | Compra confirmada, operación correcta |
| `Advertencia` / `AdvertenciaFondo` | `#8A5A00` / `#FFF4D6` | 5,8:1 | Último minuto, cupo bajo, seguro no contratado |
| `Error` / `ErrorFondo` | `Firebrick` `#B22222` / `#FCE8E6` | 6,6:1 | Validaciones, rechazo, reserva vencida. Es el rojo actual del temporizador: CP-39 no cambia |

**Regla:** el primario se usa en **un solo botón por pantalla**, el de la acción principal.
Todo lo demás es secundario.

### 4.3 Espaciado

Escala de 8 px: **4 · 8 · 12 · 16 · 24 · 32 · 48**.

| Dónde | Medida |
|---|---|
| Margen interior de la ventana | 24 |
| Entre secciones o tarjetas | 24 |
| Relleno interior de una tarjeta | 16 |
| Entre el rótulo y su campo | 4 |
| Entre un campo y el rótulo siguiente | 12 |
| Entre botones vecinos | 8 |
| Alto de campos y botones | 32 (botón principal: 36) |
| Alto de fila de grilla | 32; encabezado 36, o automático si el texto ocupa dos renglones (T-29) |

### 4.4 Estructura de ventana

```
┌──────────────────────────────────────────────────────────┐
│ ENCABEZADO  (Superficie, borde inferior 1 px)            │
│  Título de pantalla 18 pt          Paula · Nivel Plata   │
├──────────────────────────────────────────────────────────┤
│ CONTENIDO  (Fondo #F6F5FA, margen 24)                    │
│  ┌──────────── tarjeta ─────────────┐ ┌──── tarjeta ───┐ │
│  │ Título de sección 12 pt          │ │                │ │
│  │ …                                │ │                │ │
│  └──────────────────────────────────┘ └────────────────┘ │
├──────────────────────────────────────────────────────────┤
│ PIE  (Superficie, borde superior 1 px)                   │
│  [ Volver ]  (secundario)               [ Comprar ]      │
└──────────────────────────────────────────────────────────┘
```

«Volver» siempre abajo a la izquierda, como botón secundario; la acción principal siempre
abajo a la derecha. **Esto también corrige el recorrido con teclado:** hoy cinco «Volver»,
«Crear Cuenta» y «¿Olvidaste tu contraseña?» son etiquetas que el teclado no alcanza
(`docs/verificaciones/recorrido-teclado.md`, T-1 a T-5). Al pasar a botones, entran en el
ciclo de Tab.

### 4.5 Botones

| Estilo | Fondo | Texto | Borde | Cuándo |
|---|---|---|---|---|
| Primario | `Primario`; encima `PrimarioEncima`; presionado `PrimarioPresionado` | Blanco, 10 pt Semibold | — | La acción principal, una por pantalla |
| Secundario | `Superficie`; encima `PrimarioSuave` | `Primario`, 10 pt Semibold | 1 px `Borde` | Volver, Cancelar, Limpiar, acciones auxiliares |
| Peligro | `Superficie`; encima `ErrorFondo` | `Error`, 10 pt Semibold | 1 px `Borde` | Eliminar evento, eliminar sector, cancelar pago |
| Enlace | — | `Primario`, subrayado al pasar | — | «¿Cómo funcionan los niveles?», «¿Olvidaste tu contraseña?» |
| Deshabilitado | `FondoDeshabilitado` | `TextoDeshabilitado` | — | Cualquiera, sin acción posible |

Con foco de teclado: borde de 2 px `Primario`, que hoy los botones planos no muestran. Alto
32 (primario 36), relleno horizontal 20, ancho mínimo 120.

### 4.6 Grillas

- Encabezado: fondo `#F1EFF7`, texto 9 pt Semibold `TextoSecundario`, sin estilo visual de
  Windows (`EnableHeadersVisualStyles = false`), con ajuste de línea (T-29).
- Filas de 32 px, solo líneas horizontales (`SingleHorizontal`) en `Borde`, filas alternas
  `#FAF9FD`, sin columna de encabezado de fila.
- Celdas de solo lectura en `Texto` (**no en gris**: hoy parecen deshabilitadas). Las celdas
  editables en `PrimarioSuave`, con un borde fino al tener foco.
- Selección en `PrimarioSuave` con texto `Texto`, no el azul de Windows.
- Números e importes alineados a la derecha; fechas en formato `dd/MM/yyyy HH:mm`.
- `StandardTab = true`: Tab sale de la grilla en una pulsación, y las flechas recorren las
  celdas (corrige T-8 del recorrido con teclado: hoy «Mis eventos» lleva 27 Tab).
- Sin resultados: una fila de aviso en `TextoSecundario` (CP-16 sigue igual).

### 4.7 Formularios de carga

- Rótulo **arriba** del campo (9 pt Semibold `TextoSecundario`), campo de 32 px con borde
  `Borde` y borde `Primario` al tener foco; ayuda debajo en 9 pt.
- Error de validación **debajo del campo**, en `Error`, sin ventana emergente. El campo con
  error se marca con borde `Error`.
- Texto guía (cue banner) en los campos de búsqueda, como ya tiene el listado (CP-18).
- Enter ejecuta la acción principal en todas las pantallas de carga (`AcceptButton`; corrige
  U-02 en el login) y Esc vuelve (`CancelButton`; corrige T-10).

### 4.8 Mensajes

| Tipo | Cómo se muestra | Ejemplo |
|---|---|---|
| Validación de un campo | Texto en rojo debajo del campo | «El vencimiento debe tener el formato MM/AA.» |
| Resultado de una operación | Banda de color arriba del contenido, con ícono, que se cierra sola o con una X | Éxito: «Local registrado.»; Error: «El evento tiene entradas vendidas: no se puede eliminar.» |
| Confirmación (Sí/No) | **`MessageBox` nativo**, con título en lenguaje del usuario y «No» por defecto en lo destructivo | «¿Cerrar la compra?» |
| Error no previsto | `ManejadorErrores` (T-27), sin cambios | «No se pudo conectar con la base de datos…» |

**Por qué no reemplazar el `MessageBox`:** hay 78 llamadas en `Visual/`, y 11 pruebas
(CP-15, CP-17, CP-19 a CP-21, CP-24, CP-41, CP-42, CP-44, CP-48 y CP-52) responden el cuadro
nativo de Windows. Un cuadro propio obligaría a reescribirlas, y el cuadro nativo no se ve
«de facultad»: se ve como Windows. Lo que sí hay que cambiar es el **título**: hoy es el
código interno («RN-12») o está vacío (U-03). La regla es un título en castellano («No se
puede comprar»), con el código al final del texto, entre paréntesis.

### 4.9 Otros

- **Ícono** de la aplicación y de cada ventana: una «E» blanca sobre un cuadrado violeta. Va en
  la barra de título, la barra de tareas y el acceso directo.
- **Ticket PDF:** fondo blanco, una franja violeta con el nombre del evento, los datos en dos
  columnas y el QR con su zona de silencio. Se imprime sin gastar tinta y sale bien en la
  figura 4.
- **Resumen del paquete:** de un texto monoespaciado a una tabla de dos columnas (concepto a
  la izquierda, importe a la derecha) con el TOTAL en 14 pt.

---

## 5. Orden de aplicación (análisis del 05/11; el orden vigente es la sección 0)

Primero la infraestructura, después lo que el docente ve primero.

| Fase | Pantallas | Qué incluye | Horas |
|---|---|---|---|
| **0. Tema** | Las 19, automáticamente | `Tema.cs`; la base `FormularioEventAura` aplica fuente, colores, estilo de grillas y botones a todos sus controles; componentes `BotonPrimario`, `BotonSecundario`, `Tarjeta` y `BandaAviso`; quitar las fuentes y colores escritos en los diseñadores para que hereden; ícono. **Unificar la escala de diseño** (D-3): todos los diseñadores en la misma, ver la sección 8 | 12 |
| **1. Entrada** | Iniciar sesión, crear cuenta, cambiar contraseña | Diseño completo; «Crear cuenta» y «¿Olvidaste tu contraseña?» como enlaces alcanzables; Enter inicia sesión; Mail en el orden de Tab; «Mostrar contraseña» en castellano; sin el botón residual «Modificar» (U-02, U-11, U-13, T-1) | 6 |
| **2. Compra** | Menú del cliente, eventos disponibles, armar paquete, pago, compra confirmada | El circuito del instructivo y de las figuras. El resumen como tabla; rótulos de los filtros (U-08); «Cerrar sesión» como botón (T-2) | 14 |
| **3. Ticket** | PDF del ticket | Rediseño; volver a medir el QR (`tools/VerificarQrTickets.py`) | 4 |
| **4. Cliente, resto** | Mis eventos, mi suscripción | Grillas y botones; Tab sale de la grilla (T-8) | 4 |
| **5. Administración** | Menú, reportes, listas de espera, mis locales, eventos del local, administrar evento, agregar evento, agregar sector, registrar local | «Volver» como botón (T-3 a T-5), orden de Tab, foco inicial en el primer campo | 12 |
| **6. Evidencia** | — | Adaptar las pruebas, regenerar figuras y capturas, tercera medición de usabilidad (sección 6) | 18 |
| | | **Total** | **70 h** |

**Si el tiempo se acorta,** la fase 0 sola ya cambia las 19 pantallas. Las fases 4 y 5
pueden quedar con «tema automático» y sin rediseño a mano, y nada se ve remendado, porque
todo sale del mismo `Tema.cs`.

---

## 6. Qué se rompe: el costo completo

### 6.1 Pruebas automáticas (73 ejecuciones)

| Grupo | Casos | Ejecuciones | Por qué | Qué hacer |
|---|---|---|---|---|
| **Miden tamaños o posiciones** | CP-12 (alto ≤ 790), CP-13 (paquete máximo sin recortes), CP-25 (escala simulada, 3 variantes), CP-54 (encabezados) | 6 | Leen `Bounds`, `Height`, `ClientSize`, `MeasureText` con la fuente del control | **No se cambian: son la red de seguridad.** Si fallan, la pantalla nueva recorta algo. Solo CP-13 mide las líneas de `lblResumen`: si el resumen pasa a tabla, se reescribe esa parte (1 h) |
| **Comprueban colores** | CP-22, CP-23 (Gainsboro y violeta), CP-39 (violeta y Firebrick) | 3 | `Assert.AreEqual(Color…)` | El tema conserva esos tres colores: **no se rompen**. Si se cambia el violeta, se cambian las aserciones a `Tema.Primario` |
| **Dibujan la pantalla** | CP-13, CP-18, CP-25 | (incluidas arriba) | `DrawToBitmap` | Siguen valiendo |
| **Responden cuadros nativos** | CP-15, 17, 19, 20, 21, 24, 41, 42, 44, 48 y 52 | 15 | Buscan el `MessageBox` por título y texto | **No se rompen si se conserva `MessageBox`** (sección 4.8). Si se cambian los títulos (U-03), se actualiza el título esperado en cada una (2 h) |
| **Comprueban textos** | 19 aserciones en `PruebasInterfaz.cs` (encabezado «escribí», «Seleccionar evento», «Pago no realizado»…) | — | Comparan texto visible | Se actualizan donde cambie la redacción |
| **Buscan controles por nombre y tipo** | 32 controles (`btnComprar`, `dgvSectores`, `lblResumen`, `pnlResumen`…) | — | `Controls.Find(nombre)` y tipo `Button`, `Label`… | **Regla para Codex: no renombrar controles ni cambiar su tipo.** `BotonPrimario : Button` y `LinkLabel : Label` mantienen el tipo. Si un `Label` de resumen pasa a tabla, CP-13 se adapta |
| **Lógica de negocio** | CP-01 a CP-11, CP-26 a CP-38, CP-43, CP-45, CP-55 a CP-61 | 41 | No miran la interfaz | Nada |

Estimación: **6 a 8 horas de Codex** para adaptar pruebas, repartidas entre las fases. La
batería se corre al final de cada pantalla.

### 6.2 Figuras y capturas

| Qué | Cuántas | Cómo se regeneran | Horas |
|---|---|---|---|
| Figuras del instructivo (páginas 52 y 53) | 4 + 1 | `tools/CapturasInstructivo/Generar.ps1` | 0,5 + volver a pegarlas en el Word |
| Escalado (JC-R01) | 4 escalas × 2 | `Escala.ps1` + `VerificarEscala.ps1` | 0,5 |
| Excepciones (base caída, evento eliminado) | 2 | `tools/Excepciones/Verificar*.ps1` | 0,5 |
| Reserva vencida (CX-02) | 2 | `VerificarReservaVencida.ps1` (10 min reales) | 0,5 |
| Ticket (figura 4 y QR) | 1 + medición | `Generar.ps1` y `VerificarQrTickets.py` | 0,5 |

Todo es automático. **Regla:** las figuras se regeneran una sola vez, al cerrar la fase 2,
y otra vez al final si algo de la compra cambió.

### 6.3 Estudio de usabilidad

- **Qué no cambia:** el conteo de acciones de la tarea (18 acciones, 15 clics, 7 campos), si
  no se mueve nada del flujo.
- **Qué cambia y hay que volver a medir:**
  - el orden de Tab y el foco inicial, si se reacomodan controles;
  - los nombres accesibles (U-12);
  - las fricciones de teclado que se corrigen (U-02, U-07, T-1 a T-10);
  - la heurística 8 de Nielsen (estética y diseño minimalista), que hoy no tiene hallazgos
    sobre color ni tipografía porque el estudio no midió contraste.
- **Qué hacer:** una **tercera medición**, con el mismo método y las mismas herramientas
  (`Medir.ps1`, `VerificarCierreConX.ps1`, `modelo.py`). Se agrega una medición de contraste
  y una del recorrido con teclado. Son **4 horas** de Claude Code, más la actualización del
  informe. Las cifras del documento (SUS, satisfacción, hallazgos) cambian una vez más.
- **Alternativa más barata:** congelar el estudio en la segunda medición y declarar la
  renovación visual como posterior. Es más débil: el documento describiría una aplicación que
  no es la entregada.

### 6.4 Documento

- Las figuras de las páginas 52 y 53, otra vez.
- Si se hace la tercera medición: el informe de usabilidad, el resumen ejecutivo (páginas 7 y
  8), G-06 y las conclusiones, ya listados en `CORRECCIONES-DOCUMENTO.md`, con cifras nuevas.
- Una línea nueva en el historial de revisión.

---

## 7. Resumen de horas (camino A, estimación del 05/11; la vigente es la sección 0)

| Rubro | Horas | Quién |
|---|---|---|
| Fases 0 a 5 (implementación) | 52 | Codex, con encargos cerrados de Claude Code |
| Adaptar pruebas | 7 | Codex |
| Regenerar figuras y capturas | 3 | Claude Code |
| Tercera medición de usabilidad e informe | 6 | Claude Code |
| Actualizar el documento | 2 | Claude Code |
| **Total** | **70** | |
| Revisión visual de JC | 30 min por pantalla, ~10 h | JC |

---

## 8. Reglas para que no quede remendado

1. **Ningún color, fuente ni medida escrita en una pantalla:** todo sale de `Tema.cs`. Se
   agrega una prueba que lo verifica (busca `Color.FromArgb` y `new Font` fuera de `Tema.cs`).
2. **Una sola escala de diseño:** los diseñadores se editan con Windows a 100 %, o con Visual
   Studio reiniciado al 100 % (cuando la escala de Windows es mayor, Visual Studio lo ofrece en
   una barra amarilla al abrir un diseñador de WinForms). Hoy hay 11 pantallas a 120 DPI y 8 a
   144 DPI (D-3).
3. **No renombrar controles ni cambiar su tipo** (las pruebas los buscan por nombre).
4. **Un botón primario por pantalla.**
5. **Cada pantalla terminada se mira contra una captura de referencia** (la de login y la de
   compra, aprobadas por JC en la fase 1 y la fase 2).
6. **La batería se corre al terminar cada pantalla**, no al final.

---

## 9. Decisiones que necesito de JC

| # | Decisión | Mi recomendación |
|---|---|---|
| V-1 | Camino A, B o C | **A** |
| V-2 | Compatibilidad con DPI («system aware», por el manifiesto, sin tocar `App.config`) | **Sí, en la fase 0.** Es lo que más se nota en una notebook. Ahora tenemos verificación automática de escala; el 04/11 no la teníamos |
| V-3 | Tercera medición de usabilidad | **Sí.** El documento tiene que describir la aplicación entregada |
| V-4 | Rediseño del ticket PDF | **Sí** (4 h): aparece en la figura 4 y es lo que el usuario se lleva |
| V-5 | Corregir los callejones del teclado (T-1 a T-5) dentro de la renovación | **Sí:** salen solos al convertir las etiquetas en botones |
| V-6 | Títulos de los avisos en castellano (U-03) | **Sí, en las fases 1 y 2:** cuesta 2 h de pruebas y cierra un hallazgo |

---

## 10. Historia: los cronogramas con fechas (no vigentes)

> **No es el plan.** Se conserva como registro de cómo se planificó entre el 05/11 y el 10/11.
> JC dejó de planificar con fechas el 10/11: el plan vigente es la sección 0. Lo de abajo
> incluye dependencias de Máximo que ya no existen y un congelamiento el 27/11 que no era un
> plazo real.

> **Actualizado el 05/11/2026 con las decisiones de JC:** V-1 a V-6 aprobadas; la fase 0
> empieza por un **piloto** (`FrmCompraCliente`) que Máximo verifica en Visual Studio antes de
> tocar los otros 18; y **V-4, el ticket nuevo, va al principio**, en la misma tanda que el
> piloto, para que JC escanee el QR una sola vez. El encargo está en
> `TAREAS-CODEX-PRUEBAS.md`, «Sexta tanda», partes 6A y 6B. El sistema de diseño, en
> `SISTEMA-DISENO.md`.

> **Reemplazado por `ENCARGO-6B-BORRADOR.md` (versión 2), sección 5, el 10/11/2026, por la
> tarde.**
> - **Qué cambió:** 6B ahora incluye la limpieza del estilo escrito a mano y el piloto
>   corregido; los agentes trabajan en paralelo; la verificación de Máximo se hace sobre el
>   piloto corregido.
> - **El congelamiento del 27/11 sigue en pie**, con unos 3 días de margen.
> - **El día por día de abajo queda como historia.**
>
> **Re-fechado el 10/11/2026.** 6A terminó el 10/11 (77/77, commit `36a0d80`). Cambios desde
> la versión del 05/11:
> - las **64 pruebas de Máximo pasan a nosotros** (decisión de JC del 10/11): grupo A
>   automatizado por Codex (`TANDAS-PRUEBAS-64.md`), y grupos B y C a cargo de JC (31 min);
> - el fuente del documento no llegó al 08/11.
>
> **El congelamiento del 27/11 no se movió:** lo decide JC.

Supuesto: entrega el **viernes 04/12/2026**, y JC trabaja todos los días. **Los dos agentes
nunca trabajan a la vez:** los días de Codex y los de Claude Code se suman, no se superponen.

**Lo que no controlamos:**
- **6B depende de que Máximo verifique el piloto en Visual Studio.** Mientras tanto, Codex hace
  las pruebas del grupo A, que no dependen de 6B y además lo protegen.
- **Si Máximo no confirmó al terminar el grupo A (14/11)**, cada día de demora corre 6B un día.
  Y desde el 18/11 empieza a comerse el punto de control del 21/11 (ver «Si se atrasa»).

### Día por día

| Día | Quién | Qué |
|---|---|---|
| **Mar 10/11** | Claude Code | Planificación: borradores de 6B y de la séptima tanda, clasificación de las 64, este cronograma |
| | JC | Manda el mensaje a Máximo: piloto en Visual Studio y fuente del documento |
| **Mié 11/11** | Claude Code | Lo de después de 6A: mueve el reporte, adapta `Captura.exe` a DPI, captura el piloto a 100, 125 y 150 %, mide el QR del ticket nuevo y **avisa a JC que ya puede escanear** |
| | JC | Aprueba las capturas, **escanea los tickets (JC-R10)** y da el turno a Codex |
| **Jue 12 al sáb 14/11** | Codex | **Séptima tanda, T-42 a T-46**: grupo A de las 64. Batería esperada: 136. T-46 necesita dos decisiones previas de JC |
| **Dom 15/11** | Claude Code | Revisa la tanda; actualiza casos, matriz y `COMO-SE-PROBO.md`; AH-1 y AH-2; prepara el PDF del reporte para la lista de JC |
| | JC | Lista personal de las 64: 21 min. Con el sí de Máximo y el de JC: **«piloto confirmado»** |
| **Lun 16 al jue 19/11** | Codex | **6B**: T-41 (`Disposicion.cs`), T-38, T-39 y T-40 (`ENCARGO-6B-BORRADOR.md`). Batería esperada: 136 + 48 |
| | Claude Code | Entre tarea y tarea: capturas de cada tanda contra la referencia del piloto |
| **Vie 20 y sáb 21/11** | Claude Code | Repite las verificaciones sobre la aplicación nueva (escala real, teclado, excepciones, reserva) y regenera las figuras del instructivo. **21/11, punto de control:** lo que no llegó queda con el tema automático |
| **Dom 22 y lun 23/11** | Claude Code | **Tercera medición de usabilidad** |
| | JC | **23/11: arranca la conversión del B2** (ver abajo) |
| **Mar 24 y mié 25/11** | Claude Code | **Cierra la lista de correcciones**: todas las cifras (casos, cobertura, usabilidad), ítem 23 y figuras |
| **Jue 26/11** | — | Único día de margen |
| **Vie 27/11** | — | **Congelamiento del código** |
| **Sáb 28 al lun 30/11** | JC y Claude Code | B2: aplicar las correcciones a las páginas convertidas, armar con pypdf y reemplazar encabezado y pie |
| **Mar 01 al jue 03/12** | JC | Revisión final del PDF, última batería, último push, ensayo de la defensa |
| **Vie 04/12** | | Entrega |

### Si se atrasa

| Si… | Entonces |
|---|---|
| Máximo confirma después del 15/11 | 6B arranca el día siguiente a su confirmación. Desde el 18/11 no llega T-40 entera: el administrador queda con el tema automático, salvo los botones «Volver» (T-3 a T-5, 2 h), que se hacen igual porque son de teclado |
| Máximo no responde | JC decide si alcanza con la prueba del host de diseño de .NET, que ya pasó con 6A. Esa prueba no cubre la lectura del Designer por Visual Studio (`ENCARGO-6B-BORRADOR.md`, sección 0). **Si JC tiene Visual Studio en su PC, puede abrirlo él** |
| Un caso del grupo A encuentra un defecto | Codex para y JC decide: se corrige antes de 6B o queda anotado |
| El docente pide los anexos T01 a T08 | No entran. Ver «Lo que no tiene margen» |

**Re-estimación de los anexos (10/11).**
- **Como documentos** (lo real descripto sobre el código y lo demás rotulado «diseño
  propuesto»), los ocho son **37 a 52 horas estimadas**: 30 a 43 de Claude Code y 7 a 9 de JC.
- **Entran** si Claude Code los redacta mientras Codex implementa, como ya se trabajó el 10/11.
- **Depende del docente:** hay que confirmar que acepta los anexos como diseño, porque la
  Carpeta Modelo dice «documentarse **y desarrollarse**».
- **Si exige implementarlos**, vale lo de abajo.

**Lo que decía antes (los anexos como implementación):** la respuesta del docente sobre
T01 a T08 (`ANEXOS-TECNICOS-T01-T08.md`):
- **los ocho anexos completos** son 100 a 144 horas: no entran junto con la renovación. Habría
  que elegir;
- **el subconjunto mínimo** que propone ese archivo (T01, T02, T03 y T06a) son 30 a 42 horas.
  Entra si la fase 5 queda con tema automático y la tercera medición se acota.

Por eso conviene que el mail al docente salga esta semana.
