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
        private Control paginaActual;
        private Button navegacionActiva;

        public frmMenuCliente()
        {
            InitializeComponent();
            contenido = new Panel { Name = "contenidoNavegacion", Bounds = inicioCliente.Bounds, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            Controls.Remove(inicioCliente);
            Controls.Add(contenido);
            contenido.Controls.Add(inicioCliente);
            inicioCliente.Dock = DockStyle.Fill;
            paginaActual = inicioCliente;
            inicioCliente.VerEventos += () => btnEventos.PerformClick();

            RegistrarIcono(btnInicio, "\uE80F");
            RegistrarIcono(btnEventos, "\uE787");
            RegistrarIcono(btnHistorial, "\uE8A5");
            RegistrarIcono(btnSuscripcion, "\uE8D7");
            SizeChanged += (s, e) => DistribuirContenido();
            FormClosed += (s, e) => NavegacionCliente.Desregistrar(this);
            NavegacionCliente.Registrar(this);
            MarcarActivo(btnInicio);
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
            if (contenido == null) return;
            float escala = Disposicion.Escala(this);
            int margen = (int)Math.Round(24 * escala);
            int izquierda = (int)Math.Round(300 * escala);
            int arriba = (int)Math.Round(84 * escala);
            contenido.SetBounds(izquierda, arriba,
                Math.Max(1, ClientSize.Width - izquierda - margen),
                Math.Max(1, ClientSize.Height - arriba - margen));
            barraSuperior.BringToFront();
            lblNombre.BringToFront();
            label1.BringToFront();
            btnAtras.BringToFront();
            lblvolver.BringToFront();
            MarcarEstadoAtras();
        }

        private static void RegistrarIcono(Button boton, string glifo)
        {
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.Padding = new Padding(46, 0, 8, 0);
            var icono = new Label
            {
                Name = "icono" + boton.Name,
                Text = glifo,
                Font = Tema.IconosDocumentos,
                ForeColor = Tema.TextoSecundario,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(8, 0),
                Size = new Size(34, boton.Height),
                Cursor = Cursors.Hand
            };
            icono.Click += (s, e) => boton.PerformClick();
            boton.Controls.Add(icono);
            boton.Resize += (s, e) => icono.Height = boton.ClientSize.Height;
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
            if (pagina is Componentes.HostFormularioCliente host) host.Activar();
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
            if (paginaActual != inicioCliente) Mostrar(inicioCliente, false);
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
                par.Boton.Text = (par.Boton == boton ? "●  " : "   ") + par.Texto;
                par.Boton.BackColor = par.Boton == boton ? Tema.PrimarioSuave : Tema.Superficie;
                par.Boton.ForeColor = par.Boton == boton ? Tema.Primario : Tema.Texto;
            }
            navegacionActiva = boton;
        }

        private void MarcarEstadoAtras()
        {
            if (btnAtras != null) btnAtras.Enabled = historial.Count > 0 || paginaActual != inicioCliente;
        }

        private void btnInicio_Click(object sender, EventArgs e) { MostrarInicio(); }

        private void btnEventos_Click(object sender, EventArgs e)
        {
            var pantalla = new frmEventosCliente { IDUsuario = IDUsuario, NombreUsuario = NombreUsuario };
            if (NavegacionCliente.Activa) AbrirDesdeMenu(pantalla);
            else { pantalla.Show(); Hide(); }
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
