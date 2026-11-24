using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace Visual.Componentes
{
    public partial class DatoPanel : SuperficieTematica
    {
        private bool detalle;
        private Button accion;
        private Font fuenteValor;
        private Font fuenteAjustada;
        private int altoNatural;

        public DatoPanel()
        {
            InitializeComponent();
            valor.Font = Tema.PrecioTarjeta;
            valor.ForeColor = Tema.Texto;
            Paint += Disposicion.PintarTarjeta;
            ClientSizeChanged += (s, e) => AjustarDetalle();
            Disposed += (s, e) => fuenteAjustada?.Dispose();
        }

        public void Cargar(string nombre, string dato)
        {
            titulo.Text = nombre;
            valor.Text = dato;
            AccessibleName = nombre + ": " + dato;
        }

        internal void CargarDetalle(string nombre, string dato)
        {
            detalle = true;
            fuenteValor = Tema.PrecioTarjeta;
            valor.ForeColor = Tema.Texto;
            Cargar(nombre, dato);
            AjustarDetalle();
        }

        internal void ColocarAccion(Button boton)
        {
            accion = boton;
            Controls.Add(boton);
            Disposicion.ActivarTarjeta(this, boton);
            AjustarDetalle();
        }

        private void AjustarDetalle()
        {
            if (!detalle) return;
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int margen = Px(Tema.Espacio.L);
            int ancho = Math.Max(1, ClientSize.Width - margen * 2);
            int Alto(Label etiqueta) => TextRenderer.MeasureText(etiqueta.Text, etiqueta.Font,
                new Size(ancho, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix).Height;
            titulo.SetBounds(margen, margen, ancho, Alto(titulo));
            const TextFormatFlags linea = TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;
            using (var superficie = CreateGraphics())
            {
                float puntos = fuenteValor.SizeInPoints;
                int requerido = valor.Text.Split('\n').Max(texto =>
                    TextRenderer.MeasureText(superficie, texto, fuenteValor, Size.Empty, linea).Width);
                if (requerido > ancho)
                    puntos = Math.Max(8f, (float)Math.Floor(puntos * ancho / requerido * 2) / 2);
                if (valor.Font.SizeInPoints != puntos || valor.Font.Name != fuenteValor.Name)
                {
                    var anterior = fuenteAjustada;
                    fuenteAjustada = puntos == fuenteValor.SizeInPoints ? null : new Font(fuenteValor.FontFamily, puntos, fuenteValor.Style);
                    valor.Font = fuenteAjustada ?? fuenteValor;
                    anterior?.Dispose();
                }
                int altoLinea = TextRenderer.MeasureText(superficie, "Mg", valor.Font, Size.Empty, linea).Height;
                valor.SetBounds(margen, titulo.Bottom + Px(Tema.Espacio.S), ancho,
                    altoLinea * valor.Text.Split('\n').Length);
                valor.AutoEllipsis = true;
            }
            if (accion != null)
                accion.SetBounds(margen, valor.Bottom + Px(Tema.Espacio.S),
                    Math.Min(ancho, Px(Tema.AnchoBotonNormal)), Px(Tema.AltoBoton));

            int alto = margen + (accion?.Bottom ?? valor.Bottom);
            altoNatural = alto;
        }

        internal static int DistribuirGrilla(FlowLayoutPanel grilla, int anchoDisponible)
        {
            int ancho = Disposicion.Pixeles(grilla, 208);
            int espacio = Disposicion.Pixeles(grilla, Tema.Espacio.L);
            var tarjetas = grilla.Controls.OfType<DatoPanel>().ToArray();
            int maxColumnas = Math.Min(3, tarjetas.Length);
            int columnas = Math.Max(1, Math.Min(maxColumnas, (anchoDisponible + espacio) / (ancho + espacio)));
            grilla.SuspendLayout();
            grilla.FlowDirection = FlowDirection.LeftToRight;
            grilla.WrapContents = true;
            grilla.Width = columnas * (ancho + espacio);
            foreach (DatoPanel tarjeta in tarjetas)
            {
                tarjeta.Margin = new Padding(0, 0, espacio, espacio);
                tarjeta.Width = ancho;
                tarjeta.AjustarDetalle();
            }
            int alto = 0;
            for (int i = 0; i < tarjetas.Length; i += columnas)
            {
                var fila = tarjetas.Skip(i).Take(columnas).ToArray();
                int mayor = fila.Max(t => t.altoNatural);
                foreach (DatoPanel tarjeta in fila) tarjeta.Height = mayor;
                alto += mayor + espacio;
            }
            grilla.Height = alto;
            grilla.ResumeLayout(true);
            return alto;
        }

        internal void AgregarAccion(string texto, Action accion)
        {
            var boton = new BotonSecundario { Text = texto, Width = Tema.AnchoBotonNormal, Height = Tema.AltoBoton };
            boton.Click += (s, e) => accion();
            ColocarAccion(boton);
        }
    }
}
