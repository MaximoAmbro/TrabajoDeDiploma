# Evento eliminado mientras el cliente paga (T-27, CP-52 sobre los formularios reales).
# Corre EventoEliminadoEnPago.cs contra BddEventAura_Pruebas, con su propia configuración: no
# toca los App.config ni la base de desarrollo. ELIMINA «Duki en Rosario» de la base de
# pruebas: al terminar, correr la batería (tools/Proyecto.ps1 -Accion Probar), que la vuelve a
# crear con la semilla. Deja el resultado en docs/excepciones/evento-eliminado-en-pago.txt.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraEventoEliminado'
$bin = Join-Path $tmp 'bin'
$salida = Join-Path $raiz 'docs\excepciones'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin, $salida | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force

$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg.Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'EventoEliminadoEnPago.exe.config'), $cfg)

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe `
    /out:"$bin\EventoEliminadoEnPago.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll `
    /r:"$bin\Visual.exe" /r:"$bin\Entidades.dll" /r:"$bin\Repositorio.dll" /r:"$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\netstandard.dll" (Join-Path $PSScriptRoot 'EventoEliminadoEnPago.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló EventoEliminadoEnPago.cs.' }

$p = Start-Process (Join-Path $bin 'EventoEliminadoEnPago.exe') -ArgumentList "`"$salida`"" -WorkingDirectory $bin -Wait -PassThru
$archivo = Join-Path $salida 'evento-eliminado-en-pago.txt'
$lineas = [Collections.Generic.List[string]](Get-Content $archivo -Encoding UTF8)
$compra = ($lineas | Where-Object { $_ -like 'COMPRA=*' }) -replace 'COMPRA=', ''
$lineas.Remove(($lineas | Where-Object { $_ -like 'COMPRA=*' })) | Out-Null

# Qué quedó en la base: ni pagos ni entradas de la compra, y el evento ya no existe.
$q = "SET NOCOUNT ON; SELECT 'Compra ' + CAST(ID AS varchar) + ': estado ' + Estado + ', importe del pedido ' + CAST(TotalPagado AS varchar) + ' (se calcula al iniciar; el cobro es la tabla Pagos)' FROM Compras WHERE ID = $compra; " +
     "SELECT 'Pagos de la compra: ' + CAST(COUNT(*) AS varchar) FROM Pagos WHERE ID_Compra = $compra; " +
     "SELECT 'Entradas de la compra: ' + CAST(COUNT(*) AS varchar) FROM Entradas WHERE ID_Compra = $compra; " +
     "SELECT 'Evento 1 en la base: ' + CAST(COUNT(*) AS varchar) FROM Eventos WHERE ID = 1;"
$base = sqlcmd -S 'np:\\.\pipe\MSSQL$SQLEXPRESS\sql\query' -E -d BddEventAura_Pruebas -h -1 -W -Q $q
$lineas.Insert(0, "Evento eliminado durante el pago — " + (Get-Date -Format 'dd/MM/yyyy HH:mm') + " — BddEventAura_Pruebas")
$lineas.Add('8. Estado en la base después:')
foreach ($b in $base) { if ($b.Trim()) { $lineas.Add('   ' + $b.Trim()) } }
[IO.File]::WriteAllLines($archivo, $lineas, (New-Object Text.UTF8Encoding($false)))
$lineas
if ($p.ExitCode -ne 0) { throw 'La verificación terminó con error.' }
