# Tickets: lo pendiente hasta la entrega

**Lo escribe solo Claude Code. Codex lo lee y no lo modifica**: informa el avance en su reporte
final, JC se lo pasa a Claude Code y Claude Code actualiza el estado acá. JC puede corregir
cualquier ticket cuando quiera.

Última actualización: **26/11/2026, a la noche**, por Claude Code.
- Estados puestos al día con los commits `4c200ef` a `0a2d786`.
- Agregados T-27 a T-35: el encargo de las vistas web del 26/11 y la auditoría visual.
- **Verificación sobre el ejecutable del 26/11** (`docs/VERIFICACION-EJECUTABLE-2026-11-26.md`): cierran
  T-03, T-21, T-23, T-24 y T-25; se reabren T-22, T-26 y T-31; T-02 sigue abierto. Nuevos: T-36 y T-37.

**Fechas:** congelamiento del código el **01/12**; entrega el **10/12**. El alcance del documento no
se achica. Lo que tiene que estar cerrado para congelar está en `docs/CONGELAMIENTO-2026-12-01.md`.

**«Verificado»** quiere decir ejecutado por Claude Code sobre la aplicación compilada; no leído en el
código ni informado por Codex.

**Dueño:** quien hace el trabajo. Casi todos son de Codex o de Claude Code. Figuran JC o Máximo
solo cuando el ticket es una prueba manual, un dato o el Word, que no puede hacer un agente.

**Estados:**

| Estado | Qué significa |
|---|---|
| **abierto** | Nadie lo empezó |
| **en curso** | Alguien lo está haciendo |
| **bloqueado** | Espera a otro ticket o a una persona; se indica a cuál |
| **en revisión** | Hecho; falta que JC lo revise o autorice el commit |
| **cerrado** | Cumple el criterio de «Listo cuando» |

**Orden:**
1. Antes de congelar el código, las fallas: T-26 (la compra desde el menú), T-31 (CP-359), T-28 (el
   historial), T-22 (la exportación) y T-36 (el alto de Reportes). Después, T-27, T-29, T-02, T-05 y
   T-37. T-32 solo si todo eso cierra antes del 29/11.
2. T-06 congela el código.
3. De T-08 en adelante, con el código congelado.

---

## Resumen

| ID | Dueño | Qué | Estado |
|---|---|---|---|
| T-01 | Codex | Terminar y commitear la tanda visual en curso (gráficos y disponibilidad) | cerrado: `ff79658` (24/11) |
| T-02 | Codex | Prueba automática del respaldo de WebView2 | abierto: ejecutado el 26/11, CP-359 falla (ver T-31); falta el parámetro de capturas de `CapturarReforma.ps1`; 144 DPI sin probar |
| T-03 | Codex | **URGENTE, confirmado:** sin las DLL de WebView2 o sin `Newtonsoft.Json.dll`, error en lugar del respaldo | cerrado: verificado sobre el ejecutable el 26/11, las 14 pantallas caen al respaldo |
| T-04 | Codex | Carpeta de la aplicación para la defensa, completa y comprobada | abierto; ahora `Web\` tiene 27 archivos, no 11 |
| T-05 | Codex | Lo que la reforma visual dejó declarado como fuera de conformidad | abierto: Codex lista los puntos primero y JC decide (26/11) |
| T-06 | Codex | Congelar el código | bloqueado por T-02, T-05, T-22, T-26 a T-29, T-31, T-36 y la decisión de T-37 |
| T-07 | Claude Code | Las dos filas del catálogo con reglas distintas de su prueba | cerrado (24/11) |
| T-08 | Codex y Claude Code | Batería final, dos corridas | bloqueado por T-06 |
| T-09 | Claude Code | Valores de los marcadores `[[...]]` y las tres tablas | bloqueado por T-08 |
| T-10 | Claude Code o Codex | Capturas finales | bloqueado por T-06 |
| T-11 | Claude Code | Páginas de pantallas contra la interfaz final | bloqueado por T-10 |
| T-12 | Claude Code | Diagramas de clases 4, 5, 7 y 8d | bloqueado por T-06 |
| T-13 | Claude Code | WebView2 en la arquitectura (p. 57) y la bibliografía (p. 112) | bloqueado por T-06 |
| T-14 | JC | Los datos que faltan para el Word | abierto |
| T-15 | Claude Code, después JC | JC-R10: escanear dos tickets con el celular | abierto |
| T-16 | JC | JC-R12: las cuatro pantallas sin WebView2 | abierto: T-03 cerrado el 26/11 |
| T-17 | Máximo | La tarde en Word: marcadores, pausas e índice | bloqueado por T-09, T-11 a T-14 |
| T-18 | Claude Code | Commitear los documentos y herramientas del 24/11 | cerrado (`0ddc0c4`) |
| T-19 | Claude Code | `tools/VerificarReglasAgentes.ps1` no corre | abierto |
| T-20 | Codex | **URGENTE:** sin base, la pantalla de inicio muestra la contraseña y no avisa nada | cerrado: verificado el 24/11 sobre el ejecutable (sin config y con la instancia inexistente) |
| T-21 | Codex | El cartel de esquema sin `02-semilla.sql` pide correr `04` | cerrado: verificado sobre el `Visual.exe` real el 26/11 |
| T-22 | Codex | El ticket se genera sin `try` después del pago confirmado; mensajes de PDF que no corresponden | abierto: el ticket pasa; **la exportación sin iText sigue mostrando «error inesperado»** (26/11) |
| T-23 | Codex | **Para la defensa:** los rechazos de la solicitud de Organizador dicen «error inesperado» | cerrado: verificado el 26/11, los cinco casos muestran el mensaje de la regla |
| T-24 | Codex | «Agregar evento» no deja elegir la hora | cerrado: verificado el 26/11 (evento a las 21:00) |
| T-25 | Codex | El registro de auditoría muestra la hora UTC | cerrado: verificado el 26/11 (13:26 local) |
| T-26 | Codex | **Para la defensa:** el catálogo y la compra quedan cortados dentro de «Mi cuenta» | abierto, reabierto el 26/11: **la compra desde el menú no se puede terminar** («Continuar al pago» fuera de la ventana) |
| T-27 | Codex | Vistas web: copiar el encargo de diseño del 26/11 (resumen, selección neutra, muro y velo) | en revisión, salvo el velo de los pasos del alta, que falta |
| T-28 | Codex | **Falla:** en el historial, cada compra ocupa una pantalla entera (A-1) | en curso (JC se lo pasó a Codex el 26/11) |
| T-29 | Codex | Cuatro hallazgos de una línea de la auditoría visual (M-4, M-5, B-2 y B-3) | a confirmar: JC pasó la auditoría a Codex; no está dicho si incluye estos |
| T-30 | JC | Decidir M-1 (gráfico de barras), M-3 (más de cuatro trazos violeta) y B-5 (`compra.*` sin uso) | cerrado: `4ee3003` |
| T-31 | Codex | Corregir identificadores duplicados de pruebas y completar comentarios y matriz | abierto, reabierto el 26/11: la renumeración rompe CP-359 (filtro con el nombre viejo); cierra cuando CP-359 pasa |
| T-32 | JC | P4, una sola tabla: ¿antes del congelamiento o después de la entrega? | decidido (26/11): entra solo si todo lo demás que bloquea T-06 cierra antes del 29/11 |
| T-33 | Claude Code | Volver a medir A-1 y A-2 y anotar las pruebas manuales de las vistas web | bloqueado por T-27 y T-28 |
| T-34 | Claude Code | Revisar la transcripción de los casos nuevos desde el 24/11 | hecho en parte el 26/11 (filas de Codex revisadas y corregidas); cierra con T-31 |
| T-35 | Claude Code | Poner `docs/disenos/` al día con lo que Codex integró | abierto, baja prioridad |
| T-36 | Codex | `frmReportes` mide 960 px de alto, por encima del techo de 790 | abierto (26/11) |
| T-37 | JC | ¿El alta y la recuperación avisan «Vista web no disponible» como las demás? | abierto (26/11) |
| T-38 | Codex, después JC | **P0, sin reproducir:** la aplicación se cerró sola en el pago, dos veces, sin dejar rastro | abierto: sin commit al 26/11 17:20 |
| T-39 | Codex | **P0:** una cuenta que sale del alta sin verificar no puede verificar después ni comprar | abierto: sin commit; JC decidió dónde se muestra la ruta de la bandeja |
| T-40 | Codex | El aviso verde del catálogo tapa los filtros de categoría y fecha | abierto: sin commit |
| T-41 | Codex | Pago: texto de ayuda con la tarjeta de prueba y barra automática en el vencimiento | abierto: sin commit |
| T-42 | Claude Code | Semilla de demostración: 25 eventos futuros, 7 organizadores, un solo agotado | en revisión: hecha y probada el 26/11, sin commitear |
| T-43 | Codex | Quitar el checkbox «Mostrar contraseña(s)» de las vistas web y dejar solo el ojo | pendiente de verificación en ejecución: `3100733` |
| T-44 | Codex | Pedidos directos de JC del 26/11 a la tarde: cambio de contraseña dentro de Mi cuenta, suscripción web, armador y agotados al final | pendiente de verificación en ejecución: `47ae5a3`, `a9f9c1b`, `7f76e63`, `e98a958` |

---

## Antes de congelar el código

### T-01 · Codex · Terminar y commitear la tanda visual en curso

**Qué:** hay ocho archivos sin commitear en tu zona:
- `Pruebas/BaseDePrueba.cs`: `PrepararDemostracion`, que carga `semilla-demostracion.sql` en
  `BddEventAura_Capturas_Demostracion`;
- `Pruebas/CapturarReforma.ps1`: parámetro `-Demostracion`;
- `Pruebas/PruebasInteraccionPantallas.cs`;
- `Visual/Propietario/frmReportes.cs`: `altoExtra` en `Shown`;
- `Visual/Tema.cs`: tres variables CSS nuevas;
- `Visual/Web/catalogo.css`, `Visual/Web/datos.css` y `Visual/Web/datos.js`.

JC pidió commitearlos como «wip(ui): graficos con eje y estados de disponibilidad». No se
commitearon el 24/11 porque estabas trabajando.

**Listo cuando:**
- la batería completa pasa;
- `git status` no muestra nada en `Visual/`, `Repo/`, `Datos/` ni `Pruebas/` (salvo los `App.config`,
  que nunca se publican);
- tu reporte dice el commit y la cantidad de ejecuciones.

**Estado:** cerrado. Se commiteó como `ff79658` el 24/11 a las 15:49, con el mensaje que pidió JC.

### T-02 · Codex · Prueba automática del respaldo de WebView2

**Qué:** hallazgos H-1, H-4, H-5 y H-6 de `docs/AUDITORIA-RESPALDO-WEBVIEW2.md`. Un caso nuevo, con
número CP correlativo, que fuerce la falla de la vista web en el catálogo, la compra, los reportes y
el panel del Administrador, y compruebe en cada uno:
- el panel web oculto y el aviso visible;
- los controles de Windows Forms visibles y con datos;
- ninguna superposición, con el control de CP-357;
- todo lo anterior a 96 y a 144 DPI.

Opcional, si sale con el mismo mecanismo: la falla después de que la web ya se veía (H-6).
**Cuidado:** el entorno de `PanelWeb` es `static` y queda en caché (ver H-1).

**Listo cuando:**
- el caso está en la batería y pasa;
- tiene su fila en `docs/04-casos-prueba.md` (Claude Code la transcribe desde tu reporte);
- `CapturarReforma.ps1` puede generar las capturas del respaldo con un parámetro.

**Estado:** en revisión. Según el reporte del 24/11, en `5b6fd5a`:
- H-3: el aviso «Cargando vista…»;
- H-4: el aviso de Reportes, debajo de «Generar»;
- H-5: CP-357 revisa la geometría y que los indicadores vuelvan a verse;
- H-6: la falla forzada después de preparar WebView2.

`b34b022` amplió la verificación a las seis migraciones del 24/11 a la tarde.

**Ejecutado el 26/11 (Claude Code), no pasa del todo:**
- con `vstest.console` sobre el árbol de trabajo, CP-357, CP-358 y CP-360 pasan y **CP-359 falla**:
  «Ninguna prueba coincide con el filtro… `FullyQualifiedName~CP199_RespaldoConDependenciasAusentesEnCopia`»
  (`TestResults/claude-verif-t02.trx`). Se arregla en T-31;
- `Pruebas/CapturarReforma.ps1` no tiene parámetro para capturar el respaldo; sí tiene uno de escala;
- 144 DPI, sin probar.

**Estado:** abierto. Falta: CP-359 pasando (T-31), el parámetro de capturas y una corrida a 144 DPI, o
que JC acepte declarar 144 DPI como no probado.

### T-03 · Codex · Que la falta de las DLL caiga al respaldo, no a un error

**URGENTE: es lo primero, antes que T-01.** Hallazgo H-2 de la auditoría, **confirmado sobre el
ejecutable el 24/11 a las 14:35** (Claude Code). Se probó una copia de `bin\Debug` sin
`Microsoft.Web.WebView2.*` ni `WebView2Loader.dll`, y las cuatro pantallas fallaron:
- `FileNotFoundException` en `PanelWeb.Cargar`, fuera del `try`;
- el panel del Administrador queda sin dibujar;
- reportes, catálogo y compra muestran el aviso rojo de error inesperado;
- catálogo y compra quedan además con la disposición sin ajustar.

Evidencia: `docs/verificaciones/sin-dll-webview2-2026-11-24/`. Detalle y forma de arreglarlo: la
auditoría, H-2. **`Cargar` no asíncrono y sin tipos de WebView2**, que llama dentro de un
`try/catch` a un método asíncrono que sí los tiene.

**Listo cuando:**
- sin las DLL (`Microsoft.Web.WebView2.Core.dll`, `Microsoft.Web.WebView2.WinForms.dll` y los
  `WebView2Loader.dll`), las cuatro pantallas abren en su versión de Windows Forms, con el aviso
  «Vista web no disponible…»;
- no aparece el aviso de error ni se agrega nada a `%LOCALAPPDATA%\EventAura\errores.log`;
- la disposición es la misma que con la web.

Si se puede, que T-02 cubra también este caso.

**Ampliado el 24/11 (auditoría de riesgos, sección 4):** sin `Newtonsoft.Json.dll` pasa lo mismo.
Hay un `FileNotFoundException` en `PanelWeb.<Renderizar>`, fuera de su `try`, y el panel del
Administrador muestra el aviso rojo de error. Mismo arreglo: los tipos de Newtonsoft fuera del método
que tiene el `try`. **Listo cuando** también sin `Newtonsoft.Json.dll` las cuatro pantallas caen al
respaldo con su aviso.

**Estado:** en revisión. Según el reporte del 24/11, en `5b6fd5a`:
- `PanelWeb` carga `PanelWebMotor` por reflexión dentro de un `try`;
- la prueba borra las DLL de una copia y verifica `PanelWeb` aislado, **no las pantallas**.

Esa prueba quedó numerada CP-359.

**Cerrado el 26/11, verificado sobre el ejecutable** (Claude Code). En una copia de `bin\Debug` sin
`Microsoft.Web.WebView2.*`, los tres `WebView2Loader.dll` ni `Newtonsoft.Json.dll`, las **14** pantallas con
vista web abren su versión de Windows Forms con datos, sin aviso de error y sin entradas en `errores.log`.
El panel del Administrador muestra los 8 indicadores. Once muestran «Vista web no disponible»; el alta y la
recuperación no muestran aviso (decide JC en T-37). Evidencia: `docs/verificaciones/ejecutable-2026-11-26/`,
capturas `t03-*` y `log-sindll.txt`.

### T-04 · Codex · Carpeta de la aplicación para la defensa

**Qué:** dejar escrito, en `Pruebas/GUIA-PUESTA-EN-MARCHA-MAXIMO.md` o en el reporte, qué lleva la
carpeta del programa que se usa en la defensa:
- `Visual.exe` y su `.config`, y las DLL del proyecto;
- `Microsoft.Web.WebView2.Core.dll`, `Microsoft.Web.WebView2.WinForms.dll` y `Newtonsoft.Json.dll`;
- las carpetas `runtimes\` (con `WebView2Loader.dll`) y `Web\`. Al 26/11 son 27 archivos, sin
  contar los tres `.cs`; el 24/11 eran 11.

Comprobar que una copia de esa carpeta, en otra ubicación, abre las vistas web. Al 26/11 son 14
pantallas que cargan 8 páginas.

**Listo cuando** la lista está escrita y la comprobación figura en el reporte.

**Estado:** abierto. Se hace con el código congelado: cada pantalla migrada cambia la lista.

### T-05 · Codex · Lo que la reforma declaró fuera de conformidad

**Qué:** tu reporte del 24/11 (`Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-24-REFORMA.md`, «Límites
del cotejo») deja tres cosas fuera:
1. los tamaños nativos de los selectores de Reportes y Auditoría;
2. márgenes y acciones heredados que no siguen la plantilla de formulario;
3. el límite de un tercio de espacio libre, no certificado en las 25 vistas.

Enter en Suscripción sigue como verificación manual porque abre un modal.

**Decide JC** si se hace antes de congelar o si queda declarado así. Hasta entonces, no se empieza.

**Listo cuando** JC decidió. Si decide hacerlo, cuando la batería pasa y las capturas a 96 y
144 DPI no muestran regresiones.

**Estado:** abierto. **Decisión de JC (26/11):** Codex lista primero los puntos concretos de cada uno de
los tres; con esa lista, JC decide cuáles se hacen antes del 01/12.

### T-06 · Codex · Congelar el código

**Qué:** cuando T-01 a T-05 estén cerrados, avisar que no hay más cambios de código y dar el hash
del commit congelado. Desde ese momento, en `Visual/`, `Repo/`, `Datos/` y `Pruebas/` solo entran
correcciones de fallas, y cada una se anota acá como ticket nuevo.

**Listo cuando** JC confirma el congelamiento y el hash queda anotado en `docs/ESTADO-AGENTES.md`.

**Estado:** bloqueado. Lo que tiene que estar cerrado está en `docs/CONGELAMIENTO-2026-12-01.md`.

### T-07 · Claude Code · Las dos filas del catálogo con reglas distintas de su prueba

**Qué:** el 24/11 se habían contado siete. Son dos; las otras cinco eran un error del script de
comparación (ver `docs/CIERRE-DOCUMENTO.md`, sección 5, punto 5).
- **CP-44:** `docs/04-casos-prueba.md` y la tabla caso → reglas de la p. 108 dicen «—» en reglas;
  el comentario de la prueba cita RN-05. Pasar a «RN-05». La cobertura no cambia: RN-05 ya la
  cubre CP-55.
- **CP-55:** la tabla de la versión 2.6 de la p. 108 dice «RF-01, RF-02»; la prueba, RF-01. Pasar
  a «RF-01».

**Listo cuando** el script de comparación, corrido contra el commit congelado, no da diferencias
entre el catálogo, las tablas de la p. 108 y los comentarios `// CP-xx`.

**Estado:** cerrado el 24/11.
- CP-44: «RN-05» en `docs/04-casos-prueba.md` y en la tabla caso → reglas de la p. 108.
- CP-55: «RF-01» en la tabla de la versión 2.6 de la p. 108.

El script de comparación no encuentra ninguna otra diferencia entre el catálogo y los comentarios.
La cobertura no cambia.

---

## Con el código congelado

### T-08 · Codex y Claude Code · Batería final, dos corridas

**Qué:** decisión de JC del 24/11, dos corridas completas sobre el commit de T-06:
1. **Codex**, fuera del sandbox, con la tarea «Probar»;
2. **Claude Code**, con `tools/Proyecto.ps1 -Accion Probar`, sin Codex trabajando.

**Listo cuando** las dos dan el mismo total de ejecuciones, todas correctas, sobre el mismo hash.
Ese total es el valor de `[[EJECUCIONES-LETRAS]]`.

**Estado:** bloqueado por T-06.

### T-09 · Claude Code · Valores de los marcadores y las tres tablas

**Qué:**
- completar la columna «Final» de la tabla «Marcadores de cierre» de `docs/LISTA-PARA-MAXIMO.md`
  (19 marcadores);
- rehacer la matriz de la p. 107 con `tools/Matriz/Extraer.py <commit>`;
- rehacer las dos tablas de casos de la p. 108;
- comprobar los 11 lugares que dicen la cobertura («50 de 52»).

Detalle en `docs/CIERRE-DOCUMENTO.md`, secciones 1.1 a 1.3. El hueco CP-212 a CP-353 se deja como
está (decisión de JC del 24/11).

**Listo cuando:**
- cada marcador tiene su valor final;
- `py -3 -X utf8 tools/Cierre/Inventario.py --resumen --commit <hash>` no da ninguna cifra
  escrita a mano (B, R y C en cero, salvo el rango histórico de la versión 2.5);
- las tres tablas salen de `Extraer.py` sobre ese hash.

**Estado:** bloqueado por T-08.

### T-10 · Claude Code o Codex · Capturas finales

**Qué:** rehacer unas 60 imágenes de `docs/capturas/`:
- las figuras 1 a 4 del instructivo;
- las tres del guion;
- las 56 del anexo «Pantallas del sistema».

Se hacen sobre el código congelado. Las rehace Codex, o Claude Code con
`tools/CapturasInstructivo/` si Codex no está trabajando.

**Más grande desde el 24/11:** la bienvenida, el acceso, el alta, la recuperación, el armador y seis
pantallas de gestión son ahora vistas web. Casi todas las figuras del instructivo y del guion muestran
alguna.

**Además, las herramientas de capturas probablemente ya no sirven.** Esto lo leí en el código; no lo
ejecuté. `tools/CapturasInstructivo/Captura.cs` y `tools/Usabilidad/Medicion.cs` arman la compra
escribiendo en controles de Windows Forms: `dgvSectores`, `chkHospedaje`, `lblResumen`, `pnlResumen` y
`btnComprar`. Desde `c2f80ca`, la compra es el armador web, y esos controles quedaron como adaptadores
ocultos (lo dice una prueba de `4b8d2d5`). Lo más probable es que las figuras salgan con la pantalla
vieja, o vacías, porque una captura de Windows Forms no dibuja el contenido de WebView2. Hay que probarlo
antes de contar con ellas.

**Listo cuando** las 60 se rehicieron sobre el hash de T-06 y JC las aprobó.

**Estado:** bloqueado por T-06.

### T-11 · Claude Code · Páginas de pantallas contra la interfaz final

**Qué:** las páginas en pausa (⏸) de la lista:
- p. 45, pp. 47 a 49, pp. 51 a 53 y 55;
- las especificaciones CU-01 a CU-21;
- el párrafo de usabilidad de las pp. 90 a 97;
- el anexo «Pantallas del sistema».

En cada una hay que revisar los nombres de botones y menús, los controles y los mensajes. En las
vistas web, las tablas de controles se redactan de nuevo. Eran 4 el 24/11 a la mañana; al 26/11 son
14 pantallas. Incluyen el inicio de sesión, el alta y el armador, que son el corazón del instructivo
(pp. 51 a 55).

**Listo cuando:**
- ninguna página de la lista sigue con ⏸;
- cada nombre entre comillas de esas páginas existe en la interfaz congelada (se comprueba con
  `tools/Pantallas/`).

**Estado:** bloqueado por T-10.

### T-12 · Claude Code · Diagramas de clases 4, 5, 7 y 8d

**Qué:** agregar los métodos públicos nuevos desde el 19/11:
- `GestorEventos.VendidasDeEvento`;
- `GestorListaEspera.EventosEnEspera`;
- `RepositorioEntradas.EntradasVendidas`;
- `RepositorioListaEspera.EventosEnEspera`;
- `SeleccionEnviadorCodigos.Para`.

Revisar también la vista 6 y las tres secuencias de la compra. Detalle en
`docs/CIERRE-DOCUMENTO.md`, sección 3.3.

**Listo cuando:**
- los diagramas se regeneraron con `py -3 tools/Diagramas/Renderizar.py`;
- cada operación pública de esas clases en el código congelado figura en su diagrama.

**Estado:** bloqueado por T-06.

### T-13 · Claude Code · WebView2 en la arquitectura y la bibliografía

**Qué:** pegar en la lista, con la cantidad de vistas web del código congelado:
- la fila Presentación de la tabla de capas (p. 57);
- el párrafo «Vistas web locales» (p. 57);
- la entrada de WebView2 en la bibliografía (p. 112, con el año 2026).

Redacción en `docs/CIERRE-DOCUMENTO.md`, sección 4. Las pp. 18 y 24 ya están aplicadas: opción (a),
decisión de JC del 24/11. Agregar WebView2 a la tabla «Stack» de `CLAUDE.md`.

**Listo cuando** los tres textos están en la lista sin ⏸ y la cantidad de vistas coincide con los
`PanelWeb` del código congelado.

**Estado:** bloqueado por T-06.

### T-20 · Codex · URGENTE · Sin base, la pantalla de inicio muestra la contraseña

**Qué:** `docs/AUDITORIA-RIESGOS-DEFENSA.md`, R-01 y secciones 1 y 4. **Ejecutado.**

Si la aplicación no llega a la base, `frmInicio_Load` se corta en `VerificadorEsquema.PrimerFaltante()`
(línea 34). Las causas pueden ser:
- una instancia o base inexistente o vacía;
- falta de permisos;
- que falten `Visual.exe.config`, `Datos.dll`, `Microsoft.Data.SqlClient.dll` o los SNI.

Lo que ve el usuario:
- `txtContraseñaLogin.PasswordChar` nunca se pone, así que **la contraseña se escribe a la vista**
  (`docs/verificaciones/defensa-2026-11-24/1-sin-sql-contrasena-visible.png`);
- no aparece ningún mensaje, porque la ventana todavía no es visible cuando `ManejadorErrores`
  busca dónde mostrarlo;
- con la instancia inexistente, la ventana tarda 15 s.

**Qué hacer:**
1. `PasswordChar` en el Designer, o al principio de `Load`.
2. La verificación dentro de un `try` que, ante un error de base o de configuración, muestre en la
   pantalla de inicio un aviso fijo que diga qué instancia y qué base se intentaron (de la cadena,
   sin credenciales) y deshabilite «Iniciar sesión».

**Listo cuando,** con `Data Source=.\NOEXISTE`, la contraseña sale oculta, el aviso fijo está visible
desde que se abre la ventana y hay una prueba automática que lo comprueba.

**Estado:** cerrado. Se verificó el 24/11 sobre el ejecutable, sin configuración y con la instancia
inexistente. La prueba quedó numerada CP-358.

### T-21 · Codex · El cartel de esquema sin `02` pide la migración equivocada

**Qué:** auditoría de riesgos, 2.4. **Ejecutado.** Con `01-esquema.sql` y sin `02-semilla.sql`, el cartel
dice «falta el parámetro INGRESO_HORAS_ANTES. Corré 04-seguridad-ingreso.sql», pero lo que falta es
`02`. `VerificadorEsquema` debería comprobar primero que existan los parámetros base y un
Administrador, y pedir `02-semilla.sql`.

**Listo cuando,** sobre una base con solo `01`, el cartel pide `02-semilla.sql`, y hay una prueba en
`PruebasEsquemaInicio`.

**Estado:** cerrado el 26/11, verificado. Sobre una base nueva con solo `01-esquema.sql`, el `Visual.exe`
real dice «…falta la semilla inicial con los parámetros y la cuenta Administrador. Corré
BaseDatos/02-semilla.sql y volvé a abrir EventAura.» Leído con UI Automation; no quedó captura.

### T-22 · Codex · El ticket después del pago y los mensajes del PDF

**Qué:** auditoría de riesgos, secciones 4 y 6.2. Leído en el código, **sin ejecutar**:
- **El ticket.** `FrmCompraCliente` llama a `GenerarTicket(compra)` (línea 421) sin `try`, después
  de que el pago quedó confirmado. Si falla la escritura en `Documentos` (Acceso controlado a
  carpetas, OneDrive) o falta iText o QRCoder, el usuario ve el aviso de error general con la compra
  ya pagada y sin ticket.
- **El PDF del reporte.** En `frmReportes.btnExportar_Click`, una DLL faltante
  (`FileNotFoundException`, que es una `IOException`) muestra «Cerrá el archivo si está abierto», y
  `UnauthorizedAccessException` no se captura.

**Qué hacer:**
1. Capturar la falla del ticket con un mensaje que diga que la compra está confirmada y dónde
   recuperar el ticket, si existe ese camino.
2. Distinguir esos casos en la exportación del reporte.

**Listo cuando** hay una prueba para cada caso (carpeta sin permiso de escritura) y el mensaje
corresponde a la causa.

**Estado:** en revisión. Codex lo informa hecho en `5b6fd5a`:
- el ticket conserva el pedido y remite a «Mis entradas»;
- la exportación distingue tres casos: sin permiso, dependencia ausente y archivo bloqueado.

La prueba del ticket quedó numerada CP-360. Según el reporte de Codex,
**no se simularon por separado** iText, QRCoder y los permisos de Documentos; los tres pasan por el
mismo `catch`.

**Ejecutado el 26/11 en una copia sin las DLL de iText (Claude Code):**
- **El ticket pasa.** La compra EA-005846 queda Confirmada; la pantalla dice que no se pudo crear el PDF y
  que las entradas están en «Mis entradas»; no se crea ningún archivo (`t22-2-confirmacion-sin-pdf.png`).
- **La exportación del reporte no pasa.** Muestra «Ocurrió un error inesperado y la operación no se
  completó…» (`t22-1-exportar-sin-itext.png`), y `errores.log` registra
  `System.IO.FileNotFoundException en Visual.ExportadorPDF..cctor()`.
  - **Causa:** `ExportadorPDF` tiene campos `static readonly` de tipos de iText (`DeviceRgb`, líneas 29 a
    33). Sin la DLL falla el constructor estático y .NET lo envuelve en una `TypeInitializationException`,
    que el `catch (FileNotFoundException)` de `frmReportes.btnExportar_Click` no atrapa.
  - La prueba de Codex pasa porque inyecta una `FileNotFoundException` directa.

**Qué hacer ahora:** atrapar también `TypeInitializationException` en la exportación, o sacar los
`static readonly` de iText de `ExportadorPDF`. La prueba tiene que quitar la DLL de verdad, como CP-359,
no inyectar la excepción.

**Listo cuando,** sin `itext*.dll`, «Exportar PDF» muestra el mensaje de dependencia ausente y no el de
error inesperado, y la prueba lo comprueba quitando la DLL.

**Estado:** abierto, reabierto el 26/11 por la exportación.

### T-23 · Codex · Los rechazos de la solicitud de Organizador muestran «error inesperado»

**Qué:** encontrado al recorrer el guion el 24/11 (`Pruebas/GUION-DEMOSTRACION.md`, secciones 7 y 8). **Ejecutado.**
`frmSolicitudOrganizador` (líneas 101, 113 y 118) y `frmRevisarSolicitudesOrganizador` (línea 62) capturan
`Exception` y muestran `ManejadorErrores.MensajeParaUsuario(ex)`, que para una `ReglaNegocioException`
devuelve el mensaje genérico. El usuario ve «Ocurrió un error inesperado y la operación no se completó…» en
lugar del mensaje de la regla. Comprobado con:
- CUIT inválido;
- teléfono sin confirmar;
- código de teléfono incorrecto;
- rechazo de una solicitud con el motivo vacío («Escribí el motivo del rechazo.»).

Por el mismo código, también el CBU inválido y el adjunto inválido. `frmRevisarEventos` sí muestra el
mensaje de la regla.

**Qué hacer:** capturar `ReglaNegocioException` antes del `catch` general y mostrar su mensaje, como en las
demás pantallas.

**Listo cuando** los cuatro casos muestran el mensaje de la regla (los textos están en la sección 8 del
guion) y hay una prueba que lo comprueba.

En `4b8d2d5`, `ManejadorErrores.MensajeParaUsuario` devuelve el mensaje de la `ReglaNegocioException`, y
`PruebasSolicitudesOrganizador` afirma el texto de la regla en `avisoSistema`.

**Estado:** cerrado el 26/11, verificado. CUIT inválido, CBU inválido, teléfono sin confirmar, código
incorrecto y rechazo sin motivo muestran el mensaje de la regla, sin entradas en `errores.log` (capturas
`t23-*`). Detalle menor, no bloquea: el aviso rojo tapa el campo CUIT durante 3 segundos.

### T-24 · Codex · «Agregar evento» no deja elegir la hora

**Qué:** ejecutado el 24/11. `frmAgregarEvento` usa `dtEvento.Value`, un selector con formato de fecha
larga y sin hora. El evento queda con la hora en que se abrió el formulario. Consecuencias:
- no se puede crear un evento a las 21:00;
- un evento creado para hoy queda en el pasado a los pocos minutos y no se puede vender;
- sin un evento a pocas horas, el control de ingreso (que abre 6 horas antes) no se puede mostrar en la
  defensa sin tocar la base.

**Qué hacer:** que el selector tome fecha y hora (por ejemplo, `CustomFormat = "dd/MM/yyyy HH:mm"`, como
`dtpFinPreventa`), con la hora inicial en una hora redonda futura.

**Listo cuando** el Organizador puede elegir día y hora, y hay una prueba que registra un evento a una hora
elegida.

**Estado:** en revisión, según el código. En `4b8d2d5`:
- el selector usa `dd/MM/yyyy HH:mm` y propone la próxima hora en punto;
- `PruebasGrupoAPropietario` afirma que la hora elegida se guarda.

**Estado:** cerrado el 26/11, verificado. Abierto a las 13:26, el selector propone 14:00; un evento cargado
para el 29/11/2026 a las 21:00 queda guardado a las 21:00:00 (evento 160 de `BddEventAura_Pruebas`,
`t24-1-hora-elegida.png`).

### T-25 · Codex · El registro de auditoría muestra la hora UTC

**Qué:** ejecutado el 24/11. La tabla `Auditoria` guarda la fecha en UTC: 19:52:24 cuando la PC marcaba
16:52. `frmAuditoria` la muestra sin convertirla a la hora local. El jurado ve tres horas de diferencia
con el reloj de Windows.

**Qué hacer:** convertir a hora local al mostrar (los filtros ya convierten de local a UTC desde `cfdad75`).

**Listo cuando** una acción hecha a las HH:mm locales figura a las HH:mm, y hay una prueba que lo comprueba.

**Estado:** en revisión, según el código. En `4b8d2d5`:
- `frmAuditoria` convierte de UTC a la hora local;
- una prueba registra un movimiento y afirma que se ve con menos de un minuto de diferencia con
  `DateTime.Now`.

`b34b022` volvió a tocar `frmAuditoria`, que desde el 24/11 es vista web.

**Estado:** cerrado el 26/11, verificado. Un inicio de sesión a las 13:26 locales (16:26 UTC) figura
a las 13:26, en la página web y en la grilla de respaldo (`t25-1-auditoria-hora-local.png`).

### T-26 · Codex · El catálogo y la compra quedan corridos dentro de «Mi cuenta»

**Qué:** ejecutado el 24/11, a 96 DPI con la ventana a 1216 × 799. En la navegación del cliente (de
**Eventos disponibles** a la compra):
- el título «Mi cuenta» del inicio sigue arriba;
- el contenido del catálogo queda corrido hacia abajo y a la derecha;
- en el catálogo, los botones **Ver entradas** de la primera fila de tarjetas quedan cortados por el
  borde inferior del área visible.

La compra funciona, pero se ve como dos pantallas encimadas.

Evidencia: las capturas `g3-01-catalogo.png` y `g3-06-dos-campo.png` del recorrido (descriptas en el
guion, sección 3).

**Listo cuando,** dentro de la navegación, el título corresponde a la pantalla abierta y la primera fila
de tarjetas se ve completa, a 96 y a 144 DPI.

**Estado:** en revisión, según el código. En `4b8d2d5`:
- `frmMenuCliente` pone en el título el nombre de la pantalla abierta;
- una prueba afirma «Eventos disponibles».

**Ejecutado el 26/11 (Claude Code), a 96 DPI: no pasa, y está peor que el 24/11.** El título sí cambia.
Pero la ventana «Mi cuenta» es fija (`FixedSingle`, sin maximizar, 1200 × 760) y las pantallas que aloja
quedan más grandes que ella:
- **catálogo:** la vista web va de X 304 a 1240 en un área de 1200; la segunda tarjeta queda cortada 40 px
  y «Volver» y «Ver como tabla» quedan debajo del borde (`t26-1-catalogo-en-mi-cuenta.png`);
- **ficha:** de «Armar paquete» se ven 16 px (Y 744 a 780 en 760), sin barra de desplazamiento
  (`t26-2-ficha-sin-armar-paquete.png`);
- **armador:** la vista mide 1280 de ancho y arranca en X 272; el total y «Continuar al pago» (x 930 de la
  página) quedan enteros fuera de la ventana, y la página no se desplaza
  (`t26-3-armador-sin-total-ni-continuar.png`);
- sin las DLL pasa lo mismo: «Comprar» queda en Y 814 y X 1368.

**El recorrido de compra de la sección 3 del guion no se puede completar desde el menú.**

**Causa:** `0160b55` («ampliar ventanas de pantallas migradas») puso `MinimumSize` de 1280 × 720 en
`FrmCompraCliente` y de 1000 × 680 en `frmEventosCliente` y `frmHistorialCliente`. Dentro de «Mi cuenta»,
`HostFormularioCliente` les da unos 960 de ancho y el mínimo le gana. Sueltas se ven bien: por eso CP-357,
que las abre como ventanas, pasa.

**Qué hacer ahora:** que `HostFormularioCliente` anule el `MinimumSize` de la pantalla que aloja, o que esas
tres lo pongan solo cuando son ventanas sueltas. Después, que el armador reparta su ancho en unos 960 px.

**Listo cuando,** en la ventana de 1200 × 760, se ven completos: las tarjetas del catálogo con «Volver» y
«Ver como tabla»; «Armar paquete» en la ficha; el total y «Continuar al pago» en el armador. Y hay una
prueba que lo afirma **abriendo las pantallas desde el menú**, no sueltas. 144 DPI va en la verificación
manual de T-33.

**Estado:** abierto, reabierto el 26/11. **Es el más importante para la defensa.**

---

## Vistas web del 25 y 26/11 y auditoría visual

Origen:
- el encargo de diseño del 26/11, `docs/BORRADOR-ENCARGO-CODEX-2026-11-26.md`. **No se borra hasta que
  Codex confirme que terminó:** lo está usando de referencia;
- la auditoría visual, `docs/AUDITORIA-VISUAL-2026-11-26.md`. Sus IDs (A-1, M-3…) son de ese
  informe.

### T-27 · Codex · Copiar el encargo de diseño del 26/11 a `Visual/Web/`

**Qué:** las secciones 1, 2 y 2 bis del borrador. Decisiones de JC del 26/11:

| Propuesta | Decisión |
|---|---|
| P1: resumen del armador por contenedor | **No va a la aplicación.** El bloque WebView2 de Codex queda como está (PRINCIPIOS-DISENO.md §4) |
| P2: selección neutra (`--seleccion`) | Va |
| P3: rótulos N6 en minúscula | Afuera |
| P4: una sola tabla | Ver T-32 |
| P5: muro rehecho, con el velo detrás de los textos en `--lateral-seccion` | Va |

**No se copian encima** `armador-paquete.css`, `armador-paquete.js`, `armador-paquete.html` ni
`alta-cuenta.js`: las copias de Codex tienen trabajo de integración que la maqueta no tiene (borrador,
«Antes de empezar»).

**Estado:** en revisión, salvo una línea.
- Ya están: `b4ae003` (resumen de 320 px), `756d106` (selección neutra) y `b3a0d1b` (muro y velo del N6
  y el pie). Claude Code midió el velo de `b3a0d1b` y cumple.
- **Falta** el bloque `.pasos` en `Visual/Web/alta-cuenta.css`, en el borrador, sección 2 bis. Sin él,
  los pasos pendientes del alta quedan entre 3,75 y 4,44:1 (A-2).

**Listo cuando:**
- se cumple «Cómo se da por hecho» del borrador: la batería pasa y hay capturas de las cuatro pantallas
  de entrada y del armador a 96 y a 144 DPI;
- la nueva medición de T-33 da 4,5:1 o más en los pasos.

### T-28 · Codex · El historial: cada compra ocupa una pantalla entera

**Qué:** A-1 de la auditoría, **ejecutado**.
- `sistema.css:166` define `.entrada { min-height: 100vh }` para la estructura de las pantallas de
  entrada.
- `gestion.js:25` usa la misma clase para cada renglón del historial, y `gestion.css` no pisa el alto.
- Al elegir un pedido, el usuario ve un hueco en blanco y la casilla «Devolver» queda abajo o fuera de
  la vista.
- Está en lo commiteado desde `b6d05bf`.

**Qué hacer:** renombrar la clase del historial (por ejemplo, `entrada-ticket`) o fijar `min-height: 0`
en `gestion.css`. Lo primero evita que vuelva a pasar.

**Listo cuando:**
- con una compra de dos entradas, los dos renglones y «Devolver» se ven juntos sin desplazar;
- hay una prueba que afirma el alto del renglón.

**Estado:** en curso: JC se lo pasó a Codex el 26/11.

### T-29 · Codex · Cuatro hallazgos de una línea

**Qué:** de la auditoría, todos ejecutados:
- **M-4:** «Ya tengo una cuenta.» de la bienvenida da 4,3:1 (`sistema.css:158`). Con `#fff` da 5,7.
- **M-5:** los marcadores de los campos del alta dan 3,35:1 (`sistema.css:112`). Con
  `--texto-secundario` dan 6,4.
- **B-2:** las etiquetas de estado de «Eventos del local» se estiran. Se arregla con
  `align-content: start` en `.tarjeta` (`gestion.css:7`).
- **B-3:** el detalle de la revisión de eventos pierde los saltos de línea. Se arregla con
  `white-space: pre-line`.

**Listo cuando** T-33 los vuelve a medir y dan bien.

**Estado:** a confirmar. JC le pasó la auditoría a Codex el 26/11; no está dicho si estos cuatro
entran.

### T-30 · JC · Tres decisiones de la auditoría

- **M-1, el gráfico de barras** de los reportes y del panel del Administrador:
  - corta el último renglón;
  - los rótulos se ven a 17-18 px, fuera de la escala;
  - el interlineado es 1,0.

  Arreglarlo es rehacer la escala del SVG en `datos.js` (22-55). **Es lo que más se ve en la defensa**
  de los tres.
- **M-3, más de cuatro trazos violeta**: los 8 botones del catálogo, las 9 barras de los reportes y lo
  elegido en gestión (en violeta suave; la regla dice neutro).
- **B-5:** `compra.html`, `compra.css` y `compra.js` no los carga ninguna pantalla desde `c2f80ca`. Solo
  los nombra una prueba (`PruebasInteraccionPantallas.cs:196`). Hay que decidir si se borran.

Quedan como están, salvo que JC diga otra cosa:
- **M-2:** el desglose del armador no entra entero a 1280 × 720. El total y el botón sí; JC decidió no
  tocar ese diseño.
- B-1, B-4 y B-6.

**Estado:** cerrado: resuelto en `4ee3003` («reservar el acento para acciones»).

### T-31 · Codex · Números de caso repetidos

**Qué:** encontrado por Claude Code el 26/11, leyendo el código.

| Número | Caso existente (se conserva) | Caso agregado el 24/11, ya renumerado |
|---|---|---|
| CP-198 | `PruebasPublicacionEventos`: el Organizador no rechaza eventos y el Administrador debe justificar | — |
| CP-199 | `PruebasPublicacionEventos`: el rechazo devuelve a borrador | — |
| CP-200 | `PruebasSeguridadCuenta`: la recuperación no enumera correos | — |
| CP-358 | — | `PruebasEsquemaInicio`: sin conexión, el inicio avisa y oculta la contraseña (T-20) |
| CP-359 | — | `PruebasInteraccionPantallas`: sin las DLL, respaldo (T-03) |
| CP-360 | — | `PruebasInteraccionPantallas`: falla del ticket después del pago (T-22) |

CP-364 y CP-365 ya tienen comentario `// CP-xx — RF-yy, RN-zz: …` y fila en `docs/04-casos-prueba.md`.

**Por qué importa:**
- la matriz y las tablas de la p. 108 salen de esos comentarios (`tools/Matriz/Extraer.py`): con
  números repetidos, el documento cuenta mal los casos;
- el jurado puede buscar un CP y encontrar dos.

**Resolución (26/11):** se conservaron CP-198 a CP-200 para sus pruebas originales y las tres pruebas
nuevas pasaron a CP-358, CP-359 y CP-360. CP-364 y CP-365 tienen comentarios y filas con RF, RN y
resultado esperado en `docs/04-casos-prueba.md`; la matriz de `docs/LISTA-PARA-MAXIMO.md` también se
actualizó. El rango CP-361 a CP-363 sigue libre.

**Listo:** cada `CP-xx` aparece en un solo `[TestMethod]`; se comprobó sin ejecutar la aplicación.
`Extraer.py` no detecta duplicados, así que el barrido de nombres se hizo por separado.

**Reabierto el 26/11 (verificación de Claude Code; JC lo deja abierto hasta que CP-359 pase).** La
renumeración está bien en los números y en `docs/`, pero rompe CP-359. La prueba se copia a una carpeta
temporal y se vuelve a lanzar a sí misma filtrando por el nombre del método, y el filtro quedó con el nombre
viejo: `Pruebas/PruebasInteraccionPantallas.cs:204`,
`FullyQualifiedName~CP199_RespaldoConDependenciasAusentesEnCopia`; el método ahora es `CP359_…`.
Resultado: «Ninguna prueba coincide con el filtro» (`TestResults/claude-verif-t02.trx`).

El registro `TestResults/bateria-365-2026-11-26.md` es de antes de renumerar y dice que los cambios
posteriores «solo modifican identificadores y comentarios». No es así: con la renumeración, la batería da
364 de 365.

**Qué hacer ahora:** el filtro con `nameof(CP359_RespaldoConDependenciasAusentesEnCopia)`. Mejor renombrar
también `EVENTAURA_CP199_HIJO` y la carpeta `EventAura-CP199-`. Volver a correr la batería completa
**después** de renumerar, y que el registro diga sobre qué commit se corrió.

**Listo cuando** CP-359 pasa y hay un registro de batería completa, posterior a la renumeración, con 365 de
365.

**Estado:** abierto.

### T-32 · JC · P4, una sola tabla

**Qué:** `docs/disenos/PROPUESTA-TABLA.md`. Toca `sistema.css`, `datos.css`, `datos.js` y `gestion.css`,
es decir, las vistas de reportes, del panel del Administrador y las seis de gestión.

**Recomendación de Claude Code: después de la entrega.** No arregla ninguna falla. Cambia la mitad de
las vistas web, así que obliga a rehacer sus capturas (T-10) y sus páginas (T-11). Si JC la quiere,
tiene que entrar antes de T-06.

**Estado:** decidido por JC el 26/11: entra solo si todo lo demás que bloquea T-06 cierra antes del
29/11. Si no, queda para después de la entrega.

### T-33 · Claude Code · Volver a medir A-1 y A-2, y anotar las pruebas manuales

**Qué:**
- cuando Codex termine T-27 y T-28, medir de nuevo en `Visual/Web/` con Playwright sin ventana: el alto
  de los renglones del historial y el contraste de los pasos del alta;
- pasar a `docs/PRUEBAS-PENDIENTES.md` las pruebas manuales del borrador:
  - el armador dentro de la aplicación, a 100 % y a 150 %;
  - las cuatro pantallas de entrada, a 100 % y a 150 %;
  - el historial con una compra de dos entradas.

**No se usa Playwright mientras Codex trabaja con ventanas.**

**Estado:** bloqueado por T-27 y T-28.

### T-34 · Claude Code · Los casos nuevos en el catálogo

**Qué:** transcribir a `docs/04-casos-prueba.md` los cuatro casos de T-31, ya renumerados, y cualquier
otro que Codex haya agregado desde el 24/11 (los de `b34b022` y `9b36ac6` se revisan en el código).

**26/11:** Claude Code revisó lo que Codex escribió en `docs/04-casos-prueba.md` y
`docs/LISTA-PARA-MAXIMO.md` para CP-358 a CP-360, CP-364 y CP-365. Números, textos y RF están bien. Se
corrigieron dos cosas: RNF-02 estaba en la columna de reglas de CP-358 a CP-360 (va con los RF, como en
CP-354), y un párrafo decía que los cinco casos pasaron en la corrida de 365, cuando CP-359 falla desde la
renumeración.

**Estado:** cierra con T-31, cuando haya una batería posterior a la renumeración para citar.

### T-35 · Claude Code · `docs/disenos/` al día

**Qué:** traer a la maqueta lo que Codex integró en `armador-paquete.js` y `alta-cuenta.js`. Así la
referencia no vuelve a quedar vieja (borrador, «Para Claude Code, después»).

**Estado:** abierto, baja prioridad. No es para la entrega.

### T-36 · Codex · `frmReportes` supera el alto máximo

**Qué:** encontrado en la verificación del 26/11. `frmReportes` mide **1100 × 960** de área cliente, con o
sin vista web (`t03-11-reportes-sin-dll.png`). `CLAUDE.md` («Alto máximo de un formulario») pone el techo en
790. Lo cambió `0160b55`. En la pantalla de desarrollo (1032 px útiles) entra; en una de 768, o a 125 %,
los controles de abajo quedan fuera.

**Qué hacer:** bajar el `ClientSize` a 790 de alto como máximo, compactando, sin agrandar nada.

**Listo cuando** `ClientSize.Height` ≤ 790 y CP-357 lo afirma para `frmReportes`.

**Estado:** abierto.

### T-37 · JC · El aviso del respaldo en el alta y la recuperación

**Qué:** sin las DLL de WebView2, once pantallas muestran «Vista web no disponible» y dos no: el alta de
cuenta y la recuperación de contraseña. Las dos funcionan en su versión de Windows Forms.

**Decide JC** si avisan igual que las demás (sería un ticket de Codex) o si quedan así, declarado.

**Estado:** abierto.

### T-38 · Codex, después JC · P0: cierre inesperado en el pago, sin reproducir

**Qué contó JC (26/11):** con la aplicación abierta en el método de pago, se cerró sola a los ~20 s, y
después volvió a cerrarse. Por la base, fue en `BddEventAura` (no en una `_Demostracion`): la cuenta
`test0001@pruebas.eventaura.com` inició la compra 12 a las 16:44:56, que quedó Pendiente. El build de las
16:28 lo hizo Visual Studio, y a las 16:30:38 hay un aviso de .NET (1022) típico de un proceso lanzado con
el depurador.

**Qué hizo Claude Code el 26/11, sobre el `Visual.exe` real** (copia de `bin\Debug` del build de las
16:28, contra `BddEventAura_Pruebas`, manejada con UI Automation; capturas 1, 2, 3 y 7 en
`docs/verificaciones/pago-y-verificacion-2026-11-26/`):
- Paula: catálogo → Duki → Armar paquete → una Platea → Continuar al pago → Seguir sin seguro → **Pago
  seguro** abierto **3 minutos sin tocar**, después con los cuatro campos cargados un minuto más, y hasta
  que **venció la reserva** (10 min). La aplicación no se cerró: al vencer volvió al catálogo con «La
  reserva venció o cambió…». `errores.log` no creció.
- Con una cuenta nueva verificada, la compra completa hasta «Pedido EA-000072». Tampoco se cerró.
- **Con el depurador de Visual Studio no se probó:** un agente no puede manejar el depurador.

**Qué se descartó, con evidencia:**
- **Una excepción .NET no controlada:** no hay evento 1026 de .NET Runtime ni entrada en `errores.log`
  después de las 15:44. `ManejadorErrores` registra antes de mostrar.
- **La batería de Codex:** `Pruebas/EjecutarBateriaSegura.ps1` mata todo `Visual.exe` del repositorio y
  CP-209 mata el de `bin\Debug`, pero no hubo ninguna corrida ni archivo nuevo entre las 16:20 y las 16:52.
- **El vencimiento de la reserva (`frmPago.tmrReserva`):** al vencer, el pago se cierra con `Abort` y la
  compra vuelve al catálogo; la aplicación sigue abierta. Además, a los 20 s faltan 9:40.
- **El cierre por inactividad (`ControlSesionInactiva`):** son 15 min (JC llevaba 4 desde el inicio de
  sesión) y no termina el proceso: muestra el acceso.
- **`FormularioEventAura.OnFormClosed` → `Application.Exit()`:** solo con `UserClosing` y ninguna otra
  ventana visible; con el pago abierto, «Mi cuenta» sigue visible.
- **La caída del proceso de WebView2 (`PanelWebMotor.cs:67`, `ProcessFailed` → `Desactivar`):** pasa al
  respaldo, no cierra la aplicación.

**Lo que no se puede descartar, y por qué no hay rastro:** `ManejadorErrores.Instalar` llama a
`SetErrorMode(SEM_FAILCRITICALERRORS | SEM_NOGPFAULTERRORBOX)` (`Visual/ManejadorErrores.cs:28`). Con
`SEM_NOGPFAULTERRORBOX`, una caída **nativa** (un desbordamiento de pila, un acceso inválido en el
cargador de WebView2) no pasa por el Informe de errores de Windows y **no deja ni el evento 1000**. Tampoco
queda registro de un cierre ordenado: nada anota quién llamó a `Application.Exit`.

**Qué hacer (Codex, cambio mínimo, solo diagnóstico):**
1. En `ManejadorErrores.Instalar`, registrar en `errores.log` cada cierre: `Application.ApplicationExit`
   y `AppDomain.CurrentDomain.ProcessExit` con la hora, la última ventana que se cerró y su
   `CloseReason`, y `Environment.StackTrace` cuando el cierre viene de `Application.Exit`.
2. Quitar `SEM_NOGPFAULTERRORBOX` (dejar `SEM_FAILCRITICALERRORS`), para que una caída nativa deje el
   evento 1000 en el Visor de eventos.

**Qué hace JC después:** repetir con el ejecutable, sin Visual Studio, y anotar la cuenta, el evento, qué
tocó y la hora. Si se cierra, `errores.log` y el Visor de eventos dicen por qué.

**Listo cuando:** o hay una causa con archivo y línea y está corregida, o JC repitió el recorrido dos veces
con el registro de cierre y no se cerró.

**Commits:** ninguno al 26/11 a las 17:20. Cuando exista, Claude Code repite el recorrido de JC diez veces con
el registro de cierre (encargo de JC del 26/11).

**Estado:** abierto.

### T-39 · Codex · P0: una cuenta sin verificar queda trabada

**Qué (ejecutado el 26/11 sobre el `Visual.exe` real):**
- **RN-43** exige el correo verificado para comprar. Lo aplican `GestorPaquetes.Nuevo` (`Repo/GestorPaquetes.cs:43`)
  y `GestorCompras.Iniciar` (`Repo/GestorCompras.cs:40`) con el mensaje «Verificá tu correo antes de
  iniciar una compra.» (`Repo/GestorUsuarios.cs:49`).
- **El único lugar donde se verifica es el paso 3 del alta.** `EnviarVerificacion` solo se llama al crear
  la cuenta, y en la interfaz `ConfirmarVerificacion` solo está en `frmCrearUsuario`.
- **Camino completo, sin salir del alta: funciona.** Alta → código en la bandeja → «Correo verificado» →
  inicio de sesión → Duki → pago → «Pedido EA-000072».
- **Camino roto:** crear la cuenta, pulsar **Volver al acceso** en «Verificá tu correo», e iniciar sesión.
  La cuenta entra; al pulsar **Armar paquete** aparece el aviso «Verificá tu correo antes de iniciar una
  compra.» y **no hay ningún botón ni pantalla para verificar ni para pedir otro código**. Lo mismo si el
  código venció (10 min): el alta dice «Solicitá uno nuevo», pero no hay botón para pedirlo. Captura:
  `docs/verificaciones/pago-y-verificacion-2026-11-26/8-sin-verificar-trabada.png`.
- **La semilla deja verificadas a las cuentas de la demostración:** `CorreoVerificado` tiene `DEFAULT (1)`
  (`01-esquema.sql:96`); solo las altas nuevas de Cliente entran en 0 (`RepositorioUsuarios.cs:110`).

**Qué hacer, mínimo:**
1. En el aviso de RN-43, un botón **Verificar mi correo** que llame a
   `GestorCodigosCorreo.EnviarVerificacion` con la cuenta de la sesión y abra el paso de código (el mismo
   del alta).
2. En el paso de código, un botón **Enviar otro código**.
3. **El texto de la bandeja, con cuidado.** El guion (sección 2) dice que la pantalla no muestra la ruta
   **a propósito**, porque la recuperación de una cuenta ajena podría leer el código. Por eso, la ruta
   `%LOCALAPPDATA%\EventAura\BandejaSimulada` se puede mostrar **solo en la verificación con sesión
   iniciada**, donde la cuenta es de quien la usa, y nunca en la recuperación.

   **Decidido por JC el 26/11:** la ruta se muestra **solo** en la verificación con sesión iniciada **y solo si
   el envío es simulado** (`EnviadorSimulado`; con SMTP configurado, no). En la recuperación no se muestra
   nunca. El guion (sección 2) lo dice igual.

**Listo cuando,** con una cuenta que salió del alta sin verificar, se puede pedir el código desde el aviso,
verificar y comprar sin cerrar sesión; y hay una prueba que lo recorre.

**Estado:** abierto.

### T-40 · Codex · El aviso del catálogo tapa los filtros

**Qué:** visto el 26/11 dos veces, con «Verificá tu correo antes de iniciar una compra.» y con «La reserva
venció o cambió…». El aviso verde de `AvisosEnPantalla` aparece en X 680 a 1160 e Y 247 a 330 de «Mi
cuenta», encima del selector de categoría y del de fecha del catálogo
(`docs/verificaciones/pago-y-verificacion-2026-11-26/`, capturas 3 y 8).

**Listo cuando** el aviso no tapa ningún control del catálogo (por ejemplo, debajo de los filtros o en la
franja del título), con una prueba de geometría como las de CP-357.

**Estado:** abierto.

### T-41 · Codex · Pago: ayuda con la tarjeta de prueba

**Qué:** `frmPago` no dice qué datos acepta. Las reglas están en `GestorCompras.ValidarTarjeta` y
`PasarelaSimulada.Cobrar`, y en el guion (sección 3, paso 4): 13 a 19 dígitos, sin Luhn; vencimiento
`MM/AA` **con barra** y no vencido; código de 3 o 4 dígitos; aprobada salvo terminación `0000`. Al
escribir `1228` en el vencimiento, el pago responde «El vencimiento debe tener el formato MM/AA.».

**Qué hacer:**
- una línea fija bajo el título: «Tarjeta de prueba: cualquier número de 13 a 19 dígitos, por ejemplo
  4111 1111 1111 1111. Las terminadas en 0000 se rechazan. Vencimiento MM/AA futuro y código de 3 dígitos.»;
- en el vencimiento, insertar la barra al escribir el tercer dígito.

**Listo cuando** la línea se ve en «Pago seguro» sin recortes a 96 DPI y `1228` queda como `12/28`.

**Estado:** abierto.

### T-42 · Claude Code · Semilla de demostración con 25 eventos

**Qué pidió JC (26/11):** 25 eventos futuros de distintas categorías y ciudades, de 6 a 8 organizadores,
con locales variados; exactamente un evento agotado; los demás con cupos distintos, alguno con pocas
entradas; sectores con precios que bajen al alejarse del escenario.

**Hecho** en `BaseDatos/semilla-demostracion.sql`, con autorización de JC para tocar `BaseDatos/` en esta
sesión:
- 6 organizadores nuevos, 8 sedes nuevas en Rosario, Santa Fe, Paraná, Córdoba y Buenos Aires;
- 23 eventos futuros nuevos: con Duki y el Clásico, **25** de **7** organizadores y 4 categorías;
- **un solo agotado:** Clásico Rosarino; tres con pocas entradas; dos sin ventas;
- la semilla comprueba al final los 25, los 6 a 8 organizadores, el único agotado y el orden de precios,
  y se detiene si algo no da;
- Paula ya no queda anotada en la lista de espera, para que el paso 6.4 del guion funcione.

**Probado el 26/11:** sobre una base nueva, con los scripts en el orden de la batería, corrida dos veces:
37 eventos, 964 entradas, 7 organizadores, 3 en lista de espera. **La batería no la usa:** corre sobre
`BddEventAura_Pruebas` y solo `Pruebas/CapturarReforma.ps1` carga esta semilla, en
`BddEventAura_Capturas_Demostracion`.

**Corregido el 26/11 a la tarde (pedido de JC):** los 6 clientes Oro absorbían las ventas que sobraban y
quedaban con 45 a 47 compras en 12 meses. **RN-19 no lo exige:** Oro empieza en 7 compras
(`FIDELIDAD_UMBRAL_ORO`). La semilla ahora suma 44 clientes generados y reparte: 21 Bronce (1 o 2
compras), 20 Plata (3 a 6) y 20 Oro (9 a 11). Comprueba que nadie pase de 12 (error 51013). Corrida dos
veces sobre una base nueva: pasan todos sus chequeos.

**Falta:** recorrer el guion con la semilla nueva (lo hace Claude Code con el escritorio libre).

**Estado:** en revisión.

### T-43 · Codex · Solo el ojo para ver la contraseña

**Qué:** JC decidió el 26/11 quitar el checkbox «Mostrar contraseña(s)» y dejar solo el ojo. El checkbox es
`checkbxShowPass` de Windows Forms, en `frmInicio`, `frmCrearUsuario` y `frmCambiarContraseña`. El 26/11 se
lo vio **encima de la vista web del alta**, debajo de «Confirmar contraseña».

**Casos que hay que reescribir:**
- **CP-98** (`PruebasSeguridad.cs:194`): afirma que `checkbxShowPass` está en el respaldo WinForms, con
  `TabStop` y dentro de la pantalla;
- **CP-176** (`PruebasVistasVisibles.cs:167`): usa `checkbxShowPass` en la recuperación;
- los ojos de las vistas web ya los prueba `ComprobarOjos` (`PruebasInteraccionPantallas.cs:227`) y no
  cambian.

El respaldo WinForms necesita su propio ojo; si no, en el respaldo no se puede ver la contraseña.

**Alcance decidido por JC el 26/11:** el checkbox desaparece **de las vistas web**; queda solo el ojo. El
respaldo WinForms conserva su casilla, y por eso CP-98 y CP-176 la siguen afirmando ahí.

**Hecho por Codex en `3100733`** («reemplazar checks de contraseña por ojos accesibles»): acceso, alta,
recuperación y cambio sin la casilla web; los ojos los prueba `ComprobarOjos`. Claude Code revisó CP-98, CP-176,
CP-211 y CP-364 contra el código el 26/11 y corrigió sus filas.

**Falta:** verificar en ejecución que el ojo está siempre visible, funciona y es un solo control en las cuatro
pantallas, y que en el alta ya no se ve `checkbxShowPass` encima de la vista web.

**Estado:** pendiente de verificación en ejecución.

### T-44 · Codex · Pedidos directos de JC del 26/11 a la tarde

Sin ticket previo; JC se los pidió a Codex directamente. Claude Code revisó las pruebas el 26/11 y actualizó
sus filas en `docs/04-casos-prueba.md`.

| Commit | Qué | Pruebas |
|---|---|---|
| `47ae5a3` | Cambiar contraseña se abre dentro de «Mi cuenta», sin ventana aparte | CP-211 |
| `a9f9c1b` | «Mi suscripción» pasa a vista web con tarjetas de plan | CP-76, CP-77, CP-78 |
| `7f76e63` | El armador separa servicios y resumen del panel | — (CSS) |
| `e98a958` | El catálogo pone los agotados al final | CP-366, nueva |

**Falta:** verificar en ejecución, con la semilla de demostración: un solo título en «Mi suscripción», las
tarjetas y Enter; el cambio de contraseña sin ventana aparte; el armador en cinco eventos con juegos de
sectores distintos, con el total a la vista y el mapa orientativo dibujado; el catálogo con 25 eventos y el
agotado al final. **CP-366 no está en ninguna corrida completa todavía.**

**Estado:** pendiente de verificación en ejecución.

---

## Personas

### T-14 · JC · Los datos que faltan para el Word

**Qué:**
- comisión y turno;
- los dos legajos;
- la fecha de la versión 2.5 y la de la 2.6 (historial, p. 6);
- las nueve «Fecha estimada» del cronograma (pp. 15 a 17).

Ya decididos el 24/11 y aplicados en la lista: «Máximo» con tilde, el año 2026, la redacción de
las pp. 18 y 24, el hueco de numeración y las dos corridas de la batería.

**Listo cuando** los datos están en la lista y no queda ningún «⏸ ESPERA A JC».

**Estado:** abierto.

### T-15 · Claude Code, después JC · JC-R10: escanear dos tickets con el celular

**Qué:** la única prueba manual de la lista original que sigue abierta. Esta PC no tiene cámara.
1. **Claude Code**, cuando Codex no esté trabajando: generar dos tickets con el código actual y
   decodificarlos con `tools/VerificarQrTickets.py`. Medir el módulo y la zona de silencio como el
   04/11 y avisarle a JC. El ticket cambió en 6A (V-4) y desde entonces no se volvió a medir.
2. **JC:** escanear los dos con el celular.

**Listo cuando** el celular lee dos códigos `EA-…` distintos y coinciden con la base. Se anota en
`docs/PRUEBAS-PENDIENTES.md`.

**Estado:** abierto.

### T-16 · JC · JC-R12: las cuatro pantallas sin WebView2

**Qué:** la prueba manual de `docs/PRUEBAS-PENDIENTES.md` (al final) y de la auditoría, sección 4.
Hay que abrir las cuatro pantallas con la vista web forzada a fallar, al 100 % y al 150 %.

**Listo cuando** las cuatro muestran su versión de Windows Forms completa, con el aviso y sin
error, a las dos escalas.

**Estado:** abierto. T-03 se cerró el 26/11 sobre el ejecutable; esta es la prueba manual de JC.

### T-17 · Máximo · La tarde en Word

**Qué:**
1. completar los marcadores con *Reemplazar todo*;
2. pegar lo que estaba en pausa;
3. regenerar el índice;
4. corregir las páginas que se citan a mano y revisar los pies.

Es «Hacé esto último» de `docs/LISTA-PARA-MAXIMO.md`.

**Listo cuando** buscar `[[` en el Word da cero, el índice está regenerado y el PDF exportado.

**Estado:** bloqueado por T-09 y T-11 a T-14.

---

## Mantenimiento

### T-18 · Claude Code · Commitear los documentos y herramientas del 24/11

**Qué:** sin commitear en la zona de Claude Code. Ninguno cambia cifras de la batería.
- `docs/LISTA-PARA-MAXIMO.md`: marcadores, decisiones del 24/11 y dos correcciones;
- `docs/CIERRE-DOCUMENTO.md`, `docs/AUDITORIA-RESPALDO-WEBVIEW2.md` y `docs/TICKETS.md`;
- `docs/ESTADO-AGENTES.md` y `docs/PRUEBAS-PENDIENTES.md` (JC-R12);
- `tools/Cierre/`.

**Listo cuando** JC revisó `git status` y autorizó el commit, con rutas explícitas.

**Estado:** cerrado: `0ddc0c4`, publicado en `origin/JC` el 24/11.

### T-19 · Claude Code · `tools/VerificarReglasAgentes.ps1` no corre

**Qué:** PowerShell lo bloquea con `UnauthorizedAccess`, por la directiva de ejecución.
Probablemente alcanza con correrlo con `powershell -ExecutionPolicy Bypass -File …`, sin cambiar
la directiva del equipo. Confirmarlo y anotar la forma de correrlo.

**Listo cuando** el script corre y la forma de correrlo está escrita en su encabezado o en
`CLAUDE.md`.

**Estado:** abierto.
