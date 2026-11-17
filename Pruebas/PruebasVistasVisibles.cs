using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        // CP-163 — RNF-02/RNF-03: los botones de las nuevas vistas también se renderizan completos.
        [DataTestMethod]
        [DataRow(typeof(TarjetaEvento),DisplayName="CP-163: Botones de tarjeta completos y primario renderizado")]
        [DataRow(typeof(FichaEvento),DisplayName="CP-163: Botones de ficha completos y primario renderizado")]
        [DataRow(typeof(InicioCliente),DisplayName="CP-163: Botones de inicio completos y primario renderizado")]
        [DataRow(typeof(EstadoVacio),DisplayName="CP-163: Acción de estado vacío con texto completo")]
        [DataRow(typeof(Visual.frmAuditoria),DisplayName="CP-163: Pantalla de auditoria con accion visible")]
        public void CP163_Botones(Type tipo)
        {
            PruebasSeguridad.EnStaSinCambiarDatos(()=>{
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
        private static System.Collections.Generic.IEnumerable<Control> Descendientes(Control raiz)
        {
            foreach(Control c in raiz.Controls){yield return c;foreach(var h in Descendientes(c))yield return h;}
        }
        // CP-158 — RF-03: no se usa GetHashCode aleatorio para las portadas.
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
        // CP-159 — RNF-02: los nuevos componentes conservan el contrato del Designer.
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
        // CP-160 — RF-03/RF-05: se navega desde datos reales de tarjeta hasta el armado real.
        [TestMethod("CP-160: Tarjeta abre ficha y el sector elegido llega al paquete")]
        public void CP160_Flujo()
        {
            EnSta(()=>{
                var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);Fake.ComoCliente(cliente);
                using(var lista=new Visual.frmEventosCliente{IDUsuario=cliente.Id}) {
                    Mostrar(lista);var tarjeta=ControlDe<FlowLayoutPanel>(lista,"tarjetasEventos").Controls.OfType<TarjetaEvento>().Single(t=>t.AccessibleName==evento.Nombre);
                    ControlDe<Button>(tarjeta,"verEntradas").PerformClick();var ficha=ControlDe<FichaEvento>(lista,"fichaEvento");
                    Assert.AreEqual(evento.Nombre,ControlDe<Label>(ficha,"tituloFicha").Text);
                    var mapa=ControlDe<MapaSectores>(ficha,"mapaFicha");var zona=mapa.Zona(0);
                    typeof(MapaSectores).GetMethod("OnMouseClick",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(mapa,new object[]{new MouseEventArgs(MouseButtons.Left,1,zona.Left+5,zona.Top+5,0)});
                    ControlDe<Button>(ficha,"comprarFicha").PerformClick();
                    var compra=Application.OpenForms.OfType<Visual.FrmCompraCliente>().Single(f=>f.Visible);
                    Assert.AreEqual(evento.Id,compra.IDEvento);Assert.AreEqual(cliente.Id,compra.IDUsuario);
                    Assert.AreEqual(1,Convert.ToInt32(ControlDe<DataGridView>(compra,"dgvSectores").Rows[0].Cells["Cantidad"].Value));
                }
            });
        }
        // CP-161 — RF-07/RN-41: el inicio no permite consultar el resumen de otro cliente.
        [TestMethod("CP-161: Inicio de cliente rechaza datos de otra cuenta")]
        public void CP161_InicioAjeno()
        {
            EnSta(()=>{var a=Fake.Cliente(NivelFidelidad.Bronce);var b=Fake.Cliente(NivelFidelidad.Oro);Fake.ComoCliente(a);
                using(var inicio=new InicioCliente())Assert.AreEqual("RN-41",Assert.ThrowsException<ReglaNegocioException>(()=>inicio.Cargar(b.Id)).Regla);
            });
        }
        // CP-162 — RF-11/RN-34: cinco reportes reales; el cliente no obtiene el panel.
        [TestMethod("CP-162: Panel contiene cinco indicadores y exige administrador")]
        public void CP162_Panel()
        {
            EnSta(()=>{var cliente=Fake.Cliente(NivelFidelidad.Bronce);Fake.ComoAdministrador();using(var panel=new PanelAdministrador()) {
                panel.Cargar();Assert.AreEqual(5,ControlDe<FlowLayoutPanel>(panel,"indicadoresPanel").Controls.Count);
                Fake.ComoCliente(cliente);Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>panel.Cargar()).Regla);
            }});
        }

        // CP-165 — RN-34: el alta de administradores aparece en administración y conserva su guarda.
        [TestMethod("CP-165: Un cliente no alcanza el alta de administradores ni su operación")]
        public void CP165_AltaAdministradorSoloAdmin()
        {
            EnSta(()=>{
                var cliente=Fake.Cliente(NivelFidelidad.Bronce);
                using(var panel=new PanelAdministrador()) {
                    Assert.IsNotNull(ControlDe<Button>(panel,"btnCrearAdministrador"));
                    Fake.ComoCliente(cliente);
                    Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>panel.Cargar()).Regla);
                    Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>ControlDe<Button>(panel,"btnCrearAdministrador").PerformClick()).Regla);
                    Assert.AreEqual("RN-34",Assert.ThrowsException<ReglaNegocioException>(()=>GestorUsuarios.Instance.CrearAdministrador(Fake.MailNuevo(),"ClaveSegura123","Admin","Prueba")).Regla);
                }
            });
        }

        [TestMethod("CP-166: El panel administrador abre el alta protegida y crea otro administrador")]
        public void CP166_AltaAdministradorDesdePanel()
        {
            EnSta(()=>{
                Fake.ComoAdministrador();
                string mail=Fake.MailNuevo();
                using(var menu=new Visual.frmMenuPropietario()) {
                    Mostrar(menu);
                    ControlDe<Button>(menu,"btnCrearAdministrador").PerformClick();
                    var alta=System.Windows.Forms.Application.OpenForms.OfType<Visual.frmCrearUsuario>().Single(f=>f.Visible);
                    Assert.AreEqual("EventAura · Crear administrador",alta.Text);
                    ControlDe<TextBox>(alta,"txtNombre").Text="Admin";
                    ControlDe<TextBox>(alta,"txtApellido").Text="Nuevo";
                    ControlDe<TextBox>(alta,"txtmail").Text=mail;
                    ControlDe<TextBox>(alta,"TxtPassword").Text="ClaveSegura123";
                    ControlDe<TextBox>(alta,"TxtConfirmpassword").Text="ClaveSegura123";
                    DialogosPrueba.Comprobar(()=>ControlDe<Button>(alta,"btnregistrarse").PerformClick(),
                        new DialogoEsperado{Titulo="Crear administrador",Texto="Usuario creado con éxito."});
                    Assert.AreEqual("Propietario",RepositorioUsuarios.Instance.ObtenerPorMail(mail).TipoUsuario);
                    Assert.IsTrue(menu.Visible);
                    alta.Dispose();
                }
            });
        }
    }
}
