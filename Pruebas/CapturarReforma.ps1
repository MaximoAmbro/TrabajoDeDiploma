param(
    [Parameter(Mandatory = $true)]
    [ValidateSet(100, 150)]
    [int]$Escala
)

$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
$binarios = Join-Path $PSScriptRoot 'bin/Debug/net48'
$hostCaptura = Join-Path $binarios 'CapturarReforma.exe'
$temporales = Join-Path $raiz 'TestResults/CapturarReforma'
[void][IO.Directory]::CreateDirectory($temporales)
$codigo = Join-Path $temporales 'CapturarReforma.cs'
$manifiesto = Join-Path $temporales 'CapturarReforma.manifest'

[IO.File]::WriteAllText($codigo, @"
using System;
using System.Reflection;
using System.Windows.Forms;

internal static class CapturarReforma
{
    [STAThread]
    private static int Main()
    {
        var pruebas = Assembly.LoadFrom("Pruebas.dll");
        var baseDePrueba = pruebas.GetType("Pruebas.BaseDePrueba");
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            baseDePrueba.GetMethod("Preparar").Invoke(null, new object[] { null });
            var tipo = pruebas.GetType("Pruebas.PruebasInteraccionPantallas");
            tipo.GetMethod("CP357_InteraccionRealDeLasPantallas").Invoke(Activator.CreateInstance(tipo), null);
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error.InnerException ?? error);
            return 1;
        }
        finally
        {
            baseDePrueba.GetMethod("Terminar").Invoke(null, null);
        }
    }
}
"@)

# El host de captura reconoce el DPI del monitor activo aunque haya cambiado
# después de iniciar sesión. Cada imagen se toma del escritorio de Windows.
$manifestOriginal = [IO.File]::ReadAllText((Join-Path $raiz 'Visual/app.manifest'))
$manifestCaptura = $manifestOriginal.Replace('>true</dpiAware>', '>true/pm</dpiAware>').
    Replace('>system</dpiAwareness>', '>PerMonitorV2, system</dpiAwareness>')
[IO.File]::WriteAllText($manifiesto, $manifestCaptura)
$csc = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
& $csc /nologo /r:System.Windows.Forms.dll "/win32manifest:$manifiesto" "/out:$hostCaptura" $codigo
if ($LASTEXITCODE -ne 0) { throw 'El host de captura no compiló.' }
Copy-Item -LiteralPath (Join-Path $binarios 'Pruebas.dll.config') -Destination ($hostCaptura + '.config') -Force

$escalaAnterior = $env:EVENTAURA_CAPTURAS_DPI
$salidaAnterior = $env:EVENTAURA_CAPTURAS_UI
Push-Location $binarios
try {
    $env:EVENTAURA_CAPTURAS_DPI = (96 * $Escala / 100).ToString()
    $env:EVENTAURA_CAPTURAS_UI = Join-Path $raiz "TestResults/Capturas-Reforma/$Escala"
    & $hostCaptura
    $resultado = $LASTEXITCODE
} finally {
    Pop-Location
    $env:EVENTAURA_CAPTURAS_DPI = $escalaAnterior
    $env:EVENTAURA_CAPTURAS_UI = $salidaAnterior
}
exit $resultado
