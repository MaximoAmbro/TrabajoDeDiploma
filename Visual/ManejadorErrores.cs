using System;
using System.Windows.Forms;

namespace Visual
{
    // Último recurso ante errores que ninguna pantalla captura: el usuario ve un mensaje en
    // castellano, sin detalles técnicos, en lugar del cuadro de .NET.
    public static class ManejadorErrores
    {
        public const string Titulo = "EventAura";
        public const string MensajeBaseDeDatos =
            "No se pudo conectar con la base de datos. Verificá que SQL Server esté en funcionamiento y volvé a intentar.";
        public const string MensajeGeneral =
            "Ocurrió un error inesperado y la operación no se completó. Si se repite, avisá al administrador.";

        // Lo llama Program.Main antes de crear cualquier formulario.
        public static void Instalar()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Mostrar(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Mostrar(e.ExceptionObject as Exception);
        }

        public static string MensajeParaUsuario(Exception ex)
        {
            if (ex is System.Data.Common.DbException) return MensajeBaseDeDatos;
            // Los mensajes que escribe Negocio ya están en castellano y dicen qué pasó.
            if (ex != null && ex.Source == "Repositorio" && !string.IsNullOrWhiteSpace(ex.Message)) return ex.Message;
            return MensajeGeneral;
        }

        public static void Mostrar(Exception ex)
        {
            MessageBox.Show(MensajeParaUsuario(ex), Titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
