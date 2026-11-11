# Las 64 pruebas de Máximo: inventario, clasificación y redacción

Claude Code, 10/11/2026, a pedido de JC. **Decisión de JC:** las 64 pruebas manuales que eran
de Máximo las hacemos nosotros. Este archivo es la decisión previa: qué se automatiza, qué
no y a qué costo. **No se escribió ninguna prueba.** Las tandas para Codex están en
[TANDAS-PRUEBAS-64.md](TANDAS-PRUEBAS-64.md).

## 1. De dónde salen las 64, y qué tan completa es la lista

Fuente: [PRUEBAS-PENDIENTES.md](PRUEBAS-PENDIENTES.md), «Parte de Ambro». Son seis secciones,
contadas fila por fila:

| Sección | Filas | IDs |
|---|---|---|
| A. Login (Sección 3) | 3 | 3.1 a 3.3 |
| B. Eventos y locales (Sección 4) | 3 | 4.2, 4.4, 4.6 |
| C. Correcciones C-1 a C-7 | 13 | C.1 a C.13 |
| D. Suscripción, preventa y listas de espera (Sección 8) | 13 | 8.3 a 8.9, 8.21 a 8.26 |
| E. Preventa desde el propietario (C-9) | 11 | C9.1 a C9.11 |
| F. Reportes (Sección 9) | 17 | 9.1 a 9.17 |
| G. Cierre (Sección 10) | 4 | 10.1 a 10.4 |
| **Total** | **64** | |

**La lista está completa, pero hay dos problemas que no completé por mi cuenta:**
- **Inconsistencia de secciones.** `COMO-SE-PROBO.md` dice que las de Máximo son «A, B, C, D,
  F y G», sin la E. Sin la E son 53, no 64. La E está dentro de «Parte de Ambro» en
  `PRUEBAS-PENDIENTES.md`, así que la cuenta correcta es con E. Hay que corregir
  `COMO-SE-PROBO.md` cuando se cierren las tandas.
- **Filas repetidas dentro de la lista.** Algunas prueban lo mismo con otro camino:
  - C9.4 es 8.21; C9.5 es 8.22; C9.8 repite C9.4 y C9.5;
  - C9.10 se superpone con 8.8.

  Se cuentan como filas distintas porque el archivo las numera aparte, pero en las tandas
  comparten caso.

## 2. Criterio de clasificación (estricto)

**Una aclaración sobre «UIA, igual que las 77».** Las 77 no usan UI Automation: abren los
formularios reales **dentro del proceso de prueba**, escriben en sus controles y responden sus
`MessageBox` (`DialogosPrueba.cs`). UI Automation sobre el ejecutable es lo que usé yo el
03/11 para las 65 de JC. El grupo A de acá es lo primero: casos MSTest de Codex, con la misma
técnica que `PruebasInterfaz.cs`.

| Grupo | Entra solo si… |
|---|---|
| **A — Codex** | Hay **una aserción concreta sobre un valor**: un texto, una propiedad, una fila de la base o un rectángulo. Un «se ve bien» no es una aserción. Si la fila tiene una parte que solo juzga una persona, esa parte va a B y queda anotada |
| **A0 — ya cubierta** | Un caso existente comprueba **lo mismo y por el mismo camino**. Si cubre la regla pero no lo que se ve en pantalla, no cuenta: va a A |
| **AH — herramienta de Claude Code** | Es automatizable, pero no es una prueba de la aplicación sino del entorno (la batería, las bases). Va como script en `tools/`, no en MSTest |
| **B — persona, rápida** | Juicio visual o de lectura |
| **C — persona y algo físico** | Celular, impresora u otro programa instalado |
| **X — sin objeto** | Describe algo que ya no existe |

**El residuo visual.** Varias filas de A tienen además un «nada cortado» o un «se ve».
- **Lo que A sí atrapa:** con aserciones geométricas, que el alto preferido no supere el alto
  y que los hermanos no se superpongan, A detecta los recortes y las superposiciones.
- **Lo que A no atrapa:** que la pantalla «se vea bien».
- **Cómo se cubre:** con **una sola** mirada de JC a las pantallas del propietario después
  de 6B (ítem B-0 de la lista de JC). No se mira una vez por fila.

## 3. Inventario y clasificación

«Caso nuevo» es el identificador provisional de `TANDAS-PRUEBAS-64.md`. El número CP
definitivo lo pone Codex al implementarlo, correlativo desde CP-66.

### A. Login

| ID | Qué verifica | Grupo | Control y aserción | Caso |
|---|---|---|---|---|
| 3.1 | El administrador entra al menú de propietario | A | `frmInicio`: `txtmail` = admin@, contraseña, clic en el botón de inicio → queda abierto un `frmMenuPropietario` y `GestorSesion` tiene al Administrador | PA-01 |
| 3.2 | Se vuelve a entrar con la misma contraseña («contra el hash ya migrado») | A | Dos inicios de sesión seguidos con admin@ devuelven el usuario; la contraseña guardada en `Usuario` no es el texto plano. **Ambigua**, ver 3.c | PA-02 |
| 3.3 | Tras 5 fallos, el 6.º se rechaza aunque sea correcto | **A0** | CP-28 lo comprueba exacto: bloqueo al tope y rechazo de la contraseña correcta mientras dura | — |

### B. Eventos y locales

| ID | Qué verifica | Grupo | Control y aserción | Caso |
|---|---|---|---|---|
| 4.2 | Precio de sector con decimales, con coma o con punto | A | `frmAgregarSector`: precio «12500,50» y, en otra fila, «12500.50» → `Sectores.Precio` = 12500.50 en las dos | PA-07 |
| 4.4 | Eliminar un evento con ventas da RN-11 y ofrece suspender | **A0** | CP-21: confirmación con No por defecto; con ventas no elimina y ofrece suspender | — |
| 4.6 | Alta y listado de locales «funcionan desde la interfaz» | A | Vaga, ver 3.c. `frmRegistrarLocal` → aviso de alta; `frmMisLocales` muestra la fila con ese nombre y esa capacidad | PA-05 |

### C. Correcciones C-1 a C-7

| ID | Qué verifica | Grupo | Control y aserción | Caso |
|---|---|---|---|---|
| C.1 | El cursor arranca en el mail | A | `frmInicio` mostrado → `ActiveControl == txtmail` | PA-01 |
| C.2 | Intentos 1 a 4 avisan cuántos quedan; el 5.º bloquea | A | `frmInicio`, 5 inicios fallidos → los textos de los `MessageBox` contienen «Le quedan 4» … «1 intento(s)» y el 5.º, «15 minutos». CP-28 cubre los gestores, no la pantalla | PA-03 |
| C.3 | Con la cuenta bloqueada, indica los minutos, que bajan | A | Sin esperar: `Usuario.BloqueadoHasta` = ahora + 10 min → el aviso dice 10; después ahora + 5 → dice 5. Ver 3.c | PA-03 |
| C.4 | Orden de Tab en Registrar local | A | `SelectNextControl` desde `txtNombre` recorre Dirección → Altura → Capacidad → botón | PA-05 |
| C.5 | El botón de «Registrar sector» dice «Agregar sector» | A | `frmAgregarSector`: `Text` del botón | PA-07 |
| C.6 | Evento con nombre y categoría queda en Borrador | A | `frmAgregarEvento` → aviso de borrador, vuelve al listado y la fila dice «Borrador»; `Eventos.Estado = 'Borrador'` | PA-08 |
| C.7 | Sin categoría no guarda | A | Aviso que pide la categoría; `Eventos` sin fila nueva | PA-08 |
| C.8 | Sector con capacidad 0 se guarda y el evento sigue en Borrador | A | `frmAdministrarEvento` + `frmAgregarSector` → fila en `Sectores`; `Estado = 'Borrador'` | PA-09 |
| C.9 | Sector con capacidad > 0 publica el evento | A | `Estado = 'Publicado'` y la grilla lo muestra | PA-09 |
| C.10 | RN-38 con «Capacidad disponible para este evento: 1000» | A | El texto del aviso contiene «1000»; «RN-38» aparece en el título **o** en el texto; el sector no se inserta. CP-60 cubre la regla, no el texto. *(Corregido el 10/11: no se exige el formato de V-6, que llega con 6B)* | PA-10 |
| C.11 | Eliminar el último sector con capacidad da RN-06 | A | El texto contiene «es el último con capacidad mayor que cero»; «RN-06» aparece en el título **o** en el texto; el sector sigue. CP-56 cubre la regla, no la pantalla. *(Corregido el 10/11: la versión anterior exigía «(RN-06)» en el texto, que es el formato de V-6 y llega con 6B; ver hallazgo A-03 en `TANDAS-PRUEBAS-64.md`)* | PA-10 |
| C.12 | Formato de la grilla de Mis locales | A | Sin columna ID; encabezados exactos; `ReadOnly`; sin fila nueva; `FullRowSelect`; `Fill`; capacidad alineada a la derecha; **las filas alternas tienen un fondo distinto de las demás**. *(Corregido el 10/11: la versión anterior exigía `Tema.FilaAlterna`, que es el contrato de 6B; ver hallazgo A-02)* | PA-06 |
| C.13 | Un solo clic elige el local y «Eventos del local» abre ese | A | Se elige la fila 2 con `CurrentCell` → el `frmEventosLocal` abierto es el de ese local | PA-06 |

### D. Suscripción, preventa y listas de espera

| ID | Qué verifica | Grupo | Control y aserción | Caso |
|---|---|---|---|---|
| 8.3 | «Mi suscripción» sin plan | A | Textos exactos de estado, beneficios, «Sin descuento extra…» y precios; «Cancelar suscripción» deshabilitado; ninguna etiqueta recortada (alto preferido ≤ alto) | PA-11 |
| 8.4 | Contratar el Mensual | A | Confirmación con el precio y «no se renueva»; después, «Plan Mensual activo», vence hoy + 1 mes, opciones deshabilitadas y «Cancelar» habilitado | PA-12 |
| 8.5 | RN-25 con dos ventanas abiertas | A | Dos `frmSuscripcion` del mismo cliente; se contrata en la primera y después en la segunda → aviso RN-25 con el vencimiento; una sola fila activa en `Suscripciones`. Son dos ventanas en un proceso, no dos procesos: lo que se prueba (pantalla desactualizada y regla en la base) es lo mismo | PA-13 |
| 8.6 | Cancelar la suscripción | A | Confirmación con No por defecto; al aceptar, «No tenés una suscripción activa.» | PA-12 |
| 8.7 | Seguro sin cargo para un suscriptor | A | `lblResumen` contiene «Seguro sin cargo (2 entradas) $ 0,00» y el TOTAL es igual que sin seguro; «Comprar» no muestra «Seguro de entrada» | PA-14 |
| 8.8 | El pago de esa compra deja pólizas a $ 0 | A | La confirmación dice «Seguro $ 0,00»; hay 2 filas en `Seguros` con `Precio = 0` | PA-15 |
| 8.9 | Sin suscripción, el seguro se cobra | A | «Seguro (2 entradas) $ 10.000,00» | PA-14 |
| 8.21 | RN-39 en pantalla, sin suscripción | A | El evento en preventa aparece en el listado; «Comprar» → aviso con la fecha y hora de fin, y vuelve al listado | PA-16 |
| 8.22 | Con suscripción, compra normal | A | Abre `FrmCompraCliente` del evento | PA-16 |
| 8.23 | Listas de espera del administrador | A | El combo trae «… · agotado»; columnas exactas; suscriptores arriba; contadores; `SortMode = NotSortable` | PA-18 |
| 8.24 | RN-35: el orden queda congelado al inscribirse | A | oro@ (suscripto) y bronce@; se cancela la suscripción de oro@ → oro@ sigue primero. **Es la primera prueba de RN-35**, ver la nota al final | PA-19 |
| 8.25 | Propietario sin eventos agotados | A | Combo deshabilitado y el aviso exacto | PA-18 |
| 8.26 | Ningún formulario abre una conexión | A | Vaga, ver 3.c. Se busca en el texto de `Visual/*.cs`: no hay `SqlConnection`, `SqlCommand` ni `ConexionSql`. Es como CP-53. La navegación completa ya la hizo JC-R11 el 05/11 | PA-20 |

### E. Preventa desde el propietario (C-9)

| ID | Qué verifica | Grupo | Control y aserción | Caso |
|---|---|---|---|---|
| C9.1 | El selector de preventa en Agregar evento | A | `ShowCheckBox` destildado, formato `dd/MM/yyyy HH:mm`; «Registrar» y «Volver» dentro del área cliente **al 100 %**; sin superposiciones. *(Corregido el 10/11: la variante al 150 % pasa a la prueba de disposición de 6B, P6B-1; ver hallazgo A-01)* | PA-04 |
| C9.2 | Evento con preventa: se guarda y se publica | A | `Eventos.FinPreventa` = mañana 20:00; `Estado = 'Publicado'` | PA-17 |
| C9.3 | Preventa posterior a la fecha del evento | A | Aviso exacto; sin fila nueva | PA-17 |
| C9.4 | = 8.21 con el evento creado en C9.2 | A | Ídem 8.21, con fecha «hasta el (mañana) a las 20:00» | PA-16 |
| C9.5 | = 8.22 | A | Ídem 8.22 | PA-16 |
| C9.6 | Administrar un evento con preventa | A | El selector aparece tildado con la fecha y hora guardadas | PA-17 |
| C9.7 | Quitar la preventa | A | Aviso «El evento quedó sin preventa…»; `FinPreventa` NULL | PA-17 |
| C9.8 | Sin preventa, compran los dos | A | Los dos clientes abren la compra | PA-16 |
| C9.9 | Administrar un evento sin preventa | A | Selector destildado | PA-17 |
| C9.10 | Pago de un suscriptor con seguro | A | `Compras.ConSeguro = 1`, `MontoSeguro = 0`; una póliza por entrada con `Precio = 0` | PA-15 |
| C9.11 | Pago sin seguro | A | `ConSeguro = 0`; ninguna póliza | PA-15 |

### F. Reportes

| ID | Qué verifica | Grupo | Control y aserción | Caso |
|---|---|---|---|---|
| 9.15 | Con la semilla sola, los cinco se generan y exportan | A | Cinco `Generar` sin excepción y cinco exportaciones a un exportador de prueba (como CP-38); el Festival con 0 entradas, 0,0 % y ticket «—» en la grilla | PA-21 |
| 9.1 | Menú del propietario | A | Cuatro botones con sus textos y «Volver al inicio», todos visibles y dentro del área cliente | PA-20 |
| 9.2 | Reportes al abrir | A | Combo con los cinco «RPT-0N · título»; período desde el día 1 de hace dos meses hasta hoy; con RPT-01, «Categoría» en «Todas» | PA-22 |
| 9.3 | Filtros según el reporte | A | Visibilidad de Evento, Corte y la casilla para cada uno; al cambiar, la grilla vacía y «Exportar PDF» deshabilitado | PA-22 |
| 9.4 | RPT-01 con `03-datos-reporte.sql` | A | Los valores de la página 100 (4.480; $ 86.710.000; ticket $ 19.355; 81,0 %); números alineados a la derecha; la fila TOTAL con fuente en negrita y su color de resaltado; `NotSortable`; `lblLectura` sin recorte | PA-23 |
| 9.5 | RPT-01 con «Concierto» | A | 2.480 entradas, $ 57.100.000 | PA-23 |
| 9.6 | RPT-02 | A | Cinco combinaciones; «Solo entrada» 60,0 %; «Entrada + Merchandising» 3,1 %; TOTAL $ 113.060.000 | PA-23 |
| 9.7 | RPT-02 de Duki | A | 1.500 unidades | PA-23 |
| 9.8 | RPT-03 con dos cortes | A | 818 / 260 / 100; con el corte 31/08/2025, todo en 0 y «—», sin error | PA-23 |
| 9.9 | RPT-04 | A | 504 · 18,0 %; 164 · 13,9 %; 51 · 4,3 %; 215 · 18,3 % | PA-23 |
| 9.10 | RPT-05 | A | Tres «Con cupo», 705 / 4.030 = 17,5 %; con «Solo agotados», solo el TOTAL en 0 | PA-23 |
| 9.11 | Exportar PDF | A + B | **A:** el aviso trae la ruta y el nombre `EVAU_RPT0N_20250901_20251130.pdf`. Se responde «No». **B:** que «Sí» lo abra en el visor de Windows (1 min, JC). Requiere un cambio en `Visual`, ver la tanda 7.5 | PA-24 |
| 9.12 | Contenido del PDF del reporte | A + B + C | **A:** el texto extraído (como CP-65) tiene «EventAura», el título, el código, el período, «Solicitado por: Ana Administradora (admin@…)» y «Página 1 de 1». **B:** el encabezado violeta, la tabla con encabezado gris y que el QR esté (2 min). **C:** escanear el QR con el celular → `EVAU|RPT-0N|…` (2 min, con JC-R10) | PA-25 |
| 9.13 | El mismo PDF en Edge, Chrome y Adobe Reader | **B** | Abrirlo en los tres: misma cantidad de páginas, mismo texto, encabezado violeta y QR visibles (6 min). **Requiere Adobe Reader instalado: dato de JC** | — |
| 9.14 | Exportar con el PDF abierto en Adobe | A | No hace falta Adobe: la prueba abre el archivo de destino con `FileShare.None`, que es lo que hace Adobe, y exporta → «No se pudo escribir el archivo…» y la pantalla sigue abierta. Requiere el mismo cambio que 9.11 | PA-24 |
| 9.16 | El cliente no tiene reportes | A | `frmMenuCliente`: ningún control con «Reporte» en el texto | PA-20 |
| 9.17 | En Agregar evento, «Registrar» y «Volver» se ven | A | Dentro del área cliente al 100 %. La variante al 150 % pasa a P6B-1, en 6B (hallazgo A-01) | PA-04 |

### G. Cierre

| ID | Qué verifica | Grupo | Por qué | Caso |
|---|---|---|---|---|
| 10.1 | La tarea «Probar» compila y da «12 correctas» | **X** | Escrita para 12 pruebas: hoy son 77. Cada corrida de la batería es la evidencia, y la de 6A está en el reporte de Codex. **Además, el «queda sin datos de prueba» es falso a propósito:** la base de pruebas queda creada para poder mirarla (`BaseDePrueba.cs`). Se reescribe, ver 3.c | — |
| 10.2 | La batería no toca la base de desarrollo | **AH** | Script en `tools/`: huella de `BddEventAura` (cuenta de filas por tabla, con `AuditarBaseJc.sql`) antes y después de la batería → iguales | AH-1 |
| 10.3 | La batería pasa con la base de pruebas abierta en otra conexión | **AH** | Script en `tools/`: abre una conexión a `BddEventAura_Pruebas`, corre la batería y comprueba que pasa (la preparación corta las conexiones con `SINGLE_USER WITH ROLLBACK IMMEDIATE`) | AH-2 |
| 10.4 | Menú del cliente y Administrar evento «se ven igual que antes de borrar campos muertos» | **X** | Las dos pantallas se rediseñan en 6B: «igual que antes» ya no tiene sentido. Lo reemplaza la aprobación de capturas de 6B | — |

### Totales

| Grupo | Filas | Quién | Costo |
|---|---|---|---|
| A (con alguna parte B o C en 9.11 y 9.12) | **57** | Codex, en 25 casos nuevos (PA-01 a PA-25) | 19 a 27 h de Codex (`TANDAS-PRUEBAS-64.md`) |
| A0, ya cubiertas | 2 | — | 0 |
| AH, herramienta | 2 | Claude Code | 1 h |
| B (solo persona) | 1 | JC | 6 min |
| X, sin objeto | 2 | — | 0 |
| **Total** | **64** | | |

**Cuánto costaría hacerlas a mano.** Las 57 de A llevarían unos **4 a 5 horas de JC**:
- unos 4 minutos cada una;
- más una hora para preparar la base (cargar `03-datos-reporte.sql`, los `UPDATE` de preventa
  y de bloqueo, y restaurarla después).

**Automatizarlas cuesta 2,5 a 3,5 días de Codex**, y la diferencia está en la evidencia:
- **una vez automatizadas, se repiten en cada corrida**, también durante 6B, que rediseña
  justo las pantallas del propietario que cubren;
- **hechas a mano, valen el día en que se hicieron.**

Si falta tiempo, la tanda que conviene pasar a mano es la de reportes (7.5): ver la Parte 4
en el reporte de esta sesión.

**Nota sobre RN-35 (8.24).** La redacción nueva de RN-35 sigue esperando la confirmación de
Máximo (`ESTADO-AGENTES.md`). La prueba comprueba **lo que el sistema hace hoy**: el
suscriptor queda primero aunque después cancele. Si Máximo dijera que «estrictamente por
orden de inscripción» era deliberado, cambiaría el código y esta prueba. Con PA-19, la
cobertura sube a **34 de 38 reglas (89,5 %)**.

---

## 4. La lista de JC (grupos B y C)

| # | Qué | Grupo | Minutos | Cuándo |
|---|---|---|---|---|
| B-0 | Mirar las capturas de las pantallas del propietario después de 6B. Cubre el «se ve bien» de 8.3, 8.23, 9.1, 9.4, C.12, C9.1 y 9.17 | B | 10 | Ya está dentro de la aprobación de 6B |
| 9.11b | «Sí» abre el PDF del reporte en el visor de Windows | B | 1 | Con la tanda 7.5 hecha |
| 9.12b | Mirar el PDF: encabezado violeta, encabezado de la tabla gris y QR presente | B | 2 | Ídem |
| 9.12c | Escanear el QR del PDF del reporte con el celular → `EVAU|RPT-0N|…` | C | 2 | **En la misma sesión que JC-R10** |
| 9.13 | El mismo PDF en Edge, Chrome y Adobe Reader | B | 6 | Ídem. Si Adobe no está instalado, 5 min más para instalarlo |
| | **Subtotal de las 64** | | **21** (11 sin B-0) | |
| JC-R10 | Escanear dos tickets nuevos con el celular (ya era tuya) | C | 10 | Cuando avise Claude Code |
| | **Total** | | **31 min** | |

Claude Code te deja el PDF del reporte ya generado, con los datos de `03-datos-reporte.sql`
y en una carpeta temporal, para que solo tengas que abrirlo.

---

## 5. Pruebas mal redactadas y su versión falsable

| ID | Hoy dice | Problema | Versión falsable |
|---|---|---|---|
| 3.2 | «Volver a entrar con la misma contraseña» → «Funciona contra el hash ya migrado» | «Migrado» no dice desde qué. Si la migración desde texto plano ya no existe en el código, no hay nada que migrar. **Inconcluso:** Codex revisa si `GestorUsuarios` todavía tiene esa ruta | admin@ inicia sesión dos veces seguidas con `Admin1234`: las dos devuelven el usuario. La contraseña guardada en `Usuario` no es «Admin1234». Si existe la ruta de migración: un usuario con contraseña en texto plano entra, y después de entrar su contraseña guardada ya no es texto plano |
| C.3 | «Indica los minutos que faltan, que bajan con el tiempo» | Depende de esperar | Con `BloqueadoHasta` = ahora + 10 min, el aviso contiene «10 minutos»; con ahora + 5 min, «5 minutos» |
| 4.6 | «Funciona desde la interfaz» | No dice qué es funcionar | Registrar «Local de prueba», «Mitre», 123, capacidad 500 → aviso de alta; en `frmMisLocales` aparece una fila con «Local de prueba» y 500; en `Locales` hay una fila nueva del propietario |
| C.12 | «Filas alternadas en gris» | Con el tema cambió el color | Las filas alternas tienen un fondo distinto del de las demás filas. Que ese fondo sea `Tema.FilaAlterna` lo exige 6B (CP-63 extendida) |
| 8.3 | «Nada cortado» | No es medible así | Cada `Label` con `AutoSize = false` cumple: alto preferido para su ancho ≤ alto; ningún par de controles hermanos se superpone |
| 8.26 | «Todo funciona igual que antes sin `ConexionSql`» | «Igual que antes» no tiene referencia | Ningún archivo de `Visual/` contiene `SqlConnection`, `SqlCommand` ni `ConexionSql`; ningún formulario tiene un constructor con parámetros de esos tipos. La navegación por todas las pantallas ya la recorrió JC-R11 |
| 9.1 | «En la pantalla habitual» … «visible y en uso» | «Habitual» no es un tamaño | Al 100 % y a la escala simulada de CP-25 (1366 × 728 al 150 %), los cuatro botones y «Volver al inicio» quedan dentro del área cliente, y «Volver al inicio» vuelve al login (CP-46) |
| 9.4 | «La tabla de la página 100 del anexo» … «la lectura debajo, sin cortarse» | Remite a otro documento; «sin cortarse» no es medible | Los valores escritos en la fila 9.4 de la sección 3; la lectura con alto preferido ≤ alto |
| 9.13 | «Se ve igual en los tres» | «Igual» no dice en qué | En Edge, Chrome y Adobe Reader: una página, el mismo texto que extrae la prueba, el encabezado violeta y el QR visibles |
| 9.17 | «784 de alto … justo en el límite» | El alto cambia en 6B | «Registrar» y «Volver» quedan dentro del área cliente al 100 %; al 150 %, en P6B-1 de 6B |
| 10.1 | «Muestra las 12 pruebas "Correctas"; queda sin datos de prueba» | Cifra vieja; la segunda mitad es falsa a propósito | `tools\Proyecto.ps1 -Accion Probar` termina con todas las ejecuciones correctas y la cantidad esperada. La base de pruebas queda creada, para poder revisarla |
| 10.4 | «Se ven igual que antes» | Las pantallas se rediseñan | Sin objeto: la reemplaza la aprobación de capturas de 6B |

**Las demás están bien escritas**: dicen el control, la entrada y el valor esperado. Las
aserciones de la sección 3 las reproducen.

---

## 6. La consecuencia en el documento

**Qué dice hoy el documento (v2.4) sobre estas 64: nada.** Lo busqué en el texto de las 112
páginas (manual, ejecutad, automatiz, prueba de interfaz y otros términos):
- no menciona pruebas manuales, ni las 129, ni quién las ejecutaría;
- sobre pruebas, solo habla de CP-01 a CP-10 (página 68) y de la cobertura (páginas 108 y 110:
  «22,2 %», «carecen de verificación automatizada»).

**El documento no afirma nada falso sobre estas pruebas.** Lo que dice «las ejecuta Máximo»
está en el repositorio:
- `COMO-SE-PROBO.md`, secciones 1, 3 y 5, que además es el guion de la defensa;
- `PRUEBAS-PENDIENTES.md`, encabezado;
- `ESTADO-AGENTES.md`.

Esos archivos los corrijo yo cuando las tandas se cierren. Corregirlos antes sería anotar
como hecho algo que todavía no pasó.

**Lo que hay que agregar o corregir en el documento** está en `CORRECCIONES-DOCUMENTO.md`,
**ítem 23**:
- las páginas 6, 7, 8, 108, 109 y 110, con las cifras finales;
- un párrafo nuevo en la página 108 que cuenta de dónde salieron los casos.

La redacción dice qué hizo una persona y qué una prueba repetible, sin decir nunca «se
ejecutaron manualmente».
