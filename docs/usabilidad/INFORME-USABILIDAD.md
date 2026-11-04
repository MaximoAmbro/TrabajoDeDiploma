# Informe de pruebas de usabilidad (simulado)

EventAura · Trabajo de Campo I · noviembre de 2026. Reemplaza los resultados de las páginas 90
a 97 del documento, con la misma estructura.

> **Declaración sobre la naturaleza de los datos.** Las sesiones de este informe son
> **simuladas**, igual que las minutas de reunión del documento: no participaron personas.
> A diferencia de la versión anterior, **el comportamiento de la interfaz no se supuso: se
> midió sobre la aplicación real.** Qué se midió y qué se derivó:
>
> | Medido sobre la aplicación real | Derivado de los perfiles |
> |---|---|
> | Acciones, pantallas, campos y caracteres de la tarea | Tiempos por perfil, con el Keystroke-Level Model sobre las acciones medidas |
> | Orden de tabulación real de cada pantalla | Errores probables de cada perfil, cada uno atado a un hecho medido |
> | Texto, título y botón por defecto de cada diálogo | Necesidad de asistencia, por recorrido cognitivo |
> | Mensajes ante cada dato inválido | Satisfacción por criterio y SUS, con reglas explícitas |
> | Pérdidas de trabajo y callejones sin salida | — |
>
> Si se ejecutan sesiones con personas, se reemplazan las tablas de la sección «Resultados»
> sin cambiar el resto: lo medido sigue valiendo.

Versión evaluada: commit `3af4154` y siguientes, con la semilla de T-21 (evento «Duki en
Rosario»; clienta Plata con 4 compras), Windows 11 a escala 100 %, 1920 × 1080.

---

## 1. Objetivo de la evaluación

Verificar que un usuario sin experiencia técnica pueda completar el flujo de armado y compra
de un paquete personalizado (el requerimiento core) dentro del tiempo comprometido, e
identificar los puntos del recorrido que generan confusión, error o pérdida de trabajo. Se
verifica además que las seis correcciones del estudio anterior (H-01 a H-06) estén en la
interfaz.

## 2. Diseño de la prueba

| Aspecto | Definición adoptada |
|---|---|
| Tipo de estudio | Evaluación sin usuarios, con tres métodos formales sobre la aplicación real: (a) **medición instrumentada** de la interfaz; (b) **evaluación heurística** con las diez heurísticas de Nielsen (1994); (c) **recorrido cognitivo** (Wharton, Rieman, Lewis y Polson, 1994) por perfil, con tiempos del **Keystroke-Level Model** (Card, Moran y Newell, 1980) |
| Objeto evaluado | Flujo de ocho pasos del Instructivo del requerimiento core, más el ingreso al sistema |
| Perfiles | Seis: los cinco del estudio anterior, con la misma edad y experiencia, y uno nuevo que usa solo teclado |
| Instrumentos | `tools/Usabilidad/Medir.ps1` (medición), `tools/Usabilidad/VerificarCierreConX.ps1` (UI Automation sobre el ejecutable), `tools/Usabilidad/modelo.py` (tiempos, errores, satisfacción y SUS) |
| Registros | [medicion.txt](medicion.txt), [cierre-con-x.txt](cierre-con-x.txt), [modelo-resultados.md](modelo-resultados.md) |

**Cómo se midió.**
- **La medición instrumentada** abre los formularios reales de `Visual.exe`, con sus gestores
  y una base recién creada con la semilla, y actúa sobre los controles como el usuario: escribe
  en los campos, elige filas, tilda casillas y pulsa botones. Registra:
  - cada acción;
  - el orden de tabulación que recorre WinForms;
  - el texto, el título y el botón por defecto de cada diálogo nativo;
  - qué queda en pantalla después de cada caso de error.
- **No se enviaron teclas al escritorio.** Las teclas que hacían falta (Enter) se mandaron a
  las ventanas de la propia aplicación.
- **Los callejones sin salida se confirmaron además sobre el ejecutable** con UI Automation.

**Por qué este diseño.** La evaluación heurística y el recorrido cognitivo son los métodos de
inspección habituales cuando no hay participantes disponibles, y Nielsen y Landauer (1993)
muestran que tres a cinco evaluadores o perfiles detectan la mayor parte de los problemas.
Atar cada conclusión de un perfil a un hecho medido evita el defecto principal de la versión
anterior: errores y tiempos que no tenían respaldo en la interfaz.

## 3. Perfiles

| Perfil | Edad | Ocupación | Soltura con la tecnología | Experiencia comprando entradas en línea | Parámetros del modelo |
|---|---|---|---|---|---|
| P1 | 21 | Estudiante universitario | Alta | Alta: compra habitualmente | Tipeo 0,20 s/tecla; lectura 250 ppm |
| P2 | 34 | Empleada administrativa | Media-alta | Media: compró varias veces | 0,28 s; 220 ppm; decisiones ×1,3 |
| P3 | 52 | Comerciante | Baja | Nula: nunca compró en línea | 1,20 s; 150 ppm; decisiones ×2; mouse ×1,5 |
| P4 | 19 | Estudiante secundario | Alta, más en el celular que en la PC | Alta | 0,35 s; 250 ppm; decisiones ×1,1 |
| P5 | 45 | Docente | Media | Media: compró alguna vez | 0,40 s; 200 ppm; decisiones ×1,5; mouse ×1,2 |
| P6 | 67 | Jubilado | Baja; baja visión, usa lupa | Nula | Solo teclado; 0,90 s; 110 ppm; decisiones ×2,2 |

**Referencias de los parámetros.** Las velocidades de tipeo son las del Keystroke-Level
Model: 0,20 s por tecla para un buen tipeador, 0,28 s para uno promedio y 1,20 s para quien
no está familiarizado con el teclado. Los demás operadores, iguales para todos: apuntar con el
mouse 1,10 s, hacer clic 0,20 s, pasar la mano del teclado al mouse 0,40 s y preparación
mental 1,35 s.

## 4. Tarea evaluada y protocolo

**Escenario** (el mismo del documento): «Querés ir a ver a Duki en Rosario con un
acompañante. Además de las entradas, necesitás dónde alojarte esa noche. Comprá lo necesario
usando la plataforma.»

**Criterio de éxito** (el mismo del documento): llegar a la pantalla de confirmación con un
paquete que incluya al menos una entrada y un servicio complementario, sin asistencia.

**Protocolo en este diseño.**
- **Asistencia:** se considera que un perfil la necesita cuando el recorrido cognitivo responde
  «No» a «¿notará que la acción correcta está disponible?» en un paso que no ofrece ninguna
  pista visible. Equivale al bloqueo de más de sesenta segundos del protocolo original.
- **Error:** una acción que obliga a rehacer un paso, la misma definición del documento.

## 5. Métricas e instrumentos

| Métrica | Forma de medición en este estudio | Umbral comprometido |
|---|---|---|
| Tasa de éxito | Perfiles que llegan a la confirmación sin asistencia | 3 de 5 (60 %) |
| Tiempo de ejecución | KLM sobre las acciones medidas, con los errores de cada perfil | Menos de 5 min (RNF-01) |
| Errores | Errores atados a hechos medidos, por perfil | Hasta 2 por perfil |
| Satisfacción por criterio | Los seis criterios del documento, de 1 a 5, con reglas explícitas | Promedio mayor que 4 |
| System Usability Scale | Diez ítems (Brooke, 1996), cada uno derivado de un criterio o de un hecho medido | 68 o más |

---

## 6. Lo medido sobre la aplicación real

### 6.1 La tarea completa

La medición completó la tarea de punta a punta con el pedido EA-000071.

| Medida | Valor |
|---|---|
| Acciones del usuario | **18**: 15 clics (6 para entrar a un campo), 7 campos completados, 65 caracteres tipeados, 2 teclas Enter |
| Pantallas | **6 pantallas y 1 diálogo**: Iniciar sesión → Mi cuenta → Eventos disponibles → Armar paquete → (diálogo «Seguro de entrada») → Pago seguro → Compra confirmada |
| Campos, en orden | Mail (19 caracteres) → Contraseña (10) → Cantidad de Campo en la grilla (1) → Titular (11) → Número de tarjeta (16) → Vencimiento (5) → Código de seguridad (3) |
| Resumen obtenido | Base $50.000; descuento Plata −$2.500; hospedaje $45.000; cargo $7.125; **total $99.625**, igual que la página 54 |

**Secuencia exacta medida:**

| Nº | Acción |
|---|---|
| 1 | Escribir el mail |
| 2 | Escribir la contraseña |
| 3 | Clic en «Iniciar sesión» (Enter no funciona) |
| 4 | Clic en «Eventos disponibles» |
| 5 | Clic en la fila «Duki en Rosario» |
| 6 | Clic en «Comprar» |
| 7 | Clic en la celda Cantidad de Campo |
| 8 | Escribir «2» |
| 9 | Enter para confirmar la celda |
| 10 | Clic en «Hospedaje» |
| 11 | Clic en «Comprar» |
| 12 | Clic en «Sí» en el aviso del seguro |
| 13 a 16 | Completar titular, número, vencimiento y código |
| 17 | Enter para pagar |
| 18 | Clic en «Aceptar» |

### 6.2 Orden de tabulación de cada pantalla

Recorrido real de WinForms con la tecla Tab, comparado con el orden de lectura (de arriba
abajo y de izquierda a derecha).

| Pantalla | Foco al abrir | Enter / Esc | Problemas medidos |
|---|---|---|---|
| Iniciar sesión | Mail | Ninguno / ninguno | **El campo Mail no tiene tabulación** (`TabStop` desactivado): desde la contraseña no se vuelve a él con Tab ni con Shift+Tab. **Enter no inicia sesión**. Hay un botón «Modificar» fuera del área visible, que igual recibe tabulación |
| Crear cuenta | **Contraseña**, no Nombre | Ninguno / ninguno | Orden Contraseña → Confirmar → Mostrar → Mail → Registrarse → Nombre → Apellido → Tipo: **dos saltos hacia atrás** |
| Mi cuenta | Eventos disponibles | Ninguno / ninguno | Ninguno |
| Eventos disponibles | Buscador | Ninguno / ninguno | **El primer Tab va a «Volver»**, abajo a la izquierda, y después sube al buscador. Enter en el buscador sí busca |
| Armar paquete | Grilla, celda «Sector» de VIP | Ninguno / ninguno | «Comprar» antes que «Volver al listado», que está a su izquierda: salto menor e intencional |
| Pago seguro | Titular | **Enter paga** / ninguno | «Pagar» antes que «Cancelar pago», intencional |

### 6.3 Diálogos y mensajes

| Situación | Título | Texto | Botón por defecto |
|---|---|---|---|
| Login vacío | «Campo vacíos» | «Por favor, ingrese usuario» (el campo se llama «Mail») | Aceptar |
| Contraseña incorrecta | «Inicio de sesión» | «Mail o contraseña incorrectos. Le quedan 4 intentos antes de que la cuenta se bloquee.» | Aceptar |
| Mail inexistente | **(vacío)** | «Mail o contraseña incorrecta» | Aceptar |
| «Comprar» sin evento elegido | «Seleccionar evento» | «Debe seleccionar un evento para comprar» | Aceptar |
| «Comprar» con solo un servicio | **«RN-13»** | «El paquete debe tener al menos una entrada: los servicios no se venden solos.» | Aceptar |
| Cantidad 7 | **«RN-12»** | «Se pueden comprar como máximo 6 entradas por operación.» | Aceptar |
| Cantidad «dos» | **(vacío)** | «La cantidad debe ser un número entero de 0 en adelante.» | Aceptar |
| Comprar sin seguro | «Seguro de entrada» | «No agregaste el seguro… después de pagar no se puede incorporar. ¿Continuar sin seguro?» | **No** |
| «Volver al listado» con paquete | «Volver al listado» | «…se descarta el paquete que armó. ¿Desea volver?» | **No** |
| «Cancelar pago» | «Cancelar pago» | «Si cancela el pago se liberan las entradas reservadas. ¿Desea cancelar?» | **Sí** |

**Errores del pago.** Se muestran en una etiqueta dentro de la pantalla, no en un diálogo, y
explican el problema con precisión:
- «Ingrese el nombre del titular de la tarjeta.»
- «El número de tarjeta debe tener entre 13 y 19 dígitos.»
- «El vencimiento debe tener el formato MM/AA.»
- «La tarjeta está vencida.»
- «El código de seguridad debe tener 3 o 4 dígitos.»
- «Pago rechazado: … Se liberaron las entradas. Puede reintentar antes de que venza la reserva.»

**Después de cualquiera de estos errores se borra el código de seguridad**, y si la tarjeta es
rechazada también el número. La pantalla no avisa que hay que volver a escribirlos.

### 6.4 Pérdidas de trabajo y callejones sin salida

| Caso | Qué pasa (medido) |
|---|---|
| **Cerrar cualquier pantalla con la X** (listado, menú o armado) | La aplicación no termina: **queda en ejecución sin ninguna ventana visible**. Confirmado sobre `Visual.exe` con UI Automation: 0 ventanas visibles, proceso vivo con 56 MB. Solo se cierra desde el Administrador de tareas. Causa: todas las pantallas navegan con `Hide()`, la ventana principal (Iniciar sesión) queda oculta, y ninguna pantalla cierra la aplicación al cerrarse |
| Cerrar «Armar paquete» con la X con un paquete armado | **Se descarta sin preguntar.** «Volver al listado», en cambio, pide confirmación |
| Ventanas acumuladas | Después de **una** compra quedan **9 ventanas ocultas** en memoria. Cada «Volver» crea una pantalla nueva en lugar de mostrar la anterior |
| «Cancelar pago» | Pide confirmación, pero con **«Sí» por defecto**: un Enter suelta la reserva. El paquete se conserva en «Armar paquete» |
| Cerrar el pago con la X | Se cierra **sin preguntar** y libera la reserva. El paquete se conserva |
| Después de «Aceptar» en la confirmación | El usuario queda en **«Armar paquete» vacío**, no en el listado ni en el menú. Para salir hacen falta 2 clics más («Volver al listado», «Volver») |
| Tildar un servicio sin entradas | Se permite sin aviso. «Comprar», los servicios y el seguro están habilitados sin entradas, aunque la página 51 del documento dice que no deberían estarlo |

### 6.5 Accesibilidad medida con UI Automation

UI Automation expone los botones y los campos de texto como «Pane», y **los campos sin
nombre**. Un lector de pantalla diría «panel» en lugar de «campo Mail» o «botón Iniciar
sesión». El botón «Modificar», que no se ve, aparece primero en el árbol accesible.

### 6.6 Correcciones del estudio anterior: verificadas

| Hallazgo anterior | Corrección | Medido |
|---|---|---|
| H-01 descuento no explicado | Leyenda junto al total | «El descuento se aplica sobre el valor de las entradas.» al pie del resumen |
| H-02 reserva sin plazo visible | Temporizador | «Reserva: 09:59» en el encabezado del pago; pasa a rojo en el último minuto (CP-39) |
| H-03 cupo no informado | «Quedan N» | Columna «Disponibles» por sector |
| H-04 seguro inadvertido | Confirmación | Diálogo «Seguro de entrada», con «No» por defecto |
| H-05 nivel no comprendido | Enlace | «¿Cómo funcionan los niveles?» junto al nivel |
| H-06 «Volver» ambiguo | Rótulo y confirmación | «Volver al listado», que pide confirmación con «No» por defecto |

---

## 7. Evaluación heurística

Diez heurísticas de Nielsen (1994). Severidad en su escala: 0 no es un problema, 1 cosmético,
2 menor, 3 mayor, 4 catastrófico. Cada violación remite al hecho medido y al hallazgo.

| Heurística | Violación | Evidencia | Sev. | Hallazgo |
|---|---|---|---|---|
| 1. Visibilidad del estado del sistema | Después de comprar, el usuario queda en un armado vacío sin indicación de qué sigue | 6.4 | 2 | U-09 |
| 1. Visibilidad del estado del sistema | El filtro de fecha muestra la fecha de hoy aunque está desactivado (casilla sin tildar) | Medición S5 | 2 | U-08 |
| 2. Relación con el mundo real | Títulos con códigos internos de reglas («RN-12», «RN-13») | 6.3 | 2 | U-03 |
| 2. Relación con el mundo real | «Comprar» del listado no compra: abre «Armar paquete» | Medición del recorrido | 1 | U-03 |
| 2. Relación con el mundo real | «Show Password» en inglés; «ingrese usuario» para un campo llamado «Mail» | 6.2, 6.3 | 1 | U-11, U-03 |
| 3. Control y libertad del usuario | Cerrar con la X deja la aplicación invisible y en ejecución; en el armado, además, pierde el paquete sin preguntar | 6.4, UI Automation | **3** | U-01 |
| 3. Control y libertad del usuario | «Cancelar pago» con «Sí» por defecto | 6.3 | 2 | U-07 |
| 4. Consistencia y estándares | Enter paga pero no inicia sesión; «No» por defecto al volver y «Sí» al cancelar; algunos avisos con título y otros sin él | 6.2, 6.3 | 2 | U-02, U-07, U-03 |
| 4. Consistencia y estándares | Orden de tabulación de «Crear cuenta» con dos saltos hacia atrás; el listado empieza en «Volver» | 6.2 | 2 | U-10 |
| 5. Prevención de errores | «Comprar», servicios y seguro habilitados sin entradas | 6.4 | 2 | U-05 |
| 5. Prevención de errores | La cantidad se escribe a mano en una celda: se aceptan textos y números fuera de rango, que después se rechazan | 6.3 | 2 | U-04 |
| 6. Reconocer antes que recordar | Nada indica dónde se escribe la cantidad; la celda activa al abrir es «Sector» de VIP | Medición del recorrido | **3** | U-04 |
| 6. Reconocer antes que recordar | Filtros del listado sin rótulos («Todas», una fecha, un buscador con texto guía) | Medición S5 | 2 | U-08 |
| 7. Flexibilidad y eficiencia | El login no acepta Enter; el campo Mail no se alcanza con Tab | 6.2 | 2 | U-02 |
| 8. Diseño estético y minimalista | Botón «Modificar» residual en el login, fuera de la vista | 6.2 | 1 | U-13 |
| 9. Reconocer, diagnosticar y recuperarse de errores | El código de seguridad se borra después de un error sin avisarlo | 6.3 | 2 | U-06 |
| 9. Reconocer, diagnosticar y recuperarse de errores | Mensajes del login inconsistentes: uno informa los intentos restantes y el otro no; uno no tiene título | 6.3 | 1 | U-03 |
| 10. Ayuda y documentación | Ayuda contextual solo para los niveles; el resto depende del instructivo | Inventario | 1 | — |
| (Accesibilidad, complementaria) | Campos sin nombre accesible; botones expuestos como «Pane» | 6.5 | 2 | U-12 |

**Lo que cumple bien.**
- **Heurística 1:** el temporizador de la reserva está a la vista y el resumen se recalcula
  en el acto.
- **Heurística 9:** los mensajes del pago dicen exactamente qué corregir.
- **Heurística 5:** la confirmación de «Volver al listado» y la del seguro tienen «No» por
  defecto.
- **Heurística 2:** el resumen explica por qué el hospedaje no lleva descuento.

## 8. Recorrido cognitivo por perfil

Las cuatro preguntas de Wharton et al. (1994) en cada paso:
1. ¿Intentará lograr el efecto correcto?
2. ¿Notará que la acción correcta está disponible?
3. ¿Asociará la acción con el efecto?
4. ¿Entenderá el progreso?

Solo se listan los pasos con alguna respuesta «No»; en el resto, las cuatro respuestas son
«Sí» para ese perfil.

| Perfil | Paso | Pregunta con «No» | Hecho medido que la sostiene | Consecuencia en el modelo |
|---|---|---|---|---|
| P1 | 8 | 4: tras «Aceptar» queda en un armado vacío | 6.4 | Satisfacción: navegación −1 |
| P2 | 8 | 4: ídem | 6.4 | Ídem |
| P3 | 3 | **2: no hay indicación de que la cantidad se escribe en la celda** | Celda activa «Sector» de VIP; la columna Cantidad solo se distingue por el color | **Asistencia** y error E-B (escribe en otra celda o no confirma) |
| P3 | 4 | 1: tilda el hospedaje antes de tener entradas, y la interfaz lo permite | «Comprar» y servicios habilitados sin entradas | Error E-A: aviso «RN-13», que no entiende como título |
| P3 | 7 | 3: escribe el vencimiento sin barra a pesar del rótulo «MM/AA» | Validación de formato; código borrado después del error | Error E-C |
| P3 | 8 | 4: no sabe si terminó; cierra con la X | 6.4: la aplicación queda invisible | Satisfacción: navegación −1 |
| P4 | 0 | 2: espera que Enter inicie sesión | Sin AcceptButton en el login | Fricción F-1 (clic adicional) |
| P4 | 8 | 4: armado vacío | 6.4 | Satisfacción: navegación −1 |
| P5 | 3 | 3: escribe la cantidad en la fila de VIP, la primera, o no la confirma | Celda activa al abrir: VIP | Error E-B |
| P5 | 8 | 4: armado vacío | 6.4 | Satisfacción: navegación −1 |
| P6 | 0 | 2: con Tab no vuelve al Mail; Enter no inicia sesión | Mail sin tabulación; sin AcceptButton | Fricciones F-1 y F-2 |
| P6 | 3 | **2: con el teclado tiene que llegar con flechas a la celda Cantidad de Campo, sin indicación** | Foco en «Sector» de VIP; los lectores de pantalla no anuncian las celdas por nombre (6.5) | **Asistencia** |
| P6 | 5 | 3: Enter en el aviso del seguro elige «No» y cree haber avanzado | Botón por defecto «No» | Error E-D |
| P6 | 7 | 3: formato del vencimiento | Ídem P3 | Error E-C |
| P6 | 8 | 4: armado vacío; cierra con la X | 6.4 | Satisfacción: navegación −1 |

---

## 9. Resultados

### Resultados por perfil

| Perfil | Completó la tarea | Tiempo (desde el ingreso) | Errores | SUS |
|---|---|---|---|---|
| P1 | Sí, sin asistencia | 1:26 | 0 | 92,5 |
| P2 | Sí, sin asistencia | 1:42 | 0 | 87,5 |
| P3 | Sí, **con asistencia** (paso 3) | 4:16 | 3 | 35,0 |
| P4 | Sí, sin asistencia | 1:41 | 0 | 87,5 |
| P5 | Sí, sin asistencia | 2:13 | 1 | 77,5 |
| P6 | Sí, **con asistencia** (paso 3) | 4:30 | 2 | 40,0 |
| **Promedio** | **4 de 6 sin asistencia (66,7 %)** | **2:38** | **1,0** | **70,0** |

- **Tiempos:** sin los dos perfiles asistidos, el promedio es de 1:45.
- **Por qué fallan P3 y P6:** el paso que los bloquea es el mismo, y es un hecho medido: la
  interfaz no indica dónde se escribe la cantidad.
- **Los errores no son aleatorios:** los seis registrados salen de cuatro puntos de la
  interfaz, también medidos (sección 6). El detalle de cada error y su costo está en
  [modelo-resultados.md](modelo-resultados.md).

### Tiempo por paso frente a la línea base

| Nº | Paso | Estimado del documento | KLM, usuario experto | Promedio de los perfiles | Desvío contra el estimado |
|---|---|---|---|---|---|
| 1 | Buscar el evento | 45 s | 7 s | 9 s | −36 s |
| 2 | Seleccionar el evento | 15 s | 4 s | 6 s | −9 s |
| 3 | Elegir sector y cantidad | 40 s | 8 s | 17 s | −23 s |
| 4 | Agregar servicios | 60 s | 6 s | 10 s | −50 s |
| 5 | Decidir el seguro | 20 s | 10 s | 18 s | −2 s |
| 6 | Revisar el resumen | 30 s | 12 s | 17 s | −13 s |
| 7 | Completar el pago | 60 s | 21 s | 42 s | −18 s |
| 8 | Confirmar y recibir el comprobante | 10 s | 10 s | 13 s | +3 s |
| | **Total de los pasos 1 a 8** | **4:40** | **1:18** | **2:13** | **−2:27** |

**Interpretación.**
- **El estimado del documento queda holgado.** El recorrido real pide 18 acciones, y un
  usuario experto lo resuelve en 1:18. Los 4:40 suponían una búsqueda más larga en un
  catálogo mayor: con dos eventos vigentes, el paso 1 se resuelve mirando la lista.
- **Los pasos que más se alejan del experto son el 3 (cantidad) y el 7 (pago).** Ahí están
  los errores E-B y E-C. Coincide con el estudio anterior: el problema está en elegir la
  cantidad, no en entender el pago.
- **El paso 8 supera el estimado** porque la confirmación tiene unas 30 palabras para leer
  (desglose, tarjeta, token y carpeta).

### Satisfacción por criterio — escala de 1 a 5

Derivada: se parte de 5 y se resta 1 por cada fricción medida que el perfil encuentra en ese
criterio. Las reglas exactas están en `tools/Usabilidad/modelo.py`.

| Criterio | P1 | P2 | P3 | P4 | P5 | P6 | Prom. |
|---|---|---|---|---|---|---|---|
| Claridad de la navegación | 4 | 4 | 3 | 3 | 4 | 1 | 3,2 |
| Facilidad para armar el paquete | 5 | 5 | 2 | 5 | 4 | 4 | 4,2 |
| Comprensión del resumen y del descuento | 5 | 4 | 4 | 5 | 4 | 4 | 4,3 |
| Claridad de los mensajes de error | 5 | 5 | 3 | 5 | 5 | 3 | 4,3 |
| Confianza en el proceso de pago | 5 | 5 | 4 | 5 | 5 | 4 | 4,7 |
| Satisfacción general | 5 | 5 | 2 | 5 | 4 | 2 | 3,8 |
| **Promedio global** | | | | | | | **4,08** |

**Hallazgo del cuestionario.**
- **El criterio más bajo es la claridad de la navegación (3,2).** En la versión anterior era
  la comprensión del resumen (3,2), que ahora sube a 4,3: la leyenda de H-01 funcionó.
- **Lo que baja la navegación son tres hechos medidos:** el usuario queda en un armado vacío
  después de comprar, la X deja la aplicación invisible, y el teclado no recorre bien el login.

### System Usability Scale

**Cómo se derivó cada respuesta (1 a 5), sin inventar ninguna:**

| Ítem | De dónde sale |
|---|---|
| 1. Lo usaría con frecuencia | Satisfacción general, con tope 4: comprar entradas es ocasional |
| 2. Innecesariamente complejo | 6 − facilidad para armar el paquete |
| 3. Fácil de usar | Satisfacción general |
| 4. Necesitaría ayuda técnica | 4 si necesitó asistencia; 2 si cometió errores; 1 si no |
| 5. Funciones bien integradas | Facilidad para armar el paquete |
| 6. Inconsistencias | 3 si encontró Enter que no responde, el «No» por defecto del seguro o el título «RN-13»; si no, 2 |
| 7. Se aprende rápido | 5 con experiencia alta, 4 media, 2 nula |
| 8. Engorroso | 6 − claridad de la navegación |
| 9. Me sentí seguro | Confianza en el proceso de pago |
| 10. Tuve que aprender mucho antes | 1 con experiencia alta, 2 media, 4 nula |

| Puntaje SUS | Interpretación (Bangor et al., 2009) | Perfiles |
|---|---|---|
| 90 o más | A — Excelente | P1 (92,5) |
| 80 a 89 | B — Bueno | P2 (87,5) y P4 (87,5) |
| 70 a 79 | C — Aceptable | P5 (77,5) |
| 60 a 69 | D — Deficiente | — |
| Menos de 60 | F — Inaceptable | P3 (35,0) y P6 (40,0) |

**Lectura.**
- **El promedio de 70,0 supera la referencia de 68, pero con una distribución partida:**
  cuatro perfiles con experiencia, entre 77,5 y 92,5, y dos perfiles, sin experiencia o sin
  mouse, por debajo de 40.
- **Es el mismo patrón que el documento señalaba en P3, ahora con causa medida.** La
  aplicación funciona para quien ya sabe comprar en línea, y falla donde la interfaz no
  muestra la acción: la cantidad en la grilla y el teclado.

### Cumplimiento de los criterios comprometidos

| Criterio | Origen | Umbral | Resultado | Veredicto |
|---|---|---|---|---|
| Cantidad de perfiles | Obj. esp. 2 | 3 o más | 6 | Cumple |
| Completaron el flujo principal | Crit. acept. 7 | 3 o más | 6 (4 sin asistencia) | Cumple |
| Tasa de éxito sin asistencia | Diseño de la prueba | 3 de 5 (60 %) | 4 de 6 (66,7 %) | Cumple |
| Tiempo del flujo completo | RNF-01 | Menos de 5 min | 2:38 promedio; máximo 4:30 | Cumple |
| Satisfacción buena o muy buena | Obj. esp. 4 | 70 % o más | 4 de 6 (66,7 %) | **No alcanza** |
| Promedio en escala de 1 a 5 | Obj. esp. 4 | Mayor que 4 | 4,08 | Cumple |
| Puntaje SUS | Referencia externa | 68 o más | 70,0 | Cumple, con poco margen |
| Errores por perfil | Diseño de la prueba | Hasta 2 | 1,0 promedio; P3 con 3 | Cumple con reservas |

**Lectura del cuadro.**
- **El objetivo específico 4 se considera alcanzado**, porque admite cumplir el porcentaje de
  satisfacción o el promedio, y el promedio (4,08) cumple.
- **El porcentaje (66,7 %) queda a un perfil del umbral.** Los dos perfiles que lo bajan
  apuntan a los mismos dos hallazgos, U-04 y U-01.
- **Corregir esos dos hallazgos subiría la tasa de éxito y la satisfacción a la vez.**

---

## 10. Hallazgos y plan de mejora

### Hallazgos

Es hallazgo todo problema con evidencia medida en la sección 6. La severidad usa la escala de
Nielsen. Los códigos U-xx no se superponen con H-01 a H-06 del estudio anterior, que están
resueltos.

| Nº | Hallazgo | Sev. | Perfiles afectados | Evidencia |
|---|---|---|---|---|
| U-01 | Cerrar una pantalla con la X deja la aplicación en ejecución y sin ventanas; en «Armar paquete», además, descarta el paquete sin preguntar | 3 | Todos; P3 y P6 en el modelo | 6.4; UI Automation sobre el ejecutable |
| U-04 | Nada indica dónde se escribe la cantidad: celda de grilla sin rótulo, con el foco inicial en otra fila y otra columna | 3 | P3, P5, P6 | 6.1, 6.2 |
| U-02 | El login no acepta Enter y el campo Mail está fuera del orden de tabulación | 2 | P4, P6 | 6.2 |
| U-03 | Títulos de aviso técnicos («RN-12», «RN-13»), vacíos o con errores («Campo vacíos»); «ingrese usuario» para el campo Mail; «Comprar» del listado abre el armado | 2 | P3 | 6.3 |
| U-05 | «Comprar», los servicios y el seguro están habilitados sin entradas, contra lo que especifica la página 51 | 2 | P3 | 6.4 |
| U-06 | El código de seguridad (y el número, tras un rechazo) se borra después de un error sin avisarlo | 2 | P3, P6 | 6.3 |
| U-07 | «Cancelar pago» tiene «Sí» por defecto; las demás confirmaciones, «No» | 2 | Teclado | 6.3 |
| U-08 | Los filtros del listado no tienen rótulos; la fecha se muestra aunque el filtro esté desactivado | 2 | P3 | Medición S5 |
| U-09 | Después de confirmar la compra el usuario queda en «Armar paquete» vacío | 2 | Todos | 6.4 |
| U-10 | Orden de tabulación de «Crear cuenta» desordenado (empieza en Contraseña); el listado empieza en «Volver» | 2 | P6 | 6.2 |
| U-12 | Campos sin nombre accesible y botones expuestos como «Pane» | 2 | Lectores de pantalla | 6.5 |
| U-11 | «Show Password» en inglés, en el login y en el alta | 1 | — | 6.2 |
| U-13 | Botón «Modificar» residual en el login, fuera de la vista pero accesible | 1 | Lectores de pantalla | 6.2, 6.5 |

### Plan de mejora priorizado

Esfuerzo en puntos del documento: 1 punto equivale a 4 horas. Todo cae en `Visual/`, la zona
de Codex. Ninguna acción agrega un requerimiento: todas corrigen cómo la interfaz comunica o
protege algo que el sistema ya hace.

| Prioridad | Acción correctiva | Hallazgo | Esfuerzo |
|---|---|---|---|
| 1 | Al cerrarse cualquier pantalla con la X, terminar la aplicación si no queda otra ventana visible; en «Armar paquete», pedir la misma confirmación que «Volver al listado» | U-01 | 1 pt |
| 2 | Al abrir la compra, poner el foco en la celda Cantidad del primer sector con cupo y agregar un rótulo «Escribí cuántas entradas querés en la columna Cantidad» | U-04 | 1 pt |
| 3 | Deshabilitar «Comprar», los servicios y el seguro hasta que haya al menos una entrada, como dice la página 51 | U-05 | 0,5 pt |
| 4 | Login: «Iniciar sesión» como botón de Enter; Mail con tabulación y primero en el orden | U-02 | 0,25 pt |
| 5 | «Cancelar pago» con «No» por defecto | U-07 | 0,25 pt |
| 6 | Títulos de los avisos en lenguaje del usuario (por ejemplo «Cantidad de entradas» en lugar de «RN-12»); «Campos vacíos»; «Ingrese su mail» | U-03 | 0,5 pt |
| 7 | Aviso «Volvé a ingresar el código de seguridad» cuando se borra | U-06 | 0,25 pt |
| 8 | Después de «Aceptar», volver al listado de eventos | U-09 | 0,25 pt |
| 9 | Rótulos «Categoría» y «Desde» en los filtros del listado | U-08 | 0,5 pt |
| 10 | Orden de tabulación de «Crear cuenta» y del listado | U-10 | 0,25 pt |
| 11 | Nombres accesibles (`AccessibleName`) en campos y botones del flujo de compra | U-12 | 0,5 pt |
| 12 | «Mostrar contraseña»; quitar el botón «Modificar» del login | U-11, U-13 | 0,25 pt |
| | **Total** | | **5,5 pt (22 h)** |

**Las prioridades 1 a 3 son las que mueven los criterios comprometidos.** Si sacan a P3 y a
P6 de la asistencia, la tasa de éxito sube a 6 de 6. La satisfacción de esos dos perfiles
mejora, pero el modelo no asegura que alcance el umbral del 70 %: habría que volver a medir
con las correcciones hechas. Suman 2,5 puntos (10 horas).

## 11. Limitaciones del estudio

1. **Carácter simulado.** No hubo personas. Lo medido sobre la aplicación es real; los
   tiempos, errores y valoraciones de cada perfil son derivaciones de un modelo con reglas
   explícitas, y deben leerse como predicciones.
2. **El KLM modela a un usuario que sabe qué hacer.** Los perfiles sin experiencia suman
   preparación mental y errores, pero la exploración libre (probar, volver atrás, dudar) está
   subestimada. Los tiempos de P3 y P6 son un piso, no un promedio.
3. **Catálogo chico.** Con dos eventos vigentes, buscar no requiere usar los filtros. Con un
   catálogo real, el paso 1 y el hallazgo U-08 pesarían más.
4. **Escala de pantalla.** Se midió a 100 %. La aplicación no declara compatibilidad con
   DPI, que es una limitación conocida de RNF-03, y a 125 % o 150 % Windows la muestra
   estirada. Esa verificación es aparte (JC-R01).
5. **Accesibilidad.** Se midió el árbol de UI Automation, pero no se probó con un lector de
   pantalla real.

## 12. Conclusión

- **El flujo cumple los criterios formales en tiempo, éxito, promedio de satisfacción y
  SUS.** Las seis correcciones del estudio anterior están en la interfaz y funcionan.
- **El problema dejó de ser la comprensión de las reglas y pasó a ser el control de la
  navegación.** La aplicación no cierra cuando el usuario cierra la ventana, y la acción
  central del armado (escribir la cantidad) no se anuncia.
- **Los dos perfiles que fallan son los que RNF-01 nombra:** quien no tiene experiencia
  técnica, y quien no puede usar el mouse.
- **Las tres primeras acciones del plan, 10 horas en total, apuntan a esos dos perfiles.**

## 13. Fuera del alcance de este informe

**Seguridad, para JC:** la pantalla «Crear cuenta» ofrece el tipo de usuario «Propietario»
además de «Cliente». Cualquier persona puede registrarse sola como administrador y entrar a la
gestión de eventos y a los reportes, cuyo único control (RN-34) es justamente ser Propietario.
No es un problema de usabilidad y no entra en el plan de mejora; requiere una decisión.

## Reproducción

```
powershell -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar          # base de pruebas con la semilla
powershell -ExecutionPolicy Bypass -File tools\Usabilidad\Medir.ps1                # docs/usabilidad/medicion.txt
powershell -ExecutionPolicy Bypass -File tools\Usabilidad\VerificarCierreConX.ps1  # U-01 sobre el ejecutable
python tools\Usabilidad\modelo.py docs\usabilidad\modelo-resultados.md             # tiempos, satisfacción y SUS
```

## Referencias

- Bangor, A., Kortum, P. y Miller, J. (2009). Determining what individual SUS scores mean:
  Adding an adjective rating scale. *Journal of Usability Studies*, 4(3), 114-123.
- Brooke, J. (1996). SUS: A «quick and dirty» usability scale. En *Usability Evaluation in
  Industry*. Taylor & Francis.
- Card, S. K., Moran, T. P. y Newell, A. (1980). The keystroke-level model for user
  performance time with interactive systems. *Communications of the ACM*, 23(7), 396-410.
- Kieras, D. (2001). *Using the Keystroke-Level Model to estimate execution times*.
  University of Michigan.
- Nielsen, J. (1994). Heuristic evaluation. En J. Nielsen y R. L. Mack (eds.), *Usability
  Inspection Methods*. John Wiley & Sons.
- Nielsen, J. y Landauer, T. K. (1993). A mathematical model of the finding of usability
  problems. *Proceedings of INTERCHI'93*, 206-213.
- Wharton, C., Rieman, J., Lewis, C. y Polson, P. (1994). The cognitive walkthrough method: A
  practitioner's guide. En J. Nielsen y R. L. Mack (eds.), *Usability Inspection Methods*.
  John Wiley & Sons.
