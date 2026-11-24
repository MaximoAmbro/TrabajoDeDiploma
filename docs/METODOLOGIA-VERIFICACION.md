# Sección nueva del documento: «Metodología de verificación»

Claude Code, 16/11/2026, a pedido de JC. Es texto para el documento entregable: lo pega Máximo
desde `docs/LISTA-PARA-MAXIMO.md`, entre el final de «Matriz de trazabilidad» (p. 109) y
«Conclusiones» (p. 110).

**Cifras definitivas.** La mutación se midió sobre el código congelado, `65225e7`. El código que se entrega es posterior, `2db272e` (`2db272ecf0f31898ee617e025ba763be1be7825e`): después de la medición se corrigió M-22, se pasó a tres perfiles, se agregaron RF-14 a RF-22 y se ajustó la interfaz. La cifra de mutación corresponde al commit medido y no se vuelve a medir (decisión de JC del 17/11). La batería y la cobertura son las del código que se entrega.

| Cifra en el texto | Valor | Fuente |
|---|---|---|
| Casos y ejecuciones | 214 casos (CP-01 a CP-211 y CP-354 a CP-357; CP-149 y CP-212 a CP-353 no se usaron), 360 ejecuciones, todas correctas, sobre `2db272e` | La corrió Codex el 24/11, fuera del sandbox; Claude Code cruzó el registro con el código, caso por caso (`TestResults/post-reinicio.log`). Sobre `967e038` eran 205 y 348 (Codex el 19/11 y Claude Code el 20/11, por separado); sobre `dba674c`, 148 y 276; sobre `65225e7`, 145 y 271 |
| Cobertura de reglas | 50 de 52 (96,2 %); sin caso RN-07 y RN-33 | `docs/04-casos-prueba.md` y los comentarios de las pruebas en `2db272e` (`tools/Matriz/Extraer.py`): dan lo mismo |
| Mutación, primera medición | 51 de 77 (66,2 %), sobre `fe04b1f` | `docs/INFORME-MUTACION-SEGURIDAD.md`, secciones 1 a 9 |
| Mutación, medición final | **73 de 77 (94,8 %), sobre `65225e7`**; dos equivalentes descubiertas después, adentro (sin ellas, 97,3 %) | `docs/INFORME-MUTACION-SEGURIDAD.md`, sección 0 |

El texto nombra solo el commit medido, `65225e7`: en el párrafo «Resultado» y otra vez después del de las cuatro no detectadas, donde dice que después se reescribió la autorización (RN-34 pasó a RN-48) y que no se volvió a medir (JC, 18/11). No nombra la versión que se entrega, para que no quede vieja cuando cambie. **Cifras definitivas desde el 24/11**, con el código que se entrega en `2db272e`.

---

## Texto para el documento

**Metodología de verificación**

Las secciones anteriores informan qué se probó y cuánto cubren las pruebas. Esta sección explica
cómo se construyeron las pruebas y, sobre todo, cómo se comprobó que sirven: que detectarían un
defecto si lo hubiera. Una prueba que pasa dice que el sistema se comporta como se espera hoy;
no dice que la prueba se daría cuenta si mañana dejara de hacerlo.

**La batería de pruebas**

Las pruebas son automatizadas y de integración. Cada caso prepara sus propios datos, ejecuta la
operación a través de los mismos componentes de negocio que usa la aplicación y verifica el
resultado contra una base de datos SQL Server real. Esa base se crea de cero al comienzo de cada
corrida, con el mismo esquema y los mismos datos iniciales que se entregan, y nunca es la base de
desarrollo. Las pruebas de interfaz abren los formularios reales de la aplicación, los operan y
leen lo que muestran.

La batería tiene doscientos catorce casos de prueba. Algunos se ejecutan varias veces con datos
distintos, por ejemplo una pantalla a dos escalas de visualización, y en total suman trescientas
sesenta ejecuciones, todas correctas en la última corrida. Como cada corrida recrea la
base, los resultados no dependen de lo que haya quedado de una corrida anterior.

**Cobertura de las reglas de negocio**

Una regla se considera cubierta cuando al menos un caso provoca la situación que la regla regula
y verifica el resultado. Si la regla rechaza una operación, el caso intenta la operación prohibida
y comprueba dos cosas: que el sistema la rechaza por esa regla y no por otra, y que los datos
quedan exactamente como estaban.

El criterio se aplicó de manera estricta. Un caso que solo muestra en pantalla el texto de una
regla no la cubre. Por ejemplo, que la pantalla de suscripción diga «No se renueva
automáticamente» no prueba que la suscripción vencida no se renueve. Dos reglas estuvieron en esa
situación y no se contaron como cubiertas hasta que tuvieron un caso que verificaba el
comportamiento: la de no renovación automática (RN-26) y la que impide agregar el seguro después
de la compra (RN-28).

Con ese criterio, cincuenta de las cincuenta y dos reglas vigentes tienen caso automatizado,
un 96,2 %. Las dos restantes no rechazan ninguna operación: RN-07 describe un cálculo y RN-33
fija la moneda.

**El límite de la cobertura**

La cobertura cuenta qué reglas tienen una prueba, pero no mide si esa prueba detectaría un
defecto. Un ejemplo de este mismo proyecto lo muestra. Una prueba verificaba que, sin una sesión
iniciada, el sistema rechaza un conjunto de operaciones protegidas, y aceptaba cualquier rechazo
como correcto. Si a una de esas operaciones se le quitaba el control de sesión, el sistema igual la
rechazaba, pero por otra regla: por ejemplo, porque el evento no estaba agotado. La prueba seguía
pasando y la defensa faltaba. Para encontrar casos así se usó la prueba de mutación.

**La prueba de mutación**

La prueba de mutación consiste en introducir a propósito un defecto en el código y comprobar si
las pruebas lo notan. Cada defecto introducido se llama mutación. Se aplica una mutación por vez,
se ejecuta la batería completa, y se observa el resultado:

- si al menos una prueba que antes pasaba ahora falla, la mutación fue **detectada**: la batería
  habría avisado de ese defecto;
- si todas las pruebas siguen pasando, la mutación **sobrevivió**: ese defecto podría introducirse
  sin que ninguna prueba lo advierta.

El porcentaje de mutaciones detectadas no mide cuántas pruebas hay, sino cuántos defectos
encontrarían. Por eso se aplicó a las defensas de seguridad, donde un defecto no se ve en el uso
normal: una operación que deja de verificar quién la pide sigue funcionando igual para el usuario
legítimo.

*Cómo se hizo.* Antes de ejecutar nada, se leyó el código y se listaron las ochenta y tres
defensas del sistema:
- los controles de sesión y de perfil de cada operación;
- la verificación y el almacenamiento de las contraseñas;
- el bloqueo por intentos fallidos;
- las verificaciones del control de ingreso por código QR;
- las restricciones de la base de datos.

Para cada una se escribió cómo romperla y un pronóstico de si la batería lo detectaría, también
por lectura. Después se ejecutó cada mutación sobre una copia aislada del código y con una base
de datos propia; antes de cada ejecución se verificaba que la conexión apuntara a esa base y no a
otra. Las tres mutaciones que afectan lecturas simultáneas, cuyo resultado puede variar entre una
ejecución y otra, se ejecutaron además veinte veces cada una. Al terminar se restauró el código
y la batería volvió a dar el mismo resultado que antes de empezar.

*Dos mediciones, sobre dos versiones del código.* La mutación se midió dos veces. La primera,
sobre una versión intermedia, mostró qué defensas no tenían una prueba capaz de notar su falta.
Con ese resultado se escribieron las pruebas que faltaban, y la segunda medición se hizo sobre el
código congelado para medirla. Las ochenta y tres mutaciones son las mismas en las dos. Entre una versión
y otra, el código cambió en algunos de los lugares que esas mutaciones tocan: setenta y nueve se
aplicaron sin ningún cambio, y en cuatro se ajustó solo la forma de ubicar la línea por modificar,
porque había pasado a otro método o aparecía dos veces. En las cuatro se rompe la misma defensa
que en la primera medición. Por eso las dos mediciones son comparables: miden las mismas defensas,
rotas de la misma manera.

*Resultado.* En la primera medición, la batería detectó cincuenta y una de las setenta y siete
mutaciones que se podían detectar: un 66,2 %. El pronóstico hecho por lectura acertó en todas las
que se esperaba que sobrevivieran: las veintitrés sobrevivieron. En la medición final, sobre el
código congelado (commit `65225e7`), la batería detectó setenta y tres de setenta y siete:
un 94,8 %. Para descartar que una prueba fallara por un motivo ajeno a la mutación, por ejemplo una
ventana de otra aplicación que tomara el foco durante una prueba de interfaz, cada detección se
comparó con la prueba que se esperaba que la detectara: las setenta y tres coinciden.

*Mutaciones equivalentes.* Algunas mutaciones no cambian el comportamiento del sistema, porque
otra defensa del mismo camino ya rechaza la operación. Por ejemplo, si una operación verifica la
sesión al principio y vuelve a verificarla enseguida, quitar la primera verificación no cambia
nada que una prueba pueda observar. Esas mutaciones se llaman equivalentes. Ninguna prueba podría
detectarlas, y por eso se excluyen del cálculo.

Antes de ejecutar, por lectura, se identificaron seis equivalentes, y se excluyeron. Al ejecutar
aparecieron dos más, una en cada medición. La primera es la mutación que quita el control de
sesión al contratar una suscripción: la operación consulta enseguida la suscripción activa del
usuario, y esa consulta repite el mismo control. La segunda es la que deja de ocultar, al cambiar
la contraseña, si la cuenta existe: la operación transforma cualquier fallo en el mismo mensaje
genérico, así que la diferencia no llega al usuario. En los dos casos el pronóstico decía que
serían detectadas; sobrevivieron, y al revisar el código resultaron equivalentes.

**Esas dos mutaciones se dejaron dentro del cálculo, como no detectadas, a propósito.** La razón es
de método. Si una mutación se declara equivalente después de ver que sobrevivió, el criterio deja
de ser independiente del resultado. Con el mismo argumento se podría excusar cualquier
sobreviviente incómoda, y el porcentaje dejaría de medir la batería para medir la habilidad de
justificarla. Las seis equivalentes se definieron antes de conocer los resultados; las otras dos, no.
Excluirlas habría dado un 97,3 %; se informa el 94,8 %.

*Qué significa que una defensa sobreviva.* No significa que el sistema sea vulnerable: la defensa
está en el código y funciona, como se verificó al leerlo. Significa que ninguna prueba fallaría si
esa defensa se quitara por error en una versión futura. Cada defensa sobreviviente se registró
como una prueba por escribir, con lo que debe verificar, y se ordenaron por la consecuencia que
tendría su falta: primero las que protegen los datos personales y de pago de otros clientes,
después el acceso sin contraseña y el control de ingreso, y al final las que solo exponen datos
operativos. Esas pruebas se escribieron después: veinte casos nuevos y el refuerzo de uno
existente. Cada uno se comprobó quitando su defensa, con lo que la prueba debía fallar, y
restaurándola, con lo que debía volver a pasar.

En la medición final quedaron cuatro mutaciones sin detectar. Dos son las equivalentes descubiertas
después de ejecutar. La tercera quita la comparación de contraseñas en tiempo constante: una
prueba funcional no puede observar cuánto tarda una comparación, y esa defensa se verificó leyendo
el código. La cuarta quita el control de perfil al registrar el ingreso de una entrada, y es un caso como el
que se describió en «El límite de la cobertura». La prueba de ese registro aceptaba cualquier
rechazo: sin el control, el sistema igual rechazaba al cliente, pero por otra regla, la que impide
controlar el ingreso a eventos ajenos, y sin sesión la operación terminaba en un error no
controlado. Con el control en su lugar el rechazo era el correcto, pero ninguna prueba lo exigía.
Lo encontró la prueba de mutación, y se corrigió después de la medición: el rechazo pasó a ser un
resultado que nombra su regla, y un caso nuevo lo exige, con un cliente y sin sesión, y falla si el
control se quita. Es también el mejor argumento a favor del método: la mutación encontró un
hueco que ninguna de las pruebas existentes advertía.

La cifra de setenta y tres de setenta y siete se midió sobre esa versión, el commit `65225e7`, y
no describe el código que se entrega. Después de la medición se reescribió la autorización: el
sistema pasó de dos perfiles a tres, Cliente, Organizador y Administrador, y los controles de
perfil cambiaron de regla. La mutación del control de perfil al registrar un ingreso, por ejemplo,
se midió cuando ese control era RN-34; hoy es RN-48. La campaña no se volvió a ejecutar sobre el
código nuevo, y por eso la cifra se informa con la versión en la que se midió.

*Un caso que muestra para qué sirve.* El control de ingreso debe admitir una sola vez cada
entrada aunque dos puertas la lean en el mismo instante (RN-18). La prueba original usaba dos
lectores simultáneos. Ejecutada sola, detectaba casi siempre las mutaciones que rompían esa
garantía, diecinueve y dieciocho veces de cada veinte. En la ejecución de la batería completa no
detectó ninguna de las dos, así que no se contaron como detectadas. La prueba se reforzó: diez
lectores arrancados juntos sobre la misma entrada, cien veces seguidas, con un ingreso válido y
nueve rechazos exigidos en cada vuelta. La prueba reforzada detectó las tres mutaciones de
concurrencia en todas las ejecuciones de control. El código de ingreso no se modificó: el sistema ya era correcto, y lo que faltaba era
una prueba capaz de demostrarlo.

*Lo que la revisión de coherencia no alcanzó.* Además de las pruebas, el documento se revisó con
una matriz de diez comprobaciones de coherencia entre los diagramas, la base de datos y el código.
Una de ellas compara los estados de la entrada en cuatro lugares: el diagrama de estados, la
clase, la restricción de la base y el código. Las cuatro listas coincidían. Pero la comprobación
compara listas de estados, no transiciones, y no puede ver de qué estado sale cada flecha. Así
quedó sin detectar que el diagrama hacía salir la devolución de la entrada confirmada en general,
lo que permitía devolver una entrada que ya se había usado para ingresar, cosa que el código
rechaza. No lo encontró el instrumento: lo encontró una revisión externa, hecha por alguien
distinto de quien había dibujado y verificado los diagramas, y se corrigió. Para que el
instrumento lo detectara habría hecho falta una comprobación más: comparar cada transición del
diagrama con la condición que la permite en el código, como se hizo con los métodos del diagrama
de secuencia.

La misma revisión encontró una segunda cosa que ningún instrumento había visto. Una pregunta
sobre una columna del diagrama entidad-relación, la cantidad vendida de cada entrada, mostró que
esa columna entra en el cálculo de los lugares disponibles y que la base la admitía vacía o con
cualquier valor. Una entrada con la cantidad vacía o en cero no se descontaría de la capacidad del
sector: sería una sobreventa. No ocurría en uso, porque el sistema siempre la escribe en uno, pero
la base no lo garantizaba y ninguna prueba verificaba esa condición. Se restringió en la base, con
su prueba.

La matriz no mira transiciones; las pruebas no miraban esa condición. Las dos cosas aparecieron al
revisar los diagramas desde afuera. Esa es la conclusión de método de este trabajo: cada
instrumento de verificación cubre algo y deja algo afuera, saber qué deja afuera es parte del
resultado, y una mirada externa encuentra lo que ningún instrumento propio estaba hecho para ver.

*Qué queda fuera de la medición.* La mutación se aplicó a la capa de negocio, a la de datos y al
esquema de la base, que es donde están las defensas; no a la interfaz gráfica, cuyos controles
ocultos o deshabilitados no son una defensa. Tampoco cubre el acceso directo a la base de datos
sin pasar por la aplicación, ni el tiempo de respuesta de la comparación de contraseñas, que una
prueba funcional no puede observar y se verificó leyendo el código.

**La revisión de pertenencia**

Cuando el sistema pasó de dos perfiles a tres, se revisó una clase de defecto completa, en lugar
de buscar fallas sueltas: el acceso a datos que pertenecen a otro dueño. Con dos perfiles, todos
los dueños de locales compartían el perfil de administrador, y la pregunta era si una cuenta de
ese perfil podía leer o modificar los locales, los eventos, las ventas o las listas de espera de
otra cuenta con solo indicar un identificador ajeno. La revisión recorrió uno por uno todos los
métodos públicos de la capa de negocio. Para cada uno comparó el código anterior al cambio con el
posterior, y registró qué dato quedaba expuesto y qué prueba lo cierra.

Se contó como fuga solo lo que estaba efectivamente abierto en el código anterior, no lo que se
abriría si se quitara una verificación. Con ese criterio, veinticuatro métodos permitían leer u
operar sobre datos de otro dueño. No son veinticuatro defectos independientes, sino cinco familias
con una misma causa: el control comprobaba el perfil de quien llamaba, pero no a quién pertenecía
el dato. Tres métodos eran de locales; trece, de eventos y sectores, entre ellos crear, publicar,
suspender o eliminar el evento de otro dueño; dos, de servicios y disponibilidad; uno, de listas
de espera, que exponía el nombre y el correo de los clientes inscriptos en eventos ajenos; y
cinco, de los reportes y de la consulta de la auditoría, que entregaban las ventas y los importes
de toda la plataforma. Todos se cerraron con dos reglas nuevas: la gestión requiere el perfil
Organizador (RN-48), y un Organizador solo opera sobre los datos de sus propios locales (RN-49).

Aparte de esas veinticuatro, la revisión encontró un defecto de otra naturaleza en el registro de
auditoría. La operación que escribe un registro era pública y recibía como argumento quién era el
autor, de modo que cualquier parte del sistema podía atribuirle una acción a otra persona. Un
registro de auditoría que se puede falsificar no sirve como evidencia. Se cerró haciendo esa
operación interna a la capa de negocio, que toma el autor de la sesión verificada y no de quien
llama. Después se cerró también la capa de datos: sus repositorios dejaron de ser públicos, de
modo que la interfaz no puede escribir un registro de auditoría por ningún camino, y un caso
automático lo comprueba.

Otros métodos se reforzaron sin que estuvieran abiertos, y no se cuentan entre las fugas. El
control de ingreso, por ejemplo, ya comparaba el dueño del evento con la sesión antes del cambio.

La prueba que cubre las lecturas verifica que un Organizador no recibe locales, eventos, sectores,
servicios, disponibilidad, listas de espera ni cifras de reportes de otro. Para comprobar que
sirve, se le quitó a propósito la verificación al código: la prueba falló, y volvió a pasar al
restaurarla. Pero la revisión deja también una lista de lo que no está probado. No todas las
escrituras tienen su caso negativo: faltan pruebas que intenten crear, publicar, suspender,
eliminar o modificar el evento o el sector de otro dueño, registrar un local a nombre de otra
cuenta, consultar los locales de otro Organizador, exportar un reporte ajeno, ejecutar los
reportes distintos del primero y consultar la auditoría como Organizador. Las verificaciones están en el código; lo que
falta es la prueba que demostraría que siguen ahí. Se informan como huecos abiertos, no como
defensas probadas.

<!-- fin del texto para el documento -->
