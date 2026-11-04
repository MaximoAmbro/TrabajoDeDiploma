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
| **Turno actual** | **JC**: revisar `git status` y los commits de este turno |
| Próximo turno | **Codex**: cuarta tanda, T-22 a T-26, en `docs/TAREAS-CODEX-PRUEBAS.md` |
| Después | **Claude Code**: volver a medir la usabilidad sobre la aplicación corregida (ver «Dependencia») |
| Rama y publicación | `main`, adelantada respecto de `origin`; **sin push** hasta que JC hable con Máximo |

## ⚠ Dependencia que no se puede saltear

**El estudio de usabilidad (`docs/usabilidad/`) describe la interfaz anterior a T-22 a T-26.**
Cuando Codex aplique esas correcciones, cambian las mediciones:
- las 18 acciones y los 15 clics de la tarea;
- los hallazgos U-01, U-04 y U-05, que pasan a corregidos;
- los tiempos, la satisfacción (66,7 % y 4,08) y el SUS (70,0) de los perfiles.

El orden obligatorio es:
1. **Codex** aplica T-22 a T-26 y la batería da 60 de 60.
2. **Claude Code** vuelve a medir:
   - `tools/Usabilidad/Medir.ps1`;
   - `VerificarCierreConX.ps1`, que tiene que informar «proceso vivo = False»;
   - `modelo.py`, ajustando las reglas a los hallazgos corregidos.

   Con eso actualiza el informe y los ítems de `CORRECCIONES-DOCUMENTO.md` que citan el
   estudio: 3, 4, 11b, 18, 19, 20 y 21.
3. Recién entonces el informe es **definitivo** y se puede pasar al documento.

El informe y el ítem 11b de la lista de correcciones tienen un aviso en este mismo sentido.

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
| T-01 a T-21 | Terminadas | `docs/RESULTADOS-TAREAS-CODEX-2026-11-04*.md` |
| **T-22: el alta pública crea solo clientes** (seguridad; autoriza cambiar el Designer de `frmCrearUsuario`) | **Lista para ejecutar**, prioridad 1 | `TAREAS-CODEX-PRUEBAS.md`, «Cuarta tanda» |
| **T-23: «Volver al inicio» cierra la sesión** | Lista, prioridad 2 | Ídem |
| **T-24: la X no deja la aplicación corriendo; confirmar antes de cerrar la compra** | Lista, prioridad 3 | Ídem |
| **T-25: indicar dónde se escribe la cantidad** | Lista, prioridad 4 | Ídem |
| **T-26: «Comprar» y el seguro deshabilitados sin entradas** (con un ajuste autorizado de CP-24) | Lista, prioridad 5 | Ídem |

Batería esperada al terminar: **60 de 60**, con los casos nuevos CP-44 a CP-50.

### Claude Code

| Tarea | Estado |
|---|---|
| Encargo T-22 a T-26 | Terminado |
| Historia de las pruebas, para la defensa | Terminada: `docs/COMO-SE-PROBO.md` |
| Manejo de excepciones, diseño y pruebas, para la defensa | Terminada: `docs/EXCEPCIONES.md`. Error no controlado **verificado** sobre la aplicación: llega el cuadro genérico de .NET |
| Volver a medir la usabilidad | **En espera** de T-22 a T-26 |

## Decisiones pendientes de JC

| Decisión | Contexto |
|---|---|
| **Manejador global de errores** y captura de `InvalidOperationException` en el pago | Verificado: un error de base de datos llega al usuario como «Excepción no controlada en la aplicación», en inglés y con botones Detalles, Continuar y Salir. Además, si se elimina un evento mientras un cliente paga, el pago termina en ese mismo cuadro. Menos de 2 horas en `Visual/`. Detalle en `docs/EXCEPCIONES.md`. **No está encargado** |
| T01 a T08 | Según la respuesta del docente; ver `ANEXOS-TECNICOS-T01-T08.md` |

## Pendiente de JC

| Pendiente | Por qué es de JC |
|---|---|
| Revisar `git status` y los commits de este turno, y darle el turno a Codex | Regla común |
| Fuente del documento: esperar a Máximo hasta el 08/11; después, plan B2 | Decisión tomada |
| Consultar a Máximo sobre RN-35, avisarle del rediseño de FrmCompraCliente y recordarle sus 64 pruebas manuales | Coordinación (ver `docs/COMO-SE-PROBO.md`) |
| Mail al docente sobre T01 a T08 | Coordinación |
| Datos de la carátula | Datos personales |
| JC-R01 a mano, JC-R10 con el celular, JC-R11 con el teclado, CX-02 por reloj | Necesitan a una persona |
| Iniciar sesión en Word con la cuenta de la UAI | Para automatizar Word |

## Bloqueado

| Qué | Bloqueado por |
|---|---|
| Informe de usabilidad definitivo, y los ítems de la lista de correcciones que lo citan | T-22 a T-26 (Codex), y después la nueva medición (Claude Code) |
| Aplicar `CORRECCIONES-DOCUMENTO.md` | El fuente del documento (Máximo, hasta el 08/11) o el plan B2 |
| Push a `origin` | Que JC hable con Máximo |

## Qué debería hacer el próximo agente

**Codex**, cuando JC le dé el turno:
1. leer este archivo y la «Cuarta tanda» de `TAREAS-CODEX-PRUEBAS.md`;
2. hacer T-22 a T-26 en orden;
3. parar y dejar el reporte en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-T22-T26.md`.

**Claude Code**, después:
1. mover ese reporte a `docs/`;
2. hacer la nueva medición de la sección «Dependencia»;
3. actualizar el informe de usabilidad, la lista de correcciones, `docs/COMO-SE-PROBO.md`
   (CP-44 a CP-50) y `docs/04-casos-prueba.md`.
