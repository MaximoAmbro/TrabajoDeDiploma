# Manejo de excepciones en EventAura

Resumen para la defensa, al 04/11/2026. La primera parte explica el diseño, la segunda cómo
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
   - `ArgumentException`, 5 capturas, y `InvalidOperationException`, 3 capturas más 3
     combinadas, en 5 pantallas (historial, suscripción, administrar evento, agregar sector y
     eventos del local): el mensaje va a un `MessageBox` o a una etiqueta de la pantalla. En el pago,
     por ejemplo, a la etiqueta roja `lblEstado`;
   - `IOException`, 1 captura: al exportar un reporte a PDF.
4. **No hay manejador global.** `Visual/Program.cs` no registra `Application.ThreadException`
   ni `AppDomain.UnhandledException`. Lo que ningún formulario captura va al manejador por
   defecto de .NET.
5. **No hay bitácora.** Ningún error queda registrado en la base ni en un archivo. Es el anexo
   técnico T06 de la Carpeta Modelo (ver `ANEXOS-TECNICOS-T01-T08.md`).

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
          catch (InvalidOperationException) ─► solo en 5 pantallas
                                         │
                 lo que ninguna pantalla captura (SqlException casi siempre;
                 InvalidOperationException en las pantallas que no la atrapan)
                                         ▼
 .NET     sin manejador propio ──► ThreadExceptionDialog: «Microsoft .NET Framework»,
                                   «Excepción no controlada en la aplicación…»,
                                   [Detalles] [Continuar] [Salir]
```

**Qué ve el usuario en cada caso:**

| Origen | Lo que aparece | Ejemplo real |
|---|---|---|
| Regla de negocio | Un aviso con el texto de la regla, y su código como título | Título «RN-12»: «Se pueden comprar como máximo 6 entradas por operación.» |
| Dato mal formado en el pago | Texto en rojo dentro de la pantalla, sin ventana aparte | «El vencimiento debe tener el formato MM/AA.» |
| Error no previsto | El cuadro genérico de .NET, con el mensaje técnico en inglés | Ver la verificación de abajo |

**Lo bueno del diseño:**
- Las reglas viajan con su código, así que el mismo identificador une el documento, el
  código, la prueba y el mensaje.
- Las transacciones nunca quedan a medias.
- Negocio no conoce la interfaz.

**Lo débil del diseño:**
- **No hay manejador global**, así que los errores no previstos llegan sin traducir.
- **No hay bitácora.**
- **El título de los avisos es el código interno** («RN-12»), no un texto para el usuario
  (hallazgo de usabilidad U-03).

### Verificado: qué ve el usuario ante un error no controlado

`tools/Excepciones/VerificarErrorNoControlado.ps1` corre la aplicación real con su bucle de
mensajes, igual que `Program.cs`, contra una base que no existe, e intenta iniciar sesión.
Resultado (`docs/excepciones/error-no-controlado.txt`):

> Ventana: `System.Windows.Forms.ThreadExceptionDialog`, título «Microsoft .NET Framework».
> Texto: «Excepción no controlada en la aplicación. Si hace clic en Continuar, la aplicación
> omitirá este error e intentará continuar. Si hace clic en Salir, la aplicación se cerrará
> inmediatamente. Cannot open database "BddEventAura_NoExiste"…». Botones: Detalles,
> Continuar, Salir.

Es lo que vería un usuario si SQL Server está detenido, o si la base no existe en la máquina
donde se instala.

---

## Parte B. Cómo se probaron los casos de excepción

### Reglas de negocio con el camino de error probado

De las 19 reglas que lanzan `ReglaNegocioException`, **12 tienen una prueba automática que
provoca el error y comprueba la regla**:

| Regla | Qué rechaza | Caso de prueba |
|---|---|---|
| RN-01 | Mail duplicado | CP-26 |
| RN-02 | Contraseña corta | CP-15 (por la interfaz, con el aviso) |
| RN-04 | Intentos fallidos y bloqueo | CP-28 |
| RN-08 | Más entradas que el cupo del sector | CP-31 |
| RN-09 | Evento pasado o no publicado | CP-30 |
| RN-10 | Lista de espera con el evento no agotado | CP-24 (con el aviso «RN-10») y CP-37 |
| RN-11 | Eliminar un evento con ventas | CP-21 (con el aviso y la oferta de suspensión) |
| RN-12 | Más de 6 entradas por operación | CP-31 |
| RN-13 | Paquete sin entradas | CP-31 |
| RN-15 | Pagar una reserva vencida | CP-34 y CP-43 |
| RN-34 | Reportes sin ser administrador | CP-35 |
| RN-36 | Inscribirse dos veces en la lista de espera | CP-37 |

**Sin prueba del camino de error: 7 reglas.**

| Regla | Qué rechaza | Por qué falta |
|---|---|---|
| RN-05 | Tipo de usuario inválido | Con T-22 el alta pública ya no elige el tipo; queda para T04 |
| RN-06 | Publicar sin sectores, o borrar el último sector | No entró en ninguna tanda |
| RN-14 | Agregar un servicio que el evento no ofrece | CP-24 prueba que la casilla queda deshabilitada, pero no la excepción del gestor |
| RN-25 | Dos suscripciones activas | No entró en ninguna tanda |
| RN-29 | Devolución sin seguro o fuera de plazo | Se prueba la devolución correcta (CP-09, CP-24), no su rechazo |
| RN-38 | Sectores que superan la capacidad del local | Regla incorporada al documento en la versión 2.5 |
| RN-39 | Comprar en preventa sin suscripción | Ídem |

### Otros errores: probados y no probados

| Error | Estado |
|---|---|
| Período de reporte invertido, código de reporte inexistente (`ArgumentException`) | Probado: CP-38 |
| Cantidad «abc» o «-1» en la grilla | Probado por la interfaz: CP-20 |
| Datos de registro vacíos | Probado por la interfaz: CP-15 |
| Pago rechazado por la pasarela | Probado: CP-33. No es una excepción: `Confirmar` devuelve `false` y el formulario lo muestra |
| **Validaciones de la tarjeta** (titular vacío, número corto, vencimiento sin barra o vencido, código de seguridad corto) | **Sin prueba automática.** Se midieron sobre la aplicación en el estudio de usabilidad (sondeo S15): cada una muestra su mensaje correcto |
| **Errores de base de datos** (`SqlException`) | **Sin prueba automática.** Verificado a mano con `VerificarErrorNoControlado.ps1`: llegan al cuadro genérico de .NET |

### Errores no controlados que pueden llegar al usuario como cuadro de Windows

| Situación | Excepción | Dónde se escapa | Estado |
|---|---|---|---|
| **SQL Server detenido o base inexistente**, en cualquier pantalla | `SqlException` | Ninguna pantalla la captura | **Verificado** al iniciar sesión |
| **Un propietario elimina el evento mientras un cliente está pagando** | `InvalidOperationException` («La compra ya no está pendiente de pago.») | `frmPago.btnPagar_Click` captura `ArgumentException` y `ReglaNegocioException`, pero no `InvalidOperationException`. Al eliminar el evento, la base pasa a Vencida sus compras abiertas | Leído en el código. **Encargado: T-27, con su prueba CP-52**, que lo reproduce |
| El pago se aprueba pero la reserva ya no estaba vigente | `InvalidOperationException` («El pago se aprobó pero la reserva ya no estaba vigente.») | Ídem | Leído en el código. Muy improbable desde T-20 |
| El evento se elimina entre el listado y la apertura del armado | `InvalidOperationException` («No se encontró ningún evento con ese ID.») | La carga de `FrmCompraCliente` solo captura `ReglaNegocioException` | Leído en el código. Con T-27 lo atrapa el manejador global y muestra el mensaje de Negocio |
| ~~Cambiar `RESERVA_MINUTOS` con una compra en curso~~ | ~~`InvalidOperationException` en el pago~~ | **Resuelto por T-20**: el plazo sale de la reserva registrada y CP-43 lo prueba | Cerrado |

**Corrección aprobada por JC y encargada a Codex: T-27** (`docs/TAREAS-CODEX-PRUEBAS.md`,
«Quinta tanda»):
- **`ManejadorErrores`** se instala en `Program.Main` (`Application.ThreadException` y
  `AppDomain.CurrentDomain.UnhandledException`) y muestra un mensaje en castellano, sin
  detalles técnicos:
  - ante un error de base de datos: «No se pudo conectar con la base de datos. Verifique que
    SQL Server esté en funcionamiento y vuelva a intentar.»;
  - ante una `InvalidOperationException` de Negocio, su propio mensaje, que ya está en
    castellano;
  - ante cualquier otro error, un mensaje general.
- **`frmPago` captura `InvalidOperationException`:** avisa «Pago no realizado» y vuelve al
  listado.
- **Pruebas:** CP-51 (el mensaje según el tipo de error), CP-52 (el pago con el evento
  eliminado) y CP-53 (`Program.Main` instala el manejador).
- **Después, Claude Code** vuelve a correr `tools/Excepciones/VerificarErrorNoControlado.ps1`
  con el manejador instalado, para comprobar que con la base caída ya no aparece el cuadro de
  .NET.

Es la base natural de la bitácora (T06) si el docente pide los anexos técnicos.

---

## Para la defensa

> «Las reglas de negocio se lanzan como una excepción propia, `ReglaNegocioException`, con el
> código de la regla y un mensaje para el usuario. Datos solo deshace la transacción y
> traduce dos casos, duplicado y falta de cupo, que Negocio convierte en reglas. Cada
> pantalla captura esas excepciones y muestra el mensaje. Doce de las diecinueve reglas que
> rechazan operaciones tienen una prueba automática que provoca el error. Lo que identificamos
> como débil es que no hay un manejador global: un error no previsto, como la base caída, llega
> al usuario con el cuadro estándar de .NET. Lo verificamos sobre la aplicación y lo
> corregimos con un manejador global que muestra un mensaje en castellano.»
