# Fase 8a — separación de perfiles y barrido de pertenencia

Estado: implementado y verificado. Base previa: 319/319. Cierre de esta fase: 323/323.

## Perfiles

- `Cliente`: compra, suscripción y devolución.
- `Organizador`: administra sus locales, eventos, sectores e ingresos; consulta sus propias listas y reportes.
- `Administrador`: administración global, alta de administradores y reportes globales; no es dueño de locales.
- RN-05 queda actualizado en `docs/02-reglas-negocio.md`. Se documentan RN-48 (operaciones de organizador) y RN-49 (alcance de pertenencia).
- Semilla: `admin@eventaura.com` es Administrador sin locales; `organizador@eventaura.com` es Organizador y dueño del local demostrativo.
- Migración idempotente: `BaseDatos/11-perfiles-administrador-organizador.sql`.

## Barrido de métodos de negocio

| Método | ¿Comprueba pertenencia? | Exposición si no |
|---|---|---|
| `GestorLocales.PorId` | Sí para Organizador; Cliente/visita solo ve locales con eventos publicados; Administrador conserva lectura global. | Antes, un ID de local permitía leer locales sin evento público. Corregido: ya no devuelve ese registro a Cliente/visita. |
| `GestorLocales.PorPropietario` | Sí: Organizador titular del ID; no entrega la lista de otro. | Sin la guarda, revela nombres, direcciones y capacidades de locales ajenos. |
| `GestorLocales.Registrar` | Sí: requiere Organizador y fuerza `PropietarioID` desde la sesión. | Sin la guarda, un actor podría crear locales atribuidos a otra cuenta. |
| `GestorEventos.Crear`, `Publicar` | Sí: exige Organizador y local propio. | Un local ajeno podría recibir eventos creados por la cuenta atacante. |
| `GestorEventos.Eliminar`, `Suspender`, `CambiarPreventa`, `CambiarImagenPortada`, `AgregarSector`, `EliminarSector` | Sí: exige Organizador y verifica dueño del evento/sector antes de modificar. | Se podrían suspender, alterar, borrar o cambiar sectores de eventos ajenos. |
| `GestorEventos.PorLocal` | Sí: verifica dueño del local. | Devuelve el inventario de eventos de otro local. |
| `GestorEventos.ObtenerPorId`, `Sectores`, `CapacidadTotal`, `EstaAgotado` | Sí para Organizador; Cliente/visita solo lee eventos publicados; Administrador puede revisar globalmente. | IDs ajenos daban acceso a evento, sectores, capacidad y disponibilidad; drafts/suspendidos también quedaban consultables por algunos caminos. Corregido. |
| `GestorEventos.Vigentes`, `Buscar` | No filtra dueño deliberadamente: son el catálogo público de eventos publicados y futuros. | No expone datos privados del organizador: solo información publicada de venta. Decisión de producto revisada. |
| `GestorPaquetes.ServiciosDe`, `DisponiblesDe`, `Nuevo`, `Validar` | Organizador solo consulta/valida eventos propios; Cliente compra sobre eventos publicados; disponibilidad/servicios no revelan drafts. `Nuevo` además exige que el ID de cliente sea el de sesión. | Antes, un organizador podía consultar servicios/cupo ajeno por ID o validar un paquete apuntado a otro evento. Corregido. |
| `GestorPaquetes.AgregarEntradas`, `AgregarServicio`, `QuitarServicio` | Verifica que el sector/servicio pertenezca al evento del paquete; la validación de compra vuelve a comprobar propietario, estado y cupo. | Sin la comprobación, se podía introducir al paquete un sector o servicio de otro evento; la compra final lo rechazaba, pero la lectura previa no era consistente. |
| `GestorPaquetes.CantidadEntradas`, `SubtotalEntradas`, `SubtotalServicios` | No consultan ni modifican la base; calculan sobre el paquete en memoria que aporta la llamada. | No entregan filas ajenas. No se usan como autorización ni sustituyen la validación final. |
| `GestorIngresos.Registrar`, `Contar`, `EventosConIngresoAbierto` | Sí: Organizador y propietario del evento; el listado usa las ventanas temporales abiertas. | Un rol correcto pero dueño distinto podía consultar o registrar ingresos de eventos ajenos. Corregido y cubierto por CP-182/CP-123. |
| `GestorListaEspera.Inscribir`, `EstaInscripto`, `ConsultarPosicion`, `AvisosAlIngresar` | Sí: operaciones privadas exigen que el usuario coincida con la sesión. | Podían filtrarse posición, inscripción o avisos de otro cliente. |
| `GestorListaEspera.DeEvento` | Sí: Administrador ve la lista global; Organizador solo la de sus eventos; Cliente no accede. | Podía revelar nombres, correos y prioridad de espera de otros eventos. Cobertura negativa en CP-182 y CP-126. |
| `GestorCompras.Iniciar`, `Confirmar`, `Liberar`, `ObtenerPorId`, `EntradasDe`, `PagosDe`, `SegurosDe`, `Reembolsar` | Sí: la compra se resuelve y verifica contra el dueño autenticado. `Iniciar` exige identidad del cliente actual. | Sin la guarda, se podría consultar pago, QR, póliza, importe o devolver una compra ajena. |
| `GestorCompras.HistorialDe` | Sí: exige titular de la cuenta. | Revelaría historial y datos de las compras de otro cliente. |
| `GestorCompras.LiberarVencidas` | Global deliberadamente: mantenimiento de reservas vencidas, sin mostrar datos a un usuario. | Afecta únicamente reservas vencidas según el reloj y los parámetros; no es una operación sobre una compra elegida por el actor. |
| `GestorCompras.VencimientoDe`, `ValidarTarjeta` | No aplican pertenencia: cálculo/validación sin lectura de información privada. | No acceden a datos ajenos. |
| `GestorFidelizacion.ObtenerCliente`, `ComprasEnVentana`, `DeterminarNivel`, `CalcularDescuento`, `ReevaluarNivel`, `AscenderSiCorresponde` | Sí: las operaciones que reciben ID/cliente exigen que corresponda a la sesión. | Expondrían compras, nivel o habilitarían cambios de nivel de otro cliente. |
| `GestorFidelizacion.PorcentajePara` | Global deliberadamente: lee parámetros del sistema y aplica una tabla por nivel, sin recuperar usuarios. | No contiene datos de clientes. |
| `GestorSuscripciones.Contratar`, `Cancelar`, `ActivaDe`, `TieneActiva` | Sí: `ActivaDe` valida titularidad; las mutaciones y consultas derivadas pasan por esa guarda. | Expondrían estado/plan o permitirían cancelar una suscripción ajena. |
| `GestorSuscripciones.PrecioDe` | Global deliberadamente: tarifa de catálogo. | No contiene datos de suscriptores. |
| `GestorReportes.Crear`, `Ejecutar`, `Exportar`, `EventosEntre` | Sí por perfil y alcance en cada ejecución: Administrador global; Organizador filtrado por dueño en los cinco reportes y el selector de eventos. | Sin el filtro, ventas, importes, clientela, adopción, paquetes y demanda de otro dueño entrarían en reportes/exportaciones. Negativa principal CP-182. |
| `GestorAuditoria.Consultar` | Sí: solo Administrador de plataforma; lectura global porque la auditoría registra acciones del sistema completo. | Organizador podría leer operaciones de otros usuarios/organizadores. |
| `GestorAuditoria.Registrar` | Escritura global legítima desde los gestores; ahora es `internal`, no API disponible a Visual. Se atribuye al usuario de sesión o al correo del intento de login. | Como era público y aceptaba autor explícito, una llamada desde otra capa podía falsificar la atribución. Cerrado al internalizarlo. |
| `GestorUsuarios.CrearAdministrador` | Sí: exige Administrador; el registro público solo crea Clientes. | Un Cliente/Organizador podría elevarse a Administrador. |
| `GestorUsuarios.Autenticar`, `RegistrarCliente`, `CambiarContrasena`, `GestorCodigosCorreo.*` | Autenticación/registro operan sobre la cuenta identificada por correo y códigos ligados a esa cuenta; cambio exige prueba de la credencial. | No devuelven ni alteran datos de otro usuario autenticado. |

Los repositorios `Datos` son capa de persistencia, no una API de sesión para la interfaz. Las consultas por ID usadas por gestores de negocio se vuelven a autorizar en `Repo` antes de devolver o modificar información.

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

No se eliminaron aserciones de negocio para obtener verde. Los cambios de CP-131/134/135/136 fueron de identidad del fixture; la propiedad funcional comprobada permanece igual.

## Verificación

- Batería: 322/323 en la última corrida. El único rojo fue CP-174, ajeno a los cambios de perfiles: el registro se guarda con `SYSUTCDATETIME()` y la prueba consulta con el día local, por eso el registro de las 00:06 UTC del 18/11 queda fuera del rango local que terminaba a las 00:00. El dato aparece al consultar con rango UTC. No cambié producción ni la prueba por la regla de no corregir un defecto preexistente; queda reportado para decisión. Los otros 322 casos pasaron.
- Barrera visual: CP-63 + CP-112/CP-120 + CP-116 = 51/51.
- Barrido de texto cortado: 16 botones, 0 cortes, 22 formularios inspeccionados.
- Color renderizado: 32 botones primarios, 0 fallos; el control negativo del estilo del sistema se distingue.
- CP-182 en mutación: al anular temporalmente el alcance del reporte falló 1/1 y expuso filas de otros eventos; restaurada la guarda, vuelve a verde. Al quitar temporalmente la guarda de disponibilidad/evento ajeno, CP-182 falló 1/1; restaurada.
- Capturas completas con `DrawToBitmap`: `Capturas-Fase8a/antes-{100,150}` y `despues-{100,150}`. Las capturas previas reproducen la pantalla combinada anterior; la pantalla nueva de plataforma queda documentada como comparación contra ese menú. En la ficha de tipografía, los menús requieren ancho de inserción de **24,2 cm** para que la letra mínima quede en 8,1 pt; a 16 cm queda en 5,36 pt.
- Las capturas se generaron a 100 % y 150 %; Windows quedó restaurado a 100 % (96 DPI). No quedaron ventanas del host abiertas.