# (c) Prototipo de FrmCompraCliente, a escala desde Visual/Cliente/FrmCompraCliente.Designer.cs
from PIL import Image, ImageDraw, ImageFont
FR="/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"
FB="/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf"
FM="/usr/share/fonts/truetype/liberation2/LiberationMono-Regular.ttf"
K=3                                   # escala de dibujo
W,H=760,595                           # ClientSize real del formulario
LILA=(116,86,174); GRIS=(164,165,169); BORDE=(150,150,150)
im=Image.new("RGB",(W*K,(H+30)*K),"white"); d=ImageDraw.Draw(im)
f=lambda p,s: ImageFont.truetype(p,int(s*K))

def caja(x,y,w,h,relleno="white",borde=BORDE,grosor=1):
    d.rectangle([x*K,y*K,(x+w)*K,(y+h)*K],fill=relleno,outline=borde,width=grosor*K)
def txt(x,y,s,fuente,color=(0,0,0)):
    d.text((x*K,y*K),s,font=fuente,fill=color)

# ---- barra de titulo (no es parte del ClientSize; se dibuja para dar contexto)
caja(0,0,W,30,(240,240,240))
txt(10,8,"FrmCompra",f(FB,11),(60,60,60))
d.line([0,30*K,W*K,30*K],fill=BORDE,width=1*K)
OY=30                                 # desplazamiento del area cliente

# ---- encabezado
txt(33,68+OY,"Festival de Rock Rosario",f(FB,22),LILA)
txt(17,122+OY,"Tu nivel: Plata · 5,0 % de descuento",f(FR,10),LILA)
txt(206,122+OY,"¿Cómo funcionan los niveles?",f(FR,10),LILA)
d.line([206*K,(136+OY)*K,353*K,(136+OY)*K],fill=LILA,width=1*K)   # subrayado del enlace

# ---- grilla de sectores (dgvSectores)
caja(17,148+OY,360,170)
cols=[("Sector",0,120),("Precio",120,80),("Disponibles",200,90),("Cantidad",290,70)]
caja(17,148+OY,360,24,(245,245,245))
for nom,dx,an in cols:
    txt(17+dx+6,154+OY,nom,f(FB,8.5),(40,40,40))
    if dx: d.line([(17+dx)*K,(148+OY)*K,(17+dx)*K,(318+OY)*K],fill=(225,225,225),width=1*K)
filas=[("Campo","$ 25.000,00","2000","2"),("Platea","$ 40.000,00","800","0"),
       ("VIP","$ 60.000,00","100","0")]
y=172+OY
for i,(s,p,disp,cant) in enumerate(filas):
    if i%2: caja(18,y,358,24,(250,249,253),None)
    txt(23,y+6,s,f(FR,8.5))
    for val,dx,an in ((p,120,80),(disp,200,90)):
        tw=d.textlength(val,font=f(FR,8.5))/K
        txt(17+dx+an-8-tw,y+6,val,f(FR,8.5))
    caja(17+290,y,70,24,(245,241,252))        # columna editable
    tw=d.textlength(cant,font=f(FR,8.5))/K
    txt(17+290+70-8-tw,y+6,cant,f(FR,8.5))
    d.line([18*K,(y+24)*K,376*K,(y+24)*K],fill=(230,230,230),width=1*K)
    y+=24

# ---- servicios y seguro
for x,yy,t in ((17,326,"Hospedaje $45.000"),(200,326,"Traslado $12.000"),
               (17,352,"Merchandising $18.000"),(200,352,"Experiencia VIP $80.000")):
    caja(x,yy+OY+4,13,13)
    if t.startswith("Hospedaje"):
        d.line([(x+3)*K,(yy+OY+10)*K,(x+6)*K,(yy+OY+14)*K],fill=LILA,width=2*K)
        d.line([(x+6)*K,(yy+OY+14)*K,(x+11)*K,(yy+OY+7)*K],fill=LILA,width=2*K)
    txt(x+19,yy+OY+3,t,f(FR,9))
caja(17,384+OY+4,13,13)
txt(36,384+OY+3,"Seguro de entrada (devolución hasta 48 h antes)",f(FR,9))

# ---- botones
caja(82,466+OY,222,44,LILA,LILA)
tw=d.textlength("Comprar",font=f(FB,11))/K
txt(82+111-tw/2,466+OY+15,"Comprar",f(FB,11),(255,255,255))
caja(9,552+OY,150,34,(250,250,250))
tw=d.textlength("Volver al listado",font=f(FR,10))/K
txt(9+75-tw/2,552+OY+10,"Volver al listado",f(FR,10))

# ---- panel de resumen
caja(395,68+OY,350,442,(255,255,255))
resumen=["RESUMEN DEL PAQUETE","",
 "Entradas (2)","  2 x Campo              $ 50.000,00",
 "Base entradas            $ 50.000,00","Desc. Plata 5,0 %        -$ 2.500,00",
 "Subtotal entradas        $ 47.500,00","",
 "Servicios (sin descuento)","  Hotel Ríos 1 noche     $ 45.000,00",
 "Subtotal servicios       $ 45.000,00","",
 "Cargo por servicio        $ 7.125,00","Seguro: no","",
 "TOTAL                    $ 99.625,00",
 "El descuento se aplica sobre el valor","de las entradas."]
yy=76+OY
for ln in resumen:
    neg = ln.startswith("TOTAL") or ln.startswith("RESUMEN")
    txt(403,yy,ln,f(FM,8) if not neg else ImageFont.truetype(FM,int(8*K)),(50,50,50))
    yy+=14.55 if ln else 8

im.resize((W,H+30),Image.LANCZOS).save("d_form.png")
print("ok",W,H+30)
