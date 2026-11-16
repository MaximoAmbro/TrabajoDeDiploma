using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Datos;
using Entidades;
using Negocio;
namespace Pruebas {
 // Figuras del ejemplo del instructivo. No son casos de prueba nuevos.
 public static class CapturasInstructivoCongelado {
  private static T C<T>(Control f,string n) where T:Control{return (T)f.Controls.Find(n,true).Single();}
  public static void Generar(string salida){
   Directory.CreateDirectory(salida);BaseDePrueba.Preparar(null);
   PruebasSeguridad.EnStaSinCambiarDatos(()=>{
    GestorUsuarios.Instance.Autenticar("plata@eventaura.com","Cliente123");
    var cliente=GestorFidelizacion.Instance.ObtenerCliente(3);var evento=RepositorioEventos.Instance.ObtenerPorId(1);
    using(var listado=new Visual.frmEventosCliente{IDUsuario=cliente.Id,NombreUsuario=cliente.Nombre})
    using(var form=new Visual.FrmCompraCliente{IDUsuario=cliente.Id,IDEvento=evento.Id,NombreUsuario=cliente.Nombre}) {
     listado.Show();Application.DoEvents();var grilla=C<DataGridView>(listado,"dgveventos");
     foreach(DataGridViewRow fila in grilla.Rows)if(Convert.ToInt32(fila.Cells["ID"].Value)==evento.Id)grilla.CurrentCell=fila.Cells["Nombre"];
     Guardar(listado,Path.Combine(salida,"figura1-listado-eventos.png"));
     form.Show();Application.DoEvents();var sectores=C<DataGridView>(form,"dgvSectores");
     var campo=sectores.Rows.Cast<DataGridViewRow>().Single(f=>Convert.ToString(f.Cells["Sector"].Value)=="Campo");
     sectores.CurrentCell=campo.Cells["Cantidad"];campo.Cells["Cantidad"].Value=2;
     if(sectores.EditingControl is TextBox)((TextBox)sectores.EditingControl).Text="2";sectores.EndEdit();
     C<CheckBox>(form,"chkHospedaje").Checked=true;Application.DoEvents();
     var flags=BindingFlags.Instance|BindingFlags.NonPublic;var paquete=(Paquete)form.GetType().GetField("paquete",flags).GetValue(form);
     var resumen=GestorFidelizacion.Instance.CalcularDescuento(cliente,paquete);
     if(resumen.Total!=99625m||resumen.Porcentaje!=0.05m)throw new InvalidOperationException("El ejemplo del instructivo no coincide con $99.625 y Plata al 5 %.");
     Guardar(form,Path.Combine(salida,"figura2-armado-paquete.png"));
     var panel=C<Panel>(form,"pnlResumen");var original=panel.Size;
     panel.AutoScrollPosition=Point.Empty;panel.AutoScroll=false;
     panel.Height=panel.Controls.Cast<Control>().Max(x=>x.Bottom)+16;
     using(var b=new Bitmap(panel.Width,panel.Height)){panel.DrawToBitmap(b,new Rectangle(Point.Empty,panel.Size));b.Save(Path.Combine(salida,"figura3-resumen-descuento.png"));}
     panel.Size=original;panel.AutoScroll=true;
     var compra=GestorCompras.Instance.Iniciar(paquete,cliente);if(!GestorCompras.Instance.Confirmar(compra,Fake.Tarjeta()))throw new InvalidOperationException("Rechazo inesperado de la pasarela simulada.");
     compra=GestorCompras.Instance.ObtenerPorId(compra.Id);
     var entradas=GestorCompras.Instance.EntradasDe(compra);var rutas=new List<string>();var pesos=CultureInfo.GetCultureInfo("es-AR");
     foreach(var entrada in entradas){var sector=RepositorioSectores.Instance.ObtenerPorId(entrada.IdSector.Value);string ruta=Path.Combine(salida,"Ticket_"+Visual.frmConfirmacion.NumeroPedido(compra)+"_"+entrada.Id+".pdf");
      string texto="ENTRADA "+evento.Nombre+"\nSECTOR: "+sector.Nombre+"\nPRECIO: "+entrada.Precio.ToString("C2",pesos)+"\nFECHA: "+evento.Fecha.ToString("dd/MM/yyyy HH:mm")+"\nPEDIDO: "+Visual.frmConfirmacion.NumeroPedido(compra)+"\nCÓDIGO: "+entrada.CodigoQR+"\n";
      form.GenerarPDF(ruta,texto,GeneradorQR.Instance.Imagen(entrada.CodigoQR));rutas.Add(ruta);
     }
     using(var conf=new Visual.frmConfirmacion{Compra=compra,Rutas=rutas,Carpeta=@"C:\Users\Cliente\Documents\A1.Tickets EVENTAURA"}){
      conf.Show();Application.DoEvents();Guardar(conf,Path.Combine(salida,"figura4b-confirmacion.png"));
     }
     File.WriteAllText(Path.Combine(salida,"ejemplo.txt"),"Paula Plata; dos Campo de $25.000; hospedaje $45.000; descuento $2.500; cargo $7.125; total $99.625; sin seguro.\r\nPedido "+Visual.frmConfirmacion.NumeroPedido(compra)+"\r\nPDF "+rutas[0]+"\r\nDesglose:\r\n"+C<Label>(form,"lblResumen").Text);
    }
   });
   GestorSesion.Instance.Cerrar();
  }
  private static void Guardar(Form f,string ruta){f.Refresh();Application.DoEvents();using(var b=new Bitmap(f.Width,f.Height)){CapturasTandaVisual.DibujarCompleta(f,b);b.Save(ruta);}}
 }
}
