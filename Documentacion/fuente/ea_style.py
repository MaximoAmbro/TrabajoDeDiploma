"""
ea_style.py — Motor de composicion que replica EXACTAMENTE el formato de EventAura.pdf

Especificacion extraida del PDF original (Google Docs -> Skia/PDF):
  Pagina .............. A4, 596 x 842 pt
  Margen izq .......... 72 pt      Margen der ...... 70 pt (ancho util 454 pt)
  Primera linea ....... top 73.6 pt   Ultima linea .... top 804 pt
  Cuerpo .............. Times New Roman 11 pt  (Liberation Serif: metrica identica)
  Interlineado ........ 14.55 pt        Espacio entre parrafos .. 14.55 pt
  Titulo H1 ........... TNR Bold 20 pt, centrado
  Subtitulo H2 ........ TNR Bold 13 pt, izquierda
  Rotulo H3 ........... TNR Bold 11 pt, izquierda
  Vineta N1 ........... '●' Arial 11 pt en x=90, texto en x=108
  Vineta N2 ........... '○' Arial 11 pt en x=126, texto en x=144
  Encabezado/pie ...... vacios (solo un espacio) -> se replican vacios
"""

from reportlab.pdfgen import canvas as rl_canvas
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont

# --------------------------------------------------------------------------- fuentes
_LIB = "/usr/share/fonts/truetype/liberation"
_LIB2 = "/usr/share/fonts/truetype/liberation2"
pdfmetrics.registerFont(TTFont("EA", f"{_LIB}/LiberationSerif-Regular.ttf"))
pdfmetrics.registerFont(TTFont("EA-B", f"{_LIB}/LiberationSerif-Bold.ttf"))
pdfmetrics.registerFont(TTFont("EA-I", f"{_LIB}/LiberationSerif-Italic.ttf"))
pdfmetrics.registerFont(TTFont("EA-BI", f"{_LIB}/LiberationSerif-BoldItalic.ttf"))
pdfmetrics.registerFont(TTFont("EA-S", f"{_LIB}/LiberationSans-Regular.ttf"))   # vinetas (Arial)
pdfmetrics.registerFont(TTFont("EA-M", f"{_LIB2}/LiberationMono-Regular.ttf"))  # codigo

# --------------------------------------------------------------------------- metricas
PW, PH = 596.0, 842.0
ML, MR = 72.0, 523.0          # x inicial / x limite derecho (texto JUSTIFICADO)
TOP_FIRST = 73.6              # 'top' de la primera linea
BOTTOM_LIMIT = 806.0          # 'top' maximo admitido para una linea
LEAD = 14.55                  # interlineado
PARA_GAP = 14.55              # espacio adicional entre parrafos
BODY, H1, H2, H3 = 11.0, 20.0, 13.0, 11.0

BUL1_X, BUL1_TX = 90.0, 108.0
BUL2_X, BUL2_TX = 126.0, 144.0

GRAY = (0.35, 0.35, 0.35)
HDR_BG = (0.906, 0.906, 0.906)


def _y(top):
    """Convierte coordenada 'top' (como la reporta pdfplumber) a 'y' de ReportLab."""
    return PH - top - BODY


class Doc:
    """Compositor secuencial con salto de pagina automatico."""

    def __init__(self, path):
        self.c = rl_canvas.Canvas(path, pagesize=(PW, PH))
        self.top = TOP_FIRST
        self._page_started = False
        self._start_page()

    # ------------------------------------------------------------------ interno
    def _start_page(self):
        self.c.setFillColorRGB(1, 1, 1)
        self.c.rect(0, 0, PW, PH, stroke=0, fill=1)   # fondo blanco, igual al original
        self.c.setFillColorRGB(0, 0, 0)
        self.top = TOP_FIRST
        self._page_started = True

    def _need(self, h):
        if self.top + h > BOTTOM_LIMIT:
            self.page_break()

    def page_break(self):
        self.c.showPage()
        self._start_page()

    # ------------------------------------------------------------------ texto base
    def _wrap(self, text, font, size, width):
        words, lines, cur = text.split(), [], ""
        for w in words:
            probe = w if not cur else cur + " " + w
            if pdfmetrics.stringWidth(probe, font, size) <= width:
                cur = probe
            else:
                if cur:
                    lines.append(cur)
                cur = w
        if cur:
            lines.append(cur)
        return lines or [""]

    _FMAP = {"r": "EA", "b": "EA-B", "i": "EA-I", "bi": "EA-BI"}

    def _draw_tokens(self, x, top, tokens, size, extra=0.0):
        """Dibuja una linea con espacios REALES en el flujo de contenido, de modo que el
        texto se pueda seleccionar y copiar correctamente. La justificacion se resuelve
        con el operador Tw (setWordSpace) en lugar de reposicionar palabra por palabra."""
        runs = []
        for w, st in tokens:
            if runs and runs[-1][1] == st:
                runs[-1][0].append(w)
            else:
                runs.append([[w], st])

        to = self.c.beginText()
        to.setTextOrigin(x, _y(top))
        to.setWordSpace(extra)          # siempre explicito: Tw persiste en el flujo
        for i, (words, st) in enumerate(runs):
            to.setFont(self._FMAP[st], size)
            chunk = " ".join(words)
            if i:
                chunk = " " + chunk
            to.textOut(chunk)
        self.c.drawText(to)

    def _put(self, font, size, x, y, s):
        """Dibuja una cadena con word-space explicito en cero. Necesario porque el
        operador Tw del PDF persiste en el flujo de contenido: sin esto, un titulo o
        una celda de tabla heredaria el espaciado de la ultima linea justificada."""
        to = self.c.beginText()
        to.setTextOrigin(x, y)
        to.setWordSpace(0)
        to.setFont(font, size)
        to.textOut(s)
        self.c.drawText(to)

    # ------------------------------------------------------------------ bloques
    def h1(self, text):
        """Titulo de seccion: 20 pt bold centrado en top=75.0; siguiente bloque en 125.5."""
        if self.top > TOP_FIRST + 1:
            self.page_break()
        self.top = 75.0
        w = pdfmetrics.stringWidth(text, "EA-B", H1)
        self._put("EA-B", H1, (PW - w) / 2.0, _y(self.top) - 5.5, text)
        self.top = 125.5

    def h2(self, text, space_before=14.55):
        """Subtitulo: 13 pt bold en el margen; siguiente linea a +14.7."""
        self._need(space_before + 22)
        if self.top > TOP_FIRST + 1:
            self.top += space_before
        self._put("EA-B", H2, ML, _y(self.top) - 1.6, text)
        self.top += 14.7

    def h3(self, text, space_before=7.3):
        self._need(space_before + 18)
        self.top += space_before
        self._put("EA-B", H3, ML, _y(self.top), text)
        self.top += LEAD

    def pre(self, lines, size=BODY, gap=True, x=ML):
        """Bloque literal (codigo / pseudocodigo) con la MISMA tipografia del cuerpo,
        tal como aparece en la seccion 'Implementacion' del documento original."""
        for ln in lines:
            self._need(LEAD)
            self._put("EA", size, x, _y(self.top), ln)
            self.top += LEAD
        if gap:
            self.top += PARA_GAP

    def p(self, text, gap=True, size=BODY, x=ML, width=None, style="r", justify=True):
        self.rich([(text, style)], gap=gap, size=size, x=x, width=width, justify=justify)

    def rich(self, segments, gap=True, size=BODY, x=ML, width=None, justify=True):
        """Parrafo justificado con mezcla de estilos.
        segments = [(texto, 'r'|'b'|'i'|'bi'), ...]"""
        width = width or (MR - x)
        space = pdfmetrics.stringWidth(" ", "EA", size)

        tokens = []
        for txt, st in segments:
            palabras = txt.split()
            for i, w in enumerate(palabras):
                # Si el segmento arranca pegado a lo anterior (no trae espacio inicial) y
                # empieza con puntuacion, se une al token previo para no dejar " ,".
                if (i == 0 and tokens and txt[:1] not in " \t"
                        and w[:1] in ",.;:)»!?%"):
                    prev_w, prev_st = tokens[-1]
                    corte = len(w) - len(w.lstrip(",.;:)»!?%"))
                    tokens[-1] = (prev_w + w[:corte], prev_st)
                    resto = w[corte:]
                    if resto:
                        tokens.append((resto, st))
                    continue
                tokens.append((w, st))
        if not tokens:
            self.top += LEAD
            return

        # ---- corte en lineas
        lines, cur, cw = [], [], 0.0
        for w, st in tokens:
            ww = pdfmetrics.stringWidth(w, self._FMAP[st], size)
            add = ww if not cur else ww + space
            if cw + add > width and cur:
                lines.append((cur, cw))
                cur, cw = [(w, st)], ww
            else:
                cur.append((w, st))
                cw += add
        lines.append((cur, cw))

        # ---- dibujo (justifica todas menos la ultima)
        for idx, (toks, lw) in enumerate(lines):
            self._need(LEAD)
            extra = 0.0
            if justify and idx < len(lines) - 1 and len(toks) > 1:
                extra = (width - lw) / (len(toks) - 1)
                if extra > size * 0.55:          # evita rios de blanco
                    extra = 0.0
            self._draw_tokens(x, self.top, toks, size, extra)
            self.top += LEAD
        if gap:
            self.top += PARA_GAP

    def bullet(self, segments, level=1, gap=False, size=BODY):
        """segments: str o [(texto,estilo),...]"""
        if isinstance(segments, str):
            segments = [(segments, "r")]
        bx, tx = (BUL1_X, BUL1_TX) if level == 1 else (BUL2_X, BUL2_TX)
        glyph = "●" if level == 1 else "○"
        self._need(LEAD)
        self._put("EA-S", size, bx, _y(self.top), glyph)
        self.rich(segments, gap=False, size=size, x=tx, width=MR - tx)
        self.top += (10.7 if not gap else PARA_GAP)

    def numbered(self, n, segments, gap=False, size=BODY, level=1):
        """Lista numerada: rotulo en x=90 y texto en x=108, igual que el original."""
        if isinstance(segments, str):
            segments = [(segments, "r")]
        bx, tx = (BUL1_X, BUL1_TX) if level == 1 else (BUL2_X, BUL2_TX)
        self._need(LEAD)
        self._put("EA", size, bx, _y(self.top), f"{n}.")
        self.rich(segments, gap=False, size=size, x=tx, width=MR - tx)
        self.top += (10.7 if not gap else PARA_GAP)

    def code(self, lines, size=8.0, gap=True):
        lead = size * 1.32
        for ln in lines:
            self._need(lead)
            self._put("EA-M", size, ML + 6, _y(self.top), ln[:118])
            self.top += lead
        if gap:
            self.top += PARA_GAP

    def table(self, headers, rows, widths, size=8.5, hdr_size=8.5, gap=True,
              aligns=None, pad=4.0):
        """Tabla con grilla fina. widths en pt (deben sumar <= 454)."""
        aligns = aligns or ["l"] * len(headers)
        # normaliza el ancho total al del bloque de texto (72 -> 523)
        total = float(sum(widths))
        k = (MR - ML) / total
        widths = [w * k for w in widths]

        def row_h(cells, fsize, font="EA"):
            m = 1
            for cell, w in zip(cells, widths):
                m = max(m, len(self._wrap(str(cell), font, fsize, w - 2 * pad)))
            return m * (fsize * 1.30) + 2 * pad

        def draw_row(cells, fsize, font, fill=None):
            h = row_h(cells, fsize, font)
            self._need(h)
            y_top = self.top
            if fill:
                self.c.setFillColorRGB(*fill)
                self.c.rect(ML, _y(y_top) + fsize - h + 2.5, sum(widths), h, stroke=0, fill=1)
                self.c.setFillColorRGB(0, 0, 0)
            cx = ML
            for cell, w, al in zip(cells, widths, aligns):
                lns = self._wrap(str(cell), font, fsize, w - 2 * pad)
                ty = y_top + pad - 1
                for ln in lns:
                    if al == "c":
                        tw = pdfmetrics.stringWidth(ln, font, fsize)
                        self._put(font, fsize, cx + (w - tw) / 2, _y(ty), ln)
                    else:
                        self._put(font, fsize, cx + pad, _y(ty), ln)
                    ty += fsize * 1.30
                cx += w
            # grilla
            self.c.setStrokeColorRGB(*GRAY)
            self.c.setLineWidth(0.5)
            yb = _y(y_top) + fsize - h + 2.5
            self.c.rect(ML, yb, sum(widths), h, stroke=1, fill=0)
            cx = ML
            for w in widths[:-1]:
                cx += w
                self.c.line(cx, yb, cx, yb + h)
            self.c.setStrokeColorRGB(0, 0, 0)
            self.top += h

        draw_row(headers, hdr_size, "EA-B", fill=HDR_BG)
        for r in rows:
            draw_row(r, size, "EA")
        if gap:
            self.top += PARA_GAP

    def figure_slot(self, caption="", height=95.0, gap=True):
        """Recuadro punteado reservado para una captura de pantalla, con epigrafe."""
        self.keep(height + 34)
        y_top = self.top
        yb = PH - y_top - height
        self.c.setStrokeColorRGB(*GRAY)
        self.c.setLineWidth(0.6)
        self.c.setDash(3, 3)
        self.c.rect(ML, yb, MR - ML, height, stroke=1, fill=0)
        self.c.setDash()
        self.c.setStrokeColorRGB(0, 0, 0)

        txt = "Espacio reservado para captura de pantalla"
        w = pdfmetrics.stringWidth(txt, "EA-I", 9)
        self.c.setFillColorRGB(*GRAY)
        self._put("EA-I", 9, (PW - w) / 2.0, yb + height / 2.0 - 3, txt)
        self.c.setFillColorRGB(0, 0, 0)

        self.top += height + 8
        cw = pdfmetrics.stringWidth(caption, "EA-B", 9)
        self._put("EA-B", 9, (PW - cw) / 2.0, _y(self.top), caption)
        self.top += LEAD
        if gap:
            self.top += PARA_GAP

    def image(self, ruta, caption="", max_h=430.0, ancho=None):
        """Inserta un PNG escalado al ancho util (o al indicado), con epigrafe centrado.

        Si no entra en lo que queda de pagina, salta de pagina antes de dibujarlo."""
        from reportlab.lib.utils import ImageReader
        iw, ih = ImageReader(ruta).getSize()
        w = ancho if ancho else (MR - ML)
        h = ih * (w / float(iw))
        if h > max_h:                      # limita el alto y recalcula el ancho
            h = max_h
            w = iw * (h / float(ih))
        self.keep(h + 26)
        x = ML + (MR - ML - w) / 2.0       # centrado en el ancho util
        yb = PH - self.top - h
        self.c.drawImage(ruta, x, yb, width=w, height=h, mask="auto")
        self.top += h + 7
        if caption:
            cw = pdfmetrics.stringWidth(caption, "EA-B", 9)
            self._put("EA-B", 9, (PW - cw) / 2.0, _y(self.top), caption)
            self.top += LEAD
        self.top += PARA_GAP

    def pagina_apaisada(self, ruta, caption="", titulo=""):
        """Pagina A4 apaisada dedicada a una figura ancha (diagramas de secuencia).

        Cierra la pagina en curso, emite una pagina horizontal con la imagen escalada
        al maximo que entre, y vuelve al formato vertical."""
        from reportlab.lib.utils import ImageReader
        self.c.showPage()
        self._page_started = False
        LW, LH = PH, PW                      # A4 apaisada: 842 x 596
        self.c.setPageSize((LW, LH))

        m_lat, m_sup, m_inf = 54.0, 52.0, 44.0
        y_tope = m_sup
        if titulo:
            self.c.setFillColorRGB(0, 0, 0)
            self.c.setFont("EA-B", H2)
            self.c.drawString(m_lat, LH - m_sup, titulo)
            y_tope += 22

        disp_w = LW - 2 * m_lat
        disp_h = LH - y_tope - m_inf - (16 if caption else 0)
        iw, ih = ImageReader(ruta).getSize()
        esc = min(disp_w / float(iw), disp_h / float(ih))
        w, h = iw * esc, ih * esc
        x = (LW - w) / 2.0
        yb = LH - y_tope - h
        self.c.drawImage(ruta, x, yb, width=w, height=h, mask="auto")

        if caption:
            cw = pdfmetrics.stringWidth(caption, "EA-B", 9)
            self.c.setFont("EA-B", 9)
            self.c.setFillColorRGB(0, 0, 0)
            self.c.drawString((LW - cw) / 2.0, yb - 14, caption)

        self.c.showPage()
        self.c.setPageSize((PW, PH))         # vuelve a vertical
        self.top = TOP_FIRST
        self._page_started = True
        self.c.setFillColorRGB(0, 0, 0)

    def space(self, pts=LEAD):
        self.top += pts

    def keep(self, pts):
        """Fuerza salto de pagina si no quedan 'pts' disponibles (evita titulos huerfanos)."""
        if self.top + pts > BOTTOM_LIMIT:
            self.page_break()

    def save(self):
        self.c.save()
