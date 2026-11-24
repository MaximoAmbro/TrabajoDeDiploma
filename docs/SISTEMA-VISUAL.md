# Sistema visual de EventAura

Referencia única para las 25 pantallas. Los colores y las fuentes salen de `Tema.cs`,
que ya existe: lo que falta no es la paleta, es aplicarla igual en todos lados.

## Escala de espaciado

Seis pasos, en píxeles lógicos a 96 DPI: **4 · 8 · 12 · 16 · 24 · 32**.
Ningún margen ni separación fuera de esa lista.

## Medidas de control

| Elemento | Regla |
|---|---|
| Botón | Alto 36, ancho mínimo 160 |
| Botones de un mismo grupo | Todos el mismo ancho, fijado por el más largo |
| Campo | Alto 32, ancho 320 |
| Etiqueta de campo | Arriba del campo, 4 px de separación, nunca adentro ni al costado |
| Entre campos | 16 px |
| Entre bloques | 24 px |
| Margen de pantalla | 32 px |
| Barra superior | Alto 56, igual en las 25 pantallas |
| Panel lateral | Ancho 240, ítem 240 x 40 |
| Foco | Borde de 2 px en Primario. Nunca el rectángulo negro |

## Relleno y contorno

Una sola acción principal por pantalla va en relleno. Todo lo demás en contorno.
En un panel lateral, el relleno marca únicamente dónde estás parado, no qué botón
es más importante.

## Tarjetas

- Todas las tarjetas de una misma fila tienen **el mismo alto**. El alto lo fija
  la más alta; las demás se estiran.
- Grilla de ancho fijo, no franjas de ancho completo. Una tarjeta de 900 px con un
  renglón de texto adentro es espacio muerto.
- El valor de una tarjeta **entra en un renglón**. Si no entra, baja el cuerpo o
  sube el ancho de la tarjeta; no se deja partir en dos líneas.
- Si una pantalla muestra dos cosas distintas (locales y eventos, por ejemplo),
  cada grupo lleva su encabezado de sección. No se mezclan en una lista corrida.

## Las tres plantillas

Las 25 pantallas entran en tres formas.

### 1. Formulario
Acceso, crear cuenta, recuperar contraseña, cambiar contraseña, registrar local,
agregar evento, agregar sector, pago.

- La marca aparece una sola vez, en la barra superior.
- El título dice qué hace la pantalla, no cómo se llama el sistema.
- La etiqueta va arriba de cada campo.
- El botón principal tiene **el mismo ancho que los campos** y arranca en el mismo
  borde izquierdo.
- Un campo que no tiene par ocupa el ancho completo de las dos columnas, no media
  pantalla con un hueco al lado.
- Los elementos auxiliares (medidor de fortaleza, casillas) se alinean con el campo
  al que pertenecen, no con el ancho total.

### 2. Menú con panel lateral
Los tres menús: cliente, organizador y administrador.

- Panel lateral de 240 px, ítems de 240 x 40, todos iguales, agrupados por sección
  con encabezado en mayúsculas.
- Un solo título: el de la pantalla donde estás.
- Quién sos va arriba a la derecha, en la barra.
- Las acciones viven en el lateral, no sueltas en el contenido.
- Nada de botones de navegación sueltos en el medio del lateral.
- El lado derecho muestra contenido real en tarjetas, agrupado por sección.

### 3. Panel con indicadores
El panel del administrador y los reportes.

- Grilla de tarjetas de ancho fijo, tres por fila, separación de 16.
- Todas las tarjetas de una fila, del mismo alto.
- Las filas siguientes usan la misma grilla; no cambia el ancho de columna de una
  fila a otra.

## Lo que no se hace, y es deliberado

- Esquinas redondeadas, sombras, animaciones, íconos en todos los botones, fuente
  propia. WinForms las dibuja mal y quedan más viejas que su ausencia. El plano
  chato y rectangular con Segoe UI es el aspecto nativo de Windows 11.
- No se renombran los controles existentes: hay pruebas que los buscan por nombre
  en texto.

## Verificación

- La comprobación de superposición incluye **todo control visible con texto**, no
  solo los interactivos. La versión que miraba solo los interactivos dejó pasar un
  título partido al medio con 360 pruebas en verde.
- Una prueba de medidas por plantilla: botones de un grupo con igual ancho, ítems
  del lateral de 240 x 40, ninguna etiqueta superpuesta con su campo, tarjetas de
  una fila con igual alto.
- Capturas a 100 % y a 150 % real de Windows, no escalado por software.
- La ventana se captura sola, sin que se vea el escritorio ni otra aplicación
  detrás de sus bordes.
