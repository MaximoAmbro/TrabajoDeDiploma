using System;
using System.Globalization;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Newtonsoft.Json.Linq;

namespace Visual
{
    // RF-13 + CU-06: contratar o cancelar la suscripción. Un plan a la vez (RN-25) y sin
    // renovación automática (RN-26). Beneficios en especie, sin descuento porcentual (RN-37).
    public partial class frmSuscripcion : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;

        // RN-33: importes en pesos argentinos.
        private static readonly CultureInfo Pesos = new CultureInfo("es-AR");
        private readonly Web.PanelWeb suscripcionWeb = new Web.PanelWeb { Name = "suscripcionWeb", Visible = false };

        public frmSuscripcion()
        {
            InitializeComponent();
            Controls.Add(suscripcionWeb);
            suscripcionWeb.Dock = DockStyle.Fill;
            barraSuperior.BringToFront();
            suscripcionWeb.VistaLista += MostrarSuscripcionWeb;
            suscripcionWeb.VistaNoDisponible += MostrarRespaldoSuscripcion;
            suscripcionWeb.Mensaje += ProcesarMensajeWeb;
            suscripcionWeb.Escape += () => btnVolver_Click(this, EventArgs.Empty);
            Shown += (s, e) => suscripcionWeb.Cargar("suscripcion.html");
            Shown += (s, e) =>
            {
                ColocarTitulo(label1);
                Disposicion.LimitarAncho(lblBeneficios);
                lblBeneficios.Height = rdbMensual.Top - lblBeneficios.Top - Disposicion.Pixeles(this, Tema.Espacio.XS);
            };
            CancelButton = btnVolver;
        }

        private void frmSuscripcion_Load(object sender, EventArgs e)
        {
            label1.Location = new System.Drawing.Point(Tema.MargenFormulario, Tema.AltoBarraSuperior + Tema.MargenFormulario);
            rdbMensual.Tag = GestorSuscripciones.PlanMensual;
            rdbAnual.Tag = GestorSuscripciones.PlanAnual;
            rdbMensual.Text = Opcion(GestorSuscripciones.PlanMensual, "por mes");
            rdbAnual.Text = Opcion(GestorSuscripciones.PlanAnual, "por año");
            rdbMensual.Checked = true;

            lblBeneficios.Text =
                "Beneficios de la suscripción:\r\n" +
                "  •  Seguro de entrada sin cargo\r\n" +
                "  •  Preventa exclusiva de eventos\r\n" +
                "  •  Prioridad en las listas de espera\r\n" +
                "Sin descuento extra: rige el de tu nivel.";
            MostrarEstado();
        }

        private string Opcion(string plan, string periodo)
        {
            return $"{plan}: {GestorSuscripciones.Instance.PrecioDe(plan).ToString("C0", Pesos)} {periodo}";
        }

        private void MostrarEstado()
        {
            Suscripcion activa = GestorSuscripciones.Instance.ActivaDe(IDUsuario);
            bool tiene = activa != null;
            lblEstado.Text = tiene
                ? $"Plan {activa.Plan} activo\r\nVence el {activa.FechaFin:dd/MM/yyyy}\r\nNo se renueva automáticamente"
                : "No tenés una suscripción activa.";
            rdbMensual.Enabled = !tiene;
            rdbAnual.Enabled = !tiene;
            int ancho = Math.Max(btnContratar.Width, btnCancelar.Width);
            int espacio = Disposicion.Pixeles(this, Tema.Espacio.S);
            int margen = Disposicion.Pixeles(this, Tema.Espacio.XL);
            btnCancelar.SetBounds(margen, btnCancelar.Top, ancho, btnCancelar.Height);
            btnContratar.SetBounds(btnCancelar.Right + espacio, btnCancelar.Top, ancho, btnCancelar.Height);
            ClientSize = new System.Drawing.Size(Math.Max(ClientSize.Width, btnContratar.Right + margen), ClientSize.Height);
            btnContratar.Enabled = !tiene;
            btnCancelar.Enabled = tiene;
            suscripcionWeb.Actualizar(DatosWeb());
        }

        private object DatosWeb()
        {
            Suscripcion activa = GestorSuscripciones.Instance.ActivaDe(IDUsuario);
            decimal mensual = GestorSuscripciones.Instance.PrecioDe(GestorSuscripciones.PlanMensual);
            decimal anual = GestorSuscripciones.Instance.PrecioDe(GestorSuscripciones.PlanAnual);
            decimal ahorro = mensual * 12m - anual;
            return new
            {
                activa = activa != null,
                plan = activa?.Plan,
                estado = activa == null ? "No tenés una suscripción activa." :
                    $"Plan {activa.Plan} activo · Vence el {activa.FechaFin:dd/MM/yyyy} · No se renueva automáticamente",
                mensual = mensual.ToString("C0", Pesos),
                anual = anual.ToString("C0", Pesos),
                ahorro = "Ahorrás " + ahorro.ToString("C0", Pesos) + " frente a 12 meses",
                beneficios = new[] { "Seguro de entrada sin cargo", "Preventa exclusiva de eventos", "Prioridad en las listas de espera" }
            };
        }

        private void MostrarSuscripcionWeb()
        {
            foreach (Control control in Controls)
                if (control != barraSuperior && control != suscripcionWeb) control.Visible = false;
            suscripcionWeb.Visible = true;
            suscripcionWeb.BringToFront();
            barraSuperior.BringToFront();
        }

        private void MostrarRespaldoSuscripcion()
        {
            foreach (Control control in Controls)
                if (control != suscripcionWeb) control.Visible = true;
            barraSuperior.BringToFront();
        }

        private void ProcesarMensajeWeb(string json)
        {
            JObject mensaje;
            try { mensaje = JObject.Parse(json); }
            catch { return; }
            string tipo = (string)mensaje["tipo"];
            if (tipo == "navegar") { btnVolver_Click(this, EventArgs.Empty); return; }
            if (tipo == "contratarConfirmado")
            {
                string plan = (string)mensaje["plan"];
                rdbMensual.Checked = plan == GestorSuscripciones.PlanMensual;
                rdbAnual.Checked = plan == GestorSuscripciones.PlanAnual;
                ProcesarContratacion(plan);
            }
            else if (tipo == "cancelarConfirmado")
            {
                ProcesarCancelacion();
            }
        }

        private void btnContratar_Click(object sender, EventArgs e)
        {
            string plan = (string)(rdbAnual.Checked ? rdbAnual.Tag : rdbMensual.Tag);
            decimal precio = GestorSuscripciones.Instance.PrecioDe(plan);
            DialogResult respuesta = DialogoEventAura.Confirmar(this,
                "Contratar suscripción",
                $"Plan: {plan}\nPrecio: {precio.ToString("C2", Pesos)}\nVigencia: " +
                (plan == GestorSuscripciones.PlanAnual ? "12 meses" : "1 mes") +
                "\nNo se renueva automáticamente al vencer.", "Confirmar suscripción", "Volver");
            if (respuesta != DialogResult.Yes) return;
            ProcesarContratacion(plan);
        }

        private void ProcesarContratacion(string plan)
        {
            try
            {
                Suscripcion nueva = GestorSuscripciones.Instance.Contratar(IDUsuario, plan);
                Avisos.Informativo(this, $"Suscripción {nueva.Plan} contratada. Vence el {nueva.FechaFin:dd/MM/yyyy}. No se renueva automáticamente.");
            }
            catch (ReglaNegocioException ex)
            {
                Avisos.Regla(this, "Suscripción", ex);
            }
            MostrarEstado();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = DialogoEventAura.Confirmar(this,
                "Cancelar suscripción", "¿Cancelar la suscripción? Los beneficios se pierden desde ahora.",
                "Cancelar suscripción", "Volver", true);
            if (respuesta != DialogResult.Yes)
            {
                return;
            }
            ProcesarCancelacion();
        }

        private void ProcesarCancelacion()
        {
            try
            {
                GestorSuscripciones.Instance.Cancelar(IDUsuario);
            }
            catch (InvalidOperationException ex)
            {
                Avisos.Informativo(this, ex.Message);
            }
            MostrarEstado();
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (NavegacionCliente.Volver()) return;
            frmMenuCliente frm = new frmMenuCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
    }
}
