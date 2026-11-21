using System.Windows.Forms;
using System.Linq;
using Negocio;
using System;

namespace Visual
{
    public static class Avisos
    {
        public static void Regla(string titulo, ReglaNegocioException ex)
        {
            Form formulario = Application.OpenForms.Cast<Form>().LastOrDefault(f => !f.IsDisposed && f.Visible);
            if (formulario != null) AvisosEnPantalla.Mostrar(formulario, ex.Message, true);
        }

        public static void Regla(Form formulario, string titulo, ReglaNegocioException ex)
        {
            if (formulario != null && !formulario.IsDisposed)
                AvisosEnPantalla.Mostrar(formulario, ex.Message, true);
        }

        public static void Informativo(Form formulario, string texto) => AvisosEnPantalla.Mostrar(formulario, texto);
        public static void Informativo(Form formulario, string texto, int duracionMs) => AvisosEnPantalla.Mostrar(formulario, texto, false, duracionMs);
        public static void Error(Form formulario, string texto) => AvisosEnPantalla.Mostrar(formulario, texto, true);
        public static IDisposable Esperar(Form formulario, Control control) => new EstadoOperacion(formulario, control);

        private sealed class EstadoOperacion : IDisposable
        {
            private readonly Form formulario;
            private readonly Control control;
            private readonly bool estabaHabilitado;
            private readonly bool esperaAnterior;
            private readonly Cursor cursorAnterior;

            public EstadoOperacion(Form formulario, Control control)
            {
                this.formulario = formulario;
                this.control = control;
                estabaHabilitado = control != null && control.Enabled;
                esperaAnterior = formulario != null && formulario.UseWaitCursor;
                cursorAnterior = Cursor.Current;
                if (control != null) control.Enabled = false;
                if (formulario != null)
                {
                    formulario.UseWaitCursor = true;
                    formulario.Refresh();
                }
                Cursor.Current = Cursors.WaitCursor;
            }

            public void Dispose()
            {
                if (control != null && !control.IsDisposed) control.Enabled = estabaHabilitado;
                if (formulario != null && !formulario.IsDisposed) formulario.UseWaitCursor = esperaAnterior;
                Cursor.Current = cursorAnterior ?? Cursors.Default;
            }
        }
    }
}
