using System;
using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    public static class Tema
    {
        public static readonly Color Primario = Color.FromArgb(116, 86, 174);
        public static readonly Color PrimarioEncima = Color.FromArgb(99, 70, 153);
        public static readonly Color PrimarioPresionado = Color.FromArgb(82, 58, 128);
        public static readonly Color PrimarioSuave = Color.FromArgb(239, 234, 248);
        public static readonly Color SecundarioPresionado = Color.FromArgb(226, 218, 242);
        public static readonly Color PeligroPresionado = Color.FromArgb(248, 212, 208);
        public static readonly Color Texto = Color.FromArgb(31, 31, 36);
        public static readonly Color TextoSecundario = Color.FromArgb(92, 95, 102);
        public static readonly Color Fondo = Color.FromArgb(246, 245, 250);
        public static readonly Color Superficie = Color.White;
        public static readonly Color EncabezadoGrilla = Color.FromArgb(241, 239, 247);
        public static readonly Color FilaAlterna = Color.FromArgb(250, 249, 253);
        public static readonly Color Borde = Color.FromArgb(220, 218, 227);
        public static readonly Color BordeControl = Color.FromArgb(142, 138, 156);
        public static readonly Color FondoDeshabilitado = Color.Gainsboro;
        public static readonly Color Error = Color.Firebrick;
        public static readonly Color ErrorFondo = Color.FromArgb(252, 232, 230);
        public static readonly Color Exito = Color.FromArgb(30, 123, 74);
        public static readonly Color ExitoFondo = Color.FromArgb(231, 244, 236);
        public static readonly Color Advertencia = Color.FromArgb(138, 90, 0);
        public static readonly Color AdvertenciaFondo = Color.FromArgb(255, 244, 214);

        public static readonly Font Marca = new Font("Segoe UI Semibold", 26f);
        public static readonly Font Titulo = new Font("Segoe UI Semibold", 18f);
        public static readonly Font Seccion = new Font("Segoe UI Semibold", 12f);
        public static readonly Font Cuerpo = new Font("Segoe UI", 10f);
        public static readonly Font Boton = new Font("Segoe UI Semibold", 10f);
        public static readonly Font Rotulo = new Font("Segoe UI Semibold", 9f);
        public static readonly Font Ayuda = new Font("Segoe UI", 9f);
        public static readonly Font Comprobante = new Font("Consolas", 10f);

        public static class Espacio
        {
            public const int XS = 4, S = 8, M = 12, L = 16, XL = 24, XXL = 32, XXXL = 48;
        }

        public const float DpiDiseno = 96f;
        public const int AltoBoton = 32, AltoBotonPrimario = 36, AnchoMinimoBoton = 120;
        public const int RellenoHorizontalBoton = 20, AltoFila = 32, AltoEncabezadoGrilla = 36;
        public const int AltoEncabezadoVentana = 72, AltoPie = 60, BordeNormal = 1, BordeFoco = 2;

        // Geometría del piloto a 96 DPI (ventana heredada de 1100 × 680 a 120 DPI).
        public static class Compra
        {
            public static readonly Size Ventana = new Size(880, 544);
            public static readonly Point Entradas = new Point(Espacio.XL, Espacio.XL);
            public static readonly Size TarjetaEntradas = new Size(400, 224);
            public static readonly Point Servicios = new Point(Espacio.XL, 272);
            public static readonly Size TarjetaServicios = new Size(400, 248);
            public static readonly Point Resumen = new Point(448, Espacio.XL);
            public static readonly Size TarjetaResumen = new Size(408, 496);
            public static readonly Point Titulo = new Point(Espacio.XL, Espacio.XS);
            public static readonly Size NombreEvento = new Size(832, 36);
            public static readonly Point Nivel = new Point(Espacio.XL, 44);
            public static readonly Point Enlace = new Point(320, 44);
            public static readonly Point TituloTarjeta = new Point(Espacio.L, Espacio.L);
            public static readonly Point Grilla = new Point(Espacio.L, 52);
            public static readonly Size Sectores = new Size(368, 156);
            public static readonly Point Agotado = new Point(Espacio.L, 52);
            public static readonly Size AvisoAgotado = new Size(368, 80);
            public static readonly Point ListaEspera = new Point(40, 152);
            public static readonly Size BotonEspera = new Size(320, AltoBoton);
            public static readonly Size BotonVolver = new Size(168, AltoBoton);
            public static readonly Size BotonComprar = new Size(160, AltoBotonPrimario);
            public static readonly Size Casilla = new Size(368, 24);
            public static readonly Point Hospedaje = new Point(Espacio.L, 52);
            public static readonly Point Traslado = new Point(Espacio.L, 84);
            public static readonly Point Merch = new Point(Espacio.L, 116);
            public static readonly Point Vip = new Point(Espacio.L, 148);
            public static readonly Point Seguro = new Point(Espacio.L, 192);
            public static readonly Size CasillaSeguro = new Size(368, 40);
        }

        // Medidas del ticket en puntos PDF; no dependen de los DPI del monitor.
        public static class Ticket
        {
            public const float Margen = 24, Franja = 28, LadoQr = 120;
            public const float Titulo = 16, Dato = 11, Rotulo = 8, Separacion = 24;
            public const float BaseQr = 64, BaseDatos = 72, BaseCodigo = 40, BaseAyuda = 24;
            public const float EspacioTitulo = 8, EspacioFila = 8;
            public const float PesoRotulo = 1, PesoDato = 3;
        }

        private static readonly Color TextoAnterior = Color.FromArgb(164, 165, 169);
        private static readonly Color TextoOscuroAnterior = Color.FromArgb(64, 64, 64);
        private static readonly Color BotonAnterior = Color.FromArgb(230, 231, 233);
        public static readonly Icon Icono = CargarIcono();

        private static Icon CargarIcono()
        {
            try
            {
                using (var recurso = typeof(Tema).Assembly.GetManifestResourceStream("Visual.Recursos.EventAura.ico"))
                {
                    if (recurso == null) return null;
                    using (var icono = new Icon(recurso)) return (Icon)icono.Clone();
                }
            }
            catch { return null; } // Un recurso ausente no debe impedir abrir el diseñador.
        }

        public static void EstiloGrilla(DataGridView g)
        {
            g.EnableHeadersVisualStyles = false;
            g.BackgroundColor = Superficie;
            g.BorderStyle = BorderStyle.None;
            g.GridColor = Borde;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            g.RowHeadersVisible = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = EncabezadoGrilla;
            g.ColumnHeadersDefaultCellStyle.ForeColor = TextoSecundario;
            g.ColumnHeadersDefaultCellStyle.Font = Rotulo;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = EncabezadoGrilla;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextoSecundario;
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            g.ColumnHeadersHeight = AltoEncabezadoGrilla;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            g.DefaultCellStyle.Font = Cuerpo;
            g.DefaultCellStyle.ForeColor = Texto;
            g.DefaultCellStyle.BackColor = Superficie;
            g.DefaultCellStyle.SelectionBackColor = PrimarioSuave;
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.AlternatingRowsDefaultCellStyle.BackColor = FilaAlterna;
            g.RowTemplate.Height = AltoFila;
            foreach (DataGridViewRow fila in g.Rows) fila.Height = AltoFila;
            g.StandardTab = true;
        }

        public static void Normalizar(Control raiz)
        {
            string familia = raiz.Font.Name;
            if (familia == "Nirmala UI" || familia == "MS Gothic" || familia == "Microsoft Sans Serif")
                raiz.Font = new Font(raiz.Font.Bold ? "Segoe UI Semibold" : "Segoe UI", raiz.Font.SizeInPoints);
            if (raiz.ForeColor == TextoAnterior) raiz.ForeColor = TextoSecundario;
            if (raiz.ForeColor == TextoOscuroAnterior) raiz.ForeColor = Texto;
            var boton = raiz as Button;
            if (boton != null && !(boton is Componentes.BotonPrimario) &&
                !(boton is Componentes.BotonSecundario) && !(boton is Componentes.BotonPeligro))
            {
                if (boton.BackColor == Primario) EstiloBoton(boton, true);
                else if (boton.BackColor == BotonAnterior) EstiloBoton(boton, false);
            }
            var grilla = raiz as DataGridView;
            if (grilla != null) EstiloGrilla(grilla);
            foreach (Control hijo in raiz.Controls) Normalizar(hijo);
        }

        // Los botones heredados conservan tamaño y posición: solo reciben estados visuales.
        private static void EstiloBoton(Button boton, bool primario)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.UseVisualStyleBackColor = false;
            boton.ForeColor = primario ? Superficie : Primario;
            boton.FlatAppearance.BorderSize = primario ? 0 : BordeNormal;
            boton.FlatAppearance.BorderColor = BordeControl;
            boton.FlatAppearance.MouseOverBackColor = primario ? PrimarioEncima : PrimarioSuave;
            boton.FlatAppearance.MouseDownBackColor = primario ? PrimarioPresionado : SecundarioPresionado;
            Color fondo = primario ? Primario : Superficie;
            boton.BackColor = boton.Enabled ? fondo : FondoDeshabilitado;
            boton.GotFocus += (s, e) => { boton.FlatAppearance.BorderSize = BordeFoco; boton.FlatAppearance.BorderColor = Primario; };
            boton.LostFocus += (s, e) => { boton.FlatAppearance.BorderSize = primario ? 0 : BordeNormal; boton.FlatAppearance.BorderColor = BordeControl; };
            boton.EnabledChanged += (s, e) => boton.BackColor = boton.Enabled ? fondo : FondoDeshabilitado;
        }
    }
}
