using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Entidades;
using Datos;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual.Componentes;
using static Pruebas.InterfazGrupoA;
namespace Pruebas
{
    [TestClass]
    public class PruebasVistasVisibles
    {
        // CP-163 — RF-05, RNF-03: comprobar textos y color renderizado de las vistas compuestas.
        [DataTestMethod]
        [DataRow(typeof(TarjetaEvento),DisplayName="CP-163: Botones de tarjeta completos y primario renderizado")]
        [DataRow(typeof(FichaEvento),DisplayName="CP-163: Botones de ficha completos y primario renderizado")]
        [DataRow(typeof(InicioCliente),DisplayName="CP-163: Botones de inicio completos y primario renderizado")]
        [DataRow(typeof(EstadoVacio),DisplayName="CP-163: Acción de estado vacío con texto completo")]
        [DataRow(typeof(Visual.frmAuditoria),DisplayName="CP-163: Pantalla de auditoria con accion visible")]
        [DataRow(typeof(Visual.frmRevisarEventos),DisplayName="CP-163: Pantalla de revision con boton primario renderizado")]
        public void CP163_Botones(Type tipo)
        {
            PruebasSeguridad.EnStaSinCambiarDatos(()=>{
                if(tipo==typeof(Visual.frmRevisarEventos)) Fake.ComoAdministrador();
                using(var vista=(Control)Activator.CreateInstance(tipo)) {
                    if(vista is EstadoVacio)((EstadoVacio)vista).Cargar("Sin datos","Ver todos");
                    foreach(Button boton in Descendientes(vista).OfType<Button>()) {
                        using(var imagen=new Bitmap(boton.Width,boton.Height)) {
                            boton.DrawToBitmap(imagen,boton.ClientRectangle);
                            if(boton is BotonPrimario) {
                                int violetas=0;for(int y=0;y<imagen.Height;y++)for(int x=0;x<imagen.Width;x++)if(imagen.GetPixel(x,y).ToArgb()==Visual.Tema.Primario.ToArgb())violetas++;
                                Assert.IsTrue(violetas>imagen.Width*imagen.Height*0.6,boton.Name+" debe verse violeta.");
                            }
                        }
                        // La misma medición nativa que usa la barrera, sobre una copia sin relleno cromático.
                        using(var copia=new Button{Text=boton.Text,Font=boton.Font,Padding=boton.Padding,Size=boton.Size,FlatStyle=FlatStyle.Flat,BackColor=Color.White,ForeColor=Color.Black,UseVisualStyleBackColor=false}) {
                            copia.FlatAppearance.BorderColor=Color.White;
                            var tinta=typeof(PruebasTextoBotones).GetMethod("Tinta",BindingFlags.Static|BindingFlags.NonPublic);
                            double actual=(double)tinta.Invoke(null,new object[]{copia});copia.Size=copia.GetPreferredSize(Size.Empty);
                            double referencia=(double)tinta.Invoke(null,new object[]{copia});
                            Assert.IsTrue(actual>=referencia*0.995,boton.Name+" corta su texto.");
                        }
                    }
                }
            });
        }

        // CP-206 — RF-16, RN-45: Visual no puede escribir auditoría suplantando al autor.
        [TestMethod("CP-206: Visual no tiene acceso al registro de auditoría")]
        public void CP206_VisualNoPuedeEscribirAuditoria()
        {
            var registrar = typeof(Datos.RepositorioAuditoria).GetMethod(
                "Registrar", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsFalse(typeof(Datos.RepositorioAuditoria).IsPublic, "El tipo del repositorio de auditoría no debe exportarse desde Datos.");
            Assert.IsNotNull(registrar, "El método de escritura de auditoría debe existir para la capa de negocio.");
            Assert.IsTrue(registrar.IsAssembly, "Registrar debe ser internal y no público.");
            Assert.IsFalse(registrar.IsPublic, "Registrar no debe formar parte de la API pública de Datos.");
            var consultar = typeof(Datos.RepositorioAuditoria).GetMethod(
                "Consultar", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(consultar);
            Assert.IsTrue(consultar.IsAssembly, "Consultar también debe permanecer dentro de Datos y Negocio.");

            foreach (string metodo in new[] { "Aprobar", "Rechazar" })
            {
                var revision = typeof(Datos.RepositorioSolicitudesOrganizador).GetMethod(
                    metodo, BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(revision, "La operación de revisión debe existir en la capa de datos.");
                Assert.IsTrue(revision.IsAssembly, metodo + " debe estar limitado al ensamblado de negocio.");
                Assert.IsFalse(revision.IsPublic, metodo + " no debe permitir que Visual falsifique al administrador.");
            }

            var amigos = typeof(Datos.RepositorioAuditoria).Assembly
                .GetCustomAttributes<InternalsVisibleToAttribute>()
                .Select(a => new AssemblyName(a.AssemblyName).Name)
                .OrderBy(n => n)
                .ToArray();
            CollectionAssert.AreEqual(new[] { "Pruebas", "Repositorio" }, amigos,
                "Solo Negocio y el ensamblado de pruebas deben recibir acceso a miembros internal de Datos.");
            CollectionAssert.AreEqual(
                new[] { "Datos.RequisitoEsquemaFaltante", "Datos.VerificadorEsquema" },
                typeof(Datos.VerificadorEsquema).Assembly.GetExportedTypes().Select(t => t.FullName).OrderBy(n => n).ToArray(),
                "La interfaz pública de Datos debe limitarse al verificador de esquema que consume frmInicio.");

            string raiz = AppDomain.CurrentDomain.BaseDirectory;
            while (raiz != null && !File.Exists(Path.Combine(raiz, "SistemaEntradas.sln")))
                raiz = Directory.GetParent(raiz)?.FullName;
            Assert.IsNotNull(raiz, "No se pudo localizar la raíz del repositorio para inspeccionar Visual.");

            string[] archivos = Directory.GetFiles(Path.Combine(raiz, "Visual"), "*.cs", SearchOption.AllDirectories);
            foreach (string archivo in archivos)
            {
                string fuente = File.ReadAllText(archivo);
                string rutaRelativa = archivo.Substring(raiz.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                Assert.IsFalse(fuente.Contains("RepositorioAuditoria"),
                    "Visual no debe conocer el repositorio de auditoría: " + rutaRelativa);
                Assert.IsFalse(fuente.Contains("Repositorio"),
                    "Visual no debe conocer repositorios de Datos: " + rutaRelativa);
                Assert.IsFalse(fuente.Contains("RepositorioSolicitudesOrganizador"),
                    "Visual no debe llamar directamente al repositorio de solicitudes: " + rutaRelativa);
                Assert.IsFalse(fuente.Contains("GestorAuditoria.Instance.Registrar"),
                    "Visual no debe registrar auditoría desde la interfaz: " + rutaRelativa);
                Assert.IsFalse(fuente.Contains("INSERT dbo.Auditoria"),
                    "Visual no debe contener escrituras directas de auditoría: " + rutaRelativa);
            }
        }

        private static System.Collections.Generic.IEnumerable<Control> Descendientes(Control raiz)
        {
            foreach(Control c in raiz.Controls){yield return c;foreach(var h in Descendientes(c))yield return h;}
        }
        // CP-158 — RF-19: no se usa GetHashCode aleatorio para las portadas.
        [TestMethod("CP-158: Portadas determinísticas se pintan sin archivos de imagen")]
        public void CP158_Portadas()
        {
            PruebasSeguridad.EnStaSinCambiarDatos(()=>{
                using(var a=new PortadaEvento{Size=new Size(272,144)})using(var b=new PortadaEvento{Size=new Size(272,144)})
                using(var ia=new Bitmap(272,144))using(var ib=new Bitmap(272,144)) {
                    a.Cargar("Evento Aura","Concierto");b.Cargar("Evento Aura","Concierto");a.DrawToBitmap(ia,a.ClientRectangle);b.DrawToBitmap(ib,b.ClientRectangle);
                    for(int y=0;y<144;y++)for(int x=0;x<272;x++)Assert.AreEqual(ia.GetPixel(x,y),ib.GetPixel(x,y));
                    Assert.AreNotEqual(ia.GetPixel(0,143),ia.GetPixel(271,0),"El fondo debe ser un degradé.");
                }
            });
        }
        // CP-159 — RF-05, RNF-02: comprobar que las vistas compuestas no serializan el tema.
        [DataTestMethod]
        [DataRow(typeof(TarjetaEvento),DisplayName="CP-159: TarjetaEvento no serializa estilo")]
        [DataRow(typeof(PortadaEvento),DisplayName="CP-159: PortadaEvento no serializa estilo")]
        [DataRow(typeof(FichaEvento),DisplayName="CP-159: FichaEvento no serializa estilo")]
        [DataRow(typeof(InicioCliente),DisplayName="CP-159: InicioCliente no serializa estilo")]
        [DataRow(typeof(PanelAdministrador),DisplayName="CP-159: PanelAdministrador no serializa estilo")]
        [DataRow(typeof(DatoPanel),DisplayName="CP-159: DatoPanel no serializa estilo")]
        [DataRow(typeof(EstadoVacio),DisplayName="CP-159: EstadoVacio no serializa estilo")]
        [DataRow(typeof(IndicadorFortaleza),DisplayName="CP-159: IndicadorFortaleza no serializa estilo")]
        public void CP159_Host(Type tipo)
        {
            PruebasSeguridad.EnStaSinCambiarDatos(()=>{
                using(var superficie=new DesignSurface(typeof(Form))) {
                    var host=(IDesignerHost)superficie.GetService(typeof(IDesignerHost));var control=(Control)host.CreateComponent(tipo,"vista");
                    ((Form)host.RootComponent).Controls.Add(control);Assert.IsNotNull(superficie.View);
                    foreach(string propiedad in new[]{"Font","ForeColor","BackColor"})Assert.IsFalse(TypeDescriptor.GetProperties(control)[propiedad].ShouldSerializeValue(control));
                    Assert.AreEqual(Size.Empty,control.MinimumSize);
                }
            });
        }
        // CP-176 — RF-15, RN-44: conservar campos de recuperacion legibles al escalar.
        [TestMethod("CP-176: La recuperación conserva espacios entre campos al escalar")]
        public void CP176_RecuperacionSinSolapamientos()
        {
            EnSta(() =>
            {
                using (var form = new Visual.frmCambiarContraseña(true))
                {
                    Mostrar(form);
                    var medidor = ControlDe<IndicadorFortaleza>(form, "fortalezaContrasena");
                    var nueva = ControlDe<TextBox>(form, "TxtPassword");
                    var confirmacion = ControlDe<TextBox>(form, "TxtConfirmpassword");
                    var check = ControlDe<CheckBox>(form, "checkbxShowPass");
                    var confirmar = ControlDe<Button>(form, "btnconfirmar");
                    Assert.IsFalse(medidor.Bounds.IntersectsWith(nueva.Bounds), "El medidor se superpone con la contraseña nueva: " + medidor.Bounds + " / " + nueva.Bounds);
                    Assert.IsFalse(medidor.Bounds.IntersectsWith(confirmacion.Bounds), "El medidor se superpone con la confirmación: " + medidor.Bounds + " / " + confirmacion.Bounds);
                    Assert.IsFalse(confirmacion.Bounds.IntersectsWith(check.Bounds), "La confirmación se superpone con el check.");
                    Assert.IsFalse(check.Bounds.IntersectsWith(confirmar.Bounds), "El check se superpone con el botón.");
                    Assert.IsTrue(confirmar.Bottom <= form.ClientSize.Height);
                }
            });
        }
        // CP-160 — RF-18/RF-19/RF-03: se navega desde la tarjeta y el mapa hasta el armado real.
        [TestMethod("CP-160: Tarjeta abre ficha y el sector elegido llega al paquete")]
        public void CP160_Flujo()
        {
            typeof(Visual.Web.PanelWeb).Assembly.GetType("Visual.Web.PanelWebMotor")
                .GetField("entorno", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
            EnSta(()=>{
                var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);Fake.EventoPublicado(1500m,5);Fake.ComoCliente(cliente);
                using(var lista=new Visual.frmEventosCliente{IDUsuario=cliente.Id}) {
                    Mostrar(lista);EsperarVistaWeb(ControlDe<Visual.Web.PanelWeb>(lista,"catalogoWeb"));var tarjeta=ControlDe<FlowLayoutPanel>(lista,"tarjetasEventos").Controls.OfType<TarjetaEvento>().Single(t=>t.AccessibleName==evento.Nombre);
                    ControlDe<Button>(tarjeta,"verEntradas").PerformClick();var ficha=ControlDe<FichaEvento>(lista,"fichaEvento");
                    Assert.AreEqual(evento.Nombre,ControlDe<Label>(ficha,"tituloFicha").Text);
                    var mapa=ControlDe<MapaSectores>(ficha,"mapaFicha");var zona=mapa.Zona(0);
                    typeof(MapaSectores).GetMethod("OnMouseClick",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(mapa,new object[]{new MouseEventArgs(MouseButtons.Left,1,zona.Left+5,zona.Top+5,0)});
                    ControlDe<Button>(ficha,"comprarFicha").PerformClick();
                    var compra=Application.OpenForms.OfType<Visual.FrmCompraCliente>().Single(f=>f.Visible);
                    Assert.AreEqual(evento.Id,compra.IDEvento);Assert.AreEqual(cliente.Id,compra.IDUsuario);
                    var sectoresCompra = ControlDe<DataGridView>(compra,"dgvSectores");
                    Assert.AreEqual(1,Convert.ToInt32(sectoresCompra.Rows[0].Cells["Cantidad"].Value));
                    var mapaCompra = ControlDe<MapaSectores>(compra, "mapaSectores");
                    Assert.AreEqual(mapaCompra.Bounds, sectoresCompra.Bounds, "La grilla de compatibilidad queda exactamente bajo las tarjetas, fuera de la vista.");
                    Assert.IsFalse(sectoresCompra.Enabled || sectoresCompra.TabStop, "El adaptador no debe mostrarse ni recibir foco.");
                    var zonaCantidad = (Rectangle)typeof(MapaSectores).GetMethod("ZonaCantidad", BindingFlags.NonPublic | BindingFlags.Instance)
                        .Invoke(mapaCompra, new object[] { mapaCompra.Zona(0) });
                    typeof(MapaSectores).GetMethod("OnMouseClick", BindingFlags.NonPublic | BindingFlags.Instance)
                        .Invoke(mapaCompra, new object[] { new MouseEventArgs(MouseButtons.Left, 1, zonaCantidad.Right - zonaCantidad.Height / 2, zonaCantidad.Top + zonaCantidad.Height / 2, 0) });
                    Assert.AreEqual(2, Convert.ToInt32(sectoresCompra.Rows[0].Cells["Cantidad"].Value), "La cantidad se modifica dentro de la tarjeta del sector.");
                    compra.Dispose();
                }
                using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id })
                {
                    Mostrar(menu);
                    ControlDe<Button>(menu, "btnEventos").PerformClick();
                    Assert.AreEqual("Eventos disponibles", menu.Controls.OfType<Label>().Single(c => c.Name == "label1").Text,
                        "El título del menú debe seguir la vista actual al navegar al catálogo.");
                    var listado = ControlDe<Visual.frmEventosCliente>(menu, "frmEventosCliente");
                    var hostCatalogo = ControlDe<HostFormularioCliente>(menu, "paginafrmEventosCliente");
                    Assert.IsTrue(listado.ClientSize.Width <= hostCatalogo.ClientSize.Width && listado.ClientSize.Height <= hostCatalogo.ClientSize.Height,
                        "El catálogo debe adaptarse al panel del menú y no conservar su tamaño mínimo de ventana suelta.");
                    var webCatalogo = ControlDe<Visual.Web.PanelWeb>(listado, "catalogoWeb");
                    EsperarVistaWeb(webCatalogo);
                    string catalogoCompleto = EjecutarVistaWeb(webCatalogo,
                        "(()=>{const cs=[...document.querySelectorAll('#catalogo .evento')];return cs.length>=2&&cs.every(c=>{const r=c.getBoundingClientRect(),b=c.querySelector('.accion').getBoundingClientRect();return r.left>=0&&r.right<=innerWidth&&b.left>=0&&b.right<=innerWidth})&&document.scrollingElement.scrollWidth<=innerWidth})()");
                    Assert.AreEqual("true", catalogoCompleto,
                        "Las tarjetas y sus acciones deben entrar en el ancho del catálogo web dentro de Mi cuenta.");
                    foreach (string nombreBoton in new[] { "btnVolver", "btnVista" })
                    {
                        var boton = ControlDe<Button>(listado, nombreBoton);
                        var bounds = listado.RectangleToClient(boton.Parent.RectangleToScreen(boton.Bounds));
                        Assert.IsTrue(listado.ClientRectangle.Contains(bounds), nombreBoton + " debe quedar dentro de la ventana Mi cuenta.");
                    }
                    EjecutarVistaWeb(webCatalogo,
                        "[...document.querySelectorAll('#catalogo .evento')].find(c=>c.dataset.evento==" + evento.Id + ").querySelector('.accion').click()");
                    Esperar(() => listado.Controls.Find("fichaEvento", true).Any(),
                        "El clic web debe abrir la ficha dentro de la navegación del cliente.");
                    var fichaCliente = ControlDe<FichaEvento>(listado, "fichaEvento");
                    var accionFicha = ControlDe<Button>(fichaCliente, "comprarFicha");
                    Assert.IsTrue(listado.ClientRectangle.Contains(listado.RectangleToClient(accionFicha.Parent.RectangleToScreen(accionFicha.Bounds))),
                        "Armar paquete debe quedar dentro de la ventana Mi cuenta.");
                    accionFicha.PerformClick();
                    Assert.AreEqual("Armar paquete", menu.Controls.OfType<Label>().Single(c => c.Name == "label1").Text,
                        "El título debe cambiar a la pantalla de compra.");
                    var hostCompra = ControlDe<HostFormularioCliente>(menu, "paginaFrmCompraCliente");
                    var compraMenu = (Visual.FrmCompraCliente)hostCompra.Pantalla;
                    var webCompra = ControlDe<Visual.Web.PanelWeb>(compraMenu, "resumenCompraWeb");
                    Assert.IsTrue(compraMenu.ClientSize.Width <= hostCompra.ClientSize.Width && compraMenu.ClientSize.Height <= hostCompra.ClientSize.Height,
                        "El armador debe usar el tamaño disponible en el panel, no el mínimo de una ventana suelta.");
                    EsperarVistaWeb(webCompra);
                    string paqueteCompleto = EjecutarVistaWeb(webCompra,
                        "(()=>{const a=document.querySelector('.area-web'),t=document.querySelector('.total').getBoundingClientRect(),b=document.querySelector('#continuar').getBoundingClientRect();return a.scrollWidth<=a.clientWidth+1&&t.right<=innerWidth&&b.right<=innerWidth&&b.bottom<=innerHeight})()");
                    Assert.AreEqual("true", paqueteCompleto,
                        "El total y Continuar al pago deben verse completos en el armador dentro de Mi cuenta.");
                }
                using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id })
                {
                    Mostrar(menu);
                    var recomendado = ControlDe<FlowLayoutPanel>(menu, "eventosInicioCliente").Controls
                        .OfType<TarjetaEvento>().Single(t => t.AccessibleName == evento.Nombre);
                    ControlDe<Button>(recomendado, "verEntradas").PerformClick();
                    var ficha = ControlDe<FichaEvento>(menu, "fichaEvento");
                    Assert.AreEqual(evento.Nombre, ControlDe<Label>(ficha, "tituloFicha").Text);
                }
            });
        }
        private static void EsperarVistaWeb(Visual.Web.PanelWeb panel)
        {
            var reloj = Stopwatch.StartNew();
            while (!panel.Preparado && reloj.Elapsed < TimeSpan.FromSeconds(20))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(panel.Preparado && panel.Visible, panel.Name + " debe cargar para comprobar su geometría.");
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

        private static string EjecutarVistaWeb(Visual.Web.PanelWeb panel, string script)
        {
            var tarea = panel.EjecutarScriptAsync(script);
            var reloj = Stopwatch.StartNew();
            while (!tarea.IsCompleted && reloj.Elapsed < TimeSpan.FromSeconds(10))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(tarea.IsCompleted, panel.Name + " debe responder a la comprobación de geometría.");
            return (tarea.GetAwaiter().GetResult() ?? "").Trim('"');
        }

        // CP-161 — RF-07/RF-22, RN-41: el inicio no permite consultar el resumen de otro cliente.
        [TestMethod("CP-161: Inicio de cliente rechaza datos de otra cuenta")]
        public void CP161_InicioAjeno()
        {
            EnSta(()=>{var a=Fake.Cliente(NivelFidelidad.Bronce);var b=Fake.Cliente(NivelFidelidad.Oro);Fake.ComoCliente(a);
                using(var inicio=new InicioCliente())Assert.AreEqual("RN-41",Assert.ThrowsException<ReglaNegocioException>(()=>inicio.Cargar(b.Id)).Regla);
            });
        }
        // CP-162 — RF-11/RF-22, RN-34: seis indicadores de cinco reportes reales; el cliente no obtiene el panel.
        [TestMethod("CP-162: Panel contiene seis indicadores y exige administrador")]
        public void CP162_Panel()
        {
            EnSta(()=>{var cliente=Fake.Cliente(NivelFidelidad.Bronce);Fake.ComoAdministrador();using(var panel=new PanelAdministrador()) {
                panel.Cargar();Assert.AreEqual(6,ControlDe<FlowLayoutPanel>(panel,"indicadoresPanel").Controls.Count);
                Fake.ComoCliente(cliente);Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>panel.Cargar()).Regla);
            }});
        }

        // CP-165 — RF-22, RN-34: impedir que un Cliente acceda a la gestión administrativa global.
        [TestMethod("CP-165: Un cliente no alcanza el alta de administradores ni su operación")]
        public void CP165_AltaAdministradorSoloAdmin()
        {
            EnSta(()=>{
                var cliente=Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoAdministrador();
                using(var menu=new Visual.frmMenuAdministrador()) {
                    Mostrar(menu);
                    var panel=ControlDe<PanelAdministrador>(menu,"panelAdministrador");
                    var boton=ControlDe<Button>(menu,"btnCrearAdministrador");
                    Assert.IsTrue(boton.Visible);
                    Fake.ComoCliente(cliente);
                    Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>panel.Cargar()).Regla);
                    Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>boton.PerformClick()).Regla);
                    Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>GestorUsuarios.Instance.CrearAdministrador(Fake.MailNuevo(),"ClaveSegura123","Admin","Prueba")).Regla);
                }
            });
        }

        // CP-166 — RF-22, RN-05: crear administradores solo desde el panel protegido.
        [TestMethod("CP-166: El panel administrador abre el alta protegida y crea otro administrador")]
        public void CP166_AltaAdministradorDesdePanel()
        {
            EnSta(()=>{
                Fake.ComoAdministrador();
                string mail=Fake.MailNuevo();
                using(var menu=new Visual.frmMenuAdministrador()) {
                    Mostrar(menu);
                    var botonAlta = ControlDe<Button>(menu,"btnCrearAdministrador");
                    Assert.IsTrue(botonAlta.Visible, "El alta debe estar visible en el panel del administrador.");
                    Assert.IsTrue(botonAlta.RectangleToScreen(botonAlta.ClientRectangle).IntersectsWith(menu.Bounds), "El botón de alta debe quedar dentro de la ventana.");
                    botonAlta.PerformClick();
                    var alta=System.Windows.Forms.Application.OpenForms.OfType<Visual.frmCrearUsuario>().Single(f=>f.Visible);
                    Assert.AreEqual("Crear administrador",alta.Text);
                    ControlDe<TextBox>(alta,"txtNombre").Text="Admin";
                    ControlDe<TextBox>(alta,"txtApellido").Text="Nuevo";
                    ControlDe<TextBox>(alta,"txtmail").Text=mail;
                    ControlDe<TextBox>(alta,"TxtPassword").Text="ClaveSegura123";
                    ControlDe<TextBox>(alta,"TxtConfirmpassword").Text="ClaveSegura123";
                    DialogosPrueba.Comprobar(()=>ControlDe<Button>(alta,"btnregistrarse").PerformClick(),
                        new DialogoEsperado{Titulo="Crear administrador",Texto="Usuario creado con éxito."});
                    Assert.AreEqual("Administrador",RepositorioUsuarios.Instance.ObtenerPorMail(mail).TipoUsuario);
                    Assert.IsTrue(menu.Visible);
                    alta.Dispose();
                }
            });
        }

        // CP-211 — RF-17/RF-22, RN-02: el usuario autenticado recupera el cambio con contraseña actual.
        [TestMethod("CP-211: El menú de Mi cuenta abre el cambio de contraseña con sesión")]
        public void CP211_AccesoCambiarContrasena()
        {
            typeof(Visual.Web.PanelWeb).Assembly.GetType("Visual.Web.PanelWebMotor")
                .GetField("entorno", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, null);
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, NombreUsuario = cliente.Nombre })
                {
                    Mostrar(menu);
                    ControlDe<Button>(menu, "btnCambiarContrasena").PerformClick();
                    var cambio = System.Windows.Forms.Application.OpenForms.OfType<Visual.frmCambiarContraseña>().Single(f => f.Visible);
                    Assert.IsFalse(cambio.Controls.Find("btnEnviarCodigo", true).Any(), "El menú autenticado debe abrir modo cambio, no recuperación.");
                    Assert.AreEqual(cliente.Mail, cambio.CorreoInicial);
                    Assert.AreEqual(menu, cambio.Retorno);
                    var host = ControlDe<HostFormularioCliente>(menu, "paginafrmCambiarContrase\u00f1a");
                    Assert.AreEqual(cambio, host.Pantalla, "El cambio debe quedar dentro del shell de Mi cuenta.");
                    Assert.AreEqual("Cambiar contrase\u00f1a", ControlDe<Label>(menu, "label1").Text);
                    var web = ControlDe<Visual.Web.PanelWeb>(cambio, "cambioContrasenaWeb");
                    EsperarVistaWeb(web);
                    Assert.AreEqual("true", EjecutarVistaWeb(web,
                        "(()=>document.querySelectorAll('.ojo-contrasena').length===3&&document.querySelector('#mail').readOnly&&document.querySelectorAll('h1').length===0)()"),
                        "El cambio web debe mostrar tres ojos, mail de solo lectura y el título del shell.");
                    menu.Close();
                }
            });
        }
    }
}
