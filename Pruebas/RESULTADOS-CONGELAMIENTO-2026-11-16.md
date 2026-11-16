# Cierre visual y congelamiento — 16/11/2026

## Publicación inicial

Se publicó primero el trabajo que bloqueaba a Máximo:
- 1525202: prioridad temporal, migración 06 y pruebas verificadas por mutación.
- baf0598: capturas completas y propuestas.

Se verificó directamente con git ls-remote que main y JC apuntaban a baf0598. El respaldo remoto continúa en 98be355, sin modificaciones. El estado completo ANTES de preparar esos commits está pegado en RESULTADOS-PUBLICACION-CIERRE-2026-11-16.md.

```text
On branch main
Your branch is up to date with 'origin/main'.

nothing to commit, working tree clean
```

```text
git log --oneline -1 origin/main
baf0598 test(visual): capturas completas con DrawToBitmap y propuestas de grillas
git log --oneline -1 origin/JC
baf0598 test(visual): capturas completas con DrawToBitmap y propuestas de grillas
```

## Historial y Reportes: A aplicada

Se modificaron únicamente los dos Designer de producción: Visual/Cliente/frmHistorialCliente.Designer.cs y Visual/Propietario/frmReportes.Designer.cs. Sin fuentes, colores ni referencias al tema agregadas. Ambos usan referencia de 96 DPI y medidas fijas, con el tema existente para el estilo. No se modificaron otros Designer ni código de negocio en este cierre.

Historial conserva las dos grillas simultáneas: compras y entradas del pedido elegido; ayuda y acciones al pie. Reportes ubica selector, fechas y filtros condicionales arriba, con grilla a todo el ancho y lectura/exportación debajo.

La verificación de los cinco reportes midió lecturas de 57 px en RPT-01 y RPT-02 frente a los 40 px del boceto. Se ajustó la misma distribución A: lectura 64 px en y=368 y grilla 128 px en vez de 152. No creció la ventana por ese ajuste; se conservó el pie. Las demás lecturas midieron 38, 19 y 19 px con el juego usado. Registro: TestResults/Cierre16/lecturas.log. No se añadió un caso de prueba por esta medición.

Medidas reales de área cliente a 100 % (no confundir las medidas del antiguo Designer a 120 DPI con píxeles efectivos):

| Pantalla | Antes | Después |
|---|---|---|
| Historial | 644×448 | 900×512 |
| Reportes | 825×506 | 900×512 |

Las dos quedan en 1350×768 de área cliente al 150 %. El marco exterior depende de cada formulario y Windows. Las medidas anteriores/posteriores se registraron con el host de la aplicación a DPI nativo.

Capturas completas DrawToBitmap, versionadas:

| Pantalla | Antes 100 % | Después 100 % | Antes 150 % | Después 150 % |
|---|---|---|---|---|
| Historial | [antes](Capturas-CIERRE-2026-11-16/antes-100/frmHistorialCliente.png) | [después](Capturas-CIERRE-2026-11-16/despues-100/frmHistorialCliente.png) | [antes](Capturas-CIERRE-2026-11-16/antes-150/frmHistorialCliente.png) | [después](Capturas-CIERRE-2026-11-16/despues-150/frmHistorialCliente.png) |
| Reportes | [antes](Capturas-CIERRE-2026-11-16/antes-100/frmReportes.png) | [después](Capturas-CIERRE-2026-11-16/despues-100/frmReportes.png) | [antes](Capturas-CIERRE-2026-11-16/antes-150/frmReportes.png) | [después](Capturas-CIERRE-2026-11-16/despues-150/frmReportes.png) |

Barrera después de Historial: 49/49. Después de Reportes: 49/49. Después de ajustar su lectura: 49/49. Cada una incluye CP-63 en veinte pantallas, siete pruebas de serialización del host real, CP-116 y CP-120. Tras cada barrera: 31 primarios renderizados correctamente, cero fallos de color. No hubo barrera roja.

El barrido nativo de todos los botones pasó de nuevo en las veinte pantallas a 96 y 144 DPI. Los archivos están en TestResults/Botones/nativo-100.log y nativo-150.log. No se agregaron ni modificaron casos de prueba.

## Cuatro figuras del instructivo: correspondencia y regeneración

Las figuras viejas de docs/capturas/instructivo/ no se sobrescribieron. Las nuevas están en la zona propia, versionadas, con los mismos nombres pedidos por el instructivo. Claude o Máximo pueden copiarlas a su carpeta de documentación cuando corresponda.

| Figura, pp. 52–53 | Pantalla o vista real | PNG nuevo, 100 % |
|---|---|---|
| 1, pasos 1–2 | frmEventosCliente, Duki seleccionado | [figura1-listado-eventos.png](Figuras-Instructivo-2026-11-16/100/figura1-listado-eventos.png) |
| 2, pasos 3–5 | FrmCompraCliente, dos Campo y hospedaje | [figura2-armado-paquete.png](Figuras-Instructivo-2026-11-16/100/figura2-armado-paquete.png) |
| 3, paso 6 | pnlResumen de Compra, incluido el TOTAL violeta | [figura3-resumen-descuento.png](Figuras-Instructivo-2026-11-16/100/figura3-resumen-descuento.png) |
| 4, pasos 7–8 | frmConfirmacion actual y primer ticket PDF del mismo pedido | [figura4-confirmacion-y-comprobante.png](Figuras-Instructivo-2026-11-16/100/figura4-confirmacion-y-comprobante.png) |

Se regeneraron las cuatro en vez de reutilizar el recorrido genérico de las veinte pantallas: ese recorrido usa cliente Bronce y otros importes, y no representa el ejemplo resuelto del documento.

Ejemplo verificado durante la captura: Paula Plata, 2 Campo de $25.000, hospedaje $45.000, descuento $2.500, cargo $7.125, total **$99.625**, sin seguro. La captura exige por código total 99625 y descuento 0,05; si no coinciden, se detiene. Se crearon la compra, el pago y sus entradas reales en _Pruebas, con los gestores actuales. La pantalla muestra sus datos persistidos, no valores pintados a mano.

La figura 3 amplía temporalmente el panel solamente en memoria para capturarlo completo, incluido el TOTAL que en la ventana puede requerir desplazamiento. No se altera el Designer. La figura 4 utiliza la confirmación actual con paso 4 y el icono de éxito Windows. El ticket se emitió mediante el mismo GenerarPDF de producción con el QR de la entrada persistida y el mismo pedido; se adjuntan ambos PDF y la confirmación separada para verificarlo. Se usó una ruta explícita bajo Pruebas/; no se escribió en Documentos. La carpeta visible de la confirmación es una ruta de demostración neutra.

Hay versiones a 100 y 150 % en subcarpetas 100/ y 150/. Para el Word se proponen las cuatro principales de 100/, manteniendo el ancho del texto y la proporción, como indica el encargo de Claude. No se agregó figura 4a de pago: no se usa en el instructivo. La fecha del evento es la de la semilla de esta corrida, 15/01/2027; revisar cualquier fecha literal del ejemplo de Word antes de pegar.

Reproducción: compilar, ejecutar Pruebas/CapturarFigurasCierre.ps1 -Escala 100 o 150. Requiere pymupdf en TestResults/Cierre16/pythondeps para rasterizar el ticket; se instaló solamente allí, sin cambiar dependencias de la aplicación. El host usa la configuración compilada de Pruebas y verifica el sufijo _Pruebas antes de recrearla. No ejecutar a la vez que otra corrida de pruebas o mutación.

## Cifras congeladas

| Magnitud | Resultado |
|---|---|
| Casos identificados en el catálogo | **145**, CP-01 a CP-145 |
| Métodos MSTest que los implementan | **142** |
| Ejecuciones, incluyendo variantes parametrizadas | **271** |
| Correctas / fallidas / omitidas | **271 / 0 / 0** |
| Reglas con caso / total | **39/41 = 95,1 %** |
| Reglas sin caso específico | RN-07 y RN-33 |
| Mutación dirigida del encargo más prioridad | **23/23**, rojo con defensa rota y verde restaurada |
| Barrera visual | **49/49** |
| Color renderizado | **31 primarios en 20 pantallas; 0 fallos** |
| Botones completos en DPI nativo | **20/20 a 96 DPI y 20/20 a 144 DPI** |

Los casos identificados se contaron por sus atributos y por el nombre del método para los seis originales CP-05 a CP-10 que no tienen ID en el nombre visible del atributo. La reflexión encontró 142 métodos de prueba; algunos agrupan más de un ID mediante variantes. La TRX de la batería final confirmó las 271 ejecuciones correctas. Ninguna prueba nueva ni regla adicional se declaró cubierta durante este cierre.

Batería final: TestResults/Cierre16/full/congelado.trx, TestResults/Cierre16/final.log, **57,9271 segundos**. La revisión global final de mutación de Claude está pendiente y se hará sobre el commit congelado. El 23/23 es el resultado dirigido verificado en la tanda anterior; no se presenta como una nueva corrida global. Repo/ y Datos/ no cambiaron desde aquella verificación en este cierre visual.

CONGELAMIENTO-2026-11-16.json guarda las cifras y SHA-256 de las capturas/PDF. El commit exacto de congelamiento es el que incorpora ese archivo y este reporte; se informa su SHA completo tras publicar y se puede recuperar sin ambigüedad con:

```powershell
git log -1 --format=%H -- Pruebas/CONGELAMIENTO-2026-11-16.json
```

No se agrega ninguna prueba posterior a ese commit sin avisar previamente a JC.

## Estado completo antes de preparar el cierre

```text
On branch main
Your branch is up to date with 'origin/main'.

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
	modified:   Pruebas/CapturasTandaVisual.cs
	modified:   Visual/Cliente/frmHistorialCliente.Designer.cs
	modified:   Visual/Propietario/frmReportes.Designer.cs
	modified:   docs/02-reglas-negocio.md
	modified:   docs/04-casos-prueba.md
	modified:   docs/AUDITORIA-DOCUMENTO-CODIGO.md
	modified:   docs/LISTA-PARA-MAXIMO.md
	modified:   docs/METODOLOGIA-VERIFICACION.md

Untracked files:
  (use "git add <file>..." to include in what will be committed)
	Pruebas/CONGELAMIENTO-2026-11-16.json
	Pruebas/CapturarCongelamiento.ps1
	Pruebas/CapturarFigurasCierre.ps1
	Pruebas/Capturas-CIERRE-2026-11-16/antes-100/frmHistorialCliente.png
	Pruebas/Capturas-CIERRE-2026-11-16/antes-100/frmReportes.png
	Pruebas/Capturas-CIERRE-2026-11-16/antes-150/frmHistorialCliente.png
	Pruebas/Capturas-CIERRE-2026-11-16/antes-150/frmReportes.png
	Pruebas/Capturas-CIERRE-2026-11-16/despues-100/frmHistorialCliente.png
	Pruebas/Capturas-CIERRE-2026-11-16/despues-100/frmReportes.png
	Pruebas/Capturas-CIERRE-2026-11-16/despues-150/frmHistorialCliente.png
	Pruebas/Capturas-CIERRE-2026-11-16/despues-150/frmReportes.png
	Pruebas/CapturasInstructivoCongelado.cs
	Pruebas/Figuras-Instructivo-2026-11-16/100/Ticket_EA-000012_7.pdf
	Pruebas/Figuras-Instructivo-2026-11-16/100/Ticket_EA-000012_8.pdf
	Pruebas/Figuras-Instructivo-2026-11-16/100/ejemplo.txt
	Pruebas/Figuras-Instructivo-2026-11-16/100/figura1-listado-eventos.png
	Pruebas/Figuras-Instructivo-2026-11-16/100/figura2-armado-paquete.png
	Pruebas/Figuras-Instructivo-2026-11-16/100/figura3-resumen-descuento.png
	Pruebas/Figuras-Instructivo-2026-11-16/100/figura4-confirmacion-y-comprobante.png
	Pruebas/Figuras-Instructivo-2026-11-16/100/figura4b-confirmacion.png
	Pruebas/Figuras-Instructivo-2026-11-16/100/ticket.png
	Pruebas/Figuras-Instructivo-2026-11-16/150/Ticket_EA-000012_7.pdf
	Pruebas/Figuras-Instructivo-2026-11-16/150/Ticket_EA-000012_8.pdf
	Pruebas/Figuras-Instructivo-2026-11-16/150/ejemplo.txt
	Pruebas/Figuras-Instructivo-2026-11-16/150/figura1-listado-eventos.png
	Pruebas/Figuras-Instructivo-2026-11-16/150/figura2-armado-paquete.png
	Pruebas/Figuras-Instructivo-2026-11-16/150/figura3-resumen-descuento.png
	Pruebas/Figuras-Instructivo-2026-11-16/150/figura4-confirmacion-y-comprobante.png
	Pruebas/Figuras-Instructivo-2026-11-16/150/figura4b-confirmacion.png
	Pruebas/Figuras-Instructivo-2026-11-16/150/ticket.png
	tools/ListaParaMaximo/.gitignore
	tools/ListaParaMaximo/Armar.py
	tools/ListaParaMaximo/plantilla.tpl
	tools/ListaParaMaximo/reemplazos.py

no changes added to commit (use "git add" and/or "git commit -a")
```

Los cambios paralelos de docs/ y tools/ son de Claude. No se preparan, se modifican ni se publican en nuestros commits. Ambos App.config siguen S de skip-worktree; ningún App.config preparado. Tampoco se agregan bin/, obj/ o TestResults/.

## Vueltas de herramienta y portones humanos

Estimación inicial: 25–45 vueltas de herramienta para Codex. Ejecuciones verificables de este cierre: tres barreras de 49 casos y sus controles de color, dos barridos nativos de veinte pantallas, una batería completa, dos pasadas de captura antes y cuatro después (el segundo par actualiza Reportes tras medir la lectura), dos pasadas de figuras del instructivo. Además se contaron métodos por reflexión y casos/ejecuciones por la TRX. No se repitió la mutación dirigida; la corrida global final queda para Claude.

Portones humanos restantes: Máximo aplica 06 en su base y pega las figuras nuevas; JC revisa que los pasos 1–8 describan indicador y éxito actuales; Claude corre la batería y la mutación global final contra el SHA congelado. Las dos distribuciones ya fueron decididas por JC y aplicadas; no queda elección A/B pendiente.
Tiempo real de reloj: 11:18:33 a 11:36:04 ART = 0,292 h. Tiempo de Codex, no horas-persona; incluye publicación inicial, capturas, herramientas PDF y validación.
