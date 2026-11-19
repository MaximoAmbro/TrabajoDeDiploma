param([string]$Salida = 'Pruebas/Recortes-Documento-Fases7-8/completas/Fase8')
$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
foreach ($proceso in @(Get-Process Visual,CapturarAnexoSemilla -ErrorAction SilentlyContinue)) {
    if ($proceso.Path -and $proceso.Path.StartsWith($raiz + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        Stop-Process -Id $proceso.Id -Force -ErrorAction SilentlyContinue
    }
}
$bin = Join-Path $raiz 'Pruebas/bin/Debug/net48'
$salidaReal = [IO.Path]::GetFullPath((Join-Path $raiz $Salida))
[void][IO.Directory]::CreateDirectory($salidaReal)
$codigo = @'
using System;
using System.IO;
using System.Reflection;
class CapturarAnexoSemilla {
    [System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern uint SetErrorMode(uint modo);
    [STAThread] static int Main(string[] args) {
        SetErrorMode(3);
        System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.ThrowException, false);
        try {
            var assembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pruebas.dll"));
            assembly.GetType("Pruebas.CapturasSemillaAnexo").GetMethod("Generar").Invoke(null, new object[] { args[0] });
            return 0;
        } catch (Exception ex) {
            Console.WriteLine((ex.InnerException ?? ex).ToString());
            return 1;
        }
    }
}
'@
$directorioHost = Join-Path $raiz 'TestResults/CapturarAnexoSemilla'
[void][IO.Directory]::CreateDirectory($directorioHost)
$fuente = Join-Path $directorioHost 'CapturarAnexoSemilla.cs'
[IO.File]::WriteAllText($fuente, $codigo, [Text.UTF8Encoding]::new($false))
$exe = Join-Path $bin 'CapturarAnexoSemilla.exe'
$csc = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
& $csc /nologo /r:System.Windows.Forms.dll "/win32manifest:$(Join-Path $raiz 'Visual/app.manifest')" "/out:$exe" $fuente
if ($LASTEXITCODE -ne 0) { throw 'No compiló el host de capturas.' }
Copy-Item -LiteralPath (Join-Path $bin 'Pruebas.dll.config') -Destination ($exe + '.config') -Force
& $exe $salidaReal
if ($LASTEXITCODE -ne 0) { throw 'No se pudieron generar las capturas del anexo.' }
