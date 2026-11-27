param([string]$Mail, [string]$Nueva, [string]$Salida)
# Guion, sección 2, pasos 4 y 5: recuperación completa con el código de la bandeja simulada, y entrar con la clave nueva.
$v = $PSScriptRoot; . "$v\Uia.ps1"
New-Item -ItemType Directory -Force $Salida | Out-Null
$p = Get-Process -Id (Get-Content "$v\pid.txt")
function Doc($w) { @($w.FindAll($TS::Descendants, (New-Object $UPC ($UAE::ControlTypeProperty, [System.Windows.Automation.ControlType]::Document)))) | Select-Object -First 1 }
function Texto($w) { (@((Doc $w).FindAll($TS::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Text -and -not $_.Current.IsOffscreen } | ForEach-Object { $_.Current.Name }) -join ' | ' }
$w = (Ventanas $p)[0]
if ($w.Current.Name -eq 'Bienvenida') { Clic (Buscar $w 'Iniciar sesión Ya tengo una cuenta.')[0]; Start-Sleep 4; $w = (Ventanas $p)[0] }
Clic (Buscar $w '¿La olvidaste?')[0]; Start-Sleep 4
$w = (Ventanas $p)[0]; "ventana: " + $w.Current.Name
$cs = Campos $w; Escribir $cs[0] $Mail; Clic (Buscar $w 'Enviar código')[0]
$t0 = Get-Date; $bandeja = "$env:LOCALAPPDATA\EventAura\BandejaSimulada"
$eml = $null
for ($i = 0; $i -lt 20 -and -not $eml; $i++) { Start-Sleep 1; $eml = Get-ChildItem $bandeja -Filter *.eml | Where-Object { $_.LastWriteTime -gt $t0.AddSeconds(-1) } | Sort-Object LastWriteTime | Select-Object -Last 1 }
Start-Sleep 2
"tras Enviar código: " + (Texto $w)
Capturar $w (Join-Path $Salida '2-recuperacion-codigo.png')
$lineas = Get-Content $eml.FullName -Encoding UTF8
"eml: " + (($lineas | Where-Object { $_ -match '^(To|Subject):' }) -join ' / ')
$codigo = (($lineas | Where-Object { $_ -match '^Código:' }) -replace '\D', '')
$cs = Campos $w; "campos visibles: $($cs.Count)"
Escribir $cs[0] $codigo; Escribir $cs[1] $Nueva; Escribir $cs[2] $Nueva
Clic (Buscar $w 'Restablecer contraseña' | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button } | Select-Object -First 1); Start-Sleep 3
"tras Restablecer: " + (Texto $w)
Capturar $w (Join-Path $Salida '2-recuperacion-lista.png')
# Volver al acceso y entrar con la nueva
$ir = Buscar $w 'Iniciar sesión' | Where-Object { $_.Current.ControlType -eq [System.Windows.Automation.ControlType]::Button } | Select-Object -Last 1
Clic $ir; Start-Sleep 4; $w = (Ventanas $p)[0]
$cs = Campos $w; Escribir $cs[0] $Mail; Escribir $cs[1] $Nueva; [System.Windows.Forms.SendKeys]::SendWait('{ENTER}'); Start-Sleep 7
$w = (Ventanas $p)[0]; "después de entrar con la nueva: ventana «" + $w.Current.Name + "»"
Capturar $w (Join-Path $Salida '2-entra-con-la-nueva.png')
