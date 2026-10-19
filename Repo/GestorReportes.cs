using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-11 + CU-08 + anexo A02. RN-34: solo el Administrador. Se verifica acá contra la sesión,
    // no solo escondiendo el botón: cualquier llamador que llegue a estos métodos pasa por el
    // control. Los reportes solo leen.
    public sealed class GestorReportes
    {
        // RN-05: el perfil Administrador se guarda como TipoUsuario «Propietario».
        private const string PerfilAdministrador = "Propietario";

        public static readonly IReadOnlyList<string> Codigos = new[]
        {
            ReporteVentasPorEvento.CodigoReporte, ReporteRankingPaquetes.CodigoReporte,
            ReporteUsuariosPorNivel.CodigoReporte, ReporteAdopcionBeneficios.CodigoReporte,
            ReporteDemandaInsatisfecha.CodigoReporte
        };

        private static GestorReportes _instance;

        public static GestorReportes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorReportes();
                }
                return _instance;
            }
        }

        private GestorReportes() { }

        public Reporte Crear(string codigo, DateTime desde, DateTime hasta)
        {
            Usuario usuario = VerificarAdministrador();
            if (desde.Date > hasta.Date)
            {
                throw new ArgumentException("La fecha «desde» no puede ser posterior a la fecha «hasta».");
            }
            Reporte reporte = Nuevo(codigo, desde, hasta);
            reporte.Solicitante = $"{usuario.Nombre} {usuario.Apellido} ({usuario.Mail})".Replace("  ", " ");
            return reporte;
        }

        public DataTable Ejecutar(Reporte r)
        {
            VerificarAdministrador();
            if (r == null)
            {
                throw new ArgumentNullException(nameof(r));
            }
            return r.Obtener(RepositorioReportes.Instance);
        }

        // Ejecuta el reporte y lo entrega en el formato del exportador. Devuelve la ruta del archivo.
        public string Exportar(Reporte r, IExportador exp, string ruta)
        {
            if (exp == null)
            {
                throw new ArgumentNullException(nameof(exp));
            }
            DataTable datos = Ejecutar(r);
            return exp.Exportar(r, datos, ruta);
        }

        public string TituloDe(string codigo)
        {
            return Nuevo(codigo, DateTime.Today, DateTime.Today).Titulo;
        }

        // Eventos con fecha en el período, para los filtros por evento (RPT-02 y RPT-05).
        public List<Evento> EventosEntre(DateTime desde, DateTime hasta)
        {
            VerificarAdministrador();
            return RepositorioReportes.Instance.EventosEntre(desde.Date, hasta.Date.AddDays(1).AddSeconds(-1))
                .Rows.Cast<DataRow>()
                .Select(f => new Evento
                {
                    Id = Convert.ToInt32(f["ID"]),
                    Nombre = Convert.ToString(f["Nombre"]),
                    Fecha = Convert.ToDateTime(f["Fecha"])
                })
                .ToList();
        }

        // ------------------------------------- privados -------------------------------------

        private static Reporte Nuevo(string codigo, DateTime desde, DateTime hasta)
        {
            switch (codigo)
            {
                case ReporteVentasPorEvento.CodigoReporte: return new ReporteVentasPorEvento(desde, hasta);
                case ReporteRankingPaquetes.CodigoReporte: return new ReporteRankingPaquetes(desde, hasta);
                case ReporteUsuariosPorNivel.CodigoReporte: return new ReporteUsuariosPorNivel(desde, hasta);
                case ReporteAdopcionBeneficios.CodigoReporte: return new ReporteAdopcionBeneficios(desde, hasta);
                case ReporteDemandaInsatisfecha.CodigoReporte: return new ReporteDemandaInsatisfecha(desde, hasta);
                default:
                    throw new ArgumentException($"No existe el reporte «{codigo}».");
            }
        }

        // RN-34
        private static Usuario VerificarAdministrador()
        {
            Usuario usuario = GestorSesion.Instance.UsuarioActual;
            if (usuario == null || usuario.TipoUsuario != PerfilAdministrador)
            {
                throw new ReglaNegocioException("RN-34", "Los reportes son accesibles únicamente al perfil Administrador.");
            }
            return usuario;
        }
    }
}
