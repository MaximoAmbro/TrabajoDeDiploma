using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual.Componentes;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasT45Detalles
    {
        public TestContext TestContext { get; set; }

        // CP-380 — RF-06/RF-07/RNF-02: el pago alojado conserva la reserva y termina una compra con su PDF real.
        [TestMethod("CP-380: El pago WebView2 en Mi cuenta completa la compra y genera el ticket")]
        public void CP380_PagoWebEnShellCompletaCompraConTicket()
        {
            EnSta(() =>
            {
                Fake.Limpiar();
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                var evento = Fake.EventoPublicado(25000m, 20);
                string[] rutasTicket = Array.Empty<string>();
                try
                {
                    using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, NombreUsuario = "Cliente" })
                    using (var paquete = new Visual.FrmCompraCliente { IDUsuario = cliente.Id, IDEvento = evento.Id, NombreUsuario = "Cliente" })
                    {
                    Mostrar(menu);
                    Assert.IsTrue(Visual.NavegacionCliente.AbrirDesdeMenu(paquete));
                    var sectores = ControlDe<DataGridView>(paquete, "dgvSectores");
                    var fila = sectores.Rows.Cast<DataGridViewRow>().First(r => Convert.ToInt32(r.Cells["Disponibles"].Value) > 0);
                    fila.Cells["Cantidad"].Value = 1;
                    sectores.EndEdit();
                    ControlDe<CheckBox>(paquete, "chkSeguro").Checked = true;
                    ControlDe<Button>(paquete, "btnComprar").PerformClick();
                    var pagoHost = EsperarValor(() => menu.Controls.Find("paginaprmPago", true).OfType<HostFormularioCliente>().FirstOrDefault()
                        ?? menu.Controls.Find("paginafrmPago", true).OfType<HostFormularioCliente>().FirstOrDefault(),
                        "El pago debe abrirse como una página del shell.");
                    var pago = (Visual.frmPago)pagoHost.Pantalla;
                    var web = ControlDe<Visual.Web.PanelWeb>(pago, "pagoWeb");
                    EsperarVistaWeb(web);
                    Assert.IsTrue(pago.Controls.Find("pagoWeb", true).Single().Visible);
                    StringAssert.Contains(EjecutarVistaWeb(web,
                        "(()=>{const f=document.querySelector('#form-pago');const i=document.querySelector('#numero');return ["+
                        "document.body.classList.contains('en-shell'),f.querySelector('label[for=numero]')!==null,"+
                        "i.getAttribute('aria-label'),i.getAttribute('aria-describedby'),document.querySelector('#titulo-pago').hidden||getComputedStyle(document.querySelector('#titulo-pago')).display==='none'].join('|')})()"),
                        "true|true|Número de tarjeta|ayuda-tarjeta|true");
                    string alineacion = EjecutarVistaWeb(web,
                        "(()=>{const x=document.querySelector('#total').getBoundingClientRect().left;return [x,...[...document.querySelectorAll('.campo > .rotulo')].filter(e=>e.parentElement.htmlFor!=='cvv').map(e=>e.getBoundingClientRect().left)].join('|')})()");
                    Assert.IsTrue(alineacion.Split('|').Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                        .All(x => Math.Abs(x - double.Parse(alineacion.Split('|')[0], System.Globalization.CultureInfo.InvariantCulture)) < 1),
                        "Pago, total y rótulos deben respetar un mismo eje izquierdo: " + alineacion);
                    StringAssert.Contains(EjecutarVistaWeb(web, "document.querySelector('#ayuda-tarjeta').textContent"), "4111 1111 1111 1111");
                    string idCompra = pago.Compra.Id.ToString();
                    TestContext?.WriteLine("CP-380 reserva activa al mostrar Pago: " + GestorCompras.Instance.VencimientoDe(pago.Compra).ToString("O"));
                    var tarjeta = Fake.Tarjeta();
                    EjecutarVistaWeb(web,
                        "(()=>{for(const [id,value] of Object.entries({titular:" + Newtonsoft.Json.JsonConvert.SerializeObject(tarjeta.Titular) + ","+
                        "numero:" + Newtonsoft.Json.JsonConvert.SerializeObject(tarjeta.Numero) + ",vencimiento:" + Newtonsoft.Json.JsonConvert.SerializeObject(tarjeta.Vencimiento) + ","+
                        "cvv:" + Newtonsoft.Json.JsonConvert.SerializeObject(tarjeta.Cvv) + "})){document.getElementById(id).value=value;}"+
                        "document.querySelector('#form-pago').requestSubmit();return 'enviado'})()");
                    var confirmacionHost = EsperarValor(() => menu.Controls.Find("paginafrmConfirmacion", true).OfType<HostFormularioCliente>().FirstOrDefault(),
                        "El pago aprobado debe navegar a la confirmación dentro del shell.");
                    var confirmacion = (Visual.frmConfirmacion)confirmacionHost.Pantalla;
                    rutasTicket = confirmacion.Rutas.ToArray();
                    Assert.IsTrue(rutasTicket.Length > 0, "La compra debe emitir al menos un PDF de ticket.");
                    Assert.IsTrue(rutasTicket.All(File.Exists), "Los archivos de ticket deben existir en disco después del pago.");
                    Assert.IsTrue(GestorCompras.Instance.PagosDe(pago.Compra).Any(p => p.Estado == EstadoPago.Aprobado),
                        "La compra debe quedar confirmada por la lógica original del negocio.");
                    Assert.AreEqual("1", confirmacion.Controls.Find("lstArchivos", true).OfType<ListBox>().Single().Items.Count.ToString());
                    TestContext?.WriteLine("CP-380 compra=" + idCompra + " tickets=" + string.Join(";", rutasTicket));
                    }
                }
                finally { foreach (string ruta in rutasTicket) if (File.Exists(ruta)) File.Delete(ruta); }
            });
        }

        // CP-379 — RNF-02: cada estado web expone un único título de pantalla y, en Mi cuenta, lo aporta el shell.
        [TestMethod("CP-379: Las vistas web no duplican el título de pantalla")]
        public void CP379_TitulosDeLasVistasNoSeDuplican()
        {
            EnSta(() =>
            {
                using (var formulario = new Form())
                using (var web = new Visual.Web.PanelWeb { Bounds = new System.Drawing.Rectangle(0, 0, 1100, 640) })
                {
                    formulario.Controls.Add(web);
                    Mostrar(formulario);
                    web.Cargar("gestion.html");
                    EsperarVistaWeb(web);
                    var vistas = new[]
                    {
                        new { Id = "historial", Titulo = "Compras recientes" },
                        new { Id = "locales", Titulo = "Mis locales" },
                        new { Id = "eventosLocal", Titulo = "Eventos del local" },
                        new { Id = "controlIngreso", Titulo = "Control de ingreso" },
                        new { Id = "auditoria", Titulo = "Auditoría" },
                        new { Id = "revisionEventos", Titulo = "Revisión de eventos" }
                    };
                    foreach (var vista in vistas)
                    {
                        EjecutarVistaWeb(web, "window.eventAuraDibujar({vista:'" + vista.Id + "'});'rendered'");
                        string estado = EjecutarVistaWeb(web,
                            "(()=>{const t=[...document.querySelectorAll('#app h1,#app h2')].map(x=>x.textContent.trim());" +
                            "return [(t.length?t[0]:''),(t.length?t.filter(x=>x===t[0]).length:0),document.querySelectorAll('#app h1').length].join('|')})()");
                        Assert.AreEqual(vista.Titulo + "|1|0", estado,
                            "La vista " + vista.Id + " no debe repetir el primer encabezado ni agregar un título de página dentro del contenido: " + estado);
                    }
                }

                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, NombreUsuario = "Cliente" })
                {
                    Mostrar(menu);
                    ControlDe<Button>(menu, "btnEventos").PerformClick();
                    var catalogo = ControlDe<HostFormularioCliente>(menu, "paginafrmEventosCliente");
                    var catalogoWeb = ControlDe<Visual.Web.PanelWeb>(catalogo.Pantalla, "catalogoWeb");
                    EsperarVistaWeb(catalogoWeb);
                    Assert.AreEqual("Eventos disponibles", menu.Controls.Find("label1", false).OfType<Label>().Single().Text);
                    Assert.IsFalse(ControlDe<Label>(catalogo.Pantalla, "label2").Visible,
                        "El catálogo alojado debe dejar el único título al shell.");
                    Assert.AreEqual("0", EjecutarVistaWeb(catalogoWeb, "document.querySelectorAll('h1').length.toString()"));

                    ControlDe<Button>(menu, "btnHistorial").PerformClick();
                    var historial = ControlDe<HostFormularioCliente>(menu, "paginafrmHistorialCliente");
                    var historialWeb = ControlDe<Visual.Web.PanelWeb>(historial.Pantalla, "historialWeb");
                    EsperarVistaWeb(historialWeb);
                    Assert.AreEqual("Mis entradas", menu.Controls.Find("label1", false).OfType<Label>().Single().Text);
                    Assert.IsFalse(ControlDe<Label>(historial.Pantalla, "label1").Visible,
                        "El historial alojado debe dejar el único título al shell.");
                    Assert.AreEqual("0", EjecutarVistaWeb(historialWeb, "document.querySelectorAll('h1').length.toString()"));

                    ControlDe<Button>(menu, "btnSuscripcion").PerformClick();
                    var suscripcion = ControlDe<HostFormularioCliente>(menu, "paginafrmSuscripcion");
                    var suscripcionWeb = ControlDe<Visual.Web.PanelWeb>(suscripcion.Pantalla, "suscripcionWeb");
                    EsperarVistaWeb(suscripcionWeb);
                    Assert.AreEqual("Mi suscripción", menu.Controls.Find("label1", false).OfType<Label>().Single().Text);
                    Assert.AreEqual("0", EjecutarVistaWeb(suscripcionWeb, "document.querySelectorAll('h1').length.toString()"));
                }
            });
        }

        // CP-378 — RNF-02/RNF-03: capturas de ejecución para revisar visualmente cada ojo propio.
        [TestMethod("CP-378: Las vistas de acceso, alta y recuperación muestran ojos accesibles")]
        public void CP378_CapturarOjosAccesoAltaYRecuperacion()
        {
            var emisorAnterior = SeleccionEnviadorCodigos.Actual;
            SeleccionEnviadorCodigos.Actual = new EnviadorSimulado();
            try
            {
                EnSta(() =>
                {
                    using (var acceso = new Visual.frmInicio())
                    {
                        Mostrar(acceso);
                        var web = ControlDe<Visual.Web.PanelWeb>(acceso, "accesoWeb");
                        EsperarVistaWeb(web);
                        Assert.AreEqual("1|true", EstadoOjos(web), "El ojo de acceso debe tener tamaño, ARIA y foco por Tab.");
                        PruebasInteraccionPantallas.CapturarSiSeSolicito(acceso, "Ojo-Acceso");
                    }

                    using (var alta = new Visual.frmCrearUsuario())
                    {
                        Mostrar(alta);
                        var web = ControlDe<Visual.Web.PanelWeb>(alta, "altaCuentaWeb");
                        EsperarVistaWeb(web);
                        Assert.AreEqual("ok", EjecutarVistaWeb(web, "document.querySelector('#continuar').click();'ok'"));
                        Esperar(() => EjecutarVistaWeb(web,
                            "!document.querySelector('[data-vista=datos]').classList.contains('oculto')") == "true",
                            "El alta debe mostrar su paso de datos para capturar ambos campos.");
                        Assert.AreEqual("2|true", EstadoOjos(web), "Los ojos del alta deben tener tamaño, ARIA y foco por Tab.");
                        PruebasInteraccionPantallas.CapturarSiSeSolicito(alta, "Ojo-Alta");
                    }

                    var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                    Fake.ComoCliente(cliente);
                    GestorCodigosCorreo.Instance.SolicitarRecuperacion(cliente.Mail);
                    using (var recuperar = new Visual.frmCambiarContraseña(true) { CorreoInicial = cliente.Mail })
                    {
                        Mostrar(recuperar);
                        var web = ControlDe<Visual.Web.PanelWeb>(recuperar, "recuperacionWeb");
                        EsperarVistaWeb(web);
                        web.Actualizar(new { paso = "codigo", mail = cliente.Mail });
                        Esperar(() => EjecutarVistaWeb(web,
                            "!document.querySelector('#form-codigo').classList.contains('oculto')") == "true",
                            "Recuperación debe mostrar los campos de contraseña para capturarlos.");
                        Assert.AreEqual("2|true", EstadoOjos(web), "Los ojos de recuperación deben tener tamaño, ARIA y foco por Tab.");
                        PruebasInteraccionPantallas.CapturarSiSeSolicito(recuperar, "Ojo-Recuperar");
                    }
                });
            }
            finally { SeleccionEnviadorCodigos.Actual = emisorAnterior; }
        }

        // CP-377 — RNF-02/RNF-03: el botón de contratación no debe quedar fuera del viewport del shell.
        [TestMethod("CP-377: Contratar suscripción queda completo a 1200x760 y 1100x720")]
        public void CP377_ContratarVisibleEnShell()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                foreach (var tamano in new[] { new System.Drawing.Size(1200, 760), new System.Drawing.Size(1100, 720) })
                {
                    using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, ClientSize = tamano })
                    {
                        Mostrar(menu);
                        ControlDe<Button>(menu, "btnSuscripcion").PerformClick();
                        var host = ControlDe<HostFormularioCliente>(menu, "paginafrmSuscripcion");
                        var web = ControlDe<Visual.Web.PanelWeb>((Visual.frmSuscripcion)host.Pantalla, "suscripcionWeb");
                        EsperarVistaWeb(web);
                        string geometria = EjecutarVistaWeb(web,
                            "(()=>{const b=document.querySelector('#contratar'),r=b.getBoundingClientRect();return [!b.classList.contains('oculto'),r.top>=0,r.bottom<=innerHeight,document.documentElement.scrollHeight<=innerHeight,innerHeight,r.top,r.bottom].join('|')})()");
                        TestContext?.WriteLine("CP-377 " + tamano + ": " + geometria);
                        Assert.IsTrue(geometria.StartsWith("true|true|true|true|", StringComparison.Ordinal),
                            "El botón Contratar tiene que verse completo sin desplazamiento, en " + tamano + ": " + geometria);
                        PruebasInteraccionPantallas.CapturarSiSeSolicito(menu, "SuscripcionT45-" + tamano.Width + "x" + tamano.Height);
                    }
                }
            });
        }

        // CP-375 — RF-14: la verificación en sesión conserva el título de la vista y muestra el código en el host.
        [TestMethod("CP-375: Verificación de correo en Mi cuenta muestra su título y el código dentro de la vista")]
        public void CP375_VerificacionEnShellVisible()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                EjecutarSqlAviso("UPDATE dbo.Usuario SET CorreoVerificado=0 WHERE ID=@ID", "@ID", cliente.Id);
                using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, NombreUsuario = "Cliente" })
                {
                    Mostrar(menu);
                    Assert.IsNotNull(GestorSesion.Instance.UsuarioActual, "La sesión de cliente debe seguir activa.");
                    menu.IniciarVerificacionCorreo();
                    var host = menu.Controls.Find("paginafrmCrearUsuario", true).OfType<HostFormularioCliente>().SingleOrDefault();
                    Assert.IsNotNull(host, "Debe abrirse la vista de verificación dentro de Mi cuenta. Aviso: " +
                        menu.Controls.Find("avisoSistema", true).FirstOrDefault()?.Text);
                    var formulario = (Visual.frmCrearUsuario)host.Pantalla;
                    var web = ControlDe<Visual.Web.PanelWeb>(formulario, "altaCuentaWeb");
                    EsperarVistaWeb(web);
                    Assert.AreEqual("Verificar correo", menu.Controls.Find("label1", true)
                        .Single(c => c.Text == "Verificar correo").Text);
                    string visible = EjecutarVistaWeb(web,
                        "(()=>{const e=document.querySelector('#codigo-correo'),r=e.getBoundingClientRect(),a=document.querySelector('[data-vista=correo]').getBoundingClientRect();return [e.offsetParent!==null,r.top>=0,r.bottom<=innerHeight,a.top>=0,a.bottom<=innerHeight,innerHeight,r.top,r.bottom,a.top,a.bottom].join('|')})()");
                    TestContext?.WriteLine("CP-375 geometría: " + visible);
                    Assert.IsTrue(visible.StartsWith("true|true|true|true|true|", StringComparison.Ordinal),
                        "El campo y la sección de verificación deben entrar sin desplazar dentro de Mi cuenta.");
                    PruebasInteraccionPantallas.CapturarSiSeSolicito(menu, "VerificacionCorreoT45");
                }
            });
        }
        // CP-376 — RF-03/RN-43: volver del rechazo de compra conserva el catálogo WebView2 en el shell.
        [TestMethod("CP-376: El rechazo por correo sin verificar vuelve al mismo catálogo web")]
        public void CP376_RN43ConservaCatalogoWeb()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                EjecutarSqlAviso("UPDATE dbo.Usuario SET CorreoVerificado=0 WHERE ID=@ID", "@ID", cliente.Id);
                var evento = Fake.EventoPublicado(1000m, 2);
                using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, NombreUsuario = "Cliente" })
                using (var catalogo = new Visual.frmEventosCliente { IDUsuario = cliente.Id, NombreUsuario = "Cliente" })
                {
                    Mostrar(menu);
                    Assert.IsTrue(Visual.NavegacionCliente.AbrirDesdeMenu(catalogo));
                    var webInicial = ControlDe<Visual.Web.PanelWeb>(catalogo, "catalogoWeb");
                    EsperarVistaWeb(webInicial);
                    var tarjeta = ControlDe<FlowLayoutPanel>(catalogo, "tarjetasEventos").Controls.OfType<TarjetaEvento>()
                        .Single(t => t.AccessibleName == evento.Nombre);
                    ControlDe<Button>(tarjeta, "verEntradas").PerformClick();
                    ControlDe<Button>(ControlDe<FichaEvento>(catalogo, "fichaEvento"), "comprarFicha").PerformClick();
                    Esperar(() => ControlDe<Button>(menu, "btnVerificarCorreoSesion").Visible,
                        "El rechazo RN-43 debe mostrar la acción de verificación.");
                    Assert.IsTrue(ReferenceEquals(catalogo, ControlDe<Visual.frmEventosCliente>(menu, "frmEventosCliente")),
                        "La vuelta del rechazo debe restaurar el catálogo existente, no crear un segundo WebView2.");
                    Assert.IsTrue(webInicial.Preparado,
                        "El catálogo web previo debe conservar su controlador después del rechazo.");
                    ControlDe<Button>(ControlDe<FichaEvento>(catalogo, "fichaEvento"), "volverFicha").PerformClick();
                    Assert.IsTrue(webInicial.Preparado && webInicial.Visible,
                        "Al volver a la lista, el catálogo web debe seguir activo y no caer al respaldo.");
                    Assert.IsTrue(ControlDe<Panel>(menu, "avisoClienteEnLinea").Visible);
                }
            });
        }
        private static void EsperarVistaWeb(Visual.Web.PanelWeb panel)
        {
            var reloj = Stopwatch.StartNew();
            while (!panel.InicializacionTerminada && reloj.Elapsed < TimeSpan.FromSeconds(20))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(panel.Preparado && panel.Visible, "WebView2 debe estar disponible para verificar T-45.");
        }

        private static string EstadoOjos(Visual.Web.PanelWeb panel)
        {
            return EjecutarVistaWeb(panel,
                "(()=>{const b=[...document.querySelectorAll('input[type=password] + .ojo-contrasena')];return b.length+'|'+b.every(x=>{const r=x.getBoundingClientRect();return r.width===40&&r.height===40&&x.getAttribute('aria-label')==='Mostrar contraseña'&&x.getAttribute('aria-pressed')==='false'&&x.tabIndex>=0})})()");
        }

        private static string EjecutarVistaWeb(Visual.Web.PanelWeb panel, string script)
        {
            var tarea = panel.EjecutarScriptAsync(script);
            var reloj = Stopwatch.StartNew();
            while (!tarea.IsCompleted && reloj.Elapsed < TimeSpan.FromSeconds(10))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(tarea.IsCompleted, "WebView2 debe responder a la comprobación de T-45.");
            return (tarea.GetAwaiter().GetResult() ?? string.Empty).Trim('"');
        }

        private static void Esperar(Func<bool> condicion, string mensaje)
        {
            var reloj = Stopwatch.StartNew();
            while (!condicion() && reloj.Elapsed < TimeSpan.FromSeconds(10))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(condicion(), mensaje);
        }

        private static T EsperarValor<T>(Func<T> obtener, string mensaje) where T : class
        {
            var reloj = Stopwatch.StartNew();
            T valor = obtener();
            while (valor == null && reloj.Elapsed < TimeSpan.FromSeconds(20))
            {
                Application.DoEvents();
                Thread.Sleep(15);
                valor = obtener();
            }
            Assert.IsNotNull(valor, mensaje);
            return valor;
        }

        private static void EjecutarSqlAviso(string sql, params object[] parametros)
        {
            using (var conexion = new Microsoft.Data.SqlClient.SqlConnection(BaseDePrueba.CadenaDePrueba()))
            using (var comando = new Microsoft.Data.SqlClient.SqlCommand(sql, conexion))
            {
                for (int i = 0; i < parametros.Length; i += 2)
                    comando.Parameters.AddWithValue(parametros[i].ToString(), parametros[i + 1]);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
