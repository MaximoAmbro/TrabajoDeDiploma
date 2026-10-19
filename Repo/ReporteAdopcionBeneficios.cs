using System;
using System.Data;
using Datos;

namespace Negocio
{
    // RPT-04: ¿qué porcentaje de clientes contrata los beneficios opcionales?
    // Tasa de adopción = contrataciones ÷ base de referencia × 100.
    // Seguro: compras confirmadas del período con Compras.ConSeguro = 1, sobre las compras
    // confirmadas del período; el ingreso es la prima cobrada (0 para un suscriptor, RN-37).
    // Suscripciones: contratadas en el período, sobre los clientes activos del período.
    public sealed class ReporteAdopcionBeneficios : Reporte
    {
        public const string CodigoReporte = "RPT-04";

        public ReporteAdopcionBeneficios(DateTime desde, DateTime hasta)
            : base(CodigoReporte, "Adopción de seguros y suscripciones", desde, hasta) { }

        public override string Filtros()
        {
            return "Período aplicado a la fecha de compra y a la fecha de inicio de la suscripción. " +
                   "Base de usuarios: clientes con al menos una compra confirmada en el período.";
        }

        public override string[] Encabezados()
        {
            return new[] { "Beneficio", "Contrataciones", "Base de referencia", "Tasa", "Ingreso asociado" };
        }

        public override DataTable Obtener(RepositorioReportes repo)
        {
            DataRow d = repo.AdopcionBeneficios(InicioRango, FinRango,
                GestorSuscripciones.PlanMensual, GestorSuscripciones.PlanAnual).Rows[0];
            decimal compras = Numero(d["Compras"]), usuarios = Numero(d["Usuarios"]);
            decimal seguros = Numero(d["ComprasConSeguro"]), mensuales = Numero(d["Mensuales"]), anuales = Numero(d["Anuales"]);
            string baseCompras = Entero(compras) + " compras";
            string baseUsuarios = Entero(usuarios) + " usuarios";

            DataTable tabla = NuevaTabla(typeof(string), typeof(int), typeof(string), typeof(decimal), typeof(decimal));
            Formatear(tabla, 1, FormatoEntero);
            AlinearDerecha(tabla, 2);
            Formatear(tabla, 3, FormatoPorcentaje);
            Formatear(tabla, 4, FormatoPesos);

            tabla.Rows.Add("Seguro de entrada", (int)seguros, baseCompras, Porcentaje(seguros, compras), Numero(d["IngresoSeguro"]));
            tabla.Rows.Add("Suscripción mensual", (int)mensuales, baseUsuarios, Porcentaje(mensuales, usuarios), Numero(d["IngresoMensual"]));
            tabla.Rows.Add("Suscripción anual", (int)anuales, baseUsuarios, Porcentaje(anuales, usuarios), Numero(d["IngresoAnual"]));
            AgregarTotales(tabla, "TOTAL SUSCRIPTOS", (int)(mensuales + anuales), baseUsuarios,
                           Porcentaje(mensuales + anuales, usuarios), Numero(d["IngresoMensual"]) + Numero(d["IngresoAnual"]));
            return tabla;
        }

        // Qué plan aporta más ingreso y qué conviene promover; adopción del seguro.
        public override string Lectura(DataTable datos)
        {
            decimal seguros = Numero(datos.Rows[0][1]), tasaSeguro = Numero(datos.Rows[0][3]);
            decimal mensuales = Numero(datos.Rows[1][1]), anuales = Numero(datos.Rows[2][1]);
            decimal ingresoMensual = Numero(datos.Rows[1][4]), ingresoAnual = Numero(datos.Rows[2][4]);
            decimal ingresoSusc = ingresoMensual + ingresoAnual;

            string texto = seguros == 0m
                ? "Ninguna compra del período contrató el seguro de entrada."
                : $"El seguro de entrada lo contrata el {Pct(tasaSeguro)} de las compras del período.";
            if (mensuales + anuales == 0m)
            {
                return texto + " No se contrataron suscripciones en el período.";
            }
            bool ganaAnual = ingresoAnual >= ingresoMensual;
            texto += ganaAnual
                ? $" La suscripción anual, con {Entero(anuales)} contrataciones, aporta el {Pct(Porcentaje(ingresoAnual, ingresoSusc))} " +
                  "del ingreso por suscripciones: migrar suscriptores mensuales al plan anual tiene más impacto económico " +
                  "que captar nuevos mensuales."
                : $" La suscripción mensual, con {Entero(mensuales)} contrataciones, aporta el {Pct(Porcentaje(ingresoMensual, ingresoSusc))} " +
                  "del ingreso por suscripciones.";
            return texto + " La suscripción no da descuento porcentual sobre la entrada (RN-37): sus beneficios son en especie.";
        }
    }
}
