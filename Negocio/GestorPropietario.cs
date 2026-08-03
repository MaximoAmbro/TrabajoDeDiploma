using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Negocio;

namespace Repositorio
{
    public partial class GestorPropietario
    {
        public List <Propietario> propietarios = new List<Propietario>();
        private static GestorPropietario _instance;

        public static GestorPropietario Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorPropietario();
                }
                return _instance;
            }
        }
    } // Lista
    public partial class GestorPropietario
    {
        public void AgregarPropietario(string nombre, string apellido, string mail, string contraseña)
        {
            Propietario propietario = new Propietario { Nombre = nombre, Apellido = apellido, Mail = mail, Contraseña = contraseña }; propietarios.Add(propietario);
        }
        public void AgregarEventoLocal(string mail, string nombreLocal, string nombreEvento, TimeSpan hora , DateTime fecha)
        {

            foreach (var c in propietarios)
            {
                if (c.Mail == mail)
                {
                  foreach (var l in c.Locales)
                    {
                        if (l.Nombre == nombreLocal)
                        {
                            l.Eventos.Add(new Evento
                            {
                                Nombre = "Evento",
                                Hora = new TimeSpan(10, 0, 0),
                                Fecha = DateTime.Now
                                , Id = 1
                            });
                        }
                    }
                }
            }
        }
        public bool EncontrarMail(string mail)
        {
            Propietario cliente1 = null;
            foreach (var c in propietarios)
            {
                if (c.Mail == mail)
                {
                    cliente1 = c;
                    break;
                }
            }
            if (cliente1 == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool EncontrarContraseña(string contraseña, string mail)
        {
            Propietario vendedor1= null;
            foreach (var c in propietarios)
            {
                if (c.Mail == mail && c.Contraseña == contraseña)
                {
                    vendedor1 = c;
                    break;
                }
            }
            if (vendedor1 == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<Local> ObtenerListaLocales(string mail)
        {
            Propietario vendedor1 = null;
            foreach (var c in   propietarios)
            {
                if (c.Mail == mail)
                {
                    vendedor1 = c;
                    return vendedor1.Locales;
                }
                vendedor1 = null;
            }
            return null;
        }
        public List<Evento> ObtenerListaEventos(string Mail, string nombreLocal)
        {
            Propietario vendedor1 = null;
            foreach (var c in propietarios)
            {
                if (c.Mail == Mail)
                {
                    foreach (var local in c.Locales)
                    {
                        if (local.Nombre == nombreLocal)
                        {
                            return local.Eventos;
                        }
                    }
                }
                vendedor1 = null;
            }
            return null;
        }
        public List<Sector> ObtenerListaSectores(string Mail, string nombreLocal, string evento)
        {
            Propietario vendedor1 = null;
            foreach (var c in propietarios)
            {
                if (c.Mail == Mail)
                {
                    foreach (var l in c.Locales)
                    {
                        if (l.Nombre == nombreLocal)
                        {
                            foreach (var e in l.Eventos)
                            {
                                if (e.Nombre == evento)
                                {
                                    return e.Sectores;
                                }
                            }
                        }
                    }
                }
                vendedor1 = null;
            }
            return null;
        }
        public void DevolverNombre(string mail, out string nombre)
        {
            nombre = "";
            foreach (var c in propietarios)
            {
                if (c.Mail == mail)
                {
                    nombre = c.Nombre;
                    break;
                }
            }
        }
        public void EliminarEvento(string Mail, string nombreLocal, string nombreEvento)
        {
            foreach(var c in propietarios)
            {
                if (c.Mail == Mail)
                {
                    foreach (var local in c.Locales)
                    {
                        if (local.Nombre == nombreLocal)
                        {
                            foreach (var evento in local.Eventos)
                            {
                                if (evento.Nombre == nombreEvento)
                                {
                                    local.Eventos.Remove(evento);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

    } // Metodos
    public partial class GestorPropietario
    {
        public void CargarLista()
        {
            var propietario = new Propietario { Nombre = "Ambro", Apellido = "Sape", Mail = "V", Contraseña = "V" };
            propietario.Locales = new List<Local>
            {
            new Local { Nombre = "Local 1", Direccion = "Calle 1",  
                Capacidad = 100,  Eventos = new List<Evento>
            {
                new Evento { Nombre = "Evento 1", Fecha = DateTime.Now, Hora = new TimeSpan(10, 0, 0), Sectores = new List<Sector>
                 {
                new Sector { Nombre = "Sector VIP", Precio = 100, Capacidad = 50 },
                new Sector { Nombre = "Sector General", Precio = 50, Capacidad = 50 },
                new Sector { Nombre = "Sector Preferencial", Precio = 75, Capacidad = 50 }
                }   },
                new Evento { Nombre = "Evento 2", Fecha = DateTime.Now.AddDays(1), Hora = new TimeSpan(10, 0, 0), Sectores = new List<Sector>
                {
                new Sector { Nombre = "Sector VIP", Precio = 120, Capacidad = 50 },
                new Sector { Nombre = "Sector General", Precio = 60, Capacidad = 50 },
                new Sector { Nombre = "Sector Preferencial", Precio = 90, Capacidad = 50 }
                }   }
            }},
            new Local { Nombre = "Local 2", Direccion = "Calle 2", 
                Capacidad = 200  , Eventos = new List<Evento>
                {
                new Evento { Nombre = "Evento 1", Fecha = DateTime.Now, Hora = new TimeSpan(10, 0, 0),Sectores = new List<Sector>
                {
                new Sector { Nombre = "Sector VIP", Precio = 150, Capacidad = 50 },
                new Sector { Nombre = "Sector General", Precio = 75, Capacidad = 50 },
                new Sector { Nombre = "Sector Preferencial", Precio = 112, Capacidad = 50 }
                }   },
                new Evento { Nombre = "Evento 2", Fecha = DateTime.Now.AddDays(1), Hora = new TimeSpan(10, 0, 0) , Sectores = new List<Sector>
                {
                new Sector { Nombre = "Sector VIP", Precio = 180, Capacidad = 50 },
                new Sector { Nombre = "Sector General", Precio = 90, Capacidad = 50 },
                new Sector { Nombre = "Sector Preferencial", Precio = 135, Capacidad = 50 }
                }   }
            }}
            };
            propietarios.Add(propietario);
        }

    } // Creacion de Elementos
}
