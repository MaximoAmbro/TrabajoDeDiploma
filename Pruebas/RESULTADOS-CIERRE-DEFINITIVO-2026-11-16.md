# Cierre definitivo autorizado — 16/11/2026

Este reporte cierra todos los puntos y sustituye el estado pendiente del avance RESULTADOS-ULTIMO-2026-11-16.md. No queda decisión técnica ni modificación pendiente del encargo. No se agregó trabajo ajeno a la lista.

## Cifras definitivas

| Dato | Resultado |
|---|---|
| Casos identificados | **148**, CP-01 a CP-148 |
| Métodos de prueba | **145** |
| Ejecuciones de la batería | **276** |
| Correctas / fallidas | **276 / 0** |
| Reglas con caso / total | **39/41 = 95,1 %**, sin cambiar la cobertura declarada |
| Medición global de mutación para el documento | **73/77 = 94,8 %, sobre 65225e7** |
| Barrera de interfaz | **49/49** |
| Color renderizado | **31 primarios en veinte pantallas, cero fallos** |

La medición global NO se repitió ni se sustituyó por otra cifra. M-22 se corrigió después de esa medición. La verificación local de las correcciones se informa por separado, sin sumarla al 73/77. Fuente verificable de la batería: EVIDENCIA-CIERRE-DEFINITIVO-2026-11-16/bateria-final.trx y bateria-final.log; duración 56,3922 segundos. La reflexión y los IDs de atributos/nombres confirman 145 métodos y CP-01…CP-148.

Diferencia contra el congelamiento de 65225e7: 271 + dos filas CP-146 + dos filas CP-147 + una CP-148 = 276. La limpieza de las ocho variantes CP-125 no agregó filas. No se agregará nada posterior sin que JC lo pida.

## M-22: corregido y probado contra su defensa

Registrar devuelve un ResultadoIngreso con **MotivoIngreso.NoAutorizado**, **Regla = RN-34** y mensaje exacto **«Esta operación es solo para el Administrador»** cuando falta la sesión administrativa. No lanza una excepción por ese rechazo ni registra el ingreso. En la UI se muestra ese resultado y se omite la consulta del contador, que también requiere autorización. El resto de los motivos y la guarda RN-42 de eventos de otro propietario conservan su contrato.

CP-146 tiene dos variantes: cliente autenticado y ausencia de sesión. Afirma el motivo, la regla y el mensaje exactos; ausencia de excepción; entrada confirmada sin FechaIngreso; contador cero; sesión intacta. Se ejecutó primero contra el comportamiento anterior: **2/2 rojas**. Con el arreglo: **2/2 verdes**. Al retirar RequerirAdministrador del método interno: **2/2 rojas**. Al restaurar los bytes originales: **2/2 verdes**. Evidencia en EVIDENCIA-M22-2026-11-16/ (antes-tipado-rojo, despues-verde, mutante-rojo y restaurado-verde).

La comprobación previa también distingue el hecho medido: en el código no mutado existía la excepción de dominio RN-34; el NullReferenceException sin sesión aparecía al quitar la defensa. El contrato pedido ahora elimina la excepción en ese rechazo y la transforma en resultado explícito.

### Aserción existente de CP-99 actualizada por el requisito

La primera batería completa dio 275/276 porque CP-99 todavía esperaba una ReglaNegocioException al llamar a Registrar como cliente. Se cambió esa aserción al contrato solicitado: resultado no válido, NoAutorizado, RN-34 y FechaIngreso persistida sin alterar. No se agregó un caso ni se debilitó el control: se exige la regla exacta en vez de aceptar cualquier excepción. El resto de CP-99 no se cambió. La batería completa posterior pasó 276/276.

**Observación documental para JC/Claude, sin corregirla por cuenta propia:** CP-99 sí tenía esa negativa genérica, aunque el informe de mutación la describía como un caso solo válido. Esa negativa no exigía RN-34; al quitar la guarda podía aceptar el rechazo RN-42. La llamada sin sesión en CP-99 era a Contar, no a Registrar. La medición 73/77 y su denominador no se alteran por esta precisión.

## Autorización y diff completo de Entidade/

JC autorizó expresamente en esta sesión solo dos adiciones en Entidade/ResultadoIngreso.cs: NoAutorizado y el código Regla del resultado. Antes de escribir se comparó el archivo con HEAD; después se comprobó que el contenido es exactamente el original más esas dos adiciones y que no hay ningún otro archivo modificado dentro de Entidade/. No se reordenaron los valores anteriores del enum ni se añadieron otros miembros.

Diff completo:

```diff
diff --git a/Entidade/ResultadoIngreso.cs b/Entidade/ResultadoIngreso.cs
index 3323ca1..9a3a56a 100644
--- a/Entidade/ResultadoIngreso.cs
+++ b/Entidade/ResultadoIngreso.cs
@@ -2,13 +2,14 @@ using System;
 
 namespace Entidades
 {
-    public enum MotivoIngreso { Valido, YaUsado, Reembolsado, NoConfirmado, OtroEvento, Inexistente, FueraDeHorario }
+    public enum MotivoIngreso { Valido, YaUsado, Reembolsado, NoConfirmado, OtroEvento, Inexistente, FueraDeHorario, NoAutorizado }
 
     public sealed class ResultadoIngreso
     {
         public MotivoIngreso Motivo { get; set; }
         public DateTime? FechaIngreso { get; set; }
         public string Mensaje { get; set; }
+        public string Regla { get; set; }
         public bool Valido { get { return Motivo == MotivoIngreso.Valido; } }
     }
 }
```

## Índice único de seguros

Se agregó **UX_Seguros_ID_Entrada** en el esquema nuevo y el script **BaseDatos/07-indice-unico-seguros.sql** para bases existentes. Antes de crear el índice, el script comprueba duplicados bajo bloqueo, muestra ID_Entrada y cantidad y aborta con error 50007; no elimina ni fusiona pólizas. CP-147 prueba el rechazo de la segunda póliza con **SqlException 2601** y el nombre exacto del índice; la otra variante permite un seguro sobre una entrada diferente. Rollback y comprobación de persistencia en ambas. Se retiró la creación del índice de las dos rutas del fixture para demostrar el rojo; restaurado volvió a verde.

Máximo debe seleccionar su base en SSMS y ejecutar **07-indice-unico-seguros.sql** antes del ejecutable nuevo. El script es idempotente si el índice ya está creado. Si informa duplicados, debe detenerse y evaluar esas filas: no hay limpieza automática. No se cambió la nulabilidad de ID_Compra ni ID_Sector en Entradas, ni se aplicó el script a la base de desarrollo de JC.

## U-09

Al cerrar la confirmación, Compra llama al recorrido existente VolverAlListado en vez de armar un paquete vacío. No se agregó pantalla ni control ni se rediseñó el listado. CP-148 recorre pago y confirmación reales y verifica listado visible, armado oculto e identidad conservada; dio rojo con el flujo anterior de 65225e7 y verde con el cambio.

El helper protege el nombre exacto del ticket que emite esa compra de prueba: conserva y restaura cualquier archivo previo; si es nuevo, retira solo ese archivo al terminar. No elimina directorios ni otros tickets. Capturas de verificación completas por DrawToBitmap:

| Vista | 100 % | 150 % |
|---|---|---|
| Confirmación antes de cerrarla | [imagen](U09-ULTIMO-2026-11-16/100/u09-confirmacion.png) | [imagen](U09-ULTIMO-2026-11-16/150/u09-confirmacion.png) |
| Listado al cerrar confirmación | [imagen](U09-ULTIMO-2026-11-16/100/u09-retorno-listado.png) | [imagen](U09-ULTIMO-2026-11-16/150/u09-retorno-listado.png) |

## Tres figuras del guion, pp. 47–49

Son pantallas actuales completas, DrawToBitmap al DPI nativo. No se usaron ventanas tapadas ni se modificó docs/. Máximo puede usar las versiones de 100 % para el Word a 16 cm de ancho con proporción bloqueada; también se incluyen las de 150 %.

| Página / pantalla | 100 % | 150 % | Menor letra del contenido impresa a 16 cm, 100/150 % |
|---|---|---|---|
| 47: inicio | [p47-inicio](Guion-Interfaz-ULTIMO-2026-11-16/100/p47-inicio.png) | [p47-inicio](Guion-Interfaz-ULTIMO-2026-11-16/150/p47-inicio.png) | **13,41 / 13,47 pt** |
| 48: registro | [p48-registro](Guion-Interfaz-ULTIMO-2026-11-16/100/p48-registro.png) | [p48-registro](Guion-Interfaz-ULTIMO-2026-11-16/150/p48-registro.png) | **12,20 / 12,26 pt** |
| 49: menú cliente | [p49-menu](Guion-Interfaz-ULTIMO-2026-11-16/100/p49-menu-cliente.png) | [p49-menu](Guion-Interfaz-ULTIMO-2026-11-16/150/p49-menu-cliente.png) | **14,89 / 14,97 pt** |

Medición: tamaño real en puntos de la menor fuente visible del contenido × DPI / 72 × (16 / 2,54 × 72) / ancho total del PNG en píxeles. No incluye el marco de Windows. El TSV de cada escala conserva ancho, DPI, fuente mínima y resultado. Ninguna figura del guion queda debajo de 7 pt; no hizo falta dividirlas. Las imágenes de U-09 son evidencia técnica, no una nueva figura propuesta para el documento.

## Limpieza del registro

Las ocho filas de CP-125 muestran ahora su ID y descripción en la corrida (nombre de TestMethod más sus DataRow). Se corrigieron las tildes de «después» e «inscripción» en UTF-8 con BOM. Se cambiaron únicamente los comentarios RF indicados por la matriz: CP-127 → RF-06; CP-129 → RF-06; CP-133 → RF-07; CP-138/139 → RF-08; CP-140 → RF-05; CP-143/144 → RF-11. No cambiaron sus aserciones ni el número de variantes.

## Estado completo al reanudar, antes de preparar archivos

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
	Pruebas/M22-PENDIENTE-2026-11-16/PruebasRechazoIngreso.cs.preparado
	Pruebas/M22-PENDIENTE-2026-11-16/antes-rojo.log
	Pruebas/M22-PENDIENTE-2026-11-16/propuesta.patch
	Pruebas/PruebasSeguroUnico.cs
	Pruebas/RESULTADOS-ULTIMO-2026-11-16.md
	Pruebas/U09-ULTIMO-2026-11-16/100/u09-confirmacion.png
	Pruebas/U09-ULTIMO-2026-11-16/100/u09-retorno-listado.png
	Pruebas/U09-ULTIMO-2026-11-16/100/u09.txt
	Pruebas/U09-ULTIMO-2026-11-16/150/u09-confirmacion.png
	Pruebas/U09-ULTIMO-2026-11-16/150/u09-retorno-listado.png
	Pruebas/U09-ULTIMO-2026-11-16/150/u09.txt
	Pruebas/VerificarUltimo16.py

no changes added to commit (use "git add" and/or "git commit -a")
```

## Estado completo antes de preparar los commits definitivos

```text
On branch main
Your branch is up to date with 'origin/main'.

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
	modified:   BaseDatos/01-esquema.sql
	modified:   Entidade/ResultadoIngreso.cs
	modified:   Pruebas/BaseDePrueba.cs
	modified:   Pruebas/PruebasGuardasMutacion.cs
	modified:   Pruebas/PruebasIngresos.cs
	modified:   Pruebas/PruebasPrioridadTemporal.cs
	modified:   Repo/GestorIngresos.cs
	modified:   Visual/Cliente/FrmCompraCliente.cs
	modified:   Visual/Propietario/frmControlIngreso.cs

Untracked files:
  (use "git add <file>..." to include in what will be committed)
	BaseDatos/07-indice-unico-seguros.sql
	Pruebas/CONGELAMIENTO-DEFINITIVO-2026-11-16.json
	Pruebas/CapturarUltimo16.ps1
	Pruebas/CapturasUltimasUI.cs
	Pruebas/EVIDENCIA-CIERRE-DEFINITIVO-2026-11-16/bateria-final.log
	Pruebas/EVIDENCIA-CIERRE-DEFINITIVO-2026-11-16/bateria-final.trx
	Pruebas/EVIDENCIA-CIERRE-DEFINITIVO-2026-11-16/verificaciones-locales.json
	Pruebas/EVIDENCIA-M22-2026-11-16/PruebasRechazoIngreso.cs.preparado
	Pruebas/EVIDENCIA-M22-2026-11-16/antes-rojo.log
	Pruebas/EVIDENCIA-M22-2026-11-16/antes-tipado-rojo.log
	Pruebas/EVIDENCIA-M22-2026-11-16/despues-verde.log
	Pruebas/EVIDENCIA-M22-2026-11-16/mutante-rojo.log
	Pruebas/EVIDENCIA-M22-2026-11-16/propuesta.patch
	Pruebas/EVIDENCIA-M22-2026-11-16/restaurado-verde.log
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/p47-inicio.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/p48-registro.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/p49-menu-cliente.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/100/tipografia.tsv
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/p47-inicio.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/p48-registro.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/p49-menu-cliente.png
	Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/150/tipografia.tsv
	Pruebas/PruebasRechazoIngreso.cs
	Pruebas/PruebasSeguroUnico.cs
	Pruebas/RESULTADOS-ULTIMO-2026-11-16.md
	Pruebas/U09-ULTIMO-2026-11-16/100/u09-confirmacion.png
	Pruebas/U09-ULTIMO-2026-11-16/100/u09-retorno-listado.png
	Pruebas/U09-ULTIMO-2026-11-16/100/u09.txt
	Pruebas/U09-ULTIMO-2026-11-16/150/u09-confirmacion.png
	Pruebas/U09-ULTIMO-2026-11-16/150/u09-retorno-listado.png
	Pruebas/U09-ULTIMO-2026-11-16/150/u09.txt
	Pruebas/VerificarUltimo16.py

no changes added to commit (use "git add" and/or "git commit -a")
```

Ambos App.config conservan S de skip-worktree y no se preparan ni publican. No se incluyen bin/, obj/, TestResults/, docs/ ni tools/. La excepción de Entidade/ en el índice de Git se limita al archivo autorizado; no habilita la carpeta.

## Publicación y congelamiento

El SHA definitivo será el commit que incorpore **CONGELAMIENTO-DEFINITIVO-2026-11-16.json** y este reporte, informado completo en la respuesta tras verificar origin/main y origin/JC con ls-remote. Se puede recuperar por:

```powershell
git log -1 --format=%H -- Pruebas/CONGELAMIENTO-DEFINITIVO-2026-11-16.json
```

El JSON conserva los números, la referencia histórica 65225e7 de la mutación global y los pares rojo/verde locales de M-22, índice y U-09 como evidencias distintas. No se reejecuta la mutación global ni se altera su porcentaje. No queda trabajo del encargo pendiente.

## Vueltas y tiempos

En esta continuación: rojo previo tipado de las dos filas CP-146; verde con el arreglo; rojo y verde restaurado de M-22; una barrera de 49 más color renderizado; dos baterías completas (la primera detectó la aserción antigua de CP-99, la segunda pasó 276/276); conteo por reflexión y TRX. Se aprovecharon las capturas y comprobaciones dirigidas del tramo anterior sin volver a abrir las pantallas.

Portones humanos restantes: Máximo ejecuta 07 y pega las tres figuras; JC evalúa si hace falta precisar la frase documental de CP-99. No queda autorización ni decisión pendiente para el código. No se modifica ningún otro defecto ni se agrega otra prueba después del SHA de cierre sin pedido expreso de JC.

Tramo anterior de trabajo registrado: 0,327 h. Esta continuación hasta preparar publicación: 0.178 h de reloj. Suma activa aproximada: 0.505 h. Se excluye la espera entre sesiones; no son horas-persona.
