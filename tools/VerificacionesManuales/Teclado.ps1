# JC-R11: funciones para recorrer el ejecutable real SOLO con teclado. Se cargan con
# «. .\Teclado.ps1». No usan el teclado ni el mouse del escritorio: cada tecla se envía como
# mensaje (WM_KEYDOWN, WM_CHAR, WM_KEYUP) al control que tiene el foco en el hilo de la
# aplicación, que es lo que recibiría si el usuario la pulsara. El foco se lee con
# GetGUIThreadInfo. Limitación: no se pueden enviar combinaciones con Shift, Ctrl o Alt
# (Shift+Tab, Alt+F4), porque el estado de esas teclas no viaja en el mensaje.
# El recorrido del 05/11/2026 y sus resultados: docs/verificaciones/recorrido-teclado.md.
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
if (-not ('Tec3' -as [type])) {
Add-Type @'
using System; using System.Collections.Generic; using System.Runtime.InteropServices; using System.Text;
public static class Tec3 {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
    [StructLayout(LayoutKind.Sequential)] struct GUI { public int cb, flags; public IntPtr active, focus, capture, menu, move, caret; public RECT r; }
    [DllImport("user32.dll")] static extern bool GetGUIThreadInfo(uint t, ref GUI g);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern uint MapVirtualKey(uint c, uint t);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint p);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] public static extern bool IsWindowEnabled(IntPtr h);
    [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr h, int i);
    [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr h, uint f);
    delegate bool Proc(IntPtr h, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr h, Proc cb, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumThreadWindows(uint t, Proc cb, IntPtr p);
    public static IntPtr Foco(uint t) { var g = new GUI(); g.cb = Marshal.SizeOf(g); GetGUIThreadInfo(t, ref g); return g.focus; }
    public static IntPtr Activa(uint t) { var g = new GUI(); g.cb = Marshal.SizeOf(g); GetGUIThreadInfo(t, ref g); return g.active; }
    public static void Tecla(IntPtr h, uint vk) {
        uint sc = MapVirtualKey(vk, 0);
        PostMessage(h, 0x0100, (IntPtr)vk, (IntPtr)(1 | (sc << 16)));
        PostMessage(h, 0x0101, (IntPtr)vk, (IntPtr)unchecked((int)(1 | (sc << 16) | 0xC0000000)));
    }
    public static void Caracter(IntPtr h, char c) {
        // Solo WM_CHAR: un WM_KEYDOWN de letra lo traduce el bucle de mensajes en otro carácter.
        uint sc = MapVirtualKey((uint)char.ToUpperInvariant(c), 0);
        PostMessage(h, 0x0102, (IntPtr)c, (IntPtr)(1 | (sc << 16)));
    }
    public static string Clase(IntPtr h) { var s = new StringBuilder(128); GetClassName(h, s, 128); return s.ToString(); }
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern IntPtr SendMessageTimeout(IntPtr h, uint m, IntPtr w, StringBuilder l, uint f, uint t, out IntPtr r);
    // GetWindowText no lee el texto de los controles de otro proceso: hay que pedirlo con WM_GETTEXT.
    public static string Texto(IntPtr h) { var s = new StringBuilder(512); IntPtr r; SendMessageTimeout(h, 0x000D, (IntPtr)512, s, 2, 1000, out r); return s.ToString(); }
    public static int Estilo(IntPtr h) { return GetWindowLong(h, -16); }
    public static IntPtr[] Hijos(IntPtr h) { var l = new List<IntPtr>(); EnumChildWindows(h, (c, p) => { l.Add(c); return true; }, IntPtr.Zero); return l.ToArray(); }
    public static IntPtr[] Ventanas(uint t) { var l = new List<IntPtr>(); EnumThreadWindows(t, (c, p) => { if (IsWindowVisible(c)) l.Add(c); return true; }, IntPtr.Zero); return l.ToArray(); }
}
'@
}

$VK = @{ Tab = 9; Enter = 13; Esc = 27; Espacio = 32; Arriba = 38; Abajo = 40; Izquierda = 37; Derecha = 39 }
$script:Hilo = 0
$script:Registro = New-Object Collections.Generic.List[string]

function Anotar([string]$texto) { $script:Registro.Add($texto); Write-Output $texto }

function Conectar([Diagnostics.Process]$proceso) {
    $proceso.Refresh()
    $pid2 = 0
    $script:Hilo = [Tec3]::GetWindowThreadProcessId($proceso.MainWindowHandle, [ref]$pid2)
}

function Foco { [Tec3]::Foco($script:Hilo) }
function Activa { [Tec3]::Activa($script:Hilo) }
function TituloActivo { [Tec3]::Texto((Activa)) }

# Nombre legible de un control: tipo, texto y, para los campos, la etiqueta más cercana.
function Describir([IntPtr]$h) {
    if ($h -eq [IntPtr]::Zero) { return '(nada)' }
    $clase = [Tec3]::Clase($h); $texto = [Tec3]::Texto($h).Replace('&', '')
    $estilo = [Tec3]::Estilo($h) -band 0xF
    if ($clase -eq 'Button' -or $clase -like '*BUTTON*') {
        if ($estilo -in 2, 3, 5, 6) { return "casilla «$texto»" }
        if ($estilo -in 4, 9) { return "opción «$texto»" }
        return "botón «$texto»"
    }
    if ($clase -like '*EDIT*') {
        $padre = [Tec3]::GetAncestor($h, 1)
        return "campo «" + (EtiquetaCercana $h $padre) + "»"
    }
    if ($clase -like '*COMBOBOX*') { return "lista «" + (EtiquetaCercana $h ([Tec3]::GetAncestor($h, 1))) + "»" }
    if ($clase -like '*STATIC*') { return "etiqueta «$texto»" }
    if ($clase -like '*Window*') {
        $nombre = try { [Windows.Automation.AutomationElement]::FromHandle($h).Current.ControlType.ProgrammaticName } catch { '' }
        if ($nombre -match 'Table|DataGrid') { return 'grilla' }
        $r = New-Object Tec3+RECT; [Tec3]::GetWindowRect($h, [ref]$r) | Out-Null
        return "control sin nombre ({0}x{1})" -f ($r.R - $r.L), ($r.B - $r.T)
    }
    return "$clase «$texto»"
}

function EtiquetaCercana([IntPtr]$h, [IntPtr]$padre) {
    $r = New-Object Tec3+RECT; [Tec3]::GetWindowRect($h, [ref]$r) | Out-Null
    $mejor = ''; $dist = 9999
    foreach ($c in [Tec3]::Hijos([Tec3]::GetAncestor($h, 2))) {
        if (-not [Tec3]::IsWindowVisible($c) -or [Tec3]::Clase($c) -notlike '*STATIC*') { continue }
        $t = [Tec3]::Texto($c).Trim(); if (-not $t) { continue }
        $e = New-Object Tec3+RECT; [Tec3]::GetWindowRect($c, [ref]$e) | Out-Null
        $arriba = $r.T - $e.B; $izq = $r.L - $e.R
        $d = if ($arriba -ge -4 -and $arriba -lt 40 -and $e.L -lt $r.R -and $e.R -gt $r.L) { $arriba }
             elseif ($izq -ge -4 -and $izq -lt 200 -and [Math]::Abs($e.T - $r.T) -lt 15) { $izq + 50 } else { 9999 }
        if ($d -lt $dist) { $dist = $d; $mejor = $t }
    }
    if ($mejor) { $mejor } else { 'sin rótulo' }
}

function Pulsar([string]$tecla, [int]$veces = 1) {
    for ($i = 0; $i -lt $veces; $i++) { [Tec3]::Tecla((Foco), [uint32]$VK[$tecla]); Start-Sleep -Milliseconds 250 }
}

function Escribir([string]$texto) {
    foreach ($c in $texto.ToCharArray()) { [Tec3]::Caracter((Foco), $c); Start-Sleep -Milliseconds 40 }
    Start-Sleep -Milliseconds 200
}

# Pulsa Tab hasta volver al control de partida y devuelve el recorrido. Si el foco se queda
# en el mismo control varias veces seguidas (una grilla recorre sus celdas), lo cuenta.
function Recorrer([int]$maximo = 60) {
    $inicio = Foco; $vistos = New-Object Collections.Generic.List[IntPtr]; $vistos.Add($inicio)
    $orden = New-Object Collections.Generic.List[string]; $orden.Add((Describir $inicio))
    $repetidos = 0
    for ($i = 1; $i -le $maximo; $i++) {
        Pulsar 'Tab'
        $f = Foco
        if ($f -eq $vistos[$vistos.Count - 1]) { $repetidos++; continue }
        if ($f -eq $inicio) { break }
        if ($vistos.Contains($f)) { $orden.Add('… vuelve a ' + (Describir $f)); break }
        $vistos.Add($f); $orden.Add((Describir $f))
    }
    [pscustomobject]@{ Orden = $orden; Vistos = $vistos; Tabs = $i; EnElMismo = $repetidos }
}

# Controles visibles y habilitados de la ventana activa que el recorrido con Tab no alcanzó.
function NoAlcanzados($vistos, [string[]]$etiquetasConClic) {
    $faltan = @()
    foreach ($c in [Tec3]::Hijos((Activa))) {
        if (-not [Tec3]::IsWindowVisible($c) -or -not [Tec3]::IsWindowEnabled($c) -or $vistos.Contains($c)) { continue }
        $clase = [Tec3]::Clase($c); $texto = [Tec3]::Texto($c).Trim()
        $interactivo = $clase -like '*EDIT*' -or $clase -like '*BUTTON*' -or $clase -like '*COMBOBOX*' -or
                       ($clase -like '*STATIC*' -and $etiquetasConClic -contains $texto)
        if ($interactivo) {
            $faltan += (Describir $c) + $(if ($clase -like '*STATIC*') { ' (etiqueta con clic: no recibe el foco)' } else { '' })
        }
    }
    $faltan
}

# Tab hasta que el foco describa algo que coincida con el patrón. Devuelve cuántos Tab hicieron falta.
function IrA([string]$patron, [int]$maximo = 40) {
    for ($i = 0; $i -le $maximo; $i++) {
        if ((Describir (Foco)) -match $patron) { return $i }
        Pulsar 'Tab'
    }
    return -1
}

function EsperarVentana([string]$patron, [int]$segundos = 15) {
    $fin = (Get-Date).AddSeconds($segundos)
    while ((Get-Date) -lt $fin) { if ((TituloActivo) -match $patron) { return $true }; Start-Sleep -Milliseconds 300 }
    return $false
}
