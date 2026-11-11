# Auditoría de asignaciones directas en Visual para preparar 6B

Codex, 10/11/2026. Revisión de solo lectura del código fuente, con inventario local completo en `TestResults/Tandas7/auditoria-tema.json`. No se modificó ninguna de las pantallas inventariadas.

## Recuento y criterio

- **18 de las 19 pantallas** tienen colores, fuentes o dimensiones fijas de estilo fuera de Tema en sus Designer: las 18 pendientes de 6B.
- **6 de esas pantallas** también tienen asignaciones directas en su código de ejecución. No son seis adicionales a las 18.
- En **4** hay sobrescritura clara de estilos de grilla después de `Tema.Normalizar`: Mis locales, Historial, Listas de espera y Reportes.
- Las otras dos son Eventos del cliente (pesos de columnas) y Pago (colores del temporizador). No toda asignación directa representa por sí misma un defecto visual.
- El piloto `FrmCompraCliente` toma su estilo de Tema. Si se cuentan también los metadatos estructurales serializados del Designer, las **19** pantallas tienen literales: el piloto conserva `AutoScaleDimensions` a 96F y `ClientSize` 880 × 544 en líneas 302 y 305, como requieren el encargo y CP-12. No se cuentan esos dos metadatos como una sobrescritura del tema.

Se buscaron colores nombrados/FromArgb, construcciones de Font y dimensiones fijas de controles/columnas. Se excluyeron bin, obj y recursos generados que no son pantallas. El inventario contiene cada coincidencia con su línea; las tablas siguientes dan los lugares relevantes para revisar. Los colores en métodos de dibujo PDF se separan de los formularios.

## Asignaciones en ejecución: las seis pantallas

| Pantalla | Archivo y líneas | Qué asigna | Cuándo y efecto |
|---|---|---|---|
| Mis locales | `Visual/Propietario/frmMisLocales.cs:40` | Fondo alterno fijo 245/245/245 | FormatearGrilla se llama en Load. Sobrescribe Tema.FilaAlterna. Reproducido por CP-71; hallazgo conocido de T-43. |
| Historial del cliente | `Visual/Cliente/frmHistorialCliente.cs:22`; `:245`, `:246`, `:247`, `:248` | Violeta local; fondo blanco, texto 64/64/64, selección violeta, alternas 245/245/245 | FormatearGrilla se aplica a las dos grillas desde Load, después del tema. La selección pierde PrimarioSuave/Texto y las alternas pierden FilaAlterna. |
| Historial del cliente, estados | `Visual/Cliente/frmHistorialCliente.cs:153`; `:186`, `:187` | Filas grises; fondo y texto de Devolver con colores locales | Se aplica al cargar detalle/actualizar selección. Es estilo de estado con significado; debe conservarse ese significado al pasarlo a tokens. |
| Historial del cliente, columnas | `Visual/Cliente/frmHistorialCliente.cs:43`, `:44`, `:45`, `:46`, `:47`, `:55`, `:61` | FillWeight 80/150/85/95/65/60/180 | Son proporciones del layout de las columnas. Están fuera de Tema; no pisan un token porque Tema no define FillWeight. |
| Listas de espera | `Visual/Propietario/frmListasEspera.cs:18`; `:48`, `:49`, `:50`, `:51` | Violeta local, fondo, texto, selección y alternas fijos | Asignados en Load, después del tema. Mismo patrón que Historial. |
| Listas de espera, columnas | `Visual/Propietario/frmListasEspera.cs:57`, `:58` | FillWeight 50/60 | Proporciones de columnas, no un fallo demostrado de geometría. |
| Reportes | `Visual/Propietario/frmReportes.cs:61`, `:62`, `:63`, `:65` | Fondo blanco, texto 64/64/64, selección violeta y encabezado 231/231/231 | Asignados en Load, después del tema. Pierde selección suave y EncabezadoGrilla. |
| Reportes, totales | `Visual/Propietario/frmReportes.cs:280`, `:281` | Nueva Font en negrita y fondo 236/231/246 | Se aplica al completar el enlace de datos. Es énfasis semántico del TOTAL; no conviene borrarlo al reaplicar el tema. |
| Eventos del cliente | `Visual/Cliente/frmEventosCliente.cs:104`, `:105`, `:106` | FillWeight 160/140/140 | Asignado al cargar resultados. Es layout propio de columnas, no una sobrescritura de colores del tema. |
| Pago | `Visual/Cliente/frmPago.cs:57`, `:58`, `:59` | ForeColor de temporizador: Firebrick o RGB violeta fijo | El temporizador vuelve a asignarlo durante la ejecución. Coincide hoy con Error/Primario, pero deja de seguirlos si cambian los tokens. El rojo de vencimiento tiene significado y debe conservarse. |

`FormularioEventAura.OnLoad`, líneas 20–24, llama a Tema.Normalizar **antes** de base.OnLoad. El Load de cada pantalla corre después: ese orden explica por qué ganan las asignaciones locales. El orden es deliberado para permitir estados y columnas específicos, no conviene invertirlo indiscriminadamente.

## Designer: las 18 pantallas con valores heredados

Cada fila da ejemplos verificables de color/fuente y medida fija. No se afirma que todos los valores de esa pantalla sean incorrectos; son lugares de migración para 6B.

| Pantalla | Archivo | Ejemplo color/fuente | Ejemplo medida |
|---|---|---:|---:|
| Confirmación | `Visual/Cliente/frmConfirmacion.Designer.cs` | 44, 45 | 46 |
| Eventos del cliente | `Visual/Cliente/frmEventosCliente.Designer.cs` | 45, 47 | 48 |
| Historial | `Visual/Cliente/frmHistorialCliente.Designer.cs` | 56, 69 | 45 |
| Menú del cliente | `Visual/Cliente/frmMenuCliente.Designer.cs` | 41, 43 | 44 |
| Pago | `Visual/Cliente/frmPago.Designer.cs` | 53, 54 | 55 |
| Suscripción | `Visual/Cliente/frmSuscripcion.Designer.cs` | 44, 45 | 46 |
| Administrar evento | `Visual/Propietario/frmAdministrarEvento.Designer.cs` | 46, 50 | 52 |
| Agregar evento | `Visual/Propietario/frmAgregarEvento.Designer.cs` | 47, 48 | 49 |
| Agregar sector | `Visual/Propietario/frmAgregarSector.Designer.cs` | 45, 46 | 47 |
| Eventos del local | `Visual/Propietario/frmEventosLocal.Designer.cs` | 43, 47 | 49 |
| Listas de espera | `Visual/Propietario/frmListasEspera.Designer.cs` | 43, 44 | 45 |
| Menú del propietario | `Visual/Propietario/frmMenuPropietario.Designer.cs` | 43, 44 | 45 |
| Mis locales | `Visual/Propietario/frmMisLocales.Designer.cs` | 56, 57 | 46 |
| Registrar local | `Visual/Propietario/frmRegistrarLocal.Designer.cs` | 47, 48 | 49 |
| Reportes | `Visual/Propietario/frmReportes.Designer.cs` | 53, 54 | 55 |
| Cambiar contraseña | `Visual/_Inicio/frmCambiarContraseña.Designer.cs` | 45, 47 | 48 |
| Crear usuario | `Visual/_Inicio/frmCrearUsuario.Designer.cs` | 63, 65 | 52 |
| Inicio | `Visual/_Inicio/frmInicio.Designer.cs` | 45, 47 | 48 |

Una asignación del Designer se ejecuta antes del Load y puede ser normalizada. No equivale automáticamente a una asignación que pisa el tema después. Además, Normalizar es una transición limitada: cambia determinadas familias y grises, conserva tamaños y no reemplaza cualquier color arbitrario. Por eso 6B debe revisar ambos sitios, no solo el Designer.

## Patrón y recomendación para el encargo de 6B

El patrón dominante es **formateo de grillas en Load**: cinco de las seis pantallas con valores en ejecución tienen grilla, cuatro escriben colores locales. Abarca cliente y propietario; no es exclusivo de administración. Pago es la excepción, por un color de estado que se actualiza con el reloj.

Para 6B conviene revisar el Designer, los métodos Formatear/Preparar/Cargar y los eventos de selección/DataBindingComplete/temporizador. Reemplazar las paletas locales por tokens conservando estados: reembolso, deshabilitado, último minuto y fila TOTAL. No agregar simplemente una pasada global de tema después de cada evento, porque borraría esos significados. Los pesos de columnas requieren una decisión de layout, no una sustitución automática de colores.

## Elemento adicional que no es una pantalla

`Visual/ExportadorPDF.cs:29` a `:33` define cinco DeviceRgb propios; `:35` y `:36`, medidas; `:90`, `:92`, `:95`, `:105`, `:107` y `:122`, tamaños de fuente PDF. No entra en el recuento de pantallas ni recibe Normalizar, pero es otro lugar donde el aspecto no sigue Tema. Se informa para que Claude determine su alcance; no se modificó el exportador ni se ejecutó T-46.

Ningún Designer ni formulario fue modificado durante esta auditoría. No se editó docs/SISTEMA-DISENO.md ni ningún otro archivo de docs/.
