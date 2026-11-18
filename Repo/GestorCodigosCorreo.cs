using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Datos;

namespace Negocio
{
    public sealed class GestorCodigosCorreo
    {
        private static readonly GestorCodigosCorreo Instancia = new GestorCodigosCorreo();
        public static GestorCodigosCorreo Instance { get { return Instancia; } }
        private GestorCodigosCorreo() { }

        public void EnviarVerificacion(int idUsuario, string correo)
        {
            Emitir(idUsuario, correo, "Verificacion");
        }

        public void SolicitarRecuperacion(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return;
            var usuario = RepositorioUsuarios.Instance.ObtenerPorMail(correo.Trim());
            if (usuario != null) Emitir(usuario.Id, usuario.Mail, "Recuperacion");
        }

        public void ConfirmarVerificacion(string correo, string codigo)
        {
            Confirmar(correo, codigo, "Verificacion", null);
        }

        // RN-51: el teléfono verificado reutiliza el mismo emisor, hash, vencimiento y contador.
        public const string PropositoTelefonoOrganizador = "TelefonoOrg";

        public void EnviarCodigoTelefono(int idUsuario, string telefono)
        {
            GestorSesion.Instance.RequerirUsuario(idUsuario);
            if (string.IsNullOrWhiteSpace(telefono)) throw new ReglaNegocioException("RN-51", "Ingresá un teléfono válido.");
            Emitir(idUsuario, telefono.Trim(), PropositoTelefonoOrganizador);
        }

        public void ConfirmarCodigoTelefono(int idUsuario, string telefono, string codigo)
        {
            GestorSesion.Instance.RequerirUsuario(idUsuario);
            var ultimo = RepositorioCodigosCorreo.Instance.Ultimo(idUsuario, PropositoTelefonoOrganizador);
            if (ultimo == null || !string.Equals(ultimo.Destinatario, telefono == null ? null : telefono.Trim(), StringComparison.Ordinal))
                throw CodigoVencido();
            ConfirmarCodigo(ultimo, idUsuario, PropositoTelefonoOrganizador, codigo, null);
        }

        public void RecuperarContrasena(string correo, string codigo, string nueva)
        {
            GestorUsuarios.Instance.ValidarContrasena(nueva);
            Confirmar(correo, codigo, "Recuperacion", Seguridad.Hash(nueva));
        }

        private static void Emitir(int idUsuario, string correo, string proposito)
        {
            string codigo;
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                uint numero = BitConverter.ToUInt32(bytes, 0) % 900000U + 100000U;
                codigo = numero.ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
            }
            var sal = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(sal);
            int minutos = GestorParametros.Instance.ObtenerEntero("CORREO_CODIGO_MIN");
            byte[] hash = CalcularHash(sal, proposito, codigo);
            int idCodigo = RepositorioCodigosCorreo.Instance.Emitir(idUsuario, proposito, sal, hash, minutos, correo);
            var pendiente = RepositorioCodigosCorreo.Instance.PorId(idCodigo);
            SeleccionEnviadorCodigos.Actual.Enviar(correo, proposito, codigo, pendiente.VenceEl);
        }

        private static void Confirmar(string correo, string codigo, string proposito, string nuevaContrasenaHash)
        {
            var usuario = string.IsNullOrWhiteSpace(correo) ? null : RepositorioUsuarios.Instance.ObtenerPorMail(correo.Trim());
            if (usuario == null) throw CodigoVencido();
            var ultimo = RepositorioCodigosCorreo.Instance.Ultimo(usuario.Id, proposito);
            ConfirmarCodigo(ultimo, usuario.Id, proposito, codigo, nuevaContrasenaHash);
        }

        private static void ConfirmarCodigo(CodigoCorreo ultimo, int usuarioId, string proposito, string codigo, string nuevaContrasenaHash)
        {
            if (ultimo == null || ultimo.Usado || ultimo.VenceEl <= DateTime.UtcNow)
                throw CodigoVencido();

            int maximo = GestorParametros.Instance.ObtenerEntero("CORREO_CODIGO_INTENTOS_MAX");
            if (ultimo.Intentos >= maximo) throw CodigoVencido();
            byte[] propuesto = CalcularHash(ultimo.Sal, proposito, codigo ?? string.Empty);
            if (!Iguales(ultimo.Hash, propuesto))
            {
                RepositorioCodigosCorreo.Instance.RegistrarIntento(ultimo.Id, maximo);
                throw new ReglaNegocioException("RN-44", "El código no es correcto. Revisá el correo y volvé a intentar.");
            }

            if (!RepositorioCodigosCorreo.Instance.Consumir(ultimo.Id, usuarioId, proposito, maximo, nuevaContrasenaHash))
                throw CodigoVencido();
        }

        private static byte[] CalcularHash(byte[] sal, string proposito, string codigo)
        {
            byte[] dato = Encoding.UTF8.GetBytes(proposito + "\0" + codigo);
            var combinado = new byte[sal.Length + dato.Length];
            Buffer.BlockCopy(sal, 0, combinado, 0, sal.Length);
            Buffer.BlockCopy(dato, 0, combinado, sal.Length, dato.Length);
            using (var sha = SHA256.Create()) return sha.ComputeHash(combinado);
        }

        private static bool Iguales(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diferencia = 0;
            for (int i = 0; i < a.Length; i++) diferencia |= a[i] ^ b[i];
            return diferencia == 0;
        }

        private static ReglaNegocioException CodigoVencido()
        {
            return new ReglaNegocioException("RN-44", "El código venció o ya se usó. Solicitá uno nuevo.");
        }
    }
}
