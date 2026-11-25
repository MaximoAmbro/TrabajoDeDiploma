param([string]$Salida = 'TestResults/T47/medicion-botones')
$ErrorActionPreference = 'Stop'
$raizT47 = Split-Path $PSScriptRoot -Parent
$binarioT47 = [IO.Path]::GetFullPath((Join-Path $raizT47 'Visual\bin\Debug\Visual.exe'))
if (-not (Test-Path -LiteralPath $binarioT47)) { throw 'Compilá primero con tools/Proyecto.ps1 -Accion Compilar.' }
[void][Reflection.Assembly]::LoadFrom($binarioT47)
$fuenteT47 = @"
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
public static class MedicionBotonesT47
{
    [STAThread]
    public static void Main(string[] args) { Ejecutar(args[0], args[1]); }
    public static void Ejecutar(string raiz, string salida)
    {
        Application.EnableVisualStyles();
        Directory.CreateDirectory(salida);
        var medidas = new List<string> { "Pantalla;Control;Texto;AnchoDisponible;AltoDisponible;AnchoPreferido;AltoPreferido;PaddingHorizontal;AnchoTexto;EntraAncho;EntraAlto;TintaTextoDisponible;TintaTextoReferencia;TextoRenderizadoCompleto;AltoMinimoRenderizado" };
        var colores = new List<string> { "Pantalla;Control;PixelesPrimario;PixelesTotales;Proporcion;UseVisualStyleBackColor;Resultado" };
        var objetivos = new Dictionary<string,string[]> {
            {"frmEventosCliente", new[]{"btnBuscar","btnLimpiar"}},
            {"frmHistorialCliente", new[]{"btnVolver"}},
            {"frmSuscripcion", new[]{"btnVolver"}},
            {"frmAdministrarEvento", new[]{"btnVolver"}},
            {"frmEventosLocal", new[]{"btnEliminarEvento","btnVolver","btnAdministrarEvento","btnAgregarEvento"}},
            {"frmListasEspera", new[]{"btnVolver"}},
            {"frmMisLocales", new[]{"btnVolver"}},
            {"frmMenuAdministrador", new[]{"btnReportesGlobales","btnCerrarSesion"}},
            {"frmReportes", new[]{"btnGenerar","btnExportar","btnVolver"}}
        };
        int cortes = 0, medidos = 0, pantallas = 0, primarios = 0;
        var fallos = new List<string>();
        foreach (string archivo in Directory.GetFiles(Path.Combine(raiz,"Visual"),"*.Designer.cs",SearchOption.AllDirectories))
        {
            string fuente = File.ReadAllText(archivo);
            if (!fuente.Contains("ClientSize")) continue;
            string nombre = Regex.Match(fuente,@"partial\s+class\s+(\w+)").Groups[1].Value;
            var tipo = typeof(Visual.frmInicio).Assembly.GetTypes().Single(t => t.Name == nombre);
            using (var form = (Form)Activator.CreateInstance(tipo))
            {
                pantallas++;
                var escala = Regex.Match(fuente,@"AutoScaleDimensions\s*=\s*new\s+System.Drawing.SizeF\((\d+)F,\s*(\d+)F\)");
                float dpiX = float.Parse(escala.Groups[1].Value), dpiY = float.Parse(escala.Groups[2].Value);
                float actualX, actualY;
                using (Graphics g = form.CreateGraphics()) { actualX = g.DpiX; actualY = g.DpiY; }
                if (objetivos.ContainsKey(nombre))
                foreach (string id in objetivos[nombre])
                {
                    var boton = (Button)form.Controls.Find(id,true).Single();
                    var size = Regex.Match(fuente,@"this\."+id+@"\.Size\s*=\s*new\s+System.Drawing.Size\((\d+),\s*(\d+)\)");
                    boton.MinimumSize = Size.Empty; // Solo la instancia de medición: no cambia el producto.
                    boton.Size = new Size((int)Math.Round(int.Parse(size.Groups[1].Value)*actualX/dpiX),
                        (int)Math.Round(int.Parse(size.Groups[2].Value)*actualY/dpiY));
                    // Button.GetPreferredSize usa el motor real de layout: incluye texto, padding y borde.
                    boton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    Size necesario = boton.GetPreferredSize(Size.Empty);
                    Size texto = TextRenderer.MeasureText(boton.Text, boton.Font);
                    bool ancho = necesario.Width <= boton.ClientSize.Width;
                    bool alto = necesario.Height <= boton.ClientSize.Height;
                                        boton.Enabled = true;
                    int textoDisponible = ContarTexto(boton, Path.Combine(salida,nombre+"-"+id+"-sin-minimo.png"));
                    Size disponible = boton.Size;
                    boton.Size = necesario;
                    int textoReferencia = ContarTexto(boton, Path.Combine(salida,nombre+"-"+id+"-referencia.png"));
                    boton.Size = disponible;
                    int altoMinimo=0;
                    for(int altoPrueba=1;altoPrueba<=necesario.Height;altoPrueba++)
                    {
                        boton.Size=new Size(necesario.Width,altoPrueba);
                        if(ContarTexto(boton,null)>=textoReferencia*0.999) { altoMinimo=altoPrueba; break; }
                    }
                    boton.Size = disponible;
                    bool renderCompleto = textoDisponible >= textoReferencia * 0.999;
                    medidas.Add(string.Join(";",nombre,id,boton.Text,boton.ClientSize.Width,boton.ClientSize.Height,
                        necesario.Width,necesario.Height,boton.Padding.Horizontal,texto.Width,ancho,alto,textoDisponible,textoReferencia,renderCompleto,altoMinimo));
                    medidos++;
                    if (!renderCompleto) cortes++;
                }
                int dePantalla = 0;
                foreach (Button boton in Controles(form).OfType<Visual.Componentes.BotonPrimario>())
                {
                    dePantalla++; primarios++;
                    boton.Enabled = true; // El primario habilitado es violeta; deshabilitado tiene su token gris.
                    using (var imagen = new Bitmap(boton.Width,boton.Height))
                    {
                        // WM_PRINT del control nativo: se examinan los píxeles, no BackColor.
                        boton.DrawToBitmap(imagen,new Rectangle(Point.Empty,imagen.Size));
                        int coinciden=0, total=imagen.Width*imagen.Height;
                        Color esperado = Visual.Tema.Primario;
                        for (int y=0;y<imagen.Height;y++) for (int x=0;x<imagen.Width;x++)
                        {
                            Color pixel=imagen.GetPixel(x,y);
                            if (Math.Abs(pixel.R-esperado.R)<=2 && Math.Abs(pixel.G-esperado.G)<=2 && Math.Abs(pixel.B-esperado.B)<=2) coinciden++;
                        }
                        double proporcion=(double)coinciden/total;
                        // Una mayoría de superficie violeta evita aprobar texto o bordes aislados.
                        bool bien=proporcion>0.5;
                        colores.Add(string.Join(";",nombre,boton.Name,coinciden,total,
                            proporcion.ToString("F4",System.Globalization.CultureInfo.InvariantCulture),boton.UseVisualStyleBackColor,bien?"OK":"FALLA"));
                        imagen.Save(Path.Combine(salida,nombre+"-"+boton.Name+".png"));
                        if (!bien) fallos.Add(nombre+"."+boton.Name);
                    }
                }
                if (dePantalla==0) colores.Add(nombre+";SIN_PRIMARIOS;;;;;NO_APLICA");
            }
        }
        // Control negativo: el botón estándar con estilo del sistema no debe aprobar como violeta.
        using(var sistema = new Button { Size=new Size(160,40), BackColor=Visual.Tema.Primario, UseVisualStyleBackColor=true })
        using(var imagen = new Bitmap(sistema.Width,sistema.Height))
        {
            sistema.DrawToBitmap(imagen,new Rectangle(Point.Empty,imagen.Size));
            imagen.Save(Path.Combine(salida,"control-negativo-sistema.png"));
            int iguales=0;
            for(int y=0;y<imagen.Height;y++) for(int x=0;x<imagen.Width;x++)
                if(imagen.GetPixel(x,y).ToArgb()==Visual.Tema.Primario.ToArgb()) iguales++;
            if(iguales>imagen.Width*imagen.Height*0.5) throw new Exception("Control negativo inválido: no distingue el estilo del sistema.");
        }
        File.WriteAllLines(Path.Combine(salida,"medidas.csv"),medidas,System.Text.Encoding.UTF8);
        File.WriteAllLines(Path.Combine(salida,"colores.csv"),colores,System.Text.Encoding.UTF8);
        Console.WriteLine("Medidos: "+medidos+"; no caben: "+cortes+"; pantallas: "+pantallas+"; primarios renderizados: "+primarios+"; fallos color: "+fallos.Count);
        if (medidos!=16 || pantallas!=22) throw new Exception("Inventario incompleto.");
        if (fallos.Count>0) throw new Exception("Color renderizado incorrecto: "+string.Join(", ",fallos));
    }
    private static int ContarTexto(Button boton, string ruta)
    {
        using (var imagen = new Bitmap(boton.Width,boton.Height))
        {
            boton.DrawToBitmap(imagen,new Rectangle(Point.Empty,imagen.Size));
            if(ruta!=null) imagen.Save(ruta);
            int cantidad=0;
            Color fondo=boton.BackColor;
            for(int y=1;y<imagen.Height-1;y++) for(int x=1;x<imagen.Width-1;x++)
            {
                Color pixel=imagen.GetPixel(x,y);
                cantidad += Math.Abs(pixel.R-fondo.R)+Math.Abs(pixel.G-fondo.G)+Math.Abs(pixel.B-fondo.B);
            }
            return cantidad;
        }
    }
    private static IEnumerable<Control> Controles(Control padre)
    {
        foreach(Control hijo in padre.Controls) { yield return hijo; foreach(var otro in Controles(hijo)) yield return otro; }
    }
}
"@
$salidaT47 = Join-Path $raizT47 $Salida
[void][IO.Directory]::CreateDirectory($salidaT47)
$fuenteRutaT47 = Join-Path $salidaT47 'MedicionBotonesT47.cs'
[IO.File]::WriteAllText($fuenteRutaT47, $fuenteT47, (New-Object Text.UTF8Encoding($false)))
$ejecutableT47 = Join-Path (Split-Path $binarioT47 -Parent) 'MedicionBotonesT47.exe'
$cscT47 = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $cscT47 /nologo /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Core.dll "/r:$binarioT47" "/out:$ejecutableT47" $fuenteRutaT47
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar la medición.' }
& $ejecutableT47 $raizT47 $salidaT47
if ($LASTEXITCODE -ne 0) { throw 'Falló la verificación de botones.' }








