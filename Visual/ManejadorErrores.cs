using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Visual
{
    // Último recurso ante errores que ninguna pantalla captura: el usuario ve un mensaje en
    // castellano, sin detalles técnicos, en lugar del cuadro de .NET.
    public static class ManejadorErrores
    {
        private static bool instalado;
        private static readonly object escritura = new object();
        private static bool applicationExitRegistrado;
        private static string ultimaVentana = "(ninguna)";
        private static string ultimoCloseReason = "(ninguno)";
        private static bool recorriendoVentanas;
        private static readonly HashSet<Form> formulariosConRegistro = new HashSet<Form>();
        [DllImport("kernel32.dll")] private static extern uint SetErrorMode(uint modo);
        public const string Titulo = "EventAura";
        public const string MensajeBaseDeDatos =
            "No se pudo conectar con la base de datos. Verificá que SQL Server esté en funcionamiento y volvé a intentar.";
        public const string MensajeGeneral =
            "Ocurrió un error inesperado y la operación no se completó. Si se repite, avisá al administrador.";

        // Lo llama Program.Main antes de crear cualquier formulario.
        public static void Instalar()
        {
            if (instalado) return;
            instalado = true;
            // Mantiene el manejo de errores críticos de Windows, pero permite que WER registre
            // las caídas nativas (evento Application Error 1000).
            SetErrorMode(0x0001);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Mostrar(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                RegistrarOrigen("excepción no controlada", ex == null ? "Excepción sin detalle" : ex.GetType().FullName + Environment.NewLine + ex);
                Mostrar(ex);
            };
            Application.Idle += ConectarRegistroDeCierres;
            Application.ApplicationExit += (s, e) => RegistrarCierreAplicacion();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => RegistrarFinProceso();
        }

        private static void ConectarRegistroDeCierres(object sender, EventArgs e)
        {
            if (recorriendoVentanas) return;
            recorriendoVentanas = true;
            try
            {
                foreach (Form formulario in Application.OpenForms)
                {
                    if (formulario == null || formulario.IsDisposed || !formulariosConRegistro.Add(formulario)) continue;
                    formulario.FormClosed += RegistrarVentanaCerrada;
                }
            }
            finally { recorriendoVentanas = false; }
        }

        private static void RegistrarVentanaCerrada(object sender, FormClosedEventArgs e)
        {
            var formulario = sender as Form;
            if (formulario == null) return;
            ultimaVentana = formulario.Name + " (" + formulario.Text + ")";
            ultimoCloseReason = e.CloseReason.ToString();
            RegistrarOrigen("cierre de ventana", "ventana=" + ultimaVentana + "; CloseReason=" + ultimoCloseReason);
        }

        private static void RegistrarCierreAplicacion()
        {
            applicationExitRegistrado = true;
            string traza = Environment.StackTrace;
            string origen = traza.IndexOf("ControlSesionInactiva", StringComparison.OrdinalIgnoreCase) >= 0
                ? "inactividad"
                : traza.IndexOf("FormularioEventAura.OnFormClosed", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "última ventana / menú"
                    : "orden interna de cierre";
            RegistrarOrigen(origen, "Application.ApplicationExit; última ventana=" + ultimaVentana +
                "; CloseReason=" + ultimoCloseReason + Environment.NewLine + traza);
        }

        private static void RegistrarFinProceso()
        {
            string origen = applicationExitRegistrado ? "fin de proceso posterior al cierre ordenado" : "fin externo o caída nativa (sin ApplicationExit)";
            RegistrarOrigen(origen, "AppDomain.ProcessExit; última ventana=" + ultimaVentana +
                "; CloseReason=" + ultimoCloseReason + "; ExitCode=" + Environment.ExitCode);
        }

        private static void RegistrarOrigen(string origen, string detalle)
        {
            try
            {
                string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EventAura");
                Directory.CreateDirectory(carpeta);
                string linea = DateTime.Now.ToString("O") + " [origen: " + origen + "] " + detalle + Environment.NewLine;
                lock (escritura) File.AppendAllText(Path.Combine(carpeta, "errores.log"), linea);
            }
            catch (IOException) { System.Diagnostics.Trace.WriteLine("No se pudo escribir el registro de cierres de EventAura."); }
            catch (UnauthorizedAccessException) { System.Diagnostics.Trace.WriteLine("Sin permiso para escribir el registro de cierres de EventAura."); }
        }

        public static string MensajeParaUsuario(Exception ex)
        {
            if (ex is Negocio.ReglaNegocioException regla) return regla.Message;
            if (ex is System.Data.Common.DbException) return MensajeBaseDeDatos;
            if (ex is InvalidOperationException && ex.Message != null)
            {
                if (ex.Message == "No se encontró ningún evento con ese ID.")
                    return "Ese evento ya no está disponible. Actualizá la pantalla e intentá de nuevo.";
                if (ex.Message == "No se encontró ningún usuario con ese ID.")
                    return "No encontramos esa cuenta. Revisá los datos e intentá de nuevo.";
                if (ex.Message == "No se encontró ningún sector con ese ID.")
                    return "Ese sector ya no está disponible. Actualizá la pantalla e intentá de nuevo.";
            }
            return MensajeGeneral;
        }

        public static void Mostrar(Exception ex)
        {
            Registrar(ex);
            var formulario=Application.OpenForms.Cast<Form>().FirstOrDefault(f=>!f.IsDisposed && f.Visible);
            if(formulario!=null) AvisosEnPantalla.Mostrar(formulario,MensajeParaUsuario(ex),true);
        }

        public static void Registrar(Exception ex)
        {
            try {
                string carpeta=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"EventAura");
                Directory.CreateDirectory(carpeta);
                // Tipo y traza permiten diagnosticar sin persistir contraseñas u otros valores de entrada.
                string detalle=ex==null?"Excepción sin detalle":ex.GetType().FullName+Environment.NewLine+ex.StackTrace;
                lock(escritura) File.AppendAllText(Path.Combine(carpeta,"errores.log"),DateTime.Now.ToString("O")+" "+detalle+Environment.NewLine);
            } catch(IOException) { System.Diagnostics.Trace.WriteLine("No se pudo escribir el registro de errores de EventAura."); }
              catch(UnauthorizedAccessException) { System.Diagnostics.Trace.WriteLine("Sin permiso para escribir el registro de errores de EventAura."); }
        }
    }
}
