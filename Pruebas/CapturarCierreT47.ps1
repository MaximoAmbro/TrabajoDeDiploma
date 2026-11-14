param([Parameter(Mandatory=$true)][string]$Visual, [Parameter(Mandatory=$true)][string]$Salida, [int]$Escala=100, [string]$Pantallas="")
$ErrorActionPreference='Stop'
$raizCaptura = Split-Path $PSScriptRoot -Parent
$binarioCaptura = [IO.Path]::GetFullPath((Join-Path $raizCaptura $Visual))
$salidaCaptura = [IO.Path]::GetFullPath((Join-Path $raizCaptura $Salida))
[void][IO.Directory]::CreateDirectory($salidaCaptura)
$fuenteCaptura = @"
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
public class CapturaCierreT47 {
 [DllImport("user32.dll")] static extern uint GetDpiForWindow(IntPtr ventana);
 [STAThread] public static void Main(string[] args) {
  Application.EnableVisualStyles();
  var ensamblado=Assembly.LoadFrom(args[0]); string salida=args[1]; int escala=int.Parse(args[2]);
  var lineas=new List<string>{"Pantalla;Control;Texto;Fuente;Puntos;Ancho;Alto;AnchoPreferido;AltoPreferido;TextoCompleto;Dpi;Cliente;Ventana;Dentro;WorkArea"};
  foreach(string nombre in args.Length>3 && args[3].Length>0 ? args[3].Split(',') : new[]{"frmEventosCliente","frmEventosLocal","frmReportes"}) {
   using(var form=(Form)Activator.CreateInstance(ensamblado.GetTypes().Single(t=>t.Name==nombre))) {
    // Fixture de disposición sin datos: se abre el formulario real, sin consultar bases.
    object clave=typeof(Form).GetField("EVENT_LOAD",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
    var eventos=(EventHandlerList)typeof(Component).GetProperty("Events",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(form,null);
    eventos.RemoveHandler(clave,eventos[clave]);
    form.TopMost=true; form.StartPosition=FormStartPosition.CenterScreen;
    form.Show(); form.Refresh(); Application.DoEvents();
    int dpi=(int)GetDpiForWindow(form.Handle);
    if(dpi!=escala*96/100) throw new Exception("DPI real inesperado: "+dpi+" para "+escala);
    var ids = Hijos(form).OfType<Button>().Where(b=>b.Name.Length>0).Select(b=>b.Name).ToArray();
    foreach(string id in ids) {
     var boton=(Button)form.Controls.Find(id,true).Single(); boton.Enabled=true;
     boton.AutoSizeMode=AutoSizeMode.GrowAndShrink;
     Size original=boton.Size, preferido=boton.GetPreferredSize(Size.Empty);
     double tinta=Medir(boton);
     boton.Size=preferido; double referencia=Medir(boton); boton.Size=original;
     bool completo=tinta>=referencia*0.999;
     lineas.Add(string.Join(";",nombre,id,boton.Text,boton.Font.Name,boton.Font.SizeInPoints.ToString(System.Globalization.CultureInfo.InvariantCulture),
      original.Width,original.Height,preferido.Width,preferido.Height,completo,dpi,form.ClientSize,form.Size,form.ClientRectangle.Contains(boton.Bounds),Screen.FromControl(form).WorkingArea));
    }
    form.Refresh(); Application.DoEvents();
    using(var imagen=new Bitmap(form.Width,form.Height)) using(var g=Graphics.FromImage(imagen)) {
     g.CopyFromScreen(form.Location,Point.Empty,form.Size); imagen.Save(Path.Combine(salida,nombre+"-escritorio.png"));
    }
    using(var completa=new Bitmap(form.Width,form.Height)) {form.DrawToBitmap(completa,new Rectangle(Point.Empty,form.Size));completa.Save(Path.Combine(salida,nombre+".png"));}
   }
  }
  File.WriteAllLines(Path.Combine(salida,"medidas.csv"),lineas,System.Text.Encoding.UTF8);
 }
 static IEnumerable<Control> Hijos(Control padre) { foreach(Control c in padre.Controls) {yield return c;foreach(var h in Hijos(c)) yield return h;} }
 static double Medir(Button boton) {
  using(var imagen=new Bitmap(boton.Width,boton.Height)) {
   boton.DrawToBitmap(imagen,new Rectangle(Point.Empty,imagen.Size)); double tinta=0; var fondo=boton.BackColor;
   int borde=Math.Max(1,boton.FlatAppearance.BorderSize);
   for(int y=borde;y<imagen.Height-borde;y++) for(int x=borde;x<imagen.Width-borde;x++) {
    var p=imagen.GetPixel(x,y); tinta+=Math.Abs(p.R-fondo.R)+Math.Abs(p.G-fondo.G)+Math.Abs(p.B-fondo.B);
   }
   return tinta;
  }
 }
}
"@
$fuenteRutaCaptura=Join-Path $salidaCaptura 'CapturaCierreT47.cs'
[IO.File]::WriteAllText($fuenteRutaCaptura,$fuenteCaptura,(New-Object Text.UTF8Encoding($false)))
$exeCaptura=Join-Path (Split-Path $binarioCaptura -Parent) 'CapturaCierreT47.exe'
$cscCaptura=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $cscCaptura /nologo /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll "/win32manifest:$(Join-Path $raizCaptura 'Visual/app.manifest')" "/out:$exeCaptura" $fuenteRutaCaptura
if($LASTEXITCODE -ne 0) { throw 'No compiló la captura.' }
& $exeCaptura $binarioCaptura $salidaCaptura $Escala $Pantallas
if($LASTEXITCODE -ne 0) { throw 'Falló la captura.' }
