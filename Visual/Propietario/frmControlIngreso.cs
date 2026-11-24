using System;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public partial class frmControlIngreso : FormularioEventAura
    {
        private Componentes.EstadoVacio estadoSinEventos;
        private readonly Web.PanelWeb ingresoWeb = new Web.PanelWeb { Name = "controlIngresoWeb", Dock = DockStyle.None, Visible = false };
        private readonly Label avisoWeb = new Label { Name = "avisoControlIngresoWeb", AutoSize = true, ForeColor = Tema.TextoSecundario, Text = "Vista web no disponible. Se muestra el control de Windows Forms.", Visible = false };
        public frmControlIngreso()
        {
            InitializeComponent();
            Controls.Add(ingresoWeb);
            Controls.Add(avisoWeb);
            ingresoWeb.VistaLista += ActualizarVistaIngreso;
            ingresoWeb.VistaNoDisponible += ActualizarVistaIngreso;
            ingresoWeb.Escape += () => { Message m = default(Message); ProcessCmdKey(ref m, Keys.Escape); };
            ingresoWeb.Mensaje += RecibirMensajeIngreso;
            Shown += (s, e) => { AjustarWebIngreso(); ingresoWeb.Cargar("gestion.html"); };
            SizeChanged += (s, e) => AjustarWebIngreso();
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
                    estadoSinEventos.Cargar("Todavía no hay eventos con el ingreso abierto en tus locales. Cuando empiece la ventana, vas a poder validar entradas acá.", "Volver al panel");
                    estadoSinEventos.Accion += () => Close();
                    Controls.Add(estadoSinEventos);
                    int espacio = Disposicion.Pixeles(this, Tema.Espacio.XL);
                    estadoSinEventos.AjustarA(new System.Drawing.Rectangle(lblEvento.Left, lblEvento.Top,
                        ClientSize.Width - 2 * lblEvento.Left, btnVolver.Top - lblEvento.Top - espacio));
                    estadoSinEventos.BringToFront();
                    lblEvento.Visible = cmbEventos.Visible = lblCodigo.Visible = txtCodigo.Visible = btnRegistrar.Visible = lblResultado.Visible = lblContador.Visible = false;
                }
                ActualizarContador();
                ActualizarModeloIngreso();
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
                ActualizarModeloIngreso();
                txtCodigo.Focus();
            }
            catch (ReglaNegocioException ex) { Avisos.Regla("Control de ingreso", ex); ActualizarModeloIngreso(); }
        }

        private void AjustarWebIngreso()
        {
            if (ingresoWeb == null || IsDisposed) return;
            int top = lblEvento.Top;
            int bottom = Math.Max(btnVolver.Top, lblResultado.Bottom);
            ingresoWeb.SetBounds(lblEvento.Left, top, Math.Max(1, ClientSize.Width - lblEvento.Left * 2), Math.Max(1, bottom - top));
            avisoWeb.Location = new System.Drawing.Point(ingresoWeb.Left, Math.Max(lblTituloIngreso.Bottom, ingresoWeb.Top - avisoWeb.Height));
        }

        private void ActualizarVistaIngreso()
        {
            bool web = ingresoWeb.Preparado;
            ingresoWeb.Visible = web;
            avisoWeb.Visible = ingresoWeb.InicializacionTerminada && !web;
            bool vacio = cmbEventos.Items.Count == 0;
            lblEvento.Visible = cmbEventos.Visible = lblCodigo.Visible = txtCodigo.Visible = btnRegistrar.Visible = lblResultado.Visible = lblContador.Visible = !web && !vacio;
            if (estadoSinEventos != null) estadoSinEventos.Visible = !web && vacio;
            if (web) ingresoWeb.BringToFront(); else avisoWeb.BringToFront();
        }

        private void ActualizarModeloIngreso()
        {
            var eventos = cmbEventos.Items.Cast<Evento>().Select(e => new { id = e.Id, nombre = e.Nombre }).ToArray();
            Evento seleccionado = cmbEventos.SelectedItem as Evento;
            ingresoWeb.Actualizar(new { vista = "controlIngreso", eventos, seleccionado = seleccionado?.Id,
                codigo = txtCodigo.Text, resultado = lblResultado.Text,
                contador = lblContador.Text });
        }

        private void RecibirMensajeIngreso(string texto)
        {
            var tipo = System.Text.RegularExpressions.Regex.Match(texto ?? "", "\\\"tipo\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"");
            if (!tipo.Success) return;
            if (tipo.Groups[1].Value == "volver") { Close(); return; }
            if (tipo.Groups[1].Value != "ingreso") return;
            var id = System.Text.RegularExpressions.Regex.Match(texto, "\\\"id\\\"\\s*:\\s*(\\d+)");
            var codigo = System.Text.RegularExpressions.Regex.Match(texto, "\\\"codigo\\\"\\s*:\\s*\"((?:\\\\.|[^\"])*)\"");
            if (!id.Success || !int.TryParse(id.Groups[1].Value, out int eventoId)) return;
            cmbEventos.SelectedValue = eventoId;
            txtCodigo.Text = codigo.Success ? Newtonsoft.Json.JsonConvert.DeserializeObject<string>("\"" + codigo.Groups[1].Value + "\"") : string.Empty;
            Registrar(this, EventArgs.Empty);
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
            ActualizarModeloIngreso();
        }

        private void Volver(object sender, EventArgs e) { Close(); }
    }
}
