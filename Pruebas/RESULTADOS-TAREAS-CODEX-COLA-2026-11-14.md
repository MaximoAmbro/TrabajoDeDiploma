# Cola Codex — 14/11/2026

## Publicación inicial autorizada

Se prepararon únicamente rutas explícitas de la zona propia, con comprobación de la lista preparada antes de cada commit. Ningún App.config estuvo preparado. Los dos conservaron S de skip-worktree.

- a2f4b55: posición privada y aviso de disponibilidad.
- 96fd6ec: primera tanda visual y barrido de botones; batería anterior 232 verdes.
- Ambos se publicaron en origin/main y origin/JC. No se tocó respaldo/jc-2026-11-04.
- El git status completo previo está en ESTADO-PREVIO-COMMITS-COLA-2026-11-14.md. El trabajo descrito debajo queda sin commitear. Claude agregó después su commit 8e3a04f; no se revirtió ni se incluyó en nuestros commits.

## Punto 2 cerrado: 243/243

Los cuatro puntos no estaban hechos en la tanda anterior; se implementaron ahora, antes de replicar pantallas.

| Caso nuevo | Prueba | Filas |
|---|---|---:|
| RN-26 | PruebasReglasD5.CP121_VencidaNoSeRenueva | 1 |
| RN-28 | PruebasReglasD5.CP122_SeguroPosteriorRechazado | 1 |
| K-7 | PruebasK7K8.CP123_VentanaPrivada | 6 |
| K-8 | PruebasK7K8.CP124_CambioUniforme | 3 |

RN-26 contrata una suscripción real, la deja vencida mediante el fixture y verifica repetidamente que no exista una activa y que abrir la pantalla no cree ni renueve el registro. RN-28 confirma una compra sin seguro, intenta confirmarla nuevamente agregándole seguro y exige el motivo exacto «La compra ya no está pendiente de pago.». También verifica persistencia sin seguro, sin póliza ni segundo pago. Se quitaron RN-26/RN-28 de los comentarios de CP-77/79/80 que no las probaban. Avisado al quedar verdes: cobertura 39/41 = 95,1 %.

CP-104 conserva una fila: 100 rondas, 10 hilos dedicados y barrera simultánea por ronda. Exactamente un ingreso válido y nueve YaUsado en cada ronda, con comprobación persistida. Los 1.000 intentos pasaron sin cambiar el SQL de ingreso; no apareció un defecto que requiriese reparación. Evidencia: TestResults/Seguridad/atomicidad-100x10.csv.

K-7: el listado usa el propietario de la sesión y la ventana temporal configurada. Registrar y contar también rechazan eventos ajenos. Prueba negativa de otro administrador, rechazo sin sesión y cuatro bordes: 18:29:59, 18:30:00 del día anterior al evento de 00:30, cierre exacto y un segundo después. Se usa un reloj interno controlado por la prueba para verificar límites exactos.

K-8: el cambio de contraseña es accesible sin sesión. Se uniformó el rechazo a «Mail o contraseña actual incorrectos.», incluyendo cuenta bloqueada y correo inexistente, sin revelar intentos restantes. CP-96 antes exigía «bloqueada»; ahora exige ese texto genérico por el cambio explícito de requisito. CP-124 compara el diálogo visible, título, permanencia de la ventana y ausencia de sesión con 0, 4 y 5 fallos previos. El bloqueo interno continúa funcionando.

232 + 11 = 243. Se avisó el número al cerrar el punto 2 y no se añadieron casos después. Solo se ampliaron comprobaciones dentro de CP-119/120 existentes.

## Prioridad real: decisión pendiente, no implementada

Hoy la prioridad ordena la lista consultada; no reserva el turno de compra. No se puede fechar una ventana de minutos con un parámetro y un guard solamente: no se persiste el instante de liberación del cupo. Estado de entrada y monto reembolsado no sustituyen ese instante.

Hace falta como mínimo persistir un instante de liberación y actualizarlo en los caminos que liberan capacidad, más el parámetro y la verificación de compra. Podría ser una columna; no necesariamente otra tabla. Ese cambio excede la condición autorizada de parámetro más verificación. No se creó tabla, entidad, proceso ni un parámetro usado como estado por evento. No corresponde afirmar exclusividad temporal hasta resolverlo. Se comunicó antes del rediseño.

## Pantallas

Primera tanda, siete: Confirmación, Inicio, Crear usuario, Cambiar contraseña, Menú cliente, Pago y Control de ingreso.

Segunda tanda, ocho: realizadas Mis locales, Eventos del local, Registrar local, Agregar sector, Administrar evento y Listas de espera. Marcadas sin rediseñar Historial (dos grillas vinculadas) y Reportes (filtros condicionales y lectura variable): no encajan directamente en las cinco plantillas aprobadas. Se conserva su estado anterior para decisión del usuario.

Total: 13 realizadas de las 15; 2 marcadas. Confirmación está incluida en las 15, no se cuenta dos veces. Con las cinco anteriores hay 18 pantallas con el patrón aplicado y dos pendientes.

Confirmación muestra paso 4, éxito con Segoe MDL2 Assets, pedido, TOTAL violeta, detalle desplazable y documentos con iconos de la misma fuente Windows. La revisión de capturas encontró y corrigió defectos propios de altura del detalle y de la lista de PDF a 150 %. Se mantuvieron medidas en Designer y tipografía/color en tema. Registrar local y Agregar sector reciben los bordes de campos del patrón, sin estilos de fuente o color serializados.

Después de cada pantalla se ejecutó BarreraDiseno: CP-63 en 20 pantallas, siete filas del host de serialización, CP-120 y CP-116, total 49/49, más color nativo de 31 primarios en 20 pantallas con control negativo. Nunca hubo una barrera roja. Las correcciones finales también pasaron la barrera.

## Evidencia y límites

- Punto 2: TestResults/Cola-punto2.log, 243/243.
- Primera tanda: TestResults/Cola/tanda1.log, 243/243.
- Cierre: TestResults/Cola/final.log.
- Barreras por pantalla y retoques: TestResults/cola-*.log (sin contar *-build.log).
- Botones nativos: TestResults/Botones/nativo-100.log y nativo-150.log: 20 pantallas, cero fallos, DPI 96 y 144 reales.
- Capturas antes: TestResults/Cola/antes-100/ y antes-150/.
- Capturas después: TestResults/Cola/despues-100/ y despues-150/.
- PNG con sufijo -escritorio: píxeles de escritorio. Otra ventana concurrente tapa parte de varias imágenes; no sirven como evidencia despejada en esos casos.
- PNG sin ese sufijo: DrawToBitmap del formulario al DPI nativo; conserva el formulario completo, pero puede tener artefactos de repintado de controles Windows. No es una captura de escritorio. Se entregan ambas variantes, sin ocultar la diferencia.
- ventanas.txt registra DPI, área cliente y tamaño externo. desglose.txt registra geometría y contenido del comprobante.

Las capturas son evidencia técnica local, no una aprobación estética de Juan Cruz. Quedan pendientes la aprobación visual y una captura de escritorio despejado para las imágenes tapadas. No se pidió aprobación para continuar las pantallas autorizadas.

No se tocaron docs/, tools/ ni App.config. El archivo nuevo de mutación en docs/ pertenece al trabajo paralelo y queda fuera de nuestro alcance. Índice vacío al cierre; ambos App.config siguen S.

## Vueltas y portones

Se conservan 22 ejecuciones de barrera con logs separados (la primera de Confirmación con 29 filas; se completó enseguida con las 49, antes de continuar). Las demás incluyen 49 casos y la comprobación nativa de color. Además se ejecutaron los dos barridos nativos completos al 100/150 %, las baterías completas por hitos y las pasadas de captura. Esto cuenta ejecuciones verificables; no es un contador exacto de todas las llamadas de lectura/edición. La estimación inicial de 70–110 vueltas de herramienta no equivale a horas de desarrollador.

Portones humanos pendientes: decidir la persistencia necesaria para prioridad temporal; definir adaptación de Historial y Reportes; aprobar el aspecto y coordinar escritorio despejado para las capturas tapadas. No se esperó ninguno para avanzar las tareas independientes.

## Tiempo de reloj

Inicio registrado: 19:07:14 ART. Publicación inicial terminada aproximadamente a las 19:08:04: 0,014 h. Punto 2 y su validación terminaron a las 19:16:47: 0,145 h desde la publicación. El resto corresponde al análisis de prioridad, las tandas visuales, capturas, verificaciones y reporte; no se cronometró cada subpunto por separado y no se inventan horas individuales. Este tiempo es ejecución real de Codex, no horas-persona ni estimación de un desarrollador.

Último cierre de Confirmación: lista de documentos contenida en panel con Dock Fill para que su reajuste nativo de altura no invada el pie; detalle con medida explícita y líneas de importes de 36 caracteres para conservarlos juntos al 150 %. Barrera cola-confirmacion-contenida: 49/49 y 31 primarios renderizados correctamente.
Cierre: 19:51:42 ART; total 0,741 h de reloj. Batería final 243/243, 1,0122 minutos.
