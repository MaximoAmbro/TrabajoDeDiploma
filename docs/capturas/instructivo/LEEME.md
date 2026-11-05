# Capturas del instructivo (PDF, páginas 52 y 53)

Las cuatro figuras que el documento deja como «Espacio reservado para captura de pantalla».
Se tomaron el 04/11/2026 con la aplicación real, a escala 100 % (96 DPI), sobre
`BddEventAura_Pruebas` recién recreada con la semilla de T-21. Reproducen el ejemplo de la
página 54: evento «Duki en Rosario», clienta Paula, nivel Plata con 4 compras, 2 entradas de
Campo a $25.000 y hospedaje de $45.000. El resumen coincide línea por línea con la tabla de
esa página: base $50.000, descuento −$2.500, subtotal $47.500, hospedaje $45.000, cargo
$7.125, **total $99.625**. Para regenerarlas: `tools/CapturasInstructivo/Generar.ps1`.

| Archivo | Epígrafe en el PDF | Qué muestra |
|---|---|---|
| `figura1-listado-eventos.png` | Figura 1 — Listado de eventos disponibles (pasos 1 y 2) | Buscador con texto guía, filtros de categoría y fecha, listado de eventos vigentes con «Duki en Rosario» seleccionado |
| `figura2-armado-paquete.png` | Figura 2 — Pantalla de armado de paquete (pasos 3 a 5) | Sectores con precio, disponibles y cantidad (2 en Campo), servicios con precio (hospedaje tildado), seguro sin tildar, nivel Plata con el enlace a los niveles, resumen y botones |
| `figura3-resumen-descuento.png` | Figura 3 — Resumen del paquete con el descuento aplicado (paso 6) | El panel de resumen recortado: base $50.000, descuento Plata 5 % −$2.500, hospedaje sin descuento $45.000, cargo $7.125, **total $99.625** y la leyenda de RN-21 |
| `figura4-confirmacion-y-comprobante.png` | Figura 4 — Confirmación de compra y comprobante emitido (pasos 7 y 8) | A la izquierda, «¡Compra confirmada!» con el pedido, el desglose, la tarjeta enmascarada y el token; a la derecha, el ticket PDF de una de las entradas, con su código y su QR |
| `figura4a-pago-simulado.png` | No tiene figura propia en el PDF | La pantalla de pago del paso 7, con el temporizador de la reserva. Opcional: sirve si se quiere separar el paso 7 del 8 |

## Observaciones para quien pegue las figuras en el documento

1. **Reproducen el ejemplo de la página 54**, con la semilla alineada en T-21. **Son las
   definitivas** (ver el punto 4). Un docente puede reproducirlas siguiendo el instructivo:
   iniciar sesión como `plata@eventaura.com`, elegir «Duki en Rosario», 2 entradas de Campo
   y el hospedaje.
2. **La ruta de la figura 4 es neutra:** `C:\Users\Cliente\Documents\A1.Tickets EVENTAURA`.
   El programa de captura reemplaza la carpeta real del usuario antes de fotografiar.
3. **Todo lo que hay que cambiar en el texto del documento** (por ejemplo, el estado de
   implementación de la página 50) está en `docs/CORRECCIONES-DOCUMENTO.md`.

4. **Definitivas: regeneradas el 05/11/2026, después de T-29.** La figura 2 muestra el
   encabezado «Cantidad (escribí aquí)» completo, en dos renglones (antes se cortaba: hallazgo
   U-14), y el cursor en la cantidad de Campo. **Si cambia la visual de las pantallas, hay que
   regenerarlas** con `Generar.ps1`.
5. **`Generar.ps1` necesita `pymupdf`** para pasar el ticket a imagen. En esta PC está
   instalado en `D:\jc-tools`: correr con `$env:PYTHONPATH = 'D:\jc-tools'`.
