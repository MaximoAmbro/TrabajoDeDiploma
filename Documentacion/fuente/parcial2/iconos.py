# Iconos UML (actor y estereotipos ICONIX) como PNG, para usarlos en Graphviz.
from PIL import Image, ImageDraw, ImageFont
F = "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"
S = 4  # supersampling

def lienzo(w, h):
    im = Image.new("RGBA", (w*S, h*S), (255, 255, 255, 0))
    return im, ImageDraw.Draw(im)

def guardar(im, nombre, w, h):
    im.resize((w, h), Image.LANCZOS).save(nombre)

def actor(nombre="actor.png", w=46, h=62):
    im, d = lienzo(w, h)
    k, cx = S, w*S//2
    r = 9*k
    d.ellipse([cx-r, 4*k, cx+r, 4*k+2*r], outline=(0,0,0,255), width=2*k)
    d.line([cx, 4*k+2*r, cx, 40*k], fill="black", width=2*k)          # tronco
    d.line([cx-14*k, 28*k, cx+14*k, 28*k], fill="black", width=2*k)   # brazos
    d.line([cx, 40*k, cx-12*k, 58*k], fill="black", width=2*k)        # piernas
    d.line([cx, 40*k, cx+12*k, 58*k], fill="black", width=2*k)
    guardar(im, nombre, w, h)

def boundary(nombre="ic_boundary.png", w=52, h=40):
    im, d = lienzo(w, h)
    k = S; cx, cy, r = 32*k, 20*k, 13*k
    d.line([6*k, 5*k, 6*k, 35*k], fill="black", width=2*k)
    d.line([6*k, 20*k, cx-r, 20*k], fill="black", width=2*k)
    d.ellipse([cx-r, cy-r, cx+r, cy+r], outline="black", width=2*k)
    guardar(im, nombre, w, h)

def control(nombre="ic_control.png", w=46, h=42):
    im, d = lienzo(w, h)
    k = S; cx, cy, r = 23*k, 23*k, 13*k
    d.ellipse([cx-r, cy-r, cx+r, cy+r], outline="black", width=2*k)
    d.line([cx-2*k, cy-r, cx+7*k, cy-r-5*k], fill="black", width=2*k)   # flecha
    d.line([cx-2*k, cy-r, cx+6*k, cy-r+4*k], fill="black", width=2*k)
    guardar(im, nombre, w, h)

def entity(nombre="ic_entity.png", w=46, h=44):
    im, d = lienzo(w, h)
    k = S; cx, cy, r = 23*k, 19*k, 13*k
    d.ellipse([cx-r, cy-r, cx+r, cy+r], outline="black", width=2*k)
    d.line([cx-r, cy+r+3*k, cx+r, cy+r+3*k], fill="black", width=2*k)
    guardar(im, nombre, w, h)

actor(); boundary(); control(); entity()
print("iconos listos")

def actor_rotulado(texto, nombre, w=150, alto_fig=66, fs=17):
    """Figura de actor con su rotulo debajo, dentro del mismo PNG."""
    lineas = texto.split("\n")
    h = alto_fig + 6 + len(lineas) * (fs + 3)
    im, d = lienzo(w, h)
    k, cx = S, w * S // 2
    r = 10 * k
    d.ellipse([cx-r, 4*k, cx+r, 4*k+2*r], outline="black", width=2*k)
    d.line([cx, 4*k+2*r, cx, 44*k], fill="black", width=2*k)
    d.line([cx-16*k, 31*k, cx+16*k, 31*k], fill="black", width=2*k)
    d.line([cx, 44*k, cx-13*k, 62*k], fill="black", width=2*k)
    d.line([cx, 44*k, cx+13*k, 62*k], fill="black", width=2*k)
    f = ImageFont.truetype(F, fs * k)
    y = (alto_fig + 6) * k
    for ln in lineas:
        ancho = d.textlength(ln, font=f)
        d.text((cx - ancho / 2, y), ln, font=f, fill="black")
        y += (fs + 3) * k
    guardar(im, nombre, w, h)

actor_rotulado("Cliente", "ac_cliente.png")
actor_rotulado("Pasarela de pago\n(simulada)", "ac_pasarela.png")
print("actores rotulados")
