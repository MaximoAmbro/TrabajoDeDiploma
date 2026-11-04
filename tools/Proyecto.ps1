param([ValidateSet('Compilar', 'Probar')][string]$Accion = 'Compilar')

$ErrorActionPreference = 'Stop'
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (-not (Test-Path -LiteralPath $vswhere)) { throw 'No se encuentra vswhere. Instale Visual Studio o Build Tools con MSBuild.' }
    $instalacion = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
    if (-not $instalacion) { throw 'No se encuentra una instalación de MSBuild.' }
    $msbuild = Join-Path $instalacion 'MSBuild\Current\Bin\MSBuild.exe'
    & $msbuild SistemaEntradas.sln -restore -p:Configuration=Debug -v:minimal -nologo
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    if ($Accion -eq 'Probar') {
        $vstest = Join-Path $instalacion 'Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe'
        if (-not (Test-Path -LiteralPath $vstest)) {
            $vstest = & $vswhere -latest -products '*' -find '**\vstest.console.exe' | Select-Object -First 1
        }
        if (-not $vstest) { throw 'No se encuentra vstest.console.exe. Instale las herramientas de pruebas de Visual Studio.' }
        & $vstest .\Pruebas\bin\Debug\net48\Pruebas.dll '/Logger:console;verbosity=normal'
        exit $LASTEXITCODE
    }
} finally {
    Pop-Location
}
