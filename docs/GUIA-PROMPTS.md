# Guía de prompts — construcción de EventAura con Claude Code

## Cómo usar esta guía

`CLAUDE.md` se carga solo en cada conversación. Los archivos de `docs/` los lee el agente
cuando se los nombrás. **Nunca pegues el PDF ni transcribas reglas en el prompt**: citá el
identificador (`RN-19`) y el archivo, y dejá que los lea.

| Enfoque | Costo aproximado |
|---|---|
| Pegar la documentación en cada prompt | ~600.000 tokens |
| Contexto persistente + citas por identificador | ~40.000 tokens |

**Una sección = una conversación nueva.** Al terminar cada una, cerrá el chat y abrí otro.
Arrastrar el historial de la sección 3 hasta la 9 multiplica el costo sin aportar nada:
el estado real del proyecto está en los archivos, no en la conversación.

**Compilá entre secciones.** Si la 6 rompe algo, tenés que saber que la 5 estaba sana.

---

## Anatomía de un prompt

```
1. OBJETIVO       una línea, en imperativo
2. ARCHIVOS       cuáles crear y cuáles modificar, explícito
3. CONTEXTO       "aplicá RN-xx a RN-yy de docs/02-reglas-negocio.md"
4. CONTRATO       las firmas que esperás
5. RESTRICCIONES  qué NO tocar
6. VERIFICACIÓN   qué tiene que compilar y qué caso probar
```

Los puntos 5 y 6 son los que más ahorran. Sin restricciones el agente refactoriza de más;
sin verificación no se da cuenta de que se equivocó.

---

## Orden de construcción

| Bloque | # | Sección | Deja funcionando |
|---|---|---|---|
| Contexto | 0 | Preparación | — |
| Cimientos | 1 | Limpieza y esquema de BD | Base creada con datos semilla |
| | 2 | Entidades y enums | Capa `Entidades` completa |
| | 3 | Capa de datos, sesión y seguridad | Login con las reglas RN-01 a RN-05 |
| Flujo core | 4 | Gestión de eventos (RF-12) | Alta y baja de eventos con sectores |
| | 5 | Búsqueda, selección y armado de paquete | Paquete armado en pantalla |
| | 6 | Fidelización y descuento (RF-08) | Descuento calculado y mostrado |
| | 7 | Reserva, pago, confirmación y ticket | Compra completa de punta a punta |
| Complementos | 8 | Suscripciones, seguro, historial, lista de espera | RF-13, RF-10, CU-03 |
| | 9 | Reportes y exportación PDF | RF-11 y anexo A02 |
| Cierre | 10 | Pruebas CP-01 a CP-10 | Suite verde |

Las secciones 5, 6 y 7 son el corazón. Si el tiempo se acorta, recortá la 8 y la 9 —
son Should y Could en la priorización MoSCoW.

---

# Sección 0 — Preparación

No genera código. Verificá que existan `CLAUDE.md` y los cuatro archivos de `docs/`.
Abrí Claude Code en la raíz del repositorio y confirmá con:

```
Leé CLAUDE.md, docs/01-requerimientos.md y docs/02-reglas-negocio.md.
Respondeme solo esto, sin escribir código:

1. ¿Sobre qué framework y qué base de datos corre el proyecto?
2. ¿Cuántos requerimientos funcionales hay y cuál es el RF-12?
3. ¿Qué dice exactamente RN-19?
4. ¿Qué pasó con RN-27?
```

**Respuestas correctas.** Si alguna falla, el agente no abrió los archivos de `docs/`:

| | Respuesta esperada |
|---|---|
| 1 | .NET Framework **4.8** con WinForms, sobre **SQL Server** |
| 2 | **13** requerimientos. RF-12 es **Gestión de eventos**, Must, 8 puntos |
| 3 | El nivel se determina por compras confirmadas en **ventana móvil de 12 meses**: Bronce 0–2, Plata 3–6, Oro 7+ |
| 4 | **Dada de baja** por inviabilidad económica. Permitía acumular descuentos hasta 20 %. Su número no se reasigna |

No sigas a la Sección 1 hasta que las cuatro salgan bien. Si el agente no lee `docs/`,
va a **inventar** el contenido de las reglas en vez de aplicarlas, y los cálculos no van
a cerrar.

---

# Sección 1 — Limpieza del repositorio y esquema de base de datos

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Dejar la solución con las 4 capas de CLAUDE.md y crear el esquema completo de la base.

ARCHIVOS
- Eliminar las carpetas Entidades/, Negocio/ y Visual_/ (código muerto, fuera de la solución).
- Crear el proyecto Datos/Datos.csproj (biblioteca de clases, .NET Framework 4.8),
  agregarlo a SistemaEntradas.sln y referenciarlo desde Repo/Repositorio.csproj.
- Crear Datos/Conexion.cs con un singleton que lea la cadena de App.config.
- Crear BaseDatos/01-esquema.sql y BaseDatos/02-semilla.sql.

CONTEXTO
Ejecutá el DDL de docs/03-esquema-datos.md tal como está, sin cambiar nombres de tablas
ni de columnas. Los datos semilla son los del final de ese mismo archivo. Los 13 registros
de la tabla Parametros salen de docs/02-reglas-negocio.md.

RESTRICCIONES
- No toques Visual/ ni Repo/ en esta sección, más allá de agregar la referencia al proyecto.
- No borres Datos/RepositorioEntradas.cs ni Datos/RepositorioEventos.cs: reescribilos.

VERIFICACIÓN
La solución compila. Los dos scripts se ejecutan sobre una base vacía sin errores y en ese
orden.
```

---

# Sección 2 — Entidades y enums

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Completar la capa Entidades con las entidades del esquema nuevo.

ARCHIVOS
En Entidade/: NivelFidelidad.cs, EstadoCompra.cs, EstadoEntrada.cs, Compra.cs, Servicio.cs,
CompraServicio.cs, Seguro.cs, Suscripcion.cs, ListaEspera.cs, Parametro.cs, Paquete.cs,
ResumenDescuento.cs. Ampliar Cliente.cs y Evento.cs.

CONTEXTO
Las tablas están en docs/03-esquema-datos.md. Cliente suma Nivel, FechaUltimaEvaluacion y
ComprasUltimos12Meses. Evento suma Categoria y Estado.

Paquete es una entidad en memoria, sin tabla: agrupa las entradas seleccionadas y los
servicios elegidos antes de confirmar la compra.

ResumenDescuento es el DTO que Negocio devuelve a Visual con el desglose:
NivelAplicado, BaseEntradas, Porcentaje, MontoDescuento, SubtotalEntradas, Servicios,
CargoServicio y Total.

RESTRICCIONES
- POCOs con propiedades automáticas. Sin lógica, sin validaciones, sin acceso a datos.
- El proyecto Entidades no referencia a ningún otro.
- Importes en decimal.

VERIFICACIÓN
Compila. Cada tabla del DDL tiene su entidad y cada columna su propiedad.
```

---

# Sección 3 — Capa de datos, sesión y seguridad

> Reescrita tras auditar el código real. El estado actual del repositorio está
> verificado, no deducido. Ver el bloque ESTADO ACTUAL del prompt.

El prompt completo se entregó en la conversación. Puntos que lo diferencian del
resto de las secciones:

- **Paso previo**: `Visual.csproj` no referencia a `Datos`. Sin esa referencia el
  código compila y falla al ejecutar.
- **Namespace**: `GestorUsuarios` está en `Repositorio`, no en `Negocio`. Migra y
  arrastra el `using` de tres formularios.
- **Arreglo de navegación**: el botón «crear cuenta» abre `frmAgregarEvento` en vez
  de `frmCrearUsuario`, que está completo y nunca se instancia. RF-01 hoy no es
  alcanzable desde la interfaz.
- **RN-02 nunca se implementó**: `ValidarUsuario` valida una regla inventada.
- **RN-05 incompleta**: `AgregarUsuario` no inserta en `Cliente` ni `Propietario`.
- **`ConexionSql` sobrevive**: los formularios de Cliente y Propietario dependen de
  él hasta su propia sección.

# Sección 4 — Gestión de eventos (RF-12)

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Implementar el alta, modificación y baja de eventos con sus sectores.

ARCHIVOS
- Datos/RepositorioEventos.cs (reescribir), Datos/RepositorioSectores.cs.
- Repo/GestorEventos.cs (reescribir sacándole el SQL).
- Visual/Propietario/: frmAgregarEvento, frmAgregarSector, frmAdministrarEvento.

CONTEXTO
RF-12 de docs/01-requerimientos.md. Aplicá RN-06, RN-07, RN-09 y RN-11 de
docs/02-reglas-negocio.md.

CONTRATO
    int  GestorEventos.Publicar(Evento e)                  // valida RN-06
    bool GestorEventos.Suspender(int idEvento)             // RN-11
    int  GestorEventos.CapacidadTotal(int idEvento)        // RN-07
    List<Evento> GestorEventos.Vigentes()                  // RN-09

RESTRICCIONES
- Los formularios existentes ya tienen su diseño: conservá los controles y solo cambiá
  el código detrás.
- El intento de publicar sin sectores lanza una excepción de dominio que el formulario
  atrapa y muestra.

VERIFICACIÓN
Compila. No se puede publicar un evento sin sector con capacidad > 0, ni eliminar uno con
entradas vendidas.
```

---

# Sección 5 — Búsqueda, selección y armado de paquete

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Implementar el requerimiento core: armar un paquete con entradas y servicios.

ARCHIVOS
- Datos/: RepositorioServicios.cs.
- Repo/: GestorPaquetes.cs, GestorBusqueda.cs.
- Visual/Cliente/: frmEventosCliente (búsqueda), frmArmarPaquete (nuevo, reemplaza la
  lógica actual de FrmCompraCliente).

CONTEXTO
RF-03, RF-04 y RF-05 de docs/01-requerimientos.md.
Aplicá RN-08, RN-09, RN-12, RN-13, RN-14 y RN-33 de docs/02-reglas-negocio.md.
La pantalla y sus controles están descriptos en el Instructivo del requerimiento core,
páginas 50 a 51 de Documentacion/EventAura_v2.pdf; la lista de controles esperados es:
lblEvento, lblSectorA/B/C, lblPrecioA/B/C, NumPrimero/NumSegundo/NumTercero,
chkHospedaje, chkTraslado, chkMerch, chkVIP, chkSeguro, lblNivel, pnlResumen,
btnContinuar, btnVolver.

Implementá además los hallazgos H-03 y H-06 de docs/04-casos-prueba.md.

CONTRATO
    List<Evento>   GestorBusqueda.Buscar(string texto, string categoria, DateTime? desde)
    Paquete        GestorPaquetes.Nuevo(int idEvento)
    void           GestorPaquetes.AgregarEntradas(Paquete p, int idSector, int cantidad)
    void           GestorPaquetes.AgregarServicio(Paquete p, int idServicio)
    void           GestorPaquetes.Validar(Paquete p)       // RN-12, RN-13, RN-14

RESTRICCIONES
- Todavía no calcules descuentos: eso es la sección 6. Mostrá el subtotal sin descuento.
- No implementes el pago: eso es la sección 7.
- El botón Continuar queda deshabilitado mientras no haya al menos una entrada.

VERIFICACIÓN
Compila. Se puede armar un paquete con 2 entradas y un hospedaje, y el sistema impide
superar las 6 entradas o agregar un servicio no habilitado para ese evento.
```

---

# Sección 6 — Fidelización y descuento (RF-08)

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Implementar el nivel de fidelidad y el cálculo del descuento.

ARCHIVOS
- Datos/RepositorioFidelizacion.cs.
- Repo/GestorFidelizacion.cs.
- Visual/Cliente/frmArmarPaquete.cs: mostrar el desglose en pnlResumen.

CONTEXTO
RF-08 de docs/01-requerimientos.md.
Aplicá RN-19 a RN-24 y RN-37 de docs/02-reglas-negocio.md, y respetá el orden de cálculo
descripto en ese archivo. El nivel se LEE de la entidad, no se recalcula al comprar:
eso es lo que materializa RN-16 y RN-22.

La consulta de la ventana móvil está en docs/03-esquema-datos.md.
Implementá los hallazgos H-01 y H-05 de docs/04-casos-prueba.md.

CONTRATO
    NivelFidelidad   GestorFidelizacion.DeterminarNivel(int idUsuario, DateTime fecha)
    decimal          GestorFidelizacion.PorcentajePara(NivelFidelidad nivel)
    ResumenDescuento GestorFidelizacion.CalcularDescuento(Cliente c, Paquete p)
    void             GestorFidelizacion.ReevaluarNivel(Cliente c, DateTime fecha)

RESTRICCIONES
- Los umbrales y porcentajes salen de la tabla Parametros, no del código.
- La suscripción NO agrega un segundo porcentaje (RN-37).
- El descuento no alcanza a servicios ni al cargo por servicio (RN-21).

VERIFICACIÓN
Compila. Con un cliente Plata, 2 entradas de $25.000 y hospedaje de $45.000, el total debe
dar exactamente $99.625 (ver la verificación numérica de docs/04-casos-prueba.md).
```

---

# Sección 7 — Reserva, pago, confirmación y ticket

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Cerrar el flujo de compra: reservar cupo, simular el pago, confirmar y emitir el ticket.

ARCHIVOS
- Datos/: RepositorioCompras.cs.
- Repo/: GestorCompras.cs, PasarelaSimulada.cs, GeneradorQR.cs.
- Visual/Cliente/: frmPago (nuevo), frmConfirmacion (nuevo).
- Reutilizar el método GenerarPDF que ya existe en Visual/Cliente/FrmCompraCliente.cs.

CONTEXTO
RF-06 y RF-07 de docs/01-requerimientos.md.
Aplicá RN-15 a RN-18 y RN-30 a RN-32 de docs/02-reglas-negocio.md.
Atención al conflicto RN-08 vs RN-15 resuelto al final de ese archivo: el control de
capacidad computa vendidas MÁS reservadas vigentes.

La pasarela es simulada: devuelve aprobado o rechazado según una regla fija y no toca
la red. Guardá solo los últimos 4 dígitos y un token ficticio (RN-30).

Implementá el hallazgo H-02 de docs/04-casos-prueba.md: temporizador visible de la reserva.

CONTRATO
    Compra GestorCompras.Iniciar(Paquete p, ResumenDescuento r)   // reserva, RN-15
    bool   GestorCompras.Confirmar(Compra c, string token)        // RN-17
    void   GestorCompras.Liberar(Compra c)                        // RN-32 y vencimiento
    void   GestorCompras.Reembolsar(Compra c)                     // RN-24

RESTRICCIONES
- Al confirmar, actualizá el contador de compras y recalculá el nivel, pero el nivel nuevo
  rige desde la operación siguiente (RN-22).
- Un QR por entrada, único (RN-18).

VERIFICACIÓN
Compila. Se completa una compra de punta a punta y se genera el PDF del ticket con QR.
Si pasan 10 minutos sin confirmar, el cupo vuelve a estar disponible.
```

---

# Sección 8 — Suscripciones, seguro, historial y lista de espera

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Completar las funcionalidades complementarias.

ARCHIVOS
- Datos/: RepositorioSuscripciones.cs, RepositorioListaEspera.cs.
- Repo/: GestorSuscripciones.cs, GestorListaEspera.cs.
- Visual/Cliente/: frmSuscripcion (nuevo), frmHistorialCliente (reescribir sacándole el SQL).
- Visual/Propietario/: frmListasEspera (nuevo).

CONTEXTO
RF-13 y RF-10 de docs/01-requerimientos.md. Casos de uso CU-03, CU-06 y CU-07.
Aplicá RN-10, RN-25, RN-26, RN-28, RN-29, RN-35, RN-36 y RN-37.

El seguro se contrata únicamente durante la compra (RN-28), así que su alta ocurre dentro
del flujo de la sección 7; acá solo se agrega la consulta y la devolución.

Implementá el hallazgo H-04 de docs/04-casos-prueba.md.

RESTRICCIONES
- La suscripción no otorga descuento porcentual (RN-37). Sus beneficios son seguro
  bonificado, preventa y prioridad en lista de espera.
- La lista de espera es solo consulta para el Administrador en esta versión.

VERIFICACIÓN
Compila. Un cliente no puede tener dos suscripciones activas ni anotarse dos veces en la
lista de espera del mismo evento.
```

---

# Sección 9 — Reportes y exportación PDF

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Implementar los cinco reportes de gestión y su exportación a PDF.

ARCHIVOS
- Datos/RepositorioReportes.cs.
- Repo/: Reporte.cs (abstracta), las 5 clases derivadas, GestorReportes.cs,
  IExportador.cs, ExportadorPDF.cs.
- Visual/Propietario/frmReportes.cs (nuevo).

CONTEXTO
RF-11 de docs/01-requerimientos.md y RN-34.
El anexo A02 del PDF, páginas 91 a 98, define los cinco reportes (RPT-01 a RPT-05), sus
fórmulas, el SQL de RPT-01, el diagrama de clases del módulo y la estructura del PDF de
salida. Leelo solo si necesitás el detalle de un indicador.

Los cinco reportes: ventas por evento, ranking de paquetes, usuarios por nivel, adopción
de seguros y suscripciones, demanda insatisfecha.

CONTRATO
    Reporte   GestorReportes.Crear(string codigo, DateTime desde, DateTime hasta)
    DataTable GestorReportes.Ejecutar(Reporte r)
    string    GestorReportes.Exportar(Reporte r, IExportador exp, string ruta)

RESTRICCIONES
- El PDF se genera con iText, nunca con impresora virtual: lo exige el punto A02 de la
  Carpeta Modelo.
- Agregar un formato de exportación nuevo no debe obligar a tocar las clases de reporte.
- Solo el perfil Administrador accede (RN-34).

VERIFICACIÓN
Compila. Los cinco reportes se generan y al menos uno se exporta a PDF con encabezado,
tabla, totales y pie con numeración.
```

---

# Sección 10 — Pruebas

```
ANTES DE EMPEZAR
Leé CLAUDE.md y cada archivo de docs/ citado en CONTEXTO. No deduzcas qué dice una
regla ni un requerimiento: abrí el archivo y leelo. Si una regla no está en los
archivos, preguntá; no la inventes.

OBJETIVO
Implementar los diez casos de prueba de la lógica de fidelización.

ARCHIVOS
- Crear el proyecto Pruebas/Pruebas.csproj (MSTest) y agregarlo a la solución.
- Pruebas/PruebasFidelizacion.cs y Pruebas/Fake.cs (constructor de datos de prueba).

CONTEXTO
Los diez casos están en docs/04-casos-prueba.md con su condición de entrada, resultado
esperado y regla verificada. CP-01 a CP-04 van en un único [DataTestMethod] con [DataRow].

CONTRATO
    Fake.ClienteCon(int compras, int dentroDeMeses)
    Fake.ClienteNivel(NivelFidelidad nivel)
    Fake.Paquete(decimal entradas, decimal servicios)
    Fake.CambiarTarifaDelEvento(Evento e, decimal nuevoPrecio)

RESTRICCIONES
- Las pruebas corren contra una base de datos de prueba separada, no contra la de
  desarrollo.
- Fake crea y limpia sus propios datos en cada corrida.

VERIFICACIÓN
Los diez casos pasan. El caso de la verificación numérica da exactamente $99.625.
```

---

## Qué hacer cuando algo sale mal

| Síntoma | Causa habitual | Qué hacer |
|---|---|---|
| El agente reescribe archivos que no pediste | Faltó la sección RESTRICCIONES | Agregá «no modifiques nada fuera de los archivos listados» |
| Inventa nombres de tablas o columnas | No leyó el esquema | Pedile explícitamente que lea `docs/03-esquema-datos.md` antes de escribir |
| Pone SQL en un formulario | Perdió la regla de capas | Recordale la regla 3 de `CLAUDE.md` |
| Hardcodea 0.075 o 7 | No usó Parametros | Citá la tabla de parámetros configurables de `docs/02-reglas-negocio.md` |
| Los totales no cierran | Aplicó el descuento sobre los servicios | RN-21: la base son solo las entradas |
| La conversación se vuelve lenta y cara | Historial demasiado largo | Cerrá el chat y abrí uno nuevo para la sección siguiente |

## Al terminar

Actualizá el historial de revisión del informe con las fechas reales de construcción y
sacá las cuatro capturas de pantalla que faltan en el Instructivo del requerimiento core,
páginas 52 y 53 del PDF.
