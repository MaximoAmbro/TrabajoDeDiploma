using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Entidades;
using QRCoder;
using static Entidades.Evento;
namespace Negocio
{
    public partial class GestorEventos //Lista
    {
        List<Evento> eventos = new List<Evento>();
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
        public bool BuscarLista(string NombreEvento)
        {
            Evento evento1 = null;
            foreach (var c in eventos)
            {
                if (c.Nombre == NombreEvento)
                {
                    evento1 = c;
                    break;
                }
            }
            if (evento1 == null)
            {

                return false;
                throw new Exception("Evento no encontrado");
            }
            else
            {
                SectorA = evento1.Sectores[0].Nombre;
                SectorB = evento1.Sectores[1].Nombre;
                SectorC = evento1.Sectores[2].Nombre;
                PrecioA = evento1.Sectores[0].Precio.ToString();
                PrecioB = evento1.Sectores[1].Precio.ToString();
                PrecioC = evento1.Sectores[2].Precio.ToString();
                return true;
            }
        }
        public void GenerarTicket(string NombreEvento, string NombreSector)
        {
            MensajeTicket = null;
            QRCodeImage = null;
            string hora = DateTime.Now.ToString();
            Guid idTicket = Guid.NewGuid();
            foreach (var c in eventos)
            {
                if (c.Nombre == NombreEvento)
                {
                    int codigoEvento = Guid.NewGuid().GetHashCode();
                    string CodigoEvento = codigoEvento.ToString().Substring(0, 8);
                    MensajeTicket =
                        "ENTRADA " + c.Nombre + "\n" +
                        "SECTOR: " + NombreSector + "\n" +
                        "PRECIO: " + c.Sectores.Find(x => x.Nombre == NombreSector).Precio.ToString() + "$\n" +
                        "HORARIO: " + hora + "\n" +
                        "FECHA: " + c.Fecha.ToString();
                    
                    string contenidoQR = "CÓDIGO: " + CodigoEvento;

                    using (var qrGenerator = new QRCodeGenerator())
                    {
                        var qrCodeData = qrGenerator.CreateQrCode(contenidoQR, QRCodeGenerator.ECCLevel.Q);
                        using (var qrCode = new PngByteQRCode(qrCodeData))
                        {
                            QRCodeImage = qrCode.GetGraphic(20);
                        }
                    }
                    break;
                }
            }
        } 
        public List<Evento> ObtenerListaEventos()
        {
            return eventos;
        }
        public void DevolverEntrada(string NombreEvento)
        {
            foreach (var c in eventos)
            {
                if (c.Nombre == NombreEvento)
                {
                    break;
                }
            }
        }
    }
    public partial class GestorEventos //Eventos cargados
    {
        /*public GestorEventos()
        {
           eventos.Add(new Evento
            {
                Nombre = "Ratones Paraonoicos",
                Capacidad = 1500,
                Ubicacion = "Complejo Vorterix",
                Fecha = DateTime.Parse("12/12/2025"),
                Hora = new TimeSpan(21, 0, 0),
                Sectores = new List<Sector>
            {
                new Sector { Nombre = "Sector VIP", Precio = 45000, Capacidad = 500 },
                new Sector { Nombre = "Sector PLATEA", Precio = 20000, Capacidad = 500 },
                new Sector { Nombre = "Sector GENERAL", Precio = 12000, Capacidad = 500 }
            }
            });

            eventos.Add(new Evento
            {
                Nombre = "Bersuit Vergarabat",
                Capacidad = 1500,
                Ubicacion = "Salón Metropolitano",
                Fecha = DateTime.Parse("12/12/2025"),
                Hora = new TimeSpan(22, 0, 0),
                Sectores = new List<Sector>
        {
            new Sector { Nombre = "Sector VIP", Precio = 70000, Capacidad = 500 },
            new Sector { Nombre = "Sector PLATEA", Precio = 45000, Capacidad = 500 },
            new Sector { Nombre = "Sector GENERAL", Precio = 25000, Capacidad = 500 }
        }
            });

            eventos.Add(new Evento
            {
                Nombre = "La Renga",
                Capacidad = 1200,
                Ubicacion = "Salón El Circulo",
                Fecha = DateTime.Parse("12/12/2025"),
                Hora = new TimeSpan(20, 0, 0),
                Sectores = new List<Sector>
        {
            new Sector { Nombre = "Sector VIP", Precio = 60000, Capacidad = 400 },
            new Sector { Nombre = "Sector PLATEA", Precio = 35000, Capacidad = 400 },
            new Sector { Nombre = "Sector GENERAL", Precio = 20000, Capacidad = 400 }
        }
            });
            eventos.Add(new Evento
            {
                Nombre = "Otto",
                Capacidad = 1800,
                Ubicacion = "Brown 3126",
                Fecha = DateTime.Parse("14/12/2025"),
                Hora = new TimeSpan(22, 30, 0),
                Sectores = new List<Sector>
                {
                new Sector { Nombre = "Sector VIP", Precio = 50000, Capacidad = 600 },
                new Sector { Nombre = "Sector PLATEA", Precio = 30000, Capacidad = 600 },
                new Sector { Nombre = "Sector GENERAL", Precio = 18000, Capacidad = 600 }
                }
            }
                );
            eventos.Add(new Evento
            {
                Nombre = "LOVA",
                Capacidad = 700,
                Ubicacion = "Brown 3126",
                Fecha = DateTime.Parse("12/12/2025"),
                Hora = new TimeSpan(00, 30, 0),
                Sectores = new List<Sector>
        {
            new Sector { Nombre = "Sector VIP", Precio = 20000, Capacidad = 400 },
            new Sector { Nombre = "Sector GENERAL", Precio = 15000, Capacidad = 400 },
            new Sector { Nombre = "Sector PERVIA", Precio = 10000, Capacidad = 400 }
        }
            });¨
        }*/
    }
}
    
