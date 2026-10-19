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
            ValidarEvento(e);
            e.Sectores = new List<Sector>();
            e.Estado = EstadoBorrador;
            return RepositorioEventos.Instance.Insertar(e);
        }

        // RN-06: el evento se publica junto con sus sectores, y al menos uno debe tener capacidad.
        // Devuelve el Id generado.
        public int Publicar(Evento e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }
            if (e.Sectores == null || !e.Sectores.Any(s => s != null && s.Capacidad > 0))
            {
                throw new ReglaNegocioException("RN-06",
                    "No se puede publicar el evento: necesita al menos un sector con capacidad mayor que cero.");
            }
            ValidarEvento(e);
            foreach (Sector sector in e.Sectores)
            {
                ValidarSector(sector);
            }
            VerificarCapacidadLocal(e.IdLocal, 0, e.Sectores.Sum(s => s.Capacidad));

            e.Estado = EstadoPublicado;
            return RepositorioEventos.Instance.Insertar(e);
        }

        // RN-11: un evento con entradas vendidas no se borra. Lanza la excepción sin tocar nada;
        // el formulario ofrece Suspender en su lugar.
        public void Eliminar(int idEvento)
        {
            if (RepositorioEventos.Instance.ContarEntradas(idEvento) > 0)
            {
                throw new ReglaNegocioException("RN-11",
                    "El evento tiene entradas vendidas: no se puede eliminar, solo suspender.");
            }
            if (!RepositorioEventos.Instance.Eliminar(idEvento))
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
        }

        // RN-11: alternativa a Eliminar. Vale tenga o no entradas vendidas.
        public void Suspender(int idEvento)
        {
            if (!RepositorioEventos.Instance.ActualizarEstado(idEvento, EstadoSuspendido))
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
        }

        // RN-10: todos los sectores vendieron su capacidad. Ahí se habilita la lista de espera.
        public bool EstaAgotado(int idEvento)
        {
            return RepositorioEventos.Instance.EstaAgotado(idEvento);
        }

        // RN-39: fija o saca (null) la preventa de un evento ya creado.
        public void CambiarPreventa(int idEvento, DateTime? finPreventa)
        {
            Evento evento = RepositorioEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null)
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            evento.FinPreventa = finPreventa;
            ValidarPreventa(evento);
            RepositorioEventos.Instance.ActualizarPreventa(idEvento, finPreventa);
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
            return RepositorioEventos.Instance.ObtenerPorId(idEvento);
        }

        public List<Evento> PorLocal(int idLocal)
        {
            return RepositorioEventos.Instance.ObtenerPorLocal(idLocal);
        }

        public List<Sector> Sectores(int idEvento)
        {
            return RepositorioSectores.Instance.ObtenerPorEvento(idEvento);
        }

        public void AgregarSector(int idEvento, Sector s)
        {
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
            List<Sector> actuales = Sectores(idEvento);
            VerificarCapacidadLocal(evento.IdLocal, actuales.Sum(x => x.Capacidad), s.Capacidad);

            s.IdEvento = idEvento;
            if (evento.Estado == EstadoBorrador && s.Capacidad > 0)
            {
                // Primer sector con capacidad: el borrador ya cumple RN-06 y se publica.
                s.Id = RepositorioSectores.Instance.InsertarYCambiarEstado(s, EstadoPublicado);
            }
            else
            {
                s.Id = RepositorioSectores.Instance.Insertar(s);
            }
        }

        public void EliminarSector(int idSector)
        {
            Sector sector = RepositorioSectores.Instance.ObtenerPorId(idSector);
            if (sector == null)
            {
                throw new InvalidOperationException("No se encontró ningún sector con ese ID.");
            }
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
    
