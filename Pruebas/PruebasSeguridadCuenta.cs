using System;
using System.Diagnostics;
using System.Linq;
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
        // CP-167 — RF-05, RN-43: impedir la compra mientras la cuenta no verificó el correo.
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

        // CP-168 — RF-01, RN-43: emitir y confirmar el código de correo desde la bandeja simulada.
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
                    Assert.IsTrue(ControlDe<Label>(form, "mensajeBandeja").Text.Contains(mensaje.Codigo));
                    Assert.IsFalse(ControlDe<Label>(form, "estadoVerificacion").Text.Contains(mensaje.Codigo));
                    Assert.IsFalse(RepositorioUsuarios.Instance.CorreoVerificado(RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id));
                    ControlDe<TextBox>(form, "txtCodigoCorreo").Text = mensaje.Codigo;
                    ControlDe<Button>(form, "btnConfirmarCorreo").PerformClick();
                    Assert.IsTrue(RepositorioUsuarios.Instance.CorreoVerificado(RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id));
                    Assert.IsTrue(ControlDe<Label>(form, "estadoVerificacion").Text.Contains("Ya podés iniciar sesión y comprar"));
                }
            });
        }

        // CP-169 — RF-01, RN-44: rechazar un código vencido sin alterar la contraseña almacenada.
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

        // CP-170 — RF-01, RN-02/RN-44: recuperar la cuenta usando la validación y el hash vigentes.
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

        // CP-171 — RF-01, RN-44: la estrategia SMTP falla claramente si no está configurada.
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

        // CP-172 — RF-01, RN-02/RN-47: medir y exigir fortaleza al registrar o cambiar una clave.
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

        // CP-173 — RF-01, RN-44: recuperar desde la interfaz y leer el código en la bandeja.
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
                    Assert.AreEqual("Si la cuenta existe, enviamos un código.", ControlDe<Label>(form, "estadoRecuperacion").Text);
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(mensaje.Codigo)), "El formulario solicitante no muestra el código.");
                    using (var bandeja = new Visual.frmBandejaSimulada(cliente.Mail, "Recuperacion"))
                        Assert.IsTrue(ControlDe<Label>(bandeja, "mensajeBandejaSimulada").Text.Contains(mensaje.Codigo), "El código aparece solamente al abrir la bandeja simulada.");
                    Assert.IsTrue(AbrirBandejaYVerCodigo(form, "btnAbrirBandejaSimulada", mensaje.Codigo));
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

        // CP-200 — RF-01, RN-44: la recuperación no distingue correo existente por texto ni latencia.
        [TestMethod("CP-200: Recuperación no enumera correos y la respuesta tarda lo mismo")]
        public void CP200_RecuperacionNoEnumeraYRespondeUniforme()
        {
            EnSta(() =>
            {
                const string existente = "admin@eventaura.com";
                const string inexistente = "test-no-existe@pruebas.eventaura.com";
                using (var form = new Visual.frmCambiarContraseña(true))
                {
                    Mostrar(form);
                    ControlDe<TextBox>(form, "txtMail").Text = existente;
                    var relojExistente = Stopwatch.StartNew();
                    ControlDe<Button>(form, "btnEnviarCodigo").PerformClick();
                    relojExistente.Stop();
                    string respuestaExistente = ControlDe<Label>(form, "estadoRecuperacion").Text;
                    var codigoExistente = EnviadorSimulado.Ultimo(existente, "Recuperacion");
                    Assert.IsNotNull(codigoExistente);
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(codigoExistente.Codigo)));

                    ControlDe<TextBox>(form, "txtMail").Text = inexistente;
                    var relojInexistente = Stopwatch.StartNew();
                    ControlDe<Button>(form, "btnEnviarCodigo").PerformClick();
                    relojInexistente.Stop();
                    string respuestaInexistente = ControlDe<Label>(form, "estadoRecuperacion").Text;
                    var señuelo = EnviadorSimulado.Ultimo(inexistente, "Recuperacion");

                    Assert.AreEqual("Si la cuenta existe, enviamos un código.", respuestaExistente);
                    Assert.AreEqual(respuestaExistente, respuestaInexistente);
                    Assert.IsNotNull(señuelo, "La bandeja simulada muestra el mismo formato para la dirección desconocida.");
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(señuelo.Codigo)));
                    Assert.IsTrue(relojExistente.ElapsedMilliseconds >= 950 && relojInexistente.ElapsedMilliseconds >= 950,
                        "Ambas solicitudes respetan el tiempo mínimo de respuesta uniforme.");
                    Assert.IsTrue(Math.Abs(relojExistente.ElapsedMilliseconds - relojInexistente.ElapsedMilliseconds) < 400,
                        "La diferencia observada entre los dos tiempos debe ser menor a 400 ms.");

                    var error = Assert.ThrowsException<ReglaNegocioException>(() =>
                        GestorCodigosCorreo.Instance.RecuperarContrasena(inexistente, señuelo.Codigo, "NuevaClave123"));
                    Assert.AreEqual("RN-44", error.Regla);
                    Assert.AreEqual("El código no es válido o venció. Solicitá uno nuevo.", error.Message);
                }
            });
        }

        // CP-201 — RF-01, RN-44: emitir un código nuevo invalida el anterior y cada código se consume una vez.
        [TestMethod("CP-201: Un nuevo código invalida el anterior y solo se usa una vez")]
        public void CP201_CodigoAnteriorInvalidoYUnSoloUso()
        {
            var cliente = Fake.Cliente(NivelFidelidad.Bronce);
            GestorCodigosCorreo.Instance.SolicitarRecuperacion(cliente.Mail);
            var anterior = EnviadorSimulado.Ultimo(cliente.Mail, "Recuperacion");
            GestorCodigosCorreo.Instance.SolicitarRecuperacion(cliente.Mail);
            var actual = EnviadorSimulado.Ultimo(cliente.Mail, "Recuperacion");
            Assert.IsNotNull(anterior);
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(anterior.Codigo, actual.Codigo);
            var errorAnterior = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorCodigosCorreo.Instance.RecuperarContrasena(cliente.Mail, anterior.Codigo, "NuevaClave123"));
            Assert.AreEqual("El código no es válido o venció. Solicitá uno nuevo.", errorAnterior.Message);

            GestorCodigosCorreo.Instance.RecuperarContrasena(cliente.Mail, actual.Codigo, "NuevaClave123");
            var errorReutilizado = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorCodigosCorreo.Instance.RecuperarContrasena(cliente.Mail, actual.Codigo, "OtraClave123"));
            Assert.AreEqual("El código no es válido o venció. Solicitá uno nuevo.", errorReutilizado.Message);
        }

        // CP-203 — RF-01, RN-01/RN-43: repetir el alta no enumera ni modifica la cuenta existente.
        [TestMethod("CP-203: El alta existente y nueva entrega la misma respuesta de verificación")]
        public void CP203_RegistroNoEnumeraCorreoExistente()
        {
            EnSta(() =>
            {
                string correo = Fake.MailNuevo();
                long tiempoNuevo;
                using (var nuevo = new Visual.frmCrearUsuario())
                {
                    Mostrar(nuevo);
                    CompletarAlta(nuevo, correo, "PrimeraClave123", "Primero");
                    var reloj = Stopwatch.StartNew();
                    ControlDe<Button>(nuevo, "btnregistrarse").PerformClick();
                    reloj.Stop();
                    tiempoNuevo = reloj.ElapsedMilliseconds;
                    Assert.IsNotNull(nuevo.Controls.Find("bandejaCorreoSimulada", true).Single());
                }

                string hashAnterior = RepositorioUsuarios.Instance.ObtenerPorMail(correo).Contraseña;
                long tiempoExistente;
                using (var existente = new Visual.frmCrearUsuario())
                {
                    Mostrar(existente);
                    CompletarAlta(existente, correo, "OtraClave456", "Segundo");
                    var reloj = Stopwatch.StartNew();
                    ControlDe<Button>(existente, "btnregistrarse").PerformClick();
                    reloj.Stop();
                    tiempoExistente = reloj.ElapsedMilliseconds;
                    Assert.IsNotNull(existente.Controls.Find("bandejaCorreoSimulada", true).Single());
                    var mensaje = EnviadorSimulado.Ultimo(correo, "Verificacion");
                    Assert.IsNotNull(mensaje);
                    Assert.IsTrue(ControlDe<Label>(existente, "mensajeBandeja").Text.Contains(mensaje.Codigo));
                }

                Assert.AreEqual(hashAnterior, RepositorioUsuarios.Instance.ObtenerPorMail(correo).Contraseña,
                    "El intento de alta repetido no cambia la contraseña de la cuenta existente.");
                Assert.IsTrue(tiempoNuevo >= 950 && tiempoExistente >= 950);
                Assert.IsTrue(Math.Abs(tiempoNuevo - tiempoExistente) < 400,
                    "El alta nueva y el correo ya registrado tardan lo mismo hasta mostrar la bandeja.");
            });
        }

        private static void CompletarAlta(Visual.frmCrearUsuario form, string correo, string clave, string nombre)
        {
            ControlDe<TextBox>(form, "txtNombre").Text = nombre;
            ControlDe<TextBox>(form, "txtApellido").Text = "Prueba";
            ControlDe<TextBox>(form, "txtmail").Text = correo;
            ControlDe<TextBox>(form, "TxtPassword").Text = clave;
            ControlDe<TextBox>(form, "TxtConfirmpassword").Text = clave;
        }

        private static System.Collections.Generic.IEnumerable<string> TextosVisibles(Control padre)
        {
            foreach (Control control in padre.Controls)
            {
                if (!string.IsNullOrEmpty(control.Text)) yield return control.Text;
                foreach (string texto in TextosVisibles(control)) yield return texto;
            }
        }

        private static bool AbrirBandejaYVerCodigo(Control origen, string nombreBoton, string codigo)
        {
            bool abierta = false;
            bool codigoVisible = false;
            using (var temporizador = new Timer { Interval = 25 })
            {
                temporizador.Tick += (s, e) =>
                {
                    var bandeja = Application.OpenForms.OfType<Visual.frmBandejaSimulada>().FirstOrDefault(f => f.Visible);
                    if (bandeja == null) return;
                    abierta = true;
                    codigoVisible = ControlDe<Label>(bandeja, "mensajeBandejaSimulada").Text.Contains(codigo);
                    bandeja.Close();
                    temporizador.Stop();
                };
                temporizador.Start();
                ControlDe<Button>(origen, nombreBoton).PerformClick();
                temporizador.Stop();
            }
            return abierta && codigoVisible;
        }

        // CP-174 — RF-11, RN-45/RN-46: registrar acciones y cerrar la sesión al vencer la inactividad.
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

        // CP-175 — RF-11, RN-45: permitir que Administración consulte los registros de auditoría.
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
