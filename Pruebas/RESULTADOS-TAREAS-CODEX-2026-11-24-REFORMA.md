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


## Acabado

La selección dibujada de los ComboBox y las celdas seleccionadas de las grillas usan Primario y texto blanco. El DateTimePicker recibe el tema del calendario que WinForms expone. Windows sigue dibujando la selección del segmento editable de la fecha.

Los tres laterales tienen glifos de 16 px y estados de puntero y presión. Las tarjetas accionables comparten los mismos fondos y el clic sobre la tarjeta invoca su acción. Administración presenta seis métricas en dos filas de tres cuando hay ancho disponible. Entradas vendidas e ingreso bruto salen del mismo reporte y ocupan tarjetas separadas. Los recuentos de revisión están bajo su propio encabezado. El glifo de Cerrar sesión sigue la referencia de Microsoft. [Segoe Fluent Icons](https://learn.microsoft.com/en-us/windows/apps/design/iconography/segoe-fluent-icons-font)

El margen común cambia a 32 px. Mis locales y Eventos del local centran la tabla cuando sus filas caben y usan el espacio disponible cuando necesitan desplazamiento. Se centran los bloques de tarjetas de los inicios. El mínimo de cada ventana toma como referencia su tamaño presentado, acotado al área de trabajo de Windows. CP-357 intenta reducir las ventanas adaptables por debajo del mínimo.

La verificación queda en `TestResults/acabado-final-360.log` y las figuras en `TestResults/Capturas-Reforma`. No se certificó el límite de un tercio de espacio libre en las 25 vistas. Algunas vistas con pocos registros conservan espacio libre y el armado de compra conserva sus márgenes interiores propios.

## Regresiones de la revisión de capturas

Los tres inicios mantienen el encabezado y las tarjetas arriba. Eventos del local muestra el nombre del local junto al título y ubica la grilla debajo. Las tarjetas reservan el alto que pide el texto. El barrido también corrigió recortes en las grillas de sectores, eventos, reportes e historial, en el estado vacío de revisión, en el detalle de solicitudes y en la leyenda de confirmación.

La compra de demostración selecciona un evento vigente con disponibilidad en la base de pruebas. Las capturas muestran Duki en Rosario, dos entradas VIP y un total de $ 138.000,00. La franja del total queda debajo del comprobante y fuera de su área de desplazamiento. CP-13 y CP-119 comprueban que se lea al abrir el resumen. CP-357 también lo comprueba antes de tomar la captura.

Los cinco enlaces usan EnlaceTematico. En los Designer de frmInicio, frmCrearUsuario y FrmCompraCliente se cambió solo el tipo instanciado. Se conservan los nombres, los eventos y el comportamiento del enlace. La captura de compra muestra el borde Primario en el enlace de niveles.

Se revisaron las 25 vistas y se regeneraron las 50 capturas del área cliente en TestResults/Capturas-Reforma, con 96 y 144 DPI informados por Windows. Los dos recorridos de captura de CP-357 finalizaron correctamente. El caso de Enter en Suscripción conserva su verificación manual.

## Lateral, catálogo y resumen de compra

Los tres menús usan el fondo oscuro y los colores de texto, puntero y sección definidos en Tema. El ítem activo usa Primario con texto e icono blancos.

Eventos abre en tarjetas de alto uniforme, en una grilla de tres columnas que se desplaza verticalmente. La semilla tiene dos eventos vigentes y el local compartido usa Av. Pellegrini 1234, Rosario. Cada tarjeta distingue el cupo disponible, bajo o agotado con un rótulo y un color. La vista de tabla sigue accesible desde Ver como tabla.

El resumen de compra usa Segoe UI con encabezados de Entradas, Servicios y Cargos. Los importes tienen dos decimales y se alinean a la derecha. CP-119 verifica que los dígitos tengan ancho uniforme. El Designer de FrmCompraCliente cambia el tipo de lblResumen al control que dibuja las filas. Conserva el nombre y el texto con la línea TOTAL. La franja queda visible y el detalle puede desplazarse. El mapa compacto deja a la vista la grilla completa de los tres sectores de Duki en Rosario.

El foco de los enlaces se indica con un subrayado. Los pasos completados usan TextoSecundario y una marca de verificación.

Pasaron 360 de 360 pruebas. CP-357 verificó que la grilla de sectores y el total entren en la captura inicial. Se regeneraron las 50 capturas del área cliente con 96 y 144 DPI reales. Las imágenes están en TestResults/Capturas-Reforma y los registros en TestResults/diseno-visual-360.log y TestResults/diseno-visual-capturas.log. Enter en Suscripción sigue como verificación manual.

## Cierre de la interfaz del cliente

El inicio usa tarjetas de resumen de 264 px lógicos y rótulos más cortos. Debajo, Eventos disponibles reutiliza las tarjetas del catálogo y abre la ficha del evento seleccionado. Usa solo eventos vigentes con cupo. La captura de la semilla muestra Duki en Rosario, el único con stock en ese escenario.

En compra, Ver servicios alterna Entradas y Servicios y seguro en la misma columna. El total queda a la vista en ambas vistas. El resumen pinta cada fila completa dentro de su área visible, incluida la ayuda cuando entra. CP-119 recorre ambos paneles y comprueba el total. CP-160 abre la ficha desde el inicio y desde el catálogo.

Los tres filtros del catálogo usan la tipografía de cuerpo, 32 px lógicos y el marco del tema. El selector de fecha conserva el calendario nativo. El fondo del catálogo coincide con el de la pantalla. CP-357 controla las medidas de los filtros y se ejecuta al tomar las 50 capturas de las 25 ventanas, a 96 y 144 DPI reales de Windows.

## Migración a WebView2

| Área | WebView2 | WinForms |
|---|---|---|
| Eventos | Tarjetas fluidas con portada, disponibilidad y selección por ID | Filtros, ficha, conmutador, DataGridView y tarjetas de respaldo |
| Reportes y plataforma | Tabla de lectura, barras de entradas por evento, distribución por categoría, línea por fecha de evento e indicadores de plataforma | Filtros y acción Exportar PDF, navegación lateral, grilla e indicadores de respaldo |
| Compra | Comprobante y detalle plegable con total fijo | Sectores, servicios, acciones de compra y lblResumen con su línea TOTAL como fuente |

PanelWeb carga archivos de Visual/Web desde la salida de compilación. Inyecta los valores de Tema.cs como variables CSS y recibe mensajes del documento local. La vista restringe navegación y recursos al host virtual local. Probamos el respaldo con una ubicación de runtime inválida en el proceso de capturas. Los formularios conservan los controles WinForms y muestran un aviso si la vista web no está disponible.

La línea usa ventas agrupadas por fecha del evento, junto con el listado de fechas que ya expone GestorReportes. Omite coincidencias ambiguas por nombre. Los indicadores del panel excluyen la fila de total que ya viene en cada resultado, para contar cada valor una vez. Las consultas, las reglas y el exportador PDF se conservan.

No se reescribió ninguna prueba funcional. CP-16, CP-17 y CP-18 conservan sus aserciones. Se amplió la captura de CP-357 para combinar el área cliente WinForms y la imagen de WebView2. Se comprobó aparte el clic HTML a la ficha, la alternancia a tabla, la actualización de filtros, los tres selectores de gráfico y la coincidencia del total web con lblResumen. Se revisaron las capturas a 96 y 144 DPI. Enter en Suscripción conserva su verificación manual.
