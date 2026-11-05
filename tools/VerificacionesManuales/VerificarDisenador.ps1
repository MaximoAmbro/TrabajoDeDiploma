# CX-03, parte técnica: comprueba por reflexión las condiciones que el diseñador de Visual
# Studio necesita para abrir los formularios que heredan de FormularioEventAura. No abre Visual
# Studio: la confirmación visual (que el diseñador dibuje la pantalla) sigue siendo de JC.
#   1. La base no es abstracta y tiene un constructor público sin parámetros: el diseñador
#      instancia la clase BASE, no el formulario.
#   2. El constructor de la base no hace nada que falle en diseño (no tiene código propio).
#   3. Cada formulario hereda de la base, tiene constructor público sin parámetros e
#      InitializeComponent.
#   4. En cada .cs, la primera clase declarada es el formulario (si no, el diseñador abre otra).
# Lee Visual\bin\Debug\Visual.exe en un proceso aparte, sin bloquear el archivo.
# Deja el resultado en docs/verificaciones/disenador-reflexion.txt.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$exe = Join-Path $raiz 'Visual\bin\Debug\Visual.exe'
$salida = Join-Path $raiz 'docs\verificaciones'
New-Item -ItemType Directory -Force $salida | Out-Null
$log = New-Object Collections.Generic.List[string]
$fallas = 0
function Anotar($ok, $texto) {
    $script:log.Add(($(if ($ok) { 'OK     ' } else { 'FALLA  ' })) + $texto)
    if (-not $ok) { $script:fallas++ }
}

$asm = [Reflection.Assembly]::Load([IO.File]::ReadAllBytes($exe))
[AppDomain]::CurrentDomain.add_AssemblyResolve({
    param($s, $e)
    $dll = Join-Path (Split-Path $exe) ((New-Object Reflection.AssemblyName($e.Name)).Name + '.dll')
    if (Test-Path $dll) { [Reflection.Assembly]::Load([IO.File]::ReadAllBytes($dll)) }
})
$base = $asm.GetType('Visual.FormularioEventAura', $true)
$log.Add("CX-03 (reflexión) — " + (Get-Date -Format 'dd/MM/yyyy HH:mm') + " — Visual.exe del " + (Get-Item $exe).LastWriteTime.ToString('dd/MM/yyyy HH:mm'))
$log.Add('')
$log.Add('Clase base Visual.FormularioEventAura')
Anotar (-not $base.IsAbstract) "no es abstracta"
Anotar ($base.BaseType -eq [Windows.Forms.Form]) ("hereda de " + $base.BaseType.FullName)
$ctor = $base.GetConstructor([Type]::EmptyTypes)
Anotar ($ctor -ne $null -and $ctor.IsPublic) "tiene constructor público sin parámetros"
$cuerpo = $ctor.GetMethodBody().GetILAsByteArray()
# Un constructor vacío compila a: ldarg.0, call Form::.ctor, (nop), ret = 7 u 8 bytes de IL.
Anotar ($cuerpo.Length -le 8) ("el constructor no tiene código propio (" + $cuerpo.Length + " bytes de IL)")
try { $instancia = [Activator]::CreateInstance($base); $instancia.Dispose(); Anotar $true "se puede instanciar, como hace el diseñador" }
catch { Anotar $false ("no se puede instanciar: " + $_.Exception.InnerException.Message) }

$log.Add('')
$log.Add('Formularios que heredan de la base')
$formularios = $asm.GetTypes() | Where-Object { $_.IsSubclassOf([Windows.Forms.Form]) -and $_ -ne $base } | Sort-Object Name
foreach ($f in $formularios) {
    $c = $f.GetConstructor([Type]::EmptyTypes)
    $ic = $f.GetMethod('InitializeComponent', [Reflection.BindingFlags]'Instance,NonPublic,Public')
    $archivo = Get-ChildItem (Join-Path $raiz 'Visual') -Recurse -Filter ($f.Name + '.cs') | Where-Object { $_.Name -notlike '*.Designer.cs' } | Select-Object -First 1
    $primera = if ($archivo) { ([regex]::Match([IO.File]::ReadAllText($archivo.FullName), '\b(?:class|struct|enum|interface)\s+(\w+)')).Groups[1].Value } else { '(sin archivo)' }
    $ok = ($f.BaseType -eq $base) -and $c -and $c.IsPublic -and $ic -and ($primera -eq $f.Name)
    Anotar $ok ("{0,-26} base {1}; constructor sin parámetros {2}; InitializeComponent {3}; primera clase del .cs: {4}" -f
        $f.Name, $f.BaseType.Name, $(if ($c -and $c.IsPublic) { 'sí' } else { 'NO' }), $(if ($ic) { 'sí' } else { 'NO' }), $primera)
}
$log.Add('')
$log.Add("Formularios: " + @($formularios).Count + ". Fallas: $fallas.")
$log.Add('Falta la confirmación visual: abrir FrmCompraCliente y frmInicio en el diseñador de Visual Studio.')
[IO.File]::WriteAllLines((Join-Path $salida 'disenador-reflexion.txt'), $log, (New-Object Text.UTF8Encoding($false)))
$log
if ($fallas) { exit 1 }
