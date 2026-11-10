using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Visual
{
    // Base de los formularios de EventAura. Si el usuario cierra la última ventana visible,
    // termina la aplicación: antes quedaba un proceso sin ventanas (hallazgo U-01).
    public class FormularioEventAura : Form
    {
        public FormularioEventAura()
        {
            Font = Tema.Cuerpo;
            BackColor = Tema.Superficie;
            Icon = Tema.Icono;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                Tema.Normalizar(this);
            base.OnLoad(e);
        }

        public static bool DebeTerminarAplicacion(Form cerrado, CloseReason motivo, IEnumerable<Form> abiertos)
        {
            return motivo == CloseReason.UserClosing &&
                   !abiertos.Any(f => f != cerrado && !f.IsDisposed && f.Visible);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // Application.MessageLoop es false en las pruebas: ahí nunca se termina el proceso.
            if (Application.MessageLoop &&
                DebeTerminarAplicacion(this, e.CloseReason, Application.OpenForms.Cast<Form>().ToList()))
            {
                Application.Exit();
            }
        }
    }
}
