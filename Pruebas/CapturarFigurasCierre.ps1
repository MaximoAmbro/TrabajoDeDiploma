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
  $salida=Join-Path $raiz "Pruebas/Figuras-Instructivo-2026-11-16/$Escala"
  [void][IO.Directory]::CreateDirectory($salida)
  $codigo=@"
using System;using System.IO;using System.Reflection;
class CapturarFigurasCierre {
 [STAThread] static int Main(string[] args) {
  try {var a=Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Pruebas.dll"));a.GetType("Pruebas.CapturasInstructivoCongelado").GetMethod("Generar").Invoke(null,new object[]{args[0]});return 0;}
  catch(Exception e){Console.WriteLine((e.InnerException??e).ToString());return 1;}
 }
}
"@
  $fuente=Join-Path $raiz 'TestResults/Cierre16/CapturarFigurasCierre.cs';[IO.File]::WriteAllText($fuente,$codigo)
  $exe=Join-Path $bin 'CapturarFigurasCierre.exe'
  & (Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe') /nologo "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
  if($LASTEXITCODE -ne 0){throw 'No compiló el host'}
  Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe+'.config') -Force
  & $exe $salida *> "TestResults/Cierre16/figuras-$Escala.log"
  if($LASTEXITCODE -ne 0){Get-Content "TestResults/Cierre16/figuras-$Escala.log" -Tail 16;throw 'Falló captura del ejemplo'}
  $ticket=Get-ChildItem -LiteralPath $salida -Filter '*.pdf' | Select-Object -First 1
  $dependencias=Join-Path $raiz 'TestResults/Cierre16/pythondeps'
  python -c 'import sys;sys.path.insert(0,sys.argv[1]);import pymupdf;pymupdf.open(sys.argv[2])[0].get_pixmap(matrix=pymupdf.Matrix(2,2),alpha=False).save(sys.argv[3])' $dependencias $ticket.FullName (Join-Path $salida 'ticket.png')
  if($LASTEXITCODE -ne 0){throw 'No se rasterizó el PDF; instalar pymupdf en TestResults/Cierre16/pythondeps'}
  Add-Type -AssemblyName System.Drawing
  $confirmacion=[Drawing.Image]::FromFile((Join-Path $salida 'figura4b-confirmacion.png'))
  $pdf=[Drawing.Image]::FromFile((Join-Path $salida 'ticket.png'))
  try {
   $anchoTicket=[int]($pdf.Width*$confirmacion.Height/$pdf.Height)
   $imagen=New-Object Drawing.Bitmap ($confirmacion.Width+24+$anchoTicket),$confirmacion.Height
   $g=[Drawing.Graphics]::FromImage($imagen)
   try {$g.Clear([Drawing.Color]::White);$g.InterpolationMode='HighQualityBicubic';$g.DrawImage($confirmacion,0,0,$confirmacion.Width,$confirmacion.Height);$g.DrawImage($pdf,$confirmacion.Width+24,0,$anchoTicket,$confirmacion.Height);$imagen.Save((Join-Path $salida 'figura4-confirmacion-y-comprobante.png'),[Drawing.Imaging.ImageFormat]::Png)}finally{$g.Dispose();$imagen.Dispose()}
  }finally{$confirmacion.Dispose();$pdf.Dispose()}
  Get-Content (Join-Path $salida 'ejemplo.txt') -Encoding UTF8 -TotalCount 3
 }finally{if((Leer-Escala).Actual -ne $original){Fijar-Escala $original}}
}finally{Pop-Location}
