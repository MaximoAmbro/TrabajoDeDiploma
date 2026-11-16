param([ValidateSet(100,150)][int]$Escala=100,[ValidateSet('antes','despues')][string]$Fase='antes')
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
 . ./tools/VerificacionesManuales/Escala.ps1
 $original=(Leer-Escala).Actual
 try {
  if($original -ne $Escala){Fijar-Escala $Escala}
  powershell -NoProfile -ExecutionPolicy Bypass -File Pruebas/CapturarTanda.ps1 -Salida "TestResults/Cierre16/$Fase-$Escala"
  if($LASTEXITCODE -ne 0){throw 'Falló captura completa'}
  $destino=Join-Path $raiz "Pruebas/Capturas-CIERRE-2026-11-16/$Fase-$Escala"
  [void][IO.Directory]::CreateDirectory($destino)
  foreach($nombre in @('frmHistorialCliente.png','frmReportes.png')) {
   Copy-Item -LiteralPath (Join-Path $raiz "TestResults/Cierre16/$Fase-$Escala/$nombre") -Destination (Join-Path $destino $nombre) -Force
  }
 }finally{if((Leer-Escala).Actual -ne $original){Fijar-Escala $original}}
}finally{Pop-Location}
