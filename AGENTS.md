# EventAura — Instrucciones para agentes (Codex)

Plataforma de venta de entradas para espectáculos de Rosario, con armado personalizado de
paquetes (entrada + hospedaje + traslado + merchandising + experiencia VIP). Proyecto
académico, Trabajo de Campo I, UAI. .NET Framework 4.8 WinForms, SQL Server, MSTest.

El contexto completo del proyecto (stack, estructura de carpetas, documentos de referencia
y estado actual) está en `CLAUDE.md`: leelo antes de empezar. Las secciones de abajo están
copiadas **textualmente** de `CLAUDE.md` y tienen que seguir iguales: si una cambia, cambia
en los dos archivos en el mismo commit. `tools/VerificarReglasAgentes.ps1` lo comprueba.

Cuando una tarea mencione una regla (`RN-19`) o un requerimiento (`RF-08`), leé su definición
en `docs/` (`01-requerimientos.md`, `02-reglas-negocio.md`). No la deduzcas.

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

- **Los dos agentes lo leen antes de hacer cualquier cosa.** Dice en qué tarea está cada uno,
  qué quedó pendiente y qué está bloqueado.
- Lo escribe solo Claude Code. Codex no lo modifica: reporta sus cambios de estado en su
  reporte final, y JC se los pasa a Claude Code.
- Si tu tarea no figura ahí ni te la dio JC en esta sesión, no empieces: avisá a JC.

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
  copia a `docs/`, y el original se borra recién cuando Codex no está trabajando.

### Trabajo en paralelo: nunca los mismos archivos

- **Los dos agentes pueden trabajar al mismo tiempo; es lo normal.** La regla es otra: **dos
  agentes nunca escriben los mismos archivos.** Cada uno escribe solo en su zona (tabla de
  arriba).
- **Dos excepciones mientras Codex trabaja:** Claude Code no modifica
  `docs/ESTADO-AGENTES.md` ni `docs/TAREAS-CODEX-PRUEBAS.md`, porque Codex los está usando para
  orientarse. Los actualiza cuando JC avisa que Codex terminó. Mientras tanto, lo nuevo va en
  borradores aparte.
- Lo que un agente lee de la zona del otro mientras el otro trabaja puede estar a medio
  escribir. Si algo se ve incompleto o inconsistente, se anota como «a confirmar cuando
  termine», no como un hecho.
- Al empezar, el agente corre `git status`. Los cambios sin commitear en la zona del otro
  agente son esperables: no se tocan ni se commitean. Si hay cambios sin commitear **en la
  zona propia** que no son de su sesión, se detiene y avisa a JC antes de modificar nada.
- Cuando el agente termina su lista, **para**: no busca qué más mejorar por su cuenta. Si ve
  algo, lo anota en el reporte para que decida JC.
- El reporte final dice qué se hizo, qué quedó pendiente, qué se encontró fuera de la zona
  propia y qué debería hacer el otro agente después.

### Commits y publicación

- **Ningún agente commitea sin que JC haya revisado `git status`** y lo haya autorizado en
  esa sesión.
- Commits por tema (interfaz, pruebas, herramientas, documentación), nunca uno gigante. Con
  los dos agentes en paralelo, cada commit lleva solo archivos de una zona.
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

<!-- compartido:arquitectura -->
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
