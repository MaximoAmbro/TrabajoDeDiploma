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

## Cotejo posterior con el sistema visual

La referencia está en `docs/SISTEMA-VISUAL.md`. Se revisaron las 25 capturas al 150 %. Los tres inicios comparten una grilla de tarjetas de 208 px con separación de 16 px y altura común por fila. Los datos auxiliares de las pruebas se limpian entre capturas exclusivamente en la base de pruebas. Administración conserva los indicadores y los pendientes en columnas de igual ancho. El organizador tiene dos secciones, Mis locales y Próximos eventos. El lateral del cliente queda sin Atrás.

El registro público muestra la marca sin identidad de sesión. Mail usa el ancho de las dos columnas y el medidor ocupa el ancho del campo de contraseña. El botón de acceso coincide con sus campos. También se corrigieron el título tapado en el estado vacío de Control de ingreso y las cuatro acciones de Eventos del local. Los grupos de botones juntos en una fila se igualan cuando el ancho disponible permite mantenerlos dentro de su contenedor.

CP-357 verifica la altura de las tarjetas por fila y su ancho fijo. Windows dibuja solo el área cliente para las figuras, con 25 PNG por cada escala. Los resultados de esta revisión quedaron en `TestResults/sistema-visual-final-360.log` y `TestResults/reforma-captura-150.log`.

## Límites del cotejo

La cobertura automatizada de medidas abarca la barra, el lateral, los grupos de acciones identificados, las tarjetas y los formularios de acceso. Siguen fuera de conformidad completa los tamaños nativos de selectores de Reportes y Auditoría, y hay márgenes y acciones heredados que no se normalizaron a todas las medidas de la plantilla de formulario. La comprobación de Enter en Suscripción conserva el caso manual porque abre un modal.


## Jerarquía y densidad

La revisión anterior se guardó en el commit local `3030f10` antes de empezar. La jerarquía usa títulos de 20 pt, encabezados de 10,5 pt con mayúsculas e interletrado de 0,1 em, datos de tarjeta de 15 pt, cuerpo de 10 pt y etiquetas y ayudas de 9 pt. El valor de tarjeta se ajusta cuando haría falta un segundo renglón. Las acciones secundarias y los enlaces subrayados usan colores de texto. Se conserva el indicador de foco pedido en el sistema visual.

Reportes muestra el resultado inicial y usa dos decimales en columnas de importes. Auditoría muestra entradas recientes, con un encabezado y rótulos de filtros. En ambas pantallas se usan el marco BordeControl, la tipografía de cuerpo y filas nominales de 32 px. CP-357 comprueba la altura de los selectores. Las grillas de Mis locales, Mis entradas y Eventos usan el espacio disponible hasta sus acciones. La búsqueda sin resultados centra el bloque de estado vacío.

En Agregar evento, Agregar sector, Registrar local, Cambiar contraseña y Pago, la acción principal toma el ancho y el borde izquierdo del campo. Los botones de cada grupo tienen igual ancho. El inicio de la revisión conservó cambios preexistentes en tres documentos, fuera del commit de la interfaz.

La batería se registra en `TestResults/jerarquia-final-360.log`. Las figuras bajo `TestResults/Capturas-Reforma` se capturaron con el host de DPI y el área cliente de Windows. La altura de títulos y el color se revisaron visualmente en las hojas de contacto de ambas escalas. El porcentaje de área ocupada varía con la cantidad de datos y no se ha certificado un umbral uniforme para todas las ventanas. Algunos márgenes heredados fuera de los formularios intervenidos conservan su disposición anterior.

Las columnas numéricas quedan alineadas a la derecha. La medición de glifos GDI mostró ancho uniforme para los dígitos en Segoe UI de 10 pt y ancho variable para Segoe UI Semibold de 15 pt. Los valores destacados en Semibold conservan esa característica fuera de las columnas.
