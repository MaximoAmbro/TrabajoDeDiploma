param([string]$Binarios='Pruebas/bin/Debug/net48',[Parameter(Mandatory=$true)][string]$Salida)
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
$bin=[IO.Path]::GetFullPath((Join-Path $raiz $Binarios));$salidaReal=[IO.Path]::GetFullPath((Join-Path $raiz $Salida))
[void][IO.Directory]::CreateDirectory($salidaReal)
$codigo=@"
using System;using System.IO;using System.Reflection;
class CapturarTanda {
 [System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern uint SetErrorMode(uint modo);
 [STAThread] static int Main(string[] args) {
  SetErrorMode(3);
  System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.ThrowException,false);
  try {var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));a.GetType("Pruebas.CapturasTandaVisual").GetMethod("Generar").Invoke(null,new object[]{args[0]});return 0;}
  catch(Exception e){Console.WriteLine((e.InnerException??e).ToString());return 1;}
 }
}
"@
$compilacion=Join-Path $raiz 'TestResults/CapturarTandaHost';[void][IO.Directory]::CreateDirectory($compilacion)
$ruta=Join-Path $compilacion 'CapturarTanda.cs';[IO.File]::WriteAllText($ruta,$codigo)
$exe=Join-Path $bin 'CapturarTanda.exe'
$csc=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $csc /nologo /r:System.Windows.Forms.dll "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $ruta
if($LASTEXITCODE -ne 0){throw 'No compiló el host de capturas.'}
Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe+'.config') -Force
& $exe $salidaReal
if($LASTEXITCODE -ne 0){throw 'Falló el recorrido para capturas.'}
