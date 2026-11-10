# Evidencia reproducible de 6A. Correr primero Proyecto.ps1 -Accion Probar.
# Muestra el piloto brevemente; restaura la escala original incluso si falla.
param([int[]]$Escalas = @(100, 125, 150))
$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
$bin = Join-Path $PSScriptRoot 'bin\Debug\net48'
$salida = Join-Path $raiz 'TestResults\6A'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$instalacion = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
$csc = Join-Path $instalacion 'MSBuild\Current\Bin\Roslyn\csc.exe'
$netstandard = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\Facades\netstandard.dll'
$referencias = @('Visual.exe', 'Entidades.dll', 'Repositorio.dll', 'Pruebas.dll', 'Microsoft.Data.SqlClient.dll') |
    ForEach-Object { '/r:' + (Join-Path $bin $_) }
& $csc /nologo /target:winexe "/out:$bin\VerificacionVisual6A.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll /r:System.Configuration.dll $referencias "/r:$netstandard" "/win32manifest:$raiz\Visual\app.manifest" "$PSScriptRoot\VerificacionVisual6A.cs"
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar la herramienta de evidencia.' }
Copy-Item -LiteralPath "$bin\Pruebas.dll.config" -Destination "$bin\VerificacionVisual6A.exe.config"

function Ejecutar-Evidencia([string]$carpeta, [string]$modo = '') {
    $argumentos = @('"' + $carpeta + '"')
    if ($modo) { $argumentos += $modo }
    $proceso = Start-Process -FilePath "$bin\VerificacionVisual6A.exe" -ArgumentList $argumentos -WindowStyle Hidden -PassThru
    if (-not $proceso.WaitForExit(30000)) {
        $proceso.Kill()
        throw 'La herramienta de evidencia no termino dentro del plazo.'
    }
    if ($proceso.ExitCode -ne 0) { throw "La herramienta de evidencia devolvio $($proceso.ExitCode)." }
}

. (Join-Path $raiz 'tools\VerificacionesManuales\Escala.ps1')
$escalaOriginal = (Leer-Escala).Actual
try {
    foreach ($porcentaje in $Escalas) {
        if ((Leer-Escala).Actual -ne $porcentaje) { Fijar-Escala $porcentaje }
        Ejecutar-Evidencia $salida
        Get-Content (Join-Path $salida "piloto-$porcentaje.txt")
    }
} finally {
    if ((Leer-Escala).Actual -ne $escalaOriginal) { Fijar-Escala $escalaOriginal }
    Write-Output "Escala restaurada: $((Leer-Escala).Actual) %"
}
Ejecutar-Evidencia (Join-Path $salida 'Tickets') 'tickets'
Write-Output "Tickets y capturas: $salida"
