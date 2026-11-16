using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Pruebas {
 public static class CapturasUltimasUI {
  private static T C<T>(Control f,string n) where T:Control{return (T)f.Controls.Find(n,true).Single();}
  public static void RetornoCompra(string salida){
   Directory.CreateDirectory(salida);
   InterfazGrupoA.EnSta(()=>{
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);Fake.ComoCliente(cliente);
    string ticket=null;byte[] anterior=null;bool existia=false;bool protegido=false;
    using(var form=new Visual.FrmCompraCliente{IDUsuario=cliente.Id,IDEvento=evento.Id,NombreUsuario="Cliente"})
    using(var reloj=new Timer{Interval=50}){
     int etapa=0;Exception error=null;
     reloj.Tick+=(s,e)=>{
      try{
       var pago=Application.OpenForms.OfType<Visual.frmPago>().FirstOrDefault();
       if(etapa==0&&pago!=null&&pago.Visible){
        etapa=1;
        // Solo el nombre exacto que esta compra nueva va a generar. Conserva cualquier archivo previo.
        string carpeta=Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"A1.Tickets EVENTAURA"));
        ticket=Path.GetFullPath(Path.Combine(carpeta,"Ticket_"+Visual.frmConfirmacion.NumeroPedido(pago.Compra)+"_Campo_1.PDF"));
        if(Path.GetDirectoryName(ticket)!=carpeta)throw new InvalidOperationException("Ticket fuera de la carpeta esperada.");
        existia=File.Exists(ticket);if(existia)anterior=File.ReadAllBytes(ticket);protegido=true;
        var tarjeta=Fake.Tarjeta();C<TextBox>(pago,"txtTitular").Text=tarjeta.Titular;C<TextBox>(pago,"txtNumero").Text=tarjeta.Numero;C<TextBox>(pago,"txtVencimiento").Text=tarjeta.Vencimiento;C<TextBox>(pago,"txtCvv").Text=tarjeta.Cvv;
        C<Button>(pago,"btnPagar").PerformClick();return;
       }
       var conf=Application.OpenForms.OfType<Visual.frmConfirmacion>().FirstOrDefault();
       if(etapa==1&&conf!=null&&conf.Visible){etapa=2;Guardar(conf,Path.Combine(salida,"u09-confirmacion.png"));conf.Close();}
      }catch(Exception ex){error=ex;reloj.Stop();foreach(Form modal in Application.OpenForms.Cast<Form>().Where(f=>f!=form).ToArray())modal.Dispose();}
     };
     try{
      form.Show();Application.DoEvents();var grid=C<DataGridView>(form,"dgvSectores");var fila=grid.Rows.Cast<DataGridViewRow>().Single(f=>Convert.ToString(f.Cells["Sector"].Value)=="Campo");
      grid.CurrentCell=fila.Cells["Cantidad"];fila.Cells["Cantidad"].Value=1;if(grid.EditingControl is TextBox)((TextBox)grid.EditingControl).Text="1";grid.EndEdit();C<CheckBox>(form,"chkSeguro").Checked=true;
      reloj.Start();C<Button>(form,"btnComprar").PerformClick();reloj.Stop();
      if(error!=null)throw error;
      Assert.AreEqual(2,etapa,"Debe completar pago y cerrar la confirmación real.");Assert.IsFalse(form.Visible,"No debe volver a mostrar el armado.");
      var listado=Application.OpenForms.OfType<Visual.frmEventosCliente>().Single(f=>f.Visible);
      Assert.AreEqual(cliente.Id,listado.IDUsuario);Assert.AreEqual(cliente.Id,GestorSesion.Instance.UsuarioActual.Id);
      Guardar(listado,Path.Combine(salida,"u09-retorno-listado.png"));
      File.WriteAllText(Path.Combine(salida,"u09.txt"),"Confirmación cerrada; listado visible; armado oculto; identidad conservada.\r\n");
     }finally{
      reloj.Stop();if(protegido){if(existia)File.WriteAllBytes(ticket,anterior);else if(File.Exists(ticket))File.Delete(ticket);}
     }
    }
   });
  }
  public static void Guion(string salida){
   Directory.CreateDirectory(salida);File.WriteAllText(Path.Combine(salida,"tipografia.tsv"),"figura\tancho_px\tdpi\tmin_fuente_pt\timpresa_16cm_pt\r\n");
   foreach(string nombre in new[]{"frmInicio","frmCrearUsuario","frmMenuCliente"}) InterfazGrupoA.EnSta(()=>{
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);Fake.ComoCliente(cliente);
    Form f=nombre=="frmInicio"?(Form)new Visual.frmInicio():nombre=="frmCrearUsuario"?(Form)new Visual.frmCrearUsuario():new Visual.frmMenuCliente();
    var flags=System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance;
    var id=f.GetType().GetField("IDUsuario",flags);if(id!=null)id.SetValue(f,cliente.Id);
    var persona=f.GetType().GetProperty("NombreUsuario",flags);if(persona!=null&&persona.CanWrite)persona.SetValue(f,"Cliente",null);
    var nombreCampo=f.GetType().GetField("NombreUsuario",flags);if(nombreCampo!=null)nombreCampo.SetValue(f,"Cliente");
    using(f){f.Show();Application.DoEvents();string archivo=nombre=="frmInicio"?"p47-inicio.png":nombre=="frmCrearUsuario"?"p48-registro.png":"p49-menu-cliente.png";
     float dpi;using(var g=f.CreateGraphics())dpi=g.DpiX;
     var fuentes=Descendientes(f).Where(c=>c.Visible&&!string.IsNullOrWhiteSpace(c.Text)&&!(c is Form)&&!(c is Panel)).Select(c=>c.Font.SizeInPoints).ToArray();
     float minima=fuentes.Min();double impresa=minima*dpi/72.0*(16.0/2.54*72.0)/f.Width;
     File.AppendAllText(Path.Combine(salida,"tipografia.tsv"),archivo+"\t"+f.Width+"\t"+dpi+"\t"+minima+"\t"+impresa.ToString("F2",System.Globalization.CultureInfo.InvariantCulture)+"\r\n");
     if(impresa<7.0)throw new InvalidOperationException("Figura no entregable a 16 cm: "+archivo+" queda en "+impresa+" puntos.");
     Guardar(f,Path.Combine(salida,archivo));
    }
   });
  }
  private static System.Collections.Generic.IEnumerable<Control> Descendientes(Control p){foreach(Control c in p.Controls){yield return c;foreach(var x in Descendientes(c))yield return x;}}
  private static void Guardar(Form f,string ruta){f.Refresh();Application.DoEvents();using(var b=new Bitmap(f.Width,f.Height)){CapturasTandaVisual.DibujarCompleta(f,b);b.Save(ruta);}}
 }
 [TestClass] public class PruebasFinCompra {
  // CP-148 — RF-06, U-09: el flujo completo vuelve al listado al cerrar la confirmación.
  [TestMethod("CP-148: Cerrar la confirmación vuelve al listado sin mostrar un armado vacío")]
  public void CP148_Retorno(){CapturasUltimasUI.RetornoCompra(Path.Combine(BaseDePrueba.RaizDelRepositorio(),"TestResults","Ultimo16","u09-prueba"));}
 }
}
