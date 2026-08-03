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
namespace Visual
{
    public partial class frmEventosCliente : Form
    {
        public int IDUsuario;
        public SqlConnection ConexionSql;
        public string NombreUsuario;
        public frmEventosCliente()
        {
            InitializeComponent();
        }
        private void frmEventosCliente_Load(object sender, EventArgs e)
        {
                  string consulta = @"
            SELECT 
                E.Nombre AS Nombre,
                E.Fecha,
                L.Direccion AS Ubicacion,
                L.Nombre AS Complejo
            FROM Eventos E
            JOIN Locales L ON E.LocalID = L.ID
                    ";
            ;
            using (SqlCommand cmd = new SqlCommand(consulta, ConexionSql))
            {
                SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgveventos.DataSource = tabla;
            }
        } 
        private void btnComprar_Click(object sender, EventArgs e)
        {
            int IDEvento = 0;
            if (dgveventos.SelectedCells.Count > 0)
            {
                string consulta = "SELECT ID FROM Eventos WHERE Fecha = @Fecha AND Nombre = @Nombre";
                using (SqlCommand sqlcomando = new SqlCommand(consulta, ConexionSql))
                {
                    sqlcomando.Parameters.AddWithValue("@Fecha", dgveventos.SelectedCells[0].OwningRow.Cells["Fecha"].Value);
                    sqlcomando.Parameters.AddWithValue("@Nombre", dgveventos.SelectedCells[0].OwningRow.Cells["Nombre"].Value);
                    ConexionSql.Open();
                    object id= sqlcomando.ExecuteScalar();
                    IDEvento = Convert.ToInt32(id);
                    ConexionSql.Close();
                }
                // una vez creado el NombreEvento, se abre el formulario de compra
                FrmCompraCliente frm = new FrmCompraCliente();
                frm.IDUsuario = IDUsuario;
                frm.IDEvento = IDEvento;
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
