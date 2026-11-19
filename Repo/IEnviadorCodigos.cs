using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Negocio
{
    public interface IEnviadorCodigos
    {
        void Enviar(string correo, string proposito, string codigo, DateTime venceEl);
    }

    // Lectura interna para la batería. El ensamblado de la aplicación no puede consultar códigos.
    internal sealed class MensajeCorreoSimulado
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
                string ruta = Ruta(correo);
                string temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
                var mensajes = Leer(ruta)
                    .Where(m => !string.Equals(m.Proposito, proposito, StringComparison.Ordinal))
                    .ToList();
                mensajes.Add(new MensajeCorreoSimulado
                {
                    Correo = correo ?? string.Empty,
                    Proposito = proposito ?? string.Empty,
                    Codigo = codigo ?? string.Empty,
                    VenceEl = venceEl.ToUniversalTime()
                });
                string contenido = string.Join(Environment.NewLine + "---" + Environment.NewLine, mensajes.Select(m =>
                    "Para: " + m.Correo + Environment.NewLine +
                    "Asunto: " + Asunto(m.Proposito) + Environment.NewLine +
                    "Propósito: " + m.Proposito + Environment.NewLine +
                    "Código: " + m.Codigo + Environment.NewLine +
                    "Vence: " + m.VenceEl.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("es-AR")) + Environment.NewLine +
                    "VenceUtcTicks: " + m.VenceEl.Ticks.ToString(CultureInfo.InvariantCulture)));
                File.WriteAllText(temporal, contenido, new UTF8Encoding(false));
                if (File.Exists(ruta)) File.Delete(ruta);
                File.Move(temporal, ruta);
            }
        }

        internal static MensajeCorreoSimulado Ultimo(string correo, string proposito)
        {
            lock (Candado)
                return Leer(Ruta(correo)).LastOrDefault(m => string.Equals(m.Proposito, proposito, StringComparison.Ordinal));
        }

        internal static string DirectorioDeBandeja => Carpeta;
        internal static string ArchivoDeBandejaParaPruebas(string correo) => Ruta(correo);

        internal static void LimpiarBandeja()
        {
            lock (Candado)
                if (Directory.Exists(Carpeta)) Directory.Delete(Carpeta, true);
        }

        private static List<MensajeCorreoSimulado> Leer(string ruta)
        {
            var mensajes = new List<MensajeCorreoSimulado>();
            if (!File.Exists(ruta)) return mensajes;
            foreach (string bloque in Regex.Split(File.ReadAllText(ruta, Encoding.UTF8), @"\r?\n---\r?\n"))
            {
                var campos = bloque.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(linea => linea.Split(new[] { ':' }, 2))
                    .Where(partes => partes.Length == 2)
                    .ToDictionary(partes => partes[0].Trim(), partes => partes[1].Trim(), StringComparer.OrdinalIgnoreCase);
                long ticks;
                if (campos.TryGetValue("Para", out string destino) &&
                    campos.TryGetValue("Propósito", out string proposito) &&
                    campos.TryGetValue("Código", out string codigo) &&
                    campos.TryGetValue("VenceUtcTicks", out string textoTicks) &&
                    long.TryParse(textoTicks, NumberStyles.Integer, CultureInfo.InvariantCulture, out ticks))
                {
                    mensajes.Add(new MensajeCorreoSimulado
                    {
                        Correo = destino,
                        Proposito = proposito,
                        Codigo = codigo,
                        VenceEl = new DateTime(ticks, DateTimeKind.Utc)
                    });
                }
            }
            return mensajes;
        }

        private static string Asunto(string proposito)
        {
            return proposito == "Verificacion" ? "Verificá tu correo de EventAura" :
                proposito == "TelefonoOrg" ? "Verificá tu teléfono" : "Recuperá tu contraseña de EventAura";
        }

        private static string Ruta(string correo)
        {
            byte[] datos = Encoding.UTF8.GetBytes((correo ?? string.Empty).Trim().ToLowerInvariant());
            using (var sha = SHA256.Create())
                return Path.Combine(Carpeta, BitConverter.ToString(sha.ComputeHash(datos)).Replace("-", string.Empty) + ".eml");
        }
    }

    public static class SeleccionEnviadorCodigos
    {
        // La versión entregada siempre usa la estrategia simulada; SMTP no se activa ni se configura.
        public static IEnviadorCodigos Actual { get; set; } = new EnviadorSimulado();
    }
}
