# Segunda tanda de Codex — T-19 y T-20

> Reporte de Codex, movido desde `Pruebas/` por Claude Code sin cambiar el contenido. Los
> cambios que describe están en el commit `3af4154`. La batería se volvió a correr después
> del traslado: 49 de 49. El reporte de la primera tanda (T-01 a T-18) es
> [RESULTADOS-TAREAS-CODEX-2026-11-04.md](RESULTADOS-TAREAS-CODEX-2026-11-04.md).

Fecha: 04/11/2026. Base: `3aa6ca7`, rama `main`, árbol limpio al iniciar. JC concedió
el turno y aprobó expresamente T-20 en su mensaje. Se leyeron `ESTADO-AGENTES.md`,
la «Segunda tanda», `AGENTS.md` y `CLAUDE.md` antes de modificar archivos.

## Resultado y orden de ejecución

Se ejecutaron únicamente los dos pasos solicitados, en orden, con una batería completa
después de cada uno. T-19 se confirmó a JC antes de comenzar T-20.

| Paso | Resultado | Omisiones | Fallos | Código de salida |
|---|---|---:|---:|---:|
| T-19, antes de modificar Repo/ | 48 de 48 correctas | 0 | 0 | 0 |
| T-20 | 49 de 49 correctas | 0 | 0 | 0 |

Comando en ambos pasos:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar
```

Salida resumida final:

```text
La serie de pruebas se ejecutó correctamente.
Pruebas totales: 49
     Correcto: 49
Tiempo total: 5,2348 Segundos
```

Compilación sin errores ni advertencias nuevas de compilación. `git diff --check` no
informó errores de espacios. No se hicieron commits ni push. El trabajo terminó al
completar T-20; no se buscaron mejoras adicionales.

## Qué se hizo

### T-19 — preparación coherente de CP-34

- `Fake.RetrasarInicioDeCompra` ahora atrasa `FechaCompra` y `ReservaVence` en una sola
  sentencia parametrizada. Si `ReservaVence` es NULL, permanece NULL por la semántica de
  `DATEADD`; no se agregó un vencimiento a una reserva liberada.
- Se quitó el Ignore de `CP34_ReservaVencida` sin modificar ninguna aserción.
- CP-34 comprobó RN-15, compra Vencida, cinco entradas disponibles y ausencia de pagos.

Este paso pasó **antes** de cambiar la aplicación. Confirma el diagnóstico de Claude
Code: el fallo anterior de CP-34 surgía de una preparación que desalineaba las fechas.
El registro anterior del hallazgo debe conservarse como historial y cerrarse con esa
clasificación, no como un defecto de aplicación corregido por T-19.

### T-20 — vencimiento persistido como fuente de plazo

El único cambio en `Repo/` es la línea de retorno de `GestorCompras.VencimientoDe` y su
comentario, tal como estaba aprobado:

```csharp
return c.ReservaVence ?? c.FechaCompra.AddMinutes(MinutosReserva());
```

Se mantiene la guarda de argumento nulo. Cuando existe una reserva, se usa su plazo
persistido; cuando es NULL, se conserva el cálculo anterior para el reintento.
No se modificaron `Confirmar`, `frmPago`, repositorios de datos ni otros gestores.

En `Pruebas/`:

- CP-39 ahora prepara también `porVencer.ReservaVence` a partir de su `FechaCompra`
  ajustada y `RESERVA_MINUTOS`; conserva la verificación del último minuto en rojo.
- Se agregó `Fake.VencerReserva(idCompra)`, que actualiza solamente `ReservaVence` a
  un minuto antes de GETDATE(), con el ID parametrizado.
- Se agregó CP-43 con comentario RF-06/RN-15 y nombre visible. La prueba inicia una
  compra de dos entradas en un sector de capacidad cinco, vence únicamente la reserva
  en la base y confirma usando el objeto original en memoria. Comprueba RN-15, compra
  Vencida, devolución del cupo a cinco y ningún intento de pago.

| CP | Método | Resultado |
|---|---|---|
| CP-34 | `PruebasCompra.CP34_ReservaVencida` | Correcta, sin Ignore |
| CP-39 | `PruebasInterfaz.CP39_TemporizadorDeReservaALaVista` | Correcta, incluida la condición roja |
| CP-43 | `PruebasCompra.CP43_PlazoFijadoPorLaReservaPersistida` | Correcta |

No se cambiaron parámetros globales ni se esperó el plazo real durante las pruebas.
Los escenarios usan `BddEventAura_Pruebas` y los datos de Fake; no se prepararon mediante
cambios en la base de desarrollo ni en los datos semilla.

## Archivos tocados para revisión de JC

Modificados:

1. `Pruebas/Fake.cs`: corrección de RetrasarInicioDeCompra y nuevo VencerReserva.
2. `Pruebas/PruebasCompra.cs`: retiro de Ignore y nueva CP-43.
3. `Pruebas/PruebasInterfaz.cs`: una asignación adicional en CP-39.
4. `Repo/GestorCompras.cs`: exclusivamente el retorno y comentario de VencimientoDe.

Nuevo: `Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-04.md` (este reporte).

No hubo cambios en Designer, Visual/, Datos/, Entidade/, BaseDatos/, docs/, tools/,
CLAUDE.md ni AGENTS.md. Ambos App.config permanecen intactos y con skip-worktree (S).

## Qué quedó pendiente y fuera de la zona de Codex

- No quedaron fallos u omisiones en la batería, ni se descubrieron defectos nuevos en
  estas dos tareas. No hubo necesidad de ampliar la autorización de Repo/.
- Continúa pendiente **CX-02**, comprobación manual por reloj: iniciar un pago, dejar
  vencer el plazo y verificar temporizador, aviso, regreso al listado, estado Vencida,
  restitución del cupo y ausencia de pago. No se ejecutó esa espera en esta tanda.
- La documentación mantiene el estado anterior de CP-34 y T-20. No se modificó por
  pertenecer al turno de Claude Code.

## Qué debería hacer Claude Code después

1. Trasladar este reporte a docs/ sin sobrescribir el informe de la primera tanda que
   ya tiene la misma fecha. Por ejemplo, conservarlo como
   `RESULTADOS-TAREAS-CODEX-2026-11-04-T19-T20.md`.
2. Aplicar la sección 12 de `MATRIZ-TRAZABILIDAD-v2.5.md`: CP-34 aprobada, sin asterisco
   ni omisión; 27 de 38 reglas verificadas (71,1 %), once pendientes. CP-43 no suma otra
   regla porque RN-15 ya se cuenta con CP-34.
3. Registrar CP-43 en `docs/04-casos-prueba.md` y las referencias correspondientes:
   RF-06, RN-15; catálogo de 43 casos y batería de 49 ejecuciones correctas.
4. Cerrar CX-01 como corrección de preparación; conservar CX-02 manual pendiente.
5. Actualizar `docs/ESTADO-AGENTES.md`: T-19 y T-20 aprobadas/ejecutadas, turno de Codex
   cerrado, resultados 48/48 y 49/49; pasar al estado que JC indique.
6. Actualizar en CLAUDE.md/AGENTS.md los estados o cifras que correspondan, manteniendo
   sus secciones compartidas sincronizadas. JC revisa git status antes de autorizar
   cualquier commit; la publicación sigue reservada a JC.
