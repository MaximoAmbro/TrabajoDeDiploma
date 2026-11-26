using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Visual.Web;

namespace Visual
{
    // La bienvenida es una vista nueva. Si WebView2 falta o no inicia, el acceso WinForms
    // sigue siendo la entrada de respaldo y la aplicación no queda en una ventana vacía.
    public sealed class frmBienvenida : FormularioEventAura
    {
        private readonly PanelWeb vista = new PanelWeb { Name = "bienvenidaWeb", Dock = DockStyle.Fill };
        private string avisoBase;
        private bool bloquear;

        public frmBienvenida()
        {
            Name = "frmBienvenida";
            Text = "EventAura · Bienvenida";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1100, 720);
            MinimumSize = new Size(1000, 680);
            Controls.Add(vista);
            vista.VistaNoDisponible += AbrirAcceso;
            vista.Mensaje += ProcesarMensaje;
            vista.Escape += AbrirAcceso;
            Shown += Iniciar;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Padding = Padding.Empty;
            var barra = Controls.OfType<Componentes.BarraSuperior>().FirstOrDefault();
            if (barra != null) barra.Visible = false;
        }

        private void Iniciar(object sender, EventArgs e)
        {
            Tuple<string, string> faltante = null;
            using (var acceso = new frmInicio())
            {
                try { faltante = acceso.VerificarEsquema(); }
                catch (Exception error)
                {
                    System.Diagnostics.Trace.WriteLine(error);
                    avisoBase = frmInicio.MensajeBaseNoDisponible();
                    bloquear = true;
                }
            }
            // Una base existente pero desactualizada conserva el aviso detallado de T-21.
            if (faltante != null) { AbrirAcceso(); return; }
            vista.Actualizar(new { aviso = avisoBase, bloquear });
            vista.Cargar("bienvenida.html");
        }

        private void ProcesarMensaje(string json)
        {
            Dictionary<string, object> mensaje;
            try { mensaje = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json); }
            catch { return; }
            object valor;
            string tipo = mensaje.TryGetValue("tipo", out valor) ? Convert.ToString(valor) : string.Empty;
            if (tipo == "escape") { AbrirAcceso(); return; }
            if (tipo != "navegar") return;
            string destino = mensaje.TryGetValue("destino", out valor) ? Convert.ToString(valor) : string.Empty;
            if (destino == "acceso") AbrirAcceso();
            else if (destino == "alta") Abrir(new frmCrearUsuario());
            else if (destino == "recuperar") Abrir(new frmCambiarContraseña(true));
        }

        private void AbrirAcceso()
        {
            Abrir(new frmInicio());
        }

        private void Abrir(Form formulario)
        {
            formulario.Show();
            Hide();
        }
    }
}
