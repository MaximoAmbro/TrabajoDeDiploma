using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-11 + CU-08 + anexo A02. El Administrador ve el universo global; el Organizador
    // recibe agregados limitados a eventos de sus locales. La consulta fuerza el alcance vigente.
    public sealed class GestorReportes
    {
        private const string PerfilAdministrador = "Administrador";
        private const string PerfilOrganizador = "Organizador";

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
            var usuario = VerificarAccesoReportes();
            if (desde.Date > hasta.Date)
            {
                throw new ArgumentException("La fecha «desde» no puede ser posterior a la fecha «hasta».");
            }
            Reporte reporte = Nuevo(codigo, desde, hasta);
            reporte.Solicitante = $"{usuario.Nombre} {usuario.Apellido} ({usuario.Mail})".Replace("  ", " ");
            reporte.IdOrganizador = usuario.TipoUsuario == PerfilOrganizador ? (int?)usuario.Id : null;
            return reporte;
        }

        public DataTable Ejecutar(Reporte r)
        {
            var usuario = VerificarAccesoReportes();
            if (r == null)
            {
                throw new ArgumentNullException(nameof(r));
            }
            r.IdOrganizador = usuario.TipoUsuario == PerfilOrganizador ? (int?)usuario.Id : null;
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
            var usuario = VerificarAccesoReportes();
            int? idOrganizador = usuario.TipoUsuario == PerfilOrganizador ? (int?)usuario.Id : null;
            return RepositorioReportes.Instance.EventosEntre(desde.Date, hasta.Date.AddDays(1).AddSeconds(-1), idOrganizador)
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

        private static SesionAutenticada VerificarAccesoReportes()
        {
            var usuario = GestorSesion.Instance.UsuarioActual;
            if (usuario == null || (usuario.TipoUsuario != PerfilAdministrador && usuario.TipoUsuario != PerfilOrganizador))
            {
                throw new ReglaNegocioException("RN-34", "Los reportes están disponibles para Administradores y Organizadores.");
            }
            return usuario;
        }
    }
}
