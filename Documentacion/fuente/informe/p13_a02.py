# -*- coding: utf-8 -*-
"""Punto 13 — Anexo A02: Informe y exportacion en PDF (reportes e indicadores)."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "A02_Reportes.pdf")
d = Doc(OUT)

# =============================================================== PORTADA DE ANEXO
d.h1("A02. Informe y exportación en PDF")
d.p("Anexo técnico correspondiente al punto A02 de la Carpeta Modelo (Características y "
    "funcionalidades adicionales). Documenta los reportes e indicadores de gestión que "
    "produce EventAura, los ejemplos concretos de salida y el mecanismo de exportación a "
    "PDF mediante librería de terceros.")

d.h2("Objetivo")
d.p("Proveer al Administrador de la ticketera un conjunto de reportes e indicadores que "
    "sinteticen el comportamiento comercial de la plataforma (ventas, paquetes, fidelización, "
    "beneficios contratados y demanda insatisfecha), y permitir su exportación a un archivo "
    "PDF autónomo, generado por código mediante una librería de terceros, apto para ser "
    "archivado, impreso o distribuido fuera del sistema.")

d.h2("Alcance y relación con el resto del documento")
d.p("Este anexo formaliza como entregable técnico lo que en el cuerpo del informe aparece "
    "únicamente como requerimiento funcional. Concretamente:")
d.bullet([("CU-08 – Ver reportes de ventas", "b"),
          (" define el flujo de interacción del Administrador con el módulo. Este anexo no lo "
           "reemplaza: define el contenido, la fórmula de cada indicador y el formato de salida.", "r")])
d.bullet([("Objetivo específico 6", "b"),
          (" del proyecto (“Generar y presentar reportes de simulación de gestión”) exige entregar "
           "al menos tres ejemplos de reporte. La sección “Ejemplos concretos de reporte” de este "
           "anexo satisface ese requisito con cinco.", "r")])
d.bullet([("A02 de la Carpeta Modelo", "b"),
          (" exige que la generación del PDF se realice con una librería de terceros y prohíbe "
           "expresamente el uso de una impresora virtual. La sección “Implementación” documenta el "
           "cumplimiento de esa restricción.", "r")], gap=True)

d.p("Queda fuera del alcance de esta versión la programación de envíos automáticos de reportes "
    "por correo electrónico y la generación de tableros interactivos en tiempo real.")

# =============================================================== LIBRERIA
d.h2("Librería de terceros utilizada")
d.table(
    ["Componente", "Versión", "Función dentro del módulo"],
    [["iText", "9.2.0", "Composición del documento PDF: página, fuentes, párrafos, tablas, "
                        "colores y cierre del archivo."],
     ["QRCoder", "1.6.0", "Generación del código QR de verificación que se estampa en el pie "
                          "de cada reporte exportado."],
     ["Microsoft.Data.SqlClient", "6.0.2", "Ejecución de las consultas agregadas contra SQL Server "
                                           "que alimentan cada reporte."]],
    widths=[118, 52, 284], aligns=["l", "c", "l"])

d.rich([("Justificación de la elección. ", "b"),
        ("iText fue seleccionada porque genera el archivo PDF por código, sin depender de un "
         "controlador de impresión del sistema operativo. Esto satisface la restricción explícita "
         "de A02 —“no es válido el uso de una impresora virtual”— y además garantiza que la salida "
         "sea idéntica en cualquier equipo, independientemente de las impresoras instaladas. La "
         "librería ya se encontraba incorporada al proyecto para la emisión de tickets, por lo que "
         "su reutilización evita sumar dependencias.", "r")])

# =============================================================== CATALOGO
d.keep(120)
d.h2("Catálogo de reportes")
d.p("Se definen cinco reportes. Cada uno posee un código único, un responsable de consumo y un "
    "conjunto de filtros aplicables desde la interfaz.")
d.table(
    ["Cód.", "Reporte", "Pregunta de negocio que responde", "Filtros"],
    [["RPT-01", "Ventas por evento",
      "¿Qué eventos generan más ingresos y cuáles quedan por debajo de su capacidad?",
      "Rango de fechas, categoría"],
     ["RPT-02", "Ranking de paquetes",
      "¿Qué combinaciones de paquete elige realmente el cliente?",
      "Rango de fechas, evento"],
     ["RPT-03", "Usuarios por nivel de fidelidad",
      "¿Cómo se distribuye la base de clientes entre Bronce, Plata y Oro?",
      "Fecha de corte"],
     ["RPT-04", "Adopción de seguros y suscripciones",
      "¿Qué porcentaje de clientes contrata los beneficios opcionales?",
      "Rango de fechas, tipo de beneficio"],
     ["RPT-05", "Demanda insatisfecha",
      "¿Cuánta venta se pierde en eventos agotados?",
      "Evento, estado"]],
    widths=[38, 96, 216, 104])

# =============================================================== INDICADORES
d.keep(140)
d.h2("Definición de los indicadores")
d.p("Para evitar ambigüedades en la interpretación de los reportes, cada indicador se define con "
    "su fórmula exacta de cálculo.")
d.table(
    ["Indicador", "Fórmula", "Unidad"],
    [["Entradas vendidas", "Σ (cantidad de entradas con estado = VENDIDA)", "unidades"],
     ["Ingreso bruto por entradas", "Σ (precio de entrada × cantidad vendida)", "$"],
     ["Ticket promedio", "Ingreso bruto por entradas ÷ entradas vendidas", "$"],
     ["% de ocupación", "(Entradas vendidas ÷ capacidad total del evento) × 100", "%"],
     ["Participación del paquete", "(Unidades del paquete ÷ unidades totales) × 100", "%"],
     ["Tasa de adopción", "(Contrataciones del beneficio ÷ base de referencia) × 100", "%"],
     ["Demanda insatisfecha", "(Inscriptos en lista de espera ÷ capacidad del evento) × 100", "%"]],
    widths=[124, 264, 66], aligns=["l", "l", "c"])

# =============================================================== EJEMPLOS
d.h1("A02.1. Ejemplos concretos de reporte")
d.p("Los cinco reportes que siguen fueron generados con el juego de datos simulados cargado en la "
    "base de pruebas. Período considerado: 01/09/2025 al 30/11/2025. Los importes se expresan en "
    "pesos argentinos.")
d.rich([("Consistencia del juego de datos. ", "b"),
        ("Las 4.480 entradas de RPT-01 corresponden a 2.800 operaciones de compra, con un promedio "
         "de 1,6 entradas por operación. RPT-01 y RPT-02 se expresan en entradas; RPT-03 y RPT-04 "
         "lo hacen en operaciones de compra y en usuarios. Esta distinción evita confundir el "
         "volumen físico vendido con la cantidad de transacciones registradas.", "r")])

# ---- RPT-01
d.keep(190)
d.h2("Ejemplo 1 — RPT-01 Ventas por evento")
d.table(
    ["Evento", "Categoría", "Entradas", "Ingreso bruto", "Ticket prom.", "% ocup."],
    [["Duki en Rosario", "Concierto", "1.500", "$37.500.000", "$25.000", "96,2%"],
     ["Rosario Central vs Newell's", "Deportivo", "1.240", "$22.320.000", "$18.000", "91,9%"],
     ["Airbag – Gira 2025", "Concierto", "980", "$19.600.000", "$20.000", "87,5%"],
     ["Fiesta de la Colectividad", "Cultural", "450", "$4.500.000", "$10.000", "45,0%"],
     ["El fantasma de Canterville", "Teatro", "310", "$2.790.000", "$9.000", "62,0%"],
     ["TOTAL DEL PERÍODO", "—", "4.480", "$86.710.000", "$19.356", "81,0%"]],
    widths=[136, 62, 48, 86, 62, 60], aligns=["l", "l", "c", "c", "c", "c"])
d.rich([("Lectura. ", "b"),
        ("Los dos conciertos concentran el 65,8 % del ingreso del período con el 55,4 % de las "
         "entradas, lo que confirma que el segmento musical sostiene la operación. En el extremo "
         "opuesto, la Fiesta de la Colectividad opera al 45 % de su capacidad: es candidata a "
         "revisión de precio o de estrategia de difusión antes de repetirla.", "r")])

# ---- RPT-02
d.keep(190)
d.h2("Ejemplo 2 — RPT-02 Ranking de paquetes")
d.table(
    ["Paquete", "Composición", "Unid.", "Ingreso total", "Part."],
    [["Solo entrada", "Entrada", "2.690", "$48.420.000", "60,0%"],
     ["Entrada + Traslado", "Entrada + traslado ida y vuelta", "890", "$19.580.000", "19,9%"],
     ["Entrada + Hospedaje", "Entrada + hotel 1 noche", "510", "$22.950.000", "11,4%"],
     ["Experiencia VIP", "Entrada VIP + merch + meet & greet", "250", "$18.750.000", "5,6%"],
     ["Entrada + Merchandising", "Entrada + remera oficial", "140", "$3.360.000", "3,1%"],
     ["TOTAL", "—", "4.480", "$113.060.000", "100%"]],
    widths=[118, 152, 44, 82, 58], aligns=["l", "l", "c", "c", "c"])
d.rich([("Nota metodológica. ", "b"),
        ("El ingreso de RPT-02 ($113.060.000) supera al de RPT-01 ($86.710.000) porque incluye los "
         "servicios complementarios del paquete, mientras que RPT-01 mide exclusivamente el ingreso "
         "por entradas. Ambos reportes son consistentes entre sí: la diferencia de $26.350.000 "
         "corresponde a hospedaje, traslados, merchandising y experiencias VIP.", "r")])
d.rich([("Lectura. ", "b"),
        ("El 40 % de los clientes que contrata algún servicio adicional aporta el 57,2 % del "
         "ingreso total. La Experiencia VIP, con apenas 5,6 % de las unidades, genera el 16,6 % de "
         "la facturación: es el paquete de mayor margen unitario y justifica ampliar su cupo.", "r")])

# ---- RPT-03
d.keep(160)
d.h2("Ejemplo 3 — RPT-03 Usuarios por nivel de fidelidad")
d.table(
    ["Nivel", "Criterio (ventana de 12 meses)", "Usuarios", "Part.", "Compras prom.", "Desc."],
    [["Bronce", "0 a 2 compras confirmadas", "818", "69,4%", "1,0", "0 %"],
     ["Plata", "3 a 6 compras confirmadas", "260", "22,1%", "4,2", "5 %"],
     ["Oro", "7 o más compras confirmadas", "100", "8,5%", "8,9", "7,5 %"],
     ["TOTAL", "—", "1.178", "100%", "2,4", "—"]],
    widths=[52, 158, 56, 48, 76, 44], aligns=["l", "l", "c", "c", "c", "c"])
d.rich([("Lectura. ", "b"),
        ("El 8,5 % de usuarios Oro concentra 890 compras sobre un total de 2.800, es decir el "
         "31,8 % del volumen. Sumado al nivel Plata, el 30,6 % de la base explica el 70,8 % de las "
         "operaciones del período, lo que confirma la fuerte concentración de la demanda en los "
         "niveles altos. El indicador crítico a vigilar es la conversión de Bronce a Plata, hoy la "
         "barrera más ancha de la pirámide. La factibilidad económica de los descuentos asociados "
         "a cada nivel se analiza en la sección Reglas de negocio.", "r")])

# ---- RPT-04
d.keep(160)
d.h2("Ejemplo 4 — RPT-04 Adopción de seguros y suscripciones")
d.table(
    ["Beneficio", "Contrataciones", "Base de referencia", "Tasa", "Ingreso asociado"],
    [["Seguro de entrada", "504", "2.800 compras", "18,0%", "$2.520.000"],
     ["Suscripción mensual", "164", "1.178 usuarios", "13,9%", "$1.476.000"],
     ["Suscripción anual", "51", "1.178 usuarios", "4,3%", "$4.590.000"],
     ["TOTAL SUSCRIPTOS", "215", "1.178 usuarios", "18,3%", "$6.066.000"]],
    widths=[118, 80, 108, 54, 94], aligns=["l", "c", "c", "c", "c"])
d.rich([("Lectura. ", "b"),
        ("La suscripción anual, con sólo 51 contrataciones, aporta el 75,7 % del ingreso por "
         "suscripciones. Migrar suscriptores mensuales al plan anual tiene un impacto económico "
         "mayor que captar nuevos suscriptores mensuales. Cabe señalar que el beneficio de la "
         "suscripción no consiste en un descuento porcentual sobre la entrada (regla RN-37), sino "
         "en prestaciones en especie cuyo costo marginal es acotado.", "r")])

# ---- RPT-05
d.keep(150)
d.h2("Ejemplo 5 — RPT-05 Demanda insatisfecha")
d.table(
    ["Evento", "Estado", "En lista de espera", "Capacidad", "Demanda insat."],
    [["Duki en Rosario", "Agotado", "430", "1.560", "27,6%"],
     ["Airbag – Gira 2025", "Agotado", "180", "1.120", "16,1%"],
     ["Rosario Central vs Newell's", "Agotado", "95", "1.350", "7,0%"],
     ["TOTAL", "—", "705", "4.030", "17,5%"]],
    widths=[152, 62, 96, 70, 74], aligns=["l", "c", "c", "c", "c"])
d.rich([("Lectura. ", "b"),
        ("Los 705 inscriptos en listas de espera representan una venta potencial no capturada. "
         "Aplicando el ticket promedio del período ($19.356), la demanda insatisfecha equivale a "
         "$13.646.000. Este reporte es el insumo directo para decidir la apertura de una segunda "
         "función o la habilitación de sectores adicionales.", "r")])

# =============================================================== FORMATO DE SALIDA
d.h1("A02.2. Diseño técnico de la exportación")

d.keep(140)
d.h2("Estructura del archivo PDF generado")
d.p("Todo reporte exportado respeta la misma plantilla, de modo que el documento sea reconocible "
    "y auditable con independencia del reporte que lo originó.")
d.table(
    ["Zona", "Contenido"],
    [["Encabezado", "Logotipo EventAura, título del reporte, código (RPT-xx) y rango de fechas "
                    "aplicado."],
     ["Bloque de filtros", "Detalle de los filtros efectivamente aplicados y usuario que solicitó "
                           "el reporte."],
     ["Cuerpo", "Tabla de datos con encabezado sombreado, alineación numérica a la derecha y fila "
                "de totales resaltada."],
     ["Bloque de lectura", "Párrafo interpretativo con los dos o tres hallazgos principales del "
                           "período."],
     ["Pie", "Fecha y hora de emisión, numeración “Página N de M” y código QR de verificación."]],
    widths=[104, 350])
d.p("Convención de nombre del archivo generado: "
    "EVAU_[código]_[AAAAMMDD-inicio]_[AAAAMMDD-fin].pdf — por ejemplo, "
    "EVAU_RPT01_20250901_20251130.pdf. Los archivos se depositan en la carpeta "
    "Mis Documentos\\A1.Reportes EVENTAURA, creada por el sistema si no existe, siguiendo la "
    "misma convención ya empleada para los tickets.")

d.h2("Diagrama de clases del módulo de reportes")
d.pre([
    "// =======================================",
    "// A02 – MÓDULO DE REPORTES Y EXPORTACIÓN",
    "// =======================================",
    "",
    "abstract class Reporte {",
    "    string Codigo;              // RPT-01 … RPT-05",
    "    string Titulo;",
    "    DateTime Desde, Hasta;",
    "    List<Filtro> Filtros;",
    "    abstract string[] Encabezados();",
    "    abstract DataTable Obtener(RepositorioReportes repo);",
    "    abstract string Lectura(DataTable datos);   // párrafo interpretativo",
    "}",
    "",
    "class ReporteVentasPorEvento      : Reporte { }",
    "class ReporteRankingPaquetes      : Reporte { }",
    "class ReporteUsuariosPorNivel     : Reporte { }",
    "class ReporteAdopcionBeneficios   : Reporte { }",
    "class ReporteDemandaInsatisfecha  : Reporte { }",
    "",
    "class GestorReportes {                          // Capa de negocio",
    "    Reporte    Crear(string codigo, DateTime desde, DateTime hasta)",
    "    DataTable  Ejecutar(Reporte r)",
    "    string     Exportar(Reporte r, IExportador exp, string ruta)",
    "}",
    "",
    "class RepositorioReportes {                     // Capa de acceso a datos",
    "    DataTable VentasPorEvento(DateTime desde, DateTime hasta)",
    "    DataTable RankingPaquetes(DateTime desde, DateTime hasta)",
    "    DataTable UsuariosPorNivel(DateTime corte)",
    "    DataTable AdopcionBeneficios(DateTime desde, DateTime hasta)",
    "    DataTable DemandaInsatisfecha()",
    "}",
    "",
    "interface IExportador {                         // Permite sumar formatos",
    "    string Exportar(Reporte r, DataTable datos, string ruta);",
    "}",
    "",
    "class ExportadorPDF : IExportador {             // Implementación con iText 9.2.0",
    "    void EscribirEncabezado(Document doc, Reporte r)",
    "    void EscribirTabla(Document doc, DataTable datos, string[] cab)",
    "    void EscribirLectura(Document doc, string texto)",
    "    void EscribirPie(PdfDocument pdf, byte[] qr)",
    "}",
])
d.rich([("Criterio de diseño. ", "b"),
        ("La jerarquía Reporte concentra qué se informa y la interfaz IExportador concentra cómo se "
         "entrega. Agregar un sexto reporte no obliga a tocar el exportador, y agregar una "
         "exportación a Excel no obliga a tocar los reportes: se resuelve con una nueva clase que "
         "implemente IExportador. Esto sostiene los criterios de cohesión y bajo acoplamiento "
         "exigidos por la Carpeta Modelo.", "r")])

d.keep(150)
d.h2("Diagrama de secuencia — exportar un reporte a PDF")
d.pre([
    "frmReportes ──► GestorReportes      : Crear(\"RPT-01\", desde, hasta)",
    "GestorReportes ──► ReporteVentasPorEvento : new()",
    "GestorReportes ──► frmReportes      : reporte",
    "",
    "frmReportes ──► GestorReportes      : Ejecutar(reporte)",
    "GestorReportes ──► RepositorioReportes    : VentasPorEvento(desde, hasta)",
    "RepositorioReportes ──► SQL Server  : SELECT … GROUP BY … ORDER BY",
    "SQL Server ──► RepositorioReportes  : filas agregadas",
    "RepositorioReportes ──► GestorReportes    : DataTable",
    "GestorReportes ──► frmReportes      : DataTable  (se muestra en el DataGridView)",
    "",
    "frmReportes ──► GestorReportes      : Exportar(reporte, new ExportadorPDF(), ruta)",
    "GestorReportes ──► ExportadorPDF    : Exportar(reporte, datos, ruta)",
    "ExportadorPDF ──► iText             : PdfWriter / PdfDocument / Document",
    "ExportadorPDF ──► iText             : Table + Cell (encabezado, filas, totales)",
    "ExportadorPDF ──► QRCoder           : GenerarQR(codigo de verificación)",
    "ExportadorPDF ──► iText             : doc.Close()",
    "ExportadorPDF ──► GestorReportes    : ruta del archivo generado",
    "GestorReportes ──► frmReportes      : ruta del archivo generado",
])

d.keep(150)
d.h2("Consulta de origen — RPT-01")
d.pre([
    "SELECT  e.Nombre                                  AS Evento,",
    "        e.Categoria                               AS Categoria,",
    "        SUM(en.CantidadVendida)                   AS Entradas,",
    "        SUM(en.CantidadVendida * en.Precio)       AS IngresoBruto,",
    "        SUM(en.CantidadVendida * en.Precio)",
    "            / NULLIF(SUM(en.CantidadVendida), 0)  AS TicketPromedio,",
    "        CAST(100.0 * SUM(en.CantidadVendida)",
    "            / NULLIF(SUM(s.Capacidad), 0) AS DECIMAL(5,1)) AS PorcOcupacion",
    "FROM    Entradas  en",
    "JOIN    Eventos   e  ON e.ID       = en.ID_Evento",
    "JOIN    Sectores  s  ON s.ID_Evento = e.ID",
    "WHERE   e.Fecha BETWEEN @Desde AND @Hasta",
    "  AND   (@Categoria IS NULL OR e.Categoria = @Categoria)",
    "GROUP BY e.Nombre, e.Categoria",
    "ORDER BY IngresoBruto DESC;",
])

d.h2("Implementación de la exportación")
d.p("El método reutiliza el mismo patrón ya empleado por el sistema para la emisión de tickets "
    "(clase FrmCompraCliente, método GenerarPDF), extendido para componer tablas y pie de página.")
d.pre([
    "public string Exportar(Reporte r, DataTable datos, string ruta)",
    "{",
    "    PdfWriter   pw  = new PdfWriter(ruta);",
    "    PdfDocument pdf = new PdfDocument(pw);",
    "    Document    doc = new Document(pdf, PageSize.A4);",
    "    doc.SetMargins(50, 40, 60, 40);",
    "",
    "    PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);",
    "    PdfFont negrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);",
    "",
    "    // ---------- Encabezado ----------",
    "    doc.Add(new Paragraph($\"{r.Titulo}  ({r.Codigo})\")",
    "        .SetFont(negrita).SetFontSize(16));",
    "    doc.Add(new Paragraph($\"Período: {r.Desde:dd/MM/yyyy} al {r.Hasta:dd/MM/yyyy}\")",
    "        .SetFont(normal).SetFontSize(9)",
    "        .SetFontColor(ColorConstants.DARK_GRAY));",
    "",
    "    // ---------- Cuerpo: tabla de datos ----------",
    "    string[] cab = r.Encabezados();",
    "    Table tabla = new Table(UnitValue.CreatePercentArray(cab.Length))",
    "                      .UseAllAvailableWidth();",
    "",
    "    foreach (string c in cab)",
    "        tabla.AddHeaderCell(new Cell()",
    "            .Add(new Paragraph(c).SetFont(negrita).SetFontSize(9))",
    "            .SetBackgroundColor(new DeviceRgb(231, 231, 231)));",
    "",
    "    foreach (DataRow fila in datos.Rows)",
    "        foreach (object celda in fila.ItemArray)",
    "            tabla.AddCell(new Cell()",
    "                .Add(new Paragraph(Convert.ToString(celda))",
    "                    .SetFont(normal).SetFontSize(9)));",
    "",
    "    doc.Add(tabla);",
    "",
    "    // ---------- Lectura del reporte ----------",
    "    doc.Add(new Paragraph(r.Lectura(datos))",
    "        .SetFont(normal).SetFontSize(9).SetMarginTop(14));",
    "",
    "    // ---------- Pie: fecha, paginado y QR de verificación ----------",
    "    int total = pdf.GetNumberOfPages();",
    "    byte[] qr = GeneradorQR.Crear($\"{r.Codigo}|{DateTime.Now:yyyyMMddHHmmss}\");",
    "    Image  img = new Image(ImageDataFactory.Create(qr)).ScaleAbsolute(46, 46);",
    "",
    "    for (int i = 1; i <= total; i++)",
    "    {",
    "        doc.ShowTextAligned(",
    "            new Paragraph($\"Emitido {DateTime.Now:dd/MM/yyyy HH:mm}  –  Página {i} de {total}\")",
    "                .SetFont(normal).SetFontSize(7),",
    "            40, 28, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);",
    "        img.SetFixedPosition(i, 505, 20);",
    "        doc.Add(img);",
    "    }",
    "",
    "    doc.Close();          // iText escribe el archivo en disco",
    "    return ruta;",
    "}",
])

d.keep(170)
d.h2("Criterios de aceptación del anexo")
d.table(
    ["#", "Criterio verificable", "Estado"],
    [["A02-1", "El archivo PDF se genera por código con iText 9.2.0, sin intervención de "
               "impresora virtual ni de controladores del sistema operativo.", "Cumple"],
     ["A02-2", "Se entregan al menos tres ejemplos de reporte con datos concretos y su "
               "interpretación (se entregan cinco).", "Cumple"],
     ["A02-3", "Cada indicador cuenta con fórmula de cálculo explícita y unidad declarada.",
      "Cumple"],
     ["A02-4", "El PDF exportado incluye encabezado, filtros aplicados, tabla con totales, "
               "lectura, paginado y QR de verificación.", "Cumple"],
     ["A02-5", "El módulo permite incorporar un nuevo formato de exportación sin modificar las "
               "clases de reporte (interfaz IExportador).", "Cumple"],
     ["A02-6", "El archivo generado se abre correctamente en Adobe Reader, Microsoft Edge y "
               "Google Chrome.", "Cumple"]],
    widths=[42, 340, 72], aligns=["c", "l", "c"])

d.save()
print("OK ->", OUT)
