# Verificación en ejecución del 26/11 a la tarde: T-38 a T-44 y el guion

Hecha por Claude Code entre las 18:15 y las 18:45, con el escritorio tomado por `.escritorio.lock`.

**Cómo:**
- Compilé el HEAD (`5890e70`) con `tools/Proyecto.ps1 -Accion Compilar`, a 96 DPI (Windows al 100 %).
- Abrí el **`Visual.exe` real** desde una copia de `bin\Debug` apuntada a `BddEventAura_Verif_Demostracion`, una base nueva con `01`, `02` y la semilla de demostración nueva.
- Lo manejé con UI Automation, mouse y teclado (`docs/verificaciones/ejecucion-2026-11-26-tarde/Uia.ps1`, `Ojos.ps1`, `Armador.ps1` y `Cierre.ps1`).
- Las capturas están en esa misma carpeta.

## Resultado

| Qué | Resultado |
|---|---|
| Ojo en el acceso, el alta, la recuperación y el cambio de contraseña (T-43) | **Pasa** |
| Sin `checkbxShowPass` sobre el alta web (T-43, `29ccb85`) | **Pasa** |
| Cambiar contraseña dentro de «Mi cuenta» (T-44, `47ae5a3`) | **Pasa** |
| «Mi suscripción»: un título, tarjetas, Enter (T-44, `a9f9c1b`) | **Pasa**, con un detalle: **Contratar** queda casi oculto |
| Armador: mapa con cada juego de sectores (T-44, `7f76e63`) | **Pasa** en los cinco eventos |
| Armador: total a la vista (T-44, `7f76e63`) | **No pasa** en ninguno de los cinco |
| Catálogo: 25 eventos, el agotado al final (T-44, `e98a958`) | **Pasa** |
| Verificar el correo con sesión iniciada y comprar (T-39, `06a9451`) | **Pasa**, con detalles de título y desplazamiento |
| El aviso no tapa los filtros (T-40) | **Pasa** |
| Ayuda de tarjeta y barra del vencimiento (T-41, `6bd5777`) | **Pasa** |
| Cierre inesperado, 10 vueltas (T-38) | **No se reprodujo**; el registro nuevo de cierres funciona |
| Guion de punta a punta | **No completo**: ver «Lo que no se recorrió» |

## El ojo (T-43)

En cada pantalla conté con UI Automation los botones «Mostrar contraseña» y las casillas visibles. Después escribí en
los campos secretos, pulsé cada ojo y conté los campos que seguían secretos.

| Pantalla | Ojos | Casillas visibles | Campos secretos antes → después de pulsar | Etiqueta después |
|---|---|---|---|---|
| Acceso | 1 (40 × 41) | 0 | 1 → 0 | «Ocultar contraseña» |
| Alta, paso «Tus datos» | 2 | 0 | 2 → 0 | «Ocultar contraseña» |
| Recuperación, paso del código | 2 | 0 | 2 → 0 | «Ocultar contraseña» |
| Cambiar contraseña, en «Mi cuenta» | 3 | 0 | 3 → 0 | «Ocultar contraseña» |

En el alta ya no aparece `checkbxShowPass`: ni visible ni fuera de pantalla (`2-alta.png`). En la recuperación, la ruta
de la bandeja **no** aparece, como decidió JC (`3-recuperacion.png`).

## Pedidos del 26/11 (T-44)

**Cambiar contraseña** (`4-cambiar-en-mi-cuenta.png`): se abre dentro de «Mi cuenta». El proceso tiene una sola ventana y
ningún modal. El título es «Cambiar contraseña» y el mail es de solo lectura.

**Mi suscripción** (`5-suscripcion.png`):
- un solo título visible, «Mi suscripción»;
- dos tarjetas, Mensual $ 9.000 y Anual $ 90.000, con «Ahorrás $ 18.000 frente a 12 meses»;
- con el foco en Mensual, **Enter** abre «¿Contratar el plan Mensual por $ 9.000,00? …». Respondí **No**.
- **Detalle:** **Contratar** ocupa Y 712 a 753 de la ventana y la vista web termina en 727. Se ven 15 de sus 41 px; hay
  que desplazar para pulsarlo con el mouse.

**Catálogo** (`6-catalogo.png`):
- 25 tarjetas, con los disponibles por fecha y **Clásico Rosarino («Agotado») último**, aunque su fecha (26/12) cae en el
  medio;
- «Taller de fotografía urbana» figura como «Pocas entradas».

**Armador**, abierto desde el catálogo de «Mi cuenta» en cinco eventos:

| Evento | Sectores en el mapa (escenario →) | TOTAL, en Y | «Continuar al pago», en Y |
|---|---|---|---|
| Duki en Rosario | VIP › Platea › Campo | 1225 | 1326 a 1367 |
| Comedia musical en la avenida | Platea preferencial › Platea › Pullman › Superpullman | 1225 | 1326 a 1367 |
| Taller de fotografía urbana | General | 1059 | 1160 a 1201 |
| Festival del río | VIP › Campo delantero › Campo general | 1225 | 1326 a 1367 |
| Copa de verano 3x3 | Preferencial › General | 1134 | 1235 a 1276 |

- El mapa se dibuja en los cinco, con los precios bajando en ese orden.
- La vista web va de Y 175 a 759 de la ventana. **«Tu paquete» quedó debajo de Entradas y de Servicios, en una sola
  columna**: el total y «Continuar al pago» quedan entre 300 y 600 px por debajo del borde.
- Antes de `7f76e63` el resumen estaba a la derecha y el total se veía.
- La compra se completa desplazando (pedido EA-000319, `10-confirmacion.png`), pero **el total no está a la vista
  mientras se arma el paquete**.

## Verificar el correo con sesión (T-39) y el aviso (T-40)

Creé una cuenta nueva, pulsé **Volver al acceso** en «Verificá tu correo» e inicié sesión.
- En **Armar paquete** aparece el aviso rojo «Verificá tu correo antes de iniciar una compra.» con el botón **Verificar
  correo**, **arriba de los filtros, sin taparlos** (`12-sin-verificar-aviso.png`).
- **Verificar correo** abre el paso del código dentro de «Mi cuenta», con el texto «Abrí la carpeta BandejaSimulada
  (C:\Users\…\AppData\Local\EventAura\BandejaSimulada) y buscá el archivo más reciente», el campo del código, **Confirmar
  correo** y **Reenviar código** (`14-verificar-con-sesion-codigo.png`).
- Con el código del `.eml` más reciente: «Correo verificado. Ya podés iniciar la compra.». Sin cerrar sesión, el armador
  de «Orquesta juvenil del litoral» se abre.

**Detalles:**
- el título de «Mi cuenta» dice «Crear cuenta» y el menú marca «Inicio»;
- la parte visible muestra solo el panel lateral del alta: el campo del código está en Y 930, fuera de la vista, y hay
  que desplazar (`13-verificar-con-sesion.png`);
- después del aviso, el catálogo que queda abajo es el **respaldo WinForms**, con el título «Armar paquete».

## Pago (T-41)

**Pago seguro** dice: «Prototipo: usá estos datos de prueba · Acepta 13 a 19 dígitos; vencimiento MM/AA no vencido;
código de 3 o 4 dígitos. Ejemplo: 4111 1111 1111 1111 · 12/29 · 123. Una tarjeta terminada en 0000 simula rechazo.».
Escribí `1229` y quedó `12/29` (`9-pago-datos.png`). Con titular, `4111 1111 1111 1111` y `123`, la compra se confirmó.

## Lista de espera (guion 6.4)

Como Paula: Clásico Rosarino → **Armar paquete** → «Entradas agotadas. Anotate en la lista de espera…» → **Anotarme en
la lista de espera** → «Te anotaste en la lista de espera de «Clásico Rosarino»…».

**Detalle:** después, el botón del respaldo dice «Ya estás en la lista de espera», pero las tres tarjetas web siguen
diciendo «Anotarme en la lista de espera» (`18-posicion.png`). No consulté la posición: el enlace no se pudo pulsar desde
UI Automation.

## Cierre inesperado (T-38)

Con los commits de Codex `52cba8e` y `9963ef6`, que registran en `errores.log` el origen de cada cierre y dejan que
Windows anote las caídas nativas (evento 1000).

**Qué hice** (`Cierre.ps1`, salida en `cierre-10-vueltas.txt`):
- el `Visual.exe` real del HEAD, apuntado a `BddEventAura`, la base donde ocurrió;
- **con `bronce@eventaura.com` y no con `test0001`**, porque la contraseña de `test0001` es de JC y no la tengo;
- en cada vuelta: «Eventos disponibles» → primer evento → **Armar paquete** → una entrada → **Continuar al pago** →
  **Seguir sin seguro** → **Pago seguro** abierto **45 s sin tocar**, vigilando el proceso cada segundo → **Cancelar
  pago**;
- **10 vueltas**, de 18:31 a 18:43.

**Qué vi:**
- en ninguna vuelta se cerró la aplicación; el proceso siguió vivo al final;
- `errores.log` anotó las 10 veces «[origen: cierre de ventana] ventana=frmPago (Pago seguro); CloseReason=None»: es el
  cierre del pago con **Cancelar pago**. **El registro funciona.** El último renglón, `CloseReason=TaskManagerClosing`
  sobre «Mi cuenta», es el cierre que hice yo al terminar;
- ningún evento 1000 ni 1026 en el Visor de eventos.

**Resultado:** no se reprodujo. La próxima vez que se cierre, `errores.log` va a decir el origen: un cierre ordenado y
la ventana, o «fin externo o caída nativa», y en ese caso también el evento 1000.

**Lo que quedó en `BddEventAura`:** 11 compras de bronce de hoy, de la 13 a la 23. Hay 9 en «Pendiente», sin reserva:
**Cancelar pago** libera el cupo (`ReservaVence` en NULL y ningún sector con cupo reservado) y deja la compra para
reintentar. Las otras 2 están en «Vencida», de dos intentos que corté. No las borré: la base es de JC.

## Lo que no se recorrió del guion

Por tiempo, y porque UI Automation no alcanza algunos controles, quedaron sin recorrer en esta pasada:
- sección 2: la recuperación completa con el código (llegué hasta los campos de contraseña nueva);
- sección 4, seguro y devolución;
- sección 5, el QR de ingreso y la privacidad entre clientes;
- sección 6, contratar la suscripción (llegué a la confirmación y respondí No) y la preventa;
- sección 7: las solicitudes de Organizador, el Administrador y el Organizador;
- sección 8, lo que tiene que fallar;
- 144 DPI.
