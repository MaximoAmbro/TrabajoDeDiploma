using System;
using Datos;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasCuentas
    {
        [TestInitialize]
        public void Preparar()
        {
            Fake.Limpiar();
            GestorSesion.Instance.Cerrar();
        }

        [TestCleanup]
        public void Terminar()
        {
            GestorSesion.Instance.Cerrar();
        }

        [ClassCleanup]
        public static void LimpiarAlFinal()
        {
            Fake.Limpiar();
        }

        // CP-26 — RF-01, RN-01, RN-03: registro Bronce, sin suscripción y con correo único.
        [TestMethod("CP-26: Registro Bronce, sin suscripción y correo único")]
        public void CP26_RegistroBronceSinSuscripcionYCorreoUnico()
        {
            string mail = Fake.MailNuevo();
            GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "Cliente");
            Usuario usuario = RepositorioUsuarios.Instance.ObtenerPorMail(mail);

            Assert.IsNotNull(usuario);
            GestorSesion.Instance.Iniciar(usuario);
            Assert.AreEqual(NivelFidelidad.Bronce, GestorFidelizacion.Instance.ObtenerCliente(usuario.Id).Nivel);
            Assert.IsNull(GestorSuscripciones.Instance.ActivaDe(usuario.Id));
            ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "Cliente"));
            Assert.AreEqual("RN-01", error.Regla);
        }

        // CP-27 — RF-02: autenticación con correo normalizado y sin exponer la contraseña.
        [TestMethod("CP-27: Inicio de sesión correcto con espacios en el correo")]
        public void CP27_InicioDeSesionCorrecto()
        {
            string mail = Fake.MailNuevo();
            GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "Cliente");

            Usuario usuario = GestorUsuarios.Instance.Autenticar("  " + mail + "  ", "Cliente123");

            Assert.IsNotNull(usuario);
            Assert.AreEqual("Cliente", usuario.TipoUsuario);
            Assert.IsNull(usuario.Contraseña);
            Assert.IsNotNull(GestorSesion.Instance.UsuarioActual);
            Assert.AreEqual(usuario.Id, GestorSesion.Instance.UsuarioActual.Id);
        }

        // CP-28 — RF-02, RN-04: intentos restantes, bloqueo configurado y recuperación al vencer.
        [TestMethod("CP-28: Bloqueo por intentos y recuperación tras el vencimiento")]
        public void CP28_BloqueoTrasIntentosFallidos()
        {
            string mail = Fake.MailNuevo();
            GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "Cliente");
            int maximo = GestorParametros.Instance.ObtenerEntero("LOGIN_INTENTOS_MAX");
            int minutos = GestorParametros.Instance.ObtenerEntero("LOGIN_BLOQUEO_MIN");

            for (int intento = 1; intento < maximo; intento++)
            {
                ReglaNegocioException error = Assert.ThrowsException<ReglaNegocioException>(() =>
                    GestorUsuarios.Instance.Autenticar(mail, "Incorrecta1"));
                Assert.AreEqual("RN-04", error.Regla);
                StringAssert.Contains(error.Message, "Le quedan " + (maximo - intento));
            }

            DateTime antes = DateTime.Now;
            ReglaNegocioException bloqueo = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.Autenticar(mail, "Incorrecta1"));
            DateTime despues = DateTime.Now;
            Assert.AreEqual("RN-04", bloqueo.Regla);
            StringAssert.Contains(bloqueo.Message, "bloqueada");
            DateTime? hasta = RepositorioUsuarios.Instance.ObtenerPorMail(mail).BloqueadoHasta;
            Assert.IsTrue(hasta.HasValue);
            Assert.IsTrue(hasta.Value >= antes.AddMinutes(minutos - 1) && hasta.Value <= despues.AddMinutes(minutos),
                "El vencimiento no corresponde al plazo de bloqueo configurado.");

            ReglaNegocioException correctoPeroBloqueado = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.Autenticar(mail, "Cliente123"));
            Assert.AreEqual("RN-04", correctoPeroBloqueado.Regla);
            StringAssert.Contains(correctoPeroBloqueado.Message, "bloqueada");

            Fake.VencerBloqueo(mail);
            Assert.IsNotNull(GestorUsuarios.Instance.Autenticar(mail, "Cliente123"));
            Usuario recuperado = RepositorioUsuarios.Instance.ObtenerPorMail(mail);
            Assert.AreEqual(0, recuperado.IntentosFallidos);
            Assert.IsNull(recuperado.BloqueadoHasta);
        }

        // CP-45 — RF-01, RN-05: el alta pública de negocio crea exclusivamente clientes Bronce.
        [TestMethod("CP-45: RegistrarCliente crea un cliente Bronce")]
        public void CP45_AltaPublicaDeNegocioCreaClienteBronce()
        {
            string mail = Fake.MailNuevo();
            GestorUsuarios.Instance.RegistrarCliente(mail, "Cliente123", "Nombre", "Prueba");

            Usuario usuario = RepositorioUsuarios.Instance.ObtenerPorMail(mail);
            Assert.IsNotNull(usuario);
            Assert.AreEqual("Cliente", usuario.TipoUsuario);
            GestorSesion.Instance.Iniciar(usuario);
            Assert.AreEqual(NivelFidelidad.Bronce, GestorFidelizacion.Instance.ObtenerCliente(usuario.Id).Nivel);
        }

        // CP-29 — RF-02: datos faltantes o correo inexistente no autentican ni abren sesión.
        [TestMethod("CP-29: Datos faltantes o correo inexistente sin sesión")]
        public void CP29_DatosFaltantesOCorreoInexistente()
        {
            string mail = Fake.MailNuevo();
            GestorUsuarios.Instance.Registrar(mail, "Cliente123", "Nombre", "Prueba", "Cliente");

            Assert.IsNull(GestorUsuarios.Instance.Autenticar(Fake.MailNuevo(), "Cliente123"));
            Assert.IsNull(GestorUsuarios.Instance.Autenticar("", "Cliente123"));
            Assert.IsNull(GestorUsuarios.Instance.Autenticar(mail, ""));
            Assert.IsFalse(GestorSesion.Instance.HaySesion);
        }
    }
}
