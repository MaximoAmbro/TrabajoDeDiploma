# Auditoría de riesgos del día de la defensa

Claude Code, 24/11/2026, a pedido de JC.

**El escenario:**
- una PC que no es la de JC, posiblemente sin internet;
- una copia de la carpeta del programa y una base recién armada;
- veinte minutos para mostrar el sistema.

La pregunta es qué falla en esas condiciones, no en el laboratorio.

**Cómo se probó.** Sobre una copia de `Visual\bin\Debug` (`1d05fc2` más lo que Codex tiene sin
commitear, compilado), en el scratchpad. Se usaron cuatro bases nuevas en `.\SQLEXPRESS`, creadas
para esto y borradas al final. **La base de desarrollo de JC no se tocó.** Cada caso se ejecutó
sobre el `Visual.exe` real, con capturas de pantalla y el registro
`%LOCALAPPDATA%\EventAura\errores.log`. Evidencia: `docs/verificaciones/defensa-2026-11-24/`.

**Cómo leer la columna «Base»:**

| Marca | Qué significa |
|---|---|
| **EJECUTADO** | Se hizo y se miró el resultado |
| **CÓDIGO** | Se leyó el código y el comportamiento es directo, pero no se ejecutó |
| **DEDUCIDO** | Depende de la PC o de condiciones que no se pudieron reproducir |

---

## Lo más grave

| # | Qué pasa | Base | Ticket |
|---|---|---|---|
| R-01 | **Si la aplicación no llega a la base, la contraseña se escribe a la vista.** `frmInicio_Load` se corta en la verificación del esquema, antes de poner `PasswordChar`. En un proyector, la contraseña queda en pantalla para el jurado | **EJECUTADO** | T-20 |
| R-02 | **Sin las DLL de WebView2 o sin `Newtonsoft.Json.dll`, aparece un error en lugar del respaldo.** Sin WebView2, el panel del Administrador queda sin dibujar | **EJECUTADO** (H-2 y este informe) | T-03 |
| R-03 | **La semilla de demostración solo corre en una base cuyo nombre termina en `_Demostracion`** (error 51000). Si la base de la defensa se llama `BddEventAura`, no carga: la cadena de conexión tiene que apuntar a la base `…_Demostracion` | **EJECUTADO** | Hoja de la defensa |

---

## 1. SQL Server y la cadena de conexión

| # | Caso | Qué ve el usuario, exactamente | Base | Qué hacer |
|---|---|---|---|---|
| 1.1 | La instancia no existe (SQL Server no instalado, otro nombre o servicio detenido) | La ventana tarda **15 s** en aparecer (el tiempo de espera de la conexión). Aparece la pantalla de inicio normal, **sin ningún mensaje**, y **la contraseña se ve en texto plano**. Al tocar «Iniciar sesión», tras otra espera, sale «No se pudo conectar con la base de datos. Verificá que SQL Server esté en funcionamiento…» durante 12 s | **EJECUTADO** con `.\NOEXISTE`. «No instalado» y «detenido» dan la misma excepción (**DEDUCIDO**; la base detenida ya se había probado el 05/11, CX-04) | T-20: ocultar la contraseña desde el Designer y mostrar en la pantalla de inicio un aviso fijo con la instancia y la base que se intentaron |
| 1.2 | La instancia existe pero la base no | Igual que 1.1, con 10 s de espera | **EJECUTADO** | Ídem |
| 1.3 | La base existe pero vacía (no se corrió `01`) | Igual que 1.1, sin espera. El cartel de «base desactualizada» **no** aparece: solo controla de la migración 04 en adelante | **EJECUTADO** | Ídem |
| 1.4 | El usuario de Windows no tiene permiso en SQL Server (autenticación integrada) | Igual que 1.2: la excepción es la misma | **DEDUCIDO** | En la hoja: correr un `SELECT` en la base con ese usuario antes de abrir la aplicación |
| 1.5 | Falta `Visual.exe.config` | La pantalla de inicio sale sin mensaje y con la contraseña a la vista. Al entrar, «Ocurrió un error inesperado…», no el mensaje de la base | **EJECUTADO** | Llevar la carpeta entera |
| 1.6 | La cadena apunta a la PC de Máximo (el `App.config` del repositorio dice `DESKTOP-5QSNMRE\AMBROMSSQLSERVER`) | Como 1.1 | **CÓDIGO** | En la hoja: editar `Visual.exe.config` con el nombre de la instancia de la PC de la defensa |
| 1.7 | SQL Server 2016 o anterior | El reporte RPT-02 usa `STRING_AGG` (SQL Server 2017 o posterior). El panel del Administrador corre los cinco reportes al entrar, así que fallaría ahí con el mensaje de la base | **CÓDIGO**. Solo está probado sobre 2022 (16.0.1000.6) | En la hoja: SQL Server 2017 o posterior, o LocalDB 2017 o posterior con `(localdb)\MSSQLLocalDB` |

## 2. La base

| # | Caso | Resultado | Base | Qué hacer |
|---|---|---|---|---|
| 2.1 | Base nueva con `01`, `02` y `03` | Completa: no aparece el cartel, entran el Administrador (panel con la vista web) y Paula. **`01-esquema.sql` ya trae todo lo de 04 a 16**: esas migraciones solo hacen falta para actualizar una base vieja | **EJECUTADO** | — |
| 2.2 | Correr 04 a 16 encima de una base nueva | Los trece corren sin error | **EJECUTADO** | — |
| 2.3 | Base vieja con las migraciones salteadas | La aplicación muestra «Tu base de datos está desactualizada: falta… Corré BaseDatos/0x-….sql», siempre la primera que falta | Mensaje: **CÓDIGO**. Lo que hace cada script si falta el anterior: **DEDUCIDO**, sin una base vieja con la que probarlo | En la defensa, base nueva (2.1): elimina el caso |
| 2.4 | Esquema sin `02-semilla.sql` (sin cuentas ni parámetros) | Cartel «falta el parámetro INGRESO_HORAS_ANTES. Corré 04-seguridad-ingreso.sql». **Engaña:** lo que falta es `02`, y con `04` no se arregla | **EJECUTADO** | T-21: comprobar primero que haya parámetros y un Administrador, y pedir `02` |
| 2.5 | La semilla de demostración sobre una base cuyo nombre no termina en `_Demostracion` | Se detiene con el error 51000 sin tocar nada | **EJECUTADO** | Crear la base con ese nombre y apuntar la cadena a ella |
| 2.6 | La semilla de demostración dos veces | La segunda no inserta nada. Después de las dos corridas hay 23 eventos (3 de `02` y 20 de la semilla) y 343 entradas (6 y 337): lo mismo que deja una sola | **EJECUTADO** | — |
| 2.7 | La semilla de demostración sobre una base que ya tiene los datos de `03` (1183 usuarios) | Carga bien: 28 eventos y 4823 entradas en total | **EJECUTADO** | Si se quiere un panel prolijo, no correr `03` en la base de la defensa |
| 2.8 | Las fechas de la demostración | Son relativas al día en que se corre la semilla (eventos hasta cuatro meses adelante) | **CÓDIGO** | Correrla en la semana de la defensa |

## 3. .NET Framework y otras dependencias

| # | Caso | Resultado | Base | Qué hacer |
|---|---|---|---|---|
| 3.1 | .NET Framework 4.8 | Viene con Windows 10 1903 o posterior y con Windows 11. En esta PC está la versión 534296 (4.8.1). Si faltara, Windows ofrece descargarlo, y **sin internet no se puede** | **DEDUCIDO** | En la hoja: comprobar `Release` ≥ 528040 en el registro |
| 3.2 | WebView2 Runtime | Viene con Windows 11. Si falta, se activa el respaldo de Windows Forms | Codex, 11:36 (`TestResults/WebView2-Respaldo/`); no lo repetí | — |
| 3.3 | Visual Studio, SDK, Office | No hacen falta | **CÓDIGO** | — |

## 4. Archivos de la carpeta: qué pasa si falta cada uno

| Archivo | Si falta | Base |
|---|---|---|
| `Visual.exe.config` | Ver 1.5 | **EJECUTADO** |
| `Datos.dll`, `Microsoft.Data.SqlClient.dll` | Como 1.1: la contraseña a la vista; al entrar, el aviso de error | **EJECUTADO** |
| `Microsoft.Data.SqlClient.SNI.*.dll` | Ídem (`DllNotFoundException`) | **EJECUTADO** |
| `Azure.*`, `Microsoft.Identity.*` | **No hacen falta:** se inicia sesión y el panel carga igual (con autenticación de Windows) | **EJECUTADO** |
| Carpeta `Web\` | Respaldo correcto, con el aviso «Vista web no disponible» | **EJECUTADO** |
| `Microsoft.Web.WebView2.*`, `runtimes\…\WebView2Loader.dll` | Error en las cuatro pantallas; el panel del Administrador queda sin dibujar (H-2) | **EJECUTADO** |
| `Newtonsoft.Json.dll` | El panel del Administrador muestra el aviso rojo de error (`FileNotFoundException` en `PanelWeb.Renderizar`); los indicadores se ven | **EJECUTADO** |
| `itext.*`, `BouncyCastle.Cryptography.dll` | La aplicación arranca. Falla **después de confirmar el pago**, al generar el ticket (`GenerarTicket` no tiene `try`): el pago queda registrado y el ticket no se genera. Al exportar un reporte, como `FileNotFoundException` es una `IOException`, sale «No pudimos guardar el PDF. Cerrá el archivo si está abierto…», que no es la causa | **CÓDIGO** |
| `QRCoder.dll` | Igual que iText, en el ticket | **CÓDIGO** |

**Regla:** copiar `Visual\bin\Debug` entero, con sus subcarpetas, y abrir la aplicación desde esa
copia antes de salir de casa.

## 5. Sin internet

| # | Qué | Resultado | Base |
|---|---|---|---|
| 5.1 | URLs externas en el código o en las páginas web | No hay ninguna: se buscó `http` en `Visual/`, `Repo/` y `Datos/` | **CÓDIGO** |
| 5.2 | Correo | Se usa el simulado salvo que estén definidas las cinco variables `EVENTAURA_SMTP_*`. En la PC de JC no están, ni para el usuario ni para la máquina | **EJECUTADO** (lectura de las variables) |
| 5.3 | Correo real configurado y sin red | El envío falla al registrarse o al recuperar la contraseña | **DEDUCIDO**. No aplica si no se definen las variables |
| 5.4 | Fuentes del PDF de reportes | Usa Nirmala UI si está instalada; si no, Helvetica | **CÓDIGO** |

## 6. Permisos y carpetas de salida

| # | Carpeta | Riesgo | Base |
|---|---|---|---|
| 6.1 | `%LOCALAPPDATA%\EventAura` (registro de errores, bandeja simulada, datos de WebView2) | Cualquier usuario de Windows puede escribir en la suya; solo falla en perfiles de quiosco u obligatorios. Si falla, el registro de errores se omite y WebView2 cae al respaldo. La bandeja simulada lanzaría un error al registrarse o recuperar la contraseña | **DEDUCIDO**; riesgo bajo |
| 6.2 | `Documentos\A1.Tickets EVENTAURA` y `Documentos\A1.Reportes EVENTAURA` | **Riesgo real en una PC de facultad:** el «Acceso controlado a carpetas» de Defender o Documentos redirigido a OneDrive pueden impedir la escritura. Ticket: error después del pago confirmado (sin `try`). Reporte: `UnauthorizedAccessException` no es `IOException`, así que sale el aviso de error general | **CÓDIGO** y **DEDUCIDO** (no se puede activar Defender acá sin cambiar la PC de JC) |
| 6.3 | Carpeta copiada desde un `.zip` bajado o recibido por correo | SmartScreen: «Windows protegió su PC» al abrir. Una PC administrada puede no dejar seguir | **DEDUCIDO** |

## 7. Primera ejecución y la demostración en vivo

| # | Caso | Resultado | Base |
|---|---|---|---|
| 7.1 | Ninguna cuenta | Ver 2.4. Sin `02` no hay Administrador, y un Administrador solo lo crea otro | **EJECUTADO** |
| 7.2 | Cuentas que trae la base | `admin@eventaura.com` / `Admin1234`. `organizador@`, `bronce@`, `plata@` y `oro@eventaura.com` / `Cliente123`. Las de la demostración (`@demo.eventaura.example`) usan la misma contraseña que `bronce@` | **EJECUTADO** (admin y plata); el resto, **CÓDIGO** |
| 7.3 | Contraseña mal cinco veces | La cuenta se bloquea **15 minutos** (RN-04) | **CÓDIGO** (parámetros de `02`) |
| 7.4 | Quince minutos sin tocar nada | Se cierra la sesión, con aviso 60 s antes (RN-46) | **CÓDIGO** |
| 7.5 | Pago | La reserva dura **10 minutos** (RN-15) | **CÓDIGO** |
| 7.6 | Registrar una cuenta en vivo | Pide un código que se escribe en un archivo de `%LOCALAPPDATA%\EventAura\BandejaSimulada`; la interfaz no lo muestra | **CÓDIGO** |
| 7.7 | Proyector de 1366 × 768 o 1024 × 768 | Las ventanas miden hasta 760 px. Al 100 % en 768 de alto equivale a 1080 al 150 %, que sí está probado (CP-25, CP-69, CP-85 y CP-357 a 144 DPI) | Cubierto **indirectamente**; no se probó en una pantalla real de 768 |

---

## Tickets nuevos o ampliados (en `docs/TICKETS.md`)

| Ticket | Qué |
|---|---|
| **T-03 (ampliado)** | Cubrir también `Newtonsoft.Json.dll` |
| **T-20 (urgente)** | Pantalla de inicio sin base: la contraseña a la vista, ningún mensaje y 15 s de espera |
| **T-21** | El cartel de esquema sin `02` pide `04` |
| **T-22** | El ticket se genera fuera de un `try` después del pago confirmado, y la exportación del PDF da mensajes que no corresponden a la causa |
