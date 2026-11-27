# Cierre del documento: inventario, secciones estables y lo que depende del código final

Claude Code, 24/11/2026, a pedido de JC. Prepara el cierre para que sea una tarde: separa lo que ya
está terminado de lo que espera al código final, y deja cada valor que depende del código como un
marcador `[[...]]` en `docs/LISTA-PARA-MAXIMO.md`.

**No cambia ninguna cifra de la batería.** Los valores provisorios son los de `2db272e` (24/11). El
código ya avanzó: después de `2db272e` hay diez commits de interfaz (reforma visual de las 25
pantallas y migración de cuatro vistas a WebView2) y Codex sigue trabajando.

Herramienta: `py -3 -X utf8 tools/Cierre/Inventario.py --resumen` lista, por página, lo que falta.
El día del cierre tiene que dar cero cifras escritas a mano y cero marcadores.

---

## 0. El cierre de un vistazo

| # | Trabajo | Quién | Tamaño | Depende de |
|---|---|---|---|---|
| 1 | Congelar el código y correr la batería completa sobre ese commit, **dos veces** (decisión de JC del 24/11) | Codex (primera corrida), Claude Code (segunda) y JC (portón) | 2 corridas | Que Codex termine |
| 2 | Calcular los valores de los 19 marcadores y regenerar las tres tablas | Claude Code | ~6 vueltas | 1 |
| 3 | Rehacer las capturas (instructivo, guion, anexo: unas 60 imágenes) | Codex, o Claude Code con `tools/CapturasInstructivo/` si Codex no está trabajando | 1 corrida + revisión | 1 |
| 4 | Revisar las páginas de pantallas contra la interfaz final (sección 3.1) | Claude Code | ~15 vueltas: es lo más largo | 1 y 3 |
| 5 | Regenerar los diagramas de clases 4, 5, 7 y 8d (sección 3.3) | Claude Code | ~4 vueltas | 1 |
| 6 | Agregar WebView2 a la arquitectura (sección 4) | Claude Code | ~2 vueltas | pp. 18 y 24 ya en la lista (opción (a), decidida el 24/11); p. 57 y p. 112, con el código final |
| 7 | Completar los marcadores en Word, pegar lo que estaba en pausa, regenerar el índice | Máximo | 1 tarde | 2 a 6 y los datos de JC |

Lo que sigue dependiendo de personas: los datos de JC (sección 5) y la tarde de Máximo en Word.

**Decisiones de JC del 24/11, ya aplicadas en la lista:** «Máximo» con tilde; los años 2025 pasan a
2026 (pp. 1, 6, 9, 44, 112 y pie); en las pp. 18 y 24 va la opción (a) de la sección 4; el hueco
CP-212 a CP-353 se deja como está; la batería final se corre dos veces. El seguimiento de todo lo
pendiente está en `docs/TICKETS.md`.

---

## 1. Inventario: cada valor del documento que depende del código

Sale de `tools/Cierre/Inventario.py`. Las páginas son las de la versión 2.4 (`Documentacion/EventAura_v2.pdf`),
las mismas que usa la lista.

### 1.1 Marcadores en el texto (19)

| Marcador | Página y lugar | Valor provisorio (2db272e) | Cómo se obtiene el final |
|---|---|---|---|
| `[[COMMIT-COMPLETO]]` | p. 1, pie de la carátula | `2db272ecf0f31898ee617e025ba763be1be7825e` | `git rev-parse HEAD` del commit congelado |
| `[[CASOS-RANGO-2.6]]` | p. 6, fila 2.6 del historial | CP-62 a CP-211 y CP-354 a CP-357 | `tools/Matriz/Extraer.py <commit>`, línea «Casos citados» |
| `[[CASOS-LETRAS]]` | p. 7, tabla «Qué contiene este informe», fila Verificación | doscientos catorce | Ídem, en letras |
| `[[CASOS-LETRAS-INICIAL]]` | p. 7, cuarto punto de «Resultados principales» | Doscientos catorce | Ídem, con mayúscula |
| `[[CASOS-ESCALA]]` | p. 37, priorización de RNF, fila RNF-03 | CP-12, CP-25, CP-64, CP-69 y CP-85 | Casos que verifican tamaño de pantalla y escala, leídos uno por uno (hoy se sumarían CP-116, CP-209, CP-210 y CP-357) |
| `[[CASOS-LETRAS]]` | p. 106, criterio de construcción | doscientos catorce | Ídem |
| `[[CASOS]]` | p. 106, nomenclatura, fila Casos de prueba | 214 | Ídem, en número |
| `[[ULTIMO-CASO]]` | p. 106, nomenclatura | CP-357 | Ídem |
| `[[NUMEROS-SIN-USO]]` | p. 106, nomenclatura | los números CP-149 y CP-212 a CP-353 no se usaron | Línea «Números sin uso» de `Extraer.py` |
| `[[BLOQUE-MATRIZ]]` | p. 107, renglón sobre la matriz | — | Se borra al rehacer la tabla (1.2) |
| `[[CASOS-RNF]]` | p. 107, párrafo bajo la matriz | RNF-02: 19 casos; RNF-03: 10; RNF-04: 3 | Filas RNF de `Extraer.py`, redactadas como hoy |
| `[[BLOQUE-CASOS-RN]]` | p. 108, renglón sobre la tabla caso → reglas | — | Se borra al rehacer las filas (1.2) |
| `[[CASOS-LETRAS]]` | p. 108, párrafo de cobertura | doscientos catorce | Ídem |
| `[[ULTIMO-CASO]]` | p. 108, título de la tabla 2.6 | CP-357 | Ídem |
| `[[BLOQUE-CASOS-2.6]]` | p. 108, renglón sobre la tabla 2.6 | — | Se borra al rehacer las filas (1.2) |
| `[[RANGO-CASOS-FINALES]]` | p. 108, «Origen de los casos» | CP-207 a CP-211 y CP-354 a CP-357 | Casos agregados desde `967e038`. **Si hay casos nuevos, revisar también la oración que los describe** |
| `[[NUMEROS-SIN-USO-ORACION]]` | p. 108, «Origen de los casos» | Los números CP-149 y CP-212 a CP-353 no se usaron. | Ídem, como oración. Si solo queda CP-149: «El número CP-149 no se usó.» |
| `[[ULTIMO-CASO]]` | p. 109, brecha G-02 | CP-357 | Ídem. **Decía «CP-206»: había quedado viejo el 24/11** y se corrigió con el marcador |
| `[[CASOS-LETRAS]]` y `[[EJECUCIONES-LETRAS]]` | Sección «Metodología de verificación», «La batería de pruebas» | doscientos catorce; trescientas sesenta | Batería del commit congelado: total de ejecuciones |

La tabla de la lista («Cifras del documento › Marcadores de cierre») tiene una columna «Final» que
se completa ese día, y es la que se le pasa a Máximo.

### 1.2 Bloques que se rehacen enteros (3)

| Bloque | Página | Tamaño hoy | Cómo se rehace |
|---|---|---|---|
| Matriz principal | p. 107 | 22 filas (RF-01 a RF-22) | `py -3 -X utf8 tools/Matriz/Extraer.py <commit>`: tabla final, tal cual |
| Tabla caso → reglas | p. 108 | CP-62 en adelante | Filas nuevas o cambiadas desde `2db272e`. Comparar con el script de comparación del 24/11 (catálogo contra comentarios) |
| Tabla «Casos incorporados en la versión 2.6» | p. 108 | CP-62 en adelante | Ídem, con condición y resultado leídos de cada prueba |

La tabla 2.5 (CP-11 a CP-61, p. 108) **no** está marcada: cambia solo si Codex vuelve a retocar el
comentario de alguno de esos casos. Se comprueba con la misma comparación.

### 1.3 Cobertura de reglas (11 lugares; hoy no cambia)

«50 de 52», «96,2 %», «cincuenta de las cincuenta y dos»: pp. 7, 8, 108 (cuatro veces), 109 (G-02),
la sección «Metodología» (dos veces) y la 110. No llevan marcador porque solo cambian si una prueba
nueva cubre RN-07 o RN-33. En el cierre se comprueba con `Extraer.py`; si cambia, son estos once
lugares, más la lista de reglas «Son RN-01 a RN-06, RN-08 a RN-26…» de la p. 108.

### 1.4 Referencias a pantallas (páginas en pausa ⏸)

| Páginas | Qué | Tamaño en la lista |
|---|---|---|
| p. 45 | Prototipos | Texto con las referencias a la interfaz |
| pp. 47 a 49 | Guion de interfaz y sus tres capturas | 3 figuras |
| pp. 51 a 53 y 55 | Instructivo: controles, recorrido, figuras 1 a 4, mensajes | 17 referencias a figuras, 10 tablas de controles |
| pp. 72 a 85 y nuevas | Especificaciones CU-01 a CU-21 | 758 renglones; 88 nombres de botones, menús o pantallas entre comillas |
| pp. 90 a 97 | Párrafo «El estudio y el rediseño de la interfaz» | 1 párrafo |
| Después de la p. 105 | Anexo «Pantallas del sistema» | 27 pantallas, 56 figuras |
| `docs/capturas/` | Imágenes de todo lo anterior | 60 archivos |

### 1.5 Fuera del Word (los actualiza Claude Code, no Máximo)

- `docs/04-casos-prueba.md`: encabezado, tabla de ejecuciones por caso y registro de cada caso nuevo
  (definición de terminado, punto 7).
- `docs/METODOLOGIA-VERIFICACION.md`: tabla «Cifras definitivas».
- Notas *Razón:* y *Cómo se armó:* de la lista que nombran `2db272e` (pp. 1 y 107): `--notas` las
  lista.

### 1.6 Si Máximo ya pegó números en lugar de marcadores

Buscar en Word, además de `[[`: «214», «doscientos catorce», «trescientas sesenta», «2db272e»,
«CP-357», y los anteriores «205», «doscientos cinco», «trescientas cuarenta y ocho», «967e038»,
«CP-206». Cada aparición es uno de los lugares de 1.1.

---

## 2. Secciones estables: terminadas y revisadas el 24/11

Revisadas contra el código de `HEAD` (`4c200ef`) y, para la lógica de negocio, contra los cambios en
`Repo/`, `Datos/`, `Entidade/` y `BaseDatos/` desde `967e038`, el último código contra el que se
leyeron prueba por prueba. Desde entonces el negocio cambió solo en dos consultas nuevas
(`GestorEventos`, `GestorListaEspera`) y en la elección del enviador de códigos; ninguna cambia
una regla.

| Páginas | Contenido | Estado |
|---|---|---|
| 1 | Carátula | Estable, salvo `[[COMMIT-COMPLETO]]` y los datos de JC |
| 6 | Historial | Estable, salvo `[[CASOS-RANGO-2.6]]` y las dos fechas de JC |
| 7 y 8 | Resumen ejecutivo | Estable, salvo dos marcadores |
| 9 a 21 | Definición, objetivos, interesados, cronograma, criterios, supuestos | Estable (datos de JC: período de la p. 9 y nueve fechas del cronograma) |
| 23 | Requerimientos funcionales | Estable |
| 18 y 24 | Compatibilidad técnica (criterio 3, RNF-03) | **Estable, pero pendiente la decisión sobre WebView2** (sección 4) |
| 25 a 27, 32 y 33 | Reglas de negocio, niveles, parámetros y conflictos | Estable |
| 28 y 29 | Factibilidad económica y costos | Estable |
| 35 a 38 | Priorización y orden de implementación | Estable, salvo `[[CASOS-ESCALA]]` (p. 37) |
| 39 a 44 | Minutas y descripción funcional | Estable |
| 50 y 54 | Estado de implementación del instructivo y ejemplo resuelto | Estable |
| 57 a 61 | Arquitectura e implementación | **Corregido hoy** el párrafo de Strategy (ver abajo). Pendiente WebView2 (sección 4) |
| 62 a 67 | Entidades y código | Estable. El código de las pp. 63 a 67 es el pseudocódigo del documento, con sus propios nombres; la lógica coincide con `GestorFidelizacion` actual (descuento, ventana, umbrales, cargo y seguro). El SQL de la p. 63 está en el código tal cual |
| 68 a 70 | Pruebas de la lógica de descuento (CP-01 a CP-10) | Estable |
| 71 | Diagramas de casos de uso | Estable |
| 86 a 89 | Dominio, estados, robustez, actividad, entidad-relación | Estable. **Los diagramas de clases no**: sección 3.3 |
| 98 a 105 | Anexo A02 | Estable. La consulta de RPT-01 coincide con `Datos/RepositorioReportes.cs` (cambia solo el corte de renglones) |
| 106 a 110 | Matriz, cobertura, brechas, metodología, conclusiones | Estable, salvo marcadores y bloques |
| 111 y 112 | Trabajo futuro y bibliografía | Estable, salvo la entrada de WebView2 (sección 4) |

**Corregido hoy en secciones estables:**
- **p. 57, Strategy:** decía que, con el correo real configurado, todos los códigos salen por correo
  real. Desde `db02dd4` (22/11, incluido en `2db272e`), el código del teléfono de la solicitud para
  ser Organizador va siempre por el canal simulado (`SeleccionEnviadorCodigos.Para`). Se agregó la
  oración, con aviso «Cambio del 24/11» para Máximo.
- **p. 109, G-02:** decía «CP-11 a CP-206»; ahora lleva `[[ULTIMO-CASO]]`.

**Lo que «estable» supone:** que Codex no toque `Repo/`, `Datos/`, `Entidade/` ni `BaseDatos/`
(salvo semillas). En el cierre se comprueba con
`git diff --stat 4c200ef <commit> -- Repo Datos Entidade BaseDatos`: si no está vacío, se lee
cada cambio antes de dar estas secciones por cerradas.

---

## 3. Secciones que dependen del código final

### 3.1 Pantallas (pausadas con ⏸ en la lista)

Las del cuadro de 1.4. Ya tienen la estructura (qué reemplaza a qué, tablas de controles, ubicación
de cada figura); lo que falta es leerlas contra la interfaz final: nombres de botones y menús,
controles de cada tabla, textos de los mensajes y las figuras. Además de la reforma visual, cuatro
vistas pasaron a WebView2: el catálogo (`frmEventosCliente`), el resumen de la compra
(`FrmCompraCliente`), los reportes (`frmReportes`) y el panel del Administrador
(`PanelAdministrador`). En esas, las tablas de controles del instructivo y del anexo ya no
describen controles de Windows Forms sino una vista web: hay que redactarlas de nuevo.

### 3.2 Catálogo de casos y cifras de la batería

Marcadores de 1.1 y bloques de 1.2.

### 3.3 Diagramas de clases (no estaban en la lista de dependientes, pero lo son)

Declaran «operaciones públicas» de cada clase y se regeneraron por última vez el 19/11. Desde
entonces:

| Diagrama | Qué le falta |
|---|---|
| Vista 5, negocio de administración (`clases-5-negocio-administracion.puml`) | `GestorEventos.VendidasDeEvento(int) : int` |
| Vista 4, negocio de la compra (`clases-4-negocio-compra.puml`) | `GestorListaEspera.EventosEnEspera(int) : List<string>` |
| Vista 7, datos (`clases-7-datos.puml`) | `RepositorioEntradas.EntradasVendidas(int)`, `RepositorioListaEspera.EventosEnEspera(int)` |
| Vista 8d, Strategy de la cuenta (`clases-8d-patron-strategy-cuenta.puml`) | `SeleccionEnviadorCodigos.Para(proposito)`; `GestorCodigosCorreo` ya no usa solo `Actual` |

**Hecho el 27/11 contra `9295e3c`:** vista 4 suma `GestorListaEspera.EventosEnEspera(int)` y `GeneradorQR.Imagen(string)`; vista 5, `GestorEventos.EntradasVendidas(int)` (el nombre real; arriba decía `VendidasDeEvento`); vista 8d, `SeleccionEnviadorCodigos.Para(string)`, la dependencia rotulada «Para(propósito)» y la nota de `EnviadorSmtp` (Program la elige si están las variables de SMTP). **La vista 7 no cambia:** muestra solo los métodos que aparecen en las secuencias, y los dos de la tabla no aparecen. 8d pasa de 13,9 a 13,7 cm. Si Codex cambia `Repo/` antes de la entrega, se repite con `tools/Diagramas/Renderizar.py`.

Se regeneran con el código final (fuentes en `docs/diagramas/fuentes/`, render con
`py -3 tools/Diagramas/Renderizar.py`). En ese momento también se revisan la vista 6 (llamadas de
autorización de los métodos nuevos) y las tres secuencias de la compra, porque `FrmCompraCliente`
cambió con WebView2.

### 3.4 Anexos de capturas

`docs/capturas/` (instructivo, escalado, pantallas): se rehacen sobre la interfaz final, después de
congelar el código.

---

## 4. WebView2 y la arquitectura

### Qué cambió

Son **cuatro** vistas, no tres. Una clase de presentación nueva, `Visual/Web/PanelWeb`, aloja un
control WebView2 que carga páginas HTML, CSS y JavaScript de la carpeta `Web` del programa. La
pantalla le pasa a la página los datos ya obtenidos del negocio, serializados como JSON
(`Newtonsoft.Json`). La página solo los dibuja y devuelve mensajes (por ejemplo, «se eligió el
evento 12»), que atiende la pantalla en C#.

### Por qué no cambian las capas

- La página **no** llega al negocio ni a los datos: no se le expone ningún objeto de la aplicación
  (`AreHostObjectsAllowed = false`), solo recibe JSON.
- `Visual/Web/DatosWebReportes.cs` obtiene sus datos de `GestorReportes`, no de la base.
- No hay acceso a la red: solo se navega al origen local `eventaura.local`, y toda otra petición
  devuelve 403. Tampoco hay descargas, ventanas nuevas ni permisos.
- Si el equipo no tiene el componente WebView2, la vista se desactiva y la pantalla usa su versión
  en Windows Forms. Esa versión sigue en el código, con un aviso en las cuatro pantallas (auditoría
  del 24/11: `docs/AUDITORIA-RESPALDO-WEBVIEW2.md`).

La regla «Visual nunca abre una conexión ni escribe SQL» se mantiene. Las cuatro capas, sus
dependencias y los patrones descriptos no cambian.

### Qué hay que agregar al documento, y dónde

**Borrador adelantado el 27/11, contra `9295e3c` (reemplaza a los textos 1 y 2 de abajo cuando se
congele el código; verificado en `Visual/Web/PanelWebMotor.cs`, líneas 71 a 87):** hoy no son
cuatro sino **15 pantallas** con vista web, servidas por 11 páginas de `Visual/Web/`: bienvenida,
acceso, alta de cuenta, recuperación y cambio de contraseña, catálogo, armador del paquete, mis
entradas, suscripción, reportes, panel del Administrador, auditoría, revisión de eventos, mis locales,
eventos del local y control de ingreso. **El Pago pasa a web (encargo de Codex):** serían 16.

1. Fila Presentación, columna Responsabilidad:
   > Veinticinco pantallas. [[VISTAS_WEB]] de ellas muestran su contenido como una página web local
   > dentro de un control WebView2. No abre conexiones ni contiene SQL: solo invoca a la capa de
   > negocio.
2. Párrafo después de la tabla:
   > **Vistas web locales.** Las vistas web son páginas HTML, CSS y JavaScript que se instalan con la
   > aplicación y se cargan desde la carpeta del programa, con la dirección local `eventaura.local`.
   > La pantalla obtiene los datos de la capa de negocio y se los pasa a la página ya calculados; la
   > página solo los dibuja y le avisa a la pantalla lo que elige el usuario. El control no navega
   > fuera de esas páginas: cualquier otra dirección recibe un rechazo. Tampoco abre ventanas nuevas,
   > descarga archivos, concede permisos ni expone objetos de la aplicación al código de la página.
   > Si el equipo no tiene el componente WebView2, cada pantalla usa su versión en Windows Forms.


Se aplica en el cierre, porque Codex todavía puede migrar más vistas: la cantidad («cuatro») es
del código final.

1. **p. 57, tabla de capas, fila Presentación.** Columna Proyecto: «Visual (Windows Forms y
   WebView2)». Columna Responsabilidad:
   > Veinticinco pantallas. Cuatro de ellas (el catálogo, el resumen de la compra, los reportes y el
   > panel del Administrador) muestran su contenido como una página web local dentro de un control
   > WebView2. No abre conexiones ni contiene SQL: solo invoca a la capa de negocio.

2. **p. 57, párrafo nuevo después de la tabla de capas:**
   > **Vistas web locales.** Las cuatro vistas web son páginas HTML, CSS y JavaScript que se
   > instalan con la aplicación y se cargan desde la carpeta del programa. La pantalla obtiene los
   > datos de la capa de negocio y se los pasa a la página ya calculados; la página solo los dibuja
   > y le avisa a la pantalla lo que elige el usuario. El control no navega fuera de esas páginas,
   > no accede a la red, no descarga archivos y no expone objetos de la aplicación al código de la
   > página. Si el equipo no tiene el componente WebView2, cada una de esas pantallas usa su versión
   > en Windows Forms.

3. **pp. 18 (criterio 3) y 24 (RNF-03): las dos dicen «sin otro software que el motor de base de
   datos».** **Decidido por JC el 24/11: opción (a)**, ya aplicada en la lista. Las dos opciones eran:
   - **(a) Mantener la frase y aclarar.** Agregar a las dos: «Las vistas web usan el componente
     WebView2, que viene incluido en Windows 11 y en las instalaciones actualizadas de Windows 10; si
     falta, la aplicación usa sus pantallas de Windows Forms.» Es fiel al código: hay versión de
     respaldo en las cuatro.
   - **(b) Declararlo como requisito.** «…sin otro software que el motor de base de datos y el
     componente WebView2 de Microsoft (incluido en Windows 11).» Es más simple, pero sobredeclara:
     la aplicación funciona sin el componente.

   **Lo que respalda (a):** Codex probó el respaldo una vez, el 24/11 a las 11:36, con una ubicación
   de runtime inválida (capturas en `TestResults/WebView2-Respaldo/`, solo a 96 DPI y con la base
   de pruebas): las cuatro pantallas muestran su versión de Windows Forms completa y el aviso. Lo
   que falta comprobar está en `docs/AUDITORIA-RESPALDO-WEBVIEW2.md` y en los tickets T-02 a T-04.

4. **p. 112, bibliografía**, con el formato de las entradas de Microsoft que ya están:
   > Microsoft. (2026). WebView2 — Documentación del control para mostrar contenido web en
   > aplicaciones de escritorio.

5. **Dependencias declaradas.** El documento no tiene una lista de paquetes; la única mención
   técnica es la p. 57 (iText, en el Strategy de reportes). Con los puntos 1 y 2 alcanza. Para el
   repositorio, el paquete es `Microsoft.Web.WebView2` 1.0.2903.40 (`Visual/packages.config`), y
   `Newtonsoft.Json` 13.0.1 pasa a usarse directamente en `Visual`. Conviene sumar WebView2 a la
   tabla «Stack» de `CLAUDE.md` (zona de Claude Code), si JC lo aprueba.

No hace falta cambiar ningún diagrama por WebView2: ninguno dibuja el interior de la capa de
presentación, salvo la vista 8b (Strategy de reportes, `frmReportes` → `ExportadorPDF`), que no
cambió.

---

## 5. Qué más falta, además de los datos que se le pidieron a JC

Los ocho datos pedidos: comisión, turno, dos legajos, año de entrega, fechas de las versiones 2.5 y
2.6 y las nueve fechas del cronograma.

**Datos de JC que no estaban en esa lista:**
1. ~~«Maximo» o «Máximo»~~ **Decidido (24/11): «Máximo».** Aplicado en la lista (p. 1).
2. ~~Los años que hoy dicen 2025~~ **Decidido (24/11): 2026.** Aplicado en la lista: pp. 1, 6, 9,
   44, 112 y pie.
3. ~~Redacción de WebView2 en las pp. 18 y 24~~ **Decidido (24/11): opción (a).** Aplicada en la
   lista.
4. ~~Numeración de los casos~~ **Decidido (24/11): el hueco CP-212 a CP-353 queda.**
   `[[NUMEROS-SIN-USO]]` se completa como está.
5. **Filas del catálogo con reglas que no coinciden con la prueba.** Corrección del 24/11: eran
   **dos**, no siete. CP-44 (el catálogo no cita RN-05; la prueba sí) y CP-55 en la tabla de la
   p. 108 (dice RF-01, RF-02; la prueba, RF-01). Las otras cinco salían de un error del script de
   comparación, que leía «RN-04/40» como solo RN-04 y tomaba las reglas mencionadas en el
   resultado esperado. Ticket T-07.
6. ~~Cuántas corridas de la batería final~~ **Decidido (24/11): dos.**

**Trabajo que no es de JC:**

7. **Congelar el código.** Codex confirma que terminó, se commitea su trabajo en curso (hoy hay ocho
   archivos sin commitear en `Visual/` y `Pruebas/`) y se corre la batería completa.
8. **Lo de las secciones 3.1, 3.3 y 3.4:** pantallas, diagramas de clases y capturas.
9. ~~`docs/ESTADO-AGENTES.md` desactualizado desde el 10/11~~ **Actualizado el 24/11.**
10. **Pruebas manuales pendientes** de `docs/PRUEBAS-PENDIENTES.md`. La última que estaba abierta
    era JC-R10 (escanear dos tickets con el celular); hay que confirmar si se hizo con el ticket
    actual.
11. **En Word (Máximo), al final:** completar los marcadores, comprobar que buscar `[[` dé cero,
    regenerar el índice, corregir las páginas que se citan a mano y revisar los pies. La lista ya
    lo tiene en «Hacé esto último», con el paso de los marcadores agregado hoy.
