param([ValidateSet(100,150)][int]$Escala=100)
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
 . ./tools/VerificacionesManuales/Escala.ps1
 $original=(Leer-Escala).Actual
 try {
  if($original -ne $Escala){Fijar-Escala $Escala}
  $bin=Join-Path $raiz 'Pruebas/bin/Debug/net48'
  $salida=Join-Path $raiz "Pruebas/Capturas-RechazoEvento/despues-$Escala"
  $hostDir=Join-Path $raiz 'TestResults/CapturarRevisionEventos';[void][IO.Directory]::CreateDirectory($hostDir)
  $fuente=Join-Path $hostDir 'CapturarRevisionEventos.cs'
  $codigo=@"
using System;using System.IO;using System.Reflection;
class CapturarRevisionEventos {
 [System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern uint SetErrorMode(uint modo);
 [STAThread] static int Main(string[] args) {
  SetErrorMode(3);System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.ThrowException,false);
  try {var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));a.GetType("Pruebas.CapturasTandaVisual").GetMethod("CapturarRevisionEventos").Invoke(null,new object[]{args[0]});return 0;}
  catch(Exception e){Console.WriteLine((e.InnerException??e).ToString());return 1;}
 }
}
"@
  [IO.File]::WriteAllText($fuente,$codigo)
  $exe=Join-Path $bin 'CapturarRevisionEventos.exe'
  & (Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe') /nologo /r:System.Windows.Forms.dll "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
  if($LASTEXITCODE -ne 0){throw 'No compiló el host de capturas.'}
  Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe+'.config') -Force
  & $exe $salida *> (Join-Path $hostDir "captura-$Escala.log")
  if($LASTEXITCODE -ne 0){Get-Content (Join-Path $hostDir "captura-$Escala.log") -Tail 20;throw 'Falló la captura de revisión de eventos.'}
  Get-Content (Join-Path $salida 'anchos.tsv')
 } finally {if((Leer-Escala).Actual -ne $original){Fijar-Escala $original}}
} finally {Pop-Location}
