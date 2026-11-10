# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo;
2. confirma que el turno es suyo;
3. corre `git status`.

Si el turno no es suyo, o si hay cambios sin commitear que no son de su sesión, no toca nada
y avisa a JC.

Última actualización: **05/11/2026**, por Claude Code, al cerrar su turno (encargo de la parte 6A y sistema de diseño).

---

## Turno

| | |
|---|---|
| **Turno actual** | **JC**: revisar `git status` del encargo y del sistema de diseño, commitear y darle el turno a Codex |
| Próximo turno | **Codex**: sexta tanda, **solo la parte 6A** (`TAREAS-CODEX-PRUEBAS.md`): tema, componentes, DPI, ícono, ticket nuevo y el piloto `FrmCompraCliente`. Y **PARAR** |
| Después | **Claude Code**: capturas del piloto, QR del ticket nuevo y figuras. **Máximo**: abre el piloto en Visual Studio. **JC**: aprueba y escanea el ticket |
| Después de eso | **Codex**: parte 6B (los otros 18), solo si este archivo dice **«piloto confirmado»** |
| Ramas | `main` local = `main`, `JC` y `respaldo/jc-2026-11-04` en GitHub hasta `98be355`. Los cambios de este turno están **sin commitear**, para que JC los revise |

**Estado del piloto: pendiente.** Esta línea la cambia Claude Code a «piloto confirmado» cuando
JC lo indique.

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
| JC-R10 | Escanear dos tickets con el celular. **Esta PC no tiene cámara** (revisado el 05/11): la lectura óptica no se pudo hacer desde acá y no se simuló. **Esperar al ticket nuevo (V-4, parte 6A)**: Claude Code avisa cuando esté | 10 min |
| (opcional) | Shift+Tab en el login y Ctrl+Tab en una grilla, con el teclado físico | 5 min |
| (opcional) | Mirar la compra en una notebook a 125 % o 150 % | 10 min |

**Total: 10 minutos obligatorios.** CX-03 pasó a Máximo (ver abajo). Claude Code cerró el 05/11 JC-R01 (escala real a 125 %,
150 % y 175 %), JC-R11 (recorrido con teclado), CX-02 (reserva vencida por reloj) y CX-04
(base caída). Detalle en `docs/PRUEBAS-PENDIENTES.md`, «Estado al 05/11/2026».

### Lo que depende de Máximo

| Qué | Para cuándo | Si no llega |
|---|---|---|
| El fuente editable del documento | **08/11** | El 09/11 arranca el plan B2 (`DOCUMENTO-ENTREGABLE.md`) |
| Sus 64 pruebas manuales (propietario, reportes, suscripción) | 25/11 | Buena parte tiene prueba automática (CP-21, CP-35 a CP-38, CP-56, CP-58, CP-60); lo visual queda sin mirar |
| Confirmar RN-35 reescrita (prioridad de suscriptores en la lista de espera) | Antes de aplicar las correcciones del documento | Queda como está en `docs/` |
| **Verificar el piloto en Visual Studio** (cierra también CX-03): abrir `FrmCompraCliente` después de la parte 6A. Mensaje para mandarle: «Mensaje para Máximo», abajo | **08/11**: la parte 6B no arranca sin esto | La renovación se frena en el piloto |

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

## Decisiones de JC del 05/11

**Tomadas:** V-1 a V-6, todas como se recomendaron: camino A con `Tema.cs`, DPI en la fase 0,
tercera medición, ticket nuevo, callejones del teclado y títulos en castellano. **Autorizó a
Codex a editar los Designer de las 19 pantallas, primero uno (el piloto).** V-4 va al
principio.

**Pendiente de JC:** aprobar `docs/SISTEMA-DISENO.md` (la sección 7 dice qué no da WinForms y
qué se hace en cambio) y, según el docente, T01 a T08.

## Mensaje para Máximo (verificación del piloto)

> Hacé pull de `main`, compilá la solución y abrí en Visual Studio el diseñador de
> `Visual/Cliente/FrmCompraCliente.cs`: decime si se dibuja sin la página de error y mandame
> una captura.
> Antes de abrirlo, poné Windows al 100 % (o aceptá «reiniciar con escala al 100 %» si Visual
> Studio lo ofrece), y cerralo **sin guardar**, aunque te pregunte.

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

**Codex**, cuando JC le dé el turno: la **parte 6A** de la sexta tanda
(`TAREAS-CODEX-PRUEBAS.md`), con `docs/SISTEMA-DISENO.md` como referencia. Batería esperada:
77. Reporte en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-6A.md`. **Parar ahí: la 6B espera la
confirmación del piloto.**

**Claude Code**, después de 6A:
1. mover el reporte a `docs/` y revisar el código;
2. **adaptar sus herramientas a la compatibilidad con DPI.** `Captura.exe` (figuras y
   `VerificarEscala`) se compila sin manifiesto y no escalaría como la aplicación: tiene que
   declararse DPI-aware igual que `Visual.exe`;
3. capturas del piloto a 100 %, 125 % y 150 % (`Escala.ps1`), para que JC las apruebe como
   referencia;
4. medir el QR del ticket nuevo (decodificación con `VerificarQrTickets.py`, más módulo y zona
   de silencio sobre el PDF) y **avisarle a JC que ya puede escanear**;
5. regenerar las figuras del instructivo;
6. con el «sí» de Máximo y de JC, anotar acá «piloto confirmado».
