# Sección nueva del documento: «Metodología de verificación»

Claude Code, 16/11/2026, a pedido de JC. Es texto para el documento entregable: lo pega Máximo
desde `docs/LISTA-PARA-MAXIMO.md`, entre el final de «Matriz de trazabilidad» (p. 109) y
«Conclusiones» (p. 110).

**Provisorio en sus cifras.** La estructura y el razonamiento son definitivos. Las cifras se
reemplazan cuando JC congele el código y se vuelva a correr la mutación sobre ese commit:

| Cifra en el texto | Hoy | Fuente | Cuándo cambia |
|---|---|---|---|
| Casos y ejecuciones | 124 casos, 243 ejecuciones, todas correctas | Corrida de Codex del 16/11 (`027527f`) | Cuando Codex termine las pruebas del encargo de mutación |
| Cobertura de reglas | 39 de 41 (95,1 %); sin caso RN-07 y RN-33 | `docs/04-casos-prueba.md` | Solo si cambian las reglas o se agrega caso a RN-07 o RN-33 |
| Mutación | 83 defensas, 6 equivalentes, 51 de 77 detectadas (66,2 %); la séptima equivalente adentro; sin ella, 67,1 % | `docs/INFORME-MUTACION-SEGURIDAD.md`, sobre `fe04b1f` | En la corrida final sobre el commit congelado |
| Commit medido | «commit» | — | Lo pone JC al congelar |

Lo que no es cifra queda igual aunque cambien los números: qué es una mutación, el criterio de
detección, el tratamiento de las equivalentes y qué significa que una defensa sobreviva. **Si en la
corrida final la séptima equivalente deja de existir o aparece otra encontrada después de correr,
el párrafo correspondiente se ajusta con el mismo criterio:** se deja adentro y se informa aparte.

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

La batería tiene ciento veinticuatro casos de prueba. Algunos se ejecutan varias veces con datos
distintos, por ejemplo una pantalla a dos escalas de visualización, y en total suman doscientas
cuarenta y tres ejecuciones, todas correctas en la última corrida. Como cada corrida recrea la
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

Con ese criterio, treinta y nueve de las cuarenta y una reglas vigentes tienen caso automatizado,
un 95,1 %. Las dos restantes no rechazan ninguna operación: RN-07 describe un cálculo y RN-33
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

*Resultado.* De las setenta y siete mutaciones que se podían detectar, la batería detectó
cincuenta y una: un 66,2 %, medido sobre el commit «commit». El pronóstico hecho por lectura acertó
en todas las que se esperaba que sobrevivieran: las veintitrés sobrevivieron.

*Mutaciones equivalentes.* Algunas mutaciones no cambian el comportamiento del sistema, porque
otra defensa del mismo camino ya rechaza la operación. Por ejemplo, si una operación verifica la
sesión al principio y vuelve a verificarla enseguida, quitar la primera verificación no cambia
nada que una prueba pueda observar. Esas mutaciones se llaman equivalentes. Ninguna prueba podría
detectarlas, y por eso se excluyen del cálculo.

Antes de ejecutar, por lectura, se identificaron seis equivalentes, y se excluyeron. Al ejecutar
apareció una séptima: la mutación que quita el control de sesión al contratar una suscripción. El
pronóstico decía que sería detectada; sobrevivió, y al revisar el código resultó también
equivalente, porque la operación consulta enseguida la suscripción activa del usuario, y esa
consulta repite el mismo control.

**Esa séptima mutación se dejó dentro del cálculo, como no detectada, a propósito.** La razón es
de método. Si una mutación se declara equivalente después de ver que sobrevivió, el criterio deja
de ser independiente del resultado. Con el mismo argumento se podría excusar cualquier
sobreviviente incómoda, y el porcentaje dejaría de medir la batería para medir la habilidad de
justificarla. Las seis equivalentes se definieron antes de conocer los resultados; la séptima, no.
Excluirla habría dado un 67,1 %; se informa el 66,2 %.

*Qué significa que una defensa sobreviva.* No significa que el sistema sea vulnerable: la defensa
está en el código y funciona, como se verificó al leerlo. Significa que ninguna prueba fallaría si
esa defensa se quitara por error en una versión futura. Cada defensa sobreviviente se registró
como una prueba por escribir, con lo que debe verificar, y se ordenaron por la consecuencia que
tendría su falta: primero las que protegen los datos personales y de pago de otros clientes,
después el acceso sin contraseña y el control de ingreso, y al final las que solo exponen datos
operativos.

*Un caso que muestra para qué sirve.* El control de ingreso debe admitir una sola vez cada
entrada aunque dos puertas la lean en el mismo instante (RN-18). La prueba original usaba dos
lectores simultáneos. Ejecutada sola, detectaba casi siempre las mutaciones que rompían esa
garantía, diecinueve y dieciocho veces de cada veinte. En la ejecución de la batería completa no
detectó ninguna de las dos, así que no se contaron como detectadas. La prueba se reforzó: diez
lectores arrancados juntos sobre la misma entrada, cien veces seguidas, con un ingreso válido y
nueve rechazos exigidos en cada vuelta. La prueba reforzada detectó las tres mutaciones de
concurrencia en todas las ejecuciones de control. El código de ingreso no se modificó: el sistema ya era correcto, y lo que faltaba era
una prueba capaz de demostrarlo.

*Qué queda fuera de la medición.* La mutación se aplicó a la capa de negocio, a la de datos y al
esquema de la base, que es donde están las defensas; no a la interfaz gráfica, cuyos controles
ocultos o deshabilitados no son una defensa. Tampoco cubre el acceso directo a la base de datos
sin pasar por la aplicación, ni el tiempo de respuesta de la comparación de contraseñas, que una
prueba funcional no puede observar y se verificó leyendo el código.

<!-- fin del texto para el documento -->
