using System;
using System.Collections.Generic;
using System.Linq;

namespace Negocio
{
    public interface IEnviadorCodigos
    {
        void Enviar(string correo, string proposito, string codigo, DateTime venceEl);
    }

    public sealed class MensajeCorreoSimulado
    {
        public string Correo { get; set; }
        public string Proposito { get; set; }
        public string Codigo { get; set; }
        public DateTime VenceEl { get; set; }
    }

    public sealed class EnviadorSimulado : IEnviadorCodigos
    {
        private static readonly object Candado = new object();
        private static readonly List<MensajeCorreoSimulado> Bandeja = new List<MensajeCorreoSimulado>();

        public void Enviar(string correo, string proposito, string codigo, DateTime venceEl)
        {
            lock (Candado)
            {
                Bandeja.RemoveAll(m => string.Equals(m.Correo, correo, StringComparison.OrdinalIgnoreCase) && m.Proposito == proposito);
                Bandeja.Add(new MensajeCorreoSimulado { Correo = correo, Proposito = proposito, Codigo = codigo, VenceEl = venceEl });
            }
        }

        public static MensajeCorreoSimulado Ultimo(string correo, string proposito)
        {
            lock (Candado)
                return Bandeja.LastOrDefault(m => string.Equals(m.Correo, correo, StringComparison.OrdinalIgnoreCase) && m.Proposito == proposito);
        }
    }

    public static class SeleccionEnviadorCodigos
    {
        // La versión entregada siempre usa la bandeja simulada. SMTP existe como estrategia,
        // pero no se activa desde código ni se configura en el cliente.
        public static IEnviadorCodigos Actual { get; set; } = new EnviadorSimulado();
    }
}
