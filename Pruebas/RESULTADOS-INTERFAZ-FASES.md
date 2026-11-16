# Renovación de interfaz: registro por fases

## Fase 1: mapa de sectores

Pantalla: `FrmCompraCliente`. Control nuevo GDI+: `MapaSectores`. Usa sectores, precios y disponibles existentes; no agrega reglas ni datos. Los ordena por precio descendente y luego ID, con escenario arriba, texto de disponibilidad y trama para agotados. La distribución es orientativa, no un plano numerado del local. El clic usa la misma celda y gestor que la compra existente; flechas/Enter ofrecen alternativa al mouse y la tabla sigue permitiendo cantidades de distintos sectores. La altura crece con los sectores y el contenedor conserva desplazamiento.

Designer modificado: únicamente `Visual/Cliente/FrmCompraCliente.Designer.cs`, para incorporar el control y distribuir mapa y cantidades. Sin fuentes, colores ni referencias a Tema.

Validación final:

- Batería: **278/278** (59,2934 s).
- Barrera: **49/49** (2,2659 s), incluye CP-63 en 20 pantallas, siete filas de serialización, dos filas de componentes adicionales y 20 filas de texto de botones.
- Color renderizado: **31 primarios, 20 pantallas, 0 fallos**, con el control negativo de la herramienta existente.
- Texto nativo: **20/20 al 100 % y 20/20 al 150 %**, DPI comprobados 96 y 144. Escala original restaurada.
- Nuevas pruebas: **CP-150**, orden por precio y rechazo del clic en agotado; **CP-151**, clic conectado al armado real. Ninguna prueba anterior modificada. Los IDs quedan pendientes de incorporar a la matriz por Claude Code; este trabajo no modifica docs/.

Evidencia DrawToBitmap completa, en `Pruebas/Evidencias-Interfaz/Fase1/`:

| Captura | Tamaño | Letra menor a 16 cm | A 24 cm |
|---|---|---|---|
| antes-100.png | 886 × 573 | 6,14 pt | 9,21 pt |
| antes-150.png | 1326 × 856 | 6,16 pt | 9,24 pt |
| fase1-100.png | 886 × 573 | 6,14 pt | 9,21 pt |
| fase1-150.png | 1326 × 856 | 6,16 pt | 9,24 pt |

**No insertar estas vistas completas a 16 cm.** El ancho previsto de inserción es **24 cm en página apaisada**; si el documento exige 16 cm, necesitan recortes separados. La fuente menor es 9 pt en pantalla; conversión: puntos de fuente × DPI × (ancho en cm / 2,54) / ancho de captura en píxeles. No se confunden píxeles con puntos impresos.

Vueltas de validación: dos baterías completas después de integrar el mapa; dos barreras específicas (antes y después del ajuste de altura); dos recorridos finales de capturas y dos barridos nativos de texto. Además se tomaron los dos recorridos de capturas anteriores. Estos son recorridos de herramientas, no horas-persona. Portón humano pendiente: aspecto del mapa y ancho de inserción del informe; no bloquea las siguientes fases autorizadas.
