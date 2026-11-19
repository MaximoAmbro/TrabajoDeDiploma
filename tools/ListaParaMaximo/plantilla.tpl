# LA lista para el Word — de la página 1 a la 112

> ## ⚠ ANTES DE TODO: nueve scripts nuevos en tu base (18/11 y 19/11)
>
> **El ejecutable de hoy no anda sin ellos.** Si ya corriste del `08` al `15` siguiendo una versión
> anterior de esta lista, solo te falta el `16` (paso 10); de paso, volvé a correr el `08`, que
> ahora corrige a qué regla apuntan dos parámetros del código por correo (no cambia ningún valor). En SQL Server Management Studio, elegí tu base de
> EventAura y ejecutá los archivos de la carpeta `BaseDatos/` **en este orden, de a uno**. Todos
> se pueden ejecutar más de una vez sin dañar nada.
>
> 0. **Si todavía no corriste del `04` al `07`** (los del 16/11), corrélos primero, en orden:
>    `04-seguridad-ingreso`, `05-check-estado-entradas`, `06-prioridad-temporal`,
>    `07-indice-unico-seguros`. Si ya los corriste, seguí desde el 1.
> 1. **`08-seguridad-cuenta-auditoria.sql`**: verificación de correo, códigos de recuperación de
>    contraseña, tabla de auditoría y los parámetros de cierre por inactividad y fortaleza de la
>    contraseña. Las cuentas que ya tenés quedan con el correo verificado.
> 2. **`09-imagenes-eventos.sql`**: agrega la imagen de portada de los eventos.
> 3. **`10-restringir-cantidad-vendida.sql`**: cada entrada vale una unidad del cupo. Muestra
>    cuántas filas corrigió; lo normal es **0**.
> 4. **Antes del `11`, comprobá esto.** Ejecutá:
>
>    ```sql
>    SELECT ID, TipoUsuario FROM Usuario WHERE Mail = 'admin@eventaura.com';
>    ```
>
>    - **Devuelve una fila:** seguí con el paso 5.
>    - **No devuelve nada:** **pará acá.** No corras del `11` al `16`: el `11` se detendría con el
>      error 51101. Ejecutá esto y mandale el resultado a JC:
>
>      ```sql
>      SELECT ID, Mail, TipoUsuario FROM Usuario WHERE TipoUsuario <> 'Cliente';
>      ```
>
> 5. **`11-perfiles-administrador-organizador.sql`**: separa los perfiles en Cliente, Organizador
>    y Administrador. Crea la cuenta `organizador@eventaura.com` (contraseña `Cliente123`) y le
>    pasa **todos los locales** que eran de `admin@eventaura.com`. Si se detiene con el error
>    51102, hay una cuenta con un perfil desconocido: pará y avisale a JC.
> 6. **`12-solicitudes-organizador.sql`**: agrega la solicitud para que un cliente pase a ser
>    organizador, con una solicitud ficticia de ejemplo a nombre de `bronce@eventaura.com`.
> 7. **`13-revision-primer-evento.sql`**: agrega el estado «en revisión» de los eventos.
> 8. **`14-primer-evento-aprobado.sql`**: marca como ya aprobados a los organizadores que tienen
>    eventos publicados, así que `organizador@eventaura.com` publica directo, sin revisión.
> 9. **`15-motivo-rechazo-evento.sql`**: guarda el motivo cuando el Administrador rechaza el primer
>    evento de un organizador, para que el organizador lo vea.
> 10. **`16-fk-local-propietario.sql`**: hace que cada local tenga que pertenecer a un usuario que
>    exista. Si se detiene con el error 51161, hay locales sin un dueño válido: pará y avisale a
>    JC; no borres nada.
>
> **⚠ Lo que cambia y no es un error:** desde el `11`, **`admin@eventaura.com` ya no tiene
> locales.** Entra a un menú de administrador de la plataforma (solicitudes, eventos en
> revisión, reportes, auditoría). Los locales y eventos están ahora en
> **`organizador@eventaura.com` / `Cliente123`**.
>
> **Cómo comprobar que quedó bien:**
>
> 1. Ejecutá:
>
>    ```sql
>    SELECT u.Mail, u.TipoUsuario, COUNT(l.ID) AS Locales
>    FROM Usuario u LEFT JOIN Locales l ON l.PropietarioID = u.ID
>    WHERE u.Mail IN ('admin@eventaura.com', 'organizador@eventaura.com')
>    GROUP BY u.Mail, u.TipoUsuario;
>    ```
>
>    Tiene que dar dos filas: `admin@…` con `Administrador` y **0** locales, y `organizador@…`
>    con `Organizador` y **1 o más** locales. Después ejecutá:
>
>    ```sql
>    SELECT COL_LENGTH('Propietario', 'PrimerEventoAprobado') AS Script14;
>    ```
>
>    Tiene que dar un número, no `NULL`.
> 2. Abrí el ejecutable. Si falta alguno de los scripts del `04` al `16`, al iniciar te dice
>    cuál.
> 3. Entrá con `organizador@eventaura.com` / `Cliente123`: tenés que ver tus locales y eventos.

> ## ✅ HOY SOLO SE PUEDE PEGAR: introducción, planteo del problema, objetivos y justificación (pp. 9 a 11), los requerimientos y las reglas (pp. 23 y 25 a 27), los parámetros (p. 33), la priorización y la estimación de costos (pp. 35, 36 y 38), el borrado de la primera copia de minutas y prototipos (pp. 39 y 40), la descripción funcional (p. 40), las minutas (p. 44), «Arquitectura e implementación» (pp. 57 a 61), las especificaciones CU-09 a CU-21 (después de la p. 85), todos los diagramas con sus textos (pp. 71 y 86 a 89).
>
> **Todo lo demás está en pausa** (⏸ en su lugar): el sistema se está ampliando y cambian los
> requerimientos, las reglas, los casos de uso, las pruebas, la arquitectura, las cifras y todas
> las figuras. Dentro de esas páginas que sí se pegan siguen esperando los datos marcados
> «⏸ ESPERA A JC». **Si ya pegaste algo de la zona en pausa, dejalo como está:** cuando se libere,
> «Cambió después del 16/11» va a decir qué volver a pegar.

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
> | pp. 25 a 32 | Reglas de negocio, factibilidad económica y niveles | Reglas nuevas desde RN-43 |
> | pp. 35 a 38 | Priorización, orden de implementación y la sección nueva «Estimación de costos» | Requerimientos nuevos: la tabla de costos suma los puntos de cada requerimiento por fase |
> | pp. 40 a 43 | Descripción funcional, alcance y remisión a los requerimientos | Funciones nuevas |
> | p. 45 | Prototipos | Son figuras de la interfaz |
> | pp. 47 a 55 | Guion de interfaz, instructivo y sus figuras | Pantallas nuevas (de 20 a unas 24) e identidad visual nueva |
> | pp. 62 a 70 | Entidades, código y pruebas | Clases, tablas y pruebas nuevas |
> | pp. 72 a 85 | Especificaciones de casos de uso (los diagramas, pp. 71 y 86 a 89, ya se pegan) | Casos de uso nuevos |
> | pp. 90 a 97 | Usabilidad: el párrafo sobre el rediseño | La interfaz vuelve a cambiar |
> | pp. 98 a 105 | Anexo A02 | Depende de las pantallas y de las consultas |
> | pp. 106 a 109 | Matriz de trazabilidad, cobertura y brechas | Requerimientos, reglas y casos nuevos |
> | Sección nueva «Metodología de verificación» | Batería y cifras | Casos y ejecuciones nuevos |
> | pp. 110 y 111 | Conclusiones, limitaciones y trabajo futuro | Por ejemplo, «no existe recuperación de contraseña» deja de ser cierto |
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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-08 · p\. 18 :: ^\*\*B-09
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-18 · p\. 18 :: ^## 4\. Decisiones

## Página 20 — Criterios de aceptación

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-07 · p\. 20 :: ^\*\*B-08

**Criterio 10. Reemplazar la línea completa por:**

> 10. Lista de espera para eventos agotados: El sistema debe registrar al usuario en la lista,
> permitirle consultar su posición y avisarle al iniciar sesión cuando el evento vuelve a tener
> entradas.

## Página 21 — Supuestos del proyecto

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran los requerimientos y las reglas de la versión 2.6.

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

**Después del requerimiento 13 y antes de la «Nota sobre los requerimientos RF-12 y RF-13», agregar**
(mismo formato que los anteriores: número y nombre, descripción, criterio de aceptación):

> 14. Verificación del correo
> Confirmar, con un código de un solo uso enviado al correo, que la cuenta de Cliente pertenece a
> quien se registra, antes de permitirle comprar.
> Criterio de aceptación: El sistema no debe permitir iniciar una compra sin el correo
> verificado. El código vence, admite un número limitado de intentos, no sirve dos veces y no se
> muestra en ninguna pantalla de la aplicación.
>
> 15. Recuperación de la contraseña
> Permitir restablecer una contraseña olvidada con un código de un solo uso enviado al correo de
> la cuenta.
> Criterio de aceptación: El sistema debe responder lo mismo exista o no la cuenta, validar la
> contraseña nueva con las mismas reglas que el registro, anular el código anterior al emitir uno
> nuevo y registrar en la auditoría cada restablecimiento, exitoso o rechazado.
>
> 16. Registro de auditoría
> Registrar las operaciones relevantes con el usuario, la acción, la entidad afectada, la fecha y
> el resultado.
> Criterio de aceptación: El Administrador de la plataforma debe poder consultar el registro
> filtrado por período y por acción, y ningún otro perfil debe poder consultarlo.
>
> 17. Protección de la sesión y de la contraseña
> Cerrar una sesión sin actividad después de un plazo configurable, con aviso previo, y exigir
> una fortaleza mínima a toda contraseña nueva.
> Criterio de aceptación: La aplicación debe avisar antes del cierre y cerrar la sesión al vencer
> el plazo. Debe rechazar una contraseña que no alcance el puntaje mínimo y mostrar la fortaleza
> mientras se escribe.
>
> 18. Mapa de sectores
> Mostrar los sectores de un evento en un mapa orientativo, con su precio y su disponibilidad.
> Criterio de aceptación: Elegir un sector en el mapa debe agregarlo al armado del paquete por el
> mismo camino que la tabla, y un sector agotado no debe poder elegirse.
>
> 19. Portadas de los eventos
> Permitir al Organizador cargar una imagen de portada para cada evento y mostrarla en el catálogo.
> Criterio de aceptación: La imagen debe reducirse y guardarse; un archivo que no es una imagen, o
> que supera el tamaño máximo, debe rechazarse; un evento sin portada debe mostrar una generada.
>
> 20. Solicitud para ser Organizador
> Permitir a un Cliente solicitar ser Organizador, con los datos del responsable, del local y de
> la cuenta bancaria, y al Administrador de la plataforma aprobar o rechazar la solicitud.
> Criterio de aceptación: La solicitud debe exigir un teléfono confirmado y validar el CUIT y el
> CBU. La decisión del Administrador debe requerir un motivo; aprobarla debe cambiar el perfil a
> Organizador y crear el local.
>
> 21. Publicación explícita y revisión del primer evento
> Publicar un evento solo cuando el Organizador lo decide, y enviar a revisión el primer evento de
> un Organizador nuevo.
> Criterio de aceptación: El sistema no debe publicar un evento sin un sector con capacidad. El
> primer evento debe quedar en revisión hasta que un Administrador lo apruebe o lo rechace con un
> motivo; un evento rechazado vuelve a borrador, con el motivo a la vista del Organizador.
>
> 22. Tres perfiles de usuario
> Distinguir al Cliente, al Organizador y al Administrador de la plataforma, cada uno con su menú
> y sus permisos.
> Criterio de aceptación: Cada perfil debe acceder solo a sus operaciones; el Organizador debe
> operar solo sobre sus locales, y solo un Administrador debe poder crear otro Administrador.

**Después de la «Nota sobre los requerimientos RF-12 y RF-13», agregar** con el mismo formato:

> Nota sobre los requerimientos RF-14 a RF-22. Se incorporaron en la versión 2.6, durante la
> construcción del prototipo, con sus reglas (RN-43 a RN-54) y sus casos de uso (CU-14 a CU-21).
> Sus puntos no se estimaron: se midieron después de construirlos (ver «Priorización de
> requerimientos funcionales» y «Estimación de costos»).


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

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran los requerimientos y las reglas de la versión 2.6.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-01 · p\. 25 :: ^\*\*A-02

**Misma oración, a continuación:** «las especificaciones de casos de uso (CU-01 a CU-08)» pasa a
«las especificaciones de casos de uso (CU-01 a CU-21)».

**Tabla «Usuarios y cuentas» (en la p. 25 o en la 26: es la que tiene RN-01 a RN-05).
Reemplazar** la fila de RN-05 por:

| RN-05 | Un usuario tiene un único perfil excluyente: Cliente, Organizador o Administrador de plataforma. El administrador de plataforma no es dueño de locales; el organizador administra solo los locales, eventos e ingresos de su cuenta. | E | RF-22 / CU-05 |
|---|---|---|---|

**Agregar** después de RN-05 (si ya habías pegado RN-40, RN-41 y RN-42 de una versión anterior
de esta lista, reemplazalas por estas):

| RN-40 | Para cambiar una contraseña se debe comprobar la contraseña actual. El rechazo no revela si el correo existe, si la contraseña es incorrecta o si la cuenta está bloqueada; los intentos y el bloqueo se aplican internamente. | A | RF-02 / CU-09 |
|---|---|---|---|
| RN-41 | Las operaciones sobre datos privados de una cuenta solo pueden devolverlos o modificarlos para el usuario autenticado titular de esa cuenta. La sesión conserva una identidad verificada y no acepta como autorización un objeto de usuario alterado por quien llama. | A | RF-02 / CU-05 |
| RN-42 | **Dada de baja.** La autorización por perfil para gestión e ingreso fue reemplazada por RN-48; la autorización por pertenencia a los datos del Organizador, incluidos sectores, listas de espera e ingresos, está en RN-49. Los reportes se rigen por RN-34. El número no se reasigna. | — | — |
| RN-43 | Una cuenta de Cliente debe verificar el correo antes de iniciar una compra. El código de verificación se envía al correo registrado; la entrega usa la bandeja simulada del sistema. | A | RF-14 / CU-04, CU-14 |
| RN-44 | Los códigos de verificación de correo, recuperación de contraseña y confirmación telefónica son de un solo uso, se almacenan protegidos, vencen según `CORREO_CODIGO_MIN` y admiten como máximo `CORREO_CODIGO_INTENTOS_MAX` intentos. La recuperación cambia la contraseña mediante las mismas validaciones y el mismo hash que el registro. | A | RF-14, RF-15 / CU-14 a CU-16 |
| RN-45 | Las operaciones de inicio de sesión, cambio de contraseña, compras, devoluciones, control de ingreso y gestión de eventos se registran en `Auditoria` con el usuario, la acción, la entidad afectada, la fecha y el resultado. Solo el Administrador de plataforma puede consultar esos registros, con filtros por período y acción. | A | RF-16 / CU-20 |
| RN-46 | Una sesión sin actividad se cierra al cumplirse `SESION_INACTIVIDAD_MIN`. La aplicación avisa con `SESION_AVISO_SEG` segundos de anticipación; cualquier actividad reinicia el plazo. | A | RF-17 / CU-05 |
| RN-47 | Una contraseña nueva debe cumplir la longitud y composición mínima de RN-02 y alcanzar el puntaje configurado en `PWD_FORTALEZA_MIN`, calculado por la variedad de minúsculas, mayúsculas, números y símbolos. | A | RF-17 / CU-04, CU-09, CU-15 |

*Razón:* texto de `docs/02-reglas-negocio.md` (Codex), sin cambios; la cuarta columna usa la
numeración RF-14 a RF-22 y CU-14 a CU-21 aprobada por JC el 19/11.

## Página 26 — Reglas de negocio

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran los requerimientos y las reglas de la versión 2.6.

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

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran los requerimientos y las reglas de la versión 2.6.

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

**Tabla de reportes y listas de espera (la que tiene RN-34). Reemplazar** la fila de RN-34 por:

| RN-34 | El administrador de plataforma accede a la administración global y a los reportes globales; los organizadores acceden a reportes limitados a eventos de sus propios locales. | A | RF-11 / CU-08 |
|---|---|---|---|

**Misma tabla, agregar al final:**

| RN-48 | La gestión de locales, eventos y control de ingreso requiere perfil Organizador. | A | RF-22 / CU-02, CU-10 a CU-12 |
|---|---|---|---|
| RN-49 | Un Organizador solo puede consultar o modificar locales, eventos, sectores, ingresos y listas de espera pertenecientes a sus locales. | A | RF-22 / CU-02, CU-07, CU-10 a CU-12 |

**Tabla nueva, a continuación, con el mismo formato y el título «Solicitudes de Organizador y
revisión del primer evento»:**

| RN-50 | Solo una cuenta Cliente puede presentar una solicitud para ser Organizador y no puede mantener más de una solicitud pendiente. La solicitud pendiente no cambia el perfil ni concede permisos de Organizador. | A | RF-20 / CU-16 |
|---|---|---|---|
| RN-51 | Antes de presentar la solicitud, el responsable debe ingresar un teléfono argentino válido y confirmarlo mediante un código de un solo uso enviado por el mecanismo de códigos del sistema. | A | RF-20 / CU-16 |
| RN-52 | La solicitud valida el dígito verificador del CUIT y del CBU; también admite un alias bancario con el formato definido. La documentación debe ser una imagen PNG o JPEG válida, comprimida y de hasta 512 KB. La verificación externa de identidad es simulada y no sustituye la revisión del Administrador. | A | RF-20 / CU-16 |
| RN-53 | Solo el Administrador de plataforma puede aprobar o rechazar solicitudes de Organizador. Ambas decisiones requieren un motivo de hasta 200 caracteres y registran la identidad del revisor y la fecha; aprobar cambia el perfil a Organizador y crea el local solicitado. | A | RF-20 / CU-18 |
| RN-54 | La primera publicación de un Organizador se envía a revisión del Administrador y no aparece en el catálogo ni admite ventas mientras está en revisión. El Administrador puede aprobarla o rechazarla; para rechazar debe ingresar un motivo no vacío de hasta 500 caracteres. El rechazo devuelve el evento a Borrador y conserva el motivo para el Organizador. Si vuelve a publicarlo antes de que se haya aprobado un evento suyo, vuelve a revisión. Tras su primera aprobación, sus publicaciones siguientes son directas. | A | RF-21 / CU-17, CU-19 |

*Razón:* texto de `docs/02-reglas-negocio.md` (Codex), sin cambios. De RN-01 a RN-54 quedan 52
reglas vigentes: RN-27 y RN-42 están dadas de baja y sus números no se reasignan.

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

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-09 · p\. 32 :: ^\*\*A-10

## Página 33 — Parámetros y conflictos

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambió con los tres perfiles y las funciones de seguridad.

**Corrección de la tarde del 18/11:** las filas «Vigencia del código enviado por correo» e «Intentos para usar un código enviado por correo» citan **RN-44**, no RN-43. Si ya las pegaste con RN-43, cambiá solo ese número en las dos filas.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-10 · p\. 33 :: ^\*\*A-11

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*A-11 · p\. 33 :: ^## 3\. Requerimientos y alcance

## Página 35 — Priorización

**✅ Se pega hoy (19/11).** Incluye la sección nueva «Estimación de costos», con la ampliación de la versión 2.6 medida y aprobada por JC. Si ya la habías pegado antes, volvé a pegarla entera: cambió.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-04 · p\. 35 :: ^\*\*B-05

**Sección nueva, debajo de «Escala de esfuerzo»** (después del párrafo que termina «…no por
estimación absoluta.»). Pegar completo:

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*Texto para pegar:\*\*$ :: ^\*\*Cuentas verificadas:\*\*

## Página 36 — Priorización de requerimientos funcionales

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran los requerimientos y las reglas de la versión 2.6.

**Tabla «Priorización de requerimientos funcionales». Agregar** al final, con el mismo formato,
estas nueve filas:

| RF-14 | Verificación del correo | Must | 1 | Hace cumplir RN-43: sin el correo verificado no se compra. |
|---|---|---|---|---|
| RF-15 | Recuperación de la contraseña | Must | 2 | Hace cumplir RN-44: devuelve el acceso a quien olvidó la contraseña, sin conocer la actual. |
| RF-16 | Registro de auditoría | Must | 1 | Hace cumplir RN-45: deja evidencia de las operaciones sensibles. |
| RF-17 | Protección de la sesión y de la contraseña | Must | 1 | Hace cumplir RN-46 y RN-47. |
| RF-18 | Mapa de sectores | Should | 1 | Mejora la elección del sector; la tabla ya permitía elegirlo. |
| RF-19 | Portadas de los eventos | Should | 1 | Mejora la presentación del catálogo; no agrega una operación. |
| RF-20 | Solicitud para ser Organizador | Must | 3 | Hace cumplir RN-50 a RN-53: ninguna cuenta se vuelve Organizador sin revisión. |
| RF-21 | Publicación explícita y revisión del primer evento | Must | 2 | Hace cumplir RN-06 y RN-54: un evento llega al catálogo solo por decisión del Organizador y, la primera vez, después de una revisión. |
| RF-22 | Tres perfiles de usuario | Must | 1 | Hace cumplir RN-05, RN-48 y RN-49: separa la administración de la plataforma de la de los locales. |

**Nota al pie de esa tabla, sobre RF-14 a RF-22:**

> Los puntos de RF-14 a RF-22 no se estimaron: se midieron después de construir cada
> requerimiento, contando las líneas de código y de pruebas que agregó, a razón de unas 250 líneas
> por punto. Esa relación es la mediana de cuatro requerimientos estimados (RF-01, RF-10, RF-11 y
> RF-13) medidos con la misma vara; varía entre 129 y 268 líneas por punto. Se redondea al entero
> más cercano, con un mínimo de un punto. Criterio de la categoría: lo que hace cumplir una regla
> o cierra un riesgo de seguridad es Must; el resto de lo construido es Should.

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

**Debajo de la nota anterior, agregar:**

> **Versión 2.6.** Los nueve requerimientos agregados suman 13 puntos medidos: 11 son Must
> (84,6 %: RF-14 a RF-17 y RF-20 a RF-22) y 2 son Should (15,4 %: RF-18 y RF-19). No se suman a
> la distribución anterior, que es la de la estimación.

## Página 37

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran los requerimientos y las reglas de la versión 2.6.

**Fila de la Fase 8:**
- en la columna Requerimientos, «RF-09 y RF-10, según margen disponible» pasa a «RF-10 (RF-09
  pasó a Won't have en la versión 2.5)»;
- en la columna Pts, «11» pasa a «3».

**Tabla, agregar al final** la fila:

| Fase 9 — Ampliación de la versión 2.6 | Must y Should | RF-14 a RF-22 | 13 |
|---|---|---|---|

**Debajo de la tabla, antes de «Riesgo de concentración», agregar:**

> Los puntos de la Fase 9 se midieron después de construirla (ver «Estimación de costos»). El
> rediseño de la interfaz (8 puntos) y la revisión de pertenencia (1 punto) no son
> requerimientos funcionales y no figuran en esta tabla.

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

**✅ Se pega hoy (19/11).** Si ya la habías pegado antes, volvé a pegarla: entran la solicitud de Organizador, la publicación y la revisión, y la seguridad de la cuenta.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-09 · p\. 40 :: ^> Si se elige la opción \(b\) de D-6

## Página 41 — Alcance y restricciones

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-10 · p\. 41 :: ^> Si se aplica la opción \(b\) de D-6
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-12 · p\. 41 :: ^\*\*B-13
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*B-13 · p\. 41 :: ^\*\*B-14

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

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-01 · p\. 48 :: ^\*\*E-02

## Página 49

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-02 · p\. 49 :: ^\*\*E-03

## Página 50 — Instructivo: estado de implementación

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^## 8\. Página 50 :: ^## 9\. Páginas 52

## Página 51

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-03 · p\. 51 :: ^\*\*E-04
@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-04 · p\. 51 :: ^\*\*E-05

## Página 52

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-05 · p\. 52 :: ^\*\*E-06

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

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambió con los tres perfiles y las funciones de seguridad. **Cambio del 19/11:** en «Patrones de diseño», el párrafo de **Strategy** cambió en su segunda mitad (el envío de los códigos). Si ya pegaste esta sección, reemplazá solo ese párrafo.

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^## Anexo 1 — Texto para D-1 :: ^\*Razón:\* código\. Las 4 capas

## Página 62 — Entidades

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*E-08 · p\. 62 :: ^\*Razón:\* base\. `BaseDatos/01-esquema\.sql` y `GestorCompras\.Iniciar`

## Páginas 63 a 67 — Código real (⚠ releé el aviso de arriba antes de empezar)

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^## Anexo 2 — Textos para D-2 :: ^\*\*pp\. 68 a 70, código de las pruebas

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

@@- docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*pp\. 68 a 70, código de las pruebas :: ^## Anexo 3

## Página 71 — Diagramas de casos de uso (cuatro figuras)

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambió con los tres perfiles.

**Reemplazar** la figura que está bajo «Diagramas de caso de uso» por cuatro, una por página,
cada una con su subtítulo:

| Subtítulo | Archivo (`docs/diagramas/`) | Ancho |
|---|---|---|
| «Cuenta» | `casos-uso-cuenta.png` | **13,8 cm** |
| «Cliente» | `casos-uso-cliente.png` | **16,0 cm** |
| «Organizador» | `casos-uso-organizador.png` | **16,0 cm** |
| «Administrador» | `casos-uso-administrador.png` | **16,0 cm** |

Si ya habías pegado las dos figuras anteriores (cliente y administrador), reemplazalas: cambiaron
las dos.

Siempre con la proporción bloqueada (clic derecho › *Tamaño y posición* › *Bloquear relación de
aspecto*). **Si la achicás, deja de leerse.**

**Texto, debajo del título:**

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 1\. Casos de uso >> ^\*\*Texto para el documento :: ^\*\*Justificación de relaciones:\*\*

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

**Campo «Actor»** (tres perfiles desde el 18/11):

| Caso de uso | Dice | Poner |
|---|---|---|
| CU-02 – Gestionar eventos | «Administrador» | «Organizador» |
| CU-07 – Consultar listas de espera | «Administrador» | «Organizador» |
| CU-08 – Generar reportes | «Administrador» | «Organizador (reportes de sus eventos) o Administrador (reportes de todos los eventos)» |

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

## Después de la página 85 — Especificaciones nuevas CU-09 a CU-21

**✅ Se pega hoy (19/11).** Las trece especificaciones están completas: CU-14 y CU-15 describen la verificación y la recuperación con el código fuera de la aplicación.

**Insertar** a continuación de CU-08, con el mismo formato que CU-01 a CU-08 (los mismos estilos
de título y de campos). No llevan diagrama chico.

@@ docs/AUDITORIA-DOCUMENTO-CODIGO.md :: ^\*\*CU-09 – Cambiar contraseña\*\*$ :: ^\*\*Lo que no se especifica y por qué\.\*\*

## Página 86 — Modelo de dominio conceptual y diagrama de clases

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambiaron las figuras y el texto con los tres perfiles.

**Modelo de dominio conceptual:** reemplazar la figura por `docs/diagramas/dominio-conceptual.png`,
a **16,0 cm** (si ya la habías pegado, reemplazala: cambió). Texto debajo del título:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 7\. Modelo de dominio conceptual >> ^\*\*Texto para el documento :: ^\*\*Justificación de relaciones:\*\*

**Diagrama de clases:** reemplazar la figura por diez figuras, **una por página**, cada una con
su subtítulo. Si ya habías pegado las siete anteriores, reemplazalas todas: cambiaron. **Si ya
pegaste las nueve del 18/11:** agregá la 6b después de la 6, y volvé a pegar el texto de abajo
(cambió el párrafo de la capa de negocio). **Si ya pegaste la 5b, la 6 o la 6b:** reemplazalas;
desde el 19/11 a la tarde muestran la verificación propia de la consulta de la auditoría (RN-45).

| Subtítulo | Imagen (`docs/diagramas/`) | Ancho |
|---|---|---|
| 1. Dominio: cuentas y catálogo | `clases-1-dominio-catalogo.png` | **14,9 cm** (página entera) |
| 1b. Dominio: suscripción, lista de espera y solicitud de Organizador | `clases-1b-dominio-cuenta.png` | **11,4 cm** |
| 2. Dominio: compra y pago | `clases-2-dominio-compra.png` | **16,0 cm** |
| 3. Dominio: entrada y seguro | `clases-3-dominio-entrada.png` | **16,0 cm** |
| 4. Negocio: gestores de la compra | `clases-4-negocio-compra.png` | **16,0 cm** |
| 5. Negocio: administración | `clases-5-negocio-administracion.png` | **16,0 cm** |
| 5b. Negocio: cuenta y seguridad | `clases-5b-negocio-cuenta-seguridad.png` | **16,0 cm** |
| 6. Negocio: verificación de identidad, perfil y pertenencia | `clases-6-autorizacion.png` | **16,0 cm** |
| 6b. Negocio: escritura de la auditoría | `clases-6b-auditoria.png` | **16,0 cm** |
| 7. Datos: repositorios | `clases-7-datos.png` | **14,3 cm** (página entera) |

Texto, debajo del título «Diagrama de clases»:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 2\. Diagrama de clases >> ^\*\*Texto para el documento :: ^\*\*Justificación de relaciones, vistas 1 a 3

**Sección nueva «Patrones de diseño»**, a continuación de las vistas de clases. Cuatro figuras:

| Subtítulo | Imagen (`docs/diagramas/`) | Ancho |
|---|---|---|
| Singleton y Repository | `clases-8a-patron-singleton-repository.png` | **16,0 cm** |
| Strategy: formato de los reportes | `clases-8b-patron-strategy.png` | **15,5 cm** |
| Jerarquía de Reporte | `clases-8c-jerarquia-reportes.png` | **16,0 cm** (es baja: puede compartir página con el texto) |
| Strategy: envío de códigos y verificación de identidad | `clases-8d-patron-strategy-cuenta.png` | **13,9 cm** (página entera) |

**Si ya pegaste la figura de Strategy de códigos del 18/11, reemplazala** (19/11): cambió un signo de visibilidad. En el texto de abajo cambió una oración del segundo párrafo de Strategy («…que escribe el mensaje en un archivo de correo fuera de la aplicación…»).

Texto, debajo del título:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 3\. Patrones >> ^\*\*Texto para el documento :: ^(\*\*Justificación|\| Vista \| Relación)

## Página 87 — Secuencia y estados

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambiaron figuras y texto con los tres perfiles.

**Diagrama de secuencia – Comprar paquete:** reemplazar la figura por tres, una por página:
`secuencia-compra-1-reserva.png`, `secuencia-compra-2-pago.png` y
`secuencia-compra-3-emision.png`, las tres a **16,0 cm**. Texto debajo del título:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 4\. Secuencia >> ^\*\*Texto para el documento :: ^\*\*Correspondencia con el código

**Diagrama de estados – Ciclo de vida de la entrada:** reemplazar la figura por
`estados-entrada.png`, a **16,0 cm** (ocupa la página entera: el título pasa a la página nueva).
Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 5\. Transición >> ^\*\*Texto para el documento :: ^(\*\*|## 6\.)

## Página 88 — Robustez, dominio actualizado y actividad

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambiaron figuras y texto con los tres perfiles.

**Diagrama de robustez (ICONIX):** reemplazar la figura por `robustez-comprar-paquete.png`, a
**16,0 cm**. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 9\. Robustez >> ^\*\*Texto para el documento :: ^\*\*Justificación de conexiones:\*\*

**Diagrama de dominio actualizado:** reemplazar la figura por tres, una por página:
`dominio-actualizado-1-cuentas-catalogo.png` y `dominio-actualizado-2-compra.png` a **16,0 cm**,
y `dominio-actualizado-3-organizacion.png` a **11,4 cm** (página entera). Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 8\. Modelo de dominio actualizado >> ^\*\*Texto para el documento :: ^\*\*Justificación

**Agregar al final del texto anterior:**

> Una entrada tiene a lo sumo un seguro. La base de datos lo garantiza con un índice único sobre
> la entrada asegurada, de modo que un mismo cliente no puede recibir dos cargos de seguro por la
> misma entrada (RN-28).

*Razón:* matriz de coherencia, comprobaciones 4 y 6. El script `07-indice-unico-seguros.sql`
agrega el índice; la vista 3 de clases y el entidad-relación de compras ya muestran 0..1.

**Diagrama de actividad:** el título «Diagrama de Actividad (flujo del Cliente)» pasa a
«Diagrama de actividad (proceso de venta)». Reemplazar la figura (que sigue en la p. 89) por
cuatro, una por página, en este orden: `actividad-0-publicacion.png` a **10,4 cm**,
`actividad-1-armado.png` a **14,2 cm**, `actividad-2-pago.png` a **13,3 cm** y
`actividad-3-ingreso.png` a **16,0 cm**. Si ya habías pegado las tres anteriores, reemplazalas:
cambiaron. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 10\. Actividad >> ^\*\*Texto para el documento :: ^(\*\*|## )

## Página 89 — Entidad-relación

**✅ Se pega hoy (18/11).** Si ya la habías pegado antes, volvé a pegarla: cambió con los tres perfiles. **Si la pegaste el 18/11 o el 19/11 a la mañana:** reemplazá solo la figura 1 y el texto de abajo; el dueño de un local ahora tiene clave foránea (script 16) y la línea Usuario–Locales pasó a ser continua.

Debajo de «DER – Diagrama entidad-relación» (hoy la página tiene solo el título), insertar
cuatro figuras, una por página, en este orden:

| Vista | Archivo (`docs/diagramas/`) | Ancho |
|---|---|---|
| 1. Usuarios y catálogo | `er-1-usuarios-catalogo.png` | **16,0 cm** (página entera) |
| 2. Compra, pago y entradas | `er-2-compras.png` | **14,6 cm** (página entera) |
| 3. Suscripciones, lista de espera y parámetros | `er-3-suscripciones-espera.png` | **12,7 cm** |
| 4. Seguridad de la cuenta, auditoría y organizadores | `er-4-seguridad-organizadores.png` | **16,0 cm** |

Si ya habías pegado las dos figuras anteriores, reemplazalas: cambiaron las dos. Texto:

@@- docs/PARA-MAXIMO-DIAGRAMAS.md :: ^## 6\. Entidad-relación >> ^\*\*Texto para el documento :: ^(\*\*|## )

## Páginas 90 a 97 — Informe de pruebas de usabilidad

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

| Dice | Poner |
|---|---|
| «CU-08 – Ver reportes de ventas define el flujo de interacción» | «CU-08 – Generar reportes define el flujo de interacción» |

*Razón:* el título de CU-08 cambió (pp. 84 y 85).

## Páginas 99 a 105 — Anexo A02

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

@@- docs/CORRECCIONES-DOCUMENTO.md :: ^## 12\. Página 99 :: ^## 17\. Página 106

## Después de la página 105 — Anexo nuevo «Pantallas del sistema»

**⏸ NO PEGAR TODAVÍA (19/11): faltan siete de las 28 pantallas.** Las de Organizador y Administrador ya están con los datos de la semilla; faltan cuatro del Cliente que todavía muestran datos de las pruebas, las dos con el indicador de fortaleza cortado y la recuperación de la contraseña. Cuando estén, el anexo se pega entero de una vez. Va como anexo nuevo, después del anexo A02 y antes de la matriz de trazabilidad, empezando en página nueva.

**El mapa de sectores (figura P7.2) va solo, en una página apaisada:** insertá un salto de sección «Página siguiente» antes y otro después, y poné en horizontal solo esa sección (*Disposición › Orientación › Horizontal*). Así el resto del anexo sigue vertical.

@@- docs/ANEXO-PANTALLAS-SISTEMA.md :: ^## Texto para pegar :: ^## Fin del texto para pegar

## Página 106 — Matriz: criterio y nomenclatura

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Criterio de construcción. Reemplazar:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y seis
> reglas, los ocho casos de uso y los diez casos de prueba resulta ilegible en formato A4

**Por:**

> Una grilla que cruce los seis objetivos con los veintidós requerimientos, las cincuenta y dos
> reglas, los veintiún casos de uso y los doscientos cuatro casos de prueba resulta ilegible en
> formato A4

**Tabla «Nomenclatura de los ejes».** Reemplazar las filas de reglas y de casos de prueba, y en
las de requerimientos funcionales y casos de uso cambiar la cantidad:

| Fila | Dice | Poner |
|---|---|---|
| Requerimientos funcionales | la cantidad «13» | «22» |
| Reglas de negocio | «RN-xx \| 36 \| Reglas de negocio (RN-27 dada de baja)» | «RN-xx \| 52 \| Reglas de negocio (RN-27 y RN-42 dadas de baja)» |
| Casos de prueba | «CP-xx \| 10 \| Desarrollo de la lógica de descuento por nivel» | «CP-xx \| 204 \| CP-01 a CP-10: Desarrollo de la lógica de descuento por nivel. CP-11 a CP-205: Matriz de trazabilidad, catálogo de casos incorporados (el número CP-149 no se usó)» |
| Casos de uso | la cantidad «8» | «21» |

## Página 107 — Matriz principal

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.
La tabla ya está armada con RF-14 a RF-22 (19/11), pero Codex está agregando una prueba más y la
columna de casos se vuelve a generar cuando la commitee.

**Reemplazar la tabla completa por:**

| RF | OE | MoSCoW | Reglas de negocio | CU | Casos de prueba | Hall. |
|---|---|---|---|---|---|---|
| RF-01 | OE-2 | Must | RN-01, RN-02, RN-03 | CU-04 | CP-14, CP-15, CP-26, CP-44, CP-45, CP-55 | — |
| RF-02 | OE-2 | Must | RN-04, RN-40, RN-41 | CU-05, CU-09 | CP-27 a CP-29, CP-46, CP-47, CP-51, CP-53, CP-66 a CP-68, CP-105, CP-107, CP-108, CP-130, CP-154 a CP-157 | — |
| RF-03 | OE-2 | Must | RN-09 | CU-01 | CP-16 a CP-18, CP-30, CP-81, CP-106, CP-160, CP-199 | — |
| RF-04 | OE-2 | Must | RN-08, RN-12, RN-33 | CU-01 | CP-20, CP-31, CP-40, CP-49, CP-54, CP-94, CP-151 | H-03 |
| RF-05 | OE-2 | Must | RN-13, RN-14, RN-28, RN-29 | CU-01, CU-13 | CP-05, CP-11, CP-13, CP-19, CP-22 a CP-25, CP-41, CP-48, CP-50, CP-57, CP-59, CP-61 a CP-64, CP-79, CP-81, CP-112, CP-116, CP-119, CP-120, CP-140, CP-152, CP-153, CP-159, CP-163 | H-04 |
| RF-06 | OE-5 | Must | RN-15, RN-16, RN-30, RN-31, RN-32 | CU-01 | CP-07, CP-32 a CP-34, CP-39, CP-43, CP-52, CP-80, CP-122, CP-127, CP-129, CP-147, CP-148, CP-181 | H-02 |
| RF-07 | OE-2 | Must | RN-17, RN-18 | CU-01, CU-12 | CP-32, CP-33, CP-65, CP-80, CP-99 a CP-104, CP-110, CP-111, CP-123, CP-128, CP-131 a CP-136, CP-142, CP-145, CP-146, CP-161 | — |
| RF-08 | OE-3 | Should | RN-19 a RN-24 | CU-01, CU-03 | CP-01 a CP-06, CP-08 a CP-11, CP-42, CP-91, CP-92, CP-115, CP-138, CP-139 | H-01, H-05 |
| RF-09 | — | Won't | — | — | — | — |
| RF-10 | OE-6 ind. | Could | RN-10, RN-35, RN-36 | CU-07 | CP-24, CP-37, CP-83, CP-84, CP-117, CP-118, CP-125, CP-126, CP-137 | — |
| RF-11 | OE-6 | Should | RN-34 | CU-08 | CP-35 a CP-38, CP-85 a CP-90, CP-109, CP-143, CP-144, CP-162, CP-182 | — |
| RF-12 | OE-2 | Must | RN-06, RN-07, RN-11, RN-38 | CU-02, CU-10, CU-11 | CP-12, CP-21, CP-56, CP-60, CP-69 a CP-75, CP-82, CP-85, CP-93, CP-94, CP-141, CP-164, CP-197 | — |
| RF-13 | OE-3 | Should | RN-25, RN-26, RN-37, RN-39 | CU-06 | CP-10, CP-58, CP-76 a CP-78, CP-94, CP-121 | — |
| RF-14 | OE-2 | Must | RN-43, RN-44 | CU-04, CU-14 | CP-167, CP-168, CP-171, CP-203 | — |
| RF-15 | OE-2 | Must | RN-44 | CU-15 | CP-169 a CP-171, CP-173, CP-176, CP-200, CP-201 | — |
| RF-16 | OE-2 | Must | RN-45 | CU-20 | CP-174, CP-175, CP-189, CP-196 | — |
| RF-17 | OE-2 | Must | RN-46, RN-47 | CU-04, CU-05, CU-09, CU-15 | CP-95 a CP-98, CP-113, CP-114, CP-124, CP-170, CP-172, CP-174 | — |
| RF-18 | OE-2 | Should | — | CU-01 | CP-150, CP-151, CP-160 | — |
| RF-19 | OE-2 | Should | — | CU-02 | CP-158, CP-160, CP-177 a CP-180 | — |
| RF-20 | OE-2 | Must | RN-50 a RN-53 | CU-16, CU-18 | CP-184 a CP-196, CP-202 | — |
| RF-21 | OE-2 | Must | RN-06, RN-54 | CU-17, CU-19 | CP-198, CP-199, CP-204, CP-205 | — |
| RF-22 | OE-2 | Must | RN-05, RN-48, RN-49 | CU-02, CU-07, CU-10 a CU-12, CU-21 | CP-85, CP-141, CP-142, CP-161, CP-162, CP-165, CP-166, CP-182 a CP-184, CP-187, CP-189, CP-190, CP-193, CP-198 | — |

*Cómo se armó:* la columna «Casos de prueba» sale de los comentarios de trazabilidad de las
pruebas en `2ffca70` (`py -3 -X utf8 tools/Matriz/Extraer.py`); las reglas y los casos de uso, de
la columna «RF / CU» de la tabla de reglas (pp. 25 a 27). RF-14 a RF-22 van con OE-2, como los
demás requerimientos de la operación de la plataforma (RF-01 a RF-05, RF-07 y RF-12): ninguno
responde a la fidelización (OE-3), al pago (OE-5) ni a la información de gestión (OE-6).
**Para que decida JC.**

El párrafo «La indicación "ind." en RF-10…» queda igual. **Agregar** debajo:

> La categoría Won't de RF-09 corresponde a la reclasificación de la versión 2.5 (brecha
> G-03). RF-14 a RF-22 se incorporaron en la versión 2.6. Los requerimientos no funcionales no
> figuran en la matriz porque no originan casos de uso; sus casos de prueba son: RNF-02,
> CP-47, CP-62, CP-63, CP-71, CP-76, CP-85, CP-88, CP-112, CP-116, CP-119, CP-120, CP-152, CP-153, CP-157 y CP-159; RNF-03, CP-12, CP-25, CP-64, CP-69, CP-85, CP-116 y CP-163; RNF-04, CP-51, CP-53, CP-156, CP-164 y CP-197. Un caso figura en más de una fila cuando prueba una
> regla de una fila desde la operación de otra. RN-41 es una regla de autorización: vale para
> todas las operaciones del sistema y no para un requerimiento solo, y se ubica en RF-02, que
> establece la sesión de la que depende. RN-48 y RN-49, que reservan al Organizador la gestión de
> sus locales, se ubican en RF-22.

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

**⏸ NO PEGAR TODAVÍA: página en pausa, depende del código.** Saltéala. Si ya la pegaste, dejala como está.

**Oración inicial. Reemplazar:**

> El análisis identificó cuatro brechas. La primera fue resuelta a partir de este mismo
> análisis; las tres restantes quedan documentadas con su acción correctiva.

**Por:**

> El análisis identificó ocho brechas. G-01 se resolvió a partir de este mismo análisis.
> En la versión 2.5 se cerró G-03 y se resolvieron G-04 y G-05; en la 2.6, G-07. G-02 quedó resuelta en el
> nivel de requerimientos y abierta en el de reglas, y G-08, resuelta en el código y abierta en
> las pruebas. G-06 quedó resuelta en nueve de sus catorce
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
| G-08 | Al separar los perfiles en la versión 2.6, la revisión de pertenencia encontró veinticuatro métodos de la capa de negocio que permitían leer u operar sobre datos de otro dueño, en cinco familias (locales, eventos y sectores, servicios y disponibilidad, listas de espera, y reportes y consulta de la auditoría), y una operación que permitía falsificar el autor de un registro de auditoría. | Alta | Resuelta en el código; abierta en las pruebas | RN-48 y RN-49 cierran los veinticuatro métodos, y la escritura de la auditoría pasó a ser interna. Las lecturas tienen prueba negativa entre organizadores, comprobada quitando la verificación. Faltan los casos negativos de las escrituras de eventos, sectores y locales, de la exportación y de los reportes distintos del primero, de la consulta de la auditoría como Organizador y del intento de falsificar el autor (ver «La revisión de pertenencia» en «Metodología de verificación»). |

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
| El envío de los códigos de verificación y de recuperación es simulado: cada mensaje se escribe en un archivo de correo de la misma computadora, fuera de la aplicación. Por eso no demuestra que quien lee el código sea el titular del correo o del teléfono. | Es una decisión de configuración, no un hueco de arquitectura: el envío está implementado con el patrón Strategy, y pasar a correo real es elegir la estrategia `EnviadorSmtp`, que ya existe, y configurar el servidor, sin cambiar la capa de negocio. La aplicación no puede abrir ni leer ese archivo, y todo restablecimiento, exitoso o rechazado, queda en el registro de auditoría con la cuenta y la fecha. Ese registro no es inalterable para quien accede directamente a la base, porque se guarda en la misma base y con las mismas credenciales que el resto de los datos. |
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
   «Conclusiones»), el anexo «Pantallas del sistema» (después del anexo A02), los títulos nuevos de CU-06, CU-07 y CU-08, y que ya no estén
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
