# CX-04: ¿qué ve el usuario si SQL Server está detenido? Detiene el servicio, abre el
# ejecutable real (Visual\bin\Debug\Visual.exe, el mismo que abre el usuario), intenta iniciar
# sesión con UI Automation y registra la ventana que aparece. Vuelve a levantar el servicio
# pase lo que pase. Necesita una consola de administrador. No toca los App.config ni ninguna
# base, y no manda teclas al escritorio: escribe con WM_SETTEXT y hace clic con BM_CLICK (los
# controles de la aplicación no exponen ValuePattern ni InvokePattern a UI Automation).
# Deja el resultado en docs/excepciones/base-caida.txt y base-caida.png.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$exe = Join-Path $raiz 'Visual\bin\Debug\Visual.exe'
$salida = Join-Path $raiz 'docs\excepciones'
$servicio = 'MSSQL$SQLEXPRESS'
New-Item -ItemType Directory -Force $salida | Out-Null

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Drawing
Add-Type -ReferencedAssemblies System.Drawing @'
using System; using System.Drawing; using System.Runtime.InteropServices;
public static class Win {
    [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr SendMessage(IntPtr h, uint m, IntPtr w, string l);
    [DllImport("dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr h, int a, out RECT r, int s);
    [StructLayout(LayoutKind.Sequential)] struct RECT { public int L, T, R, B; }
    delegate bool Proc(IntPtr h, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr h, Proc cb, IntPtr p);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, System.Text.StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, System.Text.StringBuilder s, int n);
    [DllImport("user32.dll")] static extern int GetDlgCtrlID(IntPtr h);
    // Pulsa un botón de un cuadro nativo como lo hace el cuadro mismo: WM_COMMAND con su ID.
    // BM_CLICK no alcanza cuando el cuadro no es la ventana activa.
    public static void Pulsar(IntPtr cuadro, IntPtr boton) { PostMessage(cuadro, 0x0111, (IntPtr)GetDlgCtrlID(boton), boton); }
    // Controles hijos de un cuadro de mensaje nativo: clase, texto y handle.
    public static string[][] Hijos(IntPtr h) {
        var lista = new System.Collections.Generic.List<string[]>();
        EnumChildWindows(h, (c, p) => {
            var k = new System.Text.StringBuilder(64); GetClassName(c, k, 64);
            var x = new System.Text.StringBuilder(1024); GetWindowText(c, x, 1024);
            lista.Add(new[] { k.ToString(), x.ToString(), c.ToInt64().ToString() });
            return true; }, IntPtr.Zero);
        return lista.ToArray();
    }
    public static Rectangle Marco(IntPtr h) { RECT r; DwmGetWindowAttribute(h, 9, out r, 16); return Rectangle.FromLTRB(r.L, r.T, r.R, r.B); }
    // Captura la zona y pinta de blanco lo que no es de ninguna de las dos ventanas (el escritorio).
    public static void Capturar(Rectangle a, Rectangle d, string archivo) {
        Rectangle zona = Rectangle.Union(a, d);
        using (var b = new Bitmap(zona.Width, zona.Height)) {
            using (var g = Graphics.FromImage(b)) {
                g.CopyFromScreen(zona.Location, Point.Empty, zona.Size);
                var fondo = new Region(new Rectangle(Point.Empty, zona.Size));
                a.Offset(-zona.X, -zona.Y); d.Offset(-zona.X, -zona.Y);
                fondo.Exclude(a); fondo.Exclude(d);
                g.FillRegion(Brushes.White, fondo);
            }
            b.Save(archivo, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
'@
$AE = [Windows.Automation.AutomationElement]
$Todo = [Windows.Automation.TreeScope]::Descendants
$log = New-Object Collections.Generic.List[string]
function Anotar($texto) { $log.Add($texto); Write-Output $texto }
function PorNombre($padre, $nombre) {
    $padre.FindFirst($Todo, (New-Object Windows.Automation.PropertyCondition($AE::NameProperty, $nombre)))
}
function Escribir($elemento, $texto) {
    [Win]::SendMessage([IntPtr]$elemento.Current.NativeWindowHandle, 0x000C, [IntPtr]::Zero, $texto) | Out-Null
}
function VentanasDe($procId) {
    $c = New-Object Windows.Automation.PropertyCondition($AE::ProcessIdProperty, $procId)
    @($AE::RootElement.FindAll([Windows.Automation.TreeScope]::Children, $c))
}
function Clic($elemento) {
    [Win]::PostMessage([IntPtr]$elemento.Current.NativeWindowHandle, 0xF5, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
}

$detuve = $false
$app = $null
try {
    Anotar ("CX-04 — " + (Get-Date -Format 'dd/MM/yyyy HH:mm') + " — ejecutable: Visual\bin\Debug\Visual.exe")
    Stop-Service $servicio -Force
    $detuve = $true
    (Get-Service $servicio).WaitForStatus('Stopped', [TimeSpan]::FromSeconds(60))
    Anotar ("1. Servicio ${servicio}: " + (Get-Service $servicio).Status)

    $app = Start-Process $exe -WorkingDirectory (Split-Path $exe) -PassThru
    $inicio = $null
    for ($i = 0; $i -lt 60 -and -not $inicio; $i++) {
        Start-Sleep -Milliseconds 500
        $inicio = VentanasDe $app.Id | Where-Object { PorNombre $_ 'Iniciar sesión' } | Select-Object -First 1
    }
    if (-not $inicio) { throw 'No apareció la pantalla de inicio de sesión.' }
    Anotar ("2. La aplicación abre sin la base: ventana «" + $inicio.Current.Name + "».")

    # Los dos campos de texto, de arriba abajo: mail y contraseña.
    $campos = @($inicio.FindAll($Todo, [Windows.Automation.Condition]::TrueCondition) |
        Where-Object { $_.Current.ClassName -like '*EDIT*' } | Sort-Object { $_.Current.BoundingRectangle.Y })
    if ($campos.Count -ne 2) { throw ('Se esperaban 2 campos de texto y hay ' + $campos.Count + '.') }
    Escribir $campos[0] 'plata@eventaura.com'
    Escribir $campos[1] 'Cliente123'
    $reloj = [Diagnostics.Stopwatch]::StartNew()
    Clic (PorNombre $inicio 'Iniciar sesión')
    Anotar '3. Clic en «Iniciar sesión» con plata@eventaura.com.'

    $dialogo = $null
    while (-not $dialogo -and $reloj.Elapsed.TotalSeconds -lt 60) {
        Start-Sleep -Milliseconds 300
        $dialogo = VentanasDe $app.Id | Where-Object {
            $_.Current.ClassName -eq '#32770' -or $_.Current.Name -eq 'Microsoft .NET Framework' -or
            @($_.FindAll([Windows.Automation.TreeScope]::Children, [Windows.Automation.Condition]::TrueCondition) |
              Where-Object { $_.Current.ClassName -eq '#32770' }).Count -gt 0 } | Select-Object -First 1
        if ($dialogo -and $dialogo.Current.ClassName -ne '#32770' -and $dialogo.Current.Name -ne 'Microsoft .NET Framework') {
            $dialogo = $dialogo.FindFirst([Windows.Automation.TreeScope]::Children,
                (New-Object Windows.Automation.PropertyCondition($AE::ClassNameProperty, '#32770')))
        }
    }
    if (-not $dialogo) { throw 'No apareció ninguna ventana en 60 segundos.' }
    Anotar ("4. A los {0:N1} s aparece una ventana: título «{1}», clase {2}." -f $reloj.Elapsed.TotalSeconds, $dialogo.Current.Name, $dialogo.Current.ClassName)
    $hijos = [Win]::Hijos([IntPtr]$dialogo.Current.NativeWindowHandle)
    foreach ($h in $hijos) { if ($h[0] -eq 'Static' -and $h[1].Trim()) { Anotar ("   Texto: «" + $h[1] + "»") } }
    $botones = @($hijos | Where-Object { $_[0] -eq 'Button' })
    Anotar ("   Botones: " + (($botones | ForEach-Object { '«' + $_[1].Replace('&', '') + '»' }) -join ', '))
    $esNet = $dialogo.Current.Name -eq 'Microsoft .NET Framework' -or ($botones | Where-Object { $_[1] -match 'Detalles|Details|Continuar|Continue' })
    Anotar ("5. Cuadro de excepción de .NET: " + $(if ($esNet) { 'SÍ (falla)' } else { 'no' }))
    $hInicio = [IntPtr]$inicio.Current.NativeWindowHandle
    $hDialogo = [IntPtr]$dialogo.Current.NativeWindowHandle
    [Win]::SetForegroundWindow($hInicio) | Out-Null
    [Win]::SetForegroundWindow($hDialogo) | Out-Null
    try { $dialogo.SetFocus() } catch { }
    Start-Sleep -Milliseconds 700
    $zona = [Drawing.Rectangle]::Union([Win]::Marco($hInicio), [Win]::Marco($hDialogo))
    [Win]::Capturar([Win]::Marco($hInicio), [Win]::Marco($hDialogo), (Join-Path $salida 'base-caida.png'))
    Anotar ("   Captura: docs/excepciones/base-caida.png ({0} x {1})." -f $zona.Width, $zona.Height)

    [Win]::Pulsar($hDialogo, [IntPtr][long]$botones[0][2])
    Start-Sleep -Seconds 2
    # ¿Quedó otra ventana abierta? Un segundo cuadro bloquearía el cierre.
    foreach ($v in VentanasDe $app.Id) {
        $extra = @($v.FindAll([Windows.Automation.TreeScope]::Children,
            (New-Object Windows.Automation.PropertyCondition($AE::ClassNameProperty, '#32770'))))
        Anotar ("   Ventana abierta: «" + $v.Current.Name + "» (" + $v.Current.ClassName + "), cuadros encima: " + $extra.Count +
            $(if ($extra | Where-Object { $_.Current.NativeWindowHandle -eq [int64]$hDialogo }) { ' (el mismo de antes)' } elseif ($extra.Count) { ' (otro cuadro)' } else { '' }))
        foreach ($x in @($extra) + @($v | Where-Object { $_.Current.ClassName -eq '#32770' })) {
            foreach ($h in [Win]::Hijos([IntPtr]$x.Current.NativeWindowHandle)) {
                if ($h[0] -eq 'Static' -and $h[1].Trim()) { Anotar ("     «" + $x.Current.Name + "»: «" + $h[1] + "»") }
            }
        }
    }
    $sigue = -not $app.HasExited -and (VentanasDe $app.Id | Where-Object { PorNombre $_ 'Iniciar sesión' })
    Anotar ("6. Después de «" + $botones[0][1].Replace('&', '') + "»: la aplicación " +
        $(if ($sigue) { 'sigue abierta en el inicio de sesión.' } else { 'se cerró.' }))

    if ($sigue) {
        $inicio.GetCurrentPattern([Windows.Automation.WindowPattern]::Pattern).Close()
        $termino = $app.WaitForExit(10000)
        Anotar ("7. Cerrar con la X: el proceso " + $(if ($termino) { 'termina.' } else { 'NO termina (se cierra a la fuerza).' }))
    }
}
catch {
    Anotar ("ERROR: " + $_.Exception.Message)
}
finally {
    if ($app -and -not $app.HasExited) { $app.Kill() }
    if ($detuve) {
        Start-Service $servicio
        (Get-Service $servicio).WaitForStatus('Running', [TimeSpan]::FromSeconds(60))
        $prueba = sqlcmd -S 'np:\\.\pipe\MSSQL$SQLEXPRESS\sql\query' -E -h -1 -W -Q 'SET NOCOUNT ON; SELECT 1'
        Anotar ("8. Servicio ${servicio} otra vez: " + (Get-Service $servicio).Status + "; consulta de prueba: " + ($prueba -join '').Trim())
    }
    [IO.File]::WriteAllLines((Join-Path $salida 'base-caida.txt'), $log, (New-Object Text.UTF8Encoding($false)))
}
