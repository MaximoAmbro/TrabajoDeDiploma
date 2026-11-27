param([string]$Evento, [string]$Sector, [int]$Cantidad = 1, [switch]$ConSeguro, [string]$Titular = 'Prueba Guion', [string]$Tarjeta = '4111 1111 1111 1111', [string]$Salida, [string]$Etiqueta = 'compra')
# Desde el catálogo de «Mi cuenta»: arma el paquete en el sector pedido, con o sin seguro, y paga.
$v = $PSScriptRoot; . "$v\Uia.ps1"
New-Item -ItemType Directory -Force $Salida | Out-Null
$p = Get-Process -Id (Get-Content "$v\pid.txt")
function Doc($w) { @($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Document)))) | Select-Object -First 1 }
function Invocar($e) { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { Clic $e } }
function Todos($r) { @($r.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) }
function Textos($r) { (Todos $r | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen -and $_.Current.Name } | ForEach-Object { $_.Current.Name }) -join ' | ' }
$w = (Ventanas $p)[0]
Add-Type @"
using System; using System.Runtime.InteropServices;
public static class Frente { [DllImport("user32.dll")] public static extern void keybd_event(byte k, byte s, uint f, UIntPtr e); [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h); [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int c);
  public static void Traer(IntPtr h) { keybd_event(0x12,0,0,UIntPtr.Zero); keybd_event(0x12,0,2,UIntPtr.Zero); ShowWindow(h, 9); SetForegroundWindow(h); } }
"@
[Frente]::Traer([IntPtr]$w.Current.NativeWindowHandle); Start-Sleep 1
Clic (Buscar $w 'Eventos disponibles')[-1]; Start-Sleep 6
$t = Todos (Doc $w); $i = [Array]::FindIndex([object[]]$t, [Predicate[object]] { param($x) $x.Current.Name -eq $Evento })
if ($i -lt 0) { throw "No está $Evento" }
Invocar ($t[$i..($t.Count - 1)] | Where-Object { $_.Current.Name -eq 'Ver entradas' } | Select-Object -First 1); Start-Sleep 5
Invocar (@($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::NameProperty, 'Armar paquete')))) | Where-Object { $_.Current.ControlType -ne [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen } | Select-Object -Last 1); Start-Sleep 7
$doc = Doc $w
$grupo = Todos $doc | Where-Object { $_.Current.Name -eq "Entradas de $Sector" } | Select-Object -First 1
if (-not $grupo) { $grupo = $doc }
$mas = Todos $grupo | Where-Object { $_.Current.Name -eq 'Agregar una entrada' } | Select-Object -First 1
for ($k = 0; $k -lt $Cantidad; $k++) { Invocar $mas; Start-Sleep -Milliseconds 400 }
if ($ConSeguro) { $s = Todos $doc | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::CheckBox -and $_.Current.Name -match 'Seguro' } | Select-Object -First 1; try { $s.GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern).Toggle() } catch { Clic $s }; Start-Sleep 1 }
Start-Sleep 1
$desglose = Todos $doc | Where-Object { $_.Current.Name -eq 'Desglose del paquete' } | Select-Object -First 1
"desglose: " + (Textos $desglose)
"total: " + ((Todos $doc | Where-Object { $_.Current.Name -match '^\$ [\d\.]+,\d\d$' } | Select-Object -Last 1).Current.Name)
Invocar ((Todos $doc | Where-Object { $_.Current.Name -eq 'Continuar al pago' })[0]); Start-Sleep 3
$sin = Todos (Doc $w) | Where-Object { $_.Current.Name -eq 'Seguir sin seguro' -and -not $_.Current.IsOffscreen }
if ($sin) { "aviso de seguro: " + ((Todos (Doc $w) | Where-Object { $_.Current.Name -like 'No agregaste el seguro*' } | Select-Object -First 1).Current.Name); Capturar $w (Join-Path $Salida "$Etiqueta-aviso-seguro.png"); Invocar $sin[0] }
$pago = $null; for ($s = 0; $s -lt 10 -and -not $pago; $s++) { Start-Sleep 1; $pago = @($w.FindAll($TS::Children, (New-Object $UPC ($UAE::NameProperty, 'Pago seguro')))) | Select-Object -First 1 }
if (-not $pago) { throw 'No se abrió el pago' }
[Frente]::Traer([IntPtr]$pago.Current.NativeWindowHandle); Start-Sleep 1
$cs = Todos $pago | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Pane -and -not $_.Current.Name -and $_.Current.BoundingRectangle.Height -le 40 } | Sort-Object { $_.Current.BoundingRectangle.Y }, { $_.Current.BoundingRectangle.X }
Escribir $cs[0] $Titular; Escribir $cs[1] $Tarjeta; Escribir $cs[2] '1229'; Escribir $cs[3] '123'
Capturar $pago (Join-Path $Salida "$Etiqueta-pago.png")
Invocar ((Todos $pago | Where-Object { $_.Current.Name -eq 'Pagar' })[0]); Start-Sleep 6
$todas = Ventanas $p
foreach ($x in $todas) { "ventana «{0}»: {1}" -f $x.Current.Name, ((Textos $x) -replace '\s+', ' ').Substring(0, [Math]::Min(600, ((Textos $x) -replace '\s+', ' ').Length)) }
Capturar $todas[0] (Join-Path $Salida "$Etiqueta-resultado.png")
