# Propuesta P4: una sola tabla del sistema

Claude Code, 26/11/2026. **Propuesta cerrada, sin aplicar.** JC la aprobó para después de P1, P2 y P5. La
aplica Codex en `Visual/Web/`, porque las tablas viven en las vistas integradas y no en las maquetas.

## Por qué

Hoy hay dos tablas distintas, cada una definida en la hoja de su pantalla:

| Dónde | Bordes | Relleno | Encabezado | Rayado |
|---|---|---|---|---|
| `Visual/Web/datos.css:18-22, 29` (reportes) | Solo línea inferior | 4 × 8 px | Con fondo `--fondo` | Sí |
| `Visual/Web/gestion.css:39-42` (panel del Administrador) | En todas las celdas | 8 px | Sin fondo | Sí |

`sistema.css` no tiene tabla, aunque la usan tres de las ocho vistas que lo cargan. Las dos referencias
estudiadas el 26/11 (ui.shadcn.com, sobre todo) resuelven todas las listas de registros con una sola tabla:
- solo líneas horizontales de 1 px;
- encabezado de 40 px sin fondo;
- 8 px de relleno;
- sin rayado.

**El porqué de cada decisión:**
- **Solo líneas horizontales:** un registro se lee de izquierda a derecha. Las líneas verticales y el rayado
  agregan trazos que hay que saltar, y no ayudan a encontrar una fila.
- **Encabezado sin fondo, con una línea más oscura (`--borde-control`) abajo:** se distingue de las filas sin
  sumar otra superficie.
- **Fondo blanco en el encabezado:** está en la regla porque `datos.css:28` lo deja fijo al desplazar
  (`position: sticky`). Sin fondo, las filas se verían a través del encabezado.
- **Cifras a la derecha y con dígitos de ancho fijo:** la columna de importes se lee entera de un vistazo (§1).
- **La fila de total con trazo `--texto` arriba:** es el mismo cierre que usa el total del armador.

## Cambios exactos

### 1. `Visual/Web/sistema.css`: agregar después del bloque «Avisos»

```css
/* ---------- Tabla: filas, no celdas (PRINCIPIOS-DISENO.md §5) ---------- */
.tabla { width: 100%; border-collapse: collapse; background: var(--superficie); }
.tabla th, .tabla td { padding: var(--e2) var(--e3); border-bottom: 1px solid var(--borde); text-align: left; vertical-align: top; }
/* El encabezado lleva fondo propio porque en los reportes queda fijo al desplazar (datos.css). */
.tabla th { height: var(--alto-control); padding-block: 0; vertical-align: middle; background: var(--superficie);
    font: var(--n5); font-weight: 600; letter-spacing: var(--n5-ls); color: var(--texto-secundario);
    border-bottom-color: var(--borde-control); }
.tabla .cifra { text-align: right; font-variant-numeric: tabular-nums; }
.tabla tr.total td { font-weight: 600; border-top: 1px solid var(--texto); }
```

Lo mismo va en `docs/disenos/sistema.css`; eso lo hace Claude Code cuando JC autorice.

### 2. `Visual/Web/datos.js`

**Ojo:** en este archivo la firma es `nodo(tipo, texto, clase)`. En `gestion.js` es `nodo(tipo, clase, texto)`.

| Línea | Antes | Después |
|---|---|---|
| 125 | `const tabla = nodo("table");` | `const tabla = nodo("table", null, "tabla");` |
| 127 | `… modelo.numericas[i] ? "numerica" : "" …` | `… modelo.numericas[i] ? "cifra" : "" …` |
| 132 | `… modelo.numericas[j] ? "numerica" : "" …` | `… modelo.numericas[j] ? "cifra" : "" …` |

### 3. `Visual/Web/datos.css`

| Línea | Antes | Después |
|---|---|---|
| 5 | `.valor, .numerica { font-variant-numeric: tabular-nums; }` | `.valor { font-variant-numeric: tabular-nums; }` |
| 18 a 22 | `table {…}`, `th, td {…}`, `th {…}`, `.numerica {…}`, `.total {…}` | se borran |
| 28 | `.tabla-resultados th { position: sticky; top: 0; }` | **queda** |
| 29 | `tbody tr:nth-child(even) { background: var(--fondo); }` | se borra |

### 4. `Visual/Web/gestion.css`

| Línea | Antes | Después |
|---|---|---|
| 39 a 42 | `.tabla {…}`, `.tabla th, .tabla td {…}`, `.tabla th {…}`, `.tabla tbody tr:nth-child(even) {…}` | se borran: la clase `.tabla` que ya usa `gestion.js:71` pasa a tomar la del sistema |

## Qué cambia a la vista

- **Reportes:** las filas suben de 4 a 8 px de relleno vertical (unos 8 px más por fila), el encabezado
  pierde el fondo gris y se va el rayado.
- **Auditoría (panel):** se van las líneas verticales y el rayado.
- **Altura:** las tablas largas de los reportes van a ocupar más alto. Hay que revisarlo en `frmReportes` a
  96 y a 144 DPI.

## Cómo se da por hecha

1. La batería completa pasa.
2. Las capturas de Reportes (las cinco) y de Auditoría, a 96 y a 144 DPI, no muestran texto cortado, el
   encabezado fijo de los reportes tapa las filas al desplazar y la fila de total se distingue.
3. `grep -n "numerica" Visual/Web/` no encuentra nada, y `table {` y `.tabla {` aparecen solo en
   `sistema.css`.

## Fuera de esta propuesta, para decidir aparte

`datos.css:12` marca el selector de reporte elegido con `--primario` y `--primario-suave`. Con P2, lo elegido
va en neutro (`--seleccion`). Si se quiere la misma regla en los reportes, es una línea más, pero cambia una
pantalla que no estaba en P2.
