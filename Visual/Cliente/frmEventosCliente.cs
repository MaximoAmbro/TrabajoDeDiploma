using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public string NombreUsuario;
        public frmEventosCliente()
        {
            InitializeComponent();
        }
        private const string TodasLasCategorias = "Todas";

        private void frmEventosCliente_Load(object sender, EventArgs e)
        {
            cmbCategoria.Items.Add(TodasLasCategorias);
            cmbCategoria.Items.AddRange(GestorEventos.Categorias.ToArray<object>());
            cmbCategoria.SelectedIndex = 0;
            PonerTextoGuia(txtBuscar, "Evento o dirección del local");
            CargarEventos(null, null, null);
            txtBuscar.Select();
        }

        // RF-03. Buscar aplica siempre RN-09: solo eventos Publicado con fecha futura.
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string categoria = cmbCategoria.SelectedIndex > 0 ? cmbCategoria.SelectedItem.ToString() : null;
            DateTime? desde = dtpDesde.Checked ? dtpDesde.Value.Date : (DateTime?)null;
            CargarEventos(txtBuscar.Text, categoria, desde);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            cmbCategoria.SelectedIndex = 0;
            dtpDesde.Checked = false;
            CargarEventos(null, null, null);
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;   // sin el "ding" del TextBox de una línea
                btnBuscar.PerformClick();
            }
        }

        private void CargarEventos(string texto, string categoria, DateTime? desde)
        {
            List<Evento> eventos = GestorEventos.Instance.Buscar(texto, categoria, desde);
            if (eventos.Count == 0)
            {
                DataTable vacia = new DataTable();
                vacia.Columns.Add("Mensaje", typeof(string));
                vacia.Rows.Add(texto == null && categoria == null && desde == null
                    ? "No hay eventos disponibles por el momento."
                    : "No hay eventos que coincidan con la búsqueda. Pruebe con otros filtros o «Limpiar».");
                dgveventos.DataSource = vacia;
                return;
            }

            var locales = new Dictionary<int, Local>();
            dgveventos.DataSource = eventos
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

        // Texto guía dentro del TextBox (EM_SETCUEBANNER): .NET Framework no tiene PlaceholderText.
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private static void PonerTextoGuia(TextBox caja, string texto)
        {
            const int EM_SETCUEBANNER = 0x1501;
            SendMessage(caja.Handle, EM_SETCUEBANNER, IntPtr.Zero, texto);
        }

        private void btnComprar_Click(object sender, EventArgs e)
        {
            if (dgveventos.CurrentRow != null && dgveventos.Columns.Contains("ID"))
            {
                FrmCompraCliente frm = new FrmCompraCliente();
                frm.IDUsuario = IDUsuario;
                frm.IDEvento = Convert.ToInt32(dgveventos.CurrentRow.Cells["ID"].Value);
                frm.NombreUsuario = NombreUsuario;
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
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }

    }
}
