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
        private readonly FlowLayoutPanel pendientes;
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
            pendientes.BringToFront();
            Tema.Normalizar(this);
            SizeChanged += (s,e)=>Distribuir();
        }
        public void Cargar()
        {
            GestorSesion.Instance.RequerirAdministrador();
            foreach(System.Windows.Forms.Control viejo in datos.Controls.Cast<System.Windows.Forms.Control>().ToArray())viejo.Dispose();
            var cultura=System.Globalization.CultureInfo.GetCultureInfo("es-AR");
            DateTime desde=DateTime.Today.AddMonths(-12),hasta=DateTime.Today.AddMonths(12);
            periodo.Text="Eventos: "+desde.ToString("dd/MM/yyyy")+" a "+hasta.ToString("dd/MM/yyyy")+". Niveles: al día de hoy.";
            foreach(string codigo in GestorReportes.Codigos) {
                var reporte=GestorReportes.Instance.Crear(codigo,desde,hasta);
                if(reporte is ReporteUsuariosPorNivel)((ReporteUsuariosPorNivel)reporte).FechaCorte=DateTime.Today;
                var tabla=GestorReportes.Instance.Ejecutar(reporte);
                var filas=tabla.Rows.Cast<DataRow>().Where(f=>Convert.ToString(f[0])!="TOTAL").ToArray();
                Func<int,decimal> suma=col=>filas.Sum(f=>f[col]==DBNull.Value?0m:Convert.ToDecimal(f[col]));
                string valor;
                switch(codigo) {
                    case "RPT-01":valor=suma(2).ToString("N0",cultura)+" entradas · "+suma(3).ToString("C0",cultura);break;
                    case "RPT-02":valor=filas.Length==0?"Todavía no hay compras":Convert.ToString(filas[0][2])+" entradas de la combinación líder";break;
                    case "RPT-03":valor=suma(2).ToString("N0",cultura)+" clientes activos";break;
                    case "RPT-04":valor=tabla.Rows.Count==0?"Todavía sin contrataciones":Convert.ToDecimal(tabla.Rows[0][3]).ToString("N1",cultura)+" % de compras con seguro";break;
                    default:valor=suma(2).ToString("N0",cultura)+" personas en espera";break;
                }
                var tarjeta=new DatoPanel {Name="resumen"+codigo.Replace("-","")};tarjeta.CargarDetalle(reporte.Titulo, valor, true);datos.Controls.Add(tarjeta);
            }
            eventosPendientes.CargarDetalle("Eventos por revisar", GestorEventos.Instance.PendientesDeRevision().Count.ToString(cultura));
            solicitudesPendientes.CargarDetalle("Solicitudes de organizador", GestorSolicitudesOrganizador.Instance.Pendientes().Count.ToString(cultura));
            Distribuir();
        }
        private void Distribuir()
        {
            int Px(int medida) => Disposicion.Pixeles(this, medida);
            int ancho = Math.Max(1, ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
            if (btnAuditoria.Parent == this) btnAuditoria.Left = Math.Max(0, ancho - btnAuditoria.Width);
            int anchoPeriodo = btnAuditoria.Parent == this ? ancho - btnAuditoria.Width - Px(Tema.Espacio.L) : ancho;
            int y = AutoScrollPosition.Y;
            periodo.SetBounds(0, y, Math.Max(1, anchoPeriodo), Px(Tema.Espacio.XXL));

            datos.Location = new System.Drawing.Point(0, periodo.Bottom + Px(Tema.Espacio.S));
            DatoPanel.DistribuirGrilla(datos, ancho);
            pendientes.Location = new System.Drawing.Point(0, datos.Bottom + Px(Tema.Espacio.S));
            DatoPanel.DistribuirGrilla(pendientes, ancho);
        }
    }
}
