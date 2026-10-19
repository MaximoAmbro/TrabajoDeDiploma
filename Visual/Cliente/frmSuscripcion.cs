using System;
using System.Globalization;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // RF-13 + CU-06: contratar o cancelar la suscripción. Un plan a la vez (RN-25) y sin
    // renovación automática (RN-26). Beneficios en especie, sin descuento porcentual (RN-37).
    public partial class frmSuscripcion : Form
    {
        public int IDUsuario;
        public string NombreUsuario;

        // RN-33: importes en pesos argentinos.
        private static readonly CultureInfo Pesos = new CultureInfo("es-AR");

        public frmSuscripcion()
        {
            InitializeComponent();
        }

        private void frmSuscripcion_Load(object sender, EventArgs e)
        {
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
            btnContratar.Enabled = !tiene;
            btnCancelar.Enabled = tiene;
        }

        private void btnContratar_Click(object sender, EventArgs e)
        {
            string plan = (string)(rdbAnual.Checked ? rdbAnual.Tag : rdbMensual.Tag);
            decimal precio = GestorSuscripciones.Instance.PrecioDe(plan);
            DialogResult respuesta = MessageBox.Show(
                $"¿Contratar el plan {plan} por {precio.ToString("C2", Pesos)}?\n\n" +
                "La suscripción no se renueva automáticamente al vencer.",
                "Contratar suscripción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta != DialogResult.Yes)
            {
                return;
            }
            try
            {
                Suscripcion nueva = GestorSuscripciones.Instance.Contratar(IDUsuario, plan);
                MessageBox.Show($"Suscripción {nueva.Plan} contratada. Vence el {nueva.FechaFin:dd/MM/yyyy}.",
                    "Suscripción", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ReglaNegocioException ex)
            {
                MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            MostrarEstado();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show(
                "¿Cancelar la suscripción? Los beneficios se pierden desde ahora.",
                "Cancelar suscripción", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (respuesta != DialogResult.Yes)
            {
                return;
            }
            try
            {
                GestorSuscripciones.Instance.Cancelar(IDUsuario);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Suscripción", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            MostrarEstado();
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuCliente frm = new frmMenuCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
    }
}
