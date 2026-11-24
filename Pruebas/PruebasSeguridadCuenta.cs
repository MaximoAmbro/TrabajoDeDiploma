using System;
using System.Diagnostics;
using System.IO;
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
        // CP-207 — RF-14/RF-15, RN-43/RN-44: el correo simulado es un mensaje MIME legible.
        [TestMethod("CP-207: El archivo de correo simulado es MIME y contiene el código en el cuerpo")]
        public void CP207_CorreoMimLegible()
        {
            string correo = Fake.MailNuevo();
            const string codigo = "482619";
            var vence = DateTime.UtcNow.AddMinutes(9);
            SeleccionEnviadorCodigos.Actual = new EnviadorSimulado();
            SeleccionEnviadorCodigos.Actual.Enviar(correo, "Verificacion", codigo, vence);
            string ruta = EnviadorSimulado.ArchivoDeBandejaParaPruebas(correo);
            string eml = File.ReadAllText(ruta, System.Text.Encoding.UTF8);
            int separador = eml.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            Assert.IsTrue(separador > 0, "El mensaje debe separar encabezados y cuerpo MIME con una línea vacía.");
            string encabezados = eml.Substring(0, separador);
            string cuerpo = eml.Substring(separador + 4);
            StringAssert.Contains(encabezados, "From: EventAura <eventaura@simulado.invalid>");
            StringAssert.Contains(encabezados, "To: " + correo);
            StringAssert.Contains(encabezados, "Subject: Verificacion de correo - EventAura");
            StringAssert.Contains(encabezados, "Date: ");
            StringAssert.Contains(encabezados, "Content-Type: text/plain; charset=utf-8");
            Assert.IsTrue(DateTimeOffset.TryParse(encabezados.Split(new[] { "Date: " }, StringSplitOptions.None)[1].Split('\r')[0], out _));
            StringAssert.Contains(cuerpo, "Código: " + codigo);
            StringAssert.Contains(cuerpo, "Vence:");
        }

        // CP-208 — RF-14/RF-15, RN-43/RN-44: SMTP solo se selecciona cuando están las cinco variables.
        [TestMethod("CP-208: El emisor se selecciona según las cinco variables SMTP")]
        public void CP208_SeleccionEmisorPorEntorno()
        {
            string[] nombres = { "EVENTAURA_SMTP_HOST", "EVENTAURA_SMTP_PORT", "EVENTAURA_SMTP_USER", "EVENTAURA_SMTP_PASSWORD", "EVENTAURA_SMTP_FROM" };
            var anteriores = nombres.Select(nombre => Environment.GetEnvironmentVariable(nombre)).ToArray();
            try
            {
                foreach (string nombre in nombres) Environment.SetEnvironmentVariable(nombre, "configurado-para-prueba");
                Assert.IsInstanceOfType(Visual.EnviadorSmtp.Seleccionar(), typeof(Visual.EnviadorSmtp));
                Environment.SetEnvironmentVariable(nombres[3], null);
                Assert.IsInstanceOfType(Visual.EnviadorSmtp.Seleccionar(), typeof(EnviadorSimulado));
            }
            finally
            {
                for (int i = 0; i < nombres.Length; i++) Environment.SetEnvironmentVariable(nombres[i], anteriores[i]);
            }
        }

        // CP-167 — RF-14, RN-43: impedir la compra mientras la cuenta no verificó el correo.
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

        // CP-168 — RF-14, RN-43: confirmar correo con el código leído fuera de la aplicación.
        [TestMethod("CP-168: El alta guarda el código fuera de la aplicación y verifica el correo")]
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
                    string archivo = EnviadorSimulado.ArchivoDeBandejaParaPruebas(correo);
                    Assert.IsTrue(File.Exists(archivo));
                    Assert.IsTrue(File.ReadAllText(archivo).Contains(mensaje.Codigo), "El código queda en el archivo que se abre fuera de EventAura.");
                    Assert.IsNotNull(form.Controls.Find("verificacionCorreo", true).Single());
                    Assert.IsTrue(ControlDe<Button>(form, "btnVolverAcceso").Visible, "La verificación debe ofrecer una salida visible.");
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(mensaje.Codigo)));
                    Assert.IsFalse(typeof(Visual.frmInicio).Assembly.GetTypes().Any(t => t.Name == "frmBandejaSimulada"));
                    VerificarQueLaInterfazNoPuedeLeerCodigos();
                    Assert.IsFalse(RepositorioUsuarios.Instance.CorreoVerificado(RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id));
                    ControlDe<TextBox>(form, "txtCodigoCorreo").Text = mensaje.Codigo;
                    ControlDe<Button>(form, "btnConfirmarCorreo").PerformClick();
                    Assert.IsTrue(RepositorioUsuarios.Instance.CorreoVerificado(RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id));
                    Assert.IsTrue(ControlDe<Label>(form, "estadoVerificacion").Text.Contains("Ya podés iniciar sesión y comprar"));
                    int usuarioId = RepositorioUsuarios.Instance.ObtenerPorMail(correo).Id;
                    Assert.IsTrue(RepositorioAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "Código emitido: Verificacion")
                        .Any(r => r.UsuarioId == usuarioId && r.Resultado == "Correcto"));
                }
            });
        }

        // CP-169 — RF-15, RN-44: rechazar un código vencido sin alterar la contraseña almacenada.
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
            Assert.IsTrue(RepositorioAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "Restablecimiento de contraseña rechazado")
                .Any(r => r.UsuarioId == cliente.Id && r.Resultado == "Rechazado: RN-44"));
        }

        // CP-170 — RF-15/RF-17, RN-02/RN-44: recuperar con las validaciones y el hash vigentes.
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
            var auditoria = RepositorioAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "Cambio de contraseña");
            Assert.IsTrue(auditoria.Any(r => r.UsuarioId == cliente.Id && r.Resultado == "Recuperación por código"));
            Assert.IsTrue(RepositorioAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "Código de recuperación emitido")
                .Any(r => r.UsuarioId == cliente.Id && r.Resultado == "Correcto"));
        }

        // CP-171 — RF-14/RF-15, RN-44: SMTP falla claramente si no está configurado.
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

        // CP-172 — RF-17, RN-02/RN-47: medir y exigir fortaleza al registrar o cambiar una clave.
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
                    Assert.IsTrue(ControlDe<TextBox>(registro, "TxtPassword").UseSystemPasswordChar);
                    Assert.IsTrue(ControlDe<TextBox>(registro, "TxtConfirmpassword").UseSystemPasswordChar);
                    Assert.IsTrue(ControlDe<TextBox>(cambio, "txtActual").UseSystemPasswordChar);
                    Assert.IsTrue(ControlDe<TextBox>(cambio, "TxtPassword").UseSystemPasswordChar);
                    Assert.IsTrue(ControlDe<TextBox>(cambio, "TxtConfirmpassword").UseSystemPasswordChar);
                    ControlDe<TextBox>(registro, "TxtPassword").Text = "abcdefgh1";
                    ControlDe<TextBox>(cambio, "TxtPassword").Text = "abcdefgh1";
                    Assert.AreEqual(3, ControlDe<Visual.Componentes.IndicadorFortaleza>(registro, "fortalezaContrasena").Puntaje);
                    Assert.AreEqual(3, ControlDe<Visual.Componentes.IndicadorFortaleza>(cambio, "fortalezaContrasena").Puntaje);
                    ControlDe<TextBox>(registro, "TxtPassword").Text = "Cliente123";
                    Assert.AreEqual(4, ControlDe<Visual.Componentes.IndicadorFortaleza>(registro, "fortalezaContrasena").Puntaje);
                }
            });
        }

        // CP-173 — RF-15, RN-44: recuperar desde la interfaz; el archivo se lee fuera de la aplicación.
        [TestMethod("CP-173: La recuperación no muestra el código y permite restablecer desde la pantalla")]
        public void CP173_RecuperacionDesdeLaInterfaz()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = new Visual.frmCambiarContraseña(true))
                {
                    Mostrar(form);
                    Assert.IsFalse(ControlDe<TextBox>(form, "txtActual").UseSystemPasswordChar,
                        "El código de recuperación se puede leer para ingresarlo desde el correo simulado.");
                    Assert.IsTrue(ControlDe<TextBox>(form, "TxtPassword").UseSystemPasswordChar);
                    Assert.IsTrue(ControlDe<TextBox>(form, "TxtConfirmpassword").UseSystemPasswordChar);
                    ControlDe<TextBox>(form, "txtMail").Text = cliente.Mail;
                    ControlDe<Button>(form, "btnEnviarCodigo").PerformClick();
                    var mensaje = EnviadorSimulado.Ultimo(cliente.Mail, "Recuperacion");
                    Assert.IsNotNull(mensaje);
                    string respuesta = "Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.";
                    Assert.AreEqual(respuesta, ControlDe<Label>(form, "estadoRecuperacion").Text);
                    Assert.IsTrue(File.ReadAllText(EnviadorSimulado.ArchivoDeBandejaParaPruebas(cliente.Mail)).Contains(mensaje.Codigo));
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(mensaje.Codigo)), "Ningún control visible ni oculto de la aplicación debe mostrar el código.");
                    Assert.IsFalse(typeof(Visual.frmInicio).Assembly.GetTypes().Any(t => t.Name == "frmBandejaSimulada"));
                    Assert.IsFalse(form.Controls.Find("btnAbrirBandejaSimulada", true).Any());
                    VerificarQueLaInterfazNoPuedeLeerCodigos();
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

        // CP-200 — RF-15, RN-44: la recuperación no distingue correo existente por texto ni latencia.
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

                    string respuesta = "Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección.";
                    Assert.AreEqual(respuesta, respuestaExistente);
                    Assert.AreEqual(respuestaExistente, respuestaInexistente);
                    Assert.IsNotNull(señuelo, "La respuesta simulada mantiene el formato uniforme para la dirección desconocida.");
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(señuelo.Codigo)));
                    Assert.IsTrue(File.ReadAllText(EnviadorSimulado.ArchivoDeBandejaParaPruebas(existente)).Contains(codigoExistente.Codigo));
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

        // CP-201 — RF-15, RN-44: emitir un código nuevo invalida el anterior y cada código se consume una vez.
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

        // CP-203 — RF-14, RN-01/RN-43: repetir el alta no enumera ni modifica la cuenta existente.
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
                    Assert.IsNotNull(nuevo.Controls.Find("verificacionCorreo", true).Single());
                    var mensajeNuevo = EnviadorSimulado.Ultimo(correo, "Verificacion");
                    Assert.IsNotNull(mensajeNuevo);
                    Assert.IsFalse(TextosVisibles(nuevo).Any(t => t.Contains(mensajeNuevo.Codigo)));
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
                    Assert.IsNotNull(existente.Controls.Find("verificacionCorreo", true).Single());
                    var mensaje = EnviadorSimulado.Ultimo(correo, "Verificacion");
                    Assert.IsNotNull(mensaje);
                    Assert.IsFalse(TextosVisibles(existente).Any(t => t.Contains(mensaje.Codigo)));
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

        private static void VerificarQueLaInterfazNoPuedeLeerCodigos()
        {
            string carpeta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "Visual");
            string[] fuentes = Directory.GetFiles(carpeta, "*.cs", SearchOption.AllDirectories);
            foreach (string fuente in fuentes)
            {
                string texto = File.ReadAllText(fuente);
                Assert.IsFalse(texto.Contains("EnviadorSimulado.Ultimo"), fuente + " no puede consultar el archivo de códigos.");
                Assert.IsFalse(texto.Contains("ArchivoDeBandejaParaPruebas"), fuente + " no puede consultar el archivo de códigos.");
                Assert.IsFalse(texto.Contains("frmBandejaSimulada"), fuente + " no puede declarar una pantalla de bandeja.");
                Assert.IsFalse(texto.Contains("mensaje.Codigo"), fuente + " no puede presentar el contenido de un código.");
            }
        }

        // CP-174 — RF-16/RF-17, RN-45/RN-46: registrar acciones y cerrar la sesión al vencer la inactividad.
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
                Assert.AreEqual("RN-45", Assert.ThrowsException<ReglaNegocioException>(() => GestorAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), null)).Regla);
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

        // CP-175 — RF-16, RN-45: permitir que Administración consulte los registros de auditoría.
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
                    var tabla = ControlDe<DataGridView>(form, "tablaAuditoria");
                    Assert.IsNotNull(tabla.DataSource);
                    Assert.AreEqual(8, tabla.Columns.Count, "Se conservan las ocho columnas de auditoría en los datos.");
                    CollectionAssert.AreEqual(
                        new[] { "Id", "UsuarioId", "Correo", "Acción", "Qué se modificó", "EntidadId", "Fecha y hora", "Resultado" },
                        tabla.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText).ToArray());
                    Assert.IsTrue(new[] { "Id", "UsuarioId", "EntidadId" }.All(n => !tabla.Columns[n].Visible), "Los identificadores internos no se muestran al usuario.");
                    Assert.AreEqual(5, tabla.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible), "Se muestran solo los cinco datos útiles para consultar la auditoría.");
                    Assert.IsTrue(tabla.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).Sum(c => c.Width)
                        <= tabla.ClientSize.Width, "Las columnas de auditoría deben entrar sin corte horizontal.");
                    Assert.IsTrue(Enumerable.Range(0, tabla.Columns.Count).All(i =>
                        tabla.GetColumnDisplayRectangle(i, false).Right <= tabla.ClientSize.Width),
                        "Todas las columnas de auditoría deben quedar dentro del panel.");
                    Assert.IsTrue(tabla.Right <= form.ClientSize.Width,
                        $"La grilla no debe exceder el formulario: derecha={tabla.Right}, ancho={form.ClientSize.Width}, grilla={tabla.ClientSize.Width}.");
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
