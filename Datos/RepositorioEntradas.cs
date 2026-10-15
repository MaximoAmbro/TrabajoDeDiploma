using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioEntradas : RepositorioBase
    {
        private static RepositorioEntradas _instance;

        public static RepositorioEntradas Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioEntradas();
                }
                return _instance;
            }
        }

        private RepositorioEntradas() { }

        // Inserta una fila por sector, todas en una misma transacción.
        // TODO Seccion 7: agregar ID_Compra y CodigoQR cuando exista la compra con reserva y pago.
        public void Insertar(List<Entrada> entradas)
        {
            const string consulta =
                "INSERT INTO Entradas (ID_Usuario, ID_Evento, ID_Sector, Precio, FechaVenta, CantidadVendida, Estado) " +
                "VALUES (@IDUsuario, @IDEvento, @IDSector, @Precio, @FechaVenta, @Cantidad, @Estado)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    foreach (Entrada entrada in entradas)
                    {
                        using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                        {
                            AgregarParametro(comando, "@IDUsuario", entrada.IdUsuario);
                            AgregarParametro(comando, "@IDEvento", entrada.IdEvento);
                            AgregarParametro(comando, "@IDSector", entrada.IdSector);
                            AgregarParametro(comando, "@Precio", entrada.Precio);
                            AgregarParametro(comando, "@FechaVenta", entrada.FechaVenta);
                            AgregarParametro(comando, "@Cantidad", entrada.CantidadVendida);
                            AgregarParametro(comando, "@Estado", entrada.Estado.ToString());
                            comando.ExecuteNonQuery();
                        }
                    }
                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}
