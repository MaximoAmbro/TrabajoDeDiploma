# -*- coding: utf-8 -*-
"""Preliminares y cierre: indice, historial, resumen ejecutivo, conclusiones, bibliografia."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc, ML, MR, LEAD, _y
from reportlab.pdfbase import pdfmetrics

O = os.path.join(_SAL, "")

# ─────────────────────────────────────────────────────────── INDICE
def build_indice(entradas, path):
    """entradas = [(nivel, titulo, pagina_final), ...]"""
    d = Doc(path)
    d.h1("Índice")
    size, lead = 9.5, 12.4
    for niv, txt, pg in entradas:
        d._need(lead)
        x = ML if niv == 1 else ML + 16
        f = "EA-B" if niv == 1 else "EA"
        if niv == 1:
            d.top += 3.0
            d._need(lead)
        num = str(pg)
        wn = pdfmetrics.stringWidth(num, f, size)
        wt = pdfmetrics.stringWidth(txt, f, size)
        d._put(f, size, x, _y(d.top), txt)
        d._put(f, size, MR - wn, _y(d.top), num)
        # linea de puntos
        x0, x1 = x + wt + 4, MR - wn - 4
        if x1 > x0:
            pt = pdfmetrics.stringWidth(".", "EA", size)
            n = int((x1 - x0) / pt)
            if n > 0:
                d._put("EA", size, x0, _y(d.top), "." * n)
        d.top += lead
    d.save()
    return path

# ─────────────────────────────────────────────────────────── HISTORIAL
def build_historial(path, cfg):
    d = Doc(path)
    d.h1("Historial de revisión")
    d.p("El presente historial registra la evolución del documento conforme lo exige la Carpeta "
        "Modelo. Cada fila consigna la fecha, la versión, el autor responsable del cambio y una "
        "descripción de las modificaciones incorporadas.")
    d.table(
        ["Fecha", "Versión", "Autor", "Descripción del cambio"],
        [["05/06/2025", "0.1", "M. Ambrosino", "Versión inicial: descripción del producto, "
          "objetivos, registro de interesados y cronograma de hitos."],
         ["24/06/2025", "0.2", "J. C. Riggio", "Incorporación de criterios de aceptación, "
          "supuestos, restricciones y definición de requerimientos."],
         ["15/07/2025", "0.3", "M. Ambrosino", "Alcance, minutas de reunión simuladas y "
          "prototipos de interfaz gráfica."],
         ["12/08/2025", "0.4", "M. Ambrosino", "Requerimiento core, guion de interfaz de usuario "
          "y sección de implementación."],
         ["09/09/2025", "0.5", "J. C. Riggio", "Especificaciones de casos de uso CU-01 a CU-08 y "
          "modelo de dominio conceptual."],
         ["26/09/2025", "1.0", "M. Ambrosino", "Diagramas de clases, secuencia, estados, "
          "robustez, actividad y DER. Cierre de la primera entrega."],
         ["28/09/2025", "2.0", "M. Ambrosino", "Incorporación de reglas de negocio RN-01 a RN-37, "
          "análisis de factibilidad económica de los descuentos y priorización MoSCoW."],
         ["28/09/2025", "2.1", "M. Ambrosino", "Instructivo del requerimiento core, desarrollo de "
          "la lógica de descuento por nivel y casos de prueba CP-01 a CP-10."],
         ["28/09/2025", "2.2", "J. C. Riggio", "Informe de pruebas de usabilidad y anexo A02 de "
          "reportes e indicadores de gestión."],
         ["28/09/2025", "2.3", "M. Ambrosino", "Matriz de trazabilidad. A partir de su análisis se "
          "incorporan los requerimientos RF-12 y RF-13 y se recalcula la priorización."],
         ["28/09/2025", "2.4", "M. Ambrosino", "Preliminares y cierre: índice, historial de "
          "revisión, resumen ejecutivo, conclusiones, bibliografía, encabezado normalizado y "
          "numeración de páginas."]],
        widths=[58, 44, 66, 283], aligns=["c", "c", "l", "l"], size=8.5)
    d.rich([("Nota sobre las fechas anteriores a la versión 2.0. ", "b"),
            ("Corresponden a las entregas parciales del proyecto y fueron reconstruidas a partir "
             "de las minutas de reunión y del cronograma de hitos, dado que el historial no se "
             "llevó desde el inicio. Las versiones 2.0 en adelante registran los cambios en el "
             "momento en que fueron efectuados.", "r")])
    d.save()
    return path

# ─────────────────────────────────────────────────────────── RESUMEN EJECUTIVO
def build_resumen(path):
    d = Doc(path)
    d.h1("Resumen ejecutivo")

    d.p("EventAura es una plataforma de venta de entradas para espectáculos de Rosario que se "
        "diferencia de las ticketeras tradicionales por un rasgo central: permite al cliente armar "
        "un paquete personalizado que combina la entrada con hospedaje, traslado, merchandising y "
        "experiencias VIP en una única operación de compra. El presente informe documenta su "
        "análisis, diseño y validación.")

    d.h2("El problema y la propuesta")
    d.p("Las ticketeras del mercado operan sobre una lógica acotada al acceso al evento. El "
        "cliente que viaja a un recital debe resolver por separado el alojamiento y el "
        "transporte, y la plataforma no captura ese valor. EventAura integra esos servicios en el "
        "mismo flujo, con tres mecanismos de retención: un sistema de fidelización por niveles, "
        "un esquema de suscripciones y un seguro opcional sobre la entrada.")

    d.h2("Qué contiene este informe")
    d.table(
        ["Bloque", "Contenido"],
        [["Definición", "Objetivos, interesados, cronograma, criterios de aceptación, alcance y "
                        "trece requerimientos funcionales con sus criterios de verificación."],
         ["Reglas de negocio", "Treinta y seis reglas vigentes que fijan las condiciones bajo las "
                               "cuales opera el sistema, con su análisis de factibilidad "
                               "económica y su priorización MoSCoW."],
         ["Diseño", "Requerimiento core con su instructivo de ocho pasos, guion de interfaz, "
                    "especificaciones de ocho casos de uso y modelo de dominio completo."],
         ["Desarrollo", "Lógica de descuento por nivel con su modelo de datos, algoritmos, "
                        "implementación y diez casos de prueba ejecutables."],
         ["Validación", "Informe de pruebas de usabilidad con cinco participantes y anexo A02 con "
                        "cinco reportes de gestión."],
         ["Verificación", "Matriz de trazabilidad que vincula objetivos, requerimientos, reglas, "
                          "casos de uso, pruebas y hallazgos."]],
        widths=[96, 355])

    d.h2("Resultados principales")
    d.bullet([("Viabilidad del esquema de descuentos. ", "b"),
              ("El análisis económico determinó que el punto de equilibrio de una operación se "
               "sitúa en 8,62 % de descuento sobre el valor facial. Los porcentajes se fijaron en "
               "0 %, 5 % y 7,5 % según el nivel, y se dio de baja la regla que permitía "
               "acumularlos con la suscripción por convertir al mejor cliente en deficitario.",
               "r")])
    d.bullet([("Usabilidad por encima del promedio. ", "b"),
              ("La evaluación arrojó un puntaje SUS de 74,5 frente a un valor de referencia de "
               "68, con una tasa de éxito del 80 % y un tiempo medio de 4 min 56 s, dentro del "
               "límite comprometido de cinco minutos.", "r")])
    d.bullet([("Brecha detectada y corregida. ", "b"),
              ("La matriz de trazabilidad reveló que dos casos de uso especificados en detalle "
               "carecían de un requerimiento que los originara. Se incorporaron RF-12 y RF-13 y "
               "se recalculó la priorización.", "r")], gap=True)

    d.h2("Limitaciones declaradas")
    d.p("El informe explicita tres restricciones que condicionan la lectura de sus resultados. "
        "Las sesiones de usabilidad son simuladas y deberán repetirse con participantes reales. "
        "Los reportes del anexo A02 operan sobre un juego de datos de prueba. Y la cobertura de "
        "verificación alcanza al 22,2 % de las reglas de negocio, concentrada en la lógica de "
        "fidelización.")
    d.save()
    return path

# ─────────────────────────────────────────────────────────── CONCLUSIONES
def build_conclusiones(path):
    d = Doc(path)
    d.h1("Conclusiones")

    d.p("El trabajo partió de una hipótesis comercial —que integrar servicios complementarios a la "
        "venta de entradas genera valor suficiente para diferenciar a una ticketera— y la sometió "
        "a un desarrollo completo de análisis y diseño. Las conclusiones que siguen resumen lo "
        "que ese recorrido permitió establecer.")

    d.h2("Sobre el requerimiento core")
    d.p("El armado personalizado de paquetes se confirmó como el eje del sistema, aunque por una "
        "razón distinta de la que se había previsto. El análisis de factibilidad económica mostró "
        "que la contribución de una entrada vendida es estrecha: $1.840 sobre un ticket promedio "
        "de $19.356, apenas el 9,5 % del valor facial. Son los servicios complementarios los que "
        "sostienen el margen del negocio. El paquete no es solamente un diferencial de "
        "experiencia; es la condición para que el esquema de fidelización sea financiable.")

    d.h2("Sobre las reglas de negocio")
    d.p("Explicitar las treinta y seis reglas resultó más productivo de lo esperado. Dos de ellas "
        "no habrían resistido su implementación: la acumulación de descuentos de RN-27, que "
        "arrojaba contribución negativa en todos los escenarios y fue dada de baja, y la ausencia "
        "de ventana temporal en RN-19, que habría convertido el nivel de fidelidad en un beneficio "
        "vitalicio de costo creciente. Ambas se detectaron al calcular, no al redactar.")

    d.h2("Sobre la validación con usuarios")
    d.p("El hallazgo más consistente de las pruebas de usabilidad es que las dificultades no "
        "aparecen al operar el sistema sino al comprender sus decisiones. El resumen del paquete, "
        "el nivel de fidelidad y la vigencia de la reserva son reglas correctamente definidas que "
        "la interfaz no logra comunicar. Ninguna de las seis acciones del plan de mejora introduce "
        "un requerimiento nuevo: todas consisten en explicar mejor algo que el sistema ya hace.")

    d.h2("Sobre la trazabilidad")
    d.p("La matriz cumplió la función que justifica su existencia. Dos casos de uso especificados "
        "en detalle, con seis reglas de negocio asociadas, respondían a funcionalidades que nunca "
        "habían sido declaradas como requerimiento. La omisión permaneció invisible durante toda "
        "la elaboración del informe y sólo se hizo evidente al construir el cruce sistemático.")

    d.h2("Limitaciones del trabajo")
    d.table(
        ["Limitación", "Alcance de su efecto"],
        [["Las sesiones de usabilidad son simuladas.",
          "El objetivo específico 4 se considera cumplido de manera provisoria hasta su "
          "ejecución con participantes reales."],
         ["Los datos de los reportes son ficticios.",
          "Validan el diseño del indicador y el mecanismo de exportación, no el comportamiento "
          "real del mercado."],
         ["La cobertura de pruebas alcanza el 22,2 % de las reglas.",
          "Diez de trece requerimientos carecen de verificación automatizada."],
         ["El modelo económico usa un ticket promedio único.",
          "Determina el techo admisible del descuento, no proyecta el resultado operativo."]],
        widths=[176, 275])

    d.h2("Trabajo futuro")
    d.numbered(1, "Ejecutar las sesiones de usabilidad con participantes reales y contrastar los "
                  "resultados con la línea base establecida en este informe.")
    d.numbered(2, "Extender el conjunto de pruebas al flujo de compra completo, de modo de cerrar "
                  "la brecha G-02 de la matriz de trazabilidad.")
    d.numbered(3, "Desarrollar los anexos técnicos T01 a T08 previstos por la Carpeta Modelo, con "
                  "particular atención a la arquitectura base y a la gestión de perfiles.")
    d.numbered(4, "Definir un caso de prueba de regresión por cada hallazgo de usabilidad antes de "
                  "aplicar las correcciones de la Fase 6.")
    d.numbered(5, "Resolver la duplicación de contenido detectada entre las secciones de "
                  "requerimientos, minutas y prototipos del cuerpo del informe.", gap=True)

    d.h2("Consideración final")
    d.p("El resultado más útil del trabajo no fue confirmar la propuesta inicial sino corregirla "
        "en tres puntos concretos: un descuento que no cerraba, un beneficio vitalicio que no se "
        "había advertido y dos funcionalidades que se habían especificado sin haberlas pedido. "
        "Los tres se detectaron por instrumentos de análisis y no por revisión de la redacción, lo "
        "que confirma el valor de aplicarlos antes de escribir código.")
    d.save()
    return path

# ─────────────────────────────────────────────────────────── BIBLIOGRAFIA
def build_biblio(path):
    d = Doc(path)
    d.h1("Bibliografía")
    d.h2("Fuentes metodológicas")
    for r in [
        "Bangor, A., Kortum, P., & Miller, J. (2009). Determining what individual SUS scores "
        "mean: Adding an adjective rating scale. Journal of Usability Studies, 4(3), 114–123.",
        "Brooke, J. (1996). SUS: A quick and dirty usability scale. En P. W. Jordan, B. Thomas, "
        "B. A. Weerdmeester & I. L. McClelland (Eds.), Usability Evaluation in Industry "
        "(pp. 189–194). Londres: Taylor & Francis.",
        "Molina, J. G., Ortín-Ibáñez, M. J., Moros, B., Nicolás, J., & Álvarez, J. A. T. (2000). "
        "De los procesos del negocio a los casos de uso. Jornadas de Ingeniería del Software y "
        "Bases de Datos (JISBD), 103–116.",
        "Nielsen, J., & Landauer, T. K. (1993). A mathematical model of the finding of usability "
        "problems. Proceedings of the INTERCHI '93 Conference on Human Factors in Computing "
        "Systems, 206–213.",
        "Sauro, J., & Lewis, J. R. (2016). Quantifying the User Experience: Practical Statistics "
        "for User Research (2ª ed.). Cambridge: Morgan Kaufmann.",
        "Universidad Abierta Interamericana. (2020). Carpeta Modelo versión 2020.1. Facultad de "
        "Tecnología Informática.",
    ]:
        d.p(r, justify=False)

    d.h2("Documentación técnica")
    for r in [
        "iText Software. (2025). iText Core 9.2.0 — Documentación de la biblioteca de generación "
        "de PDF para .NET.",
        "Microsoft. (2025). Microsoft.Data.SqlClient 6.0.2 — Documentación del proveedor de "
        "acceso a datos para SQL Server.",
        "QRCoder. (2025). QRCoder 1.6.0 — Biblioteca de generación de códigos QR para .NET.",
    ]:
        d.p(r, justify=False)

    d.h2("Fuentes de los parámetros económicos")
    d.p("Los valores de comisión empleados en el análisis de factibilidad económica de los "
        "descuentos fueron relevados de la información pública de plataformas del mercado "
        "argentino durante septiembre de 2025.")
    for r in [
        "Comparativa de comisiones de ticketeras en Argentina. Consultado en septiembre de 2025 "
        "en tickean.com/comisiones-ticketera",
        "Esquema de comisiones de Mercado Pago por país y plazo de acreditación. Consultado en "
        "septiembre de 2025 en guiadebancos.com",
    ]:
        d.p(r, justify=False)
    d.save()
    return path

if __name__ == "__main__":
    build_historial(O+"_historial.pdf", {})
    build_resumen(O+"_resumen.pdf")
    build_conclusiones(O+"_conclusiones.pdf")
    build_biblio(O+"_biblio.pdf")
    import pdfplumber
    for f in ["_historial","_resumen","_conclusiones","_biblio"]:
        with pdfplumber.open(O+f+".pdf") as pdf: print(f, len(pdf.pages), "pag")
