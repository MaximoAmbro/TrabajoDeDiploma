using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual.Vendedor
{
    public partial class frmAgregarSector: FormularioEventAura
    {
        public string NombreUsuario; public int IDlocal; public int IDEvento; public string NombreLocal; public int IDUsuario;
        public string NombreEvento;
        public frmAgregarSector()
        {
            InitializeComponent();
            txtNombre.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtNombre);
            txtCapacidad.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtCapacidad);
            txtPrecio.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtPrecio);
        }

        private void btnregistrarse_Click(object sender, EventArgs e)
        {
            if (Comprobarcampos () == true)
            {
                Sector sector;
                if (!LeerSector(out sector))
                {
                    return;
                }
                try
                {
                    // Agregar un sector no publica el evento; la publicación es una acción explícita.
                    GestorEventos.Instance.AgregarSector(IDEvento, sector);
                    Avisos.Informativo(this, "Sector agregado correctamente. Ya podés asignarle entradas.");
                    txtCapacidad.Clear(); txtNombre.Clear(); txtPrecio.Clear();
                }
                catch (ReglaNegocioException ex)
                {
                    Avisos.Regla("Agregar sector", ex);
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
                {
                    Avisos.Informativo(this, ex.Message);
                }
            }
        }

        private void lblVolver_Click(object sender, EventArgs e)
        {
            frmAdministrarEvento form = new frmAdministrarEvento();
            form.IDlocal = IDlocal;
            form.IDEvento = IDEvento;
            form.NombreEvento = NombreEvento;
            form.NombreUsuario = NombreUsuario;
            form.NombreLocal = NombreLocal;
            form.IDUsuario = IDUsuario;
            form.Show(); this.Hide();
        }

        // ------------------------------------- metodos -------------------------------------
        public bool Comprobarcampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { ValidacionCampo.Mostrar(txtNombre, "Escribí el nombre del sector."); txtNombre.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtCapacidad.Text)) { ValidacionCampo.Mostrar(txtCapacidad, "Ingresá la capacidad del sector."); txtCapacidad.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtPrecio.Text)) { ValidacionCampo.Mostrar(txtPrecio, "Ingresá el precio del sector."); txtPrecio.Focus(); return false; }
            return true;
        }

        // El precio acepta coma o punto decimal ("12500,50" o "12500.50"), sin separador de miles.
        private bool LeerSector(out Sector sector)
        {
            sector = null;
            int capacidad;
            if (!int.TryParse(txtCapacidad.Text.Trim(), out capacidad))
            {
                ValidacionCampo.Mostrar(txtCapacidad, "Usá un número entero para la capacidad."); txtCapacidad.Focus();
                return false;
            }
            decimal precio;
            string textoPrecio = txtPrecio.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(textoPrecio, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out precio))
            {
                ValidacionCampo.Mostrar(txtPrecio, "Escribí un precio, por ejemplo 12500,50."); txtPrecio.Focus();
                return false;
            }
            sector = new Sector
            {
                Nombre = txtNombre.Text.Trim(),
                Capacidad = capacidad,
                Precio = precio
            };
            return true;
        }
    }
}
