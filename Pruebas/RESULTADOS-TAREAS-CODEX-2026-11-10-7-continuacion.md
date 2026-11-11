# Grupo A: PA-02 y T-44, con auditoría del tema

Codex para JC y Claude Code, 10/11/2026. Base `36a0d80`, con los cambios del turno anterior todavía pendientes de revisión. Se retomó por autorización directa de JC. No se modificaron documentos de coordinación.

**Resultado final: 113 ejecuciones, 108 correctas y 5 fallidas, cero omitidas.** Son las tres conocidas de T-43 más dos resultados nuevos de T-44. No se alcanzaron las 110 correctas previstas por esas dos discrepancias adicionales. No se ocultó ninguna con Ignore. Sin commits ni push.

## 1. Resultado después de cada parte

| Parte | Ejecuciones | Correctas | Fallidas | Evidencia |
|---|---:|---:|---:|---|
| Después de PA-02 / CP-67 | **97** | **94** | **3** | CP-67 pasa; siguen exclusivamente CP-69 (150 %), CP-71 (color) y CP-75 (texto RN-06). Tiempo 9,5880 segundos. |
| Después de T-44, primera corrida | **113** | **108** | **5** | Los tres anteriores, más CP-76 y la fila sin plan de CP-81. Tiempo 10,9047 segundos. |
| Verificación completa repetida | **113** | **108** | **5** | Mismos cinco resultados, tiempo 10,8410 segundos. |

Se informó a JC el resultado de PA-02 antes de empezar T-44 y el de T-44 antes de terminar la sesión. Todas las compilaciones terminaron sin errores ni advertencias nuevas; las baterías terminan con código 1 por las pruebas fallidas preservadas.

Comando: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar`.

Logs locales: `TestResults/Tandas7/T44-inicial.txt` y `T44-verificacion.txt`.

## 2. PA-02: autorización cumplida sin modificar producción

**PA-02 → CP-67 → fila 3.2.** Se implementó como **una ejecución** con ambos escenarios, no como dos DataRow. Esto conserva el recuento esperado de 113 sin omitir la migración:

1. Se lee la credencial heredada del administrador de la semilla desde la base aislada, sin escribir otro literal en el código. Se autentica dos veces; se comprueban el usuario, la no exposición de la contraseña, la persistencia de un hash válido y que el segundo acceso conserve ese hash. Se restauran contraseña, intentos y bloqueo originales en finally.
2. Una cuenta propia de Fake recibe una credencial generada aleatoriamente en memoria. Se guarda temporalmente en texto plano solo en la base de pruebas y se autentica con el método de producción. Se verifica que fue reemplazada por un hash válido. Fake limpia la cuenta al terminar.

No se agregaron contraseñas literales al repositorio ni a comentarios, ni se escribieron las credenciales generadas en archivos o logs. Las aserciones evitan incluirlas en sus mensajes de error. No se relajó Seguridad, no se expuso ninguna API nueva ni se modificó Repo/ o Datos/.

## 3. T-44: correspondencia y resultados

| PA | CP | Filas de clasificación | Ejecuciones | Resultado |
|---|---|---|---:|---|
| PA-11 | CP-76 | 8.3 | 1 | Falla por recorte de lblBeneficios; textos, precios y estado previos a esa aserción coinciden. |
| PA-12 | CP-77 | 8.4, 8.6 | 2 | Pasan: contrato Mensual, precio/renovación, vencimiento y habilitados; cancelación con No por defecto. |
| PA-13 | CP-78 | 8.5 | 1 | Pasa: dos pantallas, RN-25 con vencimiento y una sola suscripción vigente persistida. |
| PA-14 | CP-79 | 8.7, 8.9 | 2 | Pasan: seguro sin cargo no cambia TOTAL ni pregunta por seguro; sin plan se cobran las dos primas. |
| PA-15 | CP-80 | 8.8, C9.10, C9.11 | 2 | Pasan: pago aprobado, confirmación, ConSeguro/MontoSeguro, póliza gratuita por entrada o ninguna póliza sin seguro. |
| PA-16 | CP-81 | 8.21, 8.22, C9.4, C9.5, C9.8 | 3 | Dos pasan. Falla el retorno tras RN-39 sin suscripción porque queda también una compra visible. |
| PA-17 | CP-82 | C9.2, C9.3, C9.6, C9.7, C9.9 | 5 | Pasan: alta con preventa y publicación, rechazo de fecha posterior sin insertar, administrar con/sin fecha y quitar preventa persistiendo NULL. |

T-44 aporta **16 ejecuciones: 14 correctas y 2 fallidas**. Junto con las 97 ya presentes: 113/108/5. Los pares y recorridos secundarios dentro de una ejecución conservan todas sus aserciones; no se sumaron ejecuciones artificialmente.

Las comparaciones del comprobante normalizan espacios para ignorar el relleno monoespaciado, pero comprueban conceptos e importes completos. No se modificó el comprobante ni el tamaño del TOTAL.

No se generaron tickets ni reportes en Documentos: el ensayo del seguro abre el pago y lo cancela; el de pólizas abre directamente pago y confirmación con datos de prueba. No pasa por la generación de tickets del flujo completo. CP-65, ya existente, mantiene su PDF temporal y limpieza.

## 4. Nuevos resultados, sin corrección

### A-04 — Visual/cosmético: beneficios recortados

- **CP-76 / PA-11 / 8.3**, pantalla `frmSuscripcion`.
- El Label no AutoSize `lblBeneficios` necesita más alto que el disponible para su ancho. Falla la versión falsable de la clasificación, no una valoración subjetiva de apariencia.
- El tamaño se declara en `Visual/Cliente/frmSuscripcion.Designer.cs:69` y el texto se establece en el Load de `Visual/Cliente/frmSuscripcion.cs`.
- Se comprobó primero el estado sin plan, el texto de beneficios, ambos precios obtenidos de Parametros y Cancelar deshabilitado: coinciden. El fallo impide dar por comprobadas las aserciones geométricas posteriores de la misma fila.
- **Clasificación: visual/cosmético.** Se anotó y se continuó; no se tocó la pantalla ni el Designer.

### A-05 — Funcional de navegación: compra visible tras rechazar preventa

- **CP-81 / PA-16 / 8.21**, variante sin plan.
- Preparación: evento propio publicado, FinPreventa mañana a las 20:00 y cliente propio sin suscripción; se selecciona el evento real en el listado y se pulsa Comprar.
- El evento aparece en el listado. Se muestra correctamente el aviso con la fecha y hora de fin; se abre un listado visible. Pero al terminar el clic también queda una instancia visible de `FrmCompraCliente`.
- Esperado por el recorrido de rechazo: regresar al listado sin dejar una compra visible para el cliente rechazado.
- Código a revisar: `Visual/Cliente/frmEventosCliente.cs:127` abre el formulario; `Visual/Cliente/FrmCompraCliente.cs:187` captura la regla y `:190` llama VolverAlListado durante Load. Esa secuencia de apertura es un punto a investigar, no una corrección implementada ni una causa definitiva declarada sin más diagnóstico.
- **Clasificación: funcional de navegación; requiere revisión antes de tratarlo como cosmético.** No se investigó un cobro ni se probó un bypass de RN-39. Los casos con suscripción y sin preventa pasan; no corresponde afirmar una vulnerabilidad ni pérdida de datos a partir de este resultado.
- Se detuvo la implementación y se informó a JC. Solo se completaron lectura de estilos, verificación repetida y reporte. No se reescribió la prueba para aceptar la ventana de compra ni se corrigió producción.

## 5. Los tres conocidos de T-43 siguen intactos

- CP-69 (150 %): Volver llega a y=697, fuera del área cliente que termina en 689.
- CP-71: alternas fijas de Mis locales en lugar de Tema.FilaAlterna.
- CP-75: RN-06 en el título pero no como sufijo del texto.

**La pantalla del botón Volver al 150 % es `Visual/Propietario/frmAgregarEvento`, una de las otras 18. No es el piloto `FrmCompraCliente`.** No se modificó su prueba, su código ni su Designer durante esta continuación.

## 6. Auditoría de estilos para 6B

Inventario detallado con archivos y líneas: **`Pruebas/AUDITORIA-TEMA-2026-11-10.md`**. Inventario local completo de coincidencias: `TestResults/Tandas7/auditoria-tema.json`.

Resultado:

- **18 pantallas** conservan valores fijos de estilo en el Designer; son las pendientes de 6B. El piloto solo conserva los literales estructurales explícitos de DPI y tamaño cliente que exige su contrato.
- **6 pantallas** también asignan valores fuera del tema en código de ejecución: Eventos del cliente, Historial, Pago, Listas de espera, Mis locales y Reportes.
- **4 sobrescriben estilos de grilla después del tema:** Historial, Listas de espera, Mis locales y Reportes.
- El patrón es el formateo de grillas en Load; no son solo formularios de administración. Los otros casos son pesos de columnas y colores de estado del reloj. No toda literal equivale a un fallo ni toda asignación del Designer ocurre después del tema.
- Se informa aparte ExportadorPDF, que no es pantalla y conserva su propia paleta/medidas PDF.

La base normaliza antes de base.OnLoad; por eso las asignaciones posteriores ganan. Para 6B conviene sustituir esas paletas locales por tokens conservando estados de reembolso, deshabilitado, temporizador y TOTAL. Reaplicar el tema ciegamente al final puede borrar esa información. No se cambió ningún estilo ni docs/SISTEMA-DISENO.md.

## 7. Archivos de esta continuación

- `Pruebas/PruebasGrupoALogin.cs`: agrega solo CP-67 a los casos existentes del turno anterior.
- `Pruebas/PruebasGrupoASuscripcion.cs`: CP-76 a CP-80.
- `Pruebas/PruebasGrupoAPreventa.cs`: CP-81 y CP-82.
- `Pruebas/Fake.cs`: agrega ContarSuscripcionesVigentes, consulta parametrizada para la cuenta propia de PA-13.
- `Pruebas/AUDITORIA-TEMA-2026-11-10.md`: inventario para el encargo de 6B.
- Este reporte y una nota en el primer reporte para distinguir su estado histórico del vigente.

No se modificaron pruebas ni código de los tres resultados de T-43. Los archivos InterfazGrupoA y PruebasGrupoAPropietario siguen como estaban al retomar. No se cambió código de Visual en esta continuación: el único diff pendiente de Visual sigue siendo la corrección de cuatro acentos de comentarios autorizada en el turno anterior.

Había cambios ajenos de planificación en docs/ al comenzar; durante la sesión aparecieron además modificaciones en ANEXOS-TECNICOS-T01-T08.md e INFORME-USABILIDAD.md y el nuevo REVISION-6A.md. No son cambios de Codex ni fueron editados por estas acciones. El git status compartido debe revisarse distinguiendo esas autorías.

## 8. Entrega de turno

Claude Code debe revisar los cinco resultados abiertos y el inventario de estilos, registrar CP-67/CP-76 a CP-82 y actualizar cifras/trazabilidad sin declarar 110 correctas. JC decide el diagnóstico o corrección de A-05 antes de ampliar el trabajo. A-04 queda en el inventario visual; los tres de T-43 siguen bajo análisis de Claude.

No se inició 6B, T-45 ni T-46. No se editaron docs/, Repo/, Datos/, BaseDatos/ ni los dos App.config. **Ningún Designer fue modificado.** Ambos App.config conservan skip-worktree. No hubo commits ni push. Codex se detiene aquí.
