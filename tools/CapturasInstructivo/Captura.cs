// Captura las 4 figuras del instructivo (PDF pág. 52-53) abriendo los formularios reales de
// Visual contra BddEventAura_Pruebas (ver Captura.exe.config). Responde solo ventanas de su
// propio hilo: no manda teclas al escritorio. C# 5 (csc de .NET Framework).
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

static class Captura
{
    [DllImport("dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr h, int attr, out RECT r, int size);
    [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int valor, int size);
    [DllImport("user32.dll")] static extern bool EnumThreadWindows(uint id, EnumProc cb, IntPtr p);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] static extern IntPtr GetDlgItem(IntPtr h, int id);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
    delegate bool EnumProc(IntPtr h, IntPtr p);
    [StructLayout(LayoutKind.Sequential)] struct RECT { public int L, T, R, B; }

    static string salida;
    static List<string> log = new List<string>();

    [STAThread]
    static int Main(string[] args)
    {
        salida = args[0];
        Directory.CreateDirectory(salida);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try { if (args.Length > 2 && args[1] == "--escala") VerificarEscala(args[2]); else Correr(); }
        catch (Exception ex) { log.Add("ERROR: " + ex); }
        File.WriteAllLines(Path.Combine(salida, "captura-log.txt"), log);
        foreach (string l in log) Console.WriteLine(l);
        return log.Any(l => l.StartsWith("ERROR")) ? 1 : 0;
    }

    static void Correr()
    {
        const int Paula = 3, Duki = 1;   // ejemplo de la página 54 del documento

        // Figura 1 — listado de eventos (pasos 1 y 2), con «Duki en Rosario» seleccionado.
        var eventos = new Visual.frmEventosCliente { IDUsuario = Paula, NombreUsuario = "Paula" };
        eventos.Show(); Esperar(800);
        var grilla = Buscar<DataGridView>(eventos, "dgveventos");
        // Por ID y no por nombre: el evento 1 de la semilla es el del ejemplo del documento.
        foreach (DataGridViewRow fila in grilla.Rows)
            if (Convert.ToString(fila.Cells["ID"].Value) == Duki.ToString())
                grilla.CurrentCell = fila.Cells["Nombre"];
        Guardar(eventos, "figura1-listado-eventos.png");

        // Figuras 2 y 3 — armado del paquete del ejemplo: Plata, 2 Campo a $25.000, hospedaje.
        // La compra se muestra ANTES de ocultar el listado: si no, Windows le da el foco a
        // otra aplicación y la figura sale con la barra de título gris de ventana inactiva.
        var compra = new Visual.FrmCompraCliente { IDEvento = Duki, IDUsuario = Paula, NombreUsuario = "Paula" };
        compra.Show(); Esperar(300);
        eventos.TopMost = false; eventos.Hide();
        Esperar(500);
        var sectores = Buscar<DataGridView>(compra, "dgvSectores");
        foreach (DataGridViewRow fila in sectores.Rows)
            foreach (DataGridViewCell celda in fila.Cells)
                if (Convert.ToString(celda.Value) == "Campo") fila.Cells["Cantidad"].Value = 2;
        Buscar<CheckBox>(compra, "chkHospedaje").Checked = true;
        Esperar(500);
        string resumen = Buscar<Label>(compra, "lblResumen").Text;
        log.Add("Resumen:\n" + resumen);
        if (!resumen.Contains("99.625")) log.Add("ERROR: el resumen no da $ 99.625");
        Guardar(compra, "figura2-armado-paquete.png");
        var panel = Buscar<Panel>(compra, "pnlResumen");
        GuardarZona(compra, panel.RectangleToScreen(panel.ClientRectangle), "figura3-resumen-descuento.png");

        // Figura 4 — pago y confirmación (pasos 7 y 8). Un timer responde dentro de los modales.
        int etapa = 0;
        var timer = new System.Windows.Forms.Timer { Interval = 300 };
        timer.Tick += (s, e) =>
        {
            if (etapa == 0 && ResponderDialogo("Seguro de entrada", 6)) { etapa = 1; log.Add("Seguro: Sí, continuar sin seguro"); return; }
            var pago = Application.OpenForms.OfType<Visual.frmPago>().FirstOrDefault();
            if (etapa == 1 && pago != null && pago.Visible)
            {
                etapa = 2; timer.Stop();
                Buscar<TextBox>(pago, "txtTitular").Text = "Paula Plata";
                Buscar<TextBox>(pago, "txtNumero").Text = "4111 1111 1111 1111";
                Buscar<TextBox>(pago, "txtVencimiento").Text = "12/28";
                Buscar<TextBox>(pago, "txtCvv").Text = "123";
                Esperar(400);
                Guardar(pago, "figura4a-pago-simulado.png");
                timer.Start();
                Buscar<Button>(pago, "btnPagar").PerformClick();
                return;
            }
            var conf = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.GetType().Name == "frmConfirmacion" && f.Visible);
            if (etapa == 2 && conf != null)
            {
                etapa = 3; timer.Stop();
                // La carpeta real de tickets incluye el usuario de Windows: en la figura va una neutra.
                foreach (Control c in Todos(conf))
                    if (c is TextBox && c.Text.Contains("A1.Tickets EVENTAURA"))
                        c.Text = @"C:\Users\Cliente\Documents\A1.Tickets EVENTAURA";
                Esperar(600);
                Guardar(conf, "figura4b-confirmacion.png");
                foreach (Control c in Todos(conf)) if (c is Label && c.Text.Length > 0) log.Add("Confirmación: " + c.Text.Replace("\r", "").Replace("\n", " | "));
                conf.Close();
            }
        };
        timer.Start();
        Buscar<Button>(compra, "btnComprar").PerformClick();   // vuelve cuando se cierra la confirmación
        timer.Stop();
        if (etapa != 3) log.Add("ERROR: la compra se detuvo en la etapa " + etapa);
        compra.Close();
        eventos.Close();
    }

    // JC-R01: compra con el paquete máximo a la escala que tenga Windows en este momento.
    // La aplicación no declara compatibilidad con DPI: Windows la estira y le informa un
    // área útil reducida. Acá se mide contra esa área, que es la que ve la aplicación.
    static void VerificarEscala(string nombre)
    {
        var compra = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = 3, NombreUsuario = "Paula" };
        compra.Show(); Esperar(1000);
        Buscar<Label>(compra, "label1").Text =
            "Gran Festival Internacional de Música Independiente de Rosario con artistas nacionales e invitados VIP";
        foreach (DataGridViewRow fila in Buscar<DataGridView>(compra, "dgvSectores").Rows) fila.Cells["Cantidad"].Value = 2;
        foreach (string chk in new[] { "chkHospedaje", "chkTraslado", "chkMerch", "chkVIP", "chkSeguro" })
            Buscar<CheckBox>(compra, chk).Checked = true;
        Esperar(800);
        Rectangle area = Screen.FromControl(compra).WorkingArea;
        log.Add("Escala " + nombre + ": área útil vista por la aplicación " + area.Width + " x " + area.Height
            + "; ventana " + compra.Bounds.Width + " x " + compra.Bounds.Height + " en " + compra.Bounds.Location);
        bool dentro = area.Contains(compra.Bounds);
        log.Add((dentro ? "OK" : "FALLA") + "  ventana dentro del área útil");
        foreach (string b in new[] { "btnComprar", "btnVolver" })
        {
            var boton = Buscar<Button>(compra, b);
            Rectangle r = boton.RectangleToScreen(boton.ClientRectangle);
            bool ok = area.Contains(r) && boton.Visible;
            log.Add((ok ? "OK" : "FALLA") + "  " + b + " visible en " + r);
            if (!ok) dentro = false;
        }
        var contenido = Buscar<Panel>(compra, "pnlContenidoCompra");
        log.Add("Desplazamiento del contenido: " + (contenido.VerticalScroll.Visible ? "sí" : "no hace falta"));
        Guardar(compra, "escala-" + nombre + "-compra-paquete-maximo.png");
        var resumen = Buscar<Panel>(compra, "pnlResumen");
        contenido.ScrollControlIntoView(resumen); Esperar(400);
        Guardar(compra, "escala-" + nombre + "-compra-final-del-resumen.png");
        if (!dentro) log.Add("ERROR: algo queda fuera del área útil a escala " + nombre);
        compra.Close();
    }

    static bool ResponderDialogo(string titulo, int boton)
    {
        bool ok = false;
        EnumThreadWindows(GetCurrentThreadId(), (h, p) =>
        {
            var c = new StringBuilder(64); GetClassName(h, c, 64);
            var t = new StringBuilder(256); GetWindowText(h, t, 256);
            if (c.ToString() == "#32770" && IsWindowVisible(h) && t.ToString() == titulo)
            {
                IntPtr b = GetDlgItem(h, boton);
                if (b != IntPtr.Zero) { PostMessage(b, 0xF5, IntPtr.Zero, IntPtr.Zero); ok = true; }
            }
            return true;
        }, IntPtr.Zero);
        return ok;
    }

    static void Guardar(Form f, string nombre)
    {
        Traer(f);
        RECT r; DwmGetWindowAttribute(f.Handle, 9, out r, Marshal.SizeOf(typeof(RECT)));
        GuardarZona(f, new Rectangle(r.L, r.T, r.R - r.L, r.B - r.T), nombre);
    }

    static void GuardarZona(Form f, Rectangle zona, string nombre)
    {
        Traer(f);
        using (var bmp = new Bitmap(zona.Width, zona.Height))
        {
            using (var g = Graphics.FromImage(bmp)) g.CopyFromScreen(zona.Location, Point.Empty, zona.Size);
            bmp.Save(Path.Combine(salida, nombre), ImageFormat.Png);
        }
        log.Add(nombre + ": " + zona.Width + " x " + zona.Height);
    }

    static void Traer(Form f)
    {
        // Windows 11 redondea las esquinas y deja ver lo que hay detrás de la ventana: en una
        // figura del documento aparecían píxeles de otras aplicaciones. Esquinas rectas.
        int sinRedondear = 1;   // DWMWCP_DONOTROUND
        DwmSetWindowAttribute(f.Handle, 33, ref sinRedondear, sizeof(int));   // DWMWA_WINDOW_CORNER_PREFERENCE
        f.TopMost = true; f.Activate(); SetForegroundWindow(f.Handle);
        Esperar(500);
        if (Form.ActiveForm != f) log.Add("AVISO: " + f.Name + " no quedó como ventana activa al capturarla");
    }

    static void Esperar(int ms)
    {
        var fin = DateTime.Now.AddMilliseconds(ms);
        while (DateTime.Now < fin) { Application.DoEvents(); Thread.Sleep(20); }
    }

    static T Buscar<T>(Form f, string nombre) where T : Control
    {
        return (T)f.Controls.Find(nombre, true).Single();
    }

    static IEnumerable<Control> Todos(Control padre)
    {
        foreach (Control c in padre.Controls) { yield return c; foreach (Control h in Todos(c)) yield return h; }
    }
}
