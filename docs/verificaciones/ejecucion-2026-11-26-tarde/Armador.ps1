param([string]$Evento, [string]$Salida)
# Desde el catálogo web de «Mi cuenta»: abre la ficha del evento, Armar paquete, y mide el armador.
$v = $PSScriptRoot; . "$v\Uia.ps1"
$p = Get-Process -Id (Get-Content "$v\pid.txt")
function Doc($w) { @($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Document)))) | Select-Object -First 1 }
function Invocar($e) { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { Clic $e } }
$w = (Ventanas $p)[0]
Clic (Buscar $w 'Eventos disponibles')[-1]; Start-Sleep 6
$doc = Doc $w
$todos = @($doc.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition))
# El botón «Ver entradas» que sigue al título del evento.
$i = [Array]::FindIndex([object[]]$todos, [Predicate[object]] { param($x) $x.Current.Name -eq $Evento })
if ($i -lt 0) { "NO ENCONTRADO: $Evento"; exit 1 }
$boton = $todos[$i..($todos.Count - 1)] | Where-Object { $_.Current.Name -eq 'Ver entradas' } | Select-Object -First 1
Invocar $boton; Start-Sleep 5
$armar = @($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::NameProperty, 'Armar paquete')))) | Where-Object { $_.Current.ControlType -ne [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen } | Select-Object -Last 1
Invocar $armar; Start-Sleep 7
$doc = Doc $w; $wr = $w.Current.BoundingRectangle; $dr = $doc.Current.BoundingRectangle
$t = @($doc.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition))
$nombres = $t | ForEach-Object { $_.Current.Name }
$iEsc = [Array]::IndexOf([object[]]$nombres, 'ESCENARIO')
$grupos = $t | Where-Object { $_.Current.Name -like 'Entradas de *' } | ForEach-Object { $_.Current.Name -replace '^Entradas de ', '' }
function Med($n) {
    $e = $t | Where-Object { $_.Current.Name -eq $n } | Select-Object -First 1
    if (-not $e) { return "${n}: no está" }
    $r = $e.Current.BoundingRectangle
    $dentro = ($r.X -ge $wr.X) -and ($r.Right -le $wr.Right) -and ($r.Y -ge $dr.Y) -and ($r.Bottom -le $dr.Bottom)
    "{0}: x {1}..{2}, y {3}..{4} (vista y {5}..{6}) → {7}" -f $n, [int]($r.X - $wr.X), [int]($r.Right - $wr.X), [int]($r.Y - $wr.Y), [int]($r.Bottom - $wr.Y), [int]($dr.Y - $wr.Y), [int]($dr.Bottom - $wr.Y), $(if ($dentro) { 'visible entero' } else { 'FUERA o cortado' })
}
"== $Evento"
"   mapa: ESCENARIO {0}; sectores en orden: {1}" -f $(if ($iEsc -ge 0) { 'dibujado' } else { 'NO aparece' }), ($grupos -join ' > ')
"   precios en orden: " + (($t | Where-Object { $_.Current.Name -match '^\$ [\d\.]+,\d\d$' } | Select-Object -First 8 | ForEach-Object { $_.Current.Name }) -join ' | ')
"   " + (Med 'TOTAL'); "   " + (Med 'Continuar al pago'); "   " + (Med 'Tu paquete')
$serv = $t | Where-Object { $_.Current.Name -match '^Servicios' -and $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Group } | Select-Object -First 1
$paq = $t | Where-Object { $_.Current.Name -eq 'Tu paquete' -and $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Group } | Select-Object -First 1
$ent = $t | Where-Object { $_.Current.Name -eq 'Entradas' -and $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Group } | Select-Object -First 1
foreach ($g in @($ent, $serv)) { if ($g -and $paq) { $a = $g.Current.BoundingRectangle; $b = $paq.Current.BoundingRectangle; "   separación «{0}» → «Tu paquete»: {1} px (se superponen: {2})" -f $g.Current.Name, [int]($b.X - $a.Right), $a.IntersectsWith($b) } }
Capturar $w (Join-Path $Salida ("armador-" + ($Evento -replace '[^\w]+', '-') + ".png"))
