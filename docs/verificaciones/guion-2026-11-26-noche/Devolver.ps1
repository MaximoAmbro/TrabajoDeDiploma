param([string]$Pedido, [string]$Salida)
# Guion, sección 4: devolver una entrada de un pedido con seguro desde «Mis entradas».
$v = $PSScriptRoot; . "$v\Uia.ps1"
$p = Get-Process -Id (Get-Content "$v\pid.txt")
function Todos($r) { @($r.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) }
function Textos($r) { (Todos $r | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen -and $_.Current.Name } | ForEach-Object { $_.Current.Name }) -join ' | ' }
function Invocar($e) { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { Clic $e } }
$w = (Ventanas $p)[0]
$menu = Todos $w | Where-Object { $_.Current.Name -match '^Mis entradas' -and -not $_.Current.IsOffscreen } | Select-Object -Last 1
"menú: " + $menu.Current.Name; Clic $menu; Start-Sleep 6
$todos = Todos $w
"textos con el pedido: " + (($todos | Where-Object { $_.Current.Name -match [regex]::Escape($Pedido) } | ForEach-Object { "[" + $_.Current.ControlType.ProgrammaticName + "] " + $_.Current.Name }) -join ' ;; ')
$tarjeta = $todos | Where-Object { $_.Current.Name -match [regex]::Escape($Pedido) -and $_.Current.ControlType -ne [System.Windows.Automation.ControlType]::Text } | Select-Object -First 1
if (-not $tarjeta) { $tarjeta = $todos | Where-Object { $_.Current.Name -match [regex]::Escape($Pedido) } | Select-Object -First 1 }
Invocar $tarjeta; Start-Sleep 3
$casillas = Todos $w | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::CheckBox -and $_.Current.IsEnabled -and -not $_.Current.IsOffscreen }
"casillas habilitadas: " + (($casillas | ForEach-Object { $_.Current.Name }) -join ' / ')
try { $casillas[0].GetCurrentPattern([System.Windows.Automation.TogglePattern]::Pattern).Toggle() } catch { Clic $casillas[0] }
Start-Sleep 2
$boton = Todos $w | Where-Object { $_.Current.Name -match '^Devolver \d' -and -not $_.Current.IsOffscreen } | Select-Object -First 1
"botón: " + $boton.Current.Name + " habilitado=" + $boton.Current.IsEnabled
Capturar $w (Join-Path $Salida '4-devolver-elegida.png')
Invocar $boton; Start-Sleep 2
foreach ($x in (Ventanas $p)) { foreach ($d in @($x.FindAll($TS::Children, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))))) { "cuadro «" + $d.Current.Name + "»: " + (Textos $d); $si = Todos $d | Where-Object { $_.Current.Name -in 'Sí', 'Aceptar', 'Devolver' } | Select-Object -First 1; if ($si) { Invocar $si } } }
Start-Sleep 3
foreach ($x in (Ventanas $p)) { foreach ($d in @($x.FindAll($TS::Children, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))))) { "cuadro «" + $d.Current.Name + "»: " + (Textos $d); $ok = Todos $d | Where-Object { $_.Current.Name -eq 'Aceptar' } | Select-Object -First 1; if ($ok) { Invocar $ok } } }
Start-Sleep 2
"después: " + ((Textos (Ventanas $p)[0]) -replace '\s+', ' ').Substring(0, 900)
Capturar (Ventanas $p)[0] (Join-Path $Salida '4-devolver-hecha.png')
