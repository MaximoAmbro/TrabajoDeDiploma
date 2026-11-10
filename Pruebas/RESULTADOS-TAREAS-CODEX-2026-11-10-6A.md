# Sexta tanda 6A terminada: diagnóstico, piloto y ticket nuevo

Fecha: 10/11/2026. Codex para JC y Claude Code. Base: `c24db93`.
Este reporte reemplaza el estado parcial anterior del mismo día. La regresión de CP-25 fue introducida por mi rediseño y se corrigió como parte del trabajo, conforme a la aclaración permanente de JC. No era un defecto preexistente.

**Resultado final: 77 ejecuciones correctas, cero fallidas y cero omitidas.** Implementación de 6A terminada; aprobación visual del piloto y apertura en el diseñador siguen pendientes de JC/Máximo. No se inició 6B. Sin commits ni push.

## 1. Causa raíz de CP-25, medida antes de modificar código

Se ejecutó un programa de diagnóstico contra la compilación anterior, con la base de pruebas, paquete máximo y las escalas geométricas. Se compararon límites de controles, área útil y desplazamiento, y se probaron dos controles experimentales: quitar MinimumSize y vaciar la etiqueta.

**El error era la composición del layout:** una tarjeta de resumen con dimensiones fijas mayores que la vista, dentro de otro contenedor desplazable de ancho también fijo. `ScrollControlIntoView` alinea hacia arriba una tarjeta más alta que su vista; no garantiza mostrar su borde inferior. El contenido además sumaba AutoScrollMargin a un ancho que ya incluía márgenes y provocaba desplazamiento horizontal innecesario.

| Medición anterior | 100 % | 125 % simulado | 150 % simulado, área 1366 × 728 |
|---|---:|---:|---:|
| Alto útil del contenido, con barras | 467 | 588 | 592 |
| Alto fijo de la tarjeta | 496 | 620 | 744 |
| Alto del documento completo | 380 | 474 | 569 |
| Borde inferior tras ScrollControlIntoView | 520 | 644 | 768 |
| Borde inferior tras desplazamiento manual al final | 443 | 564 | 568 |

Esto distingue un mal uso del espacio asignado a la tarjeta de la imposibilidad de leer el documento: el desplazamiento manual sí alcanzaba el final. CP-25 detectó que el recorrido automático no lo hacía.

Resultado de las hipótesis de JC:

- **Fuentes mayores:** no fueron la causa de esta regresión; el documento de 380 px cabía dentro de los 496 de tarjeta al 100 %. El fallo persistía con el texto completamente vacío.
- **Pie superpuesto:** descartado; contenido y pie tenían límites contiguos, sin intersección, en las tres mediciones.
- **AutoScroll sin recalcular al crecer el texto:** no fue la causa observada; había rango de desplazamiento y el desplazamiento manual llegaba al final.
- **MinimumSize de la etiqueta:** descartado como causa; quitarlo no cambió el alto de la tarjeta ni el fallo.
- **Causa concreta:** confundir el tamaño fijo de una tarjeta con el espacio disponible de su vista, más el doble cómputo de márgenes que generaba la barra horizontal.

Evidencia anterior: `TestResults/6A/diagnostico-antes.txt`. El diagnóstico fue comunicado a JC antes de corregir el layout.

## 2. ¿Es sistémico? Riesgos para 6B

**La regresión es específica de la implementación del piloto, no de los colores ni de las tipografías del sistema.** No exige bajar Segoe UI ni Consolas. Sí hay un riesgo compartido si en 6B se conserva la geometría fija heredada y se agregan encabezado, pie y márgenes sin redistribuir el contenido.

Estimación por lectura de los otros 18 Designer y su estructura, sin modificarlos ni abrirlos para rediseño. Las dimensiones siguientes son equivalentes a 96 DPI, no nuevas medidas propuestas. Es una evaluación de riesgo, no una lista de defectos ya reproducidos.

| Riesgo | Formularios | Motivo |
|---|---|---|
| Alto | `frmCrearUsuario` | Cinco campos en una ventana equivalente a 315 × 476; rótulos arriba, separación entre campos y acciones compiten por el alto disponible. |
| Alto | `frmPago` | Cuatro campos, importe y temporizador en unos 365 × 390; necesita distribuir contenido y acciones sin superponerlos. |
| Alto | `frmConfirmacion` | Desglose variable, carpeta y acciones en unos 365 × 467; el resumen puede crecer según las entradas y servicios. |
| Alto | `frmHistorialCliente` | Dos grillas en unos 618 × 448; filas de 32 px y encabezados automáticos requieren presupuesto independiente para cada grilla. |
| Alto | `frmEventosCliente` | Filtros y grilla en unos 619 × 329; el encabezado y pie nuevos consumen una proporción grande del alto. |
| Alto | `frmReportes` | Filtros, acciones y grilla en unos 784 × 506; los anchos de filtros y el área de resultados deben repartirse. |
| Medio | `frmRegistrarLocal`, `frmAgregarEvento`, `frmAgregarSector`, `frmCambiarContraseña` | Formularios de aproximadamente 315 px de ancho; revisar campos, fechas, rótulos y enlaces al redistribuir verticalmente. |
| Medio | `frmInicio`, `frmSuscripcion` | Marca/ayudas/enlaces o beneficios y selección de plan; el texto variable y el pie nuevo pueden consumir la reserva actual. |
| Medio | `frmAdministrarEvento`, `frmEventosLocal`, `frmMisLocales`, `frmListasEspera` | Una grilla y varias acciones en ventanas de unos 618 px de ancho y 329–386 px de alto; revisar filas visibles y botones anchos. |
| Bajo | `frmMenuCliente`, `frmMenuPropietario` | Pocos botones y más espacio vertical relativo; aun así revisar Tab y visibilidad del pie en DPI altos. |

No todos los controles van a crecer: los campos actuales en MS Gothic 15,75/16 pt y títulos de 22/28 pt se reducen al aplicar los tokens nuevos. El cuerpo de 8/8,25 pt sí crece a 10 pt. Por eso no sería correcto diagnosticar un desborde general solo por la fuente.

**Recomendaciones para Claude Code en SISTEMA-DISENO.md, sin modificarlo desde esta sesión:**

1. Mantener los tokens actuales. Agregar una regla de layout: primero calcular el área útil después del pie/encabezado; después asignar columnas, tarjetas y vistas de grilla. Convertir a 96 DPI no implica conservar dimensiones insuficientes del contenido heredado.
2. Distinguir documento y vista: una tarjeta con contenido variable puede tener desplazamiento interno; su vista debe caber en el área asignada. No usar ScrollControlIntoView como sustituto de ese contrato si el control es mayor que su contenedor.
3. Aclarar que el título largo puede ampliar el encabezado y que en el piloto el encabezado se desplaza con el contenido. Solo el pie queda fijo.
4. Contabilizar una sola vez los márgenes y reservar el ancho de las barras; evitar que un ancho fijo produzca una barra horizontal por la aparición de la vertical.
5. Definir antes de 6B un criterio para ventanas estrechas y para dos grillas: ancho mínimo de columnas, orden de lectura y cuál región se desplaza. No reducir fuentes para hacer entrar contenido.

Con encabezado 72, pie 60 y márgenes verticales 24 + 24, una ventana de 476 deja aproximadamente 296 px para contenido. Ese presupuesto explica las prioridades anteriores. Es una recomendación de contrato de layout, no un pedido de bajar los tokens.

## 3. Corrección en la capa de presentación

`FrmCompraCliente.AjustarEncabezado` distribuye ahora el conjunto según el ancho útil y el DPI, con protección frente a reentrada de eventos:

- Reparte el ancho entre las columnas y actualiza grilla, casillas y documento.
- Elimina el margen externo duplicado y la barra horizontal innecesaria.
- Calcula el título con su alto preferido; no lo tapa ni recorta un título largo.
- Asigna al resumen una vista limitada por el espacio disponible y su tamaño natural; su documento conserva fuente, contenido y desplazamiento interno cuando hace falta.
- Recalcula al cambiar vista, título o tamaño del documento y después de limitar la ventana al área de trabajo.
- Mantiene el pie separado y fijo, con ambas acciones visibles.

No se alteraron las aserciones de CP-25 ni sus escalas. No se agrandó arbitrariamente la ventana ni se cambió la fuente del resumen para pasar la prueba. El TOTAL conserva exactamente la misma fuente y tamaño que el resto del comprobante; no se cambió el texto ni la lógica de cálculo de ActualizarResumen/Importe.

La captura inicial mediante DrawToBitmap ocultaba etiquetas por el orden de dibujo de controles hermanos superpuestos. Las evidencias finales usan capturas reales de pantalla; el fondo del encabezado queda detrás y las etiquetas usan Superficie.

## 4. Verificación real a 100 %, 125 % y 150 %

`Pruebas/VerificarVisual6A.ps1` compila un ejecutable de evidencia con el mismo manifiesto de Visual, usa únicamente la base `_Pruebas` y Fake, cambia la escala mediante la herramienta existente de Claude y restaura la escala original en finally. No modifica tools/ ni App.config.

| Escala real | DPI de la ventana | Alto útil | Alto de tarjeta asignado | Documento | Resultado |
|---|---:|---:|---:|---:|---|
| 100 % | 96 | 484 | 436 | 380 | Correcto |
| 125 % | 120 | 605 | 555 | 474 | Correcto |
| 150 % | 144 | 726 | 663 | 569 | Correcto |

En las tres: final de tarjeta y documento accesibles, acciones visibles, título completo, líneas de importes sin recortes, sin superposición con el pie y sin barra horizontal externa. Se comprobó el paquete máximo y un nombre largo. Windows quedó restaurado a **100 %**.

Capturas y mediciones: `TestResults/6A/piloto-{100,125,150}-{inicio,resumen}.png` y `piloto-{100,125,150}.txt`. Monitor físico 1920 × 1080. Las variantes de CP-25 mantienen además sus áreas simuladas de notebook y monitor; no se declara haber probado físicamente otra notebook.

Reproducción, después de correr la batería:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File Pruebas\VerificarVisual6A.ps1
```

## 5. Ticket blanco listo para imprimir y escanear

T-36 implementada en `GenerarPDF`, conservando su firma y el contenido original del QR:

- A6 apaisada (420 × 298 pt de iText), papel blanco y franja Primario de 28 pt.
- Nombre Helvetica Bold 16; rótulos Helvetica 8; datos Helvetica 11; código completo Courier 8.
- QR **120 × 120 pt**, a **24 pt del borde derecho**, **64 pt del inferior** y **86 pt de la franja**.
- Colores convertidos desde Tema; ningún rectángulo negro de fondo.
- El nombre aprovecha el ancho superior y los datos quedan a la izquierda del QR. La posición vertical exacta se definió para separar título, datos y código; respeta las distancias mínimas del encargo.

**Archivos disponibles para JC:**

- `TestResults/6A/Tickets/Ticket-nuevo-1.pdf`
- `TestResults/6A/Tickets/Ticket-nuevo-2.pdf`

Son dos entradas de una compra confirmada creada en la base de pruebas, no compras de la base de trabajo. Sirven para la prueba de impresión/lectura; Fake limpió esos datos después de generar los documentos. No se escribieron tickets de esta verificación en Documentos.

Se inspeccionó la representación del PDF y se midieron imagen y separaciones. El fondo blanco se verificó también por píxel. `tools/VerificarQrTickets.py` dio **dos OK**: un QR por PDF, contenido igual al código impreso y códigos distintos. Evidencias: `TestResults/6A/qr-tickets.json`, `tickets-medidas.json` y las imágenes PNG junto a los PDF. Estos archivos están ignorados por Git, pero permanecen disponibles localmente.

Imprimir al **100 % / tamaño real**, sin ajustar al papel. El escaneo óptico con el celular sigue pendiente de JC para cerrar JC-R10; la decodificación por software no lo reemplaza.

## 6. Batería y pruebas nuevas

Comando: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar`.

- Antes del rediseño y después de T-30, T-31, T-32, T-33 y T-34: 73/73.
- Primera implementación de T-35: 70/73; regresión propia en las tres filas de CP-25.
- Después de corregir el layout: 73/73.
- Después de T-36: 73/73.
- T-37 completa y última corrida: **77/77**, tiempo **6,9753 segundos**, sin advertencias nuevas.

| Caso nuevo | Comprobación |
|---|---|
| CP-62 | Contraste WCAG de texto y estados, borde mínimo 3:1, colores nombrados y Name real de las ocho fuentes. |
| CP-63 | Piloto y Designer sin colores/fuentes propios; AutoScaleDimensions a 96F. |
| CP-64 | Proyecto y manifiesto declaran DPI de sistema, sin SetProcessDPIAware en Program. |
| CP-65 | PDF en Path.GetTempFileName con entrada confirmada de Fake; una página y texto completo de evento, sector, pedido y código; se elimina al final. |

Pruebas existentes modificadas, únicamente por V-6: CP-20 espera «Revisá los datos» y CP-24 «No se puede armar el paquete». Textos y demás aserciones se conservan. Ninguna otra prueba existente fue debilitada ni marcada Ignore.

El proyecto de pruebas necesitó `itext` y su adaptador `itext.bouncy-castle-adapter`, ambos 9.2.0 como Visual, para ejecutar realmente la nueva generación de PDF en su proceso. El primer intento sin adaptador falló en ese entorno de prueba; quedó resuelto en Pruebas.csproj. No se modificaron dependencias de Visual ni se catalogó como defecto preexistente de la aplicación.

## 7. Archivos pendientes de revisión de JC

| Archivo | Qué contiene |
|---|---|
| `Visual/Tema.cs` | Tokens, fuentes, espaciado, estilos, normalización, icono seguro y medidas del piloto/ticket. Colores Firebrick/Gainsboro nombrados; BordeControl #8E8A9C. |
| `Visual/Componentes/BotonPrimario.cs` | Button concreto primario con estados, mínimo, padding y foco. |
| `Visual/Componentes/BotonSecundario.cs` | Button concreto secundario con borde y estados. |
| `Visual/Componentes/BotonPeligro.cs` | Button concreto de peligro; preparado para 6B, sin aplicarlo a otros formularios. |
| `Visual/FormularioEventAura.cs` | Tema en constructor y normalización antes del Load, con guarda de diseño; conserva base concreta y cierre de T-24. |
| `Visual/app.manifest` | DPI de sistema, compatibilidad Windows 10/11 y asInvoker. |
| `Visual/Recursos/EventAura.ico` | E blanca en Segoe UI Semibold sobre Primario, 16/24/32/48/256 px; PNG interno incluido el de 256. |
| `Visual/Avisos.cs` | Mensaje de regla con título en castellano y código al final. |
| `Visual/Cliente/FrmCompraCliente.Designer.cs` | Único Designer editado: 96 DPI, tarjetas, botones del sistema, fuentes y colores desde Tema, Esc para volver y sin AcceptButton. |
| `Visual/Cliente/FrmCompraCliente.cs` | Layout adaptable corregido, grilla, avisos V-6, separadores y ticket blanco. |
| `Visual/Visual.csproj` | ApplicationManifest, ApplicationIcon, Compile de Tema/Avisos/tres botones, EmbeddedResource del ICO y None del manifiesto. |
| `Pruebas/Pruebas.csproj` | Referencias iText y adaptador 9.2.0 para CP-65. |
| `Pruebas/PruebasFormularios.cs` | CP-62 a CP-64 y cálculo de contraste. |
| `Pruebas/PruebasInterfaz.cs` | CP-65 y los dos títulos V-6 detallados. |
| `Pruebas/GenerarIcono6A.ps1` | Script System.Drawing reproducible del ICO; usa los colores de Tema compilado. |
| `Pruebas/VerificacionVisual6A.cs` | Herramienta de medición y capturas reales; genera dos PDF con datos de prueba. No agrega casos MSTest. |
| `Pruebas/VerificarVisual6A.ps1` | Compila la herramienta con el manifiesto y ejecuta las tres escalas/restauración y tickets. |
| Este reporte | Diagnóstico, riesgo para 6B, evidencia y pendientes. |

`label4` estaba vacía y sin código asociado: **se conserva**, vacía e invisible. ClientSize y AutoScaleDimensions conservan sus declaraciones literales del Designer para CP-12 y el contrato explícito de 96 DPI. El tamaño inicial se convirtió de 1100 × 680 a 880 × 544; el layout no depende de agrandar esa ventana para pasar CP-25.

Se verificó el manifiesto incrustado en RT_MANIFEST #1 de Visual.exe, no solo el archivo fuente. El ICO tiene las cinco entradas pedidas. Para regenerarlo después de compilar: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File Pruebas\GenerarIcono6A.ps1`.

## 8. Límites y entrega a Claude Code

No quedaron defectos preexistentes nuevos identificados por las cuatro pruebas. Las regresiones propias se corrigieron. No se tocaron Repo/, Datos/, BaseDatos/, docs/, tools/, CLAUDE.md ni AGENTS.md. **No se modificó ninguno de los otros 18 formularios ni sus diseñadores.** La normalización en ejecución desde la base común es la pedida en T-32, no la ejecución de 6B.

Los dos App.config siguen intactos y marcados `S` por skip-worktree. `git diff --check` no encontró errores; Git avisa de normalización futura LF/CRLF en FormularioEventAura.cs.

Próximo turno de Claude Code:

1. Revisar código/reporte y trasladar el reporte a docs/. Actualizar coordinación, CP-62 a CP-65, cifras de 77 ejecuciones y trazabilidad.
2. Evaluar las recomendaciones del apartado 2 antes de 6B; no hacen falta cambios de paleta ni reducción de fuentes.
3. Revisar/usar las evidencias locales y adaptar sus herramientas al manifiesto de DPI; regenerar figuras después de la aprobación visual.
4. Pedir a Máximo abrir `FrmCompraCliente` en el diseñador de Visual Studio a 100 %, sin guardar cambios de escala. No se declara aprobada esa comprobación por CP-12.
5. JC aprueba capturas e imprime/escanea los dos tickets para cerrar JC-R10.
6. Solo después de las dos aprobaciones, anotar «piloto confirmado». Hasta entonces **6B sigue bloqueada**.

Codex termina aquí. Sin commits ni push; JC debe revisar git status antes de autorizar un commit.
