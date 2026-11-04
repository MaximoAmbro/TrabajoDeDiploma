# T-21 — Semilla alineada con el ejemplo del documento

> Reporte de Codex, movido desde `Pruebas/` por Claude Code sin cambiar el contenido. Después
> de T-21, Claude Code regeneró las figuras del instructivo (muestran «Duki en Rosario» y
> $99.625, igual que la página 54) y recreó la base de desarrollo con la semilla nueva, por
> decisión de JC.

Fecha: 04/11/2026. Base: `a6f632f`, rama `main`, árbol limpio al iniciar.
Se leyeron el estado de agentes y la «Tercera tanda». JC concedió el turno y autorizó
expresamente el cambio acotado de `BaseDatos/02-semilla.sql`.

## Qué se hizo

- El primer evento insertado pasó de «Festival de Rock Rosario» a **«Duki en Rosario»**.
  Conserva el ID 1 en una base nueva, la fecha relativa, la hora, el local, la categoría,
  el estado, los sectores, las capacidades, los servicios y todos sus importes.
- El historial de la clienta Plata pasó de tres a **cuatro compras confirmadas**, con
  fechas de 2, 4, 6 y 8 meses atrás. Se actualizó el comentario correspondiente. Se
  conserva el nivel Plata y no se modificó el historial del cliente Oro.
- Se actualizó únicamente el comentario del evento 1 en `PruebasInterfaz.cs`.
  Los títulos largos inventados «Gran Festival Internacional…» quedaron intactos.
- Se buscaron otras dependencias del nombre anterior y del historial de tres compras
  en `Pruebas/`. Las referencias a tres compras de los casos de fidelización preparan
  sus propios clientes con Fake y no dependen del historial de la semilla; se conservaron.

## Verificación

Comando ejecutado:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar
```

Resultado:

```text
La serie de pruebas se ejecutó correctamente.
Pruebas totales: 49
     Correcto: 49
Tiempo total: 5,2980 Segundos
Código de salida: 0
```

Sin errores ni advertencias nuevas de compilación; sin pruebas omitidas o fallidas.
CP-11, la referencia de **$99.625**, sigue pasando.

La inicialización de la batería recreó exclusivamente `BddEventAura_Pruebas` con la
semilla nueva. Después de la corrida se ejecutaron las dos consultas de T-21 con
`sqlcmd` contra esa base:

```sql
SELECT Nombre FROM Eventos WHERE ID = 1;
```

```text
Nombre
------
Duki en Rosario
```

```sql
SELECT COUNT(*) AS ComprasConfirmadasPlata
FROM Compras c JOIN Usuario u ON u.ID = c.ID_Usuario
WHERE u.Mail = 'plata@eventaura.com' AND c.Estado = 'Confirmada';
```

```text
ComprasConfirmadasPlata
-----------------------
4
```

`git diff --check` no informó errores de espacios.

## Archivos tocados

Modificados:

1. `BaseDatos/02-semilla.sql`: nombre, límite del bucle de Plata y comentario.
2. `Pruebas/PruebasInterfaz.cs`: comentario del evento 1.

Nuevo: `Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-04-T21.md` (este reporte).

No se modificaron `03-datos-reporte.sql`, otros archivos de BaseDatos/, código de la
aplicación, Designer, docs/ o tools/. Ambos App.config siguen intactos y con
skip-worktree (S). No se recreó ni modificó `BddEventAura`.
No hubo commits ni push.

## Pendientes y traspaso a Claude Code

No se encontraron defectos ni hacen falta más cambios para completar T-21. Codex
terminó la tarea y se detuvo, sin regenerar figuras ni ampliar el alcance.

En el próximo turno, Claude Code debería:

1. Trasladar este reporte a docs/ y marcar T-21 terminada en `ESTADO-AGENTES.md`.
2. Ejecutar `tools/CapturasInstructivo/Generar.ps1` con la semilla nueva.
3. Revisar que la figura 2 muestre **«Duki en Rosario»** y la figura 3 muestre
   **$99.625**, antes de considerarlas definitivas.
4. Actualizar `docs/capturas/instructivo/LEEME.md`, el ítem 9 de
   `docs/CORRECCIONES-DOCUMENTO.md` y las referencias del estado que correspondan.

La regeneración y documentación de las figuras quedan fuera de la zona de Codex.
Si se quisiera actualizar la base de desarrollo con esta semilla, debe decidirlo JC:
esta tarea no lo autoriza ni lo necesita.
