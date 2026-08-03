using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Negocio
{
    public partial class GestorClientes // Lista
    {
        public static List<Cliente> clientes = new List<Cliente>();
        Cliente cliente = new Cliente();
        private static GestorClientes _instance;

        public static GestorClientes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorClientes();
                }
                return _instance;
            }
        }

    }
    public partial class GestorClientes // Metodos
    {

        public void CambiarContraseña(string mail, string contraseña)
        {
            foreach (var c in clientes)
            {
                if (c.Mail == mail)
                {
                    c.Contraseña = null;
                    c.Contraseña = contraseña;
                    break;
                }
            }
        }
        public void AgregarCliente(string nombre, string apellido, string mail, string contraseña)
        {
            cliente = new Cliente { Nombre = nombre, Apellido = apellido, Mail = mail, Contraseña = contraseña }; clientes.Add(cliente);
        }
        public bool EncontrarContraseña(string contraseña, string mail)
        {
            Cliente cliente1 = null;
            foreach (var c in clientes)
            {
                if (c.Mail == mail && c.Contraseña == contraseña)
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
        public bool EncontrarMail(string mail)
        {
            Cliente cliente1 = null;
            foreach (var c in clientes)
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
        public void AgregarEntrada(Evento evento, string mail)
        {
            Cliente cliente1 = null;
            foreach (var c in clientes)
            {
                if (c.Mail == mail)
                {
                    cliente1 = c;
                    cliente1.entradasUsuario.Add(evento);
                    break;
                }
            }
        }
        public List<Evento> ObtenerListaEventos(string mail)
        {
            Cliente cliente1 = null;
            foreach (var c in clientes)
            {
                if (c.Mail == mail)
                {
                    cliente1 = c;
                    return cliente1.entradasUsuario;
                }
                cliente1 = null;
            }
            return null;
        }
        public void ObtenerNombre(string mail, out string nombre)
        {
            nombre = "";
            foreach (var c in clientes)
            {
                if (c.Mail == mail)
                {
                    nombre = c.Nombre;
                    break;
                }
            }
        }
    }
        public partial class GestorClientes // Usuarios cargados
    {
        public void CargarLista()
        {
            clientes.Add(new Cliente
            {
                Nombre = "Pedro",
                Apellido = "Lopez",
                Mail = "C",
                Contraseña = "C"
            });
        } 
    } 
}

