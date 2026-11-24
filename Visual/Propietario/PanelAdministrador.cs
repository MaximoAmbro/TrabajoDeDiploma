using System;
using System.Data;
using System.Linq;
using Negocio;
using System.Windows.Forms;
namespace Visual.Componentes
{
    public partial class PanelAdministrador : SuperficieTematica
    {
        private readonly BotonSecundario btnAuditoria;
        private readonly Web.PanelWeb vistaWeb = new Web.PanelWeb { Name = "panelAdministradorWeb", Visible = false };
        private readonly Label avisoWeb = new Label
            { Name = "avisoAdministradorWeb", ForeColor = Tema.TextoSecundario, Visible = false };
        private readonly FlowLayoutPanel pendientes;
        private readonly EtiquetaEncabezado tituloPendientes = new EtiquetaEncabezado { Text = "Pendientes de revisión" };
        private readonly DatoPanel eventosPendientes;
        private readonly DatoPanel solicitudesPendientes;
        public event EventHandler AuditoriaSolicitada;
        internal Button AccionAuditoria => btnAuditoria;

        public PanelAdministrador()
        {
            InitializeComponent();
            btnAuditoria = new BotonSecundario { Name = "btnAuditoria", Text = "Registro de auditoría", Size = new System.Drawing.Size(180, 40), Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right };
            btnAuditoria.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); AuditoriaSolicitada?.Invoke(this, EventArgs.Empty); };
            Controls.Add(btnAuditoria);
            pendientes = new FlowLayoutPanel
            {
                Name = "revisionPendiente", WrapContents = true
            };
            eventosPendientes = new DatoPanel { Name = "cantidadEventosPendientes" };
            solicitudesPendientes = new DatoPanel { Name = "cantidadSolicitudesPendientes" };
            pendientes.Controls.AddRange(new Control[] { eventosPendientes, solicitudesPendientes });
            Controls.Add(pendientes);
            Controls.Add(tituloPendientes);
            pendientes.BringToFront();
            Tema.Normalizar(this);
            Controls.Add(vistaWeb);
            Controls.Add(avisoWeb);
            avisoWeb.Text = "Vista web no disponible. Se muestran los indicadores de Windows.";
            vistaWeb.VistaLista += ActualizarVistaWeb;
            vistaWeb.VistaNoDisponible += ActualizarVistaWeb;
            vistaWeb.Escape += () =>
            {
                if (FindForm()?.CancelButton is Button boton) boton.PerformClick();
            };
            Load += (s, e) => vistaWeb.Cargar("datos.html");
            SizeChanged += (s,e)=>Distribuir();
        }
        public void Cargar()
        {
            GestorSesion.Instance.RequerirAdministrador();
            foreach(System.Windows.Forms.Control viejo in datos.Controls.Cast<System.Windows.Forms.Control>().ToArray())viejo.Dispose();
            var cultura=System.Globalization.CultureInfo.GetCultureInfo("es-AR");
            DateTime desde=DateTime.Today.AddMonths(-12),hasta=DateTime.Today.AddMonths(12);
            periodo.Text="Eventos: "+desde.ToString("dd/MM/yyyy")+" a "+hasta.ToString("dd/MM/yyyy")+". Niveles: al día de hoy.";
            object ventas = null;
            foreach(string codigo in GestorReportes.Codigos) {
                var reporte=GestorReportes.Instance.Crear(codigo,desde,hasta);
                if(reporte is ReporteUsuariosPorNivel)((ReporteUsuariosPorNivel)reporte).FechaCorte=DateTime.Today;
                var tabla=GestorReportes.Instance.Ejecutar(reporte);
                var filas = tabla.Rows.Cast<DataRow>().ToArray();
                if (Equals(tabla.ExtendedProperties[Reporte.PropiedadFilaTotales], true))
                    filas = filas.Take(filas.Length - 1).ToArray();
                Func<int,decimal> suma=col=>filas.Sum(f=>f[col]==DBNull.Value?0m:Convert.ToDecimal(f[col]));
                string titulo = reporte.Titulo;
                string valor;
                switch(codigo) {
                    case "RPT-01":
                        ventas = Web.DatosWebReportes.Crear(reporte, tabla);
                        var entradas = new DatoPanel { Name = "entradasVendidasPanel" };
                        entradas.CargarDetalle("Entradas vendidas", suma(2).ToString("N0", cultura));
                        datos.Controls.Add(entradas);
                        titulo = "Ingreso bruto";
                        valor = suma(3).ToString("C0", cultura);
                        break;
                    case "RPT-02":titulo = "Paquete líder"; valor=filas.Length==0?"Sin compras":Convert.ToString(filas[0][2])+" entradas";break;
                    case "RPT-03":valor=suma(2).ToString("N0",cultura)+" clientes activos";break;
                    case "RPT-04":titulo = "Compras con seguro"; valor=tabla.Rows.Count==0?"Sin contrataciones":Convert.ToDecimal(tabla.Rows[0][3]).ToString("N1",cultura)+" %";break;
                    default:valor=suma(2).ToString("N0",cultura)+" personas en espera";break;
                }
                var tarjeta=new DatoPanel {Name="resumen"+codigo.Replace("-","")};tarjeta.CargarDetalle(titulo, valor);datos.Controls.Add(tarjeta);
            }
            eventosPendientes.CargarDetalle("Eventos por revisar", GestorEventos.Instance.PendientesDeRevision().Count.ToString(cultura));
            solicitudesPendientes.CargarDetalle("Solicitudes de organizador", GestorSolicitudesOrganizador.Instance.Pendientes().Count.ToString(cultura));
            vistaWeb.Actualizar(new
            {
                periodo = periodo.Text,
                metricas = datos.Controls.OfType<DatoPanel>().Select(tarjeta => tarjeta.ModeloWeb()).ToArray(),
                pendientes = pendientes.Controls.OfType<DatoPanel>().Select(tarjeta => tarjeta.ModeloWeb()).ToArray(),
                reporte = ventas
            });
            Distribuir();
        }

        private void ActualizarVistaWeb()
        {
            vistaWeb.Visible = vistaWeb.Preparado;
            periodo.Visible = datos.Visible = tituloPendientes.Visible = pendientes.Visible = !vistaWeb.Visible;
            avisoWeb.Visible = vistaWeb.InicializacionTerminada && !vistaWeb.Preparado;
            if (vistaWeb.Visible) vistaWeb.BringToFront();
            if (avisoWeb.Visible) avisoWeb.BringToFront();
            Distribuir();
        }

        private void Distribuir()
        {
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int ancho = Math.Max(1, ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            if (btnAuditoria.Parent == this) btnAuditoria.Left = Math.Max(0, ancho - btnAuditoria.Width);
            int anchoPeriodo = btnAuditoria.Parent == this ? ancho - btnAuditoria.Width - Px(Tema.Espacio.L) : ancho;
            int y = AutoScrollPosition.Y;
            if (vistaWeb.Preparado) return;
            avisoWeb.SetBounds(0, y, ancho, Px(Tema.Espacio.XL));
            periodo.SetBounds(0, y + (avisoWeb.Visible ? avisoWeb.Height : 0),
                Math.Max(1, anchoPeriodo), Px(Tema.Espacio.XXL));

            datos.Location = new System.Drawing.Point(0, periodo.Bottom + Px(Tema.Espacio.S));
            DatoPanel.DistribuirGrilla(datos, ancho);
            tituloPendientes.SetBounds(0, datos.Bottom + Px(Tema.Espacio.S), ancho, Px(Tema.Espacio.XL));
            pendientes.Location = new System.Drawing.Point(0, tituloPendientes.Bottom + Px(Tema.Espacio.S));
            DatoPanel.DistribuirGrilla(pendientes, ancho);
        }
    }
}
