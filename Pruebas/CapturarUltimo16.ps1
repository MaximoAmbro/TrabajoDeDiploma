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
  $salida=Join-Path $raiz "Pruebas/Guion-Interfaz-ULTIMO-2026-11-16/$Escala"
  $u09=Join-Path $raiz "Pruebas/U09-ULTIMO-2026-11-16/$Escala"
  $codigo=@"
using System;using System.IO;using System.Reflection;
class CapturarUltimo16 {
 [STAThread] static int Main(string[] args) {
  try {var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));var t=a.GetType("Pruebas.CapturasUltimasUI");t.GetMethod("Guion").Invoke(null,new object[]{args[0]});t.GetMethod("RetornoCompra").Invoke(null,new object[]{args[1]});return 0;}
  catch(Exception e){Console.WriteLine((e.InnerException??e).ToString());return 1;}
 }
}
"@
  $fuente=Join-Path $raiz 'TestResults/Ultimo16/CapturarUltimo16.cs';[IO.File]::WriteAllText($fuente,$codigo)
  $exe=Join-Path $bin 'CapturarUltimo16.exe'
  & (Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe') /nologo "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
  if($LASTEXITCODE -ne 0){throw 'No compiló el host'}
  Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe+'.config') -Force
  & $exe $salida $u09 *> "TestResults/Ultimo16/capturas-$Escala.log"
  if($LASTEXITCODE -ne 0){Get-Content "TestResults/Ultimo16/capturas-$Escala.log" -Tail 15;throw 'Captura no entregable; ver medición o flujo'}
  Get-Content (Join-Path $salida 'tipografia.tsv')
 }finally{if((Leer-Escala).Actual -ne $original){Fijar-Escala $original}}
}finally{Pop-Location}
