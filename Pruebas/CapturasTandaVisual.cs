using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Entidades;
using Negocio;
namespace Pruebas {
 public static class CapturasTandaVisual {
  [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr h,IntPtr despues,int x,int y,int w,int alto,uint flags);
  public static void Generar(string salida) {
   Directory.CreateDirectory(salida);
   foreach(string nombre in new[]{"frmAgregarEvento","frmSuscripcion","frmMenuPropietario","frmEventosCliente","FrmCompraCliente","frmHistorialCliente"})
    InterfazGrupoA.EnSta(()=> {
     var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);
     Form form;
     if(nombre=="frmAgregarEvento") {Fake.ComoAdministrador();form=new Visual.Propietario.frmAgregarEvento{IDUsuario=Fake.Administrador().Id,IDlocal=evento.IdLocal};}
     else if(nombre=="frmMenuPropietario") {Fake.ComoAdministrador();form=new Visual.frmMenuPropietario{IDUsuario=Fake.Administrador().Id,NombreUsuario="Juan Cruz"};}
     else {
      Fake.ComoCliente(cliente);
      if(nombre=="frmSuscripcion")form=new Visual.frmSuscripcion{IDUsuario=cliente.Id};
      else if(nombre=="frmEventosCliente")form=new Visual.frmEventosCliente{IDUsuario=cliente.Id};
      else if(nombre=="frmHistorialCliente") {Fake.CompraConfirmada(cliente,evento,1,true);form=new Visual.frmHistorialCliente{IDUsuario=cliente.Id};}
      else form=new Visual.FrmCompraCliente{IDUsuario=cliente.Id,IDEvento=evento.Id};
     }
     using(form) {
      form.TopMost=true;form.StartPosition=FormStartPosition.Manual;form.Location=new Point(40,40);InterfazGrupoA.Mostrar(form);form.BringToFront();form.Activate();
      if(nombre=="FrmCompraCliente") {
       var grilla=InterfazGrupoA.ControlDe<DataGridView>(form,"dgvSectores");grilla.Rows[0].Cells["Cantidad"].Value=2;
       if(grilla.EditingControl is TextBox) ((TextBox)grilla.EditingControl).Text="2";grilla.EndEdit();
      }
      if(nombre=="frmHistorialCliente") InterfazGrupoA.ControlDe<DataGridView>(form,"dgvEntradas").Rows[0].Cells["Devolver"].Value=true;
      form.BringToFront();form.Activate();SetWindowPos(form.Handle,new IntPtr(-1),0,0,0,0,0x43);System.Threading.Thread.Sleep(250);form.Refresh();Application.DoEvents();
      using(var imagen=new Bitmap(form.Width,form.Height))using(var grafico=Graphics.FromImage(imagen)) {
       grafico.CopyFromScreen(form.Location,Point.Empty,form.Size);imagen.Save(Path.Combine(salida,nombre+"-escritorio.png"));
      }
      using(var imagen=new Bitmap(form.Width,form.Height)) {form.DrawToBitmap(imagen,new Rectangle(Point.Empty,form.Size));imagen.Save(Path.Combine(salida,nombre+".png"));}
      if(nombre=="FrmCompraCliente") {
       var panel=InterfazGrupoA.ControlDe<Panel>(form,"pnlResumen");InterfazGrupoA.ControlDe<Panel>(form,"pnlContenidoCompra").ScrollControlIntoView(panel);panel.AutoScrollPosition=new Point(0,100000);Application.DoEvents();
       using(var extra=new Bitmap(form.Width,form.Height)) {form.DrawToBitmap(extra,new Rectangle(Point.Empty,form.Size));extra.Save(Path.Combine(salida,nombre+"-total.png"));}
      }
      using(var g=form.CreateGraphics())File.AppendAllText(Path.Combine(salida,"ventanas.txt"),nombre+" DPI="+g.DpiX+" cliente="+form.ClientSize+" ventana="+form.Size+"\r\n");
     }
    });
  }
 }
}
