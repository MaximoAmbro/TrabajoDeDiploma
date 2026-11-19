using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Datos;
using Entidades;
using Negocio;

namespace Pruebas
{
    // Capturas completas dentro del contenedor de navegación real del cliente.
    // Se invoca manualmente desde CapturarMiCuenta2026.ps1; no agrega casos MSTest.
    public static class CapturasMiCuenta2026
    {
        public static void Generar(string salida)
        {
            Directory.CreateDirectory(salida);
            BaseDePrueba.Preparar(null);
            PruebasSeguridad.EnStaSinCambiarDatos(() =>
            {
                var usuario = GestorUsuarios.Instance.Autenticar("plata@eventaura.com", "Cliente123");
                if (usuario == null) throw new InvalidOperationException("No se pudo iniciar sesión con Paula.");
                Cliente cliente = GestorFidelizacion.Instance.ObtenerCliente(usuario.Id);
                Evento evento = GestorEventos.Instance.Buscar("Duki", null, null).Single();
                Fake.CompraConfirmada(cliente, evento, 1, true);
                using (var menu = new Visual.frmMenuCliente { IDUsuario = usuario.Id, NombreUsuario = usuario.Nombre })
                {
                    menu.StartPosition = FormStartPosition.Manual;
                    menu.Location = new Point(30, 30);
                    menu.Show();
                    Application.DoEvents();
                    menu.Controls.Find("btnEventos", true).OfType<Button>().Single().PerformClick();
                    var alternarVista = (Button)Descendientes(menu).First(c => c.Name == "btnVista");
                    alternarVista.PerformClick();
                    alternarVista.PerformClick();
                    Application.DoEvents();
                    Guardar(menu, Path.Combine(salida, "eventos.png"));

                    var tarjeta = Descendientes(menu).First(c => c.GetType().Name == "TarjetaEvento" && c.AccessibleName == "Duki en Rosario");
                    tarjeta.Controls.Find("verEntradas", true).OfType<Button>().Single().PerformClick();
                    Application.DoEvents();
                    Guardar(menu, Path.Combine(salida, "ficha-evento.png"));
                    var mapa = Descendientes(menu).First(c => c.GetType().Name == "MapaSectores");
                    var zona = (Rectangle)mapa.GetType().GetMethod("Zona").Invoke(mapa, new object[] { 0 });
                    mapa.GetType().GetMethod("OnMouseClick", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(mapa, new object[] { new MouseEventArgs(MouseButtons.Left, 1, zona.Left + zona.Width / 2, zona.Top + zona.Height / 2, 0) });
                    Application.DoEvents();
                    ((Button)Descendientes(menu).First(c => c.Name == "comprarFicha")).PerformClick();
                    Application.DoEvents();
                    Guardar(menu, Path.Combine(salida, "compra.png"));

                    menu.Controls.Find("btnHistorial", true).OfType<Button>().Single().PerformClick();
                    Application.DoEvents();
                    Guardar(menu, Path.Combine(salida, "mis-entradas.png"));
                    menu.Controls.Find("btnSuscripcion", true).OfType<Button>().Single().PerformClick();
                    Application.DoEvents();
                    Guardar(menu, Path.Combine(salida, "suscripcion.png"));
                }
                GestorSesion.Instance.Cerrar();
            });
        }

        private static System.Collections.Generic.IEnumerable<Control> Descendientes(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (var child in Descendientes(c)) yield return child;
            }
        }

        private static void Guardar(Form menu, string ruta)
        {
            menu.Activate(); menu.Refresh(); Application.DoEvents(); Thread.Sleep(200); Application.DoEvents();
            using (var bitmap = new Bitmap(menu.ClientSize.Width, menu.ClientSize.Height))
            {
                Point origen = menu.PointToScreen(Point.Empty);
                using (var graphics = Graphics.FromImage(bitmap))
                    graphics.CopyFromScreen(origen, Point.Empty, menu.ClientSize);
                bitmap.Save(ruta);
            }
        }
    }
}
