param([string]$Exe, [string]$Salida, [int]$Veces = 5, [int]$Fuera = 40, [string]$Otra = 'notepad')
# T-38, pista de JC: con el pago abierto, pasar a otra aplicación, esperar, captura con Win+Shift+S y volver.
$v = $PSScriptRoot; . "$v\Uia.ps1"
Add-Type @"
using System; using System.Runtime.InteropServices;
public static class Tecla {
  [DllImport("user32.dll")] public static extern void keybd_event(byte k, byte s, uint f, UIntPtr e);
  [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
  public static void Combo(params byte[] ks) { foreach (var k in ks) keybd_event(k,0,0,UIntPtr.Zero); System.Threading.Thread.Sleep(80); for (int i = ks.Length-1; i>=0; i--) keybd_event(ks[i],0,2,UIntPtr.Zero); }
  public static uint PidAdelante() { uint p; GetWindowThreadProcessId(GetForegroundWindow(), out p); return p; }
}
"@
New-Item -ItemType Directory -Force $Salida | Out-Null
$log = "$env:LOCALAPPDATA\EventAura\errores.log"; $largoAntes = (Get-Item $log).Length; $inicio = Get-Date
function Doc($w) { @($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Document)))) | Select-Object -First 1 }
function Invocar($e) { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { Clic $e } }
function PorNombre($raiz, $n) { @($raiz.FindAll($TS::Descendants, (New-Object $UPC ($UAE::NameProperty, $n)))) }
function Vigilar($p, $seg) { $t = Get-Date; while (((Get-Date) - $t).TotalSeconds -lt $seg) { $p.Refresh(); if ($p.HasExited) { return $false }; Start-Sleep -Milliseconds 500 }; return $true }
$dir = Split-Path $Exe
$p = Start-Process $Exe -WorkingDirectory $dir -PassThru; Set-Content "$v\pid.txt" $p.Id
"{0:HH:mm:ss} Visual.exe pid {1} ({2})" -f (Get-Date), $p.Id, $Exe
Start-Sleep 7; $w = (Ventanas $p)[0]; Clic (Buscar $w 'Iniciar sesión Ya tengo una cuenta.')[0]; Start-Sleep 4
$w = (Ventanas $p)[0]; $cs = Campos $w; Escribir $cs[0] 'bronce@eventaura.com'; Escribir $cs[1] 'Cliente123'; [System.Windows.Forms.SendKeys]::SendWait('{ENTER}'); Start-Sleep 7
$w = (Ventanas $p)[0]
$otraP = $null
for ($i = 1; $i -le $Veces; $i++) {
    if ($p.HasExited) { "TERMINÓ antes de la vuelta $i (código $($p.ExitCode))"; break }
    Clic (Buscar $w 'Eventos disponibles')[-1]; Start-Sleep 6
    Invocar ((PorNombre (Doc $w) 'Ver entradas')[0]); Start-Sleep 5
    Invocar (PorNombre $w 'Armar paquete' | Where-Object { $_.Current.ControlType -ne [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen } | Select-Object -Last 1); Start-Sleep 7
    Invocar ((PorNombre (Doc $w) 'Agregar una entrada')[0]); Start-Sleep 1
    Invocar ((PorNombre (Doc $w) 'Continuar al pago')[0]); Start-Sleep 3
    $sin = PorNombre (Doc $w) 'Seguir sin seguro'; if ($sin) { Invocar $sin[0] }
    $pago = $null
    for ($s = 0; $s -lt 8 -and -not $pago; $s++) { Start-Sleep 1; $pago = @($w.FindAll($TS::Children, (New-Object $UPC ($UAE::NameProperty, 'Pago seguro')))) | Select-Object -First 1 }
    if (-not $pago) { "vuelta {0}: no se abrió el pago" -f $i; continue }
    Clic $pago; Start-Sleep 1
    # 1. pasar a otra aplicación
    Start-Process $Otra; Start-Sleep 3
    $adel1 = [Tecla]::PidAdelante()
    $vivo = Vigilar $p $Fuera
    if (-not $vivo) { "vuelta {0}: SE CERRÓ estando en la otra aplicación (código {1})" -f $i, $p.ExitCode; break }
    # 2. Win+Shift+S y recortar un rectángulo
    [Tecla]::Combo(0x5B, 0x10, 0x53); Start-Sleep 2
    [Raton]::SetCursorPos(200, 200); [Raton]::mouse_event(2,0,0,0,[UIntPtr]::Zero); Start-Sleep -Milliseconds 200
    for ($k = 1; $k -le 10; $k++) { [Raton]::SetCursorPos(200 + 40*$k, 200 + 25*$k); Start-Sleep -Milliseconds 40 }
    [Raton]::mouse_event(4,0,0,0,[UIntPtr]::Zero)
    $vivo = Vigilar $p 6
    if (-not $vivo) { "vuelta {0}: SE CERRÓ tras Win+Shift+S (código {1})" -f $i, $p.ExitCode; break }
    # 3. volver con Alt+Tab
    [Tecla]::Combo(0x12, 0x09); Start-Sleep 2
    $adel2 = [Tecla]::PidAdelante()
    $vivo = Vigilar $p 8
    if (-not $vivo) { "vuelta {0}: SE CERRÓ al volver (código {1})" -f $i, $p.ExitCode; break }
    Capturar $w (Join-Path $Salida ("vuelta-$i.png"))
    $pago = @($w.FindAll($TS::Children, (New-Object $UPC ($UAE::NameProperty, 'Pago seguro')))) | Select-Object -First 1
    Invocar ((PorNombre $pago 'Cancelar pago')[0]); Start-Sleep 3
    "{0:HH:mm:ss} vuelta {1}: adelante en la otra app pid {2} ({3}); tras Alt+Tab pid {4} (Visual={5}); sin cierre; pago cancelado" -f (Get-Date), $i, $adel1, $(if ($adel1 -eq $p.Id) { 'NO cambió' } else { 'cambió' }), $adel2, ($adel2 -eq $p.Id)
}
$p.Refresh(); "proceso vivo al final: {0}" -f (-not $p.HasExited)
Get-Process Notepad -ErrorAction SilentlyContinue | Stop-Process -Force
"--- errores.log nuevo:"; $fs = [IO.File]::Open($log, 'Open', 'Read', 'ReadWrite'); $fs.Seek($largoAntes, 'Begin') | Out-Null; (New-Object IO.StreamReader($fs)).ReadToEnd(); $fs.Close()
"--- eventos 1000/1026 desde el inicio:"; Get-WinEvent -FilterHashtable @{LogName = 'Application'; StartTime = $inicio } -ErrorAction SilentlyContinue | Where-Object { $_.Id -in 1000, 1026 } | ForEach-Object { "{0} {1} {2}" -f $_.TimeCreated, $_.Id, (($_.Message -split "`n")[0]) }
