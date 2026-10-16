using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // RN-30 + RN-31: intentos de pago. Esta capa nunca recibe el número completo de la tarjeta:
    // Pago solo tiene los últimos 4 dígitos y el token.
    public sealed class RepositorioPagos : RepositorioBase
    {
        private static RepositorioPagos _instance;

        public static RepositorioPagos Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioPagos();
                }
                return _instance;
            }
        }

        private RepositorioPagos() { }

        public int Insertar(Pago pago)
        {
            const string consulta =
                "INSERT INTO Pagos (ID_Compra, FechaPago, Estado, Ultimos4, Token, Motivo) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@IdCompra, @FechaPago, @Estado, @Ultimos4, @Token, @Motivo)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdCompra", pago.IdCompra);
                AgregarParametro(comando, "@FechaPago", pago.FechaPago);
                AgregarParametro(comando, "@Estado", pago.Estado.ToString());
                AgregarParametro(comando, "@Ultimos4", pago.Ultimos4);
                AgregarParametro(comando, "@Token", pago.Token);
                AgregarParametro(comando, "@Motivo", pago.Motivo);
                pago.Id = Convert.ToInt32(comando.ExecuteScalar());
                return pago.Id;
            }
        }

        // En el orden en que se intentaron.
        public List<Pago> ObtenerPorCompra(int idCompra)
        {
            const string consulta =
                "SELECT ID, ID_Compra, FechaPago, Estado, Ultimos4, Token, Motivo " +
                "FROM Pagos WHERE ID_Compra = @IdCompra ORDER BY ID";
            var pagos = new List<Pago>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdCompra", idCompra);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        EstadoPago estado;
                        Enum.TryParse(LeerString(lector, "Estado"), out estado);
                        pagos.Add(new Pago
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            IdCompra = Convert.ToInt32(lector["ID_Compra"]),
                            FechaPago = Convert.ToDateTime(lector["FechaPago"]),
                            Estado = estado,
                            Ultimos4 = LeerString(lector, "Ultimos4"),
                            Token = LeerString(lector, "Token"),
                            Motivo = LeerString(lector, "Motivo")
                        });
                    }
                }
            }
            return pagos;
        }
    }
}
