using System;
using System.Drawing;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public sealed partial class frmAuditoria : FormularioEventAura
    {
        public frmAuditoria()
        {
            InitializeComponent();
            desde.Value = DateTime.Today.AddDays(-30);
            hasta.Value = DateTime.Today;
            accion.Items.AddRange(new object[] { "Todas", "Inicio de sesión", "Cambio de contraseña", "Compra iniciada", "Compra confirmada", "Devolución", "Control de ingreso", "Gestión de eventos: crear", "Gestión de eventos: publicar", "Gestión de eventos: eliminar", "Gestión de eventos: suspender", "Gestión de eventos: preventa", "Gestión de eventos: sector" });
            accion.SelectedIndex = 0;
            buscar.Click += (s, e) => Cargar();
            Shown += (s, e) => Cargar();
        }

        private void Cargar()
        {
            tabla.DataSource = GestorAuditoria.Instance.Consultar(desde.Value.Date, hasta.Value.Date.AddDays(1), accion.SelectedIndex <= 0 ? null : Convert.ToString(accion.SelectedItem));
        }
    }
}
