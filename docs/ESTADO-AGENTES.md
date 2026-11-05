# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo;
2. confirma que el turno es suyo;
3. corre `git status`.

Si el turno no es suyo, o si hay cambios sin commitear que no son de su sesión, no toca nada
y avisa a JC.

Última actualización: **05/11/2026**, por Claude Code, al cerrar su turno.

---

## Turno

| | |
|---|---|
| **Turno actual** | **JC**: decidir V-1 a V-6 de `docs/PLAN-RENOVACION-VISUAL.md` |
| Próximo turno | **Claude Code**: escribir el encargo de la fase 0 (tema) para Codex, según lo que decida JC |
| Después | **Codex**: fase 0 de la renovación visual |
| Ramas | `main` local = `main`, `JC` y `respaldo/jc-2026-11-04` en GitHub. Push autorizado por JC el 05/11 |

**Codex no tiene tareas pendientes.** La quinta tanda está terminada (73 de 73).

---

## La foto: dónde estamos

### Código: cerrado

| Qué | Estado |
|---|---|
| Funcionalidad del sistema (secciones 1 a 10) | Completa |
| Correcciones de las pruebas manuales de JC (F-01 a F-03, O-01 a O-11) | Hechas, con prueba automática cada una |
| Seguridad: alta sin elegir perfil (T-22), cierre de sesión (T-23) | Hechas |
| Cerrar con la X termina la aplicación (T-24); base de formularios editable (T-28) | Hechas y verificadas sobre el ejecutable |
| Compra: indicación de la cantidad, «Comprar» sin entradas, encabezado completo (T-25, T-26, T-29) | Hechas |
| Manejador global de errores; pago con el evento eliminado (T-27) | Hechos y verificados con SQL Server detenido |
| Pruebas automáticas | **61 casos, 73 ejecuciones, todas correctas** |
| Cobertura de la matriz | 12 de 12 requerimientos en alcance; **33 de 38 reglas (86,8 %)**, con las 19 que rechazan operaciones |
| Usabilidad | Criterio del 70 % cumplido (100 %); SUS 78,8; 10 hallazgos menores abiertos (12 h) |

**Lo único que cambiaría el código de acá a la entrega es la renovación visual**, si JC la
aprueba (`docs/PLAN-RENOVACION-VISUAL.md`).

### Pruebas manuales: lo que le queda a JC de verdad

| ID | Qué | Tiempo |
|---|---|---|
| JC-R10 | Escanear dos tickets con el celular | 10 min |
| CX-03 | Abrir `FrmCompraCliente` y `frmInicio` en el diseñador de Visual Studio (o lo hace Máximo) | 5 min |
| (opcional) | Shift+Tab en el login y Ctrl+Tab en una grilla, con el teclado físico | 5 min |
| (opcional) | Mirar la compra en una notebook a 125 % o 150 % | 10 min |

**Total: 15 minutos obligatorios.** Claude Code cerró el 05/11 JC-R01 (escala real a 125 %,
150 % y 175 %), JC-R11 (recorrido con teclado), CX-02 (reserva vencida por reloj) y CX-04
(base caída). Detalle en `docs/PRUEBAS-PENDIENTES.md`, «Estado al 05/11/2026».

### Lo que depende de Máximo

| Qué | Para cuándo | Si no llega |
|---|---|---|
| El fuente editable del documento | **08/11** | El 09/11 arranca el plan B2 (`DOCUMENTO-ENTREGABLE.md`) |
| Sus 64 pruebas manuales (propietario, reportes, suscripción) | 25/11 | Buena parte tiene prueba automática (CP-21, CP-35 a CP-38, CP-56, CP-58, CP-60); lo visual queda sin mirar |
| Confirmar RN-35 reescrita (prioridad de suscriptores en la lista de espera) | Antes de aplicar las correcciones del documento | Queda como está en `docs/` |
| CX-03, si tiene Visual Studio | Cuando pueda | Lo hace JC |

### Lo que depende del docente

| Qué | Por qué importa |
|---|---|
| Si pide los anexos T01 a T08 | Completos son 100 a 144 h: no entran junto con la renovación. El subconjunto mínimo (T01, T02, T03 y T06a) son 30 a 42 h |
| Que acepte el rótulo «(simulado)» del estudio de usabilidad | El documento ya lo declara así; mejor saberlo antes de la defensa |

**El mail al docente conviene que salga esta semana**: su respuesta define cuánto tiempo
queda para la renovación.

### La última semana (26/11 al 04/12)

1. **27/11: congelamiento del código.** Desde ahí, solo correcciones.
2. Figuras definitivas (`Generar.ps1`) y, si hubo renovación, la tercera medición de
   usabilidad cerrada.
3. Aplicar al documento las últimas cifras de `CORRECCIONES-DOCUMENTO.md`, regenerar el
   índice y exportar el PDF.
4. Última batería, último push a `main`.
5. Ensayo de la defensa con `COMO-SE-PROBO.md` y `EXCEPCIONES.md`.

El cronograma semana por semana está en `docs/PLAN-RENOVACION-VISUAL.md`, sección 10.

---

## Quién hace qué

| Quién | Rol | Zona que modifica |
|---|---|---|
| JC (Juan Cruz Riggio) | Decide el alcance, habla con el docente y con Máximo, hace las pruebas manuales, revisa `git status` y autoriza commits y push | Todo, a mano |
| Claude Code | Revisión, documentación, matriz de trazabilidad, planificación y encargos para Codex | `docs/`, `tools/`, `CLAUDE.md`, `AGENTS.md` |
| Codex | Implementación y pruebas automáticas | `Visual/`, `Repo/`, `Datos/`, `Pruebas/`; `BaseDatos/` y Designer solo con autorización de JC |

## Decisiones pendientes de JC

| # | Decisión | Recomendación de Claude Code |
|---|---|---|
| V-1 | Camino de la renovación visual: A (estándar con sistema de diseño), B (biblioteca) o C (dibujado) | A |
| V-2 | Compatibilidad con DPI por el manifiesto, sin tocar `App.config` | Sí, en la fase 0 |
| V-3 | Tercera medición de usabilidad después de la renovación | Sí |
| V-4 | Rediseño del ticket PDF | Sí |
| V-5 | Corregir los callejones del teclado (T-1 a T-5) dentro de la renovación | Sí |
| V-6 | Títulos de los avisos en castellano (U-03) | Sí |
| — | T01 a T08 | Según la respuesta del docente |

La renovación toca los Designer de las 19 pantallas: **necesita la autorización de JC para
que Codex los edite.**

## Observaciones de este turno

- **El aviso del pago con el evento eliminado es genérico**: «La compra ya no está pendiente de
  pago». Es correcto pero no dice que el evento se eliminó. Es un cambio de texto en `frmPago`,
  si JC lo quiere.
- **La escala de Windows:** el primer intento de cambiarla (con `SPI_SETLOGICALDPIOVERRIDE`)
  dejó guardado 125 % sin aplicarlo. Se restauró a 100 % antes de seguir. Queda la clave
  `HKCU\Control Panel\Desktop\PerMonitorSettings\<monitor>` con `DpiValue = 0`, que significa
  «la recomendada» (100 %); antes no existía. El efecto es el mismo.
- **`Generar.ps1` necesita `pymupdf`**, instalado en `D:\jc-tools`: correr con
  `$env:PYTHONPATH = 'D:\jc-tools'`.

## Qué debería hacer el próximo agente

**Claude Code**, cuando JC decida V-1 a V-6:
1. escribir en `docs/TAREAS-CODEX-PRUEBAS.md` la «Sexta tanda»: la fase 0 del plan
   (`Tema.cs`, la base aplica el tema, componentes, ícono, una sola escala de diseño, y DPI si
   JC aprueba V-2), con sus pruebas y la regla de no renombrar controles;
2. pasarle el turno a Codex a través de JC.

**Codex**, cuando JC le dé el turno: la sexta tanda, y parar al terminarla.
