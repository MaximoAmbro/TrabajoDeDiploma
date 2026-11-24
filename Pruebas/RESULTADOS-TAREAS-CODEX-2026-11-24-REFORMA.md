# Reforma visual de EventAura

La reforma reúne las 25 ventanas en una misma base WinForms. Usa una barra superior de 56 px lógicos, botones de acción de 36 px con 160 px de ancho mínimo en el tema y laterales de 240 px con opciones de 240 × 40 px. Las ventanas de acceso tienen campos de 320 × 32 px con su rótulo 4 px arriba. El aviso de validación de contraseña ocupa una fila que se colapsa al corregir el campo.

Los tres inicios comparten `DatoPanel`. El cliente consulta entradas próximas, fidelidad y lista de espera. El organizador consulta locales, eventos y entradas confirmadas de su cuenta. Administración conserva los cinco indicadores y muestra debajo los dos recuentos de revisión. Sus acciones, incluida Crear administrador, están en el lateral.

## Verificación

- `TestResults/reforma-final-360.log` contiene el resultado de la batería.
- `TestResults/reforma-captura-150.log` registra las 25 ventanas y los 144 DPI informados por Windows. Se usó un host PerMonitorV2 para capturar la pantalla física y se restauró la escala inicial al terminar.
- `TestResults/Capturas-Reforma/100` y `TestResults/Capturas-Reforma/150` contienen los PNG de las mismas 25 ventanas. `Pruebas/CapturarReforma.ps1` reproduce la captura con Windows configurado en la escala solicitada.
- CP-357 mide la barra y los laterales, verifica grupos de acciones y los campos de acceso, y busca solapamientos entre controles visibles con texto e interactivos. También verifica que la fila de aviso de contraseña crezca y se colapse. Enter en Suscripción conserva la verificación manual por el modal.

## Archivos de diseñador

- `Visual/Cliente/frmEventosCliente.Designer.cs` baja las acciones de búsqueda a una fila propia.
- `Visual/Propietario/PanelAdministrador.Designer.cs` deja espacio para ordenar los indicadores arriba y la revisión debajo.
- `Visual/Propietario/frmMenuPropietario.Designer.cs` elimina los dos títulos duplicados solicitados.
- `Visual/_Inicio/frmInicio.Designer.cs` cambia el título de contenido a Iniciar sesión.

## Pendientes

La página de Claude no entregó el contenido de las tres plantillas. Falta el contraste visual contra esa referencia y completar la adaptación de selectores nativos y grupos de acciones heredados al sistema exacto. Las verificaciones intermedias encontraron regresiones que se corrigieron en conjunto. El registro de 360 corresponde al estado integrado, sin certificar 360 al cierre individual de cada fase.
