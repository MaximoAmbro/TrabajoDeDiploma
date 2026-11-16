param([string]$Nombre='interfaz')
$ErrorActionPreference='Stop'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
    powershell -NoProfile -ExecutionPolicy Bypass -File Pruebas/EjecutarBateriaSegura.ps1 `
        -Filtro 'FullyQualifiedName~CP63|FullyQualifiedName~EstiloNoSerializado|FullyQualifiedName~CP120|FullyQualifiedName~CP116'
    if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}
    powershell -NoProfile -STA -ExecutionPolicy Bypass -File Pruebas/VerificarBotonesT47.ps1 -Salida "TestResults/$Nombre-colores"
    exit $LASTEXITCODE
} finally {Pop-Location}
