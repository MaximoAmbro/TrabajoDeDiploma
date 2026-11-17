using System;
using System.Net;
using System.Net.Mail;
using Negocio;

namespace Visual
{
    // Strategy opcional para despliegues externos. EventAura nunca la selecciona por defecto.
    public sealed class EnviadorSmtp : IEnviadorCodigos
    {
        public void Enviar(string correo, string proposito, string codigo, DateTime venceEl)
        {
            string host = Environment.GetEnvironmentVariable("EVENTAURA_SMTP_HOST");
            string puerto = Environment.GetEnvironmentVariable("EVENTAURA_SMTP_PORT");
            string usuario = Environment.GetEnvironmentVariable("EVENTAURA_SMTP_USER");
            string clave = Environment.GetEnvironmentVariable("EVENTAURA_SMTP_PASSWORD");
            string remitente = Environment.GetEnvironmentVariable("EVENTAURA_SMTP_FROM");
            int numeroPuerto;
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(puerto) ||
                string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave) ||
                string.IsNullOrWhiteSpace(remitente) || !int.TryParse(puerto, out numeroPuerto))
                throw new InvalidOperationException("El envío SMTP no está configurado. Definí EVENTAURA_SMTP_HOST, EVENTAURA_SMTP_PORT, EVENTAURA_SMTP_USER, EVENTAURA_SMTP_PASSWORD y EVENTAURA_SMTP_FROM en el entorno del proceso.");

            using (var mensaje = new MailMessage(remitente, correo))
            using (var cliente = new SmtpClient(host, numeroPuerto))
            {
                mensaje.Subject = proposito == "Verificacion" ? "Verificá tu correo de EventAura" : "Recuperá tu contraseña de EventAura";
                mensaje.Body = "Tu código es " + codigo + ". Vence el " + venceEl.ToLocalTime().ToString("dd/MM/yyyy HH:mm") + ".";
                cliente.EnableSsl = true;
                cliente.Credentials = new NetworkCredential(usuario, clave);
                cliente.Send(mensaje);
            }
        }
    }
}
