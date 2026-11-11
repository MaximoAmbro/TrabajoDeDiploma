# Seguridad: Paso 0 y SEG-1 a SEG-10 — 11/11/2026

Trabajo autorizado por JC en esta sesión, en orden SEG-3, SEG-1, SEG-2, SEG-4, SEG-5 y tareas menores/textos. **SEG-6 cerrado con 310.000 iteraciones**, autorizado por JC después de la medición inicial. Sin commits ni push. Trabajo detenido al terminar SEG-6: no se inició T-41 ni 6B.

## Punto de partida y herramientas

- Árbol limpio al comenzar, HEAD `e700f1b`.
- Paso 0 ejecutado antes de editar: **136 ejecuciones, 129 correctas, 7 fallidas**, 22,3374 s.
- Comando: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/Proyecto.ps1 -Accion Probar`. El bypass afecta únicamente a ese proceso: no se cambió la política de Windows.
- Instalación resuelta por `vswhere`: `C:\Program Files\Microsoft Visual Studio\2022\Community`; MSBuild y VSTest se resuelven desde esa instalación. VSTest 17.14.0 x64.
- Ningún App.config editado ni agregado al índice. Ambos conservan `S` de skip-worktree.

## Resultados por bloque

| Cierre | Ejecuciones | Correctas | Fallidas | Evidencia local ignorada |
|---|---:|---:|---:|---|
| Paso 0, sin cambios | 136 | 129 | 7 | Salida de la sesión |
| SEG-3 | 140 | 133 | 7 | `TestResults/seg3-final.log` |
| SEG-1 | 144 | 137 | 7 | `TestResults/seg1-final.log` |
| SEG-2 | 150 | 143 | 7 | `TestResults/seg2-final.log` |
| SEG-4 | 150 | 143 | 7 | `TestResults/seg4-final.log` |
| SEG-5 | 150 | 143 | 7 | `TestResults/seg5-final.log` |
| Menores, textos y pantallas | **153** | **147** | **6** | `TestResults/seguridad-final.log`, 27,4917 s |
| SEG-6 aplicado y verificado | **156** | **150** | **6** | `TestResults/seg6-final.log`, 39,8383 s |

Las tres ejecuciones añadidas al cerrar SEG-6 son CP-108, CP-109 y una segunda fila de CP-105 a 310.000. Se conserva la medición de 600.000 y se comprueba que un login no degrade ese hash a 310.000.

Compilación Debug sin errores ni advertencias de compilador nuevas. `git diff --check` sin errores; verificación de secciones compartidas de AGENTS/CLAUDE correcta.

Los seis fallos finales ya existían: CP-85 menú propietario a 150 %, CP-81 acceso rechazado a preventa que deja la compra visible, CP-69 alta de evento a 150 %, CP-71 color de la grilla, CP-88 lectura del reporte recortada y CP-76 beneficios de suscripción recortados. **No se corrigieron esas seis funcionalidades.** CP-75 pasa después del barrido porque el aviso ahora incluye `(RN-06)` y un título en castellano; se conserva su verificación de que el último sector no se borre.

## Implementación

- **SEG-3:** validación de rol y titular en los gestores del encargo; se protegen también el conteo privado de fidelidad, el cálculo del descuento y la consulta de inscripción. Las compras y los reembolsos releen al titular persistido: falsificar el `IdUsuario` de un objeto no habilita el acceso. `LiberarVencidas` sigue siendo mantenimiento idempotente sin sesión. `Fake.ComoCliente`/`ComoAdministrador` preparan las sesiones de las pruebas; los datos se siguen preparando con SQL aislado.
- **SEG-1:** firma de tres parámetros, contraseña actual y bloqueo por el mismo camino de autenticación. Un error de contraseña actual suma un intento y no revela si el mail existe; el bloqueo mantiene RN-04. El cambio no sustituye una sesión activa de otra cuenta. Se agrega el campo actual, se quitan los precargados y el enlace de inicio dice «Cambiar contraseña». Recuperación de contraseña olvidada por correo y restablecimiento administrativo siguen fuera de este encargo.
- **SEG-2:** `FechaIngreso` anulable, parámetros 6/12 horas, `GestorIngresos`, resultado con motivo y actualización atómica que admite un único ingreso. La entrada conserva su estado de venta. Pantalla `frmControlIngreso`, eventos publicados del día, lectura por teclado/Enter, color de resultado y contador. Acceso desde el menú administrativo. Una entrada reembolsada, pendiente/vencida, usada, de otro evento, inexistente o fuera de horario se rechaza.
- **SEG-4:** registro genérico interno, amistad con Pruebas y método público `CrearAdministrador` protegido por sesión administrativa. `RegistrarCliente` conserva el alta pública.
- **SEG-5:** hashes PBKDF2 en las cuatro cuentas de semilla y en los 1.178 clientes de reporte. Las credenciales de demostración no cambian. Regeneración: `powershell -NoProfile -ExecutionPolicy Bypass -File Pruebas/GenerarHashesSemilla.ps1 -ActualizarScripts`; usa `Seguridad.Hash` y verifica el resultado, sin conexión a bases.
- **SEG-6:** contraseñas nuevas a **310.000** y actualización de hashes anteriores al autenticar, sin degradar los de más iteraciones. Los scripts de semilla y de reportes se regeneraron con 310.000. Un hash de 100.000 generado antes de cambiar la constante sigue verificando: CP-108 comprueba además el login real, la actualización persistida a 310.000 y un segundo login que reutiliza ese hash.
- **SEG-7:** `[`, `%` y `_` se buscan literalmente con parámetros SQL.
- **SEG-8:** se captura OverflowException; se rechazan además iteraciones no positivas y longitudes de sal/hash inválidas.
- **SEG-9:** sin cambio de código, como pide el encargo. Para Claude: corregir en el entregable «QR de verificación» por «código QR de identificación». Sigue pendiente documental.
- **SEG-10:** comentario que prohíbe pasar datos del usuario a `CondicionAgotado`.
- **Textos:** avisos de reglas mediante `Avisos.Regla`, títulos en castellano, mensajes de validación y éxito con título y correcciones puntuales de idioma. La confirmación de suspensión mantiene Sí/No y No predeterminado; el código RN-11 pasa al cuerpo del aviso.

## SEG-6: formato, compatibilidad y mediciones finales

**Verificación previa al cambio:** `Seguridad.Hash` almacena `PBKDF2$iteraciones$salBase64$hashBase64`; `Verificar` lee las iteraciones del segundo componente y deriva usando ese valor. Antes de tocar la constante se ejecutó la implementación de 100.000 y se conservó su salida como fixture literal de CP-108. Con la constante ya en 310.000, esa misma salida acepta `Compatibilidad123` y rechaza una contraseña incorrecta. No se reconstruyó el hash viejo después del cambio para deducir compatibilidad.

CP-108 también guarda ese hash anterior en una cuenta aislada y llama a `Autenticar`: el login funciona, persiste un hash con 310.000, la misma contraseña verifica y el segundo login no vuelve a recalcularlo. **Caso correcto en la batería final.** No hubo pérdida de acceso a cuentas existentes.

CP-105 prepara cuentas aisladas con hashes reales de 600.000 y 310.000 y mide cinco llamadas completas a `Autenticar` para cada valor, incluidas las consultas a la base. El reloj del login no incluye la creación inicial del hash ni las consultas de las aserciones posteriores.

Primera medición anterior a la decisión: **997,13; 1001,07; 988,95; 989,60; 988,36 ms**. Una ejecución supera 1 s. Se propuso **310.000**, tal como indica el encargo para ese caso, y se consultó a JC. Última medición de la batería final: 1005,48; 1011,15; 996,79; 995,42; 991,93 ms. Evidencia: `TestResults/seg6-medicion-login.txt`.

Ese archivo corresponde a la batería anterior a aplicar SEG-6. Los resultados nuevos tienen sufijos de iteraciones para conservar ambos escenarios.

| Medición con 310.000 | Tiempo real |
|---|---:|
| Cinco logins completos | 509,79; 523,37; 509,42; 518,50; 508,90 ms |
| Login promedio / máximo | **513,99 / 523,37 ms** |
| Recrear base + esquema + semilla mínima | 167,77 ms |
| Cargar datos de reporte con 1.178 clientes | 241,57 ms |
| Preparación completa anterior + reporte | **409,34 ms** |
| Regenerar y verificar hashes de scripts, con arranque PowerShell | **2.346,95 ms** |

CP-109 mide una preparación secuencial real: recreación de `BddEventAura_Pruebas`, scripts 01, 02 y 03, incluyendo toda la carga de reportes. Después del reloj comprueba que hay exactamente 1.178 clientes de reporte con prefijo `PBKDF2$310000$` y cuatro cuentas de semilla con el mismo factor. Limpia el juego de reportes al terminar. No toca la base de desarrollo. Los 409,34 ms no incluyen la regeneración externa de los hashes: esta última se midió por separado y no ocurre en cada preparación.

Evidencias ignoradas: `TestResults/seg6-medicion-login-310000.txt`, `seg6-medicion-login-600000.txt`, `seg6-medicion-semilla.txt` y `seg6-regeneracion-hashes.txt`.

**Motivo de la decisión y precisión sobre la semilla:** JC autorizó 310.000 como compromiso: 3,1 veces las iteraciones anteriores y login medido debajo del segundo. OWASP mantiene 600.000 como recomendación para PBKDF2-HMAC-SHA256; 310.000 es la decisión del proyecto y no se presenta como cumplimiento de ese valor. [Fuente oficial verificada el 11/11/2026](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#pbkdf2).

La preocupación de JC por veinte minutos correspondería a calcular un hash por cada uno de los 1.178 clientes durante cada preparación. **El código actual no hace eso:** `03-datos-reporte.sql` inserta un hash de demostración ya calculado, compartido por ese juego de datos. El generador calcula dos hashes (administrador y clientes) y los verifica; SQL carga sus valores literales. Por eso no se confirmó ese costo de veinte minutos ni fue necesario bajar las iteraciones de los clientes de demostración. Las altas reales siguen generando una sal aleatoria por contraseña.

## Pruebas nuevas y cambios intencionales

| IDs | Cobertura |
|---|---|
| CP-91 a CP-94 | Reembolso e historial ajenos, eliminación por cliente, ausencia de sesión; incluye creación administrativa protegida y visibilidad del registro genérico |
| CP-95 a CP-98 | Contraseña actual incorrecta, bloqueo, mensaje uniforme, cambio correcto y conservación de sesión; inspección real de campos vacíos y enmascarados |
| CP-99 a CP-104 | Ingreso válido, segundo ingreso/fecha, reembolso, código inventado, evento/horario y dos escaneos concurrentes; pantalla real con rechazo y contador |
| CP-105, dos filas | Medición de cinco logins reales a 600.000 y a 310.000; sin degradar hashes más fuertes |
| CP-106 | Comodines de LIKE tratados como texto |
| CP-107 | Hashes malformados/desbordados rechazados |
| CP-108 | Hash previo de 100.000 verificable, login y actualización persistida a 310.000 |
| CP-109 | Preparación completa real y 1.178 clientes de reporte + cuatro cuentas de semilla a 310.000 |

Para Claude: registrar CP-91 a CP-109 en `docs/04-casos-prueba.md`, con las dos filas de CP-105; las RN-40 a RN-42 continúan rotuladas como propuestas. No se editó esa documentación.

Cambios intencionales a pruebas existentes:

- Preparación de sesiones en pruebas de negocio/interfaz, sin quitar aserciones de reglas, importes, persistencia o navegación. Las pruebas que verifican ausencia de sesión/login conservan ese escenario.
- CP-12 espera **20** formularios por la nueva pantalla, y sigue inspeccionando la altura y el Designer de cada uno.
- CP-67 verifica ahora que la semilla ya esté hasheada y mantiene doble acceso. Su variante de migración sigue probando una credencial heredada en un cliente creado por Fake.
- CP-15, CP-21, CP-44, CP-72/73/74/75, CP-78 y CP-82 actualizan títulos esperados para reflejar el barrido. Se mantienen los textos/reglas y los efectos persistidos que verificaban.

Capturas reales, ignoradas: `TestResults/Seguridad/cambiar-contrasena.png` y `control-ingreso.png`. Inspeccionadas; se corrigió el recorte de «Mostrar contraseñas». Esto no equivale a una validación del diseñador de Visual Studio ni de un lector físico.

## Base existente y pruebas manuales pendientes

- Migración preparada: **`BaseDatos/04-seguridad-ingreso.sql`**. Fue ejecutada dos veces correctamente solo contra `BddEventAura_Pruebas`, verificando idempotencia. No se ejecutó contra la base de desarrollo ni la de Máximo.
- Un intento con el ejecutable sqlcmd no conectó por su resolución de named pipe; la comprobación se hizo con SqlClient y la cadena de Pruebas, con guarda explícita `_Pruebas`.
- Antes de usar esta versión contra una base existente, JC/Máximo deben ejecutar la migración allí. Para Claude: incorporar esa preparación a `docs/PRUEBAS-PENDIENTES.md`.
- JC: abrir «Control de ingreso», elegir un evento del día dentro del horario, copiar/escanear un código válido y pulsar Enter: verde y contador +1; repetir: rojo, fecha del primer ingreso y contador sin cambio. Probar lector USB si está disponible; el escaneo físico no se simuló.
- JC: verificar cambio de contraseña desde el enlace nuevo, mostrando/ocultando los tres campos, cambio correcto y rechazo de la contraseña anterior.

## Designer y archivos

Cambios declarados: `frmCambiarContraseña.Designer.cs` agrega campo/etiqueta, ajusta posiciones necesarias, tabulación y texto del checkbox; `frmInicio.Designer.cs` cambia el texto del enlace; `frmMenuPropietario.Designer.cs` incorpora el acceso y compacta botones sin aumentar el alto. `frmControlIngreso.Designer.cs` es nuevo, con geometría literal a 96 DPI, componentes y sin colores/fuentes definidos en el Designer.

Archivos de implementación: scripts 01/02/03 y nueva migración 04; `Datos/RepositorioEntradas.cs`, `RepositorioEventos.cs`; `Entidade/Entrada.cs`, nuevo `ResultadoIngreso.cs`; gestores de Compras, Eventos, Fidelizacion, ListaEspera, Locales, Paquetes, Sesion, Suscripciones y Usuarios; `Seguridad.cs`, nuevos `AccesoPruebas.cs` y `GestorIngresos.cs`; formularios de inicio/cambio/alta, historial, pago, suscripción, administración de eventos/sectores/locales, reportes, menú administrativo y nuevo control de ingreso; `Visual.csproj`. Pruebas: Fake, sesiones/avisos esperados en las suites existentes, investigación de preventa (acento del éxito), nuevos `PruebasSeguridad.cs`, `PruebasIngresos.cs`, `MedirLoginSeguridad.cs` y generador de hashes.

Cambios ajenos detectados durante el trabajo y preservados: `docs/DOCUMENTO-ENTREGABLE.md` y nuevo `docs/PLAN-DOCUMENTO.md`. No se editaron.

## Textos informales: alcance y pendientes encontrados

El mensaje «Dale, son 3 cositos nomas cargalos viejo» de cambio de contraseña se eliminó. Se retiraron los precargados A/B/B, se normalizaron títulos y avisos de reglas y se corrigió «Usuario creado con exito» a «Usuario creado con éxito». «Show Password» se tradujo en cambio de contraseña.

**El barrido no dejó toda la aplicación en registro formal uniforme.** La búsqueda final de textos visibles encontró estos pendientes, que se informan sin empezar otra tarea:

| Lugar | Texto que permanece |
|---|---|
| `Visual/_Inicio/frmInicio.Designer.cs:82` y `frmCrearUsuario.Designer.cs:57` | `Show Password`, en inglés |
| `Visual/Cliente/FrmCompraCliente.cs:268` | «Te anotaste en la lista de espera…» |
| `Visual/Cliente/FrmCompraCliente.cs:413`, `:438`, `:468` | «Cantidad (escribí aquí)», «Escribí cuántas entradas querés…», «Revisá los datos» |
| `Visual/Cliente/frmSuscripcion.cs:52` | «No tenés una suscripción activa.» |
| `Visual/Cliente/frmHistorialCliente.cs:72`, `:99`, y su Designer | «Con seguro, podés devolver…», «Todavía no tenés compras.» |
| `Visual/Propietario/frmAdministrarEvento.cs:127` | «Por favor, selecciona un sector para eliminar.»; mezcla de tuteo con voseo en otras pantallas |

Los ejemplos con voseo son coloquiales y locales; no equivalen al mensaje despectivo eliminado. No se hallaron otras apariciones de «Dale», «cositos» o «viejo» en los archivos C# de Visual revisados. La búsqueda por términos no equivale a una revisión editorial exhaustiva de todas las pantallas/recursos.

## Tiempo real y límites de medición

No se cronometró la dedicación activa por separado en la primera sesión. Los intervalos de pared reconstruibles con las marcas UTC de los logs son: SEG-3 **al menos 0,050 h** (17:13:33–17:16:34, excluye preparación previa); SEG-1 **0,031 h**; SEG-2 **0,065 h**; SEG-4 **0,011 h**; SEG-5 **0,026 h**. Estos cuatro últimos intervalos van del cierre anterior al cierre propio. Paso 0: ejecución **0,0062 h**. La medición inicial de 600.000, los menores, textos y pantallas se hicieron después de 17:24:33 y antes del reporte inicial; no se inventa una división de horas activas entre esas tareas.

El cierre de SEG-6 en esta continuación se midió desde **17:38:31 UTC**, con reloj de pared, e incluye verificación previa, cambio, regeneración de scripts, pruebas, mediciones y actualización de este reporte. El tiempo de cierre se registra al finalizar abajo. No se inició T-41 ni 6B.
**Cierre medido de SEG-6: 2026-11-11 17:43:11 UTC; 280 segundos (0,0778 horas de pared) desde el inicio de esta continuación.** Incluye la batería final y el reporte; la calibración de la sesión anterior se conserva documentada aparte. Sin commits ni push.
