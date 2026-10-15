using Datos;
using Entidades;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Entidades.Evento;
using static QRCoder.PayloadGenerator;
namespace Negocio
{
    public partial class GestorEventos //Singleton
    {
        public const string EstadoPublicado = "Publicado";
        public const string EstadoSuspendido = "Suspendido";

        // Límite heredado, no es una RN documentada: FrmCompraCliente solo muestra tres
        // sectores (A, B y C) por evento.
        private const int MaximoSectores = 3;

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

        // TODO Seccion 7: FrmCompraCliente crea su propia instancia por el estado que deja
        // GenerarTicket (MensajeTicket, QRCodeImage). Al migrarlo, hacer privado el constructor.
        public GestorEventos() { }
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
        public string MensajeTicket { get; set; }
        public byte[] QRCodeImage { get; set; }
    }
    public partial class GestorEventos //Metodos
    {
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
            if (e.Sectores.Count > MaximoSectores)
            {
                throw new InvalidOperationException(
                    $"Un evento admite como máximo {MaximoSectores} sectores.");
            }
            foreach (Sector sector in e.Sectores)
            {
                ValidarSector(sector);
            }

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

        // RN-07
        public int CapacidadTotal(int idEvento)
        {
            return Sectores(idEvento).Sum(s => s.Capacidad);
        }

        // RN-09: solo eventos con fecha posterior a la actual.
        public List<Evento> Vigentes()
        {
            return RepositorioEventos.Instance.ObtenerPosterioresA(DateTime.Now);
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
            if (RepositorioEventos.Instance.ObtenerPorId(idEvento) == null)
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            if (Sectores(idEvento).Count >= MaximoSectores)
            {
                throw new InvalidOperationException(
                    $"Ya se registraron {MaximoSectores} sectores para este evento. No se pueden agregar más.");
            }

            s.IdEvento = idEvento;
            s.Id = RepositorioSectores.Instance.Insertar(s);
        }

        public void EliminarSector(int idSector)
        {
            if (RepositorioSectores.Instance.ContarEntradas(idSector) > 0)
            {
                throw new InvalidOperationException(
                    "El sector tiene entradas vendidas: no se puede eliminar.");
            }
            if (!RepositorioSectores.Instance.Eliminar(idSector))
            {
                throw new InvalidOperationException("No se encontró ningún sector con ese ID.");
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

        public void GenerarTicket(string NombreEvento, string NombreSector, string PrecioSector, string FechaEvento)
        {
            MensajeTicket = null;
            QRCodeImage = null;
            string hora = DateTime.Now.ToString();
            Guid idTicket = Guid.NewGuid();

                    int codigoEvento = Guid.NewGuid().GetHashCode();
                    string CodigoEvento = codigoEvento.ToString().Substring(0, 8);
                    MensajeTicket =
                        "ENTRADA " + NombreEvento + "\n" +
                        "SECTOR: " + NombreSector + "\n" +
                        "PRECIO: " + PrecioSector + "$\n" +
                        "FECHA: " + FechaEvento + "\n";

            string contenidoQR = "CÓDIGO: " + CodigoEvento;

                    using (var qrGenerator = new QRCodeGenerator())
                    {
                        var qrCodeData = qrGenerator.CreateQrCode(contenidoQR, QRCodeGenerator.ECCLevel.Q);
                        using (var qrCode = new PngByteQRCode(qrCodeData))
                        {
                            QRCodeImage = qrCode.GetGraphic(20);
                        }
                    }

        }
       

    }

}
    
