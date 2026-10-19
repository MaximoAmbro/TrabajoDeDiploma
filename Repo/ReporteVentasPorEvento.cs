using System;
using System.Data;
using System.Linq;
using Datos;

namespace Negocio
{
    // RPT-01: ¿qué eventos generan más ingresos y cuáles quedan por debajo de su capacidad?
    // Entradas vendidas = Σ entradas Confirmada. Ingreso bruto = Σ precio × cantidad.
    // Ticket promedio = ingreso ÷ entradas. % de ocupación = entradas ÷ capacidad × 100.
    // El período se aplica a la FECHA DEL EVENTO.
    public sealed class ReporteVentasPorEvento : Reporte
    {
        public const string CodigoReporte = "RPT-01";

        public ReporteVentasPorEvento(DateTime desde, DateTime hasta)
            : base(CodigoReporte, "Ventas por evento", desde, hasta) { }

        // Filtro: null = todas las categorías.
        public string Categoria { get; set; }

        public override string Filtros()
        {
            return "Categoría: " + (Categoria ?? "todas") + ". Período aplicado a la fecha del evento.";
        }

        public override string[] Encabezados()
        {
            return new[] { "Evento", "Categoría", "Entradas", "Ingreso bruto", "Ticket prom.", "% ocup." };
        }

        public override DataTable Obtener(RepositorioReportes repo)
        {
            DataTable crudo = repo.VentasPorEvento(InicioRango, FinRango, Categoria);
            DataTable tabla = NuevaTabla(typeof(string), typeof(string), typeof(int), typeof(decimal), typeof(decimal), typeof(decimal));
            Formatear(tabla, 2, FormatoEntero);
            Formatear(tabla, 3, FormatoPesos);
            Formatear(tabla, 4, FormatoPesos);
            Formatear(tabla, 5, FormatoPorcentaje);

            decimal entradas = 0m, ingreso = 0m, capacidad = 0m;
            foreach (DataRow fila in crudo.Rows)
            {
                tabla.Rows.Add(fila["Evento"], fila["Categoria"], fila["Entradas"], fila["IngresoBruto"],
                               fila["TicketPromedio"], Numero(fila["PorcOcupacion"]));
                entradas += Numero(fila["Entradas"]);
                ingreso += Numero(fila["IngresoBruto"]);
                capacidad += Numero(fila["Capacidad"]);
            }
            AgregarTotales(tabla, "TOTAL DEL PERÍODO", "—", (int)entradas, ingreso,
                           entradas == 0m ? (object)DBNull.Value : ingreso / entradas,
                           Porcentaje(entradas, capacidad));
            return tabla;
        }

        // La categoría que más factura y el evento con menor ocupación.
        public override string Lectura(DataTable datos)
        {
            var filas = datos.Rows.Cast<DataRow>().Take(FilasDeDatos(datos)).ToList();
            decimal ingreso = filas.Sum(f => Numero(f[3]));
            decimal entradas = filas.Sum(f => Numero(f[2]));
            if (ingreso == 0m)
            {
                return filas.Count == 0
                    ? "No hay eventos en el período elegido."
                    : $"Ninguno de los {filas.Count} eventos del período registra ventas confirmadas.";
            }

            var categoria = filas.GroupBy(f => Convert.ToString(f[1]))
                .Select(g => new { Nombre = g.Key, Eventos = g.Count(), Ingreso = g.Sum(f => Numero(f[3])), Entradas = g.Sum(f => Numero(f[2])) })
                .OrderByDescending(g => g.Ingreso).First();
            string texto = categoria.Eventos == 1
                ? $"Un solo evento de la categoría {categoria.Nombre} concentra"
                : $"Los {categoria.Eventos} eventos de la categoría {categoria.Nombre} concentran";
            texto += $" el {Pct(Porcentaje(categoria.Ingreso, ingreso))} del ingreso del período con el " +
                     $"{Pct(Porcentaje(categoria.Entradas, entradas))} de las entradas.";

            DataRow menor = filas.OrderBy(f => Numero(f[5])).First();
            if (filas.Count > 1)
            {
                texto += $" En el extremo opuesto, «{menor[0]}» opera al {Pct(Numero(menor[5]))} de su capacidad: " +
                         "es candidato a revisión de precio o de estrategia de difusión antes de repetirlo.";
            }
            int sinVentas = filas.Count(f => Numero(f[2]) == 0m);
            if (sinVentas > 0)
            {
                texto += $" {sinVentas} evento(s) del período no registran ventas.";
            }
            return texto;
        }
    }
}
