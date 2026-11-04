# Correcciones de JC — 03/11/2026

> Continuación y estado posterior: [Verificación de JC — 04/11/2026](VERIFICACION-JC-2026-11-04.md).
> Incluye cierre de diálogos, dos fallos adicionales corregidos y 31/31 pruebas correctas.
>
> Commits del 04/11/2026: `4ee99d3` (títulos), `35624de` (compra), `861b02b`
> (confirmaciones y validaciones), `95bd727` (pruebas) y `ac2b0af` (herramientas).

Continuación de [la corrida inicial](RESULTADOS-PRUEBAS-JC.md), sobre `9e9bbbe` más los
cambios locales de esta entrega. No se hicieron commits ni publicaciones. La corrida
inicial se conserva como evidencia histórica: sus 65 filas **no representan una nueva
ejecución completa** después de estas correcciones.

## Cambios implementados

| Hallazgo | Corrección | Verificación de esta entrega |
|---|---|---|
| F-01 | Resumen más ancho, contenido con altura automática y desplazamiento de respaldo para contenido excepcional | Prueba de interfaz con 6 entradas en 3 sectores, 4 servicios y seguro: las líneas caben y TOTAL y leyenda están dentro del panel |
| F-02 | Encabezado separado de las dos columnas, título con ancho y alto acotados y fuente de 18 pt; nivel y enlace debajo | Título de 100 caracteres completo, sin invadir el resumen; nivel y enlace no se superponen |
| F-03 | Servicios en una columna con ancho suficiente para el concepto y precio | Comprobación del ancho preferido de las casillas con precios y con «no disponible» en el evento agotado |
| O-01 | Se eliminaron las cuatro «A» precargadas en el registro | Prueba de apertura: nombre, apellido, correo y contraseñas vacíos |
| O-02 | `Name` de registro corregido a `frmCrearUsuario` | Comprobación automática |
| O-03 | «Hola Pedro» reemplazado por «Seleccionar evento» | Revisión del manejador; falta repetir el diálogo en el recorrido interactivo |
| O-04 | Texto guía configurado para mostrarse también con foco | Revisión de `EM_SETCUEBANNER`; falta comprobación visual con foco |
| O-05 | Categoría y Ubicación con tildes; columnas distribuidas en el ancho disponible y filas con ajuste de texto | Comprobación automática de encabezados y modo Fill |
| O-06 | Listado de eventos de solo lectura, sin filas nuevas ni eliminación | Prueba de filtro sin resultados: una fila de mensaje, sin fila editable |
| O-07 | «No» por defecto al descartar el paquete | Revisión del manejador; falta repetir Sí/No en pantalla |
| O-08 | Mensaje «número entero de 0 en adelante», con rechazo explícito de negativos | Revisión de validación; falta repetir ingreso inválido en pantalla |
| O-09 | Botón de devolución gris cuando no hay selección, violeta cuando corresponde habilitarlo | Prueba de historial sin compras: deshabilitado y gris; falta repetir habilitación al tildar una entrada |
| O-10 | Confirmación antes de eliminar, con nombre del evento y «No» por defecto; también «No» al ofrecer suspensión | Revisión del manejador; falta repetir ambos caminos en pantalla |
| O-11 | Títulos legibles con marca EventAura en los 17 formularios cuyos títulos eran internos | Compilación y apertura de los formularios usados en las pruebas; falta recorrer los demás |

Además se corrigió un error de registro: el formulario comprobaba el correo dos veces,
no comprobaba el apellido y daba mensajes de campo equivocado. Ahora exige nombre,
apellido y correo no vacíos ni compuestos solo de espacios. La validación de dominio de
correo y contraseña sigue en el gestor.

Las tareas `Compilar` y `Probar` de VS Code ya no dependen de una ruta fija de Community:
`tools/Proyecto.ps1` detecta Visual Studio o Build Tools con `vswhere` y transmite el
código de salida de compilación/pruebas. Se comprobó su ejecución en esta PC.

**Designer declarado:** en compra se cambió `ClientSize` de 760 × 595 a 1100 × 680
unidades de diseño a 120 DPI (880 × 544 de área cliente a 96 DPI); se reubicaron y
dimensionaron los controles para corregir F-01 a F-03, se redujo el título de 22 a 18 pt
y se habilitó desplazamiento del resumen. No se unificó el DPI de los 19 formularios.
En registro se corrigió `Name`; en los otros Designer solo se cambiaron títulos visibles.
Estos cambios están autorizados por el pedido de corregir los hallazgos de JC.

## Resultado comprobado

- Compilación Debug con restauración: sin errores ni advertencias en la salida.
- **16/16 pruebas correctas**: las 12 anteriores y cuatro nuevas de interfaz.
- Se conserva el cálculo de referencia de **$99.625**.
- La prueba de altura declarada de los 19 formularios sigue pasando.
- Imagen generada y revisada: `TestResults/JC/compra-paquete-maximo.png`.
- La verificación visual/geométrica se ejecutó a **96 DPI, escala 100 %**. No certifica
  todavía funcionamiento real a 125 % o 150 %, ni todos los tamaños de notebook.
- No se reejecutaron los 65 flujos de la corrida inicial. Las confirmaciones nuevas y la
  interacción por teclado indicada arriba quedan explícitamente pendientes.

Repetir desde VS Code con la tarea «Probar», o:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar
```

Las pruebas recrean exclusivamente `BddEventAura_Pruebas`; necesitan permisos de SQL
Server para hacerlo. La guarda del sufijo `_Pruebas` permanece activa.

## Base de datos

La instalación `SQLEXPRESS` funciona. La base de desarrollo tiene 16 tablas y se comparó
con `BddEventAura_Pruebas`, recién creada desde `01-esquema.sql` y `02-semilla.sql`:

| Revisión | Resultado |
|---|---|
| Columnas, tipos, tamaños, precisión, escala, nulabilidad, identidad y valores predeterminados | 0 diferencias en ambos sentidos |
| Índices, columnas, unicidad, claves primarias y filtros | 0 diferencias en ambos sentidos |
| Claves foráneas y acciones referenciales, habilitación y confianza | 0 diferencias en ambos sentidos |
| `DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS` | Sin violaciones informadas |
| Locales sin propietario / entradas sin usuario / correos duplicados / sectores de otro evento | 0 / 0 / 0 / 0 |

Evidencia local: `TestResults/JC/auditoria-base.txt`. Consulta repetible:
`tools/AuditarBaseJc.sql` (solo lectura de las bases; crea únicamente tablas temporales).
La comparación exige que ambas bases existan en la misma instancia.

Esto confirma que **la estructura local coincide con los scripts actuales**. No demuestra
una migración de registros de la base original de años anteriores: no se contó con un
backup original ni un inventario de registros para compararlos. No se borró ni regeneró
la base de desarrollo y conserva los datos de la corrida de Claude.

Pendientes detectados para la etapa de datos/seguridad:

- `01-esquema.sql` no es una migración repetible: la parte B agrega columnas y crea tablas
  sin comprobar si existen. No volver a ejecutarlo sobre esta base poblada.
- `02-semilla.sql` también supone una base sin datos; no usarlo para «actualizar» la base.
- Algunas relaciones heredadas carecen de FK (por ejemplo, `Locales.PropietarioID` y
  `Entradas.ID_Usuario`). Hoy no se encontraron huérfanos; revisar restricciones mediante
  una migración versionada y con respaldo, antes de incorporar datos reales.
- La semilla contiene contraseñas de demostración en texto plano y el login las migra a
  PBKDF2. Cambiar ese mecanismo antes de una entrega con datos reales.
- `Visual/App.config` y `Pruebas/App.config` ya estaban modificados para esta PC. Se
  conservaron; sus cadenas locales no deben incluirse por accidente en un commit compartido.

## PDF y QR desde esta PC

Se renderizaron los seis PDF existentes y se decodificaron sus QR localmente con PyMuPDF
y ZXing-C++, sin subir los tickets a servicios externos. Resultado: seis QR distintos,
todos coincidentes con el código impreso en su PDF y con `Entradas.CodigoQR` en SQL Server.
Incluye las dos entradas de EA-000011 (7.10 y 10.5). Evidencia local:
`TestResults/JC/qr-tickets.json` y `codigos-base.txt`.

La validación de contenido del QR queda comprobada. **El escaneo con una cámara de celular
sigue pendiente**: un decodificador local no verifica enfoque, iluminación o lectores
concretos. El QR contiene texto `EA-…`; no abre un sitio ni existe un flujo móvil de
validación de acceso implementado por este cambio. No hace falta cambiar de lenguaje para
comprobar el contenido actual.

## Siguiente cierre de esta etapa

1. Repetir los diálogos y recorridos indicados en la tabla, incluido el evento agotado.
2. Probar compra a 125 % y 150 % y en una notebook, con título largo y paquete máximo.
3. Escanear al menos dos QR desde un celular.
4. Completar la parte del compañero en `PRUEBAS-PENDIENTES.md`; no se marca como aprobada.
5. Contrastar las consignas oficiales/anexos con la implementación: el contexto del repo
   declara RF-09 sin construir, permisos Composite e idioma Observer futuros y pendientes
   documentales/usabilidad. Verificar qué exige la entrega antes de decidir la tecnología.

La etapa de arquitectura, rediseño integral y auditoría de seguridad permanece separada
de este cierre de errores. Todavía no se declara el proyecto listo para entregar.
