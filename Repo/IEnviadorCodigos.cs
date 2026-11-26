using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static string RutaBandejaActual { get { return Carpeta; } }

        public void Enviar(string correo, string proposito, string codigo, DateTime venceEl)
        {
            lock (Candado)
            {
                Directory.CreateDirectory(Carpeta);
                string ruta = Ruta(correo, proposito);
                string temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
                var mensaje = new MensajeCorreoSimulado
                {
                    Correo = correo ?? string.Empty,
                    Proposito = proposito ?? string.Empty,
                    Codigo = codigo ?? string.Empty,
                    VenceEl = venceEl.ToUniversalTime()
                };
                string contenidoSms = "SMS simulado para " + LimpiarEncabezado(mensaje.Correo) + "\r\n" +
                    "Código: " + mensaje.Codigo + "\r\n" +
                    "Vence: " + mensaje.VenceEl.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss zzz", CultureInfo.GetCultureInfo("es-AR")) + "\r\n";
                string contenido = mensaje.Proposito == "TelefonoOrg" ? contenidoSms :
                    "From: EventAura <eventaura@simulado.invalid>\r\n" +
                    "To: " + LimpiarEncabezado(mensaje.Correo) + "\r\n" +
                    "Subject: " + Asunto(mensaje.Proposito) + "\r\n" +
                    "Date: " + DateTimeOffset.UtcNow.ToString("r", CultureInfo.InvariantCulture) + "\r\n" +
                    "MIME-Version: 1.0\r\n" +
                    "Content-Type: text/plain; charset=utf-8\r\n" +
                    "Content-Transfer-Encoding: 8bit\r\n" +
                    "X-EventAura-Purpose: " + LimpiarEncabezado(mensaje.Proposito) + "\r\n\r\n" +
                    "Código: " + mensaje.Codigo + "\r\n" +
                    "Vence: " + mensaje.VenceEl.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss zzz", CultureInfo.GetCultureInfo("es-AR")) + "\r\n" +
                    "VenceUtcTicks: " + mensaje.VenceEl.Ticks.ToString(CultureInfo.InvariantCulture) + "\r\n";
                File.WriteAllText(temporal, contenido, new UTF8Encoding(false));
                if (File.Exists(ruta)) File.Delete(ruta);
                File.Move(temporal, ruta);
            }
        }

        internal static MensajeCorreoSimulado Ultimo(string correo, string proposito)
        {
            lock (Candado)
                return Leer(Ruta(correo, proposito)).LastOrDefault(m => string.Equals(m.Proposito, proposito, StringComparison.Ordinal));
        }

        internal static string DirectorioDeBandeja => Carpeta;
        internal static string ArchivoDeBandejaParaPruebas(string correo, string proposito = null) => Ruta(correo, proposito);

        internal static void LimpiarBandeja()
        {
            lock (Candado)
                if (Directory.Exists(Carpeta)) Directory.Delete(Carpeta, true);
        }

        private static List<MensajeCorreoSimulado> Leer(string ruta)
        {
            var mensajes = new List<MensajeCorreoSimulado>();
            if (!File.Exists(ruta)) return mensajes;
            string contenido = File.ReadAllText(ruta, Encoding.UTF8);
            if (ruta.EndsWith(".sms.txt", StringComparison.OrdinalIgnoreCase))
            {
                var lineas = contenido.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string destinatarioSms = lineas.FirstOrDefault(l => l.StartsWith("SMS simulado para ", StringComparison.Ordinal));
                string codigoSms = lineas.FirstOrDefault(l => l.StartsWith("Código: ", StringComparison.Ordinal));
                string venceSms = lineas.FirstOrDefault(l => l.StartsWith("Vence: ", StringComparison.Ordinal));
                DateTime venceLocal;
                if (destinatarioSms != null && codigoSms != null && venceSms != null &&
                    DateTime.TryParse(venceSms.Substring("Vence: ".Length), CultureInfo.GetCultureInfo("es-AR"), DateTimeStyles.AssumeLocal, out venceLocal))
                    mensajes.Add(new MensajeCorreoSimulado { Correo = destinatarioSms.Substring("SMS simulado para ".Length), Proposito = "TelefonoOrg", Codigo = codigoSms.Substring("Código: ".Length), VenceEl = venceLocal.ToUniversalTime() });
                return mensajes;
            }
            int separador = contenido.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (separador < 0) return mensajes;
            var encabezados = contenido.Substring(0, separador).Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Select(linea => linea.Split(new[] { ':' }, 2))
                .Where(partes => partes.Length == 2)
                .ToDictionary(partes => partes[0].Trim(), partes => partes[1].Trim(), StringComparer.OrdinalIgnoreCase);
            string cuerpo = contenido.Substring(separador + 4);
            var campos = cuerpo.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Select(linea => linea.Split(new[] { ':' }, 2))
                .Where(partes => partes.Length == 2)
                .ToDictionary(partes => partes[0].Trim(), partes => partes[1].Trim(), StringComparer.OrdinalIgnoreCase);
            long ticks;
            if (encabezados.TryGetValue("To", out string destino) &&
                encabezados.TryGetValue("X-EventAura-Purpose", out string proposito) &&
                campos.TryGetValue("Código", out string codigo) &&
                campos.TryGetValue("VenceUtcTicks", out string textoTicks) &&
                long.TryParse(textoTicks, NumberStyles.Integer, CultureInfo.InvariantCulture, out ticks))
                mensajes.Add(new MensajeCorreoSimulado { Correo = destino, Proposito = proposito, Codigo = codigo, VenceEl = new DateTime(ticks, DateTimeKind.Utc) });
            return mensajes;
        }

        private static string LimpiarEncabezado(string valor) => (valor ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

        private static string Asunto(string proposito)
        {
            return proposito == "Verificacion" ? "Verificacion de correo - EventAura" :
                proposito == "TelefonoOrg" ? "Verificacion de telefono - EventAura" : "Recuperacion de contrasena - EventAura";
        }

        private static string Ruta(string correo, string proposito = null)
        {
            byte[] datos = Encoding.UTF8.GetBytes((correo ?? string.Empty).Trim().ToLowerInvariant());
            using (var sha = SHA256.Create())
                return Path.Combine(Carpeta, BitConverter.ToString(sha.ComputeHash(datos)).Replace("-", string.Empty) +
                    (proposito == "TelefonoOrg" ? ".sms.txt" : ".eml"));
        }
    }

    public static class SeleccionEnviadorCodigos
    {
        // Program elige SMTP si están definidas las cinco variables de entorno; si falta alguna, usa el emisor simulado.
        public static IEnviadorCodigos Actual { get; set; } = new EnviadorSimulado();

        // El teléfono no es una dirección de correo. Hasta integrar un proveedor SMS,
        // ese propósito siempre usa el canal simulado aunque SMTP esté configurado.
        public static IEnviadorCodigos Para(string proposito)
        {
            return proposito == "TelefonoOrg" ? (IEnviadorCodigos)new EnviadorSimulado() : Actual;
        }
    }
}
