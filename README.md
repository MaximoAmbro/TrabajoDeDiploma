# Pegale una leidita al readme antes que le meti un instructivo a como hacer las pruebas y eso

# EventAura

Plataforma de venta de entradas para espectáculos de Rosario, con armado personalizado de
paquetes (entrada + hospedaje + traslado + merchandising + experiencia VIP).
Trabajo de Campo I — Universidad Abierta Interamericana.

## Cómo correr las pruebas

El proyecto `Pruebas/` (MSTest, .NET Framework 4.8) verifica los casos CP-01 a CP-10, el
cálculo de referencia de $99.625 y el alto máximo de los formularios (RNF-03). El detalle de
cada caso está en [docs/04-casos-prueba.md](docs/04-casos-prueba.md).

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
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' SistemaEntradas.sln -restore -p:Configuration=Debug -v:minimal -nologo

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

Una corrida correcta informa **12 pruebas superadas**:

| Prueba | Qué verifica |
|---|---|
| CP-01 a CP-04 (4 resultados) | Nivel y descuento según la cantidad de compras: Bronce 0 %, Plata 5 %, Oro 7,5 % |
| CP-05 | El descuento se aplica a las entradas y no al hospedaje |
| CP-06 | Una compra de hace 13 meses queda fuera de la ventana de 12 |
| CP-07 | Se factura el precio congelado aunque cambie la tarifa |
| CP-08 | La séptima compra se cobra al 5 %; el nivel Oro rige desde la octava |
| CP-09 | Un reembolso baja el contador pero no el nivel |
| CP-10 | La suscripción no suma porcentaje de descuento |
| Referencia | El cálculo de ejemplo da exactamente $99.625 |
| RNF-03 | Ningún formulario pasa de 790 px de alto |

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

Las pruebas de interfaz no están automatizadas. Las que faltan correr están en
[docs/PRUEBAS-PENDIENTES.md](docs/PRUEBAS-PENDIENTES.md), con qué probar y qué resultado
esperar.
