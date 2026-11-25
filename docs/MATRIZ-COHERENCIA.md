# Matriz de coherencia: las diez comprobaciones, corridas sobre el documento

> **Origen y autoría.** Esta matriz no es un documento del profesor. Sale de la sección 6 de
> `Prompt_Documentacion_Trabajo_de_Diploma.md`, un prompt armado sobre la guía de resolución de
> Audoglio que llegó por WhatsApp el 11/11/2026 y **no dice quién lo escribió. Su autoría no está
> confirmada:** Juan Cruz lo confirma con el profesor. Si el profesor usa otras comprobaciones,
> este archivo deja constancia de dónde salieron estas.

Claude Code, 16/11/2026, a pedido de JC. Es el tercer instrumento: los otros dos (la guía oficial
y los 16 errores frecuentes) están en `docs/CHEQUEO-INSTRUMENTOS-DOCENTE.md`.

Se corrió sobre el documento **tal como queda después de pegar `docs/LISTA-PARA-MAXIMO.md`**: los
diagramas de `docs/diagramas/fuentes/`, el esquema `BaseDatos/01-esquema.sql` y el código del
commit `ea9212a`. Todo por lectura, con scripts que solo leen; no se corrió la batería ni se tocó
ninguna base.

## Pasada final, sobre `dba674c` (el código que se entrega)

Pedida por JC después del cierre de Codex, para saber si alguna de las piezas nuevas rompió algo
que ya estaba bien. Se corrió sobre la lista regenerada con todo aplicado: script 07, las tres
capturas del guion, U-09, M-22 y las cifras de 148 casos y 276 ejecuciones. Se usaron los
diagramas de `docs/diagramas/fuentes/`, `BaseDatos/01-esquema.sql` y `07-indice-unico-seguros.sql`,
y el código de `dba674c`.

**Pasan 9 de 10, las nueve sin condiciones. La 10 sigue bloqueada, no fallada.**

| # | Comprobación | Resultado | Cómo se volvió a comprobar |
|---|---|---|---|
| 1 | RF core → CU, robustez, secuencia, código | Sí | Los doce archivos de la tabla del core existen en `dba674c`. La tabla ahora nombra ese commit |
| 2 | Estados idénticos en las 4 listas | Sí | El cierre no tocó `EstadoEntrada.cs`, `05-check-estado-entradas.sql` ni `estados-entrada.puml` |
| 3 | Clases del dominio en el diagrama de clases | Sí | El párrafo de equivalencias está en la lista generada |
| 4 | Misma multiplicidad en dominio, clases y DER | **Sí, ahora sin condición** | Entrada — Seguro: 0..1 en el conceptual, en el dominio actualizado, en la vista 3 de clases y en el entidad-relación (los dos últimos, regenerados). Compra y sector: holgura explicada en el texto |
| 5 | Atributo de FK y línea a la vez | Sí | `Seguro` no tiene atributo de la entrada; la línea lleva el rol `IdEntrada` |
| 6 | Toda 1:1 con UNIQUE en la base | **Sí, ahora sin condición** | Entradas — Seguros tiene `UX_Seguros_ID_Entrada` (`01-esquema.sql` y script 07). Usuario — Cliente y Usuario — Propietario son únicas por tener la PK como FK |
| 7 | Composiciones con cascada o baja lógica | Sí (no hay) | Ninguna composición en las fuentes |
| 8 | Métodos de la secuencia en clases y código | Sí | Las 17 operaciones distintas de las tres secuencias existen en el código y en algún diagrama de clases. El cambio de U-09 no las toca: la secuencia termina al abrir la confirmación |
| 9 | Nombres consistentes | Sí, salvo glosario | Sin cambios: el documento no tiene glosario |
| 10 | Devolución del Parcial 1 | **Bloqueada (no fallada)** | No apareció la devolución |

**Lo que se buscó además, porque el cierre cambió comportamiento:**
- **U-09:** ningún texto de la lista dice que la compra vuelva a un armado vacío. Los diagramas de
  actividad y de secuencia no dibujan qué pasa después de la confirmación, así que no contradicen
  la vuelta al listado.
- **M-22:** la metodología y el informe describían el error no controlado como si fuera del código
  en uso; era de la mutación. Corregido en los dos (ver `docs/INFORME-MUTACION-SEGURIDAD.md`,
  «M-22»).
- **G-06** (p. 109) seguía diciendo «quedan diez hallazgos menores», contra la declaración de
  usabilidad de la versión 2.6, que deja cinco abiertos. Corregido: cinco, 7 horas estimadas
  (1,75 puntos de la tabla del plan de mejora del informe de usabilidad).

### Revisión externa de JC (17/11), sobre cuatro diagramas

JC revisó por su cuenta el entidad-relación, el de estados, el de robustez y el de casos de uso,
porque eran lo único del proyecto que había hecho y verificado el mismo agente.

- **Estados, contradicción con el código, corregida.** La devolución salía del borde de
  Confirmada, así que el diagrama permitía devolver una entrada ya ingresada. El código lo rechaza
  dos veces (`GestorCompras.cs:301` y el `UPDATE` de `RepositorioCompras.cs:203-204`, con
  `FechaIngreso IS NULL`). Ahora sale de «Sin ingreso». **Esta matriz no lo vio:** la comprobación 2
  compara las listas de estados, no las transiciones. Queda como límite del instrumento.
- **`Entradas.CantidadVendida`:** se usa y vale siempre 1 en lo que se guarda (`GestorCompras.cs:74-82`
  y las dos semillas); el valor mayor que 1 vive solo en el paquete en memoria. RN-18 no queda
  ambigua: un QR por fila, una fila por entrada. Aclarado en el texto del entidad-relación.
- **`Entradas.ID_Usuario`** y **el Cliente como actor y como entidad** en robustez: contestados en
  el texto de cada diagrama.
- **«Aplicar descuento por nivel»:** se queda; justificación para la defensa en
  `docs/PARA-MAXIMO-DIAGRAMAS.md`, sección 1.

## De dónde salió la matriz

No estaba en el repositorio. Apareció en el disco, fuera de él, buscando por contenido:

- **Archivo:** `Prompt_Documentacion_Trabajo_de_Diploma.md` (y su copia en PDF), en los adjuntos de
  WhatsApp Desktop, carpeta de transferencias de la semana 46 (llegó el 11/11/2026).
- **Qué es:** un prompt para armar la documentación del Trabajo de Diploma, con la estructura de
  la guía oficial (profesor Audoglio). Su sección 6 es la matriz. **No dice quién lo escribió.**
  JC confirma si es la que usa el profesor.
- No se encontró otra: se revisaron Descargas, Documentos, OneDrive, el escritorio, las carpetas
  de `D:\` y los adjuntos de WhatsApp (PDF, Word, PowerPoint, Excel, texto y Markdown).

**Texto original de la sección 6**, sin cambios:

> ## 6. Matriz de coherencia (completala al cerrar cada iteración)
>
> | Verificación | Cómo se comprueba |
> |---|---|
> | Cada RF core tiene CU, robustez, secuencia y código | Tabla de trazabilidad RF → CU → diagramas → archivos |
> | Los estados de la entidad son idénticos en el diagrama de transición, en la clase, en el enum o CHECK de la base y en el código | Comparar las 4 listas |
> | Cada clase del dominio actualizado existe en el diagrama de clases (o se explica por qué no) | Lista lado a lado |
> | Cada relación tiene la misma multiplicidad en dominio, clases y DER | Tabla de relaciones |
> | Ninguna clase tiene un atributo de clave foránea y a la vez una línea hacia la misma clase | Revisar clase por clase |
> | Toda relación 1:1 del diagrama tiene UNIQUE en la base | Revisar el script |
> | Toda composición del diagrama tiene borrado en cascada o baja lógica coherente en la base | Revisar las FK |
> | Los métodos de la secuencia existen en las clases del diagrama de clases y en el código | Buscar cada nombre |
> | Los nombres (clases, atributos, tablas) son consistentes en todo el documento | Glosario |
> | Lo que marcó la devolución del Parcial 1 está corregido y anotado en la hoja de seguimiento | Lista de observaciones vs. correcciones |

## Resultado de la primera corrida (16 y 17/11, sobre `ea9212a`)

**Pasan 9 de 10** (decisión de JC del 17/11 sobre M-1): siete sin condiciones, la 4 y la 6 cuando
entre el script 07. **La 10 está bloqueada, no fallada:** falta el material para comprobarla, y
nada indica que el documento no la cumpla.

| # | Comprobación | Antes | Ahora | Qué se hizo |
|---|---|---|---|---|
| 1 | RF core → CU, robustez, secuencia, código | **No** | Sí | La matriz principal llegaba hasta los casos de prueba. Tabla nueva en la lista, p. 107 |
| 2 | Estados idénticos en las 4 listas | Sí | Sí | — |
| 3 | Clases del dominio en el diagrama de clases | **Con huecos** | Sí | Párrafo nuevo en el texto del diagrama de clases |
| 4 | Misma multiplicidad en dominio, clases y DER | **No, en 3 relaciones** | Sí: dos explicadas, una con el script 07 | Compra y sector: holgura del esquema explicada en el texto. Seguro: **se resuelve con el script 07** (decisión de JC, M-1) |
| 5 | Atributo de FK y línea a la vez | Sí | Sí | — |
| 6 | Toda 1:1 con UNIQUE en la base | **No, en 1 relación** | **Se resuelve con el script 07** | Índice único en `Seguros.ID_Entrada`, encargado a Codex para después de la mutación final |
| 7 | Composiciones con cascada o baja lógica | Sí (no hay composiciones) | Sí | — |
| 8 | Métodos de la secuencia en clases y código | **No, 8 llamadas** | Sí | Métodos agregados a las vistas 4 y 7; diagramas regenerados |
| 9 | Nombres consistentes (glosario) | **Con huecos** | Sí, salvo glosario | El mismo párrafo de la 3. El documento no tiene glosario |
| 10 | Devolución del Parcial 1 en la hoja de seguimiento | **Bloqueada** (no fallada) | **Bloqueada** (no fallada) | Sin la devolución no se puede comprobar. Juan Cruz la busca donde apareció la matriz |

### 1. Requerimiento core hasta el código

El core es RF-05, armado de paquetes (p. 46). Tiene CU-01, la robustez «Comprar paquete», la
secuencia en tres vistas, dos actividades, la transición de estados de la entrada y su código.
Lo que faltaba era la tabla que lo muestra: la matriz de la p. 107 va de RF a CU y a casos de
prueba, sin diagramas ni archivos. **Se agregó** la tabla «Trazabilidad del requerimiento core»
en la lista, después de la matriz principal. Los doce archivos que nombra existen.

### 2. Estados de la entrada

Las cuatro listas dicen lo mismo: Pendiente, Confirmada, Reembolsada y Vencida.

| Lugar | Dónde |
|---|---|
| Diagrama de transición | `estados-entrada.puml` (Confirmada tiene dos subestados, sin ingreso e ingresada, que son `FechaIngreso` y no un valor de `Estado`) |
| Clase | enum `EstadoEntrada` en la vista 3 de clases |
| Base | `CK_Entradas_Estado` (`01-esquema.sql` y `05-check-estado-entradas.sql`) |
| Código | `Entidade/EstadoEntrada.cs` |

### 3 y 9. Nombres del dominio frente a las clases

| Dominio actualizado | Diagrama de clases | ¿Explicado antes? |
|---|---|---|
| Administrador | `Propietario` | Solo en el texto de casos de uso |
| Inscripción | `ListaEspera` | No |
| Servicio contratado | `CompraServicio` | Sí, en el dominio actualizado |
| Paquete | `Paquete` (no se guarda en la base) | En parte |
| Suscripción | `Suscripcion` | Es la tilde: no hace falta |

**Se agregó** al texto del diagrama de clases (`docs/PARA-MAXIMO-DIAGRAMAS.md`, sección 2, y de
ahí a la lista) un párrafo con las tres equivalencias y la del Paquete. Con eso pasan la 3 y la 9.

**La 9 pide un glosario, y el documento no tiene.** Ese párrafo cumple la función para los nombres
que difieren, pero no es un glosario. Si JC quiere uno, es una sección nueva: no la agregué.

### 4 y 6. Multiplicidades de la entrada

Coinciden en todas las relaciones salvo tres, las tres de la entrada:

| Relación | Dominio | Clases | DER | Por qué |
|---|---|---|---|---|
| Entrada — Compra | 1 | 0..1 | 0..1 | `Entradas.ID_Compra` admite NULL: se agregó con `ALTER TABLE … NULL` a una tabla de entradas anterior a las compras |
| Entrada — Sector | 1 | 0..1 | 0..1 | Lo mismo con `ID_Sector` |
| Entrada — Seguro | 0..1 | 0..* | 0..* | `Seguros.ID_Entrada` no tiene UNIQUE. El código emite un seguro por entrada, una sola vez, y CP-122 prueba que no se agrega después |

**Decisión de JC del 17/11 (M-1), partida en dos:**

- **Compra y sector vacíos: se explican, no se cambian.** Alterar la obligatoriedad sobre datos
  existentes puede fallar en la base de Máximo, y el código nunca crea entradas huérfanas
  (verificado: `RepositorioEntradas.Insertar` es el único `INSERT INTO Entradas`, lo llama
  `RepositorioCompras.Iniciar` dentro de la transacción de la compra con su `idCompra`, y el
  sector sale de `GestorPaquetes.AgregarEntradas`, que siempre lo recibe). El texto del dominio
  actualizado lo dice como **una holgura del esquema cuya garantía está en el código**.
- **Dos seguros para una misma entrada: se arregla.** No es una holgura sino un defecto: un
  cliente podría quedar con dos cargos sobre una entrada, y toca RN-28. Codex agrega el índice
  único (script `07`) **después de la mutación final**. **Hecho en `ce4efdc`:** la oración de la
  p. 88 quedó sin marca, la vista 3 de clases y el entidad-relación de compras se regeneraron con
  0..1, y el script 07 está en los scripts de base del principio de la lista.

### 5. Clave foránea como atributo y como línea

Revisado con un script sobre las tres vistas de entidades y los dos dominios: ninguna clase lista
como atributo el identificador ni la colección que ya representa una asociación. El
entidad-relación muestra las columnas FK porque es su notación.

### 7. Composiciones

No hay composiciones en ningún diagrama (`docs/PARA-MAXIMO-DIAGRAMAS.md`, regla 3: el código no
tiene ciclo de vida dependiente entre entidades). La comprobación no tiene nada que revisar.

### 8. Métodos de la secuencia

Las tres vistas de la secuencia «Comprar paquete» llaman a 19 operaciones distintas. Todas existen en el
código, pero 8 no aparecían en ningún diagrama de clases:

| Llamada | Faltaba en | Arreglo |
|---|---|---|
| `GestorPaquetes.VerificarPreventa` | vista 4 (es `internal`) | `~ VerificarPreventa(int, int)` |
| `RepositorioCompras.Iniciar`, `ObtenerPorId`, `Confirmar`, `Liberar`, `Rereservar`, `MarcarVencidas` | vista 7 (los repositorios estaban sin métodos) | Agregados |
| `RepositorioEntradas.ObtenerPorCompra` | vista 7 | Agregado |
| `RepositorioPagos.Insertar` | vista 7 | Agregado |

Diagramas regenerados: `clases-4-negocio-compra` (16,0 × 18,3 cm, 8,5 pt) y `clases-7-datos`
(15,0 × 23,0 cm, 9,3 pt), los dos dentro del ancho de página. Al volver a correr la comprobación,
no falta ninguna.

### 10. Devolución del Parcial 1: bloqueada, no fallada

La «hoja de seguimiento» es el historial de revisión de la p. 6 (punto 13 de la guía). Lo que no
está es **la devolución del Parcial 1**: no aparece en el repositorio ni en el disco. Sin ella no
se puede comparar observación por observación. **Está bloqueada, no fallada:** nada indica que el
documento no la cumpla. Juan Cruz la busca en el mismo lugar donde apareció la matriz; cuando
aparezca, se corre.

## Encontrado de paso, fuera de la matriz

**Las tres capturas del guion de interfaz (pp. 47 a 49) eran de antes del rediseño.** La de la
p. 47 todavía decía «Show Password». Se corrigió el texto del Menú Inicial (Enter y los dos
enlaces), y Codex hizo las capturas nuevas en el cierre: están en la lista, a 16 cm, con la letra
más chica entre 12,2 y 14,9 pt.
