using System;
using System.Linq;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Pruebas
{
    [TestClass]
    public class PruebasSeguridad
    {
        internal static void EnStaSinCambiarDatos(Action accion)
        {
            Exception error = null;
            var hilo = new System.Threading.Thread(() => {
                try { accion(); }
                catch (Exception ex) { error = ex; }
            }) { IsBackground = true };
            hilo.SetApartmentState(System.Threading.ApartmentState.STA);
            hilo.Start();
            Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(20)), "La verificación de pantalla no terminó.");
            if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        }
        [TestInitialize]
        public void Preparar() { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }

        [TestCleanup]
        public void Terminar() { GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }

        // CP-91 — RF-08, RN-41 propuesta: el dueño se relee, aun si el objeto se falsifica.
        [TestMethod("CP-91: Un cliente no reembolsa la compra de otro")]
        public void ReembolsoAjeno()
        {
            Cliente propietario = Fake.Cliente(NivelFidelidad.Bronce);
            Cliente intruso = Fake.Cliente(NivelFidelidad.Bronce);
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Compra compra = Fake.CompraConfirmada(propietario, evento, 1, true);
            var entradas = GestorCompras.Instance.EntradasDe(compra);
            Fake.ComoCliente(intruso);
            compra.IdUsuario = intruso.Id;
            entradas.ForEach(e => e.IdUsuario = intruso.Id);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorCompras.Instance.Reembolsar(entradas));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorCompras.Instance.EntradasDe(compra));
            Fake.ComoCliente(propietario);
            Assert.AreEqual(EstadoCompra.Confirmada, GestorCompras.Instance.ObtenerPorId(compra.Id).Estado);
            Assert.AreEqual(0m, GestorCompras.Instance.ObtenerPorId(compra.Id).MontoReembolsado);
        }

        // CP-92 — RF-08, RN-41 propuesta: datos privados del titular.
        [TestMethod("CP-92: Un cliente no consulta el historial de otro")]
        public void HistorialAjeno()
        {
            Cliente propietario = Fake.Cliente(NivelFidelidad.Bronce);
            Cliente intruso = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoCliente(intruso);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorCompras.Instance.HistorialDe(propietario.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorSuscripciones.Instance.ActivaDe(propietario.Id));
            Assert.ThrowsException<ReglaNegocioException>(() => GestorFidelizacion.Instance.ObtenerCliente(propietario.Id));
        }

        // CP-93 — RF-12, RN-34 generalizada: gestión reservada al administrador.
        [TestMethod("CP-93: Un cliente no elimina eventos")]
        public void EliminarEventoAjeno()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Fake.ComoCliente(Fake.Cliente(NivelFidelidad.Bronce));
            var error = Assert.ThrowsException<ReglaNegocioException>(() => GestorEventos.Instance.Eliminar(evento.Id));
            Assert.AreEqual("RN-34", error.Regla);
            Assert.IsNotNull(GestorEventos.Instance.ObtenerPorId(evento.Id));
        }

        // CP-94 — RF-04/RF-12/RF-13, RN-41/42 propuestas: sin sesión se rechaza antes de escribir.
        [TestMethod("CP-94: Las operaciones protegidas rechazan la ausencia de sesión")]
        public void SinSesion()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Fake.ComoCliente(cliente);
            Paquete paquete = Fake.Paquete(evento, 1, false);
            Compra compra = GestorCompras.Instance.Iniciar(paquete, cliente);
            GestorSesion.Instance.Cerrar();
            Action[] operaciones = {
                () => GestorEventos.Instance.Crear(new Evento()),
                () => GestorEventos.Instance.Publicar(evento),
                () => GestorEventos.Instance.Eliminar(evento.Id),
                () => GestorEventos.Instance.Suspender(evento.Id),
                () => GestorEventos.Instance.CambiarPreventa(evento.Id, null),
                () => GestorEventos.Instance.AgregarSector(evento.Id, new Sector()),
                () => GestorEventos.Instance.EliminarSector(evento.Sectores.Single().Id),
                () => GestorLocales.Instance.Registrar(new Local()),
                () => GestorListaEspera.Instance.Inscribir(evento.Id, cliente.Id),
                () => GestorCompras.Instance.Iniciar(paquete, cliente),
                () => GestorCompras.Instance.Confirmar(compra, Fake.Tarjeta()),
                () => GestorCompras.Instance.Liberar(compra),
                () => GestorSuscripciones.Instance.Contratar(cliente.Id, GestorSuscripciones.PlanMensual),
                () => GestorSuscripciones.Instance.Cancelar(cliente.Id),
                () => GestorFidelizacion.Instance.ReevaluarNivel(cliente, DateTime.Now),
                () => GestorFidelizacion.Instance.AscenderSiCorresponde(cliente, DateTime.Now)
            };
            foreach (Action operacion in operaciones)
                Assert.ThrowsException<ReglaNegocioException>(operacion);
            string mail = Fake.MailNuevo();
            Assert.ThrowsException<ReglaNegocioException>(() => GestorUsuarios.Instance.CrearAdministrador(mail, "ClaveSegura123", "Admin", "Prueba"));
            Fake.ComoCliente(cliente);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorUsuarios.Instance.CrearAdministrador(mail, "ClaveSegura123", "Admin", "Prueba"));
            Assert.IsNull(Datos.RepositorioUsuarios.Instance.ObtenerPorMail(mail));
            Fake.ComoAdministrador();
            GestorUsuarios.Instance.CrearAdministrador(mail, "ClaveSegura123", "Admin", "Prueba");
            Assert.AreEqual("Propietario", Datos.RepositorioUsuarios.Instance.ObtenerPorMail(mail).TipoUsuario);
            Assert.IsNull(typeof(GestorUsuarios).GetMethod("Registrar"), "El registro genérico no debe ser público.");
            Fake.ComoCliente(cliente);
            Assert.AreEqual(EstadoCompra.Pendiente, GestorCompras.Instance.ObtenerPorId(compra.Id).Estado);
            Assert.IsNull(GestorSuscripciones.Instance.ActivaDe(cliente.Id));
        }

        // CP-95 — RF-02, RN-04/40 propuesta: contraseña actual y contador de intentos.
        [TestMethod("CP-95: Cambiar contraseña rechaza la actual incorrecta y suma un intento")]
        public void ActualIncorrecta()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.CambiarContrasena(cliente.Mail, "Incorrecta1", "NuevaClave123"));
            Assert.AreEqual(1, Datos.RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).IntentosFallidos);
            Assert.IsNotNull(GestorUsuarios.Instance.Autenticar(cliente.Mail, "Cliente123"));
        }

        // CP-96 — RF-02, RN-04: cambiar la contraseña respeta el bloqueo existente.
        [TestMethod("CP-96: La cuenta bloqueada no cambia su contraseña")]
        public void CambioBloqueado()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Datos.RepositorioUsuarios.Instance.ActualizarIntentos(cliente.Id, 5, DateTime.Now.AddMinutes(15));
            var error = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.CambiarContrasena(cliente.Mail, "Cliente123", "NuevaClave123"));
            StringAssert.Contains(error.Message, "bloqueada");
            Fake.VencerBloqueo(cliente.Mail);
            Assert.IsNotNull(GestorUsuarios.Instance.Autenticar(cliente.Mail, "Cliente123"));
        }

        // CP-97 — RF-02, RN-40 propuesta: respuesta uniforme y sin revelar la existencia del mail.
        [TestMethod("CP-97: Mail inexistente y contraseña actual incorrecta muestran lo mismo")]
        public void CambioNoEnumera()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            var existente = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.CambiarContrasena(cliente.Mail, "Incorrecta1", "NuevaClave123"));
            var inexistente = Assert.ThrowsException<ReglaNegocioException>(() =>
                GestorUsuarios.Instance.CambiarContrasena(Fake.MailNuevo(), "Incorrecta1", "NuevaClave123"));
            Assert.AreEqual(existente.Regla, inexistente.Regla);
            Assert.AreEqual(existente.Message, inexistente.Message);
        }

        // CP-98 — RF-02, RN-40 propuesta: cambio real sin sustituir la sesión activa.
        [TestMethod("CP-98: Cambiar contraseña invalida la anterior y conserva la sesión")]
        public void CambioCorrecto()
        {
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Fake.ComoAdministrador();
            int administrador = GestorSesion.Instance.UsuarioActual.Id;
            GestorUsuarios.Instance.CambiarContrasena(cliente.Mail, "Cliente123", "NuevaClave123");
            Assert.AreEqual(administrador, GestorSesion.Instance.UsuarioActual.Id);
            Assert.ThrowsException<ReglaNegocioException>(() => GestorUsuarios.Instance.Autenticar(cliente.Mail, "Cliente123"));
            Assert.IsNotNull(GestorUsuarios.Instance.Autenticar(cliente.Mail, "NuevaClave123"));
            EnStaSinCambiarDatos(() => {
                using (var form = new Visual.frmCambiarContraseña())
                {
                    form.Show();
                    System.Windows.Forms.Application.DoEvents();
                    var mostrar = form.Controls.Find("checkbxShowPass", true).Single();
                    Assert.IsTrue(form.ClientRectangle.Contains(mostrar.Bounds), "Mostrar contraseñas debe quedar dentro de la pantalla.");
                    foreach (string nombre in new[] { "txtMail", "txtActual", "TxtPassword", "TxtConfirmpassword" })
                    {
                        var campo = (System.Windows.Forms.TextBox)form.Controls.Find(nombre, true).Single();
                        Assert.AreEqual("", campo.Text);
                        Assert.IsTrue(campo.Visible);
                        if (nombre != "txtMail") Assert.AreEqual('*', campo.PasswordChar);
                    }
                    string carpeta = System.IO.Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Seguridad");
                    System.IO.Directory.CreateDirectory(carpeta);
                    using (var imagen = new System.Drawing.Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(imagen, new System.Drawing.Rectangle(0, 0, form.Width, form.Height));
                        imagen.Save(System.IO.Path.Combine(carpeta, "cambiar-contrasena.png"));
                    }
                }
            });
        }

        // CP-106 — RF-03, SEG-7: los comodines se buscan como texto literal.
        [TestMethod("CP-106: La búsqueda trata corchetes, porcentajes y guiones bajos como texto")]
        public void BusquedaLiteral()
        {
            Evento evento = Fake.EventoPublicado(1000m, 5);
            Fake.CambiarNombreDeEvento(evento.Id, "test Festival [vip] 50% sector_unico");
            foreach (string texto in new[] { "[vip]", "50%", "sector_unico" })
                Assert.AreEqual(evento.Id, GestorEventos.Instance.Buscar(texto, null, null).Single().Id);
            Assert.AreEqual(0, GestorEventos.Instance.Buscar("sector%unico", null, null).Count);
            Assert.AreEqual(0, GestorEventos.Instance.Buscar("sector_unic_", null, null).Count);
        }

        // CP-107 — RF-02, SEG-8: hashes malformados no autentican ni propagan errores numéricos.
        [TestMethod("CP-107: Hashes con iteraciones inválidas o desbordadas se rechazan")]
        public void HashMalformado()
        {
            string salt = Convert.ToBase64String(new byte[16]);
            string hash = Convert.ToBase64String(new byte[32]);
            foreach (string iteraciones in new[] { "9999999999999999999999", "0", "-1", "texto" })
                Assert.IsFalse(Seguridad.Verificar("Clave1234", "PBKDF2$" + iteraciones + "$" + salt + "$" + hash));
            Assert.IsFalse(Seguridad.Verificar("Clave1234", "PBKDF2$1$$"));
        }

        // CP-108 — RF-02, SEG-6: fixture generado con Seguridad.Hash a 100000 ANTES de subir la constante.
        [TestMethod("CP-108: Un hash previo de 100000 verifica y migra a 310000 sin cambiar la contraseña")]
        public void CompatibilidadHashAnterior()
        {
            const string anterior = "PBKDF2$100000$HVndjLjBNMOmobOns/9Usg==$zjhFS5t/Bw+cMaul+4MTsmw3oWnUDWkoFuYxK9hxB+c=";
            Assert.AreEqual("100000", anterior.Split('$')[1]);
            Assert.IsTrue(Seguridad.Verificar("Compatibilidad123", anterior));
            Assert.IsFalse(Seguridad.Verificar("Incorrecta1", anterior));
            Assert.IsTrue(Seguridad.NecesitaActualizar(anterior));
            string nuevo = Seguridad.Hash("Compatibilidad123");
            Assert.AreEqual("310000", nuevo.Split('$')[1]);
            Assert.IsTrue(Seguridad.Verificar("Compatibilidad123", nuevo));
            Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
            Datos.RepositorioUsuarios.Instance.ActualizarContrasena(cliente.Mail, anterior);
            Assert.AreEqual(cliente.Id, GestorUsuarios.Instance.Autenticar(cliente.Mail, "Compatibilidad123").Id);
            string actualizado = Datos.RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña;
            Assert.AreEqual("310000", actualizado.Split('$')[1]);
            Assert.IsTrue(Seguridad.Verificar("Compatibilidad123", actualizado));
            Assert.IsFalse(Seguridad.NecesitaActualizar(actualizado));
            Assert.AreEqual(cliente.Id, GestorUsuarios.Instance.Autenticar(cliente.Mail, "Compatibilidad123").Id);
            Assert.AreEqual(actualizado, Datos.RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail).Contraseña);
        }
    }
}
