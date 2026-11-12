using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Visual.Componentes;
using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    public static class Tema
    {
        public static readonly Color Foco = Color.Black;
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

        private static readonly Dictionary<string, Font> fuentesNormalizadas = new Dictionary<string, Font>();
        private static readonly ConditionalWeakTable<Button, object> botonesPreparados = new ConditionalWeakTable<Button, object>();

        private static Font NormalizarFuente(Font fuente)
        {
            string familia = fuente.Bold ? "Segoe UI Semibold" : "Segoe UI";
            string clave = familia + "|" + fuente.Size.ToString(System.Globalization.CultureInfo.InvariantCulture) + "|" +
                fuente.Style + "|" + fuente.Unit + "|" + fuente.GdiCharSet + "|" + fuente.GdiVerticalFont;
            lock (fuentesNormalizadas)
            {
                Font resultado;
                if (!fuentesNormalizadas.TryGetValue(clave, out resultado))
                {
                    resultado = new Font(familia, fuente.Size, fuente.Style, fuente.Unit, fuente.GdiCharSet, fuente.GdiVerticalFont);
                    fuentesNormalizadas.Add(clave, resultado);
                }
                return resultado;
            }
        }

        public static void ComoSuperficie(Control control) { control.BackColor = Superficie; control.ForeColor = Texto; }
        public static void ComoFondo(Control control) { control.BackColor = Fondo; control.ForeColor = Texto; }
        public static void ComoDocumento(Label etiqueta) { etiqueta.Font = Comprobante; etiqueta.ForeColor = Texto; }
        public static void ComoEnlace(LinkLabel enlace)
        {
            enlace.Font = Ayuda;
            enlace.BackColor = Superficie;
            enlace.LinkColor = Primario;
            enlace.ActiveLinkColor = PrimarioPresionado;
            enlace.VisitedLinkColor = Primario;
            enlace.LinkBehavior = LinkBehavior.HoverUnderline;
        }

        public static void Normalizar(Control raiz)
        {
            string familia = raiz.Font.Name;
            if (familia == "Nirmala UI" || familia == "MS Gothic" || familia == "Microsoft Sans Serif")
                raiz.Font = NormalizarFuente(raiz.Font);
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

        // Los componentes y los botones heredados comparten todos los estados de estilo.
        public static void PrepararBoton(Button boton, RolBoton rol)
        {
            boton.Font = Boton;
            boton.MinimumSize = new Size(AnchoMinimoBoton, rol == RolBoton.Primario ? AltoBotonPrimario : AltoBoton);
            boton.Padding = new Padding(RellenoHorizontalBoton, 0, RellenoHorizontalBoton, 0);
            boton.Cursor = Cursors.Hand;
            EstadoBoton(boton, rol);
        }

        public static void EstadoBoton(Button boton, RolBoton rol)
        {
            bool primario = rol == RolBoton.Primario, peligro = rol == RolBoton.Peligro;
            boton.FlatStyle = FlatStyle.Flat;
            boton.UseVisualStyleBackColor = false;
            boton.ForeColor = primario ? Superficie : peligro ? Error : Primario;
            boton.FlatAppearance.BorderSize = boton.Focused ? BordeFoco : primario ? 0 : BordeNormal;
            boton.FlatAppearance.BorderColor = boton.Focused ? (primario ? Foco : Primario) : BordeControl;
            boton.FlatAppearance.MouseOverBackColor = primario ? PrimarioEncima : peligro ? ErrorFondo : PrimarioSuave;
            boton.FlatAppearance.MouseDownBackColor = primario ? PrimarioPresionado : peligro ? PeligroPresionado : SecundarioPresionado;
            boton.BackColor = boton.Enabled ? (primario ? Primario : Superficie) : FondoDeshabilitado;
        }

        private static void EstiloBoton(Button boton, bool primario)
        {
            RolBoton rol = primario ? RolBoton.Primario : RolBoton.Secundario;
            EstadoBoton(boton, rol);
            object marca;
            if (botonesPreparados.TryGetValue(boton, out marca)) return;
            botonesPreparados.Add(boton, new object());
            boton.GotFocus += (s, e) => EstadoBoton(boton, rol);
            boton.LostFocus += (s, e) => EstadoBoton(boton, rol);
            boton.EnabledChanged += (s, e) => EstadoBoton(boton, rol);
        }
    }
}
