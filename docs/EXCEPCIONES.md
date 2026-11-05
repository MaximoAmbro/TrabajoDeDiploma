# Manejo de excepciones en EventAura

Resumen para la defensa, al 05/11/2026. La primera parte explica el diseño, la segunda cómo
se probó. Todo lo que se afirma acá se leyó en el código, y lo marcado como **verificado**
se ejecutó sobre la aplicación real.

---

## Parte A. El diseño

### Los tipos de excepción y quién lanza cada uno

| Excepción | Quién la lanza | Cuándo | Cuántas veces aparece |
|---|---|---|---|
| **`ReglaNegocioException`** (`Repo/ReglaNegocioException.cs`) | Los gestores de Negocio | Una regla de negocio rechaza la operación. Lleva dos datos: `Regla`, el código («RN-12»), y `Message`, el texto para el usuario | 23 lanzamientos, sobre 19 reglas: RN-01, 02, 04, 05, 06, 08, 09, 10, 11, 12, 13, 14, 15, 25, 29, 34, 36, 38 y 39 |
| `ArgumentException` / `ArgumentNullException` | Gestores y repositorios | Un dato mal formado: tarjeta, período invertido, cantidad negativa, argumento nulo | 23 y 21 |
| `InvalidOperationException` | Gestores | Un estado que no debería darse: «no se encontró el evento», «la compra ya no está pendiente», «falta un parámetro» | 18 |
| `RegistroDuplicadoException` (`Datos/RepositorioBase.cs`) | Datos | SQL Server rechaza un registro por unicidad (errores 2627 y 2601) | 3 |
| `CupoInsuficienteException` (`Datos/RepositorioSectores.cs`) | Datos | El cupo de un sector no alcanza al reservar | 2 |
| `ConfigurationErrorsException` | Datos (`Conexion`) | Falta la cadena de conexión en `App.config` | 1 |
| `SqlException` | El proveedor de SQL Server | La base no responde, no existe, o rechaza una consulta | No se lanza en el código: la produce el motor |

### Cómo se reparte el trabajo entre las capas

Respeta las reglas de la arquitectura (CLAUDE.md):

1. **Datos no decide nada de negocio.** Ante un error de SQL en una transacción, hace
   `Rollback` y vuelve a lanzar la excepción. Los 12 bloques `catch` de `Datos/` son todos
   `catch { transaccion.Rollback(); throw; }`. Solo traduce dos casos a excepciones propias:
   unicidad (`RegistroDuplicadoException`) y falta de cupo (`CupoInsuficienteException`).
2. **Negocio traduce a reglas.** Atrapa esas dos excepciones de Datos y las convierte en la
   regla que corresponde. Por ejemplo, `RegistroDuplicadoException` → `RN-01 «Ya existe una
   cuenta registrada con ese mail»`, y `CupoInsuficienteException` → `RN-08`. Negocio nunca
   muestra nada: no referencia `System.Windows.Forms`.
3. **Visual muestra el mensaje.** Cada formulario envuelve las llamadas a Negocio en
   `try/catch`:
   - `ReglaNegocioException`, 19 capturas: `MessageBox.Show(ex.Message, ex.Regla, …)`. El
     texto es el de la regla y el título, su código;
   - `ArgumentException`, 5 capturas, y `InvalidOperationException`, 4 capturas más 3
     combinadas, en 5 pantallas (historial, suscripción, administrar evento, agregar sector,
     eventos del local) y en el pago: el mensaje va a un `MessageBox` o a una etiqueta de la
     pantalla. En el pago, por ejemplo, a la etiqueta roja `lblEstado`;
   - `IOException`, 1 captura: al exportar un reporte a PDF.
4. **Manejador global: `Visual/ManejadorErrores.cs`** (desde T-27). `Program.Main` lo instala
   antes de abrir la primera pantalla. Registra `Application.ThreadException` y
   `AppDomain.CurrentDomain.UnhandledException`, y lo que ninguna pantalla captura llega al
   usuario como un aviso «EventAura» en castellano, sin detalles técnicos:
   - **error de base de datos** (cualquier `DbException`, como `SqlException`): «No se pudo
     conectar con la base de datos. Verifique que SQL Server esté en funcionamiento y vuelva
     a intentar.»;
   - **error lanzado por Negocio** (`Source == "Repositorio"`): su propio mensaje, que ya está
     en castellano y dice qué pasó;
   - **cualquier otro error:** «Ocurrió un error inesperado y la operación no se completó. Si
     se repite, avise al administrador.»

   La aplicación sigue abierta después del aviso.
5. **No hay bitácora.** Ningún error queda registrado en la base ni en un archivo. Es el anexo
   técnico T06 de la Carpeta Modelo (ver `ANEXOS-TECNICOS-T01-T08.md`). El manejador global
   es el lugar natural para agregarla.

### El camino de una excepción, desde que se lanza hasta que el usuario ve algo

```
                    ┌─────────────── SQL Server ───────────────┐
                    │ error de unicidad · falta de cupo · otro │
                    └────────────────────┬─────────────────────┘
                                         ▼
 DATOS     catch { Rollback(); throw; }  ──►  traduce unicidad y cupo a
           (las transacciones no quedan       RegistroDuplicadoException /
            a medias)                         CupoInsuficienteException
                                         │                        │
                                         │ cualquier otro error   │
                                         │ (SqlException)         ▼
                                         │      NEGOCIO  traduce a ReglaNegocioException
                                         │               («RN-01», «RN-08»)
                                         │               y además lanza las suyas:
                                         │               ReglaNegocioException (19 reglas)
                                         │               ArgumentException
                                         │               InvalidOperationException
                                         ▼                        ▼
 VISUAL   try { gestor... }
          catch (ReglaNegocioException ex) ──► MessageBox(texto de la regla, título «RN-xx»)
          catch (ArgumentException ex)     ──► etiqueta o MessageBox con el mensaje
          catch (InvalidOperationException) ─► en 5 pantallas y en el pago
                                         │
                 lo que ninguna pantalla captura (SqlException casi siempre;
                 InvalidOperationException en las pantallas que no la atrapan)
                                         ▼
 MANEJADOR GLOBAL  ManejadorErrores ──► aviso «EventAura» en castellano:
 (desde T-27)                           base de datos · mensaje de Negocio · general
                                        La aplicación sigue abierta.
```

**Qué ve el usuario en cada caso:**

| Origen | Lo que aparece | Ejemplo real |
|---|---|---|
| Regla de negocio | Un aviso con el texto de la regla, y su código como título | Título «RN-12»: «Se pueden comprar como máximo 6 entradas por operación.» |
| Dato mal formado en el pago | Texto en rojo dentro de la pantalla, sin ventana aparte | «El vencimiento debe tener el formato MM/AA.» |
| Error no previsto | Un aviso «EventAura» en castellano, sin detalle técnico | Ver las verificaciones de abajo |

**Lo bueno del diseño:**
- Las reglas viajan con su código, así que el mismo identificador une el documento, el
  código, la prueba y el mensaje.
- Las transacciones nunca quedan a medias.
- Negocio no conoce la interfaz.
- Ningún error llega al usuario como el cuadro técnico de .NET.

**Lo débil del diseño:**
- **No hay bitácora**: el manejador avisa al usuario, pero no deja registro del error.
- **El título de los avisos es el código interno** («RN-12»), no un texto para el usuario
  (hallazgo de usabilidad U-03).

### Verificado: la base caída, antes y después del manejador

**Antes (04/11/2026).** `tools/Excepciones/VerificarErrorNoControlado.ps1` corrió la
aplicación con su bucle de mensajes contra una base que no existe e intentó iniciar sesión
(`docs/excepciones/error-no-controlado.txt`):

> Ventana: `System.Windows.Forms.ThreadExceptionDialog`, título «Microsoft .NET Framework».
> Texto: «Excepción no controlada en la aplicación. Si hace clic en Continuar, la aplicación
> omitirá este error e intentará continuar. Si hace clic en Salir, la aplicación se cerrará
> inmediatamente. Cannot open database "BddEventAura_NoExiste"…». Botones: Detalles,
> Continuar, Salir.

Ese programa queda como registro del «antes»: no instala el manejador.

**Después (05/11/2026, CX-04).** `tools/Excepciones/VerificarBaseCaida.ps1` **detiene el
servicio de SQL Server**, abre el ejecutable real (`Visual\bin\Debug\Visual.exe`), intenta
iniciar sesión con UI Automation y vuelve a levantar el servicio
(`docs/excepciones/base-caida.txt` y `base-caida.png`):

> La aplicación abre sin la base. A los 15 s del clic en «Iniciar sesión» aparece un aviso:
> título «EventAura», texto «No se pudo conectar con la base de datos. Verifique que SQL
> Server esté en funcionamiento y vuelva a intentar.», botón «Aceptar». Cuadro de .NET: no.
> Después de «Aceptar», la aplicación sigue en el inicio de sesión, y al cerrarla con la X
> el proceso termina. El servicio vuelve a quedar en ejecución.

Los 15 segundos son el tiempo de espera de la conexión de SQL Server, no de la aplicación.

### Verificado: el evento eliminado mientras el cliente paga

`tools/Excepciones/VerificarEventoEliminado.ps1` abre los formularios reales contra
`BddEventAura_Pruebas`, con el manejador instalado. Arma el paquete del ejemplo del documento
y, con la pantalla de pago abierta, **otro proceso** llama a `GestorEventos.Eliminar`, lo
mismo que hace el botón «Eliminar» del propietario. Después el cliente pulsa «Pagar»
(`docs/excepciones/evento-eliminado-en-pago.txt` y `.png`):

> Aviso «Pago no realizado»: «La compra ya no está pendiente de pago. Vuelva a armar la
> compra desde el listado.». Cuadro de .NET: no. La pantalla de pago devuelve Abort y la
> aplicación vuelve al listado de eventos. En la base: la compra quedó Vencida, sin pagos y
> sin entradas, y el evento ya no existe.

**Observación:** el aviso es correcto pero genérico. No dice que el evento se eliminó, porque
la compra no sabe por qué dejó de estar pendiente. Es aceptable. Si se quiere más claro, es
un cambio de texto en `frmPago`.

El evento solo se puede eliminar si no tiene entradas vendidas (RN-11). Si las tiene, el
propietario solo puede suspenderlo, y la compra en curso sigue su curso normal.

---

## Parte B. Cómo se probaron los casos de excepción

### Reglas de negocio con el camino de error probado: las 19

Las 19 reglas que lanzan `ReglaNegocioException` tienen una prueba automática que provoca el
rechazo y comprueba la regla. Las siete de abajo se agregaron el 05/11/2026 (CP-55 a CP-61);
además, comprueban que el sistema queda como estaba.

| Regla | Qué rechaza | Caso de prueba |
|---|---|---|
| RN-01 | Mail duplicado | CP-26 |
| RN-02 | Contraseña corta | CP-15 (por la interfaz, con el aviso) |
| RN-04 | Intentos fallidos y bloqueo | CP-28 |
| RN-05 | Perfil de usuario inexistente | **CP-55**: no se crea la cuenta |
| RN-06 | Eliminar el último sector de un evento publicado | **CP-56**: el sector y el estado no cambian |
| RN-08 | Más entradas que el cupo del sector | CP-31 |
| RN-09 | Evento pasado o no publicado | CP-30 |
| RN-10 | Lista de espera con el evento no agotado | CP-24 (con el aviso «RN-10») y CP-37 |
| RN-11 | Eliminar un evento con ventas | CP-21 (con el aviso y la oferta de suspensión) |
| RN-12 | Más de 6 entradas por operación | CP-31 |
| RN-13 | Paquete sin entradas | CP-31 |
| RN-14 | Agregar un servicio que el evento no ofrece | **CP-57**: el paquete no lo suma |
| RN-15 | Pagar una reserva vencida | CP-34 y CP-43 |
| RN-25 | Una segunda suscripción activa | **CP-58**: sigue la suscripción original |
| RN-29 | Devolución sin seguro o fuera de plazo | **CP-59**: las dos compras siguen confirmadas, sin reembolso |
| RN-34 | Reportes sin ser administrador | CP-35 |
| RN-36 | Inscribirse dos veces en la lista de espera | CP-37 |
| RN-38 | Sectores que superan la capacidad del local | **CP-60**: el sector no se agrega |
| RN-39 | Comprar en preventa sin suscripción | **CP-61**: el cupo no cambia |

**Alcance:** cada caso prueba el rechazo documentado, no todas sus variantes. CP-56 prueba
eliminar el último sector, no publicar un evento sin sectores. CP-60 prueba agregar un
sector, no modificarlo. RN-05 se prueba con el método genérico `Registrar`: desde T-22 el alta
pública ya no permite elegir el perfil.

### Otros errores: probados y no probados

| Error | Estado |
|---|---|
| Período de reporte invertido, código de reporte inexistente (`ArgumentException`) | Probado: CP-38 |
| Cantidad «abc» o «-1» en la grilla | Probado por la interfaz: CP-20 |
| Datos de registro vacíos | Probado por la interfaz: CP-15 |
| Pago rechazado por la pasarela | Probado: CP-33. No es una excepción: `Confirmar` devuelve `false` y el formulario lo muestra |
| Mensaje del manejador global para un error de base, uno de Negocio y uno general | Probado: CP-51. Que `Program.Main` lo instala: CP-53 |
| Evento eliminado durante el pago | Probado: CP-52. **Verificado** además sobre los formularios reales, con otro proceso |
| Base de datos caída | **Verificado** sobre el ejecutable real con el servicio detenido (CX-04). No tiene prueba automática: detener SQL Server en medio de la batería rompería las demás pruebas |
| **Validaciones de la tarjeta** (titular vacío, número corto, vencimiento sin barra o vencido, código de seguridad corto) | **Sin prueba automática.** Se midieron sobre la aplicación en el estudio de usabilidad (sondeo S15): cada una muestra su mensaje correcto |

### Errores que antes llegaban al usuario como cuadro de .NET

| Situación | Excepción | Estado |
|---|---|---|
| SQL Server detenido o base inexistente, en cualquier pantalla | `SqlException` | **Resuelto por T-27.** Aviso de base de datos. Verificado (CX-04) |
| Un propietario elimina el evento mientras un cliente está pagando | `InvalidOperationException` («La compra ya no está pendiente de pago.») | **Resuelto por T-27.** `frmPago` la captura: aviso «Pago no realizado» y vuelta al listado. CP-52 y verificado |
| El pago se aprueba pero la reserva ya no estaba vigente | `InvalidOperationException` («El pago se aprobó pero la reserva ya no estaba vigente.») | **Resuelto por T-27**, con la misma captura de `frmPago`. Muy improbable desde T-20 |
| El evento se elimina entre el listado y la apertura del armado | `InvalidOperationException` («No se encontró ningún evento con ese ID.») | **Resuelto por T-27.** Lo atrapa el manejador global y muestra el mensaje de Negocio |
| ~~Cambiar `RESERVA_MINUTOS` con una compra en curso~~ | ~~`InvalidOperationException` en el pago~~ | **Resuelto por T-20**: el plazo sale de la reserva registrada y CP-43 lo prueba |

---

## Para la defensa

> «Las reglas de negocio se lanzan como una excepción propia, `ReglaNegocioException`, con el
> código de la regla y un mensaje para el usuario. Datos solo deshace la transacción y
> traduce dos casos, duplicado y falta de cupo, que Negocio convierte en reglas. Cada
> pantalla captura esas excepciones y muestra el mensaje. Las diecinueve reglas que rechazan
> operaciones tienen una prueba automática que provoca el rechazo y comprueba que nada quedó
> a medias. Lo que no captura ninguna pantalla llega a un manejador global, que muestra un
> mensaje en castellano. Lo comprobamos deteniendo SQL Server con la aplicación abierta, y
> eliminando un evento desde otro proceso mientras un cliente pagaba. Lo que falta es la
> bitácora, y el manejador global es el lugar donde iría.»
