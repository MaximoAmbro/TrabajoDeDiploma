using System;
using System.Linq;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    internal sealed class ControlSesionInactiva : IMessageFilter
    {
        private static ControlSesionInactiva instancia;
        private readonly Timer reloj = new Timer { Interval = 1000 };
        private bool avisoMostrado;

        private ControlSesionInactiva()
        {
            Application.AddMessageFilter(this);
            reloj.Tick += (s, e) => Revisar();
            reloj.Start();
        }

        public static void Instalar()
        {
            if (instancia == null) instancia = new ControlSesionInactiva();
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x0200 || m.Msg == 0x0201 || m.Msg == 0x0204 || m.Msg == 0x0100 || m.Msg == 0x0101)
            {
                GestorSesion.Instance.RegistrarActividad();
                avisoMostrado = false;
            }
            return false;
        }

        private void Revisar()
        {
            var sesion = GestorSesion.Instance;
            var ventana = Application.OpenForms.Cast<Form>().FirstOrDefault(f => !f.IsDisposed && f.Visible);
            if (sesion.UsuarioActual == null || ventana == null) { avisoMostrado = false; return; }
            DateTime ahora = DateTime.Now;
            if (sesion.DebeCerrarPorInactividad(ahora))
            {
                sesion.Cerrar();
                avisoMostrado = false;
                var login = Application.OpenForms.Cast<Form>().OfType<frmInicio>().FirstOrDefault();
                if (login == null) { login = new frmInicio(); login.Show(); } else { login.Show(); login.BringToFront(); }
                foreach (Form form in Application.OpenForms.Cast<Form>().Where(f => f != login).ToArray()) form.Close();
                AvisosEnPantalla.Mostrar(login ?? Application.OpenForms.Cast<Form>().FirstOrDefault(), "Cerramos tu sesión por inactividad. Volvé a ingresar para seguir.", true);
                return;
            }
            if (!avisoMostrado && sesion.DebeAvisarInactividad(ahora))
            {
                avisoMostrado = true;
                int segundos = GestorParametros.Instance.ObtenerEntero("SESION_AVISO_SEG");
                AvisosEnPantalla.Mostrar(ventana, "Tu sesión se cerrará por inactividad dentro de " + segundos + " segundos. Usá la pantalla para continuar.");
            }
        }
    }
}
