// Verificación sobre el ejecutable (Claude Code, 26/11/2026). Abre los formularios reales de Visual
// desde una copia de bin\Debug, contra BddEventAura_Pruebas, y registra qué se ve: vistas web,
// avisos, cuadros de mensaje, errores.log y una captura de cada ventana. No manda teclas al
// escritorio: opera los controles de su propio proceso.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using Visual.Web;

static class Verificar
{
    [DllImport("dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr h, int attr, out RECT r, int size);
    [DllImport("user32.dll")] static extern bool EnumThreadWindows(uint id, EnumProc cb, IntPtr p);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] static extern IntPtr GetDlgItem(IntPtr h, int id);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
    delegate bool EnumProc(IntPtr h, IntPtr p);
    [StructLayout(LayoutKind.Sequential)] struct RECT { public int L, T, R, B; }

    static string salida;
    static Form raiz;
    static uint hiloUi;
    static StreamWriter log;
    static string errores = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EventAura", "errores.log");
    static int n;

    [STAThread]
    static int Main(string[] args)
    {
        string modo = args[0];
        salida = args[1];
        Directory.CreateDirectory(salida);
        log = new StreamWriter(Path.Combine(salida, "log.txt"), false, new UTF8Encoding(false)) { AutoFlush = true };
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Visual.ManejadorErrores.Instalar();
        try
        {
            var sel = typeof(Visual.EnviadorSmtp).GetMethod("Seleccionar");
            SeleccionEnviadorCodigos.Actual = (IEnviadorCodigos)sel.Invoke(null, null);
        }
        catch (Exception ex) { L("AVISO: no se pudo seleccionar el enviador: " + ex.Message); }
        raiz = new Form { ShowInTaskbar = false, Opacity = 0, Size = new Size(1, 1), StartPosition = FormStartPosition.Manual, Location = new Point(-2000, -2000) };
        raiz.Load += (s, e) =>
        {
            hiloUi = GetCurrentThreadId();
            raiz.BeginInvoke((Action)(() => raiz.Hide()));
            var t = new Thread(() =>
            {
                try
                {
                    L("Modo " + modo + " · " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " · carpeta " + AppDomain.CurrentDomain.BaseDirectory);
                    L("DLL presentes: " + string.Join(", ", new[] { "Microsoft.Web.WebView2.Core.dll", "Microsoft.Web.WebView2.WinForms.dll", "Newtonsoft.Json.dll", @"runtimes\win-x64\native\WebView2Loader.dll", "itext.kernel.dll" }
                        .Select(f => f + "=" + (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, f)) ? "sí" : "NO"))));
                    if (modo == "pantallas") Pantallas();
                    else if (modo == "t23") T23();
                    else if (modo == "t26") T26();
                    else if (modo == "t26b") T26b();
                    else if (modo == "t22") T22();
                    else if (modo == "t24") T24();
                    else if (modo == "t25") T25();
                }
                catch (Exception ex) { L("ERROR DEL VERIFICADOR: " + ex); }
                finally { L("Fin."); raiz.BeginInvoke((Action)(() => Application.Exit())); }
            }) { IsBackground = true };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        };
        Application.Run(raiz);
        return 0;
    }

    // ---------- utilidades ----------
    static void L(string s) { lock (log) log.WriteLine(s); Console.WriteLine(s); }
    static void Hacer(Action a) { raiz.BeginInvoke(a); }
    static T Pedir<T>(Func<T> f) { return (T)raiz.Invoke(f); }
    static void Pedir(Action a) { raiz.Invoke(a); }
    static void Esperar(int ms) { Thread.Sleep(ms); }
    static long TamLog() { return File.Exists(errores) ? new FileInfo(errores).Length : 0; }
    static string LogDesde(long desde)
    {
        if (!File.Exists(errores)) return "";
        using (var fs = new FileStream(errores, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            if (fs.Length <= desde) return "";
            fs.Seek(desde, SeekOrigin.Begin);
            return new StreamReader(fs).ReadToEnd();
        }
    }

    static IEnumerable<Control> Todos(Control c)
    {
        foreach (Control h in c.Controls) { yield return h; foreach (var x in Todos(h)) yield return x; }
    }
    static T Buscar<T>(Control c, string nombre) where T : Control
    {
        return Todos(c).OfType<T>().FirstOrDefault(x => x.Name == nombre);
    }
    static List<Form> Abiertos()
    {
        return Pedir(() => Application.OpenForms.Cast<Form>().Where(f => f != raiz && f.Visible && !f.IsDisposed).ToList());
    }
    static void CerrarTodo()
    {
        for (int i = 0; i < 5; i++)
        {
            CerrarCuadros(false);
            var abiertos = Pedir(() => Application.OpenForms.Cast<Form>().Where(f => f != raiz).ToList());
            if (abiertos.Count == 0) return;
            foreach (var f in abiertos.AsEnumerable().Reverse())
                Hacer(() => { try { if (!f.IsDisposed) { if (f.Modal) f.DialogResult = DialogResult.Cancel; f.Close(); } } catch { } });
            Esperar(700);
        }
    }

    // Cuadros de mensaje (#32770) abiertos por el hilo de la interfaz: se anotan y se cierran.
    static List<string> CerrarCuadros(bool anotar = true)
    {
        var textos = new List<string>();
        var hs = new List<IntPtr>();
        EnumThreadWindows(hiloUi, (h, p) =>
        {
            var cls = new StringBuilder(64); GetClassName(h, cls, 64);
            if (cls.ToString() == "#32770" && IsWindowVisible(h)) hs.Add(h);
            return true;
        }, IntPtr.Zero);
        foreach (var h in hs)
        {
            var t = new StringBuilder(256); GetWindowText(h, t, 256);
            var st = GetDlgItem(h, 0xFFFF);
            var c = new StringBuilder(2048); if (st != IntPtr.Zero) GetWindowText(st, c, 2048);
            string texto = "[" + t + "] " + c.ToString().Replace("\r\n", " ").Replace("\n", " ");
            textos.Add(texto);
            if (anotar) L("  CUADRO DE MENSAJE: " + texto);
            Captura(h, "cuadro");
            PostMessage(h, 0x0010, IntPtr.Zero, IntPtr.Zero);
        }
        return textos;
    }

    static void Captura(IntPtr h, string nombre)
    {
        RECT r;
        if (DwmGetWindowAttribute(h, 9, out r, Marshal.SizeOf(typeof(RECT))) != 0) GetWindowRect(h, out r);
        int w = r.R - r.L, al = r.B - r.T;
        if (w <= 0 || al <= 0) return;
        using (var bmp = new Bitmap(w, al))
        {
            using (var g = Graphics.FromImage(bmp)) g.CopyFromScreen(r.L, r.T, 0, 0, new Size(w, al));
            string archivo = Path.Combine(salida, (++n).ToString("00") + "-" + nombre + ".png");
            bmp.Save(archivo, ImageFormat.Png);
            L("  captura: " + Path.GetFileName(archivo) + " (" + w + "×" + al + ")");
        }
    }

    static void CapturarForm(Form f, string nombre)
    {
        Pedir(() => { f.TopMost = true; f.Activate(); SetForegroundWindow(f.Handle); f.Refresh(); });
        Esperar(500);
        IntPtr h = Pedir(() => f.Handle);
        Captura(h, nombre);
        Pedir(() => { if (!f.IsDisposed) f.TopMost = false; });
    }

    // Espera a que todas las vistas web visibles terminen de cargar (o fallen).
    static void EsperarVistas(int maxMs)
    {
        var limite = DateTime.Now.AddMilliseconds(maxMs);
        while (DateTime.Now < limite)
        {
            bool pendientes = Pedir(() => Application.OpenForms.Cast<Form>().SelectMany(f => Todos(f)).OfType<PanelWeb>()
                .Any(p => !p.IsDisposed && p.Visible && !p.InicializacionTerminada));
            if (!pendientes) break;
            Esperar(250);
        }
        Esperar(1200);
    }

    // Lo que se ve de un formulario: vistas web, avisos, controles cortados.
    static void Inspeccionar(Form f, string nombre)
    {
        L("  ventana: " + Pedir(() => f.GetType().Name + " «" + f.Text + "» cliente " + f.ClientSize.Width + "×" + f.ClientSize.Height + " en " + f.Bounds));
        var info = Pedir(() =>
        {
            var sb = new List<string>();
            foreach (var p in Todos(f).OfType<PanelWeb>())
                sb.Add("    vista web " + p.Name + ": " + (p.Visible ? "VISIBLE" : "oculta") + ", preparada=" + p.Preparado + ", terminada=" + p.InicializacionTerminada + ", " + p.Width + "×" + p.Height);
            foreach (var c in Todos(f).Where(c => c.Visible && (c.Name.StartsWith("aviso") || c.Name == "estadoCargaVistaWeb" || c.Name.StartsWith("estado"))))
                if (!string.IsNullOrWhiteSpace(c.Text)) sb.Add("    texto «" + c.Name + "»: " + c.Text.Replace("\r\n", " ").Replace("\n", " "));
            var grillas = Todos(f).OfType<DataGridView>().Where(g => g.Visible).ToList();
            foreach (var g in grillas) sb.Add("    grilla visible " + g.Name + ": " + g.Rows.Count + " filas, " + g.Width + "×" + g.Height);
            var botones = Todos(f).OfType<Button>().Where(b => b.Visible).Select(b => b.Text.Trim()).Where(t => t.Length > 0).Distinct().ToList();
            sb.Add("    botones visibles (" + botones.Count + "): " + string.Join(" · ", botones.Take(25)));
            // controles visibles que se salen del área cliente de su formulario
            var cortados = new List<string>();
            foreach (var c in Todos(f).Where(c => c.Visible && !(c is PanelWeb) && c.Width > 0 && c.Height > 0))
            {
                if (c.Parent is ScrollableControl sc && sc.AutoScroll && sc != f) continue;
                var rc = f.RectangleToClient(c.RectangleToScreen(c.ClientRectangle));
                if (rc.Bottom > f.ClientSize.Height + 1 || rc.Right > f.ClientSize.Width + 1)
                    if (c is Button || c is Label || c is TextBox || c is DataGridView)
                        cortados.Add(c.GetType().Name + " «" + (c.Text.Length > 30 ? c.Text.Substring(0, 30) : c.Text) + "» " + rc);
            }
            if (cortados.Count > 0) sb.Add("    fuera del área cliente (" + cortados.Count + "): " + string.Join(" | ", cortados.Take(8)));
            return string.Join(Environment.NewLine, sb);
        });
        if (info.Length > 0) L(info);
        CapturarForm(f, nombre);
    }

    static void Pantalla(string nombre, Action abrir)
    {
        L("");
        L("== " + nombre);
        long antes = TamLog();
        Hacer(() => { try { abrir(); } catch (Exception ex) { L("  EXCEPCIÓN AL ABRIR: " + ex.GetType().Name + ": " + ex.Message); } });
        Esperar(1500);
        EsperarVistas(20000);
        CerrarCuadros();
        foreach (var f in Abiertos()) Inspeccionar(f, nombre + "-" + Pedir(() => f.GetType().Name));
        CerrarCuadros();
        string nuevo = LogDesde(antes);
        L(nuevo.Length == 0 ? "  errores.log: sin entradas nuevas" : "  errores.log: ENTRADAS NUEVAS:" + Environment.NewLine + nuevo);
    }

    static void Entrar(string mail, string clave)
    {
        Pedir(() => { GestorUsuarios.Instance.Autenticar(mail, clave); });
        L("");
        L("-- sesión: " + mail);
    }
    static Button BotonPorTexto(Control c, string texto)
    {
        return Todos(c).OfType<Button>().FirstOrDefault(b => b.Visible && b.Text.Trim() == texto);
    }

    // ---------- las 14 pantallas con vista web ----------
    static void Pantallas()
    {
        Pantalla("01-bienvenida", () => new Visual.frmBienvenida().Show());
        CerrarTodo();
        Pantalla("02-acceso", () => new Visual.frmInicio().Show());
        CerrarTodo();
        Pantalla("03-alta", () => new Visual.frmCrearUsuario().Show());
        CerrarTodo();
        Pantalla("04-recuperacion", () => new Visual.frmCambiarContraseña(true).Show());
        CerrarTodo();

        Entrar("plata@eventaura.com", "Cliente123");
        Visual.frmMenuCliente menu = null;
        Pedir(() => { menu = new Visual.frmMenuCliente { IDUsuario = 4, NombreUsuario = "Paula" }; menu.Show(); });
        Esperar(2000);
        Pantalla("05-catalogo", () => Buscar<Button>(menu, "btnEventos").PerformClick());
        Pantalla("06a-ficha-del-evento", () =>
        {
            // La última tarjeta es «Duki en Rosario», la que tiene entradas disponibles.
            var ver = Todos(menu).OfType<Button>().Where(b => b.Visible && b.Text.Contains("Ver entradas")).LastOrDefault();
            if (ver != null) { L("  (abierta con el último «Ver entradas» del catálogo)"); ver.PerformClick(); }
            else L("  no hay un botón «Ver entradas» de Windows Forms visible");
        });
        Pantalla("06-armador", () =>
        {
            var armar = BotonPorTexto(menu, "Armar paquete");
            if (armar != null) { L("  (abierta con «Armar paquete» de la ficha)"); armar.PerformClick(); }
            else L("  no hay un botón «Armar paquete» visible");
        });
        Pantalla("07-historial", () => Buscar<Button>(menu, "btnHistorial").PerformClick());
        CerrarTodo();

        Entrar("organizador@eventaura.com", "Cliente123");
        Visual.frmMenuPropietario mp = null;
        Pedir(() => { mp = new Visual.frmMenuPropietario { IDUsuario = 2, NombreUsuario = "Organizador" }; mp.Show(); });
        Esperar(2000);
        Pantalla("08-mis-locales", () => Buscar<Button>(mp, "btnMisLocales").PerformClick());
        CerrarTodo();
        Pedir(() => { mp = new Visual.frmMenuPropietario { IDUsuario = 2, NombreUsuario = "Organizador" }; mp.Show(); });
        Pantalla("09-eventos-del-local", () => new Visual.Vendedor.frmEventosLocal { IDUsuario = 2, IDlocal = 1, NombreLocal = "Sala Demostrativa EventAura", NombreUsuario = "Organizador" }.Show());
        CerrarTodo();
        Pedir(() => { mp = new Visual.frmMenuPropietario { IDUsuario = 2, NombreUsuario = "Organizador" }; mp.Show(); });
        Pantalla("10-control-ingreso", () => Buscar<Button>(mp, "btnControlIngreso").PerformClick());
        CerrarTodo();
        Pedir(() => { mp = new Visual.frmMenuPropietario { IDUsuario = 2, NombreUsuario = "Organizador" }; mp.Show(); });
        Pantalla("11-reportes", () => Buscar<Button>(mp, "btnReportes").PerformClick());
        CerrarTodo();

        Entrar("admin@eventaura.com", "Admin1234");
        Visual.frmMenuAdministrador ma = null;
        Pantalla("12-panel-administrador", () => { ma = new Visual.frmMenuAdministrador { IDUsuario = 1, NombreUsuario = "Ana" }; ma.Show(); });
        Pantalla("13-auditoria", () => new Visual.frmAuditoria().Show());
        CerrarTodo();
        Pedir(() => { ma = new Visual.frmMenuAdministrador { IDUsuario = 1, NombreUsuario = "Ana" }; ma.Show(); });
        Esperar(1500);
        Pantalla("14-revision-eventos", () => Buscar<Button>(ma, "btnEventosEnRevision").PerformClick());
        CerrarTodo();
    }

    // ---------- T-23: mensajes de la solicitud de Organizador ----------
    static string AvisoActual(Form f)
    {
        return Pedir(() =>
        {
            var a = Todos(f).FirstOrDefault(c => c.Name == "avisoSistema" && !c.IsDisposed);
            var e = Todos(f).FirstOrDefault(c => c.Name == "estadoEnvioTelefono");
            return "avisoSistema=«" + (a == null ? "(ninguno)" : a.Text) + "» · estadoEnvioTelefono=«" + (e == null ? "" : e.Text) + "»";
        });
    }
    static void Caso(Form f, string nombre, Action accion)
    {
        L("");
        L("== " + nombre);
        long antes = TamLog();
        Pedir(() => { foreach (var a in Todos(f).Where(c => c.Name == "avisoSistema").ToList()) a.Dispose(); });
        Hacer(accion);
        Esperar(700);
        L("  " + AvisoActual(f));
        CerrarCuadros();
        CapturarForm(f, nombre);
        string nuevo = LogDesde(antes);
        L(nuevo.Length == 0 ? "  errores.log: sin entradas nuevas" : "  errores.log: ENTRADAS NUEVAS:" + Environment.NewLine + nuevo);
    }
    static void T23()
    {
        Entrar("plata@eventaura.com", "Cliente123");
        Visual.frmSolicitudOrganizador s = null;
        Pedir(() => { s = new Visual.frmSolicitudOrganizador(); s.StartPosition = FormStartPosition.CenterScreen; s.Show(); });
        Esperar(1500);
        Action<string, string> llenar = (cuit, cbu) => Pedir(() =>
        {
            Buscar<TextBox>(s, "responsable").Text = "Paula Plata";
            Buscar<TextBox>(s, "cuit").Text = cuit;
            Buscar<TextBox>(s, "telefono").Text = "341 555 1234";
            Buscar<TextBox>(s, "local").Text = "Sala de prueba";
            Buscar<TextBox>(s, "direccion").Text = "Córdoba 1000, Rosario";
            Buscar<NumericUpDown>(s, "capacidad").Value = 300;
            Buscar<TextBox>(s, "cbu").Text = cbu;
        });
        // Adjunto válido: el diálogo de archivos no se puede operar sin mandar teclas; se carga la
        // misma imagen que produciría «Adjuntar documentación», con ImagenEventoCodec.Preparar.
        Pedir(() =>
        {
            using (var bmp = new Bitmap(320, 200)) using (var g = Graphics.FromImage(bmp)) using (var ms = new MemoryStream())
            {
                g.Clear(Color.SteelBlue); g.DrawString("DNI de prueba", new Font("Segoe UI", 20), Brushes.White, 20, 80);
                bmp.Save(ms, ImageFormat.Png); ms.Position = 0;
                var img = Visual.ImagenEventoCodec.Preparar(ms);
                typeof(Visual.frmSolicitudOrganizador).GetField("imagen", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(s, img);
                typeof(Visual.frmSolicitudOrganizador).GetField("nombreArchivo", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(s, "dni.png");
            }
        });
        Func<Button> enviar = () => BotonPorTexto(s, "Enviar solicitud");

        llenar("20123456780", "eventaura.sala");
        Caso(s, "t23-1-cuit-invalido", () => enviar().PerformClick());

        llenar("20123456786", "123");
        Caso(s, "t23-extra-cbu-invalido", () => enviar().PerformClick());

        llenar("20123456786", "eventaura.sala");
        Caso(s, "t23-2-telefono-sin-confirmar", () => enviar().PerformClick());

        Caso(s, "t23-3a-enviar-codigo", () => Buscar<Button>(s, "btnEnviarCodigoTelefono").PerformClick());
        Pedir(() => Buscar<TextBox>(s, "codigo").Text = "000000");
        Caso(s, "t23-3-codigo-incorrecto", () => BotonPorTexto(s, "Confirmar teléfono").PerformClick());
        CerrarTodo();

        Entrar("admin@eventaura.com", "Admin1234");
        Form r = null;
        Pedir(() => { r = new Visual.frmRevisarSolicitudesOrganizador(); r.StartPosition = FormStartPosition.CenterScreen; r.Show(); });
        Esperar(2000);
        L("  solicitudes pendientes en la lista: " + Pedir(() =>
        {
            var lista = Todos(r).OfType<ListBox>().FirstOrDefault();
            var grilla = Todos(r).OfType<DataGridView>().FirstOrDefault();
            if (lista != null) { if (lista.Items.Count > 0 && lista.SelectedIndex < 0) lista.SelectedIndex = 0; return "lista " + lista.Items.Count + ", elegida " + lista.SelectedIndex; }
            if (grilla != null) { if (grilla.Rows.Count > 0) { grilla.CurrentCell = grilla.Rows[0].Cells.Cast<DataGridViewCell>().First(c => c.Visible); } return "grilla " + grilla.Rows.Count; }
            return "sin lista";
        }));
        Esperar(500);
        Caso(r, "t23-4-rechazo-sin-motivo", () =>
        {
            foreach (var t in Todos(r).OfType<TextBox>().Where(t => t.Multiline)) t.Text = "";
            BotonPorTexto(r, "Rechazar").PerformClick();
        });
        CerrarTodo();
    }

    // ---------- T-26: el catálogo y la compra dentro de «Mi cuenta» ----------
    static string Js(PanelWeb p, string script)
    {
        var t = Pedir(() => p.EjecutarScriptAsync(script));
        if (!t.Wait(10000)) return "(sin respuesta)";
        return t.Result;
    }
    static void T26()
    {
        Entrar("plata@eventaura.com", "Cliente123");
        Visual.frmMenuCliente menu = null;
        Pedir(() => { menu = new Visual.frmMenuCliente { IDUsuario = 4, NombreUsuario = "Paula" }; menu.Show(); });
        Esperar(2500);
        Pantalla("t26-1-inicio", () => { });
        Pantalla("t26-2-catalogo", () => Buscar<Button>(menu, "btnEventos").PerformClick());
        L("  título del menú: «" + Pedir(() => Buscar<Label>(menu, "label1").Text) + "»");
        var web = Pedir(() => Todos(menu).OfType<PanelWeb>().FirstOrDefault(p => p.Visible));
        if (web != null)
        {
            L("  vista: " + Pedir(() => web.Name + " en " + menu.RectangleToClient(web.RectangleToScreen(web.ClientRectangle)) + " de " + menu.ClientSize));
            L("  medición dentro de la página: " + Js(web, @"(() => { const vh = innerHeight, vw = innerWidth;
                const bs = [...document.querySelectorAll('button, a')].filter(b => /Ver entradas/i.test(b.textContent));
                const r = bs.map(b => { const q = b.getBoundingClientRect(); return Math.round(q.top) + '-' + Math.round(q.bottom); });
                const prim = bs.length ? Math.min(...bs.map(b => b.getBoundingClientRect().top)) : null;
                const fila = bs.filter(b => Math.abs(b.getBoundingClientRect().top - prim) < 4).map(b => b.getBoundingClientRect());
                return JSON.stringify({ vista: vw + 'x' + vh, botones: bs.length, alturas: r.slice(0, 6), primeraFilaCompleta: fila.every(q => q.bottom <= vh && q.right <= vw), scrollAlto: document.documentElement.scrollHeight });
            })()"));
            Pantalla("t26-3-compra", () => { });
            Js(web, @"(() => { const b = [...document.querySelectorAll('button, a')].find(b => /Ver entradas/i.test(b.textContent)); if (b) b.click(); return !!b; })()");
            Esperar(2500);
            EsperarVistas(15000);
            L("  título del menú después de «Ver entradas»: «" + Pedir(() => Buscar<Label>(menu, "label1").Text) + "»");
            foreach (var f in Abiertos()) Inspeccionar(f, "t26-3-compra-" + Pedir(() => f.GetType().Name));
            var compraWeb = Pedir(() => Todos(menu).OfType<PanelWeb>().FirstOrDefault(p => p.Visible && p != web));
            if (compraWeb != null)
            {
                L("  vista de la compra: " + Pedir(() => compraWeb.Name + " en " + menu.RectangleToClient(compraWeb.RectangleToScreen(compraWeb.ClientRectangle)) + " de " + menu.ClientSize));
                L("  medición dentro del armador: " + Js(compraWeb, @"(() => JSON.stringify({ vista: innerWidth + 'x' + innerHeight, scrollAlto: document.documentElement.scrollHeight,
                    h1: (document.querySelector('h1,h2')||{}).textContent, botonPago: (() => { const b=[...document.querySelectorAll('button')].find(b=>/pago|Continuar/i.test(b.textContent)); if(!b) return null; const q=b.getBoundingClientRect(); return b.textContent.trim()+' '+Math.round(q.top)+'-'+Math.round(q.bottom); })() }))()"));
            }
        }
        CerrarTodo();
    }

    // ---------- T-22: el ticket y el PDF sin iText ----------
    static void T22()
    {
        Entrar("admin@eventaura.com", "Admin1234");
        Form rep = null;
        Pedir(() => { rep = new Visual.frmReportes { IDUsuario = 1, NombreUsuario = "Ana" }; rep.StartPosition = FormStartPosition.CenterScreen; rep.Show(); });
        Esperar(3000);
        EsperarVistas(15000);
        Caso(rep, "t22-1-generar", () => BotonPorTexto(rep, "Generar").PerformClick());
        Esperar(1500);
        L("  botones: " + string.Join(" · ", Pedir(() => Todos(rep).OfType<Button>().Where(b => b.Visible).Select(b => b.Text.Trim()).ToList())));
        // La exportación abre un SaveFileDialog: se lo responde con WM_CLOSE si aparece, y se anota.
        Caso(rep, "t22-2-exportar", () =>
        {
            var b = Todos(rep).OfType<Button>().FirstOrDefault(x => x.Visible && x.Text.Contains("PDF") || x.Visible && x.Text.Contains("Exportar"));
            if (b == null) L("  no hay botón de exportación visible"); else b.PerformClick();
        });
        CerrarTodo();

        // La compra de punta a punta, en el armador web: 1 entrada de Campo, sin seguro, pago simulado.
        L(""); L("== t22-3-compra");
        Entrar("plata@eventaura.com", "Cliente123");
        long antesLog = TamLog();
        int comprasAntes = Contar("SELECT COUNT(*) FROM Compras WHERE ID_Usuario = 4");
        string tickets = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A1.Tickets EVENTAURA");
        int pdfAntes = Directory.Exists(tickets) ? Directory.GetFiles(tickets).Length : 0;
        Visual.FrmCompraCliente compra = null;
        Pedir(() => { compra = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = 4, NombreUsuario = "Paula" }; compra.StartPosition = FormStartPosition.CenterScreen; compra.Show(); });
        Esperar(2000); EsperarVistas(20000);
        var web = Pedir(() => Todos(compra).OfType<PanelWeb>().FirstOrDefault(p => p.Visible));
        L("  armador web visible: " + (web != null));
        L("  sumar Campo: " + Js(web, @"(() => { const m=[...document.querySelectorAll('button.mas')]; m[m.length-1].click(); return m.length; })()"));
        Esperar(800);
        L("  continuar: " + Js(web, @"(() => { const b=document.querySelector('#continuar'); const d=b.disabled; b.click(); return 'deshabilitado antes=' + d; })()"));
        Esperar(800);
        L("  seguir sin seguro: " + Js(web, @"(() => { const b=document.querySelector('#seguir-sin-seguro'); const vis = b && b.offsetParent !== null; if (vis) b.click(); return 'visible=' + vis; })()"));
        // El pago (modal de Windows Forms) y la confirmación.
        var limite = DateTime.Now.AddSeconds(30);
        bool pagado = false;
        while (DateTime.Now < limite)
        {
            CerrarCuadros();
            var pago = Pedir(() => Application.OpenForms.OfType<Visual.frmPago>().FirstOrDefault(f => f.Visible));
            if (pago != null && !pagado)
            {
                pagado = true;
                Pedir(() =>
                {
                    Buscar<TextBox>(pago, "txtTitular").Text = "Paula Plata";
                    Buscar<TextBox>(pago, "txtNumero").Text = "4111 1111 1111 1111";
                    Buscar<TextBox>(pago, "txtVencimiento").Text = "12/28";
                    Buscar<TextBox>(pago, "txtCvv").Text = "123";
                });
                CapturarForm(pago, "t22-3a-pago");
                Hacer(() => Buscar<Button>(pago, "btnPagar").PerformClick());
                Esperar(1500);
                continue;
            }
            var conf = Pedir(() => Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.GetType().Name == "frmConfirmacion" && f.Visible));
            if (conf != null)
            {
                Esperar(1000);
                L("  confirmación: " + Pedir(() => string.Join(" | ", Todos(conf).Where(c => c.Visible && (c is Label || c is TextBox) && c.Text.Trim().Length > 0).Select(c => c.Text.Replace("\r", "").Replace("\n", " ")))));
                CapturarForm(conf, "t22-3b-confirmacion");
                Hacer(() => conf.Close());
                break;
            }
            Esperar(500);
        }
        Esperar(1000);
        CerrarCuadros();
        L("  compras de Paula: " + comprasAntes + " → " + Contar("SELECT COUNT(*) FROM Compras WHERE ID_Usuario = 4") +
          " · última: " + Texto("SELECT TOP 1 CONCAT(ID, ' ', Estado) FROM Compras WHERE ID_Usuario = 4 ORDER BY ID DESC"));
        L("  archivos en «A1.Tickets EVENTAURA»: " + pdfAntes + " → " + (Directory.Exists(tickets) ? Directory.GetFiles(tickets).Length : 0));
        string nuevo = LogDesde(antesLog);
        L(nuevo.Length == 0 ? "  errores.log: sin entradas nuevas" : "  errores.log: ENTRADAS NUEVAS:" + Environment.NewLine + nuevo);
        CerrarTodo();
    }

    static string Cadena()
    {
        return System.Configuration.ConfigurationManager.ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>()
            .First(c => c.ConnectionString.Contains("Initial Catalog")).ConnectionString;
    }
    static string Texto(string sql)
    {
        using (var c = new System.Data.SqlClient.SqlConnection(Cadena().Replace("Encrypt=True", "Encrypt=False")))
        {
            c.Open();
            using (var k = new System.Data.SqlClient.SqlCommand(sql, c)) return Convert.ToString(k.ExecuteScalar());
        }
    }
    static int Contar(string sql) { return Convert.ToInt32(Texto(sql)); }

    // ---------- T-24: «Agregar evento» con hora ----------
    static void T24()
    {
        Entrar("organizador@eventaura.com", "Cliente123");
        Form f = null;
        Pedir(() => { f = new Visual.Propietario.frmAgregarEvento { IDUsuario = 2, IDlocal = 1, NombreUsuario = "Organizador", NombreLocal = "Sala Demostrativa EventAura" }; f.StartPosition = FormStartPosition.CenterScreen; f.Show(); });
        Esperar(2000);
        L("  selector: " + Pedir(() => { var d = Buscar<DateTimePicker>(f, "dtEvento"); return d.Format + " «" + d.CustomFormat + "» valor inicial " + d.Value.ToString("dd/MM/yyyy HH:mm:ss") + " (ahora " + DateTime.Now.ToString("HH:mm:ss") + ")"; }));
        CapturarForm(f, "t24-1-agregar-evento");
        L("  controles: " + string.Join(" | ", Pedir(() => Todos(f).Where(c => c.Visible && (c is TextBox || c is ComboBox || c is NumericUpDown || c is DateTimePicker)).Select(c => c.GetType().Name + ":" + c.Name).ToList())));
        string nombre = "Verificacion T24 " + DateTime.Now.ToString("HHmmss");
        var elegida = DateTime.Today.AddDays(3).AddHours(21);
        Pedir(() =>
        {
            Buscar<TextBox>(f, "txtNombre").Text = nombre;
            var cat = Buscar<ComboBox>(f, "cmbCategoria"); cat.SelectedIndex = 0;
            Buscar<DateTimePicker>(f, "dtEvento").Value = elegida;
        });
        L("  elegido en pantalla: " + Pedir(() => Buscar<DateTimePicker>(f, "dtEvento").Text));
        CapturarForm(f, "t24-2-con-hora");
        long antes = TamLog();
        Hacer(() => Buscar<Button>(f, "btnRegistrar").PerformClick());
        Esperar(2500);
        CerrarCuadros();
        L("  guardado en la base: " + Texto("SELECT TOP 1 CONCAT(ID, ' · ', Nombre, ' · ', CONVERT(varchar(19), Fecha, 120), ' · ', Estado) FROM Eventos WHERE Nombre = '" + nombre + "' ORDER BY ID DESC"));
        foreach (var x in Abiertos()) Inspeccionar(x, "t24-3-despues");
        string nuevo = LogDesde(antes);
        L(nuevo.Length == 0 ? "  errores.log: sin entradas nuevas" : "  errores.log: ENTRADAS NUEVAS:" + Environment.NewLine + nuevo);
        CerrarTodo();
    }

    // ---------- T-25: la hora de la auditoría ----------
    static void T25()
    {
        Entrar("admin@eventaura.com", "Admin1234");
        var antes = DateTime.Now;
        L("  hora local del inicio de sesión: " + antes.ToString("HH:mm:ss") + " · UTC " + DateTime.UtcNow.ToString("HH:mm:ss"));
        Form f = null;
        Pedir(() => { f = new Visual.frmAuditoria(); f.StartPosition = FormStartPosition.CenterScreen; f.Show(); });
        Esperar(2500);
        EsperarVistas(15000);
        Inspeccionar(f, "t25-auditoria");
        var web = Pedir(() => Todos(f).OfType<PanelWeb>().FirstOrDefault(p => p.Visible));
        if (web != null)
            L("  primeras filas en la página: " + Js(web, @"(() => JSON.stringify([...document.querySelectorAll('tr, li, .fila, .entrada, article')].slice(0, 6).map(e => e.innerText.replace(/\s+/g, ' ').slice(0, 140))))()"));
        var g = Pedir(() => Todos(f).OfType<DataGridView>().FirstOrDefault());
        if (g != null)
            L("  primeras filas de la grilla: " + Pedir(() => string.Join(" || ", g.Rows.Cast<DataGridViewRow>().Take(4).Select(r => string.Join(" | ", r.Cells.Cast<DataGridViewCell>().Select(c => Convert.ToString(c.FormattedValue)))))));
        CerrarTodo();
    }

    static void T26b()
    {
        Entrar("plata@eventaura.com", "Cliente123");
        Visual.frmMenuCliente menu = null;
        Pedir(() => { menu = new Visual.frmMenuCliente { IDUsuario = 4, NombreUsuario = "Paula" }; menu.Show(); });
        Esperar(2500);
        L("  menú: borde " + Pedir(() => menu.FormBorderStyle + ", maximizar=" + menu.MaximizeBox + ", mínimo " + menu.MinimumSize + ", máximo " + menu.MaximumSize + ", cliente " + menu.ClientSize));
        Hacer(() => Buscar<Button>(menu, "btnEventos").PerformClick());
        Esperar(2000); EsperarVistas(15000);
        var web = Pedir(() => Todos(menu).OfType<PanelWeb>().FirstOrDefault(p => p.Visible));
        Js(web, @"(() => { const b=[...document.querySelectorAll('button, a')].filter(b => /Ver entradas/i.test(b.textContent)); b[b.length-1].click(); return b.length; })()");
        Esperar(2500);
        L("  ficha: " + Pedir(() =>
        {
            var b = BotonPorTexto(menu, "Armar paquete");
            if (b == null) return "sin «Armar paquete»";
            var r = menu.RectangleToClient(b.RectangleToScreen(b.ClientRectangle));
            var cont = b.Parent; var conScroll = new List<string>();
            for (Control c = b.Parent; c != null; c = c.Parent) if (c is ScrollableControl sc && sc.AutoScroll) conScroll.Add(c.GetType().Name + " " + c.Name + " scrollV=" + sc.VerticalScroll.Visible);
            return "«Armar paquete» en " + r + " · área cliente " + menu.ClientSize + " · contenedores con AutoScroll: " + (conScroll.Count == 0 ? "ninguno" : string.Join("; ", conScroll));
        }));
        CapturarForm(menu, "t26b-1-ficha");
        Hacer(() => BotonPorTexto(menu, "Armar paquete").PerformClick());
        Esperar(2500); EsperarVistas(15000);
        var arm = Pedir(() => Todos(menu).OfType<PanelWeb>().FirstOrDefault(p => p.Visible && p.Name == "resumenCompraWeb"));
        if (arm == null) { L("  no se ve el armador web"); return; }
        var zona = Pedir(() => menu.RectangleToClient(arm.RectangleToScreen(arm.ClientRectangle)));
        L("  armador web en " + zona + " · área cliente " + Pedir(() => menu.ClientSize));
        L("  dentro del armador: " + Js(arm, @"(() => { const q = s => { const e=document.querySelector(s); if(!e) return null; const r=e.getBoundingClientRect(); return Math.round(r.left)+','+Math.round(r.top)+' '+Math.round(r.width)+'x'+Math.round(r.height); };
            return JSON.stringify({ vista: innerWidth+'x'+innerHeight, continuar: q('#continuar'), total: q('#total'), scrollAncho: document.documentElement.scrollWidth, scrollAlto: document.documentElement.scrollHeight }); })()"));
        CapturarForm(menu, "t26b-2-armador");
        CerrarTodo();
    }
}
