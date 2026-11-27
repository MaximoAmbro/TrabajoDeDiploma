# Auditoría de pulido con rúbrica, 27/11/2026

Claude Code, a pedido de JC. **Codex: esto es lo que esperabas.** Las brechas más grandes están
abiertas como T-57 a T-64 en `docs/TICKETS.md`.

## Cómo se hizo

- **Sobre la aplicación real:** el ejecutable de `9977314`, compilado desde un clon limpio. El de
  `ed23ac0` es el mismo, porque `790546f` y `ed23ac0` solo tocan pruebas. Las cuentas fueron
  `plata@`, `oro@`, `admin@` y `organizador@`, contra la base de demostración.
- **Tamaño:** «Mi cuenta», el panel del Administrador y el del Organizador, en 1200 × 760. Las
  ventanas secundarias, al tamaño con que abren (casi todas de 1116 × 759).
- **Capturas:** `docs/verificaciones/muestra-1-2026-11-27/`. Cada fila nombra la suya.
- **Reglas de código:** `web-design-guidelines` (Vercel, instalada hoy y fijada en `5a37a63`),
  aplicada a `Visual/Web/` (sección 4).
- **Escala:** 96 DPI. A 144 DPI queda pendiente, con JC presente.
- **JC puntúa las mismas pantallas** para comparar. Por eso cada nota va con su captura.

**Rúbrica.** Siete criterios de 0 a 10; la nota es el promedio, redondeado a medio punto:

| Criterio | Qué mira |
|---|---|
| J · Jerarquía | Un título claro, la acción principal se distingue, el orden de lectura |
| E · Espaciado | Ritmo, alineación, márgenes; huecos o recortes |
| T · Tipografía | Escala coherente con el resto, una sola familia, sin texto cortado |
| C · Color | Paleta de `tema.css`, contraste, sin colores sueltos |
| S · Estados | Vacío, error, éxito, foco y deshabilitado se ven y se entienden |
| M · Movimiento | Transiciones según §10 de la propuesta; sin saltos de diseño |
| A · Acabado | Detalles: títulos de ventana, mayúsculas, tamaños, ventana aparte o dentro del panel |

## 1. Tabla, de mayor a menor brecha

**Brecha = 10 − nota.**

| # | Pantalla | Tipo | J | E | T | C | S | M | A | Nota | Brecha | Captura |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Compra confirmada | WinForms en «Mi cuenta» | 4 | 3 | 4 | 6 | 5 | 2 | 3 | **4,0** | 6,0 | `compra-4-tras-pagar.png` |
| 2 | Revisión de eventos (Administrador) | web, ventana aparte | 3 | 3 | 6 | 6 | 4 | 4 | 3 | **4,0** | 6,0 | `admin-Eventos-en-revisión.png` |
| 3 | Reportes (Administrador y Organizador) | web en ventana WinForms | 5 | 2 | 6 | 6 | 5 | 4 | 4 | **4,5** | 5,5 | `admin-rep-exportar.png`, `org-Reportes.png` |
| 4 | Auditoría | web, ventana aparte | 4 | 5 | 6 | 7 | 5 | 4 | 3 | **5,0** | 5,0 | `admin-Registro-de-auditoría.png` |
| 5 | Mis locales | web, ventana aparte | 3 | 4 | 6 | 7 | 4 | 4 | 4 | **5,0** | 5,0 | `org-Mis-locales.png` |
| 6 | Eventos del local | web, ventana aparte | 5 | 3 | 6 | 7 | 5 | 4 | 4 | **5,0** | 5,0 | `org-eventos-local.png` |
| 7 | Solicitudes de organizador | WinForms, ventana aparte | 6 | 4 | 5 | 6 | 4 | 3 | 4 | **5,0** | 5,0 | `admin-sol-2-aprobar.png`, `admin-sol-3-rechazar.png` |
| 8 | Verificar correo con sesión | WinForms en «Mi cuenta» | 6 | 4 | 5 | 7 | 4 | 4 | 4 | **5,0** | 5,0 | `verif-6-verificar2.png` |
| 9 | Agregar evento / Agregar sector | WinForms, ventanas aparte | 5 | 5 | 5 | 7 | 4 | 3 | 3 | **4,5** → 5,0 con el título corregido | 5,0 | `org-agregar-evento.png`, `org-agregar-sector.png` |
| 10 | Crear administrador | WinForms, ventana aparte | 6 | 5 | 6 | 7 | 5 | 3 | 4 | **5,0** | 5,0 | `admin-crear-administrador.png` |
| 11 | Control de ingreso | web, ventana aparte | 4 | 5 | 6 | 7 | 6 | 4 | 4 | **5,0** | 5,0 | `org-Control-de-ingreso.png` |
| 12 | Mi suscripción | web en «Mi cuenta» | 7 | 7 | 5 | 8 | 6 | 6 | 4 | **6,0** | 4,0 | `sus-1-inicio.png`, `sus-2-modal.png` |
| 13 | Pago | web en «Mi cuenta» (nuevo, `9977314`) | 6 | 5 | 7 | 8 | 6 | 5 | 5 | **6,0** | 4,0 | `compra-2-pago.png` |
| 14 | Ficha del evento | WinForms en «Mi cuenta» | 7 | 6 | 5 | 6 | 6 | 4 | 5 | **5,5** → 6,0 | 4,0 | `sus-4-tras-enter.png` |
| 15 | Administrar evento | WinForms, ventana aparte | 7 | 6 | 6 | 7 | 6 | 4 | 5 | **6,0** | 4,0 | `org-publicar.png` |
| 16 | Acceso de respaldo (sin WebView2) | WinForms | 7 | 6 | 6 | 7 | 7 | 4 | 6 | **6,0** | 4,0 | `sin-webview2-1-acceso.png` |
| 17 | Alta de cuenta | web | 8 | 6 | 8 | 8 | 7 | 6 | 6 | **7,0** | 3,0 | `alta-1.png`, `ojo-alta-ventana.png` |
| 18 | Recuperar contraseña | web | 8 | 6 | 8 | 8 | 7 | 6 | 6 | **7,0** | 3,0 | `ojo-recuperar-ventana.png` |
| 19 | Cambiar contraseña | web en «Mi cuenta» | 7 | 7 | 8 | 8 | 7 | 6 | 6 | **7,0** | 3,0 | `ojo-cambiar-ventana.png` |
| 20 | Catálogo («Eventos disponibles») | web en «Mi cuenta» | 8 | 7 | 8 | 8 | 7 | 6 | 7 | **7,5** | 2,5 | `espera-2-anotado.png` |
| 21 | Panel de plataforma (Administrador) | web | 8 | 7 | 7 | 7 | 7 | 6 | 7 | **7,0** | 3,0 | `admin-1-inicio.png` |
| 22 | Inicio del Organizador | web | 8 | 7 | 8 | 8 | 7 | 6 | 7 | **7,5** | 2,5 | `org-1-inicio.png` |
| 23 | Armador del paquete | web en «Mi cuenta» | 8 | 7 | 8 | 8 | 7 | 7 | 6 | **7,5** | 2,5 | `espera-1-armador.png`, `compra-1-tras-continuar.png` |
| 24 | Inicio de «Mi cuenta» | WinForms con tarjetas | 8 | 8 | 8 | 8 | 7 | 6 | 8 | **7,5** | 2,5 | `sus-3-tras-esc.png` |
| 25 | Acceso | web | 9 | 8 | 8 | 8 | 8 | 6 | 8 | **8,0** | 2,0 | `ojo-acceso-ventana.png` |

**Sin captura nueva hoy:** Bienvenida, Mis entradas y Mi historial. «Mis entradas» tenía el título
repetido (T-46). `3be9960` unifica los títulos y CP-379 lo comprueba en el DOM, pero no lo miré
en pantalla.

## 2. Qué baja cada nota, en una línea

1. **Compra confirmada.** El tilde queda suelto a la izquierda del título; el recibo va en monoespaciada
   y en un cuadro con su propia barra; «Guardadas en Documentos.» parece un campo de texto; el
   paso «4 Listo» repite el título. Es la última pantalla de la compra y la que más se va a ver
   en la demo.
2. **Revisión de eventos.** Ventana aparte de 1116 × **823**, más que el techo de 790 de RNF-03.
   Una franja vacía donde iría el título; botones grises deshabilitados fuera de la tarjeta;
   «Cerrar» suelto abajo a la izquierda.
3. **Reportes.** El gráfico ocupa una franja de unos 140 px con barra propia y deja vacía la mitad
   de abajo; el aviso de exportación tapa el campo «Hasta»; el formulario es WinForms y el
   gráfico, web.
4. **Auditoría.** También mide 823 px de alto y no tiene título (franja vacía arriba).
5. **Mis locales.** Sin título; la tarjeta elegida no se distingue de la otra; botones a media
   altura y la mitad de abajo vacía.
6. **Eventos del local.** La lista se corta en la segunda fila (las etiquetas «Publicado» quedan
   tapadas por el borde) y debajo de los botones hay 230 px vacíos. «Administrar Evento» y
   «Agregar Evento» van con mayúscula, al revés que el resto.
7. **Solicitudes.** El aviso de éxito tapa la vista previa del documento; el motivo queda escrito
   para la solicitud siguiente; sin motivo, «Aprobar» no muestra nada; el estado vacío queda
   corrido.
8. **Verificar correo.** Debajo del formulario queda un panel gris vacío del alto de media
   pantalla. Enter no confirma el código, y la ruta de la bandeja simulada ocupa tres renglones.
9. **Agregar evento / sector.** La ventana dice «Registrar evento» y el encabezado «Agregar
   eventos», en plural. La otra dice «Agregar sector» y «Registrar sector». Al agregar un sector
   no hay aviso y el formulario queda abierto y vacío.
10. **Crear administrador.** Usa la casilla «Mostrar contraseña» y no el ojo (T-43 la quitó del
    resto); el enlace «Volver a administración» no se alinea con el botón.
11. **Control de ingreso.** Sin título; «Volver al panel» y «Volver» hacen lo mismo, en dos lugares.
12. **Mi suscripción.** **El círculo de cada opción tapa el nombre del plan («M●nsual», «A○al»).**
    Al cancelar no hay aviso de éxito.
13. **Pago.** La vista no ocupa todo el ancho (queda un hueco de unos 200 px a la derecha);
    «Reserva: 09:52» flota sin rótulo; falta el indicador de pasos que sí tiene el armador.
14. **Ficha del evento.** El mapa es un bloque amarillo con texto chico centrado. La tipografía
   es distinta de la del armador, y hay un globo («General · $ 12.500,00 · 3…») que se sale del
   borde derecho.
15. **Administrar evento.** Dos botones «Agregar sector» cuando no hay sectores (el del estado
    vacío y el de abajo).
16. **Acceso de respaldo.** Correcto y con aviso; es WinForms plano, como corresponde a un
    respaldo.
17. **Alta y recuperación.** Hay dos logos de EventAura, el de la barra y el del panel oscuro. A
   1100 × 720, «Volver» y «Crear cuenta» quedan cortados abajo y hay que desplazar.
19. **Cambiar contraseña.** Una barra de desplazamiento dentro de una tarjeta que entra entera.
20. a 25. Detalles menores. En el armador, el aviso del seguro empuja «Continuar al pago» debajo
   del borde (T-53), y el desglose tiene una barra propia de 80 px.

## 3. Títulos duplicados o que faltan

- **Dentro de «Mi cuenta» ya no se repiten:** `3be9960` deja un solo título, el del shell.
  CP-379 lo prueba en el DOM. Lo vi en el catálogo, la suscripción, el pago y el cambio de
  contraseña.
- **Decisión de Codex en `790546f`:** durante el armado, el shell sigue diciendo «Eventos
  disponibles», no «Armar paquete». Es coherente con el menú, pero el título no nombra la
  pantalla. **A decidir por JC.**
- **Las ventanas aparte del Administrador y del Organizador tienen el problema contrario:**
  - auditoría, revisión de eventos, mis locales, eventos del local y control de ingreso dejan
    vacía la franja del título;
  - en agregar evento, agregar sector y solicitudes, la barra de la ventana y el encabezado
    dicen cosas distintas.

## 4. `web-design-guidelines` sobre `Visual/Web/`

Reglas de `web-interface-guidelines@434b7f9`. Solo lo que aplica a una vista local sin red:

```text
## Visual/Web/sistema.css
Visual/Web/sistema.css:53 - outline: none en [tabindex="-1"]:focus; está bien solo si ese foco es programático (títulos). Confirmar que ningún control interactivo usa tabindex=-1
## Visual/Web/catalogo.css, datos.css, gestion.css, alta-cuenta.css
transiciones sin @media (prefers-reduced-motion); solo armador, compra, sistema y suscripción la tienen
## Visual/Web/suscripcion.html
<dialog> sin overscroll-behavior: contain
## Visual/Web/alta-cuenta.html
alta-cuenta.html:125 - <img id="vista-documento"> sin width/height
## Visual/Web/*.html
campos de mail y de código sin spellcheck="false" (solo gestion.js lo usa)
```

Pasan: no hay `transition: all`, `user-scalable=no` ni `<div onclick>`; hay `aria-live` en
todas las vistas con avisos; las cifras usan `tabular-nums`; los importes salen de `Intl`.

## 4b. Bloquea la demo y no es de pulido

T-47 (Esc en la suscripción), T-48 (publicar un evento ya pasado) y T-49 (ventanas de 823 px),
en `docs/VERIFICACION-MUESTRA-1-2026-11-27.md`.

## 5. Las 8 brechas más grandes → tickets

| Ticket | Pantalla | Prioridad |
|---|---|---|
| T-57 | Compra confirmada | P1: es el cierre de la demo |
| T-58 | Revisión de eventos | P2 (la altura es T-49, P1) |
| T-59 | Reportes: el gráfico en una franja | P1: el Administrador lo muestra |
| T-60 | Auditoría (título; la altura es T-49) | P2 |
| T-61 | Mis locales | P2 |
| T-62 | Eventos del local | P2 |
| T-63 | Solicitudes de organizador | P2 |
| T-64 | Verificar correo con sesión | P2 |

Las animaciones aprobadas por JC (apertura del modal, estado de éxito, aviso que aparece) entran
en T-57 (éxito) y en el modal de la suscripción. Hoy, el modal aparece sin transición.
