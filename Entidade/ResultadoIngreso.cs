using System;

namespace Entidades
{
    public enum MotivoIngreso { Valido, YaUsado, Reembolsado, NoConfirmado, OtroEvento, Inexistente, FueraDeHorario, NoAutorizado }

    public sealed class ResultadoIngreso
    {
        public MotivoIngreso Motivo { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Mensaje { get; set; }
        public string Regla { get; set; }
        public bool Valido { get { return Motivo == MotivoIngreso.Valido; } }
    }
}
