using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;
using Negocio;
namespace Visual
{
    public partial class frmEventosCliente : Form
    {
        public int IDUsuario;
        public SqlConnection ConexionSql; // TODO Seccion 7: quitar ConexionSql (frmMenuCliente y frmHistorialCliente la usan)
        public string NombreUsuario;
        public frmEventosCliente()
        {
            InitializeComponent();
        }
        // RN-09: solo eventos Publicado con fecha futura.
        private void frmEventosCliente_Load(object sender, EventArgs e)
        {
            var locales = new Dictionary<int, Local>();
            dgveventos.DataSource = GestorEventos.Instance.Buscar(null, null, null)
                .Select(ev =>
                {
                    Local local;
                    if (!locales.TryGetValue(ev.IdLocal, out local))
                    {
                        local = GestorLocales.Instance.PorId(ev.IdLocal);
                        locales[ev.IdLocal] = local;
                    }
                    return new
                    {
                        ID = ev.Id,
                        ev.Nombre,
                        ev.Fecha,
                        ev.Categoria,
                        Ubicacion = local?.Direccion,
                        Complejo = local?.Nombre
                    };
                })
                .ToList();
            dgveventos.Columns["ID"].Visible = false;
        }
        private void btnComprar_Click(object sender, EventArgs e)
        {
            if (dgveventos.CurrentRow != null)
            {
                FrmCompraCliente frm = new FrmCompraCliente();
                frm.IDUsuario = IDUsuario;
                frm.IDEvento = Convert.ToInt32(dgveventos.CurrentRow.Cells["ID"].Value);
                frm.NombreUsuario = NombreUsuario;
                frm.ConexionSql = ConexionSql;
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un evento para comprar", "Hola Pedro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuCliente frm = new frmMenuCliente();
            frm.IDUsuario = IDUsuario;
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }

    }
}
