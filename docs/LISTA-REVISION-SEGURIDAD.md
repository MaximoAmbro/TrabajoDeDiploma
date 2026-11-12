# Lista de verificación: revisión de SEG-1 a SEG-6

Claude Code, a pedido de JC. **Está escrita antes de leer una línea del código.** Para
saber qué dice Codex que hizo se leyó solo `Pruebas/RESULTADOS-SEGURIDAD-2026-11-11.md`.
`Repo/`, `Datos/` y `Pruebas/` no se abrieron.

**Para qué sirve:** fijar el estándar antes de ver la implementación. Una vez aprobada:
- **la lista no se afloja durante la revisión;**
- si algo resulta mal planteado, se dice en el reporte y lo decide JC.

**Cómo se marca cada punto en la revisión:**

| Marca | Significa |
|---|---|
| **Cumple** | Con la evidencia: archivo y línea, o la salida de la prueba |
| **No cumple** | Es un hallazgo, con su gravedad |
| **No verificable** | Con el motivo |

Los hallazgos los corrige Codex. La revisión no arregla nada.

**Una diferencia con lo que me pasaste:** dijiste 153 ejecuciones y 147 correctas. Esa era la
batería antes de SEG-6. El reporte dice que **SEG-6 ya está cerrado con 310.000 iteraciones,
y la batería final da 156 ejecuciones, 150 correctas y los mismos 6 fallos visuales.** La
revisión parte de esa batería, la final.

---

## 0. Cómo distinguir una prueba que verifica algo de una que solo no falla

Es la pregunta de fondo, así que va primero.

### El criterio: una prueba de seguridad cuenta solo si cumple las cuatro condiciones

| # | Condición | Cómo se comprueba | Qué la delata si falta |
|---|---|---|---|
| **P-1** | **Falla cuando se quita la defensa que dice probar** | **Prueba de mutación**, ejecutada y no razonada: se borra o invierte la línea que protege (por ejemplo, la verificación de rol de `EntradasDe`), se corre esa prueba sola y tiene que fallar | Si pasa sin la defensa, no prueba la defensa |
| **P-2** | **Afirma el motivo exacto del rechazo** | Espera la excepción de autorización, con su tipo y su mensaje, o el motivo puntual del resultado de ingreso. No espera «cualquier excepción» | Una prueba que acepta cualquier excepción pasa con un error de referencia nula o con un ID que no existe |
| **P-3** | **Afirma el estado después del rechazo** | Vuelve a leer la base: el reembolso no se hizo, la contraseña no cambió, `FechaIngreso` sigue vacía, el contador de intentos subió si debía subir | Una operación que lanza la excepción *después* de escribir pasaría igual |
| **P-4** | **Tiene su gemela positiva** | La misma preparación, con el actor legítimo, y la operación se hace | Sin la gemela, el rechazo puede venir de una preparación rota, no de la defensa |

**Banderas rojas** que se buscan en **todas** las pruebas, no solo en las de seguridad:
- `try`/`catch` que se traga la excepción;
- `ExpectedException(typeof(Exception))`, o con `AllowDerivedTypes`;
- `Assert.Inconclusive`;
- aserciones dentro de un `if`, o un `return` antes de la aserción;
- pruebas sin ninguna aserción, o cuya única aserción es «no lanzó»;
- `Assert.IsTrue(true)` y equivalentes;
- **pruebas que pasan solo por el orden:** `GestorSesion` es estático, así que una sesión
  que deja una prueba puede hacer pasar a la siguiente. Cada prueba de seguridad se corre
  **sola** y tiene que dar el mismo resultado.

**Cómo se declara el número.** Se dejan de declarar 156 a secas y se dice:
- «N ejecuciones»;
- «de las de seguridad, M resisten la mutación de su defensa».

Una prueba de seguridad que no resiste se cuenta aparte, como **prueba que no verifica**, y
va a Codex para corregir.

### La mutación: logística (requiere autorización de JC)

- **Toca código ajeno.** Para que no toque la copia de trabajo de JC ni la zona de Codex,
  se hace en un **`git worktree` aparte**, fuera de la carpeta del repositorio, sobre el
  commit que JC haga del trabajo de Codex. Nunca se commitea y se borra al terminar.
- **Comparte la base de pruebas y el escritorio.** Corre **solo cuando Codex no está
  probando**. Las pruebas de pantalla (CP-98, CP-104) se mutan al final.
- **Unas 25 mutaciones** (las de las secciones 1 a 4), cada una con su prueba esperada. Las
  aplica un script: cambia, compila, corre las pruebas filtradas y anota si fallan.
  **Esfuerzo estimado: 1,5 a 2,5 h.**

---

## 1. SEG-3: el negocio verifica quién llama

**La regla:**
- **un cliente solo opera sobre lo suyo** (RN-41, propuesta);
- **la administración es solo del Administrador** (RN-42, propuesta).

Las pruebas que pasan no alcanzan para darla por cumplida. Se verifica en el código, se
intenta saltar y se muta.

### 1.a Inventario, sin confiar en la lista del encargo

| # | Qué compruebo | Cómo |
|---|---|---|
| V-3.1 | **Todos** los métodos públicos de **todos** los gestores, no solo los 29 de la auditoría, quedan en una de tres clases: exige Administrador, exige titular, o público a propósito | Listado de los métodos `public` de cada `Gestor*` y `GestorSesion`. Cada uno, en su clase |
| V-3.2 | Cada método «público a propósito» tiene un motivo escrito | `Autenticar`, `RegistrarCliente`, el listado público de eventos, `CambiarContrasena` (protegido por la contraseña actual) y `LiberarVencidas`, que el reporte declara mantenimiento sin sesión. **`LiberarVencidas` se revisa aparte:** ¿puede, sin sesión, liberar algo que no esté vencido? |
| V-3.3 | Los métodos **nuevos** desde la auditoría están en el inventario | `GestorIngresos`, `CrearAdministrador` y los que se hayan agregado |
| V-3.4 | **La verificación es lo primero que hace el método:** antes de cualquier lectura a la base y antes de devolver nada | Lectura de cada uno. No vale verificar después de haber cargado los datos |

### 1.b De dónde sale la identidad

| # | Qué compruebo | Cómo intentaría saltarlo |
|---|---|---|
| V-3.5 | **La sesión solo la abre `Autenticar`.** No hay un método público que permita fijar el usuario de la sesión | **`Repo/AccesoPruebas.cs` es nuevo, y es lo primero que se mira.** Si `Fake.ComoAdministrador` abre la sesión por un método `public` de `Repo`, cualquier pantalla puede hacerse Administrador. Tiene que ser `internal` y estar visible solo para `Pruebas` |
| V-3.6 | **El rol sale de la sesión**, no de un parámetro ni del objeto que manda la pantalla | Buscar firmas que reciban un `tipoUsuario`, un `Usuario` o un `esAdmin` |
| V-3.7 | El usuario de la sesión no se puede cambiar desde afuera | `GestorSesion.UsuarioActual`: ¿devuelve el objeto con setters públicos (`TipoUsuario`)? Si una pantalla puede hacer `UsuarioActual.TipoUsuario = Administrador`, el rol se falsifica. Si es así, va como gravedad baja: lo haría una pantalla nuestra, no un atacante externo |
| V-3.8 | **El titular sale de la base**, no del parámetro | El reporte dice que «las compras y los reembolsos releen al titular persistido». Se verifica en cada método que recibe el ID de una compra, una entrada o un pago: el titular se lee de la base y se compara con la sesión |
| V-3.9 | Cerrar la sesión la borra, y después nada opera | «Volver al inicio» y luego cualquier operación protegida: rechazada |

### 1.c Los dos que más importan

| # | Método | Qué intento |
|---|---|---|
| V-3.10 | **`EntradasDe`**, que devuelve los **QR**, o sea la entrada misma | Como cliente A, `EntradasDe` de una compra de B: rechazado y **sin datos devueltos**. Probar IDs vecinos, porque son correlativos. Sin sesión: rechazado. Como Administrador: ¿se permite? Si se permite, que sea a propósito y quede escrito. Que el mensaje de error **no incluya** datos de la compra ajena |
| V-3.11 | **`DeEvento`**, que devuelve **nombres y mails** | Como cliente, con cualquier evento, aunque esté inscripto en él: rechazado. Sin sesión: rechazado. Como Administrador: funciona |
| V-3.12 | Los otros de lectura de datos ajenos | `HistorialDe`, `ObtenerPorId`, `PagosDe` (últimos 4 dígitos y token), `SegurosDe`, `ObtenerCliente` y `ActivaDe`: lo mismo que V-3.10 |

### 1.d Los ataques que probaría

| # | Ataque | Resultado esperado |
|---|---|---|
| V-3.13 | Sesión de A y, en el parámetro, el `IdUsuario` de B, en cada método que recibe un ID de usuario | Rechazado |
| V-3.14 | **Reembolso mezclado:** una lista de entradas a devolver con una propia y una de B | **Se rechaza todo, y no se reembolsa ninguna.** No vale reembolsar la propia y rechazar la ajena |
| V-3.15 | Un objeto `Compra` armado a mano, con `IdUsuario` de A y el ID de una compra de B | Rechazado (es la afirmación del reporte, a verificar) |
| V-3.16 | Cliente que llama a cada método de Administrador: los 7 de `GestorEventos`, los 2 de `GestorLocales`, `DeEvento`, `CrearAdministrador` y `GestorIngresos.Registrar` | Todos rechazados, y la base sin cambios |
| V-3.17 | Sin sesión, cada uno de los métodos protegidos | Todos rechazados |
| V-3.18 | El Administrador, en cada método de titular | Que el comportamiento sea el decidido (el encargo dice que se permite) y que esté probado |

### 1.e Las pruebas de SEG-3

| # | Qué compruebo |
|---|---|
| V-3.19 | CP-91 a CP-94 cumplen P-1 a P-4 |
| V-3.20 | **Hay una prueba por método protegido,** o una prueba con una fila por método. Con cuatro casos para 29 métodos, la mayoría no está probada. **Mutación:** quitar la verificación de cada uno de los 29, uno por uno; los que no hacen fallar ninguna prueba quedan listados como «protegidos sin prueba» |
| V-3.21 | **Agregar la sesión en las pruebas viejas no debilitó ninguna** | Se compara el diff de cada prueba modificada: solo se agregan líneas de sesión y no desaparece ninguna aserción. El reporte lo afirma, y se verifica línea por línea |
| V-3.22 | Las pruebas que cambiaron lo que esperan, con el motivo que declara Codex | CP-12 espera 20 formularios; cambian los títulos esperados en CP-15, CP-21, CP-44, CP-72 a CP-75, CP-78 y CP-82; CP-67. Cada cambio coincide con su motivo, y se conserva lo que la prueba verificaba |

---

## 2. SEG-1: cambiar una contraseña ajena

| # | Qué intento | Resultado esperado |
|---|---|---|
| V-1.1 | **La firma vieja desapareció:** no queda ningún método de dos parámetros, ni otro que escriba la contraseña sin pedir la actual | Buscar **todos** los lugares que escriben la contraseña, en `Repo/` y en `Datos/` |
| V-1.2 | **Caminos que no pasan por la pantalla:** ¿`Visual` referencia `Datos` directamente? Si la referencia existe, una pantalla puede llamar al repositorio que actualiza la contraseña y saltear el gestor | `Visual.csproj` y búsqueda de `Datos.` en `Visual/`. La regla 1 de `CLAUDE.md` lo prohíbe; se verifica que se cumpla |
| V-1.3 | Mail de otro y contraseña actual incorrecta | Rechazado; **la contraseña guardada no cambió** (se lee de la base); el contador de intentos de esa cuenta sube en 1 |
| V-1.4 | Cinco intentos con la actual incorrecta, y después la correcta | El sexto se rechaza con el mensaje de bloqueo de RN-04, aunque la actual sea correcta |
| V-1.5 | Un mail inexistente | El mismo mensaje, idéntico carácter por carácter, y la misma excepción que con una contraseña incorrecta |
| V-1.6 | **El tiempo de respuesta delata qué mails existen** | Con un mail existente se calcula el PBKDF2 (unos 500 ms a 310.000 iteraciones). Si con un mail inexistente responde al instante, la diferencia de tiempo revela si el mail existe. Se mide. **Vale para `Autenticar` también.** Si se confirma, es gravedad baja: se arregla calculando un hash de relleno |
| V-1.7 | La nueva no cumple RN-02 | Rechazada, y no cambia nada |
| V-1.8 | Cambio correcto | La vieja deja de servir, la nueva sirve y el contador de intentos queda como corresponda |
| V-1.9 | Con la sesión de A abierta, cambiar la contraseña de B (conociendo la de B) | Cambia, pero **la sesión sigue siendo de A**: el reporte dice «no sustituye una sesión activa». Se verifica |
| V-1.10 | **Re-alta sobre una cuenta existente:** `RegistrarCliente` o `CrearAdministrador` con el mail de otra cuenta | Rechazado por RN-01, **sin pisar** la contraseña ni el perfil de la cuenta existente |
| V-1.11 | La migración desde texto plano solo escribe el hash después de verificar la contraseña | Lectura de `Autenticar` |
| V-1.12 | La pantalla | Tres campos enmascarados y vacíos al abrir, sin «A», «B», «B»; enlace «Cambiar contraseña»; mensajes sin datos de la cuenta |
| V-1.13 | CP-95 a CP-98 cumplen P-1 a P-4 | Mutaciones: aceptar cualquier contraseña actual; no sumar el intento; cambiar el mensaje del mail inexistente |

---

## 3. SEG-2: el control de ingreso

### 3.a Las reglas de validación

Cada una con su caso y su mutación.

| # | Regla | Qué compruebo |
|---|---|---|
| V-2.1 | **Válida:** entrada confirmada, del evento elegido, dentro del horario y sin ingreso previo | Ingresa; `FechaIngreso` queda con la hora; el estado de la entrada no cambia |
| V-2.2 | **Ya usada** | Rechazada, con la fecha y hora del **primer** ingreso; `FechaIngreso` no se reescribe |
| V-2.3 | **Reembolsada** | Rechazada |
| V-2.4 | **Pendiente o vencida** | Rechazada, con un caso para cada estado. El reporte las nombra; se verifica que haya prueba de las dos |
| V-2.5 | **De otro evento** | Rechazada, y no se marca en ninguno de los dos eventos |
| V-2.6 | **Inexistente o inventada** | Rechazada. Un código con formato válido (`EA-` y 32 hexadecimales) que no existe, uno con formato inválido, uno vacío y uno muy largo |
| V-2.7 | **Fuera de horario**, en los **bordes** | Exactamente 6 h antes, un segundo antes de eso, exactamente 12 h después y un segundo después. Que la regla diga si los bordes entran, y que la prueba los fije |
| V-2.8 | **El reloj** | ¿El horario se prueba con un reloj inyectable o con eventos creados en el momento? Si depende de `DateTime.Now` sin control, la prueba de borde no es reproducible |
| V-2.9 | Los parámetros se leen de `Parametros` (`INGRESO_HORAS_ANTES` y `INGRESO_HORAS_DESPUES`), no del código | Regla de `CLAUDE.md`. Mutación: cambiar el parámetro en la base y comprobar que cambia el resultado |
| V-2.10 | Solo el Administrador registra ingresos (RN-42) | Como cliente y sin sesión: rechazado |
| V-2.11 | **Usada y después reembolsada:** una entrada que ya ingresó no se puede devolver | **Es un hueco nuevo posible:** entrar y cobrar el reintegro. `Reembolsar` tiene que rechazar las entradas con `FechaIngreso`. Que haya prueba |
| V-2.12 | Nada vuelve a poner `FechaIngreso` en vacío | Buscar toda escritura de esa columna |
| V-2.13 | El código leído se normaliza: espacios y mayúsculas de lo que escribe el lector, y siempre va por parámetro SQL | Lectura de `GestorIngresos` y del repositorio |

### 3.b La marca de usado es atómica de verdad

Que pase una prueba secuencial no alcanza. Se verifican tres cosas, y **las tres** tienen
que cumplirse:

| # | Qué compruebo | Cómo |
|---|---|---|
| V-2.14 | **En el código: una sola sentencia decide.** El ingreso se concede solo si un `UPDATE … SET FechaIngreso = … WHERE … AND FechaIngreso IS NULL` (con estado, evento y código en el `WHERE`) **afectó exactamente una fila**, según el valor que devuelve `ExecuteNonQuery` | Lectura de `RepositorioEntradas` y `GestorIngresos`. **Se rechaza el patrón «leer y después escribir»** (`SELECT` para ver si está usada y luego `UPDATE`), aunque esté dentro de una transacción: con el aislamiento por defecto, dos lecturas simultáneas ven las dos «sin usar» |
| V-2.15 | **La consulta del motivo nunca concede el ingreso** | Si el `UPDATE` no actualizó filas, la consulta que explica por qué (ya usada, reembolsada…) solo arma el mensaje. Se verifica que ningún camino devuelva «válido» desde esa consulta |
| V-2.16 | **La prueba concurrente concurre de verdad,** y falla contra una versión con carrera | En CP-104 se verifica que: (1) use **hilos reales con conexiones separadas**; (2) los suelte **a la vez**, con una barrera (`Barrier` o un evento compartido), y no con dos tareas lanzadas una después de la otra; (3) sean **más de dos**, unos 20 intentos del mismo código; (4) afirme **exactamente un ingreso** y el resto «ya usada», y lo **relea de la base**; (5) se repita varias veces en la misma corrida. **La prueba decisiva, por mutación:** reemplazar el `UPDATE` atómico por «leer, esperar 50 ms y escribir». **CP-104 tiene que fallar.** Si pasa, no prueba la atomicidad, aunque el código esté bien |

### 3.c La base y la pantalla

| # | Qué compruebo |
|---|---|
| V-2.17 | `04-seguridad-ingreso.sql`: idempotente (se corre dos veces sin error), la columna admite vacío y no toca datos existentes. El esquema `01` y la migración `04` dejan la misma estructura |
| V-2.18 | RPT-01, los disponibles y las devoluciones cuentan igual que antes: CP-32 a CP-34, CP-36 y CP-37 pasan sin cambios en sus aserciones |
| V-2.19 | `frmControlIngreso` | Solo desde el menú del Administrador; el foco vuelve al campo después de cada lectura, para que el lector USB pueda seguir; un Enter con el campo vacío no hace nada. Su `Designer` cumple la regla de 6B: geometría literal, sin fuentes ni colores |

---

## 4. SEG-4, SEG-5 y SEG-6: lo que queda por verificar

### SEG-4: el perfil Administrador no se asigna desde un método público

| # | Qué compruebo |
|---|---|
| V-4.1 | `Registrar(…, tipoUsuario)` es `internal`, y `InternalsVisibleTo` nombra solo a `Pruebas`. Nota: el ensamblado no está firmado, así que cualquier ensamblado que se llame «Pruebas» lo ve. Es aceptable para el proyecto, y se escribe como límite |
| V-4.2 | **`AccesoPruebas.cs`:** qué expone, con qué visibilidad, y si viaja en el `Repositorio.dll` de producción. Es lo mismo que V-3.5 desde el otro lado |
| V-4.3 | `CrearAdministrador` exige sesión de Administrador: un cliente y la falta de sesión se rechazan, con la base sin cambios |
| V-4.4 | Ningún camino público deja elegir el perfil: ni `RegistrarCliente` ni ninguna pantalla |

### SEG-5: contraseñas en texto plano

| # | Qué compruebo |
|---|---|
| V-5.1 | En `BaseDatos/`, ninguna contraseña que no empiece con `PBKDF2$` |
| V-5.2 | Dónde quedan las credenciales de demostración en texto plano (el generador de hashes, la documentación, el README) y que estén a propósito, como credenciales de demostración declaradas |
| V-5.3 | Los 1.178 clientes de reporte comparten un único hash, con la misma sal. Aceptable para datos ficticios; se escribe como límite |
| V-5.4 | **La base de desarrollo de JC y la de Máximo siguen con las contraseñas viejas** hasta correr la migración `04` y la semilla. Pasa a `PRUEBAS-PENDIENTES.md` como preparación |
| V-5.5 | La variante de migración de CP-67 sigue probando una credencial heredada, con un cliente creado por `Fake` y no con la semilla |

### SEG-6: las iteraciones

| # | Qué compruebo |
|---|---|
| V-6.1 | Los hashes nuevos usan 310.000. `Verificar` usa las iteraciones **guardadas** en el hash, no la constante |
| V-6.2 | Un hash viejo se re-hashea al iniciar sesión, y **uno más fuerte no se degrada** (CP-105 y CP-108) |
| V-6.3 | La comparación sigue siendo de tiempo constante |
| V-6.4 | Los valores fuera de rango (cero, negativos, enormes, sal o hash de largo inválido) se rechazan sin excepción no controlada (CP-107) |
| V-6.5 | La documentación dice «310.000, decisión del proyecto», sin presentarlo como cumplimiento de OWASP, que recomienda 600.000 |
| V-6.6 | CP-105 es una **medición**, no una verificación. **No se cuenta como prueba de seguridad:** se informa aparte. CP-109 depende del tiempo de la máquina: se verifica que no tenga un umbral que falle en una PC más lenta |

---

## 5. Lo que aplica a todo

| # | Qué compruebo |
|---|---|
| V-0.1 | Todo SQL nuevo va con parámetros; nada fuera de `Datos/` |
| V-0.2 | Los mensajes de error no revelan datos ajenos (mails, importes, códigos) |
| V-0.3 | Compila sin advertencias nuevas; los 6 fallos son los mismos 6 conocidos, por nombre |
| V-0.4 | Cada CP nuevo (CP-91 a CP-109) tiene su comentario `// CP-xx — RF, RN` y su nombre visible, como pide la definición de terminado |
| V-0.5 | Los cambios en `.Designer.cs` son los declarados en el reporte, y no hay otros |

## Orden de la revisión

1. V-3.5 y V-4.2, `AccesoPruebas.cs`: si ahí hay una puerta, cambia todo lo demás.
2. El inventario de SEG-3 (V-3.1 a V-3.4).
3. La atomicidad (V-2.14 y V-2.15) y el hueco de usada y reembolsada (V-2.11).
4. El resto de la lectura, sección por sección.
5. Las banderas rojas en las 156 pruebas.
6. La mutación (P-1 y las marcadas «mutación»), con autorización de JC y con Codex quieto.
7. El reporte: los hallazgos con gravedad, la tabla de pruebas que resisten la mutación y la
   cifra para declarar.

**Esfuerzo estimado de la revisión:**
- lectura y ataques por razonamiento: 3 a 4 h;
- mutación: 1,5 a 2,5 h.
