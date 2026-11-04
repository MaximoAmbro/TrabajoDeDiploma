# JC-R01: verifica la pantalla de compra a la escala que tenga Windows EN ESTE MOMENTO.
# Uso, después de cambiar la escala en Configuración > Sistema > Pantalla:
#   powershell -ExecutionPolicy Bypass -File tools\CapturasInstructivo\VerificarEscala.ps1 -Nombre 125
# Abre la compra con el paquete máximo (título de 100 caracteres, 2 entradas por sector, los
# 4 servicios y seguro) contra BddEventAura_Pruebas y comprueba que la ventana y los botones
# Comprar y Volver entren en el área útil. Deja capturas y resultado en docs/capturas/escalado/.
# No toca los App.config ni la base de desarrollo. Abre una ventana unos 5 segundos.
param([Parameter(Mandatory = $true)][string]$Nombre)
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraEscala'
$bin = Join-Path $tmp 'bin'
$salida = Join-Path $raiz 'docs\capturas\escalado'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin, $salida | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force

$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg.Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'Captura.exe.config'), $cfg)

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe `
    /out:"$bin\Captura.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll `
    /r:"$bin\Visual.exe" /r:"$bin\Entidades.dll" (Join-Path $PSScriptRoot 'Captura.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló Captura.cs.' }

$p = Start-Process (Join-Path $bin 'Captura.exe') -ArgumentList "`"$salida`" --escala $Nombre" `
    -WorkingDirectory $bin -Wait -PassThru
$log = Join-Path $salida "escala-$Nombre.txt"
Move-Item (Join-Path $salida 'captura-log.txt') $log -Force
Get-Content $log -Encoding UTF8
if ($p.ExitCode -ne 0) { throw "Hay elementos fuera del área útil a escala $Nombre." }
