
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
        public int IDUsuario;
        public string NombreUsuario;

        public frmMenuPropietario()
        {
            InitializeComponent();
            panelAdministrador.CrearAdministradorSolicitado += CrearAdministrador;
            panelAdministrador.AuditoriaSolicitada += (s, e) => { using (var form = new frmAuditoria()) form.ShowDialog(this); };
            SizeChanged+=(s,e)=>DistribuirPanel();
        }

        private void frmMenuVendedor_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreUsuario;
            panelAdministrador.Cargar();DistribuirPanel();
        }
        protected override void OnShown(EventArgs e){base.OnShown(e);Disposicion.AjustarAreaVisible(this);DistribuirPanel();}
        private void DistribuirPanel()
        {
            int margen=(int)(24*Disposicion.Escala(this));
            panelAdministrador.SetBounds(btnMisLocales.Right+margen,label1.Top,
                Math.Max(1,ClientSize.Width-btnMisLocales.Right-margen*2),Math.Max(1,ClientSize.Height-label1.Top-margen));
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
        private void CrearAdministrador(object sender, EventArgs e)
        {
            var alta = new frmCrearUsuario(true);
            alta.Show();
            Hide();
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
