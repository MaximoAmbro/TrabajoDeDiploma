# Encargo de seguridad — BORRADOR

Claude Code, a pedido de JC. Sale de [AUDITORIA-SEGURIDAD.md](AUDITORIA-SEGURIDAD.md). **No
es un encargo vigente:** pasa a `TAREAS-CODEX-PRUEBAS.md` cuando JC lo apruebe y Codex no
esté trabajando. Ordenado por gravedad. Todas las horas son estimadas, de Codex.

**Decisiones previas de JC** (sin ellas, SEG-2 y SEG-5 no arrancan):
- **D-1. Autorizar cambios en `BaseDatos/`:** una columna nueva en `Entradas` (SEG-2) y las
  contraseñas de los scripts guardadas como hash (SEG-5).
- **D-2. Qué hace «¿Olvidaste tu contraseña?»** (SEG-1). Sin envío de correo no se puede
  recuperar una contraseña olvidada de forma segura. Recomiendo:
  - el enlace pasa a **«Cambiar contraseña»** y pide el mail, la contraseña actual y la nueva;
  - una contraseña olvidada la restablece el Administrador (fuera de este encargo; se documenta
    como limitación).
- **D-3. Cuándo va SEG-3** respecto de 6B (ver la sección final).

**Reglas:**
- las de siempre (zona de Codex, batería al terminar cada tarea, sin commit ni push);
- **ninguna prueba existente se debilita**;
- las que cambian a propósito se declaran en el reporte con el motivo;
- **el reporte de cada tarea trae las horas reales**, como en 6B.

---

## Alta

### SEG-1. Cambiar la contraseña exige la contraseña actual (H-02) — 3 a 4 h

**Negocio:**
- **la firma nueva:** `GestorUsuarios.CambiarContrasena(string mail, string actual, string
  nueva)`;
- **el paso previo:** se autentica a la cuenta con el mismo camino que `Autenticar`;
- **qué cuenta como fallo:** una contraseña actual incorrecta **cuenta como intento fallido**;
- **con la cuenta bloqueada,** rechaza con el mismo mensaje de RN-04;
- **si el mail no existe,** responde lo mismo que con una contraseña incorrecta, para no
  revelar qué mails existen;
- **la firma de dos parámetros desaparece.**

**Pantalla** (`frmCambiarContraseña`):
- un campo «Contraseña actual»;
- sin los valores precargados «A», «B», «B»;
- los mensajes con el estilo de V-6 (`Avisos.Regla` y títulos en castellano), sin el «Dale, son
  3 cositos…»;
- en `frmInicio`, el enlace dice «Cambiar contraseña» (D-2).

**Si 6B ya hizo T-38 sobre estas pantallas, se trabaja sobre lo nuevo;** si no, T-38 la toma
con este cambio ya hecho.

**Casos nuevos (4):**
- sin la contraseña actual correcta no cambia, y suma un intento;
- con la cuenta bloqueada no cambia;
- con un mail inexistente, el mensaje es igual al de contraseña incorrecta;
- con todo correcto cambia, y la vieja deja de servir.

**Rompe pruebas:** ninguna. Ninguna prueba actual llama a `CambiarContrasena` ni abre
`frmCambiarContraseña` (a confirmar cuando Codex termine la tanda en curso).

### SEG-2. Control de ingreso: el QR vale para un único ingreso (H-01, RN-18) — 8 a 12 h

**Base** (D-1):
- `Entradas.FechaIngreso DATETIME NULL` en `01-esquema.sql`, con el `ALTER` para bases
  existentes en el bloque de preparación de `docs/PRUEBAS-PENDIENTES.md`;
- **no se agrega un estado «Usada»:** el estado de la entrada no cambia. Así RPT-01, los
  disponibles y las devoluciones siguen contando igual.

**Datos:**
- `RepositorioEntradas.RegistrarIngreso(codigo, idEvento, ahora)` en **una sola sentencia
  atómica:**

  ```
  UPDATE … SET FechaIngreso = @Ahora
  WHERE CodigoQR = @Codigo AND ID_Evento = @IdEvento
    AND Estado = 'Confirmada' AND FechaIngreso IS NULL
  ```

  Si no actualizó ninguna fila, una consulta aparte dice por qué;
- **dos escaneos simultáneos no pueden entrar los dos.**

**Negocio:**
- `GestorIngresos` (Singleton), con `Registrar(string codigo, int idEvento)`;
- **exige un Administrador en la sesión** desde el principio (no repetir H-03);
- **devuelve un resultado con el motivo:** válido; ya usado, con la fecha y hora del primer
  ingreso; reembolsado; vencido o pendiente; de otro evento; inexistente; o fuera del horario;
- **el horario de ingreso:** desde `INGRESO_HORAS_ANTES` horas antes del evento hasta
  `INGRESO_HORAS_DESPUES` después, dos parámetros nuevos en `Parametros`. Valores propuestos:
  6 y 12.

**Pantalla nueva** `frmControlIngreso`, para el Administrador, desde su menú:
- un desplegable con los eventos del día;
- un campo donde el lector USB de QR escribe el código y Enter, como un teclado;
- un resultado grande, en verde (`Exito`) o rojo (`Error`), con el motivo;
- el contador de ingresos del evento;
- se construye con el enfoque del Designer de 6B: geometría literal, sin fuentes ni colores,
  con los componentes.

**Casos nuevos (6):**
- una entrada válida ingresa;
- la misma, por segunda vez, se rechaza con la hora del primer ingreso;
- una reembolsada se rechaza;
- un código inventado se rechaza;
- una de otro evento se rechaza;
- **dos ingresos simultáneos del mismo código: entra uno solo.**

**Rompe pruebas:** ninguna. La columna nueva es anulable y no cambia los estados. **Verificar
en el reporte** que CP-32 a CP-34 y los reportes (CP-36 y CP-37) siguen pasando.

**Prueba manual para JC** (5 min): escanear con el celular un ticket a la pantalla de control.
Con un lector USB, si lo hay; si no, copiando el código del PDF.

---

## Media

### SEG-3. El negocio verifica quién llama (H-03) — 8 a 12 h

**En `GestorSesion`:**
- `RequerirAdministrador()`: lanza RN-34 generalizada, con el texto «Esta operación es solo
  para el Administrador»;
- `RequerirUsuario(int idUsuario)`: lanza si el `idUsuario` recibido no es el de la sesión,
  salvo que la sesión sea de un Administrador.

**Dónde se aplica:** en los 27 métodos de la tabla de la auditoría, sección 2.
- **Administrador:** todo `GestorEventos` que cambia datos, `GestorLocales.Registrar` y
  `PorPropietario`, y `GestorListaEspera.DeEvento`;
- **el propio usuario:** compras, reembolso, historial, entradas, pagos, pólizas,
  suscripción, inscripción a la lista de espera, paquete y cliente.

**Las pantallas no cambian:** siguen pasando `IDUsuario`, y el negocio verifica que coincida
con la sesión. Así no se toca ninguna de las pantallas que rediseña 6B.

**`Reembolsar`** verifica que la compra sea del usuario de la sesión.

**Casos nuevos (4):**
- un cliente no puede reembolsar una compra de otro;
- un cliente no puede ver el historial de otro;
- un cliente no puede eliminar un evento;
- sin sesión, ninguna operación de negocio que cambia datos se ejecuta.

**Rompe pruebas: sí, y muchas. Es la única tarea de este encargo que lo hace.**
- **Por qué:** la mayoría de las pruebas llaman a los gestores o abren pantallas sin iniciar
  sesión. Medido el día de la auditoría, en solo lectura, puede haber cambiado:
  - `PruebasInterfaz.cs`: 27 pruebas, 20 asignaciones de `IDUsuario` a pantallas, 1 inicio de
    sesión;
  - `PruebasCompra.cs`: 6 pruebas y 24 llamadas;
  - `PruebasRechazosNegocio.cs`: 7 y 14;
  - `PruebasFidelizacion.cs`: 8 y 10;
  - las del grupo A (Propietario, Suscripción, Preventa, Listas y menús): 17 pruebas en total;
  - y `Fake.cs`, que confirma compras a través de `GestorCompras.Iniciar` y `Confirmar`.
- **Cómo se resuelve sin debilitar nada:**
  - `Fake.ComoCliente(c)` y `Fake.ComoAdministrador()` inician la sesión correspondiente;
  - cada prueba que actúa en nombre de alguien lo llama en su preparación;
  - las aserciones no cambian.
- **Es mecánico pero amplio:** de 4 a 6 de las 8 a 12 horas son eso.

**Por qué la estimación es amplia:** si alguna prueba resulta depender de no tener sesión
(por ejemplo, CP-35 prueba justamente «sin sesión»), hay que leerla una por una.

### SEG-4. El perfil Administrador no se asigna desde un método público (H-04) — 1 h

- **El cambio:** `GestorUsuarios.Registrar(…, tipoUsuario)` pasa a `internal`, con
  `InternalsVisibleTo("Pruebas")`.
- **Qué queda público:** `RegistrarCliente`, para el alta, y un
  `CrearAdministrador(…)` que exige un Administrador en la sesión. Ninguna pantalla lo usa
  hoy.
- **Rompe pruebas:** no. `Fake.cs:175` y CP-55 siguen compilando gracias a
  `InternalsVisibleTo`.

### SEG-5. Ninguna contraseña en texto plano en los scripts (H-05) — 1 a 2 h

- **Qué cambia en los scripts** (D-1):
  - `02-semilla.sql`: las cuatro contraseñas, guardadas como hash PBKDF2;
  - `03-datos-reporte.sql`: la de los 1.178 clientes, como un único hash;
  - los hashes se generan con `Seguridad.Hash`, desde una herramienta que Codex deja en el
    reporte para regenerarlos.
- **Qué no cambia:** las credenciales de demostración siguen siendo las mismas para iniciar
  sesión.
- **El administrador puede cambiar la suya** con SEG-1.
- **Rompe pruebas:** probablemente no, porque el login verifica contra el hash igual.
  - **A confirmar con Codex:** la variante de migración de PA-02, si usa una cuenta de la
    semilla en texto plano. Si es así, que use una cuenta propia creada con `Fake`, que
    conserva el texto plano solo en la base de pruebas.

---

## Baja

| # | Qué | Horas | ¿Rompe pruebas? |
|---|---|---|---|
| SEG-6 | PBKDF2 a 600.000 iteraciones para los hashes nuevos. Los viejos siguen valiendo porque llevan sus iteraciones, y se re-hashean al iniciar sesión. **Medir antes** cuánto tarda un inicio de sesión con la implementación a mano: si pasa de 1 s, proponer 310.000 y avisar | 1 | No, pero la batería se hace más lenta (cada alta y cada inicio de sesión cuestan más) |
| SEG-7 | Escapar `[`, `%` y `_` del texto de búsqueda antes del `LIKE` (`RepositorioEventos.cs:98-99`) | 0,5 | No (CP-30 busca palabras comunes) |
| SEG-8 | `Seguridad.Verificar` atrapa también `OverflowException` | 0,25 | No |
| SEG-9 | El QR de los reportes: no se cambia el código. Es una corrección de texto del documento («código QR de identificación», no «de verificación»), cuando se retome | 0 | — |
| SEG-10 | Un comentario en `RepositorioEventos.CondicionAgotado` que prohíba pasarle un valor del usuario | 0,1 | No |

**H-06 (enumeración de mails por el mensaje de intentos restantes) no se corrige:** RN-04 y
la prueba C.2 piden ese mensaje. Se documenta como riesgo aceptado. La otra fuente de
enumeración, el cambio de contraseña, la elimina SEG-1.

---

## Resumen

| Tarea | Gravedad | Horas | Rompe pruebas |
|---|---|---|---|
| SEG-1 | Alta | 3 a 4 | No |
| SEG-2 | Alta | 8 a 12 | No |
| SEG-3 | Media | 8 a 12 | **Sí: la mayoría de las pruebas de interfaz y de negocio; se arreglan con la sesión en la preparación** |
| SEG-4 | Media | 1 | No |
| SEG-5 | Media | 1 a 2 | A confirmar: la variante de migración de PA-02 |
| SEG-6 a SEG-10 | Baja | 2 | No |
| **Total** | | **23 a 33** | |

**Casos nuevos:** 14 (SEG-1: 4; SEG-2: 6; SEG-3: 4).

**Reglas nuevas para el documento**, cuando se retome:
- RN-40: cambiar la contraseña exige autenticarse;
- RN-41: cada usuario opera solo sobre lo suyo;
- RN-42: la gestión de eventos, locales, listas de espera e ingreso es del Administrador.

**RN-18 queda completa con SEG-2.**

## Dónde encaja en el plan

Está en `PLAN-RENOVACION-VISUAL.md`, sección 0.1, bloque «Seguridad».
- **SEG-1, SEG-2, SEG-4 y SEG-5 van antes de 6B** (antes de T-41), después de las pruebas en
  curso (T-45 y T-46). Son las dos altas y dos baratas. JC lo pidió así para el QR, y no
  chocan con 6B: tocan `Repo/`, `Datos/`, `BaseDatos/`, una pantalla nueva y
  `frmCambiarContraseña` antes de que 6B la rediseñe.
- **SEG-3 es una decisión de JC (D-3):**
  - **Antes de 6B (lo recomiendo).** Todas las pruebas nuevas de 6B ya nacen iniciando sesión
    y no hay que reescribirlas después. Cuesta correr el rediseño unas 8 a 12 h.
  - **Después de T-40 y antes de las verificaciones finales.** El rediseño llega antes, pero
    las pruebas que agregue 6B (unas 60 filas) también hay que adaptarlas. Suma 2 a 3 h.
- **SEG-6 a SEG-10:** en cualquier hueco. Ninguna depende de otra.
