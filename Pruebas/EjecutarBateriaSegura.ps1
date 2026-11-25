param([string]$Filtro = '', [int]$TiempoMaximoSegundos = 180)
$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
function Cerrar-AplicacionesDePrueba {
    foreach($proceso in @(Get-Process Visual,CapturarTanda,EnsayoDemo -ErrorAction SilentlyContinue)) {
        if($proceso.Path -and $proceso.Path.StartsWith($raiz+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)) {
            Write-Output "Cerrando proceso anterior del repositorio: $($proceso.ProcessName) ($($proceso.Id))"
            Stop-Process -Id $proceso.Id -Force -ErrorAction SilentlyContinue
        }
    }
}
Push-Location $raiz
$resultado = 1
try {
    # Siempre, no solo cuando ya hubo un bloqueo.
    Cerrar-AplicacionesDePrueba
    powershell -NoProfile -ExecutionPolicy Bypass -File tools/Proyecto.ps1 -Accion Compilar
    if($LASTEXITCODE -ne 0){throw 'No compiló la solución'}
    $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    $vs=& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
    $vstest=Join-Path $vs 'Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe'
    $argumentos=@('Pruebas/bin/Debug/net48/Pruebas.dll','/Logger:console;verbosity=normal')
    if($Filtro){$argumentos+=('/TestCaseFilter:'+$Filtro)}
    $argumentos+=@('--',('RunConfiguration.TestSessionTimeout='+($TiempoMaximoSegundos*1000)))
    $ErrorActionPreference='Continue'
    & $vstest @argumentos
    $resultado=$LASTEXITCODE
} finally {
    Cerrar-AplicacionesDePrueba
    Pop-Location
}
exit $resultado
