using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    public sealed class GestorIngresos
    {
        private static readonly GestorIngresos instancia = new GestorIngresos();
        public static GestorIngresos Instance { get { return instancia; } }
        private GestorIngresos() { }

        public ResultadoIngreso Registrar(string codigo, int idEvento)
        {
            GestorSesion.Instance.RequerirAdministrador();
            codigo = (codigo ?? "").Trim();
            Entrada entrada = RepositorioEntradas.Instance.PorCodigo(codigo);
            ResultadoIngreso rechazo = RechazoDe(entrada, idEvento);
            if (rechazo != null) return rechazo;
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            DateTime ahora = DateTime.Now;
            int antes = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_ANTES");
            int despues = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_DESPUES");
            if (evento == null || evento.Estado != GestorEventos.EstadoPublicado ||
                ahora < evento.Fecha.AddHours(-antes) || ahora > evento.Fecha.AddHours(despues))
                return Resultado(MotivoIngreso.FueraDeHorario, "Fuera del horario de ingreso o evento no disponible.");
            if (RepositorioEntradas.Instance.RegistrarIngreso(codigo, idEvento, ahora))
                return Resultado(MotivoIngreso.Valido, "Ingreso válido. Bienvenido.", ahora);
            // Otra puerta pudo registrar o reembolsar la entrada entre la lectura y el UPDATE.
            return RechazoDe(RepositorioEntradas.Instance.PorCodigo(codigo), idEvento)
                ?? Resultado(MotivoIngreso.NoConfirmado, "La entrada cambió. Vuelva a escanear.");
        }

        public int Contar(int idEvento)
        {
            GestorSesion.Instance.RequerirAdministrador();
            return RepositorioEntradas.Instance.ContarIngresos(idEvento);
        }

        public List<Evento> EventosDeHoy()
        {
            GestorSesion.Instance.RequerirAdministrador();
            return RepositorioEventos.Instance.DelDia(DateTime.Today);
        }

        private static ResultadoIngreso RechazoDe(Entrada entrada, int evento)
        {
            if (entrada == null) return Resultado(MotivoIngreso.Inexistente, "El código no existe.");
            if (entrada.IdEvento != evento) return Resultado(MotivoIngreso.OtroEvento, "La entrada corresponde a otro evento.");
            if (entrada.Estado == EstadoEntrada.Reembolsada) return Resultado(MotivoIngreso.Reembolsado, "La entrada está reembolsada.");
            if (entrada.Estado != EstadoEntrada.Confirmada) return Resultado(MotivoIngreso.NoConfirmado, "La entrada está pendiente o vencida.");
            if (entrada.FechaIngreso.HasValue)
                return Resultado(MotivoIngreso.YaUsado, "La entrada ya ingresó el " + entrada.FechaIngreso.Value.ToString("dd/MM/yyyy HH:mm:ss") + ".", entrada.FechaIngreso);
            return null;
        }

        private static ResultadoIngreso Resultado(MotivoIngreso motivo, string mensaje, DateTime? fecha = null)
        {
            return new ResultadoIngreso { Motivo = motivo, Mensaje = mensaje, FechaIngreso = fecha };
        }
    }
}
