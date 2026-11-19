# Guion de demostración de EventAura

Guion para recorrer el sistema sin ayuda. Fue actualizado el 18/11/2026 y verificado contra los recorridos de recuperación, revisión de eventos, los formularios y la semilla. Los códigos, números de pedido y fechas concretas se generan durante el recorrido. El pago y las bandejas de correo y teléfono son simulados: no uses una tarjeta real ni esperes un correo externo.

## 1. Preparar y abrir

### Preparar una base de demostración

En PowerShell, desde `D:\proyectos\TrabajoDeDiploma`, compilá:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Proyecto.ps1 -Accion Compilar
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación; no sigas con la demo.' }
```

Para una **base nueva y vacía**, en SQL Server Management Studio ejecutá `BaseDatos/01-esquema.sql` y después `BaseDatos/02-semilla.sql`. Luego, en orden numérico, ejecutá `04-seguridad-ingreso.sql`, `05-check-estado-entradas.sql`, `06-prioridad-temporal.sql`, `07-indice-unico-seguros.sql`, `08-seguridad-cuenta-auditoria.sql`, `09-imagenes-eventos.sql`, `10-restringir-cantidad-vendida.sql`, `11-perfiles-administrador-organizador.sql`, `12-solicitudes-organizador.sql`, `13-revision-primer-evento.sql`, `14-primer-evento-aprobado.sql` y `15-motivo-rechazo-evento.sql`. `03-datos-reporte.sql` es una carga grande para mediciones y no hace falta para esta presentación. Antes de cada ejecución confirmá en SSMS que el selector apunta a la base de demostración. No ejecutes `01` ni `02` sobre una base que ya tiene datos.

En una base existente, no vuelvas a ejecutar `01` ni `02`: aplicá solo los scripts numerados que falten, en orden, sobre esa misma base. El script `15-motivo-rechazo-evento.sql` agrega el motivo de rechazo y debe aplicarse si falta.


Si usás la copia de demostración aislada que ya preparó el equipo, ejecutá `Pruebas/EjecutarBateriaSegura.ps1` solo antes de la demo: crea y siembra `BddEventAura_Pruebas`. **No lo vuelvas a correr en medio del recorrido**, porque recrea esa base y borra las compras de la demostración. Cerrá todas las ventanas de EventAura antes de preparar o reiniciar la base.

Abrí `Visual/bin/Debug/Visual.exe` (o la copia aislada que te haya indicado el equipo). Si aparece que la base está desactualizada, leé el primer requisito que falta, ejecutá el script nombrado en esa misma base y volvé a abrir. No edites `Visual/App.config` ni `Pruebas/App.config`.

### Cuentas de la semilla

| Perfil | Usuario | Contraseña | Alcance para la demostración |
|---|---|---|---|
| Administrador de plataforma | `admin@eventaura.com` | `Admin1234` | Solicitudes, eventos en revisión, reportes globales, auditoría y alta de administradores. No es dueño de los locales de la semilla. |
| Organizador de la semilla | `organizador@eventaura.com` | `Cliente123` | Locales, eventos, control de ingreso y reportes de sus propios eventos. |
| Cliente Plata | `plata@eventaura.com` | `Cliente123` | Compra y suscripción. |
| Cliente Oro | `oro@eventaura.com` | `Cliente123` | Segunda cuenta para privacidad entre clientes. |
| Cliente Bronce | `bronce@eventaura.com` | `Cliente123` | Cuenta con una solicitud de Organizador ficticia pendiente ya sembrada. |

Los dos accesos de gestión son distintos a propósito. El Administrador gobierna la plataforma; el Organizador administra únicamente sus locales. Los clientes compran, se suscriben y pueden presentar solicitudes, pero una solicitud pendiente no cambia su perfil ni concede permisos. Las cuentas y contraseñas son de la semilla local, no de producción.
## 2. Crear una cuenta, verificarla y recuperar el acceso

1. En el acceso, pulsá **Crear Cuenta**. Completá Nombre, Apellido, Mail, Contraseña y Confirmar Contraseña con datos propios de demostración. Usá una dirección nueva y una contraseña que supere el medidor; pulsá **Registrarse**. La vista cambia a **Bandeja simulada de correo** y muestra destinatario, asunto, código de seis dígitos y vencimiento. Esta bandeja es el formulario 26 del inventario: el código se consulta abriendo la bandeja, no se incluye en el mensaje de solicitud. **Importa** porque el sistema no habilita compras hasta verificar la dirección.
2. Copiá ese código en **Código de verificación** y pulsá **Confirmar correo**. **Vas a ver** `Correo verificado. Ya podés iniciar sesión y comprar.` y el botón **Volver al acceso**. Pulsalo e iniciá sesión con la cuenta recién creada. **Vas a ver** el inicio del cliente y la nueva cuenta en su nivel inicial.
3. Cerrá sesión. En el acceso pulsá **Recuperar contraseña**. Escribí el correo de la cuenta y pulsá **Enviar código**. **Vas a ver** `Si la cuenta existe, enviamos un código.` y se habilita **Abrir bandeja**. El mismo texto aparece con un correo inexistente; la respuesta tarda al menos un segundo en ambos casos. El código no aparece en ese formulario ni en el mensaje de respuesta.
4. Pulsá **Abrir bandeja**. **Vas a ver** la ventana **Bandeja simulada de correo**, con destinatario, asunto, código de seis dígitos y vencimiento. Es el equivalente local de abrir el correo de la persona dueña de la cuenta: el pedido de recuperación no revela el código. Copialo, cerrá la bandeja, ingresalo como **Código de recuperación**, escribí y confirmá la contraseña nueva, y pulsá **Restablecer contraseña**. **Vas a ver** `La contraseña se actualizó. Ya podés iniciar sesión.`. Volvé al acceso e iniciá sesión con la contraseña nueva. Cada código vence, se consume una sola vez y pedir otro invalida el anterior. **Importa** porque la recuperación conserva los controles de contraseña y el código solo se consulta desde la bandeja simulada.

## 3. Comprar de punta a punta

1. Iniciá sesión como Paula (`plata@eventaura.com` / `Cliente123`). En el menú lateral pulsá **Eventos disponibles**. En la tarjeta **Duki en Rosario**, pulsá **Ver entradas**. **Vas a ver** la ficha con portada generada, fecha, local y el mapa; el escenario aparece sobre las zonas de sectores. Al pasar el puntero por una zona se muestra su nombre, precio y disponibilidad.
2. Pulsá la zona **Campo** y después **Armar paquete**. **Vas a ver** el indicador `2 Armá`, la categoría Plata y el descuento de 5 %. En el armado elegí **2** entradas de Campo; marcá **Hospedaje** y **Traslado**; marcá **Seguro de entrada**. Dejá Merchandising y Experiencia VIP sin seleccionar para que el total sea fácil de seguir.
3. Comprobá el resumen. Con la semilla limpia, el total esperado es **$ 121.625,00**: entradas $50.000 menos $2.500 de descuento Plata, más Hospedaje $45.000, Traslado $12.000, cargo de servicio $7.125 y seguro $10.000. Pulsá **Comprar**.
4. En pago, completá los datos de prueba que acepta la simulación: titular `Paula Plata`, número `4111111111111111`, vencimiento futuro (por ejemplo `12/28`) y código `123`. Pulsá **Pagar**. **Vas a ver** el paso `3 Pagá`, el total a pagar y luego `4 Listo`, `¡Compra confirmada!`, el total y un pedido `EA-…`. **Importa** porque la aplicación muestra todo el importe antes de confirmar y entrega comprobantes por entrada. La tarjeta es de prueba; no se procesa ningún cargo bancario.
5. Pulsá **Abrir carpeta** y abrí los PDF de ese pedido. **Vas a ver** Duki, Campo, los datos de la entrada y un QR distinto para cada entrada. Cerrá el comprobante y la confirmación; la aplicación vuelve a **Eventos disponibles**.

## 4. Servicios opcionales, seguro y devolución

El recorrido anterior ya compra dos servicios y el seguro. Para mostrar la regla de que el seguro solo se contrata antes del pago, iniciá otra compra y dejá **Seguro de entrada** desmarcado. Al pulsar **Comprar**, el sistema pregunta:

> No agregaste el seguro de entrada. Solo se puede contratar ahora: después de pagar no se puede incorporar.

El título del cuadro es **Seguro de entrada**; elegí **Sí** para continuar sin seguro. No afirmes que el seguro se puede sumar después: no se puede.

Para mostrar la devolución completa necesitás una compra con seguro, aún confirmada y a más de 48 horas del evento. En **Mis entradas**, seleccioná esa compra, marcá **Devolver** en la entrada y pulsá **Devolver 1 entrada(s)**. Revisá el resumen y confirmá con **Sí**. **Vas a ver** `Devolución registrada. Se reintegran …`. La entrada queda reembolsada y ya no sirve para ingresar. En una base limpia, el evento Duki de la semilla está a 60 días; completá la compra de prueba y devolvela sin usar el QR.

## 5. QR de ingreso y privacidad entre clientes

Usá el perfil **Organizador de la semilla** (`organizador@eventaura.com` / `Cliente123`), que es dueño de los locales y eventos de demostración. En su menú pulsá **Control de ingreso**. Elegí uno de sus eventos con una entrada confirmada y todavía no usada; copiá el código del QR/PDF en **Código del ticket (lector USB o texto del PDF)** y pulsá **Registrar ingreso**. **Vas a ver** `Ingreso válido. Bienvenido.`. Volvé a registrar el mismo código: **vas a ver** `La entrada ya ingresó el {fecha y hora del primer ingreso}.` El administrador de plataforma no tiene locales y no hace el control de ingreso.

La privacidad entre clientes se demuestra con Oscar: iniciá como `oro@eventaura.com`, abrí **Mis entradas** y comprobá que solo aparecen sus compras. La aplicación no ofrece una acción para seleccionar a Paula. Si una operación intenta leer datos de otra cuenta, la regla responde `Solo podés operar sobre los datos de tu propia cuenta.`; ese intento forzado se verifica en la batería, no es un botón de la interfaz.
## 6. Suscripción, beneficios y lista de espera

1. Como Paula, abrí **Mi suscripción**. Si no tiene una activa, elegí **Mensual** y pulsá **Contratar**; confirmá **Sí**. **Vas a ver** el precio de $9.000, el aviso de que no se renueva automáticamente y la fecha de vencimiento. Los planes disponibles son Mensual ($9.000) y Anual ($90.000). La suscripción muestra sus beneficios: seguro bonificado, acceso a preventa exclusiva y prioridad temporal si la persona además está anotada en la lista de espera. El descuento por nivel sigue siendo el que muestra la barra superior.
2. **Seguro bonificado:** volvé a comprar una entrada y mirá el selector del seguro. Para un cliente con suscripción activa el seguro figura bonificado; el resumen de la compra permite comprobarlo antes de pagar.
3. **Preventa:** El Organizador puede fijar el fin de preventa al administrar uno de sus eventos. Antes de ese momento el evento se vende solo a suscriptores; la prueba de rechazo exacta está al final del guion. Después de la fecha, la venta queda disponible normalmente.
4. **Lista de espera:** abrí **Eventos disponibles**, elegí **Clásico Rosarino** cuando figure agotado y pulsá **Anotarme en la lista de espera**. **Vas a ver** `Te anotaste en la lista de espera de «Clásico Rosarino». Podés consultar tu posición. Si vuelve a haber entradas, te lo mostramos al iniciar sesión.`. Usá **Consultar mi posición** para ver `Tu posición actual en la lista es {N}.` y abrí **Inicio** para ver el aviso de lista de espera.
5. **Prioridad temporal:** cuando se devuelve una entrada de un sector agotado, durante 15 minutos el cupo liberado se reserva para suscriptores anotados en la lista de ese evento. La batería de pruebas verifica el rechazo de un no suscriptor durante esa ventana y su aceptación al vencer. No hay un botón para adelantar el reloj de la demostración; para observar las dos ramas en vivo hace falta que un administrador libere un cupo y esperar 15 minutos. No digas que la posición garantiza ser el único comprador ni que se manda un correo externo.

## 7. Tres perfiles y administración

### Solicitar ser Organizador

1. Iniciá sesión como una cuenta Cliente sin una solicitud pendiente, por ejemplo Paula (`plata@eventaura.com` / `Cliente123`). En el menú del cliente pulsá **Solicitar ser Organizador**.
2. Para reproducir el recorrido que probé, completá: responsable `Persona Ficticia de Demostración`, CUIT `30000000007`, teléfono `+5493410000000`, local `Local Ficticio de Demostración`, dirección `Dirección inventada 000, Rosario`, capacidad `500` y CBU `0000000000000000000000`. Son valores de demostración con dígitos verificadores válidos; no representan una identidad ni una cuenta bancaria. En **Adjuntar habilitación / constancia**, elegí el PNG de prueba `Pruebas/Capturas-Fase8b/despues-100/frmMenuCliente.png` (26 KB); se usa solo como imagen ficticia para probar el selector, no como documentación legal. El sistema valida el contenido y la comprime. No ingreses datos personales, CUIT ni datos bancarios reales.
3. Pulsá **Enviar código**. **Vas a ver** el código y el vencimiento en la bandeja telefónica simulada dentro del formulario. Copialo en **Código de verificación** y pulsá **Confirmar teléfono**. **Vas a ver** `Teléfono verificado.`
4. Pulsá **Enviar solicitud**. **Vas a ver** `Recibimos tu solicitud (N.º {id}). Tu cuenta sigue siendo Cliente mientras esté pendiente.` La solicitud no cambia el perfil ni abre las funciones de Organizador. En la prueba recorrí también la solicitud de Paula desde la pantalla del administrador y la aprobé; en la demo real, cada cuenta puede presentar una sola solicitud pendiente. La semilla trae además una solicitud ficticia pre-cargada para `bronce@eventaura.com` si querés mostrar la revisión sin crear otra.

### Revisar solicitudes — Administrador de plataforma

Cerrá la sesión del cliente e iniciá como Ana (`admin@eventaura.com` / `Admin1234`). En **Solicitudes de organizador**, seleccioná la solicitud. **Vas a ver** los datos del responsable y del local, el archivo adjunto y los datos de cobro. Escribí un motivo en el cuadro de revisión y pulsá **Aprobar y crear local**. **Vas a ver** `Solicitud aprobada y local creado.`. Al volver a iniciar sesión, esa cuenta pasa a Organizador y puede administrar el local creado. El rechazo requiere también un motivo escrito.

### Locales, eventos e ingreso — Organizador

Iniciá como el Organizador de la semilla. Pulsá **Registrar local** para crear un local propio, o **Mis locales** para elegir uno existente y abrir sus eventos. En **Agregar evento**, completá nombre, fecha futura y categoría; **Registrar** lo guarda como **Borrador**. Administralo, agregá al menos un sector con capacidad positiva y luego pulsá **Publicar**. Agregar el sector no publica el evento.

El Organizador de la semilla ya tiene una publicación aprobada, así que sus siguientes eventos pasan directamente a **Publicado** y aparecen en el catálogo. Para mostrar la revisión inicial, usá una cuenta que haya sido promovida desde una solicitud nueva: su primer evento queda **EnRevision**. No aparece en el catálogo ni acepta ventas. El Administrador lo encuentra en **Eventos en revisión**, selecciona el evento y puede pulsar **Aprobar y publicar** o escribir un motivo y pulsar **Rechazar y devolver**. Tras rechazarlo, el Organizador lo ve en **Mis eventos** como **Borrador**, con el motivo en la columna **Motivo del rechazo**. Puede corregirlo y volver a pulsar **Publicar**; como todavía no tuvo un evento aprobado, vuelve a **EnRevision**.

El Organizador también puede controlar ingresos, administrar su lista de espera y abrir **Reportes**. Sus cifras se limitan a sus propios eventos. Cambiar de perfil a Administrador no concede propiedad sobre esos locales.

### Administración de plataforma

En el panel de Ana se muestran los cinco indicadores globales. **Reportes globales** consulta todos los eventos; **Registro de auditoría** muestra quién hizo qué, sobre qué entidad, cuándo y con qué resultado; **Crear administrador** crea otra cuenta privilegiada. El administrador de plataforma puede revisar solicitudes y aprobar primeros eventos, pero no administra locales ni controla ingresos como si fuera su dueño.
## 8. Lo que tiene que fallar

Probá estos rechazos solo con datos de demostración. Los textos entre comillas son el mensaje esperado; algunos agregan una fecha, nombre o cantidad dinámica indicada entre llaves.

| Qué intentar | Respuesta que tiene que aparecer |
|---|---|
| Iniciar sesión con un correo existente y contraseña incorrecta; repetir con uno inexistente | En ambos casos, exactamente `Mail o contraseña incorrectos.`. No se muestra el número de intentos restantes. |
| Comprar desde una cuenta sin correo verificado | `Verificá tu correo antes de iniciar una compra.` |
| Registrar dos veces el mismo QR | `La entrada ya ingresó el {dd/MM/aaaa HH:mm:ss}.` |
| Devolver una entrada ya ingresada | `No podés devolver una entrada que ya se usó para ingresar.` |
| Devolver sin seguro | `Solo se pueden devolver entradas con seguro, y el seguro se contrata únicamente durante la compra.` |
| Intentar agregar seguro después de pagar | No se ofrece esa operación; el seguro solo se decide durante el armado. |
| Un cliente leer/operar datos de otra cuenta | `Solo podés operar sobre los datos de tu propia cuenta.` |
| Controlar ingreso de un evento ajeno | `Solo podés controlar el ingreso de eventos de tus locales.` |
| Comprar durante la preventa con una cuenta no suscriptora | `«{evento}» está en preventa exclusiva para suscriptores hasta el {dd/MM/aaaa a las HH:mm}. Contratá una suscripción desde «Mi suscripción» o volvé cuando termine la preventa.` |
| No suscriptor intenta comprar un cupo liberado dentro de los 15 minutos de prioridad | `El lugar liberado está reservado temporalmente para suscriptores inscriptos en la lista de espera. Intentá al finalizar la ventana de prioridad.` |
| Suscriptor intenta anotarse cuando aún quedan entradas | `El evento todavía tiene entradas: la lista de espera se habilita cuando se agota.` |
| Anotarse en lista de espera para evento pasado o no publicado | `El evento ya pasó o no está publicado: no admite inscripciones en la lista de espera.` |
| Ingresar un código de recuperación incorrecto, vencido o ya consumido | `El código no es válido o venció. Solicitá uno nuevo.` |
| Registrar correo que ya pertenece a una cuenta | `Ya existe una cuenta registrada con ese mail.` |
| Usar contraseña por debajo del mínimo de fortaleza | `La contraseña no alcanza la fortaleza mínima. Sumá variedad de mayúsculas, minúsculas, números o símbolos.` |
| Intentar contratar una segunda suscripción activa | `Ya tenés una suscripción activa: no se pueden tener dos al mismo tiempo.` La interfaz puede agregar el plan y vencimiento actuales. |
| Comprar más entradas que las disponibles | `En el sector {sector} quedan {N} entrada(s).` |
| Comprar más de seis entradas en una operación | `Se pueden comprar como máximo 6 entradas por operación.` |
| Pagar servicios sin entradas | `El paquete debe tener al menos una entrada: los servicios no se venden solos.` |
| Cliente intenta abrir funciones de Organizador con una solicitud pendiente | `Esta operación es solo para el Organizador.` La cuenta conserva el perfil Cliente hasta la aprobación. |
| Organizador intenta revisar una solicitud de plataforma o aprobar un evento | `Esta operación es solo para el Administrador` |
| Administrador intenta realizar una operación exclusiva del Organizador | `Esta operación es solo para el Organizador.` |
| Publicar un evento sin sectores con capacidad positiva | `No se puede publicar el evento: necesita al menos un sector con capacidad mayor que cero.` El evento queda en Borrador. |
| CUIT con dígito verificador incorrecto | `El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.` |
| CBU con dígitos verificadores incorrectos | `Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.` |
| Enviar la solicitud sin confirmar el teléfono | `Verificá este teléfono con el código enviado antes de presentar la solicitud.` |
| Adjuntar un archivo que no sea PNG/JPEG o que supere 512 KB | `Adjuntá una imagen PNG o JPEG válida, comprimida y de hasta 512 KB.` |
| Cliente con solicitud pendiente intenta abrir locales | `Esta operación es solo para el Organizador.` |
| Organizador intenta aprobar su solicitud o su evento | `Esta operación es solo para el Administrador` |
| Rechazar una solicitud con el motivo vacío | `Escribí el motivo del rechazo.` |
| Vender un evento en revisión o no publicado | `El evento no está disponible para la venta: ya pasó o no está publicado.` |
| Aprobar otra vez un evento que ya no está en revisión | `El evento ya no está pendiente de revisión.` |
| Rechazar un evento en revisión sin escribir un motivo | `Escribí el motivo del rechazo.` El evento no cambia de estado. |
| Un Organizador intenta rechazar su propio evento o el de otro | `Esta operación es solo para el Administrador` |
| Comprar en evento pasado o no publicado | `El evento no está disponible para la venta: ya pasó o no está publicado.` |
| Usar QR inexistente | `El código no existe.` |
| Usar QR de otro evento | `La entrada corresponde a otro evento.` |
| Usar QR reembolsado | `La entrada está reembolsada.` |
| Usar entrada pendiente o vencida | `La entrada está pendiente o vencida.` |
| Registrar ingreso fuera de ventana | `Fuera del horario de ingreso o evento no disponible.` |

## 9. Duración y límites de la demostración

| Parte | Tiempo aproximado |
|---|---:|
| Preparación, compilación y acceso | 5–10 min si la base ya existe; más si hay que crearla |
| Registro, verificación y recuperación | 4–6 min |
| Compra completa y comprobantes | 5–7 min |
| Ingreso y devolución | 4–6 min |
| Suscripción y lista de espera | 4–6 min; 15 minutos adicionales para observar en vivo el vencimiento de prioridad |
| Solicitud de Organizador y revisión por plataforma | 7–10 min |
| Local y evento del Organizador, publicación y revisión del primer evento | 5–8 min |
| Reportes propios/globales, auditoría y alta de administrador | 5–7 min |
| Rechazos seleccionados | 3–5 min |

El recorrido completo dura unos **42–65 minutos**, sin contar preparación de base ni los 15 minutos de espera real de prioridad. Para una exposición de quince minutos, elegí compra y comprobante, ingreso de un QR, el panel del Organizador, y el panel del Administrador mostrando la solicitud/evento en revisión y la auditoría. El alta de cuenta, recuperación y la solicitud nueva pueden quedar para preguntas o una segunda pasada.

## 10. Cosas que no conviene presentar como si fueran otra cosa

- La bandeja de correo es simulada; no llega un mensaje a Internet. `EnviadorSmtp` existe, pero no está activado ni configurado.
- El pago está simulado; no hay autorización bancaria ni cargo real.
- La prioridad funciona con una ventana real de 15 minutos. Si no querés esperar, explicá la regla y mostrale los casos de la batería; no cambies la hora de la base y digas que transcurrieron 15 minutos.
- El rediseño de navegación de una sola ventana cubre el recorrido del cliente. Las pantallas de administración todavía se abren como formularios separados.
- Las portadas de la semilla son generadas. La carga de imagen propia se demuestra desde administración con un archivo de imagen válido.
- No fuerces errores editando la base durante la presentación ni uses una compra ya reembolsada para probar ingreso. Para el QR, prepará una entrada confirmada y no usada.
