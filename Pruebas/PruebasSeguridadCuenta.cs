using System;
using System.Windows.Forms;
using Datos;
using Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasSeguridadCuenta
    {
        [TestMethod("CP-167: Una cuenta sin verificar no inicia una compra")]
        public void CP167_CorreoNoVerificadoNoCompra()
        {
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            var evento = Fake.EventoPublicado(1000m, 3);
            Fake.ComoCliente(cliente);
            Ejecutar("UPDATE dbo.Usuario SET CorreoVerificado=0 WHERE ID=" + cliente.Id);
            Assert.AreEqual("RN-43", Assert.ThrowsException<ReglaNegocioException>(() => GestorPaquetes.Instance.Nuevo(evento.Id, cliente.Id)).Regla);
            Assert.AreEqual("RN-43", Assert.ThrowsException<ReglaNegocioException>(() => GestorCompras.Instance.Iniciar(new Paquete { IdEvento = evento.Id }, cliente)).Regla);
        }

        [TestMethod("CP-168: El alta muestra la bandeja simulada y verifica el correo")]
        public void CP168_RegistroVerificaCorreoEnLaInterfaz()
        {
            EnSta(() =>
            {
                string correo = Fake.MailNuevo();
                using (var form = new Visual.frmCrearUsuario())
                {
                    Mostrar(form);
                    ControlDe<TextBox>(form, "txtNombre").Text = "Cliente";
                    ControlDe<TextBox>(form, "txtApellido").Text = "Verificado";
                    ControlDe<TextBox>(form, "txtmail").Text = correo;
                    ControlDe<TextBox>(form, "TxtPassword").Text = "Cliente123";
                    ControlDe<TextBox>(form, "TxtConfirmpassword").Text = "Cliente123";
                    ControlDe<Button>(form, "btnregistrarse").PerformClick();
                    var mensaje = EnviadorSimulado.Ultimo(correo, "Verificacion");
                    Assert.IsNotNull(mensaje);
                    Assert.IsNotNull(form.Controls.Find("bandejaCorreoSimulada", true)[0]);
                    Assert.IsFalse(RepositorioUsuarios.Instance.CorreoVerificado(RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id));
                    ControlDe<TextBox>(form, "txtCodigoCorreo").Text = mensaje.Codigo;
                    ControlDe<Button>(form, "btnConfirmarCorreo").PerformClick();
                    Assert.IsTrue(RepositorioUsuarios.Instance.CorreoVerificado(RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id));
                    Assert.IsTrue(ControlDe<Label>(form, "estadoVerificacion").Text.Contains("Ya podés iniciar sesión y comprar"));
                }
            });
        }

        [TestMethod("CP-169: Un código de recuperación vencido se rechaza sin cambiar la contraseña")]
        public void CP169_CodigoVencido()
        {
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            string anterior = RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña;
            GestorCodigosCorreo.Instance.SolicitarRecuperacion(cliente.Mail);
            var mensaje = EnviadorSimulado.Ultimo(cliente.Mail, "Recuperacion");
            Assert.IsNotNull(mensaje);
            Ejecutar("UPDATE dbo.CodigosCorreo SET VenceEl=DATEADD(MINUTE,-1,SYSUTCDATETIME()) WHERE UsuarioID=" + cliente.Id + " AND Proposito='Recuperacion'");
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorCodigosCorreo.Instance.RecuperarContrasena(cliente.Mail, mensaje.Codigo, "OtraClave123"));
            Assert.AreEqual("RN-44", error.Regla);
            Assert.AreEqual(anterior, RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña);
        }

        [TestMethod("CP-170: La recuperación activa cambia la contraseña con el hash del sistema")]
        public void CP170_RecuperacionActiva()
        {
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            GestorCodigosCorreo.Instance.SolicitarRecuperacion(cliente.Mail);
            var mensaje = EnviadorSimulado.Ultimo(cliente.Mail, "Recuperacion");
            GestorCodigosCorreo.Instance.RecuperarContrasena(cliente.Mail, mensaje.Codigo, "NuevaClave123");
            string hash = RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña;
            Assert.IsTrue(Seguridad.EsHash(hash));
            Assert.IsTrue(Seguridad.Verificar("NuevaClave123", hash));
            Assert.IsFalse(Seguridad.Verificar("Cliente123", hash));
        }

        [TestMethod("CP-171: SMTP no envía si faltan variables de entorno")]
        public void CP171_SmtpSinConfigurarFallaClaro()
        {
            string[] nombres = { "EVENTAURA_SMTP_HOST", "EVENTAURA_SMTP_PORT", "EVENTAURA_SMTP_USER", "EVENTAURA_SMTP_PASSWORD", "EVENTAURA_SMTP_FROM" };
            var anteriores = new string[nombres.Length];
            for (int i=0;i<nombres.Length;i++) { anteriores[i]=Environment.GetEnvironmentVariable(nombres[i]); Environment.SetEnvironmentVariable(nombres[i], null); }
            try
            {
                var error = Assert.ThrowsException<InvalidOperationException>(() => new Visual.EnviadorSmtp().Enviar("alguien@example.com", "Verificacion", "123456", DateTime.UtcNow.AddMinutes(10)));
                StringAssert.Contains(error.Message, "El envío SMTP no está configurado");
            }
            finally { for (int i=0;i<nombres.Length;i++) Environment.SetEnvironmentVariable(nombres[i], anteriores[i]); }
        }

        [TestMethod("CP-172: Registro y cambio muestran y exigen la fortaleza mínima")]
        public void CP172_FortalezaMinimaYMedidor()
        {
            Assert.AreEqual(3, GestorUsuarios.Instance.PuntajeFortaleza("abcdefgh1"));
            Assert.AreEqual(4, GestorUsuarios.Instance.PuntajeFortaleza("Cliente123"));
            Assert.AreEqual("RN-47", Assert.ThrowsException<ReglaNegocioException>(() => GestorUsuarios.Instance.ValidarContrasena("abcdefgh1")).Regla);
            GestorUsuarios.Instance.ValidarContrasena("Cliente123");
            EnSta(() =>
            {
                using (var registro = new Visual.frmCrearUsuario())
                using (var cambio = new Visual.frmCambiarContraseña())
                {
                    Mostrar(registro); Mostrar(cambio);
                    ControlDe<TextBox>(registro, "TxtPassword").Text = "abcdefgh1";
                    ControlDe<TextBox>(cambio, "TxtPassword").Text = "abcdefgh1";
                    Assert.AreEqual(3, ControlDe<Visual.Componentes.IndicadorFortaleza>(registro, "fortalezaContrasena").Puntaje);
                    Assert.AreEqual(3, ControlDe<Visual.Componentes.IndicadorFortaleza>(cambio, "fortalezaContrasena").Puntaje);
                    ControlDe<TextBox>(registro, "TxtPassword").Text = "Cliente123";
                    Assert.AreEqual(4, ControlDe<Visual.Componentes.IndicadorFortaleza>(registro, "fortalezaContrasena").Puntaje);
                }
            });
        }

        [TestMethod("CP-173: La recuperacion usa la bandeja simulada y restablece desde la pantalla")]
        public void CP173_RecuperacionDesdeLaInterfaz()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.frmCambiarContraseña(true))
                {
                    Mostrar(form);
                    ControlDe<TextBox>(form, "txtMail").Text = cliente.Mail;
                    ControlDe<Button>(form, "btnEnviarCodigo").PerformClick();
                    var mensaje = EnviadorSimulado.Ultimo(cliente.Mail, "Recuperacion");
                    Assert.IsNotNull(mensaje);
                    Assert.IsTrue(ControlDe<Label>(form, "estadoRecuperacion").Text.Contains(mensaje.Codigo));
                    ControlDe<TextBox>(form, "txtActual").Text = mensaje.Codigo;
                    ControlDe<TextBox>(form, "TxtPassword").Text = "NuevaClave123";
                    ControlDe<TextBox>(form, "TxtConfirmpassword").Text = "NuevaClave123";
                    ControlDe<Button>(form, "btnconfirmar").PerformClick();
                    string hash = RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña;
                    Assert.IsTrue(Seguridad.Verificar("NuevaClave123", hash));
                    Assert.IsFalse(Seguridad.Verificar("Cliente123", hash));
                }
            });
        }

        [TestMethod("CP-174: La sesión avisa y vence por inactividad, y la auditoría queda consultable solo por administración")]
        public void CP174_InactividadYAuditoria()
        {
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Assert.IsNotNull(GestorUsuarios.Instance.Autenticar(cliente.Mail, "Cliente123"));
            Fake.ComoCliente(cliente);
            var sesion = GestorSesion.Instance;
            DateTime baseTiempo = DateTime.Now;
            string minutosPrevios = GestorParametros.Instance.Obtener("SESION_INACTIVIDAD_MIN");
            string avisoPrevio = GestorParametros.Instance.Obtener("SESION_AVISO_SEG");
            try
            {
                Fake.ConfigurarParametro("SESION_INACTIVIDAD_MIN", "2");
                Fake.ConfigurarParametro("SESION_AVISO_SEG", "30");
                sesion.RegistrarActividad(baseTiempo);
                Assert.IsFalse(sesion.DebeAvisarInactividad(baseTiempo.AddSeconds(89)));
                Assert.IsTrue(sesion.DebeAvisarInactividad(baseTiempo.AddSeconds(90)));
                Assert.IsFalse(sesion.DebeCerrarPorInactividad(baseTiempo.AddSeconds(119)));
                Assert.IsTrue(sesion.DebeCerrarPorInactividad(baseTiempo.AddMinutes(2)));
                GestorAuditoria.Instance.Registrar("Compra confirmada", "Compra", 777);
                Assert.AreEqual("RN-34", Assert.ThrowsException<ReglaNegocioException>(() => GestorAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), null)).Regla);
                Fake.ComoAdministrador();
                var registros = GestorAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "Compra confirmada");
                Assert.IsTrue(registros.Exists(r => r.UsuarioId == cliente.Id && r.EntidadId == 777));
                Assert.IsTrue(GestorAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "Inicio de sesión").Exists(r => r.UsuarioId == cliente.Id));
            }
            finally
            {
                Fake.ConfigurarParametro("SESION_INACTIVIDAD_MIN", minutosPrevios);
                Fake.ConfigurarParametro("SESION_AVISO_SEG", avisoPrevio);
                sesion.Cerrar();
            }
        }

        [TestMethod("CP-175: El administrador abre el registro de auditoría desde su pantalla")]
        public void CP175_PantallaAuditoria()
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                using (var form = new Visual.frmAuditoria())
                {
                    Mostrar(form);
                    Assert.IsTrue(form.Visible);
                    Assert.IsNotNull(ControlDe<DataGridView>(form, "tablaAuditoria").DataSource);
                }
            });
        }
        private static void Ejecutar(string sql)
        {
            using (var conexion = new SqlConnection(BaseDePrueba.CadenaDePrueba()))
            using (var comando = new SqlCommand(sql, conexion)) { conexion.Open(); comando.ExecuteNonQuery(); }
        }
    }
}
