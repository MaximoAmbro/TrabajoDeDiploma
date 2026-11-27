# Enter en «Mi suscripción», con Playwright sobre el HTML (26/11, noche)

**Qué se probó:**
- `Visual/Web/suscripcion.html` y `suscripcion.js` tal como están en `a9f9c1b`, el último commit que los toca.
- Se abrieron en Chromium (Playwright) a 1000 × 560, con un **puente de prueba**: antes de cargar la página se
  define `window.chrome.webview.postMessage`, que guarda cada mensaje en una lista. Así la página cree que está
  dentro de WebView2 y no usa el panel «Maqueta».
- Los datos se dibujan con `window.eventAuraActualizar`, como hace `PanelWeb`.

**Esto no es la aplicación.** Prueba lo que la página le manda a la pantalla WinForms. Que la pantalla abra la
confirmación lo verificó la pasada en ejecución de la tarde (Enter abrió «¿Contratar el plan Mensual por
$ 9.000,00?»). JC lo comprueba a mano.

## Resultado

| Caso | Mensajes recibidos por el puente | Resultado |
|---|---|---|
| Foco en Mensual, Enter | `{tipo: "contratar", plan: "Mensual"}` | **Pasa** |
| Flecha derecha (queda Anual marcado), Enter | `{tipo: "contratar", plan: "Anual"}` | **Pasa** |
| Foco en **Contratar**, Enter | un solo `contratar` con `Anual`: no se duplica | **Pasa** |
| Enter con el foco fuera de los planes | ninguno | **Pasa** |
| Escape | `{tipo: "escape"}` | **Pasa** |
| Suscripción activa: radios deshabilitados, Enter | ninguno; **Contratar** oculto | **Pasa** |

**Geometría, de paso:** a 560 px de alto, **Contratar** ocupa 506 a 546 y la página mide 578. El botón entra, pero la
página se desplaza 18 px. Concuerda con el detalle de T-45: dentro de «Mi cuenta», a 1216 × 799, se veían 15 de
sus 41 px. Captura: `suscripcion-enter.png`.

El código que lo hace es `suscripcion.js`. Escucha `keydown` en el documento y solo actúa con Enter dentro de
`#planes`. Si **Contratar** está visible, lo pulsa. El botón con foco usa el Enter nativo, que no pasa por ese
manejador. Por eso no hay doble envío.
