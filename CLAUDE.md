# EventAura — Contexto del proyecto

Plataforma de venta de entradas para espectáculos de Rosario. El diferencial es el
**armado personalizado de paquetes**: entrada + hospedaje + traslado + merchandising +
experiencia VIP en una sola operación.

Proyecto académico — Trabajo de Campo I, Universidad Abierta Interamericana.
La documentación completa está en `Documentacion/EventAura_v2.pdf` (112 páginas).
**No hace falta leer el PDF**: todo lo necesario para programar está en `docs/`.

---

## Stack

| Componente | Versión |
|---|---|
| .NET Framework | 4.8 (Visual) · netstandard2.0 (Entidades, Datos, Repo) |
| Base de datos | SQL Server |
| Acceso a datos | Microsoft.Data.SqlClient 6.0.2 |
| Generación de PDF | iText 9.2.0 |
| Códigos QR | QRCoder 1.6.0 |
| Pruebas | MSTest |

---

<!-- compartido:arquitectura (igual en AGENTS.md; verificar con tools/VerificarReglasAgentes.ps1) -->
## Arquitectura — 4 capas, obligatorio

```
Visual  (WinForms)     →  Negocio  (Gestores)  →  Datos  (Repositorios)  →  SQL Server
                                    ↓
                              Entidades  (POCOs, sin lógica)
```

**Reglas de dependencia, no negociables:**

1. `Visual` **nunca** abre una conexión ni escribe SQL. Solo habla con `Negocio`.
2. `Negocio` **nunca** referencia `System.Windows.Forms`. No muestra MessageBox: devuelve
   resultados o lanza excepciones de dominio.
3. `Datos` es el único lugar donde existe `SqlCommand`, `SqlConnection` y texto SQL.
4. `Entidades` no referencia a nadie. Son POCOs con propiedades automáticas.
5. Todo SQL usa **parámetros** (`cmd.Parameters.AddWithValue`). Nunca concatenación.

**Patrones esperados por la cátedra:**

- **Singleton** en todos los gestores de `Negocio` y repositorios de `Datos`
  (seguir el estilo ya existente en `Repo/GestorClientes.cs`).
- **Composite** para el árbol de permisos (anexo T04, futuro).
- **Observer** para el cambio de idioma (anexo T05, futuro).
<!-- /compartido -->

---

## Estructura de carpetas

| Carpeta | Proyecto | Namespace | Rol |
|---|---|---|---|
| `Entidade/` | Entidades.csproj | `Entidades` | POCOs y enums |
| `Datos/` | Datos.csproj | `Datos` | Repositorios, único acceso a SQL |
| `Repo/` | Repositorio.csproj | `Negocio` | Gestores, reglas de negocio |
| `Visual/` | Visual.csproj | `Visual` | Formularios WinForms |
| `Pruebas/` | Pruebas.csproj | `Pruebas` | MSTest |

> **Una inconsistencia heredada del proyecto original:** la carpeta del proyecto de
> negocio se llama `Repo/` pero el proyecto es `Repositorio.csproj`. No la renombres.
>
> La otra, los dos namespaces dentro de ese proyecto, **quedó cerrada en la Sección 10**:
> el último archivo en `Repositorio`, `GestorPropietario.cs`, pasó a `Negocio`. Todo
> `Repo/` está en `Negocio` y ya no existe el namespace `Repositorio`: un archivo nuevo de
> negocio va en `Negocio`, y un `using Repositorio;` no compila.

**Carpetas eliminadas** (eran código muerto fuera de la solución): `Entidades/`,
`Negocio/`, `Visual_/`. Si volvés a verlas, están de más.

---

<!-- compartido:convenciones -->
## Convenciones de código

- Nombres en **español**, igual que el dominio: `GestorEventos`, `RepositorioCompras`.
- Formularios: prefijo `frm` (`frmMenuCliente`). Los existentes mezclan `frm` y `Frm`;
  para archivos nuevos usar siempre `frm` minúscula.
- Métodos de repositorio: verbo + sustantivo (`ObtenerPorId`, `ContarComprasConfirmadas`).
- Importes: `decimal`, nunca `float` ni `double`.
- Fechas: `DateTime`. La ventana de fidelización usa `AddMonths(-12)`.
- Los valores numéricos de las reglas **no se escriben en el código**: se leen de la tabla
  `Parametros` (ver `docs/02-reglas-negocio.md`, sección de parámetros configurables).
- **Archivos `.Designer.cs`**: el criterio es **no rediseñar por gusto**. No se reposiciona,
  redimensiona ni recolorea nada que ya funcione. Sí se permite, siempre declarándolo en el
  reporte:
  - **agregar** un control que un requerimiento necesita;
  - **reemplazar** un grupo de controles por otro cuando la tarea lo pide de forma explícita;
  - corregir el **orden de tabulación** (`TabIndex`);
  - corregir el **texto visible** de etiquetas y botones.
  Ante la duda, preguntá antes de tocar. El formato de un `DataGridView` va por código en el
  `Load`, no en el Designer.
- **Alto máximo de un formulario**: `ClientSize` no pasa de **790 px de alto**. El área útil
  de la pantalla de desarrollo es 824 px y la barra de título suma unos 30: un formulario más
  alto lo recorta Windows y los controles de abajo quedan inalcanzables. Ya pasó una vez:
  `frmMenuPropietario` llegó a 1020 px y dejó «Volver al inicio» fuera de la vista, así que el
  administrador no tenía forma de volver al inicio de sesión. Si lo que hay que agregar no
  entra, **compactá el espaciado; no agrandes el formulario**. En una máquina con escala de
  Windows al 125 % el área útil es menor todavía, así que 790 es un techo y no un objetivo.
  Lo exige RNF-03 (compatibilidad multipantalla).
  Nota heredada: 11 formularios declaran `AutoScaleDimensions` 120F y 8 declaran 144F, porque
  se diseñaron en dos máquinas con escalados distintos. No unificarlo sin rehacer la
  verificación visual de los 19.
<!-- /compartido -->

---

<!-- compartido:convivencia -->
## Reglas de convivencia entre agentes

En este repositorio trabajan dos agentes, **Claude Code** y **Codex**, siempre a pedido de
JC (Juan Cruz Riggio). El repositorio es compartido con Máximo Ambrosino: lo que se publica
le llega a él.

**Hay otro agente trabajando hacia el mismo entregable**: la entrega final del Trabajo de
Campo, a principios de diciembre de 2026. Cada uno lee la salida del otro, así que los
reportes se escriben para el otro agente, no solo para JC.

| Quién | Qué hace |
|---|---|
| JC | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex |
| Codex | Implementación y pruebas automáticas |

### Antes de empezar: `docs/ESTADO-AGENTES.md`

- **Los dos agentes lo leen antes de hacer cualquier cosa.** Dice de quién es el turno, en
  qué tarea está cada uno, qué quedó pendiente y qué está bloqueado.
- Lo escribe solo Claude Code. Codex no lo modifica: reporta sus cambios de estado en su
  reporte final, y JC se los pasa a Claude Code.
- Si el turno no es tuyo, no toques nada y avisá a JC.

### Reparto de carpetas

| Agente | Puede modificar |
|---|---|
| Codex | `Visual/`, `Repo/`, `Datos/`, `Pruebas/` |
| Claude Code | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |

- Fuera de su zona, un agente **solo lee**. Si la tarea necesita un cambio en la zona del
  otro, no lo hace: lo describe en su reporte final.
- Lo que no figura en la tabla (`Entidade/`, `BaseDatos/`, `.vscode/`, `.gitignore`,
  `README.md`, `SistemaEntradas.sln`, `Documentacion/`) no se toca sin que JC lo autorice
  de forma explícita en esa sesión.
- Las pruebas manuales que surjan del trabajo de Codex van en su reporte final, con qué
  probar y qué resultado se espera. Claude Code las pasa a `docs/PRUEBAS-PENDIENTES.md`.
- Codex deja sus reportes en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>.md`. Claude Code los
  mueve a `docs/` en su turno siguiente.

### Un agente por vez

- **Nunca trabajan los dos agentes al mismo tiempo.** El turno está en
  `docs/ESTADO-AGENTES.md`.
- Al empezar, el agente corre `git status`. Si hay cambios sin commitear que no son de su
  sesión, se detiene y avisa a JC antes de modificar nada.
- Cuando el agente termina su lista, **para**: no busca qué más mejorar por su cuenta. Si ve
  algo, lo anota en el reporte para que decida JC.
- El reporte final dice qué se hizo, qué quedó pendiente, qué se encontró fuera de la zona
  propia y qué debería hacer el otro agente después.

### Commits y publicación

- **Ningún agente commitea sin que JC haya revisado `git status`** y lo haya autorizado en
  esa sesión.
- Commits por tema (interfaz, pruebas, herramientas, documentación), nunca uno gigante.
- **Ningún agente hace `git push`**, `push --force` ni abre pull requests. Publicar lo decide
  y lo hace JC, de acuerdo con Máximo.

### Configuración local: los `App.config` nunca se publican

- `Visual/App.config` y `Pruebas/App.config` guardan la cadena de conexión de cada máquina.
  En la PC de JC apuntan a `.\SQLEXPRESS`; en el repositorio, a la instancia de Máximo. Si
  se publica la de JC, la aplicación de Máximo deja de conectarse.
- En la PC de JC los dos están marcados con `git update-index --skip-worktree`: `git status`
  no los muestra y `git add -A` no los incluye. Para comprobarlo:
  `git ls-files -v Visual/App.config Pruebas/App.config` (la `S` inicial indica que siguen
  protegidos).
- La marca se revierte **solo** en dos casos: (a) un `git pull` trae cambios a esos archivos
  y git avisa que se sobrescribirían; (b) hay que cambiar a propósito algo compartido en
  ellos, por ejemplo un `bindingRedirect`. Procedimiento, que hace JC y no un agente:
  1. `git update-index --no-skip-worktree Visual/App.config Pruebas/App.config`
  2. `git stash push -- Visual/App.config Pruebas/App.config` (guarda la cadena local)
  3. `git pull`, o hacer el cambio compartido y commitearlo con la cadena de Máximo
  4. `git stash pop` y, si hay conflicto, conservar `.\SQLEXPRESS`
  5. `git update-index --skip-worktree Visual/App.config Pruebas/App.config`
<!-- /compartido -->

---

## Documentos de referencia

| Archivo | Contenido |
|---|---|
| `docs/01-requerimientos.md` | RF-01 a RF-13 y RNF-01 a RNF-05 |
| `docs/02-reglas-negocio.md` | RN-01 a RN-39, parámetros configurables y factibilidad económica |
| `docs/03-esquema-datos.md` | DDL completo y datos semilla |
| `docs/04-casos-prueba.md` | CP-01 a CP-10 |
| `docs/05-reportes.md` | RPT-01 a RPT-05, PDF de salida y correcciones al anexo A02 |
| `docs/GUIA-PROMPTS.md` | Orden de construcción, sección por sección |
| `docs/ESTADO-AGENTES.md` | Turno, tareas, pendientes y bloqueos de cada agente. **Leerlo antes de empezar** |
| `docs/CORRECCIONES-DOCUMENTO.md` | **Lista única** de correcciones al documento entregable (reglas, instructivo, A02, matriz, carátula), por página |
| `docs/DOCUMENTO-ENTREGABLE.md` | Estado del documento: el PDF es un armado con pypdf, el fuente no está versionado, y plan B |
| `docs/usabilidad/INFORME-USABILIDAD.md` | Estudio de usabilidad simulado, medido sobre la aplicación real; hallazgos U-01 a U-13 y plan de mejora |
| `docs/ANEXOS-TECNICOS-T01-T08.md` | Mapa de los anexos técnicos de la Carpeta Modelo: qué existe, qué falta, horas y orden |
| `docs/COMO-SE-PROBO.md` | Historia completa de las pruebas: manuales, automáticas e inspecciones; quién, cuándo y cómo |
| `docs/EXCEPCIONES.md` | Diseño del manejo de excepciones y qué caminos de error están probados |
| `docs/TAREAS-CODEX-PRUEBAS.md` | Encargos para Codex y su estado |
| `docs/PRUEBAS-PENDIENTES.md` | Pruebas manuales por hacer |

Cuando una tarea mencione una regla (`RN-19`) o un requerimiento (`RF-08`), leé su
definición del archivo correspondiente. No la deduzcas.

---

## Estado actual

Secciones 0 a 10 de `docs/GUIA-PROMPTS.md` construidas, más las correcciones C-1 a C-9.

Construido: login con PBKDF2 y bloqueo por intentos, alta de usuarios, gestión de eventos
con estados y sectores, búsqueda y filtros, armado de paquete con servicios y seguro,
descuento por nivel de fidelidad, reserva de cupo con vencimiento, pago simulado con
tokenización, confirmación y ticket PDF con QR por entrada, suscripciones con preventa y
seguro sin cargo, devolución por entrada, historial de compras, lista de espera, y los
cinco reportes con exportación a PDF.

Pruebas: el proyecto `Pruebas/` corre los casos CP-01 a CP-61 (`docs/04-casos-prueba.md`):
73 ejecuciones, todas correctas.
Errores no previstos: `Visual/ManejadorErrores.cs`, instalado en `Program.Main`, los muestra
como un aviso en castellano (ver `docs/EXCEPCIONES.md`).
Los de `PruebasInterfaz.cs` abren formularios reales y responden sus MessageBox. Son de integración contra
`BddEventAura_Pruebas`, que se recrea en cada corrida (ver `docs/04-casos-prueba.md`). Se
corren con la tarea «Probar» de VS Code, que llama a `tools/Proyecto.ps1`; `dotnet test` no
sirve porque no compila los `.resx` de `Visual`. Las pruebas de interfaz necesitan una sesión
de escritorio abierta y guardan capturas en `TestResults/` (ignorada por git).

Herramientas en `tools/`: `Proyecto.ps1` (compilar/probar, encuentra MSBuild solo),
`AuditarBaseJc.sql` (compara la base de desarrollo con la de pruebas),
`VerificarQrTickets.py` (decodifica los QR de los tickets PDF) y `CapturasInstructivo/`
(`Generar.ps1` rehace las figuras del instructivo; `VerificarEscala.ps1` comprueba la compra a
la escala de Windows vigente) y `Usabilidad/` (`Medir.ps1` mide el flujo de compra;
`VerificarCierreConX.ps1` prueba con UI Automation el cierre con la X sobre el ejecutable;
`modelo.py` deriva tiempos, satisfacción y SUS por perfil) y `Excepciones/`
(`VerificarErrorNoControlado.ps1` muestra qué ve el usuario ante un error que nadie
captura). Las de `CapturasInstructivo/` y
`Medir.ps1` abren la aplicación real contra `BddEventAura_Pruebas` con su propia
configuración, sin tocar los `App.config`.

Sin construir: **RF-09, recomendaciones personalizadas** (Could, 8 puntos; nunca entró en
ninguna sección).

Pendiente que no es código:
- aplicar `docs/CORRECCIONES-DOCUMENTO.md` al documento entregable, bloqueado hasta tener su
  fuente (ver `docs/DOCUMENTO-ENTREGABLE.md`);
- cerrar las pruebas manuales abiertas de `docs/PRUEBAS-PENDIENTES.md`.

**Simulados por diseño, aceptado por la cátedra:** las minutas y el estudio de usabilidad no
requieren personas reales. El estudio rehecho en `docs/usabilidad/` mide la interfaz sobre la
aplicación real y deriva de esa medición los resultados por perfil.

Las 4 figuras del instructivo están en `docs/capturas/instructivo/` y ya son las definitivas:
reproducen el ejemplo de la página 54 con la semilla de T-21.

---

<!-- compartido:terminado -->
## Definición de terminado

Una tarea está completa cuando:

1. La solución compila sin errores ni advertencias nuevas.
2. No hay SQL fuera de `Datos/`.
3. Las reglas de negocio citadas en la tarea están implementadas y son verificables.
4. Los casos de prueba asociados pasan (tarea «Probar» o `tools/Proyecto.ps1 -Accion Probar`).
5. **Las pruebas de interfaz que no pudiste ejecutar quedaron anotadas**, con qué probar y
   qué resultado se espera: Claude Code las escribe al final de `docs/PRUEBAS-PENDIENTES.md`;
   Codex las lista en su reporte final para que se transcriban.
6. El reporte final declara todo cambio en un `.Designer.cs` y lista los archivos tocados,
   para que JC revise `git status` antes de autorizar el commit.
7. Toda prueba automática nueva tiene un ID `CP-xx` correlativo, registrado en
   `docs/04-casos-prueba.md`. Lleva un comentario `// CP-xx — RF-yy, RN-zz: …` y el ID en el
   nombre visible: `[TestMethod("CP-xx: …")]`, o `DisplayName` en cada `DataRow`. Con MSTest
   3.6.4, `[TestMethod(DisplayName = …)]` no compila.

> No des por verificada una prueba manual que no corriste ni la omitas: anotala.
<!-- /compartido -->
