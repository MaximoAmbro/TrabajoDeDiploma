# LA lista para el Word — de la página 1 a la 112

> ## ✅ HOY SOLO SE PUEDE PEGAR: introducción, planteo del problema, objetivos y justificación (pp. 9 a 11), el borrado de la primera copia de minutas y prototipos (pp. 39 y 40), las minutas (p. 44) y la sección nueva «Estimación de costos» (p. 35).
>
> **Todo lo demás está en pausa** (⏸ en su lugar): el sistema se está ampliando y cambian los
> requerimientos, las reglas, los casos de uso, las pruebas, la arquitectura, las cifras y todas
> las figuras. Dentro de esas páginas que sí se pegan siguen esperando los datos marcados
> «⏸ ESPERA A JC». **Si ya pegaste algo de la zona en pausa, dejalo como está:** cuando se libere,
> «Cambió después del 16/11» va a decir qué volver a pegar.

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
> **⏸ En pausa (17/11): no vuelvas a pegar nada de esta tabla por ahora.** Todas sus filas son de
> la zona que se mueve con el código. Cuando se libere, esta tabla se actualiza y dice qué volver a
> pegar.
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

> ## ⏸ NO PEGAR TODAVÍA: la lista completa (17/11)
>
> Dos motivos. **La zona en pausa** depende del código, que Codex está ampliando en cuatro fases
> (mapa de sectores; identidad visual; tarjetas de evento y tres pantallas nuevas; seguridad e
> imágenes de eventos): lo que se pegue ahí hoy queda viejo. **Los datos de JC** faltan: si se
> pegan antes, el documento dice algo falso. Cada página en pausa tiene la marca también en su
> lugar, para que te frene aunque llegues sin leer esto.
>
> **Zona en pausa (depende del código):**
>
> | Páginas | Qué contiene | Por qué se mueve |
> |---|---|---|
> | p. 1 | La línea del commit en la carátula | El código que se entrega va a ser otro |
> | pp. 6 a 8 | Historial de revisión, resumen ejecutivo y limitaciones declaradas | Resumen las cifras y las funciones del sistema |
> | pp. 18, 20 y 21 | Criterios de aceptación y supuestos | Siguen a los requerimientos y a las integraciones simuladas (llega el envío de correo simulado) |
> | pp. 23 y 24 | Requerimientos funcionales y no funcionales | Funciones nuevas: verificación de correo, recuperación de contraseña, auditoría, imágenes de eventos, mapa de sectores |
> | pp. 25 a 33 | Reglas de negocio, factibilidad económica, niveles, parámetros y conflictos | Reglas nuevas desde RN-43 y parámetros nuevos |
> | pp. 35 a 38 | Priorización y orden de implementación (**salvo** la sección nueva «Estimación de costos», que sí se pega) | Requerimientos nuevos |
> | pp. 40 a 43 | Descripción funcional, alcance y remisión a los requerimientos | Funciones nuevas |
> | p. 45 | Prototipos | Son figuras de la interfaz |
> | pp. 47 a 55 | Guion de interfaz, instructivo y sus figuras | Pantallas nuevas (de 20 a unas 24) e identidad visual nueva |
> | pp. 57 a 70 | Arquitectura, entidades, código y pruebas | Clases, tablas, patrones (Strategy del envío de correo) y pruebas nuevas |
> | pp. 71 a 89 | Casos de uso, especificaciones y todos los diagramas | Casos de uso nuevos; los diagramas se regeneran |
> | pp. 90 a 97 | Usabilidad: el párrafo sobre el rediseño | La interfaz vuelve a cambiar |
> | pp. 98 a 105 | Anexo A02 | Depende de las pantallas y de las consultas |
> | pp. 106 a 109 | Matriz de trazabilidad, cobertura y brechas | Requerimientos, reglas y casos nuevos |
> | Sección nueva «Metodología de verificación» | Batería y cifras | Casos y ejecuciones nuevos |
> | pp. 110 y 111 | Conclusiones, limitaciones y trabajo futuro | Por ejemplo, «no existe recuperación de contraseña» deja de ser cierto |
> | Arriba de todo | Los scripts de la base | Llegan al menos dos scripts nuevos |
>
> **Datos de JC:**
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

## Cifras del documento (⏸ van a cambiar)

Son las del código `dba674c`. Con las fases nuevas cambian casos, ejecuciones, reglas y quizás la
cobertura. No las uses para corregir nada hasta que esta tabla vuelva a decir «definitivas».


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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**⏸ ESPERA A JC** (comisión, turno, legajos, año y si es «Maximo» o «Máximo»). Cuando lleguen:
**Página 1.** Reemplazar:

> Comisión: «completar» · Turno: «completar»
> Legajos: «completar» – «completar»

y el año debajo de «Rosario, Argentina»: «2025».

**Pie de las 112 páginas.** Reemplazar en:

> Materia: Trabajo de Campo I · Docente: Audoglio · Comisión: «completar» · Turno: «completar» ·
> Localización: Rosario · Año: 2025

**Encabezado de las páginas** (hoy dice «Alumnos: Ambrosino, Máximo (Legajo «completar») ·
Riggio, Juan Cruz (Legajo «completar»)»): completar los dos legajos.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**B-01 · p. 9 · R.** Dice: «aplicando tecnologías como Windows Forms (interfaz) y ASP.NET Core
(backend).» Poner: «aplicando Windows Forms para la interfaz, capas de negocio y de datos en C#
sobre .NET Framework 4.8 y SQL Server como base de datos.» *Razón:* no hay ASP.NET en la
solución.

**Objetivo específico 1, «Cómo medirlo». Reemplazar:**

> validado por al menos dos revisiones de parte del “cliente” ficticio (documentadas en
> minutas).

**Por:**

> validado por al menos dos revisiones de parte del cliente ficticio, documentadas en minutas
> simuladas.

**⏸ ESPERA A JC:** «entre junio y diciembre de 2025» depende del año que decida JC.

## Página 10

**B-16 · p. 10, Objetivo específico 2 · R.**
- **Dice:** «Construir un prototipo (en Figma, Adobe XD, o similar) que permita».
- **Poner:** «Construir un prototipo funcional de escritorio que permita».
- *Por qué sin «Windows Forms»:* el mismo documento dice que los objetivos no citan tecnologías
  puntuales (p. 9), y la guía del docente lo pide.

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

**B-17 · p. 16, Fase 6 · R.**
- **Dice:** «Simulación del formulario de pago con tokenización (sin backend).»
- **Poner:** «Simulación del formulario de pago con tokenización (sin pasarela real).»

## Página 18

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**B-08 · p. 18, criterio 1 · R.** Dice: «aplicación de descuentos por suscripción o rankeo».
Poner: «aplicación del descuento por nivel de fidelidad y de los beneficios de la suscripción».
*Razón:* RN-37.
**B-18 · p. 18, criterio 3 «Compatibilidad técnica» · R.** Reemplazar los tres puntos **por:**

> ● El prototipo es una aplicación de escritorio para Windows 10 u 11 y no requiere otro
>   software que el motor de base de datos.
> ● Las pantallas entran completas en el área útil de una notebook y de un monitor de
>   escritorio: ninguna supera los 790 píxeles de alto.

*Razón:* es el mismo cambio que el ítem 4b de CORRECCIONES aplica a RNF-03; el criterio repetía
lo de los navegadores.

## Página 20 — Criterios de aceptación

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**B-07 · p. 20, criterio 9 · R.** Reemplazar la línea completa por: «9. Recomendaciones
personalizadas: excluido del alcance (RF-09 pasó a Won't have en la versión 2.5).»

**Criterio 10. Reemplazar la línea completa por:**

> 10. Lista de espera para eventos agotados: El sistema debe registrar al usuario en la lista,
> permitirle consultar su posición y avisarle al iniciar sesión cuando el evento vuelve a tener
> entradas.

## Página 21 — Supuestos del proyecto

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**B-14 · p. 21, Supuesto 2 · R.**
- **Dice:** «El sistema será evaluado en base a un prototipo navegable y documentación técnica, no
  a una implementación funcional con backend real.»
- **Poner:** «El sistema se evalúa sobre un prototipo funcional, una aplicación de escritorio con
  base de datos propia, y su documentación técnica. Las integraciones externas (pasarela de pago y
  proveedores de servicios) son simuladas.»

**Supuesto 4. Reemplazar:**

> 4. Los usuarios de prueba estarán disponibles para realizar simulaciones de compra y
> responder encuestas de usabilidad.

**Por:**

> 4. La evaluación de usabilidad se realiza como estudio simulado: los participantes son
> perfiles definidos y sus resultados se derivan de mediciones sobre el prototipo, sin sesiones
> con personas reales.

**B-15 · p. 21, Supuesto 7 · R.**
- **Dice:** «(Figma, Draw.io, herramientas UAI)».
- **Poner:** «(Visual Studio, SQL Server Express, PlantUML y herramientas UAI)».

## Página 23 — Requerimientos funcionales

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**RF-10, criterio de aceptación. Reemplazar:**

> Criterio de aceptación: El sistema debe registrar al usuario en la lista e informarle que será
> notificado.

**Por:**

> Criterio de aceptación: El sistema debe registrar al usuario en la lista y confirmarle la
> inscripción. El cliente puede consultar su posición, que respeta el orden de RN-35. Si el
> evento vuelve a tener entradas, el sistema se lo informa al iniciar sesión; el aviso no reserva
> el lugar.

**B-02 · p. 23 · RF-11 · R.** Dice: «El sistema debe mostrar tablas o gráficos con datos ficticios
exportables.» Poner: «El sistema debe mostrar tablas con datos ficticios, exportables a PDF.»
*Razón:* `frmReportes` tiene grilla y «Exportar PDF», sin gráficos.
**B-03 · p. 23 · RF-12 · R.** Dice: «Permitir al Administrador crear, modificar y desactivar
eventos, definiendo sus sectores, capacidades y precios.» Poner: «Permitir al Administrador
crear, suspender y eliminar eventos, agregar y quitar sus sectores con capacidad y precio, y
fijar su período de preventa.» *Razón:* `GestorEventos` (completa T-7).

## Página 24 — Requerimientos no funcionales, punto 3 (RNF-03)

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**A-01 · p. 25 · R.** Dice: «los once requerimientos funcionales (RF-01 a RF-11)». Poner: «los
trece requerimientos funcionales (RF-01 a RF-13)». *Razón:* el propio documento agrega RF-12 y
RF-13 en la p. 23, y las reglas RN-06, RN-07 y RN-11 los citan como origen.

**Misma oración, a continuación:** «las especificaciones de casos de uso (CU-01 a CU-08)» pasa a
«las especificaciones de casos de uso (CU-01 a CU-13)».

**Tabla «Usuarios y cuentas» (en la p. 25 o en la 26: es la que tiene RN-01 a RN-05).
Agregar** después de RN-05:

| RN-40 | Cambiar la contraseña exige la contraseña actual. Una contraseña actual incorrecta cuenta como intento fallido a los efectos de RN-04, y con la cuenta bloqueada el cambio se rechaza. | A | RF-02 / CU-09 |
|---|---|---|---|
| RN-41 | Un cliente solo consulta y opera sobre sus propias compras, entradas, pagos, pólizas, suscripción e inscripciones en listas de espera. El Administrador puede hacerlo sobre las de cualquier cliente. | A | RF-02 / CU-05 |
| RN-42 | La gestión de eventos, sectores y locales, la consulta de las listas de espera y el control de ingreso son exclusivos del perfil Administrador. Los reportes se rigen por RN-34. | A | RF-12 / CU-02 |

## Página 26 — Reglas de negocio

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**A-02 · p. 26 · RN-11 · R.** Dice: «Un evento con entradas vendidas no puede eliminarse: sólo
puede suspenderse o reprogramarse, conservando la trazabilidad de lo vendido.» Poner: «Un evento
con entradas vendidas no puede eliminarse: sólo puede suspenderse, conservando la trazabilidad
de lo vendido.» *Razón:* código. `GestorEventos` tiene `Eliminar` y `Suspender`, y no existe
ninguna reprogramación.
**A-03 · p. 26 · RN-18 · R** (reemplaza el texto del ítem 25 de CORRECCIONES). Poner: «Cada
entrada vendida genera un código QR único e irrepetible, válido para un único ingreso. El
control de ingreso acepta la primera lectura y rechaza las siguientes. Rechaza también la
entrada que no está confirmada, la que corresponde a otro evento y la que se presenta fuera del
horario de ingreso, que va desde seis horas antes hasta doce horas después del inicio del
evento.» *Razón:* código. `GestorIngresos.Registrar` usa `INGRESO_HORAS_ANTES` = 6 e
`INGRESO_HORAS_DESPUES` = 12.
**A-04 · p. 26 · RN-23 · R.** Dice: «El nivel se reevalúa el primer día de cada mes.» Poner: «El
nivel se reevalúa una vez por mes calendario, la primera vez que el cliente arma o confirma una
compra en ese mes.» El resto de la regla queda igual. *Razón:* código. No hay ningún proceso del
día 1: `ReevaluarNivel` se ejecuta al abrir la compra (`FrmCompraCliente.cs:173`) y al
confirmarla (`GestorCompras.Confirmar`). Para el cliente, el efecto es el mismo.
**A-05 · p. 26 · RN-24 · R.** Dice: «Sólo computan para el nivel las compras confirmadas y no
reembolsadas.» Poner: «Sólo computan para el nivel las compras confirmadas. Una compra cuyas
entradas se devolvieron todas deja de computar; una con devolución parcial sigue computando,
porque la operación existió.» *Razón:* código. En `GestorCompras.Reembolsar` la devolución
parcial deja la compra en estado Confirmada.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**A-06 · p. 27 · RN-26 · R.** Dice: «La suscripción no se renueva de forma automática: vencido el
período, el cliente debe contratarla nuevamente.» Poner lo mismo, más: «El cliente puede
cancelarla antes del vencimiento: pierde los beneficios desde ese momento y no se le reintegra
el precio.» *Razón:* código. `GestorSuscripciones.Cancelar` existe, con el aviso «Los beneficios
se pierden desde ahora», y no hay ningún reintegro.

**RN-29. Reemplazar** la fila:

| RN-29 | El seguro habilita la devolución del valor de la entrada hasta cuarenta y ocho horas antes del inicio del evento. | A | RF-05 |
|---|---|---|---|

**Por:**

| RN-29 | El seguro habilita la devolución hasta cuarenta y ocho horas antes del inicio del evento. La devolución es por entrada: el cliente elige cuáles da de baja, y cada una debe estar confirmada, asegurada y sin ingreso registrado. Se reintegra el precio congelado de cada entrada menos su parte proporcional del descuento por nivel y, si con ello no queda ninguna entrada confirmada en la compra, también los servicios complementarios. No se reintegran el cargo por servicio ni la prima del seguro. El costo de los servicios devueltos se recupera del proveedor, condición que debe constar en el acuerdo con cada uno. | A | RF-05 / CU-13 |
|---|---|---|---|

**A-08 · p. 27 · RN-31 · R.** Dice: «Toda operación de pago se registra con su estado resultante
(aprobado, rechazado o pendiente), sea cual fuere el desenlace.» Poner: «Toda operación de pago
se registra con su estado resultante, aprobado o rechazado, sea cual fuere el desenlace.»
*Razón:* base. `CK_Pagos_Estado` solo admite 'Aprobado' y 'Rechazado'. «Pendiente» es un estado
de la compra, no del pago. **Verificado el 13/11** contra `GestorCompras.Confirmar`: no hay
ningún pago intermedio (sección 4). Se aplica.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

| Dice | Poner |
|---|---|
| «$19.356» | «$19.355» |

*Razón:* recalculado; todas las contribuciones, el 8,62 % y el 9,5 % quedan iguales.

## Página 29 — Costo del programa

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Reemplazar:**

> El costo anual de los descuentos, calculado sobre el juego de datos de prueba, asciende a
> $3.758.161.

**Por:**

> El costo anual estimado de los descuentos, calculado sobre el juego de datos simulado,
> asciende a $3.758.161.

## Página 32 — Niveles de fidelidad

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**A-09 · p. 32 · tabla de niveles, columna «Beneficios adicionales» · R.**

| Fila | Dice | Poner |
|---|---|---|
| Bronce | «Acceso estándar a la preventa general» | «—» |
| Plata | «Preventa anticipada de 24 horas» | «—» |
| Oro | «Preventa anticipada de 48 h y prioridad en lista de espera» | «—» |

En el mismo párrafo, cambiar «Los beneficios diferenciales del nivel Oro se orientan al acceso
anticipado —de costo marginal nulo para la plataforma— antes que a una rebaja mayor de precio.»
por «El nivel no otorga otros beneficios que el descuento: la preventa y la prioridad en la
lista de espera son beneficios de la suscripción (RN-37, RN-39 y RN-35).»

*Razón:* código. La preventa solo mira la suscripción (`GestorPaquetes.VerificarPreventa`), y la
lista solo ordena por `ConSuscripcion`. Además, la tabla contradecía la RN-37 del propio
documento.

## Página 33 — Parámetros y conflictos

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**A-10 · p. 33 · Parámetros configurables · R.**
- **Borrar la fila** «Periodicidad de reevaluación de nivel | RN-23 | Mensual». No es un
  parámetro: no existe en la tabla `Parametros`.
- **Agregar** estas filas:

| Parámetro | Regla | Valor inicial |
|---|---|---|
| Cargo por servicio | — | 15 % |
| Precio del seguro por entrada | RN-28 | $5.000 |
| Bonificación del seguro para suscriptores | RN-37 | 100 % |
| Precio del plan mensual | RF-13 | $9.000 |
| Precio del plan anual | RF-13 | $90.000 |
| Horas de ingreso antes del evento | RN-18 | 6 |
| Horas de ingreso después del evento | RN-18 | 12 |
| Minutos de prioridad de los suscriptores al liberarse un lugar | RN-35 | 15 |

*Razón:* base. Son los valores de `BaseDatos/02-semilla.sql`.
**A-11 · p. 33 · Conflictos · R**. Dice:
«tres pares de reglas». Poner: «cuatro pares de reglas». Y agregar la fila:

| RN-35 vs. RN-37 | RN-37 promete prioridad al suscriptor en la lista de espera. | Primero los suscriptores y después el resto, cada grupo por orden de inscripción. La condición de suscriptor se congela al inscribirse. Cuando se libera un lugar, durante quince minutos solo pueden comprarlo los suscriptores inscriptos. |
|---|---|---|

## Página 35 — Priorización

**⏸ NO PEGAR TODAVÍA esta página, salvo la sección nueva «Estimación de costos», que sí se pega hoy.** El resto depende de los requerimientos, que van a cambiar.

**B-04 · p. 35 · R+N.** Dice: «Los once requerimientos funcionales y los cinco no funcionales».
Poner: «Los trece requerimientos funcionales y los cinco no funcionales».
- Dice: «nueve de los once requerimientos funcionales quedaran en esa categoría, concentrando
  alrededor del 85 % del esfuerzo». Poner: «diez de los trece requerimientos funcionales
  quedaran en esa categoría, concentrando alrededor del 80 % del esfuerzo estimado».
- *Cuenta:* 49 + 5 + 8 = 62 de 78 puntos, que da 79,5 %.

**Sección nueva, debajo de «Escala de esfuerzo»** (después del párrafo que termina «…no por
estimación absoluta.»). Pegar completo:

> **Estimación de costos**
>
> **Supuesto: valor hora V = $30.500.** Fuente: Colegio de Ingenieros Especialistas de la
> Provincia de Santa Fe, Resolución N.º 123 (aprobada el 24/09/2026, vigente desde el
> 01/11/2026), Anexo I, «Tareas de sistemas de información — Desarrollo», rol 1.2 «Analista
> Programador», nivel Junior. Se adopta el nivel junior porque el equipo está formado por dos
> estudiantes. Todos los importes de esta sección resultan de multiplicar las horas por V: si V
> cambia, basta con repetir esa multiplicación.
>
> Las horas son horas-persona de trabajo de ingeniería y surgen de la escala de esfuerzo (un punto
> equivale a cuatro horas). Cubren el desarrollo que esa escala estima; no se agregan horas de
> relevamiento ni de documentación, que el proyecto no estimó por separado.
>
> | Fase | Requerimientos | Puntos | Horas | Costo (horas × V) |
> |---|---|---|---|---|
> | Fase 4 — Desarrollo del prototipo, 1ª iteración | RF-12, RF-01 a RF-05, RF-07 | 41 | 164 | $5.002.000 |
> | Fase 6 — Desarrollo ampliado, 2ª iteración | RF-06, RF-08, RF-13 | 18 | 72 | $2.196.000 |
> | Fase 7 — Generación de reportes | RF-11 | 8 | 32 | $976.000 |
> | Fase 8 — Ajustes finales | RF-10 | 3 | 12 | $366.000 |
> | **Alcance construido** | | **70** | **280** | **$8.540.000** |
> | RF-09, estimado y no construido (Won't have desde la versión 2.5) | RF-09 | 8 | 32 | $976.000 |
> | **Estimación aprobada** | | **78** | **312** | **$9.516.000** |
>
> **Otros costos.** No hay costo de licencias: las herramientas empleadas (Visual Studio, SQL
> Server Express y PlantUML) se usan en versiones gratuitas o provistas por la universidad. Los
> equipos son de los integrantes y no se imputan.
>
> **Sensibilidad al valor hora.**
>
> | Escenario | Valor hora | Alcance construido (280 h) | Estimación aprobada (312 h) |
> |---|---|---|---|
> | V − 25 % | $22.875 | $6.405.000 | $7.137.000 |
> | Supuesto | $30.500 | $8.540.000 | $9.516.000 |
> | V + 25 % | $38.125 | $10.675.000 | $11.895.000 |
>
> Una variación de ±25 % en el valor hora mueve el costo de la estimación aprobada entre
> $7.137.000 y $11.895.000. Como el costo es proporcional al valor hora, el resultado no depende
> de ningún otro supuesto.

## Página 36 — Priorización de requerimientos funcionales

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Tabla «Priorización de requerimientos no funcionales», fila RNF-03. Reemplazar** la
justificación:

> Deseable, pero la evaluación del prototipo se realiza en escritorio.

**Por:**

> Deseable, pero la evaluación del prototipo se realiza en escritorio. Se cumple en tamaño de
> pantalla (casos CP-12 y CP-25); la nitidez con escalado de Windows superior al 100 % queda
> como limitación conocida (ver «Requerimientos no funcionales», punto 3).

**B-05 · p. 37 · W-06 · R.** En la columna Motivo, dice: «Requiere hardware específico. El código
QR se genera pero no se valida.» Poner: «DADA DE BAJA en la versión 2.5: se implementó como
control de ingreso (RN-18), con lector USB o con el texto del ticket. El número no se reasigna.»
*Razón:* `frmControlIngreso` y `GestorIngresos` existen.

**Tabla «Requerimientos excluidos — categoría Won't have». Agregar** la fila:

| W-07 | Recomendaciones personalizadas (RF-09) | Reclasificado desde Could en la versión 2.5. No responde a ningún objetivo específico ni tiene caso de uso, y es el requerimiento no obligatorio más costoso (8 puntos). |
|---|---|---|

## Página 38 — Orden de implementación derivado

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**B-09 · p. 40, «Descripción funcional» · R.** Hoy afirma cosas que el sistema no tiene:
- sitio web;
- e-ticket enviado por correo;
- reclamo de seguro por formulario;
- nivel según el monto comprado;
- motor de recomendaciones;
- aviso automático por correo;
- tickets de soporte;
- descuento por suscripción.

**Reemplazar los tres párrafos completos**, desde «El usuario ingresa a la página principal de
EventAura» hasta «nivel alcanzado y beneficios vigentes.», **por:**

> El cliente inicia sesión en la aplicación de escritorio de EventAura y accede al listado de
> eventos vigentes. Puede filtrarlo por texto (nombre del evento, del local o su dirección), por
> categoría (concierto, deportivo, teatro o cultural) y por fecha. Al elegir un evento se abre la
> pantalla de armado del paquete. Allí indica la cantidad de entradas de cada sector, con su
> precio y su cupo restante, y marca los servicios complementarios que ese evento ofrece:
> hospedaje, traslado, merchandising y experiencia VIP. También puede contratar el seguro de
> entrada. El resumen se recalcula con cada cambio y muestra la base de entradas, el descuento
> por nivel, los servicios, el cargo por servicio, el seguro y el total.
>
> Al confirmar el paquete, el sistema reserva el cupo por diez minutos y abre el pago simulado. El
> cliente ingresa los datos de una tarjeta ficticia, que se tokeniza, y la pasarela simulada
> aprueba o rechaza la operación; del pago se conservan solo los cuatro últimos dígitos y el
> token. Si el pago se aprueba, la compra queda confirmada y el sistema genera un ticket en PDF
> por entrada. Cada ticket lleva un código QR único, que el control de ingreso acepta una sola
> vez. Con el seguro contratado, el cliente puede devolver entradas desde su historial hasta
> cuarenta y ocho horas antes del evento.
>
> El nivel de fidelidad (Bronce, Plata u Oro) se determina por la cantidad de compras confirmadas
> en los últimos doce meses, y fija el descuento sobre las entradas. La suscripción, mensual o
> anual, no otorga descuento: bonifica el seguro, habilita la preventa y da prioridad en la lista
> de espera. Si un evento está agotado, el cliente puede inscribirse en su lista de espera y consultar su
> posición; si vuelve a haber entradas, el sistema se lo avisa al iniciar sesión. El
> Administrador registra locales, crea eventos con sus sectores y su preventa, consulta las
> listas de espera, controla el ingreso y genera cinco reportes de gestión exportables a PDF.

*Razón:* código. Cada afirmación sale de un gestor: `GestorEventos.Buscar`, `GestorPaquetes`,
`GestorFidelizacion.CalcularDescuento`, `GestorCompras.Iniciar`, `Confirmar` y `Reembolsar`,
`GestorIngresos`, `GestorSuscripciones`, `GestorListaEspera` y `GestorReportes`.

## Página 41 — Alcance y restricciones

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**B-10 · p. 41, «Alcance», los dos párrafos · R** (unifica B-10 y B-11; redactado el 13/11 con la
guía del docente, punto 5). La guía pide el alcance «sin entrar en temas técnicos ni
tecnológicos» y con inclusiones y exclusiones explícitas. Reemplazar los dos párrafos completos,
desde «EventAura en su primera versión abarca» hasta «el foco está en el usuario final y el
administrador de eventos.», **por:**

> EventAura, en su primera versión, abarca la gestión de eventos y la venta de entradas de una
> ticketera local.
>
> **Inclusiones:**
> - el registro de clientes y el inicio de sesión;
> - el alta de locales y de eventos, con sus sectores, precios y período de preventa;
> - la búsqueda de eventos y el armado de paquetes personalizados: entradas y servicios
>   complementarios (hospedaje, traslado, merchandising y experiencia VIP);
> - la compra con pago simulado y la emisión de una entrada por persona, con un código único;
> - el control de ingreso al evento, que acepta cada entrada una sola vez;
> - el programa de fidelización por niveles, con su descuento sobre las entradas;
> - la suscripción mensual o anual, con sus beneficios;
> - el seguro de entrada y la devolución de las entradas aseguradas;
> - la lista de espera para los eventos agotados, con la consulta de la posición y el aviso al
>   iniciar sesión cuando vuelve a haber entradas;
> - cinco reportes de gestión para la administración.
>
> **Exclusiones** (quedan planteadas para iteraciones futuras):
> - las recomendaciones personalizadas de eventos (RF-09);
> - los avisos por correo electrónico o notificación push (W-03); el aviso de la lista de espera
>   se muestra dentro de la aplicación;
> - la reprogramación de eventos: un evento con entradas vendidas se suspende, pero no cambia de
>   fecha;
> - el cobro real a través de una pasarela de pago, y el cobro de la suscripción;
> - la aplicación móvil nativa y el acceso desde un navegador web;
> - la logística interna de cada evento (contratación de artistas, catering o disposición del
>   recinto);
> - las tareas contables de la empresa (facturación, balances y pagos a proveedores);
> - la integración con sistemas ERP o CRM externos;
> - la gestión de empleados y de recursos internos, como el inventario de merchandising.

*Razón:* código. Cada inclusión sale de un gestor (ver B-09); cada exclusión, de lo que no
existe. Las cuatro últimas son las del párrafo original, que siguen siendo ciertas.
**B-12 · p. 41, Restricción 2 · R.**
- **Dice:** «Alcance centrado en prototipado, no en desarrollo funcional. El producto final es un
  prototipo navegable, no un sistema programado con backend funcional. Esto restringe la
  validación de ciertos aspectos técnicos como seguridad real, velocidad de respuesta o
  integraciones en tiempo real.»
- **Poner:** «Alcance de prototipo funcional. El producto final es un prototipo funcional de
  escritorio, con base de datos propia, y no un sistema en producción. Esto restringe la
  validación de aspectos como el rendimiento con carga real, la seguridad de un despliegue en red
  y las integraciones en tiempo real.»
**B-13 · p. 41, Restricción 3 · R.**
- **Dice:** «(Figma, Draw.io, Google Drive), lo que puede restringir la calidad gráfica o la
  capacidad de simulación avanzada.»
- **Poner:** «(Visual Studio, SQL Server Express y PlantUML), lo que puede restringir la capacidad
  de simulación avanzada.»

## Páginas 42 y 43 — Segunda copia de los requerimientos (se reemplaza por una remisión)

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**E-01 · p. 48 · R (T-5).**
- **Dice:** «aunque el mismo formato se emplea para agregar locales, eventos, vehículos y otras
  entidades.»
- **Poner:** «aunque el mismo formato se emplea para agregar locales, eventos y sectores.»

## Página 49

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**E-02 · p. 49 · R (T-6).**
- **Dice:** «(por ejemplo, Cliente, Administrador de local, Conductor)».
- **Poner:** «(Cliente o Administrador)».

## Página 50 — Instructivo: estado de implementación

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Reemplazar** el párrafo completo:

> **Estado de implementación.** A la fecha de esta versión se encuentran construidas la
> búsqueda y el listado de eventos, la selección de sector y cantidad, el pago simulado y la
> emisión del comprobante con código QR. La incorporación de servicios complementarios, la
> contratación del seguro y el desglose del descuento por nivel están especificados en este
> instructivo pero aún no implementados; corresponden a la segunda iteración prevista en la
> Fase 6 del cronograma. El instructivo describe, en consecuencia, el comportamiento
> definido para el sistema, y no únicamente lo ya construido.

**Por:**

> **Estado de implementación.** A la fecha de esta versión el recorrido completo está
> construido: búsqueda y listado de eventos, selección de sector y cantidad, servicios
> complementarios, seguro de entrada, desglose del descuento por nivel, reserva con
> vencimiento, pago simulado y emisión de un comprobante con código QR por entrada. Las
> figuras de esta sección son capturas del prototipo, y el ejemplo resuelto coincide con
> los datos de prueba del sistema.

## Página 51

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**E-03 · p. 51, «Composición de un paquete» · R.** Tres filas:

| Componente | Columna «Cantidad admitida»: dice | Poner |
|---|---|---|
| Merchandising | «De 0 a 3 artículos» | «0 o 1» |
| Experiencia VIP | «0 o 1; exige entrada de sector VIP» | «0 o 1» |
| Seguro de entrada | «1 por entrada adquirida» | «Se contrata para todas las entradas de la compra: una póliza por entrada» |

*Razón:* código. `GestorPaquetes.AgregarServicio` admite cada servicio una vez, sin cantidad y
sin condición de sector; `Paquete.ConSeguro` vale para la compra entera.
**E-04 · p. 51, tabla de controles de la pantalla de armado · R.** Reemplazar la tabla
completa por:

| Control | Tipo | Contenido y comportamiento |
|---|---|---|
| label1 | Etiqueta | Nombre del evento. Sólo lectura. |
| lblNivel | Etiqueta | Nivel de fidelidad vigente y porcentaje de descuento, por ejemplo «Tu nivel: Plata · 5,0 % de descuento». |
| lnkNiveles | Enlace | «¿Cómo funcionan los niveles?»: explica los umbrales vigentes (RN-19). |
| dgvSectores | Grilla | Una fila por sector, con su nombre, su precio y las entradas disponibles. El precio queda congelado al iniciar la compra (RN-16). En la columna «Cantidad (escribí aquí)» el cliente escribe cuántas entradas quiere. La suma no puede superar 6 (RN-12) ni el cupo del sector (RN-08). |
| chkHospedaje / chkTraslado / chkMerch / chkVIP | Casilla | Servicios complementarios. Aparecen deshabilitados si el evento no los ofrece (RN-14). |
| chkSeguro | Casilla | Seguro de entrada, con devolución hasta 48 h antes. Se habilita solo cuando hay al menos una entrada (RN-28). |
| pnlResumen | Panel | Desglose del paquete. Se recalcula automáticamente ante cada cambio. |
| lblAgotado / btnListaEspera | Etiqueta y botón | Aparecen solo con el evento agotado, y ofrecen la inscripción en la lista de espera (RN-10). |
| btnComprar | Botón | Reserva el cupo y abre el pago. Deshabilitado mientras no haya al menos una entrada (RN-13). Sin seguro, pide confirmar que se continúa sin él. |
| btnVolver | Botón | «Volver al listado». Si hay un paquete armado, pide confirmación antes de descartarlo; no reserva cupo. |

## Página 52

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**E-05 · p. 52, «Recorrido paso a paso», pasos 6 y 7 · R.**

| Paso | Columna | Dice | Poner |
|---|---|---|---|
| 6 | Acción del cliente | «Revisa el resumen del paquete.» | «Revisa el resumen del paquete y presiona «Comprar».» |
| 6 | Respuesta del sistema | «Muestra base de entradas, descuento por nivel, servicios sin descuento, cargo por servicio y total.» | «Muestra base de entradas, descuento por nivel, servicios sin descuento, cargo por servicio, seguro y total. Al confirmar, reserva el cupo por diez minutos y abre el pago.» |
| 6 | Reglas | «RN-20, RN-21, RN-37» | «RN-15, RN-20, RN-21, RN-37» |
| 7 | Respuesta del sistema | «Reserva el cupo por diez minutos, tokeniza los datos y envía la operación a la pasarela simulada.» | «Tokeniza los datos y envía la operación a la pasarela simulada, dentro del plazo de la reserva.» |

*Razón:* código. `GestorCompras.Iniciar` reserva al presionar «Comprar»; la pantalla de pago
llega después y muestra la cuenta regresiva.

## Páginas 52 y 53 — Figuras 1 a 4 del instructivo

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

Sin cambios.

## Página 55 — Situaciones particulares y mensajes de validación

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

Reemplaza las páginas 57 a 61 completas, desde el título «Implementación» hasta el final de
`class GestorReportes`. El título queda: **«Arquitectura e implementación»**.

> El sistema es una aplicación de escritorio en C# sobre .NET Framework 4.8, con SQL Server como
> base de datos. Se organiza en cuatro capas, con dependencias en un solo sentido:
>
> | Capa | Proyecto | Responsabilidad |
> |---|---|---|
> | Presentación | Visual (Windows Forms) | Veinte pantallas. No abre conexiones ni contiene SQL: solo invoca a la capa de negocio. |
> | Negocio | Repositorio (namespace Negocio) | Gestores que aplican las reglas de negocio. No referencia la interfaz: informa los rechazos con excepciones de dominio, que identifican la regla (RN-xx). |
> | Datos | Datos | Repositorios: único lugar con conexiones y SQL, siempre con consultas parametrizadas. |
> | Entidades | Entidades | Clases de dominio sin lógica, compartidas por las tres capas. |
>
> **Patrones de diseño aplicados.**
> - **Singleton**, en veintiocho clases: doce gestores de negocio, trece repositorios, la
>   conexión, la pasarela simulada y el generador de códigos QR. Cada una expone una única
>   instancia (`Instance`) con constructor privado.
> - **Repository:** las dieciséis tablas se acceden a través de trece repositorios, el único
>   lugar con SQL; cada uno hereda de `RepositorioBase` la apertura de la conexión y la creación
>   de comandos parametrizados.
> - **Strategy**, en la exportación de reportes: el negocio conoce solo la interfaz
>   `IExportador`, y la pantalla de reportes elige la implementación (`ExportadorPDF`, con iText).
> - **Herencia en los reportes:** una clase abstracta `Reporte` con cinco especializaciones, una
>   por reporte, creadas por `GestorReportes`.
>
> **Entidades y tablas.** El modelo persistido tiene dieciséis tablas:
> - Usuario, con sus perfiles Cliente y Propietario (el Administrador);
> - Locales, Eventos y Sectores;
> - Compras, Entradas y Pagos;
> - Servicios, ServiciosEvento y CompraServicios;
> - Seguros, Suscripciones y ListaEspera;
> - Parametros, que guarda los valores configurables de las reglas.
>
> El paquete no se persiste como entidad propia: se arma en memoria y, al confirmarse, se
> registra como una compra con sus entradas y sus servicios. Los diagramas de clases y el
> diagrama entidad-relación de este documento muestran cada entidad con sus atributos y
> relaciones.

## Página 62 — Entidades

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**E-08 · p. 62, tabla de entidades, fila «Entradas» · R.**

| Columna | Dice | Poner |
|---|---|---|
| Definición | «Se agrega ID_Compra como clave foránea hacia Compras.» | «Se agregan ID_Compra (clave foránea hacia Compras), ID_Sector, Estado, CodigoQR y FechaIngreso. Cada fila pasa a ser una entrada individual.» |
| Justificación | «Vincula cada entrada con la operación que la originó.» | «Vincula cada entrada con la operación que la originó, y permite el cupo por sector (RN-08), el QR único (RN-18) y la devolución por entrada (RN-29).» |

## Páginas 63 a 67 — Código real (⚠ releé el aviso de arriba antes de empezar)

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**p. 63, «Definición de las estructuras».** Reemplazar el bloque `CREATE TABLE Compras` y las
cuatro líneas `ALTER TABLE` que lo siguen **por**:

```sql
CREATE TABLE Compras (
    ID                INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario        INT           NOT NULL,
    FechaCompra       DATETIME      NOT NULL,
    Estado            VARCHAR(12)   NOT NULL,   -- Pendiente|Confirmada|Reembolsada|Vencida
    NivelAplicado     VARCHAR(8)    NOT NULL,   -- congelado, RN-22
    DescuentoAplicado DECIMAL(4,3)  NOT NULL,   -- congelado, RN-16
    BaseEntradas      DECIMAL(12,2) NOT NULL,
    MontoDescuento    DECIMAL(12,2) NOT NULL,
    MontoServicios    DECIMAL(12,2) NOT NULL,
    CargoServicio     DECIMAL(12,2) NOT NULL,
    MontoSeguro       DECIMAL(12,2) NOT NULL DEFAULT 0,   -- RN-28
    ConSeguro         BIT           NOT NULL DEFAULT 0,
    TotalPagado       DECIMAL(12,2) NOT NULL,
    MontoReembolsado  DECIMAL(12,2) NOT NULL DEFAULT 0,   -- RN-29
    ReservaVence      DATETIME      NULL,                 -- RN-15
    CONSTRAINT FK_Compras_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID),
    CONSTRAINT CK_Compras_Estado
        CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida'))
);
ALTER TABLE Entradas ADD ID_Compra INT NULL;
ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Compras
      FOREIGN KEY (ID_Compra) REFERENCES Compras(ID);
ALTER TABLE Usuario  ADD Nivel VARCHAR(8) NOT NULL DEFAULT 'Bronce';
ALTER TABLE Usuario  ADD FechaUltimaEvaluacion DATE NULL;
```

El índice `IX_Compras_Usuario_Fecha` que sigue no cambia.

**p. 64, «Algoritmo de cálculo del descuento».** En el pseudocódigo:
- reemplazar la línea `total ← subtotalE + servicios + cargo` **por** estas tres:

```
    seguro     ← cantidad de entradas × precio del seguro, si se contrata   // RN-28
    seguro     ← 0 si el cliente tiene suscripción activa                   // RN-37
    total      ← subtotalE + servicios + cargo + seguro
```

- en `DEVOLVER ResumenDescuento(…)`, agregar `seguro` antes de `total`.

**En el párrafo «Sobre la suscripción»**, reemplazar «seguro bonificado, preventa y prioridad—
se resuelven fuera de este cálculo y no alteran el importe de la entrada.» **por** «preventa y
prioridad— se resuelven fuera de este cálculo, y el seguro bonificado lleva su prima a cero sin
alterar el importe de la entrada.»

**p. 64, «Diseño de clases».**

| Dice | Poner |
|---|---|
| «con el gestor de negocio implementado como singleton, del mismo modo que GestorClientes.» | «con el gestor de negocio implementado como singleton, del mismo modo que los demás gestores que aplican reglas de negocio.» |
| `enum EstadoCompra   { Pendiente, Confirmada, Reembolsada }` | `enum EstadoCompra   { Pendiente, Confirmada, Reembolsada, Vencida }` |
| `decimal        BaseEntradas, MontoDescuento, TotalPagado;` | `decimal        BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, MontoSeguro, TotalPagado, MontoReembolsado;` |
| En `ResumenDescuento`: `decimal        SubtotalEntradas, Servicios, CargoServicio, Total;` | `decimal        SubtotalEntradas, Servicios, CargoServicio, Seguro, Total;` |
| En `GestorFidelizacion`, después de `ReevaluarNivel` | Agregar la línea `void             AscenderSiCorresponde(Cliente cliente, DateTime fecha) // RN-22` |

**p. 65, `PorcentajePara`.** Reemplazar el método completo. **Poner:**

```csharp
public decimal PorcentajePara(NivelFidelidad nivel)
{
    switch (nivel)                                   // RN-20, valores en Parametros
    {
        case NivelFidelidad.Oro:
            return DescuentoAdmisible(GestorParametros.Instance.ObtenerDecimal("DESCUENTO_ORO"));
        case NivelFidelidad.Plata:
            return DescuentoAdmisible(GestorParametros.Instance.ObtenerDecimal("DESCUENTO_PLATA"));
        default:
            return 0m;                               // Bronce no tiene descuento
    }
}
private static decimal DescuentoAdmisible(decimal porcentaje)
{
    if (porcentaje > GestorParametros.Instance.ObtenerDecimal("DESCUENTO_MAX_ADMISIBLE"))
        throw new ReglaNegocioException("RN-20", "El descuento configurado supera el máximo admisible.");
    return porcentaje;
}
```

**p. 65, `DeterminarNivel`.** Reemplazar la línea `DateTime desde = fechaReferencia.AddMonths(-12);
// RN-19` **por** estas dos:

```csharp
    int meses = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_VENTANA_MESES");   // RN-19
    DateTime desde = fechaReferencia.AddMonths(-meses);
```

Y las líneas `if (n >= 7) …` y `if (n >= 3) …` **por:**

```csharp
    if (n >= GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_ORO"))   return NivelFidelidad.Oro;
    if (n >= GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_PLATA")) return NivelFidelidad.Plata;
```

**p. 66, `CalcularDescuento`.**
- **Agregar** como primera línea del método, antes del comentario «// Base: solo entradas»:

```csharp
    GestorSesion.Instance.RequerirUsuario(cliente.Id);   // RN-41: solo el titular o el Administrador
```

- **Reemplazar** la línea del cargo **por:**

```csharp
    decimal cargo      = Math.Round(subtotalE * GestorParametros.Instance.ObtenerDecimal("CARGO_SERVICIO"), 2);
```

- **Agregar**, antes del `return`:

```csharp
    decimal seguro = paquete.ConSeguro
        ? GestorPaquetes.Instance.CantidadEntradas(paquete)
          * GestorParametros.Instance.ObtenerDecimal("SEGURO_PRECIO") : 0m;
    if (seguro > 0m && GestorSuscripciones.Instance.TieneActiva(cliente.Id))      // RN-37
        seguro = Math.Round(seguro * (1m - GestorParametros.Instance
                     .ObtenerDecimal("SUSCRIPCION_SEGURO_DESCUENTO")), 2);
```

- **Dentro del `return`,** agregar `Seguro = seguro,` y cambiar `Total = subtotalE + servicios +
  cargo` por `Total = subtotalE + servicios + cargo + seguro`.

**p. 67, diagrama de secuencia en texto.**

| Dice | Poner |
|---|---|
| `FrmCompraCliente ──► GestorCompras       : RegistrarCompra(resumen)` | `FrmCompraCliente ──► GestorCompras       : Iniciar(paquete, cliente)   // reserva 10 min` |
| `GestorCompras ──► PasarelaSimulada       : Procesar(total)` | `frmPago ──► GestorCompras                : Confirmar(compra, tarjeta)` y debajo `GestorCompras ──► PasarelaSimulada : Cobrar(tarjeta, total)` |
| `GestorFidelizacion ──► RepositorioFidelizacion : ActualizarNivel(…)` | Agregar antes: `GestorCompras ──► GestorFidelizacion : ReevaluarNivel(…) y AscenderSiCorresponde(…)` |

## Páginas 68 a 70 — Pruebas de la lógica de descuento

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**p. 68, primer párrafo. Reemplazar:**

> El objetivo específico 3 del proyecto exige registrar al menos cinco casos con distintos
> perfiles de usuario; el conjunto aquí presentado lo duplica.

**Por:**

> El objetivo específico 3 del proyecto exige verificar las reglas del programa con al menos
> cinco casos de prueba; el conjunto aquí presentado lo duplica.

*Razón:* el objetivo 3 cambió en la p. 10.

El listado de código **queda como está**. Debajo del título «Implementación de las pruebas»,
**agregar:**

las pruebas», agregar:

> El código vigente está en `Pruebas/PruebasFidelizacion.cs` y se ejecuta en cada corrida del
> conjunto de pruebas. El listado que sigue muestra su estructura. En el código, los métodos se
> llaman `CP01a04_NivelYDescuentoSegunCompras`, `CP05_ElDescuentoNoAlcanzaALosServicios`,
> `CP06_LaCompraDeHace13MesesQuedaFueraDeLaVentana`, `CP07_SeFacturaElPrecioCongelado`,
> `CP08_LaSeptimaSeCobraAl5YOroRigeDesdeLaOctava`, `CP09_ReembolsoBajaElContadorPeroNoElNivel` y
> `CP10_LaSuscripcionNoSumaPorcentaje`, y preparan los datos sobre una base de prueba que se
> recrea en cada ejecución.

## Página 71 — Diagramas de casos de uso (dos figuras)

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Reemplazar** la figura que está bajo «Diagramas de caso de uso» por:
- `docs/diagramas/casos-uso-cliente.png`, a **16,0 cm** de ancho;
- en la página siguiente, con el título «Diagramas de caso de uso (Administrador)»,
  `docs/diagramas/casos-uso-administrador.png`, a **16,0 cm**.

Siempre con la proporción bloqueada (clic derecho › *Tamaño y posición* › *Bloquear relación de
aspecto*). **Si la achicás, deja de leerse.**

**Texto, debajo del título:**

> Los casos de uso se presentan en dos diagramas, uno por actor, para que cada uno se lea a
> tamaño de página. La mayoría de los casos de uso corresponde a una pantalla del sistema y a una
> operación de la capa de negocio. Otros ocurren dentro de la compra, sin pantalla propia: el
> descuento por nivel, que la compra aplica siempre, y el seguro, que el cliente elige mientras
> arma el paquete. Se dibujan para que esas reglas del negocio queden visibles y trazables en el
> diagrama. El pago se resuelve con una pasarela simulada que forma parte del sistema,
> por lo que no se modela como actor. Las relaciones «include» indican pasos que la compra
> realiza siempre (el descuento por nivel y el pago); las «extend» indican comportamiento
> opcional, con su condición entre corchetes: contratar el seguro, inscribirse en la lista de
> espera cuando el evento está agotado y devolver una entrada asegurada con más de 48 horas de
> anticipación.
>
> El Administrador corresponde al perfil «Propietario» de la base de datos (RN-05). Además de los
> casos de uso de gestión, incorpora el control de ingreso, que verifica que cada código QR se use
> una sola vez (RN-18).

## Páginas 72 a 85 — Especificaciones CU-01 a CU-08

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**En las ocho especificaciones, borrar el diagrama chico** que está debajo del título de cada una
(son ocho figuras, una por caso de uso). Las relaciones de cada caso de uso ya están en el
diagrama general de la p. 71.

**Títulos** (en el título de la especificación y en el campo «Id y nombre»):

| Dice | Poner |
|---|---|
| «CU-06 – Suscribirse al sistema» | «CU-06 – Gestionar suscripción» |
| «CU-07 – Gestionar listas de espera» | «CU-07 – Consultar listas de espera» |
| «CU-08 – Ver reportes de ventas» | «CU-08 – Generar reportes» |

**E-09 · CU-01, pp. 72 y 73 · R (T-1 y T-2).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «arma un paquete personalizado (entradas + servicios), se aplica el descuento por nivel/suscripción y se realiza un pago simulado. Si todo es correcto, se registra la compra y se genera la entrada digital» | «arma un paquete personalizado (entradas, servicios y, si quiere, seguro), se aplica el descuento por nivel de fidelidad y se realiza un pago simulado. Si todo es correcto, se registra la compra y se genera una entrada digital por cada entrada comprada.» |
| Paso 4 | «El sistema calcula el total del paquete y aplica automáticamente descuentos por nivel de fidelidad y/o suscripción activa.» | «El sistema calcula el total del paquete y aplica automáticamente el descuento por nivel de fidelidad; con suscripción activa, el seguro no tiene cargo.» |
| Paso 5 | «El Cliente confirma y pasa a la pantalla de pago simulado.» | «El Cliente confirma; el sistema reserva el cupo por diez minutos y abre la pantalla de pago simulado.» |
| Paso 8, segunda viñeta | «Actualiza los puntos / nivel del Cliente.» | «Actualiza el contador de compras del Cliente y, si corresponde, su nivel, que rige desde la compra siguiente.» |
| Paso 8, tercera viñeta | «Genera la entrada digital (código QR) y la asocia al Cliente.» | «Genera un ticket en PDF por entrada, cada uno con su código QR único.» |
| FA1 | «Si el Cliente intenta confirmar sin entradas seleccionadas, el sistema muestra mensaje de error y vuelve al paso 3.» | «Mientras el paquete no tenga al menos una entrada, el sistema mantiene deshabilitados el botón «Comprar» y la casilla del seguro, y el Cliente sigue en el paso 3.» |
| Después de FA3, agregar | — | «FA4 – Evento en preventa. Si el evento está en preventa y el Cliente no tiene suscripción activa, el sistema informa que la compra está reservada a suscriptores y no permite armar el paquete (RN-39).» |
| Después de FA4, agregar | — | «FA5 – Reserva vencida. Si pasan diez minutos sin pago aprobado, el sistema informa que venció la reserva y libera el cupo; el Cliente debe volver a armar la compra (RN-15).» |
| Después de FA5, agregar | — | «FA6 – Lugar reservado para suscriptores. Si el sector elegido acaba de liberar una entrada y el evento tiene inscriptos con suscripción en su lista de espera, durante quince minutos solo ellos pueden comprarla. A cualquier otro Cliente el sistema le informa: «El lugar liberado está reservado temporalmente para suscriptores inscriptos en la lista de espera. Intentá al finalizar la ventana de prioridad.», y no reserva el cupo (RN-35).» |
| Postcondición 1 | «Existe un registro de Compra asociado al Cliente y al Paquete.» | «Existe un registro de Compra asociado al Cliente, con sus entradas, servicios y pagos.» |
| Postcondición 3 | «Se actualizan sus puntos de fidelidad y, si corresponde, su nivel.» | «Se actualiza su contador de compras y, si corresponde, su nivel.» |


**E-10 · CU-02, pp. 74 y 75 · R (T-7).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «Permite al Administrador crear, modificar y desactivar eventos de la plataforma, controlando la información básica (nombre, fecha, ubicación, precio, cupos).» | «Permite al Administrador crear, suspender y eliminar eventos de sus locales, y administrar sus sectores (nombre, precio y capacidad) y su período de preventa.» |
| Paso 1 | «El Administrador accede al módulo “Gestionar eventos”.» | «El Administrador ingresa a «Mis locales» y elige «Ver eventos» de un local.» |
| Paso 3, viñeta 1 | «Crear un nuevo evento (título, fecha, ubicación, cupo, precios).» | «Crear un nuevo evento (nombre, fecha, categoría y preventa). Queda en borrador hasta tener un sector con capacidad.» |
| Paso 3, viñeta 2 | «Modificar datos de un evento existente.» | «Agregar o quitar sectores, con su precio y capacidad, y cambiar la preventa.» |
| Paso 3, viñeta 3 | «Desactivar o eliminar un evento (si aún no tiene ventas asociadas, según regla).» | «Eliminar un evento sin ventas, o suspenderlo.» |
| Paso 5 | «(alta, modificación o baja lógica)» | «(alta, cambio de sectores o de preventa, suspensión o eliminación)» |
| FA2 | «ofrece marcarlo como “No disponible”.» | «ofrece suspenderlo.» |
| Después de FA2, agregar | — | «FA3 – Sectores. Para agregar o quitar sectores del evento, el Administrador elige «Administrar evento» (CU-11, que extiende este caso de uso).» |
| Postcondición 1 | «La tabla de Evento queda actualizada con las altas, modificaciones o bajas lógicas.» | «Los eventos y sus sectores quedan actualizados.» |
| Postcondición 2 | «Solo se mostrarán a los Clientes los eventos que estén activos y con cupo.» | «Solo se muestran a los Clientes los eventos publicados con fecha futura; los agotados aparecen con la opción de lista de espera.» |

**E-11 · CU-03, p. 76 · R.**

| Lugar | Dice | Poner |
|---|---|---|
| Paso 1 | «El Cliente ingresa a su perfil y selecciona “Ver historial de compras”.» | «El Cliente selecciona «Mis eventos» en su menú.» |
| Paso 2 | «(evento, fecha, monto, estado)» | «(pedido, evento, fecha, estado, cantidad de entradas, total pagado y monto devuelto)» |
| Paso 3 | «El Cliente selecciona una compra para ver el detalle del paquete (entradas, servicios adicionales, descuentos).» | «El Cliente selecciona una compra para ver sus entradas: sector, precio, estado, seguro y código. Si alguna tiene seguro, puede elegirla para devolverla.» |
| FA1 | «“No se registran compras realizadas”» | ««Todavía no tenés compras.»» |
| Postcondición | «No se modifica información; solo se consulta.» | «Sin devolución, no se modifica información; solo se consulta.» |

**E-12 · CU-04, pp. 77 y 78 · R.**

| Lugar | Dice | Poner |
|---|---|---|
| Paso 4 | «El sistema crea el Cliente con nivel inicial (ej. Bronce).» | «El sistema crea el Cliente con nivel Bronce y sin suscripción (RN-03).» |
| FA1, punto 1 | «En el paso 4, si el correo ya existe, el sistema informa: “El correo ingresado ya se encuentra registrado”.» | «En el paso 3, si el correo ya existe, el sistema informa: «Ya existe una cuenta registrada con ese mail.»» |
| FA1, punto 2 | «Ofrece al usuario ir a “Iniciar sesión” o recuperar contraseña (si se implementa).» | «El usuario puede volver a «Iniciar sesión» con «Ya tengo cuenta».» |
| Postcondición 1 | «Existe un nuevo registro en la tabla CLIENTE con un id_cliente único.» | «Existe un nuevo usuario con perfil Cliente, identificado por su correo.» |

**E-13 · CU-05, pp. 79 y 80 · R (T-8).**

| Lugar | Dice | Poner |
|---|---|---|
| FA1, punto 1 | «En el paso 5, si el correo no existe o la contraseña es inválida, el sistema muestra un mensaje: “Usuario o contraseña incorrectos”.» | «En el paso 2, si el correo no existe, la contraseña es inválida o la cuenta está bloqueada, el sistema muestra el mismo mensaje: «Mail o contraseña incorrectos.»» (K-3 corregido en `2af267a`, 14/11) |
| Título de FA2 | «FA2 – Usuario bloqueado (opcional)» | «FA2 – Usuario bloqueado» |
| FA2, punto 1 | «Si se detectan múltiples intentos fallidos, el sistema puede marcar el usuario como bloqueado temporalmente.» | «Tras cinco intentos fallidos consecutivos, el sistema bloquea la cuenta por quince minutos (RN-04).» |
| FA2, punto 2 | «Se indica al usuario que contacte soporte o espere un tiempo.» | «El sistema no revela el bloqueo: muestra el mismo mensaje que en FA1. Pasados los quince minutos, el usuario puede volver a intentar.» |


**E-14 · CU-06, pp. 80 a 82 · R (T-13).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «Permite contratar una suscripción (mensual/anual) que otorga beneficios adicionales, usando un flujo de pago simulado.» | «Permite contratar una suscripción mensual o anual, que otorga beneficios en especie, y cancelarla. El cobro del plan queda fuera del alcance de esta versión (RF-13).» |
| Actores secundarios | «Pasarela de pago simulada» | «-» |
| Precondición 2 | «El Cliente no tiene una suscripción activa (o está por vencer según la regla que definas).» | «El Cliente no tiene una suscripción activa.» |
| Precondición 3 | «La pasarela de pago está disponible en modo simulación.» | Borrar |
| Paso 1 | «El Cliente entra a su perfil y selecciona “Suscribirse”.» | «El Cliente selecciona «Mi suscripción» en su menú.» |
| Paso 5 | «El Cliente confirma e ingresa datos de pago (simulado).» | «El Cliente confirma.» |
| Paso 6 | «El sistema envía operación a la pasarela simulada.» | Borrar y renumerar |
| Paso 7 | «Si se aprueba, registra la Suscripción (fecha inicio/fin) y marca cliente como suscripto.» | «El sistema registra la Suscripción con su plan, precio y fechas de inicio y fin.» |
| FA1 completo | «FA1 – Pago rechazado» y sus dos puntos | «FA1 – Cancelar suscripción. Con una suscripción activa, el Cliente puede cancelarla. El sistema pide confirmación («Los beneficios se pierden desde ahora») y la da de baja, sin reintegro (RN-26).» |
| Postcondición 1 | «Si el pago fue aprobado, el Cliente queda con un registro de Suscripción activo asociado a su cuenta.» | «El Cliente queda con un registro de Suscripción activo asociado a su cuenta.» |
| Postcondición 2 | «(descuentos, seguro, prioridad)» | «(seguro sin cargo, preventa y prioridad en la lista de espera), sin descuento porcentual (RN-37)» |

**E-15 · CU-07, p. 83 · R (T-9).** El paso 4 está al final de esta sección.

| Lugar | Dice | Poner |
|---|---|---|
| Paso 1 | «El Administrador ingresa a “Gestionar listas de espera”.» | «El Administrador elige «Listas de espera» en su menú.» |
| Paso 5 | «El Administrador puede exportar o registrar esta información.» | «El Administrador puede elegir otro evento o volver al menú.» |
| FA1 | «Si el Administrador selecciona un evento sin usuarios en lista, el sistema muestra el mensaje: “Este evento no tiene lista de espera asociada”.» | «Si ningún evento de sus locales está agotado ni tiene lista, el sistema muestra: «No hay eventos agotados ni listas de espera en tus locales.»» |
| Debajo de las postcondiciones, agregar | — | «La inscripción en la lista la realiza el Cliente desde la compra (CU-01, FA3).» |

**E-16 · CU-08, pp. 84 y 85 · R (T-10).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «visualice reportes simulados con información agregada» | «visualice reportes con información agregada, calculados sobre los datos registrados (en esta práctica, un juego de datos ficticio),» |
| Paso 1 | «El Administrador selecciona la opción “Ver reportes de ventas”.» | «El Administrador elige «Reportes» en su menú.» |
| Paso 2, agregar una viñeta | — | «Demanda insatisfecha (listas de espera).» |
| Paso 4 | «El sistema genera el reporte con datos simulados y lo muestra en forma de tabla y/o gráficos.» | «El sistema genera el reporte y lo muestra en una tabla, con una lectura del resultado.» |
| Paso 5 | «(rango de fechas, categoría de evento)» | «(rango de fechas y, según el reporte, categoría, evento o solo eventos agotados)» |
| Paso 6 | «Opcionalmente, el Administrador puede exportar el reporte (por ejemplo, a PDF o imagen, aunque sea conceptual en esta práctica).» | «Opcionalmente, el Administrador exporta el reporte a PDF.» |
| FA1, punto 1 | «el sistema muestra un mensaje (“No se encontraron registros para el período seleccionado”).» | «el sistema lo informa en la lectura del reporte, por ejemplo: «No hay eventos en el período elegido.»» |
| Debajo de FA1 | Un «2.» suelto, sin texto | Borrarlo |

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Insertar** a continuación de CU-08, con el mismo formato que CU-01 a CU-08 (los mismos estilos
de título y de campos). No llevan diagrama chico.

**CU-09 – Cambiar contraseña**

**Id y nombre:** CU-09 – Cambiar contraseña

**Descripción:** Permite a un usuario registrado, Cliente o Administrador, reemplazar su
contraseña. Para hacerlo debe conocer la contraseña actual.

**Actor:** Cliente o Administrador

**Actores secundarios:** -

**Precondiciones:**
- El usuario tiene una cuenta registrada.
- No necesita haber iniciado sesión: la opción está en la pantalla de inicio.

**Flujo principal:**
1. El usuario elige «Cambiar contraseña» en la pantalla de inicio de sesión.
2. El sistema muestra cuatro campos: mail, contraseña actual, nueva contraseña y confirmación.
   Las contraseñas se muestran enmascaradas, salvo que el usuario marque «Mostrar contraseñas».
3. El usuario completa los campos y presiona «Confirmar».
4. El sistema verifica que los cuatro campos estén completos y que la nueva contraseña coincida
   con su confirmación.
5. El sistema verifica el mail y la contraseña actual (RN-40).
6. El sistema verifica que la nueva contraseña cumpla RN-02.
7. El sistema guarda la nueva contraseña cifrada, vacía los campos de contraseña e informa «La
   contraseña se cambió correctamente.».

**Flujo alternativo:**

FA1 – Datos incompletos. En el paso 4, si falta un campo, el sistema pide completarlo, por
ejemplo «Ingresá la contraseña actual.». Si la confirmación no coincide, informa «Las contraseñas
no coinciden». El usuario vuelve al paso 3.

FA2 – Credenciales incorrectas. En el paso 5, si el mail no existe o la contraseña actual es
incorrecta, el sistema informa «Mail o contraseña actual incorrectos.», el mismo mensaje en los
dos casos. Una contraseña actual incorrecta cuenta como intento fallido (RN-04).

FA3 – Cuenta bloqueada. Si la cuenta está bloqueada, o si el intento fallido completa cinco
consecutivos, el sistema rechaza el cambio con el mismo mensaje de FA2, «Mail o contraseña
actual incorrectos.». No informa que la cuenta está bloqueada ni cuántos intentos quedan: como la
opción no exige sesión, cualquier otro mensaje revelaría qué cuentas existen. El bloqueo se
registra igual y rige también para el inicio de sesión (RN-04).

FA4 – Contraseña débil. En el paso 6, si la nueva contraseña no tiene al menos ocho caracteres,
una letra y un dígito, el sistema informa «La contraseña debe tener al menos 8 caracteres, con al
menos una letra y un dígito.» y no la guarda. Como la contraseña actual era correcta, no se
cuenta un intento fallido.

**Postcondiciones:**
- La contraseña anterior deja de servir y la nueva permite iniciar sesión.
- El contador de intentos fallidos de la cuenta vuelve a cero.
- No se inicia sesión: el usuario vuelve a la pantalla de inicio con «Volver».

---

**CU-10 – Registrar local**

**Id y nombre:** CU-10 – Registrar local

**Descripción:** Permite al Administrador dar de alta un local donde se realizarán sus eventos,
con su capacidad máxima.

**Actor:** Administrador

**Actores secundarios:** -

**Precondiciones:**
- El Administrador inició sesión.

**Flujo principal:**
1. El Administrador elige «Registrar local» en su menú.
2. El sistema muestra los campos nombre, dirección, altura y capacidad.
3. El Administrador completa los campos y presiona «Registrar local».
4. El sistema verifica que el local tenga nombre y que la capacidad sea un número entero no
   negativo. La dirección y la altura no se validan: se guardan juntas, como un único texto.
5. El sistema registra el local a nombre del Administrador, informa «Local registrado
   correctamente» y vuelve al menú.

**Flujo alternativo:**

FA1 – Capacidad inválida. En el paso 4, si la capacidad no es un número entero, el sistema
informa «La capacidad debe ser un número entero.»; si es negativa, «La capacidad del local no
puede ser negativa.». El Administrador vuelve al paso 3.

FA2 – Sin nombre. En el paso 4, si el nombre está vacío, el sistema informa «El local debe tener
nombre.» y el Administrador vuelve al paso 3.

FA3 – Volver sin registrar. El Administrador puede elegir «Volver» en el paso 3; el sistema
vuelve al menú sin registrar nada.

**Postcondiciones:**
- El local queda registrado y aparece en «Mis locales».
- Su capacidad limita la suma de los sectores de cada evento que se realice en él (RN-38).

---

**CU-11 – Gestionar sectores**

**Id y nombre:** CU-11 – Gestionar sectores

**Descripción:** Permite al Administrador agregar y quitar los sectores de un evento, cada uno
con nombre, precio y capacidad. Extiende CU-02 cuando el Administrador agrega o quita un sector.

**Actor:** Administrador

**Actores secundarios:** -

**Precondiciones:**
- El Administrador inició sesión.
- Eligió un evento de uno de sus locales (CU-02).

**Flujo principal:**
1. En «Eventos del local», el Administrador elige un evento y presiona «Administrar Evento».
2. El sistema muestra los sectores del evento, con su nombre, precio y capacidad, y la fecha de
   fin de la preventa.
3. El Administrador presiona «Agregar sector».
4. El sistema muestra los campos nombre, capacidad y precio.
5. El Administrador los completa y presiona «Agregar sector».
6. El sistema verifica los datos y que la suma de las capacidades de los sectores no supere la
   del local (RN-38).
7. El sistema registra el sector, informa «Sector agregado correctamente.» y vacía los campos
   para cargar otro. Si el evento estaba en borrador y el sector tiene capacidad, el evento queda
   publicado (RN-06).

**Flujo alternativo:**

FA1 – Quitar un sector. En el paso 3, el Administrador elige un sector de la lista y presiona
«Eliminar sector». El sistema lo elimina sin pedir confirmación y actualiza la lista. Si no
eligió ninguno, informa «Por favor, seleccioná un sector para eliminar.».

FA2 – Sector con ventas. En FA1, si el sector tiene entradas vendidas o devueltas, el sistema
informa «El sector tiene entradas vendidas: no se puede eliminar.».

FA3 – Último sector con capacidad. En FA1, si el evento está publicado y es su último sector con
capacidad, el sistema informa «No se puede eliminar el sector: es el último con capacidad mayor
que cero de un evento publicado.» (RN-06).

FA4 – Capacidad del local. En el paso 6, si la suma supera la capacidad del local, el sistema
informa la capacidad del local y la que queda disponible para el evento, y no registra el
sector (RN-38).

FA5 – Datos inválidos. En el paso 6, el sistema informa «Por favor, completá todos los
campos.», «La capacidad debe ser un número entero.», «El precio debe ser un número, por ejemplo
12500.50.» o «La capacidad y el precio del sector no pueden ser negativos.», según el caso. El
Administrador vuelve al paso 5.

**Postcondiciones:**
- Los sectores del evento quedan actualizados.
- La capacidad del evento es la suma de las de sus sectores (RN-07).
- Un evento publicado conserva al menos un sector con capacidad (RN-06).

---

**CU-12 – Controlar ingreso**

**Id y nombre:** CU-12 – Controlar ingreso

**Descripción:** Permite al Administrador validar en la puerta las entradas de un evento de
sus locales, con el código QR del ticket leído por un lector USB o tipeado desde el PDF. Cada entrada
ingresa una sola vez (RN-18).

**Actor:** Administrador

**Actores secundarios:** -

**Precondiciones:**
- El Administrador inició sesión.
- Al menos un evento publicado de sus locales tiene el ingreso abierto: desde seis horas antes
  hasta doce horas después de su inicio.

**Flujo principal:**
1. El Administrador elige «Control de ingreso» en su menú.
2. El sistema muestra los eventos publicados de los locales del Administrador que tienen el
   ingreso abierto en ese momento, aunque hayan empezado el día anterior; el contador
   de ingresos registrados del evento elegido, y el mensaje «Escaneá una entrada.».
3. El Administrador elige el evento.
4. El Administrador lee el código del ticket con el lector, que lo confirma con Enter, o lo
   tipea y presiona «Registrar ingreso».
5. El sistema verifica que el código exista, que la entrada sea de ese evento, que esté
   confirmada y sin ingreso previo, y que la hora actual esté dentro del horario de ingreso:
   desde seis horas antes hasta doce horas después del inicio del evento.
6. El sistema registra la fecha y hora del ingreso, muestra «Ingreso válido. Bienvenido.» en
   verde, actualiza el contador y deja el campo vacío para la próxima entrada.

**Flujo alternativo:**

FA1 – Sin eventos con ingreso abierto. En el paso 2, si ningún evento de sus locales tiene el
ingreso abierto, el sistema muestra «No hay eventos con ingreso abierto en tus locales.» y
deshabilita el campo y el botón.

FA2 – Entrada rechazada. En el paso 5, si alguna verificación falla, el sistema no registra el
ingreso y muestra el motivo en rojo:
- «El código no existe.»
- «La entrada corresponde a otro evento.»
- «La entrada está reembolsada.»
- «La entrada está pendiente o vencida.»
- «La entrada ya ingresó el» seguido de la fecha y hora del primer ingreso.
- «Fuera del horario de ingreso o evento no disponible.»

FA3 – Lectura simultánea. Si dos puertas leen la misma entrada al mismo tiempo, solo una
registra el ingreso. La otra muestra que la entrada ya ingresó o, si la entrada cambió por otro
motivo, «La entrada cambió. Volvé a escanear.».

FA4 – Evento de otro Administrador. Si el evento no es de uno de sus locales, el sistema no
registra ni cuenta ingresos e informa «Solo podés controlar el ingreso de eventos de tus
locales.» (RN-42).

**Postcondiciones:**
- La entrada queda con su fecha de ingreso: no puede volver a ingresar ni devolverse (RN-29).
- El contador muestra los ingresos registrados del evento.

---

**CU-13 – Devolver entrada**

**Id y nombre:** CU-13 – Devolver entrada

**Descripción:** Permite al Cliente devolver una o más entradas aseguradas de una compra, hasta
cuarenta y ocho horas antes del evento, y recibir el reintegro que fija RN-29. Extiende CU-03
cuando la compra tiene entradas con seguro.

**Actor:** Cliente

**Actores secundarios:** -

**Precondiciones:**
- El Cliente inició sesión.
- Tiene una compra confirmada con al menos una entrada asegurada.

**Flujo principal:**
1. En «Mis eventos» (CU-03), el Cliente elige una compra.
2. El sistema muestra sus entradas. Solo las confirmadas y con seguro tienen habilitada la
   casilla «Devolver». Una ayuda indica el plazo y aclara que no se reintegran el cargo por
   servicio ni el seguro.
3. El Cliente marca las entradas que quiere devolver; el botón muestra «Devolver» con la
   cantidad elegida.
4. El Cliente presiona el botón.
5. El sistema pide confirmación, con «No» como respuesta predeterminada: «¿Devolver N
   entrada(s)? Se reintegra el precio de cada entrada con su descuento. Si devolvés todas las que
   quedan, también los servicios. El cargo por servicio y el seguro no se reintegran. La
   devolución no se puede deshacer.».
6. El Cliente confirma.
7. El sistema vuelve a verificar con los datos guardados: que la compra sea del Cliente y esté
   confirmada; que cada entrada esté confirmada, asegurada y sin ingreso registrado, y que
   falten más de cuarenta y ocho horas para el evento.
8. El sistema registra la devolución: las entradas quedan reembolsadas y su cupo vuelve a estar
   disponible. Si no queda ninguna entrada confirmada en la compra, la compra pasa a
   reembolsada.
9. El sistema informa «Devolución registrada. Se reintegran» y el importe, y actualiza el
   historial.

El importe es el precio de cada entrada devuelta menos su parte del descuento por nivel. Si con
esa devolución no queda ninguna entrada confirmada, se reintegran además los servicios.

**Flujo alternativo:**

FA1 – El Cliente no confirma. En el paso 6, si responde «No», no cambia nada.

FA2 – Fuera de plazo. En el paso 7, si faltan cuarenta y ocho horas o menos, el sistema informa
«El seguro permite devolver hasta 48 horas antes del evento (hasta el» y el límite, seguido de
«Ese plazo ya pasó.».

FA3 – Entrada usada. En el paso 7, si alguna entrada ya registró su ingreso, el sistema informa
«No podés devolver una entrada que ya se usó para ingresar.».

FA4 – La compra cambió. Si otra operación modificó la compra mientras tanto, el sistema informa
«La compra cambió mientras se procesaba la devolución. Volvé a abrir el historial.» y recarga el
historial.

**Postcondiciones:**
- Las entradas devueltas quedan reembolsadas y su cupo se libera para otra compra. Los clientes
  de la lista de espera del evento ven el aviso la próxima vez que inician sesión.
- Si la compra quedó reembolsada, deja de computar para el nivel (RN-24); el nivel vigente se
  conserva hasta la reevaluación mensual (RN-23).

## Página 86 — Modelo de dominio conceptual y diagrama de clases

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Modelo de dominio conceptual:** reemplazar la figura por `docs/diagramas/dominio-conceptual.png`,
a **16,0 cm**. Texto debajo del título:

> El modelo de dominio conceptual muestra los conceptos del negocio y cómo se relacionan, sin
> atributos ni operaciones y sin clases de software. Surge de los requerimientos (RF-01 a RF-13) y
> de las reglas de negocio. Los usuarios son de dos tipos, Cliente y Administrador. El
> Administrador gestiona locales y los eventos que se realizan en ellos, divididos en sectores, y
> cada evento ofrece servicios complementarios. El Cliente arma un paquete para un evento, con
> entradas y servicios; al confirmarse, el paquete se convierte en una compra, que emite las
> entradas, cada una correspondiente a un sector y opcionalmente cubierta por un seguro. Una compra
> admite varios intentos de pago. El Cliente tiene un nivel de fidelidad, puede contratar
> suscripciones y anotarse en la lista de espera de un evento agotado.

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

> El diagrama de clases se presenta en vistas, porque el sistema completo supera las cuarenta
> clases y en una sola figura resultaría ilegible. Las vistas siguen las capas de la arquitectura:
> tres para las entidades del dominio (proyecto Entidades), tres para la capa de negocio (proyecto
> Repositorio, espacio de nombres Negocio) y una para la capa de datos. Una clase que aparece en
> más de una vista se dibuja completa en una sola y, en las demás, solo con su nombre y la
> indicación de la vista donde está completa.
>
> Las entidades son clases sin lógica, con propiedades públicas. Cada asociación entre entidades
> se implementa con una propiedad que guarda el identificador de la otra (por ejemplo,
> `Entrada.IdEvento`); esas propiedades no se repiten como atributos, porque la asociación ya las
> representa. La navegabilidad de cada flecha sigue el sentido de esa propiedad en el código, y
> la tabla de justificación indica de qué propiedad sale cada una. Los estados de la entrada, de
> la compra y del pago son enumeraciones con los mismos valores que las restricciones `CHECK` de la
> base de datos.
>
> Tres conceptos del modelo de dominio llevan otro nombre en el código, y así aparecen en las
> vistas: el Administrador es la clase `Propietario`, que es el nombre de su perfil en la base de
> datos (RN-05); la Inscripción en la lista de espera es la clase `ListaEspera`, y el Servicio
> contratado es la clase `CompraServicio`. El Paquete existe como clase pero no se guarda en la
> base: es el armado en curso, y al confirmarlo se convierte en una Compra.
>
> En la capa de negocio, las vistas 4 y 5 muestran las operaciones públicas de cada gestor, más
> las internas que aparecen en el diagrama de secuencia, y la vista 6, la verificación de identidad que agregó la revisión de seguridad: antes de operar, cada
> gestor consulta a `GestorSesion` si quien llama es el titular de los datos (RN-41) o el
> Administrador (RN-34, RN-42). La vista 7 muestra la capa de datos: trece repositorios que
> heredan de `RepositorioBase`, únicos responsables del acceso a SQL Server; de cada uno se
> muestran los métodos que aparecen en los diagramas de secuencia.

**Sección nueva «Patrones de diseño»**, a continuación de las vistas de clases. Tres figuras:

| Subtítulo | Imagen (`docs/diagramas/`) | Ancho |
|---|---|---|
| Singleton y Repository | `clases-8a-patron-singleton-repository.png` | **16,0 cm** |
| Strategy | `clases-8b-patron-strategy.png` | **15,5 cm** |
| Jerarquía de Reporte | `clases-8c-jerarquia-reportes.png` | **16,0 cm** (es baja: puede compartir página con el texto) |

Texto, debajo del título:

> El sistema aplica cuatro patrones de diseño. Cada uno se muestra con su estructura y con un caso
> real del código.
>
> **Singleton.** Los doce gestores que aplican las reglas de negocio, los trece repositorios de
> datos, la conexión, la pasarela simulada y el generador de códigos QR tienen una única instancia: un campo estático privado
> `_instance`, un constructor privado y una propiedad estática `Instance` que crea la instancia la
> primera vez que se pide (veintiocho clases en total). La implementación no usa bloqueo, porque la
> aplicación es de escritorio y todas las llamadas ocurren en el hilo de la interfaz. En la figura,
> el campo `_instance` aparece subrayado por ser estático.
>
> **Repository.** Cada repositorio es el único lugar del sistema que contiene SQL para sus tablas.
> La capa de negocio le pide y recibe entidades, nunca filas ni sentencias. La clase abstracta
> `RepositorioBase` reúne lo común: abrir la conexión, crear comandos con parámetros y leer valores
> que pueden ser nulos.
>
> **Strategy.** El formato de salida de los reportes es una estrategia. `GestorReportes` recibe un
> `IExportador` y no sabe qué formato produce; la pantalla de reportes elige la estrategia concreta,
> `ExportadorPDF`, que está en la capa de presentación porque usa la biblioteca iText. Agregar otro
> formato es agregar otra clase que implemente la interfaz, sin modificar ningún reporte. Hoy existe
> una sola estrategia concreta.
>
> **Jerarquía de reportes.** Los cinco reportes del anexo A02 heredan de la clase abstracta
> `Reporte`, que define qué se informa (encabezados, datos y lectura); cada subclase redefine esas
> tres operaciones. `GestorReportes` elige la subclase según el código del reporte.

## Página 87 — Secuencia y estados

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Diagrama de secuencia – Comprar paquete:** reemplazar la figura por tres, una por página:
`secuencia-compra-1-reserva.png`, `secuencia-compra-2-pago.png` y
`secuencia-compra-3-emision.png`, las tres a **16,0 cm**. Texto debajo del título:

> La secuencia de la compra se presenta en tres partes, que corresponden a las dos operaciones de
> la capa de negocio: `Iniciar`, que reserva el cupo, y `Confirmar`, que cobra y emite las entradas.
> Los participantes son las clases reales del sistema: los formularios de la capa de presentación,
> los gestores de negocio y los repositorios de datos. La pantalla nunca accede a la base de datos:
> lo hace siempre a través de un gestor y un repositorio.
>
> Antes de operar, `GestorCompras` verifica con `GestorSesion` que quien compra es el titular
> (RN-41). La reserva congela los precios y el descuento por nivel, y deja la compra y sus entradas
> en estado Pendiente durante el plazo de reserva (RN-15). Si el pago se rechaza, el cupo se
> devuelve y la compra admite un reintento (RN-32). Si se aprueba, cada entrada recibe un código QR
> único (RN-18), y la compra y sus entradas pasan a Confirmada en una sola transacción. El nivel
> del cliente se reevalúa después de la compra y rige desde la siguiente (RN-22, RN-23).

**Diagrama de estados – Ciclo de vida de la entrada:** reemplazar la figura por
`estados-entrada.png`, a **16,0 cm** (ocupa la página entera: el título pasa a la página nueva).
Texto:

> Una entrada tiene cuatro estados: Pendiente, Confirmada, Reembolsada y Vencida. Son los mismos en
> el diagrama, en la enumeración `EstadoEntrada`, en la restricción `CK_Entradas_Estado` de la
> tabla `Entradas` y en las sentencias de la capa de datos. La entrada nace Pendiente al iniciarse
> la compra, con el cupo del sector reservado; pasa a Confirmada cuando el pago se aprueba y recibe
> su código QR; pasa a Vencida si vence el plazo de reserva o se da de baja el evento o el sector;
> y pasa a Reembolsada si el cliente la devuelve con seguro contratado, con más de 48 horas de
> anticipación y antes de usarla para ingresar.
>
> El ingreso al evento no es un estado de venta: la entrada sigue Confirmada y solo se registra la
> fecha y hora de la primera lectura del código QR. Por eso se modela como un subestado de
> Confirmada, y no como un quinto estado. Una segunda lectura del mismo código se rechaza (RN-18),
> y una entrada que ya ingresó no se puede devolver: la devolución sale del subestado sin ingreso.

## Página 88 — Robustez, dominio actualizado y actividad

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Diagrama de robustez (ICONIX):** reemplazar la figura por `robustez-comprar-paquete.png`, a
**16,0 cm**. Texto:

> El diagrama de robustez del caso de uso «Comprar paquete» vincula su especificación con el
> diseño. Incluye el curso alternativo de la lista de espera cuando el evento está agotado. Las
> fronteras son las tres pantallas que usa el cliente; los controles, las clases de negocio que
> resuelven cada paso; las entidades, los conceptos del dominio que se leen o se modifican. Se
> respetan las reglas de conexión del método: el actor solo se comunica con fronteras, las
> fronteras solo con controles, y las entidades solo con controles. Los controles pueden
> comunicarse entre sí: el control de la compra verifica la identidad del cliente, cobra con la
> pasarela simulada, genera los códigos QR y usa los controles de paquetes y fidelización.
>
> El Cliente aparece dos veces, con dos papeles distintos: como actor es la persona que usa las
> pantallas, y como entidad es la información que el sistema guarda de ella, su identidad para la
> sesión y su nivel de fidelidad. El método distingue así a quien interactúa con el sistema de los
> datos que el sistema conserva sobre él.

**Diagrama de dominio actualizado:** reemplazar la figura por dos, una por página:
`dominio-actualizado-1-cuentas-catalogo.png` y `dominio-actualizado-2-compra.png`, las dos a
**16,0 cm**. Texto:

> El modelo de dominio actualizado retoma los conceptos del modelo conceptual y les agrega sus
> atributos con el tipo de cada uno. No incluye identificadores ni claves foráneas, porque las
> asociaciones ya representan esos vínculos. Se presenta en dos vistas para que se lea a tamaño de
> página: la primera reúne usuarios, catálogo, suscripciones, lista de espera y las enumeraciones;
> la segunda, el paquete, la compra y lo que la compra genera.
>
> Respecto del conceptual, el nivel de fidelidad pasa a ser un atributo del cliente con un tipo
> enumerado (Bronce, Plata, Oro), y la cantidad de compras de los últimos doce meses aparece como
> atributo derivado, porque se calcula y no se guarda (RN-19). Los estados de la compra, de la
> entrada y del pago son enumeraciones con los mismos valores que la base de datos. La relación de
> muchos a muchos entre compra y servicio complementario lleva una clase de asociación, Servicio
> contratado, con la cantidad y el precio unitario congelado al comprar (RN-16).
>
> El dominio expresa la regla del negocio: toda entrada pertenece a una compra y a un sector. En la
> base de datos, las columnas de la compra y del sector de la entrada admiten valores nulos, porque
> se agregaron a una tabla de entradas anterior a ellas; por eso el diagrama de clases y el
> entidad-relación muestran esas dos asociaciones con multiplicidad 0..1. Es una holgura del
> esquema cuya garantía está en el código: el sistema crea las entradas en un único lugar, dentro
> de la transacción de la compra, y siempre con su compra y su sector.

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

> El diagrama de actividad describe el proceso de negocio de punta a punta, desde que el
> Administrador carga un evento hasta que el Cliente ingresa con su entrada. Tiene un carril por
> actor y ninguno para el sistema, que no es un actor del proceso. Se presenta en tres partes,
> unidas por los conectores A y B. El evento se crea en borrador y queda publicado cuando se le
> agrega el primer sector con capacidad (RN-06). El Cliente busca un evento; si está agotado, se
> anota en la lista de espera. Si hay cupo, arma el paquete, elige si agrega el seguro, revisa el
> resumen con el descuento de su nivel y confirma, con lo que reserva el cupo por un plazo
> limitado (RN-15). Puede reintentar el pago mientras dure ese plazo; si no lo concreta, pierde la
> reserva. Con el pago aprobado recibe sus entradas con código QR. Antes del evento puede devolver
> una entrada asegurada con más de 48 horas de anticipación (RN-29); el día del evento la presenta
> en el ingreso, y el Administrador la acepta solo si está confirmada, corresponde al evento, está
> dentro del horario y no ingresó antes (RN-18).

## Página 89 — Entidad-relación

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

Debajo de «DER – Diagrama entidad-relación» (hoy la página tiene solo el título), insertar
`er-1-usuarios-catalogo.png` a **15,4 cm** y, en la página siguiente, `er-2-compras.png` a
**14,6 cm**. Las dos ocupan la página entera. Texto:

> El diagrama entidad-relación se obtiene del script de creación de la base
> (`BaseDatos/01-esquema.sql`) y muestra sus dieciséis tablas en dos vistas. La primera reúne
> usuarios, locales, eventos, sectores, servicios, suscripciones, lista de espera y parámetros; la
> segunda, la compra con sus pagos, entradas, servicios contratados y pólizas. Las tablas que
> aparecen en las dos vistas se muestran completas en una sola y, en la otra, solo con su clave.
>
> Cada columna indica su tipo; el asterisco marca las obligatorias (`NOT NULL`), y las marcas PK,
> FK, UQ y CK señalan la clave primaria, las claves foráneas, las restricciones de unicidad y las de
> verificación. Las relaciones usan la notación de pata de gallo, con la cardinalidad escrita en
> cada extremo. Cliente y Propietario comparten la clave de Usuario (una tabla por tipo de
> usuario). La relación entre Eventos y Servicios es de muchos a muchos y se resuelve con la tabla
> intermedia ServiciosEvento. La contraseña se guarda como `PBKDF2$iteraciones$sal$hash`, nunca en
> texto plano.
>
> La tabla Entradas proviene del esquema original, anterior a las compras. Por eso guarda el
> usuario además de la compra, y por eso la compra y el sector son columnas agregadas después que
> admiten valores nulos. El sistema escribe en cada entrada el mismo usuario de su compra, y nunca
> crea una entrada sin compra ni sin sector. Cada fila es una sola entrada: la cantidad vendida
> vale 1 en todas las filas que guarda el sistema, y cada una tiene su propio código QR, que se
> admite una sola vez en el ingreso (RN-18). La cantidad elegida para cada sector existe solo
> mientras se arma el paquete, que no se guarda en la base.

## Páginas 90 a 97 — Informe de pruebas de usabilidad

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Título de la sección** (y su entrada en el índice): **reemplazar** «Informe de pruebas de
usabilidad» **por** «Informe de pruebas de usabilidad (simulado)», igual que las minutas
llevan «(simuladas)».

**Contenido: reemplazar las páginas 90 a 97 completas** por el informe de
[usabilidad/INFORME-USABILIDAD.md](usabilidad/INFORME-USABILIDAD.md). Tiene los mismos
apartados, en el mismo orden:
- objetivo, diseño, perfiles, tarea y protocolo, métricas;
- resultados por perfil, tiempos contra la línea base, satisfacción, SUS y cumplimiento de
  criterios;
- hallazgos, plan de mejora, limitaciones y conclusión.

Agrega tres apartados: lo medido sobre la aplicación, la evaluación heurística y el recorrido
cognitivo. Las referencias nuevas (Nielsen, 1994; Wharton et al., 1994; Card, Moran y Newell,
1980; Kieras, 2001) van a la Bibliografía de la página 112.

La «Declaración sobre la naturaleza de los datos» del principio del informe **es la que
sostiene el rótulo**: separa lo medido de lo derivado. No hay que acortarla.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

| Dice | Poner |
|---|---|
| «CU-08 – Ver reportes de ventas define el flujo de interacción» | «CU-08 – Generar reportes define el flujo de interacción» |

*Razón:* el título de CU-08 cambió (pp. 84 y 85).

## Páginas 99 a 105 — Anexo A02

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

### Tabla «Catálogo de reportes», columna Filtros

| Fila | Dice | Corresponde |
|---|---|---|
| RPT-04 | Rango de fechas, tipo de beneficio | Rango de fechas |
| RPT-05 | Evento, estado | Rango de fechas, evento, solo agotados |

### Tabla «Definición de los indicadores»

**Reemplazar** la fórmula de «Entradas vendidas»:

> Σ (cantidad de entradas con estado = VENDIDA)

**Por:**

> Σ (cantidad de entradas con estado Confirmada)

`VENDIDA` no existe en el esquema: las reservas sin pagar, las vencidas y las devueltas no
cuentan como ventas.

**Agregar** la fila, después de «% de ocupación», porque RPT-02 la usa:

| Ingreso del paquete | Σ (base de entradas + servicios) de las compras confirmadas; sin cargo por servicio ni seguro | $ |
|---|---|---|

---

### 13. Página 100 — A02: ejemplos 1 y 2

### Ejemplo 1 — RPT-01, fila TOTAL DEL PERÍODO

En la columna «Ticket prom.», **reemplazar** `$19.356` **por** `$19.355`
(86.710.000 ÷ 4.480 = 19.354,9).

### Ejemplo 1 — Lectura

**Reemplazar:**

> Los dos conciertos concentran el 65,8 % del ingreso del período

**Por:**

> Los dos conciertos concentran el 65,9 % del ingreso del período

(57.100.000 ÷ 86.710.000 = 65,85 %, que redondea a 65,9.)

### Ejemplo 2 — RPT-02, columna Paquete

**Reemplazar** «Experiencia VIP» **por** «Entrada + Merchandising + VIP». No hay catálogo de
paquetes: el nombre se arma con los tipos de servicio presentes, en orden alfabético. La
columna Composición no cambia.

---

### 14. Página 101 — A02: ejemplo 5 (RPT-05)

**Reemplazar** «Agotado» **por** «Con cupo» en las tres filas: vendieron 96,2 %, 91,9 % y
87,5 % según RPT-01, así que por RN-10 no están agotados.

En la lectura, **reemplazar:**

> Aplicando el ticket promedio del período ($19.356), la demanda insatisfecha equivale a
> $13.646.000.

**Por:**

> Aplicando el ticket promedio del período ($19.355), la demanda insatisfecha equivale a
> $13.645.275. Que los tres eventos figuren «Con cupo» no contradice la cifra: las listas se
> formaron mientras estaban agotados, y después se liberaron lugares por devoluciones.

---

### 15. Páginas 102 y 103 — A02: diseño técnico

### Página 102 — tabla «Estructura del archivo PDF generado», fila Encabezado

**Reemplazar** «Logotipo EventAura» **por** «Nombre EventAura como texto, en violeta (116, 86,
174) y Nirmala UI». No hay logotipo en el repositorio.

### Página 102 — diagrama de clases, clase `Reporte`

**Reemplazar** la línea:

> List<Filtro> Filtros;

**Por:**

> virtual string Filtros();   // cada reporte declara sus filtros como propiedades y los describe

### Página 103 — clase `RepositorioReportes`

**Reemplazar** las cinco firmas **por:**

```
class RepositorioReportes {                     // Capa de acceso a datos
    DataTable VentasPorEvento(DateTime desde, DateTime hasta, string categoria)
    DataTable RankingPaquetes(DateTime desde, DateTime hasta, int? idEvento)
    DataTable UsuariosPorNivel(DateTime desdeVentana, DateTime corte, int umbralPlata, int umbralOro)
    DataTable AdopcionBeneficios(DateTime desde, DateTime hasta, string planMensual, string planAnual)
    DataTable DemandaInsatisfecha(DateTime desde, DateTime hasta, int? idEvento, bool soloAgotados)
}
```

### Página 103 — clase `ExportadorPDF`

**Reemplazar** el comentario `// Implementación con iText 9.2.0` **por**
`// Capa Visual: iText es presentación; Negocio solo conoce IExportador`.

### Página 103 — diagrama de secuencia

**Reemplazar** la línea:

> ExportadorPDF ──► QRCoder : GenerarQR(codigo de verificación)

**Por:**

> ExportadorPDF ──► GeneradorQR : Imagen(código de verificación)   (usa QRCoder)

---

### 16. Páginas 104 y 105 — A02: SQL de RPT-01 y código de exportación

### Página 104 — «Consulta de origen — RPT-01»

**Reemplazar la consulta completa.** La del documento no corre contra el esquema
(`s.ID_Evento` no existe: es `EventoID`). Además, el `JOIN` con Sectores multiplica las
filas y no filtra el estado:

```sql
SELECT e.Nombre AS Evento,
       e.Categoria AS Categoria,
       ISNULL(SUM(en.CantidadVendida), 0) AS Entradas,
       ISNULL(SUM(en.CantidadVendida * en.Precio), 0) AS IngresoBruto,
       SUM(en.CantidadVendida * en.Precio)
         / NULLIF(SUM(en.CantidadVendida), 0) AS TicketPromedio,
       CAST(100.0 * ISNULL(SUM(en.CantidadVendida), 0)
         / NULLIF((SELECT SUM(s.Capacidad) FROM Sectores s WHERE s.EventoID = e.ID), 0)
         AS DECIMAL(5,1)) AS PorcOcupacion
FROM   Eventos e
LEFT JOIN Entradas en ON en.ID_Evento = e.ID AND en.Estado = 'Confirmada'
WHERE  e.Fecha BETWEEN @Desde AND @Hasta
  AND  (@Categoria IS NULL OR e.Categoria = @Categoria)
GROUP BY e.ID, e.Nombre, e.Categoria
ORDER BY IngresoBruto DESC;
```

**Agregar** debajo:

> La capacidad se obtiene en una subconsulta y no en un JOIN, para no multiplicar las
> filas. El LEFT JOIN conserva los eventos sin ventas, que son los que interesa detectar.
> Se agrupa por el ID del evento: dos eventos con el mismo nombre no se suman.

### Página 104 — código de exportación

**Reemplazar:**

> Document doc = new Document(pdf, PageSize.A4);

**Por:**

> Document doc = new Document(pdf, PageSize.A4, false);   // sin volcado inmediato: el pie necesita el total de páginas

### Página 105 — pie del código de exportación

**Reemplazar** desde `int total = pdf.GetNumberOfPages();` hasta el cierre del `for`, **por:**

```csharp
    int total = pdf.GetNumberOfPages();
    string verificacion = $"EVAU|{r.Codigo}|{r.Desde:yyyyMMdd}-{r.Hasta:yyyyMMdd}|" +
                          $"{DateTime.Now:yyyyMMddHHmmss}|{r.Solicitante}";
    byte[] qr = GeneradorQR.Instance.Imagen(verificacion);
    for (int i = 1; i <= total; i++)
    {
        doc.ShowTextAligned(
            new Paragraph($"Emitido {DateTime.Now:dd/MM/yyyy HH:mm}  –  Página {i} de {total}")
                .SetFont(normal).SetFontSize(7),
            40, 28, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
        // Una imagen nueva por página: la misma instancia de Image no se repite en todas.
        Image img = new Image(ImageDataFactory.Create(qr)).ScaleAbsolute(46, 46);
        img.SetFixedPosition(i, 505, 20);
        doc.Add(img);
    }
```

`GeneradorQR.Crear(...)` no existe: la API es `GeneradorQR.Instance.Imagen(string)`.

## Página 106 — Matriz: criterio y nomenclatura

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**Reemplazar la tabla completa:**

| Caso de prueba | Reglas verificadas |
|---|---|
| CP-01 — Dos compras, nivel Bronce | RN-19, RN-20 |
| CP-02 — Tres compras, nivel Plata | RN-19, RN-20 |
| CP-03 — Seis compras, no asciende | RN-19 |
| CP-04 — Siete compras, nivel Oro | RN-19, RN-20 |
| CP-05 — La base excluye los servicios | RN-21 |
| CP-06 — Ventana móvil de doce meses | RN-19 |
| CP-07 — Precio congelado al iniciar | RN-16 |
| CP-08 — El ascenso rige desde la compra siguiente | RN-22 |
| CP-09 — Reembolso y reevaluación mensual | RN-23, RN-24 |
| CP-10 — La suscripción no acumula porcentaje | RN-37 |
| CP-11 — Verificación de referencia de $ 99.625 | RN-20, RN-21 |
| CP-12 — Alto máximo de los formularios | — (RNF-03) |
| CP-13 — Paquete máximo sin recortes | RN-10 |
| CP-14 — Registro sin datos precargados | — |
| CP-15 — El registro rechaza datos inválidos | RN-02 |
| CP-16 — Búsqueda sin resultados | — |
| CP-17 — Comprar sin evento seleccionado | — |
| CP-18 — Texto guía de la búsqueda | — |
| CP-19 — Volver con un paquete armado | — |
| CP-20 — Cantidad inválida | — |
| CP-21 — Eliminar un evento con y sin ventas | RN-11 |
| CP-22 — Devolución sin entradas elegidas | — |
| CP-23 — Elegir entradas para devolver | — |
| CP-24 — Evento agotado que recupera cupo | RN-10, RN-14 |
| CP-25 — Compra a escala 125 % y 150 % | — (RNF-03) |
| CP-26 — Alta: nivel inicial y correo único | RN-01, RN-03 |
| CP-27 — Inicio de sesión correcto | — |
| CP-28 — Bloqueo por intentos fallidos | RN-04 |
| CP-29 — Datos faltantes o correo inexistente | — |
| CP-30 — Filtros de búsqueda y evento no vigente | RN-09 |
| CP-31 — Topes del paquete | RN-08, RN-12, RN-13 |
| CP-32 — Pago aprobado | RN-17, RN-18, RN-30, RN-31 |
| CP-33 — Pago rechazado | RN-17, RN-31, RN-32 |
| CP-34 — Reserva vencida | RN-15 |
| CP-35 — Acceso a reportes | RN-34 |
| CP-36 — RPT-01 cuenta solo lo confirmado | — |
| CP-37 — RPT-05 y lista de espera | RN-10, RN-36 |
| CP-38 — Exportación y período inválido | — |
| CP-39 — Temporizador de la reserva | — |
| CP-40 — Cupo restante por sector | — |
| CP-41 — Confirmación al comprar sin seguro | — |
| CP-42 — Explicación de los niveles | — |
| CP-43 — El plazo lo fija la reserva | RN-15 |
| CP-44 — Alta sin selector de perfil | — |
| CP-45 — El alta pública crea clientes | RN-03 |
| CP-46 — Volver al inicio cierra la sesión | — |
| CP-47 — La aplicación termina al cerrar la última ventana | — (RNF-02) |
| CP-48 — Cerrar la compra confirma el descarte | — |
| CP-49 — Indicación de dónde se escribe la cantidad | — |
| CP-50 — Comprar y seguro requieren entradas | — |
| CP-51 — Mensaje del manejador global según el error | — (RNF-02) |
| CP-52 — Pago de un evento eliminado | — |
| CP-53 — El manejador se instala al iniciar | — (RNF-02) |
| CP-54 — Encabezados de la grilla completos | — |
| CP-55 — Perfil de usuario inválido | RN-05 |
| CP-56 — Último sector de un evento publicado | RN-06 |
| CP-57 — Servicio que el evento no ofrece | RN-14 |
| CP-58 — Segunda suscripción activa | RN-25 |
| CP-59 — Devolución sin seguro o fuera de plazo | RN-29 |
| CP-60 — Sectores que superan la capacidad del local | RN-38 |
| CP-61 — Preventa sin suscripción | RN-39 |

**y agregar a continuación** estas filas (CP-62 a CP-148):

| CP-62 — Los colores y las fuentes de Tema | — (RNF-02) |
| CP-63 — El código de cada una de las 20 pantallas y de su Designer | — (RNF-02) |
| CP-64 — El proyecto Visual | — (RNF-03) |
| CP-65 — Ticket PDF de una entrada confirmada, generado en una carpeta temporal | RN-18 |
| CP-66 — Abrir el login; iniciar sesión como Administrador | RN-05 |
| CP-67 — Dos inicios de sesión seguidos del Administrador; un cliente con contraseña heredada en texto plano | — |
| CP-68 — Contraseñas incorrectas desde la pantalla de login, hasta el bloqueo; parámetros de bloqueo de 10 y 5 minutos | RN-04 |
| CP-69 — «Agregar evento» al 100 % y al 150 % | RN-39 |
| CP-70 — Recorrer «Registrar local» con Tab; dar de alta un local | — |
| CP-71 — «Mis locales» con dos locales; mover la celda actual al segundo | — (RNF-02) |
| CP-72 — El botón de sectores; precio «12500,50» y «12500.50» | — |
| CP-73 — Alta de un evento con categoría y sin categoría | RN-06 |
| CP-74 — Agregar a un borrador un sector de capacidad 0 y uno de capacidad 10 | RN-06 |
| CP-75 — Desde la pantalla, un sector que supera la capacidad del local; eliminar el último sector | RN-06, RN-38 |
| CP-76 — «Mi suscripción» sin plan | — (RNF-02) |
| CP-77 — Contratar el plan mensual; cancelarlo | RN-25 |
| CP-78 — Dos ventanas de suscripción abiertas; contratar en las dos | RN-25 |
| CP-79 — Resumen de compra de 2 entradas con suscripción y sin ella | RN-37 |
| CP-80 — Pagar con seguro siendo suscriptor; pagar sin seguro | RN-37 |
| CP-81 — Evento en preventa: cliente sin plan, suscriptor; evento sin preventa | RN-39 |
| CP-82 — Alta con preventa; preventa posterior al evento; administrar con preventa; quitarla; administrar sin preventa | RN-06, RN-39 |
| CP-83 — Lista de espera de un evento agotado; un propietario sin listas | RN-35 |
| CP-84 — Un suscriptor inscripto en la lista cancela su plan | RN-35 |
| CP-85 — Menú del propietario al 100 % y al 150 %; menú del cliente; código de Visual | — (RNF-02, RNF-03) |
| CP-86 — La semilla sola, sin datos de volumen | — |
| CP-87 — Los filtros de cada reporte, RPT-01 a RPT-05 | — |
| CP-88 — Los datos de demostración del anexo A02 | — (RNF-02) |
| CP-89 — Exportar a una carpeta temporal; exportar sobre un archivo bloqueado | — |
| CP-90 — PDF exportado desde la pantalla de reportes | — |
| CP-91 — Sesión de un cliente que intenta devolver la compra de otro, con el titular del objeto cambiado a mano | RN-41 |
| CP-92 — Sesión de un cliente que pide el historial, la suscripción y el perfil de otro | RN-41 |
| CP-93 — Sesión de un cliente que elimina un evento | RN-34, RN-42 |
| CP-94 — Sin sesión: 16 operaciones protegidas y CrearAdministrador; después, CrearAdministrador como cliente y… | RN-41, RN-42 |
| CP-95 — Cambiar la contraseña con la actual incorrecta | RN-04, RN-40 |
| CP-96 — Cambiar la contraseña de una cuenta bloqueada, con la actual correcta | RN-04 |
| CP-97 — Cambiar la contraseña con un correo inexistente y con la actual incorrecta | RN-40 |
| CP-98 — Con la sesión del Administrador, cambiar la contraseña de un cliente; abrir la pantalla | RN-40 |
| CP-99 — Entrada confirmada, del evento elegido y dentro del horario; después, un cliente que intenta registrarla y… | RN-18, RN-34, RN-42 |
| CP-100 — El mismo código escaneado dos veces | RN-18 |
| CP-101 — Entrada reembolsada | RN-18, RN-29 |
| CP-102 — Código con formato válido que no existe | RN-18 |
| CP-103 — La entrada en otro evento; el evento dentro de 2 días y hace 2 días | RN-18 |
| CP-104 — Diez puertas escanean el mismo código a la vez, arrancadas juntas por una barrera; cien rondas con un… | RN-18 |
| CP-105 — Inicio de sesión real con hashes de 600.000 y 310.000 iteraciones | — |
| CP-106 — Buscar «[vip]», «50%» y «sector_unico» | — |
| CP-107 — Hashes con iteraciones enormes, 0, negativas o no numéricas, y con sal y hash vacíos | — |
| CP-108 — Hash previo de 100.000 iteraciones | — |
| CP-109 — Preparación completa de la base de pruebas | — |
| CP-110 — Cada estado de EstadoEntrada y el estado ajeno «Usada», escritos en la tabla | RN-18, RN-29 |
| CP-111 — Compra asegurada de 2 entradas: una ya ingresó; devolver las dos, en los dos órdenes | RN-18, RN-29 |
| CP-112 — Los siete componentes visuales en un host de diseño real | — (RNF-01, RNF-02) |
| CP-113 — Contrato de la sesión: Iniciar fuera de lo público; propiedades de la sesión; perfil e identidad del… | RN-41, RN-42 |
| CP-114 — Inicio de sesión con correo existente e inexistente; quinto fallo; cuenta bloqueada | RN-04 |
| CP-115 — Descuento configurado de Plata y de Oro por debajo, igual y por encima de DESCUENTO_MAX_ADMISIBLE | RN-20 |
| CP-116 — El texto de cada botón de las 20 pantallas, dibujado por Windows, también con el borde de foco | — (RNF-02, RNF-03) |
| CP-117 — Consultar la posición en la lista de espera: la propia, con suscriptores adelante, la de otro cliente y… | RN-35, RN-41 |
| CP-118 — Aviso de disponibilidad: evento agotado, entrada liberada, avisos de otra cuenta, sin sesión, reserva que… | RN-41 |
| CP-119 — Resumen de compra vacío y con dos entradas | — (RNF-02) |
| CP-120 — Los componentes nuevos IndicadorPasos y EtiquetaTotal en un host de diseño real | — (RNF-02) |
| CP-121 — Suscripción contratada y vencida; consultarla y abrir su pantalla varias veces | RN-26 |
| CP-122 — Compra confirmada sin seguro; volver a confirmarla agregándole seguro | RN-28 |
| CP-123 — Eventos para el control de ingreso: de otro administrador, sin sesión, y un evento de las 00:30 en los… | RN-18, RN-41, RN-42 |
| CP-124 — Cambiar la contraseña sin sesión, con 0, 4 y 5 fallos previos, y con un correo inexistente | RN-04, RN-40 |
| CP-125 — Un sector vuelve a tener una entrada (devolución, reserva vencida o pago rechazado) y hay un suscriptor… | RN-35, RN-37 |
| CP-126 — Pedir la lista de espera de un evento como cliente y sin sesión | RN-34 |
| CP-127 — Pedir los pagos de una compra ajena y sin sesión | RN-41 |
| CP-128 — Pedir por su número una compra de otro cliente, y sin sesión | RN-41 |
| CP-129 — Pedir las pólizas de seguro de otro cliente, y sin sesión | RN-41 |
| CP-130 — Iniciar sesión usando como contraseña el hash guardado de una cuenta ya migrada a PBKDF2 | RN-04 |
| CP-131 — Registrar el ingreso de una entrada de un evento suspendido, dentro del horario de ingreso | RN-18 |
| CP-132 — Registrar en la base el ingreso de una entrada contra un evento distinto del suyo, sin el filtro previo… | RN-18 |
| CP-133 — La entrada se devuelve entre la lectura y la escritura del ingreso | RN-18, RN-29 |
| CP-134 — Registrar el ingreso de una entrada pendiente y de una vencida | RN-18 |
| CP-135 — Registrar ingresos en el segundo exacto de apertura y de cierre de la ventana, y un segundo antes y después | RN-18 |
| CP-136 — Leer un código de entrada con espacios alrededor | RN-18 |
| CP-137 — Consultar si otro cliente está inscripto en una lista de espera, y sin sesión | RN-41 |
| CP-138 — Contar las compras de fidelización de otro cliente, y sin sesión | RN-41 |
| CP-139 — Calcular el descuento de otro cliente, y sin sesión | RN-41 |
| CP-140 — Armar un paquete a nombre de otro cliente, y sin sesión | RN-41 |
| CP-141 — Consultar los locales de un propietario como cliente, y sin sesión | RN-34 |
| CP-142 — Listar los eventos con ingreso abierto como cliente, y sin sesión | RN-34 |
| CP-143 — Ejecutar un reporte ya creado después de cerrar la sesión del administrador o como cliente | RN-34 |
| CP-144 — Pedir el catálogo de eventos de un reporte como cliente, y sin sesión | RN-34 |
| CP-145 — Insertar dos entradas con el mismo código QR | RN-18 |
| CP-146 — Registrar el ingreso de una entrada como cliente y sin sesión | RN-34 |
| CP-147 — Insertar en la base un segundo seguro para una entrada que ya tiene uno, y uno para otra entrada | RN-28 |
| CP-148 — Comprar, pagar y cerrar la pantalla de confirmación (interfaz) | — |

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

lógica de descuento por nivel». La columna «Prueba automatizada» es el nombre del método en
`Pruebas/`.

| Caso | Qué verifica | Prueba automatizada | RF | Hall. | Ejec. |
|---|---|---|---|---|---|
| CP-11 | Cliente Plata, 2 entradas Campo a $ 25.000 y hospedaje de $ 45.000: los siete pasos del cálculo, total $ 99.625 | `Referencia_DaExactamente99625` | RF-05, RF-08 | — | 1 |
| CP-12 | Ningún formulario declara un área cliente de más de 790 px de alto | `RNF03_NingunFormularioPasaDe790DeAlto` | RNF-03 | — | 1 |
| CP-13 | Paquete máximo con título de 100 caracteres: título, nivel y enlace sin superponerse; cada importe y el TOTAL completos, con la leyenda del descuento. En un evento agotado, servicios deshabilitados | `Compra_PaqueteMaximoYNombreLargoSinRecortes` | RF-05 | H-01 | 1 |
| CP-14 | El alta de usuario abre con todos los campos vacíos | `Registro_AbreSinDatosPrecargados` | RF-01 | — | 1 |
| CP-15 | Nombre, apellido o correo con solo espacios, y contraseña «abc»: aviso y sin alta | `Registro_RechazaDatosInvalidosSinCrearUsuario` | RF-01 | — | 4 |
| CP-16 | Búsqueda sin resultados: una fila de aviso, grilla sin edición | `Eventos_SinResultadosNoPermiteAgregarFilas` | RF-03 | — | 1 |
| CP-17 | «Comprar» sin evento elegido: aviso «Seleccionar evento», no abre la compra | `Eventos_ComprarSinResultadosMuestraAvisoCorrecto` | RF-03 | — | 1 |
| CP-18 | Texto guía visible con el cursor en la búsqueda | `Eventos_GuiaRegistradaConFocoEnLaBusqueda` | RF-03 | — | 1 |
| CP-19 | «Volver al listado» pide confirmación, con No por defecto; No conserva el paquete | `Compra_VolverConfirmaSinPerderPaqueteAlCancelar` | RF-05 | H-06 | 1 |
| CP-20 | Cantidad «abc» o «-1»: aviso y se restaura la anterior | `Compra_CantidadInvalidaRestauraElPaquete` | RF-04 | — | 2 |
| CP-21 | Eliminar pide confirmación; con ventas no elimina y ofrece suspender | `Propietario_EliminarRequiereConfirmacionYPreservaVentas` | RF-12 | — | 1 |
| CP-22 | Sin entradas elegidas, «Devolver» deshabilitado y gris | `Historial_SinSeleccionDistingueBotonDeshabilitado` | RF-05 | — | 1 |
| CP-23 | Tildar una entrada asegurada habilita «Devolver»; destildarla lo deshabilita | `Historial_TildarYDestildarActualizaEstadoYColor` | RF-05 | — | 1 |
| CP-24 | Evento agotado que recupera cupo: vuelve la grilla, solo servicios del evento, sin inscribir en la lista | `Compra_CupoLiberadoVuelveAHabilitarServiciosDisponibles` | RF-05, RF-10 | — | 1 |
| CP-25 | A 125 % y 150 % la compra entra en pantalla y las acciones quedan a la vista | `Compra_EscalaSimuladaMantieneAccionesDentroDelArea` | RF-05, RNF-03 | — | 3 |
| CP-26 | El alta nace Bronce y sin suscripción; un correo repetido se rechaza | `CP26_RegistroBronceSinSuscripcionYCorreoUnico` | RF-01 | — | 1 |
| CP-27 | Inicio de sesión correcto: devuelve el usuario sin la contraseña y abre la sesión | `CP27_InicioDeSesionCorrecto` | RF-02 | — | 1 |
| CP-28 | Cada intento fallido informa los restantes; al tope se bloquea y rechaza incluso la contraseña correcta; al vencer, entra y el contador vuelve a cero | `CP28_BloqueoTrasIntentosFallidos` | RF-02 | — | 1 |
| CP-29 | Correo inexistente o datos vacíos: no autentica ni abre sesión | `CP29_DatosFaltantesOCorreoInexistente` | RF-02 | — | 1 |
| CP-30 | Filtros por nombre, dirección, categoría y fecha; el evento pasado no aparece y rechaza compra y lista de espera; el suspendido no aparece | `CP30_FiltrosYEventoNoVigente` | RF-03 | — | 1 |
| CP-31 | Rechaza más entradas que el cupo, más que el máximo por operación y servicios sin entradas | `CP31_TopesDelPaquete` | RF-04, RF-05 | — | 1 |
| CP-32 | Pago aprobado: compra confirmada, QR únicos por entrada, pago registrado con los últimos 4 dígitos y el token, sin el número completo | `CP32_PagoAprobadoConfirmacionQrYRegistro` | RF-06, RF-07 | — | 1 |
| CP-33 | Pago rechazado: queda registrado, la compra no se confirma y el cupo vuelve para otro cliente | `CP33_PagoRechazadoLiberaCupoYQuedaRegistrado` | RF-06, RF-07 | — | 1 |
| CP-34 | Reserva vencida: rechaza el cobro, vence la compra y devuelve el cupo | `CP34_ReservaVencida` | RF-06 | — | 1 |
| CP-35 | Los reportes rechazan a quien no es Administrador | `CP35_SoloAdministradorAccede` | RF-11 | — | 1 |
| CP-36 | RPT-01 cuenta solo entradas confirmadas: cantidades, ingreso, ticket y ocupación | `CP36_VentasCuentaSoloEntradasConfirmadas` | RF-11 | — | 1 |
| CP-37 | RPT-05 calcula «Agotado» o «Con cupo» en el momento; una inscripción repetida se rechaza | `CP37_DemandaAgotadoYListaDeEspera` | RF-10, RF-11 | — | 1 |
| CP-38 | La exportación recibe los mismos datos que el reporte; período o código inválidos se rechazan | `CP38_ExportacionYPeriodoInvalido` | RF-11 | — | 1 |
| CP-39 | El temporizador de la reserva está a la vista y pasa a rojo en el último minuto | `CP39_TemporizadorDeReservaALaVista` | RF-06 | H-02 | 1 |
| CP-40 | La grilla muestra el cupo restante de cada sector | `CP40_CupoRestantePorSector` | RF-04 | H-03 | 1 |
| CP-41 | Comprar sin seguro pide confirmación, con No por defecto, y no reserva nada si se cancela | `CP41_ConfirmacionAlComprarSinSeguro` | RF-05 | H-04 | 1 |
| CP-42 | El enlace de niveles muestra los umbrales vigentes | `CP42_ExplicacionDeLosNiveles` | RF-08 | H-05 | 1 |
| CP-43 | El plazo de pago lo fija la reserva registrada, no el parámetro vigente: una reserva vencida no se cobra | `CP43_PlazoFijadoPorLaReservaPersistida` | RF-06 | — | 1 |
| CP-44 | El alta de usuario no ofrece elegir el perfil y crea un cliente | `CP44_AltaPublicaSinSelectorDePerfil` | RF-01 | — | 1 |
| CP-45 | El alta pública de negocio crea un cliente Bronce | `CP45_AltaPublicaDeNegocioCreaClienteBronce` | RF-01 | — | 1 |
| CP-46 | «Volver al inicio» cierra la sesión del cliente y del administrador | `CP46_VolverAlInicioCierraLaSesion` | RF-02 | — | 2 |
| CP-47 | La aplicación termina solo cuando el usuario cierra la última ventana visible | `CP47_CierreDeUltimaVentanaVisible` | RNF-02 | — | 4 |
| CP-48 | Cerrar la compra con un paquete armado pide confirmación, con No por defecto | `CP48_CerrarCompraConfirmaDescarte` | RF-05 | — | 1 |
| CP-49 | La compra arranca en la celda Cantidad, con encabezado y ayuda | `CP49_CantidadIndicadaYSeleccionada` | RF-04 | — | 1 |
| CP-50 | «Comprar» y el seguro se habilitan solo con al menos una entrada | `CP50_CompraYSeguroRequierenEntradas` | RF-05 | — | 1 |
| CP-51 | Ante un error de base de datos, uno de Negocio y uno genérico, el usuario recibe un mensaje en castellano sin detalle técnico | `CP51_MensajeDeErrorParaUsuario` | RNF-02 | — | 3 |
| CP-52 | Si el evento se elimina mientras el cliente paga, el pago se rechaza con aviso y no se cobra | `CP52_PagoDeEventoEliminado` | RF-06 | — | 1 |
| CP-53 | La aplicación instala el manejador de errores antes de abrir su primera pantalla | `CP53_ProgramInstalaManejadorGlobal` | RNF-02 | — | 1 |
| CP-54 | Los encabezados de la grilla de sectores se leen completos | `CP54_EncabezadosDeSectoresCompletos` | RF-04 | H-03 | 1 |
| CP-55 | Un perfil de usuario inexistente se rechaza sin crear la cuenta | `CP55_RegistroRechazaPerfilInvalido` | RF-01, RF-02 | — | 1 |
| CP-56 | No se elimina el último sector de un evento publicado | `CP56_RechazaEliminarUltimoSectorPublicado` | RF-12 | — | 1 |
| CP-57 | Un servicio de otro evento no entra al paquete | `CP57_RechazaServicioDeOtroEvento` | RF-05 | — | 1 |
| CP-58 | Un cliente con suscripción activa no contrata otra | `CP58_RechazaSegundaSuscripcionActiva` | RF-13 | — | 1 |
| CP-59 | La devolución sin seguro o fuera de plazo se rechaza sin reembolso | `CP59_RechazaDevolucionSinSeguroOFueraDePlazo` | RF-05 | — | 1 |
| CP-60 | Un sector que supera la capacidad del local no se agrega | `CP60_RechazaSectorQueSuperaCapacidadDelLocal` | RF-12 | — | 1 |
| CP-61 | En preventa, un cliente sin suscripción no inicia la compra | `CP61_RechazaPreventaSinSuscripcion` | RF-05, RF-13 | — | 1 |

**y, debajo, una segunda tabla** con el título «Casos de prueba incorporados en la versión 2.6
(CP-62 a CP-148)»:

| Caso | Condición de entrada | Resultado esperado | RF | Reglas |
|---|---|---|---|---|
| CP-62 | Los colores y las fuentes de `Tema` | Cada par de texto y fondo cumple el contraste WCAG (el foco sobre el primario, al menos 3:1); las familias tipográficas existen en el sistema | RNF-02 | — |
| CP-63 | El código de cada una de las 20 pantallas y de su Designer (20 variantes) | Ninguna declara colores, fuentes ni medidas propias: el estilo sale de `Tema` | RNF-02 | — |
| CP-64 | El proyecto `Visual` | El DPI se declara en el manifiesto, sin llamadas manuales en `Program` | RNF-03 | — |
| CP-65 | Ticket PDF de una entrada confirmada, generado en una carpeta temporal | El texto del PDF conserva evento, sector, número de pedido y el código QR completo | RF-07 | RN-18 |
| CP-66 | Abrir el login; iniciar sesión como Administrador (2 variantes) | El foco arranca en el correo; el Administrador llega al menú de propietario y queda en la sesión | RF-02 | RN-05 |
| CP-67 | Dos inicios de sesión seguidos del Administrador; un cliente con contraseña heredada en texto plano | Los dos entran; la contraseña heredada se acepta una vez y queda guardada como hash | RF-02 | — |
| CP-68 | Contraseñas incorrectas desde la pantalla de login, hasta el bloqueo; parámetros de bloqueo de 10 y 5 minutos (2 variantes) | Muestra los intentos restantes y bloquea al quinto fallo; el aviso dice los minutos del parámetro vigente | RF-02 | RN-04 |
| CP-69 | «Agregar evento» al 100 % y al 150 % (2 variantes) | El selector de categoría y las acciones quedan dentro de la ventana | RF-12, RNF-03 | RN-39 |
| CP-70 | Recorrer «Registrar local» con Tab; dar de alta un local (2 variantes) | El orden de Tab sigue los campos; el local queda en la base y aparece en «Mis locales» | RF-12 | — |
| CP-71 | «Mis locales» con dos locales; mover la celda actual al segundo (2 variantes) | La grilla aplica las propiedades y los colores del tema; se abren los eventos del segundo local | RF-12, RNF-02 | — |
| CP-72 | El botón de sectores; precio «12500,50» y «12500.50» (3 variantes) | El botón dice «Agregar sector»; los dos precios se guardan como 12.500,50 | RF-12 | — |
| CP-73 | Alta de un evento con categoría y sin categoría (2 variantes) | Con categoría, se guarda y se lista como borrador; sin categoría, no se inserta | RF-12 | RN-06 |
| CP-74 | Agregar a un borrador un sector de capacidad 0 y uno de capacidad 10 (2 variantes) | Con capacidad 0 sigue en borrador; el primer sector con capacidad publica el evento | RF-12 | RN-06 |
| CP-75 | Desde la pantalla, un sector que supera la capacidad del local; eliminar el último sector (2 variantes) | RN-38 informa la capacidad disponible y no inserta; RN-06 informa el código y conserva el sector | RF-12 | RN-06, RN-38 |
| CP-76 | «Mi suscripción» sin plan | Muestra precios, beneficios y controles completos, sin recortes | RF-13, RNF-02 | — |
| CP-77 | Contratar el plan mensual; cancelarlo (2 variantes) | Al contratar, muestra precio y vencimiento y habilita los beneficios. Al cancelar, pide confirmación con No por defecto y quita los beneficios | RF-13 | RN-25 |
| CP-78 | Dos ventanas de suscripción abiertas; contratar en las dos | La segunda se rechaza con RN-25: no se crean dos planes | RF-13 | RN-25 |
| CP-79 | Resumen de compra de 2 entradas con suscripción y sin ella (2 variantes) | El suscriptor tiene el seguro sin cargo y no recibe la pregunta del seguro; sin suscripción, el seguro de 2 entradas cuesta $10.000 | RF-05 | RN-37 |
| CP-80 | Pagar con seguro siendo suscriptor; pagar sin seguro (2 variantes) | Con seguro, una póliza gratuita por entrada; sin seguro, ninguna póliza | RF-06, RF-07 | RN-37 |
| CP-81 | Evento en preventa: cliente sin plan, suscriptor; evento sin preventa (3 variantes) | El evento se lista; al cliente sin plan se le rechaza con la fecha y hora en que termina la preventa; el suscriptor abre la compra; sin preventa compran todos. **Falla:** la preventa rechazada deja visible la compra | RF-03, RF-05 | RN-39 |
| CP-82 | Alta con preventa; preventa posterior al evento; administrar con preventa; quitarla; administrar sin preventa (5 variantes) | La fecha se guarda y se conserva al publicar; la posterior al evento se rechaza sin insertar; la pantalla muestra lo guardado; quitarla guarda NULL | RF-12 | RN-06, RN-39 |
| CP-83 | Lista de espera de un evento agotado; un propietario sin listas (2 variantes) | Columnas, prioridad y contadores a la vista; sin listas, el desplegable queda deshabilitado | RF-10 | RN-35 |
| CP-84 | Un suscriptor inscripto en la lista cancela su plan | Conserva la prioridad con que se inscribió | RF-10 | RN-35 |
| CP-85 | Menú del propietario al 100 % y al 150 %; menú del cliente; código de `Visual` (3 variantes) | Las acciones del propietario siguen visibles; el cliente no ve reportes; `Visual` no tiene conexiones ni comandos SQL. | RF-11, RF-12, RNF-02, RNF-03 | — |
| CP-86 | La semilla sola, sin datos de volumen | Los cinco reportes se generan y se exportan | RF-11 | — |
| CP-87 | Los filtros de cada reporte, RPT-01 a RPT-05 (5 variantes) | Valores iniciales, visibilidad y limpieza correctos en cada uno | RF-11 | — |
| CP-88 | Los datos de demostración del anexo A02 (8 variantes) | RPT-01 a RPT-05 reproducen las cifras del anexo y sus filtros; RPT-01 alinea los números, resalta el TOTAL y muestra la lectura | RF-11, RNF-02 | — |
| CP-89 | Exportar a una carpeta temporal; exportar sobre un archivo bloqueado (2 variantes) | Exporta y ofrece abrir el PDF; con el archivo bloqueado, informa el error y la pantalla sigue igual | RF-11 | — |
| CP-90 | PDF exportado desde la pantalla de reportes | El texto contiene la marca, el código, el período, quién lo pidió y la paginación | RF-11 | — |
| CP-91 | Sesión de un cliente que intenta devolver la compra de otro, con el titular del objeto cambiado a mano | Rechazado; tampoco ve las entradas. La compra sigue Confirmada y sin reembolso | RF-08 | RN-41 |
| CP-92 | Sesión de un cliente que pide el historial, la suscripción y el perfil de otro | Los tres, rechazados | RF-08 | RN-41 |
| CP-93 | Sesión de un cliente que elimina un evento | Rechazado con RN-34; el evento sigue existiendo | RF-12 | RN-34, RN-42 |
| CP-94 | Sin sesión: 16 operaciones protegidas y `CrearAdministrador`; después, `CrearAdministrador` como cliente y como Administrador | Las 17 se rechazan sin sesión, y el cliente tampoco puede. El Administrador crea la cuenta con perfil Propietario. El registro genérico no es público. La compra sigue Pendiente y sin suscripción | RF-04, RF-12, RF-13 | RN-41, RN-42 |
| CP-95 | Cambiar la contraseña con la actual incorrecta | Rechazado; el contador de intentos sube a 1 y la contraseña anterior sigue sirviendo | RF-02 | RN-04, RN-40 |
| CP-96 | Cambiar la contraseña de una cuenta bloqueada, con la actual correcta | Rechazado con el mensaje genérico «Mail o contraseña actual incorrectos.», sin decir que está bloqueada; al vencer el bloqueo, la anterior sigue sirviendo | RF-02 | RN-04 |
| CP-97 | Cambiar la contraseña con un correo inexistente y con la actual incorrecta | Las dos respuestas son iguales: misma regla y mismo mensaje | RF-02 | RN-40 |
| CP-98 | Con la sesión del Administrador, cambiar la contraseña de un cliente; abrir la pantalla | La anterior deja de servir y la nueva sirve; la sesión sigue siendo del Administrador. Pantalla: tres campos enmascarados y vacíos, todo dentro de la ventana | RF-02 | RN-40 |
| CP-99 | Entrada confirmada, del evento elegido y dentro del horario; después, un cliente que intenta registrarla y una consulta del contador sin sesión | Ingresa: `FechaIngreso` queda registrada, el estado de venta no cambia y el contador da 1. En la pantalla, el segundo escaneo dice «ya ingresó». Al cliente se le rechaza el registro por RN-34, sin alterar el ingreso; sin sesión, el contador se rechaza | RF-07 | RN-18, RN-34, RN-42 |
| CP-100 | El mismo código escaneado dos veces | El segundo se rechaza como ya usado, con la fecha y hora del primero | RF-07 | RN-18 |
| CP-101 | Entrada reembolsada | Se rechaza como reembolsada; el contador queda en 0 | RF-07 | RN-18, RN-29 |
| CP-102 | Código con formato válido que no existe | Se rechaza como inexistente; el contador queda en 0 | RF-07 | RN-18 |
| CP-103 | La entrada en otro evento; el evento dentro de 2 días y hace 2 días | Rechazada por otro evento y por fuera de horario; `FechaIngreso` sigue vacía | RF-07 | RN-18 |
| CP-104 | Diez puertas escanean el mismo código a la vez, arrancadas juntas por una barrera; cien rondas con un código nuevo cada una | En cada ronda, exactamente un ingreso válido y nueve «ya usado», comprobado en la base. Pasó sin cambiar el SQL de ingreso | RF-07 | RN-18 |
| CP-105 | Inicio de sesión real con hashes de 600.000 y 310.000 iteraciones (2 variantes) | Mide el tiempo. El hash de 600.000 no se reescribe con menos iteraciones. **Es una medición, no se cuenta como prueba de seguridad** | RF-02 | — |
| CP-106 | Buscar «[vip]», «50%» y «sector_unico» | Cada texto se busca literal: encuentra el evento, y «sector%unico» y «sector_unic_» no encuentran nada | RF-03 | — |
| CP-107 | Hashes con iteraciones enormes, 0, negativas o no numéricas, y con sal y hash vacíos | Ninguno autentica y no se propaga ninguna excepción | RF-02 | — |
| CP-108 | Hash previo de 100.000 iteraciones | Verifica; al iniciar sesión se reescribe a 310.000 con la misma contraseña, y un segundo inicio no lo vuelve a reescribir | RF-02 | — |
| CP-109 | Preparación completa de la base de pruebas | Carga 1.178 clientes de reporte con hashes a 310.000; la guarda `_Pruebas` sigue activa | RF-11 | — |
| CP-110 | Cada estado de `EstadoEntrada` y el estado ajeno «Usada», escritos en la tabla | La base admite los cuatro y rechaza el ajeno por `CK_Entradas_Estado`, que está activa y confiable | RF-07 | RN-18, RN-29 |
| CP-111 | Compra asegurada de 2 entradas: una ya ingresó; devolver las dos, en los dos órdenes (2 variantes) | RN-29 rechaza la devolución y no cambia nada, ni por el gestor ni por el repositorio. La entrada no usada se puede devolver sola | RF-07 | RN-18, RN-29 |
| CP-112 | Los siete componentes visuales en un host de diseño real (7 variantes) | Ninguno guarda su estilo en el Designer | RNF-01, RNF-02 | — |
| CP-113 | Contrato de la sesión: `Iniciar` fuera de lo público; propiedades de la sesión; perfil e identidad del usuario modificados después de iniciar (4 variantes) | `Iniciar` es `internal`; la sesión solo se lee; cambiar el objeto `Usuario` no eleva el perfil ni suplanta otra cuenta | RF-02 | RN-41, RN-42 |
| CP-114 | Inicio de sesión con correo existente e inexistente; quinto fallo; cuenta bloqueada (3 variantes) | El rechazo es idéntico en texto y título, sin revelar si la cuenta existe ni si está bloqueada; el bloqueo se registra igual | RF-02 | RN-04 |
| CP-115 | Descuento configurado de Plata y de Oro por debajo, igual y por encima de `DESCUENTO_MAX_ADMISIBLE` (6 variantes) | Por debajo o igual se aplica; por encima se rechaza con RN-20, sin recortarlo en silencio | RF-08 | RN-20 |
| CP-116 | El texto de cada botón de las 20 pantallas, dibujado por Windows, también con el borde de foco (20 variantes) | Ningún botón recorta su texto | RNF-02, RNF-03 | — |
| CP-117 | Consultar la posición en la lista de espera: la propia, con suscriptores adelante, la de otro cliente y sin sesión (4 variantes) | El cliente ve su posición, que respeta la prioridad de los suscriptores; la de otro y sin sesión se rechazan | RF-10 | RN-35, RN-41 |
| CP-118 | Aviso de disponibilidad: evento agotado, entrada liberada, avisos de otra cuenta, sin sesión, reserva que ocupa el último cupo, evento suspendido (6 variantes) | Solo hay aviso al iniciar sesión cuando de verdad se puede comprar; no se leen avisos ajenos ni sin sesión | RF-10 | RN-41 |
| CP-119 | Resumen de compra vacío y con dos entradas (2 variantes) | El TOTAL destacado coincide con el resumen, y el indicador marca el paso correcto | RF-05, RNF-02 | — |
| CP-120 | Los componentes nuevos `IndicadorPasos` y `EtiquetaTotal` en un host de diseño real (2 variantes) | Ninguno guarda su estilo en el Designer | RNF-02 | — |
| CP-121 | Suscripción contratada y vencida; consultarla y abrir su pantalla varias veces | No queda ninguna activa, y consultar o abrir la pantalla no crea ni renueva el registro | RF-13 | RN-26 |
| CP-122 | Compra confirmada sin seguro; volver a confirmarla agregándole seguro | Rechazo con «La compra ya no está pendiente de pago.»; la compra queda sin seguro, sin póliza y sin segundo pago | RF-06 | RN-28 |
| CP-123 | Eventos para el control de ingreso: de otro administrador, sin sesión, y un evento de las 00:30 en los bordes de su ventana (6 variantes) | No se listan, registran ni cuentan eventos ajenos; sin sesión, rechazo. El evento aparece desde el segundo exacto de apertura hasta el de cierre, y no un segundo antes ni después | RF-07 | RN-18, RN-41, RN-42 |
| CP-124 | Cambiar la contraseña sin sesión, con 0, 4 y 5 fallos previos, y con un correo inexistente (3 variantes) | El rechazo es el mismo en texto, título y comportamiento: no revela si la cuenta existe ni si está bloqueada. El bloqueo interno sigue funcionando | RF-02 | RN-04, RN-40 |
| CP-125 | Un sector vuelve a tener una entrada (devolución, reserva vencida o pago rechazado) y hay un suscriptor inscripto en la lista de espera; compran un no suscriptor, el suscriptor inscripto, un suscriptor sin inscripción y un reintento de pago anterior, dentro y fuera de la ventana (8 variantes) | Dentro de la ventana solo compra el suscriptor inscripto; los demás reciben el rechazo RN-35 sin consumir el cupo ni dejar compra o pago nuevo. Fuera de la ventana compra cualquiera | RF-10 | RN-35, RN-37 |
| CP-126 | Pedir la lista de espera de un evento como cliente y sin sesión | Rechazo RN-34 sin devolver nombres ni correos; el administrador la obtiene | RF-10 | RN-34 |
| CP-127 | Pedir los pagos de una compra ajena y sin sesión | Rechazo RN-41 sin devolver el dato; el titular los obtiene | RF-06 | RN-41 |
| CP-128 | Pedir por su número una compra de otro cliente, y sin sesión | Rechazo RN-41 sin devolver la compra | RF-07 | RN-41 |
| CP-129 | Pedir las pólizas de seguro de otro cliente, y sin sesión | Rechazo RN-41 sin devolver las pólizas | RF-06 | RN-41 |
| CP-130 | Iniciar sesión usando como contraseña el hash guardado de una cuenta ya migrada a PBKDF2 | No autentica ni se toma como contraseña para migrar | RF-02 | RN-04 |
| CP-131 | Registrar el ingreso de una entrada de un evento suspendido, dentro del horario de ingreso | Rechazo; la entrada no queda ingresada | RF-07 | RN-18 |
| CP-132 | Registrar en la base el ingreso de una entrada contra un evento distinto del suyo, sin el filtro previo del gestor | La base no registra el ingreso | RF-07 | RN-18 |
| CP-133 | La entrada se devuelve entre la lectura y la escritura del ingreso | La base no registra el ingreso de una entrada que ya no está confirmada | RF-07 | RN-18, RN-29 |
| CP-134 | Registrar el ingreso de una entrada pendiente y de una vencida | Cada una recibe su rechazo específico y ninguna queda ingresada | RF-07 | RN-18 |
| CP-135 | Registrar ingresos en el segundo exacto de apertura y de cierre de la ventana, y un segundo antes y después | Los dos bordes se aceptan; los segundos de afuera se rechazan | RF-07 | RN-18 |
| CP-136 | Leer un código de entrada con espacios alrededor | El código se normaliza y el ingreso queda registrado | RF-07 | RN-18 |
| CP-137 | Consultar si otro cliente está inscripto en una lista de espera, y sin sesión | Rechazo RN-41 sin devolver el dato | RF-10 | RN-41 |
| CP-138 | Contar las compras de fidelización de otro cliente, y sin sesión | Rechazo RN-41 sin devolver el dato | RF-08 | RN-41 |
| CP-139 | Calcular el descuento de otro cliente, y sin sesión | Rechazo RN-41 sin devolver el dato | RF-08 | RN-41 |
| CP-140 | Armar un paquete a nombre de otro cliente, y sin sesión | Rechazo RN-41; no se crea el paquete | RF-05 | RN-41 |
| CP-141 | Consultar los locales de un propietario como cliente, y sin sesión | Rechazo RN-34 sin devolver el dato | RF-12 | RN-34 |
| CP-142 | Listar los eventos con ingreso abierto como cliente, y sin sesión | Rechazo RN-34 sin devolver el listado | RF-07 | RN-34 |
| CP-143 | Ejecutar un reporte ya creado después de cerrar la sesión del administrador o como cliente | Rechazo RN-34 sin devolver el reporte | RF-11 | RN-34 |
| CP-144 | Pedir el catálogo de eventos de un reporte como cliente, y sin sesión | Rechazo RN-34 sin devolver el catálogo | RF-11 | RN-34 |
| CP-145 | Insertar dos entradas con el mismo código QR | El índice único de la base rechaza la segunda y la reversión deja intacta la primera | RF-07 | RN-18 |
| CP-146 | Registrar el ingreso de una entrada como cliente y sin sesión (2 variantes) | El rechazo es un resultado, no una excepción: motivo «no autorizado», regla RN-34 y el mensaje «Esta operación es solo para el Administrador». La entrada sigue confirmada y sin ingreso, el contador da 0 y la sesión no cambia | RF-07 | RN-34 |
| CP-147 | Insertar en la base un segundo seguro para una entrada que ya tiene uno, y uno para otra entrada (2 variantes) | El índice único de la base rechaza el segundo seguro de la misma entrada; la otra entrada admite el suyo. La reversión deja todo como estaba | RF-06 | RN-28 |
| CP-148 | Comprar, pagar y cerrar la pantalla de confirmación (interfaz) | Vuelve al listado de eventos, sin mostrar un armado vacío, y la sesión sigue siendo del cliente | RF-06 | — |

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**Metodología de verificación**

Las secciones anteriores informan qué se probó y cuánto cubren las pruebas. Esta sección explica
cómo se construyeron las pruebas y, sobre todo, cómo se comprobó que sirven: que detectarían un
defecto si lo hubiera. Una prueba que pasa dice que el sistema se comporta como se espera hoy;
no dice que la prueba se daría cuenta si mañana dejara de hacerlo.

**La batería de pruebas**

Las pruebas son automatizadas y de integración. Cada caso prepara sus propios datos, ejecuta la
operación a través de los mismos componentes de negocio que usa la aplicación y verifica el
resultado contra una base de datos SQL Server real. Esa base se crea de cero al comienzo de cada
corrida, con el mismo esquema y los mismos datos iniciales que se entregan, y nunca es la base de
desarrollo. Las pruebas de interfaz abren los formularios reales de la aplicación, los operan y
leen lo que muestran.

La batería tiene ciento cuarenta y ocho casos de prueba. Algunos se ejecutan varias veces con datos
distintos, por ejemplo una pantalla a dos escalas de visualización, y en total suman doscientas
setenta y seis ejecuciones, todas correctas en la última corrida. Como cada corrida recrea la
base, los resultados no dependen de lo que haya quedado de una corrida anterior.

**Cobertura de las reglas de negocio**

Una regla se considera cubierta cuando al menos un caso provoca la situación que la regla regula
y verifica el resultado. Si la regla rechaza una operación, el caso intenta la operación prohibida
y comprueba dos cosas: que el sistema la rechaza por esa regla y no por otra, y que los datos
quedan exactamente como estaban.

El criterio se aplicó de manera estricta. Un caso que solo muestra en pantalla el texto de una
regla no la cubre. Por ejemplo, que la pantalla de suscripción diga «No se renueva
automáticamente» no prueba que la suscripción vencida no se renueve. Dos reglas estuvieron en esa
situación y no se contaron como cubiertas hasta que tuvieron un caso que verificaba el
comportamiento: la de no renovación automática (RN-26) y la que impide agregar el seguro después
de la compra (RN-28).

Con ese criterio, treinta y nueve de las cuarenta y una reglas vigentes tienen caso automatizado,
un 95,1 %. Las dos restantes no rechazan ninguna operación: RN-07 describe un cálculo y RN-33
fija la moneda.

**El límite de la cobertura**

La cobertura cuenta qué reglas tienen una prueba, pero no mide si esa prueba detectaría un
defecto. Un ejemplo de este mismo proyecto lo muestra. Una prueba verificaba que, sin una sesión
iniciada, el sistema rechaza un conjunto de operaciones protegidas, y aceptaba cualquier rechazo
como correcto. Si a una de esas operaciones se le quitaba el control de sesión, el sistema igual la
rechazaba, pero por otra regla: por ejemplo, porque el evento no estaba agotado. La prueba seguía
pasando y la defensa faltaba. Para encontrar casos así se usó la prueba de mutación.

**La prueba de mutación**

La prueba de mutación consiste en introducir a propósito un defecto en el código y comprobar si
las pruebas lo notan. Cada defecto introducido se llama mutación. Se aplica una mutación por vez,
se ejecuta la batería completa, y se observa el resultado:

- si al menos una prueba que antes pasaba ahora falla, la mutación fue **detectada**: la batería
  habría avisado de ese defecto;
- si todas las pruebas siguen pasando, la mutación **sobrevivió**: ese defecto podría introducirse
  sin que ninguna prueba lo advierta.

El porcentaje de mutaciones detectadas no mide cuántas pruebas hay, sino cuántos defectos
encontrarían. Por eso se aplicó a las defensas de seguridad, donde un defecto no se ve en el uso
normal: una operación que deja de verificar quién la pide sigue funcionando igual para el usuario
legítimo.

*Cómo se hizo.* Antes de ejecutar nada, se leyó el código y se listaron las ochenta y tres
defensas del sistema:
- los controles de sesión y de perfil de cada operación;
- la verificación y el almacenamiento de las contraseñas;
- el bloqueo por intentos fallidos;
- las verificaciones del control de ingreso por código QR;
- las restricciones de la base de datos.

Para cada una se escribió cómo romperla y un pronóstico de si la batería lo detectaría, también
por lectura. Después se ejecutó cada mutación sobre una copia aislada del código y con una base
de datos propia; antes de cada ejecución se verificaba que la conexión apuntara a esa base y no a
otra. Las tres mutaciones que afectan lecturas simultáneas, cuyo resultado puede variar entre una
ejecución y otra, se ejecutaron además veinte veces cada una. Al terminar se restauró el código
y la batería volvió a dar el mismo resultado que antes de empezar.

*Dos mediciones, sobre dos versiones del código.* La mutación se midió dos veces. La primera,
sobre una versión intermedia, mostró qué defensas no tenían una prueba capaz de notar su falta.
Con ese resultado se escribieron las pruebas que faltaban, y la segunda medición se hizo sobre el
código congelado para medirla. Las ochenta y tres mutaciones son las mismas en las dos. Entre una versión
y otra, el código cambió en algunos de los lugares que esas mutaciones tocan: setenta y nueve se
aplicaron sin ningún cambio, y en cuatro se ajustó solo la forma de ubicar la línea por modificar,
porque había pasado a otro método o aparecía dos veces. En las cuatro se rompe la misma defensa
que en la primera medición. Por eso las dos mediciones son comparables: miden las mismas defensas,
rotas de la misma manera.

*Resultado.* En la primera medición, la batería detectó cincuenta y una de las setenta y siete
mutaciones que se podían detectar: un 66,2 %. El pronóstico hecho por lectura acertó en todas las
que se esperaba que sobrevivieran: las veintitrés sobrevivieron. En la medición final, sobre el
código congelado (commit `65225e7`), la batería detectó setenta y tres de setenta y siete:
un 94,8 %. Para descartar que una prueba fallara por un motivo ajeno a la mutación, por ejemplo una
ventana de otra aplicación que tomara el foco durante una prueba de interfaz, cada detección se
comparó con la prueba que se esperaba que la detectara: las setenta y tres coinciden.

*Mutaciones equivalentes.* Algunas mutaciones no cambian el comportamiento del sistema, porque
otra defensa del mismo camino ya rechaza la operación. Por ejemplo, si una operación verifica la
sesión al principio y vuelve a verificarla enseguida, quitar la primera verificación no cambia
nada que una prueba pueda observar. Esas mutaciones se llaman equivalentes. Ninguna prueba podría
detectarlas, y por eso se excluyen del cálculo.

Antes de ejecutar, por lectura, se identificaron seis equivalentes, y se excluyeron. Al ejecutar
aparecieron dos más, una en cada medición. La primera es la mutación que quita el control de
sesión al contratar una suscripción: la operación consulta enseguida la suscripción activa del
usuario, y esa consulta repite el mismo control. La segunda es la que deja de ocultar, al cambiar
la contraseña, si la cuenta existe: la operación transforma cualquier fallo en el mismo mensaje
genérico, así que la diferencia no llega al usuario. En los dos casos el pronóstico decía que
serían detectadas; sobrevivieron, y al revisar el código resultaron equivalentes.

**Esas dos mutaciones se dejaron dentro del cálculo, como no detectadas, a propósito.** La razón es
de método. Si una mutación se declara equivalente después de ver que sobrevivió, el criterio deja
de ser independiente del resultado. Con el mismo argumento se podría excusar cualquier
sobreviviente incómoda, y el porcentaje dejaría de medir la batería para medir la habilidad de
justificarla. Las seis equivalentes se definieron antes de conocer los resultados; las otras dos, no.
Excluirlas habría dado un 97,3 %; se informa el 94,8 %.

*Qué significa que una defensa sobreviva.* No significa que el sistema sea vulnerable: la defensa
está en el código y funciona, como se verificó al leerlo. Significa que ninguna prueba fallaría si
esa defensa se quitara por error en una versión futura. Cada defensa sobreviviente se registró
como una prueba por escribir, con lo que debe verificar, y se ordenaron por la consecuencia que
tendría su falta: primero las que protegen los datos personales y de pago de otros clientes,
después el acceso sin contraseña y el control de ingreso, y al final las que solo exponen datos
operativos. Esas pruebas se escribieron después: veinte casos nuevos y el refuerzo de uno
existente. Cada uno se comprobó quitando su defensa, con lo que la prueba debía fallar, y
restaurándola, con lo que debía volver a pasar.

En la medición final quedaron cuatro mutaciones sin detectar. Dos son las equivalentes descubiertas
después de ejecutar. La tercera quita la comparación de contraseñas en tiempo constante: una
prueba funcional no puede observar cuánto tarda una comparación, y esa defensa se verificó leyendo
el código. La cuarta quita el control de perfil al registrar el ingreso de una entrada, y es un caso como el
que se describió en «El límite de la cobertura». La prueba de ese registro aceptaba cualquier
rechazo: sin el control, el sistema igual rechazaba al cliente, pero por otra regla, la que impide
controlar el ingreso a eventos ajenos, y sin sesión la operación terminaba en un error no
controlado. Con el control en su lugar el rechazo era el correcto, pero ninguna prueba lo exigía.
Lo encontró la prueba de mutación, y se corrigió después de la medición: el rechazo pasó a ser un
resultado que nombra su regla, y un caso nuevo lo exige, con un cliente y sin sesión, y falla si el
control se quita. Por eso la cifra de setenta y tres de setenta y siete corresponde al commit
medido, `65225e7`, y no a la versión que se entrega, `dba674c`. Es también el mejor argumento a
favor del método: la mutación encontró un hueco que ninguna de las pruebas existentes advertía.

*Un caso que muestra para qué sirve.* El control de ingreso debe admitir una sola vez cada
entrada aunque dos puertas la lean en el mismo instante (RN-18). La prueba original usaba dos
lectores simultáneos. Ejecutada sola, detectaba casi siempre las mutaciones que rompían esa
garantía, diecinueve y dieciocho veces de cada veinte. En la ejecución de la batería completa no
detectó ninguna de las dos, así que no se contaron como detectadas. La prueba se reforzó: diez
lectores arrancados juntos sobre la misma entrada, cien veces seguidas, con un ingreso válido y
nueve rechazos exigidos en cada vuelta. La prueba reforzada detectó las tres mutaciones de
concurrencia en todas las ejecuciones de control. El código de ingreso no se modificó: el sistema ya era correcto, y lo que faltaba era
una prueba capaz de demostrarlo.

*Lo que la revisión de coherencia no alcanzó.* Además de las pruebas, el documento se revisó con
una matriz de diez comprobaciones de coherencia entre los diagramas, la base de datos y el código.
Una de ellas compara los estados de la entrada en cuatro lugares: el diagrama de estados, la
clase, la restricción de la base y el código. Las cuatro listas coincidían. Pero la comprobación
compara listas de estados, no transiciones, y no puede ver de qué estado sale cada flecha. Así
quedó sin detectar que el diagrama hacía salir la devolución de la entrada confirmada en general,
lo que permitía devolver una entrada que ya se había usado para ingresar, cosa que el código
rechaza. No lo encontró el instrumento: lo encontró una revisión externa, hecha por alguien
distinto de quien había dibujado y verificado los diagramas, y se corrigió. Para que el
instrumento lo detectara habría hecho falta una comprobación más: comparar cada transición del
diagrama con la condición que la permite en el código, como se hizo con los métodos del diagrama
de secuencia. La conclusión es la misma que la de la prueba de mutación: también los instrumentos
de verificación tienen que verificarse, y saber qué no cubre cada uno es parte del resultado.

*Qué queda fuera de la medición.* La mutación se aplicó a la capa de negocio, a la de datos y al
esquema de la base, que es donde están las defensas; no a la interfaz gráfica, cuyos controles
ocultos o deshabilitados no son una defensa. Tampoco cubre el acceso directo a la base de datos
sin pasar por la aplicación, ni el tiempo de respuesta de la comparación de contraseñas, que una
prueba funcional no puede observar y se verificó leyendo el código.

«Mantenimiento de la matriz» y el párrafo final de la página no cambian.

## Página 110 — Conclusiones

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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
