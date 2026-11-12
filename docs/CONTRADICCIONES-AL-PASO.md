# Contradicciones encontradas al paso

Las anotó Claude Code el 12/11/2026, mientras regeneraba los diagramas. **No es la auditoría del
documento**, que es la próxima tanda: es lo que apareció sin buscarlo. Las páginas son de
`Documentacion/EventAura_v2.pdf` (v2.4). Ninguna está aplicada.

## Texto del documento que contradice al código

| # | Página | Dice | El código | Fuente |
|---|---|---|---|---|
| T-1 | 72 (CU-01) | «se aplica el descuento por nivel/suscripción» | La suscripción no da descuento; da seguro sin cargo y preventa | RN-37; `GestorFidelizacion.CalcularDescuento` |
| T-2 | 89 (actividad) | «aplicar descuentos (nivel y/o suscripción)», «Actualizar puntos y nivel» | Igual que T-1; no hay puntos: el nivel se calcula por cantidad de compras en 12 meses | RN-19; `GestorFidelizacion.ComprasEnVentana` |
| T-3 | 89 (actividad) | «Mostrar pantalla de confirmación + recomendaciones de eventos» | Las recomendaciones (RF-09) **no están construidas** | `CLAUDE.md`, «Sin construir» |
| T-4 | 86 y 88 (clases, dominio) | `puntos`, `sumarPuntos`, `umbralPuntos` | No existen | `Entidade/Cliente.cs` |
| T-5 | 48 (guion de interfaz) | el formato «se emplea para agregar locales, eventos, vehículos» | No hay vehículos | `Visual/` |
| T-6 | 49 (guion de interfaz) | roles «Cliente, Administrador de local, Conductor» | Solo hay dos perfiles: Cliente y Propietario (Administrador) | RN-05; `Usuario.TipoUsuario` |
| T-7 | 74 (CU-02) | «crear, modificar y desactivar eventos» | Hay crear, suspender, eliminar, cambiar preventa y gestionar sectores; no hay modificación general de un evento | `GestorEventos` |
| T-8 | 80 (CU-05, FA2) | «Usuario bloqueado (opcional)» | El bloqueo por intentos está implementado, no es opcional | RN-04; `GestorUsuarios.Autenticar` |
| T-9 | 82 (CU-07) | «Gestionar listas de espera», del Administrador | El Administrador solo consulta; quien se inscribe es el cliente | `GestorListaEspera.Inscribir` / `DeEvento` |
| T-10 | 84 (CU-08) | «reportes simulados» | Los reportes consultan la base; lo de demostración son los datos que carga `03-datos-reporte.sql`. **A confirmar** qué quiso decir el texto | `RepositorioReportes` |
| T-11 | 87 (estados) | el reembolso lo dispara un «evento cancelado» | Lo pide el cliente, con seguro, hasta 48 h antes | RN-29; `GestorCompras.Reembolsar` |
| T-12 | 71 (casos de uso) | «Modificar local», «Eliminar local», «Gestionar stock», «Ver carrito» | No existen | `GestorLocales` (solo `Registrar` y consultas) |

## Hallazgos en el código o la base, para que decida JC

No son del documento, pero un diagrama fiel los muestra.

| # | Qué | Dónde | Zona |
|---|---|---|---|
| C-1 | `Locales.PropietarioID` no tiene FK declarada (en el ER va punteada) | `BaseDatos/01-esquema.sql:46` | `BaseDatos/`: necesita autorización de JC |
| C-2 | Cuatro referencias entre entidades que nadie usa: `Cliente.entradasUsuario` (una `List<Evento>` con nombre de entradas), `Propietario.Locales`, `Local.Eventos` y `Entrada.Cliente`. El diagrama de clases las muestra porque existen; si se borran, se regeneran las vistas 1 y 3 | `Entidade/` | `Entidade/`: necesita autorización de JC |
| C-3 | `GestorClientes` y `GestorPropietario` están vacíos, sin constructor privado | `Repo/` | Codex |
| C-4 | `Seguros.ID_Entrada` no tiene UNIQUE: la base admite dos pólizas para una entrada, aunque el código crea una sola | `BaseDatos/01-esquema.sql` | `BaseDatos/` |
| C-5 | `Evento.Estado` es un `string` libre, mientras la base tiene `CHECK` con tres valores. Es el mismo cruce de la regla 13, para los eventos | `Entidade/Evento.cs` | `Entidade/` |

## Correcciones a lo que dije antes

- **El informe del 12/11 decía «14 gestores» Singleton. Son 12:** hay 14 clases `Gestor*`, pero
  `GestorClientes` y `GestorPropietario` no son Singleton (C-3). Con los 13 repositorios,
  `Conexion`, `PasarelaSimulada` y `GeneradorQR`, son 28 clases.
- **`CLAUDE.md` dice «Singleton en todos los gestores»**: le faltan las mismas dos excepciones. Lo
  corrijo cuando JC lo apruebe.
