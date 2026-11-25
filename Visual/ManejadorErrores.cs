using System;
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
            // El proceso registra sus errores; no delega en un cuadro modal de Windows.
            SetErrorMode(0x0001 | 0x0002);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Mostrar(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Mostrar(e.ExceptionObject as Exception);
        }

        public static string MensajeParaUsuario(Exception ex)
        {
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
