using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public static class Avisos
    {
        public static void Regla(string titulo, ReglaNegocioException ex)
        {
            MessageBox.Show(ex.Message + " (" + ex.Regla + ")", titulo,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
