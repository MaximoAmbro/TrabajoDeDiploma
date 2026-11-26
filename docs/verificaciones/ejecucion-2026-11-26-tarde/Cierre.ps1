param([string]$Copia, [string]$Salida, [int]$Veces = 10, [int]$Espera = 45)
# T-38: abre el pago, espera sin tocar, lo cancela y repite. Vigila el proceso cada segundo.
$v = $PSScriptRoot; . "$v\Uia.ps1"
New-Item -ItemType Directory -Force $Salida | Out-Null
$log = "$env:LOCALAPPDATA\EventAura\errores.log"; $largoAntes = (Get-Item $log).Length; $inicio = Get-Date
function Doc($w) { @($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Document)))) | Select-Object -First 1 }
function Invocar($e) { try { $e.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke() } catch { Clic $e } }
function PorNombre($raiz, $n) { @($raiz.FindAll($TS::Descendants, (New-Object $UPC ($UAE::NameProperty, $n)))) }
$p = Start-Process "$Copia\Visual.exe" -WorkingDirectory $Copia -PassThru; Set-Content "$v\pid.txt" $p.Id
"{0:HH:mm:ss} Visual.exe pid {1}" -f (Get-Date), $p.Id
Start-Sleep 7; $w = (Ventanas $p)[0]; Clic (Buscar $w 'Iniciar sesión Ya tengo una cuenta.')[0]; Start-Sleep 4
$w = (Ventanas $p)[0]; $cs = Campos $w; Escribir $cs[0] 'bronce@eventaura.com'; Escribir $cs[1] 'Cliente123'; [System.Windows.Forms.SendKeys]::SendWait('{ENTER}'); Start-Sleep 7
$w = (Ventanas $p)[0]
for ($i = 1; $i -le $Veces; $i++) {
    if ($p.HasExited) { "TERMINÓ antes de la vuelta $i (código $($p.ExitCode))"; break }
    Clic (Buscar $w 'Eventos disponibles')[-1]; Start-Sleep 6
    Invocar ((PorNombre (Doc $w) 'Ver entradas')[0]); Start-Sleep 5
    Invocar (PorNombre $w 'Armar paquete' | Where-Object { $_.Current.ControlType -ne [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen } | Select-Object -Last 1); Start-Sleep 7
    Invocar ((PorNombre (Doc $w) 'Agregar una entrada')[0]); Start-Sleep 1
    Invocar ((PorNombre (Doc $w) 'Continuar al pago')[0]); Start-Sleep 3
    $sin = PorNombre (Doc $w) 'Seguir sin seguro'; if ($sin) { Invocar $sin[0] }
    $t0 = Get-Date; $pago = $null
    for ($s = 0; $s -lt 8 -and -not $pago; $s++) { Start-Sleep 1; $pago = @($w.FindAll($TS::Children, (New-Object $UPC ($UAE::NameProperty, 'Pago seguro')))) | Select-Object -First 1 }
    if (-not $pago) { "vuelta {0}: no se abrió el pago" -f $i; continue }
    $cerro = $false
    while (((Get-Date) - $t0).TotalSeconds -lt $Espera) { $p.Refresh(); if ($p.HasExited) { $cerro = $true; break }; Start-Sleep 1 }
    if ($cerro) { "vuelta {0}: SE CERRÓ a los {1:n1} s del pago (código {2})" -f $i, ((Get-Date) - $t0).TotalSeconds, $p.ExitCode; break }
    Invocar ((PorNombre $pago 'Cancelar pago')[0]); Start-Sleep 3
    "{0:HH:mm:ss} vuelta {1}: pago abierto {2} s sin cierre; cancelado" -f (Get-Date), $i, $Espera
}
$p.Refresh(); "proceso vivo al final: {0}" -f (-not $p.HasExited)
Capturar $w (Join-Path $Salida 'cierre-final.png')
"--- errores.log nuevo:"; $fs = [IO.File]::Open($log, 'Open', 'Read', 'ReadWrite'); $fs.Seek($largoAntes, 'Begin') | Out-Null; (New-Object IO.StreamReader($fs)).ReadToEnd(); $fs.Close()
"--- eventos 1000/1026 desde el inicio:"; Get-WinEvent -FilterHashtable @{LogName = 'Application'; StartTime = $inicio } -ErrorAction SilentlyContinue | Where-Object { $_.Id -in 1000, 1026 } | ForEach-Object { "{0} {1} {2}" -f $_.TimeCreated, $_.Id, (($_.Message -split "`n")[0]) }
