# Propuesta: esfuerzo visual desigual, donde se ve

Claude Code, a pedido de JC. **E-1, E-2 y E-3 están en el plan vigente**, como T-49 de
`ENCARGO-6B-BORRADOR.md`. Todas las horas son estimadas, de Codex.

**El criterio de JC: el esfuerzo va donde se ve.** Las 15 pantallas que se ven de pasada
quedan con el sistema de diseño parejo (6B). Las de abajo reciben algo más.

## 1. Las pantallas elegidas, y por qué esas

| Pantalla | Por qué |
|---|---|
| **`FrmCompraCliente`** (armado del paquete) | Sale en las figuras 2 y 3, y es el ejemplo de $ 99.625 de la página 54. Es el requerimiento central del trabajo y la pantalla donde el docente se va a quedar más tiempo |
| **`frmEventosCliente`** (listado) | Figura 1: la primera pantalla del recorrido del instructivo |
| **`frmConfirmacion`** y el **ticket** | Figura 4: el final del recorrido. El ticket ya se rehízo en 6A y no necesita más que el escaneo de JC |
| **`frmInicio`** (login), como cuarta, opcional | No sale en las figuras, pero es lo primero que se ve en cualquier demostración y en la defensa |

`frmPago` está en el medio del flujo. Recibe solo el indicador de pasos (abajo), para que el
recorrido no tenga un hueco.

## 2. Qué hacerles

| # | Idea | Dónde | Horas | ¿Rompe alguna de las pruebas? | Vale la pena |
|---|---|---|---|---|---|
| **E-1** | **Encabezado del flujo con indicador de pasos.** Un componente dibujado (`IndicadorPasos`, en su `OnPaint`): «1 Elegí el evento · 2 Armá tu paquete · 3 Pagá · 4 Listo», con el paso actual en `Primario` y los hechos con una marca. Va arriba de las cuatro pantallas del flujo, y debajo el título de cada una | Eventos, Compra, Pago, Confirmación | **5 a 8** | No toca ningún control que buscan las pruebas. **A confirmar con CP-13 y CP-25:** el encabezado crece unos 28 px y deja menos alto para el resumen al 150 %. El patrón de documento y vista lo absorbe, pero hay que correrlo | **Sí. Es lo que más «se ve de hoy»:** el indicador de pasos es el patrón de cualquier compra en línea actual, y además orienta. Ataca lo que el estudio de usabilidad mostró, que los perfiles sin experiencia no sabían en qué paso estaban |
| **E-2** | **TOTAL destacado.** Debajo del comprobante en Consolas, una franja con «TOTAL» y el importe en `Tema.Titulo` (18 pt) y `Primario`. `lblResumen` **no cambia** y sigue teniendo su línea TOTAL | Compra (y el mismo componente en Confirmación) | **2 a 3** | **No** rompe: `lblResumen` queda igual, y las pruebas lo leen a él. CP-25 tiene que seguir llegando al final de la tarjeta, que ahora es un poco más alta | **Sí.** La figura 3 y el ejemplo de la página 54 terminan en ese número. Hoy el TOTAL tiene el mismo tamaño que «Cargo por servicio». Es lo que `SISTEMA-DISENO.md` 7 declaraba imposible dentro de una sola etiqueta; con una etiqueta aparte, es barato |
| **E-3** | **Confirmación como estado de éxito.** Ícono grande de visto bueno en `Exito`, dibujado con la fuente de íconos de Windows (Segoe MDL2 Assets, que viene con Windows 10 y 11, sin archivos de imagen y nítida a cualquier escala). El número de pedido destacado. Cada ticket en la lista, con un ícono de documento | Confirmación | **3 a 4** (incluye el ayudante de íconos, 1 h) | **No**: conserva los nombres y los textos de los controles | **Sí.** Es la «recompensa» del recorrido y la figura 4. Hoy es un formulario gris con un título |
| **E-4** | **Login en dos columnas.** A la izquierda, una franja `Primario` con la «E» del ícono, «EventAura» en `Tema.Marca` y una frase; a la derecha, el formulario | Inicio | **3 a 4** | **No**, si se conservan los nombres (`txtMailLogin`, el botón de inicio, el foco inicial en el mail, que comprueba PA-01) | **Sí, si JC la pide** (queda en reserva). Es la primera impresión, pero no está en el documento. Es la cuarta, no una de las tres |
| E-5 | Tarjetas con esquinas redondeadas (componente `Tarjeta`, dibujado con suavizado y sin `Region`, que es lo que dentaba) | Las tarjetas de las cuatro pantallas | 3 a 4 | No | **No.** Se puede hacer bien, pero queda sola. La grilla, los campos, los desplegables y las casillas son nativos y rectos: una tarjeta redonda con controles rectos adentro se ve como un parche. Las esquinas rectas son coherentes con Windows y con todo lo demás |
| E-6 | Íconos en todos los botones (carrito en Comprar, flecha en Volver, candado en Pagar) | Las cuatro pantallas | 2 a 3 | No (el texto no cambia) | **No**, salvo los de E-3. Un ícono en cada botón agrega ruido. Con el indicador de pasos y el TOTAL, el flujo ya tiene sus puntos de atención |
| E-7 | Estados vacíos ilustrados («No hay eventos que coincidan», con dibujo) | Eventos | 2 a 3 | **Sí: rompe CP-16**, que exige una fila de aviso en la grilla | **No.** En la demostración casi nunca aparece y obliga a reescribir una prueba |
| E-8 | Eventos como tarjetas en lugar de grilla | Eventos | 8 a 12 | **Sí:** CP-16, CP-17, CP-18 y las del grupo A que usan `dgveventos` | **No.** Es lo más vistoso de la lista y lo peor en costo y riesgo. La figura 1 es la grilla, y la grilla con el tema queda bien |
| E-9 | Sombras, transparencias, animaciones, Mica | — | — | — | **No.** WinForms parpadea y las sombras falsas se ven más viejas que su ausencia (`SISTEMA-DISENO.md` 7) |
| E-10 | Una fuente propia (Inter u otra) | — | 3 a 5 | Riesgo en CP-62, que comprueba los nombres de las fuentes | **No.** Las fuentes privadas en WinForms no llegan a los controles nativos (desplegables, `MessageBox`) y quedarían dos tipografías. Segoe UI es la de Windows 11 |
| E-11 | Gráficos en los reportes (`System.Windows.Forms.DataVisualization`, que viene con .NET 4.8) | Reportes | 4 a 6 | No | **No ahora.** Es una pantalla del administrador, fuera de las cuatro. Queda anotado para el anexo A02 si sobra tiempo |

## 3. Recomendación

**Tres cosas bien hechas: E-1, E-2 y E-3. Son 10 a 15 horas estimadas, y están en el plan
vigente como T-49.** E-4 es la cuarta y queda en reserva: entra si JC la pide (3 a 4 h más).

**Ninguna de las cuatro rompe una prueba por diseño:**
- conservan los nombres, los tipos y los textos que las pruebas buscan;
- E-1 y E-2 agregan alto en la pantalla de compra, así que hay que correr CP-13 y CP-25 con
  ellas. El patrón de disposición está hecho justamente para eso;
- se agregan dos pruebas:
  - el TOTAL destacado coincide con la línea TOTAL de `lblResumen`;
  - el indicador marca el paso correcto en cada pantalla.

**Lo que no hago, con franqueza:**
- **esquinas redondeadas:** se ven bien solas, no al lado de controles nativos rectos;
- **íconos en todos los botones:** ruido;
- **estados vacíos ilustrados y tarjetas de eventos:** rompen pruebas a cambio de algo que
  casi no se ve en la demostración;
- **sombras, animaciones y fuente propia:** WinForms los hace mal.

**Cuándo.** En T-49, inmediatamente después de T-39, que rediseña Eventos, Pago y Confirmación
con el patrón. La compra se toca dos veces: T-41 la corrige y T-49 le agrega E-1 y E-2. Si JC
pide E-4, va en T-38.
