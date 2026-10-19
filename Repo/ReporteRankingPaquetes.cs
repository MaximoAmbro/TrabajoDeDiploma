using System;
using System.Data;
using System.Linq;
using Datos;

namespace Negocio
{
    // RPT-02: ¿qué combinaciones de paquete elige realmente el cliente?
    // No hay catálogo de paquetes: la combinación es el conjunto de tipos de servicio de cada
    // compra confirmada y el nombre se arma con los tipos presentes. Unid. = entradas.
    // Ingreso = entradas + servicios, sin cargo por servicio ni seguro: la diferencia con RPT-01
    // son exactamente los servicios complementarios. Participación = unidades ÷ total × 100.
    public sealed class ReporteRankingPaquetes : Reporte
    {
        public const string CodigoReporte = "RPT-02";

        public ReporteRankingPaquetes(DateTime desde, DateTime hasta)
            : base(CodigoReporte, "Ranking de paquetes", desde, hasta) { }

        // Filtro: null = todos los eventos del período.
        public int? IdEvento { get; set; }
        public string NombreEvento { get; set; }

        private decimal servicios;

        public override string Filtros()
        {
            return "Evento: " + (IdEvento.HasValue ? NombreEvento : "todos") + ". Período aplicado a la fecha del evento.";
        }

        public override string[] Encabezados()
        {
            return new[] { "Paquete", "Composición", "Unid.", "Ingreso total", "Part." };
        }

        public override DataTable Obtener(RepositorioReportes repo)
        {
            DataTable crudo = repo.RankingPaquetes(InicioRango, FinRango, IdEvento);
            DataTable tabla = NuevaTabla(typeof(string), typeof(string), typeof(int), typeof(decimal), typeof(decimal));
            Formatear(tabla, 2, FormatoEntero);
            Formatear(tabla, 3, FormatoPesos);
            Formatear(tabla, 4, FormatoPorcentaje);

            decimal unidades = crudo.Rows.Cast<DataRow>().Sum(f => Numero(f["Unidades"]));
            decimal ingreso = 0m;
            servicios = 0m;
            foreach (DataRow fila in crudo.Rows)
            {
                string tipos = Convert.ToString(fila["Tipos"]);
                string nombres = fila["Servicios"] == DBNull.Value ? null : Convert.ToString(fila["Servicios"]);
                tabla.Rows.Add(
                    tipos.Length == 0 ? "Solo entrada" : "Entrada + " + tipos.Replace("|", " + "),
                    nombres == null ? "Entrada" : "Entrada + " + nombres,
                    fila["Unidades"], fila["Ingreso"],
                    Porcentaje(Numero(fila["Unidades"]), unidades));
                ingreso += Numero(fila["Ingreso"]);
                servicios += Numero(fila["MontoServicios"]);
            }
            AgregarTotales(tabla, "TOTAL", "—", (int)unidades, ingreso, unidades == 0m ? 0m : 100m);
            return tabla;
        }

        // Peso de los paquetes con servicios y la combinación de mayor ingreso por unidad.
        public override string Lectura(DataTable datos)
        {
            var filas = datos.Rows.Cast<DataRow>().Take(FilasDeDatos(datos)).ToList();
            decimal unidades = filas.Sum(f => Numero(f[2]));
            decimal ingreso = filas.Sum(f => Numero(f[3]));
            if (unidades == 0m)
            {
                return "No hay compras confirmadas de eventos del período elegido.";
            }

            var conServicios = filas.Where(f => Convert.ToString(f[0]) != "Solo entrada").ToList();
            if (conServicios.Count == 0)
            {
                return "Todas las compras del período fueron de solo entrada: ningún cliente sumó servicios.";
            }
            decimal unidadesServ = conServicios.Sum(f => Numero(f[2]));
            decimal ingresoServ = conServicios.Sum(f => Numero(f[3]));
            DataRow mejor = conServicios.OrderByDescending(f => Numero(f[3]) / Math.Max(1m, Numero(f[2]))).First();

            return $"Las entradas vendidas con algún servicio adicional ({Pct(Porcentaje(unidadesServ, unidades))} de las " +
                   $"unidades) aportan el {Pct(Porcentaje(ingresoServ, ingreso))} del ingreso total. " +
                   $"«{mejor[0]}», con el {Pct(Porcentaje(Numero(mejor[2]), unidades))} de las unidades, genera el " +
                   $"{Pct(Porcentaje(Numero(mejor[3]), ingreso))} de la facturación: es la combinación de mayor ingreso por " +
                   $"unidad. Nota metodológica: el ingreso incluye {Pesos(servicios)} de servicios complementarios, que " +
                   "es la diferencia con el ingreso por entradas de RPT-01.";
        }
    }
}
