# Investigación de preventa y cierre de T-45 / T-46

## Preventa: categoría B, presentación sin efectos en datos

Se reprodujo por la interfaz real: acceso del administrador, alta de local,
evento con preventa y sector, regreso al inicio, registro de cliente sin plan,
acceso del cliente, selección del evento y Comprar. `InvestigacionPreventaReal.cs`
invoca el `Program.Main` original con su bucle de mensajes real y automatiza los
controles. No usa Fake ni escrituras SQL para preparar ese recorrido. Las
contraseñas nuevas se generan en memoria, sin literales ni registro en archivos.
Se utiliza exclusivamente el catálogo de pruebas, con configuración copiada al
ejecutable de investigación; ningún App.config fuente fue modificado.

Después de aceptar RN-39 quedan visibles Eventos disponibles y Armar paquete.
La segunda es una ventana vacía: cero filas de sectores y título «Seleccioná tus
entradas». No es una fila de compra en el historial.

| Comprobación para el cliente/evento del recorrido | Antes | Después |
|---|---:|---:|
| Compras | 0 | 0 |
| Entradas | 0 | 0 |
| QR de entradas confirmadas | 0 | 0 |
| Reservas vigentes | 0 | 0 |
| Pagos | 0 | 0 |
| Total pagado | 0 | 0 |
| Seguros | 0 | 0 |
| Compras computadas para fidelización | 0 | 0 |
| Disponibles del sector | 3 | 3 |

No existe registro de compra ni estado que informar; tampoco entrada ni QR que
presentar, ocupación de lugares, cobro o efecto en descuentos. Los registros de
preparación (usuario, local, evento y sector) sí existen porque se dieron de alta
normalmente: no son compras rechazadas. La corrida completa posterior recreó la
base de pruebas.

RN-39 está cumplida: `GestorPaquetes.Nuevo` valida la preventa antes de construir
el paquete y `GestorCompras.Iniciar` vuelve a validarla antes de insertar. No se
producen efectos sobre RN-15, RN-17/18 ni RN-19/24. Entre las 38 reglas no hay una
que exija cerrar esa ventana vacía: queda una decisión de navegación/presentación
(RNF-02), sin corregir. Esto describe el recorrido rechazado observado; no es una
auditoría general de todos los posibles bypass.

Evidencia local: `TestResults/PreventaReal/recorrido.txt`, `FrmCompraCliente.png`
y `frmEventosCliente.png`. Identificadores de esa corrida: evento 62, usuario 77,
sector 72. El investigador queda como herramienta explícita, no como otro caso
MSTest que altere el número de ejecuciones.

## Resultados de las tandas

Todas las corridas usaron `tools\Proyecto.ps1 -Accion Probar`.

| Tanda | Ejecuciones | Correctas | Fallidas |
|---|---:|---:|---:|
| T-45 | 119 | 113 | 6 |
| T-46 | 136 | 129 | 7 |
| Verificación completa repetida de T-46 | 136 | 129 | 7 |

La batería no está completamente verde. Se conservaron los cinco fallos conocidos
(CP-69, CP-71, CP-75, CP-76 y CP-81) y aparecieron dos visuales. Ninguno fue
corregido, ignorado ni convertido en una aserción permisiva.

### T-45: PA-18 a PA-20

- CP-83, dos ejecuciones: listado de espera agotado y propietario sin locales.
- CP-84, una ejecución: prioridad de nivel y conservación de la suscripción
  registrada al inscribirse después de cancelar el plan.
- CP-85, tres ejecuciones: menú propietario, menú cliente y separación de acceso
  a datos respecto de la interfaz.

Hallazgo visual nuevo: CP-85 encuentra «Volver» fuera del área cliente del menú
del propietario a 150 %. Es `frmMenuPropietario`, una de las otras 18 pantallas,
no el piloto. El fallo previo CP-69 corresponde a `frmAgregarEvento`.

Archivos de la tanda: `PruebasGrupoAListasYMenus.cs` y `Fake.cs`. El ayudante nuevo
crea un propietario válido sin locales; la limpieza elimina su fila Propietario
antes del usuario para mantener el aislamiento de ese escenario.

### T-46: PA-21 a PA-25

- CP-86: reportes con semilla sin volumen.
- CP-87: cinco ejecuciones de selector, período, filtros y estado de exportación.
- CP-88: ocho ejecuciones de cifras, cortes y presentación de reportes.
- CP-89: dos ejecuciones de exportación y destino bloqueado.
- CP-90: contenido del PDF exportado desde la interfaz.

Los datos de `03-datos-reporte.sql` se cargan únicamente en la clase que los
necesita, con guardia de catálogo `_Pruebas`. Se limpian por local y patrón de
usuarios del script. Comprobación SQL posterior a la segunda corrida: cero
locales de reportes y cero usuarios del patrón. No quedaron carpetas temporales
`EventAura-reportes-*`.

Hallazgo visual nuevo: CP-88 detecta que `lblLectura` de `frmReportes` no tiene
alto suficiente para mostrar su texto completo. Las aserciones de cifras pasaron.
Se deja sin corregir. No apareció un hallazgo nuevo de negocio, datos o seguridad.

Archivos de la tanda: `PruebasGrupoAReportes.cs`, `BaseDePrueba.cs` y `Fake.cs`.
Excepción de producción autorizada: `Visual/Propietario/frmReportes.cs` permite
sobrescribir internamente la carpeta de exportación y
`Visual/Properties/AssemblyInfo.cs` permite acceder desde Pruebas. Las pruebas
asignan una carpeta temporal antes de mostrar la ventana; sin esa asignación la
aplicación conserva exactamente `Documentos/A1.Reportes EVENTAURA`. Se comprueba
también ese valor por omisión. No se exportaron estos reportes a Documentos ni
se abrió un visor externo.

PA-21 usa el nombre actual del evento 1 («Duki en Rosario»), no la denominación
vieja «Festival» del encargo; conserva la comprobación de cero entradas y ticket
sin dato. Las fechas de los casos de volumen son las del conjunto de datos SQL.

Logs: `TestResults/Tandas7/T45.txt`, `T46-inicial.txt` y `T46-verificacion.txt`.

## Entrega a Claude Code y límites

Revisar los siete fallos pendientes, incorporar esta evidencia y decidir la
navegación de la preventa vacía. Los nuevos hallazgos visuales son CP-85 y CP-88;
no se han adelantado arreglos de 6B. La extracción del PDF verifica contenido;
la revisión visual impresa y el escaneo físico siguen siendo comprobaciones
manuales, no se dan por realizados aquí.

El plan y la clasificación están siendo revisados simultáneamente por Claude.
Se siguió el encargo directo de JC: conservar los cinco fallos y alcanzar 119 y
136 ejecuciones; no se adaptaron pruebas anteriores a cambios concurrentes de
documentación. Este reporte no modifica esos documentos.

También se agregó `InvestigacionPreventaReal.cs` y este reporte en Pruebas/.
El diff previo de cuatro acentos en comentarios de FrmCompraCliente permanece;
no pertenece a estas dos tandas. Los archivos de pruebas e informes anteriores
que ya estaban sin seguimiento tampoco se atribuyen a este trabajo.

Confirmaciones: ningún Designer modificado; ningún App.config modificado;
ningún archivo de docs/, tools/, CLAUDE.md o AGENTS.md editado por este trabajo.
Las modificaciones concurrentes de Claude se conservaron. Sin commits ni push.
T-41 y 6B no se iniciaron. Trabajo detenido al terminar T-46.
