# (a) Diagrama de casos de uso - foco en CU-01 Comprar paquete
import subprocess
g = r'''
digraph CU {
  rankdir=LR; bgcolor="white"; splines=true; nodesep=0.30; ranksep=0.85;
  node [fontname="Liberation Sans", fontsize=13];
  edge [fontname="Liberation Sans", fontsize=11, color="#333333"];

  cliente  [shape=none, label="", image="ac_cliente.png"];
  pasarela [shape=none, label="", image="ac_pasarela.png"];

  subgraph cluster_sis {
    label="EventAura"; fontname="Liberation Sans"; fontsize=11; fontcolor="#444444";
    style="rounded"; color="#888888"; margin=16;

    cu01 [shape=ellipse, style=filled, fillcolor="#EDE7F6", color="#7456AE",
          penwidth=1.6, label="CU-01\nComprar paquete"];
    buscar  [shape=ellipse, label="Buscar y seleccionar\nevento"];
    armar   [shape=ellipse, label="Armar paquete\n(entradas + servicios)"];
    desc    [shape=ellipse, label="Aplicar descuento\npor nivel"];
    reserva [shape=ellipse, label="Reservar cupo\n(10 min)"];
    pagar   [shape=ellipse, label="Pagar\n(simulado)"];
    emitir  [shape=ellipse, label="Emitir entrada\ncon QR"];
    espera  [shape=ellipse, style=dashed, label="Inscribirse en\nlista de espera"];

    cu01 -> buscar  [style=dashed, label="«include»"];
    cu01 -> armar   [style=dashed, label="«include»"];
    cu01 -> desc    [style=dashed, label="«include»"];
    cu01 -> reserva [style=dashed, label="«include»"];
    cu01 -> pagar   [style=dashed, label="«include»"];
    cu01 -> emitir  [style=dashed, label="«include»"];
    espera -> cu01  [style=dashed, label="«extend»\n[evento agotado]"];
  }

  cliente -> cu01 [arrowhead=none];
  pagar -> pasarela [arrowhead=none];
}
'''
open("d_casos.dot","w").write(g)
subprocess.run(["dot","-Tpng","-Gdpi=170","d_casos.dot","-o","d_casos.png"],check=True)
print("ok")
