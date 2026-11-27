# Verificación de `muestra-1`, 27/11/2026

Claude Code, sin JC. Todo se verificó ejecutando y midiendo. Capturas y registros en
`docs/verificaciones/muestra-1-2026-11-27/`.

**Resultado:** sin fallas P0 en cliente, Administrador ni Organizador. El tag `muestra-1` va sobre
`ed23ac0`. Las fallas son P1 a P3: T-47 a T-64 en `docs/TICKETS.md`.

## Base

- **Ejecutable:** compilado desde un clon limpio de la rama `JC`. Para la batería, sobre `9977314` y
  después sobre `ed23ac0`. Lo recorrido en pantalla es de `9977314`. Entre ese commit y `ed23ac0`
  solo cambian tres archivos de `Pruebas/`.
- **Bases:**
  - `BddEventAura_Demostracion`, la de la demo, rearmada hoy con `01`, `02` y la semilla;
  - `BddEventAura_Muestra_Demostracion`, una copia descartable para aprobar, rechazar y publicar sin
    gastar los datos de la demo.
- **Escritorio:** el lock de Codex de las 11:31 se tomó como abandonado a las 12:12 (41 min, sin
  `Visual.exe` ni pruebas). A las 12:43 y 12:44 Codex lanzó dos `vstest` con mi lock puesto: frené
  hasta que terminaron. Después, Codex borró mi lock al terminar. Una de mis sesiones terminó sin
  dejar rastro en ese lapso. Probablemente la cerró la corrida de Codex, pero no lo pude confirmar.
- **Escala:** 96 DPI durante toda la verificación; no se cambió.

## Batería, corrida independiente desde el clon

| Commit | Resultado | Registro |
|---|---|---|
| `9977314` | **378/379**. CP-160 falló dos veces seguidas: esperaba el título «Armar paquete» y el shell dice «Eventos disponibles» | `bateria-9977314.log` |
| `ed23ac0` | **379/379**. `790546f` cambió la expectativa de CP-160 | `bateria-ed23ac0.log` |

Correr con `EVENTAURA_CAPTURAS_UI` sin `EVENTAURA_CAPTURAS_DPI` hace fallar 9 casos con «Especificá
el DPI real esperado». No es una falla de la aplicación.

## 1. Cliente

| Qué | Resultado | Evidencia |
|---|---|---|
| T-45 en 5 eventos de 1, 2, 3, 4 y 4 sectores | **Pasa.** El total queda en la y 533 a 548 y «Continuar al pago» en la y 637 a 678, dentro de una vista que termina en 759, en los cinco | `t45-*.png` |
| 1100 × 720 | **No se alcanza en esta pantalla:** «Mi cuenta» no baja de 1200 × 760 de área cliente (ventana de 1216 × 799), y ese mínimo solo baja en pantallas más chicas. CP-373 lo prueba fijando el área cliente | — |
| El ojo, en los 8 campos de contraseña | **Pasa.** Un solo ojo por campo, sin el nativo de Edge, en acceso (1), alta (2), recuperación (2) y cambiar contraseña (3) | `ojo-*.png` |
| Modal de contratar | **Abre y Enter confirma** («Plan Mensual activo · Vence el 27/12/2026»). **Esc lo cierra pero además saca a «Inicio»** (T-47, P1) | `sus-2` a `sus-7` |
| Modal de cancelar | Abre. **Enter confirma la cancelación** (T-51, P2) | `sus-8`, `sus-9` |
| Diálogos del sistema en compra y suscripción | **Ninguno.** UI Automation no encontró ninguna ventana aparte del proceso en todo el recorrido | — |
| Catálogo | **Web, sin caer al respaldo**, en cada vuelta | `t45-*.png` |
| Lista de espera (Clásico Rosarino) | **Pasa.** «Te anotaste en la lista de espera…» y el sector pasa a «Ya estás en la lista de espera» | `espera-*.png` |
| Verificación con sesión (`oro@`, con el correo marcado sin verificar) | **Pasa.** RN-43 rechaza el armado con **Verificar correo**. La vista abre con el título «Verificar correo» y el campo del código; el código de la bandeja la confirma y la cuenta vuelve a `CorreoVerificado=1`. Una vez el primer clic no abrió nada (T-55) | `verif-*.png` |
| Compra completa con ticket | **Pasa.** Taller de fotografía urbana: «Seguir sin seguro», el Pago web con la tarjeta de prueba y Enter dan **EA-000319**, $ 13.656,25. El PDF trae la imagen del QR (no lo decodifiqué: faltan bibliotecas y no instalé nada). **El aviso del seguro corta «Continuar al pago»** (T-53) | `compra-*.png`, `ticket-EA-000319.pdf` |
| Pago web (nuevo) | Sus 4 campos ahora son `Edit` con nombre para UI Automation: T-46, punto 2, resuelto | `compra-2-pago.png` |

**Lo que queda en `BddEventAura_Demostracion`:**
- `plata@`: la compra EA-000319 y una inscripción en la lista de espera de Clásico Rosarino;
- su suscripción se contrató y se canceló;
- `oro@`: un código de verificación consumido.

Si JC quiere la base intacta, se rearma en dos minutos (guion, «Armar la base»).

## 2. Administrador y Organizador

| Qué | Resultado | Evidencia |
|---|---|---|
| Menú por rol (RN-48) | **Pasa.** El Administrador ve «Plataforma», «Revisión» y «Cuenta», y nada de compra. El Organizador ve «Locales y eventos», «Operación», «Resultados» y «Cuenta» | `admin-1-inicio.png`, `org-1-inicio.png` |
| Solicitudes de organizador | **Pasa.** Dos pendientes con responsable, CUIT, teléfono, CBU o alias y la constancia. Exigen un motivo: sin él, «Aprobar» no hace nada visible (T-55) | `admin-Solicitudes-de-organizador.png` |
| Aprobar / rechazar | **Pasa.** «Solicitud aprobada y local creado.» / «Solicitud rechazada con el motivo registrado.». En la base: `Aprobada` con el local creado, y `Rechazada`. Nada se aprueba solo | `admin-sol-*.png` |
| Validación de CUIT y CBU (RN-52) | **Por código, no por pantalla:** `GestorSolicitudesOrganizador.CuitValido` (dígito verificador), `CbuValido` (los dos dígitos de control) y `CbuAliasValido`. No envié una solicitud con datos inválidos | — |
| Eventos en revisión | **Abre, vacía:** la semilla no trae ninguno (T-56). Ventana de 823 px (T-49) | `admin-Eventos-en-revisión.png` |
| Auditoría (RN-45) | **Pasa.** Registra mis inicios de sesión, la compra iniciada y confirmada y los códigos emitidos, con hora y correo. Ventana de 823 px, sin título (T-49, T-60) | `admin-Registro-de-auditoría.png` |
| Reportes globales y PDF | **Pasa.** RPT-01 se genera y se exporta a `Documentos\A1.Reportes EVENTAURA\EVAU_RPT01_20260901_20261127.pdf`. El gráfico se ve en una franja (T-59) | `admin-rep-exportar.png`, `reporte-RPT01-admin.pdf` |
| Crear administrador | Abre; no lo envié. Usa la casilla «Mostrar contraseña» | `admin-crear-administrador.png` |
| Locales del organizador (RN-49) | **Pasa.** Ve solo Galpón 11 y Galpón 17; los locales de los otros seis organizadores no aparecen | `org-Mis-locales.png` |
| Crear evento, sector y precio, publicar | **Funciona, con una falla:** el evento se guarda como borrador; el sector «Campo», 100 lugares a $ 15.000, se guarda; **Publicar lo publica directo** porque el organizador ya tuvo un evento aprobado (RN-54). **Publicó un evento con fecha pasada** (T-48, P1) | `org-agregar-evento*.png`, `org-administrar*.png`, `org-publicar.png` |
| Control de ingreso | **Abre:** «No hay eventos con ingreso abierto en tus locales». No se puede mostrar con la semilla (T-56) | `org-Control-de-ingreso.png` |
| Reportes del organizador (RN-49) | **Pasa a la vista:** solo eventos suyos, incluido el recién publicado. No lo comparé fila por fila con la base | `org-Reportes.png` |
| Ventanas | **Paneles en 1200 × 760.** Las pantallas secundarias abren como ventanas aparte de 1116 × 759, salvo Auditoría y Revisión (823). **Su X cierra toda la aplicación** (T-52) | — |

**Lo que no existe:**
- revisar un evento pendiente y control de ingreso con datos, con la semilla actual (T-56);
- lista de espera del organizador: el menú la tiene y no la abrí.

## 7. Clon limpio y README

Clon de la rama `JC` en `%TEMP%\eaclon`, siguiendo el README.

| Paso | Qué pasó |
|---|---|
| Clonar en el scratchpad (ruta de unos 110 caracteres) | **Falla la compilación:** `NuGet.targets` no encuentra archivos de `packages\…`, por el límite de 260 caracteres. El README ahora pide una ruta corta |
| El MSBuild del README (`-restore`) | **Falla:** «Este proyecto hace referencia a los paquetes NuGet que faltan…» (`Microsoft.Data.SqlClient.SNI.targets`). `Visual` usa `packages.config` y `-restore` no lo restaura. **Con `-p:RestorePackagesConfig=true`, compila sin errores.** También estaba mal `tools/Proyecto.ps1`: corregido en `a51a7cd` |
| Crear la base y cargar la semilla | El README viejo no lo decía. Agregado y ejecutado: `01` y `02` terminan con código 0 |
| `sqlcmd -S .\SQLEXPRESS` | **Falla** con el `sqlcmd` nuevo: «Timed out waiting for pipe '\\.\pipe\SQLLocal\SQLEXPRESS'». La canalización con nombre funciona. Agregado al README |
| Apuntar la aplicación | El README viejo no lo decía. Agregado: se edita `Visual.exe.config` y no `App.config` |
| Abrir | Abre «Bienvenida»; entran Ana, Paula y el organizador |
| **Sin WebView2** (`WEBVIEW2_BROWSER_EXECUTABLE_FOLDER` apuntando a una carpeta que no existe) | **Entra el respaldo:** el acceso WinForms avisa «La vista web no está disponible…» y el catálogo de tarjetas, «Vista web no disponible. Podés seguir usando las tarjetas y la tabla.». No desinstalé el runtime: lo simulé así | `sin-webview2-*.png` |
| Batería según el README | 379/379 sobre `ed23ac0` |
