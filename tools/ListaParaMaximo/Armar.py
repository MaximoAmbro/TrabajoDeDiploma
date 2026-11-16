"""Arma docs/LISTA-PARA-MAXIMO.md desde plantilla.tpl.

Uso, desde cualquier carpeta:
    py -3 -X utf8 tools/ListaParaMaximo/Armar.py

Pasos:
1. Genera, desde docs/04-casos-prueba.md, las dos tablas que la lista copia:
   generado/cobertura.md (caso → reglas) y generado/tabla-casos.md (tabla de CP-62 en adelante).
2. Recorre la plantilla. Una línea
       @@ archivo :: [ancla >> ] regex_inicio :: regex_fin
   copia del archivo desde la línea que cumple inicio (incluida; si hay ancla, la busca después
   del ancla) hasta la anterior a la primera que cumple fin. Con @@- la línea de inicio no se
   copia. El archivo es relativo al repositorio; si empieza con «generado/», es relativo a esta
   carpeta.
3. Aplica reemplazos.py: pares (viejo, nuevo) de texto exacto, o ("re:" + patrón, nuevo).
   Cada par tiene que encontrar algo; si uno no encuentra, no escribe nada y lo informa.
"""
import os
import re
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.normpath(os.path.join(AQUI, "..", ".."))
GENERADO = os.path.join(AQUI, "generado")
REGISTRO = os.path.join(RAIZ, "docs", "04-casos-prueba.md")
SALIDA = os.path.join(RAIZ, "docs", "LISTA-PARA-MAXIMO.md")


def seccion_casos():
    reg = open(REGISTRO, encoding="utf-8").read()
    m = re.search(r"^## Casos CP-62 a CP-(\d+)", reg, re.M)
    if not m:
        sys.exit("No encuentro «## Casos CP-62 a CP-n» en docs/04-casos-prueba.md")
    return reg[m.start():], int(m.group(1))


def generar_cobertura(sec, ultimo):
    filas = []
    for l in sec.split("\n"):
        m = re.match(r"^\| (CP-(\d+)) \| [^|]+ \| ([^|]+) \| [^|]+ \| ([^|]+) \| ([^|]+) \|", l)
        if not m or not 62 <= int(m.group(2)) <= ultimo:
            continue
        cp, cond, rf, rn = m.group(1), m.group(3).strip(), m.group(4).strip(), m.group(5).strip()
        cond = re.sub(r"\s*\(\d+ variantes?\)", "", cond)
        cond = re.sub(r"\*\*[^*]+\*\*", "", cond).strip().rstrip(".").replace("`", "")
        if len(cond) > 110:
            cond = cond[:107].rsplit(" ", 1)[0] + "…"
        if rn in ("—", "-", ""):
            rnfs = [x.strip() for x in rf.split(",") if x.strip().startswith("RNF")]
            rn = "— (" + ", ".join(rnfs) + ")" if rnfs else "—"
        filas.append(f"| {cp} — {cond} | {rn} |")
    return "\n".join(filas) + "\nFIN-COBERTURA\n"


def generar_tabla(sec):
    out = ["| Caso | Condición de entrada | Resultado esperado | RF | Reglas |", "|---|---|---|---|---|"]
    for l in sec.split("\n"):
        c = [x.strip() for x in l.strip().strip("|").split("|")]
        if len(c) == 7 and re.match(r"CP-\d+$", c[0]):
            out.append("| " + " | ".join([c[0], c[2], c[3], c[4], c[5]]) + " |")
    return "\n".join(out) + "\nFIN-TABLA\n"


def incluir(spec, excluir_inicio):
    archivo, ini, fin = [x.strip() for x in spec.split("::")]
    if archivo.startswith("generado/"):
        ruta = os.path.join(AQUI, archivo)
    else:
        ruta = os.path.join(RAIZ, archivo)
    lineas = open(ruta, encoding="utf-8").read().split("\n")
    desde = 0
    if " >> " in ini:
        ancla, ini = [x.strip() for x in ini.split(" >> ")]
        desde = next(k for k, l in enumerate(lineas) if re.search(ancla, l))
    try:
        i = next(k for k in range(desde, len(lineas)) if re.search(ini, lineas[k]))
        j = next(k for k in range(i + 1, len(lineas)) if re.search(fin, lineas[k]))
    except StopIteration:
        sys.exit("NO SE ENCONTRÓ: " + spec)
    bloque = lineas[i + (1 if excluir_inicio else 0):j]
    while bloque and (not bloque[-1].strip() or bloque[-1].strip() == "---"):
        bloque.pop()
    while bloque and not bloque[0].strip():
        bloque.pop(0)
    return "\n".join(bloque)


def main():
    sec, ultimo = seccion_casos()
    os.makedirs(GENERADO, exist_ok=True)
    open(os.path.join(GENERADO, "cobertura.md"), "w", encoding="utf-8").write(generar_cobertura(sec, ultimo))
    open(os.path.join(GENERADO, "tabla-casos.md"), "w", encoding="utf-8").write(generar_tabla(sec))

    out = []
    for l in open(os.path.join(AQUI, "plantilla.tpl"), encoding="utf-8").read().split("\n"):
        if l.startswith("@@-"):
            out.append(incluir(l[3:], True))
        elif l.startswith("@@"):
            out.append(incluir(l[2:], False))
        else:
            out.append(l)
    texto = "\n".join(out)

    sys.path.insert(0, AQUI)
    from reemplazos import R
    faltan = []
    for viejo, nuevo in R:
        if viejo.startswith("re:"):
            texto, n = re.subn(viejo[3:], nuevo, texto, flags=re.S)
            if n == 0:
                faltan.append(viejo[:90])
        else:
            if viejo not in texto:
                faltan.append(viejo[:90])
            texto = texto.replace(viejo, nuevo)
    if faltan:
        print("NO ENCONTRADOS (no se escribió nada):", *faltan, sep="\n  ")
        sys.exit(1)
    open(SALIDA, "w", encoding="utf-8", newline="\n").write(texto)
    print("ok", len(texto.split("\n")), "líneas →", os.path.relpath(SALIDA, RAIZ))


if __name__ == "__main__":
    main()
