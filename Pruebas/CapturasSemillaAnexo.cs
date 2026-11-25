using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Pruebas
{
    // Regenera vistas del anexo con las cuentas, locales y eventos de 02-semilla.sql.
    public static class CapturasSemillaAnexo
    {
        public static void Generar(string carpeta)
        {
            Directory.CreateDirectory(carpeta);
            BaseDePrueba.ConfigurarErroresDePrueba();
            BaseDePrueba.Preparar(null);

            var organizador = Autenticar("organizador@eventaura.com");
            var local = GestorLocales.Instance.PorPropietario(organizador.Id).Single();
            var evento = GestorEventos.Instance.PorLocal(local.Id).Single(e => e.Nombre == "Duki en Rosario");
            CapturarComo(organizador, "Organizador", () =>
            {
                Capturar(carpeta, "frmMenuPropietario.png", () => new Visual.frmMenuPropietario { IDUsuario = organizador.Id, NombreUsuario = organizador.Nombre });
                Capturar(carpeta, "frmMisLocales.png", () => new Visual.frmMisLocales { IDUsuario = organizador.Id, NombreUsuario = organizador.Nombre });
                Capturar(carpeta, "frmEventosLocal.png", () => new Visual.Vendedor.frmEventosLocal
                {
                    IDUsuario = organizador.Id, IDlocal = local.Id, NombreUsuario = organizador.Nombre, NombreLocal = local.Nombre
                });
                CapturarPanelEventos(carpeta, organizador, local);
                Capturar(carpeta, "frmRegistrarLocal.png", () => new Visual.frmRegistrarLocal { IDUsuario = organizador.Id, NombreUsuario = organizador.Nombre });
                Capturar(carpeta, "frmAgregarEvento.png", () => new Visual.Propietario.frmAgregarEvento
                {
                    IDUsuario = organizador.Id, IDlocal = local.Id, NombreUsuario = organizador.Nombre, NombreLocal = local.Nombre
                });
                Capturar(carpeta, "frmAgregarSector.png", () => new Visual.Vendedor.frmAgregarSector
                {
                    IDUsuario = organizador.Id, IDlocal = local.Id, IDEvento = evento.Id,
                    NombreUsuario = organizador.Nombre, NombreLocal = local.Nombre, NombreEvento = evento.Nombre
                });
                Capturar(carpeta, "frmAdministrarEvento.png", () => new Visual.Vendedor.frmAdministrarEvento
                {
                    IDUsuario = organizador.Id, IDlocal = local.Id, IDEvento = evento.Id,
                    NombreUsuario = organizador.Nombre, NombreLocal = local.Nombre, NombreEvento = evento.Nombre
                });
                Capturar(carpeta, "frmControlIngreso.png", () => new Visual.frmControlIngreso());
                Capturar(carpeta, "frmListasEspera.png", () => new Visual.frmListasEspera
                {
                    IDUsuario = organizador.Id, NombreUsuario = organizador.Nombre
                });
            });

            var administrador = Autenticar("admin@eventaura.com", "Admin1234");
            CapturarComo(administrador, "Administrador", () =>
            {
                Capturar(carpeta, "frmMenuAdministrador.png", () => new Visual.frmMenuAdministrador { IDUsuario = administrador.Id, NombreUsuario = administrador.Nombre });
                Capturar(carpeta, "frmReportes.png", () => new Visual.frmReportes { IDUsuario = administrador.Id, NombreUsuario = administrador.Nombre }, form =>
                    form.Controls.Find("btnGenerar", true).OfType<Button>().Single().PerformClick());
                Capturar(carpeta, "frmAuditoria.png", () => new Visual.frmAuditoria());
                Capturar(carpeta, "frmRevisarSolicitudesOrganizador.png", () => new Visual.frmRevisarSolicitudesOrganizador());
                Capturar(carpeta, "frmRevisarEventos.png", () => new Visual.frmRevisarEventos());
                Capturar(carpeta, "frmCrearAdministrador.png", () => new Visual.frmCrearUsuario(true));
            });

            // Estas dos vistas son previas al inicio de sesión; la barra debe quedar solo con la marca.
            GestorSesion.Instance.Cerrar();
            Capturar(carpeta, "frmCrearUsuario.png", () => new Visual.frmCrearUsuario());
            Capturar(carpeta, "frmRecuperarContraseña.png", () => new Visual.frmCambiarContraseña(true), form =>
            {
                form.Controls.Find("txtMail", true).OfType<TextBox>().Single().Text = "bronce@eventaura.com";
                form.Controls.Find("btnEnviarCodigo", true).OfType<Button>().Single().PerformClick();
            });

            var cliente = Autenticar("bronce@eventaura.com");
            CapturarComo(cliente, "Cliente", () =>
            {
                Capturar(carpeta, "frmCambiarContraseña.png", () => new Visual.frmCambiarContraseña());
                Capturar(carpeta, "frmSolicitudOrganizador.png", () => new Visual.frmSolicitudOrganizador(), form =>
                {
                    form.Controls.Find("telefono", true).OfType<TextBox>().Single().Text = "+5493410000123";
                    form.Controls.Find("btnEnviarCodigoTelefono", true).OfType<Button>().Single().PerformClick();
                });
            });
            GestorSesion.Instance.Cerrar();
        }

        private static Entidades.Usuario Autenticar(string correo, string contraseña = "Cliente123")
        {
            GestorSesion.Instance.Cerrar();
            var usuario = GestorUsuarios.Instance.Autenticar(correo, contraseña);
            if (usuario == null) throw new InvalidOperationException("No se pudo autenticar la cuenta sembrada " + correo + ".");
            return usuario;
        }

        private static void CapturarComo(Entidades.Usuario usuario, string perfil, Action capturas)
        {
            GestorSesion.Instance.Cerrar();
            string contraseña = usuario.Mail == "admin@eventaura.com" ? "Admin1234" : "Cliente123";
            var autenticado = GestorUsuarios.Instance.Autenticar(usuario.Mail, contraseña);
            if (autenticado == null || autenticado.TipoUsuario != perfil)
                throw new InvalidOperationException("La cuenta de captura no tiene el perfil esperado: " + perfil + ".");
            try { capturas(); }
            finally { GestorSesion.Instance.Cerrar(); }
        }

        private static void Capturar(string carpeta, string archivo, Func<Form> crear, Action<Form> preparar = null)
        {
            using (var form = crear())
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(24, 24);
                form.Show();
                Application.DoEvents();
                preparar?.Invoke(form);
                Application.DoEvents();
                form.Refresh();
                Application.DoEvents();
                using (var imagen = new Bitmap(form.Width, form.Height))
                {
                    CapturasTandaVisual.DibujarCompleta(form, imagen);
                    imagen.Save(Path.Combine(carpeta, archivo));
                }
            }
            foreach (Form abierta in Application.OpenForms.Cast<Form>().ToArray()) abierta.Dispose();
        }

        private static void CapturarPanelEventos(string carpeta, Entidades.Usuario organizador, Local local)
        {
            string raiz = Directory.GetParent(Directory.GetParent(carpeta).FullName).FullName;
            string destino = Path.Combine(raiz, "paneles", "Fase8b", "frmEventosLocal", "tabla-eventos.png");
            Directory.CreateDirectory(Path.GetDirectoryName(destino));
            using (var form = new Visual.Vendedor.frmEventosLocal
            {
                IDUsuario = organizador.Id, IDlocal = local.Id, NombreUsuario = organizador.Nombre, NombreLocal = local.Nombre
            })
            {
                form.Show();
                Application.DoEvents();
                var grilla = form.Controls.Find("dgvEventos", true).OfType<DataGridView>().Single();
                using (var imagen = new Bitmap(grilla.Width, grilla.Height))
                {
                    grilla.DrawToBitmap(imagen, new Rectangle(Point.Empty, grilla.Size));
                    imagen.Save(destino);
                }
            }
        }
    }
}
