# -*- coding: utf-8 -*-
"""Examen Parcial 2 - TFI - parte 3 (puntos h..j + anexo)."""

import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)
_SAL  = os.path.join(_AQUI, "salida")
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
sys.path.insert(0, _AQUI)
def construir(d):
    # ---------------------------------------------------------------- h
    d.page_break()
    d.h2("h. Diagramas de secuencia")
    d.p("El caso de uso se presenta en dos diagramas porque tiene dos transacciones distintas, "
        "separadas por la interacción del cliente con la pantalla de pago. Entre una y otra la "
        "compra queda persistida en estado Pendiente, de modo que la reserva sobrevive incluso "
        "si la aplicación se cierra.")

    d.pagina_apaisada("d_sec1.png", "Figura 6 — Secuencia: del armado a la reserva",
                      "h.1  Armado del paquete y reserva del cupo")
    d.p("Dos detalles que el diagrama hace explícitos. El primero es que el descuento se calcula "
        "dos veces: una al mostrar el resumen en pantalla y otra al iniciar la compra. No es "
        "redundancia: el nivel del cliente se lee de la entidad y los precios ya están "
        "congelados en el paquete, de modo que ambos cálculos coinciden y el segundo es el que "
        "queda grabado. El segundo es que la liberación de reservas vencidas ocurre al abrir la "
        "pantalla: no hay proceso en segundo plano, así que la limpieza es perezosa.")

    d.pagina_apaisada("d_sec2.png", "Figura 7 — Secuencia: del cobro a la entrada emitida",
                      "h.2  Pago simulado, confirmación y emisión")
    d.p("El fragmento alternativo distingue las dos ramas. En ambas el intento queda registrado "
        "en Pagos antes de decidir, que es lo que exige RN-31: se registra la operación de pago "
        "con su estado resultante, haya sido aprobada o no. En la rama de rechazo el cupo se "
        "devuelve de inmediato y la compra permanece en Pendiente, de modo que el cliente puede "
        "reintentar con otra tarjeta sin rearmar el paquete; el plazo, eso sí, sigue corriendo "
        "desde que la compra se inició.")

    # ---------------------------------------------------------------- i
    d.page_break()
    d.h2("i. Implementación de patrones de diseño")
    d.p("Se documentan tres patrones efectivamente implementados, con su ubicación en el código "
        "y el problema concreto que resuelven. Se omiten deliberadamente los patrones que el "
        "proyecto menciona como previstos pero no construidos, para no atribuir al sistema algo "
        "que no tiene.")

    d.h3("Singleton — instancia única por gestor y repositorio")
    d.rich([("Dónde. ", "b"), ("Veintinueve clases: los catorce repositorios de Datos y los "
            "quince gestores de Negocio. La implementación sigue el mismo molde en todas, con "
            "constructor privado y propiedad estática Instance de inicialización perezosa.", "r")])
    d.rich([("Qué resuelve. ", "b"), ("Dos cosas distintas. En los repositorios, concentra la "
            "construcción de conexiones en un punto único. En GestorParametros es el patrón el "
            "que hace viable la regla de que ningún valor numérico esté escrito en el código: la "
            "tabla Parametros se lee una sola vez por ejecución y queda en memoria, de modo que "
            "consultar el porcentaje de descuento no cuesta una consulta por invocación.", "r")])
    d.code([
        "public sealed class GestorFidelizacion",
        "{",
        "    private static GestorFidelizacion _instance;",
        "",
        "    public static GestorFidelizacion Instance",
        "    {",
        "        get",
        "        {",
        "            if (_instance == null) { _instance = new GestorFidelizacion(); }",
        "            return _instance;",
        "        }",
        "    }",
        "",
        "    private GestorFidelizacion() { }",
        "}"])
    d.rich([("Limitación asumida. ", "b"), ("La inicialización no es segura ante hilos. La "
            "aplicación es de escritorio y de un solo hilo de interfaz, de modo que la condición "
            "de carrera no puede darse; en un servicio web habría que sincronizar el acceso o "
            "delegar la vida del objeto a un contenedor de inyección.", "r")])

    d.keep(260)
    d.h3("Strategy — formatos de exportación intercambiables")
    d.rich([("Dónde. ", "b"), ("La interfaz IExportador se declara en Negocio y ExportadorPDF la "
            "implementa en Visual. GestorReportes recibe la estrategia como parámetro y no "
            "conoce ninguna implementación concreta.", "r")])
    d.code([
        "public interface IExportador",
        "{",
        "    string Exportar(Reporte r, DataTable datos, string ruta);",
        "}",
        "",
        "// En GestorReportes: la estrategia entra por parametro.",
        "public string Exportar(Reporte r, IExportador exp, string ruta)",
        "{",
        "    return exp.Exportar(r, Ejecutar(r), ruta);",
        "}"])
    d.rich([("Qué resuelve. ", "b"), ("Separa qué se informa de cómo se entrega. Agregar una "
            "exportación a Excel es una clase nueva que implemente la interfaz, sin tocar "
            "ninguno de los cinco reportes. Pero el beneficio decisivo es de arquitectura: iText "
            "es una librería de presentación y Negocio no la referencia. Sin esta interfaz, "
            "generar el PDF desde la capa de negocio habría obligado a que Negocio dependiera de "
            "iText, rompiendo la regla de capas.", "r")])

    d.keep(250)
    d.h3("Template Method — esqueleto común de los reportes")
    d.rich([("Dónde. ", "b"), ("La clase abstracta Reporte define el algoritmo y delega tres "
            "pasos en sus cinco subclases concretas.", "r")])
    d.code([
        "public abstract class Reporte",
        "{",
        "    public abstract string[]  Encabezados();",
        "    public abstract DataTable Obtener(RepositorioReportes repo);",
        "    public abstract string    Lectura(DataTable datos);",
        "}"])
    d.rich([("Qué resuelve. ", "b"), ("Los cinco reportes comparten la estructura —encabezado, "
            "filtros aplicados, tabla con totales, párrafo interpretativo y pie— y difieren solo "
            "en la consulta, en los nombres de las columnas y en cómo se redacta la lectura. El "
            "exportador recorre esa estructura sin saber qué reporte está componiendo. Un sexto "
            "reporte es una subclase nueva.", "r")])

    d.h3("Patrones de arquitectura")
    d.rich([("Repository. ", "b"), ("Cada repositorio encapsula el acceso a un agregado y expone "
            "métodos del dominio —ObtenerPorId, ContarComprasConfirmadas— en lugar de consultas. "
            "La capa de negocio nunca ve una sentencia SQL.", "r")])
    d.rich([("Capas. ", "b"), ("La separación en cuatro proyectos con dependencias "
            "unidireccionales, descrita en el punto d.", "r")])

    # ---------------------------------------------------------------- j
    d.page_break()
    d.h2("j. Gestión de configuración")
    d.h3("Datos demo")
    d.p("El juego de datos está versionado como código, en tres guiones ejecutables en orden. "
        "Ninguno depende del estado previo de la base, de modo que cualquier integrante del "
        "equipo reconstruye un entorno idéntico en menos de un minuto.")
    d.table(["Guion", "Qué carga", "Para qué"], [
        ["01-esquema.sql", "Las tablas, claves, índices y restricciones.",
         "Crea el esquema completo. Incluye las tablas preexistentes con guardas, de modo que "
         "sirve tanto para una base vacía como para una ya poblada."],
        ["02-semilla.sql", "Diecisiete parámetros, cuatro usuarios —uno por nivel más el "
         "administrador—, un local, tres eventos, sus sectores, cuatro servicios y un historial "
         "de compras coherente con el nivel de cada cliente.",
         "Es el mínimo para recorrer los casos de uso. El historial está construido para que "
         "cada usuario quede efectivamente en su nivel según RN-19."],
        ["03-datos-reporte.sql", "1.178 usuarios, 2.800 compras y 4.480 entradas distribuidas en "
         "cinco eventos.",
         "Da volumen a los reportes. Se ejecuta aparte y puede correrse dos veces seguidas "
         "porque empieza borrando lo que él mismo cargó."]],
        widths=[96, 170, 185], aligns=["l", "l", "l"])
    d.rich([("Por qué los datos demo son código y no un respaldo. ", "b"),
            ("Un archivo de respaldo binario no se puede revisar en un cambio, no se puede "
             "comparar entre versiones y queda desactualizado en cuanto el esquema cambia. Un "
             "guion versionado se lee, se discute y evoluciona junto al esquema que lo "
             "acompaña.", "r")])

    d.h3("Control de versiones")
    d.p("El repositorio está en Git con remoto en GitHub. La estrategia de commits es deliberada: "
        "una unidad de trabajo verificable por commit, con el mensaje identificando la etapa.")
    d.table(["Prefijo del mensaje", "Qué agrupa", "Commits"], [
        ["Seccion N: …", "Una etapa de construcción con su verificación. Las etapas 4 y 5 "
                         "ocupan dos commits cada una porque se cerraron en dos entregas.", "12"],
        ["Correccion …", "Defectos detectados al verificar una etapa ya cerrada. Agrupan nueve "
                         "correcciones identificadas como C-1 a C-9.", "3"],
        ["Docs: …", "Cambios de documentación sin efecto sobre el código.", "5"],
        ["Esquema: …", "Correcciones de integridad sobre el modelo de datos.", "2"],
        ["Otros", "Preparación del repositorio, README e instructivos.", "6"]],
        widths=[96, 270, 85], aligns=["l", "l", "c"])
    d.p("Veintiocho commits en total. La relación entre correcciones y commits no es uno a uno: "
        "las siete primeras correcciones se agruparon en un solo commit por tratarse de la "
        "misma revisión, mientras que C-8 y C-9 se aislaron porque cada una resolvía un defecto "
        "independiente detectado después.")
    d.p("El valor de esta disciplina se verificó en la práctica. Al cerrar la etapa nueve se "
        "detectó que un cliente con suscripción quedaba sin póliza de seguro, en silencio. Para "
        "demostrar que el defecto era real y no una interpretación, se reconstruyó el binario de "
        "la etapa anterior en un árbol de trabajo separado y se ejecutó contra él la misma "
        "prueba: cero pólizas antes del cambio, tres después. Eso solo es posible si cada etapa "
        "quedó en un commit propio y compilable.")

    d.h3("Trazabilidad entre el código y los requerimientos")
    d.p("Cada regla de negocio aparece citada por su identificador en el comentario del método "
        "que la implementa. La consecuencia es que el recorrido entre el requerimiento y la "
        "línea de código que lo cumple es una búsqueda de texto, en cualquiera de los dos "
        "sentidos.")

    # ---------------------------------------------------------------- anexo
    d.page_break()
    d.h2("Anexo — Deriva entre el diseño preliminar y la implementación")
    d.p("Los diagramas de la versión 2.4 del informe se elaboraron antes de construir el "
        "software. La implementación obligó a revisarlos. Se consignan las diferencias con su "
        "motivo, porque un diagrama que contradice al código es peor que no tenerlo.")
    d.table(["Diseño preliminar", "Implementación", "Motivo del cambio"], [
        ["Cliente acumula puntos; NivelUsuario define umbrales de puntos.",
         "No hay puntos. El nivel se deriva de contar compras confirmadas en una ventana móvil "
         "de doce meses.",
         "Un saldo de puntos solo crece y vuelve vitalicio el beneficio. La ventana móvil hace "
         "que el nivel pueda bajar solo, lo que acota el costo del programa. Se analizó en la "
         "sección de factibilidad económica."],
        ["Carrito como entidad persistente.",
         "Paquete en memoria, sin tabla.",
         "Lo que el cliente arma antes de confirmar no es información que el negocio necesite "
         "conservar. Persistirlo habría obligado a limpiar carritos abandonados."],
        ["ProductoServicio abstracto con subclases Hospedaje, Traslado y Merchandising.",
         "Una tabla Servicios con atributo Tipo.",
         "Las variantes no agregan comportamiento, solo un valor distinto."],
        ["Entrada con fila y asiento.",
         "Entrada con sector, sin numeración.",
         "Ningún requerimiento pide asiento numerado; el cupo se controla por sector (RN-08)."],
        ["Estados de la entrada: Disponible, Reservada, Vendida, Cancelada.",
         "Pendiente, Confirmada, Reembolsada, Vencida.",
         "Una reserva caducada no es una cancelación: se distingue para conservar el rastro del "
         "intento que exige RN-31."],
        ["Importes declarados como float.",
         "decimal en todo el sistema.",
         "float introduce error de representación en operaciones monetarias."],
        ["Sin tablas de pagos, seguros, suscripciones ni parámetros.",
         "Pagos, Seguros, Suscripciones, CompraServicios y Parametros.",
         "RN-30 y RN-31 no tenían dónde registrarse, y los valores de las reglas estaban "
         "escritos en el código."],
        ["Controladores denominados ControladorCompra y ServicioCompra.",
         "GestorPaquetes, GestorFidelizacion y GestorCompras.",
         "La responsabilidad quedó repartida según el momento del ciclo de vida: armar, calcular "
         "y cerrar la operación."]],
        widths=[128, 128, 195], aligns=["l", "l", "l"])
    d.p("La misma revisión alcanzó al anexo A02 del informe, cuya consulta de ejemplo para el "
        "primer reporte no era ejecutable contra el esquema real: nombraba una columna "
        "inexistente, multiplicaba las filas por un producto cartesiano y no filtraba por estado. "
        "La versión corregida y el detalle de los tres errores están documentados en el "
        "repositorio.")
