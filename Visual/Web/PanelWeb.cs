using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Visual.Web
{
    public sealed class PanelWeb : UserControl
    {
        private object motor;
        private object datos;
        private bool fallida;
        private readonly Label estadoCarga = new Label
        {
            Name = "estadoCargaVistaWeb",
            Dock = DockStyle.Fill,
            Text = "Cargando vista…",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Tema.TextoSecundario,
            BackColor = Tema.Superficie
        };

        public bool Preparado { get; private set; }
        public bool InicializacionTerminada => Preparado || fallida;

        public Task CapturarAsync(Stream imagen)
        {
            try { return (Task)motor?.GetType().GetMethod("CapturarAsync").Invoke(motor, new object[] { imagen }); }
            catch (Exception error) { Desactivar(error); return Task.CompletedTask; }
        }
        public event Action VistaLista;
        public event Action VistaNoDisponible;
        public event Action<string> Mensaje;
        public event Action Escape;

        public PanelWeb()
        {
            Dock = DockStyle.Fill;
            BackColor = Tema.Superficie;
            TabStop = false;
            Controls.Add(estadoCarga);
        }

        public void Cargar(string archivo)
        {
            if (motor != null || fallida) return;
            Visible = true;
            estadoCarga.Visible = true;
            estadoCarga.BringToFront();
            try
            {
                string ensamblado = typeof(PanelWeb).Assembly.GetName().Name;
                var tipo = Type.GetType("Visual.Web.PanelWebMotor, " + ensamblado, true);
                motor = Activator.CreateInstance(tipo);
                var control = (Control)motor;
                control.Dock = DockStyle.Fill;
                Controls.Add(control);
                control.BringToFront();
                Vincular("VistaLista", _ =>
                {
                    Preparado = true;
                    estadoCarga.Dispose();
                    VistaLista?.Invoke();
                });
                Vincular("VistaNoDisponible", _ => Desactivar(null));
                Vincular("Mensaje", valor => Mensaje?.Invoke((string)valor));
                Vincular("Escape", _ => Escape?.Invoke());
                if (datos != null) tipo.GetMethod("Actualizar").Invoke(motor, new[] { datos });
                tipo.GetMethod("Cargar").Invoke(motor, new object[] { archivo });
            }
            catch (Exception error)
            {
                Desactivar(error);
            }
        }

        public void Actualizar(object modelo)
        {
            datos = modelo;
            if (motor == null || fallida) return;
            try { motor.GetType().GetMethod("Actualizar").Invoke(motor, new[] { modelo }); }
            catch (Exception error) { Desactivar(error); }
        }

        internal void SimularFallaPosteriorALaCarga()
        {
            if (!Preparado || motor == null) return;
            try { motor.GetType().GetMethod("Desactivar", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(motor, null); }
            catch (Exception error) { Desactivar(error); }
        }

        private void Vincular(string nombre, Action<object> accion)
        {
            motor.GetType().GetEvent(nombre).AddEventHandler(motor, accion);
        }

        private void Desactivar(Exception error)
        {
            if (IsDisposed || fallida) return;
            if (error != null) System.Diagnostics.Trace.WriteLine(error);
            fallida = true;
            Preparado = false;
            Hide();
            VistaNoDisponible?.Invoke();
        }
    }
}
