# Resultado de T-01 a T-18

> Reporte de Codex. Lo escribió en `Pruebas/RESULTADOS-TAREAS-CODEX.md` porque no puede
> escribir en `docs/`; Claude Code lo movió acá sin cambiar el contenido. Los cambios que
> describe están en el commit `5eabe41`.
>
> **Diagnóstico de H-CODEX-01 (CP-34)**, escrito por Claude Code: no es un defecto de la
> aplicación. La compra queda Pendiente por la preparación que pedía T-10, que atrasaba solo
> `FechaCompra`. Ver «Segunda tanda» en [TAREAS-CODEX-PRUEBAS.md](TAREAS-CODEX-PRUEBAS.md) y
> el detalle en [ESTADO-AGENTES.md](ESTADO-AGENTES.md).

Base: `f550199`. Corrida: 04/11/2026. Se ejecutó el encargo en orden y se detuvo el
trabajo al completar T-18. No se hicieron commits ni push. Los cambios de esta tanda
están exclusivamente en `Pruebas/`; no se modificaron los `App.config` protegidos.

## Batería final

Comando ejecutado después de cada tarea:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar
```

Última salida resumida:

```text
La serie de pruebas se ejecutó correctamente.
Pruebas totales: 48
     Correcto: 47
    Omitido: 1
Tiempo total: 5,2555 Segundos
Código de salida: 0
```

Compilación sin errores ni advertencias nuevas. La prueba omitida es CP-34: conserva
el resultado exigido por el encargo y lleva `Ignore("Hallazgo: …")`. No debe contarse
como aprobada. No hay pruebas fallidas activas.

## Verificación después de cada tarea

| Tarea | Correctas | Omitidas | Fallidas |
|---|---:|---:|---:|
| T-01 | 31 | 0 | 0 |
| T-02 | 32 | 0 | 0 |
| T-03 | 33 | 0 | 0 |
| T-04 | 34 | 0 | 0 |
| T-05 | 35 | 0 | 0 |
| T-06 | 36 | 0 | 0 |
| T-07 | 37 | 0 | 0 |
| T-08 | 38 | 0 | 0 |
| T-09 | 39 | 0 | 0 |
| T-10, después de registrar el hallazgo | 39 | 1 | 0 |
| T-11 | 40 | 1 | 0 |
| T-12 | 41 | 1 | 0 |
| T-13 | 42 | 1 | 0 |
| T-14 | 43 | 1 | 0 |
| T-15 | 44 | 1 | 0 |
| T-16 | 45 | 1 | 0 |
| T-17 | 46 | 1 | 0 |
| T-18 | 47 | 1 | 0 |

La primera ejecución de T-10 reprodujo el defecto (39 correctas y 1 fallida). Se mantuvo
la prueba y se aplicó la excepción indicada en la regla 2 antes de continuar.

## CP → método → resultado

Los casos CP-01 a CP-10 no se modificaron y siguen pasando. CP-11 a CP-25 recibieron
comentario de trazabilidad y nombre visible, sin cambios en sus aserciones. Se compararon
los cuerpos de los métodos originales con `HEAD`: sin cambios.

| CP | Clase | Método | Resultado |
|---|---|---|---|
| CP-11 | PruebasFidelizacion | `Referencia_DaExactamente99625` | Correcta |
| CP-12 | PruebasFormularios | `RNF03_NingunFormularioPasaDe790DeAlto` | Correcta |
| CP-13 | PruebasInterfaz | `Compra_PaqueteMaximoYNombreLargoSinRecortes` | Correcta |
| CP-14 | PruebasInterfaz | `Registro_AbreSinDatosPrecargados` | Correcta |
| CP-15 | PruebasInterfaz | `Registro_RechazaDatosInvalidosSinCrearUsuario` | 4 variantes correctas |
| CP-16 | PruebasInterfaz | `Eventos_SinResultadosNoPermiteAgregarFilas` | Correcta |
| CP-17 | PruebasInterfaz | `Eventos_ComprarSinResultadosMuestraAvisoCorrecto` | Correcta |
| CP-18 | PruebasInterfaz | `Eventos_GuiaRegistradaConFocoEnLaBusqueda` | Correcta |
| CP-19 | PruebasInterfaz | `Compra_VolverConfirmaSinPerderPaqueteAlCancelar` | Correcta |
| CP-20 | PruebasInterfaz | `Compra_CantidadInvalidaRestauraElPaquete` | 2 variantes correctas |
| CP-21 | PruebasInterfaz | `Propietario_EliminarRequiereConfirmacionYPreservaVentas` | Correcta |
| CP-22 | PruebasInterfaz | `Historial_SinSeleccionDistingueBotonDeshabilitado` | Correcta |
| CP-23 | PruebasInterfaz | `Historial_TildarYDestildarActualizaEstadoYColor` | Correcta |
| CP-24 | PruebasInterfaz | `Compra_CupoLiberadoVuelveAHabilitarServiciosDisponibles` | Correcta |
| CP-25 | PruebasInterfaz | `Compra_EscalaSimuladaMantieneAccionesDentroDelArea` | 3 variantes correctas |
| CP-26 | PruebasCuentas | `CP26_RegistroBronceSinSuscripcionYCorreoUnico` | Correcta |
| CP-27 | PruebasCuentas | `CP27_InicioDeSesionCorrecto` | Correcta |
| CP-28 | PruebasCuentas | `CP28_BloqueoTrasIntentosFallidos` | Correcta |
| CP-29 | PruebasCuentas | `CP29_DatosFaltantesOCorreoInexistente` | Correcta |
| CP-30 | PruebasCompra | `CP30_FiltrosYEventoNoVigente` | Correcta |
| CP-31 | PruebasCompra | `CP31_TopesDelPaquete` | Correcta |
| CP-32 | PruebasCompra | `CP32_PagoAprobadoConfirmacionQrYRegistro` | Correcta |
| CP-33 | PruebasCompra | `CP33_PagoRechazadoLiberaCupoYQuedaRegistrado` | Correcta |
| CP-34 | PruebasCompra | `CP34_ReservaVencida` | Omitida por hallazgo H-CODEX-01 |
| CP-35 | PruebasReportes | `CP35_SoloAdministradorAccede` | Correcta |
| CP-36 | PruebasReportes | `CP36_VentasCuentaSoloEntradasConfirmadas` | Correcta |
| CP-37 | PruebasReportes | `CP37_DemandaAgotadoYListaDeEspera` | Correcta |
| CP-38 | PruebasReportes | `CP38_ExportacionYPeriodoInvalido` | Correcta |
| CP-39 | PruebasInterfaz | `CP39_TemporizadorDeReservaALaVista` | Correcta |
| CP-40 | PruebasInterfaz | `CP40_CupoRestantePorSector` | Correcta |
| CP-41 | PruebasInterfaz | `CP41_ConfirmacionAlComprarSinSeguro` | Correcta |
| CP-42 | PruebasInterfaz | `CP42_ExplicacionDeLosNiveles` | Correcta |

## Hallazgo sin corregir: H-CODEX-01 / CP-34 / RN-15

Reproducción indicada por T-10:

1. Crear con Fake un evento de capacidad 5 e iniciar una compra de 2 entradas.
2. Comprobar disponibles = 3.
3. Retrasar únicamente `Compras.FechaCompra` en `RESERVA_MINUTOS + 1`, con el helper
   parametrizado solicitado, manteniendo `ReservaVence` tal como fue guardada.
4. Confirmar con la tarjeta válida de Fake.

Confirmar lanza RN-15, como se esperaba. Sin embargo, al releer la compra permanece
Pendiente, en lugar de Vencida. Mensaje exacto de la aserción:

```text
Error de Assert.AreEqual. Se esperaba <Vencida>, pero es <Pendiente>.
```

La lectura del código muestra dos criterios de vencimiento: `VencimientoDe` calcula a
partir de `FechaCompra`, mientras que `MarcarVencidas` utiliza `ReservaVence` si tiene
valor. En este escenario el primero venció y el segundo todavía está en el futuro.
El alcance comprobado es esa inconsistencia bajo la preparación exigida por T-10;
no se verificó aquí un vencimiento real por espera del reloj.

La ejecución se detuvo en la aserción de estado. Las aserciones posteriores de cupo
liberado y ausencia de pagos quedaron escritas, pero no fueron alcanzadas en esa corrida.
No se modificaron la aplicación, el helper para ajustar también `ReservaVence` ni las
aserciones para aceptar Pendiente.

Marca conservada:

```csharp
[Ignore("Hallazgo: CP-34 devuelve RN-15 pero la compra persiste Pendiente; se esperaba Vencida al retrasar FechaCompra.")]
```

No se encontraron otros defectos al ejecutar las tareas asignadas.

## Pendientes para pasar a docs/

1. **Registrar H-CODEX-01:** decidir cómo resolver la divergencia entre `FechaCompra` y
   `ReservaVence`. Después de la corrección autorizada, retirar el Ignore de CP-34 y
   comprobar sus cuatro resultados: RN-15, compra Vencida, cupo restituido y ningún pago.
   Mientras tanto CP-34 y la liberación por vencimiento no deben figurar como aprobadas
   por esta prueba en la matriz.
2. **Comprobación manual de RN-15 por reloj:** en una base de pruebas, iniciar un pago y
   dejar transcurrir `RESERVA_MINUTOS` sin modificar las fechas. Esperado: aviso de reserva
   vencida, compra y entradas Vencida, cupo restituido y ningún pago. Distinguir esta
   comprobación del escenario de fechas alteradas de CP-34. No ejecutada en esta tanda.
3. **Ajuste de sintaxis del encargo:** MSTest 3.6.4 expone `TestMethodAttribute.DisplayName`
   como propiedad de solo lectura. Se usó `[TestMethod("CP-xx: …")]`, que asigna el nombre
   mediante el constructor. Los DataRow sí conservan `DisplayName = "CP-xx: …"`. Reflejar
   esa sintaxis compatible al actualizar ejemplos; no requiere cambiar dependencias.
4. Al actualizar trazabilidad, incorporar CP-26 a CP-42, los nombres visibles de CP-11 a
   CP-25 y el estado pendiente explícito de CP-34. Esta tanda no verifica los pendientes
   manuales previos de otras escalas reales de Windows, cámaras de celular o lectores PDF.

## Archivos tocados

Modificados:

- `Pruebas/Fake.cs` (seis helpers solicitados, con SQL parametrizado).
- `Pruebas/PruebasFidelizacion.cs` (etiquetado CP-11).
- `Pruebas/PruebasFormularios.cs` (etiquetado CP-12).
- `Pruebas/PruebasInterfaz.cs` (etiquetado y CP-39 a CP-42).

Nuevos:

- `Pruebas/PruebasCuentas.cs`.
- `Pruebas/PruebasCompra.cs`.
- `Pruebas/PruebasReportes.cs`.
- `Pruebas/RESULTADOS-TAREAS-CODEX.md` (este reporte).

Las tres clases nuevas implementan TestInitialize, TestCleanup y ClassCleanup según el
encargo. Los datos nuevos se generan con Fake/registro con MailNuevo y quedan dentro de
la limpieza de Fake. No se modificaron datos semilla para preparar las pruebas.

`Visual/App.config` y `Pruebas/App.config` siguen con skip-worktree (S). Los dos documentos
sin seguimiento que ya estaban al iniciar (`docs/TAREAS-CODEX-PRUEBAS.md` y
`docs/MATRIZ-TRAZABILIDAD-v2.5.md`) se conservaron intactos. La compilación y las pruebas
generan los binarios/capturas habituales ignorados por git.
