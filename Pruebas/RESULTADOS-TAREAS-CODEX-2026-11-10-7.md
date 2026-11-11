# Grupo A: primer corte histórico de T-42 y T-43

**Estado histórico.** JC autorizó después PA-02 y la continuación de T-44. El resultado vigente está en `RESULTADOS-TAREAS-CODEX-2026-11-10-7-continuacion.md`: 113 ejecuciones, 108 correctas y 5 fallidas. Este primer corte se conserva como evidencia de la parada anterior.

Fecha: 10/11/2026. Codex para JC y Claude Code. Base: `36a0d80` (6A, 77 ejecuciones).

**No se completaron las tres tandas.** PA-02 requiere la confirmación expresa de JC; T-43 detectó tres discrepancias preexistentes. Se detuvo el trabajo de implementación según la regla 7 de TANDAS-PRUEBAS-64.md. T-44 no se inició. Sin commits ni push.

## Resultado exacto por tanda

| Tanda | Estado | Batería observada | Diferencia con el plan |
|---|---|---|---|
| T-42 | PA-01 y PA-03 implementadas; PA-02 pendiente de confirmación | **81/81 correctas**, dos corridas seguidas, 7,6910 y 8,1154 segundos | Falta la ejecución de PA-02 para 82. Si JC confirma también la variante de migración, serían 83. |
| T-43 | Siete casos, quince ejecuciones implementadas; tres fallan | **96 totales: 93 correctas, 3 fallidas, 0 omitidas**. Resultado reproducido en dos corridas finales: 8,3965 y 8,5232 segundos | El total es 96, no 97, por PA-02 pendiente. Además hay tres resultados que requieren decisión de JC. |
| T-44 | No iniciada, por la parada exigida al detectar las discrepancias anteriores | Sin corrida de esta tanda | No se alcanzaron 113; no se declaran cubiertas sus filas. |

Después de T-42 se informó el estado parcial y el número antes de pasar a las pruebas independientes de T-43. PA-02 no se implementó por silencio ni por haber transcurrido tiempo desde la pregunta.

Comando utilizado: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar`.

La compilación pasa sin advertencias nuevas. Las dos corridas finales terminan con código **1**, por las tres pruebas indicadas debajo. La batería **no está en verde**. No se agregaron Ignore ni se alteraron pruebas existentes para ocultarlas.

Logs completos: `TestResults/Tandas7/T43-corrida-1.txt` y `T43-corrida-2.txt` (evidencia local ignorada por Git).

## 1. Caso 3.2 / PA-02: la migración existe

`Repo/GestorUsuarios.cs`, método privado `ContrasenaValida`, líneas 141–155:

1. Intenta verificar la contraseña contra el hash.
2. Si lo persistido **no** es un hash y coincide con la contraseña recibida, actualiza la contraseña con `Seguridad.Hash(pwd)` y autentica.
3. Si lo persistido ya es un hash, ese mecanismo no admite usar el hash como contraseña.

Se comunicó el resultado a JC y se pidió confirmación para las variantes de doble acceso del administrador y migración de una cuenta de prueba. **No llegó confirmación durante esta ejecución; PA-02 no está escrita.** CP-67 queda reservado, no descubierto ni contado por MSTest.

La semilla de admin@ contiene originalmente `Admin1234` en texto plano. PA-01 comprueba únicamente la navegación de login que ya estaba autorizada; guarda y restaura contraseña, intentos y bloqueo del administrador al terminar. No se usa esa ejecución como sustituto de PA-02 ni se afirma haber probado su contrato de migración.

Si se autorizan ambas variantes de PA-02, las cuentas previstas del plan subirían una ejecución: T-42 a 83, T-43 a 98 y T-44 a 114, una vez resueltos los fallos. Con solo el doble acceso, se mantienen 82/97/113.

## 2. Resultados preexistentes encontrados, sin corregir

### A-01 — Volver fuera de la vista de notebook

- Caso: **PA-04 / CP-69**, filas C9.1 y 9.17, variante 150 %.
- Pantalla: `Visual/Propietario/frmAgregarEvento.cs` y su Designer, **sin modificaciones**.
- Escenario: escala geométrica 1,5 con la misma técnica de fuentes de CP-25 y ventana limitada a 1366 × 728.
- Resultado: `lblVolver` ocupa `{X=177,Y=664,Width=67,Height=33}`. Su borde inferior es **697**, pero el área cliente termina en **689**. Quedan **8 px** fuera.
- Esperado: Registrar y Volver completamente dentro del área cliente, visibles y sin superponerse a hermanos.
- La variante 100 % pasa. El selector de preventa cumple ShowCheckBox, Checked=false y formato `dd/MM/yyyy HH:mm`.
- No se modificó el layout ni el Designer para corregirlo; sería trabajo sobre una de las pantallas bloqueadas para 6B y necesita decisión de JC.

### A-02 — Mis locales pisa el color del tema

- Caso: **PA-06 / CP-71**, fila C.12.
- Código preexistente: `Visual/Propietario/frmMisLocales.cs:40`, `FormatearGrilla`.
- Esperado según la versión falsable de la clasificación: `Tema.FilaAlterna`.
- Obtenido: gris fijo RGB **245/245/245**; el token es **250/249/253**.
- La pantalla aplica ese color en su Load, después de la normalización de la clase base. No se tocó esa línea.
- Las otras propiedades de grilla comprobadas antes del color pasan. La variante de selección/navegación del segundo local también pasa.
- Es una discrepancia visual respecto del contrato del tema, no una falla de registro de locales.

### A-03 — RN-06 está en el título, no en el texto solicitado

- Caso: **PA-10 / CP-75**, fila C.11.
- Código preexistente: `Visual/Propietario/frmAdministrarEvento.cs:117`.
- Datos: evento publicado de Fake, con un solo sector de capacidad positiva; clic real en Eliminar sector.
- Esperado por la clasificación: el texto contiene **`(RN-06)`**.
- Obtenido: título **RN-06** y texto «No se puede eliminar el sector: es el último con capacidad mayor que cero de un evento publicado.», sin el sufijo.
- **El sector sí se conserva**: la prueba verifica los IDs persistidos en finally aunque falle la comprobación del mensaje.
- No se cambió la aplicación ni se relajó la aserción. El aviso actual conserva el estilo anterior a V-6 de 6B; corresponde decidir si el encargo pretendía exigir esa convención antes de autorizar 6B.

Los tres resultados se reprodujeron en dos corridas finales. A-01 es un recorte geométrico; A-02 y A-03 son diferencias entre la presentación preexistente y lo que exige la clasificación. No deben presentarse los tres como fallos de lógica de negocio.

## 3. Correspondencia PA → CP y filas cubiertas

| Tanda | PA | CP | Filas manuales | Ejecuciones | Estado |
|---|---|---|---|---:|---|
| T-42 | PA-01 | CP-66 | 3.1, C.1 | 2 | Pasan: foco y menú del administrador |
| T-42 | PA-02 | CP-67 reservado | 3.2 | 0 | Pendiente de confirmación, sin implementar |
| T-42 | PA-03 | CP-68 | C.2, C.3 | 2 | Pasan: cinco fallos y avisos de 10/5 minutos |
| T-43 | PA-04 | CP-69 | C9.1, 9.17 | 2 | 1 pasa, 1 falla: A-01 |
| T-43 | PA-05 | CP-70 | 4.6, C.4 | 2 | Pasan: Tab y alta/listado/persistencia del local |
| T-43 | PA-06 | CP-71 | C.12, C.13 | 2 | 1 pasa, 1 falla: A-02 |
| T-43 | PA-07 | CP-72 | 4.2, C.5 | 3 | Pasan: botón y precios con coma/punto |
| T-43 | PA-08 | CP-73 | C.6, C.7 | 2 | Pasan: borrador y rechazo sin categoría |
| T-43 | PA-09 | CP-74 | C.8, C.9 | 2 | Pasan: capacidad cero y publicación con capacidad positiva |
| T-43 | PA-10 | CP-75 | C.10, C.11 | 2 | 1 pasa, 1 falla: RN-38 correcto y A-03 |

Total agregado: **9 casos implementados y 19 ejecuciones**; 16 pasan y 3 fallan. Sumadas a las 77 anteriores: 96 ejecuciones, 93 correctas.

Se aplicaron las versiones falsables que entraron en estas tandas (C.3, 4.6, C.12 y 9.17). El plan llama `txtmail` al campo del login, pero su Name real es `txtMailLogin`: se buscó ese nombre real sin renombrar el control ni cambiar el propósito de la aserción.

No se modificó ninguna prueba automática anterior. Los controles se buscan por Name y con tipos base que aceptan derivados. Para Volver se usa Control, de modo que un eventual cambio de Label a LinkLabel o Button no exige cambiar el selector.

## 4. Preparación y aislamiento

- Formularios reales en STA y MessageBox respondidos con DialogosPrueba, sin teclado global.
- Los escenarios de locales, eventos y sectores usan datos propios reconocibles por Fake; el local registrado desde UI también tiene nombre único con prefijo `test `.
- El administrador de la semilla se restaura después del login. Los ensayos de bloqueo usan un cliente propio; se restauran sus intentos antes de limpiarlo.
- Se agregó una preparación de borrador sin sectores y otra de capacidad de local, ambas en Fake con SQL parametrizado, solo sobre los datos propios de cada prueba.
- La primera preparación de bloqueo usaba DateTime.Now con fracciones; SQL datetime podía redondear unos milisegundos hacia arriba y producir «6 minutos» en el límite de cinco. **Era una imprecisión de mi fixture nueva**, no un defecto de la aplicación: se corrigió preparando el reloj con precisión de segundos. Las aserciones de «10 minutos» y «5 minutos» no cambiaron. Las dos corridas finales reproducen únicamente A-01 a A-03.
- No se exportaron reportes ni se generaron archivos nuevos en Documentos. Los permisos adelantados para T-46 no se utilizaron porque T-46 está fuera de esta sesión.

## 5. Archivos tocados, tanda por tanda

### T-42

- `Pruebas/InterfazGrupoA.cs` — helper compartido para STA, búsqueda por nombre, mostrar formularios y limpieza.
- `Pruebas/PruebasGrupoALogin.cs` — CP-66 y CP-68; no contiene CP-67.

### T-43

- `Pruebas/PruebasGrupoAPropietario.cs` — CP-69 a CP-75, con los tres resultados fallidos preservados.
- `Pruebas/Fake.cs` — EventoBorradorSinSectores y CambiarCapacidadDeLocal para preparar datos aislados.

### Corrección pequeña autorizada aparte

- `Visual/Cliente/FrmCompraCliente.cs` — solo cuatro palabras de comentarios: según, máximo, útil y distribución. Ningún cambio ejecutable ni en textos visibles. No se tocó el bloque del resumen ni el TOTAL.

### Reporte

- Este archivo, para trasladar a docs/ en el turno de Claude Code.

## 6. Cambios ajenos ya presentes al iniciar

El git status inicial ya mostraba cambios de planificación de Claude Code. Se preservaron sin modificarlos:

- Modificados: `docs/CORRECCIONES-DOCUMENTO.md`, `docs/ESTADO-AGENTES.md`, `docs/PLAN-RENOVACION-VISUAL.md`.
- Sin seguimiento: `docs/ENCARGO-6B-BORRADOR.md`, `docs/PRUEBAS-64-CLASIFICACION.md`, `docs/TANDAS-PRUEBAS-64.md`, `docs/verificaciones/disenador-host-6A.txt`.

El estado escrito seguía considerando el plan borrador. La instrucción directa de JC de esta sesión autorizó T-42 a T-44; no se editó el archivo de coordinación para hacerlo coincidir.

## 7. Qué tiene que decidir JC y qué sigue para Claude Code

1. **JC confirma PA-02 / 3.2**: doble acceso solo, ambas variantes o mantener pendiente. La ruta de migración existe; se informó antes de escribir la prueba.
2. **JC decide sobre A-01 a A-03.** No se corrigen en esta tanda ni se ocultan con Ignore. Para A-03 conviene que Claude confirme si la clasificación está adelantando el cambio de mensajes de 6B. Para A-02, análogamente, la grilla preexistente sobrescribe el tema.
3. Claude Code revisa este reporte y actualiza sus documentos, correspondencia PA/CP, recuentos reales y estado. No marcar T-42/T-43/T-44 como completadas ni actualizar a 113 correctas.
4. Tras esas decisiones se podrá cerrar T-42/T-43 y retomar T-44. **T-45 y T-46 no se ejecutaron. 6B sigue bloqueada.**

Confirmaciones: ningún Designer fue modificado, incluidos los otros 18; tampoco se modificaron sus archivos de formulario. No se tocaron Repo/, Datos/, BaseDatos/, tools/ ni los dos App.config. Ambos App.config mantienen `S` de skip-worktree. `git diff --check` no reportó errores de espacios; los avisos LF/CRLF corresponden a los documentos ajenos preexistentes.

Sin commits ni push. Codex se detiene aquí por los resultados que el plan exige consultar.
