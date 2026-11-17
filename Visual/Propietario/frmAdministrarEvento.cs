using Entidades;
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
using System.Runtime.Versioning;

namespace Visual.Vendedor
{
    public partial class frmAdministrarEvento : FormularioEventAura
    {
        private Visual.Componentes.EstadoVacio estadoSinSectores;
        public int IDUsuario; public int IDlocal; public int IDEvento;
        public string NombreUsuario;
        public string NombreLocal;
        public string NombreEvento { get; set; }

        public frmAdministrarEvento()
        {
            InitializeComponent();
            estadoSinSectores = new Visual.Componentes.EstadoVacio { Name = "estadoSinSectores", Visible = false };
            estadoSinSectores.AjustarA(dgvSectores);
            estadoSinSectores.Cargar("Este evento todavía no tiene sectores. Agregá uno para ofrecer entradas.", "Agregar sector");
            estadoSinSectores.Accion += () => btnAgregarSector.PerformClick();
            Controls.Add(estadoSinSectores);
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmEventosLocal form = new frmEventosLocal();
            form.IDlocal = IDlocal;
            form.NombreUsuario = NombreUsuario;
            form.IDUsuario = IDUsuario;
            form.NombreLocal = NombreEvento;
            form.Show(); this.Hide();
        }
        private void frmAgregarEventoLocal_Load(object sender, EventArgs e)
        {
            lblEvento.Text = NombreEvento;
            CargarPreventa();
            CargarPortada();
            CargarDGV();
        }

        private void CargarPortada()
        {
            Evento evento = GestorEventos.Instance.ObtenerPorId(IDEvento);
            if (evento != null) portadaEvento.Cargar(evento.Nombre, evento.Categoria, evento.ImagenPortada);
        }

        private void btnElegirPortada_Click(object sender, EventArgs e)
        {
            using (var selector = new OpenFileDialog { Title = "Elegí una portada", Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Todos los archivos|*.*" })
            {
                if (selector.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    byte[] imagen;
                    using (var archivo = System.IO.File.OpenRead(selector.FileName)) imagen = ImagenEventoCodec.Preparar(archivo);
                    GestorEventos.Instance.CambiarImagenPortada(IDEvento, imagen);
                    CargarPortada();
                }
                catch (ArgumentException ex) { Avisos.Informativo(this, ex.Message); }
                catch (InvalidOperationException ex) { Avisos.Informativo(this, ex.Message); }
            }
        }

        // RN-39: destildado = sin preventa.
        private void CargarPreventa()
        {
            Evento evento = GestorEventos.Instance.ObtenerPorId(IDEvento);
            if (evento != null && evento.FinPreventa.HasValue)
            {
                dtpFinPreventa.Value = evento.FinPreventa.Value;
                dtpFinPreventa.Checked = true;
            }
            else
            {
                dtpFinPreventa.Checked = false;
            }
        }

        private void btnGuardarPreventa_Click(object sender, EventArgs e)
        {
            DateTime? fin = dtpFinPreventa.Checked ? dtpFinPreventa.Value : (DateTime?)null;
            try
            {
                GestorEventos.Instance.CambiarPreventa(IDEvento, fin);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Avisos.Informativo(this, ex.Message);
                CargarPreventa();
                return;
            }
            Avisos.Informativo(this, fin.HasValue
                ? $"Preventa para suscriptores hasta el {fin.Value:dd/MM/yyyy HH:mm}."
                : "El evento quedó sin preventa: pueden comprar todos los clientes.");
        }
        public void CargarDGV()
        {
            List<Sector> sectores = GestorEventos.Instance.Sectores(IDEvento);
            if (sectores.Count == 0)
            {
                dgvSectores.Visible = false;
                estadoSinSectores.Visible = true;
                return;
            }

            estadoSinSectores.Visible = false;
            dgvSectores.Visible = true;
            dgvSectores.DataSource = sectores
                .Select(s => new { ID = s.Id, s.Nombre, s.Capacidad, s.Precio })
                .ToList();
            dgvSectores.Columns["ID"].Visible = false;
        }

        private void btnAgregarSector_Click(object sender, EventArgs e)
        {
            frmAgregarSector frm = new frmAgregarSector();
            frm.IDlocal = IDlocal;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.IDEvento = IDEvento;
            frm.NombreEvento = NombreEvento;
            frm.NombreLocal=NombreLocal;
            frm.Show(); this.Close();
        }
        private void btnEliminarSector_Click(object sender, EventArgs e)
        {
            if (dgvSectores.CurrentRow != null && dgvSectores.Columns.Contains("ID"))
            {
                int idSector = Convert.ToInt32(dgvSectores.CurrentRow.Cells["ID"].Value);
                try
                {
                    GestorEventos.Instance.EliminarSector(idSector);
                }
                catch (ReglaNegocioException ex)
                {
                    Avisos.Regla("Administrar evento", ex);
                }
                catch (InvalidOperationException ex)
                {
                    Avisos.Informativo(this, ex.Message);
                }
                CargarDGV();
            }
            else
            {
                Avisos.Informativo(this, "Elegí un sector antes de eliminarlo.");
            }
        }
        private void btnVolver_Click1(object sender, EventArgs e)
        {
            frmEventosLocal frmEventosLocal = new frmEventosLocal();
            frmEventosLocal.IDlocal = IDlocal;
            frmEventosLocal.IDUsuario = IDUsuario;
            frmEventosLocal.NombreUsuario = NombreUsuario;
            frmEventosLocal.NombreLocal = NombreLocal;
            frmEventosLocal.Show(); this.Hide();
        }
    }
}
