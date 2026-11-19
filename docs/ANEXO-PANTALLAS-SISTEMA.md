# Anexo «Pantallas del sistema»

Claude Code, 19/11/2026. Fuente del anexo nuevo que pidió JC (18/11): el juego completo de
pantallas, una vista por pantalla, con su epígrafe. La lista para Máximo lo incluye desde acá.

**En pausa (19/11):** JC pidió que todo el anexo use datos de la semilla. Codex recaptura las
pantallas de Organizador y Administrador, que hoy muestran datos de las pruebas, y las seis que
faltan. Las cinco de «Mi cuenta» ya son las definitivas. Cuando lleguen, se rehacen las filas y se
miden de nuevo con `tools/Pantallas/Recortar.ps1` y el mismo cálculo.

## Cómo se eligió cada captura

- **Una sola versión por pantalla: la que coincide con cómo se ve hoy la aplicación al
  ejecutarla** (criterio de JC), no la más nueva por fecha. Se comparó cada captura con el último
  commit que cambió su formulario.
- **Las cinco pantallas del Cliente que se abren dentro de «Mi cuenta»** (eventos, ficha, compra,
  mis entradas y suscripción) usan las vistas que Codex capturó el 18/11 dentro de la ventana,
  con la semilla de Paula (`b9d7834`). Las capturas sueltas de esos formularios (`Fase7/` y
  `Fase8/frmEventosCliente`, `FrmCompraCliente`, `frmHistorialCliente`, `frmSuscripcion`,
  `ficha-evento`) ya no muestran cómo se ven: quedan fuera.
- **Sus recortes de detalle salen de esas mismas cinco vistas**, para que la vista completa y su
  detalle muestren los mismos datos. Los genera `tools/Pantallas/Recortar.ps1` en
  `docs/capturas/pantallas/`; el script mide cada recorte y falla si la letra impresa queda por
  debajo de 8 pt.
- **Las demás pantallas** usan la vista completa de `Fase8/` (o `Fase8d/` para la revisión de
  eventos) y los recortes de Codex de la misma fase de la captura (`Fase8b`, `Fase8c` o `Fase8d`).
- **Un solo mapa de sectores, en página apaisada** (decisión de JC): el de la ficha de «Mi
  cuenta», con los tres sectores del evento de Paula. El de `Fase7/ficha` mostraba un solo
  sector de otro evento y no coincidía con la ficha del anexo.
- **Vistas de disposición:** una vista completa que a 16 cm queda con letra menor de 8 pt se
  inserta igual, porque muestra cómo se organiza la pantalla, pero su epígrafe lo dice
  explícitamente y la siguen los recortes legibles (decisión de JC del 19/11).
- **Cuántas son:** 25 formularios dan 28 vistas. Se suman la ficha del evento, que es una vista
  dentro de «Mi cuenta» y no un formulario, y dos modos de pantallas que ya están en la lista:
  la recuperación de la contraseña (`frmCambiarContraseña`) y la creación de un administrador
  (`frmCrearUsuario`).

## Cómo se calcularon el ancho y la letra

La letra más chica de las capturas es de 9 pt a 96 ppp (medición de Codex en
`Pruebas/Recortes-Documento-Fases7-8/ancho-minimo-8_1pt.tsv`). Una imagen de *w* píxeles mide
*w* / 96 × 2,54 cm a tamaño real. Se inserta a ese tamaño si entra en 16 cm (24,7 cm en la
página apaisada), y si no, a 16 cm; la letra impresa es 9 pt × ancho insertado / tamaño real.

## Texto para pegar

**Título del anexo:** «Pantallas del sistema».

**Introducción (pegar debajo del título):**

> Este anexo presenta las pantallas del prototipo de la versión 2.6, una vista por pantalla,
> tal como se ven al ejecutar la aplicación. Se agrupan por perfil: acceso y cuenta, Cliente,
> Organizador y Administrador de la plataforma. Cada pantalla tiene una vista completa. Cuando
> esa vista no se puede leer a tamaño de página, el epígrafe la identifica como vista de
> disposición y la siguen recortes del panel central a tamaño legible. Los datos que se ven son
> los de demostración de la semilla y de la batería de pruebas, no datos reales.

**Las pantallas, en este orden.** Cada imagen va centrada, con su epígrafe debajo, al ancho de
la tabla. Si una figura y sus recortes no entran en la misma página, pasá los recortes a la
siguiente; no achiques ninguna imagen.

**Acceso y cuenta**

**Pantalla 1. Inicio de sesión (frmInicio).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P1 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmInicio.png` | 10,7 cm | 9,0 pt |
| P1.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmInicio/panel.png` | 10,3 cm | 9,0 pt |

**Epígrafes:**

> Figura P1 — Inicio de sesión: el acceso con correo y contraseña, y los caminos para crear una cuenta y recuperar la contraseña.
>
> Figura P1.1 — Detalle del formulario de acceso, en la pantalla «Inicio de sesión».

**Pantalla 2. Crear cuenta (frmCrearUsuario).** ⏸ Todavía no: Codex cambió la verificación del correo en `5a4b645` (el código va a un archivo que se abre fuera de la aplicación); falta la captura nueva. Dejá el título y seguí.

**Pantalla 3. Cambiar contraseña (frmCambiarContraseña).** ⏸ Todavía no: Codex la cambió en `5a4b645`; falta la captura nueva. Dejá el título y seguí.

**Pantalla 4. Recuperar contraseña (frmCambiarContraseña, modo recuperación).** ⏸ Todavía no: Codex cerró la recuperación en `5a4b645`; falta la captura nueva. Dejá el título y seguí.

**Cliente**

**Pantalla 5. Inicio de «Mi cuenta» (frmMenuCliente).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P5 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMenuCliente.png` | 16,0 cm | 4,5 pt (disposición) |
| P5.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmMenuCliente/inicio-cliente.png` | 11,4 cm | 9,0 pt |

**Epígrafes:**

> Figura P5 — Inicio de «Mi cuenta». Vista de disposición: muestra cómo se organiza la pantalla (el menú lateral del Cliente y el panel de inicio con su nivel, sus próximas entradas y su lista de espera) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P5.1 — Detalle del panel de inicio del Cliente, en el inicio de «Mi cuenta».

**Pantalla 6. Eventos disponibles (frmEventosCliente, dentro de «Mi cuenta»).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P6 | `Pruebas/Recortes-Documento-Fases7-8/completas/MiCuenta-2026-11-18/eventos.png` | 16,0 cm | 4,5 pt (disposición) |
| P6.1 | `docs/capturas/pantallas/eventos-tarjetas.png` | 15,2 cm | 9,0 pt |

**Epígrafes:**

> Figura P6 — Eventos disponibles. Vista de disposición: muestra cómo se organiza la pantalla (el catálogo de eventos publicados con sus filtros y los pasos de la compra) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P6.1 — Detalle de las tarjetas de los eventos, en la pantalla «Eventos disponibles».

**Pantalla 7. Ficha del evento (dentro de «Mi cuenta»).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P7 | `Pruebas/Recortes-Documento-Fases7-8/completas/MiCuenta-2026-11-18/ficha-evento.png` | 16,0 cm | 4,5 pt (disposición) |
| P7.1 | `docs/capturas/pantallas/ficha-portada-datos.png` | 16,0 cm | 8,4 pt |
| P7.2 | `docs/capturas/pantallas/ficha-mapa.png` | 22,8 cm **página apaisada** | 9,0 pt |

**Epígrafes:**

> Figura P7 — Ficha del evento. Vista de disposición: muestra cómo se organiza la pantalla (la portada, los datos del evento y el mapa de sectores) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P7.1 — Detalle de la portada y los datos del evento, en la pantalla «Ficha del evento».
>
> Figura P7.2 — Detalle del mapa de sectores, en la pantalla «Ficha del evento».

**Pantalla 8. Armado del paquete (FrmCompraCliente, dentro de «Mi cuenta»).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P8 | `Pruebas/Recortes-Documento-Fases7-8/completas/MiCuenta-2026-11-18/compra.png` | 16,0 cm | 4,5 pt (disposición) |
| P8.1 | `docs/capturas/pantallas/compra-entradas.png` | 10,5 cm | 9,0 pt |
| P8.2 | `docs/capturas/pantallas/compra-resumen.png` | 10,1 cm | 9,0 pt |

**Epígrafes:**

> Figura P8 — Armado del paquete. Vista de disposición: muestra cómo se organiza la pantalla (la elección de entradas por sector y el resumen del paquete con el descuento del nivel) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P8.1 — Detalle de las entradas y las cantidades, en la pantalla «Armado del paquete».
>
> Figura P8.2 — Detalle del resumen y el total del paquete, en la pantalla «Armado del paquete».

**Pantalla 9. Pago simulado (frmPago).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P9 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmPago.png` | 11,8 cm | 9,0 pt |
| P9.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmPago/panel.png` | 11,4 cm | 9,0 pt |

**Epígrafes:**

> Figura P9 — Pago simulado: los datos de la tarjeta para el pago simulado.
>
> Figura P9.1 — Detalle del formulario de pago, en la pantalla «Pago simulado».

**Pantalla 10. Confirmación de la compra (frmConfirmacion).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P10 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmConfirmacion.png` | 16,0 cm | 7,1 pt (disposición) |
| P10.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmConfirmacion/comprobante.png` | 8,5 cm | 9,0 pt |
| P10.2 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmConfirmacion/total.png` | 9,0 cm | 9,0 pt |
| P10.3 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmConfirmacion/entradas-pdf.png` | 9,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P10 — Confirmación de la compra. Vista de disposición: muestra cómo se organiza la pantalla (el pedido confirmado, su comprobante y las entradas en PDF con su código QR) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P10.1 — Detalle del desglose del comprobante, en la pantalla «Confirmación de la compra».
>
> Figura P10.2 — Detalle del total de la compra, en la pantalla «Confirmación de la compra».
>
> Figura P10.3 — Detalle de las entradas en PDF y su código QR, en la pantalla «Confirmación de la compra».

**Pantalla 11. Mis entradas (frmHistorialCliente, dentro de «Mi cuenta»).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P11 | `Pruebas/Recortes-Documento-Fases7-8/completas/MiCuenta-2026-11-18/mis-entradas.png` | 16,0 cm | 4,5 pt (disposición) |
| P11.1 | `docs/capturas/pantallas/mis-entradas-izquierda.png` | 12,8 cm | 9,0 pt |
| P11.2 | `docs/capturas/pantallas/mis-entradas-derecha.png` | 9,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P11 — Mis entradas. Vista de disposición: muestra cómo se organiza la pantalla (el historial de pedidos, las entradas de cada pedido y la devolución) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P11.1 — Detalle del pedido, el evento, la fecha y el estado, en la pantalla «Mis entradas».
>
> Figura P11.2 — Detalle de las entradas, el total pagado, lo devuelto, el seguro y el código, en la pantalla «Mis entradas».

**Pantalla 12. Mi suscripción (frmSuscripcion, dentro de «Mi cuenta»).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P12 | `Pruebas/Recortes-Documento-Fases7-8/completas/MiCuenta-2026-11-18/suscripcion.png` | 16,0 cm | 4,5 pt (disposición) |
| P12.1 | `docs/capturas/pantallas/suscripcion-panel.png` | 10,7 cm | 9,0 pt |

**Epígrafes:**

> Figura P12 — Mi suscripción. Vista de disposición: muestra cómo se organiza la pantalla (los planes, los beneficios de la suscripción y su contratación) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P12.1 — Detalle de los planes y los beneficios, en la pantalla «Mi suscripción».

**Pantalla 13. Solicitar ser Organizador (frmSolicitudOrganizador).** ⏸ Todavía no: Codex cambió la confirmación del teléfono en `5a4b645`; falta la captura nueva. Dejá el título y seguí.

**Organizador**

**Pantalla 14. Menú del Organizador (frmMenuPropietario).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P14 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMenuPropietario.png` | 16,0 cm | 5,4 pt (disposición) |
| P14.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmMenuPropietario/panel-organizador.png` | 16,0 cm | 8,4 pt |

**Epígrafes:**

> Figura P14 — Menú del Organizador. Vista de disposición: muestra cómo se organiza la pantalla (las operaciones del Organizador sobre sus locales) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P14.1 — Detalle del panel de organización, en la pantalla «Menú del Organizador».

**Pantalla 15. Mis locales (frmMisLocales).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P15 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMisLocales.png` | 16,0 cm | 6,4 pt (disposición) |
| P15.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmMisLocales/locales.png` | 13,9 cm | 9,0 pt |
| P15.2 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmMisLocales/capacidad.png` | 7,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P15 — Mis locales. Vista de disposición: muestra cómo se organiza la pantalla (los locales del Organizador) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P15.1 — Detalle del nombre y la dirección de los locales, en la pantalla «Mis locales».
>
> Figura P15.2 — Detalle de la capacidad de los locales, en la pantalla «Mis locales».

**Pantalla 16. Registrar local (frmRegistrarLocal).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P16 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmRegistrarLocal.png` | 11,4 cm | 9,0 pt |
| P16.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmRegistrarLocal/panel.png` | 11,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P16 — Registrar local: el alta de un local.
>
> Figura P16.1 — Detalle del formulario del local, en la pantalla «Registrar local».

**Pantalla 17. Eventos del local (frmEventosLocal).** ⏸ Todavía no: La captura disponible es anterior a la columna «Motivo del rechazo» (commit `40e91b3`). Codex tiene que recapturarla. Dejá el título y seguí.

**Pantalla 18. Agregar evento (frmAgregarEvento).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P18 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAgregarEvento.png` | 16,0 cm | 8,8 pt |
| P18.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmAgregarEvento/panel.png` | 15,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P18 — Agregar evento: el alta de un evento, que queda en borrador.
>
> Figura P18.1 — Detalle del formulario del evento, en la pantalla «Agregar evento».

**Pantalla 19. Administrar evento (frmAdministrarEvento).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P19 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAdministrarEvento.png` | 16,0 cm | 6,4 pt (disposición) |
| P19.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8c/frmAdministrarEvento/datos-y-sectores.png` | 15,5 cm | 9,0 pt |
| P19.2 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8c/frmAdministrarEvento/portada.png` | 5,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P19 — Administrar evento. Vista de disposición: muestra cómo se organiza la pantalla (la preventa, los sectores, la portada y la publicación del evento) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P19.1 — Detalle de la preventa y los sectores, en la pantalla «Administrar evento».
>
> Figura P19.2 — Detalle de la portada y la categoría, en la pantalla «Administrar evento».

**Pantalla 20. Agregar sector (frmAgregarSector).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P20 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAgregarSector.png` | 11,4 cm | 9,0 pt |
| P20.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmAgregarSector/panel.png` | 11,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P20 — Agregar sector: el alta de un sector con su capacidad y su precio.
>
> Figura P20.1 — Detalle del formulario del sector, en la pantalla «Agregar sector».

**Pantalla 21. Control de ingreso (frmControlIngreso).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P21 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmControlIngreso.png` | 16,0 cm | 7,4 pt (disposición) |
| P21.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmControlIngreso/formulario.png` | 16,0 cm | 8,2 pt |

**Epígrafes:**

> Figura P21 — Control de ingreso. Vista de disposición: muestra cómo se organiza la pantalla (la validación del código de una entrada en la puerta) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P21.1 — Detalle del control de ingreso, en la pantalla «Control de ingreso».

**Pantalla 22. Listas de espera (frmListasEspera).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P22 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmListasEspera.png` | 16,0 cm | 6,4 pt (disposición) |
| P22.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmListasEspera/estado-lista.png` | 15,1 cm | 9,0 pt |

**Epígrafes:**

> Figura P22 — Listas de espera. Vista de disposición: muestra cómo se organiza la pantalla (las listas de espera de los eventos del Organizador) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P22.1 — Detalle de la lista y su estado vacío, en la pantalla «Listas de espera».

**Pantalla 23. Reportes (frmReportes).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P23 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmReportes.png` | 16,0 cm | 6,0 pt (disposición) |
| P23.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmReportes/estado-reporte.png` | 8,6 cm | 9,0 pt |

**Epígrafes:**

> Figura P23 — Reportes. Vista de disposición: muestra cómo se organiza la pantalla (los reportes de ventas limitados a los eventos del Organizador) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P23.1 — Detalle del estado vacío y la acción para generar un reporte, en la pantalla «Reportes».

**Administrador de la plataforma**

**Pantalla 24. Panel de plataforma (frmMenuAdministrador).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P24 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMenuAdministrador.png` | 16,0 cm | 5,4 pt (disposición) |
| P24.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8c/frmMenuAdministrador/indicadores-izquierda.png` | 11,9 cm | 9,0 pt |
| P24.2 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8c/frmMenuAdministrador/indicadores-derecha.png` | 11,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P24 — Panel de plataforma. Vista de disposición: muestra cómo se organiza la pantalla (las operaciones del Administrador y los indicadores globales) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P24.1 — Detalle de los indicadores globales, columna izquierda, en la pantalla «Panel de plataforma».
>
> Figura P24.2 — Detalle de los indicadores globales, columna derecha, en la pantalla «Panel de plataforma».

**Pantalla 25. Crear administrador (frmCrearUsuario, modo administrador).** ⏸ Todavía no: No hay captura de este modo. Es la misma pantalla que la 2, con otro título y otro botón; se captura junto con ella. Dejá el título y seguí.

**Pantalla 26. Solicitudes de organizador (frmRevisarSolicitudesOrganizador).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P26 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmRevisarSolicitudesOrganizador.png` | 16,0 cm | 4,6 pt (disposición) |
| P26.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmRevisarSolicitudesOrganizador/lista.png` | 7,9 cm | 9,0 pt |
| P26.2 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmRevisarSolicitudesOrganizador/datos.png` | 11,4 cm | 9,0 pt |
| P26.3 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmRevisarSolicitudesOrganizador/documento.png` | 9,5 cm | 9,0 pt |

**Epígrafes:**

> Figura P26 — Solicitudes de organizador. Vista de disposición: muestra cómo se organiza la pantalla (la revisión de una solicitud con los datos del responsable, del local y la documentación) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P26.1 — Detalle de la lista de solicitudes, en la pantalla «Solicitudes de organizador».
>
> Figura P26.2 — Detalle de los datos del responsable y del local, en la pantalla «Solicitudes de organizador».
>
> Figura P26.3 — Detalle del documento adjunto, en la pantalla «Solicitudes de organizador».

**Pantalla 27. Eventos en revisión (frmRevisarEventos).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P27 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8d/frmRevisarEventos.png` | 16,0 cm | 5,1 pt (disposición) |
| P27.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8d/frmRevisarEventos/lista.png` | 16,0 cm | 8,9 pt |
| P27.2 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8d/frmRevisarEventos/revision.png` | 9,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P27 — Eventos en revisión. Vista de disposición: muestra cómo se organiza la pantalla (la aprobación o el rechazo, con motivo, del primer evento de un Organizador) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P27.1 — Detalle de la lista de eventos en revisión, en la pantalla «Eventos en revisión».
>
> Figura P27.2 — Detalle de los datos, el motivo y las acciones, en la pantalla «Eventos en revisión».

**Pantalla 28. Registro de auditoría (frmAuditoria).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P28 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAuditoria.png` | 16,0 cm | 5,9 pt (disposición) |
| P28.1 | `Pruebas/Recortes-Documento-Fases7-8/paneles/Fase8b/frmAuditoria/estado-vacio.png` | 8,7 cm | 9,0 pt |

**Epígrafes:**

> Figura P28 — Registro de auditoría. Vista de disposición: muestra cómo se organiza la pantalla (la consulta del registro de auditoría por período y acción) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P28.1 — Detalle de los filtros y el estado vacío, en la pantalla «Registro de auditoría».

## Fin del texto para pegar
