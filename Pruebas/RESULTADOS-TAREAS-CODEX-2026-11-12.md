# CHECK, reembolso, textos y T-41 — cierre del 12/11/2026

## Alcance y estado

Trabajo de JC: tres arreglos y después T-41. Inicio: 21:44:49 UTC, sobre `main`, HEAD `3ad6010`. Al empezar sólo aparecía `docs/LISTA-REVISION-SEGURIDAD.md`, de Claude Code; coincidía con el trabajo paralelo anunciado. Durante la tarea también aparecieron `docs/diagramas/` y `tools/Diagramas/`: no se tocaron.

**Completados los cuatro puntos.** La primera parte se detuvo para aclarar la contradicción entre traducir `Show Password` y no tocar los otros Designer. JC autorizó después únicamente el `Text` de los dos controles; se aplicó esa excepción y se terminó el punto 3 antes de implementar T-41. No se adoptó un reemplazo en ejecución para eludir la restricción.

No se editaron docs/, tools/, AGENTS.md, CLAUDE.md ni los App.config. Sin commits ni push.

## 1. CHECK de Entradas.Estado

Antes de editar o recrear la base de pruebas se consultaron las conexiones locales de Visual y Pruebas, sin mostrar sus cadenas. Se buscó cada valor fuera de Pendiente, Confirmada, Reembolsada y Vencida, comparando los bytes para detectar también diferencias de mayúsculas o espacios.

- `BddEventAura`: **ningún valor fuera del conjunto**.
- `BddEventAura_Pruebas`: **ningún valor fuera del conjunto**.
- Agregado `CK_Entradas_Estado` a `BaseDatos/01-esquema.sql`, en el lote posterior al alta de las columnas.
- Migración nueva `BaseDatos/05-check-estado-entradas.sql`: valida primero con bloqueo dentro de la transacción; muestra los estados desconocidos y aborta si hay alguno. No corrige ni borra filas. Usa `WITH CHECK` y deja habilitada y confiable la restricción.
- La migración se ejecutó dos veces en cada una de las dos bases locales. La consulta a sys.check_constraints confirmó `is_disabled=0` e `is_not_trusted=0` en ambas.
- CP-110 prueba en SQL los cuatro estados del enum, revierte cada modificación y exige error 547 de CK_Entradas_Estado al intentar `Usada`.

**Batería: 157 ejecuciones, 151 correctas y los mismos 6 fallos conocidos.** Evidencia ignorada: `TestResults/2026-11-12-punto1.log`; ejecución de pruebas 53,3082 s.

### Qué ejecuta Máximo

Tras recibir el código, seleccionar su base EventAura existente en SSMS y ejecutar **`BaseDatos/05-check-estado-entradas.sql` completo**. No debe ejecutar otra vez `01-esquema.sql` sobre una base ya preparada. Si 05 muestra un estado desconocido y aborta, conservar el valor y avisar a JC: no sanearlo automáticamente.

Si aún no aplicó la migración de ingreso de la tanda anterior, también necesita `BaseDatos/04-seguridad-ingreso.sql` para disponer de FechaIngreso y de los parámetros de ingreso que usa la aplicación. Ninguna migración se ejecutó desde esta sesión sobre una base de Máximo.

## 2. Reembolso después del ingreso

- `Datos/RepositorioCompras.cs`: el UPDATE de cada entrada exige ahora `FechaIngreso IS NULL` junto con Estado=Confirmada. La transacción existente revierte todo el lote si una entrada no cumple.
- `Repo/GestorCompras.cs`: al releer las entradas persistidas, rechaza las usadas con RN-29: «No podés devolver una entrada que ya se usó para ingresar».
- CP-111 tiene dos filas: evento a 72 horas, ingreso habilitado 96 horas antes y cobertura de devolución de 48 horas, para superponer las ventanas. Registra un ingreso real, verifica el rechazo en negocio usando objetos obtenidos antes del ingreso y llama además al repositorio directamente.
- Las variantes ponen la entrada usada al principio o al final del lote: ambas conservan los estados, el importe y FechaIngreso, incluida la reversión de la entrada que SQL pudo haber actualizado primero.
- La entrada no usada de la misma compra sigue pudiendo devolverse. El parámetro se restaura en finally y sólo se altera en `_Pruebas`.

**Batería: 159 ejecuciones, 153 correctas y los mismos 6 fallos conocidos.** Evidencia ignorada: `TestResults/2026-11-12-punto2.log`; ejecución 46,9916 s.

## 3. Textos y voseo

Se emparejaron los mensajes de Visual y Repo con el voseo ya usado por la aplicación:

- Ingrese/Seleccione/Pruebe → Ingresá/Seleccioná/Probá.
- Vuelva/Consulte/Elija/Ajuste → Volvé/Consultá/Elegí/Ajustá.
- Verifique/avise/complete/Escanee → Verificá/avisá/completá/Escaneá.
- «Por favor, selecciona…» → «Por favor, seleccioná…».
- ¿Desea…? → ¿Querés…?; «Si vuelve/cierra/cancela» → «Si volvés/cerrás/cancelás»; «que armó» → «que armaste».
- Puede/Debe → Podés/Tenés que, según la frase.
- Su/sus/Le → Tu/tus/Te, incluyendo los avisos de bloqueo y suscripción.
- Contrate/agréguele → Contratá/agregale.
- Se actualizaron únicamente las expectativas de texto de las pruebas afectadas. No se quitaron aserciones ni se cambiaron los efectos de negocio comprobados.

**Batería del voseo: 159 ejecuciones, 153 correctas y los mismos 6 fallos conocidos.** Evidencia ignorada: `TestResults/2026-11-12-punto3-registro.log`; ejecución 47,4524 s.

**Batería al terminar el punto 3, incluidas las traducciones autorizadas: 159 ejecuciones, 153 correctas y los mismos 6 fallos.** Evidencia: `TestResults/2026-11-12-punto3-final.log`; ejecución 44,1644 s.

### Revisión de inglés y excepción de los Designer

Los dos textos completos de controles en inglés se tradujeron:

- `Visual/_Inicio/frmInicio.Designer.cs:82`: `Show Password` → `Mostrar contraseña`.
- `Visual/_Inicio/frmCrearUsuario.Designer.cs:57`: `Show Password` → `Mostrar contraseña`.

Se verificó contra HEAD el contenido completo de ambos archivos: la diferencia exacta es esa única propiedad `Text` por archivo. No se cambiaron tamaños, fuentes, colores, posiciones, tabulación ni finales de línea. El diff se mostró a JC durante el trabajo y queda reproducido aquí:

```diff
--- Visual/_Inicio/frmInicio.Designer.cs
+++ Visual/_Inicio/frmInicio.Designer.cs
-            this.checkbxShowPass.Text = "Show Password";
+            this.checkbxShowPass.Text = "Mostrar contraseña";
--- Visual/_Inicio/frmCrearUsuario.Designer.cs
+++ Visual/_Inicio/frmCrearUsuario.Designer.cs
-            this.checkbxShowPass.Text = "Show Password";
+            this.checkbxShowPass.Text = "Mostrar contraseña";
```

Se revisaron también los anglicismos del inventario anterior: Mail en rótulos/avisos, Merchandising en servicios, Ranking en el reporte, ticket/tickets en rótulos y nombres de archivos, Token de pago y el servicio de catálogo Meet & Greet. **Continúan presentes**; no se presentan como traducciones realizadas ni se renombraron datos de catálogo. La revisión no encontró otro mensaje completo estático en inglés en los controles. No se modificó un tercer Designer por idioma. No se confundieron los términos anteriores con marcas ni siglas PDF, QR, USB o VIP. Las excepciones y datos dinámicos externos pueden aportar otros textos, como ya señalaba el inventario; este punto no cambia el manejador técnico de errores.

## Fallos preexistentes conservados

CP-85 menú propietario a 150 %, CP-81 compra visible tras rechazo de preventa, CP-69 alta de evento a 150 %, CP-71 color de la grilla, CP-88 lectura del reporte recortada y CP-76 beneficios de suscripción recortados. Ninguno se corrigió.

## 4. T-41: piloto y serialización

- **R-1:** `BotonTematico` concentra los tres componentes de botón. Sus propiedades de estilo se declaran en el componente, con los métodos `ShouldSerialize...`, valores por omisión para FlatStyle/UseVisualStyleBackColor y exclusión de serialización de las propiedades de estilo y de FlatAppearance. La primera ejecución del host detectó que los métodos solos no alcanzaban con la propiedad Font heredada; se corrigió la implementación declarando esas propiedades en los componentes, sin debilitar la prueba.
- **Roles:** componentes `EtiquetaTitulo`, `EtiquetaSeccion`, `EtiquetaRotulo` y `EtiquetaAyuda`, derivados de Label, con el mismo tratamiento de Font/ForeColor/BackColor. El piloto usa cinco asignaciones de rol: título del evento, ayuda del nivel y tres títulos de tarjeta.
- **R-2/R-3:** se eliminó `Tema.Compra`. El Designer del piloto tiene geometría literal a 96 DPI y en la escala de 8, sin fuentes, colores, FlatStyle/FlatAppearance ni referencias a Tema. Se quitaron **43 asignaciones de estilo** del Designer, contando colores de enlace y LinkBehavior. El código aplica los roles visuales de paneles, enlace y documento mediante Tema.
- **R-4:** la guarda permanece activa durante toda la distribución y `Disposicion.RepetirUnaVez` admite como máximo dos pasadas. Una prueba fuerza la oscilación de ancho y exige exactamente dos, sin llamada recursiva.
- **O-1:** `Tema.Foco = Color.Black`. El borde de foco del primario usa ese token; CP-62 comprueba contraste de al menos 3:1 con Primario. Se prueba el foco real, la pérdida de foco y los estados habilitado/deshabilitado.
- **O-2/O-3:** caché de fuentes normalizadas por familia/tamaño/estilo/unidad/charset/orientación. Dos controles con la misma fuente reutilizan el mismo objeto y se conservan cursiva y subrayado.
- **O-4:** todos los estados de los botones se calculan en `Tema.EstadoBoton`, utilizado por los componentes y por la adaptación de botones heredados. En los heredados se conserva la geometría y no se duplican las suscripciones a eventos al normalizar varias veces.
- **Disposición extraída:** `Visual/Disposicion.cs` contiene escala, ajuste al área visible, alineación parametrizada de acciones, documento/vista y pintado de tarjetas/separadores.
- Se corrigió el comentario «l?neas» y se retiraron los dos using no usados señalados en la revisión del piloto; sin cambios de funcionalidad del ticket.

### Prueba principal: CP-112, siete filas

Se agregó la referencia de pruebas a `System.Design`. Cada fila crea un **DesignSurface real**, obtiene su `IDesignerHost`, fuerza la vista, crea el componente con el host, comprueba Site.DesignMode y que tenga diseñador. Ejecuta `PropertyDescriptor.ShouldSerializeValue` sobre Font/BackColor/ForeColor y, para botones, FlatStyle/FlatAppearance/MinimumSize/Padding/Cursor/UseVisualStyleBackColor: todos deben devolver false.

Además exige que Text/Location/Size sigan serializándose cuando se editan; verifica la fuente visible del rol y que FlatAppearance esté oculta a la serialización. Como control negativo crea un Button estándar en el mismo host, le asigna Tema.Boton y comprueba que su Font **sí** se serialice. No se sustituyó esta prueba por una inspección fuera del host.

Las siete filas son los tres botones y los cuatro roles de etiqueta. No se agregaron filas para pantallas ajenas. CP-63 exige también que no haya propiedades de estilo ni Tema en el Designer del piloto. **CP-13, CP-25 (sus tres variantes) y CP-54 pasan sin modificar esas pruebas.**

**Batería completa final: 166 ejecuciones, 160 correctas y los mismos 6 fallos conocidos**, 50,2836 s, `TestResults/2026-11-12-t41-final.log`. Tras asegurar que la captura de evidencia no quede tapada por otra ventana, las siete filas del host se volvieron a ejecutar: **7/7**, 1,4829 s. Ese ajuste de TopMost existe sólo en la captura de prueba; no en el formulario del producto.

Compilación Debug sin errores ni advertencias de compilador nuevas. `git diff --check` sin errores y `VerificarReglasAgentes.ps1` correcto. Los App.config conservan S de skip-worktree y no se editaron.

### Evidencia y prueba manual pendiente

- Capturas ignoradas de CP-13 y CP-25 en `TestResults/Interfaz/`.
- Captura real inspeccionada: `TestResults/T41/piloto-pantalla-real.png`. Se ve el título, el nivel, el enlace, las tarjetas y las acciones. DrawToBitmap no equivale a una captura de pantalla real; para esta evidencia se usó CopyFromScreen.
- **Para JC, después del commit:** realizar la verificación de Visual Studio Community descrita en la sección 4 del encargo v3: abrir el piloto y EventosCliente, revisar propiedades y guardar una prueba. El host automatizado no se presenta como una apertura o guardado realizados dentro de Visual Studio. Sólo JC/Claude pueden dar por confirmado ese piloto manual.

### Límites respetados

Los únicos Designer con diferencias son:

1. `Visual/Cliente/FrmCompraCliente.Designer.cs`: T-41.
2. `Visual/_Inicio/frmInicio.Designer.cs`: únicamente el Text autorizado.
3. `Visual/_Inicio/frmCrearUsuario.Designer.cs`: únicamente el Text autorizado.

**Ningún otro Designer se modificó.** No se inició T-47, T-48 ni el rediseño de las otras pantallas. Los seis defectos conocidos siguen intactos.

Archivos adicionales de T-41: los tres botones existentes, nuevos `Componentes/BotonTematico.cs` y `Componentes/Etiquetas.cs`, `Tema.cs`, `Disposicion.cs`, `FrmCompraCliente.cs`, `Visual.csproj`, `Pruebas.csproj`, `PruebasFormularios.cs` y nuevo `PruebasSerializacionComponentes.cs`. Para Claude: registrar CP-110, CP-111 (dos filas) y CP-112 (siete filas) en la documentación, sin necesidad de que Codex toque docs/.

## Horas reales

Intervalos de pared registrados con reloj UTC; incluyen ejecución de pruebas y lecturas de preparación, sin la espera posterior de respuesta de JC:

| Punto | Intervalo | Horas |
|---|---|---:|
| 1: CHECK, consulta previa y migración | 21:44:49–21:47:19 | 0,0417 |
| 2: reembolso y dos variantes | 21:47:19–21:49:15 | 0,0322 |
| 3: voseo y batería | 21:49:15–21:51:06 | 0,0308 |
| 3: traducciones autorizadas, revisión y batería | 21:57:00–21:59:02 | 0,0339 |
| **3: total de ambos tramos** | Sin contar la espera de aclaración | **0,0647** |
| 4: T-41 | 21:59:02 hasta 2026-11-12 22:11:35 UTC | 0,2092 |

No se contabiliza la espera de aclaración como trabajo activo. Al cerrar el reporte termina el encargo de cuatro puntos; no se empieza otra tarea.

Hora de cierre: 2026-11-12 22:11:35 UTC. T-41: 753 segundos (0,2092 horas de pared), incluidas pruebas, verificaciones y reporte. Sin commits ni push.
