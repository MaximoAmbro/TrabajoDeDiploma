// CX-03, segunda parte: carga los formularios en el host de diseño de .NET (DesignSurface, de
// System.Design.dll), el mismo tipo de host que usa el diseñador de Visual Studio. No es
// Visual Studio: VS además interpreta el código de InitializeComponent desde el .cs, y eso no
// se reproduce acá. Tres comprobaciones:
//   1. La clase base FormularioEventAura como componente raíz: es lo que VS instancia al abrir
//      cualquiera de los 19 formularios.
//   2. Cada formulario como componente raíz: corre su constructor e InitializeComponent dentro
//      del host, en modo diseño. Es MÁS exigente que VS, que no corre el constructor del
//      formulario. Si falla en el evento Load (que VS no ejecuta), se anota «LOAD» aparte.
//   2b. Cada formulario solo con su constructor, sin mostrarlo: InitializeComponent sin Load.
//   3. Cada tipo de control que crean los Designer, creado por el host sobre la base.
// C# 5. Salida: el archivo que recibe como argumento.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

static class Disenador
{
    static List<string> log = new List<string>();
    static int fallas, cargasEnLoad;

    static void Anotar(bool ok, string texto) { log.Add((ok ? "OK     " : "FALLA  ") + texto); if (!ok) fallas++; }

    [STAThread]
    static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Type baseComun = typeof(Visual.FormularioEventAura);
        log.Add("CX-03 (host de diseño) — " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        log.Add("");

        log.Add("1. La clase base como raíz del diseñador (lo que hace Visual Studio)");
        Cargar(baseComun, null);

        log.Add("");
        log.Add("2. Cada formulario como raíz: constructor e InitializeComponent en modo diseño");
        var formularios = baseComun.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseComun)).OrderBy(t => t.Name).ToList();
        foreach (Type f in formularios) Cargar(f, null);

        log.Add("");
        log.Add("2b. Cada formulario solo con su constructor (InitializeComponent, sin mostrarlo ni correr Load)");
        foreach (Type f in formularios)
        {
            try
            {
                using (var form = (Form)Activator.CreateInstance(f))
                    Anotar(true, string.Format("{0,-26} construido; controles creados {1}", f.Name, Contar(form)));
            }
            catch (Exception ex) { Anotar(false, f.Name + ": " + Mensaje(ex)); }
        }

        log.Add("");
        log.Add("3. Cada tipo de control de los Designer, creado por el host sobre la base");
        Type[] tipos = { typeof(Label), typeof(Button), typeof(TextBox), typeof(DataGridView), typeof(CheckBox),
                         typeof(DateTimePicker), typeof(ComboBox), typeof(RadioButton), typeof(Panel), typeof(ListBox),
                         typeof(LinkLabel), typeof(Timer) };
        Cargar(baseComun, tipos);

        log.Add("");
        log.Add("Formularios: " + formularios.Count + ". Fallas: " + fallas + ". Fallas en Load (fuera de lo que ejecuta Visual Studio): " + cargasEnLoad + ".");
        File.WriteAllLines(args[0], log, new UTF8Encoding(false));
        return fallas == 0 ? 0 : 1;
    }

    static void Cargar(Type raiz, Type[] hijos)
    {
        try
        {
            using (var superficie = new DesignSurface())
            {
                superficie.BeginLoad(raiz);
                if (superficie.LoadErrors.Count > 0)
                {
                    foreach (object e in superficie.LoadErrors)
                        {
                        // Si nace en el Load del formulario, Visual Studio no lo ejecuta (instancia la base):
                        // se registra aparte, sin contarlo como falla ni como OK.
                        bool enLoad = e is Exception && Pila((Exception)e).Any(l => l.Contains("_Load(")) && Pila((Exception)e).Any(l => l.Contains("OnLoad"));
                        if (enLoad) { log.Add("LOAD   " + raiz.Name + ": falla en su evento Load, que el diseñador de Visual Studio no ejecuta: " + Mensaje((Exception)e)); cargasEnLoad++; }
                        else Anotar(false, raiz.Name + ": error de carga: " + (e is Exception ? Mensaje((Exception)e) : e.ToString()));
                        if (e is Exception) foreach (string l in Pila((Exception)e)) log.Add("         " + l);
                    }
                    return;
                }
                var host = (IDesignerHost)superficie.GetService(typeof(IDesignerHost));
                var form = host.RootComponent as Form;
                bool enDiseno = form != null && form.Site != null && form.Site.DesignMode;
                var disenador = host.GetDesigner(host.RootComponent);
                // La vista es lo que el diseñador dibuja: obtenerla fuerza a crear el control raíz.
                var vista = superficie.View as Control;
                if (hijos == null)
                {
                    Anotar(form != null && enDiseno && disenador != null && vista != null,
                        string.Format("{0,-26} raíz {1}; modo diseño {2}; diseñador {3}; controles creados {4}",
                            raiz.Name, form == null ? "—" : form.GetType().Name, enDiseno ? "sí" : "NO",
                            disenador == null ? "NO" : disenador.GetType().Name, form == null ? 0 : Contar(form)));
                    return;
                }
                foreach (Type t in hijos)
                {
                    try
                    {
                        var c = host.CreateComponent(t);
                        var control = c as Control;
                        if (control != null) form.Controls.Add(control);
                        Anotar(host.GetDesigner(c) != null || !(c is Control),
                            string.Format("{0,-26} creado y con diseñador {1}", t.Name,
                                host.GetDesigner(c) == null ? "—" : host.GetDesigner(c).GetType().Name));
                    }
                    catch (Exception ex) { Anotar(false, t.Name + ": " + Mensaje(ex)); }
                }
            }
        }
        catch (Exception ex) { Anotar(false, raiz.Name + ": " + Mensaje(ex)); }
    }

    // Las líneas de la pila que son del código de EventAura (Visual, Negocio, Datos).
    static IEnumerable<string> Pila(Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        return (ex.StackTrace ?? "").Split('\n').Select(l => l.Trim())
            .Where(l => l.StartsWith("en Visual.") || l.StartsWith("en Negocio.") || l.StartsWith("en Datos.") || l.StartsWith("at Visual.") || l.Contains("OnLoad") || l.Contains("CreateHandle"))
            .Select(l => System.Text.RegularExpressions.Regex.Replace(l, @" en [A-Z]:\\.*$| in [A-Z]:\\.*$", "")).Take(6);
    }

    static int Contar(Control c) { int n = 0; foreach (Control h in c.Controls) n += 1 + Contar(h); return n; }

    static string Mensaje(Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        return ex.GetType().Name + ": " + ex.Message;
    }
}
