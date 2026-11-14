using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual.Componentes;
using static Pruebas.InterfazGrupoA;
namespace Pruebas {
 [TestClass] public class PruebasExtrasVisuales {
  // CP-119 — RF-05/RNF-02: resumen original y total destacado coherentes; paso correcto.
  [DataTestMethod]
  [DataRow(0,DisplayName="CP-119: TOTAL destacado coincide con el resumen vacío")]
  [DataRow(2,DisplayName="CP-119: TOTAL destacado coincide con dos entradas")]
  public void CP119_Total(int cantidad) {
   EnSta(()=> {
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);Fake.ComoCliente(cliente);
    using(var form=new Visual.FrmCompraCliente{IDUsuario=cliente.Id,IDEvento=evento.Id}) {
     Mostrar(form);if(cantidad>0) {var grilla=ControlDe<DataGridView>(form,"dgvSectores");grilla.Rows[0].Cells["Cantidad"].Value=cantidad;}
     var resumen=ControlDe<Label>(form,"lblResumen");var total=ControlDe<EtiquetaTotal>(form,"lblTotalDestacado");
     string importe=Regex.Match(resumen.Text,@"(?m)^TOTAL\s+([^\r\n]+)").Groups[1].Value.Trim();
     Assert.IsTrue(importe.Length>0);Assert.AreEqual("TOTAL "+importe,total.Text);
     Assert.AreEqual(18f,total.Font.SizeInPoints);Assert.AreEqual(Visual.Tema.Primario,total.ForeColor);
     Assert.AreEqual(2,ControlDe<IndicadorPasos>(form,"indicadorPasos").PasoActual);
     var panel=ControlDe<Panel>(form,"pnlResumen");ControlDe<Panel>(form,"pnlContenidoCompra").ScrollControlIntoView(panel);panel.AutoScrollPosition=new Point(0,100000);Application.DoEvents();
     Rectangle lectura=total.RectangleToScreen(total.ClientRectangle);
     for(Control padre=total.Parent;padre!=null;padre=padre.Parent)
      Assert.IsTrue(padre.ClientRectangle.Contains(padre.RectangleToClient(lectura)),"El TOTAL debe poder leerse completo en "+padre.Name);
    }
    using(var listado=new Visual.frmEventosCliente{IDUsuario=cliente.Id}) {Mostrar(listado);Assert.AreEqual(1,ControlDe<IndicadorPasos>(listado,"indicadorPasos").PasoActual);}
   });
  }
  // CP-120 — RNF-02: los nuevos componentes respetan la barrera del host real.
  [DataTestMethod]
  [DataRow(typeof(IndicadorPasos),DisplayName="CP-120: IndicadorPasos no serializa estilo")]
  [DataRow(typeof(EtiquetaTotal),DisplayName="CP-120: EtiquetaTotal no serializa estilo")]
  public void CP120_Componentes(Type tipo) {
   PruebasSeguridad.EnStaSinCambiarDatos(()=> {
    using(var superficie=new DesignSurface(typeof(Form))) {
     var host=(IDesignerHost)superficie.GetService(typeof(IDesignerHost));var control=(Control)host.CreateComponent(tipo,"extra");
     ((Form)host.RootComponent).Controls.Add(control);Assert.IsNotNull(superficie.View);
     foreach(string propiedad in new[]{"Font","ForeColor","BackColor"}) Assert.IsFalse(TypeDescriptor.GetProperties(control)[propiedad].ShouldSerializeValue(control));
     Assert.AreEqual(Size.Empty,control.MinimumSize);
    }
   });
  }
 }
}
