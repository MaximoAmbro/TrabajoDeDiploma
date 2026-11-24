# Auditoría: las vistas de respaldo en Windows Forms de las cuatro pantallas con WebView2

Claude Code, 24/11/2026, a pedido de JC. **Lectura de código, sin editar nada de `Visual/`.** Base:
`1d05fc2` más los cambios sin commitear de Codex en `Visual/` (los ocho archivos del 24/11). Lo
que sale de aplicar esto va para Codex: tickets T-02 a T-04 de `docs/TICKETS.md`.

**Resumen.** El respaldo está bien diseñado y funciona cuando lo que falta es el runtime: las cuatro
pantallas dibujan su versión de Windows Forms con los mismos datos que la web y un aviso. Codex lo
probó **una vez**, a mano, a las 11:36 del 24/11 (capturas en `TestResults/WebView2-Respaldo/`). Lo
que falta es que esa prueba quede en la batería. **Lo urgente es H-2, confirmado sobre el
ejecutable a las 14:35:** si faltan las DLL de WebView2 en la carpeta del programa, las cuatro
pantallas muestran un aviso de error en lugar del respaldo, y el panel del Administrador queda sin
dibujar.

Corrige lo que dije el 24/11 en `docs/CIERRE-DOCUMENTO.md`: no era cierto que «nadie la miró». Codex
la miró una vez. Lo que no hay es una prueba que la vuelva a mirar.

---

## 1. Cómo funciona el respaldo (`Visual/Web/PanelWeb.cs`)

- **La versión de Windows Forms es la que se ve primero.** Cada pantalla arma siempre sus controles
  de Windows Forms y de ellos saca lo que le pasa a la web (`ModeloWeb()`): las tarjetas del
  catálogo, `lblResumen` de la compra, `dgvReporte` y `lblLectura` de los reportes y los `DatoPanel`
  del panel del Administrador. La vista web empieza oculta y tapa a la de Windows Forms recién cuando
  la página confirma que dibujó (`Preparado`). **El respaldo no es un camino aparte: es la pantalla
  de siempre sin la capa web encima.**
- **`Desactivar()`** oculta el panel web, marca `fallida` y dispara `VistaNoDisponible`. Las cuatro
  pantallas se suscriben y muestran su versión de Windows Forms con un aviso. Se llama si:

| Falla | Dónde se captura | ¿Cae al respaldo? |
|---|---|---|
| Falta el runtime de WebView2 (`CreateAsync` lanza `WebView2RuntimeNotFoundException`) | `try` de `Cargar` | Sí |
| Falta `WebView2Loader.dll` (`runtimes\win-*\native`) | `try` de `Cargar` | Sí |
| Falta el `.html` en `Web\` | `File.Exists` dentro del `try` | Sí |
| La navegación falla | `NavigationCompleted` con `!IsSuccess` | Sí |
| El script no devuelve `true` (error de JavaScript, página vieja) | `Renderizar` | Sí, aunque la web ya se estuviera viendo |
| Se cae el proceso del navegador | `ProcessFailed` | Sí |
| La carpeta de datos (`%LOCALAPPDATA%\EventAura\WebView2`) no se puede escribir | `try` de `Cargar` | Sí |
| **Faltan `Microsoft.Web.WebView2.Core.dll` o `.WinForms.dll`** junto al ejecutable | — | **No: ver H-2** |
| La inicialización nunca termina (ni éxito ni error) | — | Se queda en Windows Forms, sin aviso. Aceptable |

## 2. Las cuatro pantallas

| Pantalla | Qué dibuja el respaldo | De dónde salen los datos | Aviso | ¿Lo ejercitan las pruebas? |
|---|---|---|---|---|
| Catálogo (`frmEventosCliente`) | Tarjetas `TarjetaEvento` en grilla, conmutador a tabla, ficha | `GestorEventos.Buscar` y `GestorLocales`: lo mismo que la web | «Vista web no disponible. Podés seguir usando las tarjetas y la tabla.», arriba de las tarjetas | **El camino sí:** CP-160 hace clic en «Ver entradas» de una tarjeta de Windows Forms y llega a la compra. **El aspecto no** |
| Resumen de la compra (`FrmCompraCliente`) | `lblResumen` (filas y TOTAL) y `lblTotalDestacado` | `GestorFidelizacion.CalcularDescuento`: `lblResumen` es la fuente de la web | «Vista web no disponible», a la derecha del título «Resumen» | **Los importes sí:** CP-13, CP-119 y otras leen `lblResumen`. **El aspecto no** |
| Reportes (`frmReportes`) | `dgvReporte` con formatos y fila de totales, y `lblLectura` | `GestorReportes.Ejecutar`: la misma tabla que la web | «Vista web no disponible. El reporte se muestra en la tabla.», encima de la grilla | **Los datos sí:** las pruebas de `PruebasGrupoAReportes` (CP-86 a CP-90 y CP-177 a CP-180) leen `dgvReporte` y `lblLectura`. **El aspecto no** |
| Panel del Administrador (`PanelAdministrador`) | Seis `DatoPanel` y los dos pendientes de revisión | Los cinco reportes y los dos gestores de revisión | «Vista web no disponible. Se muestran los indicadores de Windows.», arriba del período | **Ni el camino ni el aspecto:** con la web visible, los `DatoPanel` quedan ocultos y el control de su grilla en CP-357 queda vacío (H-5) |

**Lo que muestran las capturas de Codex del respaldo** (`TestResults/WebView2-Respaldo/`, 96 DPI,
base de pruebas, revisadas una por una): las cuatro están completas y sin superposiciones. El aviso
de los reportes queda 4 px debajo del botón «Generar».

---

## 3. Hallazgos para Codex

### H-1 · Alta · El respaldo no está en la batería

La prueba de las 11:36 no se puede repetir desde el repositorio: ni `CapturarReforma.ps1` ni
ninguna prueba fuerza la falla (en `Pruebas/` no aparece el nombre de ningún aviso:
`avisoCatalogoWeb`, `avisoCompraWeb`, `avisoReporteWeb`, `avisoAdministradorWeb`). Cualquier cambio
posterior puede romper el respaldo sin que nada lo note. **Ya pasó una vez:** el cambio sin
commitear de `frmReportes.cs` (`altoExtra`, hasta 144 px más de alto en `Shown`) mueve «Exportar»,
«Volver», `lblLectura` y la altura de `dgvReporte`, que es justo lo que se ve en el respaldo. Esa
versión no se capturó sin la web.

**Qué hacer:** una prueba nueva (CP correlativo) que fuerce la falla y, en las cuatro pantallas,
compruebe lo siguiente:
- el panel web está oculto y el aviso está visible;
- los controles de Windows Forms están visibles y con datos;
- no hay superposiciones, con el mismo control de CP-357;
- todo lo anterior a 96 y a 144 DPI.

**Ojo con `entorno`:** es `static` y queda en caché si se creó bien. Por eso, poner una variable de
entorno a mitad de la corrida no sirve si otra prueba ya creó el entorno. Hay dos salidas, y elige
Codex:
- un interruptor interno de `PanelWeb` que llame a `Desactivar()` sin intentar cargar;
- correr esa prueba en un proceso propio con `WEBVIEW2_BROWSER_EXECUTABLE_FOLDER` apuntando a una
  carpeta que no existe.

**Listo cuando** la batería tiene ese caso y pasa, y las capturas del respaldo se generan con un
parámetro de `CapturarReforma.ps1`.

### H-2 · **Alta, confirmado** · Si faltan las DLL de WebView2, no hay respaldo sino un error

**Comprobado el 24/11 a las 14:35, sobre el ejecutable.** Se copió `Visual\bin\Debug` a otra carpeta,
se borraron de la copia `Microsoft.Web.WebView2.*` y los tres `WebView2Loader.dll`, y se abrió el
`Visual.exe` de la copia contra la base de desarrollo, solo para mirar. Resultado, en las cuatro
pantallas: `System.IO.FileNotFoundException` en `PanelWeb.Cargar`, que no queda dentro de su `try`.

| Pantalla | Dónde salta (`errores.log`) | Qué se ve |
|---|---|---|
| Panel del Administrador, la primera pantalla después de iniciar sesión como Administrador | `PanelAdministrador.cs:47`, en `Load`, mientras Windows Forms crea los controles | **El panel queda sin dibujar:** solo el título y «Pendientes de revisión»; ni indicadores, ni lateral visible (se transparenta lo que hay detrás), ni aviso de respaldo |
| Reportes | `frmReportes.cs:77`, en `Shown` | Aviso rojo «Ocurrió un error inesperado y la operación no se completó…» encima de las fechas. La grilla y la lectura funcionan. Sin aviso de respaldo |
| Catálogo | `frmEventosCliente.cs:54`, en `Shown` (desde `OnShown`, línea 76) | Aviso rojo y **disposición sin ajustar**: queda el título «Mi cuenta» arriba, el contenido corrido y las tarjetas cortadas abajo. La ficha abre igual |
| Compra | `FrmCompraCliente.cs:252`, en `OnShown`, antes de `base.OnShown` | Aviso rojo sobre los pasos, título «Mi cuenta» arriba y pasos cortados. El resumen calcula bien |

Evidencia: `docs/verificaciones/sin-dll-webview2-2026-11-24/`, con las cuatro capturas y el tramo
de `errores.log`.

**Lo que había deducido el 24/11 a la mañana (se mantiene como explicación):**

`PanelWeb.Cargar` es `async void` y su máquina de estados guarda campos de tipos de WebView2, como
`TaskAwaiter<CoreWebView2Environment>`. Si `Microsoft.Web.WebView2.Core.dll` no está junto al
ejecutable, la excepción de carga se produciría **al llamar a `Cargar`**, antes de entrar al `try`.
Habría que ver dónde salta en cada pantalla:

| Pantalla | Dónde salta | Qué pasa |
|---|---|---|
| Catálogo y reportes | En `Shown` | `ManejadorErrores` muestra su aviso cada vez que se abre la pantalla |
| Panel del Administrador | En `Load` | Ídem |
| Compra | En `OnShown`, antes de `base.OnShown` | Además no corre `AjustarAreaVisible` y la disposición queda sin ajustar |

En los cuatro casos el aviso de respaldo no aparece, porque `InicializacionTerminada` sigue en
`false`.

**Cuándo pasa:** si a la defensa se lleva una copia de la carpeta del programa sin las DLL. Con
`bin\Debug` completo no pasa.

**Qué hacer:**
1. Que la carga no pueda lanzar fuera del `try`. `Cargar` tiene que ser un método **no asíncrono**
   que no mencione ningún tipo de WebView2 y que llame, dentro de un `try/catch`, a otro método
   asíncrono donde estén esos tipos. Si los tipos quedan en el mismo método, la excepción salta al
   compilarlo, antes del `try`. El `catch` captura `FileNotFoundException`, `FileLoadException` y
   `TypeLoadException` y llama a `Desactivar()`.
2. Una lista de qué tiene que llevar la carpeta de la defensa: `Visual.exe` y su `.config`, las DLL
   del proyecto, `Microsoft.Web.WebView2.Core.dll`, `Microsoft.Web.WebView2.WinForms.dll`,
   `Newtonsoft.Json.dll`, `runtimes\` y `Web\`.

**Listo cuando,** sin las DLL, las cuatro pantallas abren en Windows Forms con su aviso y sin el
aviso de error.

### H-3 · Baja · En una máquina sin falla, la pantalla cambia de aspecto al abrirse

Mientras la web carga, se ve la versión de Windows Forms; cuando la página termina de dibujar, la
web la tapa. En el primer arranque del runtime eso puede tardar uno o dos segundos y se ven dos
pantallas distintas, una después de la otra. En la defensa puede parecer un parpadeo o una falla.
**Qué hacer**, si JC lo considera necesario: durante la carga, mostrar la superficie vacía o un
estado «Cargando» en lugar de la versión de Windows Forms. No toca el respaldo: si la carga falla,
se muestra la versión de Windows Forms igual que hoy.

### H-4 · Baja · El aviso de los reportes queda muy cerca de «Generar»

`avisoWeb` se ubica en `dgvReporte.Top - Font.Height`, con todo el ancho de la grilla. A 96 DPI
queda 4 px debajo de «Generar» (`btnGenerar` llega a y = 276). A 144 DPI y con el `altoExtra`
nuevo no está medido. Lo cubre la prueba de H-1. Si se superpone, conviene ubicar el aviso en
relación con `btnGenerar.Bottom`.

### H-5 · Baja · Desde `d9f7f08`, CP-357 ya no controla la grilla de indicadores del Administrador

CP-357 revisa los `DatoPanel` con `Where(t => t.Visible)`. Con la web visible, `PanelAdministrador`
oculta `datos` y `pendientes`, así que en una corrida normal la lista queda vacía y la aserción no
controla nada. Se arregla con H-1, que corre ese control con el respaldo activo.

### H-6 · Baja · Cambiar a Windows Forms con la web ya visible: el código está bien, pero no está probado

Si el script falla en una actualización posterior, por ejemplo al cambiar un filtro del catálogo o
generar otro reporte, `Desactivar()` oculta la web y cada pantalla recalcula su versión de Windows
Forms. En `PanelAdministrador`, `Distribuir()` no calculaba la grilla mientras la web estaba visible
y la calcula en ese momento. El código es correcto en las cuatro, pero ninguna prueba lo ejercita.
Si Codex elige el interruptor de H-1, puede activarlo después de `Preparado` y cubrir también este
caso.

---

## 4. Prueba manual para JC (JC-R12, también en `docs/PRUEBAS-PENDIENTES.md`)

Con la aplicación compilada (`bin\Debug`), en una consola de PowerShell:

```powershell
$env:WEBVIEW2_BROWSER_EXECUTABLE_FOLDER = 'C:\no-existe'
& '.\Visual\bin\Debug\Visual.exe'
```

Abrir el catálogo, una compra, los reportes y el inicio del Administrador. **Resultado esperado:**
cada una muestra su versión de Windows Forms completa, sin superposiciones, con el aviso «Vista web
no disponible…» y sin ningún aviso de error. Repetir con Windows al 150 %. Al cerrar la consola, la
variable desaparece.
