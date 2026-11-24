# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo y `docs/TICKETS.md`;
2. confirma que el ticket es suyo y que no está bloqueado;
3. corre `git status`.

Si el ticket no es suyo, o si hay cambios sin commitear en su zona que no son de su sesión, no toca
nada y avisa a JC.

**Documento operativo:** `Pruebas/GUION-DEMOSTRACION.md` queda a cargo de Claude Code. Es
documentación y Claude Code está ejecutando el guion; Codex no lo modifica.

Última actualización: **24/11/2026, a la tarde**, por Claude Code. Reemplaza la del 10/11, que
había quedado vieja: decía 61 casos y que la renovación visual estaba en duda.

---

## Turno

| | |
|---|---|
| **Codex** | **Primero T-03 y T-20, urgentes** (auditoría de riesgos de la defensa, `docs/AUDITORIA-RIESGOS-DEFENSA.md`). T-03: sin las DLL de WebView2, las cuatro pantallas dan error (confirmado el 24/11 a las 14:35). **Después, T-01:** la tanda visual sin commitear (gráficos con eje, estados de disponibilidad, capturas con la semilla de demostración). Después, T-02 a T-04 (respaldo de WebView2) y T-05 si JC lo aprueba. Al terminar, T-06: congelar el código |
| **Claude Code** | Lo que no depende del congelamiento: T-07 (catálogo) y T-19; T-15 (medir el QR para JC-R10) cuando Codex no esté trabajando, porque abre la aplicación. Lo demás del cierre (T-09 a T-13) espera a T-06 y T-08 |
| **JC** | Revisar `git status` y autorizar los commits pendientes (T-01, T-18), decidir T-05, pasar los datos de T-14 |
| **Ramas** | Rama de trabajo: `JC`. Local en `4c200ef` (semilla de demostración); `origin/JC` en `1d05fc2`, sin la semilla: el push lo decide JC. `main` local quedó en `e2d8ff6`, 21 commits detrás de `origin/main`. `respaldo/jc-2026-11-04` no se toca |

**Codex: tu encargo vigente es T-01 y los tickets que siguen en `docs/TICKETS.md`.** Los encargos
viejos de `docs/TAREAS-CODEX-PRUEBAS.md` (hasta la parte 6B) están cerrados o reemplazados: no
arranques ninguno de ahí.

---

## La foto: dónde estamos

### Código

| Qué | Estado |
|---|---|
| Funcionalidad | RF-01 a RF-22, todos construidos salvo RF-09 (Won't have), con tres perfiles: Cliente, Organizador y Administrador de plataforma |
| Reglas | 52 vigentes (RN-01 a RN-54; RN-27 y RN-42 dadas de baja). **50 de 52 cubiertas** por pruebas |
| Batería | **214 casos, 360 ejecuciones, todas correctas, sobre `2db272e`** (22/11; Codex, fuera del sandbox). Después de la reforma y de WebView2, Codex informa otra vez 360 de 360 (`TestResults/diseno-visual-360.log`). Las cifras del documento se fijan recién con el código congelado (T-08) |
| Mutación de las defensas de seguridad | 73 de 77 mutantes detectados, sobre `65225e7` |
| Interfaz | Reforma visual de las 25 pantallas (24/11) y cuatro vistas en WebView2 (abajo) |
| Congelamiento | **No congelado.** El cronograma del 10/11 lo ponía el 27/11; la fecha la confirma JC con T-06 |

### Lo que pasó desde el 10/11, en una línea por tema

- **11 al 16/11:** séptima tanda y cierres de seguridad, publicación y pruebas de mutación.
- **17 al 19/11:** tres perfiles, solicitudes de Organizador con CUIT y teléfono, publicación con
  revisión del primer evento, seguridad de la cuenta y auditoría (RF-14 a RF-22, RN-43 a RN-54).
- **20 al 22/11:** normalización de botones y teclado, correo SMTP configurable por entorno con el
  simulado por defecto, y el código del teléfono siempre simulado (`db02dd4`). Pruebas sobre
  ventanas reales (`2db272e`).
- **24/11, madrugada y mañana:** reforma visual de las 25 pantallas en cuatro commits (`cfb8352`,
  `3030f10`, `48647f0` y `24a1e6c`), los seis commits de interfaz de abajo y la semilla de
  demostración.

### Los seis commits de interfaz del 24/11 (después de `d93c4b3`)

| Commit | Hora | Qué |
|---|---|---|
| `24da9ed` | 10:39 | Corrige las regresiones que salieron de revisar las capturas de la reforma |
| `72cfcc6` | 10:51 | Lateral oscuro en los tres menús, catálogo en tarjetas y resumen de compra por filas |
| `ffe5c0f` | 11:16 | Cierre visual del inicio del cliente, el catálogo y la compra |
| `e846ef7` | 11:21 | **WebView2:** el catálogo pasa a una página web local |
| `d9f7f08` | 11:29 | **WebView2:** reportes con gráficos y el panel del Administrador |
| `1d05fc2` | 11:37 | **WebView2:** el comprobante de la compra |

Reporte de Codex: `Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-24-REFORMA.md`.

### La migración a WebView2

- **Cuatro vistas:** el catálogo (`frmEventosCliente`), el resumen de la compra
  (`FrmCompraCliente`), los reportes (`frmReportes`) y el panel del Administrador
  (`PanelAdministrador`). El resto de cada pantalla (filtros, acciones, navegación) sigue en
  Windows Forms.
- **`Visual/Web/PanelWeb.cs`** carga `Visual/Web/*.html` desde la carpeta del programa. Recibe los
  datos ya calculados como JSON y le devuelve mensajes a la pantalla. No expone objetos de la
  aplicación, no navega fuera de `eventaura.local` y no accede a la red. Paquete:
  `Microsoft.Web.WebView2` 1.0.2903.40.
- **Las capas no cambian:** la página no llega al negocio ni a los datos.
- **Respaldo:** si la vista web falla, la pantalla muestra su versión de Windows Forms con un
  aviso. Auditado el 24/11 (`docs/AUDITORIA-RESPALDO-WEBVIEW2.md`): funciona, pero no está en la
  batería (T-02) y no cubre la falta de las DLL (T-03).
- **Documento:** las pp. 18 y 24 ya tienen la oración de WebView2; la p. 57 y la bibliografía van
  con el código congelado (T-13).

### La semilla de demostración (`4c200ef`)

`BaseDatos/semilla-demostracion.sql`: datos para la demostración y las capturas, **no para la
batería**. Contiene:
- 4 locales de Rosario con direcciones reales y 20 eventos de las categorías existentes, entre seis
  meses atrás y cuatro adelante;
- sectores distintos por local y 337 entradas vendidas de forma despareja: 3 eventos agotados,
  varios a medias y 2 sin ventas;
- 14 clientes `@demo.eventaura.example` en los tres niveles, con historial coherente con RN-19 a
  RN-23 y RN-37;
- 3 personas en lista de espera;
- solicitudes de Organizador pendientes, rechazada y aprobada, con CUIT válido (RN-52).

Se puede correr más de una vez: la segunda no inserta nada. Se verificó en una base aparte. Codex
la usa, sin commitear (T-01), con `BaseDePrueba.PrepararDemostracion`, que crea
`BddEventAura_Capturas_Demostracion`, y con `CapturarReforma.ps1 -Demostracion`.

### El documento

El cierre está preparado para hacerse en una tarde:
- **`docs/CIERRE-DOCUMENTO.md`:** el inventario, las secciones estables y las que dependen del
  código;
- **`docs/LISTA-PARA-MAXIMO.md`:** cada valor que depende del código está como un marcador
  `[[...]]`;
- **`tools/Cierre/Inventario.py`:** cuenta lo que falta.

Las páginas de pantallas siguen en pausa hasta las capturas finales.

**Decisiones de JC del 24/11, ya aplicadas:**
- «Máximo», con tilde;
- los años 2025 pasan a 2026;
- en las pp. 18 y 24, la redacción (a) de WebView2;
- el hueco CP-212 a CP-353 se deja;
- la batería final se corre dos veces.

### Pruebas manuales abiertas

| ID | Qué | Ticket |
|---|---|---|
| JC-R10 | Escanear dos tickets con el celular (esta PC no tiene cámara) | T-15 |
| JC-R12 | Las cuatro pantallas sin WebView2, al 100 % y al 150 % | T-16 |
| (manual de Codex) | Enter en Suscripción, porque abre un modal | T-05 |

---

## Quién hace qué

| Quién | Rol | Zona que modifica |
|---|---|---|
| JC (Juan Cruz Riggio) | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push | Todo, a mano |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |
| Codex | Implementación y pruebas automáticas | `Visual/`, `Repo/`, `Datos/`, `Pruebas/`; `BaseDatos/` y Designer solo con autorización de JC |

Los tickets (`docs/TICKETS.md`) los escribe solo Claude Code. Codex informa el avance en su
reporte final (`Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>.md`).

## Observaciones de este turno

- **`CLAUDE.md`, «Estado actual», está viejo:** dice 61 casos y 73 ejecuciones. Si JC quiere, lo
  actualiza Claude Code junto con T-13. Mientras tanto, la cifra vigente es la de este archivo.
- **Corrección de Claude Code:** el 24/11 había dicho que el respaldo de WebView2 no lo había mirado
  nadie. Codex lo probó una vez a las 11:36 (`TestResults/WebView2-Respaldo/`). También habían
  quedado fechados «25/11» el documento de cierre y la lista; se corrigió a 24/11.
- **`Generar.ps1` necesita `pymupdf`**, instalado en `D:\jc-tools`: correr con
  `$env:PYTHONPATH = 'D:\jc-tools'`.

## Qué debería hacer el próximo agente

**Codex:**
1. **T-03 y T-20, urgentes**; después T-21 y T-22;
2. terminar T-01 y reportar;
3. T-02 y T-04;
4. T-05 solo si JC lo aprueba;
5. avisar cuando el código pueda congelarse (T-06).

No toques `docs/`: lo que tengas que decir sobre los tickets, ponelo en tu reporte.

**Claude Code:**
1. T-07 y T-19 mientras Codex trabaja, sin correr la batería ni abrir pantallas; T-15 cuando Codex
   no esté trabajando;
2. con T-06 cerrado, T-08 (segunda corrida) y T-09 a T-13, en ese orden.
