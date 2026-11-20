param([string]$Salida = "Pruebas/Capturas-UI-Normalizada-2026-11-20")
$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
$salidaReal = [IO.Path]::GetFullPath((Join-Path $raiz $Salida))
$bin = Join-Path $raiz 'Pruebas/bin/Debug/net48'
$trabajo = Join-Path $raiz 'TestResults/CapturarInterfazNormalizada'
$evidencia = Join-Path $trabajo 'evidencia'
[void][IO.Directory]::CreateDirectory($trabajo)
[void][IO.Directory]::CreateDirectory($evidencia)
[void][IO.Directory]::CreateDirectory($salidaReal)

$codigo = @"
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
class CapturarInterfazNormalizada {
 [System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern uint SetErrorMode(uint modo);
 [STAThread] static int Main(string[] args) {
  SetErrorMode(3);
  Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException, false);
  try {
   var a = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pruebas.dll"));
   string salida = args[0];
   a.GetType("Pruebas.CapturasMiCuenta2026").GetMethod("Generar").Invoke(null, new object[] { Path.Combine(salida, "mi-cuenta") });
   a.GetType("Pruebas.CapturasInstructivoCongelado").GetMethod("Generar").Invoke(null, new object[] { Path.Combine(salida, "instructivo") });
   a.GetType("Pruebas.CapturasUltimasUI").GetMethod("Guion").Invoke(null, new object[] { Path.Combine(salida, "guion") });
   return 0;
  } catch (Exception e) { Console.WriteLine((e.InnerException ?? e).ToString()); return 1; }
 }
}
"@

$fuente = Join-Path $trabajo 'CapturarInterfazNormalizada.cs'
$exe = Join-Path $bin 'CapturarInterfazNormalizada.exe'
[IO.File]::WriteAllText($fuente, $codigo)
$csc = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
& $csc /nologo /r:System.Windows.Forms.dll "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
if ($LASTEXITCODE -ne 0) { throw 'No compiló el host de capturas.' }
Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe + '.config') -Force
& $exe $evidencia
if ($LASTEXITCODE -ne 0) { throw 'Falló una de las capturas; revisá el error del host.' }

$seleccion = @(
    @{ Origen = 'guion/p47-inicio.png'; Destino = 'p47-inicio.png' },
    @{ Origen = 'guion/p48-registro.png'; Destino = 'p48-registro.png' },
    @{ Origen = 'mi-cuenta/mi-cuenta.png'; Destino = 'p49-mi-cuenta.png' },
    @{ Origen = 'mi-cuenta/mi-cuenta-menu-lateral.png'; Destino = 'p49-menu-lateral.png' },
    @{ Origen = 'guion/p47-inicio.png'; Destino = 'anexo-inicio-sesion.png' },
    @{ Origen = 'mi-cuenta/mi-cuenta.png'; Destino = 'anexo-mi-cuenta.png' },
    @{ Origen = 'mi-cuenta/mi-cuenta-menu-lateral.png'; Destino = 'anexo-mi-cuenta-menu-lateral.png' },
    @{ Origen = 'instructivo/anexo-pago.png'; Destino = 'anexo-pago.png' },
    @{ Origen = 'instructivo/figura4b-confirmacion.png'; Destino = 'anexo-confirmacion.png' }
)
foreach ($archivo in $seleccion) {
    Copy-Item -LiteralPath (Join-Path $evidencia $archivo.Origen) -Destination (Join-Path $salidaReal $archivo.Destino) -Force
}
Copy-Item -LiteralPath (Join-Path $evidencia 'guion/tipografia.tsv') -Destination (Join-Path $salidaReal 'tipografia.tsv') -Force
Copy-Item -LiteralPath (Join-Path $evidencia 'guion/p49-requiere-recorte.txt') -Destination (Join-Path $salidaReal 'p49-requiere-recorte.txt') -Force
