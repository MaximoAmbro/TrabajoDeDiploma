# Retoma de quinta tanda y pruebas de rechazos de negocio

Fecha: 05/11/2026. Responsable: Codex. Destinatarios: JC y Claude Code.
Base: `81c5f62`. Sin commits ni push.

## Estado al retomar

Se revisaron `git status --short`, `git diff --stat`, el estado de agentes y el encargo completo de T-27 a T-29. Los cambios pendientes correspondían a la sesión interrumpida que JC pidió conservar. No se reimplementaron las tareas. El turno explícito de JC prevalece sobre el estado anterior del archivo de coordinación.

| Tarea | Estado real comprobado |
|---|---|
| T-27 | Aplicada. `ManejadorErrores` instalado al principio de `Program.Main`, incluido en el proyecto; mensajes de base de datos, negocio y error general conforme al encargo. `frmPago` captura el evento eliminado, detiene el temporizador y devuelve Abort sin cobrar. CP-51 tiene tres filas; CP-52 y CP-53 pasan. |
| T-28 | Aplicada. `FormularioEventAura` es una clase concreta. CP-12 excluye únicamente la base común, sigue comprobando las 19 pantallas y pasa sin Ignore. La apertura real del diseñador sigue pendiente de JC o Máximo. |
| T-29 | Aplicada. Encabezados con WrapMode=True y alto AutoSize en `CargarSectores`, sin cambiar texto ni ancho de columnas. CP-54, CP-13 y CP-25 pasan. La figura del instructivo debe regenerarla Claude Code. |

La primera batería de esta retoma dio **66/66 correctas**, sin pruebas omitidas, antes de iniciar la parte opcional. No fue necesario completar ninguna de las tres tareas.

## Parte opcional: siete pruebas agregadas

Archivo nuevo: `Pruebas/PruebasRechazosNegocio.cs`. Usa los datos aislados y la limpieza de Fake, cierra la sesión entre pruebas y no modifica la semilla ni parámetros globales.

| Caso | Regla | Operación rechazada y comprobación | Batería completa posterior |
|---|---|---|---|
| CP-55 | RN-05 | Perfil inexistente en el método genérico Registrar; devuelve la regla y no crea la cuenta. | 67/67 |
| CP-56 | RN-06 | Eliminar el último sector con capacidad de un evento publicado; conserva sector y estado del evento. | 68/68 |
| CP-57 | RN-14 | Agregar al paquete un servicio de otro evento; conserva las entradas y no agrega servicios. | 69/69 |
| CP-58 | RN-25 | Contratar otro plan con una suscripción activa; conserva el ID y el plan originales. | 70/70 |
| CP-59 | RN-29 | Devolver sin seguro y devolver con seguro fuera del plazo; ambas compras siguen confirmadas con reembolso cero. | 71/71 |
| CP-60 | RN-38 | Agregar un sector que, sumado al existente, supera la capacidad del local; no se inserta y conserva el sector original. | 72/72 |
| CP-61 | RN-39 | Iniciar un paquete durante preventa sin suscripción activa; devuelve la regla y conserva el cupo. | 73/73 |

**Agregadas: 7. Salteadas: 0. Ignore nuevos: 0.**

RN-05 se pudo probar directamente con el método genérico que ya existe. El motivo histórico de EXCEPCIONES.md (el alta pública ya no permite elegir perfil) no impide probar ese rechazo de negocio. No se implementó T04 ni se volvió a agregar el selector a la interfaz.

Estas pruebas provocan los siete errores documentados; no representan cobertura de todas las ramas de cada regla. Por ejemplo, CP-56 cubre eliminar el último sector, no publicar sin sectores; CP-60 cubre agregar, no modificar sectores. CP-59 comprueba dos motivos de rechazo dentro de una ejecución.

Se ejecutó `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar` después de cada prueba nueva. Todas las corridas terminaron con código 0. Resultado final: **73 ejecuciones correctas, cero fallidas y cero omitidas**, tiempo de la última corrida 6,1426 segundos. La compilación no informó advertencias nuevas.

## Archivos pendientes para revisión de JC

Ya modificados por la sesión interrumpida y verificados en esta retoma:

- `Visual/ManejadorErrores.cs` (nuevo).
- `Visual/Visual.csproj`: entrada Compile del manejador.
- `Visual/Program.cs`.
- `Visual/Cliente/frmPago.cs`.
- `Visual/FormularioEventAura.cs`.
- `Visual/Cliente/FrmCompraCliente.cs`.
- `Pruebas/PruebasFormularios.cs`.
- `Pruebas/PruebasInterfaz.cs`.

Agregados en esta retoma:

- `Pruebas/PruebasRechazosNegocio.cs`.
- Este reporte.

La parte opcional solo agregó archivos en Pruebas/. No se modificaron Designer, Repo/, Datos/, BaseDatos/, docs/ ni tools/. Los dos App.config siguen con skip-worktree (`S`) y no se tocaron. `git diff --check` no encontró errores de espacios; Git avisa de normalización futura LF/CRLF en FormularioEventAura.cs.

## Defectos encontrados y pendientes fuera de esta zona

No se encontraron defectos nuevos con estas pruebas. No se corrigió código de aplicación durante la parte opcional.

Pendientes de validación que no deben darse por cerrados por el resultado automático:

1. **JC o Máximo, CX-03:** abrir los diseñadores de `FrmCompraCliente` y `frmInicio` en Visual Studio; deben dibujarse sin errores de clase base. La clase concreta y CP-12 son correctos, pero no sustituyen esta comprobación.
2. **Claude Code, T-27 / CX-04:** ajustar `tools/Excepciones/ErrorNoControlado.cs` para instalar el manejador y ejecutar la comprobación de base caída. Debe aparecer el mensaje en castellano sin el cuadro de excepción de .NET. No se detuvo SQL Server en esta retoma.
3. **Claude Code, T-29:** regenerar las figuras del instructivo y comprobar visualmente el encabezado completo a escala 100 %.

## Próximo turno de Claude Code

- Mover este reporte a docs/ y actualizar ESTADO-AGENTES.md.
- Hacer las dos verificaciones anteriores de su zona y mantener CX-03 como pendiente de JC/Máximo hasta recibir evidencia.
- Registrar CP-51 a CP-61 y sus requerimientos/reglas en 04-casos-prueba.md; reflejar 73 ejecuciones en COMO-SE-PROBO.md y documentos que correspondan.
- Actualizar EXCEPCIONES.md: T-27 implementada y siete nuevos rechazos provocados automáticamente. Mantener separada la comprobación del manejador global con SQL caído hasta ejecutarla.
- Actualizar la lista de correcciones y el estado del instructivo según los resultados reales.

Codex termina aquí el alcance solicitado. JC debe revisar git status antes de autorizar cualquier commit. No se hizo commit ni push.
