# CX-02: deja vencer una reserva por reloj (RESERVA_MINUTOS, hoy 10 minutos reales) y comprueba
# que libera el cupo sin cobrar. Corre ReservaVencida.cs contra BddEventAura_Pruebas, con su
# propia configuración: no toca los App.config ni la base de desarrollo. Antes: correr la
# batería (tools/Proyecto.ps1 -Accion Probar), así la base de pruebas está con la semilla.
# Deja el resultado en docs/verificaciones/reserva-vencida.txt y sus capturas.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraReservaVencida'
$bin = Join-Path $tmp 'bin'
$salida = Join-Path $raiz 'docs\verificaciones'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin, $salida | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force

$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg.Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'ReservaVencida.exe.config'), $cfg)

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe `
    /out:"$bin\ReservaVencida.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll `
    /r:"$bin\Visual.exe" /r:"$bin\Entidades.dll" /r:"$bin\Repositorio.dll" `
    /r:"$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\netstandard.dll" (Join-Path $PSScriptRoot 'ReservaVencida.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló ReservaVencida.cs.' }

$minutos = (sqlcmd -S 'np:\\.\pipe\MSSQL$SQLEXPRESS\sql\query' -E -d BddEventAura_Pruebas -h -1 -W -Q "SET NOCOUNT ON; SELECT Valor FROM Parametros WHERE Clave = 'RESERVA_MINUTOS'" | Where-Object { $_.Trim() } | Select-Object -First 1).Trim()
$p = Start-Process (Join-Path $bin 'ReservaVencida.exe') -ArgumentList "`"$salida`"" -WorkingDirectory $bin -Wait -PassThru
$archivo = Join-Path $salida 'reserva-vencida.txt'
$lineas = [Collections.Generic.List[string]](Get-Content $archivo -Encoding UTF8)
$marca = $lineas | Where-Object { $_ -like 'COMPRA=*' }
$compra = $marca -replace 'COMPRA=', ''
$lineas.Remove($marca) | Out-Null

$q = "SET NOCOUNT ON; SELECT 'Compra ' + CAST(ID AS varchar) + ': estado ' + Estado + '; reserva de ' + CAST(DATEDIFF(SECOND, FechaCompra, ReservaVence) / 60 AS varchar) + ' min' FROM Compras WHERE ID = $compra; " +
     "SELECT 'Pagos de la compra: ' + CAST(COUNT(*) AS varchar) FROM Pagos WHERE ID_Compra = $compra; " +
     "SELECT 'Entradas de la compra por estado: ' + ISNULL(STRING_AGG(Estado + ' ' + CAST(n AS varchar), ', '), 'ninguna') FROM (SELECT Estado, COUNT(*) n FROM Entradas WHERE ID_Compra = $compra GROUP BY Estado) x;"
$base = sqlcmd -S 'np:\\.\pipe\MSSQL$SQLEXPRESS\sql\query' -E -d BddEventAura_Pruebas -h -1 -W -Q $q
$lineas.Insert(0, "CX-02 — reserva vencida por reloj — " + (Get-Date -Format 'dd/MM/yyyy') + " — BddEventAura_Pruebas, RESERVA_MINUTOS = $minutos")
$lineas.Add('8. Estado en la base después:')
foreach ($b in $base) { if ($b.Trim()) { $lineas.Add('   ' + $b.Trim()) } }
[IO.File]::WriteAllLines($archivo, $lineas, (New-Object Text.UTF8Encoding($false)))
$lineas
if ($p.ExitCode -ne 0) { throw 'La verificación terminó con error.' }
