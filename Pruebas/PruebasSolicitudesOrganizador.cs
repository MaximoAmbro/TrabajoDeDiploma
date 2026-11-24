using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasSolicitudesOrganizador
    {
        private Entidades.Cliente cliente;
        private const string Telefono = "+5493412345678";
        private static readonly byte[] Png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADUlEQVR4nGP4z8AAAAMBAQDJ/pLvAAAAAElFTkSuQmCC");

        [TestInitialize]
        public void Preparar() { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); cliente = Fake.Cliente(NivelFidelidad.Bronce); Fake.ComoCliente(cliente); }
        [TestCleanup]
        public void Limpiar() { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); SeleccionVerificadorIdentidad.Actual = new VerificadorIdentidadSimulado(); }

        // CP-184 — RF-20/RF-22, RN-50: una solicitud pendiente no otorga permisos de Organizador.
        [TestMethod("CP-184: Solicitud pendiente no concede permisos de Organizador")]
        public void CP184_SolicitudPendienteNoConcedePermisos()
        {
            Assert.IsTrue(GestorSolicitudesOrganizador.CuitValido("20-12345678-6"));
            Assert.IsTrue(GestorSolicitudesOrganizador.CbuValido(new string('0',22)));
            var enviada = new EnviadorSimulado(); SeleccionEnviadorCodigos.Actual = enviada;
            GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono(Telefono);
            var codigo = EnviadorSimulado.Ultimo(Telefono, GestorCodigosCorreo.PropositoTelefonoOrganizador);
            Assert.IsNotNull(codigo, "El código de teléfono debe salir por el mismo enviador simulado.");
            GestorSolicitudesOrganizador.Instance.ConfirmarCodigoTelefono(Telefono, codigo.Codigo);
            int id = GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida());
            Assert.IsTrue(id > 0);
            var persistido = Datos.RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail);
            Assert.AreEqual("Cliente", persistido.TipoUsuario, "La solicitud pendiente no cambia el perfil persistido.");
            GestorSesion.Instance.Cerrar();
            GestorUsuarios.Instance.Autenticar(cliente.Mail, "Cliente123");
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorLocales.Instance.PorPropietario(cliente.Id));
            Assert.AreEqual("RN-48", error.Regla);
            Assert.AreEqual("Esta operación es solo para el Organizador.", error.Message);
            var noAdmin = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Pendientes());
            Assert.AreEqual("RN-34", noAdmin.Regla);
        }

        // CP-185 — RF-20, RN-52: rechazar CUIT con digito verificador incorrecto.
        [TestMethod("CP-185: CUIT con dígito verificador incorrecto se rechaza")]
        public void CP185_CuitConVerificadorIncorrectoSeRechaza()
        {
            var solicitud = SolicitudValida(); solicitud.Cuit = "20-12345678-5";
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(solicitud));
            Assert.AreEqual("RN-52", error.Regla);
            Assert.AreEqual("El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.", error.Message);
            Assert.AreEqual(error.Message, Visual.ManejadorErrores.MensajeParaUsuario(error), "La pantalla no debe tapar una regla válida con un error genérico.");
            InterfazGrupoA.EnSta(() =>
            {
                Fake.ComoCliente(cliente);
                using (var form = new Visual.frmSolicitudOrganizador())
                {
                    InterfazGrupoA.Mostrar(form);
                    CargarCamposSolicitud(form, "20-12345678-5");
                    form.Controls.OfType<Button>().Single(b => b.Text == "Enviar solicitud").PerformClick();
                    Assert.AreEqual(error.Message, InterfazGrupoA.ControlDe<Label>(form, "avisoSistema").Text);
                }
            });
        }

        // CP-186 — RF-20, RN-52: rechazar CBU con digito verificador incorrecto.
        [TestMethod("CP-186: CBU con dígito verificador incorrecto se rechaza")]
        public void CP186_CbuConVerificadorIncorrectoSeRechaza()
        {
            var solicitud = SolicitudValida(); solicitud.CbuAlias = "0000000000000000000001";
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(solicitud));
            Assert.AreEqual("RN-52", error.Regla);
            Assert.AreEqual("Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.", error.Message);
        }

        // CP-187 — RF-20/RF-22, RN-34: el acceso a aprobar solicitudes se rechaza para Cliente.
        [TestMethod("CP-187: Cliente no puede aprobar una solicitud")]
        public void CP187_ClienteNoPuedeAprobarSolicitud()
        {
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Aprobar(1, "Revisada"));
            Assert.AreEqual("RN-34", error.Regla);
            Assert.AreEqual("Esta operación es solo para el Administrador", error.Message);
        }

        // CP-188 — RF-20, RN-53: exigir un motivo para rechazar una solicitud.
        [TestMethod("CP-188: Rechazar una solicitud exige motivo")]
        public void CP188_RechazoSinMotivoSeRechaza()
        {
            Fake.ComoAdministrador();
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Rechazar(1, "  "));
            Assert.AreEqual("RN-53", error.Regla);
            Assert.AreEqual("Escribí el motivo del rechazo.", error.Message);
            Assert.AreEqual(error.Message, Visual.ManejadorErrores.MensajeParaUsuario(error));
            InterfazGrupoA.EnSta(() =>
            {
                var solicitante = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(solicitante);
                VerificarTelefono();
                GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida());
                Fake.ComoAdministrador();
                using (var form = new Visual.frmRevisarSolicitudesOrganizador())
                {
                    InterfazGrupoA.Mostrar(form);
                    form.Controls.OfType<Button>().Single(b => b.Text == "Rechazar").PerformClick();
                    Assert.AreEqual(error.Message, InterfazGrupoA.ControlDe<Label>(form, "avisoSistema").Text);
                }
            });
        }

        // CP-190 — RF-20/RF-22, RN-34: el acceso a rechazar solicitudes se deniega a Organizador.
        [TestMethod("CP-190: Organizador no puede rechazar solicitud de plataforma")]
        public void CP190_OrganizadorNoPuedeRechazarSolicitudDePlataforma()
        {
            Fake.ComoOrganizador();
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Rechazar(1, "Motivo de prueba."));
            Assert.AreEqual("RN-34", error.Regla);
            Assert.AreEqual("Esta operación es solo para el Administrador", error.Message);
        }

        // CP-191 — RF-20, RN-51: exigir la confirmacion del telefono antes de solicitar perfil Organizador.
        [TestMethod("CP-191: Teléfono sin verificar impide solicitar perfil Organizador")]
        public void CP191_TelefonoSinConfirmarImpideLaSolicitud()
        {
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida()));
            Assert.AreEqual("RN-51", error.Regla);
            Assert.AreEqual("Verificá este teléfono con el código enviado antes de presentar la solicitud.", error.Message);
            Assert.AreEqual(error.Message, Visual.ManejadorErrores.MensajeParaUsuario(error));
            InterfazGrupoA.EnSta(() =>
            {
                Fake.ComoCliente(cliente);
                using (var form = new Visual.frmSolicitudOrganizador())
                {
                    InterfazGrupoA.Mostrar(form);
                    CargarCamposSolicitud(form, "20-12345678-6");
                    form.Controls.OfType<Button>().Single(b => b.Text == "Enviar solicitud").PerformClick();
                    Assert.AreEqual(error.Message, InterfazGrupoA.ControlDe<Label>(form, "avisoSistema").Text);
                }
            });
        }

        // CP-192 — RF-20, RN-52: rechazar documentacion que no sea imagen valida.
        [TestMethod("CP-192: Documentación que no es imagen válida se rechaza")]
        public void CP192_DocumentacionQueNoEsImagenSeRechaza()
        {
            VerificarTelefono();
            var solicitud = SolicitudValida(); solicitud.Documentacion = new byte[] { 1,2,3,4 };
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(solicitud));
            Assert.AreEqual("RN-52", error.Regla);
            Assert.AreEqual("Adjuntá una imagen PNG o JPEG válida, comprimida y de hasta 512 KB.", error.Message);
        }

        // CP-193 — RF-20/RF-22, RN-34: la revisión administrativa se protege con el control de acceso global.
        [TestMethod("CP-193: Solicitud visible para Cliente y revisión protegida")]
        public void CP193_AccesoDeSolicitudVisibleYRevisionProtegida()
        {
            using (var menuCliente = new Visual.frmMenuCliente())
            {
                var boton = menuCliente.Controls.Find("btnSolicitudOrganizador", true).SingleOrDefault();
                Assert.IsNotNull(boton, "El Cliente necesita una entrada visible para solicitar el perfil.");
                Assert.IsTrue(boton.Enabled);
                Assert.AreEqual("Solicitar ser Organizador", boton.Text);
            }
            Fake.ComoAdministrador();
            using (var menuAdmin = new Visual.frmMenuAdministrador())
            {
                var boton = menuAdmin.Controls.Find("btnSolicitudesOrganizador", true).SingleOrDefault();
                Assert.IsNotNull(boton, "El Administrador necesita una entrada visible para revisar solicitudes.");
                Assert.IsTrue(boton.Enabled);
            }
            GestorSesion.Instance.Cerrar(); Fake.ComoCliente(cliente);
            var error = Assert.ThrowsException<ReglaNegocioException>(() => new Visual.frmRevisarSolicitudesOrganizador());
            Assert.AreEqual("RN-34", error.Regla);
        }

        // CP-194 — RF-20, RN-52: aceptar alias bancario con el formato definido.
        [TestMethod("CP-194: Alias bancario válido se acepta")]
        public void CP194_AliasBancarioEsAceptado()
        {
            var enviador = new EnviadorSimulado(); SeleccionEnviadorCodigos.Actual = enviador;
            VerificarTelefono();
            var solicitud = SolicitudValida(); solicitud.CbuAlias = "eventaura.demo";
            Assert.IsTrue(GestorSolicitudesOrganizador.CbuAliasValido(solicitud.CbuAlias));
            Assert.IsTrue(GestorSolicitudesOrganizador.Instance.Solicitar(solicitud) > 0);
        }

        // CP-196 — RF-16/RF-20, RN-45/RN-53: auditar la decision de rechazo y su motivo.
        [TestMethod("CP-196: Rechazo con motivo queda auditado")]
        public void CP196_RechazoConMotivoQuedaAuditado()
        {
            VerificarTelefono();
            int id = GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida());
            var admin = Fake.Administrador(); Fake.ComoAdministrador();
            GestorSolicitudesOrganizador.Instance.Rechazar(id, "La documentación de ejemplo requiere una revisión adicional.");
            var solicitud = GestorSolicitudesOrganizador.Instance.Obtener(id);
            Assert.AreEqual("Rechazada", solicitud.Estado);
            Assert.AreEqual("La documentación de ejemplo requiere una revisión adicional.", solicitud.MotivoRevision);
            Assert.AreEqual(admin.Id, solicitud.RevisadaPor);
            Assert.IsNotNull(solicitud.RevisadaEl);
            var auditoria = Datos.RepositorioAuditoria.Instance.Consultar(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddMinutes(5), "Solicitud de Organizador: rechazar");
            Assert.IsTrue(auditoria.Any(x=>x.UsuarioId==admin.Id && x.EntidadId==id && x.Resultado==solicitud.MotivoRevision));
        }

        // CP-195 — RF-20, RN-51: rechazar telefonos fuera del formato argentino.
        [TestMethod("CP-195: Teléfono fuera del formato argentino se rechaza")]
        public void CP195_TelefonoNoArgentinoSeRechaza()
        {
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono("0000000000"));
            Assert.AreEqual("RN-51", error.Regla);
            Assert.AreEqual("Ingresá un teléfono argentino válido, con característica y número.", error.Message);
        }

        // CP-189 — RF-16/RF-20/RF-22, RN-45/RN-53: promover al Organizador, crear su local y auditar la aprobacion.
        [TestMethod("CP-189: Aprobación promueve a Organizador y audita")]
        public void CP189_AprobacionPromueveYRegistraAuditoria()
        {
            var enviador = new EnviadorSimulado(); SeleccionEnviadorCodigos.Actual=enviador;
            GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono(Telefono);
            GestorSolicitudesOrganizador.Instance.ConfirmarCodigoTelefono(Telefono, EnviadorSimulado.Ultimo(Telefono,GestorCodigosCorreo.PropositoTelefonoOrganizador).Codigo);
            int id = GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida());
            var admin = Fake.Administrador(); Fake.ComoAdministrador();
            GestorSolicitudesOrganizador.Instance.Aprobar(id, "Datos de demostración revisados.");
            var usuario = Datos.RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail);
            Assert.AreEqual("Organizador", usuario.TipoUsuario);
            GestorSesion.Instance.Cerrar(); Fake.ComoOrganizador(usuario);
            Assert.IsTrue(GestorLocales.Instance.PorPropietario(usuario.Id).Any(x=>x.Nombre=="test local de prueba"));
            var auditoria = Datos.RepositorioAuditoria.Instance.Consultar(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddMinutes(5), "Solicitud de Organizador: aprobar");
            Assert.IsTrue(auditoria.Any(x=>x.UsuarioId==admin.Id && x.EntidadId==id && x.Resultado=="Datos de demostración revisados."));
        }

        // CP-202 — RF-20, RN-44/RN-51: el código telefónico solo se guarda para lectura externa.
        [TestMethod("CP-202: El código telefónico queda fuera de la interfaz")]
        public void CP202_CodigoTelefonoNoApareceEnLaSolicitud()
        {
            InterfazGrupoA.EnSta(() =>
            {
                var solicitante = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(solicitante);
                using (var form = new Visual.frmSolicitudOrganizador())
                {
                    InterfazGrupoA.Mostrar(form);
                    InterfazGrupoA.ControlDe<TextBox>(form, "telefono").Text = Telefono;
                    InterfazGrupoA.ControlDe<Button>(form, "btnEnviarCodigoTelefono").PerformClick();
                    var mensaje = EnviadorSimulado.Ultimo(Telefono, GestorCodigosCorreo.PropositoTelefonoOrganizador);
                    Assert.IsNotNull(mensaje, InterfazGrupoA.ControlDe<Label>(form, "estadoEnvioTelefono").Text);
                    Assert.AreEqual("Si el teléfono es válido, guardamos un SMS simulado fuera de EventAura. Abrí el archivo del mensaje para continuar.",
                        InterfazGrupoA.ControlDe<Label>(form, "estadoEnvioTelefono").Text);
                    var confirmacion = form.Controls.OfType<Button>().Single(b => b.Text == "Confirmar teléfono");
                    InterfazGrupoA.ControlDe<TextBox>(form, "codigo").Text = mensaje.Codigo == "000000" ? "000001" : "000000";
                    confirmacion.PerformClick();
                    var avisoRegla = InterfazGrupoA.ControlDe<Label>(form, "avisoSistema");
                    Assert.AreEqual("El código no es válido o venció. Solicitá uno nuevo.", avisoRegla.Text,
                        "El código incorrecto debe conservar el mensaje de RN-44.");
                    string archivo = EnviadorSimulado.ArchivoDeBandejaParaPruebas(Telefono, GestorCodigosCorreo.PropositoTelefonoOrganizador);
                    Assert.IsTrue(System.IO.File.Exists(archivo));
                    Assert.IsTrue(System.IO.File.ReadAllText(archivo).Contains(mensaje.Codigo));
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(mensaje.Codigo)),
                        "El código no debe aparecer en el formulario que lo solicita.");
                    Assert.IsFalse(typeof(Visual.frmInicio).Assembly.GetTypes().Any(t => t.Name == "frmBandejaSimulada"));
                    string codigoAuditable = "Código emitido: " + GestorCodigosCorreo.PropositoTelefonoOrganizador;
                    Assert.IsTrue(Datos.RepositorioAuditoria.Instance.Consultar(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), codigoAuditable)
                        .Any(r => r.UsuarioId == solicitante.Id && r.Resultado == "Correcto"));
                }
            });
        }

        // CP-355 — RF-20, RN-51: el SMS simulado permite completar la solicitud aunque SMTP esté configurado.
        [TestMethod("CP-355: Solicitud de Organizador completa con SMTP configurado usa SMS simulado")]
        public void CP355_SolicitudCompletaConSmtpConfigurado()
        {
            string[] nombres = { "EVENTAURA_SMTP_HOST", "EVENTAURA_SMTP_PORT", "EVENTAURA_SMTP_USER", "EVENTAURA_SMTP_PASSWORD", "EVENTAURA_SMTP_FROM" };
            string[] previos = nombres.Select(Environment.GetEnvironmentVariable).ToArray();
            try
            {
                string[] valores = { "smtp.example.invalid", "587", "demo", "no-secret-test", "eventaura@example.invalid" };
                for (int i = 0; i < nombres.Length; i++) Environment.SetEnvironmentVariable(nombres[i], valores[i]);
                SeleccionEnviadorCodigos.Actual = Visual.EnviadorSmtp.Seleccionar();
                Assert.IsInstanceOfType(SeleccionEnviadorCodigos.Actual, typeof(Visual.EnviadorSmtp));

                GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono(Telefono);
                var mensaje = EnviadorSimulado.Ultimo(Telefono, GestorCodigosCorreo.PropositoTelefonoOrganizador);
                Assert.IsNotNull(mensaje, "El propósito telefónico debe seleccionar SMS simulado aun cuando SMTP esté activo.");
                StringAssert.Contains(File.ReadAllText(EnviadorSimulado.ArchivoDeBandejaParaPruebas(Telefono, GestorCodigosCorreo.PropositoTelefonoOrganizador)), mensaje.Codigo);
                GestorSolicitudesOrganizador.Instance.ConfirmarCodigoTelefono(Telefono, mensaje.Codigo);
                Assert.IsTrue(GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida()) > 0,
                    "La solicitud debe completar el flujo de verificación telefónica sin intentar enviar un correo a un número.");
            }
            finally
            {
                for (int i = 0; i < nombres.Length; i++) Environment.SetEnvironmentVariable(nombres[i], previos[i]);
                SeleccionEnviadorCodigos.Actual = new EnviadorSimulado();
            }
        }

        // CP-356 — RF-14/RF-15, RN-43/RN-44: un proveedor fallido no deja un código activo en la base.
        [TestMethod("CP-356: Envío fallido no persiste el código")]
        public void CP356_EnvioFallidoNoPersisteCodigo()
        {
            var destinatario = Fake.Cliente(NivelFidelidad.Bronce);
            SeleccionEnviadorCodigos.Actual = new EnviadorQueFalla();
            Assert.ThrowsException<IOException>(() => GestorCodigosCorreo.Instance.SolicitarRecuperacion(destinatario.Mail));
            Assert.IsNull(Datos.RepositorioCodigosCorreo.Instance.Ultimo(destinatario.Id, "Recuperacion"),
                "La emisión debe ocurrir después de que el proveedor confirme la entrega.");
        }

        private sealed class EnviadorQueFalla : IEnviadorCodigos
        {
            public void Enviar(string correo, string proposito, string codigo, DateTime venceEl)
            {
                throw new IOException("Fallo de entrega simulado para la prueba.");
            }
        }

        private static System.Collections.Generic.IEnumerable<string> TextosVisibles(Control padre)
        {
            foreach (Control control in padre.Controls)
            {
                if (!string.IsNullOrEmpty(control.Text)) yield return control.Text;
                foreach (string texto in TextosVisibles(control)) yield return texto;
            }
        }

        private SolicitudOrganizador SolicitudValida() => new SolicitudOrganizador { Responsable="Responsable de prueba", Cuit="20-12345678-6", Telefono=Telefono,
            NombreLocal="test local de prueba", Direccion="Dirección de prueba 123", Capacidad=100, CbuAlias=new string('0',22), Documentacion=Png, NombreArchivo="constancia.png" };

        private void VerificarTelefono()
        {
            var enviador = new EnviadorSimulado(); SeleccionEnviadorCodigos.Actual = enviador;
            GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono(Telefono);
            var mensaje = EnviadorSimulado.Ultimo(Telefono, GestorCodigosCorreo.PropositoTelefonoOrganizador);
            GestorSolicitudesOrganizador.Instance.ConfirmarCodigoTelefono(Telefono, mensaje.Codigo);
        }

        private static void CargarCamposSolicitud(Visual.frmSolicitudOrganizador form, string valorCuit)
        {
            InterfazGrupoA.ControlDe<TextBox>(form, "responsable").Text = "Responsable de prueba";
            InterfazGrupoA.ControlDe<TextBox>(form, "cuit").Text = valorCuit;
            InterfazGrupoA.ControlDe<TextBox>(form, "telefono").Text = Telefono;
            InterfazGrupoA.ControlDe<TextBox>(form, "local").Text = "Sala de prueba";
            InterfazGrupoA.ControlDe<TextBox>(form, "direccion").Text = "Calle de prueba 123";
            InterfazGrupoA.ControlDe<NumericUpDown>(form, "capacidad").Value = 200;
            InterfazGrupoA.ControlDe<TextBox>(form, "cbu").Text = new string('0', 22);
            typeof(Visual.frmSolicitudOrganizador).GetField("imagen", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(form, Png);
            typeof(Visual.frmSolicitudOrganizador).GetField("nombreArchivo", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(form, "documentacion.png");
        }
    }
}
