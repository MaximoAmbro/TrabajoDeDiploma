"""Renderiza los diagramas de docs/diagramas/fuentes y verifica que se lean en la página.

Uso:  py -3 tools/Diagramas/Renderizar.py [nombre ...]
      (sin nombres, todos los .puml que no empiezan con «_»)

Para cada diagrama:
  1. PlantUML genera un SVG (para medir) y un PNG a 300 dpi (para pegar en el documento);
  2. del SVG se toma el ancho, el alto y la letra más chica;
  3. se calcula el ancho de inserción con el que esa letra queda en LETRA_MINIMA puntos;
  4. si ese ancho no entra en el área de texto de una página A4 vertical (ANCHO_MAX x ALTO_MAX),
     el diagrama NO se puede leer impreso y el script lo marca como FALLA.

El tamaño que informa es el que hay que usar al insertar la imagen: más chico, la letra baja
de LETRA_MINIMA; más grande, no entra en la página.

Requisitos: Java 21 y plantuml.jar (por defecto en D:\\jc-tools\\plantuml; se puede cambiar con
las variables PLANTUML_JAVA y PLANTUML_JAR). No usa Graphviz: los diagramas declaran
«!pragma layout smetana», el motor de diagramación que trae PlantUML.
"""
import glob
import os
import re
import subprocess
import sys
import xml.etree.ElementTree as ET

RAIZ = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
FUENTES = os.path.join(RAIZ, "docs", "diagramas", "fuentes")
SALIDA = os.path.join(RAIZ, "docs", "diagramas")

JAVA = os.environ.get("PLANTUML_JAVA") or next(
    iter(glob.glob(r"D:\jc-tools\plantuml\jdk-*-jre\bin\java.exe")), "java")
JAR = os.environ.get("PLANTUML_JAR", r"D:\jc-tools\plantuml\plantuml.jar")

PT_POR_CM = 72 / 2.54
LETRA_MINIMA = 8.5            # pt; el cuerpo del documento es de 12 pt, las figuras no bajan de 8
ANCHO_MAX = 16.0              # cm: área de texto de la página A4 del documento
ALTO_MAX = 23.0               # cm: alto útil, descontando encabezado, pie y título de la figura


def medir(svg):
    raiz = ET.parse(svg).getroot()
    ancho = float(re.match(r"[\d.]+", raiz.get("width")).group())
    alto = float(re.match(r"[\d.]+", raiz.get("height")).group())
    textos = [t for t in raiz.iter() if t.tag.endswith("text") and t.get("font-size")]
    # PlantUML dibuja sus advertencias (p. ej. un skinparam obsoleto) dentro del diagrama.
    if any(t.get("font-family") == "monospace" for t in textos):
        raise RuntimeError(f"{svg}: PlantUML dejó una advertencia dentro del diagrama")
    return ancho, alto, min(float(t.get("font-size")) for t in textos)


def renderizar(puml):
    for formato, extra in (("-tsvg", []), ("-tpng", ["-Sdpi=300"])):
        r = subprocess.run([JAVA, "-Djava.awt.headless=true", "-jar", JAR, "-charset", "UTF-8", formato,
                            *extra, "-o", SALIDA, puml], capture_output=True, text=True)
        if r.returncode != 0:
            raise RuntimeError(r.stderr or r.stdout)


def main(nombres):
    pumls = [os.path.join(FUENTES, n if n.endswith(".puml") else n + ".puml") for n in nombres] or \
        sorted(p for p in glob.glob(os.path.join(FUENTES, "*.puml")) if not os.path.basename(p).startswith("_"))
    fallas = 0
    print(f"{'diagrama':34} {'px':>11} {'letra':>5}  {'insertar a':>16}  {'letra impresa':>13}")
    for puml in pumls:
        nombre = os.path.splitext(os.path.basename(puml))[0]
        renderizar(puml)
        ancho, alto, letra = medir(os.path.join(SALIDA, nombre + ".svg"))
        # Ancho de inserción para que la letra más chica quede en LETRA_MINIMA.
        cm = LETRA_MINIMA * ancho / letra / PT_POR_CM
        cm = max(cm, min(ANCHO_MAX, ancho / PT_POR_CM))      # no hace falta achicar lo que ya entra
        cm = min(cm, ANCHO_MAX, ALTO_MAX * ancho / alto)       # y nunca más que la página
        impresa = letra * cm * PT_POR_CM / ancho
        ok = impresa >= LETRA_MINIMA - 0.05
        fallas += not ok
        print(f"{nombre:34} {ancho:5.0f}x{alto:<5.0f} {letra:5.1f}  {cm:5.1f} x {cm * alto / ancho:4.1f} cm"
              f"  {impresa:9.1f} pt  {'ok' if ok else 'FALLA: no entra legible'}")
    sys.exit(1 if fallas else 0)


if __name__ == "__main__":
    main(sys.argv[1:])
