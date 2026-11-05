// REGISTRO DEL «ANTES» de T-27: no instala ManejadorErrores, para mostrar lo que veía el usuario
// sin él (el cuadro de .NET). Lo que ve ahora la aplicación real con la base caída lo comprueba
// VerificarBaseCaida.ps1 (CX-04).
// ¿Qué ve el usuario ante un error que nadie captura? Corre la aplicación real con su bucle de
// mensajes (Application.Run, igual que Program.cs), contra una base INEXISTENTE (ver el .config
// que arma VerificarErrorNoControlado.ps1), e intenta iniciar sesión. Registra la ventana que
// aparece y la cierra. No envía teclas al escritorio. C# 5.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

static class ErrorNoControlado
{
    [STAThread]
    static void Main(string[] args)
    {
        var log = new StringBuilder();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var inicio = new Visual.frmInicio();
        int etapa = 0;
        var t = new Timer { Interval = 400 };
        t.Tick += (s, e) =>
        {
            if (etapa == 0)
            {
                etapa = 1;
                inicio.Controls.Find("txtMailLogin", true)[0].Text = "plata@eventaura.com";
                inicio.Controls.Find("txtContraseñaLogin", true)[0].Text = "Cliente123";
                log.AppendLine("1. Login con la base de datos inaccesible: clic en «Iniciar sesión».");
                ((Button)inicio.Controls.Find("btninicio", true)[0]).PerformClick();   // la excepción sube al bucle
                return;
            }
            if (etapa >= 1 && etapa < 20)
            {
                etapa++;
                var dialogo = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f != inicio && f.Visible);
                if (dialogo == null) return;
                log.AppendLine("2. Ventana que ve el usuario: tipo " + dialogo.GetType().FullName + ", título «" + dialogo.Text + "».");
                foreach (Control c in Todos(dialogo))
                    if ((c is Label || c is Button || c is TextBox) && c.Visible && c.Text.Trim().Length > 0)
                        log.AppendLine("   " + c.GetType().Name + ": «" + c.Text.Replace("\r", "").Replace("\n", " ⏎ ").Substring(0, Math.Min(240, c.Text.Length)) + "»");
                dialogo.Close();
                etapa = 99;
                t.Stop();
                Application.Exit();
            }
            if (etapa == 20) { log.AppendLine("2. No apareció ninguna ventana de error."); t.Stop(); Application.Exit(); }
        };
        t.Start();
        Application.Run(inicio);
        File.WriteAllText(Path.Combine(args[0], "error-no-controlado.txt"), log.ToString(), new UTF8Encoding(false));
    }

    static System.Collections.Generic.IEnumerable<Control> Todos(Control p)
    {
        foreach (Control c in p.Controls) { yield return c; foreach (Control h in Todos(c)) yield return h; }
    }
}
