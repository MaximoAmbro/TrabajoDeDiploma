using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    internal static class InterfazGrupoA
    {
        public static T ControlDe<T>(Control padre, string nombre) where T : Control
        {
            return (T)padre.Controls.Find(nombre, true).Single();
        }

        public static void Mostrar(Form formulario)
        {
            formulario.Show();
            Application.DoEvents();
        }

        public static void EnSta(Action accion)
        {
            Fake.Limpiar();
            GestorSesion.Instance.Cerrar();
            Exception error = null;
            var hilo = new Thread(() =>
            {
                try { Application.EnableVisualStyles(); accion(); }
                catch (Exception ex) { error = ex; }
                finally
                {
                    foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                        if (!form.InvokeRequired) form.Dispose();
                    GestorSesion.Instance.Cerrar();
                }
            }) { IsBackground = true };
            hilo.SetApartmentState(ApartmentState.STA);
            hilo.Start();
            int segundosMaximos = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EVENTAURA_CAPTURAS_UI")) ? 30 : 180;
            Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(segundosMaximos)), "La interfaz no terminó en " + segundosMaximos + " segundos.");
            Fake.Limpiar();
            if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        }
    }
}
