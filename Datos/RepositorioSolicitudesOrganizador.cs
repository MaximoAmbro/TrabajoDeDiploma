using System;
using System.Collections.Generic;
using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioSolicitudesOrganizador : RepositorioBase
    {
        private static readonly RepositorioSolicitudesOrganizador Instancia = new RepositorioSolicitudesOrganizador();
        public static RepositorioSolicitudesOrganizador Instance { get { return Instancia; } }
        private RepositorioSolicitudesOrganizador() { }

        public int Crear(SolicitudOrganizador solicitud)
        {
            const string sql = @"INSERT dbo.SolicitudesOrganizador
(UsuarioID,Responsable,CUIT,Telefono,TelefonoVerificado,NombreLocal,Direccion,Capacidad,Documentacion,NombreArchivo,CBUAlias)
OUTPUT INSERTED.ID VALUES(@UsuarioID,@Responsable,@CUIT,@Telefono,1,@NombreLocal,@Direccion,@Capacidad,@Documentacion,@NombreArchivo,@CBUAlias)";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                Agregar(comando, solicitud);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public SolicitudOrganizador Obtener(int id)
        {
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, "SELECT TOP 1 * FROM dbo.SolicitudesOrganizador WHERE ID=@ID"))
            {
                comando.Parameters.AddWithValue("@ID", id);
                using (var lector = comando.ExecuteReader()) return lector.Read() ? Leer(lector) : null;
            }
        }

        public List<SolicitudOrganizador> Pendientes()
        {
            var salida = new List<SolicitudOrganizador>();
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, "SELECT * FROM dbo.SolicitudesOrganizador WHERE Estado='Pendiente' ORDER BY CreadaEl,ID"))
            using (var lector = comando.ExecuteReader()) while (lector.Read()) salida.Add(Leer(lector));
            return salida;
        }

        public void Aprobar(int id, int administradorId, string motivo)
        {
            using (var conexion = AbrirConexion())
            using (var tx = conexion.BeginTransaction())
            {
                try
                {
                    SolicitudOrganizador s;
                    using (var c = CrearComando(conexion, "SELECT * FROM dbo.SolicitudesOrganizador WITH (UPDLOCK,HOLDLOCK) WHERE ID=@ID AND Estado='Pendiente'", tx))
                    { c.Parameters.AddWithValue("@ID", id); using (var r = c.ExecuteReader()) { if (!r.Read()) throw new InvalidOperationException("La solicitud ya no está pendiente."); s = Leer(r); } }
                    using (var c = CrearComando(conexion, "UPDATE dbo.Usuario SET TipoUsuario='Organizador' WHERE ID=@UsuarioID AND TipoUsuario='Cliente'; DELETE dbo.Cliente WHERE ID=@UsuarioID; INSERT dbo.Propietario(ID) SELECT @UsuarioID WHERE NOT EXISTS(SELECT 1 FROM dbo.Propietario WHERE ID=@UsuarioID);", tx))
                    { c.Parameters.AddWithValue("@UsuarioID", s.UsuarioId); if (c.ExecuteNonQuery() < 1) throw new InvalidOperationException("La cuenta ya no tiene perfil de Cliente."); }
                    using (var c = CrearComando(conexion, "INSERT dbo.Locales(Nombre,Direccion,Capacidad,PropietarioID) VALUES(@Nombre,@Direccion,@Capacidad,@UsuarioID)", tx))
                    { c.Parameters.AddWithValue("@Nombre", s.NombreLocal); c.Parameters.AddWithValue("@Direccion", s.Direccion); c.Parameters.AddWithValue("@Capacidad", s.Capacidad); c.Parameters.AddWithValue("@UsuarioID", s.UsuarioId); c.ExecuteNonQuery(); }
                    using (var c = CrearComando(conexion, "UPDATE dbo.SolicitudesOrganizador SET Estado='Aprobada',MotivoRevision=@Motivo,RevisadaPor=@Admin,RevisadaEl=SYSUTCDATETIME() WHERE ID=@ID AND Estado='Pendiente'; INSERT dbo.Auditoria(UsuarioID,UsuarioMail,Accion,Entidad,EntidadID,Resultado) SELECT @Admin,u.Mail,'Solicitud de Organizador: aprobar','SolicitudOrganizador',@ID,@Motivo FROM dbo.Usuario u WHERE u.ID=@Admin;", tx))
                    { c.Parameters.AddWithValue("@Motivo", motivo); c.Parameters.AddWithValue("@Admin", administradorId); c.Parameters.AddWithValue("@ID", id); c.ExecuteNonQuery(); }
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public void Rechazar(int id, int administradorId, string motivo)
        {
            const string actualizar = @"UPDATE dbo.SolicitudesOrganizador SET Estado='Rechazada',MotivoRevision=@Motivo,RevisadaPor=@Admin,RevisadaEl=SYSUTCDATETIME()
WHERE ID=@ID AND Estado='Pendiente'";
            const string auditar = @"INSERT dbo.Auditoria(UsuarioID,UsuarioMail,Accion,Entidad,EntidadID,Resultado)
SELECT @Admin,u.Mail,'Solicitud de Organizador: rechazar','SolicitudOrganizador',@ID,@Motivo FROM dbo.Usuario u WHERE u.ID=@Admin";
            using (var conexion = AbrirConexion())
            using (var tx = conexion.BeginTransaction())
            {
                try
                {
                    using (var comando = CrearComando(conexion, actualizar, tx))
                    {
                        comando.Parameters.AddWithValue("@Motivo", motivo); comando.Parameters.AddWithValue("@Admin", administradorId); comando.Parameters.AddWithValue("@ID", id);
                        if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("La solicitud ya no está pendiente.");
                    }
                    using (var comando = CrearComando(conexion, auditar, tx))
                    {
                        comando.Parameters.AddWithValue("@Motivo", motivo); comando.Parameters.AddWithValue("@Admin", administradorId); comando.Parameters.AddWithValue("@ID", id);
                        if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("No se pudo registrar la auditoría de la revisión.");
                    }
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        private static void Agregar(SqlCommand c, SolicitudOrganizador s)
        {
            c.Parameters.AddWithValue("@UsuarioID", s.UsuarioId); c.Parameters.AddWithValue("@Responsable", s.Responsable);
            c.Parameters.AddWithValue("@CUIT", s.Cuit); c.Parameters.AddWithValue("@Telefono", s.Telefono);
            c.Parameters.AddWithValue("@NombreLocal", s.NombreLocal); c.Parameters.AddWithValue("@Direccion", s.Direccion);
            c.Parameters.AddWithValue("@Capacidad", s.Capacidad); c.Parameters.AddWithValue("@Documentacion", s.Documentacion);
            c.Parameters.AddWithValue("@NombreArchivo", s.NombreArchivo); c.Parameters.AddWithValue("@CBUAlias", s.CbuAlias);
        }

        private static SolicitudOrganizador Leer(SqlDataReader r)
        {
            return new SolicitudOrganizador { Id = Convert.ToInt32(r["ID"]), UsuarioId = Convert.ToInt32(r["UsuarioID"]),
                Responsable = LeerString(r,"Responsable"), Cuit = LeerString(r,"CUIT"), Telefono = LeerString(r,"Telefono"),
                NombreLocal = LeerString(r,"NombreLocal"), Direccion = LeerString(r,"Direccion"), Capacidad = Convert.ToInt32(r["Capacidad"]),
                Documentacion = (byte[])r["Documentacion"], NombreArchivo = LeerString(r,"NombreArchivo"), CbuAlias = LeerString(r,"CBUAlias"),
                Estado = LeerString(r,"Estado"), MotivoRevision = LeerString(r,"MotivoRevision"),
                RevisadaPor = r["RevisadaPor"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["RevisadaPor"]),
                RevisadaEl = LeerFechaNula(r,"RevisadaEl") };
        }
    }
}
