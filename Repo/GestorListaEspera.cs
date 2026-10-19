using System;
using System.Collections.Generic;
using Datos;
using Entidades;

namespace Negocio
{
    // RF-10 + CU-07. En esta versión la lista es solo consulta: no se notifica a nadie ni se asigna
    // el cupo que se libere.
    public sealed class GestorListaEspera
    {
        private static GestorListaEspera _instance;

        public static GestorListaEspera Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GestorListaEspera();
                }
                return _instance;
            }
        }

        private GestorListaEspera() { }

        // RN-09: evento vigente. RN-10: solo con el evento agotado. RN-36: una vez por evento.
        // RN-35: la condición de suscriptor se congela acá; si después se le vence el plan, el
        // lugar en la lista no cambia.
        public void Inscribir(int idEvento, int idUsuario)
        {
            Evento evento = GestorEventos.Instance.ObtenerPorId(idEvento);
            if (evento == null)
            {
                throw new InvalidOperationException("No se encontró ningún evento con ese ID.");
            }
            if (evento.Estado != GestorEventos.EstadoPublicado || evento.Fecha <= DateTime.Now)
            {
                throw new ReglaNegocioException("RN-09",
                    "El evento ya pasó o no está publicado: no admite inscripciones en la lista de espera.");
            }
            if (!GestorEventos.Instance.EstaAgotado(idEvento))
            {
                throw new ReglaNegocioException("RN-10",
                    "El evento todavía tiene entradas: la lista de espera se habilita cuando se agota.");
            }
            if (EstaInscripto(idEvento, idUsuario))
            {
                throw YaInscripto();
            }

            var inscripcion = new ListaEspera
            {
                IdEvento = idEvento,
                IdUsuario = idUsuario,
                FechaAlta = DateTime.Now,
                ConSuscripcion = GestorSuscripciones.Instance.TieneActiva(idUsuario)
            };
            try
            {
                RepositorioListaEspera.Instance.Insertar(inscripcion);
            }
            catch (RegistroDuplicadoException)
            {
                throw YaInscripto();   // dos ventanas a la vez: UX_ListaEspera frenó la segunda
            }
        }

        public bool EstaInscripto(int idEvento, int idUsuario)
        {
            return RepositorioListaEspera.Instance.Existe(idEvento, idUsuario);
        }

        // RN-35: suscriptores primero y, dentro de cada grupo, por orden de inscripción.
        public List<ListaEspera> DeEvento(int idEvento)
        {
            return RepositorioListaEspera.Instance.ObtenerPorEvento(idEvento);
        }

        private static ReglaNegocioException YaInscripto()
        {
            return new ReglaNegocioException("RN-36",
                "Ya está en la lista de espera de este evento. Le avisaremos si se libera un lugar.");
        }
    }
}
