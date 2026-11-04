# Muestra qué ve el usuario cuando un error no está capturado: corre la aplicación real con
# su bucle de mensajes contra una base INEXISTENTE (BddEventAura_NoExiste) e intenta iniciar
# sesión. No toca los App.config ni ninguna base. Deja el resultado en
# docs/excepciones/error-no-controlado.txt. Abre una ventana unos segundos.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraExcepciones'
$bin = Join-Path $tmp 'bin'
$salida = Join-Path $raiz 'docs\excepciones'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin, $salida | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force

$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg -replace 'Initial Catalog=[^;]+;', 'Initial Catalog=BddEventAura_NoExiste;'
if ($cfg -notmatch 'BddEventAura_NoExiste') { throw 'No se pudo apuntar la configuración a una base inexistente.' }
[IO.File]::WriteAllText((Join-Path $bin 'ErrorNoControlado.exe.config'), $cfg)

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe `
    /out:"$bin\ErrorNoControlado.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll `
    /r:"$bin\Visual.exe" (Join-Path $PSScriptRoot 'ErrorNoControlado.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló ErrorNoControlado.cs.' }

Start-Process (Join-Path $bin 'ErrorNoControlado.exe') -ArgumentList "`"$salida`"" -WorkingDirectory $bin -Wait
Get-Content (Join-Path $salida 'error-no-controlado.txt') -Encoding UTF8
