using Entidades;
using Microsoft.Data.SqlClient;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Entidades.Evento;
using static QRCoder.PayloadGenerator;
namespace Negocio
{
    public partial class GestorEventos //Lista
    {
        public string SectorA { get; set; }
        public string SectorB { get; set; }
        public string SectorC { get; set; }
        public string PrecioA { get; set; }
        public string PrecioB { get; set; }
        public string PrecioC { get; set; }
        public int TotalEntradas { get; set; }
        public string MensajeTicket { get; set; }
        public byte[] QRCodeImage { get; set; }
    }
    public partial class GestorEventos //Metodos
    {
        public void EliminarEvento(int idEvento, SqlConnection _conexion)
        {
            string consulta = "DELETE FROM Eventos WHERE ID = @ID";
            using (SqlCommand comando = new SqlCommand(consulta, _conexion)) 
            {
                comando.Parameters.AddWithValue("@ID", idEvento);
                _conexion.Open();
                int filasAfectadas = comando.ExecuteNonQuery();
                _conexion.Close();
                if (filasAfectadas == 0)
                {
                    throw new Exception("No se encontró ningún evento con ese ID.");
                }
            }
        }
        public void GenerarTicket(string NombreEvento, string NombreSector, string PrecioSector, string FechaEvento)
        {
            MensajeTicket = null;
            QRCodeImage = null;
            string hora = DateTime.Now.ToString();
            Guid idTicket = Guid.NewGuid();

                    int codigoEvento = Guid.NewGuid().GetHashCode();
                    string CodigoEvento = codigoEvento.ToString().Substring(0, 8);
                    MensajeTicket =
                        "ENTRADA " + NombreEvento + "\n" +
                        "SECTOR: " + NombreSector + "\n" +
                        "PRECIO: " + PrecioSector + "$\n" +
                        "FECHA: " + FechaEvento + "\n";

            string contenidoQR = "CÓDIGO: " + CodigoEvento;

                    using (var qrGenerator = new QRCodeGenerator())
                    {
                        var qrCodeData = qrGenerator.CreateQrCode(contenidoQR, QRCodeGenerator.ECCLevel.Q);
                        using (var qrCode = new PngByteQRCode(qrCodeData))
                        {
                            QRCodeImage = qrCode.GetGraphic(20);
                        }
                    }

        }
        public void AgregarEvento(string nombre, DateTime fecha, int IDLocal, SqlConnection _conexion, out int idEvento)
        {
                    string consulta = @"
                INSERT INTO Eventos (Nombre, Fecha, LocalID)
                VALUES (@Nombre, @Fecha, @LocalID);
                SELECT CAST(SCOPE_IDENTITY() AS int);
            ";

            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@Nombre", nombre);
                comando.Parameters.AddWithValue("@Fecha", fecha);
                comando.Parameters.AddWithValue("@LocalID", IDLocal);

                _conexion.Open();

                idEvento = (int)comando.ExecuteScalar();

                _conexion.Close();
            }
        }
        public void EliminarSector(string Nombre, int idEvento, SqlConnection _conexion)
        {
            string consulta = "DELETE FROM Sectores WHERE Nombre = @Nombre AND EventoID = @EventoID";
            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@EventoID", idEvento);
                comando.Parameters.AddWithValue("@Nombre", Nombre);
                _conexion.Open();
                int filasAfectadas = comando.ExecuteNonQuery();
                _conexion.Close();
                if (filasAfectadas == 0)
                {
                    throw new Exception("No se encontró ningún evento con ese ID.");
                }
            }
        }

        public void AgregarSector(string nombre, int capacidad, int precio, int idEvento, SqlConnection _conexion)
        {
            string consulta = "INSERT INTO Sectores (Nombre, Capacidad, Precio, EventoID) VALUES (@Nombre, @Capacidad, @Precio, @EventoID)";
            using (SqlCommand comando = new SqlCommand(consulta, _conexion))
            {
                comando.Parameters.AddWithValue("@Nombre", nombre);
                comando.Parameters.AddWithValue("@Capacidad", capacidad);
                comando.Parameters.AddWithValue("@Precio", precio);
                comando.Parameters.AddWithValue("@EventoID", idEvento);
                _conexion.Open();
                comando.ExecuteNonQuery();
                _conexion.Close();
            }
            ReasignarPrecio(idEvento, _conexion);
        }
        public void ReasignarPrecio(int idEvento, SqlConnection conexion)
        {
            List<(int SectorID, decimal Precio)> sectores = new List<(int SectorID, decimal Precio)>();
            string select = "SELECT ID, Precio FROM Sectores WHERE EventoID = @ID_Evento";
            using (SqlCommand comando = new SqlCommand(select, conexion))
            {
                comando.Parameters.AddWithValue("@ID_Evento", idEvento);
                conexion.Open();
                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int sectorID = reader.GetInt32(0);
                        decimal precio = reader.GetDecimal(1);
                        sectores.Add((sectorID, precio));
                    }
                }
                conexion.Close();
                var sectoresOrdenados = sectores.OrderByDescending(s => s.Precio).ToList();
                string update = "UPDATE Sectores SET PrecioID = @PrecioID WHERE ID = @SectorID";
                conexion.Open();
                for (int i = 0; i < sectoresOrdenados.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(update, conexion))
                    {
                        int? precioID;

                        if (i < 3)
                        {
                            precioID = i + 1;
                        }
                        else
                        {
                            precioID = null;
                        }
                        cmd.Parameters.AddWithValue("@SectorID", sectoresOrdenados[i].SectorID);
                        cmd.Parameters.AddWithValue("@PrecioID", (object)precioID ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                conexion.Close();
            }
        }
       

    }

}
    
