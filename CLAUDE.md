# EventAura — Contexto del proyecto

Plataforma de venta de entradas para espectáculos de Rosario. El diferencial es el
**armado personalizado de paquetes**: entrada + hospedaje + traslado + merchandising +
experiencia VIP en una sola operación.

Proyecto académico — Trabajo de Campo I, Universidad Abierta Interamericana.
La documentación completa está en `Documentacion/EventAura_v2.pdf` (112 páginas).
**No hace falta leer el PDF**: todo lo necesario para programar está en `docs/`.

---

## Stack

| Componente | Versión |
|---|---|
| .NET Framework | 4.8 (Visual) · netstandard2.0 (Entidades, Datos, Repo) |
| Base de datos | SQL Server |
| Acceso a datos | Microsoft.Data.SqlClient 6.0.2 |
| Generación de PDF | iText 9.2.0 |
| Códigos QR | QRCoder 1.6.0 |
| Pruebas | MSTest |

---

## Arquitectura — 4 capas, obligatorio

```
Visual  (WinForms)     →  Negocio  (Gestores)  →  Datos  (Repositorios)  →  SQL Server
                                    ↓
                              Entidades  (POCOs, sin lógica)
```

**Reglas de dependencia, no negociables:**

1. `Visual` **nunca** abre una conexión ni escribe SQL. Solo habla con `Negocio`.
2. `Negocio` **nunca** referencia `System.Windows.Forms`. No muestra MessageBox: devuelve
   resultados o lanza excepciones de dominio.
3. `Datos` es el único lugar donde existe `SqlCommand`, `SqlConnection` y texto SQL.
4. `Entidades` no referencia a nadie. Son POCOs con propiedades automáticas.
5. Todo SQL usa **parámetros** (`cmd.Parameters.AddWithValue`). Nunca concatenación.

**Patrones esperados por la cátedra:**

- **Singleton** en todos los gestores de `Negocio` y repositorios de `Datos`
  (seguir el estilo ya existente en `Repo/GestorClientes.cs`).
- **Composite** para el árbol de permisos (anexo T04, futuro).
- **Observer** para el cambio de idioma (anexo T05, futuro).

---

## Estructura de carpetas

| Carpeta | Proyecto | Namespace | Rol |
|---|---|---|---|
| `Entidade/` | Entidades.csproj | `Entidades` | POCOs y enums |
| `Datos/` | Datos.csproj | `Datos` | Repositorios, único acceso a SQL |
| `Repo/` | Repositorio.csproj | `Negocio` | Gestores, reglas de negocio |
| `Visual/` | Visual.csproj | `Visual` | Formularios WinForms |
| `Pruebas/` | Pruebas.csproj | `Pruebas` | MSTest |

> **Dos inconsistencias heredadas del proyecto original:**
>
> 1. La carpeta del proyecto de negocio se llama `Repo/` pero el proyecto es
>    `Repositorio.csproj`. No la renombres.
> 2. Dentro de ese proyecto conviven **dos namespaces**:
>    `Negocio` en `GestorClientes.cs` y `GestorEventos.cs`;
>    `Repositorio` en `GestorUsuarios.cs` y `GestorPropietario.cs`.
>    Los formularios importan uno u otro según el gestor que usen.
>
> El destino es unificar todo en `Negocio`. Cada sección migra los archivos que toca
> y actualiza el `using` de los formularios afectados. **Antes de escribir un
> `namespace`, verificá cuál usa el archivo que estás modificando.**

**Carpetas eliminadas** (eran código muerto fuera de la solución): `Entidades/`,
`Negocio/`, `Visual_/`. Si volvés a verlas, están de más.

---

## Convenciones de código

- Nombres en **español**, igual que el dominio: `GestorEventos`, `RepositorioCompras`.
- Formularios: prefijo `frm` (`frmMenuCliente`). Los existentes mezclan `frm` y `Frm`;
  para archivos nuevos usar siempre `frm` minúscula.
- Métodos de repositorio: verbo + sustantivo (`ObtenerPorId`, `ContarComprasConfirmadas`).
- Importes: `decimal`, nunca `float` ni `double`.
- Fechas: `DateTime`. La ventana de fidelización usa `AddMonths(-12)`.
- Los valores numéricos de las reglas **no se escriben en el código**: se leen de la tabla
  `Parametros` (ver `docs/02-reglas-negocio.md`, sección de parámetros configurables).
- **Archivos `.Designer.cs`**: el criterio es **no rediseñar por gusto**. No se reposiciona,
  redimensiona ni recolorea nada que ya funcione. Sí se permite, siempre declarándolo en el
  reporte:
  - **agregar** un control que un requerimiento necesita;
  - **reemplazar** un grupo de controles por otro cuando la tarea lo pide de forma explícita;
  - corregir el **orden de tabulación** (`TabIndex`);
  - corregir el **texto visible** de etiquetas y botones.
  Ante la duda, preguntá antes de tocar. El formato de un `DataGridView` va por código en el
  `Load`, no en el Designer.

---

## Documentos de referencia

| Archivo | Contenido |
|---|---|
| `docs/01-requerimientos.md` | RF-01 a RF-13 y RNF-01 a RNF-05 |
| `docs/02-reglas-negocio.md` | RN-01 a RN-37 y parámetros configurables |
| `docs/03-esquema-datos.md` | DDL completo y datos semilla |
| `docs/04-casos-prueba.md` | CP-01 a CP-10 |
| `docs/GUIA-PROMPTS.md` | Orden de construcción, sección por sección |

Cuando una tarea mencione una regla (`RN-19`) o un requerimiento (`RF-08`), leé su
definición del archivo correspondiente. No la deduzcas.

---

## Estado actual

Construido: login, alta de usuarios, listado de eventos, alta de eventos/sectores/locales,
compra de entradas por sector, emisión de ticket PDF con QR.

Sin construir: servicios complementarios, paquetes, seguro, suscripciones, descuentos por
nivel, lista de espera, reportes, recomendaciones.

---

## Definición de terminado

Una tarea está completa cuando:

1. La solución compila sin errores ni advertencias nuevas.
2. No hay SQL fuera de `Datos/`.
3. Las reglas de negocio citadas en la tarea están implementadas y son verificables.
4. Los casos de prueba asociados pasan.
5. **Las pruebas de interfaz que no pudiste ejecutar quedaron anotadas** en
   `docs/PRUEBAS-PENDIENTES.md`, con qué probar, qué resultado se espera y el commit
   que las introduce. Agregá tu sección al final del archivo antes de commitear.

> Las pruebas manuales de interfaz están diferidas hasta terminar las 10 secciones.
> No las des por verificadas ni las omitas: anotalas.
