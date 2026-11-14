param([ValidateSet(100,150)][int]$Escala=100)
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
 # La base _Pruebas debe existir: tools/Proyecto.ps1 -Accion Probar la prepara.
 powershell -NoProfile -ExecutionPolicy Bypass -File tools/Proyecto.ps1 -Accion Compilar *> TestResults/botones-todas-build.log
 if($LASTEXITCODE -ne 0){throw 'No compiló la solución.'}
 . ./tools/VerificacionesManuales/Escala.ps1
 $original=(Leer-Escala).Actual
 try {
  if($original -ne $Escala){Fijar-Escala $Escala}
  $codigo=@"
using System;using System.IO;using System.Linq;using System.Reflection;using System.Runtime.InteropServices;
class BarridoTodas {
 [DllImport("user32.dll")] static extern uint GetDpiForSystem();
 [STAThread] static int Main(string[] args) {
  int dpi=(int)GetDpiForSystem();if(dpi!=int.Parse(args[0])*96/100) {Console.WriteLine("DPI inesperado: "+dpi);return 2;}
  var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));var t=a.GetType("Pruebas.PruebasTextoBotones");
  var visual=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Visual.exe"));int total=0,fallos=0;
  foreach(var f in visual.GetTypes().Where(x=>typeof(System.Windows.Forms.Form).IsAssignableFrom(x) && x.Name!="FormularioEventAura" && !x.IsAbstract)) {
   total++;try {t.GetMethod("CP116_TextoCompleto").Invoke(Activator.CreateInstance(t),new object[]{f.Name});Console.WriteLine("OK "+f.Name);}
   catch(Exception e){fallos++;Console.WriteLine("FALLA "+f.Name+": "+(e.InnerException??e).Message);}
  }
  Console.WriteLine("Pantallas="+total+"; fallos="+fallos+"; DPI="+dpi);return total==20 && fallos==0?0:1;
 }
}
"@
  [void][IO.Directory]::CreateDirectory((Join-Path $raiz 'TestResults/Botones'))
  $fuente=Join-Path $raiz 'TestResults/Botones/BarridoTodas.cs';[IO.File]::WriteAllText($fuente,$codigo)
  $exe=Join-Path $raiz 'Pruebas/bin/Debug/net48/BarridoTodas.exe'
  $csc=Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
  & $csc /nologo /r:System.Core.dll /r:System.Windows.Forms.dll "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
  if($LASTEXITCODE -ne 0){throw 'No compiló el host con DPI.'}
  Copy-Item -LiteralPath (Join-Path $raiz 'Pruebas/bin/Debug/net48/Pruebas.dll.config') -Destination ($exe+'.config') -Force
  & $exe $Escala *> "TestResults/Botones/nativo-$Escala.log"
  $resultado=$LASTEXITCODE
  Get-Content "TestResults/Botones/nativo-$Escala.log"
  if($resultado -ne 0){throw 'Hay botones recortados o una escala no verificada.'}
 } finally {if((Leer-Escala).Actual -ne $original){Fijar-Escala $original}}
} finally {Pop-Location}
