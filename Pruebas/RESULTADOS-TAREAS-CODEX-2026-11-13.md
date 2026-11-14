# T-47/T-48, excepción de botones y revisión de seguridad

Codex, 13/11/2026. **T-48 cerrada. T-47 mantiene una decisión pendiente por recortes de Volver fuera de los cinco botones autorizados. R-01, enumeración de login, CP-81, K-4 y K-5 implementados y verificados. K-1 pendiente de decisión. Sin commit ni push.**

## Estado inicial y alcance

Se verificó git status antes de trabajar. Persistían exactamente los cambios propios de la sesión anterior; había documentación/diagramas/herramientas en paralelo de Claude. Se leyeron los encargos y la sección 8 de AUDITORIA-DOCUMENTO-CODIGO.md. Las instrucciones explícitas de JC de esta sesión prevalecen sobre el estado documental histórico.

No se escribió docs/, tools/ ni ninguno de los dos App.config. Ambos conservan S de skip-worktree. No se inició T-38, T-39, T-40 ni T-49. Las medidas cambiadas pertenecen únicamente a la excepción de cinco botones y sus ventanas. Los archivos temporales de comparación están en TestResults/ (ignorado).

## 1. T-47/T-48 y los cinco números del punto de control

| Qué | Estimado | Real observado |
|---|---|---|
| Horas T-47 + T-48 | 7–10 h | Aproximadamente 0,34 h acumuladas: 0,12 h + 0,1425 h de los dos avances del 12/11 y 0,0753 h del tramo de hoy hasta la batería T-47. Falta resolver Volver; no es una duración de cierre completo |
| Líneas de fuentes y colores borradas | 314 | 266 asignaciones reales (94 Font + 172 colores), 48 UseVisualStyleBackColor adicionales: 314 con el criterio original. Más 43 FlatStyle/FlatAppearance: 357 líneas de estilo retiradas |
| Pantallas limpiadas | 18 | 18 Designer; piloto y Control de ingreso ya estaban limpios. CP-63 controla las 20 |
| Líneas de estilo de ejecución reemplazadas | 23 en 5 pantallas | 23 en Historial, Listas de espera, Mis locales, Reportes y Pago. Eventos del cliente conserva FillWeight |
| Filas nuevas por la limpieza | +18 sobre las 19 originales | +19: CP-63 pasó de una a 20 filas. Batería antes de seguridad: 185 / 181 / 4 |

El recuento previo de 314 no era de fuentes/colores puros. El tiempo observado es de Codex y no tiempo humano; los avances fueron automatizados con roles revisados, comparación de fuentes y pruebas. Los recortes adicionales impiden usar ese tiempo como validación del plan completo.

### Geometría aplicada (unidades del Designer)

| Archivo/control | Antes | Después | Motivo |
|---|---|---|---|
| Eventos del cliente / Buscar | 95×32 | 123×32 | Texto y padding del componente |
| Eventos del cliente / Limpiar | 95×32 | 135×36 | Texto completo y 24 px efectivos de alto al 100 % |
| Eventos del cliente / Limpiar Location | (821,93) | (849,93) | Buscar termina en x=843; se conserva el espacio original de 6 |
| Eventos del cliente / ClientSize | 928×494 | 996×494 | Limpiar termina en 984; se conservan 12 de margen derecho, sin crecimiento vertical |
| Eventos del local / Administrar Evento | 202×35 | 208×35 | Texto completo |
| Reportes / Generar | 116×34 | 120×34 | Texto completo |
| Reportes / Exportar PDF | 116×34 | 160×34 | Texto completo |
| Reportes / ClientSize | 980×632 | 1024×632 | Exportar termina en 1012; se conservan 12 de margen derecho, sin crecimiento vertical |

La ventana de Eventos del local conserva 773×412. Una comparación automatizada contra HEAD de Location, Size, ClientSize, DPI, Dock, Anchor, TabIndex, MinimumSize y MaximumSize de todos los Designer confirmó que solo cambiaron las ocho asignaciones de esta tabla. Evidencia: TestResults/T47-cierre/geometria.json. No se movió ningún otro control.

### 100 % y 150 % reales de Windows

Se compilaron dos versiones: HEAD ee25328 en una copia aislada para el antes, y el árbol actual para el después. Se cambió Windows con DisplayConfigSetDeviceInfo usando la herramienta existente de Claude en modo de lectura/ejecución, sin editarla. Cada proceso nuevo lleva el mismo manifiesto de DPI de Visual y exige GetDpiForWindow = 96 o 144 según la escala. Windows se restauró al 100 %.

Se capturaron las tres pantallas antes y después en ambas escalas: **12 capturas de ventana nativa mediante CopyFromScreen**. Se usa una fixture de disposición: los formularios reales se abren sin sus manejadores Load de datos, y la normalización de estilo de la base sí corre. Esto no sustituye una captura del flujo con datos; las pruebas funcionales siguen cubriendo el flujo real contra la base de pruebas.

Los cinco botones autorizados muestran el texto completo y sus Bounds quedan dentro del cliente a ambas escalas. Las ventanas completas quedan dentro del área de trabajo. Las dimensiones efectivas después son:

| Pantalla | Cliente al 100 % | Ventana al 100 % | Cliente al 150 % | Ventana al 150 % |
|---|---|---|---|---|
| Eventos del cliente | 664×329 | 670×358 | 996×494 | 1002×534 |
| Eventos del local | 618×330 | 634×369 | 928×494 | 950×550 |
| Reportes | 819×506 | 825×535 | 1229×758 | 1235×798 |

### Bloqueo de T-47: otros botones Volver

La captura después al 100 % de Eventos del cliente muestra «Volve»: con el foco y el estilo del componente se pierde la r final. HEAD mostraba Volver completo. Es **introducido por T-47**, no preexistente. El detector de tinta también marca diferencias en Volver de Eventos del local y a 150 %; las capturas permiten revisarlas, sin presentar el detector por sí solo como prueba inequívoca de truncación visual.

No se cambió la geometría ni el padding de esos botones: no forman parte de la excepción de cinco ni son desplazados por ellos. **Decisión pendiente:** autorizar el arreglo de estos Volver o ampliar el criterio del componente. Por eso no se declara T-47 cerrada. T-48 sí quedó cerrada. Se siguió con los puntos siguientes de JC sin esperar respuesta.

## 2. De quién era el defecto de los cinco botones

La comparación usa el código completo de HEAD, no fuentes supuestas. Se conserva la herencia original del Designer y la normalización que ya hacía la aplicación antes de T-47. Fuentes efectivas originales: Segoe UI Semibold de 8,25 pt heredadas de los formularios, salvo Administrar Evento, de 9 pt. T-47 usa 10 pt y agrega padding horizontal del componente. El 38 % de ancho adicional de Exportar no se atribuye solo a la fuente.

Anchos efectivos al 100 %; el ancho preferido incluye la disposición original del botón:

| Botón | Ancho anterior | Ancho requerido anterior | ¿Se cortaba antes al 100/150 %? | Causa del recorte observado tras T-47 |
|---|---:|---:|---|---|
| Buscar | 63 | 48 | No / No | T-47 |
| Limpiar | 63 | 53 | No / No | T-47 (también alto) |
| Administrar Evento | 162 | 122 | No / No | T-47 |
| Generar | 93 | 55 | No / No | T-47 |
| Exportar PDF | 93 | 81 | No / No | T-47; fuente y padding |

Se comparó la tinta del texto renderizado con la referencia sin recorte, incluyendo antialias. Las capturas confirman los cinco resultados. **Ninguno de estos cinco era preexistente.** Los defectos preexistentes de aspecto CP-69/76/85 permanecen separados.

## 3. R-01: primero rojo, después verde

Se escribió CP-113 antes del arreglo. Cuatro filas fallaron contra la implementación original: contrato público, salida mutable, mutación del perfil original y mutación del ID original. TestResults/R01-rojo.log: 4 / 0 / 4, 0,8219 s.

Después:
- GestorSesion.Iniciar es internal; Fake sigue accediendo mediante el InternalsVisibleTo existente.
- UsuarioActual devuelve SesionAutenticada: clase sellada con getters sin setters, sin contraseña y sin referencias al Usuario mutable original.
- Las verificaciones de autorización usan ese snapshot. Cambiar el Usuario recibido/retornado por la autenticación no altera la identidad almacenada.
- GestorReportes consume la identidad inmutable; no se convirtió el snapshot otra vez a Usuario.

TestResults/R01-verde.log: **4 / 4 / 0**, 1,0887 s. Las cuatro filas vuelven a pasar en la batería completa. Hallazgo preexistente, corregido.

## 4. Enumeración de cuentas en login (K-3)

Autenticar devuelve el mismo rechazo RN-04, «Mail o contraseña incorrectos.», para correo inexistente, contraseña incorrecta, quinto fallo y cuenta bloqueada. No expone cantidad de intentos ni plazos. La pantalla usa el mismo título, aviso y comportamiento en todos esos rechazos.

RN-04 mantiene contador, bloqueo de 15 minutos y recuperación al vencer. CP-28 y CP-68 se actualizaron porque su expectativa de revelar intentos/minutos fue revocada explícitamente por JC; conservan y refuerzan las comprobaciones del estado persistido. CP-29 refleja el contrato uniforme del correo inexistente, manteniendo la falta de sesión para todos los datos inválidos.

CP-114 tiene tres filas. En cada una se ejecutan dos intentos reales de interfaz: correo existente e inexistente. Compara el texto completo observado del diálogo, título, permanencia de la pantalla, ausencia de sesión y de ventanas de destino. Comprueba además el contador/bloqueo persistido de la cuenta existente. Escenarios: primer fallo, quinto fallo y cuenta ya bloqueada. Las tres filas fallaron antes del cambio (TestResults/K3-rojo.log) y pasan después.

Se hace una verificación PBKDF2 ficticia para cuentas inexistentes y bloqueadas, evitando omitir todo el trabajo criptográfico en esas ramas. Esto no se presenta como una prueba formal de tiempo constante; CP-114 verifica los mensajes y el comportamiento de interfaz pedido. Sus cuentas de prueba usan hashes reales de 310.000 iteraciones.

Batería intermedia tras R-01 y K-3: **192 / 188 / 4**, 42,2511 s, TestResults/K3-bateria.log. Hallazgo preexistente, corregido.

## 5. Demás hallazgos de código, en orden

| Hallazgo | Resultado y clasificación |
|---|---|
| K-2 / CP-81 | Preexistente corregido. Hide dentro de Load no cancelaba el Show en curso. El formulario registra el acceso rechazado y SetVisibleCore garantiza que no quede visible al regresar; OnShown no ejecuta la disposición del acceso rechazado. CP-81 pasa en sus tres variantes sin modificar su archivo de pruebas |
| K-1 / aviso de lista de espera | **Decisión pendiente**, preexistente. RN-10 solo habilita la lista al agotarse y RN-35 define el orden; ninguna promete avisos. Pero RF-10, en docs/01-requerimientos.md:16, sí dice «Registra al usuario e informa que será notificado». No es solo texto de pantalla. Además el cliente no tiene una consulta de posición: DeEvento requiere Administrador. No se construyeron notificaciones ni una consulta nueva, ni se reemplazó una promesa por otra función inexistente. JC debe decidir si se corrige el alcance documental y qué consulta se ofrece |
| K-3 | Preexistente corregido, sección anterior |
| K-4 / máximo admisible | Preexistente corregido. PorcentajePara de Plata/Oro rechaza con RN-20 un porcentaje mayor al parámetro DESCUENTO_MAX_ADMISIBLE. No modifica ni recorta silenciosamente el porcentaje. CP-115, seis filas: ambos niveles por debajo, en igualdad y por encima del máximo. La preparación cambia parámetros solo en la base _Pruebas y los restaura en finally |
| K-5 / alta de local | Preexistente corregido: «Registar local» y «Registrarse» pasan a «Registrar local». Solo se cambió Text de esos dos controles; se conservan nombres, roles y eventos |
| K-6 / R-01 | Preexistente corregido, sección 3 |

No se amplió el alcance para resolver los dos pendientes. No se arreglaron los tres defectos visuales preexistentes restantes.

## Batería final

**198 ejecuciones: 195 correctas, 3 fallidas; 49,8355 s.** TestResults/2026-11-13-cierre-final.log.

Cuenta desde 184: +1 CP-63 de Control de ingreso, +4 CP-113, +3 CP-114, +6 CP-115 = 198. Para el punto de control de limpieza: 166 +19 =185; las otras 13 filas son seguridad y tope de descuento.

- CP-63: **20/20**.
- CP-112, host de diseño real: **7/7**; mínimo vacío por defecto y mínimo explícito serializable en los tres botones.
- CP-81: **3/3** sin tocar la prueba.
- CP-13, CP-25 y CP-54 siguen pasando sin cambios en sus pruebas.
- Fallan únicamente CP-69 (Agregar evento a 150 %), CP-76 (beneficios de suscripción) y CP-85 (menú de propietario a 150 %), preexistentes.
- El recorte de Volver introducido por T-47 está fuera de la cobertura geométrica de esa batería. El 195/198 no lo convierte en resuelto.
- Prueba auxiliar de color ampliada: **20 pantallas, 31 primarios habilitados, cero fallos**, con control negativo de Button estándar con estilo del sistema. Incluye Control de ingreso. No agrega filas MSTest. CSV/PNG en TestResults/T47-cierre/colores-20/.

Compilación correcta. git diff --check correcto. Los dos App.config siguen protegidos. No se hicieron commits ni push.

## Capturas y evidencia

Cada carpeta tiene frmEventosCliente.png, frmEventosLocal.png, frmReportes.png y medidas.csv:
- [Antes 100 %](../TestResults/T47-cierre/antes-100/frmEventosCliente.png)
- [Después 100 %](../TestResults/T47-cierre/despues-100/frmEventosCliente.png)
- [Antes 150 %](../TestResults/T47-cierre/antes-150/frmEventosCliente.png)
- [Después 150 %](../TestResults/T47-cierre/despues-150/frmEventosCliente.png)

CapturarCierreT47.ps1 reproduce la captura con un binario indicado. VerificarBotonesT47.ps1 mide los 14 originales y verifica color de las 20 pantallas. Son verificaciones auxiliares, sin cambiar configuraciones ni abrir conexiones. La copia de HEAD se compiló en TestResults/T47-cierre/baseline con sus archivos propios, sin alterar los originales del repositorio.

## Horas reales

Tiempo de Codex, medido por marcas de esta sesión; no tiempo de un desarrollador humano:

| Tramo | Tiempo |
|---|---:|
| Excepción de cinco botones, comparación HEAD, capturas y batería T-47: 14:43:36–14:48:07 | 0,0753 h |
| R-01, prueba roja y contrato nuevo, hasta verde: 14:48:08–14:49:25 | 0,0214 h |
| Enumeración, prueba roja, cambios de contrato y batería: 14:49:26–14:51:52 | 0,0406 h |
| K-1/K-2/K-4/K-5, implementación y comprobaciones parciales: 14:51:53–14:56:02 | 0,0692 h |
| Verificación final, revisión y reporte: 14:56:02–15:01:53 | 0.0977 h |
| **Total de esta sesión, 14:43:36–15:01:53 ART** | **0.3050 h** |

Los tramos reflejan tiempo corrido de trabajo; las comprobaciones finales son compartidas por las tareas. No se atribuyen como una duración exacta independiente de cada hallazgo. Los dos avances anteriores sumaban aproximadamente 0,26 h adicionales.

## Archivos propios y entrega a Claude

Además de los 18 Designer y archivos de T-47/T-48 inventariados en los reportes del 12/11, esta sesión modifica: Visual/Cliente/FrmCompraCliente.cs; Visual/_Inicio/frmInicio.cs; Repo/GestorSesion.cs; Repo/SesionAutenticada.cs (nuevo); Repo/GestorReportes.cs; Repo/GestorUsuarios.cs; Repo/GestorFidelizacion.cs; Pruebas/PruebasRevisionSeguridad.cs (nuevo); Pruebas/DialogosPrueba.cs; Pruebas/Fake.cs; Pruebas/PruebasCuentas.cs; Pruebas/PruebasGrupoALogin.cs; Pruebas/PruebasFormularios.cs; Pruebas/CapturarCierreT47.ps1 (nuevo); Pruebas/VerificarBotonesT47.ps1 y este reporte. No se modificó el Designer de Control de ingreso ni el piloto.

Claude debe registrar CP-113/114/115 y la fila adicional CP-63, los defectos preexistentes corregidos, los cinco recortes introducidos por T-47 ya resueltos y el pendiente Volver, además de la contradicción RF-10/lista de espera. Los reportes anteriores son históricos y no se reescribieron como si esas decisiones ya hubieran existido.

**Trabajo detenido tras recorrer todos los puntos autorizados.** Pendientes para JC: alcance de Volver y promesa/consulta de lista de espera. T-47 no se da por cerrada artificialmente.
