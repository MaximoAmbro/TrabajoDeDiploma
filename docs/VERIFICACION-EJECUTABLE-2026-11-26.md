# Verificación sobre el ejecutable de los ocho tickets «en revisión» (26/11/2026)

Claude Code, 26/11/2026, entre las 13:09 y las 13:29, con Codex parado y el escritorio libre. Pedido de JC:
verificar ejecutando T-03, T-23 y T-26 primero, y después T-02, T-21, T-22, T-24 y T-25.

**Va aparte de `docs/TICKETS.md` a propósito:** mientras se hacía esta verificación aparecieron cambios sin
commitear de otro agente en la zona de Claude Code: `docs/TICKETS.md` a las 13:12, `docs/04-casos-prueba.md`
y `docs/LISTA-PARA-MAXIMO.md` a las 13:13. Por la regla de convivencia, Claude Code no los toca hasta que
JC decida. Cuando lo haga, este resultado se pasa a TICKETS.

## Resumen

| Ticket | Resultado | En una línea |
|---|---|---|
| T-03 | **Pasa** en el ejecutable; **la prueba automática falla** | Las 14 pantallas caen a su versión de Windows Forms, sin error ni entradas en `errores.log`. El panel del Administrador ya no queda en blanco. Pero CP-359 falla por la renumeración sin commitear (ver T-02) |
| T-23 | **Pasa** | Los cuatro casos, y además el CBU inválido, muestran el mensaje de la regla |
| T-26 | **No pasa** | El título ya cambia, pero dentro de «Mi cuenta» la compra no se puede completar: «Armar paquete», el total y «Continuar al pago» quedan fuera de la ventana. Lo causa `0160b55` |
| T-02 | **No pasa** del todo | CP-357 pasa. **CP-359 falla**: después de renumerar, sigue filtrando por el nombre viejo `CP199_…`. No hay parámetro de capturas del respaldo. No se probó 144 DPI |
| T-21 | **Pasa** | Con una base con solo `01-esquema.sql`, el `Visual.exe` real pide `02-semilla.sql` |
| T-22 | **Pasa a medias** | El ticket después del pago se recupera bien. **La exportación del reporte sin iText muestra «Ocurrió un error inesperado…»** |
| T-24 | **Pasa** | El selector toma fecha y hora; un evento elegido a las 21:00 queda a las 21:00 en la base |
| T-25 | **Pasa** | La auditoría muestra 13:26 local (16:26 UTC) |

## Cómo se ejecutó

- **Compilación:** `tools/Proyecto.ps1 -Accion Compilar` sobre `HEAD` `2be9da5`, a las 13:09. Pantalla de
  1920 × 1080 a 96 DPI (100 %); área útil de 1920 × 1032.
- **Copias de `Visual\bin\Debug`** en la carpeta temporal de Claude Code, con la configuración apuntando a
  `BddEventAura_Pruebas` (la que la batería recrea en cada corrida):
  - `completa`, sin quitar nada;
  - `sindll`, sin `Microsoft.Web.WebView2.Core.dll`, `Microsoft.Web.WebView2.WinForms.dll`,
    `Newtonsoft.Json.dll` ni los tres `runtimes\*\native\WebView2Loader.dll`;
  - `sinitext`, sin los 13 `itext*.dll`.
- **Verificador:** `Verificar.cs`, copiado junto a la evidencia. Es un programa que corre dentro de cada copia,
  instala `ManejadorErrores` como `Program.Main` y abre los formularios reales de `Visual.exe`:
  - cuando existe un botón, desde ese botón del menú o de la pantalla (`PerformClick`), como el usuario;
  - en la vista web, con `click()` sobre el elemento de la página;
  - registra las vistas web, los avisos, los cuadros de mensaje, lo nuevo en `errores.log` y una captura de
    cada ventana.
- **No se mandaron teclas ni clics al escritorio.**
- **Diferencia con la aplicación real:** el verificador entra con `GestorUsuarios.Autenticar` en lugar de
  usar la pantalla de acceso, y algunas ventanas sin botón propio en el recorrido se abren directo. Son las
  pantallas de entrada, «Eventos del local» y «Auditoría». Los formularios, su código y sus DLL son los del
  ejecutable.
- **T-21 se hizo con el `Visual.exe` real:** UI Automation leyó el cartel. Para el resto se intentó también
  con UI Automation, pero los campos de Windows Forms no exponen el patrón de valor y no se pudo escribir en
  ellos. Por eso se usó el verificador.
- **Evidencia:** `docs/verificaciones/ejecutable-2026-11-26/`, con 24 capturas, los registros `log-*.txt`,
  `Verificar.cs` y `Preparar.ps1`.

---

## T-03 · Sin las DLL de WebView2 ni Newtonsoft · ejecutado · pasa

**Qué hice:** con la copia `sindll`, abrí las 14 pantallas que tienen vista web: bienvenida, acceso, alta,
recuperación, catálogo, ficha y armador, historial, mis locales, eventos del local, control de ingreso,
reportes, panel del Administrador, auditoría y revisión de eventos. Después las abrí igual con la copia
`completa`, como control.

**Qué vi:**
- En las 14, la vista web quedó oculta («terminada», no «preparada») y se ve la versión de Windows Forms con
  sus datos: tarjetas, grillas e indicadores.
- **`errores.log`:** ninguna entrada nueva en todo el recorrido. Ningún aviso rojo ni cuadro de error.
- **El panel del Administrador**, que el 24/11 quedaba en blanco, muestra los 8 indicadores con el aviso
  «Vista web no disponible. Se muestran los indicadores de Windows.»
  (`t03-12-panel-administrador-sin-dll.png`).
- **Avisos:** 11 pantallas muestran «Vista web no disponible…». **El alta y la recuperación no muestran
  ningún aviso:** se ve el formulario clásico completo, sin explicación. La bienvenida pasa directo al acceso,
  que sí avisa. Es una diferencia, no un error: lo decide JC.
- Con la copia `completa`, las 14 cargan la vista web («VISIBLE, preparada»), también sin entradas en
  `errores.log`.

**Resultado:** pasa sobre el ejecutable, a 96 DPI. No se probó 144 DPI, porque exige cambiar la escala de
Windows. La prueba automática de este caso falla: ver T-02.

## T-23 · Mensajes de la solicitud de Organizador · ejecutado · pasa

**Qué hice:**
- Como Paula, abrí `frmSolicitudOrganizador` y pulsé sus botones con datos armados para cada caso:
  - CUIT `20123456780`, con el dígito verificador mal;
  - CBU `123`;
  - todo válido y el teléfono sin confirmar;
  - «Enviar código» y después «Confirmar teléfono» con `000000`.
- Para llegar a la verificación del teléfono hace falta un adjunto válido. El diálogo de archivos no se puede
  operar sin teclas, así que el verificador cargó la imagen con `ImagenEventoCodec.Preparar`, lo mismo que
  hace «Adjuntar documentación».
- Como Ana, en «Solicitudes de organizador», elegí la solicitud pendiente y pulsé «Rechazar» con el motivo
  vacío.

**Qué vi:** el aviso de la pantalla (`avisoSistema`) y sus capturas `t23-*.png`.

| Caso | Mensaje en pantalla |
|---|---|
| CUIT inválido | «El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.» |
| CBU inválido (extra) | «Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.» |
| Teléfono sin confirmar | «Verificá este teléfono con el código enviado antes de presentar la solicitud.» |
| Código incorrecto | «El código no es válido o venció. Solicitá uno nuevo.» |
| Rechazo sin motivo | «Escribí el motivo del rechazo.» |

`errores.log`: sin entradas. **Pasa.**

Detalle menor: el aviso rojo aparece arriba a la derecha y tapa el campo CUIT, que es el que hay que corregir
(`t23-1-cuit-invalido.png`). Desaparece a los 3 segundos.

## T-26 · El catálogo y la compra dentro de «Mi cuenta» · ejecutado · **no pasa**

**Qué hice:**
- Como Paula, abrí `frmMenuCliente`.
- Pulsé «Eventos disponibles» y, en la página del catálogo, el último «Ver entradas» (Duki).
- En la ficha, «Armar paquete».
- Medí la posición de cada control contra el área de la ventana, y dentro de la página con JavaScript.
  Lo repetí con la copia sin DLL.

**Qué vi:**
- **El título sí cambió:** «Eventos disponibles» y «Armar paquete». Esa parte del ticket está.
- **La ventana «Mi cuenta» es fija:** `FixedSingle`, sin maximizar, de 1200 × 760. El usuario no puede
  agrandarla.
- **Catálogo:** la vista web va de X 304 a 1240 en un área de 1200. La segunda tarjeta queda cortada 40 px a
  la derecha. «Volver» y «Ver como tabla» quedan debajo del borde (Y 742 a 778)
  (`t26-1-catalogo-en-mi-cuenta.png`).
- **Ficha:** «Armar paquete» queda en X 1024 a 1224 e Y 744 a 780, con un área de 1200 × 760: se ven 16 px
  del botón. Ningún contenedor tiene desplazamiento (`t26-2-ficha-sin-armar-paquete.png`). No probé la rueda
  del mouse; no hay barra.
- **Armador:**
  - la vista web mide 1280 × 720 y arranca en X 272, así que solo se ven 928 px de la página;
  - dentro de la página, «Continuar al pago» está en x 930 a 1216 y el total en x 930: **los dos quedan
    enteros fuera de la ventana**;
  - la página no se desplaza, porque su ancho de desplazamiento es 1280, igual que la vista
    (`t26-3-armador-sin-total-ni-continuar.png`).
- **Sin las DLL pasa lo mismo:** «Comprar» queda en Y 814 y X 1368, fuera del área.

**Causa (leída en el código):** `0160b55` («ampliar ventanas de pantallas migradas», 26/11 00:08) les puso
`MinimumSize` a tres pantallas:

| Pantalla | Tamaño mínimo |
|---|---|
| `FrmCompraCliente` | 1280 × 720 |
| `frmEventosCliente` | 1000 × 680 |
| `frmHistorialCliente` | 1000 × 680 |

Dentro de «Mi cuenta», `HostFormularioCliente` les da el área de contenido, de unos 960 de ancho. El mínimo
le gana, y la pantalla queda más grande que su contenedor y recortada. Sueltas se ven bien: por eso CP-357, que
las abre como ventanas, pasa.

**Resultado:** no pasa, y es peor que el 24/11. **El recorrido de compra de la sección 3 del guion (menú
lateral → Eventos disponibles → Ver entradas → Armar paquete → Continuar al pago) no se puede completar** en
la ventana de «Mi cuenta».

## T-02 · Prueba automática del respaldo · ejecutado · no pasa del todo

**Qué hice:**
- recompilé el árbol de trabajo (con los cambios sin commitear de Codex en `Pruebas/`);
- corrí con `vstest.console` las cuatro pruebas del respaldo y de la resiliencia: CP-357, CP-358, CP-359 y
  CP-360 (`TestResults/claude-verif-t02.trx`);
- busqué en `Pruebas/CapturarReforma.ps1` un parámetro de capturas del respaldo.

**Qué vi:**

| Prueba | Resultado |
|---|---|
| CP-357 | Correcta (13 s) |
| CP-358 | Correcta |
| CP-360 | Correcta |
| **CP-359** | **Incorrecta** |

El mensaje de CP-359: «Ninguna prueba coincide con el filtro… `FullyQualifiedName~CP199_RespaldoConDependenciasAusentesEnCopia`».
- **Causa:** la prueba se copia a una carpeta temporal y se vuelve a lanzar a sí misma filtrando por el nombre
  del método (`Pruebas/PruebasInteraccionPantallas.cs:204`). En la renumeración sin commitear, el método pasó
  a llamarse `CP359_RespaldoConDependenciasAusentesEnCopia`, pero el filtro no se cambió.
- El registro de Codex `TestResults/bateria-365-2026-11-26.md` dice que la batería se corrió **antes** de
  renumerar, y que los cambios posteriores «solo modifican identificadores y comentarios, no el cuerpo de las
  pruebas». **No es así:** con la renumeración, la batería da 364 de 365.

**Otros puntos del ticket:**
- `CapturarReforma.ps1` no tiene un parámetro para capturar el respaldo; sí tiene uno de escala.
- No se probó 144 DPI.

**Resultado:** no pasa hasta corregir el filtro. Se arregla con una línea: `nameof(CP359_RespaldoConDependenciasAusentesEnCopia)`,
o el nombre nuevo.

## T-21 · Base con solo `01-esquema.sql` · ejecutado · pasa

**Qué hice:**
- creé `BddEventAura_Verif01` y le corrí solo `BaseDatos/01-esquema.sql` (11 lotes, 20 tablas, `Parametros` y
  `Usuario` vacías);
- abrí el **`Visual.exe` real** de una copia apuntada a esa base y leí la ventana con UI Automation;
- después borré la base.

**Qué vi:** en la ventana «Iniciar sesión», el texto «Tu base de datos está desactualizada: falta la semilla
inicial con los parámetros y la cuenta Administrador. Corré BaseDatos/02-semilla.sql y volvé a abrir
EventAura.» `errores.log` no creció.

No quedó captura: otra aplicación estaba en primer plano y la imagen salió con esa ventana. Se descartó, y vale
el texto leído.

**Pasa.**

## T-22 · El ticket después del pago y el PDF del reporte · ejecutado · pasa a medias

**Qué hice,** con la copia `sinitext`:
1. **Reportes:** como Ana, en `frmReportes`, pulsé «Generar» y después «Exportar PDF».
2. **Compra completa en el armador web:** como Paula, en Duki, «+» en Campo, «Continuar al pago» y «Seguir
   sin seguro». En el pago simulado, tarjeta 4111…1111 y «Pagar».

**Qué vi:**
- **La compra: pasa.**
  - La compra quedó **Confirmada** en la base (pedido EA-005846; las compras de Paula pasaron de 4 a 5).
  - La confirmación dice: «El pago quedó confirmado como pedido EA-005846, pero no pudimos crear el PDF. Tus
    entradas y sus códigos se conservan en Mis entradas…» (`t22-2-confirmacion-sin-pdf.png`).
  - No se creó ningún archivo en «A1.Tickets EVENTAURA» (12 antes, 12 después) y `errores.log` no creció.
- **La exportación: no pasa.**
  - Aparece el aviso rojo **«Ocurrió un error inesperado y la operación no se completó. Si se repite, avisá al
    administrador.»** (`t22-1-exportar-sin-itext.png`).
  - `errores.log` registra `System.IO.FileNotFoundException en Visual.ExportadorPDF..cctor()`.
  - **Causa (leída en el código):** `ExportadorPDF` tiene campos `static readonly` de tipos de iText
    (`DeviceRgb`, líneas 29 a 33). Sin la DLL falla el constructor estático, y .NET lo envuelve en una
    `TypeInitializationException`. El `catch (FileNotFoundException)` de `btnExportar_Click` no la atrapa.
  - Por eso la prueba de Codex, que inyecta una `FileNotFoundException` directa, pasa, y la falta real de la
    DLL no.

## T-24 · «Agregar evento» con hora · ejecutado · pasa

**Qué hice:** como Organizador, abrí `frmAgregarEvento` a las 13:26. Cargué nombre, categoría y 29/11/2026
21:00, pulsé «Registrar» y consulté la base.

**Qué vi:**
- el selector, con formato `dd/MM/yyyy HH:mm` y valor inicial 26/11/2026 14:00, la próxima hora en punto;
- en la base, el evento 160, «Verificacion T24 132632», con fecha 2026-11-29 21:00:00, en borrador;
- el aviso «Evento guardado como borrador. Agregale un sector para publicarlo.»;
- `errores.log`, sin entradas.

**Pasa.**

## T-25 · La hora de la auditoría · ejecutado · pasa

**Qué hice:** como Ana, inicié sesión a las 13:26:41 locales (16:26:41 UTC) y abrí «Registro de auditoría».
Leí la página web y la grilla de respaldo.

**Qué vi:** la primera fila, «26/11/2026 13:26 admin@eventaura.com Inicio de sesión», en la página y en la
grilla (`t25-1-auditoria-hora-local.png`). **Pasa.**

---

## Otros hallazgos de esta verificación

- **`frmReportes` mide 1100 × 960 de área cliente**, con o sin vista web (`t03-11-reportes-sin-dll.png`).
  `CLAUDE.md` pone el techo en 790 de alto. Lo cambió `0160b55`. En esta pantalla de 1032 útiles entra; en
  una de 768, no.
- **Cambios sin commitear de otro agente en la zona de Claude Code** (13:12 y 13:13):
  - `docs/TICKETS.md` marca T-31 como «cerrado»;
  - `docs/04-casos-prueba.md` y `docs/LISTA-PARA-MAXIMO.md` registran CP-358 a CP-360, CP-364 y CP-365, y
    365 ejecuciones.

  Además:
  - En la fila de CP-358 de `04-casos-prueba.md`, «RNF-02» quedó en la columna de reglas.
  - **T-31 no está cerrado:** la renumeración rompe CP-359.

## Tickets para Codex que salen de acá (los transcribe Claude Code cuando JC lo autorice)

1. **T-26, reabierto, importa para la defensa.** Dentro de «Mi cuenta», las pantallas no pueden ser más
   grandes que el área de contenido.
   - **Qué hacer:** que `HostFormularioCliente` anule el `MinimumSize` de la pantalla que aloja, o que esas
     tres pantallas lo pongan solo cuando son ventanas sueltas. Después, revisar que el armador reparta su
     ancho en unos 960 px.
   - **Listo cuando**, en la ventana de 1200 × 760, se ven completos:
     - las tarjetas del catálogo;
     - «Armar paquete» de la ficha;
     - el total y «Continuar al pago» del armador.

     Hay una prueba que lo afirma con el menú, no con la pantalla suelta.
2. **T-31, falta una línea:** el filtro de `PruebasInteraccionPantallas.cs:204` con el nombre nuevo, mejor
   con `nameof`. Volver a correr la batería **después** de renumerar, y que el registro lo diga.
3. **T-22, la exportación:** atrapar también `TypeInitializationException`, o sacar los `static readonly`
   de iText de `ExportadorPDF`. La prueba tiene que quitar la DLL de verdad, como CP-359, no inyectar la
   excepción.
4. **`frmReportes` a 790 de alto como máximo** (`CLAUDE.md`, «Alto máximo de un formulario»).
5. **Para que decida JC:** si el alta y la recuperación tienen que avisar «Vista web no disponible» como las
   demás pantallas.

## Lo que no se probó

- **144 DPI (150 %):** exige cambiar la escala de Windows.
- **La rueda del mouse** dentro de «Mi cuenta».
- **El adjunto de la solicitud con el diálogo de archivos real.**
- **El inicio de sesión con la pantalla real:** en todos los casos salvo T-21, el verificador entra con
  `GestorUsuarios.Autenticar`.
