using System;
using System.Windows.Forms;
using System.Drawing;

namespace Visual.Componentes
{
    public partial class DatoPanel : SuperficieTematica
    {
        private bool detalle;
        private Button accion;

        public DatoPanel()
        {
            InitializeComponent();
            valor.Font = Tema.PrecioTarjeta;
            valor.ForeColor = Tema.Primario;
            Paint += Disposicion.PintarTarjeta;
            SizeChanged += (s, e) => AjustarDetalle();
        }

        public void Cargar(string nombre, string dato)
        {
            titulo.Text = nombre;
            valor.Text = dato;
            AccessibleName = nombre + ": " + dato;
        }

        internal void CargarDetalle(string nombre, string dato, bool resaltar = false)
        {
            detalle = true;
            valor.Font = resaltar ? Tema.PrecioTarjeta : Tema.Cuerpo;
            valor.ForeColor = resaltar ? Tema.Primario : Tema.Texto;
            Cargar(nombre, dato);
            AjustarDetalle();
        }

        internal void ColocarAccion(Button boton)
        {
            accion = boton;
            Controls.Add(boton);
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
            valor.SetBounds(margen, titulo.Bottom + Px(Tema.Espacio.S), ancho, Alto(valor));
            if (accion != null)
                accion.SetBounds(margen, valor.Bottom + Px(Tema.Espacio.S),
                    Math.Min(ancho, Px(Tema.AnchoBotonNormal)), Px(Tema.AltoBoton));

            int alto = margen + (accion?.Bottom ?? valor.Bottom);
            if (Height != alto) Height = alto;
        }

        internal void AgregarAccion(string texto, Action accion)
        {
            var boton = new BotonSecundario { Text = texto, Width = Tema.AnchoBotonNormal, Height = Tema.AltoBoton };
            boton.Click += (s, e) => accion();
            ColocarAccion(boton);
        }
    }
}
