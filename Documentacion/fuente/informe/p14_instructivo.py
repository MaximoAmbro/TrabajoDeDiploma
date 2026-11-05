# -*- coding: utf-8 -*-
"""Punto 14 — Instructivo del requerimiento core (armado de paquetes)."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "Instructivo_Core.pdf")
d = Doc(OUT)

d.h1("Instructivo del requerimiento core")

d.p("La sección Requerimiento core fundamenta por qué el armado personalizado de paquetes "
    "constituye el núcleo conceptual de EventAura. El presente instructivo cubre la otra mitad: "
    "cómo se opera esa funcionalidad, paso por paso, con sus validaciones, sus mensajes y un "
    "ejemplo resuelto de extremo a extremo.")
d.p("Complementa además el Guion de Interfaz de Usuario. Aquel documenta el Menú Inicial, el Menú "
    "Agregar y el Menú de Usuario; la pantalla de armado de paquete —la más relevante del "
    "sistema— no estaba descripta hasta aquí y queda especificada en este instructivo.")

d.rich([("Estado de implementación. ", "b"),
        ("A la fecha de esta versión se encuentran construidas la búsqueda y el listado de "
         "eventos, la selección de sector y cantidad, el pago simulado y la emisión del "
         "comprobante con código QR. La incorporación de servicios complementarios, la "
         "contratación del seguro y el desglose del descuento por nivel están especificados en "
         "este instructivo pero aún no implementados; corresponden a la segunda iteración prevista "
         "en la Fase 6 del cronograma. El instructivo describe, en consecuencia, el comportamiento "
         "definido para el sistema, y no únicamente lo ya construido.", "r")])

d.h2("Destinatario")
d.p("El instructivo está dirigido a dos perfiles. Al cliente final, como guía de uso de la "
    "funcionalidad principal. Y al equipo de soporte técnico, identificado como interesado 7 del "
    "proyecto, que necesita conocer el comportamiento esperado para poder distinguir un error del "
    "sistema de un uso incorrecto.")

# ==================================================================== PRECONDICIONES
d.keep(170)
d.h2("Precondiciones")
d.table(
    ["Condición", "Qué ocurre si no se cumple", "Regla"],
    [["El cliente se encuentra autenticado.",
      "El sistema redirige al Menú Inicial antes de permitir la compra.", "RN-05"],
     ["El evento tiene fecha posterior a la actual.",
      "El evento no aparece en el listado de resultados.", "RN-09"],
     ["El evento posee al menos un sector con capacidad disponible.",
      "El evento se muestra como agotado y se ofrece la lista de espera.", "RN-06, RN-10"],
     ["El evento tiene servicios complementarios habilitados.",
      "La sección de servicios aparece deshabilitada; sólo puede comprarse la entrada.",
      "RN-14"]],
    widths=[166, 226, 59])

# ==================================================================== COMPOSICION
d.keep(200)
d.h2("Composición de un paquete")
d.p("Un paquete es la unidad que el cliente compra. Se compone de un elemento obligatorio y "
    "hasta cinco opcionales.")
d.table(
    ["Componente", "Obligatorio", "Cantidad admitida", "Regla"],
    [["Entrada", "Sí", "De 1 a 6 por operación", "RN-12, RN-13"],
     ["Hospedaje", "No", "0 o 1", "RN-14"],
     ["Traslado", "No", "0 o 1", "RN-14"],
     ["Merchandising", "No", "De 0 a 3 artículos", "RN-14"],
     ["Experiencia VIP", "No", "0 o 1; exige entrada de sector VIP", "RN-14"],
     ["Seguro de entrada", "No", "1 por entrada adquirida", "RN-28, RN-29"]],
    widths=[104, 72, 202, 73], aligns=["l", "c", "l", "c"])
d.rich([("Regla que conviene retener. ", "b"),
        ("Los servicios complementarios no pueden adquirirse por separado: todo paquete debe "
         "contener al menos una entrada (RN-13). EventAura no opera como agencia de viajes sino "
         "como ticketera que amplía la experiencia alrededor del evento.", "r")])

# ==================================================================== PANTALLA
d.keep(290)
d.h2("Pantalla de armado de paquete")
d.p("La pantalla mantiene la línea minimalista definida en el Guion de Interfaz de Usuario: "
    "campos espaciados, etiquetas comprensibles y ausencia de elementos distractivos. Se organiza "
    "en tres bloques verticales —entradas, servicios y resumen— de modo que el cliente perciba el "
    "avance de la construcción de su paquete sin cambiar de ventana.")
d.table(
    ["Control", "Tipo", "Contenido y comportamiento"],
    [["lblEvento", "Etiqueta", "Nombre, fecha y local del evento. Sólo lectura."],
     ["lblSectorA / B / C", "Etiqueta", "Denominación de cada sector disponible. Sólo lectura."],
     ["lblPrecioA / B / C", "Etiqueta",
      "Precio unitario del sector. Queda congelado al iniciar la operación (RN-16)."],
     ["NumPrimero / NumSegundo / NumTercero", "Selector numérico",
      "Cantidad de entradas por sector. Mínimo 0. La suma no puede superar 6 (RN-12) ni el cupo "
      "disponible del sector (RN-08)."],
     ["chkHospedaje / chkTraslado / chkMerch / chkVIP", "Casilla",
      "Servicios complementarios. Aparecen deshabilitados si el evento no los tiene habilitados "
      "(RN-14)."],
     ["chkSeguro", "Casilla",
      "Seguro de entrada. Se habilita sólo cuando hay al menos una entrada seleccionada (RN-28)."],
     ["lblNivel", "Etiqueta",
      "Nivel de fidelidad vigente y porcentaje aplicable. Muestra «Bronce — sin descuento» cuando "
      "corresponde."],
     ["pnlResumen", "Panel",
      "Desglose del paquete. Se recalcula automáticamente ante cada cambio."],
     ["btnContinuar", "Botón",
      "Avanza al pago. Permanece deshabilitado mientras no haya al menos una entrada (RN-13)."],
     ["btnVolver", "Botón",
      "Regresa al listado de eventos. Descarta la selección sin reservar cupo."]],
    widths=[118, 66, 267])

# ==================================================================== PASO A PASO
d.keep(300)
d.h1("Recorrido paso a paso")
d.p("El recorrido completo consta de ocho pasos. Para cada uno se indica la acción que ejecuta el "
    "cliente, la respuesta del sistema y las reglas de negocio que intervienen.")
d.table(
    ["Nº", "Acción del cliente", "Respuesta del sistema", "Reglas"],
    [["1", "Ingresa un término en el buscador o filtra por categoría, fecha o ubicación.",
      "Lista los eventos vigentes que coinciden. Los eventos cuya fecha ya transcurrió no se "
      "muestran.", "RF-03, RN-09"],
     ["2", "Selecciona un evento del listado.",
      "Abre la pantalla de armado y presenta los sectores con precio y disponibilidad.",
      "RN-06, RN-07"],
     ["3", "Indica la cantidad de entradas por sector.",
      "Verifica el cupo y el máximo de seis entradas. Calcula el subtotal de entradas.",
      "RN-08, RN-12"],
     ["4", "Marca los servicios complementarios que desea incorporar.",
      "Ofrece únicamente los servicios habilitados para ese evento. Impide continuar si no hay "
      "al menos una entrada.", "RN-13, RN-14"],
     ["5", "Decide si contrata el seguro de entrada.",
      "Lo ofrece una sola vez y advierte que no podrá incorporarse con posterioridad.",
      "RN-28, RN-29"],
     ["6", "Revisa el resumen del paquete.",
      "Muestra base de entradas, descuento por nivel, servicios sin descuento, cargo por "
      "servicio y total.", "RN-20, RN-21, RN-37"],
     ["7", "Ingresa los datos ficticios de tarjeta y confirma.",
      "Reserva el cupo por diez minutos, tokeniza los datos y envía la operación a la pasarela "
      "simulada.", "RN-15, RN-16, RN-30"],
     ["8", "Recibe la confirmación.",
      "Registra la compra, emite un código QR único por entrada, genera el comprobante en PDF y "
      "actualiza el contador de compras.", "RN-17, RN-18, RN-22"]],
    widths=[20, 130, 220, 81], aligns=["c", "l", "l", "c"])

d.figure_slot(height=168, caption="Figura 1 — Listado de eventos disponibles (pasos 1 y 2)")
d.figure_slot(height=168, caption="Figura 2 — Pantalla de armado de paquete (pasos 3 a 5)")
d.figure_slot(height=168, caption="Figura 3 — Resumen del paquete con el descuento aplicado (paso 6)")
d.figure_slot(height=168, caption="Figura 4 — Confirmación de compra y comprobante emitido (pasos 7 y 8)")

# ==================================================================== EJEMPLO
d.keep(280)
d.h1("Ejemplo completo resuelto")
d.p("Se reproduce una operación íntegra con los valores del juego de datos de prueba. El caso "
    "coincide con el utilizado en la sección Desarrollo de la lógica de descuento por nivel, de "
    "modo que ambas secciones puedan contrastarse entre sí.")
d.table(
    ["Dato de partida", "Valor"],
    [["Cliente", "Nivel Plata (4 compras confirmadas en los últimos doce meses)"],
     ["Evento", "Duki en Rosario"],
     ["Entradas", "2 unidades del sector Campo, a $25.000 cada una"],
     ["Servicio complementario", "Hospedaje por una noche, $45.000"],
     ["Seguro", "No contratado"]],
    widths=[156, 295])
d.p("Al completar el paso 6, el panel de resumen presenta el siguiente desglose:")
d.table(
    ["Concepto", "Detalle", "Importe"],
    [["Base de entradas", "2 × $25.000", "$50.000"],
     ["Descuento por nivel", "Plata, 5 % sobre la base", "−$2.500"],
     ["Subtotal de entradas", "", "$47.500"],
     ["Hospedaje", "Sin descuento (RN-21)", "$45.000"],
     ["Cargo por servicio", "15 % sobre el subtotal de entradas", "$7.125"],
     ["TOTAL A PAGAR", "", "$99.625"]],
    widths=[132, 219, 100], aligns=["l", "l", "c"])
d.rich([("Por qué el hospedaje no recibe descuento. ", "b"),
        ("El descuento por nivel se aplica únicamente sobre el valor de las entradas (RN-21). Si "
         "se extendiera a los servicios complementarios, el margen de intermediación que financia "
         "el propio descuento desaparecería y el esquema dejaría de ser sostenible. El "
         "fundamento numérico se desarrolla en la sección Factibilidad económica de los "
         "descuentos.", "r")])

# ==================================================================== ERRORES
d.keep(260)
d.h1("Mensajes de validación")
d.p("El sistema impide que el cliente construya un paquete inválido. Cada bloqueo se acompaña de "
    "un mensaje que indica la causa y la acción necesaria para resolverla.")
d.table(
    ["Situación", "Mensaje mostrado", "Qué debe hacer el cliente", "Regla"],
    [["Se supera el cupo del sector",
      "«El sector seleccionado no dispone de esa cantidad de entradas.»",
      "Reducir la cantidad o elegir otro sector.", "RN-08"],
     ["Se superan las seis entradas",
      "«No es posible adquirir más de seis entradas por operación.»",
      "Reducir la cantidad solicitada.", "RN-12"],
     ["Intenta continuar sin entradas",
      "«El paquete debe incluir al menos una entrada.»",
      "Seleccionar al menos una entrada.", "RN-13"],
     ["Servicio no habilitado",
      "«Este evento no ofrece el servicio seleccionado.»",
      "Elegir otro servicio o continuar sin él.", "RN-14"],
     ["Vence la reserva de cupo",
      "«La reserva expiró. Debe reiniciar la compra.»",
      "Volver a armar el paquete.", "RN-15"],
     ["La pasarela rechaza el pago",
      "«El pago fue rechazado. Puede reintentar o cancelar la operación.»",
      "Reintentar con otros datos o cancelar.", "RN-32"],
     ["El evento está agotado",
      "«Este evento se encuentra agotado. ¿Desea sumarse a la lista de espera?»",
      "Inscribirse en la lista o volver al listado.", "RN-10"],
     ["Ya figura en la lista de espera",
      "«Usted ya figura en la lista de espera de este evento.»",
      "Regresar al listado de eventos.", "RN-36"]],
    widths=[104, 158, 132, 57])

# ==================================================================== FAQ
d.keep(240)
d.h2("Situaciones particulares")
d.p("Se documentan las consultas que, por experiencia del rubro, concentran la mayor parte de los "
    "pedidos de soporte. Cada respuesta remite a la regla que la sustenta.")
d.table(
    ["Consulta del cliente", "Respuesta", "Regla"],
    [["Quiero agregar el seguro después de haber comprado.",
      "No es posible. El seguro se contrata exclusivamente durante la compra, y así se advierte "
      "en el paso 5.", "RN-28"],
     ["Compré y no subí de nivel.",
      "El ascenso rige a partir de la operación siguiente. La compra que provoca el ascenso se "
      "liquida con el nivel anterior.", "RN-22"],
     ["Bajé de nivel de un día para el otro.",
      "El nivel se reevalúa el primer día de cada mes. Una compra que sale de la ventana de doce "
      "meses recién impacta en esa fecha.", "RN-19, RN-23"],
     ["Me devolvieron una compra, ¿pierdo el nivel?",
      "El contador desciende de inmediato, pero el nivel se conserva hasta la reevaluación "
      "mensual.", "RN-24, RN-23"],
     ["Tengo suscripción, ¿no me corresponde un descuento mayor?",
      "La suscripción no otorga descuento porcentual. Sus beneficios son el seguro bonificado, la "
      "preventa anticipada y la prioridad en lista de espera.", "RN-37"],
     ["Dejé la compra por la mitad y perdí las entradas.",
      "La reserva de cupo dura diez minutos. Vencido ese plazo las entradas vuelven a estar "
      "disponibles para otros clientes.", "RN-15"]],
    widths=[142, 250, 59])

# ==================================================================== TIEMPO
d.keep(220)
d.h2("Tiempo estimado de ejecución")
d.p("El requerimiento no funcional RNF-01 establece que el flujo principal debe completarse en "
    "menos de cinco minutos por un usuario sin experiencia técnica. El desglose por paso permite "
    "verificar ese compromiso y sirve como línea base para las pruebas de usabilidad.")
d.table(
    ["Nº", "Paso", "Tiempo estimado"],
    [["1", "Buscar el evento", "45 s"],
     ["2", "Seleccionar el evento", "15 s"],
     ["3", "Elegir sector y cantidad de entradas", "40 s"],
     ["4", "Agregar servicios complementarios", "60 s"],
     ["5", "Decidir la contratación del seguro", "20 s"],
     ["6", "Revisar el resumen del paquete", "30 s"],
     ["7", "Completar el pago simulado", "60 s"],
     ["8", "Confirmar y recibir el comprobante", "10 s"],
     ["", "TOTAL ESTIMADO", "4 min 40 s"]],
    widths=[28, 331, 92], aligns=["c", "l", "c"])
d.p("El total se ubica veinte segundos por debajo del límite comprometido. El margen es estrecho, "
    "de modo que los pasos 4 y 7 —los más extensos— constituyen los puntos a observar con mayor "
    "atención durante las pruebas con usuarios.")

d.h2("Relación con otras secciones")
d.table(
    ["Sección", "Qué aporta a este instructivo"],
    [["Requerimiento core", "Fundamenta por qué esta funcionalidad es el núcleo del sistema."],
     ["Reglas de negocio", "Provee las restricciones que el instructivo traduce a mensajes."],
     ["Factibilidad económica de los descuentos",
      "Explica por qué el descuento alcanza a las entradas y no a los servicios."],
     ["Desarrollo de la lógica de descuento por nivel",
      "Detalla el cálculo que el paso 6 muestra al cliente."],
     ["Guion de Interfaz de Usuario",
      "Define el lenguaje visual que la pantalla de armado continúa."]],
    widths=[176, 275])

d.save()
print("OK ->", OUT)
