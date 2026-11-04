# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo;
2. confirma que el turno es suyo;
3. corre `git status`.

Si el turno no es suyo, o si hay cambios sin commitear que no son de su sesión, no toca nada
y avisa a JC.

Última actualización: **04/11/2026**, por Claude Code, al cerrar su turno.

---

## Turno

| | |
|---|---|
| **Turno actual** | **JC**: revisar `git status` y estos commits; decidir T-20 |
| Último turno cerrado | Claude Code, 04/11/2026: coordinación, diagnóstico de CP-34, documentación y matriz |
| Próximo turno | **Codex**: segunda tanda, T-19 y T-20 si se aprueba |
| Rama y publicación | `main`, adelantada respecto de `origin`; **sin push** hasta que JC hable con Máximo |

## Quién hace qué

| Quién | Rol | Zona que modifica |
|---|---|---|
| JC (Juan Cruz Riggio) | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push | Todo, a mano |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |
| Codex | Implementación y pruebas automáticas | `Visual/`, `Repo/`, `Datos/`, `Pruebas/` |

Los dos agentes trabajan hacia el mismo entregable, la entrega final del Trabajo de Campo a
principios de diciembre de 2026, y cada uno lee la salida del otro. Codex deja sus reportes
en `Pruebas/` y Claude Code los mueve a `docs/`.

## Tareas por agente

### Codex

| Tarea | Estado | Dónde |
|---|---|---|
| T-01 a T-18: CP-11 a CP-42 | **Terminada**, commit `5eabe41` | [RESULTADOS-TAREAS-CODEX-2026-11-04.md](RESULTADOS-TAREAS-CODEX-2026-11-04.md) |
| T-19: corregir la preparación de CP-34 y quitar el `Ignore` | **Lista para ejecutar** | [TAREAS-CODEX-PRUEBAS.md](TAREAS-CODEX-PRUEBAS.md), «Segunda tanda» |
| T-20: una sola fuente para el plazo de la reserva (modifica `Repo/`) | **Esperando aprobación de JC** | Ídem |

### Claude Code

| Tarea | Estado |
|---|---|
| Matriz de trazabilidad v2.5, texto para el Word | Terminada: [MATRIZ-TRAZABILIDAD-v2.5.md](MATRIZ-TRAZABILIDAD-v2.5.md), con 38 reglas y CP-34 sin contar |
| CP-11 a CP-42 en `docs/04-casos-prueba.md` | Terminada |
| Pendientes de Codex en `docs/PRUEBAS-PENDIENTES.md` | Terminada (CX-01 y CX-02) |
| Actualizar la matriz cuando Codex cierre T-19 o T-20 | **En espera** de Codex: los cambios están listos en la sección 12 de la matriz |

## Pendiente de JC

| Pendiente | Por qué es de JC |
|---|---|
| Revisar `git status` y los commits de este turno | Regla común |
| Aprobar o descartar T-20 | Cambia `Repo/` en el flujo de pago a un mes de la entrega |
| Pasar la matriz v2.5 y RN-38/RN-39 al Word del documento | El repositorio solo tiene el PDF |
| Confirmar con Máximo RF-09 como Won't y avisarle del rediseño de FrmCompraCliente, antes del push | Coordinación con Máximo |
| Preguntar al docente si los anexos técnicos T01 a T08 son obligatorios en esta entrega | Coordinación con el docente |
| Completar carátula: comisión, turno, legajos y año | Datos personales |
| Pruebas manuales: compra al 125 % y 150 % reales (JC-R01), QR con el celular (JC-R10), recorrido de formularios (JC-R11), reserva vencida por reloj (CX-02) | Necesitan a una persona |
| Decidir si RN-35, RN-06 y RN-29 se actualizan en el documento | Sección 11 de la matriz |

## Bloqueado

| Qué | Bloqueado por |
|---|---|
| CP-34 cuenta como aprobada (CX-01) | T-19, de Codex |
| Cifras finales de la matriz (71,1 % de reglas) | T-19 |
| Push a `origin` | Que JC hable con Máximo |
| Cambios en el documento entregable | El Word editable, que no está en el repositorio |

## Qué debería hacer el próximo agente

**Codex**, cuando JC le dé el turno:
1. leer este archivo y la «Segunda tanda» de `TAREAS-CODEX-PRUEBAS.md`;
2. hacer T-19;
3. hacer T-20 solo si JC la aprobó de forma explícita;
4. parar y dejar el reporte en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>.md`.

**Claude Code**, en su próximo turno:
1. mover ese reporte a `docs/`;
2. aplicar la sección 12 de la matriz si CP-34 pasa;
3. actualizar este archivo.
