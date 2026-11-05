# Informe de pruebas de usabilidad (simulado)

EventAura · Trabajo de Campo I · noviembre de 2026. Reemplaza los resultados de las páginas 90
a 97 del documento, con la misma estructura.

> **Declaración sobre la naturaleza de los datos.** Las sesiones de este informe son
> **simuladas**, igual que las minutas de reunión del documento: no participaron personas.
> A diferencia de la versión anterior del documento, **el comportamiento de la interfaz no se
> supuso: se midió sobre la aplicación real, dos veces.** Primero se midió, después se
> corrigió lo encontrado y se volvió a medir con el mismo método. Qué se midió y qué se
> derivó:
>
> | Medido sobre la aplicación real | Derivado de los perfiles |
> |---|---|
> | Acciones, pantallas, campos y caracteres de la tarea | Tiempos por perfil, con el Keystroke-Level Model sobre las acciones medidas |
> | Orden de tabulación real de cada pantalla | Errores probables de cada perfil, cada uno atado a un hecho medido |
> | Texto, título y botón por defecto de cada diálogo | Necesidad de asistencia, por recorrido cognitivo |
> | Mensajes ante cada dato inválido | Satisfacción por criterio y SUS, con reglas explícitas |
> | Pérdidas de trabajo y callejones sin salida | — |

**Versiones medidas.** Las dos son de la semilla con el evento «Duki en Rosario» y la clienta
Plata con 4 compras, en Windows 11 a escala 100 %, 1920 × 1080:

| Medición | Commit | Registros |
|---|---|---|
| **Primera** | `7915c09` | [medicion-antes-T22.txt](medicion-antes-T22.txt), [cierre-con-x-antes-T22.txt](cierre-con-x-antes-T22.txt), [modelo-resultados-antes-T22.md](modelo-resultados-antes-T22.md) |
| **Segunda**, después de las correcciones T-22 a T-26 | `bb41f23` | [medicion.txt](medicion.txt), [cierre-con-x.txt](cierre-con-x.txt), [modelo-resultados.md](modelo-resultados.md) |

---

## 0. Antes y después

| Métrica | Primera medición | Segunda medición | Umbral | Cambio |
|---|---|---|---|---|
| Acciones de la tarea | 18 | 18 | — | = |
| Clics / campos / caracteres | 15 / 7 / 65 | 15 / 7 / 65 | — | = |
| Pantallas | 6 y 1 diálogo | 6 y 1 diálogo | — | = |
| Perfiles que completan **sin asistencia** | 4 de 6 (66,7 %) | **6 de 6 (100 %)** | 60 % | ▲ |
| Tiempo promedio desde el ingreso | 2:38 | 2:35 | Menos de 5 min | ▲ |
| Tiempo del perfil más lento | 4:30 (P6) | 4:27 (P6) | Menos de 5 min | ▲ |
| Errores promedio por perfil | 1,0 (máximo 3) | 0,8 (máximo 2) | Hasta 2 | ▲ |
| Satisfacción promedio (1 a 5) | 4,08 | **4,36** | Mayor que 4 | ▲ |
| **Perfiles con satisfacción buena o muy buena** | **66,7 % — no alcanza** | **100 % — cumple** | **70 %** | ▲ |
| SUS promedio | 70,0 | **78,8** | 68 | ▲ |
| Perfiles con SUS F («inaceptable») | 2 (P3 con 35,0 y P6 con 40,0) | 0 (P3 con 65,0 y P6 con 62,5) | — | ▲ |
| Cerrar una pantalla con la X | Aplicación viva sin ventanas | La aplicación termina (UI Automation: «proceso vivo = False») | — | ▲ |
| Cerrar la compra con la X con un paquete armado | Se descarta sin preguntar | Pregunta, con «No» por defecto | — | ▲ |
| «Comprar» sin entradas | Habilitado; lleva al aviso «RN-13» | Deshabilitado | — | ▲ |
| Hallazgos abiertos | 13 (2 de severidad mayor) | **10 (ninguno mayor)**: 3 resueltos, 1 nuevo (U-14) que también se resolvió | — | ▲ |
| Plan de mejora pendiente | 5,5 pt (22 h) | **3,0 pt (12 h)** | — | ▲ |

**Lectura.**
- **Las métricas de la tarea no cambiaron, y no debían cambiar.** El ejemplo del documento
  usa el sector Campo, que es la tercera fila: hay que hacer clic en esa celda igual que
  antes.
- **Las correcciones atacaban otra cosa:** que el usuario **sepa qué hacer** (dónde se escribe
  la cantidad) y **no pierda trabajo ni quede atrapado** (la X, «Comprar» sin entradas). Por
  eso los cambios aparecen en la asistencia, los errores, la satisfacción y el SUS, no en el
  conteo de clics.
- **El criterio del 70 %, que en la primera medición no se alcanzaba, ahora se cumple.**

---

## 1. Objetivo de la evaluación

Verificar que un usuario sin experiencia técnica pueda completar el flujo de armado y compra
de un paquete personalizado (el requerimiento core) dentro del tiempo comprometido, e
identificar los puntos del recorrido que generan confusión, error o pérdida de trabajo. Se
verifica además que las seis correcciones del estudio anterior (H-01 a H-06) estén en la
interfaz.

## 2. Diseño de la prueba

| Aspecto | Definición adoptada |
|---|---|
| Tipo de estudio | Evaluación sin usuarios, con tres métodos formales sobre la aplicación real: (a) **medición instrumentada** de la interfaz; (b) **evaluación heurística** con las diez heurísticas de Nielsen (1994); (c) **recorrido cognitivo** (Wharton, Rieman, Lewis y Polson, 1994) por perfil, con tiempos del **Keystroke-Level Model** (Card, Moran y Newell, 1980) |
| Iteraciones | Dos: medición, corrección de los hallazgos de mayor severidad (T-22 a T-26) y nueva medición con el mismo método y los mismos perfiles |
| Objeto evaluado | Flujo de ocho pasos del Instructivo del requerimiento core, más el ingreso al sistema |
| Perfiles | Seis: los cinco del estudio anterior, con la misma edad y experiencia, y uno nuevo que usa solo teclado |
| Instrumentos | `tools/Usabilidad/Medir.ps1` (medición), `tools/Usabilidad/VerificarCierreConX.ps1` (UI Automation sobre el ejecutable), `tools/Usabilidad/modelo.py` (tiempos, errores, satisfacción y SUS; con la versión «antes» o «despues») |

**Cómo se midió.**
- **La medición instrumentada** abre los formularios reales de `Visual.exe`, con sus gestores
  y una base recién creada con la semilla, y actúa sobre los controles como el usuario: escribe
  en los campos, elige filas, tilda casillas y pulsa botones. Registra:
  - cada acción;
  - el orden de tabulación que recorre WinForms;
  - el texto, el título y el botón por defecto de cada diálogo nativo;
  - qué queda en pantalla después de cada caso de error.
- **No se enviaron teclas al escritorio.**
- **Los callejones sin salida se confirmaron además sobre el ejecutable** con UI Automation.

**Por qué este diseño.**
- La evaluación heurística y el recorrido cognitivo son los métodos de inspección habituales
  cuando no hay participantes disponibles.
- Atar cada conclusión de un perfil a un hecho medido evita el defecto principal de la
  versión anterior: errores y tiempos sin respaldo en la interfaz.
- Repetir la medición después de corregir convierte el informe en evidencia de mejora, no en
  una opinión.

## 3. Perfiles

| Perfil | Edad | Ocupación | Soltura con la tecnología | Experiencia comprando entradas en línea | Parámetros del modelo |
|---|---|---|---|---|---|
| P1 | 21 | Estudiante universitario | Alta | Alta: compra habitualmente | Tipeo 0,20 s/tecla; lectura 250 ppm |
| P2 | 34 | Empleada administrativa | Media-alta | Media: compró varias veces | 0,28 s; 220 ppm; decisiones ×1,3 |
| P3 | 52 | Comerciante | Baja | Nula: nunca compró en línea | 1,20 s; 150 ppm; decisiones ×2; mouse ×1,5 |
| P4 | 19 | Estudiante secundario | Alta, más en el celular que en la PC | Alta | 0,35 s; 250 ppm; decisiones ×1,1 |
| P5 | 45 | Docente | Media | Media: compró alguna vez | 0,40 s; 200 ppm; decisiones ×1,5; mouse ×1,2 |
| P6 | 67 | Jubilado | Baja; baja visión, usa lupa | Nula | Solo teclado; 0,90 s; 110 ppm; decisiones ×2,2 |

Operadores del Keystroke-Level Model, iguales para todos: apuntar con el mouse 1,10 s, hacer
clic 0,20 s, pasar la mano del teclado al mouse 0,40 s y preparación mental 1,35 s.

## 4. Tarea evaluada y protocolo

**Escenario** (el mismo del documento): «Querés ir a ver a Duki en Rosario con un
acompañante. Además de las entradas, necesitás dónde alojarte esa noche. Comprá lo necesario
usando la plataforma.»

**Criterio de éxito:** llegar a la pantalla de confirmación con al menos una entrada y un
servicio complementario, sin asistencia.

**Asistencia:** se considera que un perfil la necesita cuando el recorrido cognitivo responde
«No» a «¿notará que la acción correcta está disponible?» en un paso sin ninguna pista visible.
Equivale al bloqueo de más de sesenta segundos del protocolo original.

**Error:** una acción que obliga a rehacer un paso, la misma definición del documento.

## 5. Métricas e instrumentos

| Métrica | Forma de medición | Umbral comprometido |
|---|---|---|
| Tasa de éxito | Perfiles que llegan a la confirmación sin asistencia | 3 de 5 (60 %) |
| Tiempo de ejecución | KLM sobre las acciones medidas, con los errores de cada perfil | Menos de 5 min (RNF-01) |
| Errores | Errores atados a hechos medidos, por perfil | Hasta 2 por perfil |
| Satisfacción por criterio | Los seis criterios del documento, de 1 a 5, con reglas explícitas | Promedio mayor que 4; 70 % de perfiles con 4 o 5 |
| System Usability Scale | Diez ítems (Brooke, 1996), derivados de criterios y hechos medidos | 68 o más |

---

## 6. Lo medido sobre la aplicación real

### 6.1 La tarea completa (sin cambios entre las dos mediciones)

| Medida | Valor |
|---|---|
| Acciones del usuario | **18**: 15 clics (6 para entrar a un campo), 7 campos completados, 65 caracteres tipeados, 2 teclas Enter |
| Pantallas | **6 pantallas y 1 diálogo**: Iniciar sesión → Mi cuenta → Eventos disponibles → Armar paquete → (diálogo «Seguro de entrada») → Pago seguro → Compra confirmada |
| Campos, en orden | Mail (19 caracteres) → Contraseña (10) → Cantidad de Campo en la grilla (1) → Titular (11) → Número de tarjeta (16) → Vencimiento (5) → Código de seguridad (3) |
| Resumen obtenido | Base $50.000; descuento Plata −$2.500; hospedaje $45.000; cargo $7.125; **total $99.625**, igual que la página 54 |

### 6.2 Orden de tabulación y foco de cada pantalla

| Pantalla | Primera medición | Segunda medición |
|---|---|---|
| Iniciar sesión | Mail sin tabulación; Enter no inicia sesión; botón «Modificar» fuera de la vista | **Sin cambios** (U-02, U-13) |
| Crear cuenta | Foco inicial en Contraseña; dos saltos hacia atrás; selector de perfil | **El selector de perfil se eliminó** (T-22). El foco y los dos saltos siguen (U-10) |
| Mi cuenta | Sin problemas | Sin problemas |
| Eventos disponibles | El primer Tab va a «Volver» | **Sin cambios** (U-10) |
| Armar paquete | Foco en la celda «Sector» de VIP; «Comprar» habilitado sin entradas | **Foco en la celda Cantidad de VIP**, columna titulada «Cantidad (escribí aquí)» con ayuda emergente (T-25). «Comprar» y el seguro **deshabilitados** hasta elegir una entrada: no entran en la tabulación (T-26) |
| Pago seguro | Enter paga | Sin cambios |

### 6.3 Diálogos y mensajes

| Situación | Título | Texto | Botón por defecto |
|---|---|---|---|
| Login vacío | «Campo vacíos» | «Por favor, ingrese usuario» (el campo se llama «Mail») | Aceptar |
| Contraseña incorrecta | «Inicio de sesión» | «Mail o contraseña incorrectos. Le quedan 4 intentos…» | Aceptar |
| Mail inexistente | (vacío) | «Mail o contraseña incorrecta» | Aceptar |
| «Comprar» sin evento elegido | «Seleccionar evento» | «Debe seleccionar un evento para comprar» | Aceptar |
| ~~«Comprar» con solo un servicio~~ | ~~«RN-13»~~ | **Ya no se puede llegar:** «Comprar» está deshabilitado sin entradas | — |
| Cantidad 7 | «RN-12» | «Se pueden comprar como máximo 6 entradas por operación.» | Aceptar |
| Cantidad «dos» | (vacío) | «La cantidad debe ser un número entero de 0 en adelante.» | Aceptar |
| Comprar sin seguro | «Seguro de entrada» | «… ¿Continuar sin seguro?» | No |
| «Volver al listado» con paquete | «Volver al listado» | «…se descarta el paquete que armó. ¿Desea volver?» | No |
| **Cerrar la compra con la X, con paquete** (nuevo) | «Cerrar la compra» | «Si cierra la ventana se descarta el paquete que armó. ¿Desea cerrar?» | **No** |
| «Cancelar pago» | «Cancelar pago» | «…se liberan las entradas reservadas. ¿Desea cancelar?» | **Sí** (U-07, abierto) |

Los errores del pago se muestran en una etiqueta y explican con precisión qué corregir. Siguen
sin cambios: después de cualquier error **se borra el código de seguridad**, y tras un rechazo
también el número (U-06).

### 6.4 Pérdidas de trabajo y callejones sin salida

| Caso | Primera medición | Segunda medición |
|---|---|---|
| Cerrar una pantalla con la X | La aplicación queda **viva y sin ventanas** (UI Automation: proceso vivo, 56 MB) | **La aplicación termina** (UI Automation: «proceso vivo = False; ventanas visibles = 0») |
| Cerrar «Armar paquete» con la X con un paquete armado | Se descarta sin preguntar | **Pregunta**, con «No» por defecto; con «No» se conserva el paquete |
| Ventanas ocultas acumuladas después de una compra | 9 | 9: cada «Volver» crea una pantalla nueva. Ya no deja un proceso huérfano, pero sigue usando memoria |
| «Cancelar pago» | «Sí» por defecto | Sin cambios (U-07) |
| Cerrar el pago con la X | Se cierra sin preguntar y libera la reserva; el paquete se conserva | Sin cambios |
| Después de «Aceptar» en la confirmación | El usuario queda en un armado vacío | Sin cambios (U-09) |
| Tildar un servicio sin entradas | Se permite y «Comprar» queda habilitado | Se permite, pero **«Comprar» queda deshabilitado** hasta elegir una entrada |

### 6.5 Accesibilidad medida con UI Automation

Sin cambios: UI Automation expone los botones y los campos como «Pane», y los campos sin
nombre accesible (U-12).

### 6.6 Correcciones verificadas

**Las del estudio anterior** (H-01 a H-06) siguen en la interfaz: la leyenda del descuento,
el temporizador de la reserva, la columna «Disponibles», la confirmación del seguro, el
enlace de los niveles y «Volver al listado» con confirmación.

**Las de esta ronda (T-22 a T-26)**, cada una con su prueba automática:

| Corrección | Hallazgo | Medido en la segunda medición | Prueba |
|---|---|---|---|
| T-22: el alta pública crea solo clientes | Seguridad | «Crear cuenta» ya no tiene selector de perfil | CP-44, CP-45 |
| T-23: «Volver al inicio» cierra la sesión | Seguridad | — (no afecta la tarea de compra) | CP-46 |
| T-24: la X termina la aplicación; la compra pide confirmación | U-01 | UI Automation: «proceso vivo = False»; diálogo «Cerrar la compra» | CP-47, CP-48 |
| T-25: indicación de dónde se escribe la cantidad | U-04 | Encabezado «Cantidad (escribí aquí)», ayuda y foco en la celda | CP-49 |
| T-26: «Comprar» y el seguro sin entradas | U-05 | Los dos deshabilitados al abrir | CP-50 |

**Hallazgo nuevo de la segunda medición (U-14).** A escala 100 %, la columna es más angosta
que el encabezado nuevo, y se lee «Cantidad (escribí», sin el «aquí)». Se vio en la figura 2
del instructivo regenerada, no en la medición instrumentada, que no mide el ancho de los
encabezados. **Se corrigió el 05/11/2026** (T-29): los encabezados ajustan línea y alto, y
«Cantidad (escribí aquí)» se lee completo en dos renglones, como muestra la figura 2
regenerada. CP-54 comprueba que cada encabezado entra en su alto. Volver a medir encontró un
defecto que introdujo la propia corrección, que es justamente para lo que sirve.

**Otra observación de la segunda medición.** Al abrir el armado, la celda Cantidad de VIP
queda en modo edición mostrando «0». Escribir «2» funciona, porque el valor se interpreta
igual. Pero el foco cae en la fila de VIP, la primera, y el ejemplo usa Campo, la tercera.
Quien no lee la columna Sector puede cargar la cantidad en VIP. Se mantiene como el error
probable E-B de P3 y P5.

---

## 7. Evaluación heurística

Diez heurísticas de Nielsen (1994). Severidad en su escala: 0 no es un problema, 1 cosmético,
2 menor, 3 mayor, 4 catastrófico. La última columna indica el estado después de las
correcciones.

| Heurística | Violación | Sev. | Hallazgo | Estado |
|---|---|---|---|---|
| 1. Visibilidad del estado del sistema | Después de comprar, el usuario queda en un armado vacío | 2 | U-09 | Abierto |
| 1. Visibilidad del estado del sistema | El filtro de fecha muestra una fecha aunque está desactivado | 2 | U-08 | Abierto |
| 2. Relación con el mundo real | Títulos con códigos internos («RN-12»); «Comprar» del listado abre el armado; «ingrese usuario» para el campo Mail | 2 | U-03 | Abierto. Ya no se llega al aviso «RN-13» |
| 2. Relación con el mundo real | «Show Password» en inglés | 1 | U-11 | Abierto |
| 3. Control y libertad del usuario | La X deja la aplicación invisible y en ejecución; el armado pierde el paquete sin preguntar | 3 | U-01 | **Resuelto** (T-24) |
| 3. Control y libertad del usuario | «Cancelar pago» con «Sí» por defecto | 2 | U-07 | Abierto |
| 4. Consistencia y estándares | Enter paga pero no inicia sesión; botones por defecto distintos; avisos con y sin título | 2 | U-02, U-07, U-03 | Abierto |
| 4. Consistencia y estándares | Orden de tabulación de «Crear cuenta» y del listado | 2 | U-10 | Abierto |
| 5. Prevención de errores | «Comprar» y el seguro habilitados sin entradas | 2 | U-05 | **Resuelto** (T-26) |
| 5. Prevención de errores | La cantidad se escribe a mano en una celda; los valores inválidos se rechazan después | 2 | U-04 | Mitigado: ayuda con el máximo por compra |
| 6. Reconocer antes que recordar | Nada indicaba dónde se escribe la cantidad | 3 | U-04 | **Resuelto** (T-25) |
| 6. Reconocer antes que recordar | Filtros del listado sin rótulos | 2 | U-08 | Abierto |
| 7. Flexibilidad y eficiencia | El login no acepta Enter; el Mail no se alcanza con Tab | 2 | U-02 | Abierto |
| 8. Diseño estético y minimalista | Botón «Modificar» residual en el login | 1 | U-13 | Abierto |
| 9. Recuperarse de errores | El código de seguridad se borra sin avisar | 2 | U-06 | Abierto |
| 10. Ayuda y documentación | Ayuda contextual en los niveles y, ahora, en la cantidad | 1 | — | Mejorado |
| (Accesibilidad) | Campos sin nombre accesible; botones como «Pane» | 2 | U-12 | Abierto |

## 8. Recorrido cognitivo por perfil

Preguntas de Wharton et al. (1994):
1. ¿Intentará lograr el efecto correcto?
2. ¿Notará que la acción correcta está disponible?
3. ¿Asociará la acción con el efecto?
4. ¿Entenderá el progreso?

Se listan los pasos con algún «No» en la segunda medición, y qué cambió respecto de la
primera.

| Perfil | Paso | Pregunta con «No» | Hecho medido | Consecuencia | Primera medición |
|---|---|---|---|---|---|
| Todos | 8 | 4: tras «Aceptar» queda en un armado vacío | 6.4 | Satisfacción: navegación −1 | Igual |
| P3 | 3 | 3: escribe la cantidad en la fila de VIP, donde está el foco, en lugar de Campo | Foco inicial en la Cantidad de VIP | Error E-B | Antes era «No» en la pregunta 2: necesitaba **asistencia**. Ahora encuentra dónde escribir |
| P3 | 4 | — | «Comprar» deshabilitado sin entradas | Sin error | Antes: error E-A (aviso «RN-13») |
| P3 | 7 | 3: escribe el vencimiento sin barra | Validación de formato; código borrado | Error E-C | Igual |
| P3 | 8 | — | La X termina la aplicación | Sin penalización | Antes cerraba con la X y la aplicación quedaba invisible |
| P4 | 0 | 2: espera que Enter inicie sesión | Login sin AcceptButton | Fricción F-1 | Igual |
| P5 | 3 | 3: escribe la cantidad en VIP | Ídem P3 | Error E-B | Igual |
| P6 | 0 | 2: con Tab no vuelve al Mail; Enter no inicia sesión | 6.2 | Fricciones F-1 y F-2 | Igual |
| P6 | 3 | — | El foco ya está en la columna Cantidad: Abajo ×2, «2», Enter (4 teclas) | Sin asistencia | Antes necesitaba **asistencia** y 7 teclas |
| P6 | 5 | 3: Enter en el aviso del seguro elige «No» | Botón por defecto «No» | Error E-D | Igual |
| P6 | 7 | 3: formato del vencimiento | Ídem P3 | Error E-C | Igual |

---

## 9. Resultados (segunda medición)

### Resultados por perfil

| Perfil | Completó la tarea | Tiempo (desde el ingreso) | Errores | SUS | Primera medición |
|---|---|---|---|---|---|
| P1 | Sí, sin asistencia | 1:26 | 0 | 92,5 | Igual |
| P2 | Sí, sin asistencia | 1:42 | 0 | 87,5 | Igual |
| P3 | Sí, **sin asistencia** | 4:01 | 2 | 65,0 | Con asistencia, 4:16, 3 errores, SUS 35,0 |
| P4 | Sí, sin asistencia | 1:41 | 0 | 87,5 | Igual |
| P5 | Sí, sin asistencia | 2:13 | 1 | 77,5 | Igual |
| P6 | Sí, **sin asistencia** | 4:27 | 2 | 62,5 | Con asistencia, 4:30, 2 errores, SUS 40,0 |
| **Promedio** | **6 de 6 sin asistencia** | **2:35** | **0,8** | **78,8** | 4 de 6; 2:38; 1,0; 70,0 |

Las correcciones mejoraron a **los dos perfiles que el estudio anterior dejaba atrás**, que son
los que nombra RNF-01. Los perfiles con experiencia no cambian: ya completaban la tarea sin
problemas.

### Tiempo por paso frente a la línea base

| Nº | Paso | Estimado del documento | KLM, usuario experto | Promedio de los perfiles |
|---|---|---|---|---|
| 1 | Buscar el evento | 45 s | 7 s | 9 s |
| 2 | Seleccionar el evento | 15 s | 4 s | 6 s |
| 3 | Elegir sector y cantidad | 40 s | 8 s | 17 s |
| 4 | Agregar servicios | 60 s | 6 s | 8 s (antes 10 s: P3 ya no comete E-A) |
| 5 | Decidir el seguro | 20 s | 10 s | 18 s |
| 6 | Revisar el resumen | 30 s | 12 s | 17 s |
| 7 | Completar el pago | 60 s | 21 s | 42 s |
| 8 | Confirmar y recibir el comprobante | 10 s | 10 s | 13 s |
| | **Total de los pasos 1 a 8** | **4:40** | **1:18** | **2:10** (antes 2:13) |

**Interpretación.** El estimado del documento queda holgado frente al recorrido real. Los pasos
que más se alejan del experto siguen siendo el 3 (cantidad) y el 7 (pago), donde están los
errores E-B y E-C, que no se corrigieron en esta ronda.

### Satisfacción por criterio — escala de 1 a 5

| Criterio | P1 | P2 | P3 | P4 | P5 | P6 | Prom. | Primera medición |
|---|---|---|---|---|---|---|---|---|
| Claridad de la navegación | 4 | 4 | 4 | 3 | 4 | 2 | 3,5 | 3,2 |
| Facilidad para armar el paquete | 5 | 5 | 4 | 5 | 4 | 5 | 4,7 | 4,2 |
| Comprensión del resumen y del descuento | 5 | 4 | 4 | 5 | 4 | 4 | 4,3 | 4,3 |
| Claridad de los mensajes de error | 5 | 5 | 4 | 5 | 5 | 3 | 4,5 | 4,3 |
| Confianza en el proceso de pago | 5 | 5 | 4 | 5 | 5 | 4 | 4,7 | 4,7 |
| Satisfacción general | 5 | 5 | 4 | 5 | 4 | 4 | 4,5 | 3,8 |
| **Promedio global** | | | | | | | **4,36** | 4,08 |

**Perfiles con satisfacción general buena o muy buena: 6 de 6 (100 %), contra el 70 %
comprometido.** En la primera medición eran 4 de 6 (66,7 %) y no alcanzaba.

**Sensibilidad del resultado.** La satisfacción general es el promedio redondeado de los otros
cinco criterios:
- **P6 queda justo en el límite:** su promedio es 3,6, que se redondea a 4. Si se le contara
  3,6 como «insatisfecho», el resultado sería 5 de 6, el 83 %, y **el criterio se seguiría
  cumpliendo**.
- **P3 promedia 4,0 sin redondeo.**
- **El criterio más bajo sigue siendo la navegación** (3,5): el armado vacío después de
  comprar (U-09) y las fricciones de teclado del login (U-02) siguen abiertos.

### System Usability Scale

Cada respuesta se deriva de un criterio o de un hecho medido, con las mismas reglas en las dos
mediciones. Están en `tools/Usabilidad/modelo.py`:

| Ítem | De dónde sale |
|---|---|
| 1. Lo usaría con frecuencia | Satisfacción general, con tope 4: comprar entradas es ocasional |
| 2. Innecesariamente complejo | 6 − facilidad para armar el paquete |
| 3. Fácil de usar | Satisfacción general |
| 4. Necesitaría ayuda técnica | 4 si necesitó asistencia; 2 si cometió errores; 1 si no |
| 5. Funciones bien integradas | Facilidad para armar el paquete |
| 6. Inconsistencias | 3 si encontró Enter que no responde, el «No» por defecto del seguro o el título «RN-13»; si no, 2 |
| 7. Se aprende rápido | 5 con experiencia alta, 4 media, 2 nula |
| 8. Engorroso | 6 − claridad de la navegación |
| 9. Me sentí seguro | Confianza en el proceso de pago |
| 10. Tuve que aprender mucho antes | 1 con experiencia alta, 2 media, 4 nula |

| Puntaje SUS | Interpretación (Bangor et al., 2009) | Segunda medición | Primera medición |
|---|---|---|---|
| 90 o más | A — Excelente | P1 (92,5) | P1 |
| 80 a 89 | B — Bueno | P2 (87,5), P4 (87,5) | P2, P4 |
| 70 a 79 | C — Aceptable | P5 (77,5) | P5 |
| 60 a 69 | D — Deficiente | P3 (65,0), P6 (62,5) | — |
| Menos de 60 | F — Inaceptable | — | P3 (35,0), P6 (40,0) |

**Promedio de 78,8**, contra 70,0 en la primera medición y 68 de referencia.
- **Ningún perfil queda en «inaceptable».**
- **P3 y P6 suben de F a D.** Lo que los separa de C son los errores de formato en el pago
  (U-06) y las fricciones de teclado (U-02 y U-07), que siguen abiertos.

### Cumplimiento de los criterios comprometidos

| Criterio | Origen | Umbral | Primera medición | Segunda medición | Veredicto |
|---|---|---|---|---|---|
| Cantidad de perfiles | Obj. esp. 2 | 3 o más | 6 | 6 | Cumple |
| Completaron el flujo principal | Crit. acept. 7 | 3 o más | 6 (4 sin asistencia) | 6 (6 sin asistencia) | Cumple |
| Tasa de éxito sin asistencia | Diseño | 60 % | 66,7 % | **100 %** | Cumple |
| Tiempo del flujo completo | RNF-01 | Menos de 5 min | 2:38 (máximo 4:30) | 2:35 (máximo 4:27) | Cumple |
| Satisfacción buena o muy buena | Obj. esp. 4 | 70 % o más | 66,7 % | **100 %** | **Cumple** (antes no alcanzaba) |
| Promedio en escala de 1 a 5 | Obj. esp. 4 | Mayor que 4 | 4,08 | 4,36 | Cumple |
| Puntaje SUS | Referencia | 68 o más | 70,0 | 78,8 | Cumple |
| Errores por perfil | Diseño | Hasta 2 | 1,0 (P3 con 3) | 0,8 (máximo 2) | **Cumple** (antes con reservas) |

**Lectura.** Después de las correcciones se cumplen todos los criterios, sin reservas. El
objetivo específico 4 queda alcanzado por las dos vías que admite: el porcentaje y el
promedio.

---

## 10. Hallazgos y plan de mejora

### Hallazgos

| Nº | Hallazgo | Sev. | Estado |
|---|---|---|---|
| U-01 | Cerrar una pantalla con la X dejaba la aplicación en ejecución y sin ventanas; el armado descartaba el paquete sin preguntar | 3 | **Resuelto** (T-24; CP-47, CP-48; UI Automation) |
| U-04 | Nada indicaba dónde se escribe la cantidad | 3 | **Resuelto** (T-25; CP-49). Queda la observación de la fila (sección 6.6) |
| U-05 | «Comprar» y el seguro habilitados sin entradas | 2 | **Resuelto** (T-26; CP-50) |
| U-02 | El login no acepta Enter; el Mail está fuera del orden de tabulación | 2 | Abierto |
| U-03 | Títulos técnicos («RN-12») o vacíos; «Campo vacíos»; «ingrese usuario»; «Comprar» del listado abre el armado | 2 | Abierto. Ya no se llega al título «RN-13» |
| U-06 | El código de seguridad se borra después de un error sin avisar | 2 | Abierto |
| U-07 | «Cancelar pago» con «Sí» por defecto | 2 | Abierto |
| U-08 | Filtros del listado sin rótulos | 2 | Abierto |
| U-09 | Después de confirmar, el usuario queda en un armado vacío | 2 | Abierto |
| U-10 | Orden de tabulación de «Crear cuenta» y del listado | 2 | Abierto |
| U-12 | Campos sin nombre accesible; botones como «Pane» | 2 | Abierto |
| U-11 | «Show Password» en inglés | 1 | Abierto |
| U-13 | Botón «Modificar» residual en el login | 1 | Abierto |
| U-14 | El encabezado «Cantidad (escribí aquí)» se cortaba a 100 %: se leía «Cantidad (escribí» | 2 | **Resuelto** (T-29; CP-54; figura 2). Lo encontró la segunda medición |

**4 resueltos, contando el nuevo (U-14), y 10 abiertos; ninguno de severidad mayor.**

### Plan de mejora recalculado

1 punto equivale a 4 horas. Todo cae en `Visual/`.

| Prioridad | Acción correctiva | Hallazgo | Esfuerzo |
|---|---|---|---|
| ~~1~~ | ~~Terminar la aplicación al cerrar la última ventana; confirmar en el armado~~ | U-01 | Hecho |
| ~~2~~ | ~~Foco y rótulo en la cantidad~~ | U-04 | Hecho |
| ~~3~~ | ~~«Comprar» y el seguro sin entradas~~ | U-05 | Hecho |
| ~~0~~ | ~~Encabezado de la cantidad en dos renglones~~ | U-14 | Hecho (T-29) |
| 1 | Login: «Iniciar sesión» como botón de Enter; Mail con tabulación y primero en el orden | U-02 | 0,25 pt |
| 2 | «Cancelar pago» con «No» por defecto | U-07 | 0,25 pt |
| 3 | Títulos de los avisos en lenguaje del usuario; «Campos vacíos»; «Ingrese su mail» | U-03 | 0,5 pt |
| 4 | Aviso «Volvé a ingresar el código de seguridad» cuando se borra | U-06 | 0,25 pt |
| 5 | Después de «Aceptar», volver al listado de eventos | U-09 | 0,25 pt |
| 6 | Rótulos «Categoría» y «Desde» en los filtros del listado | U-08 | 0,5 pt |
| 7 | Orden de tabulación de «Crear cuenta» y del listado | U-10 | 0,25 pt |
| 8 | Nombres accesibles en campos y botones del flujo de compra | U-12 | 0,5 pt |
| 9 | «Mostrar contraseña»; quitar el botón «Modificar» del login | U-11, U-13 | 0,25 pt |
| | **Total pendiente** | | **3,0 pt (12 h)**, antes 5,5 pt (22 h) |

**Las prioridades 1, 2 y 4 son las que más pesan en la experiencia de P3 y P6**, los perfiles
con SUS «D». Corrigen la fricción F-1 y los errores E-C y E-D. Suman 0,75 puntos (3 horas).

## 11. Limitaciones del estudio

1. **Carácter simulado.** No hubo personas. Lo medido sobre la aplicación es real; los
   tiempos, errores y valoraciones de cada perfil son derivaciones de un modelo con reglas
   explícitas, iguales en las dos mediciones, y deben leerse como predicciones.
2. **El KLM modela a un usuario que sabe qué hacer.** Los tiempos de los perfiles sin
   experiencia son un piso, no un promedio.
3. **Redondeo de la satisfacción general.** P6 queda en 3,6, redondeado a 4. Sin redondeo, el
   porcentaje sería 83 % y el criterio se seguiría cumpliendo.
4. **Catálogo chico.** Con dos eventos vigentes, buscar no requiere filtros.
5. **Escala de pantalla.** Se midió a 100 %. La aplicación no declara compatibilidad con DPI,
   que es una limitación conocida de RNF-03.
6. **Accesibilidad.** Se midió el árbol de UI Automation, pero no se probó con un lector de
   pantalla real.

## 12. Conclusión

- **El estudio se hizo en dos vueltas:** medir, corregir y volver a medir.
  - **La primera medición** encontró que la aplicación funcionaba para quien ya sabe comprar
    en línea, pero dejaba atrás a los dos perfiles que RNF-01 nombra: el que no tiene
    experiencia técnica y el que no usa mouse. Hubo dos problemas mayores: la X dejaba la
    aplicación invisible, y nada indicaba dónde se escribe la cantidad.
  - **Se corrigieron esos dos problemas y un tercero:** «Comprar» sin entradas. De paso se
    cerraron dos agujeros de seguridad: el alta como administrador y la falta de cierre de
    sesión.
  - **La segunda medición**, con el mismo método, muestra que los seis perfiles completan la
    tarea sin asistencia, la satisfacción buena pasa del 66,7 % al 100 % y el SUS sube de
    70,0 a 78,8.
- **Todos los criterios comprometidos se cumplen.**
- **La segunda medición encontró un hallazgo nuevo**, el encabezado cortado, que también se
  corrigió.
- **Lo que queda son diez hallazgos menores**, por 12 horas. Los encabeza el teclado en el
  login y el pago.

## 13. Fuera del alcance de este informe

Las dos observaciones de seguridad del informe anterior están resueltas:
- **El alta pública ya no permite elegir «Propietario»** (T-22). El arreglo también está en la
  capa de negocio, porque el combo permitía escribir el valor a mano.
- **«Volver al inicio» cierra la sesión** (T-23).

## Reproducción

```
powershell -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar          # base de pruebas con la semilla
powershell -ExecutionPolicy Bypass -File tools\Usabilidad\Medir.ps1                # docs/usabilidad/medicion.txt
powershell -ExecutionPolicy Bypass -File tools\Usabilidad\VerificarCierreConX.ps1  # U-01 sobre el ejecutable
python tools\Usabilidad\modelo.py docs\usabilidad\modelo-resultados.md despues     # segunda medición
python tools\Usabilidad\modelo.py docs\usabilidad\modelo-resultados-antes-T22.md antes   # primera medición
```

## Referencias

- Bangor, A., Kortum, P. y Miller, J. (2009). Determining what individual SUS scores mean:
  Adding an adjective rating scale. *Journal of Usability Studies*, 4(3), 114-123.
- Brooke, J. (1996). SUS: A «quick and dirty» usability scale. En *Usability Evaluation in
  Industry*. Taylor & Francis.
- Card, S. K., Moran, T. P. y Newell, A. (1980). The keystroke-level model for user
  performance time with interactive systems. *Communications of the ACM*, 23(7), 396-410.
- Kieras, D. (2001). *Using the Keystroke-Level Model to estimate execution times*.
  University of Michigan.
- Nielsen, J. (1994). Heuristic evaluation. En J. Nielsen y R. L. Mack (eds.), *Usability
  Inspection Methods*. John Wiley & Sons.
- Nielsen, J. y Landauer, T. K. (1993). A mathematical model of the finding of usability
  problems. *Proceedings of INTERCHI'93*, 206-213.
- Wharton, C., Rieman, J., Lewis, C. y Polson, P. (1994). The cognitive walkthrough method: A
  practitioner's guide. En J. Nielsen y R. L. Mack (eds.), *Usability Inspection Methods*.
  John Wiley & Sons.
