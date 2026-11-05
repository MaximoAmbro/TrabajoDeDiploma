# JC-R11 — Recorrido de las pantallas principales solo con teclado

**05/11/2026, sobre el ejecutable real** (`Visual\bin\Debug\Visual.exe`, copiado a una
carpeta temporal y apuntado a `BddEventAura_Pruebas`), a escala 100 %.

**Cómo se hizo.** Con `tools/VerificacionesManuales/Teclado.ps1`:
- cada tecla se envió como mensaje (`WM_KEYDOWN`/`WM_KEYUP`, o `WM_CHAR` para escribir) al
  control que tenía el foco en el hilo de la aplicación, que es lo que recibe la aplicación
  cuando el usuario la pulsa;
- el foco se leyó con `GetGUIThreadInfo`;
- no se usó el teclado ni el mouse del escritorio.

**Límite del método:** no se pueden enviar combinaciones con Shift o Ctrl (Shift+Tab,
Ctrl+Tab). Alt+F4 sí se probó, porque el bit de Alt viaja en el mensaje.

## Resultado en una línea

**El circuito del cliente se completa entero con teclado.** Iniciar sesión, buscar, armar el
paquete del ejemplo, pagar y confirmar funcionan (pedido EA-000073: 2 Campo + hospedaje,
pagado con Enter). Pero **cinco acciones no se pueden hacer con teclado**, porque son
etiquetas con clic y no botones, y **tres de ellas son callejones sin salida**: el usuario
solo sale con Alt+F4, que cierra la aplicación entera.

## Dónde se traba

| # | Pantalla | Qué pasa con teclado | Gravedad | Hallazgo |
|---|---|---|---|---|
| T-1 | Iniciar sesión | «Crear Cuenta» y «¿Olvidaste tu contraseña?» son etiquetas: **no se alcanzan**. Un usuario de solo teclado no puede registrarse | Alta | Nuevo |
| T-2 | Menú del cliente y menú del administrador | «Volver al inicio» (cerrar sesión) es una etiqueta: **no se puede cerrar la sesión**. Alt+F4 cierra la aplicación | Media | Nuevo |
| T-3 | Registrar local | «Volver» es una etiqueta: **callejón sin salida**. Solo se sale registrando un local o con Alt+F4 | Alta | Nuevo |
| T-4 | Agregar sector | Ídem. Además, el orden de Tab es Capacidad → «Agregar sector» → Nombre → Precio | Alta | Nuevo |
| T-5 | Registrar evento | Ídem. Además, el foco arranca en el botón «Registrar» y el orden de Tab es desordenado | Alta | Nuevo |
| T-6 | Iniciar sesión | El foco arranca en Mail, pero **Mail no está en el ciclo de Tab**: si el usuario sale del campo, no vuelve. Enter en la contraseña no inicia sesión | Media | U-02 (conocido) |
| T-7 | Armar paquete → «Seguro de entrada» | El foco del aviso está en «No»: Enter cancela la compra. Tab lleva a «Sí» | Baja | E-D (conocido) |
| T-8 | Mis eventos | Tab recorre celda por celda las dos grillas: 27 de las 30 pulsaciones del ciclo | Baja | Nuevo |
| T-9 | Eventos disponibles | Enter en la búsqueda no busca; los filtros de categoría y fecha no tienen rótulo | Baja | U-08 (conocido) |
| T-10 | Todas | Esc no hace nada en ninguna pantalla (ninguna tiene `CancelButton`) | Baja | Nuevo |

T-1 a T-5 tienen la misma causa: `lblcrearcuenta`, `lblolividarcontra`, `lblvolver` (dos
menús), `lblyatengocuen` (alta de usuario y registrar local) y `lblVolver` (agregar evento y
agregar sector) son `Label` con un evento `Click`. Una etiqueta no recibe el foco. La
corrección es la misma en las siete: convertirlas en `LinkLabel` o en `Button`.

## Lo que funciona bien

- **Pago seguro:** el foco arranca en Titular, Tab sigue el orden de lectura (Titular,
  Número, Vencimiento, Código, Pagar, Cancelar) y Enter paga.
- **Compra confirmada:** el foco arranca en «Aceptar» y Enter cierra.
- **Armar paquete:** el foco arranca en la cantidad (T-25). Flechas, «2» y Enter cargan las
  entradas, Tab sale de la grilla a los servicios, Espacio tilda el hospedaje, y «Comprar» se
  alcanza con Tab cuando hay entradas (T-26).
- **Grupos de opciones** (suscripción mensual o anual): las flechas cambian la opción, como en
  cualquier programa de Windows.
- **Alt+F4** cierra la aplicación desde cualquier pantalla (verificado en el menú, en
  «Registrar local» y en «Registrar evento»), gracias a T-24.

## Recorrido por pantalla

| Pantalla | Foco inicial | Ciclo de Tab | Fuera del ciclo |
|---|---|---|---|
| Iniciar sesión | Mail | Contraseña → «Show Password» → «Iniciar sesión» → «Modificar» | Mail, «Crear Cuenta», «¿Olvidaste tu contraseña?» |
| Mi cuenta (cliente) | «Eventos disponibles» | «Eventos disponibles» → «Mis eventos» → «Mi suscripción» | «Volver al inicio» |
| Eventos disponibles | Búsqueda | Búsqueda → categoría → fecha → «Buscar» → «Limpiar» → grilla (9 Tab) → «Comprar» → «Volver» | — |
| Armar paquete | Cantidad de VIP, en edición | Grilla (5 Tab) → 4 servicios → seguro → «¿Cómo funcionan los niveles?» → «Volver al listado» → «Comprar» | «Comprar» y seguro, mientras no hay entradas (correcto) |
| Seguro de entrada | «No» | «No» ↔ «Sí» | — |
| Pago seguro | Titular | Titular → Número → Vencimiento → Código → «Pagar» → «Cancelar pago» | — |
| Compra confirmada | «Aceptar» | «Aceptar» → «Abrir carpeta» → ruta → lista de tickets | — |
| Mis eventos | Grilla | Grilla → grilla → «Volver» (30 Tab en total) | «Devolver», deshabilitado sin selección (correcto) |
| Mi suscripción | «Mensual» | Opción elegida → «Contratar» → «Volver» | «Anual»: se llega con flechas (correcto) |
| Administración | «Mis locales» | «Mis locales» → «Registrar local» → «Listas de espera» → «Reportes» | «Volver al inicio» |
| Mis locales | «Volver» | «Volver» → grilla → «Ver eventos» | — |
| Eventos del local | «Volver» | «Volver» → grilla (8 Tab) → «Eliminar evento» → «Administrar Evento» → «Agregar Evento» | — |
| Administrar evento | «Volver» | «Volver» → grilla (8 Tab) → «Agregar sector» → «Eliminar sector» → fecha de preventa → «Guardar preventa» | — |
| Agregar sector | Capacidad | Capacidad → «Agregar sector» → Nombre → Precio | «Volver» |
| Registrar local | Nombre | Nombre → Dirección → Altura → Capacidad → «Registrarse» | «Volver» |
| Registrar evento | «Registrar» | «Registrar» → fecha → Nombre → Categoría → fecha | «Volver» |
| Listas de espera | Lista de eventos | Lista → grilla → «Volver» | — |
| Reportes | Lista de reportes | Reporte → desde → hasta → filtro → «Generar» → grilla → «Volver» | — |

«Crear cuenta» no se recorrió: con teclado no se puede abrir (T-1).

## Qué le queda a JC

Nada de JC-R11. Si se quiere una confirmación con el teclado físico, alcanza con probar
Shift+Tab en el login (debería saltear Mail, igual que Tab) y Ctrl+Tab en una grilla (debería
salir de la grilla en una pulsación). Son 5 minutos.

Los tickets PDF de la compra de prueba (EA-000073) se borraron de `Documentos`.
