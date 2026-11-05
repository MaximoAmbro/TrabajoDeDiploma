# -*- coding: utf-8 -*-
"""Punto 16 — Priorizacion MoSCoW de requerimientos."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "MoSCoW_Priorizacion.pdf")
d = Doc(OUT)

d.h1("Priorización de requerimientos")

d.p("Los once requerimientos funcionales y los cinco no funcionales enunciados anteriormente no "
    "poseen la misma criticidad ni el mismo costo de construcción. Priorizarlos permite decidir "
    "qué se construye primero, qué puede postergarse y qué se descarta si el tiempo disponible se "
    "reduce. Sin esa decisión tomada de antemano, el recorte de alcance termina resolviéndose "
    "sobre la fecha de entrega y de manera improvisada.")

d.h2("Método adoptado")
d.p("Se aplica la técnica MoSCoW, que clasifica cada requerimiento en una de cuatro categorías "
    "excluyentes. Su ventaja frente a una escala numérica de prioridad es que obliga a declarar "
    "qué queda afuera, y no solamente qué va primero.")
d.table(
    ["Categoría", "Definición operativa adoptada", "Pregunta de decisión"],
    [["Must", "El entregable carece de sentido sin este requerimiento.",
      "¿Puede presentarse el prototipo sin esto?"],
     ["Should", "Importante y comprometido, pero el entregable sigue siendo válido si se "
                "posterga.", "¿El objetivo general se cumple igual?"],
     ["Could", "Agrega valor percibido; se incorpora únicamente si queda margen de tiempo.",
      "¿Su ausencia se nota?"],
     ["Won't", "Explícitamente fuera del alcance de esta versión del producto.",
      "¿Está fuera de las restricciones del proyecto?"]],
    widths=[52, 233, 166], aligns=["l", "l", "l"])

d.h2("Criterio de asignación")
d.p("La categoría Must se asignó tomando como referencia el objetivo general del proyecto —"
    "disponer de un prototipo navegable que reproduzca el flujo completo de compra— y no cada uno "
    "de los objetivos específicos por separado.")
d.rich([("Fundamento de esta decisión. ", "b"),
        ("Los objetivos específicos 3 y 6 comprometen el sistema de fidelización por rankeo "
         "(RF-08) y los reportes de gestión (RF-11). Clasificarlos como Must por el solo hecho de "
         "figurar en un objetivo llevaría a que nueve de los once requerimientos funcionales "
         "quedaran en esa categoría, concentrando alrededor del 85 % del esfuerzo. Una "
         "priorización en la que casi todo resulta obligatorio no prioriza nada: pierde su "
         "utilidad como herramienta de decisión frente a un atraso. Por esa razón ambos "
         "requerimientos se clasifican como Should: son compromisos asumidos para las fases 6 y 7 "
         "del cronograma, pero su ausencia no invalidaría el prototipo como entregable.", "r")])

d.h2("Escala de esfuerzo")
d.p("El esfuerzo se expresa en puntos, donde un punto equivale a cuatro horas de trabajo "
    "efectivo. La estimación total del desarrollo asciende a 78 puntos, es decir 312 horas, "
    "cifra compatible con el presupuesto horario previsto por la cátedra para la asignatura "
    "considerando los dos integrantes del equipo, aunque con menor holgura que la estimación "
    "inicial. Los valores se asignaron por comparación relativa entre requerimientos, no por "
    "estimación absoluta.")

# ------------------------------------------------------------------ RF
d.keep(300)
d.h2("Priorización de requerimientos funcionales")
d.table(
    ["ID", "Requerimiento", "MoSCoW", "Pts", "Justificación de la categoría"],
    [["RF-01", "Registro de usuarios", "Must", "2",
      "Sin cuenta no hay compra, historial ni nivel de fidelidad."],
     ["RF-02", "Inicio de sesión", "Must", "3",
      "Puerta de acceso al sistema; incluye el bloqueo previsto en RN-04."],
     ["RF-03", "Búsqueda de eventos", "Must", "5",
      "Primer paso del flujo que el objetivo general exige reproducir."],
     ["RF-04", "Selección de entradas", "Must", "5",
      "Sin selección no hay objeto de compra."],
     ["RF-05", "Armado de paquetes personalizados", "Must", "13",
      "Requerimiento core del proyecto y principal diferencial frente a una ticketera "
      "tradicional."],
     ["RF-06", "Simulación de pago seguro", "Must", "8",
      "Exigido por el criterio de aceptación 4 y por el objetivo específico 5."],
     ["RF-07", "Confirmación de compra", "Must", "5",
      "Cierra el flujo principal; sin esta pantalla el recorrido queda trunco."],
     ["RF-08", "Aplicación de descuentos por rankeo", "Should", "5",
      "Objetivo específico 3. El flujo de compra opera correctamente sin el descuento."],
     ["RF-11", "Generación de reportes de ventas", "Should", "8",
      "Objetivo específico 6. No interviene en el recorrido del cliente."],
     ["RF-09", "Recomendaciones personalizadas", "Could", "8",
      "El más costoso de los no obligatorios y sin objetivo específico asociado."],
     ["RF-10", "Lista de espera para eventos agotados", "Could", "3",
      "Costo bajo; se incorpora si el avance de los Must lo permite."],
     ["RF-12", "Gestión de eventos", "Must", "8",
      "Sin eventos cargados no hay nada que vender: condiciona a todos los demás."],
     ["RF-13", "Contratación de suscripciones", "Should", "5",
      "Genera ingreso propio, pero el flujo de compra opera con independencia de ella."]],
    widths=[32, 104, 44, 24, 247], aligns=["c", "l", "c", "c", "l"])

# ------------------------------------------------------------------ distribucion
d.keep(190)
d.h2("Distribución del esfuerzo")
d.table(
    ["Categoría", "Requerimientos", "Puntos", "Participación", "Referencia habitual"],
    [["Must", "8", "49", "62,8 %", "≤ 60 %"],
     ["Should", "3", "18", "23,1 %", "≈ 20 %"],
     ["Could", "2", "11", "14,1 %", "≈ 20 %"],
     ["TOTAL", "13", "78", "100 %", "—"]],
    widths=[74, 98, 60, 96, 123], aligns=["l", "c", "c", "c", "c"])
d.rich([("Observación sobre el desvío. ", "b"),
        ("La categoría Must concentra el 62,8 % del esfuerzo, casi tres puntos por encima del "
         "máximo habitualmente recomendado. El desvío se declara en lugar de corregirse artificialmente, "
         "porque responde a una característica del producto y no a una falla de la estimación: el "
         "flujo de compra es indivisible como entregable. No es posible presentar un prototipo que "
         "permita buscar y seleccionar un evento pero no confirmar la operación. La mitigación "
         "consiste en que la categoría Could funcione como amortiguador: ante cualquier atraso se "
         "descartan sus once puntos sin renegociar el alcance comprometido. La incorporación de "
         "RF-12 y RF-13, derivada del análisis de trazabilidad, redujo levemente el desvío en "
         "lugar de agravarlo.", "r")])

# ------------------------------------------------------------------ RNF
d.keep(200)
d.h2("Priorización de requerimientos no funcionales")
d.p("Los requerimientos no funcionales se priorizan por separado porque no compiten por el mismo "
    "esfuerzo de construcción: operan como restricciones transversales sobre los requerimientos "
    "funcionales ya clasificados.")
d.table(
    ["ID", "Requerimiento no funcional", "MoSCoW", "Justificación de la categoría"],
    [["RNF-01", "Usabilidad (flujo completo en menos de 5 minutos)", "Must",
      "Figura como criterio de aceptación explícito del producto."],
     ["RNF-02", "Experiencia de usuario", "Must",
      "Condiciona directamente la evaluación del objetivo específico 4."],
     ["RNF-04", "Seguridad conceptual (HTTPS y tokenización)", "Must",
      "Requerido por el objetivo específico 5 y por la regla RN-30."],
     ["RNF-05", "Mantenibilidad y documentación", "Must",
      "Exigido por la Carpeta Modelo como condición de la entrega."],
     ["RNF-03", "Compatibilidad multipantalla", "Should",
      "Deseable, pero la evaluación del prototipo se realiza en escritorio."]],
    widths=[42, 146, 46, 217], aligns=["c", "l", "c", "l"])

# ------------------------------------------------------------------ WONT
d.keep(230)
d.h2("Requerimientos excluidos — categoría Won't have")
d.p("La categoría Won't have delimita por escrito el borde del alcance. Su función es evitar que "
    "una funcionalidad ausente se interprete como una omisión: se trata de exclusiones "
    "deliberadas y fundamentadas para esta versión del producto.")
d.table(
    ["ID", "Funcionalidad excluida", "Motivo de la exclusión"],
    [["W-01", "Integración con pasarela de pago real",
      "Restricción explícita del proyecto: no se procesan transacciones reales."],
     ["W-02", "Aplicación móvil nativa",
      "Fuera del alcance tecnológico definido (interfaz en Windows Forms)."],
     ["W-03", "Notificaciones automáticas por correo o push",
      "Requiere infraestructura de terceros no disponible en el entorno de práctica."],
     ["W-04", "Reventa entre usuarios o mercado secundario",
      "No integra el modelo de negocio de esta versión."],
     ["W-05", "Interfaz en múltiples idiomas",
      "Corresponde al anexo técnico T05, no implementado en la presente entrega."],
     ["W-06", "Validación de acceso mediante lectura de QR en puerta",
      "Requiere hardware específico. El código QR se genera pero no se valida."]],
    widths=[36, 160, 255], aligns=["c", "l", "l"])

# ------------------------------------------------------------------ ORDEN
d.keep(200)
d.h2("Orden de implementación derivado")
d.p("La priorización se traduce en el siguiente ordenamiento, que respeta los entregables ya "
    "comprometidos en el cronograma de hitos del proyecto.")
d.table(
    ["Fase del cronograma", "Categoría", "Requerimientos", "Pts"],
    [["Fase 4 — Desarrollo del prototipo, 1ª iteración", "Must",
      "RF-12, RF-01, RF-02, RF-03, RF-04, RF-05, RF-07", "41"],
     ["Fase 6 — Desarrollo ampliado, 2ª iteración", "Must y Should",
      "RF-06, RF-08, RF-13", "18"],
     ["Fase 7 — Generación de reportes simulados", "Should", "RF-11", "8"],
     ["Fase 8 — Ajustes finales", "Could", "RF-09 y RF-10, según margen disponible", "11"]],
    widths=[168, 76, 175, 32], aligns=["l", "c", "l", "c"])
d.rich([("Riesgo de concentración. ", "b"),
        ("La Fase 4 absorbe 41 de los 78 puntos, es decir el 52,6 % del esfuerzo total. RF-12 se "
         "ubica primero dentro de esa fase porque sin eventos cargados en el sistema no resulta "
         "posible ejercitar ninguno de los requerimientos restantes. Esa concentración constituye "
         "el principal riesgo de cronograma del proyecto y obliga a verificar el avance antes de "
         "iniciar la Fase 5.", "r")])
d.p("La Fase 5, de validación y revisión intermedia, no incorpora requerimientos nuevos: su "
    "propósito es verificar con usuarios reales los Must ya construidos en la Fase 4, antes de "
    "comprometer esfuerzo en las categorías siguientes.")

# ------------------------------------------------------------------ REPRIORIZACION
d.keep(170)
d.h2("Regla de repriorización")
d.p("La priorización no es un documento estático. Se establece el siguiente procedimiento para el "
    "caso de que el avance real se aparte de lo planificado:")
d.numbered(1, [("Punto de control. ", "b"),
               ("Al cierre de la Fase 6 se verifica el porcentaje de avance de los requerimientos "
                "Must.", "r")])
d.numbered(2, [("Si los Must no alcanzan el 100 %. ", "b"),
               ("Se descarta íntegramente la categoría Could (RF-09 y RF-10) y el esfuerzo "
                "liberado se reasigna a completarlos.", "r")])
d.numbered(3, [("Si el desvío persiste. ", "b"),
               ("Los Should se reducen a su versión mínima: los reportes se presentan en formato "
                "de tabla, sin gráficos ni exportación.", "r")])
d.numbered(4, [("Límite infranqueable. ", "b"),
               ("Los requerimientos Must no se negocian. Antes de recortar uno de ellos se "
                "solicita una extensión del plazo, por tratarse del núcleo comprometido con el "
                "cliente.", "r")], gap=True)
d.p("Toda modificación de categoría posterior a la aprobación de este documento debe registrarse "
    "en el historial de revisión, indicando fecha, requerimiento afectado, categoría anterior, "
    "categoría nueva y motivo del cambio.")

d.save()
print("OK ->", OUT)
