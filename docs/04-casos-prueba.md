# Casos de prueba

Proyecto `Pruebas/` con MSTest. Hay 61 casos: CP-01 a CP-10 son los de la lógica de
fidelización, que define el documento; CP-11 a CP-61 se agregaron en noviembre de 2026
(ver «Casos CP-11 a CP-61» más abajo). CP-01 a CP-04 se resuelven con `[DataTestMethod]`.
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
| CP-12 | PruebasFormularios | Los 19 formularios de `Visual` | Ninguno declara más de 790 px de alto | RNF-03 | — | — |
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
| CP-44 | PruebasInterfaz | Abrir el alta de usuario y registrarse con datos válidos | No hay selector de perfil; el usuario creado es Cliente | RF-01 | — | — |
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
| CP-55 | PruebasRechazosNegocio | `Registrar` con un perfil inexistente | RN-05; no se crea la cuenta | RF-01, RF-02 | RN-05 | — |
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

## Hallazgos de usabilidad a contemplar en la UI

| # | Hallazgo | Acción | Regla |
|---|---|---|---|
| H-01 | No se entiende por qué el servicio no recibe descuento | Leyenda junto al total: «El descuento se aplica sobre el valor de las entradas» | RN-21 |
| H-02 | La vigencia de la reserva no está a la vista | Temporizador fijo en el encabezado desde la pantalla de pago | RN-15 |
| H-03 | El selector no informa el cupo restante | Mostrar «Quedan N entradas» junto a cada sector | RN-08 |
| H-04 | La advertencia del seguro pasa inadvertida | Confirmación explícita al avanzar sin contratarlo | RN-28 |
| H-05 | El indicador de nivel no se comprende | Enlace «¿Cómo funcionan los niveles?» | RN-19 |
| H-06 | «Volver» se interpreta como cancelar todo | Renombrar a «Volver al listado» y pedir confirmación | — |
