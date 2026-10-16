using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // RF-06 + RF-07: ciclo de vida de una compra. Cada operación es una sola transacción:
    // reservar, confirmar y liberar no pueden dejar media compra escrita.
    //
    // Compras.ReservaVence indica si la compra retiene cupo: con valor, sus entradas Pendiente
    // están sumadas en Sectores.CantidadReservada; en NULL, el cupo ya se devolvió (pago
    // rechazado, RN-32) y un reintento tiene que volver a reservar.
    public sealed class RepositorioCompras : RepositorioBase
    {
        private static RepositorioCompras _instance;

        public static RepositorioCompras Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioCompras();
                }
                return _instance;
            }
        }

        private RepositorioCompras() { }

        // RN-15 + RN-16: inserta la compra Pendiente con sus importes congelados, sus servicios y sus
        // entradas Pendiente, y reserva el cupo. Lanza CupoInsuficienteException si un sector no
        // alcanza; en ese caso no queda nada escrito. Devuelve el ID de la compra.
        public int Iniciar(Compra compra, List<CompraServicio> servicios, List<Entrada> entradas)
        {
            const string insertarCompra =
                "INSERT INTO Compras (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado, " +
                "                     BaseEntradas, MontoDescuento, MontoServicios, CargoServicio, " +
                "                     MontoSeguro, TotalPagado, ReservaVence) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@IdUsuario, @FechaCompra, @Estado, @NivelAplicado, @DescuentoAplicado, " +
                "        @BaseEntradas, @MontoDescuento, @MontoServicios, @CargoServicio, " +
                "        @MontoSeguro, @TotalPagado, @ReservaVence)";
            const string insertarServicio =
                "INSERT INTO CompraServicios (ID_Compra, ID_Servicio, Cantidad, PrecioUnit) " +
                "VALUES (@IdCompra, @IdServicio, @Cantidad, @PrecioUnit)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int idCompra;
                    using (SqlCommand comando = CrearComando(conexion, insertarCompra, transaccion))
                    {
                        AgregarParametro(comando, "@IdUsuario", compra.IdUsuario);
                        AgregarParametro(comando, "@FechaCompra", compra.FechaCompra);
                        AgregarParametro(comando, "@Estado", compra.Estado.ToString());
                        AgregarParametro(comando, "@NivelAplicado", compra.NivelAplicado.ToString());
                        AgregarParametro(comando, "@DescuentoAplicado", compra.DescuentoAplicado);
                        AgregarParametro(comando, "@BaseEntradas", compra.BaseEntradas);
                        AgregarParametro(comando, "@MontoDescuento", compra.MontoDescuento);
                        AgregarParametro(comando, "@MontoServicios", compra.MontoServicios);
                        AgregarParametro(comando, "@CargoServicio", compra.CargoServicio);
                        AgregarParametro(comando, "@MontoSeguro", compra.MontoSeguro);
                        AgregarParametro(comando, "@TotalPagado", compra.TotalPagado);
                        AgregarParametro(comando, "@ReservaVence", compra.ReservaVence);
                        idCompra = Convert.ToInt32(comando.ExecuteScalar());
                    }

                    foreach (CompraServicio servicio in servicios)
                    {
                        using (SqlCommand comando = CrearComando(conexion, insertarServicio, transaccion))
                        {
                            AgregarParametro(comando, "@IdCompra", idCompra);
                            AgregarParametro(comando, "@IdServicio", servicio.IdServicio);
                            AgregarParametro(comando, "@Cantidad", servicio.Cantidad);
                            AgregarParametro(comando, "@PrecioUnit", servicio.PrecioUnit);
                            comando.ExecuteNonQuery();
                        }
                        servicio.IdCompra = idCompra;
                    }

                    RepositorioEntradas.Instance.Insertar(entradas, idCompra, conexion, transaccion);

                    int? sinCupo = RepositorioSectores.Instance.ReservarCompra(idCompra, conexion, transaccion);
                    if (sinCupo.HasValue)
                    {
                        throw new CupoInsuficienteException(sinCupo.Value);
                    }

                    transaccion.Commit();
                    compra.Id = idCompra;
                    return idCompra;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // Devuelve null si no existe.
        public Compra ObtenerPorId(int idCompra)
        {
            const string consulta =
                "SELECT ID, ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado, BaseEntradas, " +
                "       MontoDescuento, MontoServicios, CargoServicio, MontoSeguro, TotalPagado, ReservaVence " +
                "FROM Compras WHERE ID = @ID";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@ID", idCompra);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return null;
                    }
                    EstadoCompra estado;
                    Enum.TryParse(LeerString(lector, "Estado"), out estado);
                    NivelFidelidad nivel;
                    Enum.TryParse(LeerString(lector, "NivelAplicado"), out nivel);
                    return new Compra
                    {
                        Id = Convert.ToInt32(lector["ID"]),
                        IdUsuario = Convert.ToInt32(lector["ID_Usuario"]),
                        FechaCompra = Convert.ToDateTime(lector["FechaCompra"]),
                        Estado = estado,
                        NivelAplicado = nivel,
                        DescuentoAplicado = LeerDecimal(lector, "DescuentoAplicado"),
                        BaseEntradas = LeerDecimal(lector, "BaseEntradas"),
                        MontoDescuento = LeerDecimal(lector, "MontoDescuento"),
                        MontoServicios = LeerDecimal(lector, "MontoServicios"),
                        CargoServicio = LeerDecimal(lector, "CargoServicio"),
                        MontoSeguro = LeerDecimal(lector, "MontoSeguro"),
                        TotalPagado = LeerDecimal(lector, "TotalPagado"),
                        ReservaVence = LeerFechaNula(lector, "ReservaVence")
                    };
                }
            }
        }

        // RN-17: pago aprobado. Compra a Confirmada, devuelve el cupo reservado (que ahora pasa a
        // contarse como vendido), confirma las entradas con su QR y graba las pólizas.
        // Devuelve false, sin escribir nada, si la compra ya no estaba Pendiente con cupo reservado.
        public bool Confirmar(int idCompra, List<Entrada> entradas, DateTime fechaVenta, List<Seguro> seguros)
        {
            const string confirmarCompra =
                "UPDATE Compras SET Estado = 'Confirmada' " +
                "WHERE ID = @ID AND Estado = 'Pendiente' AND ReservaVence IS NOT NULL";
            const string insertarSeguro =
                "INSERT INTO Seguros (ID_Entrada, Precio, VenceEl) VALUES (@IdEntrada, @Precio, @VenceEl)";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    using (SqlCommand comando = CrearComando(conexion, confirmarCompra, transaccion))
                    {
                        AgregarParametro(comando, "@ID", idCompra);
                        if (comando.ExecuteNonQuery() != 1)
                        {
                            transaccion.Rollback();
                            return false;
                        }
                    }

                    // Antes de cambiar el estado de las entradas: LiberarCompra suma las Pendiente.
                    RepositorioSectores.Instance.LiberarCompra(idCompra, conexion, transaccion);
                    RepositorioEntradas.Instance.Confirmar(entradas, fechaVenta, conexion, transaccion);

                    foreach (Seguro seguro in seguros)
                    {
                        using (SqlCommand comando = CrearComando(conexion, insertarSeguro, transaccion))
                        {
                            AgregarParametro(comando, "@IdEntrada", seguro.IdEntrada);
                            AgregarParametro(comando, "@Precio", seguro.Precio);
                            AgregarParametro(comando, "@VenceEl", seguro.VenceEl);
                            comando.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // RN-32: devuelve el cupo de una compra Pendiente y deja ReservaVence en NULL. La compra
        // sigue Pendiente para admitir un reintento. Devuelve false si no retenía cupo.
        public bool Liberar(int idCompra)
        {
            const string soltarReserva =
                "UPDATE Compras SET ReservaVence = NULL " +
                "WHERE ID = @ID AND Estado = 'Pendiente' AND ReservaVence IS NOT NULL";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    using (SqlCommand comando = CrearComando(conexion, soltarReserva, transaccion))
                    {
                        AgregarParametro(comando, "@ID", idCompra);
                        if (comando.ExecuteNonQuery() != 1)
                        {
                            transaccion.Rollback();
                            return false;
                        }
                    }
                    RepositorioSectores.Instance.LiberarCompra(idCompra, conexion, transaccion);
                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // Reintento tras un rechazo: vuelve a reservar el cupo de las entradas Pendiente hasta
        // reservaVence. Lanza CupoInsuficienteException si ya no alcanza (y no reserva nada).
        // Devuelve false si la compra no estaba Pendiente sin reserva.
        public bool Rereservar(int idCompra, DateTime reservaVence)
        {
            const string tomarReserva =
                "UPDATE Compras SET ReservaVence = @ReservaVence " +
                "WHERE ID = @ID AND Estado = 'Pendiente' AND ReservaVence IS NULL";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    using (SqlCommand comando = CrearComando(conexion, tomarReserva, transaccion))
                    {
                        AgregarParametro(comando, "@ReservaVence", reservaVence);
                        AgregarParametro(comando, "@ID", idCompra);
                        if (comando.ExecuteNonQuery() != 1)
                        {
                            transaccion.Rollback();
                            return false;
                        }
                    }
                    int? sinCupo = RepositorioSectores.Instance.ReservarCompra(idCompra, conexion, transaccion);
                    if (sinCupo.HasValue)
                    {
                        throw new CupoInsuficienteException(sinCupo.Value);
                    }
                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // RN-15: pasa a Vencida las compras Pendiente cuyo plazo terminó y devuelve el cupo de las
        // que todavía lo retenían. El plazo es ReservaVence; si ya es NULL (cupo devuelto por un
        // rechazo), FechaCompra + minutosReserva. El UPDATE con OUTPUT toma cada compra una sola
        // vez aunque dos pantallas corran esto a la vez. Devuelve cuántas compras venció.
        public int MarcarVencidas(DateTime ahora, int minutosReserva)
        {
            const string consulta =
                "DECLARE @Vencidas TABLE (ID INT, TeniaReserva BIT); " +
                "UPDATE Compras SET Estado = 'Vencida' " +
                "OUTPUT INSERTED.ID, CASE WHEN DELETED.ReservaVence IS NULL THEN 0 ELSE 1 END INTO @Vencidas " +
                "WHERE Estado = 'Pendiente' " +
                "  AND (   (ReservaVence IS NOT NULL AND ReservaVence <= @Ahora) " +
                "       OR (ReservaVence IS NULL AND DATEADD(MINUTE, @Minutos, FechaCompra) <= @Ahora)); " +
                "UPDATE s SET s.CantidadReservada = s.CantidadReservada - r.Cantidad " +
                "FROM Sectores s " +
                "JOIN (SELECT e.ID_Sector, SUM(e.CantidadVendida) AS Cantidad " +
                "      FROM Entradas e JOIN @Vencidas v ON v.ID = e.ID_Compra AND v.TeniaReserva = 1 " +
                "      WHERE e.Estado = 'Pendiente' GROUP BY e.ID_Sector) r ON r.ID_Sector = s.ID; " +
                "UPDATE e SET e.Estado = 'Vencida' " +
                "FROM Entradas e JOIN @Vencidas v ON v.ID = e.ID_Compra " +
                "WHERE e.Estado = 'Pendiente'; " +
                "SELECT COUNT(*) FROM @Vencidas;";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                try
                {
                    int cantidad;
                    using (SqlCommand comando = CrearComando(conexion, consulta, transaccion))
                    {
                        AgregarParametro(comando, "@Ahora", ahora);
                        AgregarParametro(comando, "@Minutos", minutosReserva);
                        cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    }
                    transaccion.Commit();
                    return cantidad;
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
