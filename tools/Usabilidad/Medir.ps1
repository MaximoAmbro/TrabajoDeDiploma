# Mide la usabilidad del flujo de compra sobre la aplicación real y deja el resultado en
# docs/usabilidad/medicion.txt. Abre los formularios de Visual contra BddEventAura_Pruebas, con
# su propia configuración: no toca los App.config ni la base de desarrollo, y no manda teclas
# al escritorio. Antes: compilar y correr la batería (tools/Proyecto.ps1 -Accion Probar), así
# la base de pruebas queda con la semilla. Abre ventanas unos 30 segundos.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraUsabilidad'
$bin = Join-Path $tmp 'bin'
$salida = Join-Path $raiz 'docs\usabilidad'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin, $salida | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force

$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg.Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'Medicion.exe.config'), $cfg)

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe `
    /out:"$bin\Medicion.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll `
    /r:"$bin\Visual.exe" /r:"$bin\Entidades.dll" /r:"$bin\Repositorio.dll" /r:"$bin\Datos.dll" `
    /r:"$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\netstandard.dll" `
    (Join-Path $PSScriptRoot 'Medicion.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló Medicion.cs.' }

$p = Start-Process (Join-Path $bin 'Medicion.exe') -ArgumentList "`"$salida`"" -WorkingDirectory $bin -Wait -PassThru
Get-Content (Join-Path $salida 'medicion.txt') -Encoding UTF8
if ($p.ExitCode -ne 0) { throw 'La medición terminó con error.' }
