# -*- coding: utf-8 -*-
"""Examen Parcial 2 - TFI - parte 2 (puntos d..g)."""

import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)
_SAL  = os.path.join(_AQUI, "salida")
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
sys.path.insert(0, _AQUI)
def construir(d):
    # ---------------------------------------------------------------- d
    d.page_break()
    d.h2("d. Arquitectura de la solución")
    d.p("La solución se organiza en cuatro capas con dependencias unidireccionales. Cada capa es "
        "un proyecto independiente de la solución, de modo que la separación no es una convención "
        "de carpetas sino una restricción que el compilador hace cumplir: si Visual intentara "
        "abrir una conexión, no compilaría, porque no referencia el proveedor de datos.")
    d.image("d_arq.png", "Figura 4 — Arquitectura en cuatro capas", max_h=400)

    d.h3("Reglas de dependencia")
    for t in ["Visual nunca abre una conexión ni escribe SQL. Solo invoca a Negocio.",
              "Negocio no referencia System.Windows.Forms. No muestra mensajes: devuelve "
              "resultados o lanza excepciones de dominio que la capa de presentación traduce.",
              "Datos es el único lugar donde existen SqlCommand, SqlConnection y texto SQL.",
              "Entidades no referencia a nadie: son objetos con propiedades automáticas y sin "
              "lógica.",
              "Todo acceso a la base usa consultas parametrizadas; nunca concatenación de cadenas."]:
        d.bullet([(t, "r")])

    d.rich([("Consecuencia práctica de la elección de framework. ", "b"),
            ("Entidades, Negocio y Datos compilan contra netstandard2.0 y suman 5.374 líneas. "
             "Solo Visual, con 6.933 líneas, está atado a WinForms. Si el proyecto migrara a una "
             "interfaz web, las tres primeras capas se reutilizarían sin modificación desde una "
             "API, porque ningún gestor conoce la presentación. La excepción ReglaNegocioException "
             "expone el identificador de la regla incumplida, que en un servicio HTTP se traduce "
             "directamente al cuerpo de una respuesta 400.", "r")])

    # ---------------------------------------------------------------- e
    d.page_break()
    d.h2("e. Especificación de los modelos de datos")
    d.p("Se detallan las tablas que intervienen en CU-01. Los tipos son los efectivamente "
        "declarados en BaseDatos/01-esquema.sql. Todos los importes son DECIMAL(12,2): el "
        "proyecto prohíbe float y double para valores monetarios.")

    d.h3("Compras — la operación")
    d.table(["Columna", "Tipo", "Descripción"], [
        ["ID", "INT IDENTITY", "Clave primaria."],
        ["ID_Usuario", "INT NOT NULL", "FK a Usuario. Quién compró."],
        ["FechaCompra", "DATETIME NOT NULL", "Momento de inicio. Fija el plazo de la reserva."],
        ["Estado", "VARCHAR(12)", "Pendiente | Confirmada | Reembolsada | Vencida."],
        ["NivelAplicado", "VARCHAR(8)", "Nivel congelado al iniciar (RN-22)."],
        ["DescuentoAplicado", "DECIMAL(4,3)", "Porcentaje congelado (RN-16)."],
        ["BaseEntradas", "DECIMAL(12,2)", "Suma de las entradas, antes del descuento."],
        ["MontoDescuento", "DECIMAL(12,2)", "Descuento por nivel, solo sobre entradas (RN-21)."],
        ["MontoServicios", "DECIMAL(12,2)", "Servicios complementarios, sin descuento."],
        ["CargoServicio", "DECIMAL(12,2)", "15 % sobre el subtotal de entradas ya descontado."],
        ["MontoSeguro", "DECIMAL(12,2)", "Prima total. Cero si el cliente tiene suscripción."],
        ["ConSeguro", "BIT NOT NULL", "Si contrató el seguro. Necesaria porque con suscripción "
                                      "la prima vale cero y el monto no alcanza para saberlo."],
        ["TotalPagado", "DECIMAL(12,2)", "Lo que se cobró. No cambia ante una devolución."],
        ["MontoReembolsado", "DECIMAL(12,2)", "Acumulado devuelto (RN-29)."],
        ["ReservaVence", "DATETIME NULL", "Con valor, la compra retiene cupo; en NULL ya lo "
                                          "devolvió."]],
        widths=[104, 104, 243], aligns=["l", "l", "l"])

    d.rich([("Invariante de la fila. ", "b"),
            ("Para toda compra se cumple que BaseEntradas − MontoDescuento + MontoServicios + "
             "CargoServicio + MontoSeguro = TotalPagado. Es la condición que se verifica en las "
             "pruebas automatizadas sobre el total de las filas, y la razón por la que "
             "MontoReembolsado es una columna aparte en lugar de restarse del total pagado.", "r")])

    d.keep(190)
    d.h3("Entradas — una fila por entrada vendida")
    d.table(["Columna", "Tipo", "Descripción"], [
        ["ID", "INT IDENTITY", "Clave primaria."],
        ["ID_Compra", "INT NULL", "FK a Compras. Agrupa las entradas de una operación."],
        ["ID_Sector", "INT NULL", "FK a Sectores. Sin esta columna RN-08 no se puede evaluar."],
        ["Precio", "DECIMAL(12,2)", "Precio unitario congelado al armar el paquete (RN-16)."],
        ["CantidadVendida", "INT", "Vale siempre 1: una fila por entrada."],
        ["Estado", "VARCHAR(12)", "Pendiente | Confirmada | Reembolsada | Vencida."],
        ["CodigoQR", "VARCHAR(64)", "Único e irrepetible (RN-18). Índice único filtrado."]],
        widths=[104, 104, 243], aligns=["l", "l", "l"])

    d.h3("Restricciones que la base garantiza por estructura")
    d.p("Cinco reglas no se delegan al código: se imponen en el esquema. El motivo es que una "
        "validación en memoria no resiste dos operaciones simultáneas, y porque una regla "
        "escrita solo en C# deja de cumplirse ante cualquier escritura que no pase por la "
        "aplicación.")
    d.table(["Regla", "Restricción", "Qué impide"], [
        ["RN-01", "UQ_Usuario_Mail", "Dos cuentas con el mismo correo."],
        ["RN-08", "FK_Entradas_Sectores", "Una entrada sin sector: la regla quedaría inevaluable."],
        ["RN-18", "UX_Entradas_CodigoQR", "Un QR repetido habilitaría dos ingresos."],
        ["RN-25", "UX_Suscripcion_Activa", "Dos suscripciones activas por usuario (índice "
                                           "filtrado sobre Activa = 1)."],
        ["RN-36", "UX_ListaEspera", "Inscribirse dos veces en el mismo evento."]],
        widths=[52, 150, 249], aligns=["c", "l", "l"])

    d.h3("Consulta de cupo disponible")
    d.p("El control de disponibilidad resuelve el conflicto entre RN-08 y RN-15: lo disponible "
        "descuenta tanto lo vendido como lo reservado y aún vigente.")
    d.code([
        "SELECT  s.Capacidad",
        "      - ISNULL((SELECT SUM(e.CantidadVendida)",
        "                FROM   Entradas e",
        "                WHERE  e.ID_Sector = s.ID",
        "                  AND  e.Estado    = 'Confirmada'), 0)",
        "      - s.CantidadReservada                        AS Disponible",
        "FROM    Sectores s",
        "WHERE   s.ID = @IdSector;"])
    d.p("La reserva se toma con un UPDATE condicional en una sola sentencia, cuya cláusula WHERE "
        "repite ese cálculo. Así la verificación y la escritura son atómicas: dos compras "
        "simultáneas sobre el último lugar no pueden sobrevender, porque la segunda no afecta "
        "ninguna fila y la transacción se deshace.")

    # ---------------------------------------------------------------- f
    d.page_break()
    d.h2("f. Diagrama de robustez")
    d.p("El diagrama aplica la notación ICONIX y respeta sus reglas de conexión: el actor solo "
        "toca objetos de interfaz; los de interfaz solo hablan con controladores; y únicamente "
        "los controladores acceden a entidades. Esa disciplina es la que se traduce luego en las "
        "cuatro capas de la arquitectura.")
    d.image("d_robustez.png", "Figura 5 — Robustez de CU-01 (ICONIX)", max_h=350)
    d.p("La correspondencia con el código es directa: los objetos de interfaz son los "
        "formularios del proyecto Visual, los controladores son los gestores del proyecto "
        "Negocio, y las entidades son las clases del proyecto Entidades que los repositorios "
        "materializan. No hay ninguna flecha entre un objeto de interfaz y una entidad, que es "
        "exactamente la regla que impide que un formulario escriba SQL.")

    # ---------------------------------------------------------------- g
    d.page_break()
    d.h2("g. Especificación de los formularios de carga")
    d.h3("FrmCompraCliente — armado del paquete")
    d.table(["Control", "Tipo", "Entrada y validación"], [
        ["dgvSectores", "DataGridView", "Una fila por sector. Solo la columna Cantidad es "
         "editable; acepta enteros no negativos. Si el valor supera el cupo se rechaza con RN-08 "
         "y la celda vuelve al valor anterior."],
        ["chkHospedaje, chkTraslado, chkMerch, chkVIP", "CheckBox", "Un servicio por tipo. Se "
         "habilitan solo si el evento los tiene asociados (RN-14); si no, el texto indica «no "
         "disponible» y el control queda deshabilitado."],
        ["chkSeguro", "CheckBox", "Opcional. Solo puede contratarse durante la compra (RN-28)."],
        ["lblResumen", "Label", "Solo lectura. Recalculado ante cualquier cambio."],
        ["btnComprar", "Button", "Valida el paquete completo y reserva el cupo. Si no hay "
         "seguro, pide confirmación explícita (H-04)."]],
        widths=[112, 74, 265], aligns=["l", "l", "l"])

    d.h3("Validaciones de negocio que se disparan desde este formulario")
    for cod, txt in [
        ("RN-12", "Máximo seis entradas por operación, sumando todos los sectores."),
        ("RN-13", "El paquete debe contener al menos una entrada."),
        ("RN-08", "La cantidad pedida no supera el cupo disponible del sector."),
        ("RN-09", "El evento debe estar publicado y con fecha futura."),
        ("RN-39", "Durante la preventa, solo compra un cliente con suscripción activa.")]:
        d.rich([(cod + ". ", "b"), (txt, "r")])

    d.keep(210)
    d.h3("frmPago — datos de la tarjeta")
    d.table(["Control", "Tipo", "Entrada y validación"], [
        ["txtTitular", "TextBox", "Obligatorio, hasta 60 caracteres."],
        ["txtNumero", "TextBox", "Entre 13 y 19 dígitos (ISO/IEC 7812). Se admiten espacios y "
         "guiones, que se descartan."],
        ["txtVencimiento", "TextBox", "Formato MM/AA. Se rechaza una tarjeta vencida; vale hasta "
         "el último día del mes impreso."],
        ["txtCvv", "TextBox", "Tres o cuatro dígitos. Se muestra enmascarado y se limpia tras "
         "cada intento."],
        ["lblTemporizador", "Label", "Cuenta regresiva de la reserva, siempre visible (H-02). "
         "Al llegar a cero libera el cupo y cierra la pantalla."]],
        widths=[112, 74, 265], aligns=["l", "l", "l"])
    d.rich([("Tratamiento de los datos sensibles. ", "b"),
            ("La clase DatosTarjeta es transitoria: no tiene tabla y su instancia vive solo "
             "mientras dura el cobro. De la tarjeta se persisten únicamente los últimos cuatro "
             "dígitos y un token ficticio, y una restricción CHECK en la columna Ultimos4 impide "
             "por estructura que se cuele otra cosa. El número completo nunca llega a la capa de "
             "datos (RN-30).", "r")])
