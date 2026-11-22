using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public sealed class frmSolicitudOrganizador : FormularioEventAura
    {
        private readonly TextBox responsable = Campo();
        private readonly TextBox cuit = Campo();
        private readonly TextBox telefono = Campo();
        private readonly TextBox codigo = Campo();
        private readonly TextBox local = Campo();
        private readonly TextBox direccion = Campo();
        private readonly NumericUpDown capacidad = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 500 };
        private readonly TextBox cbu = Campo();
        private readonly Label archivo = new Label { AutoSize = true, Text = "No seleccionaste documentación." };
        private byte[] imagen;
        private string nombreArchivo;
        private readonly Componentes.EtiquetaAyuda estadoEnvioTelefono = new Componentes.EtiquetaAyuda { Name="estadoEnvioTelefono", Location=new Point(24,254), Size=new Size(820,36) };

        public frmSolicitudOrganizador()
        {
            GestorSesion.Instance.RequerirUsuario(GestorSesion.Instance.UsuarioActual?.Id ?? 0);
            responsable.Name = "responsable"; cuit.Name = "cuit"; codigo.Name = "codigo";
            local.Name = "local"; direccion.Name = "direccion"; cbu.Name = "cbu"; capacidad.Name = "capacidad";
            telefono.Name = "telefono";
            if (GestorSesion.Instance.UsuarioActual.TipoUsuario != "Cliente")
                throw new ReglaNegocioException("RN-50", "La solicitud de Organizador está disponible para cuentas Cliente.");
            Text = "Solicitar perfil de Organizador"; StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(900, 680); MinimumSize = new Size(900, 680); MaximizeBox = false;
            var titulo = new Componentes.EtiquetaTitulo { Text = "Solicitá ser Organizador", Location = new Point(24, 18), Size = new Size(500, 40) };
            Controls.Add(titulo);
            Agregar("Responsable · nombre completo", responsable, 24, 78);
            Agregar("CUIT (11 dígitos)", cuit, 455, 78);
            Agregar("Teléfono argentino", telefono, 24, 148);
            Agregar("Código de verificación", codigo, 455, 148);
            var enviar = new Componentes.BotonSecundario { Name = "btnEnviarCodigoTelefono", Text = "Enviar código", Location = new Point(455, 210), Size = new Size(150, 38) };
            var confirmar = new Componentes.BotonSecundario { Text = "Confirmar teléfono", Location = new Point(615, 210), Size = new Size(190, 38) };
            enviar.Click += (s,e)=>EnviarCodigo(enviar);
            confirmar.Click += (s,e)=>Ejecutar(()=>GestorSolicitudesOrganizador.Instance.ConfirmarCodigoTelefono(telefono.Text,codigo.Text), "Teléfono verificado.", confirmar);
            Controls.AddRange(new Control[] { enviar, confirmar, estadoEnvioTelefono });
            Agregar("Nombre del local", local, 24, 298);
            Agregar("Dirección", direccion, 455, 298);
            Agregar("Capacidad", capacidad, 24, 368);
            Agregar("CBU de 22 dígitos o alias", cbu, 455, 368);
            var adjuntar = new Componentes.BotonSecundario { Text = "Adjuntar documentación", Location = new Point(24, 442), Size = new Size(300, 38) };
            archivo.Location = new Point(340, 452); archivo.Size = new Size(500, 24);
            adjuntar.Click += Adjuntar;
            var nota = new Componentes.EtiquetaAyuda { Text = "La identidad externa se simula por formato; la aprobación final la realiza una persona administradora.", Location = new Point(24, 504), Size = new Size(820, 50) };
            var enviarSolicitud = new Componentes.BotonPrimario { Text = "Enviar solicitud", Location = new Point(24, 576), Size = new Size(210, 44) };
            enviarSolicitud.Click += (s,e)=>CrearSolicitud();
            var cancelar = new Componentes.BotonSecundario { Text = "Cerrar", Location = new Point(250, 576), Size = new Size(120, 44) };
            cancelar.Click += (s,e)=>Close();
            Controls.AddRange(new Control[] { adjuntar, archivo, nota, enviarSolicitud, cancelar });
        }

        private void EnviarCodigo(Button boton)
        {
            try
            {
                using (Avisos.Esperar(this, boton))
                    GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono(telefono.Text);
                estadoEnvioTelefono.Text = "Si el teléfono es válido, guardamos un SMS simulado fuera de EventAura. Abrí el archivo del mensaje para continuar.";
            }
            catch (ReglaNegocioException ex) { estadoEnvioTelefono.Text = ex.Message; }
            catch (Exception) { estadoEnvioTelefono.Text = "No pudimos enviar el código. Revisá la conexión o la configuración de correo y volvé a intentarlo."; }
        }

        private static TextBox Campo() => new TextBox { Width = 380 };
        private void Agregar(string etiqueta, Control control, int x, int y)
        {
            Controls.Add(new Componentes.EtiquetaAyuda { Text = etiqueta, Location = new Point(x, y), Size = new Size(400, 24) });
            control.Location = new Point(x, y + 27); control.Size = new Size(400, 30); Controls.Add(control);
        }
        private void Adjuntar(object sender, EventArgs e)
        {
            using (var dialogo = new OpenFileDialog { Filter = "Imágenes PNG o JPEG|*.png;*.jpg;*.jpeg", Title = "Elegí la documentación" })
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                try { using (var stream = File.OpenRead(dialogo.FileName)) imagen = ImagenEventoCodec.Preparar(stream); nombreArchivo = Path.GetFileName(dialogo.FileName); archivo.Text = "Documentación adjunta y validada."; }
            catch (Exception ex) { imagen = null; Avisos.Error(this, ManejadorErrores.MensajeParaUsuario(ex)); }
            }
        }
        private void CrearSolicitud()
        {
            try
            {
                GestorSolicitudesOrganizador.Instance.Solicitar(new SolicitudOrganizador { Responsable=responsable.Text, Cuit=cuit.Text, Telefono=telefono.Text,
                    NombreLocal=local.Text, Direccion=direccion.Text, Capacidad=(int)capacidad.Value, CbuAlias=cbu.Text, Documentacion=imagen, NombreArchivo=nombreArchivo });
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { Avisos.Error(this, ManejadorErrores.MensajeParaUsuario(ex)); }
        }
        private void Ejecutar(Action accion, string correcto, Control boton)
        {
            try { using (Avisos.Esperar(this, boton)) accion(); Avisos.Informativo(this, correcto); }
            catch (Exception ex) { Avisos.Error(this, ManejadorErrores.MensajeParaUsuario(ex)); }
        }
    }
}
