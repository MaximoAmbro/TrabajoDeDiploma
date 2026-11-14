# T-47: mínimo, medición de texto y color renderizado

12/11/2026. Codex. Continuación del avance T-47/T-48. **No cerrado: propuestas de ancho pendientes de autorización individual y conflictos de geometría que impiden cerrar solo ensanchando.**

## Decisión aplicada

Se retiró MinimumSize de Tema.PrepararBoton. BotonTematico deja de ocultar MinimumSize y de impedir su serialización: ahora el Designer puede guardar un mínimo explícito si lo decide el rediseño. No se agregó ningún mínimo global. CP-112 comprueba, en sus tres filas de botones del host de diseño real, mínimo vacío y serialización de un mínimo explícito; los estilos continúan sin serializarse.

Antes de retirar el mínimo del producto se midieron los 14 botones en instancias temporales: MinimumSize vacío y Size declarado convertido de los DPI heredados a 96 DPI de esta sesión. Se mantuvo la fuente y el padding efectivos del componente. No se alteró ningún alto ni posición del Designer.

## Los 14 resultados antes de los cambios de ancho

Unidades: píxeles efectivos a 96 DPI. Ancho requerido = GetPreferredSize con AutoSizeMode.GrowAndShrink, incluidos padding y borde; es un ancho suficiente del motor de layout, no solo el ancho de los glifos. El texto se comprobó además renderizando cada botón a su tamaño disponible y a su tamaño preferido. Comparación de tinta de texto, incluidos píxeles antialias, sin el borde exterior; inspección de capturas confirma las truncaciones. Comparar solo el tamaño preferido habría marcado incorrectamente los 14 como recortados por alto: no se usó ese criterio para la conclusión.

| Pantalla | Control | Texto | Ancho disponible | Ancho requerido | Alto disponible | Texto original |
|---|---|---|---:|---:|---:|---|
| frmEventosCliente | btnBuscar | Buscar | 63 | 82 | 21 | Cortado |
| frmEventosCliente | btnLimpiar | Limpiar | 63 | 90 | 21 | Cortado |
| frmHistorialCliente | btnVolver | Volver | 90 | 89 | 29 | Completo |
| frmSuscripcion | btnVolver | Volver | 90 | 89 | 29 | Completo |
| frmAdministrarEvento | btnVolver | Volver | 90 | 89 | 22 | Completo |
| frmEventosLocal | btnEliminarEvento | Eliminar evento | 162 | 144 | 28 | Completo |
| frmEventosLocal | btnVolver | Volver | 90 | 89 | 22 | Completo |
| frmEventosLocal | btnAdministrarEvento | Administrar Evento | 162 | 166 | 28 | Cortado |
| frmEventosLocal | btnAgregarEvento | Agregar Evento | 162 | 144 | 28 | Completo |
| frmListasEspera | btnVolver | Volver | 90 | 89 | 29 | Completo |
| frmMisLocales | btnVolver | Volver | 86 | 83 | 21 | Completo |
| frmReportes | btnGenerar | Generar | 93 | 96 | 27 | Cortado |
| frmReportes | btnExportar | Exportar PDF | 93 | 128 | 27 | Cortado |
| frmReportes | btnVolver | Volver | 90 | 89 | 29 | Completo |

Son **5 botones recortados**, no más de cinco: Buscar, Limpiar, Administrar Evento, Generar y Exportar PDF. Los otros nueve muestran el texto completo.

## Diffs de ancho preparados para autorización uno por uno

Estos cinco cambios quedan en el árbol como propuestas. Altos, posiciones, DPI y textos se conservaron.

Visual/Cliente/frmEventosCliente.Designer.cs:
```diff
-            this.btnBuscar.Size = new System.Drawing.Size(95, 32);
+            this.btnBuscar.Size = new System.Drawing.Size(123, 32);
-            this.btnLimpiar.Size = new System.Drawing.Size(95, 32);
+            this.btnLimpiar.Size = new System.Drawing.Size(135, 32);
```

Visual/Propietario/frmEventosLocal.Designer.cs:
```diff
-            this.btnAdministrarEvento.Size = new System.Drawing.Size(202, 35);
+            this.btnAdministrarEvento.Size = new System.Drawing.Size(208, 35);
```

Visual/Propietario/frmReportes.Designer.cs:
```diff
-            this.btnGenerar.Size = new System.Drawing.Size(116, 34);
+            this.btnGenerar.Size = new System.Drawing.Size(120, 34);
-            this.btnExportar.Size = new System.Drawing.Size(116, 34);
+            this.btnExportar.Size = new System.Drawing.Size(160, 34);
```

La medición de la propuesta da 82, 90, 166, 96 y 128 px efectivos respectivamente. Buscar, Administrar Evento, Generar y Exportar muestran el texto completo al renderizarse individualmente con esos anchos. **Limpiar conserva un recorte vertical de la p:** a 90×21 conserva 54252 unidades de tinta frente a 55378 de la referencia. Requiere 24 px efectivos de alto; equivalen a 36 en su Designer a 144 DPI, frente a los 32 actuales. No se cambió su alto porque solo se autorizó preparar anchos.

### Conflictos adicionales de disposición

La revisión de las posiciones heredadas muestra que ensanchar no basta para disponer estos controles:
- Buscar empieza en x=720 y el nuevo ancho 123 termina en 843; Limpiar empieza en 821: se solapan 22 unidades del Designer.
- Limpiar termina en 956 (821+135), pero la ventana tiene ancho 928: desborda 28 unidades.
- Exportar termina en 1012 (852+160), pero Reportes tiene ancho 980: desborda 32 unidades.

No se movieron controles ni se agrandaron ventanas para sortearlo. Por tanto, **no es correcto autorizar ciegamente los cinco anchos como solución final**. Administrar Evento y Generar son propuestas sin esos conflictos; Eventos del cliente y Exportar requieren una decisión sobre alcance de disposición. Si corresponde al rediseño, queda para T-38/39/40, sin iniciarlo ahora.

## Color renderizado

Prueba reproducible en Pruebas/VerificarBotonesT47.ps1. Compila un ejecutable de medición temporal/ignorado junto al binario para resolver dependencias; no modifica configuración ni consulta bases. Instancia las 19 pantallas originales sin ejecutar sus Load de datos. Dibuja cada botón nativo con DrawToBitmap (WM_PRINT), recorre todos los píxeles y exige que más del 50 % de la superficie coincida con Tema.Primario, tolerancia de 2 unidades por canal. No aprueba leyendo BackColor.

**30 primarios habilitados: 30 correctos, cero fallos de color.** Listas de espera no tiene primarios y se registra NO_APLICA. Devolver y Exportar se verifican habilitados: originalmente están deshabilitados y su gris es deliberado. La prueba no convierte ese estado gris en un error. UseVisualStyleBackColor es false en los 30; el tema ya lo asignaba explícitamente y no fue necesario corregirlo.

Control negativo: un Button estándar con estilo del sistema activado y BackColor violeta no supera la comprobación de píxeles violetas. Su imagen queda junto con las capturas de los 30 primarios. Se inspeccionaron capturas de Inicio, Exportar y las comparaciones de Buscar, Administrar y Generar.

Evidencia ignorada:
- TestResults/T47/medicion-botones/medidas.csv: medición original antes de retirar el mínimo del producto.
- TestResults/T47/medicion-botones-propuesta/medidas.csv: después de los cinco anchos propuestos, incluyendo alto mínimo observado.
- TestResults/T47/medicion-botones-propuesta/colores.csv: 19 pantallas, 30 primarios, píxeles y proporciones.
- PNG de los controles a tamaño disponible y de referencia, y control-negativo-sistema.png.

Reproducir, después de compilar:
```powershell
powershell -NoProfile -STA -ExecutionPolicy Bypass -File Pruebas/VerificarBotonesT47.ps1 -Salida TestResults/T47/medicion-botones-propuesta
```

El script informa por separado recortes y fallos de color; el estado de salida exige inventario completo y color correcto. Que salga 0 no significa que el texto que el CSV marca recortado esté resuelto.

## Batería y estado

**184 ejecuciones, 180 correctas, 4 fallidas**, 44,8574 s. Log TestResults/2026-11-12-t47-sin-minimo.log. CP-112 siete filas correctas con MinimumSize editable/serializable. CP-13, CP-25 y CP-54 permanecen correctas sin modificar esas pruebas. Los cuatro fallos son los mismos previos: CP-85, CP-81, CP-69 y CP-76.

La prueba auxiliar de píxeles no agrega filas MSTest. No se amplió el rediseño ni se corrigieron otros defectos. Sin docs/, App.config, commit ni push. Ambos App.config mantienen S de skip-worktree. Se conservaron los cambios paralelos de Claude.

Archivos de esta continuación: Visual/Tema.cs; Visual/Componentes/BotonTematico.cs; los tres Designer cuyos diffs de ancho se muestran arriba; Pruebas/PruebasSerializacionComponentes.cs; Pruebas/VerificarBotonesT47.ps1 y este reporte. Los cambios anteriores de T-47/T-48 siguen sin commitear.

## Horas reales

Desde la marca inicial 20:56:17 hasta 21:04:49 ART: **0.1425 h** en esta continuación (medición, retiro del mínimo, propuestas, pruebas y reporte). Tiempo de Codex; no es una estimación humana. No incluye el avance anterior de aproximadamente 0,12 h.

**Detenido:** pendientes las autorizaciones individuales y la decisión sobre el alto de Limpiar, los solapamientos y los desbordes; no se presenta T-47 como cerrada.
