using System;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public partial class frmControlIngreso : FormularioEventAura
    {
        public frmControlIngreso() { InitializeComponent(); }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                cmbEventos.DisplayMember = "Nombre";
                cmbEventos.ValueMember = "Id";
                cmbEventos.DataSource = GestorIngresos.Instance.EventosDeHoy();
                txtCodigo.Enabled = cmbEventos.Items.Count > 0;
                btnRegistrar.Enabled = txtCodigo.Enabled;
                lblResultado.Font = Tema.Titulo;
                lblResultado.Text = txtCodigo.Enabled ? "Escanee una entrada." : "No hay eventos publicados para hoy.";
                ActualizarContador();
                txtCodigo.Focus();
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
                ActualizarContador();
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
            lblResultado.Text = "Escanee una entrada.";
            ActualizarContador();
        }

        private void Volver(object sender, EventArgs e) { Close(); }
    }
}
