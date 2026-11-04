# Tareas para Codex — pruebas CP-26 a CP-42 y etiquetado de CP-11 a CP-25

Encargo cerrado. Objetivo: cerrar las brechas G-02 (RF sin pruebas) y G-04 (hallazgos sin
regresión) de la matriz de trazabilidad (`docs/MATRIZ-TRAZABILIDAD-v2.5.md`, sección 10).
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

## Lo que no se hace: probar el vencimiento esperando el reloj

Esperar el plazo real llevaría `RESERVA_MINUTOS` (10 minutos) por corrida. Con el parámetro
en 1 minuto, igual sumaría más de un minuto a una batería que hoy tarda 5 segundos.
Inyectar un reloj en `GestorCompras` sería un cambio de diseño en `Repo/` a un mes de la
entrega. Con T-19, la prueba ya reproduce el estado real que deja el paso del tiempo (las
dos columnas atrasadas juntas), así que esperar no agrega cobertura.

Lo que sí vale es **una comprobación manual por reloj, una sola vez**: dejar vencer una
reserva en la aplicación (`docs/PRUEBAS-PENDIENTES.md`, CX-02). Verifica el temporizador y
el aviso de `frmPago`, que ninguna prueba automática cubre de punta a punta.
