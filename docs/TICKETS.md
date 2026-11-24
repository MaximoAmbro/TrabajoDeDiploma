# Tickets: lo pendiente hasta la entrega

**Lo escribe solo Claude Code. Codex lo lee y no lo modifica**: informa el avance en su reporte
final, JC se lo pasa a Claude Code y Claude Code actualiza el estado acá. JC puede corregir
cualquier ticket cuando quiera.

Última actualización: **24/11/2026**, por Claude Code.

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

**Orden:** T-03 y T-20 primero (urgentes); T-01 a T-05 antes de congelar el código; T-06 congela; de T-08 en adelante, con el
código congelado.

---

## Resumen

| ID | Dueño | Qué | Estado |
|---|---|---|---|
| T-01 | Codex | Terminar y commitear la tanda visual en curso (gráficos y disponibilidad) | en curso |
| T-02 | Codex | Prueba automática del respaldo de WebView2 | abierto |
| T-03 | Codex | **URGENTE, confirmado:** sin las DLL de WebView2 o sin `Newtonsoft.Json.dll`, error en lugar del respaldo | en revisión: Codex lo informa hecho en `5b6fd5a` (CP-199); falta verificarlo sobre el ejecutable |
| T-04 | Codex | Carpeta de la aplicación para la defensa, completa y comprobada | abierto |
| T-05 | Codex | Lo que la reforma visual dejó declarado como fuera de conformidad | abierto (a decidir por JC) |
| T-06 | Codex | Congelar el código | bloqueado por T-01 a T-05 |
| T-07 | Claude Code | Las dos filas del catálogo con reglas distintas de su prueba | cerrado (24/11) |
| T-08 | Codex y Claude Code | Batería final, dos corridas | bloqueado por T-06 |
| T-09 | Claude Code | Valores de los marcadores `[[...]]` y las tres tablas | bloqueado por T-08 |
| T-10 | Claude Code o Codex | Capturas finales | bloqueado por T-06 |
| T-11 | Claude Code | Páginas de pantallas contra la interfaz final | bloqueado por T-10 |
| T-12 | Claude Code | Diagramas de clases 4, 5, 7 y 8d | bloqueado por T-06 |
| T-13 | Claude Code | WebView2 en la arquitectura (p. 57) y la bibliografía (p. 112) | bloqueado por T-06 |
| T-14 | JC | Los datos que faltan para el Word | abierto |
| T-15 | Claude Code, después JC | JC-R10: escanear dos tickets con el celular | abierto |
| T-16 | JC | JC-R12: las cuatro pantallas sin WebView2 | bloqueado por T-03 |
| T-17 | Máximo | La tarde en Word: marcadores, pausas e índice | bloqueado por T-09, T-11 a T-14 |
| T-18 | Claude Code | Commitear los documentos y herramientas del 24/11 | cerrado (`0ddc0c4`) |
| T-19 | Claude Code | `tools/VerificarReglasAgentes.ps1` no corre | abierto |
| T-20 | Codex | **URGENTE:** sin base, la pantalla de inicio muestra la contraseña y no avisa nada | cerrado: verificado el 24/11 sobre el ejecutable (sin config y con la instancia inexistente) |
| T-21 | Codex | El cartel de esquema sin `02-semilla.sql` pide correr `04` | en revisión: Codex lo informa hecho en `5b6fd5a` |
| T-22 | Codex | El ticket se genera sin `try` después del pago confirmado; mensajes de PDF que no corresponden | en revisión: Codex lo informa hecho en `5b6fd5a` (CP-200) |
| T-23 | Codex | **Para la defensa:** los rechazos de la solicitud de Organizador dicen «error inesperado» | abierto |
| T-24 | Codex | «Agregar evento» no deja elegir la hora | abierto |
| T-25 | Codex | El registro de auditoría muestra la hora UTC | abierto |
| T-26 | Codex | El catálogo y la compra quedan corridos dentro de «Mi cuenta» | abierto |

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

**Estado:** en curso.

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

**Estado:** abierto.

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

**Estado:** abierto, **urgente**.

### T-04 · Codex · Carpeta de la aplicación para la defensa

**Qué:** dejar escrito, en `Pruebas/GUIA-PUESTA-EN-MARCHA-MAXIMO.md` o en el reporte, qué lleva la
carpeta del programa que se usa en la defensa:
- `Visual.exe` y su `.config`, y las DLL del proyecto;
- `Microsoft.Web.WebView2.Core.dll`, `Microsoft.Web.WebView2.WinForms.dll` y `Newtonsoft.Json.dll`;
- las carpetas `runtimes\` (con `WebView2Loader.dll`) y `Web\` (once archivos).

Comprobar que una copia de esa carpeta, en otra ubicación, abre las cuatro vistas web.

**Listo cuando** la lista está escrita y la comprobación figura en el reporte.

**Estado:** abierto.

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

**Estado:** abierto, a decidir por JC.

### T-06 · Codex · Congelar el código

**Qué:** cuando T-01 a T-05 estén cerrados, avisar que no hay más cambios de código y dar el hash
del commit congelado. Desde ese momento, en `Visual/`, `Repo/`, `Datos/` y `Pruebas/` solo entran
correcciones de fallas, y cada una se anota acá como ticket nuevo.

**Listo cuando** JC confirma el congelamiento y el hash queda anotado en `docs/ESTADO-AGENTES.md`.

**Estado:** bloqueado por T-01 a T-05.

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

**Listo cuando** las 60 se rehicieron sobre el hash de T-06 y JC las aprobó.

**Estado:** bloqueado por T-06.

### T-11 · Claude Code · Páginas de pantallas contra la interfaz final

**Qué:** las páginas en pausa (⏸) de la lista:
- p. 45, pp. 47 a 49, pp. 51 a 53 y 55;
- las especificaciones CU-01 a CU-21;
- el párrafo de usabilidad de las pp. 90 a 97;
- el anexo «Pantallas del sistema».

En cada una hay que revisar los nombres de botones y menús, los controles y los mensajes. En las
cuatro vistas web, las tablas de controles se redactan de nuevo.

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

**Estado:** abierto, **urgente**.

### T-21 · Codex · El cartel de esquema sin `02` pide la migración equivocada

**Qué:** auditoría de riesgos, 2.4. **Ejecutado.** Con `01-esquema.sql` y sin `02-semilla.sql`, el cartel
dice «falta el parámetro INGRESO_HORAS_ANTES. Corré 04-seguridad-ingreso.sql», pero lo que falta es
`02`. `VerificadorEsquema` debería comprobar primero que existan los parámetros base y un
Administrador, y pedir `02-semilla.sql`.

**Listo cuando,** sobre una base con solo `01`, el cartel pide `02-semilla.sql`, y hay una prueba en
`PruebasEsquemaInicio`.

**Estado:** abierto.

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

**Estado:** abierto.

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

**Estado:** abierto. Importa para la defensa: si el jurado pide un CUIT inválido, hoy el sistema dice
«error inesperado».

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

**Estado:** abierto.

### T-25 · Codex · El registro de auditoría muestra la hora UTC

**Qué:** ejecutado el 24/11. La tabla `Auditoria` guarda la fecha en UTC: 19:52:24 cuando la PC marcaba
16:52. `frmAuditoria` la muestra sin convertirla a la hora local. El jurado ve tres horas de diferencia
con el reloj de Windows.

**Qué hacer:** convertir a hora local al mostrar (los filtros ya convierten de local a UTC desde `cfdad75`).

**Listo cuando** una acción hecha a las HH:mm locales figura a las HH:mm, y hay una prueba que lo comprueba.

**Estado:** abierto.

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

**Estado:** abierto.

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

**Estado:** bloqueado por T-03. Si se hace antes, el caso de las DLL no queda cubierto.

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
