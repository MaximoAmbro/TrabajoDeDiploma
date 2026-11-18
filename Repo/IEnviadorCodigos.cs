using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
        internal static string DirectorioBandeja { get; set; }

        private static string Carpeta
        {
            get
            {
                return string.IsNullOrWhiteSpace(DirectorioBandeja)
                    ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EventAura", "BandejaSimulada")
                    : DirectorioBandeja;
            }
        }

        public void Enviar(string correo, string proposito, string codigo, DateTime venceEl)
        {
            lock (Candado)
            {
                Directory.CreateDirectory(Carpeta);
                string ruta = Ruta(correo, proposito);
                string temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllLines(temporal, new[] {
                    correo ?? string.Empty, proposito ?? string.Empty, codigo ?? string.Empty,
                    venceEl.ToUniversalTime().Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture)
                }, new UTF8Encoding(false));
                if (File.Exists(ruta)) File.Delete(ruta);
                File.Move(temporal, ruta);
            }
        }

        public static MensajeCorreoSimulado Ultimo(string correo, string proposito)
        {
            lock (Candado)
            {
                string ruta = Ruta(correo, proposito);
                if (!File.Exists(ruta)) return null;
                string[] datos = File.ReadAllLines(ruta, Encoding.UTF8);
                long ticks;
                if (datos.Length != 4 || !long.TryParse(datos[3], out ticks)) return null;
                return new MensajeCorreoSimulado {
                    Correo = datos[0], Proposito = datos[1], Codigo = datos[2],
                    VenceEl = new DateTime(ticks, DateTimeKind.Utc)
                };
            }
        }

        internal static void LimpiarBandeja()
        {
            lock (Candado)
                if (Directory.Exists(Carpeta)) Directory.Delete(Carpeta, true);
        }

        private static string Ruta(string correo, string proposito)
        {
            byte[] datos = Encoding.UTF8.GetBytes(((correo ?? string.Empty).Trim().ToLowerInvariant()) + "\0" + (proposito ?? string.Empty));
            using (var sha = SHA256.Create())
                return Path.Combine(Carpeta, BitConverter.ToString(sha.ComputeHash(datos)).Replace("-", string.Empty) + ".eml");
        }
    }

    public static class SeleccionEnviadorCodigos
    {
        // La versión entregada siempre usa la bandeja simulada. SMTP existe como estrategia,
        // pero no se activa desde código ni se configura en el cliente.
        public static IEnviadorCodigos Actual { get; set; } = new EnviadorSimulado();
    }
}
