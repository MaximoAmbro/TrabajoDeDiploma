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
            return Registrar(codigo, idEvento, DateTime.Now);
        }

        internal ResultadoIngreso Registrar(string codigo, int idEvento, DateTime ahora)
        {
            try { GestorSesion.Instance.RequerirAdministrador(); }
            catch (ReglaNegocioException ex) when (ex.Regla == "RN-34")
            {
                GestorAuditoria.Instance.Registrar("Control de ingreso", "Evento", idEvento, "Rechazado");
                var rechazoAutorizacion = Resultado(MotivoIngreso.NoAutorizado, ex.Message);
                rechazoAutorizacion.Regla = ex.Regla;
                return rechazoAutorizacion;
            }
            RequerirEventoPropio(idEvento);
            codigo = (codigo ?? "").Trim();
            Entrada entrada = RepositorioEntradas.Instance.PorCodigo(codigo);
            ResultadoIngreso rechazo = RechazoDe(entrada, idEvento);
            if (rechazo != null) return rechazo;
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            int antes = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_ANTES");
            int despues = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_DESPUES");
            if (evento == null || evento.Estado != GestorEventos.EstadoPublicado ||
                ahora < evento.Fecha.AddHours(-antes) || ahora > evento.Fecha.AddHours(despues))
                return Resultado(MotivoIngreso.FueraDeHorario, "Fuera del horario de ingreso o evento no disponible.");
            if (RepositorioEntradas.Instance.RegistrarIngreso(codigo, idEvento, ahora))
            {
                GestorAuditoria.Instance.Registrar("Control de ingreso", "Evento", idEvento, "Correcto");
                return Resultado(MotivoIngreso.Valido, "Ingreso válido. Bienvenido.", ahora);
            }
            // Otra puerta pudo registrar o reembolsar la entrada entre la lectura y el UPDATE.
            return RechazoDe(RepositorioEntradas.Instance.PorCodigo(codigo), idEvento)
                ?? Resultado(MotivoIngreso.NoConfirmado, "La entrada cambió. Volvé a escanear.");
        }

        public int Contar(int idEvento)
        {
            GestorSesion.Instance.RequerirAdministrador();
            RequerirEventoPropio(idEvento);
            return RepositorioEntradas.Instance.ContarIngresos(idEvento);
        }

        public List<Evento> EventosConIngresoAbierto()
        {
            return EventosConIngresoAbierto(DateTime.Now);
        }

        internal List<Evento> EventosConIngresoAbierto(DateTime ahora)
        {
            GestorSesion.Instance.RequerirAdministrador();
            int antes = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_ANTES");
            int despues = GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_DESPUES");
            return RepositorioEventos.Instance.ConIngresoAbierto(GestorSesion.Instance.UsuarioActual.Id,
                ahora.AddHours(-despues), ahora.AddHours(antes));
        }

        private static void RequerirEventoPropio(int idEvento)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            Local local = evento == null ? null : RepositorioLocales.Instance.ObtenerPorId(evento.IdLocal);
            if (local == null || local.IdPropietario != GestorSesion.Instance.UsuarioActual.Id)
                throw new ReglaNegocioException("RN-42", "Solo podés controlar el ingreso de eventos de tus locales.");
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
