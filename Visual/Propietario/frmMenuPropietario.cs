
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;

namespace Visual
{
    public partial class frmMenuPropietario : FormularioEventAura
    {
        private readonly Panel zonaOrganizador;
        private readonly Componentes.EtiquetaTitulo tituloOrganizador;
        private readonly Componentes.EtiquetaAyuda ayudaOrganizador;
        private readonly Componentes.BotonSecundario btnCambiarContrasena;
        private readonly Componentes.EtiquetaRotulo seccionLocales;
        private readonly Componentes.EtiquetaRotulo seccionOperacion;
        private readonly Componentes.EtiquetaRotulo seccionResultados;
        private readonly Componentes.EtiquetaRotulo seccionCuenta;
        public int IDUsuario;
        public string NombreUsuario;

        public frmMenuPropietario()
        {
            InitializeComponent();
            seccionLocales = Seccion("seccionLocales", "LOCALES Y EVENTOS", 164);
            seccionOperacion = Seccion("seccionOperacion", "OPERACIÓN", 296);
            seccionResultados = Seccion("seccionResultados", "RESULTADOS", 428);
            seccionCuenta = Seccion("seccionCuenta", "CUENTA", 536);
            Controls.AddRange(new Control[] { seccionLocales, seccionOperacion, seccionResultados, seccionCuenta });
            btnMisLocales.Location = new Point(24, 188);
            btnRegistrarLocal.Location = new Point(24, 232);
            btnControlIngreso.Location = new Point(24, 320);
            btnListasEspera.Location = new Point(24, 364);
            btnReportes.Location = new Point(24, 452);
            lblvolver.Location = new Point(24, 628);
            lblvolver.Text = "Cerrar sesión";
            zonaOrganizador = new Panel { Name = "zonaOrganizador", BackColor = Tema.Superficie, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            tituloOrganizador = new Componentes.EtiquetaTitulo { Name = "tituloOrganizador", Text = "Tu espacio de organización" };
            ayudaOrganizador = new Componentes.EtiquetaAyuda { Name = "ayudaOrganizador", Text = "Administrá tus locales y eventos, controlá el ingreso y consultá reportes de tus propias ventas." };
            zonaOrganizador.Controls.Add(tituloOrganizador);
            zonaOrganizador.Controls.Add(ayudaOrganizador);
            Controls.Add(zonaOrganizador);
            btnCambiarContrasena = new Componentes.BotonSecundario { Name = "btnCambiarContrasena", Text = "Cambiar contraseña", Location = new Point(24, 576), Size = new Size(272, 40) };
            btnCambiarContrasena.Click += (s, e) =>
            {
                var cambio = new frmCambiarContraseña { CorreoInicial = GestorSesion.Instance.UsuarioActual?.Mail, Retorno = this };
                cambio.Show();
                Hide();
            };
            Controls.Add(btnCambiarContrasena);
            btnCambiarContrasena.BringToFront();
            zonaOrganizador.BringToFront();
            btnCambiarContrasena.BringToFront();
            SizeChanged+=(s,e)=>{DistribuirMenu();DistribuirPanel();};
            ClientSizeChanged+=(s,e)=>{DistribuirMenu();DistribuirPanel();};
        }

        private static Componentes.EtiquetaRotulo Seccion(string nombre, string texto, int arriba)
        {
            return new Componentes.EtiquetaRotulo { Name = nombre, Text = texto, Location = new Point(24, arriba), Size = new Size(272, 20) };
        }

        private void frmMenuVendedor_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreUsuario;
            GestorSesion.Instance.RequerirOrganizador();
            DistribuirPanel();
        }
        protected override void OnShown(EventArgs e){base.OnShown(e);Disposicion.AjustarAreaVisible(this);DistribuirMenu();DistribuirPanel();}
        private void DistribuirMenu()
        {
            if (seccionLocales == null || btnMisLocales == null) return;
            float escala = Disposicion.Escala(this);
            if (escala <= 1.01f) return;
            int px(float valor) => (int)Math.Round(valor * escala);
            int margen = px(32), ancho = px(272), altoRotulo = px(14), hueco = px(1);
            int Alto(Button boton) => px(boton == btnCambiarContrasena || boton == lblvolver ? 28 : boton is Componentes.BotonPrimario ? 36 : 32);
            int arriba = label1.Bottom + px(3);
            int baseTotal = 4 * (altoRotulo + hueco) + Alto(btnMisLocales) + Alto(btnRegistrarLocal) +
                Alto(btnControlIngreso) + Alto(btnListasEspera) + Alto(btnReportes) + Alto(btnCambiarContrasena) + Alto(lblvolver);
            int espacio = Math.Max(0, ClientSize.Height - margen - arriba - baseTotal);
            int huecoGrupo = Math.Max(px(1), Math.Min(px(8), espacio / 3));
            int total = baseTotal + 3 * huecoGrupo;
            int y = Math.Max(arriba, ClientSize.Height - margen - total);

            void Rotulo(Componentes.EtiquetaRotulo control, string texto)
            {
                control.Text = texto;
                control.SetBounds(margen, y, ancho, altoRotulo);
                y += altoRotulo + hueco;
            }
            void Accion(Button control, int alto)
            {
                control.SetBounds(margen, y, ancho, alto);
                y += alto;
            }
            Rotulo(seccionLocales, "LOCALES Y EVENTOS");
            Accion(btnMisLocales, Alto(btnMisLocales)); Accion(btnRegistrarLocal, Alto(btnRegistrarLocal)); y += huecoGrupo;
            Rotulo(seccionOperacion, "OPERACIÓN");
            Accion(btnControlIngreso, Alto(btnControlIngreso)); Accion(btnListasEspera, Alto(btnListasEspera)); y += huecoGrupo;
            Rotulo(seccionResultados, "RESULTADOS");
            Accion(btnReportes, Alto(btnReportes)); y += huecoGrupo;
            Rotulo(seccionCuenta, "CUENTA");
            Accion(btnCambiarContrasena, Alto(btnCambiarContrasena)); Accion(lblvolver, Alto(lblvolver));
        }
        private void DistribuirPanel()
        {
            int margen=(int)(24*Disposicion.Escala(this));
            zonaOrganizador.SetBounds(btnMisLocales.Right+margen,label1.Top,
                Math.Max(1,ClientSize.Width-btnMisLocales.Right-margen*2),Math.Max(1,ClientSize.Height-label1.Top-margen));
            tituloOrganizador.SetBounds(24,24,Math.Max(1,zonaOrganizador.ClientSize.Width-48),40);
            ayudaOrganizador.SetBounds(24,72,Math.Max(1,zonaOrganizador.ClientSize.Width-48),96);
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnMisLocales_Click(object sender, EventArgs e)
        {
            frmMisLocales frm = new frmMisLocales();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnRegistrarLocal_Click(object sender, EventArgs e)
        {
            frmRegistrarLocal frm = new frmRegistrarLocal();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnListasEspera_Click(object sender, EventArgs e)
        {
            frmListasEspera frm = new frmListasEspera();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnReportes_Click(object sender, EventArgs e)
        {
            frmReportes frm = new frmReportes();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
        private void btnControlIngreso_Click(object sender, EventArgs e)
        {
            using (var control = new frmControlIngreso()) control.ShowDialog(this);
        }

        private void lblvolver_Click(object sender, EventArgs e)
        {
            GestorSesion.Instance.Cerrar();
            frmInicio frmInicio = new frmInicio();
            frmInicio.Show(); this.Hide();
        }
    }
}
