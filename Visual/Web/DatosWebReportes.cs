using System;
using System.Data;
using System.Linq;
using Negocio;

namespace Visual.Web
{
    internal static class DatosWebReportes
    {
        internal static object Crear(Reporte reporte, DataTable tabla)
        {
            var columnas = tabla.Columns.Cast<DataColumn>().ToArray();
            var filas = tabla.Rows.Cast<DataRow>().ToArray();
            bool hayTotal = Equals(tabla.ExtendedProperties[Reporte.PropiedadFilaTotales], true);
            var detalle = hayTotal ? filas.Take(filas.Length - 1).ToArray() : filas;

            var barras = Array.Empty<Punto>();
            var categorias = Array.Empty<Punto>();
            var temporal = Array.Empty<Punto>();
            if (reporte is ReporteVentasPorEvento)
            {
                barras = detalle.Select(fila => new Punto(Convert.ToString(fila[0]), Cantidad(fila[2]))).ToArray();
                categorias = detalle.GroupBy(fila => Convert.ToString(fila[1]))
                    .Select(grupo => new Punto(grupo.Key, grupo.Sum(fila => Cantidad(fila[2])))).ToArray();

                // RPT-01 agrupa las ventas por evento. Vinculamos los nombres inequívocos con
                // la fecha del catálogo de reportes, sin inferir fechas de transacciones.
                if (detalle.Length != 0)
                {
                    var fechas = GestorReportes.Instance.EventosEntre(reporte.Desde, reporte.Hasta)
                        .GroupBy(evento => evento.Nombre, StringComparer.OrdinalIgnoreCase)
                        .Where(grupo => grupo.Count() == 1)
                        .ToDictionary(grupo => grupo.Key, grupo => grupo.Single().Fecha.Date, StringComparer.OrdinalIgnoreCase);
                    temporal = detalle.Where(fila => fechas.ContainsKey(Convert.ToString(fila[0])))
                        .GroupBy(fila => fechas[Convert.ToString(fila[0])])
                        .OrderBy(grupo => grupo.Key)
                        .Select(grupo => new Punto(grupo.Key.ToString("dd/MM/yyyy"), grupo.Sum(fila => Cantidad(fila[2])))).ToArray();
                }
            }

            return new
            {
                titulo = reporte.Titulo,
                columnas = columnas.Select(columna => columna.ColumnName).ToArray(),
                filas = filas.Select(fila => columnas.Select(columna => Formatear(fila[columna], columna)).ToArray()).ToArray(),
                numericas = columnas.Select(columna => columna.ExtendedProperties.ContainsKey(Reporte.PropiedadFormato) ||
                    columna.ExtendedProperties.ContainsKey(Reporte.PropiedadDerecha)).ToArray(),
                hayTotal, barras, categorias, temporal
            };
        }

        private static decimal Cantidad(object valor) => valor == DBNull.Value ? 0 : Convert.ToDecimal(valor);

        private static string Formatear(object valor, DataColumn columna)
        {
            if (valor == DBNull.Value) return "—";
            string formato = columna.ExtendedProperties[Reporte.PropiedadFormato] as string;
            if (valor is IFormattable dato)
                return dato.ToString(formato == "C0" ? "C2" : formato, Reporte.Cultura);
            return Convert.ToString(valor, Reporte.Cultura);
        }

        private sealed class Punto
        {
            public string etiqueta { get; }
            public decimal valor { get; }

            public Punto(string etiqueta, decimal valor)
            {
                this.etiqueta = etiqueta;
                this.valor = valor;
            }
        }
    }
}