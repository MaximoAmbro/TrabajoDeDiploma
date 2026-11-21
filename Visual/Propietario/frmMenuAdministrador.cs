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
        private Componentes.BotonSecundario btnEventosEnRevision;
        private Componentes.BotonSecundario btnCambiarContrasena;
        private Componentes.BotonSecundario btnCrearAdministrador;
        private Componentes.PanelAdministrador panel;

        public int IDUsuario;
        public string NombreUsuario;

        public frmMenuAdministrador()
        {
            InitializeComponent();
            btnReportesGlobales.Click += AbrirReportes;
            btnCerrarSesion.Click += CerrarSesion;
            btnSolicitudes.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); using (var form = new frmRevisarSolicitudesOrganizador()) form.ShowDialog(this); panel.Cargar(); };
            btnEventosEnRevision.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); using (var form = new frmRevisarEventos()) form.ShowDialog(this); };
            btnCrearAdministrador = new Componentes.BotonSecundario { Name = "btnCrearAdministrador", Text = "Crear administrador", Location = new Point(24, 180), Size = new Size(184, 40) };
            btnCrearAdministrador.Click += CrearAdministrador;
            Controls.Add(btnCrearAdministrador);
            btnCambiarContrasena = new Componentes.BotonSecundario { Name = "btnCambiarContrasena", Text = "Cambiar contraseña", Location = new Point(216, 180), Size = new Size(184, 40) };
            btnCambiarContrasena.Click += (s, e) =>
            {
                var cambio = new frmCambiarContraseña { CorreoInicial = GestorSesion.Instance.UsuarioActual?.Mail, Retorno = this };
                cambio.Show();
                Hide();
            };
            Controls.Add(btnCambiarContrasena);
            btnCerrarSesion.Location = new Point(408, 180);
            btnCerrarSesion.Size = new Size(136, 40);
            panel.Location = new Point(24, 232);
            panel.Height = Math.Max(1, ClientSize.Height - panel.Top - 24);
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
