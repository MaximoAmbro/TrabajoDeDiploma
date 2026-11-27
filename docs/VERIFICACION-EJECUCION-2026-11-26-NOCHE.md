# Verificación en ejecución del 26/11 a la noche: T-38, Enter de la suscripción y guion

Hecha por Claude Code entre las 20:46 y las 21:35. El escritorio estuvo tomado con `.escritorio.lock` desde las
20:47. Al terminar se borró el lock, se cerró `Visual.exe` y Windows quedó en 96 DPI (100 %), comprobado.

## T-38: el cierre al cambiar de ventana

- **El registro (20:46):** `errores.log` no tiene nada después de las 18:43, y el Visor de eventos no tiene ningún
  1000, 1001 ni 1026 entre las 20:20 y las 20:50. El ejecutable ya tenía el registro de cierres. Cerrar sin dejar
  ningún rastro es lo que pasa cuando otro proceso termina la aplicación; con Visual Studio, el depurador. **Es una
  deducción.**
- **5 vueltas sin depurador, de 20:48 a 20:57:**
  1. pago abierto;
  2. pasar al Bloc de notas 40 s;
  3. Win+Shift+S con un recorte;
  4. volver con Alt+Tab;
  5. cancelar.

  **Ningún cierre.** Evidencia en `docs/verificaciones/cierre-al-cambiar-de-ventana-2026-11-26/`. El detalle y lo que
  le toca a JC están en T-38 (`docs/TICKETS.md`).
- **Lo que quedó en `BddEventAura`:** 5 compras Pendientes más de `bronce@`, de la 24 a la 28, sin reserva. No se
  borraron.

## Enter en «Mi suscripción»

Con Playwright sobre `Visual/Web/suscripcion.html` de `a9f9c1b`, con un puente de prueba. Pasan los seis casos:
`docs/verificaciones/suscripcion-enter-2026-11-26/RESULTADO.md`. En la aplicación real, Enter abrió «Contratar
suscripción» (abajo).

## Guion

Sobre el ejecutable de `5890e70`, contra `BddEventAura_Guion_Demostracion`. Era una base nueva con `01`, `02` y la
semilla de demostración, y se borró al terminar. Capturas y scripts en `docs/verificaciones/guion-2026-11-26-noche/`.

| Sección | Resultado |
|---|---|
| 2, recuperación con el código (`malena.rios@`) | **Pasa.** El `.eml` correcto, «La contraseña se actualizó.», entra con la nueva. La recuperación no muestra la ruta de la bandeja |
| 4, compra sin seguro | **Pasa.** El aviso de seguro dentro del armador y **Seguir sin seguro**; EA-000320, $ 28.750,00 (Bronce) |
| 4, compra con seguro | **Pasa.** EA-000321, $ 33.750,00 con seguro de $ 5.000 |
| 4, devolución | **Pasa.** «Devolución registrada. Se reintegran $ 25.000,00.», entrada Reembolsada |
| 6.1, contratar con Enter | **Pasa.** «Contratar suscripción» → Sí → «Plan Mensual activo · Vence el 26/12/2026» |
| 5, 6.3 (preventa), 7, 8 y 144 DPI | **No recorridos** (ver abajo) |

**Fallas, como T-46:**
- «Mis entradas» se titula «Mis eventos», con un segundo título;
- los campos de Pago no son `Edit` para UI Automation;
- una alineación del pago.

**Por qué quedaron afuera 5, 7, 8 y 144 DPI:**
- **Tiempo.** Cada sección se automatiza aparte, porque las vistas web y los campos del pago no se pueden manejar
  con nombres estables.
- **Lo que se intentó para ir más rápido.** Abrir la depuración remota de WebView2 y manejar las vistas con
  Playwright. La variable de entorno no alcanza, porque la aplicación fija sus propios argumentos
  (`PanelWebMotor.cs:53`). La alternativa, una política en el registro de Windows, la bloqueó el control de
  permisos y no la reintenté.
- **144 DPI.** Cambiar la escala se hace desde Configuración, y ya pasó que quedó en 150 %. No lo hice sin JC
  presente.

**Un cierre provocado por la automatización, no T-38.** A las 21:16:53 la aplicación de la base de demostración
se cerró con `UserClosing` en sus tres ventanas, y `errores.log` lo registró como cierre ordenado. En ese momento,
mis clics y teclas caían sobre una consola que tapaba la mitad izquierda del pago. Probablemente lo
provocó la automatización, pero no lo pude confirmar: ninguna de las teclas que mandé era un cierre. El registro de cierres de T-38 dejó el origen, como se esperaba.
