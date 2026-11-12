using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual;
using Visual.Componentes;

namespace Pruebas
{
    [TestClass]
    public class PruebasSerializacionComponentes
    {
        // CP-112 — RNF-01/RNF-02, T-41: host real, siete filas; ninguna serializa su estilo.
        [DataTestMethod]
        [DataRow(typeof(BotonPrimario), DisplayName = "CP-112: BotonPrimario no serializa estilo en el host de diseño")]
        [DataRow(typeof(BotonSecundario), DisplayName = "CP-112: BotonSecundario no serializa estilo en el host de diseño")]
        [DataRow(typeof(BotonPeligro), DisplayName = "CP-112: BotonPeligro no serializa estilo en el host de diseño")]
        [DataRow(typeof(EtiquetaTitulo), DisplayName = "CP-112: EtiquetaTitulo no serializa estilo en el host de diseño")]
        [DataRow(typeof(EtiquetaSeccion), DisplayName = "CP-112: EtiquetaSeccion no serializa estilo en el host de diseño")]
        [DataRow(typeof(EtiquetaRotulo), DisplayName = "CP-112: EtiquetaRotulo no serializa estilo en el host de diseño")]
        [DataRow(typeof(EtiquetaAyuda), DisplayName = "CP-112: EtiquetaAyuda no serializa estilo en el host de diseño")]
        public void EstiloNoSerializado(Type tipo)
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() =>
            {
                using (var superficie = new DesignSurface(typeof(Form)))
                {
                    Assert.AreEqual(0, superficie.LoadErrors.Count, "No debe haber errores al cargar el host.");
                    var host = (IDesignerHost)superficie.GetService(typeof(IDesignerHost));
                    Assert.IsNotNull(host);
                    Assert.IsNotNull(superficie.View);
                    var raiz = (Form)host.RootComponent;
                    var control = (Control)host.CreateComponent(tipo, "componente");
                    raiz.Controls.Add(control);
                    Assert.IsTrue(control.Site.DesignMode);
                    Assert.IsNotNull(host.GetDesigner(control));
                    string[] propiedades = control is Button
                        ? new[] { "Font", "BackColor", "ForeColor", "FlatStyle", "FlatAppearance", "MinimumSize", "Padding", "Cursor", "UseVisualStyleBackColor" }
                        : new[] { "Font", "BackColor", "ForeColor" };
                    PropertyDescriptorCollection descriptores = TypeDescriptor.GetProperties(control);
                    foreach (string nombre in propiedades)
                    {
                        Assert.IsNotNull(descriptores[nombre]);
                        Assert.IsFalse(descriptores[nombre].ShouldSerializeValue(control), tipo.Name + "." + nombre + " se serializa.");
                    }
                    Font fuenteEsperada = tipo == typeof(EtiquetaTitulo) ? Tema.Titulo :
                        tipo == typeof(EtiquetaSeccion) ? Tema.Seccion : tipo == typeof(EtiquetaRotulo) ? Tema.Rotulo :
                        tipo == typeof(EtiquetaAyuda) ? Tema.Ayuda : Tema.Boton;
                    Assert.AreEqual(fuenteEsperada, control.Font, "El estilo debe verse también en el host.");
                    if (control is Button)
                        Assert.AreEqual(DesignerSerializationVisibility.Hidden, descriptores["FlatAppearance"].SerializationVisibility,
                            "El objeto de apariencia tampoco debe serializar sus colores internos.");
                    control.Text = "Texto editable";
                    control.Location = new Point(24, 24);
                    control.Size = new Size(200, 48);
                    foreach (string nombre in new[] { "Text", "Location", "Size" })
                        Assert.IsTrue(descriptores[nombre].ShouldSerializeValue(control), nombre + " debe seguir siendo editable y serializable.");
                    // Control negativo: el mismo host sí detecta el problema en un botón normal.
                    var sinProteccion = (Button)host.CreateComponent(typeof(Button), "sinProteccion");
                    raiz.Controls.Add(sinProteccion);
                    sinProteccion.Font = Tema.Boton;
                    Assert.IsTrue(TypeDescriptor.GetProperties(sinProteccion)["Font"].ShouldSerializeValue(sinProteccion));

                    if (tipo == typeof(BotonPrimario))
                    {
                        VerificarFocoCacheYRepeticion();
                        VerificarVistaRealDelPiloto();
                    }
                }
            });
        }

        private static void VerificarFocoCacheYRepeticion()
        {
            using (var form = new Form())
            using (var primario = new BotonPrimario())
            using (var secundario = new BotonSecundario())
            {
                primario.Location = new Point(0, 0);
                secundario.Location = new Point(0, 64);
                form.Controls.Add(primario);
                form.Controls.Add(secundario);
                form.Show();
                primario.Focus();
                Application.DoEvents();
                Assert.IsTrue(primario.Focused);
                Assert.AreEqual(Tema.Foco, primario.FlatAppearance.BorderColor);
                Assert.AreEqual(Tema.BordeFoco, primario.FlatAppearance.BorderSize);
                secundario.Focus();
                Application.DoEvents();
                Assert.AreEqual(0, primario.FlatAppearance.BorderSize);
                primario.Enabled = false;
                Assert.AreEqual(Tema.FondoDeshabilitado, primario.BackColor);
                primario.Enabled = true;
                Assert.AreEqual(Tema.Primario, primario.BackColor);
            }
            using (var origen = new Font("Microsoft Sans Serif", 10f, FontStyle.Italic | FontStyle.Underline))
            using (var uno = new Label { Font = origen })
            using (var dos = new Label { Font = origen })
            {
                Tema.Normalizar(uno);
                Tema.Normalizar(dos);
                Assert.AreSame(uno.Font, dos.Font, "La fuente normalizada debe reutilizarse.");
                Assert.AreEqual(origen.Style, uno.Font.Style);
            }
            using (var contenido = new Panel { Width = 200 })
            {
                int pasadas = 0;
                Disposicion.RepetirUnaVez(contenido, () => { pasadas++; contenido.Width = contenido.Width == 200 ? 180 : 200; });
                Assert.AreEqual(2, pasadas, "La oscilación de ancho no debe producir una tercera pasada.");
            }
        }

        private static void VerificarVistaRealDelPiloto()
        {
            var cliente = Fake.Cliente(Entidades.NivelFidelidad.Bronce);
            var evento = Fake.EventoPublicado(1000m, 5);
            Fake.ComoCliente(cliente);
            try
            {
                using (var form = new FrmCompraCliente { IDUsuario = cliente.Id, IDEvento = evento.Id })
                {
                    form.TopMost = true; // Sólo la captura de prueba: evita ventanas ajenas sobre el piloto.
                    form.Show();
                    form.BringToFront();
                    form.Activate();
                    form.Refresh();
                    Application.DoEvents();
                    var titulo = (Label)form.Controls.Find("label1", true)[0];
                    Assert.IsTrue(titulo.Visible);
                    Assert.AreEqual(evento.Nombre, titulo.Text);
                    string carpeta = System.IO.Path.Combine(BaseDePrueba.RaizDelRepositorio(), "TestResults", "T41");
                    System.IO.Directory.CreateDirectory(carpeta);
                    using (var imagen = new Bitmap(form.Width, form.Height))
                    using (Graphics grafico = Graphics.FromImage(imagen))
                    {
                        grafico.CopyFromScreen(form.Location, Point.Empty, form.Size);
                        imagen.Save(System.IO.Path.Combine(carpeta, "piloto-pantalla-real.png"));
                    }
                }
            }
            finally { Negocio.GestorSesion.Instance.Cerrar(); Fake.Limpiar(); }
        }
    }
}
