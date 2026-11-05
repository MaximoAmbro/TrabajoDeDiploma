# -*- coding: utf-8 -*-
"""Examen Parcial 2 - TFI - parte 1 (intro + puntos a..e)."""
from ea_style import Doc, ML, MR
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)
_SAL  = os.path.join(_AQUI, "salida")
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
sys.path.insert(0, _AQUI)

def construir(d):
    d.h1("Examen Parcial 2 — Trabajo Final de Ingeniería")
    d.p("Universidad Abierta Interamericana · Facultad de Tecnología Informática. "
        "Proyecto EventAura — plataforma de venta de entradas con armado personalizado de "
        "paquetes. Alumnos: Ambrosino, Máximo · Riggio, Juan Cruz.", justify=False)

    d.h2("Caso de uso seleccionado y alcance de este documento", space_before=18)
    d.rich([("Se presenta ", "r"), ("CU-01 — Comprar paquete", "b"),
            (", que realiza el requerimiento RF-05 «Armado de paquetes personalizados». Es el "
             "caso de uso core del proyecto por tres motivos: es el requerimiento de mayor "
             "esfuerzo estimado (13 puntos sobre 78, el doble que cualquier otro), materializa "
             "el diferencial del producto —combinar entrada, hospedaje, traslado, merchandising "
             "y experiencia VIP en una sola operación— y es el que más reglas de negocio pone en "
             "juego: de las treinta y ocho vigentes, veintidós intervienen en su recorrido.", "r")])

    d.rich([("Sobre la versión de los diagramas. ", "b"),
            ("Los diagramas que siguen fueron elaborados contra el código efectivamente "
             "implementado y no contra el diseño preliminar de la versión 2.4 del informe. "
             "La construcción del software, realizada en diez etapas verificadas, introdujo "
             "cambios de modelo que el diseño original no anticipaba. Las diferencias no se "
             "ocultan: se documentan una por una en el anexo final, con su justificación.", "r")])

    # ---------------------------------------------------------------- a
    d.page_break()
    d.h2("a. Diagrama de casos de uso")
    d.p("El actor principal es el Cliente autenticado. La pasarela de pago participa como actor "
        "secundario: es un componente simulado que devuelve aprobado o rechazado según una regla "
        "fija, sin acceso a la red, conforme al alcance académico del proyecto.")
    d.image("d_casos.png", "Figura 1 — Casos de uso: CU-01 y sus relaciones", max_h=330)
    d.p("Las seis relaciones «include» corresponden a pasos que siempre se ejecutan dentro del "
        "caso de uso. La relación «extend» hacia la lista de espera se activa únicamente cuando "
        "el evento está agotado según RN-10, es decir, cuando ningún sector conserva capacidad "
        "sin vender.")

    d.h3("Especificación de CU-01")
    d.table(["Campo", "Contenido"], [
        ["Identificador", "CU-01 — Comprar paquete"],
        ["Actor principal", "Cliente (autenticado)"],
        ["Actor secundario", "Pasarela de pago simulada"],
        ["Requerimiento", "RF-05 (core) · RF-04 · RF-06 · RF-07 · RF-08"],
        ["Precondiciones", "El cliente tiene sesión iniciada. Existe al menos un evento publicado "
                           "con fecha futura y cupo disponible."],
        ["Postcondiciones", "Existe una fila en Compras en estado Confirmada, una fila en Entradas "
                            "por cada entrada adquirida con su código QR único, y un registro del "
                            "intento de pago en Pagos. El nivel de fidelidad del cliente queda "
                            "recalculado y rige desde la operación siguiente."]],
        widths=[96, 355], aligns=["l", "l"])

    d.h3("Flujo principal")
    for i, t in enumerate([
        "El cliente selecciona un evento del listado y abre la pantalla de compra.",
        "El sistema libera las reservas caducadas y muestra el cupo disponible por sector, junto "
        "con el nivel de fidelidad vigente del cliente y el porcentaje que le corresponde.",
        "El cliente indica la cantidad de entradas por sector y marca los servicios "
        "complementarios que desea incorporar.",
        "El sistema recalcula el resumen en cada cambio: base de entradas, descuento por nivel, "
        "subtotal, servicios, cargo por servicio, seguro y total.",
        "El cliente confirma. El sistema valida el paquete, registra la compra en estado "
        "Pendiente y reserva el cupo por diez minutos.",
        "El cliente ingresa los datos de la tarjeta en la pantalla de pago.",
        "El sistema valida el formato, solicita el cobro a la pasarela simulada y registra el "
        "intento con su resultado.",
        "Si el pago se aprueba, el sistema confirma la compra, emite una entrada con código QR "
        "único por cada unidad adquirida, graba las pólizas de seguro si corresponde y reevalúa "
        "el nivel del cliente.",
        "El sistema genera un PDF por entrada y muestra la pantalla de confirmación con el "
        "número de pedido y el desglose de lo pagado."]):
        d.numbered(i + 1, [(t, "r")])

    d.h3("Flujos alternativos")
    for cod, txt in [
        ("FA1 — Paquete sin entradas", "Si el cliente intenta confirmar habiendo elegido solo "
         "servicios, el sistema rechaza la operación citando RN-13: los servicios no se venden "
         "sueltos."),
        ("FA2 — Cupo insuficiente", "Si entre el armado y la confirmación otro cliente agotó el "
         "sector, la reserva falla y el sistema informa cuántas entradas quedan (RN-08)."),
        ("FA3 — Pago rechazado", "El sistema registra el intento, devuelve el cupo en el acto "
         "(RN-32) y deja la compra pendiente para que el cliente reintente con otra tarjeta "
         "dentro del plazo original."),
        ("FA4 — Reserva vencida", "Si transcurren diez minutos sin confirmar, la compra pasa a "
         "Vencida y el cupo vuelve a estar disponible (RN-15)."),
        ("FA5 — Evento agotado", "Si el evento no conserva cupo en ningún sector, la pantalla "
         "ofrece la inscripción en la lista de espera en lugar de la grilla de sectores (RN-10).")]:
        d.rich([(cod + ". ", "b"), (txt, "r")])

    # ---------------------------------------------------------------- b
    d.page_break()
    d.h2("b. Diagrama de dominio")
    d.p("El modelo distingue tres grupos: las entidades persistentes, que tienen tabla propia; "
        "el paquete, que vive únicamente en memoria durante el armado; y los parámetros de "
        "configuración, que externalizan los valores numéricos de las reglas.")
    d.image("d_dominio.png", "Figura 2 — Modelo de dominio según la implementación", max_h=540)

    d.h3("Decisiones de modelado que conviene justificar")
    d.rich([("Paquete no tiene tabla. ", "b"),
            ("Es una estructura en memoria que agrupa lo que el cliente está armando. Solo se "
             "materializa al iniciar la compra, momento en que se convierte en una fila de "
             "Compras con sus filas de Entradas y CompraServicios. Mientras el cliente no "
             "confirma, no hay nada escrito en la base.", "r")])
    d.rich([("Compra es una entidad propia y no un atributo de Entrada. ", "b"),
            ("RN-19 cuenta operaciones, no entradas: sin agrupar, un cliente que compra tres "
             "entradas en una transacción ascendería de nivel al triple de velocidad. Además "
             "RN-16 y RN-22 exigen congelar el nivel y el porcentaje aplicados, y esos son "
             "atributos de la operación.", "r")])
    d.rich([("Una fila de Entradas por entrada. ", "b"),
            ("RN-18 exige un código QR único por entrada, válido para un único ingreso, y la "
             "columna CodigoQR es una por fila. Lo mismo vale para Seguros, que lleva una póliza "
             "por entrada. Por eso CantidadVendida vale siempre uno.", "r")])
    d.rich([("Un solo catálogo de servicios con atributo Tipo. ", "b"),
            ("El diseño preliminar proponía una jerarquía con ProductoServicio abstracto y una "
             "subclase por tipo. Se descartó: las cuatro variantes no agregan comportamiento "
             "propio, solo difieren en un valor, y la jerarquía obligaría a una tabla por "
             "subclase sin beneficio alguno.", "r")])

    # ---------------------------------------------------------------- c
    d.page_break()
    d.h2("c. Prototipo del formulario de carga")
    d.p("El formulario de carga del caso de uso es la pantalla de armado del paquete. El "
        "prototipo que sigue está dibujado a escala a partir de las coordenadas declaradas en "
        "Visual/Cliente/FrmCompraCliente.Designer.cs, de modo que refleja la disposición real de "
        "los controles y no una maqueta idealizada.")
    d.image("d_form.png", "Figura 3 — FrmCompraCliente: armado del paquete", max_h=420)
    d.p("La pantalla resuelve tres hallazgos del informe de usabilidad. La columna «Disponibles» "
        "responde a H-03, que señalaba que el selector no informaba el cupo restante. El enlace "
        "«¿Cómo funcionan los niveles?» responde a H-05. Y la leyenda bajo el total, «El "
        "descuento se aplica sobre el valor de las entradas», responde a H-01, el hallazgo más "
        "repetido entre los participantes.")
    d.p("Los importes del resumen corresponden al caso de verificación numérica del proyecto: "
        "cliente de nivel Plata, dos entradas de Campo a $25.000 y un hospedaje de $45.000, con "
        "un total de $99.625. Ese número atraviesa el instructivo, la especificación del "
        "descuento y los casos de prueba, y funciona como control cruzado de la implementación.")
