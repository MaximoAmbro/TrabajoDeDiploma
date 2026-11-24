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
     Assert.AreEqual(Visual.Tema.Cuerpo, resumen.Font);
     using (Graphics lienzo = resumen.CreateGraphics())
      Assert.AreEqual(1, "0123456789".Select(digito => TextRenderer.MeasureText(lienzo, digito.ToString(),
       resumen.Font, Size.Empty, TextFormatFlags.NoPadding).Width).Distinct().Count(), "Los importes requieren dígitos tabulares.");
     string importe=Regex.Match(resumen.Text,@"(?m)^TOTAL\s+([^\r\n]+)").Groups[1].Value.Trim();
     Assert.IsTrue(importe.Length>0);Assert.AreEqual("TOTAL "+importe,total.Text);
     Assert.AreEqual(15f,total.Font.SizeInPoints);Assert.AreEqual(Visual.Tema.Texto,total.ForeColor);
     Assert.AreEqual(2,ControlDe<IndicadorPasos>(form,"indicadorPasos").PasoActual);
     Rectangle lectura=total.RectangleToScreen(total.ClientRectangle);
     for(Control padre=total.Parent;padre!=null;padre=padre.Parent)
      Assert.IsTrue(padre.ClientRectangle.Contains(padre.RectangleToClient(lectura)),"El TOTAL debe poder leerse completo en "+padre.Name);
    }
    var pendiente=GestorCompras.Instance.Iniciar(Fake.Paquete(evento,1,false),cliente);
    using(var pago=new Visual.frmPago{Compra=pendiente}) {Mostrar(pago);Assert.AreEqual(3,ControlDe<IndicadorPasos>(pago,"indicadorPasos").PasoActual);}
    var confirmada=Fake.CompraConfirmada(cliente,evento,Math.Max(1,cantidad),false);
    using(var confirmacion=new Visual.frmConfirmacion{Compra=confirmada}) {
     confirmacion.Rutas.AddRange(new[]{@"C:\privado\Ticket_1.pdf", @"C:\privado\Ticket_2.pdf"});
     confirmacion.Carpeta=@"C:\privado";
     Mostrar(confirmacion);Assert.AreEqual(4,ControlDe<IndicadorPasos>(confirmacion,"indicadorPasos").PasoActual);
     Assert.AreEqual("TOTAL "+confirmada.TotalPagado.ToString("C2",new System.Globalization.CultureInfo("es-AR")),ControlDe<EtiquetaTotal>(confirmacion,"lblTotalConfirmado").Text);
     var visibles=new[]{ControlDe<Label>(confirmacion,"lblDesglose").Text,ControlDe<TextBox>(confirmacion,"txtCarpeta").Text}
      .Concat(ControlDe<ListBox>(confirmacion,"lstArchivos").Items.Cast<object>().Select(x=>Convert.ToString(x))).ToArray();
     Assert.IsFalse(visibles.Any(t=>t.Contains("tok_sim_")||t.Contains("Token de pago:")||t.Contains(@"C:\privado")||t.Contains("Ticket_1.pdf")));
     Assert.AreEqual("Entrada 1",ControlDe<ListBox>(confirmacion,"lstArchivos").Items[0]);
     Assert.AreEqual("Guardadas en Documentos.",ControlDe<TextBox>(confirmacion,"txtCarpeta").Text);
    }
    using(var listado=new Visual.frmEventosCliente{IDUsuario=cliente.Id}) {Mostrar(listado);Assert.AreEqual(1,ControlDe<IndicadorPasos>(listado,"indicadorPasos").PasoActual);}
   });
  }
        // CP-120 — RF-05, RNF-02: los controles reutilizables aplican estilo en ejecucion sin serializarlo.
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
     if(tipo==typeof(IndicadorPasos)) {
      var exito=(Control)host.CreateComponent(typeof(IconoExito),"exito");((Form)host.RootComponent).Controls.Add(exito);
      foreach(string p in new[]{"Font","ForeColor","BackColor"}) Assert.IsFalse(TypeDescriptor.GetProperties(exito)[p].ShouldSerializeValue(exito));
      Assert.AreEqual("Segoe MDL2 Assets",Visual.Tema.IconosGrandes.Name);
     }
    }
   });
  }
 }
}
