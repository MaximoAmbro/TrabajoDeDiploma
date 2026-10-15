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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Visual
{
    public partial class frmHistorialCliente : Form
    {
        public int IDUsuario;
        public SqlConnection ConexionSql;
        public string NombreUsuario { get; set; }
        public string Mail { get; set; }
        public frmHistorialCliente()
        {
            InitializeComponent();
        }
        private void frmHistorialCliente_Load(object sender, EventArgs e)
        {
            // TODO Seccion 8: mover este SQL a Datos/
            string consulta = "SELECT E.Nombre AS NombreEvento, L.Direccion AS Ubicacion, E.Fecha " +
                  "FROM Entradas EN " +
                  "JOIN Eventos E ON EN.ID_Evento = E.ID " +
                  "JOIN Locales L ON E.LocalID = L.ID " +
                  "WHERE EN.ID_Usuario = @IDUsuario";

            using (SqlCommand cmd = new SqlCommand(consulta, ConexionSql))
            {
                cmd.Parameters.AddWithValue("@IDUsuario", IDUsuario);

                SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);

                dgvHistorial.DataSource = tabla;
            }
        }
        
        // --------------------------------------LLAMADO A OTROS FORMULARIOS--------------------------------------
        private void btnVolver_Click(object sender, EventArgs e)
        {
            frmMenuCliente frm = new frmMenuCliente();
            frm.NombreUsuario = NombreUsuario;
            frm.IDUsuario = IDUsuario;
            frm.ConexionSql = ConexionSql;
            frm.Show();
            dgvHistorial.ClearSelection();
            this.Hide();
        }

    }
}
