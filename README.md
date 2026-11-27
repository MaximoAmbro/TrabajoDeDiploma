# Pegale una leidita al readme antes que le meti un instructivo a como hacer las pruebas y eso

# EventAura

Plataforma de venta de entradas para espectáculos de Rosario, con armado personalizado de
paquetes (entrada + hospedaje + traslado + merchandising + experiencia VIP).
Trabajo de Campo I — Universidad Abierta Interamericana.

## Abrir la aplicación desde cero

Comprobado el 27/11/2026 sobre un clon limpio de la rama `JC` (`ed23ac0`), en Windows 11 con
Visual Studio 2022 Community y SQL Server 2022 Express. Detalle de la comprobación:
`docs/VERIFICACION-MUESTRA-1-2026-11-27.md`, sección 7.

### Requisitos

- **Windows 10 u 11.**
- **Visual Studio 2022** con la carga de trabajo «Desarrollo de escritorio de .NET».
- **SQL Server 2017 o posterior** (Express alcanza), con autenticación de Windows y permiso para
  crear bases. Para ejecutar los scripts sirve SQL Server Management Studio o `sqlcmd`.
- **Runtime de WebView2.** Viene con Windows 11 y con Edge en Windows 10. Si falta, la aplicación
  abre igual y usa sus pantallas en Windows Forms (ver «Sin WebView2» abajo).

### 1. Clonar en una ruta corta

Cloná en una carpeta de ruta corta, por ejemplo `C:\EventAura`. **Con una ruta larga la
compilación falla:** los paquetes NuGet tienen archivos de nombre largo y Windows no admite rutas
de más de 260 caracteres. Pasó en la comprobación, con el clon dentro de la carpeta temporal del
usuario.

### 2. Crear la base y cargar la semilla

1. Creá una base vacía llamada `BddEventAura`.
2. **Con esa base seleccionada** (no `master`: `01-esquema.sql` no elige base), ejecutá en orden:
   1. `BaseDatos/01-esquema.sql`
   2. `BaseDatos/02-semilla.sql`

   Con `sqlcmd`, desde la raíz del repositorio (cambiá `.\SQLEXPRESS` por tu instancia):
   ```powershell
   sqlcmd -S .\SQLEXPRESS -E -b -Q "CREATE DATABASE BddEventAura"
   sqlcmd -S .\SQLEXPRESS -E -b -d BddEventAura -i BaseDatos\01-esquema.sql
   sqlcmd -S .\SQLEXPRESS -E -b -d BddEventAura -i BaseDatos\02-semilla.sql
   ```
   Si `sqlcmd` responde «Timed out waiting for pipe '\\.\pipe\SQLLocal\SQLEXPRESS'», es el
   `sqlcmd` nuevo (el de `C:\Program Files\SqlCmd`), que no se conecta por memoria compartida.
   Pasó en la comprobación. Usá la canalización con nombre en lugar de `.\SQLEXPRESS`:
   `-S 'np:\\.\pipe\MSSQL$SQLEXPRESS\sql\query'` (entre comillas simples en PowerShell), o
   ejecutá los scripts desde SQL Server Management Studio.
3. **No hace falta ningún script del 03 al 16.** `01-esquema.sql` ya trae el esquema completo; los
   del 04 al 16 son para actualizar una base vieja, y el 03 es una carga grande para medir
   reportes.

**Para la demostración** hay una base aparte con más datos (25 eventos futuros, 58 clientes, 964
entradas vendidas): su nombre tiene que terminar en `_Demostracion`, y después de `01` y `02` se
corre `BaseDatos/semilla-demostracion.sql`. El paso a paso y cómo apuntar la aplicación a ella y
volver está en `Pruebas/GUION-DEMOSTRACION.md`, «Armar la base de demostración».

### 3. Compilar

Abrí `SistemaEntradas.sln` en Visual Studio y elegí **Compilar → Compilar solución**: Visual
Studio restaura solo los paquetes NuGet. Desde una terminal PowerShell, en la raíz:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' SistemaEntradas.sln -restore -p:RestorePackagesConfig=true -p:Configuration=Debug -v:minimal -nologo
```

**`-p:RestorePackagesConfig=true` es necesario.** El proyecto `Visual` declara sus paquetes en
`packages.config`, y `-restore` solo no los descarga: en un clon limpio la compilación falla con
«Este proyecto hace referencia a los paquetes NuGet que faltan en este equipo»
(`Microsoft.Data.SqlClient.SNI.targets`). Con otra edición de Visual Studio, cambiá `Community` en
la ruta.

### 4. Apuntar la aplicación a tu servidor

La cadena de conexión del repositorio apunta a la instancia de Máximo
(`DESKTOP-5QSNMRE\AMBROMSSQLSERVER`). **Si tu instancia es otra**, después de compilar abrí
`Visual\bin\Debug\Visual.exe.config` con el Bloc de notas y, en `connectionString`, cambiá
`Data Source=` por tu instancia (por ejemplo `.\SQLEXPRESS`). No edites `Visual/App.config`:
guarda la configuración de cada máquina y no se publica (ver `CLAUDE.md`).

### 5. Abrir

Abrí **`Visual\bin\Debug\Visual.exe`** con doble clic. Aparece «Bienvenido a EventAura» y, desde
ahí, el acceso. Cuentas de la semilla:

| Perfil | Mail | Contraseña |
|---|---|---|
| Administrador | `admin@eventaura.com` | `Admin1234` |
| Organizador | `organizador@eventaura.com` | `Cliente123` |
| Clientes | `plata@`, `oro@` y `bronce@eventaura.com` | `Cliente123` |

**La demostración se corre desde el ejecutable compilado, nunca desde Visual Studio** (ni con F5 ni
con «Iniciar sin depurar»). Una vez, en medio de una prueba, la aplicación lanzada desde Visual
Studio se cerró sin dejar ningún registro, y fuera del depurador no se pudo reproducir (ticket
T-38, `docs/TICKETS.md`). La aplicación igual registra cada cierre en
`%LOCALAPPDATA%\EventAura\errores.log`.

Si al abrir dice que la base está desactualizada, el aviso nombra el script que falta: ejecutalo en
esa base y volvé a abrir.

### Sin WebView2

Si el equipo no tiene el runtime de WebView2, la aplicación abre igual. Va directo al acceso en
Windows Forms, que avisa «La vista web no está disponible. Podés iniciar sesión con este
formulario.», y cada pantalla web usa su versión de respaldo. El catálogo, por ejemplo, muestra
tarjetas y una tabla. Comprobado el 27/11 haciendo que la aplicación no encuentre el runtime
(`docs/VERIFICACION-MUESTRA-1-2026-11-27.md`). El runtime se instala gratis desde Microsoft: buscá
«WebView2 Runtime Evergreen».

## Cómo correr las pruebas

El proyecto `Pruebas/` (MSTest, .NET Framework 4.8) tiene 234 casos y 379 ejecuciones. El
detalle de cada caso está en [docs/04-casos-prueba.md](docs/04-casos-prueba.md).

Son **pruebas de integración contra SQL Server**, no unitarias: usan los mismos gestores y la
misma base que la aplicación. Por eso necesitan un servidor disponible.

### Requisitos

- **Visual Studio 2022** con la carga de trabajo «Desarrollo de escritorio de .NET», que
  instala MSBuild y las herramientas de prueba (`vstest.console.exe`).
- **SQL Server** accesible con autenticación de Windows y permiso para crear y borrar bases.

### 1. Apuntar a tu servidor

Abrí [Pruebas/App.config](Pruebas/App.config) y cambiá el `Data Source` por el nombre de tu
instancia:

```xml
<add name="Visual.Properties.Settings.BddEventAura"
     connectionString="Data Source=TU-PC\TU-INSTANCIA;Initial Catalog=BddEventAura_Pruebas;Integrated Security=True;Encrypt=True;TrustServerCertificate=True" ... />
```

**No cambies el `Initial Catalog`.** La base de pruebas se borra y se crea de nuevo en cada
corrida, así que el nombre tiene que terminar en `_Pruebas`. Si no termina así, las pruebas
se niegan a arrancar y no tocan ninguna base. Así la base de desarrollo (`BddEventAura`)
queda a salvo.

No hace falta crear la base a mano: la primera corrida la crea.

### 2. Correrlas

**Desde VS Code:** `Ctrl+Shift+P` → «Tasks: Run Task» → **Probar**. La tarea compila la
solución con el MSBuild de Visual Studio y después corre las pruebas.

**Desde una terminal PowerShell**, en la raíz del repositorio:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' SistemaEntradas.sln -restore -p:RestorePackagesConfig=true -p:Configuration=Debug -v:minimal -nologo

$vstest = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products '*' -requires Microsoft.VisualStudio.PackageGroup.TestTools.Core -find '**\TestPlatform\vstest.console.exe' | Select-Object -First 1
& $vstest .\Pruebas\bin\Debug\net48\Pruebas.dll /Logger:"console;verbosity=normal"
```

Si tenés otra edición de Visual Studio (Professional, Enterprise), cambiá `Community` en la
ruta de MSBuild.

Para correr una sola prueba, agregá un filtro al final del comando de `vstest`:

```powershell
& $vstest .\Pruebas\bin\Debug\net48\Pruebas.dll /Tests:CP07
```

> **`dotnet test` no sirve.** No puede compilar los `.resx` del proyecto `Visual`, que es
> WinForms sobre .NET Framework, y `Pruebas` depende de él. Usá MSBuild y `vstest`.

### 3. Qué esperar

Una corrida completa informa **379 pruebas superadas** y tarda unos 4 minutos. Muchas abren las
pantallas reales: **no uses el teclado ni el mouse mientras corren**, y dejá la sesión de
Windows abierta y sin bloquear. Las capturas que guardan van a `TestResults/`.

### Qué hace cada corrida con la base

1. Borra `BddEventAura_Pruebas` si existe y la crea de nuevo.
2. Le aplica [BaseDatos/01-esquema.sql](BaseDatos/01-esquema.sql) y
   [BaseDatos/02-semilla.sql](BaseDatos/02-semilla.sql).
3. Corre las pruebas de a una, nunca en paralelo, porque comparten la base. Cada prueba
   crea sus propios datos, marcados como de prueba (mails `test…@pruebas.eventaura.com`), y
   los borra al terminar. No modifica los datos de la semilla.

La base queda creada al final, para poder revisarla si algo falló. La próxima corrida la
vuelve a borrar.

### Si algo falla

| Síntoma | Causa probable |
|---|---|
| «Las pruebas se niegan a correr: el App.config de Pruebas apunta a …» | El `Initial Catalog` no termina en `_Pruebas` |
| Error de conexión o «servidor no encontrado» | El `Data Source` no coincide con tu instancia, o el servicio de SQL Server está detenido |
| Error de permisos al crear la base | Tu usuario de Windows no puede crear bases en ese servidor |
| No encuentra `MSBuild.exe` | Otra edición de Visual Studio: ajustá la ruta |
| `$vstest` queda vacío | Falta instalar las herramientas de prueba de Visual Studio |

### Pruebas manuales

Las pruebas manuales que faltan correr están en
[docs/PRUEBAS-PENDIENTES.md](docs/PRUEBAS-PENDIENTES.md), con qué probar y qué resultado
esperar.
