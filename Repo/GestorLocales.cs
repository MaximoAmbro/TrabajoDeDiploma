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
            return RepositorioLocales.Instance.ObtenerPorId(idLocal);
        }

        public List<Local> PorPropietario(int idPropietario)
        {
            return RepositorioLocales.Instance.ObtenerPorPropietario(idPropietario);
        }

        // Devuelve el Id generado.
        public int Registrar(Local l)
        {
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

            l.Id = RepositorioLocales.Instance.Insertar(l);
            return l.Id;
        }
    }
}
