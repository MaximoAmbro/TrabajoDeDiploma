# LA lista para el Word — de la página 1 a la 112

> ## ⚠ ANTES DE TODO: los scripts de la base de datos
>
> El ejecutable nuevo necesita cambios en tu base de EventAura. En SQL Server Management Studio,
> **elegí tu base de EventAura** y ejecutá estos scripts de la carpeta `BaseDatos/`, **en este
> orden**, antes de abrir el ejecutable nuevo. Los cuatro se pueden ejecutar más de una vez sin
> dañar nada:
>
> 1. **`04-seguridad-ingreso.sql`**, solo si tu base es anterior al 11/11 (si no estás seguro,
>    ejecutalo). Agrega lo que usa el control de ingreso por QR. **Si no lo corrés**, el control
>    de ingreso falla.
> 2. **`05-check-estado-entradas.sql`**. Agrega la restricción de los cuatro estados posibles de
>    una entrada. Si encuentra una entrada con otro estado, muestra cuál y se detiene sin
>    modificar nada: en ese caso, avisale a JC. **Si no lo corrés**, la aplicación funciona, pero
>    la base acepta estados de entrada que no existen.
> 3. **`06-prioridad-temporal.sql`**. Agrega la ventana de prioridad de la lista de espera
>    (RN-35). **Si no lo corrés, no se puede iniciar ninguna compra:** el ejecutable nuevo lee una
>    columna que tu base todavía no tiene.
> 4. **`07-indice-unico-seguros.sql`**. Agrega el índice que impide dos seguros sobre una misma
>    entrada (RN-28). Antes de crearlo busca entradas con más de un seguro: si encuentra alguna,
>    muestra cuáles y se detiene con el error 50007, sin borrar ni modificar nada. En ese caso,
>    no sigas y avisale a JC. **Si no lo corrés**, la aplicación funciona, pero la base acepta dos
>    seguros para la misma entrada, que son dos cargos al cliente.

> ## ↻ CAMBIÓ DESPUÉS DEL 16/11: volver a pegar
>
> Lo que estaba sin marcar en una versión anterior de esta lista (la del 16/11, commit `654e1d9`, o
> la del 17/11, commit `0e1fc5a`) y cambió después. Si todavía no lo pegaste, no hagas nada: pegá
> lo que dice ahora. Si ya lo pegaste, volvé a pegarlo. **Las cifras de la batería cambiaron:**
> eran 145 casos y 271 ejecuciones, y son 148 y 276. La cobertura de reglas no cambió (39 de 41).
>
> | Dónde | Qué cambió | Qué volver a pegar |
> |---|---|---|
> | p. 1, carátula | Línea nueva con el commit del código que se entrega | Solo esa línea (la entrada «Esto no espera a JC: el commit del código») |
> | p. 6, historial, fila de la versión 2.6 | «CP-62 a CP-145» pasa a «CP-62 a CP-148» | La fila 2.6 entera (la fecha sigue esperando a JC) |
> | p. 7, «Qué contiene este informe», fila «Verificación» | «ciento cuarenta y cinco» pasa a «ciento cuarenta y ocho» | La celda «Poner» de esa fila |
> | p. 7, «Resultados principales», cuarto punto | Antes estaba marcado; después llevó la cifra de mutación, y ahora dice «Ciento cuarenta y ocho casos» en lugar de «Ciento cuarenta y cinco casos», y «sobre el código congelado» sin «para la entrega» | El punto entero |
> | Diagrama de clases, vista 3 («Dominio: entrada y seguro») | El seguro pasa de 0..* a 0..1 por entrada | La imagen `clases-3-dominio-entrada.png`, al mismo ancho |
> | Diagrama de dominio actualizado (p. 88) | Al final del texto se agrega una oración sobre el seguro | Solo esa oración (está en su lugar de la lista) |
> | p. 89, entidad-relación de compras | El seguro pasa de 0..* a 0..1 por entrada, con el índice único | La imagen `er-2-compras.png`, al mismo ancho |
> | pp. 90 a 97, párrafo «El estudio y el rediseño de la interfaz» | El punto de U-09: ahora, al cerrar la pantalla de éxito, vuelve al listado | El punto de U-09 |
> | p. 106, «Criterio de construcción» y tabla «Nomenclatura de los ejes» | «ciento cuarenta y cinco» y «145» pasan a «ciento cuarenta y ocho» y «148» | El párrafo y la fila «Casos de prueba» |
> | p. 107, matriz principal | RF-06 suma CP-147 y CP-148; RF-07 suma CP-146 | Las filas RF-06 y RF-07 |
> | p. 108, tabla «Cobertura de reglas…» | La fila de CP-99 suma RN-34; hay tres filas nuevas al final, CP-146 a CP-148 | La fila de CP-99 y las tres nuevas |
> | p. 108, el párrafo que sigue a esa tabla | Empieza con «Los ciento cuarenta y ocho casos» | El párrafo |
> | p. 108, tabla «Casos de prueba incorporados en la versión 2.6» | El título dice «CP-62 a CP-148»; cambió la fila de CP-99; hay tres filas nuevas al final | El título, la fila de CP-99 y las tres nuevas |
> | p. 108, «Origen de los casos» | Oración nueva al final del tercer párrafo, sobre CP-146 a CP-148 | El tercer párrafo («Los casos CP-91 a CP-115…») |
> | p. 108, tabla «Análisis de cobertura» | Última fila: 5 de 14 (35,7 %), por U-09 con su prueba | La última fila |
> | p. 109, oración inicial y tabla de brechas, filas G-02 y G-06 | G-02: «CP-11 a CP-148». G-06: la acción suma lo que corrigió la versión 2.6 y quedan cinco hallazgos, no diez; la oración inicial lo dice igual | La oración inicial y las dos filas |
> | Sección nueva «Metodología de verificación» (después de la p. 109) | La medición final de mutación (73 de 77); «La batería…» dice ciento cuarenta y ocho casos y doscientas setenta y seis ejecuciones; el párrafo «En la medición final quedaron cuatro…» termina distinto (M-22 corregido) | **La sección entera**, desde su título hasta el párrafo «Qué queda fuera de la medición», inclusive. Es más seguro que buscar párrafo por párrafo |
> | p. 111, trabajo futuro, punto 1 | Se borró «, y el regreso a un armado vacío de U-09» | La celda «Poner» del punto 1 |
> | p. 87, diagrama de estados | La devolución sale ahora de «Sin ingreso», no del borde de «Confirmada»: una entrada ingresada no se puede devolver. La nota y los dos párrafos del texto lo dicen | La imagen `estados-entrada.png`, al mismo ancho (16,0 cm), y los dos párrafos del texto del diagrama |
> | p. 88, texto del diagrama de robustez | Párrafo nuevo al final: por qué el Cliente aparece como actor y como entidad | Solo ese párrafo (está en su lugar de la lista) |
> | p. 89, texto del entidad-relación | Párrafo nuevo al final: por qué Entradas guarda el usuario y qué es la cantidad vendida | Solo ese párrafo (está en su lugar de la lista) |
> | pp. 52 y 53, figura 2 del instructivo | Pasa a una versión recortada, con otro archivo y otro ancho (10,7 cm) | La figura 2, si pegaste la entera. La figura 1 no cambia |

> ## ⏸ NO PEGAR TODAVÍA: la lista completa
>
> **Todo lo que no lleva la marca ⏸ se pega hoy, sin preguntar.** Lo único que queda marcado son
> datos que tiene que mandar JC: si se pega antes, el documento dice algo falso. Cada
> uno tiene la marca también en su lugar de la lista, para que te frene aunque llegues sin leer
> esto.
>
> | Dónde | Qué no se pega | De qué depende |
> |---|---|---|
> | p. 1 y pie de todas las páginas | Comisión, turno, legajos, año | Datos de JC |
> | p. 6, historial | La fecha de las filas nuevas | El día en que se pega (JC) |
> | p. 9 | El período «entre junio y diciembre de 2025» | El año que decida JC |
> | pp. 12 a 14, registro de interesados | El rol de Juan Cruz | Dato de JC |
> | pp. 15 a 17, cronograma de hitos | Las nueve «Fecha estimada: x» | Datos de JC |
> | p. 44, Minuta Nº 2 | Si el profesor figura «de la cátedra (rol simulado)» | Dato de JC |
> | p. 45, prototipos | La frase sobre Figma | Si existe el archivo de Figma (JC) |

> ## ⚠ Las páginas 63 a 67 (y el código de las 102 a 105): bloques de código que Word estropea sin que se note
>
> Word cambia `"` por `“ ”`, `'` por `’`, `--` por `–` y `->` por `→`. Dentro de un
> `CREATE TABLE` o de un método C# eso deja el bloque **mal y con aspecto correcto**.
>
> **Antes de pegar cualquier bloque de código:**
> 1. *Archivo › Opciones › Revisión › Opciones de Autocorrección*. En las pestañas
>    **«Autoformato mientras escribe»** y **«Autoformato»**, destildá «Comillas rectas por
>    comillas tipográficas», «Guiones (--) por guiones (—)» y «Símbolos con caracteres
>    especiales». En **«Autocorrección»**, destildá «Reemplazar texto mientras escribe».
> 2. **Pegá con *Pegado especial › Texto sin formato*** (Ctrl+Alt+V), nunca con Ctrl+V.
> 3. Aplicá al bloque el estilo de código que ya usan esas páginas: fuente **Consolas 9 pt**,
>    interlineado sencillo, sin sangría de primera línea. Si el documento tiene un estilo con
>    nombre para el código, usá ese.
> 4. **Comprobá cada bloque:** buscá en él `“`, `”`, `’` y `–` (Ctrl+F). Tiene que dar cero.
>
> Al terminar el código, podés volver a activar el autoformato.

## Cómo usar esta lista

- **Una sola pasada, de arriba abajo.** Cada entrada dice la página (de la versión 2.4 que está
  en `Documentacion/EventAura_v2.pdf`), qué hacer y el texto exacto.
- **Cómo copiar sin perder tablas:** abrí este archivo en VS Code con la vista previa
  (Ctrl+Shift+V), seleccioná ahí y pegá en Word. Las tablas llegan como tablas.
- **«Dice» / «Poner»:** buscá en Word el texto de «Dice» (si no aparece entero, buscá sus
  primeras cinco o seis palabras: el PDF parte los renglones) y reemplazalo por «Poner».
- **Las líneas en cursiva que empiezan con *Razón:*** son para JC: no se pegan.
- **⏸ ESPERA A JC** y **⏸ NO PEGAR TODAVÍA:** no la apliques; saltéala. Están todas en la tabla del principio.
- **Las páginas se corren** a medida que pegás. Buscá siempre por el texto, no por el número.
- **No hay nada para decidir.** Si algo no encaja con lo que ves en el Word, no lo resuelvas:
  anotalo y avisale a JC.

## Cifras del documento (definitivas)

| Dato | Valor hoy | Dónde aparece en esta lista |
|---|---|---|
| Reglas vigentes | 41 (cuarenta y una) | pp. 6, 7, 106, 108, 109, 110 |
| Reglas con caso de prueba | 39 de 41 = 95,1 % | pp. 8, 108, 109, 110, sección nueva después de la 109 |
| Reglas sin caso | RN-07 y RN-33 | pp. 8, 108, 109, 110, 111 |
| Casos de prueba | 148 (ciento cuarenta y ocho), CP-01 a CP-148 | pp. 6, 7, 106, 107, 108, 109, sección nueva |
| Ejecuciones | 276, todas correctas, sobre `dba674c`, el código que se entrega: la corrió Codex y la repitió Claude Code por separado, con el mismo resultado | sección nueva |
| Mutación | 73 de 77 (94,8 %) sobre `65225e7`, el código congelado para medirla; la primera medición, 51 de 77 (66,2 %) sobre `fe04b1f`, se cita como punto de partida | p. 7, sección nueva |
| Código que se entrega | Commit `dba674c` (completo: `dba674c5d3e870003f3b194de2ee905c6d83da69`) | p. 1, sección nueva |
| Casos de uso especificados | 13 (trece), CU-01 a CU-13 | pp. 7, 106, 108 |

---

## Página 1 — Carátula, y pie y encabezado de todas las páginas

**⏸ ESPERA A JC** (comisión, turno, legajos, año y si es «Maximo» o «Máximo»). Cuando lleguen:
@@- docs/CORRECCIONES-DOCUMENTO.md :: ^## 1\. Carátula :: ^Los datos que faltan están

**Esto no espera a JC: la versión, en las 112 páginas.** Cada página muestra «EventAura —
versión 2.4» junto al nombre de la sección y el número de página. Pasa a «EventAura — versión
2.6». Se cambia una vez en el encabezado o pie donde está (si el documento tiene secciones con
encabezados distintos, en cada una) y se comprueba en tres páginas al azar.

*Razón:* el historial de revisión de la p. 6 cierra con la versión 2.6; si el encabezado sigue en
2.4, el documento se contradice en cada página.

**Esto tampoco espera a JC: el commit del código, en la carátula.** Agregar una línea al pie de la
carátula, debajo de «Rosario, Argentina» y del año, con la misma letra que el resto de la
carátula y un punto más chica si no entra:

> Código entregado: commit dba674c5d3e870003f3b194de2ee905c6d83da69

*Razón:* la sección «Metodología de verificación» distingue el código medido (`65225e7`) del que
se entrega (`dba674c`). La carátula fija cuál es el que se entrega.

## Páginas 2 a 5 — Índice

No se toca ahora: se regenera al final («Hacé esto último»).

## Página 6 — Historial de revisión

**Agregar** estas siete filas al final de la tabla. «fecha» es el día en que se pegan los
cambios: **⏸ ESPERA A JC** solo para la fecha; el resto se pega. Las seis primeras son la versión
2.5 (las correcciones del 4 al 11 de noviembre); la última, la 2.6, que es la que se entrega.

| Fecha | Versión | Autor | Descripción del cambio |
|---|---|---|---|
| «fecha» | 2.5 | Riggio, Juan Cruz | Reglas de negocio alineadas con el prototipo: se incorporan RN-38 (capacidad del local) y RN-39 (preventa para suscriptores), que el sistema ya aplicaba, y se reescribe RN-35 (prioridad de los suscriptores en la lista de espera, coherente con RN-37). Se detallan RN-06, RN-18, RN-23, RN-24, RN-26, RN-29 y RN-31 (brecha G-05). |
| «fecha» | 2.5 | Riggio, Juan Cruz | Revisión de seguridad: control de ingreso por QR (RN-18), cambio de contraseña con la contraseña actual (RN-40) y verificación de identidad y de perfil en la capa de negocio (RN-41 y RN-42). Reglas vigentes: 41. Brecha G-07. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Instructivo del requerimiento core: estado de implementación actualizado. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Anexo A02: consulta de RPT-01 corregida, catálogo, indicadores, diseño técnico y ejemplos ajustados a la implementación. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Matriz de trazabilidad: casos de prueba CP-11 a CP-61. RF-09 se reclasifica de Could a Won't have según la regla de repriorización (brecha G-03). Brecha G-04 resuelta. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Informe de pruebas de usabilidad rehecho como estudio simulado medido sobre la aplicación real: medición instrumentada de la interfaz, evaluación heurística, recorrido cognitivo y Keystroke-Level Model con seis perfiles, en dos mediciones: antes y después de corregir los tres hallazgos de mayor peso (U-01, U-04 y U-05). Hallazgos U-01 a U-14, cuatro de ellos corregidos con prueba de regresión, y nuevo plan de mejora (brecha G-06). RNF-03 alineado con la plataforma Windows Forms, con su limitación de escalado declarada. |
| «fecha» | 2.6 | Riggio, Juan Cruz | Respecto de la 2.5: arquitectura e implementación descriptas sobre el prototipo, con los veinticinco diagramas regenerados desde el código (casos de uso, dominio, clases, patrones, secuencia, estados, robustez, actividad y entidad-relación); especificaciones CU-01 a CU-08 alineadas con el prototipo y especificaciones nuevas CU-09 a CU-13; anexo de estimación de costos. Interfaz rediseñada, figuras 1 a 4 del instructivo tomadas sobre ella, y relación entre los hallazgos de usabilidad y el rediseño. RN-35 con ventana de prioridad para los suscriptores. Numeración de las reglas: 41 vigentes, de RN-01 a RN-42; RN-27 sigue dada de baja (versión 2.0) y su número no se reutiliza, y las reglas nuevas se numeraron a continuación: RN-38 y RN-39 en la versión 2.5, y RN-40, RN-41 y RN-42 con la revisión de seguridad. Matriz de trazabilidad con los casos CP-62 a CP-148 y cobertura de 39 de las 41 reglas; brecha G-07 resuelta. Sección nueva «Metodología de verificación»: batería de pruebas, criterio de cobertura y prueba de mutación de las defensas de seguridad. Minutas y prototipos en una sola copia, dentro de «Definición de requerimientos». Carátula y pie: comisión, turno, legajos y año de entrega. |

## Página 7 — Resumen ejecutivo, «Qué contiene este informe»

| Fila | Dice | Poner |
|---|---|---|
| Reglas de negocio | «Treinta y seis reglas vigentes que fijan las condiciones bajo las cuales opera el sistema» | «Cuarenta y una reglas vigentes que fijan las condiciones bajo las cuales opera el sistema» |
| Diseño | «Requerimiento core con su instructivo de ocho pasos, guion de interfaz, especificaciones de ocho casos de uso y modelo de dominio completo.» | «Requerimiento core con su instructivo de ocho pasos, guion de interfaz, arquitectura en cuatro capas, especificaciones de trece casos de uso y diagramas de dominio, clases, patrones, secuencia, estados, robustez, actividad y entidad-relación.» |
| Validación | «Informe de pruebas de usabilidad con cinco participantes y anexo A02 con cinco reportes de gestión.» | «Informe de pruebas de usabilidad (simulado, medido sobre la aplicación) con seis perfiles, y anexo A02 con cinco reportes de gestión.» |
| Verificación | «…casos de uso, pruebas y hallazgos.» | «…casos de uso, ciento cuarenta y ocho casos de prueba automatizados y hallazgos.» |

En «Resultados principales», **reemplazar** el punto «Usabilidad por encima del promedio»:

> Usabilidad por encima del promedio. La evaluación arrojó un puntaje SUS de 74,5 frente a un
> valor de referencia de 68, con una tasa de éxito del 80 % y un tiempo medio de 4 min 56 s,
> dentro del límite comprometido de cinco minutos.

**Por:**

> Usabilidad por encima del promedio, medida dos veces. El estudio, simulado sobre mediciones
> de la aplicación real, encontró que los perfiles sin experiencia y de solo teclado
> necesitaban asistencia; se corrigieron las causas y se volvió a medir. Resultado final: un
> puntaje SUS de 78,8 frente a la referencia de 68, los seis perfiles completan la tarea sin
> asistencia y el tiempo medio es de 2 min 35 s, dentro del límite de cinco minutos.

**Mismo lugar, agregar un cuarto punto** al final de «Resultados principales», con la misma
viñeta:

> Pruebas que detectan defectos, medidas. Ciento cuarenta y ocho casos de prueba automatizados
> verifican treinta y nueve de las cuarenta y una reglas de negocio. Para comprobar que esas
> pruebas detectarían un defecto real, se quitó a propósito cada una de las defensas de
> seguridad del sistema, una por vez: la primera medición mostró cuáles no advertía la batería,
> se escribieron las pruebas que faltaban y la medición final, sobre el código congelado,
> detectó setenta y tres de las setenta y siete que una prueba puede detectar.

*Razón:* coherencia. El resumen ejecutivo anuncia los resultados principales y la versión 2.6
agrega la sección «Metodología de verificación»; sin este punto, el resumen no la menciona.

## Página 8 — Resumen ejecutivo, «Limitaciones declaradas»

**Reemplazar:**

> Las sesiones de usabilidad son simuladas y deberán repetirse con participantes reales.

**Por:**

> Las sesiones de usabilidad son simuladas: el comportamiento de la interfaz se midió sobre la
> aplicación real, y los tiempos y valoraciones de cada perfil se derivaron de esa medición.

**Reemplazar:**

> Y la cobertura de verificación alcanza al 22,2 % de las reglas de negocio, concentrada en
> la lógica de fidelización.

**Por:**

> Y la cobertura de verificación alcanza al 95,1 % de las reglas de negocio: dos de las
> cuarenta y una no tienen todavía un caso de prueba automatizado, y ninguna de esas dos
> rechaza operaciones.

## Página 9

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-01 · p\. 9 :: ^\*\*B-02

**Objetivo específico 1, «Cómo medirlo». Reemplazar:**

> validado por al menos dos revisiones de parte del “cliente” ficticio (documentadas en
> minutas).

**Por:**

> validado por al menos dos revisiones de parte del cliente ficticio, documentadas en minutas
> simuladas.

**⏸ ESPERA A JC:** «entre junio y diciembre de 2025» depende del año que decida JC.

## Página 10

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-16 · p\. 10 :: ^\*\*B-17

**Objetivo específico 3** (empieza en la p. 10 y sigue en la 11). Reemplazar el objetivo
completo, desde su número hasta su «Cuándo evaluarlo», **por:**

> 3. Definir un programa de fidelización económicamente sostenible para los clientes frecuentes
>    ○ Qué lograr: establecer las reglas de un programa por niveles y de una suscripción, de modo
>      que el beneficio otorgado no comprometa la contribución de cada operación.
>    ○ Cómo medirlo: las reglas del programa quedan documentadas con su origen y verificadas por
>      al menos cinco casos de prueba, y el descuento del nivel más alto (7,5 %) no supera el
>      descuento máximo admisible calculado en la factibilidad económica (8,62 %).
>    ○ Cuándo evaluarlo: antes de la entrega de la versión completa del prototipo.

**Objetivo específico 4, «Qué lograr». Reemplazar:**

> mediante encuestas y entrevistas cortas a los participantes de las pruebas de prototipo.

**Por:**

> mediante un estudio de usabilidad simulado sobre el prototipo, con perfiles de participantes
> definidos y cuestionarios de satisfacción y SUS cuyas respuestas se derivan de mediciones
> sobre la aplicación.

## Página 11

**Objetivos específicos 5 y 6.** Reemplazar cada objetivo completo, desde su número hasta su
«Cuándo evaluarlo», **por:**

> 5. Resguardar los datos de pago de los clientes
>    ○ Qué lograr: definir el tratamiento de los datos de tarjeta en la compra, de modo que la
>      organización no conserve datos sensibles de pago.
>    ○ Cómo medirlo: ningún registro de pago conserva el número completo de la tarjeta, sino
>      solo sus cuatro últimos dígitos y un token, verificado por casos de prueba.
>    ○ Cuándo evaluarlo: al completar el flujo de compra del prototipo.

> 6. Disponer de información de gestión para las decisiones comerciales
>    ○ Qué lograr: contar con información sintetizada sobre ventas, fidelización y adopción de
>      beneficios, que la administración use para decidir precios, promociones y programación de
>      eventos.
>    ○ Cómo medirlo: al menos tres reportes de gestión, cada uno con un ejemplo y la decisión que
>      permite tomar.
>    ○ Cuándo evaluarlo: al cierre de la práctica, junto con el prototipo final.

## Páginas 12 a 14 — Registro de interesados

**⏸ ESPERA A JC** (confirma el rol de Juan Cruz). Cuando confirme, las tres cosas van juntas:

1. **Título:** «Registro de interesados» pasa a «Registro de interesados (simulado)».
2. **Debajo del Interesado 2, agregar** con el mismo formato:

> **Interesado 3: Juan Cruz Riggio (Desarrollador)**
> - Organización: Universidad Abierta Interamericana (UAI)
> - Localización: Rosario, Argentina
> - Rol en el proyecto: desarrollador; responsable del alcance, de las pruebas y de la
>   documentación
> - Expectativas principales: cumplir con los requisitos académicos, entregar en tiempo y forma
>   y recibir retroalimentación clara del cliente y del profesor
> - Influencia: Alta
> - Fase de mayor interés: construcción del prototipo, pruebas y entrega final
> - Clasificación: Interno – Apoyo

   Los interesados que siguen pasan a numerarse del 4 al 8.
3. **Debajo del título, agregar** (ya con la numeración nueva):

> Registro simulado, como las minutas. Son ficticios:
> - el Interesado 1, Jorge Gastón Gómez, de Ticketera Rosario S.A., el cliente del caso de
>   negocio;
> - el Interesado 7, Carlos Quintana;
> - el Interesado 8, el equipo de soporte.
>
> Los interesados 5 y 6 describen roles (los usuarios finales y la pasarela de pago), no
> personas que hayan participado. Son reales el equipo de desarrollo y el docente evaluador.

## Páginas 15 a 17 — Cronograma de hitos

**⏸ ESPERA A JC:** las nueve «Fecha estimada: x».

**Fase 1. Reemplazar:**

> Conformación del equipo (proyecto individual).

**Por:**

> Conformación del equipo (dos integrantes).

**Fase 5. Reemplazar:**

> Encuesta de usabilidad aplicada a 2 o 3 usuarios de prueba.

**Por:**

> Estudio de usabilidad simulado sobre el prototipo, con perfiles de participantes definidos.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-17 · p\. 16 :: ^\*\*B-18

## Página 18

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-08 · p\. 18 :: ^\*\*B-09
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-18 · p\. 18 :: ^## 4\. Decisiones

## Página 20 — Criterios de aceptación

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-07 · p\. 20 :: ^\*\*B-08

**Criterio 10. Reemplazar la línea completa por:**

> 10. Lista de espera para eventos agotados: El sistema debe registrar al usuario en la lista,
> permitirle consultar su posición y avisarle al iniciar sesión cuando el evento vuelve a tener
> entradas.

## Página 21 — Supuestos del proyecto

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-14 · p\. 21 :: ^\*\*B-15

**Supuesto 4. Reemplazar:**

> 4. Los usuarios de prueba estarán disponibles para realizar simulaciones de compra y
> responder encuestas de usabilidad.

**Por:**

> 4. La evaluación de usabilidad se realiza como estudio simulado: los participantes son
> perfiles definidos y sus resultados se derivan de mediciones sobre el prototipo, sin sesiones
> con personas reales.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-15 · p\. 21 :: ^\*\*B-16

## Página 23 — Requerimientos funcionales

**RF-10, criterio de aceptación. Reemplazar:**

> Criterio de aceptación: El sistema debe registrar al usuario en la lista e informarle que será
> notificado.

**Por:**

> Criterio de aceptación: El sistema debe registrar al usuario en la lista y confirmarle la
> inscripción. El cliente puede consultar su posición, que respeta el orden de RN-35. Si el
> evento vuelve a tener entradas, el sistema se lo informa al iniciar sesión; el aviso no reserva
> el lugar.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-02 · p\. 23 :: ^\*\*B-03
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-03 · p\. 23 :: ^\*\*B-04

## Página 24 — Requerimientos no funcionales, punto 3 (RNF-03)

**Reemplazar** el punto 3 completo:

> 3. Compatibilidad técnica
>    ○ El prototipo debe poder visualizarse correctamente en navegadores modernos (Chrome,
>      Firefox, Edge) sin requerir instalación de software adicional.
>    ○ El diseño debe adaptarse de forma adecuada a diferentes tamaños de pantalla (notebook,
>      monitor de escritorio y, en la medida de lo posible, tablet o móvil).

**Por:**

> 3. Compatibilidad técnica
>    ○ El prototipo es una aplicación de escritorio para Windows (Windows Forms, .NET Framework
>      4.8) y debe ejecutarse en Windows 10 u 11 sin otro software que el motor de base de datos.
>    ○ Las pantallas deben entrar completas en el área útil de una notebook y de un monitor de
>      escritorio: ninguna supera los 790 píxeles de alto, y la de compra mantiene siempre a la
>      vista sus acciones principales, con desplazamiento del contenido cuando hace falta.
>    ○ **Limitación conocida.** La aplicación no declara compatibilidad con el escalado de
>      pantalla de Windows (DPI). Con una escala superior al 100 %, Windows la amplía como una
>      imagen: las pantallas mantienen su disposición y siguen siendo operables, pero el texto
>      se ve menos nítido. Se aceptó para esta versión porque resolverlo exige revisar las
>      veinte pantallas.

## Página 25 — Reglas de negocio

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-01 · p\. 25 :: ^\*\*A-02

**Misma oración, a continuación:** «las especificaciones de casos de uso (CU-01 a CU-08)» pasa a
«las especificaciones de casos de uso (CU-01 a CU-13)».

**Tabla «Usuarios y cuentas» (en la p. 25 o en la 26: es la que tiene RN-01 a RN-05).
Agregar** después de RN-05:

| RN-40 | Cambiar la contraseña exige la contraseña actual. Una contraseña actual incorrecta cuenta como intento fallido a los efectos de RN-04, y con la cuenta bloqueada el cambio se rechaza. | A | RF-02 / CU-09 |
|---|---|---|---|
| RN-41 | Un cliente solo consulta y opera sobre sus propias compras, entradas, pagos, pólizas, suscripción e inscripciones en listas de espera. El Administrador puede hacerlo sobre las de cualquier cliente. | A | RF-02 / CU-05 |
| RN-42 | La gestión de eventos, sectores y locales, la consulta de las listas de espera y el control de ingreso son exclusivos del perfil Administrador. Los reportes se rigen por RN-34. | A | RF-12 / CU-02 |

## Página 26 — Reglas de negocio

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-02 · p\. 26 :: ^> \*\*Resuelta el 14/11
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-03 · p\. 26 :: ^\*\*A-04
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-04 · p\. 26 :: ^- \*\*Mismo cambio en p\. 55
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-05 · p\. 26 :: ^- \*\*p\. 55,\*\* fila «Me devolvieron

**RN-06. Reemplazar** la fila:

| RN-06 | Un evento no puede publicarse si no tiene al menos un sector definido con capacidad mayor que cero. | A | RF-12 / CU-02 |
|---|---|---|---|

**Por:**

| RN-06 | Un evento publicado tiene en todo momento al menos un sector con capacidad mayor que cero: no puede publicarse sin sectores, ni puede eliminarse el último sector de un evento ya publicado. | E | RF-12 / CU-02 |
|---|---|---|---|

**Agregar** después de RN-11:

| RN-38 | La suma de las capacidades de los sectores de un evento no puede superar la capacidad del local donde se realiza. Se verifica al agregar o modificar un sector. | E | RF-12 / CU-11 |
|---|---|---|---|

## Página 27 — Reglas de negocio

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-06 · p\. 27 :: ^\*\*A-07

**RN-29. Reemplazar** la fila:

| RN-29 | El seguro habilita la devolución del valor de la entrada hasta cuarenta y ocho horas antes del inicio del evento. | A | RF-05 |
|---|---|---|---|

**Por:**

| RN-29 | El seguro habilita la devolución hasta cuarenta y ocho horas antes del inicio del evento. La devolución es por entrada: el cliente elige cuáles da de baja, y cada una debe estar confirmada, asegurada y sin ingreso registrado. Se reintegra el precio congelado de cada entrada menos su parte proporcional del descuento por nivel y, si con ello no queda ninguna entrada confirmada en la compra, también los servicios complementarios. No se reintegran el cargo por servicio ni la prima del seguro. El costo de los servicios devueltos se recupera del proveedor, condición que debe constar en el acuerdo con cada uno. | A | RF-05 / CU-13 |
|---|---|---|---|

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-08 · p\. 27 :: ^\*\*A-09

**Agregar** después de RN-37:

| RN-39 | Mientras no haya terminado el período de preventa de un evento, sólo un cliente con suscripción activa puede iniciar una compra de ese evento; el evento sigue visible en el listado para todos los clientes. Un evento sin período de preventa no tiene esta restricción. Es el beneficio de preventa que enuncia RN-37. | A | RF-13 / CU-01 |
|---|---|---|---|

**RN-35. Reemplazar** la fila:

| RN-35 | La lista de espera respeta estrictamente el orden de inscripción. | E | RF-10 / CU-07 |
|---|---|---|---|

**Por:**

| RN-35 | La lista de espera se ordena primero por los clientes que se inscribieron con suscripción activa y después por el resto; dentro de cada grupo, por orden de inscripción. La condición de suscriptor se toma al inscribirse y no cambia aunque la suscripción venza o se cancele después. Es el beneficio de prioridad que enuncia RN-37. | E | RF-10 / CU-07 |
|---|---|---|---|

**Agregar al final del texto de RN-35** (Codex construyó la ventana en `1525202`):

> Cuando un sector vuelve a tener una entrada disponible, porque se devolvió, venció una reserva o
> se rechazó un pago, durante los quince minutos siguientes solo pueden comprarla los inscriptos
> en la lista de espera que se anotaron con suscripción. Si el evento no tiene inscriptos con
> suscripción, la entrada queda a la venta de inmediato. El plazo es un parámetro configurable.

*Razón:* código. `RepositorioSectores.Reservar` rechaza la reserva con RN-35 dentro de la ventana,
en la misma transacción que reserva el cupo; el parámetro es `PRIORIDAD_ESPERA_MINUTOS` (15) y lo
prueba CP-125 (ocho variantes). Con la ventana construida, «prioridad en lista de espera» de las
pp. 27, 30, 40, 55 y 82 ya es exacto y no se toca.

## Página 28 — Factibilidad económica

| Dice | Poner |
|---|---|
| «$19.356» | «$19.355» |

*Razón:* recalculado; todas las contribuciones, el 8,62 % y el 9,5 % quedan iguales.

## Página 29 — Costo del programa

**Reemplazar:**

> El costo anual de los descuentos, calculado sobre el juego de datos de prueba, asciende a
> $3.758.161.

**Por:**

> El costo anual estimado de los descuentos, calculado sobre el juego de datos simulado,
> asciende a $3.758.161.

## Página 32 — Niveles de fidelidad

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-09 · p\. 32 :: ^\*\*A-10

## Página 33 — Parámetros y conflictos

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-10 · p\. 33 :: ^\*\*A-11
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-11 · p\. 33 :: ^## 3\. Requerimientos y alcance

## Página 35 — Priorización

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-04 · p\. 35 :: ^\*\*B-05

**Sección nueva, debajo de «Escala de esfuerzo»** (después del párrafo que termina «…no por
estimación absoluta.»). Pegar completo:

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*Texto para pegar:\*\*$ :: ^\*\*Cuentas verificadas:\*\*

## Página 36 — Priorización de requerimientos funcionales

**Fila RF-09. Reemplazar:**

| RF-09 | Recomendaciones personalizadas | Could | 8 | El más costoso de los no obligatorios y sin objetivo específico asociado. |
|---|---|---|---|---|

**Por:**

| RF-09 | Recomendaciones personalizadas | Won't (desde v2.5) | 8 | Era Could: el más costoso de los no obligatorios y sin objetivo específico asociado. Se excluye en la Fase 8 por la regla de repriorización (brecha G-03). |
|---|---|---|---|---|

**Título de la tabla «Distribución del esfuerzo»:** pasa a «Distribución del esfuerzo
estimado», y la columna «Puntos», a «Puntos estimados». La tabla no se recalcula.

**En «Observación sobre el desvío»:** «concentra el 62,8 % del esfuerzo» pasa a «concentra el
62,8 % del esfuerzo estimado». **Debajo de esa observación, agregar:**

> **Nota de la versión 2.5.** RF-09 se reclasificó como Won't have en la Fase 8. Sus ocho
> puntos se liberan sin afectar a ningún requerimiento Must, como prevé la mitigación
> descrita: la categoría Could funcionó como amortiguador. La tabla de distribución se
> conserva como la estimación aprobada.

## Página 37

**Tabla «Priorización de requerimientos no funcionales», fila RNF-03. Reemplazar** la
justificación:

> Deseable, pero la evaluación del prototipo se realiza en escritorio.

**Por:**

> Deseable, pero la evaluación del prototipo se realiza en escritorio. Se cumple en tamaño de
> pantalla (casos CP-12 y CP-25); la nitidez con escalado de Windows superior al 100 % queda
> como limitación conocida (ver «Requerimientos no funcionales», punto 3).

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-05 · p\. 37 :: ^\*\*B-06

**Tabla «Requerimientos excluidos — categoría Won't have». Agregar** la fila:

| W-07 | Recomendaciones personalizadas (RF-09) | Reclasificado desde Could en la versión 2.5. No responde a ningún objetivo específico ni tiene caso de uso, y es el requerimiento no obligatorio más costoso (8 puntos). |
|---|---|---|

## Página 38 — Orden de implementación derivado

**Fila de la Fase 8:**
- en la columna Requerimientos, «RF-09 y RF-10, según margen disponible» pasa a «RF-10 (RF-09
  pasó a Won't have en la versión 2.5)»;
- en la columna Pts, «11» pasa a «3».

**En «Riesgo de concentración»:** «es decir el 52,6 % del esfuerzo total» pasa a «es decir el
52,6 % del esfuerzo total estimado».

## Páginas 39 y 40 — Minutas y prototipos (primera copia): se borra

Queda la copia de las pp. 44 y 45, que está dentro de «Definición de requerimientos», la sección
que pide el punto 11 de la guía del profesor (minutas, prototipos y especificación de
requerimientos). Esta se borra:

1. **Borrar** desde el título «Minutas de reunión (simuladas)» del principio de la p. 39 hasta la
   nota del final del prototipo 6 de la p. 40, inclusive: «Nota: Los prototipos fueron
   desarrollados en Figma (o herramienta equivalente) y se entregan como documento
   complementario en PDF y en versión interactiva.»
2. **No borrar** lo que sigue en la p. 40: el título «Descripción funcional y alcance» y su texto
   quedan (la entrada siguiente los corrige).
3. **Antes de borrar, copiá las dos fechas** de esta copia: la Minuta 1 dice «05/06/2025» y la
   Minuta 2, «17/06/2025». Van en la p. 44 (ver «Página 44»).

*Razón:* la guía decide. El historial de la p. 6 ya fecha la versión 0.1 el 05/06/2025; la copia
que queda dice «Fecha: x».

## Página 40 — Descripción funcional

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-09 · p\. 40 :: ^> Si se elige la opción \(b\) de D-6

## Página 41 — Alcance y restricciones

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-10 · p\. 41 :: ^> Si se aplica la opción \(b\) de D-6
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-12 · p\. 41 :: ^\*\*B-13
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-13 · p\. 41 :: ^\*\*B-14

## Páginas 42 y 43 — Segunda copia de los requerimientos (se reemplaza por una remisión)

1. **Borrar** desde el subtítulo «Especificación de requerimientos funcionales» (p. 42) hasta el
   final del ítem «11. Visualización de reportes simulados» y su criterio (p. 43).
2. **Dejar** el título «Definición de requerimientos» y, debajo, **pegar:**

> La especificación de los trece requerimientos funcionales y de los cinco no funcionales, cada
> uno con su criterio de aceptación, está en la sección «Definición de requerimientos» (pp. 22 a
> 24). Su priorización está en «Priorización de requerimientos» (pp. 35 a 38), y su relación con
> las reglas, los casos de uso y las pruebas, en la matriz de trazabilidad. Esta sección reúne el
> material de relevamiento del que surgieron: las minutas de reunión y los prototipos de
> interfaz.

3. **Los números de página de esa remisión** («pp. 22 a 24», «pp. 35 a 38») se ajustan al final,
   cuando se regenera el índice (ver «Hacé esto último»).

## Página 44 — Minutas (la copia que queda)

| Lugar | Dice | Poner |
|---|---|---|
| Minuta Nº 1 | «Fecha: x» | «Fecha: 05/06/2025» |
| Minuta Nº 2 | «Fecha: x» | «Fecha: 17/06/2025» |

El año de las dos fechas sigue la decisión del año de entrega (ver «Hacé esto último»).
**⏸ ESPERA A JC** solo para un dato: si en la Minuta Nº 2 «Profesor (evaluador académico)» pasa a
«Profesor de la cátedra (rol simulado)».

La remisión de la p. 42 queda entera, con su última oración («Esta sección reúne el material de
relevamiento…»): las minutas y los prototipos siguen en esta sección.

## Página 45 — Prototipos (la copia que queda)

**⏸ ESPERA A JC** para la frase de Figma: «Los siguientes prototipos fueron desarrollados en Figma
y entregados como archivo complementario en formato PDF y versión interactiva.» Si el archivo no
existe, va: «Los siguientes prototipos se elaboraron como bocetos de las pantallas principales y
representan el flujo completo de compra previsto en esta etapa.»

Estas correcciones se pegan ya, en la copia de la p. 45:

| Ítem | Dice | Poner |
|---|---|---|
| 3 | «Pantalla de carrito de compra. Muestra el resumen completo del paquete elegido, descuentos aplicados y monto total.» | «Resumen del paquete. En la misma pantalla del armado, muestra el detalle del paquete elegido, el descuento por nivel, el cargo por servicio, el seguro y el monto total.» |
| 5 | «Muestra mensaje de éxito, código de operación ficticio, resumen de compra y eventos recomendados.» | «Muestra mensaje de éxito, número de pedido, resumen de compra y la ubicación de los tickets en PDF con su código QR.» |
| 6 | «Permite ver el historial de compras, nivel de rankeo alcanzado (Bronce, Plata, Oro) y beneficios aplicables.» | «Permite ver el historial de compras y, debajo, las entradas de la compra elegida, con la devolución de las aseguradas; y contratar o cancelar la suscripción. El nivel alcanzado se muestra al armar el paquete.» |
| 7 | «Simula reportes gráficos y tablas con ventas por evento, adopción de beneficios, y usuarios por nivel de fidelidad.» | «Presenta en tablas, exportables a PDF, cinco reportes: ventas por evento, ranking de paquetes, usuarios por nivel de fidelidad, adopción de seguros y suscripciones, y demanda insatisfecha. Los filtros del reporte van arriba de la tabla, y la lectura del resultado, debajo.» |

*Razón:* JC eligió la opción A de Codex para Historial (dos listas apiladas: compras arriba,
entradas del pedido debajo) y para Reportes (filtros arriba en dos filas, tabla a todo el ancho,
lectura y acciones al pie). Codex las construyó así en `eeaf8a5` (16/11); falta que JC confirme,
mirando las dos pantallas, que el texto las describe.

## Páginas 47 a 49 — Guion de interfaz: las tres capturas

**Reemplazar las tres imágenes.** Las de hoy muestran la interfaz de antes del rediseño (la de
la p. 47 dice «Show Password»). Las nuevas están en
`Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/`. Van a **16 cm de ancho**, con la proporción
bloqueada (clic derecho › *Tamaño y posición* › «Bloquear relación de aspecto»):

| Página | Pantalla | Archivo | Letra más chica, impresa |
|---|---|---|---|
| 47 | Inicio de sesión | `p47-inicio.png` | 13,4 pt |
| 48 | Crear cuenta | `p48-registro.png` | 12,2 pt |
| 49 | Menú del cliente | `p49-menu-cliente.png` | 14,9 pt |

No uses las de la carpeta `150/`: son las mismas pantallas a otra escala, para comprobación.

*Razón:* capturas de Codex, de las pantallas completas del código que se entrega, sin ventanas
encima. A 16 cm ninguna letra queda por debajo de 12 pt.

**P. 47, mismo lugar, ya:** en la lista del Menú Inicial, **dice** «Botones para iniciar sesión,
cambiar la contraseña y crear una cuenta nueva.» **Poner:** «El botón para iniciar sesión, que
también responde a la tecla Enter, y los enlaces para cambiar la contraseña y crear una cuenta
nueva.»

*Razón:* pasada de coherencia del 16/11 sobre el rediseño. `frmInicio` tiene `AcceptButton` y los
dos accesos como enlaces («Cambiar contraseña» y «Crear Cuenta»).

## Página 48

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-01 · p\. 48 :: ^\*\*E-02

## Página 49

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-02 · p\. 49 :: ^\*\*E-03

## Página 50 — Instructivo: estado de implementación

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^## 8\. Página 50 :: ^## 9\. Páginas 52

## Página 51

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-03 · p\. 51 :: ^\*\*E-04
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-04 · p\. 51 :: ^\*\*E-05

## Página 52

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-05 · p\. 52 :: ^\*\*E-06

## Páginas 52 y 53 — Figuras 1 a 4 del instructivo

Reemplazar cada «Espacio reservado para captura de pantalla» por la imagen de la tabla. **Se
pegan hoy las cuatro.** **La figura 2 cambió:** es una versión recortada, en otra carpeta. Si ya
pegaste la entera, reemplazala. La figura 1 es la misma de antes. Están en `Pruebas/Figuras-Instructivo-2026-11-16/100/`, todas de un mismo pedido
(Paula, nivel Plata, dos entradas de Campo y una noche de hotel: total $99.625, como el ejemplo
de la p. 54). **No uses las de `docs/capturas/instructivo/`: son las de antes del rediseño.**
Proporción bloqueada en todas. Los epígrafes no cambian.

| Epígrafe | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| Figura 1 — Listado de eventos disponibles (pasos 1 y 2) | `figura1-listado-eventos.png` | **16,0 cm** | 7,2 pt |
| Figura 2 — Pantalla de armado de paquete (pasos 3 a 5) | **`Pruebas/Recortes-Instructivo-2026-11-16/figura2-armado-paquete-recorte.png`** (otra carpeta) | **10,7 cm** (su tamaño real) | 9 pt; el texto más chico de la entera, 10 pt |
| Figura 3 — Resumen del paquete con el descuento aplicado (paso 6) | `figura3-resumen-descuento.png` | **10,5 cm** (su tamaño real) | 10 pt |
| Figura 4 — Confirmación de compra y comprobante emitido (pasos 7 y 8) | **dos imágenes, una debajo de la otra, con un solo epígrafe:** `figura4b-confirmacion.png` arriba y `ticket.png` abajo | **16,0 cm** y **12,0 cm** | 7,9 pt y unos 8 pt |

*Razón de las figuras 1 y 2* (decisión de JC, con el recorte de Codex). **La figura 2 se
recortó:** conserva el evento, el nivel, las entradas y el hospedaje, y el resumen que sale del
recorte ya está en la figura 3, así que el instructivo completo no pierde nada. Va a su tamaño
real, como la figura 3: a 16 cm la letra subiría a 13,5 pt, pero la imagen tiene 404 px de ancho y
se imprimiría ampliada al 150 %, borrosa. **La figura 1 queda entera, a 7,2 pt:** un recorte suficiente para mejorarla le quitaría los filtros, las columnas o el botón Comprar, que es lo que la figura tiene que mostrar.

*Razón:* la figura 4 que armó Codex en una sola imagen (`figura4-confirmacion-y-comprobante.png`)
mide 1519 px: a 16 cm quedaría al 40 %, con la letra en unos 3,6 pt, ilegible (regla 16 del
profesor). Sus dos mitades están en la misma carpeta. La letra impresa sale de la letra de cuerpo
de la interfaz, Segoe UI de 10 pt (`docs/SISTEMA-DISENO.md`). Antes de pegar, JC confirma que el
texto de los pasos 1 a 8 (pp. 51 y 52) describe lo que muestran, en particular el indicador de
pasos y la pantalla de éxito.

## Página 54 — Ejemplo resuelto

Sin cambios.

## Página 55 — Situaciones particulares y mensajes de validación

**Fila «Bajé de nivel…». Dice:** «El nivel se reevalúa el primer día de cada mes. Una compra que
sale de la ventana de doce meses recién impacta en esa fecha.» **Poner:** «El nivel se reevalúa
una vez por mes, la primera vez que el cliente arma o confirma una compra en ese mes. Una compra
que sale de la ventana de doce meses recién impacta en esa reevaluación.»

**Fila «Me devolvieron una compra…». Dice:** «El contador desciende de inmediato, pero el nivel
se conserva hasta la reevaluación mensual.» **Poner:** «Si se devolvieron todas las entradas de
la compra, el contador desciende de inmediato; si se devolvió una parte, la compra sigue
contando. En los dos casos el nivel se conserva hasta la reevaluación mensual.»

**«Mensajes de validación».**
- **En la oración inicial, dice:** «Cada bloqueo se acompaña de un mensaje que indica la causa y
  la acción necesaria para resolverla.»
- **Poner:** «Cada bloqueo se acompaña de un mensaje que indica la causa y la acción necesaria
  para resolverla, o deja deshabilitado el control cuando la acción no corresponde.»
- **En la columna «Mensaje mostrado»:**

| Regla | Dice | Poner |
|---|---|---|
| RN-08 | «El sector seleccionado no dispone de esa cantidad de entradas.» | «En el sector Campo quedan 3 entradas.» |
| RN-12 | «No es posible adquirir más de seis entradas por operación.» | «Se pueden comprar como máximo 6 entradas por operación.» |
| RN-13 | «El paquete debe incluir al menos una entrada.» | Sin mensaje: «Comprar» y la casilla del seguro quedan deshabilitados hasta elegir una entrada. |
| RN-14 | «Este evento no ofrece el servicio seleccionado.» | Sin mensaje: la casilla del servicio aparece deshabilitada. |
| RN-15 | «La reserva expiró. Debe reiniciar la compra.» | «Venció el plazo de la reserva y se liberó el cupo. Volvé a armar la compra.» |
| RN-32 | «El pago fue rechazado. Puede reintentar o cancelar la operación.» | «Pago rechazado: Rechazada por el emisor (simulación: tarjeta terminada en 0000). Se liberaron las entradas. Podés reintentar antes de que venza la reserva.» |
| RN-10 | «Este evento se encuentra agotado. ¿Desea sumarse a la lista de espera?» | «Entradas agotadas. Anotate en la lista de espera. Los avisos de disponibilidad se muestran al iniciar sesión.» |
| RN-36 | «Usted ya figura en la lista de espera de este evento.» | «Ya estás en la lista de espera de este evento. Podés consultar tu posición en la compra; los avisos de disponibilidad se muestran al iniciar sesión.» |

## Páginas 57 a 61 — «Implementación» pasa a «Arquitectura e implementación»

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^## Anexo 1 — Texto para D-1 :: ^\*Razón:\* código\. Las 4 capas

## Página 62 — Entidades

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-08 · p\. 62 :: ^\*Razón:\* base\. `BaseDatos/01-esquema\.sql` y `GestorCompras\.Iniciar`

## Páginas 63 a 67 — Código real (⚠ releé el aviso de arriba antes de empezar)

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^## Anexo 2 — Textos para D-2 :: ^\*\*pp\. 68 a 70, código de las pruebas

## Páginas 68 a 70 — Pruebas de la lógica de descuento

**p. 68, primer párrafo. Reemplazar:**

> El objetivo específico 3 del proyecto exige registrar al menos cinco casos con distintos
> perfiles de usuario; el conjunto aquí presentado lo duplica.

**Por:**

> El objetivo específico 3 del proyecto exige verificar las reglas del programa con al menos
> cinco casos de prueba; el conjunto aquí presentado lo duplica.

*Razón:* el objetivo 3 cambió en la p. 10.

El listado de código **queda como está**. Debajo del título «Implementación de las pruebas»,
**agregar:**

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*pp\. 68 a 70, código de las pruebas :: ^## Anexo 3

## Página 71 — Diagramas de casos de uso (dos figuras)

**Reemplazar** la figura que está bajo «Diagramas de caso de uso» por:
- `docs/diagramas/casos-uso-cliente.png`, a **16,0 cm** de ancho;
- en la página siguiente, con el título «Diagramas de caso de uso (Administrador)»,
  `docs/diagramas/casos-uso-administrador.png`, a **16,0 cm**.

Siempre con la proporción bloqueada (clic derecho › *Tamaño y posición* › *Bloquear relación de
aspecto*). **Si la achicás, deja de leerse.**

**Texto, debajo del título:**

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 1\. Casos de uso >> ^\*\*Texto para el documento :: ^\*\*Justificación de relaciones:\*\*

## Páginas 72 a 85 — Especificaciones CU-01 a CU-08

**En las ocho especificaciones, borrar el diagrama chico** que está debajo del título de cada una
(son ocho figuras, una por caso de uso). Las relaciones de cada caso de uso ya están en el
diagrama general de la p. 71.

**Títulos** (en el título de la especificación y en el campo «Id y nombre»):

| Dice | Poner |
|---|---|
| «CU-06 – Suscribirse al sistema» | «CU-06 – Gestionar suscripción» |
| «CU-07 – Gestionar listas de espera» | «CU-07 – Consultar listas de espera» |
| «CU-08 – Ver reportes de ventas» | «CU-08 – Generar reportes» |

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-09 · CU-01 :: ^### Diagramas \(pp\. 71

**CU-05, agregar un paso al final del flujo principal**, con el número que siga al último:

> Si el usuario es un Cliente y algún evento de su lista de espera volvió a tener entradas, el
> sistema se lo informa antes de abrir su menú: «Hay entradas disponibles nuevamente en:», la
> lista de esos eventos y «Podés volver al listado para consultar y comprar. No es una reserva de
> lugar.».

*Razón:* código, `a2f4b55`. `frmInicio` muestra ese mensaje con `GestorListaEspera.AvisosAlIngresar`
(CP-118).

**CU-07, paso 4. Reemplazar:**

> 4. El sistema muestra la lista de Clientes en orden de inscripción.

**Por:**

> 4. El sistema muestra la lista de Clientes en orden de prioridad: primero los inscriptos
> con suscripción activa al momento de inscribirse y después el resto, cada grupo por orden
> de inscripción (RN-35).

## Después de la página 85 — Especificaciones nuevas CU-09 a CU-13

**Insertar** a continuación de CU-08, con el mismo formato que CU-01 a CU-08 (los mismos estilos
de título y de campos). No llevan diagrama chico.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*CU-09 – Cambiar contraseña\*\*$ :: ^\*\*Lo que no se especifica y por qué\.\*\*

## Página 86 — Modelo de dominio conceptual y diagrama de clases

**Modelo de dominio conceptual:** reemplazar la figura por `docs/diagramas/dominio-conceptual.png`,
a **16,0 cm**. Texto debajo del título:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 7\. Modelo de dominio conceptual >> ^\*\*Texto para el documento :: ^\*\*Justificación de relaciones:\*\*

**Diagrama de clases:** reemplazar la figura por siete figuras, **una por página**, cada una con
su subtítulo:

| Subtítulo | Imagen (`docs/diagramas/`) | Ancho |
|---|---|---|
| 1. Dominio: cuentas y catálogo | `clases-1-dominio-catalogo.png` | **15,9 cm** (página entera) |
| 2. Dominio: compra y pago | `clases-2-dominio-compra.png` | **16,0 cm** |
| 3. Dominio: entrada y seguro | `clases-3-dominio-entrada.png` | **16,0 cm** |
| 4. Negocio: gestores de la compra | `clases-4-negocio-compra.png` | **16,0 cm** |
| 5. Negocio: administración y cuenta | `clases-5-negocio-administracion.png` | **16,0 cm** |
| 6. Negocio: verificación de identidad | `clases-6-autorizacion.png` | **16,0 cm** |
| 7. Datos: repositorios | `clases-7-datos.png` | **15,1 cm** (página entera) |

Texto, debajo del título «Diagrama de clases»:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 2\. Diagrama de clases >> ^\*\*Texto para el documento :: ^\*\*Justificación de relaciones, vistas 1 a 3

**Sección nueva «Patrones de diseño»**, a continuación de las vistas de clases. Tres figuras:

| Subtítulo | Imagen (`docs/diagramas/`) | Ancho |
|---|---|---|
| Singleton y Repository | `clases-8a-patron-singleton-repository.png` | **16,0 cm** |
| Strategy | `clases-8b-patron-strategy.png` | **15,5 cm** |
| Jerarquía de Reporte | `clases-8c-jerarquia-reportes.png` | **16,0 cm** (es baja: puede compartir página con el texto) |

Texto, debajo del título:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 3\. Patrones >> ^\*\*Texto para el documento :: ^(\*\*Justificación|\| Vista \| Relación)

## Página 87 — Secuencia y estados

**Diagrama de secuencia – Comprar paquete:** reemplazar la figura por tres, una por página:
`secuencia-compra-1-reserva.png`, `secuencia-compra-2-pago.png` y
`secuencia-compra-3-emision.png`, las tres a **16,0 cm**. Texto debajo del título:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 4\. Secuencia >> ^\*\*Texto para el documento :: ^\*\*Correspondencia con el código

**Diagrama de estados – Ciclo de vida de la entrada:** reemplazar la figura por
`estados-entrada.png`, a **16,0 cm** (ocupa la página entera: el título pasa a la página nueva).
Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 5\. Transición >> ^\*\*Texto para el documento :: ^(\*\*|## 6\.)

## Página 88 — Robustez, dominio actualizado y actividad

**Diagrama de robustez (ICONIX):** reemplazar la figura por `robustez-comprar-paquete.png`, a
**16,0 cm**. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 9\. Robustez >> ^\*\*Texto para el documento :: ^\*\*Justificación de conexiones:\*\*

**Diagrama de dominio actualizado:** reemplazar la figura por dos, una por página:
`dominio-actualizado-1-cuentas-catalogo.png` y `dominio-actualizado-2-compra.png`, las dos a
**16,0 cm**. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 8\. Modelo de dominio actualizado >> ^\*\*Texto para el documento :: ^\*\*Justificación

**Agregar al final del texto anterior:**

> Una entrada tiene a lo sumo un seguro. La base de datos lo garantiza con un índice único sobre
> la entrada asegurada, de modo que un mismo cliente no puede recibir dos cargos de seguro por la
> misma entrada (RN-28).

*Razón:* matriz de coherencia, comprobaciones 4 y 6. El script `07-indice-unico-seguros.sql`
agrega el índice; la vista 3 de clases y el entidad-relación de compras ya muestran 0..1.

**Diagrama de actividad:** el título «Diagrama de Actividad (flujo del Cliente)» pasa a
«Diagrama de actividad (proceso de venta)». Reemplazar la figura (que sigue en la p. 89) por
tres, una por página: `actividad-1-armado.png` a **16,0 cm**, `actividad-2-pago.png` a
**13,6 cm** y `actividad-3-ingreso.png` a **16,0 cm**. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 10\. Actividad >> ^\*\*Texto para el documento :: ^(\*\*|## )

## Página 89 — Entidad-relación

Debajo de «DER – Diagrama entidad-relación» (hoy la página tiene solo el título), insertar
`er-1-usuarios-catalogo.png` a **15,4 cm** y, en la página siguiente, `er-2-compras.png` a
**14,6 cm**. Las dos ocupan la página entera. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 6\. Entidad-relación >> ^\*\*Texto para el documento :: ^(\*\*|## )

## Páginas 90 a 97 — Informe de pruebas de usabilidad

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^## 11b\. Páginas 90 a 97 :: ^## 12\. Página 99

**Agregar al final de la introducción del informe** (después de la entrada anterior):

> **El estudio y el rediseño de la interfaz.** Las dos mediciones de este estudio corresponden a
> la interfaz anterior al rediseño visual de la versión 2.6. El rediseño tomó como punto de
> partida los hallazgos del estudio y corrigió varios de ellos:
> - U-02: la tecla Enter inicia sesión y el campo Mail es el primero del orden de tabulación;
> - U-03: los avisos llevan un título en castellano que nombra la operación, como «No se puede
>   armar el paquete» o «Pago no realizado», y el código de la regla pasa al final del mensaje;
>   desaparecieron «Campo vacíos» e «ingrese usuario»;
> - U-11: «Show Password» pasó a «Mostrar contraseña»;
> - U-13: se quitó el botón «Modificar» que había quedado en el inicio de sesión;
> - U-09: el recorrido cognitivo registró en el paso 8 que el usuario no entendía en qué punto de
>   la compra estaba (pregunta 4). La compra muestra ahora un indicador de pasos, termina en una
>   pantalla de éxito con el número de pedido y la carpeta de los tickets, y al cerrarla vuelve al
>   listado de eventos.
>
> Siguen abiertos U-06, U-07, U-08, U-10 y U-12. La interfaz rediseñada no se volvió a evaluar con
> las herramientas de este estudio; hacerlo queda como trabajo futuro.

*Razón:* decisión de JC del 16/11: se declara, no se vuelve a medir. Cada corrección la verificó
Claude Code en el código, leyéndolo: U-02 en `frmInicio` (`AcceptButton` y orden de tabulación,
`027527f`); U-03 en `Visual/Avisos.cs` (`36a0d80`) y en los títulos de los avisos; U-11 en
`frmInicio`, `frmCrearUsuario` y `frmCambiarContraseña` (`7fc5558`); U-13 en `frmInicio`; el
indicador (`96fd6ec`) y la pantalla de éxito (`027527f`); la vuelta al listado de U-09, en
`FrmCompraCliente` (`9f3a2ed`), con su prueba de regresión, CP-148. Abiertos, también por lectura: U-06 (el
código de seguridad se borra sin aviso), U-07 («Cancelar pago» sin «No» por defecto), U-08 (el
filtro de fecha sigue sin rótulo), U-10 (el foco de «Crear cuenta» empieza en Contraseña) y U-12
(ningún control tiene nombre accesible).

## Página 98 — Anexo A02, introducción

| Dice | Poner |
|---|---|
| «CU-08 – Ver reportes de ventas define el flujo de interacción» | «CU-08 – Generar reportes define el flujo de interacción» |

*Razón:* el título de CU-08 cambió (pp. 84 y 85).

## Páginas 99 a 105 — Anexo A02

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^## 12\. Página 99 :: ^## 17\. Página 106

## Página 106 — Matriz: criterio y nomenclatura

**Criterio de construcción. Reemplazar:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y seis
> reglas, los ocho casos de uso y los diez casos de prueba resulta ilegible en formato A4

**Por:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las cuarenta y una
> reglas, los trece casos de uso y los ciento cuarenta y ocho casos de prueba resulta ilegible en
> formato A4

**Tabla «Nomenclatura de los ejes».** Reemplazar las filas de reglas y de casos de prueba, y en
la de casos de uso cambiar la cantidad:

| Fila | Dice | Poner |
|---|---|---|
| Reglas de negocio | «RN-xx \| 36 \| Reglas de negocio (RN-27 dada de baja)» | «RN-xx \| 41 \| Reglas de negocio (RN-27 dada de baja)» |
| Casos de prueba | «CP-xx \| 10 \| Desarrollo de la lógica de descuento por nivel» | «CP-xx \| 148 \| CP-01 a CP-10: Desarrollo de la lógica de descuento por nivel. CP-11 a CP-148: Matriz de trazabilidad, catálogo de casos incorporados» |
| Casos de uso | la cantidad «8» | «13» |

## Página 107 — Matriz principal

**Reemplazar la tabla completa por:**

| RF | OE | MoSCoW | Reglas de negocio | CU | Casos de prueba | Hall. |
|---|---|---|---|---|---|---|
| RF-01 | OE-2 | Must | RN-01, RN-02, RN-03 | CU-04 | CP-14, CP-15, CP-26, CP-44, CP-45, CP-55 | — |
| RF-02 | OE-2 | Must | RN-04, RN-05, RN-40, RN-41 | CU-05, CU-09 | CP-27, CP-28, CP-29, CP-46, CP-55, CP-66 a CP-68, CP-91, CP-92, CP-94 a CP-98, CP-105, CP-107, CP-108, CP-113, CP-114, CP-124, CP-130 | — |
| RF-03 | OE-2 | Must | RN-09 | CU-01 | CP-16, CP-17, CP-18, CP-30, CP-81, CP-106 | — |
| RF-04 | OE-2 | Must | RN-08, RN-12, RN-33 | CU-01 | CP-20, CP-31, CP-40, CP-49, CP-54, CP-94 | H-03 |
| RF-05 | OE-2 | Must | RN-13, RN-14, RN-28, RN-29 | CU-01, CU-13 | CP-11, CP-13, CP-19, CP-22 a CP-25, CP-31, CP-41, CP-48, CP-50, CP-57, CP-59, CP-61, CP-79, CP-81, CP-119, CP-122, CP-140 | H-04 |
| RF-06 | OE-5 | Must | RN-15, RN-16, RN-30, RN-31, RN-32 | CU-01 | CP-07, CP-32, CP-33, CP-34, CP-39, CP-43, CP-52, CP-80, CP-122, CP-127, CP-129, CP-147, CP-148 | H-02 |
| RF-07 | OE-2 | Must | RN-17, RN-18 | CU-01, CU-12 | CP-32, CP-33, CP-65, CP-80, CP-99 a CP-104, CP-110, CP-111, CP-123, CP-128, CP-131 a CP-136, CP-142, CP-145, CP-146 | — |
| RF-08 | OE-3 | Should | RN-19 a RN-24 | CU-01, CU-03 | CP-01 a CP-06, CP-08, CP-09, CP-11, CP-42, CP-91, CP-92, CP-115, CP-138, CP-139 | H-01, H-05 |
| RF-09 | — | Won't | — | — | — | — |
| RF-10 | OE-6 ind. | Could | RN-10, RN-35, RN-36 | CU-07 | CP-24, CP-37, CP-83, CP-84, CP-117, CP-118, CP-125, CP-126, CP-137 | — |
| RF-11 | OE-6 | Should | RN-34 | CU-08 | CP-35 a CP-38, CP-85 a CP-90, CP-109, CP-143, CP-144 | — |
| RF-12 | OE-2 | Must | RN-06, RN-07, RN-11, RN-38, RN-42 | CU-02, CU-10, CU-11 | CP-21, CP-56, CP-60, CP-69 a CP-75, CP-82, CP-85, CP-93, CP-94, CP-123, CP-141 | — |
| RF-13 | OE-3 | Should | RN-25, RN-26, RN-37, RN-39 | CU-06 | CP-10, CP-58, CP-61, CP-76, CP-77, CP-78, CP-94, CP-121 | — |

El párrafo «La indicación "ind." en RF-10…» queda igual. **Agregar** debajo:

> La categoría Won't de RF-09 corresponde a la reclasificación de la versión 2.5 (brecha
> G-03). Los requerimientos no funcionales no figuran en la matriz porque no originan casos
> de uso; sus casos de prueba son: RNF-01, CP-112; RNF-02, CP-47, CP-51, CP-53, CP-62, CP-63,
> CP-71, CP-76, CP-85, CP-88, CP-112, CP-116, CP-119 y CP-120; RNF-03, CP-12, CP-25, CP-64, CP-69,
> CP-85 y CP-116. Un caso
> figura en más de una fila cuando prueba una regla de una fila desde la operación de otra.
> RN-41 y RN-42 son reglas de autorización: valen para todas las operaciones del sistema y no
> para un requerimiento solo. Se ubican en RF-02, que establece la sesión de la que dependen, y
> en RF-12, que reúne las operaciones del Administrador. CP-94 figura en varias filas porque
> verifica las dos reglas en las operaciones de cada una; CP-123, en RF-07 y RF-12, porque
> verifica RN-42 en el control de ingreso.

**Agregar a continuación** el subtítulo «Trazabilidad del requerimiento core» y:

> La matriz principal llega hasta los casos de prueba. Para el requerimiento core, la
> trazabilidad sigue hasta los diagramas y el código:

| Requerimiento | Caso de uso | Diagramas | Código |
|---|---|---|---|
| RF-05, Armado de paquetes personalizados | CU-01 Comprar paquete | Robustez «Comprar paquete»; secuencia «Comprar paquete» (reserva, pago y emisión); actividad del armado y del pago; transición de estados de la entrada; clases, vistas 2, 3, 4 y 7 | `Visual/Cliente/FrmCompraCliente.cs`, `frmPago.cs` y `frmConfirmacion.cs`; `Repo/GestorPaquetes.cs`, `GestorCompras.cs` y `GestorFidelizacion.cs`; `Datos/RepositorioCompras.cs`, `RepositorioSectores.cs`, `RepositorioEntradas.cs` y `RepositorioPagos.cs`; `Entidade/Paquete.cs` y `Compra.cs` |

*Razón:* matriz de coherencia, comprobación 1 («Cada RF core tiene CU, robustez, secuencia y
código»; se comprueba con una tabla RF → CU → diagramas → archivos). La matriz principal no
llegaba a diagramas ni a archivos. Los archivos existen en el commit que se entrega, `dba674c`.

**Tabla «Trazabilidad inversa de los objetivos específicos», fila OE-4. Reemplazar** la
evidencia «Informe de pruebas de usabilidad: SUS 74,5 y 80 % de satisfacción» **por** «Informe de
pruebas de usabilidad (simulado, medido sobre la aplicación): SUS 78,8 y 100 % de satisfacción
buena o muy buena». El estado sigue «Cumplido».

**Misma tabla, fila OE-5.** El objetivo 5 cambió de nombre (p. 11). **Reemplazar** «Modelar la
integración con una pasarela segura» **por** «Resguardar los datos de pago de los clientes», y la
evidencia **por** «Flujo de pago de CU-01; reglas RN-30 y RN-31; CP-32 verifica que el pago
conserva solo los cuatro últimos dígitos y el token». El estado sigue «Cumplido».

**Misma tabla, filas OE-3 y OE-6.** Cambiar solo el nombre del objetivo: OE-3, «Definir un
programa de fidelización económicamente sostenible para los clientes frecuentes»; OE-6,
«Disponer de información de gestión para las decisiones comerciales». La evidencia y el estado
quedan.

## Página 108 — Cobertura

**Advertencia sobre OE-4, al principio de la página. Reemplazar** el párrafo completo por:

> **Advertencia sobre OE-4.** El objetivo se declara cumplido sobre la base del Informe de
> pruebas de usabilidad, que es simulado según se declara en su encabezado, como las minutas.
> A diferencia de un estudio enteramente supuesto, el comportamiento de la interfaz se midió
> sobre la aplicación real, dos veces, y cada resultado por perfil se deriva de esa
> medición. En la primera medición el porcentaje de perfiles con satisfacción buena o muy
> buena (66,7 %) no alcanzaba su umbral. Se corrigieron las causas medidas, y en la segunda
> llegó al 100 %, con un promedio de 4,36: el objetivo se cumple por las dos vías.

**Tabla «Cobertura de reglas de negocio por casos de prueba». Reemplazar la tabla completa** por
la de CP-01 a CP-61:

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^### Tabla «Cobertura de reglas de negocio por casos de prueba» :: ^\*\*Reemplazar\*\* el párrafo que sigue a la tabla

**y agregar a continuación** estas filas (CP-62 a CP-148):

@@ generado/cobertura.md :: ^\| CP-62 — :: ^FIN-COBERTURA

**El párrafo que sigue a la tabla. Reemplazar:**

> Los diez casos verifican ocho reglas distintas —RN-16, RN-19, RN-20, RN-21, RN-22, RN-23,
> RN-24 y RN-37— sobre un total de treinta y seis vigentes, lo que arroja una cobertura del
> 22,2 %. Las veintiocho reglas restantes carecen de verificación automatizada.

**Por:**

> Los ciento cuarenta y ocho casos verifican treinta y nueve reglas distintas sobre un total de
> cuarenta y una vigentes, lo que arroja una cobertura del 95,1 %. Son RN-01 a RN-06, RN-08 a
> RN-26, RN-28 a RN-32 y RN-34 a RN-42. Todas las reglas que rechazan una operación tienen un
> caso que provoca ese rechazo y comprueba que el sistema queda como estaba. Las dos restantes
> carecen de verificación automatizada: RN-07 y RN-33. Ninguna rechaza operaciones: describen un
> cálculo (RN-07) y la moneda (RN-33).
>
> Varios casos no suman reglas porque verifican otra cosa: que la interfaz comunique al
> usuario reglas que ya funcionan, que es el problema que señaló el informe de usabilidad.
> CP-39 a CP-42 son la regresión de los hallazgos H-02 a H-05: muestran el temporizador de
> RN-15, el cupo de RN-08, el aviso de RN-28 y los umbrales de RN-19, pero no verifican esas
> reglas.

**Agregar** a continuación la tabla «Casos de prueba incorporados en la versión 2.5», con las
filas de CP-11 a CP-61:

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^Va a continuación del párrafo anterior\. CP-01 a CP-10 :: ^### Tabla «Análisis de cobertura»

**y, debajo, una segunda tabla** con el título «Casos de prueba incorporados en la versión 2.6
(CP-62 a CP-148)»:

@@ generado/tabla-casos.md :: ^\| Caso \| Condición :: ^FIN-TABLA

**Después de las dos tablas, agregar:**

> **Origen de los casos.** Durante la construcción, cada sección del prototipo dejó anotadas
> las pruebas de interfaz que no podían hacerse en ese momento, con la acción, el resultado
> esperado y el cambio que las originaba: ciento veintinueve en total.
>
> Las sesenta y cinco del recorrido del cliente se ejecutaron sobre la aplicación real,
> conducida con UI Automation, y cada falla corregida quedó como caso automático (CP-13 a
> CP-25).
>
> De las sesenta y cuatro restantes:
> - cincuenta y siete se convirtieron en casos automáticos (CP-66 a CP-90), que abren los
>   formularios reales contra una base de prueba y se ejecutan en cada corrida;
> - dos ya estaban cubiertas por casos existentes;
> - dos quedaron sin objeto porque describían pantallas que se rediseñaron;
> - dos verifican el entorno de pruebas y se resolvieron con scripts;
> - una, el aspecto del reporte en tres lectores de PDF, la verificó una persona, junto con
>   la lectura de su código QR con un celular.
>
> Los casos CP-91 a CP-115, CP-123 y CP-124 surgieron de la revisión de seguridad (brecha G-07),
> y CP-126 a CP-145, de la prueba de mutación (sección «Metodología de verificación»). CP-125
> verifica la ventana de prioridad de los suscriptores (RN-35).
> CP-116, CP-119 y CP-120 verifican el rediseño visual de la interfaz; CP-117 y CP-118, la
> consulta de la posición y el aviso de la lista de espera, y CP-121 y CP-122 completan las dos
> reglas que solo se mostraban en pantalla (RN-26 y RN-28). Los tres últimos cierran lo que
> quedaba abierto al congelar el código: CP-146, el control de perfil del registro de ingreso que
> la prueba de mutación encontró sin prueba; CP-147, el índice único que impide dos seguros
> sobre una entrada, y CP-148, la regresión del hallazgo de usabilidad U-09.
>
> Se prefirió convertir las pruebas en casos automáticos porque una prueba repetible deja
> evidencia en cada ejecución. Una verificación hecha a mano solo vale el día en que se hizo.

**Tabla «Análisis de cobertura». Reemplazar la tabla completa:**

| Dimensión evaluada | Total | Cubierto | Sin cobertura | % |
|---|---|---|---|---|
| Objetivos específicos con evidencia documentada | 6 | 6 | — | 100 % |
| Requerimientos funcionales con caso de uso | 13 | 12 | RF-09 (Won't) | 92,3 % |
| Casos de uso con requerimiento que los origine | 13 | 13 | — | 100 % |
| Reglas de negocio con origen declarado | 41 | 41 | — | 100 % |
| Requerimientos funcionales con caso de prueba | 13 | 12 | RF-09 (Won't) | 92,3 % |
| Reglas de negocio verificadas por caso de prueba | 41 | 39 | 2 reglas | 95,1 % |
| Hallazgos de usabilidad con acción de mejora | 6 | 6 | — | 100 % |
| Hallazgos con la acción implementada en el prototipo | 6 | 6 | — | 100 % |
| Hallazgos con prueba de regresión asociada | 6 | 6 | — | 100 % |
| Hallazgos del estudio v2.5 (U-01 a U-14) con acción de mejora | 14 | 14 | — | 100 % |
| Hallazgos del estudio v2.5 corregidos, con prueba de regresión | 14 | 5 | 9 hallazgos | 35,7 % |

**Debajo, agregar:**

> Las filas de hallazgos H-01 a H-06 corresponden al estudio de la versión 2.4, cuyas
> correcciones están hechas. Las dos últimas corresponden al estudio rehecho en la versión 2.5
> (ver la brecha G-06). Los cinco con prueba de regresión son U-01, U-04, U-05, U-14 y U-09. El
> rediseño de la versión 2.6 corrigió además U-02, U-03, U-11 y U-13, verificados en el código
> pero sin una prueba propia, por eso no se cuentan en esa fila.

**«Lectura del cuadro». Reemplazar:**

> La trazabilidad hacia adelante, del requerimiento a su verificación, es el punto débil del
> proyecto: sólo tres de trece requerimientos cuentan con pruebas, y todas ellas se
> concentran en la lógica de fidelización.

**Por:**

> La trazabilidad hacia adelante, del requerimiento a su verificación, pasó de ser el punto
> débil del proyecto a estar completa en el nivel de requerimientos: los doce en alcance
> cuentan con pruebas, contra tres de trece en la versión 2.4. El único sin pruebas es
> RF-09, excluido. En el nivel de reglas, treinta y nueve de cuarenta y una tienen caso
> automatizado, incluidas todas las que rechazan una operación. Cómo se verificó que esos casos
> detectan los defectos que dicen detectar se explica en «Metodología de verificación».

*La subsección «Resistencia de las pruebas de seguridad» que figuraba acá pasó, ampliada, a la
sección nueva «Metodología de verificación», después de la p. 109. No se agrega nada en la
p. 108.*

## Página 109 — Brechas detectadas

**Oración inicial. Reemplazar:**

> El análisis identificó cuatro brechas. La primera fue resuelta a partir de este mismo
> análisis; las tres restantes quedan documentadas con su acción correctiva.

**Por:**

> El análisis identificó siete brechas. G-01 se resolvió a partir de este mismo análisis.
> En la versión 2.5 se cerró G-03 y se resolvieron G-04 y G-05; en la 2.6, G-07. G-02 quedó resuelta en el
> nivel de requerimientos y abierta en el de reglas. G-06 quedó resuelta en nueve de sus catorce
> hallazgos, entre ellos los tres de mayor peso, y abierta en los cinco restantes, con su plan.

**Reemplazar la tabla completa:**

| Nº | Brecha | Sev. | Estado | Acción |
|---|---|---|---|---|
| G-01 | Los casos de uso CU-02 (Gestionar eventos) y CU-06 (Gestionar suscripción) no contaban con un requerimiento funcional que los originara, pese a que las reglas RN-06, RN-07 y RN-11 por un lado, y RN-25, RN-26 y RN-37 por el otro, presuponen su existencia. | Alta | Resuelta | Se incorporaron los requerimientos RF-12 (Gestión de eventos) y RF-13 (Contratación de suscripciones) a la Definición de requerimientos, y se recalculó la priorización MoSCoW. |
| G-02 | En la versión 2.4, sólo tres de los trece requerimientos poseían casos de prueba (23,1 %) y la cobertura sobre reglas de negocio era del 22,2 %. | Alta | Resuelta en requerimientos; abierta en reglas | Se incorporaron los casos CP-11 a CP-148. Los doce requerimientos en alcance tienen pruebas. La cobertura de reglas sube al 95,1 % (39 de 41), con todas las reglas que rechazan operaciones; quedan dos reglas sin caso (RN-07 y RN-33). |
| G-03 | RF-09 (Recomendaciones personalizadas) no responde a ningún objetivo específico ni posee caso de uso asociado. | Media | Cerrada | Se reclasificó como Won't have en la Fase 8, según la regla de repriorización (W-07). |
| G-04 | En la versión 2.4, ninguno de los seis hallazgos del informe de usabilidad tenía caso de prueba de regresión. | Media | Resuelta | Las seis acciones del plan de mejora están implementadas en el prototipo y cada hallazgo tiene su regresión automatizada: H-01 en CP-13, H-02 en CP-39, H-03 en CP-40, H-04 en CP-41, H-05 en CP-42 y H-06 en CP-19. |
| G-05 | El documento no reflejaba tres reglas que el prototipo aplica: el tope de capacidad del local sobre la suma de sus sectores y la preventa exclusiva para suscriptores no estaban enunciadas, y la prioridad de los suscriptores en la lista de espera contradecía RN-35, que exigía orden estricto de inscripción. Era comportamiento implementado sin una regla documentada que lo respalde. | Alta | Resuelta | Se incorporaron RN-38 (RF-12 / CU-11) y RN-39 (RF-13 / CU-01), y se reescribió RN-35 en coherencia con RN-37. Las tres tienen sus casos de prueba: RN-38 en CP-60, RN-39 en CP-61 y RN-35 en CP-83 y CP-84. |
| G-06 | El estudio de usabilidad de la versión 2.5, medido sobre la aplicación real, identificó trece hallazgos nuevos (U-01 a U-13), y la segunda medición uno más (U-14). Dos eran de severidad mayor: cerrar una pantalla con la X dejaba la aplicación en ejecución y sin ventanas, y nada indicaba dónde se escribe la cantidad de entradas. | Media | Parcialmente resuelta | Se corrigieron U-01, U-04 y U-05, cada uno con su prueba de regresión (CP-47 a CP-50), y se volvió a medir: los seis perfiles completan la tarea sin asistencia y la satisfacción buena pasa del 66,7 % al 100 %. La segunda medición encontró U-14, el encabezado de la cantidad cortado, que también se corrigió (CP-54). En la versión 2.6, el rediseño de la interfaz corrigió U-02, U-03, U-11 y U-13, y U-09 con su prueba de regresión (CP-148). Quedan cinco hallazgos menores, U-06, U-07, U-08, U-10 y U-12 (esfuerzo estimado: 7 horas), que pasan a trabajo futuro. |
| G-07 | La revisión de seguridad de la versión 2.5 encontró que RN-18 prometía un único ingreso por entrada que el prototipo no controlaba, que el cambio de contraseña no exigía la contraseña actual y que las operaciones de negocio no verificaban el perfil ni la identidad de quien las invoca. | Alta | Resuelta | Se implementó el control de ingreso con registro atómico de la primera lectura, restringido a los eventos de los locales del Administrador; el cambio de contraseña pasó a exigir la actual y a responder lo mismo exista o no la cuenta, y el negocio verifica la sesión en cada operación. Se incorporaron RN-40 a RN-42 y se completó RN-18, con los casos CP-91 a CP-115, CP-123 y CP-124. El registro atómico se demostró con diez lectores simultáneos sobre la misma entrada durante cien rondas: en cada una ingresó exactamente uno (CP-104). |

**«Observación sobre G-03». Reemplazar** el párrafo completo **por:**

> **Observación sobre G-03.** La matriz confirmó por una vía independiente la decisión ya
> adoptada en la priorización: RF-09 había sido clasificado como Could por ser el
> requerimiento más costoso sin objetivo asociado. Que el análisis de trazabilidad llegara
> al mismo resultado partiendo de otro criterio reforzó la validez de ambos. En la Fase 8 se
> aplicó lo previsto: RF-09 no se incorporó y pasó a Won't have. La brecha se cierra por
> decisión de alcance y no por construcción.

**Debajo, agregar:**

> **Observación sobre G-05.** Esta brecha recorre la cadena en sentido inverso a las otras:
> no falta una prueba para una regla, falta la regla para un comportamiento que ya existe.
> Se prefirió documentarla aunque la cobertura bajara: con 36 reglas el porcentaje habría
> sido mayor, pero habría medido un sistema distinto del construido.
>
> **Observación sobre G-07.** Como G-05, recorre la cadena al revés: RN-18 figuraba en la
> matriz como cubierta porque su caso verificaba la mitad de la regla. Un caso asociado a una
> regla no garantiza que la verifique completa; por eso, desde esta versión, cada caso de la
> tabla de cobertura nombra lo que comprueba. Y un caso que verifica la regla completa todavía
> puede no detectar su falta: eso se midió aparte, con la prueba de mutación (ver «Metodología de
> verificación»).

**Sección nueva, a continuación del párrafo final de la p. 109 y antes de «Conclusiones».**
Título con el mismo estilo que «Matriz de trazabilidad»; los subtítulos en negrita, con el
estilo de «Criterio de construcción». **Se pega entera.**

@@- docs/METODOLOGIA-VERIFICACION.md :: ^## Texto para el documento :: ^<!-- fin del texto

«Mantenimiento de la matriz» y el párrafo final de la página no cambian.

## Página 110 — Conclusiones

**«Sobre la validación con usuarios». Reemplazar** el párrafo completo:

> El hallazgo más consistente de las pruebas de usabilidad es que las dificultades no
> aparecen al operar el sistema sino al comprender sus decisiones. El resumen del paquete, el
> nivel de fidelidad y la vigencia de la reserva son reglas correctamente definidas que la
> interfaz no logra comunicar. Ninguna de las seis acciones del plan de mejora introduce un
> requerimiento nuevo: todas consisten en explicar mejor algo que el sistema ya hace.

**Por:**

> La primera ronda de usabilidad mostró que las dificultades estaban en comprender las
> decisiones del sistema; sus seis correcciones están hechas y verificadas. La segunda ronda
> se midió sobre la aplicación real y encontró un problema distinto, de control de la
> navegación: la aplicación no terminaba cuando el usuario cerraba una ventana, y la acción
> central del armado, escribir la cantidad, no se anunciaba. Eso dejaba atrás a los dos
> perfiles que RNF-01 nombra: quien no tiene experiencia técnica y quien no puede usar el
> mouse. Se corrigió y se volvió a medir con el mismo método. Los seis perfiles completaron la
> tarea sin asistencia, y la satisfacción buena pasó del 66,7 % al 100 %. Medir, corregir y
> volver a medir resultó más convincente que cualquier estimación.

**«Sobre las reglas de negocio»:** «Explicitar las treinta y seis reglas resultó más productivo de
lo esperado.» pasa a «Explicitar las cuarenta y una reglas resultó más productivo de lo
esperado.»

**Después del párrafo «Sobre la trazabilidad», agregar** un párrafo con el mismo formato
(subtítulo en negrita al principio):

> **Sobre la verificación.** Contar cuántas reglas tienen una prueba no alcanzó para saber si
> las pruebas servían. La prueba de mutación mostró que una parte de las defensas del sistema
> podía quitarse sin que ninguna prueba fallara. Cada una se registró como prueba por escribir y
> después se escribió, y una de ellas llevó a reforzar la prueba de ingresos simultáneos hasta
> demostrar una garantía que antes solo se suponía. La decisión de método más importante fue no
> reclasificar una mutación después de conocer su resultado: un porcentaje que se ajusta a
> posteriori deja de medir.

*Razón:* coherencia con la sección nueva «Metodología de verificación». El párrafo no lleva
cifras, así que no cambia cuando se congelen los números.

**Tabla «Limitaciones del trabajo», primera fila. Reemplazar:**

| Las sesiones de usabilidad son simuladas. | El objetivo específico 4 se considera cumplido de manera provisoria hasta su ejecución con participantes reales. |
|---|---|

**Por:**

| Las sesiones de usabilidad son simuladas. | La interfaz se midió sobre la aplicación real; los tiempos y las valoraciones de cada perfil son derivaciones de esa medición y deben leerse como predicciones. |
|---|---|

**Misma tabla, tercera fila. Reemplazar:**

| La cobertura de pruebas alcanza el 22,2 % de las reglas. | Diez de trece requerimientos carecen de verificación automatizada. |
|---|---|

**Por:**

| La cobertura de pruebas alcanza el 95,1 % de las reglas. | Todos los requerimientos en alcance tienen pruebas; dos reglas, ninguna de las que rechazan operaciones, carecen de verificación automatizada. |
|---|---|

**Misma tabla, agregar al final estas tres filas:**

| Dos verificaciones dependen de una persona y de un dispositivo: la lectura de los códigos QR impresos con la cámara de un celular y el aspecto del PDF de los reportes en tres lectores. | Se hicieron una vez; no se repiten en cada corrida como los casos automáticos. |
|---|---|
| No existe recuperación de contraseña por correo. | Una contraseña olvidada la restablece el Administrador; el usuario solo puede cambiarla conociendo la actual (RN-40). |
| La aplicación de escritorio accede a la base con la cuenta de Windows del usuario. | Las verificaciones de perfil del negocio (RN-34, RN-41, RN-42) protegen la operación desde la aplicación; en producción tienen que acompañarse de permisos en la base o de un servicio intermedio. |

## Página 111 — Trabajo futuro

| Punto | Dice | Poner |
|---|---|---|
| 1 | «Ejecutar las sesiones de usabilidad con participantes reales y contrastar los resultados con la línea base establecida en este informe.» | «Evaluar la interfaz rediseñada con las mismas herramientas del estudio de usabilidad, para contrastar los resultados, y aplicar lo que queda abierto de su plan de mejora (U-06, U-07, U-08, U-10 y U-12).» |
| 2 | «Extender el conjunto de pruebas al flujo de compra completo, de modo de cerrar la brecha G-02 de la matriz de trazabilidad.» | «Completar los casos de prueba de las dos reglas sin verificación (RN-07 y RN-33), de modo de cerrar la brecha G-02 de la matriz de trazabilidad.» |
| 4 | «Definir un caso de prueba de regresión por cada hallazgo de usabilidad antes de aplicar las correcciones de la Fase 6.» | «Mantener una prueba de regresión por cada hallazgo de las próximas rondas de usabilidad, como se hizo con H-01 a H-06.» |
| 5 | «Resolver la duplicación de contenido detectada entre las secciones de requerimientos, minutas y prototipos del cuerpo del informe.» | **Borrar el punto** y renumerar los siguientes: la duplicación se resolvió en esta versión (pp. 39, 40, 42 y 43). |

---

## Hacé esto último

1. **Regenerá el índice** (pp. 2 a 5): clic derecho sobre el índice › *Actualizar campos* ›
   *Actualizar toda la tabla*. Si el índice no es automático, avisale a JC antes de rehacerlo a
   mano.
2. **Comprobá que el índice tenga estas entradas nuevas:** «Estimación de costos»,
   «Arquitectura e implementación» (antes «Implementación»), las cinco especificaciones CU-09 a
   CU-13, «Patrones de diseño», «Metodología de verificación» (entre «Matriz de trazabilidad» y
   «Conclusiones»), los títulos nuevos de CU-06, CU-07 y CU-08, y que ya no estén
   «Especificación de requerimientos funcionales» de la p. 42 ni la primera copia de «Minutas de
   reunión (simuladas)» y «Prototipos de interfaz gráfica» (p. 39).
3. **Corregí los números de página que el texto cita a mano** (no se actualizan solos). Buscá
   cada uno y poné la página nueva:

| Dónde está la cita | Dice hoy | Apunta a |
|---|---|---|
| Remisión de la p. 42 (la que pegaste) | «pp. 22 a 24» | «Definición de requerimientos» |
| Remisión de la p. 42 | «pp. 35 a 38» | «Priorización de requerimientos» |
| Fila «Guion de interfaz», tabla de la p. 6 (historial) | no se toca: es historia | — |

4. **Páginas que cambian de número:** todas las que están después de la p. 35 (se agrega
   «Estimación de costos») y, sobre todo, todas desde la p. 71: entran unas 20 páginas de
   diagramas y unas 8 de especificaciones nuevas, y salen casi 2 páginas de la remisión (pp. 42
   y 43). Calculá que lo que hoy es la p. 90 queda cerca de la 116. Por eso se busca siempre por
   texto.
5. **Revisá el pie de página de las nuevas** (encabezado y pie iguales al resto).

## No toques

- **Los diagramas** que no figuran en esta lista, ni el tamaño de los que insertaste: el ancho
  indicado es el mínimo para que se lean impresos.
- **Los prototipos** (pp. 39, 40, 44 y 45) más allá de las filas de esta lista: la copia que
  queda la decide JC.
- **Las cifras de cobertura, de casos y de ejecuciones:** pegalas como están; si en el Word
  aparece otra cifra que no está en esta lista, avisale a JC.
- **El repositorio:** no hace falta tocar ningún archivo del repositorio para aplicar esta
  lista.
