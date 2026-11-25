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
        private Componentes.EtiquetaRotulo seccionPlataforma;
        private Componentes.EtiquetaRotulo seccionRevision;
        private Componentes.EtiquetaRotulo seccionCuenta;

        public int IDUsuario;
        public string NombreUsuario;

        public frmMenuAdministrador()
        {
            InitializeComponent();
            seccionPlataforma = Seccion("seccionPlataforma", "PLATAFORMA", 24, 184);
            seccionRevision = Seccion("seccionRevision", "REVISIÓN", 232, 240);
            seccionCuenta = Seccion("seccionCuenta", "CUENTA", 496, 184);
            Controls.AddRange(new Control[] { seccionPlataforma, seccionRevision, seccionCuenta });
            btnReportesGlobales.Location = new Point(24, 152);
            btnSolicitudes.Location = new Point(232, 152);
            btnEventosEnRevision.Location = new Point(232, 200);
            btnCambiarContrasena = new Componentes.BotonSecundario { Name = "btnCambiarContrasena", Text = "Cambiar contraseña", Location = new Point(496, 152), Size = new Size(184, 40) };
            btnCambiarContrasena.Click += (s, e) =>
            {
                var cambio = new frmCambiarContraseña { CorreoInicial = GestorSesion.Instance.UsuarioActual?.Mail, Retorno = this };
                cambio.Show();
                Hide();
            };
            Controls.Add(btnCambiarContrasena);
            btnCerrarSesion.Location = new Point(496, 200);
            btnCerrarSesion.Size = new Size(136, 40);
            panel.Location = new Point(24, 260);
            panel.Height = Math.Max(1, ClientSize.Height - panel.Top - 24);
            btnReportesGlobales.Click += AbrirReportes;
            btnCerrarSesion.Click += CerrarSesion;
            btnSolicitudes.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); using (var form = new frmRevisarSolicitudesOrganizador()) form.ShowDialog(this); panel.Cargar(); };
            btnEventosEnRevision.Click += (s, e) => { GestorSesion.Instance.RequerirAdministrador(); using (var form = new frmRevisarEventos()) form.ShowDialog(this); };
            btnCrearAdministrador = new Componentes.BotonSecundario { Name = "btnCrearAdministrador", Text = "Crear administrador", Location = new Point(24, 200), Size = new Size(184, 40) };
            btnCrearAdministrador.Click += CrearAdministrador;
            Controls.Add(btnCrearAdministrador);
            panel.AuditoriaSolicitada += (s, e) => { using (var form = new frmAuditoria()) form.ShowDialog(this); };
            Load += (s, e) =>
            {
                GestorSesion.Instance.RequerirAdministrador();
                panel.Cargar();
                barra.ActualizarSesion();
            };
        }

        private static Componentes.EtiquetaRotulo Seccion(string nombre, string texto, int x, int ancho)
        {
            return new Componentes.EtiquetaRotulo { Name = nombre, Text = texto, Location = new Point(x, 124), Size = new Size(ancho, 20) };
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
