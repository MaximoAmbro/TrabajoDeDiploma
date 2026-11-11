# Correcciones pendientes del documento — EventAura v2.4 → v2.5

**Lista única** de todo lo que hay que cambiar en el documento entregable
(`Documentacion/EventAura_v2.pdf`, versión 2.4, 112 páginas) cuando aparezca su fuente
editable. Está ordenada por página, para aplicarla de corrido de la primera a la última.
Por qué el fuente no está en el repositorio y cuál es el plan B: ver
[DOCUMENTO-ENTREGABLE.md](DOCUMENTO-ENTREGABLE.md).

Cifras al 05/11/2026, verificadas contra el código y la batería del commit `51735a8`:
**61 casos de prueba y 73 ejecuciones, todas correctas; 38 reglas vigentes, 33 con caso de
prueba (86,8 %).**

**Cómo copiar al Word sin perder las tablas.** Abrí este archivo en VS Code y la vista previa
(`Ctrl+Shift+V`). Seleccioná desde la vista previa, copiá y pegá en Word. Las tablas llegan
como tablas; si copiás el texto crudo, llegan como líneas con `|`.

**Convenciones.**
- **Reemplazar**: el texto viejo se cita tal como figura en el PDF y debajo va el nuevo.
- **Agregar**: el texto no existía en el documento.
- Los números de página son los de la versión 2.4. Si el texto nuevo corre contenido, el
  índice (páginas 2 a 5) se vuelve a generar al final (ítem 22).
- «⚠ DATO DE JC» marca lo que no puedo completar sin un dato tuyo.

## Índice de la lista

| # | Pág. | Qué | Origen de la decisión |
|---|---|---|---|
| 1 | 1 y pie de todas | Carátula: comisión, turno, legajos y año | Pendiente de datos de JC |
| 2 | 6 | Historial de revisión: cinco filas nuevas | Regla de la página 38 |
| 3 | 7 | Resumen ejecutivo: 36 → 38 reglas; resultados de usabilidad | RN-38 y RN-39; estudio v2.5 |
| 4 | 8 | Resumen ejecutivo: cobertura; limitación de usabilidad | Matriz; estudio v2.5 |
| 4b | 24 | RNF-03: WinForms en lugar de navegadores; limitación de DPI | Decisión de JC |
| 5 | 26 | RN-06 alineada con `docs/`; RN-38 nueva | Decisión de JC |
| 6 | 27 | RN-29 y RN-35 alineadas con `docs/`; RN-39 nueva | Decisión de JC |
| 7 | 36 a 38 | RF-09 pasa a Won't have; RNF-03 con su limitación | Decisión de JC |
| 8 | 50 | Estado de implementación del instructivo | Decisión de JC |
| 9 | 52 y 53 | Pegar las figuras 1 a 4 | Paso 1 del 04/11 |
| 10 | 54 | Ejemplo resuelto: sin cambios de texto | Decisión de JC: se alinea la semilla |
| 11 | 83 | CU-07: orden de la lista de espera | RN-35 |
| 11b | 90 a 97 | Informe de usabilidad: rótulo «(simulado)» y contenido nuevo | Estudio v2.5 |
| 12 | 99 | A02: catálogo e indicadores | `docs/05-reportes.md` |
| 13 | 100 | A02: ejemplos RPT-01 y RPT-02 | `docs/05-reportes.md` |
| 14 | 101 | A02: ejemplo RPT-05 | `docs/05-reportes.md` |
| 15 | 102 y 103 | A02: diseño técnico | `docs/05-reportes.md` |
| 16 | 104 y 105 | A02: SQL de RPT-01 y código de exportación | `docs/05-reportes.md` |
| 17 | 106 | Matriz: criterio y nomenclatura | Matriz |
| 18 | 107 | Matriz principal; evidencia de OE-4 | Matriz; estudio v2.5 |
| 19 | 108 | Advertencia sobre OE-4; cobertura y catálogo CP-11 a CP-61 | Matriz; estudio v2.5 |
| 20 | 109 | Brechas G-02 a G-06 | Matriz; estudio v2.5 |
| 21 | 110 y 111 | Conclusiones, limitaciones y trabajo futuro | Matriz |
| 22 | 2 a 5 | Regenerar el índice | Al final |
| 23 | 6, 7, 8, 108, 109 y 110 | Las 64 pruebas de interfaz pendientes, convertidas en casos automáticos: cifras y párrafo de origen | Decisión de JC del 10/11 (`PRUEBAS-64-CLASIFICACION.md`) |
| 24 | 9, 10, 12 a 17, 21, 29, 36, 38, 39, 44, 45 y anexos | Rótulo parejo: todo lo simulado dice «simulado» y todo esfuerzo dice «estimado» | Regla de JC del 10/11 |
| 25 | 6, 7, 8, 25 o 26, 106 a 111 | Seguridad: RN-18 completa, RN-40 a RN-42 nuevas, matriz y cobertura sobre 41 reglas. **Condicionado a SEG-1 a SEG-3** | Auditoría de seguridad del 10/11 (`AUDITORIA-SEGURIDAD.md`) |

---

## 1. Carátula (página 1) y pie de todas las páginas — ⚠ DATO DE JC

**Página 1.** Reemplazar:

> Comisión: «completar» · Turno: «completar»
> Legajos: «completar» – «completar»

y el año debajo de «Rosario, Argentina»: «2025».

**Pie de las 112 páginas.** Reemplazar en:

> Materia: Trabajo de Campo I · Docente: Audoglio · Comisión: «completar» · Turno: «completar» ·
> Localización: Rosario · Año: 2025

**Encabezado de las páginas** (hoy dice «Alumnos: Ambrosino, Máximo (Legajo «completar») ·
Riggio, Juan Cruz (Legajo «completar»)»): completar los dos legajos.

Los datos que faltan están en la sección «Datos que necesito de JC», al final de este archivo.

---

## 2. Página 6 — Historial de revisión

**Agregar** cinco filas al final de la tabla. «fecha» es el día en que se aplican los cambios
en el Word.

| Fecha | Versión | Autor | Descripción del cambio |
|---|---|---|---|
| «fecha» | 2.5 | Riggio, Juan Cruz | Reglas de negocio alineadas con el prototipo: se incorporan RN-38 (capacidad del local) y RN-39 (preventa para suscriptores), que el sistema ya aplicaba, y se reescribe RN-35 (prioridad de los suscriptores en la lista de espera, coherente con RN-37). Se detallan RN-06 y RN-29. Reglas vigentes: 38 (brecha G-05). |
| «fecha» | 2.5 | Riggio, Juan Cruz | Instructivo del requerimiento core: estado de implementación actualizado e incorporación de las figuras 1 a 4. CU-07 alineado con RN-35. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Anexo A02: consulta de RPT-01 corregida, catálogo, indicadores, diseño técnico y ejemplos ajustados a la implementación. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Matriz de trazabilidad: casos de prueba CP-11 a CP-61 y cobertura recalculada sobre 38 reglas. RF-09 se reclasifica de Could a Won't have según la regla de repriorización (brecha G-03). Brecha G-04 resuelta; G-02 resuelta en requerimientos y abierta en reglas. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Informe de pruebas de usabilidad rehecho como estudio simulado medido sobre la aplicación real: medición instrumentada de la interfaz, evaluación heurística, recorrido cognitivo y Keystroke-Level Model con seis perfiles, en dos mediciones: antes y después de corregir los tres hallazgos de mayor peso (U-01, U-04 y U-05). Hallazgos U-01 a U-14, cuatro de ellos corregidos con prueba de regresión (U-14, el que encontró la segunda medición, también), y nuevo plan de mejora (brecha G-06). RNF-03 alineado con la plataforma Windows Forms, con su limitación de escalado declarada. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Carátula y pie: comisión, turno, legajos y año de entrega. |

> La página 38 exige que todo cambio de categoría MoSCoW quede en el historial «indicando
> fecha, requerimiento afectado, categoría anterior, categoría nueva y motivo». La cuarta
> fila lo cumple.

---

## 3. Página 7 — Resumen ejecutivo, «Qué contiene este informe»

En la fila «Reglas de negocio», **reemplazar:**

> Treinta y seis reglas vigentes que fijan las condiciones bajo las cuales opera el sistema

**Por:**

> Treinta y ocho reglas vigentes que fijan las condiciones bajo las cuales opera el sistema

En la fila «Validación», **reemplazar:**

> Informe de pruebas de usabilidad con cinco participantes y anexo A02 con cinco reportes de gestión.

**Por:**

> Informe de pruebas de usabilidad (simulado, medido sobre la aplicación) con seis perfiles, y
> anexo A02 con cinco reportes de gestión.

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

---

## 4. Página 8 — Resumen ejecutivo, «Limitaciones declaradas»

Dos reemplazos. **Primero:**

> Las sesiones de usabilidad son simuladas y deberán repetirse con participantes reales.

**Por:**

> Las sesiones de usabilidad son simuladas: el comportamiento de la interfaz se midió sobre la
> aplicación real, y los tiempos y valoraciones de cada perfil se derivaron de esa medición.

**Reemplazar:**

> Y la cobertura de verificación alcanza al 22,2 % de las reglas de negocio, concentrada en
> la lógica de fidelización.

**Por:**

> Y la cobertura de verificación alcanza al 86,8 % de las reglas de negocio: cinco de las
> treinta y ocho no tienen todavía un caso de prueba automatizado, y ninguna de esas cinco
> rechaza operaciones.

---

## 4b. Página 24 — Requerimientos no funcionales, punto 3 (RNF-03)

El texto actual describe un prototipo web, pero la aplicación es Windows Forms, como dice el
mismo documento en W-02 («Fuera del alcance tecnológico definido (interfaz en Windows
Forms)»). Se aplica el criterio que JC decidió para RN-35: el documento se alinea con el
producto. La limitación de escalado la decidió JC el 04/11/2026: no se activa la
compatibilidad con DPI, y queda documentada.

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
>      diecinueve pantallas.

---

## 5. Página 26 — Reglas: «Eventos, sectores y disponibilidad»

Las reglas respetan el criterio de la página 25: verificables, atómicas y sin mencionar
pantallas, controles ni tablas.

**Reemplazar** la fila de RN-06:

| RN-06 | Un evento no puede publicarse si no tiene al menos un sector definido con capacidad mayor que cero. | A | RF-12 / CU-02 |
|---|---|---|---|

**Por:**

| RN-06 | Un evento publicado tiene en todo momento al menos un sector con capacidad mayor que cero: no puede publicarse sin sectores, ni puede eliminarse el último sector de un evento ya publicado. | E | RF-12 / CU-02 |
|---|---|---|---|

El tipo pasa de A a E: dejó de ser una condición de una sola operación (publicar) y es una
restricción que los datos cumplen siempre, como la define `docs/02-reglas-negocio.md`.

**Agregar** después de RN-11:

| RN-38 | La suma de las capacidades de los sectores de un evento no puede superar la capacidad del local donde se realiza. Se verifica al agregar o modificar un sector. | E | RF-12 / CU-02 |
|---|---|---|---|

---

## 6. Página 27 — Reglas: «Suscripciones y seguros» y «Reportes y listas de espera»

**Reemplazar** la fila de RN-29:

| RN-29 | El seguro habilita la devolución del valor de la entrada hasta cuarenta y ocho horas antes del inicio del evento. | A | RF-05 |
|---|---|---|---|

**Por:**

| RN-29 | El seguro habilita la devolución hasta cuarenta y ocho horas antes del inicio del evento. La devolución es por entrada: el cliente elige cuáles da de baja, y cada una debe estar confirmada y asegurada. Se reintegra el precio congelado de cada entrada menos su parte proporcional del descuento por nivel y, si con ello no queda ninguna entrada confirmada en la compra, también los servicios complementarios. No se reintegran el cargo por servicio ni la prima del seguro. El costo de los servicios devueltos se recupera del proveedor, condición que debe constar en el acuerdo con cada uno. | A | RF-05 |
|---|---|---|---|

**Agregar** después de RN-37:

| RN-39 | Mientras no haya terminado el período de preventa de un evento, sólo un cliente con suscripción activa puede iniciar una compra de ese evento; el evento sigue visible en el listado para todos los clientes. Un evento sin período de preventa no tiene esta restricción. Es el beneficio de preventa que enuncia RN-37. | A | RF-13 / CU-01 |
|---|---|---|---|

**Reemplazar** la fila de RN-35:

| RN-35 | La lista de espera respeta estrictamente el orden de inscripción. | E | RF-10 / CU-07 |
|---|---|---|---|

**Por:**

| RN-35 | La lista de espera se ordena primero por los clientes que se inscribieron con suscripción activa y después por el resto; dentro de cada grupo, por orden de inscripción. La condición de suscriptor se toma al inscribirse y no cambia aunque la suscripción venza o se cancele después. Es el beneficio de prioridad que enuncia RN-37. | E | RF-10 / CU-07 |
|---|---|---|---|

> RN-35 queda sujeta a la confirmación de Máximo (si «estrictamente» fue deliberado). JC
> decidió alinear el documento con el código: la prioridad para suscriptores es lo que el
> producto hace y lo que promete RN-37.

---

## 7. Páginas 36 a 38 — Priorización (RF-09 pasa a Won't have)

### Página 36 — tabla «Priorización de requerimientos funcionales», fila RF-09

**Reemplazar:**

| RF-09 | Recomendaciones personalizadas | Could | 8 | El más costoso de los no obligatorios y sin objetivo específico asociado. |
|---|---|---|---|---|

**Por:**

| RF-09 | Recomendaciones personalizadas | Won't (desde v2.5) | 8 | Era Could: el más costoso de los no obligatorios y sin objetivo específico asociado. Se excluye en la Fase 8 por la regla de repriorización (brecha G-03). |
|---|---|---|---|---|

### Página 36 — debajo de «Observación sobre el desvío»

La tabla «Distribución del esfuerzo» **no se recalcula**: queda como la estimación aprobada.

**Agregar:**

> **Nota de la versión 2.5.** RF-09 se reclasificó como Won't have en la Fase 8. Sus ocho
> puntos se liberan sin afectar a ningún requerimiento Must, como prevé la mitigación
> descrita: la categoría Could funcionó como amortiguador. La tabla de distribución se
> conserva como la estimación aprobada.

### Página 37 — tabla «Requerimientos excluidos — categoría Won't have»

**Agregar** la fila:

| W-07 | Recomendaciones personalizadas (RF-09) | Reclasificado desde Could en la versión 2.5. No responde a ningún objetivo específico ni tiene caso de uso, y es el requerimiento no obligatorio más costoso (8 puntos). |
|---|---|---|

### Página 37 — tabla «Priorización de requerimientos no funcionales», fila RNF-03

**Reemplazar** la justificación:

> Deseable, pero la evaluación del prototipo se realiza en escritorio.

**Por:**

> Deseable, pero la evaluación del prototipo se realiza en escritorio. Se cumple en tamaño de
> pantalla (casos CP-12 y CP-25); la nitidez con escalado de Windows superior al 100 % queda
> como limitación conocida (ver Definición de requerimientos, punto 3).

### Página 38 — tabla «Orden de implementación derivado», fila de la Fase 8

**Reemplazar** en la columna Requerimientos:

> RF-09 y RF-10, según margen disponible

**Por:**

> RF-10 (RF-09 pasó a Won't have en la versión 2.5)

---

## 8. Página 50 — Instructivo: estado de implementación

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

---

## 9. Páginas 52 y 53 — Figuras del recorrido

Reemplazar cada «Espacio reservado para captura de pantalla» por la imagen
correspondiente de `docs/capturas/instructivo/`. Los epígrafes no cambian.

| Epígrafe | Archivo |
|---|---|
| Figura 1 — Listado de eventos disponibles (pasos 1 y 2) | `figura1-listado-eventos.png` |
| Figura 2 — Pantalla de armado de paquete (pasos 3 a 5) | `figura2-armado-paquete.png` |
| Figura 3 — Resumen del paquete con el descuento aplicado (paso 6) | `figura3-resumen-descuento.png` |
| Figura 4 — Confirmación de compra y comprobante emitido (pasos 7 y 8) | `figura4-confirmacion-y-comprobante.png` |

> Las figuras son las definitivas: se regeneraron el 05/11/2026, después de T-29, con
> «Duki en Rosario» y la clienta Plata con 4 compras. La figura 2 muestra el encabezado
> «Cantidad (escribí aquí)» completo, en dos renglones. El resumen coincide línea por línea con
> la tabla de la página 54.

---

## 10. Página 54 — Ejemplo resuelto

**Sin cambios de texto.** JC decidió alinear la semilla con el documento, no al revés:
después de T-21, el sistema tiene «Duki en Rosario» y una clienta Plata con 4 compras, con
los mismos importes ($99.625). El docente puede seguir el instructivo en la aplicación y
reproducir el ejemplo exacto.

---

## 11. Página 83 — CU-07 (lista de espera), paso 4

**Reemplazar:**

> 4. El sistema muestra la lista de Clientes en orden de inscripción.

**Por:**

> 4. El sistema muestra la lista de Clientes en orden de prioridad: primero los inscriptos
> con suscripción activa al momento de inscribirse y después el resto, cada grupo por orden
> de inscripción (RN-35).

---

## 11b. Páginas 90 a 97 — Informe de pruebas de usabilidad

> **Definitivo.** El informe tiene las dos mediciones: antes y después de T-22 a T-26. U-14,
> el hallazgo que encontró la segunda medición, se corrigió después (T-29). La tabla «Antes y
> después» del principio del informe es buen material para la defensa.

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

---

## 12. Página 99 — A02: catálogo e indicadores

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

## 13. Página 100 — A02: ejemplos 1 y 2

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

## 14. Página 101 — A02: ejemplo 5 (RPT-05)

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

## 15. Páginas 102 y 103 — A02: diseño técnico

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

## 16. Páginas 104 y 105 — A02: SQL de RPT-01 y código de exportación

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

---

## 17. Página 106 — Matriz: criterio de construcción y nomenclatura

### Criterio de construcción

**Reemplazar:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y seis
> reglas, los ocho casos de uso y los diez casos de prueba resulta ilegible en formato A4

**Por:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y ocho
> reglas, los ocho casos de uso y los sesenta y un casos de prueba resulta ilegible en
> formato A4

### Tabla «Nomenclatura de los ejes»

**Reemplazar** las dos filas:

| Reglas de negocio | RN-xx | 36 | Reglas de negocio (RN-27 dada de baja) |
|---|---|---|---|
| Casos de prueba | CP-xx | 10 | Desarrollo de la lógica de descuento por nivel |

**Por:**

| Reglas de negocio | RN-xx | 38 | Reglas de negocio (RN-27 dada de baja) |
|---|---|---|---|
| Casos de prueba | CP-xx | 61 | CP-01 a CP-10: Desarrollo de la lógica de descuento por nivel. CP-11 a CP-61: Matriz de trazabilidad, catálogo de casos incorporados |

---

## 18. Página 107 — Matriz principal

**Reemplazar la tabla completa.** Cambian la columna «Casos de prueba», las reglas de RF-10
(sin cambio de número, RN-35 reescrita), RF-12 (se suma RN-38) y RF-13 (se suma RN-39), y la
categoría de RF-09.

| RF | OE | MoSCoW | Reglas de negocio | CU | Casos de prueba | Hall. |
|---|---|---|---|---|---|---|
| RF-01 | OE-2 | Must | RN-01, RN-02, RN-03 | CU-04 | CP-14, CP-15, CP-26, CP-44, CP-45, CP-55 | — |
| RF-02 | OE-2 | Must | RN-04, RN-05 | CU-05 | CP-27, CP-28, CP-29, CP-46, CP-55 | — |
| RF-03 | OE-2 | Must | RN-09 | CU-01 | CP-16, CP-17, CP-18, CP-30 | — |
| RF-04 | OE-2 | Must | RN-08, RN-12, RN-33 | CU-01 | CP-20, CP-31, CP-40, CP-49, CP-54 | H-03 |
| RF-05 | OE-2 | Must | RN-13, RN-14, RN-28, RN-29 | CU-01 | CP-11, CP-13, CP-19, CP-22 a CP-25, CP-31, CP-41, CP-48, CP-50, CP-57, CP-59, CP-61 | H-04 |
| RF-06 | OE-5 | Must | RN-15, RN-16, RN-30, RN-31, RN-32 | CU-01 | CP-07, CP-32, CP-33, CP-34, CP-39, CP-43, CP-52 | H-02 |
| RF-07 | OE-2 | Must | RN-17, RN-18 | CU-01 | CP-32, CP-33 | — |
| RF-08 | OE-3 | Should | RN-19 a RN-24 | CU-01, CU-03 | CP-01 a CP-06, CP-08, CP-09, CP-11, CP-42 | H-01, H-05 |
| RF-09 | — | Won't | — | — | — | — |
| RF-10 | OE-6 ind. | Could | RN-10, RN-35, RN-36 | CU-07 | CP-24, CP-37 | — |
| RF-11 | OE-6 | Should | RN-34 | CU-08 | CP-35, CP-36, CP-37, CP-38 | — |
| RF-12 | OE-2 | Must | RN-06, RN-07, RN-11, RN-38 | CU-02 | CP-21, CP-56, CP-60 | — |
| RF-13 | OE-3 | Should | RN-25, RN-26, RN-37, RN-39 | CU-06 | CP-10, CP-58, CP-61 | — |

El párrafo «La indicación "ind." en RF-10…» queda igual.

**Agregar** debajo:

> La categoría Won't de RF-09 corresponde a la reclasificación de la versión 2.5 (brecha
> G-03). Los requerimientos no funcionales no figuran en la matriz porque no originan casos
> de uso. RNF-03 (compatibilidad multipantalla) cuenta con dos casos de prueba, CP-12 y
> CP-25, y RNF-02 (experiencia de usuario) con tres: CP-47, que verifica que la aplicación
> termina al cerrar su última ventana, y CP-51 y CP-53, que verifican que un error no previsto
> llega al usuario como un mensaje en castellano. CP-55 figura en RF-01 y RF-02, y CP-61 en
> RF-05 y RF-13, porque prueban una regla de una fila desde la operación de la otra.

En la tabla «Trazabilidad inversa de los objetivos específicos», fila OE-4, **reemplazar** la
evidencia:

> Informe de pruebas de usabilidad: SUS 74,5 y 80 % de satisfacción

**Por:**

> Informe de pruebas de usabilidad (simulado, medido sobre la aplicación): SUS 78,8 y 100 %
> de satisfacción buena o muy buena

El estado sigue «Cumplido». El resto de la tabla no cambia.

---

## 19. Página 108 — Cobertura

### Advertencia sobre OE-4, al principio de la página

**Reemplazar:**

> **Advertencia sobre OE-4.** El objetivo se declara cumplido sobre la base del Informe de
> pruebas de usabilidad, cuyas sesiones son simuladas según se declara en su encabezado. El
> cumplimiento quedará plenamente acreditado una vez ejecutadas las sesiones con participantes
> reales, previstas para la Fase 8 del cronograma.

**Por:**

> **Advertencia sobre OE-4.** El objetivo se declara cumplido sobre la base del Informe de
> pruebas de usabilidad, que es simulado según se declara en su encabezado, como las minutas.
> A diferencia de un estudio enteramente supuesto, el comportamiento de la interfaz se midió
> sobre la aplicación real, dos veces, y cada resultado por perfil se deriva de esa
> medición. En la primera medición el porcentaje de perfiles con satisfacción buena o muy
> buena (66,7 %) no alcanzaba su umbral. Se corrigieron las causas medidas, y en la segunda
> llegó al 100 %, con un promedio de 4,36: el objetivo se cumple por las dos vías.

### Tabla «Cobertura de reglas de negocio por casos de prueba»

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

**Reemplazar** el párrafo que sigue a la tabla:

> Los diez casos verifican ocho reglas distintas —RN-16, RN-19, RN-20, RN-21, RN-22, RN-23,
> RN-24 y RN-37— sobre un total de treinta y seis vigentes, lo que arroja una cobertura del
> 22,2 %. Las veintiocho reglas restantes carecen de verificación automatizada.

**Por:**

> Los sesenta y un casos verifican treinta y tres reglas distintas sobre un total de
> treinta y ocho vigentes, lo que arroja una cobertura del 86,8 %. Son RN-01 a RN-06, RN-08 a
> RN-25, RN-29 a RN-32, RN-34 y RN-36 a RN-39. Las diecinueve reglas que rechazan una
> operación tienen un caso que provoca ese rechazo y comprueba que el sistema queda como
> estaba. Las cinco restantes carecen de verificación automatizada: RN-07, RN-26, RN-28, RN-33
> y RN-35. Ninguna rechaza operaciones: describen un cálculo (RN-07), la ausencia de
> renovación automática (RN-26), el momento de contratar el seguro (RN-28), la moneda (RN-33)
> y el orden de la lista de espera (RN-35).
>
> Varios casos no suman reglas porque verifican otra cosa: que la interfaz comunique al
> usuario reglas que ya funcionan, que es el problema que señaló el informe de usabilidad.
> CP-39 a CP-42 son la regresión de los hallazgos H-02 a H-05: muestran el temporizador de
> RN-15, el cupo de RN-08, el aviso de RN-28 y los umbrales de RN-19, pero no verifican esas
> reglas.

### Agregar: tabla «Casos de prueba incorporados en la versión 2.5»

Va a continuación del párrafo anterior. CP-01 a CP-10 siguen definidos en «Desarrollo de la
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

### Tabla «Análisis de cobertura»

**Reemplazar la tabla completa.** Se agrega la fila de hallazgos con la acción implementada:

| Dimensión evaluada | Total | Cubierto | Sin cobertura | % |
|---|---|---|---|---|
| Objetivos específicos con evidencia documentada | 6 | 6 | — | 100 % |
| Requerimientos funcionales con caso de uso | 13 | 12 | RF-09 (Won't) | 92,3 % |
| Casos de uso con requerimiento que los origine | 8 | 8 | — | 100 % |
| Reglas de negocio con origen declarado | 38 | 38 | — | 100 % |
| Requerimientos funcionales con caso de prueba | 13 | 12 | RF-09 (Won't) | 92,3 % |
| Reglas de negocio verificadas por caso de prueba | 38 | 33 | 5 reglas | 86,8 % |
| Hallazgos de usabilidad con acción de mejora | 6 | 6 | — | 100 % |
| Hallazgos con la acción implementada en el prototipo | 6 | 6 | — | 100 % |
| Hallazgos con prueba de regresión asociada | 6 | 6 | — | 100 % |
| Hallazgos del estudio v2.5 (U-01 a U-14) con acción de mejora | 14 | 14 | — | 100 % |
| Hallazgos del estudio v2.5 corregidos, con prueba de regresión | 14 | 4 | 10 hallazgos | 28,6 % |

Las filas de hallazgos H-01 a H-06 corresponden al estudio de la versión 2.4, cuyas
correcciones están hechas. Las dos últimas corresponden al estudio rehecho en la versión 2.5
(ver la brecha G-06).

> Los cuatro hallazgos corregidos son U-01, U-04, U-05 y U-14, con sus regresiones CP-47 y
> CP-48, CP-49, CP-50 y CP-54. **Si antes de pasar esto al Word se corrigen otros,**
> actualizar esta fila y G-06.

### Lectura del cuadro

**Reemplazar:**

> La trazabilidad hacia adelante, del requerimiento a su verificación, es el punto débil del
> proyecto: sólo tres de trece requerimientos cuentan con pruebas, y todas ellas se
> concentran en la lógica de fidelización.

**Por:**

> La trazabilidad hacia adelante, del requerimiento a su verificación, pasó de ser el punto
> débil del proyecto a estar completa en el nivel de requerimientos: los doce en alcance
> cuentan con pruebas, contra tres de trece en la versión 2.4. El único sin pruebas es
> RF-09, excluido. En el nivel de reglas, treinta y tres de treinta y ocho tienen caso
> automatizado, incluidas todas las que rechazan una operación. Quedan cinco, entre ellas
> RN-35, que esta versión reescribió.

---

## 20. Página 109 — Brechas detectadas

**Reemplazar** la oración inicial:

> El análisis identificó cuatro brechas. La primera fue resuelta a partir de este mismo
> análisis; las tres restantes quedan documentadas con su acción correctiva.

**Por:**

> El análisis identificó seis brechas. G-01 se resolvió a partir de este mismo análisis.
> En la versión 2.5 se cerró G-03 y se resolvieron G-04 y G-05. G-02 quedó resuelta en el
> nivel de requerimientos y abierta en el de reglas. G-06, nueva, quedó resuelta en sus tres
> hallazgos de mayor peso y en el que encontró la segunda medición, y abierta en el resto,
> con su plan.

**Reemplazar la tabla completa.** G-01 no cambia y se repite para que la tabla quede entera:

| Nº | Brecha | Sev. | Estado | Acción |
|---|---|---|---|---|
| G-01 | Los casos de uso CU-02 (Gestionar eventos) y CU-06 (Suscribirse) no contaban con un requerimiento funcional que los originara, pese a que las reglas RN-06, RN-07 y RN-11 por un lado, y RN-25, RN-26 y RN-37 por el otro, presuponen su existencia. | Alta | Resuelta | Se incorporaron los requerimientos RF-12 (Gestión de eventos) y RF-13 (Contratación de suscripciones) a la Definición de requerimientos, y se recalculó la priorización MoSCoW. |
| G-02 | En la versión 2.4, sólo tres de los trece requerimientos poseían casos de prueba (23,1 %) y la cobertura sobre reglas de negocio era del 22,2 %. | Alta | Resuelta en requerimientos; abierta en reglas | Se incorporaron los casos CP-11 a CP-61. Los doce requerimientos en alcance tienen pruebas. La cobertura de reglas sube al 86,8 % (33 de 38), con todas las reglas que rechazan operaciones; quedan cinco reglas sin caso (RN-07, RN-26, RN-28, RN-33 y RN-35). |
| G-03 | RF-09 (Recomendaciones personalizadas) no responde a ningún objetivo específico ni posee caso de uso asociado. | Media | Cerrada | Se reclasificó como Won't have en la Fase 8, según la regla de repriorización (W-07). |
| G-04 | En la versión 2.4, ninguno de los seis hallazgos del informe de usabilidad tenía caso de prueba de regresión. | Media | Resuelta | Las seis acciones del plan de mejora están implementadas en el prototipo y cada hallazgo tiene su regresión automatizada: H-01 en CP-13, H-02 en CP-39, H-03 en CP-40, H-04 en CP-41, H-05 en CP-42 y H-06 en CP-19. |
| G-05 | El documento no reflejaba tres reglas que el prototipo aplica: el tope de capacidad del local sobre la suma de sus sectores y la preventa exclusiva para suscriptores no estaban enunciadas, y la prioridad de los suscriptores en la lista de espera contradecía RN-35, que exigía orden estricto de inscripción. Era comportamiento implementado sin una regla documentada que lo respalde. | Alta | Resuelta | Se incorporaron RN-38 (RF-12 / CU-02) y RN-39 (RF-13 / CU-01), y se reescribió RN-35 en coherencia con RN-37. RN-38 y RN-39 tienen su caso de prueba (CP-60 y CP-61); RN-35 queda sin caso, dentro de la parte abierta de G-02. |
| G-06 | El estudio de usabilidad de la versión 2.5, medido sobre la aplicación real, identificó trece hallazgos nuevos (U-01 a U-13), y la segunda medición uno más (U-14). Dos eran de severidad mayor: cerrar una pantalla con la X dejaba la aplicación en ejecución y sin ventanas, y nada indicaba dónde se escribe la cantidad de entradas. | Media | Parcialmente resuelta | Se corrigieron U-01, U-04 y U-05, cada uno con su prueba de regresión (CP-47 a CP-50), y se volvió a medir: los seis perfiles completan la tarea sin asistencia y la satisfacción buena pasa del 66,7 % al 100 %. La segunda medición encontró U-14, el encabezado de la cantidad cortado, que también se corrigió (CP-54). Quedan diez hallazgos menores (esfuerzo estimado: 12 horas), que pasan a trabajo futuro. |

### Observación sobre G-03

**Reemplazar** el párrafo completo:

> **Observación sobre G-03.** La matriz confirma por una vía independiente la decisión ya
> adoptada en la priorización: RF-09 había sido clasificado como Could por ser el
> requerimiento más costoso sin objetivo asociado. Que el análisis de trazabilidad llegue al
> mismo resultado partiendo de otro criterio refuerza la validez de ambos.

**Por:**

> **Observación sobre G-03.** La matriz confirmó por una vía independiente la decisión ya
> adoptada en la priorización: RF-09 había sido clasificado como Could por ser el
> requerimiento más costoso sin objetivo asociado. Que el análisis de trazabilidad llegara
> al mismo resultado partiendo de otro criterio reforzó la validez de ambos. En la Fase 8 se
> aplicó lo previsto: RF-09 no se incorporó y pasó a Won't have. La brecha se cierra por
> decisión de alcance y no por construcción.

### Agregar: observación sobre G-05

Debajo de la observación sobre G-03:

> **Observación sobre G-05.** Esta brecha recorre la cadena en sentido inverso a las otras:
> no falta una prueba para una regla, falta la regla para un comportamiento que ya existe.
> Se prefirió documentarla aunque la cobertura bajara: con 36 reglas el porcentaje habría
> sido mayor, pero habría medido un sistema distinto del construido.

«Mantenimiento de la matriz» y el párrafo final de la página no cambian.

---

## 21. Páginas 110 y 111 — Conclusiones

### Página 110 — «Sobre la validación con usuarios»

**Reemplazar** el párrafo completo:

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

### Página 110 — «Sobre las reglas de negocio»

**Reemplazar:**

> Explicitar las treinta y seis reglas resultó más productivo de lo esperado.

**Por:**

> Explicitar las treinta y ocho reglas resultó más productivo de lo esperado.

### Página 110 — tabla «Limitaciones del trabajo», primera fila

**Reemplazar:**

| Las sesiones de usabilidad son simuladas. | El objetivo específico 4 se considera cumplido de manera provisoria hasta su ejecución con participantes reales. |
|---|---|

**Por:**

| Las sesiones de usabilidad son simuladas. | La interfaz se midió sobre la aplicación real; los tiempos y las valoraciones de cada perfil son derivaciones de esa medición y deben leerse como predicciones. |
|---|---|

### Página 110 — tabla «Limitaciones del trabajo», tercera fila

**Reemplazar:**

| La cobertura de pruebas alcanza el 22,2 % de las reglas. | Diez de trece requerimientos carecen de verificación automatizada. |
|---|---|

**Por:**

| La cobertura de pruebas alcanza el 86,8 % de las reglas. | Todos los requerimientos en alcance tienen pruebas, pero cinco reglas, ninguna de las que rechazan operaciones, carecen de verificación automatizada. |
|---|---|

### Página 111 — Trabajo futuro

**Reemplazar** el punto 1:

> Ejecutar las sesiones de usabilidad con participantes reales y contrastar los resultados con
> la línea base establecida en este informe.

**Por:**

> Aplicar el resto del plan de mejora del estudio de usabilidad (U-02, U-03 y U-06 a U-13) y
> repetir la medición con las mismas herramientas para contrastar los resultados.

**Reemplazar** el punto 2:

> Extender el conjunto de pruebas al flujo de compra completo, de modo de cerrar la brecha
> G-02 de la matriz de trazabilidad.

**Por:**

> Completar los casos de prueba de las cinco reglas sin verificación, empezando por RN-35,
> el orden de la lista de espera que esta versión reescribió, de modo de cerrar la brecha
> G-02 de la matriz de trazabilidad.

**Reemplazar** el punto 4:

> Definir un caso de prueba de regresión por cada hallazgo de usabilidad antes de aplicar las
> correcciones de la Fase 6.

**Por:**

> Mantener una prueba de regresión por cada hallazgo de las próximas rondas de usabilidad,
> como se hizo con H-01 a H-06.

---

## 22. Páginas 2 a 5 — Índice

Al terminar, regenerar el índice del documento: los ítems 5, 6, 12 a 16, 19 y 23 agregan
contenido y pueden correr páginas.

---

## 23. Las 64 pruebas de interfaz pendientes (páginas 6, 7, 8, 108, 109 y 110)

> **Abierto: depende de que Codex cierre la séptima tanda** (`TANDAS-PRUEBAS-64.md`). Las
> cifras van entre «comillas angulares» y se completan con el reporte de esa tanda, **no
> antes**. Las cifras de los ítems 2, 4, 19, 20 y 21 (61 casos, 73 ejecuciones) tampoco están
> al día: la parte 6A agregó CP-62 a CP-65 (77 ejecuciones). Se actualizan todas juntas al
> cerrar.

**Qué dice hoy el documento sobre estas pruebas: nada.** Se buscó en el texto de las 112
páginas: no menciona pruebas manuales ni quién las ejecutaría. Solo habla de CP-01 a CP-10 y
de la cobertura. **No hay que retractar nada.** Lo que hay que hacer es contar de dónde
salieron los casos nuevos sin afirmar algo que no pasó: **ninguna de estas 64 la ejecutó una
persona tildando una lista**, y la redacción no lo sugiere.

### Página 108 — Agregar: párrafo «Origen de los casos», después de la tabla «Casos de prueba incorporados en la versión 2.5»

> **Origen de los casos.** Durante la construcción, cada sección del prototipo dejó anotadas
> las pruebas de interfaz que no podían hacerse en ese momento, con la acción, el resultado
> esperado y el cambio que las originaba: ciento veintinueve en total.
>
> Las sesenta y cinco del recorrido del cliente se ejecutaron sobre la aplicación real,
> conducida con UI Automation, y cada falla corregida quedó como caso automático (CP-13 a
> CP-25).
>
> De las sesenta y cuatro restantes:
> - «57» se convirtieron en casos automáticos (CP-«66» a CP-«90»), que abren los formularios
>   reales contra una base de prueba y se ejecutan en cada corrida;
> - dos ya estaban cubiertas por casos existentes;
> - dos quedaron sin objeto porque describían pantallas que se rediseñaron;
> - dos verifican el entorno de pruebas y se resolvieron con scripts;
> - una, el aspecto del reporte en tres lectores de PDF, la verificó una persona, junto con
>   la lectura de su código QR con un celular.
>
> Se prefirió convertirlas en casos automáticos porque una prueba repetible deja evidencia en
> cada ejecución. Una verificación hecha a mano solo vale el día en que se hizo.

> **Para JC:** si el docente pregunta quién ejecutó las 65 del cliente, la respuesta está en
> `docs/COMO-SE-PROBO.md`: Claude Code, a pedido de JC, con UI Automation, el 03/11/2026.

### Página 108 — tabla «Casos de prueba incorporados en la versión 2.5»

**Agregar** las filas CP-62 a CP-«90», con el mismo formato que las del ítem 19. Las de CP-62 a
CP-65 salen del reporte de 6A. Las demás, del reporte de la séptima tanda, que trae la
correspondencia PA → CP y las filas manuales que cubre cada caso.

### Página 108 — cobertura

Si PA-19 (RN-35) queda como caso, reemplazar en el ítem 19, tabla «Análisis de cobertura»,
fila «Reglas de negocio verificadas por caso de prueba»:

| Reglas de negocio verificadas por caso de prueba | 38 | 33 | 5 reglas | 86,8 % |
|---|---|---|---|---|

**Por:**

| Reglas de negocio verificadas por caso de prueba | 38 | 34 | 4 reglas | 89,5 % |
|---|---|---|---|---|

Y en el párrafo del ítem 19 («Los sesenta y un casos verifican…»):
- **cifras:** «sesenta y un casos» → «Los «N» casos»; «treinta y tres reglas» → «treinta y
  cuatro»; «86,8 %» → «89,5 %»;
- **listas de reglas:** RN-35 se agrega a la de reglas cubiertas y se quita de las cinco sin
  caso, que pasan a ser cuatro: RN-07, RN-26, RN-28 y RN-33.

### Página 6 — Historial de revisión (ítem 2, cuarta fila)

**Reemplazar** «casos de prueba CP-11 a CP-61» **por** «casos de prueba CP-11 a CP-«90»,
incluidas las pruebas de interfaz pendientes de la construcción, convertidas en casos
automáticos».

### Páginas 8, 109 y 110 — cifras

Con RN-35 cubierta, aplicar «86,8 %» → «89,5 %» y «cinco reglas» → «cuatro reglas»:
- en el ítem 4 (página 8);
- en el ítem 20 (G-02, página 109): la lista de reglas sin caso pierde RN-35;
- en el ítem 21: tercera fila de «Limitaciones» y punto 2 de «Trabajo futuro», que hoy empieza
  «empezando por RN-35» y pasa a «empezando por RN-28».

### Página 110 — tabla «Limitaciones del trabajo», agregar una fila

| Dos verificaciones dependen de una persona y de un dispositivo: la lectura de los códigos QR impresos con la cámara de un celular y el aspecto del PDF de los reportes en tres lectores. | Se hicieron una vez, el «fecha»; no se repiten en cada corrida como los casos automáticos. |
|---|---|

---

## 24. Rótulo parejo de lo simulado y de lo estimado (todo el documento)

**Regla de JC del 10/11/2026.**
- **Lo simulado:** todo lo simulado dice que es simulado. Un documento que lo dice cumple la
  consigna; el mismo documento sin esa palabra afirma algo que no pasó.
- **El esfuerzo:** toda cifra de esfuerzo dice «estimado». Se escribe «esfuerzo estimado: 14
  horas», nunca «se invirtieron 14 horas».

**Cómo se revisó.** Se recorrió el texto de las 112 páginas buscando: simulad, ficticio,
participante, encuesta, entrevista, horas, puntos, esfuerzo, estimad y datos.

**Lo que ya está bien rotulado y no cambia:**
- minutas «(simuladas)», páginas 39 y 44;
- caso de negocio simulado, páginas 41 y 42;
- reportes con datos ficticios, páginas 17, 19, 20, 21, 43, 84, 85, 100 y 110;
- pago simulado en todo el documento;
- «Tiempo estimado de ejecución», página 56;
- «La estimación total del desarrollo asciende a 78 puntos», página 35.

**El informe de usabilidad** (páginas 90 a 97) lo cubre el ítem 11b. Sus cifras de esfuerzo ya
dicen «estimado» en `docs/usabilidad/INFORME-USABILIDAD.md`, corregido el 10/11.

### Página 9 — Objetivo específico 1, «Cómo medirlo»

**Reemplazar:**

> validado por al menos dos revisiones de parte del “cliente” ficticio (documentadas en
> minutas).

**Por:**

> validado por al menos dos revisiones de parte del cliente ficticio, documentadas en minutas
> simuladas.

### Página 10 — Objetivo específico 4, «Qué lograr»

**Reemplazar:**

> mediante encuestas y entrevistas cortas a los participantes de las pruebas de prototipo.

**Por:**

> mediante un estudio de usabilidad simulado sobre el prototipo, con perfiles de participantes
> definidos y cuestionarios de satisfacción y SUS cuyas respuestas se derivan de mediciones
> sobre la aplicación.

«Cómo medirlo» no cambia: el umbral del 70 % se evalúa sobre el estudio simulado (ítem 19,
advertencia sobre OE-4).

### Páginas 12 a 14 — Registro de interesados

**Título: reemplazar** «Registro de interesados» **por** «Registro de interesados
(simulado)», también en el índice.

**Agregar** debajo del título:

> Registro simulado, como las minutas. Son ficticios:
> - el Interesado 1, Jorge Gastón Gómez, de Ticketera Rosario S.A., el cliente del caso de
>   negocio;
> - el Interesado 6, Carlos Quintana;
> - el Interesado 7, el equipo de soporte.
>
> Los interesados 4 y 5 describen roles (los usuarios finales y la pasarela de pago), no
> personas que hayan participado. Son reales el equipo de desarrollo y el docente evaluador.

**⚠ DATO DE JC — el registro deja afuera a un integrante.**
- El Interesado 2 es solo Máximo Ambrosino. ¿Se agrega «Juan Cruz Riggio (Desarrollador)» con
  los mismos campos, o se cambia el Interesado 2 a «Equipo de desarrollo: Máximo Ambrosino y
  Juan Cruz Riggio»?
- Recomiendo la segunda opción: cambia una línea en lugar de agregar un bloque.

### Páginas 15 a 17 — Cronograma de hitos

**⚠ DATO DE JC.** Las nueve fases dicen «Fecha estimada: x». Una «x» en un entregable final
no es simulación: es un hueco.

**Propuesta:** completar cada fase con su fecha.
- **Fases 1 a 3:** se pueden tomar del historial de revisión (página 6): versiones 0.1 a 0.3,
  de junio y julio de 2025.
- **Fase 4 en adelante:** salen de los commits de 2026. Las fechas ya cumplidas se rotulan
  «Fecha:», y la de entrega, «Fecha estimada:».
- **Inconcluso:** el historial dice 2025 y el código es de octubre y noviembre de 2026. La
  página 6 trae una nota sobre las fechas anteriores, que no revisé en detalle. Hay que
  decidir con qué año queda el cronograma.

**Fase 1, reemplazar:**

> Conformación del equipo (proyecto individual).

**Por:**

> Conformación del equipo (dos integrantes).

**Fase 5, reemplazar:**

> Encuesta de usabilidad aplicada a 2 o 3 usuarios de prueba.

**Por:**

> Estudio de usabilidad simulado sobre el prototipo, con perfiles de participantes definidos.

### Página 21 — Supuestos del proyecto, punto 4

**Reemplazar:**

> 4. Los usuarios de prueba estarán disponibles para realizar simulaciones de compra y
> responder encuestas de usabilidad.

**Por:**

> 4. La evaluación de usabilidad se realiza como estudio simulado: los participantes son
> perfiles definidos y sus resultados se derivan de mediciones sobre el prototipo, sin sesiones
> con personas reales.

### Página 29 — Costo del programa

**Reemplazar:**

> El costo anual de los descuentos, calculado sobre el juego de datos de prueba, asciende a
> $3.758.161.

**Por:**

> El costo anual estimado de los descuentos, calculado sobre el juego de datos simulado,
> asciende a $3.758.161.

### Página 36 — Distribución del esfuerzo

**Título de la tabla: reemplazar** «Distribución del esfuerzo» **por** «Distribución del
esfuerzo estimado», y la columna «Puntos» por «Puntos estimados».

En «Observación sobre el desvío», **reemplazar** «concentra el 62,8 % del esfuerzo» **por**
«concentra el 62,8 % del esfuerzo estimado».

### Página 38 — Riesgo de concentración

**Reemplazar** «es decir el 52,6 % del esfuerzo total» **por** «es decir el 52,6 % del
esfuerzo total estimado».

### Páginas 39 y 44 — minutas repetidas (hallado en la revisión, no es de rótulo)

- **Dos secciones con el mismo título.** El documento tiene «Minutas de reunión (simuladas)»
  en la página 39, con fechas (05/06/2025 y 17/06/2025), y otra vez en la página 44, con
  «Fecha: x» y otro texto. Lo mismo pasa con «Prototipos de interfaz gráfica» (páginas 39 y
  45).
- **⚠ DATO DE JC:** ¿cuál es la versión vigente?
- **Recomendación:** quedarse con la de la página 39, que tiene fechas, y quitar la otra.
  Quitar páginas corre la numeración: se resuelve en el ítem 22.

**Inconcluso, Minuta 2:** sus participantes son «Máximo Ambrosino, Profesor de la cátedra».
- Es una minuta simulada con una persona real.
- El rótulo «(simuladas)» la cubre, pero conviene que el docente no la lea como una reunión
  que se le atribuye.
- **Propuesta:** «Profesor de la cátedra (rol simulado)». Lo decide JC.

### Página 45 — Prototipos

**⚠ A CONFIRMAR CON JC Y MÁXIMO.** El texto dice:

> Los siguientes prototipos fueron desarrollados en Figma y entregados como archivo
> complementario en formato PDF y versión interactiva.

- **Si ese archivo existe y se entrega**, queda como está.
- **Si no existe, reemplazar por:**

> Los siguientes prototipos se elaboraron como bocetos de las pantallas principales y
> representan el flujo completo de compra previsto en esta etapa.

No está en el repositorio ni se lo mencionó en ningún momento.

### Anexos técnicos T01 a T08, si se agregan

Cada anexo distingue dos cosas:
- **lo implementado:** se describe sobre el código real;
- **lo que se presenta como diseño:** va bajo un subtítulo que lo dice, «Diseño propuesto
  (no implementado en el prototipo)».

Las horas que cite un anexo van como «esfuerzo estimado». Ver `ANEXOS-TECNICOS-T01-T08.md`,
re-estimación del 10/11.

---

## 25. Seguridad: RN-18 completa y tres reglas nuevas (páginas 6, 7, 8, 25 o 26, 106 a 111)

> **Condicionado: no se aplica nada de este ítem antes de que Codex entregue la tarea que lo
> respalda, con sus casos pasando.** Es exactamente el error que encontró la auditoría:
> RN-18 promete «un único ingreso» y el sistema no lo controla. Una regla escrita sin
> implementar repite ese error.
>
> | Corrección | Se aplica cuando está hecha | Casos que la respaldan |
> |---|---|---|
> | RN-18 completa | SEG-2 (control de ingreso) | 6 |
> | RN-40 | SEG-1 (cambio de contraseña) | 4 |
> | RN-41 y RN-42 | SEG-3 (el negocio verifica quién llama) | 4 |
>
> Las tareas están en `ENCARGO-SEGURIDAD-BORRADOR.md`. **Si SEG-3 no llega a la entrega,**
> RN-41 y RN-42 no entran como reglas: van a «Limitaciones» y a «Trabajo futuro» (al final
> de este ítem). `docs/02-reglas-negocio.md` las recibe en el mismo momento, no antes.

### Página 25 o 26 — Reglas: «Usuarios y cuentas»

**La página exacta queda a confirmar al abrir el Word:** es la que tiene la tabla «Usuarios
y cuentas», antes de la del ítem 5.

**Agregar** después de RN-05. Siguen la numeración, como RN-38 y RN-39, y RN-27 queda dada de
baja sin reutilizarse. Respetan el criterio de la página 25: verificables, atómicas y sin
mencionar pantallas, controles ni tablas.

| RN-40 | Cambiar la contraseña exige la contraseña actual. Una contraseña actual incorrecta cuenta como intento fallido a los efectos de RN-04, y con la cuenta bloqueada el cambio se rechaza. | A | RF-02 / CU-05 |
|---|---|---|---|
| RN-41 | Un cliente solo consulta y opera sobre sus propias compras, entradas, pagos, pólizas, suscripción e inscripciones en listas de espera. El Administrador puede hacerlo sobre las de cualquier cliente. | A | RF-02 / CU-05 |
| RN-42 | La gestión de eventos, sectores y locales, la consulta de las listas de espera y el control de ingreso son exclusivos del perfil Administrador. Los reportes se rigen por RN-34. | A | RF-12 / CU-02 |

Por qué así:
- **RN-40** no dice «olvidé mi contraseña». Sin envío de correo no hay recuperación segura,
  y por eso el enlace pasa a «Cambiar contraseña» (decisión D-2 del encargo).
- **RN-41** dice «consulta y opera» porque la auditoría encontró las dos cosas: historial,
  entradas y pagos ajenos se pueden leer, y una compra ajena se puede reembolsar.
- **RN-42** deja afuera los reportes para no duplicar RN-34, que ya está escrita y probada
  (CP-35).

### Página 26 o 27 — Reglas: «Compra y armado de paquetes», RN-18

**Reemplazar** el texto de RN-18 (las columnas de tipo y origen no cambian):

> Cada entrada vendida genera un QR único e irrepetible, válido para un único ingreso.

**Por:**

> Cada entrada vendida genera un código QR único e irrepetible, válido para un único
> ingreso: el control de ingreso acepta la primera lectura y rechaza las siguientes, y
> rechaza la entrada que no está confirmada o que corresponde a otro evento.

La regla ya decía «único ingreso». El texto nuevo dice qué se verifica, para que se pueda
probar. **El control de ingreso no se agrega como requerimiento nuevo:** es la verificación
de RN-18, y RN-18 pertenece a RF-07. Si el docente lo lee como funcionalidad nueva, la
alternativa es un RF-14 «Control de ingreso» (Should, 5 puntos estimados), con su fila en la
matriz y en la priorización.

### Página 106 — Matriz: nomenclatura (ítem 17)

En la fila de reglas, «38» → «41». La descripción sigue «Reglas de negocio (RN-27 dada de
baja)». En el criterio de construcción, «las treinta y ocho reglas» → «las cuarenta y una
reglas».

### Página 107 — Matriz principal (ítem 18)

**Reemplazar** tres filas de la tabla del ítem 18. Los casos nuevos se nombran CP-«S1» a
CP-«S14», y Codex les da número correlativo al entregarlos, después del último que exista en
ese momento (la séptima tanda y 6B agregan antes):
- **S1 a S4:** SEG-1;
- **S5 a S10:** SEG-2;
- **S11 a S14:** SEG-3. S11 es el reembolso ajeno, S12 el historial ajeno, S13 eliminar un
  evento siendo cliente y S14 operar sin sesión.

| RF | OE | MoSCoW | Reglas de negocio | CU | Casos de prueba | Hall. |
|---|---|---|---|---|---|---|
| RF-02 | OE-2 | Must | RN-04, RN-05, RN-40, RN-41 | CU-05 | CP-27, CP-28, CP-29, CP-46, CP-55, CP-«S1» a CP-«S4», CP-«S11», CP-«S12», CP-«S14» | — |
| RF-07 | OE-2 | Must | RN-17, RN-18 | CU-01 | CP-32, CP-33, CP-«S5» a CP-«S10» | — |
| RF-12 | OE-2 | Must | RN-06, RN-07, RN-11, RN-38, RN-42 | CU-02 | CP-21, CP-56, CP-60, CP-«S13», CP-«S14» | — |

Las otras diez filas no cambian.

**Agregar** al párrafo que va debajo de la tabla:

> RN-41 y RN-42 son reglas de autorización: valen para todas las operaciones del sistema y
> no para un requerimiento solo. Se ubican en RF-02, que establece la sesión de la que
> dependen, y en RF-12, que reúne las operaciones del Administrador. CP-«S14» figura en las
> dos filas porque verifica las dos reglas a la vez.

### Página 108 — Cobertura (ítem 19)

**Cuánto da.** Las cifras de hoy son 33 de 38 (86,8 %), o 34 de 38 (89,5 %) si PA-19 queda como
caso de RN-35 (ítem 23).

| Qué entró antes de la entrega | Reglas | Con caso, sin RN-35 | Con caso, con RN-35 |
|---|---|---|---|
| **SEG-1, SEG-2 y SEG-3 (lo previsto)** | **41** | **36 (87,8 %)** | **37 (90,2 %)** |
| SEG-1 y SEG-2; SEG-3 no | 39 | 34 (87,2 %) | 35 (89,7 %) |
| Ninguna | 38 | 32 (84,2 %) | 33 (86,8 %) |

**Por qué la última fila baja:**
- **hoy el documento cuenta RN-18 como cubierta por CP-32,** y CP-32 solo prueba que el
  código es único;
- **si no se implementa el ingreso,** lo honesto es sacarla de las cubiertas, o dejarla con
  una nota de «cubierta en parte».

**Las reglas sin caso no cambian:** RN-07, RN-26, RN-28 y RN-33, más RN-35 si PA-19 no
entra. Ninguna de las tres nuevas queda sin caso.

**Con lo previsto (primera fila, sin RN-35),** reemplazar en el ítem 19:
- **en la tabla «Análisis de cobertura», la fila de reglas:**

| Reglas de negocio verificadas por caso de prueba | 41 | 36 | 5 reglas | 87,8 % |
|---|---|---|---|---|

- **la de «Reglas de negocio con origen declarado»:** 38 → 41, cubiertas 41, 100 %;
- **en el párrafo «Los sesenta y un casos verifican…»:**
  - «treinta y tres reglas distintas sobre un total de treinta y ocho» → «treinta y seis
    reglas distintas sobre un total de cuarenta y una»;
  - «86,8 %» → «87,8 %»;
  - en la lista de reglas cubiertas se agregan RN-40 a RN-42;
  - «Las diecinueve reglas que rechazan una operación» → «Las veintidós», porque las tres
    nuevas rechazan operaciones. **A recontar** si PA-19 o la séptima tanda cambian esa cuenta.
- **en la tabla «Casos de prueba incorporados en la versión 2.5»,** las filas CP-«S1» a
  CP-«S14», con el formato del ítem 19. Las trae el reporte de Codex de cada SEG.

**En «Lectura del cuadro»,** «treinta y tres de treinta y ocho» → «treinta y seis de
cuarenta y una».

### Página 109 — Brechas (ítem 20)

**En la oración inicial,** «seis brechas» → «siete brechas», y se agrega al final: «G-07,
nueva, de seguridad, quedó resuelta».

**Agregar** la fila:

| G-07 | La revisión de seguridad de la versión 2.5 encontró que RN-18 prometía un único ingreso por entrada que el prototipo no controlaba, que el cambio de contraseña no exigía la contraseña actual y que las operaciones de negocio no verificaban el perfil ni la identidad de quien las invoca. | Alta | Resuelta | Se implementó el control de ingreso con registro atómico de la primera lectura, el cambio de contraseña pasó a exigir la actual y el negocio verifica la sesión en cada operación. Se incorporaron RN-40 a RN-42 y se completó RN-18, con catorce casos de prueba (CP-«S1» a CP-«S14»). |
|---|---|---|---|---|

**En la fila G-02,** «La cobertura de reglas sube al 86,8 % (33 de 38)» → «sube al 87,8 %
(36 de 41)».

**Agregar** debajo de la observación sobre G-05:

> **Observación sobre G-07.** Como G-05, recorre la cadena al revés: RN-18 figuraba en la
> matriz como cubierta porque su caso verificaba la mitad de la regla. Un caso asociado a una
> regla no garantiza que la verifique completa; por eso, desde esta versión, cada caso de la
> tabla de cobertura nombra lo que comprueba.

### Páginas 110 y 111 — Conclusiones (ítem 21)

- **«Sobre las reglas de negocio»:** «treinta y ocho» → «cuarenta y una».
- **«Limitaciones», tercera fila:** «86,8 %» → «87,8 %».
- **«Limitaciones», agregar dos filas:**

| No existe recuperación de contraseña por correo. | Una contraseña olvidada la restablece el Administrador; el usuario solo puede cambiarla conociendo la actual (RN-40). |
|---|---|
| La aplicación de escritorio accede a la base con la cuenta de Windows del usuario. | Las verificaciones de perfil del negocio (RN-34, RN-41, RN-42) protegen la operación desde la aplicación; en producción tienen que acompañarse de permisos en la base o de un servicio intermedio. |

### Páginas 6, 7 y 8 — Historial y resumen ejecutivo (ítems 2, 3 y 4)

Se actualizan junto con el ítem 23, al cerrar:
- **las cifras:** 38 → 41 reglas, y la cobertura que resulte de la tabla de arriba;
- **la fila del historial de la versión 2.5** suma: «Revisión de seguridad: control de
  ingreso por QR, RN-40 a RN-42 y brecha G-07».

### Si SEG-3 no llega a la entrega

- **Reglas:** se agrega solo RN-40, y RN-18 se completa.
- **Cifras:** 39 reglas.
- **G-07 queda «Parcialmente resuelta»,** con la acción: «queda pendiente que el negocio
  verifique la sesión en cada operación».
- **«Trabajo futuro» suma un punto:**

> Verificar en la capa de negocio el perfil y la identidad de quien invoca cada operación,
> y formalizarlo como reglas: un cliente opera solo sobre lo suyo y la administración es
> exclusiva del perfil Administrador.

---

## Datos que necesito de JC

**Las cinco correcciones del 10/11 que esperan un dato de JC** (todas en el ítem 24). Quedan
anotadas y **en pausa** por decisión de JC del 10/11: la prioridad pasó a que el sistema
funcione, sea seguro y se vea bien.

1. **Registro de interesados (páginas 12 a 14).** Falta Juan Cruz Riggio: ¿se agrega como
   interesado aparte o se cambia el Interesado 2 a «Equipo de desarrollo»?
2. **Cronograma de hitos (páginas 15 a 17).** Las nueve fases dicen «Fecha estimada: x».
   Faltan las fechas, y decidir si el año es 2025 o 2026.
3. **Minutas y prototipos repetidos (páginas 39, 44 y 45).** ¿Qué versión queda?
4. **Minuta 2.** Es una minuta simulada con el profesor real como participante: ¿se agrega
   «(rol simulado)»?
5. **Prototipos en Figma (página 45).** El texto dice que se entregaron como archivo
   complementario. ¿Existe ese archivo? Si no existe, la frase se reemplaza.

Para el ítem 1. Pasalos en este formato, una línea por dato:

```
Comisión: <como figura en el sistema de la UAI, por ejemplo 5TN>
Turno: <Mañana | Tarde | Noche>
Legajo Ambrosino, Máximo: <número>
Legajo Riggio, Juan Cruz: <número>
Año de la entrega: <2026, salvo que la cátedra diga otro>
Fecha del historial: <dd/mm/aaaa, el día que se pasan estos cambios al Word>
```

Una consulta más: la carátula dice «Maximo Ambrosino», sin tilde, y el encabezado dice
«Ambrosino, Máximo», con tilde. ¿Cuál es la forma correcta?
