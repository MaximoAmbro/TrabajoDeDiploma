# Anexos técnicos T01 a T08 — mapa para el caso de que sean obligatorios

## Re-estimación del 10/11/2026: los anexos como documentos

**JC corrigió el supuesto.** La tabla original (más abajo) calculaba 100 a 144 h de
**implementar** cada anexo. El trabajo se entrega con simulaciones, habladas con el docente.
Los anexos se calculan ahora como **documentos verosímiles**:
- **lo que ya está implementado** se describe sobre el código real;
- **lo que no está implementado** se presenta como diseño, con el rótulo «Diseño propuesto (no
  implementado en el prototipo)» (`CORRECCIONES-DOCUMENTO.md`, ítem 24).

**Todas las horas de esta sección son estimadas.**

**⚠ Inconcluso, y hay que preguntárselo al docente con estas palabras.** La Carpeta Modelo
dice que cada anexo «deberá documentarse **y desarrollarse**» (sección T00). Lo que está
hablado con el docente, según lo escrito, son las minutas y el estudio de usabilidad. No
encontré nada escrito que diga que los anexos técnicos pueden entregarse como diseño sin
implementar. Si el docente exige desarrollarlos, vuelve a valer la tabla original.

**Qué cambió en el código desde el 04/11**, y pasa de diseño a descripción de lo real:
- **T02:** el logout existe («Volver al inicio» cierra la sesión, T-23, CP-46);
- **T02:** la aplicación termina al cerrar la última ventana (T-24, CP-47);
- **T02:** hay un manejador global de errores (T-27).

| Anexo | Real (sobre el código) | Diseño (rotulado) | Claude Code | JC |
|---|---|---|---|---|
| T01 Arquitectura | Las 4 capas; diagrama de componentes; persistencia (`Conexion`, transacciones, traducción de errores a reglas); mapa de navegación de las 19 pantallas con código jerárquico | Por qué no es MDI | 4 a 6 h | 0,5 h |
| T02 Login / logout | Login con PBKDF2 y bloqueo; logout; cierre; manejador de errores | Auditoría (depende de T06) y verificación de integridad al iniciar (depende de T08) | 3 a 4 h | 0,5 h |
| T03 Encriptado | Hash PBKDF2-HMAC-SHA256, salt, 100.000 iteraciones; la tarjeta no se guarda (RN-30) | Cifrado AES del token de pago, con la clave protegida por DPAPI | 3 a 4 h | 0,5 h |
| T04 Perfiles | Los dos perfiles fijos y RN-34, como punto de partida | Composite de permisos: clases, DER, secuencia y árbol de ejemplo | 5 a 7 h | 1 h |
| T05 Idiomas | — (el documento lo excluye, W-05) | Observer: clases, DER de idiomas y leyendas, secuencia del cambio. **JC decide si se hace o se mantiene la exclusión** | 3 a 5 h | 0,5 h |
| T06a y T06b | El registro de pagos (RN-31) como antecedente | Bitácora con búsqueda combinada; historial de una entidad con restauración | 4 a 6 h | 1 h |
| T07 Backup | El backup y el `RESTORE VERIFYONLY` hechos el 04/11, que sí ocurrieron | Catálogo, pantalla y restauración desde la aplicación | 3 a 4 h | 0,5 h |
| T08 Dígitos verificadores | — | DVH y DVV con un **ejemplo calculado de verdad** sobre filas de la semilla, rotulado como ejemplo | 4 a 6 h | 0,5 h |
| Armado | Pasar los ocho a Word con el formato del documento y agregarlos con pypdf | — | 1 h | 2 a 3 h |
| **Total estimado** | | | **30 a 43 h** | **7 a 9 h** |

**Total: 37 a 52 horas estimadas**, contra 100 a 144. El subconjunto mínimo (T01, T02, T03 y
T06a) baja a unas **16 a 22 horas estimadas**.

**Los diagramas (inconcluso).** En esta PC no hay PlantUML, Java ni Graphviz. Sí está Node,
y con él se puede usar Mermaid (`npx @mermaid-js/mermaid-cli`), que descarga un navegador. No
lo instalé: hace falta tu permiso. Sin herramienta, los diagramas se dibujan a mano, con 4 a
8 h estimadas más de JC.

---

# Mapa original del 04/11/2026 (los anexos como implementación)

Preparado el 04/11/2026, mientras se espera la respuesta del docente. **No se armó ningún
anexo**: esto es el relevamiento de qué pide cada uno, qué existe hoy en el repositorio y
cuánto falta.

## Qué pide la Carpeta Modelo

Para **cada** anexo, la Carpeta Modelo (sección T00) dice que «deberán documentarse **y
desarrollarse**», con estas partes:

- objetivo;
- descripción detallada de cómo funciona;
- diagrama de clases;
- DER, si es necesario;
- secuencia, si es necesario.

No alcanza con documentar: hay que implementarlo.

El documento entregable (versión 2.4) los deja como trabajo futuro (página 111, punto 3).
Además excluye de forma explícita el T05 (W-05: «Interfaz en múltiples idiomas: corresponde al
anexo técnico T05, no implementado en la presente entrega»).

## Mapa

Horas realistas para dos personas que ya conocen el código, incluidas pruebas y documento.
El código va a la zona de Codex (`Visual/`, `Repo/`, `Datos/`, `Pruebas/`) y el esquema a
`BaseDatos/`. La documentación es de Claude Code.

| Anexo | Qué pide | Qué existe hoy | Qué falta | Origen | Horas |
|---|---|---|---|---|---|
| **T01 Arquitectura base** | Arquitectura de 4 capas como mínimo y documentada; esquema de persistencia detallado (consulta y actualización); diagrama de componentes; mapa de navegación de todas las pantallas con código jerárquico; criterios de POO. MDI recomendado | **Implementada.** Cuatro capas con reglas de dependencia (CLAUDE.md), Singleton en gestores y repositorios, todo el SQL parametrizado en `Datos/`, transacciones en `RepositorioCompras`. El PDF ya tiene diagramas de clases, secuencia, robustez, dominio y DER (páginas 71 a 89). La navegación entre las 19 pantallas está relevada (`docs/usabilidad/medicion.txt`) | Solo documentación: diagrama de componentes (5 ensamblados y sus dependencias), descripción de la persistencia (`Conexion`, `RepositorioBase`, transacciones, traducción de errores SQL a reglas) y mapa de navegación con códigos. No es MDI: justificarlo (recomendado, no obligatorio) | **Material existente** | **6 a 8** |
| **T02 Log in / Log out** | Política de login y logout; procesos de arranque, login, apagado y logout (permisos, auditoría, integridad); Singleton | **Login implementado:** PBKDF2, bloqueo por intentos (RN-04), `GestorSesion` Singleton. Probado en CP-27 a CP-29 | **No hay logout:** ninguna pantalla llama a `GestorSesion.Cerrar()`, y «Volver al inicio» deja la sesión abierta. No hay proceso de arranque (verificar la base, integridad) ni de apagado: la aplicación ni siquiera termina al cerrar una pantalla (U-01 de usabilidad). Falta la auditoría, que depende de T06 | Mitad existente, mitad nuevo | **6 a 8** (más T06 y T08 para la auditoría y la integridad) |
| **T03 Encriptado** | Algoritmo simétrico o asimétrico para datos sensibles, y hash para contraseñas | **Hash implementado:** PBKDF2-HMAC-SHA256, 100.000 iteraciones, salt por usuario (`Repo/Seguridad.cs`). La tarjeta no se guarda (RN-30) | **Falta el cifrado** de un dato sensible. El candidato natural es el token de pago, o el mail y los datos personales, con AES y la clave protegida con DPAPI (`ProtectedData`, que ya está entre las dependencias). Incluye migrar los datos existentes | Mitad existente | **8 a 12** |
| **T04 Perfiles de usuario** | Permisos atómicos y compuestos con código único; patrón Composite; árbol de permisos en TreeView con funciones recursivas; perfiles definibles desde el sistema | **Nada.** Hay dos perfiles fijos por texto (`TipoUsuario`: Cliente o Propietario, RN-05), y los controles comparan ese texto (por ejemplo, RN-34 en reportes) | Todo: entidades (permiso, familia, patente), tablas y DER, gestor, pantalla con TreeView, y reemplazar cada comprobación de `TipoUsuario` por una de permisos. Toca la autorización de toda la aplicación | **Desde cero** | **20 a 30** |
| **T05 Múltiples idiomas** | Cambio de idioma dinámico de todas las leyendas; alta de idiomas y leyendas desde el sistema; patrón Observer; sin archivos de recursos estáticos | **Nada.** El documento lo excluye (W-05) | Todo: tablas de idioma y leyendas, gestor observable, y los **19 formularios** como observadores, con cada texto pasado a una clave. Es el anexo que más pantallas toca | **Desde cero** | **20 a 30** |
| **T06a Bitácora** | Registro de toda operación de usuario (fecha, hora, usuario, actividad, datos); búsqueda combinada | **Nada.** Lo más parecido es el registro de pagos (RN-31) | Tabla, gestor Singleton, llamadas en cada operación (login, logout, compra, devolución, eventos, reportes) y pantalla de búsqueda con filtros combinados | **Desde cero** | **10 a 14** |
| **T06b Control de cambios** | Historial de una entidad elegida: quién, cuándo y qué; recomponer un estado anterior | **Nada.** `Compras` congela importes (RN-16), pero eso no es un historial | Elegir la entidad (Eventos o Sectores, por los cambios de precio y de fecha), tabla de historial, registro en cada modificación y pantalla para ver y restaurar | **Desde cero** | **8 a 10** |
| **T07 Backup** | Administrar copias de seguridad: catálogo y archivos físicos | **Nada en la aplicación.** El 04/11 se hizo un backup a mano con `BACKUP DATABASE` y funcionó en SQL Server Express | Tabla catálogo, gestor que ejecute el backup y la restauración con SQL, y pantalla para crear, listar y restaurar | **Desde cero** | **8 a 12** |
| **T08 Dígitos verificadores** | DVH por registro y DVV por tabla, con un algoritmo que use posición de carácter y de atributo; verificación al iniciar, antes del login; aviso al administrador; restauración; tabla DVV en el DER | **Nada** | Elegir las tablas sensibles (Usuario, Compras, Entradas, Pagos), columna DVH, tabla DVV, recálculo en cada escritura de esos repositorios, verificación en el arranque (T02) y procedimiento de recálculo autorizado. Toca todas las escrituras de esas tablas | **Desde cero** | **14 a 20** |
| | | | | **Total** | **100 a 144 h (25 a 36 pt)** |

## En qué orden los haría

1. **T01**: es solo documentación sobre lo que ya funciona. Es la base que citan los demás.
2. **T02**: el login ya está; agregar el logout y los procesos de arranque y apagado, que
   arregla de paso el hallazgo U-01. Deja los «ganchos» para T06 y T08.
3. **T03**: el hash ya está; falta un cifrado acotado, sin dependencias con otros anexos.
4. **T06a y T06b**: la bitácora la usan T02 (auditoría), T07 y T08 (registrar backups y
   errores de integridad).
5. **T07**: es independiente y además sirve de red para T08 (restaurar si la integridad falla).
6. **T08**: va después de que el esquema esté estable, porque cada cambio de tabla obliga a
   recalcular los dígitos.
7. **T04**: es grande y cambia la autorización de toda la aplicación. Conviene hacerlo con
   todo lo demás terminado y probado.
8. **T05**: toca las 19 pantallas y el documento lo excluyó. Va último, o se negocia.

## Lectura para decidir

- **T01, T02 y T03 se resuelven con material existente**, en unas 20 a 28 horas entre los
  tres.
- **T04 a T08 son desarrollo nuevo**, de 80 a 116 horas. A un mes de la entrega, con dos
  personas y el resto de la lista, no entran todos.
- **Si el docente los pide, conviene preguntarle si alcanza con un subconjunto.** El orden de
  arriba propone uno: T01, T02, T03 y T06a, unas 30 a 42 horas, que cubre arquitectura,
  seguridad y auditoría.

## Dependencias con el resto del proyecto

- **T02 y el hallazgo U-01** (la aplicación no termina al cerrar una pantalla) se resuelven
  juntos.
- **T04 reemplaza el control de RN-34**, que hoy compara texto. También cierra el problema
  de seguridad del alta, donde cualquiera puede elegir «Propietario»: con permisos, el alta
  pública crearía solo clientes.
- **T08 y T03 agregan columnas al esquema:** hay que regenerar `01-esquema.sql`, la semilla y
  las pruebas que arman datos con SQL directo (`Pruebas/Fake.cs`).
