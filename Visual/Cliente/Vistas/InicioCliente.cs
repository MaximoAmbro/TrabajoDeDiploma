using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Entidades;
using System.Windows.Forms;
using Negocio;
namespace Visual.Componentes
{
    public partial class InicioCliente : SuperficieTematica
    {
        public event Action VerEventos;
        public event Action VerEntradas;
        public event Action<Evento> VerEvento;
        internal const int AnchoTarjetaResumen = 264;
        private readonly EtiquetaEncabezado tituloCatalogo = new EtiquetaEncabezado { Text = "Eventos disponibles" };
        private readonly FlowLayoutPanel catalogo = new FlowLayoutPanel { Name = "eventosInicioCliente" };
        private readonly EstadoVacio sinEventos = new EstadoVacio { Name = "sinEventosInicioCliente" };
        private bool distribuyendo;
        private bool tieneEntradas;
        private readonly FlowLayoutPanel tarjetas;
        private readonly DatoPanel tarjetaNivel;
        private readonly DatoPanel tarjetaEntradas;
        private readonly DatoPanel tarjetaEspera;

        public InicioCliente()
        {
            InitializeComponent();
            verEventos.Click += (s, e) =>
            {
                if (tieneEntradas) VerEntradas?.Invoke();
                else VerEventos?.Invoke();
            };
            tarjetas = new FlowLayoutPanel
            {
                Name = "tarjetasInicioCliente",
                FlowDirection = FlowDirection.LeftToRight
            };
            tarjetaNivel = CrearTarjeta("fidelidadCliente");
            tarjetaEntradas = CrearTarjeta("entradasCliente");
            tarjetaEspera = CrearTarjeta("esperaCliente");
            Controls.AddRange(new Control[] { tarjetas, tituloCatalogo, catalogo, sinEventos });
            sinEventos.Cargar("Por ahora no hay eventos con entradas disponibles.", "Ver el catálogo");
            sinEventos.Accion += () => VerEventos?.Invoke();
            sinEventos.Hide();
            tituloCatalogo.Hide();
            tarjetaEntradas.ColocarAccion(verEventos);
            tarjetaEspera.AgregarAccion("Explorar eventos", () => VerEventos?.Invoke());
            SizeChanged += (s, e) => DistribuirTarjetas();
            foreach (Control etiqueta in new Control[] { nivel, siguiente, tituloProximas, icono, proximas, tituloAvisos, avisos, ultimo })
                etiqueta.Hide();
        }

        private void DistribuirTarjetas()
        {
            if (distribuyendo) return;
            distribuyendo = true;
            SuspendLayout();
            try
            {
                int ancho = Math.Max(1, ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
                int espacio = Disposicion.Pixeles(this, Tema.Espacio.M);
                int altoTitulo = Disposicion.Pixeles(this, Tema.Espacio.XXL);
                int arriba = AutoScrollPosition.Y;
                DatoPanel.DistribuirGrilla(tarjetas, ancho, AnchoTarjetaResumen);
                tarjetas.Top = arriba;
                tituloCatalogo.SetBounds(0, tarjetas.Bottom + espacio, ancho, altoTitulo);
                int inicioCatalogo = tituloCatalogo.Bottom + Disposicion.Pixeles(this, Tema.Espacio.S);
                catalogo.SetBounds(0, inicioCatalogo, ancho, TarjetaEvento.DistribuirGrilla(catalogo, ancho, altoPortada: 64));
                sinEventos.AjustarA(new Rectangle(0, inicioCatalogo, ancho, Disposicion.Pixeles(this, 144)));
            }
            finally
            {
                ResumeLayout(true);
                distribuyendo = false;
            }
        }

        private void CargarCatalogo()
        {
            foreach (Control anterior in catalogo.Controls.Cast<Control>().ToArray()) anterior.Dispose();
            foreach (Evento evento in GestorEventos.Instance.Vigentes().Where(evento => !GestorEventos.Instance.EstaAgotado(evento.Id)))
            {
                var tarjeta = new TarjetaEvento();
                tarjeta.Cargar(evento, GestorLocales.Instance.PorId(evento.IdLocal), GestorEventos.Instance.Sectores(evento.Id));
                tarjeta.VerEntradas += elegido => VerEvento?.Invoke(elegido);
                catalogo.Controls.Add(tarjeta);
            }
            tituloCatalogo.Show();
            sinEventos.Visible = catalogo.Controls.Count == 0;
        }

        private DatoPanel CrearTarjeta(string nombre)
        {
            var tarjeta = new DatoPanel { Name = nombre };
            tarjetas.Controls.Add(tarjeta);
            return tarjeta;
        }
        public void Cargar(int id)
        {
            if(id<=0 || !GestorUsuarios.Instance.ExisteUsuarioDeSesion()) {tarjetaEntradas.CargarDetalle("Próximas entradas", "Ingresá para ver tus próximas entradas."); return;}
            var cliente=GestorFidelizacion.Instance.ObtenerCliente(id);
            GestorFidelizacion.Instance.ReevaluarNivel(cliente,DateTime.Now);
            nivel.Text=cliente.Nivel+" · "+GestorFidelizacion.Instance.PorcentajePara(cliente.Nivel).ToString("P1",System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
            int compras=GestorFidelizacion.Instance.ComprasEnVentana(id,DateTime.Now);
            int umbral=GestorParametros.Instance.ObtenerEntero(cliente.Nivel==NivelFidelidad.Bronce?"FIDELIDAD_UMBRAL_PLATA":"FIDELIDAD_UMBRAL_ORO");
            siguiente.Text=cliente.Nivel==NivelFidelidad.Oro?"Nivel máximo":
                Math.Max(0,umbral-compras)+" compras para "+(cliente.Nivel==NivelFidelidad.Bronce?"Plata":"Oro");
            var agenda = new List<(int Id, DateTime Fecha, string Nombre, int Cantidad)>();
            foreach(var compra in GestorCompras.Instance.HistorialDe(id).Where(c=>c.Estado==EstadoCompra.Confirmada)) {
                foreach(var grupo in GestorCompras.Instance.EntradasDe(compra).Where(e=>e.Estado==EstadoEntrada.Confirmada).GroupBy(e=>e.IdEvento)) {
                    var ev=GestorEventos.Instance.ObtenerPorId(grupo.Key);
                    if (ev != null && ev.Fecha >= DateTime.Now) agenda.Add((ev.Id, ev.Fecha, ev.Nombre, grupo.Sum(e => e.CantidadVendida)));
                }
            }
            var entradas = agenda.GroupBy(e => new { e.Id, e.Fecha, e.Nombre }).OrderBy(g => g.Key.Fecha)
                .ToArray();
            var espera = GestorListaEspera.Instance.EventosEnEspera(id);
            tieneEntradas = agenda.Count > 0;
            verEventos.Text = tieneEntradas ? "Ver mis entradas" : "Ver eventos";
            tarjetaNivel.CargarDetalle("Fidelidad · " + siguiente.Text, nivel.Text + " de descuento");
            if (entradas.Length == 1)
            {
                var evento = entradas[0];
                int cantidad = evento.Sum(e => e.Cantidad);
                tarjetaEntradas.CargarDetalle("Próximas · " + evento.Key.Nombre,
                    $"{evento.Key.Fecha:dd/MM HH:mm} · {cantidad} {(cantidad == 1 ? "entrada" : "entradas")}");
            }
            else
                tarjetaEntradas.CargarDetalle("Próximas entradas", entradas.Length == 0
                    ? "Sin entradas próximas" : $"{agenda.Sum(e => e.Cantidad)} entradas · {entradas.Length} eventos");
            string tituloEspera = espera.Count == 0 ? "Lista de espera" : "Lista de espera" + Environment.NewLine + string.Join(Environment.NewLine, espera);
            tarjetaEspera.CargarDetalle(tituloEspera, espera.Count == 0
                ? "Sin inscripciones" : espera.Count == 1 ? "1 inscripción" : $"{espera.Count} inscripciones");
            CargarCatalogo();
            DistribuirTarjetas();
        }
    }
}
