# -*- coding: utf-8 -*-
"""Ensambla el documento final: preliminares, cuerpo, cierre, encabezado y numeracion."""
import sys, io
import pdfplumber
from collections import defaultdict
from pypdf import PdfReader, PdfWriter
from reportlab.pdfgen import canvas as rl_canvas
from reportlab.pdfbase import pdfmetrics
import ea_style
from ea_style import PW, PH, ML, MR
from front_back import build_indice, build_historial, build_resumen, build_conclusiones, build_biblio

O    = os.path.join(_SAL, "")
DOC  = os.path.join(_RAIZ, "..")
BASE = DOC + "EventAura_v2.pdf"

# ══════════════════════════════════════════════ DATOS DEL ENCABEZADO
CFG = dict(
    universidad = "UNIVERSIDAD ABIERTA INTERAMERICANA — Facultad de Tecnología Informática",
    materia     = "Trabajo de Campo I",
    docente     = "Audoglio",
    localizacion= "Rosario",
    comision    = "«completar»",
    turno       = "«completar»",
    anio        = "2025",
    sistema     = "EventAura",
    version     = "2.4",
    alumno1     = "Ambrosino, Máximo",
    legajo1     = "«completar»",
    alumno2     = "Riggio, Juan Cruz",
    legajo2     = "«completar»",
)

# ══════════════════════════════════════════════ 1. TITULOS DEL CUERPO
BOLD = ("TimesNewRomanPS-BoldMT", "LiberationSerif-Bold")
def extraer_titulos(path, desde, hasta):
    res = []
    with pdfplumber.open(path) as pdf:
        for i in range(desde, hasta + 1):
            p = pdf.pages[i - 1]
            lineas = defaultdict(list)
            for w in p.extract_words(extra_attrs=['size', 'fontname']):
                lineas[round(w['top'], 1)].append(w)
            for t in sorted(lineas):
                ws = lineas[t]
                if not all(any(b in w['fontname'] for b in BOLD) for w in ws):
                    continue
                sz = round(max(w['size'] for w in ws))
                if sz not in (20, 13):
                    continue
                txt = ' '.join(w['text'] for w in ws).strip()
                if sz == 13 and min(w['x0'] for w in ws) > 75:
                    continue
                if not (4 <= len(txt) <= 62) or txt.endswith(':'):
                    continue
                res.append((1 if sz == 20 else 2, txt, i))
    return res

cuerpo = extraer_titulos(BASE, 2, 102)          # se descarta caratula y hoja final en blanco
print(f"titulos del cuerpo: {len(cuerpo)}")

# ══════════════════════════════════════════════ 2. PRELIMINARES Y CIERRE
build_historial(O + "_historial.pdf", CFG)
build_resumen(O + "_resumen.pdf")
build_conclusiones(O + "_conclusiones.pdf")
build_biblio(O + "_biblio.pdf")
N_HIST = len(PdfReader(O + "_historial.pdf").pages)
N_RES  = len(PdfReader(O + "_resumen.pdf").pages)
N_CON  = len(PdfReader(O + "_conclusiones.pdf").pages)
N_BIB  = len(PdfReader(O + "_biblio.pdf").pages)

def entradas_para(I):
    """Construye el indice para una cantidad I de paginas de indice."""
    off = I + 3                                  # pagina final = pagina_v2 + off
    e  = [(1, "Historial de revisión", 1 + I + 1)]
    e += [(1, "Resumen ejecutivo", 1 + I + 2)]
    e += [(n, t, p + off) for n, t, p in cuerpo]
    fin = 1 + I + N_HIST + N_RES + 101
    e += [(1, "Conclusiones", fin + 1)]
    e += [(1, "Bibliografía", fin + 1 + N_CON)]
    return e

I = 3
for _ in range(6):                               # converge en una o dos vueltas
    build_indice(entradas_para(I), O + "_indice.pdf")
    real = len(PdfReader(O + "_indice.pdf").pages)
    if real == I:
        break
    I = real
print(f"indice: {I} paginas")

# ══════════════════════════════════════════════ 3. ENSAMBLADO
base = PdfReader(BASE)
w = PdfWriter()
w.add_page(base.pages[0])                                    # caratula
for p in PdfReader(O + "_indice.pdf").pages:      w.add_page(p)
for p in PdfReader(O + "_historial.pdf").pages:   w.add_page(p)
for p in PdfReader(O + "_resumen.pdf").pages:     w.add_page(p)
for p in base.pages[1:102]:                       w.add_page(p)   # cuerpo
for p in PdfReader(O + "_conclusiones.pdf").pages:w.add_page(p)
for p in PdfReader(O + "_biblio.pdf").pages:      w.add_page(p)
TOTAL = len(w.pages)
with open(O + "_sin_sellar.pdf", "wb") as f:
    w.write(f)
print(f"ensamblado: {TOTAL} paginas")

# ══════════════════════════════════════════════ 4. SECCION POR PAGINA
off = I + 3
seccion = {}
for n in range(1, TOTAL + 1):
    seccion[n] = ""
for n in range(2, 2 + I):                 seccion[n] = "Índice"
seccion[1 + I + 1] = "Historial de revisión"
for n in range(1 + I + 2, 1 + I + 2 + N_RES): seccion[n] = "Resumen ejecutivo"
actual = "Descripción del producto"
h1 = {p + off: t for n, t, p in cuerpo if n == 1}
for n in range(1 + I + N_HIST + N_RES + 1, 1 + I + N_HIST + N_RES + 102):
    if n in h1:
        actual = h1[n]
    seccion[n] = actual
fin = 1 + I + N_HIST + N_RES + 101
for n in range(fin + 1, fin + 1 + N_CON):        seccion[n] = "Conclusiones"
for n in range(fin + 1 + N_CON, TOTAL + 1):      seccion[n] = "Bibliografía"

# ══════════════════════════════════════════════ 5. SELLADO
buf = io.BytesIO()
c = rl_canvas.Canvas(buf, pagesize=(PW, PH))
GR = (0.30, 0.30, 0.30)

def centrado(txt, font, size, y, color=(0, 0, 0)):
    c.setFillColorRGB(*color)
    to = c.beginText(); to.setTextOrigin((PW - pdfmetrics.stringWidth(txt, font, size)) / 2.0, y)
    to.setWordSpace(0); to.setFont(font, size); to.textOut(txt); c.drawText(to)
    c.setFillColorRGB(0, 0, 0)

def izq(txt, font, size, x, y, color=(0, 0, 0)):
    c.setFillColorRGB(*color)
    to = c.beginText(); to.setTextOrigin(x, y)
    to.setWordSpace(0); to.setFont(font, size); to.textOut(txt); c.drawText(to)
    c.setFillColorRGB(0, 0, 0)

for n in range(1, TOTAL + 1):
    if n == 1:
        # datos que la Carpeta Modelo exige en la caratula y no estaban
        centrado(f"Docente: {CFG['docente']}", "EA", 11, 165)
        centrado(f"Comisión: {CFG['comision']}    ·    Turno: {CFG['turno']}", "EA", 11, 148)
        centrado(f"Legajos: {CFG['legajo1']} – {CFG['legajo2']}", "EA", 11, 131)
    else:
        izq(CFG['universidad'], "EA-B", 7.4, ML, 814, GR)
        izq(f"Materia: {CFG['materia']}   ·   Docente: {CFG['docente']}   ·   "
            f"Comisión: {CFG['comision']}   ·   Turno: {CFG['turno']}   ·   "
            f"Localización: {CFG['localizacion']}   ·   Año: {CFG['anio']}",
            "EA", 6.9, ML, 804.5, GR)
        izq(f"Alumnos: {CFG['alumno1']} (Legajo {CFG['legajo1']})   ·   "
            f"{CFG['alumno2']} (Legajo {CFG['legajo2']})", "EA", 6.9, ML, 795, GR)
        izq(f"{CFG['sistema']} — versión {CFG['version']}", "EA-BI", 6.9, ML, 785.5, GR)
        sec = seccion.get(n, "")
        if sec:
            wd = pdfmetrics.stringWidth(sec, "EA-I", 6.9)
            izq(sec, "EA-I", 6.9, MR - wd, 785.5, GR)
        c.setStrokeColorRGB(*GR); c.setLineWidth(0.4); c.line(ML, 779, MR, 779)
        centrado(str(n), "EA", 9.5, 26)
    c.showPage()
c.save()
buf.seek(0)

sellos = PdfReader(buf)
libro  = PdfReader(O + "_sin_sellar.pdf")
out = PdfWriter()
for i, pg in enumerate(libro.pages):
    pg.merge_page(sellos.pages[i])
    out.add_page(pg)
out.add_metadata({'/Title': 'EventAura — Trabajo de Campo I',
                  '/Author': 'Maximo Ambrosino - Juan Cruz Riggio',
                  '/Subject': 'Universidad Abierta Interamericana'})
with open(DOC + "EventAura_v2.pdf", "wb") as f:
    out.write(f)
print(f"FINAL: {len(out.pages)} paginas -> EventAura_v2.pdf")
