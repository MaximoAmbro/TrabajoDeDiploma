using System;
using System.Drawing;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public sealed partial class frmAuditoria : FormularioEventAura
    {
        private Componentes.EstadoVacio estadoVacio;
        public frmAuditoria()
        {
            InitializeComponent();
            desde.Value = DateTime.Today.AddDays(-30);
            hasta.Value = DateTime.Today;
            accion.Items.AddRange(new object[] { "Todas", "Inicio de sesión", "Cambio de contraseña", "Compra iniciada", "Compra confirmada", "Devolución", "Control de ingreso", "Gestión de eventos: crear", "Gestión de eventos: publicar", "Gestión de eventos: eliminar", "Gestión de eventos: suspender", "Gestión de eventos: preventa", "Gestión de eventos: sector" });
            accion.SelectedIndex = 0;
            buscar.Click += (s, e) => Cargar();
            Shown += (s, e) => Cargar();
            estadoVacio = new Componentes.EstadoVacio { Name = "estadoSinAuditoria", Visible = false };
            estadoVacio.AjustarA(tabla);
            estadoVacio.Cargar("No encontramos movimientos con estos filtros. Probá ampliar el período.", "Ampliar período");
            estadoVacio.Accion += () => { desde.Value = DateTime.Today.AddYears(-1); accion.SelectedIndex = 0; Cargar(); };
            Controls.Add(estadoVacio);
        }

        private void Cargar()
        {
            var filas = GestorAuditoria.Instance.Consultar(desde.Value.Date, hasta.Value.Date.AddDays(1), accion.SelectedIndex <= 0 ? null : Convert.ToString(accion.SelectedItem));
            tabla.DataSource = filas;
            tabla.Visible = filas.Count > 0;
            estadoVacio.Visible = filas.Count == 0;
        }
    }
}
