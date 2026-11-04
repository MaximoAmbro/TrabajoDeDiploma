# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo;
2. confirma que el turno es suyo;
3. corre `git status`.

Si el turno no es suyo, o si hay cambios sin commitear que no son de su sesión, no toca nada
y avisa a JC.

Última actualización: **04/11/2026, a la noche**, por Claude Code, al cerrar su turno.

---

## Turno

| | |
|---|---|
| **Turno actual** | **JC**: revisar `git status` y los commits de este turno; tomar las decisiones de abajo |
| Último turno cerrado | Claude Code, 04/11/2026: figuras definitivas, base de desarrollo recreada, estudio de usabilidad, mapa de T01 a T08, correcciones del documento |
| Próximo turno | **Sin asignar.** Depende de las decisiones pendientes de JC. Candidato: Codex, con las prioridades 1 a 3 del plan de usabilidad |
| Rama y publicación | `main`, adelantada respecto de `origin`; **sin push** hasta que JC hable con Máximo |

## Quién hace qué

| Quién | Rol | Zona que modifica |
|---|---|---|
| JC (Juan Cruz Riggio) | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push | Todo, a mano |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |
| Codex | Implementación y pruebas automáticas | `Visual/`, `Repo/`, `Datos/`, `Pruebas/`; `BaseDatos/` solo con autorización de JC |

Los dos agentes trabajan hacia el mismo entregable, la entrega final del Trabajo de Campo a
principios de diciembre de 2026, y cada uno lee la salida del otro. Codex deja sus reportes
en `Pruebas/` y Claude Code los mueve a `docs/`.

## Tareas por agente

### Codex

| Tarea | Estado | Dónde |
|---|---|---|
| T-01 a T-21 | **Terminadas.** La última, T-21 (semilla alineada con el documento), quedó en el commit de este turno | `docs/RESULTADOS-TAREAS-CODEX-2026-11-04*.md` |
| Sin tareas asignadas | — | Las candidatas están en «Decisiones pendientes de JC» |

### Claude Code

| Tarea | Estado |
|---|---|
| Figuras del instructivo | **Terminadas y definitivas**: «Duki en Rosario», $99.625; figura 2 sin barra gris ni píxeles ajenos |
| Base de desarrollo | **Recreada** con la semilla de T-21. Copia previa (.bak) verificada |
| Estudio de usabilidad simulado, medido sobre la aplicación | **Terminado**: `docs/usabilidad/`. 13 hallazgos (U-01 a U-13), plan de 5,5 pt |
| Mapa de T01 a T08 | **Terminado**: `docs/ANEXOS-TECNICOS-T01-T08.md`. De 100 a 144 h si se piden todos; T01 a T03 salen de material existente |
| Correcciones del documento | **Al día**: `docs/CORRECCIONES-DOCUMENTO.md`, con RNF-03 y su limitación de DPI, el rótulo «(simulado)» y el informe nuevo de usabilidad, y la brecha G-06 |

## Decisiones pendientes de JC

| Decisión | Contexto | Si se aprueba |
|---|---|---|
| Aplicar antes de la entrega las prioridades 1 a 3 del plan de usabilidad | U-01: la X deja la aplicación invisible y en ejecución. U-04: nada indica dónde se escribe la cantidad. U-05: «Comprar» habilitado sin entradas. 10 h, en `Visual/` | Claude Code escribe el encargo para Codex con pruebas de regresión, como con H-01 a H-06 |
| **Seguridad:** el alta pública ofrece «Propietario» | Cualquiera puede registrarse sola como administrador y entrar a reportes y eventos (RN-34 solo verifica ese perfil) | Encargo a Codex para que el alta cree solo clientes |
| **Logout:** ninguna pantalla cierra la sesión | «Volver al inicio» no llama a `GestorSesion.Cerrar()`. También es parte de T02 | Encargo a Codex, chico; se puede juntar con U-01 |
| T01 a T08 | Según la respuesta del docente | Ver el orden propuesto en `ANEXOS-TECNICOS-T01-T08.md` |

## Pendiente de JC

| Pendiente | Por qué es de JC |
|---|---|
| Revisar `git status` y los commits de este turno | Regla común |
| Fuente del documento: esperar a Máximo hasta el 08/11; después, plan B2 | Decisión tomada; falta la respuesta de Máximo |
| Consultar a Máximo sobre RN-35 y avisarle del rediseño de FrmCompraCliente | Coordinación |
| Enviar el mail al docente sobre T01 a T08 | Coordinación |
| Datos de la carátula (formato al final de `CORRECCIONES-DOCUMENTO.md`) | Datos personales |
| JC-R01 a mano (125 % y 150 %), JC-R10 con el celular, CX-02 por reloj | Necesitan a una persona |
| Iniciar sesión en Word con la cuenta de la UAI | Hace falta para automatizar Word |

## Bloqueado

| Qué | Bloqueado por |
|---|---|
| Aplicar `CORRECCIONES-DOCUMENTO.md` | El fuente del documento (Máximo, hasta el 08/11) o el plan B2 |
| Próximo turno de Codex | Las decisiones pendientes de JC |
| Push a `origin` | Que JC hable con Máximo |

## Qué debería hacer el próximo agente

**Codex**: nada hasta que JC decida y Claude Code escriba el encargo.

**Claude Code**, cuando JC decida:
1. escribir el encargo para Codex (prioridades 1 a 3 de usabilidad, alta sin «Propietario»,
   logout), con un CP por cada corrección;
2. si llega el fuente del documento, aplicar `CORRECCIONES-DOCUMENTO.md`.
