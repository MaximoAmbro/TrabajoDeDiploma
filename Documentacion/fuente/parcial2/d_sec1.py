# (h.1) Secuencia - armado del paquete y reserva del cupo
from sec import render
P = [("cli","Cliente","actor"), ("frm","FrmCompraCliente","boundary"),
     ("gp","GestorPaquetes","control"), ("gf","GestorFidelizacion","control"),
     ("gc","GestorCompras","control"), ("rc","RepositorioCompras","entity")]
M = [
 {"t":"call","de":"cli","a":"frm","txt":"abrir(idEvento)"},
 {"t":"call","de":"frm","a":"gc","txt":"LiberarVencidas()   « RN-15: libera reservas caducadas »"},
 {"t":"call","de":"frm","a":"gp","txt":"Nuevo(idEvento, idUsuario)   « RN-09 vigente · RN-39 preventa »"},
 {"t":"call","de":"frm","a":"gf","txt":"ObtenerCliente() / ReevaluarNivel()   « RN-23 »"},
 {"t":"return","de":"gf","a":"frm","txt":"Cliente (nivel persistido)"},
 {"t":"call","de":"cli","a":"frm","txt":"indicar cantidad por sector"},
 {"t":"call","de":"frm","a":"gp","txt":"AgregarEntradas(paquete, idSector, cantidad)"},
 {"t":"note","de":"gp","txt":"RN-08 cupo · RN-12 máximo 6 por operación · RN-16 congela el precio"},
 {"t":"call","de":"frm","a":"gf","txt":"CalcularDescuento(cliente, paquete)   « RN-20 · RN-21 »"},
 {"t":"return","de":"gf","a":"frm","txt":"ResumenDescuento"},
 {"t":"call","de":"cli","a":"frm","txt":"Comprar"},
 {"t":"call","de":"frm","a":"gc","txt":"Iniciar(paquete, cliente)"},
 {"t":"call","de":"gc","a":"gp","txt":"Validar(paquete)   « RN-13 »"},
 {"t":"call","de":"gc","a":"rc","txt":"Iniciar(compra, servicios, entradas)"},
 {"t":"note","de":"rc","txt":"Una transacción: Compra y Entradas en Pendiente + reserva del cupo"},
 {"t":"return","de":"gc","a":"frm","txt":"Compra (Pendiente · ReservaVence = ahora + 10 min)"},
]
print(render("d_sec1.png", P, M, ancho_col=270, paso=50, fs=17))
