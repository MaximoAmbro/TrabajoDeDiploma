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
    public partial class frmEventosCliente : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;
        private bool vistaTabla;
        private bool distribuyendoTarjetas;
        public frmEventosCliente()
        {
            InitializeComponent();
            Tema.ComoFondo(tarjetas);
            tarjetas.ClientSizeChanged += (s, e) => DistribuirCatalogo();
            btnVista.Click += (s, e) => MostrarVista(!vistaTabla);
        }
        protected override void OnShown(EventArgs e){base.OnShown(e);Disposicion.AjustarAreaVisible(this);}
        private const string TodasLasCategorias = "Todas";

        private void frmEventosCliente_Load(object sender, EventArgs e)
        {
            dgveventos.ReadOnly = true;
            dgveventos.AllowUserToAddRows = false;
            dgveventos.AllowUserToDeleteRows = false;
            dgveventos.RowHeadersVisible = false;
            dgveventos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgveventos.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgveventos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
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

        private void MostrarVista(bool tabla)
        {
            vistaTabla = tabla;
            tarjetas.Visible = !tabla;
            dgveventos.Visible = tabla;
            btnComprar.Visible = tabla || !btnVista.Enabled;
            btnVista.Text = tabla ? "Ver tarjetas" : "Ver como tabla";
        }

        private void DistribuirCatalogo()
        {
            if (distribuyendoTarjetas) return;
            distribuyendoTarjetas = true;
            tarjetas.SuspendLayout();
            try
            {
                CentrarEstadoSinResultados();
                var catalogo = tarjetas.Controls.OfType<Componentes.TarjetaEvento>().ToArray();
                if (catalogo.Length == 0) return;
                int espacio = Disposicion.Pixeles(tarjetas, Tema.Espacio.S);
                int ancho = Math.Max(1, (tarjetas.Width - tarjetas.Padding.Horizontal -
                    SystemInformation.VerticalScrollBarWidth - 2 * espacio) / 3);
                for (int i = 0; i < catalogo.Length; i++)
                {
                    catalogo[i].Margin = new Padding(0, 0, i % 3 == 2 ? 0 : espacio, espacio);
                    catalogo[i].Distribuir(ancho);
                }
                int alto = catalogo.Max(tarjeta => tarjeta.Height);
                foreach (var tarjeta in catalogo) tarjeta.IgualarAlto(alto);
            }
            finally
            {
                tarjetas.ResumeLayout(true);
                distribuyendoTarjetas = false;
            }
        }

        private void CentrarEstadoSinResultados()
        {
            var estado = tarjetas.Controls["estadoSinResultados"] as Componentes.EstadoVacio;
            if (estado == null) return;
            var area = new Rectangle(tarjetas.Padding.Left, tarjetas.Padding.Top,
                Math.Max(1, tarjetas.ClientSize.Width - tarjetas.Padding.Horizontal),
                Math.Max(1, tarjetas.ClientSize.Height - tarjetas.Padding.Vertical));
            estado.AjustarA(area);
        }

        private void CargarEventos(string texto, string categoria, DateTime? desde)
        {
            List<Evento> eventos = GestorEventos.Instance.Buscar(texto, categoria, desde);
            foreach(Control anterior in tarjetas.Controls.Cast<Control>().ToArray()) anterior.Dispose();
            tarjetas.Controls.Clear();
            btnVista.Enabled = eventos.Count > 0;
            if (eventos.Count == 0)
            {
                var estado=new Componentes.EstadoVacio { Name = "estadoSinResultados" };
                estado.Cargar("No encontramos eventos para esa búsqueda. Probá con otro nombre o quitá los filtros.","Ver todos");
                estado.Accion += () => btnLimpiar.PerformClick();
                estado.Margin = Padding.Empty;
                tarjetas.Controls.Add(estado);
                MostrarVista(false);
                CentrarEstadoSinResultados();
                dgveventos.DataSource = null;
                dgveventos.Visible = false;
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
            dgveventos.Columns["Categoria"].HeaderText = "Categoría";
            dgveventos.Columns["Ubicacion"].HeaderText = "Ubicación";
            dgveventos.Columns["Nombre"].FillWeight = 160;
            dgveventos.Columns["Ubicacion"].FillWeight = 140;
            dgveventos.Columns["Complejo"].FillWeight = 140;
            foreach(var ev in eventos) {
                var tarjeta=new Componentes.TarjetaEvento();
                tarjeta.Cargar(ev,locales[ev.IdLocal],GestorEventos.Instance.Sectores(ev.Id));
                tarjeta.VerEntradas+=ElegirEvento;
                tarjetas.Controls.Add(tarjeta);
            }
            MostrarVista(vistaTabla);
            DistribuirCatalogo();
        }

        private void ElegirEvento(Evento elegido)
        {
            var fila=dgveventos.Rows.Cast<DataGridViewRow>().Single(f=>Convert.ToInt32(f.Cells["ID"].Value)==elegido.Id);
            dgveventos.CurrentCell=fila.Cells["Nombre"];
            var ficha=new Componentes.FichaEvento {Name="fichaEvento",Dock=DockStyle.Fill};
            ficha.Cargar(elegido);
            tarjetas.Visible=false;dgveventos.Visible=false;pnlBusqueda.Visible=false;pnlAcciones.Visible=false;
            pnlExplorar.Controls.Add(ficha);ficha.BringToFront();
            ficha.Volver+=()=>{ficha.Dispose();pnlBusqueda.Visible=true;pnlAcciones.Visible=true;MostrarVista(vistaTabla);};
            ficha.Comprar+=(ev,sector)=>AbrirCompra(ev.Id,sector);
        }

        private void AbrirCompra(int idEvento,int? sector=null)
        {
            var compra=new FrmCompraCliente {IDUsuario=IDUsuario,IDEvento=idEvento,NombreUsuario=NombreUsuario,SectorInicial=sector};
            if (NavegacionCliente.Navegar(compra)) return;
            compra.Show();Hide();
        }

        // Texto guía dentro del TextBox (EM_SETCUEBANNER): .NET Framework no tiene PlaceholderText.
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private static void PonerTextoGuia(TextBox caja, string texto)
        {
            const int EM_SETCUEBANNER = 0x1501;
            SendMessage(caja.Handle, EM_SETCUEBANNER, new IntPtr(1), texto);
        }

        private void btnComprar_Click(object sender, EventArgs e)
        {
            if (dgveventos.CurrentRow != null && dgveventos.Columns.Contains("ID"))
            {
                AbrirCompra(Convert.ToInt32(dgveventos.CurrentRow.Cells["ID"].Value));
            }
            else
            {
                Avisos.Informativo(this, "Elegí un evento de la lista para ver sus entradas.");
            }
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (NavegacionCliente.Inicio()) return;
            frmMenuCliente frm = new frmMenuCliente();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show();
            this.Hide();
        }

    }
}
