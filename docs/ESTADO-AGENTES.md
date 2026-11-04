# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo;
2. confirma que el turno es suyo;
3. corre `git status`.

Si el turno no es suyo, o si hay cambios sin commitear que no son de su sesión, no toca nada
y avisa a JC.

Última actualización: **04/11/2026, a la tarde**, por Claude Code, al cerrar su turno.

---

## Turno

| | |
|---|---|
| **Turno actual** | **JC**: revisar `git status` y los commits de este turno |
| Último turno cerrado | Claude Code, 04/11/2026: figuras del instructivo, plan B del documento, escalado, QR, lista única de correcciones |
| Próximo turno | **Codex**: T-21, alinear la semilla con el ejemplo del documento |
| Después | **Claude Code**: regenerar las figuras con la semilla nueva |
| Rama y publicación | `main`, adelantada respecto de `origin`; **sin push** hasta que JC hable con Máximo |

## Quién hace qué

| Quién | Rol | Zona que modifica |
|---|---|---|
| JC (Juan Cruz Riggio) | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push | Todo, a mano |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |
| Codex | Implementación y pruebas automáticas | `Visual/`, `Repo/`, `Datos/`, `Pruebas/`; `BaseDatos/` solo en T-21, con autorización de JC |

Los dos agentes trabajan hacia el mismo entregable, la entrega final del Trabajo de Campo a
principios de diciembre de 2026, y cada uno lee la salida del otro. Codex deja sus reportes
en `Pruebas/` y Claude Code los mueve a `docs/`.

## Tareas por agente

### Codex

| Tarea | Estado | Dónde |
|---|---|---|
| T-01 a T-18: CP-11 a CP-42 | Terminada, commit `5eabe41` | [RESULTADOS-TAREAS-CODEX-2026-11-04.md](RESULTADOS-TAREAS-CODEX-2026-11-04.md) |
| T-19: preparación de CP-34 | Terminada, commit `3af4154`. Era un artefacto del encargo, no un defecto | [RESULTADOS-TAREAS-CODEX-2026-11-04-T19-T20.md](RESULTADOS-TAREAS-CODEX-2026-11-04-T19-T20.md) |
| T-20: el plazo de la reserva sale de `ReservaVence`; CP-43 | Terminada, commit `3af4154`. 49 de 49 | Ídem |
| **T-21: la semilla reproduce el ejemplo del documento** («Duki en Rosario», clienta Plata con 4 compras) | **Lista para ejecutar.** Modifica `BaseDatos/02-semilla.sql`, con autorización de JC | [TAREAS-CODEX-PRUEBAS.md](TAREAS-CODEX-PRUEBAS.md), «Tercera tanda» |

### Claude Code

| Tarea | Estado |
|---|---|
| Figuras 1 a 4 del instructivo, con ruta neutra | Terminadas en `docs/capturas/instructivo/`. **Se regeneran después de T-21** |
| Plan B del documento (evaluación, sin ejecutar) | Terminada: [DOCUMENTO-ENTREGABLE.md](DOCUMENTO-ENTREGABLE.md). Recomendación: esperar a Máximo; si no aparece, B2 (solo las páginas que cambian, de 6 a 9 h) |
| Lista única de correcciones del documento | Terminada: [CORRECCIONES-DOCUMENTO.md](CORRECCIONES-DOCUMENTO.md). Tiene 22 ítems por página; la matriz quedó en 27 de 38 reglas y 43 casos |
| Escalado real (JC-R01) | Verificado a 100 % real. 125 %, 150 % y 175 % esperan que JC cambie la escala; los pasos están en `PRUEBAS-PENDIENTES.md` |
| QR (JC-R10) | Medidos: cumplen ISO/IEC 18004 y no hay que agrandarlos. Falta el escaneo con el celular |
| Mail al docente sobre T01 a T08 | Redactado en el informe del turno, para que JC lo revise y lo envíe |
| Regenerar figuras y actualizar el LEEME | **En espera** de T-21 |

## Pendiente de JC

| Pendiente | Por qué es de JC |
|---|---|
| Revisar `git status` y los commits de este turno | Regla común |
| Darle el turno a Codex para T-21 | Coordinación |
| Pedirle a Máximo el fuente del documento, el script de armado y las partes; consultarle RN-35 y avisarle del rediseño de FrmCompraCliente | Coordinación con Máximo. Si no responde, decidir el plan B (ver `DOCUMENTO-ENTREGABLE.md`) |
| Enviar el mail al docente sobre los anexos técnicos T01 a T08 | Coordinación con el docente |
| Pasar los datos de la carátula (formato al final de `CORRECCIONES-DOCUMENTO.md`) | Datos personales |
| Iniciar sesión en Word con la cuenta de la UAI | Hace falta para que Word convierta y guarde por automatización |
| Escalas 125 %, 150 % y 175 % para JC-R01: cambiar la escala y avisar, o correr el script | Cambia la configuración de la PC |
| Escanear dos tickets con el celular (JC-R10) y la prueba por reloj de la reserva (CX-02) | Necesitan a una persona |
| Las 5 sesiones reales de usabilidad | Necesitan participantes |

## Bloqueado

| Qué | Bloqueado por |
|---|---|
| Aplicar `CORRECCIONES-DOCUMENTO.md`, incluido el anexo A02 | El fuente del documento (Máximo) o la decisión de JC sobre el plan B |
| Figuras definitivas del instructivo | T-21, de Codex |
| Cerrar JC-R01 | Que JC cambie la escala de Windows |
| Push a `origin` | Que JC hable con Máximo |

## Qué debería hacer el próximo agente

**Codex**, cuando JC le dé el turno:
1. leer este archivo y la «Tercera tanda» de `TAREAS-CODEX-PRUEBAS.md`;
2. hacer T-21;
3. parar y dejar el reporte en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-T21.md`.

**Claude Code**, en su próximo turno:
1. mover ese reporte a `docs/`;
2. correr `tools/CapturasInstructivo/Generar.ps1` y revisar que las figuras muestren «Duki
   en Rosario» y $99.625;
3. actualizar el LEEME de las capturas, el ítem 9 de `CORRECCIONES-DOCUMENTO.md` y este
   archivo.
