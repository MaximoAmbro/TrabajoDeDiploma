using System;
using Microsoft.Data.SqlClient;

namespace Datos
{
    internal sealed class CodigoCorreo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Proposito { get; set; }
        public byte[] Sal { get; set; }
        public byte[] Hash { get; set; }
        public DateTime VenceEl { get; set; }
        public int Intentos { get; set; }
        public bool Usado { get; set; }
        public string Destinatario { get; set; }
    }

    internal sealed class RepositorioCodigosCorreo : RepositorioBase
    {
        private static readonly RepositorioCodigosCorreo Instancia = new RepositorioCodigosCorreo();
        public static RepositorioCodigosCorreo Instance { get { return Instancia; } }
        private RepositorioCodigosCorreo() { }

        public int Emitir(int usuarioId, string proposito, byte[] sal, byte[] hash, int minutos, string destinatario = null)
        {
            const string sql = @"
UPDATE dbo.CodigosCorreo SET Usado=1 WHERE UsuarioID=@UsuarioID AND Proposito=@Proposito AND Usado=0;
INSERT dbo.CodigosCorreo(UsuarioID,Proposito,Sal,CodigoHash,VenceEl,Destinatario)
VALUES(@UsuarioID,@Proposito,@Sal,@Hash,DATEADD(MINUTE,@Minutos,SYSUTCDATETIME()),@Destinatario);
SELECT CONVERT(INT,SCOPE_IDENTITY());";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@UsuarioID", usuarioId);
                AgregarParametro(comando, "@Proposito", proposito);
                AgregarParametro(comando, "@Sal", sal);
                AgregarParametro(comando, "@Hash", hash);
                AgregarParametro(comando, "@Minutos", minutos);
                AgregarParametro(comando, "@Destinatario", destinatario);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public CodigoCorreo PorId(int id)
        {
            const string sql = "SELECT ID,UsuarioID,Proposito,Sal,CodigoHash,VenceEl,Intentos,Usado,Destinatario FROM dbo.CodigosCorreo WHERE ID=@ID";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@ID", id);
                using (var lector = comando.ExecuteReader()) return lector.Read() ? Leer(lector) : null;
            }
        }

        public CodigoCorreo Ultimo(int usuarioId, string proposito)
        {
            const string sql = "SELECT TOP (1) ID,UsuarioID,Proposito,Sal,CodigoHash,VenceEl,Intentos,Usado,Destinatario FROM dbo.CodigosCorreo WHERE UsuarioID=@UsuarioID AND Proposito=@Proposito ORDER BY ID DESC";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@UsuarioID", usuarioId);
                AgregarParametro(comando, "@Proposito", proposito);
                using (var lector = comando.ExecuteReader()) return lector.Read() ? Leer(lector) : null;
            }
        }

        public void RegistrarIntento(int id, int maximo)
        {
            const string sql = "UPDATE dbo.CodigosCorreo SET Intentos=Intentos+1 WHERE ID=@ID AND Usado=0 AND VenceEl>SYSUTCDATETIME() AND Intentos<@Maximo";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@ID", id);
                AgregarParametro(comando, "@Maximo", maximo);
                comando.ExecuteNonQuery();
            }
        }

        public bool Consumir(int id, int usuarioId, string proposito, int maximo, string hashNuevaContrasena)
        {
            using (var conexion = AbrirConexion())
            using (var transaccion = conexion.BeginTransaction())
            {
                try
                {
                    const string consumir = "UPDATE dbo.CodigosCorreo SET Usado=1 WHERE ID=@ID AND UsuarioID=@UsuarioID AND Proposito=@Proposito AND Usado=0 AND Intentos<@Maximo AND VenceEl>SYSUTCDATETIME()";
                    using (var comando = CrearComando(conexion, consumir, transaccion))
                    {
                        AgregarParametro(comando, "@ID", id);
                        AgregarParametro(comando, "@UsuarioID", usuarioId);
                        AgregarParametro(comando, "@Proposito", proposito);
                        AgregarParametro(comando, "@Maximo", maximo);
                        if (comando.ExecuteNonQuery() != 1) { transaccion.Rollback(); return false; }
                    }

                    string actualizar = proposito == "Verificacion"
                        ? "UPDATE dbo.Usuario SET CorreoVerificado=1 WHERE ID=@UsuarioID"
                        : proposito == "Recuperacion"
                            ? "UPDATE dbo.Usuario SET Contraseña=@Hash,IntentosFallidos=0,BloqueadoHasta=NULL WHERE ID=@UsuarioID"
                            : "UPDATE dbo.Usuario SET CorreoVerificado=CorreoVerificado WHERE ID=@UsuarioID";
                    using (var comando = CrearComando(conexion, actualizar, transaccion))
                    {
                        AgregarParametro(comando, "@UsuarioID", usuarioId);
                        if (hashNuevaContrasena != null) AgregarParametro(comando, "@Hash", hashNuevaContrasena);
                        if (comando.ExecuteNonQuery() != 1) { transaccion.Rollback(); return false; }
                    }
                    transaccion.Commit();
                    return true;
                }
                catch { transaccion.Rollback(); throw; }
            }
        }

        private static CodigoCorreo Leer(SqlDataReader lector)
        {
            return new CodigoCorreo
            {
                Id = Convert.ToInt32(lector["ID"]), UsuarioId = Convert.ToInt32(lector["UsuarioID"]),
                Proposito = Convert.ToString(lector["Proposito"]), Sal = (byte[])lector["Sal"],
                Hash = (byte[])lector["CodigoHash"], VenceEl = DateTime.SpecifyKind(Convert.ToDateTime(lector["VenceEl"]), DateTimeKind.Utc),
                Intentos = Convert.ToInt32(lector["Intentos"]), Usado = Convert.ToBoolean(lector["Usado"])
                , Destinatario = lector["Destinatario"] == DBNull.Value ? null : Convert.ToString(lector["Destinatario"])
            };
        }
    }
}
