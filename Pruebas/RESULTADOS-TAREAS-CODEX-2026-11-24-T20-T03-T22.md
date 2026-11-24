# Resultado de T-20, T-03, T-22 y cierre visual

Fecha: 24/11/2026. Rama `JC`.

## Cambios

- T-20: `txtContraseñaLogin.PasswordChar` queda en el Designer. El inicio captura errores del chequeo de esquema y mantiene un aviso dentro de la ventana con `Data Source` e `Initial Catalog`, sin credenciales; deshabilita el acceso. La comprobación de `Datos.dll` ahora se resuelve por reflexión para que su ausencia también caiga en ese aviso. `Datos/Conexion.cs` limita el tiempo de conexión a 3 s y los avisos de error transitorios a 3 s.
- T-03: `PanelWeb` ya no declara tipos de WebView2 ni Newtonsoft. Carga `PanelWebMotor` por reflexión, dentro de un `try`; si faltan esos ensamblados, conserva la vista WinForms y el aviso. La prueba CP-199 copia el resultado compilado, exige que estén las cuatro DLL/loaders y los borra antes de ejecutar el caso aislado.
- T-21: `VerificadorEsquema` distingue una base con `01` pero sin los parámetros ni la cuenta Administrador de una base sin esquema; en el primer caso nombra `02-semilla.sql`.
- T-22: la creación del PDF queda capturada después del pago confirmado. La confirmación conserva el número de pedido e informa que los códigos siguen en Mis entradas. La exportación de Reportes diferencia falta de permisos, dependencia/archivo ausente y archivo bloqueado.
- RPT-02: se reemplazaron las dos expresiones `STRING_AGG` con `FOR XML PATH`, orden y decodificación XML. CP-88 conserva el resultado esperado de las combinaciones, sus conteos e importes.
- Catálogo: disponibilidad verde, ámbar y gris; “Ver entradas” es un botón HTML. Los datos previos a crear el motor se transfieren a WebView2.
- Respaldo H-3: la vista muestra “Cargando vista…” mientras inicializa; desaparece al completar o fallar. H-4 posiciona el aviso de Reportes bajo “Generar”. CP-357 revisa geometría y transición al respaldo. H-5 verifica que los indicadores WinForms vuelvan a estar visibles. H-6 fuerza el fallo después de preparar WebView2.

## Verificación

- `tools/Proyecto.ps1 -Accion Probar`: 363/363 correctas sobre la base de pruebas. Las tres pruebas nuevas son CP-198 (aviso y contraseña), CP-199 (DLL ausentes físicamente) y CP-200 (ticket recuperable).
- Capturas: 25 a 96 DPI y 25 a 144 DPI, con el escalado real de Windows. Se usó `PrepararDemostracion` sobre `BddEventAura_Capturas_Demostracion`; la base `_Pruebas` no se usó para capturar. Se revisaron catálogo, Reportes, compra y panel del Administrador.
- Archivos: `TestResults/Capturas-Reforma/100/` y `TestResults/Capturas-Reforma/150/`. El registro de captura quedó en `%TEMP%\eventaura-demo-capturas-definitivas.log`.

## Límites

CP-198 inyecta una excepción en el punto de chequeo del inicio; no se ejecutaron en esta ronda los seis escenarios manuales separados (instancia/base inexistente, base vacía, ausencia de config, Datos y SNI). CP-199 sí borra físicamente las dependencias web en una copia temporal, pero verifica `PanelWeb` de forma aislada. CP-200 inyecta una falla de escritura después de confirmar; no se simularon por separado iText, QRCoder y los permisos de Documentos. Estas tres causas siguen el mismo `catch` de recuperación.

No se modificaron `Visual/App.config` ni `Pruebas/App.config`. No se hicieron cambios en `docs/` ni se pusheó.
