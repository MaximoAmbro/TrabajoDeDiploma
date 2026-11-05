# Renderizador de diagramas de secuencia UML con PIL.
from PIL import Image, ImageDraw, ImageFont

FR = "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"
FB = "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf"
FI = "/usr/share/fonts/truetype/liberation/LiberationSans-Italic.ttf"
S = 3                      # supersampling
MOR, LILA = (70,70,70), (116,86,174)

def _f(p, s): return ImageFont.truetype(p, int(s*S))

def render(archivo, participantes, mensajes, ancho_col=250, margen=40,
           alto_cab=56, paso=40, fs=11):
    """participantes: [(id, 'Nombre', 'tipo')]  tipo: actor|boundary|control|entity|db
       mensajes: dicts con tipo: call|return|self|note|frag|frag_else|frag_end"""
    n = len(participantes)
    W = margen*2 + ancho_col*(n-1) + 300
    filas = sum(2 if m["t"] == "note" else 1 for m in mensajes)
    H = margen + alto_cab + 34 + filas*paso + 70
    im = Image.new("RGB", (W*S, H*S), "white")
    d = ImageDraw.Draw(im)
    fr, fb, fi = _f(FR, fs), _f(FB, fs), _f(FI, fs-1)

    x = {}
    x0 = margen + 150
    for i, (pid, _, _) in enumerate(participantes):
        x[pid] = x0 + i*ancho_col

    # cajas de participante + linea de vida
    y_caja, h_caja = margen, 34
    y_fin = H - margen - 26
    for pid, nom, tipo in participantes:
        cx = x[pid]
        w = max(d.textlength(nom, font=fb)/S + 26, 112)
        relleno = {"actor": (255,255,255), "boundary": (246,243,251),
                   "control": (241,248,233), "entity": (255,248,225),
                   "db": (236,239,241)}.get(tipo, (255,255,255))
        for yy in (y_caja, y_fin):
            d.rectangle([(cx-w/2)*S, yy*S, (cx+w/2)*S, (yy+h_caja)*S],
                        fill=relleno, outline=MOR, width=1*S)
            tw = d.textlength(nom, font=fb)
            d.text(((cx*S)-tw/2, (yy+9)*S), nom, font=fb, fill=(0,0,0))
        d.line([cx*S, (y_caja+h_caja)*S, cx*S, y_fin*S], fill=(150,150,150), width=1*S)

    def flecha(xa, xb, yy, texto, estilo="call"):
        pun = 7
        if estilo == "return":
            for seg in range(int(min(xa,xb)), int(max(xa,xb)), 10):
                d.line([seg*S, yy*S, (seg+5)*S, yy*S], fill=MOR, width=1*S)
        else:
            d.line([xa*S, yy*S, xb*S, yy*S], fill=MOR, width=max(1,int(1.3))*S)
        s = 1 if xb > xa else -1
        d.polygon([(xb*S, yy*S), ((xb-s*pun)*S, (yy-4)*S), ((xb-s*pun)*S, (yy+4)*S)], fill=MOR)
        fuente = fi if estilo == "return" else fr
        tw = d.textlength(texto, font=fuente)
        d.text(((xa+xb)/2*S - tw/2, (yy-fs-5)*S), texto, font=fuente, fill=(0,0,0))

    y = margen + alto_cab + 20
    pila = []
    for m in mensajes:
        t = m["t"]
        if t in ("call", "return"):
            flecha(x[m["de"]], x[m["a"]], y, m["txt"], t)
        elif t == "self":
            cx = x[m["de"]]
            d.line([cx*S, y*S, (cx+34)*S, y*S], fill=MOR, width=1*S)
            d.line([(cx+34)*S, y*S, (cx+34)*S, (y+13)*S], fill=MOR, width=1*S)
            d.line([(cx+34)*S, (y+13)*S, (cx+7)*S, (y+13)*S], fill=MOR, width=1*S)
            d.polygon([(cx*S,(y+13)*S), ((cx+8)*S,(y+9)*S), ((cx+8)*S,(y+17)*S)], fill=MOR)
            d.text(((cx+42)*S, (y-3)*S), m["txt"], font=fr, fill=(0,0,0))
        elif t == "note":
            tw = d.textlength(m["txt"], font=fi)/S
            cx = x[m.get("de", participantes[0][0])]
            x1, x2 = cx+16, cx+16+tw+18
            if x2 > W - margen:                 # no se sale del lienzo: ancla a la derecha
                x2 = W - margen
                x1 = x2 - tw - 18
            if x1 < margen:
                x1, x2 = margen, margen + tw + 18
            d.polygon([(x1*S,(y-9)*S), ((x2-9)*S,(y-9)*S), (x2*S,y*S),
                       (x2*S,(y+19)*S), (x1*S,(y+19)*S)],
                      fill=(255,253,231), outline=(180,170,120))
            d.text(((x1+9)*S, (y-3)*S), m["txt"], font=fi, fill=(90,80,40))
            y += paso
        elif t == "frag":
            x1, x2 = margen+20, W-margen-20
            pila.append((y-16, m["txt"]))
            d.rectangle([x1*S, (y-16)*S, x2*S, (y-16)*S], outline=LILA)
            d.line([x1*S,(y-16)*S, x2*S,(y-16)*S], fill=LILA, width=1*S)
            d.line([x1*S,(y-16)*S, x1*S,(y-16)*S], fill=LILA, width=1*S)
            d.polygon([(x1*S,(y-16)*S), ((x1+56)*S,(y-16)*S), ((x1+56)*S,(y-1)*S),
                       ((x1+48)*S,(y+6)*S), (x1*S,(y+6)*S)], outline=LILA)
            d.text(((x1+8)*S, (y-13)*S), "alt", font=fb, fill=LILA)
            d.text(((x1+66)*S, (y-13)*S), m["txt"], font=fi, fill=LILA)
        elif t == "frag_else":
            x1, x2 = margen+20, W-margen-20
            d.line([x1*S, (y-14)*S, x2*S, (y-14)*S], fill=LILA, width=1*S)
            d.text(((x1+8)*S, (y-11)*S), m["txt"], font=fi, fill=LILA)
        elif t == "frag_end":
            x1, x2 = margen+20, W-margen-20
            yt, _ = pila.pop()
            d.rectangle([x1*S, yt*S, x2*S, (y-12)*S], outline=LILA, width=1*S)
            y -= paso//2
        y += paso

    im.resize((W, H), Image.LANCZOS).save(archivo)
    return W, H
