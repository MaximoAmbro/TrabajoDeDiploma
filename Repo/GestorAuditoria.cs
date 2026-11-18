using System;
using System.Collections.Generic;
using Datos;

namespace Negocio
{
    public sealed class GestorAuditoria
    {
        private static readonly GestorAuditoria Instancia = new GestorAuditoria();
        public static GestorAuditoria Instance { get { return Instancia; } }
        private GestorAuditoria() { }

        internal void Registrar(string accion, string entidad = null, int? entidadId = null, string resultado = "Correcto", int? usuarioId = null, string usuarioMail = null)
        {
            var sesion = GestorSesion.Instance.UsuarioActual;
            Datos.RepositorioAuditoria.Instance.Registrar(usuarioId ?? sesion?.Id, Limitar(usuarioMail ?? sesion?.Mail, 150), Limitar(accion, 80), Limitar(entidad, 80), entidadId, Limitar(resultado, 200));
        }

        private static string Limitar(string valor, int maximo)
        {
            return valor == null || valor.Length <= maximo ? valor : valor.Substring(0, maximo);
        }

        public List<RegistroAuditoria> Consultar(DateTime desde, DateTime hasta, string accion)
        {
            GestorSesion.Instance.RequerirAdministrador();
            if (hasta <= desde) throw new ArgumentException("El fin del período debe ser posterior al inicio.");
            return RepositorioAuditoria.Instance.Consultar(desde, hasta, accion);
        }
    }
}
