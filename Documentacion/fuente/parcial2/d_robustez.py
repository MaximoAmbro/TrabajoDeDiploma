# (f) Diagrama de robustez (ICONIX) de CU-01 - clases reales
import subprocess

def ico(archivo, texto):
    return (f'<<TABLE BORDER="0" CELLBORDER="0" CELLSPACING="0" CELLPADDING="1">'
            f'<TR><TD><IMG SRC="{archivo}"/></TD></TR>'
            f'<TR><TD><FONT POINT-SIZE="9">{texto}</FONT></TD></TR></TABLE>>')

B = lambda t: ico("ic_boundary.png", t)
C = lambda t: ico("ic_control.png", t)
E = lambda t: ico("ic_entity.png", t)

g = f'''
digraph ROB {{
  rankdir=LR; bgcolor=white; nodesep=0.28; ranksep=0.80; pad=0.4; splines=true;
  node [shape=plaintext, fontname="Liberation Sans", fontsize=9];
  edge [fontname="Liberation Sans", fontsize=8, color="#444444", arrowhead=none];

  cli [shape=none, label="", image="ac_cliente.png"];

  b1 [label={B("frmEventosCliente")}];
  b2 [label={B("FrmCompraCliente")}];
  b3 [label={B("frmPago")}];
  b4 [label={B("frmConfirmacion")}];

  c1 [label={C("GestorPaquetes")}];
  c2 [label={C("GestorFidelizacion")}];
  c3 [label={C("GestorCompras")}];
  c4 [label={C("PasarelaSimulada")}];
  c5 [label={C("GeneradorQR")}];

  e1 [label={E("Evento / Sector")}];
  e2 [label={E("Paquete")}];
  e3 [label={E("Compra")}];
  e4 [label={E("Entrada")}];
  e5 [label={E("Pago")}];
  e6 [label={E("Seguro")}];

  cli -> b1; b1 -> b2 [style=dotted, label="evento elegido"];
  b2 -> c1 [label="Nuevo / AgregarEntradas"];
  b2 -> c2 [label="CalcularDescuento"];
  b2 -> c3 [label="Iniciar"];
  b3 -> c3 [label="Confirmar"];
  b4 -> c3 [label="EntradasDe"];
  c1 -> e1; c1 -> e2;
  c2 -> e2; c2 -> e3 [label="nivel congelado"];
  c3 -> e3; c3 -> e4; c3 -> e6;
  c3 -> c4 [label="Cobrar", style=dashed];
  c3 -> c5 [label="NuevoCodigo", style=dashed];
  c4 -> e5;
  c5 -> e4 [label="CodigoQR"];
  {{rank=same; b1; b2; b3; b4;}}
  {{rank=same; c1; c2; c3; c4; c5;}}
}}
'''
open("d_robustez.dot","w").write(g)
subprocess.run(["dot","-Tpng","-Gdpi=150","d_robustez.dot","-o","d_robustez.png"],check=True)
print("ok")
