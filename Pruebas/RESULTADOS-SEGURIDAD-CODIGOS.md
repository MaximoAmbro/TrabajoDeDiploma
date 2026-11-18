# Seguridad de los códigos de verificación y recuperación

La aplicación entregada usa `EnviadorSimulado`. Cada mensaje se escribe en un archivo `.eml` de la bandeja local de EventAura (`%LOCALAPPDATA%\EventAura\BandejaSimulada`), separado por destino y propósito. La vista que solicita el código devuelve siempre el mismo texto y no lo incluye. El código aparece solamente al abrir la bandeja simulada, que representa el paso de abrir el correo recibido.

La recuperación responde «Si la cuenta existe, enviamos un código.» tanto para una cuenta registrada como para una dirección inexistente. Ambas rutas tienen un mínimo de un segundo; CP-200 compara además los tiempos observados y exige una diferencia menor a 400 ms. Para una dirección inexistente se escribe un mensaje señuelo en la bandeja, pero no se persiste ningún código que permita cambiar una cuenta.

Los códigos vencen según `CORREO_CODIGO_MIN`, se almacenan verificados mediante hash, tienen límite de intentos y solo se consumen una vez. Emitir otro código invalida el anterior; el archivo de la bandeja se reemplaza por el último mensaje. Estas propiedades se cubren con CP-169 y CP-201. La verificación del teléfono de Organizador usa el mismo enviador y el mismo ciclo de emisión y consumo.

## Límite de la simulación

La bandeja simulada **no demuestra que quien usa el sistema sea titular del correo o del teléfono**: cualquier persona con acceso a esa máquina puede abrirla. Es una decisión de configuración para la versión entregada y la demostración, no una limitación de arquitectura: `IEnviadorCodigos` admite las estrategias `EnviadorSimulado` y `EnviadorSmtp`. SMTP existe, compila y no se activa en esta entrega. No se guardan credenciales de correo en el repositorio ni en archivos de configuración.

## Verificaciones

- CP-168: el alta permite consultar el código en la bandeja simulada, no en el mensaje de estado, y confirmar el correo.
- CP-169: un código vencido se rechaza sin cambiar la contraseña.
- CP-173: la recuperación completa se realiza desde el formulario y la bandeja.
- CP-200: la recuperación no distingue cuenta existente de inexistente por mensaje o tiempo observado; el código no aparece en la pantalla solicitante y el señuelo no recupera cuentas.
- CP-201: un código nuevo invalida el previo y el código vigente no se reutiliza.
- CP-202: el código telefónico solo aparece al abrir la bandeja simulada.
- CP-203: el alta nueva y el correo ya registrado tienen el mismo resultado visible; repetir el alta no modifica la contraseña existente.

La batería completa terminó en **347/347**. En esta tanda CP-12 se amplió de 25 a 26 formularios para incluir la nueva bandeja simulada; los 347 casos pasaron en 1 min 26 s de tiempo de pruebas. La primera corrida previa a ese ajuste contó 346/347 porque CP-12 conservaba el inventario anterior; no fue un fallo de negocio.

Las cuatro figuras regeneradas están en `docs/capturas/instructivo/` y en `Pruebas/Figuras-Instructivo-2026-11-16/100/`; muestran el recorrido de Paula y el total de $99.625. La evidencia al 150 % queda en `Pruebas/Figuras-Instructivo-2026-11-16/150/`.
