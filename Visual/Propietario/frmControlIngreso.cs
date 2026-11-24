using System;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public partial class frmControlIngreso : FormularioEventAura
    {
        private Componentes.EstadoVacio estadoSinEventos;
        public frmControlIngreso()
        {
            InitializeComponent();
            Shown += (s, e) =>
            {
                ColocarCampo(lblCodigo, txtCodigo);
                ColocarRotulo(lblEvento, cmbEventos);
                btnRegistrar.Left = txtCodigo.Right + Disposicion.Pixeles(this, Tema.Espacio.S);
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                cmbEventos.DisplayMember = "Nombre";
                cmbEventos.ValueMember = "Id";
                cmbEventos.DataSource = GestorIngresos.Instance.EventosConIngresoAbierto();
                txtCodigo.Enabled = cmbEventos.Items.Count > 0;
                btnRegistrar.Enabled = txtCodigo.Enabled;
                lblResultado.Font = Tema.Titulo;
                lblResultado.Text = txtCodigo.Enabled ? "Escaneá una entrada." : "No hay eventos con ingreso abierto en tus locales.";
                if (!txtCodigo.Enabled)
                {
                    estadoSinEventos = new Componentes.EstadoVacio { Name = "estadoSinIngresoAbierto" };
                    estadoSinEventos.AjustarA(new System.Drawing.Rectangle(24, 140, 672, 272));
                    estadoSinEventos.Cargar("Todavía no hay eventos con el ingreso abierto en tus locales. Cuando empiece la ventana, vas a poder validar entradas acá.", "Volver al panel");
                    estadoSinEventos.Accion += () => Close();
                    Controls.Add(estadoSinEventos);
                    estadoSinEventos.BringToFront();
                    lblEvento.Visible = cmbEventos.Visible = lblCodigo.Visible = txtCodigo.Visible = btnRegistrar.Visible = lblResultado.Visible = lblContador.Visible = false;
                }
                ActualizarContador();
                if (txtCodigo.Enabled) txtCodigo.Focus();
                else btnVolver.Focus();
            }
            catch (ReglaNegocioException ex) { Avisos.Regla("Control de ingreso", ex); Close(); }
        }

        private void Registrar(object sender, EventArgs e)
        {
            Evento evento = cmbEventos.SelectedItem as Evento;
            if (evento == null || string.IsNullOrWhiteSpace(txtCodigo.Text)) return;
            try
            {
                ResultadoIngreso resultado = GestorIngresos.Instance.Registrar(txtCodigo.Text, evento.Id);
                lblResultado.Text = resultado.Mensaje;
                lblResultado.ForeColor = resultado.Valido ? Tema.Exito : Tema.Error;
                if (resultado.Motivo != MotivoIngreso.NoAutorizado) ActualizarContador();
                txtCodigo.Clear();
                txtCodigo.Focus();
            }
            catch (ReglaNegocioException ex) { Avisos.Regla("Control de ingreso", ex); }
        }

        private void ActualizarContador()
        {
            Evento evento = cmbEventos.SelectedItem as Evento;
            lblContador.Text = "Ingresos registrados: " + (evento == null ? 0 : GestorIngresos.Instance.Contar(evento.Id));
        }

        private void EventoCambiado(object sender, EventArgs e)
        {
            lblResultado.Text = "Escaneá una entrada.";
            ActualizarContador();
        }

        private void Volver(object sender, EventArgs e) { Close(); }
    }
}
