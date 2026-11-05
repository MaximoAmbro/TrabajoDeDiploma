# -*- coding: utf-8 -*-
"""Punto 15 — Reglas de negocio (RN-01 a RN-36)."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "RN_ReglasNegocio.pdf")
d = Doc(OUT)

W = [36, 286, 28, 80]
AL = ["c", "l", "c", "c"]
HDR = ["ID", "Enunciado de la regla", "T", "Origen"]

d.h1("Reglas de negocio")

d.p("Los requerimientos funcionales enunciados en la sección anterior describen qué debe hacer el "
    "sistema. Las reglas de negocio, en cambio, establecen bajo qué condiciones debe hacerlo: "
    "definen las restricciones, los cálculos y las políticas que el software debe respetar con "
    "independencia de la interfaz o de la tecnología empleada. Separarlas del requerimiento "
    "funcional evita que queden implícitas en el código y permite modificarlas sin rediseñar el "
    "caso de uso que las invoca.")

d.h2("Criterio de identificación y nomenclatura")
d.p("Se relevaron las reglas a partir de tres fuentes: los once requerimientos funcionales "
    "(RF-01 a RF-11), las especificaciones de casos de uso (CU-01 a CU-08) y las minutas de "
    "reunión con el cliente. Cada regla recibe un identificador único con el formato RN-xx, "
    "correlativo y sin reutilización: si una regla se da de baja, su número no se reasigna.")
d.p("Toda regla cumple tres condiciones para ser admitida como tal: es verificable (puede "
    "afirmarse si el sistema la cumple o no), es atómica (no puede descomponerse en dos reglas "
    "independientes) y es independiente de la implementación (no menciona pantallas, controles ni "
    "tablas).")

d.h2("Clasificación adoptada")
d.table(
    ["Tipo", "Denominación", "Qué establece"],
    [["E", "Estructural", "Una restricción que los datos del sistema deben satisfacer siempre. "
                          "Su violación implica un estado inválido."],
     ["D", "Derivación", "Un valor que se calcula a partir de otros. Define la fórmula, no el "
                         "momento del cálculo."],
     ["A", "Acción", "Una condición que habilita o impide ejecutar una operación."]],
    widths=[34, 96, 321], aligns=["c", "l", "l"])

d.keep(200)
d.h2("Gestión de usuarios y cuentas")
d.table(HDR, [
    ["RN-01", "Una dirección de correo electrónico identifica unívocamente a un usuario: no pueden "
              "coexistir dos cuentas con el mismo correo.", "E", "RF-01 / CU-04"],
    ["RN-02", "La contraseña debe tener ocho caracteres como mínimo e incluir al menos una letra y "
              "un dígito.", "E", "RF-01 / CU-04"],
    ["RN-03", "Toda cuenta nueva se crea con nivel de fidelidad Bronce y sin suscripción activa.",
     "A", "RF-01 / CU-04"],
    ["RN-04", "Tras cinco intentos fallidos consecutivos de autenticación, la cuenta queda "
              "bloqueada por quince minutos.", "A", "RF-02 / CU-05"],
    ["RN-05", "Un usuario posee un único perfil, Cliente o Administrador, y ambos son excluyentes "
              "entre sí.", "E", "RF-02 / CU-05"],
], widths=W, aligns=AL)

d.keep(230)
d.h2("Eventos, sectores y disponibilidad")
d.table(HDR, [
    ["RN-06", "Un evento no puede publicarse si no tiene al menos un sector definido con capacidad "
              "mayor que cero.", "A", "RF-12 / CU-02"],
    ["RN-07", "La capacidad total de un evento es la suma de las capacidades de sus sectores.",
     "D", "RF-12 / CU-02"],
    ["RN-08", "La cantidad de entradas vendidas de un sector nunca puede superar su capacidad.",
     "E", "RF-04 / CU-01"],
    ["RN-09", "Un evento cuya fecha de realización ya transcurrió no admite nuevas ventas ni "
              "inscripciones en lista de espera.", "A", "RF-03 / CU-01"],
    ["RN-10", "Un evento se considera agotado cuando todos sus sectores alcanzaron su capacidad; "
              "en ese estado se habilita la lista de espera.", "D", "RF-10 / CU-07"],
    ["RN-11", "Un evento con entradas vendidas no puede eliminarse: sólo puede suspenderse o "
              "reprogramarse, conservando la trazabilidad de lo vendido.", "A", "RF-12 / CU-02"],
], widths=W, aligns=AL)

d.keep(250)
d.h2("Compra y armado de paquetes")
d.table(HDR, [
    ["RN-12", "Una misma operación de compra admite un máximo de seis entradas por evento.",
     "A", "RF-04 / CU-01"],
    ["RN-13", "Un paquete debe contener al menos una entrada; los servicios complementarios no "
              "pueden adquirirse de forma aislada.", "E", "RF-05 / CU-01"],
    ["RN-14", "Un servicio complementario sólo puede incorporarse a un paquete si el evento lo "
              "tiene habilitado.", "A", "RF-05 / CU-01"],
    ["RN-15", "Al iniciar el proceso de pago, las entradas seleccionadas quedan reservadas por "
              "diez minutos; vencido ese plazo sin confirmación, el cupo se libera.",
     "A", "RF-06 / CU-01"],
    ["RN-16", "El precio se congela al iniciar la compra: una modificación posterior de tarifa no "
              "afecta a la operación en curso.", "A", "RF-06 / CU-01"],
    ["RN-17", "Una compra sólo se considera confirmada si la pasarela de pago devuelve estado "
              "aprobado.", "A", "RF-07 / CU-01"],
    ["RN-18", "Cada entrada vendida genera un código QR único e irrepetible, válido para un único "
              "ingreso.", "E", "RF-07 / CU-01"],
], widths=W, aligns=AL)

d.keep(240)
d.h2("Fidelización y descuentos por nivel")
d.table(HDR, [
    ["RN-19", "El nivel de fidelidad se determina por la cantidad de compras confirmadas dentro "
              "de una ventana móvil de los últimos doce meses: Bronce de 0 a 2, Plata de 3 a 6, "
              "Oro de 7 en adelante.", "D", "RF-08 / CU-01"],
    ["RN-20", "El descuento por nivel es de 0 % para Bronce, 5 % para Plata y 7,5 % para Oro.",
     "D", "RF-08 / CU-01"],
    ["RN-21", "El descuento por nivel se aplica exclusivamente sobre el valor de las entradas; no "
              "alcanza a los servicios complementarios ni al cargo de servicio.",
     "D", "RF-08 / CU-01"],
    ["RN-22", "El nivel se recalcula automáticamente al confirmarse cada compra y rige a partir de "
              "la operación siguiente.", "A", "RF-08 / CU-01"],
    ["RN-23", "El nivel se reevalúa el primer día de cada mes. Entre dos reevaluaciones "
              "consecutivas el nivel permanece estable, aunque la ventana móvil de RN-19 haya "
              "dejado de cubrir alguna compra.", "A", "RF-08"],
    ["RN-24", "Sólo computan para el nivel las compras confirmadas y no reembolsadas.",
     "D", "RF-08 / CU-03"],
], widths=W, aligns=AL)

d.keep(220)
d.h2("Suscripciones y seguros de entrada")
d.table(HDR, [
    ["RN-25", "Un cliente no puede mantener dos suscripciones activas en forma simultánea.",
     "E", "RF-13 / CU-06"],
    ["RN-26", "La suscripción no se renueva de forma automática: vencido el período, el cliente "
              "debe contratarla nuevamente.", "A", "RF-13 / CU-06"],
    ["RN-27", "DADA DE BAJA. Establecía la acumulación del descuento por suscripción con el "
              "descuento por nivel hasta un tope conjunto del 20 %. Se retira por inviabilidad "
              "económica; el análisis consta en la sección Factibilidad económica de los "
              "descuentos. Su número no se reasigna.", "—", "CU-06 / RF-08"],
    ["RN-28", "El seguro de entrada sólo puede contratarse durante el proceso de compra; no admite "
              "incorporación posterior.", "A", "RF-05 / CU-01"],
    ["RN-29", "El seguro habilita la devolución del valor de la entrada hasta cuarenta y ocho "
              "horas antes del inicio del evento.", "A", "RF-05"],
    ["RN-37", "La suscripción no otorga descuento porcentual sobre el valor de la entrada. Sus "
              "beneficios son en especie —seguro bonificado, preventa anticipada y prioridad en "
              "lista de espera— y no se acumulan con el descuento por nivel.",
     "A", "RF-13 / CU-06"],
], widths=W, aligns=AL)

d.keep(190)
d.h2("Pagos")
d.table(HDR, [
    ["RN-30", "El sistema no almacena el número completo de la tarjeta: conserva únicamente los "
              "cuatro últimos dígitos y el token devuelto por la pasarela.",
     "E", "RNF-04 / CU-01"],
    ["RN-31", "Toda operación de pago se registra con su estado resultante (aprobado, rechazado o "
              "pendiente), sea cual fuere el desenlace.", "E", "RF-06 / CU-01"],
    ["RN-32", "Un pago rechazado libera de inmediato el cupo reservado por RN-15.",
     "A", "RF-06 / CU-01"],
    ["RN-33", "Todos los importes del sistema se expresan en pesos argentinos.", "E", "RF-04"],
], widths=W, aligns=AL)

d.keep(170)
d.h2("Reportes y listas de espera")
d.table(HDR, [
    ["RN-34", "Los reportes de gestión son accesibles únicamente al perfil Administrador.",
     "A", "RF-11 / CU-08"],
    ["RN-35", "La lista de espera respeta estrictamente el orden de inscripción.",
     "E", "RF-10 / CU-07"],
    ["RN-36", "Un cliente puede inscribirse una sola vez en la lista de espera de un mismo evento.",
     "E", "RF-10 / CU-07"],
], widths=W, aligns=AL)

# ============================================================ FACTIBILIDAD ECONOMICA
d.h1("Factibilidad económica de los descuentos")
d.p("Los porcentajes de descuento fijados en RN-20 no se establecieron por criterio comercial. Se "
    "determinaron verificando que cada transacción conserve una contribución positiva para la "
    "plataforma. Esta sección presenta el modelo empleado, sus parámetros y los resultados que "
    "fundamentan las reglas RN-19, RN-20, RN-21 y la baja de RN-27.")

d.keep(230)
d.h2("Modelo de contribución unitaria")
d.p("Una ticketera no revende entradas: intermedia. El cliente abona el valor facial de la entrada "
    "más un cargo por servicio; la plataforma remite al organizador el valor facial completo y "
    "retiene el cargo por servicio, del que debe descontar la comisión de la pasarela de pago y el "
    "costo de emisión del comprobante. El descuento por fidelidad es un beneficio comercial de "
    "EventAura, no del organizador, de modo que lo absorbe íntegramente la plataforma.")
d.table(
    ["Parámetro", "Símb.", "Valor", "Origen del valor"],
    [["Precio promedio de la entrada", "P", "$19.356", "Ticket promedio del período (RPT-01)"],
     ["Cargo por servicio", "c", "15 %", "Práctica corriente del mercado argentino"],
     ["Comisión de la pasarela, con IVA", "g", "4,10 %", "Acreditación a 14 días (3,39 % + IVA)"],
     ["Costo de emisión del comprobante", "q", "$150", "Rango de mercado de $120 a $200"]],
    widths=[168, 38, 62, 183], aligns=["l", "c", "c", "l"])
d.p("La contribución unitaria de una entrada vendida con un descuento d resulta entonces:")
d.pre(["        Contribución  =  P · (1 − d) · (1 + c) · (1 − g)  −  P  −  q"])
d.p("Evaluando la expresión para los distintos niveles de descuento se obtiene:")
d.table(
    ["Descuento aplicado", "Contribución por entrada", "Veredicto"],
    [["0 % — nivel Bronce", "$1.840", "Contribución plena"],
     ["5 % — nivel Plata", "$773", "Viable"],
     ["7,5 % — nivel Oro", "$239", "Viable, con margen estrecho"],
     ["8,62 %", "$0", "Punto de equilibrio"],
     ["10 %", "−$294", "No viable"],
     ["12,5 %", "−$828", "No viable"],
     ["17,5 %", "−$1.895", "No viable"],
     ["20 %", "−$2.429", "No viable"]],
    widths=[148, 148, 155], aligns=["l", "c", "l"])

d.keep(150)
d.h2("Descuento máximo admisible")
d.p("El criterio adoptado es que la transacción mínima —una entrada sola, sin servicios "
    "complementarios— conserve contribución no negativa. Si el descuento se fijara por encima de "
    "ese umbral, todo cliente que comprase únicamente su entrada representaría una pérdida, y el "
    "esquema quedaría dependiendo de que el cliente contrate además otros servicios.")
d.p("Despejando d de la expresión anterior, el punto de equilibrio se ubica en 8,62 %. De allí se "
    "desprenden dos decisiones: el 10 % originalmente previsto para el nivel Oro se descarta por "
    "situarse por encima del equilibrio, y el valor adoptado se fija en 7,5 %, que deja un margen "
    "de seguridad de 1,1 puntos porcentuales frente a variaciones de la comisión de la pasarela o "
    "del ticket promedio.")

d.keep(200)
d.h2("Contribución anual por cliente")
d.p("El análisis por transacción es condición necesaria pero no suficiente: lo que sostiene el "
    "programa es la contribución anual del cliente, que incorpora la frecuencia de compra y el "
    "margen de los servicios complementarios. Se aplica una tasa de intermediación del 20 % sobre "
    "los servicios, no alcanzada por el descuento en virtud de RN-21.")
d.table(
    ["Nivel", "Entradas/año", "Desc.", "Contrib. entradas", "Contrib. servicios", "Total anual"],
    [["Bronce", "1,6", "0 %", "$2.945", "$1.043", "$3.988"],
     ["Plata", "6,7", "5 %", "$5.195", "$7.570", "$12.764"],
     ["Oro", "14,2", "7,5 %", "$3.409", "$24.483", "$27.892"]],
    widths=[50, 68, 44, 92, 96, 82], aligns=["l", "c", "c", "c", "c", "c"])
d.rich([("Hallazgo relevante. ", "b"),
        ("Un cliente Oro aporta siete veces la contribución de un cliente Bronce, pero su línea de "
         "entradas apenas alcanza los $3.409 anuales. Son los servicios complementarios los que "
         "sostienen el esquema. Esto confirma la necesidad de RN-21: si el descuento se extendiera "
         "a los servicios, desaparecería la fuente que lo financia y el segmento más valioso se "
         "volvería deficitario.", "r")])

d.keep(160)
d.h2("Costo del programa y frecuencia requerida")
d.p("El costo anual de los descuentos, calculado sobre el juego de datos de prueba, asciende a "
    "$3.758.161. La contribución media de una compra sin descuento es de $4.827, de modo que el "
    "programa necesita generar 779 operaciones incrementales para autofinanciarse, equivalentes a "
    "un aumento del 27,8 % en la frecuencia de compra de la base.")
d.p("Ese umbral resulta alcanzable: los estudios sobre programas de fidelización por niveles "
    "reportan que los clientes de los escalones superiores compran varias veces más que los no "
    "adheridos. El programa se considera, por lo tanto, económicamente sostenible.")

d.keep(230)
d.h2("Ventana de acumulación")
d.p("Sin una ventana temporal, el nivel alcanzado sería vitalicio: la base de clientes con "
    "descuento crecería de forma indefinida y el costo del programa nunca se estabilizaría. Se "
    "evaluaron cinco combinaciones de ventana y umbral.")
d.table(
    ["Ventana", "Umbral", "Base en Oro", "Costo anual", "Alza requerida", "Veredicto"],
    [["6 meses", "7", "2,0 %", "$0,9 M", "+6,7 %", "Nivel casi inalcanzable; no motiva"],
     ["12 meses", "5", "15,0 %", "$5,3 M", "+39,3 %", "Nivel poco selectivo; costo alto"],
     ["12 meses", "7", "8,5 %", "$3,8 M", "+27,8 %", "Adoptada"],
     ["12 meses", "9", "5,0 %", "$2,8 M", "+20,7 %", "Viable, pero desalienta el ascenso"],
     ["24 meses", "7", "22,0 %", "$6,6 M", "+49,0 %", "Costo no defendible"]],
    widths=[54, 38, 58, 60, 68, 173], aligns=["c", "c", "c", "c", "c", "l"])
d.p("Se adopta la ventana móvil de doce meses con umbral de siete compras confirmadas, conforme "
    "queda enunciado en RN-19. Las filas distintas de la adoptada son estimaciones obtenidas "
    "reescalando la distribución de frecuencia del mismo juego de datos simulado, y se presentan "
    "a título comparativo.")

d.keep(210)
d.h2("RN-27 — Evaluación y baja")
d.p("La regla RN-27 admitía acumular el descuento por nivel con un descuento por suscripción, "
    "hasta un tope conjunto del 20 %. Sometida al modelo, arroja los siguientes resultados para un "
    "cliente del nivel Oro:")
d.table(
    ["Escenario", "Desc. total", "Contrib. por entrada", "Contrib. anual del cliente"],
    [["Oro sin suscripción", "7,5 %", "$239", "$27.892"],
     ["Oro con suscripción, acumulado", "17,5 %", "−$1.895", "−$2.505"],
     ["Oro con suscripción, en el tope", "20 %", "−$2.429", "−$10.105"]],
    widths=[160, 66, 106, 119], aligns=["l", "c", "c", "c"])
d.rich([("Conclusión. ", "b"),
        ("La acumulación convierte al cliente de mayor valor del sistema en un cliente deficitario, "
         "incluso computando el margen de los servicios complementarios. No existe configuración "
         "del tope que revierta el resultado, porque el propio descuento por nivel ya se sitúa "
         "cerca del punto de equilibrio. En consecuencia, RN-27 se da de baja. Conforme al criterio "
         "de nomenclatura adoptado, su número queda retirado y no se reasigna.", "r")])
d.p("En su reemplazo se incorpora RN-37, que redefine el beneficio de la suscripción en términos "
    "no porcentuales: seguro de entrada bonificado, preventa anticipada y prioridad en lista de "
    "espera. Estas prestaciones tienen costo marginal acotado y conocido, no afectan la "
    "contribución unitaria de la entrada y permiten que el ingreso por suscripciones —$6.066.000 "
    "en el período analizado, según RPT-04— se conserve como margen íntegro.")

d.keep(140)
d.h2("Limitaciones del análisis")
d.p("El modelo opera con un ticket promedio único y no contempla la dispersión de precios entre "
    "categorías de evento; supone que el organizador percibe siempre el valor facial completo, sin "
    "participación en el costo del descuento; y omite los costos fijos de la plataforma. Su "
    "propósito es determinar el techo admisible del descuento y validar la estructura de niveles, "
    "no proyectar el resultado operativo del negocio.")

d.h1("Especificación ampliada de reglas críticas")
d.p("Las reglas que involucran un cálculo o un parámetro configurable se detallan a continuación, "
    "de modo que su implementación no admita interpretaciones divergentes.")

d.keep(210)
d.h2("RN-19 y RN-20 — Determinación del nivel y del descuento")
d.p("La asignación de nivel depende de un único contador: la cantidad de compras confirmadas y no "
    "reembolsadas del cliente (RN-24). No intervienen el monto gastado ni la antigüedad de la "
    "cuenta.")
d.table(
    ["Nivel", "Compras confirmadas", "Descuento sobre entradas", "Beneficios adicionales"],
    [["Bronce", "0 a 2", "0 %", "Acceso estándar a la preventa general"],
     ["Plata", "3 a 6", "5 %", "Preventa anticipada de 24 horas"],
     ["Oro", "7 o más", "7,5 %", "Preventa anticipada de 48 h y prioridad en lista de espera"]],
    widths=[52, 90, 100, 209], aligns=["l", "c", "c", "l"])
d.rich([("Fundamento de los porcentajes. ", "b"),
        ("Los tres valores no se fijaron por criterio comercial sino verificando que cada "
         "transacción conserve contribución positiva. El punto de equilibrio de la operación se "
         "sitúa en 8,62 % de descuento sobre el valor facial, razón por la cual el escalón "
         "superior se fija en 7,5 % y no en 10 %. El desarrollo completo del cálculo consta en la "
         "sección Factibilidad económica de los descuentos. Los beneficios diferenciales del nivel "
         "Oro se orientan al acceso anticipado —de costo marginal nulo para la plataforma— antes "
         "que a una rebaja mayor de precio.", "r")])

d.keep(180)
d.h2("RN-21 y RN-37 — Base de cálculo del descuento")
d.p("Existe un único descuento porcentual en el sistema, el que corresponde al nivel de fidelidad. "
    "La suscripción no aporta un segundo porcentaje (RN-37). El cálculo se resuelve así:")
d.numbered(1, "Se determina la base de cálculo: suma del precio de las entradas del paquete. Los "
              "servicios complementarios quedan excluidos de la base (RN-21).")
d.numbered(2, "Se aplica el descuento por nivel vigente al momento de iniciar la compra (RN-20).")
d.numbered(3, "Se adicionan, sin descuento, los servicios complementarios y el cargo de servicio.")
d.numbered(4, "Se aplican los beneficios en especie de la suscripción, si el cliente la posee: "
              "bonificación del seguro de entrada y habilitación de preventa.", gap=True)
d.rich([("Por qué los servicios quedan fuera de la base. ", "b"),
        ("El margen de intermediación sobre los servicios complementarios es lo que financia el "
         "descuento otorgado sobre la entrada. Extender el descuento a los servicios eliminaría la "
         "fuente que lo sostiene y volvería deficitario al segmento Oro. La demostración numérica "
         "se encuentra en la sección Factibilidad económica de los descuentos.", "r")])

d.keep(170)
d.h2("RN-15 — Reserva temporal del cupo")
d.p("Entre la selección de entradas y la confirmación del pago, el cupo queda en estado reservado. "
    "Esto impide la sobreventa sin bloquear el inventario de forma indefinida.")
d.table(
    ["Situación", "Efecto sobre el cupo"],
    [["El cliente confirma el pago dentro de los 10 minutos",
      "El cupo pasa de reservado a vendido."],
     ["La pasarela rechaza el pago", "El cupo se libera de inmediato (RN-32)."],
     ["Vencen los 10 minutos sin resolución", "El cupo se libera y la operación se descarta."],
     ["El cliente abandona el proceso", "El cupo se libera al vencer el plazo."]],
    widths=[200, 251])

d.keep(230)
d.h2("Parámetros configurables")
d.p("Los valores numéricos citados en las reglas no se escriben en el código fuente: se almacenan "
    "como parámetros del sistema, de modo que un cambio de política comercial no requiera "
    "recompilar la aplicación.")
d.table(
    ["Parámetro", "Regla", "Valor inicial"],
    [["Longitud mínima de contraseña", "RN-02", "8 caracteres"],
     ["Intentos fallidos antes del bloqueo", "RN-04", "5"],
     ["Duración del bloqueo de cuenta", "RN-04", "15 minutos"],
     ["Entradas máximas por operación", "RN-12", "6"],
     ["Vigencia de la reserva de cupo", "RN-15", "10 minutos"],
     ["Ventana móvil de acumulación de compras", "RN-19", "12 meses"],
     ["Umbral de nivel Plata", "RN-19", "3 compras"],
     ["Umbral de nivel Oro", "RN-19", "7 compras"],
     ["Descuento nivel Plata", "RN-20", "5 %"],
     ["Descuento nivel Oro", "RN-20", "7,5 %"],
     ["Descuento máximo admisible del sistema", "RN-20", "8,6 %"],
     ["Periodicidad de reevaluación de nivel", "RN-23", "Mensual"],
     ["Plazo de cobertura del seguro", "RN-29", "48 horas"]],
    widths=[222, 78, 151], aligns=["l", "c", "c"])

d.keep(190)
d.h2("Conflictos potenciales y su resolución")
d.p("El relevamiento detectó tres pares de reglas que podrían entrar en tensión. Se documenta el "
    "criterio de precedencia adoptado para cada caso.")
d.table(
    ["Reglas en tensión", "Situación", "Criterio adoptado"],
    [["RN-08 vs. RN-15",
      "Un cupo reservado todavía no está vendido, pero tampoco está disponible.",
      "El control de capacidad de RN-08 computa las entradas vendidas más las reservadas "
      "vigentes."],
     ["RN-22 vs. RN-16",
      "Un cliente asciende de nivel con la compra que está realizando.",
      "Rige el nivel vigente al iniciar la operación. El nuevo nivel aplica desde la compra "
      "siguiente."],
     ["RN-23 vs. RN-19",
      "La ventana móvil deja fuera una compra antigua y el cliente cae de umbral en mitad del mes.",
      "El descenso se hace efectivo recién en la reevaluación mensual, nunca en medio de una "
      "compra en curso."]],
    widths=[84, 166, 201])

d.keep(85)
d.h2("Trazabilidad")
d.p("La columna Origen de cada tabla vincula la regla con el requerimiento funcional y el caso de "
    "uso que la invocan. Ese vínculo se consolida, junto con los objetivos del proyecto y los "
    "casos de prueba, en la matriz de trazabilidad incluida como anexo al final del presente "
    "informe.")

d.save()
print("OK ->", OUT)
