using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Datos
{
    public sealed class RepositorioAuditoria : RepositorioBase
    {
        private static readonly RepositorioAuditoria Instancia = new RepositorioAuditoria();
        public static RepositorioAuditoria Instance { get { return Instancia; } }
        private RepositorioAuditoria() { }

        public void Registrar(int? usuarioId, string usuarioMail, string accion, string entidad, int? entidadId, string resultado)
        {
            const string sql = "INSERT dbo.Auditoria(UsuarioID,UsuarioMail,Accion,Entidad,EntidadID,Resultado) VALUES(@UsuarioID,@Mail,@Accion,@Entidad,@EntidadID,@Resultado)";
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@UsuarioID", usuarioId);
                AgregarParametro(comando, "@Mail", usuarioMail);
                AgregarParametro(comando, "@Accion", accion);
                AgregarParametro(comando, "@Entidad", entidad);
                AgregarParametro(comando, "@EntidadID", entidadId);
                AgregarParametro(comando, "@Resultado", resultado);
                comando.ExecuteNonQuery();
            }
        }

        public List<RegistroAuditoria> Consultar(DateTime desde, DateTime hasta, string accion)
        {
            const string sql = @"SELECT TOP (500) ID,UsuarioID,UsuarioMail,Accion,Entidad,EntidadID,Fecha,Resultado
FROM dbo.Auditoria WHERE Fecha>=@Desde AND Fecha<@Hasta AND (@Accion IS NULL OR Accion=@Accion)
ORDER BY Fecha DESC,ID DESC";
            var salida = new List<RegistroAuditoria>();
            using (var conexion = AbrirConexion())
            using (var comando = CrearComando(conexion, sql))
            {
                AgregarParametro(comando, "@Desde", desde);
                AgregarParametro(comando, "@Hasta", hasta);
                AgregarParametro(comando, "@Accion", string.IsNullOrWhiteSpace(accion) ? null : accion.Trim());
                using (var lector = comando.ExecuteReader())
                    while (lector.Read()) salida.Add(new RegistroAuditoria
                    {
                        Id = lector.GetInt64(0), UsuarioId = lector.IsDBNull(1) ? (int?)null : lector.GetInt32(1),
                        UsuarioMail = lector.IsDBNull(2) ? null : lector.GetString(2), Accion = lector.GetString(3),
                        Entidad = lector.IsDBNull(4) ? null : lector.GetString(4), EntidadId = lector.IsDBNull(5) ? (int?)null : lector.GetInt32(5),
                        Fecha = lector.GetDateTime(6), Resultado = lector.GetString(7)
                    });
            }
            return salida;
        }
    }
}
