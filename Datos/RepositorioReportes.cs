using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Datos
{
    // RF-11 + anexo A02: consultas agregadas de los cinco reportes. Solo leen.
    // Devuelven los datos crudos; el formato, los totales y la lectura los arma cada Reporte.
    //
    // Los rangos [desde, hasta] llegan ya ajustados por Negocio: hasta es el último instante
    // del día, así un evento del 30/11 a las 20:30 entra en un rango que termina el 30/11.
    public sealed class RepositorioReportes : RepositorioBase
    {
        private static RepositorioReportes _instance;

        public static RepositorioReportes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RepositorioReportes();
                }
                return _instance;
            }
        }

        private RepositorioReportes() { }

        // RPT-01. Consulta corregida del anexo (docs/05-reportes.md): sin JOIN a Sectores, que
        // multiplicaba filas, con la capacidad en subconsulta escalar y solo entradas Confirmada.
        // Filtra por FECHA DEL EVENTO: el reporte es «ventas por evento», no por período de
        // compra. El LEFT JOIN deja los eventos sin ventas, con 0 entradas y 0 % de ocupación.
        // Capacidad no se muestra: la usa el reporte para la ocupación de la fila de totales.
        public DataTable VentasPorEvento(DateTime desde, DateTime hasta, string categoria)
        {
            const string consulta =
                "SELECT e.Nombre AS Evento, " +
                "       e.Categoria AS Categoria, " +
                "       ISNULL(SUM(en.CantidadVendida), 0) AS Entradas, " +
                "       ISNULL(SUM(en.CantidadVendida * en.Precio), 0) AS IngresoBruto, " +
                "       SUM(en.CantidadVendida * en.Precio) " +
                "         / NULLIF(SUM(en.CantidadVendida), 0) AS TicketPromedio, " +
                "       CAST(100.0 * ISNULL(SUM(en.CantidadVendida), 0) " +
                "         / NULLIF((SELECT SUM(s.Capacidad) FROM Sectores s WHERE s.EventoID = e.ID), 0) " +
                "         AS DECIMAL(5,1)) AS PorcOcupacion, " +
                "       ISNULL((SELECT SUM(s.Capacidad) FROM Sectores s WHERE s.EventoID = e.ID), 0) AS Capacidad " +
                "FROM   Eventos e " +
                "LEFT JOIN Entradas en ON en.ID_Evento = e.ID AND en.Estado = 'Confirmada' " +
                "WHERE  e.Fecha BETWEEN @Desde AND @Hasta " +
                "  AND  (@Categoria IS NULL OR e.Categoria = @Categoria) " +
                "GROUP BY e.ID, e.Nombre, e.Categoria " +
                "ORDER BY IngresoBruto DESC";

            return Consultar(consulta, comando =>
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                AgregarParametro(comando, "@Categoria", categoria);
            });
        }

        // RPT-02. No hay catálogo de paquetes: la combinación se deriva del CONJUNTO de tipos de
        // servicio de cada compra confirmada (Tipos vacío = solo entrada). Unidades son entradas
        // Confirmada; el ingreso es BaseEntradas + MontoServicios, sin cargo por servicio ni
        // seguro. Mismo criterio de período que RPT-01 (fecha del evento), así la diferencia
        // entre los dos reportes son exactamente los servicios.
        public DataTable RankingPaquetes(DateTime desde, DateTime hasta, int? idEvento)
        {
            const string consulta =
                "WITH CompraEvento AS ( " +
                "    SELECT c.ID, c.BaseEntradas, c.MontoServicios, " +
                "           (SELECT COUNT(*) FROM Entradas en " +
                "            WHERE en.ID_Compra = c.ID AND en.Estado = 'Confirmada') AS Unidades, " +
                "           ISNULL((SELECT STRING_AGG(t.Tipo, '|') WITHIN GROUP (ORDER BY t.Tipo) " +
                "                   FROM (SELECT DISTINCT sv.Tipo FROM CompraServicios cs " +
                "                         JOIN Servicios sv ON sv.ID = cs.ID_Servicio " +
                "                         WHERE cs.ID_Compra = c.ID) t), '') AS Tipos " +
                "    FROM Compras c " +
                "    WHERE c.Estado = 'Confirmada' " +
                "      AND EXISTS (SELECT 1 FROM Entradas en JOIN Eventos e ON e.ID = en.ID_Evento " +
                "                  WHERE en.ID_Compra = c.ID AND e.Fecha BETWEEN @Desde AND @Hasta " +
                "                    AND (@IdEvento IS NULL OR e.ID = @IdEvento)) " +
                "), Nombres AS ( " +
                "    SELECT DISTINCT ce.Tipos, sv.Nombre FROM CompraEvento ce " +
                "    JOIN CompraServicios cs ON cs.ID_Compra = ce.ID " +
                "    JOIN Servicios sv ON sv.ID = cs.ID_Servicio " +
                ") " +
                "SELECT g.Tipos, g.Compras, g.Unidades, g.Ingreso, g.MontoServicios, " +
                "       (SELECT STRING_AGG(n.Nombre, ', ') WITHIN GROUP (ORDER BY n.Nombre) " +
                "        FROM Nombres n WHERE n.Tipos = g.Tipos) AS Servicios " +
                "FROM (SELECT Tipos, COUNT(*) AS Compras, SUM(Unidades) AS Unidades, " +
                "             SUM(BaseEntradas + MontoServicios) AS Ingreso, " +
                "             SUM(MontoServicios) AS MontoServicios " +
                "      FROM CompraEvento GROUP BY Tipos) g " +
                "ORDER BY g.Unidades DESC, g.Tipos";

            return Consultar(consulta, comando =>
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                AgregarParametro(comando, "@IdEvento", idEvento);
            });
        }

        // RPT-03. Compras confirmadas de cada cliente en la ventana (desdeVentana, corte], como
        // el conteo de RN-19, y su nivel según los umbrales. Solo clientes activos: los que
        // tienen al menos una compra en la ventana.
        public DataTable UsuariosPorNivel(DateTime desdeVentana, DateTime corte, int umbralPlata, int umbralOro)
        {
            const string consulta =
                "WITH Conteo AS ( " +
                "    SELECT c.ID_Usuario, COUNT(*) AS Compras " +
                "    FROM Compras c JOIN Usuario u ON u.ID = c.ID_Usuario " +
                "    WHERE u.TipoUsuario = 'Cliente' AND c.Estado = 'Confirmada' " +
                "      AND c.FechaCompra > @Desde AND c.FechaCompra <= @Corte " +
                "    GROUP BY c.ID_Usuario " +
                ") " +
                "SELECT CASE WHEN Compras >= @Oro THEN 'Oro' WHEN Compras >= @Plata THEN 'Plata' " +
                "            ELSE 'Bronce' END AS Nivel, " +
                "       COUNT(*) AS Usuarios, SUM(Compras) AS Compras " +
                "FROM Conteo " +
                "GROUP BY CASE WHEN Compras >= @Oro THEN 'Oro' WHEN Compras >= @Plata THEN 'Plata' ELSE 'Bronce' END";

            return Consultar(consulta, comando =>
            {
                AgregarParametro(comando, "@Desde", desdeVentana);
                AgregarParametro(comando, "@Corte", corte);
                AgregarParametro(comando, "@Plata", umbralPlata);
                AgregarParametro(comando, "@Oro", umbralOro);
            });
        }

        // RPT-04. Una sola fila. Seguro: compras confirmadas del período con ConSeguro = 1, sobre
        // el total de compras confirmadas. Suscripciones: contratadas en el período (FechaInicio)
        // por plan, sobre los clientes activos (con alguna compra confirmada en el período).
        public DataTable AdopcionBeneficios(DateTime desde, DateTime hasta, string planMensual, string planAnual)
        {
            const string consulta =
                "SELECT " +
                " (SELECT COUNT(*) FROM Compras WHERE Estado = 'Confirmada' " +
                "    AND FechaCompra BETWEEN @Desde AND @Hasta) AS Compras, " +
                " (SELECT COUNT(*) FROM Compras WHERE Estado = 'Confirmada' AND ConSeguro = 1 " +
                "    AND FechaCompra BETWEEN @Desde AND @Hasta) AS ComprasConSeguro, " +
                " (SELECT ISNULL(SUM(MontoSeguro), 0) FROM Compras WHERE Estado = 'Confirmada' AND ConSeguro = 1 " +
                "    AND FechaCompra BETWEEN @Desde AND @Hasta) AS IngresoSeguro, " +
                " (SELECT COUNT(DISTINCT c.ID_Usuario) FROM Compras c JOIN Usuario u ON u.ID = c.ID_Usuario " +
                "    WHERE u.TipoUsuario = 'Cliente' AND c.Estado = 'Confirmada' " +
                "      AND c.FechaCompra BETWEEN @Desde AND @Hasta) AS Usuarios, " +
                " (SELECT COUNT(*) FROM Suscripciones WHERE [Plan] = @Mensual " +
                "    AND FechaInicio BETWEEN @Desde AND @Hasta) AS Mensuales, " +
                " (SELECT ISNULL(SUM(Precio), 0) FROM Suscripciones WHERE [Plan] = @Mensual " +
                "    AND FechaInicio BETWEEN @Desde AND @Hasta) AS IngresoMensual, " +
                " (SELECT COUNT(*) FROM Suscripciones WHERE [Plan] = @Anual " +
                "    AND FechaInicio BETWEEN @Desde AND @Hasta) AS Anuales, " +
                " (SELECT ISNULL(SUM(Precio), 0) FROM Suscripciones WHERE [Plan] = @Anual " +
                "    AND FechaInicio BETWEEN @Desde AND @Hasta) AS IngresoAnual";

            return Consultar(consulta, comando =>
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                AgregarParametro(comando, "@Mensual", planMensual);
                AgregarParametro(comando, "@Anual", planAnual);
            });
        }

        // RPT-05. Eventos del período con inscriptos en lista de espera. Agotado es derivado
        // (RN-10): se calcula con el mismo criterio que RepositorioEventos.EstaAgotado.
        public DataTable DemandaInsatisfecha(DateTime desde, DateTime hasta, int? idEvento, bool soloAgotados)
        {
            string consulta =
                "SELECT Evento, Agotado, Inscriptos, Capacidad FROM ( " +
                "    SELECT e.Nombre AS Evento, " +
                "           CAST(CASE WHEN " + RepositorioEventos.CondicionAgotado("e.ID") + " THEN 1 ELSE 0 END AS BIT) AS Agotado, " +
                "           (SELECT COUNT(*) FROM ListaEspera le WHERE le.ID_Evento = e.ID) AS Inscriptos, " +
                "           ISNULL((SELECT SUM(s.Capacidad) FROM Sectores s WHERE s.EventoID = e.ID), 0) AS Capacidad " +
                "    FROM Eventos e " +
                "    WHERE e.Fecha BETWEEN @Desde AND @Hasta " +
                "      AND (@IdEvento IS NULL OR e.ID = @IdEvento) " +
                "      AND EXISTS (SELECT 1 FROM ListaEspera le WHERE le.ID_Evento = e.ID) " +
                ") x " +
                "WHERE @SoloAgotados = 0 OR Agotado = 1 " +
                "ORDER BY Inscriptos DESC, Evento";

            return Consultar(consulta, comando =>
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                AgregarParametro(comando, "@IdEvento", idEvento);
                AgregarParametro(comando, "@SoloAgotados", soloAgotados);
            });
        }

        // Ticket promedio de las entradas Confirmada de los eventos del período (RPT-05 lo usa
        // para valorizar la demanda insatisfecha). null si no hay ventas.
        public decimal? TicketPromedio(DateTime desde, DateTime hasta)
        {
            const string consulta =
                "SELECT SUM(en.CantidadVendida * en.Precio) / NULLIF(SUM(en.CantidadVendida), 0) " +
                "FROM Entradas en JOIN Eventos e ON e.ID = en.ID_Evento " +
                "WHERE en.Estado = 'Confirmada' AND e.Fecha BETWEEN @Desde AND @Hasta";

            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                object valor = comando.ExecuteScalar();
                return valor == null || valor == DBNull.Value ? (decimal?)null : Convert.ToDecimal(valor);
            }
        }

        // Eventos del período, para el filtro por evento de RPT-02 y RPT-05.
        public DataTable EventosEntre(DateTime desde, DateTime hasta)
        {
            const string consulta =
                "SELECT ID, Nombre, Fecha FROM Eventos WHERE Fecha BETWEEN @Desde AND @Hasta ORDER BY Fecha, Nombre";

            return Consultar(consulta, comando =>
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
            });
        }

        private DataTable Consultar(string consulta, Action<SqlCommand> parametros)
        {
            var tabla = new DataTable();
            using (SqlConnection conexion = AbrirConexion())
            using (SqlCommand comando = CrearComando(conexion, consulta))
            {
                parametros(comando);
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    tabla.Load(lector);
                }
            }
            return tabla;
        }
    }
}
