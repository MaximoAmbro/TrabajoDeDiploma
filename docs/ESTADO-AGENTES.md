# Estado de los agentes

**Lo escribe solo Claude Code.** Codex lo lee al empezar y reporta sus cambios de estado a JC,
que se los pasa a Claude Code para que los anote. JC puede corregirlo cuando quiera.

Antes de empezar cualquier tarea, cada agente:
1. lee este archivo;
2. confirma que el turno es suyo;
3. corre `git status`.

Si el turno no es suyo, o si hay cambios sin commitear que no son de su sesión, no toca nada
y avisa a JC.

Última actualización: **10/11/2026**, por Claude Code. Cerró la planificación posterior a 6A:
borradores de 6B y de la séptima tanda, clasificación de las 64 y cronograma re-fechado.

---

## Turno

| | |
|---|---|
| **Turno actual** | **JC**: revisar `git status` de la planificación del 10/11 y mandar el mensaje a Máximo (abajo) |
| Próximo turno | **Claude Code**: lo de después de 6A (lista al final de este archivo) |
| Después | **Codex**: **séptima tanda** (`TANDAS-PRUEBAS-64.md`), solo cuando JC la apruebe y la pase a `TAREAS-CODEX-PRUEBAS.md` |
| Después de eso | **Codex**: parte 6B (`ENCARGO-6B-BORRADOR.md`), solo si este archivo dice **«piloto confirmado»** |
| Ramas | `main` local = `main` y `JC` en GitHub hasta `36a0d80` (6A). `respaldo/jc-2026-11-04` quedó en `98be355`. Los cambios de este turno están **sin commitear** |

**Codex: 6A está cerrada (77/77) y no hay encargo vigente para vos.** Los dos borradores de
`docs/` no son encargos hasta que JC los apruebe.

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
| El fuente editable del documento | Venció el 08/11; se le volvió a preguntar el 10/11 | Plan B2: conversión desde el 23/11 (`PLAN-RENOVACION-VISUAL.md`, sección 10) |
| ~~Sus 64 pruebas manuales~~ | **Pasaron a nosotros el 10/11** (decisión de JC) | `PRUEBAS-64-CLASIFICACION.md` y `TANDAS-PRUEBAS-64.md` |
| Confirmar RN-35 reescrita (prioridad de suscriptores en la lista de espera) | Antes de aplicar las correcciones del documento | Queda como está en `docs/`. La prueba PA-19 comprueba lo que el sistema hace hoy |
| **Verificar el piloto en Visual Studio** (cierra también CX-03): abrir `FrmCompraCliente` **y `frmEventosCliente`**. Mensaje: «Mensaje para Máximo», abajo | **15/11**: desde ahí, cada día de demora corre 6B | La renovación se frena en el piloto |

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

## Mensaje para Máximo (verificación del piloto y fuente del documento), del 10/11

> Máxi, te pido dos cosas cuando puedas:
>
> **1) Una prueba rápida en Visual Studio.** Hacé pull de `main` y compilá. Antes de abrir
> nada, poné Windows al 100 %. Si Visual Studio te ofrece «reiniciar con escala al 100 %»,
> aceptá.
>
> Abrí en el diseñador estas dos pantallas:
> - `Visual/Cliente/FrmCompraCliente.cs`, la de compra, que ya tiene el diseño nuevo;
> - `Visual/Cliente/frmEventosCliente.cs`, el listado de eventos.
>
> Para cada una decime si se dibuja normal o si sale una página de error o de advertencias.
> Si sale, copiame el texto que muestra. Mandame una captura de las dos.
>
> **Cerralas sin guardar**, aunque te pregunte. Vos trabajás al 150 % y, si guarda, Visual
> Studio reescribe todas las medidas de la pantalla y pisa el diseño nuevo. Después podés
> volver a tu escala.
>
> **2) ¿Conseguiste el Word editable del documento?** Necesito saberlo para organizar lo que
> queda.

Cómo se lee la respuesta: `ENCARGO-6B-BORRADOR.md`, sección 0.

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

**Codex:** nada hasta que JC apruebe la séptima tanda. 6A está terminada (reporte en
`Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-10-6A.md`).

**Claude Code**, después de 6A (pendiente; el 10/11 se hizo solo la planificación y se repitió
el host de diseño sobre la compilación de 6A: `docs/verificaciones/disenador-host-6A.txt`):
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
