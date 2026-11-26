# Casos de prueba

Proyecto `Pruebas/` con MSTest. Hay 219 casos: CP-01 a CP-211, CP-354 a CP-360 y CP-364 a CP-365. Suman **365 ejecuciones**. La última corrida completa pasó **365/365** sobre `7efc928430b308b16aa0bf37e022e8fa77126bea` el 26/11/2026. El resumen está en `TestResults/bateria-365-2026-11-26.md` y la salida completa en `TestResults/bateria-365-2026-11-26.log`. Antes: sobre `2db272e`, 360/360 antes de la renumeración de CP-358 a CP-360; sobre `967e038`, 205 casos y 348 ejecuciones (corrida de Codex del 19/11 y de Claude Code del 20/11, `TestResults/Claude-967e038/bateria.log`); sobre `dba674c`, 148 casos y 276 ejecuciones; sobre el código congelado de la medición de mutación, `65225e7`, 145 casos y 271 ejecuciones.
CP-01 a CP-10 son los de la lógica de fidelización, que define el documento. CP-11 a CP-365 se
agregaron en noviembre de 2026: ver «Casos CP-11 a CP-61» y «Casos CP-62 a CP-365» más abajo. CP-01 a CP-04 se resuelven con `[DataTestMethod]`.
Cada método muestra su ID en el nombre visible: `[TestMethod("CP-xx: …")]`, o `DisplayName`
en cada `DataRow`.

> **Los diez casos son pruebas de integración contra SQL Server, no pruebas unitarias.**
> No hay ningún punto de inyección: `GestorParametros` lee los valores de las reglas de la
> tabla `Parametros` por `RepositorioParametros`, y siete archivos de `Repo/` dependen de él.
> Hasta CP-01 (Bronce, 0 %) necesita la base para resolver `PorcentajePara`. Se decidió no
> abrir un seam para las pruebas: cada caso usa la base real, por los mismos gestores que usa
> la aplicación.
>
> **Cómo se prepara la base.** `Pruebas/BaseDePrueba.cs`, en `[AssemblyInitialize]`, una vez
> por corrida: borra `BddEventAura_Pruebas` si existe (antes cierra el pool de conexiones y la
> pone en `SINGLE_USER`), la crea de nuevo desde `master` y le aplica `01-esquema.sql` y
> `02-semilla.sql`, partidos por las líneas que son exactamente `GO`. Después recarga
> `GestorParametros`. Se niega a correr si la cadena del `App.config` de `Pruebas/` no apunta a
> un catálogo terminado en `_Pruebas`: nunca toca la base de desarrollo.
>
> **Aislamiento.** Sin paralelismo (`[assembly: DoNotParallelize]`): comparten la base y los
> singletons. Cada prueba arma sus datos con `Pruebas/Fake.cs`, que marca todo lo que crea
> (mails `test…@pruebas.eventaura.com`, locales y servicios «test …») y lo borra por esa marca
> antes de cada prueba y al final. Nunca modifica la semilla. La sesión (`GestorSesion`) se
> cierra antes y después de cada prueba.
>
> **Cómo correrlas.** Tarea «Probar» de VS Code: compila con el MSBuild de Visual Studio y
> corre `vstest.console.exe`, ubicado con `vswhere`. `dotnet test` no sirve: no puede compilar
> los `.resx` de `Visual.csproj`, que es un proyecto de .NET Framework.
>
> Además de los diez casos y la verificación de referencia, `Pruebas/PruebasFormularios.cs`
> verifica RNF-03: ningún formulario declara más de 790 px de alto (ver CLAUDE.md).

| CP | Condición de entrada | Resultado esperado | Reglas |
|---|---|---|---|
| CP-01 | Cliente con 2 compras confirmadas en 12 meses | Bronce, descuento 0 % | RN-19, RN-20 |
| CP-02 | Cliente con 3 compras confirmadas | Plata, descuento 5 % | RN-19, RN-20 |
| CP-03 | Cliente con 6 compras confirmadas | Plata; no asciende | RN-19 |
| CP-04 | Cliente con 7 compras confirmadas | Oro, descuento 7,5 % | RN-19, RN-20 |
| CP-05 | Paquete Oro con entradas $40.000 y hospedaje $30.000 | Descuento $3.000; el hospedaje no se descuenta | RN-21 |
| CP-06 | Cliente con 7 compras, una hace 13 meses | Plata: la antigua queda fuera de la ventana | RN-19 |
| CP-07 | Cambia la tarifa con una compra ya iniciada | Se factura el precio congelado | RN-16 |
| CP-08 | Cliente con 6 compras realiza la séptima | La séptima se cobra al 5 %; Oro rige desde la octava | RN-22 |
| CP-09 | Cliente Oro con una compra reembolsada | Contador baja a 6; el nivel se mantiene hasta la reevaluación | RN-23, RN-24 |
| CP-10 | Cliente Oro con suscripción activa | Descuento 7,5 %; la suscripción no suma porcentaje | RN-37 |

## Verificación numérica de referencia

Cliente Plata, 2 entradas Campo a $25.000, hospedaje $45.000:

| Paso | Concepto | Importe |
|---|---|---|
| 1 | Base de cálculo, solo entradas | $50.000 |
| 2 | Porcentaje nivel Plata | 5 % |
| 3 | Monto del descuento | $2.500 |
| 4 | Subtotal de entradas | $47.500 |
| 5 | Hospedaje, sin descuento | $45.000 |
| 6 | Cargo por servicio, 15 % sobre el subtotal | $7.125 |
| 7 | **Total a pagar** | **$99.625** |

Si el cálculo no da exactamente $99.625, hay un error en la implementación.

Este cálculo es el caso **CP-11**.

## Casos CP-11 a CP-61

Mismas condiciones que los diez anteriores: integración contra `BddEventAura_Pruebas`, datos
de `Fake`, sesión cerrada antes y después. Los que dicen «interfaz» abren el formulario real
en un hilo STA y responden sus MessageBox con `Pruebas/DialogosPrueba.cs`. Necesitan una
sesión de escritorio abierta.

Cuántas veces corre cada uno: CP-15 tiene 4 variantes, CP-20 tiene 2, CP-25 tiene 3, CP-46
tiene 2, CP-47 tiene 4 y CP-51 tiene 3. Con CP-01 a CP-04, que son 4 filas de un solo método,
la batería suma **73 ejecuciones**.

| CP | Archivo | Condición de entrada | Resultado esperado | RF | Reglas | Hall. |
|---|---|---|---|---|---|---|
| CP-11 | PruebasFidelizacion | Plata, 2 entradas Campo a $25.000, hospedaje $45.000 | Los siete pasos de la verificación de referencia, total $99.625 | RF-05, RF-08 | RN-20, RN-21 | — |
| CP-12 | PruebasFormularios | Los 20 formularios de `Visual` (19 hasta SEG-2, que agregó `frmControlIngreso`) | Ninguno declara más de 790 px de alto | RNF-03 | — | — |
| CP-13 | PruebasInterfaz | Compra con título de 100 caracteres, 2 entradas por sector, 4 servicios y seguro; después, un evento agotado | Título, nivel y enlace sin superponerse; cada importe y el TOTAL completos, con la leyenda del descuento. En el agotado, servicios deshabilitados | RF-05 | RN-10 | H-01 |
| CP-14 | PruebasInterfaz | Abrir el alta de usuario | Todos los campos vacíos | RF-01 | — | — |
| CP-15 | PruebasInterfaz | Nombre, apellido o correo con solo espacios, o contraseña «abc» (4 variantes) | Aviso correspondiente; no se crea la cuenta | RF-01 | RN-02 | — |
| CP-16 | PruebasInterfaz | Búsqueda sin resultados | Una fila de aviso; grilla de solo lectura | RF-03 | — | — |
| CP-17 | PruebasInterfaz | «Comprar» sin evento elegido | Aviso «Seleccionar evento»; no abre la compra | RF-03 | — | — |
| CP-18 | PruebasInterfaz | Foco en la caja de búsqueda | Texto guía «Evento o dirección del local» visible | RF-03 | — | — |
| CP-19 | PruebasInterfaz | «Volver al listado» con paquete armado: No y después Sí | No es el botón por defecto; No conserva el paquete; Sí vuelve al listado | RF-05 | — | H-06 |
| CP-20 | PruebasInterfaz | Cantidad «abc» o «-1» (2 variantes) | Aviso y se restaura la cantidad anterior | RF-04 | — | — |
| CP-21 | PruebasInterfaz | Eliminar un evento sin ventas y otro con ventas | Confirmación con No por defecto; sin ventas se elimina; con ventas se ofrece suspender | RF-12 | RN-11 | — |
| CP-22 | PruebasInterfaz | Historial sin entradas elegidas | «Devolver» deshabilitado y gris | RF-05 | — | — |
| CP-23 | PruebasInterfaz | Tildar y destildar una entrada asegurada | «Devolver 1 entrada(s)» habilitado; vuelve a deshabilitarse | RF-05 | — | — |
| CP-24 | PruebasInterfaz | Evento agotado que recupera cupo por una devolución | Compra y servicios bloqueados; después, aviso RN-10, vuelve la grilla, solo los servicios del evento, sin inscribir en la lista. Desde T-26, «Comprar» se habilita recién al elegir una entrada | RF-05, RF-10 | RN-10, RN-14 | — |
| CP-25 | PruebasInterfaz | Compra a 125 % y 150 % en 1366 × 728 y 1920 × 1032 (3 variantes) | La ventana entra, se llega al final del resumen, «Volver» y «Comprar» a la vista | RF-05, RNF-03 | — | — |
| CP-26 | PruebasCuentas | Alta de un cliente y segunda alta con el mismo correo | Nace Bronce y sin suscripción; la segunda alta lanza RN-01 | RF-01 | RN-01, RN-03 | — |
| CP-27 | PruebasCuentas | Inicio de sesión correcto, con espacios alrededor del correo | Devuelve el usuario sin la contraseña y abre la sesión | RF-02 | — | — |
| CP-28 | PruebasCuentas | `LOGIN_INTENTOS_MAX` intentos fallidos; después, contraseña correcta; después, bloqueo vencido | Avisos con los intentos restantes; bloqueo por `LOGIN_BLOQUEO_MIN`; rechaza la contraseña correcta mientras dura; al vencer, entra y el contador vuelve a 0 | RF-02 | RN-04 | — |
| CP-29 | PruebasCuentas | Correo inexistente, correo vacío o contraseña vacía | Devuelve null; no hay sesión | RF-02 | — | — |
| CP-30 | PruebasCompra | Búsqueda por nombre, dirección, categoría y fecha; evento pasado; evento suspendido | Cada filtro incluye o excluye según corresponde. El evento pasado no aparece y rechaza paquete y lista de espera con RN-09. El suspendido no aparece | RF-03 | RN-09 | — |
| CP-31 | PruebasCompra | Más entradas que el cupo del sector; más que `COMPRA_ENTRADAS_MAX` sumando sectores; un servicio sin entradas | RN-08, RN-12 y RN-13 respectivamente | RF-04, RF-05 | RN-08, RN-12, RN-13 | — |
| CP-32 | PruebasCompra | Pago aprobado de 3 entradas | Compra Confirmada; 3 QR `EA-` + 32 hexadecimales, distintos; un pago Aprobado con los últimos 4 y el token, sin el número completo | RF-06, RF-07 | RN-17, RN-18, RN-30, RN-31 | — |
| CP-33 | PruebasCompra | Pago rechazado (tarjeta terminada en 0000) en un sector de capacidad 1 | Compra Pendiente; pago Rechazado registrado; el cupo vuelve y otro cliente puede reservarlo | RF-06, RF-07 | RN-17, RN-31, RN-32 | — |
| CP-34 | PruebasCompra | Reserva vencida al confirmar | RN-15; compra Vencida; cupo devuelto; ningún pago. La preparación atrasa juntas `FechaCompra` y `ReservaVence`, como lo haría el paso del tiempo | RF-06 | RN-15 | — |
| CP-35 | PruebasReportes | Pedir un reporte sin sesión, como Cliente y como Administrador | RN-34 en los dos primeros; el Administrador lo obtiene | RF-11 | RN-34 | — |
| CP-36 | PruebasReportes | RPT-01 con 2 compras confirmadas (1 y 2 entradas) y una solo iniciada | 3 entradas, $75.000, ticket $25.000, 30,0 % de ocupación | RF-11 | — | — |
| CP-37 | PruebasReportes | RPT-05 de un evento agotado con un inscripto; inscripción repetida; después, devolución | «Agotado» con 1 en espera; RN-36 al repetir; tras la devolución, «Con cupo», y con «solo agotados» no aparece | RF-10, RF-11 | RN-10, RN-36 | — |
| CP-38 | PruebasReportes | Exportar con un exportador de prueba; período invertido; código inexistente | El exportador recibe las mismas filas; las dos llamadas inválidas lanzan `ArgumentException` | RF-11 | — | — |
| CP-39 | PruebasInterfaz | Pantalla de pago recién abierta y con la reserva en el último minuto | «Reserva: mm:ss» a la vista, en violeta; en rojo en el último minuto | RF-06 | — | H-02 |
| CP-40 | PruebasInterfaz | Evento de capacidad 5 con 1 vendida | La grilla muestra 4 disponibles; solo «Cantidad» es editable | RF-04 | — | H-03 |
| CP-41 | PruebasInterfaz | Comprar sin tildar el seguro y responder No | Aviso «Seguro de entrada» con No por defecto; no se reserva nada | RF-05 | — | H-04 |
| CP-42 | PruebasInterfaz | Clic en «¿Cómo funcionan los niveles?» | Aviso con los umbrales vigentes de `Parametros` | RF-08 | — | H-05 |
| CP-43 | PruebasCompra | Compra iniciada cuya reserva registrada ya venció, aunque `FechaCompra` + `RESERVA_MINUTOS` todavía no | RN-15; compra Vencida; cupo devuelto; ningún pago. El plazo lo fija la reserva, no el parámetro vigente | RF-06 | RN-15 | — |
| CP-44 | PruebasInterfaz | Abrir el alta de usuario y registrarse con datos válidos | No hay selector de perfil; el usuario creado es Cliente | RF-01 | RN-05 | — |
| CP-45 | PruebasCuentas | `RegistrarCliente` con un correo nuevo | Crea un Cliente de nivel Bronce | RF-01 | RN-03 | — |
| CP-46 | PruebasInterfaz | «Volver al inicio» desde el menú del cliente y desde el del administrador (2 variantes) | La sesión queda cerrada y se ve el login | RF-02 | — | — |
| CP-47 | PruebasInterfaz | Decidir si la aplicación termina al cerrar una ventana (4 variantes) | Termina solo si el usuario cerró la última ventana visible | RNF-02 | — | — |
| CP-48 | PruebasInterfaz | Cerrar «Armar paquete» con la X, con y sin paquete | Con paquete pide confirmación, con No por defecto; No conserva el paquete; sin paquete cierra sin preguntar | RF-05 | — | — |
| CP-49 | PruebasInterfaz | Abrir la compra del evento 1 | Foco en la celda Cantidad del primer sector con cupo; encabezado con «escribí»; ayuda con el máximo por compra | RF-04 | — | H-03 |
| CP-50 | PruebasInterfaz | Abrir la compra; poner 1 entrada; volver a 0 | «Comprar» y el seguro deshabilitados sin entradas y habilitados con una | RF-05 | — | — |
| CP-51 | PruebasInterfaz | Error de base de datos, error de Negocio real (`GestorPaquetes.Nuevo` con un evento inexistente) y error genérico (3 variantes) | El manejador global devuelve el mensaje de base de datos, el de Negocio y el general; ninguno con detalle técnico | RNF-02 | — | — |
| CP-52 | PruebasInterfaz | Pantalla de pago abierta; se elimina el evento; «Pagar» | Aviso «Pago no realizado»; la pantalla devuelve Abort; ningún pago registrado | RF-06 | — | — |
| CP-53 | PruebasFormularios | Texto de `Visual/Program.cs` | `ManejadorErrores.Instalar();` está antes de `Application.Run` | RNF-02 | — | — |
| CP-54 | PruebasInterfaz | Abrir la compra del evento 1 | Cada encabezado visible de la grilla de sectores entra completo en su alto, con ajuste de línea | RF-04 | — | H-03 |
| CP-55 | PruebasRechazosNegocio | `Registrar` con un perfil inexistente | RN-05; no se crea la cuenta | RF-01 | RN-05 | — |
| CP-56 | PruebasRechazosNegocio | Eliminar el único sector de un evento publicado | RN-06; el sector y el estado del evento no cambian | RF-12 | RN-06 | — |
| CP-57 | PruebasRechazosNegocio | Agregar al paquete un servicio de otro evento | RN-14; el paquete conserva sus entradas y no suma el servicio | RF-05 | RN-14 | — |
| CP-58 | PruebasRechazosNegocio | Contratar el plan anual con el mensual activo | RN-25; sigue activa la suscripción original, con su plan | RF-13 | RN-25 | — |
| CP-59 | PruebasRechazosNegocio | Devolver una compra sin seguro; devolver una asegurada fuera del plazo | RN-29 en los dos; las compras siguen confirmadas y sin reembolso | RF-05 | RN-29 | — |
| CP-60 | PruebasRechazosNegocio | Agregar un sector que, sumado al existente, supera la capacidad del local | RN-38; no se inserta y el sector original no cambia | RF-12 | RN-38 | — |
| CP-61 | PruebasRechazosNegocio | Iniciar un paquete en preventa sin suscripción activa | RN-39; el cupo no cambia | RF-05, RF-13 | RN-39 | — |

CP-39 a CP-42 comprueban que la interfaz **comunica** RN-15, RN-08, RN-28 y RN-19. No las
verifican: por eso esas reglas no figuran en su columna.

CP-55 a CP-61 provocan el rechazo de las siete reglas que lanzaban `ReglaNegocioException` sin
una prueba de ese camino, y comprueban que el estado queda como estaba. Cubren el rechazo
documentado, no todas sus variantes: CP-56 prueba eliminar el último sector, no publicar sin
sectores; CP-60 prueba agregar un sector, no modificarlo. En la matriz, RN-05 está en la fila
de RF-02 y RN-39 en la de RF-13: por eso CP-55 y CP-61 llevan dos requerimientos.

## Casos CP-62 a CP-365

Registrados por Claude Code el 13/11/2026; CP-116 a CP-145, el 16/11; CP-146 a CP-148, en el cierre de Codex (`dba674c`); CP-150 a CP-206, el 19/11, sobre `c4ab61e`, junto con la corrección de las columnas RF y Reglas de CP-197 a CP-199, que había cambiado Codex en los comentarios; CP-207 a CP-211 y CP-354 a CP-357, el 24/11, sobre `2db272e`. CP-358 a CP-360 y CP-364 a CP-365 se añadieron el 26/11. Los números CP-149, CP-212 a CP-353 y CP-361 a CP-363 no se usaron: Codex saltó de CP-211 a CP-354 al numerar, y los números no se reasignan. Salen del comentario `// CP-xx — RF, RN` y del nombre
visible de cada prueba en `Pruebas/`, leídos uno por uno. Para CP-62 a CP-357, la procedencia
histórica de las ejecuciones sigue en `Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-12-T47-T48.md`;
los casos añadidos después pasaron en la corrida 365/365 de
`TestResults/bateria-365-2026-11-26.md`, sobre el commit `7efc928430b308b16aa0bf37e022e8fa77126bea`.

Valen las mismas condiciones que para CP-11 a CP-61:
- integración contra `BddEventAura_Pruebas`;
- datos de `Fake`;
- sesión cerrada antes y después de cada prueba.

Los que dicen «interfaz» abren el formulario real en un hilo STA y necesitan una sesión de
escritorio abierta.

**Cuántas veces corre cada uno:**

| Ejecuciones | Casos |
|---|---|
| 25 | CP-116 |
| 21 | CP-63 |
| 8 | CP-88 y CP-125 |
| 7 | CP-112 |
| 5 | CP-82 y CP-87 |
| 6 | CP-115, CP-118 y CP-123 |
| 4 | CP-113 y CP-117 |
| 3 | CP-72, CP-81, CP-85, CP-114 y CP-124 |
| 2 | CP-66, CP-68, CP-69, CP-70, CP-71, CP-73, CP-74, CP-75, CP-77, CP-79, CP-80, CP-83, CP-89, CP-105, CP-111, CP-119, CP-120, CP-146 y CP-147 |
| 1 | El resto |

CP-62 a CP-148 suman **209 ejecuciones**; CP-150 a CP-206, **69**, CP-207 a CP-211 y CP-354 a CP-357, **9**
(una cada uno), y CP-358 a CP-360 y CP-364 a CP-365, **5** (una cada uno). Con las 73 de CP-01 a CP-61,
la batería da **365**. Sobre `967e038` eran 206 de CP-62 a CP-148: desde entonces CP-116 sumó tres pantallas
(de 22 a 25: `frmAuditoria`, `frmRevisarSolicitudesOrganizador` y `frmSolicitudOrganizador`). Sobre `dba674c` eran 203
de CP-62 a CP-148: desde entonces CP-63 sumó una pantalla (de 20 a 21) y CP-116, dos (de 20 a 22).
En el cierre de Codex (`dba674c`) se sumaron dos de CP-146, dos de CP-147 y una de CP-148 a las 271
de `65225e7`; la limpieza de las ocho filas de CP-125 no agregó ninguna.
Contadas en la salida de la segunda corrida de Codex del 16/11 (`TestResults/Cola16/final.log`):
271 líneas «Correctas». Respecto de la primera corrida del 16/11 (243) se suman las 8 de CP-125 y
una de cada caso entre CP-126 y CP-145.

**CP-125, resuelto por Codex en el cierre:** las ocho filas muestran su ID y su descripción en la
corrida, y las tildes de «después» e «inscripción» están corregidas.

**Ningún caso falla en la segunda corrida del 16/11: 271 de 271.** Antes, en la primera, 243 de 243. Los tres que fallaban el 14/11 (la fila
de 150 % de CP-69, CP-76 y la fila del propietario de CP-85, todos de aspecto a escala 150 %)
pasan desde el rediseño de pantallas de Codex (`96fd6ec` y `027527f`). Claude Code repitió la
batería en el árbol principal sobre `65225e7`, después del congelamiento: **271 de 271**, el mismo
resultado que Codex. Sobre `dba674c`, el código que se entrega, los dos dieron **276 de 276**.

CP-81, que fallaba en la batería del 12/11, pasa desde `2af267a` (Codex corrigió la preventa).

**La columna «Hall.»** dice qué prueba manual reemplaza el caso. Los PA-xx son de
`docs/PRUEBAS-64-CLASIFICACION.md`, y entre paréntesis van las filas de esa planilla. Los
SEG-x y T-xx son los encargos que los originaron.

**Las reglas RN-40 a RN-42** figuran como «propuesta» en el comentario de la prueba. Acá van
con su número, porque la auditoría las da por aprobadas (`AUDITORIA-DOCUMENTO-CODIGO.md`,
sección 0).

| CP | Archivo | Condición de entrada | Resultado esperado | RF | Reglas | Hall. |
|---|---|---|---|---|---|---|
| CP-62 | PruebasFormularios | Los colores y las fuentes de `Tema` | Cada par de texto y fondo cumple el contraste WCAG (el foco sobre el primario, al menos 3:1); las familias tipográficas existen en el sistema | RNF-02 | — | 6A |
| CP-63 | PruebasFormularios | El código de cada una de las 21 pantallas que tienen Designer, y de su Designer (21 variantes) | Ninguna declara colores, fuentes ni medidas propias: el estilo sale de `Tema` | RNF-02 | — | T-48 |
| CP-64 | PruebasFormularios | El proyecto `Visual` | El DPI se declara en el manifiesto, sin llamadas manuales en `Program` | RNF-03 | — | 6A |
| CP-65 | PruebasInterfaz | Ticket PDF de una entrada confirmada, generado en una carpeta temporal | El texto del PDF conserva evento, sector, número de pedido y el código QR completo | RF-07 | RN-18 | — |
| CP-66 | PruebasGrupoALogin | Abrir el login; iniciar sesión como Administrador (2 variantes) | El foco arranca en el correo; el Administrador llega al menú de administración de la plataforma y queda en la sesión con ese perfil | RF-02 | RN-05 | PA-01 (3.1, C.1) |
| CP-67 | PruebasGrupoALogin | Dos inicios de sesión seguidos del Administrador; un cliente con contraseña heredada en texto plano | Los dos entran; la contraseña heredada se acepta una vez y queda guardada como hash | RF-02 | — | PA-02 (3.2) |
| CP-68 | PruebasGrupoALogin | Contraseñas incorrectas desde la pantalla de login, hasta el bloqueo; parámetros de bloqueo de 10 y 5 minutos (2 variantes) | Muestra los intentos restantes y bloquea al quinto fallo; el aviso dice los minutos del parámetro vigente | RF-02 | RN-04 | PA-03 (C.2, C.3) |
| CP-69 | PruebasGrupoAPropietario | «Agregar evento» al 100 % y al 150 % (2 variantes) | El selector de categoría y las acciones quedan dentro de la ventana | RF-12, RNF-03 | RN-39 | PA-04 (C9.1, 9.17) |
| CP-70 | PruebasGrupoAPropietario | Recorrer «Registrar local» con Tab; dar de alta un local (2 variantes) | El orden de Tab sigue los campos; el local queda en la base y aparece en «Mis locales» | RF-12 | — | PA-05 (4.6, C.4) |
| CP-71 | PruebasGrupoAPropietario | «Mis locales» con dos locales; mover la celda actual al segundo (2 variantes) | La grilla aplica las propiedades y los colores del tema; se abren los eventos del segundo local | RF-12, RNF-02 | — | PA-06 (C.12, C.13) |
| CP-72 | PruebasGrupoAPropietario | El botón de sectores; precio «12500,50» y «12500.50» (3 variantes) | El botón dice «Agregar sector»; los dos precios se guardan como 12.500,50 | RF-12 | — | PA-07 (4.2, C.5) |
| CP-73 | PruebasGrupoAPropietario | Alta de un evento con categoría y sin categoría (2 variantes) | Con categoría, se guarda y se lista como borrador; sin categoría, no se inserta | RF-12 | RN-06 | PA-08 (C.6, C.7) |
| CP-74 | PruebasGrupoAPropietario | Agregar a un borrador un sector de capacidad 0 y uno de capacidad 10 (2 variantes), luego publicar explícitamente | Agregar el sector nunca publica: el borrador sigue en Borrador. Al publicar el primer evento del Organizador queda EnRevision | RF-12 | RN-06, RN-54 | PA-09 (C.8, C.9) |
| CP-75 | PruebasGrupoAPropietario | Desde la pantalla, un sector que supera la capacidad del local; eliminar el último sector (2 variantes) | RN-38 informa la capacidad disponible y no inserta; RN-06 informa el código y conserva el sector | RF-12 | RN-06, RN-38 | PA-10 (C.10, C.11) |
| CP-76 | PruebasGrupoASuscripcion | «Mi suscripción» sin plan | Muestra precios, beneficios y controles completos, sin recortes | RF-13, RNF-02 | — | PA-11 (8.3) |
| CP-77 | PruebasGrupoASuscripcion | Contratar el plan mensual; cancelarlo (2 variantes) | Al contratar, muestra precio y vencimiento y habilita los beneficios. Al cancelar, pide confirmación con No por defecto y quita los beneficios | RF-13 | RN-25 | PA-12 (8.4, 8.6) |
| CP-78 | PruebasGrupoASuscripcion | Dos ventanas de suscripción abiertas; contratar en las dos | La segunda se rechaza con RN-25: no se crean dos planes | RF-13 | RN-25 | PA-13 (8.5) |
| CP-79 | PruebasGrupoASuscripcion | Resumen de compra de 2 entradas con suscripción y sin ella (2 variantes) | El suscriptor tiene el seguro sin cargo y no recibe la pregunta del seguro; sin suscripción, el seguro de 2 entradas cuesta $10.000 | RF-05 | RN-37 | PA-14 (8.7, 8.9) |
| CP-80 | PruebasGrupoASuscripcion | Pagar con seguro siendo suscriptor; pagar sin seguro (2 variantes) | Con seguro, una póliza gratuita por entrada; sin seguro, ninguna póliza | RF-06, RF-07 | RN-37 | PA-15 (8.8, C9.10, C9.11) |
| CP-81 | PruebasGrupoAPreventa | Evento en preventa: cliente sin plan, suscriptor; evento sin preventa (3 variantes) | El evento se lista; al cliente sin plan se le rechaza con la fecha y hora en que termina la preventa; el suscriptor abre la compra; sin preventa compran todos. **Falla:** la preventa rechazada deja visible la compra | RF-03, RF-05 | RN-39 | PA-16 (8.21, 8.22, C9.4, C9.5, C9.8) |
| CP-82 | PruebasGrupoAPreventa | Alta con preventa; preventa posterior al evento; administrar con preventa; quitarla; administrar sin preventa (5 variantes) | La fecha se guarda y se conserva al publicar en revisión; la posterior al evento se rechaza sin insertar; la pantalla muestra lo guardado; quitarla guarda NULL | RF-12 | RN-06, RN-39, RN-54 | PA-17 (C9.2, C9.3, C9.6, C9.7, C9.9) |
| CP-83 | PruebasGrupoAListasYMenus | Lista de espera de un evento agotado; un organizador sin locales ni listas (2 variantes) | Columnas, prioridad y contadores a la vista; sin listas, el desplegable queda deshabilitado | RF-10 | RN-35 | PA-18 (8.23, 8.25) |
| CP-84 | PruebasGrupoAListasYMenus | Un suscriptor inscripto en la lista cancela su plan | Conserva la prioridad con que se inscribió | RF-10 | RN-35 | PA-19 (8.24) |
| CP-85 | PruebasGrupoAListasYMenus | Menú del organizador al 100 % y al 150 %; menú del cliente; código de `Visual` (3 variantes) | Las acciones del organizador siguen visibles; el cliente no ve reportes; `Visual` no tiene conexiones ni comandos SQL. | RF-11, RF-12, RF-22, RNF-02, RNF-03 | — | PA-20 (9.1, 9.16, 8.26) |
| CP-86 | PruebasGrupoAReportes | La semilla sola, sin datos de volumen | Los cinco reportes se generan y se exportan | RF-11 | — | PA-21 (9.15) |
| CP-87 | PruebasGrupoAReportes | Los filtros de cada reporte, RPT-01 a RPT-05 (5 variantes) | Valores iniciales, visibilidad y limpieza correctos en cada uno | RF-11 | — | PA-22 (9.2, 9.3) |
| CP-88 | PruebasGrupoAReportes | Los datos de demostración del anexo A02 (8 variantes) | RPT-01 a RPT-05 reproducen las cifras del anexo y sus filtros; RPT-01 alinea los números, resalta el TOTAL y muestra la lectura | RF-11, RNF-02 | — | PA-23 (9.4 a 9.10) |
| CP-89 | PruebasGrupoAReportes | Exportar a una carpeta temporal; exportar sobre un archivo bloqueado (2 variantes) | Exporta y ofrece abrir el PDF; con el archivo bloqueado, informa el error y la pantalla sigue igual | RF-11 | — | PA-24 (9.11, 9.14) |
| CP-90 | PruebasGrupoAReportes | PDF exportado desde la pantalla de reportes | El texto contiene la marca, el código, el período, quién lo pidió y la paginación | RF-11 | — | PA-25 (9.12 A) |
| CP-91 | PruebasSeguridad | Sesión de un cliente que intenta devolver la compra de otro, con el titular del objeto cambiado a mano | Rechazado; tampoco ve las entradas. La compra sigue Confirmada y sin reembolso | RF-08 | RN-41 | SEG-3 |
| CP-92 | PruebasSeguridad | Sesión de un cliente que pide el historial, la suscripción y el perfil de otro | Los tres, rechazados | RF-08 | RN-41 | SEG-3 |
| CP-93 | PruebasSeguridad | Sesión de un cliente que elimina un evento | Rechazado con RN-48; el evento sigue existiendo | RF-12 | RN-48 | SEG-3 |
| CP-94 | PruebasSeguridad | Sin sesión: 16 operaciones protegidas y `CrearAdministrador`; después, `CrearAdministrador` como cliente y como Administrador | Las 17 se rechazan sin sesión, y el cliente tampoco puede. El Administrador crea la cuenta con perfil Propietario. El registro genérico no es público. La compra sigue Pendiente y sin suscripción | RF-04, RF-12, RF-13 | RN-41, RN-48 | SEG-3, SEG-4 |
| CP-95 | PruebasSeguridad | Cambiar la contraseña con la actual incorrecta | Rechazado; el contador de intentos sube a 1 y la contraseña anterior sigue sirviendo | RF-02 | RN-04, RN-40 | SEG-1 |
| CP-96 | PruebasSeguridad | Cambiar la contraseña de una cuenta bloqueada, con la actual correcta | Rechazado con el mensaje genérico «Mail o contraseña actual incorrectos.», sin decir que está bloqueada; al vencer el bloqueo, la anterior sigue sirviendo | RF-02 | RN-04 | SEG-1, K-8 |
| CP-97 | PruebasSeguridad | Cambiar la contraseña con un correo inexistente y con la actual incorrecta | Las dos respuestas son iguales: misma regla y mismo mensaje | RF-02 | RN-40 | SEG-1 |
| CP-98 | PruebasSeguridad | Con la sesión del Administrador, cambiar la contraseña de un cliente; abrir la pantalla | La anterior deja de servir y la nueva sirve; la sesión sigue siendo del Administrador. Pantalla: tres campos enmascarados y vacíos, todo dentro de la ventana | RF-02 | RN-40 | SEG-1 |
| CP-99 | PruebasIngresos | Entrada confirmada, del evento elegido y dentro del horario; después, un cliente que intenta registrarla y una consulta del contador sin sesión | Ingresa: `FechaIngreso` queda registrada, el estado de venta no cambia y el contador da 1. En la pantalla, el segundo escaneo dice «ya ingresó». Al cliente se le rechaza el registro por RN-48, sin alterar el ingreso; sin sesión, el contador se rechaza | RF-07 | RN-18, RN-48 | SEG-2 |
| CP-100 | PruebasIngresos | El mismo código escaneado dos veces | El segundo se rechaza como ya usado, con la fecha y hora del primero | RF-07 | RN-18 | SEG-2 |
| CP-101 | PruebasIngresos | Entrada reembolsada | Se rechaza como reembolsada; el contador queda en 0 | RF-07 | RN-18, RN-29 | SEG-2 |
| CP-102 | PruebasIngresos | Código con formato válido que no existe | Se rechaza como inexistente; el contador queda en 0 | RF-07 | RN-18 | SEG-2 |
| CP-103 | PruebasIngresos | La entrada en otro evento; el evento dentro de 2 días y hace 2 días | Rechazada por otro evento y por fuera de horario; `FechaIngreso` sigue vacía | RF-07 | RN-18 | SEG-2 |
| CP-104 | PruebasIngresos | Diez puertas escanean el mismo código a la vez, arrancadas juntas por una barrera; cien rondas con un código nuevo cada una | En cada ronda, exactamente un ingreso válido y nueve «ya usado», comprobado en la base. Pasó sin cambiar el SQL de ingreso | RF-07 | RN-18 | SEG-2 |
| CP-105 | MedirLoginSeguridad | Inicio de sesión real con hashes de 600.000 y 310.000 iteraciones (2 variantes) | Mide el tiempo. El hash de 600.000 no se reescribe con menos iteraciones. **Es una medición, no se cuenta como prueba de seguridad** | RF-02 | — | SEG-6 |
| CP-106 | PruebasSeguridad | Buscar «[vip]», «50%» y «sector_unico» | Cada texto se busca literal: encuentra el evento, y «sector%unico» y «sector_unic_» no encuentran nada | RF-03 | — | SEG-7 |
| CP-107 | PruebasSeguridad | Hashes con iteraciones enormes, 0, negativas o no numéricas, y con sal y hash vacíos | Ninguno autentica y no se propaga ninguna excepción | RF-02 | — | SEG-8 |
| CP-108 | PruebasSeguridad | Hash previo de 100.000 iteraciones | Verifica; al iniciar sesión se reescribe a 310.000 con la misma contraseña, y un segundo inicio no lo vuelve a reescribir | RF-02 | — | SEG-6 |
| CP-109 | MedirLoginSeguridad | Preparación completa de la base de pruebas | Carga 1.178 clientes de reporte con hashes a 310.000; la guarda `_Pruebas` sigue activa | RF-11 | — | SEG-6 |
| CP-110 | PruebasIntegridadEntradas | Cada estado de `EstadoEntrada` y el estado ajeno «Usada», escritos en la tabla | La base admite los cuatro y rechaza el ajeno por `CK_Entradas_Estado`, que está activa y confiable | RF-07 | RN-18 | — |
| CP-111 | PruebasReembolsoConIngreso | Compra asegurada de 2 entradas: una ya ingresó; devolver las dos, en los dos órdenes (2 variantes) | RN-29 rechaza la devolución y no cambia nada, ni por el gestor ni por el repositorio. La entrada no usada se puede devolver sola | RF-07 | RN-18, RN-29 | SEG-2 |
| CP-112 | PruebasSerializacionComponentes | Los siete componentes visuales en un host de diseño real (7 variantes) | Ninguno guarda su estilo en el Designer | RF-05, RNF-02 | — | T-41 |
| CP-113 | PruebasRevisionSeguridad | Contrato de la sesión: `Iniciar` fuera de lo público; propiedades de la sesión; perfil e identidad del usuario modificados después de iniciar (4 variantes) | `Iniciar` es `internal`; la sesión solo se lee; cambiar el objeto `Usuario` no eleva el perfil ni suplanta otra cuenta | RF-02 | RN-41 | K-6 (R-01) |
| CP-114 | PruebasRevisionSeguridad | Inicio de sesión con correo existente e inexistente; quinto fallo; cuenta bloqueada (3 variantes) | El rechazo es idéntico en texto y título, sin revelar si la cuenta existe ni si está bloqueada; el bloqueo se registra igual | RF-02 | RN-04 | K-3 |
| CP-115 | PruebasRevisionSeguridad | Descuento configurado de Plata y de Oro por debajo, igual y por encima de `DESCUENTO_MAX_ADMISIBLE` (6 variantes) | Por debajo o igual se aplica; por encima se rechaza con RN-20, sin recortarlo en silencio | RF-08 | RN-20 | K-4 |
| CP-116 | PruebasTextoBotones | El texto de cada botón de 25 pantallas, dibujado por Windows, también con el borde de foco (25 variantes) | Ningún botón recorta su texto | RNF-02, RNF-03 | — | Rediseño |
| CP-117 | PruebasListaEsperaCliente | Consultar la posición en la lista de espera: la propia, con suscriptores adelante, la de otro cliente y sin sesión (4 variantes) | El cliente ve su posición, que respeta la prioridad de los suscriptores; la de otro y sin sesión se rechazan | RF-10 | RN-35, RN-41 | D-6 |
| CP-118 | PruebasListaEsperaCliente | Aviso de disponibilidad: evento agotado, entrada liberada, avisos de otra cuenta, sin sesión, reserva que ocupa el último cupo, evento suspendido (6 variantes) | Solo hay aviso al iniciar sesión cuando de verdad se puede comprar; no se leen avisos ajenos ni sin sesión | RF-10 | RN-41 | D-6 |
| CP-119 | PruebasExtrasVisuales | Resumen de compra vacío y con dos entradas (2 variantes) | El TOTAL destacado coincide con el resumen, y el indicador marca el paso correcto | RF-05, RNF-02 | — | Rediseño |
| CP-120 | PruebasExtrasVisuales | Los componentes nuevos `IndicadorPasos` y `EtiquetaTotal` en un host de diseño real (2 variantes) | Ninguno guarda su estilo en el Designer | RF-05, RNF-02 | — | Rediseño |
| CP-121 | PruebasReglasD5 | Suscripción contratada y vencida; consultarla y abrir su pantalla varias veces | No queda ninguna activa, y consultar o abrir la pantalla no crea ni renueva el registro | RF-13 | RN-26 | D-5 |
| CP-122 | PruebasReglasD5 | Compra confirmada sin seguro; volver a confirmarla agregándole seguro | Rechazo con «La compra ya no está pendiente de pago.»; la compra queda sin seguro, sin póliza y sin segundo pago | RF-06 | RN-28 | D-5 |
| CP-123 | PruebasK7K8 | Eventos para el control de ingreso: de otro organizador, sin sesión, y un evento de las 00:30 en los bordes de su ventana (6 variantes) | No se listan, registran ni cuentan eventos ajenos; sin sesión, rechazo. El evento aparece desde el segundo exacto de apertura hasta el de cierre, y no un segundo antes ni después | RF-07 | RN-18, RN-48, RN-49 | K-7 |
| CP-124 | PruebasK7K8 | Cambiar la contraseña sin sesión, con 0, 4 y 5 fallos previos, y con un correo inexistente (3 variantes) | El rechazo es el mismo en texto, título y comportamiento: no revela si la cuenta existe ni si está bloqueada. El bloqueo interno sigue funcionando | RF-02 | RN-04, RN-40 | K-8 |
| CP-125 | PruebasPrioridadTemporal | Un sector vuelve a tener una entrada (devolución, reserva vencida o pago rechazado) y hay un suscriptor inscripto en la lista de espera; compran un no suscriptor, el suscriptor inscripto, un suscriptor sin inscripción y un reintento de pago anterior, dentro y fuera de la ventana (8 variantes) | Dentro de la ventana solo compra el suscriptor inscripto; los demás reciben el rechazo RN-35 sin consumir el cupo ni dejar compra o pago nuevo. Fuera de la ventana compra cualquiera | RF-10 | RN-35 | RN-35 |
| CP-126 | PruebasGuardasMutacion | Pedir la lista de espera de un evento como cliente y sin sesión | Rechazo RN-34 sin devolver nombres ni correos; el administrador la obtiene | RF-10 | RN-34 | Mutación |
| CP-127 | PruebasGuardasMutacion | Pedir los pagos de una compra ajena y sin sesión | Rechazo RN-41 sin devolver el dato; el titular los obtiene | RF-06 | RN-41 | Mutación |
| CP-128 | PruebasGuardasMutacion | Pedir por su número una compra de otro cliente, y sin sesión | Rechazo RN-41 sin devolver la compra | RF-07 | RN-41 | Mutación |
| CP-129 | PruebasGuardasMutacion | Pedir las pólizas de seguro de otro cliente, y sin sesión | Rechazo RN-41 sin devolver las pólizas | RF-06 | RN-41 | Mutación |
| CP-130 | PruebasGuardasMutacion | Iniciar sesión usando como contraseña el hash guardado de una cuenta ya migrada a PBKDF2 | No autentica ni se toma como contraseña para migrar | RF-02 | RN-04 | Mutación |
| CP-131 | PruebasGuardasMutacion | Registrar el ingreso de una entrada de un evento suspendido, dentro del horario de ingreso | Rechazo; la entrada no queda ingresada | RF-07 | RN-18 | Mutación |
| CP-132 | PruebasGuardasMutacion | Registrar en la base el ingreso de una entrada contra un evento distinto del suyo, sin el filtro previo del gestor | La base no registra el ingreso | RF-07 | RN-18 | Mutación |
| CP-133 | PruebasGuardasMutacion | La entrada se devuelve entre la lectura y la escritura del ingreso | La base no registra el ingreso de una entrada que ya no está confirmada | RF-07 | RN-18, RN-29 | Mutación |
| CP-134 | PruebasGuardasMutacion | Registrar el ingreso de una entrada pendiente y de una vencida | Cada una recibe su rechazo específico y ninguna queda ingresada | RF-07 | RN-18 | Mutación |
| CP-135 | PruebasGuardasMutacion | Registrar ingresos en el segundo exacto de apertura y de cierre de la ventana, y un segundo antes y después | Los dos bordes se aceptan; los segundos de afuera se rechazan | RF-07 | RN-18 | Mutación |
| CP-136 | PruebasGuardasMutacion | Leer un código de entrada con espacios alrededor | El código se normaliza y el ingreso queda registrado | RF-07 | RN-18 | Mutación |
| CP-137 | PruebasGuardasMutacion | Consultar si otro cliente está inscripto en una lista de espera, y sin sesión | Rechazo RN-41 sin devolver el dato | RF-10 | RN-41 | Mutación |
| CP-138 | PruebasGuardasMutacion | Contar las compras de fidelización de otro cliente, y sin sesión | Rechazo RN-41 sin devolver el dato | RF-08 | RN-41 | Mutación |
| CP-139 | PruebasGuardasMutacion | Calcular el descuento de otro cliente, y sin sesión | Rechazo RN-41 sin devolver el dato | RF-08 | RN-41 | Mutación |
| CP-140 | PruebasGuardasMutacion | Armar un paquete a nombre de otro cliente, y sin sesión | Rechazo RN-41; no se crea el paquete | RF-05 | RN-41 | Mutación |
| CP-141 | PruebasGuardasMutacion | Consultar los locales de un organizador sin la sesión de ese organizador, y con ella | Sin perfil Organizador, rechazo RN-48 sin devolver el dato; el organizador dueño ve sus locales | RF-12, RF-22 | RN-48 | Mutación |
| CP-142 | PruebasGuardasMutacion | Listar los eventos con ingreso abierto sin sesión y con sesión de Organizador | Sin perfil Organizador, rechazo RN-48 sin devolver el listado; el Organizador puede consultar los eventos propios | RF-07, RF-22 | RN-48 | Mutación |
| CP-143 | PruebasGuardasMutacion | Ejecutar un reporte ya creado después de cerrar la sesión del administrador o como cliente | Rechazo RN-34 sin devolver el reporte | RF-11 | RN-34 | Mutación |
| CP-144 | PruebasGuardasMutacion | Pedir el catálogo de eventos de un reporte como cliente, y sin sesión | Rechazo RN-34 sin devolver el catálogo | RF-11 | RN-34 | Mutación |
| CP-145 | PruebasGuardasMutacion | Insertar dos entradas con el mismo código QR | El índice único de la base rechaza la segunda y la reversión deja intacta la primera | RF-07 | RN-18 | Mutación |
| CP-146 | PruebasRechazoIngreso | Registrar el ingreso de una entrada como cliente y sin sesión (2 variantes) | El rechazo es un resultado, no una excepción: motivo «no autorizado», regla RN-34 y el mensaje «Esta operación es solo para el Administrador». La entrada sigue confirmada y sin ingreso, el contador da 0 y la sesión no cambia | RF-07 | RN-34 | M-22 |
| CP-147 | PruebasSeguroUnico | Insertar en la base un segundo seguro para una entrada que ya tiene uno, y uno para otra entrada (2 variantes) | El índice único de la base rechaza el segundo seguro de la misma entrada; la otra entrada admite el suyo. La reversión deja todo como estaba | RF-06 | RN-28 | Índice de seguros |
| CP-148 | CapturasUltimasUI | Comprar, pagar y cerrar la pantalla de confirmación (interfaz) | Vuelve al listado de eventos, sin mostrar un armado vacío, y la sesión sigue siendo del cliente | RF-06 | — | U-09 |
| CP-149 | — | Número no usado: se saltó al numerar los casos y no se reasigna | — | — | — | — |
| CP-150 | PruebasMapaSectores | Mapa con Campo ($100, con cupo) y VIP ($200, agotado); clic en cada zona | Las zonas se ordenan por precio; el clic en VIP, agotado, no elige nada, y el de Campo lo elige | RF-18 | — | Mapa de sectores |
| CP-151 | PruebasMapaSectores | Cliente en la compra de un evento con un sector; clic en la zona del mapa (interfaz) | La grilla de sectores pasa a 1 entrada y el resumen dice «1 x Campo»: el mapa arma por el mismo gestor | RF-04, RF-18 | RN-08 | Mapa de sectores |
| CP-152 | PruebasIdentidadAura | Barra superior de la marca en un host de diseño real | No guarda tipografía ni colores en el Designer; no fija un tamaño mínimo y mide 60 px de alto | RF-05, RNF-02 | — | Identidad visual |
| CP-153 | PruebasIdentidadAura | Archivo `Visual/Recursos/EventAura.ico` | Contiene cinco imágenes válidas, de 16, 32, 48, 64 y 256 px | RF-05, RNF-02 | — | Identidad visual |
| CP-154 | PruebasIdentidadAura | Sesión abierta de un cliente que ya no existe en la base; se actualiza la barra (interfaz) | La barra no falla ni muestra un usuario | RF-02 | RN-05 | Identidad visual |
| CP-155 | PruebasIdentidadAura | Sin sesión, actualizar y dibujar la barra | Muestra solo la marca, sin usuario, y se dibuja sin error | RF-02 | RN-05 | Identidad visual |
| CP-156 | PruebasEjecucionSinDialogos | Programa de ensayo cuyo formulario lanza una excepción en `Load` | El proceso termina en menos de 5 s, con código de error y el mensaje de la excepción, sin quedar esperando un cuadro de diálogo | RF-02, RNF-04 | — | Errores |
| CP-157 | PruebasEjecucionSinDialogos | Error inesperado con un detalle interno, mostrado por `ManejadorErrores` (interfaz) | Aviso dentro de la ventana con el mensaje general y sin el detalle interno; la ventana sigue habilitada | RF-02, RNF-02 | — | Errores |
| CP-158 | PruebasVistasVisibles | Dos portadas generadas con el mismo nombre y la misma categoría, sin imagen | Las dos se dibujan idénticas, píxel a píxel, con el fondo en degradé | RF-19 | — | Portadas |
| CP-159 | PruebasVistasVisibles | Ocho vistas compuestas en un host de diseño real (8 variantes) | Ninguna guarda tipografía ni colores en el Designer, y ninguna fija un tamaño mínimo | RF-05, RNF-02 | — | Vistas |
| CP-160 | PruebasVistasVisibles | Cliente en el listado: abre la ficha de un evento desde su tarjeta, elige un sector en el mapa y pulsa comprar (interfaz) | La ficha muestra el evento; se abre la compra de ese evento y de ese cliente, con 1 entrada del sector elegido | RF-03, RF-18, RF-19 | — | Vistas |
| CP-161 | PruebasVistasVisibles | Sesión de un cliente que carga el inicio de otro cliente | Rechazo RN-41 | RF-07, RF-22 | RN-41 | Vistas |
| CP-162 | PruebasVistasVisibles | Panel del administrador cargado como Administrador y después como cliente | Como Administrador muestra cinco indicadores; como cliente, rechazo RN-34 | RF-11, RF-22 | RN-34 | Vistas |
| CP-163 | PruebasVistasVisibles | Botones de tres vistas, del estado vacío y de las pantallas de auditoría y de revisión de eventos (6 variantes) | Ningún botón corta su texto; los primarios se dibujan con el color primario en más del 60 % de su superficie | RF-05, RNF-03 | — | Vistas |
| CP-164 | PruebasEsquemaInicio | Base de prueba a la que se le quitan, uno por uno, los objetos de los scripts 04 a 16; abrir el inicio de sesión (interfaz) | El diagnóstico nombra cada vez el primer objeto y el primer script que faltan; el inicio lo muestra dentro de la ventana y deshabilita el ingreso. Con la FK del script 16 restaurada, la base rechaza un local sin dueño | RF-12, RNF-04 | — | Esquema |
| CP-165 | PruebasVistasVisibles | Sesión de un cliente: cargar el panel del administrador, pulsar «Crear administrador» y llamar al alta | Las tres se rechazan con RN-34 | RF-22 | RN-34 | Administradores |
| CP-166 | PruebasVistasVisibles | Administrador: desde su menú abre «Crear administrador» y completa el alta (interfaz) | La pantalla se titula «EventAura · Crear administrador» y avisa «Usuario creado con éxito.»; la cuenta queda con perfil Administrador y el menú sigue abierto | RF-22 | RN-05 | Administradores |
| CP-167 | PruebasSeguridadCuenta | Cliente con el correo sin verificar que arma un paquete e inicia una compra | Los dos se rechazan con RN-43 | RF-14 | RN-43 | Seguridad de cuenta |
| CP-168 | PruebasSeguridadCuenta | Alta de un cliente desde la pantalla; el código se lee del archivo de la bandeja simulada y se ingresa (interfaz) | El código queda en el archivo y no en la pantalla, y la interfaz no tiene forma de leerlo. Antes de ingresarlo el correo no está verificado; después sí, con el aviso «Ya podés iniciar sesión y comprar», y la auditoría registra la emisión del código | RF-14 | RN-43 | Seguridad de cuenta |
| CP-169 | PruebasSeguridadCuenta | Código de recuperación vencido hace un minuto | Rechazo RN-44; la contraseña no cambia y la auditoría registra «Rechazado: RN-44» | RF-15 | RN-44 | Seguridad de cuenta |
| CP-170 | PruebasSeguridadCuenta | Recuperación con el código vigente y una contraseña nueva | La contraseña se guarda con el hash del sistema: la nueva sirve y la anterior no. La auditoría registra la emisión del código y el cambio | RF-15, RF-17 | RN-02, RN-44 | Seguridad de cuenta |
| CP-171 | PruebasSeguridadCuenta | Enviar un código por SMTP sin las cinco variables de entorno `EVENTAURA_SMTP_*` | Falla con «El envío SMTP no está configurado» | RF-14, RF-15 | RN-44 | Seguridad de cuenta |
| CP-172 | PruebasSeguridadCuenta | Contraseñas «abcdefgh1» y «Cliente123», en el gestor y en las pantallas de alta y de cambio (interfaz) | Puntajes 3 y 4: la primera se rechaza con RN-47 y la segunda se acepta; el medidor de las dos pantallas muestra el mismo puntaje | RF-17 | RN-02, RN-47 | Seguridad de cuenta |
| CP-173 | PruebasSeguridadCuenta | Recuperación desde la pantalla: pedir el código, leerlo del archivo y restablecer (interfaz) | La pantalla muestra solo el aviso de la bandeja; ningún control muestra el código y no existe una pantalla de bandeja. La contraseña nueva sirve y la anterior no | RF-15 | RN-44 | Seguridad de cuenta |
| CP-174 | PruebasSeguridadCuenta | Inactividad configurada en 2 min, con aviso a 30 s; un cliente registra una acción y consulta la auditoría | Avisa a los 90 s y cierra a los 120 s, no antes. El cliente no puede consultar (RN-45); el Administrador ve la acción y el inicio de sesión del cliente | RF-16, RF-17 | RN-45, RN-46 | Auditoría |
| CP-175 | PruebasSeguridadCuenta | Administrador abre la pantalla de auditoría (interfaz) | La pantalla se muestra con la tabla cargada | RF-16 | RN-45 | Auditoría |
| CP-176 | PruebasVistasVisibles | Pantalla de recuperación de contraseña (interfaz) | El medidor, los dos campos, la casilla y el botón no se superponen, y el botón entra en la ventana | RF-15 | RN-44 | Seguridad de cuenta |
| CP-177 | PruebasGrupoAReportes | Imagen PNG de 2400 × 1600 px | Se guarda comprimida, dentro del tamaño y de las medidas máximas, y se puede volver a abrir | RF-19 | RN-52 | Portadas |
| CP-178 | PruebasGrupoAReportes | Bytes de texto presentados como imagen | Rechazo «no contiene una imagen válida» | RF-19 | RN-52 | Portadas |
| CP-179 | PruebasGrupoAReportes | Organizador guarda la portada de su evento | La base devuelve la misma imagen y la vista la muestra; sin imagen propia, vuelve a la portada generada | RF-19 | RN-49, RN-52 | Portadas |
| CP-180 | PruebasGrupoAReportes | Archivo de un byte más que el límite de carga | Rechazo «supera el límite de 10 MB» | RF-19 | RN-52 | Portadas |
| CP-181 | PruebasIntegridadEntradas | Insertar en la base entradas con `CantidadVendida` nula, 2 y 0 | La base rechaza las tres (columna obligatoria y `CK_Entradas_CantidadVendida`) y no queda ninguna fila | RF-06 | RN-08 | Integridad |
| CP-182 | PruebasAutorizacionOrganizador | Dos organizadores con un evento vendido cada uno; reporte de ventas y consultas sobre el evento y el local ajenos | El Organizador ve solo su evento y sus cifras, y las once consultas sobre lo ajeno se rechazan; el Administrador ve los dos eventos | RF-11, RF-22 | RN-34, RN-49 | Organizador |
| CP-183 | PruebasAutorizacionOrganizador | Cuentas Administrador y Organizador de la semilla; el Organizador inicia sesión (interfaz) | Son cuentas distintas: el Administrador no tiene locales y el Organizador sí. El Organizador entra a su menú con su perfil | RF-22 | RN-05 | Organizador |
| CP-184 | PruebasSolicitudesOrganizador | Cliente con el teléfono confirmado presenta una solicitud válida y vuelve a iniciar sesión | La solicitud se guarda; el perfil sigue siendo Cliente y no puede ver locales (RN-48) ni las solicitudes pendientes (RN-34) | RF-20, RF-22 | RN-50 | Solicitudes |
| CP-185 | PruebasSolicitudesOrganizador | CUIT con el dígito verificador incorrecto | Rechazo RN-52 con el mensaje del CUIT | RF-20 | RN-52 | Solicitudes |
| CP-186 | PruebasSolicitudesOrganizador | CBU con el dígito verificador incorrecto | Rechazo RN-52 con el mensaje del CBU o alias | RF-20 | RN-52 | Solicitudes |
| CP-187 | PruebasSolicitudesOrganizador | Cliente que aprueba una solicitud | Rechazo RN-34, «Esta operación es solo para el Administrador» | RF-20, RF-22 | RN-34 | Solicitudes |
| CP-188 | PruebasSolicitudesOrganizador | Administrador rechaza una solicitud con el motivo en blanco | Rechazo RN-53, «Escribí el motivo del rechazo.» | RF-20 | RN-53 | Solicitudes |
| CP-189 | PruebasSolicitudesOrganizador | Cliente con el teléfono confirmado presenta la solicitud y el Administrador la aprueba con un motivo | El cliente pasa a Organizador y tiene el local solicitado; la auditoría registra la aprobación con el administrador, la solicitud y el motivo | RF-16, RF-20, RF-22 | RN-45, RN-53 | Solicitudes |
| CP-190 | PruebasSolicitudesOrganizador | Organizador que rechaza una solicitud | Rechazo RN-34 | RF-20, RF-22 | RN-34 | Solicitudes |
| CP-191 | PruebasSolicitudesOrganizador | Solicitud sin el teléfono confirmado | Rechazo RN-51 | RF-20 | RN-51 | Solicitudes |
| CP-192 | PruebasSolicitudesOrganizador | Documentación que no es una imagen | Rechazo RN-52, «Adjuntá una imagen PNG o JPEG válida…» | RF-20 | RN-52 | Solicitudes |
| CP-193 | PruebasSolicitudesOrganizador | Menús del Cliente y del Administrador; un cliente abre la revisión de solicitudes | El Cliente tiene «Solicitar ser Organizador» y el Administrador, la entrada a las solicitudes, las dos habilitadas; la pantalla de revisión rechaza al cliente con RN-34 | RF-20, RF-22 | RN-34 | Solicitudes |
| CP-194 | PruebasSolicitudesOrganizador | Solicitud con el alias bancario «eventaura.demo» | El alias es válido y la solicitud se guarda | RF-20 | RN-52 | Solicitudes |
| CP-195 | PruebasSolicitudesOrganizador | Teléfono «0000000000» | Rechazo RN-51 | RF-20 | RN-51 | Solicitudes |
| CP-196 | PruebasSolicitudesOrganizador | Administrador rechaza una solicitud con un motivo | Queda Rechazada con el motivo, el revisor y la fecha; la auditoría registra el rechazo con el motivo | RF-16, RF-20 | RN-45, RN-53 | Solicitudes |
| CP-197 | PruebasEsquemaInicio | Comparar todos los scripts numerados desde 04 con el registro de requisitos del diagnóstico de arranque | Cada migración tiene comprobación registrada; un script nuevo sin alta en el verificador hace fallar el caso | RF-12, RNF-04 | — | Esquema |
| CP-198 | PruebasPublicacionEventos | Rechazar el evento propio o ajeno como Organizador; rechazar como Administrador sin motivo | El Organizador no puede revisar; al Administrador se le exige motivo y el evento permanece EnRevision | RF-21, RF-22 | RN-48, RN-54 | Revisión de eventos |
| CP-199 | PruebasPublicacionEventos | Administrador rechaza el primer evento; Organizador ve el motivo e intenta vender y vuelve a publicar | El evento vuelve a Borrador, conserva y muestra el motivo, no aparece en catálogo ni acepta ventas; al volver a publicar entra otra vez en revisión | RF-03, RF-21 | RN-09, RN-54 | Revisión de eventos |
| CP-200 | PruebasSeguridadCuenta | Pedir la recuperación desde la pantalla para un correo existente y para uno inexistente (interfaz) | La respuesta es el mismo texto; las dos tardan al menos 950 ms y difieren en menos de 400 ms; ningún código se ve en la pantalla. El código del correo inexistente se rechaza con RN-44 | RF-15 | RN-44 | Seguridad de cuenta |
| CP-201 | PruebasSeguridadCuenta | Pedir dos códigos de recuperación seguidos; usar el primero, el segundo y otra vez el segundo | El primero ya no sirve; el segundo restablece una vez y, al reutilizarlo, se rechaza | RF-15 | RN-44 | Seguridad de cuenta |
| CP-202 | PruebasSolicitudesOrganizador | Pedir el código del teléfono desde la pantalla de la solicitud (interfaz) | El código queda en el archivo de la bandeja simulada y no en la pantalla, que muestra el aviso uniforme; la auditoría registra la emisión | RF-20 | RN-44, RN-51 | Solicitudes |
| CP-203 | PruebasSeguridadCuenta | Alta de un correo nuevo y, después, otra alta con el mismo correo y otra contraseña (interfaz) | Las dos muestran la verificación del correo sin el código y tardan lo mismo (al menos 950 ms, con menos de 400 ms de diferencia); la contraseña de la cuenta existente no cambia | RF-14 | RN-01, RN-43 | Seguridad de cuenta |
| CP-204 | PruebasPublicacionEventos | Organizador nuevo publica su primer evento, primero sin sectores; un cliente intenta comprar; el Administrador lo aprueba; después publica un segundo | Sin sectores no se publica. Con un sector pasa a revisión, no aparece en el catálogo y la venta se rechaza (RN-09); el Organizador no puede aprobarlo. Aprobado, se publica, y el segundo se publica directo | RF-21 | RN-54 | Revisión de eventos |
| CP-205 | PruebasPublicacionEventos | Publicar un evento sin sectores | Rechazo RN-06; el evento sigue en Borrador | RF-21 | RN-06 | Revisión de eventos |
| CP-206 | PruebasVistasVisibles | Métodos de escritura de la auditoría y de revisión de solicitudes en `Datos`, y el código fuente de `Visual` | `Registrar`, `Aprobar` y `Rechazar` son `internal` y solo `Repositorio` los ve; ningún archivo de `Visual` nombra esos repositorios ni escribe en la auditoría | RF-16 | RN-45 | Cierre de la auditoría |
| CP-207 | PruebasSeguridadCuenta | El archivo del correo simulado que lleva un código | Es un mensaje MIME legible, con el código en el cuerpo | RF-14, RF-15 | RN-43, RN-44 | Correo y SMS |
| CP-208 | PruebasSeguridadCuenta | Las cinco variables de entorno de SMTP, completas o con alguna faltante | El emisor SMTP se elige solo cuando están las cinco | RF-14, RF-15 | RN-43, RN-44 | Correo y SMS |
| CP-209 | PruebasArranqueYAlcance | El ejecutable real, `Visual.exe` | Completa el arranque y la ventana permanece abierta | RF-01, RNF-03 | — | Arranque y navegación |
| CP-210 | PruebasArranqueYAlcance | Las referencias de navegación entre los formularios | Cada pantalla tiene una ruta de entrada y una salida visible | RF-01, RF-22, RNF-03 | — | Arranque y navegación |
| CP-211 | PruebasVistasVisibles | Usuario con sesión iniciada, menú «Mi cuenta» (interfaz) | Abre el cambio de contraseña, que pide la contraseña actual | RF-17, RF-22 | RN-02 | Cambio de contraseña |
| CP-354 | PruebasFormularios | Las ventanas sin diseño adaptable (interfaz) | Enter, Esc, el orden de tabulación y el foco inicial responden como se espera, y la ventana no cambia de tamaño | RF-05, RNF-02 | — | Teclado |
| CP-355 | PruebasSolicitudesOrganizador | Solicitud para ser Organizador con SMTP configurado | El código del teléfono llega por el SMS simulado y la solicitud se completa | RF-20 | RN-51 | Correo y SMS |
| CP-356 | PruebasSolicitudesOrganizador | El envío de un código con un proveedor que falla | No queda un código activo en la base | RF-14, RF-15 | RN-43, RN-44 | Correo y SMS |
| CP-357 | PruebasInteraccionPantallas | Las 26 ventanas reales, abiertas y operadas (interfaz) | El foco, el teclado y la geometría responden en cada una | RNF-02, RNF-03 | — | Teclado y escala |
| CP-358 | PruebasEsquemaInicio | Falla de conexión al iniciar (interfaz) | Avisa dentro de la ventana y mantiene oculta la contraseña | RF-02, RNF-02 | — | Arranque |
| CP-359 | PruebasInteraccionPantallas | Quitar físicamente las dependencias WebView2 y Newtonsoft en una copia | Las vistas afectadas muestran su respaldo WinForms y un aviso | RF-03, RF-07, RF-11, RNF-02 | — | Respaldo web |
| CP-360 | PruebasInteraccionPantallas | Sin iText, exportar un reporte; además, fallo al crear el ticket tras el pago | La exportación avisa que falta un archivo y la compra pagada conserva la opción de recuperar el ticket | RF-07, RNF-02 | — | PDF y compra |
| CP-364 | PruebasInteraccionPantallas | Alta web completa con verificación de correo y navegación final (interfaz) | La cuenta queda creada y la acción final abre Acceso sin reiniciar el alta | RF-14 | RN-43 | Alta de cuenta |
| CP-365 | PruebasInteraccionPantallas | Compra con dos entradas en el historial web (interfaz) | La compra y ambas opciones de devolución caben en la vista | RF-05 | RN-29 | Historial |

**Notas de lectura:**
- **CP-112 no verifica RNF-01** (revisado el 19/11). RNF-01 pide que el flujo de compra se
  resuelva en menos de cinco minutos; CP-112 comprueba que siete componentes no guarden su estilo
  en el Designer, que es RNF-02. Se quitó RNF-01 del catálogo, como ya lo había quitado Codex del
  comentario, y RF-05 se agregó como en el comentario. RNF-01 no tiene caso automático: lo verifica
  la medición del informe de usabilidad (`docs/usabilidad/INFORME-USABILIDAD.md`, «Tiempo del
  flujo completo»).
- **CP-149 no existe**, ni CP-212 a CP-353 ni CP-361 a CP-363. Los números se saltaron al numerar y no se reasignan, igual que RN-27.
- **CP-207 a CP-211 y CP-354 a CP-357 (24/11, `2db272e`); CP-358 a CP-360 y CP-364 a CP-365 (26/11).** Requerimientos y reglas tomados de sus comentarios con
  `tools/Matriz/Extraer.py`. No suman reglas nuevas: la cobertura sigue en 50 de 52.
- **Requerimientos y reglas alineados con los comentarios (22/11).** Codex volvió a leer las
  pruebas y corrigió los comentarios en `42b255e` y `e3e000d`. Desde `e3e000d`, la columna RF del
  catálogo y la de los comentarios coinciden en todos los casos. En la columna Reglas, el catálogo
  sigue a los comentarios en CP-93, CP-99, CP-110, CP-123 y CP-125, leyendo cada prueba:
  - CP-93 y CP-99 esperan el rechazo con RN-48 (el perfil Organizador), no con RN-34;
  - CP-110 restringe los estados de la entrada y no toca la devolución: sale RN-29;
  - CP-123 rechaza el evento ajeno con RN-49 y la consulta sin sesión con RN-48, que es el control
    de perfil de `GestorIngresos`: RN-48 reemplaza a RN-41;
  - CP-125 verifica la ventana de prioridad de RN-35: sale RN-37 (decisión de JC, 22/11).

  Quedan cuatro filas en las que el catálogo nombra una regla más, o una distinta, que el
  comentario, a confirmar con JC: CP-44 (el comentario cita RN-05; el catálogo deja la columna
  vacía porque es una regresión de la interfaz que muestra la regla sin verificarla), CP-95 (el catálogo suma
  RN-40) y CP-101 y CP-133 (el catálogo suma RN-29). La cobertura de reglas no cambia: 50 de 52.
- **CP-165, CP-187, CP-190 y CP-193 verifican RN-34.** Citaban RN-05 y RN-53 pero esperaban el
  rechazo con RN-34: el control de perfil Administrador es el mismo para toda la administración.
  Codex corrigió los comentarios en `e2d8ff6` (20/11) y el catálogo los sigue. La cobertura no
  cambia: RN-05 y RN-53 tienen otros casos (CP-183, CP-188, CP-189 y CP-196).
- **CP-184 a CP-196 muestran su ID en el nombre visible** desde `e2d8ff6` (20/11), como pide el
  punto 7 de la definición de terminado.
- **CP-105 y CP-109 son mediciones:** no verifican una defensa. CP-105 sí afirma que un hash más
  fuerte no se degrada.
- **CP-77 cubre RN-25, no RN-26:** solo lee el texto «No se renueva automáticamente». Por eso
  RN-26 sigue sin caso (criterio D-5 de la auditoría).
- **CP-79 y CP-80 tocan el seguro,** pero no prueban que no se pueda agregar después de la compra.
  Por eso no llevan RN-28, aunque su comentario la nombre, y RN-28 sigue sin caso.
- **Criterio D-5, resuelto por JC el 14/11: no se cuentan.** Codex escribió dos pruebas reales,
  CP-121 para RN-26 y CP-122 para RN-28, verdes el 16/11. **La cobertura pasa a 39 de 41 (95,1 %).**
  Codex también quitó RN-26 y RN-28 de los comentarios de CP-77, CP-79 y CP-80.
- **CP-126 a CP-145 son las pruebas que encargó la mutación** (`docs/ENCARGO-PRUEBAS-MUTACION.md`,
  tareas 1 a 21; la 12 refuerza CP-94 y no tiene caso nuevo). Codex informa que cada una falló con
  su defensa quitada y volvió a pasar al restaurarla. Esa comprobación es de Codex, una por una; la
  cifra que va al documento es la de la corrida final de mutación sobre el código congelado.
- **El RF de CP-126 a CP-145 es el de la matriz del documento** (`docs/01-requerimientos.md`). Los comentarios de CP-127, CP-129, CP-133, CP-138, CP-139, CP-140, CP-143 y CP-144 decían otro; Codex los corrigió en el cierre, sin cambiar sus aserciones.
- **CP-146 a CP-148 salen del cierre de Codex** (`Pruebas/RESULTADOS-CIERRE-DEFINITIVO-2026-11-16.md`). CP-146 cierra M-22: dio rojo con el código anterior, verde con el arreglo, rojo al quitar el control y verde al restaurarlo. CP-147 verifica el índice único de seguros del script `07-indice-unico-seguros.sql`. CP-148 es la regresión de U-09; no verifica una regla. **La cobertura no cambia: 39 de 41.** RN-34 y RN-28 ya tenían caso.
- **CP-99 cambió en el cierre:** el cliente que intenta registrar recibía cualquier excepción y la prueba la aceptaba; ahora exige el resultado con RN-34. Por eso suma RN-34. Su llamada sin sesión es a `Contar`, no a `Registrar`.
- **CP-125 prueba la ventana de prioridad de RN-35**, construida en `1525202`: columna
  `Sectores.CupoLiberadoEl` y parámetro `PRIORIDAD_ESPERA_MINUTOS` (15). RN-35 ya tenía caso
  (CP-117); la cobertura no cambia: 39 de 41.
- **Las reglas de CP-91 a CP-104** son las que declara el comentario de la prueba. Si cada una
  resiste la mutación de su defensa, lo dice `docs/PLAN-MUTACION-SEGURIDAD.md` cuando se
  ejecute.

## Hallazgos de usabilidad a contemplar en la UI

| # | Hallazgo | Acción | Regla |
|---|---|---|---|
| H-01 | No se entiende por qué el servicio no recibe descuento | Leyenda junto al total: «El descuento se aplica sobre el valor de las entradas» | RN-21 |
| H-02 | La vigencia de la reserva no está a la vista | Temporizador fijo en el encabezado desde la pantalla de pago | RN-15 |
| H-03 | El selector no informa el cupo restante | Mostrar «Quedan N entradas» junto a cada sector | RN-08 |
| H-04 | La advertencia del seguro pasa inadvertida | Confirmación explícita al avanzar sin contratarlo | RN-28 |
| H-05 | El indicador de nivel no se comprende | Enlace «¿Cómo funcionan los niveles?» | RN-19 |
| H-06 | «Volver» se interpreta como cancelar todo | Renombrar a «Volver al listado» y pedir confirmación | — |
