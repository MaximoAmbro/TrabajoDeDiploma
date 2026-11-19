using System;
using System.Linq;
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

        // CP-184 — RF-12, RN-50: una solicitud pendiente no otorga permisos de Organizador.
        [TestMethod]
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

        // CP-185 — RF-12, RN-52: rechazar CUIT con digito verificador incorrecto.
        [TestMethod]
        public void CP185_CuitConVerificadorIncorrectoSeRechaza()
        {
            var solicitud = SolicitudValida(); solicitud.Cuit = "20-12345678-5";
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(solicitud));
            Assert.AreEqual("RN-52", error.Regla);
            Assert.AreEqual("El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.", error.Message);
        }

        // CP-186 — RF-12, RN-52: rechazar CBU con digito verificador incorrecto.
        [TestMethod]
        public void CP186_CbuConVerificadorIncorrectoSeRechaza()
        {
            var solicitud = SolicitudValida(); solicitud.CbuAlias = "0000000000000000000001";
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(solicitud));
            Assert.AreEqual("RN-52", error.Regla);
            Assert.AreEqual("Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.", error.Message);
        }

        // CP-187 — RF-12, RN-53: reservar la aprobacion de solicitudes al Administrador.
        [TestMethod]
        public void CP187_ClienteNoPuedeAprobarSolicitud()
        {
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Aprobar(1, "Revisada"));
            Assert.AreEqual("RN-34", error.Regla);
            Assert.AreEqual("Esta operación es solo para el Administrador", error.Message);
        }

        // CP-188 — RF-12, RN-53: exigir un motivo para rechazar una solicitud.
        [TestMethod]
        public void CP188_RechazoSinMotivoSeRechaza()
        {
            Fake.ComoAdministrador();
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Rechazar(1, "  "));
            Assert.AreEqual("RN-53", error.Regla);
            Assert.AreEqual("Escribí el motivo del rechazo.", error.Message);
        }

        // CP-190 — RF-12, RN-53: impedir que un Organizador revise solicitudes de plataforma.
        [TestMethod]
        public void CP190_OrganizadorNoPuedeRechazarSolicitudDePlataforma()
        {
            Fake.ComoOrganizador();
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Rechazar(1, "Motivo de prueba."));
            Assert.AreEqual("RN-34", error.Regla);
            Assert.AreEqual("Esta operación es solo para el Administrador", error.Message);
        }

        // CP-191 — RF-12, RN-51: exigir la confirmacion del telefono antes de solicitar perfil Organizador.
        [TestMethod]
        public void CP191_TelefonoSinConfirmarImpideLaSolicitud()
        {
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(SolicitudValida()));
            Assert.AreEqual("RN-51", error.Regla);
            Assert.AreEqual("Verificá este teléfono con el código enviado antes de presentar la solicitud.", error.Message);
        }

        // CP-192 — RF-12, RN-52: rechazar documentacion que no sea imagen valida.
        [TestMethod]
        public void CP192_DocumentacionQueNoEsImagenSeRechaza()
        {
            VerificarTelefono();
            var solicitud = SolicitudValida(); solicitud.Documentacion = new byte[] { 1,2,3,4 };
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.Solicitar(solicitud));
            Assert.AreEqual("RN-52", error.Regla);
            Assert.AreEqual("Adjuntá una imagen PNG o JPEG válida, comprimida y de hasta 512 KB.", error.Message);
        }

        // CP-193 — RF-12, RN-53: mostrar la solicitud y proteger su revision administrativa.
        [TestMethod]
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

        // CP-194 — RF-12, RN-52: aceptar alias bancario con el formato definido.
        [TestMethod]
        public void CP194_AliasBancarioEsAceptado()
        {
            var enviador = new EnviadorSimulado(); SeleccionEnviadorCodigos.Actual = enviador;
            VerificarTelefono();
            var solicitud = SolicitudValida(); solicitud.CbuAlias = "eventaura.demo";
            Assert.IsTrue(GestorSolicitudesOrganizador.CbuAliasValido(solicitud.CbuAlias));
            Assert.IsTrue(GestorSolicitudesOrganizador.Instance.Solicitar(solicitud) > 0);
        }

        // CP-196 — RF-12, RN-45/RN-53: auditar la decision de rechazo y su motivo.
        [TestMethod]
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

        // CP-195 — RF-12, RN-51: rechazar telefonos fuera del formato argentino.
        [TestMethod]
        public void CP195_TelefonoNoArgentinoSeRechaza()
        {
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono("0000000000"));
            Assert.AreEqual("RN-51", error.Regla);
            Assert.AreEqual("Ingresá un teléfono argentino válido, con característica y número.", error.Message);
        }

        // CP-189 — RF-12, RN-45/RN-53: promover al Organizador, crear su local y auditar la aprobacion.
        [TestMethod]
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

        // CP-202 — RF-01, RN-44/RN-51: el código telefónico no se muestra hasta abrir la bandeja simulada.
        [TestMethod("CP-202: El código telefónico solo aparece al abrir la bandeja simulada")]
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
                    Assert.AreEqual("Si el teléfono es válido, enviamos un código a la bandeja simulada.",
                        InterfazGrupoA.ControlDe<Label>(form, "estadoEnvioTelefono").Text);
                    Assert.IsFalse(TextosVisibles(form).Any(t => t.Contains(mensaje.Codigo)),
                        "El código no debe aparecer en el formulario que lo solicita.");
                    Assert.IsTrue(AbrirBandejaYVerCodigo(form, mensaje.Codigo));
                }
            });
        }

        private static System.Collections.Generic.IEnumerable<string> TextosVisibles(Control padre)
        {
            foreach (Control control in padre.Controls)
            {
                if (!string.IsNullOrEmpty(control.Text)) yield return control.Text;
                foreach (string texto in TextosVisibles(control)) yield return texto;
            }
        }

        private static bool AbrirBandejaYVerCodigo(Control origen, string codigo)
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
                    codigoVisible = InterfazGrupoA.ControlDe<Label>(bandeja, "mensajeBandejaSimulada").Text.Contains(codigo);
                    bandeja.Close();
                    temporizador.Stop();
                };
                temporizador.Start();
                InterfazGrupoA.ControlDe<Button>(origen, "btnAbrirBandejaTelefono").PerformClick();
                temporizador.Stop();
            }
            return abierta && codigoVisible;
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
    }
}
