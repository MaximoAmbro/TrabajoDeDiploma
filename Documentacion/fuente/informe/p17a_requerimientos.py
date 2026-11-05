# -*- coding: utf-8 -*-
"""Definicion de requerimientos — regenerada con RF-12 y RF-13."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc, ML, BUL1_TX, BUL2_TX, MR

OUT = os.path.join(_SAL, "Requerimientos.pdf")
d = Doc(OUT)

def rf(n, titulo, descripcion, criterio):
    d.keep(66)
    d.numbered(n, titulo)
    d.p(descripcion, gap=False, x=BUL1_TX + 2, width=MR - BUL1_TX - 2)
    d.rich([("Criterio de aceptación: ", "i"), (criterio, "r")],
           gap=False, x=BUL1_TX + 2, width=MR - BUL1_TX - 2)
    d.space(10.7)

d.h1("Definición de requerimientos")

d.h2("Requerimientos funcionales")

rf(1, "Registro de usuarios",
   "Permitir que cualquier usuario cree una cuenta ingresando su correo electrónico y una "
   "contraseña.",
   "El sistema debe validar el formato del correo, registrar al usuario y permitir luego el "
   "inicio de sesión.")

rf(2, "Inicio de sesión",
   "Permitir a los usuarios autenticarse con sus credenciales previamente registradas.",
   "El sistema debe verificar los datos ingresados y redirigir al perfil del usuario.")

rf(3, "Búsqueda de eventos",
   "Permitir filtrar eventos por nombre, categoría, fecha o ubicación.",
   "Al ingresar un término de búsqueda, se debe mostrar una lista de resultados relevantes.")

rf(4, "Selección de entradas",
   "Permitir elegir tipo y cantidad de entradas por evento.",
   "Las entradas seleccionadas deben agregarse correctamente al paquete o carrito.")

rf(5, "Armado de paquetes personalizados",
   "Combinar entradas con servicios adicionales como hotel, traslado, merchandising o acceso "
   "VIP.",
   "El sistema debe generar un resumen claro del paquete armado.")

rf(6, "Simulación de pago seguro",
   "Representar el flujo de ingreso de tarjeta de crédito sin realizar transacciones reales.",
   "El prototipo debe mostrar tokenización simulada y una confirmación visual del pago.")

rf(7, "Confirmación de compra",
   "Generar una pantalla final con el resumen del paquete comprado y un código de operación.",
   "El sistema debe mostrar un mensaje de compra exitosa con identificación del pedido.")

rf(8, "Aplicación de descuentos por rankeo",
   "Permitir aplicar beneficios según el nivel del usuario (Bronce, Plata, Oro).",
   "El sistema debe mostrar el descuento aplicado en el resumen de compra.")

rf(9, "Recomendaciones personalizadas",
   "Sugerir eventos similares basados en compras o preferencias anteriores.",
   "El sistema debe mostrar al menos dos recomendaciones al usuario luego de la compra.")

rf(10, "Lista de espera para eventos agotados",
   "Permitir a los usuarios anotarse si no hay entradas disponibles.",
   "El sistema debe registrar al usuario en la lista e informarle que será notificado.")

rf(11, "Generación de reportes de ventas (simulados)",
   "Crear informes visuales de ventas por evento, paquetes más vendidos y adopción de seguros.",
   "El sistema debe mostrar tablas o gráficos con datos ficticios exportables.")

rf(12, "Gestión de eventos",
   "Permitir al Administrador crear, modificar y desactivar eventos, definiendo sus sectores, "
   "capacidades y precios.",
   "El sistema debe impedir la publicación de un evento que no tenga al menos un sector con "
   "capacidad mayor que cero, y no debe permitir eliminar un evento que ya registre entradas "
   "vendidas.")

rf(13, "Contratación de suscripciones",
   "Permitir al Cliente contratar un plan de suscripción mensual o anual y acceder a los "
   "beneficios asociados.",
   "El sistema debe impedir que un cliente mantenga dos suscripciones activas en forma "
   "simultánea, y debe aplicar los beneficios en especie sin otorgar descuento porcentual sobre "
   "el valor de la entrada.")

d.space(4)
d.rich([("Nota sobre los requerimientos RF-12 y RF-13. ", "b"),
        ("Ambos fueron incorporados a partir del análisis de trazabilidad incluido al final del "
         "presente informe, que detectó la existencia de casos de uso y de reglas de negocio sin "
         "un requerimiento funcional que los originara: CU-02 y las reglas RN-06, RN-07 y RN-11 "
         "en el caso de la gestión de eventos, y CU-06 junto con las reglas RN-25, RN-26 y RN-37 "
         "en el de las suscripciones.", "r")])

# ------------------------------------------------------------------ RNF
d.keep(200)
d.h2("Requerimientos no funcionales")
d.p("Además de las funcionalidades descriptas, el sistema EventAura deberá cumplir con los "
    "siguientes requerimientos no funcionales:")

def rnf(n, titulo, puntos):
    d.keep(30 + 30 * len(puntos))
    d.numbered(n, [(titulo, "b")])
    for t in puntos:
        d.bullet(t, level=2)
    d.space(10.7)

rnf(1, "Usabilidad", [
    "El flujo principal de compra (buscar evento, armar paquete, simular pago y confirmar) debe "
    "poder completarse en un tiempo aproximado menor a 5 minutos por un usuario sin experiencia "
    "técnica.",
    "La interfaz debe ser clara, con elementos bien identificados y mensajes de error "
    "entendibles."])

rnf(2, "Experiencia de usuario", [
    "La navegación entre pantallas debe ser fluida, evitando pasos innecesarios.",
    "Las acciones principales (buscar, agregar al carrito, continuar al pago, confirmar compra) "
    "deben estar siempre visibles o fácilmente accesibles."])

rnf(3, "Compatibilidad técnica", [
    "El prototipo debe poder visualizarse correctamente en navegadores modernos (Chrome, "
    "Firefox, Edge) sin requerir instalación de software adicional.",
    "El diseño debe adaptarse de forma adecuada a diferentes tamaños de pantalla (notebook, "
    "monitor de escritorio y, en la medida de lo posible, tablet o móvil)."])

rnf(4, "Seguridad conceptual", [
    "El flujo de pago simulado debe representar el uso de conexión segura (HTTPS) y tokenización "
    "de tarjeta, sin almacenar datos sensibles ni números de tarjeta completos.",
    "Los mensajes del prototipo deben dejar en claro que se trata de una simulación y no de una "
    "transacción real."])

rnf(5, "Mantenibilidad y documentación", [
    "El sistema debe contar con documentación clara (modelo de dominio, diagramas UML, criterios "
    "de aceptación) que permita a un equipo técnico retomar el proyecto en futuras iteraciones.",
    "Los cambios relevantes en el diseño deben registrarse en minutas o en una hoja de cambios, "
    "indicando fecha, autor y descripción breve."])

d.save()
print("OK ->", OUT)
