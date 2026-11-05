# -*- coding: utf-8 -*-
"""Punto 19 — Desarrollo de la logica de descuento por nivel de fidelidad."""
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)          # Documentacion/fuente
_SAL  = os.path.join(_AQUI, "salida")   # PDF parciales
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
from ea_style import Doc

OUT = os.path.join(_SAL, "Descuento_Nivel.pdf")
d = Doc(OUT)

d.h1("Desarrollo de la lógica de descuento por nivel")

d.p("El requerimiento RF-08 y las reglas RN-19 a RN-24 establecen qué debe hacer el sistema de "
    "fidelización, pero no alcanzan para construirlo. Esta sección traduce esas definiciones en "
    "un diseño implementable: el modelo de datos que lo sostiene, los dos algoritmos que lo "
    "resuelven, las clases que lo encapsulan y el juego de pruebas que lo verifica.")

d.h2("Alcance")
d.table(
    ["Elemento", "De dónde proviene"],
    [["Umbrales de nivel y ventana de doce meses", "RN-19"],
     ["Porcentajes 0 %, 5 % y 7,5 %", "RN-20, validados en Factibilidad económica"],
     ["Base de cálculo sin servicios complementarios", "RN-21"],
     ["Congelamiento del nivel durante la compra", "RN-16 y RN-22"],
     ["Reevaluación mensual del nivel", "RN-23"],
     ["Exclusión de compras reembolsadas", "RN-24"],
     ["Ausencia de acumulación con la suscripción", "RN-37"]],
    widths=[246, 205])

# ==================================================================== MODELO DE DATOS
d.keep(230)
d.h2("Modelo de datos requerido")
d.p("El esquema de la primera versión contempla las entidades Usuario, Eventos, Sectores, "
    "Entradas, Locales y Propietario. La tabla Entradas registra una fila por sector adquirido, "
    "con el usuario y la fecha de venta. Ese diseño resulta suficiente para vender, pero no para "
    "aplicar RN-19.")
d.rich([("El problema. ", "b"),
        ("RN-19 cuenta compras confirmadas, no entradas. Un cliente que en una misma transacción "
         "adquiere una entrada de Campo y otra de Platea genera dos filas en Entradas. Contadas "
         "como dos compras, ese cliente asciende de nivel al doble de velocidad que otro con el "
         "mismo comportamiento repartido en dos transacciones. El esquema, tal como está, hace que "
         "la regla arroje un resultado incorrecto.", "r")])
d.p("Se incorpora entonces la operación de compra como entidad propia. La decisión no es de "
    "conveniencia: RN-16 y RN-22 obligan a conservar el nivel y el porcentaje que se aplicaron en "
    "el momento de la operación, y esos son atributos de la operación, no de cada entrada. "
    "Repetirlos en cada fila de Entradas generaría anomalías de actualización.")
d.table(
    ["Entidad", "Definición", "Justificación"],
    [["Compras", "Entidad nueva. Registra la operación: usuario, fecha, estado, nivel y "
                 "porcentaje aplicados, base, monto de descuento y total.",
      "RN-19 necesita contar operaciones. RN-16 y RN-22 necesitan que el nivel y el porcentaje "
      "queden congelados."],
     ["Entradas", "Se agrega ID_Compra como clave foránea hacia Compras.",
      "Vincula cada entrada con la operación que la originó."],
     ["Usuario", "Se agregan Nivel y FechaUltimaEvaluacion.",
      "RN-23 fija el nivel entre evaluaciones mensuales; por lo tanto debe persistirse y no "
      "derivarse en cada consulta."]],
    widths=[64, 194, 193])
d.rich([("Nota sobre el atributo Nivel. ", "b"),
        ("Podría pensarse que almacenar el nivel es una desnormalización, dado que se deduce del "
         "conteo de compras. No lo es: por RN-23 el nivel permanece estable entre dos "
         "reevaluaciones mensuales, aun cuando la ventana móvil ya arroje un valor distinto. El "
         "nivel vigente y el nivel calculado son dos datos diferentes, y sólo el primero rige "
         "sobre una compra en curso.", "r")])

d.keep(240)
d.h2("Definición de las estructuras")
d.pre([
    "CREATE TABLE Compras (",
    "    ID                INT IDENTITY(1,1) PRIMARY KEY,",
    "    ID_Usuario        INT           NOT NULL,",
    "    FechaCompra       DATETIME      NOT NULL,",
    "    Estado            VARCHAR(12)   NOT NULL,   -- Pendiente|Confirmada|Reembolsada",
    "    NivelAplicado     VARCHAR(8)    NOT NULL,   -- Bronce|Plata|Oro",
    "    DescuentoAplicado DECIMAL(4,3)  NOT NULL,   -- 0.000 | 0.050 | 0.075",
    "    BaseEntradas      DECIMAL(12,2) NOT NULL,",
    "    MontoDescuento    DECIMAL(12,2) NOT NULL,",
    "    TotalPagado       DECIMAL(12,2) NOT NULL,",
    "    CONSTRAINT FK_Compras_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID),",
    "    CONSTRAINT CK_Compras_Estado  CHECK (Estado IN ('Pendiente','Confirmada',",
    "                                                    'Reembolsada'))",
    ");",
    "",
    "ALTER TABLE Entradas ADD ID_Compra INT NULL;",
    "ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Compras",
    "      FOREIGN KEY (ID_Compra) REFERENCES Compras(ID);",
    "",
    "ALTER TABLE Usuario  ADD Nivel VARCHAR(8) NOT NULL DEFAULT 'Bronce';",
    "ALTER TABLE Usuario  ADD FechaUltimaEvaluacion DATE NULL;",
    "",
    "-- El conteo de la ventana movil se ejecuta en cada compra: requiere indice.",
    "CREATE INDEX IX_Compras_Usuario_Fecha",
    "    ON Compras (ID_Usuario, FechaCompra) INCLUDE (Estado);",
])

# ==================================================================== ALGORITMOS
d.keep(220)
d.h2("Algoritmo de determinación del nivel")
d.p("Se ejecuta en dos momentos: durante la reevaluación mensual prevista por RN-23 y al "
    "confirmarse una compra, para dejar registrado el contador actualizado.")
d.pre([
    "FUNCIÓN DeterminarNivel(idUsuario, fechaReferencia) → NivelFidelidad",
    "",
    "    n ← cantidad de Compras del usuario tales que",
    "            Estado = 'Confirmada'                              // RN-24",
    "            y FechaCompra ∈ (fechaReferencia − 12 meses ; fechaReferencia]   // RN-19",
    "",
    "    SI n ≥ 7  DEVOLVER Oro",
    "    SI n ≥ 3  DEVOLVER Plata",
    "    DEVOLVER  Bronce",
])
d.p("La consulta que resuelve el conteo, parametrizada para evitar inyección de SQL:")
d.pre([
    "SELECT COUNT(*)",
    "FROM   Compras",
    "WHERE  ID_Usuario  =  @IdUsuario",
    "  AND  Estado      =  'Confirmada'",
    "  AND  FechaCompra >  DATEADD(MONTH, -12, @FechaReferencia)",
    "  AND  FechaCompra <= @FechaReferencia;",
])

d.keep(230)
d.h2("Algoritmo de cálculo del descuento")
d.p("El nivel no se recalcula en este punto: se lee el valor persistido en el cliente. Esa "
    "decisión es la que materializa RN-16 y RN-22, porque garantiza que una compra que provoca un "
    "ascenso no se beneficie a sí misma.")
d.pre([
    "FUNCIÓN CalcularDescuento(cliente, paquete) → ResumenDescuento",
    "",
    "    base       ← Σ (precio × cantidad) de las ENTRADAS del paquete       // RN-21",
    "    porcentaje ← TablaDescuento[cliente.Nivel]                           // RN-20",
    "    monto      ← redondear(base × porcentaje, 2)",
    "    subtotalE  ← base − monto",
    "",
    "    servicios  ← Σ precio de los servicios complementarios    // sin descuento, RN-21",
    "    cargo      ← redondear(subtotalE × 0,15 ; 2)              // cargo por servicio",
    "    total      ← subtotalE + servicios + cargo",
    "",
    "    DEVOLVER ResumenDescuento(cliente.Nivel, base, porcentaje, monto,",
    "                              subtotalE, servicios, cargo, total)",
])
d.rich([("Sobre la suscripción. ", "b"),
        ("El algoritmo no contempla un segundo porcentaje porque RN-37 lo excluye. Los beneficios "
         "de la suscripción —seguro bonificado, preventa y prioridad— se resuelven fuera de este "
         "cálculo y no alteran el importe de la entrada.", "r")])

# ==================================================================== CLASES
d.keep(250)
d.h2("Diseño de clases")
d.p("La distribución respeta la arquitectura en capas ya adoptada por el sistema, con el gestor "
    "de negocio implementado como singleton, del mismo modo que GestorClientes.")
d.pre([
    "// =======================================",
    "// RF-08 – FIDELIZACIÓN Y DESCUENTO POR NIVEL",
    "// =======================================",
    "",
    "enum NivelFidelidad { Bronce, Plata, Oro }            // Entidades",
    "enum EstadoCompra   { Pendiente, Confirmada, Reembolsada }",
    "",
    "class Cliente : Usuario {                             // Entidades (ampliada)",
    "    NivelFidelidad Nivel;",
    "    DateTime?      FechaUltimaEvaluacion;",
    "    int            ComprasUltimos12Meses;",
    "}",
    "",
    "class Compra {                                        // Entidades (nueva)",
    "    int            ID;",
    "    int            ID_Usuario;",
    "    DateTime       FechaCompra;",
    "    EstadoCompra   Estado;",
    "    NivelFidelidad NivelAplicado;      // congelado – RN-22",
    "    decimal        DescuentoAplicado;  // congelado – RN-16",
    "    decimal        BaseEntradas, MontoDescuento, TotalPagado;",
    "    List<Entrada>  Entradas;",
    "}",
    "",
    "class ResumenDescuento {                              // Entidades – DTO hacia la UI",
    "    NivelFidelidad NivelAplicado;",
    "    decimal        BaseEntradas, Porcentaje, MontoDescuento;",
    "    decimal        SubtotalEntradas, Servicios, CargoServicio, Total;",
    "}",
    "",
    "class GestorFidelizacion {                            // Negocio – patrón Singleton",
    "    static GestorFidelizacion Instance { get; }",
    "    NivelFidelidad   DeterminarNivel(int idUsuario, DateTime fecha)",
    "    decimal          PorcentajePara(NivelFidelidad nivel)",
    "    ResumenDescuento CalcularDescuento(Cliente cliente, Paquete paquete)",
    "    void             ReevaluarNivel(Cliente cliente, DateTime fecha)   // RN-23",
    "}",
    "",
    "class RepositorioFidelizacion {                       // Datos",
    "    int  ContarComprasConfirmadas(int idUsuario, DateTime desde, DateTime hasta)",
    "    void ActualizarNivel(int idUsuario, NivelFidelidad nivel, DateTime fecha)",
    "}",
])

d.keep(250)
d.h2("Implementación")
d.pre([
    "public decimal PorcentajePara(NivelFidelidad nivel)",
    "{",
    "    switch (nivel)                                   // RN-20",
    "    {",
    "        case NivelFidelidad.Oro:   return 0.075m;",
    "        case NivelFidelidad.Plata: return 0.050m;",
    "        default:                   return 0.000m;",
    "    }",
    "}",
    "",
    "public NivelFidelidad DeterminarNivel(int idUsuario, DateTime fechaReferencia)",
    "{",
    "    DateTime desde = fechaReferencia.AddMonths(-12);               // RN-19",
    "    int n = RepositorioFidelizacion.Instance",
    "                .ContarComprasConfirmadas(idUsuario, desde, fechaReferencia);",
    "",
    "    if (n >= 7) return NivelFidelidad.Oro;",
    "    if (n >= 3) return NivelFidelidad.Plata;",
    "    return NivelFidelidad.Bronce;",
    "}",
    "",
    "public ResumenDescuento CalcularDescuento(Cliente cliente, Paquete paquete)",
    "{",
    "    // Base: solo entradas. Los servicios quedan fuera por RN-21.",
    "    decimal baseEntradas = paquete.Entradas.Sum(e => e.Precio * e.Cantidad);",
    "    decimal servicios    = paquete.Servicios.Sum(s => s.Precio);",
    "",
    "    // Se usa el nivel persistido, no uno recalculado: RN-16 y RN-22.",
    "    decimal porcentaje = PorcentajePara(cliente.Nivel);",
    "    decimal monto      = Math.Round(baseEntradas * porcentaje, 2);",
    "    decimal subtotalE  = baseEntradas - monto;",
    "    decimal cargo      = Math.Round(subtotalE * 0.15m, 2);",
    "",
    "    return new ResumenDescuento",
    "    {",
    "        NivelAplicado    = cliente.Nivel,",
    "        BaseEntradas     = baseEntradas,",
    "        Porcentaje       = porcentaje,",
    "        MontoDescuento   = monto,",
    "        SubtotalEntradas = subtotalE,",
    "        Servicios        = servicios,",
    "        CargoServicio    = cargo,",
    "        Total            = subtotalE + servicios + cargo",
    "    };",
    "}",
])

d.keep(200)
d.pre([
    "// ---- Capa de datos ----------------------------------------------------",
    "public int ContarComprasConfirmadas(int idUsuario, DateTime desde, DateTime hasta)",
    "{",
    "    const string sql = @\"SELECT COUNT(*) FROM Compras",
    "                          WHERE ID_Usuario  =  @IdUsuario",
    "                            AND Estado      = 'Confirmada'",
    "                            AND FechaCompra >  @Desde",
    "                            AND FechaCompra <= @Hasta\";",
    "",
    "    using (SqlCommand cmd = new SqlCommand(sql, ConexionSql))",
    "    {",
    "        cmd.Parameters.AddWithValue(\"@IdUsuario\", idUsuario);",
    "        cmd.Parameters.AddWithValue(\"@Desde\",     desde);",
    "        cmd.Parameters.AddWithValue(\"@Hasta\",     hasta);",
    "",
    "        ConexionSql.Open();",
    "        int n = Convert.ToInt32(cmd.ExecuteScalar());",
    "        ConexionSql.Close();",
    "        return n;",
    "    }",
    "}",
])

# ==================================================================== SECUENCIA
d.keep(190)
d.h2("Diagrama de secuencia — aplicación del descuento")
d.pre([
    "FrmCompraCliente ──► GestorFidelizacion  : CalcularDescuento(cliente, paquete)",
    "GestorFidelizacion ──► GestorFidelizacion: PorcentajePara(cliente.Nivel)",
    "GestorFidelizacion ──► FrmCompraCliente  : ResumenDescuento",
    "FrmCompraCliente ──► Usuario             : muestra desglose y pide confirmación",
    "",
    "Usuario ──► FrmCompraCliente             : confirma",
    "FrmCompraCliente ──► GestorCompras       : RegistrarCompra(resumen)",
    "GestorCompras ──► RepositorioCompras     : INSERT INTO Compras (…NivelAplicado,",
    "                                                    DescuentoAplicado…)",
    "GestorCompras ──► PasarelaSimulada       : Procesar(total)",
    "PasarelaSimulada ──► GestorCompras       : aprobado",
    "GestorCompras ──► RepositorioCompras     : UPDATE Compras SET Estado='Confirmada'",
    "",
    "GestorCompras ──► GestorFidelizacion     : DeterminarNivel(idUsuario, hoy)",
    "GestorFidelizacion ──► RepositorioFidelizacion : ContarComprasConfirmadas(…)",
    "GestorFidelizacion ──► RepositorioFidelizacion : ActualizarNivel(…)   // rige desde",
    "                                                                      // la compra",
    "                                                                      // siguiente",
])

# ==================================================================== VERIFICACION
d.keep(230)
d.h2("Verificación numérica")
d.p("Cliente de nivel Plata que adquiere dos entradas de Campo a $25.000 cada una y agrega "
    "hospedaje por una noche a $45.000.")
d.table(
    ["Paso", "Concepto", "Importe"],
    [["1", "Base de cálculo: sólo entradas (RN-21)", "$50.000"],
     ["2", "Porcentaje correspondiente al nivel Plata (RN-20)", "5 %"],
     ["3", "Monto del descuento", "$2.500"],
     ["4", "Subtotal de entradas", "$47.500"],
     ["5", "Servicios complementarios, sin descuento", "$45.000"],
     ["6", "Cargo por servicio, 15 % sobre el subtotal de entradas", "$7.125"],
     ["7", "TOTAL A PAGAR", "$99.625"]],
    widths=[36, 310, 105], aligns=["c", "l", "c"])
d.rich([("Consistencia con el modelo económico. ", "b"),
        ("La operación deja a la plataforma una contribución de $9.240: cobra $99.625, remite "
         "$50.000 al organizador y $36.000 al prestador de hospedaje, y afronta $4.085 de comisión "
         "de la pasarela y $300 de emisión. El mismo resultado se obtiene aplicando la fórmula "
         "unitaria de la sección Factibilidad económica: $2.085 por las dos entradas más $7.155 "
         "por el servicio complementario. Ambas secciones son, por lo tanto, coherentes entre sí.",
         "r")])

# ==================================================================== PRUEBAS
d.keep(260)
d.h1("Casos de prueba de la lógica de fidelización")
d.p("Se definen diez casos que cubren los umbrales de nivel, la base de cálculo, la ventana "
    "móvil y cada una de las reglas de congelamiento. El objetivo específico 3 del proyecto exige "
    "registrar al menos cinco casos con distintos perfiles de usuario; el conjunto aquí "
    "presentado lo duplica.")
d.table(
    ["CP", "Condición de entrada", "Resultado esperado", "Regla"],
    [["CP-01", "Cliente con 2 compras confirmadas en 12 meses",
      "Nivel Bronce, descuento 0 %", "RN-19, RN-20"],
     ["CP-02", "Cliente con 3 compras confirmadas", "Nivel Plata, descuento 5 %", "RN-19, RN-20"],
     ["CP-03", "Cliente con 6 compras confirmadas", "Nivel Plata; no asciende", "RN-19"],
     ["CP-04", "Cliente con 7 compras confirmadas", "Nivel Oro, descuento 7,5 %", "RN-19, RN-20"],
     ["CP-05", "Paquete Oro con entradas por $40.000 y hospedaje por $30.000",
      "Descuento de $3.000; el hospedaje no se descuenta", "RN-21"],
     ["CP-06", "Cliente con 7 compras, una de ellas hace 13 meses",
      "Nivel Plata: la compra antigua queda fuera de la ventana", "RN-19"],
     ["CP-07", "Cambio de tarifa del evento con una compra ya iniciada",
      "Se factura el precio congelado al iniciar", "RN-16"],
     ["CP-08", "Cliente con 6 compras que realiza la séptima",
      "La séptima se cobra con 5 %; el nivel Oro rige desde la octava", "RN-22"],
     ["CP-09", "Cliente Oro al que se le reembolsa una compra",
      "El contador baja a 6; el nivel se mantiene hasta la reevaluación", "RN-23, RN-24"],
     ["CP-10", "Cliente Oro con suscripción activa",
      "Descuento 7,5 %; la suscripción no suma porcentaje", "RN-37"]],
    widths=[32, 148, 178, 93], aligns=["c", "l", "l", "c"])

d.keep(250)
d.h2("Implementación de las pruebas")
d.p("Las pruebas se codifican como métodos ejecutables, de modo que puedan reincorporarse al "
    "conjunto de pruebas de caja blanca del sistema. Los casos CP-01 a CP-04 se resuelven con una "
    "prueba parametrizada, por tratarse de la misma condición evaluada en distintos puntos de "
    "corte.")
d.pre([
    "[TestClass]",
    "public class PruebasFidelizacion",
    "{",
    "    private readonly GestorFidelizacion g = GestorFidelizacion.Instance;",
    "",
    "    // ---- CP-01 a CP-04 : umbrales de nivel y porcentajes -------------",
    "    [DataTestMethod]",
    "    [DataRow(2, NivelFidelidad.Bronce, 0.000)]     // CP-01",
    "    [DataRow(3, NivelFidelidad.Plata,  0.050)]     // CP-02",
    "    [DataRow(6, NivelFidelidad.Plata,  0.050)]     // CP-03",
    "    [DataRow(7, NivelFidelidad.Oro,    0.075)]     // CP-04",
    "    public void Umbrales_AsignanNivelYPorcentaje(int compras,",
    "                                                 NivelFidelidad esperado,",
    "                                                 double porcentaje)",
    "    {",
    "        var cliente = Fake.ClienteCon(compras, dentroDeMeses: 12);",
    "        Assert.AreEqual(esperado, g.DeterminarNivel(cliente.ID, Hoy));",
    "        Assert.AreEqual((decimal)porcentaje, g.PorcentajePara(esperado));",
    "    }",
    "",
    "    // ---- CP-05 : la base excluye los servicios -----------------------",
    "    [TestMethod]",
    "    public void Descuento_NoAlcanzaALosServiciosComplementarios()",
    "    {",
    "        var cliente = Fake.ClienteNivel(NivelFidelidad.Oro);",
    "        var paquete = Fake.Paquete(entradas: 40000m, servicios: 30000m);",
    "",
    "        var r = g.CalcularDescuento(cliente, paquete);",
    "",
    "        Assert.AreEqual(40000m, r.BaseEntradas);",
    "        Assert.AreEqual( 3000m, r.MontoDescuento);",
    "        Assert.AreEqual(30000m, r.Servicios);",
    "    }",
    "",
    "    // ---- CP-06 : la ventana movil excluye compras antiguas -----------",
    "    [TestMethod]",
    "    public void VentanaMovil_ExcluyeComprasDeMasDeDoceMeses()",
    "    {",
    "        var cliente = Fake.ClienteCon(6, dentroDeMeses: 12)",
    "                          .MasUnaCompraHace(meses: 13);",
    "",
    "        Assert.AreEqual(NivelFidelidad.Plata, g.DeterminarNivel(cliente.ID, Hoy));",
    "    }",
])

d.keep(250)
d.pre([
    "    // ---- CP-07 : el precio queda congelado al iniciar la compra ------",
    "    [TestMethod]",
    "    public void Precio_SeCongelaAlIniciarLaCompra()",
    "    {",
    "        var paquete = Fake.Paquete(entradas: 20000m, servicios: 0m);",
    "        var compra  = GestorCompras.Instance.Iniciar(paquete);",
    "",
    "        Fake.CambiarTarifaDelEvento(paquete.Evento, nuevoPrecio: 30000m);",
    "",
    "        Assert.AreEqual(20000m, compra.BaseEntradas);",
    "    }",
    "",
    "    // ---- CP-08 : el ascenso no beneficia a la compra que lo genera ---",
    "    [TestMethod]",
    "    public void Ascenso_RigeReciénDesdeLaCompraSiguiente()",
    "    {",
    "        var cliente = Fake.ClienteCon(6, dentroDeMeses: 12);   // Plata",
    "",
    "        var septima = g.CalcularDescuento(cliente, Fake.Paquete(10000m, 0m));",
    "        Assert.AreEqual(0.050m, septima.Porcentaje);           // aun Plata",
    "",
    "        GestorCompras.Instance.Confirmar(septima);",
    "        var octava = g.CalcularDescuento(cliente, Fake.Paquete(10000m, 0m));",
    "        Assert.AreEqual(0.075m, octava.Porcentaje);            // ahora Oro",
    "    }",
    "",
    "    // ---- CP-09 : reembolso baja el contador, no el nivel -------------",
    "    [TestMethod]",
    "    public void Reembolso_BajaElContadorPeroNoElNivelHastaLaReevaluacion()",
    "    {",
    "        var cliente = Fake.ClienteCon(7, dentroDeMeses: 12);   // Oro",
    "        GestorCompras.Instance.Reembolsar(cliente.UltimaCompra);",
    "",
    "        Assert.AreEqual(6, Repo.ContarComprasConfirmadas(cliente.ID,",
    "                                                        Hoy.AddMonths(-12), Hoy));",
    "        Assert.AreEqual(NivelFidelidad.Oro, cliente.Nivel);    // sigue Oro",
    "",
    "        g.ReevaluarNivel(cliente, PrimerDiaDelMesSiguiente);",
    "        Assert.AreEqual(NivelFidelidad.Plata, cliente.Nivel);  // recién ahora baja",
    "    }",
    "",
    "    // ---- CP-10 : la suscripcion no acumula porcentaje ----------------",
    "    [TestMethod]",
    "    public void Suscripcion_NoSumaPorcentajeAlDescuentoPorNivel()",
    "    {",
    "        var cliente = Fake.ClienteNivel(NivelFidelidad.Oro).ConSuscripcionActiva();",
    "",
    "        var r = g.CalcularDescuento(cliente, Fake.Paquete(100000m, 0m));",
    "",
    "        Assert.AreEqual(0.075m, r.Porcentaje);",
    "        Assert.AreEqual( 7500m, r.MontoDescuento);",
    "    }",
    "}",
])

d.keep(180)
d.h2("Criterios de aceptación")
d.table(
    ["#", "Criterio verificable", "Estado"],
    [["D-1", "El conteo de compras agrupa por operación y no por entrada individual.",
      "Cumple"],
     ["D-2", "La ventana móvil de doce meses excluye correctamente las compras anteriores.",
      "Cumple (CP-06)"],
     ["D-3", "El descuento se calcula exclusivamente sobre el valor de las entradas.",
      "Cumple (CP-05)"],
     ["D-4", "Una compra que provoca un ascenso de nivel no se beneficia a sí misma.",
      "Cumple (CP-08)"],
     ["D-5", "El nivel desciende únicamente en la reevaluación mensual.", "Cumple (CP-09)"],
     ["D-6", "La suscripción no incorpora un segundo porcentaje de descuento.",
      "Cumple (CP-10)"],
     ["D-7", "Los diez casos de prueba se ejecutan sin fallos sobre el juego de datos de "
             "prueba.", "Cumple"]],
    widths=[32, 338, 81], aligns=["c", "l", "c"])

d.save()
print("OK ->", OUT)
