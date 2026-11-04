# Cuarta tanda — T-22 a T-26

Fecha: 04/11/2026. Base: `7915c09`, rama `main`, árbol limpio al iniciar.
JC concedió el turno y autorizó expresamente el retiro del combo en el Designer (T-22)
y el ajuste de CP-24 (T-26). Se leyeron el estado de agentes, el encargo y las reglas.

## Resultado

Se completaron las cinco tareas en orden. Después de cada tarea se ejecutó:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Proyecto.ps1 -Accion Probar
```

| Tarea | Correctas | Omitidas | Fallidas |
|---|---:|---:|---:|
| T-22 | 51 | 0 | 0 |
| T-23 | 53 | 0 | 0 |
| T-24 | 58 | 0 | 0 |
| T-25 | 59 | 0 | 0 |
| T-26 | **60** | **0** | **0** |

Última salida resumida:

```text
La serie de pruebas se ejecutó correctamente.
Pruebas totales: 60
     Correcto: 60
Tiempo total: 7,4896 Segundos
Código de salida: 0
```

Compilación sin errores ni advertencias nuevas de compilación. CP-11 conserva la
referencia de $99.625. No se hicieron commits ni push. El trabajo se detuvo después
de T-26, sin medir usabilidad ni agregar mejoras fuera del encargo.

## Qué se hizo

### T-22 — alta pública solo de clientes

- Se agregó `GestorUsuarios.RegistrarCliente`, que fija el perfil Cliente y delega
  las validaciones existentes a `Registrar`. El método Registrar existente no cambió.
- `frmCrearUsuario` llama al método nuevo sin recibir un perfil desde la interfaz.
- **Designer declarado:** se retiraron el new, las propiedades, Controls.Add y el campo
  del ComboBox `cbTipoUsuario` de `Visual/_Inicio/frmCrearUsuario.Designer.cs`.
  No se movió ni redimensionó ningún otro control.
- CP-44 comprueba que no exista el combo, completa y envía el alta real y verifica
  que el usuario registrado sea Cliente. CP-45 comprueba Cliente y Bronce directamente
  desde el método de negocio nuevo.

No basta con quitar el ítem Propietario de una lista editable: se eliminó el selector
completo y se fijó el perfil en el método usado por el alta pública, como pidió T-22.

### T-23 — cierre de sesión al volver al inicio

Los manejadores `lblvolver_Click` del menú Cliente y del menú Propietario llaman primero
a `GestorSesion.Instance.Cerrar()`. Se mantuvo el resto de la navegación. Las dos filas
de CP-46 verifican sesión cerrada y un login visible.

### T-24 — cierre del proceso y confirmación de descarte con la X

- Se agregó `Visual/FormularioEventAura.cs` con la condición de última ventana visible
  y el cierre de aplicación protegido por `Application.MessageLoop`.
- **19 formularios concretos** pasaron a heredar de esa base; incluye
  `frmAgregarSector`, cuya declaración no tenía espacio antes de los dos puntos.
- La base es **pública y abstracta**: representa comportamiento común, no una pantalla
  adicional. Así se mantienen los 19 formularios concretos y CP-12 sin cambios.
- Se agregó su entrada Compile en `Visual/Visual.csproj`, necesaria porque este proyecto
  enumera sus archivos fuente explícitamente.
- La compra sobrescribe OnFormClosing y pide confirmación con No por defecto cuando
  hay un paquete armado y el motivo es UserClosing. Un paquete vacío cierra sin aviso.
- CP-47 verifica los cuatro casos de salida. CP-48 verifica No/Sí y el cierre vacío.

#### UI Automation sobre el ejecutable

Después de T-24 se ejecutó, sin modificarlo:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\Usabilidad\VerificarCierreConX.ps1
```

Salida, código de salida 0:

```text
1. Sesión iniciada; ventanas visibles del proceso: 1
2. Listado abierto; ventanas visibles: 1
3. Listado cerrado con la X: proceso vivo = False; ventanas visibles = 0; memoria = 0 MB
```

El proceso terminó por el cierre con la X; no fue necesario que el finally del script
lo detuviera. Esta verificación se hizo sobre Visual.exe, además de la batería automática.
El script existente usa la configuración local de la aplicación y se autentica como
plata@eventaura.com contra la base de desarrollo. La batería usa exclusivamente la
base recreable `BddEventAura_Pruebas`; no se recreó la base de desarrollo.

### T-25 — indicación de la cantidad

En CargarSectores, sin modificar Designer:

- Se mantuvo el nombre interno Cantidad y se cambió su encabezado a
  «Cantidad (escribí aquí)».
- Cada celda tiene ToolTipText con el máximo leído de COMPRA_ENTRADAS_MAX.
- Se selecciona Cantidad en la primera fila con Disponibles > 0. Si no existe, no se
  cambia la celda por esta regla.
- CP-49 comprueba la selección disponible, el encabezado y la ayuda configurada.

### T-26 — Comprar y Seguro requieren entradas

- El estado de agotamiento quedó en un campo del formulario.
- ActualizarHabilitados concentra las condiciones de Comprar y Seguro y destilda el
  seguro si queda deshabilitado. Se llama al terminar MostrarSiAgotado y ActualizarResumen.
- Los servicios conservan la lógica de disponibilidad del evento.
- CP-50 verifica el estado inicial, una entrada, el regreso a cero, la eliminación
  del tilde del seguro y que Hospedaje no se deshabilite por no haber entradas.
- Se ajustó CP-24 según la autorización: al recuperarse cupo, Comprar está deshabilitado
  hasta elegir una entrada; después queda habilitado.

Se compararon los cuerpos de los métodos previos de PruebasInterfaz con HEAD: solo
cambió `Compra_CupoLiberadoVuelveAHabilitarServiciosDisponibles` (CP-24). No se alteraron
las otras aserciones existentes ni se agregaron Ignore.

## Casos nuevos

| CP | Método | Ejecuciones | Resultado |
|---|---|---:|---|
| CP-44 | `PruebasInterfaz.CP44_AltaPublicaSinSelectorDePerfil` | 1 | Correcta |
| CP-45 | `PruebasCuentas.CP45_AltaPublicaDeNegocioCreaClienteBronce` | 1 | Correcta |
| CP-46 | `PruebasInterfaz.CP46_VolverAlInicioCierraLaSesion` | 2 | Correctas |
| CP-47 | `PruebasInterfaz.CP47_CierreDeUltimaVentanaVisible` | 4 | Correctas |
| CP-48 | `PruebasInterfaz.CP48_CerrarCompraConfirmaDescarte` | 1 | Correcta |
| CP-49 | `PruebasInterfaz.CP49_CantidadIndicadaYSeleccionada` | 1 | Correcta |
| CP-50 | `PruebasInterfaz.CP50_CompraYSeguroRequierenEntradas` | 1 | Correcta |

Son siete IDs y once ejecuciones nuevas: catálogo CP-01 a CP-50 y batería de 60.

## Archivos tocados para revisión de JC

Modificados:

- `Repo/GestorUsuarios.cs`.
- `Pruebas/PruebasCuentas.cs`.
- `Pruebas/PruebasInterfaz.cs`.
- `Visual/Visual.csproj`.
- `Visual/_Inicio/frmCrearUsuario.Designer.cs` (único Designer modificado).

Los 19 archivos de formulario modificados, por herencia y, donde corresponde, por
el cambio funcional de esta tanda:

- `Visual/_Inicio/frmInicio.cs`.
- `Visual/_Inicio/frmCrearUsuario.cs`.
- `Visual/_Inicio/frmCambiarContraseña.cs`.
- `Visual/Cliente/frmMenuCliente.cs`.
- `Visual/Cliente/frmEventosCliente.cs`.
- `Visual/Cliente/FrmCompraCliente.cs`.
- `Visual/Cliente/frmPago.cs`.
- `Visual/Cliente/frmConfirmacion.cs`.
- `Visual/Cliente/frmHistorialCliente.cs`.
- `Visual/Cliente/frmSuscripcion.cs`.
- `Visual/Propietario/frmMenuPropietario.cs`.
- `Visual/Propietario/frmMisLocales.cs`.
- `Visual/Propietario/frmRegistrarLocal.cs`.
- `Visual/Propietario/frmEventosLocal.cs`.
- `Visual/Propietario/frmAgregarEvento.cs`.
- `Visual/Propietario/frmAgregarSector.cs`.
- `Visual/Propietario/frmAdministrarEvento.cs`.
- `Visual/Propietario/frmListasEspera.cs`.
- `Visual/Propietario/frmReportes.cs`.

Nuevos:

- `Visual/FormularioEventAura.cs`.
- `Pruebas/RESULTADOS-TAREAS-CODEX-2026-11-04-T22-T26.md` (este reporte).

No se modificaron Datos/, Entidade/, BaseDatos/, docs/, tools/, CLAUDE.md ni AGENTS.md.
Ambos App.config siguen intactos y protegidos con skip-worktree (S).

## Pendientes, límites y próximo turno de Claude Code

No quedaron pruebas fallidas ni omisiones. No se encontraron otros defectos al ejecutar
esta lista. No se implementó el manejador global de errores ni se amplió el alcance a
los asuntos que ESTADO-AGENTES deja pendientes de decisión de JC.

**El estudio de usabilidad vigente describe la aplicación anterior a estas correcciones.**
Las cifras de acciones, clics, tiempos, satisfacción y SUS no se recalcularon en este
turno y no deben presentarse como medidas de esta versión.

Claude Code debería, en su próximo turno:

1. Mover este reporte a docs/ y actualizar ESTADO-AGENTES con T-22 a T-26 terminadas.
2. Volver a ejecutar `tools/Usabilidad/Medir.ps1`, `VerificarCierreConX.ps1` y `modelo.py`,
   ajustando las reglas a los hallazgos corregidos antes de recalcular resultados.
3. Actualizar el informe y los ítems 3, 4, 11b, 18, 19, 20 y 21 de
   CORRECCIONES-DOCUMENTO.md que dependen del estudio.
4. Registrar CP-44 a CP-50, la batería de 60 y el ajuste autorizado de CP-24 en
   `docs/04-casos-prueba.md` y `docs/COMO-SE-PROBO.md`, y las referencias de contexto
   que correspondan.
5. Revisar si las capturas del instructivo muestran el encabezado y los estados de
   botones anteriores, para decidir su actualización dentro de su zona.

Hasta esa nueva medición, el informe de usabilidad no es definitivo. Los pendientes
manuales previos (escalas reales, QR con cámara, teclado y reserva por reloj) no se dan
por cerrados por esta tanda. Codex terminó su lista y se detuvo.
