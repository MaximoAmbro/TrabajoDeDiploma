using System;
using System.Linq;
using System.Windows.Forms;
using Datos;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasGrupoALogin
    {
        // CP-66 — RF-02, RN-05; PA-01, filas 3.1/C.1: foco y navegación real del administrador.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-66: Login inicia con foco en el correo")]
        [DataRow(true, DisplayName = "CP-66: Administrador entra al menú de propietario")]
        public void CP66_FocoYMenuDelAdministrador(bool iniciar)
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmInicio())
                {
                    Mostrar(form);
                    TextBox mail = ControlDe<TextBox>(form, "txtMailLogin");
                    if (!iniciar) { Assert.AreSame(mail, form.ActiveControl); return; }
                    Usuario anterior = RepositorioUsuarios.Instance.ObtenerPorMail("admin@eventaura.com");
                    try
                    {
                        mail.Text = anterior.Mail;
                        ControlDe<TextBox>(form, "txtContraseñaLogin").Text = "Admin1234";
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btninicio").PerformClick());
                        Visual.frmMenuPropietario menu = Application.OpenForms.OfType<Visual.frmMenuPropietario>().Single(f => f.Visible);
                        Assert.AreEqual(anterior.Id, menu.IDUsuario);
                        Assert.IsFalse(form.Visible);
                        Assert.AreEqual(anterior.Id, GestorSesion.Instance.UsuarioActual.Id);
                        Assert.AreEqual("Propietario", GestorSesion.Instance.UsuarioActual.TipoUsuario);
                    }
                    finally
                    {
                        RepositorioUsuarios.Instance.ActualizarContrasena(anterior.Mail, anterior.Contraseña);
                        RepositorioUsuarios.Instance.ActualizarIntentos(anterior.Id, anterior.IntentosFallidos, anterior.BloqueadoHasta);
                    }
                }
            });
        }

        // CP-67 — RF-02; PA-02, 3.2: doble acceso y migración, sin credenciales literales nuevas.
        [TestMethod("CP-67: Doble acceso del administrador y migración de credencial heredada")]
        public void CP67_DobleAccesoYMigracion()
        {
            EnSta(() =>
            {
                Usuario anterior = RepositorioUsuarios.Instance.ObtenerPorMail("admin@eventaura.com");
                try
                {
                    // SEG-5: la semilla ya guarda hashes; la migración heredada se prueba abajo con Fake.
                    string clave = "Admin1234";
                    Assert.IsTrue(Seguridad.EsHash(anterior.Contraseña), "La semilla no debe contener contraseñas en texto plano.");
                    string hashAnterior = null;
                    for (int acceso = 0; acceso < 2; acceso++)
                    {
                        GestorSesion.Instance.Cerrar();
                        Usuario autenticado = GestorUsuarios.Instance.Autenticar(anterior.Mail, clave);
                        Assert.IsNotNull(autenticado);
                        Assert.AreEqual(anterior.Id, autenticado.Id);
                        Assert.IsNull(autenticado.Contraseña);
                        string guardada = RepositorioUsuarios.Instance.ObtenerPorMail(anterior.Mail).Contraseña;
                        Assert.IsTrue(guardada != clave && Seguridad.EsHash(guardada), "La credencial debe persistirse hasheada.");
                        Assert.IsTrue(Seguridad.Verificar(clave, guardada));
                        if (hashAnterior != null) Assert.IsTrue(hashAnterior == guardada, "El segundo acceso debe usar el hash persistido.");
                        hashAnterior = guardada;
                    }
                }
                finally
                {
                    RepositorioUsuarios.Instance.ActualizarContrasena(anterior.Mail, anterior.Contraseña);
                    RepositorioUsuarios.Instance.ActualizarIntentos(anterior.Id, anterior.IntentosFallidos, anterior.BloqueadoHasta);
                }

                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                string heredada = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                RepositorioUsuarios.Instance.ActualizarContrasena(cliente.Mail, heredada);
                Assert.IsFalse(Seguridad.EsHash(RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña));
                Assert.IsNotNull(GestorUsuarios.Instance.Autenticar(cliente.Mail, heredada));
                string hash = RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña;
                Assert.IsTrue(hash != heredada && Seguridad.EsHash(hash), "La cuenta aislada debe migrar su credencial.");
                Assert.IsTrue(Seguridad.Verificar(heredada, hash));
            });
        }

        // CP-68 — RF-02, RN-04; PA-03, C.2/C.3: mensajes de intentos y minutos desde la pantalla.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-68: Login muestra intentos restantes y bloqueo al quinto fallo")]
        [DataRow(true, DisplayName = "CP-68: Login muestra diez y cinco minutos de bloqueo")]
        public void CP68_AvisosDeIntentosYBloqueo(bool minutos)
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Assert.AreEqual(5, GestorParametros.Instance.ObtenerEntero("LOGIN_INTENTOS_MAX"));
                Assert.AreEqual(15, GestorParametros.Instance.ObtenerEntero("LOGIN_BLOQUEO_MIN"));
                try
                {
                    using (var form = new Visual.frmInicio())
                    {
                        Mostrar(form);
                        ControlDe<TextBox>(form, "txtMailLogin").Text = cliente.Mail;
                        ControlDe<TextBox>(form, "txtContraseñaLogin").Text = "Incorrecta1";
                        if (minutos)
                        {
                            foreach (int restantes in new[] { 10, 5 })
                            {
                                // Preparar con precisión de segundos evita que SQL datetime redondee
                                // hacia arriba unos milisegundos y convierta ceil(5 min) en 6.
                                DateTime ahora = DateTime.Now;
                                DateTime hasta = ahora.AddTicks(-(ahora.Ticks % TimeSpan.TicksPerSecond)).AddMinutes(restantes);
                                RepositorioUsuarios.Instance.ActualizarIntentos(cliente.Id, 5, hasta);
                                DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btninicio").PerformClick(),
                                    new DialogoEsperado { Titulo = "Inicio de sesión", Texto = "en " + restantes + " minutos." });
                            }
                        }
                        else
                        {
                            for (int intento = 1; intento <= 5; intento++)
                            {
                                int restantes = 5 - intento;
                                string texto = intento == 5 ? "en 15 minutos." :
                                    "Te quedan " + restantes + (restantes == 1 ? " intento antes" : " intentos antes");
                                DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btninicio").PerformClick(),
                                    new DialogoEsperado { Titulo = "Inicio de sesión", Texto = texto });
                            }
                            Usuario bloqueado = RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail);
                            Assert.AreEqual(5, bloqueado.IntentosFallidos);
                            Assert.IsTrue(bloqueado.BloqueadoHasta > DateTime.Now);
                        }
                        Assert.IsTrue(form.Visible);
                        Assert.IsFalse(GestorSesion.Instance.HaySesion);
                    }
                }
                finally { RepositorioUsuarios.Instance.ActualizarIntentos(cliente.Id, 0, null); }
            });
        }
    }
}
