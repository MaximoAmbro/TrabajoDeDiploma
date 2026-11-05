# Tareas para Codex — pruebas CP-26 a CP-42 y etiquetado de CP-11 a CP-25

Encargo cerrado. Objetivo: cerrar las brechas G-02 (RF sin pruebas) y G-04 (hallazgos sin
regresión) de la matriz de trazabilidad (hoy en `docs/CORRECCIONES-DOCUMENTO.md`, ítems 17 a 21).
Base: commit `f550199`. Batería actual: 31 ejecuciones, todas correctas.

## Reglas de esta tanda

1. **Solo se modifica `Pruebas/`.** No se toca `Visual/`, `Repo/`, `Datos/` ni `Entidade/`,
   aunque una prueba encuentre un defecto.
2. **Si una prueba falla porque el código no cumple la regla, no se corrige el código ni se
   ajusta la prueba al resultado.** La prueba se deja escrita con lo que dice la regla, se
   marca `[Ignore("Hallazgo: <qué falla>")]` y se informa en el reporte final con el mensaje
   de error. JC decide si se corrige.
3. Todo dato de prueba se crea con `Fake` o con helpers nuevos dentro de `Fake.cs`, y queda
   cubierto por `Fake.Limpiar()`:
   - correos `test…@pruebas.eventaura.com`;
   - locales y servicios con nombre que empieza con «test ».
   No se modifican datos de la semilla.
4. Los valores de las reglas se leen con `GestorParametros.Instance.ObtenerEntero(...)`, nunca
   escritos en la prueba. Parámetros que se usan: `LOGIN_INTENTOS_MAX`, `LOGIN_BLOQUEO_MIN`,
   `COMPRA_ENTRADAS_MAX`, `RESERVA_MINUTOS`, `FIDELIDAD_UMBRAL_PLATA` y
   `FIDELIDAD_UMBRAL_ORO`.
5. Cada clase nueva repite el ciclo de `PruebasFidelizacion`:
   - `[TestInitialize]`: `Fake.Limpiar()` y `GestorSesion.Instance.Cerrar()`;
   - `[TestCleanup]`: `GestorSesion.Instance.Cerrar()`;
   - `[ClassCleanup]`: `Fake.Limpiar()`.
6. Cada prueba lleva:
   - un comentario `// CP-xx — RF-yy, RN-zz: <qué verifica>` arriba del método;
   - el ID en el nombre visible: `[TestMethod("CP-xx: …")]`, o `DisplayName` en cada
     `DataRow`.
   (Corregido después de la tanda: el encargo original decía
   `[TestMethod(DisplayName = …)]`, que no compila con MSTest 3.6.4 porque en esa versión la
   propiedad es de solo lectura. Codex usó el constructor, que es lo correcto.)
7. Se corre con `tools\Proyecto.ps1 -Accion Probar`. Al terminar no puede haber errores ni
   advertencias nuevas de compilación.
8. Sin commit y sin push. El reporte final lista los archivos tocados para que JC revise
   `git status`.

## Helpers que se agregan a `Pruebas/Fake.cs`

Todos con SQL parametrizado, en el mismo estilo que los existentes:

| Helper | Qué hace |
|---|---|
| `CambiarFechaDeEvento(int idEvento, DateTime fecha)` | `UPDATE Eventos SET Fecha = @Fecha WHERE ID = @ID` |
| `CambiarEstadoDeEvento(int idEvento, string estado)` | `UPDATE Eventos SET Estado = @Estado WHERE ID = @ID` |
| `RetrasarInicioDeCompra(int idCompra, int minutos)` | `UPDATE Compras SET FechaCompra = DATEADD(MINUTE, -@Minutos, FechaCompra) WHERE ID = @ID` |
| `VencerBloqueo(string mail)` | `UPDATE Usuario SET BloqueadoHasta = @Pasado WHERE Mail = @Mail`, con `@Pasado = DateTime.Now.AddMinutes(-1)` |
| `Administrador()` | Devuelve el primer `Usuario` con `TipoUsuario = 'Propietario'` de la semilla (el mismo que usa `EventoPublicado`). Solo lectura |
| `MailNuevo()` | Devuelve un correo `test{n:0000}@pruebas.eventaura.com` sin usar, con el mismo contador |

---

## T-01. Etiquetar los casos existentes CP-11 a CP-25

Agregar el comentario y el `DisplayName` de la regla 6 a los métodos que ya existen. No se
cambia ninguna aserción.

| CP | Archivo | Método | Comentario |
|---|---|---|---|
| CP-11 | PruebasFidelizacion.cs | `Referencia_DaExactamente99625` | RF-05, RF-08 — RN-20, RN-21 |
| CP-12 | PruebasFormularios.cs | `RNF03_NingunFormularioPasaDe790DeAlto` | RNF-03 |
| CP-13 | PruebasInterfaz.cs | `Compra_PaqueteMaximoYNombreLargoSinRecortes` | RF-05 — RN-10 — H-01 |
| CP-14 | PruebasInterfaz.cs | `Registro_AbreSinDatosPrecargados` | RF-01 |
| CP-15 | PruebasInterfaz.cs | `Registro_RechazaDatosInvalidosSinCrearUsuario` (4 filas) | RF-01 — RN-02 |
| CP-16 | PruebasInterfaz.cs | `Eventos_SinResultadosNoPermiteAgregarFilas` | RF-03 |
| CP-17 | PruebasInterfaz.cs | `Eventos_ComprarSinResultadosMuestraAvisoCorrecto` | RF-03 |
| CP-18 | PruebasInterfaz.cs | `Eventos_GuiaRegistradaConFocoEnLaBusqueda` | RF-03 |
| CP-19 | PruebasInterfaz.cs | `Compra_VolverConfirmaSinPerderPaqueteAlCancelar` | RF-05 — H-06 |
| CP-20 | PruebasInterfaz.cs | `Compra_CantidadInvalidaRestauraElPaquete` (2 filas) | RF-04 |
| CP-21 | PruebasInterfaz.cs | `Propietario_EliminarRequiereConfirmacionYPreservaVentas` | RF-12 — RN-11 |
| CP-22 | PruebasInterfaz.cs | `Historial_SinSeleccionDistingueBotonDeshabilitado` | RF-05 |
| CP-23 | PruebasInterfaz.cs | `Historial_TildarYDestildarActualizaEstadoYColor` | RF-05 |
| CP-24 | PruebasInterfaz.cs | `Compra_CupoLiberadoVuelveAHabilitarServiciosDisponibles` | RF-05, RF-10 — RN-10, RN-14 |
| CP-25 | PruebasInterfaz.cs | `Compra_EscalaSimuladaMantieneAccionesDentroDelArea` (3 filas) | RF-05, RNF-03 |

---

## Archivo nuevo `Pruebas/PruebasCuentas.cs` — RF-01 y RF-02

### T-02. CP-26 — Registro: nace Bronce, sin suscripción, con correo único (RF-01; RN-01, RN-03)

1. `mail = Fake.MailNuevo()`.
2. `GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "Cliente")`.
3. Leer el `id` con `Datos.RepositorioUsuarios.Instance.ObtenerPorMail(mail).Id`.
4. Comprobar:
   - `GestorFidelizacion.Instance.ObtenerCliente(id).Nivel == NivelFidelidad.Bronce`;
   - `GestorSuscripciones.Instance.ActivaDe(id) == null`.
5. Volver a registrar el mismo `mail`: tiene que lanzar `ReglaNegocioException` con
   `Regla == "RN-01"`.

### T-03. CP-27 — Inicio de sesión correcto (RF-02)

1. Registrar un usuario como en T-02.
2. `u = GestorUsuarios.Instance.Autenticar("  " + mail + "  ", "Cliente123")`, con espacios
   alrededor del correo.
3. Comprobar:
   - `u != null`;
   - `u.TipoUsuario == "Cliente"`;
   - `u.Contraseña == null`;
   - `GestorSesion.Instance.UsuarioActual.Id == u.Id`.

### T-04. CP-28 — Bloqueo tras los intentos fallidos (RF-02; RN-04)

1. Registrar un usuario como en T-02. Leer `N = LOGIN_INTENTOS_MAX` y
   `M = LOGIN_BLOQUEO_MIN`.
2. Para `i = 1 … N−1`: `Autenticar(mail, "Incorrecta1")` lanza `ReglaNegocioException` con
   `Regla == "RN-04"` y un mensaje que contiene `"Le quedan " + (N − i)`.
3. Intento `N`: lanza RN-04 y el mensaje contiene `"bloqueada"`.
   `ObtenerPorMail(mail).BloqueadoHasta` está entre `ahora + (M − 1)` y `ahora + M` minutos.
4. `Autenticar(mail, "Cliente123")`, con la contraseña correcta: también lanza RN-04 con
   `"bloqueada"`.
5. `Fake.VencerBloqueo(mail)`. Ahora `Autenticar(mail, "Cliente123")` devuelve el usuario y
   `ObtenerPorMail(mail)` tiene `IntentosFallidos == 0` y `BloqueadoHasta == null`.

### T-05. CP-29 — Datos faltantes o correo inexistente (RF-02)

Cada uno de estos casos devuelve `null` sin lanzar excepción:
- `Autenticar(Fake.MailNuevo(), "Cliente123")`;
- `Autenticar("", "Cliente123")`;
- `Autenticar(mailRegistrado, "")`.

Después de los tres: `GestorSesion.Instance.HaySesion == false`.

---

## Archivo nuevo `Pruebas/PruebasCompra.cs` — RF-03, RF-04, RF-06 y RF-07

### T-06. CP-30 — Filtros de búsqueda y evento no vigente (RF-03; RN-09)

1. `e1 = Fake.EventoPublicado(1000m, 10)`. Es categoría «Concierto», fecha hoy + 30 días y
   local en «Calle de prueba 123».
2. Con `ids(lista) = lista.Select(x => x.Id)`, comprobar sobre
   `GestorEventos.Instance.Buscar(...)`:
   - `Buscar(e1.Nombre, null, null)` contiene a `e1.Id` (por nombre);
   - `Buscar("Calle de prueba", null, null)` contiene a `e1.Id` (por ubicación);
   - `Buscar(null, "Concierto", null)` contiene a `e1.Id`;
   - `Buscar(null, otra, null)` **no** contiene a `e1.Id`, siendo `otra` la primera de
     `GestorEventos.Categorias` distinta de «Concierto»;
   - `Buscar(null, null, DateTime.Today.AddDays(29))` contiene a `e1.Id`;
   - `Buscar(null, null, DateTime.Today.AddDays(31))` **no** contiene a `e1.Id`.
3. `e2 = Fake.EventoPublicado(...)` y luego `Fake.CambiarFechaDeEvento(e2.Id,
   DateTime.Now.AddDays(-1))`. Con un cliente `c = Fake.Cliente(Bronce)`:
   - `Buscar(e2.Nombre, null, null)` no contiene a `e2.Id`;
   - `GestorPaquetes.Instance.Nuevo(e2.Id, c.Id)` lanza RN-09;
   - `GestorListaEspera.Instance.Inscribir(e2.Id, c.Id)` lanza RN-09.
4. `e3 = Fake.EventoPublicado(...)` y luego `Fake.CambiarEstadoDeEvento(e3.Id,
   GestorEventos.EstadoSuspendido)`. `Buscar(e3.Nombre, null, null)` no contiene a `e3.Id`.

### T-07. CP-31 — Topes del paquete (RF-04; RN-08, RN-12, RN-13)

1. `e = Fake.EventoPublicado(1000m, 3)`. Su sector «Campo» es el chico.
   `grande = Fake.AgregarSector(e, 1000m, 100)`. `c = Fake.Cliente(Bronce)`.
2. **RN-08:** sobre `p = GestorPaquetes.Instance.Nuevo(e.Id, c.Id)`, `AgregarEntradas(p,
   chico, 4)` lanza RN-08.
3. **RN-12:** con `max = COMPRA_ENTRADAS_MAX`, sobre un paquete nuevo:
   - `AgregarEntradas(p, grande, max)` no lanza nada;
   - después, `AgregarEntradas(p, chico, 1)` lanza RN-12.
4. **RN-13:** sobre un paquete nuevo, `s = Fake.ServicioHabilitado(e, "Hospedaje", 1000m)` y
   `AgregarServicio(p, s.Id)`. Después, `GestorPaquetes.Instance.Validar(p)` lanza RN-13.

### T-08. CP-32 — Pago aprobado: confirmación, QR y registro del pago (RF-06, RF-07; RN-17, RN-18, RN-30, RN-31)

1. `c = Fake.Cliente(Bronce)` y `e = Fake.EventoPublicado(1000m, 10)`.
2. Un paquete con 3 entradas en el sector del evento.
   `compra = GestorCompras.Instance.Iniciar(p, c)`.
3. `GestorCompras.Instance.Confirmar(compra, Fake.Tarjeta())` devuelve `true`.
4. Comprobar:
   - `ObtenerPorId(compra.Id).Estado == EstadoCompra.Confirmada`;
   - `EntradasDe(compra)` tiene 3 entradas; cada `CodigoQR` cumple
     `^EA-[0-9A-F]{32}$` y los 3 son distintos;
   - `PagosDe(compra)` tiene un solo pago, con `Estado == EstadoPago.Aprobado`,
     `Ultimos4 == "1111"` y `Token` que empieza con `"tok_sim_"`;
   - ninguna propiedad `string` del pago (recorrerlas por reflexión) contiene
     `"4111111111111111"`.

### T-09. CP-33 — Pago rechazado: libera el cupo y queda registrado (RF-06, RF-07; RN-17, RN-31, RN-32)

1. `e = Fake.EventoPublicado(1000m, 1)` y dos clientes, `c1` y `c2`.
2. `c1` inicia una compra de 1 entrada. `DisponiblesDe(sector) == 0`.
3. `Confirmar(compra, tarjeta)` con `tarjeta = Fake.Tarjeta()` y
   `Numero = "4111 1111 1111 0000"` devuelve `false`.
4. Comprobar:
   - `ObtenerPorId(compra.Id).Estado == EstadoCompra.Pendiente`;
   - `PagosDe(compra)` tiene un pago con `Estado == EstadoPago.Rechazado` y
     `Ultimos4 == "0000"`;
   - `DisponiblesDe(sector) == 1`;
   - `c2` puede armar e iniciar su propia compra de 1 entrada sin excepción.

### T-10. CP-34 — Reserva vencida (RF-06; RN-15)

1. `e = Fake.EventoPublicado(1000m, 5)` y un cliente que inicia una compra de 2 entradas.
   `DisponiblesDe(sector) == 3`.
2. `Fake.RetrasarInicioDeCompra(compra.Id, RESERVA_MINUTOS + 1)`.
3. `Confirmar(compra, Fake.Tarjeta())` lanza `ReglaNegocioException` con
   `Regla == "RN-15"`.
4. Comprobar:
   - `ObtenerPorId(compra.Id).Estado == EstadoCompra.Vencida`;
   - `DisponiblesDe(sector) == 5`;
   - `PagosDe(compra)` está vacío: no se llegó a cobrar.

---

## Archivo nuevo `Pruebas/PruebasReportes.cs` — RF-11

Período de todas las pruebas: `desde = DateTime.Today` y
`hasta = DateTime.Today.AddDays(31)`. Los eventos de `Fake` caen dentro, a 30 días. Cada
prueba abre la sesión de administrador con
`GestorSesion.Instance.Iniciar(Fake.Administrador())`, salvo T-11.

### T-11. CP-35 — Solo el Administrador accede (RF-11; RN-34)

- Sin sesión, `GestorReportes.Instance.Crear(ReporteVentasPorEvento.CodigoReporte, desde,
  hasta)` lanza `ReglaNegocioException` con `Regla == "RN-34"`.
- Con sesión de `Fake.Cliente(Bronce)`, abierta con `GestorSesion.Instance.Iniciar`, la misma
  llamada lanza RN-34.
- Con la sesión de `Fake.Administrador()`, la misma llamada devuelve un `Reporte`.
  `Ejecutar` no lanza nada.

### T-12. CP-36 — RPT-01 cuenta solo las entradas confirmadas (RF-11)

1. `e = Fake.EventoPublicado(25000m, 10)`.
2. Tres compras sobre el evento:
   - `Fake.CompraConfirmada(c1, e, 1, false)`;
   - `Fake.CompraConfirmada(c2, e, 2, false)`;
   - un tercer cliente que solo **inicia** una compra de 1 entrada, sin confirmarla.
3. `tabla = Ejecutar(Crear("RPT-01", desde, hasta))`. Tomar la fila cuyo `"Evento"` es
   `e.Nombre`.
4. Comprobar los valores de la fila:
   - `"Entradas" == 3`;
   - `"Ingreso bruto" == 75000m`;
   - `"Ticket prom." == 25000m`;
   - `"% ocup." == 30.0m`.

### T-13. CP-37 — RPT-05: agotado derivado y lista de espera (RF-11, RF-10; RN-10, RN-36)

1. `e = Fake.EventoPublicado(1000m, 1)`.
   `compra = Fake.CompraConfirmada(c1, e, 1, true)`, con seguro.
2. `GestorListaEspera.Instance.Inscribir(e.Id, c2.Id)`. Una segunda inscripción de `c2`
   lanza RN-36.
3. Con `r = (ReporteDemandaInsatisfecha)Crear("RPT-05", desde, hasta)`, `r.IdEvento = e.Id`
   y `r.NombreEvento = e.Nombre`, la fila del evento tiene:
   - `"Estado" == ReporteDemandaInsatisfecha.EstadoAgotado`;
   - `"En lista de espera" == 1`.
4. `GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(compra))`. El mismo
   reporte, ejecutado de nuevo, da `"Estado" == EstadoConCupo`. Con `r.SoloAgotados = true`,
   la tabla no tiene fila para `e.Nombre`.

### T-14. CP-38 — Exportación y período inválido (RF-11)

- Una clase privada `ExportadorDePrueba : IExportador` guarda la `DataTable` recibida y
  devuelve la ruta que le pasan.
- `GestorReportes.Instance.Exportar(r, exportador, "x.pdf")`, con RPT-01 sobre un evento con
  una compra confirmada, devuelve `"x.pdf"`. La tabla recibida tiene la misma cantidad de
  filas que `Ejecutar(r)`.
- `Crear("RPT-01", hasta, desde)`, con las fechas invertidas, lanza `ArgumentException`.
- `Crear("RPT-99", desde, hasta)` lanza `ArgumentException`.

---

## En `Pruebas/PruebasInterfaz.cs` — regresión de los hallazgos H-02 a H-05

Usar los helpers que ya tiene la clase: `EnSta`, `Cargar`, `Control<T>` y
`DialogosPrueba.Comprobar`.

### T-15. CP-39 — H-02: temporizador de la reserva a la vista (RF-06; RN-15)

1. `e = Fake.EventoPublicado(1000m, 5)`. Un cliente inicia una compra de 1 entrada.
2. Abrir `new Visual.frmPago { Compra = compra }` con `Cargar`.
3. Comprobar en `lblTemporizador`:
   - el texto cumple `^Reserva: \d{2}:\d{2}$` y los minutos son `<= RESERVA_MINUTOS`;
   - `ForeColor == Color.FromArgb(116, 86, 174)`.
4. Cerrar el formulario. Iniciar otra compra, poner
   `compra.FechaCompra = DateTime.Now.AddMinutes(-RESERVA_MINUTOS).AddSeconds(45)` en
   memoria, sin tocar la base, y abrirla en un `frmPago` nuevo. Ahora
   `lblTemporizador.ForeColor == Color.Firebrick`.

### T-16. CP-40 — H-03: cupo restante por sector (RF-04; RN-08)

1. `e = Fake.EventoPublicado(1000m, 5)`. `Fake.CompraConfirmada(c1, e, 1, false)`.
2. Abrir `FrmCompraCliente { IDEvento = e.Id, IDUsuario = c2.Id }`.
3. En `dgvSectores`, la fila del sector tiene `"Disponibles" == 4`. `"Cantidad"` es la única
   columna con `ReadOnly == false`.

### T-17. CP-41 — H-04: confirmación al comprar sin seguro (RF-05; RN-28)

1. `e = Fake.EventoPublicado(1000m, 5)`. Abrir `FrmCompraCliente` para un cliente nuevo y
   poner `"Cantidad" = 1` en la fila del sector. `chkSeguro` queda sin tildar.
2. `DialogosPrueba.Comprobar(() => btnComprar.PerformClick(), …)` con un solo diálogo
   esperado:
   - título `"Seguro de entrada"`;
   - texto que contiene `"¿Continuar sin seguro?"`;
   - `Predeterminado = DialogResult.No` y `Respuesta = DialogResult.No`.
3. Después, `DisponiblesDe(sector) == 5`: no se inició ninguna compra. El formulario sigue
   visible.

### T-18. CP-42 — H-05: explicación de los niveles (RF-08; RN-19, RN-20)

1. Abrir `FrmCompraCliente` sobre un `Fake.EventoPublicado(...)`.
2. `lnkNiveles.Visible` es `true` y su texto es `"¿Cómo funcionan los niveles?"`.
3. Invocar `lnkNiveles_LinkClicked` por reflexión dentro de `DialogosPrueba.Comprobar`, como
   la clase ya hace con `CargarEventos`. Diálogo esperado:
   - título `"¿Cómo funcionan los niveles?"`;
   - texto que contiene `$"Plata: de {plata} a {oro - 1} compras"`, con `plata` y `oro`
     leídos de `FIDELIDAD_UMBRAL_PLATA` y `FIDELIDAD_UMBRAL_ORO`.

---

## Criterio de terminado

- Están las tareas T-01 a T-18. Cada CP-26 a CP-42 corresponde a un solo método, con el ID en
  `DisplayName`.
- La batería corre al menos **48 ejecuciones**: las 31 actuales más 17 nuevas. Todas son
  correctas o están marcadas `[Ignore("Hallazgo: …")]` según la regla 2.
- El reporte final para JC incluye:
  1. los archivos tocados;
  2. la salida resumida de `tools\Proyecto.ps1 -Accion Probar`;
  3. la tabla CP → método → resultado;
  4. cada hallazgo con su mensaje de error;
  5. cualquier prueba manual que haya surgido, con qué probar y qué resultado se espera,
     para que Claude Code la pase a `docs/PRUEBAS-PENDIENTES.md`.

**Estado al 04/11/2026:** T-01 a T-18 terminadas (commit `5eabe41`; reporte en
[RESULTADOS-TAREAS-CODEX-2026-11-04.md](RESULTADOS-TAREAS-CODEX-2026-11-04.md)). 48
ejecuciones: 47 correctas y CP-34 omitida.

---

# Segunda tanda — CP-34 (T-19 y T-20)

Base: el commit que deje JC después de esta revisión. Valen las mismas reglas 1 a 8 de
arriba, con una excepción: T-20, si JC la aprueba, sí modifica `Repo/`.

## Diagnóstico: la falla de CP-34 la provocó la preparación, no la aplicación

La aplicación guarda el plazo de la reserva en dos lugares que siempre coinciden:

| Dónde | Valor | Lo usa |
|---|---|---|
| `Compras.ReservaVence`, columna | `ahora + RESERVA_MINUTOS`, escrito por `GestorCompras.Iniciar` (línea 65) y por `Rereservar` en un reintento | `RepositorioCompras.MarcarVencidas`, que pasa la compra a Vencida y devuelve el cupo |
| `GestorCompras.VencimientoDe(c)`, calculado | `c.FechaCompra + RESERVA_MINUTOS` | `GestorCompras.Confirmar`, que decide si lanza RN-15 (línea 121); `frmPago`, para el temporizador |

En el uso real los dos valores son iguales, porque `FechaCompra` y `ReservaVence` se graban
juntos a partir del mismo `ahora`. `Fake.RetrasarInicioDeCompra` (T-10) atrasó **solo
`FechaCompra`** y dejó un estado que la aplicación no produce: según `Confirmar`, la
reserva venció; según `ReservaVence`, sigue vigente.

Por eso, `Confirmar` lanza RN-15 y llama a `LiberarVencidas`, pero `MarcarVencidas` mira
`ReservaVence`, que sigue en el futuro, y no toca la compra. Queda Pendiente, con el cupo
retenido, hasta que pase `ReservaVence` y algo vuelva a llamar a `LiberarVencidas`.

**No hay proceso en segundo plano.** `LiberarVencidas` corre solo en tres momentos:
- al abrir `FrmCompraCliente` (línea 117);
- cuando se agota el temporizador de `frmPago` (línea 48);
- dentro de `Confirmar`, cuando lanza RN-15.

El error fue del encargo T-10, no del código ni de Codex.

## Riesgo real que sí existe (bajo)

Los dos valores pueden divergir de verdad en un solo caso: si alguien cambia `RESERVA_MINUTOS`
en la tabla `Parametros` mientras hay una compra en curso.

- **Si el parámetro baja**, pasa lo mismo que en CP-34: RN-15 con la compra Pendiente y el
  cupo retenido hasta el `ReservaVence` original.
- **Si el parámetro sube**, `Confirmar` acepta pagar después de `ReservaVence`. Si en el medio
  otro cliente abrió la pantalla de compra, `LiberarVencidas` ya pasó la compra a Vencida.
  Entonces `Confirmar` lanza `InvalidOperationException` («La compra ya no está pendiente de
  pago»), que `frmPago.btnPagar_Click` no captura: solo captura `ArgumentException` y
  `ReglaNegocioException`. El resultado es un error no controlado en pantalla.

## T-19. Corregir la preparación de CP-34 (obligatoria; solo `Pruebas/`)

1. En `Pruebas/Fake.cs`, `RetrasarInicioDeCompra(int idCompra, int minutos)` atrasa las dos
   columnas en la misma sentencia:
   `UPDATE Compras SET FechaCompra = DATEADD(MINUTE, -@Minutos, FechaCompra),
   ReservaVence = DATEADD(MINUTE, -@Minutos, ReservaVence) WHERE ID = @ID`.
   Si `ReservaVence` es NULL, `DATEADD` devuelve NULL y queda NULL, que es lo correcto.
2. En `PruebasCompra.CP34_ReservaVencida`, quitar el `[Ignore(...)]`. No se cambia ninguna
   aserción.
3. Correr la batería. Esperado: 48 ejecuciones, 48 correctas. CP-34 comprueba sus cuatro
   resultados: RN-15, compra Vencida, `DisponiblesDe == 5` y ningún pago.
4. Si CP-34 falla después de este cambio, se aplica la regla 2: se vuelve a marcar con
   `Ignore` y se reporta el mensaje.

## T-20. Una sola fuente para el plazo de la reserva (necesita aprobación de JC)

Solo se hace si JC la aprueba de forma explícita. Es la única tarea de esta lista que
modifica `Repo/`.

1. **`Repo/GestorCompras.cs`, método `VencimientoDe(Compra c)`.** Devuelve `c.ReservaVence`
   si tiene valor; si no, `c.FechaCompra.AddMinutes(MinutosReserva())`. Es el mismo criterio
   que `RepositorioCompras.MarcarVencidas`. Actualizar el comentario del método para que lo
   diga.
2. **No se modifica nada más en `Repo/`.** `Confirmar` ya llama a `VencimientoDe(actual)`
   con la compra releída de la base. En un reintento, `ReservaVence` es NULL y el resultado
   no cambia respecto de hoy.
3. **`Pruebas/PruebasInterfaz.cs`, `CP39_TemporizadorDeReservaALaVista`.** Donde hoy asigna
   `porVencer.FechaCompra = …`, asignar también `porVencer.ReservaVence` con el mismo valor
   más `RESERVA_MINUTOS`. Sin ese cambio, el temporizador seguiría usando el `ReservaVence`
   original y la prueba fallaría.
4. **Prueba nueva CP-43 en `PruebasCompra.cs`** (RF-06; RN-15): «el plazo lo fija la
   reserva, no el parámetro vigente».
   1. Iniciar una compra.
   2. Atrasar solo `ReservaVence`, con un helper nuevo
      `Fake.VencerReserva(int idCompra)` que hace
      `UPDATE Compras SET ReservaVence = DATEADD(MINUTE, -1, GETDATE()) WHERE ID = @ID`.
   3. `Confirmar` lanza RN-15, la compra queda Vencida y el cupo vuelve.
   Sin T-20, este escenario cobra una reserva vencida. Con T-20, la rechaza.
5. Batería esperada: 49 ejecuciones, todas correctas.

**No se agrega** un `catch (InvalidOperationException)` en `frmPago`: con T-20, el caso que
lo provocaba deja de ocurrir.

**Estado al 04/11/2026:** T-19 y T-20 terminadas (commit `3af4154`; reporte en
[RESULTADOS-TAREAS-CODEX-2026-11-04-T19-T20.md](RESULTADOS-TAREAS-CODEX-2026-11-04-T19-T20.md)).
49 de 49 correctas.

## Lo que no se hace: probar el vencimiento esperando el reloj

Esperar el plazo real llevaría `RESERVA_MINUTOS` (10 minutos) por corrida. Con el parámetro
en 1 minuto, igual sumaría más de un minuto a una batería que hoy tarda 5 segundos.
Inyectar un reloj en `GestorCompras` sería un cambio de diseño en `Repo/` a un mes de la
entrega. Con T-19, la prueba ya reproduce el estado real que deja el paso del tiempo (las
dos columnas atrasadas juntas), así que esperar no agrega cobertura.

Lo que sí vale es **una comprobación manual por reloj, una sola vez**: dejar vencer una
reserva en la aplicación (`docs/PRUEBAS-PENDIENTES.md`, CX-02). Verifica el temporizador y
el aviso de `frmPago`, que ninguna prueba automática cubre de punta a punta.

---

# Tercera tanda — T-21: la semilla reproduce el ejemplo del documento

Base: el commit que deje JC después de esta revisión. Valen las reglas 1 a 8 de la primera
tanda, con una excepción: **esta tarea modifica `BaseDatos/02-semilla.sql`**. `BaseDatos/` no
tiene dueño y JC autoriza a Codex solo para esta tarea.

**Por qué.** El instructivo del documento (página 54) resuelve un ejemplo con «Duki en
Rosario» y una clienta Plata con 4 compras confirmadas: 2 entradas de Campo a $25.000 y
hospedaje de $45.000, total $99.625. La semilla tiene los mismos importes, pero el evento se
llama «Festival de Rock Rosario» y la clienta Plata tiene 3 compras. JC decidió alinear la
semilla con el documento, no al revés. Así el docente puede seguir el instructivo en la
aplicación y obtener exactamente lo que muestra el texto.

## T-21. Cambios en `BaseDatos/02-semilla.sql`

1. **Nombre del evento vigente** (línea 68): `'Festival de Rock Rosario'` pasa a
   `'Duki en Rosario'`. No cambia nada más del evento: fecha (hoy + 60 días, 21:00), local,
   categoría «Concierto», estado, sus tres sectores (Campo $25.000 / 2000, Platea $40.000 /
   800, VIP $60.000 / 100) y sus cuatro servicios (Hotel Ríos 1 noche $45.000, Traslado,
   Remera, Meet & Greet). Sigue siendo el primer evento insertado, así que conserva el ID 1
   en una base recién creada.
2. **Compras de la clienta Plata** (líneas 116 a 127): el bucle pasa de `WHILE @i <= 3` a
   `WHILE @i <= 4`, y el comentario de la línea 117 dice «Plata: 4 compras confirmadas en
   los últimos 12 meses». Las fechas quedan en 2, 4, 6 y 8 meses atrás, dentro de la ventana
   de 12 meses. Con 4 compras sigue siendo Plata (umbrales 3 a 6), así que no cambia su
   nivel ni el de nadie.
3. **No se toca** `03-datos-reporte.sql`. Ese script crea otro «Duki en Rosario», pasado
   (27/09/2025) y en otro local («Predio Rosario (datos de reporte)»), y borra lo suyo por
   local, no por nombre: no afecta al evento de la semilla. Si alguien carga los dos
   scripts, va a haber dos eventos con ese nombre en fechas distintas, y el cliente solo ve
   el vigente (RN-09).

## T-21. Cambios en `Pruebas/`

4. En `PruebasInterfaz.cs`, línea 28, el comentario `// Festival: tres sectores de la
   semilla.` pasa a `// Duki en Rosario: tres sectores de la semilla.` Los textos «Gran
   Festival Internacional…» de las líneas 33 y 373 no cambian: son títulos largos inventados
   para la prueba de recortes, no el nombre del evento.
5. Buscar en `Pruebas/` cualquier otra dependencia del nombre del evento 1 o de que la
   clienta Plata tenga 3 compras. Al 04/11 no hay ninguna: las pruebas usan el evento por ID
   y crean sus propios clientes con `Fake`.
6. Correr la batería: esperado **49 de 49**. La base de pruebas se recrea con la semilla
   nueva en `[AssemblyInitialize]`.

## T-21. Comprobación adicional, sin cambiar código

7. Con `sqlcmd` contra `BddEventAura_Pruebas`, después de la corrida, informar en el reporte
   el resultado de:
   `SELECT Nombre FROM Eventos WHERE ID = 1;` (esperado: Duki en Rosario) y
   `SELECT COUNT(*) FROM Compras c JOIN Usuario u ON u.ID = c.ID_Usuario WHERE u.Mail = 'plata@eventaura.com' AND c.Estado = 'Confirmada';`
   (esperado: 4).

**Fuera de alcance:** la base de desarrollo `BddEventAura`. No se recrea ni se modifica:
recrearla borra los datos de las pruebas manuales y lo decide JC.

**Después de T-21, Claude Code:**
- regenera las figuras del instructivo con `tools/CapturasInstructivo/Generar.ps1`;
- actualiza `docs/capturas/instructivo/LEEME.md`;
- comprueba que la figura 2 diga «Duki en Rosario» y la figura 3 dé $99.625.

Reporte de Codex: `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-T21.md`.

**Estado al 04/11/2026:** T-21 terminada (commit `7907fc6`). 49 de 49.

---

# Cuarta tanda — T-22 a T-26: seguridad, sesión, cierre y armado del paquete

Decididas por JC el 04/11/2026, **en este orden de prioridad**. Valen las reglas 1 a 8 de la
primera tanda, con una excepción explícita de JC: **T-22 puede modificar
`Visual/_Inicio/frmCrearUsuario.Designer.cs`**, y el cambio se declara en el reporte. Base: el
commit que deje JC después de esta revisión.

**Origen:**
- T-22 y T-23 son hallazgos de seguridad: el alta como «Propietario» y la falta de logout, de
  `docs/ANEXOS-TECNICOS-T01-T08.md`.
- T-24 a T-26 son U-01, U-04 y U-05 de `docs/usabilidad/INFORME-USABILIDAD.md`.

**Batería esperada al terminar: 60 ejecuciones**, todas correctas: las 49 actuales más 11
nuevas. Los IDs nuevos son CP-44 a CP-50.

## T-22. El alta pública crea solo clientes (prioridad 1)

**Problema.** `frmCrearUsuario` pasa a `GestorUsuarios.Registrar` el texto del combo
`cbTipoUsuario`, que ofrece «Cliente» y «Propietario». Cualquiera puede registrarse sola como
administrador y entrar a la gestión de eventos y a los reportes: RN-34 solo verifica ese
perfil. Además, el combo no es de solo lectura (`DropDownStyle` por defecto): aunque se quite
el ítem, se puede **escribir** «Propietario» a mano. Por eso el arreglo va en las dos capas.

1. **`Repo/GestorUsuarios.cs`.** Agregar, junto a `Registrar`:
   ```csharp
   // Alta pública: solo clientes. El perfil Propietario no se autoasigna (RN-05, RN-34).
   public void RegistrarCliente(string mail, string pwd, string nombre, string apellido)
   {
       Registrar(mail, pwd, nombre, apellido, TipoCliente);
   }
   ```
   `Registrar` queda como está: la usa la semilla y la usaría un alta hecha por un
   administrador (T04).
2. **`Visual/_Inicio/frmCrearUsuario.cs`, línea 51.** Llamar a
   `GestorUsuarios.Instance.RegistrarCliente(txtmail.Text, TxtPassword.Text, txtNombre.Text,
   txtApellido.Text)`. No queda ninguna referencia a `cbTipoUsuario`.
3. **`Visual/_Inicio/frmCrearUsuario.Designer.cs`.** Eliminar el control `cbTipoUsuario`
   completo: la declaración, el `new`, el bloque de propiedades y el `Controls.Add`. No se
   reposiciona ni se redimensiona ningún otro control.
4. **CP-44 en `PruebasInterfaz.cs`** (RF-01; RN-05):
   - en `frmCrearUsuario` no hay ningún `ComboBox` ni ningún control llamado `cbTipoUsuario`;
   - completar nombre, apellido, `Fake.MailNuevo()` y «Cliente123» en las dos contraseñas;
   - pulsar «Registrarse» y responder el aviso de éxito con `DialogosPrueba` (texto que
     contiene «Usuario creado»);
   - `RepositorioUsuarios.Instance.ObtenerPorMail(mail).TipoUsuario == "Cliente"`.
5. **CP-45 en `PruebasCuentas.cs`** (RF-01; RN-05): `RegistrarCliente` con `Fake.MailNuevo()`
   crea un usuario con `TipoUsuario == "Cliente"` y nivel Bronce.

## T-23. «Volver al inicio» cierra la sesión (prioridad 2)

**Problema.** Ninguna pantalla llama a `GestorSesion.Instance.Cerrar()`. «Volver al inicio»
muestra el login, pero la sesión anterior sigue abierta.

1. **`Visual/Cliente/frmMenuCliente.cs`, `lblvolver_Click`** y
   **`Visual/Propietario/frmMenuPropietario.cs`, `lblvolver_Click`:** la primera línea del
   método es `GestorSesion.Instance.Cerrar();`. El resto no cambia.
2. **CP-46 en `PruebasInterfaz.cs`** (RF-02), `[DataTestMethod]` con dos filas:
   - **«cliente»:** iniciar sesión con `GestorSesion.Instance.Iniciar` usando un
     `Fake.Cliente(Bronce)` y abrir `frmMenuCliente`;
   - **«administrador»:** lo mismo con `Fake.Administrador()` y `frmMenuPropietario`;
   - en las dos filas, invocar `lblvolver_Click` por reflexión. Después,
     `GestorSesion.Instance.HaySesion == false` y hay un `frmInicio` visible.

## T-24. Cerrar con la X no deja la aplicación corriendo (prioridad 3)

**Problema, medido sobre el ejecutable con UI Automation.** Todas las pantallas navegan con
`Hide()`, y la principal (`frmInicio`) queda oculta. Si el usuario cierra una pantalla con la
X, el proceso sigue vivo sin ninguna ventana. Además, en «Armar paquete» la X descarta el
paquete sin preguntar, aunque «Volver al listado» sí pregunta.

1. **Archivo nuevo `Visual/FormularioEventAura.cs`:**
   ```csharp
   // Base de los formularios de EventAura. Si el usuario cierra la última ventana visible,
   // termina la aplicación: antes quedaba un proceso sin ventanas (hallazgo U-01).
   public class FormularioEventAura : Form
   {
       public static bool DebeTerminarAplicacion(Form cerrado, CloseReason motivo, IEnumerable<Form> abiertos)
       {
           return motivo == CloseReason.UserClosing &&
                  !abiertos.Any(f => f != cerrado && !f.IsDisposed && f.Visible);
       }

       protected override void OnFormClosed(FormClosedEventArgs e)
       {
           base.OnFormClosed(e);
           // Application.MessageLoop es false en las pruebas: ahí nunca se termina el proceso.
           if (Application.MessageLoop &&
               DebeTerminarAplicacion(this, e.CloseReason, Application.OpenForms.Cast<Form>().ToList()))
           {
               Application.Exit();
           }
       }
   }
   ```
   Es pública para que la pruebe `Pruebas/`. Con los `using` que correspondan.
2. **En las 19 clases de formulario de `Visual/`** (los `.cs`, no los `.Designer.cs`), cambiar
   `public partial class X : Form` por `public partial class X : FormularioEventAura`. Son:
   - en `_Inicio/`: frmInicio, frmCrearUsuario, frmCambiarContraseña;
   - en `Cliente/`: frmMenuCliente, frmEventosCliente, FrmCompraCliente, frmPago,
     frmConfirmacion, frmHistorialCliente, frmSuscripcion;
   - en `Propietario/`: frmMenuPropietario, frmMisLocales, frmRegistrarLocal, frmEventosLocal,
     frmAgregarEvento, frmAgregarSector, frmAdministrarEvento, frmListasEspera, frmReportes.

   Ojo: `frmAgregarSector` está declarada sin espacio antes de los dos puntos
   (`frmAgregarSector: Form`). Si la cuenta no da 19, se informa la diferencia en el reporte.
3. **`Visual/Cliente/FrmCompraCliente.cs`.** Sobrescribir `OnFormClosing`:
   - si `e.CloseReason == CloseReason.UserClosing && !PaqueteVacio()`, mostrar
     `MessageBox.Show("Si cierra la ventana se descarta el paquete que armó. ¿Desea cerrar?",
     "Cerrar la compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
     MessageBoxDefaultButton.Button2)`;
   - si la respuesta no es `Yes`, `e.Cancel = true`;
   - después, `base.OnFormClosing(e)`.

   «Volver al listado» usa `Hide()` y no pasa por acá. Con el paquete vacío no pregunta.
4. **CP-47 en `PruebasInterfaz.cs`** (RNF-02), `[DataTestMethod]` con cuatro filas sobre
   `DebeTerminarAplicacion`, con formularios creados sin mostrar o mostrados según el caso:

   | Fila | Motivo | Otras ventanas | Resultado esperado |
   |---|---|---|---|
   | 1 | `UserClosing` | otra visible | `false` |
   | 2 | `UserClosing` | ninguna otra | `true` |
   | 3 | `UserClosing` | solo ocultas | `true` |
   | 4 | `ApplicationExitCall` | ninguna otra | `false` |

5. **CP-48 en `PruebasInterfaz.cs`** (RF-05):
   - en `FrmCompraCliente` del evento 1, con `"Cantidad" = 2` en Campo, `form.Close()` muestra
     «Cerrar la compra» con «No» por defecto; respondiendo «No», el formulario sigue visible y
     la cantidad sigue en 2;
   - otro `form.Close()`, respondiendo «Sí»: el formulario ya no está visible;
   - en un `FrmCompraCliente` nuevo, sin entradas, `Close()` no muestra ningún diálogo.

**Verificación sobre el ejecutable, la hace Claude Code después:**
`tools/Usabilidad/VerificarCierreConX.ps1` tiene que informar «proceso vivo = False».

## T-25. Indicar dónde se escribe la cantidad (prioridad 4)

**Problema, medido.** La cantidad se escribe en una celda de la grilla sin ninguna
indicación, y al abrir la pantalla la celda activa es «Sector» de la primera fila.

Todo en **`Visual/Cliente/FrmCompraCliente.cs`, método `CargarSectores`**. No se toca el
Designer.

1. La columna sigue llamándose `"Cantidad"`, porque las pruebas la usan por nombre. Su
   encabezado visible pasa a `"Cantidad (escribí aquí)"`.
2. Cada celda de Cantidad tiene `ToolTipText = $"Escribí cuántas entradas querés de este
   sector (hasta {max} por compra)."`, con `max` leído de `COMPRA_ENTRADAS_MAX`.
3. Al terminar de cargar, `dgvSectores.CurrentCell` es la celda Cantidad de la **primera
   fila con `Disponibles` > 0**. Si no hay ninguna (evento agotado), no se cambia.
4. **CP-49 en `PruebasInterfaz.cs`** (RF-04; H-03): al abrir `FrmCompraCliente` del evento 1,
   - `CurrentCell.OwningColumn.Name == "Cantidad"`;
   - el `"Disponibles"` de esa fila es mayor que 0;
   - el encabezado de la columna contiene «escribí»;
   - el `ToolTipText` de la celda contiene el valor de `COMPRA_ENTRADAS_MAX`.

## T-26. «Comprar» y el seguro deshabilitados sin entradas (prioridad 5)

**Problema, medido.** «Comprar» y el seguro están habilitados sin ninguna entrada, aunque la
página 51 del documento dice que «Comprar» «permanece deshabilitado mientras no haya al menos
una entrada» y que el seguro «se habilita sólo cuando hay al menos una entrada». Los
servicios **no** cambian: la página 51 solo los deshabilita si el evento no los ofrece, y RN-13
ya se valida al comprar.

Todo en **`Visual/Cliente/FrmCompraCliente.cs`**:

1. Un campo `private bool agotado;`. `MostrarSiAgotado` asigna el campo en lugar de declarar
   una variable local con el mismo nombre.
2. Un método `private void ActualizarHabilitados()`:
   - `bool hayEntradas = GestorPaquetes.Instance.CantidadEntradas(paquete) > 0;`
   - `btnComprar.Enabled = hayEntradas && !agotado;`
   - `chkSeguro.Enabled = hayEntradas && !agotado;`
   - si `chkSeguro` queda deshabilitado y estaba tildado, se destilda.
3. Llamarlo al final de `ActualizarResumen()` y al final de `MostrarSiAgotado()`. En
   `MostrarSiAgotado` se quitan las asignaciones directas a `btnComprar.Enabled` y
   `chkSeguro.Enabled`, que pasan a este método.
4. **CP-50 en `PruebasInterfaz.cs`** (RF-05; RN-13, RN-28), sobre el evento 1:
   - al abrir, `btnComprar` y `chkSeguro` están deshabilitados;
   - con `"Cantidad" = 1` en Campo, los dos se habilitan;
   - con `"Cantidad" = 0`, se vuelven a deshabilitar.
5. **Ajuste autorizado de CP-24** (`PruebasInterfaz.cs`, línea 319). Hoy afirma que «Comprar»
   queda habilitado apenas se libera el cupo, sin entradas elegidas. Con T-26 eso cambia por
   decisión de JC. Se reemplaza esa aserción por esta secuencia:
   - `btnComprar` **deshabilitado** con la grilla visible;
   - poner `"Cantidad" = 1` en el sector;
   - `btnComprar` **habilitado**.

   Es la única aserción existente que se modifica en esta tanda.

## Criterio de terminado de la cuarta tanda

- **Batería:** 60 ejecuciones, todas correctas: 49 más CP-44, CP-45, CP-46 (2 filas), CP-47
  (4 filas), CP-48, CP-49 y CP-50. Sin advertencias nuevas de compilación.
- **Reporte:** `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-T22-T26.md`, con los archivos tocados,
  el cambio de Designer declarado (T-22), la cantidad de formularios (19 esperados) que pasaron a
  `FormularioEventAura` y cualquier hallazgo.
- **Lo que sigue lo hace Claude Code.** El estudio de usabilidad describe la interfaz
  **anterior** a esta tanda. Después, Claude Code vuelve a medir con `tools/Usabilidad/` y
  actualiza el informe y la lista de correcciones del documento. Hasta entonces, el informe no
  es definitivo.

**Estado al 04/11/2026:** T-22 a T-26 terminadas (commits `272c864`, `309b9be` y `bb41f23`).
60 de 60. Claude Code volvió a medir la usabilidad sobre la aplicación corregida.

---

# Quinta tanda — T-27 a T-29: errores no controlados, base de formularios y encabezado

> **Terminada el 05/11/2026.** Codex hizo T-27 a T-29 (66 de 66) y agregó CP-55 a CP-61, las
> pruebas del camino de error de siete reglas: batería final **73 de 73**. Reporte:
> `docs/RESULTADOS-TAREAS-CODEX-2026-11-05-T27-T29.md`. Claude Code verificó el manejador con
> SQL Server detenido y el pago con el evento eliminado (`docs/EXCEPCIONES.md`), y regeneró
> las figuras con el encabezado completo.

Valen las reglas 1 a 8 de la primera tanda. Base: el commit que deje JC después de esta
revisión. **Batería esperada al terminar: 66 ejecuciones**, todas correctas: 60 más 6 nuevas,
con los IDs CP-51 a CP-54.

## T-27. Manejador global de errores (aprobado por JC el 04/11/2026)

**Problema, verificado sobre la aplicación** con `tools/Excepciones/VerificarErrorNoControlado.ps1`:
- **sin manejador global**, un error de base de datos (SQL Server detenido, base inexistente)
  llega al usuario como el cuadro de .NET «Microsoft .NET Framework – Excepción no controlada
  en la aplicación», en inglés y con los botones Detalles, Continuar y Salir;
- **si un propietario elimina un evento mientras un cliente paga**, `GestorCompras.Confirmar`
  lanza `InvalidOperationException` («La compra ya no está pendiente de pago.»), que
  `frmPago` no captura, y termina en el mismo cuadro.

Detalle en `docs/EXCEPCIONES.md`.

1. **Archivo nuevo `Visual/ManejadorErrores.cs`**, con su entrada `<Compile Include>` en
   `Visual/Visual.csproj`, como se hizo con `FormularioEventAura.cs`:
   ```csharp
   // Último recurso ante errores que ninguna pantalla captura: el usuario ve un mensaje en
   // castellano, sin detalles técnicos, en lugar del cuadro de .NET.
   public static class ManejadorErrores
   {
       public const string Titulo = "EventAura";
       public const string MensajeBaseDeDatos =
           "No se pudo conectar con la base de datos. Verifique que SQL Server esté en funcionamiento y vuelva a intentar.";
       public const string MensajeGeneral =
           "Ocurrió un error inesperado y la operación no se completó. Si se repite, avise al administrador.";

       // Lo llama Program.Main antes de crear cualquier formulario.
       public static void Instalar()
       {
           Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
           Application.ThreadException += (s, e) => Mostrar(e.Exception);
           AppDomain.CurrentDomain.UnhandledException += (s, e) => Mostrar(e.ExceptionObject as Exception);
       }

       public static string MensajeParaUsuario(Exception ex)
       {
           if (ex is System.Data.Common.DbException) return MensajeBaseDeDatos;   // SqlException hereda de DbException
           // Los mensajes que escribe Negocio ya están en castellano y dicen qué pasó.
           if (ex != null && ex.Source == "Repositorio" && !string.IsNullOrWhiteSpace(ex.Message)) return ex.Message;
           return MensajeGeneral;
       }

       public static void Mostrar(Exception ex)
       {
           MessageBox.Show(MensajeParaUsuario(ex), Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
       }
   }
   ```
   Con los `using` que correspondan. No escribe bitácora: eso es el anexo T06.
2. **`Visual/Program.cs`, `Main`.** La primera línea es `ManejadorErrores.Instalar();`, antes
   de `Application.EnableVisualStyles()`. Se borra el comentario suelto `/// //aaaa`.
3. **`Visual/Cliente/frmPago.cs`, `btnPagar_Click`.** Agregar, después del
   `catch (ReglaNegocioException ex)`:
   ```csharp
   catch (InvalidOperationException ex)
   {
       // La compra dejó de estar pendiente por fuera de esta pantalla (por ejemplo, el evento
       // se eliminó mientras se pagaba). El cupo ya se liberó: se vuelve al listado.
       tmrReserva.Stop();
       cerrada = true;
       MessageBox.Show(ex.Message + "\n\nVuelva a armar la compra desde el listado.",
           "Pago no realizado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
       DialogResult = DialogResult.Abort;
       return;
   }
   ```
   `DialogResult.Abort` ya lleva a «Armar paquete» a volver al listado, como cuando vence la
   reserva.
4. **CP-51 en `PruebasInterfaz.cs`**, `[DataTestMethod]` con tres filas sobre
   `ManejadorErrores.MensajeParaUsuario`:

   | Fila | Excepción | Resultado esperado |
   |---|---|---|
   | «base de datos» | Una subclase de prueba de `System.Data.Common.DbException`, declarada en la clase de prueba | `MensajeBaseDeDatos` |
   | «negocio» | La `InvalidOperationException` real de `GestorPaquetes.Instance.Nuevo(999999, 3)`, capturada en la prueba | Su propio mensaje, «No se encontró ningún evento con ese ID.» |
   | «general» | `new Exception("detalle técnico")` | `MensajeGeneral`, que no contiene «detalle técnico» |

5. **CP-52 en `PruebasInterfaz.cs`** (RF-06; RN-11), el caso encontrado:
   - **preparación:** un cliente con `Fake`, un evento con `Fake.EventoPublicado(1000m, 5)` y
     una compra iniciada de 1 entrada; abrir `new Visual.frmPago { Compra = compra }`;
   - **la eliminación:** `GestorEventos.Instance.Eliminar(evento.Id)`. No tiene ventas, así
     que RN-11 la permite, y pasa a Vencida la compra abierta;
   - **el pago:** completar la tarjeta válida de `Fake.Tarjeta()` y, con `DialogosPrueba`,
     pulsar «Pagar» esperando el título «Pago no realizado» y un texto que contenga «ya no
     está pendiente»;
   - **resultado esperado:** `pago.DialogResult == DialogResult.Abort` y
     `GestorCompras.Instance.PagosDe(compra)` vacío.
6. **CP-53 en `PruebasFormularios.cs`:** el texto de `Visual/Program.cs` contiene
   `ManejadorErrores.Instalar();` antes de `Application.Run`. Se lee el archivo como hace CP-12
   con los `.Designer.cs`.

**La verificación sobre la aplicación la hace Claude Code después.** Va a ajustar
`tools/Excepciones/ErrorNoControlado.cs` para que instale el manejador, igual que
`Program.Main`, y comprobar que con la base caída aparece el mensaje en castellano y ya no
`ThreadExceptionDialog`.

## T-28. La base de formularios no debe ser abstracta (corrige un desvío de T-24)

**Problema.** En T-24 el encargo pedía `public class FormularioEventAura`, y se implementó
`public abstract class`. Codex lo hizo para que CP-12 no la exigiera como formulario con
`.Designer.cs`. El costo es que **el diseñador de formularios de Visual Studio no puede abrir
un formulario cuya clase base es abstracta**: necesita crear una instancia de la base para
dibujarlo. Los 19 formularios quedarían sin poder editarse en el diseñador, que es justo lo
que Máximo usa.

1. **`Visual/FormularioEventAura.cs`.** `public abstract class` pasa a `public class`. Nada
   más cambia.
2. **`Pruebas/PruebasFormularios.cs`, CP-12.** En el filtro de tipos, agregar
   `&& t != typeof(Visual.FormularioEventAura)` junto a `!t.IsAbstract`, con un comentario:
   «la base común no es una pantalla y no tiene Designer».
3. **Verificación manual de JC o Máximo** (no automatizable): abrir en Visual Studio el
   diseñador de `FrmCompraCliente` y de `frmInicio` y confirmar que se dibujan. Claude Code lo
   anota en `docs/PRUEBAS-PENDIENTES.md`.

## T-29. El encabezado «Cantidad (escribí aquí)» se ve completo (corrige T-25)

**Problema, visto en la figura 2 del instructivo regenerada el 04/11/2026.** A escala 100 %,
la columna es más angosta que el encabezado nuevo, y se lee «Cantidad (escribí», sin el
«aquí)». La indicación de T-25 pierde justo la parte que dice qué hacer.

1. **`Visual/Cliente/FrmCompraCliente.cs`, `CargarSectores`.** Después de crear las columnas:
   ```csharp
   dgvSectores.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
   dgvSectores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
   ```
   El encabezado pasa a dos renglones si no entra en uno. No se cambia el texto ni el ancho
   de las columnas.
2. **CP-54 en `PruebasInterfaz.cs`** (RF-04; H-03): al abrir `FrmCompraCliente` del evento 1,
   para cada columna visible de `dgvSectores`, el alto del texto del encabezado, medido con
   `TextRenderer.MeasureText(texto, fuente, new Size(ancho de la columna − 8, int.MaxValue),
   TextFormatFlags.WordBreak)`, no supera `dgvSectores.ColumnHeadersHeight`.
3. **CP-13 y CP-25 tienen que seguir pasando:** el encabezado más alto corre las filas unos
   píxeles.

Después, Claude Code regenera las figuras del instructivo.
## Criterio de terminado de la quinta tanda

- **Batería:** 66 ejecuciones, todas correctas: 60 más CP-51 (3 filas), CP-52, CP-53 y CP-54.
  CP-12 sigue pasando con la base no abstracta. Sin advertencias nuevas.
- **Reporte:** `Pruebas/RESULTADOS-TAREAS-CODEX-<fecha>-T27-T29.md`, con los archivos tocados
  y la entrada nueva del `.csproj`.
