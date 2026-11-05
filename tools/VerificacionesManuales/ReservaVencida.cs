// CX-02: la reserva vence por reloj, sin tocar fechas en la base. Abre los formularios reales
// de Visual contra BddEventAura_Pruebas (ver el .config que arma VerificarReservaVencida.ps1),
// con el manejador global instalado como en Program.Main. Arma 2 entradas de Campo de «Duki en
// Rosario», llega a la pantalla de pago, carga la tarjeta y NO paga: espera los minutos reales
// de RESERVA_MINUTOS. Captura el último minuto y el aviso de vencimiento. Responde solo
// ventanas de su propio hilo: no manda teclas al escritorio. C# 5.
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

static class ReservaVencida
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

    const int Duki = 1, Paula = 3;
    static string salida;
    static List<string> log = new List<string>();

    [STAThread]
    static int Main(string[] args)
    {
        salida = args[0];
        Directory.CreateDirectory(salida);
        Visual.ManejadorErrores.Instalar();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try { Correr(); }
        catch (Exception ex) { log.Add("ERROR: " + ex); }
        File.WriteAllLines(Path.Combine(salida, "reserva-vencida.txt"), log, new UTF8Encoding(false));
        return log.Any(l => l.StartsWith("ERROR")) ? 1 : 0;
    }

    static void Anotar(string texto) { log.Add(DateTime.Now.ToString("HH:mm:ss") + "  " + texto); }

    static void Correr()
    {
        var compra = new Visual.FrmCompraCliente { IDEvento = Duki, IDUsuario = Paula, NombreUsuario = "Paula" };
        compra.Show(); Esperar(800);
        int idCampo = 0;
        foreach (DataGridViewRow fila in Buscar<DataGridView>(compra, "dgvSectores").Rows)
            if (fila.Cells.Cast<DataGridViewCell>().Any(c => Convert.ToString(c.Value) == "Campo"))
            {
                fila.Cells["Cantidad"].Value = 2;
                idCampo = Convert.ToInt32(fila.Cells["IdSector"].Value);
            }
        Buscar<DataGridView>(compra, "dgvSectores").EndEdit();
        Esperar(400);
        int antes = Negocio.GestorPaquetes.Instance.DisponiblesDe(idCampo);
        Anotar("1. Paula arma 2 entradas de Campo de «Duki en Rosario». Disponibles en Campo: " + antes + ". Clic en «Comprar».");

        int etapa = 0;
        Visual.frmPago pago = null;
        DateTime abierto = DateTime.MinValue;
        var timer = new System.Windows.Forms.Timer { Interval = 500 };
        timer.Tick += (s, e) =>
        {
            if (etapa == 0 && Pulsar("Seguro de entrada", 6)) { etapa = 1; Anotar("2. «Seguro de entrada»: Sí, continuar sin seguro."); return; }
            pago = pago ?? Application.OpenForms.OfType<Visual.frmPago>().FirstOrDefault(f => f.Visible);
            if (etapa == 1 && pago != null)
            {
                etapa = 2; abierto = DateTime.Now;
                Buscar<TextBox>(pago, "txtTitular").Text = "Paula Plata";
                Buscar<TextBox>(pago, "txtNumero").Text = "4111 1111 1111 1111";
                Buscar<TextBox>(pago, "txtVencimiento").Text = "12/28";
                Buscar<TextBox>(pago, "txtCvv").Text = "123";
                int reservado = Negocio.GestorPaquetes.Instance.DisponiblesDe(idCampo);
                Anotar("3. Pantalla de pago: compra " + pago.Compra.Id + ", " + Buscar<Label>(pago, "lblTemporizador").Text +
                       ". Tarjeta cargada; NO se pulsa «Pagar». Disponibles en Campo: " + reservado + " (reservadas: " + (antes - reservado) + ").");
                return;
            }
            if (etapa == 2)
            {
                string reloj = Buscar<Label>(pago, "lblTemporizador").Text;
                if ((DateTime.Now - abierto).TotalSeconds % 60 < 0.5) Anotar("   " + reloj);
                if (reloj.StartsWith("Reserva: 00:4"))   // en el último minuto: rojo
                {
                    etapa = 3;
                    Anotar("4. Último minuto: «" + reloj + "», color " + Buscar<Label>(pago, "lblTemporizador").ForeColor.Name + ".");
                    Capturar(pago, IntPtr.Zero, "reserva-ultimo-minuto.png");
                }
                return;
            }
            if (etapa == 3)
            {
                IntPtr cuadro = CuadroVisible();
                if (cuadro == IntPtr.Zero) return;
                etapa = 4; timer.Stop();
                Anotar("5. A los " + (DateTime.Now - abierto).ToString(@"mm\:ss") + " de abrir el pago aparece un cuadro: «" + Texto(cuadro) + "».");
                foreach (var h in Hijos(cuadro))
                    if (h.Item1 == "Static" && h.Item2.Trim().Length > 0) Anotar("   Texto: «" + h.Item2 + "»");
                Capturar(pago, cuadro, "reserva-vencida.png");
                var aceptar = Hijos(cuadro).First(h => h.Item1 == "Button");
                PostMessage(cuadro, 0x0111, (IntPtr)GetDlgCtrlID(aceptar.Item3), aceptar.Item3);
            }
        };
        timer.Start();
        Buscar<Button>(compra, "btnComprar").PerformClick();   // vuelve cuando se cierra el pago
        timer.Stop();
        Esperar(800);
        if (etapa != 4) Anotar("ERROR: la verificación se detuvo en la etapa " + etapa);
        int despues = Negocio.GestorPaquetes.Instance.DisponiblesDe(idCampo);
        Anotar("6. Resultado de la pantalla de pago: " + pago.DialogResult + ". Ventanas visibles: " + string.Join(", ",
            Application.OpenForms.Cast<Form>().Where(f => f.Visible).Select(f => "«" + f.Text + "»")) + ".");
        Anotar("7. Disponibles en Campo: " + despues + (despues == antes ? " (el cupo volvió)." : " (FALTA CUPO: antes " + antes + ")."));
        if (despues != antes) Anotar("ERROR: el cupo no volvió.");
        log.Add("COMPRA=" + pago.Compra.Id);
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

    static bool Pulsar(string titulo, int id)
    {
        IntPtr cuadro = CuadroVisible();
        if (cuadro == IntPtr.Zero || Texto(cuadro) != titulo) return false;
        var b = Hijos(cuadro).FirstOrDefault(h => h.Item1 == "Button" && GetDlgCtrlID(h.Item3) == id);
        if (b == null) return false;
        PostMessage(cuadro, 0x0111, (IntPtr)id, b.Item3);
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

    // La pantalla de pago, con el aviso encima si lo hay; el resto, en blanco.
    static void Capturar(Form f, IntPtr cuadro, string nombre)
    {
        f.TopMost = true; SetForegroundWindow(f.Handle);
        if (cuadro != IntPtr.Zero) SetForegroundWindow(cuadro);
        Esperar(600);
        Rectangle a = Marco(f.Handle), d = cuadro != IntPtr.Zero ? Marco(cuadro) : a, zona = Rectangle.Union(a, d);
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
        Anotar("   Captura: " + nombre + " (" + zona.Width + " x " + zona.Height + ").");
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
