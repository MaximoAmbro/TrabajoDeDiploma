# Recorta el detalle legible de las pantallas del anexo «Pantallas del sistema»: las cinco vistas
# de «Mi cuenta» (semilla de Paula, 18/11/2026) y las de Organizador y Administrador que Codex
# recapturó con la semilla el 19/11/2026 (`037c6b4`). No toca las capturas de Codex: lee de Pruebas/
# y escribe en docs/capturas/pantallas/. Informa el ancho de inserción y la letra impresa de cada
# recorte, y falla si alguno queda por debajo de 8 pt.
#
# Uso: powershell -NoProfile -ExecutionPolicy Bypass -File tools\Pantallas\Recortar.ps1

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$raiz    = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$origen  = Join-Path $raiz 'Pruebas\Recortes-Documento-Fases7-8\completas'
$destino = Join-Path $raiz 'docs\capturas\pantallas'
New-Item -ItemType Directory -Force $destino | Out-Null

# La letra más chica de las capturas es de 9 pt a 96 ppp (medición de Codex en ancho-minimo-8_1pt.tsv).
$LETRA_ORIGEN = 9.0
$LETRA_MINIMA = 8.0   # regla del documento para capturas
$ANCHO_PAGINA = 16.0  # cm, página vertical
$ANCHO_APAISADA = 24.7

# carpeta\archivo origen (dentro de completas), archivo destino, x, y, ancho, alto, contenido
$recortes = @(
    @('MiCuenta-2026-11-18\eventos.png', 'eventos-tarjetas.png', 320, 225, 576, 372, 'Tarjetas de los eventos'),
    @('MiCuenta-2026-11-18\ficha-evento.png', 'ficha-portada-datos.png', 312, 95, 648, 200, 'Portada y datos del evento'),
    @('MiCuenta-2026-11-18\ficha-evento.png', 'ficha-mapa.png', 315, 322, 863, 280, 'Mapa de sectores'),
    @('MiCuenta-2026-11-18\compra.png', 'compra-entradas.png', 322, 214, 396, 442, 'Entradas y cantidades'),
    @('MiCuenta-2026-11-18\compra.png', 'compra-resumen.png', 739, 214, 382, 366, 'Resumen y total'),
    @('MiCuenta-2026-11-18\mis-entradas.png', 'mis-entradas-izquierda.png', 322, 222, 484, 224, 'Pedido, evento, fecha y estado'),
    @('MiCuenta-2026-11-18\mis-entradas.png', 'mis-entradas-derecha.png', 806, 222, 372, 224, 'Entradas, total pagado, devolución, seguro y código'),
    @('MiCuenta-2026-11-18\suscripcion.png', 'suscripcion-panel.png', 318, 168, 404, 404, 'Plan y beneficios'),
    @('Fase8\frmMenuPropietario.png', 'organizador-menu.png', 24, 110, 290, 365, 'Operaciones del Organizador'),
    @('Fase8\frmMenuPropietario.png', 'organizador-bienvenida.png', 340, 135, 530, 75, 'Panel de bienvenida'),
    @('Fase8\frmMisLocales.png', 'locales-nombre-direccion.png', 30, 176, 530, 48, 'Nombre y dirección de los locales'),
    @('Fase8\frmMisLocales.png', 'locales-capacidad.png', 556, 176, 272, 48, 'Capacidad de los locales'),
    @('Fase8\frmEventosLocal.png', 'eventos-local-tabla.png', 28, 115, 522, 235, 'Eventos del local con su estado y el motivo del rechazo'),
    @('Fase8\frmAdministrarEvento.png', 'administrar-preventa-sectores.png', 28, 150, 360, 292, 'Preventa y sectores'),
    @('Fase8\frmAdministrarEvento.png', 'administrar-portada.png', 400, 150, 430, 140, 'Guardar preventa, portada y categoría'),
    @('Fase8\frmControlIngreso.png', 'control-ingreso.png', 30, 115, 600, 255, 'Control de ingreso sin eventos abiertos'),
    @('Fase8\frmListasEspera.png', 'listas-espera.png', 25, 160, 600, 245, 'Evento elegido y estado de su lista'),
    @('Fase8\frmMenuAdministrador.png', 'administrador-operaciones-1.png', 30, 160, 420, 75, 'Operaciones, primera parte'),
    @('Fase8\frmMenuAdministrador.png', 'administrador-operaciones-2.png', 462, 160, 526, 118, 'Operaciones, segunda parte'),
    @('Fase8\frmMenuAdministrador.png', 'administrador-indicadores-1.png', 36, 280, 460, 340, 'Indicadores, columna izquierda'),
    @('Fase8\frmMenuAdministrador.png', 'administrador-indicadores-2.png', 503, 280, 460, 225, 'Indicadores, columna derecha'),
    @('Fase8\frmReportes.png', 'reportes-filtros-1.png', 25, 155, 445, 125, 'Reporte y categoría'),
    @('Fase8\frmReportes.png', 'reportes-filtros-2.png', 485, 155, 400, 150, 'Período y generación'),
    @('Fase8\frmReportes.png', 'reportes-tabla-1.png', 25, 315, 430, 90, 'Tabla del reporte, primeras columnas'),
    @('Fase8\frmReportes.png', 'reportes-lectura.png', 25, 452, 455, 26, 'Lectura del reporte'),
    @('Fase8\frmReportes.png', 'reportes-tabla-2.png', 450, 315, 432, 90, 'Tabla del reporte, últimas columnas'),
    @('Fase8\frmRevisarSolicitudesOrganizador.png', 'solicitudes-datos.png', 26, 98, 610, 150, 'Lista y datos de la solicitud'),
    @('Fase8\frmRevisarSolicitudesOrganizador.png', 'solicitudes-documento.png', 795, 98, 366, 266, 'Documentación adjunta'),
    @('Fase8\frmRevisarSolicitudesOrganizador.png', 'solicitudes-acciones.png', 345, 405, 600, 195, 'Motivo y acciones'),
    @('Fase8\frmRevisarEventos.png', 'revision-eventos.png', 672, 110, 362, 335, 'Estado, motivo del rechazo y acciones'),
    @('Fase8\frmAuditoria.png', 'auditoria-1.png', 18, 38, 580, 195, 'Filtros y primeras columnas'),
    @('Fase8\frmAuditoria.png', 'auditoria-2.png', 390, 38, 524, 195, 'Filtro de acción y últimas columnas'),
    @('Fase8\frmSolicitudOrganizador.png', 'solicitud-responsable.png', 28, 105, 410, 135, 'Responsable y teléfono'),
    @('Fase8\frmSolicitudOrganizador.png', 'solicitud-codigo.png', 458, 105, 412, 180, 'CUIT y confirmación del teléfono'),
    @('Fase8\frmSolicitudOrganizador.png', 'solicitud-local-1.png', 28, 325, 410, 135, 'Nombre y capacidad del local'),
    @('Fase8\frmSolicitudOrganizador.png', 'solicitud-local-2.png', 458, 325, 412, 135, 'Dirección y CBU'),
    @('Fase8\frmSolicitudOrganizador.png', 'solicitud-envio.png', 28, 465, 600, 190, 'Documentación y envío')
)

'{0,-36} {1,6} {2,6} {3,9} {4,9} {5,7}  {6}' -f 'recorte','px','alto','natural','insertar','letra','contenido'
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
    '{0,-36} {1,6} {2,6} {3,9} {4,9} {5,7}  {6}' -f $r[1], $r[4], $r[5], $natural, $insertar, $letra, $r[6]
}
