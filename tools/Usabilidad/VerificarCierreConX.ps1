# Hallazgo U-01 del estudio de usabilidad, sobre el ejecutable real y con UI Automation:
# después de cerrar una pantalla con la X, ¿la aplicación queda viva y sin ventanas?
# Abre Visual.exe contra la base de DESARROLLO (inicia sesión como plata@eventaura.com; no
# modifica datos salvo el hash de la contraseña de la semilla en el primer ingreso), entra al
# listado de eventos, lo cierra con WindowPattern.Close (equivale a la X) y mide. Al final
# detiene el proceso. No envía teclas al escritorio: escribe y pulsa con mensajes dirigidos a
# las ventanas de la aplicación. Uso: powershell -ExecutionPolicy Bypass -File <este archivo>
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
Add-Type @'
using System; using System.Runtime.InteropServices;
public static class VentanasEventAura { public delegate bool CB(IntPtr h, IntPtr p);
[DllImport("user32.dll")] public static extern bool EnumWindows(CB cb, IntPtr p);
[DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
[DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
[DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr SendMessage(IntPtr h, uint m, IntPtr w, string l);
[DllImport("user32.dll")] public static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
public static int Visibles(int pid){ int n=0; EnumWindows((h,p)=>{ uint q; GetWindowThreadProcessId(h,out q); if(q==pid && IsWindowVisible(h)) n++; return true; }, IntPtr.Zero); return n; } }
'@
$A = [System.Windows.Automation.AutomationElement]; $T = [System.Windows.Automation.TreeScope]
function Ventana($proceso, $titulo) {
    for ($i = 0; $i -lt 40; $i++) {
        $c = New-Object System.Windows.Automation.AndCondition(
            (New-Object System.Windows.Automation.PropertyCondition($A::ProcessIdProperty, $proceso)),
            (New-Object System.Windows.Automation.PropertyCondition($A::NameProperty, $titulo)))
        $w = $A::RootElement.FindFirst($T::Children, $c); if ($w) { return $w }; Start-Sleep -Milliseconds 250
    }
    throw "No apareció «$titulo»"
}
function PorNombre($w, $n) { $w.FindFirst($T::Descendants, (New-Object System.Windows.Automation.PropertyCondition($A::NameProperty, $n))) }
function Handle($e) { [IntPtr]$e.Current.NativeWindowHandle }

$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$bin = Join-Path $raiz 'Visual\bin\Debug'
$p = Start-Process (Join-Path $bin 'Visual.exe') -WorkingDirectory $bin -PassThru
try {
    $login = Ventana $p.Id 'EventAura · Iniciar sesión'
    # UIA expone los campos como «Pane» sin nombre (hallazgo U-12): se ubican por posición.
    $edits = @($login.FindAll($T::Descendants, [System.Windows.Automation.Condition]::TrueCondition) |
        Where-Object { $_.Current.ClassName -like '*EDIT*' } | Sort-Object { $_.Current.BoundingRectangle.Top })
    [void][VentanasEventAura]::SendMessage((Handle $edits[0]), 0x000C, [IntPtr]::Zero, 'plata@eventaura.com')
    [void][VentanasEventAura]::SendMessage((Handle $edits[1]), 0x000C, [IntPtr]::Zero, 'Cliente123')
    [void][VentanasEventAura]::PostMessage((Handle (PorNombre $login 'Iniciar sesión')), 0x00F5, [IntPtr]::Zero, [IntPtr]::Zero)
    $menu = Ventana $p.Id 'EventAura · Mi cuenta'
    "1. Sesión iniciada; ventanas visibles del proceso: " + [VentanasEventAura]::Visibles($p.Id)
    [void][VentanasEventAura]::PostMessage((Handle (PorNombre $menu 'Eventos disponibles')), 0x00F5, [IntPtr]::Zero, [IntPtr]::Zero)
    $ev = Ventana $p.Id 'EventAura · Eventos disponibles'
    "2. Listado abierto; ventanas visibles: " + [VentanasEventAura]::Visibles($p.Id)
    $ev.GetCurrentPattern([System.Windows.Automation.WindowPattern]::Pattern).Close()
    Start-Sleep -Seconds 3; $p.Refresh()
    "3. Listado cerrado con la X: proceso vivo = " + (-not $p.HasExited) + "; ventanas visibles = " +
        [VentanasEventAura]::Visibles($p.Id) + "; memoria = " + [math]::Round($p.WorkingSet64 / 1MB) + " MB"
} finally {
    if (-not $p.HasExited) { Stop-Process -Id $p.Id -Force; "4. Proceso detenido por la prueba." }
}
