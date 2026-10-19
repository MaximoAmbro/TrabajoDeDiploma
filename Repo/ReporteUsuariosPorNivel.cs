using System;
using System.Data;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    // RPT-03: ¿cómo se distribuye la base de clientes entre Bronce, Plata y Oro?
    // El nivel se calcula como RN-19: compras confirmadas en la ventana de FIDELIDAD_VENTANA_MESES
    // que termina en la fecha de corte, con los umbrales de Parametros. La base son los clientes
    // activos: los que tienen al menos una compra en esa ventana.
    public sealed class ReporteUsuariosPorNivel : Reporte
    {
        public const string CodigoReporte = "RPT-03";

        public ReporteUsuariosPorNivel(DateTime desde, DateTime hasta)
            : base(CodigoReporte, "Usuarios por nivel de fidelidad", desde, hasta)
        {
            FechaCorte = Hasta;
        }

        // Filtro: por defecto, el último día del período.
        public DateTime FechaCorte { get; set; }

        private static int Meses
        {
            get { return GestorParametros.Instance.ObtenerEntero("FIDELIDAD_VENTANA_MESES"); }
        }

        public override string Filtros()
        {
            return $"Fecha de corte: {FechaCorte:dd/MM/yyyy}. Ventana de {Meses} meses. " +
                   "Clientes con al menos una compra confirmada en la ventana.";
        }

        public override string[] Encabezados()
        {
            return new[] { "Nivel", $"Criterio (ventana de {Meses} meses)", "Usuarios", "Part.", "Compras prom.", "Desc." };
        }

        public override DataTable Obtener(RepositorioReportes repo)
        {
            int plata = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_PLATA");
            int oro = GestorParametros.Instance.ObtenerEntero("FIDELIDAD_UMBRAL_ORO");
            DateTime corte = FechaCorte.Date.AddDays(1).AddSeconds(-1);
            DataTable crudo = repo.UsuariosPorNivel(corte.AddMonths(-Meses), corte, plata, oro);

            DataTable tabla = NuevaTabla(typeof(string), typeof(string), typeof(int), typeof(decimal), typeof(decimal), typeof(decimal));
            Formatear(tabla, 2, FormatoEntero);
            Formatear(tabla, 3, FormatoPorcentaje);
            Formatear(tabla, 4, FormatoDecimal);
            Formatear(tabla, 5, FormatoPorcentaje);

            decimal usuarios = crudo.Rows.Cast<DataRow>().Sum(f => Numero(f["Usuarios"]));
            decimal compras = crudo.Rows.Cast<DataRow>().Sum(f => Numero(f["Compras"]));
            var niveles = new[]
            {
                new { Nivel = NivelFidelidad.Bronce, Criterio = $"0 a {plata - 1} compras confirmadas" },
                new { Nivel = NivelFidelidad.Plata, Criterio = $"{plata} a {oro - 1} compras confirmadas" },
                new { Nivel = NivelFidelidad.Oro, Criterio = $"{oro} o más compras confirmadas" }
            };
            foreach (var n in niveles)
            {
                DataRow fila = crudo.Rows.Cast<DataRow>().FirstOrDefault(f => Convert.ToString(f["Nivel"]) == n.Nivel.ToString());
                decimal u = fila == null ? 0m : Numero(fila["Usuarios"]);
                decimal c = fila == null ? 0m : Numero(fila["Compras"]);
                tabla.Rows.Add(n.Nivel.ToString(), n.Criterio, (int)u, Porcentaje(u, usuarios),
                               u == 0m ? (object)DBNull.Value : Math.Round(c / u, 1, MidpointRounding.AwayFromZero),
                               100m * GestorFidelizacion.Instance.PorcentajePara(n.Nivel));
            }
            AgregarTotales(tabla, "TOTAL", "—", (int)usuarios, usuarios == 0m ? 0m : 100m,
                           usuarios == 0m ? (object)DBNull.Value : Math.Round(compras / usuarios, 1, MidpointRounding.AwayFromZero),
                           DBNull.Value);
            comprasPorNivel = niveles.Select(n => crudo.Rows.Cast<DataRow>()
                    .Where(f => Convert.ToString(f["Nivel"]) == n.Nivel.ToString())
                    .Sum(f => Numero(f["Compras"])))
                .ToArray();
            return tabla;
        }

        // Compras de Bronce, Plata y Oro, de la última ejecución: la tabla muestra promedios
        // redondeados y la lectura necesita los totales exactos.
        private decimal[] comprasPorNivel = new decimal[3];

        public override string Lectura(DataTable datos)
        {
            var filas = datos.Rows.Cast<DataRow>().Take(FilasDeDatos(datos)).ToList();
            decimal usuarios = filas.Sum(f => Numero(f[2]));
            decimal compras = comprasPorNivel.Sum();
            if (usuarios == 0m)
            {
                return "Ningún cliente tiene compras confirmadas en la ventana que termina en la fecha de corte.";
            }
            decimal usuariosOro = Numero(filas[2][2]), usuariosPlata = Numero(filas[1][2]);
            DataRow mayor = filas.OrderByDescending(f => Numero(f[2])).First();

            return $"El {Pct(Porcentaje(usuariosOro, usuarios))} de usuarios Oro concentra {Entero(comprasPorNivel[2])} " +
                   $"compras sobre un total de {Entero(compras)}, es decir el {Pct(Porcentaje(comprasPorNivel[2], compras))} " +
                   $"del volumen. Sumado al nivel Plata, el {Pct(Porcentaje(usuariosOro + usuariosPlata, usuarios))} de la " +
                   $"base explica el {Pct(Porcentaje(comprasPorNivel[1] + comprasPorNivel[2], compras))} de las operaciones. " +
                   $"El nivel más numeroso es {mayor[0]} ({Pct(Numero(mayor[3]))} de la base)" +
                   (Convert.ToString(mayor[0]) == NivelFidelidad.Bronce.ToString()
                       ? ": la conversión de Bronce a Plata es el indicador crítico a vigilar."
                       : ".");
        }
    }
}
