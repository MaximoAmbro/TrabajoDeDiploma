using System;
using System.Data;
using System.Globalization;
using Datos;

namespace Negocio
{
    // Anexo A02: la jerarquía Reporte concentra QUÉ se informa; IExportador, CÓMO se entrega.
    // Agregar un reporte no toca el exportador y agregar un formato no toca los reportes.
    //
    // Obtener devuelve la tabla lista para mostrar: una columna por encabezado, valores con su
    // tipo (los números no se pasan a texto) y la fila de totales al final. Lo que la grilla y el
    // exportador necesitan saber viaja en ExtendedProperties, así no conocen ningún reporte:
    //   columna.ExtendedProperties[PropiedadFormato]  formato .NET de una columna numérica
    //   columna.ExtendedProperties[PropiedadDerecha]  columna de texto que se alinea a la derecha
    //   tabla.ExtendedProperties[PropiedadFilaTotales] la última fila es la de totales
    //
    // Cada reporte declara sus propios filtros como propiedades (no hay entidad Filtro) y los
    // describe en texto con Filtros().
    public abstract class Reporte
    {
        public const string PropiedadFormato = "Formato";
        public const string PropiedadDerecha = "Derecha";
        public const string PropiedadFilaTotales = "FilaTotales";

        public const string FormatoPesos = "C0";
        public const string FormatoEntero = "N0";
        public const string FormatoDecimal = "N1";
        public const string FormatoPorcentaje = "0.0'%'";

        // RN-33: importes en pesos argentinos.
        public static readonly CultureInfo Cultura = new CultureInfo("es-AR");

        protected Reporte(string codigo, string titulo, DateTime desde, DateTime hasta)
        {
            Codigo = codigo;
            Titulo = titulo;
            Desde = desde.Date;
            Hasta = hasta.Date;
        }

        public string Codigo { get; }          // RPT-01 … RPT-05
        public string Titulo { get; }
        public DateTime Desde { get; }
        public DateTime Hasta { get; }

        // Quién lo pidió: lo completa GestorReportes con el usuario en sesión.
        public string Solicitante { get; internal set; }

        // Convención del anexo: EVAU_[código]_[AAAAMMDD-inicio]_[AAAAMMDD-fin], sin extensión
        // (la pone el exportador o quien elige el formato).
        public string NombreArchivo
        {
            get { return $"EVAU_{Codigo.Replace("-", "")}_{Desde:yyyyMMdd}_{Hasta:yyyyMMdd}"; }
        }

        public abstract string[] Encabezados();
        public abstract DataTable Obtener(RepositorioReportes repo);
        public abstract string Lectura(DataTable datos);

        // Texto del bloque de filtros del PDF.
        public virtual string Filtros()
        {
            return "Sin filtros adicionales.";
        }

        // ------------------------------------- ayudas -------------------------------------

        // El rango incluye el día «hasta» entero.
        protected DateTime InicioRango
        {
            get { return Desde; }
        }

        protected DateTime FinRango
        {
            get { return Hasta.AddDays(1).AddSeconds(-1); }
        }

        // Una columna por encabezado, con el tipo indicado en el mismo orden.
        protected DataTable NuevaTabla(params Type[] tipos)
        {
            string[] encabezados = Encabezados();
            if (tipos.Length != encabezados.Length)
            {
                throw new InvalidOperationException("Cada encabezado necesita su tipo.");
            }
            var tabla = new DataTable(Codigo);
            for (int i = 0; i < encabezados.Length; i++)
            {
                tabla.Columns.Add(encabezados[i], tipos[i]);
            }
            return tabla;
        }

        protected static void Formatear(DataTable tabla, int columna, string formato)
        {
            tabla.Columns[columna].ExtendedProperties[PropiedadFormato] = formato;
        }

        protected static void AlinearDerecha(DataTable tabla, int columna)
        {
            tabla.Columns[columna].ExtendedProperties[PropiedadDerecha] = true;
        }

        protected static void AgregarTotales(DataTable tabla, params object[] valores)
        {
            tabla.Rows.Add(valores);
            tabla.ExtendedProperties[PropiedadFilaTotales] = true;
        }

        // Filas de datos, sin la de totales.
        protected static int FilasDeDatos(DataTable datos)
        {
            bool conTotales = datos.ExtendedProperties.ContainsKey(PropiedadFilaTotales);
            return Math.Max(0, datos.Rows.Count - (conTotales ? 1 : 0));
        }

        // Porcentaje con un decimal, 0,05 hacia arriba. 0 si la base es 0: nunca divide por cero.
        protected static decimal Porcentaje(decimal parte, decimal total)
        {
            return total == 0m ? 0m : Math.Round(100m * parte / total, 1, MidpointRounding.AwayFromZero);
        }

        protected static decimal Numero(object valor)
        {
            return valor == null || valor == DBNull.Value ? 0m : Convert.ToDecimal(valor, CultureInfo.InvariantCulture);
        }

        protected static object NuloSiCero(decimal total, Func<object> valor)
        {
            return total == 0m ? DBNull.Value : valor();
        }

        // Para los párrafos de lectura.
        protected static string Pesos(decimal importe)
        {
            return Math.Round(importe, 0, MidpointRounding.AwayFromZero).ToString(FormatoPesos, Cultura);
        }

        protected static string Pct(decimal porcentaje)
        {
            return porcentaje.ToString("0.0' %'", Cultura);
        }

        protected static string Entero(decimal valor)
        {
            return valor.ToString(FormatoEntero, Cultura);
        }
    }
}
