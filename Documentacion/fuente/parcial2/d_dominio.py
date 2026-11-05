# (b) Diagrama de dominio / clases - segun las entidades y tablas REALES
import subprocess

def clase(nombre, attrs, estereo=None, color="#FFFFFF", borde="#333333"):
    cab = (f'<TR><TD BGCOLOR="{color}"><FONT POINT-SIZE="7"><I>&#171;{estereo}&#187;</I></FONT><BR/>'
           f'<B>{nombre}</B></TD></TR>') if estereo else \
          f'<TR><TD BGCOLOR="{color}"><B>{nombre}</B></TD></TR>'
    filas = "".join(f'<TR><TD ALIGN="LEFT">{a}</TD></TR>' for a in attrs)
    return (f'<<TABLE BORDER="0" CELLBORDER="1" CELLSPACING="0" CELLPADDING="3">'
            f'{cab}{filas}</TABLE>>')

N = {
 "Usuario":  clase("Usuario", ["Id : int","Mail : string","Contraseña : string (PBKDF2)",
                               "TipoUsuario : string","Nivel : NivelFidelidad",
                               "FechaUltimaEvaluacion : DateTime?"], color="#EDE7F6"),
 "Evento":   clase("Evento", ["Id : int","Nombre : string","Fecha : DateTime",
                              "Categoria : string","Estado : string","FinPreventa : DateTime?"]),
 "Sector":   clase("Sector", ["Id : int","Nombre : string","Precio : decimal",
                              "Capacidad : int","CantidadReservada : int"]),
 "Servicio": clase("Servicio", ["Id : int","Nombre : string",
                                "Tipo : string","Precio : decimal","Activo : bool"]),
 "Paquete":  clase("Paquete", ["IdEvento : int","Entradas : List&lt;Entrada&gt;",
                               "Servicios : List&lt;Servicio&gt;","ConSeguro : bool"],
                   estereo="en memoria", color="#FFF8E1"),
 "Compra":   clase("Compra", ["Id : int","FechaCompra : DateTime","Estado : EstadoCompra",
                              "NivelAplicado : NivelFidelidad","DescuentoAplicado : decimal",
                              "BaseEntradas : decimal","MontoDescuento : decimal",
                              "MontoServicios : decimal","CargoServicio : decimal",
                              "MontoSeguro : decimal","TotalPagado : decimal",
                              "MontoReembolsado : decimal","ReservaVence : DateTime?",
                              "ConSeguro : bool"], color="#EDE7F6"),
 "Entrada":  clase("Entrada", ["Id : int","Precio : decimal","CantidadVendida : int = 1",
                               "Estado : EstadoEntrada","CodigoQR : string"]),
 "CompraServicio": clase("CompraServicio", ["Id : int","Cantidad : int",
                                            "PrecioUnit : decimal"]),
 "Pago":     clase("Pago", ["Id : int","FechaPago : DateTime","Estado : EstadoPago",
                            "Ultimos4 : char(4)","Token : string","Motivo : string"]),
 "Seguro":   clase("Seguro", ["Id : int","Precio : decimal","VenceEl : DateTime"]),
 "Suscripcion": clase("Suscripcion", ["Id : int","Plan : string","FechaInicio : DateTime",
                                      "FechaFin : DateTime","Precio : decimal","Activa : bool"]),
 "ListaEspera": clase("ListaEspera", ["Id : int","FechaAlta : DateTime",
                                      "ConSuscripcion : bool"]),
 "Parametro": clase("Parametro", ["Clave : string","Valor : string","Regla : string"],
                    estereo="configuración", color="#FFF8E1"),
}

nodos = "\n".join(f'  {k} [label={v}];' for k, v in N.items())
g = f'''
digraph DOM {{
  rankdir=TB; bgcolor=white; pad=0.5; nodesep=0.40; ranksep=0.95; splines=true; concentrate=false;
  node [shape=plaintext, fontname="Liberation Sans", fontsize=9];
  edge [fontname="Liberation Sans", fontsize=8, color="#444444", arrowhead=vee,
        labeldistance=2.2, labelfloat=true, labelfontsize=8];
{nodos}

  Usuario -> Compra       [taillabel="1", headlabel="1..*"];
  Usuario -> Suscripcion  [taillabel="1", headlabel="0..*"];
  Usuario -> ListaEspera  [taillabel="1", headlabel="0..*"];
  Evento  -> Sector       [taillabel="1", headlabel="1..*", arrowtail=diamond, dir=both];
  Evento  -> ListaEspera  [taillabel="1", headlabel="0..*"];
  Evento  -> Servicio     [taillabel="*", headlabel="*", label="ServiciosEvento\\n(RN-14)", style=dashed, arrowhead=none];
  Compra  -> Entrada      [taillabel="1", headlabel="1..*", arrowtail=diamond, dir=both];
  Compra  -> CompraServicio [taillabel="1", headlabel="0..*", arrowtail=diamond, dir=both];
  Compra  -> Pago         [taillabel="1", headlabel="1..*"];
  Entrada -> Sector       [taillabel="*", headlabel="1"];
  Entrada -> Seguro       [taillabel="1", headlabel="0..1"];
  CompraServicio -> Servicio [taillabel="*", headlabel="1"];
  Paquete -> Compra       [label="se materializa en", style=dashed];
}}
'''
open("d_dominio.dot","w").write(g)
subprocess.run(["dot","-Tpng","-Gdpi=150","d_dominio.dot","-o","d_dominio.png"],check=True)
print("ok")
