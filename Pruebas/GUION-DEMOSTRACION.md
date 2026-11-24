# Guion de demostración de EventAura

Guion para recorrer el sistema sin ayuda. Es el documento del operador: dice qué preparar, qué tocar y qué tiene que aparecer en pantalla. Los códigos, números de pedido y fechas concretas se generan durante el recorrido. El pago y las bandejas de correo y teléfono son simulados: no uses una tarjeta real ni esperes un correo externo.

**Estado de la verificación.** Desde el 24/11/2026 lo mantiene Claude Code; Codex ya no lo modifica.
- **Secciones 1 y 2:** reescritas y recorridas el 24/11/2026 sobre el ejecutable compilado con `5b6fd5a`, contra una base nueva armada con este procedimiento.
- **Secciones 3 a 8:** reescritas y recorridas el 24/11/2026 sobre el ejecutable compilado a las 16:29 (`942b451`), contra la misma base. Cada sección termina con lo verificado y lo que quedó sin verificar. La tabla de la sección 8 dice, fila por fila, qué rechazos se vieron en pantalla.
- Durante el recorrido (sección 2 a las 16:00; secciones 3 a 8 entre las 16:33 y las 16:59) no se bloqueó ninguna cuenta, no se cerró ninguna sesión por inactividad y `errores.log` no registró ningún error.

## 1. Preparar y abrir

### Compilar

En PowerShell, desde `D:\proyectos\TrabajoDeDiploma`:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Proyecto.ps1 -Accion Compilar
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación; no sigas con la demo.' }
```

El programa queda en `Visual\bin\Debug`. Para llevarlo a otra PC, copiá esa carpeta **entera**, con `Web\`, `runtimes\` y todas las DLL. Si falta un archivo, la aplicación falla o pierde las vistas web: ver `docs/AUDITORIA-RIESGOS-DEFENSA.md`, sección 4.

### Armar la base de demostración

Hace falta SQL Server 2017 o posterior. Todo esto se verificó el 24/11/2026 sobre SQL Server 2022 Express.

1. **Creá una base nueva y vacía cuyo nombre termine en `_Demostracion`**, por ejemplo `BddEventAura_Demostracion`. El sufijo es obligatorio: `semilla-demostracion.sql` se detiene con el error 51000 («Esta semilla se ejecuta solo en una base _Demostracion.») si la base se llama de otra forma. Es una protección para no cargar datos inventados en una base de trabajo.
2. **En esa base, en SQL Server Management Studio, ejecutá en este orden:**
   1. `BaseDatos/01-esquema.sql`
   2. `BaseDatos/02-semilla.sql`
   3. `BaseDatos/semilla-demostracion.sql`

   Antes de cada uno, confirmá que el selector de base de SSMS apunta a la base de demostración y no a `master`: `01-esquema.sql` no elige base y crea las tablas donde esté parado.
3. **No hace falta ningún script del 04 al 16.** `01-esquema.sql` ya trae el esquema completo, con todo lo que esos scripts agregan. Esos scripts son para actualizar una base vieja. Si los corrés igual sobre la base nueva, terminan sin error y no cambian nada que importe.
4. **`03-datos-reporte.sql` no hace falta.** Es una carga grande para mediciones: suma unos 1.200 usuarios y 4.500 entradas, y llena los indicadores del Administrador con cifras que no son las de la demostración. Si se corre, la semilla de demostración carga igual encima.

**Lo que deja la base:**
- la semilla base: 5 cuentas, el local «Sala Demostrativa EventAura» y los eventos Duki en Rosario, Clásico Rosarino (agotado) y Noche de Teatro Independiente (pasado);
- la demostración: 4 locales de Rosario, 20 eventos más, 337 entradas vendidas, 14 clientes `@demo.eventaura.example`, 3 personas en lista de espera y solicitudes de Organizador en varios estados.

**Las fechas son relativas al día en que se corre cada semilla.** Duki queda 60 días adelante, Clásico Rosarino 30, y los eventos de la demostración entre seis meses atrás y cuatro adelante. Corré las semillas en la semana de la presentación, no un mes antes.

**Verificado el 24/11/2026:**
- con `01` y `02`, el inicio no muestra el aviso de base desactualizada, y entran Ana y Paula;
- `semilla-demostracion.sql` corrida dos veces deja lo mismo que una sola: 23 eventos y 343 entradas;
- sobre una base con `03` también carga;
- sobre una base que no termina en `_Demostracion` se detiene con el error 51000 sin modificar nada.

### Una base que ya existe

No vuelvas a ejecutar `01` ni `02` sobre una base con datos. Si al abrir aparece que la base está desactualizada, el aviso nombra el primer script que falta: ejecutalo en esa misma base, volvé a abrir y repetí hasta que el aviso no aparezca.

No apuntes la demostración a `BddEventAura_Pruebas`: la batería de pruebas la borra y la recrea en cada corrida.

### Apuntar la aplicación a la base

La cadena de conexión está en `Visual.exe.config`, junto al ejecutable. En la copia que vas a usar, abrilo con el Bloc de notas y, en `connectionString`, poné:
- en `Data Source`, la instancia: `.\SQLEXPRESS`, `.` o `(localdb)\MSSQLLocalDB`;
- en `Initial Catalog`, la base del paso 1.

No edites `Visual/App.config` ni `Pruebas/App.config` del repositorio: guardan la configuración de cada máquina y no se publican.

### Abrir

Abrí `Visual.exe`. Hay tres resultados posibles:
- **Aparece el acceso con «Mail» y «Contraseña»:** la base está bien.
- **Aparece «EventAura no pudo comprobar la base de datos.»,** con la instancia y la base que intentó y sin los campos de acceso: SQL Server no está en marcha, la cadena apunta a algo que no existe o falta `Visual.exe.config`. Corregilo y volvé a abrir. Verificado el 24/11/2026 con la instancia inexistente (el aviso aparece en unos 3 s) y sin `Visual.exe.config`.
- **Aparece que la base está desactualizada:** ver «Una base que ya existe».

### Cuentas de la semilla

| Perfil | Usuario | Contraseña | Alcance para la demostración |
|---|---|---|---|
| Administrador de plataforma | `admin@eventaura.com` | `Admin1234` | Solicitudes, eventos en revisión, reportes globales, auditoría y alta de administradores. No es dueño de los locales de la semilla. |
| Organizador de la semilla | `organizador@eventaura.com` | `Cliente123` | Locales, eventos, control de ingreso y reportes de sus propios eventos. |
| Cliente Plata | `plata@eventaura.com` | `Cliente123` | Compra y suscripción. |
| Cliente Oro | `oro@eventaura.com` | `Cliente123` | Segunda cuenta para privacidad entre clientes. |
| Cliente Bronce | `bronce@eventaura.com` | `Cliente123` | Cuenta con una solicitud de Organizador ficticia pendiente ya sembrada. |
| Clientes de la demostración | `…@demo.eventaura.example` | `Cliente123` | Catorce clientes con historial de compras en los tres niveles. |

Ingresos verificados el 24/11/2026: Ana y Paula. El resto de las contraseñas sale de las semillas: los clientes de la demostración copian la de `bronce@`.

Los dos accesos de gestión son distintos a propósito. El Administrador gobierna la plataforma; el Organizador administra únicamente sus locales. Los clientes compran, se suscriben y pueden presentar solicitudes, pero una solicitud pendiente no cambia su perfil ni concede permisos. Las cuentas y contraseñas son de la semilla local, no de producción.

**Cuidado en vivo:**
- cinco contraseñas incorrectas seguidas bloquean la cuenta 15 minutos (RN-04);
- quince minutos sin usar la aplicación cierran la sesión, con aviso 60 segundos antes (RN-46).

## 2. Crear una cuenta, verificarla y recuperar el acceso

### La bandeja simulada: dónde están los códigos

Los códigos de verificación y de recuperación se escriben en archivos de correo, en:

```
%LOCALAPPDATA%\EventAura\BandejaSimulada
```

Abrí esa carpeta en el Explorador de archivos (pegá la ruta en la barra de direcciones) **antes** de empezar esta sección.

**La pantalla no muestra la ruta, a propósito.** El código prueba que quien lo usa tiene acceso al correo de la cuenta. Si EventAura mostrara dónde leerlo, cualquiera frente a la aplicación podría pedir la recuperación de una cuenta ajena e ir a buscar el código: es el agujero de recuperación que se cerró. La ruta es información del operador y por eso va en este guion, no en la interfaz.

**Qué archivo abrir.** Cada archivo `.eml` tiene por nombre un código largo (un hash) y no la dirección de correo, para que la carpeta no muestre las cuentas. **Tomá el archivo más reciente:** ordená por «Fecha de modificación». Al abrirlo con el Bloc de notas, verificá que la línea `To:` sea la dirección que estás usando. Copiá el número de la línea `Código:`. La línea `Vence:` dice hasta cuándo sirve: 10 minutos.

### El recorrido

1. **Crear la cuenta.** En el acceso, pulsá **Crear cuenta**. Completá Nombre, Apellido, Mail, Contraseña y Confirmar Contraseña con datos propios de demostración. Usá una dirección nueva y una contraseña que supere el medidor, por ejemplo `Rosario#Demo2026`, y pulsá **Registrarse**. **Vas a ver** «Verificá tu correo», el texto «Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.» y el campo **Código de verificación**. **Importa** porque el alta solo puede confirmarse con el código recibido por el canal externo simulado.
2. **Verificar el correo.** Abrí el `.eml` más reciente de la bandeja y copiá el código en **Código de verificación**. Pulsá **Confirmar correo**. **Vas a ver** «Correo verificado. Ya podés iniciar sesión y comprar.» y el botón **Volver al acceso**. Si el código está vacío o es incorrecto, aparece «El código no es válido o venció. Solicitá uno nuevo.».
3. **Entrar.** Pulsá **Volver al acceso** e iniciá sesión con la cuenta nueva. La contraseña se ve con asteriscos. **Vas a ver** «Mi cuenta» con el nivel Bronce («Bronce · 0,0 % de descuento», «3 compras para Plata»), «Sin entradas próximas», «Sin inscripciones» y, debajo, «Eventos disponibles».
4. **Pedir la recuperación.** Cerrá sesión y, en el acceso, pulsá **Recuperar contraseña**. Escribí el correo de la cuenta y pulsá **Enviar código**. **Vas a ver** el mismo texto que en el alta: «Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.». Con un correo inexistente la respuesta es idéntica y tarda lo mismo, así que la pantalla no revela qué cuentas existen.
5. **Restablecer la contraseña.** Abrí el `.eml` más reciente: su asunto es «Recuperacion de contrasena». Copiá el código en **Código de recuperación**, escribí y confirmá la contraseña nueva y pulsá **Restablecer contraseña**. **Vas a ver** «La contraseña se actualizó. Ya podés iniciar sesión.». Pulsá **Volver** e iniciá sesión con la contraseña nueva. Cada código vence, se consume una sola vez y pedir otro invalida el anterior.

**Verificado el 24/11/2026:**
- los cinco pasos, con la cuenta `lucia.guion@demo.eventaura.example`, sobre una base armada con la sección 1. Los mensajes son exactamente los que figuran arriba;
- el alta y la recuperación generaron cada una un `.eml` nuevo, el más reciente de la carpeta, con `To:` igual a la cuenta;
- el código vacío dio el mensaje de código no válido;
- después de restablecer, la cuenta entró con la contraseña nueva;
- `errores.log` no registró ningún error durante el recorrido.

**Sin verificar en esta pasada:**
- la respuesta con un correo inexistente;
- el tiempo mínimo de respuesta;
- que pedir un código nuevo invalide el anterior.

Esos tres casos los cubren las pruebas automáticas de la batería.

## 3. Comprar de punta a punta

1. **Abrir Duki.** Iniciá sesión como Paula (`plata@eventaura.com` / `Cliente123`) y, en el menú lateral, pulsá **Eventos disponibles**. Con la semilla de demostración hay más de veinte eventos: escribí `Duki` en el buscador y pulsá **Buscar**. En la tarjeta **Duki en Rosario**, pulsá **Ver entradas**. **Vas a ver** la ficha con la portada generada, «sábado 23 de enero · 21:00», el local («Galpón 17 · Estévez Boero 600, Rosario») y el mapa con el escenario sobre las zonas de los sectores.
2. **Armar el paquete.** Pulsá la zona **Campo** y después **Armar paquete**. **Vas a ver** el indicador `2 Armá`, «Tu nivel: Plata · 5,0 % de descuento» y la grilla con VIP, Platea y Campo. En la columna **Cantidad (escribí aquí)** de la fila **Campo** escribí `2` y pulsá Enter. Elegir la zona en el mapa no carga la cantidad: hay que escribirla. Pulsá **Ver servicios** y marcá **Hospedaje · $ 45.000**, **Traslado · $ 12.000** y **Seguro de entrada (devolución hasta 48 h antes)**. Dejá **Merchandising** y **VIP** sin marcar.
3. **Comprobar el resumen.** Con la base de la sección 1, el desglose es:

   | Línea | Importe |
   |---|---:|
   | 2 x Campo | $ 50.000,00 |
   | Desc. Plata 5,0 % | -$ 2.500,00 |
   | Subtotal entradas | $ 47.500,00 |
   | Hotel Ríos 1 noche | $ 45.000,00 |
   | Traslado ida y vuelta | $ 12.000,00 |
   | Cargo por servicio | $ 7.125,00 |
   | Seguro (2 entradas) | $ 10.000,00 |
   | **TOTAL** | **$ 121.625,00** |

   El cargo por servicio es el 15 % de las entradas con descuento: los servicios no lo generan. Pulsá **Comprar**.
4. **Pagar.** Se abre **Pago seguro** con «Total a pagar: $ 121.625,00» y la cuenta regresiva de la reserva (10 minutos). Completá titular `Paula Plata`, número `4111111111111111`, vencimiento `12/28` y código `123`, y pulsá **Pagar**. **Vas a ver** `4 Listo`, «¡Compra confirmada!», «Pedido EA-…», el comprobante con «TOTAL PAGADO $ 121.625,00» y «Tarjeta •••• 1111», y «2 entradas en PDF con QR: Guardadas en Documentos.». La tarjeta es de prueba: no se procesa ningún cargo.
5. **Los PDF.** Pulsá **Abrir carpeta**: los tickets están en `Documentos\A1.Tickets EVENTAURA`, uno por entrada (`Ticket_EA-…_Campo_1.PDF`, `…_2.PDF`), cada uno con un QR distinto. Pulsá **Aceptar**: la aplicación vuelve a **Eventos disponibles**.

**Verificado el 24/11/2026:**
- los cinco pasos, con el total exacto de $ 121.625,00 y el pedido EA-000121;
- los dos PDF se decodificaron con `tools/VerificarQrTickets.py`: dos códigos distintos, cada uno igual al texto de su PDF.

**No verificado:**
- **Abrir carpeta**, que abre el Explorador de Windows: se comprobaron los archivos directamente;
- la ficha al pasar el puntero por una zona.

**Atención en pantalla:** dentro de la navegación del cliente, el catálogo y la compra quedan debajo del título «Mi cuenta», corridos hacia abajo. En el catálogo, los botones **Ver entradas** de la primera fila quedan cortados por el borde inferior. Por eso conviene usar el buscador. Está anotado para Codex.

## 4. Seguro y devolución

**Comprar sin seguro.** Iniciá otra compra de Duki (1 Campo) y dejá **Seguro de entrada** sin marcar. Al pulsar **Comprar** se abre el cuadro **Seguro de entrada**:

> No agregaste el seguro de entrada. Solo se puede contratar ahora: después de pagar no se puede incorporar.
>
> ¿Continuar sin seguro?

Elegí **Sí** y pagá. Con 1 Campo, Paula paga $ 27.312,50: $ 25.000 menos el 5 %, más el 15 % de cargo. No afirmes que el seguro se puede sumar después: no se puede.

**Devolver una entrada.** Hace falta una compra con seguro, confirmada y a más de 48 horas del evento: la del paso 3 sirve.
1. En **Mis entradas**, elegí el pedido en **Historial Compras**. Abajo aparece «Entradas del pedido EA-…» con la columna **Devolver**.
2. Marcá **Devolver** en una entrada. El botón pasa a **Devolver 1 entrada(s)**: pulsalo.
3. El cuadro **Devolver entradas** dice: «¿Devolver 1 entrada(s)? Se reintegra el precio de cada entrada con su descuento. Si devolvés todas las que quedan, también los servicios. El cargo por servicio y el seguro no se reintegran. La devolución no se puede deshacer.».
4. Elegí **Sí**. **Vas a ver** «Devolución registrada. Se reintegran $ 23.750,00.» y la entrada pasa a **Reembolsada**.

En un pedido sin seguro, la interfaz no deja devolver: aunque marques la casilla, el botón **Devolver** queda deshabilitado.

**Verificado el 24/11/2026:**
- el aviso del seguro y la compra sin seguro (pedido EA-000122, $ 27.312,50);
- la devolución de una entrada del pedido EA-000121, con el reintegro de $ 23.750,00 y el estado Reembolsada;
- el botón deshabilitado en el pedido sin seguro.

## 5. QR de ingreso y privacidad entre clientes

**El ingreso solo se acepta desde 6 horas antes hasta 12 horas después del evento** (parámetros `INGRESO_HORAS_ANTES` e `INGRESO_HORAS_DESPUES`). Con las semillas, ningún evento cae en esa ventana el día de la presentación, así que hay que preparar uno.

**Preparación, antes de la presentación.** El formulario **Agregar evento** pide solo la fecha: la hora del evento queda en la hora en que se abrió el formulario (anotado para Codex). Hay dos caminos:
1. **El evento de la sección 7.** Paula, ya Organizadora, crea su evento y Ana lo aprueba. Hay que crearlo el día de la presentación unas horas antes, o crearlo y después ajustar su hora en la base de demostración (nunca en otra base):

   ```sql
   UPDATE Eventos SET Fecha = DATEADD(MINUTE, 180, CAST(GETDATE() AS smalldatetime))
   WHERE Nombre = 'Noche de prueba de ingreso';
   ```

2. **Una entrada para ese evento.** Un cliente (por ejemplo, la cuenta creada en la sección 2) compra una entrada. El código para el control de ingreso está en **Mis entradas**, en la columna **Código**, y en el texto del PDF: empieza con `EA-`.

**El control de ingreso.** Iniciá sesión con el Organizador dueño del evento y pulsá **Control de ingreso**.
1. En **Evento con ingreso abierto** aparecen solo sus eventos dentro de la ventana.
2. Pegá el código en **Código del ticket (lector USB o texto del PDF)** y pulsá **Registrar ingreso**. **Vas a ver** «Ingreso válido. Bienvenido.» y «Ingresos registrados: 1».
3. Repetí el mismo código. **Vas a ver** «La entrada ya ingresó el {dd/MM/aaaa HH:mm:ss}.».

El Administrador de plataforma no tiene locales y no hace el control de ingreso.

**Privacidad entre clientes.** Iniciá como `oro@eventaura.com` y abrí **Mis entradas**: aparecen solo los pedidos de Oscar, y ninguno de los de Paula. La aplicación no ofrece ninguna acción para elegir a otro cliente. Si una operación intenta leer datos de otra cuenta, la regla responde «Solo podés operar sobre los datos de tu propia cuenta.». Ese intento forzado se verifica en la batería; no es un botón de la interfaz.

**Verificado el 24/11/2026**, con el evento «Noche de prueba de ingreso» de Paula, con la hora ajustada por SQL a tres horas después, y la entrada comprada por la cuenta de la sección 2:
- «Ingreso válido. Bienvenido.»;
- el segundo intento: «La entrada ya ingresó el 24/11/2026 16:56:41.»;
- el QR de otro evento: «La entrada corresponde a otro evento.»;
- un código inventado: «El código no existe.»;
- Oscar no ve los pedidos EA-000121 ni EA-000122 de Paula.

**No verificado:**
- el QR reembolsado del mismo evento: el reembolsado disponible era de otro evento y dio «La entrada corresponde a otro evento.», porque ese control va primero;
- una entrada pendiente o vencida;
- el ingreso fuera de la ventana horaria.

## 6. Suscripción, beneficios y lista de espera

1. **Contratar.** Como Paula, abrí **Mi suscripción**. Sin suscripción activa dice «No tenés una suscripción activa.» y muestra los beneficios: seguro de entrada sin cargo, preventa exclusiva y prioridad en las listas de espera, «Sin descuento extra: rige el de tu nivel.». Elegí **Mensual: $ 9.000 por mes** y pulsá **Contratar**. El cuadro dice «¿Contratar el plan Mensual por $ 9.000,00? La suscripción no se renueva automáticamente al vencer.». Elegí **Sí**. **Vas a ver** «Suscripción Mensual contratada. Vence el {fecha}. No se renueva automáticamente.» y «Plan Mensual activo». Con una suscripción activa, **Contratar** queda deshabilitado: no se puede contratar una segunda.
2. **Seguro bonificado.** Armá una compra y marcá **Seguro de entrada**: el resumen muestra «Seguro sin cargo (1 entradas)» y el total no cambia. Para salir sin pagar, pulsá **Volver al listado**: el cuadro pregunta «Si volvés al listado se descarta el paquete que armaste. ¿Querés volver?».
3. **Preventa.** El Organizador puede fijar el fin de la preventa en **Administrar evento** (**Preventa para suscriptores hasta**, **Guardar preventa**). Hasta ese momento, el evento se vende solo a suscriptores.
4. **Lista de espera.** Buscá **Clásico Rosarino** (agotado: la ficha muestra los tres sectores «AGOTADO») y pulsá **Armar paquete**.
   1. **Vas a ver** «Entradas agotadas. Anotate en la lista de espera. Los avisos de disponibilidad se muestran al iniciar sesión.».
   2. Pulsá **Anotarme en la lista de espera**. **Vas a ver** «Te anotaste en la lista de espera de «Clásico Rosarino». Podés consultar tu posición. Si vuelve a haber entradas, te lo mostramos al iniciar sesión.», y el botón pasa a **Ya estás en la lista de espera**.
   3. Pulsá el enlace **Consultar mi posición**, sobre el texto subrayado. **Vas a ver** «Tu posición actual en la lista es {N}.».
   4. En **Inicio**, la tarjeta **Lista de espera** muestra los eventos y la cantidad de inscripciones. Con la semilla de demostración, Paula ya estaba en la de Guitarras del litoral.
5. **Prioridad temporal.** Cuando se devuelve una entrada de un sector agotado, durante 15 minutos el cupo liberado se reserva para los suscriptores anotados en la lista de ese evento. No hay forma de adelantar el reloj: para mostrarlo en vivo hay que esperar 15 minutos. Si no, explicá la regla y mostrá el caso de la batería. No digas que la posición garantiza ser el único comprador, ni que se manda un correo externo.

**Verificado el 24/11/2026:**
- la contratación del plan mensual (vence el 24/12/2026) y **Contratar** deshabilitado después;
- «Seguro sin cargo» en el resumen;
- el cuadro de «Volver al listado»;
- anotarse en Clásico Rosarino, la posición 1 y la tarjeta del inicio con «2 inscripciones».

**No verificado:**
- la preventa (paso 3);
- la prioridad temporal (paso 5), que necesita 15 minutos de espera real.

## 7. Tres perfiles y administración

### Solicitar ser Organizador

1. Iniciá sesión con una cuenta Cliente sin solicitud pendiente, por ejemplo Paula, y pulsá **Solicitar ser Organizador**. Se abre la ventana **Solicitar perfil de Organizador**.
2. Completá:
   - responsable: `Persona Ficticia de Demostración`;
   - CUIT: `30000000007`;
   - teléfono: `+5493410000000`;
   - nombre del local: `Local Ficticio de Demostración`;
   - dirección: `Dirección inventada 000, Rosario`;
   - capacidad: `500`;
   - CBU: `0000000000000000000000`.

   Son valores de demostración con dígitos verificadores válidos: no ingreses datos reales.
3. Pulsá **Enviar código**. **Vas a ver** «Si el teléfono es válido, guardamos un SMS simulado fuera de EventAura. Abrí el archivo del mensaje para continuar.». El código **no** aparece en el formulario: está en un archivo `.sms.txt` de la misma carpeta de la bandeja simulada (sección 2), el más reciente, con las líneas «SMS simulado para +54…», «Código:» y «Vence:». Copialo en **Código de verificación** y pulsá **Confirmar teléfono**. **Vas a ver** «Teléfono verificado.».
4. Pulsá **Adjuntar documentación** y, en el cuadro **Elegí la documentación**, elegí `Pruebas/Capturas-Fase8b/despues-100/frmMenuCliente.png` (26 KB). **Vas a ver** «Documentación adjunta y validada.».
5. Pulsá **Enviar solicitud**. **Vas a ver** «Recibimos tu solicitud. Tu cuenta sigue siendo Cliente mientras esté pendiente.», sin número de solicitud. La semilla de demostración trae además solicitudes pendientes, por ejemplo la de Bruno para «Espacio Ribera», si querés mostrar la revisión sin crear otra.

**No muestres en vivo un rechazo de este formulario.** Hoy, un CUIT inválido, el teléfono sin confirmar o un código de teléfono incorrecto muestran «Ocurrió un error inesperado y la operación no se completó…», en lugar del mensaje de la regla (anotado para Codex). La solicitud no se guarda, pero el mensaje es engañoso.

### Revisar solicitudes — Administrador de plataforma

Iniciá como Ana (`admin@eventaura.com` / `Admin1234`) y pulsá **Solicitudes de organizador**.
1. Se abre **Revisión de solicitudes**: a la izquierda, la lista de locales pedidos; en el centro, el responsable, el CUIT, el teléfono, el local, la dirección, la capacidad, el CBU o alias y la documentación; a la derecha, la imagen adjunta.
2. Elegí la solicitud y escribí un motivo en el cuadro, debajo de «Escribí un motivo antes de aprobar o rechazar.».
3. Pulsá **Aprobar y crear local**. **Vas a ver** «Solicitud aprobada y local creado.».

Al volver a iniciar sesión, la cuenta entra como Organizador: «Tu espacio de organización», con su local.

**No pulses Rechazar con el motivo vacío:** hoy muestra «Ocurrió un error inesperado…» en lugar de «Escribí el motivo del rechazo.». La solicitud no se rechaza, pero el mensaje es engañoso (anotado para Codex).

### Locales, eventos y revisión del primer evento — Organizador

1. Como el Organizador nuevo, pulsá **Mis locales**, elegí el local y pulsá **Ver eventos**. Sin eventos dice «Todavía no hay eventos en este local. Creá uno para empezar a ofrecer entradas.».
2. Pulsá **Agregar Evento**. Completá **Nombre evento**, la **Fecha** (solo el día: ver la sección 5) y la **Categoría**, y pulsá **Registrar**. **Vas a ver** «Evento guardado como borrador. Agregale un sector para publicarlo.» y el evento en estado **Borrador**.
3. Elegilo y pulsá **Administrar Evento**. Si pulsás **Publicar** sin sectores: «No se puede publicar el evento: necesita al menos un sector con capacidad mayor que cero.». Pulsá **Agregar sector**, completá **Nombre**, **Capacidad** y **Precio**, en ese orden en pantalla, y pulsá **Agregar sector**. **Vas a ver** «Sector agregado correctamente. Ya podés asignarle entradas.». Pulsá **Volver**.
4. Pulsá **Publicar**. Como es el primer evento de un Organizador nuevo: «El primer evento se envió a revisión del Administrador.». El evento queda **EnRevision**: no aparece en el catálogo; al buscarlo, un cliente ve «No encontramos eventos para esa búsqueda…».
5. **Rechazo.** Ana pulsa **Eventos en revisión**: se abre **Primer evento de cada organizador**, con el nombre, la fecha y «Al aprobarlo, aparece en el catálogo. Si se rechaza, vuelve a Borrador con el motivo.». Si pulsa **Rechazar y devolver** sin motivo: «Escribí el motivo del rechazo.», y el evento no cambia. Con el motivo escrito en **Motivo del rechazo (obligatorio)**, el evento vuelve a **Borrador** y la lista dice «No hay eventos pendientes.».
6. El Organizador ve el evento en **Borrador**, con el motivo en la columna **Motivo del rechazo**. Lo corrige y vuelve a pulsar **Publicar**: como todavía no tuvo un evento aprobado, vuelve a **EnRevision**.
7. **Aprobación.** Ana pulsa **Aprobar y publicar**: el evento queda **Publicado** y aparece en el catálogo.

El Organizador de la semilla (`organizador@eventaura.com`) ya tiene una publicación aprobada: sus eventos nuevos pasan directo a **Publicado**.

### Administración de plataforma

En el panel de Ana se ven los indicadores globales y los pendientes de revisión.
- **Reportes globales** consulta todos los eventos.
- **Registro de auditoría** muestra quién hizo qué, sobre qué entidad, cuándo y con qué resultado. **Atención:** hoy la columna **Fecha y hora** muestra la hora UTC, tres horas más que la de la PC (anotado para Codex). Si el jurado lo nota, explicalo así.
- **Crear administrador** crea otra cuenta privilegiada.

El Administrador revisa solicitudes y aprueba primeros eventos, pero no administra locales ni controla ingresos.

**Verificado el 24/11/2026:**
- el recorrido de la solicitud de Paula, con el código del `.sms.txt`, el adjunto y el envío;
- la aprobación por Ana y el ingreso de Paula como Organizador;
- la creación del evento, publicar sin sectores, el sector y el envío a revisión;
- que el evento no está en el catálogo mientras está en revisión;
- el rechazo sin motivo y con motivo;
- el evento en Borrador con el motivo, la republicación a revisión y la aprobación hasta Publicado;
- el registro de auditoría, con las acciones de los tres perfiles en orden («Gestión de eventos: aprobar primer evento», «enviar a revisión», «rechazar primer evento», «Inicio de sesión»);
- el mensaje genérico en los rechazos del formulario de solicitud y en el rechazo de solicitud sin motivo.

**No verificado:**
- el aviso de éxito de **Aprobar y publicar** y el de **Rechazar y devolver**: no se llegó a leer, pero el estado del evento se confirmó en la base;
- **Reportes globales**, **Crear administrador** y los reportes del Organizador;
- **Registrar local** desde el Organizador.

## 8. Lo que tiene que fallar

Probá estos rechazos solo con datos de demostración. Los textos entre comillas son el mensaje esperado; algunos agregan una fecha, nombre o cantidad dinámica indicada entre llaves. La columna «24/11» dice si se vio en pantalla el 24/11/2026.

| Qué intentar | Respuesta que tiene que aparecer | 24/11 |
|---|---|---|
| Iniciar sesión con un correo existente y contraseña incorrecta; repetir con uno inexistente | En ambos casos, exactamente `Mail o contraseña incorrectos.`, en el mismo tiempo. No se muestra el número de intentos restantes. | Sí, los dos casos, 1,2 s cada uno |
| Registrar un correo que ya pertenece a una cuenta | La pantalla responde igual que con un correo nuevo («Verificá tu correo», «Si la cuenta existe, enviamos un código…»), para no revelar qué cuentas existen. No se crea una segunda cuenta. | Sí. El guion anterior decía `Ya existe una cuenta registrada con ese mail.`: ese mensaje ya no se muestra en el alta |
| Comprar desde una cuenta sin correo verificado | `Verificá tu correo antes de iniciar una compra.` | No |
| Registrar dos veces el mismo QR | `La entrada ya ingresó el {dd/MM/aaaa HH:mm:ss}.` | Sí |
| Devolver una entrada ya ingresada | `No podés devolver una entrada que ya se usó para ingresar.` | No |
| Devolver sin seguro | La interfaz no lo permite: el botón **Devolver** queda deshabilitado. La regla responde `Solo se pueden devolver entradas con seguro, y el seguro se contrata únicamente durante la compra.` (batería). | Sí, el botón deshabilitado |
| Intentar agregar seguro después de pagar | No se ofrece esa operación; el seguro solo se decide durante el armado. | Sí, el aviso del armado |
| Un cliente leer u operar datos de otra cuenta | `Solo podés operar sobre los datos de tu propia cuenta.` | No; solo se vio que Oscar no ve los pedidos de Paula |
| Controlar el ingreso de un evento ajeno | `Solo podés controlar el ingreso de eventos de tus locales.` | No |
| Comprar durante la preventa con una cuenta no suscriptora | `«{evento}» está en preventa exclusiva para suscriptores hasta el {dd/MM/aaaa a las HH:mm}. Contratá una suscripción desde «Mi suscripción» o volvé cuando termine la preventa.` | No |
| No suscriptor intenta comprar un cupo liberado dentro de los 15 minutos de prioridad | `El lugar liberado está reservado temporalmente para suscriptores inscriptos en la lista de espera. Intentá al finalizar la ventana de prioridad.` | No |
| Suscriptor intenta anotarse cuando aún quedan entradas | `El evento todavía tiene entradas: la lista de espera se habilita cuando se agota.` | No |
| Anotarse en la lista de espera de un evento pasado o no publicado | `El evento ya pasó o no está publicado: no admite inscripciones en la lista de espera.` | No |
| Ingresar un código de verificación o recuperación incorrecto, vencido o ya consumido | `El código no es válido o venció. Solicitá uno nuevo.` | Sí, con el código vacío |
| Usar una contraseña por debajo del mínimo de fortaleza | `La contraseña no alcanza la fortaleza mínima. Sumá variedad de mayúsculas, minúsculas, números o símbolos.` | No |
| Intentar contratar una segunda suscripción activa | La interfaz no lo permite: **Contratar** queda deshabilitado. La regla responde `Ya tenés una suscripción activa: no se pueden tener dos al mismo tiempo.` (batería). | Sí, el botón deshabilitado |
| Comprar más entradas que las disponibles | `En el sector {sector} quedan {N} entradas.` | Sí: «En el sector General quedan 2 entradas.» (Taller de fotografía urbana) |
| Comprar más de seis entradas en una operación | `Se pueden comprar como máximo 6 entradas por operación.` | Sí |
| Pagar servicios sin entradas | **Comprar** queda deshabilitado sin entradas. La regla responde `El paquete debe tener al menos una entrada: los servicios no se venden solos.` (batería). | Sí, el botón deshabilitado |
| Cliente con una solicitud pendiente intenta abrir funciones de Organizador | `Esta operación es solo para el Organizador.` La cuenta conserva el perfil Cliente hasta la aprobación; el menú del cliente no ofrece esas funciones. | No |
| Organizador intenta revisar una solicitud o aprobar un evento | `Esta operación es solo para el Administrador` | No |
| Administrador intenta una operación exclusiva del Organizador | `Esta operación es solo para el Organizador.` | No |
| Publicar un evento sin sectores con capacidad positiva | `No se puede publicar el evento: necesita al menos un sector con capacidad mayor que cero.` El evento queda en Borrador. | Sí |
| CUIT con dígito verificador incorrecto | Debería ser `El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.`. **Hoy se ve** «Ocurrió un error inesperado y la operación no se completó…». | Sí: mensaje genérico. No mostrar hasta que Codex lo corrija |
| CBU con dígitos verificadores incorrectos | Debería ser `Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.`. Usa el mismo camino que el CUIT. | No; por el mismo código, probablemente da el mensaje genérico |
| Enviar la solicitud sin confirmar el teléfono | Debería ser `Verificá este teléfono con el código enviado antes de presentar la solicitud.`. **Hoy se ve** el mensaje genérico. | Sí: mensaje genérico |
| Código de teléfono incorrecto | **Hoy se ve** el mensaje genérico en lugar del de la regla. | Sí: mensaje genérico |
| Adjuntar un archivo que no sea PNG o JPEG, o que supere 512 KB | Debería ser `Adjuntá una imagen PNG o JPEG válida, comprimida y de hasta 512 KB.`. Usa el mismo camino que el CUIT. | No; probablemente da el mensaje genérico |
| Rechazar una solicitud con el motivo vacío | Debería ser `Escribí el motivo del rechazo.`. **Hoy se ve** el mensaje genérico. | Sí: mensaje genérico |
| Vender un evento en revisión o no publicado | No aparece en el catálogo. La regla responde `El evento no está disponible para la venta: ya pasó o no está publicado.` (batería). | Sí, que no aparece |
| Aprobar otra vez un evento que ya no está en revisión | `El evento ya no está pendiente de revisión.` | No |
| Rechazar un evento en revisión sin escribir un motivo | `Escribí el motivo del rechazo.` El evento no cambia de estado. | Sí |
| Un Organizador intenta rechazar su propio evento o el de otro | `Esta operación es solo para el Administrador` | No |
| Comprar en un evento pasado o no publicado | `El evento no está disponible para la venta: ya pasó o no está publicado.` | No |
| Usar un QR inexistente | `El código no existe.` | Sí |
| Usar un QR de otro evento | `La entrada corresponde a otro evento.` | Sí |
| Usar un QR reembolsado del mismo evento | `La entrada está reembolsada.` | No |
| Usar una entrada pendiente o vencida | `La entrada está pendiente o vencida.` | No |
| Registrar un ingreso fuera de la ventana | `Fuera del horario de ingreso o evento no disponible.` | No |

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
- No fuerces errores editando la base durante la presentación ni uses una compra ya reembolsada para probar ingreso. Para el QR, prepará antes un evento dentro de la ventana de ingreso y una entrada confirmada y no usada (sección 5).
- Hasta que Codex lo corrija, no muestres los rechazos del formulario de solicitud de Organizador ni el rechazo de solicitud sin motivo: hoy muestran «Ocurrió un error inesperado…» (sección 8).
