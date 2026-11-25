using Entidades;
using Negocio;
using System;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Visual
{
    public partial class frmInicio : FormularioEventAura
    {
        internal Func<Tuple<string, string>> VerificarEsquema = VerificarConDatos;
        private readonly Web.PanelWeb _accesoWeb = new Web.PanelWeb { Name = "accesoWeb", Visible = false };
        private readonly Label avisoAccesoWeb = new Label
        {
            Name = "avisoAccesoWeb",
            Text = "La vista web no está disponible. Podés iniciar sesión con este formulario.",
            ForeColor = Tema.TextoSecundario,
            BackColor = Tema.Superficie,
            Font = Tema.Ayuda,
            Location = new Point(24, 448),
            Size = new Size(392, 32),
            Visible = false
        };
        private string _avisoBase;
        private Visual.Componentes.EtiquetaAyuda _avisoBaseControl;
        private Padding _paddingRespaldo;
        private bool _esquemaDesactualizado;
        // La cadena de conexión la lee Datos/Conexion.cs de App.config: Visual no abre conexiones.
        public frmInicio()
        {
            InitializeComponent();
            Controls.Add(avisoAccesoWeb);
            Controls.Add(_accesoWeb);
            _accesoWeb.Dock = DockStyle.Fill;
            _accesoWeb.VistaLista += MostrarAccesoWeb;
            _accesoWeb.VistaNoDisponible += MostrarRespaldoAcceso;
            _accesoWeb.Mensaje += ProcesarMensajeAcceso;
            _accesoWeb.Escape += MostrarBienvenida;
            Shown += (s, e) => { if (!_esquemaDesactualizado) _accesoWeb.Cargar("acceso.html"); };
            label2.Font = Tema.N2;
            label5.Font = label3.Font = Tema.N5Fuerte;
            txtMailLogin.Font = txtContraseñaLogin.Font = Tema.N4;
            checkbxShowPass.Font = lblolividarcontra.Font = lblcrearcuenta.Font = Tema.N5;
            btninicio.Font = Tema.N4Fuerte;
            Tema.ComoEnlace(lblolividarcontra);
            Tema.ComoEnlace(lblcrearcuenta);
            txtMailLogin.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtMailLogin);
            txtContraseñaLogin.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtContraseñaLogin);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _paddingRespaldo = Padding;
            Padding = Padding.Empty;
        }
        
        private void frmInicio_Load(object sender, EventArgs e)
        {
            ColocarTitulo(label2);
            txtMailLogin.Left = txtContraseñaLogin.Left = checkbxShowPass.Left = Disposicion.Pixeles(this, Tema.Espacio.XXL);
            ColocarCampo(label5, txtMailLogin);
            ColocarCampo(label3, txtContraseñaLogin);
            btninicio.Left = txtMailLogin.Left;
            btninicio.Width = txtMailLogin.Width;
            int separacion = Disposicion.Pixeles(this, Tema.Espacio.S);
            int separacionAcciones = Disposicion.Pixeles(this, Tema.Espacio.L);
            checkbxShowPass.SetBounds(txtContraseñaLogin.Left, txtContraseñaLogin.Bottom + separacion,
                checkbxShowPass.Width, checkbxShowPass.Height);
            btninicio.Top = checkbxShowPass.Bottom + separacionAcciones;
            lblcrearcuenta.Top = lblolividarcontra.Top = btninicio.Bottom + separacionAcciones;
            int espacio = Disposicion.Pixeles(this, Tema.Espacio.S);
            int anchoEnlace = (txtMailLogin.Width - espacio) / 2;
            lblcrearcuenta.Left = txtMailLogin.Left;
            lblcrearcuenta.Width = lblolividarcontra.Width = anchoEnlace;
            lblolividarcontra.Left = lblcrearcuenta.Right + espacio;
            try
            {
                var faltante = VerificarEsquema();
                if (faltante != null)
                {
                    _esquemaDesactualizado = true;
                    MostrarEsquemaDesactualizado(faltante.Item1, faltante.Item2);
                    return;
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                _avisoBase = MensajeBaseNoDisponible();
                MostrarErrorDeBase(_avisoBase);
                _accesoWeb.Actualizar(new { estado = "sinBase", mensaje = _avisoBase });
            }
            
            txtMailLogin.Text = "";
            txtContraseñaLogin.Text = "";
            txtContraseñaLogin.UseSystemPasswordChar = true;
            txtMailLogin.Select();   // no depende del orden de TabIndex
            if (_avisoBase == null) _accesoWeb.Actualizar(new { estado = "listo" });
        }

        private void MostrarErrorDeBase(string mensaje = null)
        {
            txtMailLogin.Visible = label5.Visible = txtContraseñaLogin.Visible = label3.Visible = false;
            checkbxShowPass.Visible = lblolividarcontra.Visible = lblcrearcuenta.Visible = btninicio.Visible = false;
            btninicio.Enabled = false;
            if (_avisoBaseControl == null)
            {
                _avisoBaseControl = new Visual.Componentes.EtiquetaAyuda
                {
                    Name = "avisoBaseNoDisponible",
                    Location = new Point(24, 148),
                    Size = new Size(352, 128),
                    AccessibleRole = System.Windows.Forms.AccessibleRole.Alert
                };
                Controls.Add(_avisoBaseControl);
            }
            _avisoBaseControl.Text = mensaje ?? MensajeBaseNoDisponible();
            _avisoBaseControl.Visible = true;
            _avisoBaseControl.BringToFront();
        }

        internal static string MensajeBaseNoDisponible()
        {
            string destino = "No se pudo leer Visual.exe.config";
            try
            {
                var cadena = ConfigurationManager.ConnectionStrings["Visual.Properties.Settings.BddEventAura"];
                if (cadena != null)
                {
                    var valores = new DbConnectionStringBuilder { ConnectionString = cadena.ConnectionString };
                    object instancia, baseDatos;
                    valores.TryGetValue("Data Source", out instancia);
                    valores.TryGetValue("Initial Catalog", out baseDatos);
                    destino = "Instancia: " + (instancia ?? "no indicada") + " · Base: " + (baseDatos ?? "no indicada");
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
            }
            return "EventAura no pudo comprobar la base de datos. " + destino +
                   ". Revisá SQL Server y la configuración, y volvé a abrir la aplicación.";
        }

        private void MostrarAccesoWeb()
        {
            Padding = Padding.Empty;
            foreach (Control control in Controls)
                if (control != _accesoWeb) control.Visible = false;
            _accesoWeb.Visible = true;
            _accesoWeb.BringToFront();
        }

        private void MostrarRespaldoAcceso()
        {
            _accesoWeb.Visible = false;
            Padding = _paddingRespaldo;
            var barra = Controls.OfType<Componentes.BarraSuperior>().FirstOrDefault();
            if (barra != null) barra.Visible = true;
            if (_avisoBase != null)
            {
                MostrarErrorDeBase(_avisoBase);
                avisoAccesoWeb.Visible = true;
                avisoAccesoWeb.BringToFront();
                return;
            }
            foreach (Control control in Controls)
                if (control != _accesoWeb) control.Visible = true;
            avisoAccesoWeb.BringToFront();
        }

        private void MostrarBienvenida()
        {
            var bienvenida = new frmBienvenida();
            bienvenida.Show();
            Hide();
        }

        private void ProcesarMensajeAcceso(string json)
        {
            Dictionary<string, object> mensaje;
            try { mensaje = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json); }
            catch { return; }
            object valor;
            string tipo = mensaje.TryGetValue("tipo", out valor) ? Convert.ToString(valor) : string.Empty;
            if (tipo == "navegar")
            {
                string destino = mensaje.TryGetValue("destino", out valor) ? Convert.ToString(valor) : string.Empty;
                if (destino == "alta")
                {
                    new frmCrearUsuario().Show();
                    Hide();
                }
                else if (destino == "recuperar")
                {
                    string mail = mensaje.TryGetValue("mail", out valor) ? Convert.ToString(valor) : null;
                    var recuperar = new frmCambiarContraseña(true) { CorreoInicial = mail, Retorno = this };
                    recuperar.Show();
                    Hide();
                }
                else if (destino == "bienvenida") MostrarBienvenida();
                return;
            }
            if (tipo != "iniciarSesion") return;
            string correo = mensaje.TryGetValue("mail", out valor) ? Convert.ToString(valor) : string.Empty;
            string contrasena = mensaje.TryGetValue("contrasena", out valor) ? Convert.ToString(valor) : string.Empty;
            Usuario usuario;
            try
            {
                usuario = GestorUsuarios.Instance.Autenticar(correo, contrasena);
            }
            catch (ReglaNegocioException)
            {
                _accesoWeb.Actualizar(new { estado = "error", mensaje = GestorUsuarios.MensajeAccesoRechazado });
                return;
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                _accesoWeb.Actualizar(new { estado = "error", mensaje = ManejadorErrores.MensajeParaUsuario(error) });
                return;
            }
            if (usuario == null)
            {
                _accesoWeb.Actualizar(new { estado = "error", mensaje = GestorUsuarios.MensajeAccesoRechazado });
                return;
            }
            try { AbrirMenu(usuario); }
            catch (Exception error)
            {
                GestorSesion.Instance.Cerrar();
                System.Diagnostics.Trace.WriteLine(error);
                _accesoWeb.Actualizar(new { estado = "error", mensaje = ManejadorErrores.MensajeParaUsuario(error) });
            }
        }

        private static Tuple<string, string> VerificarConDatos()
        {
            var tipo = Type.GetType("Datos.VerificadorEsquema, Datos", true);
            var resultado = tipo.GetMethod("PrimerFaltante").Invoke(null, null);
            if (resultado == null) return null;
            var resultadoTipo = resultado.GetType();
            return Tuple.Create(
                Convert.ToString(resultadoTipo.GetProperty("Descripcion").GetValue(resultado, null)),
                Convert.ToString(resultadoTipo.GetProperty("Script").GetValue(resultado, null)));
        }

        private void MostrarEsquemaDesactualizado(string descripcion, string script)
        {
            txtMailLogin.Visible = label5.Visible = txtContraseñaLogin.Visible = label3.Visible = false;
            checkbxShowPass.Visible = lblolividarcontra.Visible = lblcrearcuenta.Visible = btninicio.Visible = false;
            btninicio.Enabled = false;
            var aviso = new Visual.Componentes.EtiquetaAyuda
            {
                Name = "avisoEsquemaDesactualizado",
                Text = "Tu base de datos está desactualizada: falta " + descripcion + ".\r\n\r\n" +
                       "Corré BaseDatos/" + script + " y volvé a abrir EventAura.",
                Location = new System.Drawing.Point(24, 148),
                Size = new System.Drawing.Size(352, 128),
                AccessibleRole = System.Windows.Forms.AccessibleRole.Alert
            };
            Controls.Add(aviso);
            aviso.BringToFront();
        }
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbxShowPass.Checked == true)
            {
                txtContraseñaLogin.UseSystemPasswordChar = false;
            }
            else
            {
                txtContraseñaLogin.UseSystemPasswordChar = true;
            }
        } //Le pone el asterisco a la contraseña

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void lblolividarcontra_Click(object sender, EventArgs e)
        {
            frmCambiarContraseña frm = new frmCambiarContraseña(true) { Retorno = this, CorreoInicial = txtMailLogin.Text };
            frm.Show();
            this.Hide();
        } // Llama frmCambiarContraseña
        private void lblcrearcuenta_Click(object sender, EventArgs e)
        {
            frmCrearUsuario frm = new frmCrearUsuario();
            frm.Show();
            this.Hide();
        } // Llama a frmCrearCuenta
        private void btninicio_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMailLogin.Text))
            {
                ValidacionCampo.Mostrar(txtMailLogin, "Escribí tu correo electrónico para continuar.");
                txtMailLogin.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtContraseñaLogin.Text))
            {
                ValidacionCampo.Mostrar(txtContraseñaLogin, "Escribí tu contraseña para continuar.");
                txtContraseñaLogin.Focus();
                return;
            }
            else
            {
                ValidarLista(txtMailLogin.Text, txtContraseñaLogin.Text);
            }
        } // Llamado a los menues

        // --------------------------------------FUNCIONES DE CONTROL----------------------------------------------
        public void ValidarLista(string mail, string contraseña)
        {
            Usuario usuario;
            try
            {
                usuario = GestorUsuarios.Instance.Autenticar(mail, contraseña);
            }
            catch (ReglaNegocioException ex)
            {
                // RN-04: rechazo uniforme; el bloqueo se conserva sin revelar datos de cuenta.
                Avisos.Regla("Inicio de sesión", ex);
                return;
            }

            if (usuario != null)
            {
                AbrirMenu(usuario);
            }
            else
            {
                Avisos.Regla("Inicio de sesión", new ReglaNegocioException("RN-04", GestorUsuarios.MensajeAccesoRechazado));
            }
        }

        private void AbrirMenu(Usuario usuario)
        {
            if (usuario.TipoUsuario == "Cliente")
            {
                var avisos = GestorListaEspera.Instance.AvisosAlIngresar(usuario.Id);
                var menu = new frmMenuCliente { IDUsuario = usuario.Id, NombreUsuario = usuario.Nombre };
                menu.Show();
                if (avisos.Count > 0)
                    Avisos.Informativo(menu, "Hay lugares disponibles en " + string.Join(", ", avisos) +
                        ". Podés consultarlos en eventos; el aviso no reserva un lugar.");
                Hide();
            }
            else if (usuario.TipoUsuario == "Organizador")
            {
                var menu = new frmMenuPropietario { IDUsuario = usuario.Id, NombreUsuario = usuario.Nombre };
                menu.Show();
                Hide();
            }
            else if (usuario.TipoUsuario == "Administrador")
            {
                var menu = new frmMenuAdministrador { IDUsuario = usuario.Id, NombreUsuario = usuario.Nombre };
                menu.Show();
                Hide();
            }
        }
    } // Valida la lista de usuarios

}
