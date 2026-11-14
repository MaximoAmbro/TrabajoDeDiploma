using System;
using System.Linq;
using System.Reflection;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasRevisionSeguridad
    {
        // CP-113 — RN-41/RN-42, R-01: contrato público y snapshot de autenticación.
        [DataTestMethod]
        [DataRow("inicio", DisplayName = "CP-113: Iniciar no pertenece al contrato público de sesión")]
        [DataRow("salida", DisplayName = "CP-113: La sesión expone solo propiedades de lectura")]
        [DataRow("perfil", DisplayName = "CP-113: Mutar el perfil de entrada no eleva privilegios")]
        [DataRow("identidad", DisplayName = "CP-113: Mutar el ID de entrada no suplanta otra cuenta")]
        public void CP113_SesionProtegida(string caso)
        {
            var sesion = GestorSesion.Instance;
            sesion.Cerrar();
            try
            {
                if (caso == "inicio")
                {
                    MethodInfo iniciar = typeof(GestorSesion).GetMethod("Iniciar", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Assert.IsNotNull(iniciar);
                    Assert.IsTrue(iniciar.IsAssembly, "Iniciar debe ser internal.");
                    return;
                }
                var original = new Usuario { Id = 101, TipoUsuario = "Cliente", Nombre = "Cliente" };
                sesion.Iniciar(original);
                if (caso == "salida")
                {
                    object actual = sesion.UsuarioActual;
                    foreach (PropertyInfo propiedad in actual.GetType().GetProperties())
                        Assert.IsFalse(propiedad.SetMethod != null && propiedad.SetMethod.IsPublic,
                            propiedad.Name + " permite modificar el objeto de sesión.");
                }
                else if (caso == "perfil")
                {
                    original.TipoUsuario = "Propietario";
                    Assert.ThrowsException<ReglaNegocioException>(() => sesion.RequerirAdministrador());
                }
                else
                {
                    original.Id = 202;
                    Assert.ThrowsException<ReglaNegocioException>(() => sesion.RequerirUsuario(202));
                    sesion.RequerirUsuario(101);
                }
            }
            finally { sesion.Cerrar(); }
        }
        // CP-114 — RN-04, K-3: rechazo público idéntico sin revelar existencia ni bloqueo.
        [DataTestMethod]
        [DataRow(0, DisplayName = "CP-114: Correo existente e inexistente muestran el mismo rechazo")]
        [DataRow(4, DisplayName = "CP-114: El quinto fallo bloquea internamente sin enumerar cuentas")]
        [DataRow(5, DisplayName = "CP-114: Una cuenta bloqueada no revela su existencia")]
        public void CP114_RechazoUniforme(int fallos)
        {
            InterfazGrupoA.EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Datos.RepositorioUsuarios.Instance.ActualizarContrasena(cliente.Mail, Seguridad.Hash("Cliente123"));
                Datos.RepositorioUsuarios.Instance.ActualizarIntentos(cliente.Id, fallos,
                    fallos == 5 ? DateTime.Now.AddMinutes(10) : (DateTime?)null);
                using (var form = new Visual.frmInicio())
                {
                    InterfazGrupoA.Mostrar(form);
                    var respuestas = new System.Collections.Generic.List<DialogoEsperado>();
                    foreach (string mail in new[] { cliente.Mail, Fake.MailNuevo() })
                    {
                        InterfazGrupoA.ControlDe<System.Windows.Forms.TextBox>(form, "txtMailLogin").Text = mail;
                        InterfazGrupoA.ControlDe<System.Windows.Forms.TextBox>(form, "txtContraseñaLogin").Text = "Incorrecta123";
                        var esperado = new DialogoEsperado { Titulo = "Inicio de sesión", Texto = "Mail o contraseña incorrectos." };
                        DialogosPrueba.Comprobar(() => InterfazGrupoA.ControlDe<System.Windows.Forms.Button>(form, "btninicio").PerformClick(), esperado);
                        respuestas.Add(esperado);
                        Assert.IsTrue(form.Visible);
                        Assert.IsFalse(GestorSesion.Instance.HaySesion);
                        Assert.AreEqual(1, System.Windows.Forms.Application.OpenForms.Count);
                    }
                    Assert.AreEqual(respuestas[0].TextoObservado, respuestas[1].TextoObservado);
                    Assert.AreEqual(respuestas[0].TituloObservado, respuestas[1].TituloObservado);
                    var persistido = Datos.RepositorioUsuarios.Instance.ObtenerPorMail(cliente.Mail);
                    Assert.AreEqual(Math.Min(5, fallos + 1), persistido.IntentosFallidos);
                    Assert.AreEqual(fallos >= 4, persistido.BloqueadoHasta.HasValue);
                }
            });
        }
        // CP-115 — RN-20, K-4: se respeta el tope configurado, sin recortar silenciosamente el descuento.
        [DataTestMethod]
        [DataRow(NivelFidelidad.Plata, "0.085", false, DisplayName = "CP-115: Plata admite descuento inferior al máximo")]
        [DataRow(NivelFidelidad.Plata, "0.086", false, DisplayName = "CP-115: Plata admite descuento igual al máximo")]
        [DataRow(NivelFidelidad.Plata, "0.087", true, DisplayName = "CP-115: Plata rechaza descuento superior al máximo")]
        [DataRow(NivelFidelidad.Oro, "0.085", false, DisplayName = "CP-115: Oro admite descuento inferior al máximo")]
        [DataRow(NivelFidelidad.Oro, "0.086", false, DisplayName = "CP-115: Oro admite descuento igual al máximo")]
        [DataRow(NivelFidelidad.Oro, "0.087", true, DisplayName = "CP-115: Oro rechaza descuento superior al máximo")]
        public void CP115_TopeDeDescuento(NivelFidelidad nivel, string valor, bool rechazar)
        {
            string clave = nivel == NivelFidelidad.Oro ? "DESCUENTO_ORO" : "DESCUENTO_PLATA";
            string anterior = GestorParametros.Instance.Obtener(clave);
            string maximo = GestorParametros.Instance.Obtener("DESCUENTO_MAX_ADMISIBLE");
            try
            {
                Fake.ConfigurarParametro("DESCUENTO_MAX_ADMISIBLE", "0.086");
                Fake.ConfigurarParametro(clave, valor);
                if (rechazar)
                    Assert.AreEqual("RN-20", Assert.ThrowsException<ReglaNegocioException>(() =>
                        GestorFidelizacion.Instance.PorcentajePara(nivel)).Regla);
                else
                    Assert.AreEqual(decimal.Parse(valor, System.Globalization.CultureInfo.InvariantCulture),
                        GestorFidelizacion.Instance.PorcentajePara(nivel));
            }
            finally
            {
                Fake.ConfigurarParametro(clave, anterior);
                Fake.ConfigurarParametro("DESCUENTO_MAX_ADMISIBLE", maximo);
            }
        }
    }
}
