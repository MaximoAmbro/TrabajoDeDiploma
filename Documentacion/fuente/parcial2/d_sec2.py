# (h.2) Secuencia - pago simulado, confirmacion y emision
from sec import render
P = [("fp","frmPago","boundary"), ("gc","GestorCompras","control"),
     ("ps","PasarelaSimulada","control"), ("rp","RepositorioPagos","entity"),
     ("qr","GeneradorQR","control"), ("rc","RepositorioCompras","entity"),
     ("gf","GestorFidelizacion","control")]
M = [
 {"t":"call","de":"fp","a":"gc","txt":"Confirmar(compra, datosTarjeta)"},
 {"t":"self","de":"gc","txt":"ValidarTarjeta(t)   « RN-30: el número completo no sale de acá »"},
 {"t":"call","de":"gc","a":"ps","txt":"Cobrar(tarjeta, total)   « RN-17 »"},
 {"t":"return","de":"ps","a":"gc","txt":"ResultadoPago (últimos 4 + token)"},
 {"t":"call","de":"gc","a":"rp","txt":"Insertar(pago)   « RN-31: todo intento queda registrado »"},
 {"t":"frag","txt":"[pago aprobado]"},
 {"t":"call","de":"gc","a":"qr","txt":"NuevoCodigo()  — una vez por entrada   « RN-18 »"},
 {"t":"call","de":"gc","a":"rc","txt":"Confirmar(idCompra, entradas, seguros)"},
 {"t":"note","de":"rc","txt":"Transacción: Confirmada, libera la reserva y graba las pólizas"},
 {"t":"call","de":"gc","a":"gf","txt":"ReevaluarNivel() / AscenderSiCorresponde()   « RN-22 »"},
 {"t":"return","de":"gc","a":"fp","txt":"true"},
 {"t":"frag_else","txt":"[pago rechazado]"},
 {"t":"self","de":"gc","txt":"Liberar(compra)   « RN-32: devuelve el cupo en el acto »"},
 {"t":"return","de":"gc","a":"fp","txt":"false"},
 {"t":"frag_end","txt":""},
]
print(render("d_sec2.png", P, M, ancho_col=250, paso=50, fs=17))
