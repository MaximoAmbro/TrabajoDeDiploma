using System;
using System.Linq;
using Datos;
using Entidades;
using Microsoft.Data.SqlClient;
using Negocio;

namespace Pruebas
{
    // Constructor de datos de prueba. Todo lo que crea lleva una marca reconocible y Limpiar borra
    // por esa marca, como 03-datos-reporte.sql: usuarios con mail test…@pruebas.eventaura.com,
    // locales y servicios cuyo nombre empieza con «test ». Nunca modifica los datos de la semilla.
    //
    // Prepara el ESTADO con SQL directo (usuarios, compras históricas, eventos, precios), así una
    // prueba no depende de la regla que está probando para armar su escenario. Las OPERACIONES
    // bajo prueba (armar el paquete, iniciar, confirmar) van siempre por los gestores.
    public static class Fake
    {
        private const string DominioMail = "@pruebas.eventaura.com";
        private const string Prefijo = "test ";

        private static int contador;

        public static void Limpiar()
        {
            const string consulta =
                "DECLARE @U TABLE (ID INT PRIMARY KEY); " +
                "INSERT INTO @U SELECT ID FROM Usuario WHERE Mail LIKE 'test%' + @Dominio; " +
                "DECLARE @L TABLE (ID INT PRIMARY KEY); " +
                "INSERT INTO @L SELECT ID FROM Locales WHERE Nombre LIKE @Prefijo + '%'; " +
                "DECLARE @E TABLE (ID INT PRIMARY KEY); " +
                "INSERT INTO @E SELECT ID FROM Eventos WHERE LocalID IN (SELECT ID FROM @L); " +
                "DECLARE @S TABLE (ID INT PRIMARY KEY); " +
                "INSERT INTO @S SELECT ID FROM Servicios WHERE Nombre LIKE @Prefijo + '%'; " +
                "DECLARE @C TABLE (ID INT PRIMARY KEY); " +
                "INSERT INTO @C SELECT ID FROM Compras WHERE ID_Usuario IN (SELECT ID FROM @U) " +
                "UNION SELECT ID_Compra FROM Entradas WHERE ID_Evento IN (SELECT ID FROM @E) AND ID_Compra IS NOT NULL; " +
                "DELETE s FROM Seguros s JOIN Entradas e ON e.ID = s.ID_Entrada " +
                "  WHERE e.ID_Compra IN (SELECT ID FROM @C) OR e.ID_Evento IN (SELECT ID FROM @E); " +
                "DELETE FROM Entradas WHERE ID_Compra IN (SELECT ID FROM @C) OR ID_Evento IN (SELECT ID FROM @E); " +
                "DELETE FROM CompraServicios WHERE ID_Compra IN (SELECT ID FROM @C) OR ID_Servicio IN (SELECT ID FROM @S); " +
                "DELETE FROM Pagos WHERE ID_Compra IN (SELECT ID FROM @C); " +
                "DELETE FROM Compras WHERE ID IN (SELECT ID FROM @C); " +
                "DELETE FROM ListaEspera WHERE ID_Usuario IN (SELECT ID FROM @U) OR ID_Evento IN (SELECT ID FROM @E); " +
                "DELETE FROM Suscripciones WHERE ID_Usuario IN (SELECT ID FROM @U); " +
                "DELETE FROM ServiciosEvento WHERE ID_Evento IN (SELECT ID FROM @E) OR ID_Servicio IN (SELECT ID FROM @S); " +
                "DELETE FROM Servicios WHERE ID IN (SELECT ID FROM @S); " +
                "DELETE FROM Sectores WHERE EventoID IN (SELECT ID FROM @E); " +
                "DELETE FROM Eventos WHERE ID IN (SELECT ID FROM @E); " +
                "DELETE FROM Locales WHERE ID IN (SELECT ID FROM @L); " +
                "DELETE FROM Cliente WHERE ID IN (SELECT ID FROM @U); " +
                "DELETE FROM Usuario WHERE ID IN (SELECT ID FROM @U);";
            Ejecutar(consulta, "@Dominio", DominioMail, "@Prefijo", Prefijo);
        }

        // Cliente con el nivel persistido indicado y evaluado este mes: ReevaluarNivel no lo toca
        // (RN-23), así que la compra se cobra con ese nivel (RN-22). No tiene compras.
        public static Entidades.Cliente Cliente(NivelFidelidad nivel)
        {
            int n = ++contador;
            int id = Escalar<int>(
                "INSERT INTO Usuario (Nombre, Apellido, Mail, Contraseña, TipoUsuario, Nivel, FechaUltimaEvaluacion) " +
                "OUTPUT INSERTED.ID VALUES ('Cliente', @Apellido, @Mail, 'Cliente123', 'Cliente', @Nivel, @Hoy)",
                "@Apellido", "Prueba " + n, "@Mail", $"test{n:0000}{DominioMail}",
                "@Nivel", nivel.ToString(), "@Hoy", DateTime.Today);
            Ejecutar("INSERT INTO Cliente (ID) VALUES (@ID)", "@ID", id);
            return GestorFidelizacion.Instance.ObtenerCliente(id);
        }

        // Cliente con «cantidad» compras confirmadas de hace «haceMeses» meses. El nivel persistido
        // queda en el que corresponde a esas compras, como lo dejaría la reevaluación mensual.
        public static Entidades.Cliente ClienteConCompras(int cantidad, int haceMeses)
        {
            Entidades.Cliente c = Cliente(NivelFidelidad.Bronce);
            for (int i = 0; i < cantidad; i++)
            {
                AgregarCompra(c, haceMeses, EstadoCompra.Confirmada);
            }
            NivelFidelidad nivel = GestorFidelizacion.Instance.DeterminarNivel(c.Id, DateTime.Now);
            Ejecutar("UPDATE Usuario SET Nivel = @Nivel WHERE ID = @ID", "@Nivel", nivel.ToString(), "@ID", c.Id);
            return GestorFidelizacion.Instance.ObtenerCliente(c.Id);
        }

        // Una compra histórica, sin entradas: para la ventana de RN-19 solo cuenta la fila de
        // Compras. Cierra Base − Descuento + Servicios + Cargo + Seguro = TotalPagado.
        public static void AgregarCompra(Entidades.Cliente c, int haceMeses, EstadoCompra estado)
        {
            DateTime fecha = DateTime.Now.AddMonths(-haceMeses).AddMinutes(-(++contador));
            Ejecutar(
                "INSERT INTO Compras (ID_Usuario, FechaCompra, Estado, NivelAplicado, DescuentoAplicado, BaseEntradas, " +
                "                     MontoDescuento, MontoServicios, CargoServicio, MontoSeguro, ConSeguro, TotalPagado) " +
                "VALUES (@Usuario, @Fecha, @Estado, 'Bronce', 0, 50000, 0, 0, 7500, 0, 0, 57500)",
                "@Usuario", c.Id, "@Fecha", fecha, "@Estado", estado.ToString());
        }

        // Evento Publicado dentro de 30 días, en un local propio, con un sector «Campo».
        public static Evento EventoPublicado(decimal precio, int capacidad)
        {
            int n = ++contador;
            int propietario = Escalar<int>("SELECT TOP 1 ID FROM Usuario WHERE TipoUsuario = 'Propietario' ORDER BY ID");
            int local = Escalar<int>(
                "INSERT INTO Locales (Nombre, Direccion, Capacidad, PropietarioID) OUTPUT INSERTED.ID " +
                "VALUES (@Nombre, 'Calle de prueba 123', 100000, @Propietario)",
                "@Nombre", $"{Prefijo}Local {n}", "@Propietario", propietario);
            int evento = Escalar<int>(
                "INSERT INTO Eventos (Nombre, Fecha, LocalID, Categoria, Estado) OUTPUT INSERTED.ID " +
                "VALUES (@Nombre, @Fecha, @Local, 'Concierto', 'Publicado')",
                "@Nombre", $"{Prefijo}Evento {n}", "@Fecha", DateTime.Today.AddDays(30).AddHours(21), "@Local", local);
            Ejecutar("INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) VALUES (@Evento, 'Campo', @Precio, @Capacidad)",
                     "@Evento", evento, "@Precio", precio, "@Capacidad", capacidad);
            return Releer(evento);
        }

        public static Sector AgregarSector(Evento e, decimal precio, int capacidad)
        {
            int id = Escalar<int>(
                "INSERT INTO Sectores (EventoID, Nombre, Precio, Capacidad) OUTPUT INSERTED.ID " +
                "VALUES (@Evento, @Nombre, @Precio, @Capacidad)",
                "@Evento", e.Id, "@Nombre", "Sector " + (++contador), "@Precio", precio, "@Capacidad", capacidad);
            e.Sectores = GestorEventos.Instance.Sectores(e.Id);
            return e.Sectores.First(s => s.Id == id);
        }

        // RN-14: un servicio propio, habilitado para el evento.
        public static Servicio ServicioHabilitado(Evento e, string tipo, decimal precio)
        {
            int id = Escalar<int>(
                "INSERT INTO Servicios (Nombre, Tipo, Precio, Activo) OUTPUT INSERTED.ID VALUES (@Nombre, @Tipo, @Precio, 1)",
                "@Nombre", $"{Prefijo}{tipo} {++contador}", "@Tipo", tipo, "@Precio", precio);
            Ejecutar("INSERT INTO ServiciosEvento (ID_Evento, ID_Servicio) VALUES (@Evento, @Servicio)",
                     "@Evento", e.Id, "@Servicio", id);
            return GestorPaquetes.Instance.ServiciosDe(e.Id).First(s => s.Id == id);
        }

        // Paquete armado con los gestores: «entradasPorSector» entradas en cada sector del evento.
        // Los eventos de Fake no tienen preventa, así que el usuario de Nuevo no interviene (RN-39).
        public static Paquete Paquete(Evento e, int entradasPorSector, bool conSeguro)
        {
            Paquete p = GestorPaquetes.Instance.Nuevo(e.Id, 0);
            foreach (Sector s in GestorEventos.Instance.Sectores(e.Id))
            {
                GestorPaquetes.Instance.AgregarEntradas(p, s.Id, entradasPorSector);
            }
            p.ConSeguro = conSeguro;
            return p;
        }

        // Cambio de tarifa por fuera de la aplicación (CP-07).
        public static void CambiarPrecioDeSector(int idSector, decimal nuevoPrecio)
        {
            Ejecutar("UPDATE Sectores SET Precio = @Precio WHERE ID = @ID", "@Precio", nuevoPrecio, "@ID", idSector);
        }

        public static Suscripcion SuscripcionActiva(Entidades.Cliente c, string plan)
        {
            DateTime hoy = DateTime.Today;
            Ejecutar(
                "INSERT INTO Suscripciones (ID_Usuario, [Plan], FechaInicio, FechaFin, Precio, Activa) " +
                "VALUES (@Usuario, @Plan, @Inicio, @Fin, @Precio, 1)",
                "@Usuario", c.Id, "@Plan", plan, "@Inicio", hoy,
                "@Fin", plan == GestorSuscripciones.PlanAnual ? hoy.AddYears(1) : hoy.AddMonths(1),
                "@Precio", GestorSuscripciones.Instance.PrecioDe(plan));
            return GestorSuscripciones.Instance.ActivaDe(c.Id);
        }

        // Compra completa con los gestores: Nuevo, AgregarEntradas, Iniciar y Confirmar con una
        // tarjeta que la pasarela simulada aprueba. Devuelve la compra releída de la base.
        public static Compra CompraConfirmada(Entidades.Cliente c, Evento e, int entradas, bool conSeguro)
        {
            Paquete p = GestorPaquetes.Instance.Nuevo(e.Id, c.Id);
            GestorPaquetes.Instance.AgregarEntradas(p, GestorEventos.Instance.Sectores(e.Id).First().Id, entradas);
            p.ConSeguro = conSeguro;
            Compra compra = GestorCompras.Instance.Iniciar(p, GestorFidelizacion.Instance.ObtenerCliente(c.Id));
            if (!GestorCompras.Instance.Confirmar(compra, Tarjeta()))
            {
                throw new InvalidOperationException("La pasarela simulada rechazó la tarjeta de prueba.");
            }
            return GestorCompras.Instance.ObtenerPorId(compra.Id);
        }

        // La pasarela simulada aprueba cualquier número que no termine en 0000.
        public static DatosTarjeta Tarjeta()
        {
            return new DatosTarjeta
            {
                Titular = "Cliente de Prueba",
                Numero = "4111 1111 1111 1111",
                Vencimiento = DateTime.Today.AddYears(2).ToString("MM/yy"),
                Cvv = "123"
            };
        }

        // ------------------------------------- privados -------------------------------------

        private static Evento Releer(int idEvento)
        {
            Evento evento = GestorEventos.Instance.ObtenerPorId(idEvento);
            evento.Sectores = GestorEventos.Instance.Sectores(idEvento);
            return evento;
        }

        private static void Ejecutar(string consulta, params object[] parametros)
        {
            using (SqlConnection conexion = Conexion.Instance.Crear())
            using (SqlCommand comando = Comando(consulta, conexion, parametros))
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        private static T Escalar<T>(string consulta, params object[] parametros)
        {
            using (SqlConnection conexion = Conexion.Instance.Crear())
            using (SqlCommand comando = Comando(consulta, conexion, parametros))
            {
                conexion.Open();
                return (T)Convert.ChangeType(comando.ExecuteScalar(), typeof(T));
            }
        }

        private static SqlCommand Comando(string consulta, SqlConnection conexion, object[] parametros)
        {
            var comando = new SqlCommand(consulta, conexion);
            for (int i = 0; i < parametros.Length; i += 2)
            {
                comando.Parameters.AddWithValue((string)parametros[i], parametros[i + 1] ?? DBNull.Value);
            }
            return comando;
        }
    }
}
