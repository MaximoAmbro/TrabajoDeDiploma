# Renovación de interfaz: registro por fases

## Fase 1: mapa de sectores

Pantalla: `FrmCompraCliente`. Control nuevo GDI+: `MapaSectores`. Usa sectores, precios y disponibles existentes; no agrega reglas ni datos. Los ordena por precio descendente y luego ID, con escenario arriba, texto de disponibilidad y trama para agotados. La distribución es orientativa, no un plano numerado del local. El clic usa la misma celda y gestor que la compra existente; flechas/Enter ofrecen alternativa al mouse y la tabla sigue permitiendo cantidades de distintos sectores. La altura crece con los sectores y el contenedor conserva desplazamiento.

Designer modificado: únicamente `Visual/Cliente/FrmCompraCliente.Designer.cs`, para incorporar el control y distribuir mapa y cantidades. Sin fuentes, colores ni referencias a Tema.

Validación final:

- Batería: **278/278** (59,2934 s).
- Barrera: **49/49** (2,2659 s), incluye CP-63 en 20 pantallas, siete filas de serialización, dos filas de componentes adicionales y 20 filas de texto de botones.
- Color renderizado: **31 primarios, 20 pantallas, 0 fallos**, con el control negativo de la herramienta existente.
- Texto nativo: **20/20 al 100 % y 20/20 al 150 %**, DPI comprobados 96 y 144. Escala original restaurada.
- Nuevas pruebas: **CP-150**, orden por precio y rechazo del clic en agotado; **CP-151**, clic conectado al armado real. Ninguna prueba anterior modificada. Los IDs quedan pendientes de incorporar a la matriz por Claude Code; este trabajo no modifica docs/.

Evidencia DrawToBitmap completa, en `Pruebas/Evidencias-Interfaz/Fase1/`:

| Captura | Tamaño | Letra menor a 16 cm | A 24 cm |
|---|---|---|---|
| antes-100.png | 886 × 573 | 6,14 pt | 9,21 pt |
| antes-150.png | 1326 × 856 | 6,16 pt | 9,24 pt |
| fase1-100.png | 886 × 573 | 6,14 pt | 9,21 pt |
| fase1-150.png | 1326 × 856 | 6,16 pt | 9,24 pt |

**No insertar estas vistas completas a 16 cm.** El ancho previsto de inserción es **24 cm en página apaisada**; si el documento exige 16 cm, necesitan recortes separados. La fuente menor es 9 pt en pantalla; conversión: puntos de fuente × DPI × (ancho en cm / 2,54) / ancho de captura en píxeles. No se confunden píxeles con puntos impresos.

Vueltas de validación: dos baterías completas después de integrar el mapa; dos barreras específicas (antes y después del ajuste de altura); dos recorridos finales de capturas y dos barridos nativos de texto. Además se tomaron los dos recorridos de capturas anteriores. Estos son recorridos de herramientas, no horas-persona. Portón humano pendiente: aspecto del mapa y ancho de inserción del informe; no bloquea las siguientes fases autorizadas.

Publicado en `fed7918758bc78a7d553c4b6540065eeec909c5b`, verificado por `ls-remote` en main y JC.

## Fase 2: registro histórico del primer freno (resuelto en la continuación de abajo)

Se incorporaron la marca vectorial, el generador del icono de 16/32/48/64/256 (sin travesaño en 16), la selección de Segoe Fluent Icons con respaldo MDL2 y la barra contextual como UserControl. La barra tiene su Designer de medidas; los colores y las fuentes están en Tema. Se incorporó una pantalla a la vez, conservando la distribución interna y reservando la franja superior. Esto modifica los 20 Designer de formularios. No se modificó ninguna prueba anterior.

**Barrera específica:** 20 recorridos secuenciales, uno después de cada pantalla. Cada recorrido dio **49/49**, CP-63 en 20, siete filas de serialización, dos filas de componentes adicionales y 20 filas de texto. En cada recorrido también pasó el color renderizado: 31 primarios, 20 pantallas, cero fallos. Son 20 vueltas completas de esa herramienta, no una sola corrida al final.

**Casos nuevos observados verdes:** CP-152, la barra no serializa estilo en el host real; CP-153, las cinco resoluciones del icono. Todavía no se incorporan a la cifra de una batería cerrada.

**Primer rojo observado en la batería completa posterior:**

```text
Con error CP-15: Registro rechaza datos inválidos (variante 3) [30 s]
Error de Assert.IsTrue. La interfaz no terminó en 30 segundos.
Pruebas.PruebasInterfaz.EnSta(Action accion): línea 884
Pruebas.PruebasInterfaz.Registro_RechazaDatosInvalidosSinCrearUsuario(...): línea 216
```

La variante pone el correo en blanco y espera el aviso «Ingresá tu correo electrónico.». Las variantes 1 y 2 habían pasado. La causa no está determinada: no se afirma que sea preexistente ni se atribuye a una línea sin reproducirlo. El último cierre completo anterior fue **278/278 en fase 1**.

Se detuvo únicamente el proceso vstest de esta corrida (PID 4396, iniciado el 16/11/2026 a las 19:16:56), con sus descendientes, al advertir el fallo. La batería interrumpida **no tiene total final válido**. No se reintentó para obtener verde ni se corrigió el caso o su implementación: manda la instrucción de parar al primer rojo. Registro completo conservado en `TestResults/Visible/bateria-fase2.log`; las herramientas no alteran App.config.

Capturas de identidad: hay un recorrido parcial al 100 % en `TestResults/Visible/identidad-parcial-100/`, revisado visualmente en menú del cliente y compra. **No son evidencia final de fase 2:** faltan las capturas finales de las veinte y su par al 150 %, detenidas por el portón. Por lo tanto, tampoco se declara verificada su letra impresa. No se entregan como figuras finales.

**Fases 3 y 4 no iniciadas.** Tampoco se inició la pasada final de textos y estados vacíos. No hay scripts nuevos de base, reglas RN-43 en adelante ni cambios en entidades, gestores, repositorios o App.config. No se activó SMTP ni se almacenaron credenciales. No se modificaron docs/ ni tools/.

Estado preservado: fase 1 publicada; fase 2 en el árbol local sin preparar, commitear ni publicar. El listado completo de archivos queda en `Pruebas/Evidencias-Interfaz/estado-al-frenar.txt`. No hay ningún App.config preparado; ambos conservan S de skip-worktree.

Vueltas de validación de fase 2: 20 barreras específicas con color renderizado; un recorrido parcial de capturas; una generación de icono; una batería completa interrumpida. Portón pendiente: diagnosticar el timeout de CP-15 y autorizar la continuación después de este primer rojo. No se sumaron horas-persona ni se presentan estas vueltas como estimación humana.

## Continuación: fase 2 cerrada y ejecución desatendida

JC autorizó continuar sin esperar y pidió primero eliminar los bloqueos por diálogos. Se reprodujo la corrida completa: las ventanas eran del framework, con «No se encontró ningún usuario con ese ID.». La barra consultaba un usuario de sesión ya borrado por la limpieza de otro caso. CP-15 aislada había dado 4/4: no era un bloqueo de la validación del correo ni de la detección de fuente.

La barra ahora muestra **solo la marca** si no hay sesión o el usuario no existe. Las proyecciones opcionales de encabezado no autorizan compras ni modifican la sesión. Los gestores mantienen sus guardas y pruebas anteriores. Se corrigieron también dos defectos propios de distribución al 150 %: acciones de Agregar evento 24 px más arriba y espaciado compacto del menú administrativo. CP-69 y CP-85 pasan sin modificar sus casos.

Blindaje permanente:

- `Pruebas/EjecutarBateriaSegura.ps1`: procedimiento de batería desde ahora. Cierra antes y después los procesos Visual/CapturarTanda/EnsayoDemo cuyo ejecutable pertenece a este repositorio. No mata procesos de otras carpetas. Compila y ejecuta con límite de sesión de 180 segundos, configurable.
- `BaseDePrueba.Preparar`: aplica `ThrowException` a toda la aplicación de pruebas antes de crear controles, una sola vez por proceso. Es idempotente porque CP-109 vuelve a preparar la base. No se modificó CP-109.
- El host de capturas también usa ThrowException y deshabilita los cuadros de error del sistema; no necesita que alguien cierre un cartel si falla.
- La aplicación real conserva `Application.ThreadException` y `AppDomain.UnhandledException`. Registra tipo y traza en `%LOCALAPPDATA%/EventAura/errores.log`, sin guardar valores de entrada o contraseñas. Presenta el mensaje en un aviso dentro de la ventana, no en MessageBox. Deshabilita el cuadro de fallo de Windows para su proceso.
- Límite técnico: un fallo fatal en un hilo de fondo puede terminar el proceso; registrar `AppDomain.UnhandledException` **no lo vuelve recuperable**. No se promete que el programa pueda continuar tras cualquier excepción fatal. Para errores de interfaz recuperables, el aviso es no modal.

Referencia de la API y su alcance por aplicación: [Microsoft Learn, SetUnhandledExceptionMode](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.application.setunhandledexceptionmode?view=netframework-4.8).

Pruebas nuevas de esta fase:

| Caso | Qué demuestra |
|---|---|
| CP-152 | Barra en host real sin estilo serializado |
| CP-153 | Icono con 16, 32, 48, 64 y 256 |
| CP-154 | Sesión cuyo usuario fue limpiado: solo marca, sin excepción |
| CP-155 | Sin sesión: solo marca y pintado válido |
| CP-156 | Un subproceso con excepción real en Load termina en menos de 5 s, sin quedar esperando un cartel; ensayo observado 262 ms |
| CP-157 | Manejador muestra aviso dentro de la ventana, sin detalle técnico ni bloqueo |

Cierre: **284/284 (57,4098 s)**. Las 49 filas de barrera pasan; color renderizado 31/31 primarios en 20 pantallas; texto nativo en 20/20 a 96 y 144 DPI. No hay cambios en pruebas anteriores de negocio. La única modificación al soporte de pruebas es el modo de excepciones en la inicialización y los hosts/procedimientos descritos.

Capturas completas antes/después de las veinte pantallas, DrawToBitmap, al 100 % y 150 %: `Pruebas/Evidencias-Interfaz/Fase2/`. Cada carpeta contiene `tipografia.tsv` con DPI, fuente mínima, letra a 16 cm y **ancho mínimo para 8,1 puntos**, redondeado hacia arriba a 0,1 cm. Para las vistas anteriores se usa el mínimo conservador de 9 pt del tema; para las posteriores se inspeccionan las fuentes de controles visibles y se incluye Ayuda de los componentes GDI+.

**Aclaración de fase 1 solicitada por JC:** sus cuatro figuras requieren **21,1 cm** para 8,1 puntos. Los 24 cm sugeridos antes daban margen adicional, no eran el mínimo. A 16 cm siguen sin ser aptas como figuras completas.

Vueltas de continuación: una corrida aislada de las cuatro variantes de CP-15; una reproducción completa del bloqueo; dos corridas completas de diagnóstico/corrección; una batería final de cierre; dos dirigidas del blindaje (la primera reveló el comportamiento fatal de ThrowException y motivó ensayarlo en subproceso); una barrera final de 49 filas y color; dos capturas finales y dos barridos nativos de texto. Sin portones humanos para seguir: las próximas fases están autorizadas. Portón del documento: respetar los anchos de cada TSV; no pegar a 16 cm las figuras que indiquen menos de 8 puntos.
