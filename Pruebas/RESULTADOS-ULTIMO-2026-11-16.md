> Avance histórico de las 15:50. La autorización fue otorgada y M-22 se completó después. El estado definitivo está en RESULTADOS-CIERRE-DEFINITIVO-2026-11-16.md.

# Últimos ajustes — avance y autorización pendiente — 16/11/2026

## Estado inicial completo, antes de preparar archivos

```text
On branch main
Your branch is up to date with 'origin/main'.

nothing to commit, working tree clean
```

HEAD inicial: bc19481, documentación de la medición global sobre 65225e7. Ambos App.config tenían y conservan S de skip-worktree. Ningún App.config preparado. No se modificaron docs/ ni tools/.

## M-22: preparado, pendiente de autorización de Entidade/ResultadoIngreso.cs

El código no mutado rechaza con ReglaNegocioException RN-34. La mutación que elimina esa defensa provoca RN-42 para un cliente y NullReferenceException sin sesión, como describe Claude. El encargo actual pide un contrato de resultado sin excepción: motivo NoAutorizado, código RN-34 y el mensaje exacto «Esta operación es solo para el Administrador».

Se preparó el cambio mínimo: agregar NoAutorizado al final del enum (conservando los valores anteriores) y la propiedad Regla al DTO existente; capturar RN-34 al registrar y devolver ese resultado; evitar la consulta de contador desde la UI después de un rechazo. No se proponen otras modificaciones de entidades ni cambia RN-42 de los administradores de otro propietario.

AGENTS.md exige autorización explícita fuera de la zona: «Lo que no figura en la tabla (`Entidade/`…) no se toca sin que JC lo autorice de forma explícita en esa sesión». Se consultó mediante la pregunta pendiente. No se aplicó el cambio en Entidade ni la parte dependiente en Repo/Visual.

[Diff completo revisable](EVIDENCIA-M22-2026-11-16/propuesta.patch). [Prueba CP-146 preparada](EVIDENCIA-M22-2026-11-16/PruebasRechazoIngreso.cs.preparado).

CP-146 se escribió y ejecutó contra el código actual: las dos filas (cliente y sin sesión) fallaron porque se lanza una excepción en vez de devolver el resultado. [Registro del rojo previo](EVIDENCIA-M22-2026-11-16/antes-rojo.log). La fuente quedó archivada con extensión .preparado; no forma parte de la batería activa ni se la cuenta como aprobada. Falta aplicar el cambio autorizado, verla verde y quitar la defensa para demostrar su rojo/verde posterior. El runner local Pruebas/VerificarUltimo16.py tiene esa comprobación preparada como modo m22.

## Índice único de seguros: implementado y verificado

BaseDatos/07-indice-unico-seguros.sql: índice único UX_Seguros_ID_Entrada sobre Seguros(ID_Entrada). Antes comprueba duplicados, muestra ID_Entrada y cantidad y se detiene con error 50007 sin borrar ni fusionar datos. También se agregó al esquema para instalaciones nuevas; el fixture ejecuta 07 en _Pruebas.

CP-147 tiene dos filas: una segunda póliza sobre la misma entrada se rechaza con SqlException 2601 y el nombre exacto UX_Seguros_ID_Entrada; otra entrada admite su propia póliza. Ambas hacen rollback y comprueban persistencia y sesión intactas. Se quitó el índice en las dos rutas de creación de _Pruebas para verificar que la negativa falla, y se restauraron los bytes originales: ambas volvieron a verde. No se modificó la nulabilidad de compra ni sector en Entradas.

Máximo tendrá que ejecutar 07 sobre su base seleccionada en SSMS cuando se publique esta tanda. No se aplicó el script a la base de desarrollo ni se supuso que estuviera libre de duplicados; la guarda del script los informa si aparecen.

## U-09: resuelto con el recorrido existente

Después de cerrar la confirmación, FrmCompraCliente llama al método existente VolverAlListado. Se quitó el reinicio del paquete que dejaba el armado vacío. No se agregó pantalla, control, estilo ni otra decisión visual.

CP-148 recorre la UI real: arma, paga con la pasarela simulada, cierra la confirmación y exige listado visible, armado oculto e identidad conservada. Se probó contra el flujo anterior de 65225e7: falló. Con el cambio restaurado volvió a verde. Para los PDF emitidos por la UI, el helper protege y restaura cualquier archivo previo con el nombre exacto del ticket de esa compra; si el archivo es nuevo, solo retira ese archivo de prueba al finalizar. No borra directorios ni toca otros tickets.

Capturas completas por DrawToBitmap:

| Estado | 100 % | 150 % |
|---|---|---|
| Confirmación antes de cerrarla | [captura](U09-ULTIMO-2026-11-16/100/u09-confirmacion.png) | [captura](U09-ULTIMO-2026-11-16/150/u09-confirmacion.png) |
| Listado al cerrar la confirmación | [captura](U09-ULTIMO-2026-11-16/100/u09-retorno-listado.png) | [captura](U09-ULTIMO-2026-11-16/150/u09-retorno-listado.png) |

Barrera tras U-09: 49/49, veinte pantallas; color renderizado de 31 primarios, cero fallos. No hubo rojo de serialización ni estilo.

## Figuras del guion y tamaño impreso

Se regeneraron las tres pantallas reales con DrawToBitmap, a 96 y 144 DPI, sin textos ingleses de contraseña. No se rediseñaron. Están en Guion-Interfaz-ULTIMO-2026-11-16/100/ y /150/.

| Página | Pantalla | 100 % | 150 % | Menor fuente del contenido a 16 cm, 100/150 % |
|---|---|---|---|---|
| 47 | frmInicio | [inicio](Guion-Interfaz-ULTIMO-2026-11-16/100/p47-inicio.png) | [inicio](Guion-Interfaz-ULTIMO-2026-11-16/150/p47-inicio.png) | 13,41 / 13,47 pt |
| 48 | frmCrearUsuario | [registro](Guion-Interfaz-ULTIMO-2026-11-16/100/p48-registro.png) | [registro](Guion-Interfaz-ULTIMO-2026-11-16/150/p48-registro.png) | 12,20 / 12,26 pt |
| 49 | frmMenuCliente | [menú](Guion-Interfaz-ULTIMO-2026-11-16/100/p49-menu-cliente.png) | [menú](Guion-Interfaz-ULTIMO-2026-11-16/150/p49-menu-cliente.png) | 14,89 / 14,97 pt |

Cálculo a partir de la fuente real del control, DPI del host y ancho total del PNG: puntos de la fuente × DPI / 72 × (16 / 2,54 × 72) / ancho en píxeles. Se toma la menor fuente de los controles visibles con texto del contenido (no el marco de Windows). Las tres quedan por encima de 7 pt; no se entregó ninguna figura del guion por debajo del umbral. tipografia.tsv guarda las mediciones por escala. Las capturas U-09 son evidencia técnica del recorrido, no una nueva figura propuesta para el Word.

## Limpieza del registro

- CP-125: se agregó el nombre visible del TestMethod, manteniendo sus ocho DataRow. La corrida mostró las ocho filas con CP-125 y sus descripciones. No se agregaron variantes.
- Se corrigieron los dos signos de interrogación que reemplazaban las tildes en «después» e «inscripción», guardando UTF-8 con BOM.
- Comentarios RF corregidos según docs/04-casos-prueba.md: CP-127 → RF-06; CP-129 → RF-06; CP-133 → RF-07; CP-138 y CP-139 → RF-08; CP-140 → RF-05; CP-143 y CP-144 → RF-11. No se cambiaron sus aserciones.

## Números: provisionales, sin congelamiento nuevo

Batería de los cambios terminados: **274/274** (271 previas + dos filas CP-147 + una CP-148). TestResults/Ultimo16/parcial.log y validacion-parcial/parcial.trx. No incluye la CP-146 pendiente de implementación. Si se autoriza y pasan sus dos filas, el total esperado es 276, todavía no verificado.

La cifra global histórica del documento permanece **73/77 = 94,8 %, sobre 65225e7**. Las comprobaciones rojo/verde locales de índice y U-09 no se suman a ella. No se repitió la medición global ni se reestimó el porcentaje. La corrección de M-22 se documentará como posterior a esa medición una vez completada.

No hay SHA nuevo de congelamiento: no se publica una tanda incompleta como definitiva. Queda pendiente la respuesta de JC para modificar Entidade/ResultadoIngreso.cs, completar M-22 y después publicar con las comprobaciones pedidas. Los demás puntos están preparados y verificados.

## Estado actual completo

```text
On branch main
Your branch is up to date with 'origin/main'.

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
	modified:   BaseDatos/01-esquema.sql
	modified:   Pruebas/BaseDePrueba.cs
	modified:   Pruebas/PruebasGuardasMutacion.cs
	modified:   Pruebas/PruebasPrioridadTemporal.cs
	modified:   Visual/Cliente/FrmCompraCliente.cs

Untracked files:
  (use "git add <file>..." to include in what will be committed)
	BaseDatos/07-indice-unico-seguros.sql
	Pruebas/CapturarUltimo16.ps1
	Pruebas/CapturasUltimasUI.cs
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/p47-inicio.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/p48-registro.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/p49-menu-cliente.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/tipografia.tsv
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/p47-inicio.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/p48-registro.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/p49-menu-cliente.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/tipografia.tsv
	Pruebas/EVIDENCIA-M22-2026-11-16/PruebasRechazoIngreso.cs.preparado
	Pruebas/EVIDENCIA-M22-2026-11-16/antes-rojo.log
	Pruebas/EVIDENCIA-M22-2026-11-16/propuesta.patch
	Pruebas/PruebasSeguroUnico.cs
	Pruebas/U09-ULTIMO-2026-11-16/100/u09-confirmacion.png
	Pruebas/U09-ULTIMO-2026-11-16/100/u09-retorno-listado.png
	Pruebas/U09-ULTIMO-2026-11-16/100/u09.txt
	Pruebas/U09-ULTIMO-2026-11-16/150/u09-confirmacion.png
	Pruebas/U09-ULTIMO-2026-11-16/150/u09-retorno-listado.png
	Pruebas/U09-ULTIMO-2026-11-16/150/u09.txt
	Pruebas/VerificarUltimo16.py

no changes added to commit (use "git add" and/or "git commit -a")
```

## Vueltas y portones

Se ejecutaron la corrida dirigida de diez filas (CP-125/147), CP-148 sola, los pares rojo/verde de índice y U-09, la barrera de 49, las capturas a ambas escalas, una batería completa de 274 y el rojo previo de las dos filas CP-146. La compilación activa quedó verde al archivar la CP-146 pendiente. No se ejecuta más contra la base ni se abre más UI hasta resolver el único portón restante.

Portón: autorización explícita para ese único archivo Entidade/ResultadoIngreso.cs; luego terminar M-22 y publicar el SHA definitivo. No hay decisión visual pendiente ni se detectó otro defecto que se haya arreglado fuera del encargo.

Tiempo de reloj de este avance: 15:30:34 a 15:50:10 ART, 0.327 h. No son horas-persona.
