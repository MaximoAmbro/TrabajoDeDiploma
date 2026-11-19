# Seguridad de los códigos de verificación y recuperación

## Canal simulado

La versión entregada usa `EnviadorSimulado`. Cada dirección tiene un archivo `.eml` propio bajo `%LOCALAPPDATA%\EventAura\BandejaSimulada`; el nombre del archivo se deriva de un hash del destino. La interfaz indica la carpeta, pero no contiene un botón, formulario ni otra función para abrirla o leer su contenido. El archivo se abre con una herramienta externa, como el Explorador de archivos. El código no se devuelve en los mensajes de estado.

Esto cambia el límite de confianza respecto de la implementación anterior. Antes, cualquier persona que pudiera usar la aplicación podía apretar «Abrir bandeja» y leer el código desde otro formulario de EventAura: era un agujero dentro de la aplicación. Ahora, la lectura del archivo ocurre fuera de EventAura y requiere acceso al archivo de correo simulado, que representa el acceso al buzón del destinatario. La simulación no demuestra la titularidad del correo ni del teléfono si otra persona ya tiene acceso a esa máquina o a ese archivo. Ese límite pertenece a la configuración de la entrega simulada; la arquitectura permite reemplazarla por `EnviadorSmtp`, que existe y no se activa sin configuración. No hay credenciales SMTP en el repositorio ni en archivos de configuración.

Los códigos vencen según `CORREO_CODIGO_MIN`, se guardan en la base como hash, tienen límite de intentos y se consumen una sola vez. Emitir uno nuevo invalida el anterior; en el archivo de esa dirección se reemplaza el registro del mismo propósito. Las respuestas de recuperación son uniformes para correo existente e inexistente, con tiempo mínimo de un segundo. Para la dirección inexistente se escribe un señuelo sin una cuenta ni código utilizable.

## Auditoría

La emisión queda registrada en auditoría: verificación de correo, recuperación y verificación de teléfono. La recuperación anota el destino y si había una cuenta asociada, sin registrar el código. El cambio de contraseña por recuperación queda registrado al completarse; los rechazos de restablecimiento también se registran con el código de regla. El cambio autenticado de contraseña ya estaba auditado por `GestorUsuarios`. CP-168, CP-169, CP-170 y CP-202 comprueban emisión, recuperación correcta/rechazada y envío telefónico.

## Interfaz y pruebas

Se eliminó el formulario de bandeja, el botón «Abrir bandeja» y cualquier referencia de producción que permita consultar el mensaje o el código. El inventario vuelve a **25 formularios**. CP-12 fija ese total. CP-168, CP-173 y CP-202 comprueban que el código no aparece en los controles; CP-173 además barre todos los archivos C# de `Visual/` buscando rutas de acceso o presentación del código. La misma verificación estática se ejecuta desde una prueba de alta.

- CP-168: el registro escribe el mensaje en archivo y permite confirmar el correo ingresando el código leído fuera de EventAura.
- CP-169: un código vencido se rechaza sin cambiar la contraseña y deja registro de auditoría.
- CP-170: el código válido restablece la contraseña usando las validaciones y el hash vigentes; emisión y cambio quedan auditados.
- CP-173: recuperación desde la interfaz sin mostrar el código y con cambio de contraseña exitoso.
- CP-200: recuperación no enumera correos por texto ni por tiempo observado.
- CP-201: un código nuevo invalida al anterior y el vigente no se reutiliza.
- CP-202: el código telefónico se guarda fuera de la interfaz y su emisión se audita.
- CP-203: el alta nueva y el correo existente tienen respuesta uniforme y no exponen el código.

La batería completa posterior a estos cambios terminó en **347/347**, en **1,4447 minutos**. CP-12 confirma que el inventario vuelve a 25 formularios. No se modificó ninguna prueba de negocio.
