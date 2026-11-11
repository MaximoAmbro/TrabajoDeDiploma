# Plan del documento entregable

Claude Code, a pedido de JC. La prioridad vuelve a ser el documento: el docente pide la
documentación de requerimientos presentable para la próxima reunión, y es lo que más pesa en
la nota.

Todas las horas son **estimadas**. Este plan no aplica ninguna corrección: las aplica JC
cuando apruebe el método.

---

## 1. Con qué fuentes contamos

Relevado en el repositorio.

| Pieza | Qué es | ¿Sirve para editar? |
|---|---|---|
| `Documentacion/EventAura_v2.pdf` | El documento final, versión 2.4, 112 páginas, armado con pypdf | **Es la única fuente.** Texto real, no escaneado |
| `Anexo_A02_Reportes.docx`, `Instructivo_Compra_Paquetes.docx` (raíz) | Borradores viejos con contenido que nunca se construyó | No |
| `Documentacion/EventAura.pdf` | Una versión anterior, de 2,6 MB | No: es más vieja que la 2.4 |
| `Documentacion/Entrega 1, 2 y 3.pdf`, `Carpeta modelo.pdf`, `Requermientos Proyecto 2023.pdf` | Consignas y entregas parciales | Solo como referencia |
| Las partes originales y el script que armó el PDF | — | **No están** en el repositorio ni en la PC |

**Herramientas que hay en la máquina** (no se instaló nada):
- **Word 365:** ahora está activado y acepta automatización. Abre un `.docx` y lo guarda en
  PDF sin problemas. **Abrir un PDF en Word se cuelga** cuando lo hace la automatización: el
  aviso «Word convertirá el PDF…» espera un clic que nadie da. A mano, JC sí podría hacerlo;
  no se probó.
- **pdf2docx y pymupdf:** en la carpeta temporal de Claude Code, de la prueba del 04/11.
- **LibreOffice:** no está instalado.

> **Restricción de JC (11/11): mientras Codex corre pruebas de interfaz, no se abre ninguna
> aplicación con interfaz, ni siquiera Word oculto por automatización.** Una ventana que
> roba el foco hace fallar una prueba sin que haya defecto.
>
> **El método depende de Word en un solo paso:** pasar el `.docx` corregido a PDF. Convertir,
> corregir, limpiar, reinsertar y renumerar son bibliotecas de Python, sin interfaz. Por eso:
> - **o el paso a PDF se hace solo cuando Codex no está probando**, todo junto al final de cada
>   tanda de páginas;
> - **o se instala LibreOffice y se usa sin interfaz** (`soffice --headless --convert-to pdf`).
>   Necesita la autorización de JC, y una prueba nueva de fidelidad, porque dibuja distinto
>   que Word.
>
> **La prueba del 11/11 usó Word por automatización entre las 13:59 y las 14:11**, antes de que
> llegara la restricción.

## 2. La prueba de conversión (11/11)

**Las tres páginas:**
- **la 110,** texto corrido con una tabla: conclusiones, ítem 21;
- **la 107,** la matriz principal, que es la tabla más compleja: ítem 18;
- **la 53,** con figuras: las capturas 2 a 4 del instructivo, ítem 9.

**Las correcciones aplicadas son las reales de la lista.**

**El circuito:**
1. separar la página del PDF (pymupdf);
2. convertirla a `.docx` (pdf2docx);
3. aplicar las correcciones;
4. limpiar el `.docx` (ver abajo);
5. guardar en PDF con Word;
6. reinsertar en una **copia** del PDF y renumerar las páginas siguientes.

**El original no se tocó.**

### Resultado: funciona

- **Texto y tablas:** el contenido sale entero. Las tablas conservan forma, sombreado y bordes.
- **Márgenes y posiciones:** quedan dentro de 1 a 4 puntos del original.
- **Tipografía:** pasa de Liberation Serif a Times New Roman. Liberation Serif no está
  instalada en Windows, y las dos tienen las mismas medidas: a simple vista no se distingue,
  pero los metadatos del PDF dicen otra fuente.
- **Encabezado y pie:** en la limpieza pasan a ser el encabezado y el pie reales de Word, con
  el número de página como campo. Así, una página que crece y pasa a ocupar dos lleva
  encabezado y número correctos sola.
- **Figuras:** entran sin problema.
- **Renumeración:** el script reescribe el número impreso de todas las páginas siguientes. En
  la prueba fueron 62.

### Qué se rompe o se pierde, y cómo se arregla

| Problema | Arreglo | ¿Se automatiza? |
|---|---|---|
| Las filas de las tablas tienen alto **exacto**: el texto nuevo queda cortado | Pasarlas a alto mínimo | Sí, ya está en el script |
| El encabezado queda como texto suelto en el cuerpo, y las páginas que crecen salen sin él | Moverlo al encabezado de Word | Sí, ya está |
| pdf2docx escribe el ancho de letra con decimales, y Word lo dibuja espaciado («U N I V E R S I D A D») | Borrar ese atributo | Sí, ya está |
| **Dos palabras pegadas en una unión de renglón** («permitióestablecer», página 110) | Corregirlas a mano | **No: hay que leer cada página convertida** |
| Una celda angosta corta el último caracter («CP-3» por «CP-30») | Ensanchar la columna | No, se ajusta a mano |
| «Año: 2025» salta al renglón siguiente en el encabezado | Ajustar la sangría derecha | Sí, una vez para todas |
| El título de una tabla queda solo al pie de la página (110) | «Conservar con el siguiente» | A mano |
| **Crecen tres páginas:** la 53 (figuras legibles), la 107 (casos de prueba en la matriz) y la 110. El PDF pasa de 112 a 115 páginas | Se renumera con el script; **el índice (páginas 2 a 5) se rehace**, que ya era el ítem 22 | Renumerar sí; el índice, no |
| Las figuras en el tamaño del espacio reservado (5,9 cm de alto) no se leen | Agrandarlas, a costa de una página más | Decisión de diseño |

### Cuánto tardó

| Tramo | Tiempo |
|---|---|
| Separar, convertir, aplicar las correcciones de las tres páginas, guardar con Word y armar la copia | **unos 6 minutos de trabajo**, más 5½ minutos perdidos con Word colgado al abrir el PDF |
| Segunda vuelta: limpieza de encabezado, pie, filas y espaciado, y renumeración | unos 3 minutos |
| **Total, de la primera página separada a la copia de prueba** | **unos 12 minutos de reloj** |

**Lo que este dato no incluye:**
- **escribir las correcciones:** ya estaban redactadas en la lista;
- **la revisión de JC de cada página contra el original:** es lo que la prueba no puede medir,
  y donde se van a encontrar los problemas de la tabla que no se automatizan.

### Re-estimado

- **Por página:** de 4 a 7 minutos de Claude Code, más 3 a 5 de revisión de JC. **Son 7 a 12
  minutos por página.**
- **Una sola vez:**
  - el script generalizado;
  - reemplazar comisión, turno, legajos y año en el encabezado de las 112 páginas (ítem 1);
  - el índice;
  - el armado final y una lectura de corrido.

  **Son 1,5 a 2 horas.**
- **Las ~41 páginas del plan anterior: 6 a 10 horas** (antes, 8 a 12).
- **La lista completa:** suma las páginas de los ítems 24 y 11b (ver sección 3). **Son 9 a 16
  horas, de las que 3 a 4 son de revisión de JC.**

**Archivos de la prueba**, para que JC los mire (fuera del repositorio):
- `…\scratchpad\conv\EventAura_v2-PRUEBA-CONVERSION.pdf`: la copia completa con las tres
  páginas reinsertadas;
- `…\scratchpad\conv\final-p53.png`, `final-p107.png` y `final-p110.png`: el original a la
  izquierda y el resultado a la derecha.

---

## 3. Las correcciones por lo que valen en la nota

**El criterio:**
- **imprescindible:** lo que un docente que evalúa requerimientos marca, como errores de
  contenido, inconsistencias entre secciones o lo que el documento afirma y no es así;
- **importante:** lo que hace que se vea un trabajo cuidado;
- **prescindible:** lo que se deja si el tiempo aprieta.

**Puede cambiar** cuando llegue el documento de referencia del compañero (Parte 3 del encargo).

### Imprescindibles: 4 a 6,5 horas (incluye el costo de una sola vez)

| Ítem | Pág. | Por qué es imprescindible |
|---|---|---|
| 1 | 1 y encabezado de todas | «Comisión: «completar»», legajos vacíos y «Año: 2025» en **las 112 páginas**: es lo primero que se ve. ⚠ **Necesita los datos de JC**, y decidir cómo figura Máximo ahora que quedó fuera del proyecto |
| 3, 4 | 7 y 8 | El resumen ejecutivo dice 36 reglas y 22,2 % de cobertura. El resto del documento corregido dice otra cosa |
| 4b | 24 | RNF-03 promete compatibilidad con navegadores: el sistema es de escritorio |
| 5, 6 | 26 y 27 | Las reglas son el centro de la documentación de requerimientos. RN-06, RN-29 y RN-35 no coinciden con el sistema; faltan RN-38 y RN-39 |
| 7 | 36 a 38 | RF-09 es Could en la priorización y Won't en la matriz |
| 8 | 50 | Afirma que los servicios, el seguro y el descuento no están implementados. Lo están |
| 9 | 52 y 53 | Cuatro «Espacio reservado para captura de pantalla» en el requerimiento core |
| 11 | 83 | CU-07 contradice RN-35 |
| 13, 14 | 100 y 101 | Un ejemplo de A02 marca «Agotado» eventos que por RN-10 no lo están. Son dos páginas y salen baratas |
| 17 a 21 | 106 a 111 | Matriz, cobertura, brechas y conclusiones: es lo que más mira quien evalúa trazabilidad. Hoy la matriz dice que 10 de 13 requerimientos no tienen pruebas |
| 22 | 2 a 5 | El índice: obligatorio en cuanto cualquier página se corre |
| 24, solo la parte de lo simulado | varias | La regla de JC: lo simulado dice «simulado». Incluye el «(simulado)» del título del informe de usabilidad (11b) |

### Importantes para la presentación: 4,5 a 8 horas

| Ítem | Pág. | Qué |
|---|---|---|
| 11b | 90 a 97 | Reemplazar el informe de usabilidad por el de la versión 2.5. Son 8 páginas de texto nuevo, conviene escribirlas en Word con el estilo del documento en lugar de convertirlas: 2 a 3 h. **Va atado:** si no se hace, los ítems 18, 19 y 21 conservan las cifras viejas de usabilidad (SUS 74,5) en lugar de 78,8 |
| 2 | 6 | Historial de revisión con la versión 2.5 |
| 12 | 99 | Filtros y fórmula del catálogo de reportes |
| 24, el resto | 9, 10, 12 a 17, 21, 29, 39, 44 y 45 | «Estimado» en las cifras de esfuerzo y los rótulos que faltan. **Cinco de ellos esperan un dato de JC** (ver el final de `CORRECCIONES-DOCUMENTO.md`) |
| 23 | 6 a 8 y 108 a 110 | Las pruebas de interfaz convertidas en casos. **Condicionado** al reporte de la séptima tanda de Codex |
| 25 | 6 a 8, 25 o 26 y 106 a 111 | Seguridad: RN-18 completa y RN-40 a RN-42. **Condicionado** a que Codex entregue SEG-1 a SEG-3 con sus pruebas pasando. Si no llegan, esas filas no se escriben. Las páginas ya están convertidas por los ítems 17 a 21, así que suma poco: 0,5 a 1 h |

### Prescindibles: 0,5 a 1,5 horas

| Ítem | Pág. | Por qué se puede dejar |
|---|---|---|
| 15 | 102 y 103 | Diseño técnico de A02: «logotipo» por «nombre como texto» y una línea de un diagrama |
| 16 | 104 y 105 | El SQL de RPT-01 en el anexo no corre contra el esquema. Es un error de contenido, pero está en código dentro de un anexo, y el docente revisa el código en el repositorio. Son bloques en tipografía monoespaciada, que es lo que peor convierte |
| 10 | 54 | Sin cambios |

### En resumen

| Hasta dónde | Horas estimadas | De ellas, revisión de JC |
|---|---|---|
| **Mínimo digno** (imprescindibles) | **4 a 6,5** | 1,5 a 2 |
| Lo que querríamos (más las importantes) | 8,5 a 14,5 | 3 a 4 |
| Todo | 9 a 16 | 3 a 4 |

**Orden, sin fechas:**
1. JC aprueba el método mirando la copia de prueba.
2. JC pasa los datos del ítem 1.
3. Script generalizado.
4. Imprescindibles, de la primera página a la última.
5. Índice y armado.
6. Revisión de JC.
7. Importantes, en el mismo orden.
8. Los condicionados (23 y 25), cuando Codex entregue.
