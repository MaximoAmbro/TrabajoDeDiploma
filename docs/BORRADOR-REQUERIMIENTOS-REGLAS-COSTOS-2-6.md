# Borrador: requerimientos nuevos, costos y reglas de la versión 2.6

Claude Code, 18/11/2026; actualizado el 19/11 con las decisiones de JC. **Es un borrador para que JC lo revise antes de que entre a la lista
para Máximo.** Nada de esto está en `docs/LISTA-PARA-MAXIMO.md` todavía. Cuando JC lo apruebe,
pasa a las pp. 23, 25 a 27, 35, 36 y 38.

## Lo que JC ya decidió (18/11)

- Los puntos se miden, no se estiman: ~250 líneas de código y pruebas por punto, la mediana de
  RF-01, RF-10, RF-11 y RF-13 medidos con la misma vara. Se redondea al entero más cercano, con
  un mínimo de 1.
- RF-17 y RF-18 de la propuesta anterior se unen en «Protección de la sesión y de la contraseña».
- Todo lo implementado es al menos Should; lo que hace cumplir una regla o cierra un agujero de
  seguridad es Must.
- La estimación aprobada de la p. 35 no se toca; la ampliación va en filas aparte, medidas.
- Se agregan, medidos con la misma vara, la solicitud de Organizador, la revisión y el rechazo
  del primer evento, los tres perfiles y el rediseño de la interfaz. La fase 8a va en una fila
  aparte, porque es una revisión de seguridad y no una función.
- RF-15 espera la medición nueva de Codex.

## Lo que JC decidió el 19/11

- El rediseño queda como fila de costo de RNF-02 y RNF-03, no como RF-23.
- **RF-15 son 2 puntos:** Codex midió 402 líneas, 277 de código y 125 de pruebas; a 250 por
  punto da 1,6, que se redondea a 2.
- La fase 8a queda en 1 punto, con una frase que aclara que la fila mide volumen de código.
- RN-45 ya está escrita y commiteada por Codex (`f3acc75`).

## Cómo se midió

Se sumaron las líneas que agregó cada commit en archivos `.cs` y `.sql`, de código y de pruebas.
Se ignoraron los cambios de espacios y de fin de línea (`git show -w --ignore-cr-at-eol`), que en
el rediseño inflaban la cuenta de 3.626 a 2.048 líneas. No se cuentan las capturas, las figuras,
los proyectos ni la documentación. Cuando un commit hizo varias cosas, se repartió por archivo
(el reparto está en la columna «De dónde»).

## 1. Requerimientos nuevos (p. 23 y p. 36)

| RF | Requerimiento | Prioridad | Pts | Líneas | De dónde | Reglas | CU |
|---|---|---|---|---|---|---|---|
| RF-14 | Verificación del correo | Must | 1 | ~288 | `ff489ce`: registro, bandeja y exigencia antes de comprar, más la mitad de la infraestructura de códigos | RN-43, RN-44 | CU-04, CU-14 |
| RF-15 | Recuperación de la contraseña | Must | 2 | ~402 | Medición de Codex: 277 líneas de código y 125 de pruebas | RN-44, RN-47 | CU-15 |
| RF-16 | Registro de auditoría | Must | 1 | ~236 | `ff489ce`: tabla, repositorio, gestor, pantalla y llamadas | RN-45 | CU-20 |
| RF-17 | Protección de la sesión y de la contraseña | Must | 1 | ~184 | `ff489ce`: cierre por inactividad e indicador de fortaleza | RN-46, RN-47 | CU-05, CU-09 |
| RF-18 | Mapa de sectores | Should | 1 | ~216 | `fed7918` | — | CU-01 |
| RF-19 | Portadas de los eventos | Should | 1 | ~333 | `57512c7` | — | CU-01, CU-02 |
| RF-20 | Solicitud para ser Organizador | Must | 3 | ~838 | `ac49a41` | RN-50 a RN-53 | CU-16, CU-18 |
| RF-21 | Publicación explícita y revisión del primer evento | Must | 2 | ~586 | `3e7e400`, `40e91b3`, `58e7b3b` | RN-06, RN-54 | CU-17, CU-19 |
| RF-22 | Tres perfiles de usuario | Must | 1 | ~230 | `a48eb43`, la parte de perfiles: menús, migración, sesión y alta de administradores | RN-05, RN-48 | CU-05, CU-21 |

**Texto para la p. 23** (mismo formato que RF-01 a RF-13: descripción y criterio de aceptación).
**⏸ RF-14 y RF-15 esperan:** el criterio de aceptación tiene que decir qué garantiza la
recuperación, y hoy la pantalla de recuperación deja ver el código a quien lo pide (ver el
informe a JC del 18/11).

> **RF-16 – Registro de auditoría.** El sistema debe registrar las operaciones relevantes con el
> usuario, la acción, la entidad afectada, la fecha y el resultado. *Criterio de aceptación:* el
> Administrador de la plataforma consulta el registro filtrado por período y por acción, y ningún
> otro perfil puede consultarlo.
>
> **RF-17 – Protección de la sesión y de la contraseña.** El sistema debe cerrar una sesión sin
> actividad después de un plazo configurable, con aviso previo, y exigir una fortaleza mínima a
> toda contraseña nueva. *Criterio de aceptación:* la aplicación avisa antes del cierre y cierra la
> sesión al vencer el plazo; una contraseña que no alcanza el puntaje mínimo se rechaza, y la
> pantalla muestra la fortaleza mientras se escribe.
>
> **RF-18 – Mapa de sectores.** El sistema debe mostrar los sectores de un evento en un mapa
> orientativo, con su precio y su disponibilidad. *Criterio de aceptación:* elegir un sector en el
> mapa lo agrega al armado del paquete por el mismo camino que la tabla, y un sector agotado no se
> puede elegir.
>
> **RF-19 – Portadas de los eventos.** El sistema debe permitir al Organizador cargar una imagen
> de portada para cada evento y mostrarla en el catálogo. *Criterio de aceptación:* la imagen se
> reduce y se guarda; un archivo que no es una imagen, o que supera el tamaño máximo, se rechaza;
> un evento sin portada muestra una generada.
>
> **RF-20 – Solicitud para ser Organizador.** El sistema debe permitir a un Cliente solicitar ser
> Organizador, con los datos del responsable, del local y de la cuenta bancaria, y al
> Administrador de la plataforma aprobar o rechazar la solicitud. *Criterio de aceptación:* la
> solicitud exige un teléfono confirmado y valida el CUIT y el CBU; la decisión del Administrador
> requiere un motivo; aprobarla cambia el perfil a Organizador y crea el local.
>
> **RF-21 – Publicación explícita y revisión del primer evento.** El sistema debe publicar un
> evento solo cuando el Organizador lo decide, y enviar a revisión el primer evento de un
> Organizador nuevo. *Criterio de aceptación:* no se publica un evento sin un sector con
> capacidad; el primer evento queda en revisión hasta que un Administrador lo aprueba o lo rechaza
> con un motivo, y un evento rechazado vuelve a borrador con el motivo a la vista del Organizador.
>
> **RF-22 – Tres perfiles de usuario.** El sistema debe distinguir al Cliente, al Organizador y al
> Administrador de la plataforma, cada uno con su menú y sus permisos. *Criterio de aceptación:*
> cada perfil accede solo a sus operaciones; el Organizador opera solo sobre sus locales; solo un
> Administrador crea otro Administrador.

**Nota al pie de la tabla de la p. 36** (decisión de JC: mostrar la medición, no adivinar la
escala):

> Los puntos de RF-14 a RF-22 no se estimaron: se midieron después de construir cada
> requerimiento, contando las líneas de código y de pruebas que agregó, a razón de unas 250 líneas
> por punto. Esa relación es la mediana de cuatro requerimientos estimados (RF-01, RF-10, RF-11 y
> RF-13) medidos con la misma vara; varía entre 129 y 268 líneas por punto. Se redondea al entero
> más cercano, con un mínimo de un punto.

## 2. Trabajo sin requerimiento propio (filas aparte en la p. 35)

| Fila | Pts | Líneas | De dónde | Por qué no es un RF |
|---|---|---|---|---|
| Rediseño de la interfaz (RNF-02, RNF-03) | 8 | ~2.048 | `02989e3`, `ede18ed`, `c19348e`, `888461d` | Es un requerimiento no funcional: mejora la experiencia de los RF existentes, no agrega una función |
| Revisión de pertenencia (fase 8a) | 1 | ~373 | `a48eb43`, la parte de pertenencia: guardas de los gestores, filtro de reportes y la prueba CP-182 | Es una revisión de seguridad, no una función (decisión de JC) |

**Frase para debajo de la tabla de la p. 35** (decisión de JC del 19/11):

> La fila de la revisión de pertenencia mide el volumen de código que agregó, como las demás. El
> peso de esa revisión no está en sus líneas sino en lo que verificó: 24 métodos en cinco
> familias, descriptos en «Metodología de verificación», junto con la prueba de mutación.

**Decisión que tomé y anoto:** JC pidió el rediseño «como requerimiento». Lo puse como fila de
costo atribuida a RNF-02 y RNF-03, sin número de RF, porque un rediseño no es una función nueva.
Si JC prefiere un RF, es RF-23 con los mismos 8 puntos.

## 3. Estimación de costos (p. 35), con V = $30.500

| Fila | Requerimientos | Puntos | Horas | Costo |
|---|---|---|---|---|
| Fases 4 a 8, como estaban | RF-01 a RF-08, RF-10 a RF-13 | 70 | 280 | $8.540.000 |
| **Ampliación de la versión 2.6, medida a posteriori** | RF-14 a RF-22 | **13** | **52** | **$1.586.000** |
| **Rediseño de la interfaz, medido** | RNF-02, RNF-03 | **8** | **32** | **$976.000** |
| **Revisión de pertenencia, medida** | — | **1** | **4** | **$122.000** |
| **Alcance construido** | | **92** | **368** | **$11.224.000** |
| RF-09, estimado y no construido | RF-09 | 8 | 32 | $976.000 |
| **Total** | | **100** | **400** | **$12.200.000** |

**Composición de la ampliación (13 puntos):** RF-14 1, RF-15 2, RF-16 1, RF-17 1, RF-18 1,
RF-19 1, RF-20 3, RF-21 2 y RF-22 1.

**Cuentas verificadas:** 1 + 2 + 1 + 1 + 1 + 1 + 3 + 2 + 1 = 13; 13 × 4 = 52 h;
52 × 30.500 = 1.586.000. 70 + 13 + 8 + 1 = 92; 92 × 4 = 368 h; 368 × 30.500 = 11.224.000.
92 + 8 = 100; 100 × 4 = 400 h; 400 × 30.500 = 12.200.000.

**Cambio respecto del borrador del 18/11:** con RF-15 en 2 puntos, la ampliación pasa de 11 a
13 puntos ($1.342.000 a $1.586.000), el alcance construido de 90 a 92 puntos ($10.980.000 a
$11.224.000) y el total de 98 a 100 puntos ($11.956.000 a $12.200.000).

**Cómo se explica si el profesor pregunta:** la estimación aprobada (78 puntos, $9.516.000) se
conserva tal cual. Lo construido después se midió, con la misma relación entre puntos y código que
tienen los requerimientos estimados, y va en filas separadas, para que lo estimado y lo medido no se
mezclen.

**Lo que también cambia:**
- **p. 35:** «trece requerimientos funcionales» pasa a «veintidós».
- **p. 36:** la tabla «Distribución del esfuerzo estimado» no se recalcula (ya estaba decidido:
  es la estimación aprobada). Debajo va una línea para la ampliación medida: Must 11 puntos
  (84,6 %: RF-14 a RF-17 y RF-20 a RF-22) y Should 2 puntos (15,4 %: RF-18 y RF-19), sobre 13.
- **p. 38:** el orden de implementación suma una «Fase 9, ampliación de la versión 2.6».

## 4. Reglas nuevas y modificadas (pp. 25 a 27)

Texto de `docs/02-reglas-negocio.md` (Codex, 18/11), sin cambios. La cuarta columna usa los RF y
CU propuestos arriba:
- **CU-14:** Verificar el correo.
- **CU-15:** Recuperar contraseña.
- **CU-16:** Solicitar ser Organizador.
- **CU-17:** Publicar evento.
- **CU-18:** Revisar solicitudes de Organizador.
- **CU-19:** Revisar el primer evento.
- **CU-20:** Consultar el registro de auditoría.
- **CU-21:** Crear administrador.

**Reemplazar** RN-05 y RN-34 por:

| RN-05 | Un usuario tiene un único perfil excluyente: Cliente, Organizador o Administrador de plataforma. El administrador de plataforma no es dueño de locales; el organizador administra solo los locales, eventos e ingresos de su cuenta. | E | RF-22 / CU-05 |
|---|---|---|---|
| RN-34 | El administrador de plataforma accede a la administración global y a los reportes globales; los organizadores acceden a reportes limitados a eventos de sus propios locales. | A | RF-11 / CU-08 |

**Reemplazar** las filas RN-40 a RN-42 que la lista ya mandaba agregar (p. 25) por:

| RN-40 | Para cambiar una contraseña se debe comprobar la contraseña actual. El rechazo no revela si el correo existe, si la contraseña es incorrecta o si la cuenta está bloqueada; los intentos y el bloqueo se aplican internamente. | A | RF-02 / CU-09 |
|---|---|---|---|
| RN-41 | Las operaciones sobre datos privados de una cuenta solo pueden devolverlos o modificarlos para el usuario autenticado titular de esa cuenta. La sesión conserva una identidad verificada y no acepta como autorización un objeto de usuario alterado por quien llama. | A | RF-02 / CU-05 |
| RN-42 | **Dada de baja.** La autorización por perfil para gestión e ingreso fue reemplazada por RN-48; la autorización por pertenencia a los datos del Organizador, incluidos sectores, listas de espera e ingresos, está en RN-49. Los reportes se rigen por RN-34. El número no se reasigna. | — | — |

**Agregar** a continuación:

| RN-43 | Una cuenta de Cliente debe verificar el correo antes de iniciar una compra. El código de verificación se envía al correo registrado; la entrega usa la bandeja simulada del sistema. | A | RF-14 / CU-04, CU-14 |
|---|---|---|---|
| RN-44 | Los códigos de verificación de correo, recuperación de contraseña y confirmación telefónica son de un solo uso, se almacenan protegidos, vencen según `CORREO_CODIGO_MIN` y admiten como máximo `CORREO_CODIGO_INTENTOS_MAX` intentos. La recuperación cambia la contraseña mediante las mismas validaciones y el mismo hash que el registro. | A | RF-14, RF-15 / CU-14 a CU-16 |
| RN-45 | Las operaciones de inicio de sesión, cambio de contraseña, compras, devoluciones, control de ingreso y gestión de eventos se registran en `Auditoria` con el usuario, la acción, la entidad afectada, la fecha y el resultado. Solo el Administrador de plataforma puede consultar esos registros, con filtros por período y acción. | A | RF-16 / CU-20 |
| RN-46 | Una sesión sin actividad se cierra al cumplirse `SESION_INACTIVIDAD_MIN`. La aplicación avisa con `SESION_AVISO_SEG` segundos de anticipación; cualquier actividad reinicia el plazo. | A | RF-17 / CU-05 |
| RN-47 | Una contraseña nueva debe cumplir la longitud y composición mínima de RN-02 y alcanzar el puntaje configurado en `PWD_FORTALEZA_MIN`, calculado por la variedad de minúsculas, mayúsculas, números y símbolos. | A | RF-17 / CU-04, CU-09, CU-15 |

**Tabla de reportes y listas de espera (la de RN-34): agregar:**

| RN-48 | La gestión de locales, eventos y control de ingreso requiere perfil Organizador. | A | RF-22 / CU-02, CU-10 a CU-12 |
|---|---|---|---|
| RN-49 | Un Organizador solo puede consultar o modificar locales, eventos, sectores, ingresos y listas de espera pertenecientes a sus locales. | A | RF-22 / CU-02, CU-07, CU-10 a CU-12 |

**Tabla nueva, «Solicitudes de Organizador y revisión del primer evento»:**

| RN-50 | Solo una cuenta Cliente puede presentar una solicitud para ser Organizador y no puede mantener más de una solicitud pendiente. La solicitud pendiente no cambia el perfil ni concede permisos de Organizador. | A | RF-20 / CU-16 |
|---|---|---|---|
| RN-51 | Antes de presentar la solicitud, el responsable debe ingresar un teléfono argentino válido y confirmarlo mediante un código de un solo uso enviado por el mecanismo de códigos del sistema. | A | RF-20 / CU-16 |
| RN-52 | La solicitud valida el dígito verificador del CUIT y del CBU; también admite un alias bancario con el formato definido. La documentación debe ser una imagen PNG o JPEG válida, comprimida y de hasta 512 KB. La verificación externa de identidad es simulada y no sustituye la revisión del Administrador. | A | RF-20 / CU-16 |
| RN-53 | Solo el Administrador de plataforma puede aprobar o rechazar solicitudes de Organizador. Ambas decisiones requieren un motivo de hasta 200 caracteres y registran la identidad del revisor y la fecha; aprobar cambia el perfil a Organizador y crea el local solicitado. | A | RF-20 / CU-18 |
| RN-54 | La primera publicación de un Organizador se envía a revisión del Administrador y no aparece en el catálogo ni admite ventas mientras está en revisión. El Administrador puede aprobarla o rechazarla; para rechazar debe ingresar un motivo no vacío de hasta 500 caracteres. El rechazo devuelve el evento a Borrador y conserva el motivo para el Organizador. Si vuelve a publicarlo antes de que se haya aprobado un evento suyo, vuelve a revisión. Tras su primera aprobación, sus publicaciones siguientes son directas. | A | RF-21 / CU-17, CU-19 |

**RN-45 y RN-34 no se superponen:** RN-45 regula qué se registra en la auditoría y quién la
consulta; RN-34, el alcance administrativo y los reportes. Por eso RN-34 cita RF-11 y RN-45 cita
RF-16.

**Cuenta de reglas vigentes:** de RN-01 a RN-54 hay 54 números; menos RN-27 y RN-42, dadas de
baja, quedan **52 vigentes**. Cambia «cuarenta y una» en las pp. 106 y 110 y
en la sección «Metodología de verificación». La cobertura (cuántas tienen caso) espera los números
finales de la batería.

## 5. Lo que esto cambia en la matriz (p. 107), para decidir con el borrador

Las 157 pruebas (202 casos) ya tienen su comentario `// CP-xx — RF, RN` (Codex, `f3acc75`). Pero
las pruebas de las funciones nuevas citan los requerimientos viejos, porque RF-14 a RF-22 todavía
no existen:

| Regla | Casos | RF que cita hoy el comentario | RF del borrador |
|---|---|---|---|
| RN-43 | CP-167, CP-168, CP-203 | RF-05, RF-01 | RF-14 |
| RN-44 | CP-169 a CP-171, CP-173, CP-176, CP-200 a CP-202 | RF-01 | RF-14, RF-15 |
| RN-45 | CP-174, CP-175, CP-189, CP-196 | RF-11, RF-12 | RF-16 |
| RN-46, RN-47 | CP-172, CP-174 | RF-01, RF-11 | RF-17 |
| RN-50 a RN-53 | CP-177 a CP-180, CP-184 a CP-196, CP-202 | RF-12, RF-01 | RF-20 |
| RN-54 | CP-198, CP-199, CP-204 | RF-12, RF-03 | RF-21 |
| RN-48, RN-49 | CP-94, CP-123, CP-141, CP-142, CP-179, CP-182, CP-198 | RF-04, RF-07, RF-11 a RF-13 | RF-22 |

**Propuesta:** cuando JC apruebe la numeración, Codex cambia el RF de esos comentarios, para que
el código y la matriz digan lo mismo. La matriz se arma después, desde los comentarios, como
hasta ahora. Si se armara con los RF de hoy, las funciones nuevas quedarían repartidas entre
RF-01, RF-11 y RF-12, y RF-14 a RF-22 aparecerían sin casos.
