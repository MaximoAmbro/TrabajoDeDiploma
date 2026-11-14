# Tanda del 14/11/2026: publicación, barrido de botones, RF-10 y cinco pantallas

Codex. **232/232 correctas, cero fallidas. T-47 y T-48 cerradas. Cinco rediseños para aprobación visual. E-3 de Confirmación pendiente de aclaración del alcance.**

## 1. Publicación autorizada al empezar

Se mostró git status completo y se guardó antes de commitear en ESTADO-PREVIO-COMMITS-2026-11-14.md. Se verificó el índice y se confirmó explícitamente que no había ningún App.config preparado. Los dos conservaban S de skip-worktree.

Se usaron exclusivamente rutas explícitas de la zona autorizada:
- **2af267a**: sesión inmutable, rechazo uniforme, preventa y tope de descuento (12 archivos).
- **fc9d414**: limpieza de Designer, tema, pruebas y reportes (34 archivos).

Se hicieron push a origin/main y origin/JC. No se tocó respaldo/jc-2026-11-04. Se avisó a JC al terminar: eso liberó el snapshot de seguridad para Claude. Después Claude publicó sus propios commits de documentación/herramientas hasta fe04b1f; contienen los dos commits anteriores. Esta nueva tanda queda sin commitear para revisión.

## 2. Barrido completo, con prueba de regresión

CP-116 recorre las **20 pantallas**, no solo los 14 controles del mínimo. Son **50 botones y 53 textos/estados** al terminar: también incluye las variantes de Devolver y el texto de lista de espera. Los dos enlaces Volver convertidos a botones aumentan el inventario respecto del inicio.

La prueba renderiza con el motor nativo un botón equivalente en fuente, padding y tamaño. Usa borde de foco de 2 píxeles y contraste alto para comparar la tinta completa, incluidos píxeles antialias, contra una referencia sin recorte. No aprueba solo por la propiedad Font ni por GetPreferredSize. GetPreferredSize da el ancho suficiente; el alto mínimo se busca en el renderizado. Se tolera 0,5 % de diferencia de tinta. También se comprueban controles ocultos/deshabilitados que pueden habilitarse durante el uso.

La primera pasada de texto estático falló en 7 pantallas; al incluir el texto variable de Historial apareció una octava. En total se corrigieron **13 botones en 8 pantallas**. No se presentó el primer barrido como completo cuando faltaba esa variante.

### Inventario de recortes antes del arreglo

Píxeles efectivos al 100 %. Necesita ancho incluye fuente, relleno y foco. «Falta alto» se calcula con el mínimo observado en la imagen, no con el alto preferido genérico.

| Pantalla | Control / texto | Ancho disponible | Ancho necesario | Alto disponible / mínimo | Falta alto |
|---|---|---:|---:|---|---|
| frmAdministrarEvento | btnGuardarPreventa: Guardar preventa | 130 | 160 | 32 / 24 | No |
| frmAdministrarEvento | btnVolver: Volver | 90 | 91 | 22 / 23 | Sí |
| frmConfirmacion | btnAbrirCarpeta: Abrir carpeta | 120 | 132 | 35 / 26 | No |
| frmEventosCliente | btnVolver: Volver | 81 | 85 | 22 / 23 | Sí |
| frmEventosCliente | btnBuscar: Buscar | 82 | 86 | 21 / 21 | No |
| frmEventosCliente | btnLimpiar: Limpiar | 90 | 92 | 24 / 26 | Sí |
| frmEventosLocal | btnAdministrarEvento: Administrar Evento | 166 | 170 | 28 / 23 | No |
| frmEventosLocal | btnVolver: Volver | 90 | 91 | 22 / 23 | Sí |
| frmMisLocales | btnVolver: Volver | 86 | 85 | 21 / 23 | Sí |
| frmPago | btnCancelar: Cancelar pago | 120 | 140 | 35 / 26 | No |
| frmReportes | btnExportar: Exportar PDF | 128 | 132 | 27 / 26 | No |
| frmReportes | btnGenerar: Generar | 96 | 100 | 27 / 21 | No |
| frmHistorialCliente | btnDevolver: Devolver 1/6 entrada(s) | 160 | 184 | 29 / 24 | No |

Al 150 % se volvió a medir con Windows realmente al 150 % y un host con el mismo manifiesto de DPI de la aplicación. El testhost de VSTest por sí solo seguía a 96 DPI; no se confundió eso con una verificación a 144. El host nativo exige el DPI esperado antes de ejecutar las mismas 20 filas CP-116. **Resultado final: 20/20 a 96 DPI y 20/20 a 144 DPI.** Windows quedó restaurado al 100 %.

Reproducir después de preparar la base con la batería:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Pruebas/VerificarBotonesTodas.ps1 -Escala 100
powershell -NoProfile -ExecutionPolicy Bypass -File Pruebas/VerificarBotonesTodas.ps1 -Escala 150
```

El script compila un host temporal con manifiesto, usa Pruebas.dll.config generado (no toca App.config), ejecuta las 20 filas, comprueba GetDpiForSystem y restaura la escala en finally. Los CSV individuales quedan en TestResults/Botones/96 y /144. Las clases de prueba sí quedan en el repositorio: no depende de un ejecutable temporal sin fuente.

### Cambios acotados fuera de los cinco rediseños

Además de los cinco rediseños siguientes, solo se cambiaron los botones medidos y los tamaños de ventana necesarios. No se rediseñaron esas otras pantallas.

| Pantalla | ClientSize antes → después | Justificación |
|---|---|---|
| Administrar evento | 773×412 → 773×415 | Volver necesita 3 unidades más de alto; conserva el margen inferior |
| Eventos del local | 773×412 → 773×415 | El mismo caso de Volver, sin mover grillas ni otros controles |
| Mis locales | 928×494 → 928×500 | Volver necesita 6 unidades más de alto |
| Reportes | 1024×632 → 1031×632 | Exportar necesita 7 unidades más de ancho, preservando margen derecho |
| Historial | 773×560 → 805×560 | Devolver pasa de 200 a 232, conservando los 12 de margen derecho |
| Pago | 456×488 → 456×488 | Cancelar pago pasa de 150 a 177; entra sin desplazar Pagar |
| Confirmación | 456×584 → 456×584 | Abrir carpeta pasa de 150 a 167; entra sin desplazar Aceptar |

El detalle de los 12 primeros cambios de botón está en TestResults/Tanda14/cambios-botones.json; Historial se agregó después de ampliar los estados: 200×36 → 232×36. El aumento de ancho es para los textos reales de devolución, no para el texto estático Devolver. Las medidas de los cinco rediseños reemplazan después los ajustes provisionales de Eventos del cliente.

## 3. RF-10: posición y aviso barato, sin subsistema nuevo

Implementado con tablas/campos existentes. **No se agregó tabla, entidad de dominio ni proceso de fondo.**

- ConsultarPosicion exige la sesión del titular. El repositorio aplica ROW_NUMBER sobre toda la lista del evento, ordenando ConSuscripcion DESC, FechaAlta, ID, y solo después filtra al titular. No devuelve datos de otros clientes.
- La compra incorpora un enlace Consultar mi posición para quien está inscripto, incluso si después vuelve a haber cupo. El botón de inscripción sigue deshabilitado tras anotarse, conservando la defensa contra duplicados.
- AvisosAlIngresar exige la misma autorización. Al autenticar al cliente se consultan sus listas y se avisa si hay un sector con capacidad libre, descontando ventas confirmadas **y reservas**. Se excluyen eventos suspendidos, pasados o sin sectores.
- El aviso no asigna entradas ni reserva un lugar. Se recalcula cada vez que el cliente ingresa: puede repetirse mientras siga habiendo disponibilidad. No existe estado de leído ni promesa de envío por correo.
- Los textos de inscripción/duplicado explican cuándo aparece el aviso y cómo consultar la posición.

Pruebas nuevas:
- **CP-117, 4 filas:** posición propia y enlace real; prioridad del suscriptor; posición ajena rechazada; sin sesión rechazado.
- **CP-118, 6 filas:** evento aún agotado no avisa; reembolso libera cupo y aparece el aviso en el login real; avisos ajenos rechazados; sin sesión rechazado; evento suspendido no avisa; reserva que ocupa el último cupo impide un aviso falso.

Los dos últimos controles de privacidad fallarían sin las guardas de sesión porque la consulta devolvería datos/null en vez del rechazo esperado. La posición no se filtra antes de ordenar, y no se usa el perfil como sustituto del titular.

## 4. Primera tanda visual: cinco pantallas

| Pantalla | Antes a DPI de diseño heredado | Nuevo ClientSize a 96 DPI | Qué cambia |
|---|---|---|---|
| Agregar evento, CP-69 | 473×784 a 144 DPI | 416×432 | Formulario compacto por campos; Volver es botón secundario con nombre y manejador conservados; pie con Registrar |
| Suscripción, CP-76 | 394×595 a 120 DPI | 440×448 | Estado y beneficios completos, selección de plan y acciones separadas, sin superposiciones |
| Menú de administración, CP-85 | 473×714 a 144 DPI | 400×448 | Jerarquía de título/nombre, cinco acciones y salida accesible; se conserva Control de ingreso |
| Eventos del cliente, Volver | 996×494 a 144 DPI | 840×488 | Filtros alineados, grilla, pie Volver/Comprar; indicador en paso 1 |
| Compra | 880×544 a 96 DPI | 880×544 | Indicador en paso 2 y TOTAL propio en 18 pt violeta, conservando íntegro lblResumen y su línea TOTAL |

No se comparan las dimensiones heredadas como si todas estuvieran a 96 DPI. Los CSV/ventanas.txt de capturas registran dimensiones efectivas a cada escala. La compra **no creció** para hacer pasar una prueba.

El indicador dibuja las cuatro etapas 1 Elegí / 2 Armá / 3 Pagá / 4 Listo y resalta la actual; el pasado usa el token de éxito. En esta tanda está instalado en Eventos y Compra. Pago no se rediseñó ni recibió el indicador porque no es una de las cinco pantallas.

El TOTAL destacado coincide con el importe del resumen en vacío y con dos entradas. CP-119 también verifica que pueda leerse completo después de desplazar documento y vista. Una reducción inicial de la vista hizo fallar CP-13; se corrigió recuperando espacio para el documento y su desplazamiento, **sin modificar CP-13 ni CP-25 y sin agrandar la ventana**. El encabezado comparte un contenedor con título/nivel/enlace para evitar capas de fondo superpuestas al capturarlo.

### Barrera después de cada pantalla

Después de Agregar evento, Suscripción, Menú, Eventos y Compra se ejecutaron las **20 filas CP-63 y las siete CP-112**: 27/27 en cada paso. No hubo rojo de serialización y no se continuó sobre una barrera fallida.

Se agregaron además CP-120, dos filas de host real para IndicadorPasos y EtiquetaTotal; ninguna serializa Font/ForeColor/BackColor ni impone MinimumSize. La barrera ampliada pasó **29/29** después de los ajustes finales. Pruebas propias de los extras: CP-119, 2 filas. El TOTAL se puede desplazar hasta quedar completamente dentro de los ancestros visibles, no solo estar en el árbol de controles.

### Portón pendiente: E-3 de Confirmación

La lista de esta tanda suma cinco pantallas: Agregar evento, Suscripción, Menú, Eventos del cliente y Compra. La confirmación de éxito con íconos corresponde a **frmConfirmacion**, una sexta. Se preguntó por esa excepción y no llegó respuesta durante el trabajo. Se respetó la prohibición estricta de más de cinco rediseños: **no se implementó E-3 ni se rediseñó Confirmación**. Su único cambio es el botón Abrir carpeta del barrido autorizado.

Tampoco se añadió el indicador de paso 3 en Pago ni el de paso 4 en Confirmación. No se presenta E-1 como implantado en cuatro pantallas cuando solo se habilitaron dos dentro del alcance. Esas extensiones quedan para la decisión de JC, no para una réplica automática sobre otras pantallas.

## 5. Capturas antes/después y método

Hay capturas con datos reales de la base _Pruebas para las cinco pantallas y para Historial. El antes usa el binario compilado del commit publicado fc9d414 en una copia aislada; el después usa el árbol actual. Ambos ejecutan la misma fixture, con los mismos nombres y cantidades. Los parámetros/credenciales locales no se publican ni se mostraron sus contenidos.

Se hicieron dos clases de captura:
1. CopyFromScreen, archivos -escritorio.png: conserva lo que se veía de verdad; otra ventana tapa parte de las pantallas anchas en esta sesión.
2. DrawToBitmap/WM_PRINT del formulario real cargado con datos, archivos .png: permite revisar todo el contenido sin oclusión. El marco puede verse con aspecto clásico en ese renderizado. No se presenta como una fotografía libre de oclusión del escritorio.

Se conservaron ambos métodos. Las capturas de disposición de los otros formularios del barrido no ejecutan su Load de datos; se distinguen de las capturas reales de la tanda.

| Pantalla | Antes 100 % | Después 100 % | Antes 150 % | Después 150 % |
|---|---|---|---|---|
| Agregar evento | [PNG](../TestResults/Tanda14/real-antes-100/frmAgregarEvento.png) | [PNG](../TestResults/Tanda14/real-despues-100/frmAgregarEvento.png) | [PNG](../TestResults/Tanda14/real-antes-150/frmAgregarEvento.png) | [PNG](../TestResults/Tanda14/real-despues-150/frmAgregarEvento.png) |
| Suscripción | [PNG](../TestResults/Tanda14/real-antes-100/frmSuscripcion.png) | [PNG](../TestResults/Tanda14/real-despues-100/frmSuscripcion.png) | [PNG](../TestResults/Tanda14/real-antes-150/frmSuscripcion.png) | [PNG](../TestResults/Tanda14/real-despues-150/frmSuscripcion.png) |
| Administración | [PNG](../TestResults/Tanda14/real-antes-100/frmMenuPropietario.png) | [PNG](../TestResults/Tanda14/real-despues-100/frmMenuPropietario.png) | [PNG](../TestResults/Tanda14/real-antes-150/frmMenuPropietario.png) | [PNG](../TestResults/Tanda14/real-despues-150/frmMenuPropietario.png) |
| Eventos | [PNG](../TestResults/Tanda14/real-antes-100/frmEventosCliente.png) | [PNG](../TestResults/Tanda14/real-despues-100/frmEventosCliente.png) | [PNG](../TestResults/Tanda14/real-antes-150/frmEventosCliente.png) | [PNG](../TestResults/Tanda14/real-despues-150/frmEventosCliente.png) |
| Compra | [PNG](../TestResults/Tanda14/real-antes-100/FrmCompraCliente.png) | [PNG](../TestResults/Tanda14/real-despues-100/FrmCompraCliente.png) | [PNG](../TestResults/Tanda14/real-antes-150/FrmCompraCliente.png) | [PNG](../TestResults/Tanda14/real-despues-150/FrmCompraCliente.png) |
| Historial, solo botón | [PNG](../TestResults/Tanda14/real-antes-100/frmHistorialCliente.png) | [PNG](../TestResults/Tanda14/real-despues-100/frmHistorialCliente.png) | [PNG](../TestResults/Tanda14/real-antes-150/frmHistorialCliente.png) | [PNG](../TestResults/Tanda14/real-despues-150/frmHistorialCliente.png) |

Compra, TOTAL con documento desplazado: [100 %](../TestResults/Tanda14/real-despues-100/FrmCompraCliente-total.png) y [150 %](../TestResults/Tanda14/real-despues-150/FrmCompraCliente-total.png).

Para Administrar evento, Confirmación, Eventos del local, Mis locales, Pago y Reportes: archivos por formulario en TestResults/Tanda14/botones-antes-100, botones-despues-100, botones-antes-150 y botones-despues-150. Los CSV contienen DPI nativo y tamaños. Windows se cambió realmente entre 100 y 150 y se restauró al 100; no se tomó la simulación geométrica de CP-25 como sustituto de esas capturas.

Reproducción: CapturarTanda.ps1 usa CapturasTandaVisual con datos de prueba y un host temporal con el manifiesto de Visual. CapturarCierreT47.ps1 admite lista de pantallas para la fixture sin datos. Ninguno modifica un App.config.

## 6. Verificación final

**232 ejecuciones, 232 correctas, cero fallidas; 52,3584 s.** TestResults/Tanda14/bateria-final-verificada.log.

Cuenta desde la batería publicada de 198: +20 CP-116, +4 CP-117, +6 CP-118, +2 CP-119 y +2 CP-120 = 232. CP-69, CP-76 y CP-85 pasan sin modificar sus archivos de prueba; CP-13, las tres variantes CP-25, CP-54 y CP-81 siguen verdes.

- CP-63: 20/20.
- CP-112: 7/7.
- CP-120: 2/2.
- Barrido nativo a 96 DPI: 20 pantallas, cero fallos.
- Barrido nativo a 144 DPI: 20 pantallas, cero fallos.
- Color renderizado: 31 primarios de 20 pantallas, cero fallos, con control negativo del estilo del sistema.
- Sin mínimos globales en componentes.
- git diff --check correcto.
- Ambos App.config conservan S de skip-worktree. No se prepararon para commit.

No se modificaron docs/ ni tools/. Después de la publicación inicial, lo nuevo queda sin commitear. La base y el escritorio se liberan al entregar este reporte.

## 7. Punto de control y estimación

El cierre de limpieza conserva el recuento: 266 líneas reales de fuente/color +48 UseVisualStyleBackColor +43 de estilo plano, 18 Designer limpiados y 23 líneas de estilo de ejecución en cinco pantallas. CP-63 agregó 19 filas sobre la prueba original del piloto para cubrir 20 pantallas. El barrido completo añadió después CP-116 y las correcciones de 13 botones; no se esconden esos trabajos dentro del recuento inicial de limpieza.

La estimación dada al iniciar esta tanda fue **40–70 vueltas de herramienta**, con portones humanos separados. La ejecución fue del orden de **70 vueltas de orquestación** (conteo aproximado de llamadas, no de comandos internos). Incluye publicación, comparación de binarios, pruebas rojas/verdes, dos escalas, capturas, corrección de estados variables y regresión del resumen. No se la convierte en horas de desarrollador.

Portones humanos:
- JC aprueba o redirige el aspecto de las cinco pantallas; no se replica sobre las otras quince.
- JC decide la excepción de Confirmación para E-3 y, si corresponde, los indicadores de Pago/Confirmación.
- No hubo portón de infraestructura para RF-10: se resolvió sin tabla, entidad ni proceso nuevo.

## Horas reales registradas

Tiempo corrido de Codex de esta sesión: **0.7405 h**, desde 18:13:10 hasta 18:57:35 ART. No son horas-persona estimadas. Las tareas se intercalaron y no hubo cronómetros independientes: no se inventa una precisión por tarea. Los logs guardan los tramos de publicación, barrido, RF-10, cinco barreras y capturas; aproximadamente 1 minuto de publicación, 12 de barrido/correcciones, 5 de RF-10, 8 de implementación visual y el resto de capturas, regresiones, pruebas y reporte. Son atribuciones aproximadas; el total sí procede de las marcas horarias.

**Se detiene aquí la tanda.** T-47/T-48 y RF-10 cerrados; cinco pantallas listas para revisión. E-3 y la extensión del indicador quedan bloqueadas por el límite de alcance, sin detener el trabajo independiente que sí estaba autorizado.
