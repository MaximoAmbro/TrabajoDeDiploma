# Regeneración del recurso de T-34. Ejecutar después de compilar Visual (Tema.cs).
param([string]$Raiz = (Split-Path $PSScriptRoot -Parent))
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
[void][Reflection.Assembly]::LoadFrom((Join-Path $Raiz 'Visual\bin\Debug\Visual.exe'))
$destino = Join-Path $Raiz 'Visual\Recursos\EventAura.ico'
[void][IO.Directory]::CreateDirectory((Split-Path $destino -Parent))
$imagenes = @()
foreach ($lado in @(16, 24, 32, 48, 256)) {
    $bitmap = New-Object Drawing.Bitmap($lado, $lado)
    $grafico = [Drawing.Graphics]::FromImage($bitmap)
    $fuente = New-Object Drawing.Font('Segoe UI Semibold', ($lado * 0.72), [Drawing.FontStyle]::Regular, [Drawing.GraphicsUnit]::Pixel)
    $pincel = New-Object Drawing.SolidBrush([Visual.Tema]::Superficie)
    $formato = New-Object Drawing.StringFormat
    $memoria = New-Object IO.MemoryStream
    try {
        $grafico.Clear([Visual.Tema]::Primario)
        $grafico.TextRenderingHint = [Drawing.Text.TextRenderingHint]::AntiAliasGridFit
        $formato.Alignment = [Drawing.StringAlignment]::Center
        $formato.LineAlignment = [Drawing.StringAlignment]::Center
        $grafico.DrawString('E', $fuente, $pincel, [Drawing.RectangleF]::new(0, 0, $lado, $lado), $formato)
        $bitmap.Save($memoria, [Drawing.Imaging.ImageFormat]::Png)
        $imagenes += ,@{ Lado = $lado; Bytes = $memoria.ToArray() }
    } finally {
        $memoria.Dispose(); $formato.Dispose(); $pincel.Dispose()
        $fuente.Dispose(); $grafico.Dispose(); $bitmap.Dispose()
    }
}
$archivo = [IO.File]::Create($destino)
$escritor = New-Object IO.BinaryWriter($archivo)
try {
    $escritor.Write([uint16]0); $escritor.Write([uint16]1); $escritor.Write([uint16]$imagenes.Count)
    $desplazamiento = 6 + 16 * $imagenes.Count
    foreach ($imagen in $imagenes) {
        $dimension = if ($imagen.Lado -eq 256) { 0 } else { $imagen.Lado }
        $escritor.Write([byte]$dimension); $escritor.Write([byte]$dimension)
        $escritor.Write([byte]0); $escritor.Write([byte]0)
        $escritor.Write([uint16]1); $escritor.Write([uint16]32)
        $escritor.Write([uint32]$imagen.Bytes.Length); $escritor.Write([uint32]$desplazamiento)
        $desplazamiento += $imagen.Bytes.Length
    }
    foreach ($imagen in $imagenes) { $escritor.Write([byte[]]$imagen.Bytes) }
} finally { $escritor.Dispose(); $archivo.Dispose() }
Write-Output "Generado: $destino (16, 24, 32, 48, 256; imágenes PNG)"
