# Encargo para Codex: las defensas de seguridad que ninguna prueba detecta

Claude Code, 14/11/2026, a pedido de JC. Sale de la corrida de mutación del 14/11
(`docs/INFORME-MUTACION-SEGURIDAD.md`): se rompió a propósito cada defensa, una por vez, y se
corrió la batería completa. **Cada tarea de abajo es una defensa que se rompió sin que fallara
ninguna prueba.** La prueba que hay que escribir es la que habría fallado.

## Reglas para todas las tareas

1. **Cada prueba afirma la regla, no solo el tipo de excepción.** Es la condición P-2 de
   `docs/LISTA-REVISION-SEGURIDAD.md`:
   ```csharp
   var error = Assert.ThrowsException<ReglaNegocioException>(() => …);
   Assert.AreEqual("RN-41", error.Regla);
   ```
   Una prueba que acepta cualquier `ReglaNegocioException` pasa aunque rechace otra regla. Por
   eso hoy sobreviven M-16 y M-19.
2. **Cada prueba afirma también que no cambió nada (P-3):** el dato no se devolvió, la fila no se
   escribió, la sesión sigue siendo la que era.
3. **Para dar una tarea por terminada, rompé la defensa a mano y comprobá que la prueba falla
   (P-1).** La columna «Mutación» dice qué línea romper. Después restaurala.
4. **Numerá los casos a partir del primero libre** al momento de entregar (al 14/11 el último
   commiteado es CP-120, y en el árbol hay hasta CP-124 sin commitear). Registralos con su comentario `// CP-xx — RF, RN` como siempre.
5. Usá `Fake.ComoCliente`, `Fake.ComoAdministrador` y `GestorSesion.Instance.Cerrar()` para
   armar cada sesión, como en `Pruebas/PruebasSeguridad.cs`.

## Las tareas, de la más grave a la menos grave

El orden es por lo que pasa si la defensa cae: primero lo que expone datos personales de otros
clientes, después lo que permite entrar sin la contraseña, después el control de ingreso y al
final lo que expone datos operativos sin valor para un atacante.

Códigos de regla de las guardas: `RequerirAdministrador` y `VerificarAdministrador` de reportes
lanzan **RN-34**; `RequerirUsuario` y `RequerirCompra` lanzan **RN-41**.

Cada tarea dice:
- **Prueba:** qué escribir.
- **Afirmar:** qué asserts, en orden.
- **Por qué hoy no se afirma:** qué hace la batería actual.
- **Mutación:** qué romper para comprobar que la prueba falla (regla 3).

---

### Tarea 0 — Commitear la CP-104 nueva (M-81, M-85)

- **Prueba:** ya está escrita en `Pruebas/PruebasIngresos.cs`: diez puertas, `Barrier(10)`, cien
  rondas. No hay que escribir nada.
- **Afirmar:** lo que ya afirma.
- **Por qué hoy no se afirma:** la CP-104 commiteada tiene dos hilos. Corrida sola, detecta M-81
  19 de 20 veces y M-85 18 de 20; en la corrida de la batería completa no detectó ninguna de las
  dos. La nueva las detectó 3 de 3, igual que M-84, y sin mutar no falló nunca.
- **Mutación:** ya verificada por la corrida del 14/11. Basta con que la batería completa siga
  verde.

### Tarea 1 — `GestorListaEspera.DeEvento` (M-21)

- **Prueba:**
  1. Crear `evento = Fake.EventoPublicado(1000m, 1)`.
  2. Crear el cliente A, con `Fake.CompraConfirmada(a, evento, 1, false)`, para agotarlo.
  3. Crear el cliente B; `Fake.ComoCliente(b)`; `GestorListaEspera.Instance.Inscribir(evento.Id, b.Id)`.
- **Afirmar:**
  1. Con `Fake.ComoCliente(a)`: `DeEvento(evento.Id)` lanza, y `error.Regla == "RN-34"`.
  2. Con `GestorSesion.Instance.Cerrar()`: lanza, y la regla es `"RN-34"`.
  3. Control positivo, para probar que el dato existe: con `Fake.ComoAdministrador()`,
     `DeEvento(evento.Id)` devuelve una fila con el id de B.
- **Por qué hoy no se afirma:** ninguna prueba llama a `DeEvento` con otra sesión que no sea la
  del administrador. Es el método que devuelve nombre y mail de los inscriptos.
- **Mutación:** comentar `GestorSesion.Instance.RequerirAdministrador();` al principio de
  `DeEvento`.

### Tarea 2 — `GestorCompras.PagosDe` (M-30)

- **Prueba:** cliente A con `compra = Fake.CompraConfirmada(a, evento, 1, true)`; cliente B.
- **Afirmar:**
  1. Con `Fake.ComoCliente(a)`: `PagosDe(compra)` tiene un elemento. Es el control positivo.
  2. Con `Fake.ComoCliente(b)`: `PagosDe(compra)` lanza con `"RN-41"`.
  3. Sin sesión: lanza con `"RN-41"`.
- **Por qué hoy no se afirma:** no hay prueba negativa de `PagosDe`. El pago trae los últimos
  cuatro dígitos y el token.
- **Mutación:** comentar `RequerirCompra(c == null ? 0 : c.Id);` en `PagosDe`.

### Tarea 3 — `GestorCompras.ObtenerPorId` (M-28)

- **Prueba:** la misma preparación que la tarea 2; puede ir en el mismo método de prueba.
- **Afirmar:**
  1. Con B: `ObtenerPorId(compra.Id)` lanza con `"RN-41"`.
  2. Sin sesión: lanza con `"RN-41"`.
  3. Con A: devuelve la compra con el mismo `Id`.
- **Por qué hoy no se afirma:** nadie llama a `ObtenerPorId` con una compra ajena.
- **Mutación:** cambiar `return RequerirCompra(idCompra);` por
  `return RepositorioCompras.Instance.ObtenerPorId(idCompra);`.

### Tarea 4 — `GestorCompras.SegurosDe` (M-31)

- **Prueba:** la misma preparación (la compra tiene que ser `conSeguro: true`).
- **Afirmar:**
  1. Con A: `SegurosDe(compra)` no está vacío.
  2. Con B: lanza con `"RN-41"`.
  3. Sin sesión: lanza con `"RN-41"`.
- **Por qué hoy no se afirma:** no hay prueba negativa.
- **Mutación:** comentar `RequerirCompra(c == null ? 0 : c.Id);` en `SegurosDe`.

### Tarea 5 — Un hash guardado no sirve como contraseña (M-55)

- **Prueba:**
  1. Crear el cliente A.
  2. Leer `string hash = RepositorioUsuarios.Instance.ObtenerPorMail(a.Mail).Contraseña;`.
  3. Cerrar la sesión.
- **Afirmar:**
  1. `GestorUsuarios.Instance.Autenticar(a.Mail, hash)` lanza `ReglaNegocioException`.
  2. Su `Regla` es la misma que la de `Autenticar(a.Mail, "OtraClaveIncorrecta1")`. Hacé las dos
     llamadas en la misma prueba y compará; son dos fallos, menos que el bloqueo.
  3. `GestorSesion.Instance.UsuarioActual` es `null`.
  4. `ObtenerPorMail(a.Mail).Contraseña` sigue siendo igual a `hash`: no se «migró».
- **Por qué hoy no se afirma:** ninguna prueba intenta entrar escribiendo el hash.
- **Mutación:** en `ContrasenaValida`, quitar `!Seguridad.EsHash(usuario.Contraseña) && `.

### Tarea 6 — Evento suspendido no admite ingreso (M-79)

- **Prueba:** en `PruebasIngresos`, con su `Preparar`. Después:
  `Fake.CambiarEstadoDeEvento(evento.Id, GestorEventos.EstadoSuspendido)`.
- **Afirmar:**
  1. `Registrar(entrada.CodigoQR, evento.Id).Motivo == MotivoIngreso.FueraDeHorario`.
  2. `RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso` es `null`.
  3. `GestorIngresos.Instance.Contar(evento.Id) == 0`.
- **Por qué hoy no se afirma:** no hay prueba con un evento suspendido, y la entrada está dentro
  del horario.
- **Mutación:** en `Registrar`, cambiar `evento.Estado != GestorEventos.EstadoPublicado ||` por
  `false ||`.

### Tarea 7 — El `UPDATE` de ingreso filtra por evento (M-83)

- **Prueba:** a nivel repositorio, en `PruebasIngresos`.
  `Evento otro = Fake.EventoPublicado(1000m, 5)`.
- **Afirmar:**
  1. `RepositorioEntradas.Instance.RegistrarIngreso(entrada.CodigoQR, otro.Id, DateTime.Now)`
     devuelve `false`.
  2. `PorCodigo(entrada.CodigoQR).FechaIngreso` es `null`.
- **Por qué hoy no se afirma:** CP-103 pasa por el gestor, y `RechazoDe` rechaza el otro evento
  antes de llegar al `UPDATE`. El filtro del `UPDATE` es la defensa cuando dos puertas compiten,
  y solo se ve llamando al repositorio.
- **Mutación:** en `RegistrarIngreso`, cambiar `WHERE CodigoQR = @Codigo AND ID_Evento = @IdEvento`
  por `WHERE CodigoQR = @Codigo`.

### Tarea 8 — El `UPDATE` de ingreso exige entrada confirmada (M-82)

- **Prueba:** a nivel repositorio. Reembolsar la entrada como en CP-101 (mover el evento a
  +30 días, `Reembolsar`, y volverlo a +10 minutos).
- **Afirmar:**
  1. `RepositorioEntradas.Instance.RegistrarIngreso(entrada.CodigoQR, evento.Id, DateTime.Now)`
     devuelve `false`.
  2. `PorCodigo(entrada.CodigoQR).FechaIngreso` es `null`.
  3. Su `Estado` sigue siendo `Reembolsada`.
- **Por qué hoy no se afirma:** CP-101 pasa por el gestor, que lee el estado antes de escribir.
  El filtro del `UPDATE` es la defensa contra un reembolso que cae entre la lectura y la
  escritura.
- **Mutación:** cambiar `"AND Estado = 'Confirmada' AND FechaIngreso IS NULL"` por
  `"AND FechaIngreso IS NULL"`.

### Tarea 9 — Entrada pendiente o vencida (M-73)

- **Prueba:**
  1. Agregar a `Fake` un `CambiarEstadoDeEntrada(int idEntrada, string estado)`, igual que
     `CambiarEstadoDeEvento`, con `UPDATE Entradas SET Estado = @Estado WHERE ID = @ID`. Hace
     falta porque en el flujo normal el QR se asigna al confirmar, así que una entrada con código
     que no esté confirmada solo se arma por SQL.
  2. En `PruebasIngresos`, pasar la entrada a `"Pendiente"`.
- **Afirmar:**
  1. `Motivo == MotivoIngreso.NoConfirmado`.
  2. **`Mensaje == "La entrada está pendiente o vencida."`.** Esta aserción es la que la detecta:
     sin la defensa, el `UPDATE` igual frena y el motivo de respaldo es el mismo `NoConfirmado`,
     pero con el mensaje «La entrada cambió. Volvé a escanear.».
  3. `FechaIngreso` es `null`.
  4. Repetir con `"Vencida"`.
- **Por qué hoy no se afirma:** no hay prueba con una entrada pendiente ni vencida.
- **Mutación:** quitar la línea `if (entrada.Estado != EstadoEntrada.Confirmada) return
  Resultado(MotivoIngreso.NoConfirmado, …);` de `RechazoDe`.

### Tarea 10 — Bordes de la ventana horaria (M-77)

- **Cambio previo en `Repo/`:**
  1. `Registrar(string codigo, int idEvento)` pasa a llamar a un `internal ResultadoIngreso
     Registrar(string codigo, int idEvento, DateTime ahora)`, que reemplaza `DateTime.Now`
     por `ahora`.
  2. Seguí el mismo patrón que ya usaste en `EventosConIngresoAbierto(DateTime ahora)`.
  3. `Repo/AccesoPruebas.cs` ya tiene `InternalsVisibleTo("Pruebas")`.
- **Prueba:**
  1. Una compra con dos entradas: `Fake.CompraConfirmada(cliente, evento, 2, false)`.
  2. Leer `DateTime fecha = RepositorioEventos.Instance.ObtenerPorId(evento.Id).Fecha` **de la
     base**, porque `datetime` redondea.
  3. Leer `antes` y `despues` con `GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_ANTES")`
     y `ObtenerEntero("INGRESO_HORAS_DESPUES")`.
- **Afirmar:**
  1. `Registrar(cod1, id, fecha.AddHours(-antes).AddSeconds(-1)).Motivo == FueraDeHorario`.
  2. `Registrar(cod2, id, fecha.AddHours(despues).AddSeconds(1)).Motivo == FueraDeHorario`.
  3. `Registrar(cod1, id, fecha.AddHours(-antes)).Valido`: el borde está incluido, como hoy.
  4. `Registrar(cod2, id, fecha.AddHours(despues)).Valido`.
  5. Después de los dos rechazos, ninguna de las dos entradas tiene `FechaIngreso`.
- **Por qué hoy no se afirma:** ninguna prueba toca los bordes, y sin la hora inyectada no se
  pueden tocar.
- **Mutación:** cambiar `ahora < … || ahora > …` por `ahora <= … || ahora >= …`.

### Tarea 11 — Código con espacios (M-80)

- **Prueba:** en `PruebasIngresos`.
- **Afirmar:**
  1. `Registrar("  " + entrada.CodigoQR + "  ", evento.Id).Valido`.
  2. `PorCodigo(entrada.CodigoQR).FechaIngreso` tiene valor.
- **Por qué hoy no se afirma:** ninguna prueba manda espacios.
- **Mutación:** cambiar `codigo = (codigo ?? "").Trim();` por `codigo = (codigo ?? "");`.

### Tarea 12 — CP-94 tiene que afirmar la regla (M-16 y M-19)

- **Prueba:** no es una prueba nueva; se corrige CP-94 (`PruebasSeguridad.SinSesion`).
  1. Reemplazar el arreglo `Action[] operaciones` y el `foreach` por pares
     `(Action operacion, string regla)`.
  2. La regla esperada de cada operación es la de la guarda de su primera línea:
     `RequerirAdministrador` → `"RN-34"`; `RequerirUsuario` o `RequerirCompra` → `"RN-41"`.
  3. En el `foreach`:
     ```csharp
     var error = Assert.ThrowsException<ReglaNegocioException>(par.Item1);
     Assert.AreEqual(par.Item2, error.Regla, …);
     ```
     Agregá como mensaje el índice o el nombre de la operación, para saber cuál falló.
  4. Lo mismo para las dos llamadas a `CrearAdministrador` de después del bucle (`"RN-34"`).
- **Por qué hoy no se afirma:**
  - CP-94 acepta cualquier `ReglaNegocioException`.
  - Sin la guarda, `EliminarSector` rechaza por RN-06 (el sector es el único del evento), e
    `Inscribir` por RN-10 (el evento no está agotado). La prueba pasa igual.
- **Mutación:** comentar la guarda de `EliminarSector` (debe fallar con RN-06 ≠ RN-34). Después,
  la de `Inscribir` (debe fallar con RN-10 ≠ RN-41).

### Tareas 13 a 20 — Guardas sin prueba negativa

Pueden ir todas en un mismo archivo nuevo, `Pruebas/PruebasGuardasMutacion.cs`, un método por
tarea. La preparación es común: el cliente A, el cliente B,
`evento = Fake.EventoPublicado(1000m, 5)`. En cada una, **afirmar también el control positivo**:
con la sesión correcta la llamada funciona. Así una guarda que rechace a todos no pasa.

| Tarea | Mutación | Llamada | Sesión que debe ser rechazada | Regla | Control positivo |
|---|---|---|---|---|---|
| 13 | M-20 | `GestorListaEspera.Instance.EstaInscripto(evento.Id, b.Id)` | A, y sin sesión | RN-41 | Con B devuelve `false` |
| 14 | M-38 | `GestorFidelizacion.Instance.ComprasEnVentana(b.Id, DateTime.Now)` | A, y sin sesión | RN-41 | Con B devuelve `0` |
| 15 | M-39 | `GestorFidelizacion.Instance.CalcularDescuento(b, Fake.Paquete(evento, 1, false))` (armar el paquete con la sesión de B) | A, y sin sesión | RN-41 | Con B devuelve un resumen no nulo |
| 16 | M-42 | `GestorPaquetes.Instance.Nuevo(evento.Id, b.Id)` | A, y sin sesión | RN-41 | Con B devuelve un paquete con `IdEvento == evento.Id` |
| 17 | M-18 | `GestorLocales.Instance.PorPropietario(Fake.Administrador().Id)` | A, y sin sesión | RN-34 | Con `Fake.ComoAdministrador()` devuelve al menos un local |
| 18 | M-24 | `GestorIngresos.Instance.EventosDeHoy()`; si ya está tu renombre, `EventosConIngresoAbierto()` | A, y sin sesión | RN-34 | Con administrador no lanza |
| 19 | M-47 | `GestorReportes.Instance.Ejecutar(r)`, con `r` creado antes como administrador: `Crear(GestorReportes.Codigos[0], DateTime.Today.AddDays(-30), DateTime.Today)` | A, y sin sesión | RN-34 | Con administrador devuelve un `DataTable` |
| 20 | M-48 | `GestorReportes.Instance.EventosEntre(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(30))` | A, y sin sesión | RN-34 | Con administrador no lanza |

- **Por qué hoy no se afirma:** ninguna prueba llama a estos métodos con una sesión ajena ni sin
  sesión. M-47 tiene un motivo propio: `Crear` ya exige administrador, y nadie llama a `Ejecutar`
  con un reporte ya armado desde otra sesión.
- **Mutación:** en cada uno, comentar la guarda de la primera línea del método.

### Tarea 21 — Índice único del código QR (M-89)

- **Prueba:** en `PruebasIntegridadEntradas`, junto a CP-110 y con su mismo patrón de conexión y
  transacción con `Rollback`.
  1. Crear una compra confirmada con dos entradas.
  2. Dentro de una transacción, ejecutar `UPDATE Entradas SET CodigoQR = @Primero WHERE ID = @Segunda`.
- **Afirmar:**
  1. Lanza `SqlException` con `Number == 2601` (clave duplicada en índice único).
  2. Después del `Rollback`, la segunda entrada conserva su código.
- **Por qué hoy no se afirma:** no hay prueba del índice.
- **Mutación:** quitar `CREATE UNIQUE INDEX UX_Entradas_CodigoQR …` de `BaseDatos/01-esquema.sql`.
  **No commitear ese cambio:** se hace en la copia que recrea la base de pruebas y se restaura.

### Tarea 22 — Comparación en tiempo constante (M-63): no se escribe prueba

- **Prueba:** ninguna. Una prueba funcional no observa el tiempo, y medirlo daría una prueba
  inestable.
- **Afirmar:** —
- **Por qué hoy no se afirma:** se verificó leyendo `Seguridad.SonIguales`, que acumula
  `diferencia |= a[i] ^ b[i]` sin cortar. Queda así en el informe.
- **Mutación:** —

## Al terminar

En el reporte final:
- los CP asignados a cada tarea;
- confirmar, por cada una, que **la prueba falló con la defensa rota y pasó restaurada**;
- la batería completa: ejecuciones, cuántas pasan y cuáles fallan.

Con eso, Claude Code vuelve a correr la mutación de las 26 y actualiza la cifra del documento.
