using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using Entidades;

namespace Visual
{
    public partial class frmCrearUsuario : FormularioEventAura
    {
        private readonly bool _altaAdministrador;
        private Componentes.IndicadorFortaleza _fortaleza;
        private readonly Web.PanelWeb _altaWeb = new Web.PanelWeb { Name = "altaCuentaWeb", Visible = false };
        private string _mailPendiente;
        private string _contrasenaPendiente;
        private string _perfilPendiente;
        private string _telefonoOrganizador;
        private bool _verificacionSesion;
        private DateTime _proximoReenvioCorreo;
        private Action _alCompletarVerificacionSesion;
        public frmCrearUsuario() : this(false) { }
        public frmCrearUsuario(bool altaAdministrador)
        {
            _altaAdministrador = altaAdministrador;
            InitializeComponent();
            if (!_altaAdministrador)
            {
                MinimumSize = new Size(1000, 680);
                Controls.Add(_altaWeb);
                _altaWeb.SetBounds(0, barraSuperior.Bottom, ClientSize.Width, ClientSize.Height - barraSuperior.Bottom);
                _altaWeb.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                barraSuperior.BringToFront();
                _altaWeb.VistaLista += MostrarAltaWeb;
                _altaWeb.VistaNoDisponible += () => { _altaWeb.Visible = false; MostrarRespaldoAlta(); };
                _altaWeb.Mensaje += ProcesarMensajeAlta;
                _altaWeb.Escape += () => { if (_verificacionSesion) VolverDeVerificacionSesion(); else VolverAlAcceso(); };
                _altaWeb.VistaLista += () =>
                {
                    if (_verificacionSesion)
                    {
                        _altaWeb.Actualizar(EstadoPasoCorreo());
                    }
                    else if (string.IsNullOrEmpty(_perfilPendiente))
                    {
                        _perfilPendiente = "cliente";
                        _altaWeb.Actualizar(new { paso = "elegir", parametros = new { longitudMinima = GestorParametros.Instance.ObtenerEntero("PWD_LONGITUD_MIN"), fortalezaMinima = GestorParametros.Instance.ObtenerEntero("PWD_FORTALEZA_MIN") } });
                    }
                };
                Shown += (s, e) => _altaWeb.Cargar("alta-cuenta.html");
            }
            barraSuperior.MostrarSesion = altaAdministrador;
            _fortaleza = new Componentes.IndicadorFortaleza { Name = "fortalezaContrasena", Location = new Point(24, 348), Size = new Size(392, 24) };
            Controls.Add(_fortaleza);
            TxtPassword.TextChanged += (s, e) => ActualizarFortaleza();
            txtNombre.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtNombre);
            txtApellido.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtApellido);
            txtmail.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtmail);
            TxtPassword.TextChanged += (s, e) => ValidacionCampo.Limpiar(TxtPassword);
            TxtConfirmpassword.TextChanged += (s, e) => ValidacionCampo.Limpiar(TxtConfirmpassword);
            Shown += (s, e) =>
            {
                ColocarTitulo(label1);
                label1.Left = (ClientSize.Width - label1.Width) / 2;
                int Px(int medida) => Disposicion.Pixeles(this, medida);
                ClientSize = new Size(Math.Max(1100, ClientSize.Width), Math.Max(720, ClientSize.Height));
                int izquierda = Math.Max(Px(Tema.Espacio.XXL), (ClientSize.Width - Px(2 * Tema.AnchoCampo + Tema.Espacio.L)) / 2);
                int derecha = izquierda + Px(Tema.AnchoCampo + Tema.Espacio.L);
                txtNombre.Left = txtmail.Left = TxtPassword.Left = izquierda;
                txtApellido.Left = TxtConfirmpassword.Left = derecha;
                ColocarCampo(label6, txtNombre);
                ColocarCampo(label5, txtApellido);
                ColocarCampo(lblmail, txtmail);
                txtmail.Width = Px(2 * Tema.AnchoCampo + Tema.Espacio.L);
                ColocarCampo(label3, TxtPassword);
                ColocarCampo(label4, TxtConfirmpassword);
                _fortaleza.SetBounds(TxtPassword.Left, TxtPassword.Bottom + Px(Tema.Espacio.S), TxtPassword.Width, Px(Tema.Espacio.XL));
                checkbxShowPass.Location = new Point(TxtPassword.Left, _fortaleza.Bottom + Px(Tema.Espacio.L));
                btnregistrarse.SetBounds(TxtPassword.Left, checkbxShowPass.Bottom + Px(Tema.Espacio.XL),
                    TxtPassword.Width, Px(Tema.AltoBoton));
                lblyatengocuen.Location = new Point(TxtConfirmpassword.Left, btnregistrarse.Top);
                ClientSize = new Size(ClientSize.Width, Math.Max(720, btnregistrarse.Bottom + Px(Tema.Espacio.XXL)));
            };
            if (_altaAdministrador)
            {
                Text = "EventAura · Crear administrador";
                label1.Text = "Crear administrador";
                btnregistrarse.Text = "Crear administrador";
                lblyatengocuen.Text = "Volver a administración";
            }
            Tema.ComoEnlace(lblyatengocuen);
        }

        public void ConfigurarVerificacionSesion(int idUsuario, string correo, Action alCompletar)
        {
            if (_altaAdministrador) throw new InvalidOperationException("La verificación de correo no está disponible en este formulario.");
            _verificacionSesion = true;
            _mailPendiente = (correo ?? string.Empty).Trim();
            _perfilPendiente = "cliente";
            _contrasenaPendiente = null;
            _alCompletarVerificacionSesion = alCompletar;
            GestorCodigosCorreo.Instance.EnviarVerificacion(idUsuario, _mailPendiente);
            _proximoReenvioCorreo = DateTime.UtcNow.AddSeconds(30);
        }

        private object EstadoPasoCorreo(string error = null)
        {
            int restantes = Math.Max(0, (int)Math.Ceiling((_proximoReenvioCorreo - DateTime.UtcNow).TotalSeconds));
            string ayuda = Negocio.SeleccionEnviadorCodigos.Actual is EnviadorSimulado
                ? "Abrí la carpeta BandejaSimulada (" + EnviadorSimulado.RutaBandejaActual + ") y buscá el archivo más reciente."
                : "Revisá tu correo para encontrar el código de verificación más reciente.";
            return new
            {
                paso = "correo",
                perfil = "cliente",
                verificacionSesion = _verificacionSesion,
                mail = _mailPendiente,
                ayudaBandeja = _verificacionSesion ? ayuda : null,
                reenviarRestante = _verificacionSesion || _proximoReenvioCorreo > DateTime.UtcNow ? (int?)restantes : null,
                error = error == null ? null : new { mensaje = error }
            };
        }

        private void VolverDeVerificacionSesion()
        {
            if (NavegacionCliente.Volver()) return;
            Close();
        }
        private void MostrarAltaWeb()
        {
            foreach (Control control in Controls)
                if (control != barraSuperior && control != _altaWeb) control.Visible = false;
            _altaWeb.Visible = true;
            _altaWeb.BringToFront();
            barraSuperior.BringToFront();
        }

        private void MostrarRespaldoAlta()
        {
            if (_verificacionSesion)
            {
                MostrarVerificacionCorreo(_mailPendiente, true);
                return;
            }
            foreach (Control control in Controls)
                if (control != barraSuperior && control != _altaWeb) control.Visible = true;
            barraSuperior.BringToFront();
        }

        private void VolverAlAcceso()
        {
            _contrasenaPendiente = null;
            GestorSesion.Instance.Cerrar();
            var acceso = Cerrar();
            if (acceso != null) acceso.Show();
        }

        private void ProcesarMensajeAlta(string json)
        {
            JObject mensaje;
            try { mensaje = JObject.Parse(json); }
            catch { return; }
            string tipo = (string)mensaje["tipo"];
            try
            {
                if (tipo == "navegar") { if (_verificacionSesion) VolverDeVerificacionSesion(); else VolverAlAcceso(); return; }
                if (tipo == "cancelarVerificacion") { VolverDeVerificacionSesion(); return; }
                if (tipo == "reenviarCorreo")
                {
                    int id = _verificacionSesion ? (GestorSesion.Instance.UsuarioActual?.Id ?? 0) : 0;
                    if (_verificacionSesion && id <= 0) throw new InvalidOperationException("La sesión ya no está disponible. Volvé a iniciar sesión.");
                    if (DateTime.UtcNow < _proximoReenvioCorreo)
                    {
                        _altaWeb.Actualizar(EstadoPasoCorreo("Esperá " + Math.Max(1, (int)Math.Ceiling((_proximoReenvioCorreo - DateTime.UtcNow).TotalSeconds)) + " segundos antes de pedir otro código."));
                        return;
                    }
                    if (_verificacionSesion) GestorCodigosCorreo.Instance.EnviarVerificacion(id, _mailPendiente);
                    else
                    {
                        var usuario = GestorUsuarios.Instance.Autenticar(_mailPendiente, _contrasenaPendiente ?? string.Empty);
                        GestorCodigosCorreo.Instance.EnviarVerificacion(usuario.Id, _mailPendiente);
                    }
                    _proximoReenvioCorreo = DateTime.UtcNow.AddSeconds(30);
                    _altaWeb.Actualizar(EstadoPasoCorreo());
                    return;
                }
                if (tipo == "registrar")
                {
                    var cuenta = mensaje["cuenta"];
                    _mailPendiente = ((string)cuenta["mail"] ?? string.Empty).Trim();
                    _contrasenaPendiente = (string)cuenta["contrasena"];
                    _perfilPendiente = (string)mensaje["perfil"] ?? "cliente";
                    GestorUsuarios.Instance.RegistrarCliente(_mailPendiente, _contrasenaPendiente,
                        (string)cuenta["nombre"], (string)cuenta["apellido"]);
                    _proximoReenvioCorreo = DateTime.UtcNow.AddSeconds(30);
                    _altaWeb.Actualizar(EstadoPasoCorreo());
                    return;
                }
                if (tipo == "confirmarCorreo")
                {
                    GestorCodigosCorreo.Instance.ConfirmarVerificacion(_mailPendiente, (string)mensaje["codigo"]);
                    if (_verificacionSesion)
                    {
                        _altaWeb.Actualizar(new { paso = "listo", resultado = new { perfil = "cliente", verificacionSesion = true } });
                        var completar = _alCompletarVerificacionSesion;
                        if (completar != null && IsHandleCreated) BeginInvoke(completar);
                        return;
                    }
                    if (_perfilPendiente != "organizador")
                    {
                        _contrasenaPendiente = null;
                        _altaWeb.Actualizar(new { paso = "listo", perfil = "cliente", resultado = new { perfil = "cliente" } });
                        return;
                    }
                    Usuario usuario;
                    try { usuario = GestorUsuarios.Instance.Autenticar(_mailPendiente, _contrasenaPendiente ?? string.Empty); }
                    catch (ReglaNegocioException) { usuario = null; }
                    _contrasenaPendiente = null;
                    if (usuario == null)
                    {
                        _altaWeb.Actualizar(new { paso = "listo", perfil = "cliente", resultado = new { perfil = "cliente" } });
                        return;
                    }
                    _altaWeb.Actualizar(new { paso = "telefono", perfil = "organizador" });
                    return;
                }
                if (tipo == "enviarCodigoTelefono")
                {
                    _telefonoOrganizador = GestorSolicitudesOrganizador.NormalizarTelefonoArgentino((string)mensaje["telefono"]);
                    GestorSolicitudesOrganizador.Instance.EnviarCodigoTelefono(_telefonoOrganizador);
                    _altaWeb.Actualizar(new { paso = "telefono", perfil = "organizador", envioTelefono = "Si el teléfono es válido, guardamos un SMS simulado fuera de EventAura. Abrí el archivo del mensaje para continuar." });
                    return;
                }
                if (tipo == "confirmarTelefono")
                {
                    _telefonoOrganizador = GestorSolicitudesOrganizador.NormalizarTelefonoArgentino((string)mensaje["telefono"]);
                    GestorSolicitudesOrganizador.Instance.ConfirmarCodigoTelefono(_telefonoOrganizador, (string)mensaje["codigo"]);
                    _altaWeb.Actualizar(new { paso = "telefono", perfil = "organizador", telefonoVerificado = true });
                    return;
                }
                if (tipo == "presentarSolicitud")
                {
                    var datos = mensaje["solicitud"];
                    byte[] original = Convert.FromBase64String((string)datos["documentacion"]["base64"]);
                    byte[] comprimida;
                    using (var stream = new MemoryStream(original)) comprimida = ImagenEventoCodec.Preparar(stream);
                    GestorSolicitudesOrganizador.Instance.Solicitar(new SolicitudOrganizador
                    {
                        Responsable = (string)datos["responsable"], Cuit = (string)datos["cuit"],
                        Telefono = _telefonoOrganizador, NombreLocal = (string)datos["nombreLocal"],
                        Direccion = (string)datos["direccion"], Capacidad = (int?)datos["capacidad"] ?? 0,
                        CbuAlias = (string)datos["cbuAlias"], NombreArchivo = (string)datos["documentacion"]["nombreArchivo"],
                        Documentacion = comprimida
                    });
                    GestorSesion.Instance.Cerrar();
                    _altaWeb.Actualizar(new { paso = "listo", perfil = "organizador", resultado = new { perfil = "organizador" } });
                    return;
                }
            }
            catch (ReglaNegocioException ex)
            {
                _altaWeb.Actualizar(tipo == "confirmarCorreo" || tipo == "reenviarCorreo"
                    ? EstadoPasoCorreo(ex.Message)
                    : (object)new { paso = tipo == "registrar" ? "datos" : "telefono", perfil = _perfilPendiente ?? "cliente", error = new { mensaje = ex.Message } });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                _altaWeb.Actualizar(tipo == "confirmarCorreo" || tipo == "reenviarCorreo"
                    ? EstadoPasoCorreo(ManejadorErrores.MensajeParaUsuario(ex))
                    : (object)new { paso = tipo == "registrar" ? "datos" : "telefono", perfil = _perfilPendiente ?? "cliente", error = new { mensaje = ManejadorErrores.MensajeParaUsuario(ex) } });
            }
        }
        private void frmCrearUsuario_Load(object sender, EventArgs e)
        {
            txtmail.Text="";
            txtApellido.Clear();
            txtNombre.Clear();
            TxtPassword.Clear();
            TxtConfirmpassword.Clear();
            TxtPassword.UseSystemPasswordChar = true;
            TxtConfirmpassword.UseSystemPasswordChar = true;
            ActualizarFortaleza();
        }  // El Load del formulario
        private void ActualizarFortaleza()
        {
            if (_fortaleza == null) return;
            int minimo = GestorParametros.Instance.ObtenerEntero("PWD_FORTALEZA_MIN");
            int puntaje = GestorUsuarios.Instance.PuntajeFortaleza(TxtPassword.Text);
            _fortaleza.Actualizar(TxtPassword.Text, puntaje, minimo);
        }
        private void checkbxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if(checkbxShowPass.Checked == true)
            {
                TxtPassword.UseSystemPasswordChar = false;
                TxtConfirmpassword.UseSystemPasswordChar = false;
            }
            else
            {
                TxtPassword.UseSystemPasswordChar = true;
                TxtConfirmpassword.UseSystemPasswordChar = true;
            }
        } // El de la contraseña

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnregistrarse_Click(object sender, EventArgs e)
        {
            if (RevisarTextbox()== true)
            {
                try
                {
                    _mailPendiente = txtmail.Text.Trim();
                    _contrasenaPendiente = TxtPassword.Text;
                    using (Avisos.Esperar(this, btnregistrarse))
                    {
                        if (_altaAdministrador) GestorUsuarios.Instance.CrearAdministrador(txtmail.Text, TxtPassword.Text, txtNombre.Text, txtApellido.Text);
                        else GestorUsuarios.Instance.RegistrarCliente(txtmail.Text, TxtPassword.Text, txtNombre.Text, txtApellido.Text);
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    if (ex.Regla == "RN-02" || ex.Regla == "RN-47")
                    {
                        ValidacionCampo.Mostrar(TxtPassword, ex.Message); TxtPassword.Focus();
                    }
                    else if (ex.Regla == "RN-01")
                    {
                        ValidacionCampo.Mostrar(txtmail, ex.Message); txtmail.Focus();
                    }
                    else Avisos.Regla(this, "Crear cuenta", ex);
                    return;
                }
                catch (Exception)
                {
                    ValidacionCampo.Mostrar(txtmail, "No pudimos completar el alta o enviar el código. Revisá la conexión y volvé a intentarlo.");
                    return;
                }
                _proximoReenvioCorreo = DateTime.UtcNow.AddSeconds(30);
                if (!_altaAdministrador)
                {
                    MostrarVerificacionCorreo(txtmail.Text.Trim());
                    return;
                }
                var menu = Cerrar();
                Avisos.Informativo(menu, "Usuario creado con éxito.");
            }
            else { return; }
        }
        private void MostrarVerificacionCorreo(string correo, bool desdeSesion = false)
        {
            foreach (Control anterior in Controls.Find("verificacionCorreo", true)) anterior.Dispose();
            foreach (Control control in Controls.Cast<Control>().ToArray())
                if (control.Name != "barraSuperior") control.Visible = false;
            Text = "EventAura · Verificar correo";
            float escala = Disposicion.Escala(this);
            int px(float valor) => (int)Math.Round(valor * escala);
            int ancho = Math.Max(px(280), ClientSize.Width - px(48));
            var verificacion = new Panel { Name = "verificacionCorreo", Location = new Point(px(24), px(84)), Size = new Size(ancho, Math.Max(px(300), ClientSize.Height - px(108))), AutoScroll = true };
            var titulo = new Componentes.EtiquetaTitulo { Name = "tituloVerificacion", Text = "Verificá tu correo", Location = new Point(px(8), px(8)), Size = new Size(ancho - px(24), px(40)) };
            var contenido = new Componentes.EtiquetaAyuda { Name = "instruccionesVerificacion", Location = new Point(px(8), px(60)), Size = new Size(ancho - px(24), px(80)), Text = "Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección." };
            var etiqueta = new Componentes.EtiquetaRotulo { Text = "Código de verificación", Location = new Point(px(8), px(164)), Size = new Size(px(240), px(24)) };
            var codigo = new TextBox { Name = "txtCodigoCorreo", Location = new Point(px(8), px(192)), AutoSize = false, Size = new Size(px(Tema.AnchoCampo), px(Tema.AltoCampo)), MaxLength = 6 };
            if (desdeSesion)
                contenido.Text = Negocio.SeleccionEnviadorCodigos.Actual is EnviadorSimulado
                    ? "Abrí la carpeta BandejaSimulada (" + EnviadorSimulado.RutaBandejaActual + ") y buscá el archivo más reciente."
                    : "Revisá tu correo para encontrar el código de verificación más reciente.";
            ColocarRotulo(etiqueta, codigo);
            var estado = new Componentes.EtiquetaAyuda { Name = "estadoVerificacion", Location = new Point(px(8), px(232)), Size = new Size(ancho - px(24), px(54)) };
            codigo.TabIndex = 0;
            var confirmar = new Componentes.BotonPrimario { Name = "btnConfirmarCorreo", Text = "Confirmar correo", Location = new Point(px(8), px(292)), Size = new Size(px(200), px(40)), TabIndex = 1 };
            var volver = new Componentes.BotonSecundario { Name = "btnVolverAcceso", Text = desdeSesion ? "Volver a eventos" : "Volver al acceso", Location = new Point(px(220), px(292)), Size = new Size(px(180), px(40)), Visible = true, TabIndex = 2 };
            var reenviar = new Componentes.BotonSecundario { Name = "btnReenviarCorreo", Text = "Reenviar código", Location = new Point(px(8), px(348)), Size = new Size(px(200), px(40)), TabIndex = 3 };
            var espera = new Componentes.EtiquetaAyuda { Name = "esperaReenvioCorreo", Location = new Point(px(220), px(348)), Size = new Size(px(360), px(40)), TextAlign = ContentAlignment.MiddleLeft };
            int segundos = Math.Max(0, (int)Math.Ceiling((_proximoReenvioCorreo - DateTime.UtcNow).TotalSeconds));
            var reloj = new Timer { Interval = 1000 };
            Action actualizarEspera = () =>
            {
                reenviar.Enabled = segundos <= 0;
                espera.Text = segundos > 0 ? "Podés pedir otro código en " + segundos + " s." : "¿No llegó?";
                if (segundos > 0) segundos--;
                else reloj.Stop();
            };
            actualizarEspera();
            if (segundos > 0) reloj.Start();
            verificacion.Disposed += (s, e) => reloj.Dispose();
            reloj.Tick += (s, e) => actualizarEspera();
            confirmar.Click += (s, e) =>
            {
                try
                {
                    using (Avisos.Esperar(this, confirmar))
                        GestorCodigosCorreo.Instance.ConfirmarVerificacion(correo, codigo.Text.Trim());
                    estado.Text = "Correo verificado. Ya podés iniciar sesión y comprar.";
                    confirmar.Enabled = false;
                    volver.Visible = true;
                    reenviar.Enabled = false;
                    _contrasenaPendiente = null;
                    if (desdeSesion && _alCompletarVerificacionSesion != null && IsHandleCreated) BeginInvoke(_alCompletarVerificacionSesion);
                }
                catch (ReglaNegocioException ex) { estado.Text = ex.Message; }
            };
            reenviar.Click += (s, e) =>
            {
                if (DateTime.UtcNow < _proximoReenvioCorreo) { estado.Text = "Esperá antes de pedir otro código."; return; }
                try
                {
                    int id = desdeSesion ? (GestorSesion.Instance.UsuarioActual?.Id ?? 0) : GestorUsuarios.Instance.Autenticar(correo, _contrasenaPendiente ?? string.Empty).Id;
                    if (id <= 0) throw new InvalidOperationException("La sesión ya no está disponible. Volvé a iniciar sesión.");
                    GestorCodigosCorreo.Instance.EnviarVerificacion(id, correo);
                    _proximoReenvioCorreo = DateTime.UtcNow.AddSeconds(30);
                    segundos = 30; reenviar.Enabled = false; actualizarEspera(); reloj.Start();
                    estado.Text = "Enviamos un código nuevo.";
                }
                catch (Exception ex) { estado.Text = ManejadorErrores.MensajeParaUsuario(ex); }
            };
            volver.Click += (s, e) => { if (desdeSesion) VolverDeVerificacionSesion(); else Cerrar(); };
            Tema.Normalizar(verificacion);
            verificacion.Controls.AddRange(new Control[] { titulo, contenido, etiqueta, codigo, estado, confirmar, volver, reenviar, espera });
            Controls.Add(verificacion);
            verificacion.BringToFront();
            AcceptButton = confirmar;
            CancelButton = volver;
            ReconfigurarTeclado();
            codigo.Focus();
        }
        private void lblyatengocuen_Click(object sender, EventArgs e)
        {
            Cerrar();
        } // Nos devuelve al incio

        // -------------------------------FUNCIONES DE CONTROL--------------------------------------------------------------------
        private bool RevisarTextbox()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                ValidacionCampo.Mostrar(txtNombre, "Escribí tu nombre."); txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                ValidacionCampo.Mostrar(txtApellido, "Escribí tu apellido."); txtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtmail.Text))
            {
                ValidacionCampo.Mostrar(txtmail, "Escribí tu correo electrónico."); txtmail.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(TxtPassword.Text))
            {
                ValidacionCampo.Mostrar(TxtPassword, "Elegí una contraseña."); TxtPassword.Focus();
                return false;

            }
            if (string.IsNullOrEmpty(TxtConfirmpassword.Text))
            {
                ValidacionCampo.Mostrar(TxtConfirmpassword, "Repetí tu contraseña."); TxtConfirmpassword.Focus();
                return false;
            }
            if (TxtPassword.Text != TxtConfirmpassword.Text)
            {
                ValidacionCampo.Mostrar(TxtConfirmpassword, "Las contraseñas no coinciden. Revisá lo que escribiste."); TxtConfirmpassword.Focus();
                return false;
            }
            else
            {
                return true;
            }
        } // Revisa si los textbox estan vacios
        private Form Cerrar()
        {
            if (_altaAdministrador)
            {
                var menu = Application.OpenForms.OfType<frmMenuAdministrador>().FirstOrDefault();
                if (menu != null) menu.Show();
                Hide();
                return menu;
            }
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
            return null;
        } // cierra el form

    }
}
    


    

