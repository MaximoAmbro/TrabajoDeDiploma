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
| **Turno actual** | **JC**: revisar los commits de este turno |
| Próximo turno | **Codex**: quinta tanda, T-27 a T-29, en `docs/TAREAS-CODEX-PRUEBAS.md` |
| Después | **Claude Code**: verificar T-27 sobre la aplicación con la base caída y regenerar las figuras después de T-29 |
| Ramas | `main` local con todo. Subidas: `respaldo/jc-2026-11-04` (copia de `main`) y `JC` (avance directo autorizado por JC). **`main` de GitHub no se toca** hasta que Máximo confirme si tiene trabajo sin subir |

## Estudio de usabilidad: definitivo, con antes y después

La dependencia anotada en el turno anterior se cumplió:
1. **Codex corrigió**: T-22 a T-26, con 60 de 60.
2. **Claude Code volvió a medir** con el mismo método.
3. **El informe quedó definitivo**, con las dos mediciones.

| Métrica | Antes | Después |
|---|---|---|
| Perfiles sin asistencia | 4 de 6 | 6 de 6 |
| Satisfacción buena o muy buena (umbral 70 %) | 66,7 % — no alcanzaba | **100 % — cumple** |
| SUS | 70,0 | 78,8 |
| Hallazgos abiertos | 13 (2 mayores) | 11 (ninguno mayor; incluye U-14, nuevo) |

La segunda medición encontró **U-14**: el encabezado «Cantidad (escribí aquí)» se corta a
100 %. Está encargado como T-29. Las figuras del instructivo se regeneran una vez más después
de T-29.

## Quién hace qué

| Quién | Rol | Zona que modifica |
|---|---|---|
| JC (Juan Cruz Riggio) | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push | Todo, a mano |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |
| Codex | Implementación y pruebas automáticas | `Visual/`, `Repo/`, `Datos/`, `Pruebas/`; `BaseDatos/` y Designer solo con autorización de JC |

## Tareas por agente

### Codex

| Tarea | Estado | Dónde |
|---|---|---|
| T-01 a T-26 | Terminadas. La última tanda está en los commits `272c864`, `309b9be` y `bb41f23` | `docs/RESULTADOS-TAREAS-CODEX-2026-11-04*.md` |
| **T-27: manejador global de errores; el pago con el evento eliminado** | **Lista para ejecutar.** Aprobada por JC | `TAREAS-CODEX-PRUEBAS.md`, «Quinta tanda» |
| **T-28: la base de formularios no abstracta** | Lista. Corrige un desvío de T-24: con la base abstracta, el diseñador de Visual Studio no puede abrir los formularios | Ídem |
| **T-29: el encabezado de la cantidad completo** | Lista. Corrige U-14, encontrado al volver a medir | Ídem |

Batería esperada al terminar: **66 de 66**, con los casos CP-51 a CP-54.

### Claude Code

| Tarea | Estado |
|---|---|
| Revisión y commits de T-22 a T-26 | Terminado |
| Nueva medición de la usabilidad, con antes y después | Terminada: `docs/usabilidad/` |
| Lista de correcciones del documento con las cifras finales y CP-44 a CP-50 | Terminada |
| Encargo T-27 a T-29 | Terminado |
| Verificar T-27 con la base caída (`tools/Excepciones/`, ajustando el programa para que instale el manejador) | **En espera** de Codex |
| Regenerar las figuras después de T-29 | **En espera** de Codex |

## Decisiones pendientes de JC

| Decisión | Contexto |
|---|---|
| Push a `main` de GitHub | Esperando que Máximo confirme si tiene trabajo sin subir |
| T-28 y T-29 | No estaban en tu lista; las incluí en la quinta tanda porque corrigen desvíos de T-24 y T-25. Si no las querés, se sacan del encargo antes de darle el turno a Codex |
| T01 a T08 | Según la respuesta del docente |

## Pendiente de JC

| Pendiente | Por qué es de JC |
|---|---|
| Revisar los commits de este turno y darle el turno a Codex | Regla común |
| Hablar con Máximo: trabajo sin subir, RN-35, rediseño de FrmCompraCliente, sus 64 pruebas, y abrir el diseñador después de T-28 (CX-03) | Coordinación |
| Fuente del documento: esperar a Máximo hasta el 08/11; después, plan B2 | Decisión tomada |
| Mail al docente sobre T01 a T08 | Coordinación |
| Datos de la carátula | Datos personales |
| JC-R01, JC-R10, JC-R11, CX-02 y, después de T-27, CX-04 (base caída) | Necesitan a una persona |

## Bloqueado

| Qué | Bloqueado por |
|---|---|
| Figuras definitivas del instructivo | T-29 |
| Verificar el manejador global sobre la aplicación | T-27 |
| Aplicar `CORRECCIONES-DOCUMENTO.md` | El fuente del documento (Máximo, hasta el 08/11) o el plan B2 |
| Push a `main` | Que Máximo confirme |

## Qué debería hacer el próximo agente

**Codex**, cuando JC le dé el turno:
1. leer este archivo y la «Quinta tanda» de `TAREAS-CODEX-PRUEBAS.md`;
2. hacer T-27, T-28 y T-29;
3. parar y dejar el reporte en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-T27-T29.md`.

**Claude Code**, después:
1. mover ese reporte a `docs/`;
2. ajustar `tools/Excepciones/ErrorNoControlado.cs` para que llame a
   `ManejadorErrores.Instalar()` y verificar el mensaje en castellano con la base caída;
3. regenerar las figuras del instructivo y verificar el encabezado completo;
4. actualizar `EXCEPCIONES.md`, `04-casos-prueba.md` (CP-51 a CP-54), `COMO-SE-PROBO.md`,
   la lista de correcciones y este archivo.
