using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasTextoBotones
    {
        // CP-116 — RNF-02/RNF-03: texto nativo completo, incluso con el borde de foco.
        [DataTestMethod]
        [DataRow("FrmCompraCliente", DisplayName = "CP-116: Botones completos de FrmCompraCliente")]
        [DataRow("frmAdministrarEvento", DisplayName = "CP-116: Botones completos de frmAdministrarEvento")]
        [DataRow("frmAgregarEvento", DisplayName = "CP-116: Botones completos de frmAgregarEvento")]
        [DataRow("frmAgregarSector", DisplayName = "CP-116: Botones completos de frmAgregarSector")]
        [DataRow("frmCambiarContraseña", DisplayName = "CP-116: Botones completos de frmCambiarContraseña")]
        [DataRow("frmConfirmacion", DisplayName = "CP-116: Botones completos de frmConfirmacion")]
        [DataRow("frmControlIngreso", DisplayName = "CP-116: Botones completos de frmControlIngreso")]
        [DataRow("frmCrearUsuario", DisplayName = "CP-116: Botones completos de frmCrearUsuario")]
        [DataRow("frmEventosCliente", DisplayName = "CP-116: Botones completos de frmEventosCliente")]
        [DataRow("frmEventosLocal", DisplayName = "CP-116: Botones completos de frmEventosLocal")]
        [DataRow("frmHistorialCliente", DisplayName = "CP-116: Botones completos de frmHistorialCliente")]
        [DataRow("frmInicio", DisplayName = "CP-116: Botones completos de frmInicio")]
        [DataRow("frmListasEspera", DisplayName = "CP-116: Botones completos de frmListasEspera")]
        [DataRow("frmMenuCliente", DisplayName = "CP-116: Botones completos de frmMenuCliente")]
        [DataRow("frmMenuPropietario", DisplayName = "CP-116: Botones completos de frmMenuPropietario")]
        [DataRow("frmMisLocales", DisplayName = "CP-116: Botones completos de frmMisLocales")]
        [DataRow("frmPago", DisplayName = "CP-116: Botones completos de frmPago")]
        [DataRow("frmRegistrarLocal", DisplayName = "CP-116: Botones completos de frmRegistrarLocal")]
        [DataRow("frmReportes", DisplayName = "CP-116: Botones completos de frmReportes")]
        [DataRow("frmSuscripcion", DisplayName = "CP-116: Botones completos de frmSuscripcion")]
        public void CP116_TextoCompleto(string pantalla)
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() =>
            {
                var tipo = typeof(Visual.frmInicio).Assembly.GetTypes().Single(t => t.Name == pantalla);
                using (var form = (Form)Activator.CreateInstance(tipo))
                {
                    Visual.Tema.Normalizar(form);
                    var errores = new List<string>();
                    var filas = new List<string> { "Pantalla;Control;Texto;Ancho;Alto;NecesitaAncho;NecesitaAlto;Completo" };
                    foreach (Button boton in Descendientes(form).OfType<Button>())
                    {
                        // También se prueban estados habilitables que nacen deshabilitados/ocultos.
                        foreach(string texto in Textos(boton))
                        using (var copia = new Button { Text = texto, Font = boton.Font, Padding = boton.Padding,
                            FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = Color.Black,
                            AutoSizeMode = AutoSizeMode.GrowAndShrink, Size = boton.Size, UseCompatibleTextRendering = boton.UseCompatibleTextRendering })
                        {
                            copia.UseVisualStyleBackColor = false;
                            copia.FlatAppearance.BorderSize = Visual.Tema.BordeFoco;
                            copia.FlatAppearance.BorderColor = Color.White;
                            Size disponible = copia.Size;
                            Size necesaria = copia.GetPreferredSize(Size.Empty);
                            double tinta = Tinta(copia);
                            copia.Size = necesaria;
                            double referencia = Tinta(copia);
                            int altoMinimo = necesaria.Height;
                            for(int alto=1;alto<=necesaria.Height;alto++)
                            {
                                copia.Size = new Size(necesaria.Width,alto);
                                if(Tinta(copia)>=referencia*0.995) { altoMinimo=alto; break; }
                            }
                            necesaria = new Size(necesaria.Width,altoMinimo);
                            bool completo = referencia == 0 || tinta >= referencia * 0.995;
                            filas.Add(string.Join(";",pantalla,boton.Name,texto,disponible.Width,disponible.Height,necesaria.Width,necesaria.Height,completo));
                            if (!completo) errores.Add(boton.Name + " «" + texto + "»: " + disponible + ", necesita " + necesaria);
                        }
                    }
                    float dpi; using(var grafico=Graphics.FromHwnd(IntPtr.Zero)) dpi=grafico.DpiX;
                    string salida = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "Botones", dpi.ToString());
                    Directory.CreateDirectory(salida);
                    File.WriteAllLines(Path.Combine(salida,pantalla+".csv"),filas,System.Text.Encoding.UTF8);
                    Assert.AreEqual(0,errores.Count,string.Join(Environment.NewLine,errores));
                }
            });
        }
        private static IEnumerable<string> Textos(Button boton)
        {
            yield return boton.Text;
            if(boton.Name=="btnDevolver")
            {
                yield return "Devolver 1 entrada(s)";
                yield return "Devolver "+Negocio.GestorParametros.Instance.ObtenerEntero("COMPRA_ENTRADAS_MAX")+" entrada(s)";
            }
            if(boton.Name=="btnListaEspera") yield return "Ya estás en la lista de espera";
        }
        private static IEnumerable<Control> Descendientes(Control padre)
        {
            foreach(Control hijo in padre.Controls) { yield return hijo; foreach(var otro in Descendientes(hijo)) yield return otro; }
        }
        private static double Tinta(Button boton)
        {
            using(var imagen = new Bitmap(Math.Max(1,boton.Width),Math.Max(1,boton.Height)))
            {
                boton.DrawToBitmap(imagen,new Rectangle(Point.Empty,imagen.Size));
                double suma=0; int borde=Visual.Tema.BordeFoco;
                for(int y=borde;y<imagen.Height-borde;y++) for(int x=borde;x<imagen.Width-borde;x++)
                {
                    var p=imagen.GetPixel(x,y); suma+=(255-p.R)+(255-p.G)+(255-p.B);
                }
                return suma;
            }
        }
    }
}
