# EventAura: guion ejecutable de demostración

Actualizado el 17/11/2026 para la fase 3, el diagnóstico de esquema y el alta de administradores. La batería completa se ejecutó en la estación de desarrollo: **302/302**. La notebook de Máximo debe quedar en `main` actualizado antes de empezar; no se modifican archivos `App.config` del repositorio.

**Corrección importante:** la configuración real bloquea al **quinto** intento incorrecto, durante **15 minutos**. Tres intentos no bloquean. No anuncies tres delante del profesor.

Los importes de este guion requieren empezar con la semilla limpia y seguir el orden. Las fechas, los números de pedido y los QR se generan al ejecutar: donde aparece una fecha o un pedido variable, no esperes el número del ensayo.

## 0. Compilar, preparar y abrir
### 0.0 Actualizar el código y revisar los scripts de base

En la notebook de Máximo abrí PowerShell en la carpeta del repositorio. La rama de presentación debe ser `main`, que incluye los siete scripts numerados. Primero revisá si hay trabajo local:

```powershell
git status --short --branch
git fetch origin
git switch main
git pull origin main
git log --oneline -1
```

Si `git status` muestra cambios locales o `git switch` se niega, no los borres ni los guardes encima: detenete y pedí que se preserven. El commit de publicación de esta actualización es `91aa64e` (o uno posterior de `main`). **Ves:** el nombre de rama `main` y un commit actualizado. **Importa:** `ambro` está atrasada y no contiene todo el código de la demo.

Antes de abrir el programa, revisá `BaseDatos/01-esquema.sql` a `BaseDatos/07-indice-unico-seguros.sql`. Abrí SQL Server Management Studio y conectate a la instancia de la notebook. En el selector de base de datos elegí la base destino, abrí una consulta con **Nueva consulta**, ejecutá `SELECT DB_NAME();` con **Ejecutar (F5)** y confirmá visualmente el nombre antes de seguir. Para una base nueva, creá y seleccioná una base vacía; abrí `BaseDatos/01-esquema.sql` con **Archivo > Abrir > Archivo**, ejecutalo con **F5**, y repetí con `02-semilla.sql`. Para una base ya existente, no ejecutes `01` ni `02`: abrí y ejecutá, en orden, `04-seguridad-ingreso.sql`, `05-check-estado-entradas.sql`, `06-prioridad-temporal.sql` y `07-indice-unico-seguros.sql`, siempre con la base destino seleccionada. Los scripts `04` a `07` son idempotentes. `01` y `02` se ejecutan solo para crear y poblar una base vacía. `03-datos-reporte.sql` es una carga voluminosa de medición; no la uses para la demostración normal.

El arranque ahora muestra dentro del login el primer requisito faltante y el script exacto, por ejemplo `Sectores.CupoLiberadoEl` y `BaseDatos/06-prioridad-temporal.sql`. No sigas hasta ejecutar ese script en la base de la copia y volver a abrir. **Importa:** se ve qué migración falta sin recibir una excepción técnica.

Reservá la base de pruebas y el escritorio: nadie debe correr pruebas en paralelo. Cerrá las ventanas de EventAura antes de cada comando que ejecute pruebas. Estas recrean la base terminada en `_Pruebas`; no conservan las compras de la demostración.

### 0.1 Compilar

Abrí **Windows PowerShell**, no la consola de SQL. Pegá:

```powershell
cd D:\proyectos\TrabajoDeDiploma
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Proyecto.ps1 -Accion Compilar
if ($LASTEXITCODE -ne 0) { throw 'No continuar: falló la compilación' }
```

**Ves:** entre las salidas de proyectos, `Visual -> ...\Visual\bin\Debug\Visual.exe`, sin errores. **Importa:** el ejecutable corresponde al código que vas a defender. Se necesitan las dependencias instaladas de esta PC; en otra máquina no continúes si la compilación falla.

### 0.2 Preparar comandos y una copia aislada

Pegá este bloque en esa misma consola y mantenela abierta durante el recorrido:

```powershell
$ErrorActionPreference = 'Stop'
$demo = Join-Path (Get-Location).Path 'TestResults/Demo'
[void][IO.Directory]::CreateDirectory($demo)
Get-ChildItem -LiteralPath (Join-Path (Get-Location).Path 'Visual/bin/Debug') |
    Copy-Item -Destination $demo -Recurse -Force
Copy-Item -LiteralPath Pruebas/bin/Debug/net48/Pruebas.dll.config `
    -Destination (Join-Path $demo 'Visual.exe.config') -Force
$configDemo = [xml](Get-Content -LiteralPath (Join-Path $demo 'Visual.exe.config') -Raw)
$cadenaDemo = ($configDemo.configuration.connectionStrings.add |
    Where-Object { $_.name -eq 'Visual.Properties.Settings.BddEventAura' }).connectionString
$destinoDbDemo = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $cadenaDemo
if (-not $destinoDbDemo.InitialCatalog.EndsWith('_Pruebas')) {
    throw 'DETENIDO: la copia no apunta a _Pruebas'
}
$destinoDbDemo.InitialCatalog
function Demo-Sql([string]$sql) {
    $cn = New-Object System.Data.SqlClient.SqlConnection $cadenaDemo
    $cmd = $cn.CreateCommand(); $cmd.CommandText = $sql
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $tabla = New-Object System.Data.DataTable
    try { $cn.Open(); [void]$adapter.Fill($tabla); return ,$tabla }
    finally { $adapter.Dispose(); $cmd.Dispose(); $cn.Dispose() }
}
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
$vstest = Join-Path $vs 'Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe'
$netstandard = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8/Facades/netstandard.dll'
```

**Ves:** `BddEventAura_Pruebas` en esta PC. Si no termina en `_Pruebas`, detenete. **Importa:** la demostración usa una copia del ejecutable con la configuración de pruebas; no cambia las conexiones locales de Juan Cruz ni los archivos de Máximo. Todos los `Demo-Sql` siguientes usan exclusivamente esa conexión.

Prepará la semilla pequeña mediante un caso existente:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Pruebas\EjecutarBateriaSegura.ps1 -Filtro 'FullyQualifiedName~CP128_Compra'
if ($LASTEXITCODE -ne 0) { throw 'No continuar: falló la preparación' }
```

**Ves:** una prueba, una correcta. **Importa:** la inicialización crea el esquema y la semilla, y el caso limpia sus datos temporales. La preparación CP-128 se ejecuta en una base recreada de `_Pruebas`; la batería completa deja además el juego voluminoso de reportes. Para repetir los importes del instructivo usá esta preparación pequeña.

Abrí **esta copia**, no el ejecutable habitual del proyecto:

```powershell
Start-Process -FilePath (Join-Path $demo 'Visual.exe') -WorkingDirectory $demo -WindowStyle Normal
```

**Ves:** `EventAura · Iniciar sesión`. **Importa:** desde ahora los clics trabajan sobre la base de demostración. Para cambiar de persona podés cerrar EventAura y ejecutar de nuevo este último comando; así evitás confundir sesiones.

### 0.3 Usuarios reales, verificados mediante el formulario de acceso

| Persona de la semilla | Correo | Contraseña | Qué abre |
|---|---|---|---|
| Ana Administradora | `admin@eventaura.com` | `Admin1234` | `Administración` |
| Paula Plata | `plata@eventaura.com` | `Cliente123` | `Mi cuenta` |
| Oscar Oro | `oro@eventaura.com` | `Cliente123` | `Mi cuenta` |
| Bruno Bronce | `bronce@eventaura.com` | `Cliente123` | `Mi cuenta` |

En el acceso, completá correo y contraseña y pulsá **Iniciar sesión**. El administrador se almacena con el rol de propietario; no hay que inventar una quinta cuenta. Estas son credenciales de la **semilla de demostración**, no recomendaciones para producción.


### 0.4 Mostrar el panel y crear una cuenta administradora

Para mostrar la administración, cerrá la sesión actual y abrí EventAura otra vez. Entrá como Ana con `admin@eventaura.com` / `Admin1234`. En **Administración** vas a ver los cinco indicadores de reportes y el botón **Crear administrador**. Pulsalo: aparece el formulario **Crear administrador**. Completá nombre, apellido, un correo que no esté usado y la misma contraseña válida en los dos campos; pulsá **Crear administrador**. **Ves:** `Usuario creado con éxito.` y volvés al panel. **Importa:** el alta administrativa está disponible dentro del panel; desde el alta pública solo se crean clientes. Este recorrido se ejecutó con el formulario y el gestor reales (CP-166).

## 1. Que funciona: de elegir a tener la entrada

| Paso: acción exacta | Qué tenés que ver | Por qué importa |
|---|---|---|
| Entrá como Paula. En `Mi cuenta` comprobá su nivel, y pulsá **Eventos disponibles**. En la tarjeta `Duki en Rosario`, pulsá **Ver entradas**. | La barra muestra Paula y Plata; aparece la tarjeta con portada generada y después la ficha del evento con fecha, lugar y mapa de sectores. | La identidad, la portada y la disponibilidad se construyen con los datos existentes. |
| En la ficha, pulsá **Armar paquete** y elegí **Campo** en el mapa si no quedó elegido. | Se abre el armado con el paso `2 Armá` y el nivel Plata con `5,0 % de descuento`. | La ficha y el mapa llevan al mismo armado de compra; el descuento sale del historial. |
| En la fila **Campo**, editá **Cantidad**: Ctrl+A, `2`, Tab. Marcá únicamente **Hospedaje**. Dejá el seguro sin marcar. | `TOTAL $ 99.625,00`, destacado en violeta. Desplazá el contenido y el resumen si quedan fuera de la vista. | Son los mismos componentes e importe del instructivo. |
| Pulsá **Comprar**. En **Seguro de entrada**, pulsá **Sí** para continuar sin seguro. | `No agregaste el seguro de entrada. Solo se puede contratar ahora: después de pagar no se puede incorporar.` Luego `¿Continuar sin seguro?` | El sistema avisa que el seguro se decide antes de confirmar. |
| En pago completá titular `Paula Plata`, tarjeta `4111111111111111`, vencimiento `12/28`, código `123`, y pulsá **Pagar**. | Paso `3 Pagá`, `Total a pagar: $ 99.625,00`; después `4 Listo`, `¡Compra confirmada!`, `Pedido EA-…` y `TOTAL $ 99.625,00`. | La confirmación cierra la operación y muestra su identificador. |
| Pulsá **Abrir carpeta** y abrí el PDF de Campo de ese pedido. Después cerrá la confirmación. | Carpeta `A1.Tickets EVENTAURA` dentro de Documentos; PDF con EventAura, Duki, Campo y QR. Al cerrar la confirmación volvés al listado. | La entrada existe como comprobante utilizable y el recorrido no termina en un armado vacío. |

La cuenta es: entradas $50.000 − descuento $2.500 + hospedaje $45.000 + cargo $7.125 = **$99.625**. El PDF es una entrada por localidad: su precio individual no es una factura por el total del paquete. El pago es **simulado**; no se hizo un cargo bancario.

## 2. Que las reglas se cumplen

### 2.1 Preparación: una entrada asegurada y una ventana controlada

La compra del instructivo no tiene seguro. Para probar específicamente el rechazo por ingreso, creá **otra compra de Paula**: Duki, Campo, cantidad **1**, **Seguro** marcado, ningún servicio. Pagá con los mismos datos. **Ves:** `TOTAL $ 32.312,50` y una nueva confirmación. **Importa:** no confundís el rechazo por entrada usada con el rechazo por falta de seguro.

Cerrá EventAura. En la consola:

```powershell
$compraAsegurada = [int](Demo-Sql "SELECT MAX(ID) AS ID FROM Compras WHERE ID_Usuario=3 AND ConSeguro=1 AND Estado='Confirmada'").Rows[0].ID
Demo-Sql "SELECT CodigoQR,FechaIngreso FROM Entradas WHERE ID_Compra=$compraAsegurada" | Format-Table -AutoSize
Demo-Sql "UPDATE Eventos SET Fecha=DATEADD(HOUR,72,GETDATE()) WHERE ID=1;
UPDATE Parametros SET Valor='96' WHERE Clave='INGRESO_HORAS_ANTES';"
Start-Process -FilePath (Join-Path $demo 'Visual.exe') -WorkingDirectory $demo -WindowStyle Normal
```

Copiá el `CodigoQR` que devuelve la consulta. **Ves:** un código y `FechaIngreso` vacía. **Importa:** normalmente el ingreso abre 6 horas antes y el reembolso exige más de 48: esas ventanas no se cruzan. Este montaje explícito, solo en pruebas, abre el ingreso 96 horas antes y sitúa el evento a 72 horas para demostrar la defensa independiente de entrada usada. Hay que **reiniciar** EventAura: los parámetros se cargan por proceso.

### 2.2 La misma entrada dos veces

1. Entrá como `admin@eventaura.com`. Pulsá **Control de ingreso**, elegí **Duki en Rosario**, pegá el QR y pulsá **Registrar ingreso**. **Ves:** `Ingreso válido. Bienvenido.` **Importa:** el primer uso es válido.
2. Pulsá **Registrar ingreso** con exactamente el mismo código otra vez. **Ves:** `La entrada ya ingresó el DD/MM/AAAA HH:mm:ss.` con el instante del primer registro. **Importa:** no se consume dos veces una entrada, aunque se copie el QR.

### 2.3 Reembolso después del ingreso

1. Cerrá y reabrí EventAura, entrá como Paula y pulsá **Mis eventos**. Elegí la **nueva compra asegurada**, no las compras históricas de la semilla. Marcá **Devolver** en su entrada y pulsá **Devolver 1 entrada(s)**. **Ves:** confirmación `¿Devolver 1 entrada(s)?`, con las condiciones del reintegro. **Importa:** se está operando la entrada que acabás de usar.
2. Pulsá **Sí**. **Ves:** cuadro `Devolución` con **`No podés devolver una entrada que ya se usó para ingresar. (RN-29)`**. **Importa:** la regla sigue protegiendo aun cuando la configuración permite cruzar las ventanas temporales.

### 2.4 Otro cliente no puede leer esa compra

La interfaz no ofrece un selector para leer compras de otro. No inventes un clic: esta parte muestra una petición forzada al gestor, autenticada como Oscar, desde una pequeña consola. No modifica código del repositorio. Con EventAura cerrado, pegá:

```powershell
@'
using System;
using Entidades;
using Negocio;
class AccesoAjeno {
    static int Main(string[] args) {
        GestorUsuarios.Instance.Autenticar("oro@eventaura.com", "Cliente123");
        try {
            GestorCompras.Instance.PagosDe(new Compra { Id = int.Parse(args[0]) });
            Console.WriteLine("ERROR: se devolvieron datos ajenos"); return 1;
        } catch (ReglaNegocioException ex) {
            Console.WriteLine(ex.Message + " (" + ex.Regla + ")");
            return ex.Regla == "RN-41" ? 0 : 2;
        }
    }
}
'@ | Set-Content -LiteralPath (Join-Path $demo 'AccesoAjeno.cs') -Encoding UTF8
& "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo /r:System.Core.dll "/r:$netstandard" "/r:$demo\Repositorio.dll" "/r:$demo\Entidades.dll" "/out:$demo\AccesoAjeno.exe" (Join-Path $demo 'AccesoAjeno.cs')
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación del auxiliar' }
Copy-Item -LiteralPath (Join-Path $demo 'Visual.exe.config') -Destination (Join-Path $demo 'AccesoAjeno.exe.config') -Force
$compraPaula = [int](Demo-Sql "SELECT MAX(ID) AS ID FROM Compras WHERE ID_Usuario=3 AND Estado='Confirmada'").Rows[0].ID
& (Join-Path $demo 'AccesoAjeno.exe') $compraPaula
```

**Ves en consola:** `Solo puede operar sobre los datos de tu propia cuenta. (RN-41)`. **Importa:** la autorización está en negocio, aunque se saltee la interfaz. Este auxiliar llama al sistema real; no es una pantalla ni una prueba con sesión ficticia.

## 3. Que es seguro

### 3.1 Bloqueo y rechazo uniforme

Abrí la copia y usá **Bruno**, para no bloquear a Paula ni a Ana.

| Acción | Qué ves | Por qué importa |
|---|---|---|
| Intentá entrar **tres veces** con `bronce@eventaura.com` y contraseña incorrecta, aceptando cada aviso. | Siempre `Mail o contraseña incorrectos. (RN-04)`. Todavía no está bloqueado. | La política real es cinco, no tres. |
| Hacé otros **dos** intentos incorrectos. | Exactamente el mismo mensaje. | Se alcanza el máximo sin revelar el contador al visitante. |
| Probá ahora ese correo con la contraseña correcta `Cliente123`. | El mismo rechazo; no aparece `Mi cuenta`. | La cuenta está bloqueada durante 15 minutos. |
| Probá `noexiste-demostracion@eventaura.com` con la misma contraseña. | **El mismo texto:** `Mail o contraseña incorrectos. (RN-04)`. | El mensaje no permite enumerar correos existentes. No afirmes que los tiempos sean idénticos. |

Si querés mostrar el estado interno, después del tercer intento y después del quinto ejecutá:

```powershell
Demo-Sql "SELECT IntentosFallidos,BloqueadoHasta FROM Usuario WHERE Mail='bronce@eventaura.com'" | Format-Table -AutoSize
```

**Ves:** contador 3 sin bloqueo; luego contador 5 y fecha de desbloqueo. No pruebes una contraseña correcta entre el tercero y el quinto: resetearía la cuenta. **Importa:** el bloqueo existe aunque el aviso se mantenga genérico.

### 3.2 Prioridad de 15 minutos, con el mismo cupo antes y después

Preparación explícita, con EventAura cerrado:

```powershell
Demo-Sql "UPDATE Eventos SET Fecha=DATEADD(DAY,30,GETDATE()) WHERE ID=3;
UPDATE Sectores SET Capacidad=CASE WHEN Nombre='General' THEN 1 ELSE 0 END WHERE EventoID=3;"
Start-Process -FilePath (Join-Path $demo 'Visual.exe') -WorkingDirectory $demo -WindowStyle Normal
```

**Ves:** **Noche de Teatro Independiente** como evento futuro. **Importa:** el escenario queda con un único lugar para observar una liberación real, sin cargar miles de entradas.

1. **Oscar**, que no es suscriptor: comprá Noche, **General**, cantidad 1, **Seguro** marcado y ningún servicio. Pagá. **Ves:** `TOTAL $ 20.956,25` y confirmación. **Importa:** queda agotado el único cupo.
2. **Paula**: **Mi suscripción**, plan **Mensual**, **Contratar**, confirmá **Sí**. **Ves:** inicialmente `No tenés una suscripción activa.`; confirmación `¿Contratar el plan Mensual por $ 9.000,00?` y aviso de que no se renueva automáticamente; después `Suscripción Mensual contratada. Vence el …`. **Importa:** hay una suscriptora real para la prioridad.
3. Paula: volvé a **Eventos disponibles**, elegí Noche y entrá a su compra. Pulsá **Anotarme en la lista de espera**. **Ves:** `Entradas agotadas.` y luego `Te anotaste en la lista de espera de «Noche de Teatro Independiente». Podés consultar tu posición. Si vuelve a haber entradas, te lo mostramos al iniciar sesión.` **Importa:** la prioridad requiere suscripción **e inscripción**, no alcanza con pagar el plan.
4. **Oscar**: **Mis eventos**, seleccioná su nueva compra de Noche, marcá **Devolver**, pulsá **Devolver 1 entrada(s)** y confirmá **Sí**. **Ves:** `Devolución registrada. Se reintegran $ 13.875,00.` **Importa:** la devolución libera el lugar y marca el inicio de la ventana.
5. Sin esperar 15 minutos, Oscar vuelve a Noche e intenta comprar General, cantidad 1, con Seguro. **Ves:** cuadro `No se puede completar la compra`: **`El lugar liberado está reservado temporalmente para suscriptores inscriptos en la lista de espera. Intentá al finalizar la ventana de prioridad. (RN-35)`**. **Importa:** un no suscriptor realmente queda impedido de adelantarse.
6. Para ver el vencimiento podés esperar los **15 minutos reales** desde la devolución. Para una exposición corta, cerrá EventAura y ejecutá esta **simulación declarada del paso del tiempo**, exclusivamente en pruebas:

```powershell
Demo-Sql "UPDATE Sectores SET CupoLiberadoEl=DATEADD(MINUTE,-16,GETDATE()) WHERE EventoID=3 AND Nombre='General';"
Start-Process -FilePath (Join-Path $demo 'Visual.exe') -WorkingDirectory $demo -WindowStyle Normal
```

7. Oscar repite la misma compra. **Ves:** ahora llega a pago, `Total a pagar: $ 20.956,25`, y puede confirmar. **Importa:** la exclusividad termina: fuera de la ventana vuelve a poder comprar cualquiera. No digas que esperaste si adelantaste el instante en SQL; no se cambia la duración de la regla.

## 4. Que está probado de verdad

### 4.1 Batería completa

Cerrá EventAura. No ejecutes esto con otro agente usando la base o el escritorio:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Pruebas\EjecutarBateriaSegura.ps1
```

**Ves al final:**

```text
La serie de pruebas se ejecutó correctamente.
Pruebas totales: 302
     Correcto: 302
```

**Importa:** son ejecuciones reales, incluidas las filas parametrizadas. Medición observada en esta estación: **1,0468 minutos de pruebas**, además de compilar. El conteo puede aumentar en commits posteriores; anotá la salida real y no presentes otro número: detené esta parte y guardá la salida.

### 4.2 Una defensa rota delante del profesor

Elegida: **solo un administrador puede consultar nombres y correos de la lista de espera**. Es una línea legible y el caso tarda aproximadamente un segundo; no requiere explicar criptografía.

**Archivo:** `Repo/GestorListaEspera.cs`, **línea 91**, dentro de `DeEvento`. **Prueba:** `CP126_DeEvento`, identificada como **`CP-126: DeEvento exige administrador antes de devolver datos personales`**, en `Pruebas/PruebasGuardasMutacion.cs`.

1. Antes de editar, en la consola de la preparación:

```powershell
git diff -- Repo/GestorListaEspera.cs
```

**Ves:** ninguna salida. Si ya hay cambios, **no sigas ni los sobrescribas**. **Importa:** el respaldo debe ser de la defensa original.

```powershell
$rutaDefensa = (Resolve-Path 'Repo/GestorListaEspera.cs').Path
$copiaDefensa = Join-Path $demo 'GestorListaEspera.cs.original'
Copy-Item -LiteralPath $rutaDefensa -Destination $copiaDefensa -Force
$shaOriginal = (Get-FileHash -LiteralPath $rutaDefensa -Algorithm SHA256).Hash
notepad.exe $rutaDefensa
```

2. En `DeEvento`, comentá **solo** esta línea y guardá:

```csharp
// Antes
GestorSesion.Instance.RequerirAdministrador();
// Después
// GestorSesion.Instance.RequerirAdministrador();
```

No agregues las etiquetas «Antes/Después» al archivo. Ejecutá `git diff -- Repo/GestorListaEspera.cs`: **ves una sola línea cambiada**. **Importa:** quitaste el control de autorización, no deshabilitaste la prueba.

3. Compilá y ejecutá únicamente el caso:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Proyecto.ps1 -Accion Compilar
if ($LASTEXITCODE -ne 0) { throw 'No cuenta como prueba roja: no compiló. Restaurá abajo.' }
$ErrorActionPreference = 'Continue'
& $vstest .\Pruebas\bin\Debug\net48\Pruebas.dll '/TestCaseFilter:FullyQualifiedName~CP126_DeEvento'
```

**Ves:** una prueba incorrecta y:

```text
Error de Assert.ThrowsException. Tipo de excepción esperada:<Negocio.ReglaNegocioException> pero no se lanzó ninguna excepción.
```

**Importa:** sin la defensa, se devolvieron datos personales cuando debía haber un rechazo. La prueba detecta el cambio. Ensayo final: **1,2013 segundos** de ejecución de pruebas; compilación aparte.

4. **Restaurá siempre**, aunque el paso anterior haya fallado de otra manera. Cerrá el editor sin guardar otra modificación y ejecutá:

```powershell
[IO.File]::WriteAllBytes($rutaDefensa, [IO.File]::ReadAllBytes($copiaDefensa))
if ((Get-FileHash -LiteralPath $rutaDefensa -Algorithm SHA256).Hash -ne $shaOriginal) {
    throw 'El archivo no coincide con el original'
}
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Proyecto.ps1 -Accion Compilar
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación restaurada' }
& $vstest .\Pruebas\bin\Debug\net48\Pruebas.dll '/TestCaseFilter:FullyQualifiedName~CP126_DeEvento'
git diff -- Repo/GestorListaEspera.cs
```

**Ves:** una prueba correcta y ninguna diferencia del archivo. Ensayo final restaurado: **1,1518 segundos** de ejecución de pruebas. **Importa:** restauraste exactamente los bytes y comprobaste otra vez la defensa. Se escribe el respaldo con fecha nueva a propósito: copiarlo conservando una fecha antigua puede hacer que la compilación incremental reutilice la DLL mutada.

No commitees ni publiques la mutación. Para repetir la compra desde cero, cerrá la aplicación y repetí **0.2 desde la ejecución de CP128_Compra**. Eso también restaura los parámetros, las fechas, los cupos y la cuenta bloqueada.

## Duración y selección para quince minutos

Los tiempos humanos siguientes son **estimaciones para alguien que ya ensayó**, no tiempos medidos de Juan Cruz. El ensayo automatizado ejecuta formularios mucho más rápido que una persona.

| Parte | Tiempo de exposición estimado | Tiempo observado de ejecución automática relevante |
|---|---|---|
| Preparación, fuera de la exposición | 3–5 min si el entorno está instalado | Semilla con CP-128: alrededor de 1 s de pruebas; apertura y compilación verificadas |
| 1. Compra y PDF | 4–5 min | Recorrido real de compra verificado |
| 2. Ingreso, devolución y acceso ajeno | 5–7 min con preparación lista | Rechazos y auxiliar ejecutados |
| 3. Bloqueo y prioridad | 7–10 min con salto de tiempo declarado | Cinco intentos, rechazo uniforme y ambos lados de prioridad ejecutados |
| 4. Batería y mutación/restauración | 3–4 min | Batería ~61 s; rojo 1,2013 s; verde 1,1518 s; compilaciones aparte |

Todo junto requiere aproximadamente **19–26 minutos**, preparación aparte; esperar la prioridad de verdad agrega unos 15 minutos. No prometas mostrarlo todo en quince.

**Selección recomendada de 15 minutos:** entorno ya listo; compra y PDF (5), acceso ajeno con auxiliar ya compilado (1), bloqueo y mensaje uniforme (2), batería más mutación/restauración (4), introducción y preguntas (3). Dejá ingreso/reembolso y prioridad como ampliación. Antes de la exposición, practicá una vez sin público con cronómetro.

## Qué no mostrar o prometer

- No uses la base habitual ni el ejecutable habitual: esta preparación es destructiva sobre `_Pruebas`, y los cambios de escenario son solo de demostración.
- No prometas bloqueo al tercer intento ni un aviso distinto para cuentas bloqueadas: el máximo es cinco y el rechazo genérico es intencional.
- No presentes el salto de 16 minutos como espera real. No intentes mostrar prioridad sin una suscriptora inscripta y una liberación efectiva del lugar.
- No intentes devolver la primera compra, que no tiene seguro. No escondas la preparación que cruza las ventanas: la configuración normal impide llegar naturalmente a ese escenario.
- Las compras históricas de la semilla sirven para los niveles y pueden mostrar `(sin detalle de entradas)`. Para QR, PDF y devolución elegí las compras nuevas de este recorrido.
- No repitas compras sin reiniciar la semilla esperando los mismos totales: el historial cambia el nivel y el descuento.
- No presentes la tarjeta o `HTTPS simulado` como integración bancaria real. El PDF es un ticket por entrada, no una factura fiscal.
- No hay un botón para espiar otra cuenta: la petición forzada se muestra en consola y justamente debe ser rechazada.
- No ejecutes batería o mutación mientras alguien usa la base de pruebas, ni dejes la línea de autorización comentada al terminar.
- **302/302 no significa ausencia de todo defecto.** La medición global de mutación publicada sigue siendo **73/77 (94,8 %) sobre `65225e7`**. Esta demostración dirigida verifica una defensa; no reemplaza esa medición ni cambia su cifra.

No se modificaron código permanente, `docs/` ni los `App.config` fuente para preparar este guion. Las copias, auxiliares y registros del ensayo quedan en `TestResults/`, ignorado por Git.
