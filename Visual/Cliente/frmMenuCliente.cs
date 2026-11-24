using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public partial class frmMenuCliente : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;

        private readonly Stack<Control> historial = new Stack<Control>();
        private Panel contenido;
        private readonly Panel lateral;
        private Control paginaActual;
        private Button navegacionActiva;
        private Componentes.BotonSecundario btnSolicitudOrganizador;
        private Componentes.BotonSecundario btnCambiarContrasena;
        private Componentes.EtiquetaRotulo seccionExplorar;
        private Componentes.EtiquetaRotulo seccionCompras;
        private Componentes.EtiquetaRotulo seccionCuenta;

        public frmMenuCliente()
        {
            InitializeComponent();
            seccionExplorar = Seccion("seccionExplorar", "EXPLORAR", 164);
            seccionCompras = Seccion("seccionTusCompras", "TUS COMPRAS", 292);
            seccionCuenta = Seccion("seccionCuenta", "CUENTA", 484);
            Controls.AddRange(new Control[] { seccionExplorar, seccionCompras, seccionCuenta });
            btnInicio.Location = new Point(24, 188);
            btnEventos.Location = new Point(24, 232);
            btnHistorial.Location = new Point(24, 316);
            btnSuscripcion.Location = new Point(24, 360);
            btnAtras.Hide();
            btnSolicitudOrganizador = new Componentes.BotonSecundario { Name = "btnSolicitudOrganizador", Text = "Solicitar ser Organizador", Location = new Point(24, 512), Size = new Size(240, 40) };
            btnSolicitudOrganizador.Click += (s, e) =>
            {
                using (var solicitud = new frmSolicitudOrganizador())
                    if (solicitud.ShowDialog(this) == DialogResult.OK)
                        Avisos.Informativo(this, "Recibimos tu solicitud. Tu cuenta sigue siendo Cliente mientras esté pendiente.");
            };
            Controls.Add(btnSolicitudOrganizador);
            btnSolicitudOrganizador.BringToFront();
            btnCambiarContrasena = new Componentes.BotonSecundario { Name = "btnCambiarContrasena", Text = "Cambiar contraseña", Location = new Point(24, 560), Size = new Size(240, 40) };
            btnCambiarContrasena.Click += (s, e) =>
            {
                var cambio = new frmCambiarContraseña { CorreoInicial = GestorSesion.Instance.UsuarioActual?.Mail, Retorno = this };
                cambio.Show();
                Hide();
            };
            Controls.Add(btnCambiarContrasena);
            btnCambiarContrasena.BringToFront();
            lblvolver.Location = new Point(24, 680);
            contenido = new Panel { Name = "contenidoNavegacion", Bounds = inicioCliente.Bounds, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            Controls.Remove(inicioCliente);
            Controls.Add(contenido);
            contenido.Controls.Add(inicioCliente);
            inicioCliente.Dock = DockStyle.Fill;
            paginaActual = inicioCliente;
            inicioCliente.VerEventos += () => btnEventos.PerformClick();
            inicioCliente.VerEntradas += () => btnHistorial.PerformClick();
            inicioCliente.VerEvento += evento => AbrirCatalogo(evento);

            lblNombre.Visible = false;
            lateral = Disposicion.CrearLateral(this, seccionExplorar, btnInicio, btnEventos,
                seccionCompras, btnHistorial, btnSuscripcion, seccionCuenta,
                btnSolicitudOrganizador, btnCambiarContrasena, lblvolver);
            SizeChanged += (s, e) => DistribuirContenido();
            FormClosed += (s, e) => NavegacionCliente.Desregistrar(this);
            NavegacionCliente.Registrar(this);
            MarcarActivo(btnInicio);
        }

        private static Componentes.EtiquetaRotulo Seccion(string nombre, string texto, int arriba)
        {
            return new Componentes.EtiquetaEncabezado { Name = nombre, Text = texto, Location = new Point(24, arriba), Size = new Size(240, 20) };
        }

        private void frmMenuCliente_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreUsuario;
            int id = IDUsuario > 0 ? IDUsuario : (GestorSesion.Instance.UsuarioActual?.Id ?? 0);
            IDUsuario = id;
            inicioCliente.Cargar(id);
            barraSuperior.ActualizarSesion();
            DistribuirContenido();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Disposicion.AjustarAreaVisible(this);
            DistribuirContenido();
        }

        private void DistribuirContenido()
        {
            if (lateral == null) return;
            Disposicion.DistribuirMenu(this, lateral, contenido, label1);
            MarcarEstadoAtras();
        }

        private void Mostrar(Control pagina, bool guardarActual)
        {
            if (paginaActual != null && paginaActual != pagina)
            {
                contenido.Controls.Remove(paginaActual);
                paginaActual.Visible = false;
                if (guardarActual) historial.Push(paginaActual);
                else DiferirDescarte(paginaActual);
            }

            if (pagina.Parent != contenido) contenido.Controls.Add(pagina);
            pagina.Dock = DockStyle.Fill;
            pagina.Visible = true;
            pagina.BringToFront();
            paginaActual = pagina;
            if (pagina is Componentes.HostFormularioCliente host)
            {
                host.Activar();
                label1.Text = host.Pantalla.Text.Replace("EventAura · ", "");
            }
            else label1.Text = "Mi cuenta";
            ActualizarActivo(pagina);
            MarcarEstadoAtras();
        }

        private Componentes.HostFormularioCliente Envolver(Form pantalla)
        {
            return new Componentes.HostFormularioCliente(pantalla) { Dock = DockStyle.Fill };
        }

        private void DiferirDescarte(Control control)
        {
            if (control == null || control == inicioCliente || control.IsDisposed) return;
            if (IsHandleCreated)
                BeginInvoke((Action)(() => { if (!control.IsDisposed) control.Dispose(); }));
            else control.Dispose();
        }

        internal void AbrirDesdeMenu(Form pantalla)
        {
            while (historial.Count > 0) DiferirDescarte(historial.Pop());
            Control pagina = Envolver(pantalla);
            Mostrar(pagina, false);
        }

        internal void NavegarDesdePantalla(Form pantalla)
        {
            Control pagina = Envolver(pantalla);
            Mostrar(pagina, true);
        }

        internal void VolverPagina()
        {
            if (historial.Count > 0) Mostrar(historial.Pop(), false);
            else MostrarInicio();
        }

        internal void MostrarInicio()
        {
            while (historial.Count > 0) DiferirDescarte(historial.Pop());
            if (paginaActual != inicioCliente)
            {
                inicioCliente.Cargar(IDUsuario);
                Mostrar(inicioCliente, false);
            }
            else { ActualizarActivo(inicioCliente); MarcarEstadoAtras(); }
        }

        internal void CerrarSesion()
        {
            while (historial.Count > 0) DiferirDescarte(historial.Pop());
            if (paginaActual != inicioCliente)
            {
                contenido.Controls.Remove(paginaActual);
                DiferirDescarte(paginaActual);
            }
            paginaActual = inicioCliente;
            contenido.Controls.Remove(inicioCliente);
            NavegacionCliente.Desregistrar(this);
            GestorSesion.Instance.Cerrar();
            var acceso = new frmInicio();
            acceso.Show();
            Hide();
        }

        private void ActualizarActivo(Control pagina)
        {
            Button boton = btnInicio;
            Componentes.HostFormularioCliente host = pagina as Componentes.HostFormularioCliente;
            string tipo = host?.Pantalla.GetType().Name;
            if (tipo == "frmEventosCliente" || tipo == "FrmCompraCliente" || tipo == "frmPago" || tipo == "frmConfirmacion") boton = btnEventos;
            else if (tipo == "frmHistorialCliente") boton = btnHistorial;
            else if (tipo == "frmSuscripcion") boton = btnSuscripcion;
            MarcarActivo(boton);
        }

        private void MarcarActivo(Button boton)
        {
            foreach (var par in new[] {
                new { Boton = (Button)btnInicio, Texto = "Inicio" },
                new { Boton = (Button)btnEventos, Texto = "Eventos disponibles" },
                new { Boton = (Button)btnHistorial, Texto = "Mis entradas" },
                new { Boton = (Button)btnSuscripcion, Texto = "Mi suscripción" } })
            {
                par.Boton.Text = par.Texto;
                ((Componentes.BotonTematico)par.Boton).MarcarOpcionActual(par.Boton == boton);
            }
            navegacionActiva = boton;
        }

        private void MarcarEstadoAtras()
        {
            if (btnAtras != null) btnAtras.Enabled = historial.Count > 0 || paginaActual != inicioCliente;
        }

        private void btnInicio_Click(object sender, EventArgs e) { MostrarInicio(); }

        private void btnEventos_Click(object sender, EventArgs e) => AbrirCatalogo();

        private void AbrirCatalogo(Entidades.Evento evento = null)
        {
            var pantalla = new frmEventosCliente { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            if (NavegacionCliente.Activa) AbrirDesdeMenu(pantalla);
            else { pantalla.Show(); Hide(); }
            if (evento != null) pantalla.ElegirEvento(evento);
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            var pantalla = new frmHistorialCliente { NombreUsuario = NombreUsuario, IDUsuario = IDUsuario };
            if (NavegacionCliente.Activa) AbrirDesdeMenu(pantalla);
            else { pantalla.Show(); Hide(); }
        }

        private void btnSuscripcion_Click(object sender, EventArgs e)
        {
            var pantalla = new frmSuscripcion { NombreUsuario = NombreUsuario, IDUsuario = IDUsuario };
            if (NavegacionCliente.Activa) AbrirDesdeMenu(pantalla);
            else { pantalla.Show(); Hide(); }
        }

        private void btnAtras_Click(object sender, EventArgs e) { VolverPagina(); }

        private void lblvolver_Click(object sender, EventArgs e)
        {
            if (NavegacionCliente.Activa) CerrarSesion();
            else
            {
                GestorSesion.Instance.Cerrar();
                var acceso = new frmInicio();
                acceso.Show();
                Hide();
            }
        }
    }
}
