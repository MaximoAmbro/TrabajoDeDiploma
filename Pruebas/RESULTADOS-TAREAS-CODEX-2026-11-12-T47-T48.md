# T-47 y T-48: avance detenido

12/11/2026, Codex. **Trabajo parcial; no cerrado ni listo para commit.**

## Bloqueo comprobado

Los componentes del piloto imponen MinimumSize de 120 × 36 (primario) y 120 × 32 (secundario). Al sustituir los tipos, 14 botones de 8 pantallas se agrandan aunque Size permanezca idéntico en el Designer. Eso contradice la condición de conservar medidas en T-47.

Reproducción con el constructor real de frmEventosCliente, sin cargar datos: btnBuscar declara 95 × 32 a 144 DPI; en ejecución a 96 DPI termina en 80 × 24, exactamente el mínimo del componente escalado, en lugar de aproximadamente 63 × 21. No se cambió el componente ni se introdujeron excepciones para sortearlo. JC debe decidir cómo compatibilizar el mínimo del patrón con la geometría heredada.

| Pantalla | Botones con medidas inferiores al mínimo |
|---|---|
| Eventos del cliente | btnBuscar 95×32; btnLimpiar 95×32 |
| Historial | btnVolver 113×36 |
| Suscripción | btnVolver 113×36 |
| Administrar evento | btnVolver 112×28 |
| Eventos del local | btnEliminarEvento, btnAdministrarEvento y btnAgregarEvento 202×35; btnVolver 113×28 |
| Listas de espera | btnVolver 113×36 |
| Mis locales | btnVolver 129×31 |
| Reportes | btnGenerar y btnExportar 116×34; btnVolver 113×36 |

CP-63 y las pruebas del host comprueban estilo y serialización, no esta conservación geométrica. Sus resultados verdes no cierran el bloqueo.

## Los cinco números (avance parcial)

| Qué | Estimado | Observado |
|---|---|---|
| Horas T-47 + T-48 | 7–10 | Aproximadamente 0,12 h medidas desde la primera marca horaria; no es duración de tareas completas |
| Fuentes y colores | 314 | 266 asignaciones reales + 48 UseVisualStyleBackColor = 314; además 43 FlatStyle/FlatAppearance: 357 líneas retiradas |
| Pantallas | 18 | 18 Designer editados, pendientes del bloqueo |
| Estilo en ejecución | 23 líneas en 5 pantallas | Las 23 líneas originales sustituidas o eliminadas en las cinco |
| Filas nuevas | +18 | +18: CP-63 pasó de una prueba a 19 filas; batería 184 |

El 314 original incluía UseVisualStyleBackColor. No eran 314 asignaciones reales de Font y colores. No se puede validar aún la estimación de 7–10 horas con este avance detenido.

## Cambios realizados

T-47: 23 EtiquetaTitulo (antes ≥18 pt), 3 EtiquetaSeccion (12–17 pt), 19 EtiquetaRotulo (negritas chicas), 28 BotonPrimario (violeta) y 14 BotonSecundario. Las etiquetas sin fuente propia siguen heredando; no se las redujo automáticamente a ayuda. El desglose de Confirmación conserva la fuente monoespaciada con Tema.ComoDocumento en el constructor.

Comparación automatizada contra HEAD: Location, Size, MinimumSize/MaximumSize existentes, ClientSize, DPI, AutoScaleMode, TabIndex, Text, Dock/Anchor y eventos idénticos en los 18 Designer. Esto garantiza el texto fuente, pero no impide el agrandamiento implícito descrito arriba. No se cambió el Designer del piloto ni el de Control de ingreso, añadido después de la auditoría original.

T-48:
- Historial y Listas de espera eliminan su paleta local y llaman Tema.EstiloGrilla.
- Mis locales y Reportes sustituyen las sobrescrituras por Tema.EstiloGrilla.
- Tema.FilaNoReembolsable conserva el significado gris de las entradas no reembolsables.
- Devolver conserva Enabled y el fondo deshabilitado mediante EnabledChanged del componente; se retiran los colores locales.
- Tema.FilaTotal usa una fuente compartida en negrita (Tema.Totales) y PrimarioSuave, sin crear fuentes por enlace de datos.
- Tema.ColorTemporizador conserva rojo con menos de un minuto y primario en el resto.
- Los FillWeight de Eventos del cliente y las demás grillas se conservan.
- CP-63 cubre .cs y Designer de las 19 pantallas originales. Solo exige 96 DPI para el piloto; el resto conserva sus DPI hasta el rediseño.

## Verificación

Primera pasada T-47: **166 / 161 / 5**, 48,1874 s, TestResults/2026-11-12-t47.log. Después se quitaron los nueve colores de grilla y los seis flags de checks/radios restantes, y se preservó el documento de Confirmación; la batería siguiente incluye esos ajustes.

T-47 + T-48: **184 / 180 / 4**, 48,2491 s, TestResults/2026-11-12-t48.log. Se corrigieron escapes en la expresión regular de CP-63 tras esa corrida; recompilación y comprobación final **19/19**, 711 ms, TestResults/2026-11-12-t48-cp63.log. Los cambios posteriores son mensajes/comentarios de esa prueba y este reporte; no cambió el producto. No se repitió toda la batería después de esa corrección de la prueba.

CP-13, CP-25, CP-54, CP-22/23, CP-39 y las siete filas de CP-112 siguen pasando sin modificar esas pruebas. CP-88 pasó al retirar la fuente heredada de Reportes, sin rediseño ni cambio de prueba. CP-71 pasó como consecuencia directa de quitar la sobrescritura exigida por T-48. No se trabajó por separado para arreglar esos fallos.

Quedan cuatro fallos previos: CP-85 menú de propietario al 150 %, CP-81 preventa rechazada que deja visible la compra, CP-69 Agregar evento al 150 %, CP-76 beneficios de suscripción recortados. CP-81 describe comportamiento, no solo aspecto visual.

Compilación correcta; git diff --check correcto. Sin commit ni push. Sin rediseño. App.config conserva S de skip-worktree. Aparecieron archivos de diagramas y herramientas del otro agente; no se tocaron. Árbol limpio al empezar, HEAD ee25328; origin/main y origin/JC coincidían.

## Horas y pendiente

Primera marca guardada: 19:37:49 ART. Primera batería finalizó 19:39:43: **T-47 0,0317 h** hasta esa pasada. T-48, análisis, verificación y reporte hasta aproximadamente 19:45: **0,09 h**. Total aproximado medido **0,12 h**. La lectura anterior a la primera marca no quedó cronometrada; no se inventa su duración. Son tiempos parciales.

**Detenido:** falta decisión de JC sobre mínimos y geometría. Luego corresponde probar la conservación real de medidas y repetir la batería. Claude puede trasladar este estado a docs/; Codex no escribió allí.

## Archivos

Además de los Designer de la tabla siguiente: Visual/Cliente/frmConfirmacion.cs, frmHistorialCliente.cs, frmPago.cs; Visual/Propietario/frmListasEspera.cs, frmMisLocales.cs, frmReportes.cs; Visual/Tema.cs; Pruebas/PruebasFormularios.cs y este reporte. Ningún componente del piloto fue modificado.

| Designer | Roles asignados |
|---|---|
| Visual/Cliente/frmConfirmacion.Designer.cs | lblTitulo → EtiquetaTitulo, lblPedido → EtiquetaSeccion, btnAbrirCarpeta → BotonSecundario, btnAceptar → BotonPrimario |
| Visual/Cliente/frmEventosCliente.Designer.cs | btnVolver → BotonSecundario, label2 → EtiquetaTitulo, btnComprar → BotonPrimario, btnBuscar → BotonPrimario, btnLimpiar → BotonSecundario |
| Visual/Cliente/frmHistorialCliente.Designer.cs | btnVolver → BotonSecundario, label1 → EtiquetaTitulo, btnDevolver → BotonPrimario |
| Visual/Cliente/frmMenuCliente.Designer.cs | btnEventos → BotonPrimario, btnHistorial → BotonPrimario, btnSuscripcion → BotonPrimario, label1 → EtiquetaTitulo, lblvolver → EtiquetaRotulo, lblNombre → EtiquetaTitulo |
| Visual/Cliente/frmPago.Designer.cs | lblTitulo → EtiquetaTitulo, lblTemporizador → EtiquetaSeccion, lblTotal → EtiquetaSeccion, btnPagar → BotonPrimario, btnCancelar → BotonSecundario |
| Visual/Cliente/frmSuscripcion.Designer.cs | label1 → EtiquetaTitulo, lblEstado → EtiquetaRotulo, btnContratar → BotonPrimario, btnCancelar → BotonSecundario, btnVolver → BotonSecundario |
| Visual/Propietario/frmAdministrarEvento.Designer.cs | btnVolver → BotonSecundario, label1 → EtiquetaTitulo, lblEvento → EtiquetaTitulo, btnAgregarSector → BotonPrimario, btnEliminarSector → BotonPrimario, label2 → EtiquetaTitulo, lblPreventa → EtiquetaRotulo, btnGuardarPreventa → BotonPrimario |
| Visual/Propietario/frmAgregarEvento.Designer.cs | label1 → EtiquetaRotulo, btnRegistrar → BotonPrimario, label2 → EtiquetaRotulo, lblVolver → EtiquetaRotulo, label3 → EtiquetaTitulo, lblCategoria → EtiquetaRotulo, lblPreventa → EtiquetaRotulo |
| Visual/Propietario/frmAgregarSector.Designer.cs | label5 → EtiquetaRotulo, btnregistrarse → BotonPrimario, lblmail → EtiquetaRotulo, label6 → EtiquetaRotulo, label1 → EtiquetaTitulo, lblVolver → EtiquetaRotulo |
| Visual/Propietario/frmEventosLocal.Designer.cs | btnEliminarEvento → BotonPrimario, label2 → EtiquetaTitulo, btnVolver → BotonSecundario, lblNombre → EtiquetaTitulo, btnAdministrarEvento → BotonPrimario, btnAgregarEvento → BotonPrimario |
| Visual/Propietario/frmListasEspera.Designer.cs | label1 → EtiquetaTitulo, btnVolver → BotonSecundario |
| Visual/Propietario/frmMenuPropietario.Designer.cs | lblNombre → EtiquetaTitulo, lblvolver → EtiquetaRotulo, label1 → EtiquetaTitulo, btnRegistrarLocal → BotonPrimario, btnMisLocales → BotonPrimario, btnListasEspera → BotonPrimario, btnReportes → BotonPrimario |
| Visual/Propietario/frmMisLocales.Designer.cs | label2 → EtiquetaTitulo, btnVolver → BotonSecundario, btnEventosLocal → BotonPrimario |
| Visual/Propietario/frmRegistrarLocal.Designer.cs | label5 → EtiquetaRotulo, btnregistrarse → BotonPrimario, lblyatengocuen → EtiquetaRotulo, lblmail → EtiquetaRotulo, label6 → EtiquetaRotulo, label3 → EtiquetaRotulo, label1 → EtiquetaTitulo |
| Visual/Propietario/frmReportes.Designer.cs | label1 → EtiquetaTitulo, btnGenerar → BotonPrimario, btnExportar → BotonPrimario, btnVolver → BotonSecundario |
| Visual/_Inicio/frmCambiarContraseña.Designer.cs | btnVolver → BotonSecundario, label1 → EtiquetaTitulo, btnconfirmar → BotonPrimario |
| Visual/_Inicio/frmCrearUsuario.Designer.cs | label1 → EtiquetaTitulo, lblyatengocuen → EtiquetaRotulo, btnregistrarse → BotonPrimario |
| Visual/_Inicio/frmInicio.Designer.cs | btninicio → BotonPrimario, label2 → EtiquetaTitulo, btnModificar → BotonSecundario |
