using Datos;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Entidades.Evento;
namespace Negocio
{
    public partial class GestorEventos //Singleton
    {
        // Estados de un evento (docs/03-esquema-datos.md, CK_Eventos_Estado).
        public const string EstadoBorrador = "Borrador";
        public const string EstadoEnRevision = "EnRevision";
        public const string EstadoPublicado = "Publicado";
        public const string EstadoSuspendido = "Suspendido";

        // Valores admitidos en Eventos.Categoria (docs/03-esquema-datos.md). RF-03 filtra por ellos.
        public static readonly IReadOnlyList<string> Categorias =
            new[] { "Concierto", "Deportivo", "Teatro", "Cultural" };

        private static GestorEventos _instance;

        public static GestorEventos Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorEventos();
                }
                return _instance;
            }
        }

        private GestorEventos() { }
    }
    public partial class GestorEventos //Lista
    {
        public string SectorA { get; set; }
        public string SectorB { get; set; }
        public string SectorC { get; set; }
        public string PrecioA { get; set; }
        public string PrecioB { get; set; }
        public string PrecioC { get; set; }
        public int TotalEntradas { get; set; }
    }
    public partial class GestorEventos //Metodos
    {
        // Guarda el evento sin sectores, en Borrador: RN-06 todavía no aplica. Pasa a Publicado
        // con su primer sector con capacidad (AgregarSector). Devuelve el Id generado.
        public int Crear(Evento e)
        {
            GestorSesion.Instance.RequerirOrganizador();
            ValidarEvento(e);
            RequerirLocalPropio(e.IdLocal);
            e.Sectores = new List<Sector>();
            e.Estado = EstadoBorrador;
            int id = RepositorioEventos.Instance.Insertar(e);
            GestorAuditoria.Instance.Registrar("Gestión de eventos: crear", "Evento", id);
            return id;
        }

        // RN-06: el evento se publica junto con sus sectores, y al menos uno debe tener capacidad.
        // Devuelve el Id generado.
        public int Publicar(Evento e)
        {
            GestorSesion.Instance.RequerirOrganizador();
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.Id > 0) { Publicar(e.Id); return e.Id; }
            throw new ReglaNegocioException("RN-06", "Guardá el evento como borrador, agregale sectores y después elegí Publicar.");
        }

        // RN-06/RN-54: publicar es una acción explícita. La primera publicación de un
        // organizador nuevo queda en revisión; luego de una aprobación, las siguientes son directas.
        public void Publicar(int idEvento)
        {
            GestorSesion.Instance.RequerirOrganizador();
            RequerirEventoPropio(idEvento);
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null || evento.Estado != EstadoBorrador)
                throw new ReglaNegocioException("RN-06", "Solo podés publicar un evento que esté en borrador.");
            List<Sector> sectores = RepositorioSectores.Instance.ObtenerPorEvento(idEvento);
            if (!sectores.Any(s => s != null && s.Capacidad > 0))
                throw new ReglaNegocioException("RN-06", "No se puede publicar el evento: necesita al menos un sector con capacidad mayor que cero.");
            VerificarCapacidadLocal(evento.IdLocal, 0, sectores.Sum(s => s.Capacidad));
            string nuevoEstado = RepositorioEventos.Instance.OrganizadorYaTuvoEventoAprobado(GestorSesion.Instance.UsuarioActual.Id)
                ? EstadoPublicado : EstadoEnRevision;
            if (!RepositorioEventos.Instance.ActualizarEstadoSiEs(idEvento, EstadoBorrador, nuevoEstado))
                throw new ReglaNegocioException("RN-06", "El evento cambió mientras intentabas publicarlo. Actualizá la pantalla y volvé a intentarlo.");
            GestorAuditoria.Instance.Registrar("Gestión de eventos: " + (nuevoEstado == EstadoEnRevision ? "enviar a revisión" : "publicar"), "Evento", idEvento);
        }

        public List<Evento> PendientesDeRevision()
        {
            GestorSesion.Instance.RequerirAdministrador();
            return RepositorioEventos.Instance.ObtenerPorEstado(EstadoEnRevision);
        }

        public void AprobarPrimerEvento(int idEvento)
        {
            GestorSesion.Instance.RequerirAdministrador();
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null || evento.Estado != EstadoEnRevision)
                throw new ReglaNegocioException("RN-54", "El evento ya no está pendiente de revisión.");
            if (!RepositorioEventos.Instance.AprobarPrimerEvento(idEvento))
                throw new ReglaNegocioException("RN-54", "El evento cambió mientras se revisaba. Actualizá la lista.");
            GestorAuditoria.Instance.Registrar("Gestión de eventos: aprobar primer evento", "Evento", idEvento);
        }

        public void RechazarPrimerEvento(int idEvento, string motivo)
        {
            GestorSesion.Instance.RequerirAdministrador();
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ReglaNegocioException("RN-54", "Escribí el motivo del rechazo.");
            motivo = motivo.Trim();
            if (motivo.Length > 500)
                throw new ReglaNegocioException("RN-54", "El motivo no puede superar los 500 caracteres.");
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null || evento.Estado != EstadoEnRevision)
                throw new ReglaNegocioException("RN-54", "El evento ya no está pendiente de revisión.");
            if (!RepositorioEventos.Instance.RechazarPrimerEvento(idEvento, motivo))
                throw new ReglaNegocioException("RN-54", "El evento cambió mientras se revisaba. Actualizá la lista.");
            GestorAuditoria.Instance.Registrar("Gestión de eventos: rechazar primer evento", "Evento", idEvento, motivo);
        }

        // RN-11: un evento con entradas vendidas no se borra. Lanza la excepción sin tocar nada;
        // el formulario ofrece Suspender en su lugar.
        public void Eliminar(int idEvento)
        {
            GestorSesion.Instance.RequerirOrganizador();
            RequerirEventoPropio(idEvento);
            if (RepositorioEventos.Instance.ContarEntradas(idEvento) > 0)
            {
                throw new ReglaNegocioException("RN-11",
                    "El evento tiene entradas vendidas: no se puede eliminar, solo suspender.");
            }
            if (!RepositorioEventos.Instance.Eliminar(idEvento))
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            GestorAuditoria.Instance.Registrar("Gestión de eventos: eliminar", "Evento", idEvento);
        }

        // RN-11: alternativa a Eliminar. Vale tenga o no entradas vendidas.
        public void Suspender(int idEvento)
        {
            GestorSesion.Instance.RequerirOrganizador();
            RequerirEventoPropio(idEvento);
            if (!RepositorioEventos.Instance.ActualizarEstado(idEvento, EstadoSuspendido))
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            GestorAuditoria.Instance.Registrar("Gestión de eventos: suspender", "Evento", idEvento);
        }

        // RN-10: todos los sectores vendieron su capacidad. Ahí se habilita la lista de espera.
        public bool EstaAgotado(int idEvento)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null) return false;
            if (GestorSesion.Instance.HaySesion &&
                GestorSesion.Instance.UsuarioActual.TipoUsuario == "Organizador")
                RequerirEventoPropio(idEvento);
            else if ((!GestorSesion.Instance.HaySesion ||
                GestorSesion.Instance.UsuarioActual.TipoUsuario == "Cliente") &&
                evento.Estado != EstadoPublicado) return false;
            return RepositorioEventos.Instance.EstaAgotado(idEvento);
        }

        // RN-39: fija o saca (null) la preventa de un evento ya creado.
        public void CambiarPreventa(int idEvento, DateTime? finPreventa)
        {
            GestorSesion.Instance.RequerirOrganizador();
            RequerirEventoPropio(idEvento);
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null)
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            evento.FinPreventa = finPreventa;
            ValidarPreventa(evento);
            RepositorioEventos.Instance.ActualizarPreventa(idEvento, finPreventa);
            GestorAuditoria.Instance.Registrar("Gestión de eventos: preventa", "Evento", idEvento);
        }

        public void CambiarImagenPortada(int idEvento, byte[] imagen)
        {
            GestorSesion.Instance.RequerirOrganizador();
            RequerirEventoPropio(idEvento);
            if (imagen != null && imagen.Length > 512 * 1024)
                throw new ArgumentException("La portada preparada supera el límite de 512 KB.");
            if (!RepositorioEventos.Instance.ActualizarImagenPortada(idEvento, imagen))
                throw new InvalidOperationException("No se encontró el evento para guardar su portada.");
            GestorAuditoria.Instance.Registrar("Gestión de eventos: portada", "Evento", idEvento);
        }

        // RN-07
        public int CapacidadTotal(int idEvento)
        {
            return Sectores(idEvento).Sum(s => s.Capacidad);
        }

        // RN-09: solo eventos Publicado con fecha posterior a la actual. Borrador y Suspendido
        // no son visibles para el cliente.
        public List<Evento> Vigentes()
        {
            return RepositorioEventos.Instance.ObtenerPorEstadoPosterioresA(EstadoPublicado, DateTime.Now);
        }

        // RF-03 + RN-09: solo eventos Publicado con fecha futura. Los filtros nulos o vacíos
        // no filtran. El texto se busca en el nombre del evento y en el nombre y la dirección del local.
        public List<Evento> Buscar(string texto, string categoria, DateTime? desde)
        {
            texto = string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
            categoria = string.IsNullOrWhiteSpace(categoria) ? null : categoria;
            return RepositorioEventos.Instance.Buscar(EstadoPublicado, DateTime.Now, texto, categoria, desde);
        }

        // Devuelve null si no existe.
        public Evento ObtenerPorId(int idEvento)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null) return null;
            if (GestorSesion.Instance.HaySesion && GestorSesion.Instance.UsuarioActual.TipoUsuario == "Organizador")
                RequerirEventoPropio(idEvento);
            else if ((!GestorSesion.Instance.HaySesion ||
                GestorSesion.Instance.UsuarioActual.TipoUsuario == "Cliente") &&
                evento.Estado != EstadoPublicado) return null;
            return evento;
        }

        public List<Evento> PorLocal(int idLocal)
        {
            Local local = RepositorioLocales.Instance.ObtenerPorId(idLocal);
            if (local == null) return new List<Evento>();
            GestorSesion.Instance.RequerirOrganizadorDeLocal(local.IdPropietario);
            return RepositorioEventos.Instance.ObtenerPorLocal(idLocal);
        }

        public List<Sector> Sectores(int idEvento)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null) return new List<Sector>();
            if (GestorSesion.Instance.HaySesion && GestorSesion.Instance.UsuarioActual.TipoUsuario == "Organizador")
                RequerirEventoPropio(idEvento);
            else if ((!GestorSesion.Instance.HaySesion ||
                GestorSesion.Instance.UsuarioActual.TipoUsuario == "Cliente") &&
                evento.Estado != EstadoPublicado) return new List<Sector>();
            return RepositorioSectores.Instance.ObtenerPorEvento(idEvento);
        }

        public void AgregarSector(int idEvento, Sector s)
        {
            GestorSesion.Instance.RequerirOrganizador();
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }
            ValidarSector(s);
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null)
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            RequerirEventoPropio(idEvento);
            List<Sector> actuales = Sectores(idEvento);
            VerificarCapacidadLocal(evento.IdLocal, actuales.Sum(x => x.Capacidad), s.Capacidad);

            s.IdEvento = idEvento;
            // Cargar un sector nunca publica: la transición es explícita por RN-06.
            s.Id = RepositorioSectores.Instance.Insertar(s);
            GestorAuditoria.Instance.Registrar("Gestión de eventos: sector", "Evento", idEvento);
        }

        public void EliminarSector(int idSector)
        {
            GestorSesion.Instance.RequerirOrganizador();
            Sector sector = RepositorioSectores.Instance.ObtenerPorId(idSector);
            if (sector == null)
            {
                throw new InvalidOperationException("No se encontró ningún sector con ese ID.");
            }
            RequerirEventoPropio(sector.IdEvento);
            if (RepositorioSectores.Instance.ContarEntradas(idSector) > 0)
            {
                throw new InvalidOperationException(
                    "El sector tiene entradas vendidas: no se puede eliminar.");
            }

            // RN-06 es un invariante: un evento publicado no puede quedarse sin sector con capacidad.
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(sector.IdEvento);
            bool quedaOtroConCapacidad = Sectores(sector.IdEvento)
                .Any(s => s.Id != idSector && s.Capacidad > 0);
            if (evento != null && evento.Estado == EstadoPublicado
                && sector.Capacidad > 0 && !quedaOtroConCapacidad)
            {
                throw new ReglaNegocioException("RN-06",
                    "No se puede eliminar el sector: es el último con capacidad mayor que cero de un evento publicado.");
            }

            if (!RepositorioSectores.Instance.Eliminar(idSector))
            {
                throw new InvalidOperationException("No se encontró ningún sector con ese ID.");
            }
        }

        // RN-38: la suma de capacidades de los sectores no supera la capacidad del local.
        // Si en el futuro se modifica la capacidad de un sector, pasar como ocupada la suma sin ese sector.
        private static void VerificarCapacidadLocal(int idLocal, int capacidadOcupada, int capacidadNueva)
        {
            Local local = RepositorioLocales.Instance.ObtenerPorId(idLocal);
            if (local == null)
            {
                throw new InvalidOperationException("No se encontró el local del evento.");
            }
            if (capacidadOcupada + capacidadNueva > local.Capacidad)
            {
                int disponible = Math.Max(0, local.Capacidad - capacidadOcupada);
                throw new ReglaNegocioException("RN-38",
                    $"La capacidad de los sectores superaría la del local ({local.Capacidad}). " +
                    $"Capacidad disponible para este evento: {disponible}.");
            }
        }

        private static void RequerirLocalPropio(int idLocal)
        {
            Local local = RepositorioLocales.Instance.ObtenerPorId(idLocal);
            if (local == null)
                throw new InvalidOperationException("No se encontró el local del evento.");
            GestorSesion.Instance.RequerirOrganizadorDeLocal(local.IdPropietario);
        }

        private static void RequerirEventoPropio(int idEvento)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null)
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            RequerirLocalPropio(evento.IdLocal);
        }

        private static void ValidarEvento(Evento e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (string.IsNullOrWhiteSpace(e.Nombre))
            {
                throw new ArgumentException("El evento debe tener nombre.");
            }
            if (!Categorias.Contains(e.Categoria))
            {
                throw new ArgumentException(
                    "El evento debe tener categoría: " + string.Join(", ", Categorias) + ".");
            }
            ValidarPreventa(e);
        }

        // RN-39: una preventa que termina después del evento dejaría la venta entera solo para
        // suscriptores.
        private static void ValidarPreventa(Evento e)
        {
            if (e.FinPreventa.HasValue && e.FinPreventa.Value >= e.Fecha)
            {
                throw new ArgumentException("La preventa tiene que terminar antes de la fecha del evento.");
            }
        }

        private static void ValidarSector(Sector s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.Nombre))
            {
                throw new ArgumentException("El sector debe tener nombre.");
            }
            if (s.Capacidad < 0 || s.Precio < 0)
            {
                throw new ArgumentException("La capacidad y el precio del sector no pueden ser negativos.");
            }
        }

    }

}
    
