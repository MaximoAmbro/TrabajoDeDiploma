# CX-03, segunda parte: carga la base y los 19 formularios en el host de diseño de .NET
# (Disenador.cs). Copia Visual\bin\Debug a una carpeta temporal apuntada a BddEventAura_Pruebas
# (dos constructores consultan Negocio). No toca los App.config ni la base de desarrollo.
# Deja el resultado en docs/verificaciones/disenador-host.txt.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraDisenador'
$bin = Join-Path $tmp 'bin'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force
$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config')).Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'Disenador.exe.config'), $cfg)

$fw = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319"
& "$fw\csc.exe" /nologo /target:winexe /out:"$bin\Disenador.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll `
    /r:System.Design.dll /r:System.Core.dll /r:"$bin\Visual.exe" /r:"$fw\netstandard.dll" (Join-Path $PSScriptRoot 'Disenador.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló Disenador.cs.' }

$salida = Join-Path $raiz 'docs\verificaciones\disenador-host.txt'
$p = Start-Process (Join-Path $bin 'Disenador.exe') -ArgumentList "`"$salida`"" -WorkingDirectory $bin -Wait -PassThru
Get-Content $salida -Encoding UTF8
if ($p.ExitCode -ne 0) { throw 'Hubo fallas en el host de diseño.' }
