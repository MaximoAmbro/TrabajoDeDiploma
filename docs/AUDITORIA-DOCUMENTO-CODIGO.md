# Auditoría del documento contra el código — para Máximo

Claude Code, 12/11/2026; decisiones de JC incorporadas el 13/11 y el 14/11 (sección 4). El 14/11
se agregaron las especificaciones CU-09 a CU-13 (Anexo 3) y la estimación de costos (Anexo 4). Completo: reglas, requerimientos, entidades, arquitectura, pantallas,
casos de uso, cifras página por página, decisiones y lo que está mal en el código.

**Para buscar en el Word:** el texto «Dice» está copiado del PDF. Si una frase no aparece
entera, buscá sus primeras cinco o seis palabras: el PDF parte los renglones.

Fuente: `Documentacion/EventAura_v2.pdf` (v2.4). Regla: **el código y la base mandan**; lo que
está mal en el código va aparte, en la sección 8.

**Clases:** **R** = reemplazo de texto (buscar y pegar) · **N** = número a regenerar · **D** = decisión.

## 0. Cómo se combina con `CORRECCIONES-DOCUMENTO.md`

Esa lista sigue vigente, **salvo sus cifras**: dice 38 reglas, 86,8 %, 61 casos y 73
ejecuciones. Donde este archivo da un texto para el mismo lugar, **vale este**.

| Ítem de CORRECCIONES | Qué hacer |
|---|---|
| 1, 4b, 5, 7, 8, 9, 10, 11, 11b, 12 a 16, 22, 24 | Aplicar tal cual |
| 6 (RN-29) | Usar el texto de A-05 |
| 25 (RN-40 a RN-42) | **Ya se cumple la condición**: SEG-1 a SEG-3 están hechos (CP-91 a CP-104). Aplicar las tres reglas tal cual. RN-18: usar A-03 |
| 2, 3, 4, 17 a 21, 23 | Aplicar con las cifras de la sección 5 de este archivo |

## 1. Cifras de hoy (base de todo lo demás)

| Dato | Valor | De dónde sale |
|---|---|---|
| Reglas vigentes | **41** (RN-01 a RN-42, sin RN-27) | `docs/02-reglas-negocio.md` + RN-40 a RN-42 |
| Reglas con caso de prueba | **37 de 41 = 90,2 %** | Sin caso: RN-07, RN-26, RN-28, RN-33 (ver D-5) |
| Casos de prueba | **112** (CP-01 a CP-112), en 109 métodos | Comentarios `// CP-xx` de `Pruebas/*.cs`, contados uno por uno |
| Ejecuciones | **184: 180 correctas, 4 fallidas** | Última batería de Codex (`RESULTADOS-TAREAS-CODEX-2026-11-12-T47-T48.md`). La suma por método da 184 exactos |
| Casos que fallan | CP-69, CP-76, CP-85 (aspecto a 150 %) y CP-81 (comportamiento) | Mismo reporte |
| Pantallas | **20**, no 19: se sumó `frmControlIngreso` (SEG-2) | `Visual/` |

**N:** el resultado de la batería (184 / 180 / 4) se vuelve a tomar de la última corrida de
`tools/Proyecto.ps1 -Accion Probar` antes de pegar.

## 2. Reglas de negocio (páginas 25 a 27, 32, 33 y 55)

**A-01 · p. 25 · R.** Dice: «los once requerimientos funcionales (RF-01 a RF-11)». Poner: «los
trece requerimientos funcionales (RF-01 a RF-13)». *Razón:* el propio documento agrega RF-12 y
RF-13 en la p. 23, y las reglas RN-06, RN-07 y RN-11 los citan como origen.

**A-02 · p. 26 · RN-11 · R.** Dice: «Un evento con entradas vendidas no puede eliminarse: sólo
puede suspenderse o reprogramarse, conservando la trazabilidad de lo vendido.» Poner: «Un evento
con entradas vendidas no puede eliminarse: sólo puede suspenderse, conservando la trazabilidad
de lo vendido.» *Razón:* código. `GestorEventos` tiene `Eliminar` y `Suspender`, y no existe
ninguna reprogramación.
> **Resuelta el 14/11: RN-11 se conserva con este texto.** JC había pedido retirarla sobre un
> resumen impreciso; la regla está construida y probada (sección 4). La reprogramación queda en el
> alcance como no implementada (B-10).

**A-03 · p. 26 · RN-18 · R** (reemplaza el texto del ítem 25 de CORRECCIONES). Poner: «Cada
entrada vendida genera un código QR único e irrepetible, válido para un único ingreso. El
control de ingreso acepta la primera lectura y rechaza las siguientes. Rechaza también la
entrada que no está confirmada, la que corresponde a otro evento y la que se presenta fuera del
horario de ingreso, que va desde seis horas antes hasta doce horas después del inicio del
evento.» *Razón:* código. `GestorIngresos.Registrar` usa `INGRESO_HORAS_ANTES` = 6 e
`INGRESO_HORAS_DESPUES` = 12.

**A-04 · p. 26 · RN-23 · R.** Dice: «El nivel se reevalúa el primer día de cada mes.» Poner: «El
nivel se reevalúa una vez por mes calendario, la primera vez que el cliente arma o confirma una
compra en ese mes.» El resto de la regla queda igual. *Razón:* código. No hay ningún proceso del
día 1: `ReevaluarNivel` se ejecuta al abrir la compra (`FrmCompraCliente.cs:173`) y al
confirmarla (`GestorCompras.Confirmar`). Para el cliente, el efecto es el mismo.
- **Mismo cambio en p. 55,** fila «Bajé de nivel…». Dice: «El nivel se reevalúa el primer día de
  cada mes. Una compra que sale de la ventana de doce meses recién impacta en esa fecha.» Poner:
  «El nivel se reevalúa una vez por mes, la primera vez que el cliente arma o confirma una compra
  en ese mes. Una compra que sale de la ventana de doce meses recién impacta en esa
  reevaluación.»

**A-05 · p. 26 · RN-24 · R.** Dice: «Sólo computan para el nivel las compras confirmadas y no
reembolsadas.» Poner: «Sólo computan para el nivel las compras confirmadas. Una compra cuyas
entradas se devolvieron todas deja de computar; una con devolución parcial sigue computando,
porque la operación existió.» *Razón:* código. En `GestorCompras.Reembolsar` la devolución
parcial deja la compra en estado Confirmada.
- **p. 55,** fila «Me devolvieron una compra…». Dice: «El contador desciende de inmediato, pero
  el nivel se conserva hasta la reevaluación mensual.» Poner: «Si se devolvieron todas las
  entradas de la compra, el contador desciende de inmediato; si se devolvió una parte, la compra
  sigue contando. En los dos casos el nivel se conserva hasta la reevaluación mensual.»

**A-06 · p. 27 · RN-26 · R.** Dice: «La suscripción no se renueva de forma automática: vencido el
período, el cliente debe contratarla nuevamente.» Poner lo mismo, más: «El cliente puede
cancelarla antes del vencimiento: pierde los beneficios desde ese momento y no se le reintegra
el precio.» *Razón:* código. `GestorSuscripciones.Cancelar` existe, con el aviso «Los beneficios
se pierden desde ahora», y no hay ningún reintegro.

**A-07 · p. 27 · RN-29 · R** (reemplaza el ítem 6 de CORRECCIONES). Mismo texto del ítem 6,
cambiando «cada una debe estar confirmada y asegurada» por «cada una debe estar confirmada,
asegurada y sin ingreso registrado». *Razón:* código. `Reembolsar` rechaza una entrada que tiene
`FechaIngreso` («No podés devolver una entrada que ya se usó para ingresar»), y CP-111 lo
prueba.

**A-08 · p. 27 · RN-31 · R.** Dice: «Toda operación de pago se registra con su estado resultante
(aprobado, rechazado o pendiente), sea cual fuere el desenlace.» Poner: «Toda operación de pago
se registra con su estado resultante, aprobado o rechazado, sea cual fuere el desenlace.»
*Razón:* base. `CK_Pagos_Estado` solo admite 'Aprobado' y 'Rechazado'. «Pendiente» es un estado
de la compra, no del pago. **Verificado el 13/11** contra `GestorCompras.Confirmar`: no hay
ningún pago intermedio (sección 4). Se aplica.

**A-09 · p. 32 · tabla de niveles, columna «Beneficios adicionales» · R.**

| Fila | Dice | Poner |
|---|---|---|
| Bronce | «Acceso estándar a la preventa general» | «—» |
| Plata | «Preventa anticipada de 24 horas» | «—» |
| Oro | «Preventa anticipada de 48 h y prioridad en lista de espera» | «—» |

En el mismo párrafo, cambiar «Los beneficios diferenciales del nivel Oro se orientan al acceso
anticipado —de costo marginal nulo para la plataforma— antes que a una rebaja mayor de precio.»
por «El nivel no otorga otros beneficios que el descuento: la preventa y la prioridad en la
lista de espera son beneficios de la suscripción (RN-37, RN-39 y RN-35).»

*Razón:* código. La preventa solo mira la suscripción (`GestorPaquetes.VerificarPreventa`), y la
lista solo ordena por `ConSuscripcion`. Además, la tabla contradecía la RN-37 del propio
documento.

**A-10 · p. 33 · Parámetros configurables · R.**
- **Borrar la fila** «Periodicidad de reevaluación de nivel | RN-23 | Mensual». No es un
  parámetro: no existe en la tabla `Parametros`.
- **Agregar** estas filas:

| Parámetro | Regla | Valor inicial |
|---|---|---|
| Cargo por servicio | — | 15 % |
| Precio del seguro por entrada | RN-28 | $5.000 |
| Bonificación del seguro para suscriptores | RN-37 | 100 % |
| Precio del plan mensual | RF-13 | $9.000 |
| Precio del plan anual | RF-13 | $90.000 |
| Horas de ingreso antes del evento | RN-18 | 6 |
| Horas de ingreso después del evento | RN-18 | 12 |

*Razón:* base. Son los valores de `BaseDatos/02-semilla.sql`.

**A-11 · p. 33 · Conflictos · R** (solo si se aplica el ítem 6 de CORRECCIONES, RN-35). Dice:
«tres pares de reglas». Poner: «cuatro pares de reglas». Y agregar la fila:

| RN-35 vs. RN-37 | RN-37 promete prioridad al suscriptor en la lista de espera. | Primero los suscriptores y después el resto, cada grupo por orden de inscripción. La condición de suscriptor se congela al inscribirse. |
|---|---|---|

## 3. Requerimientos y alcance

**B-01 · p. 9 · R.** Dice: «aplicando tecnologías como Windows Forms (interfaz) y ASP.NET Core
(backend).» Poner: «aplicando Windows Forms para la interfaz, capas de negocio y de datos en C#
sobre .NET Framework 4.8 y SQL Server como base de datos.» *Razón:* no hay ASP.NET en la
solución.

**B-02 · p. 23 · RF-11 · R.** Dice: «El sistema debe mostrar tablas o gráficos con datos ficticios
exportables.» Poner: «El sistema debe mostrar tablas con datos ficticios, exportables a PDF.»
*Razón:* `frmReportes` tiene grilla y «Exportar PDF», sin gráficos.

**B-03 · p. 23 · RF-12 · R.** Dice: «Permitir al Administrador crear, modificar y desactivar
eventos, definiendo sus sectores, capacidades y precios.» Poner: «Permitir al Administrador
crear, suspender y eliminar eventos, agregar y quitar sus sectores con capacidad y precio, y
fijar su período de preventa.» *Razón:* `GestorEventos` (completa T-7).

**B-04 · p. 35 · R+N.** Dice: «Los once requerimientos funcionales y los cinco no funcionales».
Poner: «Los trece requerimientos funcionales y los cinco no funcionales».
- Dice: «nueve de los once requerimientos funcionales quedaran en esa categoría, concentrando
  alrededor del 85 % del esfuerzo». Poner: «diez de los trece requerimientos funcionales
  quedaran en esa categoría, concentrando alrededor del 80 % del esfuerzo estimado».
- *Cuenta:* 49 + 5 + 8 = 62 de 78 puntos, que da 79,5 %.

**B-05 · p. 37 · W-06 · R.** En la columna Motivo, dice: «Requiere hardware específico. El código
QR se genera pero no se valida.» Poner: «DADA DE BAJA en la versión 2.5: se implementó como
control de ingreso (RN-18), con lector USB o con el texto del ticket. El número no se reasigna.»
*Razón:* `frmControlIngreso` y `GestorIngresos` existen.

**B-06 · p. 38 · Fase 8 · N.** En la columna Pts, dice «11». Poner «3». Va junto con el ítem 7 de
CORRECCIONES: excluido RF-09, en la fase queda solo RF-10.

**B-07 · p. 20, criterio 9 · R.** Reemplazar la línea completa por: «9. Recomendaciones
personalizadas: excluido del alcance (RF-09 pasó a Won't have en la versión 2.5).»

**B-08 · p. 18, criterio 1 · R.** Dice: «aplicación de descuentos por suscripción o rankeo».
Poner: «aplicación del descuento por nivel de fidelidad y de los beneficios de la suscripción».
*Razón:* RN-37.

**B-09 · p. 40, «Descripción funcional» · R.** Hoy afirma cosas que el sistema no tiene:
- sitio web;
- e-ticket enviado por correo;
- reclamo de seguro por formulario;
- nivel según el monto comprado;
- motor de recomendaciones;
- aviso automático por correo;
- tickets de soporte;
- descuento por suscripción.

**Reemplazar los tres párrafos completos**, desde «El usuario ingresa a la página principal de
EventAura» hasta «nivel alcanzado y beneficios vigentes.», **por:**

> El cliente inicia sesión en la aplicación de escritorio de EventAura y accede al listado de
> eventos vigentes. Puede filtrarlo por texto (nombre del evento, del local o su dirección), por
> categoría (concierto, deportivo, teatro o cultural) y por fecha. Al elegir un evento se abre la
> pantalla de armado del paquete. Allí indica la cantidad de entradas de cada sector, con su
> precio y su cupo restante, y marca los servicios complementarios que ese evento ofrece:
> hospedaje, traslado, merchandising y experiencia VIP. También puede contratar el seguro de
> entrada. El resumen se recalcula con cada cambio y muestra la base de entradas, el descuento
> por nivel, los servicios, el cargo por servicio, el seguro y el total.
>
> Al confirmar el paquete, el sistema reserva el cupo por diez minutos y abre el pago simulado. El
> cliente ingresa los datos de una tarjeta ficticia, que se tokeniza, y la pasarela simulada
> aprueba o rechaza la operación; del pago se conservan solo los cuatro últimos dígitos y el
> token. Si el pago se aprueba, la compra queda confirmada y el sistema genera un ticket en PDF
> por entrada. Cada ticket lleva un código QR único, que el control de ingreso acepta una sola
> vez. Con el seguro contratado, el cliente puede devolver entradas desde su historial hasta
> cuarenta y ocho horas antes del evento.
>
> El nivel de fidelidad (Bronce, Plata u Oro) se determina por la cantidad de compras confirmadas
> en los últimos doce meses, y fija el descuento sobre las entradas. La suscripción, mensual o
> anual, no otorga descuento: bonifica el seguro, habilita la preventa y da prioridad en la lista
> de espera. Si un evento está agotado, el cliente puede inscribirse en su lista de espera. El
> Administrador registra locales, crea eventos con sus sectores y su preventa, consulta las
> listas de espera, controla el ingreso y genera cinco reportes de gestión exportables a PDF.

*Razón:* código. Cada afirmación sale de un gestor: `GestorEventos.Buscar`, `GestorPaquetes`,
`GestorFidelizacion.CalcularDescuento`, `GestorCompras.Iniciar`, `Confirmar` y `Reembolsar`,
`GestorIngresos`, `GestorSuscripciones`, `GestorListaEspera` y `GestorReportes`.

> Si se elige la opción (b) de D-6, la oración de la lista de espera agrega: «; el sistema no
> envía avisos (W-03)».

**B-10 · p. 41, «Alcance», los dos párrafos · R** (unifica B-10 y B-11; redactado el 13/11 con la
guía del docente, punto 5). La guía pide el alcance «sin entrar en temas técnicos ni
tecnológicos» y con inclusiones y exclusiones explícitas. Reemplazar los dos párrafos completos,
desde «EventAura en su primera versión abarca» hasta «el foco está en el usuario final y el
administrador de eventos.», **por:**

> EventAura, en su primera versión, abarca la gestión de eventos y la venta de entradas de una
> ticketera local.
>
> **Inclusiones:**
> - el registro de clientes y el inicio de sesión;
> - el alta de locales y de eventos, con sus sectores, precios y período de preventa;
> - la búsqueda de eventos y el armado de paquetes personalizados: entradas y servicios
>   complementarios (hospedaje, traslado, merchandising y experiencia VIP);
> - la compra con pago simulado y la emisión de una entrada por persona, con un código único;
> - el control de ingreso al evento, que acepta cada entrada una sola vez;
> - el programa de fidelización por niveles, con su descuento sobre las entradas;
> - la suscripción mensual o anual, con sus beneficios;
> - el seguro de entrada y la devolución de las entradas aseguradas;
> - la lista de espera para los eventos agotados;
> - cinco reportes de gestión para la administración.
>
> **Exclusiones** (quedan planteadas para iteraciones futuras):
> - las recomendaciones personalizadas de eventos (RF-09);
> - los avisos por correo electrónico, incluido el aviso de la lista de espera (W-03);
> - la reprogramación de eventos: un evento con entradas vendidas se suspende, pero no cambia de
>   fecha;
> - el cobro real a través de una pasarela de pago, y el cobro de la suscripción;
> - la aplicación móvil nativa y el acceso desde un navegador web;
> - la logística interna de cada evento (contratación de artistas, catering o disposición del
>   recinto);
> - las tareas contables de la empresa (facturación, balances y pagos a proveedores);
> - la integración con sistemas ERP o CRM externos;
> - la gestión de empleados y de recursos internos, como el inventario de merchandising.

*Razón:* código. Cada inclusión sale de un gestor (ver B-09); cada exclusión, de lo que no
existe. Las cuatro últimas son las del párrafo original, que siguen siendo ciertas.

> Si se aplica la opción (b) de D-6, la exclusión de los avisos dice además: «la inscripción en
> la lista de espera se confirma en pantalla, pero el sistema no avisa cuando se libera un
> lugar».

**B-11 · p. 41.** Queda dentro de B-10.

**B-12 · p. 41, Restricción 2 · R.**
- **Dice:** «Alcance centrado en prototipado, no en desarrollo funcional. El producto final es un
  prototipo navegable, no un sistema programado con backend funcional. Esto restringe la
  validación de ciertos aspectos técnicos como seguridad real, velocidad de respuesta o
  integraciones en tiempo real.»
- **Poner:** «Alcance de prototipo funcional. El producto final es un prototipo funcional de
  escritorio, con base de datos propia, y no un sistema en producción. Esto restringe la
  validación de aspectos como el rendimiento con carga real, la seguridad de un despliegue en red
  y las integraciones en tiempo real.»

**B-13 · p. 41, Restricción 3 · R.**
- **Dice:** «(Figma, Draw.io, Google Drive), lo que puede restringir la calidad gráfica o la
  capacidad de simulación avanzada.»
- **Poner:** «(Visual Studio, SQL Server Express y PlantUML), lo que puede restringir la capacidad
  de simulación avanzada.»

**B-14 · p. 21, Supuesto 2 · R.**
- **Dice:** «El sistema será evaluado en base a un prototipo navegable y documentación técnica, no
  a una implementación funcional con backend real.»
- **Poner:** «El sistema se evalúa sobre un prototipo funcional, una aplicación de escritorio con
  base de datos propia, y su documentación técnica. Las integraciones externas (pasarela de pago y
  proveedores de servicios) son simuladas.»

**B-15 · p. 21, Supuesto 7 · R.**
- **Dice:** «(Figma, Draw.io, herramientas UAI)».
- **Poner:** «(Visual Studio, SQL Server Express, PlantUML y herramientas UAI)».

**B-16 · p. 10, Objetivo específico 2 · R.**
- **Dice:** «Construir un prototipo (en Figma, Adobe XD, o similar) que permita».
- **Poner:** «Construir un prototipo funcional de escritorio que permita».
- *Por qué sin «Windows Forms»:* el mismo documento dice que los objetivos no citan tecnologías
  puntuales (p. 9), y la guía del docente lo pide.

**B-17 · p. 16, Fase 6 · R.**
- **Dice:** «Simulación del formulario de pago con tokenización (sin backend).»
- **Poner:** «Simulación del formulario de pago con tokenización (sin pasarela real).»

**B-18 · p. 18, criterio 3 «Compatibilidad técnica» · R.** Reemplazar los tres puntos **por:**

> ● El prototipo es una aplicación de escritorio para Windows 10 u 11 y no requiere otro
>   software que el motor de base de datos.
> ● Las pantallas entran completas en el área útil de una notebook y de un monitor de
>   escritorio: ninguna supera los 790 píxeles de alto.

*Razón:* es el mismo cambio que el ítem 4b de CORRECCIONES aplica a RNF-03; el criterio repetía
lo de los navegadores.

## 4. Decisiones

**Estado al 14/11.** JC resolvió con la regla del docente: el código y la base mandan.

| Tema | Resolución | Qué se aplica |
|---|---|---|
| RN-23, día 1 contra primera compra del mes | Se corrige el texto | A-04, tal cual |
| p. 32, preventa por nivel contra por suscripción | Se corrige el texto | A-09, tal cual |
| W-06, validación del QR en puerta | Se corrige: está construida | B-05, tal cual |
| pp. 9 y 40 a 42, sitio web, ASP.NET, correo y recomendaciones | Se reemplaza con el texto real | B-01 y B-09 a B-13. B-10 se rehízo el 13/11 en el formato de la guía del docente (inclusiones y exclusiones) |
| pp. 57 a 61, modelo que no existe (D-1) | **Aprobado (a):** se reemplaza por la sección de arquitectura | Anexo 1 |
| RN-31, estado «pendiente» del pago | Condicional; ver la verificación abajo. **Resultado: se corrige el texto** | A-08, tal cual |
| RN-11 | **Se conserva sin «reprogramarse»** (14/11). No se retira nada | A-02 y B-10, tal cual |
| D-4, casos de uso sin especificación | **Se escriben** (14/11) | Anexo 3 |
| D-2 | **(a) para las pp. 63 a 67, (b) para las 68 a 70** (JC, 14/11) | Anexo 2 y la nota de las pruebas |
| D-3 | **(a), la remisión** (JC, 14/11) | Texto de (a) en D-3 |
| D-5 | **(a), 90,2 %**; Codex escribe dos pruebas reales para RN-26 y RN-28. El texto del 95,1 % se escribe solo cuando Codex las entregue verdes (JC, 14/11) | Sección 5 |
| D-6 | **(c)**, el aviso dentro de la aplicación (JC, 14/11). Codex evalúa una ventana para suscriptores; los dos textos de RN-35 están en `LISTA-PARA-MAXIMO.md`, p. 27 | Textos de (c) en D-6 |
| Estimación de costos (I-4 del chequeo) | **Se hace** (14/11), con el valor hora como supuesto | Anexo 4 |
| D-7, los ocho diagramas chicos de CU-01 a CU-08 | **Se quitan** (resuelta por Claude con el criterio de JC, 14/11) | Ver «D-7» abajo |

### RN-31: verificación del flujo de pago (resuelta)

**No hay ningún momento intermedio.** En `GestorCompras.Confirmar`:
- la llamada a `PasarelaSimulada.Cobrar` es sincrónica y devuelve aprobado o rechazado;
- en la línea siguiente se inserta el pago con uno de esos dos estados.

No existe un pago grabado «en espera», ni una respuesta diferida de la pasarela. El enum
`EstadoPago` tiene solo `Aprobado` y `Rechazado`, y `CK_Pagos_Estado` admite esos dos.

Lo que está pendiente es **la compra** (`EstadoCompra.Pendiente`), mientras corre la reserva.
La base no está incompleta: el texto mezcló los estados de la compra con los del pago. **Se
aplica A-08.** No hay nada para Codex.

> Un matiz que no cambia A-08: si la tarjeta no tiene al menos cuatro dígitos, `ValidarTarjeta`
> la rechaza **antes** de llamar a la pasarela, y no se graba ningún pago. Es una validación del
> formulario, no una operación de pago, así que «toda operación de pago se registra» sigue
> siendo cierto.

### RN-11: se conserva (resuelta el 14/11)

**La primera resolución partió de un resumen mío impreciso**, que decía «RN-11 promete reprogramar
eventos, y no existe». Lo exacto: **RN-11 está construida y probada; lo único que no existe es una
palabra de su texto.**

- La regla dice: «un evento con entradas vendidas no puede eliminarse: sólo puede suspenderse
  **o reprogramarse**».
- `GestorEventos.Eliminar` la aplica: con entradas vendidas lanza `ReglaNegocioException("RN-11")`
  y no borra nada. `Suspender` es la alternativa.
- La prueban CP-21 (eliminar con ventas ofrece suspender) y CP-52 (eliminar el evento durante el
  pago).

**JC decidió conservarla sin «reprogramarse»:** se aplica A-02, y B-10 anota la reprogramación
entre las exclusiones. Las cifras siguen sobre 41 reglas.

### D-1 · pp. 57 a 61, «Implementación» — **resuelta: (a)**

Se reemplazan las cinco páginas por el texto del [Anexo 1](#anexo-1--texto-para-d-1-a). El título
queda «Arquitectura e implementación». El índice se corre unas 4 páginas (ítem 22 de
CORRECCIONES).

### Las que siguen abiertas: D-2, D-3, D-5 y D-6, completas

**Revisadas el 14/11 contra el texto del PDF v2.4 y el código de hoy.** Cada una trae el texto
actual tal como está en el documento, las dos opciones redactadas enteras, la recomendación y lo
que se pierde con cada camino. Al revisarlas aparecieron tres correcciones a lo que había escrito
el 13/11; están marcadas con **Corrige el 13/11**.

**Dependencias entre ellas:**
- D-6 depende del trabajo en curso de Codex (opción (c), ver D-6). También toca el RF-10 de la p. 43. Si D-3 queda en (a), esa copia desaparece y D-6 solo toca la
  p. 20, la p. 23 y la p. 55.
- D-2 (a) depende de `PorcentajePara`, que Codex cambió en `2af267a` (ya está incorporado).
- D-5 no depende de las otras. Sus cifras suponen RN-11 conservada (41 reglas) y 112 casos;
  cuando Codex entregue CP-113 y CP-114, «ciento doce» pasa a «ciento catorce» con cualquiera de
  las dos opciones, y los porcentajes no cambian, porque esos dos casos prueban reglas que ya
  tenían caso (RN-04, RN-41 y RN-42).

---

#### D-2 · pp. 63 a 70: el código y las pruebas que muestra el documento no son los reales

**Hallazgo.** La sección «Desarrollo de la lógica de descuento por nivel» muestra código escrito
antes de construir el sistema. Texto actual, bloque por bloque:

- **p. 63, «Definición de las estructuras»:**

```sql
CREATE TABLE Compras (
    ID                INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario        INT           NOT NULL,
    FechaCompra       DATETIME      NOT NULL,
    Estado            VARCHAR(12)   NOT NULL,   -- Pendiente|Confirmada|Reembolsada
    NivelAplicado     VARCHAR(8)    NOT NULL,   -- Bronce|Plata|Oro
    DescuentoAplicado DECIMAL(4,3)  NOT NULL,   -- 0.000 | 0.050 | 0.075
    BaseEntradas      DECIMAL(12,2) NOT NULL,
    MontoDescuento    DECIMAL(12,2) NOT NULL,
    TotalPagado       DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_Compras_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID),
    CONSTRAINT CK_Compras_Estado  CHECK (Estado IN ('Pendiente','Confirmada',
                                                    'Reembolsada'))
);
```

  La base real (`BaseDatos/01-esquema.sql`) tiene cuatro estados (agrega `Vencida`) y seis
  columnas más: `MontoServicios`, `CargoServicio`, `MontoSeguro`, `ConSeguro`, `MontoReembolsado`
  y `ReservaVence`.
- **p. 64, pseudocódigo:** `total ← subtotalE + servicios + cargo`. El real suma el seguro.
- **p. 64, párrafo:** «Los beneficios de la suscripción —seguro bonificado, preventa y
  prioridad— se resuelven fuera de este cálculo y no alteran el importe de la entrada.» En el
  real, el seguro bonificado sí está dentro del cálculo.
- **pp. 64 y 65, «Diseño de clases»:** `enum EstadoCompra { Pendiente, Confirmada, Reembolsada }`;
  `decimal BaseEntradas, MontoDescuento, TotalPagado;`; `ResumenDescuento` sin `Seguro`;
  `GestorFidelizacion` sin `AscenderSiCorresponde`; «del mismo modo que GestorClientes», que no es
  Singleton.
- **p. 65, `PorcentajePara`:** `case NivelFidelidad.Oro: return 0.075m;` y
  `case NivelFidelidad.Plata: return 0.050m;`. El real lee `DESCUENTO_ORO` y `DESCUENTO_PLATA` de
  `Parametros`, que es lo que exige la p. 33 del mismo documento.
- **p. 65, `DeterminarNivel`:** `AddMonths(-12)`, `if (n >= 7)` y `if (n >= 3)`. El real lee
  `FIDELIDAD_VENTANA_MESES`, `FIDELIDAD_UMBRAL_ORO` y `FIDELIDAD_UMBRAL_PLATA`.
- **p. 66, `CalcularDescuento`:** `decimal cargo = Math.Round(subtotalE * 0.15m, 2);` y
  `Total = subtotalE + servicios + cargo`. El real lee `CARGO_SERVICIO`, suma el seguro y empieza
  verificando la sesión (`RequerirUsuario`, RN-41).
- **p. 67, secuencia en texto:** `RegistrarCompra(resumen)`, `Procesar(total)` y un
  `UPDATE Compras SET Estado='Confirmada'` directo. En el real la compra se abre con
  `GestorCompras.Iniciar` (reserva de diez minutos) y se cobra en `GestorCompras.Confirmar`, que
  llama a `PasarelaSimulada.Cobrar`.
- **pp. 68 a 70, pruebas:** una clase con `Umbrales_AsignanNivelYPorcentaje`,
  `Descuento_NoAlcanzaALosServiciosComplementarios`, `Fake.ClienteCon(…)`, etc. Los métodos
  reales de `Pruebas/PruebasFidelizacion.cs` se llaman distinto y usan otros auxiliares.

**Opción (a): reemplazar cada bloque por el código real.** El texto completo está en el
[Anexo 2](#anexo-2--textos-para-d-2), bloque por bloque, revisado el 14/11 contra el código. Para
las pruebas, la versión (a) es pegar las líneas 38 a 177 de `Pruebas/PruebasFidelizacion.cs`
(commit `2af267a`) en lugar del listado de las pp. 68 a 70.

**Opción (b): dejar el código del documento y agregar una nota debajo de cada bloque.** Las siete
notas, en el orden de las páginas:

> **p. 63, debajo del bloque SQL:** «El esquema vigente está en `BaseDatos/01-esquema.sql`.
> Difiere de este bloque en que la compra admite un cuarto estado, Vencida (reserva que venció
> sin pago, RN-15), y en seis columnas más: MontoServicios, CargoServicio, MontoSeguro y
> ConSeguro (seguro, RN-28), MontoReembolsado (devolución, RN-29) y ReservaVence (vencimiento de
> la reserva, RN-15).»
>
> **p. 64, debajo del pseudocódigo:** «En el código vigente (`GestorFidelizacion.CalcularDescuento`)
> el total suma además el seguro: la cantidad de entradas por el precio del seguro, sin cargo si el
> cliente tiene suscripción activa (RN-28 y RN-37). El cargo por servicio se lee del parámetro
> CARGO_SERVICIO.»
>
> **p. 65, debajo de «Diseño de clases»:** «En el código vigente, EstadoCompra tiene además el
> estado Vencida; Compra guarda también MontoServicios, CargoServicio, MontoSeguro y
> MontoReembolsado; ResumenDescuento incluye el Seguro, y GestorFidelizacion agrega
> AscenderSiCorresponde (RN-22). Todos los gestores son Singleton.»
>
> **p. 65, debajo de «Implementación»:** «En el código vigente, los porcentajes, los umbrales y
> la ventana no están escritos en el código: se leen de la tabla Parametros (DESCUENTO_ORO,
> DESCUENTO_PLATA, FIDELIDAD_UMBRAL_ORO, FIDELIDAD_UMBRAL_PLATA y FIDELIDAD_VENTANA_MESES).»
>
> **p. 66, debajo de `CalcularDescuento`:** «En el código vigente, el método verifica primero que
> la sesión corresponda al cliente (RN-41), lee el cargo de CARGO_SERVICIO y suma el seguro al
> total.»
>
> **p. 67, debajo de la secuencia:** «En el código vigente, la compra se abre con
> GestorCompras.Iniciar, que reserva el cupo por diez minutos, y se cobra en
> GestorCompras.Confirmar, que llama a PasarelaSimulada.Cobrar. Si el pago se aprueba, el nivel
> se actualiza con ReevaluarNivel y AscenderSiCorresponde. Los diagramas de secuencia de la
> sección de diagramas muestran el flujo vigente.»
>
> **pp. 68 a 70, debajo del título «Implementación de las pruebas»:** el texto que ya está al
> final del [Anexo 2](#anexo-2--textos-para-d-2) («El código vigente está en
> `Pruebas/PruebasFidelizacion.cs`…»).

**Recomiendo (a) para las pp. 63 a 67 y (b) para las pp. 68 a 70.**
- **Por qué (a) para el código:** con (b), el documento sigue mostrando importes escritos en el
  código (p. 65) mientras la p. 33 dice que se leen de `Parametros`, y una secuencia que no
  coincide con el código. Eso es el error frecuente 10 del docente («secuencia que no coincide
  con el código»), y su regla es que el código manda.
- **Por qué (b) para las pruebas:** el listado real tiene 140 líneas monoespaciadas, contra 95
  de hoy. La nota dice dónde está el código y cómo se llaman los métodos, y no afirma nada falso.

**Qué se pierde con cada camino:**

| | (a) | (b) |
|---|---|---|
| Coherencia con el código | Total | Las contradicciones quedan escritas, con su nota al lado |
| Errores del docente | Pasa el 10 | Falla el 10 en la p. 67 |
| Trabajo de Máximo | Seis bloques monoespaciados, unos 40 minutos; es lo que peor convierte en Word | Siete párrafos, unos 15 minutos |
| Paginación | Unas 2 páginas más con las pruebas en (a); casi nada con las pruebas en (b) | Media página más |
| Cobertura y numeración | Sin cambio | Sin cambio |

**Corrige el 13/11:** el Anexo 2 tenía tres diferencias con el código. `DeterminarNivel` seguía
con `AddMonths(-12)` escrito en el código, y `CalcularDescuento` no mostraba la verificación de
sesión. Además, `PorcentajePara` cambió: Codex le agregó el tope `DESCUENTO_MAX_ADMISIBLE` (K-4,
commit `2af267a` del 14/11). El Anexo 2 ya está corregido; vale la versión «con el tope».

---

#### D-3 · pp. 42 y 43: la segunda copia de los requerimientos

**Hallazgo.** Después de las restricciones, el documento repite los requerimientos con otro
texto, bajo «Definición de requerimientos / Especificación de requerimientos funcionales». La
copia principal está en las pp. 22 y 23.

**Corrige el 13/11:** dije que contradecía en dos líneas. Son seis, y además le faltan RF-12 y
RF-13. Texto actual de cada una:

| Ítem | Dice hoy (pp. 42 y 43) | Qué contradice |
|---|---|---|
| 2 | «Criterio de aceptación: El sistema debe autenticar correctamente y redirigir al perfil del usuario.» | No hay perfil: se abre el menú del Cliente o del Administrador |
| 4 | «Las entradas seleccionadas deben aparecer correctamente en el carrito o paquete.» | No hay carrito (I-3 del chequeo) |
| 8 | «Aplicar descuentos según el nivel del usuario (Bronce, Plata, Oro) al total de la compra.» | RN-21: el descuento es solo sobre las entradas |
| 9 | «Sugerir eventos basados en historial simulado o intereses seleccionados por el usuario. Criterio de aceptación: Se deben mostrar al menos dos eventos sugeridos al finalizar la compra.» | RF-09 pasó a Won't have |
| 10 | «Permitir a los usuarios inscribirse para recibir notificación si se liberan entradas. Criterio de aceptación: El sistema debe confirmar la inscripción y mantener el orden de solicitud.» | RN-35 (primero los suscriptores) y D-6 (no hay notificación) |
| 11 | «El sistema debe generar reportes ficticios (ventas por evento, usuarios por nivel, uso de seguros). Criterio de aceptación: Deben visualizarse en forma de tabla o gráfico con datos simulados.» | B-02: hay tablas y PDF, no gráficos |
| 12 y 13 | No están | La copia principal tiene trece |

**Otra corrección al 13/11:** propuse «borrarla». La guía del docente, en su punto 11, pide que
«Definición de requerimientos» incluya minutas, prototipos **y especificación de
requerimientos**, y esta copia es justamente la que ocupa ese lugar, antes de las minutas de la
p. 44. Borrarla sin más deja el punto 11 sin especificación. Por eso la opción (a) ahora es
reemplazarla por una remisión.

**Opción (a): reemplazar la copia por una remisión.** Borrar desde «Especificación de
requerimientos funcionales» (p. 42) hasta el final del ítem 11 (p. 43). Dejar el título
«Definición de requerimientos» y, debajo, **poner:**

> La especificación de los trece requerimientos funcionales y de los cinco no funcionales, cada
> uno con su criterio de aceptación, está en la sección «Definición de requerimientos» (pp. 22 a
> 24). Su priorización está en «Priorización de requerimientos» (pp. 35 a 38), y su relación con
> las reglas, los casos de uso y las pruebas, en la matriz de trazabilidad. Esta sección reúne el
> material de relevamiento del que surgieron: las minutas de reunión y los prototipos de
> interfaz.

Y en la p. 111, trabajo futuro 5, **dice:** «Resolver la duplicación de contenido detectada entre
las secciones de requerimientos, minutas y prototipos del cuerpo del informe.» **Poner:**
«Resolver la duplicación de contenido detectada entre las minutas y los prototipos del cuerpo del
informe.» En el índice desaparece la entrada «Especificación de requerimientos funcionales
… 42».

**Opción (b): corregir la copia.** Reemplazar los ítems 2, 4, 8, 9, 10 y 11 y agregar el 12 y
el 13. Texto completo de cada uno:

> 2. Inicio de sesión. Permitir a los usuarios acceder al sistema utilizando su cuenta
>    registrada. Criterio de aceptación: El sistema debe autenticar correctamente y abrir el menú
>    que corresponde al perfil del usuario.
>
> 4. Selección de entradas. El usuario podrá seleccionar sector y cantidad de entradas según
>    disponibilidad. Criterio de aceptación: Las entradas seleccionadas deben aparecer
>    correctamente en el resumen del paquete.
>
> 8. Aplicación de beneficios por nivel de usuario. Aplicar el descuento que corresponde al
>    nivel del usuario (Bronce, Plata, Oro) sobre el valor de las entradas. Criterio de
>    aceptación: El descuento debe aplicarse automáticamente y ser visible antes de confirmar la
>    compra.
>
> 9. Recomendaciones automáticas. Excluido del alcance: pasó a Won't have en la versión 2.5.
>
> 10. Lista de espera para eventos agotados. Permitir a los usuarios inscribirse en la lista de
>     espera de un evento agotado. Criterio de aceptación: El sistema debe confirmar la
>     inscripción y ordenar la lista primero por los suscriptores y después por el resto, cada
>     grupo por orden de inscripción. *(Con D-6 (b), agregar: «El aviso cuando se libera un
>     lugar no está implementado en esta versión (W-03).»)*
>
> 11. Visualización de reportes. El sistema debe generar cinco reportes de gestión (ventas por
>     evento, usuarios por nivel, adopción de beneficios, ranking de paquetes y demanda
>     insatisfecha) a partir de los datos registrados. Criterio de aceptación: Deben
>     visualizarse en forma de tabla y poder exportarse a PDF.
>
> 12. Gestión de eventos. Permitir al Administrador crear, suspender y eliminar eventos, agregar
>     y quitar sus sectores con capacidad y precio, y fijar su período de preventa. Criterio de
>     aceptación: El sistema debe impedir la publicación de un evento sin un sector con capacidad
>     mayor que cero, y no debe permitir eliminar un evento con entradas vendidas.
>
> 13. Contratación de suscripciones. Permitir al Cliente contratar y cancelar un plan mensual o
>     anual. Criterio de aceptación: El sistema debe impedir dos suscripciones activas
>     simultáneas y aplicar los beneficios en especie, sin descuento porcentual sobre la entrada.

**Recomiendo (a).**
- La copia no agrega nada que no esté en las pp. 22 y 23, y el propio documento la marca como
  duplicación en su trabajo futuro 5.
- Con (b), el documento queda con dos versiones del mismo requerimiento que hay que mantener
  iguales; ya divergieron una vez.
- (a) es además menos trabajo, y conserva la estructura del punto 11 de la guía.

**Qué se pierde con cada camino:**

| | (a) | (b) |
|---|---|---|
| Punto 11 de la guía | Pasa con remisión: la especificación está, en otra página | Pasa con la especificación en su lugar |
| Riesgo | Que el docente quiera la especificación físicamente dentro del punto 11 | Dos textos del mismo requerimiento que pueden volver a divergir |
| Trabajo de Máximo | Borrar dos páginas y pegar un párrafo: 5 minutos | Ocho ítems: unos 20 minutos |
| Paginación | El documento pierde casi dos páginas; el índice se corre | Crece un cuarto de página |
| D-6 | La p. 43 deja de existir | Hay que aplicar D-6 también en el ítem 10 |
| Cobertura y numeración | Sin cambio | Sin cambio |

---

#### D-7 · Los ocho diagramas chicos de CU-01 a CU-08 — **resuelta el 14/11: se quitan**

**Criterio de JC:** si cada diagrama chico no muestra nada que el general no muestre, se quita;
si muestra lo propio de su caso de uso (inclusiones, extensiones, actores particulares), se
regenera.

**Lo que se comparó.** Las imágenes de las pp. 72 a 85, extraídas del PDF, contra el diagrama
general nuevo (`casos-uso-cliente.puml` y `casos-uso-administrador.puml`), que sale del código.

**Lo que muestra cada uno que el general no muestra, y si existe en el código:**

| CU | Lo propio del diagrama chico | ¿Existe? |
|---|---|---|
| CU-01 | «Seleccionar evento» y «Armar paquete personalizado» como «include» de «Comprar paquete»; «Aplicar descuentos por nivel/suscripción»; «Confirmar pago compra» | No como casos de uso. Seleccionar y armar son pasos del propio CU-01 (pasos 2 a 5): dibujarlos como «include» es descomposición funcional. La suscripción no da descuento (RN-37). El pago y el descuento por nivel ya están en el general |
| CU-02 | «Cargar evento nuevo», «Modificar evento existente» y «Desactivar / eliminar evento» como «include» | No. No hay modificación de eventos, y alta y baja son opciones del mismo caso, no casos incluidos |
| CU-06 | «Simular pago suscripción» incluido, con la **pasarela de pago como actor** | No. La suscripción no cobra (RF-13) y la pasarela es parte del sistema: es el error 12 |
| CU-03 a CU-05, CU-07 y CU-08 | El caso de uso con su actor, más las variantes de cada texto viejo | Lo que es cierto ya está en el general |

**Conclusión: se quitan los ocho.** Las inclusiones, extensiones y actores reales de cada caso
de uso están todos en el diagrama general. Regenerarlos desde el código daría ocho recortes del
general, sin nada propio. Lo único propio que muestran hoy son errores: la descomposición
funcional de CU-01 y CU-02 y la pasarela como actor en CU-06.

**Corrección a la consigna:** no son catorce páginas. Son ocho figuras chicas repartidas en las pp.
72 a 85, y quitarlas ahorra entre dos y tres páginas.

**Con esto, CU-06 queda sin la pasarela como actor**, que era la condición de JC para cualquiera
de los dos caminos.

#### D-4 · Casos de uso sin especificación — **resuelta el 14/11: se escriben**

Las cinco especificaciones están en el [Anexo 3](#anexo-3--especificaciones-cu-09-a-cu-13),
redactadas desde el código. Los títulos de CU-06, CU-07 y CU-08 se alinean con el diagrama: «CU-06
– Gestionar suscripción», «CU-07 – Consultar listas de espera» y «CU-08 – Generar reportes».

---

#### D-5 · Qué se cuenta como regla con caso de prueba

**Hallazgo.** Tres casos tocan dos reglas sin probar lo que la regla prohíbe:
- **RN-26**, «La suscripción no se renueva automáticamente». CP-77 solo comprueba que la pantalla
  muestre el texto «No se renueva automáticamente» (`PruebasGrupoASuscripcion.cs`, línea 83).
  Ninguna prueba deja vencer una suscripción y verifica que no se renueve.
- **RN-28**, «El seguro se contrata solo durante la compra; no admite incorporación posterior».
  CP-41, CP-50, CP-79 y CP-80 contratan o cobran el seguro durante la compra. Ninguno intenta
  agregarlo después.

El comentario del código de CP-77 dice «RN-25/RN-26», y los de CP-79 y CP-80 dicen «RN-28». Por
eso la cifra depende de qué se cuenta.

**Texto actual en el documento** (v2.4, antes de las correcciones): p. 8, «la cobertura de
verificación alcanza al 22,2 % de las reglas de negocio, concentrada en la lógica de
fidelización»; p. 108, «Los diez casos verifican ocho reglas distintas … treinta y seis vigentes
… 22,2 % … veintiocho»; pp. 109 a 111, G-02, limitaciones y trabajo futuro 2 con esas cifras.

**Opción (a): no contarlas. Da 37 de 41, 90,2 %.** Es el texto de la sección 5, completo, sin
cambios. Las reglas sin caso son RN-07, RN-26, RN-28 y RN-33.

**Opción (b): contarlas. Da 39 de 41, 95,1 %.** Reemplaza las filas de la sección 5 así:

| Pág. | Poner |
|---|---|
| 8 | «la cobertura de verificación alcanza al 95,1 % de las reglas de negocio: dos de las cuarenta y una no tienen todavía un caso de prueba automatizado, y ninguna de esas dos rechaza operaciones.» |
| 108, párrafo | «Los ciento doce casos verifican treinta y nueve reglas distintas sobre un total de cuarenta y una vigentes, lo que arroja una cobertura del 95,1 %. Las dos restantes carecen de verificación automatizada: RN-07 y RN-33. Ninguna rechaza operaciones.» |
| 108, análisis | «41 / 39 / 2 reglas / 95,1 %» |
| 109, G-02 | «95,1 % (39 de 41)»; reglas sin caso: «RN-07 y RN-33» |
| 110, limitaciones | «La cobertura de pruebas alcanza el 95,1 % de las reglas. \| Todos los requerimientos en alcance tienen pruebas; dos reglas, ninguna de las que rechazan operaciones, carecen de verificación automatizada.» |
| 111, trabajo futuro 2 | «Completar los casos de prueba de las dos reglas sin verificación (RN-07 y RN-33), de modo de cerrar la brecha G-02.» |

Y en `docs/04-casos-prueba.md`, agregar RN-26 a CP-77 y RN-28 a CP-79 y CP-80.

**Recomiendo (a).**
- Es la cifra que JC ya dio, y es la honesta: leer un texto en pantalla no prueba que algo no
  ocurra.
- Con (b), si el docente abre CP-77 para ver cómo se prueba RN-26, encuentra una comparación de
  texto, y la cifra pierde credibilidad entera.

**Qué se pierde con cada camino:**

| | (a) | (b) |
|---|---|---|
| Cifra | 90,2 % | 95,1 % |
| Coherencia con el código | El registro y los comentarios de CP-77, CP-79 y CP-80 dicen cosas distintas: Codex tendría que quitar «RN-26» y «RN-28» de esos tres comentarios, unos 5 minutos | Coincide con los comentarios |
| Defensa ante el docente | Firme | Débil en dos reglas |
| Trabajo de Máximo | Ninguno extra: la sección 5 ya está escrita así | Seis filas, unos 10 minutos |

**Un camino que da 95,1 % y es honesto:** (a) ahora, más dos pruebas reales que escribe Codex.
- Para RN-26: dejar vencer una suscripción y comprobar que no queda activa ni se crea otra.
- Para RN-28: intentar agregar el seguro a una compra confirmada y comprobar que se rechaza.

Es más o menos una hora de Codex más una corrida de la batería. Si entran antes de la entrega,
la sección 5 pasa a las cifras de (b), sin la debilidad de (b).

---

#### D-6 · La lista de espera promete un aviso que no existe

> **Novedad del 14/11, a la tarde: Codex está construyendo una tercera vía, (c).** En su cambio
> sin commitear, el cliente consulta su posición en la lista y, al iniciar sesión, ve qué eventos
> de su lista volvieron a tener entradas (CP-117 y siguientes). Si eso es encargo de JC, D-6 queda
> resuelta por (c) cuando Codex lo commitee, y (a) y (b) quedan de respaldo. Los textos de (c)
> están al final de esta sección, marcados como provisorios.

**Hallazgo.** El sistema no avisa a nadie cuando se libera un lugar: W-03 lo excluye y ningún
código lo hace. Sin embargo, la pantalla y el documento lo prometen.

**En el código, tres textos** (zona de Codex):
- `Visual/Cliente/FrmCompraCliente.cs`, líneas 253 y 254: «Te anotaste en la lista de espera de
  «…». Te vamos a notificar si se libera un lugar.»
- `Visual/Cliente/FrmCompraCliente.Designer.cs`, línea 170 (`lblAgotado`): «Entradas
  agotadas. Anotate en la lista de espera y te vamos a notificar si se libera un lugar.»
- `Repo/GestorListaEspera.cs`, línea 87: «Ya estás en la lista de espera de este evento. Te
  avisaremos si se libera un lugar.»

**En el documento, texto actual:**
- **p. 20, criterio 10:** «Lista de espera para eventos agotados: El sistema debe registrar al
  usuario en la lista e informarle que será notificado.»
- **p. 23, RF-10:** «Permitir a los usuarios anotarse si no hay entradas disponibles. Criterio de
  aceptación: El sistema debe registrar al usuario en la lista e informarle que será
  notificado.»
- **p. 43, ítem 10:** ver D-3.
- **p. 40** («si se libera un cupo, se le notifica automáticamente por correo electrónico») **y
  p. 41** («lista de espera que notifica automáticamente»): ya los reemplazan B-09 y B-10, que
  están aprobados.

**Corrige el 13/11:** dije que «los mensajes de la p. 55» dependían de esto. La p. 55 de hoy no
promete nada: dice «¿Desea sumarse a la lista de espera?» y «Usted ya figura en la lista de espera
de este evento.». Lo que depende de D-6 son los textos de reemplazo que propone la sección 6 para
esas dos filas, porque copian la pantalla.

**Opción (a): Codex quita la promesa y el documento dice «confirma la inscripción».**

Codex cambia los tres textos por:
- «Te anotaste en la lista de espera de «…».»
- «Entradas agotadas.» y, en la línea siguiente, «Anotate en la lista de espera.»
- «Ya estás en la lista de espera de este evento.»

El documento queda así:
- **p. 20, criterio 10:** «10. Lista de espera para eventos agotados: El sistema debe registrar
  al usuario en la lista y confirmarle la inscripción.»
- **p. 23, RF-10, criterio:** «El sistema debe registrar al usuario en la lista y confirmarle la
  inscripción. La lista se ordena primero por los suscriptores y después por el resto (RN-35).»
- **p. 55:** las filas RN-10 y RN-36 de la sección 6, con su texto «Con D-6 (a)».
- **B-09 y B-10:** tal como están.

**Opción (b): el código queda como está y el documento lo declara como limitación.**
- **p. 20, criterio 10:** «10. Lista de espera para eventos agotados: El sistema debe registrar
  al usuario en la lista e informarle que será notificado. En esta versión el aviso no se envía
  (W-03): se declara como limitación.»
- **p. 23, RF-10, criterio:** «El sistema debe registrar al usuario en la lista e informarle que
  será notificado. En esta versión el aviso no se envía (W-03).»
- **p. 55:** las filas RN-10 y RN-36 de la sección 6, con su texto «Con D-6 (b)».
- **B-09:** la oración de la lista de espera termina con «; el sistema no envía avisos (W-03)».
- **B-10:** la exclusión de los avisos agrega: «la inscripción en la lista de espera se confirma
  en pantalla, pero el sistema no avisa cuando se libera un lugar».
- **p. 110, limitaciones, fila nueva:** «La pantalla de la lista de espera anuncia un aviso que
  esta versión no envía. | Declarado; el aviso queda para una iteración futura (W-03).»

**Recomiendo (a).** Es el único caso en que «el código manda» se aplica al revés: el código
promete algo que el sistema no hace, y lo que hay que corregir es el código.
- Con (b), el documento admite por escrito que la pantalla le dice al cliente algo falso. Para
  el docente eso es un defecto declarado, no una limitación.

**Qué se pierde con cada camino:**

| | (a) | (b) |
|---|---|---|
| Código | Codex cambia tres textos, unos 15 minutos, más una corrida de la batería. Verifiqué que ninguna prueba compara esos textos y que ninguna figura del instructivo los muestra | Nada |
| Coherencia | Pantalla, documento y alcance dicen lo mismo | Coherente, pero con un defecto a la vista |
| Trabajo de Máximo | Dos criterios y dos filas: 10 minutos | Dos criterios, dos filas, dos agregados y una fila de limitaciones: 15 minutos |
| Cobertura y numeración | Sin cambio | Sin cambio |
| Dependencia | La corrida de Codex tiene que entrar antes de que Máximo pegue la p. 55 | Ninguna |

**Opción (c), en curso por Codex (provisoria: se confirma contra su commit).** Lo que hace el
código sin commitear, leído el 14/11:
- en la compra, un enlace «Mi posición» muestra «Tu posición actual en la lista es N.», en el
  orden de RN-35;
- al iniciar sesión, si algún evento de la lista del cliente volvió a tener entradas, el sistema
  muestra «Hay entradas disponibles nuevamente en:», la lista de eventos, y «Podés volver al
  listado para consultar y comprar. No es una reserva de lugar.»;
- el aviso se calcula cada vez que el cliente inicia sesión: no reserva el lugar, y les llega a
  todos los inscriptos del evento a la vez, no solo al primero de la lista;
- no hay correo ni notificación push: W-03 sigue excluido.

Textos con (c):
- **p. 20, criterio 10:** «10. Lista de espera para eventos agotados: El sistema debe registrar
  al usuario en la lista, permitirle consultar su posición y avisarle al iniciar sesión cuando el
  evento vuelve a tener entradas.»
- **p. 23, RF-10, criterio:** «El sistema debe registrar al usuario en la lista y confirmarle la
  inscripción. El cliente puede consultar su posición, que respeta el orden de RN-35. Si el
  evento vuelve a tener entradas, el sistema se lo informa al iniciar sesión; el aviso no reserva
  el lugar.»
- **p. 55, fila RN-10:** «Entradas agotadas. Anotate en la lista de espera. Los avisos de
  disponibilidad se muestran al iniciar sesión.»
- **p. 55, fila RN-36:** «Ya estás en la lista de espera de este evento. Podés consultar tu
  posición en la compra; los avisos de disponibilidad se muestran al iniciar sesión.»
- **B-09:** la oración «Si un evento está agotado, el cliente puede inscribirse en su lista de
  espera.» pasa a «Si un evento está agotado, el cliente puede inscribirse en su lista de espera y
  consultar su posición; si vuelve a haber entradas, el sistema se lo avisa al iniciar sesión.»
- **B-10, inclusiones:** «la lista de espera para los eventos agotados» pasa a «la lista de
  espera para los eventos agotados, con la consulta de la posición y el aviso al iniciar sesión
  cuando vuelve a haber entradas».
- **B-10, exclusiones:** «los avisos por correo electrónico, incluido el aviso de la lista de
  espera (W-03)» pasa a «los avisos por correo electrónico o notificación push (W-03); el aviso de
  la lista de espera se muestra dentro de la aplicación».
- **Anexo 3, CU-13, postcondición:** «Nadie de la lista de espera recibe aviso (W-03).» pasa a
  «Los clientes de la lista de espera del evento ven el aviso la próxima vez que inician
  sesión.»
- **CU-05, flujo principal:** agregar el aviso como último paso del inicio de sesión del
  Cliente. El texto, cuando Codex commitee.

**Si (c) se confirma, la recomendo sobre (a) y (b):** cumple lo que la pantalla prometía, sin
infraestructura externa, y el documento no necesita declarar un defecto.
- Un punto para que decida JC: el aviso les llega a todos a la vez, así que el orden de RN-35
  solo se ve en la posición, y no decide quién compra primero. El texto de (c) lo dice («no
  reserva el lugar»).

## 5. Cifras de la matriz y del resumen (reemplazan las de CORRECCIONES)

| Pág. | Dice hoy (v2.4) | Poner | Clase |
|---|---|---|---|
| 7 | «Treinta y seis reglas vigentes» | «Cuarenta y una reglas vigentes» | R |
| 7, fila Verificación | «…casos de uso, pruebas y hallazgos.» | «…casos de uso, ciento doce casos de prueba automatizados y hallazgos.» | R |
| 8 | «la cobertura de verificación alcanza al 22,2 % de las reglas de negocio, concentrada en la lógica de fidelización.» | «la cobertura de verificación alcanza al 90,2 % de las reglas de negocio: cuatro de las cuarenta y una no tienen todavía un caso de prueba automatizado, y ninguna de esas cuatro rechaza operaciones.» | R |
| 106 | «las treinta y seis reglas, los ocho casos de uso y los diez casos de prueba» | «las cuarenta y una reglas, los ocho casos de uso y los ciento doce casos de prueba» | R |
| 106, nomenclatura | RN-xx «36»; CP-xx «10» | RN-xx «41»; CP-xx «112» | R |
| 108, párrafo | «Los diez casos verifican ocho reglas distintas … treinta y seis vigentes … 22,2 % … veintiocho» | «Los ciento doce casos verifican treinta y siete reglas distintas sobre un total de cuarenta y una vigentes, lo que arroja una cobertura del 90,2 %. Las cuatro restantes carecen de verificación automatizada: RN-07, RN-26, RN-28 y RN-33. Ninguna rechaza operaciones.» | R |
| 108, análisis | «Reglas … 36 / 8 / 28 reglas / 22,2 %» | «41 / 37 / 4 reglas / 90,2 %» | R |
| 108, análisis | «Reglas con origen declarado 36 / 36» | «41 / 41» | R |
| 108, análisis | «Requerimientos con caso de prueba 13 / 3 / 10 / 23,1 %» | «13 / 12 / RF-09 (Won't) / 92,3 %» | R |
| 109, G-02 | (texto del ítem 20 de CORRECCIONES) | Cambiar «86,8 % (33 de 38)» por «90,2 % (37 de 41)» y la lista de reglas sin caso por «RN-07, RN-26, RN-28 y RN-33» | R |
| 109, G-05 | «RN-35 queda sin caso» (ítem 20 de CORRECCIONES) | «RN-35 tiene sus casos (CP-83 y CP-84)» | R |
| 109, G-07 | «CP-«S1» a CP-«S14»» (ítem 25 de CORRECCIONES) | «CP-91 a CP-104» | R |
| 110 | «Explicitar las treinta y seis reglas» | «Explicitar las cuarenta y una reglas» | R |
| 110, limitaciones | «La cobertura de pruebas alcanza el 22,2 % de las reglas. \| Diez de trece requerimientos carecen de verificación automatizada.» | «La cobertura de pruebas alcanza el 90,2 % de las reglas. \| Todos los requerimientos en alcance tienen pruebas; cuatro reglas, ninguna de las que rechazan operaciones, carecen de verificación automatizada.» | R |
| 110, limitaciones (fila nueva) | — | «Cuatro de las 184 ejecuciones fallan: tres de presentación a escala 150 % (CP-69, CP-76, CP-85) y una de comportamiento (CP-81). \| Están identificadas y abiertas.» | N (última batería) |
| 111, trabajo futuro 2 | (ítem 21 de CORRECCIONES) | «Completar los casos de prueba de las cuatro reglas sin verificación (RN-07, RN-26, RN-28 y RN-33), de modo de cerrar la brecha G-02.» | R |
| 28 y 110 | «$19.356» | «$19.355» (como el ítem 13 de CORRECCIONES). Recalculado: todas las contribuciones, el 8,62 % y el 9,5 % quedan iguales | R |
| 23 de CORRECCIONES | «57», CP-«66» a CP-«90» | CP-66 a CP-90 son reales; el «57» sale de `docs/TANDAS-PRUEBAS-64.md` | N |

**Verificadas sin cambio:**
- puntos de esfuerzo de la p. 36 (49 / 18 / 11 / 78; 62,8 %, 23,1 % y 14,1 %);
- Fase 4 = 41 puntos (52,6 %);
- 312 h;
- tiempos de la p. 56 (4 min 40 s);
- ejemplo de la p. 54 ($99.625, igual a CP-11);
- contribución de la p. 67 ($9.240, $2.085 y $7.155);
- tabla de la p. 28;
- $6.066.000;
- versiones de la p. 112 (iText 9.2.0, SqlClient 6.0.2 y QRCoder 1.6.0).

**Matriz de la p. 107, columna «Casos de prueba».** Sumar a las filas del ítem 18 de
CORRECCIONES, y usar las filas RF-02, RF-07 y RF-12 del ítem 25 con estos números:

| Fila | Casos que se suman |
|---|---|
| RF-02 | CP-66, CP-67, CP-68, CP-95 a CP-98, CP-105, CP-107, CP-108 |
| RF-03 | CP-81, CP-106 |
| RF-04 | CP-94 |
| RF-05 | CP-79, CP-81 |
| RF-06 | CP-80 |
| RF-07 | CP-65, CP-80, CP-99 a CP-104, CP-110, CP-111 |
| RF-08 | CP-91, CP-92 |
| RF-10 | CP-83, CP-84 |
| RF-11 | CP-85 a CP-90, CP-109 |
| RF-12 | CP-69 a CP-75, CP-82, CP-85, CP-93, CP-94 |
| RF-13 | CP-76, CP-77, CP-78, CP-94 |

Debajo de la tabla: «RNF-01: CP-112. RNF-02: CP-62, CP-63, CP-71, CP-76, CP-85, CP-88, CP-112.
RNF-03: CP-64, CP-69, CP-85.» Sale de los comentarios `// CP-xx — RF-yy` de `Pruebas/`.

## 6. Pantallas y casos de uso

En los casos de uso, las T-x son las contradicciones de `CONTRADICCIONES-AL-PASO.md`: acá va su
texto de reemplazo. Con D-4 resuelta, los títulos quedan «CU-06 – Gestionar suscripción», «CU-07 –
Consultar listas de espera» y «CU-08 – Generar reportes», en el título de la especificación y en
el campo «Id y nombre». Los cinco casos nuevos están en el Anexo 3.

### Guion de interfaz e instructivo (pp. 48 a 55)

**E-01 · p. 48 · R (T-5).**
- **Dice:** «aunque el mismo formato se emplea para agregar locales, eventos, vehículos y otras
  entidades.»
- **Poner:** «aunque el mismo formato se emplea para agregar locales, eventos y sectores.»

**E-02 · p. 49 · R (T-6).**
- **Dice:** «(por ejemplo, Cliente, Administrador de local, Conductor)».
- **Poner:** «(Cliente o Administrador)».

**E-03 · p. 51, «Composición de un paquete» · R.** Tres filas:

| Componente | Columna «Cantidad admitida»: dice | Poner |
|---|---|---|
| Merchandising | «De 0 a 3 artículos» | «0 o 1» |
| Experiencia VIP | «0 o 1; exige entrada de sector VIP» | «0 o 1» |
| Seguro de entrada | «1 por entrada adquirida» | «Se contrata para todas las entradas de la compra: una póliza por entrada» |

*Razón:* código. `GestorPaquetes.AgregarServicio` admite cada servicio una vez, sin cantidad y
sin condición de sector; `Paquete.ConSeguro` vale para la compra entera.

**E-04 · p. 51, tabla de controles de la pantalla de armado · R.** Reemplazar la tabla
completa por:

| Control | Tipo | Contenido y comportamiento |
|---|---|---|
| label1 | Etiqueta | Nombre del evento. Sólo lectura. |
| lblNivel | Etiqueta | Nivel de fidelidad vigente y porcentaje de descuento, por ejemplo «Tu nivel: Plata · 5,0 % de descuento». |
| lnkNiveles | Enlace | «¿Cómo funcionan los niveles?»: explica los umbrales vigentes (RN-19). |
| dgvSectores | Grilla | Una fila por sector, con su nombre, su precio y las entradas disponibles. El precio queda congelado al iniciar la compra (RN-16). En la columna «Cantidad (escribí aquí)» el cliente escribe cuántas entradas quiere. La suma no puede superar 6 (RN-12) ni el cupo del sector (RN-08). |
| chkHospedaje / chkTraslado / chkMerch / chkVIP | Casilla | Servicios complementarios. Aparecen deshabilitados si el evento no los ofrece (RN-14). |
| chkSeguro | Casilla | Seguro de entrada, con devolución hasta 48 h antes. Se habilita solo cuando hay al menos una entrada (RN-28). |
| pnlResumen | Panel | Desglose del paquete. Se recalcula automáticamente ante cada cambio. |
| lblAgotado / btnListaEspera | Etiqueta y botón | Aparecen solo con el evento agotado, y ofrecen la inscripción en la lista de espera (RN-10). |
| btnComprar | Botón | Reserva el cupo y abre el pago. Deshabilitado mientras no haya al menos una entrada (RN-13). Sin seguro, pide confirmar que se continúa sin él. |
| btnVolver | Botón | «Volver al listado». Si hay un paquete armado, pide confirmación antes de descartarlo; no reserva cupo. |

**E-05 · p. 52, «Recorrido paso a paso», pasos 6 y 7 · R.**

| Paso | Columna | Dice | Poner |
|---|---|---|---|
| 6 | Acción del cliente | «Revisa el resumen del paquete.» | «Revisa el resumen del paquete y presiona «Comprar».» |
| 6 | Respuesta del sistema | «Muestra base de entradas, descuento por nivel, servicios sin descuento, cargo por servicio y total.» | «Muestra base de entradas, descuento por nivel, servicios sin descuento, cargo por servicio, seguro y total. Al confirmar, reserva el cupo por diez minutos y abre el pago.» |
| 6 | Reglas | «RN-20, RN-21, RN-37» | «RN-15, RN-20, RN-21, RN-37» |
| 7 | Respuesta del sistema | «Reserva el cupo por diez minutos, tokeniza los datos y envía la operación a la pasarela simulada.» | «Tokeniza los datos y envía la operación a la pasarela simulada, dentro del plazo de la reserva.» |

*Razón:* código. `GestorCompras.Iniciar` reserva al presionar «Comprar»; la pantalla de pago
llega después y muestra la cuenta regresiva.

**E-06 · p. 55, «Mensajes de validación» · R.**
- **En la oración inicial, dice:** «Cada bloqueo se acompaña de un mensaje que indica la causa y
  la acción necesaria para resolverla.»
- **Poner:** «Cada bloqueo se acompaña de un mensaje que indica la causa y la acción necesaria
  para resolverla, o deja deshabilitado el control cuando la acción no corresponde.»
- **En la columna «Mensaje mostrado»:**

| Regla | Dice | Poner |
|---|---|---|
| RN-08 | «El sector seleccionado no dispone de esa cantidad de entradas.» | «En el sector Campo quedan 3 entradas.» (el sector y la cantidad son los del caso) |
| RN-12 | «No es posible adquirir más de seis entradas por operación.» | «Se pueden comprar como máximo 6 entradas por operación.» |
| RN-13 | «El paquete debe incluir al menos una entrada.» | Sin mensaje: «Comprar» y la casilla del seguro quedan deshabilitados hasta elegir una entrada. |
| RN-14 | «Este evento no ofrece el servicio seleccionado.» | Sin mensaje: la casilla del servicio aparece deshabilitada. |
| RN-15 | «La reserva expiró. Debe reiniciar la compra.» | «Venció el plazo de la reserva y se liberó el cupo. Volvé a armar la compra.» |
| RN-32 | «El pago fue rechazado. Puede reintentar o cancelar la operación.» | «Pago rechazado: Rechazada por el emisor (simulación: tarjeta terminada en 0000). Se liberaron las entradas. Podés reintentar antes de que venza la reserva.» |
| RN-10 | «Este evento se encuentra agotado. ¿Desea sumarse a la lista de espera?» | Con D-6 (a): «Entradas agotadas. Anotate en la lista de espera.» Con D-6 (b): el texto de hoy, «Entradas agotadas. Anotate en la lista de espera y te vamos a notificar si se libera un lugar.» |
| RN-36 | «Usted ya figura en la lista de espera de este evento.» | Con D-6 (a): «Ya estás en la lista de espera de este evento.» Con D-6 (b): el de hoy, que agrega «Te avisaremos si se libera un lugar.» |

### Prototipos (pp. 39, 40 y 45)

**E-07 · R.** Valen para la copia que quede, según el dato de JC n.º 3 de CORRECCIONES.

| Pág. | Dice | Poner |
|---|---|---|
| 39, ítem 3 | «Muestra el resumen completo del paquete, aplica descuentos por nivel de usuario o suscripción, y ofrece opción de continuar al pago.» | «Muestra el resumen completo del paquete, aplica el descuento por nivel de usuario y ofrece opción de continuar al pago.» |
| 40, ítem 5 | «Presenta un mensaje de éxito con un código de compra ficticio y recomendaciones de eventos relacionados.» | «Presenta un mensaje de éxito con el número de pedido, el desglose del importe y la carpeta donde quedan los tickets en PDF, uno por entrada, con su código QR.» |
| 40, ítem 6 | «Muestra el nivel alcanzado (Bronce, Plata, Oro), beneficios disponibles, historial de compras y acceso a listas de espera.» | «Da acceso a los eventos disponibles, al historial de compras con la devolución de entradas aseguradas y a la suscripción con sus beneficios. El nivel alcanzado se muestra al armar el paquete.» |
| 45, ítem 5 | «Muestra mensaje de éxito, código de operación ficticio, resumen de compra y eventos recomendados.» | «Muestra mensaje de éxito, número de pedido, resumen de compra y la ubicación de los tickets en PDF con su código QR.» |
| 45, ítem 6 | «Permite ver el historial de compras, nivel de rankeo alcanzado (Bronce, Plata, Oro) y beneficios aplicables.» | «Permite ver el historial de compras y devolver entradas aseguradas, y contratar o cancelar la suscripción. El nivel alcanzado se muestra al armar el paquete.» |
| 45, ítem 7 | «Simula reportes gráficos y tablas con ventas por evento, adopción de beneficios, y usuarios por nivel de fidelidad.» | «Presenta en tablas, exportables a PDF, cinco reportes: ventas por evento, ranking de paquetes, usuarios por nivel de fidelidad, adopción de seguros y suscripciones, y demanda insatisfecha.» |

### Entidades (p. 62)

**E-08 · p. 62, tabla de entidades, fila «Entradas» · R.**

| Columna | Dice | Poner |
|---|---|---|
| Definición | «Se agrega ID_Compra como clave foránea hacia Compras.» | «Se agregan ID_Compra (clave foránea hacia Compras), ID_Sector, Estado, CodigoQR y FechaIngreso. Cada fila pasa a ser una entrada individual.» |
| Justificación | «Vincula cada entrada con la operación que la originó.» | «Vincula cada entrada con la operación que la originó, y permite el cupo por sector (RN-08), el QR único (RN-18) y la devolución por entrada (RN-29).» |

*Razón:* base. `BaseDatos/01-esquema.sql` y `GestorCompras.Iniciar`, que graba una fila por
entrada.

### Especificaciones de casos de uso (pp. 72 a 85)

**E-09 · CU-01, pp. 72 y 73 · R (T-1 y T-2).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «arma un paquete personalizado (entradas + servicios), se aplica el descuento por nivel/suscripción y se realiza un pago simulado. Si todo es correcto, se registra la compra y se genera la entrada digital» | «arma un paquete personalizado (entradas, servicios y, si quiere, seguro), se aplica el descuento por nivel de fidelidad y se realiza un pago simulado. Si todo es correcto, se registra la compra y se genera una entrada digital por cada entrada comprada.» |
| Paso 4 | «El sistema calcula el total del paquete y aplica automáticamente descuentos por nivel de fidelidad y/o suscripción activa.» | «El sistema calcula el total del paquete y aplica automáticamente el descuento por nivel de fidelidad; con suscripción activa, el seguro no tiene cargo.» |
| Paso 5 | «El Cliente confirma y pasa a la pantalla de pago simulado.» | «El Cliente confirma; el sistema reserva el cupo por diez minutos y abre la pantalla de pago simulado.» |
| Paso 8, segunda viñeta | «Actualiza los puntos / nivel del Cliente.» | «Actualiza el contador de compras del Cliente y, si corresponde, su nivel, que rige desde la compra siguiente.» |
| Paso 8, tercera viñeta | «Genera la entrada digital (código QR) y la asocia al Cliente.» | «Genera un ticket en PDF por entrada, cada uno con su código QR único.» |
| FA1 | «Si el Cliente intenta confirmar sin entradas seleccionadas, el sistema muestra mensaje de error y vuelve al paso 3.» | «Mientras el paquete no tenga al menos una entrada, el sistema mantiene deshabilitados el botón «Comprar» y la casilla del seguro, y el Cliente sigue en el paso 3.» |
| Después de FA3, agregar | — | «FA4 – Evento en preventa. Si el evento está en preventa y el Cliente no tiene suscripción activa, el sistema informa que la compra está reservada a suscriptores y no permite armar el paquete (RN-39).» |
| Después de FA4, agregar | — | «FA5 – Reserva vencida. Si pasan diez minutos sin pago aprobado, el sistema informa que venció la reserva y libera el cupo; el Cliente debe volver a armar la compra (RN-15).» |
| Postcondición 1 | «Existe un registro de Compra asociado al Cliente y al Paquete.» | «Existe un registro de Compra asociado al Cliente, con sus entradas, servicios y pagos.» |
| Postcondición 3 | «Se actualizan sus puntos de fidelidad y, si corresponde, su nivel.» | «Se actualiza su contador de compras y, si corresponde, su nivel.» |

> FA4 describe lo que hace el negocio. Hoy la pantalla falla en ese camino (K-2): si Codex no
> lo corrige antes de la entrega, FA4 igual es correcto, y el fallo queda en «Limitaciones».

**E-10 · CU-02, pp. 74 y 75 · R (T-7).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «Permite al Administrador crear, modificar y desactivar eventos de la plataforma, controlando la información básica (nombre, fecha, ubicación, precio, cupos).» | «Permite al Administrador crear, suspender y eliminar eventos de sus locales, y administrar sus sectores (nombre, precio y capacidad) y su período de preventa.» |
| Paso 1 | «El Administrador accede al módulo “Gestionar eventos”.» | «El Administrador ingresa a «Mis locales» y elige «Ver eventos» de un local.» |
| Paso 3, viñeta 1 | «Crear un nuevo evento (título, fecha, ubicación, cupo, precios).» | «Crear un nuevo evento (nombre, fecha, categoría y preventa). Queda en borrador hasta tener un sector con capacidad.» |
| Paso 3, viñeta 2 | «Modificar datos de un evento existente.» | «Agregar o quitar sectores, con su precio y capacidad, y cambiar la preventa.» |
| Paso 3, viñeta 3 | «Desactivar o eliminar un evento (si aún no tiene ventas asociadas, según regla).» | «Eliminar un evento sin ventas, o suspenderlo.» |
| Paso 5 | «(alta, modificación o baja lógica)» | «(alta, cambio de sectores o de preventa, suspensión o eliminación)» |
| FA2 | «ofrece marcarlo como “No disponible”.» | «ofrece suspenderlo.» |
| Después de FA2, agregar | — | «FA3 – Sectores. Para agregar o quitar sectores del evento, el Administrador elige «Administrar evento» (CU-11, que extiende este caso de uso).» |
| Postcondición 1 | «La tabla de Evento queda actualizada con las altas, modificaciones o bajas lógicas.» | «Los eventos y sus sectores quedan actualizados.» |
| Postcondición 2 | «Solo se mostrarán a los Clientes los eventos que estén activos y con cupo.» | «Solo se muestran a los Clientes los eventos publicados con fecha futura; los agotados aparecen con la opción de lista de espera.» |

**E-11 · CU-03, p. 76 · R.**

| Lugar | Dice | Poner |
|---|---|---|
| Paso 1 | «El Cliente ingresa a su perfil y selecciona “Ver historial de compras”.» | «El Cliente selecciona «Mis eventos» en su menú.» |
| Paso 2 | «(evento, fecha, monto, estado)» | «(pedido, evento, fecha, estado, cantidad de entradas, total pagado y monto devuelto)» |
| Paso 3 | «El Cliente selecciona una compra para ver el detalle del paquete (entradas, servicios adicionales, descuentos).» | «El Cliente selecciona una compra para ver sus entradas: sector, precio, estado, seguro y código. Si alguna tiene seguro, puede elegirla para devolverla.» |
| FA1 | «“No se registran compras realizadas”» | ««Todavía no tenés compras.»» |
| Postcondición | «No se modifica información; solo se consulta.» | «Sin devolución, no se modifica información; solo se consulta.» |

**E-12 · CU-04, pp. 77 y 78 · R.**

| Lugar | Dice | Poner |
|---|---|---|
| Paso 4 | «El sistema crea el Cliente con nivel inicial (ej. Bronce).» | «El sistema crea el Cliente con nivel Bronce y sin suscripción (RN-03).» |
| FA1, punto 1 | «En el paso 4, si el correo ya existe, el sistema informa: “El correo ingresado ya se encuentra registrado”.» | «En el paso 3, si el correo ya existe, el sistema informa: «Ya existe una cuenta registrada con ese mail.»» |
| FA1, punto 2 | «Ofrece al usuario ir a “Iniciar sesión” o recuperar contraseña (si se implementa).» | «El usuario puede volver a «Iniciar sesión» con «Ya tengo cuenta».» |
| Postcondición 1 | «Existe un nuevo registro en la tabla CLIENTE con un id_cliente único.» | «Existe un nuevo usuario con perfil Cliente, identificado por su correo.» |

**E-13 · CU-05, pp. 79 y 80 · R (T-8).**

| Lugar | Dice | Poner |
|---|---|---|
| FA1, punto 1 | «En el paso 5, si el correo no existe o la contraseña es inválida, el sistema muestra un mensaje: “Usuario o contraseña incorrectos”.» | «En el paso 2, si el correo no existe, la contraseña es inválida o la cuenta está bloqueada, el sistema muestra el mismo mensaje: «Mail o contraseña incorrectos.»» (K-3 corregido en `2af267a`, 14/11) |
| Título de FA2 | «FA2 – Usuario bloqueado (opcional)» | «FA2 – Usuario bloqueado» |
| FA2, punto 1 | «Si se detectan múltiples intentos fallidos, el sistema puede marcar el usuario como bloqueado temporalmente.» | «Tras cinco intentos fallidos consecutivos, el sistema bloquea la cuenta por quince minutos (RN-04).» |
| FA2, punto 2 | «Se indica al usuario que contacte soporte o espere un tiempo.» | «El sistema no revela el bloqueo: muestra el mismo mensaje que en FA1. Pasados los quince minutos, el usuario puede volver a intentar.» |

> Desde `2af267a` (14/11) el inicio de sesión no revela si el correo existe, cuántos intentos
> quedan ni si la cuenta está bloqueada (CP-114). Si D-6 queda en (c), CU-05 suma un paso al
> final: el aviso de la lista de espera.

**E-14 · CU-06, pp. 80 a 82 · R (T-13).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «Permite contratar una suscripción (mensual/anual) que otorga beneficios adicionales, usando un flujo de pago simulado.» | «Permite contratar una suscripción mensual o anual, que otorga beneficios en especie, y cancelarla. El cobro del plan queda fuera del alcance de esta versión (RF-13).» |
| Actores secundarios | «Pasarela de pago simulada» | «-» |
| Precondición 2 | «El Cliente no tiene una suscripción activa (o está por vencer según la regla que definas).» | «El Cliente no tiene una suscripción activa.» |
| Precondición 3 | «La pasarela de pago está disponible en modo simulación.» | Borrar |
| Paso 1 | «El Cliente entra a su perfil y selecciona “Suscribirse”.» | «El Cliente selecciona «Mi suscripción» en su menú.» |
| Paso 5 | «El Cliente confirma e ingresa datos de pago (simulado).» | «El Cliente confirma.» |
| Paso 6 | «El sistema envía operación a la pasarela simulada.» | Borrar y renumerar |
| Paso 7 | «Si se aprueba, registra la Suscripción (fecha inicio/fin) y marca cliente como suscripto.» | «El sistema registra la Suscripción con su plan, precio y fechas de inicio y fin.» |
| FA1 completo | «FA1 – Pago rechazado» y sus dos puntos | «FA1 – Cancelar suscripción. Con una suscripción activa, el Cliente puede cancelarla. El sistema pide confirmación («Los beneficios se pierden desde ahora») y la da de baja, sin reintegro (RN-26).» |
| Postcondición 1 | «Si el pago fue aprobado, el Cliente queda con un registro de Suscripción activo asociado a su cuenta.» | «El Cliente queda con un registro de Suscripción activo asociado a su cuenta.» |
| Postcondición 2 | «(descuentos, seguro, prioridad)» | «(seguro sin cargo, preventa y prioridad en la lista de espera), sin descuento porcentual (RN-37)» |

**E-15 · CU-07, p. 83 · R (T-9).** El paso 4 lo cubre el ítem 11 de CORRECCIONES.

| Lugar | Dice | Poner |
|---|---|---|
| Paso 5 | «El Administrador puede exportar o registrar esta información.» | «El Administrador puede elegir otro evento o volver al menú.» |
| FA1 | «Si el Administrador selecciona un evento sin usuarios en lista, el sistema muestra el mensaje: “Este evento no tiene lista de espera asociada”.» | «Si ningún evento de sus locales está agotado ni tiene lista, el sistema muestra: «No hay eventos agotados ni listas de espera en tus locales.»» |
| Debajo de las postcondiciones, agregar | — | «La inscripción en la lista la realiza el Cliente desde la compra (CU-01, FA3).» |

**E-16 · CU-08, pp. 84 y 85 · R (T-10).**

| Lugar | Dice | Poner |
|---|---|---|
| Descripción | «visualice reportes simulados con información agregada» | «visualice reportes con información agregada, calculados sobre los datos registrados (en esta práctica, un juego de datos ficticio),» |
| Paso 2, agregar una viñeta | — | «Demanda insatisfecha (listas de espera).» |
| Paso 4 | «El sistema genera el reporte con datos simulados y lo muestra en forma de tabla y/o gráficos.» | «El sistema genera el reporte y lo muestra en una tabla, con una lectura del resultado.» |
| Paso 5 | «(rango de fechas, categoría de evento)» | «(rango de fechas y, según el reporte, categoría, evento o solo eventos agotados)» |
| Paso 6 | «Opcionalmente, el Administrador puede exportar el reporte (por ejemplo, a PDF o imagen, aunque sea conceptual en esta práctica).» | «Opcionalmente, el Administrador exporta el reporte a PDF.» |
| FA1, punto 1 | «el sistema muestra un mensaje (“No se encontraron registros para el período seleccionado”).» | «el sistema lo informa en la lectura del reporte, por ejemplo: «No hay eventos en el período elegido.»» |
| Debajo de FA1 | Un «2.» suelto, sin texto | Borrarlo |

### Diagramas (pp. 71 y 86 a 89)

Los cubre `docs/PARA-MAXIMO-DIAGRAMAS.md`, con fuente, imagen, ancho y texto de cada uno. Los
diagramas chicos de cada especificación (pp. 72 a 85) quedan sin rehacer; ver D-4.

## 6b. Las demás cifras del documento, página por página

Las del resumen y la matriz están en la sección 5. «Sin cambio» quiere decir que se verificó
contra el código, la base o una cuenta.

| Pág. | Cifra en el documento | Debería decir | Clase / origen |
|---|---|---|---|
| 1 y pie de todas | «2025»; comisión, turno y legajos en «completar» | Datos de JC | D (ítem 1 de CORRECCIONES) |
| 2 a 5 | Números de página del índice | Se regeneran al final | N (ítem 22 de CORRECCIONES) |
| 6 | Versiones 0.1 a 2.4 y fechas | Sin cambio (historia); se agregan las filas 2.5 | Ítem 2 de CORRECCIONES, con 41 reglas y CP-11 a CP-112 |
| 9 | «entre junio y diciembre de 2025» | Según el año que decida JC | D (dato de JC n.º 2 de CORRECCIONES) |
| 9 | «100 % del flujo principal» | Sin cambio | El flujo completo existe |
| 10 | OE-3: «al menos cinco casos de prueba» | Sin cambio: hay 112 | — |
| 11 | OE-6: «al menos tres ejemplos de reporte» | Sin cambio: hay cinco | — |
| 15 a 17 | «Fecha estimada: x», nueve veces | Datos de JC | D (CORRECCIONES, ítem 24) |
| 18 y 24 | «5 minutos» | Sin cambio | RNF-01 |
| 20 | Criterio 9: «al menos dos recomendaciones» | Ver B-07 | R |
| 25 | «once requerimientos funcionales (RF-01 a RF-11)» | Ver A-01 | R |
| 25 y 26 | 8 caracteres; 5 intentos; 15 minutos; 6 entradas; 10 minutos; 12 meses; 0–2, 3–6 y 7+; 0 %, 5 % y 7,5 % | Sin cambio | `Parametros`, `02-semilla.sql` |
| 26 | «el primer día de cada mes» (RN-23) | Ver A-04 | R |
| 27 | «20 %» (RN-27), «cuarenta y ocho horas», «cuatro últimos dígitos» | Sin cambio | RN-27 histórica; `SEGURO_HORAS_COBERTURA`; `CK_Pagos_Ultimos4` |
| 28 | P «$19.356» | «$19.355» | R (sección 5) |
| 28 | c 15 %, g 4,10 % (3,39 % + IVA), q $150 y la tabla de contribución ($1.840, $773, $239, $0, −$294, −$828, −$1.895, −$2.429) | Sin cambio | Recalculado con P = 19.355: da lo mismo. $1.840 y $239 están truncados (1.840,7 y 239,7); no cambia ninguna conclusión |
| 29 | 8,62 %; 10 %; 7,5 %; 1,1 puntos | Sin cambio | Recalculado |
| 29 y 30 | Tablas anuales, $3.758.161, $4.827, 779, 27,8 %, ventanas | Fuera de alcance | Juego de datos simulado |
| 30 | «$6.066.000» | Sin cambio | 1.476.000 + 4.590.000, los planes del anexo A02 |
| 32 | 0 a 2, 3 a 6, 7 o más; 0 %, 5 % y 7,5 %; 8,62 %; 10 % | Sin cambio; la columna de beneficios, ver A-09 | — |
| 33 | «10 minutos» (tabla de RN-15) | Sin cambio | `RESERVA_MINUTOS` |
| 33 | Tabla de parámetros | Ver A-10 | R |
| 33 | «tres pares de reglas» | Ver A-11 | R |
| 35 | «78 puntos, es decir 312 horas» | Sin cambio (78 × 4) | Rótulo «estimado»: ítem 24 de CORRECCIONES |
| 35 | «once requerimientos», «nueve de los once», «85 %» | Ver B-04 | R+N |
| 36 | 2, 3, 5, 5, 13, 8, 5, 5, 8, 8, 3, 8, 5; 49, 18, 11 y 78; 62,8 %, 23,1 % y 14,1 %; «casi tres puntos»; «once puntos» | Sin cambio | Sumas verificadas |
| 37 | W-01 a W-06 | W-06, ver B-05 | R |
| 38 | Fase 4: 41; Fase 6: 18; Fase 7: 8; «41 de los 78», «52,6 %» | Sin cambio | Sumas verificadas |
| 38 | Fase 8: «11» | «3» | Ver B-06 |
| 50 a 52 | «ocho pasos», «hasta cinco opcionales», «De 1 a 6» | Sin cambio | — |
| 51 | «De 0 a 3 artículos» | «0 o 1» | Ver E-03 |
| 54 | 2 × $25.000; $50.000; −$2.500; $47.500; $45.000; $7.125; $99.625; 4 compras | Sin cambio | CP-11 y la semilla |
| 56 | 45, 15, 40, 60, 20, 30, 60 y 10 s; total 4 min 40 s; «veinte segundos» | Sin cambio | Suma: 280 s |
| 63 | `CHECK` con 3 estados; `DECIMAL(4,3)`; 12 meses; umbrales 7 y 3 | Estados: 4; ver Anexo 2 | D-2 |
| 64 | «0,15» (cargo) | Sin cambio | `CARGO_SERVICIO` |
| 65 | `0.075m`, `0.050m` escritos en el código | Ver Anexo 2 | D-2 |
| 67 | $99.625; $50.000; $36.000; $4.085; $300; $9.240; $2.085; $7.155 | Sin cambio | Recalculado |
| 68 | CP-01 a CP-10: 2, 3, 6 y 7 compras; $40.000, $30.000 y $3.000; 13 meses | Sin cambio | `PruebasFidelizacion.cs` |
| 68 | «diez casos», «al menos cinco», «lo duplica» | Sin cambio: se refiere a esta sección | — |
| 70 | D-1 a D-7, «Cumple» | Sin cambio | CP-01 a CP-10 pasan en la última batería |
| 72 a 85 | Números de paso en FA de CU-04 y CU-05 | Ver E-12 y E-13 | R |
| 110 | «$1.840», «9,5 %» | Sin cambio | Recalculado con $19.355 |
| 111 | «T01 a T08» | Sin cambio | — |
| 112 | iText 9.2.0, Microsoft.Data.SqlClient 6.0.2, QRCoder 1.6.0 | Sin cambio | Los `.csproj` |

## 7. Fuera de alcance

Por decisión de JC: anexos (A02, pp. 98 a 105), informe de usabilidad (pp. 90 a 97), minutas y
cifras del juego de datos simulado (pp. 29 y 30).

## 8. El código está mal (para Codex, aparte)

| # | Qué | Dónde |
|---|---|---|
| K-1 | La interfaz promete notificar cuando se libera un lugar, y no existe notificación (W-03) | `FrmCompraCliente.cs:243` y `lblAgotado`; `GestorListaEspera.YaInscripto` |
| K-2 | CP-81 falla: la preventa rechazada deja visible la compra. **`2af267a` dice corregir la preventa:** a confirmar con la próxima batería | Reporte T-47/T-48 |
| K-8 | **Nuevo, 14/11.** El cambio de contraseña todavía revela una cuenta bloqueada: con el correo de una cuenta bloqueada responde «La cuenta está bloqueada…», y con un correo inexistente, «Mail o contraseña actual incorrectos.». El inicio de sesión ya no lo revela (K-3), así que el cambio de contraseña queda como la única forma de saber si un correo existe. CU-09 FA3 describe el comportamiento de hoy. Ojo: CP-96 exige hoy la palabra «bloqueada», así que corregirlo cambia también esa prueba. **Decide JC** | `GestorUsuarios.CambiarContrasena` (pasa `ocultarCuenta`, pero `CuentaBloqueada` se lanza antes) |
| K-3 | **Corregido por Codex en `2af267a` (14/11):** mensaje único, CP-114. Antes: el inicio de sesión revela si un correo existe. Con un correo inexistente dice «Mail o contraseña incorrecta»; con uno existente informa los intentos que quedan. El cambio de contraseña ya es uniforme (CP-97) | `frmInicio.cs` y `GestorUsuarios.AutenticarCuenta` |
| K-4 | `DESCUENTO_MAX_ADMISIBLE` se guarda, pero ninguna operación lo controla. **Corregido por Codex en `2af267a` (14/11)** con `DescuentoAdmisible`; falta registrar su caso de prueba | `GestorFidelizacion.PorcentajePara` |
| K-7 | El control de ingreso lista los eventos publicados de hoy **de todos los locales**, no solo los del Administrador que lo abre. Además, un evento que empieza cerca de la medianoche no aparece el día anterior, aunque su horario de ingreso ya haya empezado. No contradice ninguna regla escrita (RN-42 habla del perfil, no del dueño), pero CU-07 sí filtra «en tus locales». **Decide JC** si es un defecto | `GestorIngresos.EventosDeHoy` y `RepositorioEventos.DelDia` |
| K-5 | Texto visible «Registar local», y un botón «Registrarse» en el alta de local | `frmRegistrarLocal.Designer.cs` |
| K-6 | R-01: `GestorSesion.Iniciar` era público. **Corregido por Codex en `2af267a` (14/11):** `internal`, con sesión inmutable y CP-113 | `docs/ENCARGO-REVISION-SEGURIDAD.md` |

## Anexo 1 — Texto para D-1 (a)

Reemplaza las páginas 57 a 61 completas, desde el título «Implementación» hasta el final de
`class GestorReportes`. El título queda: **«Arquitectura e implementación»**.

> El sistema es una aplicación de escritorio en C# sobre .NET Framework 4.8, con SQL Server como
> base de datos. Se organiza en cuatro capas, con dependencias en un solo sentido:
>
> | Capa | Proyecto | Responsabilidad |
> |---|---|---|
> | Presentación | Visual (Windows Forms) | Veinte pantallas. No abre conexiones ni contiene SQL: solo invoca a la capa de negocio. |
> | Negocio | Repositorio (namespace Negocio) | Gestores que aplican las reglas de negocio. No referencia la interfaz: informa los rechazos con excepciones de dominio, que identifican la regla (RN-xx). |
> | Datos | Datos | Repositorios: único lugar con conexiones y SQL, siempre con consultas parametrizadas. |
> | Entidades | Entidades | Clases de dominio sin lógica, compartidas por las tres capas. |
>
> **Patrones de diseño aplicados.**
> - **Singleton**, en veintiocho clases: doce gestores de negocio, trece repositorios, la
>   conexión, la pasarela simulada y el generador de códigos QR. Cada una expone una única
>   instancia (`Instance`) con constructor privado.
> - **Repository:** las dieciséis tablas se acceden a través de trece repositorios, el único
>   lugar con SQL; cada uno hereda de `RepositorioBase` la apertura de la conexión y la creación
>   de comandos parametrizados.
> - **Strategy**, en la exportación de reportes: el negocio conoce solo la interfaz
>   `IExportador`, y la pantalla de reportes elige la implementación (`ExportadorPDF`, con iText).
> - **Herencia en los reportes:** una clase abstracta `Reporte` con cinco especializaciones, una
>   por reporte, creadas por `GestorReportes`.
>
> **Entidades y tablas.** El modelo persistido tiene dieciséis tablas:
> - Usuario, con sus perfiles Cliente y Propietario (el Administrador);
> - Locales, Eventos y Sectores;
> - Compras, Entradas y Pagos;
> - Servicios, ServiciosEvento y CompraServicios;
> - Seguros, Suscripciones y ListaEspera;
> - Parametros, que guarda los valores configurables de las reglas.
>
> El paquete no se persiste como entidad propia: se arma en memoria y, al confirmarse, se
> registra como una compra con sus entradas y sus servicios. Los diagramas de clases y el
> diagrama entidad-relación de este documento muestran cada entidad con sus atributos y
> relaciones.

*Razón:* código. Las 4 capas son los proyectos de la solución. Los 12 gestores Singleton son los
14 `Gestor*` menos `GestorClientes` y `GestorPropietario`. Hay 13 repositorios en `Datos/`. Las
16 tablas salen de `BaseDatos/01-esquema.sql`.

## Anexo 2 — Textos para D-2

**p. 63, «Definición de las estructuras».** Reemplazar el bloque `CREATE TABLE Compras` y las
cuatro líneas `ALTER TABLE` que lo siguen **por**:

```sql
CREATE TABLE Compras (
    ID                INT IDENTITY(1,1) PRIMARY KEY,
    ID_Usuario        INT           NOT NULL,
    FechaCompra       DATETIME      NOT NULL,
    Estado            VARCHAR(12)   NOT NULL,   -- Pendiente|Confirmada|Reembolsada|Vencida
    NivelAplicado     VARCHAR(8)    NOT NULL,   -- congelado, RN-22
    DescuentoAplicado DECIMAL(4,3)  NOT NULL,   -- congelado, RN-16
    BaseEntradas      DECIMAL(12,2) NOT NULL,
    MontoDescuento    DECIMAL(12,2) NOT NULL,
    MontoServicios    DECIMAL(12,2) NOT NULL,
    CargoServicio     DECIMAL(12,2) NOT NULL,
    MontoSeguro       DECIMAL(12,2) NOT NULL DEFAULT 0,   -- RN-28
    ConSeguro         BIT           NOT NULL DEFAULT 0,
    TotalPagado       DECIMAL(12,2) NOT NULL,
    MontoReembolsado  DECIMAL(12,2) NOT NULL DEFAULT 0,   -- RN-29
    ReservaVence      DATETIME      NULL,                 -- RN-15
    CONSTRAINT FK_Compras_Usuario FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID),
    CONSTRAINT CK_Compras_Estado
        CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida'))
);
ALTER TABLE Entradas ADD ID_Compra INT NULL;
ALTER TABLE Entradas ADD CONSTRAINT FK_Entradas_Compras
      FOREIGN KEY (ID_Compra) REFERENCES Compras(ID);
ALTER TABLE Usuario  ADD Nivel VARCHAR(8) NOT NULL DEFAULT 'Bronce';
ALTER TABLE Usuario  ADD FechaUltimaEvaluacion DATE NULL;
```

El índice `IX_Compras_Usuario_Fecha` que sigue no cambia.

**p. 64, «Algoritmo de cálculo del descuento».** En el pseudocódigo:
- reemplazar la línea `total ← subtotalE + servicios + cargo` **por** estas tres:

```
    seguro     ← cantidad de entradas × precio del seguro, si se contrata   // RN-28
    seguro     ← 0 si el cliente tiene suscripción activa                   // RN-37
    total      ← subtotalE + servicios + cargo + seguro
```

- en `DEVOLVER ResumenDescuento(…)`, agregar `seguro` antes de `total`.

**En el párrafo «Sobre la suscripción»**, reemplazar «seguro bonificado, preventa y prioridad—
se resuelven fuera de este cálculo y no alteran el importe de la entrada.» **por** «preventa y
prioridad— se resuelven fuera de este cálculo, y el seguro bonificado lleva su prima a cero sin
alterar el importe de la entrada.»

**p. 64, «Diseño de clases».**

| Dice | Poner |
|---|---|
| «con el gestor de negocio implementado como singleton, del mismo modo que GestorClientes.» | «con el gestor de negocio implementado como singleton, del mismo modo que los demás gestores que aplican reglas de negocio.» |
| `enum EstadoCompra   { Pendiente, Confirmada, Reembolsada }` | `enum EstadoCompra   { Pendiente, Confirmada, Reembolsada, Vencida }` |
| `decimal        BaseEntradas, MontoDescuento, TotalPagado;` | `decimal        BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, MontoSeguro, TotalPagado, MontoReembolsado;` |
| En `ResumenDescuento`: `decimal        SubtotalEntradas, Servicios, CargoServicio, Total;` | `decimal        SubtotalEntradas, Servicios, CargoServicio, Seguro, Total;` |
| En `GestorFidelizacion`, después de `ReevaluarNivel` | Agregar la línea `void             AscenderSiCorresponde(Cliente cliente, DateTime fecha) // RN-22` |

**p. 65, `PorcentajePara`.** Reemplazar el método completo. **Hay dos versiones:** Codex le agregó
el tope `DESCUENTO_MAX_ADMISIBLE` (K-4) en el commit `2af267a` del 14/11, así que **vale la
primera**. La segunda queda solo por si ese cambio se revirtiera.

**Si el tope ya está commiteado, poner:**

```csharp
public decimal PorcentajePara(NivelFidelidad nivel)
{
    switch (nivel)                                   // RN-20, valores en Parametros
    {
        case NivelFidelidad.Oro:
            return DescuentoAdmisible(GestorParametros.Instance.ObtenerDecimal("DESCUENTO_ORO"));
        case NivelFidelidad.Plata:
            return DescuentoAdmisible(GestorParametros.Instance.ObtenerDecimal("DESCUENTO_PLATA"));
        default:
            return 0m;                               // Bronce no tiene descuento
    }
}
private static decimal DescuentoAdmisible(decimal porcentaje)
{
    if (porcentaje > GestorParametros.Instance.ObtenerDecimal("DESCUENTO_MAX_ADMISIBLE"))
        throw new ReglaNegocioException("RN-20", "El descuento configurado supera el máximo admisible.");
    return porcentaje;
}
```

**Si todavía no está, poner:**

```csharp
public decimal PorcentajePara(NivelFidelidad nivel)
{
    switch (nivel)                                   // RN-20, valores en Parametros
    {
        case NivelFidelidad.Oro:
            return GestorParametros.Instance.ObtenerDecimal("DESCUENTO_ORO");
        case NivelFidelidad.Plata:
            return GestorParametros.Instance.ObtenerDecimal("DESCUENTO_PLATA");
        default:
            return 0m;                               // Bronce no tiene descuento
    }
}
```

**p. 65, `DeterminarNivel`.** Reemplazar la línea `DateTime desde = fechaReferencia.AddMonths(-12);
// RN-19` **por** estas dos:

```csharp
    int meses = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_VENTANA_MESES");   // RN-19
    DateTime desde = fechaReferencia.AddMonths(-meses);
```

Y las líneas `if (n >= 7) …` y `if (n >= 3) …` **por:**

```csharp
    if (n >= GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_ORO"))   return NivelFidelidad.Oro;
    if (n >= GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_PLATA")) return NivelFidelidad.Plata;
```

**p. 66, `CalcularDescuento`.**
- **Agregar** como primera línea del método, antes del comentario «// Base: solo entradas»:

```csharp
    GestorSesion.Instance.RequerirUsuario(cliente.Id);   // RN-41: solo el titular o el Administrador
```

- **Reemplazar** la línea del cargo **por:**

```csharp
    decimal cargo      = Math.Round(subtotalE * GestorParametros.Instance.ObtenerDecimal("CARGO_SERVICIO"), 2);
```

- **Agregar**, antes del `return`:

```csharp
    decimal seguro = paquete.ConSeguro
        ? GestorPaquetes.Instance.CantidadEntradas(paquete)
          * GestorParametros.Instance.ObtenerDecimal("SEGURO_PRECIO") : 0m;
    if (seguro > 0m && GestorSuscripciones.Instance.TieneActiva(cliente.Id))      // RN-37
        seguro = Math.Round(seguro * (1m - GestorParametros.Instance
                     .ObtenerDecimal("SUSCRIPCION_SEGURO_DESCUENTO")), 2);
```

- **Dentro del `return`,** agregar `Seguro = seguro,` y cambiar `Total = subtotalE + servicios +
  cargo` por `Total = subtotalE + servicios + cargo + seguro`.

**p. 67, diagrama de secuencia en texto.**

| Dice | Poner |
|---|---|
| `FrmCompraCliente ──► GestorCompras       : RegistrarCompra(resumen)` | `FrmCompraCliente ──► GestorCompras       : Iniciar(paquete, cliente)   // reserva 10 min` |
| `GestorCompras ──► PasarelaSimulada       : Procesar(total)` | `frmPago ──► GestorCompras                : Confirmar(compra, tarjeta)` y debajo `GestorCompras ──► PasarelaSimulada : Cobrar(tarjeta, total)` |
| `GestorFidelizacion ──► RepositorioFidelizacion : ActualizarNivel(…)` | Agregar antes: `GestorCompras ──► GestorFidelizacion : ReevaluarNivel(…) y AscenderSiCorresponde(…)` |

**pp. 68 a 70, código de las pruebas (opción b de D-2).** Debajo del título «Implementación de
las pruebas», agregar:

> El código vigente está en `Pruebas/PruebasFidelizacion.cs` y se ejecuta en cada corrida del
> conjunto de pruebas. El listado que sigue muestra su estructura. En el código, los métodos se
> llaman `CP01a04_NivelYDescuentoSegunCompras`, `CP05_ElDescuentoNoAlcanzaALosServicios`,
> `CP06_LaCompraDeHace13MesesQuedaFueraDeLaVentana`, `CP07_SeFacturaElPrecioCongelado`,
> `CP08_LaSeptimaSeCobraAl5YOroRigeDesdeLaOctava`, `CP09_ReembolsoBajaElContadorPeroNoElNivel` y
> `CP10_LaSuscripcionNoSumaPorcentaje`, y preparan los datos sobre una base de prueba que se
> recrea en cada ejecución.

## Anexo 3 — Especificaciones CU-09 a CU-13

**Resuelve D-4.** Las cinco se escribieron el 14/11 leyendo el código, no el diseño previo. Los
mensajes van entre comillas tal como aparecen en pantalla.

**Dónde van:** después de CU-08 (p. 85) y antes de los diagramas (p. 86), con el mismo formato
que CU-01 a CU-08. No llevan diagrama chico propio: los cinco están en el diagrama general de
casos de uso. Si Máximo quiere agregarles uno, tiene que respetar los errores 8 y 16.

**Provisorio:** CU-09 cambia si Codex corrige K-8 (FA3 pasaría a mostrar el mismo mensaje que
FA2), y CU-13 cambia si D-6 queda en (c). Si cambia algún mensaje, se ajusta la frase entre
comillas, no el flujo. Los rechazos por regla se muestran con el código al final, por ejemplo
«Mail o contraseña actual incorrectos. (RN-04)»; las especificaciones citan el texto sin el
código, igual que CU-01 a CU-08.

---

**CU-09 – Cambiar contraseña**

**Id y nombre:** CU-09 – Cambiar contraseña

**Descripción:** Permite a un usuario registrado, Cliente o Administrador, reemplazar su
contraseña. Para hacerlo debe conocer la contraseña actual.

**Actor:** Cliente o Administrador

**Actores secundarios:** -

**Precondiciones:**
- El usuario tiene una cuenta registrada.
- No necesita haber iniciado sesión: la opción está en la pantalla de inicio.

**Flujo principal:**
1. El usuario elige «Cambiar contraseña» en la pantalla de inicio de sesión.
2. El sistema muestra cuatro campos: mail, contraseña actual, nueva contraseña y confirmación.
   Las contraseñas se muestran enmascaradas, salvo que el usuario marque «Mostrar contraseñas».
3. El usuario completa los campos y presiona «Confirmar».
4. El sistema verifica que los cuatro campos estén completos y que la nueva contraseña coincida
   con su confirmación.
5. El sistema verifica el mail y la contraseña actual (RN-40).
6. El sistema verifica que la nueva contraseña cumpla RN-02.
7. El sistema guarda la nueva contraseña cifrada, vacía los campos de contraseña e informa «La
   contraseña se cambió correctamente.».

**Flujo alternativo:**

FA1 – Datos incompletos. En el paso 4, si falta un campo, el sistema pide completarlo, por
ejemplo «Ingresá la contraseña actual.». Si la confirmación no coincide, informa «Las contraseñas
no coinciden». El usuario vuelve al paso 3.

FA2 – Credenciales incorrectas. En el paso 5, si el mail no existe o la contraseña actual es
incorrecta, el sistema informa «Mail o contraseña actual incorrectos.», el mismo mensaje en los
dos casos. Una contraseña actual incorrecta cuenta como intento fallido (RN-04).

FA3 – Cuenta bloqueada. Si la cuenta está bloqueada, o si el intento fallido completa cinco
consecutivos, el sistema rechaza el cambio e informa cuántos minutos faltan para volver a
intentar (RN-04).

FA4 – Contraseña débil. En el paso 6, si la nueva contraseña no tiene al menos ocho caracteres,
una letra y un dígito, el sistema informa «La contraseña debe tener al menos 8 caracteres, con al
menos una letra y un dígito.» y no la guarda. Como la contraseña actual era correcta, no se
cuenta un intento fallido.

**Postcondiciones:**
- La contraseña anterior deja de servir y la nueva permite iniciar sesión.
- El contador de intentos fallidos de la cuenta vuelve a cero.
- No se inicia sesión: el usuario vuelve a la pantalla de inicio con «Volver».

---

**CU-10 – Registrar local**

**Id y nombre:** CU-10 – Registrar local

**Descripción:** Permite al Administrador dar de alta un local donde se realizarán sus eventos,
con su capacidad máxima.

**Actor:** Administrador

**Actores secundarios:** -

**Precondiciones:**
- El Administrador inició sesión.

**Flujo principal:**
1. El Administrador elige «Registrar local» en su menú.
2. El sistema muestra los campos nombre, dirección, altura y capacidad.
3. El Administrador completa los campos y presiona «Registrar local».
4. El sistema verifica que el local tenga nombre y que la capacidad sea un número entero no
   negativo. La dirección y la altura no se validan: se guardan juntas, como un único texto.
5. El sistema registra el local a nombre del Administrador, informa «Local registrado
   correctamente» y vuelve al menú.

**Flujo alternativo:**

FA1 – Capacidad inválida. En el paso 4, si la capacidad no es un número entero, el sistema
informa «La capacidad debe ser un número entero.»; si es negativa, «La capacidad del local no
puede ser negativa.». El Administrador vuelve al paso 3.

FA2 – Sin nombre. En el paso 4, si el nombre está vacío, el sistema informa «El local debe tener
nombre.» y el Administrador vuelve al paso 3.

FA3 – Volver sin registrar. El Administrador puede elegir «Volver» en el paso 3; el sistema
vuelve al menú sin registrar nada.

**Postcondiciones:**
- El local queda registrado y aparece en «Mis locales».
- Su capacidad limita la suma de los sectores de cada evento que se realice en él (RN-38).

---

**CU-11 – Gestionar sectores**

**Id y nombre:** CU-11 – Gestionar sectores

**Descripción:** Permite al Administrador agregar y quitar los sectores de un evento, cada uno
con nombre, precio y capacidad. Extiende CU-02 cuando el Administrador agrega o quita un sector.

**Actor:** Administrador

**Actores secundarios:** -

**Precondiciones:**
- El Administrador inició sesión.
- Eligió un evento de uno de sus locales (CU-02).

**Flujo principal:**
1. En «Eventos del local», el Administrador elige un evento y presiona «Administrar Evento».
2. El sistema muestra los sectores del evento, con su nombre, precio y capacidad, y la fecha de
   fin de la preventa.
3. El Administrador presiona «Agregar sector».
4. El sistema muestra los campos nombre, capacidad y precio.
5. El Administrador los completa y presiona «Agregar sector».
6. El sistema verifica los datos y que la suma de las capacidades de los sectores no supere la
   del local (RN-38).
7. El sistema registra el sector, informa «Sector agregado correctamente.» y vacía los campos
   para cargar otro. Si el evento estaba en borrador y el sector tiene capacidad, el evento queda
   publicado (RN-06).

**Flujo alternativo:**

FA1 – Quitar un sector. En el paso 3, el Administrador elige un sector de la lista y presiona
«Eliminar sector». El sistema lo elimina sin pedir confirmación y actualiza la lista. Si no
eligió ninguno, informa «Por favor, seleccioná un sector para eliminar.».

FA2 – Sector con ventas. En FA1, si el sector tiene entradas vendidas o devueltas, el sistema
informa «El sector tiene entradas vendidas: no se puede eliminar.».

FA3 – Último sector con capacidad. En FA1, si el evento está publicado y es su último sector con
capacidad, el sistema informa «No se puede eliminar el sector: es el último con capacidad mayor
que cero de un evento publicado.» (RN-06).

FA4 – Capacidad del local. En el paso 6, si la suma supera la capacidad del local, el sistema
informa la capacidad del local y la que queda disponible para el evento, y no registra el
sector (RN-38).

FA5 – Datos inválidos. En el paso 6, el sistema informa «Por favor, completá todos los
campos.», «La capacidad debe ser un número entero.», «El precio debe ser un número, por ejemplo
12500.50.» o «La capacidad y el precio del sector no pueden ser negativos.», según el caso. El
Administrador vuelve al paso 5.

**Postcondiciones:**
- Los sectores del evento quedan actualizados.
- La capacidad del evento es la suma de las de sus sectores (RN-07).
- Un evento publicado conserva al menos un sector con capacidad (RN-06).

---

**CU-12 – Controlar ingreso**

**Id y nombre:** CU-12 – Controlar ingreso

**Descripción:** Permite al Administrador validar en la puerta las entradas de un evento del
día, con el código QR del ticket leído por un lector USB o tipeado desde el PDF. Cada entrada
ingresa una sola vez (RN-18).

**Actor:** Administrador

**Actores secundarios:** -

**Precondiciones:**
- El Administrador inició sesión.
- Hay al menos un evento publicado con fecha de hoy.

**Flujo principal:**
1. El Administrador elige «Control de ingreso» en su menú.
2. El sistema muestra los eventos publicados con fecha de hoy, de todos los locales; el contador
   de ingresos registrados del evento elegido, y el mensaje «Escaneá una entrada.».
3. El Administrador elige el evento.
4. El Administrador lee el código del ticket con el lector, que lo confirma con Enter, o lo
   tipea y presiona «Registrar ingreso».
5. El sistema verifica que el código exista, que la entrada sea de ese evento, que esté
   confirmada y sin ingreso previo, y que la hora actual esté dentro del horario de ingreso:
   desde seis horas antes hasta doce horas después del inicio del evento.
6. El sistema registra la fecha y hora del ingreso, muestra «Ingreso válido. Bienvenido.» en
   verde, actualiza el contador y deja el campo vacío para la próxima entrada.

**Flujo alternativo:**

FA1 – Sin eventos hoy. En el paso 2, si no hay eventos publicados con fecha de hoy, el sistema
muestra «No hay eventos publicados para hoy.» y deshabilita el campo y el botón.

FA2 – Entrada rechazada. En el paso 5, si alguna verificación falla, el sistema no registra el
ingreso y muestra el motivo en rojo:
- «El código no existe.»
- «La entrada corresponde a otro evento.»
- «La entrada está reembolsada.»
- «La entrada está pendiente o vencida.»
- «La entrada ya ingresó el» seguido de la fecha y hora del primer ingreso.
- «Fuera del horario de ingreso o evento no disponible.»

FA3 – Lectura simultánea. Si dos puertas leen la misma entrada al mismo tiempo, solo una
registra el ingreso. La otra muestra que la entrada ya ingresó o, si la entrada cambió por otro
motivo, «La entrada cambió. Volvé a escanear.».

**Postcondiciones:**
- La entrada queda con su fecha de ingreso: no puede volver a ingresar ni devolverse (RN-29).
- El contador muestra los ingresos registrados del evento.

---

**CU-13 – Devolver entrada**

**Id y nombre:** CU-13 – Devolver entrada

**Descripción:** Permite al Cliente devolver una o más entradas aseguradas de una compra, hasta
cuarenta y ocho horas antes del evento, y recibir el reintegro que fija RN-29. Extiende CU-03
cuando la compra tiene entradas con seguro.

**Actor:** Cliente

**Actores secundarios:** -

**Precondiciones:**
- El Cliente inició sesión.
- Tiene una compra confirmada con al menos una entrada asegurada.

**Flujo principal:**
1. En «Mis eventos» (CU-03), el Cliente elige una compra.
2. El sistema muestra sus entradas. Solo las confirmadas y con seguro tienen habilitada la
   casilla «Devolver». Una ayuda indica el plazo y aclara que no se reintegran el cargo por
   servicio ni el seguro.
3. El Cliente marca las entradas que quiere devolver; el botón muestra «Devolver» con la
   cantidad elegida.
4. El Cliente presiona el botón.
5. El sistema pide confirmación, con «No» como respuesta predeterminada: «¿Devolver N
   entrada(s)? Se reintegra el precio de cada entrada con su descuento. Si devolvés todas las que
   quedan, también los servicios. El cargo por servicio y el seguro no se reintegran. La
   devolución no se puede deshacer.».
6. El Cliente confirma.
7. El sistema vuelve a verificar con los datos guardados: que la compra sea del Cliente y esté
   confirmada; que cada entrada esté confirmada, asegurada y sin ingreso registrado, y que
   falten más de cuarenta y ocho horas para el evento.
8. El sistema registra la devolución: las entradas quedan reembolsadas y su cupo vuelve a estar
   disponible. Si no queda ninguna entrada confirmada en la compra, la compra pasa a
   reembolsada.
9. El sistema informa «Devolución registrada. Se reintegran» y el importe, y actualiza el
   historial.

El importe es el precio de cada entrada devuelta menos su parte del descuento por nivel. Si con
esa devolución no queda ninguna entrada confirmada, se reintegran además los servicios.

**Flujo alternativo:**

FA1 – El Cliente no confirma. En el paso 6, si responde «No», no cambia nada.

FA2 – Fuera de plazo. En el paso 7, si faltan cuarenta y ocho horas o menos, el sistema informa
«El seguro permite devolver hasta 48 horas antes del evento (hasta el» y el límite, seguido de
«Ese plazo ya pasó.».

FA3 – Entrada usada. En el paso 7, si alguna entrada ya registró su ingreso, el sistema informa
«No podés devolver una entrada que ya se usó para ingresar.».

FA4 – La compra cambió. Si otra operación modificó la compra mientras tanto, el sistema informa
«La compra cambió mientras se procesaba la devolución. Volvé a abrir el historial.» y recarga el
historial.

**Postcondiciones:**
- Las entradas devueltas quedan reembolsadas y su cupo se libera para otra compra. Nadie de la
  lista de espera recibe aviso (W-03).
- Si la compra quedó reembolsada, deja de computar para el nivel (RN-24); el nivel vigente se
  conserva hasta la reevaluación mensual (RN-23).

---

**Lo que no se especifica y por qué.** El diagrama tiene también «Consultar mis locales»,
«Buscar eventos», «Aplicar descuento por nivel», «Pagar la compra», «Contratar seguro»,
«Inscribirse en lista de espera» y «Exportar reporte a PDF». Cada uno ya está como paso o como
flujo alternativo de una especificación: CU-02, paso 1; CU-01, pasos 2 a 8 y FA3; CU-08, paso 6.
«Consultar mis locales» es el único sin texto propio: es la lista desde la que arranca CU-02. Si
el docente pide una especificación por cada óvalo, son unas 3 vueltas más.

## Anexo 4 — Estimación de costos

**Resuelve I-4 del chequeo.** Va después de «Escala de esfuerzo» (p. 35), como sección nueva
«Estimación de costos».

**De dónde salen las horas.** Son las horas-persona de ingeniería que el documento ya estima en
la escala de esfuerzo: un punto vale cuatro horas, y la estimación aprobada es de 78 puntos (312
h). No son tiempos medidos de ejecución, y en particular no son el tiempo que tardaron los
agentes: una estimación de costos estima el esfuerzo del trabajo, no el de la herramienta que lo
ejecutó. La distribución por fase es la de la p. 38 con B-06 aplicado (Fase 8 = 3 puntos).

**El valor hora, en un solo lugar.** El supuesto está en la primera oración. Todos los importes
son horas × V. Si JC cambia V, cada importe se recalcula con una multiplicación, y no hay otro
número que tocar.

**Texto para pegar:**

> **Estimación de costos**
>
> **Supuesto: valor hora V = $30.500.** Fuente: Colegio de Ingenieros Especialistas de la
> Provincia de Santa Fe, Resolución N.º 123 (aprobada el 24/09/2026, vigente desde el
> 01/11/2026), Anexo I, «Tareas de sistemas de información — Desarrollo», rol 1.2 «Analista
> Programador», nivel Junior. Se adopta el nivel junior porque el equipo está formado por dos
> estudiantes. Todos los importes de esta sección resultan de multiplicar las horas por V: si V
> cambia, basta con repetir esa multiplicación.
>
> Las horas son horas-persona de trabajo de ingeniería y surgen de la escala de esfuerzo (un punto
> equivale a cuatro horas). Cubren el desarrollo que esa escala estima; no se agregan horas de
> relevamiento ni de documentación, que el proyecto no estimó por separado.
>
> | Fase | Requerimientos | Puntos | Horas | Costo (horas × V) |
> |---|---|---|---|---|
> | Fase 4 — Desarrollo del prototipo, 1ª iteración | RF-12, RF-01 a RF-05, RF-07 | 41 | 164 | $5.002.000 |
> | Fase 6 — Desarrollo ampliado, 2ª iteración | RF-06, RF-08, RF-13 | 18 | 72 | $2.196.000 |
> | Fase 7 — Generación de reportes | RF-11 | 8 | 32 | $976.000 |
> | Fase 8 — Ajustes finales | RF-10 | 3 | 12 | $366.000 |
> | **Alcance construido** | | **70** | **280** | **$8.540.000** |
> | RF-09, estimado y no construido (Won't have desde la versión 2.5) | RF-09 | 8 | 32 | $976.000 |
> | **Estimación aprobada** | | **78** | **312** | **$9.516.000** |
>
> **Otros costos.** No hay costo de licencias: las herramientas empleadas (Visual Studio, SQL
> Server Express y PlantUML) se usan en versiones gratuitas o provistas por la universidad. Los
> equipos son de los integrantes y no se imputan.
>
> **Sensibilidad al valor hora.**
>
> | Escenario | Valor hora | Alcance construido (280 h) | Estimación aprobada (312 h) |
> |---|---|---|---|
> | V − 25 % | $22.875 | $6.405.000 | $7.137.000 |
> | Supuesto | $30.500 | $8.540.000 | $9.516.000 |
> | V + 25 % | $38.125 | $10.675.000 | $11.895.000 |
>
> Una variación de ±25 % en el valor hora mueve el costo de la estimación aprobada entre
> $7.137.000 y $11.895.000. Como el costo es proporcional al valor hora, el resultado no depende
> de ningún otro supuesto.

**Cuentas verificadas:**
- 41 + 18 + 8 + 3 = 70 puntos, y 70 + 8 = 78.
- Horas: 164 + 72 + 32 + 12 = 280, y 280 + 32 = 312.
- Importes: 5.002.000 + 2.196.000 + 976.000 + 366.000 = 8.540.000, y 8.540.000 + 976.000 =
  9.516.000, igual a 312 × 30.500.

**Para JC, antes de pegar:**
- **V es un supuesto mío con fuente oficial, para que lo confirmes o lo cambies.** Leí la
  resolución en el PDF publicado por el colegio (`especialistas.org.ar/wp-content/uploads/RDP_123.pdf`).
- **Es la resolución vigente, verificada el 14/11.** El Colegio publicó tres en 2026: la 117
  (desde el 01/05), la 122 (desde el 01/08, Analista Programador Junior $29.500) y la **123
  (desde el 01/11, $30.500)**. Cada una deroga la anterior; abrí los tres PDF del sitio del
  colegio (`especialistas.org.ar`, «Resoluciones provinciales»). La 123 rige hoy.
- **Si sale una más nueva antes de la entrega** (el colegio actualiza cada tres meses: la
  próxima sería desde el 01/02/2027), se cambia V y las horas no se tocan.
- **El año del documento:** el pie dice 2025 y la tarifa es de 2026. Se resuelve con el dato de
  JC n.º 2 de CORRECCIONES (el año).
- **Otra opción de rol:** «Desarrollador de Aplicaciones», Junior, también vale $30.500 en la
  misma tabla, así que el número no cambia.
- **Si se quiere que Word recalcule solo:** marcar el número del supuesto con un marcador
  llamado `ValorHora`, y en cada celda de costo insertar un campo (Ctrl+F9) con
  `= 164 * ValorHora \# "$ #.##0"`, cambiando las horas por fila; F9 actualiza. Es opcional, y
  no lo probé en el Word de Máximo, así que conviene verificar el separador de miles. Sin campos,
  recalcular son seis multiplicaciones.

## Cómo se armó y qué no se revisó

- **Lectura:** las 112 páginas del PDF, extraídas como texto.
- **Contraste:** contra `Repo/`, `Datos/`, `Entidade/`, `Visual/` (las 20 pantallas),
  `BaseDatos/01-esquema.sql`, `02-semilla.sql` y `04-seguridad-ingreso.sql`, `Pruebas/` y el
  último reporte de Codex.
- **Lo que no se hizo:** no se corrió la batería, no se tocó la base y no se abrió ninguna
  ventana.
- **Las cifras de casos y ejecuciones** se contaron en el código fuente.
- **No revisado,** por estar fuera de alcance: anexos, usabilidad, minutas y juego de datos
  simulado.
