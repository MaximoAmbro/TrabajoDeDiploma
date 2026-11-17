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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Visual
{
    public partial class frmMenuCliente : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;
        public frmMenuCliente()
        {
            InitializeComponent();
            inicioCliente.VerEventos+=()=>btnEventos.PerformClick();
            SizeChanged+=(s,e)=>DistribuirInicio();
        }
        private void frmMenuCliente_Load(object sender, EventArgs e)
        {
            lblNombre.Text = NombreUsuario;
            int id=IDUsuario>0?IDUsuario:(GestorSesion.Instance.UsuarioActual?.Id??0);
            inicioCliente.Cargar(id);barraSuperior.ActualizarSesion();DistribuirInicio();
        }
        protected override void OnShown(EventArgs e){base.OnShown(e);Disposicion.AjustarAreaVisible(this);DistribuirInicio();}
        private void DistribuirInicio()
        {
            int margen=(int)(24*Disposicion.Escala(this));
            inicioCliente.SetBounds(btnEventos.Right+margen,label1.Top,
                System.Math.Max(1,ClientSize.Width-btnEventos.Right-margen*2),System.Math.Max(1,ClientSize.Height-label1.Top-margen));
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnEventos_Click(object sender, EventArgs e)
        {
            frmEventosCliente frm = new frmEventosCliente();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            frmHistorialCliente frm = new frmHistorialCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
        private void btnSuscripcion_Click(object sender, EventArgs e)
        {
            frmSuscripcion frm = new frmSuscripcion();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show();
            this.Hide();
        }
        private void lblvolver_Click(object sender, EventArgs e)
        {
            GestorSesion.Instance.Cerrar();
            frmInicio frm = new frmInicio();
            frm.Show();
            this.Hide();
        }
    }
}
