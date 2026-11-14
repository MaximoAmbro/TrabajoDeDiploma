# Plan de mutación de las defensas de seguridad

Claude Code, 13/11/2026, a pedido de JC. **Solo lectura: no se ejecutó ninguna mutación.** Se
ejecuta cuando Codex libere la base de pruebas y el escritorio.

El criterio es el de `docs/LISTA-REVISION-SEGURIDAD.md`, sección 0: una prueba de seguridad
cuenta solo si cumple **P-1 a P-4**. Este plan prepara la condición **P-1**: la prueba falla cuando
se quita la defensa que dice probar.

## 0. Cómo leerlo

Cada mutación rompe una defensa y nombra la prueba que debería fallar. El **pronóstico** sale de
leer el código:

| Pronóstico | Significa |
|---|---|
| **Muere** | La prueba falla con la mutación: la defensa está probada |
| **Sobrevive** | Todas las pruebas pasan sin la defensa: **la defensa no tiene prueba** |
| **Equivalente** | Pasa porque otra defensa hace lo mismo más adelante. No es un hueco del sistema, pero esa línea no está probada por sí sola |
| **Incierto** | Depende de un dato que solo se ve al correr |

**Las sobrevivientes son el resultado que importa:** son pruebas que faltan, o pruebas que pasan
sin probar nada.

### El código está cambiando mientras se escribe esto

Codex tiene sin terminar, en `Repo/`, los arreglos de K-3 y R-01:
- `GestorSesion.Iniciar` pasó a `internal`;
- la sesión pasó a ser una copia inmutable (`SesionAutenticada`);
- `Autenticar` ahora da un mensaje único;
- hay pruebas nuevas, CP-113 y CP-114, en `Pruebas/PruebasRevisionSeguridad.cs`.

Por eso las mutaciones se anclan al **método y al fragmento de código**, no al número de línea.
Las de la sección 1 marcadas «(en curso)» se confirman cuando Codex entregue su reporte.

## 1. La sesión y los perfiles (SEG-3, SEG-4, R-01)

| # | Defensa | Mutación | Prueba que debe fallar | Pronóstico |
|---|---|---|---|---|
| M-01 | `GestorSesion.RequerirAdministrador`: `UsuarioActual == null \|\|` | Quitar ese fragmento | CP-94 (sin sesión sale `NullReferenceException`, no la esperada) | Muere |
| M-02 | `RequerirAdministrador`: `UsuarioActual.TipoUsuario != "Propietario"` | Reemplazar por `false` | CP-93 (afirma RN-34), CP-99 (cliente que registra un ingreso) | Muere |
| M-03 | `RequerirUsuario`: `UsuarioActual.Id != idUsuario` | Reemplazar por `false` (cualquier sesión opera sobre cualquier cuenta) | CP-91, CP-92 | Muere |
| M-04 | `RequerirUsuario`: el Administrador opera sobre cuentas ajenas | Quitar `UsuarioActual.TipoUsuario != "Propietario" &&` | CP-99 (el Administrador lee las entradas de un cliente) | Muere |
| M-05 | `GestorReportes.VerificarAdministrador`: perfil distinto de Administrador | Reemplazar la condición del perfil por `false` | CP-35 | Muere |
| M-06 | `GestorUsuarios.Registrar` es `internal` (SEG-4) | Volverlo `public` | CP-94 (afirma que `GetMethod("Registrar")` es nulo) | Muere |
| M-07 | `GestorSesion.Iniciar` es `internal` **(en curso)** | Volverlo `public` | CP-113, fila «inicio» | A confirmar |
| M-08 | `SesionAutenticada` copia los datos al iniciar **(en curso)** | Devolver el `Usuario` original, mutable | CP-113, filas «perfil» e «identidad» | A confirmar |

### Una mutación por método protegido (V-3.20)

**La mutación es la misma para todos:** se borra la línea `GestorSesion.Instance.Requerir…` o
`RequerirCompra(…)` del método, y nada más.

| # | Método | Prueba que debe fallar | Pronóstico | Por qué |
|---|---|---|---|---|
| M-10 | `GestorEventos.Crear` | CP-94 | Muere | Sin la guarda, `ValidarEvento` lanza `ArgumentException`, que no es la esperada |
| M-11 | `GestorEventos.Publicar` | CP-94 | Muere (resuelta el 14/11) | El evento de `Fake.EventoPublicado` tiene un sector de 5 en un local de 100.000: sin la guarda, `Publicar` pasa RN-06 y RN-38 e inserta un evento duplicado sin lanzar nada, y CP-94 falla |
| M-12 | `GestorEventos.Eliminar` | CP-93 | Muere | CP-93 afirma el código RN-34 |
| M-13 | `GestorEventos.Suspender` | CP-94 | Muere | Sin la guarda, no lanza nada |
| M-14 | `GestorEventos.CambiarPreventa` | CP-94 | Muere | Sin la guarda, no lanza nada |
| M-15 | `GestorEventos.AgregarSector` | CP-94 | Muere | `ValidarSector(new Sector())` lanza `ArgumentException` |
| M-16 | `GestorEventos.EliminarSector` | CP-94 | **Sobrevive** | El sector es el único del evento: lanza RN-06, que también es `ReglaNegocioException`. **CP-94 no mira qué regla rechazó** (falla P-2) |
| M-17 | `GestorLocales.Registrar` | CP-94 | Muere | `ArgumentException` por el nombre vacío |
| M-18 | `GestorLocales.PorPropietario` | — | **Sobrevive** | Ninguna prueba lo llama como cliente ni sin sesión |
| M-19 | `GestorListaEspera.Inscribir` | CP-94 | **Sobrevive** | El evento no está agotado: lanza RN-10, que CP-94 acepta igual (P-2) |
| M-20 | `GestorListaEspera.EstaInscripto` | — | **Sobrevive** | Sin prueba negativa |
| M-21 | **`GestorListaEspera.DeEvento`**, que devuelve nombres y mails | — | **Sobrevive** | Sin prueba negativa. **Es V-3.11, uno de los dos métodos que más importan** |
| M-22 | `GestorIngresos.Registrar` | CP-99 | Muere | CP-99 prueba como cliente y sin sesión |
| M-23 | `GestorIngresos.Contar` | CP-99 | Muere | Lo llama sin sesión |
| M-24 | `GestorIngresos.EventosDeHoy` | — | **Sobrevive** | Sin prueba negativa |
| M-25 | `GestorCompras.Iniciar` | CP-94 | Equivalente | `CalcularDescuento`, que se llama adentro, vuelve a exigir al titular |
| M-26 | `GestorCompras.Confirmar` | CP-94 | Muere | Sin la guarda, confirma la compra |
| M-27 | `GestorCompras.Liberar` | CP-94 | Muere | Sin la guarda, libera |
| M-28 | `GestorCompras.ObtenerPorId` (que devuelva el repositorio directo) | — | **Sobrevive** | Nadie la llama con una compra ajena |
| M-29 | `GestorCompras.EntradasDe`, que devuelve los QR | CP-91 | Muere | Lo afirma como intruso |
| M-30 | `GestorCompras.PagosDe` | — | **Sobrevive** | Sin prueba negativa (últimos 4 dígitos y token) |
| M-31 | `GestorCompras.SegurosDe` | — | **Sobrevive** | Sin prueba negativa |
| M-32 | `GestorCompras.HistorialDe` | CP-92 | Muere | — |
| M-33 | `GestorCompras.Reembolsar`, primera guarda | CP-91 | Equivalente | `RequerirCompra` la repite enseguida |
| M-34 | `GestorCompras.Reembolsar`, `RequerirCompra` → lectura directa | CP-91 | Muere | — |
| M-35 | `RequerirCompra`, primera línea (sesión) | CP-94 | Equivalente | La segunda línea rechaza igual sin sesión. **Pero lee la base antes de verificar,** contra V-3.4. No se puede probar con una aserción: se informa como hallazgo de lectura |
| M-36 | `RequerirCompra`, segunda línea (titular) | CP-91 | Muere | — |
| M-37 | `GestorFidelizacion.ObtenerCliente` | CP-92 | Muere | — |
| M-38 | `GestorFidelizacion.ComprasEnVentana` | — | **Sobrevive** | Sin prueba negativa |
| M-39 | `GestorFidelizacion.CalcularDescuento` | — | **Sobrevive** | Sin prueba negativa |
| M-40 | `GestorFidelizacion.ReevaluarNivel` | CP-94 | Muere (resuelta el 14/11) | `Fake.Cliente` graba `FechaUltimaEvaluacion` = hoy y `ObtenerCliente` la lee: sin la guarda, el método retorna sin lanzar y CP-94 falla |
| M-41 | `GestorFidelizacion.AscenderSiCorresponde` | CP-94 | Equivalente | `ComprasEnVentana` la vuelve a exigir |
| M-42 | `GestorPaquetes.Nuevo` | — | **Sobrevive** | Sin prueba negativa |
| M-43 | `GestorSuscripciones.Contratar` | CP-94 | Muere | — |
| M-44 | `GestorSuscripciones.Cancelar` | CP-94 | Muere | Sin suscripción, lanza `InvalidOperationException` |
| M-45 | `GestorSuscripciones.ActivaDe` | CP-92 | Muere | — |
| M-46 | `GestorUsuarios.CrearAdministrador` | CP-94 | Muere | Afirma también que la cuenta no se creó |
| M-47 | `GestorReportes.Ejecutar` | — | **Sobrevive** | `Crear` ya exige; nadie llama a `Ejecutar` con un reporte armado y otra sesión |
| M-48 | `GestorReportes.EventosEntre` | — | **Sobrevive** | Sin prueba negativa |

## 2. Contraseñas (SEG-1, SEG-6, SEG-8)

| # | Defensa | Mutación | Prueba que debe fallar | Pronóstico |
|---|---|---|---|---|
| M-50 | `CambiarContrasena` verifica la actual | `ContrasenaValida` devuelve siempre `true` | CP-95 | Muere |
| M-51 | Cada fallo suma un intento | En `RegistrarIntentoFallido`, no llamar a `ActualizarIntentos` | CP-95 (afirma 1), CP-28 | Muere |
| M-52 | La cuenta bloqueada no opera | Quitar el `if` de `BloqueadoHasta` en `AutenticarCuenta` | CP-96, CP-28 | Muere |
| M-53 | El cambio no revela si el mail existe | En `CambiarContrasena`, pasar `ocultarCuenta: false` | CP-97 | Muere |
| M-54 | El cambio no reemplaza la sesión activa | En `CambiarContrasena`, pasar `iniciarSesion: true` | CP-98 | Muere. CP-98 abre una pantalla: va al final |
| M-55 | Un hash guardado no sirve como contraseña | En `ContrasenaValida`, quitar `!Seguridad.EsHash(usuario.Contraseña) &&` | — | **Sobrevive.** Ninguna prueba intenta entrar escribiendo el hash como contraseña |
| M-56 | Un mail no se registra dos veces | Quitar `ExisteMail` en `Registrar` | CP-26 | Equivalente: `UQ_Usuario_Mail` y la captura de `RegistroDuplicadoException` dan el mismo RN-01 |
| M-57 | Los hashes nuevos usan 310.000 | `Iteraciones = 100000` | CP-108 | Muere |
| M-58 | `Verificar` usa las iteraciones guardadas | Usar la constante | CP-108 (el hash de 100.000 deja de verificar) | Muere |
| M-59 | Un hash más fuerte no se degrada | En `NecesitaActualizar`, `< Iteraciones` → `!= Iteraciones` | CP-105, fila de 600.000 | Muere. CP-105 es una medición, pero esta aserción la tiene |
| M-60 | Sal y hash de largo válido | Quitar el control de largo en `Verificar` | CP-107 (`PBKDF2$1$$` autenticaría) | Muere |
| M-61 | Iteraciones no positivas | Quitar `iteraciones <= 0` | CP-107 | Equivalente: con 0 o −1 iteraciones el hash calculado igual no coincide |
| M-62 | Desborde controlado | Quitar el `catch (OverflowException)` | CP-107 | Muere |
| M-63 | Comparación en tiempo constante | `SonIguales` corta en el primer byte distinto | — | **Sobrevive, y es esperable:** una prueba funcional no ve el tiempo. Se verifica por lectura y no se cuenta como prueba |
| M-64 | Respuesta de login uniforme **(K-3, en curso)** | Volver a lanzar el mensaje con intentos restantes | CP-114 | A confirmar |

> **El tiempo de respuesta** (V-1.6) no se prueba con una mutación. Codex agregó un hash de
> relleno para la cuenta bloqueada. Cuando entregue, se mide aparte: correo inexistente,
> existente y bloqueado, 20 veces cada uno.

## 3. Control de ingreso (SEG-2)

| # | Defensa | Mutación | Prueba que debe fallar | Pronóstico |
|---|---|---|---|---|
| M-70 | Código inexistente | Quitar `if (entrada == null)` en `RechazoDe` | CP-102 | Muere (`NullReferenceException`) |
| M-71 | Otro evento | Quitar la línea de `OtroEvento` | CP-103 | Muere: el `UPDATE` no actualiza y el motivo sale «NoConfirmado» |
| M-72 | Reembolsada | Quitar la línea de `Reembolsado` | CP-101 | Muere |
| M-73 | Pendiente o vencida | Quitar la línea de `NoConfirmado` | — | **Sobrevive:** no hay prueba con una entrada pendiente ni vencida (V-2.4). Además, el `UPDATE` la frena y el motivo de respaldo es el mismo |
| M-74 | Ya usada | Quitar la línea de `YaUsado` | CP-100 | Muere |
| M-75 | Antes del horario | Quitar `ahora < evento.Fecha.AddHours(-antes)` | CP-103 (evento en 2 días) | Muere |
| M-76 | Después del horario | Quitar `ahora > evento.Fecha.AddHours(despues)` | CP-103 (evento hace 2 días) | Muere |
| M-77 | Bordes del horario | `<` → `<=` y `>` → `>=` | — | **Sobrevive:** ninguna prueba toca los bordes (V-2.7) |
| M-78 | Horas leídas de `Parametros` | `antes = 6` escrito en el código | CP-111 (fija 96 h y el evento a 72 h) | Muere |
| M-79 | Solo eventos publicados | Quitar `evento.Estado != GestorEventos.EstadoPublicado` | — | **Sobrevive:** no hay prueba con un evento suspendido |
| M-80 | Código normalizado | Quitar el `.Trim()` | — | **Sobrevive:** ninguna prueba manda espacios |
| M-81 | **El `UPDATE` decide:** `AND FechaIngreso IS NULL` | Quitarlo de `RepositorioEntradas.RegistrarIngreso` | CP-104 | Incierto: CP-100 pasa igual porque la lectura previa ya lo frena. Solo CP-104 lo ve, si los dos hilos leen antes de que alguno escriba. **Se corre 20 veces** |
| M-82 | El `UPDATE` exige `Estado = 'Confirmada'` | Quitarlo | — | **Sobrevive** (equivalente en secuencia): la lectura previa lo frena. La carrera reembolso-ingreso no tiene prueba |
| M-83 | El `UPDATE` exige `ID_Evento` | Quitarlo | — | **Sobrevive** (equivalente en secuencia) |
| M-84 | Leer y después escribir (V-2.16) | Reemplazar el `UPDATE` condicional por `SELECT`, 50 ms de espera y `UPDATE` sin condición | CP-104 | Incierto. **Se corre 20 veces.** Si pasa alguna vez, CP-104 no prueba la atomicidad |
| M-85 | Una fila exacta | `ExecuteNonQuery() == 1` → `>= 0` | CP-104 | Incierto, como M-81 |
| M-86 | Usada no se devuelve, en el gestor | Quitar el `if` de `FechaIngreso` en `GestorCompras.Reembolsar` | CP-111 | Muere |
| M-87 | Usada no se devuelve, en la base | Quitar `AND FechaIngreso IS NULL` del `UPDATE` de `RepositorioCompras.Reembolsar` | CP-111 (llama al repositorio directo) | Muere |
| M-88 | `CK_Entradas_Estado` | Quitar la restricción del esquema de la copia | CP-110 | Muere |
| M-89 | `UX_Entradas_CodigoQR` | Quitar el índice único de la copia | — | **Sobrevive:** sin prueba. Riesgo bajo, porque el código tiene 128 bits al azar |

## 4. Búsqueda (SEG-7)

| # | Defensa | Mutación | Prueba que debe fallar | Pronóstico |
|---|---|---|---|---|
| M-90 | Escapar `[`, `%` y `_` en `RepositorioEventos` | Quitar los tres `Replace` | CP-106 | Muere |

## 5. Lo que se espera encontrar

**Pronóstico global, sobre 83 mutaciones:**

| Pronóstico | Cuántas |
|---|---|
| Mueren | 48 |
| Sobreviven sin prueba | 23 |
| Equivalentes | 6 |
| Inciertas, de concurrencia | 3 |
| A confirmar | 3 (provisorio: las tres mueren) |

Suma: 48 + 23 + 6 + 3 + 3 = 83. El 13/11 decía 46 que mueren y 5 inciertas; M-11 y M-40 se
resolvieron leyendo el código el 14/11 (sección 5b).

### 5b. Las 14 que no son «muere» ni «sobrevive» (revisado el 14/11)

**Equivalentes (6): M-25, M-33, M-35, M-41, M-56 y M-61.** Sí están clasificadas. La defensa
existe dos veces: si se rompe una, la otra rechaza lo mismo, y la prueba pasa con razón.
- Lo que se puede afirmar: el comportamiento está protegido.
- Lo que no: que esa línea en particular esté probada. Si alguien borra la otra copia, esta queda
  sin prueba.
- M-35 deja además un hallazgo de lectura: `RequerirCompra` lee la base antes de verificar la
  sesión (V-3.4).

**Inciertas que se resolvieron leyendo (2): M-11 y M-40 mueren.** Dependían del estado que dejan
los auxiliares de prueba, y ese estado se puede leer en `Pruebas/Fake.cs` sin correr nada (ver
las filas de la tabla).

**Inciertas de concurrencia (3): M-81, M-84 y M-85.** Estas no se pueden clasificar leyendo, y
no es por falta de lectura.
- Las tres rompen la atomicidad del ingreso: el `UPDATE` condicional que decide quién entra
  primero.
- Su resultado depende de cómo intercale el sistema operativo los dos hilos de CP-104, y eso
  cambia de una corrida a otra.
- Si la mutación muere, es porque los dos hilos leyeron antes de que alguno escribiera; si
  sobrevive, es porque uno terminó antes de que el otro empezara. Las dos cosas pueden pasar con
  el mismo código.
- **Mientras no se corran, sobre la atomicidad del ingreso no se puede afirmar nada:** ni que
  esté probada ni que no. El código es correcto (lo leí), pero ninguna prueba lo demuestra con
  certeza.
- Correr cada una 20 veces da evidencia, no certeza. **El arreglo de fondo es una prueba más
  fuerte:**
  - una barrera que suelte los hilos al mismo tiempo;
  - diez hilos en lugar de dos;
  - repetir la prueba en un bucle.
  Con eso, M-81, M-84 y M-85 mueren siempre, y quedan clasificables.

**A confirmar (3): M-07, M-08 y M-64.** Rompen defensas de K-6 y K-3, que Codex commiteó en
`2af267a` el 14/11. Las leí en ese código. **Pronóstico, las tres mueren:**
- M-07 (volver `public` a `Iniciar`) la mata CP-113, fila «inicio», que afirma
  `iniciar.IsAssembly`;
- M-08 (guardar el `Usuario` original en lugar de una copia) la matan las filas «perfil» e
  «identidad», que modifican el usuario después de iniciar la sesión y esperan el rechazo;
- M-64 (volver al mensaje con intentos restantes) la mata CP-114, que compara el texto de los
  dos rechazos.

Quedan «a confirmar» solo porque todavía no se corrieron: el código ya está commiteado. La mutación sigue esperando a que Codex libere la base, porque al 14/11 a la tarde sigue trabajando, ahora en la lista de espera.

**Lo que dicen las sobrevivientes, para Codex:**

1. **CP-94 no cumple P-2.** Acepta cualquier `ReglaNegocioException` y no mira cuál. Por eso M-16
   y M-19 sobreviven: rechaza otra regla y la prueba pasa igual.
   - **Arreglo:** afirmar `error.Regla` en cada operación, RN-34 o RN-41 según corresponda. Es
     un cambio de una línea por operación.
2. **Doce métodos protegidos no tienen prueba negativa:**
   - **`DeEvento`, el primero:** devuelve nombres y mails de otros clientes;
   - `PagosDe`, `SegurosDe` y `ObtenerPorId`;
   - `PorPropietario`, `EstaInscripto` y `EventosDeHoy`;
   - `ComprasEnVentana`, `CalcularDescuento` y `Nuevo`;
   - `Ejecutar` y `EventosEntre`.

   **Arreglo:** una prueba con una fila por método, como cliente ajeno y sin sesión, que
   afirme la regla.
3. **Ingreso:** faltan cuatro casos:
   - bordes del horario (V-2.7), fijados con eventos a ±6 h y ±12 h exactos, más o menos un
     minuto;
   - entrada pendiente y entrada vencida (V-2.4);
   - evento suspendido;
   - código con espacios alrededor.
4. **CP-104 suelta solo dos hilos, una vez** (V-2.16 pide unos 20 intentos, repetidos). Con dos
   hilos, M-81 y M-84 pueden sobrevivir por azar. Si sobreviven, el arreglo es subir a 20
   hilos y repetir 5 veces dentro de la prueba.
5. **M-55: nadie prueba que el hash guardado no sirva como contraseña.** El código lo
   defiende; falta la prueba.

## 6. Cómo se ejecuta, cuando Codex libere la base

**Condiciones previas (las dos, sin excepción):**
- **Codex commiteó su trabajo** (K-3, K-6, CP-113 y CP-114; también K-4, que apareció en su
  cambio). El árbol de trabajo nace de un commit, y lo que no esté commiteado no entra en la
  copia. La mutación corre sobre ese commit, no antes.
- **La cadena de conexión efectiva está verificada** (paso 2).

1. **Copia aparte:** `git worktree add --detach D:\mutacion-eventaura HEAD`. Queda fuera de
   `D:\proyectos\TrabajoDeDiploma`.
2. **Configuración local:** copiar el `Pruebas/App.config` de JC a la copia. La copia nunca se
   commitea.
   - **Por qué es imprescindible:** el árbol de trabajo nace del commit, y en el commit la cadena
     es la de Máximo: `Data Source=DESKTOP-5QSNMRE\AMBROMSSQLSERVER; Initial
     Catalog=BddEventAura_Pruebas` (leído el 14/11 con `git show HEAD:Pruebas/App.config`).
     `skip-worktree` protege solo el archivo del árbol principal, no la copia.
   - **Qué pasaría sin el paso:** lo más probable es que la conexión falle, porque esa máquina no
     está en la red de JC. Pero si fuera alcanzable, cada corrida **borraría y recrearía la base
     de pruebas de Máximo**. Su base de desarrollo no corre riesgo: `BaseDePrueba.Preparar` se
     niega a tocar un catálogo que no termine en `_Pruebas`. Igual es inaceptable, y por eso
     sigue esta verificación.
   - **Verificación obligatoria, antes de la primera mutación** (pedido de JC del 14/11):
     1. compilar la copia una vez sin mutar nada;
     2. leer la cadena **efectiva**: la del `Pruebas.dll.config` que queda junto al `Pruebas.dll`
        que va a correr, no la del `App.config` fuente;
     3. comprobar que `Data Source` sea exactamente `.\SQLEXPRESS` e `Initial Catalog`
        exactamente `BddEventAura_Pruebas`, y copiar la línea al informe;
     4. el script repite la misma comprobación antes de cada corrida y aborta si cambia.
   - **Si no se puede verificar, no se arranca.** Se informa y se espera a JC.
3. **Por cada mutación, con un script:**
   - aplicar el cambio;
   - compilar;
   - correr **solo** las pruebas nombradas, filtradas por nombre visible;
   - anotar si fallaron;
   - revertir el archivo.

   Cada corrida recrea `BddEventAura_Pruebas`, la misma base que usa Codex: por eso, con Codex
   quieto. Las pruebas con pantalla (CP-98, CP-99 y CP-113 si la tiene) van al final.
4. **Las de concurrencia** (M-81, M-84 y M-85) corren CP-104 20 veces cada una.
5. **Al terminar:**
   - `git worktree remove D:\mutacion-eventaura` y `git worktree prune`;
   - correr la batería completa **desde el árbol principal**. Recrea la base y tiene que dar los
     mismos números que la última de Codex;
   - si no los da, la mutación dejó algo, y se informa antes de seguir.

**Estimación:**
- **Lo mío:** unas 25 vueltas de herramienta para el script y el informe. La ejecución lleva
  unas 80 corridas de compilar y probar, de 1 a 2 minutos cada una, más 60 repeticiones cortas de
  CP-104: entre 1,5 y 3 h de reloj, sin atención mía.
- **Portones humanos:**
  - el commit de Codex de su propio trabajo (su zona la commitea Codex);
  - el aviso de que Codex terminó;
  - la lectura del informe.
