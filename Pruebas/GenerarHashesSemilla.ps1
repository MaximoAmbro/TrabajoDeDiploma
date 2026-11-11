param([switch]$ActualizarScripts)

$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
# Usa exactamente la implementación de producción, sin conexión a ninguna base.
Add-Type -Path (Join-Path $raiz 'Repo/Seguridad.cs')
$admin = [Negocio.Seguridad]::Hash('Admin1234')
$cliente = [Negocio.Seguridad]::Hash('Cliente123')
if (-not [Negocio.Seguridad]::Verificar('Admin1234', $admin) -or
    -not [Negocio.Seguridad]::Verificar('Cliente123', $cliente)) { throw 'Falló la verificación de los hashes.' }
if ($ActualizarScripts) {
    $utf8 = New-Object System.Text.UTF8Encoding($true)
    foreach ($nombre in @('02-semilla.sql', '03-datos-reporte.sql')) {
        $ruta = Join-Path $raiz "BaseDatos/$nombre"
        $texto = [IO.File]::ReadAllText($ruta)
        # También permite regenerar hashes previos, sin cambiar las credenciales de demostración.
        $texto = [regex]::Replace($texto, "('admin@eventaura.com', )'[^']*'", { param($m) $m.Groups[1].Value + "'$admin'" })
        $texto = [regex]::Replace($texto, "('(?:bronce|plata|oro)@eventaura.com', )'[^']*'", { param($m) $m.Groups[1].Value + "'$cliente'" })
        if ($nombre -eq '03-datos-reporte.sql') {
            $texto = [regex]::Replace($texto, "(?m)^(\s*)'(?:Cliente123|PBKDF2[^']*)', 'Cliente',", { param($m) $m.Groups[1].Value + "'$cliente', 'Cliente'," })
        }
        [IO.File]::WriteAllText($ruta, $texto, $utf8)
    }
    'Hashes regenerados y verificados en ambos scripts. No se modificó ninguna base.'
} else {
    [pscustomobject]@{ Administrador = $admin; Clientes = $cliente }
}
