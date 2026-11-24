using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Visual.Componentes;
using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    public static class Tema
    {
        public static readonly Color Primario = Color.FromArgb(116, 86, 174);
        public static readonly Color LateralFondo = Color.FromArgb(36, 29, 48);
        public static readonly Color LateralTexto = Color.FromArgb(207, 201, 220);
        public static readonly Color LateralEncima = Color.FromArgb(50, 42, 66);
        public static readonly Color LateralSeccion = Color.FromArgb(139, 131, 158);
        public static readonly Color Foco = Primario;
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
        public static readonly Font FirmaNormal = new Font("Segoe UI", 16f);
        // 16/12/19 píxeles lógicos a 96 DPI, expresados en puntos para escalar con Windows.
        public static readonly Font TituloTarjeta = new Font("Segoe UI Semibold",12f);
        public static readonly Font DatoTarjeta = new Font("Segoe UI",9f);
        public static readonly Font PrecioTarjeta = new Font("Segoe UI Semibold",15f);
        public static readonly Color[] PaletaPortadas = { TextoSecundario,Color.FromArgb(62,71,87),Color.FromArgb(160,86,141),Color.FromArgb(72,117,145),Color.FromArgb(105,69,143),Color.FromArgb(83,132,126) };
        public static readonly Color MarcaAguaPortada=Color.FromArgb(71,255,255,255);
        public static Font FuenteInicialPortada(int alto){return new Font("Segoe UI",Math.Max(24,alto*0.75f),FontStyle.Bold,GraphicsUnit.Pixel);}
        public static readonly Font FirmaFuerte = new Font("Segoe UI", 16f, FontStyle.Bold);
        public static readonly Font IconosLateral = new Font(FamiliaIconos(), 12f);
        public static readonly Font IconosInterfaz = new Font(FamiliaIconos(), 24f);

        public static void EstiloAviso(Control aviso, bool error)
        {
            aviso.BackColor = error ? ErrorFondo : ExitoFondo;
            aviso.ForeColor = error ? Error : Texto;
            aviso.Font = Cuerpo;
        }

        private static string FamiliaIconos()
        {
            using (var fuentes = new System.Drawing.Text.InstalledFontCollection())
                foreach (var familia in fuentes.Families)
                    if (familia.Name == "Segoe Fluent Icons") return familia.Name;
            return "Segoe MDL2 Assets";
        }
        public static readonly Font Titulo = new Font("Segoe UI Semibold", 20f);
        public static readonly Font Seccion = new Font("Segoe UI Semibold", 10.5f);
        public static readonly Font Cuerpo = new Font("Segoe UI", 10f);
        public static readonly Font Boton = new Font("Segoe UI Semibold", 10f);
        public static readonly Font Rotulo = new Font("Segoe UI Semibold", 9f);
        public const float InterletradoSeccion = 0.1f;
        public static readonly Font Ayuda = new Font("Segoe UI", 9f);
        public static readonly Font Totales = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font IconosGrandes = new Font("Segoe MDL2 Assets", 32f);
        public static readonly Font IconosDocumentos = new Font("Segoe MDL2 Assets", 16f);
        public static readonly Font Comprobante = new Font("Consolas", 10f);

        internal static Dictionary<string, string> VariablesWeb()
        {
            string Css(Font fuente) => fuente.SizeInPoints.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                "pt '" + fuente.Name + "', sans-serif";
            var variables = new Dictionary<string, string>
            {
                ["primario"] = ColorTranslator.ToHtml(Primario),
                ["primario-suave"] = ColorTranslator.ToHtml(PrimarioSuave),
                ["superficie"] = ColorTranslator.ToHtml(Superficie),
                ["texto"] = ColorTranslator.ToHtml(Texto),
                ["texto-secundario"] = ColorTranslator.ToHtml(TextoSecundario),
                ["borde"] = ColorTranslator.ToHtml(Borde),
                ["exito"] = ColorTranslator.ToHtml(ExitoFondo),
                ["exito-texto"] = ColorTranslator.ToHtml(Exito),
                ["advertencia-texto"] = ColorTranslator.ToHtml(Advertencia),
                ["primario-encima"] = ColorTranslator.ToHtml(PrimarioEncima),
                ["advertencia"] = ColorTranslator.ToHtml(AdvertenciaFondo),
                ["agotado"] = ColorTranslator.ToHtml(FondoDeshabilitado),
                ["espacio-l"] = Espacio.L + "px",
                ["espacio-m"] = Espacio.M + "px",
                ["espacio-s"] = Espacio.S + "px",
                ["espacio-xs"] = Espacio.XS + "px",
                ["cuerpo"] = Css(Cuerpo),
                ["ayuda"] = Css(Ayuda),
                ["titulo-tarjeta"] = Css(TituloTarjeta),
                ["precio"] = Css(PrecioTarjeta)
            };
            for (int i = 0; i < PaletaPortadas.Length; i++)
                variables["grafico-" + i] = ColorTranslator.ToHtml(PaletaPortadas[i]);
            return variables;
        }

        public static class Espacio
        {
            public const int XS = 4, S = 8, M = 12, L = 16, XL = 24, XXL = 32;
        }

        public const float DpiDiseno = 96f;
        public const int AltoBoton = 36, AltoBotonPrimario = AltoBoton;
        public const int AnchoCampo = 320, AltoCampo = 32;
        public const int AnchoBotonCompacto = 160, AnchoBotonNormal = 184, AnchoBotonAmplio = 240;
        public const int MargenFormulario = Espacio.XXL, SeparacionControl = Espacio.S;
        public const int RellenoHorizontalBoton = Espacio.L, RellenoIconoBoton = Espacio.XXL + Espacio.L, AltoFila = 32, AltoEncabezadoGrilla = 36;
        public const int AltoEncabezadoVentana = 72, AltoBarraSuperior = 56, AltoPie = 60, BordeNormal = 1, BordeFoco = 2;

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
            g.DefaultCellStyle.SelectionBackColor = Primario;
            g.DefaultCellStyle.SelectionForeColor = Superficie;
            g.AlternatingRowsDefaultCellStyle.BackColor = FilaAlterna;
            g.RowTemplate.Height = AltoFila;
            foreach (DataGridViewRow fila in g.Rows) fila.Height = AltoFila;
            g.StandardTab = true;
            if (!grillasPreparadas.TryGetValue(g, out _))
            {
                grillasPreparadas.Add(g, new object());
                g.DataBindingComplete += (s, e) => AlinearNumeros(g);
            }
            AlinearNumeros(g);
        }

        private static void AlinearNumeros(DataGridView grilla)
        {
            foreach (DataGridViewColumn columna in grilla.Columns)
            {
                var tipo = columna.ValueType;
                if (tipo == null) continue;
                tipo = Nullable.GetUnderlyingType(tipo) ?? tipo;
                switch (Type.GetTypeCode(tipo))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        columna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        if (tipo == typeof(decimal) && string.IsNullOrEmpty(columna.DefaultCellStyle.Format))
                            columna.DefaultCellStyle.Format = "N2";
                        break;
                }
            }
        }

        internal static void EstiloTablaDeConsulta(DataGridView grilla)
        {
            EstiloGrilla(grilla);
            grilla.BorderStyle = BorderStyle.None;
            DibujarMarcoDeCampo(grilla);
            grilla.GridColor = BordeControl;
            grilla.RowTemplate.Height = Disposicion.Pixeles(grilla, AltoCampo);
            grilla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grilla.ColumnHeadersHeight = grilla.RowTemplate.Height;
            foreach (DataGridViewRow fila in grilla.Rows)
                fila.MinimumHeight = grilla.RowTemplate.Height;
        }

        internal static void EstiloSelector(Control selector)
        {
            selector.Font = Cuerpo;
            selector.ForeColor = Texto;
            int alto = Disposicion.Pixeles(selector, AltoCampo);
            selector.MinimumSize = new Size(0, alto);
            selector.Height = alto;
            if (selector is ComboBox combo)
                PrepararCombo(combo, alto);
            DibujarMarcoDeCampo(selector);
        }

        private static void PrepararCombo(ComboBox combo, int alto)
        {
            if (combo.DrawMode == DrawMode.OwnerDrawVariable) return;
            if (combo.DrawMode != DrawMode.OwnerDrawFixed)
            {
                combo.FlatStyle = FlatStyle.Flat;
                combo.DrawMode = DrawMode.OwnerDrawFixed;
                combo.DrawItem += (s, e) =>
                {
                    bool seleccionado = (e.State & DrawItemState.Selected) != 0;
                    using (var fondo = new SolidBrush(seleccionado ? Primario : Superficie))
                        e.Graphics.FillRectangle(fondo, e.Bounds);
                    if (e.Index < 0) return;
                    string texto = combo.GetItemText(combo.Items[e.Index]);
                    var area = e.Bounds;
                    area.Inflate(-Disposicion.Pixeles(combo, Espacio.XS), 0);
                    TextRenderer.DrawText(e.Graphics, texto, Cuerpo, area, seleccionado ? Superficie : Texto,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                };
            }
            combo.ItemHeight = Math.Max(1, alto);
            int bordeNativo = combo.Height - combo.ItemHeight;
            combo.ItemHeight = Math.Max(1, alto - bordeNativo);
        }

        private static void DibujarMarcoDeCampo(Control campo)
        {
            Control contenedor = campo.Parent;
            if (contenedor == null || marcosDeCampos.TryGetValue(campo, out _)) return;
            marcosDeCampos.Add(campo, new object());
            contenedor.Paint += (s, e) =>
            {
                if (!campo.Visible) return;
                var exterior = Rectangle.Inflate(campo.Bounds, 1, 1);
                ControlPaint.DrawBorder(e.Graphics, exterior, BordeControl, ButtonBorderStyle.Solid);
            };
            contenedor.Invalidate(Rectangle.Inflate(campo.Bounds, 1, 1));
        }

        private static readonly ConditionalWeakTable<Control, object> marcosDeCampos = new ConditionalWeakTable<Control, object>();
        private static readonly ConditionalWeakTable<DataGridView, object> grillasPreparadas = new ConditionalWeakTable<DataGridView, object>();
        private static readonly Dictionary<string, Font> fuentesNormalizadas = new Dictionary<string, Font>();
        private static readonly ConditionalWeakTable<Button, object> botonesPreparados = new ConditionalWeakTable<Button, object>();
        private static readonly ConditionalWeakTable<Form, object> barrasAjustadas = new ConditionalWeakTable<Form, object>();

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

        // Estados de negocio: se conservan al aplicar el tema de las grillas.
        public static void FilaNoReembolsable(DataGridViewRow fila)
        {
            fila.DefaultCellStyle.ForeColor = TextoSecundario;
        }

        public static void FilaTotal(DataGridViewRow fila)
        {
            fila.DefaultCellStyle.Font = Totales;
            fila.DefaultCellStyle.BackColor = EncabezadoGrilla;
        }

        public static Color ColorTemporizador(TimeSpan restante)
        {
            return restante.TotalMinutes < 1 ? Error : Texto;
        }

        public static void ComoSuperficie(Control control) { control.BackColor = Superficie; control.ForeColor = Texto; }
        public static void ComoFondo(Control control) { control.BackColor = Fondo; control.ForeColor = Texto; }
        public static void ComoDocumento(Label etiqueta) { etiqueta.Font = Comprobante; etiqueta.ForeColor = Texto; }
        public static void ComoEnlace(LinkLabel enlace)
        {
            enlace.Font = Ayuda;
            enlace.BackColor = Superficie;
            enlace.LinkColor = TextoSecundario;
            enlace.ActiveLinkColor = Texto;
            enlace.VisitedLinkColor = TextoSecundario;
            enlace.LinkBehavior = LinkBehavior.AlwaysUnderline;
        }

        public static void Normalizar(Control raiz)
        {
            if (raiz.Parent is FlowLayoutPanel || raiz.Parent is TableLayoutPanel)
            {
                int dpiContenedor = Math.Max(96, Disposicion.Dpi(raiz));
                int margen = MedidaDpi(SeparacionControl / 2, dpiContenedor);
                raiz.Margin = new Padding(margen);
            }
            var formulario = raiz as FormularioEventAura;
            if (formulario != null)
            {
                // La compra ya distribuye el margen de 24 px dentro de sus paneles desplazables.
                if (!(formulario is FrmCompraCliente))
                    formulario.Padding = new Padding(MedidaDpi(MargenFormulario, Math.Max(96, Disposicion.Dpi(formulario))));
                if (formulario.TopLevel && !formulario.Controls.OfType<Componentes.BarraSuperior>().Any())
                    AgregarBarraSuperior(formulario);
                if (formulario.Text.StartsWith("EventAura · ", StringComparison.Ordinal))
                    formulario.Text = formulario.Text.Substring("EventAura · ".Length);
                AjustarBarraSuperior(formulario);
            }
            string familia = raiz.Font.Name;
            if (familia == "Nirmala UI" || familia == "MS Gothic" || familia == "Microsoft Sans Serif")
                raiz.Font = NormalizarFuente(raiz.Font);
            if (raiz.ForeColor == TextoAnterior) raiz.ForeColor = TextoSecundario;
            if (raiz.ForeColor == TextoOscuroAnterior) raiz.ForeColor = Texto;
            var boton = raiz as Button;
            if (boton != null)
            {
                RolBoton rol = boton is Componentes.BotonPrimario ? RolBoton.Primario :
                    boton is Componentes.BotonPeligro ? RolBoton.Peligro : RolBoton.Secundario;
                if (!(boton is BotonTematico))
                    PrepararBoton(boton, rol);
                boton.Font = Boton;
                int dpi = Math.Max(96, Disposicion.Dpi(boton));
                bool tieneIcono = boton.Controls.OfType<Label>().Any(etiqueta =>
                    etiqueta.Name.StartsWith("icono", StringComparison.OrdinalIgnoreCase));
                int rellenoIzquierdo = tieneIcono ? RellenoIconoBoton : RellenoHorizontalBoton;
                boton.Padding = new Padding(MedidaDpi(rellenoIzquierdo, dpi), 0, MedidaDpi(RellenoHorizontalBoton, dpi), 0);
                bool lateral = boton.Parent?.Name == "navegacionLateral";
                boton.Width = MedidaDpi(lateral ? Disposicion.AnchoLateral : AnchoBotonSegunDiseno(boton), dpi);
                boton.Height = MedidaDpi(lateral ? Disposicion.AltoOpcionLateral : AltoBoton, dpi);
            }
            if (raiz is ComboBox lista && lista.DropDownStyle == ComboBoxStyle.DropDownList)
                PrepararCombo(lista, lista.Height);
            if (raiz is DateTimePicker fecha)
            {
                fecha.CalendarTitleBackColor = Primario;
                fecha.CalendarTitleForeColor = Superficie;
            }
            var grilla = raiz as DataGridView;
            if (grilla != null) EstiloGrilla(grilla);
            foreach (Control hijo in raiz.Controls) Normalizar(hijo);
            NormalizarSeparacionBotones(raiz);
        }

        private static void NormalizarSeparacionBotones(Control contenedor)
        {
            int separacion = MedidaDpi(SeparacionControl, Math.Max(96, Disposicion.Dpi(contenedor)));
            var botones = contenedor.Controls.Cast<Control>().OfType<Button>()
                .Where(b => b.Bounds.Width > 0 && b.Bounds.Height > 0)
                .OrderBy(b => b.Top).ThenBy(b => b.Left).ToArray();
            if (contenedor is FlowLayoutPanel flujo && flujo.FlowDirection == FlowDirection.LeftToRight && botones.Length > 1)
            {
                int ancho = botones.Max(b => b.Width);
                foreach (Button boton in botones) boton.Width = ancho;
                return;
            }

            foreach (var fila in botones.GroupBy(b => b.Top))
            {
                var grupo = fila.ToArray();
                if (grupo.Length < 2 || grupo.Zip(grupo.Skip(1), (a, b) => b.Left - a.Right)
                    .Any(hueco => hueco > Disposicion.Pixeles(contenedor, Espacio.XXL))) continue;
                int ancho = grupo.Max(b => b.Width);
                int total = ancho * grupo.Length + separacion * (grupo.Length - 1);
                var area = new Rectangle(grupo[0].Left, fila.Key, total, grupo[0].Height);
                if (area.Right > contenedor.ClientSize.Width - contenedor.Padding.Right) continue;
                if (contenedor.Controls.Cast<Control>().Any(c => !(c is Button) && c.Dock == DockStyle.None &&
                    c.Bounds.IntersectsWith(area) && (c is Label || c.TabStop))) continue;
                Disposicion.AlinearGrupo(grupo);
            }

            for (int i = 0; i < botones.Length; i++)
            {
                var actual = botones[i];
                for (int j = i + 1; j < botones.Length; j++)
                {
                    var siguiente = botones[j];
                    int solape = Math.Min(actual.Bottom, siguiente.Bottom) - Math.Max(actual.Top, siguiente.Top);
                    if (solape < Math.Min(actual.Height, siguiente.Height) / 2) continue;
                    int anchoNecesario = actual.Width + siguiente.Width + separacion + contenedor.Padding.Horizontal;
                    if (contenedor.ClientSize.Width > 0 && contenedor.ClientSize.Width < anchoNecesario) continue;
                    if (siguiente.Left < actual.Right + separacion)
                    {
                        int margen = Math.Max(contenedor.Padding.Left, MedidaDpi(Espacio.XL, Math.Max(96, Disposicion.Dpi(contenedor))));
                        if (siguiente.Anchor.HasFlag(AnchorStyles.Right) && siguiente.Left - actual.Width - separacion >= margen)
                            actual.Left = siguiente.Left - actual.Width - separacion;
                        else if (siguiente.Left <= actual.Left && actual.Anchor.HasFlag(AnchorStyles.Left))
                            siguiente.Left = actual.Right + separacion;
                        else
                            siguiente.Left = actual.Right + separacion;
                    }
                }
            }
        }

        private static int MedidaDpi(int logico, int dpi)
        {
            return (int)Math.Round(logico * (dpi / DpiDiseno), MidpointRounding.AwayFromZero);
        }

        private static void AgregarBarraSuperior(Form formulario)
        {
            int barra = MedidaDpi(AltoBarraSuperior, Disposicion.Dpi(formulario));
            var contenido = formulario.Controls.Cast<Control>().Where(c => c.Dock == DockStyle.None).ToArray();
            if (contenido.Length > 0)
            {
                int primerControl = contenido.Min(c => c.Top);
                int desplazamiento = Math.Max(0, barra + MedidaDpi(Espacio.S, Disposicion.Dpi(formulario)) - primerControl);
                if (desplazamiento > 0)
                {
                    formulario.SuspendLayout();
                    formulario.ClientSize = new Size(formulario.ClientSize.Width, formulario.ClientSize.Height + desplazamiento);
                    foreach (Control control in contenido) control.Top += desplazamiento;
                    formulario.ResumeLayout();
                }
            }
            formulario.Controls.Add(new Componentes.BarraSuperior { Name = "barraSuperior", Dock = DockStyle.Top });
        }

        private static void AjustarBarraSuperior(Form formulario)
        {
            foreach (var barra in formulario.Controls.OfType<Componentes.BarraSuperior>())
            {
                if (barra.Dock == DockStyle.Top)
                {
                    barra.Dock = DockStyle.None;
                    barra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    int alto = MedidaDpi(AltoBarraSuperior, Math.Max(96, Disposicion.Dpi(formulario)));
                    if (formulario.TopLevel && formulario.Controls.Cast<Control>().Any(c => c != barra && c.Dock == DockStyle.Fill))
                        formulario.Padding = new Padding(formulario.Padding.Left, alto, formulario.Padding.Right, formulario.Padding.Bottom);
                    barra.SetBounds(0, 0, formulario.ClientSize.Width, alto);
                    barra.BringToFront();
                    object marca;
                    if (!barrasAjustadas.TryGetValue(formulario, out marca))
                    {
                        barrasAjustadas.Add(formulario, new object());
                        formulario.SizeChanged += (s, e) =>
                        {
                            var actual = formulario.Controls.OfType<Componentes.BarraSuperior>().FirstOrDefault();
                            if (actual != null) actual.SetBounds(0, 0, formulario.ClientSize.Width, MedidaDpi(AltoBarraSuperior, Math.Max(96, Disposicion.Dpi(formulario))));
                        };
                    }
                }
            }
        }

        private static int AnchoBotonSegunDiseno(Button boton)
        {
            float aLogico = DpiDiseno / Disposicion.Dpi(boton);
            if (boton.FindForm() is frmInicio && boton.Name == "btninicio") return AnchoCampo;
            if (boton.Name == "btnDevolver") return AnchoBotonAmplio;
            if (boton.Name == "btnListaEspera")
                return Math.Max(AnchoBotonAmplio, (int)Math.Round((boton.Parent.ClientSize.Width - boton.Left * 2) * aLogico));
            bool tieneIcono = boton.Controls.OfType<Label>().Any(etiqueta => etiqueta.Name.StartsWith("icono", StringComparison.OrdinalIgnoreCase));
            int relleno = tieneIcono ? RellenoIconoBoton + RellenoHorizontalBoton : RellenoHorizontalBoton * 2;
            int requerido = (int)Math.Ceiling(TextRenderer.MeasureText(boton.Text ?? string.Empty, Boton, Size.Empty,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width * aLogico) + relleno + BordeFoco * 2 + 8;
            foreach (int ancho in new[] { AnchoBotonCompacto, AnchoBotonNormal, AnchoBotonAmplio })
                if (ancho >= requerido) return ancho;
            return AnchoBotonAmplio;
        }

        internal static void DefinirAccionPrincipal(Form formulario)
        {
            var candidatos = new List<BotonTematico>();
            void Buscar(Control contenedor)
            {
                if (contenedor.Name == "navegacionLateral") return;
                foreach (Control hijo in contenedor.Controls)
                {
                    if (hijo is BotonPrimario boton) candidatos.Add(boton);
                    Buscar(hijo);
                }
            }
            Buscar(formulario);
            var principal = formulario.AcceptButton as BotonTematico
                ?? candidatos.FirstOrDefault(b => b.Visible) ?? candidatos.FirstOrDefault();
            void Aplicar(Control contenedor)
            {
                if (contenedor.Name == "navegacionLateral") return;
                foreach (Control hijo in contenedor.Controls)
                {
                    if (hijo is BotonTematico boton)
                        boton.DefinirRolVisual(ReferenceEquals(principal, boton));
                    Aplicar(hijo);
                }
            }
            Aplicar(formulario);
        }

        // Los componentes y los botones heredados comparten todos los estados de estilo.
        public static void PrepararBoton(Button boton, RolBoton rol)
        {
            boton.Font = Boton;
            boton.Padding = new Padding(RellenoHorizontalBoton, 0, RellenoHorizontalBoton, 0);
            boton.Cursor = Cursors.Hand;
            EstadoBoton(boton, rol);
        }

        public static void EstadoBoton(Button boton, RolBoton rol, bool opcionActual = false)
        {
            bool lateral = boton.Parent?.Name == "navegacionLateral";
            bool primario = opcionActual || !lateral && rol == RolBoton.Primario;
            bool peligro = rol == RolBoton.Peligro;
            boton.FlatStyle = FlatStyle.Flat;
            boton.UseVisualStyleBackColor = false;
            boton.ForeColor = primario ? Color.White : lateral ? LateralTexto : peligro ? Error : Texto;
            boton.FlatAppearance.BorderSize = boton.Focused ? Disposicion.Pixeles(boton, BordeFoco)
                : primario || lateral ? 0 : Disposicion.Pixeles(boton, BordeNormal);
            boton.FlatAppearance.BorderColor = boton.Focused ? Foco : BordeControl;
            boton.FlatAppearance.MouseOverBackColor = lateral ? (primario ? Primario : LateralEncima)
                : primario ? PrimarioEncima : peligro ? ErrorFondo : Fondo;
            boton.FlatAppearance.MouseDownBackColor = lateral ? (primario ? PrimarioPresionado : ControlPaint.Dark(LateralEncima))
                : primario ? PrimarioPresionado : peligro ? PeligroPresionado : Borde;
            boton.BackColor = boton.Enabled ? (primario ? Primario : lateral ? LateralFondo : Superficie) : FondoDeshabilitado;
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
