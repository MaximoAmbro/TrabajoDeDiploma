using System;
using System.Collections.Generic;
using System.Globalization;
using Datos;
using Entidades;

namespace Negocio
{
    // Valores numéricos de las reglas (docs/02-reglas-negocio.md, parámetros configurables).
    // Se leen una sola vez de la tabla Parametros y quedan en memoria.
    public sealed class GestorParametros
    {
        private static GestorParametros _instance;

        public static GestorParametros Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorParametros();
                }
                return _instance;
            }
        }

        private Dictionary<string, string> _cache;

        private GestorParametros() { }

        public string Obtener(string clave)
        {
            if (_cache == null)
            {
                Recargar();
            }

            string valor;
            if (!_cache.TryGetValue(clave, out valor))
            {
                throw new InvalidOperationException(
                    $"Falta el parámetro '{clave}' en la tabla Parametros.");
            }
            return valor;
        }

        public int ObtenerEntero(string clave)
        {
            return int.Parse(Obtener(clave), CultureInfo.InvariantCulture);
        }

        // Los decimales se guardan con punto ('0.050'), sin importar la cultura del equipo.
        public decimal ObtenerDecimal(string clave)
        {
            return decimal.Parse(Obtener(clave), NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        // Descarta la caché, por si se modifica la tabla con la aplicación abierta.
        public void Recargar()
        {
            var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (Parametro parametro in RepositorioParametros.Instance.ObtenerTodos())
            {
                cache[parametro.Clave] = parametro.Valor;
            }
            _cache = cache;
        }
    }
}
