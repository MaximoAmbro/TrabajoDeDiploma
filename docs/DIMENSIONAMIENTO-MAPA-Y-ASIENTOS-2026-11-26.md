# Dimensionamiento: más eventos, mapa por zonas o asiento exacto (26/11/2026)

Informe de Claude Code a pedido de JC. **No hay código**: es para decidir qué entra antes del
congelamiento del 01/12. Las estimaciones son días de trabajo efectivo, sin optimismo. Quedan cuatro días:
del 27 al 30/11.

## Recomendación

**Antes del 01/12, solo A, que ya está hecha.** B no aporta casi nada sobre lo que ya existe y toca la
pantalla de compra que se está arreglando. C no entra: lleva más días de los que quedan y obliga a
rehacer diagramas y páginas del documento. Queda como trabajo futuro declarado.

| | A. 25 eventos con sectores | B. Mapa por zonas, sin cambiar la base | C. Asiento exacto en 10 eventos |
|---|---|---|---|
| Esquema | Ninguno | Ninguno | Tabla nueva, columna nueva, índice único filtrado y migración 17 |
| RN-38 | Sin cambio; la semilla lo comprueba | Sin cambio | Cambia: la capacidad de un sector numerado es su cantidad de asientos |
| Reserva de 10 min (RN-15) | Sin cambio | Sin cambio | Reserva un asiento, no solo un cupo; el vencimiento lo libera |
| Dos compradores, mismo lugar | Sin cambio: ya está resuelto por cupo | Sin cambio | Hay que resolverlo por asiento; se puede con el índice único |
| Pruebas nuevas | Ninguna obligatoria | 2 o 3 | 8 a 12 |
| Diagramas | Ninguno | Ninguno | DER y diagramas de clases 4, 5, 7 y 8d |
| Días de Claude Code | 0,5 hecho + 0,5 para recorrer el guion | 0,5 a 1 | 3 a 4 |
| Días de Codex | 0 | 1,5 a 2 | 6 a 8 |
| ¿Antes del 01/12? | **Sí** (ya está) | **No** | **No** |

## Lo que ya existe y cambia la cuenta

- **El cupo ya se reparte sin carreras.** `RepositorioSectores.Reservar` suma a `CantidadReservada` con un
  `UPDATE` condicionado al cupo libre, dentro de la transacción de la compra, y bloquea el sector con
  `UPDLOCK, HOLDLOCK` para la prioridad de la lista de espera. Dos compradores del mismo sector no pueden
  pasarse del cupo (RN-08 con RN-15).
- **El mapa por zonas ya está, como esquema orientativo.** La ficha y el armador web dibujan el escenario y
  los sectores ordenados por precio, del más caro, más cerca, al más barato
  (`Visual/Web/armador-paquete.js:68`). El respaldo WinForms tiene el suyo, `Visual/Componentes/MapaSectores.cs`,
  que dice en su comentario: «Esquema orientativo, no plano de butacas: no inventa geometría del local».
- **La semilla nueva (T-42) hace que ese mapa sea coherente:** en todos los eventos, el precio baja al
  alejarse del escenario, y la semilla lo comprueba.

## A. 25 eventos con sectores

**Hecha el 26/11** (T-42): `BaseDatos/semilla-demostracion.sql`. Tiene 25 eventos futuros de 7
organizadores, 9 sedes en 5 ciudades y un solo agotado. Detalle en `docs/TICKETS.md`, T-42, y en el guion,
sección 1.

- **Esquema, reserva, concurrencia:** sin cambios.
- **RN-38:** la semilla se detiene si un evento pasa la capacidad de su sede (error 51005).
- **Pruebas:** la batería no usa esta semilla. Una prueba que la cargue serviría para que un cambio de
  esquema no la rompa sin aviso, pero suma unos 20 s de base nueva a cada corrida. No la recomiendo antes
  del 01/12: la semilla ya se comprueba a sí misma.
- **Documento:** ningún diagrama. Las capturas finales (T-10), que se hacen igual después del
  congelamiento, van a mostrar el catálogo nuevo: `Pruebas/CapturarReforma.ps1` carga esta semilla. Las
  cuatro figuras del instructivo no cambian: `tools/CapturasInstructivo/Generar.ps1` no usa esta semilla.
- **Falta:** que Claude Code recorra el guion con la semilla nueva (medio día, con el escritorio libre).

## B. Mapa del estadio por zonas, sin cambiar la base

Lo que agregaría sobre lo que existe: un dibujo con forma de sala (herradura, laterales, pullman) en lugar
de franjas, con zonas que se pulsan para elegir el sector.

- **Esquema:** ninguno, pero tampoco hay de dónde sacar la forma. La base no guarda geometría: la forma
  saldría del nombre del sector («Palcos», «Platea alta»), y eso es inventar la sala. Es justo lo que el
  mapa actual evita a propósito.
- **RN-38, reserva de 10 min, concurrencia:** sin cambios. El mapa elige un sector y la compra sigue igual.
- **Pruebas nuevas:** 2 o 3. Pulsar una zona elige su sector; una zona agotada no se puede elegir; el orden
  por precio se mantiene. Más la geometría en «Mi cuenta», como CP-357.
- **Documento:** ningún diagrama. Cambian las páginas y capturas de la ficha y del armador (T-10 y T-11).
- **Días:** Codex, 1,5 a 2, porque son la vista web y el respaldo WinForms. Claude Code, 0,5 a 1, para
  verificar sobre el ejecutable y actualizar guion y páginas.
- **Riesgo:** toca el armador, que es el recorrido de compra de la demostración y lo que T-26 está
  arreglando ahora. Una regresión ahí deja en «no» los renglones 1 y 2 del congelamiento.

**Por qué no:** suma poco sobre el mapa orientativo actual y arriesga la pantalla más importante de la
defensa, con cuatro días y la lista de Codex ya llena.

## C. Asiento exacto (fila y número) en 10 eventos

### Esquema
- Tabla `Asientos (ID, SectorID, Fila, Numero)`, con `UNIQUE (SectorID, Fila, Numero)`.
- `Sectores.Numerado BIT NOT NULL DEFAULT 0`: los sectores sin numerar (campo, general) siguen como hoy.
- `Entradas.ID_Asiento INT NULL`, con clave foránea a `Asientos`.
- **Índice único filtrado:** `UNIQUE (ID_Asiento) WHERE ID_Asiento IS NOT NULL AND Estado IN ('Pendiente','Confirmada')`.
  Una entrada vencida o reembolsada deja el asiento libre sin tocar nada más.
- Migración `17-asientos.sql`, y la semilla de demostración genera los asientos de 10 eventos: unos miles
  de filas.

### RN-38 y una regla nueva
- RN-38 sigue igual: la suma de los sectores no pasa la capacidad del local.
- **Regla nueva:** en un sector numerado, la capacidad es la cantidad de asientos. Cambiar la capacidad de
  un sector con ventas obliga a decidir qué pasa con los asientos vendidos: lo más simple es prohibirlo y
  declararlo.

### Reserva de 10 minutos (RN-15)
Hoy la reserva suma al contador del sector y crea las entradas `Pendiente`. Con asientos, cada entrada
`Pendiente` lleva su `ID_Asiento`:
- **vence:** `LiberarVencidas` las pasa a `Vencida` y el índice filtrado libera el asiento solo;
- **pago rechazado (RN-32) o devolución:** igual, por el estado;
- **prioridad de la lista de espera:** sigue siendo por sector (`CupoLiberadoEl`). El asiento liberado queda
  reservado para suscriptores esos 15 minutos, como el cupo. No cambia la regla.

### Dos compradores, el mismo asiento
Los dos lo ven libre y lo eligen. El primero inserta su entrada `Pendiente` con el asiento. Al segundo, el
índice único le rechaza el `INSERT` (error 2601) dentro de su transacción, y se deshace todo, incluido el
contador. `Datos` traduce ese error a una `ReglaNegocioException` del tipo «Ese asiento lo acaba de reservar
otra persona. Elegí otro.», y el mapa se recarga. No hace falta un bloqueo nuevo: lo garantiza el índice.
**Hay que probarlo con dos conexiones en paralelo,** no secuencial.

### Pruebas nuevas (8 a 12)
- el asiento elegido queda en la entrada y en el ticket PDF;
- dos compras simultáneas del mismo asiento: una confirma y la otra recibe el mensaje;
- la reserva vencida libera el asiento;
- el pago rechazado y la devolución lo liberan;
- un sector numerado se agota cuando se venden todos sus asientos (RN-10);
- la capacidad de un sector numerado es igual a su cantidad de asientos;
- el control de ingreso muestra el asiento;
- el mapa de asientos en la vista web y en el respaldo WinForms, dentro de «Mi cuenta».

### Documento
- **Reglas y requerimientos:** dos reglas nuevas y la redacción de RF-06.
- **Diagramas:** el DER, el diccionario de datos (`docs/03-esquema-datos.md`) y los diagramas de clases 4, 5,
  7 y 8d (T-12, que ya está pendiente, se agranda).
- **Casos y matriz:** las 8 a 12 filas nuevas y la matriz de trazabilidad.
- **Páginas y capturas:** el instructivo de compra (el ejemplo de la p. 54 suma el paso del asiento), las
  páginas de pantallas de la ficha, el armador, el ticket y el control de ingreso (T-11), y sus capturas
  (T-10).

### Días
- **Codex: 6 a 8.** Migración y semilla, `Datos` y `Repo`, la grilla de asientos en la web y en el respaldo
  WinForms, el PDF y el QR, el control de ingreso, y las pruebas.
- **Claude Code: 3 a 4.** Reglas, RF-06, DER, cuatro diagramas, catálogo de casos, matriz, páginas, guion y
  la verificación sobre el ejecutable.

**Por qué no:** son más días que los cuatro que quedan, y la lista de Codex antes del congelamiento ya está
llena: T-22, T-26, T-27, T-28, T-31, T-36, T-38 a T-41 y T-43. Además, del 01/12 al 10/12 suma unos 3 días
de documento a un período que ya tiene la batería final, las capturas, las páginas y los diagramas. Se
puede declarar en el documento como **trabajo futuro**, con el diseño de esta sección.

## Cómo se calcularon los días

- Por lo que tocan, no por analogía. A y B no cambian datos ni reglas. C cambia esquema, dos capas, dos
  interfaces, el PDF y el documento.
- Cada pantalla web de este proyecto lleva también su respaldo WinForms (T-03). Eso duplica la interfaz de
  B y de C.
- Un día de Codex incluye la batería completa. Un día de Claude Code incluye verificar sobre el ejecutable,
  que necesita el escritorio sin Codex.
