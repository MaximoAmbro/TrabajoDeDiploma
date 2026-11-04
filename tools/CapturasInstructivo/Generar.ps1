# Regenera las figuras del instructivo (PDF, páginas 52 y 53) en docs/capturas/instructivo/.
# Abre los formularios reales contra BddEventAura_Pruebas, con su propia configuración: no
# toca los App.config ni la base de desarrollo, y no manda teclas al escritorio.
#
# Antes: compilar la solución y correr la batería una vez (tools/Proyecto.ps1 -Accion Probar),
# así la base de pruebas queda con la semilla. Va a abrir ventanas unos 15 segundos.
# El comprobante que genera la compra se mueve de Documentos\A1.Tickets EVENTAURA a la carpeta
# temporal, para no mezclarlo con tickets reales.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$tmp = Join-Path $env:TEMP 'EventAuraCapturas'
$bin = Join-Path $tmp 'bin'
$png = Join-Path $tmp 'png'
Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $bin, $png | Out-Null
Copy-Item (Join-Path $raiz 'Visual\bin\Debug\*') $bin -Recurse -Force

$cfg = [IO.File]::ReadAllText((Join-Path $bin 'Visual.exe.config'))
$cfg = $cfg.Replace('Initial Catalog=BddEventAura;', 'Initial Catalog=BddEventAura_Pruebas;')
if ($cfg -notmatch 'BddEventAura_Pruebas') { throw 'No se pudo apuntar la configuración a la base de pruebas.' }
[IO.File]::WriteAllText((Join-Path $bin 'Captura.exe.config'), $cfg)

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe `
    /out:"$bin\Captura.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll `
    /r:"$bin\Visual.exe" /r:"$bin\Entidades.dll" (Join-Path $PSScriptRoot 'Captura.cs')
if ($LASTEXITCODE -ne 0) { throw 'No compiló Captura.cs.' }

$p = Start-Process (Join-Path $bin 'Captura.exe') -ArgumentList "`"$png`"" -WorkingDirectory $bin -Wait -PassThru
Get-Content (Join-Path $png 'captura-log.txt') -Encoding UTF8
if ($p.ExitCode -ne 0) { throw 'La captura terminó con error.' }

$tickets = Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'A1.Tickets EVENTAURA'
$pedido = (Select-String -Path (Join-Path $png 'captura-log.txt') -Pattern 'Pedido (EA-\d+)').Matches[0].Groups[1].Value
Move-Item (Join-Path $tickets "Ticket_${pedido}_*.PDF") $png -Force

# Figura 4: confirmación a la izquierda y el primer ticket a la derecha, a la misma altura.
# El ticket se pasa a imagen con PyMuPDF (python -m pip install pymupdf).
$ticket = Get-ChildItem $png -Filter "Ticket_${pedido}_*_1.PDF" | Select-Object -First 1
python -c "import sys,pymupdf; pymupdf.open(sys.argv[1])[0].get_pixmap(matrix=pymupdf.Matrix(2,2),alpha=False).save(sys.argv[2])" $ticket.FullName (Join-Path $png 'ticket.png')
Add-Type -AssemblyName System.Drawing
$a = [Drawing.Image]::FromFile((Join-Path $png 'figura4b-confirmacion.png'))
$t = [Drawing.Image]::FromFile((Join-Path $png 'ticket.png'))
$tw = [int]($t.Width * $a.Height / $t.Height)
$bmp = New-Object Drawing.Bitmap ($a.Width + 24 + $tw), $a.Height
$g = [Drawing.Graphics]::FromImage($bmp)
$g.Clear([Drawing.Color]::White)
$g.InterpolationMode = 'HighQualityBicubic'
$g.DrawImage($a, 0, 0, $a.Width, $a.Height)
$g.DrawImage($t, $a.Width + 24, 0, $tw, $a.Height)
$bmp.Save((Join-Path $png 'figura4-confirmacion-y-comprobante.png'), [Drawing.Imaging.ImageFormat]::Png)
$g.Dispose(); $bmp.Dispose(); $a.Dispose(); $t.Dispose()

$destino = Join-Path $raiz 'docs\capturas\instructivo'
New-Item -ItemType Directory -Force $destino | Out-Null
foreach ($f in 'figura1-listado-eventos', 'figura2-armado-paquete', 'figura3-resumen-descuento',
               'figura4-confirmacion-y-comprobante', 'figura4a-pago-simulado') {
    Copy-Item (Join-Path $png "$f.png") $destino -Force
}
Write-Output "Figuras copiadas a $destino"
