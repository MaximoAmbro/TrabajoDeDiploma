using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasTextoBotones
    {
        // CP-116 — RNF-02/RNF-03: ningún botón recorta su texto al dibujarse.
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
        [DataRow("frmMenuAdministrador", DisplayName = "CP-116: Botones completos de frmMenuAdministrador")]
        [DataRow("frmMenuPropietario", DisplayName = "CP-116: Botones completos de frmMenuPropietario")]
        [DataRow("frmMisLocales", DisplayName = "CP-116: Botones completos de frmMisLocales")]
        [DataRow("frmPago", DisplayName = "CP-116: Botones completos de frmPago")]
        [DataRow("frmRegistrarLocal", DisplayName = "CP-116: Botones completos de frmRegistrarLocal")]
        [DataRow("frmReportes", DisplayName = "CP-116: Botones completos de frmReportes")]
        [DataRow("frmRevisarEventos", DisplayName = "CP-116: Botones completos de frmRevisarEventos")]
        [DataRow("frmAuditoria", DisplayName = "CP-116: Botones completos de frmAuditoria")]
        [DataRow("frmRevisarSolicitudesOrganizador", DisplayName = "CP-116: Botones completos de frmRevisarSolicitudesOrganizador")]
        [DataRow("frmSolicitudOrganizador", DisplayName = "CP-116: Botones completos de frmSolicitudOrganizador")]
        [DataRow("frmSuscripcion", DisplayName = "CP-116: Botones completos de frmSuscripcion")]
        public void CP116_TextoCompleto(string pantalla)
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() =>
            {
                var tipo = typeof(Visual.frmInicio).Assembly.GetTypes().Single(t => t.Name == pantalla);
                if (pantalla == "frmRevisarEventos" || pantalla == "frmAuditoria" || pantalla == "frmRevisarSolicitudesOrganizador") Fake.ComoAdministrador();
                if (pantalla == "frmSolicitudOrganizador") Fake.ComoCliente(Fake.Cliente(NivelFidelidad.Bronce));
                using (var form = (Form)Activator.CreateInstance(tipo))
                {
                    Visual.Tema.Normalizar(form);
                    int margen = (int)Math.Round(Visual.Tema.MargenFormulario * Math.Max(96, form.DeviceDpi) / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero);
                    if (pantalla == "FrmCompraCliente")
                    {
                        Assert.AreEqual(24, Descendientes(form).OfType<Panel>().Single(p => p.Name == "pnlContenidoCompra").AutoScrollMargin.Width, "margen interior del contenido de compra");
                        Assert.AreEqual(24, Descendientes(form).OfType<Panel>().Single(p => p.Name == "pnlAccionesCompra").Padding.Left, "margen del pie de compra");
                    }
                    else Assert.AreEqual(margen, form.Padding.Left, pantalla + " margen común de ventana");
                    var errores = new List<string>();
                    var filas = new List<string> { "Pantalla;Control;Texto;Ancho;Alto;NecesitaAncho;NecesitaAlto;Completo" };
                    foreach (Button boton in Descendientes(form).OfType<Button>())
                    {
                        int dpiBoton = Math.Max(96, boton.DeviceDpi);
                        int altoEsperado = (int)Math.Round((boton is Visual.Componentes.BotonPrimario ? Visual.Tema.AltoBotonPrimario : Visual.Tema.AltoBoton) * dpiBoton / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero);
                        int[] anchosNormalizados = { Visual.Tema.AnchoBotonCompacto, Visual.Tema.AnchoBotonNormal, Visual.Tema.AnchoBotonAmplio };
                        int relleno = (int)Math.Round(Visual.Tema.RellenoHorizontalBoton * dpiBoton / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero);
                        if (boton.Parent?.Name == "navegacionLateral")
                            altoEsperado = Visual.Disposicion.Pixeles(boton, Visual.Disposicion.AltoOpcionLateral);
                        Assert.AreEqual(altoEsperado, boton.Height, pantalla + "/" + boton.Name + " alto normalizado");
                        if (pantalla == "frmInicio" && boton.Name == "btninicio")
                            Assert.AreEqual(Visual.Disposicion.Pixeles(boton, Visual.Tema.AnchoCampo), boton.Width, pantalla + "/" + boton.Name + " ancho de fila");
                        else if (boton.Name == "btnListaEspera")
                            Assert.AreEqual(boton.Parent.ClientSize.Width - boton.Left * 2, boton.Width, pantalla + "/" + boton.Name + " ancho de fila");
                        else
                            Assert.IsTrue(anchosNormalizados.Any(ancho => boton.Width == (int)Math.Round(ancho * dpiBoton / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero)),
                                pantalla + "/" + boton.Name + " debe usar uno de los anchos comunes: " + boton.Width);
                        Assert.AreEqual(Visual.Tema.Boton.Name, boton.Font.Name, pantalla + "/" + boton.Name + " tipografía");
                        int rellenoIzquierdo = boton.Controls.OfType<Label>().Any(etiqueta => etiqueta.Name.StartsWith("icono", StringComparison.OrdinalIgnoreCase))
                            ? (int)Math.Round(Visual.Tema.RellenoIconoBoton * dpiBoton / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero)
                            : relleno;
                        Assert.AreEqual(rellenoIzquierdo, boton.Padding.Left, pantalla + "/" + boton.Name + " margen interior");
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
                    foreach (Control control in Descendientes(form).Where(c => c.Parent is FlowLayoutPanel || c.Parent is TableLayoutPanel))
                    {
                        int dpiControl = Math.Max(96, control.DeviceDpi);
                        int margenEsperado = (int)Math.Round((Visual.Tema.SeparacionControl / 2) * dpiControl / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero);
                        Assert.AreEqual(new Padding(margenEsperado), control.Margin, pantalla + "/" + control.Name + " separación de layout");
                    }
                    foreach (Control contenedor in new Control[] { form }.Concat(Descendientes(form)))
                    {
                        var botonesFila = contenedor.Controls.OfType<Button>().OrderBy(b => b.Top).ThenBy(b => b.Left).ToArray();
                        for (int i = 0; i < botonesFila.Length; i++)
                            for (int j = i + 1; j < botonesFila.Length; j++)
                            {
                                var a = botonesFila[i]; var b = botonesFila[j];
                                int solape = Math.Min(a.Bottom, b.Bottom) - Math.Max(a.Top, b.Top);
                                if (solape >= Math.Min(a.Height, b.Height) / 2 && b.Left >= a.Left)
                                {
                                    int requerido = (int)Math.Round(Visual.Tema.Espacio.S * Math.Max(96, contenedor.DeviceDpi) / Visual.Tema.DpiDiseno, MidpointRounding.AwayFromZero);
                                    int anchoRequerido = a.Width + b.Width + requerido + contenedor.Padding.Horizontal;
                                    if (contenedor.ClientSize.Width > 0 && contenedor.ClientSize.Width < anchoRequerido) continue;
                                    Assert.IsTrue(b.Left - a.Right >= requerido, pantalla + " separación " + a.Name + " / " + b.Name + "; gap=" + (b.Left - a.Right) + "; a=" + a.Bounds + "; b=" + b.Bounds + "; parent=" + contenedor.ClientSize);
                                }
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
