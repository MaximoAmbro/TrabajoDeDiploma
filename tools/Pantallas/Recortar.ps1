# Recorta el detalle legible de las cinco vistas de «Mi cuenta» (semilla de Paula, 18/11/2026)
# para el anexo «Pantallas del sistema». No toca las capturas de Codex: lee de Pruebas/ y escribe
# en docs/capturas/pantallas/. Informa el ancho de inserción y la letra impresa de cada recorte.
#
# Uso: powershell -NoProfile -ExecutionPolicy Bypass -File tools\Pantallas\Recortar.ps1

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$raiz    = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$origen  = Join-Path $raiz 'Pruebas\Recortes-Documento-Fases7-8\completas\MiCuenta-2026-11-18'
$destino = Join-Path $raiz 'docs\capturas\pantallas'
New-Item -ItemType Directory -Force $destino | Out-Null

# La letra más chica de las capturas es de 9 pt a 96 ppp (medición de Codex en ancho-minimo-8_1pt.tsv).
$LETRA_ORIGEN = 9.0
$LETRA_MINIMA = 8.0   # regla del documento para capturas
$ANCHO_PAGINA = 16.0  # cm, página vertical
$ANCHO_APAISADA = 24.7

# archivo origen, archivo destino, x, y, ancho, alto, contenido
$recortes = @(
    @('eventos.png',      'eventos-tarjetas.png',      320, 225, 576, 372, 'Tarjetas de los eventos'),
    @('ficha-evento.png', 'ficha-portada-datos.png',   312,  95, 648, 200, 'Portada y datos del evento'),
    @('ficha-evento.png', 'ficha-mapa.png',            315, 322, 863, 280, 'Mapa de sectores'),
    @('compra.png',       'compra-entradas.png',       322, 214, 396, 442, 'Entradas y cantidades'),
    @('compra.png',       'compra-resumen.png',        739, 214, 382, 366, 'Resumen y total'),
    @('mis-entradas.png', 'mis-entradas-izquierda.png',322, 222, 484, 224, 'Pedido, evento, fecha y estado'),
    @('mis-entradas.png', 'mis-entradas-derecha.png',  806, 222, 372, 224, 'Entradas, total pagado, devolución, seguro y código'),
    @('suscripcion.png',  'suscripcion-panel.png',     318, 168, 404, 404, 'Plan y beneficios')
)

'{0,-28} {1,6} {2,6} {3,9} {4,9} {5,7}  {6}' -f 'recorte','px','alto','natural','insertar','letra','contenido'
foreach ($r in $recortes) {
    $img = [System.Drawing.Image]::FromFile((Join-Path $origen $r[0]))
    try {
        $rect = New-Object System.Drawing.Rectangle $r[2], $r[3], $r[4], $r[5]
        if ($rect.Right -gt $img.Width -or $rect.Bottom -gt $img.Height) { throw "El recorte $($r[1]) sale de la imagen." }
        $bmp = New-Object System.Drawing.Bitmap $r[4], $r[5]
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.DrawImage($img, (New-Object System.Drawing.Rectangle 0, 0, $r[4], $r[5]), $rect, [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()
        $bmp.Save((Join-Path $destino $r[1]), [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    } finally { $img.Dispose() }

    $natural = [math]::Round($r[4] / 96 * 2.54, 1)                 # cm a tamaño real, letra de 9 pt
    $limite  = if ($r[1] -eq 'ficha-mapa.png') { $ANCHO_APAISADA } else { $ANCHO_PAGINA }
    $insertar = [math]::Min($natural, $limite)
    $letra = [math]::Round($LETRA_ORIGEN * $insertar / $natural, 1)
    if ($letra -lt $LETRA_MINIMA) { throw "$($r[1]): letra de $letra pt a $insertar cm." }
    '{0,-28} {1,6} {2,6} {3,9} {4,9} {5,7}  {6}' -f $r[1], $r[4], $r[5], $natural, $insertar, $letra, $r[6]
}
