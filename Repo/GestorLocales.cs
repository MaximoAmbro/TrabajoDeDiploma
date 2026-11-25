using System;
using System.Collections.Generic;
using Datos;
using Entidades;

namespace Negocio
{
    // Locales del propietario. No responde a ningún RF: existe para que Visual no tenga SQL propio.
    public sealed class GestorLocales
    {
        private static GestorLocales _instance;

        public static GestorLocales Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorLocales();
                }
                return _instance;
            }
        }

        private GestorLocales() { }

        // Devuelve null si no existe.
        public Local PorId(int idLocal)
        {
            Local local = RepositorioLocales.Instance.ObtenerPorId(idLocal);
            if (local == null) return null;
            var sesion = GestorSesion.Instance.UsuarioActual;
            if (sesion != null && sesion.TipoUsuario == "Organizador")
                GestorSesion.Instance.RequerirOrganizadorDeLocal(local.IdPropietario);
            else if ((sesion == null || sesion.TipoUsuario == "Cliente") &&
                !RepositorioEventos.Instance.ObtenerPorLocal(idLocal).Exists(e => e.Estado == GestorEventos.EstadoPublicado))
                return null;
            return local;
        }

        public List<Local> PorPropietario(int idPropietario)
        {
            GestorSesion.Instance.RequerirOrganizadorDeLocal(idPropietario);
            return RepositorioLocales.Instance.ObtenerPorPropietario(GestorSesion.Instance.UsuarioActual.Id);
        }

        // Devuelve el Id generado.
        public int Registrar(Local l)
        {
            GestorSesion.Instance.RequerirOrganizador();
            if (l == null)
            {
                throw new ArgumentNullException(nameof(l));
            }
            if (string.IsNullOrWhiteSpace(l.Nombre))
            {
                throw new ArgumentException("El local debe tener nombre.");
            }
            if (l.Capacidad < 0)
            {
                throw new ArgumentException("La capacidad del local no puede ser negativa.");
            }

            l.IdPropietario = GestorSesion.Instance.UsuarioActual.Id;

            l.Id = RepositorioLocales.Instance.Insertar(l);
            return l.Id;
        }
    }
}
