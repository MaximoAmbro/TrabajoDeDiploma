param([ValidateSet(100,150)][int]$Escala=100,[switch]$SoloPropuestas)
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
 . ./tools/VerificacionesManuales/Escala.ps1
 $original=(Leer-Escala).Actual
 try {
  if($original -ne $Escala){Fijar-Escala $Escala}
  $bin=Join-Path $raiz 'Pruebas/bin/Debug/net48'
  $codigo=@"
using System;using System.IO;using System.Reflection;
class CapturarCola16 {
 [STAThread] static int Main(string[] args) {
  try {var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));
   if(args[2]!="solo") a.GetType("Pruebas.CapturasTandaVisual").GetMethod("Generar").Invoke(null,new object[]{args[0]});
   a.GetType("Pruebas.PropuestasGrillasCola16").GetMethod("Generar").Invoke(null,new object[]{args[1]});return 0;}
  catch(Exception e){Console.WriteLine((e.InnerException??e).ToString());return 1;}
 }
}
"@
  $fuente=Join-Path $raiz 'TestResults/Cola16/CapturarCola16.cs'
  [IO.File]::WriteAllText($fuente,$codigo)
  $exe=Join-Path $bin 'CapturarCola16.exe'
  & (Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe') /nologo "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
  if($LASTEXITCODE -ne 0){throw 'No compiló el host'}
  Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe+'.config') -Force
  & $exe (Join-Path $raiz "TestResults/Cola16/capturas-$Escala") (Join-Path $raiz "TestResults/Cola16/propuestas-$Escala") $(if($SoloPropuestas){"solo"}else{"todas"})
  if($LASTEXITCODE -ne 0){throw 'Falló captura completa'}
 }finally{if((Leer-Escala).Actual -ne $original){Fijar-Escala $original}}
}finally{Pop-Location}
