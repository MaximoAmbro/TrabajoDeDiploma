// Medición de usabilidad sobre la aplicación real (docs/usabilidad/). Abre los formularios de
// Visual contra BddEventAura_Pruebas, actúa sobre sus controles como lo haría el usuario y
// registra lo que pasa: acciones, pantallas, orden de tabulación, diálogos y pérdidas de datos.
// No envía teclas al escritorio: las teclas (Enter) se mandan con PostMessage a ventanas de
// este mismo proceso. C# 5 (csc de .NET Framework). Uso: Medicion.exe <carpeta de salida>
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Entidades;
using Negocio;

static class Medicion
{
    [DllImport("user32.dll")] static extern bool EnumThreadWindows(uint id, EnumProc cb, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr h, EnumProc cb, IntPtr p);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] static extern int GetDlgCtrlID(IntPtr h);
    [DllImport("user32.dll")] static extern IntPtr GetDlgItem(IntPtr h, int id);
    [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
    [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
    delegate bool EnumProc(IntPtr h, IntPtr p);

    static readonly List<string> salida = new List<string>();
    static void L(string s) { salida.Add(s); }

    // ---------------- contador de acciones del usuario (camino de la tarea) ----------------
    static int clics, campos, caracteres, teclas, decisiones;
    static readonly List<string> pantallas = new List<string>();
    static readonly List<string> pasos = new List<string>();
    static void Pantalla(string p) { if (!pantallas.Contains(p)) pantallas.Add(p); }
    static void Clic(string que) { clics++; pasos.Add("clic: " + que); }
    static void Campo(TextBox t, string valor, string que)
    {
        clics++; campos++; caracteres += valor.Length;
        t.Focus(); t.Text = valor;
        pasos.Add("campo: " + que + " (" + valor.Length + " caracteres)");
        Esperar(150);
    }
    static void Tecla(string que) { teclas++; pasos.Add("tecla: " + que); }

    // ---------------- diálogos nativos: se registran y se responden según un plan ----------------
    class Respuesta { public string Titulo; public int Boton; }
    static readonly Queue<Respuesta> plan = new Queue<Respuesta>();
    static readonly List<string> dialogos = new List<string>();
    static readonly HashSet<IntPtr> vistos = new HashSet<IntPtr>();
    static System.Windows.Forms.Timer vigia;

    static void Vigilar()
    {
        EnumThreadWindows(GetCurrentThreadId(), (h, p) =>
        {
            if (!IsWindowVisible(h) || Clase(h) != "#32770" || vistos.Contains(h)) return true;
            vistos.Add(h);
            string titulo = Texto(h);
            var estaticos = new List<string>(); var botones = new List<string>();
            EnumChildWindows(h, (c, q) =>
            {
                string k = Clase(c);
                if (k == "Static" && Texto(c).Trim().Length > 0) estaticos.Add(Texto(c).Replace("\r", "").Replace("\n", " ⏎ "));
                if (k == "Button") botones.Add(Texto(c).Replace("&", "") + "#" + GetDlgCtrlID(c));
                return true;
            }, IntPtr.Zero);
            int defecto = SendMessage(h, 0x400, IntPtr.Zero, IntPtr.Zero).ToInt32() & 0xffff;   // DM_GETDEFID
            string porDefecto = botones.FirstOrDefault(b => b.EndsWith("#" + defecto)) ?? ("id " + defecto);
            dialogos.Add("«" + titulo + "» | " + string.Join(" ", estaticos) + " | botones: " + string.Join(", ", botones) + " | por defecto: " + porDefecto);
            // Respuesta planificada si el título coincide; si no, la opción conservadora:
            // No (7), Cancelar (2) o Aceptar (1), la primera que exista.
            var candidatos = new List<int>();
            if (plan.Count > 0 && (plan.Peek().Titulo == null || titulo == plan.Peek().Titulo)) candidatos.Add(plan.Dequeue().Boton);
            candidatos.AddRange(new[] { 7, 2, 1 });
            IntPtr b2 = IntPtr.Zero;
            foreach (int id in candidatos) { b2 = GetDlgItem(h, id); if (b2 != IntPtr.Zero) break; }
            PostMessage(b2, 0xF5, IntPtr.Zero, IntPtr.Zero);   // BM_CLICK
            return true;
        }, IntPtr.Zero);
    }

    static string UltimoDialogo() { return dialogos.Count == 0 ? "(ninguno)" : dialogos[dialogos.Count - 1]; }

    [STAThread]
    static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        vigia = new System.Windows.Forms.Timer { Interval = 120 };
        vigia.Tick += (s, e) => Vigilar();
        vigia.Start();
        int codigo = 0;
        try
        {
            Inventarios();
            Recorrido();
            Sondeos();
        }
        catch (Exception ex) { L("ERROR: " + ex); codigo = 1; }
        File.WriteAllLines(Path.Combine(args[0], "medicion.txt"), salida, new UTF8Encoding(false));
        return codigo;
    }

    // ============================== 1. Inventario y tabulación ==============================
    static void Inventarios()
    {
        L("# 1. INVENTARIO Y ORDEN DE TABULACIÓN");
        Inventariar(new Visual.frmInicio(), "Iniciar sesión");
        Inventariar(new Visual.frmCrearUsuario(), "Crear cuenta");
        Inventariar(new Visual.frmMenuCliente { IDUsuario = 3, NombreUsuario = "Paula" }, "Mi cuenta");
        Inventariar(new Visual.frmEventosCliente { IDUsuario = 3, NombreUsuario = "Paula" }, "Eventos disponibles");
        Inventariar(new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = 3, NombreUsuario = "Paula" }, "Armar paquete");
        Compra c = IniciarCompraDePrueba();
        var pago = new Visual.frmPago { Compra = c };
        Inventariar(pago, "Pago seguro");
        Application.OpenForms.Cast<Form>().ToList().ForEach(f => { if (f.Visible) f.Hide(); });
    }

    static void Inventariar(Form f, string nombre)
    {
        f.Show(); Esperar(700);
        L("");
        L("## " + nombre + " — " + f.GetType().Name + " — título «" + f.Text + "» — área cliente " + f.ClientSize.Width + " × " + f.ClientSize.Height);
        L("Enter (AcceptButton): " + (f.AcceptButton == null ? "ninguno" : ((Control)f.AcceptButton).Name) +
          " | Esc (CancelButton): " + (f.CancelButton == null ? "ninguno" : ((Control)f.CancelButton).Name) +
          " | control con foco al abrir: " + (f.ActiveControl == null ? "ninguno" : f.ActiveControl.Name));
        var todos = Descendientes(f).Where(c => c.Visible).ToList();
        L("Controles visibles: " + todos.Count + " | campos de texto: " + todos.Count(c => c is TextBox) +
          " | botones: " + todos.Count(c => c is Button) + " | casillas: " + todos.Count(c => c is CheckBox) +
          " | enlaces (Label/LinkLabel con Click): " + todos.Count(c => c is LinkLabel || (c is Label && c.Cursor == Cursors.Hand)));
        foreach (Control c in todos.Where(c => c is TextBox || c is Button || c is CheckBox || c is ComboBox || c is DateTimePicker || c is DataGridView || c is LinkLabel || c.Cursor == Cursors.Hand))
            L("  · " + c.GetType().Name + " " + c.Name + " «" + Resumir(c.Text) + "»" + (c.Enabled ? "" : " [deshabilitado]") +
              " en (" + f.PointToClient(c.Parent.PointToScreen(c.Location)).X + "," + f.PointToClient(c.Parent.PointToScreen(c.Location)).Y + ")" +
              (c.TabStop ? "" : " [sin tabulación]"));

        // Orden real de tabulación: el mismo recorrido que hace WinForms con la tecla Tab.
        var orden = new List<Control>();
        Control actual = null;
        for (int i = 0; i < 200; i++)
        {
            actual = f.GetNextControl(actual, true);
            if (actual == null) break;
            if (actual.CanSelect && actual.TabStop && actual.Visible && actual.Enabled && !(actual is Label) && !(actual is Panel) && !(actual is GroupBox))
                orden.Add(actual);
        }
        L("Orden de tabulación real (" + orden.Count + "): " + string.Join(" → ", orden.Select(c => c.Name)));
        var visual = orden.OrderBy(c => Banda(f, c)).ThenBy(c => Pos(f, c).X).ToList();
        L("Orden de lectura visual:            " + string.Join(" → ", visual.Select(c => c.Name)));
        int saltos = 0;
        for (int i = 1; i < orden.Count; i++)
        {
            Point a = Pos(f, orden[i - 1]), b = Pos(f, orden[i]);
            bool atras = Banda(f, orden[i]) < Banda(f, orden[i - 1]) || (Banda(f, orden[i]) == Banda(f, orden[i - 1]) && b.X < a.X);
            if (atras) { saltos++; L("  SALTO HACIA ATRÁS: " + orden[i - 1].Name + " (" + a.X + "," + a.Y + ") → " + orden[i].Name + " (" + b.X + "," + b.Y + ")"); }
        }
        L("Saltos de tabulación contra el orden visual: " + saltos);
        var textos = todos.OfType<TextBox>().ToList();
        foreach (TextBox t in textos)
        {
            Label etiqueta = todos.OfType<Label>().Where(l => !(l is LinkLabel)).OrderBy(l => Distancia(f, l, t)).FirstOrDefault();
            string cue = t.Text.Length == 0 ? LeerCue(t) : "";
            L("  campo " + t.Name + ": rótulo más cercano «" + (etiqueta == null ? "(ninguno)" : Resumir(etiqueta.Text)) + "» a " +
              (etiqueta == null ? 0 : (int)Distancia(f, etiqueta, t)) + " px" + (cue.Length > 0 ? " | texto guía «" + cue + "»" : "") +
              (t.PasswordChar != '\0' || t.UseSystemPasswordChar ? " | oculto" : ""));
        }
    }

    // EM_GETCUEBANNER: wParam = búfer, lParam = tamaño en caracteres.
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")] static extern IntPtr SendMessageTexto(IntPtr h, uint m, StringBuilder w, IntPtr l);
    static string LeerCue(TextBox t) { var sb = new StringBuilder(256); SendMessageTexto(t.Handle, 0x1502, sb, new IntPtr(sb.Capacity)); return sb.ToString(); }

    // ============================== 2. Recorrido de la tarea ==============================
    static void Recorrido()
    {
        L(""); L("# 2. RECORRIDO DE LA TAREA (escenario del documento: Duki, 2 entradas, alojamiento)");
        clics = campos = caracteres = teclas = decisiones = 0; pantallas.Clear(); pasos.Clear(); dialogos.Clear();
        var t0 = DateTime.Now;

        var inicio = new Visual.frmInicio(); inicio.Show(); Esperar(500); Pantalla("Iniciar sesión");
        Campo(Buscar<TextBox>(inicio, "txtMailLogin"), "plata@eventaura.com", "mail");
        Campo(Buscar<TextBox>(inicio, "txtContraseñaLogin"), "Cliente123", "contraseña");
        // ¿Enter en la contraseña inicia sesión? Se manda la tecla a la caja de este proceso.
        var caja = Buscar<TextBox>(inicio, "txtContraseñaLogin");
        PostMessage(caja.Handle, 0x100, (IntPtr)0x0D, IntPtr.Zero); PostMessage(caja.Handle, 0x101, (IntPtr)0x0D, IntPtr.Zero);
        Esperar(900);
        bool enterLogin = Abierto<Visual.frmMenuCliente>() != null;
        L("Enter en la contraseña inicia sesión: " + (enterLogin ? "sí" : "NO — hay que hacer clic en «Iniciar sesión»"));
        if (enterLogin) Tecla("Enter en contraseña"); else { Clic("Iniciar sesión"); Buscar<Button>(inicio, "btninicio").PerformClick(); Esperar(900); }

        var menu = Abierto<Visual.frmMenuCliente>(); Pantalla("Mi cuenta (menú)");
        L("Menú del cliente: «" + Buscar<Label>(menu, "lblNombre").Text + "»; opciones: " +
          string.Join(", ", Descendientes(menu).OfType<Button>().Where(b => b.Visible).Select(b => b.Text)));
        Clic("Eventos disponibles"); Buscar<Button>(menu, "btnEventos").PerformClick(); Esperar(900);

        var eventos = Abierto<Visual.frmEventosCliente>(); Pantalla("Eventos disponibles");
        var grilla = Buscar<DataGridView>(eventos, "dgveventos");
        L("Listado al abrir: " + grilla.Rows.Count + " eventos; fila seleccionada por defecto: «" +
          (grilla.CurrentRow == null ? "(ninguna)" : Convert.ToString(grilla.CurrentRow.Cells["Nombre"].Value)) + "»");
        foreach (DataGridViewRow fila in grilla.Rows)
            if (Convert.ToString(fila.Cells["ID"].Value) == "1") grilla.CurrentCell = fila.Cells["Nombre"];
        Clic("fila «Duki en Rosario»");
        Clic("Comprar (listado)"); Buscar<Button>(eventos, "btnComprar").PerformClick(); Esperar(1000);

        var compra = Abierto<Visual.FrmCompraCliente>(); Pantalla("Armar paquete");
        L("Pantalla que abre «Comprar» del listado: «" + compra.Text + "»");
        var sectores = Buscar<DataGridView>(compra, "dgvSectores");
        L("Celda activa al abrir la compra: " + (sectores.CurrentCell == null ? "(ninguna)" :
          sectores.Columns[sectores.CurrentCell.ColumnIndex].Name + " de «" + Convert.ToString(sectores.CurrentRow.Cells[0].Value) + "»") +
          "; columnas editables: " + string.Join(", ", sectores.Columns.Cast<DataGridViewColumn>().Where(c => !c.ReadOnly && c.Visible).Select(c => c.HeaderText)));
        foreach (DataGridViewRow fila in sectores.Rows)
            foreach (DataGridViewCell celda in fila.Cells)
                if (Convert.ToString(celda.Value) == "Campo") { sectores.CurrentCell = fila.Cells["Cantidad"]; fila.Cells["Cantidad"].Value = 2; }
        Clic("celda Cantidad de Campo"); caracteres += 1; campos++; pasos.Add("campo: cantidad 2 en la grilla (1 carácter)"); Tecla("Enter para confirmar la celda");
        Clic("Hospedaje"); Buscar<CheckBox>(compra, "chkHospedaje").Checked = true; Esperar(400);
        L("Resumen tras elegir: " + Resumir(Buscar<Label>(compra, "lblResumen").Text.Replace("\r\n", " / "), 400));

        plan.Enqueue(new Respuesta { Titulo = "Seguro de entrada", Boton = 6 });   // Sí: el escenario no pide seguro
        var t = new System.Windows.Forms.Timer { Interval = 200 };
        int etapa = 0; string pedido = null;
        t.Tick += (s, e) =>
        {
            var pago = Abierto<Visual.frmPago>();
            if (etapa == 0 && pago != null)
            {
                etapa = 1; t.Stop(); Pantalla("Pago seguro");
                L("Pago: foco al abrir en «" + (pago.ActiveControl == null ? "(ninguno)" : pago.ActiveControl.Name) + "»; temporizador «" + Buscar<Label>(pago, "lblTemporizador").Text + "»");
                Campo(Buscar<TextBox>(pago, "txtTitular"), "Paula Plata", "titular");
                Campo(Buscar<TextBox>(pago, "txtNumero"), "4111111111111111", "número de tarjeta");
                Campo(Buscar<TextBox>(pago, "txtVencimiento"), "12/28", "vencimiento");
                Campo(Buscar<TextBox>(pago, "txtCvv"), "123", "código de seguridad");
                Tecla("Enter para pagar (AcceptButton)");
                t.Start();
                var cvv = Buscar<TextBox>(pago, "txtCvv");
                PostMessage(cvv.Handle, 0x100, (IntPtr)0x0D, IntPtr.Zero); PostMessage(cvv.Handle, 0x101, (IntPtr)0x0D, IntPtr.Zero);
                return;
            }
            var conf = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.GetType().Name == "frmConfirmacion" && f.Visible);
            if (etapa == 1 && conf != null)
            {
                etapa = 2; t.Stop(); Pantalla("Compra confirmada");
                foreach (Control c in Descendientes(conf)) if (c.Name == "lblPedido") pedido = c.Text;
                L("Confirmación: «" + pedido + "»; foco en «" + (conf.ActiveControl == null ? "(ninguno)" : conf.ActiveControl.Name) + "»");
                Clic("Aceptar (confirmación)");
                conf.Close();
            }
        };
        t.Start();
        Clic("Comprar (armado)"); decisiones++;
        Buscar<Button>(compra, "btnComprar").PerformClick();
        t.Stop();
        Clic("Sí, continuar sin seguro (diálogo)"); Pantalla("Diálogo «Seguro de entrada»");
        double segundos = (DateTime.Now - t0).TotalSeconds;

        L("Diálogos durante la tarea:"); foreach (string d in dialogos) L("  " + d);
        L("RESULTADO: tarea completa = " + (pedido != null) + " | " + pedido);
        L("ACCIONES: clics " + clics + " | campos completados " + campos + " | caracteres tipeados " + caracteres + " | teclas especiales " + teclas);
        L("PANTALLAS distintas: " + pantallas.Count + " → " + string.Join(" → ", pantallas));
        L("Secuencia de acciones:"); for (int i = 0; i < pasos.Count; i++) L("  " + (i + 1) + ". " + pasos[i]);
        L("Duración de la ejecución automatizada (sin tiempos humanos): " + segundos.ToString("0.0") + " s");

        L("Después de «Aceptar»: ventanas visibles = " + string.Join(", ", Application.OpenForms.Cast<Form>().Where(f => f.Visible).Select(f => f.Text)) +
          "; resumen ahora: " + Resumir(Buscar<Label>(compra, "lblResumen").Text.Replace("\r\n", " / "), 120));
        L("Ventanas ocultas acumuladas en memoria: " + Application.OpenForms.Cast<Form>().Count(f => !f.Visible) +
          " (" + string.Join(", ", Application.OpenForms.Cast<Form>().Where(f => !f.Visible).Select(f => f.GetType().Name)) + ")");
        MoverTickets(pedido);
        Application.OpenForms.Cast<Form>().ToList().ForEach(f => f.Hide());
    }

    // ============================== 3. Sondeos de error y pérdida ==============================
    static void Sondeos()
    {
        L(""); L("# 3. SONDEOS: ERRORES, PÉRDIDAS Y CALLEJONES");

        var inicio = new Visual.frmInicio(); inicio.Show(); Esperar(400);
        Buscar<Button>(inicio, "btninicio").PerformClick(); Esperar(700);
        L("S1 login con todo vacío → " + UltimoDialogo());
        Buscar<TextBox>(inicio, "txtMailLogin").Text = "plata@eventaura.com";
        Buscar<TextBox>(inicio, "txtContraseñaLogin").Text = "otraClave1";
        Buscar<Button>(inicio, "btninicio").PerformClick(); Esperar(700);
        L("S2 contraseña incorrecta → " + UltimoDialogo());
        Buscar<TextBox>(inicio, "txtMailLogin").Text = "nadie@eventaura.com";
        Buscar<Button>(inicio, "btninicio").PerformClick(); Esperar(700);
        L("S3 correo inexistente → " + UltimoDialogo());
        RestablecerIntentos("plata@eventaura.com");
        var chk = Buscar<CheckBox>(inicio, "checkbxShowPass");
        L("S4 casilla para ver la contraseña: «" + chk.Text + "»; botón «Modificar» visible en el login: " + Buscar<Button>(inicio, "btnModificar").Visible);
        inicio.Hide();

        var eventos = new Visual.frmEventosCliente { IDUsuario = 3, NombreUsuario = "Paula" }; eventos.Show(); Esperar(600);
        var dtp = Descendientes(eventos).OfType<DateTimePicker>().FirstOrDefault();
        var combo = Descendientes(eventos).OfType<ComboBox>().FirstOrDefault();
        L("S5 filtros del listado: categoría = ComboBox «" + (combo == null ? "?" : combo.Text) + "»; fecha = DateTimePicker con casilla " +
          (dtp == null ? "?" : (dtp.ShowCheckBox ? "(desmarcada: " + (!dtp.Checked) + ")" : "sin casilla")) +
          "; rótulos de texto en la pantalla: " + string.Join(", ", Descendientes(eventos).OfType<Label>().Where(l => l.Visible).Select(l => "«" + Resumir(l.Text) + "»")));
        var g = Buscar<DataGridView>(eventos, "dgveventos");
        Buscar<TextBox>(eventos, "txtBuscar").Text = "zzz"; Buscar<Button>(eventos, "btnBuscar").PerformClick(); Esperar(500);
        Buscar<Button>(eventos, "btnComprar").PerformClick(); Esperar(700);
        L("S6 «Comprar» con búsqueda sin resultados → " + UltimoDialogo());
        Buscar<Button>(eventos, "btnLimpiar").PerformClick(); Esperar(500);
        L("S7 al limpiar, fila seleccionada sin que el usuario elija: «" + (g.CurrentRow == null ? "(ninguna)" : Convert.ToString(g.CurrentRow.Cells["Nombre"].Value)) + "»");
        eventos.Hide();

        var compra = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = 3, NombreUsuario = "Paula" }; compra.Show(); Esperar(700);
        var sec = Buscar<DataGridView>(compra, "dgvSectores");
        DataGridViewRow campo = sec.Rows.Cast<DataGridViewRow>().First(r => r.Cells.Cast<DataGridViewCell>().Any(c => Convert.ToString(c.Value) == "Campo"));
        L("S8 sin entradas: «Comprar» habilitado = " + Buscar<Button>(compra, "btnComprar").Enabled + "; hospedaje habilitado = " + Buscar<CheckBox>(compra, "chkHospedaje").Enabled +
          "; seguro habilitado = " + Buscar<CheckBox>(compra, "chkSeguro").Enabled);
        int antes = dialogos.Count;
        Buscar<CheckBox>(compra, "chkHospedaje").Checked = true; Esperar(300);
        L("S9 tildar hospedaje sin entradas → " + (dialogos.Count > antes ? UltimoDialogo() : "(sin diálogo)") + "; resumen: " + Resumir(Buscar<Label>(compra, "lblResumen").Text.Replace("\r\n", " / "), 200));
        antes = dialogos.Count;
        Buscar<Button>(compra, "btnComprar").PerformClick(); Esperar(700);
        L("S10 «Comprar» con solo un servicio → " + (dialogos.Count > antes ? UltimoDialogo() : "(sin diálogo)"));
        Buscar<CheckBox>(compra, "chkHospedaje").Checked = false;
        antes = dialogos.Count; campo.Cells["Cantidad"].Value = 7; Esperar(600);
        L("S11 cantidad 7 (tope 6) → " + (dialogos.Count > antes ? UltimoDialogo() : "(sin diálogo)") + "; queda " + campo.Cells["Cantidad"].Value);
        antes = dialogos.Count; campo.Cells["Cantidad"].Value = "dos"; Esperar(600);
        L("S12 cantidad «dos» → " + (dialogos.Count > antes ? UltimoDialogo() : "(sin diálogo)"));
        var clasico = new Visual.FrmCompraCliente { IDEvento = 2, IDUsuario = 3, NombreUsuario = "Paula" }; clasico.Show(); Esperar(700);
        L("S13 evento agotado: aviso «" + Resumir(Buscar<Label>(clasico, "lblAgotado").Text) + "» visible = " + Buscar<Label>(clasico, "lblAgotado").Visible +
          "; botón «" + Buscar<Button>(clasico, "btnListaEspera").Text + "»");
        clasico.Close();

        campo.Cells["Cantidad"].Value = 2; Buscar<CheckBox>(compra, "chkHospedaje").Checked = true; Esperar(400);
        int abiertasAntes = Application.OpenForms.Count;
        antes = dialogos.Count; compra.Close(); Esperar(700);
        L("S14 cerrar «Armar paquete» con la X con un paquete armado → " + (dialogos.Count > antes ? UltimoDialogo() : "se cierra SIN preguntar") +
          "; ventanas visibles después: " + Application.OpenForms.Cast<Form>().Count(f => f.Visible) +
          "; ventanas ocultas que siguen vivas: " + Application.OpenForms.Cast<Form>().Count(f => !f.Visible));

        // Pago: errores de datos, rechazo, cancelar y cerrar con la X.
        Compra enPago = IniciarCompraDePrueba();
        int sector = GestorCompras.Instance.EntradasDe(enPago).First().IdSector.Value;
        var pago = new Visual.frmPago { Compra = enPago }; pago.Show(); Esperar(600);
        var estado = Buscar<Label>(pago, "lblEstado");
        Action<string, string, string, string, string> intentar = (nombre, titular, numero, vto, cvv) =>
        {
            Buscar<TextBox>(pago, "txtTitular").Text = titular; Buscar<TextBox>(pago, "txtNumero").Text = numero;
            Buscar<TextBox>(pago, "txtVencimiento").Text = vto; Buscar<TextBox>(pago, "txtCvv").Text = cvv;
            int d0 = dialogos.Count;
            Buscar<Button>(pago, "btnPagar").PerformClick(); Esperar(600);
            L("S15 pago, " + nombre + " → etiqueta «" + estado.Text.Replace("\n", " ⏎ ") + "»" + (dialogos.Count > d0 ? " | diálogo " + UltimoDialogo() : "") +
              " | ¿borró lo tipeado? número «" + Buscar<TextBox>(pago, "txtNumero").Text + "», CVV «" + Buscar<TextBox>(pago, "txtCvv").Text + "»");
        };
        intentar("sin titular", "", "4111111111111111", "12/28", "123");
        intentar("número de 12 dígitos", "Paula Plata", "411111111111", "12/28", "123");
        intentar("vencimiento 1228 sin barra", "Paula Plata", "4111111111111111", "1228", "123");
        intentar("tarjeta vencida 01/20", "Paula Plata", "4111111111111111", "01/20", "123");
        intentar("CVV de 2 dígitos", "Paula Plata", "4111111111111111", "12/28", "12");
        intentar("tarjeta rechazada (termina en 0000)", "Paula Plata", "4111111111110000", "12/28", "123");
        L("S16 cupo del sector tras el rechazo: disponibles " + GestorPaquetes.Instance.DisponiblesDe(sector));
        plan.Enqueue(new Respuesta { Titulo = "Cancelar pago", Boton = 7 });   // No
        antes = dialogos.Count; Buscar<Button>(pago, "btnCancelar").PerformClick(); Esperar(700);
        L("S17 «Cancelar pago» → " + (dialogos.Count > antes ? UltimoDialogo() : "(sin diálogo)"));
        antes = dialogos.Count; int disp0 = GestorPaquetes.Instance.DisponiblesDe(sector);
        pago.Close(); Esperar(600);
        L("S18 cerrar el pago con la X → " + (dialogos.Count > antes ? UltimoDialogo() : "se cierra SIN preguntar") +
          "; disponibles del sector antes " + disp0 + " y después " + GestorPaquetes.Instance.DisponiblesDe(sector));

        L(""); L("Ventanas de la aplicación que quedan vivas al final (todas ocultas): " + Application.OpenForms.Count);
    }

    // ------------------------------------- auxiliares -------------------------------------
    static Compra IniciarCompraDePrueba()
    {
        Paquete p = GestorPaquetes.Instance.Nuevo(1, 3);
        int campo = GestorEventos.Instance.Sectores(1).First(s => s.Nombre == "Campo").Id;
        GestorPaquetes.Instance.AgregarEntradas(p, campo, 1);
        return GestorCompras.Instance.Iniciar(p, GestorFidelizacion.Instance.ObtenerCliente(3));
    }

    static void RestablecerIntentos(string mail)
    {
        var u = Datos.RepositorioUsuarios.Instance.ObtenerPorMail(mail);
        if (u != null) Datos.RepositorioUsuarios.Instance.ActualizarIntentos(u.Id, 0, null);
    }

    static void MoverTickets(string pedido)
    {
        if (string.IsNullOrEmpty(pedido)) return;
        string codigo = pedido.Replace("Pedido", "").Trim();
        string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A1.Tickets EVENTAURA");
        // Solo los de este pedido de la base de pruebas y generados en esta corrida: nunca un
        // ticket real del usuario aunque coincidiera el número.
        foreach (string f in Directory.GetFiles(carpeta, "Ticket_" + codigo + "_*.PDF"))
            if (File.GetLastWriteTime(f) > DateTime.Now.AddMinutes(-10)) File.Delete(f);
        L("Tickets de prueba " + codigo + " eliminados de la carpeta del usuario.");
    }

    static T Abierto<T>() where T : Form { return Application.OpenForms.OfType<T>().LastOrDefault(f => f.Visible); }
    static T Buscar<T>(Form f, string n) where T : Control { return (T)f.Controls.Find(n, true).Single(); }
    static IEnumerable<Control> Descendientes(Control p) { foreach (Control c in p.Controls) { yield return c; foreach (Control h in Descendientes(c)) yield return h; } }
    static Point Pos(Form f, Control c) { return f.PointToClient(c.Parent.PointToScreen(c.Location)); }
    static int Banda(Form f, Control c) { return (Pos(f, c).Y + c.Height / 2) / 24; }
    static double Distancia(Form f, Control a, Control b)
    {
        Rectangle ra = new Rectangle(Pos(f, a), a.Size), rb = new Rectangle(Pos(f, b), b.Size);
        int dx = Math.Max(0, Math.Max(ra.Left - rb.Right, rb.Left - ra.Right));
        int dy = Math.Max(0, Math.Max(ra.Top - rb.Bottom, rb.Top - ra.Bottom));
        return Math.Sqrt(dx * dx + dy * dy);
    }
    static string Resumir(string s, int n = 60) { s = (s ?? "").Replace("\r", " ").Replace("\n", " "); return s.Length > n ? s.Substring(0, n) + "…" : s; }
    static string Clase(IntPtr h) { var s = new StringBuilder(64); GetClassName(h, s, 64); return s.ToString(); }
    static string Texto(IntPtr h) { var s = new StringBuilder(2048); GetWindowText(h, s, 2048); return s.ToString(); }
    static void Esperar(int ms) { var fin = DateTime.Now.AddMilliseconds(ms); while (DateTime.Now < fin) { Application.DoEvents(); Thread.Sleep(15); } }
}
