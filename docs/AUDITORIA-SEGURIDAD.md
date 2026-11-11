# Auditoría de seguridad (solo lectura)

Claude Code, a pedido de JC. **No se modificó código, no se corrió la batería ni se abrió la
aplicación.** Todo sale de leer `Repo/`, `Datos/`, `Visual/`, `BaseDatos/` y `Pruebas/`. Lo
que está en `Pruebas/` puede estar a medio escribir, porque Codex trabaja ahí en paralelo: lo
que dependa de eso dice «a confirmar».

**Gravedad**, según la definición de JC:
- **alta:** alguien puede entrar sin pagar, acceder a datos de otro o escalar privilegios;
- **media** y **baja:** el resto.

**El encargo con los arreglos** está en
[ENCARGO-SEGURIDAD-BORRADOR.md](ENCARGO-SEGURIDAD-BORRADOR.md).

**Un dato de contexto para leer todo lo demás.** EventAura es una aplicación de escritorio
que se conecta directo a SQL Server, con la autenticación de Windows de quien la usa:
- el límite de confianza real es el permiso de ese usuario de Windows sobre la base;
- quien puede abrir la base con esa cuenta no necesita la aplicación;
- por eso, las verificaciones en `Negocio` son una segunda barrera: protegen contra errores
  de la interfaz y contra caminos que la interfaz no previó, no contra alguien con la base
  abierta.

Esto no baja la gravedad de lo que se puede hacer **desde la aplicación**, que es lo que
clasifica esta auditoría.

---

## Resumen

| # | Hallazgo | Gravedad |
|---|---|---|
| H-01 | **El QR no se valida en ningún lado.** No hay control de ingreso: nada rechaza un ticket reembolsado, uno ya usado ni uno inventado. RN-18 promete «válido para un único ingreso» y no está implementado | **Alta** |
| H-02 | **«¿Olvidaste tu contraseña?» cambia la contraseña de cualquier cuenta sabiendo solo el mail**, la del administrador incluida, sin pedir la actual y sin pasar por el bloqueo | **Alta** |
| H-03 | 27 métodos de negocio que cambian datos o devuelven información sensible no verifican quién llama: ni el rol ni que lo que tocan sea suyo. Solo los reportes verifican (RN-34) | Media |
| H-04 | `GestorUsuarios.Registrar` es público y acepta el perfil «Propietario» (Administrador) como parámetro. La interfaz usa `RegistrarCliente`, pero el método que da privilegios está expuesto | Media |
| H-05 | Contraseñas en texto plano en los scripts de datos: la del administrador de la semilla, conocida y escrita en la documentación, y las de 1.178 clientes del juego de reportes. Se migran a hash recién en el primer inicio de sesión | Media |
| H-06 | El inicio de sesión distingue un mail inexistente de uno existente (mensajes distintos); el cambio de contraseña lo dice explícitamente | Baja |
| H-07 | PBKDF2 con 100.000 iteraciones: por debajo de las 600.000 que recomienda OWASP (2023) para HMAC-SHA256 | Baja |
| H-08 | El QR de los reportes PDF se presenta como «de verificación», pero es texto plano sin firma y nada lo verifica | Baja |
| H-09 | La búsqueda de eventos no escapa `%` ni `_` del texto en el `LIKE` (no es inyección: el valor va parametrizado) | Baja |
| H-10 | `Seguridad.Verificar` no atrapa `OverflowException` si las iteraciones guardadas no entran en un `int` | Baja |
| — | **Inyección SQL: no se encontró ninguna** | — |
| — | **Credenciales en el repositorio: no hay contraseñas de SQL ni claves**; solo contraseñas de demostración (pregunta 5) | — |

---

## 1. Integridad del QR

**Qué contiene el QR de un ticket:** el texto `EA-` seguido de 32 caracteres hexadecimales.
- `GeneradorQR.NuevoCodigo`, `Repo/GeneradorQR.cs:29-32`: `"EA-" + Guid.NewGuid().ToString("N")`.
- Se guarda en `Entradas.CodigoQR`, con un índice único (`UX_Entradas_CodigoQR`).
- Se genera recién al confirmar el pago (`GestorCompras.Confirmar`, línea 165).
- No contiene el número de compra, ni el evento, ni ningún otro dato.

| Pregunta | Respuesta |
|---|---|
| **¿Lleva un identificador predecible?** | **No.** Es un GUID versión 4: 122 bits aleatorios. En Windows, `Guid.NewGuid` usa el generador aleatorio criptográfico del sistema. Adivinar un código vigente es impracticable: hay más de 10³⁶ posibles. El número de pedido (`EA-000072`) sí es predecible, pero **no está en el QR** |
| **¿Se valida contra la base al escanearlo?** | **No existe ningún escaneo.** No hay pantalla, método de negocio ni consulta que reciba un código y diga si es válido. El comentario de `GeneradorQR.cs:34` habla de «lo que lee el control de ingreso», pero ese control no existe |
| **¿Se puede usar dos veces? ¿Hay marca de usado?** | **No hay marca de usado.** Los estados de una entrada son Pendiente, Confirmada, Reembolsada y Vencida (`CK_Entradas_Estado`). No hay «usada» ni fecha de ingreso. **RN-18 dice «válido para un único ingreso», y esa mitad de la regla no está implementada ni probada** (CP-32 prueba que el código es único y tiene el formato correcto, no el ingreso) |
| **¿Hay firma o verificación de integridad?** | **No, y no hace falta si se valida contra la base.** El código es un secreto aleatorio que funciona como llave: lo que lo hace seguro es que el control de ingreso pregunte a la base. Una firma (HMAC) solo serviría para validar sin conexión, que no es este caso |

**La respuesta clara:**
- **El código está bien hecho:** nadie puede fabricar uno válido.
- **El problema es que nadie pregunta si es válido.** Hoy, en la puerta:
  - **un ticket reembolsado sigue sirviendo.** Un cliente con seguro compra, devuelve la
    entrada antes de las 48 horas (RN-29), cobra el reintegro y conserva el PDF;
  - **un ticket sirve para varias personas.** Basta con reenviar el PDF: no hay marca de usado;
  - **un QR inventado no se puede rechazar.** Cualquier texto `EA-…` con forma de código no
    tiene contra qué compararse.

**Gravedad alta: es exactamente «entrar sin pagar».** La corrección es un **control de
ingreso**:
1. un administrador escanea, con un lector USB de QR, que en Windows escribe el código como
   un teclado;
2. el sistema verifica contra la base que la entrada exista, esté Confirmada, sea de ese
   evento y no se haya usado;
3. la marca como usada en una sola operación atómica.

Es el ítem SEG-2 del encargo.

---

## 2. Autorización del lado del negocio

**Cómo funciona hoy.**
- **La identidad de quien usa la aplicación:**
  - la guarda `GestorSesion.UsuarioActual` al iniciar sesión;
  - las pantallas no la usan: se pasan entre sí un campo público `IDUsuario`, y lo entregan a
    los métodos de negocio como parámetro;
  - **el negocio confía en ese parámetro.**
- **Los únicos métodos que miran la sesión** son los de `GestorReportes` (RN-34).

**Desde la interfaz actual no hay un camino para usar estos métodos con el usuario o el rol
equivocados.** Las pantallas del administrador solo se abren desde su menú, y cada pantalla
recibe el `IDUsuario` del inicio de sesión. Por eso es **media**: es el mismo tipo de agujero
que el del perfil Propietario, y basta con que una pantalla nueva o un cambio se equivoque.

### Lista completa: métodos que cambian datos o devuelven información sensible

| Archivo | Método | Qué hace | ¿Verifica quién llama? |
|---|---|---|---|
| `Repo/GestorReportes.cs` | `Crear`, `Ejecutar`, `Exportar` (por `Ejecutar`), `EventosEntre` | Reportes con datos de ventas y clientes | **Sí: exige Administrador** (`VerificarAdministrador`, RN-34) |
| `Repo/GestorUsuarios.cs` | `CambiarContrasena(mail, nueva)` | Cambia la contraseña de cualquier cuenta | **No** (H-02, alta) |
| `Repo/GestorUsuarios.cs` | `Registrar(…, tipoUsuario)` | Crea una cuenta con el perfil que se le pase, también Administrador | **No** (H-04) |
| `Repo/GestorEventos.cs` | `Crear` | Crea un evento | **No**: ni rol ni dueño del local |
| `Repo/GestorEventos.cs` | `Publicar` | Publica un evento | **No** |
| `Repo/GestorEventos.cs` | `Eliminar` | Elimina un evento | **No** |
| `Repo/GestorEventos.cs` | `Suspender` | Suspende un evento | **No** |
| `Repo/GestorEventos.cs` | `CambiarPreventa` | Cambia la preventa | **No** |
| `Repo/GestorEventos.cs` | `AgregarSector` | Agrega un sector | **No** |
| `Repo/GestorEventos.cs` | `EliminarSector` | Elimina un sector | **No** |
| `Repo/GestorLocales.cs` | `Registrar` | Crea un local para el propietario que diga el objeto | **No** |
| `Repo/GestorLocales.cs` | `PorPropietario(idPropietario)` | Locales de cualquier propietario | **No** |
| `Repo/GestorListaEspera.cs` | `DeEvento` | Nombre y mail de los clientes en espera (datos personales) | **No**: debería exigir Administrador |
| `Repo/GestorListaEspera.cs` | `Inscribir(idEvento, idUsuario)` | Inscribe a cualquier usuario | **No**: no verifica que `idUsuario` sea el de la sesión |
| `Repo/GestorCompras.cs` | `Iniciar(p, c)` | Reserva cupo a nombre del cliente que reciba | **No** |
| `Repo/GestorCompras.cs` | `Confirmar(c, t)` | Paga una compra pendiente de cualquiera | **No** (impacto bajo: pagar por otro) |
| `Repo/GestorCompras.cs` | `Liberar(c)` | Libera la reserva de cualquier compra | **No** |
| `Repo/GestorCompras.cs` | `Reembolsar(entradas)` | Devuelve dinero por entradas de cualquier compra | **No**: no verifica que las entradas sean del usuario de la sesión |
| `Repo/GestorCompras.cs` | `HistorialDe(idUsuario)` | Historial de compras de cualquier usuario | **No** |
| `Repo/GestorCompras.cs` | `ObtenerPorId(idCompra)` | Cualquier compra | **No** |
| `Repo/GestorCompras.cs` | `EntradasDe(c)` | Entradas de una compra **con sus códigos QR**, que son la llave de ingreso | **No** |
| `Repo/GestorCompras.cs` | `PagosDe(c)` | Pagos: últimos 4 dígitos y token | **No** |
| `Repo/GestorCompras.cs` | `SegurosDe(c)` | Pólizas de una compra | **No** |
| `Repo/GestorSuscripciones.cs` | `Contratar(idUsuario, plan)` | Contrata a nombre de cualquiera | **No** |
| `Repo/GestorSuscripciones.cs` | `Cancelar(idUsuario)` | Cancela la suscripción de cualquiera | **No** |
| `Repo/GestorSuscripciones.cs` | `ActivaDe(idUsuario)` | Suscripción de cualquiera | **No** |
| `Repo/GestorPaquetes.cs` | `Nuevo(idEvento, idUsuario)` | Arma un paquete a nombre de cualquiera (el nivel y la preventa son los de ese usuario) | **No** |
| `Repo/GestorFidelizacion.cs` | `ObtenerCliente(idUsuario)` | Datos y nivel de cualquier cliente | **No** |
| `Repo/GestorFidelizacion.cs` | `ReevaluarNivel(c, fecha)` | Escribe el nivel de cualquier cliente | **No** (es interno del flujo de compra) |
| `Repo/GestorFidelizacion.cs` | `AscenderSiCorresponde(c, fecha)` | Escribe el nivel de cualquier cliente | **No** (ídem) |
| `Repo/GestorCompras.cs` | `LiberarVencidas()` | Vence reservas de todos | No verifica, y está bien: no depende de quién llame y es idempotente |
| `Repo/GestorUsuarios.cs` | `Autenticar`, `RegistrarCliente` | Inicio de sesión y alta pública | No corresponde: son los puntos de entrada públicos |

**Total: 29 métodos sin verificación que deberían tenerla:**
- 27 en siete gestores: Eventos 7, Compras 9, Suscripciones 3, Fidelización 3, Locales 2,
  Lista de espera 2 y Paquetes 1;
- `CambiarContrasena` y `Registrar`, de `GestorUsuarios`. Cuatro verifican, todos en reportes. Las consultas de
catálogo (`Vigentes`, `Buscar`, `Sectores`, `ServiciosDe`, `DisponiblesDe`, `PrecioDe`,
`ObtenerPorId` de eventos) son públicas a propósito y no se cuentan.

---

## 3. Inyección SQL

**No se encontró ninguna consulta armada con datos del usuario.**
- **Cómo se buscó:** en `Datos/` se revisaron todas las concatenaciones e interpolaciones
  junto a texto SQL. Hay 158 llamadas a `AgregarParametro`/`AddWithValue`.
- **Las tres concatenaciones que existen pegan solo constantes del código:**

| Archivo:línea | Qué concatena | Riesgo |
|---|---|---|
| `Datos/RepositorioCompras.cs:112` y `:141` | La constante `ColumnasCompra` (lista de columnas) | Ninguno |
| `Datos/RepositorioCompras.cs:146` | Los **nombres** de parámetros `@Estado0`, `@Estado1`…; los valores van como parámetros | Ninguno |
| `Datos/RepositorioEventos.cs:149-151` | `CondicionAgotado(idEvento)`, que es `internal` y se llama solo con las constantes `"@ID"` (`:135`) y `"e.ID"` (`RepositorioReportes.cs:175`) | Ninguno hoy. Si alguien la llamara con un valor del usuario, sí. Conviene un comentario que lo prohíba |

**Fuera de `Datos/`:** `Pruebas/InvestigacionPreventaReal.cs:173-178` concatena una lista de
IDs dentro de un `IN (…)`. Es código de prueba, sobre la base de pruebas y con IDs que genera
la propia prueba. No se entrega en la aplicación. **A confirmar cuando Codex termine**: el
archivo es de la investigación que está haciendo ahora.

---

## 4. Contraseñas

**El algoritmo** (`Repo/Seguridad.cs`):

| Aspecto | Valor |
|---|---|
| Algoritmo | PBKDF2 con HMAC-SHA256 (RFC 2898), implementado a mano porque en netstandard2.0 `Rfc2898DeriveBytes` solo da SHA-1 |
| Sal | 16 bytes aleatorios por contraseña, de `RandomNumberGenerator` (criptográfico) |
| Iteraciones | 100.000 (`Seguridad.cs:14`). OWASP recomienda hoy 600.000 para PBKDF2-HMAC-SHA256 (H-07) |
| Salida | 32 bytes. Se guarda como `PBKDF2$iteraciones$sal$hash`, así se pueden subir las iteraciones sin invalidar las viejas |
| Comparación | En tiempo constante (`SonIguales`) |
| Política | RN-02: 8 caracteres como mínimo, con al menos una letra y un dígito (parámetro `PWD_LONGITUD_MIN`) |

**La migración desde texto plano** (`GestorUsuarios.ContrasenaValida`, líneas 141-157):
- **Cómo funciona:** si lo guardado no es un hash y coincide con lo ingresado, se reemplaza
  por su hash y se deja entrar. Está bien protegida: si lo guardado ya es un hash, no acepta
  el hash como contraseña.
- **¿Está completa? Como mecanismo, sí:**
  - toda alta (`Registrar`) y todo cambio (`CambiarContrasena`) guardan un hash;
  - la migración solo convierte cuentas cuando alguien inicia sesión.
- **Lo que queda en texto plano** (H-05, media). No se copia ningún valor:
  - `BaseDatos/02-semilla.sql`, líneas 42, 47, 51 y 55: las cuatro cuentas de la semilla, a
    propósito (comentario de las líneas 6 y 7). **La del administrador es conocida:** figura en
    `docs/` y en las herramientas. Cualquiera que lea la documentación entra como
    administrador en una base recién creada;
  - `BaseDatos/03-datos-reporte.sql`, línea 140: una misma contraseña en texto plano para los
    1.178 clientes del juego de reportes;
  - `Pruebas/Fake.cs`, líneas 103 a 105: los clientes de prueba. Solo en la base de pruebas,
    que se recrea en cada corrida, así que no es un riesgo de la aplicación.
- **No hay forma segura de cambiar la contraseña por defecto.** El único camino es el de H-02.

---

## 5. Credenciales en el repositorio

Se buscó en todos los archivos que se entregan (sin `bin`, `obj`, `.git` ni `TestResults`):
contraseñas, cadenas de conexión, usuarios de SQL, claves y tokens. **No se copia ningún
valor.**

| Archivo | Línea | Qué hay | Riesgo |
|---|---|---|---|
| `Visual/App.config` | 6 | Cadena de conexión con **autenticación de Windows**: servidor y base, **sin usuario ni contraseña** | Ninguno: no es un secreto. Revela el nombre de la instancia de una PC |
| `Pruebas/App.config` | 6 | Ídem | Ídem |
| `Visual/Properties/Settings.settings` | 8, 11 | Ídem, valor por omisión | Ídem |
| `Visual/Properties/Settings.Designer.cs` | 29 | Ídem, generado | Ídem |
| `README.md` | 31 | Una cadena de conexión de ejemplo, con autenticación de Windows | Ninguno |
| `BaseDatos/02-semilla.sql` | 42, 47, 51, 55 | Contraseñas de demostración en texto plano | H-05 |
| `BaseDatos/03-datos-reporte.sql` | 140 | Contraseña de demostración en texto plano | H-05 |
| `tools/Excepciones/ErrorNoControlado.cs` | 31 | Contraseña de demostración (herramienta de verificación) | Baja: es la de la semilla |
| `tools/Excepciones/VerificarBaseCaida.ps1` | 100 | Ídem | Baja |
| `tools/Usabilidad/Medicion.cs` | 186 | Ídem | Baja |
| `tools/Usabilidad/VerificarCierreConX.ps1` | 42 | Ídem | Baja |
| `docs/` (varios) y `Pruebas/` (varios) | — | Las contraseñas de demostración, para las pruebas manuales y automáticas | Baja |

**No hay contraseñas de SQL Server, claves de API, certificados ni tokens reales.** Dos
coincidencias de la búsqueda eran falsos positivos: `SistemaEntradas.sln:91` (un GUID) y
`Repo/GestorUsuarios.cs:131` (una comparación con `null`).

---

## 6. Intentos fallidos y bloqueo de cuenta

| | Estado |
|---|---|
| **Qué existe** | RN-04 (`GestorUsuarios.RegistrarIntentoFallido`): `LOGIN_INTENTOS_MAX` fallos consecutivos (5) bloquean `LOGIN_BLOQUEO_MIN` minutos (15). Los dos son parámetros. Un inicio de sesión correcto pone el contador en cero. Un bloqueo vencido vuelve a contar desde uno. Los mensajes dicen los intentos restantes y los minutos que faltan |
| **Qué está probado** | CP-28, en los gestores: avisos, bloqueo, rechazo de la contraseña correcta durante el bloqueo, y desbloqueo al vencer. CP-68 (PA-03), en la pantalla real: los cinco avisos y los minutos que bajan. **Está bien cubierto** |
| **Qué falta** | (a) **El cambio de contraseña no pasa por el bloqueo** (H-02): con la cuenta bloqueada, igual se le puede cambiar la contraseña. (b) Cualquiera puede bloquear la cuenta del administrador con 5 intentos: es el costo de todo bloqueo por cuenta, aceptable en este alcance (baja, sin acción). (c) El mensaje de intentos restantes revela que el mail existe (H-06): lo pide RN-04 y lo prueba C.2, así que **se acepta y se documenta**, no se cambia |

---

## 7. Las reglas de seguridad entre las 38

| Regla | Qué protege | Caso que la prueba | ¿Cubierta? |
|---|---|---|---|
| RN-01 | Identidad: un correo, una cuenta | CP-26 | Sí |
| RN-02 | Contraseñas fuertes | CP-15 | Sí |
| RN-04 | Fuerza bruta: bloqueo por intentos | CP-28, CP-68 | Sí |
| RN-05 | Perfiles excluyentes | CP-55 (perfil inválido); CP-44 y CP-45 (el alta pública crea clientes) | Sí, pero el método que asigna el perfil está expuesto (H-04) |
| RN-17 | No se confirma sin pago aprobado | CP-32, CP-33 | Sí |
| **RN-18** | **Un QR único, válido para un único ingreso** | CP-32 (único y con formato) | **Solo la mitad.** «Válido para un único ingreso» **no está implementado ni probado** (H-01) |
| RN-30 | No se guarda la tarjeta completa | CP-32 | Sí |
| RN-31 | Todo pago queda registrado (auditoría) | CP-32, CP-33 | Sí |
| RN-34 | Reportes solo para el Administrador | CP-35 | Sí |

**Además, faltan reglas.** Ninguna de las 38 dice:
- que **cambiar la contraseña exige estar autenticado** (H-02);
- que **cada usuario opera solo sobre lo suyo**: sus compras, devoluciones y suscripción (H-03);
- que **la gestión de eventos, locales y listas de espera es del Administrador**: RN-34 cubre
  solo los reportes (H-03).

**Propuesta:** RN-40 a RN-42, cada una con su caso, cuando se retome el documento (hoy en
pausa por decisión de JC). No las agregué a `02-reglas-negocio.md`.

---

## Otras cosas vistas, fuera de las siete preguntas

`frmCambiarContraseña`:
- arranca con los campos ya completados («A», «B», «B»; `frmCambiarContraseña.cs:24-26`);
- cuando falta un dato, muestra el mensaje «Dale, son 3 cositos nomas cargalos viejo»
  (`:60`).

No es de seguridad, pero es texto visible y se reescribe con SEG-1.
