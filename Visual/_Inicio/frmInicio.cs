using Entidades;
using Negocio;
using System;
using System.Configuration;
using System.Data.Common;
using System.Windows.Forms;

namespace Visual
{
    public partial class frmInicio : FormularioEventAura
    {
        internal Func<Tuple<string, string>> VerificarEsquema = VerificarConDatos;
        // La cadena de conexión la lee Datos/Conexion.cs de App.config: Visual no abre conexiones.
        public frmInicio()
        {
            InitializeComponent();
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
        
        private void frmInicio_Load(object sender, EventArgs e)
        {
            ColocarTitulo(label2);
            txtMailLogin.Left = txtContraseñaLogin.Left = checkbxShowPass.Left = Disposicion.Pixeles(this, Tema.Espacio.XXL);
            ColocarCampo(label5, txtMailLogin);
            ColocarCampo(label3, txtContraseñaLogin);
            btninicio.Left = txtMailLogin.Left;
            btninicio.Width = txtMailLogin.Width;
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
                    MostrarEsquemaDesactualizado(faltante.Item1, faltante.Item2);
                    return;
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                MostrarErrorDeBase();
                return;
            }
            
            txtMailLogin.Text = "";
            txtContraseñaLogin.Text = "";
            txtContraseñaLogin.UseSystemPasswordChar = true;
            txtMailLogin.Select();   // no depende del orden de TabIndex
        }

        private void MostrarErrorDeBase()
        {
            txtMailLogin.Visible = label5.Visible = txtContraseñaLogin.Visible = label3.Visible = false;
            checkbxShowPass.Visible = lblolividarcontra.Visible = lblcrearcuenta.Visible = btninicio.Visible = false;
            btninicio.Enabled = false;
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
            var aviso = new Visual.Componentes.EtiquetaAyuda
            {
                Name = "avisoBaseNoDisponible",
                Text = "EventAura no pudo comprobar la base de datos. " + destino +
                       ". Revisá SQL Server y la configuración, y volvé a abrir la aplicación.",
                Location = new System.Drawing.Point(24, 148),
                Size = new System.Drawing.Size(352, 128),
                AccessibleRole = System.Windows.Forms.AccessibleRole.Alert
            };
            Controls.Add(aviso);
            aviso.BringToFront();
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
