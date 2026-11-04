# Verificación y correcciones de JC — 04/11/2026

Continuación de [las correcciones del 03/11](CORRECCIONES-JC-2026-11-03.md), sobre
`9e9bbbe` más los cambios locales. No hubo commits, publicaciones ni cambios en las
consignas, el lenguaje, las reglas de precios o la estructura de la base.

> Commiteado después de esta verificación: `35624de` (compra), `861b02b` (confirmaciones
> y validaciones) y `95bd727` (pruebas; la clase `PruebasInterfazJc` pasó a llamarse
> `PruebasInterfaz` y sus capturas van a `TestResults/Interfaz/`).

## Resultado

**31/31 pruebas correctas**, ejecutadas con la tarea `Probar`/`tools/Proyecto.ps1`.
La compilación Debug con restauración no mostró errores ni advertencias. Se conservan
las 12 pruebas originales, incluida la referencia de $99.625, y las cuatro agregadas
el 03/11. Esta tanda agrega 15 casos ejecutados (incluidas las variantes de datos).

No equivale a repetir las 65 pruebas manuales originales: se cerraron los casos
específicos detallados abajo con formularios reales, mensajes nativos y datos aislados.

## Pendientes que se comprobaron

| Pendiente | Evidencia de esta corrida | Estado |
|---|---|---|
| JC-R02: evento agotado | Apertura real, servicios no disponibles deshabilitados y con texto que cabe; botón de compra deshabilitado | Comprobado por las pruebas de interfaz |
| JC-R03: comprar sin resultados | Clic en Comprar abre «Seleccionar evento», con el mensaje correcto; no abre otra compra | Comprobado con MessageBox nativo |
| JC-R04: guía de búsqueda | Windows devuelve la guía configurada y la caja tiene foco; imagen revisada con el texto visible | Comprobado a 96 DPI |
| JC-R05: volver con paquete | «No» es el botón por defecto; cancelar conserva cantidades y resumen; «Sí» vuelve al listado | Comprobados ambos caminos |
| JC-R06: abc y -1 | El mensaje indica «número entero de 0 en adelante» y se conservan la cantidad anterior y el resumen | Comprobadas las dos variantes |
| JC-R07: selección de devolución | Una entrada confirmada con seguro habilita el botón violeta; destildar lo deshabilita y vuelve gris | Comprobado con compra de prueba |
| JC-R08: eliminar/suspender | No por defecto; cancelar conserva el evento; confirmar elimina el evento sin ventas; RN-11 protege al que tiene ventas; se verificaron No/Sí en la suspensión | Comprobado con eventos de prueba |
| JC-R09: registro | Nombre, apellido y correo con espacios son rechazados con su aviso; contraseña abc es rechazada por el gestor; no se registra el correo de prueba | Comprobadas cuatro variantes |

Los diálogos se inspeccionan y responden en el hilo de cada prueba, mediante sus propios
controles nativos. El helper no envía teclas globales ni opera ventanas de otros procesos.
Los formularios de navegación abiertos por una prueba se liberan al terminar el hilo.

## Fallo nuevo: la compra no entraba en notebook al 150 %

La simulación de 150 % produjo una ventana de **1336 × 855 px** para un área útil de
**1366 × 728 px**. El tamaño anterior dejaba acciones fuera de la pantalla.

Se corrigió `FrmCompraCliente` para que:

- «Volver al listado» y «Comprar» permanezcan en una franja inferior fija.
- El resto del contenido tenga desplazamiento si no cabe.
- Al mostrarse, la ventana se limite al área útil del monitor y quede centrada.
- Los botones se realineen después de un cambio de tamaño o escala.

Se mantuvo el `ClientSize` declarado en Designer y el DPI de diseño. La agrupación del
contenido y las acciones se hace por código, conservando los controles existentes y
sus manejadores. Es un ajuste funcional por RNF-02/RNF-03, no un rediseño integral.

| Escala simulada | Área útil simulada | Resultado |
|---|---|---|
| 125 % | 1366 × 728 | La ventana entra y las dos acciones permanecen visibles |
| 150 % | 1366 × 728 | La ventana se acota al área; el contenido se desplaza y el final del resumen es accesible; ambas acciones permanecen visibles |
| 150 % | 1920 × 1032 | La ventana entra y las dos acciones permanecen visibles |

Los tres casos incluyen título de 100 caracteres, 2 entradas en cada uno de los tres
sectores, los cuatro servicios y seguro. Se comprueba que el título cabe, que cada línea
monetaria cabe en el resumen y que se puede desplazar hasta el final del panel.

**Límite de esta evidencia:** la pantalla física sigue a 96 DPI. Las pruebas amplían
tamaños y fuentes para simular el raster de 125 %/150 % y acotan la ventana a un área
de notebook. No cambiaron la escala de Windows ni constituyen una prueba real en otro
monitor. Esa aceptación sigue pendiente. En la notebook simulada al 150 %, ver el final
del resumen puede desplazar el título hacia arriba; los botones siguen visibles.

Imágenes locales generadas y revisadas:

- `TestResults/JC/compra-paquete-maximo.png` (96 DPI).
- `TestResults/JC/compra-simulada-150-1366x728.png` (desplazada hasta el resumen).
- `TestResults/JC/compra-simulada-125-1366x728.png`.
- `TestResults/JC/compra-simulada-150-1920x1032.png`.
- `TestResults/JC/eventos-guia-con-foco.png`.

## Fallo nuevo: los servicios no se rehabilitaban al liberarse cupo

Se creó un evento de capacidad 1 con Hospedaje, se compró su entrada con seguro y se abrió
la compra cuando estaba agotado. Una devolución liberó esa entrada mientras la ventana
seguía abierta. Al intentar anotarse, RN-10 devolvía a la compra normal, pero Hospedaje
permanecía deshabilitado.

La prueba reprodujo ese fallo. Se corrigió `MostrarSiAgotado` para actualizar las casillas
tanto al agotarse como al recuperarse el cupo, habilitando solo los servicios presentes
en el evento (RN-14). La prueba pasó después del cambio: vuelve la grilla, se habilitan
Comprar y Hospedaje, Traslado continúa deshabilitado porque no se ofrece y no se crea
una inscripción indebida en la lista de espera.

## Datos y alcance

Las compras, devoluciones, registros inválidos, eliminaciones y suspensiones de esta
tanda se ejecutaron exclusivamente contra `BddEventAura_Pruebas`. No se alteró la base
de desarrollo para preparar estos escenarios. Los datos `test…` se limpian al terminar.
Se conservaron las conexiones locales ya existentes en los dos `App.config`.

No se hicieron nuevos cambios de esquema. La auditoría de estructura del 03/11 sigue
siendo aplicable; no se certifica una migración de datos históricos sin el backup original.

## Lo que falta para cerrar esta etapa

1. **Windows a 125 % y 150 % reales**, incluida una notebook: compra, pago, confirmación,
   historial y navegación por teclado. No sustituirlo por las imágenes simuladas.
2. **JC-R10:** escanear dos tickets con cámara de celular. El contenido de los seis QR
   ya fue decodificado localmente y comparado con PDF/base el 03/11.
3. **JC-R11:** recorrido de los demás formularios cuyos títulos se corrigieron.
4. Completar los recorridos de la parte del compañero en `PRUEBAS-PENDIENTES.md`.
5. Confirmar si las consignas oficiales vigentes coinciden con los documentos del repo
   antes de decidir migraciones de tecnología, permisos/idioma y el rediseño integral.

No se considera el proyecto listo para entregar ni se modifica la decisión de evaluar
tecnología después de cerrar estas pruebas.
