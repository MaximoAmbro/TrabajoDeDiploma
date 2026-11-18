using System;
using System.Linq;
using System.Net;
using System.Text;
using Datos;
using Entidades;

namespace Negocio
{
    public interface IVerificadorIdentidad
    {
        bool Verificar(string responsable, string cuit);
    }

    // La simulación valida los formatos localmente; la aprobación final siempre es manual.
    public sealed class VerificadorIdentidadSimulado : IVerificadorIdentidad
    {
        public bool Verificar(string responsable, string cuit) => !string.IsNullOrWhiteSpace(responsable) && GestorSolicitudesOrganizador.CuitValido(cuit);
    }

    // Integración real opcional. No se activa por defecto ni tiene secretos en el repositorio.
    public sealed class VerificadorIdentidadExterno : IVerificadorIdentidad
    {
        public bool Verificar(string responsable, string cuit)
        {
            string endpoint = Environment.GetEnvironmentVariable("EVENTAURA_IDENTIDAD_ENDPOINT");
            string token = Environment.GetEnvironmentVariable("EVENTAURA_IDENTIDAD_TOKEN");
            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("La verificación externa de identidad no está configurada; definí EVENTAURA_IDENTIDAD_ENDPOINT y EVENTAURA_IDENTIDAD_TOKEN fuera del repositorio.");
            var request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = "POST"; request.ContentType = "application/json"; request.Headers["Authorization"] = "Bearer " + token;
            byte[] cuerpo = Encoding.UTF8.GetBytes("{\"responsable\":\"" + Escapar(responsable) + "\",\"cuit\":\"" + Escapar(cuit) + "\"}");
            request.ContentLength = cuerpo.Length;
            using (var stream = request.GetRequestStream()) stream.Write(cuerpo, 0, cuerpo.Length);
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                return response.StatusCode == HttpStatusCode.OK && reader.ReadToEnd().IndexOf("\"verificado\":true", StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private static string Escapar(string valor) => (valor ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
    }

    public static class SeleccionVerificadorIdentidad
    {
        public static IVerificadorIdentidad Actual { get; set; } = new VerificadorIdentidadSimulado();
    }

    public sealed class GestorSolicitudesOrganizador
    {
        private static readonly GestorSolicitudesOrganizador Instancia = new GestorSolicitudesOrganizador();
        public static GestorSolicitudesOrganizador Instance { get { return Instancia; } }
        private GestorSolicitudesOrganizador() { }

        public void EnviarCodigoTelefono(string telefono)
        {
            var actual = GestorSesion.Instance.UsuarioActual;
            if (actual == null || actual.TipoUsuario != "Cliente")
                throw new ReglaNegocioException("RN-50", "Solo una cuenta Cliente puede solicitar ser Organizador.");
            string normalizado = NormalizarTelefonoArgentino(telefono);
            GestorCodigosCorreo.Instance.EnviarCodigoTelefono(actual.Id, normalizado);
        }

        public void ConfirmarCodigoTelefono(string telefono, string codigo)
        {
            var actual = GestorSesion.Instance.UsuarioActual;
            if (actual == null || actual.TipoUsuario != "Cliente")
                throw new ReglaNegocioException("RN-50", "Solo una cuenta Cliente puede solicitar ser Organizador.");
            GestorCodigosCorreo.Instance.ConfirmarCodigoTelefono(actual.Id, NormalizarTelefonoArgentino(telefono), codigo);
        }

        public int Solicitar(SolicitudOrganizador solicitud)
        {
            var usuario = GestorSesion.Instance.UsuarioActual;
            if (usuario == null || usuario.TipoUsuario != "Cliente")
                throw new ReglaNegocioException("RN-50", "Solo una cuenta Cliente puede solicitar ser Organizador.");
            if (solicitud == null) throw new ArgumentNullException(nameof(solicitud));
            solicitud.Cuit = Digitos(solicitud.Cuit);
            solicitud.CbuAlias = (solicitud.CbuAlias ?? string.Empty).Trim().ToLowerInvariant();
            solicitud.Telefono = NormalizarTelefonoArgentino(solicitud.Telefono);
            if (string.IsNullOrWhiteSpace(solicitud.Responsable) || solicitud.Responsable.Trim().Length > 160 ||
                string.IsNullOrWhiteSpace(solicitud.NombreLocal) || solicitud.NombreLocal.Trim().Length > 100 ||
                string.IsNullOrWhiteSpace(solicitud.Direccion) || solicitud.Direccion.Trim().Length > 160 ||
                string.IsNullOrWhiteSpace(solicitud.NombreArchivo) || solicitud.NombreArchivo.Length > 180)
                throw new ReglaNegocioException("RN-50", "Completá los datos del responsable y del local.");
            if (!CuitValido(solicitud.Cuit)) throw new ReglaNegocioException("RN-52", "El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.");
            if (!CbuAliasValido(solicitud.CbuAlias)) throw new ReglaNegocioException("RN-52", "Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.");
            if (solicitud.Capacidad <= 0) throw new ReglaNegocioException("RN-50", "La capacidad del local debe ser mayor que cero.");
            if (solicitud.Documentacion == null || solicitud.Documentacion.Length == 0 || solicitud.Documentacion.Length > 512 * 1024 || !CabeceraImagenValida(solicitud.Documentacion))
                throw new ReglaNegocioException("RN-52", "Adjuntá una imagen PNG o JPEG válida, comprimida y de hasta 512 KB.");
            var codigo = RepositorioCodigosCorreo.Instance.Ultimo(usuario.Id, GestorCodigosCorreo.PropositoTelefonoOrganizador);
            if (codigo == null || !codigo.Usado || codigo.VenceEl <= DateTime.UtcNow || !string.Equals(codigo.Destinatario, solicitud.Telefono, StringComparison.Ordinal))
                throw new ReglaNegocioException("RN-51", "Verificá este teléfono con el código enviado antes de presentar la solicitud.");
            if (!SeleccionVerificadorIdentidad.Actual.Verificar(solicitud.Responsable, solicitud.Cuit))
                throw new ReglaNegocioException("RN-52", "No pudimos validar el formato de la identidad. Revisá los datos antes de enviar.");
            solicitud.UsuarioId = usuario.Id;
            try { return RepositorioSolicitudesOrganizador.Instance.Crear(solicitud); }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            { throw new ReglaNegocioException("RN-50", "Ya tenés una solicitud de Organizador pendiente."); }
        }

        public SolicitudOrganizador Obtener(int id)
        {
            GestorSesion.Instance.RequerirAdministrador();
            return RepositorioSolicitudesOrganizador.Instance.Obtener(id);
        }

        public System.Collections.Generic.List<SolicitudOrganizador> Pendientes()
        {
            GestorSesion.Instance.RequerirAdministrador();
            return RepositorioSolicitudesOrganizador.Instance.Pendientes();
        }

        public void Aprobar(int id, string motivo)
        {
            GestorSesion.Instance.RequerirAdministrador();
            if (string.IsNullOrWhiteSpace(motivo)) throw new ReglaNegocioException("RN-53", "Escribí el motivo de la aprobación.");
            if (motivo.Trim().Length > 200) throw new ReglaNegocioException("RN-53", "El motivo no puede superar los 200 caracteres.");
            RepositorioSolicitudesOrganizador.Instance.Aprobar(id, GestorSesion.Instance.UsuarioActual.Id, motivo.Trim());
        }

        public void Rechazar(int id, string motivo)
        {
            GestorSesion.Instance.RequerirAdministrador();
            if (string.IsNullOrWhiteSpace(motivo)) throw new ReglaNegocioException("RN-53", "Escribí el motivo del rechazo.");
            if (motivo.Trim().Length > 200) throw new ReglaNegocioException("RN-53", "El motivo no puede superar los 200 caracteres.");
            RepositorioSolicitudesOrganizador.Instance.Rechazar(id, GestorSesion.Instance.UsuarioActual.Id, motivo.Trim());
        }

        public static bool CuitValido(string valor)
        {
            string d = Digitos(valor);
            if (d.Length != 11) return false;
            int[] pesos = { 5,4,3,2,7,6,5,4,3,2 };
            int suma = 0; for (int i=0;i<10;i++) suma += (d[i]-'0')*pesos[i];
            int verificador = 11 - suma % 11;
            if (verificador == 11) verificador = 0;
            if (verificador == 10) { if (d.Substring(0,2) != "23") return false; suma = 0; int[] pesos23={4,3,2,7,6,5,4,3,2,0}; for(int i=0;i<10;i++) suma+=(d[i]-'0')*pesos23[i]; verificador=11-suma%11; if(verificador==11)verificador=0; }
            return verificador == d[10]-'0';
        }

        public static bool CbuValido(string valor)
        {
            string d=Digitos(valor); if(d.Length!=22)return false;
            if (DigitoCbu(d.Substring(0,7), new[]{7,1,3,9,7,1,3}) != d[7]-'0') return false;
            return DigitoCbu(d.Substring(8,13),new[]{3,9,7,1,3,9,7,1,3,9,7,1,3}) == d[21]-'0';
        }
        public static bool CbuAliasValido(string valor)
        {
            string d=Digitos(valor);
            if (d.Length == 22 && d.Length == (valor ?? string.Empty).Length) return CbuValido(d);
            string alias=(valor ?? string.Empty).Trim().ToLowerInvariant();
            return alias.Length >= 6 && alias.Length <= 20 && alias.All(c => char.IsLower(c) || char.IsDigit(c) || c=='.' || c=='-');
        }
        private static int DigitoCbu(string tramo,int[] pesos){int suma=0;for(int i=0;i<pesos.Length;i++)suma+=(tramo[i]-'0')*pesos[i];return (10-suma%10)%10;}
        private static string Digitos(string valor) => new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        public static string NormalizarTelefonoArgentino(string valor)
        {
            string d=Digitos(valor);
            if(d.StartsWith("549")) d=d.Substring(3);
            else if(d.StartsWith("54")) d=d.Substring(2);
            if(d.StartsWith("0")) d=d.Substring(1);
            if(d.Length>=11 && d[0]=='9') d=d.Substring(1);
            if(d.Length!=10 || d[0]<'2' || d[0]>'4' || d.All(c=>c=='0'))
                throw new ReglaNegocioException("RN-51", "Ingresá un teléfono argentino válido, con característica y número.");
            return "+549"+d;
        }
        private static bool CabeceraImagenValida(byte[] b) =>
            (b.Length>=8 && b[0]==137 && b[1]==80 && b[2]==78 && b[3]==71 && b[4]==13 && b[5]==10 && b[6]==26 && b[7]==10) ||
            (b.Length>=3 && b[0]==255 && b[1]==216 && b[2]==255);
    }
}
