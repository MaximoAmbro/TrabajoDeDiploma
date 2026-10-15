using System;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    public sealed class GestorUsuarios
    {
        private static GestorUsuarios _instance;

        public static GestorUsuarios Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorUsuarios();
                }
                return _instance;
            }
        }

        private const string TipoCliente = "Cliente";
        private const string TipoPropietario = "Propietario";

        private GestorUsuarios() { }

        // Devuelve el usuario autenticado, o null si faltan datos o el mail no existe.
        // Lanza ReglaNegocioException (RN-04) si la contraseña es incorrecta, con los intentos
        // que quedan, o si la cuenta está bloqueada, con los minutos que faltan.
        public Usuario Autenticar(string mail, string pwd)
        {
            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrEmpty(pwd))
            {
                return null;
            }

            Usuario usuario = RepositorioUsuarios.Instance.ObtenerPorMail(mail.Trim());
            if (usuario == null)
            {
                return null;
            }

            DateTime ahora = DateTime.Now;
            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value > ahora)
            {
                throw CuentaBloqueada(usuario.BloqueadoHasta.Value, ahora);
            }

            if (!ContrasenaValida(usuario, pwd))
            {
                throw RegistrarIntentoFallido(usuario, ahora);
            }

            if (usuario.IntentosFallidos != 0 || usuario.BloqueadoHasta.HasValue)
            {
                RepositorioUsuarios.Instance.ActualizarIntentos(usuario.Id, 0, null);
                usuario.IntentosFallidos = 0;
                usuario.BloqueadoHasta = null;
            }

            usuario.Contraseña = null;   // el hash no sale de esta capa
            GestorSesion.Instance.Iniciar(usuario);
            return usuario;
        }

        public void Registrar(string mail, string pwd, string nombre, string apellido, string tipoUsuario)
        {
            if (string.IsNullOrWhiteSpace(mail))
            {
                throw new ReglaNegocioException("RN-01", "Ingrese un mail.");
            }
            if (tipoUsuario != TipoCliente && tipoUsuario != TipoPropietario)
            {
                throw new ReglaNegocioException("RN-05",
                    $"El tipo de usuario debe ser {TipoCliente} o {TipoPropietario}.");
            }
            ValidarContrasena(pwd);

            mail = mail.Trim();
            if (RepositorioUsuarios.Instance.ExisteMail(mail))
            {
                throw MailDuplicado();
            }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Mail = mail,
                Contraseña = Seguridad.Hash(pwd),
                TipoUsuario = tipoUsuario
            };

            try
            {
                // RN-03: nace en Bronce y sin suscripción (no se inserta nada en Suscripciones).
                RepositorioUsuarios.Instance.Insertar(usuario, NivelFidelidad.Bronce);
            }
            catch (RegistroDuplicadoException)
            {
                // Otro alta con el mismo mail entró entre ExisteMail e Insertar.
                throw MailDuplicado();
            }
        }

        // TODO Seccion N: exigir autenticación o un token por mail antes de cambiar la contraseña.
        public void CambiarContrasena(string mail, string nueva)
        {
            ValidarContrasena(nueva);

            if (string.IsNullOrWhiteSpace(mail) ||
                !RepositorioUsuarios.Instance.ActualizarContrasena(mail.Trim(), Seguridad.Hash(nueva)))
            {
                throw new ReglaNegocioException("RN-01", "No existe una cuenta con ese mail.");
            }
        }

        // RN-02: longitud mínima PWD_LONGITUD_MIN, al menos una letra y un dígito.
        public void ValidarContrasena(string pwd)
        {
            int longitudMinima = GestorParametros.Instance.ObtenerEntero("PWD_LONGITUD_MIN");

            if (pwd == null || pwd.Length < longitudMinima || !pwd.Any(char.IsLetter) || !pwd.Any(char.IsDigit))
            {
                throw new ReglaNegocioException("RN-02",
                    $"La contraseña debe tener al menos {longitudMinima} caracteres, " +
                    "con al menos una letra y un dígito.");
            }
        }

        // ------------------------------------- privados -------------------------------------

        private bool ContrasenaValida(Usuario usuario, string pwd)
        {
            if (Seguridad.Verificar(pwd, usuario.Contraseña))
            {
                return true;
            }

            // Migración: contraseña heredada en texto plano. Se acepta una vez y se reemplaza por su hash.
            // Solo si lo guardado no es un hash; si no, alguien que conozca el hash podría usarlo como contraseña.
            if (!Seguridad.EsHash(usuario.Contraseña) && usuario.Contraseña == pwd)
            {
                RepositorioUsuarios.Instance.ActualizarContrasena(usuario.Mail, Seguridad.Hash(pwd));
                return true;
            }

            return false;
        }

        // RN-04: tras LOGIN_INTENTOS_MAX fallos consecutivos, bloqueo de LOGIN_BLOQUEO_MIN minutos.
        // Devuelve la excepción a lanzar: intentos restantes, o bloqueo si se llegó al tope.
        private ReglaNegocioException RegistrarIntentoFallido(Usuario usuario, DateTime ahora)
        {
            int intentosMax = GestorParametros.Instance.ObtenerEntero("LOGIN_INTENTOS_MAX");
            int bloqueoMinutos = GestorParametros.Instance.ObtenerEntero("LOGIN_BLOQUEO_MIN");

            // Si había un bloqueo y ya venció, la cuenta empieza a contar de nuevo.
            int intentos = usuario.BloqueadoHasta.HasValue ? 1 : usuario.IntentosFallidos + 1;
            DateTime? bloqueadoHasta = intentos >= intentosMax ? ahora.AddMinutes(bloqueoMinutos) : (DateTime?)null;

            RepositorioUsuarios.Instance.ActualizarIntentos(usuario.Id, intentos, bloqueadoHasta);

            if (bloqueadoHasta.HasValue)
            {
                return CuentaBloqueada(bloqueadoHasta.Value, ahora);
            }

            int restantes = intentosMax - intentos;
            return new ReglaNegocioException("RN-04",
                $"Mail o contraseña incorrectos. Le quedan {restantes} " +
                (restantes == 1 ? "intento" : "intentos") + " antes de que la cuenta se bloquee.");
        }

        private static ReglaNegocioException CuentaBloqueada(DateTime hasta, DateTime ahora)
        {
            int minutos = Math.Max(1, (int)Math.Ceiling((hasta - ahora).TotalMinutes));
            return new ReglaNegocioException("RN-04",
                $"La cuenta está bloqueada por demasiados intentos fallidos. Vuelva a intentar en {minutos} " +
                (minutos == 1 ? "minuto." : "minutos."));
        }

        private static ReglaNegocioException MailDuplicado()
        {
            return new ReglaNegocioException("RN-01", "Ya existe una cuenta registrada con ese mail.");
        }
    }
}
