# Informe: resistencia de las pruebas de seguridad, medida por mutación

Claude Code, 14/11/2026, a pedido de JC. Plan y pronóstico: `docs/PLAN-MUTACION-SEGURIDAD.md`.
Encargo que sale de acá: `docs/ENCARGO-PRUEBAS-MUTACION.md`.

## 1. Resultado en una línea

**De 77 defensas rotas a propósito, la batería completa detectó 51: 66,2 %.** Las 83 mutaciones
menos las 6 declaradas equivalentes antes de correr dan 77. Las 26 que no se detectaron no son
agujeros: la defensa está en el código y funciona. Lo que falta es la prueba que lo demuestre.

## 2. Qué se midió y cómo

- **Código medido:** el commit `fe04b1f` (14/11), con la batería de 115 casos que corre 198
  veces. No incluye el trabajo sin commitear de Codex, salvo la fase 2 (sección 5).
- **Las 83 mutaciones** son las de `docs/PLAN-MUTACION-SEGURIDAD.md`, M-01 a M-90. Cada una
  rompe una sola defensa: quita un control de sesión o de perfil, cambia una condición, saca
  un filtro del `UPDATE` o quita una restricción del esquema.
- **El procedimiento, una mutación por vez:**
  1. aplicar el cambio en una copia de trabajo aparte (`git worktree`, fuera del repositorio,
     nunca commiteada);
  2. compilar;
  3. correr la batería completa;
  4. la mutación «muere» si falla alguna prueba que no fallaba sin mutar;
  5. restaurar el archivo byte por byte.
- **Base:** una base propia, `BddEventAura_Mutacion_Pruebas`. Antes de cada corrida se comprobó
  que la cadena efectiva de `Pruebas.dll.config` y `Visual.exe.config` era exactamente
  `Data Source=.\SQLEXPRESS;Initial Catalog=BddEventAura_Mutacion_Pruebas;…`, la única del
  archivo. Si no, la corrida se abortaba. Ni `BddEventAura` ni `BddEventAura_Pruebas` se tocaron.
- **Batería sin mutar, antes y después:** 198 ejecuciones, 195 pasan y fallan 3, las mismas
  en las dos corridas (CP-69, CP-76 y CP-85, las tres de escala al 150 %). Esas tres no cuentan
  como muertes.
- **Concurrencia:** M-81, M-84 y M-85 se corrieron además 20 veces solo con CP-104. El control
  sin mutar, 20 veces, no falló nunca.
- **Tiempo:** unos 53 s por mutación y 83 s las de concurrencia; en total, unas tres horas de
  máquina.

## 3. Resultado contra el pronóstico

| Pronóstico | Murió | Sobrevivió |
|---|---|---|
| Muere (48) | 47 | 1: **M-43** |
| Sobrevive (23) | 0 | 23 |
| Equivalente (6) | 0 | 6 |
| A confirmar (3: M-07, M-08, M-64) | 3 | 0 |
| Incierta (3: M-81, M-84, M-85) | 1: M-84 | 2: M-81 y M-85 |
| **Total (83)** | **51** | **32** |

- **Los 23 que se pronosticaron como sobrevivientes sobrevivieron todos.** El pronóstico, hecho
  leyendo las pruebas, acertó en ese grupo sin excepciones.
- **M-07 y M-08** (sesión iniciable desde afuera, sesión que sigue al usuario mutable) mueren
  por CP-113.
- **M-64** (el bloqueo por intentos) muere por seis pruebas: CP-114, CP-28 y CP-68.
- **M-43 fue error mío**; está en la sección 4.

**Las 26 no detectadas:** 32 sobrevivientes − 6 equivalentes = 26, que son los 23 previstos, M-43,
M-81 y M-85.

## 4. Las equivalentes, aparte

No se usan para mejorar el número: las seis declaradas antes se sacan del denominador, como en
cualquier medición de mutación, y la séptima no se saca.

| ID | Defensa | Por qué el comportamiento no cambia |
|---|---|---|
| M-25 | Control de sesión en `GestorCompras.Iniciar` | `CalcularDescuento`, que se llama adentro, vuelve a exigir al titular |
| M-33 | Primera guarda de `GestorCompras.Reembolsar` | `RequerirCompra` la repite enseguida |
| M-35 | Primera línea de `RequerirCompra` (sesión) | La segunda línea rechaza igual sin sesión. **Pero lee la base antes de verificar** (V-3.4): no se puede probar con una aserción y queda como hallazgo de lectura |
| M-41 | Control en `GestorFidelizacion.AscenderSiCorresponde` | `ComprasEnVentana` la vuelve a exigir |
| M-56 | `ExisteMail` antes de registrar | `UQ_Usuario_Mail` y la captura de `RegistroDuplicadoException` dan el mismo RN-01 |
| M-61 | `iteraciones <= 0` al verificar un hash | Con 0 o −1 iteraciones, el hash calculado igual no coincide |
| **M-43** | Control de sesión en `GestorSuscripciones.Contratar` | **Pronosticada «Muere», sobrevivió.** Leyendo el código, es equivalente: `Contratar` llama a `TieneActiva` → `ActivaDe`, que hace el mismo `RequerirUsuario` antes de cualquier escritura. CP-94 pasa por esa segunda guarda |

**M-43 se cuenta como no detectada.** Reclasificarla como equivalente después de ver que
sobrevivió es justamente lo que infla un número. Si JC la acepta como equivalente, la cifra
pasa a 51 de 76 (67,1 %). No la recomiendo para el documento.

## 5. Concurrencia: M-81, M-84 y M-85

| Mutación | Qué rompe | Batería completa (1 corrida) | CP-104 commiteada, sola, 20 veces | CP-104 nueva de Codex, sola, 3 veces |
|---|---|---|---|---|
| M-81 | Quita `AND FechaIngreso IS NULL` del `UPDATE` | Sobrevive | Falla 19 de 20 | Falla 3 de 3 |
| M-84 | Leer, esperar 50 ms y escribir sin condición | Muere | (muerta) | Falla 3 de 3 |
| M-85 | `ExecuteNonQuery() == 1` → `>= 0` | Sobrevive | Falla 18 de 20 | Falla 3 de 3 |
| Control sin mutar | — | — | Falla 0 de 20 | Falla 0 de 3 |

- **La CP-104 commiteada** (dos hilos) las detecta casi siempre cuando corre sola, pero no en la
  corrida de la batería completa. Para el número cuentan como no detectadas, porque el criterio
  es una corrida de la batería.
- **La CP-104 nueva de Codex** (diez puertas, `Barrier(10)`, cien rondas; escrita, **sin
  commitear** al 14/11) las detecta las tres en cada corrida. Se probó copiando solo
  `Pruebas/PruebasIngresos.cs` a la copia de trabajo. El control sin mutar no falló.
- **Cuando esté commiteada y verde,** la cifra pasa a **53 de 77 (68,8 %)**. Es la tarea 0 del
  encargo.

## 6. Las 23 defensas sin prueba, por consecuencia

El orden es por lo que pasaría si la defensa faltara: primero los datos personales de otros
clientes, después entrar sin contraseña, después el control de ingreso y al final los datos
operativos. La prueba que hay que escribir para cada una está en el encargo.

| # | ID | Defensa sin prueba | Qué pasaría si faltara |
|---|---|---|---|
| 1 | M-21 | `GestorListaEspera.DeEvento` exige administrador | Un cliente lee nombre y mail de todos los inscriptos de un evento |
| 2 | M-30 | `GestorCompras.PagosDe` exige la compra propia | Un cliente ve los últimos cuatro dígitos y el token de pago de otro |
| 3 | M-28 | `GestorCompras.ObtenerPorId` exige la compra propia | Un cliente lee la compra de otro: importes, evento y estado |
| 4 | M-31 | `GestorCompras.SegurosDe` exige la compra propia | Un cliente ve las pólizas de otro |
| 5 | M-55 | Un hash guardado no sirve como contraseña | Quien obtenga el hash de la base entra con él, y el sistema además lo «migra» |
| 6 | M-79 | El ingreso exige evento publicado | Un evento suspendido sigue admitiendo gente en la puerta |
| 7 | M-83 | El `UPDATE` de ingreso filtra por `ID_Evento` | Si se saltea la lectura previa, una entrada de un evento abre la puerta de otro |
| 8 | M-82 | El `UPDATE` de ingreso exige `Estado = 'Confirmada'` | Una entrada reembolsada entre la lectura y la escritura ingresa igual |
| 9 | M-73 | El ingreso rechaza una entrada pendiente o vencida | El rechazo sale con el motivo genérico «La entrada cambió». Hoy solo se llega por SQL, porque el QR se asigna al confirmar |
| 10 | M-77 | Bordes de la ventana horaria de ingreso | Se rechaza a alguien que llega justo en el borde. No se puede probar sin inyectar la hora |
| 11 | M-80 | El código escaneado se normaliza con `Trim` | Un lector que agrega un espacio rechaza entradas válidas |
| 12 | M-16 | `GestorEventos.EliminarSector` exige administrador | CP-94 acepta cualquier rechazo: la operación falla por RN-06 y la prueba pasa igual |
| 13 | M-19 | `GestorListaEspera.Inscribir` exige el usuario propio | Igual que M-16, con RN-10 |
| 14 | M-20 | `EstaInscripto` exige el usuario propio | Un cliente averigua si otro está en una lista de espera |
| 15 | M-38 | `ComprasEnVentana` exige el usuario propio | Un cliente sabe cuántas compras hizo otro |
| 16 | M-39 | `CalcularDescuento` exige el cliente propio | Un cliente deduce el nivel de fidelidad de otro |
| 17 | M-42 | `GestorPaquetes.Nuevo` exige el usuario propio | Se arma un paquete a nombre de otro |
| 18 | M-18 | `GestorLocales.PorPropietario` exige administrador | Un cliente lista los locales del propietario |
| 19 | M-24 | `GestorIngresos.EventosDeHoy` exige administrador | Un cliente ve qué eventos tienen ingreso hoy |
| 20 | M-47 | `GestorReportes.Ejecutar` exige administrador | Un reporte armado por el administrador se ejecuta desde otra sesión |
| 21 | M-48 | `GestorReportes.EventosEntre` exige administrador | Un cliente usa la consulta de eventos por período de los reportes |
| 22 | M-89 | Índice único `UX_Entradas_CodigoQR` | Dos entradas con el mismo código. Riesgo bajo: el código tiene 128 bits al azar |
| 23 | M-63 | Comparación del hash en tiempo constante | Ataque por tiempo de respuesta. **Una prueba funcional no puede verlo:** se verifica leyendo el código y no se cuenta como prueba |

## 7. Texto para el documento

**Va como subsección nueva «Resistencia de las pruebas de seguridad», después de «Lectura del
cuadro» (p. 108).** Es provisorio hasta que JC confirme los números finales.

> Para comprobar que las pruebas de seguridad detectan efectivamente la falta de cada defensa,
> y no solo que el sistema funciona con las defensas en su lugar, se aplicó una prueba de
> mutación sobre la versión entregada. Se identificaron ochenta y tres defensas: los controles
> de sesión y de perfil de los gestores, la verificación y el almacenamiento de contraseñas,
> el bloqueo por intentos fallidos, el control de ingreso por código QR y las restricciones de
> la base de datos. Cada una se eliminó o alteró por separado, en una copia aislada del código
> y con una base de datos propia, y después se corrió la batería completa de 198 ejecuciones.
> Una mutación se considera detectada cuando falla al menos una prueba que sin la mutación
> pasaba.
>
> Seis mutaciones resultaron equivalentes: el comportamiento no cambia porque otra defensa del
> mismo camino ya rechaza la operación. Se excluyen del cálculo. **De las setenta y siete
> restantes, la batería detectó cincuenta y una (66,2 %).** Las veintiséis no detectadas no son
> vulnerabilidades: la defensa está en el código y funciona. Lo que falta es la prueba que lo
> demuestre. Quedan registradas como trabajo pendiente, ordenadas por la consecuencia que
> tendría la falta de cada defensa; las primeras son las que protegen los datos personales y
> de pago de otros clientes.
>
> La medición no cubre la interfaz gráfica, los accesos que no pasan por la capa de negocio,
> como una conexión directa a la base de datos, ni el tiempo de respuesta de la comparación de
> contraseñas, que una prueba funcional no puede observar y se verificó leyendo el código.

**Variante, solo si Codex commitea la CP-104 nueva y está verde antes de entregar.** Cambiar la
oración en negrita por:

> De las setenta y siete restantes, la batería detectó cincuenta y tres (68,8 %).

…y «Las veintiséis no detectadas» por «Las veinticuatro no detectadas».

## 8. Qué queda afuera de la medición

- **El trabajo sin commitear de Codex al 14/11.** Por ejemplo, el control nuevo de que el
  administrador solo controle el ingreso de sus propios eventos (RN-42 en `GestorIngresos`) y
  `EventosConIngresoAbierto`. Esas defensas no se mutaron.
- **La interfaz.** Las mutaciones son en `Repo/`, `Datos/` y el esquema. Que un botón esté oculto
  no es una defensa: la defensa es el gestor.
- **Un atacante con acceso directo a SQL Server.** Fuera del modelo de amenazas del trabajo.
- **M-63 (tiempo constante)** y **M-77 (bordes)**: la primera no es observable por una prueba
  funcional; la segunda lo será cuando `Registrar` acepte la hora (tarea 10 del encargo).
- **La batería en el árbol principal** no se corrió al cerrar, porque Codex tenía trabajo sin
  commitear sobre `BddEventAura_Pruebas`. La comprobación equivalente se hizo en la copia de
  trabajo ya restaurada: 198/195/3, igual que antes de mutar. **Queda pendiente** correrla en
  el árbol principal cuando Codex no esté trabajando.

## 9. Cierre de la corrida

| Condición | Estado |
|---|---|
| Copia de trabajo fuera del repositorio y nunca commiteada | `D:\mutacion-eventaura`, quitada con `git worktree remove --force` y `prune`. `git worktree list` muestra solo el árbol principal |
| Lo único cambiado en la copia, al final | Los dos `App.config` redirigidos a la base de mutación. Ningún archivo de código quedó mutado |
| Base de mutación | `BddEventAura_Mutacion_Pruebas` borrada. Quedan `BddEventAura` y `BddEventAura_Pruebas`, sin tocar |
| Batería restaurada | 198/195/3, con los mismos tres fallos de base |
| Salidas | `D:\mutacion-salida\` (fuera del repositorio): `resultados.json`, `resultados-fase2.json`, `driver.log`, `fase2.log`, y los `.trx` de cada corrida |
