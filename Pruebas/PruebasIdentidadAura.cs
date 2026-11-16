using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual.Componentes;

namespace Pruebas
{
    [TestClass]
    public class PruebasIdentidadAura
    {
        // CP-154 — RNF-02: la sesión puede sobrevivir al borrado de un usuario de prueba.
        [TestMethod("CP-154: Encabezado sin usuario persistido no bloquea el formulario")]
        public void CP154_SesionSinUsuario()
        {
            InterfazGrupoA.EnSta(() => {
                var cliente=Fake.Cliente(Entidades.NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);Fake.Limpiar();
                using(var barra=new BarraSuperior()) {
                    barra.ActualizarSesion();
                    Assert.IsTrue(string.IsNullOrEmpty(barra.AccessibleDescription));
                }
            });
        }
        // CP-155 — RNF-02: abrir la aplicación sin autenticarse es un estado normal.
        [TestMethod("CP-155: Barra sin sesión muestra solo la marca sin consultar un usuario")]
        public void CP155_SinSesion()
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() => {
                Negocio.GestorSesion.Instance.Cerrar();
                using(var barra=new BarraSuperior()) {
                    barra.ActualizarSesion();
                    Assert.IsTrue(string.IsNullOrEmpty(barra.AccessibleDescription));
                    using(var imagen=new System.Drawing.Bitmap(barra.Width,barra.Height))
                        barra.DrawToBitmap(imagen,barra.ClientRectangle);
                }
            });
        }
        // CP-152 — RNF-02: la barra conserva el contrato del host de diseño real.
        [TestMethod("CP-152: Barra de identidad no serializa tipografía ni colores")]
        public void CP152_Host()
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() => {
                using (var superficie = new DesignSurface(typeof(Form))) {
                    var host=(IDesignerHost)superficie.GetService(typeof(IDesignerHost));
                    var barra=(BarraSuperior)host.CreateComponent(typeof(BarraSuperior),"marca");
                    ((Form)host.RootComponent).Controls.Add(barra);
                    Assert.IsNotNull(superficie.View);
                    foreach(var propiedad in new[]{"Font","BackColor","ForeColor"})
                        Assert.IsFalse(TypeDescriptor.GetProperties(barra)[propiedad].ShouldSerializeValue(barra),propiedad);
                    Assert.AreEqual(System.Drawing.Size.Empty,barra.MinimumSize);
                    Assert.AreEqual(60,barra.Height);
                }
            });
        }
        // CP-153 — RNF-01/RNF-02: el recurso entregado contiene las cinco resoluciones pedidas.
        [TestMethod("CP-153: Icono Aura incluye 16, 32, 48, 64 y 256 píxeles")]
        public void CP153_Icono()
        {
            using(var lector=new BinaryReader(File.OpenRead(Path.Combine(BaseDePrueba.RaizDelRepositorio(),"Visual","Recursos","EventAura.ico")))) {
                Assert.AreEqual(0,lector.ReadUInt16());Assert.AreEqual(1,lector.ReadUInt16());
                Assert.AreEqual(5,lector.ReadUInt16());
                foreach(int lado in new[]{16,32,48,64,256}) {
                    Assert.AreEqual(lado==256?0:lado,(int)lector.ReadByte());
                    Assert.AreEqual(lado==256?0:lado,(int)lector.ReadByte());
                    lector.ReadBytes(6); uint largo=lector.ReadUInt32(), offset=lector.ReadUInt32();
                    Assert.IsTrue(largo>0 && offset+largo<=lector.BaseStream.Length);
                }
            }
        }
    }
}
