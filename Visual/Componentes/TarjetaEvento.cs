using System;
using System.Globalization;
using System.Linq;
using Entidades;
using Negocio;
using System.Drawing;
using System.Windows.Forms;
namespace Visual.Componentes
{
    public partial class TarjetaEvento : SuperficieTematica
    {
        public event Action<Evento> VerEntradas;
        private Evento evento;
        private readonly EtiquetaAyuda disponibilidad = new EtiquetaAyuda { Name = "disponibilidadEvento" };
        public TarjetaEvento()
        {
            InitializeComponent();
            titulo.Font = Tema.TituloTarjeta;
            fecha.Font = lugar.Font = Tema.DatoTarjeta;
            foreach (Label etiqueta in new Label[] { titulo, fecha, lugar, precio })
                etiqueta.UseMnemonic = false;
            Controls.Add(disponibilidad);
            precio.Font=Tema.PrecioTarjeta;precio.ForeColor=Tema.Texto;
            accion.Click+=(s,e)=>VerEntradas?.Invoke(evento);
            Paint+=Disposicion.PintarTarjeta;
            Disposicion.ActivarTarjeta(this, accion, disponibilidad);
        }
        public void Cargar(Evento dato,Local local,System.Collections.Generic.IEnumerable<Sector> sectores)
        {
            evento=dato;portada.Cargar(dato.Nombre,dato.Categoria,dato.ImagenPortada);
            titulo.Text=dato.Nombre;fecha.Text=dato.Fecha.ToString("ddd d MMM · HH:mm",CultureInfo.GetCultureInfo("es-AR"));
            lugar.Text=local==null?"Consultá los datos del evento":local.Nombre+" · "+local.Direccion;
            var opciones = sectores.Select(sector => new
            {
                Sector = sector,
                Cupo = GestorPaquetes.Instance.DisponiblesDe(sector.Id)
            }).ToArray();
            var disponibles = opciones.Where(opcion => opcion.Cupo > 0).ToArray();
            var precios = (disponibles.Length == 0 ? opciones : disponibles).Select(opcion => opcion.Sector.Precio).ToArray();
            precio.Text = precios.Length == 0 ? "Próximamente" :
                "Desde " + precios.Min().ToString("C0", CultureInfo.GetCultureInfo("es-AR"));
            bool agotado = disponibles.Length == 0;
            bool poco = !agotado && disponibles.All(opcion => opcion.Cupo <= Math.Max(1, opcion.Sector.Capacidad / 10));
            disponibilidad.Text = opciones.Length == 0 ? "Sin sectores" : agotado ? "Agotado" : poco ? "Pocas entradas" : "Disponible";
            disponibilidad.BackColor = agotado ? Tema.FondoDeshabilitado : poco ? Tema.AdvertenciaFondo : Tema.ExitoFondo;
            disponibilidad.ForeColor = Tema.Texto;
            AccessibleName=dato.Nombre;
        }
        internal object ModeloWeb()
        {
            int tono = PortadaEvento.ColorDelNombre(evento.Nombre);
            return new
            {
                id = evento.Id,
                nombre = titulo.Text,
                fecha = fecha.Text,
                lugar = lugar.Text,
                precio = precio.Text,
                disponibilidad = disponibilidad.Text,
                estado = disponibilidad.BackColor == Tema.ExitoFondo ? "disponible" :
                    disponibilidad.BackColor == Tema.AdvertenciaFondo ? "escaso" : "agotado",
                color = ColorTranslator.ToHtml(Tema.PaletaPortadas[tono]),
                colorFinal = ColorTranslator.ToHtml(Tema.PaletaPortadas[(tono + 2) % Tema.PaletaPortadas.Length]),
                imagen = evento.ImagenPortada == null ? null : Convert.ToBase64String(evento.ImagenPortada)
            };
        }

        private void Distribuir(int ancho, int altoPortada)
        {
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int margen = Px(Tema.Espacio.L);
            int separacion = Px(Tema.Espacio.XS);
            int interior = Math.Max(1, ancho - margen * 2);
            Width = ancho;
            portada.SetBounds(0, 0, ancho, Px(altoPortada));
            int y = portada.Bottom + margen;
            foreach (Label etiqueta in new Label[] { titulo, fecha, lugar, precio })
            {
                int alto = etiqueta.GetPreferredSize(new Size(interior, 0)).Height;
                etiqueta.SetBounds(margen, y, interior, alto);
                y = etiqueta.Bottom + separacion;
            }
            int rellenoEtiqueta = Px(Tema.Espacio.S);
            disponibilidad.Padding = new Padding(rellenoEtiqueta, 0, rellenoEtiqueta, 0);
            var medidaEtiqueta = disponibilidad.GetPreferredSize(new Size(interior, 0));
            disponibilidad.SetBounds(margen, y, Math.Min(interior, medidaEtiqueta.Width), medidaEtiqueta.Height + separacion);
            accion.SetBounds(margen, disponibilidad.Bottom + Px(Tema.Espacio.S), interior, Px(Tema.AltoBoton));
            Height = accion.Bottom + rellenoEtiqueta;
        }

        internal static int DistribuirGrilla(FlowLayoutPanel grilla, int anchoDisponible, int altoPortada = 96)
        {
            var tarjetas = grilla.Controls.OfType<TarjetaEvento>().ToArray();
            if (tarjetas.Length == 0) return 0;
            int espacio = Disposicion.Pixeles(grilla, Tema.Espacio.S);
            int ancho = Math.Max(1, (anchoDisponible - grilla.Padding.Horizontal - 2 * espacio) / 3);
            grilla.SuspendLayout();
            try
            {
                for (int i = 0; i < tarjetas.Length; i++)
                {
                    tarjetas[i].Margin = new Padding(0, 0, i % 3 == 2 ? 0 : espacio, espacio);
                    tarjetas[i].Distribuir(ancho, altoPortada);
                }
                int alto = tarjetas.Max(tarjeta => tarjeta.Height);
                foreach (TarjetaEvento tarjeta in tarjetas) tarjeta.IgualarAlto(alto);
                return ((tarjetas.Length + 2) / 3) * (alto + espacio);
            }
            finally { grilla.ResumeLayout(true); }
        }

        private void IgualarAlto(int alto)
        {
            accion.Top += alto - Height;
            Height = alto;
        }
    }
}
