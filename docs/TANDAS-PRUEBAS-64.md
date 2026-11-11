# Séptima tanda — las 64 de Máximo, grupo A (BORRADOR)

Claude Code, 10/11/2026. **No es un encargo vigente:** pasa a `TAREAS-CODEX-PRUEBAS.md` cuando
JC lo apruebe y le dé el turno a Codex. Clasificación y aserción de cada fila:
[PRUEBAS-64-CLASIFICACION.md](PRUEBAS-64-CLASIFICACION.md), sección 3.

**Qué cubre:** las 57 filas del grupo A, en 25 casos (PA-01 a PA-25) y cinco tareas (T-42 a
T-46). PA es un identificador provisional: Codex pone el número CP correlativo al implementar
cada caso, desde CP-66, en el orden en que los haga, y lo registra en
`docs/04-casos-prueba.md` como dice la definición de terminado.

**Por qué va antes que 6B, si JC lo aprueba:**
- 6B espera la verificación de Máximo, que no controlamos;
- estas pruebas son la red de seguridad de las pantallas del propietario que 6B va a
  rediseñar: si 6B rompe un comportamiento, se entera la batería, no la defensa.

## Triaje de T-43 (10/11/2026, Claude Code)

Reporte de Codex: `Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-10-7.md`. Los tres hallazgos
aparecen en pantallas que **todavía no tienen el diseño nuevo**. **Ninguno es del piloto.**

| Hallazgo | Qué es | Qué hay que hacer |
|---|---|---|
| **A-01**: «Volver» 8 px fuera del área cliente al 150 %, en `frmAgregarEvento` | **Defecto real** de esa pantalla, que **6B arregla** con el tratamiento estándar (pie fijo con `Dock` y ventana ajustada al área de trabajo). Además, **error mío de calendario**: la clasificación pedía la variante al 150 % antes de 6B | La fila al 150 % de PA-04 **sale de esta tanda** y entra en P6B-1, en 6B. Queda la del 100 %, que pasa. **Si T-40 no llega al 21/11**, esta pantalla necesita el arreglo mínimo igual: «Volver» en un pie fijo (ya previsto en «Si se atrasa», `PLAN-RENOVACION-VISUAL.md`) |
| **A-02**: `frmMisLocales` pisa `Tema.FilaAlterna` con un gris fijo | **Error de la clasificación.** Exigí el contrato del tema antes de 6B. El color fijo se pone en el `Load`, y el `Load` gana sobre `Tema.Normalizar` **porque así lo pidió T-32**. **6B lo arregla**, porque CP-63 extendida no admite `Color.FromArgb` en los `.cs` de las 19 pantallas | La aserción pasa a «las filas alternas tienen un fondo distinto». No se toca la aplicación ahora |
| **A-03**: «RN-06» en el título y no «(RN-06)» en el texto | **Error de la clasificación.** Pedí el formato de V-6 antes de 6B. La aplicación se comporta como antes de V-6, y está bien que así sea hasta 6B | La aserción pasa a «RN-06 en el título o en el texto», y lo mismo para RN-38 en C.10. Pasa hoy y después de 6B. **No se cambia la aplicación** |

**Por qué A-03 no es una contradicción entre V-6 y las pruebas.**
- **V-6 no saca el código de regla:** lo pasa del título al final del texto
  («… (RN-06)», `SISTEMA-DISENO.md` 5.4).
- **Cuáles de las 11 pruebas de diálogos esperan un título «RN-xx»:** una sola, CP-21, con
  «RN-11» de `frmEventosLocal`. Las otras diez ya esperan títulos en castellano.
- **Cambiar ahora solo `frmAdministrarEvento`** no afecta a ninguna de las 11. Igual no lo
  recomiendo: 6B aplica V-6 en todas las pantallas de una vez.
- **Lo que hará 6B:** cambia los 10 lugares que usan `ex.Regla` como título (`frmAdministrarEvento`,
  `frmAgregarSector`, `frmEventosLocal`, `frmHistorialCliente`, `frmPago`, `frmSuscripcion` y
  cuatro en `frmReportes`). Eso afecta a CP-21 y a la aserción de RN-38 de PA-10, que con la
  corrección de arriba ya no depende del formato.

**A confirmar a escala real.** El 8 px de A-01 sale de la simulación geométrica. No lo
repetí con Windows al 150 % porque Codex está corriendo pruebas de interfaz en esta misma PC,
y cambiar la escala se las rompe.

**JC mira mal un detalle de A-01.** El recorte **no lo destapó 6A**: también pasaba antes, en
una notebook de 1366 × 768 al 150 %.
- **Antes de 6A**, Windows estiraba la ventana como imagen, pero la ventana tenía que entrar
  igual en la pantalla. Esta mide 473 × 784 a 144 DPI: 315 × 523 lógicos, y estirada, otra vez
  473 × 784 físicos. El área útil de esa notebook ronda los 728 px, así que el pie ya quedaba
  afuera.
- **En el monitor de JC** (1920 × 1080) entra con y sin 6A. Por eso nunca se vio.

**Las baterías esperadas, corregidas:**
- **T-43:** 14 ejecuciones (la fila al 150 % de PA-04 pasa a 6B). Da 96 con PA-02 y 95 sin ella.
- **Las siguientes:** T-44 112, T-45 118 y T-46 135, o una más cada una si JC autoriza las
  dos variantes de PA-02.
- **Pendiente de JC:** la confirmación de PA-02, que Codex pidió.

## Reglas de la tanda

1. **Zona:** `Pruebas/`. En `Visual/`, **solo** el cambio de la tarea T-46, y únicamente si JC lo
   autorizó (ver T-46). Ningún `.Designer.cs`, ni `Repo/`, `Datos/`, `BaseDatos/` ni
   `App.config`.
2. **Técnica:** la de `PruebasInterfaz.cs`. Formularios reales en el proceso de prueba, datos
   con `Fake`, `MessageBox` respondidos con `DialogosPrueba`. Base `BddEventAura_Pruebas`.
3. **Las pruebas no buscan controles por posición ni por tipo exacto.** Los buscan por nombre
   (`Controls.Find`) y aceptan cualquier tipo derivado: 6B va a pasar `Button` a
   `BotonPrimario` y `Label` a `LinkLabel` o a `BotonSecundario`, conservando los nombres.
4. **Nada de colores escritos:** si una aserción compara un color, compara con un token de
   `Tema` (`Tema.FilaAlterna`, `Tema.Primario`).
5. **Ningún archivo en Documentos.** Todo PDF va a una ruta temporal y se borra al final, como
   CP-65.
6. **Cada prueba deja la base como la encontró**, o crea sus propios datos con nombres únicos.
   La batería tiene que pasar dos veces seguidas.
7. **Si una prueba encuentra un defecto de la aplicación:**
   - no se debilita la prueba ni se corrige la aplicación en la misma tanda;
   - se para, y el reporte dice qué falla, con qué datos y qué se esperaba.

   Decide JC.
8. **Batería al terminar cada tarea**, con la cuenta esperada de la tabla final. Si no
   coincide, se explica en el reporte.
9. **Sin commit ni push.** Reporte en `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-7.md`, con la
   correspondencia PA → CP y las filas manuales que cubre cada caso.

---

## T-42. Login (A y C.1 a C.3) — 3 casos

| Caso | Filas | Qué hace |
|---|---|---|
| PA-01 | 3.1, C.1 | `frmInicio` abierto: `ActiveControl` es `txtmail`. Inicio de sesión con admin@ y `Admin1234` → queda abierto `frmMenuPropietario` y la sesión es del Administrador. 2 ejecuciones |
| PA-02 | 3.2 | Dos inicios seguidos con admin@: los dos devuelven el usuario, y la contraseña guardada no es el texto plano. **Antes de escribirla:** revisar si `GestorUsuarios` todavía tiene una ruta de migración desde texto plano. Si la tiene, agregar la segunda variante (ver la clasificación, sección 5). Si no, decirlo en el reporte. 1 ejecución (o 2) |
| PA-03 | C.2, C.3 | Cinco inicios fallidos desde la pantalla: los textos de los `MessageBox`, de «Le quedan 4» a «1 intento(s)», y el 5.º con «15 minutos». Después, `BloqueadoHasta` = ahora + 10 min → «10 minutos»; ahora + 5 min → «5 minutos». Restaurar el usuario al terminar. 2 ejecuciones |

**Batería esperada: 82** (77 + 5).

## T-43. Propietario: locales, eventos y sectores (B, C.4 a C.13, C9.1, 9.17) — 7 casos

| Caso | Filas | Qué hace |
|---|---|---|
| PA-04 | C9.1, 9.17 | `frmAgregarEvento`: el selector de preventa con casilla y destildado, formato `dd/MM/yyyy HH:mm`; «Registrar» y «Volver» dentro del área cliente y sin superponerse a nada, al 100 % y en la escala simulada de CP-25 (1366 × 728 al 150 %). 2 ejecuciones |
| PA-05 | 4.6, C.4 | `frmRegistrarLocal`: orden de Tab desde Nombre hasta el botón. Alta de un local con nombre único → aviso, fila en `Locales` y fila en `frmMisLocales`. 2 ejecuciones |
| PA-06 | C.12, C.13 | `frmMisLocales`: las propiedades de la grilla de C.12. Elegir la segunda fila con un solo `CurrentCell` y abrir «Eventos del local» → es el de ese local. 2 ejecuciones |
| PA-07 | 4.2, C.5 | `frmAgregarSector`: el botón dice «Agregar sector». Precio «12500,50» y, en otra ejecución, «12500.50» → `Precio` = 12500.50. 3 ejecuciones |
| PA-08 | C.6, C.7 | `frmAgregarEvento`: con nombre y categoría → aviso de borrador, la fila en el listado dice «Borrador». Sin categoría → aviso y ninguna fila nueva. 2 ejecuciones |
| PA-09 | C.8, C.9 | Sobre un borrador: un sector de capacidad 0 → se guarda y sigue en Borrador; uno de capacidad > 0 → «Publicado». 2 ejecuciones |
| PA-10 | C.10, C.11 | Desde la pantalla: RN-38 con «1000» en el texto y sin inserción; RN-06 al eliminar el último sector con capacidad, y el sector sigue. 2 ejecuciones |

**Batería esperada: 97** (82 + 15).

## T-44. Suscripción y preventa (8.3 a 8.9, 8.21, 8.22, C9.2 a C9.11) — 7 casos

| Caso | Filas | Qué hace |
|---|---|---|
| PA-11 | 8.3 | `frmSuscripcion` con un cliente sin plan: los textos exactos, los precios desde `Parametros`, «Cancelar suscripción» deshabilitado y ninguna etiqueta recortada. 1 ejecución |
| PA-12 | 8.4, 8.6 | Contratar el Mensual: la confirmación trae el precio y «no se renueva»; después, el estado y los habilitados. Cancelar: la confirmación tiene «No» por defecto; al aceptar, «No tenés una suscripción activa.». 2 ejecuciones |
| PA-13 | 8.5 | Dos `frmSuscripcion` del mismo cliente; se contrata en las dos → RN-25 con el vencimiento en la segunda, y una sola fila activa. 1 ejecución |
| PA-14 | 8.7, 8.9 | `FrmCompraCliente`, 2 Campo y seguro: con suscripción, «Seguro sin cargo (2 entradas) $ 0,00», el TOTAL igual y sin la pregunta del seguro; sin suscripción, «Seguro (2 entradas) $ 10.000,00». 2 ejecuciones |
| PA-15 | 8.8, C9.10, C9.11 | Pago aprobado de un suscriptor con seguro → la confirmación dice «Seguro $ 0,00»; `ConSeguro = 1`, `MontoSeguro = 0` y una póliza por entrada con `Precio = 0`. Sin seguro → `ConSeguro = 0` y ninguna póliza. 2 ejecuciones |
| PA-16 | 8.21, 8.22, C9.4, C9.5, C9.8 | Evento con `FinPreventa` = mañana 20:00: aparece en el listado; un cliente sin suscripción → aviso RN-39 con «mañana a las 20:00» y vuelta al listado; uno suscripto → abre la compra. Con `FinPreventa` NULL → compran los dos. 3 ejecuciones |
| PA-17 | C9.2, C9.3, C9.6, C9.7, C9.9 | Desde el propietario: alta con preventa (se guarda la fecha, se publica con un sector); preventa posterior al evento (aviso, sin fila nueva); administrar con preventa (selector tildado con la fecha); quitarla (aviso y NULL); administrar sin preventa (destildado). 5 ejecuciones |

**Batería esperada: 113** (97 + 16).

## T-45. Listas de espera y menús (8.23 a 8.26, 9.1, 9.16) — 3 casos

| Caso | Filas | Qué hace |
|---|---|---|
| PA-18 | 8.23, 8.25 | `frmListasEspera` del administrador, con un evento agotado y dos inscriptos (uno suscripto): el texto del combo, las columnas exactas, el suscriptor arriba, los contadores y `NotSortable`. Con un propietario sin eventos agotados: combo deshabilitado y el aviso exacto. 2 ejecuciones |
| PA-19 | 8.24 | RN-35: inscribir a bronce@ sin suscripción y a oro@ con suscripción; cancelar la suscripción de oro@ → oro@ sigue primero. **Primera prueba de RN-35.** Ver la nota de la clasificación: la redacción de RN-35 todavía espera a Máximo. 1 ejecución |
| PA-20 | 9.1, 9.16, 8.26 | `frmMenuPropietario`: los cuatro botones y «Volver al inicio», visibles y dentro del área cliente. `frmMenuCliente`: ningún control con «Reporte». Texto de `Visual/*.cs` sin `SqlConnection`, `SqlCommand` ni `ConexionSql` (como CP-53). 3 ejecuciones |

**Batería esperada: 119** (113 + 6).

## T-46. Reportes (9.2 a 9.15) — 5 casos

**Dos decisiones previas de JC, sin las cuales esta tarea no arranca:**

1. **Un cambio chico en `Visual/` para poder probar la exportación sin escribir en Documentos.**
   - **Qué se agrega:** una propiedad interna para la carpeta de salida de los reportes, con
     Documentos como valor por omisión y `InternalsVisibleTo("Pruebas")`.
   - **Qué cambia para el usuario:** nada.
   - **Qué pasa sin él:** 9.11 y 9.14 vuelven a ser manuales (5 min de JC).
2. **Cargar `BaseDatos/03-datos-reporte.sql` en la base de pruebas.**
   - **Cómo:** en el `ClassInitialize` de la clase de estos casos. En el `ClassCleanup`, se
     borra lo que cargó con el mismo criterio que usa el bloque 1 del script: el local «Predio
     Rosario (datos de reporte)» y los usuarios `rpt####@datos.eventaura.com`. Ese borrado lo
     escribe Codex en `Fake`, sin tocar `BaseDatos/`.
   - **Riesgo:** que otro caso vea esos datos si el orden cambia. La regla 6 lo controla: la
     batería pasa dos veces seguidas. Si no pasa, se para y se informa.

| Caso | Filas | Qué hace |
|---|---|---|
| PA-21 | 9.15 | Con la semilla sola: los cinco reportes se generan y se exportan a un exportador de prueba (como CP-38). En RPT-01, el Festival tiene 0 entradas, 0,0 % y ticket «—». 1 ejecución |
| PA-22 | 9.2, 9.3 | `frmReportes`: valores iniciales (combo, período, «Todas»), y para cada reporte, qué filtro se ve, la grilla vacía y «Exportar PDF» deshabilitado. 5 ejecuciones |
| PA-23 | 9.4 a 9.10 | Con `03-datos-reporte.sql`, período 01/09/2025 a 30/11/2025: los valores de cada fila, una ejecución por fila (7). Una más para la presentación de RPT-01: alineación a la derecha, TOTAL en negrita y resaltado, `NotSortable` y lectura sin recorte. 8 ejecuciones |
| PA-24 | 9.11, 9.14 | Con la carpeta temporal: el aviso trae la ruta y el nombre `EVAU_RPT01_20250901_20251130.pdf` (se responde «No»). Con el archivo de destino abierto con `FileShare.None` → «No se pudo escribir el archivo…» y la pantalla sigue abierta. 2 ejecuciones |
| PA-25 | 9.12, parte A | El texto del PDF exportado (`PdfTextExtractor`, como CP-65) tiene «EventAura», el título, el código, el período, «Solicitado por: Ana Administradora» y «Página 1 de 1». 1 ejecución |

**Batería esperada: 136** (119 + 17).

---

## Resumen y costo

| Tarea | Casos | Ejecuciones nuevas | Batería al terminar | Horas de Codex (estimadas) |
|---|---|---|---|---|
| T-42 | 3 | 5 | 82 | 2 a 3 |
| T-43 | 7 | 15 | 97 | 4 a 6 |
| T-44 | 7 | 16 | 113 | 5 a 7 |
| T-45 | 3 | 6 | 119 | 3 a 4 |
| T-46 | 5 | 17 | 136 | 5 a 7 |
| **Total** | **25** | **59** | **136** | **19 a 27** |

**Si 6B entra antes**, las baterías esperadas se corren en las filas que haya agregado 6B (48,
según `ENCARGO-6B-BORRADOR.md`).

**Después de la tanda, Claude Code:**
- AH-1 y AH-2 (10.2 y 10.3) como scripts en `tools/`, más o menos 1 hora;
- mueve el reporte a `docs/`;
- actualiza `04-casos-prueba.md`, la matriz, `COMO-SE-PROBO.md`, `PRUEBAS-PENDIENTES.md` y las
  cifras del ítem 23 de `CORRECCIONES-DOCUMENTO.md`;
- genera el PDF de reporte para la lista de JC.
