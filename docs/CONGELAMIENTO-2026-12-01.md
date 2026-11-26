# Congelamiento del código: 01/12/2026

Lista para mirar el **30/11**. Cada renglón se contesta con **sí** o **no**, sin interpretar. Se congela
el 01/12 solo si los doce dicen sí. Con un solo no, no se congela, y JC decide qué hacer.

Escrita por Claude Code el 26/11 a pedido de JC. Los tickets están en `docs/TICKETS.md`.

## Las doce condiciones

**A. Fallas que hoy están abiertas.** Las comprueba Claude Code **ejecutando la aplicación**, no
leyendo el código ni el reporte de Codex. Se usa el método de `docs/VERIFICACION-EJECUTABLE-2026-11-26.md`.

| # | Ticket | Se mira así | ¿Sí? |
|---|---|---|---|
| 1 | T-26 | Como Paula, desde «Mi cuenta» sin agrandar la ventana: Eventos disponibles → Ver entradas (Duki) → Armar paquete → Continuar al pago → Pagar. Termina con la compra **Confirmada** en la base. | |
| 2 | T-26 | En esa ventana de 1200 × 760 se ven **enteros**: las tarjetas del catálogo con «Volver» y «Ver como tabla», «Armar paquete» en la ficha, y el total y «Continuar al pago» en el armador. | |
| 3 | T-22 | En una copia sin `itext*.dll`, «Exportar PDF» de Reportes **no** dice «Ocurrió un error inesperado», y `errores.log` no crece. | |
| 4 | T-36 | `frmReportes`, abierto, tiene `ClientSize.Height` de **790 o menos**. | |
| 5 | T-28 | En el historial web, con una compra de dos entradas, los dos renglones y «Devolver» se ven **juntos, sin desplazar**. | |
| 6 | T-27 | En los pasos del alta, el texto sobre la foto tiene contraste de **4,5:1 o más**, medido con Playwright sin ventana. | |

**B. Pruebas automáticas.**

| # | Ticket | Se mira así | ¿Sí? |
|---|---|---|---|
| 7 | T-31 | CP-359 **pasa**. | |
| 8 | T-06 | Hay un registro de batería completa en `TestResults/` que cumple tres cosas: **cero fallas**, el hash anotado es igual a `git rev-parse HEAD`, y se corrió después del último commit que toca `Visual/`, `Repo/`, `Datos/` o `Pruebas/`. | |
| 9 | — | Cada prueba nueva desde el 24/11 tiene su fila en `docs/04-casos-prueba.md`, y ningún `CP-xx` se repite en los `[TestMethod]`. | |

**C. Decisiones de JC, anotadas en `docs/TICKETS.md`.** Cuando la decisión es hacer algo, ese algo cuenta
recién cuando está hecho y comprobado como en A.

| # | Ticket | Se mira así | ¿Sí? |
|---|---|---|---|
| 10 | T-05 | Codex entregó la lista de puntos, JC decidió cuáles entran, y los que entran están hechos. | |
| 11 | T-37 y T-29 | Está anotado: si el alta y la recuperación avisan sin la vista web, y si los cuatro hallazgos de T-29 entran. Si entran, están hechos. | |
| 12 | T-02 | O bien `CapturarReforma.ps1` tiene el parámetro del respaldo y se corrió a 144 DPI, o bien JC anotó que los dos quedan declarados como no hechos. | |

**Y además**, que no es una condición más: `git status` muestra limpias `Visual/`, `Repo/`, `Datos/` y
`Pruebas/`, salvo los dos `App.config` protegidos. Si no, el hash del renglón 8 no es el código que se entrega.

## Lo que no hace falta para congelar

- **T-32 (P4, una sola tabla).** Entra solo si los doce renglones están en sí antes del 29/11. Si entra,
  los renglones 8 y 9 se vuelven a mirar después.
- **Se hacen con el código ya congelado:** T-04 (la carpeta de la defensa) y T-08 a T-13 (batería final,
  marcadores, capturas, páginas, diagramas y arquitectura).
- **Las pruebas manuales de JC:** T-15 (escanear los tickets) y T-16 (las pantallas sin WebView2).

## Cuidado con los tiempos

- **Los renglones 1 a 6 necesitan el escritorio.** Claude Code no puede ejecutarlos mientras Codex trabaja
  con ventanas, así que el 30/11 tiene que haber un rato en que Codex esté parado. Lo mismo vale para la
  batería del renglón 8, que abre formularios reales.
- **Si el congelamiento se corre, el tiempo sale del documento.** La entrega del 10/12 no se mueve, así que
  cada día de atraso es un día menos para T-08 a T-13 y para la tarde de Máximo en Word (T-17).
