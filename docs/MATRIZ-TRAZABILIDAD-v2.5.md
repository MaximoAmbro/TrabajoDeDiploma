# Actualización de la matriz de trazabilidad — EventAura v2.5

Texto listo para reemplazar en `Documentacion/EventAura_v2.pdf` (versión 2.4).

Cifras al 04/11/2026, con las pruebas del commit `5eabe41`: **42 casos y 48 ejecuciones,
47 correctas y 1 omitida** (CP-34). Las cuentas usan **38 reglas vigentes**, porque esta
versión incorpora RN-38 y RN-39 al documento.

**Cómo copiar al Word sin perder las tablas.** Abrí este archivo en VS Code y la vista previa
(`Ctrl+Shift+V`). Seleccioná desde la vista previa, copiá y pegá en Word. Las tablas llegan
como tablas.

**Convenciones.**
- **Reemplazar**: el texto viejo se cita tal como figura en el PDF y debajo va el nuevo.
- **Agregar**: el texto no existía en el documento.
- Los números de página son los de la versión 2.4. Si el texto nuevo corre contenido, el
  índice (páginas 3 y 4) se vuelve a generar.

**CP-34 no se cuenta.** Es la prueba de RN-15 (reserva vencida) y quedó omitida. La causa es
la preparación de la prueba, no la aplicación: ver «Segunda tanda» en
[TAREAS-CODEX-PRUEBAS.md](TAREAS-CODEX-PRUEBAS.md). Figura en la matriz con asterisco y no
suma cobertura. Si Codex la corrige antes de la entrega, aplicar la sección 12.

---

## 0. Casos de prueba: de dónde salen los 42

| Grupo | Métodos | Ejecuciones | ID |
|---|---|---|---|
| Lógica de descuento (versión 2.4) | 7 | 10 | CP-01 a CP-10 |
| Ya existían en el código, sin ID en el documento | 2 | 2 | CP-11, CP-12 |
| Interfaz, pruebas manuales del 03 y 04/11/2026 | 13 | 19 | CP-13 a CP-25 |
| Gestores e interfaz: completar RF y hallazgos | 17 | 17 | CP-26 a CP-42 |
| **Total** | **39** | **48** | **42 casos** |

El ID se asigna por caso, no por ejecución. CP-01 a CP-04 son un solo método con cuatro filas
de datos; CP-15, CP-20 y CP-25 corren con 4, 2 y 3 variantes.

## 1. Resumen de lo que cambia

| Pág. | Sección | Cambio |
|---|---|---|
| 6 | Historial de revisión | Dos filas nuevas, versión 2.5 |
| 7 | Resumen ejecutivo, «Qué contiene este informe» | 36 → 38 reglas |
| 8 | Resumen ejecutivo, limitaciones | Cobertura 22,2 % → 68,4 % |
| 26 | Reglas: eventos, sectores y disponibilidad | Fila nueva RN-38 |
| 27 | Reglas: suscripciones y seguros | Fila nueva RN-39 |
| 36 | Priorización de RF | RF-09: Could → Won't, más una nota |
| 37 | Requerimientos excluidos | Fila nueva W-07 |
| 38 | Orden de implementación | Fase 8 sin RF-09 |
| 106 | Criterio de construcción y nomenclatura | 36 → 38 reglas; 10 → 42 casos |
| 107 | Matriz principal | Columna de casos; RN-38, RN-39; RF-09 Won't |
| 108 | Cobertura | Tablas recalculadas y catálogo CP-11 a CP-42 |
| 109 | Brechas detectadas | G-02 a G-04 actualizadas; G-05 nueva |
| 110 | Conclusiones y limitaciones | 36 → 38 reglas; cobertura |
| 111 | Trabajo futuro | Puntos 2 y 4 |

---

## 2. Página 6 — Historial de revisión

**Agregar** dos filas al final de la tabla:

| Fecha | Versión | Autor | Descripción del cambio |
|---|---|---|---|
| «fecha» | 2.5 | Riggio, Juan Cruz | Se incorporan las reglas RN-38 (capacidad del local) y RN-39 (preventa para suscriptores), que el prototipo ya aplicaba sin estar documentadas (brecha G-05). Las reglas vigentes pasan de 36 a 38. |
| «fecha» | 2.5 | Riggio, Juan Cruz | Actualización de la matriz de trazabilidad: casos de prueba CP-11 a CP-42 y cobertura recalculada sobre 38 reglas. RF-09 se reclasifica de Could a Won't have, según la regla de repriorización (brecha G-03). Brecha G-04 resuelta; G-02 resuelta en requerimientos y abierta en reglas. |

> La página 38 exige que todo cambio de categoría MoSCoW quede en el historial «indicando
> fecha, requerimiento afectado, categoría anterior, categoría nueva y motivo». La segunda
> fila lo cumple.

---

## 3. Páginas 7 y 8 — Resumen ejecutivo

### Página 7 — tabla «Qué contiene este informe», fila «Reglas de negocio»

**Reemplazar:**

> Treinta y seis reglas vigentes que fijan las condiciones bajo las cuales opera el sistema

**Por:**

> Treinta y ocho reglas vigentes que fijan las condiciones bajo las cuales opera el sistema

### Página 8 — «Limitaciones declaradas»

**Reemplazar:**

> Y la cobertura de verificación alcanza al 22,2 % de las reglas de negocio, concentrada en
> la lógica de fidelización.

**Por:**

> Y la cobertura de verificación alcanza al 68,4 % de las reglas de negocio: doce de las
> treinta y ocho no tienen todavía un caso de prueba automatizado.

---

## 4. Páginas 26 y 27 — Reglas nuevas

Las dos reglas respetan el criterio de la página 25: son verificables, atómicas y no
mencionan pantallas, controles ni tablas.

### Página 26 — tabla «Eventos, sectores y disponibilidad», después de RN-11

**Agregar** la fila:

| RN-38 | La suma de las capacidades de los sectores de un evento no puede superar la capacidad del local donde se realiza. Se verifica al agregar o modificar un sector. | E | RF-12 / CU-02 |
|---|---|---|---|

### Página 27 — tabla «Suscripciones y seguros de entrada», después de RN-37

**Agregar** la fila:

| RN-39 | Mientras no haya terminado el período de preventa de un evento, sólo un cliente con suscripción activa puede iniciar una compra de ese evento; el evento sigue visible en el listado para todos los clientes. Un evento sin período de preventa no tiene esta restricción. Es el beneficio de preventa que enuncia RN-37. | A | RF-13 / CU-01 |
|---|---|---|---|

---

## 5. Páginas 36 a 38 — Priorización (por RF-09)

### Página 36 — tabla «Priorización de requerimientos funcionales», fila RF-09

**Reemplazar** la fila:

| RF-09 | Recomendaciones personalizadas | Could | 8 | El más costoso de los no obligatorios y sin objetivo específico asociado. |
|---|---|---|---|---|

**Por:**

| RF-09 | Recomendaciones personalizadas | Won't (desde v2.5) | 8 | Era Could: el más costoso de los no obligatorios y sin objetivo específico asociado. Se excluye en la Fase 8 por la regla de repriorización (brecha G-03). |
|---|---|---|---|---|

### Página 36 — debajo de «Observación sobre el desvío»

La tabla «Distribución del esfuerzo» se conserva como estimación aprobada.

**Agregar** a continuación de «Observación sobre el desvío»:

> **Nota de la versión 2.5.** RF-09 se reclasificó como Won't have en la Fase 8. Sus ocho
> puntos se liberan sin afectar a ningún requerimiento Must, como prevé la mitigación
> descrita: la categoría Could funcionó como amortiguador. La tabla de distribución se
> conserva como la estimación aprobada.

### Página 37 — tabla «Requerimientos excluidos — categoría Won't have»

**Agregar** la fila:

| W-07 | Recomendaciones personalizadas (RF-09) | Reclasificado desde Could en la versión 2.5. No responde a ningún objetivo específico ni tiene caso de uso, y es el requerimiento no obligatorio más costoso (8 puntos). |
|---|---|---|

### Página 38 — tabla «Orden de implementación derivado», fila de la Fase 8

**Reemplazar** en la columna Requerimientos:

> RF-09 y RF-10, según margen disponible

**Por:**

> RF-10 (RF-09 pasó a Won't have en la versión 2.5)

---

## 6. Página 106 — Criterio de construcción y nomenclatura

### Criterio de construcción

**Reemplazar:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y seis
> reglas, los ocho casos de uso y los diez casos de prueba resulta ilegible en formato A4

**Por:**

> Una grilla que cruce los seis objetivos con los trece requerimientos, las treinta y ocho
> reglas, los ocho casos de uso y los cuarenta y dos casos de prueba resulta ilegible en
> formato A4

### Tabla «Nomenclatura de los ejes»

**Reemplazar** las dos filas:

| Reglas de negocio | RN-xx | 36 | Reglas de negocio (RN-27 dada de baja) |
|---|---|---|---|
| Casos de prueba | CP-xx | 10 | Desarrollo de la lógica de descuento por nivel |

**Por:**

| Reglas de negocio | RN-xx | 38 | Reglas de negocio (RN-27 dada de baja) |
|---|---|---|---|
| Casos de prueba | CP-xx | 42 | CP-01 a CP-10: Desarrollo de la lógica de descuento por nivel. CP-11 a CP-42: Matriz de trazabilidad, catálogo de casos incorporados |

---

## 7. Página 107 — Matriz principal

**Reemplazar la tabla completa.** Cambian:
- la columna «Casos de prueba»;
- las reglas de RF-12 (se suma RN-38) y de RF-13 (se suma RN-39);
- la categoría de RF-09.

| RF | OE | MoSCoW | Reglas de negocio | CU | Casos de prueba | Hall. |
|---|---|---|---|---|---|---|
| RF-01 | OE-2 | Must | RN-01, RN-02, RN-03 | CU-04 | CP-14, CP-15, CP-26 | — |
| RF-02 | OE-2 | Must | RN-04, RN-05 | CU-05 | CP-27, CP-28, CP-29 | — |
| RF-03 | OE-2 | Must | RN-09 | CU-01 | CP-16, CP-17, CP-18, CP-30 | — |
| RF-04 | OE-2 | Must | RN-08, RN-12, RN-33 | CU-01 | CP-20, CP-31, CP-40 | H-03 |
| RF-05 | OE-2 | Must | RN-13, RN-14, RN-28, RN-29 | CU-01 | CP-11, CP-13, CP-19, CP-22 a CP-25, CP-31, CP-41 | H-04 |
| RF-06 | OE-5 | Must | RN-15, RN-16, RN-30, RN-31, RN-32 | CU-01 | CP-07, CP-32, CP-33, CP-34\*, CP-39 | H-02 |
| RF-07 | OE-2 | Must | RN-17, RN-18 | CU-01 | CP-32, CP-33 | — |
| RF-08 | OE-3 | Should | RN-19 a RN-24 | CU-01, CU-03 | CP-01 a CP-06, CP-08, CP-09, CP-11, CP-42 | H-01, H-05 |
| RF-09 | — | Won't | — | — | — | — |
| RF-10 | OE-6 ind. | Could | RN-10, RN-35, RN-36 | CU-07 | CP-24, CP-37 | — |
| RF-11 | OE-6 | Should | RN-34 | CU-08 | CP-35, CP-36, CP-37, CP-38 | — |
| RF-12 | OE-2 | Must | RN-06, RN-07, RN-11, RN-38 | CU-02 | CP-21 | — |
| RF-13 | OE-3 | Should | RN-25, RN-26, RN-37, RN-39 | CU-06 | CP-10 | — |

El párrafo «La indicación "ind." en RF-10…» queda igual.

**Agregar**, debajo de ese párrafo:

> \* CP-34 verifica RN-15 y está pendiente de corrección: no se computa en la cobertura.
> La categoría Won't de RF-09 corresponde a la reclasificación de la versión 2.5 (brecha
> G-03). Los requerimientos no funcionales no figuran en la matriz porque no originan casos
> de uso. RNF-03 (compatibilidad multipantalla) cuenta con dos casos de prueba, CP-12 y
> CP-25.

La tabla «Trazabilidad inversa de los objetivos específicos» de la misma página no cambia.

---

## 8. Página 108 — Cobertura

La advertencia sobre OE-4, al principio de la página, no cambia.

### Tabla «Cobertura de reglas de negocio por casos de prueba»

**Reemplazar la tabla completa:**

| Caso de prueba | Reglas verificadas |
|---|---|
| CP-01 — Dos compras, nivel Bronce | RN-19, RN-20 |
| CP-02 — Tres compras, nivel Plata | RN-19, RN-20 |
| CP-03 — Seis compras, no asciende | RN-19 |
| CP-04 — Siete compras, nivel Oro | RN-19, RN-20 |
| CP-05 — La base excluye los servicios | RN-21 |
| CP-06 — Ventana móvil de doce meses | RN-19 |
| CP-07 — Precio congelado al iniciar | RN-16 |
| CP-08 — El ascenso rige desde la compra siguiente | RN-22 |
| CP-09 — Reembolso y reevaluación mensual | RN-23, RN-24 |
| CP-10 — La suscripción no acumula porcentaje | RN-37 |
| CP-11 — Verificación de referencia de $ 99.625 | RN-20, RN-21 |
| CP-12 — Alto máximo de los formularios | — (RNF-03) |
| CP-13 — Paquete máximo sin recortes | RN-10 |
| CP-14 — Registro sin datos precargados | — |
| CP-15 — El registro rechaza datos inválidos | RN-02 |
| CP-16 — Búsqueda sin resultados | — |
| CP-17 — Comprar sin evento seleccionado | — |
| CP-18 — Texto guía de la búsqueda | — |
| CP-19 — Volver con un paquete armado | — |
| CP-20 — Cantidad inválida | — |
| CP-21 — Eliminar un evento con y sin ventas | RN-11 |
| CP-22 — Devolución sin entradas elegidas | — |
| CP-23 — Elegir entradas para devolver | — |
| CP-24 — Evento agotado que recupera cupo | RN-10, RN-14 |
| CP-25 — Compra a escala 125 % y 150 % | — (RNF-03) |
| CP-26 — Alta: nivel inicial y correo único | RN-01, RN-03 |
| CP-27 — Inicio de sesión correcto | — |
| CP-28 — Bloqueo por intentos fallidos | RN-04 |
| CP-29 — Datos faltantes o correo inexistente | — |
| CP-30 — Filtros de búsqueda y evento no vigente | RN-09 |
| CP-31 — Topes del paquete | RN-08, RN-12, RN-13 |
| CP-32 — Pago aprobado | RN-17, RN-18, RN-30, RN-31 |
| CP-33 — Pago rechazado | RN-17, RN-31, RN-32 |
| CP-34 — Reserva vencida (pendiente) | RN-15 \* |
| CP-35 — Acceso a reportes | RN-34 |
| CP-36 — RPT-01 cuenta solo lo confirmado | — |
| CP-37 — RPT-05 y lista de espera | RN-10, RN-36 |
| CP-38 — Exportación y período inválido | — |
| CP-39 — Temporizador de la reserva | — |
| CP-40 — Cupo restante por sector | — |
| CP-41 — Confirmación al comprar sin seguro | — |
| CP-42 — Explicación de los niveles | — |

**Reemplazar** el párrafo que sigue a la tabla:

> Los diez casos verifican ocho reglas distintas —RN-16, RN-19, RN-20, RN-21, RN-22, RN-23,
> RN-24 y RN-37— sobre un total de treinta y seis vigentes, lo que arroja una cobertura del
> 22,2 %. Las veintiocho reglas restantes carecen de verificación automatizada.

**Por:**

> Los cuarenta y dos casos verifican veintiséis reglas distintas sobre un total de treinta y
> ocho vigentes, lo que arroja una cobertura del 68,4 %. Son RN-01 a RN-04, RN-08 a RN-14,
> RN-16 a RN-24, RN-30 a RN-32, RN-34, RN-36 y RN-37. Las doce restantes carecen de
> verificación automatizada: RN-05, RN-06, RN-07, RN-15, RN-25, RN-26, RN-28, RN-29, RN-33,
> RN-35, RN-38 y RN-39. RN-15 tiene su caso escrito (CP-34), pendiente de corrección, por lo
> que no se computa.
>
> Varios casos no suman reglas porque verifican otra cosa: que la interfaz comunique al
> usuario reglas que ya funcionan, que es el problema que señaló el informe de usabilidad.
> CP-39 a CP-42 son la regresión de los hallazgos H-02 a H-05: muestran el temporizador de
> RN-15, el cupo de RN-08, el aviso de RN-28 y los umbrales de RN-19, pero no verifican esas
> reglas.

### Agregar: tabla «Casos de prueba incorporados en la versión 2.5»

Va a continuación del párrafo anterior. CP-01 a CP-10 siguen definidos en «Desarrollo de la
lógica de descuento por nivel». La columna «Prueba automatizada» es el nombre del método en
`Pruebas/`.

| Caso | Qué verifica | Prueba automatizada | RF | Hall. | Ejec. |
|---|---|---|---|---|---|
| CP-11 | Cliente Plata, 2 entradas Campo a $ 25.000 y hospedaje de $ 45.000: los siete pasos del cálculo, total $ 99.625 | `Referencia_DaExactamente99625` | RF-05, RF-08 | — | 1 |
| CP-12 | Ningún formulario declara un área cliente de más de 790 px de alto | `RNF03_NingunFormularioPasaDe790DeAlto` | RNF-03 | — | 1 |
| CP-13 | Paquete máximo con título de 100 caracteres: título, nivel y enlace sin superponerse; cada importe y el TOTAL completos, con la leyenda del descuento. En un evento agotado, servicios deshabilitados | `Compra_PaqueteMaximoYNombreLargoSinRecortes` | RF-05 | H-01 | 1 |
| CP-14 | El alta de usuario abre con todos los campos vacíos | `Registro_AbreSinDatosPrecargados` | RF-01 | — | 1 |
| CP-15 | Nombre, apellido o correo con solo espacios, y contraseña «abc»: aviso y sin alta | `Registro_RechazaDatosInvalidosSinCrearUsuario` | RF-01 | — | 4 |
| CP-16 | Búsqueda sin resultados: una fila de aviso, grilla sin edición | `Eventos_SinResultadosNoPermiteAgregarFilas` | RF-03 | — | 1 |
| CP-17 | «Comprar» sin evento elegido: aviso «Seleccionar evento», no abre la compra | `Eventos_ComprarSinResultadosMuestraAvisoCorrecto` | RF-03 | — | 1 |
| CP-18 | Texto guía visible con el cursor en la búsqueda | `Eventos_GuiaRegistradaConFocoEnLaBusqueda` | RF-03 | — | 1 |
| CP-19 | «Volver al listado» pide confirmación, con No por defecto; No conserva el paquete | `Compra_VolverConfirmaSinPerderPaqueteAlCancelar` | RF-05 | H-06 | 1 |
| CP-20 | Cantidad «abc» o «-1»: aviso y se restaura la anterior | `Compra_CantidadInvalidaRestauraElPaquete` | RF-04 | — | 2 |
| CP-21 | Eliminar pide confirmación; con ventas no elimina y ofrece suspender | `Propietario_EliminarRequiereConfirmacionYPreservaVentas` | RF-12 | — | 1 |
| CP-22 | Sin entradas elegidas, «Devolver» deshabilitado y gris | `Historial_SinSeleccionDistingueBotonDeshabilitado` | RF-05 | — | 1 |
| CP-23 | Tildar una entrada asegurada habilita «Devolver»; destildarla lo deshabilita | `Historial_TildarYDestildarActualizaEstadoYColor` | RF-05 | — | 1 |
| CP-24 | Evento agotado que recupera cupo: vuelve la grilla, solo servicios del evento, sin inscribir en la lista | `Compra_CupoLiberadoVuelveAHabilitarServiciosDisponibles` | RF-05, RF-10 | — | 1 |
| CP-25 | A 125 % y 150 % la compra entra en pantalla y las acciones quedan a la vista | `Compra_EscalaSimuladaMantieneAccionesDentroDelArea` | RF-05, RNF-03 | — | 3 |
| CP-26 | El alta nace Bronce y sin suscripción; un correo repetido se rechaza | `CP26_RegistroBronceSinSuscripcionYCorreoUnico` | RF-01 | — | 1 |
| CP-27 | Inicio de sesión correcto: devuelve el usuario sin la contraseña y abre la sesión | `CP27_InicioDeSesionCorrecto` | RF-02 | — | 1 |
| CP-28 | Cada intento fallido informa los restantes; al tope se bloquea y rechaza incluso la contraseña correcta; al vencer, entra y el contador vuelve a cero | `CP28_BloqueoTrasIntentosFallidos` | RF-02 | — | 1 |
| CP-29 | Correo inexistente o datos vacíos: no autentica ni abre sesión | `CP29_DatosFaltantesOCorreoInexistente` | RF-02 | — | 1 |
| CP-30 | Filtros por nombre, dirección, categoría y fecha; el evento pasado no aparece y rechaza compra y lista de espera; el suspendido no aparece | `CP30_FiltrosYEventoNoVigente` | RF-03 | — | 1 |
| CP-31 | Rechaza más entradas que el cupo, más que el máximo por operación y servicios sin entradas | `CP31_TopesDelPaquete` | RF-04, RF-05 | — | 1 |
| CP-32 | Pago aprobado: compra confirmada, QR únicos por entrada, pago registrado con los últimos 4 dígitos y el token, sin el número completo | `CP32_PagoAprobadoConfirmacionQrYRegistro` | RF-06, RF-07 | — | 1 |
| CP-33 | Pago rechazado: queda registrado, la compra no se confirma y el cupo vuelve para otro cliente | `CP33_PagoRechazadoLiberaCupoYQuedaRegistrado` | RF-06, RF-07 | — | 1 |
| CP-34 \* | Reserva vencida: rechaza el cobro, vence la compra y devuelve el cupo. Pendiente de corrección | `CP34_ReservaVencida` | RF-06 | — | 1 (omitida) |
| CP-35 | Los reportes rechazan a quien no es Administrador | `CP35_SoloAdministradorAccede` | RF-11 | — | 1 |
| CP-36 | RPT-01 cuenta solo entradas confirmadas: cantidades, ingreso, ticket y ocupación | `CP36_VentasCuentaSoloEntradasConfirmadas` | RF-11 | — | 1 |
| CP-37 | RPT-05 calcula «Agotado» o «Con cupo» en el momento; una inscripción repetida se rechaza | `CP37_DemandaAgotadoYListaDeEspera` | RF-10, RF-11 | — | 1 |
| CP-38 | La exportación recibe los mismos datos que el reporte; período o código inválidos se rechazan | `CP38_ExportacionYPeriodoInvalido` | RF-11 | — | 1 |
| CP-39 | El temporizador de la reserva está a la vista y pasa a rojo en el último minuto | `CP39_TemporizadorDeReservaALaVista` | RF-06 | H-02 | 1 |
| CP-40 | La grilla muestra el cupo restante de cada sector | `CP40_CupoRestantePorSector` | RF-04 | H-03 | 1 |
| CP-41 | Comprar sin seguro pide confirmación, con No por defecto, y no reserva nada si se cancela | `CP41_ConfirmacionAlComprarSinSeguro` | RF-05 | H-04 | 1 |
| CP-42 | El enlace de niveles muestra los umbrales vigentes | `CP42_ExplicacionDeLosNiveles` | RF-08 | H-05 | 1 |

### Tabla «Análisis de cobertura»

**Reemplazar la tabla completa.** Se agrega la fila de hallazgos con la acción implementada:

| Dimensión evaluada | Total | Cubierto | Sin cobertura | % |
|---|---|---|---|---|
| Objetivos específicos con evidencia documentada | 6 | 6 | — | 100 % |
| Requerimientos funcionales con caso de uso | 13 | 12 | RF-09 (Won't) | 92,3 % |
| Casos de uso con requerimiento que los origine | 8 | 8 | — | 100 % |
| Reglas de negocio con origen declarado | 38 | 38 | — | 100 % |
| Requerimientos funcionales con caso de prueba | 13 | 12 | RF-09 (Won't) | 92,3 % |
| Reglas de negocio verificadas por caso de prueba | 38 | 26 | 12 reglas | 68,4 % |
| Hallazgos de usabilidad con acción de mejora | 6 | 6 | — | 100 % |
| Hallazgos con la acción implementada en el prototipo | 6 | 6 | — | 100 % |
| Hallazgos con prueba de regresión asociada | 6 | 6 | — | 100 % |

### Lectura del cuadro

**Reemplazar:**

> La trazabilidad hacia adelante, del requerimiento a su verificación, es el punto débil del
> proyecto: sólo tres de trece requerimientos cuentan con pruebas, y todas ellas se
> concentran en la lógica de fidelización.

**Por:**

> La trazabilidad hacia adelante, del requerimiento a su verificación, pasó de ser el punto
> débil del proyecto a estar completa en el nivel de requerimientos. Los doce requerimientos
> en alcance cuentan con pruebas, contra tres de trece en la versión 2.4; el único sin
> pruebas es RF-09, excluido. Lo que queda abierto está en el nivel de reglas: doce de
> treinta y ocho no tienen caso automatizado. Entre ellas están las dos incorporadas en
> esta versión (RN-38 y RN-39) y RN-15, cuyo caso existe pero está pendiente de corrección.

---

## 9. Página 109 — Brechas detectadas

**Reemplazar** la oración inicial:

> El análisis identificó cuatro brechas. La primera fue resuelta a partir de este mismo
> análisis; las tres restantes quedan documentadas con su acción correctiva.

**Por:**

> El análisis identificó cinco brechas. G-01 se resolvió a partir de este mismo análisis.
> En la versión 2.5 se cerró G-03, se resolvieron G-04 y G-05, y G-02 quedó resuelta en el
> nivel de requerimientos y abierta en el de reglas.

**Reemplazar la tabla completa.** G-01 no cambia y se repite para que la tabla quede entera:

| Nº | Brecha | Sev. | Estado | Acción |
|---|---|---|---|---|
| G-01 | Los casos de uso CU-02 (Gestionar eventos) y CU-06 (Suscribirse) no contaban con un requerimiento funcional que los originara, pese a que las reglas RN-06, RN-07 y RN-11 por un lado, y RN-25, RN-26 y RN-37 por el otro, presuponen su existencia. | Alta | Resuelta | Se incorporaron los requerimientos RF-12 (Gestión de eventos) y RF-13 (Contratación de suscripciones) a la Definición de requerimientos, y se recalculó la priorización MoSCoW. |
| G-02 | En la versión 2.4, sólo tres de los trece requerimientos poseían casos de prueba (23,1 %) y la cobertura sobre reglas de negocio era del 22,2 %. | Alta | Resuelta en requerimientos; abierta en reglas | Se incorporaron los casos CP-11 a CP-42. Los doce requerimientos en alcance tienen pruebas. La cobertura de reglas sube al 68,4 % (26 de 38). Quedan doce reglas sin caso, entre ellas RN-15, cuyo caso CP-34 está pendiente de corrección. |
| G-03 | RF-09 (Recomendaciones personalizadas) no responde a ningún objetivo específico ni posee caso de uso asociado. | Media | Cerrada | Se reclasificó como Won't have en la Fase 8, según la regla de repriorización (W-07). |
| G-04 | En la versión 2.4, ninguno de los seis hallazgos del informe de usabilidad tenía caso de prueba de regresión. | Media | Resuelta | Las seis acciones del plan de mejora están implementadas en el prototipo y cada hallazgo tiene su regresión automatizada: H-01 en CP-13, H-02 en CP-39, H-03 en CP-40, H-04 en CP-41, H-05 en CP-42 y H-06 en CP-19. |
| G-05 | El prototipo aplicaba dos reglas que el documento no enunciaba: el tope de capacidad del local sobre la suma de sus sectores y la preventa exclusiva para suscriptores. Era comportamiento implementado sin regla documentada que lo respalde. | Alta | Resuelta | Se incorporaron RN-38 (RF-12 / CU-02) y RN-39 (RF-13 / CU-01). No tienen todavía caso de prueba: quedan dentro de la parte abierta de G-02. |

### Observación sobre G-03

**Reemplazar** el párrafo completo:

> **Observación sobre G-03.** La matriz confirma por una vía independiente la decisión ya
> adoptada en la priorización: RF-09 había sido clasificado como Could por ser el
> requerimiento más costoso sin objetivo asociado. Que el análisis de trazabilidad llegue al
> mismo resultado partiendo de otro criterio refuerza la validez de ambos.

**Por:**

> **Observación sobre G-03.** La matriz confirmó por una vía independiente la decisión ya
> adoptada en la priorización: RF-09 había sido clasificado como Could por ser el
> requerimiento más costoso sin objetivo asociado. Que el análisis de trazabilidad llegara
> al mismo resultado partiendo de otro criterio reforzó la validez de ambos. En la Fase 8 se
> aplicó lo previsto: RF-09 no se incorporó y pasó a Won't have. La brecha se cierra por
> decisión de alcance y no por construcción.

### Agregar: observación sobre G-05

Va debajo de la observación sobre G-03:

> **Observación sobre G-05.** Esta brecha recorre la cadena en sentido inverso a las otras:
> no falta una prueba para una regla, falta la regla para un comportamiento que ya existe.
> Se prefirió documentarla aunque la cobertura bajara: con 36 reglas el porcentaje habría
> sido mayor, pero habría medido un sistema distinto del construido.

Las secciones «Mantenimiento de la matriz» y el párrafo final de la página no cambian.

---

## 10. Páginas 110 y 111 — Conclusiones

### Página 110 — «Sobre las reglas de negocio»

**Reemplazar:**

> Explicitar las treinta y seis reglas resultó más productivo de lo esperado.

**Por:**

> Explicitar las treinta y ocho reglas resultó más productivo de lo esperado.

### Página 110 — tabla «Limitaciones del trabajo», tercera fila

**Reemplazar** la fila:

| La cobertura de pruebas alcanza el 22,2 % de las reglas. | Diez de trece requerimientos carecen de verificación automatizada. |
|---|---|

**Por:**

| La cobertura de pruebas alcanza el 68,4 % de las reglas. | Todos los requerimientos en alcance tienen pruebas, pero doce reglas carecen de verificación automatizada, entre ellas RN-15 (vigencia de la reserva). |
|---|---|

### Página 111 — Trabajo futuro

**Reemplazar** el punto 2:

> Extender el conjunto de pruebas al flujo de compra completo, de modo de cerrar la brecha
> G-02 de la matriz de trazabilidad.

**Por:**

> Completar los casos de prueba de las doce reglas sin verificación, empezando por RN-15,
> RN-38 y RN-39, de modo de cerrar la brecha G-02 de la matriz de trazabilidad.

**Reemplazar** el punto 4:

> Definir un caso de prueba de regresión por cada hallazgo de usabilidad antes de aplicar las
> correcciones de la Fase 6.

**Por:**

> Mantener una prueba de regresión por cada hallazgo de las próximas rondas de usabilidad,
> como se hizo con H-01 a H-06.

---

## 11. Lo que no cambié y conviene que decidas

1. **RN-35 dice otra cosa en el documento y en el código.** El PDF (página 27) dice que la
   lista de espera respeta «estrictamente el orden de inscripción». `docs/02-reglas-negocio.md`
   la reescribió: primero los que se inscribieron con suscripción activa, cada grupo por
   orden de inscripción y con la condición congelada al inscribirse. Así lo exige RN-37, que
   promete prioridad en la lista de espera. Es el mismo tipo de brecha que G-05: el
   comportamiento no coincide con la regla documentada.
2. **RN-06 y RN-29 están más detalladas en `docs/` que en el PDF.** RN-06 es un invariante
   que también impide borrar el último sector de un evento publicado. RN-29 dice qué se
   devuelve y qué no en una devolución por entrada. No contradicen al PDF, solo lo amplían.

---

## 12. Si Codex corrige CP-34 antes de la entrega

Corresponde a T-19 en `TAREAS-CODEX-PRUEBAS.md`. Si CP-34 pasa, se cambian estos valores:

| Dónde | Hoy | Con CP-34 corregida |
|---|---|---|
| Ejecuciones | 48: 47 correctas, 1 omitida | 48 de 48 correctas |
| Reglas verificadas | 26 de 38 (68,4 %) | 27 de 38 (71,1 %) |
| Reglas sin caso | 12 | 11; sale RN-15 |
| Asteriscos de CP-34 y su nota | Se mantienen | Se quitan |
| Textos de páginas 8, 108, 109, 110 y 111 | 68,4 %, doce reglas, mención de RN-15 | 71,1 %, once reglas, sin mención de RN-15 |

Si además se aprueba T-20, se agrega **CP-43** (RF-06, RN-15): «el plazo lo fija la reserva y
no el parámetro vigente». No cambia la cobertura de reglas, porque RN-15 ya estaría
contada. Se suma una fila al catálogo y el total pasa a 43 casos y 49 ejecuciones.
