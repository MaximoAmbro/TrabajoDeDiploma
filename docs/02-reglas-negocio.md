# Reglas de negocio

Tipo: **E** estructural (restricción de datos) · **D** derivación (cálculo) · **A** acción (habilita o impide).

## Usuarios y cuentas

| ID | Regla | T |
|---|---|---|
| RN-01 | Un correo identifica unívocamente a un usuario: no pueden coexistir dos cuentas con el mismo correo. | E |
| RN-02 | La contraseña tiene 8 caracteres como mínimo, con al menos una letra y un dígito. | E |
| RN-03 | Toda cuenta nueva se crea con nivel Bronce y sin suscripción activa. | A |
| RN-04 | Tras 5 intentos fallidos consecutivos la cuenta se bloquea por 15 minutos. | A |
| RN-05 | Un usuario tiene un único perfil excluyente: Cliente, Organizador o Administrador de plataforma. El administrador de plataforma no es dueño de locales; el organizador administra solo los locales, eventos e ingresos de su cuenta. | E |

## Seguridad de cuentas y autorización

| ID | Regla | T |
|---|---|---|
| RN-40 | Para cambiar una contraseña se debe comprobar la contraseña actual. El rechazo no revela si el correo existe, si la contraseña es incorrecta o si la cuenta está bloqueada; los intentos y el bloqueo se aplican internamente. | A |
| RN-41 | Las operaciones sobre datos privados de una cuenta solo pueden devolverlos o modificarlos para el usuario autenticado titular de esa cuenta. La sesión conserva una identidad verificada y no acepta como autorización un objeto de usuario alterado por quien llama. | A |
| RN-42 | **DADA DE BAJA.** La autorización por perfil para gestión e ingreso fue reemplazada por RN-48; la autorización por pertenencia a los datos del Organizador, incluidos sectores, listas de espera e ingresos, está en RN-49. Los reportes se rigen por RN-34. El número no se reasigna. | — |
| RN-43 | Una cuenta de Cliente debe verificar el correo antes de iniciar una compra. El código de verificación se envía al correo registrado; la entrega usa la bandeja simulada del sistema. | A |
| RN-44 | Los códigos de verificación de correo, recuperación de contraseña y confirmación telefónica son de un solo uso, se almacenan protegidos, vencen según `CORREO_CODIGO_MIN` y admiten como máximo `CORREO_CODIGO_INTENTOS_MAX` intentos. La recuperación cambia la contraseña mediante las mismas validaciones y el mismo hash que el registro. | A |
| RN-45 | Las operaciones de inicio de sesión, cambio de contraseña, compras, devoluciones, control de ingreso y gestión de eventos se registran en `Auditoria` con el usuario, la acción, la entidad afectada, la fecha y el resultado. Solo el Administrador de plataforma puede consultar esos registros, con filtros por período y acción. | A |
| RN-46 | Una sesión sin actividad se cierra al cumplirse `SESION_INACTIVIDAD_MIN`. La aplicación avisa con `SESION_AVISO_SEG` segundos de anticipación; cualquier actividad reinicia el plazo. | A |
| RN-47 | Una contraseña nueva debe cumplir la longitud y composición mínima de RN-02 y alcanzar el puntaje configurado en `PWD_FORTALEZA_MIN`, calculado por la variedad de minúsculas, mayúsculas, números y símbolos. | A |

## Eventos, sectores y disponibilidad

| ID | Regla | T |
|---|---|---|
| RN-06 | Un evento publicado tiene **siempre** al menos un sector con capacidad mayor que cero. Es un invariante, no una verificación de un solo momento: impide publicar sin sectores y también impide eliminar el último sector de un evento ya publicado. | E |
| RN-07 | La capacidad total de un evento es la suma de las capacidades de sus sectores. | D |
| RN-08 | Las entradas vendidas de un sector nunca superan su capacidad. | E |
| RN-09 | Un evento con fecha pasada no admite ventas ni inscripciones en lista de espera. | A |
| RN-10 | Un evento está agotado cuando todos sus sectores alcanzaron su capacidad; ahí se habilita la lista de espera. | D |
| RN-11 | Un evento con entradas vendidas no se elimina: se suspende o se reprograma. | A |
| RN-38 | La suma de las capacidades de los sectores de un evento no puede superar la capacidad del local donde se realiza. Se verifica al agregar o modificar un sector. | E |
| RN-39 | **Preventa.** Mientras la fecha actual sea anterior a `Eventos.FinPreventa`, solo un cliente con suscripción activa puede iniciar una compra de ese evento. El evento igual se ve en el listado. `FinPreventa` en NULL significa que el evento no tiene preventa. Es el beneficio de preventa de RN-37. | A |

## Solicitudes de Organizador y revisión del primer evento

| ID | Regla | T |
|---|---|---|
| RN-50 | Solo una cuenta Cliente puede presentar una solicitud para ser Organizador y no puede mantener más de una solicitud pendiente. La solicitud pendiente no cambia el perfil ni concede permisos de Organizador. | A |
| RN-51 | Antes de presentar la solicitud, el responsable debe ingresar un teléfono argentino válido y confirmarlo mediante un código de un solo uso enviado por el mecanismo de códigos del sistema. | A |
| RN-52 | La solicitud valida el dígito verificador del CUIT y del CBU; también admite un alias bancario con el formato definido. La documentación debe ser una imagen PNG o JPEG válida, comprimida y de hasta 512 KB. La verificación externa de identidad es simulada y no sustituye la revisión del Administrador. | A |
| RN-53 | Solo el Administrador de plataforma puede aprobar o rechazar solicitudes de Organizador. Ambas decisiones requieren un motivo de hasta 200 caracteres y registran la identidad del revisor y la fecha; aprobar cambia el perfil a Organizador y crea el local solicitado. | A |
| RN-54 | La primera publicación de un Organizador se envía a revisión del Administrador y no aparece en el catálogo ni admite ventas mientras está en revisión. El Administrador puede aprobarla o rechazarla; para rechazar debe ingresar un motivo no vacío de hasta 500 caracteres. El rechazo devuelve el evento a Borrador y conserva el motivo para el Organizador. Si vuelve a publicarlo antes de que se haya aprobado un evento suyo, vuelve a revisión. Tras su primera aprobación, sus publicaciones siguientes son directas. | A |

## Compra y armado de paquetes

| ID | Regla | T |
|---|---|---|
| RN-12 | Máximo 6 entradas por operación y por evento, sumando todos los sectores elegidos. | A |
| RN-13 | Un paquete debe contener al menos una entrada; los servicios no se venden sueltos. | E |
| RN-14 | Un servicio complementario solo se incorpora si el evento lo tiene habilitado. | A |
| RN-15 | Al iniciar el pago las entradas quedan reservadas 10 minutos; vencido el plazo el cupo se libera. | A |
| RN-16 | El precio se congela al iniciar la compra. Un cambio de tarifa posterior no afecta la operación en curso. | A |
| RN-17 | Una compra se confirma solo si la pasarela devuelve estado aprobado. | A |
| RN-18 | Cada entrada vendida genera un QR único e irrepetible, válido para un único ingreso. | E |

## Fidelización y descuentos

| ID | Regla | T |
|---|---|---|
| RN-19 | El nivel se determina por compras confirmadas en **ventana móvil de 12 meses**: Bronce 0–2, Plata 3–6, Oro 7+. | D |
| RN-20 | Descuento por nivel: Bronce 0 %, Plata 5 %, Oro 7,5 %. | D |
| RN-21 | El descuento se aplica **solo sobre el valor de las entradas**. No alcanza a servicios ni al cargo por servicio. | D |
| RN-22 | El nivel se recalcula al confirmarse cada compra y rige **desde la operación siguiente**. | A |
| RN-23 | El nivel se reevalúa el primer día de cada mes. Entre reevaluaciones permanece estable. | A |
| RN-24 | Solo computan las compras confirmadas y no reembolsadas. | D |

## Suscripciones y seguros

| ID | Regla | T |
|---|---|---|
| RN-25 | Un cliente no mantiene dos suscripciones activas en simultáneo. | E |
| RN-26 | La suscripción no se renueva automáticamente. | A |
| RN-27 | **DADA DE BAJA.** Permitía acumular descuentos hasta 20 %. Se retiró por inviabilidad económica. El número no se reasigna. | — |
| RN-28 | El seguro se contrata solo durante la compra; no admite incorporación posterior. | A |
| RN-29 | El seguro habilita devolución hasta 48 horas antes del evento. La devolución es **por entrada**: el cliente elige cuáles da de baja, y cada una tiene que estar confirmada y tener póliza. **Se devuelve** el precio congelado de cada entrada menos su parte proporcional del descuento por nivel y, si con eso no queda ninguna entrada confirmada en la compra, también los servicios complementarios. **No se devuelven** el cargo por servicio ni la prima del seguro. El costo de los servicios devueltos se recupera del proveedor: esa condición tiene que constar en el acuerdo con cada proveedor. | A |
| RN-37 | La suscripción **no** otorga descuento porcentual y no se acumula con el descuento por nivel. Sus beneficios son en especie: **seguro sin cargo** (la prima se multiplica por 1 − `SUSCRIPCION_SEGURO_DESCUENTO`; con 1.000 queda en 0 y la póliza se graba igual), **preventa** según RN-39 y **prioridad en la lista de espera** según RN-35. | A |

## Pagos

| ID | Regla | T |
|---|---|---|
| RN-30 | No se almacena el número completo de tarjeta: solo los últimos 4 dígitos y el token. | E |
| RN-31 | Toda operación de pago se registra con su estado resultante. | E |
| RN-32 | Un pago rechazado libera de inmediato el cupo reservado por RN-15. | A |
| RN-33 | Todos los importes se expresan en pesos argentinos. | E |

## Reportes y listas de espera

| ID | Regla | T |
|---|---|---|
| RN-34 | El administrador de plataforma accede a la administración global y a los reportes globales; los organizadores acceden a reportes limitados a eventos de sus propios locales. | A |
| RN-35 | La lista de espera se ordena primero por los clientes que se inscribieron con suscripción activa y después por el resto; dentro de cada grupo, por orden de inscripción. La condición de suscriptor se **congela al inscribirse** (`ListaEspera.ConSuscripcion`): si después la suscripción vence o se cancela, el lugar no cambia. **Ventana de prioridad** (`1525202`): cuando un sector vuelve a tener una entrada (devolución, reserva vencida o pago rechazado) y el evento tiene algún inscripto con suscripción, durante `PRIORIDAD_ESPERA_MINUTOS` solo pueden comprar ese sector los inscriptos con suscripción; los demás reciben el rechazo RN-35. Sin inscriptos con suscripción no hay ventana. El instante sale de `Sectores.CupoLiberadoEl`. | E |
| RN-36 | Un cliente se inscribe una sola vez en la lista de espera de un mismo evento. | E |
| RN-48 | La gestión de locales, eventos y control de ingreso requiere perfil Organizador. | A |
| RN-49 | Un Organizador solo puede consultar o modificar locales, eventos, sectores, ingresos y listas de espera pertenecientes a sus locales. | A |

---

## Orden de cálculo del descuento

```
base       = Σ (precio × cantidad) de las ENTRADAS          // RN-21
porcentaje = TablaDescuento[cliente.Nivel]                  // RN-20, nivel PERSISTIDO
monto      = redondear(base × porcentaje, 2)
subtotalE  = base − monto
servicios  = Σ precio de servicios complementarios          // sin descuento
cargo      = redondear(subtotalE × 0,15 ; 2)
seguro     = cantidad de entradas × SEGURO_PRECIO, si se contrata   // RN-28
seguro     = redondear(seguro × (1 − SUSCRIPCION_SEGURO_DESCUENTO) ; 2), con suscripción activa  // RN-37
total      = subtotalE + servicios + cargo + seguro
```

El seguro se cobra **por entrada** (una póliza por entrada en `Seguros`): con 3 entradas son
3 × $5.000 = $15.000. No recibe el descuento por nivel ni el cargo por servicio.

El nivel **se lee de la entidad**, no se recalcula en el momento del cálculo.
Eso es lo que materializa RN-16 y RN-22.

## Parámetros configurables

No escribir estos valores en el código. Van en la tabla `Parametros`.

| Clave | Regla | Valor inicial |
|---|---|---|
| `PWD_LONGITUD_MIN` | RN-02 | 8 |
| `PWD_FORTALEZA_MIN` | RN-47 | 4 |
| `LOGIN_INTENTOS_MAX` | RN-04 | 5 |
| `LOGIN_BLOQUEO_MIN` | RN-04 | 15 |
| `COMPRA_ENTRADAS_MAX` | RN-12 | 6 |
| `RESERVA_MINUTOS` | RN-15 | 10 |
| `FIDELIDAD_VENTANA_MESES` | RN-19 | 12 |
| `FIDELIDAD_UMBRAL_PLATA` | RN-19 | 3 |
| `FIDELIDAD_UMBRAL_ORO` | RN-19 | 7 |
| `DESCUENTO_PLATA` | RN-20 | 0.050 |
| `DESCUENTO_ORO` | RN-20 | 0.075 |
| `DESCUENTO_MAX_ADMISIBLE` | RN-20 | 0.086 |
| `CARGO_SERVICIO` | — | 0.150 |
| `SEGURO_HORAS_COBERTURA` | RN-29 | 48 |
| `SEGURO_PRECIO` | RN-28 | 5000 |
| `SUSCRIPCION_MENSUAL_PRECIO` | RF-13 | 9000 (anexo A02: 1.476.000 ÷ 164) |
| `SUSCRIPCION_ANUAL_PRECIO` | RF-13 | 90000 (anexo A02: 4.590.000 ÷ 51) |
| `SUSCRIPCION_SEGURO_DESCUENTO` | RN-37 | 1.000 (100 %: sin cargo) |
| `PRIORIDAD_ESPERA_MINUTOS` | RN-35 | 15 (`BaseDatos/06-prioridad-temporal.sql`) |
| `CORREO_CODIGO_MIN` | RN-44 | 10 |
| `CORREO_CODIGO_INTENTOS_MAX` | RN-44 | 5 |
| `SESION_INACTIVIDAD_MIN` | RN-46 | 15 |
| `SESION_AVISO_SEG` | RN-46 | 60 |

## Conflictos resueltos

| Reglas | Criterio adoptado |
|---|---|
| RN-08 vs RN-15 | El control de capacidad computa vendidas **más** reservadas vigentes. |
| RN-22 vs RN-16 | Rige el nivel vigente al iniciar la operación. |
| RN-23 vs RN-19 | El descenso se hace efectivo en la reevaluación mensual, nunca en medio de una compra. |
| RN-35 vs RN-37 | RN-35 decía «orden estrictamente de inscripción» y RN-37 promete prioridad al suscriptor. Se reescribió RN-35: primero suscriptores, después el resto, cada grupo por orden de inscripción. La prioridad se **congela** al inscribirse, igual que `NivelAplicado` en `Compras`: si se consultara la suscripción al leer la lista, el orden cambiaría cada vez que a alguien se le vence el plan. |

## Análisis económico: exposición del seguro sin cargo

Con `SUSCRIPCION_SEGURO_DESCUENTO` = 1.000 el suscriptor no paga la prima de
`SEGURO_PRECIO` = $5.000 por entrada (RN-37). Lo que se resigna:

- **Sin tope.** La bonificación es de $5.000 por entrada, en todas las compras del período,
  hasta el máximo de 6 entradas por operación (RN-12). Una sola compra de 6 entradas
  bonifica $30.000.
- **Selección adversa.** El beneficio atrae justamente a quien piensa comprar mucho y con
  seguro, o a quien prevé que puede tener que devolver: el cliente puede suscribirse para
  una compra concreta y cancelar después (RN-26 no lo impide).
- **El plan mensual no se cubre, y con los precios del anexo A02 menos todavía.** Desde la
  Sección 9 el mensual cuesta $9.000 (antes $12.000): una sola compra de **2 entradas o
  más** con seguro ya bonifica $10.000 o más, y el plan se paga solo en la primera
  operación (antes hacían falta 3). El anual mejora: a $90.000 (antes $75.000) equivale a
  18 entradas aseguradas en el año (antes 15). La exposición se concentra en el mensual,
  justamente el plan que se puede contratar para una sola compra y no renovar.
- La devolución (RN-29) no reintegra el cargo por servicio ni la prima, y el costo de los
  servicios se recupera del proveedor; con el seguro sin cargo, la prima que no se
  reintegra es 0, así que en una devolución de un suscriptor el único ingreso que queda es
  el cargo por servicio.

Si la exposición resulta excesiva, el ajuste es de parámetro, no de código: bajar
`SUSCRIPCION_SEGURO_DESCUENTO` (por ejemplo a 0.500) o subir el precio de los planes.
