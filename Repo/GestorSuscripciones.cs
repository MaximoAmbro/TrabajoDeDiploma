using System;
using System.Collections.Generic;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-13 + CU-06. La suscripción no da descuento porcentual (RN-37): sus beneficios son el
    // seguro sin cargo (GestorFidelizacion), la preventa (RN-39, GestorPaquetes) y la prioridad en
    // la lista de espera (RN-35, GestorListaEspera).
    //
    // Vigencia: desde FechaInicio hasta el día anterior a FechaFin. El día de FechaFin ya está
    // vencida. RN-26: no hay renovación automática; vencida, TieneActiva devuelve false aunque la
    // fila siga con Activa = 1.
    public sealed class GestorSuscripciones
    {
        public const string PlanMensual = "Mensual";
        public const string PlanAnual = "Anual";

        public static readonly IReadOnlyList<string> Planes = new[] { PlanMensual, PlanAnual };

        private static GestorSuscripciones _instance;

        public static GestorSuscripciones Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorSuscripciones();
                }
                return _instance;
            }
        }

        private GestorSuscripciones() { }

        // RN-25: no se contrata si ya hay una vigente. La verificación previa da el mensaje; el
        // índice UX_Suscripcion_Activa cubre la carrera entre dos ventanas.
        public Suscripcion Contratar(int idUsuario, string plan)
        {
            decimal precio = PrecioDe(plan);
            if (TieneActiva(idUsuario))
            {
                throw YaTieneUna(ActivaDe(idUsuario));
            }

            DateTime hoy = DateTime.Today;
            var suscripcion = new Suscripcion
            {
                IdUsuario = idUsuario,
                Plan = plan,
                FechaInicio = hoy,
                FechaFin = plan == PlanAnual ? hoy.AddYears(1) : hoy.AddMonths(1),
                Precio = precio,
                Activa = true
            };
            try
            {
                RepositorioSuscripciones.Instance.Insertar(suscripcion, hoy);
            }
            catch (RegistroDuplicadoException)
            {
                throw YaTieneUna(ActivaDe(idUsuario));
            }
            return suscripcion;
        }

        // RN-26: la baja es manual. Pone Activa en 0, lo que libera el índice para contratar otra.
        public void Cancelar(int idUsuario)
        {
            if (!RepositorioSuscripciones.Instance.Cancelar(idUsuario))
            {
                throw new InvalidOperationException("No tiene una suscripción activa para cancelar.");
            }
        }

        // La suscripción vigente del usuario, o null si no tiene o se le venció (RN-26).
        public Suscripcion ActivaDe(int idUsuario)
        {
            Suscripcion suscripcion = RepositorioSuscripciones.Instance.ObtenerActiva(idUsuario);
            return suscripcion != null && EstaVigente(suscripcion, DateTime.Today) ? suscripcion : null;
        }

        public bool TieneActiva(int idUsuario)
        {
            return ActivaDe(idUsuario) != null;
        }

        // RF-13: precios en la tabla Parametros.
        public decimal PrecioDe(string plan)
        {
            switch (plan)
            {
                case PlanMensual:
                    return GestorParametros.Instance.ObtenerDecimal("SUSCRIPCION_MENSUAL_PRECIO");
                case PlanAnual:
                    return GestorParametros.Instance.ObtenerDecimal("SUSCRIPCION_ANUAL_PRECIO");
                default:
                    throw new ArgumentException("El plan debe ser " + string.Join(" o ", Planes) + ".");
            }
        }

        // ------------------------------------- privados -------------------------------------

        private static bool EstaVigente(Suscripcion s, DateTime hoy)
        {
            return s.Activa && s.FechaInicio.Date <= hoy.Date && hoy.Date < s.FechaFin.Date;
        }

        private static ReglaNegocioException YaTieneUna(Suscripcion actual)
        {
            string detalle = actual == null
                ? ""
                : $" Su plan {actual.Plan} vence el {actual.FechaFin:dd/MM/yyyy}.";
            return new ReglaNegocioException("RN-25",
                "Ya tiene una suscripción activa: no se pueden tener dos al mismo tiempo." + detalle);
        }
    }
}
