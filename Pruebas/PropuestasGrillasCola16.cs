using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;
namespace Pruebas {
 // Propuestas para decidir distribución. No cambia formularios ni Designer de producción.
 public static class PropuestasGrillasCola16 {
  private static Control C(Form f,string n){return f.Controls.Find(n,true).Single();}
  private static void P(Form f,string n,int x,int y,int w,int h){var c=C(f,n);c.AutoSize=false;c.Anchor=AnchorStyles.Top|AnchorStyles.Left;c.Bounds=new Rectangle(x,y,w,h);}
  public static void Generar(string salida){
   Directory.CreateDirectory(salida);
   File.WriteAllText(Path.Combine(salida,"medidas.txt"),string.Empty);
   foreach(string nombre in new[]{"Historial","Reportes"}) foreach(string opcion in new[]{"A","B"}) InterfazGrupoA.EnSta(()=>{
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);var evento=Fake.EventoPublicado(1000m,5);Fake.CompraConfirmada(cliente,evento,2,true);
    Form f=nombre=="Historial"?(Form)new Visual.frmHistorialCliente{IDUsuario=cliente.Id}:new Visual.frmReportes();
    if(nombre=="Historial")Fake.ComoCliente(cliente);else Fake.ComoAdministrador();
    using(f){
     // Medidas de propuesta en píxeles lógicos a 96 DPI; el formulario escala de una sola vez.
     f.AutoScaleMode=AutoScaleMode.None;f.ClientSize=new Size(900,512);f.Text="PROPUESTA "+nombre+" "+opcion;
     P(f,"label1",24,24,852,32);
     if(nombre=="Historial"){
      P(f,"dgvCompras",24,80,852,120);P(f,"lblDetalle",24,216,852,24);P(f,"dgvEntradas",24,248,852,128);
      P(f,"lblAyuda",24,392,852,40);P(f,"btnVolver",24,448,144,40);P(f,"btnDevolver",680,448,196,40);
      if(opcion=="B"){
       var tabs=new TabControl{Name="propuestaPestanas",Bounds=new Rectangle(24,80,852,296)};
       var compras=new TabPage("Compras");var entradas=new TabPage("Entradas del pedido");tabs.TabPages.Add(compras);tabs.TabPages.Add(entradas);f.Controls.Add(tabs);
       var g=C(f,"dgvCompras");g.Parent=compras;g.Dock=DockStyle.Fill;g=C(f,"dgvEntradas");g.Parent=entradas;g.Dock=DockStyle.Fill;C(f,"lblDetalle").Visible=false;
      }
     }else{
      if(opcion=="A"){
       P(f,"lblReporte",24,72,440,20);P(f,"cmbReporte",24,96,440,28);P(f,"lblDesde",488,72,184,20);P(f,"dtpDesde",488,96,184,28);P(f,"lblHasta",696,72,180,20);P(f,"dtpHasta",696,96,180,28);
       P(f,"lblFiltro",24,144,440,20);P(f,"cmbFiltro",24,168,440,28);P(f,"dtpCorte",24,168,440,28);P(f,"chkSoloAgotados",488,144,388,24);P(f,"btnGenerar",688,176,188,40);
       P(f,"dgvReporte",24,232,852,152);P(f,"lblLectura",24,392,852,40);
      }else{
       P(f,"lblReporte",24,80,288,20);P(f,"cmbReporte",24,104,288,28);P(f,"lblDesde",24,144,136,20);P(f,"dtpDesde",24,168,136,28);P(f,"lblHasta",176,144,136,20);P(f,"dtpHasta",176,168,136,28);
       P(f,"lblFiltro",24,208,288,20);P(f,"cmbFiltro",24,232,288,28);P(f,"dtpCorte",24,232,288,28);P(f,"chkSoloAgotados",24,272,288,48);P(f,"btnGenerar",24,336,288,40);
       P(f,"dgvReporte",336,80,540,288);
       var lectura=C(f,"lblLectura");var panelLectura=new Panel{Name="propuestaLectura",Bounds=new Rectangle(336,384,540,56),AutoScroll=true};
       f.Controls.Add(panelLectura);lectura.Parent=panelLectura;lectura.AutoSize=false;lectura.Bounds=new Rectangle(0,0,516,120);
      }
      P(f,"btnVolver",24,448,144,40);P(f,"btnExportar",680,448,196,40);
     }
     float escala;using(var g=f.CreateGraphics())escala=g.DpiX/96f;
     if(escala!=1f)f.Scale(new SizeF(escala,escala));
     f.StartPosition=FormStartPosition.Manual;f.Location=new Point(32,32);f.Show();Application.DoEvents();
     if(nombre=="Reportes"){((DateTimePicker)C(f,"dtpHasta")).Value=DateTime.Today.AddDays(40);((Button)C(f,"btnGenerar")).PerformClick();}
     f.PerformLayout();f.Refresh();Application.DoEvents();
     Capturar(f,Path.Combine(salida,nombre+"-"+opcion+".png"));
     if(nombre=="Historial"&&opcion=="B"){((TabControl)C(f,"propuestaPestanas")).SelectedIndex=1;Application.DoEvents();Capturar(f,Path.Combine(salida,nombre+"-B-detalle.png"));}
     using(var g=f.CreateGraphics())File.AppendAllText(Path.Combine(salida,"medidas.txt"),nombre+"-"+opcion+" DPI="+g.DpiX+" cliente="+f.ClientSize+"\r\n");
    }
   });
  }
  private static void Capturar(Form f,string ruta){using(var b=new Bitmap(f.Width,f.Height)){f.DrawToBitmap(b,new Rectangle(Point.Empty,f.Size));b.Save(ruta);}}
 }
}
