param([int]$Escala=100)
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
 . ./tools/VerificacionesManuales/Escala.ps1
 $original=(Leer-Escala).Actual
 try {
  Fijar-Escala $Escala
  $codigo=@'
using System;using System.IO;using System.Reflection;using System.Runtime.InteropServices;
class VerificarVistasDpi {
 [DllImport("user32.dll")] static extern uint GetDpiForSystem();
 [DllImport("kernel32.dll")] static extern uint SetErrorMode(uint modo);
 [STAThread] static int Main(string[] args){
  SetErrorMode(3);System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.ThrowException,false);
  if(GetDpiForSystem()!=uint.Parse(args[0])*96/100){Console.WriteLine("DPI incorrecto");return 2;}
  var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));
  var visual=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Visual.exe"));
  var tipo=a.GetType("Pruebas.PruebasVistasVisibles");int fallos=0;
  foreach(string nombre in new[]{"TarjetaEvento","FichaEvento","InicioCliente","EstadoVacio"})try{
   tipo.GetMethod("CP163_Botones").Invoke(Activator.CreateInstance(tipo),new object[]{visual.GetType("Visual.Componentes."+nombre)});
   Console.WriteLine("VERDE "+nombre+"; DPI="+GetDpiForSystem());
  }catch(Exception e){fallos++;Console.WriteLine("ROJO "+nombre+": "+(e.InnerException??e));}
  return fallos==0?0:1;
 }
}
'@
  $fuente=Join-Path $raiz 'TestResults/Visible/VerificarVistasDpi.cs';[IO.File]::WriteAllText($fuente,$codigo)
  $exe=Join-Path $raiz 'Pruebas/bin/Debug/net48/VerificarVistasDpi.exe'
  & "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo /r:System.Windows.Forms.dll "/win32manifest:$raiz/Visual/app.manifest" "/out:$exe" $fuente
  if($LASTEXITCODE -ne 0){throw 'No compiló el host DPI'}
  Copy-Item -LiteralPath Pruebas/bin/Debug/net48/Pruebas.dll.config -Destination ($exe+'.config') -Force
  & $exe $Escala
  if($LASTEXITCODE -ne 0){throw 'Barrera de las nuevas vistas en rojo'}
 }finally{Fijar-Escala $original}
}finally{Pop-Location}
