using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    // RF-10 + CU-07: consulta de las listas de espera, solo lectura en esta versión. Se elige un
    // evento agotado de los locales del administrador y se ve la lista en el orden de RN-35.
    public partial class frmListasEspera : Form
    {
        public int IDUsuario;
        public string NombreUsuario;

        private static readonly Color Violeta = Color.FromArgb(116, 86, 174);

        public frmListasEspera()
        {
            InitializeComponent();
        }

        // Lo que muestra el combo de eventos.
        private sealed class ItemEvento
        {
            public Evento Evento;
            public bool Agotado;

            public override string ToString()
            {
                return $"{Evento.Nombre} · {Evento.Fecha:dd/MM/yyyy HH:mm}" + (Agotado ? " · agotado" : "");
            }
        }

        private void frmListasEspera_Load(object sender, EventArgs e)
        {
            dgvLista.Columns.Clear();
            dgvLista.AllowUserToAddRows = false;
            dgvLista.AllowUserToDeleteRows = false;
            dgvLista.AllowUserToResizeRows = false;
            dgvLista.ReadOnly = true;
            dgvLista.RowHeadersVisible = false;
            dgvLista.MultiSelect = false;
            dgvLista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLista.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLista.BackgroundColor = Color.White;
            dgvLista.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvLista.DefaultCellStyle.SelectionBackColor = Violeta;
            dgvLista.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvLista.Columns.Add("Posicion", "Posición");
            dgvLista.Columns.Add("Cliente", "Cliente");
            dgvLista.Columns.Add("Mail", "Mail");
            dgvLista.Columns.Add("Suscriptor", "Suscriptor");
            dgvLista.Columns.Add("FechaAlta", "Inscripto el");
            dgvLista.Columns["Posicion"].FillWeight = 50;
            dgvLista.Columns["Suscriptor"].FillWeight = 60;
            dgvLista.Columns["Posicion"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvLista.Columns["Suscriptor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn columna in dgvLista.Columns)
            {
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;   // el orden es el de RN-35
            }

            CargarEventos();
        }

        // Eventos agotados de los locales del administrador (RN-10). También los que ya no están
        // agotados pero conservan inscriptos, por ejemplo después de una devolución.
        private void CargarEventos()
        {
            cmbEventos.Items.Clear();
            foreach (Local local in GestorLocales.Instance.PorPropietario(IDUsuario))
            {
                foreach (Evento evento in GestorEventos.Instance.PorLocal(local.Id))
                {
                    bool agotado = GestorEventos.Instance.EstaAgotado(evento.Id);
                    if (agotado || GestorListaEspera.Instance.DeEvento(evento.Id).Count > 0)
                    {
                        cmbEventos.Items.Add(new ItemEvento { Evento = evento, Agotado = agotado });
                    }
                }
            }

            if (cmbEventos.Items.Count == 0)
            {
                cmbEventos.Enabled = false;
                lblCantidad.Text = "No hay eventos agotados ni listas de espera en sus locales.";
                return;
            }
            cmbEventos.SelectedIndex = 0;
        }

        private void cmbEventos_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemEvento item = cmbEventos.SelectedItem as ItemEvento;
            if (item != null)
            {
                CargarLista(item.Evento);
            }
        }

        // RN-35: el orden lo da GestorListaEspera; la posición es el número de fila.
        private void CargarLista(Evento evento)
        {
            dgvLista.Rows.Clear();
            List<ListaEspera> lista = GestorListaEspera.Instance.DeEvento(evento.Id);
            int posicion = 0;
            foreach (ListaEspera inscripcion in lista)
            {
                posicion++;
                dgvLista.Rows.Add(posicion,
                    $"{inscripcion.Usuario.Nombre} {inscripcion.Usuario.Apellido}".Trim(),
                    inscripcion.Usuario.Mail,
                    inscripcion.ConSuscripcion ? "Sí" : "No",
                    inscripcion.FechaAlta.ToString("dd/MM/yyyy HH:mm"));
            }
            dgvLista.ClearSelection();
            int suscriptores = lista.Count(x => x.ConSuscripcion);
            lblCantidad.Text = lista.Count == 0
                ? "Nadie se anotó todavía en la lista de este evento."
                : $"{lista.Count} inscripto(s), {suscriptores} con suscripción. " +
                  "Los suscriptores van primero; dentro de cada grupo, por orden de inscripción.";
        }

        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.IDUsuario = IDUsuario;
            frm.NombreUsuario = NombreUsuario;
            frm.Show(); this.Hide();
        }
    }
}
