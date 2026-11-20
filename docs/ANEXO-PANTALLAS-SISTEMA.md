# Anexo «Pantallas del sistema»

Claude Code, 19/11/2026. Fuente del anexo nuevo que pidió JC (18/11): el juego completo de
pantallas, una vista por pantalla, con su epígrafe. La lista para Máximo lo incluye desde acá.

**En pausa (20/11):** 24 de las 28 pantallas están listas. Codex rehízo «Crear administrador»
(`c4ab61e`), y «Crear cuenta» y «Recuperar contraseña» (`ee01a85`): el indicador de fortaleza sale
completo, el aviso del código ya no tapa la etiqueta de la contraseña nueva, y las dos se tomaron
sin sesión, con la barra superior solo con la marca. Faltan cuatro del Cliente que siguen
mostrando datos de las pruebas: inicio de sesión, inicio de «Mi cuenta», pago y confirmación. El
anexo se pega entero cuando estén las cuatro.

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
- **Las demás pantallas** usan la vista completa de `Fase8/`, que Codex recapturó con la semilla
  el 19/11 (`037c6b4`). Sus recortes de detalle también los genera `tools/Pantallas/Recortar.ps1`
  desde esas mismas vistas: los recortes de `paneles/` de Codex son de las capturas anteriores,
  con datos de las pruebas, y ya no coinciden.
- **Sin recortes cuando no hacen falta:** una vista completa que se lee a 8 pt o más va sola; los
  recortes acompañan solo a las vistas de disposición.
- **«Reportes» va con el Administrador:** la captura es la del Administrador («Ana
  Administradora»). El Organizador abre la misma pantalla limitada a sus eventos, y el epígrafe
  lo dice.
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
> los de demostración de la semilla, no datos reales.

**Las pantallas, en este orden.** Cada imagen va centrada, con su epígrafe debajo, al ancho de
la tabla. Si una figura y sus recortes no entran en la misma página, pasá los recortes a la
siguiente; no achiques ninguna imagen.

**Acceso y cuenta**

**Pantalla 1. Inicio de sesión (frmInicio).** ⏸ Todavía no: La captura disponible es de la batería de pruebas (en la barra superior se lee «Cliente Prueba 25»), no de la semilla. Codex tiene que recapturarla con la semilla. Dejá el título y seguí.

**Pantalla 2. Crear cuenta (frmCrearUsuario).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P2 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmCrearUsuario.png` | 11,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P2 — Crear cuenta: el alta de un Cliente, con la fortaleza de la contraseña a la vista.

**Pantalla 3. Cambiar contraseña (frmCambiarContraseña).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P3 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmCambiarContraseña.png` | 11,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P3 — Cambiar contraseña: el cambio de la contraseña con la actual, la nueva y su fortaleza.

**Pantalla 4. Recuperar contraseña (frmCambiarContraseña, modo recuperación).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P4 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmRecuperarContraseña.png` | 11,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P4 — Recuperar contraseña: el pedido del código de recuperación, que se envía fuera de la aplicación, y la contraseña nueva.

**Cliente**

**Pantalla 5. Inicio de «Mi cuenta» (frmMenuCliente).** ⏸ Todavía no: La captura disponible es de la batería de pruebas (en la barra superior se lee «Cliente Prueba 32»), no de la semilla. Codex tiene que recapturarla con la semilla. Dejá el título y seguí.

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

**Pantalla 9. Pago simulado (frmPago).** ⏸ Todavía no: La captura disponible es de la batería de pruebas (en la barra superior se lee «Cliente Prueba 38»), no de la semilla. Codex tiene que recapturarla con la semilla. Dejá el título y seguí.

**Pantalla 10. Confirmación de la compra (frmConfirmacion).** ⏸ Todavía no: La captura disponible es de la batería de pruebas (en la barra superior se lee «Cliente Prueba 13»), no de la semilla. Codex tiene que recapturarla con la semilla. Muestra además una ruta de `TestResults`. Dejá el título y seguí.

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

**Pantalla 13. Solicitar ser Organizador (frmSolicitudOrganizador).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P13 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmSolicitudOrganizador.png` | 16,0 cm | 5,9 pt (disposición) |
| P13.1 | `docs/capturas/pantallas/solicitud-responsable.png` | 10,8 cm | 9,0 pt |
| P13.2 | `docs/capturas/pantallas/solicitud-codigo.png` | 10,9 cm | 9,0 pt |
| P13.3 | `docs/capturas/pantallas/solicitud-local-1.png` | 10,8 cm | 9,0 pt |
| P13.4 | `docs/capturas/pantallas/solicitud-local-2.png` | 10,9 cm | 9,0 pt |
| P13.5 | `docs/capturas/pantallas/solicitud-envio.png` | 15,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P13 — Solicitar ser Organizador. Vista de disposición: muestra cómo se organiza la pantalla (los datos del responsable, la confirmación del teléfono con un código que llega a un archivo de correo simulado, los datos del local y la documentación) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P13.1 — Detalle del responsable y el teléfono, en la pantalla «Solicitar ser Organizador».
>
> Figura P13.2 — Detalle del CUIT y la confirmación del teléfono, en la pantalla «Solicitar ser Organizador».
>
> Figura P13.3 — Detalle del nombre y la capacidad del local, en la pantalla «Solicitar ser Organizador».
>
> Figura P13.4 — Detalle de la dirección y el CBU, en la pantalla «Solicitar ser Organizador».
>
> Figura P13.5 — Detalle de la documentación y el envío de la solicitud, en la pantalla «Solicitar ser Organizador».

**Organizador**

**Pantalla 14. Menú del Organizador (frmMenuPropietario).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P14 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMenuPropietario.png` | 16,0 cm | 5,4 pt (disposición) |
| P14.1 | `docs/capturas/pantallas/organizador-menu.png` | 7,7 cm | 9,0 pt |
| P14.2 | `docs/capturas/pantallas/organizador-bienvenida.png` | 14,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P14 — Menú del Organizador. Vista de disposición: muestra cómo se organiza la pantalla (las operaciones del Organizador sobre sus locales) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P14.1 — Detalle de las operaciones del Organizador, en la pantalla «Menú del Organizador».
>
> Figura P14.2 — Detalle del panel de bienvenida, en la pantalla «Menú del Organizador».

**Pantalla 15. Mis locales (frmMisLocales).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P15 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMisLocales.png` | 16,0 cm | 6,4 pt (disposición) |
| P15.1 | `docs/capturas/pantallas/locales-nombre-direccion.png` | 14,0 cm | 9,0 pt |
| P15.2 | `docs/capturas/pantallas/locales-capacidad.png` | 7,2 cm | 9,0 pt |

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

**Epígrafes:**

> Figura P16 — Registrar local: el alta de un local.

**Pantalla 17. Eventos del local (frmEventosLocal).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P17 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmEventosLocal.png` | 16,0 cm | 6,4 pt (disposición) |
| P17.1 | `docs/capturas/pantallas/eventos-local-tabla.png` | 13,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P17 — Eventos del local. Vista de disposición: muestra cómo se organiza la pantalla (los eventos de un local con su estado y el motivo del rechazo, y sus acciones) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P17.1 — Detalle de los eventos del local, su estado y el motivo del rechazo, en la pantalla «Eventos del local».

**Pantalla 18. Agregar evento (frmAgregarEvento).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P18 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAgregarEvento.png` | 16,0 cm | 8,8 pt |

**Epígrafes:**

> Figura P18 — Agregar evento: el alta de un evento, que queda en borrador.

**Pantalla 19. Administrar evento (frmAdministrarEvento).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P19 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAdministrarEvento.png` | 16,0 cm | 6,4 pt (disposición) |
| P19.1 | `docs/capturas/pantallas/administrar-preventa-sectores.png` | 9,5 cm | 9,0 pt |
| P19.2 | `docs/capturas/pantallas/administrar-portada.png` | 11,4 cm | 9,0 pt |

**Epígrafes:**

> Figura P19 — Administrar evento. Vista de disposición: muestra cómo se organiza la pantalla (la preventa, los sectores y la portada del evento) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P19.1 — Detalle de la preventa y los sectores, en la pantalla «Administrar evento».
>
> Figura P19.2 — Detalle de la portada y la categoría, en la pantalla «Administrar evento».

**Pantalla 20. Agregar sector (frmAgregarSector).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P20 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAgregarSector.png` | 11,4 cm | 9,0 pt |

**Epígrafes:**

> Figura P20 — Agregar sector: el alta de un sector con su capacidad y su precio.

**Pantalla 21. Control de ingreso (frmControlIngreso).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P21 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmControlIngreso.png` | 16,0 cm | 7,4 pt (disposición) |
| P21.1 | `docs/capturas/pantallas/control-ingreso.png` | 15,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P21 — Control de ingreso. Vista de disposición: muestra cómo se organiza la pantalla (la validación de las entradas en la puerta, aquí sin eventos con el ingreso abierto) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P21.1 — Detalle del control de ingreso sin eventos abiertos, en la pantalla «Control de ingreso».

**Pantalla 22. Listas de espera (frmListasEspera).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P22 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmListasEspera.png` | 16,0 cm | 6,4 pt (disposición) |
| P22.1 | `docs/capturas/pantallas/listas-espera.png` | 15,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P22 — Listas de espera. Vista de disposición: muestra cómo se organiza la pantalla (las listas de espera de los eventos del Organizador) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P22.1 — Detalle del evento elegido y el estado de su lista, en la pantalla «Listas de espera».

**Administrador de la plataforma**

**Pantalla 23. Panel de plataforma (frmMenuAdministrador).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P23 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmMenuAdministrador.png` | 16,0 cm | 5,4 pt (disposición) |
| P23.1 | `docs/capturas/pantallas/administrador-operaciones-1.png` | 11,1 cm | 9,0 pt |
| P23.2 | `docs/capturas/pantallas/administrador-operaciones-2.png` | 13,9 cm | 9,0 pt |
| P23.3 | `docs/capturas/pantallas/administrador-indicadores-1.png` | 12,2 cm | 9,0 pt |
| P23.4 | `docs/capturas/pantallas/administrador-indicadores-2.png` | 12,2 cm | 9,0 pt |

**Epígrafes:**

> Figura P23 — Panel de plataforma. Vista de disposición: muestra cómo se organiza la pantalla (las operaciones del Administrador y los indicadores globales) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P23.1 — Detalle de las operaciones, primera parte, en la pantalla «Panel de plataforma».
>
> Figura P23.2 — Detalle de las operaciones, segunda parte, en la pantalla «Panel de plataforma».
>
> Figura P23.3 — Detalle de los indicadores globales, columna izquierda, en la pantalla «Panel de plataforma».
>
> Figura P23.4 — Detalle de los indicadores globales, columna derecha, en la pantalla «Panel de plataforma».

**Pantalla 24. Reportes (frmReportes).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P24 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmReportes.png` | 16,0 cm | 6,0 pt (disposición) |
| P24.1 | `docs/capturas/pantallas/reportes-filtros-1.png` | 11,8 cm | 9,0 pt |
| P24.2 | `docs/capturas/pantallas/reportes-filtros-2.png` | 10,6 cm | 9,0 pt |
| P24.3 | `docs/capturas/pantallas/reportes-tabla-1.png` | 11,4 cm | 9,0 pt |
| P24.4 | `docs/capturas/pantallas/reportes-tabla-2.png` | 11,4 cm | 9,0 pt |
| P24.5 | `docs/capturas/pantallas/reportes-lectura.png` | 12,0 cm | 9,0 pt |

**Epígrafes:**

> Figura P24 — Reportes. Vista de disposición: muestra cómo se organiza la pantalla (los reportes de gestión por período y categoría, con su exportación a PDF; el Organizador ve la misma pantalla limitada a sus eventos) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P24.1 — Detalle del reporte y la categoría, en la pantalla «Reportes».
>
> Figura P24.2 — Detalle del período y la generación, en la pantalla «Reportes».
>
> Figura P24.3 — Detalle de la tabla del reporte, primeras columnas, en la pantalla «Reportes».
>
> Figura P24.4 — Detalle de la tabla del reporte, últimas columnas, en la pantalla «Reportes».
>
> Figura P24.5 — Detalle de la lectura del reporte, en la pantalla «Reportes».

**Pantalla 25. Crear administrador (frmCrearUsuario, modo administrador).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P25 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmCrearAdministrador.png` | 11,8 cm | 9,0 pt |

**Epígrafes:**

> Figura P25 — Crear administrador: el alta de otro Administrador de la plataforma, que solo puede hacer un Administrador.

**Pantalla 26. Solicitudes de organizador (frmRevisarSolicitudesOrganizador).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P26 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmRevisarSolicitudesOrganizador.png` | 16,0 cm | 4,6 pt (disposición) |
| P26.1 | `docs/capturas/pantallas/solicitudes-datos.png` | 16,0 cm | 8,9 pt |
| P26.2 | `docs/capturas/pantallas/solicitudes-documento.png` | 9,7 cm | 9,0 pt |
| P26.3 | `docs/capturas/pantallas/solicitudes-acciones.png` | 15,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P26 — Solicitudes de organizador. Vista de disposición: muestra cómo se organiza la pantalla (la revisión de una solicitud con los datos del responsable, del local y la documentación) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P26.1 — Detalle de la lista y los datos de la solicitud, en la pantalla «Solicitudes de organizador».
>
> Figura P26.2 — Detalle de la documentación adjunta, en la pantalla «Solicitudes de organizador».
>
> Figura P26.3 — Detalle del motivo y las acciones, en la pantalla «Solicitudes de organizador».

**Pantalla 27. Eventos en revisión (frmRevisarEventos).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P27 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmRevisarEventos.png` | 16,0 cm | 5,1 pt (disposición) |
| P27.1 | `docs/capturas/pantallas/revision-eventos.png` | 9,6 cm | 9,0 pt |

**Epígrafes:**

> Figura P27 — Eventos en revisión. Vista de disposición: muestra cómo se organiza la pantalla (la aprobación o el rechazo, con motivo, del primer evento de un Organizador; la semilla no tiene eventos pendientes) y no está pensada para leerse a tamaño de página. El detalle legible está en la figura siguiente.
>
> Figura P27.1 — Detalle del estado, el motivo del rechazo y las acciones, en la pantalla «Eventos en revisión».

**Pantalla 28. Registro de auditoría (frmAuditoria).**

| Figura | Archivo | Ancho | Letra impresa |
|---|---|---|---|
| P28 | `Pruebas/Recortes-Documento-Fases7-8/completas/Fase8/frmAuditoria.png` | 16,0 cm | 5,9 pt (disposición) |
| P28.1 | `docs/capturas/pantallas/auditoria-1.png` | 15,3 cm | 9,0 pt |
| P28.2 | `docs/capturas/pantallas/auditoria-2.png` | 13,9 cm | 9,0 pt |

**Epígrafes:**

> Figura P28 — Registro de auditoría. Vista de disposición: muestra cómo se organiza la pantalla (la consulta del registro de auditoría por período y acción) y no está pensada para leerse a tamaño de página. El detalle legible está en las figuras siguientes.
>
> Figura P28.1 — Detalle de los filtros y las primeras columnas, en la pantalla «Registro de auditoría».
>
> Figura P28.2 — Detalle del filtro de acción y las últimas columnas, en la pantalla «Registro de auditoría».

## Fin del texto para pegar
