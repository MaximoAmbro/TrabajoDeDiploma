# (d) Arquitectura de la solucion - 4 capas apiladas
import subprocess

def capa(titulo, sub, clases, bg, fg):
    items = "".join(f'<TR><TD ALIGN="LEFT"><FONT POINT-SIZE="9">{c}</FONT></TD></TR>' for c in clases)
    return (f'<<TABLE BORDER="0" CELLBORDER="0" CELLSPACING="2" CELLPADDING="3" BGCOLOR="{bg}">'
            f'<TR><TD ALIGN="LEFT"><FONT POINT-SIZE="11" COLOR="{fg}"><B>{titulo}</B></FONT>'
            f'<BR ALIGN="LEFT"/><FONT POINT-SIZE="8" COLOR="{fg}">{sub}</FONT></TD></TR>'
            f'{items}</TABLE>>')

V = capa("Visual", "Visual.csproj &#183; net48 &#183; WinForms &#183; iText 9.2.0",
         ["FrmCompraCliente &#183; frmPago &#183; frmConfirmacion",
          "ExportadorPDF  <I>(implementa IExportador)</I>"], "#F6F3FB", "#7456AE")
N = capa("Negocio", "Repositorio.csproj &#183; netstandard2.0 &#183; QRCoder 1.6.0",
         ["GestorPaquetes &#183; GestorFidelizacion &#183; GestorCompras",
          "PasarelaSimulada &#183; GeneradorQR &#183; GestorParametros",
          "ReglaNegocioException &#183; IExportador"], "#F1F8E9", "#2E7D32")
D = capa("Datos", "Datos.csproj &#183; netstandard2.0 &#183; único lugar con SqlCommand",
         ["RepositorioCompras &#183; RepositorioEntradas &#183; RepositorioSectores",
          "RepositorioPagos &#183; RepositorioFidelizacion &#183; <I>(14 en total)</I>",
          "Conexion &#183; RepositorioBase"], "#E8F0FE", "#1565C0")
E = capa("Entidades", "Entidades.csproj &#183; netstandard2.0 &#183; POCOs, sin lógica",
         ["Paquete &#183; Compra &#183; Entrada &#183; ResumenDescuento",
          "DatosTarjeta <I>(transitoria, RN-30)</I> &#183; enums"], "#FFF8E1", "#B26A00")

g = f'''
digraph ARQ {{
  rankdir=TB; bgcolor=white; ranksep=0.55; pad=0.4; nodesep=0.5;
  node [shape=box, style="rounded", fontname="Liberation Sans", color="#777777"];
  edge [fontname="Liberation Sans", fontsize=9, color="#333333"];

  V [label={V}]; N [label={N}]; D [label={D}]; E [label={E}];
  SQL [label="SQL Server — BddEventAura", shape=cylinder, style=filled,
       fillcolor="#ECEFF1", color="#37474F", fontsize=10];

  V -> N [penwidth=1.5, label="  solo llama a Negocio: nunca abre\\l  una conexión ni escribe SQL\\l"];
  N -> D [penwidth=1.5, label="  repositorios singleton\\l"];
  D -> SQL [penwidth=1.5, label="  Microsoft.Data.SqlClient 6.0.2\\l"];
  N -> E [style=dashed, constraint=false, label="  usa"];
  D -> E [style=dashed, constraint=false, label="  mapea"];
  {{rank=same; N; E;}}
}}
'''
open("d_arq.dot","w").write(g)
subprocess.run(["dot","-Tpng","-Gdpi=155","d_arq.dot","-o","d_arq.png"],check=True)
print("ok")
