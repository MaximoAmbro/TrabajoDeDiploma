using Entidades;
using Negocio;
using Datos;
using System;
using System.Windows.Forms;

namespace Visual
{
    public partial class frmInicio : FormularioEventAura
    {
        // La cadena de conexión la lee Datos/Conexion.cs de App.config: Visual no abre conexiones.
        public frmInicio()
        {
            InitializeComponent();
            Tema.ComoEnlace(lblolividarcontra);
            Tema.ComoEnlace(lblcrearcuenta);
            txtMailLogin.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtMailLogin);
            txtContraseñaLogin.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtContraseñaLogin);
        }
        
        private void frmInicio_Load(object sender, EventArgs e)
        {
            ColocarCampo(label5, txtMailLogin);
            ColocarCampo(label3, txtContraseñaLogin);
            int espacio = Disposicion.Pixeles(this, Tema.Espacio.S);
            int anchoEnlace = (txtMailLogin.Width - espacio) / 2;
            lblcrearcuenta.Left = txtMailLogin.Left;
            lblcrearcuenta.Width = lblolividarcontra.Width = anchoEnlace;
            lblolividarcontra.Left = lblcrearcuenta.Right + espacio;
            var faltante = VerificadorEsquema.PrimerFaltante();
            if (faltante != null)
            {
                MostrarEsquemaDesactualizado(faltante);
                return;
            }
            
            txtMailLogin.Text = "";
            txtContraseñaLogin.Text = "";
            txtContraseñaLogin.PasswordChar = '*';
            txtMailLogin.Select();   // no depende del orden de TabIndex
        }

        private void MostrarEsquemaDesactualizado(RequisitoEsquemaFaltante faltante)
        {
            txtMailLogin.Visible = label5.Visible = txtContraseñaLogin.Visible = label3.Visible = false;
            checkbxShowPass.Visible = lblolividarcontra.Visible = lblcrearcuenta.Visible = btninicio.Visible = false;
            btninicio.Enabled = false;
            var aviso = new Visual.Componentes.EtiquetaAyuda
            {
                Name = "avisoEsquemaDesactualizado",
                Text = "Tu base de datos está desactualizada: falta " + faltante.Descripcion + ".\r\n\r\n" +
                       "Corré BaseDatos/" + faltante.Script + " y volvé a abrir EventAura.",
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
                txtContraseñaLogin.PasswordChar = '\0';
            }
            else
            {
                txtContraseñaLogin.PasswordChar = '*';
            }
        } //Le pone el asterisco a la contraseña

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void lblolividarcontra_Click(object sender, EventArgs e)
        {
            frmCambiarContraseña frm = new frmCambiarContraseña(true);
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
                if (usuario.TipoUsuario == "Cliente")
                {
                    var avisos = GestorListaEspera.Instance.AvisosAlIngresar(usuario.Id);
                    frmMenuCliente frm = new frmMenuCliente();
                    frm.IDUsuario = usuario.Id;
                    frm.NombreUsuario = usuario.Nombre;
                    frm.Show();
                    if (avisos.Count > 0)
                        Avisos.Informativo(frm, "Hay lugares disponibles en " + string.Join(", ", avisos) +
                            ". Podés consultarlos en eventos; el aviso no reserva un lugar.");
                    this.Hide();
                } // Se abre el menu de cliente
                if (usuario.TipoUsuario == "Organizador")
                {
                    frmMenuPropietario frm = new frmMenuPropietario();
                    frm.IDUsuario = usuario.Id;
                    frm.NombreUsuario = usuario.Nombre;
                    frm.Show();
                    this.Hide();
                } // Se abre el menu de vendedor
                else if (usuario.TipoUsuario == "Administrador")
                {
                    frmMenuAdministrador frm = new frmMenuAdministrador();
                    frm.IDUsuario = usuario.Id;
                    frm.NombreUsuario = usuario.Nombre;
                    frm.Show();
                    this.Hide();
                }
            }
            else
            {
                Avisos.Regla("Inicio de sesión", new ReglaNegocioException("RN-04", GestorUsuarios.MensajeAccesoRechazado));
            }
        }
    } // Valida la lista de usuarios

}
