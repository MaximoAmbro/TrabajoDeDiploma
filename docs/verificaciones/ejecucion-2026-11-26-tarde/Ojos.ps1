# Cuenta los ojos de contraseña, las casillas visibles y los campos secretos de una ventana.
function Ojos($w, $tag) {
    $todos = @($w.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition))
    $ojos = @($todos | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button -and $_.Current.Name -match '(Mostrar|Ocultar) contrase' -and -not $_.Current.IsOffscreen })
    $casillas = @($todos | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::CheckBox -and -not $_.Current.IsOffscreen })
    $secretos = @(Campos $w | Where-Object { $_.Current.IsPassword })
    Write-Host ("{0}: ojos visibles={1}; casillas visibles={2} [{3}]; campos secretos={4}" -f $tag, $ojos.Count, $casillas.Count, (($casillas | ForEach-Object { $_.Current.Name }) -join ','), $secretos.Count)
    foreach ($o in $ojos) { $r = $o.Current.BoundingRectangle; Write-Host ("   ojo «{0}» {1}x{2} en ({3},{4})" -f $o.Current.Name, [int]$r.Width, [int]$r.Height, [int]$r.X, [int]$r.Y) }
    return , $ojos
}
# Escribe en el campo secreto número $i, pulsa su ojo y dice si el texto quedó a la vista.
function ProbarOjo($w, $tag) {
    $ojos = Ojos $w $tag
    $secretosAntes = @(Campos $w | Where-Object { $_.Current.IsPassword }).Count
    foreach ($o in $ojos) { Clic $o; Start-Sleep -Milliseconds 500 }
    $secretosDespues = @(Campos $w | Where-Object { $_.Current.IsPassword }).Count
    $nombres = (@($w.FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button -and $_.Current.Name -match '(Mostrar|Ocultar) contrase' } | ForEach-Object { $_.Current.Name }) -join ' / '
    Write-Host ("   tras pulsar los {0} ojos: secretos {1} -> {2}; etiquetas: {3}" -f $ojos.Count, $secretosAntes, $secretosDespues, $nombres)
    foreach ($o in (Ojos $w ($tag + ' (abiertos)'))) { Clic $o; Start-Sleep -Milliseconds 300 }
}
