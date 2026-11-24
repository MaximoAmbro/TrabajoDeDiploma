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
    public partial class frmRegistrarLocal : FormularioEventAura
    {
        public int IDUsuario;
        public string NombreUsuario;
        public frmRegistrarLocal()
        {
            InitializeComponent();
            Shown += (s, e) =>
            {
                ColocarTitulo(label1);
                ColocarCampo(label6, txtNombre);
                ColocarCampo(label5, txtDireccion);
                ColocarCampo(lblmail, txtAltura);
                ColocarCampo(label3, txtCapacidad);
                txtAltura.Left = txtDireccion.Right + Disposicion.Pixeles(this, Tema.Espacio.S);
                ColocarRotulo(lblmail, txtAltura);
                ClientSize = new System.Drawing.Size(txtAltura.Right + Disposicion.Pixeles(this, Tema.Espacio.XL), ClientSize.Height);
            };
            txtNombre.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtNombre);
            txtDireccion.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtDireccion);
            txtAltura.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtAltura);
            txtCapacidad.TextChanged += (s, e) => ValidacionCampo.Limpiar(txtCapacidad);
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
                ValidacionCampo.Mostrar(txtCapacidad, "Escribí la capacidad con números enteros."); txtCapacidad.Focus();
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
                if (ex.Message.IndexOf("nombre", StringComparison.OrdinalIgnoreCase) >= 0)
                    ValidacionCampo.Mostrar(txtNombre, "Escribí el nombre del local.");
                else if (ex.Message.IndexOf("capacidad", StringComparison.OrdinalIgnoreCase) >= 0)
                    ValidacionCampo.Mostrar(txtCapacidad, "La capacidad no puede ser negativa.");
                else Avisos.Informativo(this, ex.Message);
                return;
            }
            var menu = Cerrar();
            Avisos.Informativo(menu, "Local registrado correctamente.");
        }
        private Visual.frmMenuPropietario Cerrar()
        {
            frmMenuPropietario frm = new frmMenuPropietario();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.Show(); this.Hide();
            return frm;
        }

        private void frmRegistrarLocal_Load(object sender, EventArgs e)
        {

        }
    }
}
