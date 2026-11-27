using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    partial class frmAuditoria
    {
        private DateTimePicker desde;
        private DateTimePicker hasta;
        private ComboBox accion;
        private Button buscar;
        private DataGridView tabla;

        private void InitializeComponent()
        {
            desde = new DateTimePicker { Name = "desdeAuditoria", Format = DateTimePickerFormat.Short, Location = new Point(68, 12), Size = new Size(120, 24) };
            hasta = new DateTimePicker { Name = "hastaAuditoria", Format = DateTimePickerFormat.Short, Location = new Point(250, 12), Size = new Size(120, 24) };
            accion = new ComboBox { Name = "filtroAccion", DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(390, 12), Size = new Size(300, 24), Anchor = AnchorStyles.Top | AnchorStyles.Left };
            buscar = new Componentes.BotonPrimario { Name = "buscarAuditoria", Text = "Buscar", Location = new Point(710, 8), Size = new Size(120, 32), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            tabla = new DataGridView { Name = "tablaAuditoria", ReadOnly = true, AllowUserToAddRows = false, AutoGenerateColumns = true, Location = new Point(12, 52), Size = new Size(876, 456), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.AddRange(new Control[] { desde, hasta, accion, buscar, tabla });
            Text = "Auditoría";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1000, 680);
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }
    }
}
