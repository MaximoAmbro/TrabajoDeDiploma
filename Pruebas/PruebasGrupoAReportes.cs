using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasGrupoAReportesSemilla
    {
        // CP-86 — RF-11; PA-21, 9.15: los cinco reportes admiten la semilla sin datos de volumen.
        [TestMethod("CP-86: Semilla sola genera y exporta los cinco reportes sin ventas del evento 1")]
        public void CP86_SemillaSola()
        {
            EnSta(() =>
            {
                Assert.AreEqual(0, Fake.ContarDatosReporte(), "El escenario de volumen no debe filtrarse a la semilla.");
                GestorSesion.Instance.Iniciar(Fake.Administrador());
                DateTime desde = DateTime.Today.AddYears(-1), hasta = DateTime.Today.AddYears(1);
                foreach (string codigo in GestorReportes.Codigos)
                {
                    Reporte reporte = GestorReportes.Instance.Crear(codigo, desde, hasta);
                    DataTable datos = GestorReportes.Instance.Ejecutar(reporte);
                    var exportador = new ExportadorMemoria();
                    Assert.AreEqual("memoria.pdf", GestorReportes.Instance.Exportar(reporte, exportador, "memoria.pdf"));
                    Assert.IsNotNull(exportador.Datos);
                    Assert.AreEqual(datos.Rows.Count, exportador.Datos.Rows.Count);
                }
                using (var form = new Visual.frmReportes())
                {
                    Mostrar(form);
                    ControlDe<DateTimePicker>(form, "dtpDesde").Value = desde;
                    ControlDe<DateTimePicker>(form, "dtpHasta").Value = hasta;
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnGenerar").PerformClick());
                    string nombre = GestorEventos.Instance.ObtenerPorId(1).Nombre;
                    DataGridView grilla = ControlDe<DataGridView>(form, "dgvReporte");
                    DataGridViewRow fila = grilla.Rows.Cast<DataGridViewRow>().Single(r => Convert.ToString(r.Cells[0].Value) == nombre);
                    Assert.AreEqual(0, Convert.ToInt32(fila.Cells[2].Value));
                    Assert.AreEqual(0m, Convert.ToDecimal(fila.Cells[5].Value));
                    Assert.AreEqual("—", Convert.ToString(fila.Cells[4].FormattedValue));
                }
            });
        }

        private sealed class ExportadorMemoria : IExportador
        {
            public DataTable Datos;
            public string Exportar(Reporte reporte, DataTable datos, string ruta) { Datos = datos; return ruta; }
        }
    }

    [TestClass]
    public class PruebasGrupoAReportesVolumen
    {
        private static readonly DateTime Desde = new DateTime(2025, 9, 1), Hasta = new DateTime(2025, 11, 30);

        [ClassInitialize]
        public static void Cargar(TestContext contexto)
        {
            BaseDePrueba.CargarDatosReporte();
            Assert.AreEqual(1179, Fake.ContarDatosReporte(), "Se esperan el local de reporte y sus 1178 usuarios.");
        }

        [ClassCleanup]
        public static void Limpiar()
        {
            Fake.LimpiarDatosReporte();
            Assert.AreEqual(0, Fake.ContarDatosReporte());
            GestorSesion.Instance.Cerrar();
        }

        // CP-87 — RF-11; PA-22, 9.2/9.3: valores iniciales, visibilidad y limpieza de cada filtro.
        [DataTestMethod]
        [DataRow(0, DisplayName = "CP-87: Filtros y limpieza de RPT-01")]
        [DataRow(1, DisplayName = "CP-87: Filtros y limpieza de RPT-02")]
        [DataRow(2, DisplayName = "CP-87: Filtros y limpieza de RPT-03")]
        [DataRow(3, DisplayName = "CP-87: Filtros y limpieza de RPT-04")]
        [DataRow(4, DisplayName = "CP-87: Filtros y limpieza de RPT-05")]
        public void CP87_Filtros(int indice)
        {
            EnSta(() =>
            {
                using (var form = Abrir(0, false))
                {
                    ComboBox combo = ControlDe<ComboBox>(form, "cmbReporte");
                    CollectionAssert.AreEqual(new[] { "RPT-01 · Ventas por evento", "RPT-02 · Ranking de paquetes",
                        "RPT-03 · Usuarios por nivel de fidelidad", "RPT-04 · Adopción de seguros y suscripciones",
                        "RPT-05 · Demanda insatisfecha" }, combo.Items.Cast<object>().Select(x => x.ToString()).ToArray());
                    Assert.AreEqual(0, combo.SelectedIndex);
                    DateTime hoy = DateTime.Today;
                    Assert.AreEqual(new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-2), ControlDe<DateTimePicker>(form, "dtpDesde").Value.Date);
                    Assert.AreEqual(hoy, ControlDe<DateTimePicker>(form, "dtpHasta").Value.Date);
                    Assert.AreEqual("Todas", ControlDe<ComboBox>(form, "cmbFiltro").SelectedItem.ToString());
                    combo.SelectedIndex = indice;
                    bool filtro = indice == 0 || indice == 1 || indice == 4;
                    Assert.AreEqual(filtro, ControlDe<ComboBox>(form, "cmbFiltro").Visible);
                    Assert.AreEqual(indice == 2, ControlDe<DateTimePicker>(form, "dtpCorte").Visible);
                    Assert.AreEqual(indice == 4, ControlDe<CheckBox>(form, "chkSoloAgotados").Visible);
                    if (filtro) Assert.AreEqual(indice == 0 ? "Todas" : "Todos", ControlDe<ComboBox>(form, "cmbFiltro").SelectedItem.ToString());
                    Generar(form);
                    Assert.IsTrue(ControlDe<Button>(form, "btnExportar").Enabled);
                    combo.SelectedIndex = (indice + 1) % 5;
                    Assert.AreEqual(0, ControlDe<DataGridView>(form, "dgvReporte").Rows.Count);
                    Assert.IsFalse(ControlDe<Button>(form, "btnExportar").Enabled);
                    combo.SelectedIndex = indice;
                    Assert.AreEqual(0, ControlDe<DataGridView>(form, "dgvReporte").Rows.Count);
                }
            });
        }

        // CP-88 — RF-11, RNF-02; PA-23, 9.4 a 9.10: datos del anexo y presentación de RPT-01.
        [DataTestMethod]
        [DataRow("ventas", DisplayName = "CP-88: RPT-01 reproduce entradas, ingreso, ticket y ocupación")]
        [DataRow("concierto", DisplayName = "CP-88: RPT-01 filtra Concierto")]
        [DataRow("paquetes", DisplayName = "CP-88: RPT-02 reproduce cinco combinaciones y porcentajes")]
        [DataRow("duki", DisplayName = "CP-88: RPT-02 filtra las 1500 unidades de Duki")]
        [DataRow("niveles", DisplayName = "CP-88: RPT-03 reproduce niveles y corte sin actividad")]
        [DataRow("beneficios", DisplayName = "CP-88: RPT-04 reproduce adopción de seguros y planes")]
        [DataRow("demanda", DisplayName = "CP-88: RPT-05 reproduce demanda y filtro Solo agotados")]
        [DataRow("presentacion", DisplayName = "CP-88: RPT-01 alinea números, resalta TOTAL y muestra la lectura")]
        public void CP88_DatosDelAnexo(string escenario)
        {
            EnSta(() =>
            {
                int indice = escenario == "paquetes" || escenario == "duki" ? 1 : escenario == "niveles" ? 2 :
                    escenario == "beneficios" ? 3 : escenario == "demanda" ? 4 : 0;
                using (var form = Abrir(indice))
                {
                    if (escenario == "concierto") ElegirFiltro(form, "Concierto");
                    if (escenario == "duki") ElegirFiltro(form, "Duki en Rosario ·");
                    DataTable datos = Generar(form);
                    DataRow total = datos.Rows[datos.Rows.Count - 1];
                    switch (escenario)
                    {
                        case "ventas":
                            Assert.AreEqual(4480, Convert.ToInt32(total[2])); Assert.AreEqual(86710000m, Convert.ToDecimal(total[3]));
                            Assert.AreEqual(19355m, Math.Round(Convert.ToDecimal(total[4]), 0));
                            Assert.AreEqual(81.0m, Math.Round(Convert.ToDecimal(total[5]), 1)); break;
                        case "concierto":
                            Assert.AreEqual(2480, Convert.ToInt32(total[2])); Assert.AreEqual(57100000m, Convert.ToDecimal(total[3])); break;
                        case "paquetes":
                            Assert.AreEqual(6, datos.Rows.Count); Assert.AreEqual(113060000m, Convert.ToDecimal(total[3]));
                            Assert.AreEqual(60.0m, Math.Round(Convert.ToDecimal(Fila(datos, "Solo entrada")[4]), 1));
                            Assert.AreEqual(3.1m, Math.Round(Convert.ToDecimal(Fila(datos, "Entrada + Merchandising")[4]), 1)); break;
                        case "duki": Assert.AreEqual(1500, Convert.ToInt32(total[2])); break;
                        case "niveles":
                            CollectionAssert.AreEqual(new[] { 818, 260, 100 }, datos.Rows.Cast<DataRow>().Take(3).Select(r => Convert.ToInt32(r[2])).ToArray());
                            ControlDe<DateTimePicker>(form, "dtpCorte").Value = new DateTime(2025, 8, 31);
                            DataTable sinActividad = Generar(form);
                            foreach (DataRow fila in sinActividad.Rows)
                            {
                                Assert.AreEqual(0, Convert.ToInt32(fila[2])); Assert.AreEqual(0m, Convert.ToDecimal(fila[3]));
                                Assert.AreEqual(DBNull.Value, fila[4]);
                            }
                            break;
                        case "beneficios":
                            CollectionAssert.AreEqual(new[] { 504, 164, 51, 215 }, datos.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r[1])).ToArray());
                            CollectionAssert.AreEqual(new[] { 18.0m, 13.9m, 4.3m, 18.3m }, datos.Rows.Cast<DataRow>().Select(r => Math.Round(Convert.ToDecimal(r[3]), 1)).ToArray()); break;
                        case "demanda":
                            Assert.AreEqual(4, datos.Rows.Count);
                            Assert.IsTrue(datos.Rows.Cast<DataRow>().Take(3).All(r => Convert.ToString(r[1]) == "Con cupo"));
                            Assert.AreEqual(705, Convert.ToInt32(total[2])); Assert.AreEqual(4030, Convert.ToInt32(total[3]));
                            Assert.AreEqual(17.5m, Math.Round(Convert.ToDecimal(total[4]), 1));
                            ControlDe<CheckBox>(form, "chkSoloAgotados").Checked = true;
                            DataTable agotados = Generar(form); Assert.AreEqual(1, agotados.Rows.Count);
                            Assert.AreEqual(0, Convert.ToInt32(agotados.Rows[0][2])); break;
                        case "presentacion":
                            DataGridView grilla = ControlDe<DataGridView>(form, "dgvReporte");
                            foreach (DataGridViewColumn columna in grilla.Columns)
                            {
                                Assert.AreEqual(DataGridViewColumnSortMode.NotSortable, columna.SortMode);
                                if (columna.Index >= 2) Assert.AreEqual(DataGridViewContentAlignment.MiddleRight, columna.DefaultCellStyle.Alignment);
                            }
                            DataGridViewRow filaTotal = grilla.Rows[grilla.Rows.Count - 1];
                            Assert.IsTrue(filaTotal.DefaultCellStyle.Font.Bold);
                            Assert.AreNotEqual(Visual.Tema.Superficie, filaTotal.DefaultCellStyle.BackColor);
                            Label lectura = ControlDe<Label>(form, "lblLectura");
                            Assert.IsTrue(lectura.GetPreferredSize(new Size(lectura.Width, 0)).Height <= lectura.Height, "Lectura del reporte recortada."); break;
                    }
                }
            });
        }

        // CP-89 — RF-11; PA-24, 9.11/9.14: exportación temporal y archivo bloqueado como en Adobe.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-89: Exportar usa la carpeta temporal indicada y ofrece abrir el PDF")]
        [DataRow(true, DisplayName = "CP-89: Archivo bloqueado informa el error y conserva la pantalla")]
        public void CP89_ExportacionTemporal(bool bloqueado)
        {
            EnSta(() =>
            {
                string carpeta = Temporal();
                try
                {
                    using (var defecto = new Visual.frmReportes())
                        Assert.AreEqual(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "A1.Reportes EVENTAURA"), defecto.CarpetaExportacion);
                    using (var form = Abrir(0, true, carpeta))
                    {
                        Generar(form);
                        string ruta = Path.Combine(carpeta, "EVAU_RPT01_20250901_20251130.pdf");
                        if (bloqueado)
                        {
                            using (var archivo = new FileStream(ruta, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                                DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnExportar").PerformClick(),
                                    new DialogoEsperado { Titulo = "Exportar", Texto = "No se pudo escribir el archivo." });
                            Assert.IsTrue(form.Visible); Assert.IsTrue(ControlDe<Button>(form, "btnExportar").Enabled);
                        }
                        else
                        {
                            Exportar(form, ruta);
                            Assert.IsTrue(File.Exists(ruta));
                        }
                    }
                }
                finally { BorrarTemporal(carpeta); }
            });
        }

        // CP-90 — RF-11; PA-25, 9.12 A: texto del PDF exportado desde la pantalla real.
        [TestMethod("CP-90: PDF del reporte contiene marca, código, período, solicitante y paginación")]
        public void CP90_TextoDelPdf()
        {
            EnSta(() =>
            {
                string carpeta = Temporal();
                try
                {
                    using (var form = Abrir(0, true, carpeta))
                    {
                        Generar(form);
                        string ruta = Path.Combine(carpeta, "EVAU_RPT01_20250901_20251130.pdf"); Exportar(form, ruta);
                        using (var lector = new iText.Kernel.Pdf.PdfReader(ruta))
                        using (var pdf = new iText.Kernel.Pdf.PdfDocument(lector))
                        {
                            Assert.AreEqual(1, pdf.GetNumberOfPages());
                            string texto = Regex.Replace(iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdf.GetPage(1)), @"\s+", " ");
                            foreach (string fragmento in new[] { "EventAura", "Ventas por evento", "RPT-01", "01/09/2025", "30/11/2025",
                                "Solicitado por: Ana Administradora (admin@eventaura.com)", "Página 1 de 1" }) StringAssert.Contains(texto, fragmento);
                        }
                    }
                }
                finally { BorrarTemporal(carpeta); }
            });
        }

        private static Visual.frmReportes Abrir(int indice, bool ejemplo = true, string carpeta = null)
        {
            GestorSesion.Instance.Iniciar(Fake.Administrador());
            var form = new Visual.frmReportes();
            if (carpeta != null) form.CarpetaExportacion = carpeta;
            Mostrar(form);
            if (ejemplo)
            {
                ControlDe<DateTimePicker>(form, "dtpDesde").Value = Desde;
                ControlDe<DateTimePicker>(form, "dtpHasta").Value = Hasta;
            }
            ControlDe<ComboBox>(form, "cmbReporte").SelectedIndex = indice;
            return form;
        }
        private static DataTable Generar(Form form)
        {
            DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnGenerar").PerformClick());
            return (DataTable)ControlDe<DataGridView>(form, "dgvReporte").DataSource;
        }
        private static void ElegirFiltro(Form form, string prefijo)
        {
            ComboBox combo = ControlDe<ComboBox>(form, "cmbFiltro");
            combo.SelectedIndex = Enumerable.Range(0, combo.Items.Count).Single(i => combo.Items[i].ToString().StartsWith(prefijo, StringComparison.Ordinal));
        }
        private static DataRow Fila(DataTable tabla, string nombre) => tabla.Rows.Cast<DataRow>().Single(r => Convert.ToString(r[0]) == nombre);
        private static string Temporal()
        {
            string carpeta = Path.Combine(Path.GetTempPath(), "EventAura-reportes-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta); return carpeta;
        }
        private static void BorrarTemporal(string carpeta)
        {
            foreach (string archivo in Directory.GetFiles(carpeta)) File.Delete(archivo);
            Directory.Delete(carpeta);
        }
        private static void Exportar(Form form, string ruta)
        {
            DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnExportar").PerformClick(),
                new DialogoEsperado { Titulo = "Exportar", Texto = ruta, Predeterminado = DialogResult.Yes, Respuesta = DialogResult.No });
        }
    }
}
