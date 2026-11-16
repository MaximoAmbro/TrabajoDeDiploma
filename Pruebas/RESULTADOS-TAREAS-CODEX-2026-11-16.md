# Cierre Codex — prioridad, mutación y propuestas — 16/11/2026

## Estado inicial y publicación anterior

El pedido de guardar las 0,74 h ya se había cumplido en el turno anterior. Se verificó antes de tocar archivos:

```text
On branch main
Your branch is up to date with 'origin/main'.

nothing to commit, working tree clean
```

`origin/main` y `origin/JC`: `027527f feat(visual): pantalla de exito y trece pantallas con patron aprobado (bateria 243 verdes)`. Ese commit incluye `c9d3a1c`, reglas y seguridad. No se duplicaron commits ni push. Ambos App.config conservaban S y siguen protegidos; índice vacío. El respaldo remoto sigue en 98be355; no se toca su referencia local, que ya difería (81c5f62).

## Punto 2: prioridad real implementada

**No se puede derivar todo el instante.** MontoReembolsado es un DECIMAL acumulado, no una fecha. ReservaVence permite conocer una fecha de vencimiento, pero se pone en NULL al liberar por rechazo; tampoco fecha devoluciones parciales ni completas. No hay un registro histórico del reembolso que permita obtener su instante por sector.

Se agregó exactamente una columna: **Sectores.CupoLiberadoEl DATETIME NULL**. No hay tabla, entidad de negocio ni proceso nuevo. La excepción técnica CupoPrioritarioException sigue el patrón de CupoInsuficienteException de Datos; no es una entidad persistida.

La columna se actualiza dentro de la transacción que libera disponibilidad:

- devolución de entradas: solamente los sectores efectivamente devueltos;
- vencimiento de reservas: solamente las que todavía retenían cupo, usando el instante de liberación efectiva;
- liberación por pago rechazado y otras reservas abiertas: misma transacción que descuenta CantidadReservada;
- confirmar una compra NO inicia una ventana: el reservado pasa a vendido y no aparece disponibilidad.

El parámetro **PRIORIDAD_ESPERA_MINUTOS** se lee de Parametros al reservar; valor inicial elegido: **15 minutos**, configurable. La exclusividad es por sector y se activa si existe una inscripción prioritaria para el evento. Sin inscriptos prioritarios no se bloquea una venta sin beneficiario. Dentro de la ventana solo compra quien tiene una inscripción con ConSuscripcion=1. Se conserva el criterio congelado de RN-35: el beneficio de esa inscripción no cambia al vencer o cancelar luego el plan. No se transforma el orden FIFO dentro del grupo en turnos individuales: los inscriptos prioritarios compiten entre sí; el resto espera el final de la ventana.

La comprobación toma UPDLOCK/HOLDLOCK sobre el sector y se ejecuta dentro de la misma transacción que reserva; un rechazo RN-35 revierte la compra nueva, sus entradas y servicios. El reintento de pago también pasa por ella antes de reservar y antes de crear otro pago. No alcanza con un control visual ni con armar el paquete antes de liberarse el lugar.

**Prueba CP-125, ocho filas:** no suscriptor rechazado dentro; suscriptor inscripto aceptado dentro; no suscriptor aceptado fuera; vencimiento rechazando al común y aceptando al prioritario; pago rechazado activando ventana; suscriptor sin inscripción rechazado; reintento previo rechazado sin volver a reservar ni insertar otro pago. Las negativas comprueban el cupo intacto y que no persista una compra nueva. También se retiró la defensa SQL: CP-125 falló y volvió a verde al restaurarla.

### Qué corre Máximo

En SSMS, elegir su base de EventAura y ejecutar **BaseDatos/06-prioridad-temporal.sql** antes de usar el ejecutable nuevo. Es idempotente: agrega la columna si falta e inserta el parámetro si falta, conservando el valor si ya existe. No inventa fechas históricas: los sectores existentes quedan en NULL hasta una liberación nueva. No se aplicó a la base local de Juan Cruz; la prueba recrea exclusivamente _Pruebas.

Para instalaciones nuevas, 01-esquema.sql y 02-semilla.sql ya incluyen la columna y el parámetro. El fixture también ejecuta 06, comprobando la compatibilidad con la creación nueva. Las primeras corridas de esta tanda verificaron además la rama de migración sobre el esquema sin la columna.

## Punto 4: encargo de mutación cerrado

No son 23 casos nuevos: la tarea 0 ya existía, la 12 refuerza CP-94 y la 22 pide no escribir una prueba temporal. Se añadieron **20 casos CP-126 a CP-145**. La tarea 0 quedó publicada en c9d3a1c, con el estrés de 100 rondas y diez hilos; sus mutaciones ya estaban verificadas por Claude y la batería completa continúa verde. La tarea 22 se verificó por lectura de Seguridad.SonIguales: acumula XOR sin salida anticipada.

Cada caso negativo afirma la regla, que no se devolvió el dato y que la sesión no fue sustituida. Los controles positivos prueban que el dato o la operación existen; los casos de ingreso e índice comprueban directamente el estado persistido y los rollback. CP-130 primero migra realmente el fixture heredado a PBKDF2, confirma que lo almacenado es un hash y recién entonces intenta usarlo como contraseña; no confunde el texto plano inicial de Fake con un hash.

| Tarea | Caso | Mutante local | Resultado comprobado |
|---|---|---|---|
| 1 | CP-126 | 1 | Rojo al retirar defensa; verde restaurada |
| 2 | CP-127 | 2 | Rojo al retirar defensa; verde restaurada |
| 3 | CP-128 | 3 | Rojo al retirar defensa; verde restaurada |
| 4 | CP-129 | 4 | Rojo al retirar defensa; verde restaurada |
| 5 | CP-130 | 5 | Rojo al retirar defensa; verde restaurada |
| 6 | CP-131 | 6 | Rojo al retirar defensa; verde restaurada |
| 7 | CP-132 | 7 | Rojo al retirar defensa; verde restaurada |
| 8 | CP-133 | 8 | Rojo al retirar defensa; verde restaurada |
| 9 | CP-134 | 9 | Rojo al retirar defensa; verde restaurada |
| 10 | CP-135 | 10 | Rojo al retirar defensa; verde restaurada |
| 11 | CP-136 | 11 | Rojo al retirar defensa; verde restaurada |
| 12 | CP-94 | 12a | Rojo al retirar defensa; verde restaurada |
| 12 | CP-94 | 12b | Rojo al retirar defensa; verde restaurada |
| 13 | CP-137 | 13 | Rojo al retirar defensa; verde restaurada |
| 14 | CP-138 | 14 | Rojo al retirar defensa; verde restaurada |
| 15 | CP-139 | 15 | Rojo al retirar defensa; verde restaurada |
| 16 | CP-140 | 16 | Rojo al retirar defensa; verde restaurada |
| 17 | CP-141 | 17 | Rojo al retirar defensa; verde restaurada |
| 18 | CP-142 | 18 | Rojo al retirar defensa; verde restaurada |
| 19 | CP-143 | 19 | Rojo al retirar defensa; verde restaurada |
| 20 | CP-144 | 20 | Rojo al retirar defensa; verde restaurada |
| 21 | CP-145 | 21 | Rojo al retirar defensa; verde restaurada |

Resultado: **22 mutaciones del encargo detectadas**, incluyendo las dos de CP-94. Más la mutación adicional de prioridad: **23/23**. Para cada una se compiló el mutante, se ejecutó el caso, se restauraron los bytes originales en finally, se recompiló y se ejecutó el mismo caso en verde. CP-126 se recontroló con el cliente A indicado en el encargo; prioridad se recontroló después de agregar el reintento. Ambos pares también rojo/verde.

No se dejó rota ninguna defensa. El runner reproducible está en Pruebas/VerificarMutacionesCola16.py; toma copias en memoria y restaura los bytes de la fuente mutada en finally. No correrlo mientras alguien modifica esos archivos o usa _Pruebas. Los logs de compilación, rojo y verde quedan en TestResults/Cola16/mutacion/; resultados.json resume cada tarea y recontrol/ contiene las repeticiones indicadas.

## Punto 3: propuestas, sin adaptación de producción

La dificultad es **distribución**, no estilo. Los colores, tipografía, grillas y botones siguen el tema actual. Los dos formularios y sus Designer permanecen sin cambios. Los bocetos son instancias reales reorganizadas solo en memoria desde Pruebas/PropuestasGrillasCola16.cs; las capturas se identifican como PROPUESTA. No se implementó navegación ni una nueva pantalla de producción.

### Historial

La plantilla de una sola grilla no define cómo repartir alto entre la selección de una compra y la selección de sus entradas. Además hay siete columnas de compras, seis de entradas, ayuda de reembolso y acción vinculada al detalle. Ocultar una grilla o quitar contexto requiere elegir una distribución.

- **A — dos grillas apiladas:** compras arriba, entradas del pedido debajo, ayuda y acciones al pie. Se conservan selección y detalle simultáneamente. Recomendada: mantiene el flujo existente y permite revisar pedido y devolución juntos. Menos filas visibles en cada grilla.
- **B — pestañas Compras / Entradas del pedido:** una grilla por pestaña y más alto útil. Se conserva la selección al cambiar de pestaña. Hace falta una interacción extra y se pierde la comparación simultánea; la captura adicional B-detalle muestra la segunda pestaña.

Capturas: [A 100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-100/Historial-A.png), [A 150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-150/Historial-A.png), [B 100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-100/Historial-B.png), [B 150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-150/Historial-B.png), [detalle B 150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-150/Historial-B-detalle.png).

### Reportes

La plantilla de lista simple no reserva espacio para cinco tipos de reporte con filtros diferentes (categoría, evento, corte, agotados), selector de período, lectura y exportación. Si esos filtros quedan en una sola fila, compiten con el ancho de la grilla; si se separan, hay que decidir cuánto alto dedicarles.

- **A — filtros superiores en dos filas:** selector y fechas arriba; filtro condicional y Generar debajo; grilla a todo el ancho; lectura y acciones al pie. Recomendada: conserva el ancho para nombres e importes y coincide con el patrón de listas aprobado. Menos alto de grilla.
- **B — filtros en columna izquierda:** controles apilados y grilla a la derecha. Más filas visibles y una zona estable de filtros, a costa de columnas más estrechas y mayor truncamiento de nombres.

Capturas: [A 100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-100/Reportes-A.png), [A 150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-150/Reportes-A.png), [B 100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-100/Reportes-B.png), [B 150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/propuestas-150/Reportes-B.png).

Las propuestas usan área cliente de 900×512 a 96 DPI y 1350×768 a 144 DPI. El marco externo depende del formulario/Windows. Son opciones para decidir; no una aprobación visual ni evidencia de adaptación terminada. Los demás filtros existentes conservan su comportamiento; el boceto principal muestra RPT-01.

## Punto 5: capturas rehechas, sin mezclar con las tapadas

Se rehicieron **las 20 pantallas completas en ambas escalas**, no solo las que antes se identificaron como tapadas: **40 imágenes principales**, más dos vistas de TOTAL de compra. Se guardaron aparte en Cola16. No se usan para el nuevo reporte los archivos -escritorio ni las capturas antiguas de Cola/.

Todas las imágenes nuevas se obtienen con DrawToBitmap sobre el formulario real a DPI nativo. Para corregir el problema de WM_PRINT de controles transparentes, el capturador vuelve a solicitar DrawToBitmap a los botones reales y los dibuja en sus coordenadas reales, respetando el recorte de sus padres. No captura escritorio, no elimina ventanas de la imagen ni cambia la geometría de producción. Se verificó visualmente el botón Volver de Listas de espera y Registrarse al 150 %, que antes quedaban tapados por ese defecto del capturador.

El manifiesto TestResults/Cola16/manifest-capturas.json identifica método, condición de rehecha y SHA-256 de cada PNG: 42 capturas actuales + 10 imágenes de propuestas (incluyen segunda pestaña) = 52. Las capturas muestran datos de prueba; los QR/rutas/fechas pertenecen al fixture, no a operaciones reales.

| Pantalla rehecha | 96 DPI | 144 DPI |
|---|---|---|
| frmAdministrarEvento | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmAdministrarEvento.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmAdministrarEvento.png) |
| frmAgregarEvento | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmAgregarEvento.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmAgregarEvento.png) |
| frmAgregarSector | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmAgregarSector.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmAgregarSector.png) |
| frmCambiarContraseña | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmCambiarContraseña.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmCambiarContraseña.png) |
| FrmCompraCliente | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/FrmCompraCliente.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/FrmCompraCliente.png) |
| frmConfirmacion | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmConfirmacion.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmConfirmacion.png) |
| frmControlIngreso | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmControlIngreso.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmControlIngreso.png) |
| frmCrearUsuario | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmCrearUsuario.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmCrearUsuario.png) |
| frmEventosCliente | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmEventosCliente.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmEventosCliente.png) |
| frmEventosLocal | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmEventosLocal.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmEventosLocal.png) |
| frmHistorialCliente | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmHistorialCliente.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmHistorialCliente.png) |
| frmInicio | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmInicio.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmInicio.png) |
| frmListasEspera | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmListasEspera.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmListasEspera.png) |
| frmMenuCliente | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmMenuCliente.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmMenuCliente.png) |
| frmMenuPropietario | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmMenuPropietario.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmMenuPropietario.png) |
| frmMisLocales | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmMisLocales.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmMisLocales.png) |
| frmPago | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmPago.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmPago.png) |
| frmRegistrarLocal | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmRegistrarLocal.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmRegistrarLocal.png) |
| frmReportes | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmReportes.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmReportes.png) |
| frmSuscripcion | [100 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-100/frmSuscripcion.png) | [150 %](D:/proyectos/TrabajoDeDiploma/TestResults/Cola16/capturas-150/frmSuscripcion.png) |

## Números finales y alcance

- **271 ejecuciones: 271 correctas, cero fallidas.** TestResults/Cola16/final.log, 57,5082 segundos.
- 243 anteriores + 8 filas de CP-125 + 20 casos CP-126…145. CP-94 sigue siendo una fila.
- Encargo de mutación: 22/22 mutantes efectivos del encargo detectados, más el de prioridad: 23/23. Tareas 0 y 22 tratadas como indica el encargo.
- Barrera visual: 49/49; veinte pantallas, siete filas de serialización, CP-116/120. Color renderizado: 31 primarios, cero fallos. TestResults/cola16-patron.log y su carpeta de color.
- Sin implementación del rediseño de Historial ni Reportes. No se tocó ningún archivo de Visual/ en esta tanda.
- No se modificaron docs/, tools/ ni App.config. Los cambios concurrentes que aparecieron en docs/ son de Claude y no se tocaron.
- El trabajo nuevo queda sin commitear para revisión de status. La corrida final global de mutación de Claude debe usar este código cuando se autorice y publique; el remoto todavía tiene la tanda anterior.

## Vueltas y portones humanos

Estimación inicial: 80–140 vueltas de herramienta de Codex, no horas de desarrollador. Evidencia ejecutada: 23 pares rojo/verde y dos pares de recontrol = **50 ejecuciones dirigidas y 50 compilaciones**, dos baterías completas (la primera detectó errores de preparación propios de los fixtures nuevos; la segunda cerró 271/271), una corrida dirigida verde de 28 filas antes de sumar reintento, una barrera completa de 49 y dos pasadas de captura por escala (la segunda rehizo el capturador). Lecturas y ediciones se hicieron además de esas ejecuciones. Este desglose no pretende ser un contador exacto de llamadas a la API de herramientas.

Portones humanos: elegir A/B para Historial y Reportes antes de aplicarlas; revisar status y autorizar la próxima publicación; revisión global final de mutación de Claude sobre el código publicado. No se esperó ninguno para completar los puntos independientes. Máximo debe aplicar 06 antes de usar el ejecutable nuevo.
Tiempo real de reloj de esta tanda: 09:47:49 a 10:08:42 ART, 0,348 h. No son horas-persona. Se intercalaron las tareas independientes; no se inventa una imputación individual de horas que no se cronometró.

Se recapturaron adem?s solo las propuestas en ambas escalas tras agregar el panel de lectura desplazable de la opci?n B de Reportes. El manifiesto corresponde a estos PNG definitivos.
Cierre definitivo 10:10:14 ART: 0.374 h de reloj.
