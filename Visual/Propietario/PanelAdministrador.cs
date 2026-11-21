using System;
using System.Data;
using System.Linq;
using Negocio;
namespace Visual.Componentes
{
    public partial class PanelAdministrador : SuperficieTematica
    {
        private readonly BotonSecundario btnAuditoria;
        public event EventHandler AuditoriaSolicitada;

        public PanelAdministrador()
        {
            InitializeComponent();
            btnAuditoria = new BotonSecundario { Name = "btnAuditoria", Text = "Registro de auditoría", Size = new System.Drawing.Size(180, 40), Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right };
            btnAuditoria.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); AuditoriaSolicitada?.Invoke(this, EventArgs.Empty); };
            Controls.Add(btnAuditoria);
            Tema.Normalizar(this);
            datos.SizeChanged += (s,e)=>Distribuir();
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
                var tarjeta=new DatoPanel {Name="resumen"+codigo.Replace("-","")};tarjeta.Cargar(reporte.Titulo,valor);datos.Controls.Add(tarjeta);
            }
            Distribuir();
        }
        private void Distribuir()
        {
            var prefAuditoria = btnAuditoria.GetPreferredSize(System.Drawing.Size.Empty);
            btnAuditoria.Size = new System.Drawing.Size(Math.Max(btnAuditoria.Width, prefAuditoria.Width + 8), Math.Max(btnAuditoria.Height, prefAuditoria.Height + 4));
            periodo.SetBounds(0, 0, Math.Max(1, ClientSize.Width - btnAuditoria.Width - 16), 52);
            btnAuditoria.Location = new System.Drawing.Point(Math.Max(0, ClientSize.Width - btnAuditoria.Width), 6);
            foreach(System.Windows.Forms.Control dato in datos.Controls)
                dato.Width=Math.Max(1,(datos.ClientSize.Width-System.Windows.Forms.SystemInformation.VerticalScrollBarWidth)/2-dato.Margin.Horizontal);
        }
    }
}
