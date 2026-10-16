using System;
using System.Linq;
using Entidades;

namespace Negocio
{
    // RF-06: pasarela de pago simulada. No toca la red y decide por una regla fija, para que las
    // pruebas sean deterministas: una tarjeta terminada en 0000 se rechaza, el resto se aprueba.
    // El número completo no sale de acá: el resultado solo trae los últimos 4 y un token ficticio.
    public sealed class PasarelaSimulada
    {
        public const string TerminacionRechazada = "0000";

        private static PasarelaSimulada _instance;

        public static PasarelaSimulada Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PasarelaSimulada();
                }
                return _instance;
            }
        }

        private PasarelaSimulada() { }

        public ResultadoPago Cobrar(DatosTarjeta t, decimal monto)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }
            string digitos = new string((t.Numero ?? "").Where(char.IsDigit).ToArray());
            if (digitos.Length < 4)
            {
                throw new ArgumentException("El número de tarjeta no es válido.");
            }

            var resultado = new ResultadoPago
            {
                Ultimos4 = digitos.Substring(digitos.Length - 4),
                Token = "tok_sim_" + Guid.NewGuid().ToString("N")
            };
            if (monto <= 0m)
            {
                resultado.Aprobado = false;
                resultado.Motivo = "Importe inválido.";
            }
            else if (resultado.Ultimos4 == TerminacionRechazada)
            {
                resultado.Aprobado = false;
                resultado.Motivo = "Rechazada por el emisor (simulación: tarjeta terminada en 0000).";
            }
            else
            {
                resultado.Aprobado = true;
            }
            return resultado;
        }
    }

    // Respuesta de la pasarela. Sin el número de tarjeta (RN-30).
    public class ResultadoPago
    {
        public bool Aprobado { get; set; }
        public string Ultimos4 { get; set; }
        public string Token { get; set; }
        public string Motivo { get; set; }
    }
}
