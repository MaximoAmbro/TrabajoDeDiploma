# Fuente editable de los documentos

El informe de 112 páginas y el Examen Parcial 2 **no se escribieron en Word**: se componen
por código con ReportLab, a partir de los guiones de esta carpeta. Esto es lo editable.
El `.pdf` de `Documentacion/` es la salida, no el original.

## Por qué está hecho así

El formato del informe reproduce el de un documento exportado desde Google Docs: A4 de
596 × 842 pt, márgenes de 72 y 70 pt, cuerpo Times New Roman de 11 pt, interlineado de
14,55 pt. `ea_style.py` tiene esas medidas extraídas del PDF original y las respeta, de
modo que una página regenerada es indistinguible de una original.

La ventaja práctica es que el documento se versiona, se compara entre revisiones y se
regenera completo en segundos. La desventaja es que se edita escribiendo Python, no
arrastrando el cursor.

## Qué hace falta

    python -m pip install reportlab pypdf pdfplumber pillow
    # para los diagramas del parcial 2, ademas:
    python -m pip install graphviz        # y Graphviz instalado (comando dot en el PATH)

Las fuentes Liberation (métricas idénticas a Times New Roman y Arial) tienen que estar
instaladas. En Linux vienen con `fonts-liberation`; en Windows se bajan de
liberationfonts.org. Las rutas están al principio de `ea_style.py`: si tu instalación las
deja en otro lado, es lo único que hay que tocar.

## Cómo regenerar

### El informe completo

    cd informe
    python p15_rn.py          # cada guion produce su PDF parcial
    python p16_moscow.py
    ...
    python ensamblar.py       # une los parciales y numera

### El Examen Parcial 2

    cd parcial2
    python iconos.py          # 1. iconos UML (actor, boundary, control, entity)
    python d_casos.py         # 2. los seis diagramas
    python d_dominio.py
    python d_arq.py
    python d_robustez.py
    python d_sec1.py
    python d_sec2.py
    python d_form.py
    python armar.py           # 3. compone el PDF final

Los `.png` ya generados están incluidos, así que si solo vas a corregir texto podés correr
`armar.py` directamente y saltearte los diagramas.

## Mapa de archivos

| Archivo | Qué produce |
|---|---|
| `ea_style.py` | Motor de composición. No produce nada por sí solo. |
| `informe/p13_a02.py` | Anexo A02: reportes y exportación a PDF |
| `informe/p14_instructivo.py` | Instructivo del requerimiento core |
| `informe/p15_rn.py` | Reglas de negocio RN-01 a RN-39 |
| `informe/p16_moscow.py` | Priorización MoSCoW |
| `informe/p17_trazabilidad.py` | Matriz de trazabilidad |
| `informe/p17a_requerimientos.py` | Definición de requerimientos |
| `informe/p18_usabilidad.py` | Informe de pruebas de usabilidad |
| `informe/p19_descuento.py` | Lógica del descuento por nivel |
| `informe/front_back.py` | Portada, índice, historial, conclusiones, bibliografía |
| `informe/ensamblar.py` | Une los parciales y aplica encabezado y numeración |
| `parcial2/doc1.py` a `doc3.py` | Texto del parcial, dividido en tres bloques |
| `parcial2/d_*.py` | Un diagrama cada uno |
| `parcial2/sec.py` | Renderizador de diagramas de secuencia |
| `parcial2/armar.py` | Compone el PDF del parcial |

## API de `ea_style.Doc`

    d = Doc("salida.pdf")
    d.h1(texto)                      # titulo centrado, 20 pt
    d.h2(texto)  ·  d.h3(texto)      # subtitulo y rotulo
    d.p(texto)                       # parrafo justificado
    d.rich([(texto, "b"), (texto, "r")])   # parrafo con estilos: r, b, i, bi
    d.bullet([...])  ·  d.numbered(n, [...])
    d.table(encabezados, filas, widths=[...], aligns=["l","c","r"])
    d.code([lineas])  ·  d.pre([lineas])
    d.image(ruta, caption)           # PNG escalado al ancho util
    d.pagina_apaisada(ruta, caption, titulo)   # pagina A4 horizontal para una figura ancha
    d.figure_slot(caption, height)   # recuadro punteado para una captura pendiente
    d.page_break()  ·  d.space()  ·  d.keep(pts)
    d.save()

`keep(pts)` evita que un título quede huérfano al pie: reserva ese alto y, si no entra,
salta de página antes de escribir.

## Una diferencia entre este motor y el que generó el informe publicado

`ea_style.py` incorpora tres cambios hechos para el Examen Parcial 2:

1. `image()` — inserta un PNG real. Antes solo existía `figure_slot()`, que dibuja el
   recuadro punteado de una captura pendiente.
2. `pagina_apaisada()` — emite una página horizontal para una figura ancha y vuelve a
   vertical. Se usa en los diagramas de secuencia.
3. `rich()` ya no deja un espacio antes de la puntuación. Antes, `[("texto","b"),
   (", y sigue","r")]` componía «texto , y sigue».

El tercero **cambia el corte de líneas** respecto del informe publicado, así que al
regenerarlo esperá diferencias de paginación. No es un error: el resultado es mejor.
Si necesitaras reproducir el PDF v2.4 exactamente, hay que revertir ese cambio en `rich()`.

## Pendiente conocido

- Las cuatro capturas del Instructivo (páginas 52 y 53) siguen como `figure_slot()`.
- El anexo A02 tiene correcciones documentadas en `docs/05-reportes.md` que todavía no se
  aplicaron al fuente: tres errores en el SQL de RPT-01, dos de redondeo y el estado
  «Agotado» de RPT-05.
- RN-35 se reescribió en `docs/02-reglas-negocio.md` (prioridad al suscriptor, congelada
  al inscribirse) pero el PDF publicado todavía dice «estrictamente orden de inscripción».

## Sobrantes a borrar

`parcial2/` arrastra cuatro PNG de verificación que no usa ningún guion y se pueden
eliminar: `chk_cliente.png`, `chk_dom.png`, `chk_nota.png` y `actor.png` (este último
quedó reemplazado por `ac_cliente.png` y `ac_pasarela.png`, que llevan el rótulo incluido).
