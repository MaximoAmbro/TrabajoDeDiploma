# Comprueba que las secciones marcadas <!-- compartido:NOMBRE --> ... <!-- /compartido -->
# sean idénticas en CLAUDE.md (Claude Code) y AGENTS.md (Codex).
#   tools\VerificarReglasAgentes.ps1               -> informa diferencias; sale con 1 si hay
#   tools\VerificarReglasAgentes.ps1 -Sincronizar  -> copia las secciones de CLAUDE.md a AGENTS.md
param([switch]$Sincronizar)

$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
$claude = Join-Path $raiz 'CLAUDE.md'
$agents = Join-Path $raiz 'AGENTS.md'
$patron = '(?s)<!-- compartido:(?<nombre>[a-z]+)[^>]*-->\r?\n(?<cuerpo>.*?)<!-- /compartido -->'
$utf8 = New-Object System.Text.UTF8Encoding($false)

function Bloques([string]$texto) {
    $bloques = [ordered]@{}
    foreach ($m in [regex]::Matches($texto, $patron)) {
        $bloques[$m.Groups['nombre'].Value] = ($m.Groups['cuerpo'].Value -replace "`r`n", "`n")
    }
    $bloques
}

$textoClaude = [IO.File]::ReadAllText($claude, $utf8)
$textoAgents = [IO.File]::ReadAllText($agents, $utf8)
$deClaude = Bloques $textoClaude
$deAgents = Bloques $textoAgents

if ($deClaude.Count -eq 0) { throw 'CLAUDE.md no tiene secciones marcadas como compartidas.' }

if ($Sincronizar) {
    $nuevo = [regex]::Replace($textoAgents, $patron, {
        param($m)
        $nombre = $m.Groups['nombre'].Value
        if (-not $deClaude.Contains($nombre)) { return $m.Value }
        $cabecera = $m.Value.Substring(0, $m.Value.IndexOf('-->') + 3)
        $cuerpo = $deClaude[$nombre]
        if ($textoAgents.Contains("`r`n")) { $cuerpo = $cuerpo -replace "`n", "`r`n"; $salto = "`r`n" } else { $salto = "`n" }
        "$cabecera$salto$cuerpo<!-- /compartido -->"
    })
    [IO.File]::WriteAllText($agents, $nuevo, $utf8)
    $textoAgents = $nuevo
    $deAgents = Bloques $textoAgents
}

$diferencias = 0
foreach ($nombre in $deClaude.Keys) {
    if (-not $deAgents.Contains($nombre)) {
        Write-Output "FALTA  en AGENTS.md: sección compartida '$nombre'"; $diferencias++
    } elseif ($deClaude[$nombre] -ne $deAgents[$nombre]) {
        Write-Output "DIFIERE  sección '$nombre'"; $diferencias++
    } else {
        Write-Output "IGUAL  sección '$nombre'"
    }
}
foreach ($nombre in $deAgents.Keys) {
    if (-not $deClaude.Contains($nombre)) { Write-Output "SOBRA  en AGENTS.md: sección '$nombre'"; $diferencias++ }
}
exit [int]($diferencias -gt 0)
