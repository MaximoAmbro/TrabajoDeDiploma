
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public partial class frmMenuPropietario : FormularioEventAura
    {
        private readonly FlowLayoutPanel zonaOrganizador;
        private readonly Panel lateral;
        private readonly Componentes.EtiquetaTitulo tituloOrganizador;
        private readonly Componentes.BotonSecundario btnCambiarContrasena;
        private readonly Componentes.EtiquetaRotulo seccionLocales;
        private readonly Componentes.EtiquetaRotulo seccionOperacion;
        private readonly Componentes.EtiquetaRotulo seccionResultados;
        private readonly Componentes.EtiquetaRotulo seccionCuenta;
        public int IDUsuario;
        public string NombreUsuario;

        public frmMenuPropietario()
        {
            InitializeComponent();
            seccionLocales = Seccion("seccionLocales", "LOCALES Y EVENTOS", 164);
            seccionOperacion = Seccion("seccionOperacion", "OPERACIÓN", 296);
            seccionResultados = Seccion("seccionResultados", "RESULTADOS", 428);
            seccionCuenta = Seccion("seccionCuenta", "CUENTA", 536);
            Controls.AddRange(new Control[] { seccionLocales, seccionOperacion, seccionResultados, seccionCuenta });
            btnMisLocales.Location = new Point(24, 188);
            btnRegistrarLocal.Location = new Point(24, 232);
            btnControlIngreso.Location = new Point(24, 320);
            btnListasEspera.Location = new Point(24, 364);
            btnReportes.Location = new Point(24, 452);
            lblvolver.Location = new Point(24, 628);
            lblvolver.Text = "Cerrar sesión";
            zonaOrganizador = new FlowLayoutPanel { Name = "zonaOrganizador", BackColor = Tema.Fondo, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            tituloOrganizador = new Componentes.EtiquetaTitulo { Name = "tituloOrganizador", Text = "Tu espacio de organización" };
            Controls.AddRange(new Control[] { tituloOrganizador, zonaOrganizador });
            btnCambiarContrasena = new Componentes.BotonSecundario { Name = "btnCambiarContrasena", Text = "Cambiar contraseña", Location = new Point(24, 576), Size = new Size(272, 40) };
            btnCambiarContrasena.Click += (s, e) =>
            {
                var cambio = new frmCambiarContraseña { CorreoInicial = GestorSesion.Instance.UsuarioActual?.Mail, Retorno = this };
                cambio.Show();
                Hide();
            };
            var inicio = new Componentes.BotonSecundario { Name = "btnInicio", Text = "Inicio" };
            inicio.Click += (s, e) => zonaOrganizador.AutoScrollPosition = Point.Empty;
            lateral = Disposicion.CrearLateral(this, inicio, seccionLocales, btnMisLocales, btnRegistrarLocal,
                seccionOperacion, btnControlIngreso, btnListasEspera, seccionResultados, btnReportes,
                seccionCuenta, btnCambiarContrasena, lblvolver);
            inicio.MarcarOpcionActual(true);
            ClientSizeChanged += (s, e) => DistribuirPanel();
        }

        private static Componentes.EtiquetaRotulo Seccion(string nombre, string texto, int arriba)
        {
            return new Componentes.EtiquetaRotulo { Name = nombre, Text = texto, Location = new Point(24, arriba), Size = new Size(272, 20) };
        }

        private void frmMenuVendedor_Load(object sender, EventArgs e)
        {
            GestorSesion.Instance.RequerirOrganizador();
            CargarTarjetas();
            DistribuirPanel();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Disposicion.AjustarAreaVisible(this);
            DistribuirPanel();
        }

        private void DistribuirPanel()
        {
            Disposicion.DistribuirMenu(this, lateral, zonaOrganizador, tituloOrganizador);
            foreach (Componentes.DatoPanel tarjeta in zonaOrganizador.Controls)
                tarjeta.Width = Math.Max(1, zonaOrganizador.ClientSize.Width - tarjeta.Margin.Horizontal - SystemInformation.VerticalScrollBarWidth);
        }

        private void CargarTarjetas()
        {
            int id = GestorSesion.Instance.UsuarioActual.Id;
            var locales = GestorLocales.Instance.PorPropietario(id);
            var eventos = new List<Entidades.Evento>();
            foreach (var local in locales)
            {
                var delLocal = GestorEventos.Instance.PorLocal(local.Id);
                AgregarTarjeta(local.Nombre, delLocal.Count == 1 ? "1 evento" : $"{delLocal.Count} eventos");
                eventos.AddRange(delLocal);
            }

            if (locales.Count == 0)
                AgregarTarjeta("Mis locales", "Todavía no registraste un local.", "Registrar local", () => btnRegistrarLocal.PerformClick());

            var proximos = eventos.Where(e => e.Fecha >= DateTime.Now && e.Estado != GestorEventos.EstadoSuspendido)
                .OrderBy(e => e.Fecha).ToArray();
            if (proximos.Length == 0)
                AgregarTarjeta("Próximos eventos", "Todavía no tenés eventos con fecha próxima.", "Mis locales", () => btnMisLocales.PerformClick());
            foreach (var evento in proximos)
            {
                int entradas = GestorEventos.Instance.EntradasVendidas(evento.Id);
                string ventas = entradas == 1 ? "1 entrada vendida" : $"{entradas} entradas vendidas";
                AgregarTarjeta(evento.Nombre, $"{evento.Fecha:dd/MM/yyyy HH:mm} · {ventas}");
            }
        }

        private void AgregarTarjeta(string titulo, string valor, string textoAccion = null, Action accion = null)
        {
            var tarjeta = new Componentes.DatoPanel();
            tarjeta.CargarDetalle(titulo, valor);
            zonaOrganizador.Controls.Add(tarjeta);
            if (accion != null) tarjeta.AgregarAccion(textoAccion, accion);
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnMisLocales_Click(object sender, EventArgs e)
        {
            frmMisLocales frm = new frmMisLocales();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnRegistrarLocal_Click(object sender, EventArgs e)
        {
            frmRegistrarLocal frm = new frmRegistrarLocal();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnListasEspera_Click(object sender, EventArgs e)
        {
            frmListasEspera frm = new frmListasEspera();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnReportes_Click(object sender, EventArgs e)
        {
            frmReportes frm = new frmReportes();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnControlIngreso_Click(object sender, EventArgs e)
        {
            using (var control = new frmControlIngreso()) control.ShowDialog(this);
        }

        private void lblvolver_Click(object sender, EventArgs e)
        {
            GestorSesion.Instance.Cerrar();
            frmInicio frmInicio = new frmInicio();
            frmInicio.Show(); this.Hide();
        }
    }
}
