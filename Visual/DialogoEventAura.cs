using System;
using System.Drawing;
using System.Windows.Forms;

namespace Visual
{
    /// <summary>Diálogo modal propio con el tema de EventAura y navegación de teclado.</summary>
    public sealed class DialogoEventAura : Form
    {
        public string TituloDialogo { get; }
        public string MensajeDialogo { get; }

        private DialogoEventAura(string titulo, string mensaje, string confirmar, string volver,
            bool peligro, DialogResult predeterminado)
        {
            TituloDialogo = titulo ?? "EventAura";
            MensajeDialogo = mensaje ?? string.Empty;
            Name = "dialogoEventAura";
            Text = TituloDialogo;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(480, 230);
            BackColor = Tema.Superficie;
            ForeColor = Tema.Texto;
            Font = Tema.Cuerpo;
            KeyPreview = true;

            var tituloLabel = new Label { Name = "dialogoTitulo", Text = TituloDialogo,
                Font = Tema.Titulo, ForeColor = Tema.Texto, AutoSize = false,
                Location = new Point(28, 24), Size = new Size(424, 42) };
            var mensajeLabel = new Label { Name = "dialogoMensaje", Text = MensajeDialogo,
                Font = Tema.Cuerpo, ForeColor = Tema.Texto, AutoSize = false,
                Location = new Point(28, 76), Size = new Size(424, 82) };
            var acciones = new FlowLayoutPanel { Name = "dialogoAcciones", FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false, Location = new Point(28, 176), Size = new Size(424, 40), BackColor = Tema.Superficie };
            var aceptar = new Button { Name = "dialogoConfirmar", Text = confirmar ?? "Confirmar", Size = new Size(190, 40),
                DialogResult = DialogResult.Yes, Font = Tema.Boton, BackColor = peligro ? Tema.Error : Tema.Primario,
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false };
            aceptar.FlatAppearance.BorderColor = peligro ? Tema.Error : Tema.Primario;
            var cancelar = new Button { Name = "dialogoVolver", Text = volver ?? "Volver", Size = new Size(150, 40),
                DialogResult = DialogResult.No, Font = Tema.Boton, BackColor = Tema.Superficie,
                ForeColor = Tema.Texto, FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false };
            cancelar.FlatAppearance.BorderColor = Tema.BordeControl;
            acciones.Controls.Add(aceptar);
            acciones.Controls.Add(cancelar);
            Controls.AddRange(new Control[] { tituloLabel, mensajeLabel, acciones });
            AcceptButton = predeterminado == DialogResult.No ? cancelar : aceptar;
            CancelButton = cancelar;
            ActiveControl = AcceptButton as Control;
        }

        public static DialogResult Confirmar(IWin32Window propietario, string titulo, string mensaje,
            string confirmar = "Confirmar", string volver = "Volver", bool peligro = false,
            DialogResult predeterminado = DialogResult.No)
        {
            using (var dialogo = new DialogoEventAura(titulo, mensaje, confirmar, volver, peligro, predeterminado))
                return propietario == null ? dialogo.ShowDialog() : dialogo.ShowDialog(propietario);
        }

        public static void Informar(IWin32Window propietario, string titulo, string mensaje)
        {
            using (var dialogo = new DialogoEventAura(titulo, mensaje, "Aceptar", null, false, DialogResult.OK))
                if (propietario == null) dialogo.ShowDialog(); else dialogo.ShowDialog(propietario);
        }
    }
}
