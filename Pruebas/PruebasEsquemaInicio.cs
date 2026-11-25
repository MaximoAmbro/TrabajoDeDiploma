using System;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datos;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasEsquemaInicio
    {
        // CP-164 — RF-12, RNF-04: detectar el primer script de esquema faltante y explicarlo al abrir.
        [TestMethod("CP-164: Inicio nombra el primer script faltante y lo muestra dentro de la ventana")]
        public void CP164_PrimerScriptYAvisoEnVentana()
        {
            Assert.IsNull(VerificadorEsquema.PrimerFaltante(), "La base recién preparada debe satisfacer el esquema.");
            try
            {
                Ejecutar("DROP INDEX UX_Seguros_ID_Entrada ON dbo.Seguros; " +
                         "ALTER TABLE dbo.Sectores DROP COLUMN CupoLiberadoEl; " +
                         "ALTER TABLE dbo.Entradas DROP CONSTRAINT CK_Entradas_Estado; " +
                         "ALTER TABLE dbo.Entradas DROP COLUMN FechaIngreso;");
                AssertDiagnostico("la columna Entradas.FechaIngreso", "04-seguridad-ingreso.sql");

                Ejecutar("ALTER TABLE dbo.Entradas ADD FechaIngreso DATETIME NULL;");
                AssertDiagnostico("la restricción Entradas.CK_Entradas_Estado", "05-check-estado-entradas.sql");

                using (var form = new Visual.frmInicio())
                {
                    Mostrar(form);
                    var aviso = form.Controls.Find("avisoEsquemaDesactualizado", true).Single();
                    Assert.IsTrue(aviso.Text.Contains("Entradas.CK_Entradas_Estado"));
                    Assert.IsTrue(aviso.Text.Contains("BaseDatos/05-check-estado-entradas.sql"));
                    Assert.IsFalse(ControlDe<System.Windows.Forms.Button>(form, "btninicio").Enabled);
                }

                Ejecutar("ALTER TABLE dbo.Entradas WITH CHECK ADD CONSTRAINT CK_Entradas_Estado " +
                         "CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida'));");
                AssertDiagnostico("la columna Sectores.CupoLiberadoEl", "06-prioridad-temporal.sql");
                Ejecutar("ALTER TABLE dbo.Sectores ADD CupoLiberadoEl DATETIME NULL;");
                AssertDiagnostico("el índice único Seguros.UX_Seguros_ID_Entrada", "07-indice-unico-seguros.sql");
                Ejecutar("CREATE UNIQUE INDEX UX_Seguros_ID_Entrada ON dbo.Seguros(ID_Entrada);");
                Ejecutar("ALTER TABLE dbo.Eventos DROP COLUMN ImagenPortada;");
                AssertDiagnostico("la columna Eventos.ImagenPortada", "09-imagenes-eventos.sql");
                Ejecutar("ALTER TABLE dbo.Eventos ADD ImagenPortada VARBINARY(MAX) NULL;");

                Ejecutar("ALTER TABLE dbo.Entradas DROP CONSTRAINT CK_Entradas_CantidadVendida;");
                AssertDiagnostico("la restricción Entradas.CK_Entradas_CantidadVendida", "10-restringir-cantidad-vendida.sql");
                Ejecutar("ALTER TABLE dbo.Entradas WITH CHECK ADD CONSTRAINT CK_Entradas_CantidadVendida CHECK (CantidadVendida=1);");

                Ejecutar("ALTER TABLE dbo.Usuario DROP CONSTRAINT CK_Usuario_TipoUsuario;");
                AssertDiagnostico("la restricción Usuario.CK_Usuario_TipoUsuario", "11-perfiles-administrador-organizador.sql");
                Ejecutar("ALTER TABLE dbo.Usuario WITH CHECK ADD CONSTRAINT CK_Usuario_TipoUsuario CHECK (TipoUsuario IN ('Cliente','Organizador','Administrador'));");

                Ejecutar("ALTER TABLE dbo.CodigosCorreo DROP COLUMN Destinatario;");
                AssertDiagnostico("la columna CodigosCorreo.Destinatario", "12-solicitudes-organizador.sql");
                Ejecutar("ALTER TABLE dbo.CodigosCorreo ADD Destinatario VARCHAR(180) NULL;");

                Ejecutar("ALTER TABLE dbo.Eventos DROP CONSTRAINT CK_Eventos_Estado;");
                AssertDiagnostico("el estado Eventos.EnRevision", "13-revision-primer-evento.sql");
                Ejecutar("ALTER TABLE dbo.Eventos WITH CHECK ADD CONSTRAINT CK_Eventos_Estado CHECK (Estado IN ('Borrador','EnRevision','Publicado','Suspendido'));");

                Ejecutar("ALTER TABLE dbo.Propietario DROP CONSTRAINT DF_Propietario_PrimerEventoAprobado; ALTER TABLE dbo.Propietario DROP COLUMN PrimerEventoAprobado;");
                AssertDiagnostico("la columna Propietario.PrimerEventoAprobado", "14-primer-evento-aprobado.sql");
                Ejecutar("ALTER TABLE dbo.Propietario ADD PrimerEventoAprobado BIT NOT NULL CONSTRAINT DF_Propietario_PrimerEventoAprobado DEFAULT(0) WITH VALUES;");

                Ejecutar("ALTER TABLE dbo.Eventos DROP COLUMN MotivoRechazo;");
                AssertDiagnostico("la columna Eventos.MotivoRechazo", "15-motivo-rechazo-evento.sql");
                Ejecutar("ALTER TABLE dbo.Eventos ADD MotivoRechazo NVARCHAR(500) NULL;");
                Ejecutar("ALTER TABLE dbo.Locales DROP CONSTRAINT FK_Locales_Propietario;");
                AssertDiagnostico("la clave foránea Locales.PropietarioID → Usuario.ID", "16-fk-local-propietario.sql");
                Ejecutar("ALTER TABLE dbo.Locales WITH CHECK ADD CONSTRAINT FK_Locales_Propietario FOREIGN KEY (PropietarioID) REFERENCES dbo.Usuario(ID);");
                var localSinUsuario = Assert.ThrowsException<SqlException>(() => Ejecutar(
                    "INSERT dbo.Locales(Nombre,Direccion,Capacidad,PropietarioID) VALUES ('Local huérfano de prueba','Dirección de prueba',100,2147483000);"));
                Assert.AreEqual(547, localSinUsuario.Number, "La FK debe bloquear un PropietarioID que no existe.");
                Assert.IsNull(VerificadorEsquema.PrimerFaltante());
            }
            finally
            {
                Ejecutar("IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Seguros') AND name='UX_Seguros_ID_Entrada') " +
                         "CREATE UNIQUE INDEX UX_Seguros_ID_Entrada ON dbo.Seguros(ID_Entrada); " +
                         "IF COL_LENGTH('dbo.Sectores','CupoLiberadoEl') IS NULL ALTER TABLE dbo.Sectores ADD CupoLiberadoEl DATETIME NULL; " +
                         "IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Entradas') AND name='CK_Entradas_Estado') " +
                         "ALTER TABLE dbo.Entradas WITH CHECK ADD CONSTRAINT CK_Entradas_Estado CHECK (Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida')); " +
                         "IF COL_LENGTH('dbo.Entradas','FechaIngreso') IS NULL ALTER TABLE dbo.Entradas ADD FechaIngreso DATETIME NULL;");
                Ejecutar("IF COL_LENGTH('dbo.Eventos','ImagenPortada') IS NULL ALTER TABLE dbo.Eventos ADD ImagenPortada VARBINARY(MAX) NULL;");
                Ejecutar("IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Entradas') AND name='CK_Entradas_CantidadVendida') ALTER TABLE dbo.Entradas WITH CHECK ADD CONSTRAINT CK_Entradas_CantidadVendida CHECK (CantidadVendida=1);");
                Ejecutar("IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Usuario') AND name='CK_Usuario_TipoUsuario') ALTER TABLE dbo.Usuario WITH CHECK ADD CONSTRAINT CK_Usuario_TipoUsuario CHECK (TipoUsuario IN ('Cliente','Organizador','Administrador'));");
                Ejecutar("IF COL_LENGTH('dbo.CodigosCorreo','Destinatario') IS NULL ALTER TABLE dbo.CodigosCorreo ADD Destinatario VARCHAR(180) NULL;");
                Ejecutar("IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID('dbo.Eventos') AND name='CK_Eventos_Estado') ALTER TABLE dbo.Eventos WITH CHECK ADD CONSTRAINT CK_Eventos_Estado CHECK (Estado IN ('Borrador','EnRevision','Publicado','Suspendido'));");
                Ejecutar("IF COL_LENGTH('dbo.Propietario','PrimerEventoAprobado') IS NULL ALTER TABLE dbo.Propietario ADD PrimerEventoAprobado BIT NOT NULL CONSTRAINT DF_Propietario_PrimerEventoAprobado DEFAULT(0) WITH VALUES; ELSE IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE parent_object_id=OBJECT_ID('dbo.Propietario') AND name='DF_Propietario_PrimerEventoAprobado') ALTER TABLE dbo.Propietario ADD CONSTRAINT DF_Propietario_PrimerEventoAprobado DEFAULT(0) FOR PrimerEventoAprobado;");
                Ejecutar("IF COL_LENGTH('dbo.Eventos','MotivoRechazo') IS NULL ALTER TABLE dbo.Eventos ADD MotivoRechazo NVARCHAR(500) NULL;");
                Ejecutar("IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id=OBJECT_ID('dbo.Locales') AND name='FK_Locales_Propietario') ALTER TABLE dbo.Locales WITH CHECK ADD CONSTRAINT FK_Locales_Propietario FOREIGN KEY (PropietarioID) REFERENCES dbo.Usuario(ID);");
            }
        }

        // CP-197 — RF-12, RNF-04: mantener el diagnostico sincronizado con todas las migraciones numeradas.
        [TestMethod("CP-197: El registro de diagnóstico incluye cada migración numerada")]
        public void CP197_RegistroCoincideConMigraciones()
        {
            var scripts = Directory.GetFiles(Path.Combine(BaseDePrueba.RaizDelRepositorio(), "BaseDatos"), "??-*.sql")
                .Select(Path.GetFileName)
                .Where(nombre => { int numero; return int.TryParse(nombre.Substring(0, 2), out numero) && numero >= 4; })
                .OrderBy(nombre => nombre, StringComparer.Ordinal)
                .ToArray();
            CollectionAssert.AreEqual(scripts, VerificadorEsquema.ScriptsRegistrados.ToArray(),
                "Todo script de migración numerado debe tener una comprobación registrada en VerificadorEsquema.");
        }

        private static void AssertDiagnostico(string descripcion, string script)
        {
            var faltante = VerificadorEsquema.PrimerFaltante();
            Assert.IsNotNull(faltante);
            Assert.AreEqual(descripcion, faltante.Descripcion);
            Assert.AreEqual(script, faltante.Script);
        }

        private static void Ejecutar(string sql)
        {
            using (var conexion = new SqlConnection(BaseDePrueba.CadenaDePrueba()))
            using (var comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
