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
using Visual.Vendedor;

namespace Visual.Propietario
{
    public partial class frmAgregarEvento : FormularioEventAura
    {
        private byte[] _imagenPortada;
        public int IDUsuario; public int IDlocal; public string NombreUsuario; public string NombreLocal;
        public frmAgregarEvento()
        {
            InitializeComponent();
            Shown += (s, e) =>
            {
                ColocarTitulo(label3);
                int margen = Disposicion.Pixeles(this, Tema.Espacio.XXL);
                txtNombre.Left = dtEvento.Left = cmbCategoria.Left = dtpFinPreventa.Left = margen;
                foreach (Control campo in new Control[] { dtEvento, cmbCategoria, dtpFinPreventa })
                {
                    Tema.EstiloSelector(campo);
                    campo.Width = Disposicion.Pixeles(this, Tema.AnchoCampo);
                }
                ColocarCampo(label1, txtNombre);
                ColocarRotulo(label2, dtEvento);
                ColocarRotulo(lblCategoria, cmbCategoria);
                ColocarRotulo(lblPreventa, dtpFinPreventa);
                ColocarRotulo(lblPortada, btnElegirPortada);
                Disposicion.AlinearAccionesFormulario(this, txtNombre, btnRegistrar, lblVolver);
            };
            // Items y no DataSource: con DataSource, WinForms preselecciona el primer valor al
            // mostrarse y la categoría dejaría de ser una elección explícita.
            cmbCategoria.Items.AddRange(GestorEventos.Categorias.ToArray<object>());
            txtNombre.TextChanged += (s, e) => portadaEvento.Cargar(string.IsNullOrWhiteSpace(txtNombre.Text) ? "Tu evento" : txtNombre.Text, CategoriaActual(), _imagenPortada);
            cmbCategoria.SelectedIndexChanged += (s, e) => portadaEvento.Cargar(string.IsNullOrWhiteSpace(txtNombre.Text) ? "Tu evento" : txtNombre.Text, CategoriaActual(), _imagenPortada);
            txtNombre.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtNombre);
            cmbCategoria.SelectedIndexChanged += (s, e) => ValidacionCampo.Limpiar(cmbCategoria);
        }

        private string CategoriaActual() => cmbCategoria.SelectedItem == null ? "Portada generada" : cmbCategoria.SelectedItem.ToString();

        private void btnElegirPortada_Click(object sender, EventArgs e)
        {
            using (var selector = new OpenFileDialog { Title = "Elegí una portada", Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Todos los archivos|*.*" })
            {
                if (selector.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    using (var archivo = System.IO.File.OpenRead(selector.FileName)) _imagenPortada = ImagenEventoCodec.Preparar(archivo);
                    portadaEvento.Cargar(string.IsNullOrWhiteSpace(txtNombre.Text) ? "Tu evento" : txtNombre.Text, CategoriaActual(), _imagenPortada);
                }
                catch (ArgumentException ex) { Avisos.Informativo(this, ex.Message); }
            }
        }

        // El evento se guarda en Borrador y se vuelve al listado. Agregar sectores no lo publica:
        // desde Administrar evento, el organizador elige Publicar cuando haya capacidad disponible.
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                ValidacionCampo.Mostrar(txtNombre, "Escribí el nombre del evento."); txtNombre.Focus();
                return;
            }
            if (cmbCategoria.SelectedItem == null)
            {
                ValidacionCampo.Mostrar(cmbCategoria, "Seleccioná la categoría del evento."); cmbCategoria.Focus();
                return;
            }

            Evento evento = new Evento
            {
                Nombre = txtNombre.Text.Trim(),
                Fecha = dtEvento.Value,
                IdLocal = IDlocal,
                Categoria = cmbCategoria.SelectedItem.ToString(),
                // RN-39: destildado = sin preventa.
                FinPreventa = dtpFinPreventa.Checked ? dtpFinPreventa.Value : (DateTime?)null,
                ImagenPortada = _imagenPortada
            };

            try
            {
                GestorEventos.Instance.Crear(evento);
            }
            catch (ArgumentException ex)
            {
                Avisos.Informativo(this, ex.Message);
                return;
            }
            var listado = VolverAlListado();
            Avisos.Informativo(listado, "Evento guardado como borrador. Agregale un sector para publicarlo.");
        }

        private void lblVolver_Click(object sender, EventArgs e)
        {
            VolverAlListado();
        }

        private frmEventosLocal VolverAlListado()
        {
            frmEventosLocal frm = new frmEventosLocal();
            frm.NombreUsuario = NombreUsuario;
            frm.IDlocal = IDlocal;
            frm.IDUsuario = IDUsuario;
            frm.NombreLocal = NombreLocal;
            frm.Show(); this.Hide();
            return frm;
        }

        // ------------------------------------- metodo -------------------------------------
    }
}
