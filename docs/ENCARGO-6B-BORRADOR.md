# Encargo 6B — BORRADOR, versión 3

Claude Code, a pedido de JC. **Reemplaza las versiones 1 y 2.** No es un encargo vigente:
pasa a `TAREAS-CODEX-PRUEBAS.md` cuando JC lo apruebe y Codex no esté trabajando (mientras
trabaja, ese archivo no se toca).

**Qué cambió en la versión 3:**
- **Sin fechas.** El orden y las dependencias están en `PLAN-RENOVACION-VISUAL.md`, sección 0.
- **Máximo queda fuera.** La verificación del piloto en Visual Studio la hace JC (sección 4).
  Nada de este encargo espera a otra persona.
- **6B completo:** la limpieza, el rediseño de las 18 y los tres extras visuales
  (`PROPUESTA-VISUAL-PANTALLAS-CLAVE.md`, E-1 a E-3). Los recortes pasan a reserva.

**Qué cambió respecto de la versión 1.** 6B no es «aplicar el tema»: es limpiar el estilo
escrito a mano y después aplicar el tema bien. Las fuentes:
- **`docs/REVISION-6A.md`:**
  - R-1: los botones guardan su estilo al guardar desde el diseñador;
  - R-2: hay medidas de una pantalla dentro del tema;
  - R-3: las referencias al tema en el Designer;
  - R-4: una recursión sin tope;
  - O-1: el foco invisible del botón primario;
  - O-2: las fuentes que no se liberan;
  - O-4: el estilo de botón duplicado.
- **`Pruebas/AUDITORIA-TEMA-2026-11-10.md`, de Codex:**
  - 18 Designer con estilo escrito a mano: medí 314 líneas de fuentes y colores;
  - 6 pantallas que asignan estilo en ejecución;
  - 4 que pisan el estilo de grilla después del tema.

**Todas las horas de este archivo son estimadas.**

---

## 1. El enfoque del Designer (regla nueva para las 19 pantallas)

| Va en el Designer | No va en el Designer |
|---|---|
| Controles, nombres, textos, `TabIndex`, `Dock`/`Anchor` | **Ninguna** fuente (`Font`) |
| Posición y tamaño **como números a 96 DPI**, múltiplos de la escala de 8 | **Ningún** color (`ForeColor`, `BackColor`, `Color.*`) |
| `AutoScaleDimensions = 96F, 96F` | `FlatStyle`, `FlatAppearance` |
| Los tipos `BotonPrimario`, `BotonSecundario`, `BotonPeligro` y los componentes de rol (abajo) | **Ninguna** referencia a `Tema.*` (era R-3) |

**Por qué.** Es exactamente lo que Visual Studio escribe y lee siempre. Si alguien guarda
desde el diseñador, no se pierde nada. El estilo vive en dos lugares, y en ninguno se puede
romper al guardar:

1. **Componentes que no serializan su estilo** (corrige R-1):
   - los tres botones, con sus propiedades de estilo marcadas como valor por omisión
     (`[DefaultValue]` igual al del tema, o `ShouldSerialize…() => false`);
   - **componentes de rol para las etiquetas**, con la misma técnica: `EtiquetaTitulo`,
     `EtiquetaSeccion`, `EtiquetaRotulo` y `EtiquetaAyuda` (heredan de `Label`, así que las
     pruebas que buscan `Label` siguen andando);
   - cambiar el tipo de una `Label` a uno de estos queda autorizado como el cambio
     `Button` → `BotonPrimario`.
2. **El tema en ejecución**, para lo demás: la fuente y el fondo de la ventana (la base ya lo
   hace bien), las grillas (`Tema.EstiloGrilla`), y `Tema.Normalizar` para lo que no tiene rol.

**Las medidas de una pantalla no van en `Tema.cs`** (corrige R-2). Se borra `Tema.Compra`.
`Tema.cs` conserva solo los tokens.

**Los colores de estado se conservan, como token.** El auditor lo advirtió y tiene razón:
- **qué estados:** la fila reembolsada en gris, «Devolver» deshabilitado (CP-22 y CP-23), el
  temporizador en rojo en el último minuto (CP-39) y la fila TOTAL de los reportes;
- **cómo se escriben:** nunca con números; con métodos de `Tema` que los nombran, por ejemplo
  `Tema.FilaReembolsada(fila)`, `Tema.FilaTotal(fila)` o `Tema.ColorTemporizador(vence)`;
- **el orden no cambia:** el `Load` sigue corriendo después de `Normalizar`, pero ya no trae
  colores propios.

**Lo que no es estilo y queda en el código de cada pantalla:** los pesos de las columnas
(`FillWeight`). Son disposición, no tema.

**Regla 3 del encargo, reescrita:** «Ningún color ni fuente fuera de `Tema.cs` y de
`Visual/Componentes/`. Las medidas de una pantalla, en su Designer y a 96 DPI.»

---

## 2. Tareas, en orden

`B0` es la batería al terminar las tandas de pruebas (T-46): 135, o 136 si PA-02 quedó con
las dos variantes.

| # | Qué | Detalle | Horas de Codex | Batería al terminar |
|---|---|---|---|---|
| **T-41** | **Piloto corregido** | R-1 (componentes que no se serializan); R-2 y R-3 (`FrmCompraCliente.Designer.cs` con geometría literal, sin fuentes, colores ni `Tema.*`, y etiquetas con componentes de rol); R-4 (repetir una sola vez); O-1 (foco con `Tema.Foco` en el primario, ver `SISTEMA-DISENO.md` 5.1); O-2 (caché de fuentes en `Normalizar`); O-4 (un único estilo de botón). Extraer `Visual/Disposicion.cs` (sección 3). **Prueba nueva:** ningún componente serializa su estilo, con `ShouldSerializeValue` en un host de diseño (como la medición de `REVISION-6A.md`), una fila por componente. CP-62 suma el par `Foco` sobre `Primario` (≥ 3:1). CP-13, CP-25 y CP-54 siguen pasando sin cambios | 6 a 8 | B0 + 7 (3 botones + 4 roles) |
| **T-47** | **Limpieza de los 18 Designer** | Se borra cada `Font`, `ForeColor`, `BackColor`, `FlatStyle` y `FlatAppearance` (314 líneas de fuentes y colores). **En la misma pasada**, cada etiqueta que tenía una fuente propia recibe su componente de rol según su tamaño: 18 pt o más es título; 12 a 17 pt, sección; las negritas chicas, rótulo. Cada botón pasa a su tipo de componente según su color: el violeta, primario; `#E6E7E9`, secundario. **No se mueve ni se redimensiona nada todavía.** Sin este paso de roles, un título de 22 pt quedaría en 10 pt al perder su fuente | 4 a 6 | B0 + 7 |
| **T-48** | **Lo que pisa el tema en ejecución** | Las 6 pantallas de la auditoría: Historial (`:22`, `:153`, `:186-187`, `:245-248`); Listas de espera (`:18`, `:48-51`); Mis locales (`:40`); Reportes (`:61-65`, `:280-281`); Pago (`:57-59`); Eventos del cliente (solo se revisan sus pesos de columna, que quedan). Los colores pasan a tokens o a métodos de estado de `Tema`. **CP-63 se extiende a las 19 pantallas**, a su `.cs` y a su Designer, con una fila por pantalla | 3 a 4 | B0 + 7 + 18 |
| **T-38** | Entrada: `frmInicio`, ★`frmCrearUsuario`, `frmCambiarContraseña` | Lo que ya decía T-38 (enlaces, Enter, Tab, «Mostrar contraseña», sin «Modificar»), más la geometría a 96 DPI con las tres regiones, y el patrón (★) en `frmCrearUsuario` | 4 a 6 | + 3 (patrón) + 2 (estándar) |
| **T-39** | Cliente: `frmMenuCliente`, ★`frmEventosCliente`, ★`frmPago`, ★`frmConfirmacion`, ★`frmHistorialCliente`, `frmSuscripcion` | Lo que ya decía T-39, más el patrón en los cuatro marcados | 10 a 14 | + 12 + 2 |
| **T-49** | **Los tres extras visuales** (`PROPUESTA-VISUAL-PANTALLAS-CLAVE.md`) | **E-1:** encabezado del flujo con `IndicadorPasos` en Eventos, Compra, Pago y Confirmación. **E-2:** TOTAL destacado en la compra, sin tocar `lblResumen`. **E-3:** confirmación como estado de éxito, con el ayudante de íconos de Segoe MDL2 Assets. Va **inmediatamente después de T-39**, sobre las mismas pantallas. Dos pruebas nuevas: el TOTAL destacado coincide con la línea TOTAL de `lblResumen`, y el indicador marca el paso correcto en cada una de las cuatro pantallas | 10 a 15 | + 1 + 4 |
| **T-40** | Administración: ★`frmReportes` y 8 más | Lo que ya decía T-40 («Volver» como botón, conservando nombre y manejador; foco inicial; Tab), más el patrón en `frmReportes`. Incluye la variante al 150 % de `frmAgregarEvento` que salió de PA-04 (A-01) | 8 a 12 | + 3 + 8 |
| **T-50** | El PDF de los reportes con los tokens | `ExportadorPDF.cs:29-36` tiene cinco colores y medidas propios. Pasarlos a `Tema`, convertidos como en el ticket | 1 | + 0 |
| | **Pruebas que cambian a propósito** | CP-21 («RN-11» como título → título en castellano, por V-6); las de V-6 que nombra la versión 1 | 2 a 3, repartidas | |
| **Total** | | | **48 a 69** | **B0 + 60** |

**Orden de ejecución:** T-41 → T-47 → T-48 → T-39 → T-49 → T-38 → T-40 → T-50. Va primero
el flujo de compra, que es lo que más se ve, y la entrada antes que la administración. Las
dependencias entre tareas están en `PLAN-RENOVACION-VISUAL.md`, sección 0.

**Para el control que pidió JC, el reporte de cada tarea dice:**
- **las horas reales** de trabajo de la tarea, sin contar las esperas de una respuesta de JC;
- **lo contable de la tarea** (líneas borradas, roles asignados, pantallas tocadas, filas de
  prueba nuevas).

Es lo que se compara al cerrar T-48 (`PLAN-RENOVACION-VISUAL.md`, sección 0, «Control»).

**Las filas de la batería:**
- **T-41:** 7, de la prueba de serialización (3 botones + 4 componentes de rol).
- **T-47:** ninguna nueva.
- **T-48:** 18, de CP-63 en las 18 pantallas que faltan (la del piloto ya existía).
- **T-38 a T-40:** 30, de la prueba de disposición P6B-1:
  - 6 pantallas con el patrón × 3 escalas = 18;
  - 12 estándar × 1 = 12.
- **T-49:** 5 (1 del TOTAL + 4 del indicador de pasos).
- **Total:** 7 + 18 + 30 + 5 = 60. Si Codex agrupa las filas de otra forma, lo declara y se
  corrige la cuenta.

**Reglas de las tareas:** las de la sexta tanda en `TAREAS-CODEX-PRUEBAS.md`, con la regla 3
reescrita (sección 1), más:
- **una pantalla por vez**, con la batería al terminar cada una;
- si una prueba de tamaño falla (CP-12, CP-13, CP-25, CP-54, P6B-1), **se corrige la
  pantalla, no la prueba**.

---

## 3. El patrón de disposición adaptable (para las seis ★)

Sin cambios de fondo respecto de la versión 1. Ahora apoyado en `Disposicion.cs`, que se
extrae en T-41:
- **funciones extraídas del piloto:** `PintarTarjeta`, `PintarSeparadorInferior`,
  `PintarSeparadorSuperior` y `AjustarAreaVisible(form)`;
- **parametrizadas:** `AlinearAcciones(pie, secundario, principal)` y
  `AjustarDocumento(tarjeta, etiqueta, altoDisponible)`, que separa el «documento y vista»
  del resumen y lo necesitan confirmación y reportes;
- **la escala:** `Escala(control)`;
- **la guarda de reentrada:** con **una sola repetición** (R-4).

Los diez pasos, sin cambios:
1. **Tres regiones:**
   - pie fijo con `Dock = Bottom`, de alto `Tema.AltoPie`;
   - contenido con `Dock = Fill` y `AutoScroll`;
   - el encabezado va **dentro** del contenido y se desplaza con él.
2. **Un solo método `Distribuir()`** fija todas las posiciones del contenido.
3. **Cuándo corre:**
   - al final del constructor;
   - en `SizeChanged` del contenido;
   - en `TextChanged` de las etiquetas variables;
   - en `OnShown`, después de ajustar al área de trabajo.
4. **Medidas:** cada una es un token × la escala; ningún número fijo.
5. **Ancho útil:** el del contenido menos dos márgenes. Los márgenes se cuentan una vez y
   nunca aparece una barra horizontal.
6. **Documento y vista:** la tarjeta mide el mínimo entre su alto natural y lo que queda; el
   texto no cambia de fuente.
7. **Grillas:** el resto del espacio, con un mínimo de encabezado + 3 filas.
8. **Pie:** el secundario a la izquierda y el principal a la derecha.
9. **Ventana:** limitada al área de trabajo y centrada.
10. **Prohibido:**
    - `ScrollControlIntoView` sobre un control más alto que su vista;
    - el alto de una tarjeta variable en el Designer;
    - agrandar la ventana para pasar una prueba.

**Disposición de cada una, a 96 DPI:**

| Pantalla | Ventana | Contenido | Pie |
|---|---|---|---|
| `frmCrearUsuario` | 400 × 400 | Dos columnas de 168: Nombre / Apellido; Mail a todo el ancho; Contraseña / Confirmar; «Mostrar contraseña» | «Ya tengo cuenta» (`LinkLabel`) · `btnregistrarse` primario, `AcceptButton` |
| `frmPago` | 440 × 440 | En el encabezado, «Pago», el temporizador a la derecha y la ayuda de seguridad. Tarjeta: total; Titular y Número a todo el ancho; Vencimiento / Código; `lblEstado`, hasta dos renglones | «Cancelar pago» de peligro, con «No» por defecto · «Pagar» primario, `AcceptButton` |
| `frmConfirmacion` | 720 × 450 | Dos columnas: «Detalle» (desglose como documento y vista) y «Entradas» (carpeta y lista) | «Abrir carpeta» secundario · «Aceptar» primario |
| `frmHistorialCliente` | 760 × 540 | Grillas apiladas: compras, de encabezado + 4 filas; entradas, el resto, con mínimo de 3; la ayuda debajo | «Volver» secundario · «Devolver» **primario**, para que CP-22 y CP-23 no cambien |
| `frmEventosCliente` | 840 × 450 | Filtros con rótulos arriba (U-08); la grilla el resto | «Volver» secundario · «Comprar» primario. Sin `AcceptButton`: Enter busca |
| `frmReportes` | 880 × 550 | Dos filas de filtros, «Generar» primario en la segunda; la grilla, con mínimo de encabezado + 5 filas; la lectura como documento | «Volver» secundario · «Exportar PDF» secundario |

**Tratamiento estándar, para las otras 12:**
- las tres regiones, con las posiciones del Designer pasadas a 96 DPI y a la escala de 8;
- sin `Distribuir()`;
- pasan P6B-1 al 150 %. Si una falla, pasa al patrón.

---

## 4. La verificación del piloto en Visual Studio, para JC

**Quién y cuándo.** La hace JC, una sola vez, **sobre el piloto corregido**:
- **cuándo:** después de que T-41 esté terminada y commiteada (el commit importa para el paso
  6, que vuelve atrás con git);
- **qué bloquea:** solo el rediseño (T-39 en adelante). La limpieza (T-47 y T-48) no espera a
  esta verificación, porque no cambia la forma en que se escribe el Designer.

**Tiempo estimado:**
- instalar Visual Studio, la primera vez: 45 a 60 min, casi todo de descarga;
- la verificación: 20 a 30 min.

### Antes de empezar

1. **Instalar Visual Studio Community** (2022 o posterior) con la carga de trabajo
   **«Desarrollo de escritorio de .NET»**. En «Componentes individuales», confirmar que esté
   marcado **«Paquete de compatibilidad de .NET Framework 4.8»** (en inglés, *targeting
   pack*). Sin él, el proyecto `Visual` no compila.
2. **Windows al 100 %** (Configuración → Pantalla → Escala). Si Visual Studio muestra una
   barra amarilla que ofrece **«Reiniciar Visual Studio con escala al 100 %»**, aceptar.
3. **Que Codex no esté trabajando en el piloto.** Si está con otra pantalla, no importa: el
   paso 6 vuelve atrás un solo archivo.
4. Abrir `SistemaEntradas.sln` y compilar (**Ctrl+Mayús+B**). Tiene que terminar sin errores:
   el diseñador usa lo compilado.

### Qué abrir y qué mirar

**Paso 1. El piloto.**
- **Qué abrir:** en el Explorador de soluciones, `Visual/Cliente/FrmCompraCliente.cs`, clic
  derecho → **«Diseñador de vistas»** (o **Mayús+F7**).
- **Qué mirar:** que se dibuje la pantalla, y no una página blanca con errores ni una barra
  amarilla de advertencias. Si aparece alguna de las dos, **copiar el texto completo**.
- **Captura:** sacar una.

**Paso 2. Los estilos se ven.** En el diseñador:
- «Comprar» es violeta con texto blanco;
- «Volver» es blanco con texto violeta;
- el nombre del evento se ve grande (título);
- «Entradas», «Servicios y seguro» y «Resumen» se ven como subtítulos.

Las posiciones pueden no coincidir con las de la aplicación en marcha: el diseñador muestra
la disposición inicial, y la aplicación la recalcula al abrirse. **Eso es esperable.**

**Paso 3. Los botones no van a guardar su estilo** (la corrección R-1).
- **Qué hacer:** clic en «Comprar» y mirar la ventana **Propiedades** (F4).
- **La regla:** Visual Studio muestra **en negrita** los valores que escribiría en el
  Designer al guardar.
- **Qué tiene que estar en letra normal:** `BackColor`, `ForeColor`, `Font` y `FlatStyle`.
- **Qué es normal que esté en negrita:** `Location`, `Size`, `Text`, `Name` y `TabIndex`.
- **Después:** repetir con «Volver» y con el nombre del evento (`label1`).

**Paso 4. Una pantalla fuera del piloto.**
- **Qué abrir:** `Visual/Cliente/frmEventosCliente.cs`, de la misma forma.
- **Si T-47 ya terminó,** tiene que verse con la letra y los colores del tema.
- **Si no terminó,** se ve con el estilo viejo, y eso también es correcto.
- **En los dos casos,** lo que importa es que se dibuje sin errores.

**Paso 5. Un guardado de prueba.** Comprueba que guardar desde el diseñador no rompe nada.
- **Qué hacer:** en el piloto, mover «Volver» un poco con el mouse, volver a dejarlo donde
  estaba y guardar (**Ctrl+S**).
- **Qué mirar:** desde la carpeta del repositorio, en una consola:

  ```
  git diff -- Visual/Cliente/FrmCompraCliente.Designer.cs
  ```

- **Resultado esperado:** ninguna línea nueva con `Font`, `ForeColor`, `BackColor`,
  `Color.FromArgb` ni `FlatStyle`. Puede haber cambios de orden o de `Location`, y no
  importan.
- **Si hay alguna de esas líneas,** copiarla.

**Paso 6. Volver atrás el guardado de prueba:**

```
git checkout -- Visual/Cliente/FrmCompraCliente.Designer.cs
```

- **Qué hace:** deja el archivo exactamente como en el último commit (por eso T-41 tiene que
  estar commiteada).
- **Qué no hacer:** no commitear nada de esta prueba.
- **Después:** cerrar Visual Studio.

### Qué significa cada resultado

| Resultado | Significa | Qué se hace |
|---|---|---|
| Pasos 1 a 5 bien | **Piloto confirmado** | JC le avisa a Claude Code, que lo anota en `ESTADO-AGENTES.md`. Sigue T-39 |
| Paso 1: página de error o advertencias | Visual Studio no puede construir algún componente nuevo (botones, roles o `Disposicion`) | Se le pasa a Codex el texto copiado. Se corrige el componente, **no** las pantallas. T-47 y T-48 siguen; el rediseño espera esa corrección |
| Paso 2: el piloto se dibuja, pero gris o con letra chica | Los componentes no aplican su estilo dentro del diseñador | Igual que la fila anterior. No es grave para el usuario (en marcha se ve bien), pero sí para editar |
| Paso 3 o 5: `BackColor`, `Font` u otras en negrita, o líneas de estilo en el `git diff` | R-1 no quedó resuelta: un guardado desde el diseñador fija el estilo a mano | Vuelve a Codex. **La prueba de serialización de T-41 debería haberlo detectado**: si pasó y esto falla, la prueba también se corrige |
| Paso 4: `frmEventosCliente` con error | Algo de la base común o del tema se rompió | **Se para todo 6B** hasta encontrar la causa |

---

## 5. Re-estimado

**6B completo: 48 a 69 horas estimadas de Codex**, con la limpieza y los tres extras
visuales. El orden de las tareas, sus dependencias y el resto del plan (tandas de pruebas,
verificaciones y tercera medición) están en `PLAN-RENOVACION-VISUAL.md`, sección 0. **No hay
fechas.**

| Tarea | Horas estimadas |
|---|---|
| T-41, el piloto corregido | 6 a 8 |
| T-47 y T-48, la limpieza | 7 a 10 |
| T-39, el cliente | 10 a 14 |
| T-49, los extras visuales | 10 a 15 |
| T-38, la entrada | 4 a 6 |
| T-40, la administración | 8 a 12 |
| T-50, el PDF de los reportes | 1 |
| Las pruebas que cambian a propósito | 2 a 3 |
| **Total** | **48 a 69** |

**Reserva (no se planifica con ellos; JC los saca si algo no entra):**

| Recorte | Ahorro estimado | Costo |
|---|---|---|
| R-b. Extras visuales: solo E-2 y E-3, sin el encabezado con pasos | 5 a 8 h | El flujo de compra pierde la pieza que más «se ve moderna» |
| R-c. Administración (T-40) sin pasar a 96 DPI: limpieza, roles, tres regiones y «Volver» en el pie | 6 a 9 h | Las 9 pantallas del administrador quedan con la distribución de antes; `frmReportes` sin el patrón puede recortarse al 150 % |
| R-d. Tercera medición acotada a la compra | 4 h | Bajo |
| R-e. Las 12 estándar sin pasar a 96 DPI | 8 a 12 h | Los Designer siguen en 120 y 144 DPI y se reescriben si alguien los abre en Visual Studio a otra escala |

(R-a, «no esperar a Máximo», ya no existe: Máximo quedó fuera del camino.)

---

## 6. Cambios a `SISTEMA-DISENO.md` que entran con este encargo

Se aplican cuando JC apruebe el encargo:
1. **Regla 3 y sección 8:** el enfoque del Designer de la sección 1.
2. **Componentes de rol:** en 5.1 (botones) y en una sección nueva para etiquetas.
3. **Disposición (sección 4, ya propuesta en la versión 1):**
   - presupuesto de alto, con 880 × 550 como máximo;
   - documento y vista;
   - dos grillas;
   - el encabezado se desplaza con el contenido;
   - espaciado compacto en los formularios de carga.
4. **Regla 4:** «Volver» puede pasar de `Label` a `BotonSecundario` conservando su nombre y su
   manejador; las etiquetas, a componentes de rol.
5. **«El diseñador se usa para mirar»** se reemplaza por algo mejor: **guardar desde el
   diseñador no rompe nada**, y lo garantiza la prueba de serialización.

**Ya aplicado:** el foco del botón primario (token `Foco`, en 5.1, en 6 y en la paleta).

---

## 7. Observaciones para Codex (de la revisión de 6A)

- `FrmCompraCliente.cs:706` todavía tiene «l?neas» en un comentario (O-7).
- `Microsoft.Identity.Client` e `iText.Pdfa.Checker` sobran en los `using` del piloto (O-8).
  Son anteriores a 6A.
