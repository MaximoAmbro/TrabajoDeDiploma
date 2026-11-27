param([string]$Salida)
# Guion, sección 6.1: contratar el plan Mensual con Enter y aceptar.
$v = $PSScriptRoot; . "$v\Uia.ps1"
$p = Get-Process -Id (Get-Content "$v\pid.txt")
function Todos($r) { @($r.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) }
function Textos($r) { (Todos $r | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen -and $_.Current.Name } | ForEach-Object { $_.Current.Name }) -join ' | ' }
function Invocar($e) { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { Clic $e } }
$w = (Ventanas $p)[0]
Clic (Todos $w | Where-Object { $_.Current.Name -eq 'Mi suscripción' -and -not $_.Current.IsOffscreen } | Select-Object -Last 1); Start-Sleep 6
"antes: " + (Textos $w)
$mensual = Todos $w | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::RadioButton -and $_.Current.Name -match 'Mensual' } | Select-Object -First 1
Clic $mensual; Start-Sleep 1; [System.Windows.Forms.SendKeys]::SendWait('{ENTER}'); Start-Sleep 2
foreach ($d in @($w.FindAll($TS::Children, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))))) { "cuadro «" + $d.Current.Name + "»: " + (Textos $d); Capturar $d (Join-Path $Salida '6-confirmar.png'); Invocar (Todos $d | Where-Object { $_.Current.Name -eq 'Sí' } | Select-Object -First 1) }
Start-Sleep 3
foreach ($d in @($w.FindAll($TS::Children, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))))) { "cuadro «" + $d.Current.Name + "»: " + (Textos $d); Invocar (Todos $d | Where-Object { $_.Current.Name -eq 'Aceptar' } | Select-Object -First 1) }
Start-Sleep 3
"después: " + (Textos $w)
"botones visibles: " + ((Todos $w | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button -and -not $_.Current.IsOffscreen -and $_.Current.Name -match 'Contratar|Cancelar suscripción' } | ForEach-Object { $_.Current.Name + " (hab=" + $_.Current.IsEnabled + ")" }) -join ', ')
Capturar $w (Join-Path $Salida '6-suscripcion-activa.png')
