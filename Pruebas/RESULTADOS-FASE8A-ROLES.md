# Fase 8a — separación de perfiles y barrido de pertenencia

Estado: implementado y verificado. Base previa: 319/319. Cierre de esta fase: 323/323.

## Perfiles

- `Cliente`: compra, suscripción y devolución.
- `Organizador`: administra sus locales, eventos, sectores e ingresos; consulta sus propias listas y reportes.
- `Administrador`: administración global, alta de administradores y reportes globales; no es dueño de locales.
- RN-05 queda actualizado en `docs/02-reglas-negocio.md`. Se documentan RN-48 (operaciones de organizador) y RN-49 (alcance de pertenencia).
- Semilla: `admin@eventaura.com` es Administrador sin locales; `organizador@eventaura.com` es Organizador y dueño del local demostrativo.
- Migración idempotente: `BaseDatos/11-perfiles-administrador-organizador.sql`.

## Revisión retrospectiva del barrido de pertenencia

La comparación es contra el código inmediatamente anterior a 8a (`a48eb43^`). «Abierto» significa que una cuenta Propietario podía usar el rol compartido y pasar un ID de otro dueño, o que el método no tenía guarda. No cuento las posibilidades formuladas solo como “si se quitara la guarda”. El barrido encontró **24 métodos con exposición efectiva de datos/operaciones entre dueños y 1 método de auditoría falsificable**.

### Métodos efectivamente abiertos antes de 8a

| Método | Qué dato se filtraba o qué se podía tocar | Estado antes de 8a | Prueba que lo cierra |
|---|---|---|---|
| `GestorLocales.PorId` | Dirección, capacidad e identidad del propietario de un local ajeno, incluso sin evento publicado. | Abierto por ID. | CP-182 rechaza la lectura de un local ajeno como Organizador. |
| `GestorLocales.PorPropietario` | Nombres, direcciones y capacidades de todos los locales del ID recibido. | Abierto para el rol compartido; aceptaba ID arbitrario. | **Sin caso negativo de otro Organizador.** CP-141 prueba rol Cliente/sin sesión y acceso propio, no propiedad ajena. |
| `GestorLocales.Registrar` | Creación de un local atribuido a un `PropietarioID` elegido por quien llama. | Abierto; no fijaba el titular desde la sesión. | **Sin caso negativo de atribución ajena.** |
| `GestorEventos.Crear` | Alta de evento en un local de otro Organizador. | Abierto por perfil, sin validar dueño del local. | **Sin caso negativo de creación en local ajeno.** |
| `GestorEventos.Publicar(Evento)` | Publicación de evento y sectores asociados a un local ajeno. | Abierto por perfil, sin validar dueño del local. | **Sin caso negativo de publicación ajena.** |
| `GestorEventos.Eliminar` | Eliminación de un evento ajeno sin ventas. | Abierto por perfil, sin validar dueño. | **Sin caso negativo de eliminación ajena.** CP-93 solo rechaza al Cliente por perfil. |
| `GestorEventos.Suspender` | Suspensión de un evento ajeno. | Abierto por perfil, sin validar dueño. | **Sin caso negativo de suspensión ajena.** |
| `GestorEventos.CambiarPreventa` | Alteración de la fecha de preventa de otro dueño. | Abierto por perfil, sin validar dueño. | **Sin caso negativo de modificación ajena.** |
| `GestorEventos.CambiarImagenPortada` | Sustitución de la portada de otro dueño. | Abierto por perfil, sin validar dueño. | **Sin caso negativo de modificación ajena.** |
| `GestorEventos.PorLocal` | Nombres, fechas y estados de los eventos de un local ajeno. | Abierto por ID. | CP-182 rechaza el acceso al local ajeno. |
| `GestorEventos.ObtenerPorId` | Datos de eventos ajenos, incluidos borradores o suspendidos. | Abierto por ID. | CP-182 rechaza un evento ajeno. |
| `GestorEventos.Sectores` | Nombres, precios y capacidades de sectores de otro dueño. | Abierto por ID. | CP-182 rechaza los sectores de un evento ajeno. |
| `GestorEventos.AgregarSector` | Incorporación de un sector a un evento ajeno. | Abierto por perfil, sin validar dueño del evento. | **Sin caso negativo de alta en evento ajeno.** |
| `GestorEventos.EliminarSector` | Eliminación de un sector ajeno sin entradas vendidas. | Abierto por perfil, sin validar dueño del evento. | **Sin caso negativo de eliminación ajena.** |
| `GestorEventos.CapacidadTotal` | Capacidad agregada de un evento ajeno. | Abierto por ID. | CP-182 rechaza la lectura de capacidad ajena. |
| `GestorEventos.EstaAgotado` | Disponibilidad de un evento ajeno. | Abierto por ID, incluso para eventos no publicados. | CP-182 rechaza la consulta de agotamiento ajena. |
| `GestorPaquetes.ServiciosDe` | Servicios, nombres y precios configurados para un ID de evento ajeno. | Abierto por ID, incluso antes de publicarse. | CP-182 rechaza la consulta de servicios de un evento ajeno. |
| `GestorPaquetes.DisponiblesDe` | Cupo disponible de un sector ajeno. | Abierto por ID. | CP-182 rechaza la consulta de disponibilidad ajena. |
| `GestorListaEspera.DeEvento` | Identidad, correo, prioridad y fecha de inscripción de clientes en la lista de otro evento. | Abierto por el rol compartido; no filtraba por dueño. | CP-182 cubre otro Organizador; CP-126 cubre Cliente y sesión ausente. |
| `GestorReportes.Crear` | Reporte global con ventas, importes, clientela, beneficios y demanda de otros locales. | Abierto por perfil; el perfil de dueño también satisfacía “Administrador”. | CP-182 prueba la creación del RPT-01 con alcance propio; **no cubre los cinco tipos de reporte**. |
| `GestorReportes.Ejecutar` | Filas y agregados globales del reporte. | Abierto por perfil; sin filtro por dueño. | CP-182 demuestra el filtro del RPT-01; **no cubre los otros cuatro tipos**. |
| `GestorReportes.Exportar` | El mismo conjunto global, escrito a un archivo exportado. | Abierto por perfil; heredaba la consulta global. | **Sin negativa de exportación entre dueños.** |
| `GestorReportes.EventosEntre` | Catálogo global de eventos y fechas usado para filtrar reportes. | Abierto por perfil, sin filtro de dueño. | CP-182 comprueba que el selector del Organizador contiene solo su evento. |
| `GestorAuditoria.Consultar` | Acciones, usuarios y resultados de toda la plataforma. | Abierto a todo Propietario por el perfil compartido. | **Sin negativa específica para Organizador.** CP-174 prueba Cliente frente a Administrador. |
| `GestorAuditoria.Registrar` | Falsificación de autor: la capa llamadora podía indicar `usuarioId` y correo de otra persona. | Abierto: era público y aceptaba el autor como argumento. | **Sin caso negativo directo** que intente suplantar autor; quedó `internal`, pero la cobertura actual no prueba esa visibilidad desde Visual. |

### Endurecimiento preventivo; no eran fugas abiertas antes de 8a

| Método | Qué habría expuesto si se quitaba la guarda | Estado antes de 8a | Prueba relacionada |
|---|---|---|---|
| `GestorIngresos.Registrar`, `Contar`, `EventosConIngresoAbierto` | Ingresos y eventos de otros locales. | **No estaban abiertos:** ya comparaban el dueño del evento con la sesión y el listado filtraba por propietario. No cuentan entre las fugas de 8a. | CP-123 es regresión de pertenencia y ventana temporal; no demuestra que el fallo existiera antes de 8a. |
| `GestorPaquetes.AgregarEntradas`, `AgregarServicio`, `QuitarServicio`, `Validar` | Permitirían armar o validar un objeto de paquete en memoria con IDs de otro evento; no persistían datos ni compras por sí mismos y el inicio de compra revalidaba. | Endurecimiento de coherencia del objeto, no fuga persistida entre dueños. | CP-57 verifica el rechazo de un servicio de otro evento; no es una prueba de aislamiento de propiedad. |
| `GestorEventos.Vigentes`, `Buscar` | No filtran por dueño. | Global deliberado: solo catálogo público de eventos publicados y futuros; no se contó como fuga. | Casos del catálogo CP-30 y CP-81 prueban publicación/preventa, no aislamiento de dueño. |
| Gestores de compras, pagos, seguros, fidelización, suscripciones y datos de espera por cliente | Datos privados de una cuenta distinta. | Ya comprobaban titularidad; no se identificó apertura pre-8a en el diff. | CP-127–CP-140 y CP-161 cubren varias de estas guardas. |

Por lo tanto, el informe anterior mezclaba defectos efectivos con riesgo hipotético y además atribuyó a CP-182 pruebas de escritura que no tiene. La cifra defendible es **24 métodos de acceso/operación entre dueños y 1 método de auditoría falsificable**, con los huecos de prueba indicados explícitamente; no “ocho fugas” inferidas de escenarios condicionales.
## Pruebas cambiadas por el nuevo contrato

| Prueba | Antes | Ahora |
|---|---|---|
| CP-12 | Contaba 21 formularios del modelo anterior. | Cuenta 22 porque hay una pantalla distinta de Administrador de plataforma. |
| CP-63 | Cubría las pantallas previas. | Incluye el nuevo menú de Administrador; conserva la exigencia de no serializar estilo. |
| CP-66 | El Administrador entraba al menú compartido de Propietario. | Verifica que abre `frmMenuAdministrador` y mantiene perfil `Administrador`. |
| CP-116 | Verificaba el texto de botones de las pantallas existentes. | Añade los dos botones del menú nuevo; total de 21 pantallas verificadas. |
| CP-126 | Esperaba negar a todo perfil salvo Administrador la lectura de una lista. | Cliente/visita siguen rechazados; Organizador solo lee su evento y Administrador conserva lectura global. |
| CP-131, CP-134, CP-135, CP-136 | Entraban a las pruebas del control con sesión de Administrador, que antes también actuaba como dueño. | Usan Organizador dueño; las afirmaciones sobre suspensión, estado de entrada, bordes y normalización no cambian. |
| CP-141, CP-142 | Trataban locales e ingreso como funciones exclusivas del Administrador. | Rechazan Cliente/visita y prueban el acceso de un Organizador solo a sus datos. |
| CP-143, CP-144 | Reportes y selector de eventos eran solo del Administrador. | Organizador puede consultarlos con alcance propio; Administrador global; ausencia de sesión y Cliente siguen rechazados. |
| CP-93, CP-94, CP-146 | Esperaban RN-34 para operaciones que antes se agrupaban bajo Administrador. | Esperan RN-48 y el rechazo exacto de Organizador para operaciones de gestión; las operaciones de cuenta conservan RN-41. |
| CP-99, CP-101, CP-111 | Usaban privilegio implícito del antiguo perfil para leer y devolver entradas. | Separan sesión de Organizador para ingreso y de Cliente titular para compras/devolución; la regla probada no se debilitó. |
| CP-37 | El Administrador actuaba también como comprador para preparar un reembolso del reporte. | La compra/devolución se hace con Cliente titular; el reporte se ejecuta luego como Administrador. |
| CP-182 (nuevo) | — | Organizador A no recibe filas/cifras, eventos, sectores, locales, servicios, disponibilidad, lista o conteo de ingreso del Organizador B; Administrador conserva reportes globales. |
| CP-183 (nuevo) | — | Confirma que la cuenta Administrador no posee locales, que Organizador es otra cuenta y que la cuenta semilla entra al menú de Organizador. |
| CP-74 | Antes esperaba que al pulsar Publicar el evento quedara `Publicado`. | Agregar un sector deja `Borrador`; la publicación explícita del primer evento queda `EnRevision` para la revisión de RN-54. |
| CP-82 | Antes esperaba que al pulsar Publicar quedara `Publicado`. | Comprueba que la preventa se conserva cuando la primera publicación queda `EnRevision`; las otras cuatro variantes no cambian. |

No se eliminaron aserciones de negocio para obtener verde. Los cambios de CP-131/134/135/136 fueron de identidad del fixture; la propiedad funcional comprobada permanece igual.

## Verificación

- Batería: 322/323 en la última corrida. El único rojo fue CP-174, ajeno a los cambios de perfiles: el registro se guarda con `SYSUTCDATETIME()` y la prueba consulta con el día local, por eso el registro de las 00:06 UTC del 18/11 queda fuera del rango local que terminaba a las 00:00. El dato aparece al consultar con rango UTC. No cambié producción ni la prueba por la regla de no corregir un defecto preexistente; queda reportado para decisión. Los otros 322 casos pasaron.
- Barrera visual: CP-63 + CP-112/CP-120 + CP-116 = 51/51.
- Barrido de texto cortado: 16 botones, 0 cortes, 22 formularios inspeccionados.
- Color renderizado: 32 botones primarios, 0 fallos; el control negativo del estilo del sistema se distingue.
- CP-182 en mutación: al anular temporalmente el alcance del reporte falló 1/1 y expuso filas de otros eventos; restaurada la guarda, vuelve a verde. Al quitar temporalmente la guarda de disponibilidad/evento ajeno, CP-182 falló 1/1; restaurada.
- Capturas completas con `DrawToBitmap`: `Capturas-Fase8a/antes-{100,150}` y `despues-{100,150}`. Las capturas previas reproducen la pantalla combinada anterior; la pantalla nueva de plataforma queda documentada como comparación contra ese menú. En la ficha de tipografía, los menús requieren ancho de inserción de **24,2 cm** para que la letra mínima quede en 8,1 pt; a 16 cm queda en 5,36 pt.
- Las capturas se generaron a 100 % y 150 %; Windows quedó restaurado a 100 % (96 DPI). No quedaron ventanas del host abiertas.
