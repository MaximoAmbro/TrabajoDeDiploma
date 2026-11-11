using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // Una fila por entrada (CantidadVendida = 1): cada una lleva su propio QR (RN-18) y su propia
    // póliza de seguro. Nacen Pendiente al reservar y pasan a Confirmada al aprobarse el pago.
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

        // RN-18: el predicado y la escritura son una única sentencia, también con dos lectores.
        public bool RegistrarIngreso(string codigo, int idEvento, DateTime ahora)
        {
            const string sql = "UPDATE Entradas SET FechaIngreso = @Ahora " +
                "WHERE CodigoQR = @Codigo AND ID_Evento = @IdEvento " +
                "AND Estado = 'Confirmada' AND FechaIngreso IS NULL";
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@Ahora", ahora);
                AgregarParametro(comando, "@Codigo", codigo);
                AgregarParametro(comando, "@IdEvento", idEvento);
                return comando.ExecuteNonQuery() == 1;
            }
        }

        public Entrada PorCodigo(string codigo)
        {
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion,
                "SELECT ID, ID_Evento, Estado, FechaIngreso FROM Entradas WHERE CodigoQR = @Codigo"))
            {
                AgregarParametro(comando, "@Codigo", codigo);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read()) return null;
                    return new Entrada {
                        Id = Convert.ToInt32(lector["ID"]), IdEvento = Convert.ToInt32(lector["ID_Evento"]),
                        Estado = (EstadoEntrada)Enum.Parse(typeof(EstadoEntrada), LeerString(lector, "Estado")),
                        FechaIngreso = lector["FechaIngreso"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(lector["FechaIngreso"])
                    };
                }
            }
        }

        public int ContarIngresos(int idEvento)
        {
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion,
                "SELECT COUNT(*) FROM Entradas WHERE ID_Evento = @Evento AND FechaIngreso IS NOT NULL"))
            {
                AgregarParametro(comando, "@Evento", idEvento);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public List<Entrada> ObtenerPorCompra(int idCompra)
        {
            const string consulta =
                "SELECT ID, ID_Evento, ID_Sector, ID_Usuario, ID_Compra, Precio, CantidadVendida, " +
                "       FechaVenta, Estado, CodigoQR, FechaIngreso " +
                "FROM Entradas WHERE ID_Compra = @IdCompra ORDER BY ID";
            var entradas = new List<Entrada>();

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@IdCompra", idCompra);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        EstadoEntrada estado;
                        Enum.TryParse(LeerString(lector, "Estado"), out estado);
                        entradas.Add(new Entrada
                        {
                            Id = Convert.ToInt32(lector["ID"]),
                            IdEvento = Convert.ToInt32(lector["ID_Evento"]),
                            IdSector = Convert.ToInt32(lector["ID_Sector"]),
                            IdUsuario = Convert.ToInt32(lector["ID_Usuario"]),
                            IdCompra = Convert.ToInt32(lector["ID_Compra"]),
                            Precio = LeerDecimal(lector, "Precio"),
                            CantidadVendida = Convert.ToInt32(lector["CantidadVendida"]),
                            FechaVenta = Convert.ToDateTime(lector["FechaVenta"]),
                            Estado = estado,
                            CodigoQR = LeerString(lector, "CodigoQR"),
                            FechaIngreso = lector["FechaIngreso"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(lector["FechaIngreso"])
                        });
                    }
                }
            }
            return entradas;
        }

        // Inserta las entradas de la compra con su ID_Compra y su CodigoQR (null mientras estén
        // Pendiente). Lo llama RepositorioCompras dentro de su transacción.
        internal void Insertar(List<Entrada> entradas, int idCompra, SqlConnection conexion, SqlTransaction transaccion)
        {
            const string consulta =
                "INSERT INTO Entradas (ID_Usuario, ID_Evento, ID_Sector, ID_Compra, Precio, FechaVenta, " +
                "                      CantidadVendida, Estado, CodigoQR) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@IDUsuario, @IDEvento, @IDSector, @IDCompra, @Precio, @FechaVenta, " +
                "        @Cantidad, @Estado, @CodigoQR)";

            foreach (Entrada entrada in entradas)
            {
                using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                {
                    AgregarParametro(comando, "@IDUsuario", entrada.IdUsuario);
                    AgregarParametro(comando, "@IDEvento", entrada.IdEvento);
                    AgregarParametro(comando, "@IDSector", entrada.IdSector);
                    AgregarParametro(comando, "@IDCompra", idCompra);
                    AgregarParametro(comando, "@Precio", entrada.Precio);
                    AgregarParametro(comando, "@FechaVenta", entrada.FechaVenta);
                    AgregarParametro(comando, "@Cantidad", entrada.CantidadVendida);
                    AgregarParametro(comando, "@Estado", entrada.Estado.ToString());
                    AgregarParametro(comando, "@CodigoQR", entrada.CodigoQR);
                    entrada.Id = Convert.ToInt32(comando.ExecuteScalar());
                    entrada.IdCompra = idCompra;
                }
            }
        }

        // Pasa a Confirmada cada entrada Pendiente con su CodigoQR. El índice UX_Entradas_CodigoQR
        // rechaza un código repetido (RN-18) y la transacción del llamador se deshace entera.
        internal void Confirmar(List<Entrada> entradas, DateTime fechaVenta, SqlConnection conexion, SqlTransaction transaccion)
        {
            const string consulta =
                "UPDATE Entradas SET Estado = 'Confirmada', CodigoQR = @CodigoQR, FechaVenta = @FechaVenta " +
                "WHERE ID = @ID AND Estado = 'Pendiente'";

            foreach (Entrada entrada in entradas)
            {
                using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                {
                    AgregarParametro(comando, "@CodigoQR", entrada.CodigoQR);
                    AgregarParametro(comando, "@FechaVenta", fechaVenta);
                    AgregarParametro(comando, "@ID", entrada.Id);
                    if (comando.ExecuteNonQuery() != 1)
                    {
                        throw new InvalidOperationException(
                            $"La entrada {entrada.Id} ya no está pendiente.");
                    }
                }
            }
        }
    }
}
