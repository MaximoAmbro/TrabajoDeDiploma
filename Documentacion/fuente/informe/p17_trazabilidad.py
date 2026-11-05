# -*- coding: utf-8 -*-
"""Punto 17 — Matriz de trazabilidad."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "Trazabilidad.pdf")
d = Doc(OUT)

d.h1("Matriz de trazabilidad")

d.p("La matriz de trazabilidad verifica que cada elemento del proyecto tenga origen y destino: "
    "que ningún objetivo quede sin requerimiento que lo realice, que ninguna regla de negocio "
    "flote sin un requerimiento que la invoque, y que ningún caso de uso exista sin haber sido "
    "pedido. Su utilidad no reside en exhibir consistencia sino en localizar los puntos donde "
    "falta.")

d.h2("Criterio de construcción")
d.p("Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y seis "
    "reglas, los ocho casos de uso y los diez casos de prueba resulta ilegible en formato A4 y no "
    "aporta información adicional. Se adopta en su lugar una matriz encadenada: una fila por "
    "requerimiento funcional y una columna por cada eslabón de la cadena. El recorrido puede "
    "hacerse en ambos sentidos, desde el objetivo hacia la prueba o a la inversa.")

d.h2("Nomenclatura de los ejes")
d.table(
    ["Eje", "Prefijo", "Cant.", "Sección donde se define"],
    [["Objetivos específicos", "OE-xx", "6", "Objetivos del proyecto"],
     ["Requerimientos funcionales", "RF-xx", "13", "Definición de requerimientos"],
     ["Requerimientos no funcionales", "RNF-xx", "5", "Definición de requerimientos"],
     ["Reglas de negocio", "RN-xx", "36", "Reglas de negocio (RN-27 dada de baja)"],
     ["Casos de uso", "CU-xx", "8", "Especificaciones de casos de uso"],
     ["Casos de prueba", "CP-xx", "10", "Desarrollo de la lógica de descuento por nivel"],
     ["Hallazgos de usabilidad", "H-xx", "6", "Informe de pruebas de usabilidad"]],
    widths=[142, 52, 40, 217], aligns=["l", "c", "c", "l"])

# ==================================================================== MATRIZ PRINCIPAL
d.h1("Matriz principal")
d.p("Cada fila recorre la cadena completa que parte de un requerimiento funcional. El guion "
    "indica ausencia de vínculo, y toda ausencia se retoma en el análisis de brechas.")
d.table(
    ["RF", "OE", "MoSCoW", "Reglas de negocio", "CU", "Casos de prueba", "Hall."],
    [["RF-01", "OE-2", "Must", "RN-01, RN-02, RN-03", "CU-04", "—", "—"],
     ["RF-02", "OE-2", "Must", "RN-04, RN-05", "CU-05", "—", "—"],
     ["RF-03", "OE-2", "Must", "RN-09", "CU-01", "—", "—"],
     ["RF-04", "OE-2", "Must", "RN-08, RN-12, RN-33", "CU-01", "—", "H-03"],
     ["RF-05", "OE-2", "Must", "RN-13, RN-14, RN-28, RN-29", "CU-01", "—", "H-04"],
     ["RF-06", "OE-5", "Must", "RN-15, RN-16, RN-30, RN-31, RN-32", "CU-01", "CP-07", "H-02"],
     ["RF-07", "OE-2", "Must", "RN-17, RN-18", "CU-01", "—", "—"],
     ["RF-08", "OE-3", "Should", "RN-19 a RN-24", "CU-01, CU-03",
      "CP-01 a CP-06, CP-08, CP-09", "H-01, H-05"],
     ["RF-09", "—", "Could", "—", "—", "—", "—"],
     ["RF-10", "OE-6 ind.", "Could", "RN-10, RN-35, RN-36", "CU-07", "—", "—"],
     ["RF-11", "OE-6", "Should", "RN-34", "CU-08", "—", "—"],
     ["RF-12", "OE-2", "Must", "RN-06, RN-07, RN-11", "CU-02", "—", "—"],
     ["RF-13", "OE-3", "Should", "RN-25, RN-26, RN-37", "CU-06", "CP-10", "—"]],
    widths=[32, 42, 42, 118, 56, 100, 61], aligns=["c", "c", "c", "l", "c", "l", "c"], size=8.0)
d.p("La indicación «ind.» en RF-10 señala un vínculo indirecto: la lista de espera no realiza por "
    "sí misma ningún objetivo, pero alimenta el reporte RPT-05 de demanda insatisfecha, que sí "
    "forma parte del objetivo específico 6.")

# ==================================================================== OBJETIVOS
d.keep(260)
d.h2("Trazabilidad inversa de los objetivos específicos")
d.p("La lectura en sentido contrario responde la pregunta que interesa al evaluador: para cada "
    "objetivo comprometido, qué lo realiza y dónde consta la evidencia.")
d.table(
    ["OE", "Objetivo", "Requerimientos", "Evidencia documentada", "Estado"],
    [["OE-1", "Documentar el proceso actual de venta", "—",
      "Minutas de reunión, Diagrama de Actividad y Modelo de dominio", "Cumplido"],
     ["OE-2", "Prototipo navegable del flujo completo de compra", "RF-01 a RF-07, RF-12",
      "Prototipos de interfaz, Instructivo del requerimiento core e Informe de pruebas de "
      "usabilidad", "Cumplido"],
     ["OE-3", "Simular la fidelización por rankeo y suscripciones", "RF-08, RF-13",
      "Desarrollo de la lógica de descuento por nivel y diez casos de prueba", "Cumplido"],
     ["OE-4", "Evaluar la usabilidad general", "RNF-01, RNF-02",
      "Informe de pruebas de usabilidad: SUS 74,5 y 80 % de satisfacción", "Cumplido"],
     ["OE-5", "Modelar la integración con una pasarela segura", "RF-06, RNF-04",
      "Flujo de pago de CU-01, reglas RN-30 y RN-31", "Cumplido"],
     ["OE-6", "Generar reportes de simulación de gestión", "RF-11",
      "Anexo A02: cinco reportes con fórmulas y ejemplos resueltos", "Cumplido"]],
    widths=[32, 106, 78, 172, 63], aligns=["c", "l", "l", "l", "c"], size=8.0)
d.rich([("Advertencia sobre OE-4. ", "b"),
        ("El objetivo se declara cumplido sobre la base del Informe de pruebas de usabilidad, "
         "cuyas sesiones son simuladas según se declara en su encabezado. El cumplimiento quedará "
         "plenamente acreditado una vez ejecutadas las sesiones con participantes reales, "
         "previstas para la Fase 8 del cronograma.", "r")])

# ==================================================================== COBERTURA CP
d.keep(230)
d.h2("Cobertura de reglas de negocio por casos de prueba")
d.table(
    ["Caso de prueba", "Reglas verificadas"],
    [["CP-01 — Dos compras, nivel Bronce", "RN-19, RN-20"],
     ["CP-02 — Tres compras, nivel Plata", "RN-19, RN-20"],
     ["CP-03 — Seis compras, no asciende", "RN-19"],
     ["CP-04 — Siete compras, nivel Oro", "RN-19, RN-20"],
     ["CP-05 — La base excluye los servicios", "RN-21"],
     ["CP-06 — Ventana móvil de doce meses", "RN-19"],
     ["CP-07 — Precio congelado al iniciar", "RN-16"],
     ["CP-08 — El ascenso rige desde la compra siguiente", "RN-22"],
     ["CP-09 — Reembolso y reevaluación mensual", "RN-23, RN-24"],
     ["CP-10 — La suscripción no acumula porcentaje", "RN-37"]],
    widths=[280, 171])
d.p("Los diez casos verifican ocho reglas distintas —RN-16, RN-19, RN-20, RN-21, RN-22, RN-23, "
    "RN-24 y RN-37— sobre un total de treinta y seis vigentes, lo que arroja una cobertura del "
    "22,2 %. Las veintiocho reglas restantes carecen de verificación automatizada.")

# ==================================================================== ANALISIS
d.keep(230)
d.h2("Análisis de cobertura")
d.table(
    ["Dimensión evaluada", "Total", "Cubierto", "Sin cobertura", "%"],
    [["Objetivos específicos con evidencia documentada", "6", "6", "—", "100 %"],
     ["Requerimientos funcionales con caso de uso", "13", "12", "RF-09", "92,3 %"],
     ["Casos de uso con requerimiento que los origine", "8", "8", "—", "100 %"],
     ["Reglas de negocio con origen declarado", "36", "36", "—", "100 %"],
     ["Requerimientos funcionales con caso de prueba", "13", "3", "10 requerimientos", "23,1 %"],
     ["Reglas de negocio verificadas por caso de prueba", "36", "8", "28 reglas", "22,2 %"],
     ["Hallazgos de usabilidad con acción de mejora", "6", "6", "—", "100 %"],
     ["Hallazgos con prueba de regresión asociada", "6", "0", "6 hallazgos", "0 %"]],
    widths=[214, 40, 52, 92, 53], aligns=["l", "c", "c", "c", "c"])
d.rich([("Lectura del cuadro. ", "b"),
        ("La trazabilidad hacia atrás —de la regla al requerimiento y del requerimiento al "
         "objetivo— es completa. La trazabilidad hacia adelante, del requerimiento a su "
         "verificación, es el punto débil del proyecto: sólo tres de trece requerimientos "
         "cuentan con pruebas, y todas ellas se concentran en la lógica de fidelización.", "r")])

# ==================================================================== BRECHAS
d.h1("Brechas detectadas")
d.p("El análisis identificó cuatro brechas. La primera fue resuelta a partir de este mismo "
    "análisis; las tres restantes quedan documentadas con su acción correctiva.")
d.table(
    ["Nº", "Brecha", "Sev.", "Estado", "Acción"],
    [["G-01", "Los casos de uso CU-02 (Gestionar eventos) y CU-06 (Suscribirse) no contaban con "
              "un requerimiento funcional que los originara, pese a que las reglas RN-06, RN-07 "
              "y RN-11 por un lado, y RN-25, RN-26 y RN-37 por el otro, presuponen su "
              "existencia.", "Alta", "Resuelta",
      "Se incorporaron los requerimientos RF-12 (Gestión de eventos) y RF-13 (Contratación de "
      "suscripciones) a la Definición de requerimientos, y se recalculó la priorización "
      "MoSCoW."],
     ["G-02", "Sólo tres de los trece requerimientos poseen casos de prueba. La cobertura sobre "
              "reglas de negocio alcanza el 22,2 %.", "Alta", "Abierta",
      "Extender el conjunto de pruebas al flujo de compra. Corresponde al anexo de pruebas del "
      "software, no incluido en el alcance de la presente entrega."],
     ["G-03", "RF-09 (Recomendaciones personalizadas) no responde a ningún objetivo específico "
              "ni posee caso de uso asociado.", "Media", "Abierta",
      "Se mantiene como Could. De no incorporarse durante la Fase 8, corresponde reclasificarlo "
      "como Won't have."],
     ["G-04", "Los seis hallazgos del informe de usabilidad no tienen caso de prueba de "
              "regresión.", "Media", "Abierta",
      "Definir un caso de prueba por hallazgo antes de aplicar las correcciones previstas en la "
      "Fase 6."]],
    widths=[30, 160, 34, 50, 177], aligns=["c", "l", "c", "c", "l"], size=8.0)
d.rich([("Observación sobre G-03. ", "b"),
        ("La matriz confirma por una vía independiente la decisión ya adoptada en la "
         "priorización: RF-09 había sido clasificado como Could por ser el requerimiento más "
         "costoso sin objetivo asociado. Que el análisis de trazabilidad llegue al mismo "
         "resultado partiendo de otro criterio refuerza la validez de ambos.", "r")])

d.keep(190)
d.h2("Mantenimiento de la matriz")
d.p("La matriz pierde valor si no se actualiza junto con el resto del documento. Se establecen "
    "las siguientes reglas de mantenimiento:")
d.numbered(1, "Todo requerimiento nuevo se incorpora con su objetivo, su categoría MoSCoW y su "
              "caso de uso antes de iniciar su construcción.")
d.numbered(2, "Toda regla de negocio nueva declara su requerimiento y su caso de uso de origen en "
              "la columna Origen de su tabla.")
d.numbered(3, "Todo caso de prueba nuevo declara qué reglas verifica, de modo que el porcentaje "
              "de cobertura pueda recalcularse.")
d.numbered(4, "La matriz se revisa al cierre de cada fase del cronograma. Las brechas abiertas se "
              "arrastran hasta su resolución o su aceptación explícita.", gap=True)
d.p("El presente análisis deja constancia de su propia utilidad: la brecha G-01 permaneció "
    "invisible durante toda la elaboración del informe y sólo se hizo evidente al construir la "
    "matriz. Dos casos de uso especificados en detalle, con seis reglas de negocio asociadas, "
    "respondían a funcionalidades que nunca habían sido declaradas como requerimiento.")

d.save()
print("OK ->", OUT)
