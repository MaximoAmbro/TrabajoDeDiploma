# Cambia la escala de Windows del monitor principal como lo hace Configuración > Pantalla
# (DisplayConfigSetDeviceInfo con DISPLAYCONFIG_DEVICE_INFO_SET_DPI_SCALE). Se carga con
# «. .\Escala.ps1». Leer-Escala devuelve el porcentaje actual y el recomendado;
# Fijar-Escala cambia al porcentaje pedido. Para revertir, Fijar-Escala con el de partida.
# Nota: SPI_SETLOGICALDPIOVERRIDE, el otro método conocido, en esta versión de Windows solo
# escribía el registro sin aplicar el cambio; no usarlo.
if (-not ('EscalaWin' -as [type])) {
Add-Type @'
using System; using System.Runtime.InteropServices;
public static class EscalaWin {
    [DllImport("user32.dll")] static extern int GetDisplayConfigBufferSizes(uint f, out uint p, out uint m);
    [DllImport("user32.dll")] static extern int QueryDisplayConfig(uint f, ref uint np, byte[] p, ref uint nm, byte[] m, IntPtr t);
    [DllImport("user32.dll")] static extern int DisplayConfigGetDeviceInfo(byte[] r);
    [DllImport("user32.dll")] static extern int DisplayConfigSetDeviceInfo(byte[] r);
    [DllImport("user32.dll")] public static extern bool SetProcessDpiAwarenessContext(IntPtr v);
    [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern IntPtr GetDesktopWindow();
    public static readonly int[] Valores = { 100, 125, 150, 175, 200, 225, 250, 300, 350, 400, 450, 500 };
    static byte[] Origen() {
        uint np, nm; GetDisplayConfigBufferSizes(2, out np, out nm);
        var p = new byte[np * 72]; var m = new byte[nm * 64];
        if (QueryDisplayConfig(2, ref np, p, ref nm, m, IntPtr.Zero) != 0) throw new Exception("QueryDisplayConfig");
        var o = new byte[12]; Array.Copy(p, 0, o, 0, 12); return o;   // LUID del adaptador e id de origen
    }
    // Devuelve { mínimo, actual, máximo } relativos al recomendado.
    public static int[] Leer() {
        var r = new byte[32]; BitConverter.GetBytes(-3).CopyTo(r, 0); BitConverter.GetBytes(32).CopyTo(r, 4); Origen().CopyTo(r, 8);
        if (DisplayConfigGetDeviceInfo(r) != 0) throw new Exception("DisplayConfigGetDeviceInfo");
        return new[] { BitConverter.ToInt32(r, 20), BitConverter.ToInt32(r, 24), BitConverter.ToInt32(r, 28) };
    }
    public static int Fijar(int relativo) {
        var r = new byte[24]; BitConverter.GetBytes(-4).CopyTo(r, 0); BitConverter.GetBytes(24).CopyTo(r, 4); Origen().CopyTo(r, 8);
        BitConverter.GetBytes(relativo).CopyTo(r, 20);
        return DisplayConfigSetDeviceInfo(r);
    }
}
'@
}
[EscalaWin]::SetProcessDpiAwarenessContext([IntPtr](-4)) | Out-Null

function Leer-Escala {
    $v = [EscalaWin]::Leer(); $rec = [Math]::Abs($v[0])
    [pscustomobject]@{
        Actual = [EscalaWin]::Valores[$rec + $v[1]]; Recomendada = [EscalaWin]::Valores[$rec]
        DpiEscritorio = [EscalaWin]::GetDpiForWindow([EscalaWin]::GetDesktopWindow())
    }
}

function Fijar-Escala([int]$porcentaje) {
    $v = [EscalaWin]::Leer(); $rec = [Math]::Abs($v[0])
    $i = [Array]::IndexOf([EscalaWin]::Valores, $porcentaje)
    if ($i -lt 0 -or ($i - $rec) -gt $v[2]) { throw "Escala $porcentaje % no disponible en este monitor." }
    $r = [EscalaWin]::Fijar($i - $rec); if ($r -ne 0) { throw "DisplayConfigSetDeviceInfo devolvió $r." }
    Start-Sleep -Seconds 3
}
