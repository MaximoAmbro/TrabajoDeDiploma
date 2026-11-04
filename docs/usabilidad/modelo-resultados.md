# Resultados del modelo (generado por tools/Usabilidad/modelo.py)

> Estudio de usabilidad **simulado**. Las acciones salen de la medición sobre la aplicación real;
> los tiempos, errores, satisfacción y SUS se derivan con las reglas del script.

## Tiempo por paso: estimado del documento y KLM experto

| Nº | Paso | Estimado doc. | KLM experto | Acciones medidas (mouse) |
|---|---|---|---|---|
| 0 | Iniciar sesión | — | 12.8 s | 1 clic(s), 30 tecla(s), 0 palabras a leer |
| 1 | Buscar el evento | 0:45 | 6.9 s | 1 clic(s), 0 tecla(s), 12 palabras a leer |
| 2 | Seleccionar el evento | 0:15 | 4.0 s | 2 clic(s), 0 tecla(s), 0 palabras a leer |
| 3 | Elegir sector y cantidad | 0:40 | 8.4 s | 1 clic(s), 2 tecla(s), 20 palabras a leer |
| 4 | Agregar servicios | 1:00 | 5.9 s | 1 clic(s), 0 tecla(s), 12 palabras a leer |
| 5 | Decidir el seguro | 0:20 | 10.0 s | 1 clic(s), 0 tecla(s), 25 palabras a leer |
| 6 | Revisar el resumen | 0:30 | 12.1 s | 0 clic(s), 0 tecla(s), 45 palabras a leer |
| 7 | Completar el pago | 1:00 | 21.3 s | 1 clic(s), 39 tecla(s), 25 palabras a leer |
| 8 | Confirmar y recibir el comprobante | 0:10 | 9.8 s | 1 clic(s), 0 tecla(s), 30 palabras a leer |
| | **Pasos 1 a 8** | **4:40** | **1:18** | |

## Tiempos, errores y asistencia por perfil

| Perfil | Descripción | Tiempo total (con ingreso) | Pasos 1 a 8 | Errores | Fricciones | Asistencia |
|---|---|---|---|---|---|---|
| P1 | 21, estudiante universitario, experiencia alta | 1:26 | 1:15 | 0 (—) | — | No |
| P2 | 34, empleada administrativa, experiencia media | 1:42 | 1:28 | 0 (—) | — | No |
| P3 | 52, comerciante, sin experiencia en compras en línea | 4:16 | 3:33 | 3 (E-A, E-B, E-C) | — | Sí — paso 3: nada indica que la cantidad se escribe en la celda |
| P4 | 19, estudiante secundario, experiencia alta, usa más el celular | 1:41 | 1:23 | 0 (—) | F-1 | No |
| P5 | 45, docente, experiencia media | 2:13 | 1:55 | 1 (E-B) | — | No |
| P6 | 67, jubilado, baja visión, usa solo teclado y lupa | 4:30 | 3:46 | 2 (E-C, E-D) | F-1, F-2 | Sí — paso 3: con el teclado hay que llegar con flechas a la celda Cantidad de Campo, sin indicación |
| **Promedio** | | **2:38** | **2:13** | **1.0** | | **4 de 6 sin asistencia** |

Promedio sin los perfiles asistidos: 1:45.

## Tiempo por paso y perfil (segundos, incluye errores y fricciones)

| Nº | Paso | P1 | P2 | P3 | P4 | P5 | P6 | Promedio | Estimado doc. |
|---|---|---|---|---|---|---|---|---|---|
| 0 | Iniciar sesión | 10 | 14 | 44 | 18 | 18 | 44 | 25 | — |
| 1 | Buscar el evento | 7 | 8 | 12 | 7 | 9 | 13 | 9 | 45 |
| 2 | Seleccionar el evento | 4 | 4 | 6 | 4 | 5 | 10 | 6 | 15 |
| 3 | Elegir sector y cantidad | 8 | 9 | 33 | 9 | 23 | 20 | 17 | 40 |
| 4 | Agregar servicios | 6 | 7 | 25 | 6 | 8 | 11 | 10 | 60 |
| 5 | Decidir el seguro | 10 | 12 | 17 | 10 | 13 | 48 | 18 | 20 |
| 6 | Revisar el resumen | 12 | 14 | 21 | 12 | 16 | 28 | 17 | 30 |
| 7 | Completar el pago | 18 | 23 | 82 | 24 | 29 | 75 | 42 | 60 |
| 8 | Confirmar y recibir el comprobante | 10 | 11 | 17 | 10 | 13 | 20 | 13 | 10 |

## Satisfacción por criterio (1 a 5) — derivada

Regla: se parte de 5 y se resta 1 por cada fricción medida que el perfil encuentra en ese criterio (detalle en el informe). «Satisfacción general» es el promedio redondeado de los otros cinco, menos 1 si el perfil necesitó asistencia.

| Criterio | P1 | P2 | P3 | P4 | P5 | P6 | Prom. |
|---|---|---|---|---|---|---|---|
| Claridad de la navegación | 4 | 4 | 3 | 3 | 4 | 1 | 3.2 |
| Facilidad para armar el paquete | 5 | 5 | 2 | 5 | 4 | 4 | 4.2 |
| Comprensión del resumen y del descuento | 5 | 4 | 4 | 5 | 4 | 4 | 4.3 |
| Claridad de los mensajes de error | 5 | 5 | 3 | 5 | 5 | 3 | 4.3 |
| Confianza en el proceso de pago | 5 | 5 | 4 | 5 | 5 | 4 | 4.7 |
| Satisfacción general | 5 | 5 | 2 | 5 | 4 | 2 | 3.8 |
| **Promedio global** | | | | | | | **4.08** |

Perfiles con satisfacción general buena o muy buena (4 o 5): 4 de 6 (67 %).

## System Usability Scale — derivado

Reglas de las respuestas (1 a 5) en el informe, sección «System Usability Scale».

| Perfil | Ítems 1 a 10 | Puntaje SUS | Grado (Bangor et al., 2009) |
|---|---|---|---|
| P1 | 4 1 5 1 5 2 5 2 5 1 | 92.5 | A |
| P2 | 4 1 5 1 5 2 4 2 5 2 | 87.5 | B |
| P3 | 2 4 2 4 2 3 2 3 4 4 | 35.0 | F |
| P4 | 4 1 5 1 5 3 5 3 5 1 | 87.5 | B |
| P5 | 4 2 4 2 4 2 4 2 5 2 | 77.5 | C |
| P6 | 2 2 2 4 4 3 2 5 4 4 | 40.0 | F |
| **Promedio** | | **70.0** | |
