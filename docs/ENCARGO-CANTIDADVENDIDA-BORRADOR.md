# Encargo pendiente para Codex: `Entradas.CantidadVendida` restringida a 1

Claude Code, 17/11/2026, por decisión de JC. **Borrador:** Codex está en las cuatro fases y no se
lo interrumpe. Este encargo pasa a `docs/TAREAS-CODEX-PRUEBAS.md` cuando JC avise que Codex
terminó (mientras trabaja, ese archivo no se toca).

## Por qué

`Entradas.CantidadVendida` es `INT NULL` y no tiene restricción (`BaseDatos/01-esquema.sql:74`).
El sistema siempre la escribe en 1: una fila por entrada, cada una con su QR
(`Repo/GestorCompras.cs:74-82` y las dos semillas). Pero la columna se lee para calcular el cupo:

```sql
-- Datos/RepositorioSectores.cs:127-131, ObtenerDisponibles
SELECT s.Capacidad
     - ISNULL((SELECT SUM(e.CantidadVendida) FROM Entradas e
               WHERE e.ID_Sector = s.ID AND e.Estado = 'Confirmada'), 0)
     - s.CantidadReservada AS Disponible ...
```

También la suman `RepositorioSectores` (otras tres consultas), `RepositorioEventos`,
`RepositorioListaEspera`, `RepositorioCompras` y `RepositorioReportes`.

- Con **NULL**, `SUM` la ignora; con **0**, no resta. En los dos casos la entrada no cuenta
  contra la capacidad: **sobreventa** (RN-08).
- Con un valor **mayor que 1**, el sector parece más lleno de lo que está y bloquea ventas, y los
  reportes cuentan entradas que no existen.

No pasa en uso, porque el único que escribe entradas es el código. Pero la base no lo garantiza
y ninguna prueba lo verifica. Criterio de JC: **se restringe en la base lo que, al violarse, hace
daño.**

## Qué hacer, en este orden

1. **Script nuevo en `BaseDatos/`**, con el número siguiente libre (después de los que agreguen
   las cuatro fases). Idempotente, como el 05 y el 07:
   1. **Primero limpiar:** `UPDATE Entradas SET CantidadVendida = 1 WHERE CantidadVendida IS NULL
      OR CantidadVendida <> 1`, y **mostrar cuántas filas tocó** (`@@ROWCOUNT`, con un `SELECT` o
      `PRINT` que lo diga).
   2. **Después** `ALTER COLUMN CantidadVendida INT NOT NULL` y
      `ADD CONSTRAINT CK_Entradas_CantidadVendida CHECK (CantidadVendida = 1)`, solo si no existe.

   **El orden importa:** si el `ALTER` corre antes de la limpieza, falla en cualquier base que
   tenga una fila mala.
2. **`BaseDatos/01-esquema.sql`:** las mismas dos restricciones, para que una base nueva nazca
   con ellas.
3. **Prueba con su CP correlativo:** la base rechaza un `INSERT` con `CantidadVendida = NULL` y
   otro con `CantidadVendida = 2` (y conviene `0`), con reversión y la comprobación de que no
   quedó nada. Rojo sin la restricción, verde con ella, como CP-147.

## Qué no hacer

- **No cambiar el cálculo de disponibilidad a `COUNT(*)`** (decisión de JC): es la consulta más
  cargada de significado del sistema. La restricción la vuelve correcta sin tocarla.
- No tocar las otras consultas que suman la columna: con la restricción, todas son correctas.

## Lo que cambia en el documento cuando entre

- **Scripts de base** de la lista para Máximo: el script nuevo, con qué pasa si no se corre
  (la base admite una entrada que no cuenta contra el cupo).
- **Entidad-relación** (`er-2-compras`): `CantidadVendida` pasa a obligatoria (asterisco) y con
  `CK`; fila nueva en la tabla «Restricciones que no entran en la figura». El texto con el criterio
  ya está escrito (`docs/PARA-MAXIMO-DIAGRAMAS.md`, sección 6).
- **Registro de casos y matriz:** el CP nuevo, en la fila de RF-04, que es la de RN-08.
