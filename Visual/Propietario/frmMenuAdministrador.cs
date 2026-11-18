using System;
using System.Drawing;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    // Acciones globales reservadas al perfil Administrador de plataforma.
    public sealed partial class frmMenuAdministrador : FormularioEventAura
    {
        private Componentes.BarraSuperior barra;
        private Componentes.EtiquetaTitulo titulo;
        private Componentes.BotonPrimario btnReportesGlobales;
        private Componentes.BotonSecundario btnCerrarSesion;
        private Componentes.BotonSecundario btnSolicitudes;
        private Componentes.PanelAdministrador panel;

        public int IDUsuario;
        public string NombreUsuario;

        public frmMenuAdministrador()
        {
            InitializeComponent();
            btnReportesGlobales.Click += AbrirReportes;
            btnCerrarSesion.Click += CerrarSesion;
            btnSolicitudes.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); using (var form = new frmRevisarSolicitudesOrganizador()) form.ShowDialog(this); panel.Cargar(); };
            panel.CrearAdministradorSolicitado += CrearAdministrador;
            panel.AuditoriaSolicitada += (s, e) => { using (var form = new frmAuditoria()) form.ShowDialog(this); };
            Load += (s, e) =>
            {
                GestorSesion.Instance.RequerirAdministrador();
                panel.Cargar();
                barra.ActualizarSesion();
            };
        }

        private void AbrirReportes(object sender, EventArgs e)
        {
            GestorSesion.Instance.RequerirAdministrador();
            var form = new frmReportes { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            form.Show();
            Hide();
        }

        private void CrearAdministrador(object sender, EventArgs e)
        {
            GestorSesion.Instance.RequerirAdministrador();
            var alta = new frmCrearUsuario(true);
            alta.Show();
            Hide();
        }

        private void CerrarSesion(object sender, EventArgs e)
        {
            GestorSesion.Instance.Cerrar();
            new frmInicio().Show();
            Hide();
        }
    }
}
