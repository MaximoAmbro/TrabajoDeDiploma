// ¿Qué ve el cliente si el propietario elimina el evento mientras él está pagando? Abre los
// formularios reales de Visual contra BddEventAura_Pruebas (ver el .config que arma
// VerificarEventoEliminado.ps1), con el manejador global instalado como en Program.Main.
// Arma el paquete del ejemplo del documento y, con la pantalla de pago abierta, lanza OTRO
// proceso (este mismo programa con --eliminar) que llama a GestorEventos.Eliminar, lo mismo
// que hace el botón «Eliminar» del propietario (frmEventosLocal). Después pulsa «Pagar».
// Responde solo ventanas de su propio hilo: no manda teclas al escritorio. C# 5.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

static class EventoEliminadoEnPago
{
    [DllImport("dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr h, int attr, out RECT r, int size);
    [DllImport("user32.dll")] static extern bool EnumThreadWindows(uint id, EnumProc cb, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr h, EnumProc cb, IntPtr p);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] static extern int GetDlgCtrlID(IntPtr h);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
    delegate bool EnumProc(IntPtr h, IntPtr p);
    [StructLayout(LayoutKind.Sequential)] struct RECT { public int L, T, R, B; }

    const int Duki = 1, Paula = 3;   // ejemplo de la página 54 del documento
    static string salida;
    static List<string> log = new List<string>();

    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "--eliminar") return Eliminar(int.Parse(args[1]));

        salida = args[0];
        Directory.CreateDirectory(salida);
        Visual.ManejadorErrores.Instalar();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try { Correr(); }
        catch (Exception ex) { log.Add("ERROR: " + ex); }
        File.WriteAllLines(Path.Combine(salida, "evento-eliminado-en-pago.txt"), log, new UTF8Encoding(false));
        foreach (string l in log) Console.WriteLine(l);
        return log.Any(l => l.StartsWith("ERROR")) ? 1 : 0;
    }

    // El «propietario»: otro proceso, otra conexión, el mismo método que el botón Eliminar.
    static int Eliminar(int idEvento)
    {
        try
        {
            Negocio.GestorEventos.Instance.Eliminar(idEvento);
            Console.WriteLine("Evento " + idEvento + " eliminado.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
            return 1;
        }
    }

    static void Correr()
    {
        var compra = new Visual.FrmCompraCliente { IDEvento = Duki, IDUsuario = Paula, NombreUsuario = "Paula" };
        compra.Show(); Esperar(800);
        foreach (DataGridViewRow fila in Buscar<DataGridView>(compra, "dgvSectores").Rows)
            if (fila.Cells.Cast<DataGridViewCell>().Any(c => Convert.ToString(c.Value) == "Campo")) fila.Cells["Cantidad"].Value = 2;
        Buscar<DataGridView>(compra, "dgvSectores").EndEdit();
        Esperar(400);
        log.Add("1. Cliente Paula arma el paquete de «Duki en Rosario»: 2 entradas de Campo. Clic en «Comprar».");

        int etapa = 0;
        Visual.frmPago pago = null;
        var timer = new System.Windows.Forms.Timer { Interval = 300 };
        timer.Tick += (s, e) =>
        {
            if (etapa == 0 && Pulsar("Seguro de entrada", 6)) { etapa = 1; log.Add("2. «Seguro de entrada»: Sí, continuar sin seguro."); return; }
            pago = pago ?? Application.OpenForms.OfType<Visual.frmPago>().FirstOrDefault(f => f.Visible);
            if (etapa == 1 && pago != null)
            {
                etapa = 2; timer.Stop();
                log.Add("3. Pantalla de pago abierta. Compra " + pago.Compra.Id + ", pendiente, con el cupo reservado.");
                Buscar<TextBox>(pago, "txtTitular").Text = "Paula Plata";
                Buscar<TextBox>(pago, "txtNumero").Text = "4111 1111 1111 1111";
                Buscar<TextBox>(pago, "txtVencimiento").Text = "12/28";
                Buscar<TextBox>(pago, "txtCvv").Text = "123";
                Esperar(300);

                var propietario = new ProcessStartInfo(Application.ExecutablePath, "--eliminar " + Duki)
                { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true };
                using (var p = Process.Start(propietario))
                {
                    string respuesta = p.StandardOutput.ReadToEnd().Trim();
                    p.WaitForExit();
                    log.Add("4. Otro proceso (el propietario) llama a GestorEventos.Eliminar(" + Duki + "): «" + respuesta + "».");
                    if (p.ExitCode != 0) log.Add("ERROR: el propietario no pudo eliminar el evento.");
                }
                log.Add("5. El cliente, que no se enteró, pulsa «Pagar».");
                timer.Start();
                Buscar<Button>(pago, "btnPagar").PerformClick();   // vuelve cuando se cierra el aviso
                return;
            }
            if (etapa == 2)
            {
                IntPtr cuadro = CuadroVisible();
                if (cuadro == IntPtr.Zero) return;
                etapa = 3; timer.Stop();
                string titulo = Texto(cuadro);
                log.Add("6. Aparece un cuadro: título «" + titulo + "».");
                foreach (var h in Hijos(cuadro))
                    if (h.Item1 == "Static" && Texto(h.Item3).Trim().Length > 0)
                        log.Add("   Texto: «" + Texto(h.Item3).Replace("\r", "").Replace("\n", " ⏎ ") + "»");
                bool esNet = titulo == "Microsoft .NET Framework";
                log.Add("   Cuadro de excepción de .NET: " + (esNet ? "SÍ (falla)" : "no"));
                Capturar(pago, cuadro, "evento-eliminado-en-pago.png");
                var aceptar = Hijos(cuadro).FirstOrDefault(h => h.Item1 == "Button");
                PostMessage(cuadro, 0x0111, (IntPtr)GetDlgCtrlID(aceptar.Item3), aceptar.Item3);   // WM_COMMAND
                if (esNet) log.Add("ERROR: el usuario ve el cuadro de .NET.");
            }
        };
        timer.Start();
        Buscar<Button>(compra, "btnComprar").PerformClick();   // vuelve cuando se cierra el pago
        timer.Stop();
        Esperar(800);
        if (etapa != 3) log.Add("ERROR: la prueba se detuvo en la etapa " + etapa);
        log.Add("7. Resultado de la pantalla de pago: " + (pago != null ? pago.DialogResult.ToString() : "—") +
                ". Ventanas visibles después: " + string.Join(", ",
                    Application.OpenForms.Cast<Form>().Where(f => f.Visible).Select(f => "«" + f.Text + "»")) + ".");
        log.Add("COMPRA=" + (pago != null ? pago.Compra.Id : 0));
        // Como Application.Exit al cerrar la última ventana: sin la confirmación de «Cerrar la compra»,
        // que solo aparece cuando el usuario cierra esa pantalla con el paquete armado.
        foreach (Form f in Application.OpenForms.Cast<Form>().ToList()) f.Hide();
    }

    static IntPtr CuadroVisible()
    {
        IntPtr encontrado = IntPtr.Zero;
        EnumThreadWindows(GetCurrentThreadId(), (h, p) =>
        {
            var c = new StringBuilder(64); GetClassName(h, c, 64);
            if (IsWindowVisible(h) && (c.ToString() == "#32770" || Texto(h) == "Microsoft .NET Framework")) encontrado = h;
            return true;
        }, IntPtr.Zero);
        return encontrado;
    }

    // Pulsa el botón con ese ID (6 = Sí) en el cuadro de ese título.
    static bool Pulsar(string titulo, int id)
    {
        IntPtr cuadro = CuadroVisible();
        if (cuadro == IntPtr.Zero || Texto(cuadro) != titulo) return false;
        var b = Hijos(cuadro).FirstOrDefault(h => h.Item1 == "Button" && GetDlgCtrlID(h.Item3) == id);
        if (b == null) return false;
        PostMessage(cuadro, 0x0111, (IntPtr)GetDlgCtrlID(b.Item3), b.Item3);
        return true;
    }

    static List<Tuple<string, string, IntPtr>> Hijos(IntPtr h)
    {
        var lista = new List<Tuple<string, string, IntPtr>>();
        EnumChildWindows(h, (c, p) =>
        {
            var k = new StringBuilder(64); GetClassName(c, k, 64);
            lista.Add(Tuple.Create(k.ToString(), Texto(c), c));
            return true;
        }, IntPtr.Zero);
        return lista;
    }

    static string Texto(IntPtr h)
    {
        var t = new StringBuilder(1024); GetWindowText(h, t, 1024); return t.ToString();
    }

    static Rectangle Marco(IntPtr h)
    {
        RECT r; DwmGetWindowAttribute(h, 9, out r, Marshal.SizeOf(typeof(RECT)));
        return Rectangle.FromLTRB(r.L, r.T, r.R, r.B);
    }

    // La pantalla de pago con el aviso encima; lo que no es de ninguna de las dos, en blanco.
    static void Capturar(Form f, IntPtr cuadro, string nombre)
    {
        f.TopMost = true; SetForegroundWindow(f.Handle); SetForegroundWindow(cuadro);
        Esperar(600);
        Rectangle a = Marco(f.Handle), d = Marco(cuadro), zona = Rectangle.Union(a, d);
        using (var bmp = new Bitmap(zona.Width, zona.Height))
        {
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(zona.Location, Point.Empty, zona.Size);
                var fondo = new Region(new Rectangle(Point.Empty, zona.Size));
                a.Offset(-zona.X, -zona.Y); d.Offset(-zona.X, -zona.Y);
                fondo.Exclude(a); fondo.Exclude(d);
                g.FillRegion(Brushes.White, fondo);
            }
            bmp.Save(Path.Combine(salida, nombre), ImageFormat.Png);
        }
        log.Add("   Captura: docs/excepciones/" + nombre + " (" + zona.Width + " x " + zona.Height + ").");
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
}
