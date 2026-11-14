param([string]$Nombre='patron')
$ErrorActionPreference='Continue'
$raiz=Split-Path $PSScriptRoot -Parent
Push-Location $raiz
try {
 powershell -NoProfile -ExecutionPolicy Bypass -File tools/Proyecto.ps1 -Accion Compilar *> "TestResults/$Nombre-build.log"
 if($LASTEXITCODE -ne 0) {Get-Content "TestResults/$Nombre-build.log" -Tail 12;exit 1}
 $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
 $vs=& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
 & (Join-Path $vs 'Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe') Pruebas/bin/Debug/net48/Pruebas.dll '/TestCaseFilter:FullyQualifiedName~CP63|FullyQualifiedName~EstiloNoSerializado|FullyQualifiedName~CP120' '/Logger:console;verbosity=normal' *> "TestResults/$Nombre.log"
 $resultado=$LASTEXITCODE
 Get-Content "TestResults/$Nombre.log" -Tail 7
 exit $resultado
} finally {Pop-Location}
