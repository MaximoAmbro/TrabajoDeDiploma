param([string]$Nombre, [string[]]$Quitar = @())
# Copia bin\Debug a verif\<Nombre>, le quita los archivos pedidos, apunta la configuración a
# BddEventAura_Pruebas y compila el verificador dentro de la copia.
$ErrorActionPreference = 'Stop'
$raiz = 'D:\proyectos\TrabajoDeDiploma'
$aqui = $PSScriptRoot
$bin = Join-Path $aqui $Nombre
if (Test-Path $bin) { Remove-Item $bin -Recurse -Force }
New-Item -ItemType Directory $bin | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force
foreach ($patron in $Quitar) {
    Get-ChildItem $bin -Recurse -File -Filter $patron | ForEach-Object { Remove-Item $_.FullName -Force; "quitado: " + $_.FullName.Substring($bin.Length + 1) }
}
$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg.Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'Pruebas.exe.config'), $cfg)
[IO.File]::WriteAllText((Join-Path $bin 'Visual.exe.config'), $cfg)
$csc = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
$fac = 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\Facades\netstandard.dll'
& $csc /nologo /langversion:latest /target:winexe /platform:anycpu /out:"$bin\Pruebas.exe" `
    /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll /r:System.Data.dll /r:System.Configuration.dll /r:"$fac" `
    /r:"$bin\Visual.exe" /r:"$bin\Repositorio.dll" /r:"$bin\Entidades.dll" /r:"$bin\Datos.dll" (Join-Path $aqui 'Verificar.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló el verificador.' }
"listo: $bin"
