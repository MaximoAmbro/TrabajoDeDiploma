using System.Windows.Forms;
using System.Linq;
using Negocio;

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
    }
}
