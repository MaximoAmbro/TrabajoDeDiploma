using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Entidades;

namespace Visual.Componentes
{
    // Esquema orientativo, no plano de butacas: no inventa geometría del local.
    // La fuente y los colores provienen del tema; sus medidas las da el contenedor.
    public class MapaSectores : EtiquetaAyuda
    {
        private readonly ToolTip ayuda = new ToolTip();
        private readonly List<Sector> sectores = new List<Sector>();
        private readonly Dictionary<int, int> disponibles = new Dictionary<int, int>();
        private readonly Dictionary<int, int> cantidades = new Dictionary<int, int>();
        private int seleccionado = -1;
        private int sobre = -1;
        public event Action<int> SectorElegido;
        public event Action<int, int> CantidadSolicitada;
        internal bool Compacto { get; set; }

        private int Columnas => Compacto ? Math.Min(3, Math.Max(1, sectores.Count)) : sectores.Count <= 3 ? 1 : 2;

        public MapaSectores()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable, true);
            TabStop = true;
            AccessibleName = "Mapa orientativo de sectores";
            AccessibleDescription = "Ordenados por precio, del escenario hacia atrás. Usá las flechas y Enter para elegir; también podés usar la tabla de cantidades.";
        }

        public void Cargar(IEnumerable<Sector> origen, Func<int, int> cupo, Func<int, int> cantidad = null)
        {
            sectores.Clear();
            sectores.AddRange(origen.OrderByDescending(s => s.Precio).ThenBy(s => s.Id));
            disponibles.Clear();
            cantidades.Clear();
            foreach (var sector in sectores)
            {
                disponibles[sector.Id] = Math.Max(0, cupo(sector.Id));
                cantidades[sector.Id] = Math.Max(0, cantidad == null ? 0 : cantidad(sector.Id));
            }
            if (!sectores.Any(s => s.Id == seleccionado)) seleccionado = -1;
            Invalidate();
        }

        public Rectangle Zona(int indice)
        {
            int columnas = Columnas;
            int filas = Math.Max(1, (sectores.Count + columnas - 1) / columnas);
            int espacio = Math.Max(4, Font.Height / 3);
            int escenario = Font.Height * 2;
            int ancho = Math.Max(1, (ClientSize.Width - espacio * (columnas + 1)) / columnas);
            int alto = Math.Max(1, (ClientSize.Height - escenario - espacio * (filas + 1)) / filas);
            return new Rectangle(espacio + (indice % columnas) * (ancho + espacio),
                escenario + espacio + (indice / columnas) * (alto + espacio), ancho, alto);
        }

        public int AltoParaTodosLosSectores
        {
            get { int columnas = Columnas; return Font.Height * (4 + 5 * Math.Max(1, (sectores.Count + columnas - 1) / columnas)); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Tema.Superficie);
            using (var escenario = new SolidBrush(Tema.Texto))
                e.Graphics.FillRectangle(escenario, new Rectangle(0, 0, Width, Font.Height * 2));
            TextRenderer.DrawText(e.Graphics, "ESCENARIO · mapa orientativo", Font,
                new Rectangle(0, 0, Width, Font.Height * 2), Tema.Superficie,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            if (sectores.Count == 0)
                TextRenderer.DrawText(e.Graphics, "Todavía no hay sectores para este evento.", Font,
                    ClientRectangle, Tema.TextoSecundario, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            for (int i = 0; i < sectores.Count; i++)
            {
                var s = sectores[i]; int quedan = disponibles[s.Id];
                bool pocos = quedan > 0 && quedan <= Math.Max(1, s.Capacidad / 10);
                var rect = Zona(i);
                var fondo = quedan == 0 ? Tema.FondoDeshabilitado : pocos ? Tema.AdvertenciaFondo : Tema.ExitoFondo;
                using (var pincel = new SolidBrush(fondo)) e.Graphics.FillRectangle(pincel, rect);
                if (quedan == 0)
                    using (var patron = new HatchBrush(HatchStyle.LightDownwardDiagonal, Tema.Borde, fondo))
                        e.Graphics.FillRectangle(patron, rect);
                using (var borde = new Pen(s.Id == seleccionado ? Tema.Texto : Tema.BordeControl,
                    s.Id == seleccionado ? Tema.BordeFoco : Tema.BordeNormal)) e.Graphics.DrawRectangle(borde, rect);
                string estado = quedan == 0 ? "AGOTADO" : pocos ? "POCAS · " + quedan + " disponibles" : quedan + " disponibles";
                var contenido = Rectangle.Inflate(rect, -4, -2);
                contenido.Height = Math.Max(1, contenido.Height - Font.Height - 4);
                TextRenderer.DrawText(e.Graphics, s.Nombre + " · " + s.Precio.ToString("C0", CultureInfo.GetCultureInfo("es-AR")) +
                    Environment.NewLine + estado, Font, contenido, Tema.Texto,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
                if (Compacto)
                {
                    Rectangle control = ZonaCantidad(rect);
                    using (var fondoControl = new SolidBrush(Tema.Fondo)) e.Graphics.FillRectangle(fondoControl, control);
                    using (var bordeControl = new Pen(Tema.BordeControl)) e.Graphics.DrawRectangle(bordeControl, control);
                    Rectangle menos = new Rectangle(control.Left, control.Top, control.Height, control.Height);
                    Rectangle mas = new Rectangle(control.Right - control.Height, control.Top, control.Height, control.Height);
                    TextRenderer.DrawText(e.Graphics, "−", Font, menos, Tema.Texto, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    TextRenderer.DrawText(e.Graphics, cantidades[s.Id].ToString(CultureInfo.InvariantCulture), Font,
                        Rectangle.FromLTRB(menos.Right, control.Top, mas.Left, control.Bottom), Tema.Texto,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    TextRenderer.DrawText(e.Graphics, "+", Font, mas, Tema.Texto, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
            if (Focused) ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(ClientRectangle, -1, -1));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int indice = -1;
            for (int i = 0; i < sectores.Count; i++) if (Zona(i).Contains(e.Location)) { indice = i; break; }
            if (indice == sobre) return;
            sobre = indice;
            ayuda.SetToolTip(this, indice < 0 ? "Esquema calculado; no representa ubicaciones numeradas." :
                sectores[indice].Nombre + " · " + sectores[indice].Precio.ToString("C2", CultureInfo.GetCultureInfo("es-AR")) +
                " · " + disponibles[sectores[indice].Id] + " disponibles");
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e); Focus();
            for (int i = 0; i < sectores.Count; i++)
            {
                Rectangle zona = Zona(i);
                if (!zona.Contains(e.Location)) continue;
                Sector sector = sectores[i];
                if (Compacto && ZonaCantidad(zona).Contains(e.Location))
                {
                    Rectangle control = ZonaCantidad(zona);
                    bool sumar = e.X >= control.Right - control.Height;
                    int siguiente = cantidades[sector.Id] + (sumar ? 1 : -1);
                    if (siguiente >= 0 && disponibles[sector.Id] > 0)
                    {
                        SectorElegido?.Invoke(sector.Id);
                        CantidadSolicitada?.Invoke(sector.Id, siguiente);
                    }
                    return;
                }
                Elegir(i);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            int indice = sectores.FindIndex(s => s.Id == seleccionado);
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) indice = Math.Min(sectores.Count - 1, indice + 1);
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left) indice = Math.Max(0, indice - 1);
            else if (e.KeyCode != Keys.Enter && e.KeyCode != Keys.Space) return;
            if (indice >= 0 && indice < sectores.Count) Elegir(indice);
            e.Handled = true;
        }
        private void Elegir(int indice)
        {
            var s = sectores[indice];
            if (disponibles[s.Id] == 0) return;
            seleccionado = s.Id; Invalidate(); SectorElegido?.Invoke(s.Id);
        }

        private Rectangle ZonaCantidad(Rectangle zona)
        {
            int alto = Math.Min(Math.Max(20, Font.Height + 6), Math.Max(1, zona.Height - 4));
            return new Rectangle(zona.Left + 3, zona.Bottom - alto - 3, Math.Max(1, zona.Width - 6), alto);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) ayuda.Dispose();
            base.Dispose(disposing);
        }
    }
}
