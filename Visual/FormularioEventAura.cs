using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Visual
{
    // Base de los formularios de EventAura. Si el usuario cierra la última ventana visible,
    // termina la aplicación: antes quedaba un proceso sin ventanas (hallazgo U-01).
    public class FormularioEventAura : Form
    {
        public FormularioEventAura()
        {
            Font = Tema.Cuerpo;
            BackColor = Tema.Superficie;
            Icon = Tema.Icono;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                Tema.Normalizar(this);
            base.OnLoad(e);
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ReconfigurarTeclado();
                FijarTamanoSiNoEsAdaptable();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            BeginInvoke((MethodInvoker)(() =>
            {
                Control primerCampo = DescendientesEnOrden(this)
                    .FirstOrDefault(c => EsCampoInicial(c) && c.Visible && c.Enabled && c.CanFocus);
                if (primerCampo != null) primerCampo.Focus();
            }));
        }

        protected void ReconfigurarTeclado()
        {
            OrdenarTabulacion(this);
            if (AcceptButton == null && !EsMenuPrincipal())
            {
                var botones = DescendientesEnOrden(this).OfType<Button>().ToArray();
                string[] preferidos = { "btninicio", "btnregistrarse", "btnconfirmar", "btnPagar", "btnComprar", "btnGenerar", "btnBuscar", "btnRegistrar", "btnPublicarEvento", "btnEnviarSolicitud", "btnConfirmarCorreo", "btnAprobarPrimerEvento", "btnContratar", "aprobar", "btnAceptar" };
                foreach (string nombre in preferidos)
                {
                    Button boton = botones.FirstOrDefault(b => string.Equals(b.Name, nombre, StringComparison.OrdinalIgnoreCase) && b.Enabled && b.Visible);
                    if (boton != null) { AcceptButton = boton; break; }
                }
                if (AcceptButton == null)
                {
                    var principales = botones.Where(b => b.Enabled && b.Visible && b is Componentes.BotonPrimario).ToArray();
                    if (principales.Length == 1) AcceptButton = principales[0];
                }
            }
            if (CancelButton == null)
            {
                var botones = DescendientesEnOrden(this).OfType<Button>().ToArray();
                string[] nombres = { "btnCancelar", "btnVolver", "btnAtras", "btnCerrar", "btnVolverAcceso", "lblvolver" };
                foreach (string nombre in nombres)
                {
                    Button boton = botones.FirstOrDefault(b => string.Equals(b.Name, nombre, StringComparison.OrdinalIgnoreCase) && b.Enabled && b.Visible);
                    if (boton != null) { CancelButton = boton; break; }
                }
                if (CancelButton == null)
                    CancelButton = botones.FirstOrDefault(b =>
                    {
                        if (!b.Enabled || !b.Visible) return false;
                        string texto = (b.Text ?? string.Empty).Trim();
                        return new[] { "Cancelar", "Volver", "Cerrar" }.Contains(texto, StringComparer.OrdinalIgnoreCase) ||
                               texto.StartsWith("Volver ", StringComparison.OrdinalIgnoreCase);
                    });
            }
        }

        private bool EsMenuPrincipal()
        {
            string nombre = GetType().Name;
            return nombre == "frmMenuCliente" || nombre == "frmMenuPropietario" || nombre == "frmMenuAdministrador";
        }

        private static void OrdenarTabulacion(Control contenedor)
        {
            var hijos = contenedor.Controls.Cast<Control>().OrderBy(c => c.Top).ThenBy(c => c.Left).ToArray();
            int indice = 0;
            foreach (Control hijo in hijos) hijo.TabIndex = indice++;
            foreach (Control hijo in hijos) OrdenarTabulacion(hijo);
        }

        private static System.Collections.Generic.IEnumerable<Control> DescendientesEnOrden(Control raiz)
        {
            foreach (Control hijo in raiz.Controls.Cast<Control>().OrderBy(c => c.Top).ThenBy(c => c.Left))
            {
                yield return hijo;
                foreach (Control nieto in DescendientesEnOrden(hijo)) yield return nieto;
            }
        }

        private static bool EsCampoInicial(Control control)
        {
            if (!control.TabStop || control is Button || control is LinkLabel) return false;
            var texto = control as TextBoxBase;
            if (texto != null) return !texto.ReadOnly;
            if (control is ComboBox || control is NumericUpDown || control is DateTimePicker || control is DataGridView || control is RadioButton || control is CheckBox || control is ListBox)
                return true;
            return false;
        }

        private void FijarTamanoSiNoEsAdaptable()
        {
            if (FormBorderStyle != System.Windows.Forms.FormBorderStyle.Sizable &&
                FormBorderStyle != System.Windows.Forms.FormBorderStyle.SizableToolWindow)
                return;

            bool adaptable = DescendientesEnOrden(this).Any(c =>
                c.Dock == DockStyle.Fill ||
                (c.Anchor & (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom)) ==
                (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom));

            if (!adaptable)
            {
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                MaximizeBox = false;
            }
        }

        public static bool DebeTerminarAplicacion(Form cerrado, CloseReason motivo, IEnumerable<Form> abiertos)
        {
            return motivo == CloseReason.UserClosing &&
                   !abiertos.Any(f => f != cerrado && !f.IsDisposed && f.Visible);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // Application.MessageLoop es false en las pruebas: ahí nunca se termina el proceso.
            if (Application.MessageLoop &&
                DebeTerminarAplicacion(this, e.CloseReason, Application.OpenForms.Cast<Form>().ToList()))
            {
                Application.Exit();
            }
        }
    }
}
