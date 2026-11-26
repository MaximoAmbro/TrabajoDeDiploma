# Funciones de UI Automation para manejar el Visual.exe real sin tocar su código.
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms, System.Drawing
Add-Type @"
using System; using System.Runtime.InteropServices;
public static class Raton {
  [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
  [DllImport("user32.dll")] public static extern void mouse_event(uint f, uint x, uint y, uint d, UIntPtr e);
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
  public static void Clic(int x, int y) { SetCursorPos(x, y); mouse_event(2,0,0,0,UIntPtr.Zero); mouse_event(4,0,0,0,UIntPtr.Zero); }
}
"@
$script:A = [System.Windows.Automation.AutomationElement]
$script:C = [System.Windows.Automation.PropertyCondition]
$script:TS = [System.Windows.Automation.TreeScope]
function Ventanas($proc) {
    $c = New-Object $C ($A::ProcessIdProperty, $proc.Id)
    @($A::RootElement.FindAll($TS::Children, $c))
}
function Ventana($proc, $titulo, $seg = 30) {
    $lim = (Get-Date).AddSeconds($seg)
    while ((Get-Date) -lt $lim) {
        if ($proc.HasExited) { throw "El proceso terminó (código $($proc.ExitCode))" }
        $w = Ventanas $proc | Where-Object { $_.Current.Name -like $titulo } | Select-Object -First 1
        if ($w) { return $w }
        Start-Sleep -Milliseconds 400
    }
    throw "No apareció la ventana $titulo; hay: " + ((Ventanas $proc | ForEach-Object { $_.Current.Name }) -join ' / ')
}
function Buscar($raiz, $nombre, $seg = 15) {
    $lim = (Get-Date).AddSeconds($seg)
    while ((Get-Date) -lt $lim) {
        $e = @($raiz.FindAll($TS::Descendants, (New-Object $C ($A::NameProperty, $nombre)))) | Where-Object { -not $_.Current.IsOffscreen -and $_.Current.BoundingRectangle.Width -gt 0 }
        if ($e) { return $e }
        Start-Sleep -Milliseconds 400
    }
    throw "No apareció «$nombre»"
}
function Clic($e) {
    $r = $e.Current.BoundingRectangle
    [Raton]::Clic([int]($r.X + $r.Width / 2), [int]($r.Y + $r.Height / 2))
}
function Escribir($e, $texto) {
    Clic $e; Start-Sleep -Milliseconds 250
    [System.Windows.Forms.SendKeys]::SendWait('^a'); [System.Windows.Forms.SendKeys]::SendWait('{DEL}')
    [System.Windows.Forms.SendKeys]::SendWait(($texto -replace '([+^%~(){}\[\]])', '{$1}'))
}
function Campos($raiz) {
    @($raiz.FindAll($TS::Descendants, (New-Object $C ($A::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)))) | Where-Object { -not $_.Current.IsOffscreen -and $_.Current.BoundingRectangle.Width -gt 0 }
}
function Volcar($raiz, $max = 80) {
    @($raiz.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) | Where-Object { $_.Current.Name -and -not $_.Current.IsOffscreen } | Select-Object -First $max | ForEach-Object { "  [$($_.Current.ControlType.ProgrammaticName -replace 'ControlType.','')] $($_.Current.Name)" }
}
function Capturar($w, $ruta) {
    $r = $w.Current.BoundingRectangle
    $bmp = New-Object System.Drawing.Bitmap ([int]$r.Width), ([int]$r.Height)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen([int]$r.X, [int]$r.Y, 0, 0, $bmp.Size)
    $bmp.Save($ruta); $g.Dispose(); $bmp.Dispose()
}
