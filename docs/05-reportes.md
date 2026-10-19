# Reportes de gestión (RF-11, anexo A02)

Los cinco reportes del anexo A02 del PDF (páginas 98 a 105), tal como quedaron implementados
en la Sección 9. **Donde el anexo y este archivo difieren, manda este archivo**: la sección
«Correcciones al anexo» lista qué hay que regenerar en el PDF.

Solo el perfil Administrador los ve (RN-34). `GestorReportes` lo verifica contra
`GestorSesion.UsuarioActual` en cada operación, no solo escondiendo el botón. Ninguna
consulta de reportes escribe.

## Catálogo

| Cód. | Reporte | Período aplicado a | Filtros | Base |
|---|---|---|---|---|
| RPT-01 | Ventas por evento | Fecha del evento | Categoría | Entradas `Confirmada` |
| RPT-02 | Ranking de paquetes | Fecha del evento | Evento | Compras `Confirmada` |
| RPT-03 | Usuarios por nivel de fidelidad | Ventana de `FIDELIDAD_VENTANA_MESES` hasta la fecha de corte | Fecha de corte (por defecto, el último día del período) | Clientes activos en la ventana |
| RPT-04 | Adopción de seguros y suscripciones | Fecha de compra; fecha de inicio de la suscripción | — | Compras `Confirmada`; clientes activos en el período |
| RPT-05 | Demanda insatisfecha | Fecha del evento | Evento; solo agotados | Eventos con inscriptos en lista de espera |

El rango incluye el día «hasta» entero: un evento del 30/11 a las 20:30 entra en un período
que termina el 30/11.

**Por qué RPT-01 filtra por fecha del evento y no por fecha de compra.** El reporte es
«ventas por evento»: responde cuánto vendió cada evento, no cuánto se vendió en un período
de compra. Así lo define el anexo. RPT-02 usa el mismo criterio para que la diferencia entre
los dos sea exactamente el monto de los servicios.

**Clientes activos.** RPT-03 y RPT-04 cuentan como base a los clientes con al menos una compra
confirmada en la ventana (RPT-03) o en el período (RPT-04). Un cliente registrado que nunca
compró no entra. Por eso en RPT-03 el nivel Bronce nunca muestra a alguien con 0 compras,
aunque su criterio diga «0 a 2».

## Indicadores

| Indicador | Fórmula | Unidad |
|---|---|---|
| Entradas vendidas | Σ cantidad de entradas con estado `Confirmada` | unidades |
| Ingreso bruto por entradas | Σ (precio de entrada × cantidad vendida), entradas `Confirmada` | $ |
| Ticket promedio | Ingreso bruto por entradas ÷ entradas vendidas | $ |
| % de ocupación | Entradas vendidas ÷ capacidad total del evento × 100 | % |
| Ingreso del paquete | Σ (`BaseEntradas` + `MontoServicios`) de las compras `Confirmada`; sin cargo por servicio ni seguro | $ |
| Participación del paquete | Unidades (entradas) del paquete ÷ unidades totales × 100 | % |
| Tasa de adopción | Contrataciones del beneficio ÷ base de referencia × 100 | % |
| Demanda insatisfecha | Inscriptos en lista de espera ÷ capacidad del evento × 100 | % |

Los porcentajes se redondean a un decimal, 0,05 hacia arriba. Un divisor 0 da 0 % (o «—» en
el ticket promedio): un evento sin ventas aparece con 0 entradas y 0 % de ocupación, no
desaparece.

## RPT-02: cómo se arma la combinación

No hay catálogo de paquetes: `Paquete` vive en memoria y no tiene tabla. El dato real está en
`CompraServicios`. Cada compra confirmada se clasifica por el **conjunto** de tipos de servicio
que tiene (`Servicios.Tipo`), y el nombre se arma con los tipos presentes, en orden
alfabético: sin servicios, «Solo entrada»; con hospedaje, «Entrada + Hospedaje»; con
merchandising y VIP, «Entrada + Merchandising + VIP». La columna Composición lista los
servicios del catálogo que aparecen en esa combinación. Ningún nombre está escrito en el
código.

## RPT-05: agotado es derivado

«Agotado» no es un estado guardado: es RN-10 (todos los sectores vendieron su capacidad,
contando solo entradas `Confirmada`). La condición SQL está escrita una sola vez, en
`RepositorioEventos.CondicionAgotado`, y la usan `EstaAgotado` y el reporte.

El reporte lista los eventos del período que tienen inscriptos en lista de espera, con la
columna Estado calculada en el momento: «Agotado» o «Con cupo». Un evento con lista y con
cupo es posible: la lista se formó cuando estaba agotado y después se liberaron lugares
(devoluciones, RN-29), o se agregó un sector. La casilla «Solo eventos agotados» deja solo
los que hoy cumplen RN-10.

## PDF de salida

Generado con iText 9.2.0 por código, nunca con impresora virtual (A02). `ExportadorPDF` está
en `Visual`, porque iText solo está referenciado ahí y Negocio no depende de una librería de
presentación: Negocio declara `IExportador` y `GestorReportes.Exportar` lo recibe por
parámetro. Un exportador a Excel sería otra clase que implemente `IExportador`, sin tocar los
reportes; un sexto reporte es otra clase derivada de `Reporte`, sin tocar el exportador.

Cada `Reporte` devuelve una `DataTable` con los valores tipados y declara en
`ExtendedProperties` el formato de cada columna numérica y que la última fila es la de
totales. La grilla y el exportador leen esas marcas; no conocen ningún reporte.

| Zona | Contenido |
|---|---|
| Encabezado | «EventAura» como texto en violeta (116, 86, 174) y Nirmala UI, la tipografía de la aplicación; título, código y período |
| Filtros | Filtros aplicados y quién lo pidió (nombre y mail del usuario en sesión) |
| Tabla | Encabezado sombreado (231, 231, 231) y repetido en cada página, números a la derecha, fila de totales en negrita y resaltada |
| Lectura | Párrafo calculado a partir de los datos del reporte |
| Pie | Fecha y hora de emisión, «Página N de M», nombre del archivo y QR de verificación en cada página |

QR de verificación: `EVAU|código|AAAAMMDD-AAAAMMDD|emisión AAAAMMDDhhmmss|solicitante`, generado
con `GeneradorQR.Imagen`.

Nombre: `EVAU_[código]_[AAAAMMDD-inicio]_[AAAAMMDD-fin].pdf`, por ejemplo
`EVAU_RPT01_20250901_20251130.pdf`. Carpeta: `Mis Documentos\A1.Reportes EVENTAURA`, creada si
no existe.

## Juego de datos simulados

`BaseDatos/03-datos-reporte.sql`. **No** se corre con la semilla: se corre después, solo para
ver los reportes con volumen, y se puede correr dos veces seguidas porque empieza borrando lo
que cargó antes. Período de los ejemplos: 01/09/2025 al 30/11/2025. Los eventos son pasados,
así que no aparecen en el listado del cliente (RN-09). Reproduce los cinco ejemplos del anexo,
con estas diferencias, que son errores de cuenta del anexo y no del juego de datos:

| Dato del anexo | Valor correcto | Por qué |
|---|---|---|
| Ticket promedio del período: $19.356 | **$19.355** | 86.710.000 ÷ 4.480 = 19.354,9. Con 4.480 entradas y $86.710.000 no puede dar 19.356 |
| RPT-05, lectura: la demanda insatisfecha equivale a $13.646.000 | **$13.645.275** | Arrastra el ticket anterior: 705 × 19.355 |
| RPT-01, lectura: los conciertos concentran el 65,8 % del ingreso | **65,9 %** | 57.100.000 ÷ 86.710.000 = 65,85 %, que redondea a 65,9. El anexo truncó |
| RPT-05: los tres eventos figuran «Agotado» | **Con cupo** | Vendieron 96,2 %, 91,9 % y 87,5 % según RPT-01: por RN-10 no están agotados |

Los precios de suscripción que implica RPT-04 (1.476.000 ÷ 164 = $9.000 y 4.590.000 ÷ 51 =
$90.000) son los de `Parametros` desde la Sección 9.

## Correcciones al anexo (para regenerar las páginas del PDF)

### SQL de RPT-01 (página 104)

El SQL del anexo no corre contra el esquema real y, aunque corriera, daría números mal:

1. **Columna inexistente.** `JOIN Sectores s ON s.ID_Evento = e.ID`: la columna es `EventoID`.
2. **El JOIN multiplica filas.** Unir `Entradas` con `Sectores` por evento da una fila por cada
   par entrada × sector: con 3 sectores y 4 entradas, 12 filas. `SUM(en.CantidadVendida)` queda
   multiplicado por la cantidad de sectores y `SUM(s.Capacidad)` por la cantidad de entradas,
   así que el porcentaje de ocupación sale mal por un factor distinto en el numerador y en el
   denominador.
3. **No filtra el estado.** Contaría como ventas las reservas `Pendiente`, las `Vencida` y las
   `Reembolsada`.

Consulta corregida (`RepositorioReportes.VentasPorEvento`, que agrega además la capacidad del
evento como columna para calcular la ocupación de la fila de totales):

```sql
SELECT e.Nombre AS Evento,
       e.Categoria AS Categoria,
       ISNULL(SUM(en.CantidadVendida), 0) AS Entradas,
       ISNULL(SUM(en.CantidadVendida * en.Precio), 0) AS IngresoBruto,
       SUM(en.CantidadVendida * en.Precio)
         / NULLIF(SUM(en.CantidadVendida), 0) AS TicketPromedio,
       CAST(100.0 * ISNULL(SUM(en.CantidadVendida), 0)
         / NULLIF((SELECT SUM(s.Capacidad) FROM Sectores s WHERE s.EventoID = e.ID), 0)
         AS DECIMAL(5,1)) AS PorcOcupacion
FROM   Eventos e
LEFT JOIN Entradas en ON en.ID_Evento = e.ID AND en.Estado = 'Confirmada'
WHERE  e.Fecha BETWEEN @Desde AND @Hasta
  AND  (@Categoria IS NULL OR e.Categoria = @Categoria)
GROUP BY e.ID, e.Nombre, e.Categoria
ORDER BY IngresoBruto DESC;
```

La capacidad va en una subconsulta escalar, no en un JOIN. El `LEFT JOIN` deja ver los
eventos sin ventas, que son justamente los que interesa detectar. Se agrupa por `e.ID`: dos
eventos con el mismo nombre no se suman.

### Otras correcciones

| Página | Dice | Corresponde |
|---|---|---|
| 99 | «estado = VENDIDA» | `Confirmada`: VENDIDA no existe en el esquema |
| 99 | Filtro de RPT-05 «Evento, estado» con «Agotado» como estado | Agotado es derivado (RN-10); el filtro es «solo agotados» |
| 100 | Paquetes con nombre de catálogo («Experiencia VIP») | No hay catálogo: el nombre sale de los tipos de servicio («Entrada + Merchandising + VIP») |
| 102 | «Logotipo EventAura» | No hay logotipo en el repositorio: el nombre va como texto |
| 102 | `List<Filtro> Filtros` en `Reporte` | No hay entidad Filtro: cada reporte declara sus filtros como propiedades y los describe con `Filtros()` |
| 103 | `ExportadorPDF` en la capa de negocio | En `Visual`: iText es presentación. Negocio solo conoce `IExportador` |
| 103 | `RepositorioReportes.DemandaInsatisfecha()` sin parámetros | Recibe período, evento y «solo agotados» |
| 105 | `GeneradorQR.Crear(...)` | No existe: la API es `GeneradorQR.Instance.Imagen(string)` |
| 105 | Una sola instancia de `Image` agregada en todas las páginas | Una imagen nueva por página: iText no coloca una copia por página de la misma instancia |
| 105 | `new Document(pdf, PageSize.A4)` y después el pie | `new Document(pdf, PageSize.A4, false)`: sin volcado inmediato, para conocer el total de páginas antes de escribir los pies |
