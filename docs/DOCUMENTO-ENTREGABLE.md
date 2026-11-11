# Estado del documento entregable

> **Desactualizado en el plan. El vigente es [PLAN-DOCUMENTO.md](PLAN-DOCUMENTO.md) (11/11):**
> - Máximo quedó fuera del proyecto y el fuente no va a llegar;
> - Word ya está activado;
> - el método de convertir y reinsertar páginas se probó sobre tres páginas y funciona.
>
> Lo de abajo queda como historia del relevamiento.

Relevado por Claude Code el 04/11/2026.

## Qué hay y qué falta

| Pieza | Estado |
|---|---|
| `Documentacion/EventAura_v2.pdf` (versión 2.4, 112 páginas) | Es lo único que hay del documento final. No es editable |
| El fuente editable de ese PDF | **No está en el repositorio ni en la PC de JC.** Se buscó en C: y D:, OneDrive, Descargas, temporales de Word y el historial de git, incluidas las ramas `origin/Ambro` y `origin/JC`. JC se lo pidió a Máximo |
| `Anexo_A02_Reportes.docx` e `Instructivo_Compra_Paquetes.docx`, en la raíz del repo | **Borradores viejos, no el fuente.** Tienen otro contenido que el PDF: tablas inventadas en dólares, niveles «Bronce 0-5 compras», Oro al 10 %, impuestos del 10 %, envío de mails. Editarlos no cambia el entregable |
| Correcciones a aplicar | Todas en una sola lista: [CORRECCIONES-DOCUMENTO.md](CORRECCIONES-DOCUMENTO.md) |
| Figuras del instructivo (páginas 52 y 53) | Listas en `docs/capturas/instructivo/`. Se regeneran después de T-21 (semilla alineada con el ejemplo) |

## Cómo se armó el PDF

- **Productor:** `pypdf`, una librería de Python que une y edita PDF. No es Word.
- **Autor en los metadatos:** «Maximo Ambrosino - Juan Cruz Riggio».
- **Tipografías:** Liberation Serif y Liberation Sans en casi todo el cuerpo. Son las que
  usa LibreOffice cuando no tiene Times New Roman y Arial, típico de una conversión en Linux
  o en un servidor. La portada usa EB Garamond; el código, Roboto Mono; algunas páginas,
  Arial y Times. Las páginas de reportes tienen marcas de iText.
- **Conclusión:** el PDF se compuso con un script a partir de varias partes, probablemente
  documentos convertidos con LibreOffice y unidos con pypdf. Puede que no exista un único
  Word de 112 páginas, y por eso hay que pedirle a Máximo tanto las partes como el script.

## Composición del PDF (medida página por página)

- **Texto:** todo es texto real, no hay páginas escaneadas.
- **Tablas:** se detectan unas 73.
- **Imágenes:** hay 20 rasterizadas, que son los diagramas: páginas 47 a 49 (prototipos) y 71
  a 89 (casos de uso, clases, secuencia, estados, robustez, dominio, actividad y DER). Ya
  son imágenes en el PDF, así que una conversión las conserva tal cual.
- **Encabezado y pie:** iguales en las 112 páginas, con «Comisión: «completar»», «Turno:
  «completar»», «Año: 2025» y los legajos sin completar.

## Plan B: reconstruir un editable desde el PDF

Se usa solo si Máximo no aparece con el fuente.

### Prueba hecha

Se convirtió una muestra de 8 páginas: portada, dos tablas de reglas, un diagrama y las
cuatro páginas de la matriz (1, 26, 27, 88 y 106 a 109). Se usó `pdf2docx`, porque Word
todavía no tenía la licencia activada y no permite convertir por automatización. Resultado,
en 2,6 segundos:

- **Tablas:** las 12 tablas reales salieron con la forma correcta. La matriz principal quedó
  de 14 × 7, las de reglas de 4 columnas, la de cobertura de 9 × 5 y la de brechas de 5 × 5.
  El contenido de cada celda está entero.
- **Diagrama:** quedó como imagen, igual que en el PDF.
- **Encabezado de página:** cada uno salió como una tabla suelta de 1 × 2 en el cuerpo, 8
  tablas basura en 8 páginas. Hay que borrarlas y rehacer el encabezado como encabezado real
  de Word.
- **Pie:** quedó como texto en el cuerpo, con el número de página escrito a mano.
- **Estilos:** no hay estilos de título, todo es formato directo. Sin estilos, Word no puede
  regenerar el índice.

Cuando Word esté activado, conviene repetir la muestra con su conversión propia (abrir el
PDF en Word) y comparar. Suele dar un resultado parecido.

### Fidelidad esperada

| Elemento | Fidelidad | Trabajo manual |
|---|---|---|
| Texto | Completa | Revisar cortes de párrafo y guiones |
| Tablas (unas 73) | Buena en forma y contenido | Rehacer sombreados y anchos; volver a unir las que el PDF parte entre dos páginas |
| Matriz de trazabilidad | Buena, verificada en la muestra | Igual que las tablas |
| Diagramas (20) | Exacta, como imagen | Ninguno. No serán editables, pero tampoco lo son en el PDF |
| Bloques de código (páginas 102 a 105) | Texto correcto; puede cambiar la tipografía | Aplicar una monoespaciada |
| Encabezado y pie | Mala: quedan como basura en el cuerpo | Borrar y rehacer con encabezado y pie de Word |
| Índice (páginas 2 a 5) | Queda como texto fijo | Aplicar estilos de título y regenerarlo |
| Portada | Aceptable | Revisar la tipografía (EB Garamond) |

### Opciones y costo

| Opción | Qué es | Horas realistas | A favor | En contra |
|---|---|---|---|---|
| **B1. Reconstrucción completa** | Convertir las 112 páginas y dejarlas como un Word limpio | **13 a 19 h**: conversión 0,5; encabezados y pies 1,5; estilos de título e índice 2 a 3; revisión de 73 tablas 3 a 4; listas y código 1 a 2; revisión visual de 112 páginas contra el PDF 3 a 4; aplicar las correcciones 2 a 3 | Queda un editable de verdad para cualquier cambio posterior, por ejemplo devoluciones del docente | Es la opción más cara, y la mayor parte del trabajo es en páginas que no cambian |
| **B2. Solo las páginas que cambian (recomendada)** | Convertir unas 25 páginas (1, 6 a 8, 26 y 27, 36 a 38, 50, 52 a 54, 83, 99 a 112 y el índice), corregirlas, exportarlas y reemplazarlas en el PDF original con pypdf, que es como se armó. Para el encabezado y el pie de las demás páginas (comisión, turno, legajos y año), un script que reemplace solo esos textos en el PDF | **6 a 9 h**: conversión y limpieza de 25 páginas 3 a 4; aplicar las correcciones 2 a 3; script de encabezado y pie, armado final y control de numeración 1 a 2 | Tres veces más barata. Las páginas que no cambian quedan idénticas al original | Las páginas 26 y 27 crecen (dos reglas nuevas y tres más largas): hay que hacerlas entrar compactando, o se corren todas las siguientes. El catálogo CP-11 a CP-43 agrega 2 o 3 páginas, pero va hacia el final (106 a 112), así que solo se corren esas y el índice. Cualquier cambio futuro vuelve a requerir este procedimiento |
| B3. Solo edición del PDF | Tapar y reescribir texto sobre el PDF, sin convertir | No alcanza | — | No permite agregar filas a tablas ni páginas nuevas |

### Recomendación

1. **Esperar el fuente de Máximo**, que es por lejos lo mejor: con el fuente, aplicar la lista
   lleva unas 3 horas.
2. **Si no aparece en pocos días, B2.** Cubre toda la lista de correcciones al menor costo.
3. **B1 solo si** el docente pide cambios extensos después de leer el documento, o si se
   quiere un editable para el futuro.

Fijar una fecha límite para la respuesta de Máximo (por ejemplo, el 08/11/2026) deja tiempo
para B2 sin presión.
