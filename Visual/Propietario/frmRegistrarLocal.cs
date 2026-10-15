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
    public partial class frmRegistrarLocal : Form
    {
        public int IDUsuario;
        public string NombreUsuario;
        public SqlConnection ConexionSql; // TODO Seccion 5-7: quitar ConexionSql
        public frmRegistrarLocal()
        {
            InitializeComponent();
        }

        private void lblyatengocuen_Click(object sender, EventArgs e)
        {
            Cerrar();
        }
        private void btnregistrarse_Click(object sender, EventArgs e)
        {
            int capacidad;
            if (!int.TryParse(txtCapacidad.Text.Trim(), out capacidad))
            {
                MessageBox.Show("La capacidad debe ser un número entero.");
                return;
            }

            Local local = new Local
            {
                Nombre = txtNombre.Text,
                Direccion = txtDireccion.Text + " " + txtAltura.Text,
                Capacidad = capacidad,
                IdPropietario = IDUsuario
            };
            try
            {
                GestorLocales.Instance.Registrar(local);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Local registrado correctamente");
            Cerrar();
        }
        private void Cerrar()
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.ConexionSql = ConexionSql;
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
        }

        private void frmRegistrarLocal_Load(object sender, EventArgs e)
        {

        }
    }
}
