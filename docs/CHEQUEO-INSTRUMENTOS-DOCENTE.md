# El documento pasado por los instrumentos del docente

Claude Code, 13/11/2026, a pedido de JC. Solo lectura.

**Supuesto de partida:** ya se aplicaron
- `CORRECCIONES-DOCUMENTO.md`;
- `AUDITORIA-DOCUMENTO-CODIGO.md`;
- los diagramas de `PARA-MAXIMO-DIAGRAMAS.md`.

Lo que sigue es lo que **todavía** perdería puntos.

**Clases:** **R** = reemplazo de texto · **N** = número a regenerar · **D** = decisión. Igual que
en la auditoría.

## 0. Qué instrumentos había

| Instrumento | Dónde está | Estado |
|---|---|---|
| **Estructura oficial** | La «Guía de resolución» de las consignas de la Entrega 1 a la 3 (`Documentacion/Entrega 1.pdf` a `Entrega 3.pdf`; las tres son iguales salvo el encabezado). Son 13 puntos, más tres formalidades: portada, resumen ejecutivo e índice | Corrida. **Da 16 ítems, no 15.** Si la estructura de 15 que tenés en mente es otro archivo, pasámelo |
| **16 errores frecuentes** | Tu mensaje del 12/11 (la auditoría de diagramas) | Corrida |
| **Matriz de coherencia de 10 comprobaciones** | Sección 6 de `Prompt_Documentacion_Trabajo_de_Diploma.md`, adjunto de WhatsApp del 11/11 (fuera del repositorio) | **Corrida el 16/11:** `docs/MATRIZ-COHERENCIA.md`. Pasan 8 de 10 (dos con explicación en el texto); la 10 espera la devolución del Parcial 1 |

Además se miraron los «Criterios de resolución» de la misma consigna, porque dicen cómo pondera.

## 1. La guía oficial, punto por punto

| # | Punto de la guía | Pasa | Qué falta exactamente | Clase |
|---|---|---|---|---|
| F-1 | Portada | **No, hasta tener los datos** | Comisión, turno, legajos y año. Ítem 1 de CORRECCIONES, con el formato de datos de su final | D (datos de JC) |
| F-2 | Resumen ejecutivo | Sí | Con las cifras de la sección 5 de la auditoría | — |
| F-3 | Índice | Sí, al final | Regenerarlo después de todo lo demás (ítem 22 de CORRECCIONES) | N |
| 1 | Nombre del proyecto | Sí | — | — |
| 2 | Siglas | Sí: «EventAura (EVAU)», p. 9 | — | — |
| 3 | Descripción, menos de 200 palabras | Sí, con B-01 | Unas 130 palabras. El «cuándo» depende del año que fije JC | D (año) |
| 4 | Objetivos medibles; los específicos, **sin enunciar funciones del sistema** | **No** | OE-3, OE-5 y OE-6 son funciones del sistema. Además, OE-3 y OE-5 contradicen al código. Ver I-1 | R |
| 5 | Alcance sin temas técnicos, con inclusiones y exclusiones | Sí, con B-10 | B-10 se rehízo hoy en ese formato | — |
| 6 | Registro de interesados (nombre, organización, localización, rol, expectativas, influencia, fase, clasificación) | **No** | Falta Juan Cruz Riggio. Es la decisión 1 del final de CORRECCIONES; el texto está en I-2 | R |
| 7 | Cronograma de hitos | **No, hasta tener las fechas** | «Fecha estimada: x», nueve veces (ítem 24 de CORRECCIONES) | D (datos de JC) |
| 8 | Criterios de aceptación | Sí | Con B-07, B-08 y B-18 | — |
| 9 | Supuestos | Sí | Con B-14 y B-15 | — |
| 10 | Restricciones | Sí | Con B-12 y B-13 | — |
| 11 | Definición de requerimientos: minutas, prototipos y especificación | **Casi** | Las minutas son simuladas por diseño. Quedan dos cosas: la frase de Figma (decisión 5 de CORRECCIONES) y la «pantalla de carrito», que no existe. Ver I-3. La copia repetida de las pp. 42 a 43 depende de D-3 | R + D |
| 12.a | Diagrama de actividad | Sí | Diagramas nuevos | — |
| 12.b | Transición de estados, **con las reglas en notas sobre cada transición** | Sí | El diagrama nuevo lleva la regla en cada transición (ficha 5 de PARA-MAXIMO) | — |
| 12.1 | Especificación del requerimiento core | Sí | — | — |
| 12.2 | Guion de interfaz | Sí | Con E-01 y E-02 | — |
| 12.3.1.1 | Diagramas de casos de uso, con include y extend | Sí | Diagramas nuevos | — |
| 12.3.1.2 | **Especificación de caso de uso (detallada)** | **Sí, con el Anexo 3 de la auditoría** (14/11) | Las cinco especificaciones que faltaban (CU-09 a CU-13) están escritas desde el código. **Queda abierto:** los ocho diagramas chicos de CU-01 a CU-08 siguen siendo los viejos (ver la sección 4, D-7) | R |
| 12.3.2.1 | Dominio conceptual | Sí | — | — |
| 12.4.1.1 | Robustez (opcional) | Sí | La del caso principal | — |
| 12.4.1.2 | Dominio actualizado, con atributos, relaciones y multiplicidad | Sí | — | — |
| 12.5.1 | Secuencias, **explicitando la arquitectura** | Sí | La secuencia nueva, más la sección de arquitectura del Anexo 1 | — |
| 12.5.2 | Clases, con atributos, métodos, relaciones y multiplicidad | Sí | — | — |
| 12.6.1 | Código | **Depende de D-2** | Con (a), sí | D |
| 12.6.2 | Entidad-relación con cardinalidad, PK y FK | Sí | Hoy la p. 89 está en blanco: la reemplazan las dos vistas nuevas | — |
| 13 | Hoja de seguimiento: fecha, autor, descripción | Sí | Agregar las filas de la versión 2.5 (ítem 2 de CORRECCIONES) | R |

### Lo que pondera la consigna y el documento no muestra

**I-4 · «Realización de estimaciones de tiempos y costos del proyecto».** **Resuelta el 14/11.**
- **Tiempo:** está (78 puntos, 312 h).
- **Costo:** el texto completo está en el Anexo 4 de `AUDITORIA-DOCUMENTO-CODIGO.md`.
  - Las horas son las de la escala de esfuerzo del documento: horas-persona de ingeniería, no
    tiempos de herramienta.
  - El valor hora (V = $30.500) es un supuesto con fuente: Resolución 123 del Colegio de
    Ingenieros Especialistas de Santa Fe, vigente desde el 01/11/2026, Analista Programador
    Junior. Lo confirma o lo cambia
    JC.
  - Total: $9.516.000 para la estimación aprobada (312 h) y $8.540.000 para el alcance
    construido (280 h). Con ±25 % en V, entre $7.137.000 y $11.895.000.

## 2. Los 16 errores frecuentes, sobre todo el documento

Los diagramas nuevos ya se revisaron uno por uno: `PARA-MAXIMO-DIAGRAMAS.md`, «Las 16 reglas del
profesor». Acá va **el documento entero**, que incluye el texto y los diagramas que no se
rehicieron.

| # | Error | Pasa | Dónde falla y qué hacer |
|---|---|---|---|
| 1 | FK duplicada con la línea | Sí | — |
| 2 | Asociaciones sin multiplicidad | Sí | — |
| 3 | Composición mal usada | Sí (no hay) | — |
| 4 | Navegabilidad invertida | Sí | — |
| 5 | 1:N dibujado como 1:1 sin UNIQUE | Sí | — |
| 6 | N:M sin tabla intermedia | Sí | — |
| 7 | Herencia por conveniencia | Sí | — |
| 8 | include/extend al revés o sin condición | **No se sabe en los diagramas chicos** (pp. 72 a 85) | Siguen sin revisar: ver D-7 en la sección 4 |
| 9 | Robustez con conexiones prohibidas | Sí | Verificado por programa |
| 10 | Secuencia que accede a la base o no coincide con el código | **Sí, con el Anexo 2** | Sin el Anexo 2, la secuencia en texto de la p. 67 llama a métodos que no existen |
| 11 | Clases de software en el dominio | Sí | — |
| 12 | El sistema como actor o como carril | **No** | El diagrama chico de CU-06 (p. 80) tiene la pasarela como actor (T-13). **Sigue abierto:** D-7 en la sección 4 |
| 13 | Estados distintos en diagrama, clase y base | **Sí, con el Anexo 2** | Sin él, el `CHECK` de `Compras` de la p. 63 tiene 3 estados y la base 4. Y sin el Anexo 1, las pp. 57 a 61 traen «Agotado» y «Cancelado», que no existen |
| 14 | Objetivos que son funciones o no son medibles | **No** | OE-3, OE-5 y OE-6. Texto en I-1 |
| 15 | Patrón nombrado que no se ve en el diagrama | **Sí, con el Anexo 1 y el Anexo 2** | El Anexo 1 nombra Singleton, Repository, Strategy y la herencia de reportes, y las vistas 8a a 8c los muestran. Sin el Anexo 2, la p. 64 dice «del mismo modo que GestorClientes», que no es Singleton |
| 16 | Figuras ilegibles al 100 % | **No se sabe** en los diagramas chicos y en las capturas viejas | Se ve al armar el PDF: hay que leer cada figura al 100 % |

## 3. Los textos que faltaban

**I-1 · pp. 10 y 11, Objetivos específicos 3, 5 y 6 · R** (errores 4 y 14). Reemplazar cada
objetivo completo, desde su número hasta su «Cuándo evaluarlo», **por:**

> 3. Definir un programa de fidelización económicamente sostenible para los clientes frecuentes
>    ○ Qué lograr: establecer las reglas de un programa por niveles y de una suscripción, de modo
>      que el beneficio otorgado no comprometa la contribución de cada operación.
>    ○ Cómo medirlo: las reglas del programa quedan documentadas con su origen y verificadas por
>      al menos cinco casos de prueba, y el descuento del nivel más alto (7,5 %) no supera el
>      descuento máximo admisible calculado en la factibilidad económica (8,62 %).
>    ○ Cuándo evaluarlo: antes de la entrega de la versión completa del prototipo.

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

*Por qué:*
- El OE-3 de hoy promete preventas por nivel (son por suscripción, A-09) y una «pantalla de
  carrito» que no existe.
- El OE-5 promete HTTPS, y la aplicación es de escritorio y no usa la red.
- Los dos y el OE-6 describen funciones del sistema, y la guía dice «no deben enunciarse
  funciones específicas que tendrá el sistema».
- Las cifras de reemplazo salen del propio documento (pp. 29 y 32) y del código (RN-30, que
  prueba CP-32).

**I-2 · p. 12, Registro de interesados · R.** Agregar debajo del Interesado 2, con el mismo
formato:

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

Los interesados siguientes se renumeran del 4 al 8. El rol de Juan Cruz va como propuesta:
**JC lo confirma** (es la decisión 1 del final de CORRECCIONES, resuelta en la opción «se agrega
aparte»).

**I-3 · p. 45, Prototipos, ítem 3 · R.**
- **Dice:** «Pantalla de carrito de compra. Muestra el resumen completo del paquete elegido,
  descuentos aplicados y monto total.»
- **Poner:** «Resumen del paquete. En la misma pantalla del armado, muestra el detalle del paquete
  elegido, el descuento por nivel, el cargo por servicio, el seguro y el monto total.»
- *Razón:* no hay carrito; el resumen está en `FrmCompraCliente`.
- **Mismo cambio en la p. 39, ítem 3**, si queda esa copia (dato de JC n.º 3 de CORRECCIONES).

## 4. Qué es irrecuperable

**Nada, en el texto.** Todo lo que falla tiene un texto de reemplazo, o espera un dato o una
decisión de JC.

Lo que más cuesta, ordenado por horas de una persona:

1. **D-4, las especificaciones: hechas el 14/11** (Anexo 3 de la auditoría). Falta que JC las
   revise y que Máximo pegue unas 8 páginas.
   **D-7, los ocho diagramas chicos de CU-01 a CU-08 (pp. 72 a 85): resuelta el 14/11, se quitan**
   (ver D-7 en la auditoría). Lo que sigue queda como historia de la decisión.
   Hoy pierden puntos por el error 12 (la pasarela como actor en CU-06) y, sin revisar, por los
   errores 8 y 16.
   - **(a) Quitarlos.** El diagrama general de casos de uso ya muestra cada caso con sus
     relaciones. No es trabajo mío; Máximo los borra en unos 5 minutos.
   - **(b) Rehacerlos en PlantUML desde el diagrama general.** Unas 10 vueltas mías, más la
     revisión de JC y 15 minutos de Máximo.
   - **Recomiendo (a):** repetir el mismo dibujo ocho veces no suma, y cada copia es un lugar más
     donde el docente puede encontrar una contradicción.
2. **Los datos de JC:** portada, año y las nueve fechas del cronograma. Son minutos, pero nadie más
   los puede poner.
3. **La legibilidad de las figuras viejas (error 16).** Se ve recién al armar el PDF.

**Dos cosas que no se arreglan y conviene que el texto diga tal cual:**
- **Las minutas y el estudio de usabilidad son simulados.** La cátedra lo acepta y el documento
  lo rotula. No se tocan (fuera de alcance por decisión de JC).
- **El historial de revisión tiene cinco versiones con la misma fecha, 28/09/2025.** No se puede
  reconstruir la fecha real. La «Nota sobre las fechas» de la p. 6 ya lo explica: conviene que
  quede.

## 5. Estimación

**Lo mío:**
- esta revisión ya está hecha;
- la matriz de 10 comprobaciones, cuando llegue: unas 5 vueltas de herramienta;
- D-4: hecha.
- D-7, si se elige (b): unas 10.

**Portones humanos:**
- **JC:**
  - datos de portada y fechas;
  - confirmar o cambiar el valor hora de I-4 (V = $30.500);
  - el rol de I-2;
  - D-2, D-3, D-5, D-6 y D-7;
  - pasar la matriz.
- **Máximo:** pegar I-1 a I-4 en el Word, unos 20 minutos, más las ocho páginas de CU-09 a
  CU-13, unos 30 minutos.
