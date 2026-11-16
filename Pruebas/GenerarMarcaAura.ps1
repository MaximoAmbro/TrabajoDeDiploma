# Regenera el icono multirresolución desde el trazado GDI+, sin imágenes externas.
param([string]$Raiz = (Split-Path $PSScriptRoot -Parent))
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
[void][Reflection.Assembly]::LoadFrom((Join-Path $Raiz 'Visual/bin/Debug/Visual.exe'))
$imagenes = @()
foreach ($lado in @(16,32,48,64,256)) {
    $bitmap = New-Object Drawing.Bitmap($lado,$lado)
    $g = [Drawing.Graphics]::FromImage($bitmap)
    $camino = New-Object Drawing.Drawing2D.GraphicsPath
    $pincel = New-Object Drawing.SolidBrush([Visual.Tema]::Primario)
    $memoria = New-Object IO.MemoryStream
    try {
        $g.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $radio = [single]($lado * 0.2)
        $limite = [single]($lado - 1)
        $camino.AddArc([single]0,[single]0,$radio,$radio,[single]180,[single]90)
        $camino.AddArc(($limite-$radio),[single]0,$radio,$radio,[single]270,[single]90)
        $camino.AddArc(($limite-$radio),($limite-$radio),$radio,$radio,[single]0,[single]90)
        $camino.AddArc([single]0,($limite-$radio),$radio,$radio,[single]90,[single]90)
        $camino.CloseFigure()
        $g.FillPath($pincel,$camino)
        [Visual.Componentes.MarcaAura]::Dibujar($g,[Drawing.RectangleF]::new(0,0,$lado,$lado),[Visual.Tema]::Superficie,($lado -ne 16))
        $bitmap.Save($memoria,[Drawing.Imaging.ImageFormat]::Png)
        $imagenes += ,@{Lado=$lado;Bytes=$memoria.ToArray()}
    } finally {$memoria.Dispose();$pincel.Dispose();$camino.Dispose();$g.Dispose();$bitmap.Dispose()}
}
$destino = Join-Path $Raiz 'Visual/Recursos/EventAura.ico'
$escritor = New-Object IO.BinaryWriter([IO.File]::Create($destino))
try {
    $escritor.Write([uint16]0);$escritor.Write([uint16]1);$escritor.Write([uint16]$imagenes.Count)
    $offset=6+16*$imagenes.Count
    foreach($imagen in $imagenes) {
        $dimension=if($imagen.Lado -eq 256){0}else{$imagen.Lado}
        $escritor.Write([byte]$dimension);$escritor.Write([byte]$dimension)
        $escritor.Write([byte]0);$escritor.Write([byte]0)
        $escritor.Write([uint16]1);$escritor.Write([uint16]32)
        $escritor.Write([uint32]$imagen.Bytes.Length);$escritor.Write([uint32]$offset)
        $offset += $imagen.Bytes.Length
    }
    foreach($imagen in $imagenes){$escritor.Write([byte[]]$imagen.Bytes)}
} finally {$escritor.Dispose()}
Write-Output 'Icono Aura: 16, 32, 48, 64 y 256; sin travesaño en 16.'
