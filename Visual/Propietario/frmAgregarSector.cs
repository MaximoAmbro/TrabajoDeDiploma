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
using Microsoft.Data.SqlClient;
using Negocio;

namespace Visual.Vendedor
{
    public partial class frmAgregarSector: Form
    {
        public string NombreUsuario; public int IDlocal; public int IDEvento; public string NombreLocal; public int IDUsuario;
        public string NombreEvento;
        public SqlConnection ConexionSql; // TODO Seccion 5-7: quitar ConexionSql
        public frmAgregarSector()
        {
            InitializeComponent();
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
                    // Si el evento está en Borrador y el sector tiene capacidad, queda publicado.
                    GestorEventos.Instance.AgregarSector(IDEvento, sector);
                    MessageBox.Show("Sector agregado correctamente.");
                    txtCapacidad.Clear(); txtNombre.Clear(); txtPrecio.Clear();
                }
                catch (ReglaNegocioException ex)
                {
                    MessageBox.Show(ex.Message, ex.Regla, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
                {
                    MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void lblVolver_Click(object sender, EventArgs e)
        {
            frmAdministrarEvento form = new frmAdministrarEvento();
            form.ConexionSql = ConexionSql;
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
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCapacidad.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }

        // El precio acepta coma o punto decimal ("12500,50" o "12500.50"), sin separador de miles.
        private bool LeerSector(out Sector sector)
        {
            sector = null;
            int capacidad;
            if (!int.TryParse(txtCapacidad.Text.Trim(), out capacidad))
            {
                MessageBox.Show("La capacidad debe ser un número entero.");
                return false;
            }
            decimal precio;
            string textoPrecio = txtPrecio.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(textoPrecio, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out precio))
            {
                MessageBox.Show("El precio debe ser un número, por ejemplo 12500.50.");
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
