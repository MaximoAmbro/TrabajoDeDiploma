# -*- coding: utf-8 -*-
"""Punto 18 — Informe de pruebas de usabilidad (sesiones simuladas)."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "Pruebas_Usabilidad.pdf")
d = Doc(OUT)

d.h1("Informe de pruebas de usabilidad")

d.rich([("Declaración sobre la naturaleza de los datos. ", "b"),
        ("Las cinco sesiones documentadas en este informe son simuladas. Los participantes, los "
         "tiempos, los errores y las respuestas a los cuestionarios no provienen de usuarios "
         "reales: fueron construidos como ejercicio metodológico para la presente entrega. El "
         "diseño de la prueba, el protocolo de moderación, los instrumentos de medición y el plan "
         "de mejora sí constituyen metodología aplicable y conservan validez con independencia de "
         "los valores registrados. Se prevé ejecutar las sesiones con participantes reales una vez "
         "finalizado el desarrollo del prototipo, reemplazando las tablas de resultados sin "
         "modificar la estructura del informe.", "r")])

d.h2("Objetivo de la evaluación")
d.p("Verificar que un usuario sin experiencia técnica pueda completar el flujo de armado y compra "
    "de un paquete personalizado —el requerimiento core del sistema— dentro del tiempo "
    "comprometido, e identificar los puntos del recorrido que generan confusión o error.")

d.h2("Diseño de la prueba")
d.table(
    ["Aspecto", "Definición adoptada"],
    [["Tipo de estudio", "Prueba de tareas moderada, presencial, con protocolo de pensamiento en "
                         "voz alta."],
     ["Cantidad de participantes", "Cinco."],
     ["Objeto evaluado", "Flujo de ocho pasos descripto en el Instructivo del requerimiento "
                         "core."],
     ["Duración por sesión", "Entre veinte y treinta minutos, incluida la entrevista final."],
     ["Registro", "Cronometraje por paso, planilla de errores y dos cuestionarios de cierre."]],
    widths=[128, 323])
d.rich([("Sobre el tamaño de la muestra. ", "b"),
        ("El objetivo específico 2 del proyecto exige al menos tres participantes y la Fase 5 del "
         "cronograma prevé dos o tres. Se adoptaron cinco siguiendo el modelo de Nielsen y "
         "Landauer (1993), según el cual una prueba cualitativa con cinco participantes revela "
         "aproximadamente el 85 % de los problemas de usabilidad de una interfaz. Superar el "
         "mínimo exigido con un criterio fundamentado resulta preferible a cumplirlo de forma "
         "ajustada.", "r")])

d.keep(200)
d.h2("Perfil de los participantes")
d.p("Se buscó cubrir el rango de edad y de experiencia previa del público objetivo, incluyendo "
    "deliberadamente un participante sin experiencia en compras en línea.")
d.table(
    ["Part.", "Edad", "Ocupación", "Experiencia en compra de entradas en línea"],
    [["P1", "21", "Estudiante universitario", "Alta — compra habitualmente"],
     ["P2", "34", "Empleada administrativa", "Media — compró en varias oportunidades"],
     ["P3", "52", "Comerciante", "Nula — nunca compró entradas en línea"],
     ["P4", "19", "Estudiante secundario", "Alta — compra habitualmente"],
     ["P5", "45", "Docente", "Media — compró en alguna oportunidad"]],
    widths=[42, 36, 148, 225], aligns=["c", "c", "l", "l"])

d.keep(210)
d.h2("Tarea evaluada y protocolo de moderación")
d.rich([("Escenario leído al participante. ", "b"),
        ("«Querés ir a ver a Duki en Rosario con un acompañante. Además de las entradas, "
         "necesitás dónde alojarte esa noche. Comprá lo necesario usando la plataforma.»", "i")])
d.p("El escenario no menciona pantallas ni botones: describe una necesidad, de modo que el "
    "recorrido que el participante elija sea información en sí misma.")
d.table(
    ["Regla de moderación", "Fundamento"],
    [["El participante verbaliza lo que piensa mientras opera.",
      "Permite distinguir un error de ejecución de un problema de comprensión."],
     ["El moderador no interviene salvo bloqueo superior a sesenta segundos.",
      "Evita inducir el recorrido y mantiene comparables los tiempos."],
     ["Toda asistencia se registra con su momento y su motivo.",
      "La sesión asistida no se computa como éxito autónomo."],
     ["No se responden consultas hasta finalizada la tarea.",
      "Las dudas surgidas durante el recorrido son el dato buscado."]],
    widths=[208, 243])
d.rich([("Criterio de éxito. ", "b"),
        ("Alcanzar la pantalla de confirmación con un paquete que incluya al menos una entrada y "
         "un servicio complementario, sin asistencia del moderador.", "r")])

d.keep(200)
d.h2("Métricas e instrumentos")
d.table(
    ["Métrica", "Forma de medición", "Umbral comprometido"],
    [["Tasa de éxito", "Proporción de participantes que alcanzan la confirmación sin asistencia.",
      "3 de 5"],
     ["Tiempo de ejecución", "Cronómetro desde el ingreso hasta la confirmación.",
      "Menos de 5 min (RNF-01)"],
     ["Errores", "Acciones que obligan a rehacer un paso del recorrido.",
      "Hasta 2 por participante"],
     ["Satisfacción por criterio", "Cuestionario de seis criterios, escala de 1 a 5.",
      "Promedio mayor que 4"],
     ["System Usability Scale", "Cuestionario estándar de diez ítems (Brooke, 1996).",
      "68 o más"]],
    widths=[104, 234, 113], aligns=["l", "l", "c"])

# ==================================================================== RESULTADOS
d.h1("Resultados")

d.h2("Resultados por participante")
d.table(
    ["Part.", "Completó la tarea", "Tiempo", "Errores", "SUS"],
    [["P1", "Sí, sin asistencia", "3:15", "0", "87,5"],
     ["P2", "Sí, sin asistencia", "4:32", "1", "77,5"],
     ["P3", "Sí, con asistencia", "7:48", "4", "55,0"],
     ["P4", "Sí, sin asistencia", "3:41", "1", "82,5"],
     ["P5", "Sí, sin asistencia", "5:26", "2", "70,0"],
     ["", "PROMEDIO", "4:56", "1,6", "74,5"]],
    widths=[52, 168, 82, 74, 75], aligns=["c", "l", "c", "c", "c"])
d.p("La tasa de éxito sin asistencia es del 80 %: cuatro de los cinco participantes completaron "
    "el recorrido por sus propios medios. El tiempo promedio de 4 min 56 s se ubica por debajo "
    "del límite de cinco minutos, aunque dos participantes lo excedieron. Excluyendo la sesión "
    "asistida, el promedio desciende a 4 min 14 s.")
d.rich([("Lectura del caso P3. ", "b"),
        ("El único participante sin experiencia previa en compras en línea requirió asistencia, "
         "acumuló cuatro errores y otorgó el puntaje SUS más bajo. Lejos de constituir un caso "
         "atípico descartable, es el perfil que el requerimiento no funcional RNF-01 nombra "
         "expresamente —«un usuario sin experiencia técnica»— y por lo tanto el que mejor mide el "
         "cumplimiento del compromiso asumido.", "r")])

d.keep(230)
d.h2("Observaciones registradas por participante")
d.table(
    ["Part.", "Errores observados y comentarios relevantes"],
    [["P1", "Ningún error. Completó el recorrido sin detenerse. Consultó al finalizar qué "
            "significaba el indicador de nivel."],
     ["P2", "Presionó «Volver» para corregir la cantidad de entradas y perdió la selección "
            "completa. Preguntó por qué el hospedaje no figuraba con descuento."],
     ["P3", "No localizó el filtro por fecha. Solicitó más entradas que el cupo del sector. "
            "Intentó marcar hospedaje sin haber seleccionado entrada. Dejó vencer la reserva y "
            "debió reiniciar."],
     ["P4", "Solicitó más entradas que el cupo disponible. Recorrido fluido en el resto."],
     ["P5", "Dejó vencer la reserva durante la carga de los datos de pago. Intentó contratar el "
            "seguro una vez confirmada la compra."]],
    widths=[42, 409])

d.keep(230)
d.h2("Tiempo por paso frente a la línea base")
d.p("El Instructivo del requerimiento core estima el recorrido en 4 min 40 s. La comparación por "
    "paso permite localizar dónde se concentra el desvío.")
d.table(
    ["Nº", "Paso", "Estimado", "Observado", "Desvío"],
    [["1", "Buscar el evento", "45 s", "48 s", "+3 s"],
     ["2", "Seleccionar el evento", "15 s", "13 s", "−2 s"],
     ["3", "Elegir sector y cantidad de entradas", "40 s", "55 s", "+15 s"],
     ["4", "Agregar servicios complementarios", "60 s", "63 s", "+3 s"],
     ["5", "Decidir la contratación del seguro", "20 s", "17 s", "−3 s"],
     ["6", "Revisar el resumen del paquete", "30 s", "49 s", "+19 s"],
     ["7", "Completar el pago simulado", "60 s", "43 s", "−17 s"],
     ["8", "Confirmar y recibir el comprobante", "10 s", "8 s", "−2 s"],
     ["", "TOTAL", "4:40", "4:56", "+16 s"]],
    widths=[26, 235, 66, 68, 56], aligns=["c", "l", "c", "c", "c"])
d.rich([("Interpretación. ", "b"),
        ("Los pasos 3 y 6 concentran treinta y cuatro segundos de desvío y son, precisamente, "
         "donde se localizan los dos hallazgos de severidad alta. El paso 7 resultó más rápido de "
         "lo estimado, pero ese dato debe tomarse con cautela: los datos de tarjeta son ficticios "
         "y los participantes no los verificaron. Con datos reales ese paso sería más lento.",
         "r")])

d.keep(220)
d.h2("Satisfacción por criterio — escala de 1 a 5")
d.table(
    ["Criterio evaluado", "P1", "P2", "P3", "P4", "P5", "Prom."],
    [["Claridad de la navegación", "5", "4", "3", "5", "4", "4,2"],
     ["Facilidad para armar el paquete", "5", "4", "2", "4", "4", "3,8"],
     ["Comprensión del resumen y del descuento", "4", "3", "2", "4", "3", "3,2"],
     ["Claridad de los mensajes de error", "4", "4", "3", "4", "3", "3,6"],
     ["Confianza en el proceso de pago", "5", "5", "4", "5", "4", "4,6"],
     ["Satisfacción general", "5", "4", "3", "5", "4", "4,2"],
     ["PROMEDIO GLOBAL", "", "", "", "", "", "3,93"]],
    widths=[221, 36, 36, 36, 36, 36, 50], aligns=["l", "c", "c", "c", "c", "c", "c"])
d.rich([("Hallazgo del cuestionario. ", "b"),
        ("El promedio global de 3,93 queda por debajo del umbral de 4 comprometido en el objetivo "
         "específico 4. El criterio que tracciona el resultado hacia abajo es la comprensión del "
         "resumen y del descuento, con 3,2, el más bajo de los seis. En el extremo opuesto, la "
         "confianza en el proceso de pago obtiene 4,6, lo que indica que la representación de la "
         "tokenización cumple su propósito comunicativo.", "r")])

d.keep(200)
d.h2("System Usability Scale")
d.p("El SUS es un cuestionario estándar de diez ítems que produce un puntaje único entre 0 y 100. "
    "Su valor reside en disponer de un marco de referencia externo: sobre más de cinco mil "
    "mediciones, el promedio de la industria se ubica en 68 puntos.")
d.table(
    ["Puntaje SUS", "Interpretación (Bangor et al., 2009)", "Participantes"],
    [["90 o más", "A — Excelente", "—"],
     ["80 a 89", "B — Bueno", "P1 (87,5) y P4 (82,5)"],
     ["70 a 79", "C — Aceptable", "P2 (77,5) y P5 (70,0)"],
     ["60 a 69", "D — Deficiente", "—"],
     ["Menos de 60", "F — Inaceptable", "P3 (55,0)"]],
    widths=[104, 197, 150], aligns=["c", "l", "l"])
d.p("El promedio de 74,5 puntos supera el valor de referencia de 68 y corresponde a la categoría "
    "C de la escala de Bangor. El resultado es satisfactorio pero no sobresaliente, y la "
    "dispersión entre 55,0 y 87,5 confirma que la experiencia depende fuertemente de la "
    "familiaridad previa del usuario con este tipo de plataformas.")

d.keep(200)
d.h2("Cumplimiento de los criterios comprometidos")
d.table(
    ["Criterio", "Origen", "Umbral", "Resultado", "Veredicto"],
    [["Cantidad de participantes", "Obj. esp. 2", "3 o más", "5", "Cumple"],
     ["Completaron el flujo principal", "Crit. acept. 7", "3 o más", "5", "Cumple"],
     ["Tiempo del flujo completo", "RNF-01", "Menos de 5 min", "4:56 promedio",
      "Cumple con reservas"],
     ["Satisfacción buena o muy buena", "Obj. esp. 4", "70 % o más", "80 %", "Cumple"],
     ["Promedio en escala de 1 a 5", "Obj. esp. 4", "Mayor que 4", "3,93", "No alcanza"],
     ["Puntaje SUS", "Referencia externa", "68 o más", "74,5", "Cumple"],
     ["Errores por participante", "Diseño de la prueba", "Hasta 2", "1,6 promedio; P3 con 4",
      "Cumple con reservas"]],
    widths=[130, 72, 78, 92, 79], aligns=["l", "c", "c", "c", "c"])
d.rich([("Lectura del cuadro. ", "b"),
        ("El objetivo específico 4 se considera alcanzado, dado que admite satisfacer el "
         "porcentaje de satisfacción o el promedio numérico, y el primero se cumple con holgura. "
         "No obstante, el promedio de 3,93 y los dos participantes que superaron los cinco "
         "minutos señalan que el margen es estrecho y que las mejoras identificadas no son "
         "opcionales si se pretende sostener el compromiso con usuarios menos experimentados.",
         "r")])

# ==================================================================== HALLAZGOS
d.h1("Hallazgos y plan de mejora")
d.p("Se consideraron hallazgos aquellos problemas observados en al menos dos participantes, o en "
    "uno solo cuando impidieron completar un paso. La severidad se clasifica en tres niveles: "
    "alta cuando bloquea o retrasa de manera significativa la tarea, media cuando genera una "
    "confusión que el usuario logra superar, y baja cuando constituye una molestia menor.")
d.table(
    ["Nº", "Hallazgo", "Sev.", "Detectado en", "Evidencia"],
    [["H-01", "El resumen no explica por qué el servicio complementario no recibe descuento.",
      "Alta", "P2, P3, P5", "Consultaron al moderador tras ver el total. Criterio de comprensión: "
      "3,2."],
     ["H-02", "La vigencia de la reserva no está a la vista; el límite de diez minutos pasa "
              "inadvertido.", "Alta", "P3, P5",
      "Ambos dejaron vencer la reserva y debieron reiniciar la operación."],
     ["H-03", "El selector de cantidad no informa el cupo restante de cada sector.",
      "Media", "P3, P4", "Ambos solicitaron más entradas que las disponibles."],
     ["H-04", "La advertencia sobre el seguro pasa inadvertida.", "Media", "P3, P5",
      "P5 intentó contratarlo tras confirmar la compra; P3 no recordaba haberla leído."],
     ["H-05", "El indicador de nivel se muestra pero su efecto no se comprende.",
      "Media", "P1, P3, P5", "Ninguno pudo explicar el significado de «Plata — 5 %» al "
      "finalizar."],
     ["H-06", "El botón «Volver» se interpreta como cancelación total de la operación.",
      "Baja", "P2", "Lo presionó para corregir una cantidad y perdió toda la selección."]],
    widths=[32, 140, 36, 66, 177])
d.p("Una observación no fue elevada a la categoría de hallazgo: P3 no localizó el filtro por "
    "fecha en el listado de eventos. Al registrarse en un único participante y no impedir la "
    "continuidad de la tarea, se deja asentada para su verificación en la próxima ronda.")

d.keep(220)
d.h2("Plan de mejora priorizado")
d.table(
    ["Acción correctiva", "Hallazgo", "Esfuerzo", "Fase"],
    [["Incorporar junto al total la leyenda «El descuento se aplica sobre el valor de las "
      "entradas».", "H-01", "1 pt", "Fase 6"],
     ["Mostrar el temporizador de la reserva fijo en el encabezado a partir del paso 7.",
      "H-02", "2 pt", "Fase 6"],
     ["Indicar «Quedan N entradas» junto a cada sector disponible.", "H-03", "2 pt", "Fase 6"],
     ["Convertir la advertencia del seguro en una confirmación explícita al avanzar sin "
      "contratarlo.", "H-04", "1 pt", "Fase 6"],
     ["Agregar un enlace «¿Cómo funcionan los niveles?» junto al indicador de nivel.",
      "H-05", "1 pt", "Fase 8"],
     ["Renombrar el botón a «Volver al listado» y solicitar confirmación antes de descartar.",
      "H-06", "1 pt", "Fase 8"],
     ["TOTAL", "", "8 pt", ""]],
    widths=[277, 62, 62, 50], aligns=["l", "c", "c", "c"])
d.p("El plan insume ocho puntos de esfuerzo, equivalentes a treinta y dos horas de trabajo, es "
    "decir el 12,3 % del esfuerzo total estimado para el desarrollo. Las cuatro acciones que "
    "responden a hallazgos de severidad alta y media se concentran en la Fase 6, de modo que las "
    "correcciones queden incorporadas antes de la generación de reportes.")
d.rich([("Observación relevante. ", "b"),
        ("Ninguna de las seis acciones introduce un requerimiento nuevo. Todas consisten en "
         "comunicar mejor reglas de negocio que ya existen: RN-21 en H-01, RN-15 en H-02, RN-08 "
         "en H-03, RN-28 en H-04 y RN-19 en H-05. El problema detectado no es de definición "
         "funcional sino de comunicación en la interfaz.", "r")])

d.keep(200)
d.h2("Limitaciones del estudio")
d.numbered(1, [("Carácter simulado. ", "b"),
               ("Como se declara al inicio, las sesiones no se realizaron con usuarios reales.",
                "r")])
d.numbered(2, [("Tamaño de muestra. ", "b"),
               ("Cinco participantes revelan alrededor del 85 % de los problemas en promedio, "
                "pero con dispersión considerable: algunos conjuntos de cinco detectan el 99 % y "
                "otros apenas el 55 %.", "r")])
d.numbered(3, [("Pago ficticio. ", "b"),
               ("El paso 7 se ejecuta con datos de tarjeta inventados que nadie verifica, por lo "
                "que su tiempo observado subestima el de una operación real.", "r")])
d.numbered(4, [("Alcance acotado. ", "b"),
               ("Se evaluó únicamente el flujo de armado y compra. La gestión de eventos y los "
                "reportes de administración no fueron objeto de prueba.", "r")])
d.numbered(5, [("Ausencia de medición de aprendizaje. ", "b"),
               ("Cada participante ejecutó la tarea una sola vez. Una segunda ejecución arrojaría "
                "tiempos menores y permitiría distinguir la dificultad inicial de la dificultad "
                "persistente.", "r")], gap=True)

d.keep(170)
d.h2("Conclusión")
d.p("El prototipo permite que un usuario sin conocimientos técnicos complete la compra de un "
    "paquete personalizado dentro del tiempo comprometido, con un nivel de satisfacción por "
    "encima del promedio de la industria. Los criterios formales del proyecto se cumplen.")
d.p("El estudio identifica, sin embargo, un patrón consistente: las dificultades no se presentan "
    "al operar el sistema sino al comprender lo que el sistema decidió. El resumen del paquete, "
    "el nivel de fidelidad y la vigencia de la reserva son reglas correctamente implementadas que "
    "la interfaz no logra comunicar. Las seis acciones del plan de mejora apuntan a esa brecha.")
d.p("Se prevé repetir la evaluación durante la Fase 8 del cronograma, una vez aplicadas las "
    "correcciones de la Fase 6, empleando el mismo protocolo y los mismos instrumentos para que "
    "los resultados sean comparables.")

d.keep(170)
d.h2("Referencias")
d.p("Bangor, A., Kortum, P., & Miller, J. (2009). Determining what individual SUS scores mean: "
    "Adding an adjective rating scale. Journal of Usability Studies, 4(3), 114–123.",
    justify=False)
d.p("Brooke, J. (1996). SUS: A quick and dirty usability scale. En P. W. Jordan, B. Thomas, B. A. "
    "Weerdmeester & I. L. McClelland (Eds.), Usability Evaluation in Industry (pp. 189–194). "
    "Taylor & Francis.", justify=False)
d.p("Nielsen, J., & Landauer, T. K. (1993). A mathematical model of the finding of usability "
    "problems. Proceedings of the INTERCHI '93 Conference on Human Factors in Computing Systems, "
    "206–213.", justify=False)
d.p("Sauro, J., & Lewis, J. R. (2016). Quantifying the User Experience: Practical Statistics for "
    "User Research (2ª ed.). Morgan Kaufmann.", justify=False)

d.save()
print("OK ->", OUT)
