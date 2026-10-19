using System;
using System.Data;
using System.Linq;
using Datos;

namespace Negocio
{
    // RPT-05: ¿cuánta venta se pierde en eventos agotados?
    // Demanda insatisfecha = inscriptos en lista de espera ÷ capacidad del evento × 100.
    // Lista los eventos del período (por fecha del evento) que tienen inscriptos. «Agotado» no
    // es un estado guardado: se deriva con RN-10 en la consulta, con el mismo criterio que
    // GestorEventos.EstaAgotado. Un evento puede tener lista y ya no estar agotado: el cupo se
    // liberó después de formarse la lista.
    public sealed class ReporteDemandaInsatisfecha : Reporte
    {
        public const string CodigoReporte = "RPT-05";
        public const string EstadoAgotado = "Agotado";
        public const string EstadoConCupo = "Con cupo";

        public ReporteDemandaInsatisfecha(DateTime desde, DateTime hasta)
            : base(CodigoReporte, "Demanda insatisfecha", desde, hasta) { }

        // Filtros: null = todos los eventos; SoloAgotados descarta los que hoy tienen cupo.
        public int? IdEvento { get; set; }
        public string NombreEvento { get; set; }
        public bool SoloAgotados { get; set; }

        // Ticket promedio de las ventas del período: valoriza la demanda en la lectura.
        private decimal? ticketPromedio;

        public override string Filtros()
        {
            return "Evento: " + (IdEvento.HasValue ? NombreEvento : "todos") + ". Estado: " +
                   (SoloAgotados ? "solo agotados" : "todos los que tienen lista de espera") +
                   ". Período aplicado a la fecha del evento.";
        }

        public override string[] Encabezados()
        {
            return new[] { "Evento", "Estado", "En lista de espera", "Capacidad", "Demanda insat." };
        }

        public override DataTable Obtener(RepositorioReportes repo)
        {
            DataTable crudo = repo.DemandaInsatisfecha(InicioRango, FinRango, IdEvento, SoloAgotados);
            ticketPromedio = repo.TicketPromedio(InicioRango, FinRango);

            DataTable tabla = NuevaTabla(typeof(string), typeof(string), typeof(int), typeof(int), typeof(decimal));
            Formatear(tabla, 2, FormatoEntero);
            Formatear(tabla, 3, FormatoEntero);
            Formatear(tabla, 4, FormatoPorcentaje);

            decimal inscriptos = 0m, capacidad = 0m;
            foreach (DataRow fila in crudo.Rows)
            {
                decimal i = Numero(fila["Inscriptos"]), c = Numero(fila["Capacidad"]);
                tabla.Rows.Add(fila["Evento"], Convert.ToBoolean(fila["Agotado"]) ? EstadoAgotado : EstadoConCupo,
                               (int)i, (int)c, Porcentaje(i, c));
                inscriptos += i;
                capacidad += c;
            }
            AgregarTotales(tabla, "TOTAL", "—", (int)inscriptos, (int)capacidad, Porcentaje(inscriptos, capacidad));
            return tabla;
        }

        public override string Lectura(DataTable datos)
        {
            var filas = datos.Rows.Cast<DataRow>().Take(FilasDeDatos(datos)).ToList();
            decimal inscriptos = filas.Sum(f => Numero(f[2]));
            if (inscriptos == 0m)
            {
                return "Ningún evento del período tiene inscriptos en lista de espera.";
            }
            string texto = $"Los {Entero(inscriptos)} inscriptos en listas de espera representan una venta potencial no capturada.";
            if (ticketPromedio.HasValue)
            {
                // Con el ticket redondeado que se muestra, así la cuenta se puede rehacer a mano.
                decimal ticket = Math.Round(ticketPromedio.Value, 0, MidpointRounding.AwayFromZero);
                texto += $" Aplicando el ticket promedio del período ({Pesos(ticket)}), la demanda " +
                         $"insatisfecha equivale a {Pesos(inscriptos * ticket)}.";
            }
            int conCupo = filas.Count(f => Convert.ToString(f[1]) == EstadoConCupo);
            if (conCupo > 0)
            {
                texto += $" {conCupo} de los {filas.Count} eventos no están agotados según RN-10: tienen inscriptos " +
                         "aunque todavía les queda cupo.";
            }
            return texto + " Es el insumo para decidir una segunda función o la habilitación de sectores adicionales.";
        }
    }
}
