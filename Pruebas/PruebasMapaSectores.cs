using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual.Componentes;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasMapaSectores
    {
        // CP-150 — RF-18: el mapa usa los precios reales y no permite elegir cupo agotado.
        [TestMethod("CP-150: Mapa ordena por precio y rechaza un sector agotado")]
        public void CP150_OrdenYCupo()
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() => {
                using (var mapa = new MapaSectores { Size = new Size(400, 260) })
                {
                    mapa.Cargar(new[] { new Sector { Id=1, Nombre="Campo", Precio=100, Capacidad=10 },
                        new Sector { Id=2, Nombre="VIP", Precio=200, Capacidad=1 } }, id => id == 2 ? 0 : 10);
                    int elegido = 0; mapa.SectorElegido += id => elegido = id;
                    var click = typeof(MapaSectores).GetMethod("OnMouseClick", BindingFlags.Instance | BindingFlags.NonPublic);
                    for (int i=0;i<2;i++) {
                        var zona=mapa.Zona(i);
                        click.Invoke(mapa,new object[]{new MouseEventArgs(MouseButtons.Left,1,zona.Left+5,zona.Top+5,0)});
                        Assert.AreEqual(i==0?0:1,elegido,"VIP agotado va primero; Campo disponible se puede elegir.");
                    }
                }
            });
        }
        // CP-151 — RF-18/RF-04, RN-08: seleccionar una zona utiliza el armado existente.
        [TestMethod("CP-151: Clic en mapa arma una entrada por el mismo gestor")]
        public void CP151_Compra()
        {
            EnSta(() => {
                var cliente=Fake.Cliente(NivelFidelidad.Bronce); var evento=Fake.EventoPublicado(1000m,5);
                Fake.ComoCliente(cliente);
                using(var f=new Visual.FrmCompraCliente{IDUsuario=cliente.Id,IDEvento=evento.Id}) {
                    Mostrar(f);var mapa=ControlDe<MapaSectores>(f,"mapaSectores");var zona=mapa.Zona(0);
                    typeof(MapaSectores).GetMethod("OnMouseClick",BindingFlags.Instance|BindingFlags.NonPublic)
                        .Invoke(mapa,new object[]{new MouseEventArgs(MouseButtons.Left,1,zona.Left+5,zona.Top+5,0)});
                    Assert.AreEqual(1,Convert.ToInt32(ControlDe<DataGridView>(f,"dgvSectores").Rows[0].Cells["Cantidad"].Value));
                    StringAssert.Contains(ControlDe<Label>(f,"lblResumen").Text,"1 x Campo");
                }
            });
        }
    }
}
