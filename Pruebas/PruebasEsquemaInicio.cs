using System;
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
            }
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
