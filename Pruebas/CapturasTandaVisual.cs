using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Entidades;
using Negocio;
namespace Pruebas {
 public static class CapturasTandaVisual {
  [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr h,IntPtr despues,int x,int y,int w,int alto,uint flags);
  public static void Generar(string salida) {
   Directory.CreateDirectory(salida);
   foreach(string nombre in typeof(Visual.frmInicio).Assembly.GetTypes().Where(t=>typeof(Form).IsAssignableFrom(t) && !t.IsAbstract && t.Name!="FormularioEventAura").Select(t=>t.Name).OrderBy(n=>n))
    InterfazGrupoA.EnSta(()=> {
     var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);
     var administrativos=new[]{"frmAgregarEvento","frmAgregarSector","frmRegistrarLocal","frmAdministrarEvento","frmEventosLocal","frmMisLocales","frmMenuPropietario","frmListasEspera","frmReportes","frmControlIngreso"};
     bool administrador=administrativos.Contains(nombre);
     Compra compra=null;
     if(nombre=="frmConfirmacion" || nombre=="frmHistorialCliente") compra=Fake.CompraConfirmada(cliente,evento,2,true);
     if(nombre=="frmPago") {Fake.ComoCliente(cliente);compra=GestorCompras.Instance.Iniciar(Fake.Paquete(evento,1,false),cliente);}
     if(nombre=="frmControlIngreso") Fake.CambiarFechaDeEvento(evento.Id,DateTime.Now.AddMinutes(10));
     if(nombre=="frmListasEspera") {
      Fake.CompraConfirmada(cliente,evento,5,false);var otro=Fake.Cliente(NivelFidelidad.Bronce);Fake.ComoCliente(otro);GestorListaEspera.Instance.Inscribir(evento.Id,otro.Id);
     }
     if(administrador)Fake.ComoAdministrador();else Fake.ComoCliente(cliente);
     Form form=(Form)Activator.CreateInstance(typeof(Visual.frmInicio).Assembly.GetTypes().Single(t=>t.Name==nombre));
     Asignar(form,"IDUsuario",administrador?Fake.Administrador().Id:cliente.Id);
     Asignar(form,"IDlocal",evento.IdLocal);Asignar(form,"IDEvento",evento.Id);
     Asignar(form,"NombreUsuario","Juan Cruz");Asignar(form,"NombreLocal","Sala central");Asignar(form,"NombreEvento",evento.Nombre);
     if(compra!=null)Asignar(form,"Compra",compra);
     Asignar(form,"Carpeta",Path.Combine(salida,"entradas"));
     Asignar(form,"Rutas",new System.Collections.Generic.List<string>{"EA-000001-1.pdf","EA-000001-2.pdf"});
     using(form) {
      form.TopMost=true;form.StartPosition=FormStartPosition.Manual;form.Location=new Point(40,40);InterfazGrupoA.Mostrar(form);form.BringToFront();form.Activate();
      if(nombre=="FrmCompraCliente") {
       var grilla=InterfazGrupoA.ControlDe<DataGridView>(form,"dgvSectores");grilla.Rows[0].Cells["Cantidad"].Value=2;
       if(grilla.EditingControl is TextBox) ((TextBox)grilla.EditingControl).Text="2";grilla.EndEdit();
      }
      if(nombre=="frmHistorialCliente") InterfazGrupoA.ControlDe<DataGridView>(form,"dgvEntradas").Rows[0].Cells["Devolver"].Value=true;
      if(nombre=="frmConfirmacion") {var d=InterfazGrupoA.ControlDe<Label>(form,"lblDesglose");File.WriteAllText(Path.Combine(salida,"desglose.txt"),d.Bounds+" visible="+d.Visible+" texto="+d.Text+" padre="+d.Parent.Bounds);}
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
  private static void Asignar(Form form,string nombre,object valor) {
   var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.IgnoreCase;
   var campo=form.GetType().GetField(nombre,flags);if(campo!=null) {campo.SetValue(form,valor);return;}
   var propiedad=form.GetType().GetProperty(nombre,flags);if(propiedad!=null && propiedad.CanWrite)propiedad.SetValue(form,valor,null);
  }
 }
}
